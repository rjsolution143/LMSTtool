using System.Collections.Generic;

namespace ISmart;

public interface IDeviceManager
{
	SortedList<string, IDevice> Devices { get; }

	List<IDevice> ManualDevices { get; }

	bool IncludeManualDevices { get; set; }

	bool BackgroundScan { get; set; }

	IDevice ManualDevice(bool hidden);

	IDevice ManualDevice(string model);

	IDevice ManualDevice();

	void RemoveManualDevice();

	void MergeManualDevice(IDevice manualDevice, IDevice physicalDevice);

	void Refresh();

	List<List<string>> GetVIDPID();

	List<SortedList<string, string>> PortInfo();
}
