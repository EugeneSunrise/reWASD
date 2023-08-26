using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DiscSoft.NET.Common.Utils;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.DataModels.ControllerProfileInfo;
using XBEliteWPF.Infrastructure.Controller;

namespace XBEliteWPF.DataModels.GamepadActiveProfiles
{
	[Serializable]
	public class AutoGamesDetectionGamepadProfilesCollection : ObservableDictionary<string, GamepadProfilesCollection>
	{
		public AutoGamesDetectionGamepadProfilesCollection()
		{
		}

		protected AutoGamesDetectionGamepadProfilesCollection(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			foreach (SerializationEntry serializationEntry in info)
			{
				if (serializationEntry.Name == "AdditionalParameters")
				{
					this.AdditionalParameters = (Dictionary<string, string>)info.GetValue("AdditionalParameters", typeof(Dictionary<string, string>));
				}
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("AdditionalParameters", this.AdditionalParameters);
		}

		public void SetSlotProfile(string currentGameName, BaseControllerVM controller, Slot slot, GamepadProfile value)
		{
			string id = controller.ID;
			if (!base.ContainsKey(currentGameName))
			{
				base.Add(currentGameName, new GamepadProfilesCollection());
			}
			if (!base[currentGameName].ContainsKey(id))
			{
				base[currentGameName].Add(id, new GamepadProfiles(controller));
			}
			else
			{
				base[currentGameName][id].UpdateControllerInfosChainIfRequired(controller);
			}
			if (!base[currentGameName][id].SlotProfiles.ContainsKey(slot))
			{
				base[currentGameName][id].SlotProfiles.Add(slot, null);
			}
			base[currentGameName][id].SlotProfiles[slot] = value;
		}

		public bool RemoveConfig(Config config)
		{
			return this.RemoveConfig(config.ConfigPath);
		}

		public bool RemoveConfig(string configPath)
		{
			bool flag = false;
			Func<KeyValuePair<Slot, GamepadProfile>, bool> <>9__0;
			foreach (KeyValuePair<string, GamepadProfilesCollection> keyValuePair in ((IEnumerable<KeyValuePair<string, GamepadProfilesCollection>>)this))
			{
				foreach (KeyValuePair<string, GamepadProfiles> keyValuePair2 in ((IEnumerable<KeyValuePair<string, GamepadProfiles>>)keyValuePair.Value))
				{
					ObservableDictionary<Slot, GamepadProfile> slotProfiles = keyValuePair2.Value.SlotProfiles;
					Func<KeyValuePair<Slot, GamepadProfile>, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = delegate(KeyValuePair<Slot, GamepadProfile> kvp)
						{
							GamepadProfile value = kvp.Value;
							return ((value != null) ? value.ConfigPath : null) == configPath;
						});
					}
					flag = slotProfiles.RemoveAll(func) || flag;
				}
			}
			return flag;
		}

		public readonly Dictionary<string, string> AdditionalParameters = new Dictionary<string, string>();
	}
}
