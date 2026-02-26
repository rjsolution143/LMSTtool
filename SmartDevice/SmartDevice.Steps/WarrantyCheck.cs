using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class WarrantyCheck : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_164d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1608: Unknown result type (might be due to invalid IL or missing references)
		//IL_160e: Invalid comparison between Unknown and I4
		//IL_198d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1948: Unknown result type (might be due to invalid IL or missing references)
		//IL_194e: Invalid comparison between Unknown and I4
		//IL_185b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1816: Unknown result type (might be due to invalid IL or missing references)
		//IL_181c: Invalid comparison between Unknown and I4
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string empty = string.Empty;
		string empty2 = string.Empty;
		if (((dynamic)base.Info.Args).InputName != null && ((dynamic)base.Info.Args).InputName != string.Empty)
		{
			empty = ((dynamic)base.Info.Args).InputName.ToString();
			empty2 = base.Cache[empty];
		}
		else
		{
			empty = "DeviceSerialNumber";
			empty2 = val.SerialNumber;
			if (empty2 == string.Empty || empty2 == "UNKNOWN")
			{
				empty2 = val.ID;
			}
		}
		bool flag = true;
		if (((dynamic)base.Info.Args).ActiveOnly != null)
		{
			flag = ((dynamic)base.Info.Args).ActiveOnly;
		}
		bool flag2 = false;
		if (((dynamic)base.Info.Args).VoidOnly != null)
		{
			flag2 = ((dynamic)base.Info.Args).VoidOnly;
			if (flag2)
			{
				flag = false;
			}
		}
		bool flag3 = false;
		if (((dynamic)base.Info.Args).NonActive != null)
		{
			flag3 = ((dynamic)base.Info.Args).NonActive;
			if (flag3)
			{
				flag = false;
				flag2 = false;
			}
		}
		bool flag4 = false;
		if (((dynamic)base.Info.Args).ConfirmStatus != null)
		{
			flag4 = ((dynamic)base.Info.Args).ConfirmStatus;
		}
		bool flag5 = false;
		if (((dynamic)base.Info.Args).UnlockCheck != null)
		{
			flag5 = ((dynamic)base.Info.Args).UnlockCheck;
		}
		bool flag6 = false;
		if (((dynamic)base.Info.Args).IgnoreErrors != null)
		{
			flag6 = ((dynamic)base.Info.Args).IgnoreErrors;
		}
		bool flag7 = false;
		if (((dynamic)base.Info.Args).AllowBlank != null)
		{
			flag7 = ((dynamic)base.Info.Args).AllowBlank;
		}
		string text = string.Empty;
		if (((dynamic)base.Info.Args).ErrorMessage != null && ((dynamic)base.Info.Args).ErrorMessage != string.Empty)
		{
			text = ((dynamic)base.Info.Args).ErrorMessage.ToString();
			text = Smart.Locale.Xlate(text);
		}
		if (empty2 == "000000000000000")
		{
			Smart.Log.Debug(TAG, "Device is a blank board with IMEI " + val.SerialNumber);
			if (flag7 || flag3)
			{
				LogPass();
			}
			else if (flag6)
			{
				LogResult((Result)7);
			}
			else
			{
				LogResult((Result)1, "No warranty info for Blank serial number");
			}
			return;
		}
		List<string> list = new List<string>();
		if (((dynamic)base.Info.Args).RequiredFields != null)
		{
			foreach (object item2 in ((dynamic)base.Info.Args).RequiredFields)
			{
				string item = (string)(dynamic)item2;
				list.Add(item);
			}
		}
		Smart.Log.Info(TAG, $"Checking warranty status for {empty} {empty2}");
		SortedList<string, string> sortedList = new SortedList<string, string>();
		try
		{
			sortedList = Smart.Web.WarrantyRequest(empty2, flag5);
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, $"Error checking warranty status for {empty} {empty2}");
			if (ex.Message.ToLowerInvariant().Contains("not found"))
			{
				base.Cache[empty + "NotFound"] = true;
			}
			if (flag6 || flag3)
			{
				LogResult((Result)7);
				return;
			}
			throw;
		}
		if (flag)
		{
			string text2 = sortedList["status_code"];
			Smart.Log.Debug(TAG, "Found warranty status code " + text2);
			if (text.Contains("{0}"))
			{
				text = string.Format(text, text2);
			}
			if (text2 != "ACT" && text2 != "BTL")
			{
				if (flag6)
				{
					LogResult((Result)7);
					return;
				}
				if (!(text != string.Empty))
				{
					throw new NotSupportedException("Warranty status not supported: " + text2);
				}
				string text3 = Smart.Locale.Xlate(base.Info.Name);
				if (!flag4)
				{
					val.Prompt.MessageBox(text3, text, (MessageBoxButtons)0, (MessageBoxIcon)16);
					throw new NotSupportedException("Warranty status not supported: " + text2);
				}
				if ((int)val.Prompt.MessageBox(text3, text, (MessageBoxButtons)4, (MessageBoxIcon)16) != 6)
				{
					throw new NotSupportedException("Warranty status not supported: " + text2);
				}
				Smart.Log.Info(TAG, $"User chose to ignore {text2} warranty status");
			}
			Smart.Log.Info(TAG, $"Found {text2} SN '{empty2}'");
		}
		else if (flag2)
		{
			string text4 = sortedList["status_code"];
			Smart.Log.Debug(TAG, "Found warranty status code " + text4);
			bool flag8 = false;
			if (text4 != "VOI" && sortedList.ContainsKey("cit") && sortedList["cit"] != null && sortedList["cit"] != string.Empty)
			{
				string text5 = sortedList["cit"].ToLowerInvariant().Trim();
				string iD = val.ID;
				if (iD != null && iD != string.Empty && iD != "UNKNOWN")
				{
					iD = iD.ToLowerInvariant().Trim();
					if (text5 != iD)
					{
						Smart.Log.Info(TAG, $"VOI warranty exception: Warranty Track ID {text5.ToUpperInvariant()} does not match Device Track ID {iD.ToUpperInvariant()}");
						flag8 = true;
					}
				}
			}
			if (text.Contains("{0}"))
			{
				text = string.Format(text, text4);
			}
			if (text4 != "VOI" && !flag8)
			{
				if (flag6)
				{
					LogResult((Result)7);
					return;
				}
				if (!(text != string.Empty))
				{
					throw new NotSupportedException("Warranty status not supported: " + text4);
				}
				string text6 = Smart.Locale.Xlate(base.Info.Name);
				if (!flag4)
				{
					val.Prompt.MessageBox(text6, text, (MessageBoxButtons)0, (MessageBoxIcon)16);
					throw new NotSupportedException("Warranty status not supported: " + text4);
				}
				if ((int)val.Prompt.MessageBox(text6, text, (MessageBoxButtons)4, (MessageBoxIcon)16) != 6)
				{
					throw new NotSupportedException("Warranty status not supported: " + text4);
				}
				Smart.Log.Info(TAG, $"User chose to ignore {text4} warranty status");
			}
			Smart.Log.Info(TAG, $"Found {text4} SN '{empty2}'");
		}
		else if (flag3)
		{
			string text7 = sortedList["status_code"];
			Smart.Log.Debug(TAG, "Found warranty status code " + text7);
			if (text.Contains("{0}"))
			{
				text = string.Format(text, text7);
			}
			if (text7 == "ACT")
			{
				if (flag6)
				{
					LogResult((Result)7);
					return;
				}
				if (!(text != string.Empty))
				{
					throw new NotSupportedException("Warranty status not supported: " + text7);
				}
				string text8 = Smart.Locale.Xlate(base.Info.Name);
				if (!flag4)
				{
					val.Prompt.MessageBox(text8, text, (MessageBoxButtons)0, (MessageBoxIcon)16);
					throw new NotSupportedException("Warranty status not supported: " + text7);
				}
				if ((int)val.Prompt.MessageBox(text8, text, (MessageBoxButtons)4, (MessageBoxIcon)16) != 6)
				{
					throw new NotSupportedException("Warranty status not supported: " + text7);
				}
				Smart.Log.Info(TAG, $"User chose to ignore {text7} warranty status");
			}
			Smart.Log.Info(TAG, $"Found {text7} SN '{empty2}'");
		}
		else
		{
			Smart.Log.Info(TAG, $"Found SN '{empty2}'");
		}
		if (sortedList.ContainsKey("dual_mode_serial_no") && sortedList["dual_mode_serial_no"] != null && sortedList["dual_mode_serial_no"].Trim() != string.Empty && sortedList["dual_mode_serial_no_type"].Trim() == "IMEI")
		{
			string text9 = sortedList["dual_mode_serial_no"].Trim();
			string text10 = empty + "Dual";
			base.Cache[text10] = text9;
			Smart.Log.Info(TAG, $"Found {text10} SN '{text9}'");
		}
		if (sortedList.ContainsKey("tri_mode_serial_no") && sortedList["tri_mode_serial_no"] != null && sortedList["tri_mode_serial_no"].Trim() != string.Empty && sortedList["tri_mode_serial_no_type"].Trim() == "IMEI")
		{
			string text11 = sortedList["tri_mode_serial_no"].Trim();
			string text12 = empty + "Tri";
			base.Cache[text12] = text11;
			Smart.Log.Info(TAG, $"Found {text12} SN '{text11}'");
		}
		if (sortedList.ContainsKey("status_code") && sortedList["status_code"] != null && sortedList["status_code"] != string.Empty)
		{
			val.Log.AddInfo("WarrantyCode", sortedList["status_code"]);
		}
		if (sortedList.ContainsKey("source_serial_no") && sortedList["source_serial_no"] != null && sortedList["source_serial_no"] != string.Empty)
		{
			val.Log.AddInfo("WarrantySerialNumber", sortedList["source_serial_no"]);
			if (!base.Cache.ContainsKey("WarrantySerialNumber"))
			{
				base.Cache["WarrantySerialNumber"] = sortedList["source_serial_no"];
			}
		}
		if (sortedList.ContainsKey("dual_mode_serial_no") && sortedList["dual_mode_serial_no"] != null && sortedList["dual_mode_serial_no"] != string.Empty)
		{
			val.Log.AddInfo("WarrantyDualModeSerialNumber", sortedList["dual_mode_serial_no"]);
		}
		if (sortedList.ContainsKey("cit") && sortedList["cit"] != null && sortedList["cit"] != string.Empty)
		{
			val.Log.AddInfo("WarrantyCit", sortedList["cit"]);
		}
		if (sortedList.ContainsKey("iccid") && sortedList["iccid"] != null && sortedList["iccid"] != string.Empty)
		{
			val.Log.AddInfo("WarrantyIccid", sortedList["iccid"]);
		}
		if (sortedList.ContainsKey("customer_model_no") && sortedList["customer_model_no"] != null && sortedList["customer_model_no"] != string.Empty)
		{
			val.Log.AddInfo("WarrantyCustomerModelNumber", sortedList["customer_model_no"]);
		}
		string key = empty + "Details";
		if (flag5)
		{
			key = empty + "UnlockDetails";
		}
		base.Cache[key] = sortedList;
		if (list.Count > 0)
		{
			foreach (string item3 in list)
			{
				if (!sortedList.ContainsKey(item3) || (sortedList[item3] != null && sortedList[item3].Length < 2))
				{
					Smart.Log.Error(TAG, $"Data missing for {item3} which is a required field");
					LogResult((Result)1, $"Data missing for {item3} which is a required field");
					return;
				}
				base.Cache[item3] = sortedList[item3];
				Smart.Log.Debug(TAG, $"Cache {item3} = {sortedList[item3]}");
			}
		}
		LogPass();
	}
}
