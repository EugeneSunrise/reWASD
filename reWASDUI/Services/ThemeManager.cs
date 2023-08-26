using System;
using System.IO;
using System.Windows;
using DiscSoft.NET.Common.Utils;

namespace reWASDUI.Services
{
	public class ThemeManager
	{
		public event ThemeManager.ThemeChanged OnThemeChanged;

		private ThemeManager()
		{
		}

		public static ThemeManager Instance
		{
			get
			{
				ThemeManager themeManager;
				if ((themeManager = ThemeManager.instance) == null)
				{
					themeManager = (ThemeManager.instance = new ThemeManager());
				}
				return themeManager;
			}
		}

		public bool IsFilledSVGTheme
		{
			get
			{
				return this.CurrentThemeUri == null || this.CurrentThemeUri.ToString().Contains("Filled");
			}
		}

		public void SetCurrentSVGTheme()
		{
			if (RegistryHelper.GetString("GuiNamespace", "SVGScheme", "Filled", false) == "Filled")
			{
				this.SetFilledSVGTheme();
				return;
			}
			this.SetTransparentSVGTheme();
		}

		public void SetFilledSVGTheme()
		{
			this.SetSVGTheme(new Uri("SVG/Gamepads/WiredFilled.xaml", UriKind.Relative));
			RegistryHelper.SetString("GuiNamespace", "SVGScheme", "Filled");
		}

		public void SetTransparentSVGTheme()
		{
			this.SetSVGTheme(new Uri("SVG/Gamepads/WiredTransparent.xaml", UriKind.Relative));
			RegistryHelper.SetString("GuiNamespace", "SVGScheme", "Transparent");
		}

		public void SetSVGTheme(Uri themeUri)
		{
			if (themeUri == this.CurrentThemeUri)
			{
				return;
			}
			this.CurrentThemeUri = themeUri;
			ResourceDictionary resourceDictionary = null;
			try
			{
				resourceDictionary = Application.LoadComponent(themeUri) as ResourceDictionary;
			}
			catch (IOException)
			{
			}
			if (resourceDictionary != null)
			{
				if (this._resourceIndex == -1)
				{
					Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
					this._resourceIndex = Application.Current.Resources.MergedDictionaries.Count - 1;
				}
				else
				{
					Application.Current.Resources.MergedDictionaries[this._resourceIndex] = resourceDictionary;
				}
				if (this.OnThemeChanged != null)
				{
					this.OnThemeChanged();
				}
			}
		}

		public Uri CurrentThemeUri;

		private int _resourceIndex = -1;

		private static ThemeManager instance;

		public delegate void ThemeChanged();
	}
}
