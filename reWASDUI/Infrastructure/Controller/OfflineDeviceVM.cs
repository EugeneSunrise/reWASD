using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoftReWASDServiceNamespace;
using XBEliteWPF.DataModels.ControllerProfileInfo;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Infrastructure.Controller
{
	public class OfflineDeviceVM : ControllerVM
	{
		public ControllerProfileInfoCollection ControllerProfileInfoCollection { get; set; }

		public OfflineDeviceVM(ControllerProfileInfoCollection controllerProfileInfoCollection)
		{
			this.ControllerProfileInfoCollection = controllerProfileInfoCollection;
			base.ControllerFriendlyName = this.ControllerProfileInfoCollection.ControllerProfileInfos[0].ControllerType.TryGetDescription();
			this.ID = IControllerProfileInfoCollectionContainerExtensions.CalculateID(this.ControllerProfileInfoCollection);
			base.ControllerType = this.ControllerProfileInfoCollection.ControllerProfileInfos[0].ControllerType;
			base.Types = this.ControllerProfileInfoCollection.ControllerProfileInfos.Select((ControllerProfileInfo item) => ControllerTypeExtensions.ConvertEnumToPhysicalType(item.ControllerType)).ToArray<uint>();
		}

		public override Drawing MiniGamepadSVGIco
		{
			get
			{
				return null;
			}
		}

		public bool IsOfflineDevice
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

		public override bool IsGyroscopePresent
		{
			get
			{
				return ControllerTypeExtensions.IsGyroAvailiable(base.ControllerType);
			}
		}

		public override Task<GamepadState> GetControllerPressedButtons()
		{
			return Task.FromResult<GamepadState>(new GamepadState());
		}

		public override List<BaseControllerVM> GetLEDSupportedControllers()
		{
			throw new NotImplementedException();
		}

		public override void UpdateControllerState(REWASD_GET_CONTROLLER_STATE_RESPONSE controllerState)
		{
		}

		protected override void Vibrate()
		{
		}
	}
}
