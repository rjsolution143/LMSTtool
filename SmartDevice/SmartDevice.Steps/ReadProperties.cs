using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ISmart;

namespace SmartDevice.Steps;

public class ReadProperties : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string empty = string.Empty;
		try
		{
			empty = Smart.ADB.Shell(val.ID, "getprop", 10000);
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Error sending ADB shell getprop command: " + ex.Message);
			Smart.Log.Error(TAG, ex.ToString());
			LogResult((Result)4, "Could not send ADB shell getprop command", ex.Message);
			return;
		}
		Smart.Log.Debug(TAG, "\r\n" + empty);
		MatchCollection matchCollection = new Regex("^\\[(?<key>.*)\\]: \\[(?<value>.*)\\]", RegexOptions.Multiline).Matches(empty);
		SortedList<string, string> sortedList = new SortedList<string, string>();
		foreach (Match item in matchCollection)
		{
			string value = item.Groups["key"].Value;
			string value2 = item.Groups["value"].Value;
			sortedList[value] = value2;
		}
		if (sortedList.Count < 1)
		{
			LogResult((Result)1, "No properties read from device");
			return;
		}
		val.Communicating = true;
		val.LastConnected = DateTime.Now;
		try
		{
			ReadProps(sortedList, val);
		}
		catch (Exception ex2)
		{
			Smart.Log.Error(TAG, "Error reading props: " + ex2.Message);
			Smart.Log.Error(TAG, ex2.ToString());
			LogResult((Result)4, "Could not read props (probably RAM size related)", ex2.Message);
			return;
		}
		base.Cache["props"] = sortedList;
		DeviceType type = val.Type;
		SetPreCondition(((object)(DeviceType)(ref type)).ToString());
		LogPass();
	}

	private void ReadProps(SortedList<string, string> props, IDevice device)
	{
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Expected O, but got Unknown
		GetRamSize(device, props);
		GetFlashSize(props);
		props.TryGetValue("ro.product.brand", out var value);
		Smart.Log.Debug(TAG, "Report Brand: " + value);
		base.Log.AddInfo("Brand", value);
		GetCountryCode(props);
		GetDualSimConfig(props);
		string sku = GetSku(props);
		if (props.TryGetValue("ro.product.model", out var value2))
		{
			Smart.Log.Debug(TAG, "Product Model: " + value2);
			base.Log.AddInfo(DetectionKey.NameToDisplayStringLookup["productModel"], value2);
		}
		if (props.TryGetValue("ro.build.display.id", out var value3))
		{
			Smart.Log.Debug(TAG, "Build Display:" + value3);
			base.Log.AddInfo(DetectionKey.NameToDisplayStringLookup["builddisplay"], value3);
		}
		if (props.TryGetValue("ro.build.id", out var value4))
		{
			Smart.Log.Debug(TAG, "FlashId: " + value4);
			base.Log.AddInfo("FlashId", value4);
		}
		if (props.TryGetValue("ro.boot.slot_suffix", out var value5))
		{
			Smart.Log.Debug(TAG, "current-slot: " + value5);
			base.Log.AddInfo("current-slot", value5.Replace("_", string.Empty));
		}
		if (props.TryGetValue("ro.boot.uid", out var value6))
		{
			Smart.Log.Debug(TAG, "uid: " + value6);
			base.Log.AddInfo("uid", value6);
		}
		string value7 = string.Empty;
		string text = string.Empty;
		if (props.TryGetValue("gsm.version.baseband", out value7))
		{
			value7 = value7.Trim();
			string[] array = value7.Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length > 1)
			{
				value7 = array[0].Replace(",", string.Empty);
				Smart.Log.Debug(TAG, "FlexId:" + value7);
				base.Log.AddInfo("FlexId", value7);
				text = array[1];
			}
			else if (array.Length == 1)
			{
				text = GetFsgVersionFromAlternates(props);
				if (string.IsNullOrEmpty(text.Trim()))
				{
					text = array[0];
				}
			}
			Smart.Log.Debug(TAG, "FSGVersion:" + text);
			base.Log.AddInfo(DetectionKey.NameToDisplayStringLookup["fsgVersion"], text);
			props[DetectionKey.NameToPropertyLookup["fsgVersion"]] = text;
		}
		string productString = GetProductString(props);
		if (productString != string.Empty)
		{
			string group = default(string);
			device.Type = Smart.Rsd.GetDeviceType(productString, ref group);
			device.Group = group;
			ReadNonMobileDeviceSerials(device, props, productString);
		}
		else if (sku.ToUpper().StartsWith("XT"))
		{
			device.Type = (DeviceType)1;
		}
		IResultLogger log = base.Log;
		DeviceType type = device.Type;
		log.AddInfo("Type", ((object)(DeviceType)(ref type)).ToString());
		if (device.Group != string.Empty)
		{
			base.Log.AddInfo("Group", device.Group);
		}
		ILog log2 = Smart.Log;
		string tAG = TAG;
		type = device.Type;
		log2.Debug(tAG, $"Device Type: {((object)(DeviceType)(ref type)).ToString()}");
		Smart.Log.Debug(TAG, $"Device Group: {device.Group}");
		DetectionKey val = new DetectionKey(props, string.Empty, true, "");
		string hardwareCode = val.HardwareCode;
		if (hardwareCode != string.Empty)
		{
			Smart.Log.Debug(TAG, "HardwareCode: " + hardwareCode);
			base.Log.AddInfo("Hardware Code", hardwareCode);
		}
		foreach (string key in DetectionKey.NameToDisplayStringLookup.Keys)
		{
			string value8 = val.GetValue(key);
			if (value8 != string.Empty)
			{
				Smart.Log.Debug(TAG, $"{key}: {value8}");
				base.Log.AddInfo(DetectionKey.NameToDisplayStringLookup[key], value8);
				if (key == "roCarrier")
				{
					device.RoCarrier = value8;
				}
			}
		}
		foreach (string item in new List<string> { "secure", "securestate", "ro.secure", "ro.secure_boot.state" })
		{
			if (props.TryGetValue(item, out var value9))
			{
				Smart.Log.Debug(TAG, item + ":" + value9);
				base.Log.AddInfo(item, value9);
			}
		}
	}

	private string GetFsgVersionFromAlternates(SortedList<string, string> properties)
	{
		string text = string.Empty;
		foreach (string item in new List<string> { "vendor.ril.baseband.config.version" })
		{
			if (properties.TryGetValue(item, out var value))
			{
				text = value.Trim();
				if (text != string.Empty)
				{
					break;
				}
			}
		}
		return text;
	}

	private string GetRamSize(IDevice device, SortedList<string, string> props)
	{
		string value = null;
		string text = DetectionKey.NameToPropertyLookup["ramSize"];
		foreach (string item in new List<string> { "ro.hw.ram", "ro.vendor.hw.ram", "ro.boot.Mem", text })
		{
			if (props.TryGetValue(item, out value))
			{
				break;
			}
		}
		if (value == null)
		{
			string[] array = Smart.ADB.Shell(device.ID, "cat /proc/meminfo", 10000).Split(new string[1] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			value = string.Empty;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3 = array2[i].Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries);
				if (array3.Length > 1 && string.Compare(array3[0], "MemTotal:", ignoreCase: true) == 0)
				{
					if (long.TryParse(array3[1].Trim(), out var result))
					{
						value = (int)((double)result / 1048576.0 + 0.5) + "GB";
					}
					break;
				}
			}
		}
		if (value != string.Empty)
		{
			props[text] = value;
		}
		return value;
	}

	private string GetFlashSize(SortedList<string, string> props)
	{
		string value = null;
		string text = DetectionKey.NameToPropertyLookup["flashSize"];
		foreach (string item in new List<string> { text, "ro.hw.storage", "ro.vendor.hw.storage", "ro.boot.Storage" })
		{
			if (props.TryGetValue(item, out value))
			{
				break;
			}
		}
		if (value == null)
		{
			value = string.Empty;
			if (props.ContainsKey(text) && long.TryParse(props[text], out var result))
			{
				value = (int)((double)result * 1.024 / 1000000000.0 + 0.5) + "GB";
			}
		}
		if (value != string.Empty)
		{
			props[text] = value;
		}
		return value;
	}

	private string GetCountryCode(SortedList<string, string> props)
	{
		string value = null;
		foreach (string item in new List<string> { "ro.lenovo.easyimage.code", "persist.sys.withsim.country", "ro.product.countrycode", "ro.carrier" })
		{
			if (props.TryGetValue(item, out value))
			{
				Smart.Log.Debug(TAG, "Country Code: " + value);
				base.Log.AddInfo("Country Code", value);
				break;
			}
		}
		return value;
	}

	private void ReadNonMobileDeviceSerials(IDevice device, SortedList<string, string> props, string product)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Invalid comparison between Unknown and I4
		if ((int)device.Type == 1 || (int)device.Type == 0)
		{
			return;
		}
		SortedList<string, string> sortedList = default(SortedList<string, string>);
		device.WiFiOnlyDevice = Smart.Rsd.IsWifiDevice(product, ref sortedList);
		base.Log.AddInfo("WiFiOnly", device.WiFiOnlyDevice.ToString());
		Smart.Log.Debug(TAG, "WiFiOnly: " + device.WiFiOnlyDevice);
		if (props.TryGetValue(sortedList["ADB_UID_PROP"], out var value))
		{
			value = value.Trim();
			Smart.Log.Debug(TAG, string.Format("Read {0}: {1}", sortedList["ADB_UID_PROP"], value));
			base.Log.AddInfo("TrackId", value);
			if (device.ID == string.Empty)
			{
				device.ID = value;
			}
		}
		else
		{
			Smart.Log.Error(TAG, string.Format("Failed to read uid using property {0} in adb", sortedList["ADB_UID_PROP"]));
		}
		if (!device.WiFiOnlyDevice)
		{
			if (props.TryGetValue(sortedList["ADB_IMEI_PROP"], out var value2))
			{
				value2 = value2.Trim();
				Smart.Log.Debug(TAG, string.Format("Read {0}: {1}", sortedList["ADB_IMEI_PROP"], value2));
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
				Smart.Log.Error(TAG, string.Format("Failed to read IMEI using property {0} in adb", sortedList["ADB_IMEI_PROP"]));
			}
			if (sortedList["ADB_IMEI2_PROP"] != string.Empty)
			{
				if (props.TryGetValue(sortedList["ADB_IMEI2_PROP"], out var value3))
				{
					value3 = value3.Trim();
					Smart.Log.Debug(TAG, string.Format("Read {0}: {1}", sortedList["ADB_IMEI2_PROP"], value3));
					if (value3 != string.Empty)
					{
						device.SerialNumber2 = value3;
					}
				}
				else
				{
					Smart.Log.Error(TAG, string.Format("Failed to read IMEI2 using property {0} in adb", sortedList["ADB_IMEI2_PROP"]));
				}
			}
		}
		string value4 = GetGSN(props);
		if (!string.IsNullOrEmpty(value4) || props.TryGetValue(sortedList["ADB_GSN_PROP"], out value4))
		{
			device.GSN = value4;
			base.Log.AddInfo("GSN", value4);
			if (device.WiFiOnlyDevice || device.Group == "LST")
			{
				device.SerialNumber = value4;
			}
			if ((int)device.Type == 2 && !device.WiFiOnlyDevice && string.IsNullOrEmpty(device.SerialNumber))
			{
				device.SerialNumber = value4;
			}
		}
		else
		{
			Smart.Log.Error(TAG, "Failed to read GSN in adb");
		}
		if (props.TryGetValue(sortedList["ADB_PSN_PROP"], out var value5))
		{
			device.PSN = value5;
			Smart.Log.Debug(TAG, string.Format("Read {0}: {1}", sortedList["ADB_PSN_PROP"], value5));
		}
		else
		{
			Smart.Log.Error(TAG, string.Format("Failed to read PSN using property {0} in adb", sortedList["ADB_PSN_PROP"]));
		}
	}

	private string GetDualSimConfig(SortedList<string, string> props)
	{
		string value = null;
		string text = DetectionKey.NameToPropertyLookup["dualSim"];
		foreach (string item in new List<string> { text, "ro.vendor.hw.dualsim" })
		{
			if (props.TryGetValue(item, out value))
			{
				props[text] = value;
				Smart.Log.Debug(TAG, "Sim Config:" + value);
				break;
			}
		}
		return value;
	}

	private string GetSku(SortedList<string, string> props)
	{
		string value = string.Empty;
		string text = DetectionKey.NameToPropertyLookup["sku"];
		foreach (string item in new List<string> { text, "ro.zuk.product.market" })
		{
			if (props.TryGetValue(item, out value))
			{
				props[text] = value;
				break;
			}
		}
		return value;
	}

	private string GetProductString(SortedList<string, string> props)
	{
		string text = string.Empty;
		foreach (string item in new List<string> { "ro.build.product", "ro.product.model" })
		{
			if (props.TryGetValue(item, out var value))
			{
				value = value.Trim();
				if (value != string.Empty)
				{
					text = value;
					Smart.Log.Debug(TAG, "Product:" + text);
					base.Log.AddInfo("Product", text);
					break;
				}
			}
		}
		return text;
	}

	private string GetGSN(SortedList<string, string> props)
	{
		string value = null;
		foreach (string item in new List<string> { "sys.lenovosn", "ro.odm.lenovo.gsn", "sys.customsn.showcode", "ro.lenovosn2", "persist.radio.factory_phone_sn", "gsm.lenovosn2", "persist.sys.snvalue", "persist.sys.snvalue", "ro.serialno", "lenovo.sn" })
		{
			if (props.TryGetValue(item, out value))
			{
				value = value.Trim();
				if (value != string.Empty)
				{
					Smart.Log.Debug(TAG, $"Property: {item} => GSN: {value}");
					break;
				}
			}
		}
		return value;
	}
}
