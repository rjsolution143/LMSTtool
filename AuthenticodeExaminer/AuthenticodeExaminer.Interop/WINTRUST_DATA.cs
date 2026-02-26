using System;

namespace AuthenticodeExaminer.Interop;

internal struct WINTRUST_DATA
{
	public uint cbStruct;

	public IntPtr pPolicyCallbackData;

	public IntPtr pSIPClientData;

	public WinTrustDataUIChoice dwUIChoice;

	public WinTrustRevocationChecks fdwRevocationChecks;

	public WinTrustUnionChoice dwUnionChoice;

	public WINTRUST_DATA_UNION trustUnion;

	public WinTrustStateAction dwStateAction;

	public IntPtr hWVTStateData;

	public IntPtr pwszURLReference;

	public WinTrustProviderFlags dwProvFlags;

	public WinTrustUIContext dwUIContext;

	public IntPtr pSignatureSettings;
}
