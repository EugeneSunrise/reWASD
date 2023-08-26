using System;
using System.Windows;
using System.Windows.Media;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using Prism.Commands;

namespace DTOverlay
{
	public class SectorInfoUIE : ZBindableBase
	{
		public OverlayMenuCircleE owner { get; set; }

		public SectorInfoUIE(OverlayMenuCircleE iowner)
		{
			this.owner = iowner;
			this.IsActive = false;
			this.IsUnderMouse = false;
		}

		public bool IsEmpty
		{
			get
			{
				return this._isEmpty;
			}
			set
			{
				this.SetProperty<bool>(ref this._isEmpty, value, "IsEmpty");
			}
		}

		public bool IsShowBindingWindow
		{
			get
			{
				return this._IsShowBindingWindow;
			}
			set
			{
				this.SetProperty<bool>(ref this._IsShowBindingWindow, value, "IsShowBindingWindow");
			}
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
				this.OnPropertyChanged("FillColor");
				this.OnPropertyChanged("ButtonVisible");
				this.OnPropertyChanged("RadiusBig");
			}
		}

		public bool IsUnderMouse
		{
			get
			{
				return this._isUnderMouse;
			}
			set
			{
				this.SetProperty<bool>(ref this._isUnderMouse, value, "IsUnderMouse");
				this.OnPropertyChanged("FillColor");
				this.OnPropertyChanged("ButtonVisible");
			}
		}

		public Point PC
		{
			get
			{
				return this._pc;
			}
			set
			{
				this.SetProperty<Point>(ref this._pc, value, "PC");
			}
		}

		public Point P1
		{
			get
			{
				return this._p1;
			}
			set
			{
				this.SetProperty<Point>(ref this._p1, value, "P1");
			}
		}

		public Point P2
		{
			get
			{
				return this._p2;
			}
			set
			{
				this.SetProperty<Point>(ref this._p2, value, "P2");
			}
		}

		public Point ShowSubmenuPos
		{
			get
			{
				return this._ShowSubmenuPos;
			}
			set
			{
				this.SetProperty<Point>(ref this._ShowSubmenuPos, value, "ShowSubmenuPos");
			}
		}

		public Point ButtonMinusPos
		{
			get
			{
				return this._buttonMinusPos;
			}
			set
			{
				this.SetProperty<Point>(ref this._buttonMinusPos, value, "ButtonMinusPos");
			}
		}

		public Point ButtonPlusPosSecond
		{
			get
			{
				return this._buttonPlusPosSecond;
			}
			set
			{
				this.SetProperty<Point>(ref this._buttonPlusPosSecond, value, "ButtonPlusPosSecond");
			}
		}

		public Point ButtonPlusSector
		{
			get
			{
				return this._buttonPlusSector;
			}
			set
			{
				this.SetProperty<Point>(ref this._buttonPlusSector, value, "ButtonPlusSector");
			}
		}

		public bool ButtonVisible
		{
			get
			{
				return this.IsActive;
			}
		}

		public Point ButtonPlusPos
		{
			get
			{
				return this._buttonPlusPos;
			}
			set
			{
				this.SetProperty<Point>(ref this._buttonPlusPos, value, "ButtonPlusPos");
			}
		}

		public double RadiusBig
		{
			get
			{
				return this._RadiusBig;
			}
			set
			{
				this.SetProperty<double>(ref this._RadiusBig, value, "RadiusBig");
			}
		}

		public double SmallRadius
		{
			get
			{
				return this._SmallRadius;
			}
			set
			{
				this.SetProperty<double>(ref this._SmallRadius, value, "SmallRadius");
			}
		}

		public Point IconPos
		{
			get
			{
				return this._iconPos;
			}
			set
			{
				this.SetProperty<Point>(ref this._iconPos, value, "IconPos");
				this.OnPropertyChanged("IconPos");
			}
		}

		public Brush FillColor
		{
			get
			{
				Brush brush;
				if (this.IsEmpty)
				{
					brush = new SolidColorBrush(Colors.Black);
					brush.Opacity = 160.0;
				}
				else if (this.IsActive && !this.IsUnderMouse)
				{
					brush = Application.Current.TryFindResource("OverlayItemSelectedForeground") as SolidColorBrush;
				}
				else if (this.IsActive && this.IsUnderMouse)
				{
					brush = Application.Current.TryFindResource("OverlayItemSelectedHoveredForeground") as SolidColorBrush;
				}
				else if (!this.IsActive && this.IsUnderMouse)
				{
					brush = Application.Current.TryFindResource("OverlayItemHoverForeground") as SolidColorBrush;
				}
				else
				{
					brush = Application.Current.TryFindResource("OverlayItemForeground") as SolidColorBrush;
				}
				if (brush.IsFrozen)
				{
					brush = brush.Clone();
				}
				return brush;
			}
			set
			{
				this.FillColor = value;
			}
		}

		public DelegateCommand OnMouseEnterCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._onMouseEnterCommand) == null)
				{
					delegateCommand = (this._onMouseEnterCommand = new DelegateCommand(new Action(this.OnMouseEnter)));
				}
				return delegateCommand;
			}
		}

		private void OnMouseEnter()
		{
			this.IsUnderMouse = true;
		}

		public DelegateCommand OnMouseLeaveCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._onMouseLeaveCommand) == null)
				{
					delegateCommand = (this._onMouseLeaveCommand = new DelegateCommand(new Action(this.OnMouseLeave)));
				}
				return delegateCommand;
			}
		}

		private void OnMouseLeave()
		{
			this.IsUnderMouse = false;
		}

		public double CenterPosX
		{
			get
			{
				return this._CenterPosX;
			}
			set
			{
				this.SetProperty<double>(ref this._CenterPosX, value, "CenterPosX");
			}
		}

		public double CenterPosY
		{
			get
			{
				return this._CenterPosY;
			}
			set
			{
				this.SetProperty<double>(ref this._CenterPosY, value, "CenterPosY");
			}
		}

		private bool _isEmpty;

		private bool _IsShowBindingWindow;

		private bool _isActive;

		private bool _isUnderMouse;

		private Point _pc;

		private Point _p1;

		private Point _p2;

		private Point _ShowSubmenuPos;

		private Point _buttonMinusPos;

		private Point _buttonPlusPosSecond;

		private Point _buttonPlusSector;

		private Point _buttonPlusPos;

		private double _RadiusBig;

		private double _SmallRadius;

		private Point _iconPos;

		private DelegateCommand _onMouseEnterCommand;

		private DelegateCommand _onMouseLeaveCommand;

		private double _CenterPosX;

		private double _CenterPosY;
	}
}
