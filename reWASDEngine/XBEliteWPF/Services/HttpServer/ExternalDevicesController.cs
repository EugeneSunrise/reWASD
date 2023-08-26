using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Utils;
using Microsoft.AspNetCore.Mvc;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDEngine;
using XBEliteWPF.DataModels;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Infrastructure.ExternalDeviceRelationsModel;
using XBEliteWPF.Infrastructure.ExternalDevices;
using XBEliteWPF.Services.Interfaces;

namespace XBEliteWPF.Services.HttpServer
{
	[Route("v1.7/ExternalDevices")]
	public class ExternalDevicesController : ControllerBase
	{
		[HttpGet]
		[Route("ExternalDeviceRelations")]
		public IActionResult ExternalDeviceRelations()
		{
			ExternalDeviceRelationsCollection externalDeviceRelationsCollection = new ExternalDeviceRelationsCollection();
			if (externalDeviceRelationsCollection.Deserialize(BinDataSerialize.USER_IMPERSONATED_EXTERNAL_DEVICES_RELATIONS_COLLECTION_FULL_SAVE_FILE_PATH))
			{
				return this.Ok(externalDeviceRelationsCollection);
			}
			return this.Ok();
		}

		[HttpPost]
		[Route("SaveExternalDeviceRelations")]
		public IActionResult SaveExternalDeviceRelations([FromBody] ExternalDeviceRelationsCollection externalDevicesRelationsCollection)
		{
			bool flag = externalDevicesRelationsCollection.Serialize(BinDataSerialize.USER_IMPERSONATED_EXTERNAL_DEVICES_RELATIONS_COLLECTION_FULL_SAVE_FILE_PATH);
			Engine.GamepadService.ExternalDeviceRelationsHelper.LoadRelations();
			return this.Ok(flag);
		}

		[HttpGet]
		[Route("ExternalDevices")]
		public IActionResult ExternalDevices()
		{
			return this.Ok(Engine.GamepadService.ExternalDevices);
		}

		[HttpPost]
		[Route("SaveExternalDevices")]
		public IActionResult SaveExternalDevices([FromBody] ExternalDevicesCollection externalDevices)
		{
			Engine.GamepadService.ExternalDevices = externalDevices;
			return this.Ok(Engine.GamepadService.BinDataSerialize.SaveExternalDevices());
		}

		[HttpGet]
		[Route("ExternalClients")]
		public IActionResult ExternalClients()
		{
			return this.Ok(Engine.GamepadService.ExternalClients);
		}

		[HttpGet]
		[Route("ExternalDeviceDisableRemapForSerialPort/{serialPort}")]
		public async Task<IActionResult> ExternalDeviceDisableRemapForSerialPort(string serialPort)
		{
			Tracer.TraceWrite("EngineServiceController.ExternalDeviceDisableRemapForSerialPort: " + serialPort, false);
			await Engine.GamepadService.ExternalDeviceRelationsHelper.ExternalDeviceDisableRemapForSerialPort(serialPort);
			return this.Ok();
		}

		[HttpPost]
		[Route("SaveExternalClients")]
		public IActionResult SaveExternalClients([FromBody] ObservableCollection<ExternalClient> externalClients)
		{
			Engine.GamepadService.ExternalClients = externalClients;
			return this.Ok(Engine.GamepadService.BinDataSerialize.SaveExternalClients());
		}

		[HttpPost]
		[Route("ExternalDeviceBluetoothReconnect")]
		public async Task<IActionResult> ExternalDeviceBluetoothReconnect([FromBody] SelectSlotInfo slotInfo)
		{
			Tracer.TraceWrite("EngineServiceController.ExternalDeviceBluetoothReconnect", false);
			await Engine.GamepadService.ExternalDeviceRelationsHelper.ExternalDeviceBluetoothReconnect(slotInfo.ID, slotInfo.Slot);
			return this.Ok();
		}

		[HttpPost]
		[Route("GetExternalState")]
		public IActionResult GetExternalState([FromBody] GetExternalStateInfo info)
		{
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			Game game = ((gameProfilesService != null) ? gameProfilesService.GamesCollection.FirstOrDefault((Game g) => g.Name.Equals(info.GameName)) : null);
			if (game == null)
			{
				return this.NotFound();
			}
			Config config = game.ConfigCollection.FirstOrDefault((Config c) => c.Name.Equals(info.ConfigName));
			if (config == null)
			{
				return this.NotFound();
			}
			BaseControllerVM baseControllerVM = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == info.ControllerId);
			ExternalState externalState = Engine.GamepadService.ExternalDeviceRelationsHelper.GetExternalState(info.IsConfigExternal, info.ConfigVirtualGamepadType, config, baseControllerVM, info.Slot, info.ExternalDevice, info.ExternalClient, info.GamepadAuth);
			return this.Ok(externalState);
		}

		[HttpPost]
		[Route("GetExternalDeviceState")]
		public IActionResult GetExternalDeviceState([FromBody] GetExternalStateInfo info)
		{
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			Game game = ((gameProfilesService != null) ? gameProfilesService.GamesCollection.FirstOrDefault((Game g) => g.Name.Equals(info.GameName)) : null);
			if (game == null)
			{
				return this.NotFound();
			}
			Config config = game.ConfigCollection.FirstOrDefault((Config c) => c.Name.Equals(info.ConfigName));
			if (config == null)
			{
				return this.NotFound();
			}
			BaseControllerVM baseControllerVM = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == info.ControllerId);
			ExternalDeviceState externalDeviceState = Engine.GamepadService.ExternalDeviceRelationsHelper.GetExternalDeviceState(info.IsConfigExternal, info.ConfigVirtualGamepadType, config, baseControllerVM, info.Slot, info.ExternalDevice, info.ExternalClient, info.GamepadAuth, null);
			return this.Ok(externalDeviceState);
		}

		[HttpPost]
		[Route("GetExternalDeviceStateWithProfiles")]
		public IActionResult GetExternalDeviceStateWithProfiles([FromBody] GetExternalStateInfo info)
		{
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			Game game = ((gameProfilesService != null) ? gameProfilesService.GamesCollection.FirstOrDefault((Game g) => g.Name.Equals(info.GameName)) : null);
			if (game == null)
			{
				return this.NotFound();
			}
			Config config = game.ConfigCollection.FirstOrDefault((Config c) => c.Name.Equals(info.ConfigName));
			if (config == null)
			{
				return this.NotFound();
			}
			BaseControllerVM baseControllerVM = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == info.ControllerId);
			ExternalDeviceState externalDeviceStateWithProfiles = Engine.GamepadService.ExternalDeviceRelationsHelper.GetExternalDeviceStateWithProfiles(info.IsConfigExternal, info.ConfigVirtualGamepadType, config, baseControllerVM, info.Slot, info.ExternalDevice, info.ExternalClient, info.GamepadAuth, null);
			return this.Ok(externalDeviceStateWithProfiles);
		}
	}
}
