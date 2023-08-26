using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DiscSoft.NET.Common.Utils;
using DiscSoftReWASDServiceNamespace;
using reWASDCommon.Infrastructure.UDP;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDCommon.Utils;
using reWASDCommon.Utils.Converters;
using reWASDCommon.Utils.Crc32;
using XBEliteWPF.Infrastructure;

namespace reWASDEngine.Services.HttpServer
{
	public class GamepadUdpServer
	{
		public bool IsUdpReserved
		{
			get
			{
				return this.IsUdpEnabledInPreferences && !this.IsUdpServerHasException;
			}
		}

		public bool IsUdpEnabledInPreferences
		{
			get
			{
				return RegistryHelper.GetValue("Preferences\\Servers", "IsUdpSelected", 1, false) == 1;
			}
			set
			{
				RegistryHelper.SetValue("Preferences\\Servers", "IsUdpSelected", value);
			}
		}

		private int GetPortFromPreferences
		{
			get
			{
				return RegistryHelper.GetValue("Preferences\\Servers", "UdpPort", GamepadUdpServerSettings.DEFAULT_PORTS[0], false);
			}
		}

		public int CurrentPort
		{
			get
			{
				Socket udpSocket = this.UdpSocket;
				IPEndPoint ipendPoint = (IPEndPoint)((udpSocket != null) ? udpSocket.LocalEndPoint : null);
				if (ipendPoint == null)
				{
					return this.GetPortFromPreferences;
				}
				return ipendPoint.Port;
			}
		}

		public bool AddUDPController(ushort serviceProfileId)
		{
			try
			{
				ulong[] padIds = this.GetControllerIds(serviceProfileId);
				Func<ulong, bool> <>9__3;
				bool flag = this.PadList.Exists(delegate(PadMeta x)
				{
					IEnumerable<ulong> enumerable = x.PadIds.Where((ulong y) => y > 0UL);
					Func<ulong, bool> func;
					if ((func = <>9__3) == null)
					{
						func = (<>9__3 = (ulong id) => padIds.Where((ulong z) => z > 0UL).Contains(id));
					}
					return enumerable.All(func);
				});
				if (flag)
				{
					Func<ulong, bool> <>9__7;
					PadMeta padMeta = this.PadList.Find(delegate(PadMeta x)
					{
						IEnumerable<ulong> enumerable2 = x.PadIds.Where((ulong y) => y > 0UL);
						Func<ulong, bool> func2;
						if ((func2 = <>9__7) == null)
						{
							func2 = (<>9__7 = (ulong id) => padIds.Where((ulong z) => z > 0UL).Contains(id));
						}
						return enumerable2.All(func2);
					});
					int num = this.PadList.IndexOf(padMeta);
					padMeta.ServiceProfileId = serviceProfileId;
					this.PadList[num] = padMeta;
				}
				if (this.PadList.Count < 4)
				{
					if (!flag)
					{
						byte padNum2;
						byte padNum;
						for (padNum = 0; padNum < 4; padNum = padNum2 + 1)
						{
							if (!this.PadList.Exists((PadMeta x) => x.PadNum == padNum))
							{
								this.PadList.Add(PadData.InitializeNewPadMeta(padNum, serviceProfileId, padIds));
								break;
							}
							padNum2 = padNum;
						}
					}
					if (this.PadList.Count != 0 && this.IsUdpEnabledInPreferences)
					{
						Application application = Application.Current;
						if (application != null)
						{
							application.Dispatcher.BeginInvoke(new Action(delegate
							{
								this.Start();
							}), Array.Empty<object>());
						}
					}
				}
				else if (!flag)
				{
					int num2 = this.PadList.IndexOf(this.PadList.Find((PadMeta x) => x.PadNum == 0));
					this.PadList[num2] = PadData.InitializeNewPadMeta((byte)num2, serviceProfileId, padIds);
				}
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		public bool DeleteUDPController(ushort serviceProfileId)
		{
			try
			{
				this.GetControllerIds(serviceProfileId);
				if (this.PadList.Exists((PadMeta x) => x.ServiceProfileId == serviceProfileId))
				{
					this.PadList.RemoveAll((PadMeta x) => x.ServiceProfileId == serviceProfileId);
				}
				if (this.PadList.Count == 0)
				{
					Tracer.TraceWrite("DeleteUDPController => PadList.Count == 0", false);
					Application application = Application.Current;
					if (application != null)
					{
						application.Dispatcher.BeginInvoke(new Action(delegate
						{
							this.StopServer();
						}), Array.Empty<object>());
					}
				}
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		public List<PadMeta> GetControllers()
		{
			return this.PadList;
		}

		public List<ushort> GetControllersServiceIds()
		{
			return this.PadList.Select((PadMeta client) => client.ServiceProfileId).ToList<ushort>();
		}

		public ulong[] GetControllerIds(ushort serviceProfileID)
		{
			REWASD_CONTROLLER_PROFILE_EX profileExByServiceProfileId = Engine.GamepadService.GetProfileExByServiceProfileId((ulong)serviceProfileID);
			if (profileExByServiceProfileId != null)
			{
				REWASD_CONTROLLER_PROFILE? rewasd_CONTROLLER_PROFILE = profileExByServiceProfileId.FindProfileById(serviceProfileID);
				if (rewasd_CONTROLLER_PROFILE != null)
				{
					return rewasd_CONTROLLER_PROFILE.Value.Id;
				}
			}
			return null;
		}

		public GamepadUdpServerState GetGamepadUdpServerState()
		{
			return new GamepadUdpServerState
			{
				IsUdpEnabledInPreferences = this.IsUdpEnabledInPreferences,
				IsUdpRunning = this.IsRunning,
				IsUdpServerHasException = this.IsUdpServerHasException,
				PadList = this.PadList,
				Port = this.CurrentPort,
				IsUdpReserved = this.IsUdpReserved
			};
		}

		public void AsyncStart()
		{
			Application application = Application.Current;
			if (application == null)
			{
				return;
			}
			application.Dispatcher.BeginInvoke(new Action(delegate
			{
				this.Start();
			}), Array.Empty<object>());
		}

		public bool Start()
		{
			return this.Start(this.GetPortFromPreferences, "");
		}

		public bool Start(int port, string listenAddress = "")
		{
			bool flag;
			try
			{
				if (this.PadList.Count > 0 && this.IsUdpEnabledInPreferences)
				{
					if (this.IsRunning && port != this.CurrentPort)
					{
						this.StopServer();
					}
					if (!this.IsRunning)
					{
						this.StartServer(port, listenAddress);
						this.UdpPooler();
					}
					flag = true;
				}
				else
				{
					if (this.IsRunning)
					{
						this.StopServer();
					}
					flag = false;
				}
			}
			catch (Exception)
			{
				this.IsUdpServerHasException = true;
				flag = false;
			}
			return flag;
		}

		public void StopServer()
		{
			Tracer.TraceWrite("DeleteUDPController => StopServer start", false);
			this.IsRunning = false;
			this.IsUdpServerHasException = false;
			if (this.UdpSocket != null)
			{
				this.UdpSocket.Close();
				this.UdpSocket = null;
				this.Clients.Clear();
			}
		}

		public bool IsPortNotLockedByOtherApplication()
		{
			bool flag = true;
			if (!this.IsRunning)
			{
				try
				{
					new UdpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), this.CurrentPort)).Close();
				}
				catch (Exception)
				{
					flag = false;
				}
			}
			return flag;
		}

		public GamepadUdpServer()
		{
			this._pool = new SemaphoreSlim(80);
			this.ArgsList = new SocketAsyncEventArgs[80];
			for (int i = 0; i < 80; i++)
			{
				SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
				socketAsyncEventArgs.SetBuffer(new byte[100], 0, 100);
				socketAsyncEventArgs.Completed += this.SocketEvent_Completed;
				this.ArgsList[i] = socketAsyncEventArgs;
			}
		}

		private async Task UdpPooler()
		{
			while (this.IsRunning)
			{
				await Task.Delay(TimeSpan.FromMilliseconds(4.0));
				for (int index = 0; index < this.PadList.Count; index++)
				{
					PadMeta padMeta = this.PadList[index];
					byte[] array = await Engine.XBServiceCommunicator.GetVirtualControllerReport(padMeta.ServiceProfileId);
					if (array != null)
					{
						padMeta.InputReport = array;
						this.PadList[index] = padMeta;
						padMeta = default(PadMeta);
					}
				}
				if (this.Clients.Count > 0 && this.PadList.Count > 0)
				{
					this.NewReportIncoming();
				}
			}
		}

		private void SocketEvent_Completed(object sender, SocketAsyncEventArgs e)
		{
			this._pool.Release();
		}

		private void CompletedSynchronousSocketEvent()
		{
			this._pool.Release();
		}

		private void StartServer(int port, string listenAddress = "")
		{
			if (this.IsRunning)
			{
				if (this.UdpSocket != null)
				{
					this.UdpSocket.Close();
					this.UdpSocket = null;
				}
				this.IsRunning = false;
			}
			this.UdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			try
			{
				IPAddress ipaddress;
				if (listenAddress == "127.0.0.1" || listenAddress == "")
				{
					ipaddress = IPAddress.Loopback;
				}
				else if (listenAddress == "0.0.0.0")
				{
					ipaddress = IPAddress.Any;
				}
				else
				{
					IEnumerable<IPAddress> hostAddresses = Dns.GetHostAddresses(listenAddress);
					ipaddress = null;
					using (IEnumerator<IPAddress> enumerator = hostAddresses.Where((IPAddress ip) => ip.AddressFamily == AddressFamily.InterNetwork).GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							ipaddress = enumerator.Current;
						}
					}
					if (ipaddress == null)
					{
						throw new SocketException(10049);
					}
				}
				this.UdpSocket.Bind(new IPEndPoint(ipaddress, port));
			}
			catch (SocketException ex)
			{
				this.UdpSocket.Close();
				this.UdpSocket = null;
				throw ex;
			}
			byte[] array = new byte[4];
			new Random().NextBytes(array);
			this.ServerId = BitConverter.ToUInt32(array, 0);
			this.IsUdpServerHasException = false;
			this.IsRunning = true;
			this.StartReceive();
		}

		private int BeginPacket(byte[] packetBuf)
		{
			int num = 0;
			packetBuf[num++] = 68;
			packetBuf[num++] = 83;
			packetBuf[num++] = 85;
			packetBuf[num++] = 83;
			Array.Copy(BitConverter.GetBytes(1001), 0, packetBuf, num, 2);
			num += 2;
			Array.Copy(BitConverter.GetBytes((int)((ushort)packetBuf.Length - 16)), 0, packetBuf, num, 2);
			num += 2;
			Array.Clear(packetBuf, num, 4);
			num += 4;
			Array.Copy(BitConverter.GetBytes(this.ServerId), 0, packetBuf, num, 4);
			return num + 4;
		}

		private void FinishPacket(byte[] packetBuf)
		{
			Array.Clear(packetBuf, 8, 4);
			Array.Copy(BitConverter.GetBytes(Crc32Algorithm.Compute(packetBuf, 0, packetBuf.Length)), 0, packetBuf, 8, 4);
		}

		private void SendPacket(IPEndPoint clientEP, byte[] usefulData)
		{
			byte[] array = new byte[usefulData.Length + 16];
			int num = this.BeginPacket(array);
			Array.Copy(usefulData, 0, array, num, usefulData.Length);
			this.FinishPacket(array);
			this.PoolLock.EnterWriteLock();
			int num2 = this.listInd;
			int num3 = this.listInd + 1;
			this.listInd = num3;
			this.listInd = num3 % 80;
			SocketAsyncEventArgs socketAsyncEventArgs = this.ArgsList[num2];
			this.PoolLock.ExitWriteLock();
			this._pool.Wait();
			socketAsyncEventArgs.RemoteEndPoint = clientEP;
			Array.Copy(array, socketAsyncEventArgs.Buffer, array.Length);
			bool flag = false;
			try
			{
				flag = this.UdpSocket.SendToAsync(socketAsyncEventArgs);
				if (!flag)
				{
					this.CompletedSynchronousSocketEvent();
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				if (!flag)
				{
					this.CompletedSynchronousSocketEvent();
				}
			}
		}

		private void ProcessIncoming(byte[] localMsg, IPEndPoint clientEP)
		{
			try
			{
				int num = 0;
				if (localMsg[0] == 68 && localMsg[1] == 83 && localMsg[2] == 85 && localMsg[3] == 67)
				{
					num += 4;
					int num2 = (int)BitConverter.ToUInt16(localMsg, num);
					num += 2;
					if (num2 <= 1001)
					{
						uint num3 = (uint)BitConverter.ToUInt16(localMsg, num);
						num += 2;
						if (num3 >= 0U)
						{
							num3 += 16U;
							if ((ulong)num3 <= (ulong)((long)localMsg.Length))
							{
								if ((ulong)num3 < (ulong)((long)localMsg.Length))
								{
									byte[] array = new byte[num3];
									Array.Copy(localMsg, array, (long)((ulong)num3));
									localMsg = array;
								}
								uint num4 = BitConverter.ToUInt32(localMsg, num);
								localMsg[num++] = 0;
								localMsg[num++] = 0;
								localMsg[num++] = 0;
								localMsg[num++] = 0;
								uint num5 = Crc32Algorithm.Compute(localMsg);
								if (num4 == num5)
								{
									BitConverter.ToUInt32(localMsg, num);
									num += 4;
									uint num6 = BitConverter.ToUInt32(localMsg, num);
									num += 4;
									if (num6 == 1048576U)
									{
										byte[] array2 = new byte[8];
										int num7 = 0;
										Array.Copy(BitConverter.GetBytes(1048576U), 0, array2, num7, 4);
										num7 += 4;
										Array.Copy(BitConverter.GetBytes(1001), 0, array2, num7, 2);
										num7 += 2;
										array2[num7++] = 0;
										array2[num7++] = 0;
										this.SendPacket(clientEP, array2);
									}
									else if (num6 == 1048577U)
									{
										int num8 = BitConverter.ToInt32(localMsg, num);
										num += 4;
										if (num8 >= 0 && num8 <= 4)
										{
											int num9 = num;
											for (int i = 0; i < num8; i++)
											{
												if (localMsg[num9 + i] >= 4)
												{
													return;
												}
											}
											byte[] array3 = new byte[16];
											byte b = 0;
											while ((int)b < num8)
											{
												byte currRequest = localMsg[num9 + (int)b];
												PadMeta padMeta = this.PadList.Find((PadMeta x) => x.PadNum == currRequest);
												int num10 = 0;
												Array.Copy(BitConverter.GetBytes(1048577U), 0, array3, num10, 4);
												num10 += 4;
												array3[num10++] = currRequest;
												array3[num10++] = ((padMeta.ServiceProfileId == 0) ? 0 : 2);
												array3[num10++] = ((padMeta.ServiceProfileId == 0) ? 0 : 2);
												array3[num10++] = ((padMeta.ServiceProfileId == 0) ? 0 : 1);
												array3[num10++] = 0;
												array3[num10++] = 0;
												array3[num10++] = 0;
												array3[num10++] = 0;
												array3[num10++] = 0;
												array3[num10++] = ((padMeta.ServiceProfileId == 0) ? 0 : (currRequest + 1));
												array3[num10++] = ((padMeta.ServiceProfileId == 0) ? 0 : 239);
												array3[num10++] = 0;
												this.SendPacket(clientEP, array3);
												b += 1;
											}
										}
									}
									else if (num6 == 1048578U)
									{
										byte b2 = localMsg[num++];
										byte b3 = localMsg[num++];
										byte[] array4 = new byte[6];
										Array.Copy(localMsg, num, array4, 0, array4.Length);
										num += array4.Length;
										PhysicalAddress physicalAddress = new PhysicalAddress(array4);
										Dictionary<IPEndPoint, UdpClientRequestTimes> clients = this.Clients;
										lock (clients)
										{
											if (this.Clients.ContainsKey(clientEP))
											{
												this.Clients[clientEP].RequestPadInfo(b2, b3, physicalAddress);
											}
											else
											{
												UdpClientRequestTimes udpClientRequestTimes = new UdpClientRequestTimes();
												udpClientRequestTimes.RequestPadInfo(b2, b3, physicalAddress);
												this.Clients[clientEP] = udpClientRequestTimes;
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

		private void ReceiveCallback(IAsyncResult iar)
		{
			byte[] array = null;
			EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
			try
			{
				int num = ((Socket)iar.AsyncState).EndReceiveFrom(iar, ref endPoint);
				array = new byte[num];
				Array.Copy(this.RequestBuffer, array, num);
			}
			catch (Exception)
			{
			}
			this.StartReceive();
			if (array != null)
			{
				this.ProcessIncoming(array, (IPEndPoint)endPoint);
			}
		}

		private void StartReceive()
		{
			try
			{
				if (this.IsRunning)
				{
					EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
					this.UdpSocket.BeginReceiveFrom(this.RequestBuffer, 0, this.RequestBuffer.Length, SocketFlags.None, ref endPoint, new AsyncCallback(this.ReceiveCallback), this.UdpSocket);
				}
			}
			catch (SocketException)
			{
				uint num = 2147483648U;
				uint num2 = 402653184U;
				uint num3 = num | num2 | 12U;
				this.UdpSocket.IOControl((int)num3, new byte[] { Convert.ToByte(false) }, null);
				this.StartReceive();
			}
		}

		private void NewReportIncoming()
		{
			for (int i = 0; i < this.PadList.Count; i++)
			{
				PadMeta padMeta = this.PadList[i];
				if (!this.IsRunning)
				{
					return;
				}
				byte[] array = new byte[100];
				int num = this.BeginPacket(array);
				Array.Copy(BitConverter.GetBytes(1048578U), 0, array, num, 4);
				num += 4;
				array[num++] = padMeta.PadNum;
				array[num++] = 2;
				array[num++] = 2;
				array[num++] = 1;
				array[num++] = 0;
				array[num++] = 0;
				array[num++] = 0;
				array[num++] = 0;
				array[num++] = 0;
				array[num++] = padMeta.PadNum;
				array[num++] = 239;
				array[num++] = 1;
				uint packetCounter = padMeta.PacketCounter;
				padMeta.PacketCounter = packetCounter + 1U;
				Array.Copy(BitConverter.GetBytes(packetCounter), 0, array, num, 4);
				num += 4;
				if (GamepadStateConverter.ConvertToBytes(padMeta.InputReport, ref padMeta.totalMicroSec, num, ref array))
				{
					this.FinishPacket(array);
				}
				foreach (KeyValuePair<IPEndPoint, UdpClientRequestTimes> keyValuePair in this.Clients)
				{
					this.PoolLock.EnterWriteLock();
					int num2 = this.listInd;
					int num3 = this.listInd + 1;
					this.listInd = num3;
					this.listInd = num3 % 80;
					SocketAsyncEventArgs socketAsyncEventArgs = this.ArgsList[num2];
					this.PoolLock.ExitWriteLock();
					this._pool.Wait();
					socketAsyncEventArgs.RemoteEndPoint = keyValuePair.Key;
					Array.Copy(array, socketAsyncEventArgs.Buffer, array.Length);
					bool flag = false;
					try
					{
						flag = this.UdpSocket.SendToAsync(socketAsyncEventArgs);
					}
					catch (SocketException)
					{
					}
					finally
					{
						if (!flag)
						{
							this.CompletedSynchronousSocketEvent();
						}
					}
					this.PadList[i] = padMeta;
				}
			}
		}

		private const int MAX_PAD_SLOTS = 4;

		private const int ARG_BUFFER_LEN = 80;

		private const ushort MaxProtocolVersion = 1001;

		public bool IsUdpServerHasException;

		public bool IsRunning;

		public List<PadMeta> PadList = new List<PadMeta>();

		private SocketAsyncEventArgs[] ArgsList;

		private byte[] RequestBuffer = new byte[1024];

		private Socket UdpSocket;

		private uint ServerId;

		private int listInd;

		private SemaphoreSlim _pool;

		private ReaderWriterLockSlim PoolLock = new ReaderWriterLockSlim();

		private Dictionary<IPEndPoint, UdpClientRequestTimes> Clients = new Dictionary<IPEndPoint, UdpClientRequestTimes>();
	}
}
