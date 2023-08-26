using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.SecondaryWindows;
using Prism.Commands;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.KeyBindingsModel.MacroBinding;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Infrastructure.KeyBindings.MacroBinding
{
	public class MacroSequence : SortableObservableCollection<BaseMacro>, IDisposable
	{
		public bool HasComboWithAdvancedFeature
		{
			get
			{
				if (this.IsOnetime)
				{
					return this.Count((BaseMacro m) => m.IsGamepadBinding) / 2 + this.Count((BaseMacro m) => m.IsGamepadStick) + this.Count((BaseMacro m) => m.IsTouchSwipe) + this.Count((BaseMacro m) => m.IsTouchZoom) + this.Count((BaseMacro m) => m.IsGamepadBinding && (GamepadButtonExtensions.IsTouchpadOneFingerTouch(((MacroGamepadBinding)m).GamepadButtonDescription.Button) || GamepadButtonExtensions.IsTouchpadTwoFingersTouch(((MacroGamepadBinding)m).GamepadButtonDescription.Button))) > 0;
				}
				if (this.Count((BaseMacro m) => m.IsGamepadBinding) + this.Count((BaseMacro m) => m.IsGamepadStick) + this.Count((BaseMacro m) => m.IsTouchSwipe) + this.Count((BaseMacro m) => m.IsTouchZoom) + this.Count((BaseMacro m) => m.IsGamepadBinding && (GamepadButtonExtensions.IsTouchpadOneFingerTouch(((MacroGamepadBinding)m).GamepadButtonDescription.Button) || GamepadButtonExtensions.IsTouchpadTwoFingersTouch(((MacroGamepadBinding)m).GamepadButtonDescription.Button))) <= 0)
				{
					return this.Any((BaseMacro x) => x.IsTouchSwipe && (((MacroTouchSwipe)x).Duration != 250U || GamepadButtonExtensions.IsTouchpadTwoFingersSwipe(((MacroTouchSwipe)x).GamepadButton)));
				}
				return true;
			}
		}

		public bool IsHoldUntilRelease
		{
			get
			{
				return this.MacroSequenceType == 0;
			}
		}

		public bool IsOnetime
		{
			get
			{
				return this.MacroSequenceType == 1;
			}
		}

		public bool IsMultiSelectedItems
		{
			get
			{
				return this.Count((BaseMacro item) => item.IsSelected) > 1;
			}
		}

		public bool SupressOnCollectionChanged
		{
			get
			{
				return this._supressOnCollectionChanged;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._supressOnCollectionChanged, value, "SupressOnCollectionChanged") && !value)
				{
					this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				}
			}
		}

		public int MacroCrutchCount
		{
			get
			{
				return this.Count((BaseMacro p) => p is MacroCrutchHoldUntillRelease);
			}
		}

		public bool HasRumble
		{
			get
			{
				return this.Any((BaseMacro p) => p is MacroRumble);
			}
		}

		public bool IsParentBindingTurbo
		{
			get
			{
				return this._isParentBindingTurbo;
			}
			set
			{
				this.SetProperty<bool>(ref this._isParentBindingTurbo, value, "IsParentBindingTurbo");
			}
		}

		public bool IsParentBindingToggle
		{
			get
			{
				return this._isParentBindingToggle;
			}
			set
			{
				this.SetProperty<bool>(ref this._isParentBindingToggle, value, "IsParentBindingToggle");
			}
		}

		public MacroSequenceType MacroSequenceType
		{
			get
			{
				return this._macroSequenceType;
			}
			set
			{
				if (this.SetProperty<MacroSequenceType>(ref this._macroSequenceType, value, "MacroSequenceType"))
				{
					this.OnPropertyChanged("IsHoldUntilRelease");
					this.OnPropertyChanged("IsOnetime");
					this.IsChanged = true;
				}
			}
		}

		public int RepeatCount
		{
			get
			{
				return this._repeatCount;
			}
			set
			{
				if (this.SetProperty<int>(ref this._repeatCount, value, "RepeatCount"))
				{
					this.OnPropertyChanged("RepeatInterval");
					this.OnPropertyChanged("RepeatIntervalIsEnabled");
					this.IsChanged = true;
					PropertyChangedEventHandler macroSequencePropertyChanged = this.MacroSequencePropertyChanged;
					if (macroSequencePropertyChanged == null)
					{
						return;
					}
					macroSequencePropertyChanged(this, null);
				}
			}
		}

		public string TotalComboTime
		{
			get
			{
				return this._totalComboTime;
			}
			set
			{
				this.SetProperty<string>(ref this._totalComboTime, value, "TotalComboTime");
			}
		}

		private void CalculateTotalComboTime()
		{
			if (!this.IsOnetime)
			{
				return;
			}
			if (!this.Any((BaseMacro x) => x.IsBreak))
			{
				this.TotalComboTime = this.CalculateComboTimeToBreak(0).ToString() + " " + DTLocalization.GetString(12117);
				return;
			}
			this.TotalComboTime = string.Empty;
			int num = this.Count((BaseMacro x) => x.IsBreak) + 1;
			for (int i = 1; i <= num; i++)
			{
				this.TotalComboTime = this.TotalComboTime + this.CalculateComboTimeToBreak(i).ToString() + " " + DTLocalization.GetString(12117);
				if (i != num)
				{
					this.TotalComboTime += "; ";
				}
			}
		}

		private int CalculateComboTimeToBreak(int breakCounter = 0)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < base.Count; i++)
			{
				BaseMacro baseMacro = base[i];
				if (baseMacro.IsBreak && breakCounter != 0)
				{
					num2++;
					if (breakCounter == num2)
					{
						break;
					}
				}
				else if (breakCounter - 1 == num2 || breakCounter == 0)
				{
					if (baseMacro is BaseDurationMacro)
					{
						num += (int)(baseMacro as BaseDurationMacro).Duration;
					}
					else if (!(baseMacro is MacroGamepadStickCompensation) && i > 0 && i < base.Count && (baseMacro.IsKeyBinding || baseMacro.IsVirtualGamepadBinding) && (base[i - 1].IsKeyBinding || base[i - 1].IsVirtualGamepadBinding))
					{
						num3++;
					}
				}
			}
			int num4 = num3 * this.KeyDelay;
			if (num4 < 0)
			{
				num4 = 0;
			}
			num += num4;
			if (this.RepeatCount > 1)
			{
				int num5 = (this.RepeatCount - 1) * this.RepeatInterval;
				num = num * this.RepeatCount + num5;
			}
			return num;
		}

		public bool RepeatIntervalIsEnabled
		{
			get
			{
				return this.RepeatCount > 1;
			}
		}

		public int RepeatInterval
		{
			get
			{
				if (this.RepeatCount == 1)
				{
					return 0;
				}
				return this._repeatInterval;
			}
			set
			{
				if (this.SetProperty<int>(ref this._repeatInterval, value, "RepeatInterval"))
				{
					this.IsChanged = true;
				}
			}
		}

		public int TurboDelay
		{
			get
			{
				return this._turboDelay;
			}
			set
			{
				if (this.SetProperty<int>(ref this._turboDelay, value, "TurboDelay"))
				{
					this.IsChanged = true;
				}
			}
		}

		public int KeyDelay
		{
			get
			{
				return this._keyDelay;
			}
			set
			{
				if (value == 0 && MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(11455), MessageBoxButton.YesNo, MessageBoxImage.Exclamation, "ConfirmZeroDelayBetweenKeys", MessageBoxResult.Yes, false, 0.0, null, null, null, null, null, null) != MessageBoxResult.Yes)
				{
					this.OnPropertyChanged("KeyDelay");
					return;
				}
				if (this.SetProperty<int>(ref this._keyDelay, value, "KeyDelay"))
				{
					this.IsChanged = true;
				}
			}
		}

		public int KeyDelaySilentChange
		{
			set
			{
				if (this._keyDelay == value)
				{
					return;
				}
				this._keyDelay = value;
				this.OnPropertyChanged("KeyDelay");
			}
		}

		public int MacroCleanThreshold
		{
			get
			{
				return this._macroCleanThreshold;
			}
			set
			{
				if (this.SetProperty<int>(ref this._macroCleanThreshold, value, "MacroCleanThreshold"))
				{
					this.IsChanged = true;
				}
			}
		}

		public int MacroPauseBetweenItems
		{
			get
			{
				return this._macroPauseBetweenItems;
			}
			set
			{
				if (this.SetProperty<int>(ref this._macroPauseBetweenItems, value, "MacroPauseBetweenItems"))
				{
					this.IsChanged = true;
				}
			}
		}

		public ActivatorType ActivatorType
		{
			get
			{
				return this._activatorType;
			}
			set
			{
				this.SetProperty<ActivatorType>(ref this._activatorType, value, "ActivatorType");
			}
		}

		public bool IsSingleActivator
		{
			get
			{
				return this.ActivatorType == 0;
			}
		}

		public bool IsLongActivator
		{
			get
			{
				return this.ActivatorType == 1;
			}
		}

		public bool IsDoubleActivator
		{
			get
			{
				return this.ActivatorType == 2;
			}
		}

		public bool IsTripleActivator
		{
			get
			{
				return this.ActivatorType == 3;
			}
		}

		public bool IsStartActivator
		{
			get
			{
				return this.ActivatorType == 4;
			}
		}

		public bool IsReleaseActivator
		{
			get
			{
				return this.ActivatorType == 5;
			}
		}

		public bool IsChanged
		{
			get
			{
				return this._isChanged;
			}
			set
			{
				if (value)
				{
					this._dontBotherMeWithStickCompensation = false;
				}
				if (this.SetProperty<bool>(ref this._isChanged, value, "IsChanged"))
				{
					PropertyChangedEventHandler macroSequencePropertyChanged = this.MacroSequencePropertyChanged;
					if (macroSequencePropertyChanged != null)
					{
						macroSequencePropertyChanged(this, new PropertyChangedEventArgs("IsChanged"));
					}
				}
				if (this.IsOnetime)
				{
					this.CalculateTotalComboTime();
				}
			}
		}

		public bool IsGamepadBindingPresent
		{
			get
			{
				return this.GamepadBindingNodePresent;
			}
		}

		public bool IsSwipeOrZoomBindingPresent
		{
			get
			{
				return this.SwipeOrZoomBindingNodePresent;
			}
		}

		public bool IsVirtualTouchpadBindingPresent
		{
			get
			{
				return this.VirtualTouchpadBindingNodePresent;
			}
		}

		public bool IsGamepadStickBindingPresent
		{
			get
			{
				return this.GamepadStickBindingNodePresent;
			}
		}

		public GamepadButton GamepadButton
		{
			get
			{
				return this._controllerButton.GamepadButton;
			}
		}

		public GamepadButtonDescription GamepadButtonDescription
		{
			get
			{
				return this._controllerButton.GamepadButtonDescription;
			}
		}

		public KeyScanCodeV2 KeyScanCode
		{
			get
			{
				return this._controllerButton.KeyScanCode;
			}
		}

		public AssociatedControllerButton ControllerButton
		{
			get
			{
				return this._controllerButton;
			}
		}

		public int BindingNodeCount
		{
			get
			{
				return this.Count((BaseMacro m) => m.IsKeyBinding || m.IsGamepadBinding || m.IsGamepadStick || m.IsTouchZoom || m.IsTouchSwipe);
			}
		}

		public bool BindingNodePresent
		{
			get
			{
				return this.Any((BaseMacro m) => m.IsKeyBinding || m.IsGamepadBinding || m.IsGamepadStick || m.IsTouchZoom || m.IsTouchSwipe);
			}
		}

		public bool RumbleNodePresent
		{
			get
			{
				return this.Any((BaseMacro m) => m.IsRumble);
			}
		}

		private int GamepadBindingNodeCount
		{
			get
			{
				return this.Count((BaseMacro m) => m.IsGamepadBinding || m.IsGamepadStick || m.IsTouchZoom || m.IsTouchSwipe);
			}
		}

		private bool GamepadBindingNodePresent
		{
			get
			{
				return this.Any((BaseMacro m) => m.IsGamepadBinding || m.IsGamepadStick || m.IsTouchZoom || m.IsTouchSwipe);
			}
		}

		private bool SwipeOrZoomBindingNodePresent
		{
			get
			{
				return this.Any((BaseMacro m) => m.IsTouchZoom || m.IsTouchSwipe);
			}
		}

		private bool VirtualTouchpadBindingNodePresent
		{
			get
			{
				return this.Any((BaseMacro m) => m.IsTouchZoom || m.IsTouchSwipe || (m.IsGamepadBinding && GamepadButtonExtensions.IsAnyVirtualTouchpad(((MacroGamepadBinding)m).GamepadButtonDescription.Button)));
			}
		}

		private int GamepadStickBindingNodeCount
		{
			get
			{
				return this.Count((BaseMacro m) => m.IsGamepadStick);
			}
		}

		private bool GamepadStickBindingNodePresent
		{
			get
			{
				return this.Any((BaseMacro m) => m.IsGamepadStick);
			}
		}

		private int PauseNodeCount
		{
			get
			{
				return this.Count((BaseMacro m) => m.IsPause);
			}
		}

		private int RumbleNodeCount
		{
			get
			{
				return this.Count((BaseMacro m) => m.IsRumble);
			}
		}

		private int BreakNodeCount
		{
			get
			{
				return this.Count((BaseMacro m) => m.IsBreak);
			}
		}

		private int FakeNodeCount
		{
			get
			{
				return this.Count((BaseMacro m) => m is MacroCrutchHoldUntillRelease);
			}
		}

		public DelegateCommand ClearCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._clear) == null)
				{
					delegateCommand = (this._clear = new DelegateCommand(new Action(this.ClearWithUserConfirmation), new Func<bool>(this.CleanCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void ClearWithUserConfirmation()
		{
			if (MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(11454), MessageBoxButton.YesNo, MessageBoxImage.Question, "ConfirmClearCombo", MessageBoxResult.Yes, false, 0.0, null, null, null, null, null, null) == MessageBoxResult.Yes)
			{
				base.Clear();
				this.IsChanged = true;
				PropertyChangedEventHandler macroSequencePropertyChanged = this.MacroSequencePropertyChanged;
				if (macroSequencePropertyChanged == null)
				{
					return;
				}
				macroSequencePropertyChanged(this, new PropertyChangedEventArgs("IsChanged"));
			}
		}

		private bool CleanCanExecute()
		{
			return this.Any<BaseMacro>();
		}

		public DelegateCommand ClearSilentCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._clearSilent) == null)
				{
					delegateCommand = (this._clearSilent = new DelegateCommand(new Action(this.Clear)));
				}
				return delegateCommand;
			}
		}

		public ICommand ClearAllPausesCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._clearAllPauses) == null)
				{
					relayCommand = (this._clearAllPauses = new RelayCommand(new Action(this.ClearAllPauses), new Func<bool>(this.ClearAllPausesCanExecute)));
				}
				return relayCommand;
			}
		}

		private void ClearAllPauses()
		{
			this.Remove((BaseMacro macro) => macro.IsPause);
		}

		private bool ClearAllPausesCanExecute()
		{
			return this.Any((BaseMacro macro) => macro.IsPause);
		}

		public DelegateCommand CleanPausesWThresholdCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._CleanPausesWThreshold) == null)
				{
					delegateCommand = (this._CleanPausesWThreshold = new DelegateCommand(new Action(this.CleanPausesWThreshold)));
				}
				return delegateCommand;
			}
		}

		private void CleanPausesWThreshold()
		{
			this.Remove((BaseMacro macro) => macro.IsPause && (ulong)((MacroPause)macro).Duration <= (ulong)((long)this.MacroCleanThreshold));
		}

		public DelegateCommand AddPausesBetweenItemsCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._AddPausesBetweenItems) == null)
				{
					delegateCommand = (this._AddPausesBetweenItems = new DelegateCommand(new Action(this.AddPausesBetweenItems)));
				}
				return delegateCommand;
			}
		}

		private void AddPausesBetweenItems()
		{
			for (int i = base.Count - 1; i >= 0; i--)
			{
				BaseMacro baseMacro = base[i];
				if (i > 0)
				{
					BaseMacro baseMacro2 = base[i - 1];
					if (baseMacro.IsKeyBinding && baseMacro2.IsKeyBinding)
					{
						this.InsertOrAdd(new MacroPause(this, (uint)this.MacroPauseBetweenItems), i, false);
					}
					if (baseMacro.IsGamepadBinding && baseMacro2.IsGamepadBinding)
					{
						this.InsertOrAdd(new MacroPause(this, (uint)this.MacroPauseBetweenItems), i, false);
					}
				}
			}
		}

		public DelegateCommand SwitchToHoldUntilReleaseCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._switchToHoldUntilRelease) == null)
				{
					delegateCommand = (this._switchToHoldUntilRelease = new DelegateCommand(new Action(this.SwitchToHoldUntilRelease)));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand SwitchToExecuteAtOnceCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._switchToExecuteAtOnce) == null)
				{
					delegateCommand = (this._switchToExecuteAtOnce = new DelegateCommand(new Action(this.SwitchToExecuteAtOnce)));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand AddRumbleCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._AddRumble) == null)
				{
					delegateCommand = (this._AddRumble = new DelegateCommand(new Action(this.AddRumble), new Func<bool>(this.AddRumbleCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void AddRumble()
		{
			this.InsertOrAdd(new MacroRumble(this, 150U, 80, 80, 60, 60), int.MaxValue, false);
		}

		private bool AddRumbleCanExecute()
		{
			return true;
		}

		public DelegateCommand AddBreakCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._addBreak) == null)
				{
					delegateCommand = (this._addBreak = new DelegateCommand(new Action(this.AddBreak), new Func<bool>(this.AddBreakCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void AddBreak()
		{
			this.InsertOrAdd(new MacroBreak(this), int.MaxValue, false);
		}

		private bool AddBreakCanExecute()
		{
			return this.BindingNodePresent || this.RumbleNodePresent;
		}

		public DelegateCommand AddPauseCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._addPause) == null)
				{
					delegateCommand = (this._addPause = new DelegateCommand(new Action(this.AddPause), new Func<bool>(this.AddPauseCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void AddPause()
		{
			this.InsertOrAdd(new MacroPause(this, 500U), int.MaxValue, false);
		}

		private bool AddPauseCanExecute()
		{
			return true;
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
			reWASDApplicationCommands.CopyMacroSequence(this);
		}

		private bool CopySelectedItemsCanExecute()
		{
			List<BaseMacro> list = this.Where((BaseMacro item) => item.IsSelected).ToList<BaseMacro>();
			bool flag;
			if (list.Count > 0)
			{
				flag = list.Any((BaseMacro item) => item is MacroGamepadBinding || item is MacroKeyBinding);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			bool flag3;
			if (list.Count > 0)
			{
				flag3 = list.Any((BaseMacro item) => item is MacroRumble || item is MacroPause || item is MacroBreak);
			}
			else
			{
				flag3 = false;
			}
			bool flag4 = flag3;
			return reWASDApplicationCommands.CanCopyMacroSequence(this) && (this.IsHoldUntilRelease || flag4 || (this.IsMultiSelectedItems && flag2));
		}

		public DelegateCommand PasteHotkeysCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._pasteHotkeysCommand) == null)
				{
					delegateCommand = (this._pasteHotkeysCommand = new DelegateCommand(delegate
					{
						this.PasteItems(true);
					}, () => reWASDApplicationCommands.CanPasteMacroSequence()));
				}
				return delegateCommand;
			}
		}

		public RelayCommand PasteCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._pasteCommand) == null)
				{
					relayCommand = (this._pasteCommand = new RelayCommand(delegate
					{
						this.PasteItems(false);
					}, () => reWASDApplicationCommands.CanPasteMacroSequence()));
				}
				return relayCommand;
			}
		}

		private void PasteItems(bool isHotkeys)
		{
			BaseMacro baseMacro;
			if (isHotkeys)
			{
				baseMacro = this.FirstOrDefault((BaseMacro i) => i.IsSelected);
			}
			else
			{
				baseMacro = this.FirstOrDefault((BaseMacro i) => i.IsRightClicked);
			}
			int num = base.Count;
			if (baseMacro != null)
			{
				num = base.IndexOf(baseMacro);
				if (isHotkeys)
				{
					num++;
				}
			}
			reWASDApplicationCommands.PasteMacroSequence(this, num);
			this.IsChanged = true;
			PropertyChangedEventHandler macroSequencePropertyChanged = this.MacroSequencePropertyChanged;
			if (macroSequencePropertyChanged == null)
			{
				return;
			}
			macroSequencePropertyChanged(this, new PropertyChangedEventArgs("IsChanged"));
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
			BaseMacro baseMacro = this.FirstOrDefault((BaseMacro i) => i.IsSelected);
			if (baseMacro != null)
			{
				reWASDApplicationCommands.ReplaceMacroSequence(this, base.IndexOf(baseMacro) + 1);
			}
		}

		public DelegateCommand RemoveCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._remove) == null)
				{
					delegateCommand = (this._remove = new DelegateCommand(delegate
					{
						this.RemoveSelectedItems(true);
					}, () => this.RemoveCanExecute()));
				}
				return delegateCommand;
			}
		}

		private bool RemoveCanExecute()
		{
			return this.Where((BaseMacro item) => item.IsSelected).ToList<BaseMacro>().Count > 0;
		}

		public event PropertyChangedEventHandler MacroSequencePropertyChanged;

		public MacroSequence(GamepadButton gamepadButton, ActivatorType activatorType)
			: this(gamepadButton, 1, activatorType)
		{
		}

		public MacroSequence(GamepadButton gamepadButton, MacroSequenceType macroSequenceType = 1, ActivatorType activatorType = 0)
			: this(GamepadButtonDescription.GamepadButtonDescriptionDictionary[gamepadButton], macroSequenceType, activatorType)
		{
		}

		public MacroSequence(GamepadButtonDescription gbd, MacroSequenceType mst = 1, ActivatorType at = 0)
			: this(new AssociatedControllerButton(), mst, at)
		{
			this._controllerButton.GamepadButtonDescription = gbd;
		}

		public MacroSequence(KeyScanCodeV2 ksc, MacroSequenceType mst = 1, ActivatorType at = 0)
			: this(new AssociatedControllerButton(), mst, at)
		{
			this._controllerButton.KeyScanCode = ksc;
		}

		public MacroSequence(AssociatedControllerButton acb, MacroSequenceType mst = 1, ActivatorType at = 0)
			: base(true)
		{
			this._controllerButton = acb;
			this.MacroSequenceType = mst;
			this.ActivatorType = at;
			this.SupressOnCollectionChanged = true;
			this.CollectionChanged += this.OnCollectionChanged;
			base.AfterCollectionChanged += this.OnAfterCollectionChanged;
		}

		public void Dispose()
		{
			this.CollectionChanged -= this.OnCollectionChanged;
			base.AfterCollectionChanged -= this.OnAfterCollectionChanged;
			this._controllerButton = null;
			base.SuppressNotification = true;
			base.Clear();
		}

		public void SetButtonsFromAnotherInstance(AssociatedControllerButton acb)
		{
			if (acb.IsKeyScanCode)
			{
				this._controllerButton.KeyScanCode = acb.KeyScanCode;
				this._controllerButton.GamepadButton = 2001;
				return;
			}
			this._controllerButton.GamepadButton = acb.GamepadButton;
			this._controllerButton.KeyScanCode = KeyScanCodeV2.NoMap;
		}

		public List<int> GetIndexOfBreaks()
		{
			List<int> list = new List<int>();
			foreach (BaseMacro baseMacro in this)
			{
				if (baseMacro.IsBreak)
				{
					list.Add(base.IndexOf(baseMacro));
				}
			}
			return list;
		}

		public void AddKeyScanCodeAccordingToSequenceType(KeyScanCodeV2 key)
		{
			if (this.IsOnetime)
			{
				MacroKeyBinding macroKeyBinding = new MacroKeyBinding(this, key, 0);
				MacroKeyBinding macroKeyBinding2 = new MacroKeyBinding(this, key, 1);
				macroKeyBinding.Twin = macroKeyBinding2;
				macroKeyBinding2.Twin = macroKeyBinding;
				this.InsertOrAdd(macroKeyBinding, int.MaxValue, false);
				this.InsertOrAdd(macroKeyBinding2, int.MaxValue, false);
				return;
			}
			if (this.IsHoldUntilRelease)
			{
				this.InsertOrAdd(new MacroKeyBinding(this, key, 0), int.MaxValue, false);
			}
		}

		public void AddGamepadButtonAccordingToSequenceType(GamepadButtonDescription gbd)
		{
			if (GamepadButtonExtensions.IsAnyStickDirection(gbd.Button))
			{
				this.AddGamepadAxisAccordingToSequenceType(gbd);
				return;
			}
			if (GamepadButtonExtensions.IsTouchpadSwipe(gbd.Button))
			{
				this.AddTouchpadSwipeAccordingToSequenceType(gbd);
				return;
			}
			if (GamepadButtonExtensions.IsTouchpadZoom(gbd.Button))
			{
				this.AddTouchpadZoomAccordingToSequenceType(gbd);
				return;
			}
			if (this.IsOnetime)
			{
				MacroGamepadBinding macroGamepadBinding = new MacroGamepadBinding(this, gbd, 0);
				MacroGamepadBinding macroGamepadBinding2 = new MacroGamepadBinding(this, gbd, 1);
				macroGamepadBinding.Twin = macroGamepadBinding2;
				macroGamepadBinding2.Twin = macroGamepadBinding;
				this.InsertOrAdd(macroGamepadBinding, int.MaxValue, false);
				this.InsertOrAdd(macroGamepadBinding2, int.MaxValue, false);
				return;
			}
			if (this.IsHoldUntilRelease)
			{
				this.InsertOrAdd(new MacroGamepadBinding(this, gbd, 0), int.MaxValue, false);
			}
		}

		public void AddGamepadAxisAccordingToSequenceType(GamepadButtonDescription gbd)
		{
			MacroGamepadStick macroGamepadStick = new MacroGamepadStick(this, gbd, 0)
			{
				DeflectionPercentage = 100
			};
			this.InsertOrAdd(macroGamepadStick, int.MaxValue, false);
		}

		private void AddTouchpadZoomAccordingToSequenceType(GamepadButtonDescription gbd)
		{
			MacroTouchZoom macroTouchZoom = new MacroTouchZoom(this, 250U, gbd.Button);
			this.InsertOrAdd(macroTouchZoom, int.MaxValue, false);
		}

		private void AddTouchpadSwipeAccordingToSequenceType(GamepadButtonDescription gbd)
		{
			MacroTouchSwipe macroTouchSwipe = new MacroTouchSwipe(this, 250U, gbd.Button);
			this.InsertOrAdd(macroTouchSwipe, int.MaxValue, false);
		}

		public void InsertOrAdd(BaseMacro item, int i = 2147483647, bool silentAdd = false)
		{
			if (!silentAdd)
			{
				this.RemoveCrutchHoldItem();
			}
			if (i < 0)
			{
				i = 0;
			}
			if (i > base.Count - 1)
			{
				base.Add(item);
			}
			else
			{
				this.InsertItem(i, item);
			}
			if (!silentAdd)
			{
				this.AddCrutchHoldItem(false);
			}
		}

		public void AddCrutchHoldItem(bool silentAdd = false)
		{
			if (this.MacroSequenceType != null)
			{
				return;
			}
			foreach (IList<BaseMacro> list in this.ToList<BaseMacro>().Split((BaseMacro macro) => macro.IsBreak))
			{
				BaseMacro baseMacro = list.LastOrDefault((BaseMacro i) => i.IsKeyBinding || i.IsGamepadBinding || i.IsGamepadStick || i.IsPause || i.IsRumble || i.IsTouchZoom || i.IsTouchSwipe);
				BaseMacro baseMacro2 = list.LastOrDefault((BaseMacro i) => i.MacroItemType == 8);
				if (baseMacro != null && baseMacro2 == null)
				{
					MacroCrutchHoldUntillRelease macroCrutchHoldUntillRelease;
					if (this.GamepadButtonDescription.Button == 2001)
					{
						macroCrutchHoldUntillRelease = new MacroCrutchHoldUntillRelease(this, null, this.KeyScanCode);
					}
					else
					{
						macroCrutchHoldUntillRelease = new MacroCrutchHoldUntillRelease(this, this.GamepadButtonDescription, null);
					}
					base.SuppressNotification = true;
					this.InsertOrAdd(macroCrutchHoldUntillRelease, base.IndexOf(baseMacro) + 1, true);
					base.SuppressNotification = false;
				}
				else if (baseMacro2 is MacroCrutchHoldUntillRelease && this.GamepadButtonDescription.Button == 2001)
				{
					(baseMacro2 as MacroCrutchHoldUntillRelease).KeyScanCode = this.KeyScanCode;
				}
			}
		}

		public void RemoveStickCompensationItem()
		{
			try
			{
				List<BaseMacro> list = this.Where((BaseMacro m) => m is MacroGamepadStickCompensation).ToList<BaseMacro>();
				if (list.Any<BaseMacro>())
				{
					bool isChanged = this.IsChanged;
					foreach (BaseMacro baseMacro in list)
					{
						base.Remove(baseMacro);
					}
					this.IsChanged = isChanged;
				}
			}
			catch (Exception)
			{
			}
		}

		public void MoveSelectedItems(BaseMacro baseMacro)
		{
			try
			{
				base.SuppressNotification = true;
				List<BaseMacro> list = this.Where((BaseMacro m) => m.IsSelected).ToList<BaseMacro>();
				foreach (BaseMacro baseMacro2 in list)
				{
					base.Remove(baseMacro2);
				}
				int num = base.IndexOf(baseMacro);
				if (baseMacro.IsBeingDraggedOverFromLeft)
				{
					num++;
				}
				base.InsertRange(num, list);
				this.CleanupSequence(true);
				this.RemoveCrutchHoldItem();
				this.AddCrutchHoldItem(false);
				base.SuppressNotification = false;
				this.CleanupSequence(true);
				this.IsChanged = true;
			}
			catch (Exception)
			{
			}
		}

		public void RemoveSelectedItems(bool clean = true)
		{
			try
			{
				base.SuppressNotification = true;
				List<BaseMacro> list = this.Where((BaseMacro m) => m.IsSelected).ToList<BaseMacro>();
				if (!list.Any<BaseMacro>())
				{
					base.SuppressNotification = false;
				}
				else
				{
					foreach (BaseMacro baseMacro in list)
					{
						base.Remove(baseMacro);
					}
					base.SuppressNotification = false;
					if (clean)
					{
						this.CleanupSequence(true);
					}
					this.IsChanged = true;
				}
			}
			catch (Exception)
			{
			}
		}

		public void RemoveCrutchHoldItem()
		{
			try
			{
				List<BaseMacro> list = this.Where((BaseMacro m) => m is MacroCrutchHoldUntillRelease).ToList<BaseMacro>();
				if (list.Any<BaseMacro>())
				{
					bool isChanged = this.IsChanged;
					foreach (BaseMacro baseMacro in list)
					{
						base.Remove(baseMacro);
					}
					this.IsChanged = isChanged;
				}
			}
			catch (Exception)
			{
			}
		}

		private void RemoveTheOnlyNotKeyItem()
		{
			BaseMacro baseMacro = this.FirstOrDefault<BaseMacro>();
			if (baseMacro == null || baseMacro.IsKeyBinding || baseMacro.IsGamepadBinding || baseMacro.IsGamepadStick || baseMacro.IsTouchSwipe || baseMacro.IsTouchZoom || baseMacro.IsPause || baseMacro.IsRumble)
			{
				return;
			}
			base.Remove(baseMacro);
		}

		private void SwitchToExecuteAtOnce()
		{
			if (this.IsOnetime)
			{
				return;
			}
			if (this.Any<BaseMacro>())
			{
				if (MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(11565), MessageBoxButton.OKCancel, MessageBoxImage.Asterisk, "WorkWithMacroEditor", MessageBoxResult.OK, false, 0.0, null, null, null, null, null, null) == MessageBoxResult.OK)
				{
					this.MacroSequenceType = 1;
					this.ConvertHoldToOnce();
				}
			}
			else
			{
				this.MacroSequenceType = 1;
			}
			PropertyChangedEventHandler macroSequencePropertyChanged = this.MacroSequencePropertyChanged;
			if (macroSequencePropertyChanged == null)
			{
				return;
			}
			macroSequencePropertyChanged(this, new PropertyChangedEventArgs("IsChanged"));
		}

		private void SwitchToHoldUntilRelease()
		{
			if (this.IsHoldUntilRelease)
			{
				return;
			}
			if (this.Any<BaseMacro>())
			{
				if (MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, string.Format(DTLocalization.GetString(11566), this.GamepadButtonDescription.FriendlyName), MessageBoxButton.OKCancel, MessageBoxImage.Asterisk, "WorkWithMacroEditor", MessageBoxResult.OK, false, 0.0, null, null, null, null, null, null) == MessageBoxResult.OK)
				{
					this.MacroSequenceType = 0;
					this.ConvertOnceToHold();
					this.FixMacroSequenceKeys(false);
				}
			}
			else
			{
				this.MacroSequenceType = 0;
			}
			PropertyChangedEventHandler macroSequencePropertyChanged = this.MacroSequencePropertyChanged;
			if (macroSequencePropertyChanged == null)
			{
				return;
			}
			macroSequencePropertyChanged(this, new PropertyChangedEventArgs("IsChanged"));
		}

		public void TryPromptToCorrectStickDirections()
		{
			if (!this.IsOnetime)
			{
				return;
			}
			if (this._dontBotherMeWithStickCompensation)
			{
				return;
			}
			if (!this.Any((BaseMacro macro) => macro is MacroGamepadStick))
			{
				return;
			}
			bool flag = this.Any((BaseMacro macro) => macro is MacroGamepadStickCompensation);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			foreach (BaseMacro baseMacro in this)
			{
				MacroGamepadStick macroGamepadStick = baseMacro as MacroGamepadStick;
				if (macroGamepadStick != null && macroGamepadStick.Relative)
				{
					if (macroGamepadStick.Stick == null)
					{
						if (macroGamepadStick.Axis == null)
						{
							num -= (int)macroGamepadStick.DeflectionPercentage;
						}
						if (macroGamepadStick.Axis == 1)
						{
							num2 -= (int)macroGamepadStick.DeflectionPercentage;
						}
					}
					else
					{
						if (macroGamepadStick.Axis == null)
						{
							num3 -= (int)macroGamepadStick.DeflectionPercentage;
						}
						if (macroGamepadStick.Axis == 1)
						{
							num4 -= (int)macroGamepadStick.DeflectionPercentage;
						}
					}
				}
			}
			if (num == 0 && num2 == 0 && num3 == 0 && num4 == 0)
			{
				this.RemoveStickCompensationItem();
				return;
			}
			MessageBoxResult messageBoxResult;
			if (flag)
			{
				messageBoxResult = MessageBoxResult.Yes;
				this.RemoveStickCompensationItem();
			}
			else
			{
				messageBoxResult = MessageBoxWithRememberMyChoiceLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(11453), MessageBoxButton.YesNo, MessageBoxImage.Question, "ConfirmAutoCorrectStickDeflections", "GuiNamespace", "ConfirmAutoCorrectStickDeflections", false, 0.0, null, null, null, null, null);
			}
			if (messageBoxResult == MessageBoxResult.Yes)
			{
				MacroGamepadStickCompensation macroGamepadStickCompensation = new MacroGamepadStickCompensation(this, 0)
				{
					LeftStickX = num,
					LeftStickY = num2,
					RightStickX = num3,
					RightStickY = num4
				};
				this.InsertOrAdd(macroGamepadStickCompensation, int.MaxValue, false);
				return;
			}
			this._dontBotherMeWithStickCompensation = true;
		}

		public bool SaveChanges(bool verbose, ref bool errorIsShown)
		{
			bool flag = this.CanSaveChanges(verbose, ref errorIsShown);
			if (!verbose || flag)
			{
				this.IsChanged = false;
				this._dontBotherMeWithStickCompensation = true;
			}
			return flag;
		}

		public bool CanSaveChanges(bool verbose, ref bool errorIsShown)
		{
			if (!this.Any<BaseMacro>())
			{
				return true;
			}
			ushort num = 0;
			ushort num2 = 0;
			byte b = 0;
			byte b2 = 0;
			foreach (BaseMacro baseMacro in this)
			{
				MacroItemType macroItemType = baseMacro.MacroItemType;
				if (macroItemType != null)
				{
					switch (macroItemType)
					{
					case 5:
						num2 += 1;
						break;
					case 6:
						b += 1;
						break;
					case 7:
						b2 += 1;
						break;
					}
				}
				else
				{
					num += 1;
				}
			}
			return true;
		}

		private void OnCollectionChanged(object s, NotifyCollectionChangedEventArgs e)
		{
			if (this.SupressOnCollectionChanged)
			{
				return;
			}
			this.ClearCommand.RaiseCanExecuteChanged();
			this.AddRumbleCommand.RaiseCanExecuteChanged();
			this.AddBreakCommand.RaiseCanExecuteChanged();
			this.AddPauseCommand.RaiseCanExecuteChanged();
			this.OnPropertyChanged("BindingNodeCount");
			this.OnPropertyChanged("PauseNodeCount");
			this.OnPropertyChanged("RumbleNodeCount");
			this.OnPropertyChanged("BreakNodeCount");
			this.OnPropertyChanged("FakeNodeCount");
			if (e.Action == NotifyCollectionChangedAction.Reset || (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count == 1 && e.NewItems[0] is MacroCrutchHoldUntillRelease))
			{
				return;
			}
			this.IsChanged = true;
		}

		private void OnAfterCollectionChanged(object sender, EventArgs eventArgs)
		{
			if (this.SupressOnCollectionChanged)
			{
				return;
			}
			if (base.Count == 1)
			{
				this.RemoveCrutchHoldItem();
				this.RemoveTheOnlyNotKeyItem();
			}
			this.MoveStickCompensationToEndOfSequence();
			this.CleanupSequence(false);
		}

		public void ConvertOnceToHold()
		{
			base.SuppressNotification = true;
			for (int i = base.Count - 1; i >= 0; i--)
			{
				BaseMacro baseMacro = base[i];
				if ((baseMacro.IsKeyBinding || baseMacro.IsGamepadBinding) && ((BaseMacroBinding)baseMacro).MacroKeyType == 1)
				{
					base.Remove(baseMacro);
				}
				if (baseMacro is MacroGamepadStickCompensation)
				{
					base.Remove(baseMacro);
				}
			}
			this.AddCrutchHoldItem(false);
			base.SuppressNotification = false;
		}

		public void ConvertHoldToOnce()
		{
			base.SuppressNotification = true;
			this.RemoveCrutchHoldItem();
			for (int i = base.Count - 1; i >= 0; i--)
			{
				BaseMacro baseMacro = base[i];
				if (baseMacro.IsKeyBinding)
				{
					MacroKeyBinding macroKeyBinding = (MacroKeyBinding)baseMacro;
					KeyScanCodeV2 keyScanCode = macroKeyBinding.KeyScanCode;
					MacroKeyBinding macroKeyBinding2 = new MacroKeyBinding(this, keyScanCode, 1);
					macroKeyBinding.SetTwin(macroKeyBinding2);
					this.InsertOrAdd(macroKeyBinding2, i + 1, false);
				}
				if (baseMacro.IsGamepadBinding)
				{
					MacroGamepadBinding macroGamepadBinding = (MacroGamepadBinding)baseMacro;
					GamepadButtonDescription gamepadButtonDescription = macroGamepadBinding.GamepadButtonDescription;
					MacroGamepadBinding macroGamepadBinding2 = new MacroGamepadBinding(this, gamepadButtonDescription, 1);
					macroGamepadBinding.SetTwin(macroGamepadBinding2);
					this.InsertOrAdd(macroGamepadBinding2, i + 1, false);
				}
			}
			base.SuppressNotification = false;
		}

		public void PrepareForSave()
		{
			this.CleanupSequence(false);
			this.TryPromptToCorrectStickDirections();
		}

		public void CleanupSequence(bool removeUnparedKeys = false)
		{
			if (!this.Any<BaseMacro>())
			{
				return;
			}
			this.CleanupPause();
			this.FixMacroSequenceKeys(removeUnparedKeys);
			this.CalculateTotalComboTime();
		}

		public void MoveStickCompensationToEndOfSequence()
		{
			BaseMacro baseMacro = this.FirstOrDefault((BaseMacro macro) => macro is MacroGamepadStickCompensation);
			if (baseMacro != null)
			{
				this._supressOnCollectionChanged = true;
				base.Remove(baseMacro);
				this.InsertOrAdd(baseMacro, int.MaxValue, false);
				this._supressOnCollectionChanged = false;
			}
		}

		public void CleanupPause()
		{
			for (int i = base.Count - 1; i > 0; i--)
			{
				BaseMacro baseMacro = base[i];
				if (i > 0)
				{
					BaseMacro baseMacro2 = base[i - 1];
					if (baseMacro.IsPause && baseMacro2.IsPause)
					{
						this._supressOnCollectionChanged = true;
						base.Remove(baseMacro);
						base.Remove(baseMacro2);
						MacroPause macroPause = new MacroPause(this, ((MacroPause)baseMacro).Duration + ((MacroPause)baseMacro2).Duration);
						this.InsertOrAdd(macroPause, i - 1, false);
						this._supressOnCollectionChanged = false;
					}
				}
			}
		}

		public void CleanupStickCompensation()
		{
			this._supressOnCollectionChanged = true;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			for (int i = base.Count - 1; i > 0; i--)
			{
				BaseMacro baseMacro = base[i];
				if (i > 0)
				{
					MacroGamepadStickCompensation macroGamepadStickCompensation = baseMacro as MacroGamepadStickCompensation;
					if (macroGamepadStickCompensation != null)
					{
						num += macroGamepadStickCompensation.LeftStickX;
						num2 += macroGamepadStickCompensation.LeftStickY;
						num3 += macroGamepadStickCompensation.RightStickX;
						num4 += macroGamepadStickCompensation.RightStickY;
						base.Remove(baseMacro);
					}
				}
			}
			if (num + num2 + num3 + num4 != 0)
			{
				MacroGamepadStickCompensation macroGamepadStickCompensation2 = new MacroGamepadStickCompensation(this, 0)
				{
					LeftStickX = num,
					LeftStickY = num2,
					RightStickX = num3,
					RightStickY = num4
				};
				this.InsertOrAdd(macroGamepadStickCompensation2, int.MaxValue, false);
			}
			this._supressOnCollectionChanged = false;
		}

		public bool IsMacroSequenceTypeValid()
		{
			MacroSequenceType macroSequenceType = this.MacroSequenceType;
			return macroSequenceType == null || macroSequenceType == 1;
		}

		public void FixMacroSequenceKeys(bool removeUnparedKeys = false)
		{
			this.IsMacroSequenceValid(true, removeUnparedKeys);
		}

		private bool IsMacroKeyTypeValid(MacroKeyType macroKeyType)
		{
			switch (macroKeyType)
			{
			case 0:
				return true;
			case 1:
				return true;
			case 2:
				return true;
			default:
				return false;
			}
		}

		public bool IsMacroSequenceValid(bool fixSequenceInProcess = false, bool removeUnparedKeys = false)
		{
			if (!fixSequenceInProcess && !this.IsMacroSequenceTypeValid())
			{
				return false;
			}
			Collection<BaseMacro> collection = new ObservableCollection<BaseMacro>(this);
			ObservableCollection<BaseMacroBinding> observableCollection = new ObservableCollection<BaseMacroBinding>();
			foreach (BaseMacro baseMacro in collection)
			{
				if (baseMacro.IsBinding)
				{
					BaseMacroBinding macroBinding = (BaseMacroBinding)baseMacro;
					if (baseMacro.IsKeyBinding && !fixSequenceInProcess && !this.IsMacroKeyTypeValid(((MacroKeyBinding)baseMacro).MacroKeyType))
					{
						return false;
					}
					if (baseMacro.IsGamepadBinding && !fixSequenceInProcess && !this.IsMacroKeyTypeValid(((MacroGamepadBinding)baseMacro).MacroKeyType))
					{
						return false;
					}
					if (macroBinding.MacroKeyType == null || macroBinding.MacroKeyType == 2)
					{
						if (observableCollection.FirstOrDefault((BaseMacroBinding b) => b.KeyScanCode == macroBinding.KeyScanCode && b.MacroKeyType == macroBinding.MacroKeyType) != null)
						{
							if (fixSequenceInProcess && (!baseMacro.IsGamepadBinding || this.MacroSequenceType == null || (!GamepadButtonExtensions.IsAnyTriggerPress(((MacroGamepadBinding)baseMacro).GamepadButtonDescription.Button) && !GamepadButtonExtensions.IsDS3AnalogButton(((MacroGamepadBinding)baseMacro).GamepadButtonDescription.Button))))
							{
								base.Remove(baseMacro);
								continue;
							}
							if (!baseMacro.IsGamepadBinding || (!GamepadButtonExtensions.IsAnyTriggerPress(((MacroGamepadBinding)baseMacro).GamepadButtonDescription.Button) && !GamepadButtonExtensions.IsDS3AnalogButton(((MacroGamepadBinding)baseMacro).GamepadButtonDescription.Button)))
							{
								return false;
							}
						}
						observableCollection.Add(macroBinding);
						continue;
					}
					if (macroBinding.MacroKeyType == 1)
					{
						BaseMacroBinding baseMacroBinding = observableCollection.FirstOrDefault((BaseMacroBinding b) => b.KeyScanCode == macroBinding.KeyScanCode && (b.MacroKeyType == null || b.MacroKeyType == 2));
						if (baseMacroBinding != null)
						{
							macroBinding.SetTwin(baseMacroBinding);
							observableCollection.Remove(baseMacroBinding);
							continue;
						}
						if (fixSequenceInProcess)
						{
							base.Remove(baseMacro);
							continue;
						}
						return false;
					}
				}
				if (baseMacro.IsBreak)
				{
					foreach (BaseMacroBinding baseMacroBinding2 in observableCollection)
					{
						if (this.MacroSequenceType != null || baseMacroBinding2.MacroKeyType != null)
						{
							if (!fixSequenceInProcess)
							{
								return false;
							}
							base.Remove(baseMacroBinding2);
						}
					}
					observableCollection.Clear();
				}
			}
			if (this.MacroSequenceType > 0 && removeUnparedKeys && observableCollection.Count > 0)
			{
				foreach (BaseMacroBinding baseMacroBinding3 in observableCollection)
				{
					base.Remove(baseMacroBinding3);
				}
			}
			return true;
		}

		public bool IsMacroSequenceNeedLicence()
		{
			return base.Count != 0 && (base.Count != 1 || (!base[0].IsRumble && !base[0].IsBinding)) && (base.Count != 2 || !base[0].IsRumble || !base[1].IsBinding);
		}

		protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (object.Equals(storage, value))
			{
				return false;
			}
			storage = value;
			this.OnPropertyChanged(propertyName);
			return true;
		}

		public virtual void OnPropertyChanged(string propertyName = null)
		{
			PropertyChangedEventArgs propertyChangedEventArgs = new PropertyChangedEventArgs(propertyName);
			this.OnPropertyChanged(propertyChangedEventArgs);
		}

		public void SuppressCollectionChangedNotification()
		{
			this.SupressOnCollectionChanged = true;
		}

		public void AllowCollectionChangedNotification()
		{
			this.SupressOnCollectionChanged = false;
			base.SuppressNotification = false;
		}

		public MacroSequence Clone(AssociatedControllerButton controllerButton = null)
		{
			MacroSequence macroSequence = ((controllerButton != null) ? new MacroSequence(controllerButton, this._macroSequenceType, 0) : new MacroSequence(this.ControllerButton, this._macroSequenceType, 0));
			macroSequence._activatorType = this.ActivatorType;
			macroSequence._isParentBindingToggle = this.IsParentBindingToggle;
			macroSequence._isParentBindingTurbo = this.IsParentBindingTurbo;
			macroSequence._dontBotherMeWithStickCompensation = this._dontBotherMeWithStickCompensation;
			macroSequence._keyDelay = this._keyDelay;
			macroSequence._macroCleanThreshold = this._macroCleanThreshold;
			macroSequence._macroPauseBetweenItems = this._macroPauseBetweenItems;
			macroSequence._repeatCount = this._repeatCount;
			macroSequence._repeatInterval = this._repeatInterval;
			macroSequence._turboDelay = this._turboDelay;
			macroSequence.SupressOnCollectionChanged = true;
			bool flag = false;
			foreach (BaseMacro baseMacro in this)
			{
				BaseMacro baseMacro2 = baseMacro.Clone(macroSequence);
				macroSequence.Add(baseMacro2);
				flag = true;
			}
			if (flag)
			{
				macroSequence.FixMacroSequenceKeys(false);
			}
			macroSequence.SupressOnCollectionChanged = false;
			return macroSequence;
		}

		public void AddMacrosFromModel(MacroSequence model)
		{
			bool flag = false;
			foreach (BaseMacro baseMacro in model)
			{
				BaseMacro baseMacro2 = BaseMacro.CloneFromModel(this, baseMacro);
				base.Add(baseMacro2);
				flag = true;
			}
			if (flag)
			{
				this.FixMacroSequenceKeys(false);
			}
		}

		public async Task AsyncAddMacrosFromModel(MacroSequence model)
		{
			bool flag = false;
			int counter = 0;
			foreach (BaseMacro baseMacro in model)
			{
				BaseMacro baseMacro2 = BaseMacro.CloneFromModel(this, baseMacro);
				base.Add(baseMacro2);
				int num = counter;
				counter = num + 1;
				if (num % 10 == 0)
				{
					await Task.Delay(1);
				}
				flag = true;
			}
			List<BaseMacro>.Enumerator enumerator = default(List<BaseMacro>.Enumerator);
			if (flag)
			{
				this.FixMacroSequenceKeys(false);
			}
		}

		public void RaiseCanExecuteChangedForCommands()
		{
			foreach (BaseMacro baseMacro in this)
			{
				baseMacro.RaiseCanExecuteChangedForCommands();
			}
		}

		public void CopyFromModel(MacroSequence model)
		{
			base.Clear();
			this._controllerButton.CopyFromModel(model.ControllerButton);
			this._macroSequenceType = model.MacroSequenceType;
			this._activatorType = model.ActivatorType;
			this._isParentBindingToggle = model.IsParentBindingToggle;
			this._isParentBindingTurbo = model.IsParentBindingTurbo;
			this._keyDelay = model.KeyDelay;
			this._macroCleanThreshold = model.MacroCleanThreshold;
			this._macroPauseBetweenItems = model.MacroPauseBetweenItems;
			this._repeatCount = model.RepeatCount;
			this._repeatInterval = model.RepeatInterval;
			this._turboDelay = model.TurboDelay;
			this.SupressOnCollectionChanged = true;
			this.AddMacrosFromModel(model);
			this.SupressOnCollectionChanged = false;
			this.CalculateTotalComboTime();
		}

		public void CopyToModel(MacroSequence model)
		{
			model.Clear();
			this._controllerButton.CopyToModel(model.ControllerButton);
			model.MacroSequenceType = this._macroSequenceType;
			model.ActivatorType = this._activatorType;
			model.IsParentBindingToggle = this._isParentBindingToggle;
			model.IsParentBindingTurbo = this._isParentBindingTurbo;
			model.KeyDelay = this.KeyDelay;
			model.MacroCleanThreshold = this._macroCleanThreshold;
			model.MacroPauseBetweenItems = this._macroPauseBetweenItems;
			model.RepeatCount = this._repeatCount;
			model.RepeatInterval = this._repeatInterval;
			model.TurboDelay = this._turboDelay;
			foreach (BaseMacro baseMacro in this)
			{
				BaseMacro baseMacro2 = baseMacro.Clone(model);
				model.Add(baseMacro2);
			}
		}

		private MacroSequenceType _macroSequenceType;

		private int _repeatCount = 1;

		private int _repeatInterval;

		private bool _isParentBindingTurbo;

		private bool _isParentBindingToggle;

		private int _turboDelay;

		private int _keyDelay = 10;

		private int _macroCleanThreshold = 2550;

		private int _macroPauseBetweenItems = 10;

		private ActivatorType _activatorType;

		private bool _isChanged;

		private bool _supressOnCollectionChanged;

		private AssociatedControllerButton _controllerButton;

		private bool _dontBotherMeWithStickCompensation = true;

		private string _totalComboTime;

		private DelegateCommand _clear;

		private DelegateCommand _clearSilent;

		private RelayCommand _clearAllPauses;

		private DelegateCommand _CleanPausesWThreshold;

		private DelegateCommand _AddPausesBetweenItems;

		private DelegateCommand _switchToHoldUntilRelease;

		private DelegateCommand _switchToExecuteAtOnce;

		private DelegateCommand _AddRumble;

		private DelegateCommand _addBreak;

		private DelegateCommand _addPause;

		private DelegateCommand _copyCommand;

		private DelegateCommand _pasteHotkeysCommand;

		private RelayCommand _pasteCommand;

		private DelegateCommand _replaceCommand;

		private DelegateCommand _remove;
	}
}
