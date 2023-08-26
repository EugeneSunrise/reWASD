using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Microsoft.Win32;
using Prism.Commands;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.Controller;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.ViewModels.Preferences
{
	public class InputDevicesVM : ZBindable
	{
		public bool IsSwitchToVID
		{
			get
			{
				return this._isSwitchToVID;
			}
			set
			{
				this.SetProperty<bool>(ref this._isSwitchToVID, value, "IsSwitchToVID");
			}
		}

		public bool IsSwitchPhysical
		{
			get
			{
				return this._isSwitchPhysical;
			}
			set
			{
				this.SetProperty<bool>(ref this._isSwitchPhysical, value, "IsSwitchPhysical");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsHideVID
		{
			get
			{
				return this._isHideVID;
			}
			set
			{
				this.SetProperty<bool>(ref this._isHideVID, value, "IsHideVID");
			}
		}

		public Localizable EmulateText
		{
			get
			{
				if (!this.IsMouseInputDevice)
				{
					return new Localizable(12176);
				}
				return new Localizable(12175);
			}
		}

		public ObservableCollection<reWASDInputDevice> DevicesCollection
		{
			get
			{
				return this._devicesCollection;
			}
			set
			{
				this.SetProperty<ObservableCollection<reWASDInputDevice>>(ref this._devicesCollection, value, "DevicesCollection");
			}
		}

		public reWASDInputDevice CurrentInputDevice
		{
			get
			{
				return this._currentInputDevice;
			}
			set
			{
				if (this.SetProperty<reWASDInputDevice>(ref this._currentInputDevice, value, "CurrentInputDevice"))
				{
					this.SetSwitchDevice();
					this.RemoveCurrentEntryCommand.RaiseCanExecuteChanged();
				}
			}
		}

		private bool IsMouseInputDevice { get; }

		public event InputDevicesVM.OptionChangedDelegate OptionChanged;

		public InputDevicesVM(bool isMouseInputDevice = true)
		{
			this.IsMouseInputDevice = isMouseInputDevice;
			this._isSwitchToVID = true;
		}

		public void Initialize()
		{
			this._isHideVID = !InputDevicesVM.GetVAOption();
			this.FillInputDevicesCollection();
			this.ShowHideVID();
			this.SetCurrentDevice();
			this.SetSwitchDevice();
			TranslationManager.Instance.LanguageChanged += delegate([Nullable(2)] object s, EventArgs e)
			{
				this.OnPropertyChanged("EmulateText");
			};
		}

		private static bool GetVAOption()
		{
			RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32).OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\hidgamemap\\Parameters", false);
			return registryKey != null && registryKey.GetValue("VirtualInputDeviceEnabled", 1).Equals(0);
		}

		public void ShowHideVID()
		{
			reWASDInputDevice reWASDInputDevice = new reWASDInputDevice
			{
				DeviceID = "VirtualID",
				DeviceName = DTLocalization.GetString(12177),
				IsVirtual = true
			};
			this.DevicesCollection.Remove((reWASDInputDevice item) => item.IsVirtual);
			if (InputDevicesVM.GetVAOption())
			{
				this.IsSwitchPhysical = true;
				this.IsHideVID = false;
				return;
			}
			this.DevicesCollection.Insert(0, reWASDInputDevice);
			this.IsHideVID = true;
		}

		public void ApplyChanges()
		{
			RegistryHelper.SetString("InputDevices", this.GetFullRegKey("InputDeviceID"), this.CurrentInputDevice.DeviceID);
			RegistryHelper.SetString("InputDevices", this.GetFullRegKey("InputDeviceName"), this.CurrentInputDevice.DeviceName);
			RegistryHelper.SetValue("InputDevices", this.GetFullRegKey("SwitchToVirtualDevice"), this.IsSwitchToVID);
		}

		private string GetFullRegKey(string key)
		{
			return (this.IsMouseInputDevice ? "Mouse" : "Keyboard") + key;
		}

		private void FillInputDevicesCollection()
		{
			this.DevicesCollection = new ObservableCollection<reWASDInputDevice>
			{
				new reWASDInputDevice
				{
					DeviceID = "VirtualID",
					DeviceName = DTLocalization.GetString(12177),
					IsVirtual = true
				}
			};
			foreach (BaseControllerVM baseControllerVM in App.GamepadService.GamepadCollection)
			{
				if (baseControllerVM.ControllerFamily == 3)
				{
					using (List<BaseControllerVM>.Enumerator enumerator2 = ((CompositeControllerVM)baseControllerVM).NonNullBaseControllers.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							BaseControllerVM baseControllerVM2 = enumerator2.Current;
							this.FillDeviceCollection(baseControllerVM2, baseControllerVM2.ID);
						}
						continue;
					}
				}
				this.FillDeviceCollection(baseControllerVM, "");
			}
		}

		private void FillDeviceCollection(BaseControllerVM gamepad, string compositeID = "")
		{
			if (this.IsMouseInputDevice)
			{
				if (!gamepad.ControllerTypeEnums.Any((ControllerTypeEnum x) => ControllerTypeExtensions.IsMouse(x)))
				{
					return;
				}
			}
			else if (!gamepad.ControllerTypeEnums.Any((ControllerTypeEnum x) => ControllerTypeExtensions.IsKeyboardStandart(x)))
			{
				return;
			}
			string deviceId = (string.IsNullOrEmpty(compositeID) ? gamepad.ID : compositeID);
			if (deviceId.Contains("-"))
			{
				deviceId = gamepad.Ids.Where((ulong gamepadId) => gamepadId > 0UL).Aggregate("", (string current, ulong gamepadId) => current + gamepadId.ToString() + ";");
				deviceId = deviceId.TrimEnd(';');
			}
			if (this.DevicesCollection.All((reWASDInputDevice x) => x.DeviceID != deviceId))
			{
				this.DevicesCollection.Add(new reWASDInputDevice
				{
					DeviceID = deviceId,
					DeviceName = gamepad.ControllerDisplayName
				});
			}
		}

		private void SetCurrentDevice()
		{
			string deviceID = RegistryHelper.GetString("InputDevices", this.GetFullRegKey("InputDeviceID"), "", false);
			string @string = RegistryHelper.GetString("InputDevices", this.GetFullRegKey("InputDeviceName"), "", false);
			this.CurrentInputDevice = this.DevicesCollection.FirstOrDefault((reWASDInputDevice item) => item.DeviceID == deviceID);
			if (this.CurrentInputDevice != null)
			{
				return;
			}
			int num = 0;
			if (!string.IsNullOrEmpty(deviceID) && !string.IsNullOrEmpty(@string))
			{
				this.DevicesCollection.Add(new reWASDInputDevice
				{
					DeviceID = deviceID,
					DeviceName = @string
				});
				num = this.DevicesCollection.Count - 1;
			}
			this.CurrentInputDevice = this.DevicesCollection[num];
		}

		private void SetSwitchDevice()
		{
			string deviceID = RegistryHelper.GetString("InputDevices", this.GetFullRegKey("InputDeviceID"), "", false);
			reWASDInputDevice reWASDInputDevice = this.DevicesCollection.FirstOrDefault((reWASDInputDevice item) => item.DeviceID == deviceID);
			if (this.CurrentInputDevice == reWASDInputDevice)
			{
				this.IsSwitchToVID = RegistryHelper.GetBool("InputDevices", this.GetFullRegKey("SwitchToVirtualDevice"), true);
			}
			reWASDInputDevice currentInputDevice = this.CurrentInputDevice;
			if (currentInputDevice != null && currentInputDevice.IsVirtual)
			{
				this.IsSwitchToVID = true;
			}
			this.IsSwitchPhysical = !this.IsSwitchToVID;
		}

		public override void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (!this.HasAttributeDefined(propertyName, typeof(DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent)))
			{
				InputDevicesVM.OptionChangedDelegate optionChanged = this.OptionChanged;
				if (optionChanged != null)
				{
					optionChanged();
				}
			}
			base.OnPropertyChanged(propertyName);
		}

		public DelegateCommand RemoveCurrentEntryCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._removeCurrentEntry) == null)
				{
					delegateCommand = (this._removeCurrentEntry = new DelegateCommand(new Action(this.RemoveCurrentEntry), new Func<bool>(this.RemoveCurrentEntryCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void RemoveCurrentEntry()
		{
			if (DTMessageBox.Show(DTLocalization.GetString(12183), MessageBoxButton.YesNo) != MessageBoxResult.Yes)
			{
				return;
			}
			int num = this._devicesCollection.IndexOf(this.CurrentInputDevice);
			this._devicesCollection.Remove((reWASDInputDevice g) => g.DeviceID == this.CurrentInputDevice.DeviceID && g.DeviceName == this.CurrentInputDevice.DeviceName);
			if (num >= this._devicesCollection.Count)
			{
				num--;
			}
			this.CurrentInputDevice = this.DevicesCollection[num];
		}

		private bool RemoveCurrentEntryCanExecute()
		{
			if (this.CurrentInputDevice == null || this.CurrentInputDevice.IsVirtual)
			{
				return false;
			}
			foreach (BaseControllerVM baseControllerVM in App.GamepadService.GamepadCollection)
			{
				if (baseControllerVM.ControllerFamily == 3)
				{
					if (((CompositeControllerVM)baseControllerVM).NonNullBaseControllers.Any((BaseControllerVM compGamepad) => compGamepad.ID == this.CurrentInputDevice.DeviceID))
					{
						return false;
					}
				}
				else
				{
					string text = baseControllerVM.ID;
					if (text.Contains("-"))
					{
						text = baseControllerVM.Ids.Where((ulong gamepadId) => gamepadId > 0UL).Aggregate("", (string current, ulong gamepadId) => current + gamepadId.ToString() + ";");
						text = text.TrimEnd(';');
					}
					if (text == this.CurrentInputDevice.DeviceID)
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool _isSwitchToVID;

		private bool _isSwitchPhysical;

		private bool _isHideVID;

		private reWASDInputDevice _currentInputDevice;

		private ObservableCollection<reWASDInputDevice> _devicesCollection;

		private DelegateCommand _removeCurrentEntry;

		public delegate void OptionChangedDelegate();
	}
}
