using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Utils;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Services;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class AddPS4ConsoleStep2VM : BasePageVM
	{
		public GamepadService GamepadService { get; set; }

		public override PageType PageType
		{
			get
			{
				return PageType.AddPS4ConsoleStep2;
			}
		}

		public ControllerVM DS4ControllerVM { get; set; }

		public AddPS4ConsoleStep2VM(WizardVM wizard)
			: base(wizard)
		{
			this.GamepadService = (GamepadService)App.GamepadService;
			this.Alias = "PS4 console";
			this.MacAddressText = "00:00:00:00:00:00";
			this.ConsoleType = 1;
		}

		protected override async void NavigateToNextPage()
		{
			if (!this.IsMacAddressError)
			{
				this.IsSaveEnabled = false;
				await this.SaveExternalClients();
				bool flag = await App.GameProfilesService.ApplyCurrentProfile(false);
				this.IsSaveEnabled = true;
				base.GoPage(flag ? PageType.AddPS4ConsoleStep3 : PageType.AddPS4ConsoleStepConfigRequired);
			}
			else
			{
				base.GoPage(PageType.AddPS4ConsoleErrorPage);
			}
		}

		protected override void NavigatePreviousPage()
		{
			base.GoPage(PageType.AddPS4ConsoleStep1);
		}

		public override void OnShowPage()
		{
			this.Alias = this._wizard.Result.Alias;
			this.StartPoller();
		}

		public override void OnHidePage()
		{
			this.StopPoller();
		}

		public bool IsSaveEnabled
		{
			get
			{
				return this._isSaveEnabled;
			}
			set
			{
				this.SetProperty(ref this._isSaveEnabled, value, "IsSaveEnabled");
			}
		}

		public ConsoleType ConsoleType { get; set; }

		public ulong MacAddress { get; set; }

		public bool IsMacAddressError { get; set; }

		private string Alias { get; set; }

		private string MacAddressText { get; set; }

		public ExternalClient ExternalClient { get; set; }

		private void StartPoller()
		{
			this._pollingTimer = new DispatcherTimer();
			this._pollingTimer.Tick += delegate([Nullable(2)] object o, EventArgs e)
			{
				this.CheckNewPS4WithMac();
			};
			this._pollingTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
			this._pollingTimer.Start();
		}

		private void StopPoller()
		{
			DispatcherTimer pollingTimer = this._pollingTimer;
			if (pollingTimer != null)
			{
				pollingTimer.Stop();
			}
			this._pollingTimer = null;
		}

		private ControllerVM FindDS4Controller()
		{
			ControllerVM controllerVM = (ControllerVM)App.GamepadService.GamepadCollection.FirstOrDefault(delegate(BaseControllerVM item)
			{
				ControllerVM controllerVM2 = item as ControllerVM;
				return controllerVM2 != null && controllerVM2.ControllerType == 4;
			});
			if (controllerVM == null)
			{
				foreach (BaseControllerVM baseControllerVM in App.GamepadService.GamepadCollection)
				{
					CompositeControllerVM compositeControllerVM = baseControllerVM as CompositeControllerVM;
					if (compositeControllerVM != null)
					{
						controllerVM = (ControllerVM)compositeControllerVM.BaseControllers.FirstOrDefault(delegate(BaseControllerVM composititem)
						{
							ControllerVM controllerVM3 = composititem as ControllerVM;
							return controllerVM3 != null && controllerVM3.ControllerType == 4;
						});
						return controllerVM;
					}
				}
				return controllerVM;
			}
			return controllerVM;
		}

		private void CheckNewPS4WithMac()
		{
			this.DS4ControllerVM = this.FindDS4Controller();
			if (this.DS4ControllerVM != null && this.DS4ControllerVM.MasterBthAddr != 0UL)
			{
				this.IsMacAddressError = false;
				this.MacAddress = this.DS4ControllerVM.MasterBthAddr;
				this.MacAddressText = UtilsCommon.MacAddressToString(this.MacAddress, ":");
				this.ConsoleType = 1;
				this.IsSaveEnabled = true;
				this.StopPoller();
				return;
			}
			if (this.DS4ControllerVM != null)
			{
				this.IsMacAddressError = true;
				this.IsSaveEnabled = true;
				this.StopPoller();
				return;
			}
			this.IsSaveEnabled = false;
		}

		private async Task SaveExternalClients()
		{
			if (this.DS4ControllerVM != null)
			{
				ExternalClient externalClient = new ExternalClient
				{
					Alias = this.Alias,
					MacAddress = this.MacAddress,
					ConsoleType = this.ConsoleType
				};
				await this._wizard.SaveExternalClient(externalClient);
				this.ExternalClient = externalClient;
				ExternalDevice currentExternalDevice = (this._wizard.FindPage(PageType.AdaptersSettings) as AdaptersSettingsVM).CurrentExternalDevice;
				await App.GamepadService.ExternalDeviceRelationsHelper.AddAndSaveRelation(currentExternalDevice, this.ExternalClient, new GamepadAuth(this.DS4ControllerVM.ID, this.DS4ControllerVM.ControllerDisplayName, this.DS4ControllerVM.FirstControllerType));
			}
		}

		private DispatcherTimer _pollingTimer;

		public bool _isSaveEnabled;
	}
}
