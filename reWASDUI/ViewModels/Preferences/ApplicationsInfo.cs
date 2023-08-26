using System;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using Prism.Commands;

namespace reWASDUI.ViewModels.Preferences
{
	public class ApplicationsInfo : ZBindableBase
	{
		public event RemoveApplicationDelegat RemoveApplicationEvent;

		public string NameExe
		{
			get
			{
				return this._nameExe;
			}
			set
			{
				this.SetProperty<string>(ref this._nameExe, value, "NameExe");
			}
		}

		public string ToolTipText
		{
			get
			{
				return this._tooltip;
			}
			set
			{
				this.SetProperty<string>(ref this._tooltip, value, "ToolTipText");
			}
		}

		public bool BlackList
		{
			get
			{
				return this._blackList;
			}
			set
			{
				this.SetProperty<bool>(ref this._blackList, value, "BlackList");
			}
		}

		public DelegateCommand RemoveApplication
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._removeApplication) == null)
				{
					delegateCommand = (this._removeApplication = new DelegateCommand(new Action(this.ChangeConfigsFolder)));
				}
				return delegateCommand;
			}
		}

		private void ChangeConfigsFolder()
		{
			RemoveApplicationDelegat removeApplicationEvent = this.RemoveApplicationEvent;
			if (removeApplicationEvent == null)
			{
				return;
			}
			removeApplicationEvent(this.NameExe);
		}

		public bool IsWhiteList
		{
			get
			{
				return !this.BlackList;
			}
		}

		private string _nameExe;

		private string _tooltip;

		private bool _blackList;

		private DelegateCommand _removeApplication;
	}
}
