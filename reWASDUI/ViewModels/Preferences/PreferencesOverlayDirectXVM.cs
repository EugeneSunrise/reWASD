using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Prism.Commands;
using reWASDUI.ViewModels.Preferences.Base;

namespace reWASDUI.ViewModels.Preferences
{
	public class PreferencesOverlayDirectXVM : PreferencesBaseVM
	{
		public PreferencesOverlayDirectXVM(PreferencesOverlayVM parent)
		{
			this.ParentVM = parent;
		}

		public override Task Initialize()
		{
			this.ShowDirectXOverlay = App.UserSettingsService.ShowDirectXOverlay;
			this.ReadBlackListFile();
			string directX_Apps = App.UserSettingsService.DirectX_Apps;
			this.ApplicationNamesCollection.Clear();
			foreach (string text in directX_Apps.Split(';', StringSplitOptions.None))
			{
				if (text.Length > 0)
				{
					this.AddToAppsList(text);
				}
			}
			return Task.CompletedTask;
		}

		public override void Refresh()
		{
			this.ReadBlackListFile();
		}

		private void ReadBlackListFile()
		{
			string text = this.CheckAndCreateDirectoryForOverlayData() + "\\BlackListDirectXAPP";
			if (File.Exists(text))
			{
				this.BlackList = File.ReadAllLines(text).ToList<string>();
			}
		}

		private string CheckAndCreateDirectoryForOverlayData()
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Disc-Soft", "reWASD");
			text += "\\OverlayData";
			try
			{
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
			}
			catch (Exception)
			{
				Tracer.TraceWrite("OverlayManager CheckAndCreateDirectoryForOverlayData failed. Overlay in DirectX wil be not available.", false);
			}
			return text;
		}

		private bool CheckBlackList(string nameExe)
		{
			return this.BlackList.FirstOrDefault((string stringToCheck) => stringToCheck.Contains(nameExe)) != null;
		}

		public string GetApplicationsString()
		{
			string text = "";
			foreach (ApplicationsInfo applicationsInfo in this.ApplicationNamesCollection)
			{
				text += applicationsInfo.NameExe;
				text += ";";
			}
			return text;
		}

		public override Task<bool> ApplyChanges()
		{
			App.UserSettingsService.ShowDirectXOverlay = this.ShowDirectXOverlay;
			string applicationsString = this.GetApplicationsString();
			App.UserSettingsService.DirectX_Apps = applicationsString;
			return Task.FromResult<bool>(true);
		}

		public void ShowOverlayChanged()
		{
		}

		public PreferencesOverlayVM ParentVM { get; set; }

		public bool ShowDirectXOverlay
		{
			get
			{
				return this._showDirectXOverlay;
			}
			set
			{
				if (value == this._showDirectXOverlay)
				{
					return;
				}
				this._showDirectXOverlay = value;
				this.OnPropertyChanged("ShowDirectXOverlay");
				this.OnPropertyChanged("IsAddControllEnable");
			}
		}

		public ObservableCollection<ApplicationsInfo> ApplicationNamesCollection
		{
			get
			{
				ObservableCollection<ApplicationsInfo> observableCollection;
				if ((observableCollection = this._messages) == null)
				{
					observableCollection = (this._messages = new ObservableCollection<ApplicationsInfo>());
				}
				return observableCollection;
			}
		}

		private bool IsAppListContain(string appName)
		{
			bool flag = false;
			using (IEnumerator<ApplicationsInfo> enumerator = this.ApplicationNamesCollection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.NameExe == appName)
					{
						flag = true;
						break;
					}
				}
			}
			return flag;
		}

		private void AddToAppsList(string appName)
		{
			ApplicationsInfo applicationsInfo = new ApplicationsInfo();
			applicationsInfo.NameExe = appName;
			applicationsInfo.RemoveApplicationEvent += this.RemoveApplicationName;
			applicationsInfo.BlackList = this.CheckBlackList(appName);
			if (applicationsInfo.BlackList)
			{
				applicationsInfo.ToolTipText = DTLocalization.GetString(12340);
			}
			else
			{
				applicationsInfo.ToolTipText = null;
			}
			this.ApplicationNamesCollection.Add(applicationsInfo);
		}

		public void AddAutodetectProcess(string name)
		{
			if (this.IsAppListContain(name))
			{
				DTMessageBox.Show(DTLocalization.GetString(11456), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
				return;
			}
			if (this.CheckBlackList(name))
			{
				DTMessageBox.Show(DTLocalization.GetString(12340), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
				return;
			}
			this.AddToAppsList(name);
			base.FireOptionChanged();
		}

		public DelegateCommand<string> RemoveApplicationNameCommand
		{
			get
			{
				DelegateCommand<string> delegateCommand;
				if ((delegateCommand = this._removeApplicationName) == null)
				{
					delegateCommand = (this._removeApplicationName = new DelegateCommand<string>(new Action<string>(this.RemoveApplicationName)));
				}
				return delegateCommand;
			}
		}

		public void RemoveApplicationName(string applicationName)
		{
			if (applicationName == null)
			{
				return;
			}
			foreach (ApplicationsInfo applicationsInfo in this.ApplicationNamesCollection)
			{
				if (applicationsInfo.NameExe == applicationName)
				{
					this.ApplicationNamesCollection.Remove(applicationsInfo);
					break;
				}
			}
			base.FireOptionChanged();
		}

		public bool IsAddControllEnable
		{
			get
			{
				return this._showDirectXOverlay && this.ParentVM.ShowOverlay;
			}
		}

		private List<string> BlackList = new List<string>();

		private bool _showDirectXOverlay;

		private ObservableCollection<ApplicationsInfo> _messages;

		private DelegateCommand<string> _removeApplicationName;
	}
}
