using System;
using System.Diagnostics;
using DiscSoft.NET.Common.Utils;
using Newtonsoft.Json.Serialization;

namespace reWASDEngine.Services.HttpServer
{
	public class HttpServerTracer : ITraceWriter
	{
		public TraceLevel LevelFilter
		{
			get
			{
				return TraceLevel.Warning;
			}
		}

		public void Trace(TraceLevel level, string message, Exception ex)
		{
			Tracer.TraceWrite(message, false);
			if (ex != null)
			{
				Tracer.TraceWrite(ex.Message, false);
			}
		}
	}
}
