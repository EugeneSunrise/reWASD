using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings.XB;
using XBEliteWPF.License.Licensing.ComStructures;

namespace reWASDUI.Controls.XBBindingControls
{
	public partial class XBBindingTooltip : ToolTip, INotifyPropertyChanged
	{
		public XBBinding XBBinding
		{
			get
			{
				return (XBBinding)base.GetValue(XBBindingTooltip.XBBindingProperty);
			}
			set
			{
				base.SetValue(XBBindingTooltip.XBBindingProperty, value);
			}
		}

		public bool IsAdvancedMappingFeatureNotRequired
		{
			get
			{
				BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
				bool flag;
				if (currentGamepad == null)
				{
					flag = false;
				}
				else
				{
					ControllerVM currentController = currentGamepad.CurrentController;
					ControllerTypeEnum? controllerTypeEnum = ((currentController != null) ? new ControllerTypeEnum?(currentController.ControllerType) : null);
					ControllerTypeEnum controllerTypeEnum2 = 16;
					flag = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
				}
				if (!flag)
				{
					BaseControllerVM currentGamepad2 = App.GamepadService.CurrentGamepad;
					bool flag2;
					if (currentGamepad2 == null)
					{
						flag2 = false;
					}
					else
					{
						ControllerVM currentController2 = currentGamepad2.CurrentController;
						ControllerTypeEnum? controllerTypeEnum = ((currentController2 != null) ? new ControllerTypeEnum?(currentController2.ControllerType) : null);
						ControllerTypeEnum controllerTypeEnum2 = 63;
						flag2 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
					}
					if (!flag2)
					{
						BaseControllerVM currentGamepad3 = App.GamepadService.CurrentGamepad;
						bool flag3;
						if (currentGamepad3 == null)
						{
							flag3 = false;
						}
						else
						{
							ControllerVM currentController3 = currentGamepad3.CurrentController;
							ControllerTypeEnum? controllerTypeEnum = ((currentController3 != null) ? new ControllerTypeEnum?(currentController3.ControllerType) : null);
							ControllerTypeEnum controllerTypeEnum2 = 2;
							flag3 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
						}
						if (!flag3)
						{
							BaseControllerVM currentGamepad4 = App.GamepadService.CurrentGamepad;
							bool flag4;
							if (currentGamepad4 == null)
							{
								flag4 = false;
							}
							else
							{
								ControllerVM currentController4 = currentGamepad4.CurrentController;
								ControllerTypeEnum? controllerTypeEnum = ((currentController4 != null) ? new ControllerTypeEnum?(currentController4.ControllerType) : null);
								ControllerTypeEnum controllerTypeEnum2 = 22;
								flag4 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
							}
							if (!flag4)
							{
								BaseControllerVM currentGamepad5 = App.GamepadService.CurrentGamepad;
								bool flag5;
								if (currentGamepad5 == null)
								{
									flag5 = false;
								}
								else
								{
									ControllerVM currentController5 = currentGamepad5.CurrentController;
									ControllerTypeEnum? controllerTypeEnum = ((currentController5 != null) ? new ControllerTypeEnum?(currentController5.ControllerType) : null);
									ControllerTypeEnum controllerTypeEnum2 = 3;
									flag5 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
								}
								if (!flag5)
								{
									BaseControllerVM currentGamepad6 = App.GamepadService.CurrentGamepad;
									if (currentGamepad6 == null)
									{
										return false;
									}
									ControllerVM currentController6 = currentGamepad6.CurrentController;
									ControllerTypeEnum? controllerTypeEnum = ((currentController6 != null) ? new ControllerTypeEnum?(currentController6.ControllerType) : null);
									ControllerTypeEnum controllerTypeEnum2 = 12;
									return (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
								}
							}
						}
					}
				}
				return true;
			}
		}

		public bool IsAdvancedMappingFeatureNotRequiredForUnmap
		{
			get
			{
				BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
				bool flag;
				if (currentGamepad == null)
				{
					flag = false;
				}
				else
				{
					ControllerVM currentController = currentGamepad.CurrentController;
					ControllerTypeEnum? controllerTypeEnum = ((currentController != null) ? new ControllerTypeEnum?(currentController.ControllerType) : null);
					ControllerTypeEnum controllerTypeEnum2 = 16;
					flag = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
				}
				if (!flag)
				{
					BaseControllerVM currentGamepad2 = App.GamepadService.CurrentGamepad;
					bool flag2;
					if (currentGamepad2 == null)
					{
						flag2 = false;
					}
					else
					{
						ControllerVM currentController2 = currentGamepad2.CurrentController;
						ControllerTypeEnum? controllerTypeEnum = ((currentController2 != null) ? new ControllerTypeEnum?(currentController2.ControllerType) : null);
						ControllerTypeEnum controllerTypeEnum2 = 63;
						flag2 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
					}
					if (!flag2)
					{
						BaseControllerVM currentGamepad3 = App.GamepadService.CurrentGamepad;
						bool flag3;
						if (currentGamepad3 == null)
						{
							flag3 = false;
						}
						else
						{
							ControllerVM currentController3 = currentGamepad3.CurrentController;
							ControllerTypeEnum? controllerTypeEnum = ((currentController3 != null) ? new ControllerTypeEnum?(currentController3.ControllerType) : null);
							ControllerTypeEnum controllerTypeEnum2 = 19;
							flag3 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
						}
						if (!flag3)
						{
							BaseControllerVM currentGamepad4 = App.GamepadService.CurrentGamepad;
							bool flag4;
							if (currentGamepad4 == null)
							{
								flag4 = false;
							}
							else
							{
								ControllerVM currentController4 = currentGamepad4.CurrentController;
								ControllerTypeEnum? controllerTypeEnum = ((currentController4 != null) ? new ControllerTypeEnum?(currentController4.ControllerType) : null);
								ControllerTypeEnum controllerTypeEnum2 = 28;
								flag4 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
							}
							if (!flag4)
							{
								BaseControllerVM currentGamepad5 = App.GamepadService.CurrentGamepad;
								bool flag5;
								if (currentGamepad5 == null)
								{
									flag5 = false;
								}
								else
								{
									ControllerVM currentController5 = currentGamepad5.CurrentController;
									ControllerTypeEnum? controllerTypeEnum = ((currentController5 != null) ? new ControllerTypeEnum?(currentController5.ControllerType) : null);
									ControllerTypeEnum controllerTypeEnum2 = 54;
									flag5 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
								}
								if (!flag5)
								{
									BaseControllerVM currentGamepad6 = App.GamepadService.CurrentGamepad;
									bool flag6;
									if (currentGamepad6 == null)
									{
										flag6 = false;
									}
									else
									{
										ControllerVM currentController6 = currentGamepad6.CurrentController;
										ControllerTypeEnum? controllerTypeEnum = ((currentController6 != null) ? new ControllerTypeEnum?(currentController6.ControllerType) : null);
										ControllerTypeEnum controllerTypeEnum2 = 37;
										flag6 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
									}
									if (!flag6)
									{
										BaseControllerVM currentGamepad7 = App.GamepadService.CurrentGamepad;
										bool flag7;
										if (currentGamepad7 == null)
										{
											flag7 = false;
										}
										else
										{
											ControllerVM currentController7 = currentGamepad7.CurrentController;
											ControllerTypeEnum? controllerTypeEnum = ((currentController7 != null) ? new ControllerTypeEnum?(currentController7.ControllerType) : null);
											ControllerTypeEnum controllerTypeEnum2 = 38;
											flag7 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
										}
										if (!flag7)
										{
											BaseControllerVM currentGamepad8 = App.GamepadService.CurrentGamepad;
											bool flag8;
											if (currentGamepad8 == null)
											{
												flag8 = false;
											}
											else
											{
												ControllerVM currentController8 = currentGamepad8.CurrentController;
												ControllerTypeEnum? controllerTypeEnum = ((currentController8 != null) ? new ControllerTypeEnum?(currentController8.ControllerType) : null);
												ControllerTypeEnum controllerTypeEnum2 = 39;
												flag8 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
											}
											if (!flag8)
											{
												BaseControllerVM currentGamepad9 = App.GamepadService.CurrentGamepad;
												bool flag9;
												if (currentGamepad9 == null)
												{
													flag9 = false;
												}
												else
												{
													ControllerVM currentController9 = currentGamepad9.CurrentController;
													ControllerTypeEnum? controllerTypeEnum = ((currentController9 != null) ? new ControllerTypeEnum?(currentController9.ControllerType) : null);
													ControllerTypeEnum controllerTypeEnum2 = 40;
													flag9 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
												}
												if (!flag9)
												{
													BaseControllerVM currentGamepad10 = App.GamepadService.CurrentGamepad;
													if (currentGamepad10 == null)
													{
														return false;
													}
													ControllerVM currentController10 = currentGamepad10.CurrentController;
													ControllerTypeEnum? controllerTypeEnum = ((currentController10 != null) ? new ControllerTypeEnum?(currentController10.ControllerType) : null);
													ControllerTypeEnum controllerTypeEnum2 = 62;
													return (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
												}
											}
										}
									}
								}
							}
						}
					}
				}
				return true;
			}
		}

		public bool IsXbBindingHint
		{
			get
			{
				if (this.XBBinding != null)
				{
					if ((!App.LicensingService.IsAdvancedMappingFeatureUnlocked && (!App.GameProfilesService.CurrentGame.CurrentConfig.CurrentSubConfigData.IsMouse || this.XBBinding.IsMouseDirection) && App.GameProfilesService.CurrentGame.CurrentConfig.CurrentSubConfigData.IsMouse) || !App.GameProfilesService.CurrentGame.CurrentConfig.IsEditConfigMode)
					{
						return false;
					}
					if (this.XBBinding.IsRemapedOrUnmapped && !App.LicensingService.IsAdvancedMappingFeatureUnlocked && (!this.IsAdvancedMappingFeatureNotRequired || !this.XBBinding.IsStickDirection || this.XBBinding.IsUnmapped) && (!this.XBBinding.IsUnmapped || App.GameProfilesService.CurrentGame.CurrentConfig.CurrentSubConfigData.IsGamepad || this.XBBinding.IsMouseStick) && (!this.IsAdvancedMappingFeatureNotRequired || this.XBBinding.IsStickDirection || this.XBBinding.IsMouseStick) && (!this.IsAdvancedMappingFeatureNotRequiredForUnmap || !this.XBBinding.IsUnmapped || App.GameProfilesService.CurrentGame.CurrentConfig.CurrentSubConfigData.IsMouse) && (!this.IsAdvancedMappingFeatureNotRequiredForUnmap || !App.GameProfilesService.CurrentGame.CurrentConfig.CurrentSubConfigData.IsMouse || this.XBBinding.IsMouseStick || !App.GameProfilesService.CurrentGame.CurrentConfig.IsEditConfigMode))
					{
						return false;
					}
				}
				return true;
			}
		}

		public XBBindingTooltip()
		{
			this.InitializeComponent();
			base.Loaded += this.XBBindingTooltip_Loaded;
		}

		private void XBBindingTooltip_Loaded(object sender, RoutedEventArgs e)
		{
			App.EventAggregator.GetEvent<CurrentButtonMappingChanged>().Subscribe(delegate(object x)
			{
				this.OnPropertyChanged("IsAdvancedMappingFeatureNotRequired");
				this.OnPropertyChanged("IsAdvancedMappingFeatureNotRequiredForUnmap");
			});
			App.LicensingService.OnLicenseChangedCompleted += delegate(LicenseCheckResult s, bool ex)
			{
				this.OnPropertyChanged("IsXbBindingHint");
			};
			this.OnPropertyChanged("IsXbBindingHint");
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
			}
		}

		public static readonly DependencyProperty XBBindingProperty = DependencyProperty.Register("XBBinding", typeof(XBBinding), typeof(XBBindingTooltip), new PropertyMetadata(null));
	}
}
