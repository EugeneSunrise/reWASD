using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using reWASDUI.Infrastructure;
using reWASDUI.ViewModels.Base;
using reWASDUI.Views;
using XBEliteWPF.License.Licensing.ComStructures;
using XBEliteWPF.Utils;

namespace reWASDUI.ViewModels.EngineController
{
	public class BaseEngineControllerVM : BaseServicesVM
	{
		public BaseEngineControllerVM(IContainerProvider uc)
			: base(uc)
		{
			TranslationManager.Instance.LanguageChanged += delegate([Nullable(2)] object sender, EventArgs args)
			{
				this.UpdateProperies();
			};
			App.LicensingService.OnLicenseChangedCompleted += delegate(LicenseCheckResult s, bool a)
			{
				this.SetLicenseInfo();
			};
			App.EventAggregator.GetEvent<PreferencesChanged>().Subscribe(delegate(object x)
			{
				this.PreferenceChanged();
			});
			Task.Delay(200);
			this.SetLicenseInfo();
		}

		protected virtual void PreferenceChanged()
		{
			this.UpdateProperies();
		}

		private FeatureState? LicenseFeatureState
		{
			get
			{
				if (this.LicenseInfo == null)
				{
					return null;
				}
				FeatureState[] featuresStates = this.LicenseInfo.GetValueOrDefault().FeaturesStates;
				if (featuresStates == null)
				{
					return null;
				}
				return new FeatureState?(featuresStates.FirstOrDefault((FeatureState x) => x.pszFeatureId == "mobile-controller"));
			}
		}

		public bool IsLicenseNotActivated
		{
			get
			{
				FeatureState? featureState;
				return this.LicenseFeatureState != null && featureState.GetValueOrDefault().licenseStatus == 1;
			}
		}

		public bool IsFeatureHasTrialDays
		{
			get
			{
				FeatureState? featureState;
				return this.LicenseFeatureState != null && featureState.GetValueOrDefault().TrialDaysLeft > 0;
			}
		}

		public bool IsLicenseTrial
		{
			get
			{
				FeatureState? featureState;
				return this.LicenseFeatureState != null && featureState.GetValueOrDefault().licenseStatus == 2;
			}
		}

		public bool IsLicenseHasNonActivatedTrial
		{
			get
			{
				return this.IsLicenseNotActivated && this.IsFeatureHasTrialDays;
			}
		}

		public bool IsLicenseNeedBuy
		{
			get
			{
				return (this.IsLicenseNotActivated || this.IsLicenseTrial) && !this.IsFeatureHasTrialDays;
			}
		}

		private async void SetLicenseInfo()
		{
			LicenseInfo licenseInfo = await App.LicensingService.GetLicenseInfo();
			this.LicenseInfo = new LicenseInfo?(licenseInfo);
			this.UpdateProperies();
		}

		public void UpdateProperies()
		{
			this.OnPropertyChanged("IsLicenseHasNonActivatedTrial");
			this.OnPropertyChanged("IsLicenseNotActivated");
			this.OnPropertyChanged("IsLicenseNeedBuy");
		}

		public DelegateCommand ActivateMobileFeatureCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._activateMobileFeature) == null)
				{
					delegateCommand = (this._activateMobileFeature = new DelegateCommand(new Action(this.ActivateMobileFeature)));
				}
				return delegateCommand;
			}
		}

		protected virtual async void ActivateMobileFeature()
		{
			if (this.IsLicenseNotActivated || (this.IsLicenseTrial && !this.IsFeatureHasTrialDays))
			{
				if (this.IsLicenseNeedBuy)
				{
					Dictionary<object, object> dictionary = new Dictionary<object, object>();
					dictionary.Add("navigatePath", typeof(LicenseMain));
					NavigationParameters navigationParameters = new NavigationParameters();
					navigationParameters.Add("tab", "mobile-controller");
					dictionary.Add("NavigationParameters", navigationParameters);
					reWASDApplicationCommands.NavigateContentCommand.Execute(dictionary);
					if (this.LicenseInfo != null && this.LicenseInfo.GetValueOrDefault().License == 3)
					{
						DSUtils.GoUrl(string.Format("https://www.daemon-tools.cc/cart/set_upgrade?config[type]=features&config[serial]={0}&config[features][]={1}&utm_campaign=rewasd2&utm_source=app&utm_medium=feature", (this.LicenseInfo != null) ? this.LicenseInfo.GetValueOrDefault().Serial : null, "mobile-controller"));
					}
					else
					{
						DSUtils.GoUrl(string.Format("https://www.daemon-tools.cc/cart/buy_check?abbr=rewasd&features[]={0}&utm_campaign=rewasd2&utm_source=app&utm_medium=feature", "mobile-controller"));
					}
					SenderGoogleAnalytics.SendMessageEvent("License", "Buy", "mobile-controller", -1L, false);
				}
				else
				{
					await App.LicensingService.ActivateFeatureAsync("mobile-controller");
				}
			}
		}

		private LicenseInfo? LicenseInfo;

		private DelegateCommand _activateMobileFeature;
	}
}
