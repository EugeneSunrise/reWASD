using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Prism.Commands;
using reWASDCommon.Infrastructure;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Services.Interfaces;
using reWASDUI.Utils.WebUtils;
using reWASDUI.Utils.XBUtil;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.ViewModels
{
	public class ShareVM : ZBindable
	{
		public ShareVM(ConfigVM configVM, IConfigFileService ifs, IGameProfilesService gps, ILicensingService ls)
		{
			this._configVM = configVM;
			this._configFileService = ifs;
			this._gameProfileService = gps;
			this._licensingService = ls;
			this.ConfigDescription = ((configVM.CurrentBindingCollection == null || string.IsNullOrEmpty(configVM.CurrentBindingCollection.Comment)) ? ("Custom configuration to play " + this._configVM.ParentGame.Name + " game") : configVM.CurrentBindingCollection.Comment);
		}

		public string ConfigDescription
		{
			get
			{
				return this._configDescription;
			}
			set
			{
				this.SetProperty<string>(ref this._configDescription, value, "ConfigDescription");
			}
		}

		public DelegateCommand ShareCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._ShareCommand) == null)
				{
					delegateCommand = (this._ShareCommand = new DelegateCommand(new Action(this.Share)));
				}
				return delegateCommand;
			}
		}

		private void Share()
		{
			this.ShareConfig();
		}

		private async void ShareConfig()
		{
			bool nextAttempt = true;
			IWebProxy requestProxy = WebRequest.DefaultWebProxy;
			ConfigData bindings = XBUtils.CreateDefaultCollectionXBBindingWrappers(this._configVM, true);
			int num;
			try
			{
				TaskAwaiter<bool> taskAwaiter = this._configFileService.ParseConfigFile(this._configVM.GameName, this._configVM.Name, this._configVM.ConfigPath, bindings, true, null).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
					num = -1;
				}
				if (!taskAwaiter.GetResult())
				{
					DTMessageBox.Show(DTLocalization.GetString(11076) + " " + this._configVM.ConfigPath, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
					return;
				}
			}
			catch (Exception)
			{
				try
				{
					File.Delete(this._configVM.ConfigPath);
				}
				catch (Exception)
				{
				}
				DTMessageBox.Show(DTLocalization.GetString(11076), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
				return;
			}
			bindings.First<SubConfigData>().MainXBBindingCollection.Comment = this.ConfigDescription;
			try
			{
				await this._configFileService.SaveConfigFile(this._configVM.GameName, this._configVM.Name, bindings);
			}
			catch (Exception ex)
			{
				DTMessageBox.Show(DTLocalization.GetString(11076) + " " + this._configVM.ConfigPath, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
				Tracer.TraceException(ex, "ShareConfig");
				return;
			}
			if (!this._configFileService.ValidateConfigFile(this._configVM.ConfigPath))
			{
				Tracer.TraceWrite("ShareConfig: ValidateConfigFile() return FALSE", false);
				DTMessageBox.Show(DTLocalization.GetString(11210) + " " + this._configVM.ConfigPath, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
			}
			else if (this._configVM.IsEmpty)
			{
				Tracer.TraceWrite("ShareConfig: ConfigVM.IsEmpty return TRUE", false);
				DTMessageBox.Show(DTLocalization.GetString(11125) + " " + this._configVM.ConfigPath, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
			}
			else
			{
				string text = "";
				try
				{
					text = this.GetEscapedDataString(Convert.ToBase64String(File.ReadAllBytes(this._configVM.ConfigPath)));
				}
				catch (Exception)
				{
					DTMessageBox.Show(DTLocalization.GetString(11076) + " " + this._configVM.ConfigPath, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
					return;
				}
				string text2 = "";
				try
				{
					using (FileStream fileStream = File.OpenRead(this._configVM.ConfigPath))
					{
						using (SHA1 sha = SHA1.Create())
						{
							text2 = BitConverter.ToString(sha.ComputeHash(fileStream)).Replace("-", string.Empty).ToLower();
						}
					}
				}
				catch (Exception)
				{
					DTMessageBox.Show(DTLocalization.GetString(11075) + " " + this._configVM.ConfigPath, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
					return;
				}
				string text3 = Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(this._configVM.Name + ".rewasd")));
				string text4 = "filename=" + text3 + "&";
				text4 = text4 + "filebody=" + text + "&";
				text4 = text4 + "sha1=" + text2 + "&";
				text4 = text4 + "gamename=" + this._configVM.ParentGame.Name + "&";
				if (this._licensingService.IsPaidUser)
				{
					text4 = text4 + "serial=" + this._licensingService.Serial;
				}
				string text5 = AESCrypter.EncryptString(text4, null);
				string escapedDataString = this.GetEscapedDataString(text5);
				string text6 = "";
				int num2 = 0;
				while (nextAttempt)
				{
					nextAttempt = false;
					try
					{
						byte[] bytes = Encoding.UTF8.GetBytes("config=" + escapedDataString);
						num2 = bytes.Length;
						HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://store.rewasd.com/config/v2");
						httpWebRequest.Proxy = requestProxy;
						httpWebRequest.UserAgent = Constants.WEB_PRODUCT_USER_AGENT;
						httpWebRequest.Method = "POST";
						httpWebRequest.ContentType = "application/x-www-form-urlencoded";
						httpWebRequest.ContentLength = (long)num2;
						Stream requestStream = httpWebRequest.GetRequestStream();
						requestStream.Write(bytes, 0, bytes.Length);
						requestStream.Close();
						using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
						{
							using (Stream responseStream = httpWebResponse.GetResponseStream())
							{
								using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
								{
									text6 = AESCrypter.DecryptString(streamReader.ReadToEnd(), null);
								}
							}
						}
						try
						{
							Clipboard.SetText(text6);
						}
						catch (Exception ex2)
						{
							Tracer.TraceException(ex2, "ShareConfig");
						}
						this.Window.DialogResult = new bool?(true);
					}
					catch (WebException ex3)
					{
						if (ex3.Response == null)
						{
							Tracer.TraceWrite("ShareConfig: WebException. wx.Response is NULL", false);
							DTMessageBox.Show(this.Window, DTLocalization.GetString(11102), MessageBoxButton.OK, MessageBoxImage.Hand, null);
							return;
						}
						HttpWebResponse httpWebResponse2 = ex3.Response as HttpWebResponse;
						if (httpWebResponse2 != null)
						{
							if (httpWebResponse2.StatusCode == HttpStatusCode.BadRequest)
							{
								Tracer.TraceWrite("ShareConfig: WebException. webResp.StatusCode = HttpStatusCode.BadRequest", false);
								DTMessageBox.Show("The remote server returned an error: (400) Bad Request", MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
								SenderGoogleAnalytics.SendMessageEvent("GUI", "ShareError", (num2 / 1000).ToString(), -1L, false);
								return;
							}
							if (httpWebResponse2.StatusCode == HttpStatusCode.RequestEntityTooLarge)
							{
								Tracer.TraceWrite("ShareConfig: WebException. webResp.StatusCode = HttpStatusCode.RequestEntityTooLarge", false);
								DTMessageBox.Show(DTLocalization.GetString(11445), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
								SenderGoogleAnalytics.SendMessageEvent("GUI", "ShareError", (num2 / 1000).ToString(), -1L, false);
								return;
							}
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 1);
							defaultInterpolatedStringHandler.AppendLiteral("ShareConfig: WebException. webResp.StatusCode = ");
							defaultInterpolatedStringHandler.AppendFormatted<HttpStatusCode>(httpWebResponse2.StatusCode);
							Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
						}
						Stream responseStream2 = ex3.Response.GetResponseStream();
						try
						{
							if (responseStream2 != null)
							{
								StreamReader streamReader2 = new StreamReader(responseStream2);
								try
								{
									Tracer.TraceWrite("ShareConfig: WebException. wx.Response.GetResponseStream()", false);
									DTMessageBox.Show(this.Window, streamReader2.ReadToEnd(), MessageBoxButton.OK, MessageBoxImage.Hand, null);
								}
								finally
								{
									if (num < 0 && streamReader2 != null)
									{
										((IDisposable)streamReader2).Dispose();
									}
								}
							}
						}
						finally
						{
							if (num < 0 && responseStream2 != null)
							{
								((IDisposable)responseStream2).Dispose();
							}
						}
					}
				}
				if (!string.IsNullOrEmpty(text6))
				{
					DSUtils.GoUrl(text6);
				}
			}
		}

		private string GetEscapedDataString(string str)
		{
			int num = 32000;
			StringBuilder stringBuilder = new StringBuilder();
			int num2 = str.Length / num;
			for (int i = 0; i <= num2; i++)
			{
				if (i < num2)
				{
					stringBuilder.Append(Uri.EscapeDataString(str.Substring(num * i, num)));
				}
				else
				{
					stringBuilder.Append(Uri.EscapeDataString(str.Substring(num * i)));
				}
			}
			return stringBuilder.ToString();
		}

		private ConfigVM _configVM;

		private string _configDescription;

		private IConfigFileService _configFileService;

		private IGameProfilesService _gameProfileService;

		private ILicensingService _licensingService;

		public Window Window;

		private DelegateCommand _ShareCommand;
	}
}
