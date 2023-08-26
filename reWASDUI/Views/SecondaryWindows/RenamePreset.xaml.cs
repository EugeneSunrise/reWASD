using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.Controls;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.SecondaryWindows.Base;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using reWASDCommon.Infrastructure;
using reWASDUI.Utils;
using reWASDUI.Utils.XBUtil;

namespace reWASDUI.Views.SecondaryWindows
{
	public partial class RenamePreset : BaseSecondaryWindow
	{
		public RenamePreset()
		{
			this.InitializeComponent();
		}

		private bool IsPresetNameValid(string preset)
		{
			return !string.IsNullOrEmpty(preset) && XBValidators.ValidateShiftName(preset);
		}

		protected override void OkButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.IsPresetNameValid(this.textBox.Text.Trim()))
			{
				List<string> list = new List<string>();
				list.Add(".rewasd");
				list.Add(".rewasd.bak");
				string text = list.OrderByDescending((string x) => x.Length).First<string>();
				string text2 = RegistryHelper.GetString("Config", "PresetsFolderPath", Path.Combine(Constants.COMMON_DOCUMENTS_DIRECTORY_PATH, "Presets"), false);
				if (!this.IsPathCorrect(text2))
				{
					text2 = Path.Combine(Constants.COMMON_DOCUMENTS_DIRECTORY_PATH, "Presets");
				}
				string text3 = XBUtils.NormalizeToMaxPathTrimFilename(text2, this.textBox.Text.Trim(), text, null);
				if (File.Exists(Path.Combine(text2, text3) + ".rewasd"))
				{
					DTMessageBox.Show(DTLocalization.GetString(12579), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
					return;
				}
				this.WindowResult = MessageBoxResult.OK;
				base.Close();
			}
		}

		private bool IsPathCorrect(string path)
		{
			path = path.Trim();
			return !string.IsNullOrEmpty(path) && !(path.Substring(0, 1) == ".") && !(path.Substring(0, 1) == "\\");
		}

		private void textBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.OkButton.IsEnabled = this.IsPresetNameValid(this.textBox.Text.Trim());
		}
	}
}
