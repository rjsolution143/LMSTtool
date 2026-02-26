using System;
using System.Collections.Generic;

namespace ISmart;

public interface IDeviceListener
{
	DateTime LastArrival { get; }

	DateTime LastRemoval { get; }

	void Listen();

	SortedList<string, SortedList<string, string>> GetFastbootDevicesInfo();
}
