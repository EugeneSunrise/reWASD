using System;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using Prism.Commands;
using reWASDUI.DataModels.CompositeDevicesCollection;
using reWASDUI.Services;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.ViewModels.SecondaryWindows
{
	public class CompositeDevicesWindowVM : ZBindable
	{
		public GamepadService GamepadService { get; private set; }

		public DelegateCommand AddGroupCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._AddGroup) == null)
				{
					delegateCommand = (this._AddGroup = new DelegateCommand(new Action(this.AddGroup), new Func<bool>(this.AddGroupCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void AddGroup()
		{
			CompositeDevice compositeDevice = new CompositeDevice();
			this.GamepadService.CompositeDevices.Add(compositeDevice);
			this.GamepadService.CompositeDevices.CurrentEditItem = compositeDevice;
		}

		private bool AddGroupCanExecute()
		{
			return this.GamepadService.ControllersAvailiableForComposition.Count > 1 && this.GamepadService.CompositeDevices.CurrentEditItem == null;
		}

		public CompositeDevicesWindowVM(GamepadService gamepadService)
		{
			this.GamepadService = gamepadService;
			this.GamepadService.CompositeDevices.CurrentEditItemChanged += delegate
			{
				this.AddGroupCommand.RaiseCanExecuteChanged();
			};
			this.GamepadService.ControllersAvailiableForComposition.CollectionChanged += delegate([Nullable(2)] object sender, NotifyCollectionChangedEventArgs args)
			{
				this.AddGroupCommand.RaiseCanExecuteChanged();
			};
		}

		private DelegateCommand _AddGroup;
	}
}
