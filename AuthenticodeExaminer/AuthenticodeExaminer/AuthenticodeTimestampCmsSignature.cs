using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using AuthenticodeExaminer.Interop;

namespace AuthenticodeExaminer;

public sealed class AuthenticodeTimestampCmsSignature : CmsSignatureBase
{
	public ICmsSignature OwningSignature { get; }

	internal unsafe AuthenticodeTimestampCmsSignature(AsnEncodedData data, ICmsSignature owningSignature)
	{
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Invalid comparison between Unknown and I4
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Invalid comparison between Unknown and I4
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		OwningSignature = owningSignature;
		base.Kind = SignatureKind.AuthenticodeTimestamp;
		base.AdditionalCertificates = owningSignature.AdditionalCertificates;
		fixed (byte* value = data.RawData)
		{
			uint pcbStructInfo = 0u;
			if (Crypt32.CryptDecodeObjectEx(EncodingType.PKCS_7_ASN_ENCODING | EncodingType.X509_ASN_ENCODING, (IntPtr)500, new IntPtr(value), (uint)data.RawData.Length, CryptDecodeFlags.CRYPT_DECODE_ALLOC_FLAG, IntPtr.Zero, out LocalBufferSafeHandle pvStructInfo, ref pcbStructInfo))
			{
				using (pvStructInfo)
				{
					CMSG_SIGNER_INFO cMSG_SIGNER_INFO = Marshal.PtrToStructure<CMSG_SIGNER_INFO>(pvStructInfo.DangerousGetHandle());
					base.Signature = ReadBlob(cMSG_SIGNER_INFO.EncryptedHash);
					base.DigestAlgorithm = new Oid(cMSG_SIGNER_INFO.HashAlgorithm.pszObjId);
					base.HashEncryptionAlgorithm = new Oid(cMSG_SIGNER_INFO.HashEncryptionAlgorithm.pszObjId);
					base.SerialNumber = ReadBlob(cMSG_SIGNER_INFO.SerialNumber);
					base.UnsignedAttributes = ReadAttributes(cMSG_SIGNER_INFO.UnauthAttrs);
					base.SignedAttributes = ReadAttributes(cMSG_SIGNER_INFO.AuthAttrs);
					UniversalSubjectIdentifier universalSubjectIdentifier = new UniversalSubjectIdentifier(cMSG_SIGNER_INFO.Issuer, cMSG_SIGNER_INFO.SerialNumber);
					if ((int)universalSubjectIdentifier.Type == 2)
					{
						base.Certificate = FindCertificate((string)universalSubjectIdentifier.Value, OwningSignature.AdditionalCertificates);
					}
					else if ((int)universalSubjectIdentifier.Type == 1)
					{
						base.Certificate = FindCertificate((X509IssuerSerial)universalSubjectIdentifier.Value, OwningSignature.AdditionalCertificates);
					}
				}
				return;
			}
			throw new InvalidOperationException("Failed to read Authenticode signature");
		}
	}

	public override IReadOnlyList<ICmsSignature> GetNestedSignatures()
	{
		List<ICmsSignature> list = new List<ICmsSignature>();
		foreach (CryptographicAttributeObject unsignedAttribute in base.UnsignedAttributes)
		{
			AsnEncodedDataEnumerator enumerator2 = unsignedAttribute.Values.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				AsnEncodedData current2 = enumerator2.Current;
				ICmsSignature item;
				switch (unsignedAttribute.Oid.Value)
				{
				case "1.2.840.113549.1.9.6":
					item = new AuthenticodeTimestampCmsSignature(current2, OwningSignature);
					break;
				case "1.3.6.1.4.1.311.3.3.1":
					item = new CmsSignature(current2, SignatureKind.Rfc3161Timestamp);
					break;
				case "1.3.6.1.4.1.311.2.4.1":
					item = new CmsSignature(current2, SignatureKind.NestedSignature);
					break;
				default:
					continue;
				}
				list.Add(item);
			}
		}
		return list.AsReadOnly();
	}
}
