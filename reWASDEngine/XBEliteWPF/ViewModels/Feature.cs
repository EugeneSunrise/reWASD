using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils.Clases;
using XBEliteWPF.License;
using XBEliteWPF.License.Licensing.ComStructures;

namespace XBEliteWPF.ViewModels
{
	public class Feature : NotifyPropertyChangedObject
	{
		public Feature(LicenseMainVM featureOwner, string featureid, int name, int bigName, int description, string icon, string iconBought, string picture, string pictureBought, string linkName, string learnMoreLink = null)
		{
			this._featureOwner = featureOwner;
			this._featureid = featureid;
			this._bigName = new Localizable(bigName);
			this._bigName.PropertyChanged += delegate([Nullable(2)] object sender, PropertyChangedEventArgs args)
			{
				this.OnPropertyChanged("BigName");
				this.OnPropertyChanged("FeatureTrialDaysLeft");
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
			this.OnPropertyChanged("IsFeatureActivatedAndNotSelected");
			this.OnPropertyChanged("IsItemSelected");
			this.OnPropertyChanged("Description");
			this.OnPropertyChanged("TryBtnFeatureVisibility");
			this.OnPropertyChanged("BuyBtnFeatureVisibility");
			this.OnPropertyChanged("ActivateBtnFeatureVisibility");
			this.OnPropertyChanged("BigName");
			this.OnPropertyChanged("FeatureTrialDaysLeft");
			this.OnPropertyChanged("IsTrialFeatureVisibility");
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
			return this._featureState.licenseStatus == 2 && this._featureState.TrialDaysLeft > 0;
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

		public ICommand TryFeatureLicenseCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._tryFeatureLicenseCommand) == null)
				{
					relayCommand = (this._tryFeatureLicenseCommand = new RelayCommand(new Action(this.TryFeatureLicense)));
				}
				return relayCommand;
			}
		}

		private async void TryFeatureLicense()
		{
			await this._featureOwner.TryFeature(this._featureid);
		}

		public ICommand ActivateFeatureLicenseCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._activateFeatureLicenseCommand) == null)
				{
					relayCommand = (this._activateFeatureLicenseCommand = new RelayCommand(new Action(this.ActivateFeatureLicense)));
				}
				return relayCommand;
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
				RelayCommand relayCommand;
				if ((relayCommand = this._buyFeatureLicenseCommand) == null)
				{
					relayCommand = (this._buyFeatureLicenseCommand = new RelayCommand(new Action(this.BuyFeatureLicense)));
				}
				return relayCommand;
			}
		}

		private void BuyFeatureLicense()
		{
			this._featureOwner.BuyFeature(this._featureid);
		}

		public ICommand LearnMoreFeatureLicenseCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._learnMoreFeatureLicenseCommand) == null)
				{
					relayCommand = (this._learnMoreFeatureLicenseCommand = new RelayCommand(new Action(this.LearnMoreFeatureLicense)));
				}
				return relayCommand;
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

		private RelayCommand _tryFeatureLicenseCommand;

		private RelayCommand _activateFeatureLicenseCommand;

		private RelayCommand _buyFeatureLicenseCommand;

		private RelayCommand _learnMoreFeatureLicenseCommand;
	}
}
