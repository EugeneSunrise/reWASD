using System;
using System.Windows;
using System.Windows.Media;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDEngine.Services.OverlayAPI
{
	public class RightButtonsInfo : ZBindableBase
	{
		public Drawing Image
		{
			get
			{
				return this._image;
			}
			set
			{
				this.SetProperty<Drawing>(ref this._image, value, "Image");
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this.SetProperty<string>(ref this._name, value, "Name");
			}
		}

		public string AutoName
		{
			get
			{
				return this._autoname;
			}
			set
			{
				this.SetProperty<string>(ref this._autoname, value, "AutoName");
			}
		}

		public float Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if (!this.SetProperty<float>(ref this._value, value, "Value"))
				{
					return;
				}
				this.OnPropertyChanged("PressedBrush");
				this.OnPropertyChanged("StringValue");
			}
		}

		public Brush PressedBrush
		{
			get
			{
				if (this.Value <= 0f)
				{
					return new SolidColorBrush(Colors.White);
				}
				return Application.Current.TryFindResource("CreamBrush") as SolidColorBrush;
			}
		}

		public string StringValue
		{
			get
			{
				if (this.Name.Length <= 0)
				{
					return "";
				}
				return this.Value.ToString("f2");
			}
		}

		private string _name;

		private string _autoname;

		private float _value;

		private Drawing _image;

		public GamepadButton btn;
	}
}
