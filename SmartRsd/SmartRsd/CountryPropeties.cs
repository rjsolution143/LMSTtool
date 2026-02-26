using System;

namespace SmartRsd;

public class CountryPropeties : IComparable
{
	public string CountryGroup { get; }

	public string RoCarrier { get; }

	public string DefaultLanguage { get; }

	public string CountryCode { get; }

	public string RsdCarrier { get; }

	public CountryPropeties(string countryGroup, string roCarrier, string language, string countryCode, string rsdCarrier)
	{
		CountryGroup = countryGroup;
		RoCarrier = ((string.Compare(roCarrier, "None", ignoreCase: true) == 0) ? string.Empty : roCarrier);
		DefaultLanguage = ((string.Compare(language, "None", ignoreCase: true) == 0) ? string.Empty : language);
		CountryCode = ((string.Compare(countryCode, "None", ignoreCase: true) == 0) ? string.Empty : countryCode);
		RsdCarrier = rsdCarrier;
	}

	public int CompareTo(object o)
	{
		CountryPropeties countryPropeties = o as CountryPropeties;
		string strA = CountryGroup + RoCarrier;
		string strB = countryPropeties.CountryGroup + countryPropeties.RoCarrier;
		return string.Compare(strA, strB);
	}
}
