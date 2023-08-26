using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using DiscSoft.NET.Common.Utils;
using Microsoft.AspNetCore.Mvc;
using Prism.Events;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDCommon.Utils;
using reWASDEngine;
using reWASDEngine.OverlayAPI.RemapWindow;
using reWASDEngine.Services.HttpServer.Data;
using reWASDEngine.Services.OverlayAPI;
using reWASDEngine.Utils;
using XBEliteWPF.Infrastructure;

namespace XBEliteWPF.Services.HttpServer
{
	[Route("v1.7/EngineService")]
	public class EngineServiceController : ControllerBase
	{
		[HttpGet]
		[Route("Refresh")]
		public async Task<IActionResult> Refresh()
		{
			Tracer.TraceWrite("EngineServiceController.Refresh", false);
			await Engine.GameProfilesService.FillGamesCollection();
			await Engine.GamepadService.BinDataSerialize.LoadGamepadProfileRelations(true);
			return this.Ok();
		}

		[HttpGet]
		[Route("WaitForInited")]
		public async Task<IActionResult> WaitForInited()
		{
			await Engine.XBServiceCommunicator.CheckServiceAreStarted();
			await Engine.GameProfilesService.WaitForServiceInited();
			await Engine.GamepadService.WaitForServiceInited();
			Application application = Application.Current;
			if (application != null)
			{
				application.Dispatcher.BeginInvoke(new Action(delegate
				{
					Engine.UserSettingsService.Load();
					Engine.EventAggregator.GetEvent<PreferencesChanged>().Publish(null);
				}), Array.Empty<object>());
			}
			return this.Ok();
		}

		[HttpGet]
		[Route("GetServiceBluetoothDeviceInfo/{flags}")]
		public async Task<IActionResult> GetServiceBluetoothDeviceInfo(uint flags)
		{
			Tracer.TraceWrite("EngineServiceController.GetServiceBluetoothDeviceInfo", false);
			List<BluetoothDeviceInfo> list = await BluetoothUtils.GetServiceBluetoothDeviceInfo(flags);
			return this.Ok(list);
		}

		[HttpGet]
		[Route("UnpairController/{macAdress}")]
		public async Task<IActionResult> UnpairController(ulong macAdress)
		{
			Tracer.TraceWrite("EngineServiceController.UnpairController", false);
			bool flag = await BluetoothUtils.UnpairController(macAdress);
			return this.Ok(flag);
		}

		[HttpGet]
		[Route("IsBluetoothAdapterIsSupportedForNintendoConsole")]
		public async Task<IActionResult> IsBluetoothAdapterIsSupportedForNintendoConsole()
		{
			Tracer.TraceWrite("EngineServiceController.IsBluetoothAdapterIsSupportedForNintendoConsole", false);
			bool flag = await BluetoothUtils.IsBluetoothAdapterIsSupportedForNintendoConsole();
			return this.Ok(flag);
		}

		[HttpGet]
		[Route("GetDuplicateGamepadCollection")]
		public IActionResult GetDuplicateGamepadCollection()
		{
			Tracer.TraceWrite("EngineServiceController.GetDuplicateGamepadCollection", false);
			return this.Ok(Engine.GamepadService.DuplicateGamepadCollection);
		}

		[HttpGet]
		[Route("RequestReloadUserSettings")]
		public IActionResult RequestReloadUserSettings()
		{
			Tracer.TraceWrite("EngineServiceController.RequestReloadUserSettings", false);
			Application application = Application.Current;
			if (application != null)
			{
				application.Dispatcher.BeginInvoke(new Action(delegate
				{
					Engine.UserSettingsService.Load();
					IEventAggregator eventAggregator = Engine.EventAggregator;
					if (eventAggregator == null)
					{
						return;
					}
					eventAggregator.GetEvent<PreferencesChanged>().Publish(null);
				}), Array.Empty<object>());
			}
			return this.Ok();
		}

		[HttpGet]
		[Route("RequestHttpRestart")]
		public IActionResult RequestHttpRestart()
		{
			Tracer.TraceWrite("EngineServiceController.RequestHttpRestart", false);
			this.DoHttpRestartAsync();
			return this.Ok();
		}

		[HttpGet]
		[Route("RequestUdpRestart")]
		public IActionResult RequestUdpRestart()
		{
			Tracer.TraceWrite("EngineServiceController.RequestUdpRestart", false);
			this.DoUdpRestartAsync();
			return this.Ok();
		}

		public async Task<bool> DoHttpRestartAsync()
		{
			return await Engine.HttpServer.Restart();
		}

		public async Task<bool> DoUdpRestartAsync()
		{
			return await Engine.UdpServer.Restart();
		}

		[HttpGet]
		[Route("RequestReloadExternalDevicesData")]
		public IActionResult RequestReloadExternalDevicesData()
		{
			Tracer.TraceWrite("EngineServiceController.RequestReloadExternalDevicesData", false);
			Application application = Application.Current;
			if (application != null)
			{
				application.Dispatcher.BeginInvoke(new Action(delegate
				{
					Engine.GamepadService.BinDataSerialize.LoadExternalDevices();
					Engine.GamepadService.BinDataSerialize.LoadExternalClients();
					Engine.GamepadService.ExternalDeviceRelationsHelper.LoadRelations();
				}), Array.Empty<object>());
			}
			return this.Ok();
		}

		[HttpGet]
		[Route("DeletePromoController/{id}")]
		public void DeletePromoController(string id)
		{
			UtilsDebugMenu.DeletePromoController(id);
		}

		[HttpPost]
		[Route("GenerateXPSFromConfig")]
		public async Task<IActionResult> GenerateXPSFromConfig([FromBody] GenerateXPSFromConfigInfo configInfo)
		{
			EngineServiceController.<>c__DisplayClass13_0 CS$<>8__locals1 = new EngineServiceController.<>c__DisplayClass13_0();
			CS$<>8__locals1.configInfo = configInfo;
			CS$<>8__locals1.configData = RemapWindowFactory.CreateConfigData(CS$<>8__locals1.configInfo.ConfigPath);
			CS$<>8__locals1.isFinished = false;
			CS$<>8__locals1.resonse = ResponseWithError.False;
			IActionResult actionResult;
			if (CS$<>8__locals1.configData != null)
			{
				ThreadHelper.ExecuteInMainDispatcher(delegate
				{
					EngineServiceController.<>c__DisplayClass13_1 CS$<>8__locals2 = new EngineServiceController.<>c__DisplayClass13_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CreationRemapStyle creationRemapStyle = (CS$<>8__locals1.configInfo.IsBlack ? CreationRemapStyle.BlackWhitePrint : CreationRemapStyle.ColorPrint);
					CS$<>8__locals2.remapWindow = RemapWindowFactory.CreateWindow(creationRemapStyle, CS$<>8__locals1.configInfo.GamepadID, CS$<>8__locals1.configData, !CS$<>8__locals1.configInfo.IsMappings, 0, 1f);
					CS$<>8__locals2.remapWindow.ContentRendered += delegate([Nullable(2)] object sender1, EventArgs e1)
					{
						EngineServiceController.<>c__DisplayClass13_1.<<GenerateXPSFromConfig>b__1>d <<GenerateXPSFromConfig>b__1>d;
						<<GenerateXPSFromConfig>b__1>d.<>t__builder = AsyncVoidMethodBuilder.Create();
						<<GenerateXPSFromConfig>b__1>d.<>4__this = CS$<>8__locals2;
						<<GenerateXPSFromConfig>b__1>d.<>1__state = -1;
						<<GenerateXPSFromConfig>b__1>d.<>t__builder.Start<EngineServiceController.<>c__DisplayClass13_1.<<GenerateXPSFromConfig>b__1>d>(ref <<GenerateXPSFromConfig>b__1>d);
					};
					CS$<>8__locals2.remapWindow.Show();
				}, true);
				while (!CS$<>8__locals1.isFinished)
				{
					await Task.Delay(50);
				}
				actionResult = this.Ok(CS$<>8__locals1.resonse);
			}
			else
			{
				actionResult = this.Ok(CS$<>8__locals1.resonse);
			}
			return actionResult;
		}

		[HttpGet]
		[Route("RequestUDPStart/{listenAddress}:{port}")]
		public IActionResult RequestUDPStart(int port, string listenAddress)
		{
			Tracer.TraceWrite("UDPServerHttpGet.RequestUDPStart", false);
			return this.Ok(Engine.GamepadUdpServer.Start(port, listenAddress));
		}

		[HttpGet]
		[Route("RequestUDPStart")]
		public IActionResult RequestUDPStart()
		{
			Tracer.TraceWrite("UDPServerHttpGet.RequestUDPStart", false);
			int value = RegistryHelper.GetValue("Preferences\\Servers", "UdpPort", GamepadUdpServerSettings.DEFAULT_PORTS[0], false);
			return this.Ok(Engine.GamepadUdpServer.Start(value, "127.0.0.1"));
		}

		[HttpGet]
		[Route("RequestUDPStopping")]
		public IActionResult RequesUDPStopping()
		{
			Tracer.TraceWrite("UDPServerHttpGet.RequestUDPStopping", false);
			Engine.GamepadUdpServer.StopServer();
			return this.Ok(Engine.GamepadUdpServer.IsRunning);
		}

		[HttpGet]
		[Route("IsUdpRunning")]
		public IActionResult IsUdpRunning()
		{
			Tracer.TraceWrite("UDPServerHttpGet.IsUdpRunning", false);
			return this.Ok(Engine.GamepadUdpServer.IsRunning);
		}

		[HttpGet]
		[Route("GetIsUdpEnabledInPreferences")]
		public IActionResult GetIsUdpEnabledInPreferences()
		{
			Tracer.TraceWrite("UDPServerHttpGet.GetIsUdpEnabledInPreferences", false);
			return this.Ok(Engine.GamepadUdpServer.IsUdpEnabledInPreferences);
		}

		[HttpGet]
		[Route("SetIsUdpEnabledInPreferences/{value}")]
		public IActionResult SetIsUdpEnabledInPreferences(bool value)
		{
			Tracer.TraceWrite("UDPServerHttpGet.SetIsUdpEnabledInPreferences", false);
			Engine.GamepadUdpServer.IsUdpEnabledInPreferences = value;
			return this.Ok();
		}

		[HttpGet]
		[Route("GetGamepadUdpServerState")]
		public IActionResult GetGamepadUdpServerState()
		{
			Tracer.TraceWrite("UDPServerHttpGet.GetGamepadUdpServerState", false);
			return this.Ok(Engine.GamepadUdpServer.GetGamepadUdpServerState());
		}

		[HttpGet]
		[Route("IsUdpReserved")]
		public IActionResult IsUdpReserved()
		{
			Tracer.TraceWrite("UDPServerHttpGet.IsUdpReserved", false);
			return this.Ok(Engine.GamepadUdpServer.IsUdpReserved);
		}

		[HttpGet]
		[Route("IsUdpServerHasException")]
		public IActionResult IsUdpServerHasException()
		{
			Tracer.TraceWrite("UDPServerHttpGet.IsUdpServerHasException", false);
			return this.Ok(Engine.GamepadUdpServer.IsUdpServerHasException);
		}
	}
}
