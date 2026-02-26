using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using AuthenticodeExaminer.Interop;

namespace AuthenticodeExaminer;

public sealed class CmsSignature : CmsSignatureBase
{
	internal CmsSignature(SignatureKind kind, CryptMsgSafeHandle messageHandle, LocalBufferSafeHandle signerHandle, byte[]? content)
	{
		base.Content = content;
		base.Kind = kind;
		InitFromHandles(messageHandle, signerHandle);
	}

	internal unsafe CmsSignature(AsnEncodedData data, SignatureKind kind)
	{
		base.Kind = kind;
		fixed (byte* value = data.RawData)
		{
			CRYPTOAPI_BLOB pvObject = new CRYPTOAPI_BLOB
			{
				cbData = (uint)data.RawData.Length,
				pbData = new IntPtr(value)
			};
			if (!Crypt32.CryptQueryObject(CryptQueryObjectType.CERT_QUERY_OBJECT_BLOB, ref pvObject, CryptQueryContentFlagType.CERT_QUERY_CONTENT_FLAG_ALL, CryptQueryFormatFlagType.CERT_QUERY_FORMAT_FLAG_BINARY, CryptQueryObjectFlags.NONE, out EncodingType _, out CryptQueryContentType _, out CryptQueryFormatType _, IntPtr.Zero, out CryptMsgSafeHandle phMsg, IntPtr.Zero))
			{
				phMsg.Dispose();
				throw new InvalidOperationException("Unable to read signature.");
			}
			uint pcbData = 0u;
			if (Crypt32.CryptMsgGetParam(phMsg, CryptMsgParamType.CMSG_CONTENT_PARAM, 0u, LocalBufferSafeHandle.Zero, ref pcbData))
			{
				using LocalBufferSafeHandle localBufferSafeHandle = LocalBufferSafeHandle.Alloc(pcbData);
				if (Crypt32.CryptMsgGetParam(phMsg, CryptMsgParamType.CMSG_CONTENT_PARAM, 0u, localBufferSafeHandle, ref pcbData))
				{
					base.Content = new byte[pcbData];
					Marshal.Copy(localBufferSafeHandle.DangerousGetHandle(), base.Content, 0, (int)pcbData);
				}
			}
			uint pcbData2 = 0u;
			if (!Crypt32.CryptMsgGetParam(phMsg, CryptMsgParamType.CMSG_SIGNER_INFO_PARAM, 0u, LocalBufferSafeHandle.Zero, ref pcbData2))
			{
				throw new InvalidOperationException();
			}
			using LocalBufferSafeHandle localBufferSafeHandle2 = LocalBufferSafeHandle.Alloc(pcbData2);
			if (!Crypt32.CryptMsgGetParam(phMsg, CryptMsgParamType.CMSG_SIGNER_INFO_PARAM, 0u, localBufferSafeHandle2, ref pcbData2))
			{
				throw new InvalidOperationException();
			}
			InitFromHandles(phMsg, localBufferSafeHandle2);
		}
	}

	private void InitFromHandles(CryptMsgSafeHandle messageHandle, LocalBufferSafeHandle signerHandle)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Invalid comparison between Unknown and I4
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Invalid comparison between Unknown and I4
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		CMSG_SIGNER_INFO cMSG_SIGNER_INFO = Marshal.PtrToStructure<CMSG_SIGNER_INFO>(signerHandle.DangerousGetHandle());
		base.Signature = ReadBlob(cMSG_SIGNER_INFO.EncryptedHash);
		UniversalSubjectIdentifier universalSubjectIdentifier = new UniversalSubjectIdentifier(cMSG_SIGNER_INFO.Issuer, cMSG_SIGNER_INFO.SerialNumber);
		X509Certificate2Collection certificatesFromMessage = GetCertificatesFromMessage(messageHandle);
		if ((int)universalSubjectIdentifier.Type == 2)
		{
			base.Certificate = FindCertificate((string)universalSubjectIdentifier.Value, certificatesFromMessage);
		}
		else if ((int)universalSubjectIdentifier.Type == 1)
		{
			base.Certificate = FindCertificate((X509IssuerSerial)universalSubjectIdentifier.Value, certificatesFromMessage);
		}
		base.AdditionalCertificates = certificatesFromMessage;
		base.DigestAlgorithm = new Oid(cMSG_SIGNER_INFO.HashAlgorithm.pszObjId);
		base.HashEncryptionAlgorithm = new Oid(cMSG_SIGNER_INFO.HashEncryptionAlgorithm.pszObjId);
		base.SerialNumber = ReadBlob(cMSG_SIGNER_INFO.SerialNumber);
		base.UnsignedAttributes = ReadAttributes(cMSG_SIGNER_INFO.UnauthAttrs);
		base.SignedAttributes = ReadAttributes(cMSG_SIGNER_INFO.AuthAttrs);
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
					item = new AuthenticodeTimestampCmsSignature(current2, this);
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
