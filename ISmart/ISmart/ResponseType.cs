using System;

namespace ISmart;

[Flags]
public enum ResponseType : byte
{
	Request = 0,
	Failure = 1,
	Data = 2,
	Unsolicited = 4,
	Response = 0x80
}
