using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using DiscSoft.NET.Common.Utils;
using reWASDUI.Services.Interfaces;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.DataModels.GamepadActiveProfiles
{
	[Serializable]
	public class GamepadProfile : ZBindable, ISerializable
	{
		public string ConfigPath
		{
			get
			{
				if (this._config != null && !string.IsNullOrEmpty(this._config.ConfigPath) && this._configPath != this._config.ConfigPath)
				{
					this._configPath = this._config.ConfigPath;
					this.OnPropertyChanged("GameName");
					this.OnPropertyChanged("ProfileName");
				}
				return this._configPath;
			}
			set
			{
				this.SetProperty<string>(ref this._configPath, value, "ConfigPath");
			}
		}

		public string ProfileName
		{
			get
			{
				ConfigVM config = this.Config;
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
				ConfigVM config = this.Config;
				if (config == null)
				{
					return null;
				}
				return config.ParentGame.Name;
			}
		}

		public ConfigVM Config
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
				if (this.SetProperty<ConfigVM>(ref this._config, value, "Config"))
				{
					this.OnPropertyChanged("GameName");
					this.OnPropertyChanged("ProfileName");
				}
			}
		}

		public GamepadProfile()
		{
		}

		public GamepadProfile(ConfigVM p)
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
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 1);
			defaultInterpolatedStringHandler.AppendLiteral("GamepadProfile.GameProfilesService != null: ");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(App.GameProfilesService != null);
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			IGameProfilesService gameProfilesService = App.GameProfilesService;
			this.Config = ((gameProfilesService != null) ? gameProfilesService.FindConfigByPath(this.ConfigPath, false) : null);
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
			base.ApplyChanges();
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ConfigPath", this.ConfigPath);
			info.AddValue("AdditionalParameters", this.AdditionalParameters);
		}

		private string _configPath;

		private ConfigVM _config;

		private bool _isAlreadyTriedToFindProfile;

		public readonly Dictionary<string, string> AdditionalParameters = new Dictionary<string, string>();
	}
}
