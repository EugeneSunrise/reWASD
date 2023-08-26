using System;
using System.Windows.Media;
using DiscSoft.NET.Common.ViewModel.BindableBase;

namespace reWASDUI.Views.OverlayMenu
{
	public class TViewTabs : ZBindableBase
	{
		public TViewTabs(string iName, Drawing img)
		{
			this.Text = iName;
			this.Img = img;
		}

		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				this._text = value;
			}
		}

		public Drawing Img
		{
			get
			{
				return this._Img;
			}
			set
			{
				this._Img = value;
			}
		}

		private string _text;

		private Drawing _Img;
	}
}
