using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using DTOverlay;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDEngine.Services.OverlayAPI
{
	public class MessagesVM : ZBindableBase
	{
		public double Scale
		{
			get
			{
				return this._scale;
			}
			set
			{
				this.SetProperty<double>(ref this._scale, value, "Scale");
			}
		}

		public void SetScale(double scale)
		{
			this.Scale = scale;
		}

		public float Transparent
		{
			get
			{
				return this._transparent;
			}
			set
			{
				this.SetProperty<float>(ref this._transparent, value, "Transparent");
			}
		}

		public void SetTransparent(float transparent)
		{
			this.Transparent = transparent;
		}

		public ObservableCollection<MessageInfo> Messages
		{
			get
			{
				ObservableCollection<MessageInfo> observableCollection;
				if ((observableCollection = this._messages) == null)
				{
					observableCollection = (this._messages = new ObservableCollection<MessageInfo>());
				}
				return observableCollection;
			}
		}

		public void ShowDebugMessage(string Caption, string text)
		{
			this.AddReplace(new MessageInfo
			{
				IsAlwaysShow = false,
				TextFeature = "",
				DrawingToggle = null,
				DrawingDisconnectedLine = null,
				DrawingGamepadForBattery = null,
				ShortModeNewLine = false,
				AlignmentSettings = HorizontalAlignment.Center,
				TextShow = true,
				ID = "DEBUG_ID",
				timeStart = DateTime.Now,
				DeviceName = Caption,
				MessageText = text,
				MessageTextBold = "",
				Drawing = ((Drawing)Application.Current.TryFindResource("UriBtnUnmapped")).Clone()
			});
		}

		public void ShowRemapChange(string ID, string deviceName, bool replace, bool isShortMode, HorizontalAlignment align)
		{
			if (replace && this.Messages.Count > 0)
			{
				this.Messages.RemoveAt(0);
			}
			this.AddReplace(new MessageInfo
			{
				IsAlwaysShow = false,
				TextFeature = "",
				DrawingToggle = null,
				DrawingDisconnectedLine = null,
				DrawingGamepadForBattery = null,
				ShortModeNewLine = isShortMode,
				AlignmentSettings = align,
				TextShow = !isShortMode,
				ID = ID,
				timeStart = DateTime.Now,
				DeviceName = deviceName,
				MessageText = DTLocalization.GetString(12118),
				MessageTextBold = "",
				Drawing = ((Drawing)Application.Current.TryFindResource("UriBtnUnmapped")).Clone()
			});
		}

		public void ShowDisconnected(string ID, string deviceName, bool replace, bool isShortMode, HorizontalAlignment align, BaseControllerVM controller)
		{
			if (replace && this.Messages.Count > 0)
			{
				this.Messages.RemoveAt(0);
			}
			this.AddReplace(new MessageInfo
			{
				IsAlwaysShow = false,
				TextFeature = "",
				DrawingToggle = null,
				DrawingDisconnectedLine = null,
				DrawingGamepadForBattery = null,
				AlignmentSettings = align,
				ShortModeNewLine = isShortMode,
				TextShow = !isShortMode,
				ID = ID,
				timeStart = DateTime.Now,
				DeviceName = ((deviceName != null) ? deviceName : ""),
				MessageText = DTLocalization.GetString(12166),
				MessageTextBold = "",
				Drawing = controller.MiniGamepadSVGIco,
				DrawingDisconnectedLine = ((Drawing)Application.Current.TryFindResource("MiniGamepadDisconnect")).Clone()
			});
		}

		public void ShowSlot(string ID, string deviceName, string gameConfig, int slot, bool empty, bool replace, bool showGamepadSettings, bool showMappings, bool isVirtual, bool isShortMode, HorizontalAlignment align, bool isNeedAnyFeature, bool IsOnline)
		{
			if (replace && this.Messages.Count > 0)
			{
				this.Messages.RemoveAt(0);
			}
			MessageInfo messageInfo = new MessageInfo();
			messageInfo.IsOnline = IsOnline;
			messageInfo.IsAlwaysShow = false;
			messageInfo.DrawingToggle = null;
			messageInfo.DrawingDisconnectedLine = null;
			messageInfo.DrawingGamepadForBattery = null;
			messageInfo.AlignmentSettings = align;
			messageInfo.ShortModeNewLine = isShortMode;
			messageInfo.TextShow = true;
			messageInfo.ID = ID;
			messageInfo.IsVirtual = isVirtual;
			messageInfo.timeStart = DateTime.Now;
			messageInfo.DeviceName = (IsOnline ? deviceName : (deviceName + " (" + DTLocalization.GetString(12166) + ")"));
			if (empty)
			{
				messageInfo.MessageText = string.Format(DTLocalization.GetString(isShortMode ? 12189 : 12119), slot);
				messageInfo.MessageTextBold = "";
				messageInfo.Drawing = ((Drawing)Application.Current.TryFindResource("SlotEmpty")).Clone();
				messageInfo.SmallTextMapping = "";
				messageInfo.SmallTextMappingDescriptions = "";
				messageInfo.SmallTextGamepad = "";
			}
			else
			{
				string text = DTLocalization.GetString(12126);
				if (isShortMode)
				{
					text = new Regex(" ").Replace(text, "\n", 1);
				}
				messageInfo.MessageText = string.Format(text, gameConfig, slot);
				messageInfo.Drawing = ((Drawing)Application.Current.TryFindResource("SlotIncluded")).Clone();
				messageInfo.SmallTextMapping = DTLocalization.GetString(12159);
				messageInfo.SmallTextMappingDescriptions = DTLocalization.GetString(12198);
				messageInfo.SmallTextGamepad = DTLocalization.GetString(12160);
			}
			if (isNeedAnyFeature && !isShortMode)
			{
				messageInfo.SmallTextMapping = "";
				messageInfo.SmallTextMappingDescriptions = "";
				messageInfo.SmallTextGamepad = "";
				messageInfo.GamepadHotkeysInfo = null;
				messageInfo.MappingsHotkeysInfo = null;
				messageInfo.MappingsHotkeysInfoDescriptions = null;
				messageInfo.TextFeature = DTLocalization.GetString(12191);
			}
			else
			{
				messageInfo.TextFeature = "";
				if (showGamepadSettings)
				{
					messageInfo.GamepadHotkeysInfo = new HotkeysInfo();
					messageInfo.GamepadHotkeysInfo.FillGroupsForID(ID, OverlayUtils.HotkeysType.Gamepad);
				}
				else
				{
					messageInfo.GamepadHotkeysInfo = null;
				}
				if (showMappings)
				{
					messageInfo.MappingsHotkeysInfo = new HotkeysInfo();
					messageInfo.MappingsHotkeysInfo.FillGroupsForID(ID, OverlayUtils.HotkeysType.Mappings);
					messageInfo.MappingsHotkeysInfoDescriptions = new HotkeysInfo();
					messageInfo.MappingsHotkeysInfoDescriptions.FillGroupsForID(ID, OverlayUtils.HotkeysType.MappingsDescriptions);
				}
				else
				{
					messageInfo.MappingsHotkeysInfo = null;
					messageInfo.MappingsHotkeysInfoDescriptions = null;
				}
			}
			messageInfo.FillEnd();
			this.AddReplace(messageInfo);
		}

		public void HideShift(string ID)
		{
			ID += "Shift";
			for (int i = 0; i < this.Messages.Count; i++)
			{
				if (this.Messages[i].ID != null && ID == this.Messages[i].ID)
				{
					if (this.Messages.Count > i)
					{
						this.Messages.RemoveAt(i);
					}
					return;
				}
			}
		}

		public void ShowShift(string ID, string deviceName, ShiftInfo shift, bool replace, bool isShortMode, HorizontalAlignment align, bool toggle, bool isAlwaysShow)
		{
			if (replace && this.Messages.Count > 0)
			{
				this.Messages.RemoveAt(0);
			}
			MessageInfo messageInfo = new MessageInfo();
			messageInfo.IsAlwaysShow = isAlwaysShow;
			messageInfo.TextFeature = "";
			messageInfo.DrawingToggle = null;
			messageInfo.DrawingDisconnectedLine = null;
			messageInfo.DrawingGamepadForBattery = null;
			messageInfo.AlignmentSettings = align;
			messageInfo.ShortModeNewLine = isShortMode;
			messageInfo.TextShow = !isShortMode;
			messageInfo.timeStart = DateTime.Now;
			messageInfo.DeviceName = deviceName;
			messageInfo.ShiftIndex = shift.Shift;
			messageInfo.MessageTextBold = "";
			messageInfo.ID = ID + "Shift";
			string text = string.Format("Shift{0}White", shift.Shift);
			messageInfo.Drawing = ((Drawing)Application.Current.TryFindResource(text)).Clone();
			if (toggle)
			{
				messageInfo.DrawingToggle = ((Drawing)Application.Current.TryFindResource("Toggle")).Clone();
			}
			string text2 = shift.Name;
			if (text2 == null || text2.Length == 0)
			{
				if (shift.Shift == 0)
				{
					text2 = DTLocalization.GetString(11195);
				}
				else
				{
					text2 = string.Format(DTLocalization.GetString(12474), shift.Shift);
				}
			}
			if (shift.Shift != 0)
			{
				messageInfo.MessageText = string.Format(DTLocalization.GetString(12120), text2);
			}
			else
			{
				messageInfo.MessageText = string.Format(DTLocalization.GetString(12121), text2);
			}
			this.AddReplace(messageInfo);
		}

		public void ShowBatteryLevel(string deviceName, bool low, bool replace, bool isShortMode, HorizontalAlignment align, ControllerTypeEnum firstControllerType, bool isBatteryLevelPercentPresent, byte batteryPercent)
		{
			MessageInfo messageInfo = new MessageInfo();
			messageInfo.IsAlwaysShow = false;
			messageInfo.TextFeature = "";
			messageInfo.DrawingToggle = null;
			messageInfo.DrawingDisconnectedLine = null;
			messageInfo.DrawingGamepadForBattery = null;
			messageInfo.AlignmentSettings = align;
			messageInfo.ShortModeNewLine = isShortMode;
			messageInfo.TextShow = !isShortMode;
			messageInfo.timeStart = DateTime.Now;
			messageInfo.DeviceName = deviceName;
			messageInfo.MessageTextBold = "";
			if (isShortMode)
			{
				messageInfo.DrawingGamepadForBattery = XBUtils.GetDrawingForControllerTypeEnum(firstControllerType, false);
			}
			if (low)
			{
				messageInfo.Drawing = ((Drawing)Application.Current.TryFindResource("battery_overlay_low")).Clone();
				messageInfo.MessageText = DTLocalization.GetString(12122).Replace("{0}", isBatteryLevelPercentPresent ? (" (" + batteryPercent.ToString() + "%)") : "");
			}
			else
			{
				messageInfo.Drawing = ((Drawing)Application.Current.TryFindResource("battery_overlay_critical")).Clone();
				messageInfo.MessageText = DTLocalization.GetString(12123).Replace("{0}", isBatteryLevelPercentPresent ? (" (" + batteryPercent.ToString() + "%)") : "");
			}
			foreach (MessageInfo messageInfo2 in this.Messages)
			{
				if (messageInfo2.DeviceName == messageInfo.DeviceName && messageInfo2.MessageText == messageInfo.MessageText)
				{
					return;
				}
			}
			if (replace && this.Messages.Count > 0)
			{
				this.Messages.RemoveAt(0);
			}
			this.Messages.Add(messageInfo);
		}

		public void RemoveOld(int timeMessageSec)
		{
			for (;;)
			{
				IL_00:
				foreach (MessageInfo messageInfo in this.Messages)
				{
					if (!messageInfo.IsAlwaysShow && (DateTime.Now - messageInfo.timeStart).Seconds > timeMessageSec)
					{
						this.Messages.Remove(messageInfo);
						goto IL_00;
					}
				}
				break;
			}
		}

		public void RemoveAll()
		{
			this.Messages.Clear();
		}

		public int Count()
		{
			return this.Messages.Count<MessageInfo>();
		}

		private void AddReplace(MessageInfo newMessage)
		{
			for (int i = 0; i < this.Messages.Count; i++)
			{
				if (newMessage.ID != null && this.Messages[i].ID != null && newMessage.ID == this.Messages[i].ID)
				{
					this.Messages[i] = newMessage;
					return;
				}
			}
			this.Messages.Add(newMessage);
		}

		public void ShowError(string Caption, string txt, HorizontalAlignment align, bool replace = true, bool isShortMode = false)
		{
			MessageInfo messageInfo = new MessageInfo();
			messageInfo.IsAlwaysShow = false;
			messageInfo.TextFeature = "";
			messageInfo.DrawingToggle = null;
			messageInfo.DrawingDisconnectedLine = null;
			messageInfo.DrawingGamepadForBattery = null;
			messageInfo.AlignmentSettings = align;
			messageInfo.ShortModeNewLine = isShortMode;
			messageInfo.TextShow = !isShortMode;
			messageInfo.timeStart = DateTime.Now;
			messageInfo.DeviceName = Caption;
			messageInfo.MessageTextBold = "";
			messageInfo.MessageText = txt;
			messageInfo.ShiftIndex = 1;
			if (replace && this.Messages.Count > 0)
			{
				this.Messages.RemoveAt(0);
			}
			this.Messages.Add(messageInfo);
		}

		private double _scale;

		private float _transparent;

		private ObservableCollection<MessageInfo> _messages;
	}
}
