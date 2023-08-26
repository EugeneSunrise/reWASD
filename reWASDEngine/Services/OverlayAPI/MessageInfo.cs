using System;
using System.Windows;
using System.Windows.Media;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using XBEliteWPF.Utils.XBUtil;

namespace reWASDEngine.Services.OverlayAPI
{
	public class MessageInfo : ZBindableBase
	{
		public MessageInfo()
		{
			this.IsOnline = true;
		}

		public bool IsOnline
		{
			get
			{
				return this._isOnline;
			}
			set
			{
				this._isOnline = value;
				this.OnPropertyChanged("IsOnline");
			}
		}

		public HotkeysInfo GamepadHotkeysInfo
		{
			get
			{
				return this._gamepadHotkeysInfo;
			}
			set
			{
				this.SetProperty<HotkeysInfo>(ref this._gamepadHotkeysInfo, value, "GamepadHotkeysInfo");
				this.OnPropertyChanged("IsHotkeyGamepad");
			}
		}

		public HotkeysInfo MappingsHotkeysInfo
		{
			get
			{
				return this._mappingsHotkeysInfo;
			}
			set
			{
				this.SetProperty<HotkeysInfo>(ref this._mappingsHotkeysInfo, value, "MappingsHotkeysInfo");
				this.OnPropertyChanged("IsHotkeyMappings");
			}
		}

		public HotkeysInfo MappingsHotkeysInfoDescriptions
		{
			get
			{
				return this._mappingsHotkeysInfoDescriptions;
			}
			set
			{
				this.SetProperty<HotkeysInfo>(ref this._mappingsHotkeysInfoDescriptions, value, "MappingsHotkeysInfoDescriptions");
				this.OnPropertyChanged("IsHotkeyMappingsDescriptions");
			}
		}

		public void FillEnd()
		{
			this.OnPropertyChanged("SmallTextMappingNewLine");
			this.OnPropertyChanged("SmallTextMappingDescriptionsNewLine");
			this.OnPropertyChanged("SmallTextGamepadNewLine");
		}

		public string DeviceName
		{
			get
			{
				return this._deviceName;
			}
			set
			{
				this.SetProperty<string>(ref this._deviceName, value, "DeviceName");
			}
		}

		public string MessageTextBold
		{
			get
			{
				return this._messageTextBold;
			}
			set
			{
				this.SetProperty<string>(ref this._messageTextBold, value, "MessageTextBold");
			}
		}

		public string MessageText
		{
			get
			{
				return this._messageText;
			}
			set
			{
				this.SetProperty<string>(ref this._messageText, value, "MessageText");
			}
		}

		public string SmallTextGamepad
		{
			get
			{
				return this._smallTextGamepad;
			}
			set
			{
				this.SetProperty<string>(ref this._smallTextGamepad, value, "SmallTextGamepad");
				this.OnPropertyChanged("IsHotkeyGamepad");
			}
		}

		public string TextFeature
		{
			get
			{
				return this._textFeature;
			}
			set
			{
				this.SetProperty<string>(ref this._textFeature, value, "TextFeature");
				this.OnPropertyChanged("IsNeedAnyFeature");
			}
		}

		public string SmallTextMappingDescriptions
		{
			get
			{
				return this._smallTextMappingDescriptions;
			}
			set
			{
				this.SetProperty<string>(ref this._smallTextMappingDescriptions, value, "SmallTextMappingDescriptions");
				this.OnPropertyChanged("IsHotkeyMappingsDescriptions");
			}
		}

		public string SmallTextMapping
		{
			get
			{
				return this._smallTextMapping;
			}
			set
			{
				this.SetProperty<string>(ref this._smallTextMapping, value, "SmallTextMapping");
				this.OnPropertyChanged("IsHotkeyMappings");
			}
		}

		public Drawing Drawing
		{
			get
			{
				return this._drawing;
			}
			set
			{
				this.SetProperty<Drawing>(ref this._drawing, value, "Drawing");
			}
		}

		public Drawing DrawingDisconnectedLine
		{
			get
			{
				return this._drawingDisconnectedLine;
			}
			set
			{
				this.SetProperty<Drawing>(ref this._drawingDisconnectedLine, value, "DrawingDisconnectedLine");
			}
		}

		public Drawing DrawingToggle
		{
			get
			{
				return this._drawingToggle;
			}
			set
			{
				this.SetProperty<Drawing>(ref this._drawingToggle, value, "DrawingToggle");
			}
		}

		public Drawing DrawingGamepadForBattery
		{
			get
			{
				return this._drawingGamepadForBattery;
			}
			set
			{
				this.SetProperty<Drawing>(ref this._drawingGamepadForBattery, value, "DrawingGamepadForBattery");
			}
		}

		public int ShiftIndex
		{
			get
			{
				return this._shiftIndex;
			}
			set
			{
				this.SetProperty<int>(ref this._shiftIndex, value, "ShiftIndex");
				this.OnPropertyChanged("ShiftModificatorBrush");
			}
		}

		public bool SmallTextMappingNewLine
		{
			get
			{
				HotkeysInfo mappingsHotkeysInfo = this.MappingsHotkeysInfo;
				return mappingsHotkeysInfo != null && mappingsHotkeysInfo.IsOnlyOneGroup();
			}
		}

		public bool SmallTextMappingDescriptionsNewLine
		{
			get
			{
				HotkeysInfo mappingsHotkeysInfo = this.MappingsHotkeysInfo;
				return mappingsHotkeysInfo != null && mappingsHotkeysInfo.IsOnlyOneGroup();
			}
		}

		public bool SmallTextGamepadNewLine
		{
			get
			{
				HotkeysInfo gamepadHotkeysInfo = this.GamepadHotkeysInfo;
				return gamepadHotkeysInfo != null && gamepadHotkeysInfo.IsOnlyOneGroup();
			}
		}

		public bool IsHotkeyGamepad
		{
			get
			{
				return this.GamepadHotkeysInfo != null && this.GamepadHotkeysInfo.IsPresent() && this.SmallTextGamepad != null && this.SmallTextGamepad.Length > 0 && this.IsVirtual && !this.ShortModeNewLine;
			}
		}

		public bool IsHotkeyMappings
		{
			get
			{
				return this.MappingsHotkeysInfo != null && this.MappingsHotkeysInfo.IsPresent() && this.SmallTextMapping != null && this.SmallTextMapping.Length > 0 && !this.ShortModeNewLine;
			}
		}

		public bool IsHotkeyMappingsDescriptions
		{
			get
			{
				return this.MappingsHotkeysInfoDescriptions != null && this.MappingsHotkeysInfoDescriptions.IsPresent() && this.SmallTextMappingDescriptions != null && this.SmallTextMappingDescriptions.Length > 0 && !this.ShortModeNewLine;
			}
		}

		public bool IsDeviceNameShow
		{
			get
			{
				return !this.ShortModeNewLine;
			}
		}

		public bool IsTextShow
		{
			get
			{
				return this.TextShow;
			}
		}

		public bool IsBatteryGamepadShow
		{
			get
			{
				return this.DrawingGamepadForBattery != null;
			}
		}

		public bool IsToggleShow
		{
			get
			{
				return this.DrawingToggle != null;
			}
		}

		public bool IsNeedAnyFeature
		{
			get
			{
				return !this.TextFeature.Equals("");
			}
		}

		public Brush ShiftModificatorBrush
		{
			get
			{
				if (!this.IsOnline)
				{
					return Application.Current.TryFindResource("CreamBrushPressed") as SolidColorBrush;
				}
				if (this.ShiftIndex != 0)
				{
					return XBUtils.GetBrushForShiftIndex(this.ShiftIndex);
				}
				return Application.Current.TryFindResource("CreamBrush") as SolidColorBrush;
			}
		}

		public GridLength SmallWidthLength
		{
			get
			{
				if (!this.ShortModeNewLine)
				{
					return new GridLength(80.0);
				}
				return new GridLength(0.0, GridUnitType.Auto);
			}
		}

		public GridLength BigWidthLength
		{
			get
			{
				if (!this.ShortModeNewLine)
				{
					return new GridLength(300.0);
				}
				return new GridLength(0.0, GridUnitType.Auto);
			}
		}

		public HorizontalAlignment AlignmentSettings
		{
			get
			{
				return this._alignment;
			}
			set
			{
				this._alignment = value;
				this.OnPropertyChanged("Alignment");
			}
		}

		public TextTrimming TrimmingConfigName
		{
			get
			{
				if (!this.ShortModeNewLine)
				{
					return TextTrimming.None;
				}
				return TextTrimming.CharacterEllipsis;
			}
		}

		public TextWrapping WrapConfigName
		{
			get
			{
				if (!this.ShortModeNewLine)
				{
					return TextWrapping.Wrap;
				}
				return TextWrapping.NoWrap;
			}
		}

		public DateTime timeStart;

		public string ID;

		private string _deviceName;

		private string _messageTextBold;

		private string _messageText;

		private string _smallTextMapping;

		private int _shiftIndex;

		private Drawing _drawing;

		public bool IsAlwaysShow;

		public bool IsVirtual;

		private bool _isOnline;

		private HotkeysInfo _gamepadHotkeysInfo;

		private HotkeysInfo _mappingsHotkeysInfo;

		private HotkeysInfo _mappingsHotkeysInfoDescriptions;

		private string _smallTextGamepad;

		private string _textFeature;

		private string _smallTextMappingDescriptions;

		public bool ShortModeNewLine;

		public bool TextShow;

		private Drawing _drawingDisconnectedLine;

		private Drawing _drawingToggle;

		private Drawing _drawingGamepadForBattery;

		private HorizontalAlignment _alignment;
	}
}
