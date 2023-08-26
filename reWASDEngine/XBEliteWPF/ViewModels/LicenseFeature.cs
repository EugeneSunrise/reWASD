using System;
using XBEliteWPF.License;

namespace XBEliteWPF.ViewModels
{
	public class LicenseFeature : Feature
	{
		public LicenseFeature(LicenseMainVM featureOwner, string featureid, int name, int bigName, int description, string icon, string iconBought, string picture, string pictureBought, string linkname)
			: base(featureOwner, featureid, name, bigName, description, icon, iconBought, picture, pictureBought, linkname, null)
		{
			this._featureOwner = featureOwner;
		}

		public override bool IsFeatureActivated
		{
			get
			{
				return true;
			}
		}

		public override bool IsFeaturePaid
		{
			get
			{
				return this._featureOwner.CurLicenseType == 3;
			}
		}

		private LicenseMainVM _featureOwner;
	}
}
