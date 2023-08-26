using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using DiscSoftReWASDServiceNamespace;
using reWASDCommon.Services.HttpServer;
using reWASDCommon.Utils;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Services.Interfaces;

namespace reWASDEngine.Services.UdpServer
{
	public class UdpServer : ZBindableBase, IUdpServer, IDisposable
	{
		public bool StartServer()
		{
			if (this.StartServerInternal())
			{
				this.isRunning = true;
				return true;
			}
			return false;
		}

		Task<bool> IUdpServer.InitAndRun()
		{
			try
			{
				CrossPlatformLib.Init(AppContext.BaseDirectory);
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "InitAndRun");
			}
			int emulatorPort = HttpServerSettings.GetEmulatorPort();
			bool flag = this.Run(emulatorPort).Result;
			if (!flag)
			{
				flag = this.Run(HttpServerSettings.DEFAULT_EMULATOR_PORT).Result;
				if (flag)
				{
					HttpServerSettings.SetPort(this.port);
				}
			}
			if (!flag && HttpServerSettings.IsEnabledOverLocalNetwork())
			{
				HttpServerSettings.SetEnabledOverLocalNetwork(false);
				flag = this.Run(HttpServerSettings.DEFAULT_EMULATOR_PORT).Result;
				if (flag)
				{
					HttpServerSettings.SetEmulatorPort(this.port);
				}
			}
			return Task.FromResult<bool>(flag);
		}

		public void StopServer()
		{
			this.isRunning = false;
			if (this.listener != null)
			{
				this.listener.Close();
			}
		}

		Task<bool> IUdpServer.Restart()
		{
			if (this.isRunning)
			{
				this.StopServer();
			}
			return this.Run(HttpServerSettings.GetEmulatorPort());
		}

		void IUdpServer.StopAndClose()
		{
			this.StopServer();
			this.Dispose();
		}

		private Task<bool> Run(int port)
		{
			this.port = port;
			this.ip = "0.0.0.0";
			return Task.FromResult<bool>(this.StartServer());
		}

		private bool StartServerInternal()
		{
			try
			{
				this.listener = new UdpClient();
				IPEndPoint groupEP = new IPEndPoint(this.GetIpAddressFromIp(this.ip), this.port);
				this.listener.Client.Bind(groupEP);
				Task.Run(delegate
				{
					try
					{
						while (this.isRunning)
						{
							try
							{
								REWASD_SET_CONTROLLER_STATE_REQUEST rewasd_SET_CONTROLLER_STATE_REQUEST;
								bool flag;
								bool flag2;
								bool flag3;
								if (DataDecompressor.Decompress(this.listener.Receive(ref groupEP), out rewasd_SET_CONTROLLER_STATE_REQUEST, out flag, out flag2, out flag3))
								{
									Engine.EngineControllerMonitor.ResetTimer(rewasd_SET_CONTROLLER_STATE_REQUEST.Id);
									Lazy<IGamepadService> gamepadServiceLazy = Engine.GamepadServiceLazy;
									if (gamepadServiceLazy != null)
									{
										IGamepadService value = gamepadServiceLazy.Value;
										if (value != null)
										{
											EngineControllersWpapper engineControllersWpapper = value.EngineControllersWpapper;
											if (engineControllersWpapper != null)
											{
												engineControllersWpapper.SetEngineControllerState(rewasd_SET_CONTROLLER_STATE_REQUEST, flag, flag2, flag3);
											}
										}
									}
								}
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex);
							}
						}
					}
					finally
					{
						this.listener.Close();
					}
				});
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		private IPAddress GetIpAddressFromIp(string ipStr)
		{
			if (ipStr == "127.0.0.1" || ipStr == "localhost" || ipStr == "")
			{
				return IPAddress.Loopback;
			}
			if (ipStr == "0.0.0.0")
			{
				return IPAddress.Any;
			}
			IPAddress ipaddress = Dns.GetHostAddresses(ipStr).FirstOrDefault((IPAddress _ip) => _ip.AddressFamily == AddressFamily.InterNetwork);
			if (ipaddress == null)
			{
				throw new Exception("Unknown ip address: " + ipStr);
			}
			return ipaddress;
		}

		private string ip;

		private int port;

		private bool isRunning;

		private UdpClient listener;
	}
}
