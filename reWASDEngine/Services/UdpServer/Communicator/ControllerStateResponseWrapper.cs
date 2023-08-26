using System;

namespace reWASDEngine.Services.UdpServer.Communicator
{
	public class ControllerStateResponseWrapper<T>
	{
		public ControllerStateResponseWrapper(T serviceResponse, bool isResponseValid)
		{
			this.ServiceResponse = serviceResponse;
			this.IsResponseValid = isResponseValid;
		}

		public T ServiceResponse;

		public bool IsResponseValid;
	}
}
