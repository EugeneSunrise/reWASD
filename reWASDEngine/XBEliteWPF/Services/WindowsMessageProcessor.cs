using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoftReWASDGuiNamespace;
using DTOverlay;
using Microsoft.Win32.SafeHandles;
using Prism.Events;
using reWASDCommon.Utils;
using reWASDEngine;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils;

namespace XBEliteWPF.Services
{
	public class WindowsMessageProcessor : IWindowsMessageProcessor
	{
		[DllImport("User32.dll")]
		public static extern uint RegisterWindowMessage(string str);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool ChangeWindowMessageFilter(uint msg, WindowsMessageProcessor.ChangeWindowMessageFilterExAction action);

		public WindowsMessageProcessor(IGamepadService gs, IGameProfilesService gps, ILicensingService ls, IEventAggregator ea)
		{
			this._gamepadService = gs;
			this._gameProfilesService = gps;
			this._licensingService = ls;
			this._eventAggregator = ea;
			WindowsMessageProcessor._bluetoothAndSerialPortDeviceNotifications = new List<IntPtr>();
			if (!WindowsMessageProcessor.ChangeWindowMessageFilter(this.OverlayMessageType, WindowsMessageProcessor.ChangeWindowMessageFilterExAction.Allow))
			{
				Tracer.TraceWrite("Overlay WindowsMessageProcessor: Changing window message filter failed", false);
				return;
			}
			Tracer.TraceWrite("Overlay WindowsMessageProcessor: Changing window message filter ok", false);
		}

		protected override void Finalize()
		{
			try
			{
				foreach (IntPtr intPtr in WindowsMessageProcessor._bluetoothAndSerialPortDeviceNotifications)
				{
					if (intPtr != IntPtr.Zero)
					{
						UtilsNative.UnregisterDeviceNotification(intPtr);
					}
				}
				if (WindowsMessageProcessor._hidgamemapDeviceNotification != IntPtr.Zero)
				{
					UtilsNative.UnregisterDeviceNotification(WindowsMessageProcessor._hidgamemapDeviceNotification);
				}
				if (UtilsNative.IsHandleValid(WindowsMessageProcessor._driverHandle))
				{
					WindowsMessageProcessor._driverHandle.Close();
					WindowsMessageProcessor._driverHandle = null;
				}
			}
			finally
			{
				base.Finalize();
			}
		}

		public void Attach(Window window, bool attachDriverEvents = true)
		{
			HwndSource hwndSource = HwndSource.FromHwnd(new WindowInteropHelper(window).Handle);
			if (hwndSource != null)
			{
				hwndSource.AddHook(new HwndSourceHook(this.WndProc));
				if (attachDriverEvents)
				{
					WindowsMessageProcessor._driverHandle = UtilsNative.OpenHidgamemapDriver();
					if (UtilsNative.IsHandleValid(WindowsMessageProcessor._driverHandle))
					{
						WindowsMessageProcessor._hidgamemapDeviceNotification = UtilsNative.RegisterDeviceNotification(hwndSource.Handle, WindowsMessageProcessor._driverHandle);
					}
					WindowsMessageProcessor._bluetoothAndSerialPortDeviceNotifications.Add(UtilsNative.RegisterDeviceNotification(hwndSource.Handle, WindowsMessageProcessor.GUID_BTHPORT_DEVICE_INTERFACE));
					WindowsMessageProcessor._bluetoothAndSerialPortDeviceNotifications.Add(UtilsNative.RegisterDeviceNotification(hwndSource.Handle, WindowsMessageProcessor.GUID_BTH_DEVICE_INTERFACE));
					WindowsMessageProcessor._bluetoothAndSerialPortDeviceNotifications.Add(UtilsNative.RegisterDeviceNotification(hwndSource.Handle, WindowsMessageProcessor.GUID_BLUETOOTHLE_DEVICE_INTERFACE));
					WindowsMessageProcessor._bluetoothAndSerialPortDeviceNotifications.Add(UtilsNative.RegisterDeviceNotification(hwndSource.Handle, WindowsMessageProcessor.GUID_DEVINTERFACE_COMPORT));
				}
			}
			this._attachedWindow = window;
		}

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if ((long)msg == (long)((ulong)this.CloseEngineMessage))
			{
				Tracer.TraceWrite("Close Engine message", false);
				new LedOperationsDecider().ExitHelper();
				Process.GetCurrentProcess().Kill();
			}
			if (msg == 537)
			{
				if (wParam.ToInt32() == 32774)
				{
					UtilsNative.Win32.DEV_BROADCAST_HDR dev_BROADCAST_HDR = (UtilsNative.Win32.DEV_BROADCAST_HDR)Marshal.PtrToStructure(lParam, typeof(UtilsNative.Win32.DEV_BROADCAST_HDR));
					if (dev_BROADCAST_HDR.dbch_devicetype == 6 && dev_BROADCAST_HDR.dbch_size >= Marshal.SizeOf(typeof(UtilsNative.Win32.DEV_BROADCAST_HANDLE)))
					{
						UtilsNative.Win32.DEV_BROADCAST_HANDLE dev_BROADCAST_HANDLE = (UtilsNative.Win32.DEV_BROADCAST_HANDLE)Marshal.PtrToStructure(lParam, typeof(UtilsNative.Win32.DEV_BROADCAST_HANDLE));
						IntPtr intPtr = new IntPtr(dev_BROADCAST_HANDLE.dbch_data2);
						IntPtr intPtr2 = new IntPtr(dev_BROADCAST_HANDLE.dbch_data);
						if (dev_BROADCAST_HANDLE.dbch_eventguid.Equals(DiscSoftReWASDGui.GUID_REWASD_SERVICE_CONTROLLERS_CHANGED))
						{
							this.ProcessEvent("DiscSoftReWASD_msg_1", intPtr2, intPtr);
						}
						if (dev_BROADCAST_HANDLE.dbch_eventguid.Equals(DiscSoftReWASDGui.GUID_REWASD_SERVICE_PROFILES_CHANGED))
						{
							this.ProcessEvent("DiscSoftReWASD_msg_3", intPtr2, intPtr);
						}
						if (dev_BROADCAST_HANDLE.dbch_eventguid.Equals(DiscSoftReWASDGui.GUID_REWASD_SERVICE_PROFILE_STATE_CHANGED))
						{
							this.ProcessEvent("DiscSoftReWASD_msg_17", intPtr2, intPtr);
						}
						if (dev_BROADCAST_HANDLE.dbch_eventguid.Equals(DiscSoftReWASDGui.GUID_REWASD_SERVICE_PHYS_SLOT_1_SELECTED))
						{
							this.ProcessEvent("DiscSoftReWASD_msg_phys_slot_1", intPtr2, intPtr);
						}
						if (dev_BROADCAST_HANDLE.dbch_eventguid.Equals(DiscSoftReWASDGui.GUID_REWASD_SERVICE_PHYS_SLOT_2_SELECTED))
						{
							this.ProcessEvent("DiscSoftReWASD_msg_phys_slot_2", intPtr2, intPtr);
						}
						if (dev_BROADCAST_HANDLE.dbch_eventguid.Equals(DiscSoftReWASDGui.GUID_REWASD_SERVICE_PHYS_SLOT_3_SELECTED))
						{
							this.ProcessEvent("Disc SoftReWASD_msg_phys_slot_3", intPtr2, intPtr);
						}
						if (dev_BROADCAST_HANDLE.dbch_eventguid.Equals(DiscSoftReWASDGui.GUID_REWASD_SERVICE_PHYS_SLOT_4_SELECTED))
						{
							this.ProcessEvent("DiscSoftReWASD_msg_phys_slot_4", intPtr2, intPtr);
						}
						if (dev_BROADCAST_HANDLE.dbch_eventguid.Equals(DiscSoftReWASDGui.GUID_REWASD_SERVICE_MACRO_BROADCAST))
						{
							this.ProcessEvent("DiscSoftReWASD_msg_macro_command", intPtr2, intPtr);
						}
						if (dev_BROADCAST_HANDLE.dbch_eventguid.Equals(DiscSoftReWASDGui.GUID_REWASD_SERVICE_GYRO_CALIBRATION_FINISHED))
						{
							this.ProcessEvent("DiscSoftReWASD_msg_18", intPtr2, intPtr);
						}
						if (dev_BROADCAST_HANDLE.dbch_eventguid.Equals(DiscSoftReWASDGui.GUID_REWASD_SERVICE_AMIIBO_UNLOAD_BY_UID))
						{
							this.ProcessEvent("DiscSoftReWASD_msg_amiibo_unload_uid", intPtr2, intPtr);
						}
						if (dev_BROADCAST_HANDLE.dbch_eventguid.Equals(DiscSoftReWASDGui.GUID_REWASD_SERVICE_HONEYPOT_PAIRING_REJECTED))
						{
							this.ProcessEvent("DiscSoftReWASD_msg_honeypot_pairing_rejected", intPtr2, intPtr);
						}
					}
				}
				if (wParam.ToInt32() == 32768 || wParam.ToInt32() == 32772)
				{
					UtilsNative.Win32.DEV_BROADCAST_HDR dev_BROADCAST_HDR2 = (UtilsNative.Win32.DEV_BROADCAST_HDR)Marshal.PtrToStructure(lParam, typeof(UtilsNative.Win32.DEV_BROADCAST_HDR));
					if (dev_BROADCAST_HDR2.dbch_devicetype == 5 && dev_BROADCAST_HDR2.dbch_size >= Marshal.SizeOf(typeof(UtilsNative.Win32.DEV_BROADCAST_DEVICEINTERFACE)))
					{
						UtilsNative.Win32.DEV_BROADCAST_DEVICEINTERFACE dev_BROADCAST_DEVICEINTERFACE = (UtilsNative.Win32.DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(lParam, typeof(UtilsNative.Win32.DEV_BROADCAST_DEVICEINTERFACE));
						if (dev_BROADCAST_DEVICEINTERFACE.dbcc_classguid == WindowsMessageProcessor.GUID_DEVINTERFACE_COMPORT)
						{
							this._gamepadService.ExternalDeviceRelationsHelper.RefreshComPorts();
						}
						if (dev_BROADCAST_DEVICEINTERFACE.dbcc_classguid == WindowsMessageProcessor.GUID_BTHPORT_DEVICE_INTERFACE || dev_BROADCAST_DEVICEINTERFACE.dbcc_classguid == WindowsMessageProcessor.GUID_BTH_DEVICE_INTERFACE || dev_BROADCAST_DEVICEINTERFACE.dbcc_classguid == WindowsMessageProcessor.GUID_BLUETOOTHLE_DEVICE_INTERFACE || dev_BROADCAST_DEVICEINTERFACE.dbcc_classguid == WindowsMessageProcessor.GUID_DEVINTERFACE_COMPORT)
						{
							this._gamepadService.ExternalDeviceRelationsHelper.Refresh();
						}
					}
				}
			}
			if ((long)msg == (long)((ulong)this.InitializedPeripheralDevicesChanged))
			{
				WindowMessageEvent windowMessageEvent = new WindowMessageEvent("DiscSoftReWASD_msg_15", wParam, lParam);
				this._eventAggregator.GetEvent<InitializedPeripheralDevicesChanged>().Publish(windowMessageEvent);
			}
			if ((long)msg == (long)((ulong)this.CompositeDevicesChanged))
			{
				Tracer.TraceWrite("PreferencesChanged: clear CachedProfilesCollection", false);
				this._gamepadService.CachedProfilesCollection.Clear();
				WindowMessageEvent windowMessageEvent2 = new WindowMessageEvent("DiscSoftReWASD_msg_12", wParam, lParam);
				this._eventAggregator.GetEvent<CompositeDevicesChanged>().Publish(windowMessageEvent2);
			}
			if ((long)msg == (long)((ulong)this.OverlayMessageType))
			{
				OverlayMessageType overlayMessageType = (OverlayMessageType)wParam.ToInt32();
				Engine.OverlayManagerService.ReceivedOverlayMessage(overlayMessageType, lParam);
			}
			return IntPtr.Zero;
		}

		private void ProcessEvent(string msg, IntPtr wParam, IntPtr lParam)
		{
			WindowMessageEvent windowMessageEvent = new WindowMessageEvent(msg, wParam, lParam);
			if (msg == "DiscSoftReWASD_msg_9")
			{
				Tracer.TraceWrite("Before processing wnd message REWASD_MESSAGE_REMAP_ON", false);
				this._eventAggregator.GetEvent<RemapOn>().Publish(new ulong?(DSUtils.GetUlongFromWMPayload(windowMessageEvent)));
			}
			if (msg == "DiscSoftReWASD_msg_16")
			{
				Tracer.TraceWrite("Before processing wnd message REWASD_MESSAGE_REMAP_OFF", false);
				this._eventAggregator.GetEvent<RemapOff>().Publish(new ulong?(DSUtils.GetUlongFromWMPayload(windowMessageEvent)));
			}
			if (msg == "DiscSoftReWASD_msg_1")
			{
				Tracer.TraceWrite("Before processing wnd message REWASD_MESSAGE_CONTROLLER_CHANGED", false);
				this._eventAggregator.GetEvent<GamepadChanged>().Publish(windowMessageEvent);
			}
			if (msg == "DiscSoftReWASD_msg_3")
			{
				Tracer.TraceWrite("Before processing wnd message REWASD_MESSAGE_SERVICE_PROFILES_CHANGED", false);
				this._eventAggregator.GetEvent<ServiceProfilesChanged>().Publish(windowMessageEvent);
			}
			if (msg == "DiscSoftReWASD_msg_17")
			{
				Tracer.TraceWrite("Before processing wnd message REWASD_MESSAGE_PROFILE_STATE_CHANGED", false);
				this._eventAggregator.GetEvent<ServiceProfileStateChanged>().Publish(windowMessageEvent);
			}
			if (msg == "DiscSoftReWASD_msg_phys_slot_1")
			{
				Tracer.TraceWrite("Before processing wnd message REWASD_MESSAGE_PHYS_SLOT_1_SELECTED", false);
				this._eventAggregator.GetEvent<PhysSlot1Selected>().Publish(windowMessageEvent);
			}
			if (msg == "DiscSoftReWASD_msg_phys_slot_2")
			{
				Tracer.TraceWrite("Before processing wnd message REWASD_MESSAGE_PHYS_SLOT_2_SELECTED", false);
				this._eventAggregator.GetEvent<PhysSlot2Selected>().Publish(windowMessageEvent);
			}
			if (msg == "Disc SoftReWASD_msg_phys_slot_3")
			{
				Tracer.TraceWrite("Before processing wnd message REWASD_MESSAGE_PHYS_SLOT_3_SELECTED", false);
				this._eventAggregator.GetEvent<PhysSlot3Selected>().Publish(windowMessageEvent);
			}
			if (msg == "DiscSoftReWASD_msg_phys_slot_4")
			{
				Tracer.TraceWrite("Before processing wnd message REWASD_MESSAGE_PHYS_SLOT_4_SELECTED", false);
				this._eventAggregator.GetEvent<PhysSlot4Selected>().Publish(windowMessageEvent);
			}
			if (msg == "DiscSoftReWASD_msg_macro_command")
			{
				Tracer.TraceWrite("Before processing wnd message REWASD_MESSAGE_MACRO_COMMAND", false);
				this._eventAggregator.GetEvent<MacroCommandBroadcast>().Publish(windowMessageEvent);
			}
			if (msg == "DiscSoftReWASD_msg_18")
			{
				Tracer.TraceWrite("Before processing wnd message REWASD_MESSAGE_GYRO_CALIBRATION_FINISHED", false);
				this._eventAggregator.GetEvent<GyroCalibrationFinished>().Publish(windowMessageEvent);
			}
			if (msg == "DiscSoftReWASD_msg_amiibo_unload_uid")
			{
				Tracer.TraceWrite("Before processing wnd message REWASD_MESSAGE_AMIIBO_UNLOAD_BY_UID", false);
				this._eventAggregator.GetEvent<AmiiboUnloadByUidBroadcast>().Publish(windowMessageEvent);
			}
			if (msg == "DiscSoftReWASD_msg_honeypot_pairing_rejected")
			{
				Tracer.TraceWrite("Before processing wnd message REWASD_MESSAGE_HONEYPOT_PAIRING_REJECTED", false);
				this._eventAggregator.GetEvent<HoneypotPairingRejectedBroadcast>().Publish(windowMessageEvent);
			}
		}

		private uint CloseEngineMessage = WindowsMessageProcessor.RegisterWindowMessage("{A8D537A0-84DC-4F31-AA95-0B4DA69D0508}");

		private uint OverlayMessageType = WindowsMessageProcessor.RegisterWindowMessage("{EE93EBD2-FA87-40DC-BC10-51A468EC0375}");

		private uint CompositeDevicesChanged = WindowsMessageProcessor.RegisterWindowMessage("DiscSoftReWASD_msg_12");

		private uint InitializedPeripheralDevicesChanged = WindowsMessageProcessor.RegisterWindowMessage("DiscSoftReWASD_msg_15");

		private uint RemapOn = WindowsMessageProcessor.RegisterWindowMessage("DiscSoftReWASD_msg_9");

		private uint RemapOff = WindowsMessageProcessor.RegisterWindowMessage("DiscSoftReWASD_msg_16");

		private const int WmDeviceChange = 537;

		private const int DBT_CUSTOMEVENT = 32774;

		private const int DBT_DEVICEARRIVAL = 32768;

		private const int DBT_DEVICEREMOVECOMPLETE = 32772;

		private const int DBT_DEVTYP_HANDLE = 6;

		private static SafeFileHandle _driverHandle;

		private static IntPtr _hidgamemapDeviceNotification;

		private static List<IntPtr> _bluetoothAndSerialPortDeviceNotifications;

		private Window _attachedWindow;

		private IGamepadService _gamepadService;

		private IGameProfilesService _gameProfilesService;

		private ILicensingService _licensingService;

		private IEventAggregator _eventAggregator;

		private static Guid GUID_BTHPORT_DEVICE_INTERFACE = new Guid(139472938U, 45892, 20442, 155, 233, 144, 87, 107, 141, 70, 240);

		private static Guid GUID_BTH_DEVICE_INTERFACE = new Guid(15993189U, 59549, 17543, 152, 144, 135, 195, 171, 178, 17, 244);

		private static Guid GUID_BLUETOOTHLE_DEVICE_INTERFACE = new Guid(2015030808, 30515, 19684, 173, 208, 145, 244, 28, 103, 181, 146);

		private static Guid GUID_DEVINTERFACE_COMPORT = new Guid(2262880736U, 32905, 4560, 156, 228, 8, 0, 62, 48, 31, 115);

		public enum ChangeWindowMessageFilterExAction : uint
		{
			Reset,
			Allow,
			DisAllow
		}
	}
}
