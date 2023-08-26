using System;

namespace reWASDUI.Services.Interfaces
{
	public interface ISSEProcessor
	{
		void InitAndRun();

		void Restart();

		void StopAndClose();
	}
}
