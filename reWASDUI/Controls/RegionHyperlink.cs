using System;
using System.Windows.Documents;

namespace reWASDUI.Controls
{
	public class RegionHyperlink : Hyperlink
	{
		public string Text { get; }

		public new string Name { get; }

		public RegionHyperlink(string text, string name)
			: base(new Run(name))
		{
			this.Text = text;
			this.Name = name;
		}
	}
}
