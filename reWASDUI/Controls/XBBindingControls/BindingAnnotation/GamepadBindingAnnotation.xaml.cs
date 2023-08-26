using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View;
using DiscSoft.NET.Common.View.RecolorableImages;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;
using reWASDUI.Infrastructure.KeyBindings.ControllerBindings;
using reWASDUI.Infrastructure.KeyBindings.Mask;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups;
using reWASDUI.Services;
using XBEliteWPF.Infrastructure.KeyBindingsModel.ActivatorXB;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.License.Licensing.ComStructures;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Controls.XBBindingControls.BindingAnnotation
{
	public partial class GamepadBindingAnnotation : BaseGamepadBindingAnnotation
	{
		public GamepadButton? AnnotatedButton
		{
			get
			{
				return (GamepadButton?)base.GetValue(GamepadBindingAnnotation.AnnotatedButtonProperty);
			}
			set
			{
				base.SetValue(GamepadBindingAnnotation.AnnotatedButtonProperty, value);
			}
		}

		public MouseButton? MouseButton
		{
			get
			{
				return (MouseButton?)base.GetValue(GamepadBindingAnnotation.MouseButtonProperty);
			}
			set
			{
				base.SetValue(GamepadBindingAnnotation.MouseButtonProperty, value);
			}
		}

		public AssociatedControllerButton ControllerButton
		{
			get
			{
				return (AssociatedControllerButton)base.GetValue(GamepadBindingAnnotation.ControllerButtonProperty);
			}
			set
			{
				base.SetValue(GamepadBindingAnnotation.ControllerButtonProperty, value);
			}
		}

		private static void SVGElementNameChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			GamepadBindingAnnotation gamepadBindingAnnotation = d as GamepadBindingAnnotation;
			if (gamepadBindingAnnotation == null)
			{
				return;
			}
			gamepadBindingAnnotation.OnSVGElementNameChanged();
		}

		public string SVGAnnotationElementName
		{
			get
			{
				return (string)base.GetValue(GamepadBindingAnnotation.SVGAnnotationElementNameProperty);
			}
			set
			{
				base.SetValue(GamepadBindingAnnotation.SVGAnnotationElementNameProperty, value);
			}
		}

		private static void SVGDrawingImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			GamepadBindingAnnotation gamepadBindingAnnotation = d as GamepadBindingAnnotation;
			if (gamepadBindingAnnotation == null)
			{
				return;
			}
			gamepadBindingAnnotation.ForceRecolorOrHideAnnotation();
		}

		public DrawingImage SVGDrawingImage
		{
			get
			{
				return (DrawingImage)base.GetValue(GamepadBindingAnnotation.SVGDrawingImageProperty);
			}
			set
			{
				base.SetValue(GamepadBindingAnnotation.SVGDrawingImageProperty, value);
			}
		}

		public RecolorableSVG SVGContainer
		{
			get
			{
				return (RecolorableSVG)base.GetValue(GamepadBindingAnnotation.SVGContainerProperty);
			}
			set
			{
				base.SetValue(GamepadBindingAnnotation.SVGContainerProperty, value);
			}
		}

		private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
		}

		public Visibility AnnotationVisibility
		{
			get
			{
				return (Visibility)base.GetValue(GamepadBindingAnnotation.AnnotationVisibilityProperty);
			}
			set
			{
				base.SetValue(GamepadBindingAnnotation.AnnotationVisibilityProperty, value);
			}
		}

		public bool ShowAnnotationStickOnIsCurrentBinding
		{
			get
			{
				return (bool)base.GetValue(GamepadBindingAnnotation.ShowAnnotationStickOnIsCurrentBindingProperty);
			}
			set
			{
				base.SetValue(GamepadBindingAnnotation.ShowAnnotationStickOnIsCurrentBindingProperty, value);
			}
		}

		private static void ShowAnnotationStickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			GamepadBindingAnnotation gamepadBindingAnnotation = d as GamepadBindingAnnotation;
			if (gamepadBindingAnnotation == null)
			{
				return;
			}
			gamepadBindingAnnotation.RecolorOrHideAnnotation();
		}

		public bool ShowAnnotationStick
		{
			get
			{
				return (bool)base.GetValue(GamepadBindingAnnotation.ShowAnnotationStickProperty);
			}
			set
			{
				base.SetValue(GamepadBindingAnnotation.ShowAnnotationStickProperty, value);
			}
		}

		private static void OnShowHardwareMappingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			GamepadBindingAnnotation gamepadBindingAnnotation = d as GamepadBindingAnnotation;
			if (gamepadBindingAnnotation == null)
			{
				return;
			}
			gamepadBindingAnnotation.RecolorOrHideAnnotation();
		}

		public bool ShowHardwareMapping
		{
			get
			{
				return (bool)base.GetValue(GamepadBindingAnnotation.ShowHardwareMappingProperty);
			}
			set
			{
				base.SetValue(GamepadBindingAnnotation.ShowHardwareMappingProperty, value);
			}
		}

		public GamepadBindingAnnotation()
		{
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
		}

		protected override void OnAnnotationRecolorBrushChanged()
		{
			base.OnAnnotationRecolorBrushChanged();
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this._gamepadBindingAnnotationText = base.Template.FindName("GamepadBindingAnnotationText", this) as GamepadBindingAnnotationText;
			this._gamepadMaskAnnotation = base.Template.FindName("GamepadMaskAnnotation", this) as GamepadMaskAnnotation;
			this._gamepadRemapAnnotation = base.Template.FindName("GamepadRemapAnnotation", this) as GamepadRemapAnnotation;
			this._gamepadFakeGlobalVirtualAnnotation = base.Template.FindName("GamepadFakeGlobalVirtualAnnotation", this) as GamepadFakeGlobalVirtualAnnotation;
			if (this._gamepadBindingAnnotationText != null)
			{
				this._gamepadBindingAnnotationText.IsVisibleChanged += delegate(object s, DependencyPropertyChangedEventArgs e)
				{
					this.ReEvaluateVisibility();
				};
			}
			if (this._gamepadMaskAnnotation != null)
			{
				this._gamepadMaskAnnotation.IsVisibleChanged += delegate(object s, DependencyPropertyChangedEventArgs e)
				{
					this.ReEvaluateVisibility();
				};
			}
			if (this._gamepadRemapAnnotation != null)
			{
				this._gamepadRemapAnnotation.IsVisibleChanged += delegate(object s, DependencyPropertyChangedEventArgs e)
				{
					this.ReEvaluateVisibility();
				};
			}
			if (this._gamepadFakeGlobalVirtualAnnotation != null)
			{
				this._gamepadFakeGlobalVirtualAnnotation.IsVisibleChanged += delegate(object s, DependencyPropertyChangedEventArgs e)
				{
					this.ReEvaluateVisibility();
				};
			}
			this.ReEvaluateVisibility();
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			if (this.isFirstLoadInited)
			{
				return;
			}
			this.isFirstLoadInited = true;
			if (this.SVGContainer != null)
			{
				this.SVGContainer.OnSVGRendered += delegate([Nullable(2)] object o, EventArgs args)
				{
					this.RecolorOrHideAnnotation();
				};
			}
			base.EventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM o)
			{
				this.RecolorOrHideAnnotation();
			});
			base.EventAggregator.GetEvent<RequestBindingFrameBack>().Subscribe(delegate(string o)
			{
				this.ReEvaluateVisibility();
			});
			App.LicensingService.OnLicenseChangedCompleted += delegate(LicenseCheckResult s, bool e)
			{
				this.RecolorOrHideAnnotation();
			};
		}

		protected override void ReEvaluateVisibilityForGroup(BaseDirectionalGroup directionalGroup)
		{
			if (base.XBBinding == null || base.CurrentAnnotatedXBBindingCollection == null)
			{
				this.AnnotationVisibility = Visibility.Collapsed;
				return;
			}
			base.ReEvaluateVisibilityForGroup(directionalGroup);
		}

		private void OnSVGElementNameChanged()
		{
			this.RecolorOrHideAnnotation();
		}

		private void ForceRecolorOrHideAnnotation()
		{
			this._prevName = "";
			this.RecolorOrHideAnnotation();
		}

		private void RecolorOrHideAnnotation()
		{
			if (!base.IsSameCurrentSubConfig())
			{
				return;
			}
			if (this.AnnotationVisibility == Visibility.Visible && this.ShowAnnotationStick)
			{
				this.RecolorAnnotation();
				return;
			}
			this.HideAnnotation();
		}

		private SolidColorBrush GetAnnotationColor()
		{
			GameProfilesService gameProfilesService = base.GameProfilesService;
			if (((gameProfilesService != null) ? gameProfilesService.RealCurrentBeingMappedBindingCollection : null) == null)
			{
				return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
			}
			SolidColorBrush solidColorBrush = base.GameProfilesService.RealCurrentBeingMappedBindingCollection.CollectionBrush;
			solidColorBrush = new SolidColorBrush(Color.FromRgb(solidColorBrush.Color.R / 2, solidColorBrush.Color.G / 2, solidColorBrush.Color.B / 2));
			if (base.XBBinding != null)
			{
				if (base.XBBinding.IsInheritedBinding)
				{
					solidColorBrush = Application.Current.TryFindResource("DisabledControlForegroundColor") as SolidColorBrush;
				}
				else
				{
					if (base.GameProfilesService.RealCurrentBeingMappedBindingCollection.IsMainCollection && !base.XBBinding.ActivatorXBBindingDictionary.IsAnyNonSingleActivatorHasMapping)
					{
						GamepadMaskAnnotation gamepadMaskAnnotation = this._gamepadMaskAnnotation;
						if (gamepadMaskAnnotation == null || gamepadMaskAnnotation.Visibility > Visibility.Visible)
						{
							GamepadFakeGlobalVirtualAnnotation gamepadFakeGlobalVirtualAnnotation = this._gamepadFakeGlobalVirtualAnnotation;
							if (gamepadFakeGlobalVirtualAnnotation != null && gamepadFakeGlobalVirtualAnnotation.Visibility == Visibility.Visible)
							{
								return new SolidColorBrush(Color.FromRgb(127, 124, 111));
							}
						}
					}
					if (base.XBBinding.IsMouseDirection)
					{
						if ((!App.LicensingService.IsAdvancedMappingFeatureUnlocked && (!App.GameProfilesService.CurrentGame.CurrentConfig.CurrentSubConfigData.IsMouse || base.XBBinding.IsMouseDirection) && App.GameProfilesService.CurrentGame.CurrentConfig.CurrentSubConfigData.IsMouse) || !App.GameProfilesService.CurrentGame.CurrentConfig.IsEditConfigMode)
						{
							solidColorBrush = solidColorBrush.Clone();
							solidColorBrush.Opacity = 0.5;
						}
						if (base.XBBinding.IsRemapedOrUnmapped && !App.LicensingService.IsAdvancedMappingFeatureUnlocked && (!this.IsAdvancedMappingFeatureNotRequired || !base.XBBinding.IsStickDirection || base.XBBinding.IsUnmapped) && (!base.XBBinding.IsUnmapped || !App.GameProfilesService.CurrentGame.CurrentConfig.CurrentSubConfigData.IsMouse || base.XBBinding.IsMouseStick) && (!this.IsAdvancedMappingFeatureNotRequired || base.XBBinding.IsStickDirection || base.XBBinding.IsMouseStick) && (!this.IsAdvancedMappingFeatureNotRequiredForUnmap || !base.XBBinding.IsUnmapped || App.GameProfilesService.CurrentGame.CurrentConfig.CurrentSubConfigData.IsMouse) && (!this.IsAdvancedMappingFeatureNotRequiredForUnmap || !App.GameProfilesService.CurrentGame.CurrentConfig.CurrentSubConfigData.IsMouse || base.XBBinding.IsMouseStick || !App.GameProfilesService.CurrentGame.CurrentConfig.IsEditConfigMode))
						{
							solidColorBrush = solidColorBrush.Clone();
							solidColorBrush.Opacity = 0.5;
						}
					}
				}
			}
			return solidColorBrush;
		}

		public bool IsAdvancedMappingFeatureNotRequired
		{
			get
			{
				BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
				bool flag;
				if (currentGamepad == null)
				{
					flag = false;
				}
				else
				{
					ControllerVM currentController = currentGamepad.CurrentController;
					ControllerTypeEnum? controllerTypeEnum = ((currentController != null) ? new ControllerTypeEnum?(currentController.ControllerType) : null);
					ControllerTypeEnum controllerTypeEnum2 = 16;
					flag = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
				}
				if (!flag)
				{
					BaseControllerVM currentGamepad2 = App.GamepadService.CurrentGamepad;
					bool flag2;
					if (currentGamepad2 == null)
					{
						flag2 = false;
					}
					else
					{
						ControllerVM currentController2 = currentGamepad2.CurrentController;
						ControllerTypeEnum? controllerTypeEnum = ((currentController2 != null) ? new ControllerTypeEnum?(currentController2.ControllerType) : null);
						ControllerTypeEnum controllerTypeEnum2 = 63;
						flag2 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
					}
					if (!flag2)
					{
						BaseControllerVM currentGamepad3 = App.GamepadService.CurrentGamepad;
						bool flag3;
						if (currentGamepad3 == null)
						{
							flag3 = false;
						}
						else
						{
							ControllerVM currentController3 = currentGamepad3.CurrentController;
							ControllerTypeEnum? controllerTypeEnum = ((currentController3 != null) ? new ControllerTypeEnum?(currentController3.ControllerType) : null);
							ControllerTypeEnum controllerTypeEnum2 = 2;
							flag3 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
						}
						if (!flag3)
						{
							BaseControllerVM currentGamepad4 = App.GamepadService.CurrentGamepad;
							bool flag4;
							if (currentGamepad4 == null)
							{
								flag4 = false;
							}
							else
							{
								ControllerVM currentController4 = currentGamepad4.CurrentController;
								ControllerTypeEnum? controllerTypeEnum = ((currentController4 != null) ? new ControllerTypeEnum?(currentController4.ControllerType) : null);
								ControllerTypeEnum controllerTypeEnum2 = 22;
								flag4 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
							}
							if (!flag4)
							{
								BaseControllerVM currentGamepad5 = App.GamepadService.CurrentGamepad;
								bool flag5;
								if (currentGamepad5 == null)
								{
									flag5 = false;
								}
								else
								{
									ControllerVM currentController5 = currentGamepad5.CurrentController;
									ControllerTypeEnum? controllerTypeEnum = ((currentController5 != null) ? new ControllerTypeEnum?(currentController5.ControllerType) : null);
									ControllerTypeEnum controllerTypeEnum2 = 3;
									flag5 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
								}
								if (!flag5)
								{
									BaseControllerVM currentGamepad6 = App.GamepadService.CurrentGamepad;
									if (currentGamepad6 == null)
									{
										return false;
									}
									ControllerVM currentController6 = currentGamepad6.CurrentController;
									ControllerTypeEnum? controllerTypeEnum = ((currentController6 != null) ? new ControllerTypeEnum?(currentController6.ControllerType) : null);
									ControllerTypeEnum controllerTypeEnum2 = 12;
									return (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
								}
							}
						}
					}
				}
				return true;
			}
		}

		public bool IsAdvancedMappingFeatureNotRequiredForUnmap
		{
			get
			{
				BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
				bool flag;
				if (currentGamepad == null)
				{
					flag = false;
				}
				else
				{
					ControllerVM currentController = currentGamepad.CurrentController;
					ControllerTypeEnum? controllerTypeEnum = ((currentController != null) ? new ControllerTypeEnum?(currentController.ControllerType) : null);
					ControllerTypeEnum controllerTypeEnum2 = 16;
					flag = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
				}
				if (!flag)
				{
					BaseControllerVM currentGamepad2 = App.GamepadService.CurrentGamepad;
					bool flag2;
					if (currentGamepad2 == null)
					{
						flag2 = false;
					}
					else
					{
						ControllerVM currentController2 = currentGamepad2.CurrentController;
						ControllerTypeEnum? controllerTypeEnum = ((currentController2 != null) ? new ControllerTypeEnum?(currentController2.ControllerType) : null);
						ControllerTypeEnum controllerTypeEnum2 = 63;
						flag2 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
					}
					if (!flag2)
					{
						BaseControllerVM currentGamepad3 = App.GamepadService.CurrentGamepad;
						bool flag3;
						if (currentGamepad3 == null)
						{
							flag3 = false;
						}
						else
						{
							ControllerVM currentController3 = currentGamepad3.CurrentController;
							ControllerTypeEnum? controllerTypeEnum = ((currentController3 != null) ? new ControllerTypeEnum?(currentController3.ControllerType) : null);
							ControllerTypeEnum controllerTypeEnum2 = 19;
							flag3 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
						}
						if (!flag3)
						{
							BaseControllerVM currentGamepad4 = App.GamepadService.CurrentGamepad;
							bool flag4;
							if (currentGamepad4 == null)
							{
								flag4 = false;
							}
							else
							{
								ControllerVM currentController4 = currentGamepad4.CurrentController;
								ControllerTypeEnum? controllerTypeEnum = ((currentController4 != null) ? new ControllerTypeEnum?(currentController4.ControllerType) : null);
								ControllerTypeEnum controllerTypeEnum2 = 28;
								flag4 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
							}
							if (!flag4)
							{
								BaseControllerVM currentGamepad5 = App.GamepadService.CurrentGamepad;
								bool flag5;
								if (currentGamepad5 == null)
								{
									flag5 = false;
								}
								else
								{
									ControllerVM currentController5 = currentGamepad5.CurrentController;
									ControllerTypeEnum? controllerTypeEnum = ((currentController5 != null) ? new ControllerTypeEnum?(currentController5.ControllerType) : null);
									ControllerTypeEnum controllerTypeEnum2 = 54;
									flag5 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
								}
								if (!flag5)
								{
									BaseControllerVM currentGamepad6 = App.GamepadService.CurrentGamepad;
									bool flag6;
									if (currentGamepad6 == null)
									{
										flag6 = false;
									}
									else
									{
										ControllerVM currentController6 = currentGamepad6.CurrentController;
										ControllerTypeEnum? controllerTypeEnum = ((currentController6 != null) ? new ControllerTypeEnum?(currentController6.ControllerType) : null);
										ControllerTypeEnum controllerTypeEnum2 = 37;
										flag6 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
									}
									if (!flag6)
									{
										BaseControllerVM currentGamepad7 = App.GamepadService.CurrentGamepad;
										bool flag7;
										if (currentGamepad7 == null)
										{
											flag7 = false;
										}
										else
										{
											ControllerVM currentController7 = currentGamepad7.CurrentController;
											ControllerTypeEnum? controllerTypeEnum = ((currentController7 != null) ? new ControllerTypeEnum?(currentController7.ControllerType) : null);
											ControllerTypeEnum controllerTypeEnum2 = 38;
											flag7 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
										}
										if (!flag7)
										{
											BaseControllerVM currentGamepad8 = App.GamepadService.CurrentGamepad;
											bool flag8;
											if (currentGamepad8 == null)
											{
												flag8 = false;
											}
											else
											{
												ControllerVM currentController8 = currentGamepad8.CurrentController;
												ControllerTypeEnum? controllerTypeEnum = ((currentController8 != null) ? new ControllerTypeEnum?(currentController8.ControllerType) : null);
												ControllerTypeEnum controllerTypeEnum2 = 39;
												flag8 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
											}
											if (!flag8)
											{
												BaseControllerVM currentGamepad9 = App.GamepadService.CurrentGamepad;
												bool flag9;
												if (currentGamepad9 == null)
												{
													flag9 = false;
												}
												else
												{
													ControllerVM currentController9 = currentGamepad9.CurrentController;
													ControllerTypeEnum? controllerTypeEnum = ((currentController9 != null) ? new ControllerTypeEnum?(currentController9.ControllerType) : null);
													ControllerTypeEnum controllerTypeEnum2 = 40;
													flag9 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
												}
												if (!flag9)
												{
													BaseControllerVM currentGamepad10 = App.GamepadService.CurrentGamepad;
													if (currentGamepad10 == null)
													{
														return false;
													}
													ControllerVM currentController10 = currentGamepad10.CurrentController;
													ControllerTypeEnum? controllerTypeEnum = ((currentController10 != null) ? new ControllerTypeEnum?(currentController10.ControllerType) : null);
													ControllerTypeEnum controllerTypeEnum2 = 62;
													return (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
												}
											}
										}
									}
								}
							}
						}
					}
				}
				return true;
			}
		}

		private void RecolorAnnotation()
		{
			if (this.SVGDrawingImage == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.SVGAnnotationElementName))
			{
				SVGAnchorContainer svganchorContainer = base.Parent as SVGAnchorContainer;
				if (svganchorContainer != null)
				{
					if (svganchorContainer.SVGElementName == null)
					{
						return;
					}
					this.SVGAnnotationElementName = svganchorContainer.SVGElementName + "Annotation";
				}
			}
			this.RecolorAnnotation(this.SVGAnnotationElementName, this.GetAnnotationColor());
		}

		private void HideAnnotation()
		{
			if (this.SVGDrawingImage == null || string.IsNullOrEmpty(this.SVGAnnotationElementName))
			{
				return;
			}
			this.RecolorAnnotation(this.SVGAnnotationElementName, new SolidColorBrush(Colors.Transparent));
		}

		private void RecolorAnnotation(string name, SolidColorBrush brush)
		{
			if (brush == null)
			{
				brush = new SolidColorBrush(Colors.Transparent);
			}
			if (this._prevName != name || !brush.Color.Equals(this._prevColor))
			{
				Drawing itemByName = this.SVGDrawingImage.GetItemByName(name);
				if (itemByName != null)
				{
					this._prevName = name;
					this._prevColor = brush.Color;
					itemByName.ChangeColor(brush, false, true, true);
				}
			}
		}

		protected override void ReEvaluateVisibility()
		{
			if (!base.IsSameCurrentSubConfig())
			{
				return;
			}
			Visibility visibility = Visibility.Visible;
			AssociatedControllerButton controllerButton = this.ControllerButton;
			if (controllerButton != null && controllerButton.ControllerBindingFrameMode != null)
			{
				this.AnnotationVisibility = (((base.IsCurrentBinding && this.ShowAnnotationStickOnIsCurrentBinding) || !base.IsHiddenIfNotMapped) ? Visibility.Visible : Visibility.Collapsed);
			}
			else
			{
				if (base.XBBinding == null || base.CurrentAnnotatedXBBindingCollection == null)
				{
					this.AnnotationVisibility = Visibility.Collapsed;
					this.RecolorOrHideAnnotation();
					return;
				}
				bool flag = false;
				bool flag2 = false;
				if (base.CurrentAnnotatedXBBindingCollection.IsLabelModeView && base.XBBinding.IsAnnotationShouldBeShownForDescription)
				{
					flag = true;
				}
				if (base.CurrentAnnotatedXBBindingCollection.IsShowMappingsView)
				{
					if (base.XBBinding.IsAnnotationShouldBeShownForMapping)
					{
						flag = true;
						if (GamepadButtonExtensions.IsPhysicalTrackPad1Direction(base.XBBinding.GamepadButton))
						{
							BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
							object obj;
							if (currentGamepad == null)
							{
								obj = 0;
							}
							else
							{
								ControllerVM currentController = currentGamepad.CurrentController;
								bool? flag3 = ((currentController != null) ? new bool?(ControllerTypeExtensions.IsGamepadWithSonyTouchpad(currentController.ControllerType)) : null);
								bool flag4 = true;
								obj = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
							}
							object obj2 = obj;
							if (obj2 != null)
							{
								BaseXBBindingCollection hostCollection = base.XBBinding.HostCollection;
								if (((hostCollection != null) ? hostCollection.Touchpad1DirectionalGroup : null) != null)
								{
									Touchpad1DirectionalGroup touchpad1DirectionalGroup = base.XBBinding.HostCollection.Touchpad1DirectionalGroup;
									if (touchpad1DirectionalGroup != null && touchpad1DirectionalGroup.TouchpadAnalogMode && base.XBBinding.IsCurrentActivatorVirtualMappingPresent && !base.XBBinding.IsCurrentActivatorMouseMappingToTrackpadPresent && !base.XBBinding.IsCurrentActivatorOverlayRadialMenuToTrackpadPresent)
									{
										visibility = Visibility.Collapsed;
										flag = false;
									}
								}
							}
							if (obj2 != null)
							{
								BaseXBBindingCollection hostCollection2 = base.XBBinding.HostCollection;
								if (((hostCollection2 != null) ? hostCollection2.Touchpad1DirectionalGroup : null) != null && base.XBBinding.IsCurrentActivatorVirtualMappingPresent && base.XBBinding.IsCurrentActivatorFlickStickMappingToTrackpadPresent)
								{
									visibility = Visibility.Collapsed;
									flag = false;
								}
							}
						}
					}
					ControllerFamily controllerFamily = base.CurrentAnnotatedXBBindingCollection.SubConfigData.ControllerFamily;
					if (controllerFamily == null && GamepadButtonExtensions.IsDPAD(base.XBBinding.GamepadButton))
					{
						flag2 = base.CurrentAnnotatedXBBindingCollection.DPADDirectionalGroup.IsAnyDirectionVirtualMappingPresent || base.CurrentAnnotatedXBBindingCollection.DPADDirectionalGroup.IsAnyDirectionRemapedOrUnmappedShouldBeShown;
					}
					else if (controllerFamily == null && GamepadButtonExtensions.IsGyroTiltDirection(base.XBBinding.GamepadButton))
					{
						if (flag)
						{
							flag = base.CurrentAnnotatedXBBindingCollection.GyroTiltDirectionalGroup.IsButtonMappingVisible;
							base.Visibility = (flag ? Visibility.Visible : Visibility.Hidden);
						}
						flag2 = base.CurrentAnnotatedXBBindingCollection.GyroTiltDirectionalGroup.IsButtonMappingVisible && (base.CurrentAnnotatedXBBindingCollection.GyroTiltDirectionalGroup.IsAnyDirectionVirtualMappingPresent || base.CurrentAnnotatedXBBindingCollection.GyroTiltDirectionalGroup.IsAnyDirectionRemapedOrUnmappedShouldBeShown);
					}
					else if (controllerFamily == null && GamepadButtonExtensions.IsLeftStick(base.XBBinding.GamepadButton) && !GamepadButtonExtensions.IsLeftStickZone(base.XBBinding.GamepadButton))
					{
						flag2 = base.CurrentAnnotatedXBBindingCollection.LeftStickDirectionalGroup.IsAnyDirectionVirtualMappingPresent || base.CurrentAnnotatedXBBindingCollection.LeftStickDirectionalGroup.IsAnyDirectionRemapedOrUnmappedShouldBeShown;
					}
					else if (controllerFamily == null && GamepadButtonExtensions.IsRightStick(base.XBBinding.GamepadButton) && !GamepadButtonExtensions.IsRightStickZone(base.XBBinding.GamepadButton))
					{
						flag2 = base.CurrentAnnotatedXBBindingCollection.RightStickDirectionalGroup.IsAnyDirectionVirtualMappingPresent || base.CurrentAnnotatedXBBindingCollection.RightStickDirectionalGroup.IsAnyDirectionRemapedOrUnmappedShouldBeShown;
					}
					else if (controllerFamily == 2 && GamepadButtonExtensions.IsMouseDirection(base.XBBinding.GamepadButton))
					{
						flag2 = base.CurrentAnnotatedXBBindingCollection.MouseDirectionalGroup.IsAnyDirectionVirtualMappingPresent || base.CurrentAnnotatedXBBindingCollection.MouseDirectionalGroup.IsAnyDirectionRemapedOrUnmappedShouldBeShown;
					}
					ActivatorXBBinding currentActivatorXBBinding = base.XBBinding.CurrentActivatorXBBinding;
					bool flag5;
					if (currentActivatorXBBinding == null)
					{
						flag5 = false;
					}
					else
					{
						SortableObservableCollection<BaseMacroAnnotation> macroSequenceAnnotation = currentActivatorXBBinding.MacroSequenceAnnotation;
						int? num = ((macroSequenceAnnotation != null) ? new int?(macroSequenceAnnotation.Count) : null);
						int num2 = 0;
						flag5 = (num.GetValueOrDefault() > num2) & (num != null);
					}
					if (flag5)
					{
						flag = true;
					}
					MaskItemCollection maskBindingCollection = base.GameProfilesService.RealCurrentBeingMappedBindingCollection.MaskBindingCollection;
					if (maskBindingCollection != null && maskBindingCollection.IsShouldBeShownForControllerButton(base.XBBinding.ControllerButton, base.XBBinding.HostCollection.SubConfigData.IndexByControllerFamily))
					{
						flag = true;
					}
					GamepadFakeGlobalVirtualAnnotation gamepadFakeGlobalVirtualAnnotation = this._gamepadFakeGlobalVirtualAnnotation;
					if (gamepadFakeGlobalVirtualAnnotation != null && gamepadFakeGlobalVirtualAnnotation.Visibility == Visibility.Visible)
					{
						flag = true;
					}
					GamepadRemapAnnotation gamepadRemapAnnotation = this._gamepadRemapAnnotation;
					if (gamepadRemapAnnotation != null && gamepadRemapAnnotation.Visibility == Visibility.Visible)
					{
						flag = true;
					}
				}
				this.AnnotationVisibility = (((base.IsCurrentBinding && this.ShowAnnotationStickOnIsCurrentBinding) || !base.IsHiddenIfNotMapped || flag || flag2) ? Visibility.Visible : Visibility.Collapsed);
				if (this._gamepadBindingAnnotationText != null)
				{
					this._gamepadBindingAnnotationText.Visibility = visibility;
				}
			}
			this.RecolorOrHideAnnotation();
		}

		private XBBinding XBBindingBeforeInheritance
		{
			get
			{
				return this._xbBindingBeforeInheritance;
			}
			set
			{
				if (value == null)
				{
					if (this._xbBindingBeforeInheritance != null)
					{
						this._xbBindingBeforeInheritance.PropertyChanged -= base.OnXBBindingValueChanged;
						this._xbBindingBeforeInheritance.PropertyChangedExtended -= new PropertyChangedExtendedEventHandler(base.OnXBBindingValueChanged);
					}
				}
				else
				{
					value.PropertyChanged += base.OnXBBindingValueChanged;
					value.PropertyChangedExtended += new PropertyChangedExtendedEventHandler(base.OnXBBindingValueChanged);
				}
				this._xbBindingBeforeInheritance = value;
			}
		}

		private void CorrectAdaptiveTriggers()
		{
			if (!GamepadButtonExtensions.IsAnyTriggerPress(base.XBBinding.GamepadButton))
			{
				return;
			}
			if (this.XBBindingBeforeInheritance != null)
			{
				if (base.XBBinding != this.XBBindingBeforeInheritance && this.XBBindingBeforeInheritance != null)
				{
					this.XBBindingBeforeInheritance.HostCollection.UpdateAdapterTriggersMappings();
					base.XBBinding.ActivatorXBBindingDictionary.SingleActivator.IsAdaptiveTriggers = this.XBBindingBeforeInheritance.ActivatorXBBindingDictionary.SingleActivator.IsAdaptiveTriggers;
					base.XBBinding.ActivatorXBBindingDictionary.SingleActivator.IsAdaptiveTriggersInherited = this.XBBindingBeforeInheritance.ActivatorXBBindingDictionary.SingleActivator.IsAdaptiveTriggersInherited;
					return;
				}
			}
			else
			{
				base.XBBinding.HostCollection.UpdateAdapterTriggersMappings();
			}
		}

		protected override void ReEvaluateXBBinding()
		{
			if (!base.IsSameCurrentSubConfig())
			{
				return;
			}
			if (base.CurrentAnnotatedXBBindingCollection == null)
			{
				if (base.XBBinding != null)
				{
					base.XBBinding.IsInheritedBinding = false;
				}
				base.XBBinding = null;
				this.XBBindingBeforeInheritance = null;
				return;
			}
			bool flag = false;
			if (base.XBBinding != null)
			{
				flag = base.XBBinding.IsInheritedBinding;
				base.XBBinding.IsInheritedBinding = false;
				this.XBBindingBeforeInheritance = null;
			}
			XBBinding xbbinding;
			if (this.ControllerButton != null)
			{
				xbbinding = base.CurrentAnnotatedXBBindingCollection.GetXBBindingByAssociatedControllerButton(this.ControllerButton);
			}
			else if (this.AnnotatedButton != null)
			{
				xbbinding = base.CurrentAnnotatedXBBindingCollection.GetXBBindingByGamepadButtonForGui(this.AnnotatedButton.Value);
			}
			else
			{
				if (this.MouseButton == null)
				{
					if (base.XBBinding != null)
					{
						base.XBBinding.IsInheritedBinding = flag;
					}
					return;
				}
				xbbinding = base.CurrentAnnotatedXBBindingCollection.GetXBBindingByMouseButton(this.MouseButton.Value);
			}
			if (xbbinding == null)
			{
				base.XBBinding = null;
				return;
			}
			xbbinding.ActivatorXBBindingDictionary.ForEachValue(delegate(ActivatorXBBinding kvp)
			{
				kvp.RefreshAnnotations();
			});
			if (base.CurrentAnnotatedXBBindingCollection.IsShiftCollection && string.IsNullOrWhiteSpace(xbbinding.CurrentActivatorXBBinding.Description) && !xbbinding.IsDisableInheritVirtualMapFromMain && !xbbinding.IsAnyActivatorVirtualMappingPresent && !xbbinding.IsRemapedOrUnmapped && !base.GameProfilesService.RealCurrentBeingMappedBindingCollection.HasMaskForButton(xbbinding.GamepadButton))
			{
				ShiftXBBindingCollection shiftXBBindingCollection = xbbinding.HostCollection as ShiftXBBindingCollection;
				if (shiftXBBindingCollection != null)
				{
					BaseTouchpadDirectionalGroup baseTouchpadDirectionalGroup = shiftXBBindingCollection.GetDirectionalGroupByXBBinding(xbbinding) as BaseTouchpadDirectionalGroup;
					if (baseTouchpadDirectionalGroup != null && baseTouchpadDirectionalGroup.IsDigitalMode != ((BaseTouchpadDirectionalGroup)shiftXBBindingCollection.ParentBindingCollection.GetDirectionalGroupByXBBinding(xbbinding)).IsDigitalMode)
					{
						goto IL_2A3;
					}
				}
				this.XBBindingBeforeInheritance = xbbinding;
				if (this.ControllerButton != null)
				{
					xbbinding = ((ShiftXBBindingCollection)base.CurrentAnnotatedXBBindingCollection).ParentBindingCollection.GetXBBindingByAssociatedControllerButton(this.ControllerButton);
				}
				else if (this.AnnotatedButton != null)
				{
					xbbinding = ((ShiftXBBindingCollection)base.CurrentAnnotatedXBBindingCollection).ParentBindingCollection.GetXBBindingByGamepadButton(this.AnnotatedButton.Value);
				}
				else if (this.MouseButton != null)
				{
					ControllerBindingsCollection controllerBindings = ((ShiftXBBindingCollection)base.CurrentAnnotatedXBBindingCollection).ParentBindingCollection.ControllerBindings;
					bool createOnDemand = controllerBindings.CreateOnDemand;
					controllerBindings.CreateOnDemand = true;
					xbbinding = ((ShiftXBBindingCollection)base.CurrentAnnotatedXBBindingCollection).ParentBindingCollection.GetXBBindingByMouseButton(this.MouseButton.Value);
					controllerBindings.CreateOnDemand = createOnDemand;
				}
				xbbinding.IsInheritedBinding = true;
				xbbinding.CurrentActivatorXBBinding = xbbinding.ActivatorXBBindingDictionary.GetNonEmptyActivator();
			}
			IL_2A3:
			base.XBBinding = xbbinding;
			this.CorrectAdaptiveTriggers();
		}

		protected override void RefreshVisibilityBindingAndIsCurrentBinding()
		{
			this.ReEvaluateXBBinding();
			this.ReEvaluateIsCurrentBinding();
		}

		protected override void ReEvaluateIsCurrentBinding()
		{
			if (base.GameProfilesService == null)
			{
				return;
			}
			BaseXBBindingCollection currentAnnotatedXBBindingCollection = base.CurrentAnnotatedXBBindingCollection;
			ShiftXBBindingCollection shiftXBBindingCollection = base.CurrentAnnotatedXBBindingCollection as ShiftXBBindingCollection;
			if (shiftXBBindingCollection != null)
			{
				MainXBBindingCollection parentBindingCollection = shiftXBBindingCollection.ParentBindingCollection;
			}
			if (this.ControllerButton != null)
			{
				base.IsCurrentBinding = object.Equals(this.ControllerButton, base.CurrentControllerButton);
			}
			else if (this.AnnotatedButton != null)
			{
				XBBinding currentXBBinding = base.CurrentXBBinding;
				GamepadButton? gamepadButton = ((currentXBBinding != null) ? new GamepadButton?(currentXBBinding.GamepadButton) : null);
				GamepadButton? annotatedButton = this.AnnotatedButton;
				base.IsCurrentBinding = (gamepadButton.GetValueOrDefault() == annotatedButton.GetValueOrDefault()) & (gamepadButton != null == (annotatedButton != null));
			}
			else if (this.MouseButton != null)
			{
				XBBinding currentXBBinding2 = base.CurrentXBBinding;
				base.IsCurrentBinding = ((currentXBBinding2 != null) ? currentXBBinding2.KeyScanCode : null) == KeyScanCodeV2.FindKeyScanCodeByMouseButton(this.MouseButton.Value);
			}
			else
			{
				base.IsCurrentBinding = false;
			}
			this.ReEvaluateVisibility();
		}

		public static readonly DependencyProperty AnnotatedButtonProperty = DependencyProperty.Register("AnnotatedButton", typeof(GamepadButton?), typeof(GamepadBindingAnnotation), new PropertyMetadata(null, new PropertyChangedCallback(BaseXBBindingAnnotation.XBButtonToBindChangedCallback)));

		public static readonly DependencyProperty MouseButtonProperty = DependencyProperty.Register("MouseButton", typeof(MouseButton?), typeof(GamepadBindingAnnotation), new PropertyMetadata(null, new PropertyChangedCallback(BaseXBBindingAnnotation.XBButtonToBindChangedCallback)));

		public static readonly DependencyProperty ControllerButtonProperty = DependencyProperty.Register("ControllerButton", typeof(AssociatedControllerButton), typeof(GamepadBindingAnnotation), new PropertyMetadata(null, new PropertyChangedCallback(BaseXBBindingAnnotation.XBButtonToBindChangedCallback)));

		public static readonly DependencyProperty SVGAnnotationElementNameProperty = DependencyProperty.Register("SVGAnnotationElementName", typeof(string), typeof(GamepadBindingAnnotation), new PropertyMetadata(null, new PropertyChangedCallback(GamepadBindingAnnotation.SVGElementNameChangedCallback)));

		public static readonly DependencyProperty SVGDrawingImageProperty = DependencyProperty.Register("SVGDrawingImage", typeof(DrawingImage), typeof(GamepadBindingAnnotation), new PropertyMetadata(null, new PropertyChangedCallback(GamepadBindingAnnotation.SVGDrawingImageChanged)));

		public static readonly DependencyProperty SVGContainerProperty = DependencyProperty.Register("SVGContainer", typeof(RecolorableSVG), typeof(GamepadBindingAnnotation), new PropertyMetadata(null));

		public static readonly DependencyProperty AnnotationVisibilityProperty = DependencyProperty.Register("AnnotationVisibility", typeof(Visibility), typeof(GamepadBindingAnnotation), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(GamepadBindingAnnotation.PropertyChangedCallback)));

		public static readonly DependencyProperty ShowAnnotationStickOnIsCurrentBindingProperty = DependencyProperty.Register("ShowAnnotationStickOnIsCurrentBinding", typeof(bool), typeof(GamepadBindingAnnotation), new PropertyMetadata(true));

		public static readonly DependencyProperty ShowAnnotationStickProperty = DependencyProperty.Register("ShowAnnotationStick", typeof(bool), typeof(GamepadBindingAnnotation), new PropertyMetadata(true, new PropertyChangedCallback(GamepadBindingAnnotation.ShowAnnotationStickChanged)));

		public static readonly DependencyProperty ShowHardwareMappingProperty = DependencyProperty.Register("ShowHardwareMapping", typeof(bool), typeof(GamepadBindingAnnotation), new PropertyMetadata(true, new PropertyChangedCallback(GamepadBindingAnnotation.OnShowHardwareMappingChanged)));

		private GamepadBindingAnnotationText _gamepadBindingAnnotationText;

		private GamepadMaskAnnotation _gamepadMaskAnnotation;

		private GamepadRemapAnnotation _gamepadRemapAnnotation;

		private GamepadFakeGlobalVirtualAnnotation _gamepadFakeGlobalVirtualAnnotation;

		private bool isFirstLoadInited;

		private string _prevName;

		private Color _prevColor;

		private XBBinding _xbBindingBeforeInheritance;
	}
}
