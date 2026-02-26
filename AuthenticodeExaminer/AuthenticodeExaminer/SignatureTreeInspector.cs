using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using AuthenticodeExaminer.Interop;

namespace AuthenticodeExaminer;

public static class SignatureTreeInspector
{
	public static IReadOnlyList<ICmsSignature> Extract(string filePath)
	{
		if (!Crypt32.CryptQueryObject(CryptQueryObjectType.CERT_QUERY_OBJECT_FILE, filePath, CryptQueryContentFlagType.CERT_QUERY_CONTENT_FLAG_PKCS7_SIGNED_EMBED, CryptQueryFormatFlagType.CERT_QUERY_FORMAT_FLAG_BINARY, CryptQueryObjectFlags.NONE, out EncodingType _, out CryptQueryContentType _, out CryptQueryFormatType _, IntPtr.Zero, out CryptMsgSafeHandle phMsg, IntPtr.Zero))
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error == -2146885623)
			{
				return Array.Empty<ICmsSignature>();
			}
			throw new Win32Exception(lastWin32Error, "Failed to extract signature.");
		}
		using (phMsg)
		{
			if (phMsg.IsInvalid || phMsg.IsClosed)
			{
				return Array.Empty<ICmsSignature>();
			}
			return GetSignatures(phMsg);
		}
	}

	private static IReadOnlyList<ICmsSignature> GetSignatures(CryptMsgSafeHandle messageHandle)
	{
		uint pcbData = 0u;
		if (!Crypt32.CryptMsgGetParam(messageHandle, CryptMsgParamType.CMSG_SIGNER_COUNT_PARAM, 0u, LocalBufferSafeHandle.Zero, ref pcbData))
		{
			return Array.Empty<ICmsSignature>();
		}
		uint num;
		using (LocalBufferSafeHandle localBufferSafeHandle = LocalBufferSafeHandle.Alloc(pcbData))
		{
			if (!Crypt32.CryptMsgGetParam(messageHandle, CryptMsgParamType.CMSG_SIGNER_COUNT_PARAM, 0u, localBufferSafeHandle, ref pcbData))
			{
				return Array.Empty<ICmsSignature>();
			}
			num = (uint)Marshal.ReadInt32(localBufferSafeHandle.DangerousGetHandle());
		}
		List<ICmsSignature> list = new List<ICmsSignature>();
		uint pcbData2 = 0u;
		byte[] array = null;
		if (Crypt32.CryptMsgGetParam(messageHandle, CryptMsgParamType.CMSG_CONTENT_PARAM, 0u, LocalBufferSafeHandle.Zero, ref pcbData2))
		{
			using LocalBufferSafeHandle localBufferSafeHandle2 = LocalBufferSafeHandle.Alloc(pcbData2);
			if (Crypt32.CryptMsgGetParam(messageHandle, CryptMsgParamType.CMSG_CONTENT_PARAM, 0u, localBufferSafeHandle2, ref pcbData2))
			{
				array = new byte[pcbData2];
				Marshal.Copy(localBufferSafeHandle2.DangerousGetHandle(), array, 0, (int)pcbData2);
			}
		}
		for (uint num2 = 0u; num2 < num; num2++)
		{
			uint pcbData3 = 0u;
			if (!Crypt32.CryptMsgGetParam(messageHandle, CryptMsgParamType.CMSG_SIGNER_INFO_PARAM, num2, LocalBufferSafeHandle.Zero, ref pcbData3))
			{
				continue;
			}
			using LocalBufferSafeHandle localBufferSafeHandle3 = LocalBufferSafeHandle.Alloc(pcbData3);
			if (Crypt32.CryptMsgGetParam(messageHandle, CryptMsgParamType.CMSG_SIGNER_INFO_PARAM, num2, localBufferSafeHandle3, ref pcbData3))
			{
				CmsSignature item = new CmsSignature(SignatureKind.Signature, messageHandle, localBufferSafeHandle3, array);
				list.Add(item);
			}
		}
		return list.AsReadOnly();
	}
}
