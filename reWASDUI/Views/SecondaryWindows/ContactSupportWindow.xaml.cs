using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows;
using System.Windows.Markup;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.Controls;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.SecondaryWindows.Base;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Newtonsoft.Json;
using Prism.Commands;

namespace reWASDUI.Views.SecondaryWindows
{
	public partial class ContactSupportWindow : BaseSecondaryWindow, IDataErrorInfo
	{
		public bool EmailIsValid(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				return false;
			}
			try
			{
				return Regex.IsMatch(email, "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250.0));
			}
			catch (RegexMatchTimeoutException)
			{
			}
			return false;
		}

		protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (object.Equals(storage, value))
			{
				return false;
			}
			storage = value;
			this.OnPropertyChanged(propertyName);
			return true;
		}

		public string Subject
		{
			get
			{
				return this._subject;
			}
			set
			{
				this.SetProperty<string>(ref this._subject, value, "Subject");
			}
		}

		public string EMail
		{
			get
			{
				return this._email;
			}
			set
			{
				this.SetProperty<string>(ref this._email, value, "EMail");
			}
		}

		public string Message
		{
			get
			{
				return this._message;
			}
			set
			{
				this.SetProperty<string>(ref this._message, value, "Message");
			}
		}

		public ContactSupportWindow(string serial, string hwid)
			: base(MessageBoxResult.OK, true, false)
		{
			this.Serial = serial;
			this.HWID = hwid;
			base.DataContext = this;
			this.InitializeComponent();
		}

		public DelegateCommand<string> GoUrl
		{
			get
			{
				DelegateCommand<string> delegateCommand;
				if ((delegateCommand = this._goUrl) == null)
				{
					delegateCommand = (this._goUrl = new DelegateCommand<string>(delegate(string path)
					{
						Process.Start(new ProcessStartInfo(path)
						{
							UseShellExecute = true
						});
					}));
				}
				return delegateCommand;
			}
		}

		public static string UrlEncode(string url)
		{
			if (url == null)
			{
				return "";
			}
			return HttpUtility.UrlEncode(url).Replace("+", "%20");
		}

		public static string UrlDecode(string url)
		{
			return HttpUtility.UrlDecode(url);
		}

		private string GetHash(string message)
		{
			return DSUtils.sha256_hash(DSUtils.sha256_hash(message) + "P3SRcji7NbO7bQK~fFW++^$iC7)]#dW}");
		}

		private string GetParameters(string email, string subject, string message)
		{
			string text = string.Format("{0}.{1}.{2}", 6, 7, 0);
			return string.Format("message={0}&subject={1}&hash={2}&product=rewasd&version={3}&os={4}&serial={5}&hwkey={6}&from={7}", new object[]
			{
				ContactSupportWindow.UrlEncode(message),
				ContactSupportWindow.UrlEncode(subject),
				this.GetHash(message),
				text,
				ContactSupportWindow.UrlEncode(Environment.OSVersion.VersionString),
				this.Serial,
				this.HWID,
				ContactSupportWindow.UrlEncode(email)
			});
		}

		private void CheckParameterIsEmpty()
		{
			this._useValidationForEmptyFields = true;
			this.OnPropertyChanged("EMail");
			this.OnPropertyChanged("Message");
			this._useValidationForEmptyFields = false;
		}

		protected async void SendButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.EMail.Length == 0 || this.Message.Length == 0 || !this.EmailIsValid(this.EMail))
			{
				this.CheckParameterIsEmpty();
			}
			else
			{
				string parameters = this.GetParameters(this.EMail, this.Subject, this.Message);
				byte[] bytes = Encoding.UTF8.GetBytes(parameters);
				int num = bytes.Length;
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.daemon-tools.cc/contacts/send");
				httpWebRequest.Method = "POST";
				httpWebRequest.ContentType = "application/x-www-form-urlencoded";
				httpWebRequest.ContentLength = (long)num;
				Stream requestStream = httpWebRequest.GetRequestStream();
				requestStream.Write(bytes, 0, bytes.Length);
				requestStream.Close();
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					if (httpWebResponse.StatusCode == HttpStatusCode.OK)
					{
						using (Stream responseStream = httpWebResponse.GetResponseStream())
						{
							using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
							{
								string text = streamReader.ReadToEnd();
								ContactSupportWindow.ResponseResult responseResult = null;
								try
								{
									responseResult = JsonConvert.DeserializeObject<ContactSupportWindow.ResponseResult>(text);
								}
								catch (Exception)
								{
								}
								if (responseResult != null && responseResult.success)
								{
									DTMessageBox.Show(Application.Current.MainWindow, DTLocalization.GetString(12803), DTLocalization.GetString(12805), MessageBoxButton.OK, MessageBoxImage.Asterisk);
									base.Close();
								}
								else if (!string.IsNullOrEmpty((responseResult != null) ? responseResult.errors.message : null) || !string.IsNullOrEmpty((responseResult != null) ? responseResult.errors.from : null))
								{
									DTMessageBox.Show(responseResult.errors.message + ((responseResult != null) ? responseResult.errors.from : null), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
								}
								else
								{
									DTMessageBox.Show(DTLocalization.GetString(12804), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
								}
							}
							goto IL_202;
						}
					}
					DTMessageBox.Show(DTLocalization.GetString(12804), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
					IL_202:;
				}
			}
		}

		public string this[string columnName]
		{
			get
			{
				string text = string.Empty;
				if (!(columnName == "Subject"))
				{
					if (!(columnName == "EMail"))
					{
						if (columnName == "Message")
						{
							if (this._useValidationForEmptyFields && string.IsNullOrEmpty(this.Message))
							{
								text = DTLocalization.GetString(12802);
							}
						}
					}
					else if (this._useValidationForEmptyFields && string.IsNullOrEmpty(this.EMail))
					{
						text = DTLocalization.GetString(12802);
					}
					else if (!string.IsNullOrEmpty(this.EMail) && !this.EmailIsValid(this.EMail))
					{
						text = DTLocalization.GetString(12808);
					}
				}
				else if (string.IsNullOrEmpty(this.Subject))
				{
					text = DTLocalization.GetString(12802);
				}
				return text;
			}
		}

		public string Error
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		private const string URL = "https://www.daemon-tools.cc/contacts/send";

		private string Serial;

		private string HWID;

		private string _subject;

		private string _email = "";

		private string _message = "";

		private DelegateCommand<string> _goUrl;

		private bool _useValidationForEmptyFields;

		[JsonObject(0)]
		public struct Errors
		{
			public string captcha { readonly get; set; }

			public string message { readonly get; set; }

			public string from { readonly get; set; }
		}

		[JsonObject(0)]
		public class ResponseResult
		{
			public bool success { get; set; }

			public string html { get; set; }

			public ContactSupportWindow.Errors errors { get; set; }
		}
	}
}
