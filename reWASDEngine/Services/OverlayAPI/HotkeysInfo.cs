using System;
using System.Collections.Generic;
using System.Windows;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using DTOverlay;
using XBEliteWPF.Infrastructure.KeyBindingsModel;

namespace reWASDEngine.Services.OverlayAPI
{
	public class HotkeysInfo : ZBindableBase
	{
		public List<ButtonsInMessageAllDevices> ButtonsGroups
		{
			get
			{
				return this._buttonsGroups;
			}
			set
			{
				this.SetProperty<List<ButtonsInMessageAllDevices>>(ref this._buttonsGroups, value, "ButtonsGroups");
			}
		}

		public void FillGroupsForID(string ID, OverlayUtils.HotkeysType hotkeysType)
		{
			this.ButtonsGroups = new List<ButtonsInMessageAllDevices>();
			this._isPresent = false;
			List<GroupFromSettings> gamepadBinding = OverlayUtils.GetGamepadBinding(ID, hotkeysType);
			int num = 0;
			foreach (GroupFromSettings groupFromSettings in gamepadBinding)
			{
				ButtonsInMessageAllDevices buttonsInMessageAllDevices = new ButtonsInMessageAllDevices();
				buttonsInMessageAllDevices.Buttons = new List<ButtonsInMessage>();
				buttonsInMessageAllDevices.CurrentGamepadType = groupFromSettings.CurrentGamepadType;
				bool flag = false;
				int num2 = 0;
				foreach (AssociatedControllerButton associatedControllerButton in groupFromSettings.groupButtons)
				{
					if (associatedControllerButton.GamepadButton != 2001 || associatedControllerButton.IsKeyScanCode)
					{
						flag = true;
						this._isPresent = true;
						ButtonsInMessage buttonsInMessage = new ButtonsInMessage();
						buttonsInMessage.Button = new MessageButtonInfo();
						buttonsInMessage.Button.PlusVisibility = num2++ > 0;
						buttonsInMessage.CurrentGamepadType = buttonsInMessageAllDevices.CurrentGamepadType;
						buttonsInMessage.Button.Button = associatedControllerButton;
						buttonsInMessageAllDevices.Buttons.Add(buttonsInMessage);
					}
				}
				if (flag)
				{
					buttonsInMessageAllDevices.Index = num++;
					this.ButtonsGroups.Add(buttonsInMessageAllDevices);
				}
			}
		}

		public bool IsPresent()
		{
			return this._isPresent;
		}

		public bool IsOnlyOneGroup()
		{
			return this.ButtonsGroups.Count <= 1;
		}

		public Thickness Margin
		{
			get
			{
				return new Thickness((double)(this.IsOnlyOneGroup() ? 0 : (-3)), 0.0, 0.0, -3.0);
			}
		}

		private bool _isPresent;

		private List<ButtonsInMessageAllDevices> _buttonsGroups;
	}
}
