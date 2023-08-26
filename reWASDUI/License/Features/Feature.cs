using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils.Clases;
using Prism.Commands;
using Prism.Regions;
using reWASDUI.Infrastructure;
using reWASDUI.Views;
using XBEliteWPF.License.Licensing.ComStructures;

namespace reWASDUI.License.Features
{
	public class Feature : NotifyPropertyChangedObject
	{
		public bool IsFeature
		{
			get
			{
				return base.GetType() == typeof(Feature);
			}
		}

		public Feature(LicenseMainVM featureOwner, string featureid, int name, int bigName, int description, string icon, string iconBought, string picture, string pictureBought, string linkName, string learnMoreLink = null)
		{
			this._featureOwner = featureOwner;
			this._featureid = featureid;
			this._bigName = new Localizable(bigName);
			this._bigName.PropertyChanged += delegate([Nullable(2)] object sender, PropertyChangedEventArgs args)
			{
				this.OnPropertyChanged("BigName");
				this.OnPropertyChanged("FeatureTrialDaysLeft");
				this.OnPropertyChanged("FeatureStatusToolTip");
			};
			this._description = new Localizable(description);
			if (learnMoreLink != null)
			{
				this._description.FormatString = string.Concat(new string[]
				{
					"<a href=\"",
					learnMoreLink,
					"\">",
					DTLocalization.GetString(11777),
					"</a>"
				});
			}
			this._description.PropertyChanged += delegate([Nullable(2)] object sender, PropertyChangedEventArgs args)
			{
				this.OnPropertyChanged("Description");
			};
			this._linkName = linkName;
			try
			{
				Window mainWindow = Application.Current.MainWindow;
				this._iconUri = ((mainWindow != null) ? mainWindow.TryFindResource(icon) : null) as Drawing;
				Window mainWindow2 = Application.Current.MainWindow;
				this._iconBoughtUri = ((mainWindow2 != null) ? mainWindow2.TryFindResource(iconBought) : null) as Drawing;
				Window mainWindow3 = Application.Current.MainWindow;
				this._pictureUri = ((mainWindow3 != null) ? mainWindow3.TryFindResource(picture) : null) as Drawing;
				Window mainWindow4 = Application.Current.MainWindow;
				this._pictureUriBought = ((mainWindow4 != null) ? mainWindow4.TryFindResource(pictureBought) : null) as Drawing;
			}
			catch (Exception)
			{
			}
			this._featureState.licenseStatus = 1;
		}

		public void SetLearnMoreLink(string learnMoreLink)
		{
			if (learnMoreLink != null)
			{
				this._description.FormatString = string.Concat(new string[]
				{
					"<a href=\"",
					learnMoreLink,
					"\">",
					DTLocalization.GetString(11777),
					"</a>"
				});
				this.OnPropertyChanged("Description");
			}
		}

		public void RefreshState(LicenseCheckResult licenseResultInfo)
		{
			for (int i = 0; i < licenseResultInfo.FeaturesCount; i++)
			{
				if (licenseResultInfo.FeaturesStates[i].pszFeatureId == this._featureid)
				{
					this._featureState = licenseResultInfo.FeaturesStates[i];
					break;
				}
			}
			this.OnPropertyChanged("IconUri");
			this.OnPropertyChanged("FeatureId");
			this.OnPropertyChanged("CurFeatureState");
			this.OnPropertyChanged("IsItemSelected");
			this.OnPropertyChanged("Description");
			this.OnPropertyChanged("PictureUri");
			this.OnPropertyChanged("TryBtnFeatureVisibility");
			this.OnPropertyChanged("BuyBtnFeatureVisibility");
			this.OnPropertyChanged("ActivateBtnFeatureVisibility");
			this.OnPropertyChanged("BigName");
			this.OnPropertyChanged("FeatureTrialDaysLeft");
			this.OnPropertyChanged("IsTrialFeatureVisibility");
			this.OnPropertyChanged("IsShowMobleControllerButtons");
			this.OnPropertyChanged("IsInTrialMode");
			this.OnPropertyChanged("IsTrialExpired");
			this.OnPropertyChanged("IsNotActivated");
			this.OnPropertyChanged("FeatureStatusToolTip");
		}

		public bool FeatureCanBeTrial()
		{
			return this._featureState.licenseStatus == 1 && this._featureState.TrialDaysLeft > 0;
		}

		public string FeatureId
		{
			get
			{
				return this._featureid;
			}
		}

		public bool IsShowMobleControllerButtons
		{
			get
			{
				return this._featureid == "mobile-controller";
			}
		}

		public FeatureState CurFeatureState
		{
			get
			{
				return this._featureState;
			}
		}

		public string FeatureLinkName
		{
			get
			{
				return this._linkName;
			}
		}

		public string Price
		{
			get
			{
				return this._price;
			}
			set
			{
				if (this._price == value)
				{
					return;
				}
				this._price = value;
				this.OnPropertyChanged("Price");
			}
		}

		protected bool IsTrial()
		{
			return this.IsInTrialMode;
		}

		public Localizable BigName
		{
			get
			{
				return this._bigName;
			}
			set
			{
				if (this._bigName == value)
				{
					return;
				}
				this._bigName = value;
				this.OnPropertyChanged("BigName");
			}
		}

		public string FeatureTrialDaysLeft
		{
			get
			{
				return string.Format(DTLocalization.GetString(11160).Replace("%d", "{0}"), this._featureState.TrialDaysLeft);
			}
		}

		public string GetBigName()
		{
			return this._bigName.ToString();
		}

		public Drawing IconUri
		{
			get
			{
				if (this.IsFeatureActivated && this.IsFeaturePaid)
				{
					return this._iconBoughtUri;
				}
				return this._iconUri;
			}
		}

		public Drawing PictureUri
		{
			get
			{
				if (this.CurFeatureState.licenseStatus == 3)
				{
					return this._pictureUriBought;
				}
				return this._pictureUri;
			}
		}

		public string FeatureStatusToolTip
		{
			get
			{
				if (this.IsFeaturePaid)
				{
					return string.Format(DTLocalization.GetString(12809), this.BigName.ToString());
				}
				if (this.IsTrialExpired)
				{
					return DTLocalization.GetString(12811);
				}
				if (this.IsInTrialMode)
				{
					DateTime dateTime = DateTime.Now.AddDays((double)this._featureState.TrialDaysLeft);
					return string.Format(DTLocalization.GetString(12810), this.BigName.ToString(), dateTime.ToString());
				}
				if (this.IsNotActivated)
				{
					return string.Format(DTLocalization.GetString(12812), this.BigName.ToString(), this._featureState.TrialDaysLeft);
				}
				return "";
			}
		}

		public Localizable Description
		{
			get
			{
				return this._description;
			}
		}

		public bool TrialIsExpired
		{
			get
			{
				return this._featureState.TrialDaysLeft == 0;
			}
		}

		public bool IsTrialFeatureVisibility
		{
			get
			{
				return this._featureState.licenseStatus == 2;
			}
		}

		public bool IsInTrialMode
		{
			get
			{
				return this._featureState.licenseStatus == 2 && this._featureState.TrialDaysLeft > 0;
			}
		}

		public bool IsTrialExpired
		{
			get
			{
				return this._featureState.licenseStatus == 2 && this._featureState.TrialDaysLeft <= 0;
			}
		}

		public bool IsNotActivated
		{
			get
			{
				return this._featureState.licenseStatus == 1;
			}
		}

		public virtual bool IsFeatureActivated
		{
			get
			{
				return LicenseMainVM.IsFeatureActivated(this._featureState.pszFeatureId);
			}
		}

		public virtual bool IsFeaturePaid
		{
			get
			{
				return this._featureState.licenseStatus == 3;
			}
		}

		public bool IsItemSelected
		{
			get
			{
				return this._isItemSeleted;
			}
			set
			{
				if (this._isItemSeleted == value)
				{
					return;
				}
				this._isItemSeleted = value;
				this.OnPropertyChanged("IsItemSelected");
				this.OnPropertyChanged("Icon");
			}
		}

		public string TryText
		{
			get
			{
				return string.Format(DTLocalization.GetString(12815), this._featureState.TrialDaysLeft);
			}
		}

		public ICommand GoToFeatureCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._goToFeatureCommand) == null)
				{
					delegateCommand = (this._goToFeatureCommand = new DelegateCommand(new Action(this.GoToFeature)));
				}
				return delegateCommand;
			}
		}

		private async void GoToFeature()
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("navigatePath", typeof(LicenseMain));
			NavigationParameters navigationParameters = new NavigationParameters();
			navigationParameters.Add("tab", this._featureid);
			dictionary.Add("NavigationParameters", navigationParameters);
			reWASDApplicationCommands.NavigateContentCommand.Execute(dictionary);
		}

		public ICommand TryFeatureLicenseCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._tryFeatureLicenseCommand) == null)
				{
					delegateCommand = (this._tryFeatureLicenseCommand = new DelegateCommand(new Action(this.TryFeatureLicense)));
				}
				return delegateCommand;
			}
		}

		private async void TryFeatureLicense()
		{
			await this._featureOwner.ActivateFeatureAsync(this._featureid);
		}

		public ICommand ActivateFeatureLicenseCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._activateFeatureLicenseCommand) == null)
				{
					delegateCommand = (this._activateFeatureLicenseCommand = new DelegateCommand(new Action(this.ActivateFeatureLicense)));
				}
				return delegateCommand;
			}
		}

		private async void ActivateFeatureLicense()
		{
			await this._featureOwner.ActivateFeature(this._featureid);
		}

		public ICommand BuyFeatureLicenseCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._buyFeatureLicenseCommand) == null)
				{
					delegateCommand = (this._buyFeatureLicenseCommand = new DelegateCommand(new Action(this.BuyFeatureLicense)));
				}
				return delegateCommand;
			}
		}

		public async void BuyFeatureLicense()
		{
			await this._featureOwner.BuyFeature(this._featureid);
		}

		public ICommand LearnMoreFeatureLicenseCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._learnMoreFeatureLicenseCommand) == null)
				{
					delegateCommand = (this._learnMoreFeatureLicenseCommand = new DelegateCommand(new Action(this.LearnMoreFeatureLicense)));
				}
				return delegateCommand;
			}
		}

		private void LearnMoreFeatureLicense()
		{
			this._featureOwner.LearnMore(this._featureid);
		}

		public bool TryBtnFeatureVisibility
		{
			get
			{
				return this.FeatureCanBeTrial();
			}
		}

		public bool ActivateBtnFeatureVisibility
		{
			get
			{
				return this._featureOwner.CurLicenseType != 2 && !this.FeatureCanBeTrial() && this._featureState.licenseStatus != 3;
			}
		}

		public bool BuyBtnFeatureVisibility
		{
			get
			{
				return this._featureState.licenseStatus != 3;
			}
		}

		private Localizable _bigName;

		private Drawing _iconUri;

		private Drawing _iconBoughtUri;

		private Drawing _pictureUri;

		private Drawing _pictureUriBought;

		private Localizable _description;

		private string _featureid;

		private FeatureState _featureState;

		private LicenseMainVM _featureOwner;

		private string _linkName;

		private string _price;

		protected bool _isItemSeleted;

		private DelegateCommand _goToFeatureCommand;

		private DelegateCommand _tryFeatureLicenseCommand;

		private DelegateCommand _activateFeatureLicenseCommand;

		private DelegateCommand _buyFeatureLicenseCommand;

		private DelegateCommand _learnMoreFeatureLicenseCommand;
	}
}
