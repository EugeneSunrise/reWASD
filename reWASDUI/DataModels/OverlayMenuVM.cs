using System;
using DiscSoft.NET.Common.ColorStuff;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using reWASDCommon.Infrastructure;
using reWASDUI.Infrastructure.KeyBindings;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Infrastructure.KeyBindingsModel.OverlayMenu;

namespace reWASDUI.DataModels
{
	public class OverlayMenuVM : ZBindableBase
	{
		private BaseXBBindingCollection HostCollection { get; set; }

		public OverlayMenuCircle Circle
		{
			get
			{
				return this._currentMenuCircle;
			}
			set
			{
				this.SetProperty<OverlayMenuCircle>(ref this._currentMenuCircle, value, "Circle");
			}
		}

		public OverlayMenuVM(BaseXBBindingCollection hostCollection)
		{
			this.HostCollection = hostCollection;
			this.Circle = new OverlayMenuCircle(hostCollection, 3, false, new zColor(Constants.OverlayMenuSectorItemSectorColor));
		}

		public void CopyFromModel(OverlayMenuVM model)
		{
			this.Circle.CopyFromModel(model.Menu);
		}

		public void CopyToModel(OverlayMenuVM model, BaseXBBindingCollection hostCollection)
		{
			this.Circle.CopyToModel(model.Menu, hostCollection);
		}

		public void Clear()
		{
			this.Circle = new OverlayMenuCircle(this.HostCollection, 3, false, new zColor(Constants.OverlayMenuSectorItemSectorColor));
		}

		private OverlayMenuCircle _currentMenuCircle;
	}
}
