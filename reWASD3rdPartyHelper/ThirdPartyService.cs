using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using CoreWCF;
using DiscSoft.NET.Common.ColorStuff;
using DiscSoft.NET.Common.Interfaces;
using DiscSoft.NET.Common.WCF;
using reWASD3rdPartyHelper._3dPartyManufacturersAPI;
using reWASDCommon;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Interfaces;
using reWASDCommon._3dPartyManufacturersAPI;

namespace reWASD3rdPartyHelper
{
	[ServiceBehavior(ConcurrencyMode = 0, InstanceContextMode = 0, UseSynchronizationContext = false)]
	public class ThirdPartyService : BaseInterProcessCommunicationServiceWCF, IThirdPartyOperations, IInterProcessCommunicationServiceWCF, IDisposable
	{
		public ThirdPartyService()
		{
			this._3rdPartyDeviceColorChangers = new List<IDeviceColorChanger>();
			this._3rdPartyDeviceColorChangers.Add(new RazerAPI(this.pumpFinishedEvent));
			new Thread(new ThreadStart(this.WaitForCloseThreadFunc)).Start();
			ThirdPartyServiceApp.OnReinit += this.OnReinit;
		}

		private void OnReinit(object sender, EventArgs e)
		{
			foreach (IDeviceColorChanger deviceColorChanger in this._3rdPartyDeviceColorChangers)
			{
				deviceColorChanger.ReInit();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			if (disposing)
			{
				this.ExitHelper();
			}
			this.disposed = true;
		}

		~ThirdPartyService()
		{
			this.Dispose(false);
		}

		public void WaitForCloseThreadFunc()
		{
			while (this.pumpFinishedEvent.WaitOne() && !BaseInterProcessCommunicationServiceWCF.ClosePending)
			{
				Thread.Sleep(3000);
				bool flag = false;
				if (BaseInterProcessCommunicationServiceWCF.ClosePending)
				{
					break;
				}
				if (BaseInterProcessCommunicationServiceWCF.RunningTasksCounter <= 0)
				{
					using (List<IDeviceColorChanger>.Enumerator enumerator = this._3rdPartyDeviceColorChangers.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.IsPumpingEffectActive)
							{
								flag = true;
							}
						}
					}
					if (!flag)
					{
						break;
					}
				}
			}
			this.ExitHelper();
		}

		public bool ExitHelper()
		{
			foreach (IDeviceColorChanger deviceColorChanger in this._3rdPartyDeviceColorChangers)
			{
				deviceColorChanger.Stop(true, true, true, true);
				deviceColorChanger.Deinitialize();
			}
			BaseInterProcessCommunicationServiceWCF.ClosePending = true;
			BaseInterProcessCommunicationServiceWCF.RunningTasksCounter = 0;
			this.pumpFinishedEvent.Set();
			return true;
		}

		public bool IsAnyServiceRunning()
		{
			if (BaseInterProcessCommunicationServiceWCF.ClosePending)
			{
				return false;
			}
			return this._3rdPartyDeviceColorChangers.Any((IDeviceColorChanger cc) => cc.IsServiceRunning);
		}

		public bool Stop(bool resetColor = true, bool resetPlayerLed = true, bool stopMice = true, bool stopKeyboards = true)
		{
			if (BaseInterProcessCommunicationServiceWCF.ClosePending)
			{
				return false;
			}
			foreach (IDeviceColorChanger deviceColorChanger in this._3rdPartyDeviceColorChangers)
			{
				deviceColorChanger.Stop(resetColor, resetPlayerLed, stopMice, stopKeyboards);
			}
			return true;
		}

		public bool Stop2(LEDDeviceInfo ledDeviceInfo, bool resetColor = true, bool resetPlayerLed = true)
		{
			if (BaseInterProcessCommunicationServiceWCF.ClosePending)
			{
				return false;
			}
			foreach (IDeviceColorChanger deviceColorChanger in this._3rdPartyDeviceColorChangers)
			{
				deviceColorChanger.Stop(ledDeviceInfo, resetColor, resetPlayerLed, false);
			}
			return true;
		}

		public bool Deinitialize()
		{
			foreach (IDeviceColorChanger deviceColorChanger in this._3rdPartyDeviceColorChangers)
			{
				deviceColorChanger.Deinitialize();
			}
			return true;
		}

		public bool SetColor1(zColor color, LEDColorMode ledColorMode = 1, int durationMS = 5000)
		{
			if (BaseInterProcessCommunicationServiceWCF.ClosePending)
			{
				return false;
			}
			BaseInterProcessCommunicationServiceWCF.RunningTasksCounter++;
			try
			{
				foreach (IDeviceColorChanger deviceColorChanger in this._3rdPartyDeviceColorChangers)
				{
					deviceColorChanger.SetColor(color, ledColorMode, durationMS);
				}
			}
			finally
			{
				BaseInterProcessCommunicationServiceWCF.RunningTasksCounter--;
			}
			return true;
		}

		public bool SetColor2(Color color, LEDDeviceInfo ledDeviceInfo, LEDColorMode ledColorMode = 1, int durationMS = 5000, bool isAllowColorSolidOrchestratorRedirect = true)
		{
			if (BaseInterProcessCommunicationServiceWCF.ClosePending)
			{
				return false;
			}
			BaseInterProcessCommunicationServiceWCF.RunningTasksCounter++;
			try
			{
				foreach (IDeviceColorChanger deviceColorChanger in this._3rdPartyDeviceColorChangers)
				{
					deviceColorChanger.SetColor(color, ledDeviceInfo, ledColorMode, durationMS, isAllowColorSolidOrchestratorRedirect);
				}
			}
			finally
			{
				BaseInterProcessCommunicationServiceWCF.RunningTasksCounter--;
			}
			return true;
		}

		public bool SetColor3(zColor color, LEDDeviceInfo ledDeviceInfo, LEDColorMode ledColorMode = 1, int durationMS = 5000, bool isAllowColorSolidOrchestratorRedirect = true)
		{
			if (BaseInterProcessCommunicationServiceWCF.ClosePending)
			{
				return false;
			}
			BaseInterProcessCommunicationServiceWCF.RunningTasksCounter++;
			try
			{
				foreach (IDeviceColorChanger deviceColorChanger in this._3rdPartyDeviceColorChangers)
				{
					deviceColorChanger.SetColor(color, ledDeviceInfo, ledColorMode, durationMS, isAllowColorSolidOrchestratorRedirect);
				}
			}
			finally
			{
				BaseInterProcessCommunicationServiceWCF.RunningTasksCounter--;
			}
			return true;
		}

		public bool IncrementActiveConfigs()
		{
			BaseInterProcessCommunicationServiceWCF.RunningTasksCounter++;
			return true;
		}

		public bool DecrementActiveConfigs()
		{
			if (BaseInterProcessCommunicationServiceWCF.RunningTasksCounter != 0)
			{
				BaseInterProcessCommunicationServiceWCF.RunningTasksCounter--;
			}
			this.pumpFinishedEvent.Set();
			return true;
		}

		private List<IDeviceColorChanger> _3rdPartyDeviceColorChangers;

		private const int MS_TO_SHUTDOWN = 3000;

		private AutoResetEvent pumpFinishedEvent = new AutoResetEvent(false);

		private bool disposed;
	}
}
