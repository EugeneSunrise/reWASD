using System;
using System.Collections.Generic;
using System.Linq;
using DiscSoftReWASDServiceNamespace;
using Newtonsoft.Json;
using reWASDCommon.Network.HTTP.Interfaces.Controllers;
using XBEliteWPF.DataModels.ControllerProfileInfo;

namespace XBEliteWPF.Infrastructure.Controller
{
	[JsonObject(1)]
	public abstract class BaseCompositeControllerVM : BaseControllerVM, IBaseCompositeController<BaseControllerVM>, IBaseController, IControllerProfileInfoCollectionContainer
	{
		public bool HasControllers
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => !(c is OfflineDeviceVM) && c != null);
			}
		}

		public bool HasOnlineDevice
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => !(c is OfflineDeviceVM) && c != null && c.IsOnline);
			}
		}

		public override bool HasLEDGamepad
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.HasLEDGamepad);
			}
		}

		public override bool HasLEDKeyboard
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.HasLEDKeyboard);
			}
		}

		public override bool HasLEDMouse
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.HasLEDMouse);
			}
		}

		public override bool HasAnyEngineControllers
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.HasAnyEngineControllers);
			}
		}

		public override bool HasGamepadControllers
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.HasGamepadControllers);
			}
		}

		public override bool HasGamepadControllersWithFictiveButtons
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.HasGamepadControllersWithFictiveButtons);
			}
		}

		public override bool HasGamepadVibrateControllers
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.HasGamepadVibrateControllers);
			}
		}

		public override bool HasOnlineGamepadVibrateControllers
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.HasOnlineGamepadVibrateControllers);
			}
		}

		public override bool HasKeyboardControllers
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.HasKeyboardControllers);
			}
		}

		public override bool HasAnyKeyboardControllers
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.HasAnyKeyboardControllers);
			}
		}

		public override bool HasChatpadControllers
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.HasChatpadControllers);
			}
		}

		public override bool HasMouseControllers
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.HasMouseControllers);
			}
		}

		public override bool HasMouseControllersWithKeypad
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.ControllerFamily == 2 && c.HasKeyboardControllers);
			}
		}

		public override bool HasMouseControllersWithAnyKeypad
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.ControllerFamily == 2 && c.HasAnyKeyboardControllers);
			}
		}

		public override bool HasGamepadControllersWithBuiltInAnyKeypad
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.ControllerFamily == null && c.HasGamepadControllersWithBuiltInAnyKeypad);
			}
		}

		public override bool HasConsumerControllers
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.HasConsumerControllers);
			}
		}

		public override bool HasSystemControllers
		{
			get
			{
				return this.BaseControllers.Any((BaseControllerVM c) => c != null && c.HasSystemControllers);
			}
		}

		public override int ConsistsOfControllersNumber
		{
			get
			{
				return this.BaseControllers.Count<BaseControllerVM>();
			}
		}

		[JsonProperty("BaseControllers")]
		public abstract List<BaseControllerVM> BaseControllers { get; set; }

		public List<ControllerVM> Controllers
		{
			get
			{
				return this.BaseControllers.OfType<ControllerVM>().ToList<ControllerVM>();
			}
		}

		public override Guid ContainerId
		{
			get
			{
				return this._containerId;
			}
			set
			{
				this._containerId = value;
			}
		}

		public ControllerProfileInfoCollection[] ControllerProfileInfoCollections { get; set; }

		protected BaseCompositeControllerVM(bool isDebugController = false)
			: base(isDebugController)
		{
		}

		public void UpdateControllerState(REWASD_GET_CONTROLLER_STATE_RESPONSE controllerState, ulong controllerId)
		{
			BaseControllerVM baseControllerVM = this.FindController(controllerId);
			if (baseControllerVM == null)
			{
				return;
			}
			baseControllerVM.UpdateControllerState(controllerState);
		}

		public abstract void AddController(BaseControllerVM controller);

		public bool HasControllerWithId(ulong controllerId)
		{
			return this.FindController(controllerId) != null;
		}

		public bool HasControllerWithId(string ID)
		{
			return this.FindController(ID) != null;
		}

		public BaseControllerVM FindController(ulong controllerId)
		{
			return this.FindController(controllerId.ToString());
		}

		public BaseControllerVM FindController(string ID)
		{
			return this.BaseControllers.FirstOrDefault((BaseControllerVM c) => c != null && c.ID == ID);
		}

		public override bool IsConsideredTheSameControllerByID(ulong[] IDs)
		{
			if (IDs.Length != 0)
			{
				ulong[] array = (from i in IDs
					where i > 0UL
					orderby i descending
					select i).ToArray<ulong>();
				ulong[] array2 = (from c in this.Controllers
					select c.ControllerId into c
					orderby c descending
					select c).ToArray<ulong>();
				return array.Length == array2.Length && array2.SequenceEqual(array);
			}
			return false;
		}

		protected Guid _containerId;
	}
}
