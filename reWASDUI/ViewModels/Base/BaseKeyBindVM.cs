using System;
using Prism.Ioc;

namespace reWASDUI.ViewModels.Base
{
	public abstract class BaseKeyBindVM : BaseServicesVM
	{
		public BaseKeyBindVM(IContainerProvider uc)
			: base(uc)
		{
		}
	}
}
