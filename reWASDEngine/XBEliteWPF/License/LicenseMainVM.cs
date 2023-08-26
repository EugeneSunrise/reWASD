using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using reWASDEngine;
using XBEliteWPF.License.Licensing;
using XBEliteWPF.License.Licensing.ComStructures;
using XBEliteWPF.License.Licensing.Enums;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils;
using XBEliteWPF.ViewModels;

namespace XBEliteWPF.License
{
	public class LicenseMainVM
	{
		public string Serial { get; set; }

		public string HardWareID { get; set; }

		public bool IsPaidUser
		{
			get
			{
				return !string.IsNullOrEmpty(this.Serial);
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
			LicenseMainVM.<RefreshLicense>d__33 <RefreshLicense>d__;
			<RefreshLicense>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<RefreshLicense>d__.<>4__this = this;
			<RefreshLicense>d__.<>1__state = -1;
			<RefreshLicense>d__.<>t__builder.Start<LicenseMainVM.<RefreshLicense>d__33>(ref <RefreshLicense>d__);
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

		protected LicenseMainVM()
		{
			LicenseMainVM.instanceInfo = default(LicenseInfo);
			this._licenseTrialIconUri = Application.Current.TryFindResource("FeatureLicenseBig") as Drawing;
			this._licensePaidIconUri = Application.Current.TryFindResource("FeatureLicenseBigBought") as Drawing;
			this._changeLicenseIsInProgress = false;
			LicenseMainVM._features = new ObservableCollection<Feature>();
			LicenseMainVM._features.Add(new LicenseFeature(this, "license", 5005, 11096, 0, "FeatureLicenseSmall", "FeatureLicenseSmallBought", "FeatureLicenseBig", "FeatureLicenseBigBought", ""));
			LicenseMainVM._features.Add(new Feature(this, "advanced-mapping", 5005, 11171, 11172, "FeatureAdvancedMappingSmall", "FeatureAdvancedMappingSmallBought", "FeatureAdvancedMappingBig", "FeatureAdvancedMappingBigBought", "advanced-mapping", this.GetLearnMoreLink("advanced-mapping")));
			LicenseMainVM._features.Add(new Feature(this, "macros", 5005, 11173, 11174, "features-macros-small", "features-macros-small-bought", "features-macros-big", "features-macros-big-bought", "macros", this.GetLearnMoreLink("macros")));
			LicenseMainVM._features.Add(new Feature(this, "four-slots", 5005, 11175, 11176, "FeatureFourSlotsSmall", "FeatureFourSlotsSmallBought", "FeatureFourSlotsBig", "FeatureFourSlotsBigBought", "four-slots", this.GetLearnMoreLink("four-slots")));
			LicenseMainVM._features.Add(new Feature(this, "rapid-fire", 5005, 11206, 11207, "features-turbotoggle-small", "features-turbotoggle-small-bought", "features-turbotoggleslots-big", "features-turbotoggleslots-big-bought", "rapid-fire", this.GetLearnMoreLink("rapid-fire")));
			LicenseMainVM._features.Add(new Feature(this, "mobile-controller", 5005, 12511, 12512, "features-mobilecontroller-small", "features-mobilecontroller-small-bought", "features-mobilecontroller-big", "features-mobilecontroller-big-bought", "mobile-controller", this.GetLearnMoreLink("mobile-controller")));
			this.FullPackFeatureObject = new FullPackFeature(this, "bundle", 0, 11177, 11178, "FeatureSaleSmall", "FeatureSaleSmallBought", "FeatureSaleBig", "FeatureSaleBigBought", "bundle");
			LicenseMainVM._features.Add(this.FullPackFeatureObject);
		}

		protected void ExitWithTray()
		{
		}

		private async Task SetFeaturesPrices()
		{
			try
			{
				string slocale = Thread.CurrentThread.CurrentCulture.Name;
				Path.GetTempFileName();
				LicenseInfo licenseInfo = await LicenseApi.GetLicenseInfo();
				string text = await new HttpClient().GetStringAsync(string.Format("https://secure.disc-soft.com/license/features/software_abbr/reWASD/locale/{0}?paid={1}", slocale, (licenseInfo.License == 3) ? "1" : "0"));
				if (text.Length != 0)
				{
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
					}
					slocale = null;
				}
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "SetFeaturesPrices");
			}
		}

		protected async Task RefreshLicenseInfo()
		{
			TaskAwaiter<LicenseInfo> taskAwaiter = LicenseApi.GetLicenseInfo().GetAwaiter();
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
			if (this.ChangeLicenseIsInProgress || LicenseMainVM.instanceInfo.License != 2)
			{
				this.Serial = LicenseMainVM.instanceInfo.Serial;
			}
			else
			{
				this.Serial = "";
			}
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

		private async Task RefreshFeatures()
		{
			LicenseCheckResult licenseCheckResult = await LicenseApi.CheckLicense(false);
			foreach (Feature feature in LicenseMainVM._features)
			{
				feature.RefreshState(licenseCheckResult);
				feature.SetLearnMoreLink(this.GetLearnMoreLink(feature.FeatureId));
			}
		}

		private void RemoveFullPackFromFeatures()
		{
			int num = LicenseMainVM._features.IndexOf(this.FullPackFeatureObject);
			if (num >= 0)
			{
				this.Features.RemoveAt(num);
			}
			if (this.ItemSelectedIndex == num)
			{
				this.ItemSelectedIndex = 0;
			}
		}

		private void AddFullPackFeature()
		{
			LicenseMainVM._features.Add(this.FullPackFeatureObject);
		}

		public static async Task<bool> IsTrialExpired()
		{
			await LicenseApi.CheckLicense(false);
			TaskAwaiter<LicenseInfo> taskAwaiter = LicenseApi.GetLicenseInfo().GetAwaiter();
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
					IGuiHelperService guiHelperService = Engine.GuiHelperService;
					await (((guiHelperService != null) ? guiHelperService.ShowLicenseWizard(new Ref<LicenseCheckResult>(result)) : null) ?? Task.FromResult<bool>(true));
				}
				else if (!string.IsNullOrEmpty(message))
				{
					DTMessageBox.Show(message, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
				}
			}
		}

		protected void ClearSerial()
		{
			RegistryHelper.SetString("Config", "Serial", "");
		}

		public Task<LicenseCheckResult> Activate(ActivationLicenseInfo licenseInfo)
		{
			LicenseMainVM.<Activate>d__80 <Activate>d__;
			<Activate>d__.<>t__builder = AsyncTaskMethodBuilder<LicenseCheckResult>.Create();
			<Activate>d__.<>4__this = this;
			<Activate>d__.licenseInfo = licenseInfo;
			<Activate>d__.<>1__state = -1;
			<Activate>d__.<>t__builder.Start<LicenseMainVM.<Activate>d__80>(ref <Activate>d__);
			return <Activate>d__.<>t__builder.Task;
		}

		private void ActivateLicense()
		{
			LicenseMainVM.<ActivateLicense>d__81 <ActivateLicense>d__;
			<ActivateLicense>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ActivateLicense>d__.<>4__this = this;
			<ActivateLicense>d__.<>1__state = -1;
			<ActivateLicense>d__.<>t__builder.Start<LicenseMainVM.<ActivateLicense>d__81>(ref <ActivateLicense>d__);
		}

		private bool IsAllowedCharForSerialNumber(char c)
		{
			return c != '-' && ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'));
		}

		private bool ActivateLicenseCanExecute()
		{
			return this.SerialNumberInputText != null && this.SerialNumberInputText.Count<char>() == 32 && this.SerialNumberInputText.All(new Func<char, bool>(this.IsAllowedCharForSerialNumber)) && !this.RequestIsInProgress;
		}

		public async Task<bool> TryFeature(string featureGuid)
		{
			TaskAwaiter<LicenseCheckResult> taskAwaiter = this.ActivateFeatureAsync(featureGuid).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<LicenseCheckResult> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<LicenseCheckResult>);
			}
			return taskAwaiter.GetResult().IsSuccessResult == 1;
		}

		public virtual async Task OnLicenseChanged(LicenseCheckResult result, bool onlineActivation = true)
		{
			await this.RefreshLicenseInfo();
			this.RefreshFeaturesByResult(result);
		}

		public Task ActivateFeature(string featureGuid)
		{
			LicenseMainVM.<ActivateFeature>d__86 <ActivateFeature>d__;
			<ActivateFeature>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ActivateFeature>d__.<>4__this = this;
			<ActivateFeature>d__.featureGuid = featureGuid;
			<ActivateFeature>d__.<>1__state = -1;
			<ActivateFeature>d__.<>t__builder.Start<LicenseMainVM.<ActivateFeature>d__86>(ref <ActivateFeature>d__);
			return <ActivateFeature>d__.<>t__builder.Task;
		}

		public Task<LicenseCheckResult> ActivateFeatureAsync(string featureGuid)
		{
			LicenseMainVM.<ActivateFeatureAsync>d__87 <ActivateFeatureAsync>d__;
			<ActivateFeatureAsync>d__.<>t__builder = AsyncTaskMethodBuilder<LicenseCheckResult>.Create();
			<ActivateFeatureAsync>d__.<>4__this = this;
			<ActivateFeatureAsync>d__.featureGuid = featureGuid;
			<ActivateFeatureAsync>d__.<>1__state = -1;
			<ActivateFeatureAsync>d__.<>t__builder.Start<LicenseMainVM.<ActivateFeatureAsync>d__87>(ref <ActivateFeatureAsync>d__);
			return <ActivateFeatureAsync>d__.<>t__builder.Task;
		}

		public async void BuyFeature(string featureGuid)
		{
			Feature feature = LicenseMainVM._features.First((Feature item) => item.FeatureId == featureGuid);
			if (feature != null)
			{
				if (feature == this.FullPackFeatureObject)
				{
					if (this.CurLicenseType == 3)
					{
						LicenseInfo licenseInfo = await LicenseApi.GetLicenseInfo();
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
						DSUtils.GoUrl(string.Format("https://www.daemon-tools.cc/cart/set_upgrade?config[type]=features&config[serial]={0}&config[features][]={1}&utm_campaign=rewasd2&utm_source=app&utm_medium=feature", (await LicenseApi.GetLicenseInfo()).Serial, feature.FeatureLinkName));
					}
					SenderGoogleAnalytics.SendMessageEvent("License", "Buy", feature.FeatureId, -1L, false);
				}
			}
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
			if (!string.IsNullOrEmpty(this.Serial))
			{
				int num = text.IndexOf('#');
				if (num > 0)
				{
					text = text.Insert(num, "&serial=" + this.Serial);
				}
				else
				{
					text = text + "&serial=" + this.Serial;
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
			IGuiHelperService guiHelperService = Engine.GuiHelperService;
			return await (((guiHelperService != null) ? guiHelperService.ShowLicenseWizard(checkingResultInfo) : null) ?? Task.FromResult<bool>(false));
		}

		private static ObservableCollection<Feature> _features;

		private static LicenseInfo instanceInfo;

		public FullPackFeature FullPackFeatureObject;

		private NumberFormatInfo _numberFormatInfo;

		private float _totalPriceOfAllFeatures;

		private Drawing _licenseTrialIconUri;

		private Drawing _licensePaidIconUri;

		private string _serialNumberInputText;

		private LicenseType _curLicenseType;

		private bool _changeLicenseIsInProgress;

		private RelayCommand _changeLicenseCommand;

		private RelayCommand _buyLicenseCommand;

		private DelegateCommand _refreshLicenseCommand;

		private bool _isRefreshingLicenseCommand;

		private RelayCommand _closeChangeLicenseCommand;

		private RelayCommand _activateLicenseCommand;

		private RelayCommand _goToSupportCommand;

		private static string STRING_REGYSTRY_KEY_NAME = "TabItemSelectedIndex";

		private int _itemSelectedIndex = -1;

		private bool _requestIsInProgress;
	}
}
