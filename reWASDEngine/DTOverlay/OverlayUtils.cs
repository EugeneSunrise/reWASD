using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using reWASDCommon.Infrastructure.Enums;
using reWASDEngine;
using reWASDEngine.Services.OverlayAPI;
using UtilsDisplayName;
using XBEliteWPF.DataModels.GamepadSlotHotkeyCollection;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.KeyBindingsModel;

namespace DTOverlay
{
	public static class OverlayUtils
	{
		public static Rectangle GetDesktopWorkingArea(string monitor)
		{
			Rectangle rectangle = Screen.PrimaryScreen.WorkingArea;
			if (!monitor.Equals(""))
			{
				foreach (Screen screen in Screen.AllScreens)
				{
					if (monitor.Equals(ScreenInterrogatory.DeviceFriendlyName(screen)))
					{
						rectangle = screen.WorkingArea;
						break;
					}
				}
			}
			else
			{
				foreach (Screen screen2 in Screen.AllScreens)
				{
					if (screen2.Primary)
					{
						rectangle = screen2.WorkingArea;
						break;
					}
				}
			}
			return rectangle;
		}

		public static void SetAlign(string monitor, AlignType align, float offset, Window wnd)
		{
			Rectangle desktopWorkingArea = OverlayUtils.GetDesktopWorkingArea(monitor);
			int num = (int)typeof(SystemParameters).GetProperty("DpiX", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null);
			double num2 = 96.0 / (double)num;
			double num3 = wnd.ActualWidth / num2;
			double num4 = wnd.ActualHeight / num2;
			int height = desktopWorkingArea.Height;
			if ((double)offset + num4 > (double)height)
			{
				offset = (float)((double)height - num4);
			}
			if (offset < 0f)
			{
				offset = 0f;
			}
			if (align == 2)
			{
				wnd.Left = (double)(desktopWorkingArea.X + 20);
				wnd.Top = (double)(desktopWorkingArea.Y + desktopWorkingArea.Size.Height) - num4 - 20.0 - (double)offset;
			}
			else if (align == 1)
			{
				wnd.Left = (double)(desktopWorkingArea.X + desktopWorkingArea.Size.Width) - num3 - 20.0;
				wnd.Top = (double)((float)(desktopWorkingArea.Y + 20) + offset);
			}
			else if (align == null)
			{
				wnd.Left = (double)(desktopWorkingArea.X + 20);
				wnd.Top = (double)((float)(desktopWorkingArea.Y + 20) + offset);
			}
			else if (align == 3)
			{
				wnd.Left = (double)(desktopWorkingArea.X + desktopWorkingArea.Size.Width) - num3 - 20.0;
				wnd.Top = (double)(desktopWorkingArea.Y + desktopWorkingArea.Size.Height) - num4 - 20.0 - (double)offset;
			}
			else if (align == 4)
			{
				wnd.Left = (double)(desktopWorkingArea.X + desktopWorkingArea.Size.Width / 2) - num3 / 2.0;
				wnd.Top = (double)(desktopWorkingArea.Y + desktopWorkingArea.Size.Height / 2) - num4 / 2.0;
			}
			wnd.Left *= num2;
			wnd.Top *= num2;
		}

		public static System.Windows.HorizontalAlignment ConvertToWindowsAligment(AlignType align)
		{
			System.Windows.HorizontalAlignment horizontalAlignment = System.Windows.HorizontalAlignment.Right;
			if (align == null || align == 2)
			{
				horizontalAlignment = System.Windows.HorizontalAlignment.Left;
			}
			return horizontalAlignment;
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hwnd, int index);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

		public static void SetExtStyle(Window wnd)
		{
			if (OverlayManager.MESSAGES_DEBUG_MODE)
			{
				return;
			}
			WindowInteropHelper windowInteropHelper = new WindowInteropHelper(wnd);
			OverlayUtils.GetWindowLong(windowInteropHelper.Handle, -20);
			OverlayUtils.SetWindowLong(windowInteropHelper.Handle, -20, 134742184);
		}

		public static List<GroupFromSettings> GetGamepadBinding(string ID, OverlayUtils.HotkeysType hotkeysType)
		{
			List<GroupFromSettings> list = new List<GroupFromSettings>();
			HotkeyCollection hotkeyCollection;
			if (!string.IsNullOrEmpty(ID) && Engine.GamepadService.GamepadsHotkeyCollection.TryGetValue(ID, out hotkeyCollection))
			{
				if (hotkeyCollection.ControllerFamily == 3)
				{
					using (IEnumerator<KeyValuePair<string, HotkeyCollection>> enumerator = (from item in Engine.GamepadService.GamepadsHotkeyCollection.CloneOverlays()
						where item.Value.ControllerFamily != 3
						select item).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, HotkeyCollection> keyValuePair = enumerator.Current;
							if (ID.Contains(keyValuePair.Key))
							{
								if (hotkeysType == OverlayUtils.HotkeysType.Gamepad)
								{
									list.Add(new GroupFromSettings
									{
										groupButtons = new List<AssociatedControllerButton>(keyValuePair.Value.GamepadOverlayAssociatedButtonCollection),
										CurrentGamepadType = new ControllerTypeEnum?(keyValuePair.Value.FirstGamepadType)
									});
								}
								else if (hotkeysType == OverlayUtils.HotkeysType.Mappings)
								{
									list.Add(new GroupFromSettings
									{
										groupButtons = new List<AssociatedControllerButton>(keyValuePair.Value.MappingOverlayAssociatedButtonCollection),
										CurrentGamepadType = new ControllerTypeEnum?(keyValuePair.Value.FirstGamepadType)
									});
								}
								else
								{
									list.Add(new GroupFromSettings
									{
										groupButtons = new List<AssociatedControllerButton>(keyValuePair.Value.MappingDescriptionsOverlayAssociatedButtonCollection),
										CurrentGamepadType = new ControllerTypeEnum?(keyValuePair.Value.FirstGamepadType)
									});
								}
							}
						}
						return list;
					}
				}
				if (hotkeysType == OverlayUtils.HotkeysType.Gamepad)
				{
					list.Add(new GroupFromSettings
					{
						groupButtons = new List<AssociatedControllerButton>(hotkeyCollection.GamepadOverlayAssociatedButtonCollection),
						CurrentGamepadType = new ControllerTypeEnum?(hotkeyCollection.FirstGamepadType)
					});
				}
				else if (hotkeysType == OverlayUtils.HotkeysType.Mappings)
				{
					list.Add(new GroupFromSettings
					{
						groupButtons = new List<AssociatedControllerButton>(hotkeyCollection.MappingOverlayAssociatedButtonCollection),
						CurrentGamepadType = new ControllerTypeEnum?(hotkeyCollection.FirstGamepadType)
					});
				}
				else
				{
					list.Add(new GroupFromSettings
					{
						groupButtons = new List<AssociatedControllerButton>(hotkeyCollection.MappingDescriptionsOverlayAssociatedButtonCollection),
						CurrentGamepadType = new ControllerTypeEnum?(hotkeyCollection.FirstGamepadType)
					});
				}
			}
			return list;
		}

		public static bool IsContainsInLongID(string longID, string partID)
		{
			bool flag = true;
			foreach (string text in partID.Split(';', StringSplitOptions.None))
			{
				if (!longID.Contains(text))
				{
					flag = false;
					break;
				}
			}
			return flag;
		}

		public static HashInfo CalcGamepadHash(GamepadState gamepadState, HashInfo old)
		{
			double num = 20000000.0;
			double num2 = 1000000.0;
			HashInfo hashInfo = new HashInfo();
			long num3 = 17L;
			num3 = num3 * 19L + (long)gamepadState.PressedButtons.Count;
			foreach (GamepadButtonDescription gamepadButtonDescription in gamepadState.PressedButtons)
			{
				num3 = num3 * 19L + gamepadButtonDescription.Button;
			}
			num3 = num3 * 19L + (long)gamepadState.PressedButtons.Count;
			num3 = num3 * 19L + (long)((ulong)gamepadState.LeftTrigger);
			num3 = num3 * 19L + (long)((ulong)gamepadState.RightTrigger);
			num3 = num3 * 19L + (long)gamepadState.LeftStickX;
			num3 = num3 * 19L + (long)gamepadState.LeftStickY;
			num3 = num3 * 19L + (long)gamepadState.RightStickX;
			num3 = num3 * 19L + (long)gamepadState.RightStickY;
			num3 = num3 * 19L + (long)gamepadState.AdditionalStickX;
			num3 = num3 * 19L + (long)gamepadState.AdditionalStickY;
			num3 = num3 * 19L + (long)((ulong)gamepadState.LeftFingerX);
			num3 = num3 * 19L + (long)((ulong)gamepadState.LeftFingerY);
			num3 = num3 * 19L + (long)((ulong)gamepadState.RightFingerX);
			num3 = num3 * 19L + (long)((ulong)gamepadState.RightFingerY);
			num3 = num3 * 19L + (long)((ulong)gamepadState.DS3PressureCross);
			num3 = num3 * 19L + (long)((ulong)gamepadState.DS3PressureCircle);
			num3 = num3 * 19L + (long)((ulong)gamepadState.DS3PressureSquare);
			num3 = num3 * 19L + (long)((ulong)gamepadState.DS3PressureTriangle);
			num3 = num3 * 19L + (long)((ulong)gamepadState.DS3PressureL1);
			num3 = num3 * 19L + (long)((ulong)gamepadState.DS3PressureR1);
			num3 = num3 * 19L + (long)((ulong)gamepadState.DS3PressureDpadUp);
			num3 = num3 * 19L + (long)((ulong)gamepadState.DS3PressureDpadDown);
			num3 = num3 * 19L + (long)((ulong)gamepadState.DS3PressureDpadRight);
			num3 = num3 * 19L + (long)((ulong)gamepadState.DS3PressureDpadLeft);
			if (old != null)
			{
				if (old.GyroXDelta == 0L)
				{
					old.GyroXDelta = gamepadState.GyroXDelta;
				}
				if (old.GyroYDelta == 0L)
				{
					old.GyroYDelta = gamepadState.GyroYDelta;
				}
				if (old.GyroZDelta == 0L)
				{
					old.GyroZDelta = gamepadState.GyroZDelta;
				}
				long num4 = Math.Abs(gamepadState.AccelXDelta);
				if ((double)Math.Abs(num4 - old.AccelXDelta) > num)
				{
					num3 = num3 * 19L + num4;
				}
				long num5 = Math.Abs(gamepadState.AccelYDelta);
				if ((double)Math.Abs(num5 - old.AccelYDelta) > num)
				{
					num3 = num3 * 19L + num5;
				}
				long num6 = Math.Abs(gamepadState.AccelZDelta);
				if ((double)Math.Abs(num6 - old.AccelZDelta) > num)
				{
					num3 = num3 * 19L + num6;
				}
				hashInfo.AccelXDelta = num4;
				hashInfo.AccelYDelta = num5;
				hashInfo.AccelZDelta = num6;
				long num7 = Math.Abs(gamepadState.GyroXDelta);
				if ((double)Math.Abs(Math.Abs(num7) - Math.Abs(old.GyroXDelta)) > num2)
				{
					num3 = num3 * 19L + num7;
					hashInfo.GyroXDelta = gamepadState.GyroXDelta;
				}
				else
				{
					hashInfo.GyroXDelta = old.GyroXDelta;
					gamepadState.GyroXDelta = old.GyroXDelta;
				}
				long num8 = Math.Abs(gamepadState.GyroYDelta);
				if ((double)Math.Abs(Math.Abs(num8) - Math.Abs(old.GyroYDelta)) > num2)
				{
					num3 = num3 * 19L + num8;
					hashInfo.GyroYDelta = gamepadState.GyroYDelta;
				}
				else
				{
					hashInfo.GyroYDelta = old.GyroYDelta;
					gamepadState.GyroYDelta = old.GyroYDelta;
				}
				long num9 = Math.Abs(gamepadState.GyroZDelta);
				if ((double)Math.Abs(Math.Abs(num9) - Math.Abs(old.GyroZDelta)) > num2)
				{
					num3 = num3 * 19L + num9;
					hashInfo.GyroZDelta = gamepadState.GyroZDelta;
				}
				else
				{
					hashInfo.GyroZDelta = old.GyroZDelta;
					gamepadState.GyroZDelta = old.GyroZDelta;
				}
				hashInfo.AccelXDelta = num4;
				hashInfo.AccelYDelta = num5;
				hashInfo.AccelZDelta = num6;
			}
			hashInfo.Hash = num3;
			return hashInfo;
		}

		public static long CalcMousedHash(MouseState mouseState, long old)
		{
			return (17L * 19L + mouseState.MouseXDelta) * 19L + mouseState.MouseYDelta;
		}

		public static void ClearOldPng(string prefix, int fileID)
		{
			foreach (FileInfo fileInfo in new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Disc-Soft", "reWASD") + "\\OverlayData\\").GetFiles(prefix + "*"))
			{
				int num = 0;
				if (int.TryParse(Regex.Match(fileInfo.Name, "\\d+").Value, out num) && (num < fileID - 5 || (fileID > 5 && num > fileID)))
				{
					fileInfo.Delete();
				}
			}
		}

		private const int SWP_NOSIZE = 1;

		private const int SWP_NOZORDER = 4;

		private const int SWP_SHOWWINDOW = 64;

		private const int WS_EX_NOACTIVATE = 134217728;

		public enum HotkeysType
		{
			Gamepad,
			Mappings,
			MappingsDescriptions
		}
	}
}
