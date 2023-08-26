using System;
using XBEliteWPF.License;

namespace XBEliteWPF.ViewModels
{
	public class FullPackFeature : Feature
	{
		public FullPackFeature(LicenseMainVM featureOwner, string featureid, int name, int bigName, int description, string icon, string iconBought, string picture, string pictureBought, string linkname)
			: base(featureOwner, featureid, name, bigName, description, icon, iconBought, picture, pictureBought, linkname, null)
		{
		}

		public float PriceNumber;
	}
}
