using System;
using System.Windows.Input;
using DiscSoft.NET.Common.Utils.Clases;
using Prism.Commands;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal abstract class BasePageVM : NotifyPropertyChangedObject
	{
		public abstract PageType PageType { get; }

		public WizardVM Wizard
		{
			get
			{
				return this._wizard;
			}
		}

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
					DelegateCommand okCommand = this._okCommand;
					if (okCommand != null)
					{
						okCommand.RaiseCanExecuteChanged();
					}
					DelegateCommand cancelCommand = this._cancelCommand;
					if (cancelCommand != null)
					{
						cancelCommand.RaiseCanExecuteChanged();
					}
					DelegateCommand nextCommand = this._nextCommand;
					if (nextCommand == null)
					{
						return;
					}
					nextCommand.RaiseCanExecuteChanged();
				}
			}
		}

		protected BasePageVM(WizardVM wizard)
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

		public ICommand BackCommand
		{
			get
			{
				if (this._backCommand == null)
				{
					this._backCommand = new DelegateCommand(new Action(this.NavigatePreviousPage), new Func<bool>(this.CanNavigatePreviousPage));
				}
				return this._backCommand;
			}
		}

		protected virtual void NavigatePreviousPage()
		{
		}

		protected virtual bool CanNavigatePreviousPage()
		{
			return !this.IsProcessing;
		}

		protected WizardVM _wizard;

		private bool _isProcessing;

		private DelegateCommand _nextCommand;

		private DelegateCommand _cancelCommand;

		private DelegateCommand _okCommand;

		private DelegateCommand _backCommand;
	}
}
