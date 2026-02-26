using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ISmart;

namespace SmartDevice.Steps;

public class DeviceInfoUpdate : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		string empty = string.Empty;
		string text = null;
		string empty2 = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		string text4 = "1999-01-01";
		string text5 = string.Empty;
		if (((dynamic)base.Info.Args).InputName != null && ((dynamic)base.Info.Args).InputName != string.Empty)
		{
			empty = ((dynamic)base.Info.Args).InputName.ToString();
			empty2 = base.Cache[empty];
		}
		else
		{
			IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
			empty2 = val.SerialNumber;
			empty = "DeviceSerialNumber";
			if (val.SerialNumber2 != null && val.SerialNumber2.Trim() != string.Empty)
			{
				text2 = val.SerialNumber2;
			}
			if (val.ModelId != null)
			{
				bool flag = default(bool);
				text3 = Smart.Rsd.GetValue("sku", (UseCase)134, val, ref flag, false);
			}
			if (val.ManufacturingDate != null)
			{
				string manufacturingDate = val.ManufacturingDate;
				Regex regex = new Regex("(?<month>\\d\\d)-(?<day>\\d\\d)-(?<year>\\d\\d\\d\\d)");
				if (regex.IsMatch(manufacturingDate))
				{
					Match match = regex.Match(manufacturingDate);
					text4 = string.Format("{0}-{1}-{2}", match.Groups["year"], match.Groups["month"], match.Groups["day"]);
				}
			}
			if (val.ID != null && val.ID.Length == 10)
			{
				text5 = val.ID;
			}
			text = val.RecordId;
		}
		bool flag2 = false;
		if (((dynamic)base.Info.Args).IgnoreErrors != null)
		{
			flag2 = ((dynamic)base.Info.Args).IgnoreErrors;
		}
		if (text == null)
		{
			text = Smart.Convert.GenerateCode(empty2, false);
			if (text == null || !(text.Trim() != string.Empty))
			{
				Smart.Log.Error(TAG, $"No record ID available for {empty2}");
				if (flag2)
				{
					LogResult((Result)7);
				}
				else
				{
					LogResult((Result)1, "Could not generate record ID for this IMEI");
				}
				return;
			}
			Smart.Log.Verbose(TAG, "Record ID Generated");
		}
		string key = empty + "NotFound";
		bool flag3 = false;
		if (!base.Cache.ContainsKey(key))
		{
			if (base.Cache.ContainsKey(empty + "Details"))
			{
				Smart.Log.Info(TAG, $"{empty} is already in the database");
				if (empty == "DeviceSerialNumber")
				{
					((IDevice)((dynamic)base.Recipe.Info.Args).Device).RecordId = text;
				}
				base.Log.AddInfo("DeviceInfoUpdate", "NotNeeded");
				LogResult((Result)7);
			}
			else
			{
				Smart.Log.Info(TAG, $"{empty} database status is unknown");
				base.Log.AddInfo("DeviceInfoUpdate", "NotPossible");
				LogResult((Result)7);
			}
			return;
		}
		Smart.Log.Info(TAG, $"Requesting GPPD info for {empty} {empty2}");
		try
		{
			SortedList<string, string> gppdId = Smart.Web.GetGppdId(empty2);
			base.Cache[empty + "GppdId"] = gppdId["GppdId"];
			base.Cache[empty + "Customer"] = gppdId["Customer"];
			base.Cache[empty + "Protocol"] = gppdId["Protocol"];
			string text6 = gppdId["GppdId"];
			string text7 = gppdId["Customer"];
			if ((text6 == null || !(text6.Trim() != string.Empty)) && (text7 == null || !(text7.Trim() != string.Empty)))
			{
				Smart.Log.Info(TAG, $"{empty} GPPD info is unknown");
				if (empty == "DeviceSerialNumber")
				{
					object obj = (object)(IDevice)((dynamic)base.Recipe.Info.Args).Device;
					((IDevice)obj).InvalidSerialNumber = true;
					((IDevice)obj).RecordId = null;
				}
				base.Log.AddInfo("DeviceInfoUpdate", "NotPossible");
				LogResult((Result)7);
				return;
			}
			Smart.Log.Info(TAG, $"Found GPPD info for '{empty2}' with GPPD ID '{text6}' and Customer '{text7}'");
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, $"Error requesting GPPD info for {empty} {empty2}");
			Smart.Log.Verbose(TAG, ex.ToString());
			base.Log.AddInfo("DeviceInfoUpdate", "NotPossible");
			LogResult((Result)7);
			return;
		}
		if (empty == "DeviceSerialNumber")
		{
			((IDevice)((dynamic)base.Recipe.Info.Args).Device).RecordId = text;
		}
		try
		{
			if (empty2 != string.Empty && empty2 != "000000000000000")
			{
				Smart.Web.DeviceInfoUpdate(text, empty2, text2, text4, text3, text5);
			}
		}
		catch (Exception ex2)
		{
			Smart.Log.Error(TAG, $"Error updating device info for {empty} {empty2}");
			Smart.Log.Error(TAG, ex2.Message);
			Smart.Log.Verbose(TAG, ex2.ToString());
			base.Log.AddInfo("DeviceInfoUpdate", "Failed");
			if (flag2)
			{
				LogResult((Result)7);
				return;
			}
			throw;
		}
		string text8 = "Success";
		if (flag3)
		{
			text8 += "-VOI";
		}
		base.Log.AddInfo("DeviceInfoUpdate", text8);
		LogPass();
	}
}
