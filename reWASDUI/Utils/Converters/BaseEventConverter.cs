using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using reWASDCommon.Network.HTTP.DataTransferObjects.Events;
using reWASDCommon.Network.HTTP.DataTransferObjects.Events.Desktop;
using reWASDUI.Infrastructure.Controller;

namespace reWASDUI.Utils.Converters
{
	public class BaseEventConverter : JsonConverter
	{
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			{
				return null;
			}
			JObject jobject = JObject.Load(reader);
			EventType eventType = jobject["Type"].ToObject<EventType>();
			switch (eventType)
			{
			case 0:
				return jobject.ToObject<HeartbeatEvent>();
			case 1:
				return jobject.ToObject<ServiceProfilesChangedEvent>();
			case 2:
				return jobject.ToObject<ConfigAppliedEvent>();
			case 3:
				return jobject.ToObject<SlotChangedEvent>();
			case 4:
				return jobject.ToObject<RemapStateChangedEvent>();
			case 5:
				return jobject.ToObject<BatteryLevelChangedEvent>();
			case 6:
				return jobject.ToObject<ShiftChangedEvent>();
			case 7:
			{
				JsonSerializer jsonSerializer = new JsonSerializer();
				jsonSerializer.Converters.Add(new ControllersJsonConverter());
				return jobject.ToObject<ControllerConnectedEvent>(jsonSerializer);
			}
			case 8:
			{
				JsonSerializer jsonSerializer2 = new JsonSerializer();
				jsonSerializer2.Converters.Add(new UpdateControllerJsonConverter());
				return jobject.ToObject<ControllerChangedEvent>(jsonSerializer2);
			}
			case 9:
			{
				JsonSerializer jsonSerializer3 = new JsonSerializer();
				jsonSerializer3.Converters.Add(new ControllersJsonConverter());
				return jobject.ToObject<ControllerDisconnectedEvent>(jsonSerializer3);
			}
			case 10:
				return jobject.ToObject<GamepadStateChangedEvent>();
			case 11:
				return jobject.ToObject<AllControllersDisconnectedEvent>();
			case 12:
				return jobject.ToObject<ConfigSavedEvent>();
			case 13:
			case 14:
				break;
			case 15:
				return jobject.ToObject<GyroCalibrationFinishedEvent>();
			case 16:
				return jobject.ToObject<LicenseChangedEvent>();
			case 17:
				return jobject.ToObject<ProfileRelationsChangedByEngineEvent>();
			case 18:
				return jobject.ToObject<ConfigRenamedEvent>();
			case 19:
				return jobject.ToObject<ConfigDeletedEvent>();
			case 20:
				return jobject.ToObject<GameRenamedEvent>();
			case 21:
				return jobject.ToObject<GameDeletedEvent>();
			case 22:
				return jobject.ToObject<ExternalDevicesChangedEvent>();
			case 23:
				return jobject.ToObject<ExternalDeviceOutdatedEvent>();
			case 24:
				return jobject.ToObject<CompositeSettingsChangedEvent>();
			case 25:
				return jobject.ToObject<HoneypotPairingRejectedEvent>();
			default:
				switch (eventType)
				{
				case 1000:
					return jobject.ToObject<AllControllersDisconnectedUIEvent>();
				case 1001:
					return jobject.ToObject<BatteryLevelChangedUIEvent>();
				case 1002:
					return jobject.ToObject<ConfigAppliedUIEvent>();
				case 1003:
				{
					JsonSerializer jsonSerializer4 = new JsonSerializer();
					jsonSerializer4.Converters.Add(new ControllersJsonConverter());
					return jobject.ToObject<ControllerConnectedUIEvent>(jsonSerializer4);
				}
				case 1004:
				{
					JsonSerializer jsonSerializer5 = new JsonSerializer();
					jsonSerializer5.Converters.Add(new ControllersJsonConverter());
					return jobject.ToObject<ControllerDisconnectedUIEvent>(jsonSerializer5);
				}
				case 1005:
					return jobject.ToObject<RemapOffUIEvent>();
				case 1006:
					return jobject.ToObject<ShiftHideUIEvent>();
				case 1007:
					return jobject.ToObject<ShiftShowUIEvent>();
				case 1008:
					return jobject.ToObject<SlotChangedUIEvent>();
				}
				break;
			}
			return null;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			JObject.FromObject(value).WriteTo(writer, Array.Empty<JsonConverter>());
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(BaseEvent).IsAssignableFrom(objectType);
		}
	}
}
