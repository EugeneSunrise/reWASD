using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoftReWASDServiceNamespace;
using reWASDCommon.Infrastructure;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.Mask;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Views;
using reWASDUI.Views.ContentZoneGamepad;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.Wrappers;

namespace reWASDUI.Utils.XBUtil
{
	public static class XBUtils
	{
		public static TextBlock GetDescriptionForActivator(ActivatorType activatorType, bool isShift = false)
		{
			TextBlock textBlock = new TextBlock
			{
				TextWrapping = TextWrapping.Wrap,
				HorizontalAlignment = HorizontalAlignment.Stretch
			};
			if (isShift)
			{
				textBlock.Inlines.Add(new Run(DTLocalization.GetString(11552)));
			}
			else
			{
				switch (activatorType)
				{
				case 0:
					NavigationServiceHTML.SetTextToTextBlock(textBlock, DTLocalization.GetString(11553), true);
					break;
				case 1:
					NavigationServiceHTML.SetTextToTextBlock(textBlock, DTLocalization.GetString(11554), true);
					break;
				case 2:
					NavigationServiceHTML.SetTextToTextBlock(textBlock, DTLocalization.GetString(11555), true);
					break;
				case 3:
					NavigationServiceHTML.SetTextToTextBlock(textBlock, DTLocalization.GetString(11556), true);
					break;
				case 4:
					NavigationServiceHTML.SetTextToTextBlock(textBlock, DTLocalization.GetString(11557), true);
					break;
				case 5:
					NavigationServiceHTML.SetTextToTextBlock(textBlock, DTLocalization.GetString(11558), true);
					break;
				default:
					throw new ArgumentOutOfRangeException("ActivatorType", activatorType, null);
				}
			}
			return textBlock;
		}

		public static string ConvertGamepadButtonToAnchorString(GamepadButton gb)
		{
			switch (gb)
			{
			case 1:
				return "Btn1";
			case 2:
				return "Btn2";
			case 3:
				return "Btn3";
			case 4:
				return "Btn4";
			case 5:
				return "Btn5";
			case 6:
				return "Btn6";
			case 7:
				return "Btn7";
			case 8:
				return "Btn8";
			case 9:
				return "Btn9";
			case 10:
				return "Btn10";
			case 11:
				return "Btn11";
			case 12:
				return "Btn12";
			case 13:
				return "Btn13";
			case 14:
				return "Btn14";
			case 15:
				return "Btn15";
			case 16:
				return "Btn16";
			case 17:
				return "Btn17";
			case 18:
				return "Btn18";
			case 19:
				return "Btn19";
			case 20:
				return "Btn20";
			case 21:
				return "Btn21";
			case 22:
				return "Btn22";
			case 23:
				return "Btn23";
			case 24:
				return "Btn24";
			case 25:
				return "Btn25";
			case 26:
				return "Btn26";
			case 27:
				return "Btn27";
			case 29:
				return "LeftPaddleTop";
			case 30:
				return "RightPaddleTop";
			case 31:
				return "LeftPaddleBottom";
			case 32:
				return "RightPaddleBottom";
			case 33:
				return "DpadUp";
			case 34:
				return "DpadDown";
			case 35:
				return "DpadLeft";
			case 36:
				return "DpadRight";
			case 37:
			case 38:
			case 39:
				return "BindingFrame";
			case 40:
				return "LstickUp";
			case 41:
				return "LstickDown";
			case 42:
				return "LstickLeft";
			case 43:
				return "LstickRight";
			case 44:
			case 45:
			case 46:
				return "BindingFrame";
			case 47:
				return "RstickUp";
			case 48:
				return "RstickDown";
			case 49:
				return "RstickLeft";
			case 50:
				return "RstickRight";
			case 51:
				return "LT";
			case 52:
			case 53:
			case 54:
				return "BindingFrame";
			case 55:
				return "RT";
			case 56:
			case 57:
			case 58:
				return "BindingFrame";
			case 64:
				return "UpMouse";
			case 65:
				return "DownMouse";
			case 66:
				return "LeftMouse";
			case 67:
				return "RightMouse";
			case 68:
				return "GyroTiltUp";
			case 69:
				return "GyroTiltDown";
			case 70:
				return "GyroTiltLeft";
			case 71:
				return "GyroTiltRight";
			case 77:
				return "LeftTap";
			case 78:
				return "RightTap";
			case 79:
				return "CenterTap";
			case 80:
				return "DoubleTap";
			case 81:
				return "Zoom";
			case 83:
				return "Swipe";
			case 91:
				return "BtnTouchpad1Up";
			case 92:
				return "BtnTouchpad1Down";
			case 93:
				return "BtnTouchpad1Left";
			case 94:
				return "BtnTouchpad1Right";
			case 99:
				return "BtnTouchpad1Click";
			case 100:
				return "BtnTouchpad2Up";
			case 101:
				return "BtnTouchpad2Down";
			case 102:
				return "BtnTouchpad2Left";
			case 103:
				return "BtnTouchpad2Right";
			case 108:
				return "BtnTouchpad2Click";
			case 163:
				return "BtnTouchpad1Tap";
			case 164:
				return "BtnTouchpad2Tap";
			case 169:
				return "LTFullPull";
			case 170:
				return "RTFullPull";
			case 171:
				return "ScrollUp";
			case 172:
				return "ScrollDown";
			case 173:
				return "ScrollLeft";
			case 174:
				return "ScrollRight";
			case 183:
			case 184:
			case 185:
				return "BindingFrame";
			case 186:
			case 187:
			case 188:
			case 189:
			case 190:
			case 191:
			case 192:
			case 193:
			case 194:
			case 195:
			case 196:
			case 197:
			case 198:
			case 199:
			case 200:
			case 201:
			case 202:
			case 203:
			case 204:
			case 205:
			case 206:
			case 207:
			case 208:
			case 209:
			case 210:
			case 211:
			case 212:
				return "BindingFrame";
			case 216:
				return "AstickUp";
			case 217:
				return "AstickDown";
			case 218:
				return "AstickLeft";
			case 219:
				return "AstickRight";
			case 236:
			case 237:
			case 238:
			case 239:
			case 240:
			case 241:
				return "BindingFrame";
			}
			return gb.ToString();
		}

		public static string ConvertMouseButtonToAnchorString(MouseButton mb)
		{
			string text = null;
			switch (mb)
			{
			case MouseButton.Left:
				text = "Left";
				break;
			case MouseButton.Middle:
				text = "Middle";
				break;
			case MouseButton.Right:
				text = "Right";
				break;
			case MouseButton.XButton1:
				text = "XButton1";
				break;
			case MouseButton.XButton2:
				text = "XButton2";
				break;
			}
			return text;
		}

		public static string ConvertMediaButtonsToString(KeyScanCodeV2 ksc)
		{
			string text = null;
			string description = ksc.Description;
			if (description != null)
			{
				switch (description.Length)
				{
				case 11:
				{
					char c = description[7];
					if (c != 'B')
					{
						if (c == 'H')
						{
							if (description == "DIK_WEBHOME")
							{
								text = "HomeFakeButton";
							}
						}
					}
					else if (description == "DIK_WEBBACK")
					{
						text = "BackFakeButton";
					}
					break;
				}
				case 12:
					if (description == "DIK_VOLUMEUP")
					{
						text = "AddVolFakeButton";
					}
					break;
				case 13:
				{
					char c = description[5];
					if (c != 'E')
					{
						if (c != 'L')
						{
							if (c == 'R')
							{
								if (description == "DIK_PREVTRACK")
								{
									text = "PrevFakeButton";
								}
							}
						}
						else if (description == "DIK_PLAYPAUSE")
						{
							text = "PlayPauseFakeButton";
						}
					}
					else if (description == "DIK_NEXTTRACK")
					{
						text = "NextFakeButton";
					}
					break;
				}
				case 14:
					if (description == "DIK_VOLUMEDOWN")
					{
						text = "DownVolFakeButton";
					}
					break;
				}
			}
			return text;
		}

		private static string ConvertControllerBindingFrameAdditionalModesToAnchorString(ControllerBindingFrameAdditionalModes controllerBindingFrameMode)
		{
			string text = null;
			if (controllerBindingFrameMode != null)
			{
				if (controllerBindingFrameMode == 1)
				{
					text = "MouseDirectionFrame";
				}
			}
			else
			{
				text = "WizardButton";
			}
			return text;
		}

		public static string ConvertControllerButtonToAnchorString(AssociatedControllerButton cb)
		{
			if (cb.ControllerBindingFrameMode != null)
			{
				return XBUtils.ConvertControllerBindingFrameAdditionalModesToAnchorString(cb.ControllerBindingFrameMode.Value);
			}
			if (cb.IsGamepad)
			{
				return XBUtils.ConvertGamepadButtonToAnchorString(cb.GamepadButton);
			}
			if (cb.IsKeyScanCode && cb.KeyScanCode.IsMouse)
			{
				return XBUtils.ConvertMouseButtonToAnchorString(KeyScanCodeV2.FindMouseButtonByKeyScanCode(cb.KeyScanCode));
			}
			if (cb.IsKeyScanCode)
			{
				return XBUtils.ConvertMediaButtonsToString(cb.KeyScanCode);
			}
			return null;
		}

		public static MouseButton ConvertAnchorStringToMouseButton(string anchorName)
		{
			MouseButton mouseButton = MouseButton.XButton1;
			if (!(anchorName == "Left"))
			{
				if (!(anchorName == "Middle"))
				{
					if (!(anchorName == "Right"))
					{
						if (!(anchorName == "XButton1"))
						{
							if (anchorName == "XButton2")
							{
								mouseButton = MouseButton.XButton2;
							}
						}
						else
						{
							mouseButton = MouseButton.XButton1;
						}
					}
					else
					{
						mouseButton = MouseButton.Right;
					}
				}
				else
				{
					mouseButton = MouseButton.Middle;
				}
			}
			else
			{
				mouseButton = MouseButton.Left;
			}
			return mouseButton;
		}

		public static AssociatedControllerButton ConvertAnchorStringToGamepadButton(string anchorName)
		{
			GamepadButton gamepadButton = 2001;
			KeyScanCodeV2 keyScanCodeV = KeyScanCodeV2.NoMap;
			ControllerBindingFrameAdditionalModes? controllerBindingFrameAdditionalModes = null;
			if (anchorName != null)
			{
				switch (anchorName.Length)
				{
				case 2:
				{
					char c = anchorName[0];
					if (c != 'L')
					{
						if (c != 'R')
						{
							goto IL_1016;
						}
						if (!(anchorName == "RT"))
						{
							goto IL_1016;
						}
						gamepadButton = 55;
						goto IL_1022;
					}
					else
					{
						if (!(anchorName == "LT"))
						{
							goto IL_1016;
						}
						gamepadButton = 51;
						goto IL_1022;
					}
					break;
				}
				case 3:
				case 18:
					goto IL_1016;
				case 4:
				{
					char c = anchorName[3];
					switch (c)
					{
					case '1':
						if (!(anchorName == "Btn1"))
						{
							goto IL_1016;
						}
						gamepadButton = 1;
						goto IL_1022;
					case '2':
						if (!(anchorName == "Btn2"))
						{
							goto IL_1016;
						}
						gamepadButton = 2;
						goto IL_1022;
					case '3':
						if (!(anchorName == "Btn3"))
						{
							goto IL_1016;
						}
						gamepadButton = 3;
						goto IL_1022;
					case '4':
						if (!(anchorName == "Btn4"))
						{
							goto IL_1016;
						}
						gamepadButton = 4;
						goto IL_1022;
					case '5':
						if (!(anchorName == "Btn5"))
						{
							goto IL_1016;
						}
						gamepadButton = 5;
						goto IL_1022;
					case '6':
						if (!(anchorName == "Btn6"))
						{
							goto IL_1016;
						}
						gamepadButton = 6;
						goto IL_1022;
					case '7':
						if (!(anchorName == "Btn7"))
						{
							goto IL_1016;
						}
						gamepadButton = 7;
						goto IL_1022;
					case '8':
						if (!(anchorName == "Btn8"))
						{
							goto IL_1016;
						}
						gamepadButton = 8;
						goto IL_1022;
					case '9':
						if (!(anchorName == "Btn9"))
						{
							goto IL_1016;
						}
						break;
					default:
						if (c != 'm')
						{
							if (c != 't')
							{
								goto IL_1016;
							}
							if (!(anchorName == "Left"))
							{
								goto IL_1016;
							}
							keyScanCodeV = KeyScanCodeV2.FindKeyScanCodeByMouseButton(MouseButton.Left);
							goto IL_1022;
						}
						else
						{
							if (!(anchorName == "Zoom"))
							{
								goto IL_1016;
							}
							gamepadButton = 81;
							goto IL_1022;
						}
						break;
					}
					break;
				}
				case 5:
				{
					char c = anchorName[4];
					switch (c)
					{
					case '0':
						if (anchorName == "Btn10")
						{
							goto IL_CE7;
						}
						if (!(anchorName == "Btn20"))
						{
							goto IL_1016;
						}
						gamepadButton = 20;
						goto IL_1022;
					case '1':
						if (anchorName == "Btn11")
						{
							gamepadButton = 11;
							goto IL_1022;
						}
						if (!(anchorName == "Btn21"))
						{
							goto IL_1016;
						}
						gamepadButton = 21;
						goto IL_1022;
					case '2':
						if (anchorName == "Btn12")
						{
							goto IL_CF7;
						}
						if (!(anchorName == "Btn22"))
						{
							goto IL_1016;
						}
						gamepadButton = 22;
						goto IL_1022;
					case '3':
						if (anchorName == "Btn13")
						{
							goto IL_CFF;
						}
						if (!(anchorName == "Btn23"))
						{
							goto IL_1016;
						}
						gamepadButton = 23;
						goto IL_1022;
					case '4':
						if (anchorName == "Btn14")
						{
							gamepadButton = 14;
							goto IL_1022;
						}
						if (!(anchorName == "Btn24"))
						{
							goto IL_1016;
						}
						gamepadButton = 24;
						goto IL_1022;
					case '5':
						if (anchorName == "Btn15")
						{
							gamepadButton = 15;
							goto IL_1022;
						}
						if (!(anchorName == "Btn25"))
						{
							goto IL_1016;
						}
						gamepadButton = 25;
						goto IL_1022;
					case '6':
						if (anchorName == "Btn16")
						{
							gamepadButton = 16;
							goto IL_1022;
						}
						if (!(anchorName == "Btn26"))
						{
							goto IL_1016;
						}
						gamepadButton = 26;
						goto IL_1022;
					case '7':
						if (anchorName == "Btn17")
						{
							gamepadButton = 17;
							goto IL_1022;
						}
						if (!(anchorName == "Btn27"))
						{
							goto IL_1016;
						}
						gamepadButton = 27;
						goto IL_1022;
					case '8':
						if (!(anchorName == "Btn18"))
						{
							goto IL_1016;
						}
						gamepadButton = 18;
						goto IL_1022;
					case '9':
						if (!(anchorName == "Btn19"))
						{
							goto IL_1016;
						}
						gamepadButton = 19;
						goto IL_1022;
					default:
						if (c != 'e')
						{
							if (c != 't')
							{
								goto IL_1016;
							}
							if (!(anchorName == "Right"))
							{
								goto IL_1016;
							}
							keyScanCodeV = KeyScanCodeV2.FindKeyScanCodeByMouseButton(MouseButton.Right);
							goto IL_1022;
						}
						else
						{
							if (anchorName == "Share")
							{
								goto IL_CFF;
							}
							if (!(anchorName == "Swipe"))
							{
								goto IL_1016;
							}
							gamepadButton = 83;
							goto IL_1022;
						}
						break;
					}
					break;
				}
				case 6:
				{
					char c = anchorName[0];
					if (c <= 'L')
					{
						if (c != 'D')
						{
							if (c != 'L')
							{
								goto IL_1016;
							}
							if (!(anchorName == "Lstick"))
							{
								goto IL_1016;
							}
						}
						else
						{
							if (!(anchorName == "DpadUp"))
							{
								goto IL_1016;
							}
							gamepadButton = 33;
							goto IL_1022;
						}
					}
					else if (c != 'M')
					{
						if (c != 'R')
						{
							goto IL_1016;
						}
						if (!(anchorName == "Rstick"))
						{
							goto IL_1016;
						}
						goto IL_CE7;
					}
					else
					{
						if (!(anchorName == "Middle"))
						{
							goto IL_1016;
						}
						keyScanCodeV = KeyScanCodeV2.FindKeyScanCodeByMouseButton(MouseButton.Middle);
						goto IL_1022;
					}
					break;
				}
				case 7:
				{
					char c = anchorName[0];
					if (c != 'L')
					{
						if (c != 'U')
						{
							goto IL_1016;
						}
						if (!(anchorName == "UpMouse"))
						{
							goto IL_1016;
						}
						gamepadButton = 64;
						goto IL_1022;
					}
					else
					{
						if (!(anchorName == "LeftTap"))
						{
							goto IL_1016;
						}
						gamepadButton = 77;
						goto IL_1022;
					}
					break;
				}
				case 8:
				{
					char c = anchorName[7];
					if (c <= '2')
					{
						if (c != '1')
						{
							if (c != '2')
							{
								goto IL_1016;
							}
							if (!(anchorName == "XButton2"))
							{
								goto IL_1016;
							}
							keyScanCodeV = KeyScanCodeV2.FindKeyScanCodeByMouseButton(MouseButton.XButton2);
							goto IL_1022;
						}
						else
						{
							if (!(anchorName == "XButton1"))
							{
								goto IL_1016;
							}
							keyScanCodeV = KeyScanCodeV2.FindKeyScanCodeByMouseButton(MouseButton.XButton1);
							goto IL_1022;
						}
					}
					else if (c != 'n')
					{
						if (c != 'p')
						{
							if (c != 't')
							{
								goto IL_1016;
							}
							if (!(anchorName == "DpadLeft"))
							{
								goto IL_1016;
							}
							gamepadButton = 35;
							goto IL_1022;
						}
						else
						{
							if (anchorName == "RstickUp")
							{
								gamepadButton = 47;
								goto IL_1022;
							}
							if (anchorName == "LstickUp")
							{
								gamepadButton = 40;
								goto IL_1022;
							}
							if (anchorName == "AstickUp")
							{
								gamepadButton = 216;
								goto IL_1022;
							}
							if (anchorName == "RightTap")
							{
								gamepadButton = 78;
								goto IL_1022;
							}
							if (!(anchorName == "ScrollUp"))
							{
								goto IL_1016;
							}
							gamepadButton = 171;
							goto IL_1022;
						}
					}
					else
					{
						if (!(anchorName == "DpadDown"))
						{
							goto IL_1016;
						}
						gamepadButton = 34;
						goto IL_1022;
					}
					break;
				}
				case 9:
				{
					char c = anchorName[2];
					if (c <= 'f')
					{
						if (c != 'a')
						{
							if (c != 'f')
							{
								goto IL_1016;
							}
							if (!(anchorName == "LeftMouse"))
							{
								goto IL_1016;
							}
							gamepadButton = 66;
							goto IL_1022;
						}
						else
						{
							if (!(anchorName == "DpadRight"))
							{
								goto IL_1016;
							}
							gamepadButton = 36;
							goto IL_1022;
						}
					}
					else if (c != 'n')
					{
						switch (c)
						{
						case 's':
							if (!(anchorName == "Assistant"))
							{
								goto IL_1016;
							}
							goto IL_CFF;
						case 't':
						case 'v':
							goto IL_1016;
						case 'u':
							if (!(anchorName == "DoubleTap"))
							{
								goto IL_1016;
							}
							gamepadButton = 80;
							goto IL_1022;
						case 'w':
							if (!(anchorName == "DownMouse"))
							{
								goto IL_1016;
							}
							gamepadButton = 65;
							goto IL_1022;
						default:
							goto IL_1016;
						}
					}
					else
					{
						if (!(anchorName == "CenterTap"))
						{
							goto IL_1016;
						}
						gamepadButton = 79;
						goto IL_1022;
					}
					break;
				}
				case 10:
				{
					char c = anchorName[6];
					if (c <= 'L')
					{
						if (c != 'D')
						{
							if (c != 'L')
							{
								goto IL_1016;
							}
							if (anchorName == "RstickLeft")
							{
								gamepadButton = 49;
								goto IL_1022;
							}
							if (anchorName == "LstickLeft")
							{
								gamepadButton = 42;
								goto IL_1022;
							}
							if (anchorName == "AstickLeft")
							{
								gamepadButton = 218;
								goto IL_1022;
							}
							if (!(anchorName == "ScrollLeft"))
							{
								goto IL_1016;
							}
							gamepadButton = 173;
							goto IL_1022;
						}
						else
						{
							if (anchorName == "RstickDown")
							{
								gamepadButton = 48;
								goto IL_1022;
							}
							if (anchorName == "LstickDown")
							{
								gamepadButton = 41;
								goto IL_1022;
							}
							if (anchorName == "AstickDown")
							{
								gamepadButton = 217;
								goto IL_1022;
							}
							if (!(anchorName == "ScrollDown"))
							{
								goto IL_1016;
							}
							gamepadButton = 172;
							goto IL_1022;
						}
					}
					else if (c != 'P')
					{
						if (c != 'l')
						{
							if (c != 'o')
							{
								goto IL_1016;
							}
							if (!(anchorName == "RightMouse"))
							{
								goto IL_1016;
							}
							gamepadButton = 67;
							goto IL_1022;
						}
						else
						{
							if (!(anchorName == "GyroTiltUp"))
							{
								goto IL_1016;
							}
							gamepadButton = 68;
							goto IL_1022;
						}
					}
					else
					{
						if (anchorName == "LTFullPull")
						{
							gamepadButton = 169;
							goto IL_1022;
						}
						if (!(anchorName == "RTFullPull"))
						{
							goto IL_1016;
						}
						gamepadButton = 170;
						goto IL_1022;
					}
					break;
				}
				case 11:
				{
					char c = anchorName[0];
					if (c <= 'L')
					{
						if (c != 'A')
						{
							if (c != 'L')
							{
								goto IL_1016;
							}
							if (!(anchorName == "LstickClick"))
							{
								if (!(anchorName == "LstickRight"))
								{
									goto IL_1016;
								}
								gamepadButton = 43;
								goto IL_1022;
							}
						}
						else
						{
							if (!(anchorName == "AstickRight"))
							{
								goto IL_1016;
							}
							gamepadButton = 219;
							goto IL_1022;
						}
					}
					else if (c != 'R')
					{
						if (c != 'S')
						{
							goto IL_1016;
						}
						if (!(anchorName == "ScrollRight"))
						{
							goto IL_1016;
						}
						gamepadButton = 174;
						goto IL_1022;
					}
					else
					{
						if (anchorName == "RstickClick")
						{
							goto IL_CE7;
						}
						if (!(anchorName == "RstickRight"))
						{
							goto IL_1016;
						}
						gamepadButton = 50;
						goto IL_1022;
					}
					break;
				}
				case 12:
				{
					char c = anchorName[8];
					if (c != 'D')
					{
						if (c != 'L')
						{
							if (c != 't')
							{
								goto IL_1016;
							}
							if (!(anchorName == "WizardButton"))
							{
								goto IL_1016;
							}
							controllerBindingFrameAdditionalModes = new ControllerBindingFrameAdditionalModes?(0);
							goto IL_1022;
						}
						else
						{
							if (!(anchorName == "GyroTiltLeft"))
							{
								goto IL_1016;
							}
							gamepadButton = 70;
							goto IL_1022;
						}
					}
					else
					{
						if (!(anchorName == "GyroTiltDown"))
						{
							goto IL_1016;
						}
						gamepadButton = 69;
						goto IL_1022;
					}
					break;
				}
				case 13:
				{
					char c = anchorName[10];
					if (c <= 'M')
					{
						if (c != 'L')
						{
							if (c != 'M')
							{
								goto IL_1016;
							}
							if (anchorName == "LstickZoneMed")
							{
								gamepadButton = 38;
								goto IL_1022;
							}
							if (!(anchorName == "RstickZoneMed"))
							{
								goto IL_1016;
							}
							gamepadButton = 45;
							goto IL_1022;
						}
						else
						{
							if (anchorName == "LstickZoneLow")
							{
								gamepadButton = 37;
								goto IL_1022;
							}
							if (!(anchorName == "RstickZoneLow"))
							{
								goto IL_1016;
							}
							gamepadButton = 44;
							goto IL_1022;
						}
					}
					else if (c != 'T')
					{
						if (c != 'g')
						{
							if (c != 'i')
							{
								goto IL_1016;
							}
							if (!(anchorName == "TouchpadClick"))
							{
								goto IL_1016;
							}
							goto IL_CF7;
						}
						else
						{
							if (!(anchorName == "GyroTiltRight"))
							{
								goto IL_1016;
							}
							gamepadButton = 71;
							goto IL_1022;
						}
					}
					else
					{
						if (!(anchorName == "LeftPaddleTop"))
						{
							goto IL_1016;
						}
						gamepadButton = 29;
						goto IL_1022;
					}
					break;
				}
				case 14:
				{
					char c = anchorName[11];
					if (c <= '2')
					{
						if (c != '1')
						{
							if (c != '2')
							{
								goto IL_1016;
							}
							if (!(anchorName == "BtnTouchpad2Up"))
							{
								goto IL_1016;
							}
							gamepadButton = 100;
							goto IL_1022;
						}
						else
						{
							if (!(anchorName == "BtnTouchpad1Up"))
							{
								goto IL_1016;
							}
							gamepadButton = 91;
							goto IL_1022;
						}
					}
					else if (c != 'T')
					{
						if (c != 'i')
						{
							goto IL_1016;
						}
						if (anchorName == "LstickZoneHigh")
						{
							gamepadButton = 39;
							goto IL_1022;
						}
						if (!(anchorName == "RstickZoneHigh"))
						{
							goto IL_1016;
						}
						gamepadButton = 46;
						goto IL_1022;
					}
					else
					{
						if (!(anchorName == "RightPaddleTop"))
						{
							goto IL_1016;
						}
						gamepadButton = 30;
						goto IL_1022;
					}
					break;
				}
				case 15:
				{
					char c = anchorName[11];
					if (c != '1')
					{
						if (c != '2')
						{
							if (c != 'e')
							{
								goto IL_1016;
							}
							if (anchorName == "LtriggerZoneLow")
							{
								gamepadButton = 52;
								goto IL_1022;
							}
							if (anchorName == "LtriggerZoneMed")
							{
								gamepadButton = 53;
								goto IL_1022;
							}
							if (anchorName == "RtriggerZoneLow")
							{
								gamepadButton = 56;
								goto IL_1022;
							}
							if (!(anchorName == "RtriggerZoneMed"))
							{
								goto IL_1016;
							}
							gamepadButton = 57;
							goto IL_1022;
						}
						else
						{
							if (!(anchorName == "BtnTouchpad2Tap"))
							{
								goto IL_1016;
							}
							gamepadButton = 164;
							goto IL_1022;
						}
					}
					else
					{
						if (!(anchorName == "BtnTouchpad1Tap"))
						{
							goto IL_1016;
						}
						gamepadButton = 163;
						goto IL_1022;
					}
					break;
				}
				case 16:
				{
					char c = anchorName[13];
					if (c <= 'e')
					{
						if (c != 'L')
						{
							if (c != 'M')
							{
								if (c != 'e')
								{
									goto IL_1016;
								}
								if (anchorName == "BtnTouchpad1Left")
								{
									gamepadButton = 93;
									goto IL_1022;
								}
								if (!(anchorName == "BtnTouchpad2Left"))
								{
									goto IL_1016;
								}
								gamepadButton = 102;
								goto IL_1022;
							}
							else
							{
								if (anchorName == "LTrackpadZoneMed")
								{
									gamepadButton = 237;
									goto IL_1022;
								}
								if (!(anchorName == "RTrackpadZoneMed"))
								{
									goto IL_1016;
								}
								gamepadButton = 240;
								goto IL_1022;
							}
						}
						else
						{
							if (anchorName == "LTrackpadZoneLow")
							{
								gamepadButton = 236;
								goto IL_1022;
							}
							if (!(anchorName == "RTrackpadZoneLow"))
							{
								goto IL_1016;
							}
							gamepadButton = 239;
							goto IL_1022;
						}
					}
					else if (c != 'i')
					{
						if (c != 'o')
						{
							if (c != 't')
							{
								goto IL_1016;
							}
							if (!(anchorName == "LeftPaddleBottom"))
							{
								goto IL_1016;
							}
							gamepadButton = 31;
							goto IL_1022;
						}
						else
						{
							if (anchorName == "BtnTouchpad1Down")
							{
								gamepadButton = 92;
								goto IL_1022;
							}
							if (!(anchorName == "BtnTouchpad2Down"))
							{
								goto IL_1016;
							}
							gamepadButton = 101;
							goto IL_1022;
						}
					}
					else
					{
						if (anchorName == "LtriggerZoneHigh")
						{
							gamepadButton = 54;
							goto IL_1022;
						}
						if (!(anchorName == "RtriggerZoneHigh"))
						{
							goto IL_1016;
						}
						gamepadButton = 58;
						goto IL_1022;
					}
					break;
				}
				case 17:
				{
					char c = anchorName[11];
					if (c <= '2')
					{
						if (c != '1')
						{
							if (c != '2')
							{
								goto IL_1016;
							}
							if (anchorName == "BtnTouchpad2Right")
							{
								gamepadButton = 103;
								goto IL_1022;
							}
							if (!(anchorName == "BtnTouchpad2Click"))
							{
								goto IL_1016;
							}
							gamepadButton = 108;
							goto IL_1022;
						}
						else
						{
							if (anchorName == "BtnTouchpad1Right")
							{
								gamepadButton = 94;
								goto IL_1022;
							}
							if (!(anchorName == "BtnTouchpad1Click"))
							{
								goto IL_1016;
							}
							gamepadButton = 99;
							goto IL_1022;
						}
					}
					else if (c != 'B')
					{
						if (c != 'n')
						{
							goto IL_1016;
						}
						if (anchorName == "LTrackpadZoneHigh")
						{
							gamepadButton = 238;
							goto IL_1022;
						}
						if (!(anchorName == "RTrackpadZoneHigh"))
						{
							goto IL_1016;
						}
						gamepadButton = 241;
						goto IL_1022;
					}
					else
					{
						if (!(anchorName == "RightPaddleBottom"))
						{
							goto IL_1016;
						}
						gamepadButton = 32;
						goto IL_1022;
					}
					break;
				}
				case 19:
					if (!(anchorName == "MouseDirectionFrame"))
					{
						goto IL_1016;
					}
					controllerBindingFrameAdditionalModes = new ControllerBindingFrameAdditionalModes?(1);
					goto IL_1022;
				default:
					goto IL_1016;
				}
				gamepadButton = 9;
				goto IL_1022;
				IL_CE7:
				gamepadButton = 10;
				goto IL_1022;
				IL_CF7:
				gamepadButton = 12;
				goto IL_1022;
				IL_CFF:
				gamepadButton = 13;
				goto IL_1022;
			}
			IL_1016:
			GamepadButton gamepadButton2;
			if (Enum.TryParse<GamepadButton>(anchorName, out gamepadButton2))
			{
				gamepadButton = gamepadButton2;
			}
			IL_1022:
			if (GamepadButtonExtensions.IsRealButton(gamepadButton))
			{
				return new AssociatedControllerButton(gamepadButton);
			}
			if (keyScanCodeV != KeyScanCodeV2.NoMap)
			{
				return new AssociatedControllerButton(keyScanCodeV);
			}
			if (controllerBindingFrameAdditionalModes != null)
			{
				return new AssociatedControllerButton(controllerBindingFrameAdditionalModes.Value);
			}
			return new AssociatedControllerButton();
		}

		public static ConfigData CreateDefaultCollectionXBBindingWrappers(ConfigVM configVM, bool empty = false)
		{
			ConfigData configData = new ConfigData(configVM, ListSortDirection.Ascending, "ControllerFamily", "Index");
			if (!empty)
			{
				configData.Add(new SubConfigData(configData, new MainXBBindingCollection(configVM, 0), 0, 0));
				configData.Add(new SubConfigData(configData, new MainXBBindingCollection(configVM, 1), 1, 0));
				configData.Add(new SubConfigData(configData, new MainXBBindingCollection(configVM, 2), 2, 0));
				configData.SetLayersCount(Constants.DynamicShifts ? 2 : 5);
				if (Constants.CreateOverlayShift)
				{
					configData.AddShift(2);
					if (configData.IsOverlayBaseXbBindingCollectionPresent())
					{
						configData.OverlayMenu = new OverlayMenuVM(configData.GetOverlayBaseXbBindingCollection());
					}
				}
			}
			return configData;
		}

		public static Drawing GetDrawingShiftNum(int shiftMod)
		{
			Application application = Application.Current;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Shift");
			defaultInterpolatedStringHandler.AppendFormatted<int>(shiftMod);
			defaultInterpolatedStringHandler.AppendLiteral("White");
			return application.TryFindResource(defaultInterpolatedStringHandler.ToStringAndClear()) as Drawing;
		}

		public static Drawing GetDrawingShiftModificatorNum(int shiftIndex)
		{
			Application application = Application.Current;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Shift");
			defaultInterpolatedStringHandler.AppendFormatted<int>(shiftIndex);
			defaultInterpolatedStringHandler.AppendLiteral("White");
			return application.TryFindResource(defaultInterpolatedStringHandler.ToStringAndClear()) as Drawing;
		}

		public static SolidColorBrush GetBrushForShiftModificatorNum(int shiftIndex)
		{
			if (shiftIndex == 0)
			{
				return Application.Current.TryFindResource("CreamBrush") as SolidColorBrush;
			}
			Application application = Application.Current;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Shift");
			defaultInterpolatedStringHandler.AppendFormatted<int>(shiftIndex);
			defaultInterpolatedStringHandler.AppendLiteral("Brush");
			return application.TryFindResource(defaultInterpolatedStringHandler.ToStringAndClear()) as SolidColorBrush;
		}

		public static Type GetZoneTypeByControllerFamily(ControllerFamily? controllerFamily)
		{
			if (controllerFamily != null)
			{
				switch (controllerFamily.GetValueOrDefault())
				{
				case 1:
					return typeof(KeyboardMappingView);
				case 2:
					return typeof(MouseMappingView);
				case 3:
					return typeof(CompositeDeviceBlankView);
				}
			}
			return typeof(SVGGamepadWithAllAnnotations);
		}

		public static bool BlockNavigateContent { get; set; }

		public static void NavigateGamepadZoneForControllerFamily(ControllerFamily? controllerFamily)
		{
			reWASDApplicationCommands.NavigateGamepadCommand.Execute(XBUtils.GetZoneTypeByControllerFamily(controllerFamily));
		}

		public static void NavigateContentToMain()
		{
			if (!XBUtils.BlockNavigateContent)
			{
				reWASDApplicationCommands.NavigateContentCommand.Execute(typeof(MainContent));
			}
		}

		public static Drawing GetAdaptersImage(ExternalDeviceType externalDevice, bool isOnlineAndCorrect = true, bool isBluetoothSecureSimpleParingIsNotPresent = false)
		{
			Drawing drawing;
			switch (externalDevice)
			{
			case 0:
				if (isOnlineAndCorrect && !isBluetoothSecureSimpleParingIsNotPresent)
				{
					drawing = Application.Current.TryFindResource("AdapterKeyBT") as Drawing;
				}
				else
				{
					drawing = Application.Current.TryFindResource("AdapterKeyBTExclamationMark") as Drawing;
				}
				break;
			case 1:
				if (isOnlineAndCorrect)
				{
					drawing = Application.Current.TryFindResource("AdapterKeyGIMX") as Drawing;
				}
				else
				{
					drawing = Application.Current.TryFindResource("AdapterKeyGIMXExclamationMark") as Drawing;
				}
				break;
			case 2:
				if (isOnlineAndCorrect)
				{
					drawing = Application.Current.TryFindResource("AdapterKeyESP32") as Drawing;
				}
				else
				{
					drawing = Application.Current.TryFindResource("AdapterKeyESP32ExclamationMark") as Drawing;
				}
				break;
			case 3:
				if (isOnlineAndCorrect)
				{
					drawing = Application.Current.TryFindResource("AdapterKeyGIMX") as Drawing;
				}
				else
				{
					drawing = Application.Current.TryFindResource("AdapterKeyGIMXExclamationMark") as Drawing;
				}
				break;
			default:
				if (externalDevice != 1000)
				{
					drawing = Application.Current.TryFindResource("AdapterKeyQuestion") as Drawing;
				}
				else
				{
					drawing = Application.Current.TryFindResource("AddKey") as Drawing;
				}
				break;
			}
			return drawing;
		}

		public static ControllerTypeEnum? CorrectNintendoSwitchJoy(ControllerTypeEnum? controllerType)
		{
			ControllerTypeEnum? controllerTypeEnum = controllerType;
			ControllerTypeEnum controllerTypeEnum2 = 9;
			if (!((controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null)))
			{
				controllerTypeEnum = controllerType;
				controllerTypeEnum2 = 10;
				if (!((controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null)))
				{
					return controllerType;
				}
			}
			return new ControllerTypeEnum?(7);
		}

		public static ControllerTypeEnum OppositeNintendoSwitchJoyCon(ControllerTypeEnum controllerType)
		{
			if (controllerType == 9)
			{
				return 10;
			}
			return 9;
		}

		public static ControllerTypeEnum GetDefaultGamepadControllerType(XBBinding xb = null)
		{
			BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
			ControllerTypeEnum? controllerTypeEnum;
			if (currentGamepad == null)
			{
				controllerTypeEnum = null;
			}
			else
			{
				ControllerVM currentController = currentGamepad.CurrentController;
				controllerTypeEnum = ((currentController != null) ? new ControllerTypeEnum?(currentController.ControllerType) : null);
			}
			ControllerTypeEnum? controllerTypeEnum2 = controllerTypeEnum;
			if (controllerTypeEnum2 == null)
			{
				BaseControllerVM currentGamepad2 = App.GamepadService.CurrentGamepad;
				ControllerTypeEnum? controllerTypeEnum3;
				if (currentGamepad2 == null)
				{
					controllerTypeEnum3 = null;
				}
				else
				{
					ControllerVM currentController2 = currentGamepad2.CurrentController;
					controllerTypeEnum3 = ((currentController2 != null) ? currentController2.FirstGamepadType : null);
				}
				controllerTypeEnum2 = controllerTypeEnum3;
			}
			ControllerFamily? controllerFamily;
			if (xb != null)
			{
				controllerFamily = new ControllerFamily?(xb.HostCollection.SubConfigData.ControllerFamily);
			}
			else
			{
				GameVM currentGame = App.GameProfilesService.CurrentGame;
				ControllerFamily? controllerFamily2;
				if (currentGame == null)
				{
					controllerFamily2 = null;
				}
				else
				{
					ConfigVM currentConfig = currentGame.CurrentConfig;
					if (currentConfig == null)
					{
						controllerFamily2 = null;
					}
					else
					{
						SubConfigData currentSubConfigData = currentConfig.CurrentSubConfigData;
						controllerFamily2 = ((currentSubConfigData != null) ? new ControllerFamily?(currentSubConfigData.ControllerFamily) : null);
					}
				}
				controllerFamily = controllerFamily2;
				if (controllerFamily == null)
				{
					controllerFamily = new ControllerFamily?(0);
				}
			}
			ControllerFamily? controllerFamily3 = controllerFamily;
			ControllerFamily controllerFamily4 = 0;
			if (((controllerFamily3.GetValueOrDefault() == controllerFamily4) & (controllerFamily3 != null)) && (controllerTypeEnum2 == null || !ControllerTypeExtensions.IsGamepad(controllerTypeEnum2.GetValueOrDefault())))
			{
				controllerTypeEnum2 = new ControllerTypeEnum?(3);
			}
			controllerFamily3 = controllerFamily;
			controllerFamily4 = 2;
			if ((controllerFamily3.GetValueOrDefault() == controllerFamily4) & (controllerFamily3 != null))
			{
				controllerTypeEnum2 = new ControllerTypeEnum?(1001);
			}
			controllerFamily3 = controllerFamily;
			controllerFamily4 = 1;
			if ((controllerFamily3.GetValueOrDefault() == controllerFamily4) & (controllerFamily3 != null))
			{
				controllerTypeEnum2 = new ControllerTypeEnum?(1000);
			}
			return controllerTypeEnum2.Value;
		}

		public static void ExtractXBBinding(object obj, ref XBBinding xBBinding)
		{
			XBBinding xbbinding = obj as XBBinding;
			if (xbbinding != null)
			{
				xBBinding = xbbinding;
			}
		}

		public static string GetControllerRequestLink(ControllerVM controller)
		{
			string[] array = new string[9];
			array[0] = "https://www.daemon-tools.cc/contacts/producttechnicalsupport";
			int num = 1;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 2);
			defaultInterpolatedStringHandler.AppendLiteral("?extra[vid]=");
			defaultInterpolatedStringHandler.AppendFormatted<ushort>(controller.VendorId, "x4");
			defaultInterpolatedStringHandler.AppendLiteral("&extra[pid]=");
			defaultInterpolatedStringHandler.AppendFormatted<ushort?>((controller != null) ? new ushort?(controller.ProductId) : null, "x4");
			array[num] = defaultInterpolatedStringHandler.ToStringAndClear();
			array[2] = "&extra[manufacturer]=";
			array[3] = HttpUtility.UrlEncode((controller != null) ? controller.ManufacturerName : null);
			array[4] = "&extra[container]=";
			array[5] = HttpUtility.UrlEncode((controller != null) ? controller.ContainerDescription : null);
			array[6] = "&extra[description]=";
			array[7] = HttpUtility.UrlEncode((controller != null) ? controller.Description : null);
			array[8] = "&subject=controller-request";
			return string.Concat(array);
		}

		public static string NormalizeToMaxPathTrimFilename(string dir, string filename, string ext, string suffix = null)
		{
			if ((dir + filename + ext).Length > 260)
			{
				try
				{
					int num = 259 - (dir.Length + ext.Length);
					if (!string.IsNullOrEmpty(suffix))
					{
						num -= suffix.Length;
					}
					string text = filename.Substring(0, num);
					if (!string.IsNullOrEmpty(suffix))
					{
						text += suffix;
					}
					return text;
				}
				catch (Exception)
				{
				}
				return filename;
			}
			return filename;
		}

		public static void CopyVirtualSettings(MainXBBindingCollection source, MainXBBindingCollection destination)
		{
			if (source == null || destination == null)
			{
				return;
			}
			destination.MouseAcceleration = source.MouseAcceleration;
			destination.MouseDeflection = source.MouseDeflection;
			destination.MouseSensitivity = source.MouseSensitivity;
			destination.WheelDeflection = source.WheelDeflection;
			destination.VirtualKeyboardRepeatRate = source.VirtualKeyboardRepeatRate;
		}

		public static int GetListRangeElementsTotalSize(ref List<byte[]> list, int begin, int end)
		{
			int num = 0;
			if (list == null || list.Count == 0 || begin > end)
			{
				return 0;
			}
			for (int i = begin; i < end; i++)
			{
				num += list[i].Length;
			}
			return num;
		}

		public static void SetPhysicalTrackPadCurveDeflectionDefault(ref DISC_SOFT_GAMEPAD_STICK_DEFLECTION stickCurve)
		{
			if (stickCurve.HorizontalPoint == null)
			{
				stickCurve.HorizontalPoint = new DISC_SOFT_GAMEPAD_STICK_DEFLECTION_POINT[4];
			}
			stickCurve.HorizontalPoint[0].TravelDistance = 8190;
			stickCurve.HorizontalPoint[0].NewValue = 9584;
			stickCurve.HorizontalPoint[1].TravelDistance = stickCurve.HorizontalPoint[0].TravelDistance;
			stickCurve.HorizontalPoint[1].NewValue = stickCurve.HorizontalPoint[0].NewValue;
			stickCurve.HorizontalPoint[2].TravelDistance = 28000;
			stickCurve.HorizontalPoint[2].NewValue = 32768;
			stickCurve.HorizontalPoint[3].TravelDistance = stickCurve.HorizontalPoint[2].TravelDistance;
			stickCurve.HorizontalPoint[3].NewValue = stickCurve.HorizontalPoint[2].NewValue;
			if (stickCurve.VerticalPoint == null)
			{
				stickCurve.VerticalPoint = new DISC_SOFT_GAMEPAD_STICK_DEFLECTION_POINT[4];
			}
			stickCurve.VerticalPoint[0].TravelDistance = stickCurve.HorizontalPoint[0].TravelDistance;
			stickCurve.VerticalPoint[0].NewValue = stickCurve.HorizontalPoint[0].NewValue;
			stickCurve.VerticalPoint[1].TravelDistance = stickCurve.HorizontalPoint[1].TravelDistance;
			stickCurve.VerticalPoint[1].NewValue = stickCurve.HorizontalPoint[1].NewValue;
			stickCurve.VerticalPoint[2].TravelDistance = stickCurve.HorizontalPoint[2].TravelDistance;
			stickCurve.VerticalPoint[2].NewValue = stickCurve.HorizontalPoint[2].NewValue;
			stickCurve.VerticalPoint[3].TravelDistance = stickCurve.HorizontalPoint[3].TravelDistance;
			stickCurve.VerticalPoint[3].NewValue = stickCurve.HorizontalPoint[3].NewValue;
		}

		public static bool IsPhysicalTrackPadCurveDeflectionDefault(ref DISC_SOFT_GAMEPAD_STICK_DEFLECTION stickCurve)
		{
			return stickCurve.HorizontalPoint != null && stickCurve.VerticalPoint != null && (stickCurve.HorizontalPoint[0].TravelDistance == 8190 && stickCurve.HorizontalPoint[0].NewValue == 9584 && stickCurve.HorizontalPoint[1].TravelDistance == 8190 && stickCurve.HorizontalPoint[1].NewValue == 9584 && stickCurve.HorizontalPoint[2].TravelDistance == 28000 && stickCurve.HorizontalPoint[2].NewValue == 32768 && stickCurve.HorizontalPoint[3].TravelDistance == 28000 && stickCurve.HorizontalPoint[3].NewValue == 32768 && stickCurve.VerticalPoint[0].TravelDistance == 8190 && stickCurve.VerticalPoint[0].NewValue == 9584 && stickCurve.VerticalPoint[1].TravelDistance == 8190 && stickCurve.VerticalPoint[1].NewValue == 9584 && stickCurve.VerticalPoint[2].TravelDistance == 28000 && stickCurve.VerticalPoint[2].NewValue == 32768 && stickCurve.VerticalPoint[3].TravelDistance == 28000 && stickCurve.VerticalPoint[3].NewValue == 32768);
		}

		public static void SetGyroCurveDeflectionDefault(ref DISC_SOFT_GAMEPAD_STICK_DEFLECTION stickCurve)
		{
			if (stickCurve.HorizontalPoint == null)
			{
				stickCurve.HorizontalPoint = new DISC_SOFT_GAMEPAD_STICK_DEFLECTION_POINT[4];
			}
			stickCurve.HorizontalPoint[0].TravelDistance = 8000;
			stickCurve.HorizontalPoint[0].NewValue = 16000;
			stickCurve.HorizontalPoint[1].TravelDistance = stickCurve.HorizontalPoint[0].TravelDistance;
			stickCurve.HorizontalPoint[1].NewValue = stickCurve.HorizontalPoint[0].NewValue;
			stickCurve.HorizontalPoint[2].TravelDistance = 16384;
			stickCurve.HorizontalPoint[2].NewValue = 32768;
			stickCurve.HorizontalPoint[3].TravelDistance = stickCurve.HorizontalPoint[2].TravelDistance;
			stickCurve.HorizontalPoint[3].NewValue = stickCurve.HorizontalPoint[2].NewValue;
			if (stickCurve.VerticalPoint == null)
			{
				stickCurve.VerticalPoint = new DISC_SOFT_GAMEPAD_STICK_DEFLECTION_POINT[4];
			}
			stickCurve.VerticalPoint[0].TravelDistance = stickCurve.HorizontalPoint[0].TravelDistance;
			stickCurve.VerticalPoint[0].NewValue = stickCurve.HorizontalPoint[0].NewValue;
			stickCurve.VerticalPoint[1].TravelDistance = stickCurve.HorizontalPoint[1].TravelDistance;
			stickCurve.VerticalPoint[1].NewValue = stickCurve.HorizontalPoint[1].NewValue;
			stickCurve.VerticalPoint[2].TravelDistance = stickCurve.HorizontalPoint[2].TravelDistance;
			stickCurve.VerticalPoint[2].NewValue = stickCurve.HorizontalPoint[2].NewValue;
			stickCurve.VerticalPoint[3].TravelDistance = stickCurve.HorizontalPoint[3].TravelDistance;
			stickCurve.VerticalPoint[3].NewValue = stickCurve.HorizontalPoint[3].NewValue;
		}

		public static bool IsGyroCurveDeflectionDefault(ref DISC_SOFT_GAMEPAD_STICK_DEFLECTION stickCurve)
		{
			return stickCurve.HorizontalPoint != null && stickCurve.VerticalPoint != null && (stickCurve.HorizontalPoint[0].TravelDistance == 8000 && stickCurve.HorizontalPoint[0].NewValue == 16000 && stickCurve.HorizontalPoint[1].TravelDistance == 8000 && stickCurve.HorizontalPoint[1].NewValue == 16000 && stickCurve.HorizontalPoint[2].TravelDistance == 16384 && stickCurve.HorizontalPoint[2].NewValue == 32768 && stickCurve.HorizontalPoint[3].TravelDistance == 16384 && stickCurve.HorizontalPoint[3].NewValue == 32768 && stickCurve.VerticalPoint[0].TravelDistance == 8000 && stickCurve.VerticalPoint[0].NewValue == 16000 && stickCurve.VerticalPoint[1].TravelDistance == 8000 && stickCurve.VerticalPoint[1].NewValue == 16000 && stickCurve.VerticalPoint[2].TravelDistance == 16384 && stickCurve.VerticalPoint[2].NewValue == 32768 && stickCurve.VerticalPoint[3].TravelDistance == 16384 && stickCurve.VerticalPoint[3].NewValue == 32768);
		}

		private static bool IsGyroCurveDeflectionAggressive(ref DISC_SOFT_GAMEPAD_STICK_DEFLECTION stickCurve)
		{
			return stickCurve.HorizontalPoint != null && stickCurve.VerticalPoint != null && (stickCurve.HorizontalPoint[0].TravelDistance == 2730 && stickCurve.HorizontalPoint[0].NewValue == 8195 && stickCurve.HorizontalPoint[1].TravelDistance == 2730 && stickCurve.HorizontalPoint[1].NewValue == 8195 && stickCurve.HorizontalPoint[2].TravelDistance == 6000 && stickCurve.HorizontalPoint[2].NewValue == 26000 && stickCurve.HorizontalPoint[3].TravelDistance == 6000 && stickCurve.HorizontalPoint[3].NewValue == 26000 && stickCurve.VerticalPoint[0].TravelDistance == 2730 && stickCurve.VerticalPoint[0].NewValue == 8195 && stickCurve.VerticalPoint[1].TravelDistance == 2730 && stickCurve.VerticalPoint[1].NewValue == 8195 && stickCurve.VerticalPoint[2].TravelDistance == 6000 && stickCurve.VerticalPoint[2].NewValue == 26000 && stickCurve.VerticalPoint[3].TravelDistance == 6000 && stickCurve.VerticalPoint[3].NewValue == 26000);
		}

		private static bool IsGyroCurveDeflectionMinimizedAcceleration(ref DISC_SOFT_GAMEPAD_STICK_DEFLECTION stickCurve)
		{
			return stickCurve.HorizontalPoint != null && stickCurve.VerticalPoint != null && (stickCurve.HorizontalPoint[0].TravelDistance == 8000 && stickCurve.HorizontalPoint[0].NewValue == 17000 && stickCurve.HorizontalPoint[1].TravelDistance == 8000 && stickCurve.HorizontalPoint[1].NewValue == 17000 && stickCurve.HorizontalPoint[2].TravelDistance == 32768 && stickCurve.HorizontalPoint[2].NewValue == 17000 && stickCurve.HorizontalPoint[3].TravelDistance == 32768 && stickCurve.HorizontalPoint[3].NewValue == 17000 && stickCurve.VerticalPoint[0].TravelDistance == 8000 && stickCurve.VerticalPoint[0].NewValue == 17000 && stickCurve.VerticalPoint[1].TravelDistance == 8000 && stickCurve.VerticalPoint[1].NewValue == 17000 && stickCurve.VerticalPoint[2].TravelDistance == 32768 && stickCurve.VerticalPoint[2].NewValue == 17000 && stickCurve.VerticalPoint[3].TravelDistance == 32768 && stickCurve.VerticalPoint[3].NewValue == 17000);
		}

		public static void SetStickCurveDeflectionDefault(ref DISC_SOFT_GAMEPAD_STICK_DEFLECTION stickCurve)
		{
			if (stickCurve.HorizontalPoint == null)
			{
				stickCurve.HorizontalPoint = new DISC_SOFT_GAMEPAD_STICK_DEFLECTION_POINT[4];
			}
			stickCurve.HorizontalPoint[0].TravelDistance = 8190;
			stickCurve.HorizontalPoint[0].NewValue = 8195;
			stickCurve.HorizontalPoint[1].TravelDistance = stickCurve.HorizontalPoint[0].TravelDistance;
			stickCurve.HorizontalPoint[1].NewValue = stickCurve.HorizontalPoint[0].NewValue;
			stickCurve.HorizontalPoint[2].TravelDistance = 24575;
			stickCurve.HorizontalPoint[2].NewValue = 24575;
			stickCurve.HorizontalPoint[3].TravelDistance = stickCurve.HorizontalPoint[2].TravelDistance;
			stickCurve.HorizontalPoint[3].NewValue = stickCurve.HorizontalPoint[2].NewValue;
			if (stickCurve.VerticalPoint == null)
			{
				stickCurve.VerticalPoint = new DISC_SOFT_GAMEPAD_STICK_DEFLECTION_POINT[4];
			}
			stickCurve.VerticalPoint[0].TravelDistance = stickCurve.HorizontalPoint[0].TravelDistance;
			stickCurve.VerticalPoint[0].NewValue = stickCurve.HorizontalPoint[0].NewValue;
			stickCurve.VerticalPoint[1].TravelDistance = stickCurve.HorizontalPoint[1].TravelDistance;
			stickCurve.VerticalPoint[1].NewValue = stickCurve.HorizontalPoint[1].NewValue;
			stickCurve.VerticalPoint[2].TravelDistance = stickCurve.HorizontalPoint[2].TravelDistance;
			stickCurve.VerticalPoint[2].NewValue = stickCurve.HorizontalPoint[2].NewValue;
			stickCurve.VerticalPoint[3].TravelDistance = stickCurve.HorizontalPoint[3].TravelDistance;
			stickCurve.VerticalPoint[3].NewValue = stickCurve.HorizontalPoint[3].NewValue;
		}

		public static bool IsStickCurveDeflectionDefault(ref DISC_SOFT_GAMEPAD_STICK_DEFLECTION stickCurve)
		{
			return stickCurve.HorizontalPoint != null && stickCurve.VerticalPoint != null && (stickCurve.HorizontalPoint[0].TravelDistance == 8190 && stickCurve.HorizontalPoint[0].NewValue == 8195 && stickCurve.HorizontalPoint[1].TravelDistance == 8190 && stickCurve.HorizontalPoint[1].NewValue == 8195 && stickCurve.HorizontalPoint[2].TravelDistance == 24575 && stickCurve.HorizontalPoint[2].NewValue == 24575 && stickCurve.HorizontalPoint[3].TravelDistance == 24575 && stickCurve.HorizontalPoint[3].NewValue == 24575 && stickCurve.VerticalPoint[0].TravelDistance == 8190 && stickCurve.VerticalPoint[0].NewValue == 8195 && stickCurve.VerticalPoint[1].TravelDistance == 8190 && stickCurve.VerticalPoint[1].NewValue == 8195 && stickCurve.VerticalPoint[2].TravelDistance == 24575 && stickCurve.VerticalPoint[2].NewValue == 24575 && stickCurve.VerticalPoint[3].TravelDistance == 24575 && stickCurve.VerticalPoint[3].NewValue == 24575);
		}

		private static bool IsStickCurveDeflectionDelay(ref DISC_SOFT_GAMEPAD_STICK_DEFLECTION stickCurve)
		{
			return stickCurve.HorizontalPoint != null && stickCurve.VerticalPoint != null && (stickCurve.HorizontalPoint[0].TravelDistance == 8191 && stickCurve.HorizontalPoint[0].NewValue == 8195 && stickCurve.HorizontalPoint[1].TravelDistance == 8191 && stickCurve.HorizontalPoint[1].NewValue == 8195 && stickCurve.HorizontalPoint[2].TravelDistance == 28000 && stickCurve.HorizontalPoint[2].NewValue == 19000 && stickCurve.HorizontalPoint[3].TravelDistance == 28000 && stickCurve.HorizontalPoint[3].NewValue == 19000 && stickCurve.VerticalPoint[0].TravelDistance == 8191 && stickCurve.VerticalPoint[0].NewValue == 8195 && stickCurve.VerticalPoint[1].TravelDistance == 8191 && stickCurve.VerticalPoint[1].NewValue == 8195 && stickCurve.VerticalPoint[2].TravelDistance == 28000 && stickCurve.VerticalPoint[2].NewValue == 19000 && stickCurve.VerticalPoint[3].TravelDistance == 28000 && stickCurve.VerticalPoint[3].NewValue == 19000);
		}

		private static bool IsStickCurveDeflectionAggressive(ref DISC_SOFT_GAMEPAD_STICK_DEFLECTION stickCurve)
		{
			return stickCurve.HorizontalPoint != null && stickCurve.VerticalPoint != null && (stickCurve.HorizontalPoint[0].TravelDistance == 8192 && stickCurve.HorizontalPoint[0].NewValue == 8195 && stickCurve.HorizontalPoint[1].TravelDistance == 8192 && stickCurve.HorizontalPoint[1].NewValue == 8195 && stickCurve.HorizontalPoint[2].TravelDistance == 18000 && stickCurve.HorizontalPoint[2].NewValue == 26000 && stickCurve.HorizontalPoint[3].TravelDistance == 18000 && stickCurve.HorizontalPoint[3].NewValue == 26000 && stickCurve.VerticalPoint[0].TravelDistance == 8192 && stickCurve.VerticalPoint[0].NewValue == 8195 && stickCurve.VerticalPoint[1].TravelDistance == 8192 && stickCurve.VerticalPoint[1].NewValue == 8195 && stickCurve.VerticalPoint[2].TravelDistance == 18000 && stickCurve.VerticalPoint[2].NewValue == 26000 && stickCurve.VerticalPoint[3].TravelDistance == 18000 && stickCurve.VerticalPoint[3].NewValue == 26000);
		}

		private static bool IsStickCurveDeflectionInstant(ref DISC_SOFT_GAMEPAD_STICK_DEFLECTION stickCurve)
		{
			return stickCurve.HorizontalPoint != null && stickCurve.VerticalPoint != null && (stickCurve.HorizontalPoint[0].TravelDistance == 8193 && stickCurve.HorizontalPoint[0].NewValue == 8195 && stickCurve.HorizontalPoint[1].TravelDistance == 8193 && stickCurve.HorizontalPoint[1].NewValue == 8195 && stickCurve.HorizontalPoint[2].TravelDistance == 8200 && stickCurve.HorizontalPoint[2].NewValue == 13435 && stickCurve.HorizontalPoint[3].TravelDistance == 8200 && stickCurve.HorizontalPoint[3].NewValue == 13435 && stickCurve.VerticalPoint[0].TravelDistance == 8193 && stickCurve.VerticalPoint[0].NewValue == 8195 && stickCurve.VerticalPoint[1].TravelDistance == 8193 && stickCurve.VerticalPoint[1].NewValue == 8195 && stickCurve.VerticalPoint[2].TravelDistance == 8200 && stickCurve.VerticalPoint[2].NewValue == 13435 && stickCurve.VerticalPoint[3].TravelDistance == 8200 && stickCurve.VerticalPoint[3].NewValue == 13435);
		}

		private static bool IsStickCurveDeflectionSmooth(ref DISC_SOFT_GAMEPAD_STICK_DEFLECTION stickCurve)
		{
			return stickCurve.HorizontalPoint != null && stickCurve.VerticalPoint != null && (stickCurve.HorizontalPoint[0].TravelDistance == 17814 && stickCurve.HorizontalPoint[0].NewValue == 17815 && stickCurve.HorizontalPoint[1].TravelDistance == 17814 && stickCurve.HorizontalPoint[1].NewValue == 17815 && stickCurve.HorizontalPoint[2].TravelDistance == 27852 && stickCurve.HorizontalPoint[2].NewValue == 20000 && stickCurve.HorizontalPoint[3].TravelDistance == 27852 && stickCurve.HorizontalPoint[3].NewValue == 20000 && stickCurve.VerticalPoint[0].TravelDistance == 17814 && stickCurve.VerticalPoint[0].NewValue == 17815 && stickCurve.VerticalPoint[1].TravelDistance == 17814 && stickCurve.VerticalPoint[1].NewValue == 17815 && stickCurve.VerticalPoint[2].TravelDistance == 27852 && stickCurve.VerticalPoint[2].NewValue == 20000 && stickCurve.VerticalPoint[3].TravelDistance == 27852 && stickCurve.VerticalPoint[3].NewValue == 20000);
		}

		public static bool IsStickCurveDeflectionCustom(ref DISC_SOFT_GAMEPAD_STICK_DEFLECTION stickDeflection)
		{
			return !XBUtils.IsStickCurveDeflectionDefault(ref stickDeflection) && !XBUtils.IsStickCurveDeflectionDelay(ref stickDeflection) && !XBUtils.IsStickCurveDeflectionAggressive(ref stickDeflection) && !XBUtils.IsStickCurveDeflectionInstant(ref stickDeflection) && !XBUtils.IsStickCurveDeflectionSmooth(ref stickDeflection);
		}

		public static bool IsGyroCurveDeflectionCustom(ref DISC_SOFT_GAMEPAD_STICK_DEFLECTION stickDeflection)
		{
			return !XBUtils.IsGyroCurveDeflectionDefault(ref stickDeflection) && !XBUtils.IsGyroCurveDeflectionAggressive(ref stickDeflection) && !XBUtils.IsGyroCurveDeflectionMinimizedAcceleration(ref stickDeflection);
		}

		public static void CopyStickDeflection(ref DISC_SOFT_GAMEPAD_STICK_DEFLECTION src, ref DISC_SOFT_GAMEPAD_STICK_DEFLECTION dest)
		{
			if (src.HorizontalPoint == null || dest.HorizontalPoint == null || src.VerticalPoint == null || dest.VerticalPoint == null)
			{
				return;
			}
			dest.HorizontalPoint[0].TravelDistance = src.HorizontalPoint[0].TravelDistance;
			dest.HorizontalPoint[0].NewValue = src.HorizontalPoint[0].NewValue;
			dest.HorizontalPoint[1].TravelDistance = src.HorizontalPoint[1].TravelDistance;
			dest.HorizontalPoint[1].NewValue = src.HorizontalPoint[1].NewValue;
			dest.HorizontalPoint[2].TravelDistance = src.HorizontalPoint[2].TravelDistance;
			dest.HorizontalPoint[2].NewValue = src.HorizontalPoint[2].NewValue;
			dest.HorizontalPoint[3].TravelDistance = src.HorizontalPoint[3].TravelDistance;
			dest.HorizontalPoint[3].NewValue = src.HorizontalPoint[3].NewValue;
			dest.VerticalPoint[0].TravelDistance = src.VerticalPoint[0].TravelDistance;
			dest.VerticalPoint[0].NewValue = src.VerticalPoint[0].NewValue;
			dest.VerticalPoint[1].TravelDistance = src.VerticalPoint[1].TravelDistance;
			dest.VerticalPoint[1].NewValue = src.VerticalPoint[1].NewValue;
			dest.VerticalPoint[2].TravelDistance = src.VerticalPoint[2].TravelDistance;
			dest.VerticalPoint[2].NewValue = src.VerticalPoint[2].NewValue;
			dest.VerticalPoint[3].TravelDistance = src.VerticalPoint[3].TravelDistance;
			dest.VerticalPoint[3].NewValue = src.VerticalPoint[3].NewValue;
		}

		public static void CopyStickDeflectionFromBinding(DISC_SOFT_GAMEPAD_STICK_DEFLECTION_NPROPCHANGE src, ref DISC_SOFT_GAMEPAD_STICK_DEFLECTION dest)
		{
			if (src.HorizontalPoint == null || dest.HorizontalPoint == null || src.VerticalPoint == null || dest.VerticalPoint == null)
			{
				return;
			}
			dest.HorizontalPoint[0].TravelDistance = src.HorizontalPoint[0].TravelDistance;
			dest.HorizontalPoint[0].NewValue = src.HorizontalPoint[0].NewValue;
			dest.HorizontalPoint[1].TravelDistance = src.HorizontalPoint[1].TravelDistance;
			dest.HorizontalPoint[1].NewValue = src.HorizontalPoint[1].NewValue;
			dest.HorizontalPoint[2].TravelDistance = src.HorizontalPoint[2].TravelDistance;
			dest.HorizontalPoint[2].NewValue = src.HorizontalPoint[2].NewValue;
			dest.HorizontalPoint[3].TravelDistance = src.HorizontalPoint[3].TravelDistance;
			dest.HorizontalPoint[3].NewValue = src.HorizontalPoint[3].NewValue;
			dest.VerticalPoint[0].TravelDistance = src.VerticalPoint[0].TravelDistance;
			dest.VerticalPoint[0].NewValue = src.VerticalPoint[0].NewValue;
			dest.VerticalPoint[1].TravelDistance = src.VerticalPoint[1].TravelDistance;
			dest.VerticalPoint[1].NewValue = src.VerticalPoint[1].NewValue;
			dest.VerticalPoint[2].TravelDistance = src.VerticalPoint[2].TravelDistance;
			dest.VerticalPoint[2].NewValue = src.VerticalPoint[2].NewValue;
			dest.VerticalPoint[3].TravelDistance = src.VerticalPoint[3].TravelDistance;
			dest.VerticalPoint[3].NewValue = src.VerticalPoint[3].NewValue;
		}

		public static string GetFileMD5Hash(string fullPath)
		{
			StringBuilder stringBuilder = new StringBuilder();
			FileStream fileStream = null;
			StreamReader streamReader = null;
			try
			{
				fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
				streamReader = new StreamReader(fileStream);
				string text = streamReader.ReadToEnd();
				using (MD5 md = MD5.Create())
				{
					foreach (byte b in md.ComputeHash(Encoding.Unicode.GetBytes(text)))
					{
						stringBuilder.Append(b.ToString("x2"));
					}
				}
				streamReader.Close();
				fileStream.Close();
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Dispose();
				}
				if (streamReader != null)
				{
					streamReader.Dispose();
				}
			}
			return stringBuilder.ToString();
		}

		public static uint RNGRandomUInt32()
		{
			byte[] array = new byte[4];
			RandomNumberGenerator.Create().GetBytes(array);
			return BitConverter.ToUInt32(array, 0);
		}

		public static long RNGRandomLong()
		{
			byte[] array = new byte[8];
			RandomNumberGenerator.Create().GetBytes(array);
			return BitConverter.ToInt64(array, 0);
		}

		public static ulong RNGRandomULong()
		{
			byte[] array = new byte[8];
			RandomNumberGenerator.Create().GetBytes(array);
			return BitConverter.ToUInt64(array, 0);
		}

		public static IEnumerable<MaskItemConditionCollection> ConvertSlotHotkeyToMaskItemConditionCollectionList(ref List<ObservableCollection<AssociatedControllerButton>> hotkeyCollections)
		{
			List<MaskItemConditionCollection> list = new List<MaskItemConditionCollection>();
			foreach (Collection<AssociatedControllerButton> collection in hotkeyCollections)
			{
				MaskItemConditionCollection maskItemConditionCollection = new MaskItemConditionCollection(null);
				int num = 0;
				foreach (AssociatedControllerButton associatedControllerButton in collection)
				{
					if (num >= maskItemConditionCollection.Count)
					{
						break;
					}
					if (associatedControllerButton.IsGamepad)
					{
						maskItemConditionCollection[num] = new MaskItemCondition(associatedControllerButton.GamepadButton);
					}
					if (associatedControllerButton.IsKeyScanCode)
					{
						ControllerFamily controllerFamily = (associatedControllerButton.KeyScanCode.IsCategoryMouseDigital ? 2 : 1);
						maskItemConditionCollection[num] = new MaskItemCondition(associatedControllerButton.KeyScanCode, controllerFamily);
					}
					num++;
				}
				maskItemConditionCollection.RecalculateBitMask();
				list.Add(maskItemConditionCollection);
			}
			return list;
		}

		public static bool IsHotkeysSame(ObservableCollection<AssociatedControllerButton> col1, ObservableCollection<AssociatedControllerButton> col2)
		{
			bool flag = XBUtils.IsFirstInSecondPresent(col1, col2);
			bool flag2 = false;
			if (flag)
			{
				flag2 = XBUtils.IsFirstInSecondPresent(col2, col1);
			}
			return flag && flag2;
		}

		private static bool IsFirstInSecondPresent(ObservableCollection<AssociatedControllerButton> col1, ObservableCollection<AssociatedControllerButton> col2)
		{
			bool flag = true;
			bool flag2 = true;
			for (int i = 0; i < col1.Count; i++)
			{
				bool flag3 = false;
				if (col1[i].IsSet)
				{
					flag2 = false;
					for (int j = 0; j < col2.Count; j++)
					{
						if (col2[j].IsSet && col1[i].IsAssociatedSetToEqualButtons(col2[j]))
						{
							flag3 = true;
							break;
						}
					}
					flag = flag && flag3;
				}
				if (!flag)
				{
					break;
				}
			}
			return flag && !flag2;
		}

		public static bool IsSlotHotkeyCollectionsValid(List<ObservableCollection<AssociatedControllerButton>> hotkeyCollections, bool isSlots)
		{
			foreach (ObservableCollection<AssociatedControllerButton> observableCollection in hotkeyCollections)
			{
				if (observableCollection.Distinct<AssociatedControllerButton>().Count<AssociatedControllerButton>() == 1)
				{
					return false;
				}
				for (int i = 0; i < observableCollection.Count - 1; i++)
				{
					for (int j = i + 1; j < observableCollection.Count; j++)
					{
						if (observableCollection[i].IsSet && observableCollection[i].IsAssociatedSetToEqualButtons(observableCollection[j]))
						{
							return false;
						}
					}
				}
			}
			if (isSlots)
			{
				IEnumerable<MaskItemConditionCollection> enumerable = XBUtils.ConvertSlotHotkeyToMaskItemConditionCollectionList(ref hotkeyCollections);
				if (enumerable != null)
				{
					List<MaskItemBitMaskWrapper> list = new List<MaskItemBitMaskWrapper>();
					using (IEnumerator<MaskItemConditionCollection> enumerator2 = enumerable.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							MaskItemConditionCollection slotHotkeyMaskItemConditionCollection = enumerator2.Current;
							if (list.Exists((MaskItemBitMaskWrapper x) => x.Equals(slotHotkeyMaskItemConditionCollection.MaskItemBitMaskWrapper)))
							{
								return false;
							}
							list.Add(slotHotkeyMaskItemConditionCollection.MaskItemBitMaskWrapper);
						}
					}
					list.Clear();
				}
				return true;
			}
			return true;
		}

		private static string GetSlotHotkeyStringFromRegistry(Slot slot)
		{
			string text;
			switch (slot)
			{
			case 0:
				text = RegistryHelper.GetString("Config", "Slot1Collection", "4;5;2", false);
				break;
			case 1:
				text = RegistryHelper.GetString("Config", "Slot2Collection", "4;5;3", false);
				break;
			case 2:
				text = RegistryHelper.GetString("Config", "Slot3Collection", "4;5;1", false);
				break;
			case 3:
				text = RegistryHelper.GetString("Config", "Slot4Collection", "4;5;0", false);
				break;
			default:
				throw new ArgumentOutOfRangeException("slot", slot, null);
			}
			return text;
		}

		private static string GetGamepadOverlayHotkeyStringFromRegistry()
		{
			return RegistryHelper.GetString("Config", "GamepadOverlayCollection", "4;5;0", false);
		}

		private static string GetMapingdOverlayHotkeyStringFromRegistry()
		{
			return RegistryHelper.GetString("Config", "MappingOverlayCollection", "4;5;0", false);
		}

		public static ObservableCollection<AssociatedControllerButton> GetSlotHotkeyCollectionFromRegistry(Slot slot)
		{
			string slotHotkeyStringFromRegistry = XBUtils.GetSlotHotkeyStringFromRegistry(slot);
			ObservableCollection<AssociatedControllerButton> observableCollection = new ObservableCollection<AssociatedControllerButton>();
			observableCollection.Add(new AssociatedControllerButton(2000));
			observableCollection.Add(new AssociatedControllerButton(2000));
			observableCollection.Add(new AssociatedControllerButton(2000));
			XBUtils.SetCollectionFromRegistryString(observableCollection, slotHotkeyStringFromRegistry);
			return observableCollection;
		}

		private static void SetCollectionFromRegistryString(ObservableCollection<AssociatedControllerButton> slotCollection, string slotStr)
		{
			for (int i = 0; i < slotStr.Split(';', StringSplitOptions.None).Length; i++)
			{
				int num = Convert.ToInt32(slotStr.Split(';', StringSplitOptions.None)[i]);
				slotCollection[i].GamepadButtonDescription = GamepadButtonDescription.GAMEPAD_BUTTON_DESCRIPTIONS[num];
			}
		}
	}
}
