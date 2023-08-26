using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure.KeyBindingsModel.Mask;

namespace reWASDEngine.OverlayAPI.RemapWindow
{
	public partial class MaskConditionView : UserControl
	{
		public ControllerTypeEnum? ControllerType
		{
			get
			{
				return (ControllerTypeEnum?)base.GetValue(MaskConditionView.ControllerTypeProperty);
			}
			set
			{
				base.SetValue(MaskConditionView.ControllerTypeProperty, value);
			}
		}

		public MaskItemConditionCollection MaskCondition
		{
			get
			{
				return (MaskItemConditionCollection)base.GetValue(MaskConditionView.MaskConditionProperty);
			}
			set
			{
				base.SetValue(MaskConditionView.MaskConditionProperty, value);
			}
		}

		public MaskConditionView()
		{
			this.InitializeComponent();
		}

		public static readonly DependencyProperty ControllerTypeProperty = DependencyProperty.Register("ControllerType", typeof(ControllerTypeEnum?), typeof(MaskConditionView), new PropertyMetadata(null));

		public static readonly DependencyProperty MaskConditionProperty = DependencyProperty.Register("MaskCondition", typeof(MaskItemConditionCollection), typeof(MaskConditionView), new PropertyMetadata(null));
	}
}
