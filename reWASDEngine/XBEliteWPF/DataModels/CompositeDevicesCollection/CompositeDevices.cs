using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using XBEliteWPF.Infrastructure.Controller;

namespace XBEliteWPF.DataModels.CompositeDevicesCollection
{
	[Serializable]
	public class CompositeDevices : SortableObservableCollection<CompositeDevice>
	{
		public CompositeDevices()
		{
		}

		public CompositeDevices(IEnumerable<CompositeDevice> col)
			: this()
		{
			foreach (CompositeDevice compositeDevice in col)
			{
				base.Add(compositeDevice);
			}
		}

		public CompositeDevice FindCompositeForSimple(ControllerVM controller)
		{
			if (controller == null)
			{
				return null;
			}
			return this.FindCompositeForSimple(controller.ControllerId);
		}

		public CompositeDevice FindCompositeForSimple(PeripheralVM controller)
		{
			return this.FindCompositeForSimple((controller != null) ? controller.CurrentController : null);
		}

		public CompositeDevice FindCompositeForSimple(ulong controllerId)
		{
			return this.FirstOrDefault((CompositeDevice cd) => cd.ID.Contains(controllerId.ToString()));
		}

		public CompositeDevice FindCompositeForSimple(string ID)
		{
			return this.FirstOrDefault((CompositeDevice cd) => cd.ID.Contains(ID));
		}

		protected CompositeDevices(SerializationInfo info, StreamingContext context)
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

		public override void OnDeserialization(object sender)
		{
			base.OnDeserialization(sender);
			this.ForEach(delegate(CompositeDevice cd)
			{
				cd.HostCollection = this;
			});
		}

		public readonly Dictionary<string, string> AdditionalParameters = new Dictionary<string, string>();
	}
}
