using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.DataModels;

namespace reWASDUI.ViewModels.SecondaryWindows
{
	public class AddImportConfigViewModel : ZBindableBase
	{
		public ConfigVM ConfigVM
		{
			get
			{
				return this._configVM;
			}
			set
			{
				this.SetProperty<ConfigVM>(ref this._configVM, value, "ConfigVM");
				this.ConfigVM.PropertyChanged += this.ProfileVMOnPropertyChanged;
			}
		}

		public ConfigType ConfigType
		{
			get
			{
				return this._configType;
			}
			set
			{
				this.SetProperty<ConfigType>(ref this._configType, value, "ConfigType");
				this.OnPropertyChanged("IsImportAllowed");
			}
		}

		private void ProfileVMOnPropertyChanged(object s, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Name")
			{
				this.OnPropertyChanged("IsImportAllowed");
			}
		}

		public ICommand OpenCommunityCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._openCommunity) == null)
				{
					relayCommand = (this._openCommunity = new RelayCommand(new Action(this.OpenCommunity)));
				}
				return relayCommand;
			}
		}

		private void OpenCommunity()
		{
			App.OpenCommunityLink();
		}

		public ICommand ImportConfigCommand
		{
			get
			{
				RelayCommand<string> relayCommand;
				if ((relayCommand = this._importConfig) == null)
				{
					relayCommand = (this._importConfig = new RelayCommand<string>(new Action<string>(this.ImportConfig)));
				}
				return relayCommand;
			}
		}

		private void ImportConfig(string filePath)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = DTLocalization.GetString(11027) + " (*.rewasd) | *.rewasd";
			openFileDialog.CheckFileExists = true;
			openFileDialog.Multiselect = false;
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				this.ConfigVM.ConfigPath = openFileDialog.FileName;
				this.ConfigVM.Name = DSUtils.GetFileNameWithoutExtension(openFileDialog.FileName);
				this.OnPropertyChanged("IsImportAllowed");
			}
		}

		public bool IsImportAllowed
		{
			get
			{
				return !string.IsNullOrEmpty(this.ConfigVM.Name.Trim()) && ((!string.IsNullOrEmpty(this.ConfigVM.ConfigPath.Trim()) && this.ConfigType == 1) || this.ConfigType == 0);
			}
		}

		private ConfigVM _configVM;

		private ConfigType _configType;

		private RelayCommand _openCommunity;

		private RelayCommand<string> _importConfig;
	}
}
