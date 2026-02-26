using System.Collections.Generic;

namespace ISmart;

public interface ILibUsbDotNetDeviceFinder : IDeviceFinder
{
	bool UseDeviceListener { get; set; }

	List<string> FindDevices();

	List<string> Shell(string deviceID, string command, int timeout, string exe, out int exitCode, int waitForResponseTimeOut = 6000, bool usedExeDir = false);
}
