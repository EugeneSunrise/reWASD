using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.SecondaryWindows.Base;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.ViewModels.SecondaryWindows.CalibrateGyroscope;

namespace reWASDUI.Views.SecondaryWindows.CalibrateGyroscope
{
	public partial class GyroWizard : BaseSecondaryWindow
	{
		public Gyroscope Gyro
		{
			get
			{
				return this._gyroWizardVM.Gyro;
			}
		}

		public GyroWizard(BaseControllerVM controller)
		{
			this.InitializeComponent();
			this._gyroWizardVM = new GyroWizardVM(controller);
			this._gyroWizardVM.CancelEvent += delegate
			{
				this.WindowResult = (this.WindowResult = MessageBoxResult.Cancel);
				base.Close();
			};
			this._gyroWizardVM.OkEvent += delegate
			{
				this.WindowResult = MessageBoxResult.OK;
				base.Close();
			};
			base.DataContext = this._gyroWizardVM;
		}

		protected override void OkButton_Click(object sender, RoutedEventArgs e)
		{
			this._gyroWizardVM.MoveNext();
		}

		private GyroWizardVM _gyroWizardVM;
	}
}
