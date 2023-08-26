using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using reWASDCommon.Network.HTTP.DataTransferObjects.Events;

namespace reWASDEngine.Services.HttpServer
{
	[Route("v1.7/Tests")]
	public class TestsController : ControllerBase
	{
		[HttpGet]
		[Route("AllEvents")]
		{
			Type baseType = typeof(BaseEvent);
			List<Type> list = (from t in Assembly.GetAssembly(baseType).GetTypes()
				where t != baseType && baseType.IsAssignableFrom(t)
				select t).ToList<Type>();
			List<BaseEvent> list2 = new List<BaseEvent>();
			foreach (Type type in list)
			{
				if (!type.IsAbstract)
				{
					BaseEvent baseEvent = type.GetMethod("GetTestInstance", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null) as BaseEvent;
					list2.Add(baseEvent);
				}
			}
			return this.Ok(list2);
		}
	}
}
