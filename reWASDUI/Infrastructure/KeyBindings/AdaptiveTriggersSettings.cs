using System;
using System.Collections.ObjectModel;
using DiscSoft.NET.Common.ViewModel.BindableBase;

namespace reWASDUI.Infrastructure.KeyBindings
{
	public class AdaptiveTriggersSettings : ZBindableBase
	{
		public ObservableCollection<AdaptiveTriggerSettings> CreateAdaptiveTriggerSettings(ref AdaptiveTriggerSettings currentTriggerSettings, bool isForShift, AdaptiveTriggerSettings mainTriggerSettings)
		{
			ObservableCollection<AdaptiveTriggerSettings> observableCollection = new ObservableCollection<AdaptiveTriggerSettings>();
			if (isForShift)
			{
				AdaptiveTriggerSettings adaptiveTriggerSettings = this.CreateAdaptiveTriggerSettingsInherited(mainTriggerSettings);
				if (currentTriggerSettings == null || currentTriggerSettings.IsInherited)
				{
					currentTriggerSettings = adaptiveTriggerSettings;
				}
				observableCollection.Add(adaptiveTriggerSettings);
				observableCollection.Add(this.CreateAdaptiveTriggerSettingsDonotInherit());
			}
			else
			{
				AdaptiveTriggerSettings adaptiveTriggerSettings2 = this.CreateAdaptiveTriggerSettingsDefault();
				if (currentTriggerSettings == null)
				{
					currentTriggerSettings = adaptiveTriggerSettings2;
				}
				observableCollection.Add(adaptiveTriggerSettings2);
			}
			CollectionExtensions.AddRange<AdaptiveTriggerSettings>(observableCollection, new ObservableCollection<AdaptiveTriggerSettings>
			{
				this.CreateAdaptiveTriggerSettingsFullPress(),
				this.CreateAdaptiveTriggerSettingsSoftPress(),
				this.CreateAdaptiveTriggerSettingsMediumPress(),
				this.CreateAdaptiveTriggerSettingsHardPress(),
				this.CreateAdaptiveTriggerSettingsPulse(),
				this.CreateAdaptiveTriggerSettingsChoppy(),
				this.CreateAdaptiveTriggerSettingsRigidSoft(),
				this.CreateAdaptiveTriggerSettingsRigidMedium(),
				this.CreateAdaptiveTriggerSettingsRigidMax(),
				this.CreateAdaptiveTriggerSettingsHalfPress(),
				this.CreateAdaptiveTriggerSettingsRifle(ref currentTriggerSettings),
				this.CreateAdaptiveTriggerSettingsVibration(ref currentTriggerSettings)
			});
			return observableCollection;
		}

		public AdaptiveTriggerSettings CreateAdaptiveTriggerSettingsInherited(AdaptiveTriggerSettings parent)
		{
			AdaptiveTriggerSettings adaptiveTriggerSettings = parent.Clone();
			adaptiveTriggerSettings.Preset = 2;
			return adaptiveTriggerSettings;
		}

		private AdaptiveTriggerSettings CreateAdaptiveTriggerSettingsDonotInherit()
		{
			return new AdaptiveTriggerSettings(1);
		}

		public AdaptiveTriggerSettings CreateAdaptiveTriggerSettingsDefault()
		{
			return new AdaptiveTriggerSettings(0);
		}

		private AdaptiveTriggerSettings CreateAdaptiveTriggerSettingsFullPress()
		{
			return new AdaptiveTriggerSettings(3)
			{
				Mode = 2,
				StartResistance = 144,
				EndResistance = 160,
				Force = byte.MaxValue
			};
		}

		private AdaptiveTriggerSettings CreateAdaptiveTriggerSettingsSoftPress()
		{
			return new AdaptiveTriggerSettings(4)
			{
				Mode = 2,
				StartResistance = 112,
				EndResistance = 160,
				Force = byte.MaxValue
			};
		}

		private AdaptiveTriggerSettings CreateAdaptiveTriggerSettingsMediumPress()
		{
			return new AdaptiveTriggerSettings(5)
			{
				Mode = 2,
				StartResistance = 69,
				EndResistance = 160,
				Force = byte.MaxValue
			};
		}

		private AdaptiveTriggerSettings CreateAdaptiveTriggerSettingsHardPress()
		{
			return new AdaptiveTriggerSettings(6)
			{
				Mode = 2,
				StartResistance = 32,
				EndResistance = 160,
				Force = byte.MaxValue
			};
		}

		private AdaptiveTriggerSettings CreateAdaptiveTriggerSettingsPulse()
		{
			return new AdaptiveTriggerSettings(7)
			{
				Mode = 2
			};
		}

		private AdaptiveTriggerSettings CreateAdaptiveTriggerSettingsChoppy()
		{
			return new AdaptiveTriggerSettings(8)
			{
				Mode = 33
			};
		}

		private AdaptiveTriggerSettings CreateAdaptiveTriggerSettingsRigidSoft()
		{
			return new AdaptiveTriggerSettings(9)
			{
				Mode = 1
			};
		}

		private AdaptiveTriggerSettings CreateAdaptiveTriggerSettingsRigidMedium()
		{
			return new AdaptiveTriggerSettings(10)
			{
				Mode = 1,
				Force = 100
			};
		}

		private AdaptiveTriggerSettings CreateAdaptiveTriggerSettingsRigidMax()
		{
			return new AdaptiveTriggerSettings(11)
			{
				Mode = 1,
				Force = 220
			};
		}

		private AdaptiveTriggerSettings CreateAdaptiveTriggerSettingsHalfPress()
		{
			return new AdaptiveTriggerSettings(12)
			{
				Mode = 1,
				StartResistance = 85,
				Force = 100
			};
		}

		private AdaptiveTriggerSettings CreateAdaptiveTriggerSettingsRifle(ref AdaptiveTriggerSettings currentTriggerSettings)
		{
			AdaptiveTriggerSettings adaptiveTriggerSettings = new AdaptiveTriggerSettings(13);
			adaptiveTriggerSettings.Mode = 38;
			adaptiveTriggerSettings.StartResistance = 0;
			adaptiveTriggerSettings.Force = byte.MaxValue;
			adaptiveTriggerSettings.Strength = byte.MaxValue;
			if (currentTriggerSettings != null && currentTriggerSettings.Preset == 13)
			{
				adaptiveTriggerSettings.Frequency = currentTriggerSettings.Frequency;
			}
			else
			{
				adaptiveTriggerSettings.Frequency = 10;
			}
			return adaptiveTriggerSettings;
		}

		private AdaptiveTriggerSettings CreateAdaptiveTriggerSettingsVibration(ref AdaptiveTriggerSettings currentTriggerSettings)
		{
			AdaptiveTriggerSettings adaptiveTriggerSettings = new AdaptiveTriggerSettings(14);
			adaptiveTriggerSettings.Mode = 38;
			adaptiveTriggerSettings.StartResistance = 30;
			adaptiveTriggerSettings.Force = byte.MaxValue;
			if (currentTriggerSettings != null && currentTriggerSettings.Preset == 14)
			{
				adaptiveTriggerSettings.Strength = currentTriggerSettings.Strength;
				adaptiveTriggerSettings.Frequency = currentTriggerSettings.Frequency;
			}
			else
			{
				adaptiveTriggerSettings.Strength = 220;
				adaptiveTriggerSettings.Frequency = 30;
			}
			return adaptiveTriggerSettings;
		}
	}
}
