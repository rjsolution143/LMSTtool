using System.Runtime.InteropServices;

namespace AuthenticodeExaminer.Interop;

internal struct SPC_SP_OPUS_INFO
{
	[MarshalAs(UnmanagedType.LPWStr)]
	public string pwszProgramName;

	public unsafe SPC_LINK* pMoreInfo;

	public unsafe SPC_LINK* pPublisherInfo;
}
