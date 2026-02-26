using System.Collections.Generic;

namespace ISmart;

public interface IPnpDeviceFinder : IDeviceFinder
{
	SortedList<string, SortedList<string, string>> FastbootDevices { get; }

	List<string> FindDevices();

	List<string> Shell(string deviceID, string command, int timeout, string exe, out int exitCode, int waitForResponseTimeOut = 6000, bool usedExeDir = false);

	void Listen();
}
