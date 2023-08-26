using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DiscSoftReWASDServiceNamespace;
using reWASDCommon.Infrastructure.Enums;
using reWASDEngine;
using XBEliteWPF.DataModels.PeripheralDevicesCollection;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtil;
using XBEliteWPF.Utils.XBUtilModel;

namespace XBEliteWPF.Infrastructure.Controller
{
	public class PeripheralVM : BasePeripheralVM
	{
		public override bool HasXboxElite
		{
			get
			{
				return this.Types.Any((uint t) => t == ControllerTypeExtensions.ConvertEnumToPhysicalType(3) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(12));
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
				this._currentControllerVM = value;
			}
		}

		public override string ID
		{
			get
			{
				if (this.BaseControllers.Count == 0)
				{
					if (!string.IsNullOrEmpty(this._savedID))
					{
						return this._savedID;
					}
					return this.ContainerId.ToString();
				}
				else
				{
					if (this.BaseControllers.Count == 1)
					{
						if (string.IsNullOrEmpty(this._savedID))
						{
							this._savedID = this.BaseControllers[0].ID;
						}
						return this.BaseControllers[0].ID;
					}
					this._savedID = this.CalcID();
					return this._savedID;
				}
			}
			set
			{
			}
		}

		public string CalcID()
		{
			string id = string.Empty;
			this.BaseControllers.ForEach(delegate(BaseControllerVM c)
			{
				id = id + c.ID + ";";
			});
			return id.TrimEnd(';');
		}

		public override string ShortID
		{
			get
			{
				return XBUtils.CalcShortID(this.ID);
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
						text = text + "ID: " + text2 + "\n";
					}
					text.TrimEnd('\n');
				}
				return text;
			}
		}

		public override uint[] Types
		{
			get
			{
				return this._types.ToArray();
			}
			set
			{
				this._types = value.ToList<uint>();
			}
		}

		public override ulong[] Ids
		{
			get
			{
				return this._ids.ToArray();
			}
			set
			{
				this._ids = value.ToList<ulong>();
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

		public override bool HasAnyEngineControllers
		{
			get
			{
				return base.Controllers.Any((ControllerVM c) => c != null && (c.IsEngineKeyboard || c.IsAnyEngineMouse));
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
				if (base.PeripheralPhysicalType != 1 || !this.IsRazer)
				{
					return base.Controllers.Any((ControllerVM c) => c != null && c.IsEngineKeyboard);
				}
				return true;
			}
		}

		public override bool HasLEDMouse
		{
			get
			{
				if (base.PeripheralPhysicalType != 2 || !this.IsRazer)
				{
					return base.Controllers.Any((ControllerVM c) => c != null && c.IsEngineMouse);
				}
				return true;
			}
		}

		public override bool CanReinitializeController
		{
			get
			{
				return Engine.GamepadService.PeripheralDevices.Any((PeripheralDevice p) => p.ID == this.ID);
			}
		}

		public bool IsRazer
		{
			get
			{
				return base.Controllers.Any((ControllerVM c) => c != null && c.IsRazer);
			}
		}

		public override List<BaseControllerVM> BaseControllers { get; set; } = new List<BaseControllerVM>();

		public override bool IsAvailiableForComposition
		{
			get
			{
				return base.IsAvailiableForComposition && this.IsInitializedController;
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

		public PeripheralVM(Guid containerId, bool isDebugController = false)
			: base(isDebugController)
		{
			this._types = new List<uint>();
			this._ids = new List<ulong>();
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

		public override void AddController(BaseControllerVM baseController)
		{
			ControllerVM controllerVM = baseController as ControllerVM;
			if (controllerVM != null)
			{
				if (controllerVM.ContainerId != this.ContainerId)
				{
					return;
				}
				if (controllerVM.ContainerId == this.ContainerId && (controllerVM.IsKeyboard || controllerVM.IsMouse || controllerVM.IsConsumer || controllerVM.IsSystem))
				{
					this.BaseControllers.Add(controllerVM);
					this._types.Add(controllerVM.Type);
					this._ids.Add(controllerVM.ControllerId);
					this.UpdatePeripheralType(controllerVM.ControllerType);
				}
				if (this.CurrentController == null || this.CurrentController.ControllerFamily != this.ControllerFamily)
				{
					this.CurrentController = controllerVM;
				}
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

		public override bool UpdateControllerInfo(REWASD_CONTROLLER_INFO controllerInfo)
		{
			BaseControllerVM baseControllerVM = base.FindController(controllerInfo.Id);
			bool? flag = ((baseControllerVM != null) ? new bool?(baseControllerVM.UpdateControllerInfo(controllerInfo)) : null);
			if (this.HasChatpadControllers && this._isInitializedChatpad == null)
			{
				this._isInitializedChatpad = new bool?(false);
			}
			bool? flag2 = flag;
			bool flag3 = true;
			return (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
		}

		public override void UpdateControllerState(REWASD_GET_CONTROLLER_STATE_RESPONSE controllerState)
		{
		}

		public override async Task<bool> GetIsControllerPressedButton()
		{
			bool flag;
			if (!base.IsOnline || base.IsDebugController)
			{
				flag = false;
			}
			else
			{
				int i = 0;
				bool result = false;
				foreach (uint num in this._types)
				{
					if (i < this._ids.Count)
					{
						ulong num2 = this._ids[i];
						TaskAwaiter<bool> taskAwaiter = Engine.XBServiceCommunicator.IsControllerPressedButton(num2, num, ControllerTypeExtensions.ConvertPhysicalTypeToEnum(0, num, num2)).GetAwaiter();
						if (!taskAwaiter.IsCompleted)
						{
							await taskAwaiter;
							TaskAwaiter<bool> taskAwaiter2;
							taskAwaiter = taskAwaiter2;
							taskAwaiter2 = default(TaskAwaiter<bool>);
						}
						if (taskAwaiter.GetResult())
						{
							result = true;
						}
					}
					i++;
				}
				List<uint>.Enumerator enumerator = default(List<uint>.Enumerator);
				flag = result;
			}
			return flag;
		}

		public override string ControllerTypeFriendlyName
		{
			get
			{
				string controllerSavedFriendlyName = XBUtils.GetControllerSavedFriendlyName(this.ShortID);
				if (!string.IsNullOrEmpty(controllerSavedFriendlyName))
				{
					return controllerSavedFriendlyName;
				}
				ControllerVM controllerVM = base.Controllers.FirstOrDefault<ControllerVM>();
				if (controllerVM == null)
				{
					return null;
				}
				return controllerVM.ContainerDescription;
			}
			set
			{
			}
		}

		public override Drawing MiniGamepadSVGIco
		{
			get
			{
				if (this.IsNonInitializedPeripheralController)
				{
					return Application.Current.TryFindResource("MiniGamepadKeyboardUnverified") as Drawing;
				}
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

		private List<uint> _types;

		private List<ulong> _ids;

		private ControllerVM _currentControllerVM;

		private string _savedID;

		private bool? _isInitializedChatpad;
	}
}
