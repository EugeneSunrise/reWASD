using System;
using System.Threading.Tasks;

namespace XBEliteWPF.Services.Interfaces
{
	public interface IUdpServer
	{
		Task<bool> InitAndRun();

		Task<bool> Restart();

		void StopAndClose();
	}
}
