using System;
using System.Linq;
using DiscSoft.NET.Common.Utils;
using reWASDCommon.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Services;
using XBEliteWPF.DataModels.ControllerProfileInfo;

namespace reWASDUI.DataModels.ControllerProfileInfo
{
	public static class IControllerProfileInfoCollectionContainerExtensions
	{
		public static void GenerateControllerInfosChain(this IControllerProfileInfoCollectionContainer zis, BaseControllerVM baseController = null, bool force = false)
		{
			if (zis.ControllerProfileInfoCollections != null && !force)
			{
				return;
			}
			zis.ControllerProfileInfoCollections = new ControllerProfileInfoCollection[Constants.CompositeSubDevicesCount];
			for (int i = 0; i < zis.ControllerProfileInfoCollections.Length; i++)
			{
				zis.ControllerProfileInfoCollections[i] = new ControllerProfileInfoCollection();
			}
			if (baseController == null)
			{
				return;
			}
			ControllerVM controllerVM = baseController as ControllerVM;
			if (controllerVM != null)
			{
				zis.PutControllerInfoIntoChain(controllerVM, 0);
			}
			PeripheralVM peripheralVM = baseController as PeripheralVM;
			if (peripheralVM != null)
			{
				zis.PutControllerInfoIntoChain(peripheralVM, 0);
			}
			CompositeControllerVM compositeControllerVM = baseController as CompositeControllerVM;
			if (compositeControllerVM != null)
			{
				for (int j = 0; j < compositeControllerVM.BaseControllers.Count; j++)
				{
					zis.PutControllerInfoIntoChain(compositeControllerVM.BaseControllers[j], j);
				}
			}
		}

		public static void UpdateControllerInfosChainIfRequired(this IControllerProfileInfoCollectionContainer zis, BaseControllerVM baseController = null)
		{
			Tracer.TraceWrite("UpdateControllerInfosChainIfRequired for specific controller", false);
			if (zis.ControllerProfileInfoCollections == null)
			{
				Tracer.TraceWrite("ControllerProfileInfoCollections is null", false);
				return;
			}
			if (baseController == null)
			{
				Tracer.TraceWrite("Passed controller is null", false);
				return;
			}
			if (!zis.ControllerProfileInfoCollections.Any((ControllerProfileInfoCollection cpic) => cpic.ShouldRefreshControllerProfileInfos()))
			{
				Tracer.TraceWrite("Update controller chain is not required", false);
				return;
			}
			zis.GenerateControllerInfosChain(baseController, true);
			ControllerProfileInfoCollection[] controllerProfileInfoCollections = zis.ControllerProfileInfoCollections;
			for (int i = 0; i < controllerProfileInfoCollections.Length; i++)
			{
				controllerProfileInfoCollections[i].UpdateControllerTypeEnumItemsCount();
			}
		}

		public static string CalculateIDLocal(this IControllerProfileInfoCollectionContainer zis)
		{
			string text = string.Empty;
			foreach (ControllerProfileInfoCollection controllerProfileInfoCollection in zis.ControllerProfileInfoCollections)
			{
				text = text + IControllerProfileInfoCollectionContainerExtensions.CalculateID(controllerProfileInfoCollection.ControllerProfileInfos) + ";";
			}
			return text.TrimEnd(';');
		}

		public static void UpdateControllerInfosChainIfRequired(this IControllerProfileInfoCollectionContainer zis, GamepadService gs, bool force = false)
		{
			Tracer.TraceWrite("UpdateControllerInfosChainIfRequired for all controllers", false);
			if (zis.ControllerProfileInfoCollections == null)
			{
				Tracer.TraceWrite("ControllerProfileInfoCollections is null", false);
				return;
			}
			if (!force)
			{
				if (!zis.ControllerProfileInfoCollections.Any((ControllerProfileInfoCollection cpic) => cpic.ShouldRefreshControllerProfileInfos()))
				{
					Tracer.TraceWrite("Update controller chain is not required", false);
					return;
				}
			}
			foreach (BaseControllerVM baseControllerVM in gs.GamepadCollection)
			{
				string text = zis.CalculateIDLocal();
				if (string.IsNullOrEmpty(text) || baseControllerVM.CurrentController == null)
				{
					Tracer.TraceWrite("calculatedID or CurrentController is null", false);
				}
				else
				{
					string text2 = text;
					ControllerVM currentController = baseControllerVM.CurrentController;
					if (text2.Contains((currentController != null) ? currentController.ID : null))
					{
						Tracer.TraceWrite("Found controller requiring chain update", false);
						zis.GenerateControllerInfosChain(baseControllerVM, true);
						ControllerProfileInfoCollection[] controllerProfileInfoCollections = zis.ControllerProfileInfoCollections;
						for (int i = 0; i < controllerProfileInfoCollections.Length; i++)
						{
							controllerProfileInfoCollections[i].UpdateControllerTypeEnumItemsCount();
						}
					}
				}
			}
		}

		public static void PutControllerInfoIntoChain(this IControllerProfileInfoCollectionContainer zis, BaseControllerVM baseController, int index = 0)
		{
			if (baseController == null)
			{
				zis.ControllerProfileInfoCollections[index] = new ControllerProfileInfoCollection();
			}
			ControllerVM controllerVM = baseController as ControllerVM;
			if (controllerVM != null)
			{
				zis.ControllerProfileInfoCollections[index].ControllerFamily = controllerVM.ControllerFamily;
				ulong num;
				if (ulong.TryParse(controllerVM.ID, out num))
				{
					zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[0].Id = num;
				}
				zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[0].ControllerType = controllerVM.ControllerType;
				zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[0].AnalogTriggersPresent = controllerVM.IsAnalogTriggersPresent;
				zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[0].TriggerRumbleMotorPresent = controllerVM.IsTriggerRumbleMotorPresent;
				zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[0].MotorRumbleMotorPresent = controllerVM.IsMotorRumbleMotorPresent;
				zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[0].AdaptiveTriggersPresent = controllerVM.IsAdaptiveTriggersPresent;
				zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[0].GyroscopePresent = controllerVM.IsGyroscopePresent;
				zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[0].AccelerometerPresent = controllerVM.IsAccelerometerPresent;
				zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[0].TouchpadPresent = controllerVM.IsTouchpadPresent;
				zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[0].IsPS2 = controllerVM.IsPeripheralPS2;
				zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[0].ContainerId = controllerVM.ContainerId;
				zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[0].RightHandDevice = controllerVM.IsRightHandDevice;
				for (int i = 1; i < zis.ControllerProfileInfoCollections[index].ControllerProfileInfos.Length; i++)
				{
					zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[i].ResetToDefault();
				}
			}
			PeripheralVM peripheralVM = baseController as PeripheralVM;
			if (peripheralVM != null)
			{
				string[] array = peripheralVM.ID.Split(';', StringSplitOptions.None);
				zis.ControllerProfileInfoCollections[index].ControllerFamily = peripheralVM.ControllerFamily;
				for (int j = 0; j < array.Length; j++)
				{
					ulong num2;
					if (ulong.TryParse(array[j], out num2))
					{
						zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[j].Id = num2;
					}
					zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[j].ControllerType = peripheralVM.Controllers[j].ControllerType;
					zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[j].AnalogTriggersPresent = peripheralVM.Controllers[j].IsAnalogTriggersPresent;
					zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[j].TriggerRumbleMotorPresent = peripheralVM.IsTriggerRumbleMotorPresent;
					zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[j].MotorRumbleMotorPresent = peripheralVM.IsMotorRumbleMotorPresent;
					zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[j].AdaptiveTriggersPresent = peripheralVM.IsAdaptiveTriggersPresent;
					zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[j].GyroscopePresent = peripheralVM.Controllers[j].IsGyroscopePresent;
					zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[j].AccelerometerPresent = peripheralVM.Controllers[j].IsAccelerometerPresent;
					zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[j].TouchpadPresent = peripheralVM.Controllers[j].IsTouchpadPresent;
					zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[j].IsPS2 = peripheralVM.Controllers[j].IsPeripheralPS2;
					zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[j].ContainerId = peripheralVM.Controllers[j].ContainerId;
					zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[j].RightHandDevice = peripheralVM.Controllers[j].IsRightHandDevice;
				}
				for (int k = array.Length; k < zis.ControllerProfileInfoCollections[index].ControllerProfileInfos.Length; k++)
				{
					zis.ControllerProfileInfoCollections[index].ControllerProfileInfos[k].ResetToDefault();
				}
			}
		}
	}
}
