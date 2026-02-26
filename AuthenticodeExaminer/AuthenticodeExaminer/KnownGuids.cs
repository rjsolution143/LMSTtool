using System;

namespace AuthenticodeExaminer;

internal static class KnownGuids
{
	public static Guid WINTRUST_ACTION_GENERIC_VERIFY_V2 { get; } = new Guid(11191659, -12988, 4560, new byte[8] { 140, 194, 0, 192, 79, 194, 149, 238 });

}
