using System;
using XBEliteWPF.ViewModels.Base;

namespace reWASDEngine.OverlayAPI
{
	public class OverlayMenuBlackViewEVM : ZBindable
	{
		public double TintBackground
		{
			get
			{
				return this._tintBackground;
			}
			set
			{
				this.SetProperty<double>(ref this._tintBackground, value, "TintBackground");
			}
		}

		private double _tintBackground = 0.4;
	}
}
