using System;
using System.Threading.Tasks;

namespace XBEliteWPF.Services.Interfaces
{
	public interface IHttpServer
	{
		Task<bool> InitAndRun();

		Task<bool> Restart();

		void StopAndClose();
	}
}
