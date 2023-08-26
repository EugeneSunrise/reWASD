using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils.Attributes;
using Prism.Commands;
using reWASDUI.ViewModels.Preferences.Base;
using XBEliteWPF.Services;

namespace reWASDUI.ViewModels.Preferences
{
	public class PreferencesBlacklistVM : PreferencesBaseVM
	{
		public DelegateCommand<BlackListGamepad> RemoveBlacklistDeviceCommand
		{
			get
			{
				DelegateCommand<BlackListGamepad> delegateCommand;
				if ((delegateCommand = this._removeBlacklistDevice) == null)
				{
					delegateCommand = (this._removeBlacklistDevice = new DelegateCommand<BlackListGamepad>(new Action<BlackListGamepad>(this.RemoveBlacklistDevice), (BlackListGamepad device) => device != null));
				}
				return delegateCommand;
			}
		}

		private void RemoveBlacklistDevice(BlackListGamepad deviceName)
		{
			if (deviceName == null)
			{
				return;
			}
			this.BlacklistDevicesCollection.Remove(deviceName);
			this.OnPropertyChanged("BlacklistDevicesCollection");
			this.Save();
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public ObservableCollection<BlackListGamepad> BlacklistDevicesCollection
		{
			get
			{
				return this._blacklistDevicesCollection;
			}
			set
			{
				this.SetProperty<ObservableCollection<BlackListGamepad>>(ref this._blacklistDevicesCollection, value, "BlacklistDevicesCollection");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public BlackListGamepad SelectedBlackListGamepad
		{
			get
			{
				return this._selectedBlackListGamepad;
			}
			set
			{
				if (this.SetProperty<BlackListGamepad>(ref this._selectedBlackListGamepad, value, "SelectedBlackListGamepad"))
				{
					this.RemoveBlacklistDeviceCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public override Task Initialize()
		{
			this.BlacklistDevicesCollection = new ObservableCollection<BlackListGamepad>(App.GamepadService.BlacklistGamepads);
			this._blacklistDevicesCollection.CollectionChanged += delegate([Nullable(2)] object s, NotifyCollectionChangedEventArgs e)
			{
				this.SetDescription();
			};
			this.SetDescription();
			return Task.CompletedTask;
		}

		private async void Save()
		{
			App.GamepadService.BlacklistGamepads = new ObservableCollection<BlackListGamepad>(this.BlacklistDevicesCollection);
			await App.GamepadService.BinDataSerialize.SaveBlacklistDevices();
		}

		public override Task<bool> ApplyChanges()
		{
			return Task.FromResult<bool>(true);
		}

		private void SetDescription()
		{
			base.Description = ((this.BlacklistDevicesCollection.Count == 0) ? new Localizable(11377) : new Localizable());
		}

		private DelegateCommand<BlackListGamepad> _removeBlacklistDevice;

		private ObservableCollection<BlackListGamepad> _blacklistDevicesCollection = new ObservableCollection<BlackListGamepad>();

		private BlackListGamepad _selectedBlackListGamepad;
	}
}
