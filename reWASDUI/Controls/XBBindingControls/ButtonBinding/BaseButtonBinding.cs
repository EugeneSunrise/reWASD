using System;
using System.Windows;
using System.Windows.Media;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Services;

namespace reWASDUI.Controls.XBBindingControls.ButtonBinding
{
	public abstract class BaseButtonBinding : BaseXBBindingUserControl
	{
		public string Title
		{
			get
			{
				return (string)base.GetValue(BaseButtonBinding.TitleProperty);
			}
			set
			{
				base.SetValue(BaseButtonBinding.TitleProperty, value);
			}
		}

		public string TitleKeyMap
		{
			get
			{
				return (string)base.GetValue(BaseButtonBinding.TitleKeyMapProperty);
			}
			set
			{
				base.SetValue(BaseButtonBinding.TitleKeyMapProperty, value);
			}
		}

		public string TitleRemap
		{
			get
			{
				return (string)base.GetValue(BaseButtonBinding.TitleRemapProperty);
			}
			set
			{
				base.SetValue(BaseButtonBinding.TitleRemapProperty, value);
			}
		}

		public bool IsChangeCurrentBinding
		{
			get
			{
				return (bool)base.GetValue(BaseButtonBinding.IsChangeCurrentBindingProperty);
			}
			set
			{
				base.SetValue(BaseButtonBinding.IsChangeCurrentBindingProperty, value);
			}
		}

		public Brush ControlsBorderBrush
		{
			get
			{
				return (Brush)base.GetValue(BaseButtonBinding.ControlsBorderBrushProperty);
			}
			set
			{
				base.SetValue(BaseButtonBinding.ControlsBorderBrushProperty, value);
			}
		}

		public bool AllowKeyboardHooks
		{
			get
			{
				return (bool)base.GetValue(BaseButtonBinding.AllowKeyboardHooksProperty);
			}
			set
			{
				base.SetValue(BaseButtonBinding.AllowKeyboardHooksProperty, value);
			}
		}

		public bool AllowGamepadHooks
		{
			get
			{
				return (bool)base.GetValue(BaseButtonBinding.AllowGamepadHooksProperty);
			}
			set
			{
				base.SetValue(BaseButtonBinding.AllowGamepadHooksProperty, value);
			}
		}

		private static void AllowGamepadHooksCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BaseButtonBinding baseButtonBinding = d as BaseButtonBinding;
			if (baseButtonBinding == null)
			{
				return;
			}
			baseButtonBinding.OnAllowGamepadHooksChanged();
		}

		protected virtual void OnAllowGamepadHooksChanged()
		{
		}

		public GamepadButton GamepadButtonToBind
		{
			get
			{
				return (GamepadButton)base.GetValue(BaseButtonBinding.GamepadButtonToBindProperty);
			}
			set
			{
				base.SetValue(BaseButtonBinding.GamepadButtonToBindProperty, value);
			}
		}

		private static void XBButtonToBindChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BaseButtonBinding baseButtonBinding = d as BaseButtonBinding;
			if (baseButtonBinding == null)
			{
				return;
			}
			baseButtonBinding.ReEvaluateXBBinding();
		}

		public BaseButtonBinding()
		{
		}

		protected ObservableDictionary<GamepadButton, XBBinding> BindingCollection
		{
			get
			{
				GameProfilesService gameProfilesService = base.GameProfilesService;
				if (gameProfilesService == null)
				{
					return null;
				}
				return gameProfilesService.RealCurrentBeingMappedBindingCollection;
			}
		}

		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(BaseButtonBinding), new PropertyMetadata(DTLocalization.GetString(11609)));

		public static readonly DependencyProperty TitleKeyMapProperty = DependencyProperty.Register("TitleKeyMap", typeof(string), typeof(BaseButtonBinding), new PropertyMetadata(DTLocalization.GetString(11610)));

		public static readonly DependencyProperty TitleRemapProperty = DependencyProperty.Register("TitleRemap", typeof(string), typeof(BaseButtonBinding), new PropertyMetadata(DTLocalization.GetString(11334)));

		public static readonly DependencyProperty IsChangeCurrentBindingProperty = DependencyProperty.Register("IsChangeCurrentBinding", typeof(bool), typeof(BaseButtonBinding), new PropertyMetadata(true));

		public static readonly DependencyProperty ControlsBorderBrushProperty = DependencyProperty.Register("ControlsBorderBrush", typeof(Brush), typeof(BaseButtonBinding), new PropertyMetadata(null));

		public static readonly DependencyProperty AllowKeyboardHooksProperty = DependencyProperty.Register("AllowKeyboardHooks", typeof(bool), typeof(BaseButtonBinding), new PropertyMetadata(true));

		public static readonly DependencyProperty AllowGamepadHooksProperty = DependencyProperty.Register("AllowGamepadHooks", typeof(bool), typeof(BaseButtonBinding), new PropertyMetadata(true, new PropertyChangedCallback(BaseButtonBinding.AllowGamepadHooksCallback)));

		public static readonly DependencyProperty GamepadButtonToBindProperty = DependencyProperty.Register("GamepadButtonToBind", typeof(GamepadButton), typeof(BaseButtonBinding), new PropertyMetadata(0, new PropertyChangedCallback(BaseButtonBinding.XBButtonToBindChangedCallback)));
	}
}
