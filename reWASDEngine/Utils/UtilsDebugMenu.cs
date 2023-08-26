using System;
using System.Linq;
using DiscSoftReWASDServiceNamespace;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDEngine.Utils
{
	public static class UtilsDebugMenu
	{
		public static ControllerVM CreatePromoGamepadVM(uint controllerType, string name)
		{
			REWASD_CONTROLLER_INFO rewasd_CONTROLLER_INFO = REWASD_CONTROLLER_INFO.CreateBlankInstance();
			rewasd_CONTROLLER_INFO.Type = controllerType;
			rewasd_CONTROLLER_INFO.BatteryLevel = 3;
			rewasd_CONTROLLER_INFO.BatteryKind = 0;
			rewasd_CONTROLLER_INFO.ContainerId = Guid.NewGuid();
			rewasd_CONTROLLER_INFO.Id = XBUtils.RNGRandomULong();
			ControllerVM controllerVM = new ControllerVM(rewasd_CONTROLLER_INFO, true);
			controllerVM.IsPromoController = true;
			controllerVM.ControllerFriendlyName = name;
			controllerVM.SetIsInitialized(false);
			return controllerVM;
		}

		internal static void DeletePromoController(string id)
		{
			BaseControllerVM baseControllerVM = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM x) => x.ID == id && x.IsPromoController);
			Engine.GamepadService.GamepadCollection.Remove(baseControllerVM);
		}
	}
}
