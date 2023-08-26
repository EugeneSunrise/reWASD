using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using reWASDCommon.Utils;

namespace reWASDCommandLine
{
	internal class HttpUtils
	{
		public static HttpRequestMessage GetPostRequest(string json, string route)
		{
			StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");
			return new HttpRequestMessage(HttpMethod.Post, HttpUtils.GetServerAddress() + route)
			{
				Content = stringContent
			};
		}

		public static async Task<HttpResponseMessage> SendRequest(Func<HttpRequestMessage> requestFunc)
		{
			int num = 0;
			try
			{
				return await HttpUtils.client.SendAsync(requestFunc());
			}
			catch (Exception obj)
			{
				num = 1;
			}
			if (num == 1)
			{
				object obj;
				Exception e = (Exception)obj;
				TaskAwaiter<bool> taskAwaiter = UtilsCommon.TryRunEngine().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (!taskAwaiter.GetResult())
				{
					Console.WriteLine(e.Message);
					return null;
				}
				e = null;
			}
			try
			{
				return await HttpUtils.client.SendAsync(requestFunc());
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return null;
		}

		private static string GetServerAddress()
		{
			int port = HttpServerSettings.GetPort();
			string actualLocalRoute = HttpServerSettings.GetActualLocalRoute();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 3);
			defaultInterpolatedStringHandler.AppendLiteral("http://");
			defaultInterpolatedStringHandler.AppendFormatted(actualLocalRoute);
			defaultInterpolatedStringHandler.AppendLiteral(":");
			defaultInterpolatedStringHandler.AppendFormatted<int>(port);
			defaultInterpolatedStringHandler.AppendLiteral("/");
			defaultInterpolatedStringHandler.AppendFormatted("v1.7");
			defaultInterpolatedStringHandler.AppendLiteral("/GamepadService/");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		public static HttpClient client = new HttpClient();

		[DataContract]
		[JsonObject(0)]
		public class ErrorMessage
		{
			[DataMember]
			public string Message { get; set; }
		}
	}
}
