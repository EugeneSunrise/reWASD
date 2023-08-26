using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Prism.Commands;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings.MacroBinding;
using XBEliteWPF.Infrastructure;

namespace reWASDUI.Views.ContentZoneGamepad.Macro.Gamepad
{
	public partial class MacroGamepad : UserControl
	{
		public MacroSequence MacroSequence
		{
			get
			{
				return (MacroSequence)base.GetValue(MacroGamepad.MacroSequenceProperty);
			}
			set
			{
				base.SetValue(MacroGamepad.MacroSequenceProperty, value);
			}
		}

		public DelegateCommand<GamepadButton?> GamepadButtonClickCommand
		{
			get
			{
				DelegateCommand<GamepadButton?> delegateCommand;
				if ((delegateCommand = this._gamepadButtonButtonClick) == null)
				{
					delegateCommand = (this._gamepadButtonButtonClick = new DelegateCommand<GamepadButton?>(new Action<GamepadButton?>(this.MacroGamepadButtonClick)));
				}
				return delegateCommand;
			}
		}

		public MacroGamepad()
		{
			this.InitializeComponent();
		}

		private void MacroGamepadButtonClick(GamepadButton? gamepadButton)
		{
			if (gamepadButton == null)
			{
				return;
			}
			this.MacroSequence.AddGamepadButtonAccordingToSequenceType(GamepadButtonDescription.FindGamepadButtonDescriptionByGamepadButton(gamepadButton.Value));
		}

		public static readonly DependencyProperty MacroSequenceProperty = DependencyProperty.Register("MacroSequence", typeof(MacroSequence), typeof(MacroGamepad), new PropertyMetadata(null));

		private DelegateCommand<GamepadButton?> _gamepadButtonButtonClick;
	}
}
