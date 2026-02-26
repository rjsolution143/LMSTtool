using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AuthenticodeExaminer.Interop;

internal static class Crypt32
{
	[DllImport("crypt32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool CryptQueryObject([In][MarshalAs(UnmanagedType.U4)] CryptQueryObjectType dwObjectType, [In][MarshalAs(UnmanagedType.LPWStr)] string pvObject, [In][MarshalAs(UnmanagedType.U4)] CryptQueryContentFlagType dwExpectedContentTypeFlags, [In][MarshalAs(UnmanagedType.U4)] CryptQueryFormatFlagType dwExpectedFormatTypeFlags, [In][MarshalAs(UnmanagedType.U4)] CryptQueryObjectFlags dwFlags, [MarshalAs(UnmanagedType.U4)] out EncodingType pdwMsgAndCertEncodingType, [MarshalAs(UnmanagedType.U4)] out CryptQueryContentType pdwContentType, [MarshalAs(UnmanagedType.U4)] out CryptQueryFormatType pdwFormatType, [In][MarshalAs(UnmanagedType.SysInt)] IntPtr phCertStore, out CryptMsgSafeHandle phMsg, [In][MarshalAs(UnmanagedType.SysInt)] IntPtr ppvContext);

	[DllImport("crypt32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool CryptQueryObject([In][MarshalAs(UnmanagedType.U4)] CryptQueryObjectType dwObjectType, [In][Out][MarshalAs(UnmanagedType.Struct)] ref CRYPTOAPI_BLOB pvObject, [In][MarshalAs(UnmanagedType.U4)] CryptQueryContentFlagType dwExpectedContentTypeFlags, [In][MarshalAs(UnmanagedType.U4)] CryptQueryFormatFlagType dwExpectedFormatTypeFlags, [In][MarshalAs(UnmanagedType.U4)] CryptQueryObjectFlags dwFlags, [MarshalAs(UnmanagedType.U4)] out EncodingType pdwMsgAndCertEncodingType, [MarshalAs(UnmanagedType.U4)] out CryptQueryContentType pdwContentType, [MarshalAs(UnmanagedType.U4)] out CryptQueryFormatType pdwFormatType, [In][MarshalAs(UnmanagedType.SysInt)] IntPtr phCertStore, out CryptMsgSafeHandle phMsg, [In][MarshalAs(UnmanagedType.SysInt)] IntPtr ppvContext);

	[DllImport("crypt32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool CryptDecodeObjectEx([In][MarshalAs(UnmanagedType.U4)] EncodingType dwCertEncodingType, [In][MarshalAs(UnmanagedType.SysInt)] IntPtr lpszStructType, [In][MarshalAs(UnmanagedType.SysInt)] IntPtr pbEncoded, [In][MarshalAs(UnmanagedType.U4)] uint cbEncoded, [In][MarshalAs(UnmanagedType.U4)] CryptDecodeFlags dwFlags, [In][MarshalAs(UnmanagedType.SysInt)] IntPtr pDecodePara, out LocalBufferSafeHandle pvStructInfo, [In][Out][MarshalAs(UnmanagedType.U4)] ref uint pcbStructInfo);

	[DllImport("crypt32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public unsafe static extern bool CryptDecodeObjectEx([In][MarshalAs(UnmanagedType.U4)] EncodingType dwCertEncodingType, [In][MarshalAs(UnmanagedType.LPStr)] string lpszStructType, [In] void* pbEncoded, [In][MarshalAs(UnmanagedType.U4)] uint cbEncoded, [In][MarshalAs(UnmanagedType.U4)] CryptDecodeFlags dwFlags, [In][MarshalAs(UnmanagedType.SysInt)] IntPtr pDecodePara, out LocalBufferSafeHandle pvStructInfo, [In][Out][MarshalAs(UnmanagedType.U4)] ref uint pcbStructInfo);

	[DllImport("crypt32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool CryptMsgClose([In][MarshalAs(UnmanagedType.SysInt)] IntPtr hCryptMsg);

	[DllImport("crypt32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool CertCloseStore([In][MarshalAs(UnmanagedType.SysInt)] IntPtr hCertStore, [In][MarshalAs(UnmanagedType.U4)] uint dwFlags);

	[DllImport("crypt32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool CryptMsgGetParam([In] CryptMsgSafeHandle hCryptMsg, [In][MarshalAs(UnmanagedType.U4)] CryptMsgParamType dwParamType, [In][MarshalAs(UnmanagedType.U4)] uint dwIndex, [In] LocalBufferSafeHandle pvData, [In][Out][MarshalAs(UnmanagedType.U4)] ref uint pcbData);

	[DllImport("crypt32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool CryptBinaryToString([In] byte[] pbBinary, [In][MarshalAs(UnmanagedType.U4)] uint cbBinary, [In][MarshalAs(UnmanagedType.U4)] CryptBinaryToStringFlags dwFlags, [In][Out] StringBuilder pszString, [In][Out] ref uint pcchString);

	[DllImport("crypt32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.U4)]
	public static extern uint CertNameToStr([In][MarshalAs(UnmanagedType.U4)] EncodingType dwCertEncodingType, [In][MarshalAs(UnmanagedType.SysInt)] IntPtr pName, [In][MarshalAs(UnmanagedType.U4)] CertNameStrType dwStrType, [In][Out] StringBuilder? psz, [In] uint csz);
}
