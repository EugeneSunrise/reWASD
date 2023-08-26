using System;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.ViewModels.Base;

namespace reWASDUI.ViewModels
{
	public class MaskViewVM : BaseServicesVM, INavigationAware
	{
		public DelegateCommand<XBBinding> ShowMacroSettingsCommand
		{
			get
			{
				DelegateCommand<XBBinding> delegateCommand;
				if ((delegateCommand = this._showMacroSettings) == null)
				{
					delegateCommand = (this._showMacroSettings = new DelegateCommand<XBBinding>(new Action<XBBinding>(this.ShowMacroSettings)));
				}
				return delegateCommand;
			}
		}

		private void ShowMacroSettings(XBBinding xbBinding)
		{
			reWASDApplicationCommands.ShowMacroSettings(xbBinding);
		}

		public MaskViewVM(IContainerProvider uc)
			: base(uc)
		{
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			if (base.GameProfilesService.RealCurrentBeingMappedBindingCollection == null)
			{
				return;
			}
			string text = navigationContext.Uri.ToString();
			if (text.Contains("BFMain") || text.Contains("BFMask") || text.Contains("BFGamepadMapping") || text.Contains("BFAdvancedZonesSettings") || text.Contains("BFAdvanced") || text.Contains("BFRumble") || text.Contains("BFShift") || text.Contains("BFAdaptiveTriggerSettings"))
			{
				return;
			}
			base.GameProfilesService.RealCurrentBeingMappedBindingCollection.IsMaskModeView = true;
		}

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
			if (base.GameProfilesService.RealCurrentBeingMappedBindingCollection == null)
			{
				return;
			}
			string text = navigationContext.Uri.ToString();
			if (!text.Contains("MaskView") && !text.Contains("MacroSettings") && !text.Contains("BFMain") && !text.Contains("BFMask") && !text.Contains("BFShift") && !text.Contains("BFGamepadMapping") && !text.Contains("BFRumble") && !text.Contains("BFAdaptiveTriggerSettings"))
			{
				base.GameProfilesService.RealCurrentBeingMappedBindingCollection.MaskBindingCollection.CurrentEditItem = null;
				base.GameProfilesService.RealCurrentBeingMappedBindingCollection.IsMaskModeView = false;
			}
		}

		private DelegateCommand<XBBinding> _showMacroSettings;
	}
}
