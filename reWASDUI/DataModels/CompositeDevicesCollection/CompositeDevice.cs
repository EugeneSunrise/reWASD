using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using Prism.Commands;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDUI.DataModels.ControllerProfileInfo;
using reWASDUI.DataModels.GamepadActiveProfiles;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Services.Interfaces;
using XBEliteWPF.DataModels.ControllerProfileInfo;
using XBEliteWPF.Services;
using XBEliteWPF.Utils;

namespace reWASDUI.DataModels.CompositeDevicesCollection
{
	[Serializable]
	public class CompositeDevice : ZBindableBase, ISerializable, IControllerProfileInfoCollectionContainer
	{
		public bool IsBlackListedDevice
		{
			get
			{
				return App.GamepadService.BlacklistGamepads.FirstOrDefault((BlackListGamepad item) => item.ID == this.ID) != null;
			}
		}

		public CompositeDevices HostCollection { get; set; }

		public bool IsEditMode
		{
			get
			{
				return this._isEditMode;
			}
			set
			{
				this.SetProperty<bool>(ref this._isEditMode, value, "IsEditMode");
			}
		}

		public BaseControllerVM Controller1
		{
			get
			{
				return this._controller1;
			}
			set
			{
				this.SetController(ref this._controller1, value, 0, "Controller1");
			}
		}

		public BaseControllerVM Controller2
		{
			get
			{
				return this._controller2;
			}
			set
			{
				this.SetController(ref this._controller2, value, 1, "Controller2");
			}
		}

		public BaseControllerVM Controller3
		{
			get
			{
				return this._controller3;
			}
			set
			{
				this.SetController(ref this._controller3, value, 2, "Controller3");
			}
		}

		public BaseControllerVM Controller4
		{
			get
			{
				return this._controller4;
			}
			set
			{
				this.SetController(ref this._controller4, value, 3, "Controller4");
			}
		}

		public List<BaseControllerVM> Controllers
		{
			get
			{
				return new List<BaseControllerVM> { this.Controller1, this.Controller2, this.Controller3, this.Controller4 };
			}
		}

		public ControllerProfileInfoCollection[] ControllerProfileInfoCollections { get; set; }

		public DelegateCommand EditCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._edit) == null)
				{
					delegateCommand = (this._edit = new DelegateCommand(new Action(this.Edit)));
				}
				return delegateCommand;
			}
		}

		private void Edit()
		{
			this.HostCollection.CurrentEditItem = this;
		}

		public DelegateCommand RemoveCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._remove) == null)
				{
					delegateCommand = (this._remove = new DelegateCommand(new Action(this.Remove)));
				}
				return delegateCommand;
			}
		}

		public async void Remove()
		{
			if (this.HostCollection != null)
			{
				IGamepadService gs = App.GamepadService;
				if (!string.IsNullOrEmpty(this.ID) && App.GamepadService.GamepadProfileRelations.ContainsKey(this.ID))
				{
					await App.HttpClientService.Gamepad.ClearSlot(new ClearSlotInfo
					{
						ID = this.ID,
						Slots = new List<Slot> { 0, 1, 2, 3 }
					});
					gs.GamepadProfileRelations.Remove(this.ID);
				}
				this.HostCollection.Remove(this);
				BaseControllerVM controller = this.Controller1;
				if (!string.IsNullOrEmpty((controller != null) ? controller.ID : null))
				{
					App.GamepadService.SelectNextConnectedControllerById(this.Controller1.ID);
				}
				this.Controller1 = null;
				this.Controller2 = null;
				this.Controller3 = null;
				this.Controller4 = null;
				await gs.BinDataSerialize.SaveCompositeDevicesCollection();
			}
		}

		public DelegateCommand SaveCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._save) == null)
				{
					delegateCommand = (this._save = new DelegateCommand(delegate
					{
						this.Save();
					}, new Func<bool>(this.SaveCanExecute)));
				}
				return delegateCommand;
			}
		}

		public async Task<bool> Save()
		{
			bool flag;
			if (this.HostCollection == null)
			{
				flag = false;
			}
			else
			{
				IGamepadService gs = App.GamepadService;
				int compositeSubdevicesNumb = 0;
				List<BaseControllerVM> list = this.Controllers;
				List<Tuple<ulong, uint>> list2 = await App.HttpClientService.Engine.GetDuplicateGamepadCollection();
				if (!CompositeDevice.SendHiddenCompositeDevicesInfo(list, ref compositeSubdevicesNumb, list2))
				{
					list = null;
					string text = " - Composite";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 2);
					defaultInterpolatedStringHandler.AppendLiteral("Error: ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(compositeSubdevicesNumb);
					defaultInterpolatedStringHandler.AppendLiteral(" subdevices > max ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(15);
					Tracer.TraceWriteTag(text, defaultInterpolatedStringHandler.ToStringAndClear(), false);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 2);
					defaultInterpolatedStringHandler.AppendLiteral("CompositeDevice: Error: ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(compositeSubdevicesNumb);
					defaultInterpolatedStringHandler.AppendLiteral(" subdevices > max ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(15);
					Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
					DTMessageBox.Show(DTLocalization.GetString(11213), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
					flag = false;
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 1);
					defaultInterpolatedStringHandler.AppendLiteral("CompositeDevice: Try to save ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(compositeSubdevicesNumb);
					defaultInterpolatedStringHandler.AppendLiteral(" subdevices");
					Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
					App.GamepadService.SelectNextConnectedControllerById(IControllerProfileInfoCollectionContainerExtensions.CalculateID(this));
					await this.RemoveControllerRelatedEntries();
					await gs.BinDataSerialize.SaveCompositeDevicesCollection();
					this.HostCollection.CurrentEditItem = null;
					flag = true;
				}
			}
			return flag;
		}

		private bool SaveCanExecute()
		{
			return this.Controllers.Count((BaseControllerVM c) => c != null) > 1;
		}

		public CompositeDevice()
		{
			this.GenerateControllerInfosChain(null, false);
		}

		public string ID
		{
			get
			{
				return this._id ?? IControllerProfileInfoCollectionContainerExtensions.CalculateID(this);
			}
			set
			{
				this._id = value;
				this.OnPropertyChanged("ID");
			}
		}

		public async Task RemoveControllerRelatedEntries()
		{
			IGamepadService gs = App.GamepadService;
			IHttpClientService http = App.HttpClientService;
			bool flag = false;
			List<Slot> allSlots = new List<Slot> { 0, 1, 2, 3 };
			using (List<BaseControllerVM>.Enumerator enumerator = this.Controllers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BaseControllerVM controllerVM = enumerator.Current;
					if (controllerVM != null)
					{
						List<KeyValuePair<string, GamepadProfiles>> list = gs.GamepadProfileRelations.Where((KeyValuePair<string, GamepadProfiles> kvp) => kvp.Value.ID.Contains(controllerVM.ID) || controllerVM.IsConsideredTheSameControllerByID(kvp.Value.ID)).ToList<KeyValuePair<string, GamepadProfiles>>();
						foreach (KeyValuePair<string, GamepadProfiles> gpr in list)
						{
							if (gpr.Value != null)
							{
								await http.Gamepad.ClearSlot(new ClearSlotInfo
								{
									ID = gpr.Key,
									Slots = allSlots
								});
								gs.GamepadProfileRelations.Remove(gpr.Key);
								flag = true;
							}
							gpr = default(KeyValuePair<string, GamepadProfiles>);
						}
						List<KeyValuePair<string, GamepadProfiles>>.Enumerator enumerator2 = default(List<KeyValuePair<string, GamepadProfiles>>.Enumerator);
					}
				}
			}
			List<BaseControllerVM>.Enumerator enumerator = default(List<BaseControllerVM>.Enumerator);
			if (!string.IsNullOrEmpty(this.ID) && App.GamepadService.GamepadProfileRelations.ContainsKey(this.ID))
			{
				await http.Gamepad.ClearSlot(new ClearSlotInfo
				{
					ID = this.ID,
					Slots = allSlots
				});
				gs.GamepadProfileRelations.Remove(this.ID);
				flag = true;
			}
			if (flag)
			{
				await App.GamepadService.BinDataSerialize.LoadGamepadProfileRelations();
			}
		}

		private BaseControllerVM GetControllerByControllerProfileInfos(ControllerProfileInfoCollection ControllerProfileInfoCollectoin)
		{
			string text = IControllerProfileInfoCollectionContainerExtensions.CalculateID(ControllerProfileInfoCollectoin.ControllerProfileInfos);
			BaseControllerVM baseControllerVM = App.GamepadService.FindControllerBySingleId(text);
			if (baseControllerVM == null && !string.IsNullOrEmpty(text))
			{
				return new OfflineDeviceVM(ControllerProfileInfoCollectoin);
			}
			return baseControllerVM;
		}

		public void RefreshHelperProperties()
		{
			this._controller1 = this.GetControllerByControllerProfileInfos(this.ControllerProfileInfoCollections[0]);
			this.OnPropertyChanged("Controller1");
			this._controller2 = this.GetControllerByControllerProfileInfos(this.ControllerProfileInfoCollections[1]);
			this.OnPropertyChanged("Controller2");
			this._controller3 = this.GetControllerByControllerProfileInfos(this.ControllerProfileInfoCollections[2]);
			this.OnPropertyChanged("Controller3");
			this._controller4 = this.GetControllerByControllerProfileInfos(this.ControllerProfileInfoCollections[3]);
			this.OnPropertyChanged("Controller4");
			this.ID = IControllerProfileInfoCollectionContainerExtensions.CalculateID(this);
		}

		private bool SetController(ref BaseControllerVM storage, BaseControllerVM value, int controllerIndex, [CallerMemberName] string propertyName = null)
		{
			BaseControllerVM baseControllerVM = storage;
			if (this.SetProperty<BaseControllerVM>(ref storage, value, propertyName))
			{
				if (baseControllerVM != null)
				{
					baseControllerVM.IsInsideCompositeDevice = false;
				}
				if (value != null)
				{
					value.IsInsideCompositeDevice = true;
				}
				this.PutControllerInfoIntoChain(value, controllerIndex);
				this.ID = IControllerProfileInfoCollectionContainerExtensions.CalculateID(this);
				this.SaveCommand.RaiseCanExecuteChanged();
				return true;
			}
			return false;
		}

		public static bool SendHiddenCompositeDevicesInfo(List<BaseControllerVM> controllers, ref int subDevNumb, List<Tuple<ulong, uint>> duplicateGamepadCollection)
		{
			List<KeyValuePair<string, bool>> list = new List<KeyValuePair<string, bool>>
			{
				new KeyValuePair<string, bool>("Controller", false),
				new KeyValuePair<string, bool>("Keyboard", false),
				new KeyValuePair<string, bool>("Mouse", false)
			};
			subDevNumb = 0;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			using (List<BaseControllerVM>.Enumerator enumerator = controllers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BaseControllerVM controller = enumerator.Current;
					if (controller != null && controller.ControllerFamily != 4)
					{
						if (!controller.Ids.All((ulong x) => x == 0UL))
						{
							string text = " - Composite";
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(62, 1);
							defaultInterpolatedStringHandler.AppendLiteral("SendHiddenCompositeDevicesInfo: ConsistsOfControllersNumber = ");
							defaultInterpolatedStringHandler.AppendFormatted<int>(controller.ConsistsOfControllersNumber);
							Tracer.TraceWriteTag(text, defaultInterpolatedStringHandler.ToStringAndClear(), false);
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(79, 1);
							defaultInterpolatedStringHandler.AppendLiteral("CompositeDevice: SendHiddenCompositeDevicesInfo: ConsistsOfControllersNumber = ");
							defaultInterpolatedStringHandler.AppendFormatted<int>(controller.ConsistsOfControllersNumber);
							Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
							Tracer.TraceWriteTag(" - Composite", "SendHiddenCompositeDevicesInfo: ID = " + controller.ID, false);
							Tracer.TraceWrite("CompositeDevice: SendHiddenCompositeDevicesInfo: ID = " + controller.ID, false);
							subDevNumb += controller.ConsistsOfControllersNumber;
							if (controller.ControllerFamily == null)
							{
								list[0] = new KeyValuePair<string, bool>("Controller", true);
								if (duplicateGamepadCollection != null && controller.Ids[0] != 0UL)
								{
									int num = duplicateGamepadCollection.Count((Tuple<ulong, uint> x) => x.Item1 == controller.Ids[0]);
									if (num > 0)
									{
										subDevNumb += num - 1;
										string text2 = " - Composite";
										defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 1);
										defaultInterpolatedStringHandler.AppendLiteral("SendHiddenCompositeDevicesInfo: subDevNumb += ");
										defaultInterpolatedStringHandler.AppendFormatted<int>(num - 1);
										Tracer.TraceWriteTag(text2, defaultInterpolatedStringHandler.ToStringAndClear(), false);
										defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(63, 1);
										defaultInterpolatedStringHandler.AppendLiteral("CompositeDevice: SendHiddenCompositeDevicesInfo: subDevNumb += ");
										defaultInterpolatedStringHandler.AppendFormatted<int>(num - 1);
										Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
									}
								}
							}
							else if (controller.ControllerFamily == 1)
							{
								list[1] = new KeyValuePair<string, bool>("Keyboard", true);
							}
							else if (controller.ControllerFamily == 2)
							{
								list[2] = new KeyValuePair<string, bool>("Mouse", true);
							}
						}
					}
				}
			}
			if (subDevNumb > 15)
			{
				SenderGoogleAnalytics.SendMessageEvent("Composite", "Error", subDevNumb.ToString(), -1L, false);
				return false;
			}
			string text3 = "";
			int num2 = 0;
			foreach (KeyValuePair<string, bool> keyValuePair in list)
			{
				if (keyValuePair.Value)
				{
					num2++;
					if (!string.IsNullOrEmpty(text3))
					{
						text3 += " + ";
					}
					text3 += keyValuePair.Key;
				}
			}
			if (num2 <= 1)
			{
				text3 += " only";
			}
			if (!RegistryHelper.ValueExists("Analytics\\Composite", text3 + " " + subDevNumb.ToString()))
			{
				RegistryHelper.SetString("Analytics\\Composite", text3 + " " + subDevNumb.ToString(), "");
				SenderGoogleAnalytics.SendMessageEvent("Composite", text3, subDevNumb.ToString(), -1L, false);
			}
			string text4 = " - Composite";
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 1);
			defaultInterpolatedStringHandler.AppendLiteral("SendHiddenCompositeDevicesInfo: Total subDevNumb = ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(subDevNumb);
			Tracer.TraceWriteTag(text4, defaultInterpolatedStringHandler.ToStringAndClear(), false);
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(68, 1);
			defaultInterpolatedStringHandler.AppendLiteral("CompositeDevice: SendHiddenCompositeDevicesInfo: Total subDevNumb = ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(subDevNumb);
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			return true;
		}

		protected CompositeDevice(SerializationInfo info, StreamingContext context)
			: this()
		{
			foreach (SerializationEntry serializationEntry in info)
			{
				string name = serializationEntry.Name;
				if (!(name == "ControllerProfileInfoCollections"))
				{
					if (name == "AdditionalParameters")
					{
						this.AdditionalParameters = (Dictionary<string, string>)info.GetValue("AdditionalParameters", typeof(Dictionary<string, string>));
					}
				}
				else
				{
					this.ControllerProfileInfoCollections = (ControllerProfileInfoCollection[])info.GetValue("ControllerProfileInfoCollections", typeof(ControllerProfileInfoCollection[]));
				}
			}
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ControllerProfileInfoCollections", this.ControllerProfileInfoCollections);
			info.AddValue("AdditionalParameters", this.AdditionalParameters);
		}

		private string _id;

		private bool _isEditMode;

		private BaseControllerVM _controller1;

		private BaseControllerVM _controller2;

		private BaseControllerVM _controller3;

		private BaseControllerVM _controller4;

		public readonly Dictionary<string, string> AdditionalParameters = new Dictionary<string, string>();

		private DelegateCommand _edit;

		private DelegateCommand _remove;

		private DelegateCommand _save;
	}
}
