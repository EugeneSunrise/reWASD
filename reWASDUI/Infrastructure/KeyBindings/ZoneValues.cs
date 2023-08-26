using System;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using Newtonsoft.Json;
using reWASDCommon.Infrastructure.KeyBindingsModel;

namespace reWASDUI.Infrastructure.KeyBindings
{
	[JsonObject(0)]
	public class ZoneValues : ZBindableBase
	{
		public ushort Low
		{
			get
			{
				return this._Low;
			}
			set
			{
				this.SetProperty<ushort>(ref this._Low, value, "Low");
			}
		}

		public ushort Med
		{
			get
			{
				return this._Med;
			}
			set
			{
				this.SetProperty<ushort>(ref this._Med, value, "Med");
			}
		}

		public ushort High
		{
			get
			{
				return this._High;
			}
			set
			{
				this.SetProperty<ushort>(ref this._High, value, "High");
			}
		}

		public ushort RightDeadZone
		{
			get
			{
				return this._RightDeadZone;
			}
			set
			{
				this.SetProperty<ushort>(ref this._RightDeadZone, value, "RightDeadZone");
			}
		}

		public ZoneValues(ushort maxValue, ushort low = 10)
		{
			this._defaltMaxValue = maxValue;
			this._defaultLow = low;
			this.Clear();
		}

		public void Clear()
		{
			this._RightDeadZone = this._defaltMaxValue;
			this.Low = this._defaultLow;
			this.Med = (ushort)Math.Floor((double)(this.Low + (this.RightDeadZone - this.Low) / 3));
			this.High = (ushort)Math.Floor((double)(this.Low + (this.RightDeadZone - this.Low) / 3 * 2));
		}

		public void CopyToModel(ZoneValues model)
		{
			model.High = this.High;
			model.Med = this.Med;
			model.Low = this.Low;
			model.RightDeadZone = this.RightDeadZone;
		}

		public void CopyFromModel(ZoneValues model)
		{
			this.High = model.High;
			this.Med = model.Med;
			this.Low = model.Low;
			this.RightDeadZone = model.RightDeadZone;
		}

		private ushort _Low;

		private ushort _Med;

		private ushort _High;

		private ushort _RightDeadZone;

		private ushort _defaltMaxValue;

		private ushort _defaultLow = 10;
	}
}
