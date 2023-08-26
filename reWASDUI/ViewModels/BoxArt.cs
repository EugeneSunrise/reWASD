using System;
using System.Runtime.Serialization;

namespace reWASDUI.ViewModels
{
	[DataContract]
	public class BoxArt
	{
		[DataMember(Name = "original")]
		public string Original { get; set; }

		[DataMember(Name = "medium")]
		public string Medium { get; set; }

		[DataMember(Name = "small")]
		public string Small { get; set; }

		public Uri SmallUrl
		{
			get
			{
				return new Uri("https://img.cdn.rewasd.com" + this.Small);
			}
		}
	}
}
