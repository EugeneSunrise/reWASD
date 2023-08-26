using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using DiscSoftReWASDServiceNamespace;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Infrastructure.Controller
{
	public class PeripheralVM : BasePeripheralVM
	{
		public override bool HasXboxElite
		{
			get
			{
				return base.Types.Any((uint t) => t == ControllerTypeExtensions.ConvertEnumToPhysicalType(3) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(12));
			}
		}

		public override ControllerVM CurrentController
		{
			get
			{
				return this._currentControllerVM;
			}
			set
			{
				this.SetProperty<ControllerVM>(ref this._currentControllerVM, value, "CurrentController");
			}
		}

		public override string ShortID
		{
			get
			{
				return this.ID;
			}
		}

		public override string HintID
		{
			get
			{
				string id = this.ID;
				string text = string.Empty;
				if (!string.IsNullOrEmpty(this.ID))
				{
					foreach (string text2 in id.Split(';', StringSplitOptions.None))
					{
						try
						{
							string text3 = text;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 1);
							defaultInterpolatedStringHandler.AppendLiteral("ID: ");
							defaultInterpolatedStringHandler.AppendFormatted<ulong>(Convert.ToUInt64(text2), "X16");
							defaultInterpolatedStringHandler.AppendLiteral("\n");
							text = text3 + defaultInterpolatedStringHandler.ToStringAndClear();
						}
						catch
						{
						}
					}
					text.TrimEnd('\n');
				}
				return text;
			}
		}

		public override ControllerFamily ControllerFamily
		{
			get
			{
				PeripheralPhysicalType peripheralPhysicalType = base.PeripheralPhysicalType;
				if (peripheralPhysicalType == 1)
				{
					return 1;
				}
				if (peripheralPhysicalType != 2)
				{
					return 4;
				}
				return 2;
			}
		}

		public override bool HasKeyboardControllers
		{
			get
			{
				return base.Controllers.Any((ControllerVM c) => c != null && c.IsKeyboard);
			}
		}

		public override bool HasAnyKeyboardControllers
		{
			get
			{
				return base.Controllers.Any((ControllerVM c) => c != null && c.IsAnyKeyboard);
			}
		}

		public override bool HasChatpadControllers
		{
			get
			{
				return base.Controllers.Any((ControllerVM c) => c != null && c.IsChatpad);
			}
		}

		public override bool HasMouseControllers
		{
			get
			{
				return base.Controllers.Any((ControllerVM c) => c != null && c.IsMouse);
			}
		}

		public override bool HasMouseControllersWithKeypad
		{
			get
			{
				if (base.Controllers.Any((ControllerVM c) => c != null && c.IsMouse))
				{
					return base.Controllers.Any((ControllerVM c) => c != null && c.IsKeyboard);
				}
				return false;
			}
		}

		public override bool HasMouseControllersWithAnyKeypad
		{
			get
			{
				if (base.Controllers.Any((ControllerVM c) => c != null && c.IsMouse))
				{
					return base.Controllers.Any((ControllerVM c) => c != null && c.IsAnyKeyboard);
				}
				return false;
			}
		}

		public override bool HasConsumerControllers
		{
			get
			{
				return base.Controllers.Any((ControllerVM c) => c != null && (c.IsConsumer || c.IsKeyboardPS2));
			}
		}

		public override bool HasSystemControllers
		{
			get
			{
				return base.Controllers.Any((ControllerVM c) => c != null && (c.IsSystem || c.IsKeyboardPS2));
			}
		}

		public override bool HasLEDKeyboard
		{
			get
			{
				return base.PeripheralPhysicalType == 1 && this.IsRazer;
			}
		}

		public override bool HasLEDMouse
		{
			get
			{
				return base.PeripheralPhysicalType == 2 && this.IsRazer;
			}
		}

		public bool IsRazer
		{
			get
			{
				return base.Controllers.Any((ControllerVM c) => c != null && c.IsRazer);
			}
		}

		public override bool IsAvailiableForComposition
		{
			get
			{
				return base.IsAvailiableForComposition && base.IsInitializedController;
			}
		}

		public override List<LEDSupportedDevice> GetLEDSupportedDevicesEnum()
		{
			List<LEDSupportedDevice> list = new List<LEDSupportedDevice>();
			if (base.PeripheralPhysicalType == 1 && this.HasLEDKeyboard)
			{
				list.Add(8);
			}
			if (base.PeripheralPhysicalType == 2 && this.HasLEDMouse)
			{
				list.Add(9);
			}
			return list;
		}

		protected override void OnOnlineChanged()
		{
			List<ControllerVM> controllers = base.Controllers;
			if (controllers == null)
			{
				return;
			}
			controllers.ForEach(delegate(ControllerVM controller)
			{
				controller.IsOnline = base.IsOnline;
			});
		}

		public PeripheralVM(Guid containerId)
		{
			this._containerId = containerId;
		}

		public bool InitPeripheralPhysicalType()
		{
			if (base.Controllers.Count > 0)
			{
				if (base.Controllers.Any((ControllerVM c) => c.IsKeyboard))
				{
					if (base.Controllers.All((ControllerVM c) => !c.IsMouse))
					{
						base.PeripheralPhysicalType = 1;
						return true;
					}
				}
				if (base.Controllers.Any((ControllerVM c) => c.IsMouse))
				{
					if (base.Controllers.All((ControllerVM c) => !c.IsKeyboard))
					{
						base.PeripheralPhysicalType = 2;
						return true;
					}
				}
			}
			return false;
		}

		private void UpdatePeripheralType(ControllerTypeEnum controllerType)
		{
			if (ControllerTypeExtensions.IsGamepad(controllerType))
			{
				return;
			}
			byte b = base.PeripheralType;
			if (controllerType != 503)
			{
				if (controllerType - 504 > 1)
				{
					switch (controllerType)
					{
					case 1000:
						goto IL_41;
					case 1001:
						break;
					case 1002:
						b |= 4;
						goto IL_5B;
					case 1003:
						b |= 8;
						goto IL_5B;
					default:
						goto IL_5B;
					}
				}
				b |= 2;
				goto IL_5B;
			}
			IL_41:
			b |= 1;
			IL_5B:
			base.PeripheralType = b;
		}

		public override void UpdateControllerInfo()
		{
			BaseControllerVM baseControllerVM = base.FindController(this.ID);
			if (baseControllerVM != null)
			{
				baseControllerVM.UpdateControllerInfo();
			}
			if (this.HasChatpadControllers && this._isInitializedChatpad == null)
			{
				this._isInitializedChatpad = new bool?(false);
			}
		}

		public override void UpdateControllerState(REWASD_GET_CONTROLLER_STATE_RESPONSE controllerState)
		{
		}

		public override Drawing MiniGamepadSVGIco
		{
			get
			{
				if (this.IsNonInitializedPeripheralController)
				{
					return Application.Current.TryFindResource("MiniGamepadKeyboardUnverified") as Drawing;
				}
				switch (base.FirstControllerType)
				{
				case 503:
					return Application.Current.TryFindResource("MiniEngineControllerMobileKeyboard") as Drawing;
				case 504:
					return Application.Current.TryFindResource("MiniEngineControllerMobileMouse") as Drawing;
				case 505:
					return Application.Current.TryFindResource("MiniEngineControllerMouseTouchpad") as Drawing;
				default:
					switch (base.PeripheralType)
					{
					case 0:
					case 4:
					case 8:
					case 12:
						return Application.Current.TryFindResource("MiniGamepadExclamation") as Drawing;
					case 1:
					case 9:
						return Application.Current.TryFindResource("MiniGamepadKeyboard") as Drawing;
					case 2:
						return Application.Current.TryFindResource("MiniGamepadMouse5btn") as Drawing;
					case 3:
						if (base.PeripheralPhysicalType == 2)
						{
							return Application.Current.TryFindResource("MiniGamepadMouse7btn") as Drawing;
						}
						return Application.Current.TryFindResource("MiniGamepadKeyboardMouse") as Drawing;
					case 5:
					case 13:
						return Application.Current.TryFindResource("MiniGamepadKeyboardMedia") as Drawing;
					case 6:
					case 10:
					case 14:
						return Application.Current.TryFindResource("MiniGamepadMouse7btn") as Drawing;
					case 7:
					case 11:
					case 15:
						if (base.PeripheralPhysicalType == 2)
						{
							return Application.Current.TryFindResource("MiniGamepadMouse7btn") as Drawing;
						}
						return Application.Current.TryFindResource("MiniGamepadKeyboardMediaMouse") as Drawing;
					default:
						return Application.Current.TryFindResource("MiniGamepadKeyboardUnverified") as Drawing;
					}
					break;
				}
			}
		}

		public override List<BaseControllerVM> GetLEDSupportedControllers()
		{
			if (this.HasKeyboardControllers || this.HasMouseControllers)
			{
				return new List<BaseControllerVM> { this };
			}
			return new List<BaseControllerVM>();
		}

		public override bool IsConsideredTheSameControllerByID(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return false;
			}
			if (base.IsConsideredTheSameControllerByID(id))
			{
				return true;
			}
			if (id.Contains(';'))
			{
				string[] array = id.Split(';', StringSplitOptions.None);
				array = array.OrderBy((string c) => c).ToArray<string>();
				string[] array2 = (from c in base.Controllers
					select c.ID into c
					orderby c
					select c).ToArray<string>();
				return array.Length == array2.Length && array2.SequenceEqual(array);
			}
			return false;
		}

		public virtual bool IsConsideredTheSameControllerByID(ulong id)
		{
			return this.ID.Contains(id.ToString());
		}

		public override bool IsNonInitializedChatpad
		{
			get
			{
				bool? isInitializedChatpad = this._isInitializedChatpad;
				bool flag = true;
				return !((isInitializedChatpad.GetValueOrDefault() == flag) & (isInitializedChatpad != null)) && this.HasChatpadControllers;
			}
		}

		public override void SetIsInitialized(bool value)
		{
			this._isInitializedChatpad = new bool?(value);
			this.OnPropertyChanged("IsNonInitializedChatpad");
			this.OnPropertyChanged("IsInitializedController");
		}

		public BatteryLevel ControllerBatteryLevel
		{
			get
			{
				return 4;
			}
		}

		public bool IsControllerBatteryBlockVisible
		{
			get
			{
				return false;
			}
		}

		public bool IsConnectionWired
		{
			get
			{
				return true;
			}
		}

		private ControllerVM _currentControllerVM;

		private bool? _isInitializedChatpad;
	}
}
