using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using Prism.Commands;
using Prism.Ioc;
using reWASDCommon;
using reWASDCommon.Utils;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class BluetoothSettingsVM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.BluetoothSettings;
			}
		}

		public bool IsNeedReboot
		{
			get
			{
				return this._isNeedReboot;
			}
			set
			{
				if (this._isNeedReboot != value)
				{
					this._isNeedReboot = value;
					this.OnPropertyChanged("IsNeedReboot");
				}
			}
		}

		public bool IsNeedChangeAdapterType
		{
			get
			{
				return this._isNeedChangeAdapterType;
			}
			set
			{
				if (this._isNeedChangeAdapterType != value)
				{
					this._isNeedChangeAdapterType = value;
					this.OnPropertyChanged("IsNeedChangeAdapterType");
				}
			}
		}

		public bool IsSecureSimpleParingIsNotPresent
		{
			get
			{
				return this._isSecureSimpleParingIsNotPresent;
			}
			set
			{
				if (this._isSecureSimpleParingIsNotPresent != value)
				{
					this._isSecureSimpleParingIsNotPresent = value;
					this.OnPropertyChanged("IsSecureSimpleParingIsNotPresent");
				}
			}
		}

		public override void OnShowPage()
		{
			this.CanAddBluetooth = (BluetoothUtils.IsBluetoothExist() && !App.GamepadService.ExternalDevices.IsBluetoothExist) || BluetoothUtils.IsRebootRequired();
			if (!this.CanAddBluetooth)
			{
				this.StartBTPoller();
			}
		}

		public override void OnHidePage()
		{
			this.StopBTPoller();
		}

		public BluetoothSettingsVM(WizardVM wizard)
			: base(wizard)
		{
			this.ExternalDeviceCollection = new ExternalDevicesCollection(App.GamepadService.ExternalDevices);
			this.CheckAdapterType();
		}

		private void CheckAdapterType()
		{
			this.IsNeedChangeAdapterType = BluetoothUtils.NeedInitForRewasd();
			this.IsNeedReboot = BluetoothUtils.IsRebootRequired();
			this.IsSecureSimpleParingIsNotPresent = BluetoothUtils.IsLmpSecureSimpleParingIsNotPresent();
		}

		private void StartBTPoller()
		{
			this._pollingTimer = new DispatcherTimer();
			this._pollingTimer.Tick += delegate([Nullable(2)] object o, EventArgs e)
			{
				this.CheckBluetoothAdapter();
			};
			this._pollingTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
			this._pollingTimer.Start();
		}

		public bool CanAddBluetooth
		{
			get
			{
				return this._canAddBluetooth;
			}
			set
			{
				if (this._canAddBluetooth == value)
				{
					return;
				}
				this._canAddBluetooth = value;
				this.OnPropertyChanged("CanAddBluetooth");
			}
		}

		private void CheckBluetoothAdapter()
		{
			if (BluetoothUtils.IsBluetoothExist() && !App.GamepadService.ExternalDevices.IsBluetoothExist)
			{
				this.CanAddBluetooth = true;
				this.StopBTPoller();
			}
		}

		private void StopBTPoller()
		{
			DispatcherTimer pollingTimer = this._pollingTimer;
			if (pollingTimer == null)
			{
				return;
			}
			pollingTimer.Stop();
		}

		private async void ChangeAdapterType()
		{
			base.IsProcessing = true;
			IAdminOperations adminOperations = IContainerProviderExtensions.Resolve<IAdminOperations>(App.Container);
			if (adminOperations != null)
			{
				int num = await adminOperations.InitBluetoothSettingsForRewasd();
				this.IsNeedReboot = num == 1;
			}
			if (BluetoothUtils.IsBluetoothLocalRadioInfoInvalidCod(true))
			{
				this.IsNeedReboot = true;
			}
			base.IsProcessing = false;
			this.CheckAdapterType();
		}

		protected override void NavigateToNextPage()
		{
			if (this.IsNeedChangeAdapterType)
			{
				this.ChangeAdapterType();
			}
		}

		protected override void NavigatePreviousPage()
		{
			base.GoPage(PageType.DeviceType);
		}

		public ExternalDevicesCollection ExternalDeviceCollection
		{
			get
			{
				return this._externalDeviceCollection;
			}
			set
			{
				if (this._externalDeviceCollection == value)
				{
					return;
				}
				this.SetProperty(ref this._externalDeviceCollection, value, "ExternalDeviceCollection");
			}
		}

		public DelegateCommand FinishCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._finishCommand) == null)
				{
					delegateCommand = (this._finishCommand = new DelegateCommand(new Action(this.ReturnToAdapterSettings)));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand RebootLaterCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._rebootLaterCommand) == null)
				{
					delegateCommand = (this._rebootLaterCommand = new DelegateCommand(new Action(this.ReturnToAdapterSettings)));
				}
				return delegateCommand;
			}
		}

		private void ReturnToAdapterSettings()
		{
			this.SaveExternalDevices();
			base.GoPage(PageType.AdaptersSettings);
		}

		public DelegateCommand RebootNowCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._rebootNowCommand) == null)
				{
					delegateCommand = (this._rebootNowCommand = new DelegateCommand(delegate
					{
						DSUtils.RebootNow();
					}));
				}
				return delegateCommand;
			}
		}

		private async void SaveExternalDevices()
		{
			if (!this.ExternalDeviceCollection.IsBluetoothExist)
			{
				this.ExternalDeviceCollection.Add(this._wizard.Result.Clone());
				ObservableCollection<ExternalDevice> observableCollection = new ObservableCollection<ExternalDevice>(this.ExternalDeviceCollection.Where((ExternalDevice x) => !x.IsDummy).ToList<ExternalDevice>());
				App.GamepadService.ExternalDevices = new ExternalDevicesCollection(observableCollection);
				await App.GamepadService.BinDataSerialize.SaveExternalDevices();
				App.GamepadService.ExternalDeviceRelationsHelper.Refresh();
			}
		}

		private DispatcherTimer _pollingTimer;

		private bool _isNeedReboot;

		private bool _isNeedChangeAdapterType = true;

		private bool _isSecureSimpleParingIsNotPresent = true;

		private bool _canAddBluetooth;

		private ExternalDevicesCollection _externalDeviceCollection;

		private DelegateCommand _finishCommand;

		private DelegateCommand _rebootLaterCommand;

		private DelegateCommand _rebootNowCommand;
	}
}
