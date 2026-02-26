using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace AuthenticodeExaminer;

public abstract class TimestampSignature
{
	internal class AuthenticodeTimestampSignature : TimestampSignature
	{
		public AuthenticodeTimestampSignature(AuthenticodeTimestampCmsSignature authenticodeCmsSignature)
			: base(authenticodeCmsSignature)
		{
			foreach (CryptographicAttributeObject signedAttribute in authenticodeCmsSignature.SignedAttributes)
			{
				if (!(signedAttribute.Oid.Value != "1.2.840.113549.1.9.5") && signedAttribute.Values.Count > 0)
				{
					base.TimestampDateTime = TimestampDecoding.DecodeAuthenticodeTimestamp(signedAttribute.Values[0]);
					break;
				}
			}
		}
	}

	internal class RFC3161TimestampSignature : TimestampSignature
	{
		public RFC3161TimestampSignature(CmsSignature rfc3161Signature)
			: base(rfc3161Signature)
		{
			byte[] content = rfc3161Signature.Content;
			if (content != null)
			{
				base.TimestampDateTime = TimestampDecoding.DecodeRfc3161(content);
			}
		}
	}

	private readonly ICmsSignature _cmsSignature;

	public X509Certificate2? SigningCertificate => _cmsSignature.Certificate;

	public X509Certificate2Collection AdditionalCertificates => _cmsSignature.AdditionalCertificates;

	public byte[]? Contents => _cmsSignature.Content;

	public HashAlgorithmName DigestAlgorithmName => _cmsSignature.DigestAlgorithmName;

	public DateTimeOffset? TimestampDateTime { get; protected set; }

	public ReadOnlyMemory<byte> Signature => _cmsSignature.Signature;

	private protected TimestampSignature(ICmsSignature cmsSignature)
	{
		_cmsSignature = cmsSignature;
	}
}
