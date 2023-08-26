using System;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure.KeyBindingsModel.XBBindingDirectionalGroups;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups
{
	public class AdditionalStickDirectionalGroup : BaseStickDirectionalGroup
	{
		public AdditionalStickDirectionalGroup(BaseXBBindingCollection baseHostCollection)
			: base(baseHostCollection)
		{
		}

		public override string GroupLabel
		{
			get
			{
				return DTLocalization.GetString(12407);
			}
		}

		public override GamepadButton LeftDirection
		{
			get
			{
				return 218;
			}
		}

		public override GamepadButton UpDirection
		{
			get
			{
				return 216;
			}
		}

		public override GamepadButton RightDirection
		{
			get
			{
				return 219;
			}
		}

		public override GamepadButton DownDirection
		{
			get
			{
				return 217;
			}
		}

		public override GamepadButton UpLeft
		{
			get
			{
				return 220;
			}
		}

		public override GamepadButton UpRight
		{
			get
			{
				return 221;
			}
		}

		public override GamepadButton DownLeft
		{
			get
			{
				return 222;
			}
		}

		public override GamepadButton DownRight
		{
			get
			{
				return 223;
			}
		}

		public override GamepadButton LowZone
		{
			get
			{
				return 213;
			}
		}

		public override GamepadButton MedZone
		{
			get
			{
				return 214;
			}
		}

		public override GamepadButton HighZone
		{
			get
			{
				return 215;
			}
		}

		public override GamepadButton LowZoneUp
		{
			get
			{
				return 224;
			}
		}

		public override GamepadButton LowZoneDown
		{
			get
			{
				return 225;
			}
		}

		public override GamepadButton LowZoneLeft
		{
			get
			{
				return 226;
			}
		}

		public override GamepadButton LowZoneRight
		{
			get
			{
				return 227;
			}
		}

		public override GamepadButton MedZoneUp
		{
			get
			{
				return 228;
			}
		}

		public override GamepadButton MedZoneDown
		{
			get
			{
				return 229;
			}
		}

		public override GamepadButton MedZoneLeft
		{
			get
			{
				return 230;
			}
		}

		public override GamepadButton MedZoneRight
		{
			get
			{
				return 231;
			}
		}

		public override GamepadButton HighZoneUp
		{
			get
			{
				return 232;
			}
		}

		public override GamepadButton HighZoneDown
		{
			get
			{
				return 233;
			}
		}

		public override GamepadButton HighZoneLeft
		{
			get
			{
				return 234;
			}
		}

		public override GamepadButton HighZoneRight
		{
			get
			{
				return 235;
			}
		}

		public override GamepadButton Click
		{
			get
			{
				return 2000;
			}
		}

		public override ushort DefaultXLow
		{
			get
			{
				return 7849;
			}
		}

		public override ushort DefaultYLow
		{
			get
			{
				return 7849;
			}
		}

		public override ushort DefaultXMed
		{
			get
			{
				return 16155;
			}
		}

		public override ushort DefaultYMed
		{
			get
			{
				return 16155;
			}
		}

		public override ushort DefaultXHigh
		{
			get
			{
				return 24461;
			}
		}

		public override ushort DefaultYHigh
		{
			get
			{
				return 24461;
			}
		}

		public override bool IsButtonMappingVisible
		{
			get
			{
				return this.IsAdditionalMode;
			}
		}

		public override bool ResetToDefault(bool askConfirmation = true)
		{
			bool flag = base.ResetToDefault(askConfirmation);
			if (flag && base.HostCollection is MainXBBindingCollection)
			{
				this.IsAdditionalMode = true;
			}
			return flag;
		}

		public bool IsNativeModeVisible
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				return base.CurrentControllerType != null && ControllerTypeExtensions.IsFlydigiApex2(controllerTypeEnum.GetValueOrDefault());
			}
		}

		public bool IsNativeMode
		{
			get
			{
				ShiftXBBindingCollection shiftXBBindingCollection = base.HostCollection as ShiftXBBindingCollection;
				if (shiftXBBindingCollection != null)
				{
					return shiftXBBindingCollection.SubConfigData.MainXBBindingCollection.AdditionalStickDirectionalGroup.IsNativeMode;
				}
				return this._isNativeMode;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isNativeMode, value, "IsNativeMode"))
				{
					if (value)
					{
						MainXBBindingCollection mainXBBindingCollection = base.HostCollection as MainXBBindingCollection;
						if (mainXBBindingCollection != null)
						{
							mainXBBindingCollection.SetEnablePropertyChanged(false, false);
							base.ResetToDefault(false);
							mainXBBindingCollection.ShiftXBBindingCollections.ForEach(delegate(ShiftXBBindingCollection sc)
							{
								sc.AdditionalStickDirectionalGroup.ResetToDefault(false);
							});
							mainXBBindingCollection.SetEnablePropertyChanged(true, false);
						}
					}
					base.IsChanged = true;
					this.OnPropertyChanged("IsAdditionalMode");
					this.OnPropertyChanged("IsButtonMappingVisible");
					this.OnPropertyChanged("IsAdvancedSettingsVisible");
				}
			}
		}

		public bool IsAdditionalMode
		{
			get
			{
				return !this.IsNativeMode;
			}
			set
			{
				if (this.IsAdditionalMode == value)
				{
					return;
				}
				if (this.SetProperty<bool>(ref this._isNativeMode, !value, "IsAdditionalMode"))
				{
					base.IsChanged = true;
					this.OnPropertyChanged("IsNativeMode");
					this.OnPropertyChanged("IsButtonMappingVisible");
					this.OnPropertyChanged("IsAdvancedSettingsVisible");
					App.EventAggregator.GetEvent<RequestRefreshAnnotationVisibilityForGroup>().Publish(this);
				}
			}
		}

		public GamepadButton GetNativeDirection(GamepadButton directionButton)
		{
			switch (directionButton)
			{
			case 216:
				return 47;
			case 217:
				return 48;
			case 218:
				return 49;
			case 219:
				return 50;
			default:
				return 2000;
			}
		}

		public void CopyFromModel(AdditionalStickDirectionalGroup model)
		{
			this._isNativeMode = model.IsNativeMode;
			base.CopyFromModel(model);
		}

		public void CopyToModel(AdditionalStickDirectionalGroup model)
		{
			model.IsNativeMode = this.IsNativeMode;
			base.CopyToModel(model);
		}

		private bool _isNativeMode;
	}
}
