using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using DiscSoft.NET.Common.Utils;
using Prism.Events;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Services;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Utils.GenericInheritance;

namespace reWASDUI.Controls.XBBindingControls.ButtonBinding
{
	public partial class MultiButtonBinding : BaseButtonBinding
	{
		private static void ButtonsToBindChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MultiButtonBinding multiButtonBinding = d as MultiButtonBinding;
			if (multiButtonBinding == null)
			{
				return;
			}
			if (multiButtonBinding.ButtonsToBind != null && multiButtonBinding.ButtonsToBind.Any<GamepadButton>() && !multiButtonBinding.ButtonsToBind.Contains(multiButtonBinding.CurrentButtonToBind))
			{
				multiButtonBinding.CurrentButtonToBind = multiButtonBinding.ButtonsToBind.FirstOrDefault<GamepadButton>();
			}
			multiButtonBinding.ReEvaluateXBBinding();
		}

		public GamepadButtonsObservableCollection ButtonsToBind
		{
			get
			{
				return (GamepadButtonsObservableCollection)base.GetValue(MultiButtonBinding.ButtonsToBindProperty);
			}
			set
			{
				base.SetValue(MultiButtonBinding.ButtonsToBindProperty, value);
			}
		}

		public GamepadButtonsObservableCollection FilteredButtonsToBind
		{
			get
			{
				return (GamepadButtonsObservableCollection)base.GetValue(MultiButtonBinding.FilteredButtonsToBindProperty);
			}
			set
			{
				base.SetValue(MultiButtonBinding.FilteredButtonsToBindProperty, value);
			}
		}

		private static void CurrentButtonToBindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MultiButtonBinding multiButtonBinding = d as MultiButtonBinding;
			if (multiButtonBinding == null)
			{
				return;
			}
			if (multiButtonBinding.IsChangeCurrentBinding && multiButtonBinding.IsVisible && multiButtonBinding.CurrentButtonToBind != null)
			{
				GameProfilesService gameProfilesService = multiButtonBinding.GameProfilesService;
				if (gameProfilesService == null)
				{
					return;
				}
				BaseXBBindingCollection realCurrentBeingMappedBindingCollection = gameProfilesService.RealCurrentBeingMappedBindingCollection;
				if (realCurrentBeingMappedBindingCollection == null)
				{
					return;
				}
				realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(multiButtonBinding.CurrentButtonToBind), true);
			}
		}

		public GamepadButton CurrentButtonToBind
		{
			get
			{
				return (GamepadButton)base.GetValue(MultiButtonBinding.CurrentButtonToBindProperty);
			}
			set
			{
				base.SetValue(MultiButtonBinding.CurrentButtonToBindProperty, value);
			}
		}

		public bool IsInitialyFocused
		{
			get
			{
				return (bool)base.GetValue(MultiButtonBinding.IsInitialyFocusedProperty);
			}
			set
			{
				base.SetValue(MultiButtonBinding.IsInitialyFocusedProperty, value);
			}
		}

		public bool MappingSelectorIsVisible
		{
			get
			{
				return (bool)base.GetValue(MultiButtonBinding.MappingSelectorIsVisibleProperty);
			}
			set
			{
				base.SetValue(MultiButtonBinding.MappingSelectorIsVisibleProperty, value);
			}
		}

		public bool TurboIsVisible
		{
			get
			{
				return (bool)base.GetValue(MultiButtonBinding.TurboIsVisibleProperty);
			}
			set
			{
				base.SetValue(MultiButtonBinding.TurboIsVisibleProperty, value);
			}
		}

		public bool ToggleIsVisible
		{
			get
			{
				return (bool)base.GetValue(MultiButtonBinding.ToggleIsVisibleProperty);
			}
			set
			{
				base.SetValue(MultiButtonBinding.ToggleIsVisibleProperty, value);
			}
		}

		public MultiButtonBinding()
		{
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
			base.Unloaded += this.OnUnloaded;
			base.GotFocus += this.OnGotFocus;
		}

		private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			if (base.AllowGamepadHooks)
			{
				EventAggregator eventAggregator = base.EventAggregator;
				if (eventAggregator != null)
				{
					eventAggregator.GetEvent<GamepadKeyPressed>().Unsubscribe(new Action<List<GamepadButtonDescription>>(this.OnGamepadKeyPressed));
				}
			}
			bool isChangeCurrentBinding = base.IsChangeCurrentBinding;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			if (this.isLoaded)
			{
				return;
			}
			this.isLoaded = true;
			if (base.AllowGamepadHooks)
			{
				EventAggregator eventAggregator = base.EventAggregator;
				if (eventAggregator != null)
				{
					eventAggregator.GetEvent<GamepadKeyPressed>().Subscribe(new Action<List<GamepadButtonDescription>>(this.OnGamepadKeyPressed));
				}
			}
			this.SetCurrentBinding();
		}

		private void OnGotFocus(object sender, RoutedEventArgs routedEventArgs)
		{
			GameProfilesService gameProfilesService = base.GameProfilesService;
			if (((gameProfilesService != null) ? gameProfilesService.RealCurrentBeingMappedBindingCollection : null) == null)
			{
				return;
			}
			if (base.IsChangeCurrentBinding)
			{
				base.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(this.CurrentButtonToBind), true);
			}
		}

		private void OnGamepadKeyPressed(List<GamepadButtonDescription> xboxButtonDescriptions)
		{
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				List<GamepadButton> list = this.ButtonsToBind.Intersect(xboxButtonDescriptions.Select((GamepadButtonDescription xbdsc) => xbdsc.Button)).ToList<GamepadButton>();
				if (list.Any<GamepadButton>())
				{
					this.CurrentButtonToBind = list.FirstOrDefault<GamepadButton>();
				}
			}, true);
		}

		protected override void ReEvaluateXBBinding()
		{
			if (this.ButtonsToBind == null || base.BindingCollection == null)
			{
				return;
			}
			List<GamepadButton> list = base.BindingCollection.Keys.ToList<GamepadButton>();
			this.FilteredButtonsToBind = new GamepadButtonsObservableCollection(this.ButtonsToBind.Intersect(list));
			if (!this.FilteredButtonsToBind.Contains(this.CurrentButtonToBind))
			{
				this.CurrentButtonToBind = this.FilteredButtonsToBind.FirstOrDefault<GamepadButton>();
			}
		}

		protected override void OnServicesReady()
		{
			base.OnServicesReady();
			this.SetCurrentBinding();
		}

		private void SetCurrentBinding()
		{
			if (base.IsChangeCurrentBinding && this.IsInitialyFocused && this.FilteredButtonsToBind != null && base.GameProfilesService != null && base.GameProfilesService.RealCurrentBeingMappedBindingCollection != null)
			{
				if (base.GameProfilesService.RealCurrentBeingMappedBindingCollection.All((KeyValuePair<GamepadButton, XBBinding> kvp) => kvp.Key != this.CurrentButtonToBind))
				{
					this.CurrentButtonToBind = this.FilteredButtonsToBind.FirstOrDefault<GamepadButton>();
				}
				if (this.CurrentButtonToBind != null)
				{
					base.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(this.CurrentButtonToBind), true);
				}
			}
		}

		public static readonly DependencyProperty ButtonsToBindProperty = DependencyProperty.Register("ButtonsToBind", typeof(GamepadButtonsObservableCollection), typeof(MultiButtonBinding), new PropertyMetadata(null, new PropertyChangedCallback(MultiButtonBinding.ButtonsToBindChangedCallback)));

		public static readonly DependencyProperty FilteredButtonsToBindProperty = DependencyProperty.Register("FilteredButtonsToBind", typeof(GamepadButtonsObservableCollection), typeof(MultiButtonBinding), new PropertyMetadata(null));

		public static readonly DependencyProperty CurrentButtonToBindProperty = DependencyProperty.Register("CurrentButtonToBind", typeof(GamepadButton), typeof(MultiButtonBinding), new PropertyMetadata(0, new PropertyChangedCallback(MultiButtonBinding.CurrentButtonToBindChanged)));

		public static readonly DependencyProperty IsInitialyFocusedProperty = DependencyProperty.Register("IsInitialyFocused", typeof(bool), typeof(MultiButtonBinding), new PropertyMetadata(true));

		public static readonly DependencyProperty MappingSelectorIsVisibleProperty = DependencyProperty.Register("MappingSelectorIsVisible", typeof(bool), typeof(MultiButtonBinding), new PropertyMetadata(true));

		public static readonly DependencyProperty TurboIsVisibleProperty = DependencyProperty.Register("TurboIsVisible", typeof(bool), typeof(MultiButtonBinding), new PropertyMetadata(true));

		public static readonly DependencyProperty ToggleIsVisibleProperty = DependencyProperty.Register("ToggleIsVisible", typeof(bool), typeof(MultiButtonBinding), new PropertyMetadata(true));

		private bool isLoaded;
	}
}
