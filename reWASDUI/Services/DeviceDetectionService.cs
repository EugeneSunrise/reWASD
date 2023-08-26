using System;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using Prism.Events;
using reWASDUI.Infrastructure.Controller;
using XBEliteWPF.Services.Interfaces;

namespace reWASDUI.Services
{
	public class DeviceDetectionService : ZBindableBase, IDeviceDetectionService
	{
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isEnabled, value, "IsEnabled"))
				{
					if (this._isEnabled)
					{
						this.StartPollingTimer();
						return;
					}
					this.StopPollingTimer();
				}
			}
		}

		public DeviceDetectionService(IEventAggregator ea)
		{
			this._eventAggregator = ea;
		}

		private void StartPollingTimer()
		{
			this._stopDetectionTimer = new DispatcherTimer();
			this._stopDetectionTimer.Tick += delegate([Nullable(2)] object o, EventArgs e)
			{
				this.StopPollingTimer();
			};
			this._stopDetectionTimer.Interval = new TimeSpan(0, 0, 30);
			this._stopDetectionTimer.Start();
			this._pollingTimer = new DispatcherTimer();
			this._pollingTimer.Tick += delegate([Nullable(2)] object o, EventArgs e)
			{
				this.CheckButtonPressed();
			};
			this._pollingTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
			this._pollingTimer.Start();
		}

		private void StopPollingTimer()
		{
			this._stopDetectionTimer.Stop();
			this._pollingTimer.Stop();
			this._isEnabled = false;
			this.OnPropertyChanged("IsEnabled");
		}

		private void CheckButtonPressed()
		{
			foreach (BaseControllerVM baseControllerVM in App.GamepadService.GamepadCollection)
			{
				baseControllerVM.CheckControllerPressedButton();
			}
		}

		private IEventAggregator _eventAggregator;

		private bool _isEnabled;

		private DispatcherTimer _stopDetectionTimer;

		private DispatcherTimer _pollingTimer;
	}
}
