using System;
using System.Windows.Controls;
using System.Windows.Input;
using DiscSoft.NET.Common.Utils;

namespace reWASDUI.Controls
{
	public class ExtendedListBoxItem : ListBoxItem
	{
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (base.IsSelected)
			{
				if (KeysState.IsCtrlHold)
				{
					base.IsSelected = false;
				}
				e.Handled = true;
				return;
			}
			base.OnMouseLeftButtonDown(e);
		}
	}
}
