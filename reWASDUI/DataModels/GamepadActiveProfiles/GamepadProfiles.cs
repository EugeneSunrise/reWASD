using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings;
using XBEliteWPF.DataModels.ControllerProfileInfo;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.DataModels.GamepadActiveProfiles
{
	[Serializable]
	public class GamepadProfiles : BaseSerializable
	{
		public bool IsRemapToggled
		{
			get
			{
				return this._isRemapToggled;
			}
			set
			{
				this.SetProperty<bool>(ref this._isRemapToggled, value, "IsRemapToggled");
			}
		}

		public SlotProfilesDictionary SlotProfiles
		{
			get
			{
				return this._slotProfiles;
			}
			set
			{
				this.SetProperty<SlotProfilesDictionary>(ref this._slotProfiles, value, "SlotProfiles");
			}
		}

		public ControllerProfileInfoCollection[] ControllerProfileInfoCollections { get; set; }

		public string ID
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
				this.OnPropertyChanged("ID");
			}
		}

		public GamepadProfiles(BaseControllerVM controller)
		{
			this.SlotProfiles = new SlotProfilesDictionary();
		}

		private GamepadProfiles()
		{
		}

		protected GamepadProfiles(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public BaseControllerVM GetAssociatedController()
		{
			return App.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.IsConsideredTheSameControllerByID(this.ID));
		}

		public GamepadProfiles Clone()
		{
			return new GamepadProfiles
			{
				ControllerProfileInfoCollections = this.ControllerProfileInfoCollections,
				ID = this.ID,
				SlotProfiles = new SlotProfilesDictionary(this.SlotProfiles),
				IsRemapToggled = this.IsRemapToggled
			};
		}

		public bool IsAdaptiveLeftTriggerSettingsPresent()
		{
			foreach (KeyValuePair<Slot, GamepadProfile> keyValuePair in ((IEnumerable<KeyValuePair<Slot, GamepadProfile>>)this.SlotProfiles))
			{
				ConfigVM config = keyValuePair.Value.Config;
				bool flag;
				if (config == null)
				{
					flag = false;
				}
				else
				{
					ConfigData configData = config.ConfigData;
					bool? flag2;
					if (configData == null)
					{
						flag2 = null;
					}
					else
					{
						flag2 = new bool?(configData.Any((SubConfigData x) => x.IsGamepad && x.MainXBBindingCollection != null && x.MainXBBindingCollection.IsAnyAdaptiveLeftTriggerSettingsPresent()));
					}
					bool? flag3 = flag2;
					bool flag4 = true;
					flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
				}
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsAdaptiveRightTriggerSettingsPresent()
		{
			return this.SlotProfiles.Any(delegate(KeyValuePair<Slot, GamepadProfile> slotProfile)
			{
				ConfigVM config = slotProfile.Value.Config;
				if (config == null)
				{
					return false;
				}
				ConfigData configData = config.ConfigData;
				bool? flag;
				if (configData == null)
				{
					flag = null;
				}
				else
				{
					flag = new bool?(configData.Any((SubConfigData x) => x.IsGamepad && x.MainXBBindingCollection != null && x.MainXBBindingCollection.IsAnyAdaptiveRightTriggerSettingsPresent()));
				}
				bool? flag2 = flag;
				bool flag3 = true;
				return (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
			});
		}

		private ControllerTypeEnum GetControllerType()
		{
			if (base.AdditionalParameters.ContainsKey("ControllerTypeEnum"))
			{
				return (ControllerTypeEnum)Enum.Parse(typeof(ControllerTypeEnum), base.AdditionalParameters["ControllerTypeEnum"]);
			}
			return 0;
		}

		private ControllerTypeEnum[] GetControllerTypes()
		{
			ControllerTypeEnum[] array = new ControllerTypeEnum[15];
			if (base.AdditionalParameters.ContainsKey("ControllerTypeEnum"))
			{
				array[0] = this.GetControllerType();
			}
			if (base.AdditionalParameters.ContainsKey("ControllerTypeEnums"))
			{
				string[] array2 = base.AdditionalParameters["ControllerTypeEnums"].Split(';', StringSplitOptions.None);
				for (int i = 0; i < array2.Length; i++)
				{
					array[i] = (ControllerTypeEnum)Enum.Parse(typeof(ControllerTypeEnum), array2[i]);
				}
			}
			return array;
		}

		private bool GetAnalogTriggersPresent()
		{
			return base.AdditionalParameters.ContainsKey("AnalogTriggersPresent") && bool.Parse(base.AdditionalParameters["AnalogTriggersPresent"]);
		}

		private string _id;

		private bool _isRemapToggled;

		private SlotProfilesDictionary _slotProfiles;
	}
}
