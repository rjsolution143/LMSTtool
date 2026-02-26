using System.Collections.Generic;

namespace AuthenticodeExaminer;

internal static class SignatureExtensions
{
	internal static IEnumerable<ICmsSignature> VisitAll(this ICmsSignature signature, SignatureKind kind, bool deep)
	{
		foreach (ICmsSignature nested in signature.GetNestedSignatures())
		{
			if ((nested.Kind & kind) > (SignatureKind)0)
			{
				yield return nested;
				foreach (ICmsSignature item in nested.VisitAll(kind, deep))
				{
					yield return item;
				}
			}
			else
			{
				if (!deep)
				{
					continue;
				}
				foreach (ICmsSignature item2 in nested.VisitAll(kind, deep: true))
				{
					yield return item2;
				}
			}
		}
	}

	internal static IEnumerable<ICmsSignature> VisitAll(this IReadOnlyList<ICmsSignature> signatures, SignatureKind kind, bool deep)
	{
		foreach (ICmsSignature signature in signatures)
		{
			if ((signature.Kind & kind) > (SignatureKind)0)
			{
				yield return signature;
			}
			foreach (ICmsSignature item in signature.VisitAll(kind, deep))
			{
				yield return item;
			}
		}
	}
}
