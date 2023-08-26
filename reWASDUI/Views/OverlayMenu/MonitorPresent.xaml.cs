using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DiscSoft.NET.Common.Properties;

namespace reWASDUI.Views.OverlayMenu
{
	public partial class MonitorPresent : UserControl, INotifyPropertyChanged
	{
		public MonitorPresent()
		{
			this.InitializeComponent();
		}

		public int WidhtM
		{
			get
			{
				return (int)base.GetValue(MonitorPresent.WidhtMProperty);
			}
			set
			{
				base.SetValue(MonitorPresent.WidhtMProperty, value);
			}
		}

		public int Number
		{
			get
			{
				return (int)base.GetValue(MonitorPresent.NumberProperty);
			}
			set
			{
				base.SetValue(MonitorPresent.NumberProperty, value);
			}
		}

		public int HeightM
		{
			get
			{
				return (int)base.GetValue(MonitorPresent.HeightMProperty);
			}
			set
			{
				base.SetValue(MonitorPresent.HeightMProperty, value);
			}
		}

		public string NameM
		{
			get
			{
				return (string)base.GetValue(MonitorPresent.NameMProperty);
			}
			set
			{
				base.SetValue(MonitorPresent.NameMProperty, value);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.Owner.ClearAllActive();
			this.IsActive = true;
			this.Owner.SaveActive();
		}

		private void OnMouseEnter(object sender, MouseEventArgs mouseEventArgs)
		{
			this.isUnderMouse = true;
			this.UpdateProperty();
		}

		private void OnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
		{
			this.isUnderMouse = false;
			this.UpdateProperty();
		}

		public void UpdateProperty()
		{
			this.OnPropertyChanged("Thickness");
			this.OnPropertyChanged("BackgroundInner");
		}

		public int Thickness
		{
			get
			{
				if (!this.isUnderMouse)
				{
					return 1;
				}
				return 3;
			}
		}

		public SolidColorBrush BackgroundInner
		{
			get
			{
				SolidColorBrush solidColorBrush = new SolidColorBrush((Application.Current.TryFindResource("CreamBrush") as SolidColorBrush).Color);
				if (this.IsActive)
				{
					solidColorBrush.Opacity = 0.3;
				}
				else if (this.isUnderMouse)
				{
					solidColorBrush.Opacity = 0.1;
				}
				else
				{
					solidColorBrush.Opacity = 0.01;
				}
				return solidColorBrush;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				((Grid)target).MouseLeftButtonUp += this.OnMouseLeftButtonUp;
				((Grid)target).MouseEnter += this.OnMouseEnter;
				((Grid)target).MouseLeave += this.OnMouseLeave;
			}
		}

		public static readonly DependencyProperty WidhtMProperty = DependencyProperty.Register("WidhtM", typeof(int), typeof(MonitorPresent), new PropertyMetadata(100));

		public static readonly DependencyProperty NumberProperty = DependencyProperty.Register("Number", typeof(int), typeof(MonitorPresent), new PropertyMetadata(1));

		public static readonly DependencyProperty HeightMProperty = DependencyProperty.Register("HeightM", typeof(int), typeof(MonitorPresent), new PropertyMetadata(100));

		public static readonly DependencyProperty NameMProperty = DependencyProperty.Register("NameM", typeof(string), typeof(MonitorPresent), new PropertyMetadata(""));

		public bool IsActive;

		public RadialMenuPreferences Owner;

		private bool isUnderMouse;
	}
}
