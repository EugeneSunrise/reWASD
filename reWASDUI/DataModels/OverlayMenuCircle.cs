using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DiscSoft.NET.Common.ColorStuff;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using reWASDCommon.Infrastructure;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.KeyBindings;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Infrastructure.KeyBindingsModel.OverlayMenu;
using XBEliteWPF.Views.OverlayMenu;

namespace reWASDUI.DataModels
{
	public class OverlayMenuCircle : ZBindableBase
	{
		public BaseXBBindingCollection HostCollection { get; set; }

		public zColor SubMenuColor { get; set; }

		public OverlayMenuCircle(BaseXBBindingCollection hostCollection, int Count, bool isSubmenu, zColor color)
		{
			this.SubmenuStartAlpha = -1.5707963267948966;
			this.IsSubmenu = isSubmenu;
			this.HostCollection = hostCollection;
			this.SubMenuColor = color;
			if (Count != 0)
			{
				this.CreateSectors(Count, color);
			}
			this.MainOrSubmenu = this;
		}

		public bool IsSubmenu
		{
			get
			{
				return this._IsSubmenu;
			}
			set
			{
				this.SetProperty<bool>(ref this._IsSubmenu, value, "IsSubmenu");
			}
		}

		private void SetIsChanged()
		{
			BaseXBBindingCollection hostCollection = this.HostCollection;
			bool flag;
			if (hostCollection == null)
			{
				flag = null != null;
			}
			else
			{
				SubConfigData subConfigData = hostCollection.SubConfigData;
				flag = ((subConfigData != null) ? subConfigData.ConfigData : null) != null;
			}
			if (flag)
			{
				this.HostCollection.SubConfigData.ConfigData.IsChanged = true;
			}
		}

		public bool IsTintedBackground
		{
			get
			{
				return this._IsTintedBackground;
			}
			set
			{
				this.SetProperty<bool>(ref this._IsTintedBackground, value, "IsTintedBackground");
				this.SetIsChanged();
			}
		}

		public int TintBackground
		{
			get
			{
				return this._TintBackground;
			}
			set
			{
				this._TintBackground = value;
				this.SetIsChanged();
				this.OnPropertyChanged("TintBackground");
			}
		}

		public int Scale
		{
			get
			{
				return this._Scale;
			}
			set
			{
				this._Scale = value;
				this.SetIsChanged();
				this.OnPropertyChanged("Scale");
			}
		}

		public string Monitor
		{
			get
			{
				return this._Monitor;
			}
			set
			{
				this._Monitor = value;
				this.SetIsChanged();
				this.OnPropertyChanged("Monitor");
			}
		}

		public bool IsDelayBeforeOpening
		{
			get
			{
				return this._IsDelayBeforeOpening;
			}
			set
			{
				this.SetProperty<bool>(ref this._IsDelayBeforeOpening, value, "IsDelayBeforeOpening");
				this.SetIsChanged();
			}
		}

		public int DelayBeforeOpening
		{
			get
			{
				return this._DelayBeforeOpening;
			}
			set
			{
				this._DelayBeforeOpening = value;
				this.OnPropertyChanged("DelayBeforeOpening");
				this.SetIsChanged();
			}
		}

		public ObservableCollection<SectorItem> Sectors
		{
			get
			{
				ObservableCollection<SectorItem> observableCollection;
				if ((observableCollection = this._sectors) == null)
				{
					observableCollection = (this._sectors = new ObservableCollection<SectorItem>());
				}
				return observableCollection;
			}
		}

		public int SectorsCount
		{
			get
			{
				return this.Sectors.Count<SectorItem>();
			}
		}

		public SectorItem CurrentSector
		{
			get
			{
				return this._currentSector;
			}
			set
			{
				if (value != this._currentSector)
				{
					this._currentSector = value;
					this.OnPropertyChanged("CurrentSector");
				}
			}
		}

		public int UISectorsCount
		{
			get
			{
				if (!this.IsSubmenu)
				{
					return this.SectorsCount;
				}
				return 12;
			}
		}

		private void Update()
		{
			this.OnPropertyChanged("UISectorsCount");
			this.OnPropertyChanged("SectorsCount");
			this.OnPropertyChanged("CurrentSector");
			this.OnPropertyChanged("IsMinusButtonEnabled");
			this.OnPropertyChanged("IsPlusButtonEnabled");
		}

		public ICommand AddSectorCommandPrev
		{
			get
			{
				RelayCommand<int> relayCommand;
				if ((relayCommand = this._addSectorCommandPrev) == null)
				{
					relayCommand = (this._addSectorCommandPrev = new RelayCommand<int>(new Action<int>(this.AddSectorPrev)));
				}
				return relayCommand;
			}
		}

		private void AddSectorPrev(int idx)
		{
			this.AddSector(idx);
		}

		public ICommand AddSectorCommandNext
		{
			get
			{
				RelayCommand<int> relayCommand;
				if ((relayCommand = this._addSectorCommandNext) == null)
				{
					relayCommand = (this._addSectorCommandNext = new RelayCommand<int>(new Action<int>(this.AddSectorNext)));
				}
				return relayCommand;
			}
		}

		private void AddSectorNext(int idx)
		{
			this.AddSector(idx + 1);
		}

		private void AddSector(int idx)
		{
			if (idx < 0)
			{
				return;
			}
			if (this.IsSubmenu)
			{
				idx -= 100;
			}
			if (this.SectorsCount < (this.IsSubmenu ? 8 : 16))
			{
				SectorItem sectorItem = new SectorItem(this, this.HostCollection);
				if (idx >= this.SectorsCount)
				{
					this.Sectors.Add(sectorItem);
				}
				else
				{
					this.Sectors.Insert(idx, sectorItem);
				}
				this.RefreshSectorsInfo();
				this.OnPropertyChanged("SectorsCount");
				this.OnPropertyChanged("UISectorsCount");
				this.SetActive(-1);
				this.SetActive(idx + (this.IsSubmenu ? 100 : 0));
				this.OnPropertyChanged("IsMinusButtonEnabled");
				this.OnPropertyChanged("IsPlusButtonEnabled");
				this.OnPropertyChanged("CurrentSector");
				this.SetIsChanged();
			}
		}

		public ICommand SubmenuRemoveCommand
		{
			get
			{
				RelayCommand<int> relayCommand;
				if ((relayCommand = this._SubmenuRemove) == null)
				{
					relayCommand = (this._SubmenuRemove = new RelayCommand<int>(new Action<int>(this.SubmenuRemoveClick)));
				}
				return relayCommand;
			}
		}

		private void SubmenuRemoveClick(int idx)
		{
			if (idx >= 0 && idx < this.SectorsCount)
			{
				this.SetActive(idx);
				if (this.CurrentSector.IsSubmenuOn)
				{
					if (DTMessageBox.Show(DTLocalization.GetString(12745), MessageBoxButton.YesNo, MessageBoxImage.Question, null, false, MessageBoxResult.None) == MessageBoxResult.No)
					{
						return;
					}
					this.RemoveSubmenu(idx);
				}
			}
		}

		public ICommand SubmenuAddCommand
		{
			get
			{
				RelayCommand<int> relayCommand;
				if ((relayCommand = this._SubmenuAdd) == null)
				{
					relayCommand = (this._SubmenuAdd = new RelayCommand<int>(new Action<int>(this.SubmenuAddClick)));
				}
				return relayCommand;
			}
		}

		private void SubmenuAddClick(int idx)
		{
			if (idx >= 0 && idx < this.SectorsCount)
			{
				this.SetActive(idx);
				if (!this.CurrentSector.IsSubmenuOn)
				{
					this.AddSubmenu(idx);
				}
			}
		}

		public ICommand SubmenuCommand
		{
			get
			{
				RelayCommand<int> relayCommand;
				if ((relayCommand = this._SubmenuCommand) == null)
				{
					relayCommand = (this._SubmenuCommand = new RelayCommand<int>(new Action<int>(this.SubmenuClick)));
				}
				return relayCommand;
			}
		}

		private void SubmenuClick(int idx)
		{
			if (idx >= 0 && idx < this.SectorsCount)
			{
				this.SetActive(idx);
				if (this.CurrentSector.IsSubmenuOn)
				{
					this.SwitchSubmenu(idx);
				}
			}
		}

		public RelayCommand<int> DublicateCommand
		{
			get
			{
				RelayCommand<int> relayCommand;
				if ((relayCommand = this._DublicateCommand) == null)
				{
					relayCommand = (this._DublicateCommand = new RelayCommand<int>(new Action<int>(this.DublicateCommandClick), new Predicate<int>(this.DuplicateClickCanExecute)));
				}
				return relayCommand;
			}
		}

		private bool DuplicateClickCanExecute(int idx)
		{
			if (idx >= 100)
			{
				idx -= 100;
			}
			if (idx > this.SectorsCount - 1)
			{
				return false;
			}
			if (this.IsPlusButtonEnabled)
			{
				if (this.Sectors[idx].SelectedIcon.Resource != RadialMenuIcons.StandardIcons[0].Resource)
				{
					return true;
				}
				if (this.Sectors[idx].SectorColor.GetColor() != Constants.OverlayMenuSectorItemSectorColor)
				{
					return true;
				}
				if (this.Sectors[idx].IsSubmenuOn)
				{
					return true;
				}
				if (!this.Sectors[idx].XBBinding.IsEmpty)
				{
					return true;
				}
			}
			return false;
		}

		private void DublicateCommandClick(int idx)
		{
			if (idx >= 100)
			{
				idx -= 100;
			}
			if (idx >= 0 && idx < this.SectorsCount)
			{
				this.AddSector(idx + 1 + (this.IsSubmenu ? 100 : 0));
				this.Sectors[idx + 1].CopyFromSector(this.Sectors[idx]);
				if (this.Sectors[idx].IsSubmenuOn && this.Sectors[idx].IsSubmenuOn && this.Sectors[idx].Submenu != null)
				{
					this.Sectors[idx + 1].Submenu = new OverlayMenuCircle(this.HostCollection, 0, true, this.Sectors[idx + 1].SectorColor);
					foreach (SectorItem sectorItem in this.Sectors[idx].Submenu.Sectors)
					{
						SectorItem sectorItem2 = new SectorItem(this, this.HostCollection);
						sectorItem2.CopyFromSector(sectorItem);
						this.Sectors[idx + 1].Submenu.Sectors.Add(sectorItem2);
					}
				}
			}
			this.RefreshSectorsInfo();
			this.SetIsChanged();
			this.SetActive(-1);
		}

		public RelayCommand<int> ClearCommand
		{
			get
			{
				RelayCommand<int> relayCommand;
				if ((relayCommand = this._ClearCommand) == null)
				{
					relayCommand = (this._ClearCommand = new RelayCommand<int>(new Action<int>(this.ClearClick), new Predicate<int>(this.ClearClickCanExecute)));
				}
				return relayCommand;
			}
		}

		private bool ClearClickCanExecute(int idx)
		{
			if (idx >= 100)
			{
				idx -= 100;
			}
			return idx <= this.SectorsCount - 1 && (this.Sectors[idx].SelectedIcon.Resource != RadialMenuIcons.StandardIcons[0].Resource || this.Sectors[idx].SectorColor.GetColor() != Constants.OverlayMenuSectorItemSectorColor || this.Sectors[idx].IsSubmenuOn || !this.Sectors[idx].XBBinding.IsEmpty);
		}

		private void ClearClick(int idx)
		{
			if (idx >= 100)
			{
				idx -= 100;
			}
			if (idx >= 0 && idx < this.SectorsCount)
			{
				this.CurrentSector = this.Sectors[idx];
				if (this.CurrentSector.IsSubmenuVisible)
				{
					this.MainOrSubmenu = this;
					this.CurrentSector.IsSubmenuVisible = false;
				}
				if (!this.IsSubmenu)
				{
					this.Sectors[idx].SectorColor.SetColor(Constants.OverlayMenuSectorItemSectorColor, false);
				}
				this.Sectors[idx].SelectedIcon = RadialMenuIcons.StandardIcons[0];
				this.Sectors[idx].XBBinding.ClearBindingCommand.Execute();
				this.Sectors[idx].IsSubmenuOn = false;
				this.SetActive(-1);
			}
		}

		public RelayCommand<int?> MovePrevCommandWithCanExecute
		{
			get
			{
				RelayCommand<int?> relayCommand;
				if ((relayCommand = this._MovePrevCommandWithCanExecute) == null)
				{
					relayCommand = (this._MovePrevCommandWithCanExecute = new RelayCommand<int?>(new Action<int?>(this.MovePrevCommandClick), new Predicate<int?>(this.MovePrevCanExecute)));
				}
				return relayCommand;
			}
		}

		public RelayCommand<int?> MovePrevCommand
		{
			get
			{
				RelayCommand<int?> relayCommand;
				if ((relayCommand = this._MovePrevCommand) == null)
				{
					relayCommand = (this._MovePrevCommand = new RelayCommand<int?>(new Action<int?>(this.MovePrevCommandClick)));
				}
				return relayCommand;
			}
		}

		private bool MovePrevCanExecute(int? idx)
		{
			if (idx == null)
			{
				return false;
			}
			if (this.IsSubmenu)
			{
				int num = 0;
				int? num2 = idx - 100;
				if ((num == num2.GetValueOrDefault()) & (num2 != null))
				{
					return false;
				}
			}
			return true;
		}

		private void MovePrevCommandClick(int? idx)
		{
			if (idx != null)
			{
				this.MoveSector(idx.Value, idx.Value - 1);
			}
		}

		public RelayCommand<int?> MoveNextCommandWithCanExecute
		{
			get
			{
				RelayCommand<int?> relayCommand;
				if ((relayCommand = this._MoveNextCommandWithCanExecute) == null)
				{
					relayCommand = (this._MoveNextCommandWithCanExecute = new RelayCommand<int?>(new Action<int?>(this.MoveNextCommandClick), new Predicate<int?>(this.MoveNextCanExecute)));
				}
				return relayCommand;
			}
		}

		public RelayCommand<int?> MoveNextCommand
		{
			get
			{
				RelayCommand<int?> relayCommand;
				if ((relayCommand = this._MoveNextCommand) == null)
				{
					relayCommand = (this._MoveNextCommand = new RelayCommand<int?>(new Action<int?>(this.MoveNextCommandClick)));
				}
				return relayCommand;
			}
		}

		private bool MoveNextCanExecute(int? idx)
		{
			if (idx == null)
			{
				return false;
			}
			if (this.IsSubmenu)
			{
				int num = this.Sectors.Count - 1;
				int? num2 = idx - 100;
				if ((num == num2.GetValueOrDefault()) & (num2 != null))
				{
					return false;
				}
			}
			return true;
		}

		private void MoveNextCommandClick(int? idx)
		{
			if (idx != null)
			{
				this.MoveSector(idx.Value, idx.Value + 1);
			}
		}

		private void MoveSector(int idx, int moveToIdx)
		{
			if (idx >= 100)
			{
				idx -= 100;
				moveToIdx -= 100;
			}
			if (idx >= 0 && idx < this.SectorsCount)
			{
				SectorItem sectorItem = new SectorItem(this.MainOrSubmenu, this.HostCollection);
				sectorItem.CopyFromSector(this.Sectors[idx]);
				if (sectorItem.IsSubmenuOn && this.Sectors[idx].IsSubmenuOn && this.Sectors[idx].Submenu != null)
				{
					sectorItem.Submenu = new OverlayMenuCircle(this.HostCollection, 0, true, sectorItem.SectorColor);
					foreach (SectorItem sectorItem2 in this.Sectors[idx].Submenu.Sectors)
					{
						SectorItem sectorItem3 = new SectorItem(this, this.HostCollection);
						sectorItem3.CopyFromSector(sectorItem2);
						sectorItem.Submenu.Sectors.Add(sectorItem3);
					}
				}
				if (moveToIdx < 0)
				{
					moveToIdx = this.SectorsCount + moveToIdx;
				}
				else if (moveToIdx >= this.SectorsCount)
				{
					moveToIdx %= this.SectorsCount;
				}
				this.Sectors[idx].CopyFromSector(this.Sectors[moveToIdx]);
				if (this.Sectors[idx].IsSubmenuOn)
				{
					if (!this.Sectors[moveToIdx].IsSubmenuOn || this.Sectors[moveToIdx].Submenu == null)
					{
						goto IL_237;
					}
					this.Sectors[idx].Submenu = new OverlayMenuCircle(this.HostCollection, 0, true, this.Sectors[idx].SectorColor);
					using (IEnumerator<SectorItem> enumerator = this.Sectors[moveToIdx].Submenu.Sectors.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SectorItem sectorItem4 = enumerator.Current;
							SectorItem sectorItem5 = new SectorItem(this, this.HostCollection);
							sectorItem5.CopyFromSector(sectorItem4);
							this.Sectors[idx].Submenu.Sectors.Add(sectorItem5);
						}
						goto IL_237;
					}
				}
				if (this.Sectors[idx].Submenu != null)
				{
					this.Sectors[idx].Submenu = null;
				}
				IL_237:
				this.Sectors[moveToIdx].CopyFromSector(sectorItem);
				if (this.Sectors[moveToIdx].IsSubmenuOn)
				{
					if (!sectorItem.IsSubmenuOn || sectorItem.Submenu == null)
					{
						goto IL_32C;
					}
					this.Sectors[moveToIdx].Submenu = new OverlayMenuCircle(this.HostCollection, 0, true, this.Sectors[moveToIdx].SectorColor);
					using (IEnumerator<SectorItem> enumerator = sectorItem.Submenu.Sectors.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SectorItem sectorItem6 = enumerator.Current;
							SectorItem sectorItem7 = new SectorItem(this, this.HostCollection);
							sectorItem7.CopyFromSector(sectorItem6);
							this.Sectors[moveToIdx].Submenu.Sectors.Add(sectorItem7);
						}
						goto IL_32C;
					}
				}
				if (this.Sectors[moveToIdx].Submenu != null)
				{
					this.Sectors[moveToIdx].Submenu = null;
				}
			}
			IL_32C:
			this.RefreshSectorsInfo();
			if (this.Sectors[idx].IsSubmenuOn && this.Sectors[idx].Submenu != null)
			{
				this.Sectors[idx].Submenu.RefreshSectorsInfo();
			}
			if (this.Sectors[moveToIdx].IsSubmenuOn && this.Sectors[moveToIdx].Submenu != null)
			{
				this.Sectors[moveToIdx].Submenu.RefreshSectorsInfo();
			}
			this.SetIsChanged();
			if (this.IsSubmenu)
			{
				this.SetActive(moveToIdx + 100);
				return;
			}
			this.SetActive(moveToIdx);
		}

		public ICommand RemoveSectorCommand
		{
			get
			{
				RelayCommand<int> relayCommand;
				if ((relayCommand = this._removeSectorCommand) == null)
				{
					relayCommand = (this._removeSectorCommand = new RelayCommand<int>(new Action<int>(this.RemoveSector)));
				}
				return relayCommand;
			}
		}

		private void RemoveSector(int idx)
		{
			if (DTMessageBox.Show(DTLocalization.GetString(this.IsSubmenu ? 12744 : 12715), MessageBoxButton.YesNo, MessageBoxImage.Question, null, false, MessageBoxResult.None) == MessageBoxResult.No)
			{
				return;
			}
			if (this.IsSubmenu)
			{
				idx -= 100;
			}
			if (this.SectorsCount > (this.IsSubmenu ? 2 : 3))
			{
				if (idx >= this.SectorsCount)
				{
					idx = 0;
				}
				this.Sectors.RemoveAt(idx);
				this.CurrentSector = null;
				this.RefreshSectorsInfo();
				for (int i = 0; i < this.Sectors.Count; i++)
				{
					this.Sectors[i].IsSubmenuVisible = false;
				}
				this.OnPropertyChanged("IsMinusButtonEnabled");
				this.OnPropertyChanged("IsPlusButtonEnabled");
				this.OnPropertyChanged("IsPlusButtonEnabled");
				this.OnPropertyChanged("SectorsCount");
				this.OnPropertyChanged("RemoveSector");
				this.SetIsChanged();
			}
		}

		public bool IsPlusButtonEnabled
		{
			get
			{
				if (!this.IsSubmenu)
				{
					return this.SectorsCount < 16;
				}
				return this.SectorsCount < 8;
			}
		}

		public bool IsMinusButtonEnabled
		{
			get
			{
				if (!this.IsSubmenu)
				{
					return this.SectorsCount > 3;
				}
				return this.SectorsCount > 2;
			}
		}

		public void SetActive(int numberSector)
		{
			if (this.IsSubmenu)
			{
				numberSector -= 100;
				if (numberSector >= this.SectorsCount)
				{
					return;
				}
			}
			if (numberSector >= 100 && this.CurrentSector != null && this.CurrentSector.IsSubmenuVisible)
			{
				this.CurrentSector.Submenu.SetActive(numberSector - 100);
				this.MainOrSubmenu = this.CurrentSector.Submenu;
				return;
			}
			if (numberSector == -1 && this.CurrentSector != null && this.CurrentSector.IsSubmenuVisible)
			{
				this.CurrentSector.Submenu.SetActive(numberSector);
				this.MainOrSubmenu = this.CurrentSector.Submenu;
				return;
			}
			if (this.MainOrSubmenu.IsSubmenu && this.MainOrSubmenu.CurrentSector != null)
			{
				this.MainOrSubmenu.CurrentSector.IsActive = false;
				this.CurrentSector = null;
			}
			this.MainOrSubmenu = this;
			for (int i = 0; i < this.Sectors.Count; i++)
			{
				bool flag = false;
				if (numberSector >= this.SectorsCount)
				{
					numberSector--;
				}
				if (i == numberSector)
				{
					if (!this.Sectors[i].IsActive)
					{
						flag = true;
					}
					this.Sectors[i].IsActive = true;
				}
				else
				{
					if (this.Sectors[i].IsActive)
					{
						flag = true;
					}
					this.Sectors[i].IsActive = false;
					this.Sectors[i].IsSubmenuVisible = false;
				}
				if (flag && this.Sectors[i].IsActive)
				{
					this.CurrentSector = this.Sectors[i];
				}
			}
			if (numberSector == -1)
			{
				this.CurrentSector = null;
			}
			this.OnPropertyChanged("IsSubmenuVisible");
			this.RefreshSectorsInfo();
		}

		public void CreateSectors(int n, zColor color)
		{
			this.Sectors.Clear();
			for (int i = 0; i < n; i++)
			{
				SectorItem sectorItem = new SectorItem(this, this.HostCollection);
				sectorItem.SectorColor = color;
				this.Sectors.Add(sectorItem);
			}
			this.RefreshSectorsInfo();
		}

		private void RefreshSectorsInfo()
		{
			for (int i = 0; i < this.Sectors.Count; i++)
			{
				this.Sectors[i].NumberSector = (this.IsSubmenu ? (i + 100) : i);
			}
		}

		public RelayCommand<int> AddSubmenuCommand
		{
			get
			{
				RelayCommand<int> relayCommand;
				if ((relayCommand = this._addSubmenuCommand) == null)
				{
					relayCommand = (this._addSubmenuCommand = new RelayCommand<int>(new Action<int>(this.AddSubmenu)));
				}
				return relayCommand;
			}
		}

		private bool AddSubmenuCanExecute(int idx)
		{
			return !this.IsSubmenu && !this.Sectors[idx].IsSubmenuOn;
		}

		private void AddSubmenu(int idx)
		{
			if (idx >= 0 && idx < this.SectorsCount)
			{
				this.CurrentSector = this.Sectors[idx];
				this.CurrentSector.SetSubmenu(true);
				this.SetActive(idx);
				this.CurrentSector.XBBinding.RemoveKeyBinding();
				this.MainOrSubmenu = this.CurrentSector.Submenu;
				this.RefreshSectorsInfo();
			}
		}

		private void RemoveSubmenu(int idx)
		{
			if (idx >= 0 && idx < this.SectorsCount)
			{
				this.CurrentSector = this.Sectors[idx];
				this.CurrentSector.SetSubmenu(false);
				this.MainOrSubmenu = this;
				this.RefreshSectorsInfo();
			}
		}

		public bool IsSubmenuVisible
		{
			get
			{
				return this.CurrentSector != null && this.CurrentSector.IsSubmenuVisible;
			}
		}

		public ICommand SwitchSubmenuCommand
		{
			get
			{
				RelayCommand<int> relayCommand;
				if ((relayCommand = this._SwitchSubmenuCommand) == null)
				{
					relayCommand = (this._SwitchSubmenuCommand = new RelayCommand<int>(new Action<int>(this.SwitchSubmenu)));
				}
				return relayCommand;
			}
		}

		private void SwitchSubmenu(int idx)
		{
			if (this.CurrentSector != null)
			{
				this.CurrentSector.IsSubmenuVisible = !this.CurrentSector.IsSubmenuVisible;
				if (this.CurrentSector.IsSubmenuVisible)
				{
					this.MainOrSubmenu = this.CurrentSector.Submenu;
				}
				else
				{
					this.MainOrSubmenu = this;
				}
				this.RefreshSectorsInfo();
			}
		}

		public void CopyFromModel(OverlayMenuCircle model)
		{
			if (model == null)
			{
				return;
			}
			this.IsTintedBackground = model.IsTintedBackground;
			this.TintBackground = model.TintBackground;
			this.Scale = model.Scale;
			this.Monitor = model.Monitor;
			this.IsDelayBeforeOpening = model.IsDelayBeforeOpening;
			this.DelayBeforeOpening = model.DelayBeforeOpening;
			if (model.Sectors != null && model.Sectors.Count > 0)
			{
				this.Sectors.Clear();
				foreach (SectorItem sectorItem in model.Sectors)
				{
					SectorItem sectorItem2 = new SectorItem(this, this.HostCollection);
					sectorItem2.CopyFromModel(sectorItem);
					this.Sectors.Add(sectorItem2);
				}
			}
			this.RefreshSectorsInfo();
		}

		public void CopyToModel(OverlayMenuCircle model, BaseXBBindingCollection hostCollection)
		{
			if (model == null)
			{
				return;
			}
			model.IsTintedBackground = this.IsTintedBackground;
			model.TintBackground = this.TintBackground;
			model.Scale = this.Scale;
			model.Monitor = this.Monitor;
			model.IsDelayBeforeOpening = this.IsDelayBeforeOpening;
			model.DelayBeforeOpening = this.DelayBeforeOpening;
			if (this.Sectors != null && this.SectorsCount > 0)
			{
				model.Sectors = new List<SectorItem>();
				foreach (SectorItem sectorItem in this.Sectors)
				{
					SectorItem sectorItem2 = new SectorItem
					{
						SelectedIcon = RadialMenuIcons.StandardIcons[0],
						SectorColor = new zColor(Constants.OverlayMenuSectorItemSectorColor),
						XBBinding = null,
						Submenu = null
					};
					sectorItem.CopyToModel(sectorItem2, hostCollection);
					model.Sectors.Add(sectorItem2);
				}
			}
		}

		public OverlayMenuCircle MainOrSubmenu
		{
			get
			{
				return this._mainOrSubmenu;
			}
			set
			{
				this.SetProperty<OverlayMenuCircle>(ref this._mainOrSubmenu, value, "MainOrSubmenu");
			}
		}

		public void ClearActive()
		{
			this.MainOrSubmenu.CurrentSector = null;
			this.CurrentSector = null;
			if (this.MainOrSubmenu.Sectors.All((SectorItem item) => !item.IsActive))
			{
				this.MainOrSubmenu = this;
			}
			foreach (SectorItem sectorItem in this.MainOrSubmenu.Sectors)
			{
				sectorItem.IsActive = false;
				sectorItem.IsSubmenuVisible = false;
			}
			this.SetActive(-1);
			this.OnPropertyChanged("IsSubmenuVisible");
			this.RefreshSectorsInfo();
		}

		public double SubmenuStartAlpha;

		private bool _IsSubmenu;

		private bool _IsTintedBackground = true;

		private int _TintBackground = 40;

		private int _Scale = 60;

		private string _Monitor = "";

		private bool _IsDelayBeforeOpening;

		private int _DelayBeforeOpening = 500;

		private ObservableCollection<SectorItem> _sectors;

		private SectorItem _currentSector;

		private RelayCommand<int> _addSectorCommandPrev;

		private RelayCommand<int> _addSectorCommandNext;

		private RelayCommand<int> _SubmenuRemove;

		private RelayCommand<int> _SubmenuAdd;

		private RelayCommand<int> _SubmenuCommand;

		private RelayCommand<int> _DublicateCommand;

		private RelayCommand<int> _ClearCommand;

		private RelayCommand<int?> _MovePrevCommandWithCanExecute;

		private RelayCommand<int?> _MovePrevCommand;

		private RelayCommand<int?> _MoveNextCommandWithCanExecute;

		private RelayCommand<int?> _MoveNextCommand;

		private RelayCommand<int> _removeSectorCommand;

		private RelayCommand<int> _addSubmenuCommand;

		private RelayCommand<int> _SwitchSubmenuCommand;

		private OverlayMenuCircle _mainOrSubmenu;
	}
}
