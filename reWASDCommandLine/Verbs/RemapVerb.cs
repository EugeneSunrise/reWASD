using System;
using CommandLine;

namespace reWASDCommandLine.Verbs
{
	[Verb("remap", false, HelpText = "Turn remap on/off for device(s).\nExample: reWASDCommandLine.exe remap --id 318184554375544835;318934554375544834 --state on")]
	internal class RemapVerb
	{
		[Option("id", HelpText = "Device ID. 'all' to affect all devices.", Required = true)]
		public string id { get; set; }

		[Option("state", HelpText = "Remap state. 'on' or 'off'.", Required = true)]
		public string state { get; set; }

		public const string ALL_DEVICES = "all";
	}
}
