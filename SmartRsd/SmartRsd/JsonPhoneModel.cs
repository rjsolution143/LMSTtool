using System;
using System.Collections.Generic;
using ISmart;

namespace SmartRsd;

public class JsonPhoneModel
{
	public string Name { get; private set; }

	public string Technology { get; private set; }

	public string MSMarketName { get; private set; }

	public bool SameSofwareUpdate { get; private set; }

	public JsonConfigList PhoneIdString { get; private set; }

	public JsonRecipeGroup RecipeGroup { get; private set; }

	public JsonFileEntry ModelImage { get; private set; }

	public JsonFileEntry ModelConfiguration { get; private set; }

	public JsonMatrix Matrix { get; private set; }

	public Dictionary<string, JsonConfig> ConfigIdToJsonConfigLookup { get; private set; }

	public Dictionary<string, JsonConfig> FingerPrintToJsonConfigLookup { get; private set; }

	public Dictionary<string, JsonRecipe> DefaultUsecaseToRecipeLookup { get; private set; }

	public bool Valid { get; private set; }

	private string TAG => GetType().FullName;

	public JsonPhoneModel(ModelDef model, JsonMatrix matrix)
	{
		Matrix = matrix;
		Valid = false;
		Name = ((model.name != null) ? model.name : string.Empty).Trim();
		Technology = ((model.technology != null) ? model.technology : string.Empty).Trim();
		MSMarketName = ((model.motoServiceMKTName != null) ? model.motoServiceMKTName : string.Empty).Trim();
		ConfigIdToJsonConfigLookup = new Dictionary<string, JsonConfig>();
		FingerPrintToJsonConfigLookup = new Dictionary<string, JsonConfig>();
		DefaultUsecaseToRecipeLookup = new Dictionary<string, JsonRecipe>();
		SameSofwareUpdate = model.samesoftwareupdate != null && Convert.ToBoolean(model.samesoftwareupdate);
		PhoneIdString = null;
		if (model.configList != null)
		{
			PhoneIdString = new JsonConfigList(model.configList, this);
		}
		else if (Matrix.FullyParsed)
		{
			Smart.Log.Error(TAG, string.Format("{0} entry is missing [PhoneModel {1}] [Carrier {2}]", "configList", Name, matrix.InternalName));
		}
		if (model.image != null)
		{
			ModelImage = new JsonFileEntry(model.image, this);
		}
		else if (Matrix.FullyParsed)
		{
			Smart.Log.Error(TAG, string.Format("{0} is missing [PhoneName {1}] [Carrier {2}]", "image", Name, matrix.InternalName));
		}
		if (model.configuration != null)
		{
			ModelConfiguration = new JsonFileEntry(model.configuration, this);
		}
		else if (Matrix.FullyParsed)
		{
			Smart.Log.Error(TAG, string.Format("{0} is missing [PhoneModel {1}] [Carrier {2}]", "configuration", Name, matrix.InternalName));
		}
		if (model.recipes != null)
		{
			RecipeGroup = new JsonRecipeGroup(model.recipes, this);
			Valid = RecipeGroup.Valid;
		}
		string text = Name + "|" + Matrix.InternalName;
		if (!Matrix.FullyParsed)
		{
			string[] array = Name.Split(new char[1] { '/' });
			string arg = array[0].Trim();
			string value = array[1].Trim();
			string arg2 = Matrix.InternalName.Trim();
			JsonMatrixParser.Instance.AllModelCarrierToFlexModelLookup[$"{arg}/{arg2}"] = value;
			JsonMatrixParser.Instance.AllModelIdToPhoneModelLookup[text] = this;
			return;
		}
		if (!Valid)
		{
			Smart.Log.Error(TAG, string.Format("recipes is not valid. Skipping this model [PhoneModel {1}] [Carrier {2}]", Name, matrix.InternalName));
			return;
		}
		if (Name == string.Empty)
		{
			Smart.Log.Error(TAG, string.Format("{0} is missing in {1} [Carrier {2}]", "name", "phoneModels", matrix.InternalName));
		}
		else if (!JsonMatrixParser.Instance.ModelIdToPhoneModelookup.ContainsKey(text))
		{
			JsonMatrixParser.Instance.ModelIdToPhoneModelookup.Add(text, this);
		}
		else
		{
			Smart.Log.Error(TAG, $"ModelId {text} is duplicated [PhoneName {Name}] [Carrier {matrix.InternalName}]");
		}
		if (!JsonMatrixParser.Instance.CarrierToModelsLookup[Matrix.InternalName].Contains(Name))
		{
			JsonMatrixParser.Instance.CarrierToModelsLookup[Matrix.InternalName].Add(Name);
		}
		if (JsonMatrixParser.Instance.ModelToCarriersLookup.ContainsKey(Name))
		{
			if (!JsonMatrixParser.Instance.ModelToCarriersLookup[Name].Contains(Matrix.InternalName))
			{
				JsonMatrixParser.Instance.ModelToCarriersLookup[Name].Add(Matrix.InternalName);
			}
		}
		else
		{
			List<string> value2 = new List<string> { Matrix.InternalName };
			JsonMatrixParser.Instance.ModelToCarriersLookup.Add(Name, value2);
		}
	}

	public DeviceProfile GetDeviceProfile(string configId)
	{
		DeviceProfile result = null;
		JsonConfig value = null;
		if (PhoneIdString != null && PhoneIdString.JsonConfigs.Count > 0)
		{
			if (configId != string.Empty)
			{
				ConfigIdToJsonConfigLookup.TryGetValue(configId, out value);
			}
			else
			{
				ConfigIdToJsonConfigLookup.TryGetValue(PhoneIdString.LatestVersion, out value);
			}
			if (value != null)
			{
				result = value.GetDeviceProfile();
			}
		}
		return result;
	}

	public DeviceProfile GetDeviceProfileWithFingerPrint(string fingerPrint)
	{
		DeviceProfile result = null;
		JsonConfig value = null;
		if (PhoneIdString != null && PhoneIdString.JsonConfigs.Count > 0)
		{
			if (fingerPrint != string.Empty)
			{
				FingerPrintToJsonConfigLookup.TryGetValue(fingerPrint, out value);
			}
			if (value == null)
			{
				ConfigIdToJsonConfigLookup.TryGetValue(PhoneIdString.LatestVersion, out value);
			}
			if (value != null)
			{
				result = value.GetDeviceProfile();
			}
		}
		return result;
	}

	public void Delete()
	{
		string key = Name + "|" + Matrix.InternalName;
		string[] array = Name.Split(new char[1] { '/' });
		array[0].Trim();
		array[1].Trim();
		Matrix.InternalName.Trim();
		if (JsonMatrixParser.Instance.ModelIdToPhoneModelookup.ContainsKey(key))
		{
			JsonMatrixParser.Instance.ModelIdToPhoneModelookup.Remove(key);
		}
		if (JsonMatrixParser.Instance.CarrierToModelsLookup[Matrix.InternalName].Contains(Name))
		{
			JsonMatrixParser.Instance.CarrierToModelsLookup[Matrix.InternalName].Remove(Name);
		}
		if (JsonMatrixParser.Instance.ModelToCarriersLookup.ContainsKey(Name) && JsonMatrixParser.Instance.ModelToCarriersLookup[Name].Contains(Matrix.InternalName))
		{
			JsonMatrixParser.Instance.ModelToCarriersLookup[Name].Remove(Matrix.InternalName);
		}
		foreach (JsonConfig jsonConfig in PhoneIdString.JsonConfigs)
		{
			List<DetectionKey> list = new List<DetectionKey>();
			foreach (DetectionKey key2 in JsonMatrixParser.Instance.DetectionKeyToJsonConfigLookup.Keys)
			{
				if (JsonMatrixParser.Instance.DetectionKeyToJsonConfigLookup[key2].Contains(jsonConfig))
				{
					list.Add(key2);
				}
			}
			foreach (DetectionKey item in list)
			{
				JsonMatrixParser.Instance.DetectionKeyToJsonConfigLookup[item].Remove(jsonConfig);
				if (JsonMatrixParser.Instance.DetectionKeyToJsonConfigLookup[item].Count < 1)
				{
					JsonMatrixParser.Instance.DetectionKeyToJsonConfigLookup.Remove(item);
				}
			}
		}
	}
}
