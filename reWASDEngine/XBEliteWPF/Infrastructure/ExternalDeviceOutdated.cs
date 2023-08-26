using System;
using Prism.Events;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace XBEliteWPF.Infrastructure
{
	public class ExternalDeviceOutdated : PubSubEvent<ExternalDevice>
	{
	}
}
