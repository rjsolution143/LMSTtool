using System.Runtime.InteropServices;

namespace AuthenticodeExaminer.Interop;

[StructLayout(LayoutKind.Explicit)]
internal struct WINTRUST_DATA_UNION
{
	[FieldOffset(0)]
	public unsafe WINTRUST_FILE_INFO* pFile;
}
