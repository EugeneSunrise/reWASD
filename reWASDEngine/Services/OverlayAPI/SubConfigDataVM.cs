using System;
using System.Collections.Generic;
using System.Linq;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure.KeyBindingsModel.XB;
using XBEliteWPF.Infrastructure.KeyBindingsModel.XBBindingDirectionalGroups;
using XBEliteWPF.Utils.Extensions;

namespace reWASDEngine.Services.OverlayAPI
{
	public class SubConfigDataVM : ZBindableBase
	{
		public List<RemapDataItem> Buttons
		{
			get
			{
				List<RemapDataItem> list;
				if ((list = this._buttons) == null)
				{
					list = (this._buttons = new List<RemapDataItem>());
				}
				return list;
			}
		}

		public int ControllerFamilyIndex { get; set; }

		public ControllerFamily ControllerFamily
		{
			get
			{
				return this._controllerFamily;
			}
			set
			{
				this.SetProperty<ControllerFamily>(ref this._controllerFamily, value, "ControllerFamily");
			}
		}

		public ControllerTypeEnum? ControllerType { get; set; }

		public bool Shift1ColumnVisible
		{
			get
			{
				return this._shift1ColumnVisible;
			}
			set
			{
				this.SetProperty<bool>(ref this._shift1ColumnVisible, value, "Shift1ColumnVisible");
			}
		}

		public bool Shift2ColumnVisible
		{
			get
			{
				return this._shift2ColumnVisible;
			}
			set
			{
				this.SetProperty<bool>(ref this._shift2ColumnVisible, value, "Shift2ColumnVisible");
			}
		}

		public bool Shift3ColumnVisible
		{
			get
			{
				return this._shift3ColumnVisible;
			}
			set
			{
				this.SetProperty<bool>(ref this._shift3ColumnVisible, value, "Shift3ColumnVisible");
			}
		}

		public bool Shift4ColumnVisible
		{
			get
			{
				return this._shift4ColumnVisible;
			}
			set
			{
				this.SetProperty<bool>(ref this._shift4ColumnVisible, value, "Shift4ColumnVisible");
			}
		}

		public bool Shift5ColumnVisible
		{
			get
			{
				return this._shift5ColumnVisible;
			}
			set
			{
				this.SetProperty<bool>(ref this._shift5ColumnVisible, value, "Shift5ColumnVisible");
			}
		}

		public bool Shift6ColumnVisible
		{
			get
			{
				return this._shift6ColumnVisible;
			}
			set
			{
				this.SetProperty<bool>(ref this._shift6ColumnVisible, value, "Shift6ColumnVisible");
			}
		}

		public bool Shift7ColumnVisible
		{
			get
			{
				return this._shift7ColumnVisible;
			}
			set
			{
				this.SetProperty<bool>(ref this._shift7ColumnVisible, value, "Shift7ColumnVisible");
			}
		}

		public bool Shift8ColumnVisible
		{
			get
			{
				return this._shift8ColumnVisible;
			}
			set
			{
				this.SetProperty<bool>(ref this._shift8ColumnVisible, value, "Shift8ColumnVisible");
			}
		}

		public bool Shift9ColumnVisible
		{
			get
			{
				return this._shift9ColumnVisible;
			}
			set
			{
				this.SetProperty<bool>(ref this._shift9ColumnVisible, value, "Shift9ColumnVisible");
			}
		}

		public bool Shift10ColumnVisible
		{
			get
			{
				return this._shift10ColumnVisible;
			}
			set
			{
				this.SetProperty<bool>(ref this._shift10ColumnVisible, value, "Shift10ColumnVisible");
			}
		}

		public bool Shift11ColumnVisible
		{
			get
			{
				return this._shift11ColumnVisible;
			}
			set
			{
				this.SetProperty<bool>(ref this._shift11ColumnVisible, value, "Shift11ColumnVisible");
			}
		}

		public bool Shift12ColumnVisible
		{
			get
			{
				return this._shift12ColumnVisible;
			}
			set
			{
				this.SetProperty<bool>(ref this._shift12ColumnVisible, value, "Shift12ColumnVisible");
			}
		}

		private bool ConainsAddaptiveTrigger(RemapDataItem item, int shift)
		{
			return shift <= 11 && !new AdaptiveTriggerPreset[]
			{
				default(AdaptiveTriggerPreset),
				2,
				1
			}.Contains(item.AdaptiveTrigger[shift]);
		}

		public void CalcColumnVisible()
		{
			this.Shift1ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift1 != null || this.ConainsAddaptiveTrigger(item, 1));
			this.Shift2ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift2 != null || this.ConainsAddaptiveTrigger(item, 2));
			this.Shift3ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift3 != null || this.ConainsAddaptiveTrigger(item, 3));
			this.Shift4ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift4 != null || this.ConainsAddaptiveTrigger(item, 4));
			this.Shift5ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift5 != null || this.ConainsAddaptiveTrigger(item, 5));
			this.Shift6ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift6 != null || this.ConainsAddaptiveTrigger(item, 6));
			this.Shift7ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift7 != null || this.ConainsAddaptiveTrigger(item, 7));
			this.Shift8ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift8 != null || this.ConainsAddaptiveTrigger(item, 8));
			this.Shift9ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift9 != null || this.ConainsAddaptiveTrigger(item, 9));
			this.Shift10ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift10 != null || this.ConainsAddaptiveTrigger(item, 10));
			this.Shift11ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift11 != null || this.ConainsAddaptiveTrigger(item, 11));
			this.Shift12ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift12 != null || this.ConainsAddaptiveTrigger(item, 12));
		}

		public void CalcMaskColumnVisible()
		{
			this.Shift1ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift1 != null);
			this.Shift2ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift2 != null);
			this.Shift3ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift3 != null);
			this.Shift4ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift4 != null);
			this.Shift5ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift5 != null);
			this.Shift6ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift6 != null);
			this.Shift7ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift7 != null);
			this.Shift8ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift8 != null);
			this.Shift9ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift9 != null);
			this.Shift10ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift10 != null);
			this.Shift11ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift11 != null);
			this.Shift12ColumnVisible = this.Buttons.Any((RemapDataItem item) => item.XbBindingShift12 != null);
		}

		public int VisibleColumnCount
		{
			get
			{
				return 2 + (this.Shift1ColumnVisible ? 1 : 0) + (this.Shift2ColumnVisible ? 1 : 0) + (this.Shift3ColumnVisible ? 1 : 0) + (this.Shift4ColumnVisible ? 1 : 0) + (this.Shift5ColumnVisible ? 1 : 0) + (this.Shift6ColumnVisible ? 1 : 0) + (this.Shift7ColumnVisible ? 1 : 0) + (this.Shift8ColumnVisible ? 1 : 0) + (this.Shift9ColumnVisible ? 1 : 0) + (this.Shift10ColumnVisible ? 1 : 0) + (this.Shift11ColumnVisible ? 1 : 0) + (this.Shift12ColumnVisible ? 1 : 0);
			}
		}

		public double Column0MaxWidth
		{
			get
			{
				return this._column0MaxWidth;
			}
			set
			{
				this.SetProperty<double>(ref this._column0MaxWidth, value, "Column0MaxWidth");
			}
		}

		public double Column1MaxWidth
		{
			get
			{
				return this._column1MaxWidth;
			}
			set
			{
				this.SetProperty<double>(ref this._column1MaxWidth, value, "Column1MaxWidth");
			}
		}

		public double Column2MaxWidth
		{
			get
			{
				return this._column2MaxWidth;
			}
			set
			{
				this.SetProperty<double>(ref this._column2MaxWidth, value, "Column2MaxWidth");
			}
		}

		public double Column3MaxWidth
		{
			get
			{
				return this._column3MaxWidth;
			}
			set
			{
				this.SetProperty<double>(ref this._column3MaxWidth, value, "Column3MaxWidth");
			}
		}

		public double Column4MaxWidth
		{
			get
			{
				return this._column4MaxWidth;
			}
			set
			{
				this.SetProperty<double>(ref this._column4MaxWidth, value, "Column4MaxWidth");
			}
		}

		public double Column5MaxWidth
		{
			get
			{
				return this._column5MaxWidth;
			}
			set
			{
				this.SetProperty<double>(ref this._column5MaxWidth, value, "Column5MaxWidth");
			}
		}

		public double Column6MaxWidth
		{
			get
			{
				return this._column6MaxWidth;
			}
			set
			{
				this.SetProperty<double>(ref this._column6MaxWidth, value, "Column6MaxWidth");
			}
		}

		public double Column7MaxWidth
		{
			get
			{
				return this._column7MaxWidth;
			}
			set
			{
				this.SetProperty<double>(ref this._column7MaxWidth, value, "Column7MaxWidth");
			}
		}

		public double Column8MaxWidth
		{
			get
			{
				return this._column8MaxWidth;
			}
			set
			{
				this.SetProperty<double>(ref this._column8MaxWidth, value, "Column8MaxWidth");
			}
		}

		public double Column9MaxWidth
		{
			get
			{
				return this._column9MaxWidth;
			}
			set
			{
				this.SetProperty<double>(ref this._column9MaxWidth, value, "Column9MaxWidth");
			}
		}

		public double Column10MaxWidth
		{
			get
			{
				return this._column10MaxWidth;
			}
			set
			{
				this.SetProperty<double>(ref this._column10MaxWidth, value, "Column10MaxWidth");
			}
		}

		public double Column11MaxWidth
		{
			get
			{
				return this._column11MaxWidth;
			}
			set
			{
				this.SetProperty<double>(ref this._column11MaxWidth, value, "Column11MaxWidth");
			}
		}

		public double Column12MaxWidth
		{
			get
			{
				return this._column12MaxWidth;
			}
			set
			{
				this.SetProperty<double>(ref this._column12MaxWidth, value, "Column12MaxWidth");
			}
		}

		public double Column13MaxWidth
		{
			get
			{
				return this._column13MaxWidth;
			}
			set
			{
				this.SetProperty<double>(ref this._column13MaxWidth, value, "Column13MaxWidth");
			}
		}

		private bool CanBeInherited(RemapDataItem item, int shift)
		{
			if (item.XbBindingMain == null)
			{
				return false;
			}
			if (item.XbBindingMain.ControllerButton.IsKeyScanCode)
			{
				return true;
			}
			if (!item.XbBindingMain.IsAnyActivatorVirtualMappingPresent && item.XbBindingMain.HostCollection.SubConfigData.ConfigData.IsVirtualGamepadMappingPresent() && ControllerTypeExtensions.IsGamepadWithSonyTouchpad(VirtualControllerTypeExtensions.GetControllerType(item.XbBindingMain.HostCollection.SubConfigData.ConfigData.VirtualGamepadType)) && GamepadButtonExtensions.IsPhysicalTrackPad1Direction(item.XbBindingMain.ControllerButton.GamepadButton))
			{
				Touchpad1DirectionalGroup touchpad1DirectionalGroup = item.XbBindingMain.HostCollection.SubConfigData.MainXBBindingCollection.ShiftXBBindingCollections[shift - 1].Touchpad1DirectionalGroup;
				bool flag;
				if (touchpad1DirectionalGroup == null)
				{
					flag = false;
				}
				else
				{
					XBBinding tapValue = touchpad1DirectionalGroup.TapValue;
					bool? flag2 = ((tapValue != null) ? new bool?(tapValue.IsUnmapped) : null);
					bool flag3 = true;
					flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				if (flag)
				{
					return false;
				}
			}
			return true;
		}

		public void FillInheritedItems()
		{
			if (this.Shift1ColumnVisible)
			{
				this.Buttons.ForEach(delegate(RemapDataItem button)
				{
					if (this.CanBeInherited(button, 1))
					{
						button.FillInherited(1);
					}
				});
			}
			if (this.Shift2ColumnVisible)
			{
				this.Buttons.ForEach(delegate(RemapDataItem button)
				{
					if (this.CanBeInherited(button, 2))
					{
						button.FillInherited(2);
					}
				});
			}
			if (this.Shift3ColumnVisible)
			{
				this.Buttons.ForEach(delegate(RemapDataItem button)
				{
					if (this.CanBeInherited(button, 3))
					{
						button.FillInherited(3);
					}
				});
			}
			if (this.Shift4ColumnVisible)
			{
				this.Buttons.ForEach(delegate(RemapDataItem button)
				{
					if (this.CanBeInherited(button, 4))
					{
						button.FillInherited(4);
					}
				});
			}
			if (this.Shift5ColumnVisible)
			{
				this.Buttons.ForEach(delegate(RemapDataItem button)
				{
					if (this.CanBeInherited(button, 5))
					{
						button.FillInherited(5);
					}
				});
			}
			if (this.Shift6ColumnVisible)
			{
				this.Buttons.ForEach(delegate(RemapDataItem button)
				{
					if (this.CanBeInherited(button, 6))
					{
						button.FillInherited(6);
					}
				});
			}
			if (this.Shift7ColumnVisible)
			{
				this.Buttons.ForEach(delegate(RemapDataItem button)
				{
					if (this.CanBeInherited(button, 7))
					{
						button.FillInherited(7);
					}
				});
			}
			if (this.Shift8ColumnVisible)
			{
				this.Buttons.ForEach(delegate(RemapDataItem button)
				{
					if (this.CanBeInherited(button, 8))
					{
						button.FillInherited(8);
					}
				});
			}
			if (this.Shift9ColumnVisible)
			{
				this.Buttons.ForEach(delegate(RemapDataItem button)
				{
					if (this.CanBeInherited(button, 9))
					{
						button.FillInherited(9);
					}
				});
			}
			if (this.Shift10ColumnVisible)
			{
				this.Buttons.ForEach(delegate(RemapDataItem button)
				{
					if (this.CanBeInherited(button, 10))
					{
						button.FillInherited(10);
					}
				});
			}
			if (this.Shift11ColumnVisible)
			{
				this.Buttons.ForEach(delegate(RemapDataItem button)
				{
					if (this.CanBeInherited(button, 11))
					{
						button.FillInherited(11);
					}
				});
			}
			if (this.Shift12ColumnVisible)
			{
				this.Buttons.ForEach(delegate(RemapDataItem button)
				{
					if (this.CanBeInherited(button, 12))
					{
						button.FillInherited(12);
					}
				});
			}
			this.Buttons.ForEach(delegate(RemapDataItem button)
			{
				if ((button.Btn.GamepadButton == 51 || button.Btn.GamepadButton == 55) && button.AdaptiveTrigger[0] != null)
				{
					for (int i = 1; i <= 11; i++)
					{
						if (button.AdaptiveTrigger[i] == null)
						{
							button.AdaptiveTrigger[i] = 2;
						}
					}
				}
			});
		}

		public int MaxVisibleButtons { get; set; }

		public List<RemapDataItem> HiddenButtons
		{
			get
			{
				List<RemapDataItem> list;
				if ((list = this._hiddenButtons) == null)
				{
					list = (this._hiddenButtons = new List<RemapDataItem>());
				}
				return list;
			}
		}

		public bool IsMaskItems
		{
			get
			{
				return this._isMaskItems;
			}
			set
			{
				this.SetProperty<bool>(ref this._isMaskItems, value, "IsMaskItems");
			}
		}

		public void RestrictVisibleButtons()
		{
			if (this.Buttons.Count > this.MaxVisibleButtons)
			{
				int num = this.Buttons.Count - this.MaxVisibleButtons;
				this._hiddenButtons = this.Buttons.GetRange(this.MaxVisibleButtons, num);
				this.Buttons.RemoveRange(this.MaxVisibleButtons, num);
			}
		}

		public bool NextVisibleButtons()
		{
			if (this.HiddenButtons.Count == 0)
			{
				return false;
			}
			int num = this.HiddenButtons.Count;
			if (num > this.MaxVisibleButtons)
			{
				num = this.MaxVisibleButtons;
			}
			this._buttons = this.HiddenButtons.GetRange(0, num);
			this.HiddenButtons.RemoveRange(0, num);
			this.OnPropertyChanged("Buttons");
			return true;
		}

		private List<RemapDataItem> _buttons;

		private ControllerFamily _controllerFamily;

		private bool _shift1ColumnVisible;

		private bool _shift2ColumnVisible;

		private bool _shift3ColumnVisible;

		private bool _shift4ColumnVisible;

		private bool _shift5ColumnVisible;

		private bool _shift6ColumnVisible;

		private bool _shift7ColumnVisible;

		private bool _shift8ColumnVisible;

		private bool _shift9ColumnVisible;

		private bool _shift10ColumnVisible;

		private bool _shift11ColumnVisible;

		private bool _shift12ColumnVisible;

		private double _column0MaxWidth = double.NaN;

		private double _column1MaxWidth = double.NaN;

		private double _column2MaxWidth = double.NaN;

		private double _column3MaxWidth = double.NaN;

		private double _column4MaxWidth = double.NaN;

		private double _column5MaxWidth = double.NaN;

		private double _column6MaxWidth = double.NaN;

		private double _column7MaxWidth = double.NaN;

		private double _column8MaxWidth = double.NaN;

		private double _column9MaxWidth = double.NaN;

		private double _column10MaxWidth = double.NaN;

		private double _column11MaxWidth = double.NaN;

		private double _column12MaxWidth = double.NaN;

		private double _column13MaxWidth = double.NaN;

		private List<RemapDataItem> _hiddenButtons;

		private bool _isMaskItems;
	}
}
