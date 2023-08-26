using System;
using System.Runtime.CompilerServices;
using System.Windows;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using Prism.Commands;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure.KeyBindingsModel.MacroBinding;

namespace reWASDUI.Infrastructure.KeyBindings.MacroBinding
{
	public abstract class BaseMacro : ZBindableBase
	{
		public bool IsBinding
		{
			get
			{
				return this.IsKeyBinding || this.IsGamepadBinding;
			}
		}

		public bool IsKeyBinding
		{
			get
			{
				return this is MacroKeyBinding;
			}
		}

		public bool IsGamepadBinding
		{
			get
			{
				return this is MacroGamepadBinding;
			}
		}

		public bool IsGamepadStick
		{
			get
			{
				return this is MacroGamepadStick;
			}
		}

		public bool IsGamepadStickCompensation
		{
			get
			{
				return this is MacroGamepadStickCompensation;
			}
		}

		public bool IsTouchSwipe
		{
			get
			{
				return this is MacroTouchSwipe;
			}
		}

		public bool IsTouchZoom
		{
			get
			{
				return this is MacroTouchZoom;
			}
		}

		public bool IsPause
		{
			get
			{
				return this is MacroPause;
			}
		}

		public bool IsBreak
		{
			get
			{
				return this is MacroBreak;
			}
		}

		public bool IsRumble
		{
			get
			{
				return this is MacroRumble;
			}
		}

		public bool IsVirtualGamepadBinding
		{
			get
			{
				return this.IsGamepadBinding || this.IsGamepadStick || this.IsTouchZoom || this.IsTouchSwipe;
			}
		}

		public abstract MacroItemType MacroItemType { get; }

		public MacroSequence MacroSequence { get; set; }

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsBeingDragged
		{
			get
			{
				return this._isBeingDragged;
			}
			set
			{
				this.SetProperty<bool>(ref this._isBeingDragged, value, "IsBeingDragged");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsBeingDraggedOverFromRight
		{
			get
			{
				return this._isBeingDraggedOverFromRight;
			}
			set
			{
				this.SetProperty<bool>(ref this._isBeingDraggedOverFromRight, value, "IsBeingDraggedOverFromRight");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsBeingDraggedOverFromLeft
		{
			get
			{
				return this._isBeingDraggedOverFromLeft;
			}
			set
			{
				this.SetProperty<bool>(ref this._isBeingDraggedOverFromLeft, value, "IsBeingDraggedOverFromLeft");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsHighlighted
		{
			get
			{
				return this._isHighlighted;
			}
			set
			{
				this.SetProperty<bool>(ref this._isHighlighted, value, "IsHighlighted");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isSelected, value, "IsSelected"))
				{
					this.MacroSequence.RaiseCanExecuteChangedForCommands();
				}
			}
		}

		protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (propertyName != "IsSelected")
			{
				bool isSelected = this.IsSelected;
				bool flag = base.SetProperty<T>(ref storage, value, propertyName);
				this.IsSelected = isSelected;
				return flag;
			}
			return base.SetProperty<T>(ref storage, value, propertyName);
		}

		public void RaiseCanExecuteChangedForCommands()
		{
			this.CopyCommand.RaiseCanExecuteChanged();
			this.PasteCommand.RaiseCanExecuteChanged();
			this.ReplaceCommand.RaiseCanExecuteChanged();
			this.RemoveCommand.RaiseCanExecuteChanged();
			this.AddBreakAfterCommand.RaiseCanExecuteChanged();
			this.AddRumbleAfterCommand.RaiseCanExecuteChanged();
			this.AddPauseAfterCommand.RaiseCanExecuteChanged();
		}

		public DelegateCommand RemoveCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._remove) == null)
				{
					delegateCommand = (this._remove = new DelegateCommand(new Action(this.Remove), delegate
					{
						MacroSequence macroSequence = this.MacroSequence;
						return macroSequence != null && macroSequence.Count > 0;
					}));
				}
				return delegateCommand;
			}
		}

		protected virtual void Remove()
		{
			if (this.MacroSequence == null)
			{
				return;
			}
			this.MacroSequence.Remove(this);
			this.MacroSequence.RemoveSelectedItems(true);
			this.MacroSequence.RemoveCrutchHoldItem();
			this.MacroSequence.AddCrutchHoldItem(false);
			this.MacroSequence = null;
		}

		public DelegateCommand AddPauseAfterCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._addPauseAfter) == null)
				{
					delegateCommand = (this._addPauseAfter = new DelegateCommand(new Action(this.AddPauseAfter), new Func<bool>(this.AddPauseAfterCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void AddPauseAfter()
		{
			MacroSequence macroSequence = this.MacroSequence;
			if (macroSequence == null)
			{
				return;
			}
			macroSequence.InsertOrAdd(new MacroPause(this.MacroSequence, 500U), this.MacroSequence.IndexOf(this) + 1, false);
		}

		private bool AddPauseAfterCanExecute()
		{
			MacroSequence macroSequence = this.MacroSequence;
			return macroSequence != null && !macroSequence.IsMultiSelectedItems;
		}

		public DelegateCommand AddBreakAfterCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._addBreakAfter) == null)
				{
					delegateCommand = (this._addBreakAfter = new DelegateCommand(new Action(this.AddBreakAfter), new Func<bool>(this.AddBreakAfterCanExecute)));
				}
				return delegateCommand;
			}
		}

		public string DisabledToolTip
		{
			get
			{
				if (this.MacroSequence.IsParentBindingToggle || this.MacroSequence.IsParentBindingTurbo)
				{
					return "Turbo and Toggle set limits on Combo. To use full Combo power, go Back to uncheck them.";
				}
				return null;
			}
		}

		private void AddBreakAfter()
		{
			MacroSequence macroSequence = this.MacroSequence;
			if (macroSequence == null)
			{
				return;
			}
			macroSequence.InsertOrAdd(new MacroBreak(this.MacroSequence), this.MacroSequence.IndexOf(this) + 1, false);
		}

		protected virtual bool AddBreakAfterCanExecute()
		{
			if (this.MacroSequence == null || this.MacroSequence.IsMultiSelectedItems)
			{
				return false;
			}
			bool flag = this.MacroItemType == 7;
			bool flag2 = this.MacroItemType == 5;
			bool flag3 = this.MacroItemType == 6;
			bool flag4 = this.MacroSequence.IsParentBindingToggle || this.MacroSequence.IsParentBindingTurbo;
			return !flag && ((!flag2 && !flag3) || !flag4);
		}

		public DelegateCommand AddRumbleAfterCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._addRumbleAfter) == null)
				{
					delegateCommand = (this._addRumbleAfter = new DelegateCommand(new Action(this.AddRumbleAfter), new Func<bool>(this.AddRumbleAfterCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void AddRumbleAfter()
		{
			MacroSequence macroSequence = this.MacroSequence;
			if (macroSequence == null)
			{
				return;
			}
			macroSequence.InsertOrAdd(new MacroRumble(this.MacroSequence, 150U, 80, 80, 60, 60), this.MacroSequence.IndexOf(this) + 1, false);
		}

		private bool AddRumbleAfterCanExecute()
		{
			MacroSequence macroSequence = this.MacroSequence;
			return macroSequence != null && !macroSequence.IsMultiSelectedItems;
		}

		public DelegateCommand CopyCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._copyCommand) == null)
				{
					delegateCommand = (this._copyCommand = new DelegateCommand(new Action(this.CopySelectedItems), new Func<bool>(this.CopySelectedItemsCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void CopySelectedItems()
		{
			reWASDApplicationCommands.CopyMacroSequence(this.MacroSequence);
		}

		private bool CopySelectedItemsCanExecute()
		{
			return reWASDApplicationCommands.CanCopyMacroSequence(this.MacroSequence) && (this.MacroSequence.IsHoldUntilRelease || this.MacroSequence.IsMultiSelectedItems || (!(this is MacroGamepadBinding) && !(this is MacroKeyBinding)));
		}

		public Visibility CopyCommandVisibility
		{
			get
			{
				if (this.MacroSequence.IsHoldUntilRelease)
				{
					return Visibility.Visible;
				}
				if (reWASDApplicationCommands.CanCopyMacroSequence(this.MacroSequence) && (this.MacroSequence.IsMultiSelectedItems || (!(this is MacroGamepadBinding) && !(this is MacroKeyBinding))))
				{
					return Visibility.Visible;
				}
				return Visibility.Collapsed;
			}
		}

		public DelegateCommand PasteCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._pasteCommand) == null)
				{
					delegateCommand = (this._pasteCommand = new DelegateCommand(new Action(this.PasteSelectedItems), () => reWASDApplicationCommands.CanPasteMacroSequence()));
				}
				return delegateCommand;
			}
		}

		private void PasteSelectedItems()
		{
			reWASDApplicationCommands.PasteMacroSequence(this.MacroSequence, this.MacroSequence.IndexOf(this) + 1);
		}

		public DelegateCommand ReplaceCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._replaceCommand) == null)
				{
					delegateCommand = (this._replaceCommand = new DelegateCommand(delegate
					{
						this.ReplaceSelectedItems();
					}, () => reWASDApplicationCommands.CanPasteMacroSequence()));
				}
				return delegateCommand;
			}
		}

		private void ReplaceSelectedItems()
		{
			if (this.MacroSequence != null)
			{
				reWASDApplicationCommands.ReplaceMacroSequence(this.MacroSequence, this.MacroSequence.IndexOf(this) + 1);
			}
		}

		public BaseMacro(MacroSequence macroSequence)
		{
			this.MacroSequence = macroSequence;
		}

		public abstract BaseMacro Clone(MacroSequence hostMacroSequence);

		public abstract BaseMacro Clone(MacroSequence hostMacroSequence);

		public static BaseMacro CloneFromModel(MacroSequence hostMacroSequence, BaseMacro model)
		{
			MacroGamepadBinding macroGamepadBinding = model as MacroGamepadBinding;
			if (macroGamepadBinding != null)
			{
				return new MacroGamepadBinding(hostMacroSequence, macroGamepadBinding.GamepadButtonDescription, macroGamepadBinding.MacroKeyType)
				{
					DeflectionPercentage = macroGamepadBinding.DeflectionPercentage
				};
			}
			MacroTouchZoom macroTouchZoom = model as MacroTouchZoom;
			if (macroTouchZoom != null)
			{
				return new MacroTouchZoom(hostMacroSequence, macroTouchZoom.Duration, macroTouchZoom.GamepadButton);
			}
			MacroTouchSwipe macroTouchSwipe = model as MacroTouchSwipe;
			if (macroTouchSwipe != null)
			{
				return new MacroTouchSwipe(hostMacroSequence, macroTouchSwipe.Duration, macroTouchSwipe.GamepadButton);
			}
			MacroRumble macroRumble = model as MacroRumble;
			if (macroRumble != null)
			{
				return new MacroRumble(hostMacroSequence, macroRumble.Duration, macroRumble.MotorLeft, macroRumble.MotorRight, macroRumble.TriggerLeft, macroRumble.TriggerRight);
			}
			MacroKeyBinding macroKeyBinding = model as MacroKeyBinding;
			if (macroKeyBinding != null)
			{
				return new MacroKeyBinding(hostMacroSequence, macroKeyBinding.KeyScanCode, macroKeyBinding.MacroKeyType);
			}
			if (model is MacroBreak)
			{
				return new MacroBreak(hostMacroSequence);
			}
			MacroCrutchHoldUntillRelease macroCrutchHoldUntillRelease = model as MacroCrutchHoldUntillRelease;
			if (macroCrutchHoldUntillRelease != null)
			{
				return new MacroCrutchHoldUntillRelease(hostMacroSequence, macroCrutchHoldUntillRelease.GamepadButtonDescription, macroCrutchHoldUntillRelease.KeyScanCode);
			}
			MacroPause macroPause = model as MacroPause;
			if (macroPause != null)
			{
				return new MacroPause(hostMacroSequence, macroPause.Duration);
			}
			MacroGamepadStick macroGamepadStick = model as MacroGamepadStick;
			if (macroGamepadStick != null)
			{
				return new MacroGamepadStick(hostMacroSequence, macroGamepadStick.GamepadButtonDescription, macroGamepadStick.MacroKeyType)
				{
					DeflectionPercentage = macroGamepadStick.DeflectionPercentage,
					Relative = macroGamepadStick.Relative,
					GamepadIndex = macroGamepadStick.GamepadIndex
				};
			}
			MacroGamepadStickCompensation macroGamepadStickCompensation = model as MacroGamepadStickCompensation;
			if (macroGamepadStickCompensation == null)
			{
				return null;
			}
			return new MacroGamepadStickCompensation(hostMacroSequence, macroGamepadStickCompensation.MacroKeyType)
			{
				GamepadIndex = macroGamepadStickCompensation.GamepadIndex,
				LeftStickX = macroGamepadStickCompensation.LeftStickX,
				LeftStickY = macroGamepadStickCompensation.LeftStickY,
				RightStickX = macroGamepadStickCompensation.RightStickX,
				RightStickY = macroGamepadStickCompensation.RightStickY
			};
		}

		private bool _isBeingDragged;

		private bool _isBeingDraggedOverFromRight;

		private bool _isBeingDraggedOverFromLeft;

		private bool _isHighlighted;

		private bool _isSelected;

		public bool IsRightClicked;

		private DelegateCommand _remove;

		private DelegateCommand _addPauseAfter;

		private DelegateCommand _addBreakAfter;

		private DelegateCommand _addRumbleAfter;

		private DelegateCommand _copyCommand;

		private DelegateCommand _pasteCommand;

		private DelegateCommand _replaceCommand;
	}
}
