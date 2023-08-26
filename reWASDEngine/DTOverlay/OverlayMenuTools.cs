using System;
using System.Collections.Generic;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Utils.XBUtil;

namespace DTOverlay
{
	internal class OverlayMenuTools
	{
		public static void FillBoundMenuDirections(BaseControllerVM controller, string ID, ConfigData configData, List<ulong> TouchPad1IDs, List<ulong> TouchPad2IDs, List<ulong> MouseIDs, List<ulong> StickLeftIDs, List<ulong> StickRighrIDs, List<ulong> AddStickIDs, List<ulong> touchPad1ClickRequiredIDs, List<ulong> touchPad2ClickRequiredIDs)
		{
			Dictionary<string, SubConfigData> dictionary;
			XBUtils.PairDeviceIDAndSubconfigs(controller, configData, out dictionary);
			foreach (KeyValuePair<string, SubConfigData> keyValuePair in dictionary)
			{
				foreach (ShiftXBBindingCollection shiftXBBindingCollection in keyValuePair.Value.MainXBBindingCollection.ShiftXBBindingCollections)
				{
					if (shiftXBBindingCollection.IsOverlayShift)
					{
						if (shiftXBBindingCollection.Touchpad1DirectionalGroup.GetTouchpadZoneClickRequired(controller.FirstControllerType) && !shiftXBBindingCollection.Touchpad1DirectionalGroup.TouchpadAnalogMode)
						{
							OverlayMenuTools.FillIDs(keyValuePair.Key, touchPad1ClickRequiredIDs);
						}
						if (shiftXBBindingCollection.Touchpad2DirectionalGroup.GetTouchpadZoneClickRequired(controller.FirstControllerType) && !shiftXBBindingCollection.Touchpad2DirectionalGroup.TouchpadAnalogMode)
						{
							OverlayMenuTools.FillIDs(keyValuePair.Key, touchPad2ClickRequiredIDs);
						}
						if (shiftXBBindingCollection.Touchpad1DirectionalGroup.IsBoundToOverlayMenuDirections)
						{
							OverlayMenuTools.FillIDs(keyValuePair.Key, TouchPad1IDs);
						}
						if (shiftXBBindingCollection.Touchpad2DirectionalGroup.IsBoundToOverlayMenuDirections)
						{
							OverlayMenuTools.FillIDs(keyValuePair.Key, TouchPad2IDs);
						}
						if (shiftXBBindingCollection.MouseDirectionalGroup.IsBoundToOverlayMenuDirections)
						{
							OverlayMenuTools.FillIDs(keyValuePair.Key, MouseIDs);
						}
						if (shiftXBBindingCollection.LeftStickDirectionalGroup.IsBoundToOverlayMenuDirections)
						{
							OverlayMenuTools.FillIDs(keyValuePair.Key, StickLeftIDs);
						}
						if (shiftXBBindingCollection.RightStickDirectionalGroup.IsBoundToOverlayMenuDirections)
						{
							OverlayMenuTools.FillIDs(keyValuePair.Key, StickRighrIDs);
						}
						if (shiftXBBindingCollection.AdditionalStickDirectionalGroup.IsBoundToOverlayMenuDirections)
						{
							OverlayMenuTools.FillIDs(keyValuePair.Key, AddStickIDs);
						}
					}
				}
			}
		}

		private static void FillIDs(string ID, List<ulong> listIDs)
		{
			string[] array = ID.Split(';', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				listIDs.Add(ulong.Parse(array[i]));
			}
		}
	}
}
