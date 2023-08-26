using System;
using System.Threading.Tasks;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Microsoft.Win32;
using Prism.Ioc;
using reWASDCommon;
using reWASDUI.ViewModels.Preferences.Base;
using XBEliteWPF.Utils;

namespace reWASDUI.ViewModels.Preferences
{
	public class PreferencesInputDevicesVM : PreferencesBaseVM
	{
		public InputDevicesVM MouseInputDevice
		{
			get
			{
				return this._mouseInputDevices;
			}
		}

		public InputDevicesVM KeyboardInputDevice
		{
			get
			{
				return this._keyboardInputDevices;
			}
		}

		public bool UsePhysicalUSBHub
		{
			get
			{
				return this._usePhysicalUSBHub;
			}
			set
			{
				if (value == this._usePhysicalUSBHub)
				{
					return;
				}
				this._usePhysicalUSBHub = value;
				this.OnPropertyChanged("UsePhysicalUSBHub");
			}
		}

		public PreferencesInputDevicesVM()
		{
			this._mouseInputDevices = new InputDevicesVM(true);
			this._keyboardInputDevices = new InputDevicesVM(false);
			this._keyboardInputDevices.OptionChanged += this.AnyOptionChanged;
			this._mouseInputDevices.OptionChanged += this.AnyOptionChanged;
		}

		public override Task Initialize()
		{
			this._keyboardInputDevices.Initialize();
			this._mouseInputDevices.Initialize();
			this._dontCreateVID = this.GetVAOption();
			this.UsePhysicalUSBHub = base.UserSettingsService.UsePhysicalUSBHubOption;
			this.SetDescription();
			return Task.CompletedTask;
		}

		public void AnyOptionChanged()
		{
			this.InputDeviceIsChangedToVirtual();
			base.FireOptionChanged();
		}

		public override async Task<bool> ApplyChanges()
		{
			this._mouseInputDevices.ApplyChanges();
			this._keyboardInputDevices.ApplyChanges();
			bool askForReboot = false;
			if (base.UserSettingsService.UsePhysicalUSBHubOption != this.UsePhysicalUSBHub)
			{
				await App.AdminOperations.SetUsePhysicalUSBHub(this.UsePhysicalUSBHub);
				bool usePhysicalUSBHubOption = base.UserSettingsService.UsePhysicalUSBHubOption;
				if (usePhysicalUSBHubOption == this.UsePhysicalUSBHub)
				{
					askForReboot = true;
				}
				this.UsePhysicalUSBHub = usePhysicalUSBHubOption;
			}
			bool flag = askForReboot;
			askForReboot = flag | await this.SwitchVirtualAdapter(this.DontCreateVID);
			if (askForReboot && DTMessageBox.Show(DTLocalization.GetString(11483), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				DSUtils.RebootNow();
			}
			base.FireRequiredInputDeviceRefresh();
			return true;
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool DontCreateVID
		{
			get
			{
				return this._dontCreateVID;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._dontCreateVID, value, "DontCreateVID"))
				{
					if (value && DTMessageBox.Show(DTLocalization.GetString(12181), MessageBoxButton.YesNo) == MessageBoxResult.No)
					{
						this._dontCreateVID = false;
						return;
					}
					base.FireOptionChanged();
				}
			}
		}

		private bool InputDeviceIsChangedToVirtual()
		{
			if (this._keyboardInputDevices.CurrentInputDevice != null && this._mouseInputDevices.CurrentInputDevice != null && (this._keyboardInputDevices.CurrentInputDevice.IsVirtual || this._mouseInputDevices.CurrentInputDevice.IsVirtual))
			{
				this.DontCreateVID = false;
			}
			return !this.DontCreateVID;
		}

		public bool GetVAOption()
		{
			string text = "SYSTEM\\CurrentControlSet\\Services\\hidgamemap\\Parameters";
			string text2 = "VirtualInputDeviceEnabled";
			return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32).OpenSubKey(text, false).GetValue(text2, 1)
				.Equals(0);
		}

		private async Task<bool> SwitchVirtualAdapter(bool dontCreateVID)
		{
			if (this.DontCreateVID != this.GetVAOption())
			{
				bool flag = await ((AdminOperationsDecider)IContainerProviderExtensions.Resolve<IAdminOperations>(App.Container)).SwitchVirtualAdapter(dontCreateVID);
				this.MouseInputDevice.ShowHideVID();
				this.KeyboardInputDevice.ShowHideVID();
				if (flag)
				{
					await this.ApplyChanges();
					return true;
				}
				if (!this.InputDeviceIsChangedToVirtual())
				{
					this.DontCreateVID = !dontCreateVID;
					return false;
				}
			}
			return false;
		}

		private void SetDescription()
		{
			base.Description = new Localizable();
		}

		private InputDevicesVM _mouseInputDevices;

		private InputDevicesVM _keyboardInputDevices;

		private bool _usePhysicalUSBHub;

		private bool _dontCreateVID;
	}
}
