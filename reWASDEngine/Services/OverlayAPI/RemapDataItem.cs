using System;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Infrastructure.KeyBindingsModel.Mask;
using XBEliteWPF.Infrastructure.KeyBindingsModel.XB;

namespace reWASDEngine.Services.OverlayAPI
{
	public class RemapDataItem : ZBindableBase
	{
		public object HostObject { get; set; }

		public AssociatedControllerButton Btn
		{
			get
			{
				return this._bnt;
			}
			set
			{
				this.SetProperty<AssociatedControllerButton>(ref this._bnt, value, "Btn");
			}
		}

		public AdaptiveTriggerPreset[] AdaptiveTrigger
		{
			get
			{
				return this._adaptiveTrigger;
			}
			set
			{
				this.SetProperty<AdaptiveTriggerPreset[]>(ref this._adaptiveTrigger, value, "AdaptiveTrigger");
			}
		}

		public MaskItem Msk
		{
			get
			{
				return this._msk;
			}
			set
			{
				this.SetProperty<MaskItem>(ref this._msk, value, "Msk");
			}
		}

		public bool IsLabelMode
		{
			get
			{
				return this._isLabelMode;
			}
			set
			{
				this.SetProperty<bool>(ref this._isLabelMode, value, "IsLabelMode");
			}
		}

		public XBBinding XbBindingMain
		{
			get
			{
				return this._xbBindingMain;
			}
			set
			{
				this.SetProperty<XBBinding>(ref this._xbBindingMain, value, "XbBindingMain");
			}
		}

		public XBBinding XbBindingShift1
		{
			get
			{
				return this._xbBindingShift1;
			}
			set
			{
				this.SetProperty<XBBinding>(ref this._xbBindingShift1, value, "XbBindingShift1");
			}
		}

		public XBBinding XbBindingShift2
		{
			get
			{
				return this._xbBindingShift2;
			}
			set
			{
				this.SetProperty<XBBinding>(ref this._xbBindingShift2, value, "XbBindingShift2");
			}
		}

		public XBBinding XbBindingShift3
		{
			get
			{
				return this._xbBindingShift3;
			}
			set
			{
				this.SetProperty<XBBinding>(ref this._xbBindingShift3, value, "XbBindingShift3");
			}
		}

		public XBBinding XbBindingShift4
		{
			get
			{
				return this._xbBindingShift4;
			}
			set
			{
				this.SetProperty<XBBinding>(ref this._xbBindingShift4, value, "XbBindingShift4");
			}
		}

		public XBBinding XbBindingShift5
		{
			get
			{
				return this._xbBindingShift5;
			}
			set
			{
				this.SetProperty<XBBinding>(ref this._xbBindingShift5, value, "XbBindingShift5");
			}
		}

		public XBBinding XbBindingShift6
		{
			get
			{
				return this._xbBindingShift6;
			}
			set
			{
				this.SetProperty<XBBinding>(ref this._xbBindingShift6, value, "XbBindingShift6");
			}
		}

		public XBBinding XbBindingShift7
		{
			get
			{
				return this._xbBindingShift7;
			}
			set
			{
				this.SetProperty<XBBinding>(ref this._xbBindingShift7, value, "XbBindingShift7");
			}
		}

		public XBBinding XbBindingShift8
		{
			get
			{
				return this._xbBindingShift8;
			}
			set
			{
				this.SetProperty<XBBinding>(ref this._xbBindingShift8, value, "XbBindingShift8");
			}
		}

		public XBBinding XbBindingShift9
		{
			get
			{
				return this._xbBindingShift9;
			}
			set
			{
				this.SetProperty<XBBinding>(ref this._xbBindingShift9, value, "XbBindingShift9");
			}
		}

		public XBBinding XbBindingShift10
		{
			get
			{
				return this._xbBindingShift10;
			}
			set
			{
				this.SetProperty<XBBinding>(ref this._xbBindingShift10, value, "XbBindingShift10");
			}
		}

		public XBBinding XbBindingShift11
		{
			get
			{
				return this._xbBindingShift11;
			}
			set
			{
				this.SetProperty<XBBinding>(ref this._xbBindingShift11, value, "XbBindingShift11");
			}
		}

		public XBBinding XbBindingShift12
		{
			get
			{
				return this._xbBindingShift12;
			}
			set
			{
				this.SetProperty<XBBinding>(ref this._xbBindingShift12, value, "XbBindingShift12");
			}
		}

		public bool IsBindingExist(int shift)
		{
			return (shift == 0 && this.XbBindingMain != null) || (shift == 1 && this.XbBindingShift1 != null) || (shift == 2 && this.XbBindingShift2 != null) || (shift == 3 && this.XbBindingShift3 != null) || (shift == 4 && this.XbBindingShift4 != null) || (shift == 5 && this.XbBindingShift5 != null) || (shift == 6 && this.XbBindingShift6 != null) || (shift == 7 && this.XbBindingShift7 != null) || (shift == 8 && this.XbBindingShift8 != null) || (shift == 9 && this.XbBindingShift9 != null) || (shift == 10 && this.XbBindingShift10 != null) || (shift == 11 && this.XbBindingShift11 != null) || (shift == 12 && this.XbBindingShift12 != null);
		}

		public void FillInherited(int shift)
		{
			if (this.XbBindingMain == null)
			{
				return;
			}
			ShiftXBBindingCollection shiftXBBindingCollection = this.XbBindingMain.HostCollection.SubConfigData.MainXBBindingCollection.ShiftXBBindingCollections[shift - 1];
			if (shift == 1 && this.XbBindingShift1 == null)
			{
				this.XbBindingShift1 = this.XbBindingMain.Clone();
				this.XbBindingShift1.HostCollection = shiftXBBindingCollection;
				this.XbBindingShift1.IsInheritedBinding = true;
			}
			if (shift == 2 && this.XbBindingShift2 == null)
			{
				this.XbBindingShift2 = this.XbBindingMain.Clone();
				this.XbBindingShift2.HostCollection = shiftXBBindingCollection;
				this.XbBindingShift2.IsInheritedBinding = true;
			}
			if (shift == 3 && this.XbBindingShift3 == null)
			{
				this.XbBindingShift3 = this.XbBindingMain.Clone();
				this.XbBindingShift3.HostCollection = shiftXBBindingCollection;
				this.XbBindingShift3.IsInheritedBinding = true;
			}
			if (shift == 4 && this.XbBindingShift4 == null)
			{
				this.XbBindingShift4 = this.XbBindingMain.Clone();
				this.XbBindingShift4.HostCollection = shiftXBBindingCollection;
				this.XbBindingShift4.IsInheritedBinding = true;
			}
			if (shift == 5 && this.XbBindingShift5 == null)
			{
				this.XbBindingShift5 = this.XbBindingMain.Clone();
				this.XbBindingShift5.HostCollection = shiftXBBindingCollection;
				this.XbBindingShift5.IsInheritedBinding = true;
			}
			if (shift == 6 && this.XbBindingShift6 == null)
			{
				this.XbBindingShift6 = this.XbBindingMain.Clone();
				this.XbBindingShift6.HostCollection = shiftXBBindingCollection;
				this.XbBindingShift6.IsInheritedBinding = true;
			}
			if (shift == 7 && this.XbBindingShift7 == null)
			{
				this.XbBindingShift7 = this.XbBindingMain.Clone();
				this.XbBindingShift7.HostCollection = shiftXBBindingCollection;
				this.XbBindingShift7.IsInheritedBinding = true;
			}
			if (shift == 8 && this.XbBindingShift8 == null)
			{
				this.XbBindingShift8 = this.XbBindingMain.Clone();
				this.XbBindingShift8.HostCollection = shiftXBBindingCollection;
				this.XbBindingShift8.IsInheritedBinding = true;
			}
			if (shift == 9 && this.XbBindingShift9 == null)
			{
				this.XbBindingShift9 = this.XbBindingMain.Clone();
				this.XbBindingShift9.HostCollection = shiftXBBindingCollection;
				this.XbBindingShift9.IsInheritedBinding = true;
			}
			if (shift == 10 && this.XbBindingShift10 == null)
			{
				this.XbBindingShift10 = this.XbBindingMain.Clone();
				this.XbBindingShift10.HostCollection = shiftXBBindingCollection;
				this.XbBindingShift10.IsInheritedBinding = true;
			}
			if (shift == 11 && this.XbBindingShift11 == null)
			{
				this.XbBindingShift11 = this.XbBindingMain.Clone();
				this.XbBindingShift11.HostCollection = shiftXBBindingCollection;
				this.XbBindingShift11.IsInheritedBinding = true;
			}
			if (shift == 12 && this.XbBindingShift12 == null)
			{
				this.XbBindingShift12 = this.XbBindingMain.Clone();
				this.XbBindingShift12.HostCollection = shiftXBBindingCollection;
				this.XbBindingShift12.IsInheritedBinding = true;
			}
		}

		private AssociatedControllerButton _bnt;

		private MaskItem _msk;

		private AdaptiveTriggerPreset[] _adaptiveTrigger = new AdaptiveTriggerPreset[13];

		private bool _isLabelMode;

		private XBBinding _xbBindingMain;

		private XBBinding _xbBindingShift1;

		private XBBinding _xbBindingShift2;

		private XBBinding _xbBindingShift3;

		private XBBinding _xbBindingShift4;

		private XBBinding _xbBindingShift5;

		private XBBinding _xbBindingShift6;

		private XBBinding _xbBindingShift7;

		private XBBinding _xbBindingShift8;

		private XBBinding _xbBindingShift9;

		private XBBinding _xbBindingShift10;

		private XBBinding _xbBindingShift11;

		private XBBinding _xbBindingShift12;
	}
}
