using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using reWASDCommon.Network.HTTP.Interfaces.Controllers;
using reWASDUI.DataModels.CompositeDevicesCollection;
using XBEliteWPF.DataModels.ControllerProfileInfo;

namespace reWASDUI.Infrastructure.Controller
{
	public class ControllersJsonConverter : JsonConverter
	{
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
				baseControllerVM = new ControllerVM();
			}
			else if (text == "PeripheralVM")
			{
				baseControllerVM = new PeripheralVM(default(Guid));
			}
			else if (text == "CompositeControllerVM")
			{
				baseControllerVM = new CompositeControllerVM(new CompositeDevice());
			}
			else
			{
				if (!(text == "OfflineDeviceVM"))
				{
					return null;
				}
				baseControllerVM = new OfflineDeviceVM(new ControllerProfileInfoCollection());
			}
			serializer.Populate(jobject.CreateReader(), baseControllerVM);
			baseControllerVM.Init();
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
