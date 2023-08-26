using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DiscSoft.NET.Common.Utils;
using DiscSoftReWASDServiceNamespace;
using reWASDCommon.Infrastructure.Enums;
using reWASDEngine;
using Unity;
using Unity.Resolution;
using XBEliteWPF.DataModels;
using XBEliteWPF.DataModels.GamepadActiveProfiles;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;

namespace XBEliteWPF.Services
{
	public class EventProcessor : IEventProcessor
	{
		public event HiddenCommandEventHandler OverlayShiftChange;

		public event ControllerSlotChangedHandler OverlaySlotChange;

		public event GamepadHotkeyHandler OverlayGamepadHotkey;

		public event MappingHotkeyHandler OverlayMappingHotkey;

		public event HideRadialMenuHandler OverlayHideRadialMenu;

		public EventProcessor(IUnityContainer uc)
		{
			this._gameProfilesService = UnityContainerExtensions.Resolve<IGameProfilesService>(uc, Array.Empty<ResolverOverride>());
			this._gamepadService = UnityContainerExtensions.Resolve<IGamepadService>(uc, Array.Empty<ResolverOverride>());
			this._userSettingsService = UnityContainerExtensions.Resolve<IUserSettingsService>(uc, Array.Empty<ResolverOverride>());
			this._ledService = UnityContainerExtensions.Resolve<ILEDService>(uc, Array.Empty<ResolverOverride>());
		}

		public async void ProcessHiddenServiceCommand(ushort profileID, RewasdHiddenServiceCommand rewasdHiddenServiceCommand)
		{
			EventProcessor.<>c__DisplayClass20_0 CS$<>8__locals1 = new EventProcessor.<>c__DisplayClass20_0();
			CS$<>8__locals1.profileID = profileID;
			Tracer.TraceWrite("ProcessHiddenServiceCommand", false);
			if (rewasdHiddenServiceCommand == 26 || rewasdHiddenServiceCommand == 27 || rewasdHiddenServiceCommand == 28)
			{
				if (rewasdHiddenServiceCommand == 26)
				{
					Tracer.TraceWrite("ProcessHiddenServiceCommand: OverlayShowGamepad", false);
				}
				if (rewasdHiddenServiceCommand == 27)
				{
					Tracer.TraceWrite("ProcessHiddenServiceCommand: OverlayShowMappings", false);
				}
				if (rewasdHiddenServiceCommand == 28)
				{
					Tracer.TraceWrite("ProcessHiddenServiceCommand: OverlayShowMappingDescriptions", false);
				}
				EventProcessor.<>c__DisplayClass20_0 CS$<>8__locals2 = CS$<>8__locals1;
				Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper = this._gamepadService.ServiceProfilesCollection.FirstOrDefault((Wrapper<REWASD_CONTROLLER_PROFILE_EX> sp) => sp.Value.SlotManagerProfileId == CS$<>8__locals1.profileID);
				CS$<>8__locals2.profile = ((wrapper != null) ? wrapper.Value.GetCurrentSlotProfile() : null);
			}
			else
			{
				EventProcessor.<>c__DisplayClass20_0 CS$<>8__locals3 = CS$<>8__locals1;
				Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper2 = this._gamepadService.ServiceProfilesCollection.FirstOrDefault((Wrapper<REWASD_CONTROLLER_PROFILE_EX> sp) => sp.Value.ServiceProfileIds.Contains(CS$<>8__locals1.profileID));
				CS$<>8__locals3.profile = ((wrapper2 != null) ? wrapper2.Value.FindProfileById(CS$<>8__locals1.profileID) : null);
			}
			if (CS$<>8__locals1.profile != null)
			{
				Tracer.TraceWrite("ProcessHiddenServiceCommand: profile != null. Proceed", false);
				if (rewasdHiddenServiceCommand.IsLEDReactionRequired())
				{
					this.ProcessLEDReactionRequiredCommand(CS$<>8__locals1.profile, rewasdHiddenServiceCommand);
				}
				ulong[] array = CS$<>8__locals1.profile.Value.Id.ToArray<ulong>();
				CS$<>8__locals1.Id = XBUtils.CalculateID(array);
				GamepadProfiles gamepadProfiles = this._gamepadService.GamepadProfileRelations.FirstOrDefault((KeyValuePair<string, GamepadProfiles> kvp) => CS$<>8__locals1.Id == kvp.Key).Value;
				if (gamepadProfiles == null)
				{
					gamepadProfiles = this._gamepadService.GamepadProfileRelations.FirstOrDefault((KeyValuePair<string, GamepadProfiles> kvp) => CS$<>8__locals1.profile.Value.Id.Any((ulong id) => id != 0UL && kvp.Key.Contains(id.ToString()))).Value;
				}
				if (gamepadProfiles == null)
				{
					Tracer.TraceWriteTag(" - LEDService", "ProcessHiddenServiceCommand: gamepadProfiles not found - unexpected situation", false);
					Tracer.TraceWrite("ProcessHiddenServiceCommand: gamepadProfiles not found - unexpected situation!", false);
				}
				else
				{
					Slot? slot = null;
					BaseControllerVM controller = gamepadProfiles.GetAssociatedController();
					if (controller == null)
					{
						Tracer.TraceWrite("ProcessHiddenServiceCommand: controller not found!", false);
					}
					if (controller != null)
					{
						bool flag = false;
						Tracer.TraceWrite("ProcessHiddenServiceCommand: controller != null. Proceed", false);
						Slot slot2 = REWASD_CONTROLLER_PROFILE_Extensions.GetSlot(CS$<>8__locals1.profile.Value);
						GamepadProfile gamepadProfile2 = gamepadProfiles.SlotProfiles.TryGetValue(slot2);
						if ((rewasdHiddenServiceCommand >= 0 && rewasdHiddenServiceCommand <= 21) || (rewasdHiddenServiceCommand >= 30 && rewasdHiddenServiceCommand <= 50))
						{
							bool flag2 = rewasdHiddenServiceCommand >= 30 && rewasdHiddenServiceCommand <= 50;
							string shiftName = this.GetShiftName(gamepadProfiles, controller, rewasdHiddenServiceCommand);
							HiddenCommandEventHandler overlayShiftChange = this.OverlayShiftChange;
							if (overlayShiftChange != null)
							{
								overlayShiftChange(rewasdHiddenServiceCommand, gamepadProfile2, shiftName, controller, flag2);
							}
						}
						switch (rewasdHiddenServiceCommand)
						{
						case 22:
							slot = new Slot?(0);
							break;
						case 23:
							slot = new Slot?(1);
							break;
						case 24:
							slot = new Slot?(2);
							break;
						case 25:
							slot = new Slot?(3);
							break;
						case 26:
						case 27:
						case 28:
						case 29:
							slot = new Slot?(REWASD_CONTROLLER_PROFILE_Extensions.GetSlot(CS$<>8__locals1.profile.Value));
							flag = rewasdHiddenServiceCommand == 28;
							break;
						}
						if (slot == null)
						{
							Tracer.TraceWrite("ProcessHiddenServiceCommand: slot not found!", false);
						}
						if (slot != null)
						{
							Tracer.TraceWrite("ProcessHiddenServiceCommand: slot != null. Proceed", false);
							Slot value = slot.Value;
							GamepadProfile gamepadProfile = gamepadProfiles.SlotProfiles.TryGetValue(value);
							controller.FillFriendlyName();
							switch (rewasdHiddenServiceCommand)
							{
							case 26:
							{
								Tracer.TraceWrite("ProcessHiddenServiceCommand: OverlayGamepadHotkey?.Invoke ", false);
								GamepadHotkeyHandler overlayGamepadHotkey = this.OverlayGamepadHotkey;
								if (overlayGamepadHotkey != null)
								{
									overlayGamepadHotkey(controller.ID, controller.ControllerDisplayName, (gamepadProfile != null) ? gamepadProfile.GameName : "", (gamepadProfile != null) ? gamepadProfile.ProfileName : "");
								}
								break;
							}
							case 27:
							case 28:
							{
								Tracer.TraceWrite("ProcessHiddenServiceCommand: OverlayMappingHotkey?.Invoke ", false);
								MappingHotkeyHandler overlayMappingHotkey = this.OverlayMappingHotkey;
								if (overlayMappingHotkey != null)
								{
									overlayMappingHotkey(controller.ID, controller.ControllerDisplayName, (gamepadProfile != null) ? gamepadProfile.GameName : "", (gamepadProfile != null) ? gamepadProfile.ProfileName : "", flag);
								}
								break;
							}
							case 29:
							{
								Tracer.TraceWrite("ProcessHiddenServiceCommand: OverlayHideRadialMenu?.Invoke ", false);
								HideRadialMenuHandler overlayHideRadialMenu = this.OverlayHideRadialMenu;
								if (overlayHideRadialMenu != null)
								{
									overlayHideRadialMenu();
								}
								break;
							}
							default:
							{
								Tracer.TraceWrite("ProcessHiddenServiceCommand: OverlaySlotChange?.Invoke ", false);
								Slot currentSlot = controller.CurrentSlot;
								Slot? slot3 = slot;
								if (!((currentSlot == slot3.GetValueOrDefault()) & (slot3 != null)))
								{
									await controller.RefreshCurrentSlot();
									RegistryHelper.SetValue("Controllers\\" + controller.ShortID, "CurrentSlot", slot.Value);
									ControllerSlotChangedHandler overlaySlotChange = this.OverlaySlotChange;
									if (overlaySlotChange != null)
									{
										overlaySlotChange(controller, gamepadProfile, slot.Value, CS$<>8__locals1.profile, false);
									}
								}
								break;
							}
							}
							gamepadProfile = null;
						}
					}
				}
			}
		}

		private string GetShiftName(GamepadProfiles gamepadProfiles, BaseControllerVM controller, RewasdHiddenServiceCommand command)
		{
			string text = null;
			Slot currentSlot = controller.CurrentSlot;
			GamepadProfile gamepadProfile = gamepadProfiles.SlotProfiles.TryGetValue(currentSlot);
			string gameName = "";
			string profileName = "";
			if (gamepadProfile != null)
			{
				gameName = gamepadProfile.GameName;
				profileName = gamepadProfile.ProfileName;
				string.Format("{0}: {1}", gameName, profileName);
				IGameProfilesService gameProfilesService = Engine.GameProfilesService;
				Game game = ((gameProfilesService != null) ? gameProfilesService.GamesCollection.FirstOrDefault((Game g) => g.Name.Equals(gameName)) : null);
				if (game != null)
				{
					Config config = game.ConfigCollection.FirstOrDefault((Config c) => c.Name.Equals(profileName));
					if (config != null)
					{
						config.ReadConfigFromJsonIfNotLoaded(false);
						using (List<SubConfigData>.Enumerator enumerator = config.ConfigData.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								SubConfigData subConfigData = enumerator.Current;
								int num = 0;
								if (command >= 0 && command <= 21)
								{
									num = (int)command;
								}
								if (command >= 30 && command <= 50)
								{
									num = (int)(command - 30 + 1);
								}
								BaseXBBindingCollection collectionByLayer = subConfigData.MainXBBindingCollection.GetCollectionByLayer(num);
								if (collectionByLayer != null)
								{
									text = collectionByLayer.Description;
								}
							}
						}
					}
				}
			}
			return text;
		}

		private async void ProcessLEDReactionRequiredCommand(REWASD_CONTROLLER_PROFILE? profile, RewasdHiddenServiceCommand rewasdHiddenServiceCommand)
		{
			Tracer.TraceWriteTag(" - LEDService", "Led reaction required", false);
			string text = " - LEDService";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Command: ");
			defaultInterpolatedStringHandler.AppendFormatted<RewasdHiddenServiceCommand>(rewasdHiddenServiceCommand);
			Tracer.TraceWriteTag(text, defaultInterpolatedStringHandler.ToStringAndClear(), false);
			if (profile != null && Engine.UserSettingsService.IsLedSettingsEnabled)
			{
				ulong[] array = profile.Value.Id.ToArray<ulong>();
				string Id = XBUtils.CalculateID(array);
				GamepadProfiles gamepadProfiles = this._gamepadService.GamepadProfileRelations.FirstOrDefault((KeyValuePair<string, GamepadProfiles> kvp) => Id == kvp.Key).Value;
				if (gamepadProfiles == null)
				{
					Tracer.TraceWriteTag(" - LEDService", "gamepadProfiles not found - unexpected situation", false);
				}
				else
				{
					BaseControllerVM controller = gamepadProfiles.GetAssociatedController();
					if (controller == null)
					{
						Tracer.TraceWriteTag(" - LEDService", "ProcessLEDReactionRequiredCommand Controller not found - unexpected situation", false);
					}
					else
					{
						await controller.RefreshCurrentSlot();
						Slot slot = controller.CurrentSlot;
						int num = 0;
						if (rewasdHiddenServiceCommand >= 1 && rewasdHiddenServiceCommand <= 21)
						{
							num = (int)rewasdHiddenServiceCommand;
						}
						if (rewasdHiddenServiceCommand >= 30 && rewasdHiddenServiceCommand <= 50)
						{
							num = (int)(rewasdHiddenServiceCommand - 30 + 1);
						}
						if (rewasdHiddenServiceCommand != null)
						{
							switch (rewasdHiddenServiceCommand)
							{
							case 22:
								slot = 0;
								break;
							case 23:
								slot = 1;
								break;
							case 24:
								slot = 2;
								break;
							case 25:
								slot = 3;
								break;
							}
						}
						GamepadProfile gamepadProfile = gamepadProfiles.SlotProfiles.TryGetValue(slot);
						await this._ledService.ApplyLEDsToControllerAccordingToSettings(controller, gamepadProfile, new Slot?(slot), num, false, false);
					}
				}
			}
		}

		private IGameProfilesService _gameProfilesService;

		private IGamepadService _gamepadService;

		private IUserSettingsService _userSettingsService;

		private ILEDService _ledService;
	}
}
