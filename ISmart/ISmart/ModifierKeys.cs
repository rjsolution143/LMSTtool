using System;

namespace ISmart;

[Flags]
public enum ModifierKeys : uint
{
	Alt = 1u,
	Control = 2u,
	Shift = 4u,
	Win = 8u
}
