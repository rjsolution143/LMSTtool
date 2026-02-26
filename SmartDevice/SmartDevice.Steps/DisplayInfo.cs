using System;
using System.Collections.Generic;
using System.Drawing;
using ISmart;

namespace SmartDevice.Steps;

public class DisplayInfo : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		SortedList<string, string> sortedList = new SortedList<string, string>();
		foreach (string key2 in val.Log.Info.Keys)
		{
			sortedList[key2] = val.Log.Info[key2];
		}
		sortedList["SerialNumber"] = val.SerialNumber;
		if (val.SerialNumber != val.SerialNumber2)
		{
			sortedList["SerialNumber2"] = val.SerialNumber2;
		}
		sortedList["TrackID"] = val.ID;
		bool flag = default(bool);
		sortedList["Model"] = Smart.Rsd.GetValue("sku", (UseCase)134, val, ref flag, false);
		if (sortedList["Model"] == null || string.IsNullOrEmpty(sortedList["Model"].Trim()))
		{
			string value = "UNKNOWN";
			if (val.Log.Info.ContainsKey(DetectionKey.NameToDisplayStringLookup["sku"]))
			{
				value = val.Log.Info[DetectionKey.NameToDisplayStringLookup["sku"]];
			}
			sortedList["Model"] = value;
		}
		List<string> list = new List<string>(new string[6] { "SerialNumber", "SerialNumber2", "TrackID", "Model", "eSIMIccid", "eSIMEid" });
		List<string> list2 = new List<string>(new string[2]
		{
			DetectionKey.NameToDisplayStringLookup["fingerPrint"],
			"FrontColor"
		});
		if (((dynamic)base.Info.Args).ExtraDataTypesForQrCode != null)
		{
			string[] collection = ((dynamic)base.Info.Args).ExtraDataTypesForQrCode.ToString().Split(',');
			list2.AddRange(collection);
		}
		List<string> list3 = new List<string>(new string[5] { "WarrantySerialNumber", "WarrantyDualModeSerialNumber", "WarrantyCit", "WarrantyCustomerModelNumber", "WarrantyIccid" });
		List<Image> list4 = new List<Image>();
		string text = string.Empty;
		foreach (string item4 in list)
		{
			if (!sortedList.ContainsKey(item4))
			{
				Smart.Log.Debug(TAG, $"No info found for {item4}");
				continue;
			}
			string text2 = sortedList[item4];
			if (text2 == null || text2.Trim() == string.Empty || text2 == "UNKNOWN")
			{
				Smart.Log.Debug(TAG, $"Blank info found for {item4}");
				continue;
			}
			Smart.Log.Debug(TAG, $"Device info {item4} found: {text2}");
			switch (item4)
			{
			case "SerialNumber":
				list3.Remove("WarrantySerialNumber");
				break;
			case "SerialNumber2":
				list3.Remove("WarrantyDualModeSerialNumber");
				break;
			case "TrackID":
				list3.Remove("WarrantyCit");
				break;
			case "Model":
				list3.Remove("WarrantyCustomerModelNumber");
				break;
			case "eSIMIccid":
				list3.Remove("WarrantyIccid");
				break;
			}
			Image item = Smart.Graphics.Barcode(text2);
			list4.Add(item);
			text = text + "+" + text2;
		}
		foreach (string item5 in list3)
		{
			if (!sortedList.ContainsKey(item5))
			{
				Smart.Log.Debug(TAG, $"No warranty info found for {item5}");
				continue;
			}
			string text3 = sortedList[item5];
			if (text3 == null || text3.Trim() == string.Empty || text3 == "UNKNOWN")
			{
				Smart.Log.Debug(TAG, $"Blank warranty info found for {item5}");
				continue;
			}
			Smart.Log.Debug(TAG, $"Warranty info {item5} found: {text3}");
			if (item5 == "WarrantyCustomerModelNumber")
			{
				text3 = FindModelName(text3);
				if (text3 == null || text3 == string.Empty)
				{
					Smart.Log.Debug(TAG, "Could not find market model for sales number found");
					continue;
				}
				Smart.Log.Debug(TAG, $"Market model for {item5} found: {text3}");
			}
			Image item2 = Smart.Graphics.Barcode(text3);
			list4.Add(item2);
			text = text + "+" + text3;
		}
		foreach (string item6 in list2)
		{
			if (string.IsNullOrEmpty(item6.Trim()))
			{
				continue;
			}
			if (item6.StartsWith("$"))
			{
				string key = item6.Substring(1);
				if (base.Cache.ContainsKey(key))
				{
					string text4 = base.Cache[key];
					text = text + "+" + text4;
				}
			}
			else if (!sortedList.ContainsKey(item6))
			{
				Smart.Log.Debug(TAG, $"No info found for {item6}");
			}
			else
			{
				string text5 = sortedList[item6];
				if (text5 == null || text5.Trim() == string.Empty || text5 == "UNKNOWN")
				{
					Smart.Log.Debug(TAG, $"Blank info found for {item6}");
					continue;
				}
				Smart.Log.Debug(TAG, $"Device info {item6} found: {text5}");
				text = text + "+" + text5;
			}
		}
		text = text.Trim().Trim(new char[1] { '+' });
		Smart.Log.Debug(TAG, $"Qr Code Data {text}");
		Image item3 = Smart.Graphics.QrCode(text);
		list4.Add(item3);
		if (list4.Count < 1)
		{
			Smart.Log.Error(TAG, "No device info found to display");
			LogResult((Result)1, "No device info found to display");
			return;
		}
		Image val2 = Smart.Graphics.JoinImages(list4);
		string arg = Smart.Locale.Xlate("Device Serial Number");
		string text6 = $"{arg} {val.SerialNumber}";
		try
		{
			val.Prompt.ShowImage(text6, val2);
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, ex.Message);
			Smart.Log.Verbose(TAG, ex.ToString());
		}
		LogPass();
	}

	private string FindModelName(string salesModel)
	{
		string saleModelFileContent = Smart.Fsb.GetSaleModelFileContent();
		ICsvFile obj = Smart.NewCsvFile();
		obj.Load(saleModelFileContent, ',');
		foreach (SortedList<string, string> item in (IEnumerable<SortedList<string, string>>)obj)
		{
			if (item["Sales Model"].ToLowerInvariant() == salesModel.ToLowerInvariant())
			{
				return item["Market Name"];
			}
		}
		return string.Empty;
	}
}
