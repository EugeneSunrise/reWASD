using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using Prism.Ioc;
using reWASDUI.Services.Interfaces;
using reWASDUI.Utils;

namespace reWASDUI.Views.ContentZoneGamepad.AdvancedStick
{
	[DoNotCacheView]
	public partial class AdvancedStickSettings : BaseDirectionalAnalogGroupUserControl
	{
		public AdvancedStickSettings()
		{
			this.InitializeComponent();
			base.Loaded += delegate(object sender, RoutedEventArgs args)
			{
				App.KeyPressedPollerService.SuspendPollingUntilStarted();
			};
			base.IsVisibleChanged += this.AdvancedStickSettings_IsVisibleChanged;
		}

		private void AdvancedStickSettings_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				IContainerProviderExtensions.Resolve<IGuiScaleService>(App.Container).ScaleValue = 1.0;
				App.KeyPressedPollerService.SuspendPollingUntilStarted();
				return;
			}
			App.KeyPressedPollerService.StartPolling(true);
			IContainerProviderExtensions.Resolve<IGuiScaleService>(App.Container).CalculateScaleValue();
		}
	}
}
