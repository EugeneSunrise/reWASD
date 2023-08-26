using System;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Properties;
using reWASDUI.Infrastructure.KeyBindings;
using XBEliteWPF.Infrastructure;

namespace reWASDUI.Services.Interfaces
{
	public interface IConfigFileService
	{
		Task<bool> ParseConfigFile(string gameName, string configName, string configPath, ConfigData bindingsCollections, bool verbose = true, [CanBeNull] JsonInfo jsonInfo = null);

		Task<bool> SaveConfigFile(string gameName, string configName, ConfigData bindingsCollections);

		bool ValidateConfigFile(string configFileName);

		bool XMLGetVersion(string fileName, ref double version);
	}
}
