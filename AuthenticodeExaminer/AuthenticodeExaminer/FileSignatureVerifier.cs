using System;
using System.Runtime.InteropServices;
using AuthenticodeExaminer.Interop;

namespace AuthenticodeExaminer;

internal static class FileSignatureVerifier
{
	public unsafe static int IsFileSignatureValid(string file, RevocationChecking revocationChecking)
	{
		IntPtr intPtr = Marshal.StringToHGlobalUni(file);
		try
		{
			WinTrustProviderFlags winTrustProviderFlags = WinTrustProviderFlags.NONE;
			WinTrustRevocationChecks winTrustRevocationChecks = WinTrustRevocationChecks.WTD_REVOKE_NONE;
			if (revocationChecking != 0)
			{
				if (revocationChecking == RevocationChecking.Offline)
				{
					winTrustProviderFlags |= WinTrustProviderFlags.WTD_CACHE_ONLY_URL_RETRIEVAL;
				}
				winTrustProviderFlags |= WinTrustProviderFlags.WTD_REVOCATION_CHECK_CHAIN;
				winTrustRevocationChecks |= WinTrustRevocationChecks.WTD_REVOKE_WHOLECHAIN;
			}
			else
			{
				winTrustProviderFlags |= WinTrustProviderFlags.WTD_REVOCATION_CHECK_NONE;
			}
			WINTRUST_DATA* ptr = stackalloc WINTRUST_DATA[1];
			WINTRUST_FILE_INFO* pFile = stackalloc WINTRUST_FILE_INFO[1];
			ptr->cbStruct = (uint)Marshal.SizeOf<WINTRUST_DATA>();
			ptr->dwProvFlags = winTrustProviderFlags;
			ptr->dwStateAction = WinTrustStateAction.WTD_STATEACTION_IGNORE;
			ptr->dwUIChoice = WinTrustDataUIChoice.WTD_UI_NONE;
			ptr->dwUIContext = WinTrustUIContext.WTD_UICONTEXT_EXECUTE;
			ptr->dwUnionChoice = WinTrustUnionChoice.WTD_CHOICE_FILE;
			ptr->fdwRevocationChecks = winTrustRevocationChecks;
			ptr->trustUnion = new WINTRUST_DATA_UNION
			{
				pFile = pFile
			};
			ptr->trustUnion.pFile->cbStruct = (uint)Marshal.SizeOf<WINTRUST_FILE_INFO>();
			ptr->trustUnion.pFile->pcwszFilePath = intPtr;
			return Wintrust.WinVerifyTrustEx(new IntPtr(-1), KnownGuids.WINTRUST_ACTION_GENERIC_VERIFY_V2, ptr);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}
}
