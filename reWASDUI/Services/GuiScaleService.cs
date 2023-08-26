using System;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using reWASDUI.Services.Interfaces;

namespace reWASDUI.Services
{
	public class GuiScaleService : ZBindableBase, IGuiScaleService
	{
		public double ScaleValue
		{
			get
			{
				return this._scaleValue;
			}
			set
			{
				this.SetProperty<double>(ref this._scaleValue, value, "ScaleValue");
			}
		}

		public double SvgContainerWidth
		{
			get
			{
				return this._svgContainerWidth;
			}
			set
			{
				if (this.SetProperty<double>(ref this._svgContainerWidth, value, "SvgContainerWidth"))
				{
					this.CalculateScaleValue();
				}
			}
		}

		public double SvgContainerHeight
		{
			get
			{
				return this._svgContainerHeight;
			}
			set
			{
				if (this.SetProperty<double>(ref this._svgContainerHeight, value, "SvgContainerHeight"))
				{
					this.CalculateScaleValue();
				}
			}
		}

		public void CalculateScaleValue()
		{
			if (this._svgContainerHeight != 0.0 && this._svgContainerWidth != 0.0)
			{
				this.ScaleValue = Math.Max(0.7, Math.Min(this._svgContainerWidth / 626.0, this._svgContainerHeight / 433.0));
			}
		}

		private double _scaleValue = 1.0;

		private double _svgContainerWidth;

		private double _svgContainerHeight;
	}
}
