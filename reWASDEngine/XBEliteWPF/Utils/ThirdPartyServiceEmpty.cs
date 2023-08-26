using System;
using System.Threading.Tasks;
using System.Windows.Media;
using DiscSoft.NET.Common.ColorStuff;
using DiscSoft.NET.Common.Interfaces;
using reWASDCommon;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon._3dPartyManufacturersAPI;

namespace XBEliteWPF.Utils
{
	public class ThirdPartyServiceEmpty : IThirdPartyOperations, IInterProcessCommunicationServiceWCF
	{
		public bool ExitHelper()
		{
			return true;
		}

		public bool IsAnyServiceRunning()
		{
			return true;
		}

		public bool Stop(bool resetColor = true, bool resetPlayerLed = true, bool stopMice = true, bool stopKeyboards = true)
		{
			return true;
		}

		public bool Stop2(LEDDeviceInfo ledDeviceInfo, bool resetColor = true, bool resetPlayerLed = true)
		{
			return true;
		}

		public bool SetColor1(zColor color, LEDColorMode ledColorMode = 1, int durationMS = 5000)
		{
			return true;
		}

		public bool SetColor2(Color color, LEDDeviceInfo ledDeviceInfo, LEDColorMode ledColorMode = 1, int durationMS = 5000, bool isAllowColorSolidOrchestratorRedirect = true)
		{
			return true;
		}

		public bool SetColor3(zColor color, LEDDeviceInfo ledDeviceInfo, LEDColorMode ledColorMode = 1, int durationMS = 5000, bool isAllowColorSolidOrchestratorRedirect = true)
		{
			return true;
		}

		public bool IncrementActiveConfigs()
		{
			return true;
		}

		public bool DecrementActiveConfigs()
		{
			return true;
		}

		public bool Ping()
		{
			return true;
		}

		public Task<bool> PingAsyncZ()
		{
			return Task.FromResult<bool>(true);
		}
	}
}
