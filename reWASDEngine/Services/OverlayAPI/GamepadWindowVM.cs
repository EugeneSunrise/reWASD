using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtil;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDEngine.Services.OverlayAPI
{
	public class GamepadWindowVM : ZBindableBase
	{
		public float Transparent
		{
			get
			{
				return this._transparent;
			}
			set
			{
				this.SetProperty<float>(ref this._transparent, value, "Transparent");
			}
		}

		public ObservableCollection<MessageInfo> Messages
		{
			get
			{
				ObservableCollection<MessageInfo> observableCollection;
				if ((observableCollection = this._messages) == null)
				{
					observableCollection = (this._messages = new ObservableCollection<MessageInfo>());
				}
				return observableCollection;
			}
		}

		public ObservableCollection<LeftButtonsInfo> LeftButtons
		{
			get
			{
				ObservableCollection<LeftButtonsInfo> observableCollection;
				if ((observableCollection = this._leftButtons) == null)
				{
					observableCollection = (this._leftButtons = new ObservableCollection<LeftButtonsInfo>());
				}
				return observableCollection;
			}
		}

		public ObservableCollection<RightButtonsInfo> RightButtons
		{
			get
			{
				ObservableCollection<RightButtonsInfo> observableCollection;
				if ((observableCollection = this._rightButtons) == null)
				{
					observableCollection = (this._rightButtons = new ObservableCollection<RightButtonsInfo>());
				}
				return observableCollection;
			}
		}

		public GamepadWindowVM()
		{
			this.gyroYaw = 0.5;
			this.gyroPitch = 0.5;
			this.gyroRoll = 0.5;
		}

		public string LeftXValeString
		{
			get
			{
				return this.LeftXValue.ToString("n5");
			}
		}

		public string LeftYValeString
		{
			get
			{
				return this.LeftYValue.ToString("n5");
			}
		}

		public float LeftXValue
		{
			get
			{
				return this._leftXValue;
			}
			set
			{
				if (this._leftXValue == value)
				{
					return;
				}
				this.SetProperty<float>(ref this._leftXValue, value, "LeftXValue");
				this.OnPropertyChanged("LeftXValeString");
			}
		}

		public HorizontalAlignment AlignmentSettings
		{
			get
			{
				return this._alignment;
			}
			set
			{
				this._alignment = value;
				this.OnPropertyChanged("Alignment");
				this.OnPropertyChanged("Margin");
			}
		}

		public Thickness Margin
		{
			get
			{
				if (RegistryHelper.GetValue("Overlay", "ShowControllerOnly", 0, false) == 0)
				{
					return new Thickness(0.0, 0.0, 0.0, 0.0);
				}
				if (this.AlignmentSettings != HorizontalAlignment.Left)
				{
					return new Thickness(0.0, 0.0, 49.0, 0.0);
				}
				return new Thickness(49.0, 0.0, 0.0, 0.0);
			}
		}

		public HotkeysInfo HotKeyButtons
		{
			get
			{
				return this._gamepadHotkeysInfo;
			}
			set
			{
				this.SetProperty<HotkeysInfo>(ref this._gamepadHotkeysInfo, value, "HotKeyButtons");
			}
		}

		public bool NewLine
		{
			get
			{
				return this.HotKeyButtons.IsOnlyOneGroup();
			}
		}

		public void FillEnd()
		{
			this.OnPropertyChanged("NewLine");
		}

		public string ToCloseString
		{
			get
			{
				return DTLocalization.GetString(12164);
			}
			set
			{
			}
		}

		public double gyroYaw
		{
			get
			{
				return this._gyroX;
			}
			set
			{
				this.SetProperty<double>(ref this._gyroX, value, "gyroYaw");
			}
		}

		public double gyroPitch
		{
			get
			{
				return this._gyroY;
			}
			set
			{
				this.SetProperty<double>(ref this._gyroY, value, "gyroPitch");
			}
		}

		public double gyroRoll
		{
			get
			{
				return this._gyroZ;
			}
			set
			{
				this.SetProperty<double>(ref this._gyroZ, value, "gyroRoll");
			}
		}

		public float LeftYValue
		{
			get
			{
				return this._leftYValue;
			}
			set
			{
				if (this._leftYValue == value)
				{
					return;
				}
				this.SetProperty<float>(ref this._leftYValue, value, "LeftYValue");
				this.OnPropertyChanged("LeftYValeString");
			}
		}

		public string RightXValeString
		{
			get
			{
				return this.RightXValue.ToString("n5");
			}
		}

		public string RightYValeString
		{
			get
			{
				return this.RightYValue.ToString("n5");
			}
		}

		public float RightXValue
		{
			get
			{
				return this._rightXValue;
			}
			set
			{
				if (this._rightXValue == value)
				{
					return;
				}
				this.SetProperty<float>(ref this._rightXValue, value, "RightXValue");
				this.OnPropertyChanged("RightXValeString");
			}
		}

		public float RightYValue
		{
			get
			{
				return this._rightYValue;
			}
			set
			{
				if (this._rightYValue == value)
				{
					return;
				}
				this.SetProperty<float>(ref this._rightYValue, value, "RightYValue");
				this.OnPropertyChanged("RightYValeString");
			}
		}

		public double Scale
		{
			get
			{
				return Engine.UserSettingsService.GamepadWidowScale;
			}
		}

		public bool IsGamepadVisible
		{
			get
			{
				return this._isGamepadVisible;
			}
			set
			{
				if (this._isGamepadVisible == value)
				{
					return;
				}
				this.SetProperty<bool>(ref this._isGamepadVisible, value, "IsGamepadVisible");
			}
		}

		public bool IsGyroVisible
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum = this.ControllerType;
				ControllerTypeEnum controllerTypeEnum2 = 4;
				if (!((controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null)))
				{
					controllerTypeEnum = this.ControllerType;
					controllerTypeEnum2 = 14;
					if (!((controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null)))
					{
						controllerTypeEnum = this.ControllerType;
						controllerTypeEnum2 = 7;
						if (!((controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null)))
						{
							controllerTypeEnum = this.ControllerType;
							controllerTypeEnum2 = 8;
							if (!((controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null)))
							{
								controllerTypeEnum = this.ControllerType;
								controllerTypeEnum2 = 9;
								if (!((controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null)))
								{
									controllerTypeEnum = this.ControllerType;
									controllerTypeEnum2 = 10;
									if (!((controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null)))
									{
										return false;
									}
								}
							}
						}
					}
				}
				return true;
			}
		}

		public bool IsTableVisible
		{
			get
			{
				return this._isTableVisible;
			}
			set
			{
				if (this._isTableVisible == value)
				{
					return;
				}
				this.SetProperty<bool>(ref this._isTableVisible, value, "IsTableVisible");
			}
		}

		public Drawing GamepadDrawing
		{
			get
			{
				return this._gamepadDrawing;
			}
			set
			{
				this.SetProperty<Drawing>(ref this._gamepadDrawing, value, "GamepadDrawing");
			}
		}

		public Drawing LeftStickDrawing
		{
			get
			{
				return this._leftStickDrawing;
			}
			set
			{
				this.SetProperty<Drawing>(ref this._leftStickDrawing, value, "LeftStickDrawing");
			}
		}

		public Drawing RightStickDrawing
		{
			get
			{
				return this._rightStickDrawing;
			}
			set
			{
				this.SetProperty<Drawing>(ref this._rightStickDrawing, value, "RightStickDrawing");
			}
		}

		public int Count()
		{
			return this.Messages.Count<MessageInfo>();
		}

		public void OverlayKeyPressed(GamepadState gamepadState, ControllerTypeEnum controllerType, float deltaTime)
		{
			this.IsGamepadVisible = true;
			this.ShowTable(gamepadState, deltaTime);
			this.ShowKeysButtons(gamepadState);
		}

		private void ShowTable(GamepadState gamepadState, float deltaTime)
		{
			this.LeftXValue = (float)gamepadState.LeftStickX / 32767f;
			this.LeftYValue = (float)gamepadState.LeftStickY / 32767f;
			double num = 2147483648.0;
			double num2 = ((deltaTime < 100f) ? (30.0 / (double)deltaTime) : (100.0 / (double)deltaTime));
			double num3 = 1.399999976158142;
			if (this.GyroXDeltaOld != 0.0)
			{
				this.gyroPitch = 0.5 + ((double)gamepadState.GyroXDelta - this.GyroXDeltaOld) * num2 * num3 / num;
				this.gyroRoll = 0.5 + ((double)gamepadState.GyroYDelta - this.GyroYDeltaOld) * num2 * num3 / num;
				this.gyroYaw = 0.5 + ((double)gamepadState.GyroZDelta - this.GyroZDeltaOld) * num2 * num3 / num;
			}
			this.GyroXDeltaOld = (double)gamepadState.GyroXDelta;
			this.GyroYDeltaOld = (double)gamepadState.GyroYDelta;
			this.GyroZDeltaOld = (double)gamepadState.GyroZDelta;
			this.RightXValue = (float)gamepadState.RightStickX / 32767f;
			this.RightYValue = (float)gamepadState.RightStickY / 32767f;
			foreach (RightButtonsInfo rightButtonsInfo in this.RightButtons)
			{
				rightButtonsInfo.Value = 0f;
			}
			foreach (LeftButtonsInfo leftButtonsInfo in this.LeftButtons)
			{
				leftButtonsInfo.Value = 0f;
			}
			foreach (GamepadButtonDescription gamepadButtonDescription in gamepadState.PressedButtons)
			{
				foreach (LeftButtonsInfo leftButtonsInfo2 in this.LeftButtons)
				{
					if (leftButtonsInfo2.btn == gamepadButtonDescription.Button)
					{
						leftButtonsInfo2.Value = this.GetValue(leftButtonsInfo2.btn, gamepadState);
					}
				}
			}
			this.CheckAndSetBtn(gamepadState, 33);
			this.CheckAndSetBtn(gamepadState, 34);
			this.CheckAndSetBtn(gamepadState, 35);
			this.CheckAndSetBtn(gamepadState, 36);
			this.CheckAndSetBtn(gamepadState, 99);
			this.SetRightBtnValue(51, (float)gamepadState.LeftTrigger / 32767f);
			this.SetRightBtnValue(55, (float)gamepadState.RightTrigger / 32767f);
		}

		private void CheckAndSetBtn(GamepadState gamepadState, GamepadButton btn)
		{
			bool flag = false;
			foreach (GamepadButtonDescription gamepadButtonDescription in gamepadState.PressedButtons)
			{
				if (btn == gamepadButtonDescription.Button)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				this.SetRightBtnValue(btn, this.GetValue(btn, gamepadState));
			}
		}

		private float GetValue(GamepadButton btn, GamepadState gamepadState)
		{
			float num = 1f;
			if (this.ControllerType != null && ControllerTypeExtensions.IsSonyDS3(this.ControllerType.GetValueOrDefault()))
			{
				if (btn == 1)
				{
					num = ((gamepadState.DS3PressureCross != 0) ? ((float)gamepadState.DS3PressureCross / 255f) : 1f);
				}
				else if (btn == 2)
				{
					num = ((gamepadState.DS3PressureCircle != 0) ? ((float)gamepadState.DS3PressureCircle / 255f) : 1f);
				}
				else if (btn == 3)
				{
					num = ((gamepadState.DS3PressureSquare != 0) ? ((float)gamepadState.DS3PressureSquare / 255f) : 1f);
				}
				else if (btn == 4)
				{
					num = ((gamepadState.DS3PressureTriangle != 0) ? ((float)gamepadState.DS3PressureTriangle / 255f) : 1f);
				}
				else if (btn == 5)
				{
					num = ((gamepadState.DS3PressureL1 != 0) ? ((float)gamepadState.DS3PressureL1 / 255f) : 1f);
				}
				else if (btn == 6)
				{
					num = ((gamepadState.DS3PressureR1 != 0) ? ((float)gamepadState.DS3PressureR1 / 255f) : 1f);
				}
				else if (btn == 33)
				{
					num = ((gamepadState.DS3PressureDpadUp != 0) ? ((float)gamepadState.DS3PressureDpadUp / 255f) : 1f);
				}
				else if (btn == 34)
				{
					num = ((gamepadState.DS3PressureDpadDown != 0) ? ((float)gamepadState.DS3PressureDpadDown / 255f) : 1f);
				}
				else if (btn == 36)
				{
					num = ((gamepadState.DS3PressureDpadRight != 0) ? ((float)gamepadState.DS3PressureDpadRight / 255f) : 1f);
				}
				else if (btn == 35)
				{
					num = ((gamepadState.DS3PressureDpadLeft != 0) ? ((float)gamepadState.DS3PressureDpadLeft / 255f) : 1f);
				}
			}
			return num;
		}

		private void SetRightBtnValue(GamepadButton btn, float value)
		{
			foreach (RightButtonsInfo rightButtonsInfo in this.RightButtons)
			{
				if (rightButtonsInfo.btn == btn)
				{
					rightButtonsInfo.Value = value;
					break;
				}
			}
		}

		private void SetEllipsePosition(object obj, Point pos)
		{
			if (obj is GeometryDrawing)
			{
				object geometry = ((GeometryDrawing)obj).Geometry;
				if (geometry is EllipseGeometry)
				{
					((EllipseGeometry)geometry).Center = pos;
				}
			}
		}

		private Point GetEllipsePosition(object obj)
		{
			Point center = new Point(0.0, 0.0);
			if (obj is GeometryDrawing)
			{
				object geometry = ((GeometryDrawing)obj).Geometry;
				if (geometry is EllipseGeometry)
				{
					center = ((EllipseGeometry)geometry).Center;
				}
			}
			return center;
		}

		private void ShowKeysButtons(GamepadState gamepadState)
		{
			bool flag = false;
			bool flag2 = false;
			foreach (string text in this.hideBtnsNew)
			{
				this.GamepadDrawing.GetItemByName(text).ChangeColor(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), true, true, true);
			}
			this.hideBtnsNew.Clear();
			foreach (GamepadButtonDescription gamepadButtonDescription in gamepadState.PressedButtons)
			{
				string overlaySVGElementName = XBUtils.GetOverlaySVGElementName(gamepadButtonDescription.Button);
				double num = 2147483647.0;
				double num2 = 2147483647.0;
				if (overlaySVGElementName.CompareTo("Lstick") == 0)
				{
					num = 14.0 * (double)gamepadState.LeftStickX / 32767.0;
					num2 = -14.0 * (double)gamepadState.LeftStickY / 32767.0;
					this.SetStick(this.LeftStickDrawing, "Stick", num * 2.0, num2 * 2.0);
					flag = true;
				}
				else if (overlaySVGElementName.CompareTo("Rstick") == 0 || overlaySVGElementName.CompareTo("Lstick") == 0)
				{
					num = 14.0 * (double)gamepadState.RightStickX / 32767.0;
					num2 = -14.0 * (double)gamepadState.RightStickY / 32767.0;
					this.SetStick(this.RightStickDrawing, "Stick", num * 2.0, num2 * 2.0);
					flag2 = true;
				}
				double num3 = 128.0;
				if (this.ControllerType != null && ControllerTypeExtensions.IsSonyDS3(this.ControllerType.GetValueOrDefault()))
				{
					num3 = (double)this.GetValue(gamepadButtonDescription.Button, gamepadState) * 128.0;
				}
				if (gamepadButtonDescription.Button == 51)
				{
					num3 = (double)((float)gamepadState.LeftTrigger) * 128.0 / 32767.0;
				}
				else if (gamepadButtonDescription.Button == 55)
				{
					num3 = (double)((float)gamepadState.RightTrigger) * 128.0 / 32767.0;
				}
				this.GamepadDrawing.GetItemByName(overlaySVGElementName).ChangeColor(new SolidColorBrush(Color.FromArgb((byte)num3, byte.MaxValue, 231, 149)), true, true, true);
				this.SetStick(this.GamepadDrawing, overlaySVGElementName, num, num2);
				this.hideBtnsNew.Add(overlaySVGElementName);
			}
			if (!flag)
			{
				this.SetStick(this.LeftStickDrawing, "Stick", 0.0, 0.0);
				this.LeftStickDrawing.GetItemByName("StickGroup").ChangeColor(new SolidColorBrush(Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue)), true, true, true);
			}
			if (!flag2)
			{
				this.SetStick(this.RightStickDrawing, "Stick", 0.0, 0.0);
				this.RightStickDrawing.GetItemByName("StickGroup").ChangeColor(new SolidColorBrush(Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue)), true, true, true);
			}
		}

		private void SetStick(Drawing drawing, string SVGName, double dx, double dy)
		{
			Point point;
			if (this.stickCenters.TryGetValue(SVGName, out point))
			{
				point = new Point(point.X + dx, point.Y + dy);
				this.SetEllipsePosition(drawing.GetItemByName(SVGName), point);
			}
		}

		public void SetControllerType(ControllerTypeEnum? controllerType)
		{
			ControllerTypeEnum? controllerTypeEnum;
			if (this.ControllerType != null)
			{
				if (this.ControllerType == null)
				{
					return;
				}
				ControllerTypeEnum? controllerType2 = this.ControllerType;
				controllerTypeEnum = controllerType;
				if ((controllerType2.GetValueOrDefault() == controllerTypeEnum.GetValueOrDefault()) & (controllerType2 != null == (controllerTypeEnum != null)))
				{
					return;
				}
			}
			this.ControllerType = controllerType;
			controllerTypeEnum = controllerType;
			ControllerTypeEnum controllerTypeEnum2 = 1;
			if ((controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null))
			{
				this.GamepadDrawing = ((Drawing)Application.Current.TryFindResource("GamepadXB360")).Clone();
			}
			else
			{
				controllerTypeEnum = controllerType;
				controllerTypeEnum2 = 2;
				if ((controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null))
				{
					this.GamepadDrawing = ((Drawing)Application.Current.TryFindResource("GamepadXBOne")).Clone();
				}
				else
				{
					controllerTypeEnum = controllerType;
					controllerTypeEnum2 = 4;
					if (!((controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null)))
					{
						controllerTypeEnum = controllerType;
						controllerTypeEnum2 = 14;
						if (!((controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null)))
						{
							controllerTypeEnum = controllerType;
							controllerTypeEnum2 = 7;
							if (!((controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null)))
							{
								controllerTypeEnum = controllerType;
								controllerTypeEnum2 = 8;
								if (!((controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null)))
								{
									controllerTypeEnum = controllerType;
									controllerTypeEnum2 = 9;
									if (!((controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null)))
									{
										controllerTypeEnum = controllerType;
										controllerTypeEnum2 = 10;
										if (!((controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null)))
										{
											if (this.ControllerType != null && ControllerTypeExtensions.IsSonyDS3(this.ControllerType.GetValueOrDefault()))
											{
												this.GamepadDrawing = ((Drawing)Application.Current.TryFindResource("GamepadDS3")).Clone();
												goto IL_1E0;
											}
											goto IL_1E0;
										}
									}
								}
							}
							this.GamepadDrawing = ((Drawing)Application.Current.TryFindResource("GamepadNSPro")).Clone();
							goto IL_1E0;
						}
					}
					this.GamepadDrawing = ((Drawing)Application.Current.TryFindResource("GamepadDS4")).Clone();
				}
			}
			IL_1E0:
			this.stickCenters["Rstick"] = this.GetEllipsePosition(this.GamepadDrawing.GetItemByName("Rstick"));
			this.stickCenters["Lstick"] = this.GetEllipsePosition(this.GamepadDrawing.GetItemByName("Lstick"));
			this.LeftStickDrawing = ((Drawing)Application.Current.TryFindResource("DpadOverlay")).Clone();
			this.RightStickDrawing = ((Drawing)Application.Current.TryFindResource("DpadOverlay")).Clone();
			this.stickCenters["Stick"] = this.GetEllipsePosition(this.LeftStickDrawing.GetItemByName("Stick"));
			Collection<GamepadButton> collection = XBUtils.CreateMaskButtonsCollection(new ControllerTypeEnum[] { controllerType.Value }, false);
			this.LeftButtons.Clear();
			this.RightButtons.Clear();
			foreach (GamepadButton gamepadButton in collection)
			{
				if (gamepadButton != null && gamepadButton <= 27)
				{
					string text = XBUtils.ConvertGamepadButtonToDescription(gamepadButton, controllerType);
					string text2 = gamepadButton.ToString();
					Drawing annotationDrawingForGamepadButton = XBUtils.GetAnnotationDrawingForGamepadButton(gamepadButton, controllerType, false);
					if (!string.IsNullOrEmpty(text))
					{
						LeftButtonsInfo leftButtonsInfo = new LeftButtonsInfo();
						leftButtonsInfo.AutoName = text2;
						leftButtonsInfo.Name = text;
						leftButtonsInfo.Image = annotationDrawingForGamepadButton;
						leftButtonsInfo.Value = 0f;
						leftButtonsInfo.btn = gamepadButton;
						this.LeftButtons.Add(leftButtonsInfo);
					}
				}
			}
			List<GamepadButton> list = new List<GamepadButton>();
			list.Add(33);
			list.Add(34);
			list.Add(35);
			list.Add(36);
			list.Add(2000);
			list.Add(99);
			list.Add(2000);
			list.Add(51);
			list.Add(55);
			bool flag = false;
			foreach (GamepadButton gamepadButton2 in list)
			{
				if (!flag)
				{
					controllerTypeEnum = this.ControllerType;
					controllerTypeEnum2 = 4;
					if (((controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null)) || gamepadButton2 != 99)
					{
						string text3 = XBUtils.ConvertGamepadButtonToDescription(gamepadButton2, controllerType);
						string text4 = gamepadButton2.ToString();
						Drawing annotationDrawingForGamepadButton2 = XBUtils.GetAnnotationDrawingForGamepadButton(gamepadButton2, controllerType, false);
						if (!string.IsNullOrEmpty(text3))
						{
							RightButtonsInfo rightButtonsInfo = new RightButtonsInfo();
							if (gamepadButton2 != 2000)
							{
								rightButtonsInfo.AutoName = text4;
								rightButtonsInfo.Name = text3;
								rightButtonsInfo.Image = annotationDrawingForGamepadButton2;
								rightButtonsInfo.Value = 0f;
								rightButtonsInfo.btn = gamepadButton2;
							}
							else
							{
								rightButtonsInfo.Name = "";
								rightButtonsInfo.AutoName = "";
							}
							this.RightButtons.Add(rightButtonsInfo);
						}
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					flag = false;
				}
			}
			this.OnPropertyChanged("IsGyroVisible");
		}

		public ControllerTypeEnum? ControllerType;

		public string ID;

		private const int C_MAX_STICK_VALUE = 32767;

		private const int C_MAX_STICK_OFFSET = 14;

		private float _transparent;

		private bool _isGamepadVisible;

		private bool _isTableVisible;

		private ObservableCollection<MessageInfo> _messages;

		private ObservableCollection<LeftButtonsInfo> _leftButtons;

		private ObservableCollection<RightButtonsInfo> _rightButtons;

		private Drawing _gamepadDrawing;

		private Drawing _leftStickDrawing;

		private Drawing _rightStickDrawing;

		private IDictionary<string, Point> stickCenters = new Dictionary<string, Point>();

		private float _leftXValue;

		private float _leftYValue;

		private float _rightXValue;

		private float _rightYValue;

		private List<string> hideBtnsNew = new List<string>();

		private double GyroXDeltaOld;

		private double GyroYDeltaOld;

		private double GyroZDeltaOld;

		private HorizontalAlignment _alignment;

		private HotkeysInfo _gamepadHotkeysInfo;

		private double _gyroX;

		private double _gyroY;

		private double _gyroZ;
	}
}
