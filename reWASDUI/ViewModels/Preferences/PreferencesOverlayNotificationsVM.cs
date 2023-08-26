using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.View.Controls.MultiRangeSlider;
using DTOverlay;
using reWASDUI.ViewModels.Preferences.Base;

namespace reWASDUI.ViewModels.Preferences
{
	public class PreferencesOverlayNotificationsVM : PreferencesBaseVM
	{
		public PreferencesOverlayNotificationsVM(PreferencesOverlayVM parent)
		{
			this.ParentVM = parent;
		}

		public override Task Initialize()
		{
			string @string = RegistryHelper.GetString("Overlay", "MonitorMessages", "", false);
			this.SelectedMonitor = this.ParentVM.GetCheckedMonitorName(this.SelectedMonitor, @string);
			this.ShowBatteryIsLow = Convert.ToBoolean(RegistryHelper.GetValue("Overlay", "ShowBatteryIsLow", 0, false));
			this.ShowBatteryIsCritical = Convert.ToBoolean(RegistryHelper.GetValue("Overlay", "ShowBatteryIsCritical", 1, false));
			this.ShowRemapIsONOFF = Convert.ToBoolean(RegistryHelper.GetValue("Overlay", "ShowRemapIsONOFF", 1, false));
			this.ShowSlotIsChanged = Convert.ToBoolean(RegistryHelper.GetValue("Overlay", "ShowSlotIsChanged", 1, false));
			this.ShowShiftIsChanged = App.UserSettingsService.ShowShiftIsChanged;
			this.ShowShiftIsChangedToggle = App.UserSettingsService.ShowShiftIsChangedToggle;
			this.ShowDisconnected = Convert.ToBoolean(RegistryHelper.GetValue("Overlay", "ShowDisconnected", 1, false));
			this.ShowMessages = Convert.ToBoolean(RegistryHelper.GetValue("Overlay", "ShowMessages", 1, false));
			this.ShowShortMessages = Convert.ToBoolean(RegistryHelper.GetValue("Overlay", "ShowShortMessages", 0, false));
			this.AlignNotification = RegistryHelper.GetValue("Overlay", "AlignNotification", 3, false);
			this.TransparentMessages = RegistryHelper.GetValue("Overlay", "TransparentMessages", 80, false);
			this.TimeMessages = RegistryHelper.GetValue("Overlay", "TimeMessages", 5, false);
			this.NotHideShift = App.UserSettingsService.NotHideShift;
			this.Scale = App.UserSettingsService.MessagesWidowScale;
			return Task.CompletedTask;
		}

		public override Task<bool> ApplyChanges()
		{
			RegistryHelper.SetValue("Overlay", "ShowBatteryIsLow", Convert.ToInt32(this.ShowBatteryIsLow));
			RegistryHelper.SetValue("Overlay", "ShowBatteryIsCritical", Convert.ToInt32(this.ShowBatteryIsCritical));
			RegistryHelper.SetValue("Overlay", "ShowSlotIsChanged", Convert.ToInt32(this.ShowSlotIsChanged));
			App.UserSettingsService.ShowShiftIsChanged = this.ShowShiftIsChanged;
			App.UserSettingsService.ShowShiftIsChangedToggle = this.ShowShiftIsChangedToggle;
			RegistryHelper.SetValue("Overlay", "ShowDisconnected", Convert.ToInt32(this.ShowDisconnected));
			RegistryHelper.SetValue("Overlay", "ShowRemapIsONOFF", Convert.ToInt32(this.ShowRemapIsONOFF));
			RegistryHelper.SetValue("Overlay", "ShowMessages", Convert.ToInt32(this.ShowMessages));
			RegistryHelper.SetValue("Overlay", "ShowShortMessages", Convert.ToInt32(this.ShowShortMessages));
			RegistryHelper.SetValue("Overlay", "AlignNotification", Convert.ToInt32(this.AlignNotification));
			RegistryHelper.SetValue("Overlay", "TransparentMessages", this.TransparentMessages);
			RegistryHelper.SetValue("Overlay", "TimeMessages", this.TimeMessages);
			App.UserSettingsService.NotHideShift = this.NotHideShift;
			RegistryHelper.SetString("Overlay", "MonitorMessages", this.SelectedMonitor);
			App.UserSettingsService.MessagesWidowScale = this.Scale;
			return Task.FromResult<bool>(true);
		}

		public void ShowOverlayChanged()
		{
			this.OnPropertyChanged("ShowMessagesControll");
			this.OnPropertyChanged("ShowMessagesAndOverlay");
			this.OnPropertyChanged("ShowNotHideShift");
			this.OnPropertyChanged("VisibilityBrushMessages");
			this.OnPropertyChanged("VisibilityBrushMessagesGrey");
		}

		public PreferencesOverlayVM ParentVM { get; set; }

		public string SelectedMonitor
		{
			get
			{
				return this._selectedMonitor;
			}
			set
			{
				this.SetProperty<string>(ref this._selectedMonitor, value, "SelectedMonitor");
			}
		}

		public bool ShowBatteryIsLow
		{
			get
			{
				return this._showBatteryIsLow;
			}
			set
			{
				if (value == this._showBatteryIsLow)
				{
					return;
				}
				this._showBatteryIsLow = value;
				this.OnPropertyChanged("ShowBatteryIsLow");
				this.OnPropertyChanged("ShowMessagesControll");
			}
		}

		public bool ShowBatteryIsCritical
		{
			get
			{
				return this._showBatteryIsCritical;
			}
			set
			{
				if (value == this._showBatteryIsCritical)
				{
					return;
				}
				this._showBatteryIsCritical = value;
				this.OnPropertyChanged("ShowBatteryIsCritical");
				this.OnPropertyChanged("ShowMessagesControll");
			}
		}

		public bool ShowRemapIsONOFF
		{
			get
			{
				return this._showRemapIsONOFF;
			}
			set
			{
				if (value == this._showRemapIsONOFF)
				{
					return;
				}
				this._showRemapIsONOFF = value;
				this.OnPropertyChanged("ShowRemapIsONOFF");
				this.OnPropertyChanged("ShowMessagesControll");
			}
		}

		public bool ShowSlotIsChanged
		{
			get
			{
				return this._showSlotIsChanged;
			}
			set
			{
				if (value == this._showSlotIsChanged)
				{
					return;
				}
				this._showSlotIsChanged = value;
				this.OnPropertyChanged("ShowSlotIsChanged");
				this.OnPropertyChanged("ShowMessagesControll");
			}
		}

		public bool ShowDisconnected
		{
			get
			{
				return this._showDisconnected;
			}
			set
			{
				if (value == this._showDisconnected)
				{
					return;
				}
				this._showDisconnected = value;
				this.OnPropertyChanged("ShowDisconnected");
				this.OnPropertyChanged("ShowMessagesControll");
			}
		}

		public bool ShowShiftIsChangedToggle
		{
			get
			{
				return this._showShiftIsChangedToggle;
			}
			set
			{
				if (value == this._showShiftIsChangedToggle)
				{
					return;
				}
				this._showShiftIsChangedToggle = value;
				this.OnPropertyChanged("ShowShiftIsChangedToggle");
				this.OnPropertyChanged("ShowMessagesControll");
				this.OnPropertyChanged("ShowNotHideShift");
			}
		}

		public bool ShowShiftIsChanged
		{
			get
			{
				return this._showShiftIsChanged;
			}
			set
			{
				if (value == this._showShiftIsChanged)
				{
					return;
				}
				this._showShiftIsChanged = value;
				this.OnPropertyChanged("ShowShiftIsChanged");
				this.OnPropertyChanged("ShowMessagesControll");
				this.OnPropertyChanged("ShowNotHideShift");
			}
		}

		public bool? ShowMessagesControll
		{
			get
			{
				bool? flag = new bool?(true);
				if (this._showMessages)
				{
					if (!this.ShowBatteryIsLow && !this.ShowBatteryIsCritical && !this.ShowRemapIsONOFF && !this.ShowSlotIsChanged && !this.ShowShiftIsChangedToggle && !this.ShowShiftIsChanged && !this.ShowDisconnected)
					{
						MultiRangeSlider.FireCloseAllPopups();
						this.ShowMessages = false;
						return new bool?(false);
					}
					if (this.ShowBatteryIsLow && this.ShowBatteryIsCritical && this.ShowRemapIsONOFF && this.ShowSlotIsChanged && this.ShowShiftIsChangedToggle && this.ShowShiftIsChanged && this.ShowDisconnected)
					{
						flag = new bool?(true);
					}
					else
					{
						flag = null;
					}
				}
				else
				{
					flag = new bool?(false);
				}
				return flag;
			}
			set
			{
				this.ShowMessages = value.Value;
				if (this.ShowMessages && !this.ShowBatteryIsLow && !this.ShowBatteryIsCritical && !this.ShowRemapIsONOFF && !this.ShowSlotIsChanged && !this.ShowShiftIsChangedToggle && !this.ShowShiftIsChanged && !this.ShowDisconnected)
				{
					this.ShowBatteryIsLow = true;
					this.ShowBatteryIsCritical = true;
					this.ShowRemapIsONOFF = true;
					this.ShowSlotIsChanged = true;
					this.ShowShiftIsChanged = true;
					this.ShowShiftIsChangedToggle = true;
					this.ShowDisconnected = true;
				}
				this.OnPropertyChanged("ShowMessagesControll");
				this.OnPropertyChanged("ShowNotHideShift");
				this.OnPropertyChanged("ShowMessagesAndOverlay");
				this.OnPropertyChanged("VisibilityBrushMessages");
				this.OnPropertyChanged("VisibilityBrushMessagesGrey");
				MultiRangeSlider.FireCloseAllPopups();
			}
		}

		public bool ShowMessages
		{
			get
			{
				return this._showMessages;
			}
			set
			{
				if (value == this._showMessages)
				{
					return;
				}
				this._showMessages = value;
				this.OnPropertyChanged("ShowMessagesControll");
				this.OnPropertyChanged("ShowNotHideShift");
				this.OnPropertyChanged("ShowMessagesAndOverlay");
				this.OnPropertyChanged("VisibilityBrushMessages");
				this.OnPropertyChanged("VisibilityBrushMessagesGrey");
			}
		}

		public bool ShowShortMessages
		{
			get
			{
				return this._showShortMessages;
			}
			set
			{
				if (value == this._showShortMessages)
				{
					return;
				}
				this._showShortMessages = value;
				this.OnPropertyChanged("ShowShortMessages");
			}
		}

		public bool NotHideShift
		{
			get
			{
				return this._notHideShift;
			}
			set
			{
				if (value == this._notHideShift)
				{
					return;
				}
				this._notHideShift = value;
				this.OnPropertyChanged("NotHideShift");
			}
		}

		public bool ShowNotHideShift
		{
			get
			{
				return this._showMessages && this.ParentVM.ShowOverlay && (this._showShiftIsChanged || this._showShiftIsChangedToggle);
			}
		}

		public bool ShowMessagesAndOverlay
		{
			get
			{
				return this._showMessages && this.ParentVM.ShowOverlay;
			}
		}

		public Brush VisibilityBrushMessages
		{
			get
			{
				if (!this.ShowMessagesAndOverlay)
				{
					return new SolidColorBrush(Colors.Gray);
				}
				return Application.Current.TryFindResource("Shift2Brush") as SolidColorBrush;
			}
		}

		public Brush VisibilityBrushMessagesGrey
		{
			get
			{
				if (!this.ShowMessagesAndOverlay)
				{
					return new SolidColorBrush(Color.FromRgb(80, 80, 80));
				}
				return new SolidColorBrush(Colors.Gray);
			}
		}

		public AlignType AlignNotification
		{
			get
			{
				return this._alignNotification;
			}
			set
			{
				if (value == this._alignNotification)
				{
					return;
				}
				this._alignNotification = value;
				this.OnPropertyChanged("AlignNotification");
				this.OnPropertyChanged("AlignNotificationTopLeft");
				this.OnPropertyChanged("AlignNotificationBottomLeft");
				this.OnPropertyChanged("AlignNotificationBottomRight");
				this.OnPropertyChanged("AlignNotificationTopRight");
			}
		}

		public bool AlignNotificationTopLeft
		{
			get
			{
				return this._alignNotification == 0;
			}
			set
			{
				if (!value)
				{
					return;
				}
				this.AlignNotification = 0;
			}
		}

		public bool AlignNotificationTopRight
		{
			get
			{
				return this._alignNotification == 1;
			}
			set
			{
				if (!value)
				{
					return;
				}
				this.AlignNotification = 1;
			}
		}

		public bool AlignNotificationBottomLeft
		{
			get
			{
				return this._alignNotification == 2;
			}
			set
			{
				if (!value)
				{
					return;
				}
				this.AlignNotification = 2;
			}
		}

		public bool AlignNotificationBottomRight
		{
			get
			{
				return this._alignNotification == 3;
			}
			set
			{
				if (!value)
				{
					return;
				}
				this.AlignNotification = 3;
			}
		}

		public int TransparentMessages
		{
			get
			{
				return this._opacityMessages;
			}
			set
			{
				this._opacityMessages = value;
				this.OnPropertyChanged("TransparentMessages");
			}
		}

		public double Scale
		{
			get
			{
				return this._scale;
			}
			set
			{
				this._scale = value;
				this.OnPropertyChanged("Scale");
			}
		}

		public int TimeMessages
		{
			get
			{
				return this._timeMessages;
			}
			set
			{
				this._timeMessages = value;
				this.OnPropertyChanged("TimeMessages");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool ExpandPosition
		{
			get
			{
				return this._expandPosition;
			}
			set
			{
				if (value == this._expandPosition)
				{
					return;
				}
				this._expandPosition = value;
				this._expandAdjustment = false;
				if (!this._expandAdjustment)
				{
					MultiRangeSlider.FireCloseAllPopups();
				}
				this.OnPropertyChanged("ExpandPosition");
				this.OnPropertyChanged("ExpandAdjustment");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool ExpandAdjustment
		{
			get
			{
				return this._expandAdjustment;
			}
			set
			{
				if (value == this._expandAdjustment)
				{
					return;
				}
				this._expandAdjustment = value;
				this._expandPosition = false;
				if (!this._expandAdjustment)
				{
					MultiRangeSlider.FireCloseAllPopups();
				}
				this.OnPropertyChanged("ExpandAdjustment");
				this.OnPropertyChanged("ExpandPosition");
			}
		}

		private string _selectedMonitor;

		private bool _showBatteryIsLow;

		private bool _showBatteryIsCritical;

		private bool _showRemapIsONOFF;

		private bool _showSlotIsChanged;

		private bool _showDisconnected;

		private bool _showShiftIsChangedToggle;

		private bool _showShiftIsChanged;

		private bool _showMessages;

		private bool _showShortMessages;

		private bool _notHideShift;

		private AlignType _alignNotification;

		private int _opacityMessages;

		private double _scale;

		private int _timeMessages;

		private bool _expandPosition = true;

		private bool _expandAdjustment;
	}
}
