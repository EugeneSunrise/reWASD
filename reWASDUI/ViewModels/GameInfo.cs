using System;
using System.Runtime.Serialization;
using DiscSoft.NET.Common.Localization;
using Prism.Commands;

namespace reWASDUI.ViewModels
{
	[DataContract]
	public class GameInfo
	{
		[DataMember(Name = "id")]
		public string Id { get; set; }

		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "url")]
		public string Url { get; set; }

		[DataMember(Name = "created_at")]
		public string CreatedAt { get; set; }

		[DataMember(Name = "configs_count")]
		public int ConfigsCount { get; set; }

		[DataMember(Name = "description")]
		public string Description { get; set; }

		[DataMember(Name = "box_art")]
		public BoxArt BoxArt { get; set; }

		public string ConfigsCountStr
		{
			get
			{
				return string.Format(DTLocalization.GetString(12722), this.ConfigsCount);
			}
		}

		public DelegateCommand ExploreContentCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._exploreContentCommand) == null)
				{
					delegateCommand = (this._exploreContentCommand = new DelegateCommand(delegate
					{
						this.Parrent.CurrentGame = this;
					}));
				}
				return delegateCommand;
			}
		}

		public CommunityConfigsViewVM Parrent;

		private DelegateCommand _exploreContentCommand;
	}
}
