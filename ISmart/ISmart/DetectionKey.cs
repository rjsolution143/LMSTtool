using System;
using System.Collections.Generic;
using System.Text;

namespace ISmart;

public class DetectionKey
{
	public const string Ignore = "IGNORE";

	public const string FingerPrint = "fingerPrint";

	public const string BlurVersion = "blur";

	public const string FsgVersion = "fsgVersion";

	public const string RoCarrier = "roCarrier";

	public const string SimConfig = "simConfig";

	public const string Sku = "sku";

	public const string HardwareProperty = "hardwareProperty";

	public const string HardwareProperty1 = "hardwareProperty1";

	public const string CustomerSwVersion = "customerSwVersion";

	public const string FlashSize = "flashSize";

	public const string RamSize = "ramSize";

	public const string DualSim = "dualSim";

	public const string BuildDisplay = "builddisplay";

	public const string ProductModel = "productModel";

	public const string LST = "LST";

	public const string Esim = "ESim";

	public static Dictionary<string, string> NameToPropertyLookup = new Dictionary<string, string>
	{
		{ "blur", "ro.build.version.full" },
		{ "fingerPrint", "ro.build.fingerprint" },
		{ "fsgVersion", "ril.baseband.config.version" },
		{ "roCarrier", "ro.carrier" },
		{ "sku", "ro.boot.hardware.sku" },
		{ "simConfig", "persist.radio.multisim.config" },
		{ "hardwareProperty", "gsm.serial" },
		{ "hardwareProperty1", "gsm.sn1" },
		{ "customerSwVersion", "ro.build.version.incremental" },
		{ "flashSize", "vold.emmc_size" },
		{ "ramSize", "total.ram.size" },
		{ "dualSim", "ro.boot.dualsim" },
		{ "builddisplay", "ro.build.display.id" },
		{ "productModel", "ro.product.model" },
		{ "ESim", "ro.vendor.hw.esim" }
	};

	public static Dictionary<string, string> NameToDisplayStringLookup = new Dictionary<string, string>
	{
		{ "blur", "Blur Version" },
		{ "fingerPrint", "Fingerprint" },
		{ "fsgVersion", "FSG Version" },
		{ "roCarrier", "RoCarrier" },
		{ "sku", "SKU" },
		{ "simConfig", "SIM Config" },
		{ "hardwareProperty", "HW Property" },
		{ "hardwareProperty1", "HW Prop 1" },
		{ "customerSwVersion", "Cust SW Ver" },
		{ "flashSize", "Flash Size" },
		{ "ramSize", "RAM Size" },
		{ "dualSim", "Dual SIM" },
		{ "builddisplay", "Build Display" },
		{ "productModel", "Product Model" },
		{ "ESim", "ro.vendor.hw.esim" }
	};

	private static Dictionary<string, List<string>> mAlternatePropertyLookup = new Dictionary<string, List<string>> { 
	{
		"ril.baseband.config.version",
		new List<string> { "gsm.version.baseband", "gsm.version.baseband1", "vendor.ril.baseband.config.version" }
	} };

	private string mGroup;

	public Dictionary<string, string> PropertyToValueLookUp { get; private set; }

	public bool DeviceKey { get; private set; }

	public string HardwareCode { get; private set; }

	public DetectionKey(SortedList<string, string> properties, string defaultString, bool deviceKey, string group = "")
	{
		PropertyToValueLookUp = new Dictionary<string, string>();
		DeviceKey = deviceKey;
		mGroup = group;
		foreach (string value2 in NameToPropertyLookup.Values)
		{
			if (properties.TryGetValue(value2, out var value))
			{
				PropertyToValueLookUp.Add(value2, value.Trim());
			}
			else
			{
				PropertyToValueLookUp.Add(value2, defaultString);
			}
		}
		HardwareCode = GetHardwareCode(deviceKey);
		if (deviceKey && GetValue("fsgVersion") == string.Empty)
		{
			PropertyToValueLookUp[NameToPropertyLookup["fsgVersion"]] = GetFsgVersionFromAlternates(properties);
		}
	}

	public bool Matches(DetectionKey detectionKey)
	{
		if (DeviceKey == detectionKey.DeviceKey)
		{
			return false;
		}
		foreach (string key in PropertyToValueLookUp.Keys)
		{
			if (string.Compare(detectionKey.PropertyToValueLookUp[key], "IGNORE", ignoreCase: true) != 0 && string.Compare(PropertyToValueLookUp[key], "IGNORE", ignoreCase: true) != 0 && string.Compare(PropertyToValueLookUp[key], detectionKey.PropertyToValueLookUp[key], ignoreCase: true) != 0)
			{
				return false;
			}
		}
		return true;
	}

	public bool Equals(DetectionKey detectionKey)
	{
		if (DeviceKey != detectionKey.DeviceKey)
		{
			return false;
		}
		foreach (string key in PropertyToValueLookUp.Keys)
		{
			if (string.Compare(PropertyToValueLookUp[key], detectionKey.PropertyToValueLookUp[key], ignoreCase: true) != 0)
			{
				return false;
			}
		}
		return true;
	}

	public string GetValue(string name)
	{
		string text = string.Empty;
		if (NameToPropertyLookup.TryGetValue(name, out var value))
		{
			text = PropertyToValueLookUp[value];
		}
		if (string.Compare(text, "IGNORE", ignoreCase: true) == 0)
		{
			text = string.Empty;
		}
		return text;
	}

	public bool IsEmpty()
	{
		foreach (string value in PropertyToValueLookUp.Values)
		{
			if (value != string.Empty && string.Compare(value, "IGNORE", ignoreCase: true) != 0)
			{
				return false;
			}
		}
		return true;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string key in PropertyToValueLookUp.Keys)
		{
			stringBuilder.Append($"\"{key}\": \"{PropertyToValueLookUp[key]}\", ");
		}
		return stringBuilder.ToString();
	}

	public override int GetHashCode()
	{
		string text = GetValue("fingerPrint");
		if (text == string.Empty || mGroup == "LST")
		{
			text = GetValue("builddisplay") + GetValue("productModel");
		}
		return text.GetHashCode();
	}

	private string GetHardwareCode(bool deviceKey)
	{
		string text = string.Empty;
		string key = NameToPropertyLookup["hardwareProperty"];
		string value = GetValue("hardwareProperty");
		if (value == string.Empty)
		{
			value = GetValue("hardwareProperty1");
			key = NameToPropertyLookup["hardwareProperty1"];
		}
		if (!deviceKey)
		{
			text = value;
		}
		else if (value != string.Empty)
		{
			switch (value.Split(new char[1] { ' ' })[0].Length)
			{
			case 18:
				text = value.Substring(3, 2);
				break;
			case 23:
				text = value.Substring(14, 2);
				break;
			case 25:
				text = value.Substring(23, 2);
				break;
			}
			PropertyToValueLookUp[key] = text;
		}
		return text;
	}

	private string GetFsgVersionFromAlternates(SortedList<string, string> properties)
	{
		string result = string.Empty;
		string key = NameToPropertyLookup["fsgVersion"];
		if (mAlternatePropertyLookup.TryGetValue(key, out var value))
		{
			foreach (string item in value)
			{
				if (!properties.TryGetValue(item, out var value2))
				{
					continue;
				}
				value2 = value2.Trim();
				if (value2 != string.Empty)
				{
					string[] array = value2.Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries);
					if (array.Length > 1)
					{
						result = array[1].Trim();
						break;
					}
					if (array.Length == 1)
					{
						result = array[0].Trim();
						break;
					}
				}
			}
		}
		return result;
	}
}
