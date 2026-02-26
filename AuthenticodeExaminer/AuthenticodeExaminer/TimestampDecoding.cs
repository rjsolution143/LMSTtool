using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using AuthenticodeExaminer.Interop;

namespace AuthenticodeExaminer;

internal class TimestampDecoding
{
	public unsafe static DateTimeOffset? DecodeAuthenticodeTimestamp(AsnEncodedData data)
	{
		if (data.Oid.Value != "1.2.840.113549.1.9.5")
		{
			throw new ArgumentException("Data is not a signing time object.", "data");
		}
		LocalBufferSafeHandle pvStructInfo;
		fixed (byte* pbEncoded = data.RawData)
		{
			uint pcbStructInfo = 0u;
			if (!Crypt32.CryptDecodeObjectEx(EncodingType.PKCS_7_ASN_ENCODING | EncodingType.X509_ASN_ENCODING, "1.2.840.113549.1.9.5", pbEncoded, (uint)data.RawData.Length, CryptDecodeFlags.CRYPT_DECODE_ALLOC_FLAG, IntPtr.Zero, out pvStructInfo, ref pcbStructInfo))
			{
				throw new InvalidOperationException("Failed to decode data.");
			}
		}
		using (pvStructInfo)
		{
			FILETIME fILETIME = Marshal.PtrToStructure<FILETIME>(pvStructInfo.DangerousGetHandle());
			return DateTimeOffset.FromFileTime(((long)fILETIME.dwHighDateTime << 32) | (uint)fILETIME.dwLowDateTime);
		}
	}

	public unsafe static DateTimeOffset? DecodeRfc3161(byte[] content)
	{
		fixed (byte* value = content)
		{
			uint pcbStructInfo = 0u;
			if (!Crypt32.CryptDecodeObjectEx(EncodingType.PKCS_7_ASN_ENCODING | EncodingType.X509_ASN_ENCODING, (IntPtr)34, new IntPtr(value), (uint)content.Length, CryptDecodeFlags.CRYPT_DECODE_ALLOC_FLAG, IntPtr.Zero, out LocalBufferSafeHandle pvStructInfo, ref pcbStructInfo))
			{
				return null;
			}
			using (pvStructInfo)
			{
				CRYPT_SEQUENCE_OF_ANY cRYPT_SEQUENCE_OF_ANY = Marshal.PtrToStructure<CRYPT_SEQUENCE_OF_ANY>(pvStructInfo.DangerousGetHandle());
				if (cRYPT_SEQUENCE_OF_ANY.cValue < 5)
				{
					return null;
				}
				CRYPTOAPI_BLOB cRYPTOAPI_BLOB = cRYPT_SEQUENCE_OF_ANY.rgValue[4];
				uint pcbStructInfo2 = 0u;
				if (!Crypt32.CryptDecodeObjectEx(EncodingType.PKCS_7_ASN_ENCODING | EncodingType.X509_ASN_ENCODING, (IntPtr)30, cRYPTOAPI_BLOB.pbData, cRYPTOAPI_BLOB.cbData, CryptDecodeFlags.CRYPT_DECODE_ALLOC_FLAG, IntPtr.Zero, out LocalBufferSafeHandle pvStructInfo2, ref pcbStructInfo2))
				{
					return null;
				}
				using (pvStructInfo2)
				{
					FILETIME fILETIME = Marshal.PtrToStructure<FILETIME>(pvStructInfo2.DangerousGetHandle());
					return DateTimeOffset.FromFileTime(((long)fILETIME.dwHighDateTime << 32) | (uint)fILETIME.dwLowDateTime);
				}
			}
		}
	}
}
