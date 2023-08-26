using System;
using CommandLine;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDCommandLine.Verbs
{
	[Verb("select_slot", false, HelpText = "Select slot.\nExample: reWASDCommandLine.exe select_slot --id 318184554375544835;318934554375544834 --slot slot1")]
	internal class SelectSlotVerb
	{
		[Option("id", HelpText = "Device ID.", Required = true)]
		public string id { get; set; }

		[Option("slot", HelpText = "Device slot to be selected. 'slot1', 'slot2', 'slot3' or 'slot4'", Required = true)]
		public Slot slot { get; set; }
	}
}
