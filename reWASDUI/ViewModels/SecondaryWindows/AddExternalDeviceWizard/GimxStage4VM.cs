using System;
using System.Collections.ObjectModel;
using System.Linq;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using Prism.Commands;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class GimxStage4VM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.GimxStage4;
			}
		}

		public GimxStage4VM(WizardVM wizard)
			: base(wizard)
		{
			this.ExternalDeviceCollection = new ObservableCollection<ExternalDevice>(App.GamepadService.ExternalDevices);
		}

		public ObservableCollection<ExternalDevice> ExternalDeviceCollection
		{
			get
			{
				ObservableCollection<ExternalDevice> externalDeviceCollection = this._externalDeviceCollection;
				if (externalDeviceCollection != null && externalDeviceCollection.Count > 1)
				{
					for (int i = 1; i < this._externalDeviceCollection.Count; i++)
					{
						if (this._externalDeviceCollection[i].SortIndex < this._externalDeviceCollection[i - 1].SortIndex)
						{
							this._externalDeviceCollection = new ObservableCollection<ExternalDevice>(this._externalDeviceCollection.OrderBy((ExternalDevice x) => x.SortIndex));
							break;
						}
					}
				}
				return this._externalDeviceCollection;
			}
			set
			{
				if (this._externalDeviceCollection == value)
				{
					return;
				}
				this.SetProperty(ref this._externalDeviceCollection, value, "ExternalDeviceCollection");
			}
		}

		public DelegateCommand ReturnToAdapterSettingsCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._returnToAdapterSettingsCommand) == null)
				{
					delegateCommand = (this._returnToAdapterSettingsCommand = new DelegateCommand(new Action(this.ReturnToAdapterSettings)));
				}
				return delegateCommand;
			}
		}

		private void ReturnToAdapterSettings()
		{
			this.SaveExternalDevices();
			base.GoPage(PageType.AdaptersSettings);
		}

		private async void SaveExternalDevices()
		{
			if (this.ExternalDeviceCollection.All((ExternalDevice x) => x.ExternalDeviceId != this._wizard.Result.ExternalDeviceId))
			{
				this._wizard.Result.Alias = this._wizard.Result.Alias + " (" + this._wizard.Result.SerialPort + ")";
				this.ExternalDeviceCollection.Add(this._wizard.Result.Clone());
				ObservableCollection<ExternalDevice> observableCollection = new ObservableCollection<ExternalDevice>(this.ExternalDeviceCollection.Where((ExternalDevice x) => !x.IsDummy).ToList<ExternalDevice>());
				App.GamepadService.ExternalDevices = new ExternalDevicesCollection(observableCollection);
				await App.GamepadService.BinDataSerialize.SaveExternalDevices();
				App.GamepadService.ExternalDeviceRelationsHelper.Refresh();
				this.OnPropertyChanged("ExternalDevices");
			}
		}

		private ObservableCollection<ExternalDevice> _externalDeviceCollection;

		private DelegateCommand _returnToAdapterSettingsCommand;
	}
}
