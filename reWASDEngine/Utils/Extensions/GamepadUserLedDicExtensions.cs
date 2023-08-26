using System;
using System.Collections.Generic;
using System.Linq;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.LED;
using XBEliteWPF.Services;
using XBEliteWPF.Utils.Extensions;

namespace reWASDEngine.Utils.Extensions
{
	public static class GamepadUserLedDicExtensions
	{
		public static void AddGamepad(this GamepadsPlayerLedDictionary dic, string id, ulong[] ids, uint[] controllerTypes, string displayName)
		{
			List<ControllerTypeEnum> list = ControllerTypeExtensions.ConvertPhysicalTypesToEnums(0, controllerTypes, ids).ToList<ControllerTypeEnum>();
			dic.AddGamepad(id, list[0], displayName);
		}

		public static void AddGamepad(this GamepadsPlayerLedDictionary dic, string ID, ControllerTypeEnum controllerType, string displayName)
		{
			if (!ControllerTypeExtensions.IsGamepad(controllerType))
			{
				return;
			}
			if (dic.ContainsKey(ID))
			{
				if (dic[ID].DisplayName != displayName)
				{
					dic[ID].DisplayName = displayName;
				}
				BinDataSerialize.Save(dic, BinDataSerialize.USER_IMPERSONATED_GAMEPAD_PLAYER_LED_COLLECTION_FULL_SAVE_FILE_PATH);
				return;
			}
			LEDSupportedDevice? ledsupportedDevice = ControllerTypeExtensions.ConvertEnumToLEDSupportedType(controllerType);
			if (ledsupportedDevice != null && LEDSupportedDeviceExtensions.IsPlayerLEDIndicationAllowed(ledsupportedDevice.GetValueOrDefault()))
			{
				PlayerLedSettings playerLedSettings = new PlayerLedSettings
				{
					SupportedDeviceType = ledsupportedDevice.Value,
					DisplayName = displayName,
					LedNumber = 1
				};
				dic.Add(ID, playerLedSettings);
				BinDataSerialize.Save(dic, BinDataSerialize.USER_IMPERSONATED_GAMEPAD_PLAYER_LED_COLLECTION_FULL_SAVE_FILE_PATH);
			}
		}
	}
}
