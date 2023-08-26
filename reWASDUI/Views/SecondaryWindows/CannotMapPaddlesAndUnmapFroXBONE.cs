using System;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.View.SecondaryWindows;

namespace reWASDUI.Views.SecondaryWindows
{
	public class CannotMapPaddlesAndUnmapFroXBONE : InformationalWindowWithDoNotShowLogic
	{
		protected override string DoNotShowRegistryKey
		{
			get
			{
				return "CANNOT_MAP_PADDLES_AND_UNMAP_DO_NOT_SHOW";
			}
		}

		public CannotMapPaddlesAndUnmapFroXBONE()
		{
			base.MessageTitle = DTLocalization.GetString(5016);
			base.MessageText = DTLocalization.GetString(11095);
		}
	}
}
