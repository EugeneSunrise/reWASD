using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;
using DTOverlay;
using RadialMenu.Controls;
using reWASDCommon.Infrastructure.Enums;
using reWASDEngine.Services.OverlayAPI;
using XBEliteWPF.DataModels;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Infrastructure.KeyBindingsModel.ActivatorXB;
using XBEliteWPF.Infrastructure.KeyBindingsModel.ControllerBindings;
using XBEliteWPF.Infrastructure.KeyBindingsModel.OverlayMenu;
using XBEliteWPF.Infrastructure.KeyBindingsModel.XB;

namespace Overlay.NET.Wpf
{
	public class OverlayMenuViewE : Window, INotifyPropertyChanged, IComponentConnector, IStyleConnector
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
			}
		}

		public object RootWindow { get; private set; }

		public OverlayMenuVME ViewModel
		{
			get
			{
				return this._dataContext;
			}
		}

		public OverlayMenuViewE(OverlayMenuE overlayMenuE, BaseControllerVM controller, string ID, ushort serviceProfileId, Config config, bool itoggle)
		{
			ConfigData configData = config.ConfigData;
			if (configData != null)
			{
				OverlayMenuVM overlayMenu = configData.OverlayMenu;
				OverlayMenuTools.FillBoundMenuDirections(controller, ID, configData, this.TouchPad1IDs, this.TouchPad2IDs, this.MouseIDs, this.StickLeftIDs, this.StickRighrIDs, this.AddStickIDs, this.TouchPad1ClickRequiredIDs, this.TouchPad2ClickRequiredIDs);
				OverlayMenuVME overlayMenuVME = new OverlayMenuVME(overlayMenuE, overlayMenu, serviceProfileId, this.TouchPad1IDs, this.TouchPad2IDs, this.MouseIDs, this.StickLeftIDs, this.StickRighrIDs, this.AddStickIDs, this.TouchPad1ClickRequiredIDs, this.TouchPad2ClickRequiredIDs, itoggle);
				this._dataContext = overlayMenuVME;
				base.DataContext = this._dataContext;
				this.UpdateButtons(controller, config);
				this.InitializeComponent();
				this.OnPropertyChanged("ViewModel");
				this.IsOkIsLoaded = overlayMenuVME.IsLoaded;
			}
		}

		public void UpdateButtons(BaseControllerVM controller, Config config)
		{
			bool flag = false;
			bool flag2 = false;
			if (controller != null && controller.IsCompositeDevice)
			{
				CompositeControllerVM compositeControllerVM = controller as CompositeControllerVM;
				if (compositeControllerVM != null)
				{
					List<BaseControllerVM> list = compositeControllerVM.BaseControllers.Where((BaseControllerVM x) => x != null && x.ControllerFamily == 0).ToList<BaseControllerVM>();
					for (int i = 0; i < list.Count<BaseControllerVM>(); i++)
					{
						List<SubConfigData> list2 = config.ConfigData.GetSubConfigGamepadList().ToList<SubConfigData>();
						ShiftXBBindingCollection shiftXBBindingCollection2;
						try
						{
							if (list2.Count<SubConfigData>() > 1)
							{
								SubConfigData subConfigData = list2[i];
								ShiftXBBindingCollection shiftXBBindingCollection;
								if (subConfigData == null)
								{
									shiftXBBindingCollection = null;
								}
								else
								{
									shiftXBBindingCollection = subConfigData.MainXBBindingCollection.ShiftXBBindingCollections.FirstOrDefault((ShiftXBBindingCollection y) => y.IsOverlayShift);
								}
								shiftXBBindingCollection2 = shiftXBBindingCollection;
							}
							else
							{
								ShiftXBBindingCollection shiftXBBindingCollection3;
								if (list2 == null)
								{
									shiftXBBindingCollection3 = null;
								}
								else
								{
									SubConfigData subConfigData2 = list2.FirstOrDefault<SubConfigData>();
									if (subConfigData2 == null)
									{
										shiftXBBindingCollection3 = null;
									}
									else
									{
										shiftXBBindingCollection3 = subConfigData2.MainXBBindingCollection.ShiftXBBindingCollections.FirstOrDefault((ShiftXBBindingCollection y) => y.IsOverlayShift);
									}
								}
								shiftXBBindingCollection2 = shiftXBBindingCollection3;
							}
						}
						catch (Exception)
						{
							ShiftXBBindingCollection shiftXBBindingCollection4;
							if (list2 == null)
							{
								shiftXBBindingCollection4 = null;
							}
							else
							{
								SubConfigData subConfigData3 = list2.FirstOrDefault<SubConfigData>();
								if (subConfigData3 == null)
								{
									shiftXBBindingCollection4 = null;
								}
								else
								{
									shiftXBBindingCollection4 = subConfigData3.MainXBBindingCollection.ShiftXBBindingCollections.FirstOrDefault((ShiftXBBindingCollection y) => y.IsOverlayShift);
								}
							}
							shiftXBBindingCollection2 = shiftXBBindingCollection4;
						}
						this.FillOverlayButtonsForButtonMappings(list[i], shiftXBBindingCollection2, ref flag, ref flag2);
					}
					List<BaseControllerVM> list3 = compositeControllerVM.BaseControllers.Where((BaseControllerVM x) => x != null && x.ControllerFamily == 1).ToList<BaseControllerVM>();
					for (int j = 0; j < list3.Count<BaseControllerVM>(); j++)
					{
						List<SubConfigData> list4 = config.ConfigData.GetSubConfigKeyboardList().ToList<SubConfigData>();
						ShiftXBBindingCollection shiftXBBindingCollection6;
						try
						{
							if (list4.Count<SubConfigData>() > 1)
							{
								SubConfigData subConfigData4 = list4[j];
								ShiftXBBindingCollection shiftXBBindingCollection5;
								if (subConfigData4 == null)
								{
									shiftXBBindingCollection5 = null;
								}
								else
								{
									shiftXBBindingCollection5 = subConfigData4.MainXBBindingCollection.ShiftXBBindingCollections.FirstOrDefault((ShiftXBBindingCollection y) => y.IsOverlayShift);
								}
								shiftXBBindingCollection6 = shiftXBBindingCollection5;
							}
							else
							{
								ShiftXBBindingCollection shiftXBBindingCollection7;
								if (list4 == null)
								{
									shiftXBBindingCollection7 = null;
								}
								else
								{
									SubConfigData subConfigData5 = list4.FirstOrDefault<SubConfigData>();
									if (subConfigData5 == null)
									{
										shiftXBBindingCollection7 = null;
									}
									else
									{
										shiftXBBindingCollection7 = subConfigData5.MainXBBindingCollection.ShiftXBBindingCollections.FirstOrDefault((ShiftXBBindingCollection y) => y.IsOverlayShift);
									}
								}
								shiftXBBindingCollection6 = shiftXBBindingCollection7;
							}
						}
						catch (Exception)
						{
							ShiftXBBindingCollection shiftXBBindingCollection8;
							if (list4 == null)
							{
								shiftXBBindingCollection8 = null;
							}
							else
							{
								SubConfigData subConfigData6 = list4.FirstOrDefault<SubConfigData>();
								if (subConfigData6 == null)
								{
									shiftXBBindingCollection8 = null;
								}
								else
								{
									shiftXBBindingCollection8 = subConfigData6.MainXBBindingCollection.ShiftXBBindingCollections.FirstOrDefault((ShiftXBBindingCollection y) => y.IsOverlayShift);
								}
							}
							shiftXBBindingCollection6 = shiftXBBindingCollection8;
						}
						this.FillOverlayButtonsForControllerBindings(list3[j], shiftXBBindingCollection6, ref flag, ref flag2);
					}
					List<BaseControllerVM> list5 = compositeControllerVM.BaseControllers.Where((BaseControllerVM x) => x != null && x.ControllerFamily == 2).ToList<BaseControllerVM>();
					for (int k = 0; k < list5.Count<BaseControllerVM>(); k++)
					{
						List<SubConfigData> list6 = config.ConfigData.GetSubConfigMouseList().ToList<SubConfigData>();
						ShiftXBBindingCollection shiftXBBindingCollection10;
						try
						{
							if (list6.Count<SubConfigData>() > 1)
							{
								SubConfigData subConfigData7 = list6[k];
								ShiftXBBindingCollection shiftXBBindingCollection9;
								if (subConfigData7 == null)
								{
									shiftXBBindingCollection9 = null;
								}
								else
								{
									shiftXBBindingCollection9 = subConfigData7.MainXBBindingCollection.ShiftXBBindingCollections.FirstOrDefault((ShiftXBBindingCollection y) => y.IsOverlayShift);
								}
								shiftXBBindingCollection10 = shiftXBBindingCollection9;
							}
							else
							{
								ShiftXBBindingCollection shiftXBBindingCollection11;
								if (list6 == null)
								{
									shiftXBBindingCollection11 = null;
								}
								else
								{
									SubConfigData subConfigData8 = list6.FirstOrDefault<SubConfigData>();
									if (subConfigData8 == null)
									{
										shiftXBBindingCollection11 = null;
									}
									else
									{
										shiftXBBindingCollection11 = subConfigData8.MainXBBindingCollection.ShiftXBBindingCollections.FirstOrDefault((ShiftXBBindingCollection y) => y.IsOverlayShift);
									}
								}
								shiftXBBindingCollection10 = shiftXBBindingCollection11;
							}
						}
						catch (Exception)
						{
							ShiftXBBindingCollection shiftXBBindingCollection12;
							if (list6 == null)
							{
								shiftXBBindingCollection12 = null;
							}
							else
							{
								SubConfigData subConfigData9 = list6.FirstOrDefault<SubConfigData>();
								if (subConfigData9 == null)
								{
									shiftXBBindingCollection12 = null;
								}
								else
								{
									shiftXBBindingCollection12 = subConfigData9.MainXBBindingCollection.ShiftXBBindingCollections.FirstOrDefault((ShiftXBBindingCollection y) => y.IsOverlayShift);
								}
							}
							shiftXBBindingCollection10 = shiftXBBindingCollection12;
						}
						this.FillOverlayButtonsForControllerBindings(list5[k], shiftXBBindingCollection10, ref flag, ref flag2);
					}
					return;
				}
			}
			else
			{
				SubConfigData subConfigData10 = config.ConfigData.FirstOrDefault(delegate(SubConfigData x)
				{
					if (controller != null)
					{
						ControllerFamily controllerFamily3 = x.ControllerFamily;
						BaseControllerVM controller3 = controller;
						ControllerFamily? controllerFamily4;
						if (controller3 == null)
						{
							controllerFamily4 = null;
						}
						else
						{
							ControllerVM currentController2 = controller3.CurrentController;
							controllerFamily4 = ((currentController2 != null) ? new ControllerFamily?(currentController2.ControllerFamily) : null);
						}
						ControllerFamily? controllerFamily5 = controllerFamily4;
						return controllerFamily3 == controllerFamily5.GetValueOrDefault();
					}
					return false;
				});
				ShiftXBBindingCollection shiftXBBindingCollection13;
				if (subConfigData10 == null)
				{
					shiftXBBindingCollection13 = null;
				}
				else
				{
					shiftXBBindingCollection13 = subConfigData10.MainXBBindingCollection.ShiftXBBindingCollections.FirstOrDefault((ShiftXBBindingCollection y) => y.IsOverlayShift);
				}
				ShiftXBBindingCollection shiftXBBindingCollection14 = shiftXBBindingCollection13;
				if (controller != null)
				{
					BaseControllerVM controller2 = controller;
					ControllerFamily? controllerFamily;
					if (controller2 == null)
					{
						controllerFamily = null;
					}
					else
					{
						ControllerVM currentController = controller2.CurrentController;
						controllerFamily = ((currentController != null) ? new ControllerFamily?(currentController.ControllerFamily) : null);
					}
					ControllerFamily? controllerFamily2 = controllerFamily;
					if (controllerFamily2.GetValueOrDefault() == null)
					{
						this.FillOverlayButtonsForButtonMappings(controller, shiftXBBindingCollection14, ref flag, ref flag2);
						return;
					}
				}
				this.FillOverlayButtonsForControllerBindings(controller, shiftXBBindingCollection14, ref flag, ref flag2);
			}
		}

		private void FillOverlayButtonsForControllerBindings(BaseControllerVM controller, ShiftXBBindingCollection overlayShift, ref bool isSetApply, ref bool isSetBack)
		{
			AssociatedControllerButton associatedControllerButton = new AssociatedControllerButton(2001);
			associatedControllerButton.SetDefaultButtons(true);
			XBBinding xbbinding;
			if (overlayShift == null)
			{
				xbbinding = null;
			}
			else
			{
				ControllerBinding controllerBinding = overlayShift.ControllerBindings.FirstOrDefault((ControllerBinding x) => x.XBBinding.ActivatorXBBindingDictionary.Any((KeyValuePair<ActivatorType, ActivatorXBBinding> y) => y.Value.MappedKey.IsOverlayMenuCommand && y.Value.MappedKey.Description == "OVERLAY_MENU_APPLY"));
				xbbinding = ((controllerBinding != null) ? controllerBinding.XBBinding : null);
			}
			XBBinding xbbinding2 = xbbinding;
			if (!isSetApply)
			{
				this.ViewModel.Circle.UIUseButton = ((xbbinding2 != null) ? xbbinding2.ControllerButton : null) ?? associatedControllerButton;
				if (((xbbinding2 != null) ? xbbinding2.ControllerButton : null) != null)
				{
					OverlayMenuCircleE circle = this.ViewModel.Circle;
					ControllerTypeEnum? controllerTypeEnum;
					if (controller == null)
					{
						controllerTypeEnum = null;
					}
					else
					{
						ControllerVM currentController = controller.CurrentController;
						controllerTypeEnum = ((currentController != null) ? new ControllerTypeEnum?(currentController.ControllerType) : null);
					}
					ControllerTypeEnum? controllerTypeEnum2 = controllerTypeEnum;
					circle.UIUseButtonControllerType = controllerTypeEnum2.GetValueOrDefault();
					isSetApply = !isSetApply;
				}
			}
			XBBinding xbbinding3;
			if (overlayShift == null)
			{
				xbbinding3 = null;
			}
			else
			{
				ControllerBinding controllerBinding2 = overlayShift.ControllerBindings.FirstOrDefault((ControllerBinding x) => x.XBBinding.ActivatorXBBindingDictionary.Any((KeyValuePair<ActivatorType, ActivatorXBBinding> y) => y.Value.MappedKey.IsOverlayMenuCommand && y.Value.MappedKey.Description == "OVERLAY_MENU_CANCEL"));
				xbbinding3 = ((controllerBinding2 != null) ? controllerBinding2.XBBinding : null);
			}
			XBBinding xbbinding4 = xbbinding3;
			if (!isSetBack)
			{
				this.ViewModel.Circle.UICancelButton = ((xbbinding4 != null) ? xbbinding4.ControllerButton : null) ?? associatedControllerButton;
				if (((xbbinding4 != null) ? xbbinding4.ControllerButton : null) != null)
				{
					OverlayMenuCircleE circle2 = this.ViewModel.Circle;
					ControllerTypeEnum? controllerTypeEnum3;
					if (controller == null)
					{
						controllerTypeEnum3 = null;
					}
					else
					{
						ControllerVM currentController2 = controller.CurrentController;
						controllerTypeEnum3 = ((currentController2 != null) ? new ControllerTypeEnum?(currentController2.ControllerType) : null);
					}
					ControllerTypeEnum? controllerTypeEnum2 = controllerTypeEnum3;
					circle2.UICancelButtonControllerType = controllerTypeEnum2.GetValueOrDefault();
					isSetBack = !isSetBack;
				}
			}
		}

		private void FillOverlayButtonsForButtonMappings(BaseControllerVM controller, ShiftXBBindingCollection overlayShift, ref bool isSetApply, ref bool isSetBack)
		{
			AssociatedControllerButton associatedControllerButton = new AssociatedControllerButton(2001);
			associatedControllerButton.SetDefaultButtons(true);
			XBBinding xbbinding = overlayShift.FirstOrDefault((XBBinding x) => x.ActivatorXBBindingDictionary.Any((KeyValuePair<ActivatorType, ActivatorXBBinding> y) => y.Value.MappedKey.IsOverlayMenuCommand && y.Value.MappedKey.Description == "OVERLAY_MENU_APPLY"));
			if (!isSetApply)
			{
				this.ViewModel.Circle.UIUseButton = ((xbbinding != null) ? xbbinding.ControllerButton : null) ?? associatedControllerButton;
				if (((xbbinding != null) ? xbbinding.ControllerButton : null) != null)
				{
					OverlayMenuCircleE circle = this.ViewModel.Circle;
					ControllerTypeEnum? controllerTypeEnum;
					if (controller == null)
					{
						controllerTypeEnum = null;
					}
					else
					{
						ControllerVM currentController = controller.CurrentController;
						controllerTypeEnum = ((currentController != null) ? new ControllerTypeEnum?(currentController.ControllerType) : null);
					}
					ControllerTypeEnum? controllerTypeEnum2 = controllerTypeEnum;
					circle.UIUseButtonControllerType = controllerTypeEnum2.GetValueOrDefault();
					isSetApply = !isSetApply;
				}
			}
			XBBinding xbbinding2 = overlayShift.FirstOrDefault((XBBinding x) => x.ActivatorXBBindingDictionary.Any((KeyValuePair<ActivatorType, ActivatorXBBinding> y) => y.Value.MappedKey.IsOverlayMenuCommand && y.Value.MappedKey.Description == "OVERLAY_MENU_CANCEL"));
			if (!isSetBack)
			{
				this.ViewModel.Circle.UICancelButton = ((xbbinding2 != null) ? xbbinding2.ControllerButton : null) ?? associatedControllerButton;
				if (((xbbinding2 != null) ? xbbinding2.ControllerButton : null) != null)
				{
					OverlayMenuCircleE circle2 = this.ViewModel.Circle;
					ControllerTypeEnum? controllerTypeEnum3;
					if (controller == null)
					{
						controllerTypeEnum3 = null;
					}
					else
					{
						ControllerVM currentController2 = controller.CurrentController;
						controllerTypeEnum3 = ((currentController2 != null) ? new ControllerTypeEnum?(currentController2.ControllerType) : null);
					}
					ControllerTypeEnum? controllerTypeEnum2 = controllerTypeEnum3;
					circle2.UICancelButtonControllerType = controllerTypeEnum2.GetValueOrDefault();
					isSetBack = !isSetBack;
				}
			}
		}

		private void UpdateSize(object sender, SizeChangedEventArgs e)
		{
			string text = "";
			if (this.IsOkIsLoaded && this.ViewModel != null && this.ViewModel.Circle != null)
			{
				text = this.ViewModel.Circle.Monitor;
			}
			OverlayUtils.SetAlign(text, 4, 0f, this);
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.SourceInitialized += this.OnSourceInitialized;
			base.OnInitialized(e);
		}

		private void OnSourceInitialized(object sender, EventArgs e)
		{
			OverlayUtils.SetExtStyle(this);
		}

		private void RadialMenuItem_StartAngleChanged(object sender, double e)
		{
			RadialMenuItem radialMenuItem = sender as RadialMenuItem;
			SectorItemE sectorItemE = null;
			if (radialMenuItem == null || radialMenuItem.RadialMenu == null)
			{
				return;
			}
			try
			{
				if (radialMenuItem.RadialMenu.IsSubMenu && radialMenuItem.RadialMenu.HostRadialMenuItemIndex >= 0)
				{
					OverlayMenuVME viewModel = this.ViewModel;
					SectorItemE sectorItemE2;
					if (viewModel == null)
					{
						sectorItemE2 = null;
					}
					else
					{
						OverlayMenuCircleE circle = viewModel.Circle;
						if (circle == null)
						{
							sectorItemE2 = null;
						}
						else
						{
							SectorItemE sectorItemE3 = circle.Sectors[radialMenuItem.RadialMenu.HostRadialMenuItemIndex];
							if (sectorItemE3 == null)
							{
								sectorItemE2 = null;
							}
							else
							{
								OverlayMenuCircleE submenu = sectorItemE3.Submenu;
								sectorItemE2 = ((submenu != null) ? submenu.Sectors[radialMenuItem.Index - 100] : null);
							}
						}
					}
					sectorItemE = sectorItemE2;
				}
				else if (!radialMenuItem.RadialMenu.IsSubMenu)
				{
					OverlayMenuVME viewModel2 = this.ViewModel;
					SectorItemE sectorItemE4;
					if (viewModel2 == null)
					{
						sectorItemE4 = null;
					}
					else
					{
						OverlayMenuCircleE circle2 = viewModel2.Circle;
						sectorItemE4 = ((circle2 != null) ? circle2.Sectors[radialMenuItem.Index] : null);
					}
					sectorItemE = sectorItemE4;
				}
			}
			catch
			{
			}
			if (sectorItemE == null)
			{
				return;
			}
			sectorItemE.AngleStart = radialMenuItem.StartAngle;
			sectorItemE.AngleDelta = radialMenuItem.AngleDelta;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/reWASDEngine;component/overlayapi/overlaymenuviewe.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				((OverlayMenuViewE)target).SizeChanged += this.UpdateSize;
				return;
			}
			this._contentLoaded = true;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 2)
			{
				((RadialMenuItem)target).StartAngleChanged += this.RadialMenuItem_StartAngleChanged;
			}
		}

		private OverlayMenuVME _dataContext;

		public AlignType Align;

		private List<ulong> TouchPad1ClickRequiredIDs = new List<ulong>();

		private List<ulong> TouchPad2ClickRequiredIDs = new List<ulong>();

		private List<ulong> TouchPad1IDs = new List<ulong>();

		private List<ulong> TouchPad2IDs = new List<ulong>();

		private List<ulong> MouseIDs = new List<ulong>();

		private List<ulong> StickLeftIDs = new List<ulong>();

		private List<ulong> StickRighrIDs = new List<ulong>();

		private List<ulong> AddStickIDs = new List<ulong>();

		public bool IsOkIsLoaded;

		private bool _contentLoaded;
	}
}
