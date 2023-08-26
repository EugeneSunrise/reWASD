using System;
using System.Collections.Generic;
using System.Linq;
using DiscSoft.NET.Common.Utils.Clases;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;

namespace reWASDUI.ViewModels.SecondaryWindows.CalibrateGyroscope
{
	public class GyroWizardVM : NotifyPropertyChangedObject
	{
		public event GyroWizardVM.CancelDelegate CancelEvent;

		public event GyroWizardVM.CancelDelegate OkEvent;

		public Gyroscope Gyro
		{
			get
			{
				return this._gyro;
			}
		}

		public GyroWizardVM(BaseControllerVM controller)
		{
			this._gyro = new Gyroscope
			{
				CurrentGamepad = controller
			};
			this._pages = new List<BaseGyroPageVM>();
			this._pages.Add(new CalibrateGyroAutoIsOnVM(this));
			this._pages.Add(new CalibrateGyroStartVM(this));
			this._pages.Add(new CalibrateGyroProcessingVM(this));
			this.CurrentPage = this._pages.First<BaseGyroPageVM>();
			App.EventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM o)
			{
				this.OnCancel();
			});
		}

		public BaseGyroPageVM CurrentPage
		{
			get
			{
				return this._currentPage;
			}
			set
			{
				if (value == this._currentPage)
				{
					return;
				}
				if (this._currentPage != null)
				{
					this._currentPage.OnHidePage();
				}
				this._currentPage = value;
				if (this._currentPage != null)
				{
					this._currentPage.OnShowPage();
				}
				this.OnPropertyChanged("CurrentPage");
			}
		}

		public void GoPage(PageType pageType)
		{
			this.CurrentPage = this._pages.FirstOrDefault((BaseGyroPageVM item) => item.PageType == pageType);
		}

		public void OnCancel()
		{
			this.CurrentPage = null;
			GyroWizardVM.CancelDelegate cancelEvent = this.CancelEvent;
			if (cancelEvent == null)
			{
				return;
			}
			cancelEvent();
		}

		public void OnOk()
		{
			this.CurrentPage = null;
			GyroWizardVM.CancelDelegate okEvent = this.OkEvent;
			if (okEvent == null)
			{
				return;
			}
			okEvent();
		}

		public void MoveNext()
		{
			this.CurrentPage.NextCommand.Execute(null);
		}

		private List<BaseGyroPageVM> _pages;

		private BaseGyroPageVM _currentPage;

		private Gyroscope _gyro;

		public delegate void CancelDelegate();
	}
}
