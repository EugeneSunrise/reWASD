using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using Newtonsoft.Json;
using reWASDCommon.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Services;

namespace reWASDUI.DataModels.CompositeDevicesCollection
{
	[Serializable]
	public class CompositeDevices : SortableObservableCollection<CompositeDevice>
	{
		public bool IsLoadSucess { get; private set; }

		public event CompositeDevices.CurrentEditItemChangedDelegate CurrentEditItemChanged;

		private async void AfterSetCurrentEditItem(CompositeDevice prevItem)
		{
			if (prevItem != null)
			{
				prevItem.IsEditMode = false;
				if (prevItem.SaveCommand.CanExecute())
				{
					TaskAwaiter<bool> taskAwaiter = prevItem.Save().GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						TaskAwaiter<bool> taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (!taskAwaiter.GetResult())
					{
						prevItem.RemoveCommand.Execute();
					}
				}
				else
				{
					prevItem.RemoveCommand.Execute();
				}
			}
			if (this._currentEditItem != null)
			{
				this._currentEditItem.IsEditMode = true;
			}
			CompositeDevices.CurrentEditItemChangedDelegate currentEditItemChanged = this.CurrentEditItemChanged;
			if (currentEditItemChanged != null)
			{
				currentEditItemChanged();
			}
		}

		public CompositeDevice CurrentEditItem
		{
			get
			{
				return this._currentEditItem;
			}
			set
			{
				CompositeDevice currentEditItem = this._currentEditItem;
				if (this.SetProperty(ref this._currentEditItem, value, "CurrentEditItem"))
				{
					this.AfterSetCurrentEditItem(currentEditItem);
				}
			}
		}

		public CompositeDevices()
		{
			base.CollectionChanged += this.OnCollectionChanged;
		}

		public CompositeDevices(IEnumerable<CompositeDevice> col)
			: this()
		{
			foreach (CompositeDevice compositeDevice in col)
			{
				base.Add(compositeDevice);
			}
		}

		public CompositeDevice FindCompositeForSimple(ControllerVM controller)
		{
			if (controller == null)
			{
				return null;
			}
			return this.FindCompositeForSimple(controller.ControllerId);
		}

		public CompositeDevice FindCompositeForSimple(PeripheralVM controller)
		{
			return this.FindCompositeForSimple((controller != null) ? controller.CurrentController : null);
		}

		public CompositeDevice FindCompositeForSimple(ulong controllerId)
		{
			return this.FirstOrDefault((CompositeDevice cd) => cd.ID.Contains(controllerId.ToString()));
		}

		public void RefreshHelperProperties()
		{
			foreach (CompositeDevice compositeDevice in this)
			{
				compositeDevice.RefreshHelperProperties();
			}
		}

		public bool Save(bool refreshDevices = true)
		{
			string user_IMPERSONATED_GAMEPAD_COMPOSITE_COLLECTION_FULL_SAVE_FILE_PATH = BinDataSerialize.USER_IMPERSONATED_GAMEPAD_COMPOSITE_COLLECTION_FULL_SAVE_FILE_PATH;
			Tracer.TraceWrite("CompositeDevices.Save", false);
			try
			{
				if (!Directory.Exists(Constants.PROGRAMM_DATA_DIRECTORY_PATH))
				{
					Tracer.TraceWrite("Creating " + Constants.PROGRAMM_DATA_DIRECTORY_PATH, false);
					Directory.CreateDirectory(Constants.PROGRAMM_DATA_DIRECTORY_PATH);
				}
				FileStream fileStream = DSUtils.WaitForFile(user_IMPERSONATED_GAMEPAD_COMPOSITE_COLLECTION_FULL_SAVE_FILE_PATH, FileMode.Create, FileAccess.Write, FileShare.None, 50, 100);
				if (fileStream == null)
				{
					return false;
				}
				using (fileStream)
				{
					Tracer.TraceWrite("Opened " + user_IMPERSONATED_GAMEPAD_COMPOSITE_COLLECTION_FULL_SAVE_FILE_PATH + " for writing", false);
					StreamWriter streamWriter = new StreamWriter(fileStream);
					streamWriter.Write(JsonConvert.SerializeObject(this, 1));
					streamWriter.Close();
					fileStream.Close();
				}
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "Save");
				return false;
			}
			return true;
		}

		public static CompositeDevices Load()
		{
			string user_IMPERSONATED_GAMEPAD_COMPOSITE_COLLECTION_FULL_SAVE_FILE_PATH = BinDataSerialize.USER_IMPERSONATED_GAMEPAD_COMPOSITE_COLLECTION_FULL_SAVE_FILE_PATH;
			Tracer.TraceWrite("CompositeDevices.Load", false);
			CompositeDevices compositeDevices = new CompositeDevices();
			try
			{
				if (File.Exists(user_IMPERSONATED_GAMEPAD_COMPOSITE_COLLECTION_FULL_SAVE_FILE_PATH))
				{
					FileStream fileStream = DSUtils.WaitForFile(user_IMPERSONATED_GAMEPAD_COMPOSITE_COLLECTION_FULL_SAVE_FILE_PATH, FileMode.Open, FileAccess.Read, FileShare.None, 50, 100);
					if (fileStream == null)
					{
						return compositeDevices;
					}
					using (fileStream)
					{
						StreamReader streamReader = new StreamReader(fileStream);
						string text = streamReader.ReadToEnd();
						streamReader.Close();
						compositeDevices = JsonConvert.DeserializeObject<CompositeDevices>(text);
						compositeDevices.IsLoadSucess = true;
					}
				}
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "Load");
			}
			return compositeDevices;
		}

		private async void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (CompositeDevice compositeDevice in e.NewItems.Cast<CompositeDevice>())
				{
					compositeDevice.HostCollection = this;
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (CompositeDevice item in e.OldItems.Cast<CompositeDevice>())
				{
					if (item != null)
					{
						await item.RemoveControllerRelatedEntries();
						item.HostCollection = null;
						if (this.CurrentEditItem == item)
						{
							this.CurrentEditItem = null;
						}
						item = null;
					}
				}
				IEnumerator<CompositeDevice> enumerator2 = null;
			}
		}

		protected CompositeDevices(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			foreach (SerializationEntry serializationEntry in info)
			{
				if (serializationEntry.Name == "AdditionalParameters")
				{
					this.AdditionalParameters = (Dictionary<string, string>)info.GetValue("AdditionalParameters", typeof(Dictionary<string, string>));
				}
			}
			base.CollectionChanged += this.OnCollectionChanged;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("AdditionalParameters", this.AdditionalParameters);
		}

		public override void OnDeserialization(object sender)
		{
			base.OnDeserialization(sender);
			this.ForEach(delegate(CompositeDevice cd)
			{
				cd.HostCollection = this;
			});
		}

		private CompositeDevice _currentEditItem;

		public readonly Dictionary<string, string> AdditionalParameters = new Dictionary<string, string>();

		public delegate void CurrentEditItemChangedDelegate();
	}
}
