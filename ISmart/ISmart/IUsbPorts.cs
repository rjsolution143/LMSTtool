using System;
using System.Collections.Generic;

namespace ISmart;

public interface IUsbPorts
{
	SortedList<int, UsbPortStatus> PortStatus { get; }

	void PortRefresh();

	SortedList<string, string> PortScan();

	int FindPort(SortedList<string, string> deviceInfo);

	bool WaitForPort(int fixtureIndex, TimeSpan timeout);

	UsbPortStatus PortAssign(int portIndex, SortedList<string, string> deviceInfo);

	UsbPortStatus PortQuery(int portIndex);

	void ClearPorts();
}
