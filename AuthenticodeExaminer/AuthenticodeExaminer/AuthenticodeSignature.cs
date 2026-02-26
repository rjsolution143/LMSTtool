using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace AuthenticodeExaminer;

public sealed class AuthenticodeSignature
{
	private readonly ICmsSignature _cmsSignature;

	private IReadOnlyList<TimestampSignature>? _timestampSignatures;

	private PublisherInformation? _publisherInformation;

	public X509Certificate2? SigningCertificate => _cmsSignature.Certificate;

	public X509Certificate2Collection AdditionalCertificates => _cmsSignature.AdditionalCertificates;

	public byte[]? Contents => _cmsSignature.Content;

	public HashAlgorithmName DigestAlgorithmName => _cmsSignature.DigestAlgorithmName;

	public ReadOnlyMemory<byte> Signature => _cmsSignature.Signature;

	public IReadOnlyList<TimestampSignature> TimestampSignatures
	{
		get
		{
			if (_timestampSignatures != null)
			{
				return _timestampSignatures;
			}
			List<TimestampSignature> list = new List<TimestampSignature>();
			foreach (ICmsSignature item in _cmsSignature.VisitAll(SignatureKind.AnyCounterSignature, deep: false))
			{
				if (!(item is AuthenticodeTimestampCmsSignature authenticodeCmsSignature))
				{
					if (item is CmsSignature { Kind: SignatureKind.Rfc3161Timestamp } cmsSignature)
					{
						list.Add(new TimestampSignature.RFC3161TimestampSignature(cmsSignature));
					}
				}
				else
				{
					list.Add(new TimestampSignature.AuthenticodeTimestampSignature(authenticodeCmsSignature));
				}
			}
			Interlocked.CompareExchange(ref _timestampSignatures, list, null);
			return _timestampSignatures;
		}
	}

	public PublisherInformation PublisherInformation
	{
		get
		{
			if (_publisherInformation != null)
			{
				return _publisherInformation;
			}
			PublisherInformation publisherInformation = null;
			foreach (CryptographicAttributeObject signedAttribute in _cmsSignature.SignedAttributes)
			{
				if (!(signedAttribute.Oid.Value != "1.3.6.1.4.1.311.2.1.12") && signedAttribute.Values.Count > 0)
				{
					publisherInformation = new PublisherInformation(signedAttribute.Values[0]);
					break;
				}
			}
			Interlocked.CompareExchange(ref _publisherInformation, publisherInformation ?? new PublisherInformation(), null);
			return _publisherInformation;
		}
	}

	internal AuthenticodeSignature(ICmsSignature cmsSignature)
	{
		if ((cmsSignature.Kind & SignatureKind.AnySignature) == 0)
		{
			throw new ArgumentException("The signature must be a root or nested signature.", "cmsSignature");
		}
		_cmsSignature = cmsSignature;
	}
}
