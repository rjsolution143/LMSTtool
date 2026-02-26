using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ISmart;

namespace SmartDevice.Steps;

public class ShellResponse
{
	public enum ShellCmdStatus
	{
		None = 0,
		Connecting = 1,
		Downloading = 2,
		Completed = 3,
		Outputing = 4,
		Authenticating = 5,
		Writing = 6,
		Error = -1
	}

	public enum ShellCmdType
	{
		None,
		MTFLASHTOOL,
		QCFLASHTOOL,
		SPFLASHTOOL,
		QFILFLASHTOOL,
		RDFLASHTOOL,
		MTPROGTOOL,
		MMMPROGTOOL,
		MOBAPROGTOOL,
		JAVAPROGTOOL,
		LMPROGTOOL,
		P410PROGTOOL,
		QCBLANKFLASHFILE,
		ZXPROGTOOL,
		LQPROGTOOL,
		GENERICTOOL
	}

	private List<ShellCmdType> StartWithMatchingShellCmds = new List<ShellCmdType> { ShellCmdType.P410PROGTOOL };

	public static Dictionary<string, ShellCmdType> ExeStringToShellCmdType = new Dictionary<string, ShellCmdType>
	{
		{
			"flash_tool",
			ShellCmdType.MTFLASHTOOL
		},
		{
			"QDowloader",
			ShellCmdType.QCFLASHTOOL
		},
		{
			"QcomDLoader",
			ShellCmdType.QCFLASHTOOL
		},
		{
			"QGDP_Download_Platform",
			ShellCmdType.SPFLASHTOOL
		},
		{
			"QGDP_ASECMD",
			ShellCmdType.SPFLASHTOOL
		},
		{
			"QFIL",
			ShellCmdType.QFILFLASHTOOL
		},
		{
			"ChinoeIMEICMTool",
			ShellCmdType.MTPROGTOOL
		},
		{
			"CmdDloader",
			ShellCmdType.RDFLASHTOOL
		},
		{
			"MMM",
			ShellCmdType.MMMPROGTOOL
		},
		{
			"MotoConsoleTool",
			ShellCmdType.MOBAPROGTOOL
		},
		{
			"WriteIMEITool",
			ShellCmdType.JAVAPROGTOOL
		},
		{
			"LM",
			ShellCmdType.LMPROGTOOL
		},
		{
			"p410_aftersales_imei_tool_forcmd",
			ShellCmdType.P410PROGTOOL
		},
		{
			"p41x_aftersales_imei_tool_forcmd",
			ShellCmdType.P410PROGTOOL
		},
		{
			"qboot",
			ShellCmdType.QCBLANKFLASHFILE
		},
		{
			"ZX",
			ShellCmdType.ZXPROGTOOL
		},
		{
			"LQ",
			ShellCmdType.LQPROGTOOL
		}
	};

	private static Dictionary<string, ShellCmdStatus> MTekFlashResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"% of image data has been sent",
			ShellCmdStatus.Downloading
		},
		{
			"All command exec done!",
			ShellCmdStatus.Completed
		},
		{
			"SearchUSBPortPool failed!",
			ShellCmdStatus.Error
		},
		{
			"Failed to find USB port",
			ShellCmdStatus.Error
		},
		{
			"Connect BROM failed:",
			ShellCmdStatus.Error
		},
		{
			"[BROM] Can not pass bootrom start command! Possibly target power up too early.",
			ShellCmdStatus.Error
		},
		{
			"Failed to Connect DA: S_FT_ENABLE_DRAM_FAIL",
			ShellCmdStatus.Error
		},
		{
			"[EMI] Enable DRAM Failed!",
			ShellCmdStatus.Error
		},
		{
			"Please check your load matches to your target which is to be downloaded.",
			ShellCmdStatus.Error
		},
		{
			"[DA] DA binary file contains an unsupported version in its header! Please ask for help.",
			ShellCmdStatus.Error
		},
		{
			"Failed to Connect DA: S_UNSUPPORTED_VER_OF_DA",
			ShellCmdStatus.Error
		},
		{
			"Exception",
			ShellCmdStatus.Error
		},
		{
			"[H/W] Fail to download DA to baseband chip's internal SRAM!",
			ShellCmdStatus.Error
		},
		{
			"file not found",
			ShellCmdStatus.Error
		},
		{
			"lib DA NOT match",
			ShellCmdStatus.Error
		},
		{
			"The scatter file format is invalid!",
			ShellCmdStatus.Error
		},
		{
			"Error",
			ShellCmdStatus.Error
		},
		{
			"Login Http Api Fail",
			ShellCmdStatus.Error
		},
		{
			"DA Connected",
			ShellCmdStatus.Connecting
		}
	};

	private static Dictionary<string, ShellCmdStatus> QComFlashResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"Status=status_flash_download_percent_",
			ShellCmdStatus.Downloading
		},
		{
			"Status=status_flash_download_end",
			ShellCmdStatus.Completed
		},
		{
			"Status=status_flash_download_failed",
			ShellCmdStatus.Error
		}
	};

	private static Dictionary<string, ShellCmdStatus> SPFlashResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"QGDP:Please Connect Phone...",
			ShellCmdStatus.Connecting
		},
		{
			"QGDP:TestResult=Pass",
			ShellCmdStatus.Completed
		},
		{
			"QGDP:TestResult=FailStop",
			ShellCmdStatus.Error
		}
	};

	private static Dictionary<string, ShellCmdStatus> QFilFlashResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"{percent files transferred",
			ShellCmdStatus.Downloading
		},
		{
			"Download Succeed",
			ShellCmdStatus.Completed
		},
		{
			"Download Fail",
			ShellCmdStatus.Error
		}
	};

	private static Dictionary<string, ShellCmdStatus> RDFlashResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"Connecting",
			ShellCmdStatus.Connecting
		},
		{
			"Downloading...",
			ShellCmdStatus.Downloading
		},
		{
			"DownLoad Passed",
			ShellCmdStatus.Completed
		},
		{
			"[ERROR]",
			ShellCmdStatus.Error
		},
		{
			"DownLoad Failed",
			ShellCmdStatus.Error
		},
		{
			"Failed",
			ShellCmdStatus.Error
		},
		{
			"_RESET_",
			ShellCmdStatus.Completed
		}
	};

	private static Dictionary<string, ShellCmdStatus> MTekProgResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"Please connect device",
			ShellCmdStatus.Connecting
		},
		{
			"Work done",
			ShellCmdStatus.Completed
		},
		{
			"error",
			ShellCmdStatus.Error
		},
		{
			"fail",
			ShellCmdStatus.Error
		},
		{
			"Read IMEI1(",
			ShellCmdStatus.Outputing
		},
		{
			"Read IMEI2(",
			ShellCmdStatus.Outputing
		},
		{
			"Read software version:",
			ShellCmdStatus.Outputing
		},
		{
			"Get model name:",
			ShellCmdStatus.Outputing
		},
		{
			"UnlockCode generated:",
			ShellCmdStatus.Outputing
		},
		{
			"Unlock Key:",
			ShellCmdStatus.Outputing
		},
		{
			"Clear SIMLock Successful",
			ShellCmdStatus.Outputing
		},
		{
			":",
			ShellCmdStatus.Outputing
		},
		{
			"TRUE",
			ShellCmdStatus.Outputing
		},
		{
			"FALSE",
			ShellCmdStatus.Outputing
		},
		{
			"WiFi(NV):",
			ShellCmdStatus.Outputing
		},
		{
			"BT(NV):",
			ShellCmdStatus.Outputing
		},
		{
			"Country Code:",
			ShellCmdStatus.Outputing
		}
	};

	private static Dictionary<string, ShellCmdStatus> MmmProgResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"Motorola Modem Meta Tool Passed",
			ShellCmdStatus.Completed
		},
		{
			"Motorola Modem Meta Tool Failed",
			ShellCmdStatus.Error
		},
		{
			"Motorola Modem Meta Tool Output",
			ShellCmdStatus.Authenticating
		}
	};

	private static Dictionary<string, ShellCmdStatus> MobaProgResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"Cmd Line Overall test result:Passed",
			ShellCmdStatus.Completed
		},
		{
			"Cmd Line Overall test result:Failed",
			ShellCmdStatus.Error
		},
		{
			"Cmd Line Overall test result:Canceled",
			ShellCmdStatus.Error
		},
		{
			"OutputData",
			ShellCmdStatus.Outputing
		}
	};

	private static Dictionary<string, ShellCmdStatus> JavaProgResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"Please plug in your cell phone",
			ShellCmdStatus.Connecting
		},
		{
			"Total Test Result = PASS",
			ShellCmdStatus.Completed
		},
		{
			"Total Test Result = FAIL",
			ShellCmdStatus.Error
		},
		{
			"Total Test Result = CANCEL",
			ShellCmdStatus.Error
		},
		{
			"Read",
			ShellCmdStatus.Outputing
		},
		{
			"Write",
			ShellCmdStatus.Writing
		},
		{
			"Fail",
			ShellCmdStatus.Error
		}
	};

	private static Dictionary<string, ShellCmdStatus> LMProgResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"error",
			ShellCmdStatus.Error
		},
		{
			"fail",
			ShellCmdStatus.Error
		},
		{
			"Successfully",
			ShellCmdStatus.Completed
		},
		{
			"Sucessfully",
			ShellCmdStatus.Completed
		},
		{
			"IMEI1",
			ShellCmdStatus.Outputing
		},
		{
			"IMEI2",
			ShellCmdStatus.Outputing
		},
		{
			"PSN",
			ShellCmdStatus.Outputing
		},
		{
			"WIFI",
			ShellCmdStatus.Outputing
		},
		{
			"BT",
			ShellCmdStatus.Outputing
		},
		{
			"GSN",
			ShellCmdStatus.Outputing
		},
		{
			"MEID",
			ShellCmdStatus.Outputing
		},
		{
			"CountryCode",
			ShellCmdStatus.Outputing
		}
	};

	private static Dictionary<string, ShellCmdStatus> P410ProgResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"Total Test Result = FAIL",
			ShellCmdStatus.Error
		},
		{
			"Total Test Result = PASS",
			ShellCmdStatus.Completed
		},
		{
			"IMEI1",
			ShellCmdStatus.Outputing
		},
		{
			"IMEI2",
			ShellCmdStatus.Outputing
		},
		{
			"WiFi",
			ShellCmdStatus.Outputing
		},
		{
			"BT",
			ShellCmdStatus.Outputing
		},
		{
			"BatteryId",
			ShellCmdStatus.Outputing
		},
		{
			"SimLock Sck",
			ShellCmdStatus.Writing
		},
		{
			"TSDC_Security_Login",
			ShellCmdStatus.Error
		},
		{
			"Error:",
			ShellCmdStatus.Error
		},
		{
			"Invalid parameter",
			ShellCmdStatus.Error
		}
	};

	private static Dictionary<string, ShellCmdStatus> QCBlankFlashResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"Rebooting to fastboot",
			ShellCmdStatus.Completed
		},
		{
			"Failed",
			ShellCmdStatus.Error
		},
		{
			"Canceled",
			ShellCmdStatus.Error
		},
		{
			"Error",
			ShellCmdStatus.Error
		}
	};

	private static Dictionary<string, ShellCmdStatus> ZxProgResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"DisConnect Phone OK",
			ShellCmdStatus.Completed
		},
		{
			"IMEI1ReadFromNv",
			ShellCmdStatus.Outputing
		},
		{
			"IMEI2ReadFromNv",
			ShellCmdStatus.Outputing
		},
		{
			"WifiMacFromNv",
			ShellCmdStatus.Outputing
		},
		{
			"BtMacFromNv",
			ShellCmdStatus.Outputing
		},
		{
			"GSNReadFromNv",
			ShellCmdStatus.Outputing
		},
		{
			"PSNReadFromNv",
			ShellCmdStatus.Outputing
		},
		{
			"Error",
			ShellCmdStatus.Error
		},
		{
			"Invalid",
			ShellCmdStatus.Error
		},
		{
			"fail",
			ShellCmdStatus.Error
		}
	};

	private static Dictionary<string, ShellCmdStatus> LqProgResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"DisconnectMobile=[0]",
			ShellCmdStatus.Completed
		},
		{
			"CountryCodeReadFromNv",
			ShellCmdStatus.Outputing
		},
		{
			"PSNReadFromNv",
			ShellCmdStatus.Outputing
		},
		{
			"GSNReadFromNv",
			ShellCmdStatus.Outputing
		},
		{
			"IMEI1ReadFromNv",
			ShellCmdStatus.Outputing
		},
		{
			"WifiMacFromNv",
			ShellCmdStatus.Outputing
		},
		{
			"BtMacFromNv",
			ShellCmdStatus.Outputing
		},
		{
			"Error",
			ShellCmdStatus.Error
		},
		{
			"Invalid",
			ShellCmdStatus.Error
		},
		{
			"fail",
			ShellCmdStatus.Error
		}
	};

	public static Dictionary<string, ShellCmdStatus> GenericResponseToStatus = new Dictionary<string, ShellCmdStatus>();

	private static Dictionary<ShellCmdType, Dictionary<string, ShellCmdStatus>> ShellCmdToResponse = new Dictionary<ShellCmdType, Dictionary<string, ShellCmdStatus>>
	{
		{
			ShellCmdType.MTFLASHTOOL,
			MTekFlashResponseToStatus
		},
		{
			ShellCmdType.QCFLASHTOOL,
			QComFlashResponseToStatus
		},
		{
			ShellCmdType.SPFLASHTOOL,
			SPFlashResponseToStatus
		},
		{
			ShellCmdType.QFILFLASHTOOL,
			QFilFlashResponseToStatus
		},
		{
			ShellCmdType.MTPROGTOOL,
			MTekProgResponseToStatus
		},
		{
			ShellCmdType.RDFLASHTOOL,
			RDFlashResponseToStatus
		},
		{
			ShellCmdType.MMMPROGTOOL,
			MmmProgResponseToStatus
		},
		{
			ShellCmdType.MOBAPROGTOOL,
			MobaProgResponseToStatus
		},
		{
			ShellCmdType.JAVAPROGTOOL,
			JavaProgResponseToStatus
		},
		{
			ShellCmdType.LMPROGTOOL,
			LMProgResponseToStatus
		},
		{
			ShellCmdType.P410PROGTOOL,
			P410ProgResponseToStatus
		},
		{
			ShellCmdType.QCBLANKFLASHFILE,
			QCBlankFlashResponseToStatus
		},
		{
			ShellCmdType.ZXPROGTOOL,
			ZxProgResponseToStatus
		},
		{
			ShellCmdType.LQPROGTOOL,
			LqProgResponseToStatus
		},
		{
			ShellCmdType.GENERICTOOL,
			GenericResponseToStatus
		}
	};

	private static bool mTablesUpdated = ((Func<bool>)(() => UpdateResponseLookupTables()))();

	private ShellCmdType mShellCmd;

	private static object mLock = new object();

	private double mCurrentPercent;

	public ShellCmdType ShellCmd => mShellCmd;

	private string TAG => GetType().FullName;

	public ShellResponse(string exe)
	{
		string text = Path.GetFileName(exe).ToLower();
		mShellCmd = ShellCmdType.None;
		foreach (string key in ExeStringToShellCmdType.Keys)
		{
			if (text.StartsWith(key.ToLower()) && !string.IsNullOrEmpty(key))
			{
				mShellCmd = ExeStringToShellCmdType[key];
				break;
			}
		}
		if (mShellCmd == ShellCmdType.None)
		{
			string text2 = $"Tool {exe} is not defined";
			Smart.Log.Debug(TAG, text2);
			throw new NotSupportedException(text2);
		}
		InitialShellSetup(exe, mShellCmd);
	}

	public ShellCmdStatus ParseResponse(string response, out string responseKey)
	{
		ShellCmdStatus result = ShellCmdStatus.None;
		responseKey = string.Empty;
		bool flag = false;
		foreach (string key in ShellCmdToResponse[mShellCmd].Keys)
		{
			if ((!StartWithMatchingShellCmds.Contains(mShellCmd)) ? response.ToLower().Contains(key.ToLower()) : response.ToLower().StartsWith(key.ToLower()))
			{
				responseKey = key;
				result = ShellCmdToResponse[mShellCmd][key];
				break;
			}
		}
		return result;
	}

	public double GetDownloadProgressPercent(string response, string key)
	{
		string s = "0";
		double result = 0.0;
		if (mShellCmd == ShellCmdType.MTFLASHTOOL)
		{
			if (response.Contains("G)"))
			{
				s = response[..response.IndexOf('%')];
			}
		}
		else if (mShellCmd == ShellCmdType.QCFLASHTOOL)
		{
			s = response.Substring(key.Length);
		}
		else if (mShellCmd == ShellCmdType.QFILFLASHTOOL)
		{
			int startIndex = response.IndexOf(key) + key.Length;
			s = response.Substring(startIndex).Trim();
			s = s.Substring(0, s.Length - 2);
		}
		else if (mShellCmd == ShellCmdType.RDFLASHTOOL)
		{
			mCurrentPercent += 3.3333333333333335;
			s = $"{mCurrentPercent:0.00}";
		}
		if (double.TryParse(s, out var result2))
		{
			result = ((100.0 - result2 < 0.02) ? 100.0 : result2);
		}
		return result;
	}

	public void CleanUp()
	{
		if (mShellCmd != ShellCmdType.QFILFLASHTOOL)
		{
			return;
		}
		string text = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Qualcomm\\QFIL\\QFIL.config";
		lock (mLock)
		{
			if (File.Exists(text))
			{
				try
				{
					File.SetAttributes(text, FileAttributes.Normal);
					File.Delete(text);
					return;
				}
				catch (Exception ex)
				{
					Smart.Log.Error(TAG, $"Failed to delete file {text}. ErrorMsg: {ex.Message}");
					return;
				}
			}
			Smart.Log.Debug(TAG, $"QFIL config file {text} does not exist");
		}
	}

	public string ParseChinoeOutput(string response, IDevice device, SortedList<string, dynamic> cache)
	{
		string text = null;
		if (response.Contains("IMEI"))
		{
			if (!response.Contains(":"))
			{
				int num = response.IndexOf('(');
				int num2 = response.IndexOf(')');
				text = response.Substring(num + 1, num2 - num - 1).Trim();
			}
			else
			{
				text = response.Split(new char[1] { ':' })[1].Trim();
			}
			if (response.Contains("IMEI1"))
			{
				if (Smart.Convert.IsSerialNumberValid(text, (SerialNumberType)0))
				{
					device.SerialNumber = text;
				}
				Smart.Log.Debug(TAG, "Read IMEI1 = " + text);
			}
			else if (response.Contains("IMEI2"))
			{
				if (Smart.Convert.IsSerialNumberValid(text, (SerialNumberType)0))
				{
					device.SerialNumber2 = text;
				}
				Smart.Log.Debug(TAG, "Read IMEI2 = " + text);
			}
			else
			{
				Smart.Log.Error(TAG, $"Unable to extract IMEI value from \"{response}\"");
			}
		}
		else if (response.ToLower().Contains("software version"))
		{
			text = response.Split(new char[1] { ':' })[1].Trim();
			device.Log.AddInfo("FlashId", text);
			Smart.Log.Debug(TAG, "Read software version (FlashId) = " + text);
		}
		else if (response.Contains("model name"))
		{
			text = response.Split(new char[1] { ':' })[1].Trim();
			device.Log.AddInfo("SKU", text);
			Smart.Log.Debug(TAG, "Read model name (SKU) = " + text);
		}
		else if (response.Contains("SKU"))
		{
			text = response.Split(new char[1] { ':' })[1].Trim();
			device.Log.AddInfo("SKU", text);
			Smart.Log.Debug(TAG, "Read SKU = " + text);
		}
		else if (response.Contains("Unlock"))
		{
			text = response.Split(new char[1] { ':' })[1].Trim();
			cache.Add("lock1", text);
			Smart.Log.Verbose(TAG, "Read SIM lock code = " + text);
		}
		else if (response.Contains("Clear SIMLock Successful"))
		{
			text = "0000000000000000";
			cache.Add("lock1", text);
			Smart.Log.Verbose(TAG, "SIM lock code is cleared. Set it to " + text);
		}
		else if (response == "TRUE" || response == "FALSE")
		{
			device.Log.AddInfo("Google KeyBox Status", response);
			Smart.Log.Debug(TAG, "Google KeyBox Status " + response);
			text = response;
		}
		else if (response.Contains("WiFi(NV)"))
		{
			text = response.Split(new char[1] { ':' })[1].Trim();
			device.Log.AddInfo("WiFi", text);
			Smart.Log.Debug(TAG, "Read WiFi Address = " + text);
		}
		else if (response.Contains("BT(NV)"))
		{
			text = response.Split(new char[1] { ':' })[1].Trim();
			device.Log.AddInfo("BlueTooth", text);
			Smart.Log.Debug(TAG, "Read BlueTooth Address = " + text);
		}
		else if (response.Contains("Country Code"))
		{
			text = response.Split(new char[1] { ':' })[1].Trim();
			device.Log.AddInfo("CountryCode", text);
			Smart.Log.Debug(TAG, "Read Country Code = " + text);
		}
		return text;
	}

	public void WriteInputGeneric(string message, SortedList<string, string> inputlist, string inputkey, SortedList<string, dynamic> cache, Process process)
	{
		if (message.Contains(inputkey))
		{
			Smart.Log.Debug(TAG, $"Found input key '{inputkey}' ");
			inputlist.TryGetValue(inputkey, out var value);
			if (value.StartsWith("$"))
			{
				string key = value.Substring(1);
				value = cache[key];
			}
			Smart.Log.Debug(TAG, $"Write '{value}' to input key '{inputkey}' ");
			process.StandardInput.Flush();
			process.StandardInput.WriteLine(value);
			process.StandardInput.Flush();
		}
	}

	public void WriteInput(string message, string serialNumber, string logId, string clientReqType, string prodId, string keyType, string keyName, string inputFileName, Process process)
	{
		if (message.Contains("imei1.SEND_HASH_GPSODM."))
		{
			string empty = string.Empty;
			empty = message.Trim().Substring(message.Trim().LastIndexOf('.') + 1);
			Smart.Log.Debug("SEND_HASH_GPSODM", empty);
			string text = Smart.Web.DataSignODM(serialNumber, logId, clientReqType, prodId, keyType, keyName, empty);
			Smart.Log.Debug("full_security_key from GPS", text);
			string text2 = text;
			Smart.Log.Debug("MSG_CODE", text2);
			process.StandardInput.WriteLine(text2);
		}
		else if (message.Contains("imei2.SEND_HASH_GPSODM."))
		{
			string empty2 = string.Empty;
			empty2 = message.Trim().Substring(message.Trim().LastIndexOf('.') + 1);
			Smart.Log.Debug("SEND_HASH_GPSODM", empty2);
			string text3 = Smart.Web.DataSignODM(serialNumber, logId, clientReqType, prodId, keyType, keyName, empty2);
			Smart.Log.Debug("full_security_key from GPS", text3);
			string text4 = text3;
			Smart.Log.Debug("MSG_CODE", text4);
			process.StandardInput.WriteLine(text4);
		}
		if (message.Contains("Input Imei1"))
		{
			Smart.Log.Debug(TAG, "write data to cmd line: " + serialNumber);
			process.StandardInput.WriteLine(serialNumber);
		}
		else if (message.Contains("simlock.SEND_HASH_GPSODM"))
		{
			Smart.Log.Debug(TAG, "message: " + message);
			message = message.Substring(message.IndexOf("simlock."));
			Smart.Log.Debug(TAG, "new message: " + message);
			string text5 = message.Split(new char[1] { '.' })[2];
			Smart.Log.Debug(TAG, "odmData: " + text5);
			string text6 = Smart.Web.DataSignODM(serialNumber, logId, clientReqType, prodId, keyType, keyName, text5);
			Smart.Log.Verbose(TAG, "Received signed security key: " + text6);
			process.StandardInput.WriteLine(text6);
		}
		else if (message.Contains("Input Simlock file Path"))
		{
			Smart.Log.Debug(TAG, "message: " + message);
			Smart.Log.Debug(TAG, "simlockfile: " + inputFileName);
			process.StandardInput.WriteLine(inputFileName);
		}
		else if (message.Contains("SEND_HASH_GPSODM"))
		{
			Smart.Log.Debug(TAG, "message: " + message);
			string text7 = message.Split(new char[1] { '"' })[3];
			string text8 = Smart.Web.DataSignODM(serialNumber, logId, clientReqType, prodId, keyType, keyName, text7);
			Smart.Log.Verbose(TAG, "Received signed security key: " + text8);
			process.StandardInput.WriteLine(text8);
		}
		else
		{
			process.StandardInput.WriteLine("OK");
		}
	}

	public string ParseMobaOutput(string message, string[] keys)
	{
		string[] array = message.Split(new char[1] { '=' });
		string[] array2 = array[0].Split(new char[1] { ' ' });
		string text = null;
		foreach (string text2 in keys)
		{
			if (string.Compare(array2[1].Trim(), text2.Trim(), ignoreCase: true) == 0)
			{
				if (array.Length > 1)
				{
					text = array[1].Trim();
				}
				Smart.Log.Verbose(TAG, $"msg: {message} => parsed key: {text2} value: {text}");
				break;
			}
		}
		return text;
	}

	public string ParseJavaOutput(string message, IDevice device)
	{
		string text = null;
		string[] array = message.Split(new char[1] { ',' });
		if (array.Length > 1 && string.Compare(array[1].Trim(), "Pass", ignoreCase: true) == 0)
		{
			string[] array2 = array[0].Split(new char[1] { '=' });
			if (array2.Length > 1)
			{
				text = array2[1].Trim();
				if (array2[0].Contains("IMEI1"))
				{
					if (Smart.Convert.IsSerialNumberValid(text, (SerialNumberType)0))
					{
						device.SerialNumber = text;
					}
					Smart.Log.Verbose(TAG, "Read IMEI = " + text);
				}
				else if (array2[0].Contains("IMEI2"))
				{
					if (Smart.Convert.IsSerialNumberValid(text, (SerialNumberType)0))
					{
						device.SerialNumber2 = text;
					}
					Smart.Log.Verbose(TAG, "Read IMEI2 = " + text);
				}
			}
		}
		return text;
	}

	public string ParseJavaWriteValue(string message, SortedList<string, dynamic> cache)
	{
		string result = null;
		string[] array = message.Split(new char[1] { ',' });
		if (array.Length > 1 && string.Compare(array[1].Trim(), "Pass", ignoreCase: true) == 0)
		{
			string[] array2 = array[0].Split(new char[1] { '=' });
			if (array2.Length > 1 && array2[0].ToLower().Contains("simlock"))
			{
				string text = array2[1].Trim();
				cache.Add("lock1", text);
				Smart.Log.Verbose(TAG, "Write SIM lock code = " + text);
			}
		}
		return result;
	}

	public string ParseLMOutput(string response, IDevice device)
	{
		string text = null;
		if (response.Contains("IMEI1"))
		{
			string[] array = response.Split(new char[1] { ':' });
			if (array.Length > 1)
			{
				text = array[1].Trim();
				if (Smart.Convert.IsSerialNumberValid(text, (SerialNumberType)0))
				{
					device.SerialNumber = text;
				}
			}
			Smart.Log.Debug(TAG, "Read IMEI1 = " + text);
		}
		else if (response.Contains("IMEI2"))
		{
			string[] array2 = response.Split(new char[1] { ':' });
			if (array2.Length > 1)
			{
				text = array2[1].Trim();
				if (Smart.Convert.IsSerialNumberValid(text, (SerialNumberType)0))
				{
					device.SerialNumber2 = text;
				}
			}
			Smart.Log.Debug(TAG, "Read IMEI2 = " + text);
		}
		else if (response.Contains("GSN"))
		{
			string[] array3 = response.Split(new char[1] { ':' });
			if (array3.Length > 1)
			{
				text = (device.GSN = (device.ID = array3[1].Trim(new char[1]).Trim()));
				if (device.WiFiOnlyDevice)
				{
					device.SerialNumber = text;
				}
			}
			Smart.Log.Debug(TAG, "Read GSN = " + text);
		}
		else if (response.Contains("PSN"))
		{
			string[] array4 = response.Split(new char[1] { ':' });
			if (array4.Length > 1)
			{
				text = (device.PSN = array4[1].Trim());
				device.Log.AddInfo("PSN", text);
			}
			Smart.Log.Debug(TAG, "Read PSN = " + text);
		}
		else if (response.Contains("WiFi"))
		{
			string[] array5 = response.Split(new char[1] { ':' });
			if (array5.Length > 1)
			{
				text = array5[1].Trim();
				device.Log.AddInfo("WiFi", text);
			}
			Smart.Log.Debug(TAG, "Read WiFi Address = " + text);
		}
		else if (response.Contains("BT"))
		{
			string[] array6 = response.Split(new char[1] { ':' });
			if (array6.Length > 1)
			{
				text = array6[1].Trim();
				device.Log.AddInfo("BlueTooth", text);
			}
			Smart.Log.Debug(TAG, "Read BlueTooth Address = " + text);
		}
		else if (response.Contains("MEID"))
		{
			string[] array7 = response.Split(new char[1] { ':' });
			if (array7.Length > 1)
			{
				text = array7[1].Trim();
				if (Smart.Convert.IsSerialNumberValid(text, (SerialNumberType)0))
				{
					device.SerialNumber = text;
				}
			}
			Smart.Log.Debug(TAG, "Read MEID = " + text);
		}
		else if (response.Contains("CountryCode"))
		{
			string[] array8 = response.Split(new char[1] { ':' });
			if (array8.Length > 1)
			{
				text = array8[1].Trim();
				device.Log.AddInfo("CountryCode", text);
			}
			Smart.Log.Debug(TAG, "Read CountryCode = " + text);
		}
		return text;
	}

	public string ParseP410Output(string response, IDevice device)
	{
		string text = null;
		if (response.Contains("IMEI1"))
		{
			string[] array = response.Split(new char[1] { '=' });
			if (array.Length > 1)
			{
				text = array[1].Trim();
				if (Smart.Convert.IsSerialNumberValid(text, (SerialNumberType)0))
				{
					device.SerialNumber = text;
				}
			}
			Smart.Log.Debug(TAG, "Read IMEI1 = " + text);
		}
		else if (response.Contains("IMEI2"))
		{
			string[] array2 = response.Split(new char[1] { '=' });
			if (array2.Length > 1)
			{
				text = array2[1].Trim();
				if (Smart.Convert.IsSerialNumberValid(text, (SerialNumberType)0))
				{
					device.SerialNumber2 = text;
				}
			}
			Smart.Log.Debug(TAG, "Read IMEI2 = " + text);
		}
		else if (response.ToLower().Contains("BatteryId"))
		{
			string[] array3 = response.Split(new char[1] { '=' });
			if (array3.Length > 1)
			{
				text = array3[1].Trim();
				device.Log.AddInfo("battid", text);
			}
			Smart.Log.Debug(TAG, "BatteryId = " + text);
		}
		else if (response.Contains("WiFi"))
		{
			string[] array4 = response.Split(new char[1] { '=' });
			if (array4.Length > 1)
			{
				text = array4[1].Trim();
				device.Log.AddInfo("WiFi", text);
			}
			Smart.Log.Debug(TAG, "Read WiFi Address = " + text);
		}
		else if (response.Contains("BT"))
		{
			string[] array5 = response.Split(new char[1] { '=' });
			if (array5.Length > 1)
			{
				text = array5[1].Trim();
				device.Log.AddInfo("BlueTooth", text);
			}
			Smart.Log.Debug(TAG, "Read BlueTooth Address = " + text);
		}
		return text;
	}

	public string ParseZxOutput(string response, IDevice device)
	{
		string text = null;
		if (response.Contains("IMEI1"))
		{
			string[] array = response.Split(new char[1] { '=' });
			if (array.Length > 1)
			{
				text = array[1].Replace("[", string.Empty).Replace("]", string.Empty).Trim();
				if (Smart.Convert.IsSerialNumberValid(text, (SerialNumberType)0))
				{
					if (device.Group == string.Empty)
					{
						device.SerialNumber = text;
					}
					else
					{
						device.Log.AddInfo("IMEI", text);
					}
				}
			}
			Smart.Log.Debug(TAG, "Read IMEI1 = " + text);
		}
		else if (response.Contains("IMEI2"))
		{
			string[] array2 = response.Split(new char[1] { '=' });
			if (array2.Length > 1)
			{
				text = array2[1].Replace("[", string.Empty).Replace("]", string.Empty).Trim();
				if (Smart.Convert.IsSerialNumberValid(text, (SerialNumberType)0))
				{
					device.SerialNumber2 = text;
				}
			}
			Smart.Log.Debug(TAG, "Read IMEI2 = " + text);
		}
		else if (response.Contains("GSN"))
		{
			string[] array3 = response.Split(new char[1] { '=' });
			if (array3.Length > 1)
			{
				text = (device.GSN = array3[1].Replace("[", string.Empty).Replace("]", string.Empty).Trim());
				if (device.Group == string.Empty)
				{
					device.ID = text;
					if (device.WiFiOnlyDevice)
					{
						device.SerialNumber = text;
					}
				}
				else
				{
					device.SerialNumber = text;
				}
			}
			Smart.Log.Debug(TAG, "Read GSN = " + text);
		}
		else if (response.Contains("PSN"))
		{
			string[] array4 = response.Split(new char[1] { '=' });
			if (array4.Length > 1)
			{
				text = (device.PSN = array4[1].Replace("[", string.Empty).Replace("]", string.Empty).Trim());
				device.Log.AddInfo("PSN", text);
			}
			Smart.Log.Debug(TAG, "Read PSN = " + text);
		}
		else if (response.Contains("WiFi"))
		{
			string[] array5 = response.Split(new char[1] { '=' });
			if (array5.Length > 1)
			{
				text = array5[1].Replace("[", string.Empty).Replace("]", string.Empty).Trim();
				device.Log.AddInfo("WiFi", text);
			}
			Smart.Log.Debug(TAG, "Read WiFi Address = " + text);
		}
		else if (response.Contains("BtMac"))
		{
			string[] array6 = response.Split(new char[1] { '=' });
			if (array6.Length > 1)
			{
				text = array6[1].Replace("[", string.Empty).Replace("]", string.Empty).Trim();
				device.Log.AddInfo("BlueTooth", text);
			}
			Smart.Log.Debug(TAG, "Read BlueTooth Address = " + text);
		}
		else if (response.Contains("CountryCode"))
		{
			string[] array7 = response.Split(new char[1] { '=' });
			if (array7.Length > 1)
			{
				text = array7[1].Replace("[", string.Empty).Replace("]", string.Empty).Trim();
				device.Log.AddInfo("Country Code", text);
			}
			Smart.Log.Debug(TAG, "Read CountryCode = " + text);
		}
		return text;
	}

	public string ParseP410WriteValue(string message, SortedList<string, dynamic> cache)
	{
		string result = null;
		string[] array = message.Split(new char[1] { '=' });
		if (array[0].ToLower().Contains("simlock"))
		{
			string text = array[1].Trim();
			cache.Add("lock1", text);
			Smart.Log.Verbose(TAG, "Write SIM lock code = " + text);
		}
		return result;
	}

	public string ParseGenericOutput(string message, SortedList<string, string> mOutput, string[] OutputLength, SortedList<string, dynamic> Cache)
	{
		string result = null;
		Smart.Log.Verbose(MethodBase.GetCurrentMethod().Name, "Entering generic output collecting...");
		for (int i = 0; i < mOutput.Count; i++)
		{
			string text = mOutput.Keys[i];
			string text2 = mOutput.Values[i];
			if (text.StartsWith("$"))
			{
				string key = text.Substring(1);
				text = Cache[key];
			}
			Smart.Log.Debug(TAG, $"Try to find out the value of \"{text}\" ");
			string text3 = GetResultFromResponse(text, message);
			if (OutputLength[i].ToLowerInvariant() != "all" && !string.IsNullOrEmpty(text3))
			{
				text3 = text3.Substring(0, int.Parse(OutputLength[i]));
			}
			if (!string.IsNullOrEmpty(text3))
			{
				Cache[text2] = text3;
				Smart.Log.Debug(TAG, $"Added tha value of \"{text3}\" to cache name {text2} ");
				break;
			}
		}
		return result;
	}

	private string GetResultFromResponse(string strIn, string response)
	{
		string result = string.Empty;
		if (response.Contains(strIn))
		{
			result = response.Trim().Substring(response.IndexOf(strIn)).Replace(strIn, "")
				.Trim();
		}
		return result;
	}

	private static bool UpdateResponseLookupTables()
	{
		bool result = false;
		try
		{
			string filePathName = Smart.Rsd.GetFilePathName("shellResp", (UseCase)0, (IDevice)null);
			if (File.Exists(filePathName))
			{
				string text = Smart.Rsd.ReadShellResponseUpdateContent(filePathName);
				ResponseUpdateDef responseUpdateDef = Smart.Json.LoadString<ResponseUpdateDef>(text);
				UpdateTable(responseUpdateDef.ExeStringToShellCmdType, ExeStringToShellCmdType);
				UpdateTable(responseUpdateDef.MTekFlashResponseToStatus, MTekFlashResponseToStatus);
				UpdateTable(responseUpdateDef.QComFlashResponseToStatus, QComFlashResponseToStatus);
				UpdateTable(responseUpdateDef.SPFlashResponseToStatus, SPFlashResponseToStatus);
				UpdateTable(responseUpdateDef.QFilFlashResponseToStatus, QFilFlashResponseToStatus);
				UpdateTable(responseUpdateDef.RDFlashResponseToStatus, RDFlashResponseToStatus);
				UpdateTable(responseUpdateDef.MTekProgResponseToStatus, MTekProgResponseToStatus);
				UpdateTable(responseUpdateDef.MmmProgResponseToStatus, MmmProgResponseToStatus);
				UpdateTable(responseUpdateDef.MobaProgResponseToStatus, MobaProgResponseToStatus);
				UpdateTable(responseUpdateDef.JavaProgResponseToStatus, JavaProgResponseToStatus);
				UpdateTable(responseUpdateDef.LMProgResponseToStatus, LMProgResponseToStatus);
				UpdateTable(responseUpdateDef.P410ProgResponseToStatus, P410ProgResponseToStatus);
				UpdateTable(responseUpdateDef.QCBlankFlashResponseToStatus, QCBlankFlashResponseToStatus);
				UpdateTable(responseUpdateDef.ZxProgResponseToStatus, ZxProgResponseToStatus);
				UpdateTable(responseUpdateDef.LqProgResponseToStatus, LqProgResponseToStatus);
				UpdateTable(responseUpdateDef.GenericResponseToStatus, GenericResponseToStatus);
				result = true;
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	private static void UpdateTable<TKey, TValue>(Dictionary<TKey, TValue> newTable, Dictionary<TKey, TValue> currentTable)
	{
		if (newTable == null)
		{
			return;
		}
		foreach (TKey key in newTable.Keys)
		{
			currentTable[key] = newTable[key];
		}
	}

	private void InitialShellSetup(string exePathName, ShellCmdType shellCmdtype)
	{
		Smart.Log.Verbose(TAG, $"exePathName {exePathName}, shellCmdtype {shellCmdtype.ToString()} ");
		if (shellCmdtype != ShellCmdType.MMMPROGTOOL)
		{
			return;
		}
		string directoryName = Path.GetDirectoryName(exePathName);
		string fileName = Path.GetFileName(exePathName);
		string text = "C:\\prod\\bin";
		if (!Directory.Exists(text))
		{
			Smart.File.MirrorFiles(directoryName, text);
			return;
		}
		string path = Path.Combine(text, fileName);
		if (!File.Exists(path))
		{
			try
			{
				Directory.Delete(text, recursive: true);
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, "Skip exception to delete folder:" + ex.Message + Environment.NewLine + ex.StackTrace);
			}
			Smart.File.MirrorFiles(directoryName, text);
			return;
		}
		DateTime lastWriteTime = File.GetLastWriteTime(exePathName);
		DateTime lastWriteTime2 = File.GetLastWriteTime(path);
		if (lastWriteTime != lastWriteTime2)
		{
			try
			{
				Directory.Delete(text, recursive: true);
			}
			catch (Exception ex2)
			{
				Smart.Log.Error(TAG, "Skip exception to delete folder:" + ex2.Message + Environment.NewLine + ex2.StackTrace);
			}
			Smart.File.MirrorFiles(directoryName, text);
		}
	}
}
