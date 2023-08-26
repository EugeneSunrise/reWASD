using System;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDUI.Utils
{
	public class XBValidators
	{
		public static bool ValidateGameName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				DTMessageBox.Show(Application.Current.MainWindow, DTLocalization.GetString(11025), MessageBoxButton.OK, MessageBoxImage.Hand, null);
				return false;
			}
			if (!XBUtils.IsNameValid(name))
			{
				DTMessageBox.Show(Application.Current.MainWindow, DTLocalization.GetString(11026) + " " + DTLocalization.GetString(524), MessageBoxButton.OK, MessageBoxImage.Hand, null);
				return false;
			}
			return true;
		}

		public static bool ValidateConfigName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				DTMessageBox.Show(Application.Current.MainWindow, DTLocalization.GetString(11025), MessageBoxButton.OK, MessageBoxImage.Hand, null);
				return false;
			}
			if (!XBUtils.IsNameValid(name))
			{
				DTMessageBox.Show(Application.Current.MainWindow, DTLocalization.GetString(11026) + " " + DTLocalization.GetString(524), MessageBoxButton.OK, MessageBoxImage.Hand, null);
				return false;
			}
			return true;
		}

		public static bool ValidateShiftName(string name)
		{
			if (!XBUtils.IsNameValid(name))
			{
				DTMessageBox.Show(Application.Current.MainWindow, DTLocalization.GetString(11026) + " " + DTLocalization.GetString(524), MessageBoxButton.OK, MessageBoxImage.Hand, null);
				return false;
			}
			return true;
		}
	}
}
