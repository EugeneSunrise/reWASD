using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using reWASDUI.ViewModels.Base;

namespace reWASDUI.Views.ContentZoneGamepad.AdvancedStick
{
	public partial class AdvancedSpringSettings : BaseDirectionalAnalogGroupUserControl
	{
		private BaseServicesVM _dataContext
		{
			get
			{
				return base.DataContext as BaseServicesVM;
			}
		}

		public AdvancedSpringSettings()
		{
			this.InitializeComponent();
		}
	}
}
