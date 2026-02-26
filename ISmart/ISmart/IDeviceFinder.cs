using System.Collections.Generic;

namespace ISmart;

public interface IDeviceFinder
{
	string Name { get; }

	DeviceMode Mode { get; }

	List<string> Refresh();

	bool Check(string id);
}
