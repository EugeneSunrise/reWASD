using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using LaunchDarkly.EventSource;
using Newtonsoft.Json;
using Prism.Events;
using reWASDCommon.Network.HTTP.DataTransferObjects.Events;
using reWASDCommon.Network.HTTP.DataTransferObjects.Events.Desktop;
using reWASDCommon.Utils;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Services.Interfaces;
using reWASDUI.Utils.Converters;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Services
{
	public class SSEClient : ZBindableBase, ISSEProcessor, IDisposable
	{
		public SSEClient(IEventAggregator ea)
		{
			this._eventAggregator = ea;
		}

		public static string ClientID
		{
			get
			{
				return SSEClient._clientID;
			}
		}

		public void OnMessageReceived(object sender, MessageReceivedEventArgs args)
		{
			if (args.Message.Name == "UIEvent" || args.Message.Name == "DataEvent")
			{
				return;
			}
			BaseEvent baseEvent = JsonConvert.DeserializeObject<BaseEvent>(args.Message.Data, new JsonConverter[]
			{
				new BaseEventConverter()
			});
			if (baseEvent == null)
			{
				return;
			}
			if (baseEvent.Type > 1000)
			{
				return;
			}
			try
			{
				SSEClient.<>c__DisplayClass7_0 CS$<>8__locals1 = new SSEClient.<>c__DisplayClass7_0();
				CS$<>8__locals1.<>4__this = this;
				CS$<>8__locals1.controllerInfo = baseEvent as GyroCalibrationFinishedEvent;
				if (CS$<>8__locals1.controllerInfo != null)
				{
					ThreadHelper.ExecuteInMainDispatcher(delegate
					{
						CS$<>8__locals1.<>4__this._eventAggregator.GetEvent<GyroCalibrationFinished>().Publish(CS$<>8__locals1.controllerInfo.ControllerId);
					}, true);
				}
				else
				{
					SSEClient.<>c__DisplayClass7_1 CS$<>8__locals2 = new SSEClient.<>c__DisplayClass7_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.batteryLevelChangedEvent = baseEvent as BatteryLevelChangedEvent;
					if (CS$<>8__locals2.batteryLevelChangedEvent != null)
					{
						ThreadHelper.ExecuteInMainDispatcher(delegate
						{
							IEnumerable<BaseControllerVM> allPhysicalControllers = App.GamepadService.AllPhysicalControllers;
							Func<BaseControllerVM, bool> func;
							if ((func = CS$<>8__locals2.<>9__2) == null)
							{
								func = (CS$<>8__locals2.<>9__2 = (BaseControllerVM item) => item.ID == CS$<>8__locals2.batteryLevelChangedEvent.ControllerId);
							}
							ControllerVM controllerVM = allPhysicalControllers.FirstOrDefault(func) as ControllerVM;
							if (controllerVM != null)
							{
								controllerVM.ControllerBatteryLevel = CS$<>8__locals2.batteryLevelChangedEvent.BatteryLevel;
								controllerVM.BatteryLevelPercent = CS$<>8__locals2.batteryLevelChangedEvent.BatteryLevelPercent;
								controllerVM.IsBatteryLevelPercentPresent = CS$<>8__locals2.batteryLevelChangedEvent.IsBatteryLevelPercentPresent;
								controllerVM.ControllerBatteryChargingState = CS$<>8__locals2.batteryLevelChangedEvent.ChargingState;
							}
						}, true);
					}
					else
					{
						SSEClient.<>c__DisplayClass7_2 CS$<>8__locals3 = new SSEClient.<>c__DisplayClass7_2();
						CS$<>8__locals3.CS$<>8__locals2 = CS$<>8__locals2;
						CS$<>8__locals3.gamepadStateChangedEvent = baseEvent as GamepadStateChangedEvent;
						if (CS$<>8__locals3.gamepadStateChangedEvent != null)
						{
							ThreadHelper.ExecuteInMainDispatcher(delegate
							{
								IEnumerable<BaseControllerVM> allPhysicalControllers2 = App.GamepadService.AllPhysicalControllers;
								Func<BaseControllerVM, bool> func2;
								if ((func2 = CS$<>8__locals3.<>9__4) == null)
								{
									func2 = (CS$<>8__locals3.<>9__4 = (BaseControllerVM item) => item.ID == CS$<>8__locals3.gamepadStateChangedEvent.ControllerId);
								}
								BaseControllerVM baseControllerVM = allPhysicalControllers2.FirstOrDefault(func2);
								if (baseControllerVM != null)
								{
									baseControllerVM.IsOnline = CS$<>8__locals3.gamepadStateChangedEvent.IsOnline;
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._eventAggregator.GetEvent<ControllerStateChanged>().Publish(baseControllerVM);
									if (ControllerTypeExtensions.IsAnyEngineController(baseControllerVM.FirstControllerType))
									{
										baseControllerVM.FillFriendlyName();
									}
								}
							}, true);
						}
						else
						{
							SSEClient.<>c__DisplayClass7_3 CS$<>8__locals4 = new SSEClient.<>c__DisplayClass7_3();
							CS$<>8__locals4.CS$<>8__locals3 = CS$<>8__locals3;
							CS$<>8__locals4.slotChangedData = baseEvent as SlotChangedEvent;
							if (CS$<>8__locals4.slotChangedData != null)
							{
								ThreadHelper.ExecuteInMainDispatcher(delegate
								{
									CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._eventAggregator.GetEvent<SlotChanged>().Publish(CS$<>8__locals4.slotChangedData);
								}, true);
							}
							else
							{
								SSEClient.<>c__DisplayClass7_4 CS$<>8__locals5 = new SSEClient.<>c__DisplayClass7_4();
								CS$<>8__locals5.CS$<>8__locals4 = CS$<>8__locals4;
								CS$<>8__locals5.configAppliedData = baseEvent as ConfigAppliedEvent;
								if (CS$<>8__locals5.configAppliedData != null)
								{
									ThreadHelper.ExecuteInMainDispatcher(delegate
									{
										CS$<>8__locals5.CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._eventAggregator.GetEvent<ConfigApplied>().Publish(CS$<>8__locals5.configAppliedData);
									}, true);
								}
								else
								{
									SSEClient.<>c__DisplayClass7_5 CS$<>8__locals6 = new SSEClient.<>c__DisplayClass7_5();
									CS$<>8__locals6.CS$<>8__locals5 = CS$<>8__locals5;
									CS$<>8__locals6.remapStateChanged = baseEvent as RemapStateChangedEvent;
									if (CS$<>8__locals6.remapStateChanged != null)
									{
										ThreadHelper.ExecuteInMainDispatcher(delegate
										{
											SSEClient.<>c__DisplayClass7_5.<<OnMessageReceived>b__7>d <<OnMessageReceived>b__7>d;
											<<OnMessageReceived>b__7>d.<>t__builder = AsyncVoidMethodBuilder.Create();
											<<OnMessageReceived>b__7>d.<>4__this = CS$<>8__locals6;
											<<OnMessageReceived>b__7>d.<>1__state = -1;
											<<OnMessageReceived>b__7>d.<>t__builder.Start<SSEClient.<>c__DisplayClass7_5.<<OnMessageReceived>b__7>d>(ref <<OnMessageReceived>b__7>d);
										}, true);
									}
									else
									{
										SSEClient.<>c__DisplayClass7_6 CS$<>8__locals7 = new SSEClient.<>c__DisplayClass7_6();
										CS$<>8__locals7.CS$<>8__locals6 = CS$<>8__locals6;
										CS$<>8__locals7.controllerData = baseEvent as ControllerConnectedEvent;
										if (CS$<>8__locals7.controllerData != null)
										{
											ThreadHelper.ExecuteInMainDispatcher(delegate
											{
												App.GamepadService.OnControllerConnected((BaseControllerVM)CS$<>8__locals7.controllerData.Controller);
											}, true);
										}
										else
										{
											SSEClient.<>c__DisplayClass7_7 CS$<>8__locals8 = new SSEClient.<>c__DisplayClass7_7();
											CS$<>8__locals8.CS$<>8__locals7 = CS$<>8__locals7;
											CS$<>8__locals8.controllerDisconnectedData = baseEvent as ControllerDisconnectedEvent;
											if (CS$<>8__locals8.controllerDisconnectedData != null)
											{
												ThreadHelper.ExecuteInMainDispatcher(delegate
												{
													App.GamepadService.OnControllerDisconnected(CS$<>8__locals8.controllerDisconnectedData.ControllerId, CS$<>8__locals8.controllerDisconnectedData.ContainerIdString);
												}, true);
											}
											else
											{
												SSEClient.<>c__DisplayClass7_8 CS$<>8__locals9 = new SSEClient.<>c__DisplayClass7_8();
												CS$<>8__locals9.CS$<>8__locals8 = CS$<>8__locals8;
												CS$<>8__locals9.controllerChangedData = baseEvent as ControllerChangedEvent;
												if (CS$<>8__locals9.controllerChangedData != null)
												{
													ThreadHelper.ExecuteInMainDispatcher(delegate
													{
														App.GamepadService.OnControllerChanged(CS$<>8__locals9.controllerChangedData.Controller as BaseControllerVM);
													}, true);
												}
												else if (baseEvent is CompositeSettingsChangedEvent)
												{
													ThreadHelper.ExecuteInMainDispatcher(delegate
													{
														App.GamepadService.BinDataSerialize.LoadCompositeDevicesCollection(false);
													}, true);
												}
												else
												{
													SSEClient.<>c__DisplayClass7_9 CS$<>8__locals10 = new SSEClient.<>c__DisplayClass7_9();
													CS$<>8__locals10.CS$<>8__locals9 = CS$<>8__locals9;
													CS$<>8__locals10.configSavedData = baseEvent as ConfigSavedEvent;
													if (CS$<>8__locals10.configSavedData != null)
													{
														if (CS$<>8__locals10.configSavedData.ClientID != null && !(CS$<>8__locals10.configSavedData.ClientID == SSEClient.ClientID))
														{
															ThreadHelper.ExecuteInMainDispatcher(delegate
															{
																SSEClient.<>c__DisplayClass7_9.<<OnMessageReceived>b__12>d <<OnMessageReceived>b__12>d;
																<<OnMessageReceived>b__12>d.<>t__builder = AsyncVoidMethodBuilder.Create();
																<<OnMessageReceived>b__12>d.<>4__this = CS$<>8__locals10;
																<<OnMessageReceived>b__12>d.<>1__state = -1;
																<<OnMessageReceived>b__12>d.<>t__builder.Start<SSEClient.<>c__DisplayClass7_9.<<OnMessageReceived>b__12>d>(ref <<OnMessageReceived>b__12>d);
															}, false);
														}
													}
													else
													{
														ConfigRenamedEvent configRenamedEvent = baseEvent as ConfigRenamedEvent;
														if (configRenamedEvent != null)
														{
															SSEClient.<>c__DisplayClass7_10 CS$<>8__locals11 = new SSEClient.<>c__DisplayClass7_10();
															CS$<>8__locals11.parameters = configRenamedEvent.Parameters;
															ThreadHelper.ExecuteInMainDispatcher(delegate
															{
																SSEClient.<>c__DisplayClass7_10.<<OnMessageReceived>b__15>d <<OnMessageReceived>b__15>d;
																<<OnMessageReceived>b__15>d.<>t__builder = AsyncVoidMethodBuilder.Create();
																<<OnMessageReceived>b__15>d.<>4__this = CS$<>8__locals11;
																<<OnMessageReceived>b__15>d.<>1__state = -1;
																<<OnMessageReceived>b__15>d.<>t__builder.Start<SSEClient.<>c__DisplayClass7_10.<<OnMessageReceived>b__15>d>(ref <<OnMessageReceived>b__15>d);
															}, false);
														}
														else
														{
															SSEClient.<>c__DisplayClass7_12 CS$<>8__locals12 = new SSEClient.<>c__DisplayClass7_12();
															CS$<>8__locals12.CS$<>8__locals10 = CS$<>8__locals10;
															CS$<>8__locals12.configData = baseEvent as ConfigDeletedEvent;
															if (CS$<>8__locals12.configData != null)
															{
																ThreadHelper.ExecuteInMainDispatcher(delegate
																{
																	IEnumerable<GameVM> gamesCollection = App.GameProfilesService.GamesCollection;
																	Func<GameVM, bool> func3;
																	if ((func3 = CS$<>8__locals12.<>9__20) == null)
																	{
																		func3 = (CS$<>8__locals12.<>9__20 = (GameVM g) => g.Name.Equals(CS$<>8__locals12.configData.GameName));
																	}
																	GameVM gameVM = gamesCollection.FirstOrDefault(func3);
																	ConfigVM configVM;
																	if (gameVM == null)
																	{
																		configVM = null;
																	}
																	else
																	{
																		IEnumerable<ConfigVM> configCollection = gameVM.ConfigCollection;
																		Func<ConfigVM, bool> func4;
																		if ((func4 = CS$<>8__locals12.<>9__21) == null)
																		{
																			func4 = (CS$<>8__locals12.<>9__21 = (ConfigVM c) => c.Name.Equals(CS$<>8__locals12.configData.ConfigName));
																		}
																		configVM = configCollection.FirstOrDefault(func4);
																	}
																	ConfigVM configVM2 = configVM;
																	if (CS$<>8__locals12.configData.ClientID != SSEClient.ClientID && configVM2 != null)
																	{
																		GameVM currentGame = App.GameProfilesService.CurrentGame;
																		if (((currentGame != null) ? currentGame.CurrentConfig : null) == configVM2)
																		{
																			DTMessageBox.Show(DTLocalization.GetString(12403), DTLocalization.GetString(5016), MessageBoxButton.OK, MessageBoxImage.Asterisk);
																		}
																	}
																	if (configVM2 != null)
																	{
																		configVM2.ParentGame.DeleteConfig(configVM2);
																	}
																}, false);
															}
															else
															{
																SSEClient.<>c__DisplayClass7_13 CS$<>8__locals13 = new SSEClient.<>c__DisplayClass7_13();
																CS$<>8__locals13.CS$<>8__locals11 = CS$<>8__locals12;
																CS$<>8__locals13.gameData = baseEvent as GameDeletedEvent;
																if (CS$<>8__locals13.gameData != null)
																{
																	ThreadHelper.ExecuteInMainDispatcher(delegate
																	{
																		SSEClient.<>c__DisplayClass7_13.<<OnMessageReceived>b__22>d <<OnMessageReceived>b__22>d;
																		<<OnMessageReceived>b__22>d.<>t__builder = AsyncVoidMethodBuilder.Create();
																		<<OnMessageReceived>b__22>d.<>4__this = CS$<>8__locals13;
																		<<OnMessageReceived>b__22>d.<>1__state = -1;
																		<<OnMessageReceived>b__22>d.<>t__builder.Start<SSEClient.<>c__DisplayClass7_13.<<OnMessageReceived>b__22>d>(ref <<OnMessageReceived>b__22>d);
																	}, false);
																}
																else
																{
																	GameRenamedEvent gameRenamedEvent = baseEvent as GameRenamedEvent;
																	if (gameRenamedEvent != null)
																	{
																		SSEClient.<>c__DisplayClass7_14 CS$<>8__locals14 = new SSEClient.<>c__DisplayClass7_14();
																		CS$<>8__locals14.parameters = gameRenamedEvent.Parameters;
																		ThreadHelper.ExecuteInMainDispatcher(delegate
																		{
																			SSEClient.<>c__DisplayClass7_14.<<OnMessageReceived>b__24>d <<OnMessageReceived>b__24>d;
																			<<OnMessageReceived>b__24>d.<>t__builder = AsyncVoidMethodBuilder.Create();
																			<<OnMessageReceived>b__24>d.<>4__this = CS$<>8__locals14;
																			<<OnMessageReceived>b__24>d.<>1__state = -1;
																			<<OnMessageReceived>b__24>d.<>t__builder.Start<SSEClient.<>c__DisplayClass7_14.<<OnMessageReceived>b__24>d>(ref <<OnMessageReceived>b__24>d);
																		}, false);
																	}
																	else
																	{
																		LicenseChangedEvent licenseChangedEvent = baseEvent as LicenseChangedEvent;
																		if (licenseChangedEvent != null)
																		{
																			ThreadHelper.ExecuteInMainDispatcher(delegate
																			{
																				App.LicensingService.OnLicenseChanged(licenseChangedEvent.Result, licenseChangedEvent.OnlineActivation);
																			}, true);
																		}
																		else
																		{
																			HoneypotPairingRejectedEvent honeypotPairingRejectedEvent = baseEvent as HoneypotPairingRejectedEvent;
																			if (honeypotPairingRejectedEvent != null)
																			{
																				ThreadHelper.ExecuteInMainDispatcher(delegate
																				{
																					CS$<>8__locals13.CS$<>8__locals11.CS$<>8__locals10.CS$<>8__locals9.CS$<>8__locals8.CS$<>8__locals7.CS$<>8__locals6.CS$<>8__locals5.CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._eventAggregator.GetEvent<HoneypotPairingRejected>().Publish(honeypotPairingRejectedEvent.MacAddress);
																				}, true);
																			}
																			else
																			{
																				ExternalDeviceOutdatedEvent externalDeviceEvent = baseEvent as ExternalDeviceOutdatedEvent;
																				if (externalDeviceEvent != null)
																				{
																					ThreadHelper.ExecuteInMainDispatcher(delegate
																					{
																						DTMessageBox.Show(string.Format(DTLocalization.GetString(12649), externalDeviceEvent.ExternalDevice.Alias, externalDeviceEvent.ExternalDevice.SerialPort), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
																					}, true);
																				}
																				else if (baseEvent is ExternalDevicesChangedEvent)
																				{
																					ThreadHelper.ExecuteInMainDispatcher(delegate
																					{
																						this._eventAggregator.GetEvent<ExternalHelperChanged>().Publish(null);
																					}, true);
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void Run()
		{
			try
			{
				int port = HttpServerSettings.GetPort();
				string actualLocalRoute = HttpServerSettings.GetActualLocalRoute();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 3);
				defaultInterpolatedStringHandler.AppendLiteral("http://");
				defaultInterpolatedStringHandler.AppendFormatted(actualLocalRoute);
				defaultInterpolatedStringHandler.AppendLiteral(":");
				defaultInterpolatedStringHandler.AppendFormatted<int>(port);
				defaultInterpolatedStringHandler.AppendLiteral("/");
				defaultInterpolatedStringHandler.AppendFormatted("v1.7");
				defaultInterpolatedStringHandler.AppendLiteral("/Events");
				Configuration configuration = Configuration.Builder(new Uri(defaultInterpolatedStringHandler.ToStringAndClear())).Build();
				this._eventSource = new EventSource(configuration);
				this._eventSource.MessageReceived += this.OnMessageReceived;
				this._eventSource.StartAsync();
			}
			catch (Exception ex)
			{
				Tracer.TraceWrite("Failed to run SSE", false);
				Tracer.TraceException(ex, "Run");
			}
		}

		public void InitAndRun()
		{
			this.Run();
		}

		public void Restart()
		{
			EventSource eventSource = this._eventSource;
			if (eventSource != null)
			{
				eventSource.Close();
			}
			this.Run();
		}

		public void StopAndClose()
		{
			EventSource eventSource = this._eventSource;
			if (eventSource == null)
			{
				return;
			}
			eventSource.Close();
		}

		public override void Dispose()
		{
			base.Dispose();
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			if (disposing)
			{
				this.StopAndClose();
			}
			this.disposed = true;
		}

		~SSEClient()
		{
			this.Dispose(false);
		}

		private bool _isMessageBoxShown;

		private EventSource _eventSource;

		private IEventAggregator _eventAggregator;

		private static string _clientID = Guid.NewGuid().ToString();

		private bool disposed;
	}
}
