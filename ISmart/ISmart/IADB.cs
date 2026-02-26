using System;
using System.Collections.Generic;

namespace ISmart;

public interface IADB : IDeviceFinder
{
	event EventHandler<string> OnDeviceNotify;

	List<string> FindDevices();

	string Shell(string deviceID, string command, int timeoutMs = 10000);

	void Install(string deviceID, string apkPath);

	void Uninstall(string deviceID, string apkName);

	void ForwardPort(string deviceID, int devicePort, int localPort);

	void RemoveForward(string deviceID, int localPort);

	void PushFile(string deviceID, string localFilePath, string deviceFilePath);

	void Reboot(string deviceID, string mode);
}
