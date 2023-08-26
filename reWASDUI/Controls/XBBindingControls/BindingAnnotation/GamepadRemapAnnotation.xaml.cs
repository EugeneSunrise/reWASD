using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.RecolorableImages;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;

namespace reWASDUI.Controls.XBBindingControls.BindingAnnotation
{
	public partial class GamepadRemapAnnotation : BaseXBBindingAnnotation
	{
		public bool ShowHardwareMapping
		{
			get
			{
				return (bool)base.GetValue(GamepadRemapAnnotation.ShowHardwareMappingProperty);
			}
			set
			{
				base.SetValue(GamepadRemapAnnotation.ShowHardwareMappingProperty, value);
			}
		}

		private RecolorableSVG _remapRecolorableSVG
		{
			get
			{
				return base.Template.FindName("imgRemapedToAnnotationIco", this) as RecolorableSVG;
			}
		}

		public GamepadRemapAnnotation()
		{
			this.InitializeComponent();
			App.EventAggregator.GetEvent<CurrentGamepadCurrentChanged>().Subscribe(new Action<ControllerVM>(this.OnCurrentGamepadChanged));
		}

		private void OnCurrentGamepadChanged(ControllerVM obj)
		{
			RecolorableSVG remapRecolorableSVG = this._remapRecolorableSVG;
			if (remapRecolorableSVG == null)
			{
				return;
			}
			BindingExpression bindingExpression = remapRecolorableSVG.GetBindingExpression(RecolorableSVG.DrawingProperty);
			if (bindingExpression == null)
			{
				return;
			}
			bindingExpression.UpdateTarget();
		}

		protected override void ReEvaluateXBBinding()
		{
		}

		protected override void ReEvaluateVisibility()
		{
			MultiBindingExpression multiBindingExpression = BindingOperations.GetMultiBindingExpression(this, UIElement.VisibilityProperty);
			if (multiBindingExpression == null)
			{
				return;
			}
			multiBindingExpression.UpdateTarget();
		}

		protected override void ReEvaluateIsCurrentBinding()
		{
		}

		public static readonly DependencyProperty ShowHardwareMappingProperty = DependencyProperty.Register("ShowHardwareMapping", typeof(bool), typeof(GamepadRemapAnnotation), new PropertyMetadata(true));
	}
}
