using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils;
using DTOverlay;
using reWASDEngine.Services.OverlayAPI;

namespace reWASDEngine.OverlayAPI.RemapWindow
{
	public partial class RemapWindow : Window, INotifyPropertyChanged
	{
		public event RemapWindow.WindowSizeChanged OnWindowSizeChanged;

		public event RemapWindow.WindowUpdated OnWindowUpdated;

		public event PropertyChangedEventHandler PropertyChanged;

		public bool IsUpdating
		{
			get
			{
				return this._isUpdating;
			}
			set
			{
				if (this._isUpdating != value)
				{
					this._isUpdating = value;
					if (!value && this.OnWindowUpdated != null)
					{
						this.OnWindowUpdated();
					}
				}
			}
		}

		public RemapWindowVM ViewModel
		{
			get
			{
				return this._dataContext;
			}
		}

		public RemapWindow(CreationRemapStyle style)
		{
			if (style == CreationRemapStyle.NormalCreation)
			{
				ResourceDictionary resourceDictionary = base.TryFindResource("RemapWindowResourcesForDisplay") as ResourceDictionary;
				base.Resources.MergedDictionaries.Add(resourceDictionary);
			}
			else if (style == CreationRemapStyle.BlackWhitePrint)
			{
				ResourceDictionary resourceDictionary2 = base.TryFindResource("RemapWindowResourcesForPrint") as ResourceDictionary;
				base.Resources.MergedDictionaries.Add(resourceDictionary2);
			}
			else
			{
				ResourceDictionary resourceDictionary3 = base.TryFindResource("RemapWindowResourcesForColorPrint") as ResourceDictionary;
				base.Resources.MergedDictionaries.Add(resourceDictionary3);
			}
			this.InitializeComponent();
			this._dataContext = new RemapWindowVM();
			base.DataContext = this._dataContext;
			this.ViewModel.Style = style;
		}

		private void UpdateSize(object sender, SizeChangedEventArgs e)
		{
			if (this.IsUpdating && base.IsVisible)
			{
				base.Hide();
			}
			if (this.OnWindowSizeChanged != null)
			{
				this.OnWindowSizeChanged();
			}
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.SourceInitialized += this.OnSourceInitialized;
			base.OnInitialized(e);
		}

		public void CalculateColumnWidth()
		{
			if (this._dataContext.ButtonTableMaxWidth >= base.ActualWidth)
			{
				return;
			}
			double num = this._dataContext.ButtonTableMaxWidth / this._dataContext.Scale;
			List<ContentPresenter> list = VisualTreeHelperExt.FindChildrenOnTheSameLevel<ContentPresenter>(this.SubConfigItems);
			for (int i = 0; i < list.Count; i++)
			{
				SubConfigDataVM subConfigDataVM = this._dataContext.SubConfigs[i];
				int visibleColumnCount = subConfigDataVM.VisibleColumnCount;
				double num2 = num / (double)visibleColumnCount;
				Grid grid = VisualTreeHelperExt.FindChild<Grid>(list[i], "Header");
				List<Tuple<int, double>> list2 = this.FillColumnsWidthAndSort(grid);
				this.ArrangeColumns(list2, num, num2, visibleColumnCount);
				this.SetColumnsMaxWidth(list2, grid, subConfigDataVM);
			}
		}

		private List<Tuple<int, double>> FillColumnsWidthAndSort(Grid headerGrid)
		{
			List<Tuple<int, double>> list = new List<Tuple<int, double>>();
			for (int i = 0; i < headerGrid.ColumnDefinitions.Count; i++)
			{
				ColumnDefinition columnDefinition = headerGrid.ColumnDefinitions[i];
				if (columnDefinition.ActualWidth > 0.0)
				{
					list.Add(new Tuple<int, double>(i, columnDefinition.ActualWidth));
				}
			}
			return list.OrderBy((Tuple<int, double> x) => x.Item2).ToList<Tuple<int, double>>();
		}

		private void ArrangeColumns(List<Tuple<int, double>> sortedColumns, double scaledMaxWidth, double columnMaxWidth, int visibleColumns)
		{
			for (int i = 0; i < sortedColumns.Count; i++)
			{
				if (sortedColumns[i].Item2 > columnMaxWidth)
				{
					double num = (double)(visibleColumns - (i + 1)) * columnMaxWidth;
					for (int j = 0; j < i; j++)
					{
						num += sortedColumns[j].Item2;
					}
					if (sortedColumns[i].Item2 + num > scaledMaxWidth)
					{
						double num2 = scaledMaxWidth - num;
						if (num2 < sortedColumns[i].Item2)
						{
							Tuple<int, double> tuple = sortedColumns[i];
							sortedColumns.RemoveAt(i);
							sortedColumns.Insert(i, new Tuple<int, double>(tuple.Item1, num2));
						}
					}
				}
			}
		}

		private void SetColumnsMaxWidth(List<Tuple<int, double>> sortedColumns, Grid headerGrid, SubConfigDataVM subConfig)
		{
			for (int i = 0; i < sortedColumns.Count; i++)
			{
				ColumnDefinition columnDefinition = headerGrid.ColumnDefinitions[sortedColumns[i].Item1];
				if (Math.Abs(sortedColumns[i].Item2 - columnDefinition.ActualWidth) >= 1E-07)
				{
					if (sortedColumns[i].Item1 == 0)
					{
						subConfig.Column0MaxWidth = sortedColumns[i].Item2;
					}
					else if (sortedColumns[i].Item1 == 1)
					{
						subConfig.Column1MaxWidth = sortedColumns[i].Item2;
					}
					else if (sortedColumns[i].Item1 == 2)
					{
						subConfig.Column2MaxWidth = sortedColumns[i].Item2;
					}
					else if (sortedColumns[i].Item1 == 3)
					{
						subConfig.Column3MaxWidth = sortedColumns[i].Item2;
					}
					else if (sortedColumns[i].Item1 == 4)
					{
						subConfig.Column4MaxWidth = sortedColumns[i].Item2;
					}
					else if (sortedColumns[i].Item1 == 5)
					{
						subConfig.Column5MaxWidth = sortedColumns[i].Item2;
					}
					else if (sortedColumns[i].Item1 == 6)
					{
						subConfig.Column6MaxWidth = sortedColumns[i].Item2;
					}
					else if (sortedColumns[i].Item1 == 7)
					{
						subConfig.Column7MaxWidth = sortedColumns[i].Item2;
					}
					else if (sortedColumns[i].Item1 == 8)
					{
						subConfig.Column8MaxWidth = sortedColumns[i].Item2;
					}
					else if (sortedColumns[i].Item1 == 9)
					{
						subConfig.Column9MaxWidth = sortedColumns[i].Item2;
					}
					else if (sortedColumns[i].Item1 == 10)
					{
						subConfig.Column10MaxWidth = sortedColumns[i].Item2;
					}
					else if (sortedColumns[i].Item1 == 11)
					{
						subConfig.Column11MaxWidth = sortedColumns[i].Item2;
					}
					else if (sortedColumns[i].Item1 == 12)
					{
						subConfig.Column12MaxWidth = sortedColumns[i].Item2;
					}
					else if (sortedColumns[i].Item1 == 13)
					{
						subConfig.Column13MaxWidth = sortedColumns[i].Item2;
					}
				}
			}
		}

		private void OnSourceInitialized(object sender, EventArgs e)
		{
			OverlayUtils.SetExtStyle(this);
		}

		private void ItemsControl_LayoutUpdated(object sender, EventArgs e)
		{
			if (this.IsUpdating)
			{
				base.Show();
				this.IsUpdating = false;
			}
		}

		public FrameworkElement PrintElement
		{
			get
			{
				return this.RemapGrid;
			}
		}

		public List<double> GetPageHeights(double maxHeight)
		{
			List<double> list = new List<double>();
			if (maxHeight <= 0.0 || this.SubConfigItems.Items.Count == 0)
			{
				return list;
			}
			List<ContentPresenter> list2 = VisualTreeHelperExt.FindChildrenOnTheSameLevel<ContentPresenter>(this.SubConfigItems);
			double scale = ((RemapWindowVM)base.DataContext).Scale;
			maxHeight /= scale;
			double num = 0.0;
			foreach (ContentPresenter contentPresenter in list2)
			{
				double actualHeight = this.RemapGrid.RowDefinitions[1].ActualHeight;
				RemapWindow.IncreaseHeight(list, ref num, actualHeight, maxHeight);
				Grid grid = VisualTreeHelperExt.FindChild<Grid>(contentPresenter, "Header");
				RemapWindow.IncreaseHeight(list, ref num, grid.ActualHeight, maxHeight);
				StackPanel stackPanel = VisualTreeHelperExt.FindChild<StackPanel>(VisualTreeHelperExt.FindChild<ItemsPresenter>(VisualTreeHelperExt.FindChild<ItemsControl>(contentPresenter, null), null), null);
				int childrenCount = VisualTreeHelper.GetChildrenCount(stackPanel);
				for (int i = 0; i < childrenCount; i++)
				{
					ContentPresenter contentPresenter2 = VisualTreeHelper.GetChild(stackPanel, i) as ContentPresenter;
					if (contentPresenter2 != null)
					{
						RemapWindow.IncreaseHeight(list, ref num, contentPresenter2.ActualHeight, maxHeight);
					}
				}
			}
			if (num > 0.0)
			{
				list.Add(num);
			}
			for (int j = 0; j < list.Count; j++)
			{
				List<double> list3 = list;
				int num2 = j;
				list3[num2] *= scale;
			}
			return list;
		}

		private static void IncreaseHeight(List<double> pages, ref double pageHeight, double ctrlHeight, double maxHeight)
		{
			if (pageHeight + ctrlHeight <= maxHeight)
			{
				pageHeight += ctrlHeight;
				return;
			}
			pages.Add(pageHeight);
			pageHeight = ctrlHeight;
		}

		private RemapWindowVM _dataContext;

		public AlignType Align;

		private bool _isUpdating;

		public bool IsDescriptionWindow;

		public delegate void WindowSizeChanged();

		public delegate void WindowUpdated();
	}
}
