using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using JsonSubTypes;
using Newtonsoft.Json;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.Infrastructure
{
	[JsonConverter(typeof(JsonSubtypes), new object[] { "IconType" })]
	[JsonSubtypes.KnownSubTypeAttribute(typeof(StandardIcon), 0)]
	[JsonSubtypes.KnownSubTypeAttribute(typeof(UserIcon), 1)]
	public class RadialMenuIcon
	{
		public RadialMenuIcon(string resource, List<RadialMenuIconCategory> categories, string description, RadialMenuIconType iconType = -1)
		{
			this.Resource = resource;
			this.Description = description;
			this.Categories = categories;
			this.IconType = iconType;
		}

		[JsonProperty("IconType")]
		public RadialMenuIconType IconType { get; }

		[JsonProperty("Resource")]
		public string Resource { get; set; }

		[JsonProperty("Description")]
		public string Description { get; set; }

		[JsonProperty("Categories")]
		public List<RadialMenuIconCategory> Categories { get; set; }

		public Drawing GetDrawing()
		{
			return Application.Current.TryFindResource(this.Resource) as Drawing;
		}

		public Drawing Drawing
		{
			get
			{
				return this.GetDrawing();
			}
		}

		public virtual RadialMenuIcon Clone()
		{
			throw new NotImplementedException();
		}
	}
}
