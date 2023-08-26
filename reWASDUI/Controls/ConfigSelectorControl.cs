using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.Controls;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Prism.Commands;
using Prism.Ioc;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure;
using reWASDUI.Services;
using reWASDUI.Utils;
using reWASDUI.ViewModels;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDUI.Controls
{
	public class ConfigSelectorControl : UserControlWithDropDown, INotifyPropertyChanged, IComponentConnector, IStyleConnector
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
			}
		}

		public bool IsInEditMode
		{
			get
			{
				return this._isInEditMode;
			}
			set
			{
				if (this._isInEditMode != value)
				{
					this._isInEditMode = value;
					this.OnPropertyChanged("IsInEditMode");
					if (!this._isInEditMode)
					{
						GameVM currentGame = App.GameProfilesService.CurrentGame;
						string text;
						if (currentGame == null)
						{
							text = null;
						}
						else
						{
							ConfigVM currentConfig = currentGame.CurrentConfig;
							text = ((currentConfig != null) ? currentConfig.Name : null);
						}
						this.NotNullConfigName = text;
					}
				}
			}
		}

		public string NotNullConfigName
		{
			get
			{
				return this._notNullConfigName;
			}
			set
			{
				if (value != null && this._notNullConfigName != value)
				{
					this._notNullConfigName = value;
					this.OnPropertyChanged("NotNullConfigName");
				}
			}
		}

		public DelegateCommand RenameConfigCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._renameConfig) == null)
				{
					delegateCommand = (this._renameConfig = new DelegateCommand(delegate
					{
						this.IsInEditMode = true;
					}));
				}
				return delegateCommand;
			}
		}

		public ConfigSelectorControl()
		{
			this.InitializeComponent();
			base.DataContext = IContainerProviderExtensions.Resolve<GameConfigSelectorVM>(App.Container);
			App.EventAggregator.GetEvent<CurrentConfigChanged>().Subscribe(delegate(ConfigVM shift)
			{
				this.NotNullConfigName = ((shift != null) ? shift.Name : null);
				base.IsDropDownOpen = false;
			});
			App.GameProfilesService.CurrentGameChanged += delegate(object sender, PropertyChangedExtendedEventArgs<GameVM> args)
			{
				GameVM currentGame2 = App.GameProfilesService.CurrentGame;
				string text2;
				if (currentGame2 == null)
				{
					text2 = null;
				}
				else
				{
					ConfigVM currentConfig2 = currentGame2.CurrentConfig;
					text2 = ((currentConfig2 != null) ? currentConfig2.Name : null);
				}
				this.NotNullConfigName = text2;
			};
			GameVM currentGame = App.GameProfilesService.CurrentGame;
			string text;
			if (currentGame == null)
			{
				text = null;
			}
			else
			{
				ConfigVM currentConfig = currentGame.CurrentConfig;
				text = ((currentConfig != null) ? currentConfig.Name : null);
			}
			this.NotNullConfigName = text;
			App.EventAggregator.GetEvent<ConfigCreatedByUI>().Subscribe(delegate(ConfigVM config)
			{
				GameVM currentGame3 = App.GameProfilesService.CurrentGame;
				ConfigVM configVM = ((currentGame3 != null) ? currentGame3.CurrentConfig : null);
				if (config == configVM)
				{
					this.IsInEditMode = true;
				}
			});
		}

		private void BtnShowShiftsList_OnClick(object sender, RoutedEventArgs e)
		{
			base.SetCurrentValue(UserControlWithDropDown.IsDropDownOpenProperty, !base.IsDropDownOpen);
		}

		private void ConfigsList_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Right)
			{
				e.Handled = true;
				return;
			}
			try
			{
				ListBoxItem listBoxItem = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
				if (listBoxItem != null && listBoxItem.DataContext != null)
				{
					base.IsDropDownOpen = false;
					(base.DataContext as GameConfigSelectorVM).CurrentConfig = listBoxItem.DataContext as ConfigVM;
				}
			}
			catch (Exception)
			{
			}
		}

		private void btnShowConfigsList_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				base.SetCurrentValue(UserControlWithDropDown.IsDropDownOpenProperty, !base.IsDropDownOpen);
			}
		}

		private void btnShowConfigsList_MouseEnter(object sender, MouseEventArgs e)
		{
			Popup popup = base.Template.FindName("shiftsPopup", this) as Popup;
			if (popup != null)
			{
				popup.StaysOpen = true;
			}
		}

		private void btnShowConfigsList_MouseLeave(object sender, MouseEventArgs e)
		{
			Popup popup = base.Template.FindName("shiftsPopup", this) as Popup;
			if (popup != null)
			{
				popup.StaysOpen = false;
			}
		}

		private async Task RenameConfig(ConfigVM config, string oldName, string newName)
		{
			string directoryName = Path.GetDirectoryName(config.ConfigPath);
			newName = XBUtils.NormalizeToMaxPathTrimFilename(directoryName, newName.Trim(), ".rewasd.bak", null);
			if (oldName != newName && !string.IsNullOrEmpty(newName))
			{
				if (!XBValidators.ValidateConfigName(newName))
				{
					config.Name = oldName;
					config.EditedName = oldName;
				}
				else
				{
					if (File.Exists(directoryName + "\\" + newName.ToLower() + ".rewasd") || File.Exists(directoryName + "\\" + newName + ".rewasd"))
					{
						int num = 1;
						string text = null;
						string text2 = newName;
						while (File.Exists(directoryName + "\\" + XBUtils.NormalizeToMaxPathTrimFilename(directoryName + "\\", text2.Trim().ToLower(), ".rewasd", text) + ".rewasd") || File.Exists(directoryName + "\\" + XBUtils.NormalizeToMaxPathTrimFilename(directoryName + "\\", text2.Trim(), ".rewasd", text) + ".rewasd"))
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 1);
							defaultInterpolatedStringHandler.AppendLiteral(" (");
							defaultInterpolatedStringHandler.AppendFormatted<int>(num);
							defaultInterpolatedStringHandler.AppendLiteral(")");
							text = defaultInterpolatedStringHandler.ToStringAndClear();
							text2 = newName + text;
							num++;
						}
						string text3 = newName;
						if (newName.Length >= 10)
						{
							text3 = newName.Substring(0, 8);
							text3 += "..";
						}
						if (DTMessageBox.Show(string.Format(DTLocalization.GetString(12650), text3 + text), DTLocalization.GetString(5010), MessageBoxButton.YesNo, MessageBoxImage.Exclamation) != MessageBoxResult.Yes)
						{
							return;
						}
						if (!string.IsNullOrEmpty(text2))
						{
							newName = text2;
						}
					}
					if (App.GameProfilesService.CurrentGame.ConfigCollection.FirstOrDefault((ConfigVM c) => c.Name == newName) != null)
					{
						DTMessageBox.Show(DTLocalization.GetString(11093), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
						config.Name = oldName;
						config.EditedName = oldName;
					}
					else
					{
						config.Name = newName;
						string oldConfigPath = config.ConfigPath;
						config.ConfigPath = directoryName + "\\" + XBUtils.NormalizeToMaxPathTrimFilename(directoryName + "\\", config.Name.Trim(), ".rewasd", null) + ".rewasd";
						ResponseWithError responseWithError = await App.HttpClientService.GameProfiles.RenameConfig(new RenameConfigParams
						{
							ClientID = SSEClient.ClientID,
							GameName = config.GameName,
							Name = oldName,
							NewName = newName,
							NewPath = config.ConfigPath
						});
						if (!responseWithError.Result && !string.IsNullOrEmpty(responseWithError.ErrorText))
						{
							DTMessageBox.Show(responseWithError.ErrorText, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
							config.Name = oldName;
							config.EditedName = oldName;
							config.ConfigPath = oldConfigPath;
						}
						else
						{
							oldConfigPath = null;
						}
					}
				}
			}
		}

		private async void UIElement_OnKeyDown(object sender, KeyEventArgs e)
		{
			ConfigVM config = App.GameProfilesService.CurrentGame.ConfigCollection.FirstOrDefault((ConfigVM item) => item.IsInEditMode);
			string newName = "";
			if (config != null)
			{
				newName = config.EditedName;
			}
			if (this.IsInEditMode)
			{
				GameVM currentGame = App.GameProfilesService.CurrentGame;
				config = ((currentGame != null) ? currentGame.CurrentConfig : null);
				newName = this.NotNullConfigName;
			}
			newName.Trim();
			if (e.Key == Key.Return && config != null && !string.IsNullOrEmpty(newName))
			{
				await this.RenameConfig(config, config.Name, newName);
				GameVM currentGame2 = App.GameProfilesService.CurrentGame;
				string text;
				if (currentGame2 == null)
				{
					text = null;
				}
				else
				{
					ConfigVM currentConfig = currentGame2.CurrentConfig;
					text = ((currentConfig != null) ? currentConfig.Name : null);
				}
				this.NotNullConfigName = text;
			}
			if ((e.Key == Key.Escape || (e.Key == Key.Return && string.IsNullOrEmpty(newName))) && config != null)
			{
				config.EditedName = config.Name;
				GameVM currentGame3 = App.GameProfilesService.CurrentGame;
				string text2;
				if (currentGame3 == null)
				{
					text2 = null;
				}
				else
				{
					ConfigVM currentConfig2 = currentGame3.CurrentConfig;
					text2 = ((currentConfig2 != null) ? currentConfig2.Name : null);
				}
				this.NotNullConfigName = text2;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/reWASD;component/controls/dropdowncontrols/configselectorcontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			this._contentLoaded = true;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				((SVGButton)target).Click += this.BtnShowShiftsList_OnClick;
				((SVGButton)target).MouseEnter += this.btnShowConfigsList_MouseEnter;
				((SVGButton)target).MouseLeave += this.btnShowConfigsList_MouseLeave;
				return;
			case 2:
				((EditableTextBlock)target).PreviewKeyDown += this.UIElement_OnKeyDown;
				return;
			case 3:
				((ListBox)target).PreviewKeyDown += this.UIElement_OnKeyDown;
				((ListBox)target).PreviewMouseDown += this.ConfigsList_OnPreviewMouseDown;
				return;
			default:
				return;
			}
		}

		private bool _isInEditMode;

		private string _notNullConfigName;

		private DelegateCommand _renameConfig;

		private bool _contentLoaded;
	}
}
