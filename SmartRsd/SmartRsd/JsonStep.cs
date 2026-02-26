using System;
using System.IO;

namespace SmartRsd;

public class JsonStep
{
	public const string STF_FILE = "STFFILE";

	public const string FASTBOOT_FILE = "FASTBOOTFILE";

	public const string FASTBOOTFILE_SASW = "FASTBOOTFILE_SASW";

	public const string FASTBOOTFILE_BACKFLASH = "FASTBOOTFILE_BACKFLASH";

	public const string FASTBOOTFILE_IFLASH = "FASTBOOTFILE_IFLASH";

	public const string XML_FILE = "XMLFILE";

	public const string BOOTLOADER_FILE = "BOOTLOADERFILE";

	public const string QC_RECOVERY_FILE = "QCRECOVERYFILE";

	public const string MT_RECOVERY_FILE = "MTRECOVERYFILE";

	public const string SP_RECOVERY_FILE = "SPRECOVERYFILE";

	public const string RD_RECOVERY_FILE = "RDRECOVERYFILE";

	public const string QC_BLANKFLASH_FILE = "QCBLANKFLASHFILE";

	public const string FASTBOOT_FLASH_TOOL = "FBFLASHTOOL";

	public const string MTEK_FLASH_TOOL = "MTFLASHTOOL";

	public const string QCOM_FLASH_TOOL = "QCFLASHTOOL";

	public const string SP_FLASH_TOOL = "SPFLASHTOOL";

	public const string RD_FLASH_TOOL = "RDFLASHTOOL";

	public const string COUNTRY_CODE_WRITE_TOOL = "CCWRITETOOL";

	public const string MTEK_PROGRAM_TOOL = "MTPROGTOOL";

	public const string MMM_PROGRAM_TOOL = "MMMPROGTOOL";

	public const string MOBA_PROGRAM_TOOL = "MOBAPROGTOOL";

	public const string JAVA_PROGRAM_TOOL = "JAVAPROGTOOL";

	public const string LM_PROGRAM_TOOL = "LMPROGTOOL";

	public const string P410_PROGRAM_TOOL = "P410PROGTOOL";

	public const string ZX_PROGRAM_TOOL = "ZXPROGTOOL";

	public const string LQ_PROGRAM_TOOL = "LQPROGTOOL";

	public const string KILLSWITCH_CONFIG_FILE = "KSCONFIG";

	public const string BAT_FILE = "BATCHFILE";

	public const string CSV_FILE = "CSVFILE";

	public const string AUTH_FILE = "AUTHFILE";

	public const string ZIP_FILE = "ZIPFILE";

	public const string APK_FILE = "APKFILE";

	public const string OTA_FILE = "OTAFILE";

	public const string MTEK_SIM_LOCK = "MTSIMLOCK";

	public const string MTEK_ELABEL = "MTELABEL";

	public const string GOOGLE_KEY = "GOOGLEKEY";

	public const string WIDEVINE_KEY = "WIDEVINEKEY";

	public const string DATA_TEXT = "DATATEXT";

	public const string MODEL_INFO = "MODELINFO";

	public const string VIDPID_LIST = "VIDPIDLIST";

	public const string SAME_FAMILY = "SAMEFAMILY";

	public const string DEV_CONN_IMG = "DEVICECONNECTIMG";

	public string Type { get; private set; }

	public string FileName { get; private set; }

	public string FileURL { get; private set; }

	public bool CreateFolder { get; private set; }

	public JsonRecipe Recipe { get; private set; }

	private string TAG => GetType().FullName;

	public JsonStep(StepDef step, JsonRecipe recipe)
	{
		Recipe = recipe;
		Type = ((step.type != null) ? step.type : string.Empty).ToUpper().Trim();
		bool fullyParsed = Recipe.RecipeGroup.Model.Matrix.FullyParsed;
		if (Type.Length == 0 && fullyParsed)
		{
			Smart.Log.Error(TAG, string.Format("{0} entry is empty missing [recipe {1}] [PhoneModel {2}] [Carrier {3}]", "type", recipe.Id, recipe.RecipeGroup.Model.Name, recipe.RecipeGroup.Model.Matrix.InternalName));
		}
		FileName = ((step.fileName != null) ? step.fileName : string.Empty).Trim();
		if (FileName.Length == 0 && fullyParsed)
		{
			Smart.Log.Error(TAG, string.Format("{0} is empty or missing [recipe {1}] [PhoneModel {2}] [Carrier {3}]", "fileName", recipe.Id, recipe.RecipeGroup.Model.Name, recipe.RecipeGroup.Model.Matrix.InternalName));
		}
		FileURL = ((step.fileURL != null) ? step.fileURL : string.Empty).Trim().Replace(" ", string.Empty);
		if (FileURL.Length == 0 && Type != "XMLFILE" && Type != "IGNORE" && fullyParsed)
		{
			Smart.Log.Debug(TAG, $"fileURL is empty or missing for file {FileName} [recipe {recipe.Id}] [PhoneModel {recipe.RecipeGroup.Model.Name}] [Carrier {recipe.RecipeGroup.Model.Matrix.InternalName}]");
		}
		CreateFolder = step.createFolder != null && Convert.ToBoolean(step.createFolder);
		if (fullyParsed && JsonMatrixParser.Instance.Account != null && JsonMatrixParser.Instance.Account.UseCaseAllowed(recipe.Usecase) && Configurations.FileTypeInfos.ContainsKey(Type) && Type.Length > 0 && Type != "XMLFILE" && FileURL.Length > 0 && FileName.Length > 0)
		{
			string key = Path.Combine(path2: (!(Type == "STFFILE")) ? FileName : Utilities.GetFileNameFromUri(FileURL, FileName), path1: Configurations.FileTypeInfos[Type].ParentDir);
			if (!JsonMatrixParser.Instance.FileNameToDownloadInfoLookup.ContainsKey(key))
			{
				string carrier = Recipe.RecipeGroup.Model.Matrix.Carrier;
				string mSMarketName = Recipe.RecipeGroup.Model.MSMarketName;
				DownloadInfo value = new DownloadInfo(Type, FileURL, CreateFolder, reserved: false, string.Empty, mSMarketName, carrier);
				JsonMatrixParser.Instance.FileNameToDownloadInfoLookup.Add(key, value);
			}
		}
	}
}
