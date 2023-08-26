using System;
using System.Windows.Input;
using DiscSoft.NET.Common.Utils.Clases;
using Prism.Commands;

namespace reWASDUI.ViewModels.SecondaryWindows.CalibrateGyroscope
{
	public abstract class BaseGyroPageVM : NotifyPropertyChangedObject
	{
		public abstract PageType PageType { get; }

		public bool IsProcessing
		{
			get
			{
				return this._isProcessing;
			}
			set
			{
				if (this._isProcessing != value)
				{
					this._isProcessing = value;
					this.OnPropertyChanged("IsProcessing");
					this._okCommand.RaiseCanExecuteChanged();
					this._cancelCommand.RaiseCanExecuteChanged();
					this._nextCommand.RaiseCanExecuteChanged();
				}
			}
		}

		protected BaseGyroPageVM(GyroWizardVM wizard)
		{
			this._wizard = wizard;
		}

		public void GoPage(PageType pageType)
		{
			this._wizard.GoPage(pageType);
		}

		public virtual void OnShowPage()
		{
		}

		public virtual void OnHidePage()
		{
		}

		public ICommand NextCommand
		{
			get
			{
				if (this._nextCommand == null)
				{
					this._nextCommand = new DelegateCommand(new Action(this.NavigateToNextPage), new Func<bool>(this.CanNavigateToNextPage));
				}
				return this._nextCommand;
			}
		}

		protected virtual void NavigateToNextPage()
		{
		}

		protected virtual bool CanNavigateToNextPage()
		{
			return !this.IsProcessing;
		}

		public ICommand CancelCommand
		{
			get
			{
				if (this._cancelCommand == null)
				{
					this._cancelCommand = new DelegateCommand(new Action(this.OnCancel), new Func<bool>(this.CanCancel));
				}
				return this._cancelCommand;
			}
		}

		protected virtual bool CanCancel()
		{
			return !this.IsProcessing;
		}

		protected virtual void OnCancel()
		{
			this._wizard.OnCancel();
		}

		public ICommand OkCommand
		{
			get
			{
				if (this._okCommand == null)
				{
					this._okCommand = new DelegateCommand(new Action(this.OnOk), new Func<bool>(this.CanOk));
				}
				return this._okCommand;
			}
		}

		protected void OnOk()
		{
			this._wizard.OnOk();
		}

		protected virtual bool CanOk()
		{
			return !this.IsProcessing;
		}

		protected GyroWizardVM _wizard;

		private bool _isProcessing;

		private DelegateCommand _nextCommand;

		private DelegateCommand _cancelCommand;

		private DelegateCommand _okCommand;
	}
}
