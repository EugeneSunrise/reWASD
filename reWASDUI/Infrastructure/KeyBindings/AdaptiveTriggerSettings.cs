using System;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Infrastructure.KeyBindings
{
	public class AdaptiveTriggerSettings : ZBindableBase, IEquatable<AdaptiveTriggerSettings>
	{
		public AdaptiveTriggerPreset Preset
		{
			get
			{
				return this._preset;
			}
			set
			{
				if (this.SetProperty<AdaptiveTriggerPreset>(ref this._preset, value, "Preset"))
				{
					this.Name = AdaptiveTriggerPresetExtensions.GetDescription(this.Preset);
					this.LocalizedName = this.GetLocalizedNameByPreset();
				}
			}
		}

		public bool IsInherited
		{
			get
			{
				return this.Preset == 2;
			}
		}

		public bool IsNonCustomSettingsVisible
		{
			get
			{
				return this.IsPresetRifle || this.IsPresetVibration;
			}
		}

		public bool IsPresetRifle
		{
			get
			{
				return this.Preset == 13;
			}
		}

		public bool IsPresetVibration
		{
			get
			{
				return this.Preset == 14;
			}
		}

		public bool IsFrequencyVisible
		{
			get
			{
				return this.IsPresetRifle || this.IsPresetVibration;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (this.SetProperty<string>(ref this._name, value, "Name"))
				{
					this.LocalizedName = this.GetLocalizedNameByName();
				}
			}
		}

		public Localizable LocalizedName
		{
			get
			{
				return this._localizedName;
			}
			set
			{
				this.SetProperty<Localizable>(ref this._localizedName, value, "LocalizedName");
			}
		}

		public bool IsCustom
		{
			get
			{
				return this._isCustom;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isCustom, value, "IsCustom"))
				{
					if (this._isCustom)
					{
						this.ClearNonExpert();
						return;
					}
					this.ClearExpert();
				}
			}
		}

		public byte Mode
		{
			get
			{
				return this._mode;
			}
			set
			{
				this.SetProperty<byte>(ref this._mode, value, "Mode");
			}
		}

		public byte StartResistance
		{
			get
			{
				return this._startResistance;
			}
			set
			{
				this.SetProperty<byte>(ref this._startResistance, value, "StartResistance");
			}
		}

		public byte EndResistance
		{
			get
			{
				return this._endResistance;
			}
			set
			{
				this.SetProperty<byte>(ref this._endResistance, value, "EndResistance");
			}
		}

		public byte Force
		{
			get
			{
				return this._force;
			}
			set
			{
				this.SetProperty<byte>(ref this._force, value, "Force");
			}
		}

		public byte Strength
		{
			get
			{
				return this._strength;
			}
			set
			{
				this.SetProperty<byte>(ref this._strength, value, "Strength");
			}
		}

		public byte Frequency
		{
			get
			{
				return this._frequency;
			}
			set
			{
				this.SetProperty<byte>(ref this._frequency, value, "Frequency");
			}
		}

		public byte Byte1
		{
			get
			{
				return this.Bytes[0];
			}
			set
			{
				this.SetProperty<byte>(ref this.Bytes[0], value, "Byte1");
			}
		}

		public byte Byte2
		{
			get
			{
				return this.Bytes[1];
			}
			set
			{
				this.SetProperty<byte>(ref this.Bytes[1], value, "Byte2");
			}
		}

		public byte Byte3
		{
			get
			{
				return this.Bytes[2];
			}
			set
			{
				this.SetProperty<byte>(ref this.Bytes[2], value, "Byte3");
			}
		}

		public byte Byte4
		{
			get
			{
				return this.Bytes[3];
			}
			set
			{
				this.SetProperty<byte>(ref this.Bytes[3], value, "Byte4");
			}
		}

		public byte Byte5
		{
			get
			{
				return this.Bytes[4];
			}
			set
			{
				this.SetProperty<byte>(ref this.Bytes[4], value, "Byte5");
			}
		}

		public byte Byte6
		{
			get
			{
				return this.Bytes[5];
			}
			set
			{
				this.SetProperty<byte>(ref this.Bytes[5], value, "Byte6");
			}
		}

		public byte Byte7
		{
			get
			{
				return this.Bytes[6];
			}
			set
			{
				this.SetProperty<byte>(ref this.Bytes[6], value, "Byte7");
			}
		}

		public byte Byte8
		{
			get
			{
				return this.Bytes[7];
			}
			set
			{
				this.SetProperty<byte>(ref this.Bytes[7], value, "Byte8");
			}
		}

		public byte Byte9
		{
			get
			{
				return this.Bytes[8];
			}
			set
			{
				this.SetProperty<byte>(ref this.Bytes[8], value, "Byte9");
			}
		}

		public byte Byte10
		{
			get
			{
				return this.Bytes[9];
			}
			set
			{
				this.SetProperty<byte>(ref this.Bytes[9], value, "Byte10");
			}
		}

		public AdaptiveTriggerSettings(AdaptiveTriggerPreset preset = 0)
		{
			this.Preset = preset;
			this.Name = AdaptiveTriggerPresetExtensions.GetDescription(this.Preset);
			this.LocalizedName = this.GetLocalizedNameByPreset();
			this.Clear();
		}

		public void Clear()
		{
			this._mode = 0;
			this.ClearExpert();
			this.ClearNonExpert();
		}

		public void ClearExpert()
		{
			this._isCustom = false;
			this.Bytes = new byte[10];
		}

		public void ClearNonExpert()
		{
			this._startResistance = 0;
			this._endResistance = 0;
			this._force = 0;
			this._strength = 0;
			this._frequency = 0;
		}

		private Localizable GetLocalizedNameByPreset()
		{
			switch (this.Preset)
			{
			case 0:
				return new Localizable(4403);
			case 1:
				return new Localizable(11795);
			case 2:
				return new Localizable(12301);
			case 3:
				return new Localizable(12297);
			case 4:
				return new Localizable(12303);
			case 5:
				return new Localizable(12304);
			case 6:
				return new Localizable(12305);
			case 7:
				return new Localizable(12298);
			case 8:
				return new Localizable(12311);
			case 9:
				return new Localizable(12299);
			case 10:
				return new Localizable(12306);
			case 11:
				return new Localizable(12307);
			case 12:
				return new Localizable(12312);
			case 13:
				return new Localizable(12308);
			case 14:
				return new Localizable(12229);
			default:
				return new Localizable();
			}
		}

		private Localizable GetLocalizedNameByName()
		{
			string name = this.Name;
			if (name != null)
			{
				switch (name.Length)
				{
				case 4:
					if (name == "None")
					{
						return new Localizable(4403);
					}
					break;
				case 5:
				{
					char c = name[0];
					if (c != 'P')
					{
						if (c == 'R')
						{
							if (name == "Rifle")
							{
								return new Localizable(12308);
							}
						}
					}
					else if (name == "Pulse")
					{
						return new Localizable(12298);
					}
					break;
				}
				case 6:
					if (name == "Choppy")
					{
						return new Localizable(12311);
					}
					break;
				case 9:
				{
					char c = name[0];
					if (c != 'I')
					{
						if (c == 'V')
						{
							if (name == "Vibration")
							{
								return new Localizable(12229);
							}
						}
					}
					else if (name == "Inherited")
					{
						return new Localizable(12301);
					}
					break;
				}
				case 10:
				{
					char c = name[3];
					if (c <= 'f')
					{
						if (c != 'd')
						{
							if (c == 'f')
							{
								if (name == "Half Press")
								{
									return new Localizable(12312);
								}
							}
						}
						else if (name == "Hard Press")
						{
							return new Localizable(12305);
						}
					}
					else if (c != 'l')
					{
						if (c == 't')
						{
							if (name == "Soft Press")
							{
								return new Localizable(12303);
							}
						}
					}
					else if (name == "Full Press")
					{
						return new Localizable(12297);
					}
					break;
				}
				case 12:
				{
					char c = name[1];
					if (c != 'a')
					{
						if (c == 'e')
						{
							if (name == "Medium Press")
							{
								return new Localizable(12304);
							}
						}
					}
					else if (name == "Max Rigidity")
					{
						return new Localizable(12307);
					}
					break;
				}
				case 13:
					if (name == "Soft Rigidity")
					{
						return new Localizable(12299);
					}
					break;
				case 14:
					if (name == "Do not inherit")
					{
						return new Localizable(11795);
					}
					break;
				case 15:
					if (name == "Medium Rigidity")
					{
						return new Localizable(12306);
					}
					break;
				}
			}
			return new Localizable();
		}

		public bool Equals(AdaptiveTriggerSettings trigger)
		{
			return true && trigger != null && trigger.Preset == this.Preset && trigger.Name == this.Name && trigger.IsCustom == this.IsCustom && trigger.Mode == this.Mode && trigger.StartResistance == this.StartResistance && trigger.EndResistance == this.EndResistance && trigger.Force == this.Force && trigger.Strength == this.Strength && trigger.Frequency == this.Frequency;
		}

		public bool AdaptiveTriggerSettingsNonDefault()
		{
			return this.Mode != 0 || (this.StartResistance != 0 || this.EndResistance != 0 || this.Force != 0 || this.Strength != 0 || this.Frequency != 0);
		}

		public void CopyTo(AdaptiveTriggerSettings item)
		{
			if (item == null)
			{
				return;
			}
			item.Preset = this._preset;
			item.Name = this._name;
			item.Mode = this._mode;
			item.StartResistance = this._startResistance;
			item.EndResistance = this._endResistance;
			item.Force = this._force;
			item.Strength = this._strength;
			item.Frequency = this._frequency;
		}

		public AdaptiveTriggerSettings Clone()
		{
			return new AdaptiveTriggerSettings(0)
			{
				Preset = this._preset,
				Name = this._name,
				Mode = this._mode,
				StartResistance = this._startResistance,
				EndResistance = this._endResistance,
				Force = this._force,
				Strength = this._strength,
				Frequency = this._frequency
			};
		}

		public void CopyFromModel(AdaptiveTriggerSettings item)
		{
			if (item == null)
			{
				return;
			}
			this.Preset = item.Preset;
			this.Name = item.Name;
			this._mode = item.Mode;
			this._startResistance = item.StartResistance;
			this._endResistance = item.EndResistance;
			this._force = item.Force;
			this._strength = item.Strength;
			this._frequency = item.Frequency;
			this._isInherited = item.IsInherited;
		}

		private AdaptiveTriggerPreset _preset;

		private string _name;

		private Localizable _localizedName;

		private bool _isCustom;

		private byte _mode;

		private byte _startResistance;

		private byte _endResistance;

		private byte _force;

		private byte _strength;

		private byte _frequency;

		private bool _isInherited;

		public byte[] Bytes = new byte[10];
	}
}
