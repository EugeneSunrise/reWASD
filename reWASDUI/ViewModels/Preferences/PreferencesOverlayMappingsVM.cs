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
	public class PreferencesOverlayMappingsVM : PreferencesBaseVM
	{
		public PreferencesOverlayMappingsVM(PreferencesOverlayVM parent)
		{
			this.ParentVM = parent;
		}

		public override Task Initialize()
		{
			string @string = RegistryHelper.GetString("Overlay", "MonitorMappings", "", false);
			this.SelectedMonitor = this.ParentVM.GetCheckedMonitorName(this.SelectedMonitor, @string);
			this.ShowMappings = App.UserSettingsService.IsOverlayShowMappingsEnable;
			this.AlignMappings = RegistryHelper.GetValue("Overlay", "AlignMappings", 2, false);
			this.TransparentMappings = RegistryHelper.GetValue("Overlay", "TransparentMappings", 80, false);
			this.Scale = App.UserSettingsService.RemapWidowScale;
			return Task.CompletedTask;
		}

		public override Task<bool> ApplyChanges()
		{
			App.UserSettingsService.IsOverlayShowMappingsEnable = this.ShowMappings;
			RegistryHelper.SetString("Overlay", "SelectedMonitor", this.SelectedMonitor);
			RegistryHelper.SetValue("Overlay", "AlignMappings", Convert.ToInt32(this.AlignMappings));
			RegistryHelper.SetValue("Overlay", "TransparentMappings", this.TransparentMappings);
			RegistryHelper.SetString("Overlay", "MonitorMappings", this.SelectedMonitor);
			App.UserSettingsService.RemapWidowScale = this.Scale;
			return Task.FromResult<bool>(true);
		}

		public void ShowOverlayChanged()
		{
			this.OnPropertyChanged("ShowMappingsAndOverlay");
			this.OnPropertyChanged("VisibilityBrushMappings");
			this.OnPropertyChanged("VisibilityBrushMappingsGrey");
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

		public bool ShowMappings
		{
			get
			{
				return this._showMappings;
			}
			set
			{
				if (value == this._showMappings)
				{
					return;
				}
				this._showMappings = value;
				this.OnPropertyChanged("ShowMappings");
				this.OnPropertyChanged("ShowMappingsAndOverlay");
				this.OnPropertyChanged("VisibilityBrushMappings");
				this.OnPropertyChanged("VisibilityBrushMappingsGrey");
				MultiRangeSlider.FireCloseAllPopups();
				this.ParentVM.IsDictionaryChanged = true;
			}
		}

		public bool ShowMappingsAndOverlay
		{
			get
			{
				return this._showMappings && this.ParentVM.ShowOverlay;
			}
		}

		public Brush VisibilityBrushMappings
		{
			get
			{
				if (!this.ShowMappingsAndOverlay)
				{
					return new SolidColorBrush(Colors.Gray);
				}
				return Application.Current.TryFindResource("Shift2Brush") as SolidColorBrush;
			}
		}

		public Brush VisibilityBrushMappingsGrey
		{
			get
			{
				if (!this.ShowMappingsAndOverlay)
				{
					return new SolidColorBrush(Color.FromRgb(80, 80, 80));
				}
				return new SolidColorBrush(Colors.Gray);
			}
		}

		public AlignType AlignMappings
		{
			get
			{
				return this._alignMappings;
			}
			set
			{
				if (value == this._alignMappings)
				{
					return;
				}
				this._alignMappings = value;
				this.OnPropertyChanged("AlignMappings");
				this.OnPropertyChanged("AlignMappingsTopLeft");
				this.OnPropertyChanged("AlignMappingsBottomLeft");
				this.OnPropertyChanged("AlignMappingsBottomRight");
				this.OnPropertyChanged("AlignMappingsTopRight");
			}
		}

		public bool AlignMappingsTopLeft
		{
			get
			{
				return this._alignMappings == 0;
			}
			set
			{
				if (!value)
				{
					return;
				}
				this.AlignMappings = 0;
			}
		}

		public bool AlignMappingsTopRight
		{
			get
			{
				return this._alignMappings == 1;
			}
			set
			{
				if (!value)
				{
					return;
				}
				this.AlignMappings = 1;
			}
		}

		public bool AlignMappingsBottomLeft
		{
			get
			{
				return this._alignMappings == 2;
			}
			set
			{
				this.AlignMappings = 2;
			}
		}

		public bool AlignMappingsBottomRight
		{
			get
			{
				return this._alignMappings == 3;
			}
			set
			{
				if (!value)
				{
					return;
				}
				this.AlignMappings = 3;
			}
		}

		public int TransparentMappings
		{
			get
			{
				return this._opacityMappings;
			}
			set
			{
				this._opacityMappings = value;
				this.OnPropertyChanged("TransparentMappings");
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

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public KeyValuePair<string, HotkeyCollection> CurrentKeyValuePairMapping
		{
			get
			{
				return this._currentKeyValuePairMapping;
			}
			set
			{
				if (object.Equals(this._currentKeyValuePairMapping, value))
				{
					return;
				}
				this._currentKeyValuePairMapping = value;
				HotkeyCollection value2 = this._currentKeyValuePairMapping.Value;
				if (value2 != null)
				{
					value2.RefreshButtonsForSlotHotkey(true);
				}
				this.RemoveMappingEntryCommand.RaiseCanExecuteChanged();
				this.OnPropertyChanged("CurrentKeyValuePairMapping");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public KeyValuePair<string, HotkeyCollection> CurrentKeyValuePairMappingDescriptions
		{
			get
			{
				return this._currentKeyValuePairMappingDescriptions;
			}
			set
			{
				if (object.Equals(this._currentKeyValuePairMappingDescriptions, value))
				{
					return;
				}
				this._currentKeyValuePairMappingDescriptions = value;
				HotkeyCollection value2 = this._currentKeyValuePairMappingDescriptions.Value;
				if (value2 != null)
				{
					value2.RefreshButtonsForSlotHotkey(true);
				}
				this.RemoveMappingEntryCommandDescriptions.RaiseCanExecuteChanged();
				this.OnPropertyChanged("CurrentKeyValuePairMappingDescriptions");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool ExpandMappingOverlayHotkeys
		{
			get
			{
				return this._expandMappingOverlayHotkeys;
			}
			set
			{
				if (value == this._expandMappingOverlayHotkeys)
				{
					return;
				}
				this._expandMappingOverlayHotkeys = value;
				this._expandMappingPosition = false;
				this._expandMappingAdjustment = false;
				this._expandMappingDescriptionsHotkeys = false;
				MultiRangeSlider.FireCloseAllPopups();
				this.OnPropertyChanged("ExpandMappingOverlayHotkeys");
				this.OnPropertyChanged("ExpandMappingDescriptionsHotkeys");
				this.OnPropertyChanged("ExpandMappingPosition");
				this.OnPropertyChanged("ExpandMappingAdjustment");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool ExpandMappingDescriptionsHotkeys
		{
			get
			{
				return this._expandMappingDescriptionsHotkeys;
			}
			set
			{
				if (value == this._expandMappingDescriptionsHotkeys)
				{
					return;
				}
				this._expandMappingDescriptionsHotkeys = value;
				this._expandMappingPosition = false;
				this._expandMappingAdjustment = false;
				this._expandMappingOverlayHotkeys = false;
				MultiRangeSlider.FireCloseAllPopups();
				this.OnPropertyChanged("ExpandMappingDescriptionsHotkeys");
				this.OnPropertyChanged("ExpandMappingOverlayHotkeys");
				this.OnPropertyChanged("ExpandMappingPosition");
				this.OnPropertyChanged("ExpandMappingAdjustment");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool ExpandMappingPosition
		{
			get
			{
				return this._expandMappingPosition;
			}
			set
			{
				if (value == this._expandMappingPosition)
				{
					return;
				}
				this._expandMappingPosition = value;
				this._expandMappingAdjustment = false;
				this._expandMappingOverlayHotkeys = false;
				this._expandMappingDescriptionsHotkeys = false;
				MultiRangeSlider.FireCloseAllPopups();
				this.OnPropertyChanged("ExpandMappingPosition");
				this.OnPropertyChanged("ExpandMappingAdjustment");
				this.OnPropertyChanged("ExpandMappingDescriptionsHotkeys");
				this.OnPropertyChanged("ExpandMappingOverlayHotkeys");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool ExpandMappingAdjustment
		{
			get
			{
				return this._expandMappingAdjustment;
			}
			set
			{
				if (value == this._expandMappingAdjustment)
				{
					return;
				}
				this._expandMappingAdjustment = value;
				this._expandMappingPosition = false;
				this._expandMappingOverlayHotkeys = false;
				this._expandMappingDescriptionsHotkeys = false;
				if (!this._expandMappingAdjustment)
				{
					MultiRangeSlider.FireCloseAllPopups();
				}
				this.OnPropertyChanged("ExpandMappingAdjustment");
				this.OnPropertyChanged("ExpandMappingDescriptionsHotkeys");
				this.OnPropertyChanged("ExpandMappingPosition");
				this.OnPropertyChanged("ExpandMappingOverlayHotkeys");
			}
		}

		public DelegateCommand RemoveMappingEntryCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._RemoveMappingEntry) == null)
				{
					delegateCommand = (this._RemoveMappingEntry = new DelegateCommand(new Action(this.RemoveMappingEntry), new Func<bool>(this.RemoveMappingEntryCanExecute)));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand RemoveMappingEntryCommandDescriptions
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._RemoveMappingEntryDescriptions) == null)
				{
					delegateCommand = (this._RemoveMappingEntryDescriptions = new DelegateCommand(new Action(this.RemoveMappingEntryDescriptions), new Func<bool>(this.RemoveMappingEntryCanExecuteDescriptions)));
				}
				return delegateCommand;
			}
		}

		private void RemoveMappingEntryDescriptions()
		{
			if (DTMessageBox.Show(DTLocalization.GetString(11219), MessageBoxButton.YesNo, MessageBoxImage.Question, null, false, MessageBoxResult.None) == MessageBoxResult.Yes)
			{
				App.GamepadService.GamepadsHotkeyCollection.Remove(this.CurrentKeyValuePairMappingDescriptions.Key);
				this.ParentVM.SaveHotkeys(false);
			}
		}

		private void RemoveMappingEntry()
		{
			if (DTMessageBox.Show(DTLocalization.GetString(11219), MessageBoxButton.YesNo, MessageBoxImage.Question, null, false, MessageBoxResult.None) == MessageBoxResult.Yes)
			{
				App.GamepadService.GamepadsHotkeyCollection.Remove(this.CurrentKeyValuePairMapping.Key);
				this.ParentVM.SaveHotkeys(false);
			}
		}

		private bool RemoveMappingEntryCanExecute()
		{
			return this.ParentVM.GamepadIsInCollectionOrInCompositeController(this.CurrentKeyValuePairMapping);
		}

		private bool RemoveMappingEntryCanExecuteDescriptions()
		{
			return this.ParentVM.GamepadIsInCollectionOrInCompositeController(this.CurrentKeyValuePairMappingDescriptions);
		}

		private string _selectedMonitor;

		private bool _showMappings;

		private AlignType _alignMappings;

		private int _opacityMappings;

		private double _scale;

		private KeyValuePair<string, HotkeyCollection> _currentKeyValuePairMapping;

		private KeyValuePair<string, HotkeyCollection> _currentKeyValuePairMappingDescriptions;

		private bool _expandMappingOverlayHotkeys = true;

		private bool _expandMappingDescriptionsHotkeys;

		private bool _expandMappingPosition;

		private bool _expandMappingAdjustment;

		private DelegateCommand _RemoveMappingEntry;

		private DelegateCommand _RemoveMappingEntryDescriptions;
	}
}
