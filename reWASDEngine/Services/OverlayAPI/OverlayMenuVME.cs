using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using DTOverlay;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.KeyBindingsModel.OverlayMenu;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtil;

namespace reWASDEngine.Services.OverlayAPI
{
	public class OverlayMenuVME : ZBindableBase
	{
		public OverlayMenuVME(OverlayMenuE ioverlayMenuE, OverlayMenuVM model, ushort serviceProfileId, List<ulong> touchPad1IDs, List<ulong> touchPad2IDs, List<ulong> mouseIDs, List<ulong> stickLeftIDs, List<ulong> stickRighrIDs, List<ulong> addStickIDs, List<ulong> touchPad1ClickRequiredIDs, List<ulong> touchPad2ClickRequiredIDs, bool itoggle)
		{
			this.overlayMenuE = ioverlayMenuE;
			this.toggle = itoggle;
			this.TouchPad1IDs = touchPad1IDs;
			this.TouchPad2IDs = touchPad2IDs;
			this.MouseIDs = mouseIDs;
			this.StickLeftIDs = stickLeftIDs;
			this.StickRighrIDs = stickRighrIDs;
			this.AddStickIDs = addStickIDs;
			this.TouchPad1ClickRequiredIDs = touchPad1ClickRequiredIDs;
			this.TouchPad2ClickRequiredIDs = touchPad2ClickRequiredIDs;
			if (model != null)
			{
				this.Circle = new OverlayMenuCircleE(model.Menu, serviceProfileId, false, itoggle, null, -1);
				this.silentApply = new SilentApply(this.Circle);
				this.IsLoaded = this.Circle.IsLoaded;
			}
		}

		public OverlayMenuCircleE Circle
		{
			get
			{
				return this._currentMenuCircle;
			}
			set
			{
				this.SetProperty<OverlayMenuCircleE>(ref this._currentMenuCircle, value, "Circle");
			}
		}

		public void StartPoller()
		{
			if (this.Circle.IsDelayBeforeOpening)
			{
				this.silentApply.StartPoller();
			}
		}

		public void StopPoller()
		{
			this.silentApply.StopPoller();
		}

		private bool IsActiveDevice()
		{
			bool flag = false;
			foreach (KeyValuePair<ulong, OverlayMenuVME.DeviceInfo> keyValuePair in this.devicesInfo)
			{
				if (keyValuePair.Value.isActive)
				{
					flag = true;
					break;
				}
			}
			return flag;
		}

		private OverlayMenuVME.DeviceInfo GetActiveDevice()
		{
			OverlayMenuVME.DeviceInfo deviceInfo = null;
			foreach (KeyValuePair<ulong, OverlayMenuVME.DeviceInfo> keyValuePair in this.devicesInfo)
			{
				if (keyValuePair.Value.isActive)
				{
					deviceInfo = keyValuePair.Value;
				}
			}
			return deviceInfo;
		}

		public void SetMouseInitPosition(long x, long y)
		{
			this.MouseXDeltaInit = (float)x;
			this.MouseYDeltaInit = (float)y;
		}

		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this.SetProperty<string>(ref this._name, value, "Name");
			}
		}

		public float Transparent
		{
			get
			{
				return this._transparent;
			}
			set
			{
				this.SetProperty<float>(ref this._transparent, value, "Transparent");
			}
		}

		private bool SetActiveSector()
		{
			bool flag = false;
			if (this.IsActiveDevice())
			{
				flag = true;
				OverlayMenuVME.DeviceInfo activeDevice = this.GetActiveDevice();
				Vector2 newPos = new Vector2(0f, 0f);
				double num = 0.019999999552965164;
				if (activeDevice != null)
				{
					newPos = activeDevice.newPos;
					num = activeDevice.DeadZone;
				}
				if ((double)Math.Abs(newPos.X) < num && (double)Math.Abs(newPos.Y) < num)
				{
					this.Circle.StickAtHome();
					this.Circle.NavigatorVisible = false;
					this.silentApply.lastReportedAngle = double.MaxValue;
					this.overlayMenuE.NeedInitMousePos();
				}
				else
				{
					this.Circle.NavigatorVisible = true;
					double num2 = this.CalcAngle((double)newPos.X, (double)newPos.Y);
					Math.Sqrt((double)(newPos.X * newPos.X + newPos.Y * newPos.Y));
					this.silentApply.lastReportedAngle = num2;
					this.Circle.SetActiveSector(num2, false);
				}
			}
			return flag;
		}

		private bool IsEntryFromTouchPad(ulong id, List<ulong> clickRequiredIDs, List<GamepadButtonDescription> buttons, GamepadButton button)
		{
			bool flag = true;
			if (clickRequiredIDs.Contains(id))
			{
				flag = false;
				if (buttons.Exists((GamepadButtonDescription x) => x.Button == button))
				{
					flag = true;
				}
			}
			return flag;
		}

		public bool OverlayKeyPressed(ControllerTypeEnum controllerType, GamepadState gamepadState, ulong id)
		{
			if (!this.devicesInfo.ContainsKey(id))
			{
				this.devicesInfo[id] = new OverlayMenuVME.DeviceInfo();
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			OverlayMenuVME.DeviceInfo deviceInfo = this.devicesInfo[id];
			deviceInfo.DeadZone = 0.001;
			deviceInfo.oldPos = this.devicesInfo[id].newPos;
			foreach (GamepadButtonDescription gamepadButtonDescription in gamepadState.PressedButtons)
			{
				string overlaySVGElementName = XBUtils.GetOverlaySVGElementName(gamepadButtonDescription.Button);
				if (this.StickRighrIDs.Contains(id) && overlaySVGElementName.CompareTo("Rstick") == 0)
				{
					deviceInfo.newPos.X = (float)gamepadState.RightStickX / 32767f;
					deviceInfo.newPos.Y = (float)gamepadState.RightStickY / 32767f;
					flag = true;
					deviceInfo.Event();
				}
				if (this.StickLeftIDs.Contains(id) && overlaySVGElementName.CompareTo("Lstick") == 0)
				{
					deviceInfo.newPos.X = (float)gamepadState.LeftStickX / 32767f;
					deviceInfo.newPos.Y = (float)gamepadState.LeftStickY / 32767f;
					flag2 = true;
					deviceInfo.Event();
				}
				if (this.TouchPad1IDs.Contains(id) && this.IsEntryFromTouchPad(id, this.TouchPad1ClickRequiredIDs, gamepadState.PressedButtons, 99) && overlaySVGElementName.CompareTo("Track1") == 0)
				{
					ushort num = 0;
					ushort num2 = 0;
					if (ControllerTypeExtensions.GetControllerTouchpadWidthHeight(controllerType, ref num, ref num2))
					{
						deviceInfo.newPos.X = (float)gamepadState.LeftFingerX / (float)num * 2f - 1f;
						deviceInfo.newPos.Y = (float)(num2 - gamepadState.LeftFingerY) / (float)num2 * 2f - 1f;
						flag2 = true;
						deviceInfo.Event();
					}
				}
				if (this.TouchPad2IDs.Contains(id) && this.IsEntryFromTouchPad(id, this.TouchPad2ClickRequiredIDs, gamepadState.PressedButtons, 108) && overlaySVGElementName.CompareTo("Track2") == 0)
				{
					ushort num3 = 0;
					ushort num4 = 0;
					if (ControllerTypeExtensions.GetControllerTouchpadWidthHeight(controllerType, ref num3, ref num4))
					{
						deviceInfo.newPos.X = (float)gamepadState.RightFingerX / (float)num3 * 2f - 1f;
						deviceInfo.newPos.Y = (float)(num4 - gamepadState.RightFingerY) / (float)num4 * 2f - 1f;
						flag2 = true;
						deviceInfo.Event();
					}
				}
				if (this.AddStickIDs.Contains(id) && overlaySVGElementName.CompareTo("3Stick") == 0)
				{
					deviceInfo.newPos.X = (float)gamepadState.AdditionalStickX / 32767f;
					deviceInfo.newPos.Y = (float)gamepadState.AdditionalStickY / 32767f;
					flag3 = true;
					deviceInfo.Event();
				}
			}
			if (!flag && !flag2 && !flag3 && deviceInfo.newPos.X != 0f && deviceInfo.newPos.Y != 0f)
			{
				deviceInfo.newPos.X = 0f;
				deviceInfo.newPos.Y = 0f;
			}
			this.devicesInfo[id] = deviceInfo;
			this.FindActiveDevice();
			return this.SetActiveSector();
		}

		private double CalcAngle(double dx, double dy)
		{
			double num;
			if (dy >= 0.0)
			{
				num = Math.Atan(dx / dy);
				if (num < 0.0)
				{
					num += 6.283185307179586;
				}
			}
			else
			{
				num = Math.Atan(dx / dy);
				num += 3.141592653589793;
			}
			if (dx > 0.9999999900000001 && dy < 9.99999993922529E-09 && dy >= 0.0)
			{
				num = 1.5707963267948966;
			}
			return num;
		}

		public bool OverlayMouseMove(MouseState mouseState, ulong id)
		{
			if (this.MouseIDs.Contains(id))
			{
				if (!this.devicesInfo.ContainsKey(id))
				{
					this.devicesInfo[id] = new OverlayMenuVME.DeviceInfo();
				}
				OverlayMenuVME.DeviceInfo deviceInfo = this.devicesInfo[id];
				deviceInfo.oldPos = this.devicesInfo[id].newPos;
				deviceInfo.DeadZone = 0.001;
				float num = (float)mouseState.MouseXDelta - this.MouseXDeltaInit;
				if (Math.Abs(num) > 200f)
				{
					this.MouseXDeltaInit += (Math.Abs(num) - 200f) * (float)Math.Sign(num);
				}
				float num2 = this.MouseYDeltaInit - (float)mouseState.MouseYDelta;
				if (Math.Abs(num2) > 200f)
				{
					this.MouseYDeltaInit -= (Math.Abs(num2) - 200f) * (float)Math.Sign(num2);
				}
				deviceInfo.newPos.X = num / 100f;
				if (deviceInfo.newPos.X > 1.5f)
				{
					deviceInfo.newPos.X = 1.5f;
				}
				if (deviceInfo.newPos.X < -1.5f)
				{
					deviceInfo.newPos.X = -1.5f;
				}
				deviceInfo.newPos.Y = num2 / 100f;
				if (deviceInfo.newPos.Y > 1.5f)
				{
					deviceInfo.newPos.Y = 1.5f;
				}
				if (deviceInfo.newPos.Y < -1.5f)
				{
					deviceInfo.newPos.Y = -1.5f;
				}
				deviceInfo.Event();
				this.devicesInfo[id] = deviceInfo;
				if (this.FindActiveDevice())
				{
					this.SetMouseInitPosition(mouseState.MouseXDelta, mouseState.MouseYDelta);
				}
			}
			return this.SetActiveSector();
		}

		public void HideSubmenu()
		{
			if (this.Circle.IsSubmenuVisible)
			{
				this.Circle.HideSubmenu();
				this.Circle.SetActive(-1, false);
			}
		}

		public void ExecuteOverlayMenuCommand(RewasdOverlayMenuServiceCommand rewasdOverlayMenuServiceCommand)
		{
			switch (rewasdOverlayMenuServiceCommand)
			{
			case 2:
				this.Circle.PrevSectorCommand();
				break;
			case 3:
				this.Circle.NextSectorCommand();
				break;
			case 4:
				this.Circle.ShowSubmenu();
				break;
			case 5:
				if (this.Circle.IsSubmenuVisible)
				{
					this.Circle.HideSubmenu();
				}
				else
				{
					this.Circle.SetActive(-1, false);
				}
				break;
			case 6:
				this.ArrowCommand(rewasdOverlayMenuServiceCommand);
				return;
			case 7:
				this.ArrowCommand(rewasdOverlayMenuServiceCommand);
				return;
			case 8:
				this.ArrowCommand(rewasdOverlayMenuServiceCommand);
				return;
			case 9:
				this.ArrowCommand(rewasdOverlayMenuServiceCommand);
				return;
			}
			this.ClearActiveDevice();
		}

		private RewasdOverlayMenuServiceCommand GetAlphaCommand(RewasdOverlayMenuServiceCommand command)
		{
			RewasdOverlayMenuServiceCommand rewasdOverlayMenuServiceCommand = 0;
			double alphaActiveSector = this.Circle.GetAlphaActiveSector();
			switch (command)
			{
			case 6:
				if (alphaActiveSector >= 180.0 && alphaActiveSector < 360.0)
				{
					rewasdOverlayMenuServiceCommand = 3;
				}
				else
				{
					rewasdOverlayMenuServiceCommand = 2;
				}
				break;
			case 7:
				if (alphaActiveSector >= 180.0 && alphaActiveSector < 360.0)
				{
					rewasdOverlayMenuServiceCommand = 2;
				}
				else
				{
					rewasdOverlayMenuServiceCommand = 3;
				}
				break;
			case 8:
				if (alphaActiveSector >= 90.0 && alphaActiveSector < 270.0)
				{
					rewasdOverlayMenuServiceCommand = 2;
				}
				else
				{
					rewasdOverlayMenuServiceCommand = 3;
				}
				break;
			case 9:
				if (alphaActiveSector >= 90.0 && alphaActiveSector < 270.0)
				{
					rewasdOverlayMenuServiceCommand = 3;
				}
				else
				{
					rewasdOverlayMenuServiceCommand = 2;
				}
				break;
			}
			return rewasdOverlayMenuServiceCommand;
		}

		private void ArrowCommand(RewasdOverlayMenuServiceCommand command)
		{
			if (this.Circle.CurrentSectorIndex == -1)
			{
				if (this.Circle.SetActiveCommand(command))
				{
					this.ClearActiveDevice();
					return;
				}
			}
			else
			{
				RewasdOverlayMenuServiceCommand alphaCommand;
				if ((DateTime.Now - this.prevTime).TotalMilliseconds > 1000.0 || command != this.prevCommnad)
				{
					alphaCommand = this.GetAlphaCommand(command);
				}
				else
				{
					alphaCommand = this.prevRemapedCommnad;
				}
				this.ExecuteOverlayMenuCommand(alphaCommand);
				this.prevTime = DateTime.Now;
				this.prevCommnad = command;
				this.prevRemapedCommnad = alphaCommand;
			}
		}

		private bool FindActiveDevice()
		{
			int num = 0;
			bool flag = false;
			KeyValuePair<ulong, OverlayMenuVME.DeviceInfo> keyValuePair = new KeyValuePair<ulong, OverlayMenuVME.DeviceInfo>(0UL, null);
			foreach (KeyValuePair<ulong, OverlayMenuVME.DeviceInfo> keyValuePair2 in this.devicesInfo)
			{
				int lastCount = keyValuePair2.Value.GetLastCount();
				if (lastCount > num)
				{
					num = lastCount;
					keyValuePair = keyValuePair2;
					flag = keyValuePair2.Value.isActive;
				}
				keyValuePair2.Value.isActive = false;
			}
			if (keyValuePair.Value != null)
			{
				keyValuePair.Value.isActive = true;
			}
			return !flag;
		}

		private void ClearActiveDevice()
		{
			foreach (KeyValuePair<ulong, OverlayMenuVME.DeviceInfo> keyValuePair in this.devicesInfo)
			{
				keyValuePair.Value.isActive = false;
			}
		}

		private List<ulong> TouchPad1ClickRequiredIDs;

		private List<ulong> TouchPad2ClickRequiredIDs;

		private List<ulong> TouchPad1IDs;

		private List<ulong> TouchPad2IDs;

		private List<ulong> MouseIDs;

		private List<ulong> StickLeftIDs;

		private List<ulong> StickRighrIDs;

		private List<ulong> AddStickIDs;

		public bool toggle;

		public bool IsLoaded;

		private SilentApply silentApply;

		private OverlayMenuCircleE _currentMenuCircle;

		private const double C_MIN_OFFSET_FOR_ACTIVATE = 0.1;

		private float MouseXDeltaInit;

		private float MouseYDeltaInit;

		private IDictionary<ulong, OverlayMenuVME.DeviceInfo> devicesInfo = new Dictionary<ulong, OverlayMenuVME.DeviceInfo>();

		private string _name;

		private float _transparent;

		private DateTime prevTime;

		private RewasdOverlayMenuServiceCommand prevCommnad;

		private RewasdOverlayMenuServiceCommand prevRemapedCommnad;

		private OverlayMenuE overlayMenuE;

		private class DeviceInfo
		{
			public DeviceInfo()
			{
				this.isActive = false;
				this.events = new List<DateTime>();
			}

			public void Event()
			{
				this.events.Add(DateTime.Now);
			}

			public int GetLastCount()
			{
				return this.events.Count((DateTime d) => d.Ticks > DateTime.Now.Ticks - 10000000L);
			}

			public Vector2 oldPos;

			public Vector2 newPos;

			public bool isActive;

			public double DeadZone;

			private List<DateTime> events;
		}
	}
}
