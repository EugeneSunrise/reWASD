using System;
using DiscSoft.NET.Common.Utils;
using DTOverlay;
using reWASDCommon.Infrastructure.Enums;
using reWASDEngine;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Services.Interfaces;

namespace XBEliteWPF.Services
{
	public class OverlayManagerService : IOverlayManagerService
	{
		public OverlayManager OverlayManager
		{
			get
			{
				return this._overlayManager;
			}
		}

		public OverlayManagerService()
		{
			Tracer.TraceWrite("OverlayManagerService Constructor", false);
			this._overlayManager = new OverlayManager();
			Engine.EventAggregator.GetEvent<PreferencesChanged>().Subscribe(new Action<object>(this.OnPreferencesChanged));
			this.OnPreferencesChanged(null);
		}

		private void OnPreferencesChanged(object obj)
		{
			this._overlayManager.ApplySetting();
		}

		public void ReceivedOverlayMessage(OverlayMessageType messageType, IntPtr lParam)
		{
			this._overlayManager.ReceivedOverlayMessage(messageType, lParam);
		}

		public void ExecuteOverlayMenuCommand(RewasdOverlayMenuServiceCommand rewasdOverlayMenuServiceCommand)
		{
			this._overlayManager.ExecuteOverlayMenuCommand(rewasdOverlayMenuServiceCommand);
		}

		private OverlayManager _overlayManager;
	}
}
