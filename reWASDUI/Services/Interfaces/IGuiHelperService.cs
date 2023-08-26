using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using DiscSoft.NET.Common.Utils;
using Prism.Commands;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.DataModels;
using XBEliteWPF.License.Licensing.ComStructures;

namespace reWASDUI.Services.Interfaces
{
	public interface IGuiHelperService
	{
		DelegateCommand<object> BindingFrameBackCommand { get; }

		void EditGameExecute(GameVM game);

		Task<bool> EditGameAppsExecute(GameVM game);

		void RemoveGameExecute(GameVM game);

		void CloneGameExecute(GameVM game);

		void CreateConfigExecute(GameVM game);

		void SaveAsGameExecute(GameVM config);

		Task RemovePresetExecute(ConfigVM config);

		Task DeleteConfigExecute(ConfigVM config);

		void CloneConfigExecute(ConfigVM config);

		void OpenConfigExecute(ConfigVM config);

		void ClearConfigExecute(ConfigVM config);

		void ShareConfigExecute(ConfigVM config);

		void RenameConfigExecute(ConfigVM config);

		void RenamePresetExecute(ConfigVM config);

		void PrintConfigExecute(ConfigVM config);

		void ShowRadialMenuIconSelector(ConfigVM config);

		DelegateCommand SwitchBindingLabelModeCommand { get; }

		DelegateCommand SwitchMaskViewModeCommand { get; }

		DelegateCommand SwitchVirtualStickSettingsViewModeCommand { get; }

		DelegateCommand SwitchLEDSettingsViewModeCommand { get; }

		DelegateCommand OpenLEDSettingsViewModeCommand { get; }

		DelegateCommand SwitchOverlayMenuViewModeCommand { get; }

		Task<bool> ShowLicenseWizard(Ref<LicenseCheckResult> checkingResultInfo);

		Task<bool> ShowLicenseWizardUpdate(Ref<LicenseCheckResult> checkingResultInfo);

		Task<bool> ShowLicenseWizard(Ref<HtmlOffer> offer, Ref<LicenseCheckResult> checkingResultInfo);

		Task<bool> ImportGameConfig(string configPath = "", bool isCloning = false);

		MessageBoxResult AddExternalDeviceWizard();

		MessageBoxResult AddSubConfigDialog(out ControllerFamily controllerFamily);

		bool ShowDialogAddEditGame(ref string sName, ref string sImageSourcePath, ref ObservableCollection<string> applicationNamesCollection, int configCount = -1);
	}
}
