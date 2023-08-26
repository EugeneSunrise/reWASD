using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure;

namespace reWASDUI.Infrastructure.Controller
{
	public abstract class BasePeripheralVM : BaseCompositeControllerVM
	{
		[JsonProperty("PeripheralPhysicalType")]
		[JsonConverter(typeof(StringEnumConverter))]
		public PeripheralPhysicalType PeripheralPhysicalType
		{
			get
			{
				return this._peripheralPhysicalType;
			}
			set
			{
				if (this.SetProperty<PeripheralPhysicalType>(ref this._peripheralPhysicalType, value, "PeripheralPhysicalType"))
				{
					if (value != 1)
					{
						if (value == 2)
						{
							List<ControllerVM> controllers = base.Controllers;
							ControllerVM controllerVM;
							if (controllers == null)
							{
								controllerVM = null;
							}
							else
							{
								controllerVM = controllers.FirstOrDefault((ControllerVM c) => c.IsMouse);
							}
							this.CurrentController = controllerVM;
						}
					}
					else
					{
						List<ControllerVM> controllers2 = base.Controllers;
						ControllerVM controllerVM2;
						if (controllers2 == null)
						{
							controllerVM2 = null;
						}
						else
						{
							controllerVM2 = controllers2.FirstOrDefault((ControllerVM c) => c.IsKeyboard);
						}
						this.CurrentController = controllerVM2;
					}
					this.OnPropertyChanged("IsInitializedController");
					this.OnPropertyChanged("IsNonInitializedPeripheralController");
					this.OnPropertyChanged("ControllerFamily");
				}
			}
		}

		[JsonProperty("PeripheralType")]
		[JsonConverter(typeof(StringEnumConverter))]
		public PeripheralType PeripheralType
		{
			get
			{
				return this._peripheralType;
			}
			set
			{
				this.SetProperty<PeripheralType>(ref this._peripheralType, value, "PeripheralType");
			}
		}

		[JsonProperty("IsNonInitializedPeripheralController")]
		public override bool IsNonInitializedPeripheralController
		{
			get
			{
				return this.PeripheralPhysicalType == 0;
			}
		}

		protected override void Vibrate()
		{
		}

		public override Task<GamepadState> GetControllerPressedButtons()
		{
			return Task.FromResult<GamepadState>(new GamepadState());
		}

		private PeripheralPhysicalType _peripheralPhysicalType;

		private PeripheralType _peripheralType;

		protected Guid _containerId;
	}
}
