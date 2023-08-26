using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using reWASDUI.Controls;
using XBEliteWPF.Infrastructure;

namespace reWASDUI.Views.VirtualSticks
{
	public class VirtualGyroSettingsView : BaseServicesDataContextControl, IComponentConnector
	{
		public VirtualGyroSettings VirtualGyroSettings
		{
			get
			{
				return (VirtualGyroSettings)base.GetValue(VirtualGyroSettingsView.VirtualGyroSettingsProperty);
			}
			set
			{
				base.SetValue(VirtualGyroSettingsView.VirtualGyroSettingsProperty, value);
			}
		}

		private static void OnVirtualGyroSettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VirtualGyroSettingsView virtualGyroSettingsView = d as VirtualGyroSettingsView;
		}

		public VirtualGyroSettingsView()
		{
			this.InitializeComponent();
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
			Uri uri = new Uri("/reWASD;component/views/virtualdevicesettings/virtualgyrosettingsview.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			this._contentLoaded = true;
		}

		public static readonly DependencyProperty VirtualGyroSettingsProperty = DependencyProperty.Register("VirtualGyroSettings", typeof(VirtualGyroSettings), typeof(VirtualGyroSettingsView), new PropertyMetadata(null, new PropertyChangedCallback(VirtualGyroSettingsView.OnVirtualGyroSettingsChanged)));

		private bool _contentLoaded;
	}
}
