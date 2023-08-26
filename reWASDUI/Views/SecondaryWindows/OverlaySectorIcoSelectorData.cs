using System;
using System.Collections.Generic;
using System.Linq;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure;

namespace reWASDUI.Views.SecondaryWindows
{
	public class OverlaySectorIcoSelectorData : ZBindableBase
	{
		public List<RadialMenuIcon> AllIcons
		{
			get
			{
				return this._allIcons;
			}
			set
			{
				this.SetProperty<List<RadialMenuIcon>>(ref this._allIcons, value, "AllIcons");
			}
		}

		public RadialMenuIcon SelectedIcon
		{
			get
			{
				return this._selectedIcon;
			}
			set
			{
				if (value != null)
				{
					this.SetProperty<RadialMenuIcon>(ref this._selectedIcon, value, "SelectedIcon");
				}
			}
		}

		public List<RadialMenuIconCategory> SelectedCategories
		{
			get
			{
				if (this.CurrentCategory != null)
				{
					return new List<RadialMenuIconCategory> { 1, this.CurrentCategory };
				}
				return Enum.GetValues(typeof(RadialMenuIconCategory)).Cast<RadialMenuIconCategory>().Where(delegate(RadialMenuIconCategory x)
				{
					Func<RadialMenuIconCategory, bool> <>9__2;
					return this.AllIcons.Any(delegate(RadialMenuIcon y)
					{
						IEnumerable<RadialMenuIconCategory> categories = y.Categories;
						Func<RadialMenuIconCategory, bool> func;
						if ((func = <>9__2) == null)
						{
							func = (<>9__2 = (RadialMenuIconCategory z) => z == x);
						}
						return categories.Any(func);
					});
				})
					.ToList<RadialMenuIconCategory>();
			}
		}

		public List<RadialMenuIconCategory> AllCategories
		{
			get
			{
				return this._allCategories;
			}
			set
			{
				this.SetProperty<List<RadialMenuIconCategory>>(ref this._allCategories, value, "AllCategories");
			}
		}

		public RadialMenuIconCategory CurrentCategory
		{
			get
			{
				return this._currentCategory;
			}
			set
			{
				this.SetProperty<RadialMenuIconCategory>(ref this._currentCategory, value, "CurrentCategory");
				this.RecalculateIcons();
			}
		}

		private void RecalculateIcons()
		{
			this.OnPropertyChanged("SelectedCategories");
			this.OnPropertyChanged("SelectedIcon");
		}

		public OverlaySectorIcoSelectorData()
		{
			this.AllIcons = RadialMenuIcons.GetAllIcons();
			this.AllCategories = Enum.GetValues(typeof(RadialMenuIconCategory)).Cast<RadialMenuIconCategory>().Where(delegate(RadialMenuIconCategory x)
			{
				Func<RadialMenuIconCategory, bool> <>9__2;
				return x == null || (x != 1 && this.AllIcons.Any(delegate(RadialMenuIcon y)
				{
					IEnumerable<RadialMenuIconCategory> categories = y.Categories;
					Func<RadialMenuIconCategory, bool> func;
					if ((func = <>9__2) == null)
					{
						func = (<>9__2 = (RadialMenuIconCategory z) => z == x);
					}
					return categories.Any(func);
				}));
			})
				.ToList<RadialMenuIconCategory>();
			this.CurrentCategory = 0;
		}

		private List<RadialMenuIcon> _allIcons;

		private RadialMenuIcon _selectedIcon;

		private List<RadialMenuIconCategory> _allCategories;

		private RadialMenuIconCategory _currentCategory;
	}
}
