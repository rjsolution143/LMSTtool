using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.Xml;
using System.Text;
using AuthenticodeExaminer.Interop;

namespace AuthenticodeExaminer;

internal class UniversalSubjectIdentifier
{
	public SubjectIdentifierType Type { get; }

	public object Value { get; }

	public unsafe UniversalSubjectIdentifier(CRYPTOAPI_BLOB issuer, CRYPTOAPI_BLOB serialNumber)
	{
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (IsBlobAllZero(serialNumber))
		{
			uint pcbStructInfo = 0u;
			if (Crypt32.CryptDecodeObjectEx(EncodingType.PKCS_7_ASN_ENCODING | EncodingType.X509_ASN_ENCODING, (IntPtr)7, issuer.pbData, issuer.cbData, CryptDecodeFlags.CRYPT_DECODE_ALLOC_FLAG, IntPtr.Zero, out LocalBufferSafeHandle pvStructInfo, ref pcbStructInfo))
			{
				using (pvStructInfo)
				{
					CERT_NAME_INFO cERT_NAME_INFO = Marshal.PtrToStructure<CERT_NAME_INFO>(pvStructInfo.DangerousGetHandle());
					for (long num = 0L; num < cERT_NAME_INFO.cRDN; num++)
					{
						CERT_RDN cERT_RDN = Marshal.PtrToStructure<CERT_RDN>(new IntPtr(cERT_NAME_INFO.rgRDN.ToInt64() + num * Marshal.SizeOf<CERT_RDN>()));
						for (int i = 0; i < cERT_RDN.cRDNAttr; i++)
						{
							CERT_RDN_ATTR cERT_RDN_ATTR = Marshal.PtrToStructure<CERT_RDN_ATTR>(new IntPtr(cERT_RDN.rgRDNAttr.ToInt64() + i * Marshal.SizeOf<CERT_RDN_ATTR>()));
							if (cERT_RDN_ATTR.pszObjId == "1.3.6.1.4.1.311.10.7.1")
							{
								Type = (SubjectIdentifierType)2;
								ReadOnlySpan<byte> data = cERT_RDN_ATTR.Value.AsSpan();
								Value = HexHelpers.HexEncodeBigEndian(data);
								return;
							}
						}
					}
				}
			}
		}
		uint num2 = Crypt32.CertNameToStr(EncodingType.PKCS_7_ASN_ENCODING | EncodingType.X509_ASN_ENCODING, new IntPtr(&issuer), CertNameStrType.CERT_X500_NAME_STR | CertNameStrType.CERT_NAME_STR_REVERSE_FLAG, null, 0u);
		if (num2 <= 1)
		{
			throw new InvalidOperationException();
		}
		StringBuilder stringBuilder = new StringBuilder((int)num2);
		if (Crypt32.CertNameToStr(EncodingType.PKCS_7_ASN_ENCODING | EncodingType.X509_ASN_ENCODING, new IntPtr(&issuer), CertNameStrType.CERT_X500_NAME_STR | CertNameStrType.CERT_NAME_STR_REVERSE_FLAG, stringBuilder, num2) <= 1)
		{
			throw new InvalidOperationException();
		}
		ReadOnlySpan<byte> data2 = serialNumber.AsSpan();
		X509IssuerSerial val = default(X509IssuerSerial);
		((X509IssuerSerial)(ref val)).IssuerName = stringBuilder.ToString();
		((X509IssuerSerial)(ref val)).SerialNumber = HexHelpers.HexEncodeBigEndian(data2);
		X509IssuerSerial val2 = val;
		Value = val2;
		Type = (SubjectIdentifierType)1;
	}

	private static bool IsBlobAllZero(CRYPTOAPI_BLOB blob)
	{
		ReadOnlySpan<byte> readOnlySpan = blob.AsSpan();
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			if (readOnlySpan[i] != 0)
			{
				return false;
			}
		}
		return true;
	}
}
