using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls.Buttons;
using reWASDCommon.Infrastructure;

namespace reWASDUI.Controls
{
	public class ShiftSelectorControl : UserControlWithDropDown, IComponentConnector, IStyleConnector
	{
		public bool IsDynamicShifts
		{
			get
			{
				return Constants.DynamicShifts;
			}
		}

		public ShiftSelectorControl()
		{
			this.InitializeComponent();
		}

		private void BtnShowShiftsList_OnClick(object sender, RoutedEventArgs e)
		{
			base.SetCurrentValue(UserControlWithDropDown.IsDropDownOpenProperty, !base.IsDropDownOpen);
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
			Uri uri = new Uri("/reWASD;component/controls/dropdowncontrols/shiftselectorcontrol.xaml", UriKind.Relative);
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
				((SVGButton)target).Click += this.BtnShowShiftsList_OnClick;
			}
		}

		private bool _contentLoaded;
	}
}
