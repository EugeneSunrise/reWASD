using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using reWASDCommon.Services.HttpServer;
using reWASDCommon.Utils;
using reWASDEngine.Services.HttpServer;
using XBEliteWPF.Services.Interfaces;

namespace XBEliteWPF.Services.HttpServer
{
	public class HttpServer : ZBindableBase, IHttpServer, IDisposable
	{
		private Task<bool> RunHttp(int port)
		{
			try
			{
				string text = "*";
				if (!HttpServerSettings.IsEnabledOverLocalNetwork())
				{
					text = "localhost";
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 2);
				defaultInterpolatedStringHandler.AppendLiteral("http://");
				defaultInterpolatedStringHandler.AppendFormatted(text);
				defaultInterpolatedStringHandler.AppendLiteral(":");
				defaultInterpolatedStringHandler.AppendFormatted<int>(port);
				defaultInterpolatedStringHandler.AppendLiteral("/");
				string text2 = defaultInterpolatedStringHandler.ToStringAndClear();
				Tracer.TraceWrite(string.Format("RunHttp: {0}", text2), false);
				JsonSerializerSettings defaultSettings = new JsonSerializerSettings
				{
					Formatting = 1,
					ContractResolver = new DefaultContractResolver(),
					TraceWriter = new HttpServerTracer()
				};
				JsonConvert.DefaultSettings = () => defaultSettings;
				WebApplicationBuilder webApplicationBuilder = WebApplication.CreateBuilder();
				webApplicationBuilder.Services.AddEndpointsApiExplorer();
				NewtonsoftJsonMvcBuilderExtensions.AddNewtonsoftJson(webApplicationBuilder.Services.AddControllers(), delegate(MvcNewtonsoftJsonOptions options)
				{
					options.SerializerSettings.Formatting = 1;
					options.SerializerSettings.ContractResolver = new DefaultContractResolver();
					options.SerializerSettings.ReferenceLoopHandling = 1;
				});
				webApplicationBuilder.Services.Replace(new ServiceDescriptor(typeof(IObjectModelValidator), typeof(EmptyModelValidator), ServiceLifetime.Singleton));
				WebApplication webApplication = webApplicationBuilder.Build();
				webApplication.Urls.Add(text2);
				webApplication.MapControllers();
				webApplication.RunAsync(null);
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "RunHttp");
				return Task.FromResult<bool>(false);
			}
			return Task.FromResult<bool>(true);
		}

		private Task<bool> RunDiscovery(int port)
		{
			if (!HttpServerSettings.IsEnabledOverLocalNetwork())
			{
				return Task.FromResult<bool>(true);
			}
			try
			{
				CrossPlatformLib.ConnDeviceInfo connDeviceInfo = default(CrossPlatformLib.ConnDeviceInfo);
				connDeviceInfo.ip = HttpServerSettings.GetInterface();
				connDeviceInfo.port = port;
				connDeviceInfo.devType = 1;
				connDeviceInfo.name = HttpServerSettings.GetDeviceName();
				connDeviceInfo.devId = HttpServerSettings.GetDevId();
				connDeviceInfo.emulatorPort = HttpServerSettings.GetEmulatorPort();
				CrossPlatformLib.Initialize(null, null, connDeviceInfo, 0);
				return Task.FromResult<bool>(true);
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "RunDiscovery");
			}
			return Task.FromResult<bool>(false);
		}

		private async Task<bool> Run(int port)
		{
			await this.RunDiscovery(port);
			return await this.RunHttp(port);
		}

		public Task<bool> InitAndRun()
		{
			try
			{
				CrossPlatformLib.Init(AppContext.BaseDirectory);
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "InitAndRun");
			}
			int port = HttpServerSettings.GetPort();
			bool flag = this.Run(port).Result;
			if (!flag)
			{
				foreach (int num in HttpServerSettings.DEFAULT_PORTS)
				{
					flag = this.Run(num).Result;
					if (flag)
					{
						HttpServerSettings.SetPort(num);
						break;
					}
				}
			}
			if (!flag && HttpServerSettings.IsEnabledOverLocalNetwork())
			{
				HttpServerSettings.SetEnabledOverLocalNetwork(false);
				foreach (int num2 in HttpServerSettings.DEFAULT_PORTS)
				{
					flag = this.Run(num2).Result;
					if (flag)
					{
						HttpServerSettings.SetPort(num2);
						break;
					}
				}
			}
			return Task.FromResult<bool>(flag);
		}

		public Task<bool> Restart()
		{
			WebApplication webApplication = this.server;
			if (webApplication != null)
			{
			}
			CrossPlatformLib.Stop();
			int port = HttpServerSettings.GetPort();
			return this.Run(port);
		}

		public void StopAndClose()
		{
			WebApplication webApplication = this.server;
			if (webApplication != null)
			{
			}
			CrossPlatformLib.Stop();
			CrossPlatformLib.FreeLib();
		}

		public override void Dispose()
		{
			base.Dispose();
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			if (disposing)
			{
				this.StopAndClose();
			}
			this.disposed = true;
		}

		~HttpServer()
		{
			this.Dispose(false);
		}

		private WebApplication server;

		private bool disposed;
	}
}
