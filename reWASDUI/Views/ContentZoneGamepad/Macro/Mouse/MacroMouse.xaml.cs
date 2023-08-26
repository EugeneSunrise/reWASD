using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Prism.Commands;
using reWASDUI.Infrastructure.KeyBindings.MacroBinding;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;

namespace reWASDUI.Views.ContentZoneGamepad.Macro.Mouse
{
	public partial class MacroMouse : UserControl
	{
		public MacroSequence MacroSequence
		{
			get
			{
				return (MacroSequence)base.GetValue(MacroMouse.MacroSequenceProperty);
			}
			set
			{
				base.SetValue(MacroMouse.MacroSequenceProperty, value);
			}
		}

		public DelegateCommand<MouseButton?> MouseButtonClickCommand
		{
			get
			{
				DelegateCommand<MouseButton?> delegateCommand;
				if ((delegateCommand = this._mouseButtonClick) == null)
				{
					delegateCommand = (this._mouseButtonClick = new DelegateCommand<MouseButton?>(new Action<MouseButton?>(this.MouseButtonClick)));
				}
				return delegateCommand;
			}
		}

		public MacroMouse()
		{
			this.InitializeComponent();
		}

		private void MouseButtonClick(MouseButton? mouseButton)
		{
			if (mouseButton == null)
			{
				return;
			}
			this.MacroSequence.AddKeyScanCodeAccordingToSequenceType(KeyScanCodeV2.FindKeyScanCodeByMouseButton(mouseButton.Value));
		}

		public static readonly DependencyProperty MacroSequenceProperty = DependencyProperty.Register("MacroSequence", typeof(MacroSequence), typeof(MacroMouse), new PropertyMetadata(null));

		private DelegateCommand<MouseButton?> _mouseButtonClick;
	}
}
