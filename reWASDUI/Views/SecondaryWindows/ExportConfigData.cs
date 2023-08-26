using System;
using System.IO;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using reWASDUI.DataModels;

namespace reWASDUI.Views.SecondaryWindows
{
	public class ExportConfigData : ZBindableBase
	{
		public ExportConfigData(string configPath, GameVM currentGame)
		{
			try
			{
				this.GameName = System.IO.Path.GetFileNameWithoutExtension(configPath);
				this.GamePath = configPath;
			}
			catch (Exception)
			{
			}
		}

		public bool IsAvailableToClone
		{
			get
			{
				return Directory.Exists(this.Path) && !string.IsNullOrEmpty(this.Path);
			}
		}

		public string Path
		{
			get
			{
				return this._path;
			}
			set
			{
				this.SetProperty<string>(ref this._path, value, "Path");
				this.OnPropertyChanged("IsAvailableToClone");
			}
		}

		public string GameName
		{
			get
			{
				return this._gameName;
			}
			set
			{
				this.SetProperty<string>(ref this._gameName, value, "GameName");
			}
		}

		public string GamePath
		{
			get
			{
				return this._gamePath;
			}
			set
			{
				this.SetProperty<string>(ref this._gamePath, value, "GamePath");
			}
		}

		private string _path = "";

		private string _gameName = "";

		private string _gamePath = "";
	}
}
