using System;
using System.ComponentModel;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils.Clases;

namespace reWASDEngine
{
	public class ViewModelItem<TValue> : NotifyPropertyChangedObject, IComparable<ViewModelItem<TValue>>
	{
		public string DisplayName
		{
			get
			{
				if (this._localizableDisplayName != null)
				{
					return this._localizableDisplayName.Value;
				}
				return this._displayName;
			}
			set
			{
				if (this._displayName == value)
				{
					return;
				}
				this._displayName = value;
				this.OnPropertyChanged("DisplayName");
			}
		}

		public TValue Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
				this.OnPropertyChanged("Value");
			}
		}

		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value == this._isSelected)
				{
					return;
				}
				this._isSelected = value;
				this.OnPropertyChanged("IsSelected");
			}
		}

		public Visibility IsVisibled
		{
			get
			{
				return this._isVisibled;
			}
			set
			{
				if (value == this._isVisibled)
				{
					return;
				}
				this._isVisibled = value;
				this.OnPropertyChanged("IsVisibled");
			}
		}

		public int SortValue
		{
			get
			{
				return this._sortValue;
			}
		}

		public ViewModelItem(int displayNameCode, TValue value)
			: this(displayNameCode, value, int.MinValue)
		{
		}

		public ViewModelItem(int displayNameCode, TValue value, int sortValue)
		{
			this._localizableDisplayName = new Localizable(displayNameCode);
			this._localizableDisplayName.PropertyChanged += this.OnLocalizablePropertyChanged;
			this._value = value;
			this._sortValue = sortValue;
		}

		public ViewModelItem(string displayName, TValue value)
			: this(displayName, value, int.MinValue)
		{
		}

		public ViewModelItem(string displayName, TValue value, int sortValue)
		{
			this.DisplayName = displayName;
			this._value = value;
			this._sortValue = sortValue;
		}

		public int CompareTo(ViewModelItem<TValue> other)
		{
			if (other == null)
			{
				return -1;
			}
			if (this.SortValue == -2147483648 && other.SortValue == -2147483648)
			{
				return this.DisplayName.CompareTo(other.DisplayName);
			}
			if (this.SortValue != -2147483648 && other.SortValue != -2147483648)
			{
				return this.SortValue.CompareTo(other.SortValue);
			}
			if (this.SortValue != -2147483648 && other.SortValue == -2147483648)
			{
				return -1;
			}
			return 1;
		}

		private void OnLocalizablePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.OnPropertyChanged(null);
			this.OnPropertyChanged("DisplayName");
		}

		public override string ToString()
		{
			return this.DisplayName;
		}

		private const int UNSET_SORT_VALUE = -2147483648;

		private string _displayName;

		private bool _isSelected;

		private Visibility _isVisibled;

		private readonly int _sortValue;

		private Localizable _localizableDisplayName;

		private TValue _value;
	}
}
