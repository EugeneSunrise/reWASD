using System;
using System.Windows;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.WCF;
using Microsoft.Win32;
using reWASDCommon;

namespace reWASD3rdPartyHelper
{
	public class ThirdPartyServiceApp : WCFBaseApp
	{
		protected override string PRODUCT_NAME
		{
			get
			{
				return "Disc Soft ThirdParty Service";
			}
		}

		public override string WCF_MODE_MUTEX_NAME
		{
			get
			{
				return "reWASDThirdPartyWCFMutexName";
			}
		}

		protected override string PIPE_NAME
		{
			get
			{
				return "reWASDThirdPartyWCFPipeName";
			}
		}

		protected override Type ServiceType
		{
			get
			{
				return typeof(ThirdPartyService);
			}
		}

		protected override Type ServiceInterfaceType
		{
			get
			{
				return typeof(IThirdPartyOperations);
			}
		}

		protected override int WaitTimeout
		{
			get
			{
				return int.MaxValue;
			}
		}

		public static event EventHandler OnReinit;

		public static void RaiseOnReinit()
		{
			EventHandler onReinit = ThirdPartyServiceApp.OnReinit;
			if (onReinit == null)
			{
				return;
			}
			onReinit(null, EventArgs.Empty);
		}

		private static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
		{
			if (e.Reason == SessionSwitchReason.SessionLock)
			{
				BaseInterProcessCommunicationServiceWCF.ClosePending = true;
				BaseInterProcessCommunicationServiceWCF.RunningTasksCounter = 0;
			}
		}

		public ThirdPartyServiceApp()
		{
			SystemEvents.SessionSwitch += ThirdPartyServiceApp.SystemEvents_SessionSwitch;
		}

		public static void Main(string[] args)
		{
			Application application = new ThirdPartyServiceApp();
			Tracer.PRODUCT_SHORT_NAME = "reWASD 3dPartyHelper";
			application.Run();
		}
	}
}
