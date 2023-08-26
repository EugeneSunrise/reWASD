using System;
using System.ComponentModel;
using DiscSoft.NET.Common.Localization;

namespace reWASDEngine
{
	public class ViewModelItemWithDescription<TValue> : ViewModelItem<TValue>
	{
		public ViewModelItemWithDescription(int displayNameCode, int descriptionCode, TValue value)
			: this(displayNameCode, descriptionCode, value, int.MinValue)
		{
		}

		public ViewModelItemWithDescription(int displayNameCode, int descriptionCode, TValue value, int sortValue)
			: base(displayNameCode, value, int.MinValue)
		{
			this._localizableDescription = new Localizable(descriptionCode);
			this._localizableDescription.PropertyChanged += this.OnLocalizablePropertyChanged;
		}

		private void OnLocalizablePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.OnPropertyChanged("Description");
		}

		public string Description
		{
			get
			{
				if (this._localizableDescription != null)
				{
					return this._localizableDescription.Value;
				}
				return this._description;
			}
			set
			{
				if (this._description == value)
				{
					return;
				}
				this._description = value;
			}
		}

		private string _description;

		private Localizable _localizableDescription;

		private const int UNSET_SORT_VALUE = -2147483648;
	}
}
