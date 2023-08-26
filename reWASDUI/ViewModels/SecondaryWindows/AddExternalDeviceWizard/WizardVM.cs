using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using reWASDUI.DataModels;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class WizardVM : NotifyPropertyChangedObject
	{
		public event WizardVM.CancelDelegate CancelEvent;

		public event WizardVM.CancelDelegate OkEvent;

		public ExternalDevice Result
		{
			get
			{
				return this._result;
			}
		}

		public string DeviceTypeStr
		{
			get
			{
				return DTLocalization.GetString((this.Result.DeviceType == 2) ? 12673 : 12707);
			}
		}

		public WizardVM(bool skipFirstPage = false)
		{
			this._result = new ExternalDevice();
			this._pages = new List<BasePageVM>
			{
				new AdaptersSettingsVM(this),
				new DeviceTypeVM(this),
				new BluetoothSettingsVM(this),
				new GimxStage1VM(this),
				new GimxStage2VM(this),
				new GimxStage3VM(this),
				new GimxStage4VM(this),
				new GimxFailsStageVM(this),
				new EspStage1VM(this),
				new EspStage3VM(this),
				new EspStage4VM(this),
				new EspFailsStageVM(this),
				new EspLatencySpeedVM(this),
				new ToolDownloaderVM(this),
				new AddExternalClientVM(this),
				new AddOtherBTClientVM(this),
				new AddPS4ConsoleStep1VM(this),
				new AddPS4ConsoleStep2VM(this),
				new AddPS4ConsoleStepConfigRequiredVM(this),
				new AddPS4ConsoleStep3VM(this),
				new AddPS4ConsoleStep4VM(this),
				new AddPS4ConsoleErrorPageVM(this),
				new AddNintendoSwitchConsoleStep1VM(this),
				new AddNintendoSwitchConsoleStep2VM(this),
				new AddNintendoSwitchConsoleStep2aVM(this),
				new AddNintendoSwitchConsoleStep2bVM(this),
				new AddNintendoSwitchConsoleStep3VM(this),
				new AddNintendoSwitchConsoleStepConfigRequiredVM(this),
				new AddOtherEsp32BTClientVM(this),
				new AddOtherEsp32BTClientFinishVM(this),
				new SelectAuthGamepadVM(this),
				new FinishAndWaitUSBClientVM(this)
			};
			if (skipFirstPage)
			{
				this.CurrentPage = this._pages[1];
			}
			else
			{
				this.CurrentPage = ((App.GamepadService.ExternalDevices.Count == 0) ? this._pages[1] : this._pages.First<BasePageVM>());
			}
			App.GameProfilesService.CurrentGameChanged += delegate(object s, PropertyChangedExtendedEventArgs<GameVM> e)
			{
				this.OnCancel();
			};
		}

		public WizardVM(PageType page)
		{
			this._result = new ExternalDevice();
			if (page == PageType.ToolDownloader)
			{
				this.CurrentPage = new ToolDownloaderVM(this);
			}
			if (page == PageType.EspFailsStage)
			{
				this.CurrentPage = new EspFailsStageVM(this);
			}
			this._pages = new List<BasePageVM>();
			this._pages.Add(this.CurrentPage);
		}

		public BasePageVM CurrentPage
		{
			get
			{
				return this._currentPage;
			}
			set
			{
				if (value == this._currentPage)
				{
					return;
				}
				if (this._currentPage != null)
				{
					this._currentPage.OnHidePage();
				}
				this._currentPage = value;
				if (this._currentPage != null)
				{
					this._currentPage.OnShowPage();
				}
				this.OnPropertyChanged("CurrentPage");
			}
		}

		public BasePageVM FindPage(PageType pageType)
		{
			return this._pages.FirstOrDefault((BasePageVM item) => item.PageType == pageType);
		}

		public void GoPage(PageType pageType)
		{
			if (pageType == PageType.None)
			{
				this.OnOk();
				return;
			}
			this.CurrentPage = this._pages.FirstOrDefault((BasePageVM item) => item.PageType == pageType);
			BasePageVM currentPage = this.CurrentPage;
			if (currentPage == null)
			{
				return;
			}
			currentPage.RaisePropertyChanged("AdaptersSettingsVMReinit");
		}

		public void OnCancel()
		{
			this.CurrentPage = null;
			WizardVM.CancelDelegate cancelEvent = this.CancelEvent;
			if (cancelEvent == null)
			{
				return;
			}
			cancelEvent();
		}

		public void OnOk()
		{
			this.CurrentPage = null;
			WizardVM.CancelDelegate okEvent = this.OkEvent;
			if (okEvent == null)
			{
				return;
			}
			okEvent();
		}

		public void MoveNext()
		{
			this.CurrentPage.NextCommand.Execute(null);
		}

		public async Task SaveExternalClient(ExternalClient saveExternalClient)
		{
			if (saveExternalClient.MacAddress != 0UL)
			{
				if (App.GamepadService.ExternalClients.All((ExternalClient x) => x.MacAddress != saveExternalClient.MacAddress))
				{
					App.GamepadService.ExternalClients.Add(saveExternalClient.Clone());
					await App.GamepadService.BinDataSerialize.SaveExternalClients();
				}
				else
				{
					ExternalClient externalClient = App.GamepadService.ExternalClients.FirstOrDefault((ExternalClient x) => x.MacAddress == saveExternalClient.MacAddress);
					if (externalClient != null)
					{
						if (DTMessageBox.Show(DTLocalization.GetString(12688), MessageBoxButton.YesNo, MessageBoxImage.Question, null, false, MessageBoxResult.None) == MessageBoxResult.Yes)
						{
							externalClient.Alias = saveExternalClient.Alias;
							await App.GamepadService.BinDataSerialize.SaveExternalClients();
						}
						else
						{
							saveExternalClient.Alias = externalClient.Alias;
						}
					}
				}
			}
		}

		private ExternalDevice _result;

		private List<BasePageVM> _pages;

		private BasePageVM _currentPage;

		public delegate void CancelDelegate();
	}
}
