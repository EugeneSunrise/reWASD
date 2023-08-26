using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.ViewModels.Preferences.Base
{
	public abstract class PreferencesBaseVM : ZBindable
	{
		public event PreferencesBaseVM.OptionChangedDelegate OptionChanged;

		public event PreferencesBaseVM.OptionChangedDelegate DescriptionChanged;

		public event PreferencesBaseVM.OptionChangedDelegate RequiredEnableRemap;

		public event PreferencesBaseVM.OptionChangedDelegate RequiredSteamLizardReApply;

		public event PreferencesBaseVM.OptionChangedDelegate RequiredInputDeviceRefresh;

		public abstract Task<bool> ApplyChanges();

		public abstract Task Initialize();

		public virtual void Refresh()
		{
		}

		public IUserSettingsService UserSettingsService { get; set; }

		public PreferencesBaseVM()
		{
			this.UserSettingsService = App.UserSettingsService;
		}

		protected void FireOptionChanged()
		{
			PreferencesBaseVM.OptionChangedDelegate optionChanged = this.OptionChanged;
			if (optionChanged == null)
			{
				return;
			}
			optionChanged();
		}

		protected void FireRequiredEnableRemap()
		{
			if (this.RequiredEnableRemap != null)
			{
				this.RequiredEnableRemap();
			}
		}

		protected void FireRequiredSteamLizardReApply()
		{
			if (this.RequiredSteamLizardReApply != null)
			{
				this.RequiredSteamLizardReApply();
			}
		}

		protected void FireRequiredInputDeviceRefresh()
		{
			if (this.RequiredInputDeviceRefresh != null)
			{
				this.RequiredInputDeviceRefresh();
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public Localizable Description
		{
			get
			{
				return this._descritpion;
			}
			set
			{
				if (this.SetProperty<Localizable>(ref this._descritpion, value, "Description"))
				{
					PreferencesBaseVM.OptionChangedDelegate descriptionChanged = this.DescriptionChanged;
					if (descriptionChanged == null)
					{
						return;
					}
					descriptionChanged();
				}
			}
		}

		public override void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (!this.HasAttributeDefined(propertyName, typeof(DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent)))
			{
				this.FireOptionChanged();
			}
			base.OnPropertyChanged(propertyName);
		}

		private Localizable _descritpion;

		public delegate void OptionChangedDelegate();
	}
}
