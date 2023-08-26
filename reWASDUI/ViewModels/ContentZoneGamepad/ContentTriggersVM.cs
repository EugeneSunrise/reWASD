using System;
using Prism.Ioc;
using reWASDUI.ViewModels.Base;

namespace reWASDUI.ViewModels.ContentZoneGamepad
{
	public class ContentTriggersVM : BaseKeyBindVM
	{
		public ContentTriggersVM(IContainerProvider uc)
			: base(uc)
		{
		}
	}
}
