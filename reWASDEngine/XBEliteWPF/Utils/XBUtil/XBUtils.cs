using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils;
using reWASDCommon.Infrastructure.Enums;
using reWASDEngine;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Infrastructure.KeyBindingsModel.Mask;
using XBEliteWPF.Infrastructure.KeyBindingsModel.XB;
using XBEliteWPF.Infrastructure.KeyBindingsModel.XBBindingDirectionalGroups;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;

namespace XBEliteWPF.Utils.XBUtil
{
	public static class XBUtils
	{
		public static string CalcShortID(string id)
		{
			string text = "";
			using (SHA256 sha = SHA256.Create())
			{
				byte[] array = sha.ComputeHash(Encoding.UTF8.GetBytes(id));
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.Append(array[i].ToString("x2"));
				}
				text = stringBuilder.ToString();
			}
			return text;
		}

		public static SolidColorBrush GetBrushForShiftIndex(int shiftIndex)
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

		public static string GetOverlaySVGElementName(GamepadButton button)
		{
			string text = "";
			if (button <= 55)
			{
				switch (button)
				{
				case 1:
					text = "Btn1";
					break;
				case 2:
					text = "Btn2";
					break;
				case 3:
					text = "Btn3";
					break;
				case 4:
					text = "Btn4";
					break;
				case 5:
					text = "Btn5";
					break;
				case 6:
					text = "Btn6";
					break;
				case 7:
					text = "Btn7";
					break;
				case 8:
					text = "Btn8";
					break;
				case 9:
					text = "Btn9";
					break;
				case 10:
					text = "Btn10";
					break;
				case 11:
					text = "Btn11";
					break;
				case 12:
					text = "Btn12";
					break;
				default:
					switch (button)
					{
					case 33:
						text = "DpadUp";
						break;
					case 34:
						text = "DpadDown";
						break;
					case 35:
						text = "DpadLeft";
						break;
					case 36:
						text = "DpadRight";
						break;
					case 40:
					case 41:
					case 42:
					case 43:
						text = "Lstick";
						break;
					case 47:
					case 48:
					case 49:
					case 50:
						text = "Rstick";
						break;
					case 51:
						text = "LT";
						break;
					case 55:
						text = "RT";
						break;
					}
					break;
				}
			}
			else
			{
				switch (button)
				{
				case 91:
				case 92:
				case 93:
				case 94:
				case 95:
				case 96:
				case 97:
				case 98:
					text = "Track1";
					break;
				case 99:
					text = "BtnTouchpad1Click";
					break;
				case 100:
				case 101:
				case 102:
				case 103:
				case 104:
				case 105:
				case 106:
				case 107:
					text = "Track2";
					break;
				default:
					if (button - 216 <= 3)
					{
						text = "3Stick";
					}
					break;
				}
			}
			return text;
		}

		public static ControllerTypeEnum GetDefaultGamepadControllerType()
		{
			return 3;
		}

		public static List<XBBinding> GetFictiveButtons(SubConfigData subConfigData, ControllerTypeEnum gamepadType)
		{
			if (subConfigData == null)
			{
				return null;
			}
			if (!subConfigData.IsGamepad)
			{
				return null;
			}
			if (!Engine.UserSettingsService.IsHidePhysicalController)
			{
				return null;
			}
			if (ControllerTypeExtensions.IsFictiveButtonsNotAllowed(gamepadType))
			{
				return null;
			}
			if (!subConfigData.ConfigData.IsVirtualGamepad)
			{
				return null;
			}
			List<XBBinding> list = new List<XBBinding>();
			using (List<GamepadButton>.Enumerator enumerator = XBUtils.CreatePosibleButtonsCollectionForController(new ControllerTypeEnum?(gamepadType)).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GamepadButton button = enumerator.Current;
					if (!GamepadButtonExtensions.IsMouseStick(button) && !GamepadButtonExtensions.IsAnyZone(button) && !GamepadButtonExtensions.IsMouseScroll(button) && (!GamepadButtonExtensions.IsAdditionalStick(button) || subConfigData.MainXBBindingCollection.AdditionalStickDirectionalGroup.IsNativeMode) && !GamepadButtonExtensions.IsAnyVirtualTouchpad(button) && !GamepadButtonExtensions.IsGyroLean(button) && !GamepadButtonExtensions.IsTiltDirection(button) && (button != 108 || ControllerTypeExtensions.IsAnySteam(gamepadType)) && !GamepadButtonExtensions.IsAnyPaddle(button) && !GamepadButtonExtensions.IsVirtualTouchpadDirection(button) && GamepadButtonExtensions.IsRealButton(button))
					{
						BaseTouchpadDirectionalGroup baseTouchpadDirectionalGroup = subConfigData.MainXBBindingCollection.GetDirectionalGroupByXBBinding(button) as BaseTouchpadDirectionalGroup;
						bool? flag = ((baseTouchpadDirectionalGroup != null) ? new bool?(baseTouchpadDirectionalGroup.TouchpadDigitalMode) : null);
						bool? flag2 = ((baseTouchpadDirectionalGroup != null) ? new bool?(baseTouchpadDirectionalGroup.TapValue.IsUnmapped) : null);
						if (XBUtils.GetGamepadButtonFromVirtualGamepadButton(button, flag, flag2, new ControllerTypeEnum?(VirtualControllerTypeExtensions.GetControllerType(subConfigData.MainXBBindingCollection.SubConfigData.ConfigData.VirtualGamepadType)), new ControllerTypeEnum?(gamepadType)) != null && GamepadButtonDescription.GamepadButtonDescriptionDictionary.ContainsKey(button) && subConfigData.MainXBBindingCollection.FirstOrDefault((XBBinding b) => button == b.GamepadButtonDescription.Button && b.IsAnnotationShouldBeShownForMapping(new ControllerTypeEnum?(gamepadType))) == null)
						{
							XBBinding xbbinding = new XBBinding(subConfigData.MainXBBindingCollection, button);
							if (XBUtils.ShouldVirtualGamepadButtonBeVisible(new object[]
							{
								xbbinding,
								gamepadType,
								subConfigData.MainXBBindingCollection.SubConfigData.ConfigData.VirtualGamepadType,
								new GeometryDrawing()
							}))
							{
								list.Add(xbbinding);
							}
						}
					}
				}
			}
			return list;
		}

		public static bool ShouldVirtualGamepadButtonBeVisible(object[] values)
		{
			if (values.Length != 4)
			{
				if (values.Length > 3)
				{
					if (values.Skip(4).Any((object v) => v is bool && (bool)v))
					{
						goto IL_3F;
					}
				}
				return false;
			}
			IL_3F:
			ControllerTypeEnum? controllerTypeEnum = null;
			ControllerTypeEnum? controllerTypeEnum2 = null;
			XBBinding xbbinding = null;
			GamepadButton? gamepadButton = null;
			KeyScanCodeV2 keyScanCodeV = null;
			Drawing drawing = null;
			XBUtils.ProcessArrayItem(values[0], ref gamepadButton, ref keyScanCodeV, ref xbbinding, ref controllerTypeEnum2, ref controllerTypeEnum, ref drawing);
			XBUtils.ProcessArrayItem(values[1], ref gamepadButton, ref keyScanCodeV, ref xbbinding, ref controllerTypeEnum2, ref controllerTypeEnum, ref drawing);
			XBUtils.ProcessArrayItem(values[2], ref gamepadButton, ref keyScanCodeV, ref xbbinding, ref controllerTypeEnum2, ref controllerTypeEnum, ref drawing);
			XBUtils.ProcessArrayItem(values[3], ref gamepadButton, ref keyScanCodeV, ref xbbinding, ref controllerTypeEnum2, ref controllerTypeEnum, ref drawing);
			if (drawing != null)
			{
				GamepadButton? gamepadButton2 = gamepadButton;
				GamepadButton gamepadButton3 = 2001;
				if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)) && (controllerTypeEnum2 == null || !ControllerTypeExtensions.IsAnyAzeron(controllerTypeEnum2.GetValueOrDefault())) && (gamepadButton == null || !GamepadButtonExtensions.IsAnyPaddle(gamepadButton.GetValueOrDefault())) && (controllerTypeEnum == null || !ControllerTypeExtensions.IsEngineControllerControlPad(controllerTypeEnum.GetValueOrDefault())))
				{
					if (xbbinding != null && (controllerTypeEnum2 != null && ControllerTypeExtensions.IsGamepadWithSonyTouchpad(controllerTypeEnum2.GetValueOrDefault())) && (gamepadButton != null && GamepadButtonExtensions.IsPhysicalTrackPad1Direction(gamepadButton.GetValueOrDefault())) && xbbinding.HostCollection.Touchpad1DirectionalGroup.TapValue.IsUnmapped)
					{
						return false;
					}
					if (gamepadButton != null)
					{
						gamepadButton2 = gamepadButton;
						gamepadButton3 = 13;
						if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)))
						{
							gamepadButton2 = gamepadButton;
							gamepadButton3 = 14;
							if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)))
							{
								gamepadButton2 = gamepadButton;
								gamepadButton3 = 15;
								if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)))
								{
									gamepadButton2 = gamepadButton;
									gamepadButton3 = 16;
									if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)))
									{
										gamepadButton2 = gamepadButton;
										gamepadButton3 = 17;
										if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)))
										{
											gamepadButton2 = gamepadButton;
											gamepadButton3 = 18;
											if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)))
											{
												gamepadButton2 = gamepadButton;
												gamepadButton3 = 19;
												if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)))
												{
													gamepadButton2 = gamepadButton;
													gamepadButton3 = 20;
													if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)))
													{
														gamepadButton2 = gamepadButton;
														gamepadButton3 = 21;
														if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)))
														{
															gamepadButton2 = gamepadButton;
															gamepadButton3 = 22;
															if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)))
															{
																gamepadButton2 = gamepadButton;
																gamepadButton3 = 23;
																if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)))
																{
																	gamepadButton2 = gamepadButton;
																	gamepadButton3 = 24;
																	if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)))
																	{
																		gamepadButton2 = gamepadButton;
																		gamepadButton3 = 25;
																		if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)))
																		{
																			gamepadButton2 = gamepadButton;
																			gamepadButton3 = 26;
																			if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)))
																			{
																				gamepadButton2 = gamepadButton;
																				gamepadButton3 = 27;
																				if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)))
																				{
																					gamepadButton2 = gamepadButton;
																					gamepadButton3 = 169;
																					if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)))
																					{
																						gamepadButton2 = gamepadButton;
																						gamepadButton3 = 170;
																						if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)) && !GamepadButtonExtensions.IsAnyPhysicalTrackDiagonalDirection(gamepadButton.Value) && !GamepadButtonExtensions.IsAnyStickDiagonalDirection(gamepadButton.Value))
																						{
																							gamepadButton2 = gamepadButton;
																							gamepadButton3 = 12;
																							ControllerTypeEnum? controllerTypeEnum3;
																							ControllerTypeEnum controllerTypeEnum4;
																							if ((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null))
																							{
																								controllerTypeEnum3 = controllerTypeEnum2;
																								controllerTypeEnum4 = 4;
																								if (!((controllerTypeEnum3.GetValueOrDefault() == controllerTypeEnum4) & (controllerTypeEnum3 != null)))
																								{
																									controllerTypeEnum3 = controllerTypeEnum2;
																									controllerTypeEnum4 = 14;
																									if (!((controllerTypeEnum3.GetValueOrDefault() == controllerTypeEnum4) & (controllerTypeEnum3 != null)))
																									{
																										goto IL_4D9;
																									}
																								}
																								if ((controllerTypeEnum != null && ControllerTypeExtensions.IsNintendoSwitch(controllerTypeEnum.GetValueOrDefault())) || (controllerTypeEnum != null && ControllerTypeExtensions.IsAnyEngineGamepad(controllerTypeEnum.GetValueOrDefault())) || (controllerTypeEnum != null && ControllerTypeExtensions.IsSega(controllerTypeEnum.GetValueOrDefault())) || (controllerTypeEnum != null && ControllerTypeExtensions.IsSwitchOnlineN64(controllerTypeEnum.GetValueOrDefault())) || (controllerTypeEnum != null && ControllerTypeExtensions.IsGameSirG7(controllerTypeEnum.GetValueOrDefault())) || (controllerTypeEnum != null && ControllerTypeExtensions.IsFlydigi(controllerTypeEnum.GetValueOrDefault())) || (controllerTypeEnum != null && ControllerTypeExtensions.IsGeneric(controllerTypeEnum.GetValueOrDefault())) || (controllerTypeEnum != null && ControllerTypeExtensions.IsGoogle(controllerTypeEnum.GetValueOrDefault())))
																								{
																									return false;
																								}
																							}
																							IL_4D9:
																							controllerTypeEnum3 = controllerTypeEnum2;
																							controllerTypeEnum4 = 1;
																							if (!((controllerTypeEnum3.GetValueOrDefault() == controllerTypeEnum4) & (controllerTypeEnum3 != null)))
																							{
																								controllerTypeEnum3 = controllerTypeEnum2;
																								controllerTypeEnum4 = 2;
																								if (!((controllerTypeEnum3.GetValueOrDefault() == controllerTypeEnum4) & (controllerTypeEnum3 != null)))
																								{
																									controllerTypeEnum3 = controllerTypeEnum2;
																									controllerTypeEnum4 = 5;
																									if (!((controllerTypeEnum3.GetValueOrDefault() == controllerTypeEnum4) & (controllerTypeEnum3 != null)))
																									{
																										goto IL_5F5;
																									}
																								}
																							}
																							gamepadButton2 = gamepadButton;
																							gamepadButton3 = 12;
																							if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)))
																							{
																								gamepadButton2 = gamepadButton;
																								gamepadButton3 = 163;
																								if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)))
																								{
																									gamepadButton2 = gamepadButton;
																									gamepadButton3 = 99;
																									if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)) && (gamepadButton == null || !GamepadButtonExtensions.IsPhysicalTrackPad1Direction(gamepadButton.GetValueOrDefault())) && (gamepadButton == null || !GamepadButtonExtensions.IsAnyVirtualTouchpad(gamepadButton.GetValueOrDefault())))
																									{
																										goto IL_5F5;
																									}
																								}
																							}
																							bool flag = false;
																							if (gamepadButton != null && GamepadButtonExtensions.IsPhysicalTrackPad1Direction(gamepadButton.GetValueOrDefault()))
																							{
																								flag = controllerTypeEnum != null && ControllerTypeExtensions.IsSteam(controllerTypeEnum.GetValueOrDefault());
																							}
																							if (!flag)
																							{
																								return false;
																							}
																							IL_5F5:
																							controllerTypeEnum3 = controllerTypeEnum2;
																							controllerTypeEnum4 = 7;
																							if ((controllerTypeEnum3.GetValueOrDefault() == controllerTypeEnum4) & (controllerTypeEnum3 != null))
																							{
																								gamepadButton2 = gamepadButton;
																								gamepadButton3 = 163;
																								if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)))
																								{
																									gamepadButton2 = gamepadButton;
																									gamepadButton3 = 99;
																									if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)) && (gamepadButton == null || !GamepadButtonExtensions.IsPhysicalTrackPad1Direction(gamepadButton.GetValueOrDefault())) && (gamepadButton == null || !GamepadButtonExtensions.IsAnyVirtualTouchpad(gamepadButton.GetValueOrDefault())) && (gamepadButton == null || !GamepadButtonExtensions.IsAnyTriggerZone(gamepadButton.GetValueOrDefault())))
																									{
																										goto IL_6D9;
																									}
																								}
																								bool flag2 = false;
																								if (gamepadButton != null && GamepadButtonExtensions.IsPhysicalTrackPad1Direction(gamepadButton.GetValueOrDefault()))
																								{
																									flag2 = controllerTypeEnum != null && ControllerTypeExtensions.IsSteam(controllerTypeEnum.GetValueOrDefault());
																								}
																								if (!flag2)
																								{
																									return false;
																								}
																							}
																							IL_6D9:
																							return xbbinding != null && !xbbinding.IsRemapedOrUnmapped && xbbinding.SingleActivator != null && !xbbinding.SingleActivator.IsVirtualMappingPresentSkipRumble && (!GamepadButtonExtensions.IsGyroTilt(xbbinding.ControllerButton.GamepadButton) || (controllerTypeEnum2 != null && ControllerTypeExtensions.IsVirtualGyroAvailiable(controllerTypeEnum2.GetValueOrDefault())));
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
					return false;
				}
			}
			return false;
		}

		private static void ProcessArrayItem(object item, ref GamepadButton? gamepadButton, ref KeyScanCodeV2 keyScanCode, ref XBBinding xbBindingModel, ref ControllerTypeEnum? virtualControllerType, ref ControllerTypeEnum? realControllerType, ref Drawing drawing)
		{
			XBBinding xbbinding = item as XBBinding;
			if (xbbinding != null)
			{
				xbBindingModel = xbbinding;
				gamepadButton = new GamepadButton?(xbBindingModel.GamepadButton);
			}
			if (item is GamepadButton)
			{
				GamepadButton gamepadButton2 = (GamepadButton)item;
				gamepadButton = new GamepadButton?(gamepadButton2);
			}
			GamepadButtonDescription gamepadButtonDescription = item as GamepadButtonDescription;
			if (gamepadButtonDescription != null)
			{
				gamepadButton = new GamepadButton?(gamepadButtonDescription.Button);
			}
			KeyScanCodeV2 keyScanCodeV = item as KeyScanCodeV2;
			if (keyScanCodeV != null)
			{
				keyScanCode = keyScanCodeV;
			}
			AssociatedControllerButton associatedControllerButton = item as AssociatedControllerButton;
			if (associatedControllerButton != null)
			{
				associatedControllerButton.SetRefButtons(ref gamepadButton, ref keyScanCode);
			}
			if (item is VirtualGamepadType)
			{
				VirtualGamepadType virtualGamepadType = (VirtualGamepadType)item;
				virtualControllerType = new ControllerTypeEnum?(VirtualControllerTypeExtensions.GetControllerType(virtualGamepadType));
			}
			if (item is ControllerTypeEnum)
			{
				ControllerTypeEnum controllerTypeEnum = (ControllerTypeEnum)item;
				realControllerType = new ControllerTypeEnum?(controllerTypeEnum);
			}
			ControllerTypeEnum[] array = item as ControllerTypeEnum[];
			if (array != null)
			{
				realControllerType = new ControllerTypeEnum?(ControllerTypeExtensions.GetFirstOfFamily(array, 0));
			}
			IEnumerable<ControllerTypeEnum> enumerable = item as IEnumerable<ControllerTypeEnum>;
			if (enumerable != null)
			{
				realControllerType = new ControllerTypeEnum?(ControllerTypeExtensions.GetFirstOfFamily(enumerable, 0));
			}
			Drawing drawing2 = item as Drawing;
			if (drawing2 != null)
			{
				drawing = drawing2;
			}
		}

		public static void PairDeviceIDAndSubconfigs(BaseControllerVM controller, ConfigData сonfigData, out Dictionary<string, SubConfigData> ret)
		{
			ret = new Dictionary<string, SubConfigData>();
			if (controller == null || сonfigData == null)
			{
				return;
			}
			CompositeControllerVM compositeControllerVM = controller as CompositeControllerVM;
			if (compositeControllerVM != null)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				using (List<BaseControllerVM>.Enumerator enumerator = compositeControllerVM.BaseControllers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BaseControllerVM baseControllerVM = enumerator.Current;
						if (baseControllerVM != null)
						{
							SubConfigData subConfigData = XBUtils.PairController(baseControllerVM, сonfigData, num, num2, num3);
							switch (baseControllerVM.ControllerFamily)
							{
							case 0:
							case 4:
								num++;
								break;
							case 1:
								num2++;
								break;
							case 2:
								num3++;
								break;
							}
							if (subConfigData != null)
							{
								ret[baseControllerVM.ID] = subConfigData;
							}
						}
					}
					return;
				}
			}
			if (controller is PeripheralVM || controller is ControllerVM)
			{
				SubConfigData subConfigData2 = XBUtils.PairController(controller, сonfigData, 0, 0, 0);
				if (subConfigData2 != null)
				{
					ret[controller.ID] = subConfigData2;
				}
			}
		}

		public static SubConfigData PairController(BaseControllerVM controller, ConfigData сonfigData, int idxGamepad = 0, int idxKeyboard = 0, int idxMouse = 0)
		{
			SubConfigData subConfigData = null;
			if (controller != null)
			{
				switch (controller.ControllerFamily)
				{
				case 0:
				case 4:
					subConfigData = сonfigData.FindSubConfigGamepad((сonfigData.GetSubConfigGamepadList().Count<SubConfigData>() == 1) ? 0 : idxGamepad, false);
					break;
				case 1:
					subConfigData = сonfigData.FindSubConfigKeyboard((сonfigData.GetSubConfigKeyboardList().Count<SubConfigData>() == 1) ? 0 : idxKeyboard, false);
					break;
				case 2:
					subConfigData = сonfigData.FindSubConfigMouse((сonfigData.GetSubConfigMouseList().Count<SubConfigData>() == 1) ? 0 : idxMouse, false);
					break;
				}
			}
			return subConfigData;
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
							if (list.Exists((MaskItemBitMaskWrapper x) => x.Equals(slotHotkeyMaskItemConditionCollection.MaskItemBitMaskWrapper, false)))
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
