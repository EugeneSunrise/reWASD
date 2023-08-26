using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.SecondaryWindows;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using DiscSoft.NET.Common.View.SecondaryWindows.WaitDialog;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using reWASDCommon.Infrastructure;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.SupportedControllers;
using reWASDCommon.Infrastructure.SupportedControllers.Base;
using reWASDUI.DataModels.GamepadActiveProfiles;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Services.Interfaces;
using reWASDUI.Utils.XBUtil;
using reWASDUI.Views;
using reWASDUI.Views.ContentZoneGamepad;
using reWASDUI.Views.VirtualSticks;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils;
using XBEliteWPF.Utils.XBUtilModel;
using XBEliteWPF.Views.OverlayMenu;

namespace reWASDUI.DataModels
{
	public class ConfigVM : ZBindableBase, IComparable
	{
		public Lazy<IGamepadService> GamepadServiceLazy { get; set; }

		public IGameProfilesService GameProfilesService { get; set; }

		public IHttpClientService HttpClientService { get; set; }

		public IGuiHelperService GuiHelperService
		{
			get
			{
				return App.GuiHelperService;
			}
		}

		public ILicensingService LicensingService { get; set; }

		public event PropertyChangedExtendedEventHandler<MainXBBindingCollection> CurrentBindingCollectionChanged;

		public MainXBBindingCollection CurrentBindingCollection
		{
			get
			{
				return this._currentBindingCollection;
			}
			set
			{
				MainXBBindingCollection currentBindingCollection = this._currentBindingCollection;
				if (this.SetProperty<MainXBBindingCollection>(ref this._currentBindingCollection, value, "CurrentBindingCollection"))
				{
					if (currentBindingCollection != null)
					{
						currentBindingCollection.OnMainOrShiftCollectionItemPropertyChangedExtended -= new PropertyChangedExtendedEventHandler(this.RaiseSaveCanExecuteChanged);
						currentBindingCollection.IsChangedModifiedEvent -= this.RaiseSaveCanExecuteChanged;
						MainXBBindingCollection mainXBBindingCollection = currentBindingCollection;
						mainXBBindingCollection.EnableProperyChanged = (EventHandler)Delegate.Remove(mainXBBindingCollection.EnableProperyChanged, new EventHandler(this.RaiseSaveCanExecuteChanged));
					}
					if (this._currentBindingCollection != null)
					{
						this._currentBindingCollection.OnMainOrShiftCollectionItemPropertyChangedExtended += new PropertyChangedExtendedEventHandler(this.RaiseSaveCanExecuteChanged);
						this._currentBindingCollection.IsChangedModifiedEvent += this.RaiseSaveCanExecuteChanged;
						MainXBBindingCollection currentBindingCollection2 = this._currentBindingCollection;
						currentBindingCollection2.EnableProperyChanged = (EventHandler)Delegate.Combine(currentBindingCollection2.EnableProperyChanged, new EventHandler(this.RaiseSaveCanExecuteChanged));
					}
					PropertyChangedExtendedEventHandler<MainXBBindingCollection> currentBindingCollectionChanged = this.CurrentBindingCollectionChanged;
					if (currentBindingCollectionChanged == null)
					{
						return;
					}
					currentBindingCollectionChanged(this, new PropertyChangedExtendedEventArgs<MainXBBindingCollection>("CurrentBindingCollection", currentBindingCollection, value));
				}
			}
		}

		public DelegateCommand SwitchReadOnlyModeCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._switchReadOnlyMode) == null)
				{
					delegateCommand = (this._switchReadOnlyMode = new DelegateCommand(new Action(this.SwitchReadOnlyModeView)));
				}
				return delegateCommand;
			}
		}

		private void SwitchReadOnlyModeView()
		{
			this.IsEditConfigMode = !this.IsEditConfigMode;
			this.OnPropertyChanged("IsEditConfigMode");
		}

		public bool IsEditConfigMode
		{
			get
			{
				return this._isEditConfigMode;
			}
			set
			{
				this.SetProperty<bool>(ref this._isEditConfigMode, value, "IsEditConfigMode");
			}
		}

		public SubConfigData CurrentSubConfigData
		{
			get
			{
				return this._currentSubConfigData;
			}
			set
			{
				SubConfigData currentSubConfigData = this._currentSubConfigData;
				if (currentSubConfigData != null)
				{
					MainXBBindingCollection mainXBBindingCollection = currentSubConfigData.MainXBBindingCollection;
					if (mainXBBindingCollection != null)
					{
						BaseXBBindingCollection realCurrentBeingMappedBindingCollection = mainXBBindingCollection.RealCurrentBeingMappedBindingCollection;
						if (realCurrentBeingMappedBindingCollection != null)
						{
							realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(null);
						}
					}
				}
				if (this.SetProperty<SubConfigData>(ref this._currentSubConfigData, value, "CurrentSubConfigData"))
				{
					if (value == null)
					{
						this.CurrentBindingCollection = null;
					}
					else
					{
						this.CurrentBindingCollection = this.CurrentSubConfigData.MainXBBindingCollection;
					}
					App.EventAggregator.GetEvent<CurrentBindingCollectionWrapperChanged>().Publish(this._currentSubConfigData);
				}
				MainXBBindingCollection currentBindingCollection = this.CurrentBindingCollection;
				if (currentBindingCollection != null)
				{
					currentBindingCollection.ResetCurrentMappingsAndOtherSelectors();
				}
				if (this._changeCurrentControllerOnWrapperChange && this.GameProfilesService.CurrentGame == this.ParentGame)
				{
					this.SetCurrentControllerAccordingToSubconfig();
				}
				if (this._navigateGamepadZoneOnWrapperChange && this.GameProfilesService.CurrentGame == this.ParentGame)
				{
					this.NavigateGamepadZoneAccordingToConfigOrSubconfigState();
				}
				this.SetCurrentShiftModificator();
				MainXBBindingCollection currentBindingCollection2 = this.CurrentBindingCollection;
				if (string.IsNullOrEmpty((currentBindingCollection2 != null) ? currentBindingCollection2.AppName : null))
				{
					ControllerFamily? controllerFamily = ((value != null) ? new ControllerFamily?(value.ControllerFamily) : null);
					ControllerFamily? controllerFamily2 = ((value != null) ? new ControllerFamily?(value.ControllerFamily) : null);
					if (!((controllerFamily.GetValueOrDefault() == controllerFamily2.GetValueOrDefault()) & (controllerFamily != null == (controllerFamily2 != null))))
					{
						goto IL_151;
					}
				}
				if (this.CurrentBindingCollection != null)
				{
					App.EventAggregator.GetEvent<CurrentShiftBindingCollectionChanged>().Publish(null);
				}
				IL_151:
				SupportedGamepad supportedGamepad = ControllersHelper.SupportedControllers.Find(delegate(SupportedControllerInfo x)
				{
					ControllerTypeEnum controllerType = x.ControllerType;
					BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
					ControllerTypeEnum? controllerTypeEnum;
					if (currentGamepad == null)
					{
						controllerTypeEnum = null;
					}
					else
					{
						ControllerVM currentController = currentGamepad.CurrentController;
						controllerTypeEnum = ((currentController != null) ? new ControllerTypeEnum?(currentController.ControllerType) : null);
					}
					ControllerTypeEnum? controllerTypeEnum2 = controllerTypeEnum;
					return (controllerType == controllerTypeEnum2.GetValueOrDefault()) & (controllerTypeEnum2 != null);
				}) as SupportedGamepad;
				if (supportedGamepad != null && !supportedGamepad.IsBackVisible)
				{
					App.GamepadService.CurrentGamepadFlipCommand.Execute(new bool?(false));
				}
			}
		}

		public ConfigData ConfigData
		{
			get
			{
				return this._configData;
			}
		}

		public string ConfigPath
		{
			get
			{
				return this._configPath;
			}
			set
			{
				this.SetProperty<string>(ref this._configPath, value, "ConfigPath");
			}
		}

		public GameVM ParentGame { get; set; }

		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (this.SetProperty<string>(ref this._name, value, "Name"))
				{
					this.EditedName = value;
				}
			}
		}

		public string EditedName
		{
			get
			{
				return this._editedName;
			}
			set
			{
				this.SetProperty<string>(ref this._editedName, value, "EditedName");
			}
		}

		public string GameName
		{
			get
			{
				return this._gameName;
			}
			set
			{
				this.SetProperty<string>(ref this._gameName, value, "GameName");
			}
		}

		public bool IsInEditMode
		{
			get
			{
				return this._isInEditMode;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isInEditMode, value, "IsInEditMode") && !value)
				{
					this.EditedName = this.Name;
				}
			}
		}

		public bool IsLoaded
		{
			get
			{
				return this._isLoaded;
			}
			set
			{
				this.SetProperty<bool>(ref this._isLoaded, value, "IsLoaded");
			}
		}

		public bool? IsLoadedSuccessfully
		{
			get
			{
				return this._isLoadedSuccessfully;
			}
			set
			{
				this.SetProperty<bool?>(ref this._isLoadedSuccessfully, value, "IsLoadedSuccessfully");
			}
		}

		public bool IsEmpty
		{
			get
			{
				ConfigData configData = this.ConfigData;
				return configData != null && configData.IsEmpty(false);
			}
		}

		public bool IsEmptyForApply
		{
			get
			{
				ConfigData configData = this.ConfigData;
				return configData != null && configData.IsEmpty(true);
			}
		}

		public GameChangedResult TryAskUserToSaveChanges(bool canCancel = true)
		{
			if (!this.IsChangedIncludingShiftCollections)
			{
				return 0;
			}
			if (this._tryingAskUserToSaveChanges)
			{
				return 0;
			}
			this._tryingAskUserToSaveChanges = true;
			MessageBoxResult messageBoxResult = DTMessageBox.Show(Application.Current.MainWindow, DTLocalization.GetString(11003), canCancel ? MessageBoxButton.YesNoCancel : MessageBoxButton.YesNo, MessageBoxImage.Question, null);
			this._tryingAskUserToSaveChanges = false;
			if (messageBoxResult == MessageBoxResult.Yes || messageBoxResult == MessageBoxResult.OK)
			{
				return 1;
			}
			if (messageBoxResult == MessageBoxResult.No)
			{
				return 2;
			}
			if (messageBoxResult == MessageBoxResult.Cancel)
			{
				return 3;
			}
			return 0;
		}

		public bool IsUseMouseKeyboardSettingsForAllSubConfigs
		{
			get
			{
				return this._isUseMouseKeyboardSettingsForAllSubConfigs;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isUseMouseKeyboardSettingsForAllSubConfigs, value, "IsUseMouseKeyboardSettingsForAllSubConfigs"))
				{
					this.TryApplyMouseSettingsToAllSubConfigs();
				}
			}
		}

		public void TryApplyMouseSettingsToAllSubConfigs()
		{
			if (this.IsUseMouseKeyboardSettingsForAllSubConfigs)
			{
				ushort mouseAcceleration = this.CurrentBindingCollection.MouseAcceleration;
				ushort mouseDeflection = this.CurrentBindingCollection.MouseDeflection;
				ushort mouseSensitivity = this.CurrentBindingCollection.MouseSensitivity;
				ushort wheelDeflection = this.CurrentBindingCollection.WheelDeflection;
				KeyboardRepeatType virtualKeyboardRepeatRate = this.CurrentBindingCollection.VirtualKeyboardRepeatRate;
				foreach (SubConfigData subConfigData in this.ConfigData)
				{
					subConfigData.MainXBBindingCollection.MouseAcceleration = mouseAcceleration;
					subConfigData.MainXBBindingCollection.MouseDeflection = mouseDeflection;
					subConfigData.MainXBBindingCollection.MouseSensitivity = mouseSensitivity;
					subConfigData.MainXBBindingCollection.WheelDeflection = wheelDeflection;
					subConfigData.MainXBBindingCollection.VirtualKeyboardRepeatRate = virtualKeyboardRepeatRate;
				}
			}
		}

		public bool IsAutodetectEnabledForAnySlot
		{
			get
			{
				bool flag = false;
				if (this.ParentGame == null)
				{
					return false;
				}
				if (!this.ParentGame.IsAutodetect)
				{
					return false;
				}
				AutoGamesDetectionGamepadProfilesCollection autoGamesDetectionGamepadProfileRelations = this.GamepadServiceLazy.Value.AutoGamesDetectionGamepadProfileRelations;
				if (this.GamepadServiceLazy.Value.CurrentGamepad != null && autoGamesDetectionGamepadProfileRelations.ContainsKey(this.ParentGame.Name) && autoGamesDetectionGamepadProfileRelations[this.ParentGame.Name].ContainsKey(this.GamepadServiceLazy.Value.CurrentGamepad.ID))
				{
					using (IEnumerator<KeyValuePair<Slot, GamepadProfile>> enumerator = ((IEnumerable<KeyValuePair<Slot, GamepadProfile>>)autoGamesDetectionGamepadProfileRelations[this.ParentGame.Name][this.GamepadServiceLazy.Value.CurrentGamepad.ID].SlotProfiles).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<Slot, GamepadProfile> slotProfilesValue = enumerator.Current;
							SlotInfo slotInfo = this.GamepadServiceLazy.Value.SlotsInfo.FirstOrDefault((SlotInfo si) => si.Slot == slotProfilesValue.Key);
							if (slotInfo != null && slotInfo.IsAvailable && slotProfilesValue.Value != null && slotProfilesValue.Value.ProfileName == this.Name)
							{
								flag = true;
								break;
							}
						}
					}
				}
				return flag;
			}
		}

		public void AutodetectForConfigChanged()
		{
			this.OnPropertyChanged("IsAutodetectEnabledForAnySlot");
		}

		public bool IsShowKeboardMappings
		{
			get
			{
				return this._isShowKeboardMappings;
			}
			set
			{
				this.SetProperty<bool>(ref this._isShowKeboardMappings, value, "IsShowKeboardMappings");
			}
		}

		public bool IsShowMouseMappings
		{
			get
			{
				return this._isShowMouseMappings;
			}
			set
			{
				this.SetProperty<bool>(ref this._isShowMouseMappings, value, "IsShowMouseMappings");
			}
		}

		public bool IsShowVirtualGamepadMappings
		{
			get
			{
				return this._isShowVirtualGamepadMappings;
			}
			set
			{
				this.SetProperty<bool>(ref this._isShowVirtualGamepadMappings, value, "IsShowVirtualGamepadMappings");
			}
		}

		public bool IsShowUserCommands
		{
			get
			{
				return this._isShowUserCommands;
			}
			set
			{
				this.SetProperty<bool>(ref this._isShowUserCommands, value, "IsShowUserCommands");
			}
		}

		public ConfigTemplate ConfigTemplate
		{
			get
			{
				return this._configTemplate;
			}
			set
			{
				this.SetProperty<ConfigTemplate>(ref this._configTemplate, value, "ConfigTemplate");
			}
		}

		public bool ExistAnyDescriptionsForController(BaseControllerVM controller)
		{
			bool flag = false;
			if (controller == null)
			{
				return false;
			}
			using (IEnumerator<SubConfigData> enumerator = this.ConfigData.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SubConfigData subConfig = enumerator.Current;
					if (controller.ControllerFamily == subConfig.ControllerFamily)
					{
						flag |= !subConfig.IsDescriptionEmpty();
					}
					else
					{
						CompositeControllerVM compositeControllerVM = controller as CompositeControllerVM;
						if (compositeControllerVM != null && compositeControllerVM.BaseControllers.Any((BaseControllerVM item) => item != null && item.ControllerFamily == subConfig.ControllerFamily))
						{
							flag |= !subConfig.IsDescriptionEmpty();
						}
					}
				}
			}
			return flag;
		}

		public bool ExistAnyBindingsForController(BaseControllerVM controller, bool forPrint)
		{
			bool flag = false;
			if (controller == null)
			{
				return false;
			}
			using (IEnumerator<SubConfigData> enumerator = this.ConfigData.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SubConfigData subConfig = enumerator.Current;
					bool flag2 = flag;
					bool flag3;
					if (subConfig.ConfigData.IsVirtualGamepad)
					{
						BaseControllerVM currentGamepad = this.GamepadServiceLazy.Value.CurrentGamepad;
						flag3 = currentGamepad != null && currentGamepad.HasGamepadControllersWithFictiveButtons;
					}
					else
					{
						flag3 = false;
					}
					flag = flag2 || flag3;
					if (controller.ControllerFamily == subConfig.ControllerFamily)
					{
						flag |= (forPrint ? (!subConfig.IsEmptyPrint()) : (!subConfig.IsEmpty()));
					}
					else
					{
						CompositeControllerVM compositeControllerVM = controller as CompositeControllerVM;
						if (compositeControllerVM != null && compositeControllerVM.BaseControllers.Any((BaseControllerVM item) => item != null && item.ControllerFamily == subConfig.ControllerFamily))
						{
							flag |= (forPrint ? (!subConfig.IsEmptyPrint()) : (!subConfig.IsEmpty()));
						}
					}
				}
			}
			return flag;
		}

		public DelegateCommand CreateSubConfigCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._createSubConfig) == null)
				{
					delegateCommand = (this._createSubConfig = new DelegateCommand(new Action(this.CreateSubConfig)));
				}
				return delegateCommand;
			}
		}

		private void CreateSubConfig()
		{
			ControllerFamily controllerFamily = 4;
			IGuiHelperService guiHelperService = App.GuiHelperService;
			MessageBoxResult? messageBoxResult = ((guiHelperService != null) ? new MessageBoxResult?(guiHelperService.AddSubConfigDialog(out controllerFamily)) : null);
			MessageBoxResult? messageBoxResult2 = messageBoxResult;
			MessageBoxResult messageBoxResult3 = MessageBoxResult.OK;
			if (!((messageBoxResult2.GetValueOrDefault() == messageBoxResult3) & (messageBoxResult2 != null)))
			{
				messageBoxResult2 = messageBoxResult;
				messageBoxResult3 = MessageBoxResult.Yes;
				if (!((messageBoxResult2.GetValueOrDefault() == messageBoxResult3) & (messageBoxResult2 != null)))
				{
					return;
				}
			}
			this.ConfigData.Add(new SubConfigData(this.ConfigData, new MainXBBindingCollection(this, controllerFamily), controllerFamily, this.ConfigData.Count((SubConfigData bc) => bc.ControllerFamily == controllerFamily)));
		}

		public DelegateCommand CreateGamepadSubconfigCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._createGamepadSubconfigCommand) == null)
				{
					delegateCommand = (this._createGamepadSubconfigCommand = new DelegateCommand(new Action(this.CreateGamepadSubconfig), new Func<bool>(this.CreateGamepadSubconfigCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void CreateSubConfig(ControllerFamily controllerFamily)
		{
			bool isChanged = this.ConfigData.IsChanged;
			SubConfigData subConfigData = new SubConfigData(this.ConfigData, new MainXBBindingCollection(this, controllerFamily), controllerFamily, this.ConfigData.Count((SubConfigData bc) => bc.ControllerFamily == controllerFamily));
			int num = this.ConfigData.AdditionalData.Count - 1;
			for (int i = 0; i < num; i++)
			{
				ShiftXBBindingCollection shiftXBBindingCollection = subConfigData.MainXBBindingCollection.AddShift(0, -1);
				shiftXBBindingCollection.Description = this.ConfigData[0].MainXBBindingCollection.ShiftXBBindingCollections[i].Description;
				shiftXBBindingCollection.ShiftType = this.ConfigData[0].MainXBBindingCollection.ShiftXBBindingCollections[i].ShiftType;
			}
			if (subConfigData.MainXBBindingCollection != null && this.IsUseMouseKeyboardSettingsForAllSubConfigs && this.ConfigData.Count > 0 && this.ConfigData[0].MainXBBindingCollection != null)
			{
				XBUtils.CopyVirtualSettings(this.ConfigData[0].MainXBBindingCollection, subConfigData.MainXBBindingCollection);
			}
			subConfigData.MainXBBindingCollection.AppName = this.ConfigData[0].MainXBBindingCollection.AppName;
			this.ConfigData.Add(subConfigData);
			this.OnPropertyChanged("ConfigData");
			this.OnAfterSubConfigCreatedInGUI();
			this.ConfigData.IsChanged = isChanged;
		}

		private void CreateGamepadSubconfig()
		{
			this.CreateSubConfig(0);
		}

		private bool CreateGamepadSubconfigCanExecute()
		{
			return !this.GameProfilesService.IsHidingIrrelevantSubConfigs || this.IsFutureConfigRelevantForCurrentGamepad(0);
		}

		public DelegateCommand CreateKeyboardSubconfigCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._createKeyboardSubconfigCommand) == null)
				{
					delegateCommand = (this._createKeyboardSubconfigCommand = new DelegateCommand(new Action(this.CreateKeyboardSubconfig), new Func<bool>(this.CreateKeyboardSubconfigCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void CreateKeyboardSubconfig()
		{
			this.CreateSubConfig(1);
		}

		private bool CreateKeyboardSubconfigCanExecute()
		{
			return !this.GameProfilesService.IsHidingIrrelevantSubConfigs || this.IsFutureConfigRelevantForCurrentGamepad(1);
		}

		public DelegateCommand CreateMouseSubconfigCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._createMouseSubconfigCommand) == null)
				{
					delegateCommand = (this._createMouseSubconfigCommand = new DelegateCommand(new Action(this.CreateMouseSubconfig), new Func<bool>(this.CreateMouseSubconfigCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void CreateMouseSubconfig()
		{
			this.CreateSubConfig(2);
		}

		private bool CreateMouseSubconfigCanExecute()
		{
			return !this.GameProfilesService.IsHidingIrrelevantSubConfigs || this.IsFutureConfigRelevantForCurrentGamepad(2);
		}

		public bool IsFutureConfigRelevantForCurrentGamepad(ControllerFamily family)
		{
			int num = this.ConfigData.Count((SubConfigData m) => m.ControllerFamily == family);
			BaseControllerVM currentGamepad = this.GamepadServiceLazy.Value.CurrentGamepad;
			if (currentGamepad == null || currentGamepad.IsUnsupportedControllerType)
			{
				return true;
			}
			if (currentGamepad.IsCompositeDevice)
			{
				CompositeControllerVM compositeControllerVM = currentGamepad as CompositeControllerVM;
				if (compositeControllerVM == null)
				{
					return false;
				}
				if (num > compositeControllerVM.BaseControllers.Count - 1)
				{
					return false;
				}
				int num2 = -1;
				foreach (BaseControllerVM baseControllerVM in compositeControllerVM.BaseControllers)
				{
					if (baseControllerVM != null && baseControllerVM.ControllerFamily == family)
					{
						num2++;
						if (num2 == num)
						{
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		[JsonConstructor]
		public ConfigVM(string path, string name, GameVM parentGame)
		{
			this.GameProfilesService = App.GameProfilesService;
			this.GamepadServiceLazy = App.GamepadServiceLazy;
			this.LicensingService = App.LicensingService;
			this._configFileService = App.ConfigFileService;
			this.HttpClientService = App.HttpClientService;
			this.ParentGame = parentGame;
			this.ConfigPath = path;
			this.Name = (string.IsNullOrEmpty(name) ? Path.GetFileNameWithoutExtension(this.ConfigPath) : name);
		}

		public ConfigVM(string configPath, IConfigFileService ifs, IGameProfilesService gps, Lazy<IGamepadService> gsLazy, ILicensingService ls, IHttpClientService https, GameVM parentGame)
		{
			this.GameProfilesService = gps;
			this.GamepadServiceLazy = gsLazy;
			this.HttpClientService = https;
			this.LicensingService = ls;
			this._configFileService = ifs;
			this.ParentGame = parentGame;
			this.ConfigPath = configPath;
			this._name = Path.GetFileNameWithoutExtension(this.ConfigPath);
		}

		public void InitConfig()
		{
			this._changeCurrentControllerOnWrapperChange = false;
			this._navigateGamepadZoneOnWrapperChange = false;
			this.CreateBindingCollection(false);
			this._changeCurrentControllerOnWrapperChange = true;
			this._navigateGamepadZoneOnWrapperChange = true;
		}

		public void ChangeCurrentMainWrapperAccordingToCurrentController()
		{
			this.ChangeCurrentMainWrapperAccordingToControllerVM(this.GamepadServiceLazy.Value.CurrentGamepad, true);
		}

		public void ChangeCurrentMainWrapperAccordingToControllerVM(BaseControllerVM currentController, bool navigateGamepadZone = true)
		{
			if (this.ConfigData == null)
			{
				return;
			}
			if (currentController == null)
			{
				this.CurrentSubConfigData = this.ConfigData.FirstOrDefault<SubConfigData>();
				return;
			}
			switch (currentController.ControllerFamily)
			{
			case 0:
				this.CurrentSubConfigData = this.ConfigData.FindGamepadCollection(0, false);
				return;
			case 1:
				this.CurrentSubConfigData = this.ConfigData.FindKeyboardCollection(0, false);
				return;
			case 2:
				this.CurrentSubConfigData = this.ConfigData.FindMouseCollection(0, false);
				return;
			case 3:
			{
				CompositeControllerVM compositeControllerVM = currentController as CompositeControllerVM;
				if (compositeControllerVM != null)
				{
					BaseControllerVM baseControllerVM = compositeControllerVM.CurrentController;
					if (baseControllerVM == null)
					{
						baseControllerVM = compositeControllerVM.BaseControllers.FirstOrDefault((BaseControllerVM c) => c != null);
					}
					if (baseControllerVM != null)
					{
						if (baseControllerVM.ControllerFamily == null)
						{
							this.CurrentSubConfigData = this.ConfigData.FindGamepadCollection(0, false);
							return;
						}
						if (baseControllerVM.ControllerFamily == 1)
						{
							this.CurrentSubConfigData = this.ConfigData.FindKeyboardCollection(0, false);
							return;
						}
						if (baseControllerVM.ControllerFamily == 2)
						{
							this.CurrentSubConfigData = this.ConfigData.FindMouseCollection(0, false);
						}
					}
				}
				return;
			}
			case 4:
				this.CurrentSubConfigData = this.ConfigData.FindGamepadCollection(0, false);
				return;
			default:
				return;
			}
		}

		public void NavigateGamepadZoneAccordingToConfigOrSubconfigState()
		{
			this.ConfigData.ForEach(delegate(SubConfigData item)
			{
				item.MainXBBindingCollection.IsOverlayMenuModeView = false;
				item.MainXBBindingCollection.ShiftXBBindingCollections.ForEach(delegate(ShiftXBBindingCollection shiftCollection)
				{
					shiftCollection.IsOverlayMenuModeView = false;
				});
			});
			MainXBBindingCollection currentBindingCollection = this.CurrentBindingCollection;
			if (currentBindingCollection != null && currentBindingCollection.IsMaskModeView)
			{
				reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(MaskView));
				return;
			}
			MainXBBindingCollection currentBindingCollection2 = this.CurrentBindingCollection;
			if (currentBindingCollection2 != null && currentBindingCollection2.IsVirtualStickSettingsModeView)
			{
				reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(VirtualSticksSettingsView));
				return;
			}
			MainXBBindingCollection currentBindingCollection3 = this.CurrentBindingCollection;
			if (currentBindingCollection3 != null && currentBindingCollection3.IsLEDSettingsView)
			{
				reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(LedSettingsView));
				return;
			}
			SubConfigData currentSubConfigData = this.CurrentSubConfigData;
			ControllerFamily? controllerFamily;
			if (currentSubConfigData == null)
			{
				BaseControllerVM currentGamepad = this.GamepadServiceLazy.Value.CurrentGamepad;
				controllerFamily = ((currentGamepad != null) ? new ControllerFamily?(currentGamepad.ControllerFamily) : null);
			}
			else
			{
				controllerFamily = new ControllerFamily?(currentSubConfigData.ControllerFamily);
			}
			XBUtils.NavigateGamepadZoneForControllerFamily(controllerFamily);
		}

		public async Task<ConfigData> GetConfigData()
		{
			if (!this.IsLoaded)
			{
				await this.ReadConfigFromJsonAsync(false);
			}
			return this.ConfigData;
		}

		public void ReleaseConfig()
		{
			this.IsLoaded = false;
			this.IsLoadedSuccessfully = null;
			if (this.ConfigData != null)
			{
				this._configData = null;
			}
			this.CurrentBindingCollection = null;
			this.CurrentSubConfigData = null;
		}

		private void SetCurrentShiftModificator()
		{
			IGameProfilesService gameProfilesService = this.GameProfilesService;
			int? num = ((gameProfilesService != null) ? new int?(gameProfilesService.CurrentShiftModificator) : null);
			MainXBBindingCollection currentBindingCollection = this.CurrentBindingCollection;
			int? num2;
			if (currentBindingCollection == null)
			{
				num2 = null;
			}
			else
			{
				ShiftXBBindingCollection currentShiftXBBindingCollection = currentBindingCollection.CurrentShiftXBBindingCollection;
				num2 = ((currentShiftXBBindingCollection != null) ? new int?(currentShiftXBBindingCollection.ShiftIndex) : null);
			}
			int? num3 = num2;
			if (!((num.GetValueOrDefault() == num3.GetValueOrDefault()) & (num != null == (num3 != null))))
			{
				this.GameProfilesService.ChangeCurrentShiftCollection(new int?(this.GameProfilesService.CurrentShiftModificator), true);
			}
		}

		private void SetCurrentControllerAccordingToSubconfig()
		{
			CompositeControllerVM compositeControllerVM = this.GamepadServiceLazy.Value.CurrentGamepad as CompositeControllerVM;
			if (compositeControllerVM != null && this.CurrentSubConfigData != null)
			{
				compositeControllerVM.SetCurrentControllerAccordingControllerFamilyIndex(this.CurrentSubConfigData.ControllerFamily, this.CurrentSubConfigData.Index);
			}
		}

		private void FinishCreateBindingCollection()
		{
			this._configData.IsChangedModifiedEvent += this.RaiseSaveCanExecuteChanged;
		}

		public void CreateBindingCollection(bool empty = false)
		{
			this._configData = XBUtils.CreateDefaultCollectionXBBindingWrappers(this, empty);
			if (!empty)
			{
				this.FinishCreateBindingCollection();
			}
		}

		public bool IsLoading { get; set; }

		public async Task<bool> ReadConfigFromJsonAsync(bool verbose = true)
		{
			bool flag;
			if (this.IsLoading)
			{
				flag = false;
			}
			else
			{
				this.IsLoading = true;
				this.CreateBindingCollection(true);
				bool rSuccess = false;
				try
				{
					TaskAwaiter<bool> taskAwaiter = this._configFileService.ParseConfigFile(this.GameName, this.Name, this.ConfigPath, this._configData, verbose, null).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						TaskAwaiter<bool> taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (!taskAwaiter.GetResult())
					{
						this.IsLoading = false;
						return false;
					}
				}
				catch (UnauthorizedAccessException ex)
				{
					DTMessageBox.Show(string.Format(DTLocalization.GetString(11134), DTLocalization.GetString(4276), ex.Message), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
				}
				this.IsLoaded = true;
				bool flag2 = true;
				int num = 0;
				foreach (SubConfigData subConfigData in this.ConfigData)
				{
					if (!string.IsNullOrEmpty(this.GameName))
					{
						subConfigData.MainXBBindingCollection.AppName = this.GameName;
					}
					bool flag3 = false;
					try
					{
						subConfigData.MainXBBindingCollection.SaveChanges(verbose, ref flag3);
						rSuccess = true;
					}
					catch (Exception ex2)
					{
						if (verbose)
						{
							DTMessageBox.Show(string.Format(DTLocalization.GetString(11134), this.ConfigPath, ex2.Message), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
						}
						Tracer.TraceException(ex2, "ReadConfigFromJsonAsync");
					}
					if (flag2)
					{
						int hashCode = string.Format("{0} {1} {2} {3} {4}", new object[]
						{
							subConfigData.MainXBBindingCollection.MouseAcceleration,
							subConfigData.MainXBBindingCollection.MouseDeflection,
							subConfigData.MainXBBindingCollection.MouseSensitivity,
							subConfigData.MainXBBindingCollection.WheelDeflection,
							subConfigData.MainXBBindingCollection.VirtualKeyboardRepeatRate
						}).GetHashCode();
						if (num == 0)
						{
							num = hashCode;
						}
						else
						{
							flag2 = hashCode == num;
						}
					}
					subConfigData.MainXBBindingCollection.SaveChanges(verbose, ref flag3);
					subConfigData.MainXBBindingCollection.OnAfterCollectionRead();
				}
				this.FinishCreateBindingCollection();
				this.ConfigData.ResetIsChanged();
				this.ConfigData.CheckVirtualMappingsExist();
				this._isUseMouseKeyboardSettingsForAllSubConfigs = flag2;
				this.OnPropertyChanged("IsUseMouseKeyboardSettingsForAllSubConfigs");
				this.OnPropertyChanged("ConfigData");
				this.IsLoadedSuccessfully = new bool?(rSuccess);
				this.IsLoading = false;
				flag = rSuccess;
			}
			return flag;
		}

		public async Task<bool> SaveConfig(bool forced)
		{
			bool flag = false;
			bool flag2;
			if (this.CurrentBindingCollection == null || (!forced && !this.CurrentBindingCollection.CanSave(true, ref flag)))
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = await this.SaveConfigToJson();
				this.ConfigData.IsChanged = false;
				this.SaveConfigCommand.RaiseCanExecuteChanged();
				this.PrintBindingsCommand.RaiseCanExecuteChanged();
				flag2 = flag3;
			}
			return flag2;
		}

		public async Task<bool> SaveConfigToJson()
		{
			bool rSuccess = false;
			try
			{
				if (!this._configData.IsExternal)
				{
					string text = ((this.ParentGame != null) ? this.ParentGame.Name : this.GameName);
					Lazy<IGamepadService> gamepadServiceLazy = this.GamepadServiceLazy;
					if (gamepadServiceLazy != null)
					{
						IGamepadService value = gamepadServiceLazy.Value;
						if (value != null)
						{
							ExternalDeviceRelationsHelper externalDeviceRelationsHelper = value.ExternalDeviceRelationsHelper;
							if (externalDeviceRelationsHelper != null)
							{
								externalDeviceRelationsHelper.RemoveConfigRelations(text, this.Name);
							}
						}
					}
				}
				RegistryHelper.SetValue("Config", "VirtualGamepadType", this.ConfigData.VirtualGamepadType);
				foreach (SubConfigData subConfigData in this.ConfigData)
				{
					subConfigData.MainXBBindingCollection.PrepareForSave();
					bool isGamepad = subConfigData.IsGamepad;
				}
				await this._configFileService.SaveConfigFile(this.GameName, this.Name, this._configData);
				bool flag = false;
				bool flag2 = true;
				foreach (SubConfigData subConfigData2 in this.ConfigData)
				{
					flag2 = subConfigData2.MainXBBindingCollection.SaveChanges(true, ref flag) && flag2;
				}
				rSuccess = flag2;
			}
			catch (Exception ex)
			{
				DSUtils.CheckWinDefenderAndShowMessage(string.Format(DTLocalization.GetString(11135), this.ConfigPath, ex.Message));
				Tracer.TraceException(ex, "SaveConfigToJson");
			}
			return rSuccess;
		}

		private void RaiseSaveCanExecuteChanged()
		{
			this.SaveConfigCommand.RaiseCanExecuteChanged();
			this.PrintBindingsCommand.RaiseCanExecuteChanged();
			this.ConfigData.RaiseRefreshBoundToWholeControllerProperties();
			this.OnPropertyChanged("IsEmpty");
		}

		private void RaiseSaveCanExecuteChanged(object sender, EventArgs eventArgs)
		{
			if (this._currentBindingCollection != null && this._currentBindingCollection.IsFirePropertyChanged)
			{
				this.RaiseSaveCanExecuteChanged();
			}
		}

		public void UpdateCreateSubConfigsCommandsCanExecute()
		{
			this.CreateGamepadSubconfigCommand.RaiseCanExecuteChanged();
			this.CreateKeyboardSubconfigCommand.RaiseCanExecuteChanged();
			this.CreateMouseSubconfigCommand.RaiseCanExecuteChanged();
			this.GameProfilesService.RaiseCanAddAnySubConfigChanged();
		}

		public bool IsChangedIncludingShiftCollections
		{
			get
			{
				ConfigData configData = this.ConfigData;
				if (configData != null && configData.IsChanged)
				{
					return true;
				}
				ConfigData configData2 = this.ConfigData;
				if (configData2 == null)
				{
					return false;
				}
				return configData2.Any((SubConfigData bc) => bc.MainXBBindingCollection.IsChangedIncludingShiftCollections);
			}
		}

		public void ResetChangedState()
		{
			this.ConfigData.IsChanged = false;
			foreach (SubConfigData subConfigData in this.ConfigData)
			{
				subConfigData.MainXBBindingCollection.IsChanged = false;
				foreach (ShiftXBBindingCollection shiftXBBindingCollection in subConfigData.MainXBBindingCollection.ShiftXBBindingCollections)
				{
					shiftXBBindingCollection.IsChanged = false;
				}
			}
		}

		public void RefreshShiftModificators()
		{
			this.OnAfterSubConfigCreatedInGUI();
		}

		private void OnAfterSubConfigCreatedInGUI()
		{
			this.UpdateCreateSubConfigsCommandsCanExecute();
			this.ConfigData.IsChanged = true;
		}

		public int CompareTo(object obj)
		{
			ConfigVM configVM = (ConfigVM)obj;
			return string.Compare(this.Name, configVM.Name, StringComparison.CurrentCulture);
		}

		public async Task<bool> ReadConfigFromJsonIfNotLoaded()
		{
			Tracer.TraceWrite("ReadConfigFromJSONIfNotLoaded", false);
			while (this.IsLoading)
			{
				await Task.Delay(10);
			}
			if (!this.IsLoaded && this.IsLoadedSuccessfully == null)
			{
				Tracer.TraceWrite("Config is not loaded try to load", false);
				this.IsLoadedSuccessfully = new bool?(await this.ReadConfigFromJsonAsync(false));
				Tracer.TraceWrite("Config is not loaded after try to load", false);
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Config load success ");
			defaultInterpolatedStringHandler.AppendFormatted<bool?>(this.IsLoadedSuccessfully);
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			bool? isLoadedSuccessfully = this.IsLoadedSuccessfully;
			bool flag = true;
			return (isLoadedSuccessfully.GetValueOrDefault() == flag) & (isLoadedSuccessfully != null);
		}

		public void ApplyPreset(ConfigVM preset)
		{
			if (MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(12569), MessageBoxButton.YesNo, MessageBoxImage.Question, "ConfirmAddOrDeleteShift", MessageBoxResult.Yes, false, 0.0, null, null, null, null, null, null) == MessageBoxResult.Yes)
			{
				WaitDialog.ShowDialogStatic(DTLocalization.GetString(5238), null, null, false, false, null, null);
				int shiftIndex = this.AddShift();
				if (shiftIndex == -1)
				{
					DTMessageBox.Show(string.Format(DTLocalization.GetString(12576), Array.Empty<object>()), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
					WaitDialog.TryCloseWaitDialog();
					return;
				}
				ConfigData configData = XBUtils.CreateConfigData(true);
				preset.ConfigData.CopyToModel(configData);
				this.ConfigData.ForEach(delegate(SubConfigData item)
				{
					item.MainXBBindingCollection.ShiftXBBindingCollections[shiftIndex - 1].Description = preset.Name;
				});
				reWASDApplicationCommands.FillShift(configData, 1, this, shiftIndex, false, true);
				reWASDApplicationCommands.FillPresetDirections(configData, 0, this, shiftIndex, true);
				this.ConfigData.CheckVirtualMappingsExist();
				WaitDialog.TryCloseWaitDialog();
				SenderGoogleAnalytics.SendMessageEvent("Preset", "Use", preset.Name, (long)App.GameProfilesService.PresetsCollection.Count, false);
				SenderGoogleAnalytics.SendMessageEvent("Preset", "Amount", App.GameProfilesService.PresetsCollection.Count.ToString(), -1L, false);
			}
		}

		public DelegateCommand AddShiftCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._addShiftCommand) == null)
				{
					delegateCommand = (this._addShiftCommand = new DelegateCommand(delegate
					{
						this.AddSimpleShift();
					}, delegate
					{
						ConfigData configData = this.ConfigData;
						return configData != null && configData.LayersCount < 11;
					}));
				}
				return delegateCommand;
			}
		}

		private int AddSimpleShift()
		{
			int num = -1;
			if (MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(12569), MessageBoxButton.YesNo, MessageBoxImage.Question, "ConfirmAddOrDeleteShift", MessageBoxResult.Yes, false, 0.0, null, null, null, null, null, null) == MessageBoxResult.Yes)
			{
				WaitDialog.ShowDialogStatic(DTLocalization.GetString(5238), null, null, false, false, null, null);
				num = this.AddShift();
				if (num == -1)
				{
					DTMessageBox.Show(string.Format(DTLocalization.GetString(12576), Array.Empty<object>()), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
				}
				else
				{
					SenderGoogleAnalytics.SendMessageEvent("GUI", "CreatedShifts", (this.ConfigData.LayersCount - 1).ToString(), -1L, false);
				}
				WaitDialog.TryCloseWaitDialog();
			}
			return num;
		}

		private bool CanAddShift()
		{
			if (!Constants.DynamicShifts)
			{
				int num = (Constants.CreateOverlayShift ? 6 : 5);
				if (this.ConfigData.LayersCount >= num)
				{
					return false;
				}
			}
			return this.ConfigData.LayersCount < 11;
		}

		private int AddShift()
		{
			if (!this.CanAddShift())
			{
				return -1;
			}
			int num = this.ConfigData.AddShift(0);
			this.GameProfilesService.ChangeCurrentShiftCollection(new int?(num), true);
			PubSubEvent<ShiftXBBindingCollection> @event = App.EventAggregator.GetEvent<CurrentShiftBindingCollectionChanged>();
			GameVM currentGame = this.GameProfilesService.CurrentGame;
			ShiftXBBindingCollection shiftXBBindingCollection;
			if (currentGame == null)
			{
				shiftXBBindingCollection = null;
			}
			else
			{
				ConfigVM currentConfig = currentGame.CurrentConfig;
				if (currentConfig == null)
				{
					shiftXBBindingCollection = null;
				}
				else
				{
					MainXBBindingCollection currentBindingCollection = currentConfig.CurrentBindingCollection;
					shiftXBBindingCollection = ((currentBindingCollection != null) ? currentBindingCollection.CurrentShiftXBBindingCollection : null);
				}
			}
			@event.Publish(shiftXBBindingCollection);
			this.AddShiftCommand.RaiseCanExecuteChanged();
			return num;
		}

		private async void ApplyToCurrentPreset()
		{
			MessageBoxResult result = MessageBoxResult.Yes;
			if (App.GameProfilesService.RealCurrentBeingMappedBindingCollection.IsCollectionHasMappings)
			{
				result = DTMessageBox.Show(Application.Current.MainWindow, string.Format(DTLocalization.GetString(12593), App.GameProfilesService.RealCurrentBeingMappedBindingCollection.Description), DTLocalization.GetString(5010), MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation, null, false, 0.0, MessageBoxResult.None, null, null, null);
			}
			if (result != MessageBoxResult.Cancel)
			{
				TaskAwaiter<bool> taskAwaiter = this.ReadConfigFromJsonIfNotLoaded().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (!taskAwaiter.GetResult())
				{
					DTMessageBox.Show(DTLocalization.GetString(12578), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
					await App.GameProfilesService.FillPresetsCollection(true);
				}
				else
				{
					ConfigData configData = XBUtils.CreateConfigData(true);
					this.ConfigData.CopyToModel(configData);
					reWASDApplicationCommands.FillShift(configData, 1, App.GameProfilesService.CurrentGame.CurrentConfig, App.GameProfilesService.RealCurrentBeingMappedBindingCollection.ShiftIndex, true, result == MessageBoxResult.Yes);
					reWASDApplicationCommands.FillPresetDirections(configData, 0, App.GameProfilesService.CurrentGame.CurrentConfig, App.GameProfilesService.RealCurrentBeingMappedBindingCollection.ShiftIndex, result == MessageBoxResult.Yes);
					this.ConfigData.CheckVirtualMappingsExist();
					SenderGoogleAnalytics.SendMessageEvent("Preset", "Use", this.Name, (long)App.GameProfilesService.PresetsCollection.Count, false);
					SenderGoogleAnalytics.SendMessageEvent("Preset", "Amount", App.GameProfilesService.PresetsCollection.Count.ToString(), -1L, false);
				}
			}
		}

		private async void DeletePreset()
		{
			if (MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(12574), MessageBoxButton.YesNo, MessageBoxImage.Question, "ConfirmAddOrDeletePresetShift", MessageBoxResult.Yes, false, 0.0, null, null, null, null, null, null) == MessageBoxResult.Yes)
			{
				WaitDialog.ShowDialogStatic(DTLocalization.GetString(5238), null, null, false, false, null, null);
				IGuiHelperService guiHelperService = App.GuiHelperService;
				await ((guiHelperService != null) ? guiHelperService.RemovePresetExecute(this) : null);
				WaitDialog.TryCloseWaitDialog();
			}
		}

		public DelegateCommand ApplyToCurrentCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._applyToCurrentCommand) == null)
				{
					delegateCommand = (this._applyToCurrentCommand = new DelegateCommand(delegate
					{
						this.ApplyToCurrentPreset();
					}));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand RemovePresetCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._removePresetCommand) == null)
				{
					delegateCommand = (this._removePresetCommand = new DelegateCommand(delegate
					{
						this.DeletePreset();
					}));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand RenamePresetCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._renamePresetCommand) == null)
				{
					delegateCommand = (this._renamePresetCommand = new DelegateCommand(delegate
					{
						IGuiHelperService guiHelperService = App.GuiHelperService;
						if (guiHelperService == null)
						{
							return;
						}
						guiHelperService.RenamePresetExecute(this);
					}));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand DeleteConfigCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._deleteConfig) == null)
				{
					delegateCommand = (this._deleteConfig = new DelegateCommand(delegate
					{
						IGuiHelperService guiHelperService = App.GuiHelperService;
						if (guiHelperService == null)
						{
							return;
						}
						guiHelperService.DeleteConfigExecute(this);
					}));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand CloneConfigCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._cloneConfig) == null)
				{
					delegateCommand = (this._cloneConfig = new DelegateCommand(delegate
					{
						IGuiHelperService guiHelperService = App.GuiHelperService;
						if (guiHelperService == null)
						{
							return;
						}
						guiHelperService.CloneConfigExecute(this);
					}));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand OpenConfigCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._openConfig) == null)
				{
					delegateCommand = (this._openConfig = new DelegateCommand(delegate
					{
						IGuiHelperService guiHelperService = App.GuiHelperService;
						if (guiHelperService == null)
						{
							return;
						}
						guiHelperService.OpenConfigExecute(this);
					}));
				}
				return delegateCommand;
			}
		}

		public RelayCommand ClearConfigCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._clearConfig) == null)
				{
					relayCommand = (this._clearConfig = new RelayCommand(delegate
					{
						IGuiHelperService guiHelperService = App.GuiHelperService;
						if (guiHelperService == null)
						{
							return;
						}
						guiHelperService.ClearConfigExecute(this);
					}, new Func<bool>(this.ClearConfigCommandCanExecute)));
				}
				return relayCommand;
			}
		}

		private bool ClearConfigCommandCanExecute()
		{
			if (this.ConfigData == null)
			{
				return false;
			}
			if (this.ConfigData.IsBoundToGamepad)
			{
				return true;
			}
			foreach (SubConfigData subConfigData in this.ConfigData)
			{
				if (!subConfigData.IsMainSubConfig)
				{
					return true;
				}
				if (subConfigData.MainXBBindingCollection.IsCollectionHasMappings)
				{
					return true;
				}
				if (!subConfigData.MainXBBindingCollection.IsControllerBindingsEmptyWithoutStandart)
				{
					return true;
				}
				if (subConfigData.MainXBBindingCollection.IsHardwareChangesPresent)
				{
					return true;
				}
				if (!subConfigData.MainXBBindingCollection.GamepadVibrationMainLeft.IsDefault() || !subConfigData.MainXBBindingCollection.GamepadVibrationMainRight.IsDefault() || !subConfigData.MainXBBindingCollection.GamepadVibrationTriggerLeft.IsDefault() || !subConfigData.MainXBBindingCollection.GamepadVibrationTriggerRight.IsDefault())
				{
					return true;
				}
				if (subConfigData.MainXBBindingCollection.VirtualDeviceSettings.VirtualLeftStick.IsVirtualStickSettingsNonDefault(true) || subConfigData.MainXBBindingCollection.VirtualDeviceSettings.VirtualRightStick.IsVirtualStickSettingsNonDefault(true) || subConfigData.MainXBBindingCollection.VirtualDeviceSettings.VirtualGyro.IsVirtualGyroSettingsNonDefault())
				{
					return true;
				}
				VirtualGamepadType virtualGamepadType;
				Enum.TryParse<VirtualGamepadType>(RegistryHelper.GetValue("Config", "VirtualGamepadType", 0, false).ToString(), out virtualGamepadType);
				if (subConfigData.ConfigData.IsExternal || subConfigData.ConfigData.IsUdpPresent || subConfigData.ConfigData.IsVirtualUsbHubPresent || subConfigData.ConfigData.VirtualGamepadType != virtualGamepadType)
				{
					return true;
				}
				foreach (ShiftXBBindingCollection shiftXBBindingCollection in subConfigData.MainXBBindingCollection.ShiftXBBindingCollections)
				{
					if (shiftXBBindingCollection == null)
					{
						break;
					}
					if (shiftXBBindingCollection.IsCollectionHasMappings || !shiftXBBindingCollection.IsControllerBindingsEmptyWithoutStandart || shiftXBBindingCollection.IsHardwareChangesPresent)
					{
						return true;
					}
					if (shiftXBBindingCollection.VirtualDeviceSettings.VirtualLeftStick.IsVirtualStickSettingsNonDefault(true) || shiftXBBindingCollection.VirtualDeviceSettings.VirtualRightStick.IsVirtualStickSettingsNonDefault(true) || shiftXBBindingCollection.VirtualDeviceSettings.VirtualGyro.IsVirtualGyroSettingsNonDefault())
					{
						return true;
					}
				}
				if (Constants.CreateOverlayShift)
				{
					OverlayMenuVM overlayMenu = this.ConfigData.OverlayMenu;
					if (((overlayMenu != null) ? overlayMenu.Circle : null) != null)
					{
						ObservableCollection<SectorItem> sectors = this.ConfigData.OverlayMenu.Circle.Sectors;
						if (sectors != null && sectors.Count > 0)
						{
							using (IEnumerator<SectorItem> enumerator3 = this.ConfigData.OverlayMenu.Circle.Sectors.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									if (enumerator3.Current.IsNonDefalut())
									{
										return true;
									}
								}
							}
						}
						if (!this.ConfigData.OverlayMenu.Circle.IsTintedBackground || this.ConfigData.OverlayMenu.Circle.TintBackground != 40)
						{
							return true;
						}
						if (this.ConfigData.OverlayMenu.Circle.IsDelayBeforeOpening || this.ConfigData.OverlayMenu.Circle.DelayBeforeOpening != 500)
						{
							return true;
						}
						if (this.ConfigData.OverlayMenu.Circle.Scale != 60)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public DelegateCommand ShareConfigCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._shareConfig) == null)
				{
					delegateCommand = (this._shareConfig = new DelegateCommand(delegate
					{
						IGuiHelperService guiHelperService = App.GuiHelperService;
						if (guiHelperService == null)
						{
							return;
						}
						guiHelperService.ShareConfigExecute(this);
					}));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand RenameConfigCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._renameConfig) == null)
				{
					delegateCommand = (this._renameConfig = new DelegateCommand(delegate
					{
						this.IsInEditMode = true;
					}));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand SaveConfigCommand
		{
			get
			{
				if (this._saveConfig == null)
				{
					this._saveConfig = new DelegateCommand(async delegate
					{
						await this.SaveConfig(false);
					}, new Func<bool>(this.SaveConfigCanExecute));
				}
				return this._saveConfig;
			}
		}

		private bool SaveConfigCanExecute()
		{
			return this.ConfigData != null && this.IsChangedIncludingShiftCollections;
		}

		public DelegateCommand PrintBindingsCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._printBindingsCommand) == null)
				{
					delegateCommand = (this._printBindingsCommand = new DelegateCommand(new Action(this.PrintBindings), new Func<bool>(this.PrintBindingsCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void PrintBindings()
		{
			GameVM currentGame = this.GameProfilesService.CurrentGame;
			if (((currentGame != null) ? currentGame.CurrentConfig : null) == null)
			{
				return;
			}
			if (this.IsChangedIncludingShiftCollections)
			{
				this.SaveConfigCommand.Execute();
			}
			IGuiHelperService guiHelperService = App.GuiHelperService;
			if (guiHelperService == null)
			{
				return;
			}
			guiHelperService.PrintConfigExecute(this);
		}

		private bool PrintBindingsCanExecute()
		{
			GameVM currentGame = this.GameProfilesService.CurrentGame;
			ConfigVM configVM = ((currentGame != null) ? currentGame.CurrentConfig : null);
			if (configVM == null || configVM.ConfigData == null || this.GamepadServiceLazy.Value.CurrentGamepad == null)
			{
				return false;
			}
			ControllerFamily controllerFamily = this.GamepadServiceLazy.Value.CurrentGamepad.ControllerFamily;
			List<BaseControllerVM> list = null;
			if (controllerFamily == 3)
			{
				list = (this.GamepadServiceLazy.Value.CurrentGamepad as CompositeControllerVM).BaseControllers;
			}
			using (IEnumerator<SubConfigData> enumerator = configVM.ConfigData.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SubConfigData subconfig = enumerator.Current;
					if (subconfig.ConfigData.IsVirtualGamepad)
					{
						BaseControllerVM currentGamepad = this.GamepadServiceLazy.Value.CurrentGamepad;
						if (currentGamepad != null && currentGamepad.HasGamepadControllersWithFictiveButtons)
						{
							goto IL_108;
						}
					}
					if ((subconfig.ControllerFamily != controllerFamily && (controllerFamily != 3 || !list.Any((BaseControllerVM ct) => ct != null && subconfig.ControllerFamily == ct.ControllerFamily))) || (subconfig.IsEmptyPrint() && subconfig.IsDescriptionEmpty()))
					{
						continue;
					}
					IL_108:
					return true;
				}
			}
			return false;
		}

		private string _name;

		private string _editedName;

		private string _gameName;

		private string _configPath;

		private bool _isInEditMode;

		private bool _isLoaded;

		private bool? _isLoadedSuccessfully;

		private bool _isUseMouseKeyboardSettingsForAllSubConfigs;

		private IConfigFileService _configFileService;

		private MainXBBindingCollection _currentBindingCollection;

		private SubConfigData _currentSubConfigData;

		private ConfigData _configData;

		private bool _isShowKeboardMappings = true;

		private bool _isShowMouseMappings = true;

		private bool _isShowVirtualGamepadMappings = true;

		private bool _isShowUserCommands = true;

		private ConfigTemplate _configTemplate;

		private bool _changeCurrentControllerOnWrapperChange = true;

		private bool _navigateGamepadZoneOnWrapperChange = true;

		private bool _isEditConfigMode = true;

		private DelegateCommand _switchReadOnlyMode;

		private bool _tryingAskUserToSaveChanges;

		private DelegateCommand _createSubConfig;

		private DelegateCommand _createGamepadSubconfigCommand;

		private DelegateCommand _createKeyboardSubconfigCommand;

		private DelegateCommand _createMouseSubconfigCommand;

		private DelegateCommand _addShiftCommand;

		private DelegateCommand _applyToCurrentCommand;

		private DelegateCommand _removePresetCommand;

		private DelegateCommand _renamePresetCommand;

		private DelegateCommand _deleteConfig;

		private DelegateCommand _cloneConfig;

		private DelegateCommand _openConfig;

		private RelayCommand _clearConfig;

		private DelegateCommand _shareConfig;

		private DelegateCommand _renameConfig;

		private DelegateCommand _saveConfig;

		private DelegateCommand _printBindingsCommand;
	}
}
