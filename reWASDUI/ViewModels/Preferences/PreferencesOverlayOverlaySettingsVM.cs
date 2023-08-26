using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.View.Controls.MultiRangeSlider;
using DTOverlay;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDUI.ViewModels.Preferences.Base;
using UtilsDisplayName;

namespace reWASDUI.ViewModels.Preferences
{
	public class PreferencesOverlayOverlaySettingsVM : PreferencesBaseVM
	{
		public PreferencesOverlayOverlaySettingsVM(PreferencesOverlayVM parent)
		{
			this.ParentVM = parent;
		}

		public override Task Initialize()
		{
			this.ShowOverlayMappings = App.UserSettingsService.IsShowOverlayMappings;
			this.ShowOverlayDescriptions = App.UserSettingsService.IsShowOverlayDescriptions;
			string overlayMenuSelectedMonitor = App.UserSettingsService.OverlayMenuSelectedMonitor;
			this.SelectedMonitor = this.ParentVM.GetCheckedMonitorName(this.SelectedMonitor, overlayMenuSelectedMonitor);
			this.OverlayAlign = App.UserSettingsService.OverlayMenuAlign;
			this.Scale = App.UserSettingsService.OverlayScale;
			this.UpdatePreview();
			return Task.CompletedTask;
		}

		public override Task<bool> ApplyChanges()
		{
			App.UserSettingsService.OverlayMenuAlign = this.OverlayAlign;
			App.UserSettingsService.IsShowOverlayMappings = this.ShowOverlayMappings;
			App.UserSettingsService.IsShowOverlayDescriptions = this.ShowOverlayDescriptions;
			App.UserSettingsService.OverlayScale = this.Scale;
			App.UserSettingsService.OverlayMenuSelectedMonitor = this.SelectedMonitor;
			return Task.FromResult<bool>(true);
		}

		public void ShowOverlayChanged()
		{
		}

		private async void UpdatePreview()
		{
			try
			{
				OverlayCirclePreviewResult overlayCirclePreviewResult = await App.HttpClientService.GameProfiles.CreateOverlayCirclePreview(new OverlayCirclePreviewInfo
				{
					IsShowOverlayDescriptions = this.ShowOverlayDescriptions,
					OverlayAlign = this.OverlayAlign,
					IsShowOverlayMappings = this.ShowOverlayMappings,
					Size = this.Scale
				});
				if (((overlayCirclePreviewResult != null) ? overlayCirclePreviewResult.PreviewWindow : null) != null)
				{
					using (MemoryStream memoryStream = new MemoryStream(overlayCirclePreviewResult.PreviewWindow))
					{
						BitmapImage bitmapImage = new BitmapImage();
						bitmapImage.BeginInit();
						bitmapImage.StreamSource = memoryStream;
						bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
						bitmapImage.EndInit();
						this.PngPreview = bitmapImage.Clone();
					}
				}
			}
			catch
			{
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public BitmapImage PngPreview
		{
			get
			{
				return this._pngPreview;
			}
			set
			{
				if (value == this._pngPreview)
				{
					return;
				}
				this._pngPreview = value;
				this.OnPropertyChanged("PngPreview");
			}
		}

		public bool ShowOverlayMappings
		{
			get
			{
				return this._showOverlayMappings;
			}
			set
			{
				if (value == this._showOverlayMappings)
				{
					return;
				}
				this._showOverlayMappings = value;
				this.OnPropertyChanged("ShowOverlayMappings");
				this.UpdatePreview();
			}
		}

		public bool ShowOverlayDescriptions
		{
			get
			{
				return this._showOverlayDescriptions;
			}
			set
			{
				if (value == this._showOverlayDescriptions)
				{
					return;
				}
				this._showOverlayDescriptions = value;
				this.OnPropertyChanged("ShowOverlayDescriptions");
				this.UpdatePreview();
			}
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
				this.SelectedScreen = Screen.AllScreens.FirstOrDefault((Screen x) => ScreenInterrogatory.DeviceFriendlyName(x) == value);
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public Screen SelectedScreen
		{
			get
			{
				return this._selectedScreen;
			}
			set
			{
				this.SetProperty<Screen>(ref this._selectedScreen, value, "SelectedScreen");
				this.CheckBounds();
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public double ScreenWidth
		{
			get
			{
				return this._screenWidth;
			}
			set
			{
				this.SetProperty<double>(ref this._screenWidth, value, "ScreenWidth");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public double ScreenHeight
		{
			get
			{
				return this._screenHeight;
			}
			set
			{
				this.SetProperty<double>(ref this._screenHeight, value, "ScreenHeight");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public double OverlaySize
		{
			get
			{
				return this._overlaySize;
			}
			set
			{
				this.SetProperty<double>(ref this._overlaySize, value, "OverlaySize");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public System.Windows.HorizontalAlignment OverlayHorizontalAlign
		{
			get
			{
				return this._overlayHorizontalAlign;
			}
			set
			{
				this.SetProperty<System.Windows.HorizontalAlignment>(ref this._overlayHorizontalAlign, value, "OverlayHorizontalAlign");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public VerticalAlignment OverlayVerticalAlign
		{
			get
			{
				return this._overlayVerticalAlign;
			}
			set
			{
				this.SetProperty<VerticalAlignment>(ref this._overlayVerticalAlign, value, "OverlayVerticalAlign");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public System.Windows.HorizontalAlignment ScreenResolutionHorizontalAlign
		{
			get
			{
				return this._screenResolutionHorizontalAlign;
			}
			set
			{
				this.SetProperty<System.Windows.HorizontalAlignment>(ref this._screenResolutionHorizontalAlign, value, "ScreenResolutionHorizontalAlign");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public VerticalAlignment ScreenResolutionVerticalAlign
		{
			get
			{
				return this._screenResolutionVerticalAlign;
			}
			set
			{
				this.SetProperty<VerticalAlignment>(ref this._screenResolutionVerticalAlign, value, "ScreenResolutionVerticalAlign");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public string ScreenResolutionText
		{
			get
			{
				return this._screenResolutionText;
			}
			set
			{
				this.SetProperty<string>(ref this._screenResolutionText, value, "ScreenResolutionText");
			}
		}

		private void CheckBounds()
		{
			double num = 600.0;
			double num2 = 218.0;
			double num3 = (double)this.SelectedScreen.Bounds.Width;
			double num4 = (double)this.SelectedScreen.Bounds.Height;
			double num5 = (double)(this.Scale * 2);
			double num6 = num3 / num;
			double num7 = num3 / num6;
			double num8 = num4 / num6;
			double num9 = num5 / num6;
			if (num8 > num2)
			{
				num6 = num8 / num2;
				num7 /= num6;
				num8 /= num6;
				num9 /= num6;
			}
			switch (this.OverlayAlign)
			{
			case 0:
				this.OverlayHorizontalAlign = System.Windows.HorizontalAlignment.Left;
				this.ScreenResolutionHorizontalAlign = System.Windows.HorizontalAlignment.Right;
				this.OverlayVerticalAlign = VerticalAlignment.Top;
				this.ScreenResolutionVerticalAlign = VerticalAlignment.Bottom;
				break;
			case 1:
				this.OverlayHorizontalAlign = System.Windows.HorizontalAlignment.Right;
				this.ScreenResolutionHorizontalAlign = System.Windows.HorizontalAlignment.Right;
				this.OverlayVerticalAlign = VerticalAlignment.Top;
				this.ScreenResolutionVerticalAlign = VerticalAlignment.Bottom;
				break;
			case 2:
				this.OverlayHorizontalAlign = System.Windows.HorizontalAlignment.Left;
				this.ScreenResolutionHorizontalAlign = System.Windows.HorizontalAlignment.Right;
				this.OverlayVerticalAlign = VerticalAlignment.Bottom;
				this.ScreenResolutionVerticalAlign = VerticalAlignment.Top;
				break;
			case 3:
				this.OverlayHorizontalAlign = System.Windows.HorizontalAlignment.Right;
				this.ScreenResolutionHorizontalAlign = System.Windows.HorizontalAlignment.Left;
				this.OverlayVerticalAlign = VerticalAlignment.Bottom;
				this.ScreenResolutionVerticalAlign = VerticalAlignment.Top;
				break;
			case 4:
				this.OverlayHorizontalAlign = System.Windows.HorizontalAlignment.Center;
				this.ScreenResolutionHorizontalAlign = System.Windows.HorizontalAlignment.Right;
				this.OverlayVerticalAlign = VerticalAlignment.Center;
				this.ScreenResolutionVerticalAlign = VerticalAlignment.Bottom;
				break;
			}
			this.ScreenWidth = num7;
			this.ScreenHeight = num8;
			this.OverlaySize = num9;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
			defaultInterpolatedStringHandler.AppendFormatted<double>(num3);
			defaultInterpolatedStringHandler.AppendLiteral("x");
			defaultInterpolatedStringHandler.AppendFormatted<double>(num4);
			this.ScreenResolutionText = defaultInterpolatedStringHandler.ToStringAndClear();
		}

		public AlignType OverlayAlign
		{
			get
			{
				return this._OverlayAlign;
			}
			set
			{
				if (value == this._OverlayAlign)
				{
					return;
				}
				this._OverlayAlign = value;
				this.OnPropertyChanged("OverlayAlign");
				this.OnPropertyChanged("OverlayAlignTopLeft");
				this.OnPropertyChanged("OverlayAlignBottomLeft");
				this.OnPropertyChanged("OverlayAlignBottomRight");
				this.OnPropertyChanged("OverlayAlignTopRight");
				this.OnPropertyChanged("OverlayAlignCenter");
				this.CheckBounds();
			}
		}

		public bool OverlayAlignTopLeft
		{
			get
			{
				return this._OverlayAlign == 0;
			}
			set
			{
				if (!value)
				{
					return;
				}
				this.OverlayAlign = 0;
			}
		}

		public bool OverlayAlignTopRight
		{
			get
			{
				return this._OverlayAlign == 1;
			}
			set
			{
				if (!value)
				{
					return;
				}
				this.OverlayAlign = 1;
			}
		}

		public bool OverlayAlignCenter
		{
			get
			{
				return this._OverlayAlign == 4;
			}
			set
			{
				if (!value)
				{
					return;
				}
				this.OverlayAlign = 4;
			}
		}

		public bool OverlayAlignBottomLeft
		{
			get
			{
				return this._OverlayAlign == 2;
			}
			set
			{
				if (!value)
				{
					return;
				}
				this.OverlayAlign = 2;
			}
		}

		public bool OverlayAlignBottomRight
		{
			get
			{
				return this._OverlayAlign == 3;
			}
			set
			{
				if (!value)
				{
					return;
				}
				this.OverlayAlign = 3;
			}
		}

		public int Scale
		{
			get
			{
				return this._scale;
			}
			set
			{
				this._scale = value;
				this.OnPropertyChanged("Scale");
				this.CheckBounds();
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

		private BitmapImage _pngPreview;

		private bool _showOverlayMappings;

		private bool _showOverlayDescriptions;

		private string _selectedMonitor;

		private Screen _selectedScreen;

		private double _screenWidth;

		private double _screenHeight;

		private double _overlaySize;

		private System.Windows.HorizontalAlignment _overlayHorizontalAlign;

		private VerticalAlignment _overlayVerticalAlign;

		private System.Windows.HorizontalAlignment _screenResolutionHorizontalAlign;

		private VerticalAlignment _screenResolutionVerticalAlign;

		private string _screenResolutionText;

		private AlignType _OverlayAlign;

		private int _scale;

		private bool _expandPosition = true;

		private bool _expandAdjustment;
	}
}
