using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DiscSoftReWASDServiceNamespace;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.DataModels.CompositeDevicesCollection;
using XBEliteWPF.DataModels.ControllerProfileInfo;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Infrastructure.Controller
{
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
				if (this.SetProperty<ControllerVM>(ref this._currentControllerVM, value, "CurrentController"))
				{
					EventHandler<ControllerVM> currentControllerChanged = this.CurrentControllerChanged;
					if (currentControllerChanged == null)
					{
						return;
					}
					currentControllerChanged(this, value);
				}
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
				if (base.IsNintendoSwitchJoyConComposite)
				{
					return 0;
				}
				return base.TreatAsControllerFamily;
			}
		}

		public bool IsNintendoSwitchJoyConLExpected
		{
			get
			{
				return base.Types.Any((uint t) => t == ControllerTypeExtensions.ConvertEnumToPhysicalType(9));
			}
		}

		public bool IsNintendoSwitchJoyConRExpected
		{
			get
			{
				return base.Types.Any((uint t) => t == ControllerTypeExtensions.ConvertEnumToPhysicalType(10));
			}
		}

		public override bool IsEngineMouseKeyboardComposite
		{
			get
			{
				if (base.BaseControllers.Where((BaseControllerVM bc) => bc != null).Count<BaseControllerVM>() == 2)
				{
					if (base.BaseControllers.Any(delegate(BaseControllerVM t)
					{
						uint? num = ((t != null) ? new uint?(t.FirstType) : null);
						uint num2 = ControllerTypeExtensions.ConvertEnumToPhysicalType(503);
						return (num.GetValueOrDefault() == num2) & (num != null);
					}))
					{
						return base.BaseControllers.Any(delegate(BaseControllerVM x)
						{
							uint? num3 = ((x != null) ? new uint?(x.FirstType) : null);
							uint num4 = ControllerTypeExtensions.ConvertEnumToPhysicalType(504);
							return (num3.GetValueOrDefault() == num4) & (num3 != null);
						});
					}
				}
				return false;
			}
		}

		public override bool HasXboxElite
		{
			get
			{
				return base.Types.Any((uint t) => t == ControllerTypeExtensions.ConvertEnumToPhysicalType(3) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(12));
			}
		}

		public override bool HasXboxElite2
		{
			get
			{
				return base.Types.Any((uint t) => t == ControllerTypeExtensions.ConvertEnumToPhysicalType(12));
			}
		}

		public override bool HasExclusiveCaptureControllers
		{
			get
			{
				return base.Types.Any((uint t) => t == ControllerTypeExtensions.ConvertEnumToPhysicalType(16) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(63) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(19) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(28) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(54));
			}
		}

		public override bool HasTouchpad
		{
			get
			{
				return base.Types.Any((uint t) => t == ControllerTypeExtensions.ConvertEnumToPhysicalType(16) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(63) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(4) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(14) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(20) || t == ControllerTypeExtensions.ConvertEnumToPhysicalType(21));
			}
		}

		public List<BaseControllerVM> NonNullBaseControllers
		{
			get
			{
				return base.BaseControllers.Where((BaseControllerVM bc) => bc != null).ToList<BaseControllerVM>();
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

		public override bool IsGyroscopePresent
		{
			get
			{
				return base.BaseControllers.Any((BaseControllerVM c) => c != null && c.IsGyroscopePresent);
			}
		}

		public event EventHandler<ControllerVM> CurrentControllerChanged;

		public CompositeControllerVM(CompositeDevice compositeDevice)
		{
		}

		public override void Init()
		{
			if (this._prevIndex == -1)
			{
				BaseControllerVM baseControllerVM = base.BaseControllers.FirstOrDefault<BaseControllerVM>();
				this.CurrentController = ((baseControllerVM != null) ? baseControllerVM.CurrentController : null);
			}
			else
			{
				this.SetCurrentControllerAccordingControllerFamilyIndex(this._prevControllerFamily, this._prevIndex);
			}
			base.FillFriendlyName();
		}

		private BaseControllerVM SetOfflineController(ControllerProfileInfoCollection ControllerProfileInfoCollectoin)
		{
			if (!string.IsNullOrEmpty(IControllerProfileInfoCollectionContainerExtensions.CalculateID(ControllerProfileInfoCollectoin.ControllerProfileInfos)))
			{
				return new OfflineDeviceVM(ControllerProfileInfoCollectoin);
			}
			return null;
		}

		public override void VibrateForce()
		{
			base.BaseControllers.ForEach(delegate(BaseControllerVM c)
			{
				if (c != null)
				{
					c.VibrateForce();
				}
			});
		}

		protected override void Vibrate()
		{
			base.BaseControllers.ForEach(delegate(BaseControllerVM c)
			{
				if (c != null)
				{
					c.VibrateCommand.Execute();
				}
			});
		}

		public override void UpdateControllerInfo()
		{
			BaseControllerVM baseControllerVM = base.FindController(this.ID);
			if (baseControllerVM == null)
			{
				return;
			}
			baseControllerVM.UpdateControllerInfo();
		}

		public override void UpdateControllerState(REWASD_GET_CONTROLLER_STATE_RESPONSE controllerState)
		{
		}

		public override int GetControllerFamilyCount(ControllerFamily controllerFamily)
		{
			return base.BaseControllers.Count((BaseControllerVM item) => item != null && item.ControllerFamily == controllerFamily);
		}

		public override ControllerTypeEnum? GetGamepadTypeByIndex(int index)
		{
			int num = 0;
			foreach (BaseControllerVM baseControllerVM in base.BaseControllers)
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

		public override Drawing MiniGamepadSVGIco
		{
			get
			{
				if (base.IsNintendoSwitchJoyConComposite)
				{
					return Application.Current.TryFindResource("MiniGamepadNJCon") as Drawing;
				}
				if (this.IsEngineMouseKeyboardComposite)
				{
					return Application.Current.TryFindResource("MiniEngineControllerMobileMouse+Keyboard") as Drawing;
				}
				return Application.Current.TryFindResource("MiniGamepadCompositeDevice") as Drawing;
			}
		}

		public override List<BaseControllerVM> GetLEDSupportedControllers()
		{
			List<BaseControllerVM> list = new List<BaseControllerVM>();
			foreach (BaseControllerVM baseControllerVM in base.BaseControllers)
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
			return await base.HttpClientService.Gamepad.GetControllerPressedButtons(this.ID);
		}

		private bool SetController(ref BaseControllerVM storage, BaseControllerVM value, int controllerIndex, [CallerMemberName] string propertyName = null)
		{
			if (this.SetProperty<BaseControllerVM>(ref storage, value, propertyName))
			{
				this.OnPropertyChanged("IsNintendoSwitchJoyConL");
				this.OnPropertyChanged("IsNintendoSwitchJoyConR");
				this.OnPropertyChanged("IsNintendoSwitchJoyConLExpected");
				this.OnPropertyChanged("IsNintendoSwitchJoyConRExpected");
				this.OnPropertyChanged("IsNintendoSwitchJoyConComposite");
				this.OnPropertyChanged("IsEngineMouseKeyboardComposite");
				this.OnPropertyChanged("MiniGamepadSVGIco");
				this.OnPropertyChanged("ControllerTypeFriendlyName");
				this.OnPropertyChanged("ControllerDisplayName");
				return true;
			}
			return false;
		}

		public void SetCurrentControllerAccordingControllerFamilyIndex(ControllerFamily controllerFamily, int index)
		{
			this._prevControllerFamily = controllerFamily;
			this._prevIndex = index;
			List<BaseControllerVM> list = base.BaseControllers.Where((BaseControllerVM c) => c != null && c.ControllerFamily == controllerFamily).ToList<BaseControllerVM>();
			if (list.Count > index)
			{
				this.CurrentController = list[index].CurrentController;
				return;
			}
			this.CurrentController = null;
		}

		private ControllerVM _currentControllerVM;

		private ControllerFamily _prevControllerFamily;

		private int _prevIndex = -1;
	}
}
