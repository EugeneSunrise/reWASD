using System;

namespace reWASDUI.Services.Interfaces
{
	public interface IGuiScaleService
	{
		double ScaleValue { get; set; }

		double SvgContainerWidth { get; set; }

		double SvgContainerHeight { get; set; }

		void CalculateScaleValue();
	}
}
