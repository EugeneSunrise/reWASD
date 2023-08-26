using System;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Utils.HTTP;
using Newtonsoft.Json.Linq;
using reWASDCommon._3dPartyManufacturersAPI;

namespace reWASD3rdPartyHelper._3dPartyManufacturersAPI
{
	public abstract class Base3dPartyJsonServerAPI : Base3dPartyApi
	{
		protected string ServiceAddress { get; set; }

		protected virtual async Task<string> Send(JObject jsonObject, string fullAdress)
		{
			return await HTTPHelper.PostJsonAsync(jsonObject, fullAdress);
		}

		protected virtual async Task<string> SendToServerEndpoint(JObject jsonObject, string endPoint)
		{
			return await HTTPHelper.PostJsonAsync(jsonObject, this.ServiceAddress + "/" + endPoint);
		}
	}
}
