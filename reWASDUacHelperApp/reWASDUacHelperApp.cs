using System;
using reWASDCommon;
using UacHelper;

namespace reWASDUACHelper
{
	public class reWASDUacHelperApp : UacHelperBaseApp
	{
		public override string WCF_MODE_MUTEX_NAME
		{
			get
			{
				return "reWASDUACWCFMutexName";
			}
		}

		protected override string PIPE_NAME
		{
			get
			{
				return "reWASDUACWCFPipeName";
			}
		}

		protected override Type ServiceType
		{
			get
			{
				return typeof(AdminOperationsService);
			}
		}

		protected override Type ServiceInterfaceType
		{
			get
			{
				return typeof(IAdminOperations);
			}
		}

		protected override int WaitTimeout
		{
			get
			{
				return 10000;
			}
		}

		public static void Main(string[] args)
		{
			new reWASDUacHelperApp().Run();
		}

		public const string REWASD_UAC_WCF_MODE_MUTEX_NAME = "reWASDUACWCFMutexName";

		public const string REWASD_UAC_WCF_PIPE_NAME = "reWASDUACWCFPipeName";
	}
}
