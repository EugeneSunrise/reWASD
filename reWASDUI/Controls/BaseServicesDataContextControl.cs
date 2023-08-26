using System;
using System.Windows;
using System.Windows.Controls;
using Prism.Events;
using reWASDUI.Infrastructure;
using reWASDUI.Services;
using reWASDUI.ViewModels.Base;

namespace reWASDUI.Controls
{
	public abstract class BaseServicesDataContextControl : UserControl, IBaseServicesContainer
	{
		public bool ThrowExceptionIfNoServices
		{
			get
			{
				return (bool)base.GetValue(BaseServicesDataContextControl.ThrowExceptionIfNoServicesProperty);
			}
			set
			{
				base.SetValue(BaseServicesDataContextControl.ThrowExceptionIfNoServicesProperty, value);
			}
		}

		public GamepadService GamepadService
		{
			get
			{
				return this._dataContext.GamepadService;
			}
		}

		public GameProfilesService GameProfilesService
		{
			get
			{
				return this._dataContext.GameProfilesService;
			}
		}

		public KeyBindingService KeyBindingService
		{
			get
			{
				return this._dataContext.KeyBindingService;
			}
		}

		public EventAggregator EventAggregator
		{
			get
			{
				return this._dataContext.EventAggregator;
			}
		}

		public LicensingService LicensingService
		{
			get
			{
				return this._dataContext.LicensingService;
			}
		}

		public GuiScaleService GuiScaleService
		{
			get
			{
				return this._dataContext.GuiScaleService;
			}
		}

		public GuiHelperService GuiHelperService
		{
			get
			{
				return this._dataContext.GuiHelperService;
			}
		}

		protected BaseServicesVM _dataContext
		{
			get
			{
				return base.DataContext as BaseServicesVM;
			}
		}

		protected BaseServicesDataContextControl()
		{
			base.DataContextChanged += this.OnDataContextChanged;
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != null && !(e.NewValue is BaseServicesVM) && this.ThrowExceptionIfNoServices)
			{
				throw new Exception("KeyBindingService or GamepadService or GameProfilesService or EventAggregator is(are) null. Please check why");
			}
		}

		public static readonly DependencyProperty ThrowExceptionIfNoServicesProperty = DependencyProperty.Register("ThrowExceptionIfNoServices", typeof(bool), typeof(BaseServicesDataContextControl), new PropertyMetadata(true));
	}
}
