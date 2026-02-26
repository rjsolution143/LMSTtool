using System;
using System.Collections.Generic;
using System.Xml;
using ISmart;

namespace SmartDevice.Steps;

public class ReadPropertiesInFastboot : FastbootStep
{
	private const string BOOTLOADER = "(bootloader) ";

	private string mFastbootExe;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		int num = 3000;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
			num *= 1000;
		}
		mFastbootExe = Smart.Rsd.GetFilePathName("fastbootExe", base.Recipe.Info.UseCase, val);
		List<string> list = new List<string>();
		int num2 = default(int);
		for (int i = 0; i < 2; i++)
		{
			if (list.Count >= 30)
			{
				break;
			}
			list = Smart.MotoAndroid.Shell(val.ID, "getvar all", num, mFastbootExe, ref num2, 6000, false);
			Smart.Log.Debug(TAG, "Response count: " + list.Count);
		}
		if (list.Count > 20)
		{
			val.Communicating = true;
			val.LastConnected = DateTime.Now;
		}
		else
		{
			val.Communicating = false;
		}
		if (val.Communicating)
		{
			List<string> list2 = new List<string>();
			for (int j = 0; j < 2; j++)
			{
				if (list2.Count >= 10)
				{
					break;
				}
				list2 = Smart.MotoAndroid.Shell(val.ID, "oem hw", num, mFastbootExe, ref num2, 6000, false);
				Smart.Log.Debug(TAG, "Oem hw response count: " + list2.Count);
			}
			list.AddRange(list2);
		}
		SortedList<string, string> sortedList = new SortedList<string, string>();
		foreach (string item in list)
		{
			if (item.Contains("(bootloader) "))
			{
				string[] array = item.Split(new string[2] { "(bootloader) ", ": " }, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length > 1)
				{
					sortedList[array[0]] = array[1];
				}
			}
		}
		Smart.Log.Debug(TAG, "Final response: ");
		foreach (KeyValuePair<string, string> item2 in sortedList)
		{
			Smart.Log.Debug(TAG, item2.Key + ":" + item2.Value);
		}
		string text = ReadSku(val, sortedList);
		if (sortedList.TryGetValue("product", out var value))
		{
			Smart.Log.Debug(TAG, "Read product: " + value);
			base.Log.AddInfo("Product", value);
			if (value != string.Empty)
			{
				string group = default(string);
				val.Type = Smart.Rsd.GetDeviceType(value, ref group);
				val.Group = group;
				ReadNonMobileDeviceSerial(val, sortedList, value);
			}
		}
		else if (text.ToUpper().StartsWith("XT"))
		{
			val.Type = (DeviceType)1;
		}
		IResultLogger log = base.Log;
		DeviceType type = val.Type;
		log.AddInfo("Type", ((object)(DeviceType)(ref type)).ToString());
		if (val.Group != string.Empty)
		{
			base.Log.AddInfo("Group", val.Group);
		}
		ILog log2 = Smart.Log;
		string tAG = TAG;
		type = val.Type;
		log2.Debug(tAG, $"Device Type: {((object)(DeviceType)(ref type)).ToString()}");
		Smart.Log.Debug(TAG, $"Device Group: {val.Group}");
		ReadTrackId(val, sortedList);
		ReadImei(val, sortedList);
		ReadImei2(val, sortedList);
		ReadRoCarrier(val, sortedList);
		ReadFlexIdAndFsgVersion(val, sortedList, out var _);
		ReadFingerPrint(val, sortedList);
		ReadBlurVersion(val, sortedList);
		ReadFlashSize(val, sortedList);
		ReadRamSize(val, sortedList);
		ReadFactoryDate(val, sortedList);
		ReadInfomativeProps(val, sortedList);
		ReadOemHwDualSim(val, num, sortedList);
		ReadOemHwESimFlag(val, num, sortedList);
		ReadESimEid(val, sortedList);
		ReadOemHwFrontColor(val, num, sortedList);
		base.Cache["props"] = sortedList;
		type = val.Type;
		SetPreCondition(((object)(DeviceType)(ref type)).ToString());
		LogPass();
	}

	private string ReadOemHwFrontColor(IDevice device, int timeOut, SortedList<string, string> properties)
	{
		string text = string.Empty;
		if (properties.TryGetValue("frontcolor", out var value))
		{
			text = value;
			Smart.Log.Info(TAG, string.Format("Read {0}: {1} from properties", "frontcolor", text));
		}
		if (text == string.Empty)
		{
			Smart.Log.Error(TAG, string.Format("Failed to read oem hw {0} in fastboot", "frontcolor"));
		}
		if (text != string.Empty)
		{
			base.Log.AddInfo("FrontColor", text);
			properties["FrontColor"] = text;
		}
		return text;
	}

	private string ReadESimEid(IDevice device, SortedList<string, string> properties)
	{
		if (!properties.TryGetValue("esimid", out var value))
		{
			value = string.Empty;
			Smart.Log.Error(TAG, "Failed to read eid in fastboot");
		}
		else
		{
			value = value.Trim();
			Smart.Log.Info(TAG, "Read eSIMEid: " + value);
			base.Log.AddInfo("eSIMEid", value);
		}
		return value;
	}

	private string ReadTrackId(IDevice device, SortedList<string, string> properties)
	{
		if (!properties.TryGetValue("serialno", out var value))
		{
			value = string.Empty;
			Smart.Log.Error(TAG, "Failed to read TrackId in fastboot");
		}
		else
		{
			Smart.Log.Info(TAG, "Read TrackId: " + value);
			base.Log.AddInfo("TrackId", value);
			value = value.Trim();
			if (value != string.Empty && value.ToUpper() != "UNKNOWN")
			{
				device.ID = value;
			}
		}
		return value;
	}

	private string ReadImei(IDevice device, SortedList<string, string> properties)
	{
		if (!properties.TryGetValue("imei", out var value))
		{
			value = string.Empty;
			Smart.Log.Error(TAG, "Failed to read IMEI in fastboot");
		}
		else
		{
			value = Smart.Convert.DecSerialToHex(value);
			Smart.Log.Info(TAG, "Read imei: " + value);
			base.Log.AddInfo("IMEI", value);
			device.SerialNumber = value;
		}
		return value;
	}

	private string ReadImei2(IDevice device, SortedList<string, string> properties)
	{
		if (!properties.TryGetValue("imei2", out var value))
		{
			value = string.Empty;
			Smart.Log.Verbose(TAG, "No imei2 in fastboot");
		}
		else
		{
			value = Smart.Convert.DecSerialToHex(value);
			Smart.Log.Info(TAG, "Read imei2: " + value);
			base.Log.AddInfo("IMEI2", value);
			device.SerialNumber2 = value;
		}
		return value;
	}

	private string ReadRoCarrier(IDevice device, SortedList<string, string> properties)
	{
		if (!properties.TryGetValue("ro.carrier", out var value))
		{
			value = string.Empty;
			Smart.Log.Error(TAG, "Failed to read roCarrier in fastboot");
		}
		else
		{
			Smart.Log.Info(TAG, "Read roCarrier: " + value);
			base.Log.AddInfo(DetectionKey.NameToDisplayStringLookup["roCarrier"], value);
			device.RoCarrier = value;
			properties[DetectionKey.NameToPropertyLookup["roCarrier"]] = value;
		}
		return value;
	}

	private string ReadFlexIdAndFsgVersion(IDevice device, SortedList<string, string> properties, out string fsgVersion)
	{
		string valueFromGetVarResponse = GetValueFromGetVarResponse("version-baseband", properties);
		fsgVersion = string.Empty;
		string text = string.Empty;
		if (valueFromGetVarResponse != string.Empty)
		{
			if (!valueFromGetVarResponse.Contains("not "))
			{
				string[] array = valueFromGetVarResponse.Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length > 1)
				{
					text = array[0].Replace(",", string.Empty);
					fsgVersion = array[1];
				}
				else if (array.Length == 1)
				{
					fsgVersion = array[0];
					text = array[0];
				}
			}
			else
			{
				fsgVersion = valueFromGetVarResponse;
				text = valueFromGetVarResponse;
			}
		}
		if (fsgVersion == string.Empty)
		{
			Smart.Log.Error(TAG, "Failed to read FsgVersion in fastboot");
		}
		else
		{
			Smart.Log.Info(TAG, "Read FsgVersion: " + fsgVersion);
			base.Log.AddInfo(DetectionKey.NameToDisplayStringLookup["fsgVersion"], fsgVersion);
			properties[DetectionKey.NameToPropertyLookup["fsgVersion"]] = fsgVersion;
		}
		if (text == string.Empty)
		{
			Smart.Log.Error(TAG, "Failed to read FlexId in fastboot");
		}
		else
		{
			Smart.Log.Info(TAG, "Read FlexId: " + text);
			base.Log.AddInfo("FlexId", text);
		}
		return text;
	}

	private string ReadFingerPrint(IDevice device, SortedList<string, string> properties)
	{
		string valueFromGetVarResponse = GetValueFromGetVarResponse("ro.build.fingerprint", properties);
		if (valueFromGetVarResponse == string.Empty)
		{
			Smart.Log.Error(TAG, "Failed to read fingerPrint in fastboot");
		}
		else
		{
			Smart.Log.Info(TAG, "Read fingerPrint: " + valueFromGetVarResponse);
			base.Log.AddInfo(DetectionKey.NameToDisplayStringLookup["fingerPrint"], valueFromGetVarResponse);
			properties[DetectionKey.NameToPropertyLookup["fingerPrint"]] = valueFromGetVarResponse;
			string flashId = GetFlashId(valueFromGetVarResponse);
			Smart.Log.Info(TAG, "Read FlashId: " + flashId);
			base.Log.AddInfo("FlashId", flashId);
		}
		return valueFromGetVarResponse;
	}

	private string ReadBlurVersion(IDevice device, SortedList<string, string> properties)
	{
		string valueFromGetVarResponse = GetValueFromGetVarResponse("ro.build.version.full", properties);
		if (valueFromGetVarResponse == string.Empty)
		{
			Smart.Log.Error(TAG, "Failed to read BlurVersion in fastboot");
		}
		else
		{
			Smart.Log.Info(TAG, "Read BlurVersion: " + valueFromGetVarResponse);
			base.Log.AddInfo(DetectionKey.NameToDisplayStringLookup["blur"], valueFromGetVarResponse);
			properties[DetectionKey.NameToPropertyLookup["blur"]] = valueFromGetVarResponse;
		}
		return valueFromGetVarResponse;
	}

	private string ReadSku(IDevice device, SortedList<string, string> properties)
	{
		string[] obj = new string[2] { "sku", "carrier_sku" };
		string text = string.Empty;
		string[] array = obj;
		foreach (string text2 in array)
		{
			if (properties.TryGetValue(text2, out var value))
			{
				text = value.Trim();
				if (text != string.Empty)
				{
					Smart.Log.Info(TAG, $"Read SKU: {text} from element: {text2}");
					base.Log.AddInfo(DetectionKey.NameToDisplayStringLookup["sku"], text);
					properties[DetectionKey.NameToPropertyLookup["sku"]] = text;
					break;
				}
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			Smart.Log.Error(TAG, "Failed to read SKU in fastboot");
		}
		return text;
	}

	private string ReadFlashSize(IDevice device, SortedList<string, string> properties)
	{
		string element = "emmc";
		string valueFromGetVarResponse = GetValueFromGetVarResponse("storage-type", properties);
		Smart.Log.Debug(TAG, "storage-type: " + valueFromGetVarResponse);
		if (!string.IsNullOrEmpty(valueFromGetVarResponse))
		{
			element = valueFromGetVarResponse.ToLower();
		}
		string valueFromGetVarResponse2 = GetValueFromGetVarResponse(element, properties);
		string text = string.Empty;
		if (valueFromGetVarResponse2 == string.Empty)
		{
			Smart.Log.Error(TAG, "Failed to read Flash Size in fastboot");
		}
		else
		{
			text = valueFromGetVarResponse2.Split(new char[1] { ' ' })[0];
			Smart.Log.Info(TAG, "Read Flash Size: " + text);
			base.Log.AddInfo(DetectionKey.NameToDisplayStringLookup["flashSize"], text);
			properties[DetectionKey.NameToPropertyLookup["flashSize"]] = text;
		}
		return text;
	}

	private string ReadRamSize(IDevice device, SortedList<string, string> properties)
	{
		string valueFromGetVarResponse = GetValueFromGetVarResponse("ram", properties);
		string text = string.Empty;
		if (valueFromGetVarResponse == string.Empty)
		{
			Smart.Log.Error(TAG, "Failed to read RAM Size in fastboot");
		}
		else
		{
			text = valueFromGetVarResponse.Split(new char[1] { ' ' })[0];
			Smart.Log.Info(TAG, "Read RAM Size: " + text);
			base.Log.AddInfo(DetectionKey.NameToDisplayStringLookup["ramSize"], text);
			properties[DetectionKey.NameToPropertyLookup["ramSize"]] = text;
		}
		return text;
	}

	private string ReadFactoryDate(IDevice device, SortedList<string, string> properties)
	{
		if (!properties.TryGetValue("date", out var value))
		{
			value = string.Empty;
			Smart.Log.Error(TAG, "Failed to read factory date in fastboot");
		}
		else
		{
			Smart.Log.Info(TAG, "Read factory date: " + value);
			device.ManufacturingDate = value;
			base.Log.AddInfo("Mfg Date", value);
		}
		return value;
	}

	private string ReadOemHwDualSim(IDevice device, int timeOut, SortedList<string, string> properties)
	{
		string text = string.Empty;
		if (properties.TryGetValue("dualsim", out var value))
		{
			text = value;
			Smart.Log.Info(TAG, string.Format("Read {0}: {1} from properties", "dualsim", text));
		}
		if (text == string.Empty)
		{
			try
			{
				int num = default(int);
				List<string> list = Smart.MotoAndroid.Shell(device.ID, "oem hw dualsim", timeOut, mFastbootExe, ref num, 6000, false);
				if (num == 0)
				{
					text = list[1].Split(new char[1] { ':' })[1].Trim();
					Smart.Log.Debug(TAG, string.Format("Read {0}: {1} from fastboot oem hw", "dualsim", text));
				}
				else
				{
					Smart.Log.Error(TAG, string.Format("Failed to read {0} in fastboot oem hw", "dualsim"));
				}
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, string.Format("Exception - Failed to read {0} in fastboot oem hw. ErrMsg: {1}", "dualsim", ex.Message));
			}
		}
		if (text != string.Empty)
		{
			base.Log.AddInfo(DetectionKey.NameToDisplayStringLookup["dualSim"], text);
			properties[DetectionKey.NameToPropertyLookup["dualSim"]] = text;
		}
		return text;
	}

	[Obsolete]
	private string ReadSIMConfig(IDevice device, int timeOut, SortedList<string, string> properties)
	{
		int num = default(int);
		List<string> lines = Smart.MotoAndroid.Shell(device.ID, "oem config num-sims", timeOut, mFastbootExe, ref num, 6000, false);
		string valueFromOemConfigResp = GetValueFromOemConfigResp(lines);
		string text = string.Empty;
		switch (valueFromOemConfigResp)
		{
		case "1":
			text = "ss";
			break;
		case "2":
			text = "dsds";
			break;
		case "3":
			text = "tsts";
			break;
		case "4":
			text = "qsqs";
			break;
		}
		if (text == string.Empty)
		{
			Smart.Log.Error(TAG, "Failed to determine SIM config for numSims: " + valueFromOemConfigResp);
		}
		else
		{
			Smart.Log.Info(TAG, "Read SIM Config: " + text);
			base.Log.AddInfo(DetectionKey.NameToDisplayStringLookup["simConfig"], text);
			properties[DetectionKey.NameToPropertyLookup["simConfig"]] = text;
		}
		return text;
	}

	private void ReadNonMobileDeviceSerial(IDevice device, SortedList<string, string> props, string product)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if ((int)device.Type == 1 || (int)device.Type == 0)
		{
			return;
		}
		SortedList<string, string> sortedList = default(SortedList<string, string>);
		bool flag2 = (device.WiFiOnlyDevice = Smart.Rsd.IsWifiDevice(product, ref sortedList));
		if (props.TryGetValue(sortedList["FB_UID_PROP"], out var value))
		{
			value = value.Trim();
			Smart.Log.Debug(TAG, string.Format("Read {0}: {1}", sortedList["FB_UID_PROP"], value));
			if (device.ID == string.Empty)
			{
				device.ID = value;
			}
		}
		else
		{
			Smart.Log.Error(TAG, string.Format("Failed to read uid using property {0} in fastboot", sortedList["FB_UID_PROP"]));
		}
		if (!flag2)
		{
			if (sortedList["FB_IMEI_PROP"] != string.Empty)
			{
				if (props.TryGetValue(sortedList["FB_IMEI_PROP"], out var value2))
				{
					value2 = value2.Trim();
					Smart.Log.Debug(TAG, string.Format("Read {0}: {1}", sortedList["FB_IMEI_PROP"], value2));
					if (value2 != string.Empty)
					{
						if (device.Group == string.Empty)
						{
							device.SerialNumber = value2;
						}
						else
						{
							base.Log.AddInfo("IMEI", value2);
						}
					}
				}
				else
				{
					Smart.Log.Error(TAG, string.Format("Failed to read IMEI using property {0} in fastboot", sortedList["FB_IMEI_PROP"]));
				}
			}
			if (sortedList["FB_IMEI2_PROP"] != string.Empty)
			{
				if (props.TryGetValue(sortedList["FB_IMEI2_PROP"], out var value3))
				{
					value3 = value3.Trim();
					Smart.Log.Debug(TAG, string.Format("Read {0}: {1}", sortedList["FB_IMEI2_PROP"], value3));
					if (value3 != string.Empty)
					{
						device.SerialNumber2 = value3;
					}
				}
				else
				{
					Smart.Log.Error(TAG, string.Format("Failed to read IMEI2 using property {0} in fastboot", sortedList["FB_IMEI2_PROP"]));
				}
			}
		}
		if (device.Group == string.Empty)
		{
			device.GSN = value;
			if (device.WiFiOnlyDevice || device.Group != string.Empty)
			{
				device.SerialNumber = device.GSN;
			}
			Smart.Log.Debug(TAG, $"GSN: {device.GSN}");
		}
	}

	private void ReadInfomativeProps(IDevice device, SortedList<string, string> properties)
	{
		foreach (string item in new List<string>
		{
			"secure", "securestate", "cid", "factory-modes", "battid", "channelid", "qe", "frp-state", "hwrev", "pcb-part-no",
			"current-slot", "uid", "iswarrantyvoid"
		})
		{
			if (!properties.TryGetValue(item, out var value))
			{
				Smart.Log.Error(TAG, $"Failed to read {item} in fastboot");
				continue;
			}
			Smart.Log.Info(TAG, $"Read {item}: {value}");
			base.Log.AddInfo(item, value);
		}
	}

	private string GetValueFromGetVarResponse(string element, SortedList<string, string> properties)
	{
		if (!properties.TryGetValue(element, out var value))
		{
			int num = 0;
			value = string.Empty;
			bool flag;
			do
			{
				string key = element + "[" + num + "]";
				if (flag = properties.TryGetValue(key, out var value2))
				{
					value += value2;
					num++;
				}
			}
			while (flag);
		}
		return value.Trim();
	}

	private string GetFlashId(string fingerPrint)
	{
		string result = string.Empty;
		string[] array = fingerPrint.Split(new char[1] { '/' });
		if (array.Length > 3)
		{
			result = array[3].Trim();
		}
		return result;
	}

	private string GetValueFromOemConfigResp(List<string> lines)
	{
		string result = string.Empty;
		string text = string.Empty;
		bool flag = false;
		foreach (string line in lines)
		{
			if (line.Contains("<UTAG"))
			{
				flag = true;
				text = line.Replace("(bootloader) ", string.Empty);
			}
			else if (flag)
			{
				text += line.Replace("(bootloader) ", string.Empty);
			}
			if (line.Contains("</UTAG>"))
			{
				flag = false;
				text = text.Trim();
			}
		}
		if (text != string.Empty)
		{
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(text);
				XmlNode xmlNode = xmlDocument.SelectSingleNode("UTAG/value");
				if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText))
				{
					result = xmlNode.InnerText.Trim();
				}
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, $"Malformed UTAG xml: {text}\r\n Error: {ex.Message}");
			}
		}
		return result;
	}

	private string ReadOemHwESimFlag(IDevice device, int timeOut, SortedList<string, string> properties)
	{
		string text = string.Empty;
		if (properties.TryGetValue("esim", out var value))
		{
			text = value;
			Smart.Log.Info(TAG, string.Format("Read {0}: {1} from properties", "esim", text));
		}
		if (text == string.Empty)
		{
			try
			{
				int num = default(int);
				List<string> list = Smart.MotoAndroid.Shell(device.ID, "oem hw esim", timeOut, mFastbootExe, ref num, 6000, false);
				if (num == 0)
				{
					text = ((!list[1].Contains(":")) ? list[1].Replace("(bootloader)", string.Empty).Trim() : list[1].Split(new char[1] { ':' })[1].Trim());
					Smart.Log.Debug(TAG, string.Format("Read {0}: {1} from fastboot oem hw", "esim", text));
				}
				else
				{
					Smart.Log.Error(TAG, string.Format("Failed to read {0} in fastboot oem hw", "esim"));
				}
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, string.Format("Exception - Failed to read {0} in fastboot oem hw. ErrMsg: {1}", "esim", ex.Message));
			}
		}
		if (text != string.Empty)
		{
			base.Log.AddInfo(DetectionKey.NameToDisplayStringLookup["ESim"], text);
			properties[DetectionKey.NameToPropertyLookup["ESim"]] = text;
		}
		return text;
	}
}
