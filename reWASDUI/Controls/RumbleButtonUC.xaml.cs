using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using DiscSoft.NET.Common.Utils;
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
	public partial class RumbleButtonUC : UserControl
	{
		public XBBinding XBBinding
		{
			get
			{
				return (XBBinding)base.GetValue(RumbleButtonUC.XBBindingProperty);
			}
			set
			{
				base.SetValue(RumbleButtonUC.XBBindingProperty, value);
			}
		}

		public AssociatedControllerButton ControllerButtonShowFor
		{
			get
			{
				return (AssociatedControllerButton)base.GetValue(RumbleButtonUC.ControllerButtonShowForProperty);
			}
			set
			{
				base.SetValue(RumbleButtonUC.ControllerButtonShowForProperty, value);
			}
		}

		private static void ControllerButtonShowForChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RumbleButtonUC rumbleButtonUC = d as RumbleButtonUC;
			if (rumbleButtonUC == null)
			{
				return;
			}
			rumbleButtonUC.OnControllerButtonShowForChanged(e);
		}

		private void OnControllerButtonShowForChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ResolveXBBinding();
		}

		public Type BindingFrameViewTypeToReturnBack
		{
			get
			{
				return (Type)base.GetValue(RumbleButtonUC.BindingFrameViewTypeToReturnBackProperty);
			}
			set
			{
				base.SetValue(RumbleButtonUC.BindingFrameViewTypeToReturnBackProperty, value);
			}
		}

		public DelegateCommand ShowRumbleSettingsCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._showRumbleSettings) == null)
				{
					delegateCommand = (this._showRumbleSettings = new DelegateCommand(new Action(this.ShowRumbleSettings)));
				}
				return delegateCommand;
			}
		}

		public RumbleButtonUC()
		{
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
		}

		private void ResolveXBBinding()
		{
			if (this.ControllerButtonShowFor != null && this.ControllerButtonShowFor.IsSet)
			{
				BaseXBBindingCollection realCurrentBeingMappedBindingCollection = App.GameProfilesService.RealCurrentBeingMappedBindingCollection;
				this.XBBinding = ((realCurrentBeingMappedBindingCollection != null) ? realCurrentBeingMappedBindingCollection.GetXBBindingByAssociatedControllerButton(this.ControllerButtonShowFor) : null);
			}
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			this.ResolveXBBinding();
			if (this._bindingFrameUc == null)
			{
				this._bindingFrameUc = VisualTreeHelperEx.FindAncestorByType<BindingFrameUC>(this);
			}
			if (!this._isFirstLoadInited)
			{
				this._isFirstLoadInited = true;
				if (this.ControllerButtonShowFor == null && this.XBBinding == null)
				{
					BindingOperations.SetBinding(this, RumbleButtonUC.XBBindingProperty, new Binding("GameProfilesService.RealCurrentBeingMappedBindingCollection.CurrentXBBinding"));
				}
			}
		}

		private void ShowRumbleSettings()
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
			if (this.BindingFrameViewTypeToReturnBack != null)
			{
				navigationParameters.Add("BindingFrameViewTypeToReturnBack", this.BindingFrameViewTypeToReturnBack);
			}
			else
			{
				BaseBFView baseBFView = VisualTreeHelperExt.FindParent<BaseBFView>(this, null);
				if (baseBFView != null)
				{
					navigationParameters.Add("BindingFrameViewTypeToReturnBack", baseBFView.GetType());
				}
			}
			navigationParameters.Add("XBBinding", this.XBBinding);
			this._bindingFrameUc.RegionManager.RequestNavigate(typeof(BFRumble).ToString(), navigationParameters);
		}

		public static readonly DependencyProperty XBBindingProperty = DependencyProperty.Register("XBBinding", typeof(XBBinding), typeof(RumbleButtonUC), new PropertyMetadata(null));

		public static readonly DependencyProperty ControllerButtonShowForProperty = DependencyProperty.Register("ControllerButtonShowFor", typeof(AssociatedControllerButton), typeof(RumbleButtonUC), new PropertyMetadata(null, new PropertyChangedCallback(RumbleButtonUC.ControllerButtonShowForChangedCallback)));

		public static readonly DependencyProperty BindingFrameViewTypeToReturnBackProperty = DependencyProperty.Register("BindingFrameViewTypeToReturnBack", typeof(Type), typeof(RumbleButtonUC), new PropertyMetadata(null));

		private DelegateCommand _showRumbleSettings;

		private BindingFrameUC _bindingFrameUc;

		private bool _isFirstLoadInited;
	}
}
