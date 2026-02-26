using System.Collections.Generic;

namespace SmartRsd;

public class JsonConfigList
{
	public string LatestVersion { get; private set; } = string.Empty;


	public List<JsonConfig> JsonConfigs { get; private set; } = new List<JsonConfig>();


	public JsonPhoneModel PhoneModel { get; private set; }

	public JsonConfig LatestConfig { get; set; }

	private string TAG => GetType().FullName;

	public JsonConfigList(JsonConfig config)
	{
		JsonConfigs.Add(config);
		LatestConfig = config;
		LatestVersion = config.Config;
	}

	public JsonConfigList(ConfigListDef configList, JsonPhoneModel phoneModel)
	{
		PhoneModel = phoneModel;
		LatestVersion = ((configList.latest_version != null) ? configList.latest_version : string.Empty).Trim();
		if (configList.config != null)
		{
			SortedList<string, string>[] config = configList.config;
			for (int i = 0; i < config.Length; i++)
			{
				JsonConfig item = new JsonConfig(config[i], this);
				JsonConfigs.Add(item);
			}
			if (JsonConfigs.Count == 0 && PhoneModel.Matrix.FullyParsed)
			{
				Smart.Log.Error(TAG, string.Format("No entry in {0} [PhoneModel {1}] [Carrier {2}]", "config", phoneModel.Name, phoneModel.Matrix.InternalName));
			}
		}
		else if (PhoneModel.Matrix.FullyParsed)
		{
			Smart.Log.Error(TAG, string.Format("{0} entry is missing [PhoneModel {1}] [Carrier {2}]", "config", phoneModel.Name, phoneModel.Matrix.InternalName));
		}
		if (LatestConfig == null && JsonConfigs.Count > 0)
		{
			LatestConfig = JsonConfigs[JsonConfigs.Count - 1];
			LatestConfig.LatestVersion = true;
			LatestVersion = LatestConfig.Config;
		}
	}
}
