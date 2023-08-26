using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.SecondaryWindows;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure;

namespace reWASDUI.Services.HttpClient
{
	public class BaseHttpClientService
	{
		protected void ShowDialogs(List<EnableRemapResponseDialog> dialogs, ref EnableRemapBundle bundle, out bool breakApply, bool httpError = false)
		{
			breakApply = false;
			foreach (EnableRemapResponseDialog enableRemapResponseDialog in dialogs)
			{
				if (enableRemapResponseDialog.DefaultButtonNum >= 0)
				{
					try
					{
						if (enableRemapResponseDialog.Buttons.Count == 1)
						{
							if (enableRemapResponseDialog.Buttons[0].ButtonAction == 7 || enableRemapResponseDialog.Buttons[0].ButtonAction == 12)
							{
								string message = enableRemapResponseDialog.Message;
								string text = null;
								EnableRemapButtonAction buttonAction = enableRemapResponseDialog.Buttons[0].ButtonAction;
								string text2;
								MessageBoxImage messageBoxImage;
								if (buttonAction != 7)
								{
									if (buttonAction != 12)
									{
										break;
									}
									text2 = "CheatWarning";
									messageBoxImage = MessageBoxImage.Exclamation;
								}
								else
								{
									text2 = "RemindAboutUnmappedMouse";
									messageBoxImage = MessageBoxImage.Asterisk;
									text = DTLocalization.GetString(12502);
								}
								MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, message, MessageBoxButton.OK, messageBoxImage, text2, MessageBoxResult.OK, false, 0.0, enableRemapResponseDialog.Buttons[0].Text, null, null, null, text, null);
							}
							else
							{
								DTMessageBox.Show(Application.Current.MainWindow, enableRemapResponseDialog.Message, "", MessageBoxButton.OK, httpError ? MessageBoxImage.Hand : MessageBoxImage.Asterisk, enableRemapResponseDialog.Buttons[0].Text, false, 0.0, MessageBoxResult.None, null, null, null);
							}
							bundle.UserActions.Add(enableRemapResponseDialog.Buttons[0].ButtonAction);
						}
						if (enableRemapResponseDialog.Buttons.Any((EnableRemapResponseButton x) => x.ButtonAction == 5 || x.ButtonAction == 4 || x.ButtonAction == 15 || x.ButtonAction == 16))
						{
							this.ProcessMessageBoxWithRememberMyChoiceLogic(enableRemapResponseDialog, ref bundle, out breakApply);
						}
						else if (enableRemapResponseDialog.Buttons.Count == 2)
						{
							int defaultButtonNum = enableRemapResponseDialog.DefaultButtonNum;
							int num = defaultButtonNum ^ 1;
							EnableRemapResponseButton enableRemapResponseButton;
							if (enableRemapResponseDialog.Buttons.Any((EnableRemapResponseButton x) => x.ButtonAction == 6 || x.ButtonAction == 3 || x.ButtonAction == 2 || x.ButtonAction == 1 || x.ButtonAction == 20))
							{
								enableRemapResponseButton = this.ProcessMessageBoxWithDoNotShowLogic(enableRemapResponseDialog, out breakApply);
							}
							else
							{
								MessageBoxResult messageBoxResult = DTMessageBox.Show(Application.Current.MainWindow, enableRemapResponseDialog.Message, "", MessageBoxButton.OKCancel, MessageBoxImage.Question, enableRemapResponseDialog.Buttons[defaultButtonNum].Text, false, 0.0, MessageBoxResult.OK, enableRemapResponseDialog.Buttons[num].Text, null, null);
								enableRemapResponseButton = enableRemapResponseDialog.Buttons[(messageBoxResult == MessageBoxResult.OK) ? defaultButtonNum : num];
							}
							if (enableRemapResponseButton.ButtonAction != null)
							{
								bundle.UserActions.Add(enableRemapResponseButton.ButtonAction);
							}
							if (enableRemapResponseButton.ButtonAction == null && enableRemapResponseDialog.Buttons[0].Text == DTLocalization.GetString(11824) && enableRemapResponseDialog.Buttons[1].Text == DTLocalization.GetString(5005))
							{
								breakApply = true;
							}
						}
						continue;
					}
					catch
					{
						Tracer.TraceWrite(enableRemapResponseDialog.Message, false);
						continue;
					}
				}
				Tracer.TraceWrite(enableRemapResponseDialog.Message, false);
			}
		}

		private EnableRemapResponseButton ProcessMessageBoxWithDoNotShowLogic(EnableRemapResponseDialog dlg, out bool breakApply)
		{
			breakApply = false;
			int defaultButtonNum = dlg.DefaultButtonNum;
			int num = defaultButtonNum ^ 1;
			EnableRemapResponseButton enableRemapResponseButton = null;
			string message = dlg.Message;
			string text = null;
			if (dlg.Buttons.Any((EnableRemapResponseButton x) => x.ButtonAction == 6 || x.ButtonAction == 3 || x.ButtonAction == 2 || x.ButtonAction == 20))
			{
				string text2;
				MessageBoxImage messageBoxImage;
				if (dlg.Buttons.Any((EnableRemapResponseButton x) => x.ButtonAction == 6))
				{
					text2 = "RemindAboutUnmappedMouse";
					messageBoxImage = MessageBoxImage.Question;
					text = DTLocalization.GetString(11571);
				}
				else if (dlg.Buttons.Any((EnableRemapResponseButton x) => x.ButtonAction == 3))
				{
					text2 = "RemindSteamIgnoreRewasd";
					messageBoxImage = MessageBoxImage.Asterisk;
				}
				else if (dlg.Buttons.Any((EnableRemapResponseButton x) => x.ButtonAction == 2))
				{
					text2 = "ConfirmUnplugPhysicalControler";
					messageBoxImage = MessageBoxImage.Exclamation;
				}
				else
				{
					if (!dlg.Buttons.Any((EnableRemapResponseButton x) => x.ButtonAction == 20))
					{
						return new EnableRemapResponseButton();
					}
					text2 = "ConfirmVirtualUsbHub";
					messageBoxImage = MessageBoxImage.Exclamation;
				}
				MessageBoxButton messageBoxButton = MessageBoxButton.OKCancel;
				MessageBoxResult messageBoxResult = MessageBoxResult.OK;
				MessageBoxResult messageBoxResult2 = MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, message, messageBoxButton, messageBoxImage, text2, messageBoxResult, false, 0.0, dlg.Buttons[defaultButtonNum].Text, dlg.Buttons[num].Text, null, null, text, null);
				enableRemapResponseButton = dlg.Buttons[(messageBoxResult2 == messageBoxResult) ? defaultButtonNum : num];
			}
			if (dlg.Buttons.Any((EnableRemapResponseButton x) => x.ButtonAction == 1))
			{
				string text2 = "TryApplyLockedToNoTrial";
				MessageBoxButton messageBoxButton = MessageBoxButton.YesNo;
				MessageBoxResult messageBoxResult = MessageBoxResult.Yes;
				MessageBoxImage messageBoxImage = MessageBoxImage.Asterisk;
				text = DTLocalization.GetString(11579);
				string @string = DTLocalization.GetString(11094);
				MessageBoxResult messageBoxResult2;
				if (!string.IsNullOrEmpty(dlg.LicenseInfo) && dlg.LicenseInfo.ToLower().Contains("NeedAdvancedFeatureMouseStick".ToLower()))
				{
					Window mainWindow = Application.Current.MainWindow;
					string text3 = message;
					MessageBoxButton messageBoxButton2 = messageBoxButton;
					MessageBoxImage messageBoxImage2 = messageBoxImage;
					MessageBoxResult messageBoxResult3 = messageBoxResult;
					string text4 = dlg.Buttons[defaultButtonNum].Text;
					string text5 = dlg.Buttons[num].Text;
					messageBoxResult2 = DTMessageBox.Show(mainWindow, text3, text, messageBoxButton2, messageBoxImage2, "", false, 0.0, messageBoxResult3, null, text4, text5);
				}
				else
				{
					messageBoxResult2 = MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, message, messageBoxButton, messageBoxImage, text2, messageBoxResult, false, 0.0, null, null, dlg.Buttons[defaultButtonNum].Text, dlg.Buttons[num].Text, text, @string);
				}
				enableRemapResponseButton = dlg.Buttons[(messageBoxResult2 == messageBoxResult) ? defaultButtonNum : num];
				if (messageBoxResult2 == MessageBoxResult.No)
				{
					breakApply = true;
					DSUtils.GoUrl(dlg.AdditionalParameter);
				}
			}
			return enableRemapResponseButton ?? new EnableRemapResponseButton();
		}

		private void ProcessMessageBoxWithRememberMyChoiceLogic(EnableRemapResponseDialog dlg, ref EnableRemapBundle bundle, out bool breakApply)
		{
			breakApply = false;
			int defaultButtonNum = dlg.DefaultButtonNum;
			EnableRemapResponseButton enableRemapResponseButton = null;
			string message = dlg.Message;
			if (dlg.Buttons.Count >= 2)
			{
				if (dlg.Buttons.Any((EnableRemapResponseButton x) => x.ButtonAction == 15 || x.ButtonAction == 16))
				{
					int num = defaultButtonNum ^ 1;
					MessageBoxButton messageBoxButton = MessageBoxButton.YesNo;
					MessageBoxImage messageBoxImage = MessageBoxImage.Asterisk;
					string text = DTLocalization.GetString(11579);
					MessageBoxResult messageBoxResult = MessageBoxWithRememberMyChoiceLogic.Show(Application.Current.MainWindow, message, messageBoxButton, messageBoxImage, "TryApplyLockedTrial", "GuiNamespace", "TryApplyLockedTrialResult", false, 0.0, null, null, dlg.Buttons[defaultButtonNum].Text, dlg.Buttons[num].Text, text);
					enableRemapResponseButton = dlg.Buttons[(messageBoxResult == MessageBoxResult.Yes) ? defaultButtonNum : num];
				}
			}
			if (dlg.Buttons.Count >= 2)
			{
				if (dlg.Buttons.Any((EnableRemapResponseButton x) => x.ButtonAction == 5 || x.ButtonAction == 4))
				{
					MessageBoxButton messageBoxButton = MessageBoxButton.YesNoCancel;
					MessageBoxImage messageBoxImage = MessageBoxImage.Question;
					string text = DTLocalization.GetString(11571);
					Window mainWindow = Application.Current.MainWindow;
					string text2 = message;
					MessageBoxButton messageBoxButton2 = messageBoxButton;
					MessageBoxImage messageBoxImage2 = messageBoxImage;
					string text3 = "ConfirmAskwhetherToFixConfigWithUnmappedMouse";
					string text4 = "GuiNamespace";
					string text5 = "AskwhetherToFixConfigWithUnmappedMouse";
					bool flag = false;
					double num2 = 0.0;
					string text6 = null;
					string text7 = dlg.Buttons[0].Text;
					string text8 = dlg.Buttons[1].Text;
					MessageBoxResult messageBoxResult = MessageBoxWithRememberMyChoiceLogic.Show(mainWindow, text2, messageBoxButton2, messageBoxImage2, text3, text4, text5, flag, num2, text6, DTLocalization.GetString(5005), text7, text8, text);
					if (messageBoxResult == MessageBoxResult.Yes)
					{
						enableRemapResponseButton = dlg.Buttons[0];
					}
					if (messageBoxResult == MessageBoxResult.No)
					{
						enableRemapResponseButton = dlg.Buttons[1];
					}
					if (messageBoxResult == MessageBoxResult.Cancel)
					{
						breakApply = true;
					}
				}
			}
			if (enableRemapResponseButton != null && enableRemapResponseButton.ButtonAction != null)
			{
				bundle.UserActions.Add(enableRemapResponseButton.ButtonAction);
			}
		}

		private protected bool CheckResponseError(HttpResponseMessage resp)
		{
			if (resp == null)
			{
				return false;
			}
			if (!resp.IsSuccessStatusCode)
			{
				string text = "";
				if (resp.StatusCode == HttpStatusCode.BadRequest)
				{
					text = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
				}
				if (string.IsNullOrWhiteSpace(text))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 2);
					defaultInterpolatedStringHandler.AppendLiteral("Code: ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(Convert.ToInt32(resp.StatusCode));
					defaultInterpolatedStringHandler.AppendLiteral(" ");
					defaultInterpolatedStringHandler.AppendFormatted(resp.ReasonPhrase);
					text = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				else
				{
					if (text == DTLocalization.GetString(11853))
					{
						EnableRemapResponse enableRemapResponse = new EnableRemapResponse();
						EnableRemapBundle enableRemapBundle = new EnableRemapBundle
						{
							IsUI = true
						};
						EnableRemapResponseDialog enableRemapResponseDialog = new EnableRemapResponseDialog
						{
							Message = text
						};
						enableRemapResponseDialog.AddButton(new EnableRemapResponseButton
						{
							Text = DTLocalization.GetString(5004),
							ButtonAction = 9
						}, enableRemapBundle.IsUI);
						enableRemapResponse.AddDialog(enableRemapResponseDialog);
						enableRemapResponse.DontReCallEnableRemap = true;
						enableRemapResponse.PostAction = 1;
						bool flag;
						this.ShowDialogs(enableRemapResponse.Dialogs, ref enableRemapBundle, out flag, true);
					}
					if (text == DTLocalization.GetString(12481) || text == DTLocalization.GetString(12495))
					{
						EnableRemapResponse enableRemapResponse2 = new EnableRemapResponse();
						EnableRemapBundle enableRemapBundle2 = new EnableRemapBundle
						{
							IsUI = true
						};
						EnableRemapResponseDialog enableRemapResponseDialog2 = new EnableRemapResponseDialog
						{
							Message = text
						};
						EnableRemapButtonAction enableRemapButtonAction = 0;
						if (text == DTLocalization.GetString(12481))
						{
							enableRemapButtonAction = 18;
						}
						if (text == DTLocalization.GetString(12495))
						{
							enableRemapButtonAction = 19;
						}
						enableRemapResponseDialog2.AddButton(new EnableRemapResponseButton
						{
							Text = DTLocalization.GetString(5004),
							ButtonAction = enableRemapButtonAction
						}, enableRemapBundle2.IsUI);
						enableRemapResponse2.AddDialog(enableRemapResponseDialog2);
						enableRemapResponse2.DontReCallEnableRemap = true;
						enableRemapResponse2.PostAction = 0;
						bool flag;
						this.ShowDialogs(enableRemapResponse2.Dialogs, ref enableRemapBundle2, out flag, true);
					}
				}
				Tracer.TraceWrite(text, false);
				return false;
			}
			return true;
		}
	}
}
