using System;

namespace ISmart;

[Flags]
public enum DeviceMode
{
	UNKNOWN = 1,
	ADB = 2,
	FastBoot = 4,
	TCMD = 8,
	Disconnected = 0x10
}
