using System;
using Prism.Ioc;
using reWASDUI.ViewModels.Base;

namespace reWASDUI.ViewModels.ContentZoneGamepad
{
	public class ContentPaddlesVM : BaseKeyBindVM
	{
		public ContentPaddlesVM(IContainerProvider uc)
			: base(uc)
		{
		}
	}
}
