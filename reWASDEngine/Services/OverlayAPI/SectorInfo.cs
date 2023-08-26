using System;
using System.Windows;
using System.Windows.Media;
using DiscSoft.NET.Common.ViewModel.BindableBase;

namespace reWASDEngine.Services.OverlayAPI
{
	public class SectorInfo : ZBindableBase
	{
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
				this.OnPropertyChanged("PC");
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
				this.OnPropertyChanged("P1");
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
				this.OnPropertyChanged("P2");
			}
		}

		public float RadiusBig
		{
			get
			{
				return this._radiusBig;
			}
			set
			{
				this.SetProperty<float>(ref this._radiusBig, value, "RadiusBig");
				this.OnPropertyChanged("RadiusBig");
			}
		}

		public Drawing SectorIcon
		{
			get
			{
				return this._sectorIcon;
			}
			set
			{
				this.SetProperty<Drawing>(ref this._sectorIcon, value, "SectorIcon");
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
				if (this.IsActive)
				{
					return Application.Current.TryFindResource("CreamBrush") as SolidColorBrush;
				}
				return Application.Current.TryFindResource("CreamBrushHighlighted") as SolidColorBrush;
			}
		}

		private bool _isActive;

		private Point _pc;

		private Point _p1;

		private Point _p2;

		private float _radiusBig;

		private Drawing _sectorIcon;

		private Point _iconPos;
	}
}
