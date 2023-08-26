using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.SVGPositioningControls;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Services;
using reWASDUI.ViewModels;

namespace reWASDUI.Views
{
	public class SVGGamepadControlsRecolorer : BaseSVGPositioning
	{
		public bool IsJoyConLeftPresent
		{
			get
			{
				return (bool)base.GetValue(SVGGamepadControlsRecolorer.IsJoyConLeftPresentProperty);
			}
			set
			{
				base.SetValue(SVGGamepadControlsRecolorer.IsJoyConLeftPresentProperty, value);
			}
		}

		private static void IsJoyConLeftPresentChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SVGGamepadControlsRecolorer svggamepadControlsRecolorer = d as SVGGamepadControlsRecolorer;
			if (svggamepadControlsRecolorer == null)
			{
				return;
			}
			svggamepadControlsRecolorer.OnIsJoyConLeftPresentChanged();
		}

		public bool IsJoyConRightPresent
		{
			get
			{
				return (bool)base.GetValue(SVGGamepadControlsRecolorer.IsJoyConRightPresentProperty);
			}
			set
			{
				base.SetValue(SVGGamepadControlsRecolorer.IsJoyConRightPresentProperty, value);
			}
		}

		private static void IsJoyConRightPresentChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SVGGamepadControlsRecolorer svggamepadControlsRecolorer = d as SVGGamepadControlsRecolorer;
			if (svggamepadControlsRecolorer == null)
			{
				return;
			}
			svggamepadControlsRecolorer.OnIsJoyConRightPresentChanged();
		}

		public bool IsGyroScopeAvailiable
		{
			get
			{
				return (bool)base.GetValue(SVGGamepadControlsRecolorer.IsGyroScopeAvailiableProperty);
			}
			set
			{
				base.SetValue(SVGGamepadControlsRecolorer.IsGyroScopeAvailiableProperty, value);
			}
		}

		private static void IsGyroScopeAvailiableChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SVGGamepadControlsRecolorer svggamepadControlsRecolorer = d as SVGGamepadControlsRecolorer;
			if (svggamepadControlsRecolorer == null)
			{
				return;
			}
			svggamepadControlsRecolorer.OnIsGyroScopeAvailiableChanged(e);
		}

		public bool IsWizardButtonVisible
		{
			get
			{
				return (bool)base.GetValue(SVGGamepadControlsRecolorer.IsWizardButtonVisibleProperty);
			}
			set
			{
				base.SetValue(SVGGamepadControlsRecolorer.IsWizardButtonVisibleProperty, value);
			}
		}

		private static void IsWizardButtonVisibleChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SVGGamepadControlsRecolorer svggamepadControlsRecolorer = d as SVGGamepadControlsRecolorer;
			if (svggamepadControlsRecolorer == null)
			{
				return;
			}
			svggamepadControlsRecolorer.OnIsWizardButtonVisibleChanged(e);
		}

		public SVGContainerGrid SVGContainerGrid
		{
			get
			{
				return (SVGContainerGrid)base.GetValue(SVGGamepadControlsRecolorer.SVGContainerGridProperty);
			}
			set
			{
				base.SetValue(SVGGamepadControlsRecolorer.SVGContainerGridProperty, value);
			}
		}

		private GamepadSelectorVM _dataContext
		{
			get
			{
				return base.DataContext as GamepadSelectorVM;
			}
		}

		public SVGGamepadControlsRecolorer()
		{
			base.Loaded += this.OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			if (!this._isFirstInited)
			{
				this._isFirstInited = true;
				base.SVGContainer.OnSVGRendered += this.SVGOnOnSVGRendered;
				this.SVGOnOnSVGRendered(null, null);
			}
			this.SetCurrentGamepadBindings();
			App.EventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM i)
			{
				this.SetCurrentGamepadBindings();
			});
		}

		private void SetCurrentGamepadBindings()
		{
			GamepadSelectorVM dataContext = this._dataContext;
			bool flag;
			if (dataContext == null)
			{
				flag = false;
			}
			else
			{
				GamepadService gamepadService = dataContext.GamepadService;
				bool? flag2;
				if (gamepadService == null)
				{
					flag2 = null;
				}
				else
				{
					BaseControllerVM currentGamepad = gamepadService.CurrentGamepad;
					flag2 = ((currentGamepad != null) ? new bool?(currentGamepad.IsNintendoSwitchJoyConComposite) : null);
				}
				bool? flag3 = flag2;
				bool flag4 = true;
				flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
			}
			if (flag)
			{
				BindingOperations.SetBinding(this, SVGGamepadControlsRecolorer.IsJoyConLeftPresentProperty, new Binding("GamepadService.CurrentGamepad.IsNintendoSwitchJoyConL"));
				BindingOperations.SetBinding(this, SVGGamepadControlsRecolorer.IsJoyConRightPresentProperty, new Binding("GamepadService.CurrentGamepad.IsNintendoSwitchJoyConR"));
				return;
			}
			BindingOperations.SetBinding(this, SVGGamepadControlsRecolorer.IsJoyConLeftPresentProperty, new Binding("GamepadService.CurrentGamepad.CurrentController.IsNintendoSwitchJoyConL"));
			BindingOperations.SetBinding(this, SVGGamepadControlsRecolorer.IsJoyConRightPresentProperty, new Binding("GamepadService.CurrentGamepad.CurrentController.IsNintendoSwitchJoyConR"));
		}

		private void SVGOnOnSVGRendered(object sender, EventArgs eventArgs)
		{
			this.ResetStates();
			this.ReEvaluateState();
		}

		protected override void SetPositioningBinding()
		{
		}

		protected override void OnDrawingImageChanged()
		{
		}

		private void ReEvaluateState()
		{
			if (!this.IsJoyConLeftPresent && !this.IsJoyConRightPresent)
			{
				this.RestoreInitialColorR();
				this.RestoreInitialColorL();
			}
			else
			{
				if (this.IsJoyConLeftPresent)
				{
					this.RestoreInitialColorL();
				}
				else
				{
					this.HighlightRecolorL();
				}
				if (this.IsJoyConRightPresent)
				{
					this.RestoreInitialColorR();
				}
				else
				{
					this.HighlightRecolorR();
				}
			}
			DrawingImage svgdrawingImage = base.SVGDrawingImage;
			if (svgdrawingImage == null)
			{
				return;
			}
			svgdrawingImage.SetOpacityForGroupByName("Gyro", (double)(this.IsGyroScopeAvailiable ? 1 : 0));
		}

		private void HighlightRecolorL()
		{
			if (base.SVGDrawingImage == null || this._isRecoloredL)
			{
				return;
			}
			base.SVGDrawingImage.SetOpacityForGroupByName("gamepadL", 0.6);
			this._isRecoloredL = true;
		}

		private void HighlightRecolorR()
		{
			if (base.SVGDrawingImage == null || this._isRecoloredR)
			{
				return;
			}
			base.SVGDrawingImage.SetOpacityForGroupByName("gamepadR", 0.6);
			this._isRecoloredR = true;
		}

		public void RestoreInitialColorL()
		{
			if (this._isRecoloredL)
			{
				DrawingImage svgdrawingImage = base.SVGDrawingImage;
				if (svgdrawingImage != null)
				{
					svgdrawingImage.SetOpacityForGroupByName("gamepadL", 1.0);
				}
				this._isRecoloredL = false;
			}
		}

		public void RestoreInitialColorR()
		{
			if (this._isRecoloredR)
			{
				DrawingImage svgdrawingImage = base.SVGDrawingImage;
				if (svgdrawingImage != null)
				{
					svgdrawingImage.SetOpacityForGroupByName("gamepadR", 1.0);
				}
				this._isRecoloredR = false;
			}
		}

		private void OnIsJoyConLeftPresentChanged()
		{
			this.ReEvaluateState();
		}

		private void OnIsJoyConRightPresentChanged()
		{
			this.ReEvaluateState();
		}

		private void OnIsGyroScopeAvailiableChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ReEvaluateState();
		}

		private async void OnIsWizardButtonVisibleChanged(DependencyPropertyChangedEventArgs e)
		{
			await Task.Delay(10);
			DrawingImage svgdrawingImage = base.SVGDrawingImage;
			if (svgdrawingImage != null)
			{
				svgdrawingImage.SetOpacityForGroupByName("WizardButton", (double)(this.IsWizardButtonVisible ? 1 : 0));
			}
		}

		private void ResetStates()
		{
			this._isRecoloredL = false;
			this._isRecoloredR = false;
		}

		public static readonly DependencyProperty IsJoyConLeftPresentProperty = DependencyProperty.Register("IsJoyConLeftPresent", typeof(bool), typeof(SVGGamepadControlsRecolorer), new PropertyMetadata(false, new PropertyChangedCallback(SVGGamepadControlsRecolorer.IsJoyConLeftPresentChangedCallback)));

		public static readonly DependencyProperty IsJoyConRightPresentProperty = DependencyProperty.Register("IsJoyConRightPresent", typeof(bool), typeof(SVGGamepadControlsRecolorer), new PropertyMetadata(false, new PropertyChangedCallback(SVGGamepadControlsRecolorer.IsJoyConRightPresentChangedCallback)));

		public static readonly DependencyProperty IsGyroScopeAvailiableProperty = DependencyProperty.Register("IsGyroScopeAvailiable", typeof(bool), typeof(SVGGamepadControlsRecolorer), new PropertyMetadata(false, new PropertyChangedCallback(SVGGamepadControlsRecolorer.IsGyroScopeAvailiableChangedCallback)));

		public static readonly DependencyProperty IsWizardButtonVisibleProperty = DependencyProperty.Register("IsWizardButtonVisible", typeof(bool), typeof(SVGGamepadControlsRecolorer), new PropertyMetadata(false, new PropertyChangedCallback(SVGGamepadControlsRecolorer.IsWizardButtonVisibleChangedCallback)));

		public static readonly DependencyProperty SVGContainerGridProperty = DependencyProperty.Register("SVGContainerGrid", typeof(SVGContainerGrid), typeof(SVGGamepadControlsRecolorer), new PropertyMetadata(null));

		private const string JoyConLeftGroup = "gamepadL";

		private const string JoyConRightGroup = "gamepadR";

		private const string GamepadGyro = "Gyro";

		private const string WizardButton = "WizardButton";

		private bool _isFirstInited;

		private bool _isRecoloredL;

		private bool _isRecoloredR;
	}
}
