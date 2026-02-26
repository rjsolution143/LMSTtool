using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using AuthenticodeExaminer.Interop;

namespace AuthenticodeExaminer;

public abstract class CmsSignatureBase : ICmsSignature
{
	public Oid? DigestAlgorithm { get; protected set; }

	public Oid? HashEncryptionAlgorithm { get; protected set; }

	public IReadOnlyList<CryptographicAttributeObject> UnsignedAttributes { get; protected set; } = Array.Empty<CryptographicAttributeObject>();


	public IReadOnlyList<CryptographicAttributeObject> SignedAttributes { get; protected set; } = Array.Empty<CryptographicAttributeObject>();


	public byte[] SerialNumber { get; protected set; } = Array.Empty<byte>();


	public X509Certificate2? Certificate { get; protected set; }

	public SignatureKind Kind { get; protected set; }

	public X509Certificate2Collection AdditionalCertificates { get; protected set; } = new X509Certificate2Collection();


	public byte[]? Content { get; protected set; }

	public ReadOnlyMemory<byte> Signature { get; protected set; }

	public HashAlgorithmName DigestAlgorithmName => DigestAlgorithm?.Value switch
	{
		"1.2.840.113549.2.5" => HashAlgorithmName.MD5, 
		"1.3.14.3.2.26" => HashAlgorithmName.SHA1, 
		"2.16.840.1.101.3.4.2.1" => HashAlgorithmName.SHA256, 
		"2.16.840.1.101.3.4.2.2" => HashAlgorithmName.SHA384, 
		"2.16.840.1.101.3.4.2.3" => HashAlgorithmName.SHA512, 
		_ => default(HashAlgorithmName), 
	};

	internal byte[] ReadBlob(CRYPTOAPI_BLOB blob)
	{
		return blob.AsSpan().ToArray();
	}

	internal List<CryptographicAttributeObject> ReadAttributes(CRYPT_ATTRIBUTES attributes)
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		List<CryptographicAttributeObject> list = new List<CryptographicAttributeObject>();
		int num = Marshal.SizeOf<CRYPT_ATTRIBUTE>();
		int num2 = Marshal.SizeOf<CRYPTOAPI_BLOB>();
		for (int i = 0; i < attributes.cAttr; i++)
		{
			CRYPT_ATTRIBUTE cRYPT_ATTRIBUTE = Marshal.PtrToStructure<CRYPT_ATTRIBUTE>(attributes.rgAttr + i * num);
			AsnEncodedDataCollection asnEncodedDataCollection = new AsnEncodedDataCollection();
			for (int j = 0; j < cRYPT_ATTRIBUTE.cValue; j++)
			{
				CRYPTOAPI_BLOB blob = Marshal.PtrToStructure<CRYPTOAPI_BLOB>(cRYPT_ATTRIBUTE.rgValue + j * num2);
				asnEncodedDataCollection.Add(new AsnEncodedData(cRYPT_ATTRIBUTE.pszObjId, ReadBlob(blob)));
			}
			list.Add(new CryptographicAttributeObject(new Oid(cRYPT_ATTRIBUTE.pszObjId), asnEncodedDataCollection));
		}
		return list;
	}

	private protected X509Certificate2? FindCertificate(X509IssuerSerial issuerSerial, X509Certificate2Collection certificateCollection)
	{
		X509Certificate2Collection x509Certificate2Collection = certificateCollection.Find(X509FindType.FindByIssuerDistinguishedName, ((X509IssuerSerial)(ref issuerSerial)).IssuerName, validOnly: false);
		if (x509Certificate2Collection.Count < 1)
		{
			return null;
		}
		X509Certificate2Collection x509Certificate2Collection2 = x509Certificate2Collection.Find(X509FindType.FindBySerialNumber, ((X509IssuerSerial)(ref issuerSerial)).SerialNumber, validOnly: false);
		if (x509Certificate2Collection2.Count != 1)
		{
			return null;
		}
		return x509Certificate2Collection2[0];
	}

	private protected X509Certificate2? FindCertificate(string keyId, X509Certificate2Collection certificateCollection)
	{
		X509Certificate2Collection x509Certificate2Collection = certificateCollection.Find(X509FindType.FindBySubjectKeyIdentifier, keyId, validOnly: false);
		if (x509Certificate2Collection.Count != 1)
		{
			return null;
		}
		return x509Certificate2Collection[0];
	}

	private protected X509Certificate2Collection GetCertificatesFromMessage(CryptMsgSafeHandle handle)
	{
		uint pcbData = (uint)Marshal.SizeOf<uint>();
		X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
		uint num;
		using (LocalBufferSafeHandle localBufferSafeHandle = LocalBufferSafeHandle.Alloc(pcbData))
		{
			if (!Crypt32.CryptMsgGetParam(handle, CryptMsgParamType.CMSG_CERT_COUNT_PARAM, 0u, localBufferSafeHandle, ref pcbData))
			{
				return x509Certificate2Collection;
			}
			num = (uint)Marshal.ReadInt32(localBufferSafeHandle.DangerousGetHandle(), 0);
		}
		if (num == 0)
		{
			return x509Certificate2Collection;
		}
		for (uint num2 = 0u; num2 < num; num2++)
		{
			uint pcbData2 = 0u;
			if (!Crypt32.CryptMsgGetParam(handle, CryptMsgParamType.CMSG_CERT_PARAM, num2, LocalBufferSafeHandle.Zero, ref pcbData2))
			{
				continue;
			}
			using LocalBufferSafeHandle localBufferSafeHandle2 = LocalBufferSafeHandle.Alloc(pcbData2);
			if (Crypt32.CryptMsgGetParam(handle, CryptMsgParamType.CMSG_CERT_PARAM, num2, localBufferSafeHandle2, ref pcbData2))
			{
				byte[] array = new byte[pcbData2];
				Marshal.Copy(localBufferSafeHandle2.DangerousGetHandle(), array, 0, array.Length);
				X509Certificate2 certificate = new X509Certificate2(array);
				x509Certificate2Collection.Add(certificate);
			}
		}
		return x509Certificate2Collection;
	}

	public abstract IReadOnlyList<ICmsSignature> GetNestedSignatures();
}
