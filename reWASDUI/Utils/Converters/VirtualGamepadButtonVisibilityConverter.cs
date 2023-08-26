using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups;
using reWASDUI.Services.Interfaces;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Utils.Converters
{
	public class VirtualGamepadButtonVisibilityConverter : MarkupExtension, IMultiValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			VirtualGamepadButtonVisibilityConverter virtualGamepadButtonVisibilityConverter;
			if ((virtualGamepadButtonVisibilityConverter = VirtualGamepadButtonVisibilityConverter._converter) == null)
			{
				virtualGamepadButtonVisibilityConverter = (VirtualGamepadButtonVisibilityConverter._converter = new VirtualGamepadButtonVisibilityConverter());
			}
			return virtualGamepadButtonVisibilityConverter;
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
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
				return Visibility.Collapsed;
			}
			IL_3F:
			BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
			ControllerTypeEnum? controllerTypeEnum;
			if (currentGamepad == null)
			{
				controllerTypeEnum = null;
			}
			else
			{
				ControllerVM currentController = currentGamepad.CurrentController;
				controllerTypeEnum = ((currentController != null) ? currentController.FirstGamepadType : null);
			}
			ControllerTypeEnum? controllerTypeEnum2 = controllerTypeEnum;
			ControllerTypeEnum? controllerTypeEnum3 = null;
			XBBinding xbbinding = null;
			GamepadButton? gamepadButton = null;
			KeyScanCodeV2 keyScanCodeV = null;
			Drawing drawing = null;
			this.ProcessArrayItem(values[0], ref gamepadButton, ref keyScanCodeV, ref xbbinding, ref controllerTypeEnum3, ref controllerTypeEnum2, ref drawing);
			this.ProcessArrayItem(values[1], ref gamepadButton, ref keyScanCodeV, ref xbbinding, ref controllerTypeEnum3, ref controllerTypeEnum2, ref drawing);
			this.ProcessArrayItem(values[2], ref gamepadButton, ref keyScanCodeV, ref xbbinding, ref controllerTypeEnum3, ref controllerTypeEnum2, ref drawing);
			this.ProcessArrayItem(values[3], ref gamepadButton, ref keyScanCodeV, ref xbbinding, ref controllerTypeEnum3, ref controllerTypeEnum2, ref drawing);
			if (drawing != null)
			{
				GamepadButton? gamepadButton2 = gamepadButton;
				GamepadButton gamepadButton3 = 2001;
				if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)) && (controllerTypeEnum3 == null || !ControllerTypeExtensions.IsAnyAzeron(controllerTypeEnum3.GetValueOrDefault())) && (controllerTypeEnum2 == null || !ControllerTypeExtensions.IsEngineControllerControlPad(controllerTypeEnum2.GetValueOrDefault())) && (gamepadButton == null || !GamepadButtonExtensions.IsAnyPaddle(gamepadButton.GetValueOrDefault())) && (gamepadButton == null || !GamepadButtonExtensions.IsAdditionalStick(gamepadButton.GetValueOrDefault())))
				{
					try
					{
						if (xbbinding != null && (controllerTypeEnum3 != null && ControllerTypeExtensions.IsGamepadWithSonyTouchpad(controllerTypeEnum3.GetValueOrDefault())) && (gamepadButton != null && GamepadButtonExtensions.IsPhysicalTrackPad1Direction(gamepadButton.GetValueOrDefault())))
						{
							BaseXBBindingCollection realCurrentBeingMappedBindingCollection = App.GameProfilesService.RealCurrentBeingMappedBindingCollection;
							if (realCurrentBeingMappedBindingCollection != null && realCurrentBeingMappedBindingCollection.Touchpad1DirectionalGroup.TapValue.IsUnmapped)
							{
								return Visibility.Collapsed;
							}
						}
					}
					catch (Exception)
					{
					}
					if (controllerTypeEnum3 != null && ControllerTypeExtensions.IsGamepadWithSonyTouchpad(controllerTypeEnum3.GetValueOrDefault()) && (gamepadButton != null && GamepadButtonExtensions.IsPhysicalTrackPad1Direction(gamepadButton.GetValueOrDefault())))
					{
						bool flag = false;
						if (xbbinding != null)
						{
							BaseXBBindingCollection hostCollection = xbbinding.HostCollection;
							bool flag2;
							if (((hostCollection != null) ? hostCollection.Touchpad1DirectionalGroup : null) != null)
							{
								Touchpad1DirectionalGroup touchpad1DirectionalGroup = xbbinding.HostCollection.Touchpad1DirectionalGroup;
								if (touchpad1DirectionalGroup != null && touchpad1DirectionalGroup.TouchpadAnalogMode && xbbinding.IsCurrentActivatorVirtualMappingPresent && !xbbinding.IsCurrentActivatorMouseMappingToTrackpadPresent)
								{
									flag2 = !xbbinding.IsCurrentActivatorOverlayRadialMenuToTrackpadPresent;
									goto IL_264;
								}
							}
							flag2 = false;
							IL_264:
							flag = flag2;
						}
						if (flag)
						{
							return Visibility.Visible;
						}
					}
					ControllerTypeEnum? controllerTypeEnum4 = controllerTypeEnum3;
					ControllerTypeEnum controllerTypeEnum5 = 5;
					if (!((controllerTypeEnum4.GetValueOrDefault() == controllerTypeEnum5) & (controllerTypeEnum4 != null)) && gamepadButton != null)
					{
						gamepadButton2 = gamepadButton;
						gamepadButton3 = 183;
						if ((gamepadButton2.GetValueOrDefault() >= gamepadButton3) & (gamepadButton2 != null))
						{
							gamepadButton2 = gamepadButton;
							gamepadButton3 = 212;
							if ((gamepadButton2.GetValueOrDefault() <= gamepadButton3) & (gamepadButton2 != null))
							{
								return Visibility.Hidden;
							}
						}
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
																							if ((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null))
																							{
																								controllerTypeEnum4 = controllerTypeEnum3;
																								controllerTypeEnum5 = 4;
																								if (!((controllerTypeEnum4.GetValueOrDefault() == controllerTypeEnum5) & (controllerTypeEnum4 != null)))
																								{
																									controllerTypeEnum4 = controllerTypeEnum3;
																									controllerTypeEnum5 = 14;
																									if (!((controllerTypeEnum4.GetValueOrDefault() == controllerTypeEnum5) & (controllerTypeEnum4 != null)))
																									{
																										goto IL_651;
																									}
																								}
																								if ((controllerTypeEnum2 != null && ControllerTypeExtensions.IsNintendoSwitch(controllerTypeEnum2.GetValueOrDefault())) || (controllerTypeEnum2 != null && ControllerTypeExtensions.IsAnyEngineGamepad(controllerTypeEnum2.GetValueOrDefault())) || (controllerTypeEnum2 != null && ControllerTypeExtensions.IsSega(controllerTypeEnum2.GetValueOrDefault())) || (controllerTypeEnum2 != null && ControllerTypeExtensions.IsSwitchOnlineN64(controllerTypeEnum2.GetValueOrDefault())) || (controllerTypeEnum2 != null && ControllerTypeExtensions.IsGameSirG7(controllerTypeEnum2.GetValueOrDefault())) || (controllerTypeEnum2 != null && ControllerTypeExtensions.IsFlydigi(controllerTypeEnum2.GetValueOrDefault())) || (controllerTypeEnum2 != null && ControllerTypeExtensions.IsGeneric(controllerTypeEnum2.GetValueOrDefault())) || (controllerTypeEnum2 != null && ControllerTypeExtensions.IsGoogle(controllerTypeEnum2.GetValueOrDefault())))
																								{
																									return Visibility.Collapsed;
																								}
																							}
																							IL_651:
																							controllerTypeEnum4 = controllerTypeEnum3;
																							controllerTypeEnum5 = 1;
																							if (!((controllerTypeEnum4.GetValueOrDefault() == controllerTypeEnum5) & (controllerTypeEnum4 != null)))
																							{
																								controllerTypeEnum4 = controllerTypeEnum3;
																								controllerTypeEnum5 = 2;
																								if (!((controllerTypeEnum4.GetValueOrDefault() == controllerTypeEnum5) & (controllerTypeEnum4 != null)))
																								{
																									controllerTypeEnum4 = controllerTypeEnum3;
																									controllerTypeEnum5 = 5;
																									if (!((controllerTypeEnum4.GetValueOrDefault() == controllerTypeEnum5) & (controllerTypeEnum4 != null)))
																									{
																										goto IL_772;
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
																										goto IL_772;
																									}
																								}
																							}
																							bool flag3 = false;
																							if (gamepadButton != null && GamepadButtonExtensions.IsPhysicalTrackPad1Direction(gamepadButton.GetValueOrDefault()))
																							{
																								flag3 = controllerTypeEnum2 != null && ControllerTypeExtensions.IsSteam(controllerTypeEnum2.GetValueOrDefault());
																							}
																							if (!flag3)
																							{
																								return Visibility.Collapsed;
																							}
																							IL_772:
																							controllerTypeEnum4 = controllerTypeEnum3;
																							controllerTypeEnum5 = 7;
																							if ((controllerTypeEnum4.GetValueOrDefault() == controllerTypeEnum5) & (controllerTypeEnum4 != null))
																							{
																								gamepadButton2 = gamepadButton;
																								gamepadButton3 = 163;
																								if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)))
																								{
																									gamepadButton2 = gamepadButton;
																									gamepadButton3 = 99;
																									if (!((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null)) && (gamepadButton == null || !GamepadButtonExtensions.IsPhysicalTrackPad1Direction(gamepadButton.GetValueOrDefault())) && (gamepadButton == null || !GamepadButtonExtensions.IsAnyVirtualTouchpad(gamepadButton.GetValueOrDefault())) && (gamepadButton == null || !GamepadButtonExtensions.IsAnyTriggerZone(gamepadButton.GetValueOrDefault())))
																									{
																										goto IL_85B;
																									}
																								}
																								bool flag4 = false;
																								if (gamepadButton != null && GamepadButtonExtensions.IsPhysicalTrackPad1Direction(gamepadButton.GetValueOrDefault()))
																								{
																									flag4 = controllerTypeEnum2 != null && ControllerTypeExtensions.IsSteam(controllerTypeEnum2.GetValueOrDefault());
																								}
																								if (!flag4)
																								{
																									return Visibility.Collapsed;
																								}
																							}
																							IL_85B:
																							if (xbbinding != null && xbbinding.ControllerButton.IsVirtualGamepadMappingAllowed && !xbbinding.IsRemapedOrUnmapped && xbbinding.SingleActivator != null && !xbbinding.SingleActivator.IsVirtualMappingPresentSkipRumble)
																							{
																								if (!xbbinding.IsInheritedBinding)
																								{
																									XBBinding shiftXBBinding = xbbinding.SingleActivator.ShiftXBBinding;
																									bool flag5;
																									if (shiftXBBinding == null)
																									{
																										flag5 = false;
																									}
																									else
																									{
																										ActivatorXBBinding singleActivator = shiftXBBinding.SingleActivator;
																										bool? flag6 = ((singleActivator != null) ? new bool?(singleActivator.IsVirtualMappingPresentSkipRumble) : null);
																										bool flag7 = true;
																										flag5 = (flag6.GetValueOrDefault() == flag7) & (flag6 != null);
																									}
																									if (flag5)
																									{
																										goto IL_93C;
																									}
																								}
																								if (!GamepadButtonExtensions.IsGyroTilt(xbbinding.ControllerButton.GamepadButton) || (controllerTypeEnum3 != null && ControllerTypeExtensions.IsVirtualGyroAvailiable(controllerTypeEnum3.GetValueOrDefault()) && this.IsFirstGyroControllerInSubConfig(xbbinding) && (controllerTypeEnum2 == null || !ControllerTypeExtensions.IsFlydigi(controllerTypeEnum2.GetValueOrDefault()))))
																								{
																									return Visibility.Visible;
																								}
																							}
																							IL_93C:
																							return Visibility.Collapsed;
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
					return Visibility.Collapsed;
				}
			}
			return Visibility.Collapsed;
		}

		private bool IsFirstGyroControllerInSubConfig(XBBinding xbBinding)
		{
			int index = xbBinding.HostCollection.SubConfigData.Index;
			if (xbBinding.HostCollection.SubConfigData.Index == 0)
			{
				return true;
			}
			Lazy<IGamepadService> gamepadServiceLazy = App.GamepadServiceLazy;
			object obj;
			if (gamepadServiceLazy == null)
			{
				obj = null;
			}
			else
			{
				IGamepadService value = gamepadServiceLazy.Value;
				obj = ((value != null) ? value.CurrentGamepad : null);
			}
			CompositeControllerVM compositeControllerVM = obj as CompositeControllerVM;
			if (compositeControllerVM != null)
			{
				int num = -1;
				foreach (BaseControllerVM baseControllerVM in compositeControllerVM.BaseControllers)
				{
					if (baseControllerVM != null && baseControllerVM.ControllerFamily == null)
					{
						num++;
						if (num >= index)
						{
							return true;
						}
						ControllerTypeEnum? controllerTypeEnum;
						if (num < index && (baseControllerVM.FirstGamepadType != null && ControllerTypeExtensions.IsGyroAvailiable(controllerTypeEnum.GetValueOrDefault())) && !xbBinding.HostCollection.SubConfigData.ConfigData.FindGamepadCollection(num, false).MainXBBindingCollection.GyroTiltDirectionalGroup.IsUnmapped)
						{
							return false;
						}
					}
				}
				return true;
			}
			return true;
		}

		private void ProcessArrayItem(object item, ref GamepadButton? gamepadButton, ref KeyScanCodeV2 keyScanCode, ref XBBinding xbBinding, ref ControllerTypeEnum? virtualControllerType, ref ControllerTypeEnum? realControllerType, ref Drawing drawing)
		{
			XBBinding xbbinding = item as XBBinding;
			if (xbbinding != null)
			{
				xbBinding = xbbinding;
				gamepadButton = new GamepadButton?(xbBinding.GamepadButton);
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

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static VirtualGamepadButtonVisibilityConverter _converter;
	}
}
