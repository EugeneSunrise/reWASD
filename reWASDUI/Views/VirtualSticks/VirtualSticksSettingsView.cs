using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using reWASDUI.Controls;
using reWASDUI.Infrastructure;

namespace reWASDUI.Views.VirtualSticks
{
	public class VirtualSticksSettingsView : BaseServicesDataContextControl, INotifyPropertyChanged, IComponentConnector, IStyleConnector
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(name));
		}

		public VirtualSticksSettingsView()
		{
			this.InitializeComponent();
		}

		private void ToggleSticks_OnClick(object sender, RoutedEventArgs e)
		{
			App.EventAggregator.GetEvent<CloseAllPopups>().Publish(null);
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
			Uri uri = new Uri("/reWASD;component/views/virtualdevicesettings/virtualstickssettingsview.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			this._contentLoaded = true;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				((ToggleButton)target).Click += this.ToggleSticks_OnClick;
			}
		}

		private bool _contentLoaded;
	}
}
