using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.DataModels;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.KeyBindingsModel.MacroBinding;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Infrastructure.KeyBindings.MacroBinding
{
	public class MacroGamepadBinding : BaseMacroBinding
	{
		public byte GamepadIndex
		{
			get
			{
				return this._gamepadIndex;
			}
			set
			{
				this.SetProperty<byte>(ref this._gamepadIndex, value, "GamepadIndex");
			}
		}

		public short DeflectionPercentage
		{
			get
			{
				return this._deflectionPercentage;
			}
			set
			{
				this.SetProperty<short>(ref this._deflectionPercentage, value, "DeflectionPercentage");
			}
		}

		public bool CanChangeDeflectionPercentage
		{
			get
			{
				if (base.MacroKeyType != null)
				{
					return false;
				}
				VirtualGamepadType? virtualGamepadType2;
				VirtualGamepadType virtualGamepadType3;
				if (GamepadButtonExtensions.IsAnyTriggerPress(this.GamepadButtonDescription.Button))
				{
					GameVM currentGame = App.GameProfilesService.CurrentGame;
					bool flag;
					if (currentGame == null)
					{
						flag = true;
					}
					else
					{
						ConfigVM currentConfig = currentGame.CurrentConfig;
						VirtualGamepadType? virtualGamepadType;
						if (currentConfig == null)
						{
							virtualGamepadType = null;
						}
						else
						{
							ConfigData configData = currentConfig.ConfigData;
							virtualGamepadType = ((configData != null) ? new VirtualGamepadType?(configData.VirtualGamepadType) : null);
						}
						virtualGamepadType2 = virtualGamepadType;
						virtualGamepadType3 = 4;
						flag = !((virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null));
					}
					if (flag)
					{
						return true;
					}
				}
				if (!GamepadButtonExtensions.IsDS3AnalogButton(this.GamepadButtonDescription.Button))
				{
					return false;
				}
				GameVM currentGame2 = App.GameProfilesService.CurrentGame;
				if (currentGame2 == null)
				{
					return false;
				}
				ConfigVM currentConfig2 = currentGame2.CurrentConfig;
				VirtualGamepadType? virtualGamepadType4;
				if (currentConfig2 == null)
				{
					virtualGamepadType4 = null;
				}
				else
				{
					ConfigData configData2 = currentConfig2.ConfigData;
					virtualGamepadType4 = ((configData2 != null) ? new VirtualGamepadType?(configData2.VirtualGamepadType) : null);
				}
				virtualGamepadType2 = virtualGamepadType4;
				virtualGamepadType3 = 3;
				return (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
			}
		}

		public override MacroItemType MacroItemType
		{
			get
			{
				return 1;
			}
		}

		public GamepadButtonDescription GamepadButtonDescription { get; set; }

		public new KeyScanCodeV2 KeyScanCode
		{
			get
			{
				return KeyScanCodeV2.FindKeyScanCodeByGamepadButton(this.GamepadButtonDescription.Button);
			}
		}

		public MacroGamepadBinding(MacroSequence macroSequence, GamepadButtonDescription gamepadButtonDescription, MacroKeyType macroType)
			: base(macroSequence, macroType)
		{
			this.GamepadIndex = 0;
			this.GamepadButtonDescription = gamepadButtonDescription;
			this.DeflectionPercentage = 100;
			base.PropertyChanged += delegate([Nullable(2)] object sender, PropertyChangedEventArgs args)
			{
				if (args.PropertyName == "IsBeingDragged" && base.Twin != null)
				{
					base.Twin.IsHighlighted = base.IsBeingDragged;
				}
			};
		}

		public MacroGamepadBinding(MacroSequence macroSequence, int gbEnumIndex, MacroKeyType macroType)
			: this(macroSequence, GamepadButtonDescription.GetGamepadButtonDescriptionByGamepadButtonEnumIndex(gbEnumIndex), macroType)
		{
		}

		public override bool IsBindingEqualTo(BaseMacroBinding bmb)
		{
			MacroGamepadBinding macroGamepadBinding = bmb as MacroGamepadBinding;
			return macroGamepadBinding != null && this.GamepadButtonDescription.Equals(macroGamepadBinding.GamepadButtonDescription);
		}

		public override BaseMacro Clone(MacroSequence hostMacroSequence)
		{
			return new MacroGamepadBinding(hostMacroSequence, this.GamepadButtonDescription, base.MacroKeyType)
			{
				DeflectionPercentage = this.DeflectionPercentage
			};
		}

		public override BaseMacro Clone(MacroSequence hostMacroSequence)
		{
			return new MacroGamepadBinding(this.GamepadButtonDescription, base.MacroKeyType)
			{
				DeflectionPercentage = this.DeflectionPercentage
			};
		}

		private byte _gamepadIndex;

		private short _deflectionPercentage;
	}
}
