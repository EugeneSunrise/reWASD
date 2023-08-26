using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using DTOverlay;

namespace reWASDEngine.Services.OverlayAPI
{
	internal class SilentApply
	{
		public SilentApply(OverlayMenuCircleE iCircle)
		{
			this.Circle = iCircle;
			this.angles = new ArrayList();
			for (int i = 0; i < this.Period(); i++)
			{
				this.angles.Add(new SilentApply.TYPE_AngleInfo());
			}
		}

		~SilentApply()
		{
			this.StopPoller();
		}

		private double calcDeltaAngle()
		{
			double num = 0.0;
			double num2 = double.MaxValue;
			int num3 = 0;
			foreach (object obj in this.angles)
			{
				SilentApply.TYPE_AngleInfo type_AngleInfo = (SilentApply.TYPE_AngleInfo)obj;
				if (type_AngleInfo.time != null)
				{
					num3++;
					if (num2 < 1.7976931348623157E+308)
					{
						double num4 = type_AngleInfo.angle - num2;
						num2 = type_AngleInfo.angle;
						num += num4;
					}
					else
					{
						num2 = type_AngleInfo.angle;
					}
				}
			}
			if (num3 < this.Period())
			{
				num = double.MaxValue;
				return num;
			}
			num = Math.Abs(num);
			return num;
		}

		public void StartPoller()
		{
			this._pollingTimer = new DispatcherTimer();
			this._pollingTimer.Tick += delegate([Nullable(2)] object o, EventArgs e)
			{
				this.OnTickTimer();
			};
			this._pollingTimer.Interval = new TimeSpan(0, 0, 0, 0, this.PerionTime());
			this._pollingTimer.Start();
		}

		public void StopPoller()
		{
			DispatcherTimer pollingTimer = this._pollingTimer;
			if (pollingTimer != null)
			{
				pollingTimer.Stop();
			}
			this._pollingTimer = null;
		}

		private void OnTickTimer()
		{
			int currentSectorIndex = this.Circle.CurrentSectorIndex;
			if (this.PrevActive == currentSectorIndex)
			{
				this.numberConstActive++;
			}
			else
			{
				this.numberConstActive = 0;
			}
			if (this.numberConstActive > this.Period() && this.calcDeltaAngle() < this.MaxDeltaAngle())
			{
				this.Circle.ApplyTimer();
				this.numberConstActive = 0;
			}
			if (this.lastReportedAngle < 1.7976931348623157E+308)
			{
				bool flag = false;
				foreach (object obj in this.angles)
				{
					SilentApply.TYPE_AngleInfo type_AngleInfo = (SilentApply.TYPE_AngleInfo)obj;
					if (type_AngleInfo.time == null)
					{
						type_AngleInfo.time = new DateTime?(DateTime.Now);
						type_AngleInfo.angle = this.lastReportedAngle;
						flag = true;
						break;
					}
				}
				SilentApply.TYPE_AngleInfo type_AngleInfo2 = (SilentApply.TYPE_AngleInfo)this.angles[0];
				if (!flag)
				{
					foreach (object obj2 in this.angles)
					{
						SilentApply.TYPE_AngleInfo type_AngleInfo3 = (SilentApply.TYPE_AngleInfo)obj2;
						if (type_AngleInfo2.time > type_AngleInfo3.time)
						{
							type_AngleInfo2 = type_AngleInfo3;
						}
					}
					type_AngleInfo2.time = new DateTime?(DateTime.Now);
					type_AngleInfo2.angle = this.lastReportedAngle;
				}
			}
			foreach (object obj3 in this.angles)
			{
				SilentApply.TYPE_AngleInfo type_AngleInfo4 = (SilentApply.TYPE_AngleInfo)obj3;
				if (type_AngleInfo4.time != null && (DateTime.Now - type_AngleInfo4.time.Value).TotalMilliseconds > (double)((this.Period() + 1) * this.PerionTime()))
				{
					type_AngleInfo4.time = null;
				}
			}
			this.PrevActive = currentSectorIndex;
		}

		private int Period()
		{
			return 2 + (this.Circle.DelayBeforeOpening / 500 - 1);
		}

		private int PerionTime()
		{
			return this.Circle.DelayBeforeOpening / 10;
		}

		private double MaxDeltaAngle()
		{
			return 0.01;
		}

		private OverlayMenuCircleE Circle;

		private int PrevActive = -1;

		public double lastReportedAngle = double.MaxValue;

		private int numberConstActive;

		private ArrayList angles;

		private DispatcherTimer _pollingTimer;

		private class TYPE_AngleInfo
		{
			public TYPE_AngleInfo()
			{
				this.time = null;
			}

			public DateTime? time;

			public double angle;
		}
	}
}
