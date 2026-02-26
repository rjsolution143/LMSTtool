using System;
using System.Collections.Generic;
using System.IO;
using ISmart;

namespace SmartDevice;

public class Validator : IValidator
{
	private string profilePath = string.Empty;

	private const string ALL_MODES = "All Modes";

	private const string FASTBOOT = "Fastboot";

	private const string TCMD = "TCMD";

	private const string MANUAL = "Manual";

	private const string LEGACY = "Validation";

	private string TAG => GetType().FullName;

	public Validator()
	{
		string commonStorageDir = Smart.File.CommonStorageDir;
		profilePath = Smart.File.PathJoin(commonStorageDir, "Validation\\");
		if (!Smart.File.Exists(profilePath))
		{
			Directory.CreateDirectory(profilePath);
		}
	}

	public List<string> FindRecipes(IDevice device)
	{
		List<string> list = new List<string>();
		string modelId = device.ModelId;
		string text = Smart.Rsd.LoadRecipe((UseCase)902, ref modelId, device);
		if (text != null && text.Trim() != string.Empty)
		{
			list.Add("All Modes");
		}
		text = Smart.Rsd.LoadRecipe((UseCase)901, ref modelId, device);
		if (text != null && text.Trim() != string.Empty)
		{
			list.Add("TCMD");
		}
		text = Smart.Rsd.LoadRecipe((UseCase)900, ref modelId, device);
		if (text != null && text.Trim() != string.Empty)
		{
			list.Add("Fastboot");
		}
		text = Smart.Rsd.LoadRecipe((UseCase)903, ref modelId, device);
		if (text != null && text.Trim() != string.Empty)
		{
			list.Add("Manual");
		}
		if (list.Count < 1)
		{
			Smart.Log.Debug(TAG, "No valid Validation recipes found, using Legacy recipe");
			list.Add("Validation");
		}
		return list;
	}

	public List<string> FindProfiles(IDevice device, string recipe)
	{
		List<string> list = new List<string>();
		if (!Smart.File.Exists(profilePath))
		{
			return list;
		}
		foreach (string item in Smart.File.FindFiles("*.prof", profilePath, false))
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item);
			if (recipe != string.Empty)
			{
				string text = Smart.File.ReadText(item);
				dynamic val = Smart.Json.Load(text);
				if (((string)val["recipe"]).ToLowerInvariant() != recipe.ToLowerInvariant())
				{
					continue;
				}
			}
			list.Add(fileNameWithoutExtension);
		}
		return list;
	}

	public void SaveProfile(IDevice device, string profileName, string recipe, List<ValidationItem> options)
	{
		string text = profilePath + profileName + ".prof";
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["recipe"] = recipe;
		dictionary["items"] = options;
		string text2 = Smart.Json.Dump((object)dictionary);
		Smart.File.WriteText(text, text2);
	}

	public void DeleteProfile(IDevice device, string profileName)
	{
		string text = profilePath + profileName + ".prof";
		Smart.File.Delete(text);
	}

	public List<ValidationItem> LoadProfile(IDevice device, string recipe, string profileName)
	{
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Expected O, but got Unknown
		List<ValidationItem> list = new List<ValidationItem>();
		string text = profilePath + profileName + ".prof";
		string text2 = Smart.File.ReadText(text);
		dynamic val = Smart.Json.Load(text2);
		foreach (dynamic item2 in val["items"])
		{
			string text3 = item2["Name"];
			bool flag = item2["Enabled"];
			string text4 = item2["Target"];
			string text5 = item2["Value"];
			Result val2 = (Result)item2["Result"];
			ValidationItem item = new ValidationItem(text3, flag, text4, text5, val2);
			list.Add(item);
		}
		return list;
	}

	private UseCase RecipeNameToUseCase(string recipeName)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		UseCase result = (UseCase)166;
		switch (recipeName)
		{
		case "All Modes":
			result = (UseCase)902;
			break;
		case "Fastboot":
			result = (UseCase)900;
			break;
		case "TCMD":
			result = (UseCase)901;
			break;
		case "Manual":
			result = (UseCase)903;
			break;
		case "Validation":
			result = (UseCase)166;
			break;
		default:
			Smart.Log.Error(TAG, string.Format("Unrecognized recipe: " + recipeName));
			break;
		}
		return result;
	}

	public void InitRecipe(IDevice device, string recipe)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		UseCase val = RecipeNameToUseCase(recipe);
		Smart.UseCaseRunner.Run(val, device, true, false);
	}

	public List<ValidationItem> FindItems(IDevice device)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		List<ValidationItem> list = new List<ValidationItem>();
		IResultLogger log = device.Log;
		foreach (Tuple<string, SortedList<string, object>> result in log.Results)
		{
			string item = result.Item1;
			SortedList<string, object> item2 = result.Item2;
			Result val = (Result)(dynamic)item2["result"];
			string text = (dynamic)item2["description"];
			ValidationItem item3 = new ValidationItem(item, text, val);
			list.Add(item3);
		}
		foreach (string key in log.Info.Keys)
		{
			string text2 = log.Info[key];
			ValidationItem item4 = new ValidationItem(key, false, text2, text2, (Result)8);
			list.Add(item4);
		}
		return list;
	}

	public void RunRecipe(IDevice device, string recipe, string profile)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		UseCase val = RecipeNameToUseCase(recipe);
		Smart.UseCaseRunner.Run(val, device, true, false);
	}

	public List<ValidationItem> FindResults(IDevice device, string recipe, string profile)
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Expected O, but got Unknown
		List<ValidationItem> list = new List<ValidationItem>();
		List<ValidationItem> list2 = FindItems(device);
		List<ValidationItem> list3 = LoadProfile(device, recipe, profile);
		SortedList<string, ValidationItem> sortedList = new SortedList<string, ValidationItem>();
		foreach (ValidationItem item2 in list3)
		{
			sortedList[item2.Name] = item2;
		}
		List<string> list4 = new List<string>();
		foreach (ValidationItem item3 in list2)
		{
			ValidationItem val = item3;
			if (!sortedList.ContainsKey(item3.Name))
			{
				continue;
			}
			ValidationItem val2 = sortedList[item3.Name];
			if (val2.Enabled)
			{
				val.Enabled = true;
				if (val2.Target.Trim().ToLowerInvariant() != item3.Value.Trim().ToLowerInvariant())
				{
					val = new ValidationItem(item3.Name, true, val2.Value, item3.Value, (Result)1);
				}
				if (!list4.Contains(item3.Name))
				{
					list4.Add(item3.Name);
				}
				list.Add(val);
			}
		}
		foreach (ValidationItem item4 in list3)
		{
			if (item4.Enabled && !list4.Contains(item4.Name))
			{
				ValidationItem item = new ValidationItem(item4.Name, true, item4.Value, "MISSING", (Result)1);
				list.Add(item);
			}
		}
		return list;
	}
}
