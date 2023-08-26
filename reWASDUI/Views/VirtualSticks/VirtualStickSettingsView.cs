using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.View.Controls.ComboBoxes;
using reWASDUI.Controls;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.XBEliteService;

namespace reWASDUI.Views.VirtualSticks
{
	public class VirtualStickSettingsView : BaseServicesDataContextControl, INotifyPropertyChanged, IComponentConnector, IStyleConnector
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public VirtualStickSettings VirtualStickSettings
		{
			get
			{
				return (VirtualStickSettings)base.GetValue(VirtualStickSettingsView.VirtualStickSettingsProperty);
			}
			set
			{
				base.SetValue(VirtualStickSettingsView.VirtualStickSettingsProperty, value);
			}
		}

		private static void OnVirtualStickSettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VirtualStickSettingsView virtualStickSettingsView = d as VirtualStickSettingsView;
			if (virtualStickSettingsView == null)
			{
				return;
			}
			VirtualStickSettings virtualStickSettings = e.OldValue as VirtualStickSettings;
			if (virtualStickSettings != null)
			{
				virtualStickSettingsView.DetachEventsForCustomResponse(virtualStickSettings);
				ThumbstickSensitivity thumbstickSensitivity = virtualStickSettingsView.SquarenessCollection.FirstOrDefault((ThumbstickSensitivity item) => item.IsCustom);
				if (thumbstickSensitivity != null)
				{
					thumbstickSensitivity.Deflection = virtualStickSettingsView.CreateDefaultSquareSettings().Deflection.Clone();
				}
			}
			VirtualStickSettings virtualStickSettings2 = e.NewValue as VirtualStickSettings;
			if (virtualStickSettings2 != null)
			{
				virtualStickSettingsView.AttachEventsForCustomResponse(virtualStickSettings2);
			}
			PropertyChangedEventHandler propertyChanged = virtualStickSettingsView.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(virtualStickSettingsView, new PropertyChangedEventArgs("SquareSettings"));
		}

		public ObservableCollection<ThumbstickSensitivity> SquarenessCollection
		{
			get
			{
				ObservableCollection<ThumbstickSensitivity> observableCollection;
				if ((observableCollection = this._squarenessCollection) == null)
				{
					observableCollection = (this._squarenessCollection = this.CreateSquareness());
				}
				return observableCollection;
			}
		}

		public ThumbstickSensitivity SquareSettings
		{
			get
			{
				if (this._combobox == null)
				{
					ColoredComboBox coloredComboBox = base.Template.FindName("SquarenessComboBox", this) as ColoredComboBox;
					if (coloredComboBox != null)
					{
						this._combobox = coloredComboBox;
					}
				}
				ColoredComboBox combobox = this._combobox;
				if (combobox != null && combobox.IsVisible)
				{
					ThumbstickSensitivity thumbstickSensitivity = this.SquarenessCollection.FirstOrDefault((ThumbstickSensitivity sens) => sens.CheckEqualsForDeflection(this.VirtualStickSettings.SquareSettings));
					if (this.VirtualStickSettings.IsSquared && (thumbstickSensitivity == null || thumbstickSensitivity.IsCustom))
					{
						thumbstickSensitivity = this.SquarenessCollection.FirstOrDefault((ThumbstickSensitivity sens) => sens.IsCustom);
						thumbstickSensitivity.Deflection = this.VirtualStickSettings.SquareSettings;
					}
					return thumbstickSensitivity;
				}
				return null;
			}
			set
			{
				if (value == this.SquareSettings)
				{
					return;
				}
				this.DetachEventsForCustomResponse(this.VirtualStickSettings);
				this.VirtualStickSettings.SetSquareSettings(value.Deflection.Clone());
				PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
				if (propertyChanged != null)
				{
					propertyChanged(this, new PropertyChangedEventArgs("SquareSettings"));
				}
				this.AttachEventsForCustomResponse(this.VirtualStickSettings);
			}
		}

		public VirtualStickSettingsView()
		{
			this.InitializeComponent();
		}

		private void OnHorizontalPointNewValueChanged(object sender, PropertyChangedEventArgs e)
		{
			App.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.IsChanged = true;
		}

		private void OnVirtualStickSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsSquared")
			{
				this.DetachEventsForCustomResponse(this.VirtualStickSettings);
				this.AttachEventsForCustomResponse(this.VirtualStickSettings);
			}
		}

		private void DetachEventsForCustomResponse(VirtualStickSettings settings)
		{
			settings.PropertyChanged -= this.OnVirtualStickSettingsPropertyChanged;
			settings.SquareSettings.HorizontalPoint[0].PropertyChanged -= this.OnHorizontalPointNewValueChanged;
			settings.SquareSettings.HorizontalPoint[1].PropertyChanged -= this.OnHorizontalPointNewValueChanged;
			settings.SquareSettings.HorizontalPoint[2].PropertyChanged -= this.OnHorizontalPointNewValueChanged;
			settings.SquareSettings.HorizontalPoint[3].PropertyChanged -= this.OnHorizontalPointNewValueChanged;
		}

		private void AttachEventsForCustomResponse(VirtualStickSettings settings)
		{
			settings.PropertyChanged -= this.OnVirtualStickSettingsPropertyChanged;
			ThumbstickSensitivity thumbstickSensitivity = this.SquarenessCollection.FirstOrDefault((ThumbstickSensitivity sens) => sens.CheckEqualsForDeflection(settings.SquareSettings));
			if (settings.IsSquared && (thumbstickSensitivity == null || thumbstickSensitivity.IsCustom))
			{
				settings.SquareSettings.HorizontalPoint[0].PropertyChanged += this.OnHorizontalPointNewValueChanged;
				settings.SquareSettings.HorizontalPoint[1].PropertyChanged += this.OnHorizontalPointNewValueChanged;
				settings.SquareSettings.HorizontalPoint[2].PropertyChanged += this.OnHorizontalPointNewValueChanged;
				settings.SquareSettings.HorizontalPoint[3].PropertyChanged += this.OnHorizontalPointNewValueChanged;
			}
		}

		private ThumbstickSensitivity CreateSoftSquareSettings()
		{
			return new ThumbstickSensitivity("Soft square", new Localizable(12710), 8300, 15700, 17700, 15700, 25920, 22282, 32768, 22282, false);
		}

		private ThumbstickSensitivity CreateTotallySquareSettings()
		{
			return new ThumbstickSensitivity("Totally square", new Localizable(12711), 8300, 32768, 17700, 32768, 25920, 32768, 32768, 32768, false);
		}

		private ThumbstickSensitivity CreateHalfSquareSettings()
		{
			return new ThumbstickSensitivity("Half-square", new Localizable(12712), 8300, 0, 17700, 0, 25920, 22282, 32768, 22282, false);
		}

		private ThumbstickSensitivity CreateDefaultSquareSettings()
		{
			return new ThumbstickSensitivity("Custom", new Localizable(11620), 8300, 0, 17700, 0, 25920, 22280, 32768, 22280, false);
		}

		protected virtual ObservableCollection<ThumbstickSensitivity> CreateSquareness()
		{
			return new ObservableCollection<ThumbstickSensitivity>
			{
				this.CreateSoftSquareSettings(),
				this.CreateTotallySquareSettings(),
				this.CreateHalfSquareSettings(),
				this.CreateDefaultSquareSettings()
			};
		}

		private void SquarenessComboBox_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
				if (propertyChanged == null)
				{
					return;
				}
				propertyChanged(this, new PropertyChangedEventArgs("SquareSettings"));
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/reWASD;component/views/virtualdevicesettings/virtualsticksettingsview.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			this._contentLoaded = true;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				((ColoredComboBox)target).IsVisibleChanged += this.SquarenessComboBox_OnIsVisibleChanged;
			}
		}

		private ObservableCollection<ThumbstickSensitivity> _squarenessCollection;

		public static readonly DependencyProperty VirtualStickSettingsProperty = DependencyProperty.Register("VirtualStickSettings", typeof(VirtualStickSettings), typeof(VirtualStickSettingsView), new PropertyMetadata(null, new PropertyChangedCallback(VirtualStickSettingsView.OnVirtualStickSettingsChanged)));

		private ColoredComboBox _combobox;

		private bool _contentLoaded;
	}
}
