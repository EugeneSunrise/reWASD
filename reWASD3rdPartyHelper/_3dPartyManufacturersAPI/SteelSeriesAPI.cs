using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using DiscSoft.NET.Common.ColorStuff;
using DiscSoft.NET.Common.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon._3dPartyManufacturersAPI;

namespace reWASD3rdPartyHelper._3dPartyManufacturersAPI
{
	public class SteelSeriesAPI : Base3dPartyJsonServerAPI
	{
		public override bool IsServiceRunning
		{
			get
			{
				return File.Exists(SteelSeriesAPI.PathToConfiguration);
			}
		}

		public override bool IsColorOrchestrationAllowed
		{
			get
			{
				return false;
			}
		}

		public SteelSeriesAPI()
		{
			this.Init();
			this.RegisterMetaData();
		}

		public void Init()
		{
			try
			{
				if (this.IsServiceRunning)
				{
					JObject jobject = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(SteelSeriesAPI.PathToConfiguration));
					string text = "http://";
				}
			}
			catch (Exception ex)
			{
				if (Tracer.IsTextFileTraceEnabled)
				{
					Tracer.TraceException(ex, "Init");
				}
			}
		}

		public override void Deinitialize()
		{
			this.Unbind();
		}

		public override void ReInit()
		{
		}

		public override bool IsDeviceSupported(LEDDeviceInfo deviceInfo)
		{
			return deviceInfo.LEDSupportedDevice == 8 || deviceInfo.LEDSupportedDevice == 9;
		}

		protected override async Task<bool> SetColorInternal(zColor color, LEDDeviceInfo deviceInfo, LEDColorMode ledColorMode = 1, int durationMS = 5000)
		{
			await this.SendToServerEndpoint(this.CreateColorObject(color.GetColor(), ledColorMode, durationMS, deviceInfo.LEDSupportedDevice), "bind_game_event");
			await this.SendToServerEndpoint(this.CreateEventObject(deviceInfo.LEDSupportedDevice), "game_event");
			return true;
		}

		private JObject CreateColorObject(Color color, LEDColorMode colorMode, int durationMS, LEDSupportedDevice device)
		{
			JObject jobject = new JObject();
			jobject["game"] = SteelSeriesAPI.GAME_NAME;
			JObject jobject2 = new JObject();
			if (device != 8)
			{
				if (device == 9)
				{
					jobject["event"] = "SETCOLORMOUSE";
					jobject2["device-type"] = "mouse";
					base.SetMiceColor = true;
				}
			}
			else
			{
				jobject["event"] = "SETCOLORKEYBOARD";
				jobject2["device-type"] = "keyboard";
				base.SetKeyboardColor = true;
			}
			jobject2["zone"] = "all";
			jobject2["mode"] = "color";
			JObject jobject3 = new JObject();
			jobject3["red"] = color.R;
			jobject3["green"] = color.G;
			jobject3["blue"] = color.B;
			jobject2["color"] = jobject3;
			jobject["handlers"] = new JArray(jobject2);
			return jobject;
		}

		private JObject CreateEventObject(LEDSupportedDevice device)
		{
			JObject jobject = new JObject();
			jobject["game"] = SteelSeriesAPI.GAME_NAME;
			jobject["event"] = "SETCOLORMOUSE";
			JObject jobject2 = new JObject();
			jobject2["value"] = 100;
			jobject["data"] = jobject2;
			if (device != 8)
			{
				if (device != 9)
				{
					throw new ArgumentOutOfRangeException("device", device, null);
				}
				jobject["event"] = "SETCOLORMOUSE";
			}
			else
			{
				jobject["event"] = "SETCOLORKEYBOARD";
			}
			return jobject;
		}

		public override Task Stop(bool resetColor = true, bool resetPlayerLed = true, bool stopMice = true, bool stopKeyboards = true)
		{
			this.Unbind();
			return Task.CompletedTask;
		}

		public override async Task Stop(LEDDeviceInfo ledDeviceInfo, bool resetColor = true, bool resetPlayerLed = true, bool force = false)
		{
			await this.Stop(true, true, true, true);
		}

		protected override Task<string> Send(JObject jsonObject, string fullAdress)
		{
			if (!this.IsServiceRunning)
			{
				return null;
			}
			return base.Send(jsonObject, fullAdress);
		}

		protected override Task<string> SendToServerEndpoint(JObject jsonObject, string endPoint)
		{
			if (!this.IsServiceRunning)
			{
				return null;
			}
			return base.SendToServerEndpoint(jsonObject, endPoint);
		}

		protected async Task PumpMouse()
		{
			await this.SendEvent("SETCOLORMOUSE");
		}

		protected async Task PumpKeyboard()
		{
			await this.SendEvent("SETCOLORKEYBOARD");
		}

		private async Task SendEvent(string eventName)
		{
			JObject jobject = new JObject();
			jobject["game"] = SteelSeriesAPI.GAME_NAME;
			jobject["event"] = eventName;
			JObject jobject2 = new JObject();
			jobject2["value"] = 100;
			jobject["data"] = jobject2;
			await this.SendToServerEndpoint(jobject, "game_event");
		}

		private void RegisterMetaData()
		{
			if (!this.IsServiceRunning)
			{
				return;
			}
			JObject jobject = new JObject();
			jobject["game"] = SteelSeriesAPI.GAME_NAME;
			jobject["game_display_name"] = "reWASD";
			jobject["developer"] = "Disc Soft";
			this.SendToServerEndpoint(jobject, "game_metadata");
		}

		private async void Unbind()
		{
			if (this.IsServiceRunning)
			{
				JObject jsonObject = new JObject();
				jsonObject["game"] = SteelSeriesAPI.GAME_NAME;
				jsonObject["event"] = "SETCOLORMOUSE";
				await this.SendToServerEndpoint(jsonObject, "remove_game_event");
				jsonObject["event"] = "SETCOLORKEYBOARD";
				await this.SendToServerEndpoint(jsonObject, "remove_game_event");
			}
		}

		public static string PathToConfiguration = Environment.ExpandEnvironmentVariables("%PROGRAMDATA%") + "\\SteelSeries\\SteelSeries Engine 3\\coreProps.json";

		private static readonly string GAME_NAME = "reWASD".ToUpperInvariant();

		private const string SET_MOUSE_COLOR_EVENT = "SETCOLORMOUSE";

		private const string SET_KEYBOARD_COLOR_EVENT = "SETCOLORKEYBOARD";
	}
}
