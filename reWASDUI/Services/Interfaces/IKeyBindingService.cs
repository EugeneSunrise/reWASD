using System;
using System.Collections.ObjectModel;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.Controller;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.reWASDMapping;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;

namespace reWASDUI.Services.Interfaces
{
	public interface IKeyBindingService
	{
		IGameProfilesService GameProfilesService { get; }

		ObservableCollection<BaseRewasdMapping> RewasdMappingsCollection { get; }

		ObservableCollection<KeyScanCodeV2> KeyScanCodeCollectionForKeyboard { get; }

		ObservableCollection<KeyScanCodeV2> KeyScanCodeCollectionForMouse { get; }

		ObservableCollection<KeyScanCodeV2> KeyScanCodeCollectionForGamepad { get; }

		ObservableCollection<BaseRewasdMapping> RewasdMappingsCollectionWithoutMouseAndScrolls { get; }

		ObservableCollection<BaseRewasdMapping> RewasdMappingsCollectionWithDoNotInherit { get; }

		ObservableCollection<BaseRewasdMapping> RewasdMappingsCollectionWithoutMouseAndScrollsWithDoNotInherit { get; }

		ObservableCollection<GamepadButtonDescription> ButtonsToRemap { get; }

		ObservableCollection<GamepadButton> ButtonsForMask { get; }

		ObservableCollection<GamepadButtonDescription> ShiftModifierButtons { get; }

		void RecreateKeyScanCodeCollectionForReWASDMappings(VirtualGamepadType virtualGamepadType = 0);

		void RaiseButtonsToRemapCollectionChanged();

		ObservableCollection<GamepadButton> GenerateButtonsForController(BaseControllerVM gamepadVM = null);

		ObservableCollection<GamepadButtonDescription> GenerateRemapButtonDescriptionsForController(BaseControllerVM gamepadVM = null, bool includeUnmapable = false, GamepadButtonDescription currentButtonDescription = null);

		ObservableCollection<KeyScanCodeV2> GenerateKeysForController(BaseControllerVM controllerVM = null, bool isKeyboardExpected = false, bool isMouseExpected = false, bool isMaskMode = false, bool isGamepad = false);
	}
}
