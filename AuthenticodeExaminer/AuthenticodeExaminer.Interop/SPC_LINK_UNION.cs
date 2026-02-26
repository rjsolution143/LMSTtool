using System;
using System.Runtime.InteropServices;

namespace AuthenticodeExaminer.Interop;

[StructLayout(LayoutKind.Explicit)]
internal struct SPC_LINK_UNION
{
	[FieldOffset(0)]
	public IntPtr pwszUrl;

	[FieldOffset(0)]
	public SPC_SERIALIZED_OBJECT Moniker;

	[FieldOffset(0)]
	public IntPtr pwszFile;
}
