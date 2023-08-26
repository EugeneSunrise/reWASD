using System;
using Prism.Regions;

namespace reWASDUI.Services.Interfaces
{
	public interface IRegionManagerAware
	{
		IRegionManager RegionManager { get; set; }
	}
}
