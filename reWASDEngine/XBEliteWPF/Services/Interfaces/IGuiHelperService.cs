using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using DiscSoft.NET.Common.Utils;
using Prism.Commands;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.DataModels;
using XBEliteWPF.License.Licensing.ComStructures;

namespace XBEliteWPF.Services.Interfaces
{
	public interface IGuiHelperService
	{
		DelegateCommand<object> BindingFrameBackCommand { get; }

		void EditGameExecute(Game game);

		void RemoveGameExecute(Game game);

		void CreateConfigExecute(Game game);

		void DeleteConfigExecute(Config config);

		void CloneConfigExecute(Config config);

		void OpenConfigExecute(Config config);

		void ShareConfigExecute(Config config);

		void RenameConfigExecute(Config config);

		void PrintConfigExecute(Config config);

		DelegateCommand SwitchBindingLabelModeCommand { get; }

		DelegateCommand SwitchMaskViewModeCommand { get; }

		DelegateCommand SwitchVirtualStickSettingsViewModeCommand { get; }

		DelegateCommand SwitchLEDSettingsViewModeCommand { get; }

		DelegateCommand OpenLEDSettingsViewModeCommand { get; }

		Task<bool> ShowLicenseWizard(Ref<LicenseCheckResult> checkingResultInfo);

		Task<bool> ShowLicenseWizardUpdate(Ref<LicenseCheckResult> checkingResultInfo);

		Task<bool> ShowLicenseWizard(Ref<HtmlOffer> offer, Ref<LicenseCheckResult> checkingResultInfo);

		Task ImportGameConfig(string configPath = "", bool isCloning = false);

		MessageBoxResult AddExternalDeviceWizard();

		MessageBoxResult AddSubConfigDialog(out ControllerFamily controllerFamily);

		bool ShowDialogAddEditGame(ref string sName, ref string sImageSourcePath, ref ObservableCollection<string> applicationNamesCollection, int configCount = -1);
	}
}
