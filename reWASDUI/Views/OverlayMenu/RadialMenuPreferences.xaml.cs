using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Threading;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.Controls.MultiRangeSlider;
using reWASDUI.DataModels;
using reWASDUI.Services.Interfaces;
using UtilsDisplayName;

namespace reWASDUI.Views.OverlayMenu
{
	public partial class RadialMenuPreferences : System.Windows.Controls.UserControl, INotifyPropertyChanged
	{
		public RadialMenuPreferences()
		{
			this.Monitors = new ObservableCollection<MonitorPresent>();
			this.InitializeComponent();
			base.DataContext = this;
			base.IsVisibleChanged += this.IsVisibleChangedEvent;
			this.StartPoller();
		}

		public ObservableCollection<MonitorPresent> Monitors { get; private set; }

		private void IsVisibleChangedEvent(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				this.StartPoller();
				string monitor = App.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.OverlayMenu.Circle.Monitor;
				Tracer.TraceWrite("IsVisibleChangedEvent System.Windows.Forms.Screen.AllScreens.Count(): " + Screen.AllScreens.Count<Screen>().ToString(), false);
				this.Monitors.Clear();
				int num = 0;
				int num2 = 0;
				foreach (Screen screen in Screen.AllScreens)
				{
					num2 += screen.Bounds.Width;
					num++;
					if (num >= 2)
					{
						break;
					}
				}
				double num3 = this.RootPanel.ActualWidth / (double)num2;
				num = 1;
				MonitorPresent monitorPresent = null;
				bool flag = false;
				foreach (Screen screen2 in Screen.AllScreens)
				{
					string text = ScreenInterrogatory.DeviceFriendlyName(screen2);
					MonitorPresent monitorPresent2 = new MonitorPresent
					{
						Owner = this,
						WidhtM = (int)((double)screen2.Bounds.Width * num3),
						HeightM = (int)((double)screen2.Bounds.Height * num3),
						Number = num,
						NameM = text,
						IsActive = false
					};
					if (screen2.Primary)
					{
						monitorPresent = monitorPresent2;
					}
					if (monitor.Equals(text))
					{
						monitorPresent2.IsActive = true;
						flag = true;
					}
					this.Monitors.Add(monitorPresent2);
					num++;
				}
				if (!flag)
				{
					monitorPresent.IsActive = true;
				}
				this.OnPropertyChanged("IsManyMonitors");
				this.OnPropertyChanged("Monitors");
				return;
			}
			this.StopPoller();
			MultiRangeSlider.FireCloseAllPopups();
		}

		public virtual void Dispose()
		{
			this.StopPoller();
			this.PropertyChanged = null;
		}

		public bool IsManyMonitors
		{
			get
			{
				ObservableCollection<MonitorPresent> monitors = this.Monitors;
				return monitors != null && monitors.Count > 1;
			}
		}

		public string SelectedMonitor
		{
			get
			{
				string text = "";
				foreach (MonitorPresent monitorPresent in this.Monitors)
				{
					if (monitorPresent.IsActive)
					{
						text = monitorPresent.NameM;
						break;
					}
				}
				return text;
			}
		}

		public bool vFrame1
		{
			get
			{
				return this._vFrame1;
			}
			set
			{
				this.SetProperty<bool>(ref this._vFrame1, value, "vFrame1");
			}
		}

		public bool vFrame2
		{
			get
			{
				return this._vFrame2;
			}
			set
			{
				this.SetProperty<bool>(ref this._vFrame2, value, "vFrame2");
			}
		}

		public bool vFrame3
		{
			get
			{
				return this._vFrame3;
			}
			set
			{
				this.SetProperty<bool>(ref this._vFrame3, value, "vFrame3");
			}
		}

		private void StartPoller()
		{
			this._timer = new DispatcherTimer();
			this._timer.Tick += delegate([Nullable(2)] object o, EventArgs e)
			{
				this.OnTickTimer();
			};
			this._timer.Interval = new TimeSpan(0, 0, 0, 0, 25);
			this._timer.Start();
			this.time = DateTime.Now;
		}

		private void StopPoller()
		{
			DispatcherTimer timer = this._timer;
			if (timer != null)
			{
				timer.Stop();
			}
			this._timer = null;
		}

		private void OnTickTimer()
		{
			int num = 5000;
			if (App.GameProfilesService != null)
			{
				IGameProfilesService gameProfilesService = App.GameProfilesService;
				if (((gameProfilesService != null) ? gameProfilesService.CurrentGame : null) != null)
				{
					IGameProfilesService gameProfilesService2 = App.GameProfilesService;
					bool flag;
					if (gameProfilesService2 == null)
					{
						flag = null != null;
					}
					else
					{
						GameVM currentGame = gameProfilesService2.CurrentGame;
						flag = ((currentGame != null) ? currentGame.CurrentConfig : null) != null;
					}
					if (flag)
					{
						IGameProfilesService gameProfilesService3 = App.GameProfilesService;
						bool flag2;
						if (gameProfilesService3 == null)
						{
							flag2 = null != null;
						}
						else
						{
							GameVM currentGame2 = gameProfilesService3.CurrentGame;
							flag2 = ((currentGame2 != null) ? currentGame2.CurrentConfig.ConfigData : null) != null;
						}
						if (flag2)
						{
							IGameProfilesService gameProfilesService4 = App.GameProfilesService;
							bool flag3;
							if (gameProfilesService4 == null)
							{
								flag3 = null != null;
							}
							else
							{
								GameVM currentGame3 = gameProfilesService4.CurrentGame;
								flag3 = ((currentGame3 != null) ? currentGame3.CurrentConfig.ConfigData.OverlayMenu : null) != null;
							}
							if (flag3)
							{
								IGameProfilesService gameProfilesService5 = App.GameProfilesService;
								bool flag4;
								if (gameProfilesService5 == null)
								{
									flag4 = null != null;
								}
								else
								{
									GameVM currentGame4 = gameProfilesService5.CurrentGame;
									flag4 = ((currentGame4 != null) ? currentGame4.CurrentConfig.ConfigData.OverlayMenu.Circle : null) != null;
								}
								if (flag4)
								{
									int num2 = num + App.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.OverlayMenu.Circle.DelayBeforeOpening;
									int num3 = num2 + 5000;
									TimeSpan timeSpan = DateTime.Now - this.time;
									if (timeSpan.TotalMilliseconds < (double)num)
									{
										this.vFrame1 = true;
										this.vFrame2 = false;
										this.vFrame3 = false;
										return;
									}
									if (timeSpan.TotalMilliseconds >= (double)num && timeSpan.TotalMilliseconds < (double)num2)
									{
										this.vFrame1 = false;
										this.vFrame2 = true;
										this.vFrame3 = false;
										return;
									}
									if (timeSpan.TotalMilliseconds >= (double)num2 && timeSpan.TotalMilliseconds < (double)num3)
									{
										this.vFrame1 = false;
										this.vFrame2 = false;
										this.vFrame3 = true;
										return;
									}
									this.time = DateTime.Now;
									return;
								}
							}
						}
					}
				}
			}
			this.vFrame1 = true;
			this.vFrame2 = false;
			this.vFrame3 = false;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (object.Equals(storage, value))
			{
				return false;
			}
			storage = value;
			this.ValidateProperty(propertyName, value);
			this.OnPropertyChanged(propertyName);
			return true;
		}

		public void ClearAllActive()
		{
			foreach (MonitorPresent monitorPresent in this.Monitors)
			{
				monitorPresent.IsActive = false;
			}
		}

		public void SaveActive()
		{
			App.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.OverlayMenu.Circle.Monitor = this.SelectedMonitor;
			foreach (MonitorPresent monitorPresent in this.Monitors)
			{
				monitorPresent.UpdateProperty();
			}
		}

		public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			PropertyChangedEventArgs propertyChangedEventArgs = new PropertyChangedEventArgs(propertyName);
			propertyChanged(this, propertyChangedEventArgs);
		}

		protected virtual void ValidateProperty(string propertyName, object newValue)
		{
		}

		private bool _vFrame1 = true;

		private bool _vFrame2;

		private bool _vFrame3;

		private DateTime time;

		private DispatcherTimer _timer;
	}
}
