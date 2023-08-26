using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DiscSoftReWASDServiceNamespace;
using Newtonsoft.Json;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.DataModels.CompositeDevicesCollection;
using XBEliteWPF.DataModels.ControllerProfileInfo;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtil;

namespace XBEliteWPF.Infrastructure.Controller
{
	[JsonObject(1)]
	public class CompositeControllerVM : BaseCompositeControllerVM
	{
		public override ControllerVM CurrentController
		{
			get
			{
				return this._currentControllerVM;
			}
			set
			{
				if (this._currentControllerVM != value)
				{
					this._currentControllerVM = value;
					EventHandler<ControllerVM> currentControllerChanged = this.CurrentControllerChanged;
					if (currentControllerChanged == null)
					{
						return;
					}
					currentControllerChanged(this, value);
				}
			}
		}

		public BaseControllerVM Controller1
		{
			get
			{
				return this._controller1;
			}
			set
			{
				this.SetController(ref this._controller1, value, 0, "Controller1");
			}
		}

		public BaseControllerVM Controller2
		{
			get
			{
				return this._controller2;
			}
			set
			{
				this.SetController(ref this._controller2, value, 1, "Controller2");
			}
		}

		public BaseControllerVM Controller3
		{
			get
			{
				return this._controller3;
			}
			set
			{
				this.SetController(ref this._controller3, value, 2, "Controller3");
			}
		}

		public BaseControllerVM Controller4
		{
			get
			{
				return this._controller4;
			}
			set
			{
				this.SetController(ref this._controller4, value, 3, "Controller4");
			}
		}

		public override string ID
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
				this._shortId = XBUtils.CalcShortID(this._id);
			}
		}

		public override string ShortID
		{
			get
			{
				return this._shortId;
			}
		}

		public override string HintID
		{
			get
			{
				return "ID: " + this._shortId;
			}
		}

		public override uint[] Types
		{
			get
			{
				return this._types;
			}
			set
			{
				this._types = value;
			}
		}

		public override ulong[] Ids
		{
			get
			{
				return this._ids;
			}
			set
			{
				this._ids = value;
			}
		}

		public override ControllerFamily ControllerFamily
		{
			get
			{
				return 3;
			}
		}

		public override ControllerFamily TreatAsControllerFamily
		{
			get
			{
				if (this.IsNintendoSwitchJoyConComposite)
				{
					return 0;
				}
				return base.TreatAsControllerFamily;
			}
		}

		public override bool IsUnknownControllerType
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.IsUnknownControllerType);
			}
		}

		public override bool IsInvalidControllerType
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.IsInvalidControllerType);
			}
		}

		public override bool IsUnsupportedControllerType
		{
			get
			{
				return this.IsUnknownControllerType;
			}
		}

		public override bool IsNintendoSwitchJoyConL
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.IsNintendoSwitchJoyConL && c != null && c.IsOnline);
			}
		}

		public override bool IsNintendoSwitchJoyConR
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.IsNintendoSwitchJoyConR && c != null && c.IsOnline);
			}
		}

		public bool IsNintendoSwitchJoyConLExpected
		{
			get
			{
				return this.Types.Any((uint t) => t == ControllerTypeExtensions.ConvertEnumToPhysicalType(9));
			}
		}

		public bool IsNintendoSwitchJoyConRExpected
		{
			get
			{
				return this.Types.Any((uint t) => t == ControllerTypeExtensions.ConvertEnumToPhysicalType(10));
			}
		}

		public override bool IsNintendoSwitchJoyConComposite
		{
			get
			{
				if (this.IsNintendoSwitchJoyConLExpected && this.IsNintendoSwitchJoyConRExpected)
				{
					return this.Types.Count((uint t) => t > 0U) == 2;
				}
				return false;
			}
		}

		public override bool HasXboxElite
		{
			get
			{
				return this.Types.Any((uint t) => t == ControllerTypeExtensions.ConvertEnumToPhysicalType(3) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(12));
			}
		}

		public override bool HasXboxElite2
		{
			get
			{
				return this.Types.Any((uint t) => t == ControllerTypeExtensions.ConvertEnumToPhysicalType(12));
			}
		}

		public override bool HasExclusiveCaptureControllers
		{
			get
			{
				return this.Types.Any((uint t) => t == ControllerTypeExtensions.ConvertEnumToPhysicalType(16) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(63) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(19) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(28) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(54));
			}
		}

		public override bool HasTouchpad
		{
			get
			{
				return this.Types.Any((uint t) => t == ControllerTypeExtensions.ConvertEnumToPhysicalType(16) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(63) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(4) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(14) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(20) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(21));
			}
		}

		public override List<BaseControllerVM> BaseControllers
		{
			get
			{
				return new List<BaseControllerVM> { this.Controller1, this.Controller2, this.Controller3, this.Controller4 };
			}
			set
			{
				this.Controller1 = value[0];
				this.Controller2 = value[1];
				this.Controller3 = value[2];
				this.Controller4 = value[3];
			}
		}

		public List<BaseControllerVM> NonNullBaseControllers
		{
			get
			{
				return this.BaseControllers.Where((BaseControllerVM bc) => bc != null).ToList<BaseControllerVM>();
			}
		}

		public override List<BaseControllerVM> ControllersForMaskFilter
		{
			get
			{
				List<BaseControllerVM> list = new List<BaseControllerVM>(this.NonNullBaseControllers);
				list.Insert(0, this);
				return list;
			}
		}

		public override bool IsAvailiableForComposition
		{
			get
			{
				return false;
			}
		}

		public event EventHandler<ControllerVM> CurrentControllerChanged;

		public CompositeControllerVM(CompositeDevice compositeDevice, bool isDebugController = false)
			: base(isDebugController)
		{
			this.ContainerId = Guid.NewGuid();
			base.ControllerProfileInfoCollections = compositeDevice.ControllerProfileInfoCollections;
			this.ID = compositeDevice.ID;
			this._types = ControllerTypeExtensions.ConvertEnumsToPhysicalTypes(IControllerProfileInfoCollectionContainerExtensions.GetControllerTypes(base.ControllerProfileInfoCollections, false, null));
			this._ids = IControllerProfileInfoCollectionContainerExtensions.GetControllerIds(base.ControllerProfileInfoCollections, false, null);
			this.Controller1 = this.SetOfflineController(base.ControllerProfileInfoCollections[0]);
			this.Controller2 = this.SetOfflineController(base.ControllerProfileInfoCollections[1]);
			this.Controller3 = this.SetOfflineController(base.ControllerProfileInfoCollections[2]);
			this.Controller4 = this.SetOfflineController(base.ControllerProfileInfoCollections[3]);
		}

		private BaseControllerVM SetOfflineController(ControllerProfileInfoCollection ControllerProfileInfoCollectoin)
		{
			if (!string.IsNullOrEmpty(IControllerProfileInfoCollectionContainerExtensions.CalculateID(ControllerProfileInfoCollectoin.ControllerProfileInfos)))
			{
				return new OfflineDeviceVM(ControllerProfileInfoCollectoin);
			}
			return null;
		}

		protected override void Vibrate()
		{
			this.BaseControllers.ForEach(delegate(BaseControllerVM c)
			{
				if (c != null)
				{
					c.VibrateCommand.Execute();
				}
			});
		}

		public override bool UpdateControllerInfo(REWASD_CONTROLLER_INFO controllerInfo)
		{
			BaseControllerVM baseControllerVM = base.FindController(controllerInfo.Id);
			return baseControllerVM != null && baseControllerVM.UpdateControllerInfo(controllerInfo);
		}

		public override void UpdateControllerState(REWASD_GET_CONTROLLER_STATE_RESPONSE controllerState)
		{
		}

		public override int GetControllerFamilyCount(ControllerFamily controllerFamily)
		{
			return this.BaseControllers.Count((BaseControllerVM item) => item != null && item.ControllerFamily == controllerFamily);
		}

		public override ControllerTypeEnum? GetGamepadTypeByIndex(int index)
		{
			int num = 0;
			foreach (BaseControllerVM baseControllerVM in this.BaseControllers)
			{
				if (baseControllerVM != null)
				{
					if (baseControllerVM.ControllerFamily == null)
					{
						num++;
					}
					if (num == index)
					{
						return baseControllerVM.FirstGamepadType;
					}
				}
			}
			return null;
		}

		public override bool IsBluetoothConnectionFlagPresent
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.IsBluetoothConnectionFlagPresent);
			}
		}

		public override bool IsAnalogTriggersPresent
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.IsAnalogTriggersPresent);
			}
		}

		public override bool IsTriggersFullPullPresent
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.IsTriggersFullPullPresent);
			}
		}

		public override bool IsMotorRumbleMotorPresent
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.IsMotorRumbleMotorPresent);
			}
		}

		public override bool IsTriggerRumbleMotorPresent
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.IsTriggerRumbleMotorPresent);
			}
		}

		public override bool IsAdaptiveTriggersPresent
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.IsAdaptiveTriggersPresent);
			}
		}

		public override bool IsSteamExtendedPresent
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.IsSteamExtendedPresent);
			}
		}

		public override bool IsPowerManagementPresent
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.IsPowerManagementPresent);
			}
		}

		public override bool IsAccelerometerPresent
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.IsAccelerometerPresent);
			}
		}

		public override bool IsGyroscopePresent
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.IsGyroscopePresent);
			}
		}

		public override bool IsTouchpadPresent
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.IsTouchpadPresent);
			}
		}

		public override bool IsTiltMode2
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.IsTiltMode2);
			}
		}

		public override bool IsControllerExclusiveAccessSupported
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.IsControllerExclusiveAccessSupported);
			}
		}

		public override bool IsRightHandDevice
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.IsRightHandDevice);
			}
		}

		public override Drawing MiniGamepadSVGIco
		{
			get
			{
				if (this.IsNintendoSwitchJoyConComposite)
				{
					return Application.Current.TryFindResource("MiniGamepadNJCon") as Drawing;
				}
				return Application.Current.TryFindResource("MiniGamepadCompositeDevice") as Drawing;
			}
		}

		private string FormatControllerTypeFriendlyName(BaseControllerVM item)
		{
			if (!item.IsOnline)
			{
				return "<font color=\"#858585\">" + item.ControllerDisplayName + "</font>";
			}
			return item.ControllerDisplayName;
		}

		public override string ControllerTypeFriendlyName
		{
			get
			{
				string defaultTypeBasedName = "";
				bool? isPreviousOnline = null;
				this.BaseControllers.ForEach(delegate(BaseControllerVM item)
				{
					if (item != null)
					{
						if (!string.IsNullOrEmpty(defaultTypeBasedName))
						{
							if ((isPreviousOnline == null && !item.IsOnline) || (isPreviousOnline != null && !isPreviousOnline.Value && !item.IsOnline))
							{
								defaultTypeBasedName += "<font color=\"#858585\"> + </font>";
							}
							else
							{
								defaultTypeBasedName += " + ";
							}
							isPreviousOnline = new bool?(item.IsOnline);
						}
						defaultTypeBasedName += this.FormatControllerTypeFriendlyName(item);
					}
				});
				if (!string.IsNullOrEmpty(defaultTypeBasedName))
				{
					return defaultTypeBasedName;
				}
				ControllerVM currentController = this.CurrentController;
				if (currentController == null)
				{
					return null;
				}
				return currentController.ControllerDisplayName;
			}
			set
			{
			}
		}

		public override string ControllerDisplayName
		{
			get
			{
				if (this.BaseControllers.Any((BaseControllerVM item) => item is OfflineDeviceVM))
				{
					return this.ControllerTypeFriendlyName;
				}
				return base.ControllerDisplayName;
			}
		}

		public override List<BaseControllerVM> GetLEDSupportedControllers()
		{
			List<BaseControllerVM> list = new List<BaseControllerVM>();
			foreach (BaseControllerVM baseControllerVM in this.BaseControllers)
			{
				if (baseControllerVM != null)
				{
					list.AddRange(baseControllerVM.GetLEDSupportedControllers());
				}
			}
			return list;
		}

		public override void SetCurrentSlot(Slot newSlot)
		{
			base.SetCurrentSlot(newSlot);
		}

		public override async Task<GamepadState> GetControllerPressedButtons()
		{
			GamepadState gamepadState = new GamepadState();
			foreach (BaseControllerVM baseControllerVM in this.BaseControllers)
			{
				if (baseControllerVM != null)
				{
					GamepadState gamepadState2 = gamepadState;
					GamepadState gamepadState3 = await baseControllerVM.GetControllerPressedButtons();
					gamepadState2.Add(gamepadState3);
					gamepadState2 = null;
				}
			}
			List<BaseControllerVM>.Enumerator enumerator = default(List<BaseControllerVM>.Enumerator);
			return gamepadState;
		}

		public override async Task<Slot?> GetActivePhyscialSlot()
		{
			foreach (BaseControllerVM baseControllerVM in this.BaseControllers)
			{
				if (baseControllerVM != null)
				{
					Slot? slot = await baseControllerVM.GetActivePhyscialSlot();
					if (slot != null)
					{
						return slot;
					}
				}
			}
			List<BaseControllerVM>.Enumerator enumerator = default(List<BaseControllerVM>.Enumerator);
			return null;
		}

		public override async Task<bool> GetIsControllerPressedButton()
		{
			foreach (BaseControllerVM baseControllerVM in this.BaseControllers)
			{
				bool flag = baseControllerVM != null;
				if (flag)
				{
					flag = await baseControllerVM.GetIsControllerPressedButton();
				}
				if (flag)
				{
					return true;
				}
			}
			List<BaseControllerVM>.Enumerator enumerator = default(List<BaseControllerVM>.Enumerator);
			return false;
		}

		public override void AddController(BaseControllerVM controller)
		{
			int i = 0;
			while (i < 4)
			{
				if (!(IControllerProfileInfoCollectionContainerExtensions.CalculateID(base.ControllerProfileInfoCollections[i]) == controller.ID))
				{
					string id = controller.ID;
					BaseControllerVM baseControllerVM = this.BaseControllers[i];
					if (!(id == ((baseControllerVM != null) ? baseControllerVM.ID : null)))
					{
						i++;
						continue;
					}
				}
				switch (i)
				{
				case 0:
					this.Controller1 = controller;
					break;
				case 1:
					this.Controller2 = controller;
					break;
				case 2:
					this.Controller3 = controller;
					break;
				case 3:
					this.Controller4 = controller;
					break;
				}
				controller.IsInsideCompositeDevice = true;
				break;
			}
			base.IsOnline = this.BaseControllers.Any((BaseControllerVM c) => c != null && c.IsOnline);
			if (this.CurrentController == null)
			{
				this.CurrentController = controller.CurrentController;
			}
		}

		private bool SetController(ref BaseControllerVM storage, BaseControllerVM value, int controllerIndex, [CallerMemberName] string propertyName = null)
		{
			if (storage != value)
			{
				storage = value;
				return true;
			}
			return false;
		}

		public void SetCurrentControllerAccordingControllerFamilyIndex(ControllerFamily controllerFamily, int index)
		{
			List<BaseControllerVM> list = this.BaseControllers.Where((BaseControllerVM c) => c != null && c.ControllerFamily == controllerFamily).ToList<BaseControllerVM>();
			if (list.Count > index)
			{
				this.CurrentController = list[index].CurrentController;
				return;
			}
			this.CurrentController = null;
		}

		private string _id;

		private string _shortId;

		private uint[] _types;

		private ulong[] _ids;

		private ControllerVM _currentControllerVM;

		private BaseControllerVM _controller1;

		private BaseControllerVM _controller2;

		private BaseControllerVM _controller3;

		private BaseControllerVM _controller4;
	}
}
