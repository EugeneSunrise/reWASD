using System;
using System.Windows;
using System.Windows.Controls;
using Prism.Events;
using reWASDUI.Infrastructure;
using reWASDUI.Services;

namespace reWASDUI.Controls
{
	public abstract class BaseServicesResolvingControl : UserControl, IBaseServicesContainer
	{
		public GamepadService GamepadService
		{
			get
			{
				return (GamepadService)base.GetValue(BaseServicesResolvingControl.GamepadServiceProperty);
			}
			set
			{
				base.SetValue(BaseServicesResolvingControl.GamepadServiceProperty, value);
			}
		}

		public GameProfilesService GameProfilesService
		{
			get
			{
				return (GameProfilesService)base.GetValue(BaseServicesResolvingControl.GameProfilesServiceProperty);
			}
			set
			{
				base.SetValue(BaseServicesResolvingControl.GameProfilesServiceProperty, value);
			}
		}

		public GuiHelperService GuiHelperService
		{
			get
			{
				return (GuiHelperService)base.GetValue(BaseServicesResolvingControl.GuiHelperServiceProperty);
			}
			set
			{
				base.SetValue(BaseServicesResolvingControl.GuiHelperServiceProperty, value);
			}
		}

		public KeyBindingService KeyBindingService
		{
			get
			{
				return (KeyBindingService)base.GetValue(BaseServicesResolvingControl.KeyBindingServiceProperty);
			}
			set
			{
				base.SetValue(BaseServicesResolvingControl.KeyBindingServiceProperty, value);
			}
		}

		public EventAggregator EventAggregator
		{
			get
			{
				return (EventAggregator)base.GetValue(BaseServicesResolvingControl.EventAggregatorProperty);
			}
			set
			{
				base.SetValue(BaseServicesResolvingControl.EventAggregatorProperty, value);
			}
		}

		public LicensingService LicensingService
		{
			get
			{
				return (LicensingService)base.GetValue(BaseServicesResolvingControl.LicensingServiceProperty);
			}
			set
			{
				base.SetValue(BaseServicesResolvingControl.LicensingServiceProperty, value);
			}
		}

		public GuiScaleService GuiScaleService
		{
			get
			{
				return (GuiScaleService)base.GetValue(BaseServicesResolvingControl.GuiScaleServiceProperty);
			}
			set
			{
				base.SetValue(BaseServicesResolvingControl.GuiScaleServiceProperty, value);
			}
		}

		protected BaseServicesResolvingControl()
		{
			base.Loaded += this.OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			this.SetServices();
		}

		private void SetServices()
		{
			this.KeyBindingService = (KeyBindingService)App.KeyBindingService;
			this.GamepadService = (GamepadService)App.GamepadService;
			this.GameProfilesService = (GameProfilesService)App.GameProfilesService;
			this.GuiHelperService = (GuiHelperService)App.GuiHelperService;
			this.EventAggregator = (EventAggregator)App.EventAggregator;
			this.LicensingService = (LicensingService)App.LicensingService;
			this.OnServicesReady();
		}

		protected virtual void OnServicesReady()
		{
		}

		public static readonly DependencyProperty GamepadServiceProperty = DependencyProperty.Register("GamepadService", typeof(GamepadService), typeof(BaseServicesResolvingControl), new PropertyMetadata(null));

		public static readonly DependencyProperty GameProfilesServiceProperty = DependencyProperty.Register("GameProfilesService", typeof(GameProfilesService), typeof(BaseServicesResolvingControl), new PropertyMetadata(null));

		public static readonly DependencyProperty GuiHelperServiceProperty = DependencyProperty.Register("GuiHelperService", typeof(GuiHelperService), typeof(BaseServicesResolvingControl), new PropertyMetadata(null));

		public static readonly DependencyProperty KeyBindingServiceProperty = DependencyProperty.Register("KeyBindingService", typeof(KeyBindingService), typeof(BaseServicesResolvingControl), new PropertyMetadata(null));

		public static readonly DependencyProperty EventAggregatorProperty = DependencyProperty.Register("EventAggregator", typeof(EventAggregator), typeof(BaseServicesResolvingControl), new PropertyMetadata(null));

		public static readonly DependencyProperty LicensingServiceProperty = DependencyProperty.Register("LicensingService", typeof(LicensingService), typeof(BaseServicesResolvingControl), new PropertyMetadata(null));

		public static readonly DependencyProperty GuiScaleServiceProperty = DependencyProperty.Register("GuiScaleService", typeof(GuiScaleService), typeof(BaseServicesResolvingControl), new PropertyMetadata(null));
	}
}
