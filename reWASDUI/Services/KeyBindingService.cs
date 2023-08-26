using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using Prism.Events;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.SupportedControllers;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Services.Interfaces;
using reWASDUI.Utils.XBUtil;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.reWASDMapping;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Infrastructure.reWASDMapping.RewasduserCommands;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.Services
{
	public class KeyBindingService : ZBindable, IKeyBindingService
	{
		public IGameProfilesService GameProfilesService { get; set; }

		public IEventAggregator EventAggregator { get; set; }

		public ObservableCollection<BaseRewasdMapping> RewasdMappingsCollection
		{
			get
			{
				return this._rewasdMappingsCollection;
			}
			set
			{
				this._rewasdMappingsCollection = value;
				this.OnPropertyChanged("RewasdMappingsCollection");
			}
		}

		public ObservableCollection<KeyScanCodeV2> KeyScanCodeCollectionForKeyboard
		{
			get
			{
				if (this._keyScanCodeCollectionForKeyboard == null)
				{
					this.RecreateKeyboardKeysCollectionForKeyboard(null);
				}
				return this._keyScanCodeCollectionForKeyboard;
			}
			set
			{
				this._keyScanCodeCollectionForKeyboard = value;
				this.OnPropertyChanged("KeyScanCodeCollectionForKeyboard");
			}
		}

		public ObservableCollection<KeyScanCodeV2> KeyScanCodeCollectionForMouse
		{
			get
			{
				if (this._keyScanCodeCollectionForMouse == null)
				{
					this.RecreateKeyboardKeysCollectionForMouse(null);
				}
				return this._keyScanCodeCollectionForMouse;
			}
			set
			{
				this._keyScanCodeCollectionForMouse = value;
				this.OnPropertyChanged("KeyScanCodeCollectionForMouse");
			}
		}

		public ObservableCollection<KeyScanCodeV2> KeyScanCodeCollectionForGamepad
		{
			get
			{
				if (this._keyScanCodeCollectionForGamepad == null)
				{
					this.RecreateKeyboardKeysCollectionForGamepad(null);
				}
				return this._keyScanCodeCollectionForGamepad;
			}
			set
			{
				this._keyScanCodeCollectionForGamepad = value;
				this.OnPropertyChanged("KeyScanCodeCollectionForGamepad");
			}
		}

		public ObservableCollection<BaseRewasdMapping> RewasdMappingsCollectionWithoutMouseAndScrolls
		{
			get
			{
				return this._rewasdMappingsCollectionWithoutMouseAndScrolls;
			}
			set
			{
				this._rewasdMappingsCollectionWithoutMouseAndScrolls = value;
				this.OnPropertyChanged("RewasdMappingsCollectionWithoutMouseAndScrolls");
			}
		}

		public ObservableCollection<BaseRewasdMapping> RewasdMappingsCollectionWithDoNotInherit
		{
			get
			{
				return this._rewasdMappingsCollectionWithDoNotInherit;
			}
			set
			{
				this._rewasdMappingsCollectionWithDoNotInherit = value;
				this.OnPropertyChanged("RewasdMappingsCollectionWithDoNotInherit");
			}
		}

		public ObservableCollection<BaseRewasdMapping> RewasdMappingsCollectionWithoutMouseAndScrollsWithDoNotInherit
		{
			get
			{
				return this._rewasdMappingsCollectionWithoutMouseAndScrollsWithDoNotInherit;
			}
			set
			{
				this._rewasdMappingsCollectionWithoutMouseAndScrollsWithDoNotInherit = value;
				this.OnPropertyChanged("RewasdMappingsCollectionWithoutMouseAndScrollsWithDoNotInherit");
			}
		}

		public ObservableCollection<GamepadButtonDescription> ButtonsToRemap
		{
			get
			{
				if (this._buttonsToRemap == null)
				{
					this.RecreateButtonsToRemap(null);
				}
				return this._buttonsToRemap;
			}
		}

		public ObservableCollection<GamepadButton> ButtonsForMask
		{
			get
			{
				if (this._buttonsForMask == null)
				{
					this.RecreateButtonsForMaskCollection(null);
				}
				return this._buttonsForMask;
			}
		}

		public ObservableCollection<GamepadButtonDescription> ShiftModifierButtons { get; } = new ObservableCollection<GamepadButtonDescription>();

		public KeyBindingService(IGameProfilesService gps, IEventAggregator ea)
		{
			Tracer.TraceWrite("Constructor for KeyBindingService", false);
			this.GameProfilesService = gps;
			this.EventAggregator = ea;
			this.RecreateKeyScanCodeCollectionForReWASDMappings(0);
			CollectionExtensions.AddRange<GamepadButtonDescription>(this.ShiftModifierButtons, from xbd in GamepadButtonDescription.GAMEPAD_BUTTON_DESCRIPTIONS
				where xbd.ShiftModifierAble
				orderby xbd.IndexForSorting
				select xbd);
			this.EventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(new Action<BaseControllerVM>(this.OnGamepadChanged));
			this.EventAggregator.GetEvent<CurrentGamepadCurrentChanged>().Subscribe(new Action<ControllerVM>(this.OnGamepadChanged));
			this.EventAggregator.GetEvent<CurrentShiftBindingCollectionChanged>().Subscribe(delegate(ShiftXBBindingCollection o)
			{
				this.OnPropertyChanged("RewasdMappingsCollection");
			});
		}

		private void OnGamepadChanged(BaseControllerVM controllerVM)
		{
			this.RecreateKeyboardKeysCollectionForKeyboard(controllerVM);
			this.RecreateKeyboardKeysCollectionForMouse(controllerVM);
			this.RecreateKeyboardKeysCollectionForGamepad(controllerVM);
			this.RecreateButtonsForMaskCollection(controllerVM);
			this.RecreateButtonsToRemap((controllerVM != null) ? controllerVM.CurrentController : null);
			this.RaiseButtonsToRemapCollectionChanged();
		}

		public ObservableCollection<KeyScanCodeV2> GenerateKeysForController(BaseControllerVM controllerVM = null, bool isKeyboardExpected = false, bool isMouseExpected = false, bool isMaskMode = false, bool isGamepad = false)
		{
			bool flag = true;
			bool flag2 = true;
			bool flag3 = true;
			bool flag4 = true;
			bool flag5 = false;
			if (controllerVM != null)
			{
				if (isGamepad)
				{
					ControllerVM currentController = controllerVM.CurrentController;
					if (currentController != null && currentController.ControllerFamily == 0 && controllerVM.CurrentController.IsIpega)
					{
						return new ObservableCollection<KeyScanCodeV2>(from k in KeyScanCodeV2.SCAN_CODE_TABLE
							where k.ControllerButtonTags != null
							select k into x
							where x == KeyScanCodeV2.NoMap || x == KeyScanCodeV2.VolDown || x == KeyScanCodeV2.PrevTrack || x == KeyScanCodeV2.PlayPause || x == KeyScanCodeV2.NextTrack || x == KeyScanCodeV2.VolUp
							select x);
					}
				}
				if (isGamepad)
				{
					ControllerVM currentController2 = controllerVM.CurrentController;
					if (currentController2 != null && currentController2.ControllerFamily == 0 && controllerVM.CurrentController.IsRaiju)
					{
						return new ObservableCollection<KeyScanCodeV2>(from k in KeyScanCodeV2.SCAN_CODE_TABLE
							where k.ControllerButtonTags != null
							select k into x
							where x == KeyScanCodeV2.NoMap || x == KeyScanCodeV2.DikWebHome || x == KeyScanCodeV2.DikWebBack
							select x);
					}
				}
				flag = controllerVM.HasKeyboardControllers;
				flag4 = controllerVM.HasMouseControllers;
				if (flag || flag4)
				{
					flag2 = controllerVM.HasConsumerControllers;
					flag3 = controllerVM.HasSystemControllers;
					flag5 = controllerVM.ControllerFamily == 2;
				}
				if (!controllerVM.HasKeyboardControllers && isKeyboardExpected)
				{
					flag = true;
					flag2 = true;
					flag3 = true;
					flag4 = true;
					flag5 = false;
				}
				if (!controllerVM.HasMouseControllers && isMouseExpected)
				{
					flag = true;
					flag2 = true;
					flag3 = true;
					flag4 = true;
				}
			}
			IEnumerable<KeyScanCodeV2> enumerable = KeyScanCodeV2.SCAN_CODE_TABLE.Where((KeyScanCodeV2 k) => k.ControllerButtonTags != null);
			if (!flag)
			{
				enumerable = enumerable.Where((KeyScanCodeV2 k) => !k.ControllerButtonTags.Contains(1));
			}
			if (!flag2)
			{
				enumerable = enumerable.Where((KeyScanCodeV2 k) => !k.ControllerButtonTags.Contains(2));
			}
			if (!flag3)
			{
				enumerable = enumerable.Where((KeyScanCodeV2 k) => !k.ControllerButtonTags.Contains(6));
			}
			if ((!isMaskMode && flag5) || !flag4)
			{
				enumerable = enumerable.Where((KeyScanCodeV2 k) => !k.ControllerButtonTags.Contains(3));
			}
			if (isMouseExpected && !flag5)
			{
				enumerable = enumerable.Where((KeyScanCodeV2 k) => !k.ControllerButtonTags.Contains(3));
			}
			if (flag5 && flag4)
			{
				enumerable = enumerable.OrderByDescending((KeyScanCodeV2 k) => k.IsNotMapped || k.IsMouse);
			}
			return new ObservableCollection<KeyScanCodeV2>(enumerable);
		}

		public ObservableCollection<GamepadButton> GenerateButtonsForController(BaseControllerVM gamepadVM = null)
		{
			ControllerTypeEnum[] array = new ControllerTypeEnum[] { 3 };
			bool flag = true;
			if (gamepadVM != null)
			{
				ControllerVM controllerVM = gamepadVM as ControllerVM;
				if (controllerVM != null)
				{
					array = new ControllerTypeEnum[] { controllerVM.ControllerType };
				}
				else
				{
					array = ControllerTypeExtensions.ConvertPhysicalTypesToEnums(0, gamepadVM.Types, gamepadVM.Ids);
				}
				flag = gamepadVM.IsAnalogTriggersPresent;
			}
			return XBUtils.CreateMaskButtonsCollection(array, flag);
		}

		public ObservableCollection<GamepadButtonDescription> GenerateRemapButtonDescriptionsForController(BaseControllerVM gamepadVM = null, bool includeUnmapable = false, GamepadButtonDescription currentButtonDescription = null)
		{
			ControllerTypeEnum controllerType = 3;
			if (gamepadVM != null)
			{
				controllerType = gamepadVM.FirstControllerType;
			}
			if (!ControllerTypeExtensions.IsGamepad(controllerType))
			{
				controllerType = 3;
			}
			controllerType = XBUtils.CorrectNintendoSwitchJoy(new ControllerTypeEnum?(controllerType)).Value;
			ObservableCollection<GamepadButtonDescription> observableCollection = new ObservableCollection<GamepadButtonDescription>();
			if (!ControllerTypeExtensions.IsGamepadWithSonyTouchpad(controllerType) || currentButtonDescription == null || !GamepadButtonExtensions.IsPhysicalTrackPad1Direction(currentButtonDescription.Button))
			{
				observableCollection.Add(GamepadButtonDescription.Unmapped);
			}
			observableCollection.Add(GamepadButtonDescription.Button1);
			observableCollection.Add(GamepadButtonDescription.Button2);
			observableCollection.Add(GamepadButtonDescription.Button3);
			observableCollection.Add(GamepadButtonDescription.Button4);
			observableCollection.Add(GamepadButtonDescription.DPadUp);
			observableCollection.Add(GamepadButtonDescription.DPadDown);
			observableCollection.Add(GamepadButtonDescription.DPadLeft);
			observableCollection.Add(GamepadButtonDescription.DPadRight);
			observableCollection.Add(GamepadButtonDescription.LeftBumper);
			observableCollection.Add(GamepadButtonDescription.RightBumper);
			observableCollection.Add(GamepadButtonDescription.LeftTrigger);
			observableCollection.Add(GamepadButtonDescription.RightTrigger);
			if (includeUnmapable)
			{
				observableCollection.Add(GamepadButtonDescription.LeftLowerPaddle);
				observableCollection.Add(GamepadButtonDescription.LeftUpperPaddle);
				observableCollection.Add(GamepadButtonDescription.RightLowerPaddle);
				observableCollection.Add(GamepadButtonDescription.RightUpperPaddle);
				observableCollection.Add(GamepadButtonDescription.Button11);
			}
			if (includeUnmapable)
			{
				observableCollection.Add(GamepadButtonDescription.LeftStickUp);
				observableCollection.Add(GamepadButtonDescription.LeftStickDown);
				observableCollection.Add(GamepadButtonDescription.LeftStickLeft);
				observableCollection.Add(GamepadButtonDescription.LeftStickRight);
			}
			observableCollection.Add(GamepadButtonDescription.LeftStickClick);
			if (includeUnmapable)
			{
				observableCollection.Add(GamepadButtonDescription.RightStickUp);
				observableCollection.Add(GamepadButtonDescription.RightStickDown);
				observableCollection.Add(GamepadButtonDescription.RightStickLeft);
				observableCollection.Add(GamepadButtonDescription.RightStickRight);
			}
			if (!ControllerTypeExtensions.IsXboxOrRazerWolverine2(controllerType) && !ControllerTypeExtensions.IsGoogle(controllerType) && includeUnmapable)
			{
				observableCollection.Add(GamepadButtonDescription.GyroUp);
				observableCollection.Add(GamepadButtonDescription.GyroDown);
				observableCollection.Add(GamepadButtonDescription.GyroLeft);
				observableCollection.Add(GamepadButtonDescription.GyroRight);
			}
			observableCollection.Add(GamepadButtonDescription.RightStickClick);
			observableCollection.Add(GamepadButtonDescription.Button7);
			observableCollection.Add(GamepadButtonDescription.Button8);
			ControllerTypeEnum controllerType2 = controllerType;
			if (controllerType2 <= 62)
			{
				switch (controllerType2)
				{
				case 4:
				case 14:
				case 20:
				case 21:
				case 56:
				case 57:
					observableCollection.AddIfNotContains(GamepadButtonDescription.Button11);
					observableCollection.AddIfNotContains(GamepadButtonDescription.TrackPad1Click);
					if (!includeUnmapable)
					{
						goto IL_62A;
					}
					observableCollection.Add(GamepadButtonDescription.TrackPad1Tap);
					if (ControllerTypeExtensions.IsAnySonyDualSense(controllerType))
					{
						observableCollection.AddIfNotContains(GamepadButtonDescription.Button13);
						goto IL_62A;
					}
					goto IL_62A;
				case 5:
				case 6:
					observableCollection.AddIfNotContains(GamepadButtonDescription.Button11);
					goto IL_62A;
				case 7:
				case 8:
				case 9:
				case 10:
				case 50:
					observableCollection.AddIfNotContains(GamepadButtonDescription.Button11);
					observableCollection.AddIfNotContains(GamepadButtonDescription.Button12);
					goto IL_62A;
				case 11:
				case 12:
				case 15:
				case 19:
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
				case 31:
				case 32:
				case 33:
				case 34:
				case 35:
				case 36:
				case 41:
				case 44:
				case 45:
				case 46:
				case 47:
				case 48:
				case 52:
				case 53:
				case 54:
				case 55:
					goto IL_62A;
				case 13:
					observableCollection.AddIfNotContains(GamepadButtonDescription.Button11);
					observableCollection.AddIfNotContains(GamepadButtonDescription.Button12);
					observableCollection.AddIfNotContains(GamepadButtonDescription.Button13);
					goto IL_62A;
				case 16:
					goto IL_442;
				case 17:
				case 18:
					observableCollection.Remove(GamepadButtonDescription.Button7);
					observableCollection.Remove(GamepadButtonDescription.LeftBumper);
					observableCollection.Remove(GamepadButtonDescription.LeftStickClick);
					observableCollection.Remove(GamepadButtonDescription.RightStickClick);
					if (includeUnmapable)
					{
						observableCollection.Add(GamepadButtonDescription.LeftTriggerFullPull);
						observableCollection.Add(GamepadButtonDescription.RightTriggerFullPull);
						goto IL_62A;
					}
					goto IL_62A;
				case 22:
				case 51:
					if (includeUnmapable)
					{
						observableCollection.AddIfNotContains(GamepadButtonDescription.Button13);
						goto IL_62A;
					}
					goto IL_62A;
				case 23:
				case 24:
					if (!includeUnmapable)
					{
						goto IL_62A;
					}
					observableCollection.AddIfNotContains(GamepadButtonDescription.Button13);
					if (controllerType == 23)
					{
						observableCollection.AddIfNotContains(GamepadButtonDescription.TrackPad1Click);
						observableCollection.AddIfNotContains(GamepadButtonDescription.TrackPad1Tap);
						goto IL_62A;
					}
					goto IL_62A;
				case 37:
				case 39:
				case 40:
					break;
				case 38:
					observableCollection.Add(GamepadButtonDescription.Button11);
					observableCollection.Add(GamepadButtonDescription.Button12);
					observableCollection.Add(GamepadButtonDescription.Button13);
					observableCollection.Add(GamepadButtonDescription.Button14);
					observableCollection.Add(GamepadButtonDescription.Button15);
					observableCollection.Add(GamepadButtonDescription.Button16);
					observableCollection.Add(GamepadButtonDescription.Button17);
					observableCollection.Add(GamepadButtonDescription.AdditionalStickUp);
					observableCollection.Add(GamepadButtonDescription.AdditionalStickDown);
					observableCollection.Add(GamepadButtonDescription.AdditionalStickLeft);
					observableCollection.Add(GamepadButtonDescription.AdditionalStickRight);
					goto IL_62A;
				case 42:
					observableCollection.AddIfNotContains(GamepadButtonDescription.Button12);
					goto IL_62A;
				case 43:
					observableCollection.AddIfNotContains(GamepadButtonDescription.LeftUpperPaddle);
					observableCollection.AddIfNotContains(GamepadButtonDescription.RightUpperPaddle);
					goto IL_62A;
				case 49:
					observableCollection.AddIfNotContains(GamepadButtonDescription.Button11);
					observableCollection.AddIfNotContains(GamepadButtonDescription.Button12);
					observableCollection.AddIfNotContains(GamepadButtonDescription.Button14);
					observableCollection.AddIfNotContains(GamepadButtonDescription.Button15);
					observableCollection.Remove(GamepadButtonDescription.Button7);
					goto IL_62A;
				default:
					if (controllerType2 != 62)
					{
						goto IL_62A;
					}
					break;
				}
				observableCollection.Add(GamepadButtonDescription.Button11);
				observableCollection.Add(GamepadButtonDescription.Button12);
				observableCollection.Add(GamepadButtonDescription.Button13);
				observableCollection.Add(GamepadButtonDescription.Button14);
				observableCollection.Add(GamepadButtonDescription.Button15);
				goto IL_62A;
			}
			if (controllerType2 != 63)
			{
				if (controllerType2 != 500)
				{
					goto IL_62A;
				}
				observableCollection.AddIfNotContains(GamepadButtonDescription.TrackPad1Click);
				if (includeUnmapable)
				{
					observableCollection.Add(GamepadButtonDescription.TrackPad1Tap);
					goto IL_62A;
				}
				goto IL_62A;
			}
			IL_442:
			if (includeUnmapable)
			{
				observableCollection.Add(GamepadButtonDescription.TrackPad1Up);
				observableCollection.Add(GamepadButtonDescription.TrackPad1Down);
				observableCollection.Add(GamepadButtonDescription.TrackPad1Left);
				observableCollection.Add(GamepadButtonDescription.TrackPad1Right);
				observableCollection.Add(GamepadButtonDescription.TrackPad1Click);
				observableCollection.Add(GamepadButtonDescription.TrackPad1Tap);
				observableCollection.Add(GamepadButtonDescription.TrackPad2Up);
				observableCollection.Add(GamepadButtonDescription.TrackPad2Down);
				observableCollection.Add(GamepadButtonDescription.TrackPad2Left);
				observableCollection.Add(GamepadButtonDescription.TrackPad2Right);
				observableCollection.Add(GamepadButtonDescription.TrackPad2Click);
				observableCollection.Add(GamepadButtonDescription.TrackPad2Tap);
				observableCollection.Add(GamepadButtonDescription.LeftTriggerFullPull);
				observableCollection.Add(GamepadButtonDescription.RightTriggerFullPull);
			}
			IL_62A:
			observableCollection.Remove((GamepadButtonDescription item) => item.Button != 2000 && !ControllersHelper.IsGamepadButtonExist(item.Button, controllerType));
			if (currentButtonDescription != null)
			{
				observableCollection.AddIfNotContains(currentButtonDescription);
			}
			return new ObservableCollection<GamepadButtonDescription>(observableCollection.OrderBy((GamepadButtonDescription b) => b.IndexForSorting));
		}

		public void RecreateKeyScanCodeCollectionForReWASDMappings(VirtualGamepadType virtualGamepadType = 0)
		{
			KeyScanCodeV2 noMap = KeyScanCodeV2.NoMap;
			KeyScanCodeV2 noInheritance = KeyScanCodeV2.NoInheritance;
			IEnumerable<KeyScanCodeV2> enumerable = KeyScanCodeV2.SCAN_CODE_TABLE.Where((KeyScanCodeV2 ksc) => !string.IsNullOrWhiteSpace(ksc.FriendlyName) && ksc.PCKeyCategory != 1 && ksc.PCKeyCategory != 5);
			if (virtualGamepadType == null)
			{
				enumerable = enumerable.Where((KeyScanCodeV2 ksc) => ksc != KeyScanCodeV2.GamepadButton12);
			}
			enumerable = enumerable.OrderByDescending((KeyScanCodeV2 ksc) => ksc.IsMouse);
			List<BaseRewasdMapping> list = enumerable.OfType<BaseRewasdMapping>().ToList<BaseRewasdMapping>();
			list.AddRange(BaseRewasdUserCommand.REWASD_USER_COMMAND_TABLE);
			list.Remove(noMap);
			list.Remove(noInheritance);
			list.Insert(0, noMap);
			this.RewasdMappingsCollection = new ObservableCollection<BaseRewasdMapping>(list);
			this.RewasdMappingsCollectionWithDoNotInherit = new ObservableCollection<BaseRewasdMapping>(this.RewasdMappingsCollection);
			this.RewasdMappingsCollectionWithDoNotInherit.Insert(1, noInheritance);
			List<BaseRewasdMapping> list2 = (from ksc in KeyScanCodeV2.SCAN_CODE_TABLE
				where !string.IsNullOrWhiteSpace(ksc.FriendlyName) && ksc.PCKeyCategory != 1 && ksc.PCKeyCategory != 2
				orderby ksc.IsMouse descending
				select ksc).OfType<BaseRewasdMapping>().ToList<BaseRewasdMapping>();
			list2.AddRange(BaseRewasdUserCommand.REWASD_USER_COMMAND_TABLE);
			list2.Remove(noMap);
			list2.Remove(noInheritance);
			list2.Insert(0, noMap);
			this.RewasdMappingsCollectionWithoutMouseAndScrolls = new ObservableCollection<BaseRewasdMapping>(list2);
			this.RewasdMappingsCollectionWithoutMouseAndScrollsWithDoNotInherit = new ObservableCollection<BaseRewasdMapping>(this.RewasdMappingsCollectionWithoutMouseAndScrolls);
			this.RewasdMappingsCollectionWithoutMouseAndScrollsWithDoNotInherit.Insert(1, noInheritance);
		}

		public void RecreateKeyboardKeysCollectionForKeyboard(BaseControllerVM controllerVM = null)
		{
			this._keyScanCodeCollectionForKeyboard = this.GenerateKeysForController(controllerVM, true, false, false, false);
			this.OnPropertyChanged("KeyScanCodeCollectionForKeyboard");
		}

		public void RecreateKeyboardKeysCollectionForMouse(BaseControllerVM controllerVM = null)
		{
			this._keyScanCodeCollectionForMouse = this.GenerateKeysForController(controllerVM, false, true, false, false);
			this.OnPropertyChanged("KeyScanCodeCollectionForMouse");
		}

		public void RecreateKeyboardKeysCollectionForGamepad(BaseControllerVM controllerVM = null)
		{
			this._keyScanCodeCollectionForGamepad = this.GenerateKeysForController(controllerVM, false, false, false, true);
			this.OnPropertyChanged("KeyScanCodeCollectionForGamepad");
		}

		public void RecreateButtonsForMaskCollection(BaseControllerVM gamepadVM = null)
		{
			this._buttonsForMask = this.GenerateButtonsForController(gamepadVM);
			this.OnPropertyChanged("ButtonsForMask");
		}

		private void RecreateButtonsToRemap(BaseControllerVM gamepadVM = null)
		{
			this._buttonsToRemap = this.GenerateRemapButtonDescriptionsForController(gamepadVM, false, null);
			this.OnPropertyChanged("ButtonsToRemap");
		}

		public void RaiseButtonsToRemapCollectionChanged()
		{
			foreach (GamepadButtonDescription gamepadButtonDescription in this.ButtonsToRemap)
			{
				gamepadButtonDescription.RaisePropertyChanged(null);
				gamepadButtonDescription.RaisePropertyChanged("Button");
			}
		}

		private ObservableCollection<BaseRewasdMapping> _rewasdMappingsCollection;

		private ObservableCollection<KeyScanCodeV2> _keyScanCodeCollectionForKeyboard;

		private ObservableCollection<KeyScanCodeV2> _keyScanCodeCollectionForMouse;

		private ObservableCollection<KeyScanCodeV2> _keyScanCodeCollectionForGamepad;

		private ObservableCollection<BaseRewasdMapping> _rewasdMappingsCollectionWithoutMouseAndScrolls;

		private ObservableCollection<BaseRewasdMapping> _rewasdMappingsCollectionWithDoNotInherit;

		private ObservableCollection<BaseRewasdMapping> _rewasdMappingsCollectionWithoutMouseAndScrollsWithDoNotInherit;

		public ObservableCollection<GamepadButtonDescription> _buttonsToRemap;

		private ObservableCollection<GamepadButton> _buttonsForMask;
	}
}
