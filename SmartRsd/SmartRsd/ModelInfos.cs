using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmartRsd;

public class ModelInfos
{
	private static ModelInfos sThis;

	private static string sParameterFilePathName = string.Empty;

	private Dictionary<string, Dictionary<string, string>> mPartNumbersToModelPropperties = new Dictionary<string, Dictionary<string, string>>();

	private Dictionary<string, List<string>> mModelsToPartNumbers = new Dictionary<string, List<string>>();

	private static List<string> sCapturedProperties = new List<string> { "Model", "PN", "Color", "RAM" };

	public static ModelInfos Instance(string parameterFilePathName)
	{
		if (parameterFilePathName != sParameterFilePathName || sThis == null)
		{
			sThis = new ModelInfos(parameterFilePathName);
		}
		return sThis;
	}

	public List<string> GetPartNumberLines(string phoneModel)
	{
		List<string> list = new List<string>();
		string skuFromPhoneName = Utilities.GetSkuFromPhoneName(phoneModel.Split(new char[1] { '/' })[0]);
		if (mModelsToPartNumbers.ContainsKey(skuFromPhoneName))
		{
			foreach (string item2 in mModelsToPartNumbers[skuFromPhoneName])
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string sCapturedProperty in sCapturedProperties)
				{
					if (!mPartNumbersToModelPropperties[item2].TryGetValue(sCapturedProperty, out var value))
					{
						value = ((!(sCapturedProperty == "PN")) ? "UNKNOWN" : item2);
					}
					stringBuilder.Append(value);
					stringBuilder.Append(" ");
				}
				string item = stringBuilder.ToString().Trim();
				list.Add(item);
			}
			list.Sort();
		}
		return list;
	}

	private ModelInfos(string parameterFilePathName)
	{
		if (File.Exists(parameterFilePathName))
		{
			string[] lines = File.ReadAllLines(parameterFilePathName, Encoding.UTF8);
			Parse(lines);
		}
	}

	private void Parse(string[] lines)
	{
		mPartNumbersToModelPropperties.Clear();
		mModelsToPartNumbers.Clear();
		foreach (string text in lines)
		{
			if (string.IsNullOrEmpty(text.Trim()) || text.Trim().StartsWith("//"))
			{
				continue;
			}
			string[] array = text.Split(new char[1] { ',' });
			if (!sCapturedProperties.Contains(array[1]))
			{
				continue;
			}
			if (mPartNumbersToModelPropperties.ContainsKey(array[0]))
			{
				mPartNumbersToModelPropperties[array[0]][array[1]] = array[2];
			}
			else
			{
				mPartNumbersToModelPropperties[array[0]] = new Dictionary<string, string> { 
				{
					array[1],
					array[2]
				} };
			}
			if (!(array[1] == "Model"))
			{
				continue;
			}
			if (mModelsToPartNumbers.ContainsKey(array[2]))
			{
				if (!mModelsToPartNumbers[array[2]].Contains(array[0]))
				{
					mModelsToPartNumbers[array[2]].Add(array[0]);
				}
			}
			else
			{
				mModelsToPartNumbers[array[2]] = new List<string> { array[0] };
			}
		}
	}
}
