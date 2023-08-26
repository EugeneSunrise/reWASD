using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;

namespace reWASDUI.Controls
{
	public class ControllerNamePanel : WrapPanel
	{
		public ControllerNamePanel()
		{
			base.Loaded += delegate(object s, RoutedEventArgs e)
			{
				this.CreatePanel();
			};
			App.EventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM e)
			{
				this.CreatePanel();
			});
		}

		public void CreatePanel()
		{
			BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
			if (currentGamepad != null)
			{
				base.Children.Clear();
				Style style = new Style
				{
					TargetType = typeof(TextBlock),
					Setters = 
					{
						new Setter(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Bottom),
						new Setter(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center),
						new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap),
						new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(Colors.White)),
						new Setter(TextBlock.FontSizeProperty, 20.0),
						new Setter(TextBlock.FontWeightProperty, FontWeights.SemiBold),
						new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center)
					},
					Triggers = 
					{
						new Trigger
						{
							Property = UIElement.IsEnabledProperty,
							Value = false,
							Setters = 
							{
								new Setter(UIElement.OpacityProperty, 0.5)
							}
						}
					}
				};
				base.Orientation = Orientation.Horizontal;
				base.HorizontalAlignment = HorizontalAlignment.Center;
				base.Margin = new Thickness(30.0, 0.0, 30.0, 0.0);
				if (currentGamepad.IsCompositeDevice)
				{
					CompositeControllerVM compositeControllerVM = currentGamepad as CompositeControllerVM;
					if (compositeControllerVM == null)
					{
						return;
					}
					if (!string.IsNullOrEmpty(compositeControllerVM.ControllerFriendlyName))
					{
						TextBlock textBlock = new TextBlock
						{
							Style = style,
							Name = "tblockControllerName"
						};
						textBlock.SetBinding(UIElement.IsEnabledProperty, new Binding("IsOnline")
						{
							Source = compositeControllerVM,
							Mode = BindingMode.OneWay
						});
						textBlock.SetBinding(TextBlock.TextProperty, new Binding("ControllerFriendlyName")
						{
							Source = compositeControllerVM,
							Mode = BindingMode.OneWay
						});
						textBlock.SetValue(AutomationProperties.AutomationIdProperty, textBlock.Name);
						base.Children.Add(textBlock);
						return;
					}
					int num = 0;
					for (;;)
					{
						int num2 = num;
						int? num3 = ((compositeControllerVM != null) ? new int?(compositeControllerVM.NonNullBaseControllers.Count) : null);
						if (!((num2 < num3.GetValueOrDefault()) & (num3 != null)))
						{
							break;
						}
						BaseControllerVM baseControllerVM = ((compositeControllerVM != null) ? compositeControllerVM.NonNullBaseControllers[num] : null);
						TextBlock textBlock2 = new TextBlock();
						textBlock2.Style = style;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
						defaultInterpolatedStringHandler.AppendLiteral("tblockControllerName");
						defaultInterpolatedStringHandler.AppendFormatted<int>(num + 1);
						textBlock2.Name = defaultInterpolatedStringHandler.ToStringAndClear();
						TextBlock textBlock3 = textBlock2;
						textBlock3.SetBinding(UIElement.IsEnabledProperty, new Binding("IsOnline")
						{
							Source = baseControllerVM,
							Mode = BindingMode.OneWay
						});
						textBlock3.SetBinding(TextBlock.TextProperty, new Binding("ControllerTypeFriendlyName")
						{
							Source = baseControllerVM,
							Mode = BindingMode.OneWay
						});
						textBlock3.SetValue(AutomationProperties.AutomationIdProperty, textBlock3.Name);
						base.Children.Add(textBlock3);
						baseControllerVM.UpdateGuiProperties();
						int num4 = num;
						num3 = ((compositeControllerVM != null) ? new int?(compositeControllerVM.NonNullBaseControllers.Count - 1) : null);
						if (!((num4 == num3.GetValueOrDefault()) & (num3 != null)))
						{
							TextBlock textBlock4 = new TextBlock();
							textBlock4.Text = " + ";
							textBlock4.Style = style;
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
							defaultInterpolatedStringHandler.AppendLiteral("tblockPlus");
							defaultInterpolatedStringHandler.AppendFormatted<int>(num + 1);
							textBlock4.Name = defaultInterpolatedStringHandler.ToStringAndClear();
							TextBlock textBlock5 = textBlock4;
							base.Children.Add(textBlock5);
							textBlock5.SetValue(AutomationProperties.AutomationIdProperty, textBlock5.Name);
						}
						num++;
					}
					return;
				}
				else
				{
					TextBlock textBlock6 = new TextBlock
					{
						Style = style,
						Name = "tblockControllerName"
					};
					textBlock6.SetBinding(UIElement.IsEnabledProperty, new Binding("IsOnline")
					{
						Source = currentGamepad,
						Mode = BindingMode.OneWay
					});
					textBlock6.SetBinding(TextBlock.TextProperty, new Binding("ControllerDisplayName")
					{
						Source = currentGamepad,
						Mode = BindingMode.OneWay
					});
					textBlock6.SetValue(AutomationProperties.AutomationIdProperty, textBlock6.Name);
					base.Children.Add(textBlock6);
				}
			}
		}
	}
}
