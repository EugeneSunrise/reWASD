using System;
using CommandLine;

namespace reWASDCommandLine.Verbs
{
	[Verb("clear_slot", false, HelpText = "Delete config from slot(s).\nExample: reWASDCommandLine.exe clear_slot --id 318184554375544835;318934554375544834 --slot slot1")]
	internal class ClearSlotVerb
	{
		[Option("id", HelpText = "Device ID. 'all' to apply to all devices.", Required = true)]
		public string id { get; set; }

		[Option("slot", HelpText = "Device slot to be cleared. 'all, 'slot1', 'slot2', 'slot3' or 'slot4'", Required = true)]
		public string slot { get; set; }

		public const string ALL_DEVICES = "all";

		public const string ALL_SLOTS = "all";
	}
}
