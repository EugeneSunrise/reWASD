using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.Controls;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.RecolorableImages;
using Prism.Ioc;
using reWASDUI.Controls.LicenseFeatureManaging;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.ViewModels;

namespace reWASDUI.Views.ContentZoneGamepad.GamepadSelector
{
	public partial class GamepadSelectorEmptyGame : UserControl
	{
		private GamepadSelectorVM _datacontext
		{
			get
			{
				return (GamepadSelectorVM)base.DataContext;
			}
		}

		public GamepadSelectorEmptyGame()
		{
			base.DataContext = IContainerProviderExtensions.Resolve<GamepadSelectorVM>(App.Container);
			this.InitializeComponent();
			this.tbControllerName.IsVisibleChanged += this.TbControllerNameOnIsVisibleChanged;
			App.EventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM a)
			{
				this.RecolorJoyCons();
			});
		}

		private void SvgContainer_OnSVGRendered(object sender, EventArgs e)
		{
			try
			{
				RecolorableSVG recolorableSVG = this.svgContainer;
				DrawingImage drawingImage = (DrawingImage)((recolorableSVG != null) ? recolorableSVG.Source : null);
				Drawing drawing = ((drawingImage != null) ? drawingImage.GetItemByName("WizardButton") : null);
				if (drawing != null)
				{
					drawing.ChangeColor(Colors.Transparent, null, true, true, true);
				}
				this.RecolorJoyCons();
			}
			catch (Exception)
			{
			}
		}

		private void RecolorJoyCons()
		{
			RecolorableSVG recolorableSVG = this.svgContainer;
			DrawingImage drawingImage = (DrawingImage)((recolorableSVG != null) ? recolorableSVG.Source : null);
			BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
			if (currentGamepad != null && (currentGamepad.IsNintendoSwitchJoyConComposite || currentGamepad.IsNintendoSwitchJoyConL || currentGamepad.IsNintendoSwitchJoyConR))
			{
				if (currentGamepad.IsNintendoSwitchJoyConL && !currentGamepad.IsNintendoSwitchJoyConComposite)
				{
					if (drawingImage != null)
					{
						drawingImage.SetOpacityForGroupByName("gamepadR", 0.5);
					}
				}
				else if (drawingImage != null)
				{
					drawingImage.SetOpacityForGroupByName("gamepadR", 1.0);
				}
				if (currentGamepad.IsNintendoSwitchJoyConR && !currentGamepad.IsNintendoSwitchJoyConComposite)
				{
					if (drawingImage != null)
					{
						drawingImage.SetOpacityForGroupByName("gamepadL", 0.5);
					}
				}
				else if (drawingImage != null)
				{
					drawingImage.SetOpacityForGroupByName("gamepadL", 1.0);
				}
			}
			if (currentGamepad != null && currentGamepad.IsGyroscopePresent)
			{
				if (drawingImage != null)
				{
					drawingImage.SetOpacityForGroupByName("Gyro", 1.0);
					return;
				}
			}
			else if (drawingImage != null)
			{
				drawingImage.SetOpacityForGroupByName("Gyro", 0.0);
			}
		}

		private void TbControllerNameOnIsVisibleChanged(object s, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue is bool && (bool)e.NewValue)
			{
				this.tbControllerName.Focus();
				this.tbControllerName.CaretIndex = this.tbControllerName.Text.Length;
				this.tbControllerName.SelectAll();
			}
		}

		private void tbControllerNameLostFocus(object sender, RoutedEventArgs e)
		{
			FrameworkElement frameworkElement = Keyboard.FocusedElement as FrameworkElement;
			if (((frameworkElement != null) ? frameworkElement.Name : null) != "imgbtnEdit")
			{
				GamepadSelectorVM datacontext = this._datacontext;
				if (datacontext == null)
				{
					return;
				}
				datacontext.TBControllerNameApplyChanges();
			}
		}

		private void TbControllerName_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				this._datacontext.TBControllerNameApplyChanges();
			}
			if (e.Key == Key.Escape)
			{
				this._datacontext.TBControllerNameRevertChanges();
			}
		}

		private const string JoyConLeftGroup = "gamepadL";

		private const string JoyConRightGroup = "gamepadR";

		private const string GamepadGyro = "Gyro";
	}
}
