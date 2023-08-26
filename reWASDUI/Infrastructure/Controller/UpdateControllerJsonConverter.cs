using System;
using System.Collections.Generic;
using System.Linq;
using DiscSoft.NET.Common.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using reWASDCommon.Network.HTTP.Interfaces.Controllers;
using reWASDUI.DataModels.CompositeDevicesCollection;
using XBEliteWPF.DataModels.ControllerProfileInfo;

namespace reWASDUI.Infrastructure.Controller
{
	public class UpdateControllerJsonConverter : JsonConverter
	{
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			{
				return null;
			}
			JObject item = JObject.Load(reader);
			string Id = Extensions.Value<string>(item["ID"]);
			string type = Extensions.Value<string>(item["DataType"]);
			BaseControllerVM controller = null;
			Func<BaseControllerVM, bool> <>9__1;
			Func<BaseControllerVM, bool> <>9__2;
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				IEnumerable<BaseControllerVM> gamepadCollection = App.GamepadService.GamepadCollection;
				Func<BaseControllerVM, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = (BaseControllerVM gamepad) => Id == gamepad.ID);
				}
				controller = gamepadCollection.FirstOrDefault(func);
				BaseControllerVM controller3 = controller;
				if (((controller3 != null) ? controller3.DataType : null) != type)
				{
					controller = null;
				}
				if (controller != null && controller.DataType == type)
				{
					CompositeControllerVM compositeControllerVM = controller as CompositeControllerVM;
					if (compositeControllerVM != null)
					{
						compositeControllerVM.BaseControllers.Clear();
					}
					PeripheralVM peripheralVM = controller as PeripheralVM;
					if (peripheralVM != null)
					{
						peripheralVM.BaseControllers.Clear();
					}
					serializer.Populate(item.CreateReader(), controller);
					controller.UpdateGuiProperties();
					return;
				}
				IEnumerable<BaseControllerVM> allPhysicalControllers = App.GamepadService.AllPhysicalControllers;
				Func<BaseControllerVM, bool> func2;
				if ((func2 = <>9__2) == null)
				{
					func2 = (<>9__2 = (BaseControllerVM gamepad) => Id == gamepad.ID);
				}
				controller = allPhysicalControllers.FirstOrDefault(func2);
				BaseControllerVM controller2 = controller;
				if (((controller2 != null) ? controller2.DataType : null) != type)
				{
					controller = null;
				}
				if (controller != null)
				{
					PeripheralVM peripheralVM2 = controller as PeripheralVM;
					if (peripheralVM2 != null)
					{
						peripheralVM2.BaseControllers.Clear();
					}
					serializer.Populate(item.CreateReader(), controller);
					controller.UpdateGuiProperties();
				}
			}, true);
			if (controller == null)
			{
				BaseControllerVM baseControllerVM;
				if (type == "ControllerVM")
				{
					baseControllerVM = new ControllerVM();
				}
				else if (type == "PeripheralVM")
				{
					baseControllerVM = new PeripheralVM(default(Guid));
				}
				else if (type == "CompositeControllerVM")
				{
					baseControllerVM = new CompositeControllerVM(new CompositeDevice());
				}
				else
				{
					if (!(type == "OfflineDeviceVM"))
					{
						return null;
					}
					baseControllerVM = new OfflineDeviceVM(new ControllerProfileInfoCollection());
				}
				serializer.Populate(item.CreateReader(), baseControllerVM);
				baseControllerVM.Init();
				return baseControllerVM;
			}
			return controller;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			JObject.FromObject(value).WriteTo(writer, Array.Empty<JsonConverter>());
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(BaseControllerVM).IsAssignableFrom(objectType) || typeof(IBaseController).IsAssignableFrom(objectType);
		}
	}
}
