using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DiscSoft.NET.Common.Utils;
using reWASDCommon.Network.HTTP.Interfaces;
using reWASDEngine;

namespace XBEliteWPF.DataModels.GamepadActiveProfiles
{
	[Serializable]
	public class GamepadProfile : IGamepadProfile, ISerializable
	{
		public string ConfigPath
		{
			get
			{
				if (this._config != null && !string.IsNullOrEmpty(this._config.ConfigPath) && this._configPath != this._config.ConfigPath)
				{
					this._configPath = this._config.ConfigPath;
				}
				return this._configPath;
			}
			set
			{
				this._configPath = value;
			}
		}

		public string ProfileName
		{
			get
			{
				Config config = this.Config;
				if (config == null)
				{
					return null;
				}
				return config.Name;
			}
		}

		public string GameName
		{
			get
			{
				Config config = this.Config;
				if (config == null)
				{
					return null;
				}
				return config.ParentGame.Name;
			}
		}

		public Config Config
		{
			get
			{
				if (!this._isAlreadyTriedToFindProfile && this._config == null && this.ConfigPath != null)
				{
					this._isAlreadyTriedToFindProfile = true;
					this.FindAndSetProfile();
				}
				return this._config;
			}
			set
			{
				this._config = value;
			}
		}

		public GamepadProfile()
		{
		}

		public GamepadProfile(Config p)
			: this(p.ConfigPath)
		{
			this.Config = p;
		}

		public GamepadProfile(string configPath)
		{
			this.ConfigPath = configPath;
		}

		public override string ToString()
		{
			return this.ConfigPath;
		}

		public bool FindAndSetProfile()
		{
			Tracer.TraceWrite("GamepadProfile.FindAndSetProfile", false);
			this.Config = Engine.GameProfilesService.FindConfigByPath(this.ConfigPath, false);
			return this.Config != null;
		}

		protected GamepadProfile(SerializationInfo info, StreamingContext context)
			: this()
		{
			foreach (SerializationEntry serializationEntry in info)
			{
				string name = serializationEntry.Name;
				if (!(name == "ConfigPath"))
				{
					if (!(name == "ProfilePath"))
					{
						if (name == "AdditionalParameters")
						{
							this.AdditionalParameters = (Dictionary<string, string>)info.GetValue("AdditionalParameters", typeof(Dictionary<string, string>));
						}
					}
					else
					{
						this.ConfigPath = info.GetString("ProfilePath");
					}
				}
				else
				{
					this.ConfigPath = info.GetString("ConfigPath");
				}
			}
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ConfigPath", this.ConfigPath);
			info.AddValue("AdditionalParameters", this.AdditionalParameters);
		}

		private string _configPath;

		private Config _config;

		private bool _isAlreadyTriedToFindProfile;

		public readonly Dictionary<string, string> AdditionalParameters = new Dictionary<string, string>();
	}
}
