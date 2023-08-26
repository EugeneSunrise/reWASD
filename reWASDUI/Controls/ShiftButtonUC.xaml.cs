using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Prism.Commands;
using Prism.Regions;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Controls.XBBindingControls.BindingFrame;
using reWASDUI.Controls.XBBindingControls.BindingFrame.BindingFrameViews;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.XB;
using Xceed.Wpf.Toolkit.Core.Utilities;

namespace reWASDUI.Controls
{
	public partial class ShiftButtonUC : UserControl
	{
		public XBBinding XBBinding
		{
			get
			{
				return (XBBinding)base.GetValue(ShiftButtonUC.XBBindingProperty);
			}
			set
			{
				base.SetValue(ShiftButtonUC.XBBindingProperty, value);
			}
		}

		public Type BindingFrameViewTypeToReturnBack
		{
			get
			{
				return (Type)base.GetValue(ShiftButtonUC.BindingFrameViewTypeToReturnBackProperty);
			}
			set
			{
				base.SetValue(ShiftButtonUC.BindingFrameViewTypeToReturnBackProperty, value);
			}
		}

		public DelegateCommand ShowShiftSettingsCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._showShiftSettings) == null)
				{
					delegateCommand = (this._showShiftSettings = new DelegateCommand(new Action(this.ShowShiftSettings)));
				}
				return delegateCommand;
			}
		}

		public ShiftButtonUC()
		{
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (this._bindingFrameUc == null)
			{
				this._bindingFrameUc = VisualTreeHelperEx.FindAncestorByType<BindingFrameUC>(this);
			}
			if (!this._isFirstLoadInited)
			{
				this._isFirstLoadInited = true;
				if (this.XBBinding == null)
				{
					BindingOperations.SetBinding(this, ShiftButtonUC.XBBindingProperty, new Binding("GameProfilesService.RealCurrentBeingMappedBindingCollection.CurrentXBBinding"));
				}
			}
		}

		private void ShowShiftSettings()
		{
			XBBinding xbbinding = this.XBBinding;
			if (((xbbinding != null) ? xbbinding.ControllerButton : null) != null)
			{
				AssociatedControllerButton controllerButton = this.XBBinding.ControllerButton;
				if (controllerButton.IsGamepad)
				{
					App.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(controllerButton.GamepadButton), true);
				}
				else if (controllerButton.IsKeyScanCode)
				{
					App.GameProfilesService.RealCurrentBeingMappedBindingCollection.ControllerBindings.SetCurrentEditTo(controllerButton);
				}
			}
			NavigationParameters navigationParameters = new NavigationParameters();
			navigationParameters.Add("BindingFrameViewTypeToReturnBack", this.BindingFrameViewTypeToReturnBack);
			this._bindingFrameUc.RegionManager.RequestNavigate(typeof(BFShift).ToString(), navigationParameters);
		}

		public static readonly DependencyProperty XBBindingProperty = DependencyProperty.Register("XBBinding", typeof(XBBinding), typeof(ShiftButtonUC), new PropertyMetadata(null));

		public static readonly DependencyProperty BindingFrameViewTypeToReturnBackProperty = DependencyProperty.Register("BindingFrameViewTypeToReturnBack", typeof(Type), typeof(ShiftButtonUC), new PropertyMetadata(null));

		private DelegateCommand _showShiftSettings;

		private BindingFrameUC _bindingFrameUc;

		private bool _isFirstLoadInited;
	}
}
