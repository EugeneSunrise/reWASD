using System;
using System.Threading.Tasks;

namespace reWASDEngine.Services.HttpServer.SystemMonitor
{
	public abstract class AsyncLoopMonitor
	{
		protected abstract int DELAY { get; }

		protected abstract void Check();

		public async void Monitor()
		{
			this.isMonitoring = true;
			while (this.isMonitoring)
			{
				this.Check();
				await Task.Delay(this.DELAY);
			}
		}

		public void Stop()
		{
			this.isMonitoring = false;
		}

		private bool isMonitoring;
	}
}
