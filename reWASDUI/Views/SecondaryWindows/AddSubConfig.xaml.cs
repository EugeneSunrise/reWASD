using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.SecondaryWindows.Base;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.Views.SecondaryWindows
{
	public partial class AddSubConfig : BaseSecondaryWindow
	{
		public ControllerFamily ControllerFamily
		{
			get
			{
				return this._controllerFamily;
			}
			set
			{
				this.SetProperty(ref this._controllerFamily, value, "ControllerFamily");
			}
		}

		public AddSubConfig()
		{
			this.InitializeComponent();
			base.DataContext = this;
		}

		protected override void OkButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.ControllerFamily == 4)
			{
				DTMessageBox.Show(Application.Current.MainWindow, DTLocalization.GetString(11444), MessageBoxButton.OK, MessageBoxImage.Hand, null);
				return;
			}
			this.WindowResult = MessageBoxResult.OK;
			base.Close();
		}

		private ControllerFamily _controllerFamily;
	}
}
