using System;

namespace SmartRsd;

public class CountrySimLockPropeties : IComparable
{
	public string Carrier { get; }

	public string SimLockFileName { get; }

	public string CountryCode { get; }

	public string RoCarrier { get; }

	public string ProductName { get; }

	public string RsdCarrier { get; }

	public string ElabelFileName { get; }

	public CountrySimLockPropeties(string carrier, string simLock, string countryCode, string roCarrier, string productName, string rsdCarrier, string eLabelFile = "")
	{
		Carrier = carrier;
		SimLockFileName = ((string.Compare(simLock, "None", ignoreCase: true) == 0) ? string.Empty : simLock);
		CountryCode = ((string.Compare(countryCode, "None", ignoreCase: true) == 0) ? string.Empty : countryCode);
		RoCarrier = roCarrier;
		ProductName = productName;
		RsdCarrier = rsdCarrier;
		ElabelFileName = eLabelFile;
	}

	public int CompareTo(object o)
	{
		CountrySimLockPropeties countrySimLockPropeties = o as CountrySimLockPropeties;
		return string.Compare(Carrier, countrySimLockPropeties.Carrier);
	}
}
