using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace reWASDUI.Controls
{
	public partial class IconAnimation : UserControl, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public SolidColorBrush AnimationBrush
		{
			get
			{
				return this._animationBrush;
			}
			set
			{
				if (value == this._animationBrush)
				{
					return;
				}
				this._animationBrush = value;
				this.OnPropertyChanged("AnimationBrush");
			}
		}

		public IconAnimation()
		{
			this.InitializeComponent();
			base.DataContext = this;
		}

		public void PlayAnimation()
		{
			(base.TryFindResource("Storyboard1") as Storyboard).Begin();
		}

		private SolidColorBrush _animationBrush;
	}
}
