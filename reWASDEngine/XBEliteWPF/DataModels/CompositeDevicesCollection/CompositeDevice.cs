using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using DiscSoft.NET.Common.Utils;
using reWASDEngine;
using XBEliteWPF.DataModels.ControllerProfileInfo;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Services;

namespace XBEliteWPF.DataModels.CompositeDevicesCollection
{
	[Serializable]
	public class CompositeDevice : ISerializable, IControllerProfileInfoCollectionContainer
	{
		public bool IsBlackListedDevice
		{
			get
			{
				return Engine.GamepadService.BlacklistGamepads.FirstOrDefault((BlackListGamepad item) => item.ID == this.ID) != null;
			}
		}

		public CompositeDevices HostCollection { get; set; }

		public ControllerProfileInfoCollection[] ControllerProfileInfoCollections { get; set; }

		public CompositeDevice()
		{
			this.GenerateControllerInfosChain(null, false);
		}

		public string ID
		{
			get
			{
				return this._id ?? IControllerProfileInfoCollectionContainerExtensions.CalculateID(this);
			}
			set
			{
				this._id = value;
			}
		}

		private BaseControllerVM GetControllerByControllerProfileInfos(ControllerProfileInfoCollection ControllerProfileInfoCollectoin)
		{
			string text = IControllerProfileInfoCollectionContainerExtensions.CalculateID(ControllerProfileInfoCollectoin.ControllerProfileInfos);
			BaseControllerVM baseControllerVM = Engine.GamepadService.FindControllerBySingleId(text);
			if (baseControllerVM == null && !string.IsNullOrEmpty(text))
			{
				return new OfflineDeviceVM(ControllerProfileInfoCollectoin);
			}
			return baseControllerVM;
		}

		public bool SetController(ref BaseControllerVM storage, BaseControllerVM value, int controllerIndex, [CallerMemberName] string propertyName = null)
		{
			BaseControllerVM baseControllerVM = storage;
			if (storage != value)
			{
				storage = value;
				if (baseControllerVM != null)
				{
					baseControllerVM.IsInsideCompositeDevice = false;
				}
				if (value != null)
				{
					value.IsInsideCompositeDevice = true;
				}
				this.PutControllerInfoIntoChain(value, controllerIndex);
				this.ID = IControllerProfileInfoCollectionContainerExtensions.CalculateID(this);
				return true;
			}
			return false;
		}

		protected CompositeDevice(SerializationInfo info, StreamingContext context)
			: this()
		{
			foreach (SerializationEntry serializationEntry in info)
			{
				string name = serializationEntry.Name;
				if (!(name == "ControllerProfileInfoCollections"))
				{
					if (name == "AdditionalParameters")
					{
						this.AdditionalParameters = (Dictionary<string, string>)info.GetValue("AdditionalParameters", typeof(Dictionary<string, string>));
					}
				}
				else
				{
					this.ControllerProfileInfoCollections = (ControllerProfileInfoCollection[])info.GetValue("ControllerProfileInfoCollections", typeof(ControllerProfileInfoCollection[]));
				}
			}
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ControllerProfileInfoCollections", this.ControllerProfileInfoCollections);
			info.AddValue("AdditionalParameters", this.AdditionalParameters);
		}

		private void TraceToFile()
		{
			Tracer.TraceWriteTag(" - Composite", ">>>>>>>>>>>>>>>>>>>>>>>>>>>>", false);
			string text = " - Composite";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
			defaultInterpolatedStringHandler.AppendLiteral("-------- \"Group \" ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this.HostCollection.IndexOf(this) + 1);
			defaultInterpolatedStringHandler.AppendLiteral(" --------");
			Tracer.TraceWriteTag(text, defaultInterpolatedStringHandler.ToStringAndClear(), false);
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				BaseControllerVM controllerByControllerProfileInfos = this.GetControllerByControllerProfileInfos(this.ControllerProfileInfoCollections[i]);
				if (controllerByControllerProfileInfos != null)
				{
					Tracer.TraceWriteTag(" - Composite", "----------------------------", false);
					string text2 = " - Composite";
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
					defaultInterpolatedStringHandler.AppendLiteral("------ \"Controller\" ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(num + 1);
					defaultInterpolatedStringHandler.AppendLiteral(" ------");
					Tracer.TraceWriteTag(text2, defaultInterpolatedStringHandler.ToStringAndClear(), false);
					switch (controllerByControllerProfileInfos.ControllerFamily)
					{
					case 0:
						Tracer.TraceWriteTag(" - Composite", "ControllerFamily: \"Gamepad\"", false);
						break;
					case 1:
						Tracer.TraceWriteTag(" - Composite", "ControllerFamily: \"Keyboard\"", false);
						break;
					case 2:
						Tracer.TraceWriteTag(" - Composite", "ControllerFamily: \"Mouse\"", false);
						break;
					case 3:
						Tracer.TraceWriteTag(" - Composite", "ControllerFamily: \"Composite\"", false);
						break;
					case 4:
						Tracer.TraceWriteTag(" - Composite", "ControllerFamily: \"Unknown\"", false);
						break;
					}
					if (controllerByControllerProfileInfos.IsSimpleDevice)
					{
						Tracer.TraceWriteTag(" - Composite", "", false);
						((ControllerVM)controllerByControllerProfileInfos).TraceControllerInfo(" - Composite");
					}
					if (controllerByControllerProfileInfos.IsPeripheralDevice)
					{
						Tracer.TraceWriteTag(" - Composite", "Peripheral", false);
						string text3 = " - Composite";
						defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 1);
						defaultInterpolatedStringHandler.AppendLiteral("Total subdevices: ");
						defaultInterpolatedStringHandler.AppendFormatted<int>(controllerByControllerProfileInfos.ConsistsOfControllersNumber);
						Tracer.TraceWriteTag(text3, defaultInterpolatedStringHandler.ToStringAndClear(), false);
						Tracer.TraceWriteTag(" - Composite", "", false);
						int num2 = 0;
						foreach (ControllerVM controllerVM in ((PeripheralVM)controllerByControllerProfileInfos).Controllers)
						{
							if (controllerVM != null)
							{
								string text4 = " - Composite";
								defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
								defaultInterpolatedStringHandler.AppendLiteral("SubDevice ");
								defaultInterpolatedStringHandler.AppendFormatted<int>(num2 + 1);
								defaultInterpolatedStringHandler.AppendLiteral(":");
								Tracer.TraceWriteTag(text4, defaultInterpolatedStringHandler.ToStringAndClear(), false);
								controllerVM.TraceControllerInfo(" - Composite");
								num2++;
							}
						}
					}
					num++;
				}
			}
			Tracer.TraceWriteTag(" - Composite", "<<<<<<<<<<<<<<<<<<<<<<<<<<<<", false);
			Tracer.TraceWriteTag(" - Composite", "", false);
		}

		private string _id;

		public readonly Dictionary<string, string> AdditionalParameters = new Dictionary<string, string>();
	}
}
