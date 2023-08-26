using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace reWASDUI.Controls
{
	public class ExtendedListBox : ListBox
	{
		static ExtendedListBox()
		{
			if (ExtendedListBox._notifyListItemClickedMethodInfo == null)
			{
				throw new NotSupportedException("Failed to get NotifyListItemClicked method info by reflection");
			}
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new ExtendedListBoxItem();
		}

		public void NotifyListItemClickedImp(ListBoxItem item, MouseButton button)
		{
			ExtendedListBox._notifyListItemClickedMethodInfo.Invoke(this, new object[] { item, button });
		}

		private static readonly MethodInfo _notifyListItemClickedMethodInfo = typeof(ListBox).GetMethod("NotifyListItemClicked", BindingFlags.Instance | BindingFlags.NonPublic);
	}
}
