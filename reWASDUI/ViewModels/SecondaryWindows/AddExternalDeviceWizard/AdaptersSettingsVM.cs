using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Properties;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Prism.Commands;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Utils;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Services;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.ExternalDeviceRelations;
using XBEliteWPF.Infrastructure.ExternalDevices;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class AdaptersSettingsVM : BasePageVM, INotifyPropertyChanged
	{
		public AdaptersSettingsVM(WizardVM wizard)
			: base(wizard)
		{
			this.GamepadService = (GamepadService)App.GamepadService;
			this.GameProfilesService = (GameProfilesService)App.GameProfilesService;
			this.LoadOverwriteProperty();
			this.ReInit(true);
			this.GamepadService.ExternalDeviceRelationsHelper.CurrentGamepadsAuthCollection.CollectionChanged += this.CurrentGamepadsAuthCollectionOnCollectionChanged;
			ExternalDeviceRelation relationForCurrentGamepadAndConfig = this.GamepadService.ExternalDeviceRelationsHelper.GetRelationForCurrentGamepadAndConfig();
			if (relationForCurrentGamepadAndConfig != null)
			{
				if (relationForCurrentGamepadAndConfig.ExternalDevice != null && !string.IsNullOrEmpty(relationForCurrentGamepadAndConfig.ExternalDeviceId))
				{
					int num = 0;
					using (IEnumerator<ExternalDevice> enumerator = this.ExternalDeviceCollection.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.ExternalDeviceId == relationForCurrentGamepadAndConfig.ExternalDevice.ExternalDeviceId)
							{
								this.ExternalDeviceSelectedIndex = num;
								break;
							}
							num++;
						}
					}
				}
				if (relationForCurrentGamepadAndConfig.ExternalClient != null && relationForCurrentGamepadAndConfig.ExternalClient.MacAddress != 0UL)
				{
					int num2 = 0;
					using (IEnumerator<ExternalClient> enumerator2 = this.ExternalClientsCollection.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.MacAddress == relationForCurrentGamepadAndConfig.ExternalClient.MacAddress)
							{
								this.ExternalClientSelectedIndex = num2;
								if (this.ExternalClientsCollection[this.ExternalClientSelectedIndex].IsConsoleAuthRequired)
								{
									this.UseAuthGamepad = true;
									break;
								}
								break;
							}
							else
							{
								num2++;
							}
						}
					}
				}
				if (this.IsExternalDeviceSelected() && (this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex].DeviceType == 1 || this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex].DeviceType == 3))
				{
					GameProfilesService gameProfilesService = this.GameProfilesService;
					bool flag;
					if (gameProfilesService == null)
					{
						flag = false;
					}
					else
					{
						GameVM currentGame = gameProfilesService.CurrentGame;
						VirtualGamepadType? virtualGamepadType;
						if (currentGame == null)
						{
							virtualGamepadType = null;
						}
						else
						{
							ConfigVM currentConfig = currentGame.CurrentConfig;
							if (currentConfig == null)
							{
								virtualGamepadType = null;
							}
							else
							{
								ConfigData configData = currentConfig.ConfigData;
								virtualGamepadType = ((configData != null) ? new VirtualGamepadType?(configData.VirtualGamepadType) : null);
							}
						}
						VirtualGamepadType? virtualGamepadType2 = virtualGamepadType;
						VirtualGamepadType virtualGamepadType3 = 1;
						flag = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
					}
					if (flag)
					{
						this.UseAuthGamepad = true;
					}
				}
				if (relationForCurrentGamepadAndConfig.AuthGamepad == null || string.IsNullOrEmpty(relationForCurrentGamepadAndConfig.AuthGamepad.ID))
				{
					goto IL_3AE;
				}
				int num3 = 0;
				this.UseAuthGamepad = true;
				using (IEnumerator<GamepadAuth> enumerator3 = this.GamepadService.ExternalDeviceRelationsHelper.CurrentGamepadsAuthCollection.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						if (enumerator3.Current.ID == relationForCurrentGamepadAndConfig.AuthGamepad.ID)
						{
							this.GamepadAuthSelectedIndex = num3;
							break;
						}
						num3++;
					}
					goto IL_3AE;
				}
			}
			if (this.IsExternalDeviceSelected() && (this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex].DeviceType == null || this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex].DeviceType == 2) && this.IsExternalClientSelected() && this.ExternalClientsCollection[this.ExternalClientSelectedIndex].IsConsoleAuthRequired)
			{
				this.UseAuthGamepad = true;
			}
			if (this.IsExternalDeviceSelected() && (this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex].DeviceType == 1 || this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex].DeviceType == 3))
			{
				GameProfilesService gameProfilesService2 = this.GameProfilesService;
				bool flag2;
				if (gameProfilesService2 == null)
				{
					flag2 = false;
				}
				else
				{
					GameVM currentGame2 = gameProfilesService2.CurrentGame;
					VirtualGamepadType? virtualGamepadType4;
					if (currentGame2 == null)
					{
						virtualGamepadType4 = null;
					}
					else
					{
						ConfigVM currentConfig2 = currentGame2.CurrentConfig;
						if (currentConfig2 == null)
						{
							virtualGamepadType4 = null;
						}
						else
						{
							ConfigData configData2 = currentConfig2.ConfigData;
							virtualGamepadType4 = ((configData2 != null) ? new VirtualGamepadType?(configData2.VirtualGamepadType) : null);
						}
					}
					VirtualGamepadType? virtualGamepadType2 = virtualGamepadType4;
					VirtualGamepadType virtualGamepadType3 = 1;
					flag2 = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
				}
				if (flag2)
				{
					this.UseAuthGamepad = true;
				}
			}
			IL_3AE:
			if (this.IsExternalClientSelected() && !this.ExternalClientsCollection[this.ExternalClientSelectedIndex].IsDummy)
			{
				this.GamepadService.ExternalDeviceRelationsHelper.RecreateCurrentGamepadsAuthList(this.ExternalClientsCollection[this.ExternalClientSelectedIndex]);
			}
			this.GetExternalDeviceState();
			App.EventAggregator.GetEvent<ExternalHelperChanged>().Subscribe(delegate(object sender)
			{
				this.UpdateProperies();
			});
		}

		public override void OnShowPage()
		{
			this.GamepadService.ExternalDeviceRelationsHelper.Refresh();
		}

		private async void CurrentGamepadsAuthCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			int count = this.GamepadService.ExternalDeviceRelationsHelper.CurrentGamepadsAuthCollection.Count;
			if (count > 0 && this.GamepadAuthSelectedIndex == -1)
			{
				this.GamepadAuthSelectedIndex = 0;
			}
			if (count == 0)
			{
				this.GamepadAuthSelectedIndex = -1;
			}
			await this.GetExternalDeviceState();
			this.OnPropertyChanged("IsSaveEnabled");
			this.OnPropertyChanged("AnyGamepadAuthPresent");
			this.OnPropertyChanged("CurrentAuthGamepad");
		}

		private void ReInit(bool skipAuthSelect = false)
		{
			this.ExternalDeviceCollection = new ObservableCollection<ExternalDevice>(App.GamepadService.ExternalDevices);
			this.ExternalClientsCollection = new ObservableCollection<ExternalClient>(App.GamepadService.ExternalClients);
			this.ExternalDeviceCollection.Add(new ExternalDevice
			{
				Alias = DTLocalization.GetString(11992),
				DeviceType = 1000,
				SortIndex = 1000
			});
			this.ExternalClientsCollection.Add(new ExternalClient
			{
				Alias = DTLocalization.GetString(11992),
				Dummy = true,
				SortIndex = 1000
			});
			if (!skipAuthSelect)
			{
				if (this.IsExternalClientSelected() && this.ExternalClientsCollection[this.ExternalClientSelectedIndex].IsConsoleAuthRequired)
				{
					this.UseAuthGamepad = true;
				}
				if (this.IsExternalDeviceSelected() && (this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex].DeviceType == 1 || this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex].DeviceType == 3))
				{
					GameProfilesService gameProfilesService = this.GameProfilesService;
					bool flag;
					if (gameProfilesService == null)
					{
						flag = false;
					}
					else
					{
						GameVM currentGame = gameProfilesService.CurrentGame;
						VirtualGamepadType? virtualGamepadType;
						if (currentGame == null)
						{
							virtualGamepadType = null;
						}
						else
						{
							ConfigVM currentConfig = currentGame.CurrentConfig;
							if (currentConfig == null)
							{
								virtualGamepadType = null;
							}
							else
							{
								ConfigData configData = currentConfig.ConfigData;
								virtualGamepadType = ((configData != null) ? new VirtualGamepadType?(configData.VirtualGamepadType) : null);
							}
						}
						VirtualGamepadType? virtualGamepadType2 = virtualGamepadType;
						VirtualGamepadType virtualGamepadType3 = 1;
						flag = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
					}
					if (flag)
					{
						this.UseAuthGamepad = true;
					}
				}
			}
			if (this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex].IsDummy)
			{
				this.ExternalDeviceSelectedIndex = 0;
			}
			this.SetUserConsoleType();
			this.UpdateProperies();
		}

		public GamepadService GamepadService { get; set; }

		public GameProfilesService GameProfilesService { get; set; }

		public bool RebootRequired
		{
			get
			{
				return BluetoothUtils.IsRebootRequired();
			}
		}

		public override PageType PageType
		{
			get
			{
				return PageType.AdaptersSettings;
			}
		}

		public bool IsGimxSelected
		{
			get
			{
				return this.IsExternalDeviceSelected() && this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex].DeviceType == 1;
			}
		}

		public bool IsBluetoothSelected
		{
			get
			{
				return this.IsExternalDeviceSelected() && this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex].DeviceType == 0;
			}
		}

		public bool IsEsp32Selected
		{
			get
			{
				return this.IsExternalDeviceSelected() && this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex].DeviceType == 2;
			}
		}

		public bool IsEsp32S2Selected
		{
			get
			{
				return this.IsExternalDeviceSelected() && this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex].DeviceType == 3;
			}
		}

		public bool IsDummyDeviceSelected
		{
			get
			{
				return this.ExternalDeviceSelectedIndex < this.ExternalDeviceCollection.Count && this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex].IsDummy;
			}
		}

		public bool IsDummyClientSelected
		{
			get
			{
				return this.ExternalClientSelectedIndex < this.ExternalClientsCollection.Count && this.ExternalClientsCollection[this.ExternalClientSelectedIndex].IsDummy;
			}
		}

		public bool IsConsoleClientSelected
		{
			get
			{
				return (this.ExternalClientSelectedIndex < this.ExternalClientsCollection.Count && this.ExternalClientsCollection[this.ExternalClientSelectedIndex].IsConsole) || (this.IsEsp32S2Selected && this.USBTargetConsoleType > 0);
			}
		}

		public bool IsSonyConsoleClientSelected
		{
			get
			{
				return this.ExternalClientSelectedIndex < this.ExternalClientsCollection.Count && this.ExternalClientsCollection[this.ExternalClientSelectedIndex].IsSonyConsole;
			}
		}

		public bool IsNintendoSwitchConsoleClientSelected
		{
			get
			{
				return this.ExternalClientSelectedIndex < this.ExternalClientsCollection.Count && this.ExternalClientsCollection[this.ExternalClientSelectedIndex].IsNintendoSwitchConsole;
			}
		}

		public bool IsUseAuthGamepadAlways
		{
			get
			{
				if (this.IsConsoleClientSelected)
				{
					return true;
				}
				if (!this.IsGimxSelected && !this.IsEsp32S2Selected)
				{
					return false;
				}
				GameProfilesService gameProfilesService = this.GameProfilesService;
				if (gameProfilesService == null)
				{
					return false;
				}
				GameVM currentGame = gameProfilesService.CurrentGame;
				VirtualGamepadType? virtualGamepadType;
				if (currentGame == null)
				{
					virtualGamepadType = null;
				}
				else
				{
					ConfigVM currentConfig = currentGame.CurrentConfig;
					if (currentConfig == null)
					{
						virtualGamepadType = null;
					}
					else
					{
						ConfigData configData = currentConfig.ConfigData;
						virtualGamepadType = ((configData != null) ? new VirtualGamepadType?(configData.VirtualGamepadType) : null);
					}
				}
				VirtualGamepadType? virtualGamepadType2 = virtualGamepadType;
				VirtualGamepadType virtualGamepadType3 = 1;
				return (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
			}
		}

		public bool IsHideClientsAndGamepadAuth
		{
			get
			{
				return this.IsDummyDeviceSelected || ((this.IsBluetoothSelected || this.IsEsp32Selected) && this.CurrentExternalDevice.IsOffline);
			}
		}

		public bool IsHideGamepadAuth
		{
			get
			{
				return this.IsDummyDeviceSelected || ((this.IsBluetoothSelected || this.IsEsp32Selected) && this.IsDummyClientSelected);
			}
		}

		public ObservableCollection<ExternalDevice> ExternalDeviceCollection
		{
			get
			{
				ObservableCollection<ExternalDevice> externalDeviceCollection = this._externalDeviceCollection;
				if (externalDeviceCollection != null && externalDeviceCollection.Count > 1)
				{
					for (int i = 1; i < this._externalDeviceCollection.Count; i++)
					{
						if (this._externalDeviceCollection[i].SortIndex < this._externalDeviceCollection[i - 1].SortIndex)
						{
							this._externalDeviceCollection = new ObservableCollection<ExternalDevice>(this._externalDeviceCollection.OrderBy((ExternalDevice x) => x.SortIndex));
							break;
						}
					}
				}
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

		public int ExternalDeviceSelectedIndex
		{
			get
			{
				return this._externalDeviceSelectedIndex;
			}
			set
			{
				if (value < 0)
				{
					return;
				}
				this.SetProperty(ref this._externalDeviceSelectedIndex, value, "ExternalDeviceSelectedIndex");
				this.GetExternalDeviceState();
				this.OnPropertyChanged("ExternalDeviceStateVisibility");
				this.OnPropertyChanged("IsSaveEnabled");
				this.OnPropertyChanged("IsDummyDeviceSelected");
				this.OnPropertyChanged("IsHideClientsAndGamepadAuth");
				this.OnPropertyChanged("IsHideGamepadAuth");
				this.OnPropertyChanged("IsBluetoothSelected");
				this.OnPropertyChanged("IsGimxSelected");
				this.OnPropertyChanged("CurrentExternalDevice");
				this.OnPropertyChanged("IsConsoleClientSelected");
				this.OnPropertyChanged("IsEsp32S2Selected");
				this.OnPropertyChanged("IsEsp32Selected");
				this.SetUserConsoleType();
				this.OnPropertyChanged("USBTargetConsoleType");
			}
		}

		public ExternalDevice CurrentExternalDevice
		{
			get
			{
				return this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex];
			}
		}

		public bool ExternalDeviceStateVisibility
		{
			get
			{
				return this.ExternalDeviceCollection != null && this.ExternalDeviceCollection.Count != 0 && this.ExternalDeviceSelectedIndex < this.ExternalDeviceCollection.Count && !this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex].IsDummy;
			}
		}

		public ExternalDeviceState ExternalDeviceState
		{
			get
			{
				return this._externalDeviceState;
			}
			set
			{
				if (this.SetProperty(ref this._externalDeviceState, value, "ExternalDeviceState"))
				{
					this.OnPropertyChanged("CurrentExternalDevice");
				}
			}
		}

		private async Task GetExternalDeviceState()
		{
			if (this.ExternalDeviceSelectedIndex < 0)
			{
				this.ExternalDeviceSelectedIndex = 0;
			}
			ExternalDevice externalDevice = ((this.ExternalDeviceSelectedIndex < this.ExternalDeviceCollection.Count) ? this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex] : null);
			if (this.ExternalClientSelectedIndex < 0)
			{
				this.ExternalClientSelectedIndex = 0;
			}
			ExternalClient externalClient = ((this.ExternalClientSelectedIndex < this.ExternalClientsCollection.Count) ? this.ExternalClientsCollection[this.ExternalClientSelectedIndex] : null);
			if (this.GamepadAuthSelectedIndex < 0)
			{
				this.GamepadAuthSelectedIndex = 0;
			}
			GamepadAuth gamepadAuth = ((this.UseAuthGamepad && this.GamepadAuthSelectedIndex < this.GamepadService.ExternalDeviceRelationsHelper.CurrentGamepadsAuthCollection.Count) ? this.GamepadService.ExternalDeviceRelationsHelper.CurrentGamepadsAuthCollection[this.GamepadAuthSelectedIndex] : null);
			ExternalDeviceState externalDeviceState = await this.GamepadService.ExternalDeviceRelationsHelper.GetExternalDeviceStateWithProfiles(externalDevice, externalClient, gamepadAuth);
			this.ExternalDeviceState = externalDeviceState;
		}

		public ExternalClient CurrentExternalClient
		{
			get
			{
				return this.ExternalClientsCollection[this.ExternalClientSelectedIndex];
			}
		}

		public ObservableCollection<ExternalClient> ExternalClientsCollection
		{
			get
			{
				ObservableCollection<ExternalClient> externalClientsCollection = this._externalClientsCollection;
				if (externalClientsCollection != null && externalClientsCollection.Count > 1)
				{
					for (int i = 1; i < this._externalClientsCollection.Count; i++)
					{
						if (this._externalClientsCollection[i].SortIndex < this._externalClientsCollection[i - 1].SortIndex)
						{
							this._externalClientsCollection = new ObservableCollection<ExternalClient>(this._externalClientsCollection.OrderBy((ExternalClient x) => x.SortIndex));
							break;
						}
					}
				}
				return this._externalClientsCollection;
			}
			set
			{
				if (this._externalClientsCollection == value)
				{
					return;
				}
				this.SetProperty(ref this._externalClientsCollection, value, "ExternalClientsCollection");
			}
		}

		public int ExternalClientSelectedIndex
		{
			get
			{
				return this._externalClientSelectedIndex;
			}
			set
			{
				if (value < 0)
				{
					return;
				}
				if (this.SetProperty(ref this._externalClientSelectedIndex, value, "ExternalClientSelectedIndex") && this.IsExternalClientSelected() && !this.ExternalClientsCollection[this.ExternalClientSelectedIndex].IsDummy)
				{
					this.GamepadAuthSelectedIndex = 0;
					this.GamepadService.ExternalDeviceRelationsHelper.RecreateCurrentGamepadsAuthList(this.ExternalClientsCollection[this.ExternalClientSelectedIndex]);
					if (this.IsBluetoothSelected || this.IsEsp32Selected)
					{
						this.UseAuthGamepad = this.ExternalClientsCollection[this.ExternalClientSelectedIndex].IsConsoleAuthRequired;
					}
				}
				this.OnPropertyChanged("ExternalDeviceState");
				this.OnPropertyChanged("ClientConsoleType");
				this.OnPropertyChanged("UseAuthGamepad");
				this.OnPropertyChanged("AuthText");
				this.OnPropertyChanged("IsSaveEnabled");
				this.OnPropertyChanged("AnyGamepadAuthPresent");
				this.OnPropertyChanged("IsDummyDeviceSelected");
				this.OnPropertyChanged("IsDummyClientSelected");
				this.OnPropertyChanged("IsConsoleClientSelected");
				this.OnPropertyChanged("IsSonyConsoleClientSelected");
				this.OnPropertyChanged("IsHideClientsAndGamepadAuth");
				this.OnPropertyChanged("IsHideGamepadAuth");
			}
		}

		public ConsoleType USBTargetConsoleType { get; set; }

		public int USBTargetConsoleTypeIndex
		{
			get
			{
				ConsoleType usbtargetConsoleType = this.USBTargetConsoleType;
				if (usbtargetConsoleType == 1)
				{
					return 0;
				}
				if (usbtargetConsoleType != 3)
				{
					return 2;
				}
				return 1;
			}
			set
			{
				if (this.USBTargetConsoleTypeIndex == value)
				{
					return;
				}
				switch (value)
				{
				case 0:
					this.USBTargetConsoleType = 1;
					break;
				case 1:
					this.USBTargetConsoleType = 3;
					break;
				case 2:
					this.USBTargetConsoleType = 0;
					break;
				}
				this.OnPropertyChanged("USBTargetConsoleTypeIndex");
			}
		}

		public ConsoleType ClientConsoleType
		{
			get
			{
				if (!this.IsExternalClientSelected())
				{
					return 0;
				}
				return this.ExternalClientsCollection[this.ExternalClientSelectedIndex].ConsoleType;
			}
		}

		public bool AnyGamepadAuthPresent
		{
			get
			{
				return this.GamepadService.ExternalDeviceRelationsHelper.CurrentGamepadsAuthCollection.Count > 0;
			}
		}

		public string AuthText
		{
			get
			{
				return string.Format(DTLocalization.GetString(12022), this.GetAuthGamepadDescription(this.ClientConsoleType));
			}
		}

		private string GetAuthGamepadDescription(ConsoleType consoleType)
		{
			switch (consoleType)
			{
			case 0:
			{
				GameProfilesService gameProfilesService = this.GameProfilesService;
				string text;
				if (gameProfilesService == null)
				{
					text = null;
				}
				else
				{
					GameVM currentGame = gameProfilesService.CurrentGame;
					if (currentGame == null)
					{
						text = null;
					}
					else
					{
						ConfigVM currentConfig = currentGame.CurrentConfig;
						if (currentConfig == null)
						{
							text = null;
						}
						else
						{
							ConfigData configData = currentConfig.ConfigData;
							text = ((configData != null) ? configData.VirtualGamepadType.TryGetDescription() : null);
						}
					}
				}
				string text2 = text;
				return text2.Substring(7, text2.Length - 7);
			}
			case 1:
				return 14.TryGetDescription();
			case 3:
				return "Xbox";
			}
			return string.Empty;
		}

		public GamepadAuth CurrentAuthGamepad
		{
			get
			{
				if (this.GamepadAuthSelectedIndex != -1 && this.GamepadAuthSelectedIndex < this.GamepadService.ExternalDeviceRelationsHelper.CurrentGamepadsAuthCollection.Count)
				{
					return this.GamepadService.ExternalDeviceRelationsHelper.CurrentGamepadsAuthCollection[this.GamepadAuthSelectedIndex];
				}
				return null;
			}
		}

		public int GamepadAuthSelectedIndex
		{
			get
			{
				if (this._gamepadAuthSelectedIndex >= this.GamepadService.ExternalDeviceRelationsHelper.CurrentGamepadsAuthCollection.Count)
				{
					this._gamepadAuthSelectedIndex = 0;
				}
				return this._gamepadAuthSelectedIndex;
			}
			set
			{
				this.SetProperty(ref this._gamepadAuthSelectedIndex, value, "GamepadAuthSelectedIndex");
				this.OnPropertyChanged("ExternalDeviceState");
				this.OnPropertyChanged("IsSaveEnabled");
				this.OnPropertyChanged("CurrentAuthGamepad");
				this.OnPropertyChanged("AnyGamepadAuthPresent");
			}
		}

		public bool UseAuthGamepad
		{
			get
			{
				return this._useAuthGamepad;
			}
			set
			{
				this.SetProperty(ref this._useAuthGamepad, value, "UseAuthGamepad");
				this.OnPropertyChanged("ExternalDeviceState");
				this.OnPropertyChanged("IsSaveEnabled");
			}
		}

		protected override async void NavigateToNextPage()
		{
			if (this.RebootRequired && this.IsBluetoothSelected)
			{
				base.GoPage(PageType.BluetoothSettings);
			}
			else if (this.CurrentExternalDevice.DeviceType == 3)
			{
				GameProfilesService gameProfilesService = this.GameProfilesService;
				VirtualGamepadType? virtualGamepadType;
				if (gameProfilesService == null)
				{
					virtualGamepadType = null;
				}
				else
				{
					GameVM currentGame = gameProfilesService.CurrentGame;
					if (currentGame == null)
					{
						virtualGamepadType = null;
					}
					else
					{
						ConfigVM currentConfig = currentGame.CurrentConfig;
						if (currentConfig == null)
						{
							virtualGamepadType = null;
						}
						else
						{
							ConfigData configData = currentConfig.ConfigData;
							virtualGamepadType = ((configData != null) ? new VirtualGamepadType?(configData.VirtualGamepadType) : null);
						}
					}
				}
				VirtualGamepadType? virtualGamepadType2 = virtualGamepadType;
				if (virtualGamepadType2 != null && !VirtualControllerTypeExtensions.CanWorkOnConsole(virtualGamepadType2.GetValueOrDefault(), this.USBTargetConsoleType))
				{
					if (DTMessageBox.Show(string.Format(DTLocalization.GetString(12778), this.USBTargetConsoleType.TryGetDescription(), virtualGamepadType2.TryGetLocalizedDescription()), MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, string.Format(DTLocalization.GetString(12779), ConsoleTypeExtensions.GetVirtualControllerType(this.USBTargetConsoleType).TryGetLocalizedDescription()), false, MessageBoxResult.None) == MessageBoxResult.Cancel)
					{
						return;
					}
					this.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.VirtualGamepadType = ConsoleTypeExtensions.GetVirtualControllerType(this.USBTargetConsoleType);
				}
				this.UseAuthGamepad = this.USBTargetConsoleType == 1 || this.USBTargetConsoleType == 3 || this.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.VirtualGamepadType == 1;
				if (this.UseAuthGamepad)
				{
					base.GoPage(PageType.SelectAuthGamepad);
				}
				else
				{
					TaskAwaiter<bool> taskAwaiter = this.Save().GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						TaskAwaiter<bool> taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (taskAwaiter.GetResult())
					{
						base.GoPage(PageType.FinishAndWaitUSBClient);
					}
				}
			}
			else if (this.CurrentExternalDevice.IsDummy)
			{
				this._previousDeviceIndex = this.ExternalDeviceSelectedIndex;
				base.GoPage(PageType.DeviceType);
			}
			else if (this.CurrentExternalClient.IsDummy)
			{
				ExternalDevice externalDevice = this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex];
				this._wizard.Result.DeviceType = externalDevice.DeviceType;
				this._wizard.Result.SerialPort = externalDevice.SerialPort;
				base.GoPage(PageType.AddExternalClient);
			}
		}

		public new DelegateCommand OkCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._okCommand) == null)
				{
					delegateCommand = (this._okCommand = new DelegateCommand(new Action(this.OnOk), new Func<bool>(this.CanOk)));
				}
				return delegateCommand;
			}
		}

		public async Task<bool> Save()
		{
			this.SaveOverwriteProperty();
			bool flag = this.IsExternalDeviceSelected();
			bool flag2 = this.IsExternalClientSelected();
			ExternalDevice externalDevice = (flag ? this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex] : null);
			ExternalClient externalClient = (flag2 ? this.ExternalClientsCollection[this.ExternalClientSelectedIndex] : null);
			GamepadAuth authGamepad = ((this.UseAuthGamepad && this.GamepadService.ExternalDeviceRelationsHelper.CurrentGamepadsAuthCollection != null && this.GamepadAuthSelectedIndex < this.GamepadService.ExternalDeviceRelationsHelper.CurrentGamepadsAuthCollection.Count && this.GamepadService.ExternalDeviceRelationsHelper.CurrentGamepadsAuthCollection[this.GamepadAuthSelectedIndex].IsValid()) ? this.GamepadService.ExternalDeviceRelationsHelper.CurrentGamepadsAuthCollection[this.GamepadAuthSelectedIndex] : null);
			if (this.IsEsp32S2Selected)
			{
				externalDevice.TargetConsoleType = this.USBTargetConsoleType;
				await App.GamepadService.BinDataSerialize.SaveExternalDevices();
			}
			return await this.GamepadService.ExternalDeviceRelationsHelper.AddAndSaveRelation(externalDevice, externalClient, authGamepad);
		}

		protected new async void OnOk()
		{
			await this.Save();
			this._wizard.OnOk();
		}

		public bool IsSaveEnabled
		{
			get
			{
				return this.CanSave();
			}
		}

		private bool CanSave()
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = this.IsExternalDeviceSelected();
			bool flag4 = this.IsExternalClientSelected();
			ExternalDeviceType externalDeviceType = (flag3 ? this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex].DeviceType : 1000);
			ConsoleType consoleType = (flag4 ? this.ExternalClientsCollection[this.ExternalClientSelectedIndex].ConsoleType : 0);
			switch (externalDeviceType)
			{
			case 0:
				flag2 = flag3 && flag4 && !this.RebootRequired;
				break;
			case 1:
				flag2 = flag3;
				break;
			case 2:
				flag2 = flag3 && flag4;
				break;
			case 3:
				return false;
			}
			if (this.UseAuthGamepad && this.GamepadService.ExternalDeviceRelationsHelper.CurrentGamepadsAuthCollection.Count == 0)
			{
				flag = true;
			}
			if (externalDeviceType == null || externalDeviceType == 2)
			{
				if (consoleType != null && this.UseAuthGamepad && this.GamepadService.ExternalDeviceRelationsHelper.CurrentGamepadsAuthCollection.Count == 0)
				{
					flag = true;
				}
				if (consoleType == 1)
				{
					GameProfilesService gameProfilesService = this.GameProfilesService;
					bool flag5;
					if (gameProfilesService == null)
					{
						flag5 = true;
					}
					else
					{
						GameVM currentGame = gameProfilesService.CurrentGame;
						VirtualGamepadType? virtualGamepadType;
						if (currentGame == null)
						{
							virtualGamepadType = null;
						}
						else
						{
							ConfigVM currentConfig = currentGame.CurrentConfig;
							if (currentConfig == null)
							{
								virtualGamepadType = null;
							}
							else
							{
								ConfigData configData = currentConfig.ConfigData;
								virtualGamepadType = ((configData != null) ? new VirtualGamepadType?(configData.VirtualGamepadType) : null);
							}
						}
						VirtualGamepadType? virtualGamepadType2 = virtualGamepadType;
						VirtualGamepadType virtualGamepadType3 = 2;
						flag5 = !((virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null));
					}
					if (flag5)
					{
						flag = true;
					}
				}
				if (consoleType == 3)
				{
					GameProfilesService gameProfilesService2 = this.GameProfilesService;
					bool flag6;
					if (gameProfilesService2 == null)
					{
						flag6 = true;
					}
					else
					{
						GameVM currentGame2 = gameProfilesService2.CurrentGame;
						VirtualGamepadType? virtualGamepadType4;
						if (currentGame2 == null)
						{
							virtualGamepadType4 = null;
						}
						else
						{
							ConfigVM currentConfig2 = currentGame2.CurrentConfig;
							if (currentConfig2 == null)
							{
								virtualGamepadType4 = null;
							}
							else
							{
								ConfigData configData2 = currentConfig2.ConfigData;
								virtualGamepadType4 = ((configData2 != null) ? new VirtualGamepadType?(configData2.VirtualGamepadType) : null);
							}
						}
						VirtualGamepadType? virtualGamepadType2 = virtualGamepadType4;
						VirtualGamepadType virtualGamepadType3 = 1;
						flag6 = !((virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null));
					}
					if (flag6)
					{
						flag = true;
					}
				}
			}
			bool flag8;
			if (externalDeviceType == null || externalDeviceType == 2)
			{
				GameProfilesService gameProfilesService3 = this.GameProfilesService;
				bool flag7;
				if (gameProfilesService3 == null)
				{
					flag7 = false;
				}
				else
				{
					GameVM currentGame3 = gameProfilesService3.CurrentGame;
					VirtualGamepadType? virtualGamepadType5;
					if (currentGame3 == null)
					{
						virtualGamepadType5 = null;
					}
					else
					{
						ConfigVM currentConfig3 = currentGame3.CurrentConfig;
						if (currentConfig3 == null)
						{
							virtualGamepadType5 = null;
						}
						else
						{
							ConfigData configData3 = currentConfig3.ConfigData;
							virtualGamepadType5 = ((configData3 != null) ? new VirtualGamepadType?(configData3.VirtualGamepadType) : null);
						}
					}
					VirtualGamepadType? virtualGamepadType2 = virtualGamepadType5;
					VirtualGamepadType virtualGamepadType3 = 0;
					flag7 = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
				}
				if (flag7)
				{
					flag8 = true;
					goto IL_2C5;
				}
			}
			if (externalDeviceType == null || externalDeviceType == 2)
			{
				GameProfilesService gameProfilesService4 = this.GameProfilesService;
				if (gameProfilesService4 == null)
				{
					flag8 = false;
				}
				else
				{
					GameVM currentGame4 = gameProfilesService4.CurrentGame;
					VirtualGamepadType? virtualGamepadType6;
					if (currentGame4 == null)
					{
						virtualGamepadType6 = null;
					}
					else
					{
						ConfigVM currentConfig4 = currentGame4.CurrentConfig;
						if (currentConfig4 == null)
						{
							virtualGamepadType6 = null;
						}
						else
						{
							ConfigData configData4 = currentConfig4.ConfigData;
							virtualGamepadType6 = ((configData4 != null) ? new VirtualGamepadType?(configData4.VirtualGamepadType) : null);
						}
					}
					VirtualGamepadType? virtualGamepadType2 = virtualGamepadType6;
					VirtualGamepadType virtualGamepadType3 = 3;
					flag8 = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
				}
			}
			else
			{
				flag8 = false;
			}
			IL_2C5:
			bool flag9 = flag8;
			return !base.IsProcessing && flag2 && !flag && !flag9;
		}

		private bool IsExternalDeviceSelected()
		{
			return this.ExternalDeviceSelectedIndex < this.ExternalDeviceCollection.Count && !this.ExternalDeviceCollection[this.ExternalDeviceSelectedIndex].IsDummy;
		}

		private bool IsExternalClientSelected()
		{
			ExternalDevice currentExternalDevice = this.CurrentExternalDevice;
			if (currentExternalDevice == null || currentExternalDevice.DeviceType != 3)
			{
				ExternalDevice currentExternalDevice2 = this.CurrentExternalDevice;
				if (currentExternalDevice2 == null || currentExternalDevice2.DeviceType != 1)
				{
					return this.ExternalClientSelectedIndex < this.ExternalClientsCollection.Count && !this.ExternalClientsCollection[this.ExternalClientSelectedIndex].IsDummy;
				}
			}
			return false;
		}

		public bool ExternalDeviceOverwritePrevConfig
		{
			get
			{
				return this._externalDeviceOverwritePrevConfig;
			}
			set
			{
				if (this._externalDeviceOverwritePrevConfig == value)
				{
					return;
				}
				this._externalDeviceOverwritePrevConfig = value;
				this.OnPropertyChanged("ExternalDeviceOverwritePrevConfig");
			}
		}

		private void LoadOverwriteProperty()
		{
			this.ExternalDeviceOverwritePrevConfig = App.UserSettingsService.ExternalDeviceOverwritePrevConfig;
		}

		private void SaveOverwriteProperty()
		{
			App.UserSettingsService.ExternalDeviceOverwritePrevConfig = this.ExternalDeviceOverwritePrevConfig;
			App.UserSettingsService.Save();
		}

		private async void UpdateProperies()
		{
			await this.GetExternalDeviceState();
			this.OnPropertyChanged("ExternalDeviceStateVisibility");
			this.OnPropertyChanged("ExternalDeviceState");
			this.OnPropertyChanged("IsSaveEnabled");
			this.OnPropertyChanged("IsDummyDeviceSelected");
			this.OnPropertyChanged("IsHideClientsAndGamepadAuth");
			this.OnPropertyChanged("IsHideGamepadAuth");
			this.OnPropertyChanged("IsBluetoothSelected");
			this.OnPropertyChanged("IsGimxSelected");
		}

		private void SetUserConsoleType()
		{
			if (this.CurrentExternalDevice.TargetConsoleType == null)
			{
				GameProfilesService gameProfilesService = this.GameProfilesService;
				VirtualGamepadType? virtualGamepadType;
				if (gameProfilesService == null)
				{
					virtualGamepadType = null;
				}
				else
				{
					GameVM currentGame = gameProfilesService.CurrentGame;
					if (currentGame == null)
					{
						virtualGamepadType = null;
					}
					else
					{
						ConfigVM currentConfig = currentGame.CurrentConfig;
						if (currentConfig == null)
						{
							virtualGamepadType = null;
						}
						else
						{
							ConfigData configData = currentConfig.ConfigData;
							virtualGamepadType = ((configData != null) ? new VirtualGamepadType?(configData.VirtualGamepadType) : null);
						}
					}
				}
				VirtualGamepadType? virtualGamepadType2 = virtualGamepadType;
				if (virtualGamepadType2 != null)
				{
					this.USBTargetConsoleType = VirtualControllerTypeExtensions.GetConsoleType(virtualGamepadType2.Value);
					return;
				}
			}
			this.USBTargetConsoleType = this.CurrentExternalDevice.TargetConsoleType;
			this.OnPropertyChanged("USBTargetConsoleTypeIndex");
		}

		public new event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
			if (propertyName == "AdaptersSettingsVMReinit")
			{
				this.ReInit(false);
			}
		}

		private int _previousDeviceIndex;

		private ObservableCollection<ExternalDevice> _externalDeviceCollection;

		private int _externalDeviceSelectedIndex;

		private ExternalDeviceState _externalDeviceState;

		private ObservableCollection<ExternalClient> _externalClientsCollection;

		private int _externalClientSelectedIndex;

		private int _gamepadAuthSelectedIndex = -1;

		private bool _useAuthGamepad;

		private DelegateCommand _okCommand;

		private bool _externalDeviceOverwritePrevConfig;
	}
}
