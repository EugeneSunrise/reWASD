using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Prism.Commands;
using Prism.Regions;
using reWASDUI.Infrastructure;
using reWASDUI.License.Features;
using reWASDUI.Services.Interfaces;
using reWASDUI.Views;
using XBEliteWPF.License.Licensing.ComStructures;
using XBEliteWPF.License.Licensing.Enums;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.License
{
	public class LicenseMainVM : ZBindable, INavigationAware
	{
		public event LicenseServiceDelegates.LicenseActivatedDelegate OnLicenseActivated;

		public string Serial
		{
			get
			{
				return this._serial;
			}
			set
			{
				if (this._serial == value)
				{
					return;
				}
				this._serial = value;
				this.OnPropertyChanged("Serial");
				this.OnPropertyChanged("IsPaidUser");
			}
		}

		public string HardWareID
		{
			get
			{
				return this._hardwareid;
			}
			set
			{
				if (this._hardwareid == value)
				{
					return;
				}
				this._hardwareid = value;
				this.OnPropertyChanged("HardWareID");
			}
		}

		public bool IsPaidUser
		{
			get
			{
				return !string.IsNullOrEmpty(this.Serial);
			}
		}

		public bool NewVersionAvailable
		{
			get
			{
				return this._newVersionAvailable;
			}
			set
			{
				this.SetProperty<bool>(ref this._newVersionAvailable, value, "NewVersionAvailable");
			}
		}

		public ICommand ChangeLicenseCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._changeLicenseCommand) == null)
				{
					relayCommand = (this._changeLicenseCommand = new RelayCommand(new Action(this.ChangeLicense)));
				}
				return relayCommand;
			}
		}

		private void ChangeLicense()
		{
			this.ChangeLicenseIsInProgress = true;
			this.SerialNumberInputText = this.Serial;
		}

		public ICommand BuyLicenseCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._buyLicenseCommand) == null)
				{
					relayCommand = (this._buyLicenseCommand = new RelayCommand(new Action(this.BuyLicense)));
				}
				return relayCommand;
			}
		}

		private void BuyLicense()
		{
			this.ChangeLicense();
			DSUtils.GoUrl("https://www.daemon-tools.cc/cart/buy_check?abbr=rewasd");
			SenderGoogleAnalytics.SendMessageEvent("License", "Buy", "license", -1L, false);
		}

		public DelegateCommand RefreshLicenseCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._refreshLicenseCommand) == null)
				{
					delegateCommand = (this._refreshLicenseCommand = new DelegateCommand(new Action(this.RefreshLicense), new Func<bool>(this.CanRefreshLicense)));
				}
				return delegateCommand;
			}
		}

		private bool CanRefreshLicense()
		{
			return !this._isRefreshingLicenseCommand;
		}

		private void RefreshLicense()
		{
			LicenseMainVM.<RefreshLicense>d__42 <RefreshLicense>d__;
			<RefreshLicense>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<RefreshLicense>d__.<>4__this = this;
			<RefreshLicense>d__.<>1__state = -1;
			<RefreshLicense>d__.<>t__builder.Start<LicenseMainVM.<RefreshLicense>d__42>(ref <RefreshLicense>d__);
		}

		public ICommand CloseChangeLicenseCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._closeChangeLicenseCommand) == null)
				{
					relayCommand = (this._closeChangeLicenseCommand = new RelayCommand(new Action(this.CloseChangeLicense)));
				}
				return relayCommand;
			}
		}

		private void CloseChangeLicense()
		{
			this.ChangeLicenseIsInProgress = false;
		}

		public ICommand ActivateLicenseCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._activateLicenseCommand) == null)
				{
					relayCommand = (this._activateLicenseCommand = new RelayCommand(new Action(this.ActivateLicense), new Func<bool>(this.ActivateLicenseCanExecute)));
				}
				return relayCommand;
			}
		}

		public ICommand OpenMainPageCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._openMainPage) == null)
				{
					relayCommand = (this._openMainPage = new RelayCommand(new Action(this.OpenMainPage)));
				}
				return relayCommand;
			}
		}

		private void OpenMainPage()
		{
			reWASDApplicationCommands.NavigateContentCommand.Execute(typeof(MainContent));
		}

		public ICommand GoToSupportCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._goToSupportCommand) == null)
				{
					relayCommand = (this._goToSupportCommand = new RelayCommand(new Action(this.GoToSupport)));
				}
				return relayCommand;
			}
		}

		private void GoToSupport()
		{
			DSUtils.GoUrl("https://www.daemon-tools.cc/contacts/producttechnicalsupport");
		}

		public ILicensingService LicensingService
		{
			get
			{
				return App.LicensingService;
			}
		}

		public object MainContentVM
		{
			get
			{
				return App.MainContentVM;
			}
		}

		protected LicenseMainVM()
		{
			this._licenseText = new Localizable();
			LicenseMainVM.instanceInfo = default(LicenseInfo);
			this._licenseTrialIconUri = Application.Current.TryFindResource("FeatureLicenseBig") as Drawing;
			this._licensePaidIconUri = Application.Current.TryFindResource("FeatureLicenseBigBought") as Drawing;
			this._changeLicenseIsInProgress = false;
			this._licenseText.PropertyChanged += delegate([Nullable(2)] object sender, PropertyChangedEventArgs args)
			{
				this.OnPropertyChanged("LicenseText");
				this.OnPropertyChanged("HardWareID");
			};
			LicenseMainVM._features = new ObservableCollection<Feature>();
			LicenseMainVM._features.Add(new LicenseFeature(this, "license", 5005, 11096, 0, "FeatureLicenseSmall", "FeatureLicenseSmallBought", "FeatureLicenseBig", "FeatureLicenseBigBought", ""));
			LicenseMainVM._features.Add(new Feature(this, "advanced-mapping", 5005, 11171, 11172, "FeatureAdvancedMappingSmall", "FeatureAdvancedMappingSmallBought", "FeatureAdvancedMappingBig", "FeatureAdvancedMappingBigBought", "advanced-mapping", this.GetLearnMoreLink("advanced-mapping")));
			LicenseMainVM._features.Add(new Feature(this, "macros", 5005, 11173, 11174, "features-macros-small", "features-macros-small-bought", "features-macros-big", "features-macros-big-bought", "macros", this.GetLearnMoreLink("macros")));
			LicenseMainVM._features.Add(new Feature(this, "four-slots", 5005, 11175, 11176, "FeatureFourSlotsSmall", "FeatureFourSlotsSmallBought", "FeatureFourSlotsBig", "FeatureFourSlotsBigBought", "four-slots", this.GetLearnMoreLink("four-slots")));
			LicenseMainVM._features.Add(new Feature(this, "rapid-fire", 5005, 11206, 11207, "features-rapidfire-small", "features-rapidfire-small-bought", "features-rapidfire-big", "features-rapidfire-big-bought", "rapid-fire", this.GetLearnMoreLink("rapid-fire")));
			LicenseMainVM._features.Add(new Feature(this, "mobile-controller", 5005, 12511, 12512, "features-mobilecontroller-small", "features-mobilecontroller-small-bought", "features-mobilecontroller-big", "features-mobilecontroller-big-bought", "mobile-controller", this.GetLearnMoreLink("mobile-controller")));
			this.FullPackFeatureObject = new FullPackFeature(this, "bundle", 0, 11177, 11178, "FeatureSaleSmall", "FeatureSaleSmallBought", "FeatureSaleBig", "FeatureSaleBigBought", "bundle");
			LicenseMainVM._features.Add(this.FullPackFeatureObject);
		}

		protected void ExitWithTray()
		{
			Environment.Exit(0);
		}

		private async Task SetFeaturesPrices()
		{
			try
			{
				string slocale = Thread.CurrentThread.CurrentCulture.Name;
				Path.GetTempFileName();
				LicenseInfo licenseInfo = await App.HttpClientService.LicenseApi.GetLicenseInfo();
				string text;
				try
				{
					text = await new HttpClient().GetStringAsync(string.Format("https://secure.disc-soft.com/license/features/software_abbr/reWASD/locale/{0}?paid={1}", slocale, (licenseInfo.License == 3) ? "1" : "0"));
				}
				catch (Exception)
				{
					return;
				}
				if (text.Length == 0)
				{
					return;
				}
				XElement xelement = XElement.Load(JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(text), new XmlDictionaryReaderQuotas()));
				if (xelement.Element("success").Value == "true")
				{
					this._totalPriceOfAllFeatures = 0f;
					foreach (XElement xelement2 in xelement.Element("data").Elements())
					{
						string value = xelement2.Element("price").Value;
						Feature featureByGUID = this.GetFeatureByGUID(xelement2.Name.LocalName);
						if (featureByGUID != null)
						{
							featureByGUID.Price = value;
							string value2 = xelement2.Element("value").Value;
							string value3 = xelement2.Element("sign").Value;
							if (this._numberFormatInfo == null)
							{
								this._numberFormatInfo = new NumberFormatInfo
								{
									CurrencySymbol = value3,
									CurrencyDecimalSeparator = "."
								};
							}
							if (xelement2.Name.LocalName != "bundle")
							{
								if (featureByGUID.CurFeatureState.licenseStatus != 3)
								{
									this._totalPriceOfAllFeatures += float.Parse(value2, NumberStyles.Currency, this._numberFormatInfo);
								}
							}
							else
							{
								this.FullPackFeatureObject.PriceNumber = float.Parse(value2, NumberStyles.Currency, this._numberFormatInfo);
							}
						}
					}
					this.OnPropertyChanged("TotalPriceOfAllFeatures");
				}
				slocale = null;
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "SetFeaturesPrices");
			}
			this.OnPropertyChanged("TotalPriceOfAllFeatures");
		}

		protected async Task RefreshLicenseInfo()
		{
			TaskAwaiter<LicenseInfo> taskAwaiter = App.HttpClientService.LicenseApi.GetLicenseInfo().GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<LicenseInfo> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<LicenseInfo>);
			}
			LicenseMainVM.instanceInfo = taskAwaiter.GetResult();
			this.HardWareID = LicenseMainVM.instanceInfo.HardwareId;
			this.CurLicenseType = LicenseMainVM.instanceInfo.License;
			this._licenseText.TranslationId = ((LicenseMainVM.instanceInfo.License == 2) ? 4459 : 4458);
			if (this.ChangeLicenseIsInProgress || LicenseMainVM.instanceInfo.License != 2)
			{
				this.Serial = LicenseMainVM.instanceInfo.Serial;
			}
			else
			{
				this.Serial = "";
				this._trialExpiresDate = DateTime.Now.AddDays((double)LicenseMainVM.instanceInfo.TrialDaysLeft).ToString("MMM dd, yyyy");
			}
			this.OnPropertyChanged("SerialCaption");
			this.OnPropertyChanged("LicenseText");
			this.OnPropertyChanged("CurLicenseType");
			this.OnPropertyChanged("TrialExpiresDate");
		}

		public async Task RefreshFeaturePrices()
		{
			await this.SetFeaturesPrices();
			if (this._totalPriceOfAllFeatures <= this.FullPackFeatureObject.PriceNumber)
			{
				this.RemoveFullPackFromFeatures();
			}
			else if (!this.Features.Contains(this.FullPackFeatureObject))
			{
				this.AddFullPackFeature();
			}
		}

		private void RefreshFeaturesByResult(LicenseCheckResult info)
		{
			foreach (Feature feature in LicenseMainVM._features)
			{
				feature.RefreshState(info);
				feature.SetLearnMoreLink(this.GetLearnMoreLink(feature.FeatureId));
			}
		}

		private void RemoveFullPackFromFeatures()
		{
			int num = LicenseMainVM._features.IndexOf(this.FullPackFeatureObject);
			if (num >= 0)
			{
				this.Features.RemoveAt(num);
				this.OnPropertyChanged("Features");
			}
			if (this.ItemSelectedIndex == num)
			{
				this.ItemSelectedIndex = 0;
			}
		}

		private void AddFullPackFeature()
		{
			LicenseMainVM._features.Add(this.FullPackFeatureObject);
			this.OnPropertyChanged("Features");
		}

		public string TrialExpiresDate
		{
			get
			{
				return this._trialExpiresDate;
			}
		}

		public string TotalPriceOfAllFeatures
		{
			get
			{
				return this._totalPriceOfAllFeatures.ToString("C", this._numberFormatInfo);
			}
		}

		public bool IsFullPackSelected
		{
			get
			{
				return this.ItemSelectedIndex == this.Features.IndexOf(this.FullPackFeatureObject);
			}
		}

		private int getFeatureIndex(string featureGuid)
		{
			Feature feature = LicenseMainVM._features.First((Feature item) => item.FeatureId == featureGuid);
			if (feature != null)
			{
				return LicenseMainVM._features.IndexOf(feature);
			}
			return -1;
		}

		public static async Task<bool> IsTrialExpired()
		{
			await App.HttpClientService.LicenseApi.CheckLicense(false);
			TaskAwaiter<LicenseInfo> taskAwaiter = App.HttpClientService.LicenseApi.GetLicenseInfo().GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<LicenseInfo> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<LicenseInfo>);
			}
			LicenseMainVM.instanceInfo = taskAwaiter.GetResult();
			bool flag;
			if (LicenseMainVM.instanceInfo.License == 2 && LicenseMainVM.instanceInfo.TrialDaysLeft < 0)
			{
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public static bool IsFeatureActivated(string featureGuid)
		{
			for (int i = 0; i < LicenseMainVM.instanceInfo.FeaturesCount; i++)
			{
				if (LicenseMainVM.instanceInfo.FeaturesStates[i].pszFeatureId == featureGuid)
				{
					return LicenseMainVM.IsFeatureActivated(LicenseMainVM.instanceInfo.FeaturesStates[i].licenseStatus, LicenseMainVM.instanceInfo.FeaturesStates[i].TrialDaysLeft);
				}
			}
			return false;
		}

		private static bool IsFeatureActivated(FeatureLicenseStatus status, int trialDaysLeft)
		{
			return status == 3 || (status == 2 && trialDaysLeft > 0);
		}

		public bool FeatureCanBeTrial(string featureGuid)
		{
			return LicenseMainVM._features.First((Feature item) => item.FeatureId == featureGuid).FeatureCanBeTrial();
		}

		public int ItemSelectedIndex
		{
			get
			{
				if (this._itemSelectedIndex == -1)
				{
					return RegistryHelper.GetValue(LicenseMainVM.STRING_REGYSTRY_KEY_NAME, "", 0, false);
				}
				return this._itemSelectedIndex;
			}
			set
			{
				if (this._itemSelectedIndex == value)
				{
					return;
				}
				this._itemSelectedIndex = value;
				RegistryHelper.SetValue(LicenseMainVM.STRING_REGYSTRY_KEY_NAME, "", this._itemSelectedIndex);
				this.OnPropertyChanged("ItemSelectedIndex");
				this.OnPropertyChanged("ShowLicenseHelp");
				this.OnPropertyChanged("LicenseHelpString");
				this.OnPropertyChanged("IsFullPackSelected");
			}
		}

		public LicenseType CurLicenseType
		{
			get
			{
				return this._curLicenseType;
			}
			set
			{
				if (this._curLicenseType == value)
				{
					return;
				}
				this._curLicenseType = value;
				this.OnPropertyChanged("LicenseIconUri");
				this.OnPropertyChanged("CurLicenseType");
				this.OnPropertyChanged("BuyBtnLicenseVisibility");
			}
		}

		public Drawing LicenseIconUri
		{
			get
			{
				if (this.CurLicenseType == 2)
				{
					return this._licenseTrialIconUri;
				}
				return this._licensePaidIconUri;
			}
		}

		public Localizable LicenseText
		{
			get
			{
				return this._licenseText;
			}
			set
			{
				if (this._licenseText == value)
				{
					return;
				}
				this._licenseText = value;
				this.OnPropertyChanged("LicenseText");
			}
		}

		public ObservableCollection<Feature> Features
		{
			get
			{
				return LicenseMainVM._features;
			}
		}

		public bool ChangeLicenseIsInProgress
		{
			get
			{
				return this._changeLicenseIsInProgress;
			}
			set
			{
				if (this._changeLicenseIsInProgress == value)
				{
					return;
				}
				this._changeLicenseIsInProgress = value;
				this.OnPropertyChanged("ChangeLicenseIsInProgress");
				this.OnPropertyChanged("BuyBtnLicenseVisibility");
				this.RefreshLicenseInfo();
			}
		}

		public bool RequestIsInProgress
		{
			get
			{
				return this._requestIsInProgress;
			}
			set
			{
				if (this._requestIsInProgress == value)
				{
					return;
				}
				this._requestIsInProgress = value;
				this.OnPropertyChanged("RequestIsInProgress");
			}
		}

		public bool BuyBtnLicenseVisibility
		{
			get
			{
				return this.CurLicenseType != 3;
			}
		}

		public bool ShowLicenseHelp
		{
			get
			{
				return this.CurLicenseType != 3 || (!(LicenseMainVM._features[this.ItemSelectedIndex] is LicenseFeature) && !LicenseMainVM._features[this.ItemSelectedIndex].IsFeaturePaid);
			}
		}

		public string LicenseHelpString
		{
			get
			{
				Feature feature = LicenseMainVM._features[this.ItemSelectedIndex];
				bool flag = feature is LicenseFeature;
				bool flag2 = feature is FullPackFeature;
				bool flag3 = this.CurLicenseType == 3;
				bool isFeaturePaid = feature.IsFeaturePaid;
				if (!flag3)
				{
					if (flag)
					{
						return DTLocalization.GetString(12347);
					}
					if (flag2)
					{
						return DTLocalization.GetString(12348);
					}
					return DTLocalization.GetString(12349);
				}
				else
				{
					if (!flag && !isFeaturePaid)
					{
						return DTLocalization.GetString(12350);
					}
					return "";
				}
			}
		}

		public string SerialNumberInputText
		{
			get
			{
				return this._serialNumberInputText;
			}
			set
			{
				if (this._serialNumberInputText == value)
				{
					return;
				}
				this._serialNumberInputText = value;
				this.OnPropertyChanged("SerialNumberInputText");
				this.OnPropertyChanged("CheckSerialNumber");
			}
		}

		private async Task CheckLicenseError(LicenseCheckResult result)
		{
			if (result.Result == 6)
			{
				DTMessageBox.Show(DTLocalization.GetString(4234), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
			}
			else
			{
				string message = result.Message;
				if (!string.IsNullOrEmpty(result.OfferText))
				{
					IGuiHelperService guiHelperService = App.GuiHelperService;
					await (((guiHelperService != null) ? guiHelperService.ShowLicenseWizard(new Ref<LicenseCheckResult>(result)) : null) ?? Task.FromResult<bool>(true));
				}
				else if (!string.IsNullOrEmpty(message))
				{
					DTMessageBox.Show(message, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
				}
			}
		}

		public async Task<LicenseCheckResult> Activate(ActivationLicenseInfo licenseInfo)
		{
			LicenseCheckResult licenseCheckResult = await App.HttpClientService.LicenseApi.ActivateLicense(licenseInfo);
			if (licenseCheckResult.IsSuccessResult == 1)
			{
				LicenseServiceDelegates.LicenseActivatedDelegate onLicenseActivated = this.OnLicenseActivated;
				if (onLicenseActivated != null)
				{
					onLicenseActivated.Invoke();
				}
			}
			return licenseCheckResult;
		}

		private void ActivateLicense()
		{
			LicenseMainVM.<ActivateLicense>d__114 <ActivateLicense>d__;
			<ActivateLicense>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ActivateLicense>d__.<>4__this = this;
			<ActivateLicense>d__.<>1__state = -1;
			<ActivateLicense>d__.<>t__builder.Start<LicenseMainVM.<ActivateLicense>d__114>(ref <ActivateLicense>d__);
		}

		private bool IsAllowedCharForSerialNumber(char c)
		{
			return c != '-' && ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'));
		}

		private bool ActivateLicenseCanExecute()
		{
			return this.SerialNumberInputText != null && this.SerialNumberInputText.Count<char>() == 32 && this.SerialNumberInputText.All(new Func<char, bool>(this.IsAllowedCharForSerialNumber)) && !this.RequestIsInProgress;
		}

		public virtual async Task OnLicenseChanged(LicenseCheckResult result, bool onlineActivation = true)
		{
			await this.RefreshLicenseInfo();
			this.RefreshFeaturesByResult(result);
			this.NewVersionAvailable = result.NewVersionAvailable == 1;
			this.OnPropertyChanged("ShowLicenseHelp");
			this.OnPropertyChanged("LicenseHelpString");
		}

		public virtual void RefreshLicensingProperties()
		{
		}

		public Task ActivateFeature(string featureGuid)
		{
			LicenseMainVM.<ActivateFeature>d__119 <ActivateFeature>d__;
			<ActivateFeature>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ActivateFeature>d__.<>4__this = this;
			<ActivateFeature>d__.featureGuid = featureGuid;
			<ActivateFeature>d__.<>1__state = -1;
			<ActivateFeature>d__.<>t__builder.Start<LicenseMainVM.<ActivateFeature>d__119>(ref <ActivateFeature>d__);
			return <ActivateFeature>d__.<>t__builder.Task;
		}

		public Task<LicenseCheckResult> ActivateFeatureAsync(string featureGuid)
		{
			LicenseMainVM.<ActivateFeatureAsync>d__120 <ActivateFeatureAsync>d__;
			<ActivateFeatureAsync>d__.<>t__builder = AsyncTaskMethodBuilder<LicenseCheckResult>.Create();
			<ActivateFeatureAsync>d__.<>4__this = this;
			<ActivateFeatureAsync>d__.featureGuid = featureGuid;
			<ActivateFeatureAsync>d__.<>1__state = -1;
			<ActivateFeatureAsync>d__.<>t__builder.Start<LicenseMainVM.<ActivateFeatureAsync>d__120>(ref <ActivateFeatureAsync>d__);
			return <ActivateFeatureAsync>d__.<>t__builder.Task;
		}

		public async Task BuyFeature(string featureGuid)
		{
			Feature feature = LicenseMainVM._features.First((Feature item) => item.FeatureId == featureGuid);
			if (feature != null)
			{
				if (feature == this.FullPackFeatureObject)
				{
					if (this.CurLicenseType == 3)
					{
						LicenseInfo licenseInfo = await App.HttpClientService.LicenseApi.GetLicenseInfo();
						DSUtils.GoUrl(string.Format("https://www.daemon-tools.cc/cart/set_upgrade?config[type]=features_bundle&config[serial]={0}&utm_campaign=rewasd2&utm_source=app&utm_medium=fullpack", licenseInfo.Serial));
					}
					else
					{
						DSUtils.GoUrl("https://www.daemon-tools.cc/cart/buy_check?abbr=rewasd&features_bundle=true&utm_campaign=rewasd2&utm_source=app&utm_medium=fullpack");
					}
					SenderGoogleAnalytics.SendMessageEvent("License", "Buy", "fullpack", -1L, false);
				}
				else
				{
					if (this.CurLicenseType == 2)
					{
						DSUtils.GoUrl(string.Format("https://www.daemon-tools.cc/cart/buy_check?abbr=rewasd&features[]={0}&utm_campaign=rewasd2&utm_source=app&utm_medium=feature", feature.FeatureLinkName));
					}
					else
					{
						DSUtils.GoUrl(string.Format("https://www.daemon-tools.cc/cart/set_upgrade?config[type]=features&config[serial]={0}&config[features][]={1}&utm_campaign=rewasd2&utm_source=app&utm_medium=feature", (await App.HttpClientService.LicenseApi.GetLicenseInfo()).Serial, feature.FeatureLinkName));
					}
					SenderGoogleAnalytics.SendMessageEvent("License", "Buy", feature.FeatureId, -1L, false);
				}
			}
		}

		public void ShowFeature(string featureGuid)
		{
			this.ItemSelectedIndex = this.getFeatureIndex(featureGuid);
		}

		protected Feature GetFeatureByGUID(string featureGuid)
		{
			Feature feature;
			try
			{
				feature = LicenseMainVM._features.First((Feature item) => item.FeatureId == featureGuid);
			}
			catch (Exception)
			{
				feature = null;
			}
			return feature;
		}

		public void LearnMore(string featureGUID)
		{
			DSUtils.GoUrl(this.GetLearnMoreLink(featureGUID));
		}

		private string GetLearnMoreLink(string featureGUID)
		{
			string text = string.Format("https://www.rewasd.com/advanced-controller-mapping?utm_campaign=rewasd2&utm_source=app&utm_medium=learn#{0}", featureGUID);
			if (!string.IsNullOrEmpty(this._serial))
			{
				int num = text.IndexOf('#');
				if (num > 0)
				{
					text = text.Insert(num, "&serial=" + this._serial);
				}
				else
				{
					text = text + "&serial=" + this._serial;
				}
			}
			return text;
		}

		protected string GetBuyUrlForFeatures(List<string> features)
		{
			string text;
			if (this.IsPaidUser)
			{
				string FeaturesParams2 = "";
				features.ForEach(delegate(string item)
				{
					FeaturesParams2 += string.Format("config[features][]={0}&", item);
				});
				text = string.Format("https://www.daemon-tools.cc/cart/set_upgrade?config[type]=features&config[serial]={0}&{1}utm_campaign=rewasd2&utm_source=app&utm_medium=feature", LicenseMainVM.instanceInfo.Serial, FeaturesParams2);
			}
			else
			{
				string FeaturesParams = "";
				features.ForEach(delegate(string item)
				{
					FeaturesParams += string.Format("features[]={0}&", item);
				});
				text = string.Format("https://www.daemon-tools.cc/cart/buy_check?abbr=rewasd&{0}utm_campaign=rewasd2&utm_source=app&utm_medium=feature", FeaturesParams);
			}
			return text;
		}

		protected virtual async Task<bool> ProcessResult_LicenseError(Ref<LicenseCheckResult> checkingResultInfo)
		{
			IGuiHelperService guiHelperService = App.GuiHelperService;
			return await (((guiHelperService != null) ? guiHelperService.ShowLicenseWizard(checkingResultInfo) : null) ?? Task.FromResult<bool>(false));
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			string text = (string)navigationContext.Parameters["tab"];
			if (text == null)
			{
				this.ItemSelectedIndex = 0;
				return;
			}
			if (text.Equals("advanced-mapping"))
			{
				this.ItemSelectedIndex = 1;
				return;
			}
			if (text.Equals("macros"))
			{
				this.ItemSelectedIndex = 2;
				return;
			}
			if (text.Equals("four-slots"))
			{
				this.ItemSelectedIndex = 3;
				return;
			}
			if (text.Equals("rapid-fire"))
			{
				this.ItemSelectedIndex = 4;
				return;
			}
			if (text.Equals("mobile-controller"))
			{
				this.ItemSelectedIndex = 5;
				return;
			}
			if (text.Equals("bundle"))
			{
				this.ItemSelectedIndex = 6;
				return;
			}
			this.ItemSelectedIndex = 0;
		}

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
		}

		private static ObservableCollection<Feature> _features;

		private static LicenseInfo instanceInfo;

		public FullPackFeature FullPackFeatureObject;

		private NumberFormatInfo _numberFormatInfo;

		private float _totalPriceOfAllFeatures;

		private Drawing _licenseTrialIconUri;

		private Drawing _licensePaidIconUri;

		private string _serial;

		private string _trialExpiresDate;

		private string _serialNumberInputText;

		private string _hardwareid;

		private Localizable _licenseText;

		private LicenseType _curLicenseType;

		private bool _changeLicenseIsInProgress;

		private bool _newVersionAvailable;

		private RelayCommand _changeLicenseCommand;

		private RelayCommand _buyLicenseCommand;

		private DelegateCommand _refreshLicenseCommand;

		private bool _isRefreshingLicenseCommand;

		private RelayCommand _closeChangeLicenseCommand;

		private RelayCommand _activateLicenseCommand;

		private RelayCommand _openMainPage;

		private RelayCommand _goToSupportCommand;

		private static string STRING_REGYSTRY_KEY_NAME = "TabItemSelectedIndex";

		private int _itemSelectedIndex = -1;

		private bool _requestIsInProgress;
	}
}
