using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.SecondaryWindows.Base;
using Prism.Commands;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace reWASDUI.Views.SecondaryWindows
{
	public partial class GetExternalClientAddress : BaseSecondaryWindow
	{
		public ObservableCollection<ExternalClient> ExternalClientsCollection { get; set; }

		public ExternalClient ExternalClient
		{
			get
			{
				return this._externalClient;
			}
			set
			{
				this.SetProperty(ref this._externalClient, value, "ExternalClient");
			}
		}

		public GetExternalClientAddress()
		{
			this.InitializeComponent();
			base.DataContext = this;
			this.ExternalClientsCollection = App.GamepadService.ExternalClients;
		}

		public DelegateCommand AddExternalClientCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._addExternalClientCommand) == null)
				{
					delegateCommand = (this._addExternalClientCommand = new DelegateCommand(new Action(this.AddExternalClient)));
				}
				return delegateCommand;
			}
		}

		private async void AddExternalClient()
		{
			AddExternalClient dialog = new AddExternalClient();
			dialog.ShowDialog();
			if (dialog.WindowResult == MessageBoxResult.OK && this.ExternalClientsCollection.All((ExternalClient x) => x.MacAddressText != dialog.MacAddressText))
			{
				ExternalClient externalClient = new ExternalClient
				{
					Alias = dialog.Alias,
					MacAddress = dialog.MacAddress
				};
				this.ExternalClientsCollection.Add(externalClient);
				await App.GamepadService.BinDataSerialize.SaveExternalClients();
				App.GamepadService.ExternalDeviceRelationsHelper.Refresh();
			}
		}

		public DelegateCommand AddExternalPS4ClientCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._addExternalPS4ClientCommand) == null)
				{
					delegateCommand = (this._addExternalPS4ClientCommand = new DelegateCommand(new Action(this.AddExternalPS4Client)));
				}
				return delegateCommand;
			}
		}

		private async void AddExternalPS4Client()
		{
			AddPS4ExternalClient dialog = new AddPS4ExternalClient();
			dialog.ShowDialog();
			if (dialog.WindowResult == MessageBoxResult.OK && this.ExternalClientsCollection.All((ExternalClient x) => x.MacAddressText != dialog.MacAddressText && x.ConsoleType != 1))
			{
				ExternalClient externalClient = new ExternalClient
				{
					Alias = dialog.Alias,
					MacAddress = dialog.MacAddress,
					ConsoleType = 1
				};
				this.ExternalClientsCollection.Add(externalClient);
				await App.GamepadService.BinDataSerialize.SaveExternalClients();
				App.GamepadService.ExternalDeviceRelationsHelper.Refresh();
			}
		}

		private ExternalClient _externalClient;

		private DelegateCommand _addExternalClientCommand;

		private DelegateCommand _addExternalPS4ClientCommand;
	}
}
