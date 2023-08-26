using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Prism.Commands;
using reWASDUI.Infrastructure.KeyBindings;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.DataModels.HelperModels
{
	public class GameDataVM : ZBindable
	{
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
			this.ApplicationNamesCollection.Remove(applicationName);
			if (this.ApplicationNamesCollection.Count == 0)
			{
				App.GameProfilesService.CurrentGame.IsAutodetect = false;
			}
		}

		public void SetOldValues()
		{
			this._nameOld = this.Name;
			this._imageSourcePathOld = this.ImageSourcePath;
			this._applicationNamesCollectionOld = new ObservableCollection<string>(this.ApplicationNamesCollection);
		}

		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (this.SetProperty<string>(ref this._name, value, "Name"))
				{
					this.OnPropertyChanged("IsSaveEnabled");
				}
			}
		}

		public string ImageSourcePath
		{
			get
			{
				return this._imageSourcePath;
			}
			set
			{
				if (this.SetProperty<string>(ref this._imageSourcePath, value, "ImageSourcePath"))
				{
					this.OnPropertyChanged("IsSaveEnabled");
				}
			}
		}

		public bool IsGameHasMoreThanOneConfig { get; set; }

		public bool IsGameHasAnyConfig { get; set; }

		public ObservableCollection<string> ApplicationNamesCollection
		{
			get
			{
				return this._applicationNamesCollection;
			}
			set
			{
				if (this.SetProperty<ObservableCollection<string>>(ref this._applicationNamesCollection, value, "ApplicationNamesCollection"))
				{
					if (this._applicationNamesCollection != null)
					{
						this._applicationNamesCollection.CollectionChanged += delegate([Nullable(2)] object s, NotifyCollectionChangedEventArgs e)
						{
							this.OnPropertyChanged("IsSaveEnabled");
						};
					}
					this.OnPropertyChanged("IsSaveEnabled");
				}
			}
		}

		public void AddAutodetectProcess(string name)
		{
			if (this.ApplicationNamesCollection.Contains(name))
			{
				DTMessageBox.Show(DTLocalization.GetString(11456), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
				return;
			}
			ConfigVM matchedConfig = null;
			Func<SubConfigData, bool> <>9__2;
			Func<ConfigVM, bool> <>9__1;
			if (App.GameProfilesService.GamesCollection.Any(delegate(GameVM vm)
			{
				if (vm.Name != this.Name)
				{
					IEnumerable<ConfigVM> configCollection = vm.ConfigCollection;
					Func<ConfigVM, bool> func;
					if ((func = <>9__1) == null)
					{
						func = (<>9__1 = delegate(ConfigVM configVM)
						{
							if (configVM.CurrentBindingCollection == null)
							{
								return false;
							}
							matchedConfig = configVM;
							IEnumerable<SubConfigData> configData = configVM.ConfigData;
							Func<SubConfigData, bool> func2;
							if ((func2 = <>9__2) == null)
							{
								func2 = (<>9__2 = (SubConfigData bc) => bc.MainXBBindingCollection.ProcessNames.Contains(name));
							}
							return configData.Any(func2);
						});
					}
					return configCollection.Any(func);
				}
				return false;
			}))
			{
				DTMessageBox.Show(string.Format(DTLocalization.GetString(12205), matchedConfig.ParentGame.Name), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
				return;
			}
			this.ApplicationNamesCollection.Add(name);
		}

		public bool IsSaveEnabled
		{
			get
			{
				return this.Name == null || (!string.IsNullOrEmpty(this.Name.Trim()) && (this._nameOld != this.Name || this._imageSourcePathOld != this._imageSourcePath || !this._applicationNamesCollection.SequenceEqual(this._applicationNamesCollectionOld)));
			}
		}

		private DelegateCommand<string> _removeApplicationName;

		private string _name;

		private string _imageSourcePath;

		private ObservableCollection<string> _applicationNamesCollection = new ObservableCollection<string>();

		private string _nameOld;

		private string _imageSourcePathOld;

		private ObservableCollection<string> _applicationNamesCollectionOld = new ObservableCollection<string>();
	}
}
