using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DiscSoft.NET.Common.ColorStuff;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using reWASDCommon.Infrastructure.Enums;
using reWASDEngine;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Infrastructure.KeyBindingsModel.OverlayMenu;

namespace DTOverlay
{
	public class OverlayMenuCircleE : ZBindableBase
	{
		public OverlayMenuCircleE(OverlayMenuCircle model, ushort iserviceProfileId, bool iIsSubmenu, bool itoggle, zColor sectorColor = null, int iownerSector = -1)
		{
			this.ownerSector = iownerSector;
			this.serviceProfileId = iserviceProfileId;
			this.toggle = itoggle;
			this.SubmenuStartAlpha = -1.5707963267948966;
			this.IsSubmenu = iIsSubmenu;
			this.MainOrSubmenu = this;
			this.IsLoaded = this.CreateFromModel(model, itoggle);
			this.BaseSectorColor = sectorColor;
		}

		public zColor BaseSectorColor
		{
			get
			{
				return this._baseSectorColor;
			}
			set
			{
				this.SetProperty<zColor>(ref this._baseSectorColor, value, "BaseSectorColor");
			}
		}

		private bool CreateFromModel(OverlayMenuCircle model, bool itoggle)
		{
			bool flag = false;
			if (model != null)
			{
				this.IsTintedBackground = model.IsTintedBackground;
				this.TintBackground = model.TintBackground;
				this.Scale = model.Scale;
				this.Monitor = model.Monitor;
				this.IsDelayBeforeOpening = model.IsDelayBeforeOpening;
				this.DelayBeforeOpening = model.DelayBeforeOpening;
				if (model.Sectors != null)
				{
					int num = 0;
					using (List<SectorItem>.Enumerator enumerator = model.Sectors.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SectorItem sectorItem = enumerator.Current;
							if (sectorItem == null)
							{
								return false;
							}
							SectorItemE sectorItemE = new SectorItemE(this);
							sectorItemE.NumberSector = num++;
							sectorItemE.CopyFromModel(sectorItem, itoggle);
							this.Sectors.Add(sectorItemE);
						}
						goto IL_E8;
					}
					IL_C1:
					SectorItemE sectorItemE2 = new SectorItemE(this);
					sectorItemE2.NumberSector = this.Sectors.Count;
					this.Sectors.Add(sectorItemE2);
					IL_E8:
					if (!(this.IsSubmenu ? (this.Sectors.Count < 2) : (this.Sectors.Count < 3)))
					{
						flag = true;
						goto IL_112;
					}
					goto IL_C1;
				}
				IL_112:
				this.RefreshSectorsInfo();
			}
			return flag;
		}

		public bool IsSubmenu
		{
			get
			{
				return this._IsSubmenu;
			}
			set
			{
				this.SetProperty<bool>(ref this._IsSubmenu, value, "IsSubmenu");
			}
		}

		public ObservableCollection<SectorItemE> Sectors
		{
			get
			{
				ObservableCollection<SectorItemE> observableCollection;
				if ((observableCollection = this._sectors) == null)
				{
					observableCollection = (this._sectors = new ObservableCollection<SectorItemE>());
				}
				return observableCollection;
			}
		}

		public double CurrAngleUI
		{
			get
			{
				return this._currAngleUI;
			}
			set
			{
				double num = Math.Round(value);
				if (num > this._currAngleUI + 1.0 || num < this._currAngleUI - 1.0)
				{
					this.SetProperty<double>(ref this._currAngleUI, num, "CurrAngleUI");
				}
			}
		}

		public bool NavigatorVisible
		{
			get
			{
				return this._NavigatorVisible;
			}
			set
			{
				this.SetProperty<bool>(ref this._NavigatorVisible, value, "NavigatorVisible");
			}
		}

		public double CurrAngle
		{
			get
			{
				return this._currAngle;
			}
			set
			{
				if (this.SetProperty<double>(ref this._currAngle, value, "CurrAngle"))
				{
					this.CurrAngleUI = value * 180.0 / 3.141592653589793;
				}
			}
		}

		public ControllerTypeEnum UIUseButtonControllerType
		{
			get
			{
				return this._uiUseButtonControllerType;
			}
			set
			{
				this.SetProperty<ControllerTypeEnum>(ref this._uiUseButtonControllerType, value, "UIUseButtonControllerType");
			}
		}

		public ControllerTypeEnum UICancelButtonControllerType
		{
			get
			{
				return this._uiCancelButtonControllerType;
			}
			set
			{
				this.SetProperty<ControllerTypeEnum>(ref this._uiCancelButtonControllerType, value, "UICancelButtonControllerType");
			}
		}

		public AssociatedControllerButton UIUseButton
		{
			get
			{
				return this._uiUseButton;
			}
			set
			{
				this.SetProperty<AssociatedControllerButton>(ref this._uiUseButton, value, "UIUseButton");
				this.OnPropertyChanged("IsUIUseButtonVisible");
			}
		}

		public AssociatedControllerButton UICancelButton
		{
			get
			{
				return this._uiCancelButton;
			}
			set
			{
				this.SetProperty<AssociatedControllerButton>(ref this._uiCancelButton, value, "UICancelButton");
				this.OnPropertyChanged("IsUICancelButtonVisible");
			}
		}

		public bool IsUIUseButtonVisible
		{
			get
			{
				if (this.CurrentSector != null)
				{
					AssociatedControllerButton uiuseButton = this.UIUseButton;
					return uiuseButton != null && uiuseButton.IsSet;
				}
				return false;
			}
		}

		public bool IsUICancelButtonVisible
		{
			get
			{
				if (this.IsSubmenu)
				{
					AssociatedControllerButton uicancelButton = this.UICancelButton;
					return uicancelButton != null && uicancelButton.IsSet;
				}
				return false;
			}
		}

		public int UISectorsCount
		{
			get
			{
				if (!this.IsSubmenu)
				{
					return this.SectorsCount;
				}
				return 12;
			}
		}

		public int SectorsCount
		{
			get
			{
				return this.Sectors.Count<SectorItemE>();
			}
		}

		public SectorItemE CurrentSector
		{
			get
			{
				return this._currentSector;
			}
			set
			{
				if (value != this._currentSector)
				{
					this._currentSector = value;
					this.OnPropertyChanged("CurrentSector");
				}
			}
		}

		public bool SetActiveSector(double a, bool isCurrAngleToCenter = false)
		{
			this.CurrAngle = a;
			bool flag = false;
			if (this.IsSubmenuVisible && this.CurrentSector != null && this.CurrentSector.Submenu != null)
			{
				flag = this.CurrentSector.Submenu.SetActiveSector(a, false);
			}
			else
			{
				a = a * 180.0 / 3.141592653589793;
				int num = -1;
				int num2 = 0;
				foreach (SectorItemE sectorItemE in this.Sectors)
				{
					double num3 = sectorItemE.AngleStart + sectorItemE.AngleDelta;
					if (a >= sectorItemE.AngleStart && a < num3)
					{
						num = num2;
						break;
					}
					if (num3 >= 360.0 && a + 360.0 >= sectorItemE.AngleStart && a + 360.0 < num3)
					{
						num = num2;
						break;
					}
					num2++;
				}
				this.SetActive(num, isCurrAngleToCenter);
				int currentSectorIndex = this.CurrentSectorIndex;
				if (this.prevActiveSector != currentSectorIndex)
				{
					this.prevActiveSector = currentSectorIndex;
					flag = true;
				}
			}
			return flag;
		}

		public void SetActive(int numberSector, bool isCurrAngleToCenter = false)
		{
			if (this.CurrentSector != null && this.CurrentSector.IsSubmenuVisible)
			{
				if (numberSector >= this.CurrentSector.Submenu.SectorsCount)
				{
					numberSector = -1;
				}
				this.CurrentSector.Submenu.SetActive(numberSector, false);
				if (numberSector != -1)
				{
					this.isWaitActive = true;
				}
				this.MainOrSubmenu = this.CurrentSector.Submenu;
				return;
			}
			if (numberSector == -1 && this.CurrentSector != null && this.CurrentSector.IsSubmenuVisible)
			{
				this.CurrentSector.Submenu.SetActive(numberSector, false);
				this.MainOrSubmenu = this.CurrentSector.Submenu;
				this.RefreshSectorsInfo();
				return;
			}
			this.MainOrSubmenu = this;
			for (int i = 0; i < this.Sectors.Count; i++)
			{
				bool flag = false;
				if (i == numberSector)
				{
					if (!this.Sectors[i].IsActive)
					{
						if (isCurrAngleToCenter)
						{
							double num = this.Sectors[i].AngleStart + this.Sectors[i].AngleDelta / 2.0;
							if (num >= 360.0)
							{
								num -= 360.0;
							}
							this.CurrAngle = num / 180.0 * 3.141592653589793;
						}
						this.Sectors[i].IsActive = true;
						flag = true;
					}
				}
				else
				{
					if (this.Sectors[i].IsActive)
					{
						flag = true;
					}
					this.Sectors[i].IsActive = false;
					this.Sectors[i].IsSubmenuVisible = false;
					this.Sectors[i].OpenParameter = 0f;
				}
				if (flag)
				{
					if (this.Sectors[i].IsActive)
					{
						this.CurrentSector = this.Sectors[i];
						if (this.ownerSector == -1)
						{
							Engine.XBServiceCommunicator.SetOverlayRadialMenuActiveSector(this.serviceProfileId, i, -1);
						}
						else
						{
							Engine.XBServiceCommunicator.SetOverlayRadialMenuActiveSector(this.serviceProfileId, this.ownerSector, i);
						}
					}
					this.OnPropertyChanged("CurrentSectorIndex");
				}
			}
			if (numberSector == -1 && this.CurrentSector != null)
			{
				if (this.ownerSector == -1)
				{
					Engine.XBServiceCommunicator.SetOverlayRadialMenuActiveSector(this.serviceProfileId, -1, -1);
				}
				else
				{
					Engine.XBServiceCommunicator.SetOverlayRadialMenuActiveSector(this.serviceProfileId, this.ownerSector, -1);
				}
				this.CurrentSector = null;
			}
			this.RefreshSectorsInfo();
		}

		public bool ShowSubmenu()
		{
			bool flag = false;
			if (this.CurrentSector != null && this.CurrentSector.IsSubmenuOn)
			{
				this.CurrentSector.IsSubmenuVisible = true;
				this.MainOrSubmenu = this.CurrentSector.Submenu;
				this.CurrentSector.Submenu.SetActive(0, false);
				this.isWaitActive = false;
				this.RefreshSectorsInfo();
				flag = true;
			}
			return flag;
		}

		public bool HideSubmenu()
		{
			bool flag = false;
			if (this.IsSubmenuVisible && this.CurrentSector != null)
			{
				this.CurrentSector.IsSubmenuVisible = false;
				this.MainOrSubmenu = this;
				this.isWaitActive = false;
				this.RefreshSectorsInfo();
				flag = true;
			}
			return flag;
		}

		private void RefreshSectorsInfo()
		{
			for (int i = 0; i < this.Sectors.Count; i++)
			{
				this.Sectors[i].NumberSector = (this.IsSubmenu ? (i + 100) : i);
			}
			this.OnPropertyChanged("IsUIUseButtonVisible");
			this.OnPropertyChanged("IsUICancelButtonVisible");
		}

		public int CurrentSectorIndex
		{
			get
			{
				int num = -1;
				if (!this.IsSubmenuVisible)
				{
					for (int i = 0; i < this.Sectors.Count; i++)
					{
						if (this.Sectors[i].IsActive)
						{
							num = this.Sectors[i].NumberSector;
							break;
						}
					}
				}
				else if (this.IsSubmenuVisible && this.CurrentSector != null)
				{
					num = this.CurrentSector.Submenu.CurrentSectorIndex;
				}
				this.OnPropertyChanged("CurrentMainSectorIndex");
				return num;
			}
		}

		public int CurrentMainSectorIndex
		{
			get
			{
				int num = -1;
				if (this.IsSubmenu)
				{
					num = this.ownerSector;
				}
				else
				{
					for (int i = 0; i < this.Sectors.Count; i++)
					{
						if (this.Sectors[i].IsActive)
						{
							num = this.Sectors[i].NumberSector;
							break;
						}
					}
				}
				return num;
			}
		}

		public bool IsSubmenuVisible
		{
			get
			{
				return this.CurrentSector != null && this.CurrentSector.IsSubmenuVisible;
			}
		}

		public bool IsShowSubmenuButton
		{
			get
			{
				return this.CurrentSector != null && this.CurrentSector.IsSubmenuOn;
			}
		}

		public OverlayMenuCircleE MainOrSubmenu
		{
			get
			{
				return this._mainOrSubmenu;
			}
			set
			{
				this.SetProperty<OverlayMenuCircleE>(ref this._mainOrSubmenu, value, "MainOrSubmenu");
			}
		}

		public void ApplyTimer()
		{
			if (!this.IsSubmenuVisible && this.CurrentSectorIndex != -1 && this.CurrentSectorIndex >= 0 && this.CurrentSectorIndex < this.Sectors.Count && this.Sectors[this.CurrentSectorIndex].Submenu != null)
			{
				this.ShowSubmenu();
			}
		}

		public void StickAtHome()
		{
			if (this.IsSubmenuVisible && !this.toggle)
			{
				this.HideSubmenu();
			}
			this.SetActive(-1, false);
		}

		public bool SetActiveCommand(RewasdOverlayMenuServiceCommand rewasdOverlayMenuServiceCommand)
		{
			this.NavigatorVisible = true;
			bool flag = false;
			if (!this.IsSubmenuVisible)
			{
				flag = true;
				switch (rewasdOverlayMenuServiceCommand)
				{
				case 6:
					this.SetActiveSector(0.0, true);
					break;
				case 7:
					this.SetActiveSector(3.141592653589793, true);
					break;
				case 8:
					this.SetActiveSector(1.5707963267948966, true);
					break;
				case 9:
					this.SetActiveSector(4.71238898038469, true);
					break;
				}
			}
			else if (this.IsSubmenuVisible && this.CurrentSector != null)
			{
				flag = this.CurrentSector.Submenu.SetActiveCommand(rewasdOverlayMenuServiceCommand);
			}
			return flag;
		}

		public void PrevSectorCommand()
		{
			this.NavigatorVisible = true;
			if (this.IsSubmenuVisible && this.CurrentSector != null)
			{
				this.CurrentSector.Submenu.PrevSectorCommand();
				return;
			}
			int num = this.CurrentSectorIndex;
			if (num >= 100)
			{
				num -= 100;
			}
			if (num == -1)
			{
				num = 0;
			}
			else
			{
				num--;
				if (num < 0)
				{
					if (this.IsSubmenu)
					{
						num = 0;
					}
					else
					{
						num = this.SectorsCount - 1;
					}
				}
			}
			this.SetActive(num, true);
		}

		public void NextSectorCommand()
		{
			if (this.IsSubmenuVisible && this.CurrentSector != null)
			{
				this.CurrentSector.Submenu.NextSectorCommand();
				return;
			}
			int num = this.CurrentSectorIndex;
			if (num >= 100)
			{
				num -= 100;
			}
			if (num == -1)
			{
				num = 0;
			}
			else
			{
				num++;
				if (num >= this.SectorsCount)
				{
					if (this.IsSubmenu)
					{
						num = this.SectorsCount - 1;
					}
					else
					{
						num = 0;
					}
				}
			}
			this.SetActive(num, true);
		}

		public double GetAlphaActiveSector()
		{
			double num = -1.0;
			if (this.IsSubmenuVisible && this.CurrentSector != null)
			{
				num = this.CurrentSector.Submenu.GetAlphaActiveSector();
			}
			else if (this.CurrentSector != null)
			{
				num = this.CurrentSector.AngleStart + this.CurrentSector.AngleDelta / 2.0;
				if (num >= 360.0)
				{
					num -= 360.0;
				}
			}
			return num;
		}

		public bool toggle;

		public ushort serviceProfileId;

		private int ownerSector;

		public bool IsTintedBackground;

		public int TintBackground;

		public int Scale;

		public string Monitor;

		public bool IsDelayBeforeOpening;

		public int DelayBeforeOpening;

		private zColor _baseSectorColor;

		public bool IsLoaded;

		private bool isWaitActive;

		public double SubmenuStartAlpha;

		private bool _IsSubmenu;

		private const double C_SUBMENU_ANGLE_SIZE = 4.08407029986392;

		private ObservableCollection<SectorItemE> _sectors;

		private double _currAngleUI;

		private double _currAngle;

		private bool _NavigatorVisible;

		private ControllerTypeEnum _uiUseButtonControllerType;

		private ControllerTypeEnum _uiCancelButtonControllerType;

		private AssociatedControllerButton _uiUseButton;

		private AssociatedControllerButton _uiCancelButton;

		private SectorItemE _currentSector;

		private int prevActiveSector = -1;

		private OverlayMenuCircleE _mainOrSubmenu;
	}
}
