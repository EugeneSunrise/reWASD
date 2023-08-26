using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using RadialMenu.Controls;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Controls;
using reWASDUI.Controls.XBBindingControls.BindingFrame;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;
using reWASDUI.Infrastructure.KeyBindings.ControllerBindings;
using reWASDUI.Infrastructure.KeyBindings.XB;

namespace reWASDUI.Views.OverlayMenu
{
	public partial class OverlayMenuView : BaseServicesDataContextControl, INotifyPropertyChanged
	{
		public ObservableCollection<TViewTabs> ViewTabs { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
			}
		}

		public TViewTabs CurrentViewTab
		{
			get
			{
				return this._CurrentViewTab;
			}
			set
			{
				this._CurrentViewTab = value;
				this.OnPropertyChanged("IsPrefsSelected");
				this.UpdateButtons();
			}
		}

		public OverlayMenuView()
		{
			this.ViewTabs = new ObservableCollection<TViewTabs>();
			this.ViewTabs.Add(new TViewTabs(DTLocalization.GetString(12737), Application.Current.TryFindResource("OverlayMenuMode") as Drawing));
			this.ViewTabs.Add(new TViewTabs(DTLocalization.GetString(12738), Application.Current.TryFindResource("MenuPreferences") as Drawing));
			this.CurrentViewTab = this.ViewTabs[0];
			base.IsVisibleChanged += delegate(object e, DependencyPropertyChangedEventArgs s)
			{
				this.UpdateButtons();
			};
			this.InitializeComponent();
		}

		public void UpdateButtons()
		{
			ConfigVM currentConfig = App.GameProfilesService.CurrentGame.CurrentConfig;
			BaseControllerVM controller = App.GamepadService.CurrentGamepad;
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
						List<SubConfigData> list2 = currentConfig.ConfigData.Where((SubConfigData x) => x.IsGamepad).ToList<SubConfigData>();
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
						List<SubConfigData> list4 = currentConfig.ConfigData.Where((SubConfigData x) => x.IsKeyboard).ToList<SubConfigData>();
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
						List<SubConfigData> list6 = currentConfig.ConfigData.Where((SubConfigData x) => x.IsMouse).ToList<SubConfigData>();
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
				SubConfigData subConfigData10 = currentConfig.ConfigData.FirstOrDefault(delegate(SubConfigData x)
				{
					if (controller != null)
					{
						ControllerFamily controllerFamily3 = x.ControllerFamily;
						BaseControllerVM controller2 = controller;
						ControllerFamily? controllerFamily4;
						if (controller2 == null)
						{
							controllerFamily4 = null;
						}
						else
						{
							ControllerVM currentController2 = controller2.CurrentController;
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
					BaseControllerVM controller3 = controller;
					ControllerFamily? controllerFamily;
					if (controller3 == null)
					{
						controllerFamily = null;
					}
					else
					{
						ControllerVM currentController = controller3.CurrentController;
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
				this.UIUseButton = ((xbbinding2 != null) ? xbbinding2.ControllerButton : null) ?? associatedControllerButton;
				if (((xbbinding2 != null) ? xbbinding2.ControllerButton : null) != null)
				{
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
					this.UIUseButtonControllerType = controllerTypeEnum2.GetValueOrDefault();
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
				this.UICancelButton = ((xbbinding4 != null) ? xbbinding4.ControllerButton : null) ?? associatedControllerButton;
				if (((xbbinding4 != null) ? xbbinding4.ControllerButton : null) != null)
				{
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
					this.UICancelButtonControllerType = controllerTypeEnum2.GetValueOrDefault();
					isSetBack = !isSetBack;
				}
			}
		}

		private void FillOverlayButtonsForButtonMappings(BaseControllerVM controller, ShiftXBBindingCollection overlayShift, ref bool isSetApply, ref bool isSetBack)
		{
			AssociatedControllerButton associatedControllerButton = new AssociatedControllerButton(2001);
			associatedControllerButton.SetDefaultButtons(true);
			XBBinding value = overlayShift.FirstOrDefault((KeyValuePair<GamepadButton, XBBinding> x) => x.Value.ActivatorXBBindingDictionary.Any((KeyValuePair<ActivatorType, ActivatorXBBinding> y) => y.Value.MappedKey.IsOverlayMenuCommand && y.Value.MappedKey.Description == "OVERLAY_MENU_APPLY")).Value;
			if (!isSetApply)
			{
				this.UIUseButton = ((value != null) ? value.ControllerButton : null) ?? associatedControllerButton;
				if (((value != null) ? value.ControllerButton : null) != null)
				{
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
					this.UIUseButtonControllerType = controllerTypeEnum2.GetValueOrDefault();
					isSetApply = !isSetApply;
				}
			}
			XBBinding value2 = overlayShift.FirstOrDefault((KeyValuePair<GamepadButton, XBBinding> x) => x.Value.ActivatorXBBindingDictionary.Any((KeyValuePair<ActivatorType, ActivatorXBBinding> y) => y.Value.MappedKey.IsOverlayMenuCommand && y.Value.MappedKey.Description == "OVERLAY_MENU_CANCEL")).Value;
			if (!isSetBack)
			{
				this.UICancelButton = ((value2 != null) ? value2.ControllerButton : null) ?? associatedControllerButton;
				if (((value2 != null) ? value2.ControllerButton : null) != null)
				{
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
					this.UICancelButtonControllerType = controllerTypeEnum2.GetValueOrDefault();
					isSetBack = !isSetBack;
				}
			}
		}

		public ControllerTypeEnum UIUseButtonControllerType
		{
			get
			{
				return this._uiUseButtonControllerType;
			}
			set
			{
				this.SetProperty<ControllerTypeEnum>(ref this._uiUseButtonControllerType, value, "UIUseButtonControllerType");
			}
		}

		public ControllerTypeEnum UICancelButtonControllerType
		{
			get
			{
				return this._uiCancelButtonControllerType;
			}
			set
			{
				this.SetProperty<ControllerTypeEnum>(ref this._uiCancelButtonControllerType, value, "UICancelButtonControllerType");
			}
		}

		public AssociatedControllerButton UIUseButton
		{
			get
			{
				return this._uiUseButton;
			}
			set
			{
				this.SetProperty<AssociatedControllerButton>(ref this._uiUseButton, value, "UIUseButton");
			}
		}

		public AssociatedControllerButton UICancelButton
		{
			get
			{
				return this._uiCancelButton;
			}
			set
			{
				this.SetProperty<AssociatedControllerButton>(ref this._uiCancelButton, value, "UICancelButton");
			}
		}

		public bool IsPrefsSelected
		{
			get
			{
				return this.CurrentViewTab == this.ViewTabs[1];
			}
		}

		public bool IsMenuSelected
		{
			get
			{
				return this.CurrentViewTab == this.ViewTabs[0];
			}
		}

		private void ConfigsList_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			ScrollViewer scrollViewer = VisualTreeHelperExt.FindChildren<ScrollViewer>(sender as ListBox).FirstOrDefault<ScrollViewer>();
			if (e.Delta > 0)
			{
				if (scrollViewer != null)
				{
					scrollViewer.LineLeft();
				}
			}
			else if (scrollViewer != null)
			{
				scrollViewer.LineRight();
			}
			e.Handled = true;
		}

		private void ConfigsList_OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		private void BindingFrameUC_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			(sender as BindingFrameUC).RegionManager.NavigateToDefaultView();
		}

		private void RadialMenuItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton != MouseButton.Left)
			{
				if (e.ChangedButton == MouseButton.Right && (sender as RadialMenuItem).IsEmpty)
				{
					e.Handled = true;
				}
				return;
			}
			bool isActive = (sender as RadialMenuItem).IsActive;
			RadialMenuItem radialMenuItem = sender as RadialMenuItem;
			if (((radialMenuItem != null) ? radialMenuItem.RadialMenu.ItemsControl : null) == null)
			{
				return;
			}
			if ((sender as RadialMenuItem).RadialMenu.IsSubMenu)
			{
				App.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.OverlayMenu.Circle.CurrentSector.Submenu.SetActive(isActive ? (-1) : (sender as RadialMenuItem).Index);
			}
			else if ((sender as RadialMenuItem).RadialMenu.IsSubMenuShown)
			{
				if ((sender as RadialMenuItem).Index == App.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.OverlayMenu.Circle.CurrentSector.NumberSector)
				{
					App.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.OverlayMenu.Circle.SwitchSubmenuCommand.Execute((sender as RadialMenuItem).Index);
				}
				else
				{
					App.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.OverlayMenu.Circle.SetActive(isActive ? (-1) : (sender as RadialMenuItem).Index);
				}
			}
			else
			{
				App.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.OverlayMenu.Circle.SetActive(isActive ? (-1) : (sender as RadialMenuItem).Index);
			}
			e.Handled = true;
		}

		protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (object.Equals(storage, value))
			{
				return false;
			}
			storage = value;
			this.OnPropertyChanged(propertyName);
			return true;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				((RadialMenuItem)target).PreviewMouseDown += this.RadialMenuItem_PreviewMouseDown;
				return;
			}
			if (connectionId != 2)
			{
				return;
			}
			((ListBox)target).PreviewMouseRightButtonDown += this.ConfigsList_OnPreviewMouseRightButtonDown;
			((ListBox)target).PreviewMouseWheel += this.ConfigsList_OnPreviewMouseWheel;
		}

		private TViewTabs _CurrentViewTab;

		private ControllerTypeEnum _uiUseButtonControllerType;

		private ControllerTypeEnum _uiCancelButtonControllerType;

		private AssociatedControllerButton _uiUseButton;

		private AssociatedControllerButton _uiCancelButton;
	}
}
