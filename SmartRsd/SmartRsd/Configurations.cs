using System;
using System.Collections.Generic;
using System.IO;
using ISmart;

namespace SmartRsd;

public static class Configurations
{
	public static RsdServerType ServerType = (RsdServerType)0;

	public const string IncompleteTag = "UnzipFailedTag.txt";

	public const long SMALL_FILE_SIZE = 102400000L;

	private static string mCurrentDir = string.Empty;

	private static string mFirmwareDriveName = string.Empty;

	private static string mMacId = string.Empty;

	private static StationDescriptor mStationDescriptor = StationDescriptor.Empty;

	private static string mHardwareId = string.Empty;

	private static string mXmlFirmwarePath0 = string.Empty;

	private static string mXmlFirmwarePath = string.Empty;

	private static string mZipFirmwarePath = string.Empty;

	private static string mRecoveryFilePath = string.Empty;

	private static string mSharedFirmwarePath0 = string.Empty;

	private const string STATION_ID = "StationId.enc";

	private static Dictionary<string, FileTypeInfo> mFileTypeToFileTypeInfo = new Dictionary<string, FileTypeInfo>();

	private static StationInfo mStationInfo = new StationInfo(Path.Combine(Smart.File.CommonStorageDir, "StationId.enc"));

	private static Dictionary<RsdServerType, Dictionary<string, string>> mServerToEndpointUrls;

	private static Dictionary<string, string> mSecureHostToGenerateSignedUrls;

	public const string DefaultLanguageCode = "en";

	public static string BucketName { get; set; }

	public static string RegionName { get; set; }

	public static string AWSAccessKey { get; set; }

	public static string AWSSecretKey { get; set; }

	public static bool FirmwareSharingActive { get; set; } = false;


	public static string AutoAddCarriers { get; set; } = "yes";


	public static string OfflineEnabled
	{
		get
		{
			return mStationInfo.OfflineEnabled;
		}
		set
		{
			mStationInfo.OfflineEnabled = value.Trim();
		}
	}

	public static string GeneratePdf { get; set; } = "no";


	public static Dictionary<string, string> EndPointURLs => mServerToEndpointUrls[ServerType];

	public static Dictionary<string, string> SecureHostToGenerateSignedURLs => mSecureHostToGenerateSignedUrls;

	public static string SoftwareVersion => Smart.App.Version;

	public static string UsersJsonFile
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			if ((int)ServerType == 0)
			{
				return Path.Combine(Smart.File.CommonStorageDir, "Users.json");
			}
			return Path.Combine(Smart.File.CommonStorageDir, "Users-Trg.json");
		}
	}

	public static string UserQualityFile => Path.Combine(Smart.File.CommonStorageDir, "userquality.json");

	public static string UpgradeLogPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "UpgradeLogs\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string ErrorUpgradeLogPath
	{
		get
		{
			string text = Path.Combine(UpgradeLogPath, "ErrorLogs\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string MQSLogPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "MQSLogs\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string ErrorMQSLogPath
	{
		get
		{
			string text = Path.Combine(MQSLogPath, "ErrorLogs\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string XmlLogPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "XmlLogs\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string ESimLogPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "ESimLogs\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string MatrixPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "Matrices\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string AllMatrixPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "AllMatrices\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string RecipePath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "Recipes\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string XmlFirmwarePath0
	{
		get
		{
			if (mXmlFirmwarePath0 == string.Empty)
			{
				mXmlFirmwarePath0 = Path.Combine((mStationInfo.AlternateAppPath == "no") ? Smart.File.CommonStorageDir : mStationInfo.AlternateAppPath, "XmlFirmwares\\");
			}
			return mXmlFirmwarePath0;
		}
	}

	public static string XmlFirmwarePath
	{
		get
		{
			if (mXmlFirmwarePath == string.Empty)
			{
				mXmlFirmwarePath = SharedFirmwarePath;
				if (!Directory.Exists(mXmlFirmwarePath))
				{
					Directory.CreateDirectory(mXmlFirmwarePath);
				}
			}
			return mXmlFirmwarePath;
		}
	}

	public static string SharedFirmwarePath
	{
		get
		{
			return mStationInfo.SharedFirmwarePath;
		}
		set
		{
			mStationInfo.SharedFirmwarePath = value.Trim();
		}
	}

	public static string SharedFirmwarePath0
	{
		get
		{
			if (mSharedFirmwarePath0 == string.Empty)
			{
				mSharedFirmwarePath0 = Path.Combine((mStationInfo.AlternateAppPath == "no") ? Path.GetDirectoryName(Smart.File.CommonStorageDir) : mStationInfo.AlternateAppPath, "SharedFirmwares\\");
			}
			return mSharedFirmwarePath0;
		}
	}

	public static string ZipFirmwarePath
	{
		get
		{
			if (mZipFirmwarePath == string.Empty)
			{
				mZipFirmwarePath = Path.Combine((mStationInfo.AlternateAppPath == "no") ? Smart.File.CommonStorageDir : mStationInfo.AlternateAppPath, "ZipFirmwares\\");
				if (!Directory.Exists(mZipFirmwarePath))
				{
					Directory.CreateDirectory(mZipFirmwarePath);
				}
			}
			return mZipFirmwarePath;
		}
	}

	public static string ApkFilePath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "ApkFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string UsbDriverPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "UsbDrivers\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string TemporaryPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "Temp\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string ModelImagePath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "ModelImages\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string ModelInfoPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "ModelInfo\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string CqaTestCfgPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "CqaTestConfig\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string RecoveryFilePath
	{
		get
		{
			if (mRecoveryFilePath == string.Empty)
			{
				mRecoveryFilePath = Path.Combine((mStationInfo.AlternateAppPath == "no") ? Smart.File.CommonStorageDir : mStationInfo.AlternateAppPath, "RecoveryFiles\\");
				if (!Directory.Exists(mRecoveryFilePath))
				{
					Directory.CreateDirectory(mRecoveryFilePath);
				}
			}
			return mRecoveryFilePath;
		}
	}

	public static string BlankFlashFilePath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "BlankFlashFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string FlashToolPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "FlashTools\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string XlatePath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "Xlate\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string KSConfigPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "KSConfig\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string HwTokenPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "HwToken\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string BatFilePath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "BatFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string CSVFilePath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "CsvFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string AuthFilePath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "AuthFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string CitPrinFiletPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "CitPrintFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string RefurbInfoPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "KSPrintFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string SimLockPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "SimLocks\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string ElabelPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "Elabels\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string DevConnImgFilePath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "DevConnImgFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string ColourFilePath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "ColourFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string ExecutablePath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "Executables\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string CurrentDirectory
	{
		get
		{
			if (mCurrentDir.Length == 0)
			{
				mCurrentDir = Directory.GetCurrentDirectory() + "\\";
			}
			return mCurrentDir;
		}
	}

	public static string FakeMacId => "5D9A1E";

	public static string FirmwareDriveName
	{
		get
		{
			if (mFirmwareDriveName.Length == 0)
			{
				mFirmwareDriveName = XmlFirmwarePath.Substring(0, 3);
			}
			return mFirmwareDriveName;
		}
	}

	public static StationDescriptor StationDescriptor
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			if (((object)(StationDescriptor)(ref mStationDescriptor)).Equals((object?)StationDescriptor.Empty))
			{
				mStationDescriptor = new StationDescriptor(mStationInfo.StationId, Environment.MachineName, string.Empty);
			}
			return mStationDescriptor;
		}
		set
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			mStationInfo.StationId = ((object)(StationDescriptor)(ref value)).ToString();
			mStationDescriptor = value;
		}
	}

	public static string StationHardwareId
	{
		get
		{
			if (mHardwareId == string.Empty)
			{
				mHardwareId = Utilities.GetHwId();
			}
			return mHardwareId;
		}
	}

	public static string KeepFirmwares => mStationInfo.KeepFirmwares;

	public static Dictionary<string, FileTypeInfo> FileTypeInfos => mFileTypeToFileTypeInfo;

	public static string OptionsFile => Path.Combine(Smart.File.CommonStorageDir, "options.json");

	public static string PowerTestLogPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "PowerTestLogs\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string PcbaConfigPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "PcbaConfig\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string GoogleKeyPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "GoogleKeys\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string WidevineKeyPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "WidevineKeys\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string DataTextPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "DataTexts\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string RegistryPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "RegistryFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string LabelPrintFilePath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "LabelPrintFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string TroubleShootFilePath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "TroubleShootFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	private static string TAG => "SmartRsd.Configurations";

	public static string FSBPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "FSBFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string FsbLogPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "FSBLogs\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string ErrorFsbLogPath
	{
		get
		{
			string text = Path.Combine(FsbLogPath, "ErrorLogs\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string FsbResourceFilesPath
	{
		get
		{
			string text = Path.Combine(Smart.File.CommonStorageDir, "FSBResources\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static void Initialize()
	{
		mFileTypeToFileTypeInfo.Clear();
		mFileTypeToFileTypeInfo.Add("STFFILE", new FileTypeInfo(RecipePath, isFolder: false, unzipped: false, encrypted: true, 11000L, ".json"));
		mFileTypeToFileTypeInfo.Add("FASTBOOTFILE", new FileTypeInfo(XmlFirmwarePath, isFolder: true, unzipped: true, encrypted: false, 0L, string.Empty));
		mFileTypeToFileTypeInfo.Add("FASTBOOTFILE_SASW", new FileTypeInfo(XmlFirmwarePath, isFolder: true, unzipped: true, encrypted: false, 0L, string.Empty));
		mFileTypeToFileTypeInfo.Add("FASTBOOTFILE_BACKFLASH", new FileTypeInfo(XmlFirmwarePath, isFolder: true, unzipped: true, encrypted: false, 0L, string.Empty));
		mFileTypeToFileTypeInfo.Add("FASTBOOTFILE_IFLASH", new FileTypeInfo(XmlFirmwarePath, isFolder: true, unzipped: true, encrypted: false, 0L, string.Empty));
		mFileTypeToFileTypeInfo.Add("XMLFILE", new FileTypeInfo(string.Empty, isFolder: false, unzipped: false, encrypted: false, 1000L, ".xml"));
		mFileTypeToFileTypeInfo.Add("BOOTLOADERFILE", new FileTypeInfo(XmlFirmwarePath, isFolder: true, unzipped: true, encrypted: false, 0L, string.Empty));
		mFileTypeToFileTypeInfo.Add("APKFILE", new FileTypeInfo(ApkFilePath, isFolder: false, unzipped: true, encrypted: false, 300000L, ".apk"));
		mFileTypeToFileTypeInfo.Add("ZIPFILE", new FileTypeInfo(ZipFirmwarePath, isFolder: false, unzipped: false, encrypted: false, 0L, string.Empty));
		mFileTypeToFileTypeInfo.Add("QCRECOVERYFILE", new FileTypeInfo(RecoveryFilePath, isFolder: true, unzipped: true, encrypted: false, 0L, string.Empty));
		mFileTypeToFileTypeInfo.Add("MTRECOVERYFILE", new FileTypeInfo(RecoveryFilePath, isFolder: true, unzipped: true, encrypted: false, 0L, string.Empty));
		mFileTypeToFileTypeInfo.Add("SPRECOVERYFILE", new FileTypeInfo(RecoveryFilePath, isFolder: true, unzipped: true, encrypted: false, 0L, string.Empty));
		mFileTypeToFileTypeInfo.Add("RDRECOVERYFILE", new FileTypeInfo(RecoveryFilePath, isFolder: true, unzipped: true, encrypted: false, 0L, string.Empty));
		mFileTypeToFileTypeInfo.Add("QCBLANKFLASHFILE", new FileTypeInfo(BlankFlashFilePath, isFolder: true, unzipped: true, encrypted: false, 5300000L, string.Empty));
		mFileTypeToFileTypeInfo.Add("OTAFILE", new FileTypeInfo(CqaTestCfgPath, isFolder: false, unzipped: true, encrypted: false, 1000L, string.Empty));
		mFileTypeToFileTypeInfo.Add("KSCONFIG", new FileTypeInfo(KSConfigPath, isFolder: false, unzipped: true, encrypted: false, 32000L, string.Empty));
		mFileTypeToFileTypeInfo.Add("MTFLASHTOOL", new FileTypeInfo(FlashToolPath, isFolder: true, unzipped: true, encrypted: false, 44271782L, string.Empty));
		mFileTypeToFileTypeInfo.Add("QCFLASHTOOL", new FileTypeInfo(FlashToolPath, isFolder: true, unzipped: true, encrypted: false, 9101806L, string.Empty));
		mFileTypeToFileTypeInfo.Add("SPFLASHTOOL", new FileTypeInfo(FlashToolPath, isFolder: true, unzipped: true, encrypted: false, 5000000L, string.Empty));
		mFileTypeToFileTypeInfo.Add("FBFLASHTOOL", new FileTypeInfo(FlashToolPath, isFolder: true, unzipped: true, encrypted: false, 3376667L, string.Empty));
		mFileTypeToFileTypeInfo.Add("RDFLASHTOOL", new FileTypeInfo(FlashToolPath, isFolder: true, unzipped: true, encrypted: false, 13000000L, string.Empty));
		mFileTypeToFileTypeInfo.Add("MTPROGTOOL", new FileTypeInfo(FlashToolPath, isFolder: true, unzipped: true, encrypted: false, 33000L, string.Empty));
		mFileTypeToFileTypeInfo.Add("MTSIMLOCK", new FileTypeInfo(SimLockPath, isFolder: true, unzipped: true, encrypted: false, 18000L, string.Empty));
		mFileTypeToFileTypeInfo.Add("MTELABEL", new FileTypeInfo(ElabelPath, isFolder: true, unzipped: true, encrypted: false, 8000000L, string.Empty));
		mFileTypeToFileTypeInfo.Add("MMMPROGTOOL", new FileTypeInfo(FlashToolPath, isFolder: true, unzipped: true, encrypted: false, 41000000L, string.Empty));
		mFileTypeToFileTypeInfo.Add("MOBAPROGTOOL", new FileTypeInfo(FlashToolPath, isFolder: true, unzipped: true, encrypted: false, 4900000L, string.Empty));
		mFileTypeToFileTypeInfo.Add("JAVAPROGTOOL", new FileTypeInfo(FlashToolPath, isFolder: true, unzipped: true, encrypted: false, 2450000L, string.Empty));
		mFileTypeToFileTypeInfo.Add("LMPROGTOOL", new FileTypeInfo(FlashToolPath, isFolder: true, unzipped: true, encrypted: false, 9500000L, string.Empty));
		mFileTypeToFileTypeInfo.Add("P410PROGTOOL", new FileTypeInfo(FlashToolPath, isFolder: true, unzipped: true, encrypted: false, 15200000L, string.Empty));
		mFileTypeToFileTypeInfo.Add("ZXPROGTOOL", new FileTypeInfo(FlashToolPath, isFolder: true, unzipped: true, encrypted: false, 2000000L, string.Empty));
		mFileTypeToFileTypeInfo.Add("LQPROGTOOL", new FileTypeInfo(FlashToolPath, isFolder: true, unzipped: true, encrypted: false, 2000000L, string.Empty));
		mFileTypeToFileTypeInfo.Add("BATCHFILE", new FileTypeInfo(BatFilePath, isFolder: false, unzipped: true, encrypted: false, 7000L, ".bat"));
		mFileTypeToFileTypeInfo.Add("CSVFILE", new FileTypeInfo(CSVFilePath, isFolder: false, unzipped: true, encrypted: false, 25000L, ".csv"));
		mFileTypeToFileTypeInfo.Add("AUTHFILE", new FileTypeInfo(AuthFilePath, isFolder: false, unzipped: true, encrypted: false, 3000L, ".auth"));
		mFileTypeToFileTypeInfo.Add("CCWRITETOOL", new FileTypeInfo(FlashToolPath, isFolder: true, unzipped: true, encrypted: false, 44000000L, string.Empty));
		mFileTypeToFileTypeInfo.Add("GOOGLEKEY", new FileTypeInfo(GoogleKeyPath, isFolder: false, unzipped: false, encrypted: false, 16000L, ".bin"));
		mFileTypeToFileTypeInfo.Add("WIDEVINEKEY", new FileTypeInfo(WidevineKeyPath, isFolder: false, unzipped: false, encrypted: false, 2800L, ".bin"));
		mFileTypeToFileTypeInfo.Add("DEVICECONNECTIMG", new FileTypeInfo(DevConnImgFilePath, isFolder: false, unzipped: false, encrypted: false, 70000L, ".jpg;.png;.gif"));
		mFileTypeToFileTypeInfo.Add("DATATEXT", new FileTypeInfo(DataTextPath, isFolder: false, unzipped: true, encrypted: false, 10000L, ".txt"));
		mFileTypeToFileTypeInfo.Add("MODELINFO", new FileTypeInfo(LabelPrintFilePath, isFolder: false, unzipped: false, encrypted: false, 9000L, ".txt"));
		mFileTypeToFileTypeInfo.Add("VIDPIDLIST", new FileTypeInfo(RegistryPath, isFolder: false, unzipped: false, encrypted: false, 9000L, ".txt"));
		mFileTypeToFileTypeInfo.Add("SAMEFAMILY", new FileTypeInfo(RegistryPath, isFolder: false, unzipped: false, encrypted: false, 9000L, ".txt"));
		mFileTypeToFileTypeInfo.Add("DEVICEIMAGEFILE", new FileTypeInfo(ModelImagePath, isFolder: false, unzipped: true, encrypted: false, 70000L, ".jpg;.png"));
		mFileTypeToFileTypeInfo.Add("DEVICECONFIGFILE", new FileTypeInfo(ModelInfoPath, isFolder: false, unzipped: false, encrypted: false, 22000L, ".json"));
		mFileTypeToFileTypeInfo.Add("CITANIMATIONS", new FileTypeInfo(CqaTestCfgPath, isFolder: true, unzipped: true, encrypted: false, 430000L, string.Empty));
		mFileTypeToFileTypeInfo.Add("MTFLASHTOOL_1", new FileTypeInfo(FlashToolPath, isFolder: true, unzipped: true, encrypted: false, 41751973L, string.Empty));
		mFileTypeToFileTypeInfo.Add("MTFLASHTOOL_2", new FileTypeInfo(FlashToolPath, isFolder: true, unzipped: true, encrypted: false, 45945591L, string.Empty));
		mFileTypeToFileTypeInfo.Add("QCFLASHTOOL_1", new FileTypeInfo(FlashToolPath, isFolder: true, unzipped: true, encrypted: false, 9883634L, string.Empty));
		mFileTypeToFileTypeInfo.Add("USBDRIVER", new FileTypeInfo(UsbDriverPath, isFolder: false, unzipped: false, encrypted: false, 9000000L, ".msi;.exe;.zip"));
		mFileTypeToFileTypeInfo.Add("USBDRIVERINFO", new FileTypeInfo(UsbDriverPath, isFolder: false, unzipped: false, encrypted: true, 12000L, ".json"));
		mFileTypeToFileTypeInfo.Add("XLATEJSON", new FileTypeInfo(XlatePath, isFolder: false, unzipped: false, encrypted: false, 50000L, ".json"));
		mFileTypeToFileTypeInfo.Add("HWTOKEN", new FileTypeInfo(HwTokenPath, isFolder: true, unzipped: true, encrypted: false, 1303799L, string.Empty));
		mFileTypeToFileTypeInfo.Add("APKTYPE", new FileTypeInfo(ApkFilePath, isFolder: false, unzipped: false, encrypted: false, 400000L, ".apk"));
		mFileTypeToFileTypeInfo.Add("PCBAFILE", new FileTypeInfo(PcbaConfigPath, isFolder: false, unzipped: false, encrypted: true, 900000L, ".scs"));
		mFileTypeToFileTypeInfo.Add("AUTOGENPCBAFILE", new FileTypeInfo(PcbaConfigPath, isFolder: false, unzipped: false, encrypted: true, 900000L, ".scs"));
		mFileTypeToFileTypeInfo.Add("SALEMODELMATRIX", new FileTypeInfo(PcbaConfigPath, isFolder: false, unzipped: false, encrypted: true, 500000L, ".scs"));
		mFileTypeToFileTypeInfo.Add("SALEMODELMATRIXSUP", new FileTypeInfo(PcbaConfigPath, isFolder: false, unzipped: false, encrypted: true, 10000L, ".scs"));
		mFileTypeToFileTypeInfo.Add("MTMCOUNTRYMATRIX", new FileTypeInfo(PcbaConfigPath, isFolder: false, unzipped: false, encrypted: true, 1000L, ".scs"));
		mFileTypeToFileTypeInfo.Add("NONMOBILEDEVLIST", new FileTypeInfo(PcbaConfigPath, isFolder: false, unzipped: false, encrypted: true, 1067L, ".scs"));
		mFileTypeToFileTypeInfo.Add("FSBICONS", new FileTypeInfo(FsbResourceFilesPath, isFolder: true, unzipped: true, encrypted: false, 75356L, string.Empty));
		mFileTypeToFileTypeInfo.Add("FSB_SALES_MODEL", new FileTypeInfo(FsbResourceFilesPath, isFolder: false, unzipped: false, encrypted: true, 127532L, ".scs"));
		mFileTypeToFileTypeInfo.Add("FSB_REPAIR_CODE_PRIORITY", new FileTypeInfo(FsbResourceFilesPath, isFolder: false, unzipped: false, encrypted: true, 2608L, ".scs"));
		mFileTypeToFileTypeInfo.Add("SHELL_RESP_UPDATE", new FileTypeInfo(FlashToolPath, isFolder: false, unzipped: false, encrypted: true, 3000L, ".json"));
		mFileTypeToFileTypeInfo.Add("PROGTOOLINFO", new FileTypeInfo(FlashToolPath, isFolder: false, unzipped: false, encrypted: false, 60L, ".scs"));
		mFileTypeToFileTypeInfo.Add("PKIKEYTYPES", new FileTypeInfo(PcbaConfigPath, isFolder: false, unzipped: false, encrypted: true, 3060L, ".scs"));
		mFileTypeToFileTypeInfo.Add("COLOUR", new FileTypeInfo(ColourFilePath, isFolder: false, unzipped: false, encrypted: false, 27000L, ".jpg;.png;.gif"));
		mFileTypeToFileTypeInfo.Add("EXECUTABLE", new FileTypeInfo(ExecutablePath, isFolder: true, unzipped: true, encrypted: false, 7677L, string.Empty));
		mFileTypeToFileTypeInfo.Add("TROUBLESHOOTINFO", new FileTypeInfo(TroubleShootFilePath, isFolder: false, unzipped: false, encrypted: true, 900000L, ".scs"));
		mFileTypeToFileTypeInfo.Add("TROUBLESHOOTTRANS", new FileTypeInfo(TroubleShootFilePath, isFolder: false, unzipped: false, encrypted: false, 5000L, ".json"));
	}

	public static void UpdateStationIdFile()
	{
		mStationInfo.UpdateStationIdFile();
	}

	static Configurations()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<RsdServerType, Dictionary<string, string>> dictionary = new Dictionary<RsdServerType, Dictionary<string, string>>();
		Dictionary<string, string> obj = new Dictionary<string, string>
		{
			{ "Ping", "https://ebiz-esb.motorola.com/SRPPing" },
			{ "Login", "https://ebiz-esb.motorola.com/SRPLogin" },
			{ "ClientVersionVerification", "https://ebiz-esb.motorola.com/SRPMLSTClientVersionVerification" },
			{ "MatrixVerification", "https://ebiz-esb.motorola.com/SRPMLSTMatrixVerificationV1" },
			{ "AllMatrixVerification", "https://ebiz-esb.motorola.com/SRPMLSTAllCarrierMatrixVerificationServiceV1" },
			{ "LogFileUpload", "https://ebiz-esb.motorola.com/SRPLogFileUpload" },
			{ "LogUploadRealTime", "https://ebiz-esb.motorola.com/SRPRepairLogUploadRealTimeService" },
			{ "GenerateSignedURL", "https://ebiz-esb.motorola.com/SRPGenerateSignedURL" },
			{ "ManageUserJson", "https://ebiz-esb.motorola.com/SRPManageUserJsonService" },
			{ "GamificationPointFetch", "https://ebiz-esb.motorola.com/SRPGamificationPointFetchService" }
		};
		SOATokenType val = (SOATokenType)0;
		obj.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb.motorola.com/LenovoEtokenGenerate/SOA-DEVICE_INFO");
		val = (SOATokenType)1;
		obj.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb.motorola.com/LenovoEtokenGenerate/SOA-SAMESN");
		val = (SOATokenType)2;
		obj.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb.motorola.com/LenovoEtokenGenerate/SOA-MOTO_MACHINE_INFO");
		val = (SOATokenType)3;
		obj.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb.motorola.com/LenovoEtokenGenerate/SOA-SWAP");
		val = (SOATokenType)4;
		obj.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb.motorola.com/LenovoEtokenGenerate/SOA-SNVALIDATE");
		val = (SOATokenType)5;
		obj.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb.motorola.com/LenovoEtokenGenerate/SOA-SYNC");
		val = (SOATokenType)6;
		obj.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb.motorola.com/LenovoEtokenGenerate/SOA-MOTOFOCUS");
		val = (SOATokenType)7;
		obj.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb.motorola.com/LenovoEtokenGenerate/SOA-NEWSN-PRGM");
		val = (SOATokenType)8;
		obj.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb.motorola.com/LenovoEtokenGenerate/SOA-DUALSN");
		val = (SOATokenType)9;
		obj.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb.motorola.com/LenovoEtokenGenerate/SOA-GSN");
		val = (SOATokenType)10;
		obj.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb.motorola.com/LenovoEtokenGenerate/API-CN");
		obj.Add("PushNotification", "https://ebiz-esb.motorola.com/PushNotificationService");
		obj.Add("FSBDateService", "https://ebiz-esb.motorola.com/rsdc/FSB/FetchFSBDateService");
		obj.Add("FSBLogFileUpload", "https://ebiz-esb.motorola.com/rsdc/FSBLogFileUpload/SRPFSBLogFileUploadService");
		obj.Add("FSBLogUploadRealTime", "https://ebiz-esb.motorola.com/SRPFSBLogUploadRealTimeService");
		obj.Add("SerialHistory", "https://ebiz-esb.motorola.com/PhoneHistory/SRPSerialHistoryservice");
		obj.Add("AccountUpdate", "https://ebiz-esb.motorola.com/SRPRSDAccountUpdateService");
		obj.Add("LatestSoftwareHistory", "https://ebiz-esb.motorola.com/SRPLatestSoftwareHistoryService");
		dictionary.Add((RsdServerType)0, obj);
		Dictionary<string, string> obj2 = new Dictionary<string, string>
		{
			{ "Ping", "https://ebiz-esb-test.motorola.com/SRPPing" },
			{ "Login", "https://ebiz-esb-test.motorola.com/SRPLogin" },
			{ "ClientVersionVerification", "https://ebiz-esb-test.motorola.com/SRPMLSTClientVersionVerification" },
			{ "MatrixVerification", "https://ebiz-esb-test.motorola.com/SRPMLSTMatrixVerificationV1" },
			{ "AllMatrixVerification", "https://ebiz-esb-test.motorola.com/SRPMLSTAllCarrierMatrixVerificationServiceV1" },
			{ "LogFileUpload", "https://ebiz-esb-test.motorola.com/SRPLogFileUpload" },
			{ "LogUploadRealTime", "https://ebiz-esb-test.motorola.com/SRPRepairLogUploadRealTimeService" },
			{ "GenerateSignedURL", "https://ebiz-esb-test.motorola.com/SRPGenerateSignedURL" },
			{ "ManageUserJson", "https://ebiz-esb-test.motorola.com/SRPManageUserJsonService" },
			{ "GamificationPointFetch", "https://ebiz-esb-test.motorola.com/SRPGamificationPointFetchService" }
		};
		val = (SOATokenType)0;
		obj2.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb-test.motorola.com/LenovoEtokenGenerate/SOA-DEVICE_INFO");
		val = (SOATokenType)1;
		obj2.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb-test.motorola.com/LenovoEtokenGenerate/SOA-SAMESN");
		val = (SOATokenType)2;
		obj2.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb-test.motorola.com/LenovoEtokenGenerate/SOA-MOTO_MACHINE_INFO");
		val = (SOATokenType)3;
		obj2.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb-test.motorola.com/LenovoEtokenGenerate/SOA-SWAP");
		val = (SOATokenType)4;
		obj2.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb-test.motorola.com/LenovoEtokenGenerate/SOA-SNVALIDATE");
		val = (SOATokenType)5;
		obj2.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb-test.motorola.com/LenovoEtokenGenerate/SOA-SYNC");
		val = (SOATokenType)6;
		obj2.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb-test.motorola.com/LenovoEtokenGenerate/SOA-MOTOFOCUS");
		val = (SOATokenType)7;
		obj2.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb-test.motorola.com/LenovoEtokenGenerate/SOA-NEWSN-PRGM");
		val = (SOATokenType)8;
		obj2.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb-test.motorola.com/LenovoEtokenGenerate/SOA-DUALSN");
		val = (SOATokenType)10;
		obj2.Add(((object)(SOATokenType)(ref val)).ToString(), "https://ebiz-esb-test.motorola.com/LenovoEtokenGenerate/API-CN");
		obj2.Add("PushNotification", "https://ebiz-esb-test.motorola.com/PushNotificationService");
		obj2.Add("FSBDateService", "https://ebiz-esb-test.motorola.com/rsdc/FSB/FetchFSBDateService");
		obj2.Add("FSBLogFileUpload", "https://ebiz-esb-test.motorola.com/rsdc/FSBLogFileUpload/SRPFSBLogFileUploadService");
		obj2.Add("FSBLogUploadRealTime", "https://ebiz-esb-test.motorola.com/SRPFSBLogUploadRealTimeService");
		obj2.Add("SerialHistory", "https://ebiz-esb-test.motorola.com/PhoneHistory/SRPSerialHistoryservice");
		obj2.Add("AccountUpdate", "https://ebiz-esb-test.motorola.com/SRPRSDAccountUpdateService");
		obj2.Add("LatestSoftwareHistory", "https://ebiz-esb-test.motorola.com/SRPLatestSoftwareHistoryService");
		dictionary.Add((RsdServerType)1, obj2);
		mServerToEndpointUrls = dictionary;
		mSecureHostToGenerateSignedUrls = new Dictionary<string, string>
		{
			{ "rsdsecure-cloud.motorola.com", "https://ebiz-esb.motorola.com/SRPGenerateSignedURL" },
			{ "rsddownload-secure.lenovo.com", "https://ebiz-esb.motorola.com/SRPGenerateSignedURL" },
			{ "rsddownload-nonsecure.lenovo.com", "https://ebiz-esb.motorola.com/SRPGenerateSignedURL" },
			{ "rsdsecure-test.motorola.com", "https://ebiz-esb-test.motorola.com/SRPGenerateSignedURL" },
			{ "rsdsecure-test.lenovo.com", "https://ebiz-esb-test.motorola.com/SRPGenerateSignedURL" },
			{ "rsddownload-uat.lenovo.com", "https://ebiz-esb-test.motorola.com/SRPGenerateSignedURL" }
		};
	}
}
