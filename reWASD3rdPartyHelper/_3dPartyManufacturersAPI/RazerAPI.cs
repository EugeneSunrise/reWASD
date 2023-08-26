using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Corale.Colore.Core;
using Corale.Colore.Razer.Keyboard.Effects;
using Corale.Colore.Razer.Mouse.Effects;
using DiscSoft.NET.Common.ColorStuff;
using DiscSoft.NET.Common.Utils;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon._3dPartyManufacturersAPI;

namespace reWASD3rdPartyHelper._3dPartyManufacturersAPI
{
	public class RazerAPI : Base3dPartyApi
	{
		public override bool IsServiceRunning
		{
			get
			{
				return this._isServiceRunning;
			}
		}

		public override bool IsColorOrchestrationAllowed
		{
			get
			{
				return true;
			}
		}

		public RazerAPI(AutoResetEvent pumpFinishedEvent)
		{
			this._pumpFinishedEvent = pumpFinishedEvent;
			this.Init();
			base.LEDShimmerToSaturateMS = 800;
			base.LEDShimmerBeatFullColorTimeMS = 10;
			base.LEDShimmerDelayMS = 80;
			base.LEDBreathToSaturateMS = 1600;
			base.LEDBreathBeatFullColorTimeMS = 30;
			base.LEDBreathDelayMS = 1000;
			base.LEDHeartBeatColorToSaturateMS = 80;
			base.LEDHeartBeatFullColorTimeMS = 0;
			base.LEDHeartBeatDelay1MS = 20;
			base.LEDHeartBeatDelay2MS = 800;
			base.LEDStroboscopeDelayBetweenFlashesMS = 80;
			base.LEDRainbowTransitionTimeSec = 10;
			base.LEDRainbowColorUpdateDelayMS = 30;
			base.UseFixedGradientUpdateTimimg = true;
			base.FixedGradientUpdateTimimgMS = 50;
		}

		public void Init()
		{
			try
			{
				Tracer.TraceWriteTag(" - LEDService", "RazerAPI.Init()", false);
				Tracer.TraceWriteTag(" - LEDService", "Chroma.SdkAvailable procceed init", false);
				this._chroma = Chroma.Instance;
				Thread.Sleep(1000);
				this._isServiceRunning = this._chroma.Initialized;
				string text = " - LEDService";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Chroma init state: ");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(this._isServiceRunning);
				Tracer.TraceWriteTag(text, defaultInterpolatedStringHandler.ToStringAndClear(), false);
				if (this._isServiceRunning)
				{
					this._defaultKeyboardEffect = this._chroma.Keyboard.CurrentEffectId;
					this._defaultMiceEffect = this._chroma.Mouse.CurrentEffectId;
				}
				this.ResetMouseEffect();
				this.ResetKeyboardEffect();
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
			Tracer.TraceWriteTag(" - LEDService", "RazerAPI.Deinitialize()", false);
		}

		public override void ReInit()
		{
			this._isServiceRunning = false;
			this.Init();
		}

		public override bool IsDeviceSupported(LEDDeviceInfo deviceInfo)
		{
			return deviceInfo.LEDSupportedDevice == 8 || deviceInfo.LEDSupportedDevice == 9;
		}

		protected override Task<bool> SetColorInternal(zColor color, LEDDeviceInfo deviceInfo, LEDColorMode ledColorMode = 1, int durationMS = 5000)
		{
			Tracer.TraceWriteTag(" - LEDService", "RazerAPI.SetColorInternal", false);
			if (!this._chroma.Initialized)
			{
				return Task.FromResult<bool>(false);
			}
			try
			{
				LEDSupportedDevice ledsupportedDevice = deviceInfo.LEDSupportedDevice;
				if (ledsupportedDevice != 8)
				{
					if (ledsupportedDevice == 9)
					{
						this._miceColor = new Color(color.R, color.G, color.B);
						string text = " - LEDService";
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 3);
						defaultInterpolatedStringHandler.AppendLiteral("Set mice R-");
						defaultInterpolatedStringHandler.AppendFormatted<byte>(color.R);
						defaultInterpolatedStringHandler.AppendLiteral(" G-");
						defaultInterpolatedStringHandler.AppendFormatted<byte>(color.G);
						defaultInterpolatedStringHandler.AppendLiteral(" B-");
						defaultInterpolatedStringHandler.AppendFormatted<byte>(color.B);
						Tracer.TraceWriteTag(text, defaultInterpolatedStringHandler.ToStringAndClear(), false);
						IChroma chroma = this._chroma;
						if (chroma != null)
						{
							chroma.Mouse.SetStatic(new Static(65535, this._miceColor));
						}
						base.SetMiceColor = true;
					}
				}
				else
				{
					this._keyboardColor = new Color(color.R, color.G, color.B);
					string text2 = " - LEDService";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 3);
					defaultInterpolatedStringHandler.AppendLiteral("Set keyboards R-");
					defaultInterpolatedStringHandler.AppendFormatted<byte>(color.R);
					defaultInterpolatedStringHandler.AppendLiteral(" G-");
					defaultInterpolatedStringHandler.AppendFormatted<byte>(color.G);
					defaultInterpolatedStringHandler.AppendLiteral(" B-");
					defaultInterpolatedStringHandler.AppendFormatted<byte>(color.B);
					Tracer.TraceWriteTag(text2, defaultInterpolatedStringHandler.ToStringAndClear(), false);
					IChroma chroma2 = this._chroma;
					if (chroma2 != null)
					{
						chroma2.Keyboard.SetStatic(new Static(this._keyboardColor));
					}
					base.SetKeyboardColor = true;
				}
			}
			catch (Win32Exception)
			{
			}
			return Task.FromResult<bool>(true);
		}

		public override async Task Stop(bool resetColor = true, bool resetPlayerLed = true, bool stopMice = true, bool stopKeyboards = true)
		{
			if (this._isServiceRunning)
			{
				if (stopMice)
				{
					this.StopMouse();
				}
				if (stopKeyboards)
				{
					this.StopKeyboard();
				}
			}
		}

		public override async Task Stop(LEDDeviceInfo ledDeviceInfo, bool resetColor = true, bool resetPlayerLed = true, bool force = false)
		{
			if (ledDeviceInfo.LEDSupportedDevice == 9)
			{
				await this.Stop(resetColor, resetPlayerLed, true, false);
			}
			if (ledDeviceInfo.LEDSupportedDevice == 8)
			{
				await this.Stop(resetColor, resetPlayerLed, false, true);
			}
		}

		private void StopKeyboard()
		{
			base.SetKeyboardColor = false;
			this._colorOrchestratorDinctionary[8].Stop();
			this.ResetKeyboardEffect();
		}

		private void ResetKeyboardEffect()
		{
			try
			{
				this._chroma.Keyboard.SetGuid(this._defaultKeyboardEffect);
			}
			catch (Exception ex)
			{
				if (Tracer.IsTextFileTraceEnabled)
				{
					Tracer.TraceException(ex, "ResetKeyboardEffect");
				}
				try
				{
					this._chroma.Keyboard.Clear();
				}
				catch (Exception ex2)
				{
					if (Tracer.IsTextFileTraceEnabled)
					{
						Tracer.TraceException(ex2, "ResetKeyboardEffect");
					}
				}
			}
		}

		private void ResetMouseEffect()
		{
			try
			{
				this._chroma.Mouse.SetGuid(this._defaultMiceEffect);
			}
			catch (Exception ex)
			{
				if (Tracer.IsTextFileTraceEnabled)
				{
					Tracer.TraceException(ex, "ResetMouseEffect");
				}
				try
				{
					this._chroma.Mouse.Clear();
				}
				catch (Exception ex2)
				{
					if (Tracer.IsTextFileTraceEnabled)
					{
						Tracer.TraceException(ex2, "ResetMouseEffect");
					}
				}
			}
		}

		private void StopMouse()
		{
			base.SetMiceColor = false;
			this._colorOrchestratorDinctionary[9].Stop();
			this.ResetMouseEffect();
		}

		private bool _isServiceRunning;

		private IChroma _chroma;

		private Color _miceColor = Color.Black;

		private Color _keyboardColor = Color.Black;

		private Guid _defaultKeyboardEffect = Guid.Empty;

		private Guid _defaultMiceEffect = Guid.Empty;
	}
}
