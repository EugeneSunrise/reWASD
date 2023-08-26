using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View;
using DiscSoft.NET.Common.View.RecolorableImages;
using DiscSoft.NET.Common.View.SVGPositioningControls;
using reWASDUI.Controls.XBBindingControls;
using reWASDUI.Controls.XBBindingControls.BindingAnnotation;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Utils.Converters;
using reWASDUI.Utils.XBUtil;

namespace reWASDUI.Views
{
	public class SVGContainerGrid : Grid
	{
		private RecolorableSVG SVGContainer
		{
			get
			{
				return VisualTreeHelperExt.FindChild<RecolorableSVG>(this, "svgContainer");
			}
		}

		private static void CentralCellAsteriskCoefficientChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as SVGContainerGrid).SetCentralCellAsteriskCoefficient();
		}

		public double CentralCellAsteriskCoefficient
		{
			get
			{
				return (double)base.GetValue(SVGContainerGrid.CentralCellAsteriskCoefficientProperty);
			}
			set
			{
				base.SetValue(SVGContainerGrid.CentralCellAsteriskCoefficientProperty, value);
			}
		}

		public SVGContainerGrid()
		{
			base.Loaded += this.RootGrid_OnLoaded;
		}

		private void SetCentralCellAsteriskCoefficient()
		{
			if (base.ColumnDefinitions.Count == 3)
			{
				base.ColumnDefinitions[0].Width = new GridLength(1.0, GridUnitType.Star);
				base.ColumnDefinitions[1].Width = new GridLength(this.CentralCellAsteriskCoefficient, GridUnitType.Star);
				base.ColumnDefinitions[2].Width = new GridLength(1.0, GridUnitType.Star);
			}
			if (base.RowDefinitions.Count == 3)
			{
				base.RowDefinitions[0].Height = new GridLength(1.0, GridUnitType.Star);
				base.RowDefinitions[1].Height = new GridLength(this.CentralCellAsteriskCoefficient, GridUnitType.Star);
				base.RowDefinitions[0].Height = new GridLength(1.0, GridUnitType.Star);
			}
		}

		private void RootGrid_OnLoaded(object sender, RoutedEventArgs e)
		{
			if (this._loaded)
			{
				return;
			}
			this._loaded = true;
			this.SetCentralCellAsteriskCoefficient();
			this.SetBindings(false);
		}

		private SVGElementAttachedButton GetSVGAttacheButtonByName(string name, string SVGElementName)
		{
			if (name.Length > 6 && name.Substring(name.Length - 6, 6) == "Anchor")
			{
				name = name.Substring(0, name.Length - 6);
			}
			else
			{
				AssociatedControllerButton associatedControllerButton = XBUtils.ConvertAnchorStringToGamepadButton(SVGElementName);
				if (associatedControllerButton.IsGamepad)
				{
					name = associatedControllerButton.GamepadButton.ToString();
				}
				else if (associatedControllerButton.KeyScanCode.IsMouseDigital)
				{
					name = XBUtils.ConvertAnchorStringToMouseButton(SVGElementName).ToString();
				}
			}
			foreach (object obj in base.Children)
			{
				SVGElementAttachedButton svgelementAttachedButton = obj as SVGElementAttachedButton;
				if (svgelementAttachedButton != null && svgelementAttachedButton.CommandParameter != null && svgelementAttachedButton.CommandParameter.ToString() == name)
				{
					return svgelementAttachedButton;
				}
			}
			return null;
		}

		public void SetBindings(bool forceRedraw = false)
		{
			RecolorableSVG svgcontainer = this.SVGContainer;
			foreach (object obj in base.Children)
			{
				BaseSVGPositioning baseSVGPositioning = obj as BaseSVGPositioning;
				SVGAnchorContainer svganchorContainer = obj as SVGAnchorContainer;
				SVGElementAttachedButton svgelementAttachedButton = obj as SVGElementAttachedButton;
				if (baseSVGPositioning != null)
				{
					BindingOperations.SetBinding(baseSVGPositioning, BaseSVGPositioning.RootContainerProperty, new Binding
					{
						Source = this
					});
					BindingOperations.SetBinding(baseSVGPositioning, BaseSVGPositioning.SVGContainerProperty, new Binding
					{
						Source = svgcontainer
					});
					BindingOperations.SetBinding(baseSVGPositioning, BaseSVGPositioning.AttachedImageProperty, new Binding("Image")
					{
						Source = svgcontainer
					});
					BindingOperations.SetBinding(baseSVGPositioning, BaseSVGPositioning.SVGDrawingImageProperty, new Binding("Source")
					{
						Source = svgcontainer,
						Converter = new ImageSourceToDrawingImageConverter()
					});
					baseSVGPositioning.CentralCellAsteriskCoefficient = this.CentralCellAsteriskCoefficient;
					if (forceRedraw)
					{
						baseSVGPositioning.AttachToSVGElement();
					}
				}
				if (svgelementAttachedButton != null)
				{
					MultiBinding multiBinding = new MultiBinding();
					multiBinding.Converter = new GamepadButtonToSVGAttechedElementIsCurrentBindingConverter();
					multiBinding.FallbackValue = false;
					multiBinding.Bindings.Add(new Binding("SVGElementName")
					{
						RelativeSource = RelativeSource.Self,
						Converter = new AnchorNameToControllerButtonConverter()
					});
					multiBinding.Bindings.Add(new Binding("GameProfilesService.RealCurrentBeingMappedBindingCollection.CurrentControllerButton")
					{
						Source = base.DataContext
					});
					BindingOperations.SetBinding(svgelementAttachedButton, SVGElementAttachedButton.IsCurrentBindingProperty, multiBinding);
					if (forceRedraw)
					{
						svgelementAttachedButton.AttachToSVGElement();
					}
				}
				if (svganchorContainer != null)
				{
					GamepadBindingAnnotation gamepadBindingAnnotation = svganchorContainer.Content as GamepadBindingAnnotation;
					if (gamepadBindingAnnotation != null)
					{
						BindingOperations.SetBinding(gamepadBindingAnnotation, GamepadBindingAnnotation.SVGDrawingImageProperty, new Binding("Source")
						{
							Source = svgcontainer,
							Converter = new ImageSourceToDrawingImageConverter()
						});
						BindingOperations.SetBinding(gamepadBindingAnnotation, GamepadBindingAnnotation.SVGContainerProperty, new Binding
						{
							Source = svgcontainer
						});
						string text = svganchorContainer.SVGElementName;
						if (text == "DownMouse" || text == "LeftMouse" || text == "RightMouse")
						{
							text = "UpMouse";
						}
						SVGElementAttachedButton svgattacheButtonByName = this.GetSVGAttacheButtonByName(svganchorContainer.Name, text);
						if (svgattacheButtonByName != null)
						{
							BindingOperations.SetBinding(gamepadBindingAnnotation, BaseXBBindingUserControl.IsHoveredProperty, new Binding
							{
								Source = svgattacheButtonByName,
								Path = new PropertyPath("IsRecolored", Array.Empty<object>())
							});
						}
					}
					if (forceRedraw)
					{
						svganchorContainer.AttachToSVGElement();
					}
				}
			}
		}

		private bool _loaded;

		public static readonly DependencyProperty CentralCellAsteriskCoefficientProperty = DependencyProperty.Register("CentralCellAsteriskCoefficient", typeof(double), typeof(SVGContainerGrid), new PropertyMetadata(3.0, new PropertyChangedCallback(SVGContainerGrid.CentralCellAsteriskCoefficientChangedCallback)));
	}
}
