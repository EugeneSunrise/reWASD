using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using DiscSoft.NET.Common.Utils;
using reWASDCommon.Infrastructure.Enums;
using reWASDEngine;

namespace XBEliteWPF.DataModels.GamepadActiveProfiles
{
	[Serializable]
	public class GamepadProfilesCollection : ObservableDictionary<string, GamepadProfiles>
	{
		public bool IsGlobalRemapToggled
		{
			get
			{
				return !this.AdditionalParameters.ContainsKey("IsGlobalRemapToggled") || Convert.ToBoolean(this.AdditionalParameters["IsGlobalRemapToggled"]);
			}
			set
			{
				this.AdditionalParameters["IsGlobalRemapToggled"] = value.ToString();
			}
		}

		public GamepadProfilesCollection()
		{
		}

		public GamepadProfilesCollection(Dictionary<string, GamepadProfiles> controllerProfilesCollection)
		{
			foreach (KeyValuePair<string, GamepadProfiles> keyValuePair in controllerProfilesCollection)
			{
				base.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		protected GamepadProfilesCollection(SerializationInfo info, StreamingContext context)
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

		public void SetGameName(string sNewName)
		{
			IEnumerator crutchEnumerator = base.GetCrutchEnumerator();
			while (crutchEnumerator.MoveNext())
			{
				try
				{
					KeyValuePair<ulong, GamepadProfiles> keyValuePair = (KeyValuePair<ulong, GamepadProfiles>)crutchEnumerator.Current;
					foreach (KeyValuePair<Slot, GamepadProfile> keyValuePair2 in ((IEnumerable<KeyValuePair<Slot, GamepadProfile>>)keyValuePair.Value.SlotProfiles))
					{
						keyValuePair2.Value.ConfigPath = Path.Combine(Engine.UserSettingsService.ConfigsFolderPath, sNewName + "\\Controller\\" + keyValuePair2.Value.ProfileName);
					}
				}
				catch (Exception)
				{
				}
			}
		}

		public bool RemoveConfig(Config config)
		{
			return this.RemoveConfig(config.ConfigPath);
		}

		public bool RemoveConfig(string configPath)
		{
			if (string.IsNullOrEmpty(configPath))
			{
				return false;
			}
			bool flag = false;
			Func<KeyValuePair<Slot, GamepadProfile>, bool> <>9__0;
			foreach (KeyValuePair<string, GamepadProfiles> keyValuePair in ((IEnumerable<KeyValuePair<string, GamepadProfiles>>)this))
			{
				ObservableDictionary<Slot, GamepadProfile> slotProfiles = keyValuePair.Value.SlotProfiles;
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
			return flag;
		}

		public readonly Dictionary<string, string> AdditionalParameters = new Dictionary<string, string>();
	}
}
