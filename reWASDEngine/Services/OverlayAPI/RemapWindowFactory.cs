using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using DiscSoft.NET.Common.Utils;
using DTOverlay;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.MacroCompilers;
using reWASDEngine.OverlayAPI.RemapWindow;
using XBEliteWPF.DataModels;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDEngine.Services.OverlayAPI
{
	public static class RemapWindowFactory
	{
		public static ConfigData CreateConfigData(string configPath)
		{
			ConfigData configData = XBUtils.CreateConfigData(false);
			JsonInfo jsonInfo = new JsonInfo();
			bool flag;
			if (!MacroCompiler.ParseConfigFile(configPath, configData, ref flag, jsonInfo, true))
			{
				return null;
			}
			return configData;
		}

		public static ConfigData CreateConfigData(string gameName, string profileName)
		{
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			Game game = ((gameProfilesService != null) ? gameProfilesService.GamesCollection.FirstOrDefault((Game g) => g.Name.Equals(gameName)) : null);
			if (game == null)
			{
				return null;
			}
			Config config = game.ConfigCollection.FirstOrDefault((Config c) => c.Name.Equals(profileName));
			if (config == null)
			{
				return null;
			}
			config.ReadConfigFromJsonIfNotLoaded(true);
			return config.ConfigData;
		}

		public static RemapWindow CreateWindow(CreationRemapStyle style, string ID, ConfigData configData, bool isDescriptions = false, AlignType alignType = 0, float transparent = 1f)
		{
			if (configData == null)
			{
				return null;
			}
			RemapWindow remapWindow = new RemapWindow(style);
			remapWindow.Align = alignType;
			remapWindow.IsDescriptionWindow = isDescriptions;
			int num = 10000;
			RemapWindowVM viewModel = remapWindow.ViewModel;
			if (style == CreationRemapStyle.NormalCreation)
			{
				string @string = RegistryHelper.GetString("Overlay", "MonitorMappings", "", false);
				int num2 = (int)typeof(SystemParameters).GetProperty("DpiX", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null);
				double num3 = 96.0 / (double)num2;
				num = (int)((double)OverlayUtils.GetDesktopWorkingArea(@string).Height * num3 * 0.8);
			}
			else
			{
				remapWindow.ViewModel.TablesOrientation = Orientation.Vertical;
				remapWindow.Top += 10000.0;
				viewModel.HeaderVisibility = Visibility.Collapsed;
				Size printSize = PrintHelper.GetPrintSize(new PrintDialog());
				double num4 = 48.0;
				viewModel.ButtonTableMaxWidth = printSize.Width - num4;
			}
			viewModel.ButtonTableMaxHeight = num;
			viewModel.Transparent = transparent;
			viewModel.AlignmentSettings = OverlayUtils.ConvertToWindowsAligment(alignType);
			viewModel.HotKeyButtons = new HotkeysInfo();
			viewModel.HotKeyButtons.FillGroupsForID(ID, isDescriptions ? OverlayUtils.HotkeysType.MappingsDescriptions : OverlayUtils.HotkeysType.Mappings);
			viewModel.IsLabelMode = isDescriptions;
			viewModel.FillEnd();
			BaseControllerVM baseControllerVM = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM item) => item.ID == ID);
			Dictionary<ControllerFamily, int> dictionary = new Dictionary<ControllerFamily, int>();
			foreach (SubConfigData subConfigData in configData)
			{
				if (!dictionary.ContainsKey(subConfigData.ControllerFamily))
				{
					dictionary[subConfigData.ControllerFamily] = 0;
				}
				Dictionary<ControllerFamily, int> dictionary2 = dictionary;
				ControllerFamily controllerFamily = subConfigData.ControllerFamily;
				int num5 = dictionary2[controllerFamily];
				dictionary2[controllerFamily] = num5 + 1;
				int num6 = dictionary[subConfigData.ControllerFamily];
				if (baseControllerVM == null)
				{
					viewModel.AddDeviceInfo(subConfigData, new ControllerTypeEnum?(3), num6 - 1);
					if (!subConfigData.MainXBBindingCollection.IsCollectionHasMasks)
					{
						if (!subConfigData.MainXBBindingCollection.ShiftXBBindingCollections.Any((ShiftXBBindingCollection x) => x.IsCollectionHasMasks))
						{
							continue;
						}
					}
					viewModel.AddMaskInfo(subConfigData, new ControllerTypeEnum?(3), num6 - 1);
				}
				else if (baseControllerVM.GetControllerFamilyCount(subConfigData.ControllerFamily) >= num6)
				{
					ControllerTypeEnum? gamepadTypeByIndex = baseControllerVM.GetGamepadTypeByIndex(num6);
					CompositeControllerVM compositeControllerVM = baseControllerVM as CompositeControllerVM;
					if (compositeControllerVM != null && compositeControllerVM.IsNintendoSwitchJoyConComposite)
					{
						gamepadTypeByIndex = new ControllerTypeEnum?(7);
					}
					if (subConfigData.ControllerFamily == 2)
					{
						gamepadTypeByIndex = new ControllerTypeEnum?(1001);
					}
					if (subConfigData.ControllerFamily == 1)
					{
						gamepadTypeByIndex = new ControllerTypeEnum?(1000);
					}
					viewModel.AddDeviceInfo(subConfigData, gamepadTypeByIndex, num6 - 1);
					if (!subConfigData.MainXBBindingCollection.IsCollectionHasMasks)
					{
						if (!subConfigData.MainXBBindingCollection.ShiftXBBindingCollections.Any((ShiftXBBindingCollection x) => x.IsCollectionHasMasks))
						{
							continue;
						}
					}
					viewModel.AddMaskInfo(subConfigData, viewModel.IsLabelMode ? new ControllerTypeEnum?(3) : gamepadTypeByIndex, viewModel.IsLabelMode ? 0 : (num6 - 1));
				}
			}
			viewModel.ID = ID;
			viewModel.configData = configData;
			return remapWindow;
		}
	}
}
