using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.SecondaryWindows.Base;
using Microsoft.Scripting.Utils;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.Views.SecondaryWindows
{
	public partial class ProcessListDialog : BaseSecondaryWindow
	{
		public ObservableCollection<ProcessListDialog.ProcessItem> ProcessList { get; set; }

		public List<string> InitialProcessList { get; set; }

		public ProcessListDialog()
		{
			Process[] processes = Process.GetProcesses();
			List<ProcessListDialog.ProcessItem> list = new List<ProcessListDialog.ProcessItem>();
			ImageSource imageSource = SystemIcons.Application.ToImageSource();
			foreach (Process process in processes)
			{
				list.Add(new ProcessListDialog.ProcessItem
				{
					Id = process.Id,
					Name = process.ProcessName,
					Process = process,
					Image = imageSource
				});
			}
			IEnumerable<ProcessListDialog.ProcessItem> enumerable = CollectionUtils.ToSortedList<ProcessListDialog.ProcessItem>(list, (ProcessListDialog.ProcessItem item1, ProcessListDialog.ProcessItem item2) => item1.Name.ToLower().CompareTo(item2.Name.ToLower()));
			this.ProcessList = new ObservableCollection<ProcessListDialog.ProcessItem>();
			string text = "";
			foreach (ProcessListDialog.ProcessItem processItem in enumerable)
			{
				if (text != processItem.Name)
				{
					this.ProcessList.Add(processItem);
					text = processItem.Name;
				}
			}
			this.InitializeComponent();
			base.DataContext = this;
			new Thread(delegate
			{
				using (IEnumerator<ProcessListDialog.ProcessItem> enumerator2 = this.ProcessList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ProcessListDialog.ProcessItem item = enumerator2.Current;
						try
						{
							item.FilePath = item.Name + ".exe";
							string mainModuleFileName = item.Process.GetMainModuleFileName(1024);
							item.FilePath = mainModuleFileName;
							Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(mainModuleFileName);
							ThreadHelper.ExecuteInMainDispatcher(delegate
							{
								item.Image = icon.ToImageSource();
							}, true);
						}
						catch (Exception)
						{
						}
					}
				}
				ThreadHelper.ExecuteInMainDispatcher(delegate
				{
					this.ProcessList.Remove((ProcessListDialog.ProcessItem item) => string.IsNullOrEmpty(item.FilePath));
					if (this.InitialProcessList != null)
					{
						this.ProcessList.ForEach(delegate(ProcessListDialog.ProcessItem item)
						{
							string fileName = Path.GetFileName(item.FilePath.ToLower());
							item.IsChecked = this.InitialProcessList.Any((string initialItem) => initialItem.ToLower() == fileName);
						});
					}
				}, true);
			}).Start();
		}

		public class ProcessItem : ZBindable
		{
			public int Id { get; set; }

			public string Name { get; set; }

			public string FilePath { get; set; }

			public ImageSource Image
			{
				get
				{
					return this._image;
				}
				set
				{
					this.SetProperty<ImageSource>(ref this._image, value, "Image");
				}
			}

			public bool IsChecked
			{
				get
				{
					return this._isChecked;
				}
				set
				{
					this.SetProperty<bool>(ref this._isChecked, value, "IsChecked");
				}
			}

			private ImageSource _image;

			private bool _isChecked;

			public Process Process;
		}
	}
}
