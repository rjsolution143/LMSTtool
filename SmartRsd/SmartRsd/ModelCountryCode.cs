using System.Collections.Generic;

namespace SmartRsd;

public class ModelCountryCode : ModelPropertyBase<CountryPropeties>
{
	private static Dictionary<int, string> sFormats = new Dictionary<int, string> { { 1, "ProjectName" } };

	private const int cSkipEmptyOn = 3;

	private int mIndex3 = 3;

	private int mIndex4 = 4;

	private int mIndex6 = 6;

	private int mIndex7 = 7;

	private int mIndex8 = 8;

	private int mSkipEmptyOn = 3;

	private static ModelCountryCode sThis = null;

	private static string sCsvFilePathName = string.Empty;

	private string TAG => GetType().FullName;

	public static bool FormatMatched(string csvFilePathName)
	{
		return ModelPropertyBase<CountryPropeties>.FormatMatched(csvFilePathName, 3, sFormats);
	}

	public static ModelCountryCode Instance(string csvFilePathName)
	{
		if (csvFilePathName != sCsvFilePathName || sThis == null)
		{
			sThis = new ModelCountryCode(csvFilePathName);
		}
		return sThis;
	}

	private ModelCountryCode(string csvFilePathName)
		: base(csvFilePathName)
	{
		sCsvFilePathName = csvFilePathName;
	}

	protected override void AddProperty(List<string> fields)
	{
		string text = fields[0].Replace(" ", string.Empty).ToLower();
		string text2 = fields[1].Replace(" ", string.Empty).ToLower();
		string text3 = fields[2].Replace(" ", string.Empty).ToLower();
		switch (text)
		{
		default:
			if (!(text2 == "projectname") && !(text3 == "projectname"))
			{
				if (!(fields[mSkipEmptyOn] != string.Empty))
				{
					break;
				}
				string text4 = fields[mIndex3].Trim();
				string countryGroup = fields[mIndex4].Replace("\"", string.Empty).Trim();
				string roCarrier = fields[mIndex6].Trim();
				string language = fields[mIndex7].Trim();
				string countryCode = fields[mIndex8].Trim();
				string rsdCarrier = ((mOffset > 0) ? fields[mRsdCarrierIndex].Trim() : string.Empty);
				if (text4 != string.Empty)
				{
					if (mModelToProperties.ContainsKey(text4))
					{
						mModelToProperties[text4].Add(new CountryPropeties(countryGroup, roCarrier, language, countryCode, rsdCarrier));
						break;
					}
					mModelToProperties.Add(text4, new List<CountryPropeties>
					{
						new CountryPropeties(countryGroup, roCarrier, language, countryCode, rsdCarrier)
					});
				}
				break;
			}
			goto case "channelnameconfig";
		case "channelnameconfig":
		case "names_of_project":
		case "rsdcarrier":
			if (ModelPropertyBase<CountryPropeties>.FindFieldName(fields, "rsdcarrier", out mRsdCarrierIndex))
			{
				mOffset = 1;
				mIndex3 = 3 + ((3 >= mRsdCarrierIndex) ? mOffset : 0);
				mIndex4 = 4 + ((4 >= mRsdCarrierIndex) ? mOffset : 0);
				mIndex6 = 6 + ((6 >= mRsdCarrierIndex) ? mOffset : 0);
				mIndex7 = 7 + ((7 >= mRsdCarrierIndex) ? mOffset : 0);
				mIndex8 = 8 + ((8 >= mRsdCarrierIndex) ? mOffset : 0);
				mSkipEmptyOn = 3 + ((3 >= mRsdCarrierIndex) ? mOffset : 0);
			}
			break;
		}
	}

	protected override string BuildCarrierString(CountryPropeties property, int i, string rsdCarrier)
	{
		string result = string.Empty;
		if (property.RsdCarrier == string.Empty || string.Compare(property.RsdCarrier, rsdCarrier, ignoreCase: true) == 0)
		{
			string text = property.CountryCode.Replace("-", "_");
			result = ((!(property.RoCarrier != string.Empty)) ? (i + " - " + text + " # " + property.CountryGroup) : (i + " - " + text + " # " + property.CountryGroup + "_" + property.RoCarrier));
		}
		return result;
	}
}
