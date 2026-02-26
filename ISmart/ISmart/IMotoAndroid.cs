using System.Collections.Generic;

namespace ISmart;

public interface IMotoAndroid : IDeviceFinder
{
	SortedList<string, SortedList<string, string>> FastbootDevices { get; }

	List<string> Shell(string deviceID, string command, int timeout, string exe, out int exitCode, int waitForResponseTimeOut = 6000, bool usedExeDir = false);
}
