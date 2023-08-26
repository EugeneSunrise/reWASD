using System;
using Prism.Regions;

namespace reWASDUI.Infrastructure
{
	public class SingleRegionManager : RegionManager
	{
		public Type DefaultViewType { get; set; }

		public string DefaultView
		{
			get
			{
				return this.DefaultViewType.FullName;
			}
		}

		public string RegionName { get; set; }

		public void RegisterDefaultView(Type defaultViewType, bool navigateAfterRegister = false)
		{
			this.DefaultViewType = defaultViewType;
			base.RegisterViewWithRegion(this.RegionName, this.DefaultViewType);
			if (navigateAfterRegister)
			{
				this.NavigateToDefaultView();
			}
		}

		public void NavigateToDefaultView()
		{
			base.RequestNavigate(this.RegionName, this.DefaultView);
		}

		public void RequestNavigate(string path, NavigationParameters navigationParameters)
		{
			base.RequestNavigate(this.RegionName, path, navigationParameters);
		}

		internal void RequestNavigate(string path)
		{
			base.RequestNavigate(this.RegionName, path);
		}
	}
}
