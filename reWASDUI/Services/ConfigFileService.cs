using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using DiscSoft.NET.Common.Utils;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.JSONModel;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Services.Interfaces;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDUI.Services
{
	public class ConfigFileService : IConfigFileService
	{
		private static bool XMLParseVersion(string input, ref double version)
		{
			StringReader stringReader = null;
			bool flag;
			try
			{
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
				{
					IgnoreWhitespace = true,
					IgnoreComments = true
				};
				stringReader = new StringReader(input);
				XmlReader xmlReader = XmlReader.Create(stringReader, xmlReaderSettings);
				while (xmlReader.Read())
				{
					if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name.ToLower() == "rewasd")
					{
						if (xmlReader.HasAttributes)
						{
							while (xmlReader.MoveToNextAttribute())
							{
								if (xmlReader.Name.ToLower() == "version")
								{
									version = double.Parse(xmlReader.Value, CultureInfo.InvariantCulture);
									return true;
								}
							}
						}
						version = 1.0;
						return true;
					}
				}
				flag = false;
			}
			finally
			{
				if (stringReader != null)
				{
					stringReader.Dispose();
				}
			}
			return flag;
		}

		public bool XMLGetVersion(string fileName, ref double version)
		{
			FileStream fileStream = null;
			StreamReader streamReader = null;
			bool flag;
			try
			{
				fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
				streamReader = new StreamReader(fileStream);
				string text = streamReader.ReadToEnd();
				streamReader.Close();
				fileStream.Close();
				flag = ConfigFileService.XMLParseVersion(text, ref version);
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Dispose();
				}
				if (streamReader != null)
				{
					streamReader.Dispose();
				}
			}
			return flag;
		}

		public bool ValidateConfigFile(string configFileName)
		{
			ConfigJSON_3_0 configJSON_3_ = new ConfigJSON_3_0();
			try
			{
				if (configJSON_3_.SchemaValidateConfig(configFileName))
				{
					Tracer.TraceWrite("ValidateConfigFile: success", false);
					return true;
				}
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "ValidateConfigFile");
			}
			Tracer.TraceWrite("ValidateConfigFile: JSON Schema validation FAILED for file " + configFileName, false);
			return false;
		}

		public Task<bool> ParseConfigFile(string gameName, string configName, string configPath, ConfigData bindingsCollections, bool verbose = true, JsonInfo jsonInfo = null)
		{
			Tracer.TraceWrite("ParseConfigFile: Processing config file " + configName, false);
			if (configName == null)
			{
				Tracer.TraceWrite("ParseConfigFile: config file name is empty", false);
				return Task.FromResult<bool>(false);
			}
			ConfigData configData;
			if (configName == "LizardMode.rewasd" || configName == "SteamDeckLizardMode.rewasd" || configName == "Default.rewasd")
			{
				try
				{
					ConfigJSON_3_0 configJSON_3_ = new ConfigJSON_3_0();
					configData = XBUtils.CreateConfigData(false);
					configJSON_3_.ReadBindingsFromFile(configName, configData, false, jsonInfo);
				}
				catch (Exception ex)
				{
					Tracer.TraceWrite("ParseConfigFile: parsing LizardMode config return exception", false);
					Tracer.TraceException(ex, "ParseConfigFile");
					return Task.FromResult<bool>(false);
				}
				Tracer.TraceWrite("ParseConfigFile: parsing LizardMode config success", false);
				return Task.FromResult<bool>(true);
			}
			new List<ControllerTypeEnum>().Add(1);
			ConfigJSON_3_0 configJSON_3_2 = new ConfigJSON_3_0();
			configData = XBUtils.CreateConfigData(false);
			try
			{
				configJSON_3_2.ReadBindingsFromFile(configPath, configData, false, jsonInfo);
			}
			catch (Exception ex2)
			{
				Tracer.TraceWrite("ParseConfigFile: parsing config return exception. Config path = " + configPath, false);
				Tracer.TraceException(ex2, "ParseConfigFile");
				return Task.FromResult<bool>(false);
			}
			if (configData == null)
			{
				return Task.FromResult<bool>(false);
			}
			bindingsCollections.CopyFromModel(configData);
			Tracer.TraceWrite("ParseConfigFile: parsing config file return success", false);
			return Task.FromResult<bool>(true);
		}

		public async Task<bool> SaveConfigFile(string gameName, string configName, ConfigData bindingsCollections)
		{
			ConfigData configData = XBUtils.CreateConfigData(true);
			bindingsCollections.CopyToModel(configData);
			return (await App.HttpClientService.GameProfiles.SaveConfig(new SaveConfigParams<ConfigData>
			{
				ClientID = SSEClient.ClientID,
				ConfigName = configName,
				GameName = gameName,
				ConfigData = configData
			})).Result;
		}
	}
}
