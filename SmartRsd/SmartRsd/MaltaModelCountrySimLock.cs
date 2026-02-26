using System.Collections.Generic;

namespace SmartRsd;

public class MaltaModelCountrySimLock : ModelPropertyBase<CountrySimLockPropeties>
{
	private static Dictionary<int, string> sFormats = new Dictionary<int, string>
	{
		{ 0, "TARGET" },
		{ 2, "SVNKITCarrier" },
		{ 5, "ModelNumber(HWSKU)" }
	};

	private const int cSkipEmptyOn = 2;

	private int mIndex2 = 2;

	private int mIndex5 = 5;

	private int mIndex10 = 10;

	private int mIndex11 = 11;

	private int mIndex16 = 16;

	private int mIndex17 = 17;

	private int mIndex18 = 18;

	private int mIndex21 = 21;

	private int mIndex24 = 24;

	private int mSkipEmptyOn = 2;

	private static MaltaModelCountrySimLock sThis = null;

	private static string sCsvFilePathName = string.Empty;

	private string TAG => GetType().FullName;

	public static bool FormatMatched(string csvFilePathName)
	{
		return ModelPropertyBase<CountrySimLockPropeties>.FormatMatched(csvFilePathName, 2, sFormats);
	}

	public static MaltaModelCountrySimLock Instance(string csvFilePathName)
	{
		if (csvFilePathName != sCsvFilePathName || sThis == null)
		{
			sThis = new MaltaModelCountrySimLock(csvFilePathName);
		}
		return sThis;
	}

	private MaltaModelCountrySimLock(string csvFilePathName)
		: base(csvFilePathName)
	{
		sCsvFilePathName = csvFilePathName;
	}

	protected override void AddProperty(List<string> fields)
	{
		string text = fields[0].Replace(" ", string.Empty).ToLower();
		if (text == "target" || text == "rsdcarrier")
		{
			if (ModelPropertyBase<CountrySimLockPropeties>.FindFieldName(fields, "rsdcarrier", out mRsdCarrierIndex))
			{
				mOffset = 1;
				mIndex2 = 2 + ((2 >= mRsdCarrierIndex) ? mOffset : 0);
				mIndex5 = 5 + ((5 >= mRsdCarrierIndex) ? mOffset : 0);
				mIndex10 = 10 + ((10 >= mRsdCarrierIndex) ? mOffset : 0);
				mIndex11 = 11 + ((11 >= mRsdCarrierIndex) ? mOffset : 0);
				mIndex16 = 16 + ((16 >= mRsdCarrierIndex) ? mOffset : 0);
				mIndex17 = 17 + ((17 >= mRsdCarrierIndex) ? mOffset : 0);
				mIndex18 = 18 + ((18 >= mRsdCarrierIndex) ? mOffset : 0);
				mIndex21 = 21 + ((21 >= mRsdCarrierIndex) ? mOffset : 0);
				mIndex24 = 24 + ((24 >= mRsdCarrierIndex) ? mOffset : 0);
				mSkipEmptyOn = 2 + ((2 >= mRsdCarrierIndex) ? mOffset : 0);
			}
		}
		else
		{
			if (!(fields[mSkipEmptyOn].Trim() != string.Empty))
			{
				return;
			}
			string text2 = fields[mIndex5].Trim();
			string text3 = fields[mIndex21].Trim();
			if (text3 != string.Empty)
			{
				text2 = text2 + "-" + text3;
			}
			string carrier = fields[mIndex2].Trim() + " " + fields[mIndex11].Trim();
			string simLock = (fields[mIndex16].ToUpper().StartsWith("Y") ? fields[mIndex17].Trim() : string.Empty);
			string countryCode = fields[mIndex11].Trim();
			string roCarrier = fields[mIndex10].Trim();
			string eLabelFile = fields[mIndex18].Trim();
			string productName = fields[mIndex24].Trim();
			string rsdCarrier = ((mOffset > 0) ? fields[mRsdCarrierIndex].Trim() : string.Empty);
			if (text2 != string.Empty)
			{
				if (mModelToProperties.ContainsKey(text2))
				{
					mModelToProperties[text2].Add(new CountrySimLockPropeties(carrier, simLock, countryCode, roCarrier, productName, rsdCarrier, eLabelFile));
					return;
				}
				mModelToProperties.Add(text2, new List<CountrySimLockPropeties>
				{
					new CountrySimLockPropeties(carrier, simLock, countryCode, roCarrier, productName, rsdCarrier, eLabelFile)
				});
			}
		}
	}

	protected override string BuildCarrierString(CountrySimLockPropeties property, int i, string rsdCarrier)
	{
		string result = string.Empty;
		if (property.RsdCarrier == string.Empty || string.Compare(property.RsdCarrier, rsdCarrier, ignoreCase: true) == 0)
		{
			string text = property.CountryCode.Replace("-", "_");
			result = i + " - " + text + " # " + property.Carrier;
		}
		return result;
	}
}
