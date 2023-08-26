using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.Controls;
using RadialMenu.Controls;
using reWASDUI.Controls.XBBindingControls.BindingFrame;
using reWASDUI.Controls.XBBindingControls.BindingFrame.BindingFrameViews;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.ViewModels;
using reWASDUI.Views.ContentZoneGamepad.AdvancedStick;
using reWASDUI.Views.ContentZoneGamepad.GamepadSelector;
using reWASDUI.Views.ContentZoneGamepad.Macro;
using reWASDUI.Views.OverlayMenu;

namespace reWASDUI.Views
{
	public partial class MainContent : UserControl
	{
		private MainContentVM _datacontext
		{
			get
			{
				return base.DataContext as MainContentVM;
			}
		}

		public MainContent()
		{
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			if (this._isFirstShow)
			{
				this._isFirstShow = false;
				this._datacontext.OnNavigatedTo(null);
			}
		}

		private void MainContent_OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			try
			{
				UserControl userControl = this._datacontext.RegionManager.Regions[RegionNames.Gamepad].ActiveViews.FirstOrDefault<object>() as UserControl;
				BaseXBBindingCollection realCurrentBeingMappedBindingCollection = this._datacontext.GameProfilesService.RealCurrentBeingMappedBindingCollection;
				if (!(userControl is AdvancedStickSettings) && !(userControl is MacroSettings))
				{
					if (userControl is MaskView)
					{
						if (realCurrentBeingMappedBindingCollection != null && VisualTreeHelperExt.FindParent<BindingFrameUC>((DependencyObject)e.OriginalSource, null) == null)
						{
							realCurrentBeingMappedBindingCollection.MaskBindingCollection.CurrentEditItem = null;
						}
					}
					else
					{
						OverlayMenuView overlayMenuView = userControl as OverlayMenuView;
						if (overlayMenuView != null)
						{
							if (VisualTreeHelperExt.FindParent<BindingFrameUC>((DependencyObject)e.OriginalSource, null) == null && this._datacontext.GameProfilesService.RealCurrentBeingMappedBindingCollection.IsOverlayMenuModeView)
							{
								if (overlayMenuView.IsMenuSelected)
								{
									RadialMenuItem radialMenuItem = VisualTreeHelperExt.FindParent<RadialMenuItem>((DependencyObject)e.OriginalSource, null);
									if (radialMenuItem != null)
									{
										if (radialMenuItem.IsEmpty)
										{
											return;
										}
										if (e.ChangedButton == MouseButton.Right && (radialMenuItem.RadialMenu.IsSubMenu || !radialMenuItem.RadialMenu.IsSubMenuShown))
										{
											OverlayMenuVM overlayMenu = this._datacontext.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.OverlayMenu;
											if (overlayMenu != null)
											{
												OverlayMenuCircle circle = overlayMenu.Circle;
												if (circle != null)
												{
													circle.MainOrSubmenu.SetActive(-1);
												}
											}
											return;
										}
									}
									if (VisualTreeHelperExt.FindParent<zColorPicker>((DependencyObject)e.OriginalSource, null) == null)
									{
										OverlayMenuVM overlayMenu2 = this._datacontext.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.OverlayMenu;
										if (overlayMenu2 != null)
										{
											OverlayMenuCircle circle2 = overlayMenu2.Circle;
											if (circle2 != null)
											{
												circle2.ClearActive();
											}
										}
									}
								}
							}
						}
						else if (userControl is KeyboardMappingView)
						{
							if (realCurrentBeingMappedBindingCollection != null && VisualTreeHelperExt.FindParent((DependencyObject)e.OriginalSource, new List<Type>
							{
								typeof(ComboBox),
								typeof(Popup),
								typeof(BindingFrameUC)
							}, null) == null)
							{
								realCurrentBeingMappedBindingCollection.ControllerBindings.CurrentEditItem = null;
							}
						}
						else if (userControl is MouseMappingView || userControl is SVGGamepadWithAllAnnotations)
						{
							if (this._datacontext.GameProfilesService.RealCurrentBeingMappedBindingCollection != null)
							{
								BindingFrameUC bindingFrameUC = VisualTreeHelperExt.FindParent<BindingFrameUC>((DependencyObject)e.OriginalSource, null);
								if (!realCurrentBeingMappedBindingCollection.CurrentBindingIsLeftDS3AnalogZone && !realCurrentBeingMappedBindingCollection.CurrentBindingIsRightDS3AnalogZone)
								{
									if (bindingFrameUC == null)
									{
										realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(null);
										realCurrentBeingMappedBindingCollection.ControllerBindings.CurrentEditItem = null;
										reWASDApplicationCommands.NavigateBindingFrameCommand.Execute(null);
									}
								}
							}
						}
						else
						{
							if (this._datacontext.RegionManager.Regions.ContainsRegionWithName(RegionNames.Content))
							{
								UserControl userControl2 = this._datacontext.RegionManager.Regions[RegionNames.Content].ActiveViews.FirstOrDefault<object>() as UserControl;
								if (userControl2 != null)
								{
									BindingFrameUC bindingFrameUC2 = VisualTreeHelperExt.FindChild<BindingFrameUC>(userControl2, null);
									if (((bindingFrameUC2 != null) ? bindingFrameUC2.RegionContentControl : null) != null)
									{
										if (!(bindingFrameUC2.RegionContentControl.Content is BFMain))
										{
											BFRumble bfrumble = bindingFrameUC2.RegionContentControl.Content as BFRumble;
											if ((bfrumble == null || !(bfrumble.BindingFrameViewTypeToReturnBack == null)) && !(bindingFrameUC2.RegionContentControl.Content is BFGamepadMapping) && !(bindingFrameUC2.RegionContentControl.Content is BFAdaptiveTriggerSettings))
											{
												goto IL_37E;
											}
										}
										if (realCurrentBeingMappedBindingCollection != null)
										{
											realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(null);
										}
									}
								}
							}
							IL_37E:;
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void MainContent_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			GamesSelectorVM gamesSelectorVM = this.Header.gamesSelector.DataContext as GamesSelectorVM;
			if (gamesSelectorVM != null && gamesSelectorVM.IsGameListShown)
			{
				if (e.OriginalSource is Run)
				{
					gamesSelectorVM.IsGameListShown = false;
					return;
				}
				if (VisualTreeHelperExt.FindParent<GamesSelector>((DependencyObject)e.OriginalSource, null) == null)
				{
					gamesSelectorVM.IsGameListShown = false;
				}
			}
		}

		private bool _isFirstShow = true;
	}
}
