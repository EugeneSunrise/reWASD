using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.Localization;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;

namespace reWASDUI.Controls
{
	public partial class ShiftTooltip : UserControl
	{
		public bool MainShift
		{
			get
			{
				return (bool)base.GetValue(ShiftTooltip.MainShiftProperty);
			}
			set
			{
				base.SetValue(ShiftTooltip.MainShiftProperty, value);
			}
		}

		public int Shift
		{
			get
			{
				return (int)base.GetValue(ShiftTooltip.ShiftProperty);
			}
			set
			{
				if (value != this.Shift)
				{
					base.SetValue(ShiftTooltip.ShiftProperty, value);
					this.CreateTooltip();
				}
			}
		}

		public ConfigVM Config
		{
			get
			{
				return (ConfigVM)base.GetValue(ShiftTooltip.ConfigProperty);
			}
			set
			{
				base.SetValue(ShiftTooltip.ConfigProperty, value);
			}
		}

		public IEnumerable<ActivatorXBBinding> Activators
		{
			get
			{
				return (IEnumerable<ActivatorXBBinding>)base.GetValue(ShiftTooltip.ActivatorsPropertyKey.DependencyProperty);
			}
			protected set
			{
				base.SetValue(ShiftTooltip.ActivatorsPropertyKey, value);
			}
		}

		public string ShiftHint
		{
			get
			{
				return (string)base.GetValue(ShiftTooltip.ShiftHintPropertyKey.DependencyProperty);
			}
			protected set
			{
				base.SetValue(ShiftTooltip.ShiftHintPropertyKey, value);
			}
		}

		public ShiftTooltip()
		{
			this.InitializeComponent();
			base.Loaded += this.ShiftTooltip_Loaded;
			App.EventAggregator.GetEvent<CurrentShiftBindingCollectionChanged>().Subscribe(delegate(ShiftXBBindingCollection e)
			{
				this.CreateTooltip();
			});
		}

		private void ShiftTooltip_Loaded(object sender, RoutedEventArgs e)
		{
			this.CreateTooltip();
		}

		private void CreateTooltip()
		{
			int num = 0;
			if (!this.MainShift)
			{
				num = this.Shift;
				ConfigVM config = this.Config;
				BaseXBBindingCollection baseXBBindingCollection;
				if (config == null)
				{
					baseXBBindingCollection = null;
				}
				else
				{
					MainXBBindingCollection currentBindingCollection = config.CurrentBindingCollection;
					baseXBBindingCollection = ((currentBindingCollection != null) ? currentBindingCollection.GetCollectionByLayer(this.Shift) : null);
				}
				BaseXBBindingCollection baseXBBindingCollection2 = baseXBBindingCollection;
				this.Activators = ((baseXBBindingCollection2 != null) ? baseXBBindingCollection2.EnumAllActivatorsWithShift : null);
			}
			string text = "";
			if (this.Config != null)
			{
				using (IEnumerator<SubConfigData> enumerator = this.Config.ConfigData.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						BaseXBBindingCollection collectionByLayer = enumerator.Current.MainXBBindingCollection.GetCollectionByLayer(num);
						if (collectionByLayer != null)
						{
							text = collectionByLayer.Description;
						}
					}
				}
			}
			if (text == null || text.Length == 0)
			{
				if (this.MainShift)
				{
					text = DTLocalization.GetString(11195);
				}
				else
				{
					text = string.Format(DTLocalization.GetString(12474), num);
				}
			}
			if (this.MainShift)
			{
				this.ShiftHint = text;
				this.Shift = 0;
				return;
			}
			IEnumerable<ActivatorXBBinding> activators = this.Activators;
			this.ShiftHint = ((activators != null && activators.Count<ActivatorXBBinding>() > 0) ? string.Format(DTLocalization.GetString(12409), text) : text);
		}

		public static readonly DependencyProperty MainShiftProperty = DependencyProperty.Register("MainShift", typeof(bool), typeof(ShiftTooltip), new PropertyMetadata(false));

		public static readonly DependencyProperty ShiftProperty = DependencyProperty.Register("Shift", typeof(int), typeof(ShiftTooltip), new PropertyMetadata(0));

		public static readonly DependencyProperty ConfigProperty = DependencyProperty.Register("Config", typeof(ConfigVM), typeof(ShiftTooltip), new PropertyMetadata(null));

		public static readonly DependencyPropertyKey ActivatorsPropertyKey = DependencyProperty.RegisterReadOnly("Activators", typeof(IEnumerable<ActivatorXBBinding>), typeof(ShiftTooltip), new PropertyMetadata(null));

		public static readonly DependencyPropertyKey ShiftHintPropertyKey = DependencyProperty.RegisterReadOnly("ShiftHint", typeof(string), typeof(ShiftTooltip), new PropertyMetadata(null));
	}
}
