using System.Collections.Generic;
using System.Linq;

namespace ISmart;

public struct TokenInfo
{
	private string TAG => GetType().FullName;

	public string HwDongleIp { get; private set; }

	public string PkiSource { get; private set; }

	public string PkiSourceType { get; private set; }

	public string BenchId { get; private set; }

	public static TokenInfo BlankInfo => new TokenInfo("UNKNOWN", "UNKNOWN", "UNKNOWN", "UNKNOWN");

	public TokenInfo(string hwDongleIp, string pkiSource, string pkiSourceType, string benchId)
	{
		this = default(TokenInfo);
		HwDongleIp = hwDongleIp;
		PkiSource = pkiSource;
		PkiSourceType = pkiSourceType;
		BenchId = benchId;
	}

	public override string ToString()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["HwDongleIp"] = HwDongleIp;
		dictionary["PkiSource"] = PkiSource;
		dictionary["PkiSourceType"] = PkiSourceType;
		dictionary["BenchId"] = BenchId;
		return Global.ToString(GetType().Name, dictionary.ToList());
	}

	public override bool Equals(object obj)
	{
		return ToString().Equals(obj.ToString());
	}

	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}
}
