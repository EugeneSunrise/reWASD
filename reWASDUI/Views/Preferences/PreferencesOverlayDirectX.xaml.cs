using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.Controls.CheckBoxes;
using Microsoft.Win32;
using reWASDUI.ViewModels.Preferences;
using reWASDUI.Views.SecondaryWindows;

namespace reWASDUI.Views.Preferences
{
	public partial class PreferencesOverlayDirectX : UserControl
	{
		private PreferencesOverlayDirectXVM DirectXVM
		{
			get
			{
				return (PreferencesOverlayDirectXVM)base.DataContext;
			}
		}

		public PreferencesOverlayDirectX()
		{
			this.InitializeComponent();
		}

		private void btnBrowseApplicationClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Applications | *.exe";
			openFileDialog.CheckFileExists = true;
			openFileDialog.Multiselect = false;
			bool? flag = openFileDialog.ShowDialog();
			bool flag2 = true;
			if ((flag.GetValueOrDefault() == flag2) & (flag != null))
			{
				FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
				this.DirectXVM.AddAutodetectProcess(fileInfo.Name);
			}
		}

		private void btnBrowseProcessClick(object sender, RoutedEventArgs e)
		{
			ProcessListDialog processListDialog = new ProcessListDialog();
			processListDialog.InitialProcessList = this.DirectXVM.GetApplicationsString().Split(';', StringSplitOptions.None).ToList<string>();
			processListDialog.ShowDialog();
			if (processListDialog.WindowResult == MessageBoxResult.OK)
			{
				foreach (ProcessListDialog.ProcessItem processItem in processListDialog.ProcessList)
				{
					string fileName = Path.GetFileName(processItem.FilePath);
					if (processItem.IsChecked && fileName != null && !processListDialog.InitialProcessList.Any((string item) => item.ToLower() == fileName.ToLower()))
					{
						this.DirectXVM.AddAutodetectProcess(Path.GetFileName(processItem.FilePath));
					}
					if (!processItem.IsChecked && fileName != null && processListDialog.InitialProcessList.Any((string item) => item.ToLower() == fileName.ToLower()))
					{
						this.DirectXVM.RemoveApplicationName(Path.GetFileName(processItem.FilePath));
					}
				}
			}
		}
	}
}
