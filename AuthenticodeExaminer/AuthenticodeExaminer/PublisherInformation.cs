using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using AuthenticodeExaminer.Interop;

namespace AuthenticodeExaminer;

public class PublisherInformation
{
	public string? Description { get; }

	public string? UrlLink { get; }

	public string? FileLink { get; }

	public bool IsEmpty
	{
		get
		{
			if (string.IsNullOrWhiteSpace(Description) && string.IsNullOrWhiteSpace(UrlLink))
			{
				return string.IsNullOrWhiteSpace(FileLink);
			}
			return false;
		}
	}

	public PublisherInformation()
	{
	}

	public unsafe PublisherInformation(AsnEncodedData data)
	{
		if (data.Oid.Value != "1.3.6.1.4.1.311.2.1.12")
		{
			throw new ArgumentException("Data is not a publisher information object.", "data");
		}
		LocalBufferSafeHandle pvStructInfo;
		fixed (byte* pbEncoded = data.RawData)
		{
			uint pcbStructInfo = 0u;
			if (!Crypt32.CryptDecodeObjectEx(EncodingType.PKCS_7_ASN_ENCODING | EncodingType.X509_ASN_ENCODING, "1.3.6.1.4.1.311.2.1.12", pbEncoded, (uint)data.RawData.Length, CryptDecodeFlags.CRYPT_DECODE_ALLOC_FLAG, IntPtr.Zero, out pvStructInfo, ref pcbStructInfo))
			{
				throw new InvalidOperationException("Failed to decode data.");
			}
		}
		using (pvStructInfo)
		{
			SPC_SP_OPUS_INFO sPC_SP_OPUS_INFO = Marshal.PtrToStructure<SPC_SP_OPUS_INFO>(pvStructInfo.DangerousGetHandle());
			Description = sPC_SP_OPUS_INFO.pwszProgramName?.Trim();
			if (sPC_SP_OPUS_INFO.pMoreInfo != null)
			{
				SPC_LINK* pMoreInfo = sPC_SP_OPUS_INFO.pMoreInfo;
				switch (pMoreInfo->dwLinkChoice)
				{
				case SpcLinkChoice.SPC_URL_LINK_CHOICE:
					UrlLink = Marshal.PtrToStringUni(pMoreInfo->linkUnion.pwszUrl)?.Trim();
					break;
				case SpcLinkChoice.SPC_FILE_LINK_CHOICE:
					FileLink = Marshal.PtrToStringUni(pMoreInfo->linkUnion.pwszFile)?.Trim();
					break;
				}
			}
		}
	}
}
