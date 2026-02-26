using System.Collections.Generic;

namespace SmartRsd;

public class ModelCountrySimLock : ModelPropertyBase<CountrySimLockPropeties>
{
	private static Dictionary<int, string> sFormats = new Dictionary<int, string>
	{
		{ 1, "SVNKITCarrier" },
		{ 4, "ModelNumber(HWSKU)" },
		{ 5, "SIMType" },
		{ 9, "countrycode.ontim" }
	};

	private const int cSkipEmptyOn = 4;

	private int mIndex1 = 1;

	private int mIndex4 = 4;

	private int mIndex5 = 5;

	private int mIndex9 = 9;

	private int mIndex10 = 10;

	private int mSkipEmptyOn = 4;

	private static ModelCountrySimLock sThis = null;

	private static string sCsvFilePathName = string.Empty;

	private string TAG => GetType().FullName;

	public static bool FormatMatched(string csvFilePathName)
	{
		return ModelPropertyBase<CountrySimLockPropeties>.FormatMatched(csvFilePathName, 4, sFormats);
	}

	public static ModelCountrySimLock Instance(string csvFilePathName)
	{
		if (csvFilePathName != sCsvFilePathName || sThis == null)
		{
			sThis = new ModelCountrySimLock(csvFilePathName);
		}
		return sThis;
	}

	private ModelCountrySimLock(string csvFilePathName)
		: base(csvFilePathName)
	{
		sCsvFilePathName = csvFilePathName;
	}

	protected override void AddProperty(List<string> fields)
	{
		string text = fields[0].Replace(" ", string.Empty).ToLower();
		if (text == "region" || text == "rsdcarrier")
		{
			if (ModelPropertyBase<CountrySimLockPropeties>.FindFieldName(fields, "rsdcarrier", out mRsdCarrierIndex))
			{
				mOffset = 1;
				mIndex1 = 1 + ((1 >= mRsdCarrierIndex) ? mOffset : 0);
				mIndex4 = 4 + ((4 >= mRsdCarrierIndex) ? mOffset : 0);
				mIndex5 = 5 + ((5 >= mRsdCarrierIndex) ? mOffset : 0);
				mIndex9 = 9 + ((9 >= mRsdCarrierIndex) ? mOffset : 0);
				mIndex10 = 10 + ((10 >= mRsdCarrierIndex) ? mOffset : 0);
				mSkipEmptyOn = 4 + ((4 >= mRsdCarrierIndex) ? mOffset : 0);
			}
		}
		else
		{
			if (!(fields[mSkipEmptyOn].Trim() != string.Empty))
			{
				return;
			}
			string text2 = fields[mIndex4].Trim() + "-" + fields[mIndex5].Trim();
			string carrier = fields[mIndex1].Replace("\"", string.Empty).Trim();
			string simLock = fields[mIndex10].Trim();
			string countryCode = fields[mIndex9].Trim();
			string rsdCarrier = ((mOffset > 0) ? fields[mRsdCarrierIndex].Trim() : string.Empty);
			if (text2 != string.Empty)
			{
				if (mModelToProperties.ContainsKey(text2))
				{
					mModelToProperties[text2].Add(new CountrySimLockPropeties(carrier, simLock, countryCode, string.Empty, string.Empty, rsdCarrier));
					return;
				}
				mModelToProperties.Add(text2, new List<CountrySimLockPropeties>
				{
					new CountrySimLockPropeties(carrier, simLock, countryCode, string.Empty, string.Empty, rsdCarrier)
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
			result = i + " - " + text + " # " + property.Carrier + " " + property.CountryCode;
		}
		return result;
	}
}
