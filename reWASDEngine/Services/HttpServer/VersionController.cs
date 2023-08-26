using System;
using Microsoft.AspNetCore.Mvc;
using reWASDCommon.Network.HTTP.DataTransferObjects;

namespace reWASDEngine.Services.HttpServer
{
	[Route("Version")]
	public class VersionController : ControllerBase
	{
		[HttpGet]
		[Route("")]
		public IActionResult Get()
		{
			return this.Ok(new HttpVersion());
		}
	}
}
