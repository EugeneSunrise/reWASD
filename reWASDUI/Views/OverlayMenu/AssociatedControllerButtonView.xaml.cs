using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings;

namespace reWASDUI.Views.OverlayMenu
{
	public partial class AssociatedControllerButtonView : UserControl
	{
		public AssociatedControllerButton AssociatedControllerButton
		{
			get
			{
				return (AssociatedControllerButton)base.GetValue(AssociatedControllerButtonView.AssociatedControllerButtonProperty);
			}
			set
			{
				base.SetValue(AssociatedControllerButtonView.AssociatedControllerButtonProperty, value);
			}
		}

		public ControllerTypeEnum ControllerType
		{
			get
			{
				return (ControllerTypeEnum)base.GetValue(AssociatedControllerButtonView.ControllerTypeProperty);
			}
			set
			{
				base.SetValue(AssociatedControllerButtonView.ControllerTypeProperty, value);
			}
		}

		public bool IsShowDescription
		{
			get
			{
				return (bool)base.GetValue(AssociatedControllerButtonView.IsShowDescriptionProperty);
			}
			set
			{
				base.SetValue(AssociatedControllerButtonView.IsShowDescriptionProperty, value);
			}
		}

		public bool IsShowNotSelected
		{
			get
			{
				return (bool)base.GetValue(AssociatedControllerButtonView.IsShowNotSelectedProperty);
			}
			set
			{
				base.SetValue(AssociatedControllerButtonView.IsShowNotSelectedProperty, value);
			}
		}

		public AssociatedControllerButtonView()
		{
			this.InitializeComponent();
		}

		public static readonly DependencyProperty AssociatedControllerButtonProperty = DependencyProperty.Register("AssociatedControllerButton", typeof(AssociatedControllerButton), typeof(AssociatedControllerButtonView), new PropertyMetadata(null));

		public static readonly DependencyProperty ControllerTypeProperty = DependencyProperty.Register("ControllerType", typeof(ControllerTypeEnum), typeof(AssociatedControllerButtonView), new PropertyMetadata(0));

		public static readonly DependencyProperty IsShowDescriptionProperty = DependencyProperty.Register("IsShowDescription", typeof(bool), typeof(AssociatedControllerButtonView), new PropertyMetadata(true));

		public static readonly DependencyProperty IsShowNotSelectedProperty = DependencyProperty.Register("IsShowNotSelected", typeof(bool), typeof(AssociatedControllerButtonView), new PropertyMetadata(false));
	}
}
