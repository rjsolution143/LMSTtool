using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ISmart;

namespace SmartRsd;

public class JsonDriverInfoParser
{
	public const string MTEK_FLASH_TOOL_1 = "MTFLASHTOOL_1";

	public const string MTEK_FLASH_TOOL_2 = "MTFLASHTOOL_2";

	public const string QCOM_FLASH_TOOL_1 = "QCFLASHTOOL_1";

	public const string CIT_ANIMATION_FILE = "CITANIMATIONS";

	public const string USB_DRIVER = "USBDRIVER";

	public const string USB_DRIVER_INFO = "USBDRIVERINFO";

	public const string XLATE_JSON_FILE = "XLATEJSON";

	public const string HW_TOKEN = "HWTOKEN";

	public const string APK_TYPE = "APKTYPE";

	public const string PCBA_FILE = "PCBAFILE";

	public const string AUTOGEN_PCBA_FILE = "AUTOGENPCBAFILE";

	public const string SALEMODEL_MATRIX = "SALEMODELMATRIX";

	public const string MTMCOUNTRY_MATRIX = "MTMCOUNTRYMATRIX";

	public const string SALEMODEL_MATRIX_SUPPLEMENT = "SALEMODELMATRIXSUP";

	public const string NON_MOBILE_DEVICE_LIST = "NONMOBILEDEVLIST";

	public const string FSB_ICON_FILES = "FSBICONS";

	public const string FSB_SALES_MODEL = "FSB_SALES_MODEL";

	public const string FSB_REPAIR_CODE_PRIORITY = "FSB_REPAIR_CODE_PRIORITY";

	public const string SHELL_RESP_UPDATE = "SHELL_RESP_UPDATE";

	public const string PROG_TOOL_INFO = "PROGTOOLINFO";

	public const string PKI_KEY_TYPES = "PKIKEYTYPES";

	public const string COLOUR = "COLOUR";

	public const string EXECUTABLE = "EXECUTABLE";

	public const string TROUBLESHOOT_INFO = "TROUBLESHOOTINFO";

	public const string TROUBLESHOOT_TRANS = "TROUBLESHOOTTRANS";

	private string[] mUseCaseNames = Enum.GetNames(typeof(UseCase));

	private string[] mDeviceModes = Enum.GetNames(typeof(DeviceMode));

	private static JsonDriverInfoParser sThis;

	public Dictionary<string, string> FileTypeToUniqueItemLookUp { get; private set; } = new Dictionary<string, string>();


	public List<string> GlobalUseCaseNames { get; private set; } = new List<string>();


	public static bool ContentChanged { get; set; }

	public static JsonDriverInfoParser Instance
	{
		get
		{
			if (ContentChanged)
			{
				ContentChanged = false;
				sThis = null;
			}
			if (sThis == null)
			{
				sThis = new JsonDriverInfoParser();
			}
			return sThis;
		}
	}

	private string TAG => GetType().FullName;

	private void Parse()
	{
		FileTypeToUniqueItemLookUp.Clear();
		string driverName = GetDriverName();
		if (driverName == string.Empty)
		{
			Smart.Log.Error(TAG, string.Format("There is no {0} json file.", "USBDRIVERINFO".ToString()));
			return;
		}
		string text = Utilities.AesDecryptFile(driverName, "USBDRIVERINFO");
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		dynamic val = Smart.Json.Load(text);
		foreach (dynamic item in val.driverinformation)
		{
			string text2;
			string text4;
			bool createFolder;
			string input;
			string text3;
			try
			{
				text2 = item.file_type;
				text3 = item.filename;
				text4 = item.extranet_url;
				createFolder = item.create_folder;
				input = text3.ToLower();
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, $"Exception while parsing {driverName}. ErrorMsg: {ex.Message}");
				continue;
			}
			if (!Configurations.FileTypeInfos.ContainsKey(text2))
			{
				continue;
			}
			string localFileExtensions = Configurations.FileTypeInfos[text2].LocalFileExtensions;
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text3);
			if (localFileExtensions != string.Empty && !localFileExtensions.Contains(";"))
			{
				text3 = fileNameWithoutExtension + localFileExtensions;
			}
			string text5 = Path.Combine(Configurations.FileTypeInfos[text2].ParentDir, text3);
			JsonMatrixParser.Instance.FileNameToDownloadInfoLookup[text5] = new DownloadInfo(text2, text4, createFolder, reserved: true);
			switch (text2)
			{
			case "STFFILE":
			{
				string[] array = mUseCaseNames;
				foreach (string text7 in array)
				{
					if (new Regex(text7.ToLower() + "_v\\d*").IsMatch(input))
					{
						text3 = Path.GetFileName(text4);
						text5 = Path.Combine(Configurations.FileTypeInfos[text2].ParentDir, text3);
						JsonMatrixParser.Instance.FileNameToDownloadInfoLookup[text5] = new DownloadInfo(text2, text4, createFolder, reserved: true);
						FileTypeToUniqueItemLookUp[text7] = text5;
						if (!GlobalUseCaseNames.Contains(text7))
						{
							GlobalUseCaseNames.Add(text7);
						}
						break;
					}
				}
				break;
			}
			case "APKTYPE":
				AddItemToLookup(text5, fileNameWithoutExtension, "apk");
				break;
			case "DEVICECONNECTIMG":
			{
				string fileNameBase;
				string languageCodeFromImgFileName = Utilities.GetLanguageCodeFromImgFileName(text3, out fileNameBase);
				if (fileNameBase.StartsWith("default"))
				{
					FileTypeToUniqueItemLookUp[text2 + languageCodeFromImgFileName + "default"] = text5;
					break;
				}
				bool flag = false;
				string[] array = mDeviceModes;
				foreach (string text8 in array)
				{
					if (fileNameBase.StartsWith(text8.ToLower()))
					{
						FileTypeToUniqueItemLookUp[text2 + languageCodeFromImgFileName + text8] = text5;
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
				array = mUseCaseNames;
				foreach (string text9 in array)
				{
					if (fileNameBase.StartsWith(text9.ToLower()))
					{
						FileTypeToUniqueItemLookUp[text2 + languageCodeFromImgFileName + text9] = text5;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					FileTypeToUniqueItemLookUp[text2 + languageCodeFromImgFileName + fileNameBase] = text5;
				}
				break;
			}
			case "COLOUR":
				AddItemToLookup(text5, fileNameWithoutExtension, string.Empty);
				break;
			case "EXECUTABLE":
				AddItemToLookup(text5, fileNameWithoutExtension, "exe");
				break;
			case "FBFLASHTOOL":
			{
				string text6 = string.Empty;
				if (text3.ToLower().StartsWith("devgroup"))
				{
					text6 = text3.Split(new char[1] { '_' })[0].ToLower();
				}
				FileTypeToUniqueItemLookUp[text6 + text2] = text5;
				break;
			}
			default:
				FileTypeToUniqueItemLookUp[text2] = text5;
				break;
			}
		}
	}

	private JsonDriverInfoParser()
	{
		Parse();
	}

	private static string GetDriverName()
	{
		string result = string.Empty;
		string[] files = Directory.GetFiles(Configurations.UsbDriverPath, "DriverInfo_*.json");
		if (files.Length != 0)
		{
			result = files[0];
		}
		return result;
	}

	private void AddItemToLookup(string filePathName, string fileNameWithoutExt, string subkey)
	{
		string text = fileNameWithoutExt.Split(new char[1] { '_' })[0].ToLower() + subkey;
		FileTypeToUniqueItemLookUp[text] = filePathName;
		Smart.Log.Info(TAG, "key:" + text + " value:" + filePathName);
	}
}
