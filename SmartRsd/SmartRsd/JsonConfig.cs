using System;
using System.Collections.Generic;
using System.IO;
using ISmart;

namespace SmartRsd;

public class JsonConfig
{
	public string Config { get; private set; } = string.Empty;


	public string Flex { get; private set; } = string.Empty;


	public string Firmware { get; private set; } = string.Empty;


	public string BlurVersion { get; private set; } = string.Empty;


	public string FingerPrint { get; private set; } = string.Empty;


	public string RoCarrier { get; private set; } = string.Empty;


	public string ChannelId { get; private set; } = string.Empty;


	public string FSGVersion { get; private set; } = string.Empty;


	public string FSGCarrierId { get; private set; } = string.Empty;


	public string BootLoader { get; private set; } = string.Empty;


	public string SimConfig { get; private set; } = string.Empty;


	public string Sku { get; private set; } = string.Empty;


	public string CountryCode0 { get; private set; } = string.Empty;


	public string CountryCode1 { get; private set; } = string.Empty;


	public string HardwareProperty { get; private set; } = string.Empty;


	public string HardwareProperty1 { get; private set; } = string.Empty;


	public string HardwareCode { get; private set; } = string.Empty;


	public string CustomerSwVersion { get; private set; } = string.Empty;


	public string CountryCode { get; private set; } = string.Empty;


	public string FlashSize { get; private set; } = string.Empty;


	public string RamSize { get; private set; } = string.Empty;


	public string CarrierSku { get; private set; } = string.Empty;


	public string SvnkitId { get; private set; } = string.Empty;


	public string DualSim { get; private set; } = string.Empty;


	public string RecipeProduct { get; private set; } = string.Empty;


	public string RsuSip { get; private set; } = string.Empty;


	public string RsuSocModel { get; private set; } = string.Empty;


	public string RsuOperator { get; private set; } = string.Empty;


	public string ESim { get; private set; } = string.Empty;


	public string BuildDisplay { get; private set; } = string.Empty;


	public string ProductModel { get; private set; } = string.Empty;


	public string InternalVersion { get; private set; } = string.Empty;


	public string MiiModel { get; private set; } = string.Empty;


	public JsonConfigList ConfigList { get; private set; }

	public bool LatestVersion { get; set; }

	public Dictionary<string, JsonRecipe> UsecaseToRecipeLookup { get; private set; } = new Dictionary<string, JsonRecipe>();


	private string TAG => GetType().FullName;

	public JsonConfig()
	{
		ConfigList = new JsonConfigList(this);
		LatestVersion = true;
	}

	public JsonConfig(SortedList<string, string> properties, JsonConfigList configList)
	{
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Expected O, but got Unknown
		ConfigList = configList;
		DetectionKey val = new DetectionKey(properties, "IGNORE", false, "");
		Config = (properties.TryGetValue("config", out var value) ? value.Trim() : string.Empty);
		Flex = (properties.TryGetValue("flex", out value) ? value.Trim() : string.Empty);
		Firmware = (properties.TryGetValue("firmware", out value) ? value.Trim() : string.Empty);
		BlurVersion = val.GetValue("blur");
		FingerPrint = val.GetValue("fingerPrint");
		RoCarrier = val.GetValue("roCarrier");
		ChannelId = (properties.TryGetValue("channelID", out value) ? value.Trim() : string.Empty);
		FSGCarrierId = (properties.TryGetValue("fsgCarrierID", out value) ? value.Trim() : string.Empty);
		FSGVersion = val.GetValue("fsgVersion");
		BootLoader = (properties.TryGetValue("bootloader", out value) ? value.Trim() : string.Empty);
		SimConfig = val.GetValue("simConfig");
		Sku = val.GetValue("sku");
		CountryCode0 = (properties.TryGetValue("ro.lenovo.easyimage.code", out value) ? value.Trim() : string.Empty);
		CountryCode1 = (properties.TryGetValue("persist.sys.withsim.country", out value) ? value.Trim() : string.Empty);
		FlashSize = val.GetValue("flashSize");
		RamSize = val.GetValue("ramSize");
		CarrierSku = (properties.TryGetValue("carrierSKU", out value) ? value.Trim() : string.Empty);
		SvnkitId = (properties.TryGetValue("svnkit", out value) ? value.Trim() : string.Empty);
		HardwareProperty = val.GetValue("hardwareProperty");
		HardwareProperty1 = val.GetValue("hardwareProperty1");
		CustomerSwVersion = val.GetValue("customerSwVersion");
		DualSim = val.GetValue("dualSim");
		RecipeProduct = (properties.TryGetValue("recipeProduct", out value) ? value.Trim() : string.Empty);
		RsuSip = (properties.TryGetValue("RSU_SIP", out value) ? value.Trim() : string.Empty);
		RsuSocModel = (properties.TryGetValue("RSU_SOC_MODEL", out value) ? value.Trim() : string.Empty);
		RsuOperator = (properties.TryGetValue("RSU_OPERATOR", out value) ? value.Trim() : string.Empty);
		ESim = (properties.TryGetValue("ro.vendor.hw.esim", out value) ? value.Trim() : string.Empty);
		BuildDisplay = val.GetValue("builddisplay");
		ProductModel = val.GetValue("productModel");
		InternalVersion = (properties.TryGetValue("internal.version", out value) ? value.Trim() : string.Empty);
		MiiModel = (properties.TryGetValue("mmi_model", out value) ? value.Trim() : string.Empty);
		LatestVersion = configList.LatestVersion == Config;
		HardwareCode = val.HardwareCode;
		if (CountryCode0 != string.Empty && string.Compare(CountryCode0, "IGNORE", ignoreCase: true) != 0)
		{
			CountryCode = CountryCode0;
		}
		else if (CountryCode1 != string.Empty && string.Compare(CountryCode1, "IGNORE", ignoreCase: true) != 0)
		{
			CountryCode = CountryCode1;
		}
		else if (RoCarrier != string.Empty && string.Compare(RoCarrier, "IGNORE", ignoreCase: true) != 0)
		{
			CountryCode = RoCarrier;
		}
		else
		{
			CountryCode = string.Empty;
		}
		if (Config != string.Empty && LatestVersion)
		{
			ConfigList.LatestConfig = this;
		}
		Dictionary<DetectionKey, List<JsonConfig>> lookupTable = JsonMatrixParser.Instance.MatrixTypeToMatrixInfoLookup[ConfigList.PhoneModel.Matrix.MxType].LookupTable;
		if (!val.IsEmpty())
		{
			if (!lookupTable.ContainsKey(val))
			{
				lookupTable.Add(val, new List<JsonConfig> { this });
			}
			else if (!Contains(lookupTable[val], this))
			{
				lookupTable[val].Add(this);
			}
			lookupTable[val].TrimExcess();
		}
		else if (ConfigList.PhoneModel.Matrix.FullyParsed)
		{
			Smart.Log.Error(TAG, $"detectionKey {((object)val).ToString()} of config: {Config} is empty");
		}
		if (Config.Length > 0)
		{
			if (!configList.PhoneModel.ConfigIdToJsonConfigLookup.ContainsKey(Config))
			{
				configList.PhoneModel.ConfigIdToJsonConfigLookup.Add(Config, this);
			}
			else if (ConfigList.PhoneModel.Matrix.FullyParsed)
			{
				Smart.Log.Error(TAG, $"config = {Config} is duplicated [PhoneModel {configList.PhoneModel.Name}] [Carrier {configList.PhoneModel.Matrix.InternalName}]");
			}
		}
		else if (ConfigList.PhoneModel.Matrix.FullyParsed)
		{
			Smart.Log.Error(TAG, $"config entry is missing or empty [PhoneModel {configList.PhoneModel.Name}] [Carrier {configList.PhoneModel.Matrix.InternalName}]");
		}
		if (FingerPrint != string.Empty)
		{
			configList.PhoneModel.FingerPrintToJsonConfigLookup[FingerPrint] = this;
		}
		else
		{
			configList.PhoneModel.FingerPrintToJsonConfigLookup[BuildDisplay] = this;
		}
		if (ConfigList.PhoneModel.Matrix.FullyParsed)
		{
			return;
		}
		string svnkitId = SvnkitId;
		if (svnkitId != string.Empty && !svnkitId.Contains("IGNORE"))
		{
			string[] array = svnkitId.Split(new char[1] { ',' });
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				if (!(text != string.Empty))
				{
					continue;
				}
				if (JsonMatrixParser.Instance.AllSvnkidIdToPhoneModelLookup.ContainsKey(text))
				{
					if (!Contains(JsonMatrixParser.Instance.AllSvnkidIdToPhoneModelLookup[text], configList.PhoneModel))
					{
						JsonMatrixParser.Instance.AllSvnkidIdToPhoneModelLookup[text].Add(configList.PhoneModel);
					}
				}
				else
				{
					JsonMatrixParser.Instance.AllSvnkidIdToPhoneModelLookup[text] = new List<JsonPhoneModel> { configList.PhoneModel };
				}
				JsonMatrixParser.Instance.AllSvnkidIdToPhoneModelLookup[text].TrimExcess();
			}
		}
		else if (InternalVersion != string.Empty)
		{
			if (JsonMatrixParser.Instance.AllInternalSwToPhoneModelLookup.ContainsKey(InternalVersion))
			{
				JsonMatrixParser.Instance.AllInternalSwToPhoneModelLookup[InternalVersion].Add(new ProductModelInfo(ProductModel, configList.PhoneModel));
			}
			else
			{
				JsonMatrixParser.Instance.AllInternalSwToPhoneModelLookup[InternalVersion] = new List<ProductModelInfo>
				{
					new ProductModelInfo(ProductModel, configList.PhoneModel)
				};
			}
			JsonMatrixParser.Instance.AllInternalSwToPhoneModelLookup[InternalVersion].TrimExcess();
		}
	}

	public DeviceProfile GetDeviceProfile()
	{
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		string modelImageFileName = ((ConfigList.PhoneModel.ModelImage != null) ? (Configurations.ModelImagePath + ConfigList.PhoneModel.ModelImage.FileName) : string.Empty);
		string modelInfoFileName = ((ConfigList.PhoneModel.ModelConfiguration != null) ? (Configurations.ModelInfoPath + ConfigList.PhoneModel.ModelConfiguration.FileName) : string.Empty);
		DeviceProfile deviceProfile = new DeviceProfile(ConfigList.PhoneModel.Matrix.InternalName, ConfigList.PhoneModel.Matrix.CarrierCode, ConfigList.PhoneModel.MSMarketName, ConfigList.PhoneModel.Name, ConfigList.PhoneModel.Technology, modelImageFileName, modelInfoFileName, this);
		SortedList<UseCase, RecipeDescriptor> sortedList = new SortedList<UseCase, RecipeDescriptor>();
		foreach (UseCase value in Enum.GetValues(typeof(UseCase)))
		{
			if (JsonMatrixParser.Instance.Account != null && JsonMatrixParser.Instance.Account.AllowedUseCases.Contains(value))
			{
				RecipeDescriptor recipeDescriptor = GetRecipeDescriptor(value);
				if (recipeDescriptor != null)
				{
					sortedList.Add(value, recipeDescriptor);
				}
			}
		}
		deviceProfile.UseCasesToRecipes = sortedList;
		if (UsecaseToRecipeLookup.Count > 0)
		{
			deviceProfile.ConfigId = Config;
		}
		return deviceProfile;
	}

	public bool IsAllowedUpgrade()
	{
		if (LatestVersion)
		{
			return ConfigList.PhoneModel.SameSofwareUpdate;
		}
		return true;
	}

	private RecipeDescriptor GetRecipeDescriptor(UseCase usecase)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		JsonRecipe value = null;
		string key = ((object)(UseCase)(ref usecase)).ToString().ToLower();
		RecipeDescriptor recipeDescriptor = null;
		if (UsecaseToRecipeLookup.Count > 0)
		{
			UsecaseToRecipeLookup.TryGetValue(key, out value);
		}
		if (value == null)
		{
			ConfigList.PhoneModel.DefaultUsecaseToRecipeLookup.TryGetValue(key, out value);
		}
		if (value != null)
		{
			recipeDescriptor = new RecipeDescriptor(usecase);
			foreach (JsonStep step in value.Steps)
			{
				if (step.Type == "STFFILE")
				{
					string fileNameFromUri = Utilities.GetFileNameFromUri(step.FileURL, step.FileName);
					recipeDescriptor.RecipeFile = Path.Combine(Configurations.FileTypeInfos[step.Type].ParentDir, fileNameFromUri);
				}
				else if (step.Type == "FASTBOOTFILE" || step.Type == "ZIPFILE" || step.Type == "QCRECOVERYFILE" || step.Type == "MTRECOVERYFILE" || step.Type == "SPRECOVERYFILE" || step.Type == "RDRECOVERYFILE" || step.Type == "FASTBOOTFILE_SASW" || step.Type == "FASTBOOTFILE_BACKFLASH" || step.Type == "FASTBOOTFILE_IFLASH")
				{
					recipeDescriptor.FlashFiles.Add(Path.Combine(Configurations.FileTypeInfos[step.Type].ParentDir, step.FileName));
					recipeDescriptor.FlashFileType = step.Type;
				}
				else if (step.Type == "QCBLANKFLASHFILE")
				{
					recipeDescriptor.BlankFlashFiles.Add(Path.Combine(Configurations.FileTypeInfos[step.Type].ParentDir, step.FileName));
				}
				else if (step.Type == "XMLFILE")
				{
					string parentFlashFile = GetParentFlashFile(value.Steps, step);
					if (parentFlashFile != string.Empty)
					{
						recipeDescriptor.FlashFileToXmlFileLookup[parentFlashFile] = Path.Combine(parentFlashFile, step.FileName);
					}
				}
				else if (step.Type == "BOOTLOADERFILE")
				{
					recipeDescriptor.Bootloaders.Add(Path.Combine(Configurations.FileTypeInfos[step.Type].ParentDir, step.FileName));
				}
				else if (step.Type == "APKFILE")
				{
					recipeDescriptor.ApkFile = Path.Combine(Configurations.FileTypeInfos[step.Type].ParentDir, step.FileName);
				}
				else if (step.Type == "OTAFILE")
				{
					recipeDescriptor.CqaTestCfg = Path.Combine(Configurations.FileTypeInfos[step.Type].ParentDir, step.FileName);
				}
				else if (step.Type == "KSCONFIG")
				{
					recipeDescriptor.KsConfig = Path.Combine(Configurations.FileTypeInfos[step.Type].ParentDir, step.FileName);
				}
				else if (step.Type == "MTFLASHTOOL" || step.Type == "QCFLASHTOOL" || step.Type == "SPFLASHTOOL" || step.Type == "FBFLASHTOOL" || step.Type == "RDFLASHTOOL")
				{
					recipeDescriptor.FlashTool = Path.Combine(Configurations.FileTypeInfos[step.Type].ParentDir, step.FileName);
					recipeDescriptor.FlashToolType = step.Type;
				}
				else if (step.Type == "BATCHFILE")
				{
					recipeDescriptor.BatFile = Path.Combine(Configurations.FileTypeInfos[step.Type].ParentDir, step.FileName);
				}
				else if (step.Type == "CSVFILE")
				{
					recipeDescriptor.CsvFile = Path.Combine(Configurations.FileTypeInfos[step.Type].ParentDir, step.FileName);
				}
				else if (step.Type == "AUTHFILE")
				{
					recipeDescriptor.AuthFile = Path.Combine(Configurations.FileTypeInfos[step.Type].ParentDir, step.FileName);
				}
				else if (step.Type == "MTPROGTOOL" || step.Type == "CCWRITETOOL" || step.Type == "MMMPROGTOOL" || step.Type == "MOBAPROGTOOL" || step.Type == "JAVAPROGTOOL" || step.Type == "LMPROGTOOL" || step.Type == "P410PROGTOOL" || step.Type == "ZXPROGTOOL" || step.Type == "LQPROGTOOL")
				{
					recipeDescriptor.ProgramTools.Add(Path.Combine(Configurations.FileTypeInfos[step.Type].ParentDir, step.FileName));
					recipeDescriptor.ProgramToolTypes.Add(step.Type);
				}
				else if (step.Type == "MTSIMLOCK")
				{
					recipeDescriptor.SimLockFolder = Path.Combine(Configurations.FileTypeInfos[step.Type].ParentDir, step.FileName);
				}
				else if (step.Type == "MTELABEL")
				{
					recipeDescriptor.ElabelFolder = Path.Combine(Configurations.FileTypeInfos[step.Type].ParentDir, step.FileName);
				}
				else if (step.Type == "GOOGLEKEY")
				{
					recipeDescriptor.GoogleKey = Path.Combine(Configurations.FileTypeInfos[step.Type].ParentDir, step.FileName);
				}
				else if (step.Type == "WIDEVINEKEY")
				{
					recipeDescriptor.WidevineKey = Path.Combine(Configurations.FileTypeInfos[step.Type].ParentDir, step.FileName);
				}
				else if (step.Type == "DEVICECONNECTIMG")
				{
					recipeDescriptor.DevConnImgFile = Path.Combine(Configurations.FileTypeInfos[step.Type].ParentDir, step.FileName);
				}
				else if (step.Type == "DATATEXT")
				{
					recipeDescriptor.DataText = Path.Combine(Configurations.FileTypeInfos[step.Type].ParentDir, step.FileName);
				}
				else if (step.Type == "MODELINFO")
				{
					recipeDescriptor.ModelInfoFile = Path.Combine(Configurations.FileTypeInfos[step.Type].ParentDir, step.FileName);
				}
			}
		}
		return recipeDescriptor;
	}

	private string GetParentFlashFile(List<JsonStep> steps, JsonStep xmlFileStep)
	{
		string result = string.Empty;
		int num = steps.IndexOf(xmlFileStep);
		if (num > 0)
		{
			string type = steps[num - 1].Type;
			if (new List<string> { "FASTBOOTFILE", "FASTBOOTFILE_SASW", "FASTBOOTFILE_BACKFLASH", "FASTBOOTFILE_IFLASH" }.Contains(type))
			{
				result = Path.Combine(Configurations.FileTypeInfos[type].ParentDir, steps[num - 1].FileName);
			}
		}
		return result;
	}

	private bool Contains(List<JsonConfig> jsonConfigs, JsonConfig jsonConfig)
	{
		bool result = false;
		foreach (JsonConfig jsonConfig2 in jsonConfigs)
		{
			if (jsonConfig2.ConfigList.PhoneModel.Matrix.InternalName == jsonConfig.ConfigList.PhoneModel.Matrix.InternalName && jsonConfig2.ConfigList.PhoneModel.Name == jsonConfig.ConfigList.PhoneModel.Name)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private bool Contains(List<JsonPhoneModel> jsonPhoneModels, JsonPhoneModel jsonPhoneModel)
	{
		bool result = false;
		foreach (JsonPhoneModel jsonPhoneModel2 in jsonPhoneModels)
		{
			if (jsonPhoneModel2.Matrix.InternalName == jsonPhoneModel.Matrix.InternalName && jsonPhoneModel2.Name == jsonPhoneModel.Name)
			{
				result = true;
				break;
			}
		}
		return result;
	}
}
