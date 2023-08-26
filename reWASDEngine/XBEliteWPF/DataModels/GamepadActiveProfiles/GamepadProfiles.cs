using System;
using System.Linq;
using System.Runtime.Serialization;
using reWASDCommon.Infrastructure.Enums;
using reWASDEngine;
using XBEliteWPF.DataModels.ControllerProfileInfo;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.ViewModels.Base;

namespace XBEliteWPF.DataModels.GamepadActiveProfiles
{
	[Serializable]
	public class GamepadProfiles : BaseSerializable, IControllerProfileInfoCollectionContainer
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
				return this._id ?? IControllerProfileInfoCollectionContainerExtensions.CalculateID(this);
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
			this.GenerateControllerInfosChain(controller, false);
			this.CacheID();
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
			return Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.IsConsideredTheSameControllerByID(this.ID));
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

		private void CacheID()
		{
			this.ID = IControllerProfileInfoCollectionContainerExtensions.CalculateID(this);
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
