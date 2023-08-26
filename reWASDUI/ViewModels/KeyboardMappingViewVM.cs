using System;
using System.Windows.Input;
using DiscSoft.NET.Common.Utils.Clases;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.ViewModels.Base;
using reWASDUI.Views;
using reWASDUI.Views.ContentZoneGamepad;

namespace reWASDUI.ViewModels
{
	public class KeyboardMappingViewVM : BaseServicesVM, INavigationAware
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

		public KeyboardMappingViewVM(IContainerProvider uc)
			: base(uc)
		{
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
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
			if (!text.Contains("MacroSettings") && !text.Contains("BFKeyboard") && !text.Contains("BFShift"))
			{
				base.GameProfilesService.RealCurrentBeingMappedBindingCollection.ControllerBindings.CurrentEditItem = null;
			}
		}

		public DelegateCommand ShowMouseSettingsCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._showMouseSettings) == null)
				{
					delegateCommand = (this._showMouseSettings = new DelegateCommand(new Action(this.ShowMouseSettings)));
				}
				return delegateCommand;
			}
		}

		private void ShowMouseSettings()
		{
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			if (realCurrentBeingMappedBindingCollection != null)
			{
				realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(null);
			}
			reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(MouseSettings));
		}

		public DelegateCommand<bool?> ShowGamepadCommand
		{
			get
			{
				DelegateCommand<bool?> delegateCommand;
				if ((delegateCommand = this._showGamepad) == null)
				{
					delegateCommand = (this._showGamepad = new DelegateCommand<bool?>(new Action<bool?>(this.ShowGamepad)));
				}
				return delegateCommand;
			}
		}

		private void ShowGamepad(bool? showBack = null)
		{
			if (showBack != null)
			{
				base.GamepadService.CurrentGamepadFlipCommand.Execute(new bool?(showBack.Value));
			}
			reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(SVGGamepadWithAllAnnotations));
		}

		public ICommand ShowMouseCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._showMouse) == null)
				{
					relayCommand = (this._showMouse = new RelayCommand(new Action(this.ShowMouse), new Func<bool>(this.ShowMouseCanExecute)));
				}
				return relayCommand;
			}
		}

		private void ShowMouse()
		{
			reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(MouseMappingView));
		}

		private bool ShowMouseCanExecute()
		{
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			return realCurrentBeingMappedBindingCollection != null && realCurrentBeingMappedBindingCollection.SubConfigData.IsMouse;
		}

		public DelegateCommand<bool?> ShowVibrationSettingsCommand
		{
			get
			{
				DelegateCommand<bool?> delegateCommand;
				if ((delegateCommand = this._showVibrationSettings) == null)
				{
					delegateCommand = (this._showVibrationSettings = new DelegateCommand<bool?>(new Action<bool?>(this.ShowVibrationSettings)));
				}
				return delegateCommand;
			}
		}

		private void ShowVibrationSettings(bool? val)
		{
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			if (realCurrentBeingMappedBindingCollection != null)
			{
				realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(null);
			}
			reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(reWASDUI.Views.ContentZoneGamepad.VibrationSettings));
		}

		private DelegateCommand<XBBinding> _showMacroSettings;

		private DelegateCommand _showMouseSettings;

		private DelegateCommand<bool?> _showGamepad;

		private RelayCommand _showMouse;

		private DelegateCommand<bool?> _showVibrationSettings;
	}
}
