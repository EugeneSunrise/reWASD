using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using reWASDCommon.Infrastructure;
using reWASDUI.DataModels;

namespace reWASDUI.Controls
{
	public class AddShiftButton : UserControlWithDropDown, IComponentConnector, IStyleConnector
	{
		public bool IsDynamicShifts
		{
			get
			{
				return Constants.DynamicShifts;
			}
		}

		public AddShiftButton()
		{
			this.InitializeComponent();
		}

		private void BtnAddShift_OnClick(object sender, RoutedEventArgs e)
		{
			if (App.GameProfilesService.PresetsCollection.Count > 0)
			{
				base.SetCurrentValue(UserControlWithDropDown.IsDropDownOpenProperty, !base.IsDropDownOpen);
				return;
			}
			App.GameProfilesService.CurrentGame.CurrentConfig.AddShiftCommand.Execute();
		}

		private async void ConfigsList_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Right)
			{
				e.Handled = true;
			}
			else
			{
				try
				{
					object dataContext = (ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem).DataContext;
					ConfigVM config = dataContext as ConfigVM;
					if (config != null)
					{
						base.IsDropDownOpen = false;
						TaskAwaiter<bool> taskAwaiter = config.ReadConfigFromJsonIfNotLoaded().GetAwaiter();
						if (!taskAwaiter.IsCompleted)
						{
							await taskAwaiter;
							TaskAwaiter<bool> taskAwaiter2;
							taskAwaiter = taskAwaiter2;
							taskAwaiter2 = default(TaskAwaiter<bool>);
						}
						if (!taskAwaiter.GetResult())
						{
							DTMessageBox.Show(DTLocalization.GetString(12578), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
							await App.GameProfilesService.FillPresetsCollection(true);
							return;
						}
						App.GameProfilesService.CurrentGame.CurrentConfig.ApplyPreset(config);
					}
					config = null;
				}
				catch (Exception)
				{
				}
			}
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
			Uri uri = new Uri("/reWASD;component/controls/dropdowncontrols/addshiftbutton.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
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
				((SVGButton)target).Click += this.BtnAddShift_OnClick;
				return;
			}
			if (connectionId != 2)
			{
				return;
			}
			((ListBox)target).PreviewMouseDown += this.ConfigsList_OnPreviewMouseDown;
		}

		private bool _contentLoaded;
	}
}
