using System;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Infrastructure.Controller
{
	public class PromoDeviceVM : ControllerVM
	{
		public PromoDeviceVM(ControllerTypeEnum controllerType, string controllerFriendlyName)
		{
			ulong num = 1UL;
			base.ControllerFriendlyName = controllerFriendlyName;
			this.ID = num.ToString();
			base.ControllerType = controllerType;
			base.Types = new uint[] { ControllerTypeExtensions.ConvertEnumToPhysicalType(controllerType) };
			base.ControllerBatteryKind = 0;
			base.ControllerBatteryLevel = 3;
			base.ContainerId = Guid.NewGuid();
			base.IsOnline = true;
			base.Ids = new ulong[] { num };
			this.SetIsInitialized(false);
		}

		public override bool HasEngineMouseControllers
		{
			get
			{
				return false;
			}
		}

		public override bool HasEngineMouseTouchpadControllers
		{
			get
			{
				return false;
			}
		}

		public override bool HasEngineControllers
		{
			get
			{
				return false;
			}
		}

		public override bool IsPromoController
		{
			get
			{
				return true;
			}
		}

		public override ControllerVM CurrentController
		{
			get
			{
				return this;
			}
			set
			{
			}
		}
	}
}
