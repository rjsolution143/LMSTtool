using System;

namespace ISmart;

public interface IApp
{
	string Name { get; }

	string Version { get; }

	bool Restart { get; set; }

	bool FatalError { get; set; }

	bool CrashRecovery { get; set; }

	void Run();

	void Exit();

	void ScheduleShutdownTask(string name, Action task);
}
