using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View;
using DiscSoft.NET.Common.View.SVGPositioningControls;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Controls.XBBindingControls.BindingAnnotation;
using reWASDUI.Utils.Converters;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;

namespace reWASDUI.Views
{
	public partial class SVGGamepadWithAllAnnotations : UserControl
	{
		public SVGGamepadWithAllAnnotations()
		{
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
			base.IsVisibleChanged += this.SVGGamepadWithAllAnnotations_IsVisibleChanged;
		}

		private async void SVGGamepadWithAllAnnotations_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				await this.InitContainerGrid();
				this.UpdateAttachedButtonsPos();
			}
		}

		private async void OnLoaded(object sender, RoutedEventArgs e)
		{
			await this.InitContainerGrid();
			base.UpdateLayout();
		}

		private void UpdateAttachedButtonsPos()
		{
			if (this._svgContainerGrid == null)
			{
				return;
			}
			foreach (object obj in this._svgContainerGrid.Children)
			{
				SVGElementAttachedButton svgelementAttachedButton = obj as SVGElementAttachedButton;
				if (svgelementAttachedButton != null)
				{
					svgelementAttachedButton.AttachToSVGElement();
				}
			}
		}

		private async Task WaitForContainer()
		{
			if (this._svgContainerGrid == null)
			{
				this._svgContainerGrid = VisualTreeHelperExt.FindChild<SVGContainerGrid>(this, null);
			}
			if (this._svgContainerGrid == null)
			{
				await Task.Delay(50);
				this._svgContainerGrid = base.Template.FindName("svgContainerGrid", this) as SVGContainerGrid;
			}
			if (this._svgContainerGrid == null)
			{
				await Task.Delay(200);
				this._svgContainerGrid = base.Template.FindName("svgContainerGrid", this) as SVGContainerGrid;
			}
		}

		private async Task InitContainerGrid()
		{
			if (!this._firstLoadInited)
			{
				await this.WaitForContainer();
				if (this._svgContainerGrid != null)
				{
					if (!this._firstLoadInited)
					{
						this._firstLoadInited = true;
						foreach (GamepadButton gamepadButton in this.ListOfButtonsToGenerateControlsFor)
						{
							SVGAnchorContainer svganchorContainer = new SVGAnchorContainer();
							svganchorContainer.Name = gamepadButton.ToString() + "Anchor";
							svganchorContainer.SetBinding(BaseSVGPositioning.SVGElementNameProperty, new Binding
							{
								Converter = new ControllerButtonToAnchorNameConverter(),
								ConverterParameter = gamepadButton
							});
							Grid.SetColumnSpan(svganchorContainer, 3);
							Grid.SetRowSpan(svganchorContainer, 3);
							svganchorContainer.Content = new GamepadBindingAnnotation
							{
								AnnotatedButton = new GamepadButton?(gamepadButton)
							};
							this._svgContainerGrid.Children.Insert(10, svganchorContainer);
							SVGElementAttachedButton svgelementAttachedButton = new SVGElementAttachedButton();
							svgelementAttachedButton.SetBinding(ButtonBase.CommandProperty, new Binding("GameProfilesService.ChangeCurrentBindingCommand"));
							svgelementAttachedButton.CommandParameter = gamepadButton;
							svgelementAttachedButton.HorizontalAlignment = base.HorizontalContentAlignment;
							svgelementAttachedButton.VerticalAlignment = base.VerticalContentAlignment;
							svgelementAttachedButton.SetBinding(BaseSVGPositioning.SVGElementNameProperty, new Binding
							{
								Converter = new ControllerButtonToAnchorNameConverter(),
								ConverterParameter = gamepadButton
							});
							svgelementAttachedButton.SetBinding(SVGElementAttachedButton.HighlightBrushProperty, new Binding("GameProfilesService.CurrentGamepadBindingCollection.CollectionBrush")
							{
								FallbackValue = Application.Current.TryFindResource("CreamBrush")
							});
							Grid.SetColumn(svgelementAttachedButton, 1);
							Grid.SetRow(svgelementAttachedButton, 1);
							this._svgContainerGrid.Children.Insert(10, svgelementAttachedButton);
						}
						foreach (KeyScanCodeV2 keyScanCodeV in this.ListOfFakeButtonsToGenerateControlsFor)
						{
							SVGElementFakeAttachedButton svgelementFakeAttachedButton = new SVGElementFakeAttachedButton();
							svgelementFakeAttachedButton.HorizontalAlignment = base.HorizontalContentAlignment;
							svgelementFakeAttachedButton.VerticalAlignment = base.VerticalContentAlignment;
							svgelementFakeAttachedButton.HighlightBrush = new SolidColorBrush(Colors.White);
							svgelementFakeAttachedButton.SetBinding(BaseSVGPositioning.SVGElementNameProperty, new Binding
							{
								Converter = new ControllerButtonToAnchorNameConverter(),
								ConverterParameter = keyScanCodeV
							});
							Grid.SetColumn(svgelementFakeAttachedButton, 1);
							Grid.SetRow(svgelementFakeAttachedButton, 1);
							this._svgContainerGrid.Children.Insert(10, svgelementFakeAttachedButton);
						}
						this._svgContainerGrid.SetBindings(true);
					}
				}
			}
		}

		private bool _firstLoadInited;

		private SVGContainerGrid _svgContainerGrid;

		private readonly List<GamepadButton> ListOfButtonsToGenerateControlsFor = new List<GamepadButton>
		{
			7, 8, 1, 2, 3, 4, 5, 6, 11, 13,
			9, 10, 12, 51, 169, 55, 170, 29, 30, 31,
			32, 33, 34, 35, 36, 40, 41, 42, 43, 44,
			47, 48, 49, 50, 44, 216, 217, 218, 219, 68,
			69, 70, 71, 99, 163, 91, 92, 93, 94, 100,
			101, 102, 103, 108, 164, 236, 237, 238, 239, 240,
			241, 14, 15, 16, 17, 18, 19, 20, 21, 22,
			23, 24, 25, 26, 27, 64, 65, 66, 67, 171,
			172
		};

		private readonly List<KeyScanCodeV2> ListOfFakeButtonsToGenerateControlsFor = new List<KeyScanCodeV2>
		{
			KeyScanCodeV2.VolDown,
			KeyScanCodeV2.VolUp,
			KeyScanCodeV2.PlayPause,
			KeyScanCodeV2.PrevTrack,
			KeyScanCodeV2.NextTrack,
			KeyScanCodeV2.DikWebBack,
			KeyScanCodeV2.DikWebHome
		};
	}
}
