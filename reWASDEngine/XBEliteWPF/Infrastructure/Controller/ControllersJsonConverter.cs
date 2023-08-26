using System;
using System.Collections.Generic;
using DiscSoftReWASDServiceNamespace;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using reWASDCommon.Network.HTTP.Interfaces.Controllers;
using XBEliteWPF.DataModels.CompositeDevicesCollection;
using XBEliteWPF.DataModels.ControllerProfileInfo;

namespace XBEliteWPF.Infrastructure.Controller
{
	public class ControllersJsonConverter : JsonConverter
	{
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			{
				return null;
			}
			JObject jobject = JObject.Load(reader);
			string text = Extensions.Value<string>(jobject["DataType"]);
			BaseControllerVM baseControllerVM;
			if (text == "ControllerVM")
			{
				baseControllerVM = new ControllerVM(default(REWASD_CONTROLLER_INFO), false);
			}
			else if (text == "PeripheralVM")
			{
				baseControllerVM = new PeripheralVM(default(Guid), false);
			}
			else
			{
				if (text == "CompositeControllerVM")
				{
					CompositeControllerVM compositeControllerVM = new CompositeControllerVM(new CompositeDevice(), false);
					compositeControllerVM.BaseControllers = jobject["BaseControllers"].ToObject<List<BaseControllerVM>>();
					serializer.Populate(jobject.CreateReader(), compositeControllerVM);
					return compositeControllerVM;
				}
				if (!(text == "OfflineDeviceVM"))
				{
					return null;
				}
				baseControllerVM = new OfflineDeviceVM(new ControllerProfileInfoCollection());
			}
			serializer.Populate(jobject.CreateReader(), baseControllerVM);
			return baseControllerVM;
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
