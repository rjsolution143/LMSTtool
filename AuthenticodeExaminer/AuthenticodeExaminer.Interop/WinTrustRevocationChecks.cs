using System;

namespace AuthenticodeExaminer.Interop;

[Flags]
internal enum WinTrustRevocationChecks : uint
{
	WTD_REVOKE_NONE = 0u,
	WTD_REVOKE_WHOLECHAIN = 1u
}
