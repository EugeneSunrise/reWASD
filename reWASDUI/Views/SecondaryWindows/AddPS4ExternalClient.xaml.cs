using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.SecondaryWindows.Base;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Utils;
using reWASDUI.Infrastructure.Controller;

namespace reWASDUI.Views.SecondaryWindows
{
	public partial class AddPS4ExternalClient : BaseSecondaryWindow
	{
		public ConsoleType ConsoleType
		{
			get
			{
				return this._consoleType;
			}
			set
			{
				this.SetProperty(ref this._consoleType, value, "ConsoleType");
			}
		}

		public string Alias
		{
			get
			{
				return this._alias;
			}
			set
			{
				this.SetProperty(ref this._alias, value, "Alias");
			}
		}

		public string MacAddressText
		{
			get
			{
				return this._macAddressText;
			}
			set
			{
				if (this.SetProperty(ref this._macAddressText, value, "MacAddressText"))
				{
					this.OnPropertyChanged("IsSaveEnabled");
				}
			}
		}

		public bool IsSaveEnabled
		{
			get
			{
				return this._isSaveEnabled;
			}
			set
			{
				this.SetProperty(ref this._isSaveEnabled, value, "IsSaveEnabled");
			}
		}

		public ulong MacAddress { get; set; }

		public AddPS4ExternalClient()
		{
			this.InitializeComponent();
			base.DataContext = this;
			this.Alias = "Playstation 4";
			this.MacAddressText = "00:00:00:00:00:00";
			this.ConsoleType = 1;
			this.StartPoller();
			base.Closed += delegate([Nullable(2)] object o, EventArgs e)
			{
				this.StopPoller();
			};
		}

		private void StartPoller()
		{
			this._pollingTimer = new DispatcherTimer();
			this._pollingTimer.Tick += delegate([Nullable(2)] object o, EventArgs e)
			{
				this.CheckNewPS4WithMac();
			};
			this._pollingTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
			this._pollingTimer.Start();
		}

		private void StopPoller()
		{
			DispatcherTimer pollingTimer = this._pollingTimer;
			if (pollingTimer != null)
			{
				pollingTimer.Stop();
			}
			this._pollingTimer = null;
		}

		private void CheckNewPS4WithMac()
		{
			ControllerVM controllerVM = (ControllerVM)App.GamepadService.GamepadCollection.FirstOrDefault(delegate(BaseControllerVM item)
			{
				ControllerVM controllerVM2 = item as ControllerVM;
				return controllerVM2 != null && controllerVM2.ControllerType == 4 && controllerVM2.MasterBthAddr > 0UL;
			});
			if (controllerVM != null)
			{
				this.MacAddress = controllerVM.MasterBthAddr;
				this.MacAddressText = UtilsCommon.MacAddressToString(this.MacAddress, ":");
				this.IsSaveEnabled = true;
				this.StopPoller();
			}
		}

		private DispatcherTimer _pollingTimer;

		private ConsoleType _consoleType;

		private string _alias;

		private string _macAddressText;

		public bool _isSaveEnabled;
	}
}
