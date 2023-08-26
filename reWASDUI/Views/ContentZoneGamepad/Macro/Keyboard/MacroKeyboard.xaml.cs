using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Prism.Commands;
using reWASDUI.Infrastructure.KeyBindings.MacroBinding;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;

namespace reWASDUI.Views.ContentZoneGamepad.Macro.Keyboard
{
	public partial class MacroKeyboard : UserControl
	{
		public MacroSequence MacroSequence
		{
			get
			{
				return (MacroSequence)base.GetValue(MacroKeyboard.MacroSequenceProperty);
			}
			set
			{
				base.SetValue(MacroKeyboard.MacroSequenceProperty, value);
			}
		}

		public DelegateCommand<KeyScanCodeV2> KeyboardButtonClickCommand
		{
			get
			{
				DelegateCommand<KeyScanCodeV2> delegateCommand;
				if ((delegateCommand = this._keyboardButtonClickCommand) == null)
				{
					delegateCommand = (this._keyboardButtonClickCommand = new DelegateCommand<KeyScanCodeV2>(new Action<KeyScanCodeV2>(this.KeyboardButtonClick)));
				}
				return delegateCommand;
			}
		}

		public MacroKeyboard()
		{
			this.InitializeComponent();
		}

		private void KeyboardButtonClick(KeyScanCodeV2 key)
		{
			if (key == null)
			{
				return;
			}
			this.MacroSequence.AddKeyScanCodeAccordingToSequenceType(key);
		}

		public static readonly DependencyProperty MacroSequenceProperty = DependencyProperty.Register("MacroSequence", typeof(MacroSequence), typeof(MacroKeyboard), new PropertyMetadata(null));

		private DelegateCommand<KeyScanCodeV2> _keyboardButtonClickCommand;
	}
}
