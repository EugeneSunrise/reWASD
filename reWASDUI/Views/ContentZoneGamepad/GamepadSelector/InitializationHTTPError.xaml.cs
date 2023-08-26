using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Prism.Commands;
using reWASDCommon.Infrastructure;

namespace reWASDUI.Views.ContentZoneGamepad.GamepadSelector
{
	public partial class InitializationHTTPError : UserControl
	{
		public InitializationHTTPError()
		{
			this.InitializeComponent();
			base.DataContext = this;
		}

		public DelegateCommand ShowLogFolderCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._showLogFolderCommand) == null)
				{
					delegateCommand = (this._showLogFolderCommand = new DelegateCommand(new Action(this.ShowLogFolder)));
				}
				return delegateCommand;
			}
		}

		private void ShowLogFolder()
		{
			new Process
			{
				StartInfo = 
				{
					FileName = Constants.LOG_ROOT_PATH
				}
			}.Start();
		}

		private DelegateCommand _showLogFolderCommand;
	}
}
