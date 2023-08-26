using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using DiscSoft.NET.Common.ColorStuff;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using reWASDCommon.Infrastructure;
using XBEliteWPF.Infrastructure.KeyBindingsModel.OverlayMenu;
using XBEliteWPF.Infrastructure.KeyBindingsModel.XB;

namespace DTOverlay
{
	public class SectorItemE : ZBindableBase
	{
		public SectorItemE(OverlayMenuCircleE iowner)
		{
			this.owner = iowner;
			this.XBBinding = null;
			this.SectorColor = iowner.BaseSectorColor ?? new zColor(Constants.OverlayMenuSectorItemSectorColor);
			this.SelectedIcon = RadialMenuIcons.StandardIcons[0];
		}

		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				this.SetProperty<bool>(ref this._isActive, value, "IsActive");
			}
		}

		public RadialMenuIcon SelectedIcon
		{
			get
			{
				return this._selectedIcon;
			}
			set
			{
				if (this.SetProperty<RadialMenuIcon>(ref this._selectedIcon, value, "SelectedIcon"))
				{
					this.OnPropertyChanged("SelectedIconDrawing");
				}
			}
		}

		public Drawing SelectedIconDrawing
		{
			get
			{
				Application application = Application.Current;
				RadialMenuIcon selectedIcon = this.SelectedIcon;
				return application.TryFindResource(((selectedIcon != null) ? selectedIcon.Resource : null) ?? "") as Drawing;
			}
		}

		public zColor SectorColor
		{
			get
			{
				if (this._sectorColor == null)
				{
					this._sectorColor = new zColor(Constants.OverlayMenuSectorItemSectorColor);
				}
				return this._sectorColor;
			}
			set
			{
				this.SetProperty<zColor>(ref this._sectorColor, value, "SectorColor");
			}
		}

		public double AngleStart
		{
			get
			{
				return this._angleStart;
			}
			set
			{
				this.SetProperty<double>(ref this._angleStart, value, "AngleStart");
			}
		}

		public float OpenParameter
		{
			get
			{
				return this._openParameter;
			}
			set
			{
				this.SetProperty<float>(ref this._openParameter, value, "OpenParameter");
			}
		}

		public int NumberSector
		{
			get
			{
				return this._NumberSector;
			}
			set
			{
				this.SetProperty<int>(ref this._NumberSector, value, "NumberSector");
			}
		}

		public OverlayMenuCircleE owner { get; set; }

		public XBBinding XBBinding { get; set; }

		public void CopyFromModel(SectorItem model, bool itoggle)
		{
			this.SectorColor = model.SectorColor;
			this.SelectedIcon = model.SelectedIcon;
			this.IsSubmenuOn = false;
			if (model.XBBinding != null)
			{
				this.XBBinding = model.XBBinding.Clone();
			}
			OverlayMenuCircle submenu = model.Submenu;
			bool flag;
			if (submenu == null)
			{
				flag = false;
			}
			else
			{
				List<SectorItem> sectors = submenu.Sectors;
				int? num = ((sectors != null) ? new int?(sectors.Count) : null);
				int num2 = 0;
				flag = (num.GetValueOrDefault() > num2) & (num != null);
			}
			if (flag && this.Submenu == null)
			{
				this.Submenu = new OverlayMenuCircleE(model.Submenu, this.owner.serviceProfileId, true, itoggle, this.SectorColor, this.NumberSector);
				if (this.Submenu.IsLoaded)
				{
					this.IsSubmenuOn = true;
				}
			}
		}

		public bool IsSubmenuVisible
		{
			get
			{
				return this._isSubmenuVisible && this._isSubmenuOn;
			}
			set
			{
				this.SetProperty<bool>(ref this._isSubmenuVisible, value, "IsSubmenuVisible");
			}
		}

		public bool IsSubmenuOn
		{
			get
			{
				return this._isSubmenuOn;
			}
			set
			{
				this.SetProperty<bool>(ref this._isSubmenuOn, value, "IsSubmenuOn");
			}
		}

		public OverlayMenuCircleE Submenu
		{
			get
			{
				return this._Submenu;
			}
			set
			{
				this.SetProperty<OverlayMenuCircleE>(ref this._Submenu, value, "Submenu");
			}
		}

		public double AngleDelta
		{
			get
			{
				return this._AngleDelta;
			}
			set
			{
				this.SetProperty<double>(ref this._AngleDelta, value, "AngleDelta");
			}
		}

		private bool _isActive;

		private RadialMenuIcon _selectedIcon;

		private zColor _sectorColor;

		private double _angleStart;

		private float _openParameter;

		private int _NumberSector = -1;

		private bool _isSubmenuVisible;

		private bool _isSubmenuOn;

		private OverlayMenuCircleE _Submenu;

		private double _AngleDelta;
	}
}
