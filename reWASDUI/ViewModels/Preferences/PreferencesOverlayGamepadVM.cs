using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.View.Controls.MultiRangeSlider;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using DTOverlay;
using Prism.Commands;
using reWASDUI.DataModels.GamepadSlotHotkeyCollection;
using reWASDUI.ViewModels.Preferences.Base;

namespace reWASDUI.ViewModels.Preferences
{
	public class PreferencesOverlayGamepadVM : PreferencesBaseVM
	{
		public PreferencesOverlayGamepadVM(PreferencesOverlayVM parent)
		{
			this.ParentVM = parent;
		}

		public override Task Initialize()
		{
			string @string = RegistryHelper.GetString("Overlay", "MonitorGamepad", "", false);
			this.SelectedMonitor = this.ParentVM.GetCheckedMonitorName(this.SelectedMonitor, @string);
			this.AutohideGamepad = Convert.ToBoolean(RegistryHelper.GetValue("Overlay", "AutohideGamepad", 0, false));
			this.ShowGamepad = Convert.ToBoolean(RegistryHelper.GetValue("Overlay", "ShowGamepad", 1, false));
			this.ShowControllerOnly = Convert.ToBoolean(RegistryHelper.GetValue("Overlay", "ShowControllerOnly", 0, false));
			this.AlignGamepad = RegistryHelper.GetValue("Overlay", "AlignGamepad", 3, false);
			this.TransparentGamepad = RegistryHelper.GetValue("Overlay", "TransparentGamepad", 80, false);
			this.PollingRate = RegistryHelper.GetValue("Overlay", "PollingRate", 50, false);
			this.Scale = App.UserSettingsService.GamepadWidowScale;
			return Task.CompletedTask;
		}

		public override Task<bool> ApplyChanges()
		{
			RegistryHelper.SetValue("Overlay", "AutohideGamepad", Convert.ToInt32(this.AutohideGamepad));
			RegistryHelper.SetValue("Overlay", "ShowGamepad", Convert.ToInt32(this.ShowGamepad));
			RegistryHelper.SetValue("Overlay", "ShowControllerOnly", Convert.ToInt32(this.ShowControllerOnly));
			RegistryHelper.SetValue("Overlay", "AlignGamepad", Convert.ToInt32(this.AlignGamepad));
			RegistryHelper.SetValue("Overlay", "TransparentGamepad", this.TransparentGamepad);
			RegistryHelper.SetValue("Overlay", "PollingRate", this.PollingRate);
			RegistryHelper.SetString("Overlay", "MonitorGamepad", this.SelectedMonitor);
			App.UserSettingsService.GamepadWidowScale = this.Scale;
			return Task.FromResult<bool>(true);
		}

		public void ShowOverlayChanged()
		{
			this.OnPropertyChanged("ShowGamepadAndOverlay");
			this.OnPropertyChanged("VisibilityBrushGamepad");
			this.OnPropertyChanged("VisibilityBrushGamepadGrey");
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

		public bool AutohideGamepad
		{
			get
			{
				return this._autohideGamepad;
			}
			set
			{
				if (value == this._autohideGamepad)
				{
					return;
				}
				this._autohideGamepad = value;
				this.OnPropertyChanged("AutohideGamepad");
			}
		}

		public bool ShowGamepad
		{
			get
			{
				return this._showGamepad;
			}
			set
			{
				if (value == this._showGamepad)
				{
					return;
				}
				this._showGamepad = value;
				this.OnPropertyChanged("ShowGamepad");
				this.OnPropertyChanged("ShowGamepadAndOverlay");
				this.OnPropertyChanged("VisibilityBrushGamepad");
				this.OnPropertyChanged("VisibilityBrushGamepadGrey");
				MultiRangeSlider.FireCloseAllPopups();
				this.ParentVM.IsDictionaryChanged = true;
			}
		}

		public bool ShowGamepadAndOverlay
		{
			get
			{
				return this._showGamepad && this.ParentVM.ShowOverlay;
			}
		}

		public Brush VisibilityBrushGamepad
		{
			get
			{
				if (!this.ShowGamepadAndOverlay)
				{
					return new SolidColorBrush(Colors.Gray);
				}
				return Application.Current.TryFindResource("Shift2Brush") as SolidColorBrush;
			}
		}

		public Brush VisibilityBrushGamepadGrey
		{
			get
			{
				if (!this.ShowGamepadAndOverlay)
				{
					return new SolidColorBrush(Color.FromRgb(80, 80, 80));
				}
				return new SolidColorBrush(Colors.Gray);
			}
		}

		public bool ShowControllerOnly
		{
			get
			{
				return this._showControllerOnly;
			}
			set
			{
				if (value == this._showControllerOnly)
				{
					return;
				}
				this._showControllerOnly = value;
				this.OnPropertyChanged("ShowControllerOnly");
			}
		}

		public int TransparentGamepad
		{
			get
			{
				return this._opacityGamepad;
			}
			set
			{
				this._opacityGamepad = value;
				this.OnPropertyChanged("TransparentGamepad");
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

		public int PollingRate
		{
			get
			{
				return this._pollingRate;
			}
			set
			{
				this._pollingRate = value;
				this.OnPropertyChanged("PollingRate");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public KeyValuePair<string, HotkeyCollection> CurrentKeyValuePairGamepad
		{
			get
			{
				return this._currentKeyValuePairGamepad;
			}
			set
			{
				if (object.Equals(this._currentKeyValuePairGamepad, value))
				{
					return;
				}
				this._currentKeyValuePairGamepad = value;
				HotkeyCollection value2 = this._currentKeyValuePairGamepad.Value;
				if (value2 != null)
				{
					value2.RefreshButtonsForSlotHotkey(true);
				}
				this.RemoveVirtualEntryCommand.RaiseCanExecuteChanged();
				this.OnPropertyChanged("CurrentKeyValuePairGamepad");
			}
		}

		public AlignType AlignGamepad
		{
			get
			{
				return this._alignGamepad;
			}
			set
			{
				if (value == this._alignGamepad)
				{
					return;
				}
				this._alignGamepad = value;
				this.OnPropertyChanged("AlignGamepad");
				this.OnPropertyChanged("AlignGamepadTopLeft");
				this.OnPropertyChanged("AlignGamepadBottomLeft");
				this.OnPropertyChanged("AlignGamepadBottomRight");
				this.OnPropertyChanged("AlignGamepadTopRight");
			}
		}

		public bool AlignGamepadTopLeft
		{
			get
			{
				return this._alignGamepad == 0;
			}
			set
			{
				if (!value)
				{
					return;
				}
				this.AlignGamepad = 0;
			}
		}

		public bool AlignGamepadTopRight
		{
			get
			{
				return this._alignGamepad == 1;
			}
			set
			{
				if (!value)
				{
					return;
				}
				this.AlignGamepad = 1;
			}
		}

		public bool AlignGamepadBottomLeft
		{
			get
			{
				return this._alignGamepad == 2;
			}
			set
			{
				if (!value)
				{
					return;
				}
				this.AlignGamepad = 2;
			}
		}

		public bool AlignGamepadBottomRight
		{
			get
			{
				return this._alignGamepad == 3;
			}
			set
			{
				if (!value)
				{
					return;
				}
				this.AlignGamepad = 3;
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool ExpandGamepadOverlayHotkeys
		{
			get
			{
				return this._expandGamepadOverlayHotkeys;
			}
			set
			{
				if (value == this._expandGamepadOverlayHotkeys)
				{
					return;
				}
				this._expandGamepadOverlayHotkeys = value;
				this._expandGamepadPosition = false;
				this._expandGamepadAdjustment = false;
				MultiRangeSlider.FireCloseAllPopups();
				this.OnPropertyChanged("ExpandGamepadOverlayHotkeys");
				this.OnPropertyChanged("ExpandGamepadPosition");
				this.OnPropertyChanged("ExpandGamepadAdjustment");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool ExpandGamepadPosition
		{
			get
			{
				return this._expandGamepadPosition;
			}
			set
			{
				if (value == this._expandGamepadPosition)
				{
					return;
				}
				this._expandGamepadPosition = value;
				this._expandGamepadAdjustment = false;
				this._expandGamepadOverlayHotkeys = false;
				if (!this._expandGamepadAdjustment)
				{
					MultiRangeSlider.FireCloseAllPopups();
				}
				this.OnPropertyChanged("ExpandGamepadPosition");
				this.OnPropertyChanged("ExpandGamepadAdjustment");
				this.OnPropertyChanged("ExpandGamepadOverlayHotkeys");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool ExpandGamepadAdjustment
		{
			get
			{
				return this._expandGamepadAdjustment;
			}
			set
			{
				if (value == this._expandGamepadAdjustment)
				{
					return;
				}
				this._expandGamepadAdjustment = value;
				this._expandGamepadPosition = false;
				this._expandGamepadOverlayHotkeys = false;
				if (!this._expandGamepadAdjustment)
				{
					MultiRangeSlider.FireCloseAllPopups();
				}
				this.OnPropertyChanged("ExpandGamepadAdjustment");
				this.OnPropertyChanged("ExpandGamepadPosition");
				this.OnPropertyChanged("ExpandGamepadOverlayHotkeys");
			}
		}

		public DelegateCommand RemoveVirtualEntryCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._RemoveVirtualEntry) == null)
				{
					delegateCommand = (this._RemoveVirtualEntry = new DelegateCommand(new Action(this.RemoveVirtualEntry), new Func<bool>(this.RemoveVirtualEntryCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void RemoveVirtualEntry()
		{
			if (DTMessageBox.Show(DTLocalization.GetString(11219), MessageBoxButton.YesNo, MessageBoxImage.Question, null, false, MessageBoxResult.None) == MessageBoxResult.Yes)
			{
				App.GamepadService.GamepadsHotkeyCollection.Remove(this.CurrentKeyValuePairGamepad.Key);
				this.ParentVM.SaveHotkeys(false);
			}
		}

		private bool RemoveVirtualEntryCanExecute()
		{
			return this.ParentVM.GamepadIsInCollectionOrInCompositeController(this.CurrentKeyValuePairGamepad);
		}

		private string _selectedMonitor;

		private bool _autohideGamepad;

		private bool _showGamepad;

		private bool _showControllerOnly;

		private int _opacityGamepad;

		private double _scale;

		private int _pollingRate;

		private KeyValuePair<string, HotkeyCollection> _currentKeyValuePairGamepad;

		private AlignType _alignGamepad;

		private bool _expandGamepadOverlayHotkeys = true;

		private bool _expandGamepadPosition;

		private bool _expandGamepadAdjustment;

		private DelegateCommand _RemoveVirtualEntry;
	}
}
