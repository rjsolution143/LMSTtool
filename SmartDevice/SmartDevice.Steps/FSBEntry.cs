using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class FSBEntry : BaseStep
{
	private object logHandle;

	private SortedList<string, dynamic> logDetails = new SortedList<string, object>();

	private Result logResult = (Result)2;

	private string TAG => GetType().FullName;

	public override void Setup()
	{
		base.Setup();
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		logHandle = Smart.Fsb.GetFsbLogHandle(val);
	}

	public override void TearDown()
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		base.TearDown();
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		Smart.Fsb.AddRecordToFsbLog(logHandle, new UpgradeLogRecord((SortedList<string, object>)logDetails, logResult, (UseCase)162, false, val.Log.RsdLogId), val);
		Smart.Fsb.FinalizeFsbLog(logHandle);
	}

	public override void Run()
	{
		//IL_089c: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1965: Unknown result type (might be due to invalid IL or missing references)
		//IL_1948: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f8a: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		Smart.Fsb.GetFsbLogHandle(val);
		string serialNumber = val.SerialNumber;
		List<string> list = base.Cache["FSBComplaint"];
		List<string> list2 = base.Cache["FSBSymptom"];
		string modelId = val.ModelId;
		List<string> list3 = new List<string>();
		list3.AddRange(list);
		list3.AddRange(list2);
		Smart.Log.Debug(TAG, $"Searching for FSBs for SN {serialNumber}, model {modelId}, and choices {Smart.Convert.ToCommaSeparated((IEnumerable)list3)}");
		List<string> fSBNumbers = Smart.Fsb.GetFSBNumbers(serialNumber, list, list2, modelId);
		Smart.Log.Debug(TAG, $"Found {fSBNumbers.Count} FSBs");
		SortedList<string, Tuple<object, object>> sortedList = new SortedList<string, Tuple<object, object>>();
		foreach (string item24 in fSBNumbers)
		{
			Tuple<string, string> jsonFsb = Smart.Fsb.GetJsonFsb(item24);
			dynamic val2 = Smart.Json.Load(jsonFsb.Item1);
			dynamic val3 = Smart.Json.Load(jsonFsb.Item2);
			sortedList[item24] = new Tuple<object, object>(val2, val3);
		}
		SortedList<string, Tuple<string, string>> sortedList2 = new SortedList<string, Tuple<string, string>>();
		Smart.Log.Debug(TAG, $"Found unique content for {sortedList.Count} FSBs");
		foreach (string key in sortedList.Keys)
		{
			dynamic item = sortedList[key].Item2;
			string item2 = item["name"].ToString();
			string item3 = item["issue_desc"].ToString();
			Tuple<string, string> value = new Tuple<string, string>(item2, item3);
			sortedList2.Add(key, value);
		}
		List<string> list4 = new List<string>();
		string item4 = Smart.Locale.Xlate("FSB Number");
		list4.Add(item4);
		string item5 = Smart.Locale.Xlate("FSB Name");
		list4.Add(item5);
		string item6 = Smart.Locale.Xlate("FSB Issue");
		list4.Add(item6);
		string item7 = Smart.Locale.Xlate("Status");
		list4.Add(item7);
		List<string> list5 = new List<string>();
		foreach (string item25 in list)
		{
			string item8 = Smart.Locale.Xlate(item25);
			list5.Add(item8);
		}
		List<string> list6 = new List<string>();
		foreach (string item26 in list2)
		{
			string item9 = Smart.Locale.Xlate(item26);
			list6.Add(item9);
		}
		List<string> list7 = new List<string>();
		string text = Smart.Locale.Xlate("IMEI");
		list7.Add(text + ": " + val.SerialNumber);
		string text2 = Smart.Locale.Xlate("Model");
		list7.Add(text2 + ": " + val.ModelId);
		string text3 = Smart.Locale.Xlate("Complaint");
		list7.Add(text3 + ": " + Smart.Convert.ToCommaSeparated((IEnumerable)list5));
		string text4 = Smart.Locale.Xlate("Symptoms");
		list7.Add(text4 + ": " + Smart.Convert.ToCommaSeparated((IEnumerable)list6));
		string value2 = string.Empty;
		string value3 = string.Empty;
		string[] array = val.ModelId.Split(new char[1] { '|' });
		if (array.Length != 0)
		{
			value2 = array[0];
		}
		if (array.Length > 1)
		{
			value3 = array[1];
		}
		logDetails["imei"] = serialNumber;
		logDetails["Model"] = value2;
		logDetails["Carrier"] = value3;
		logDetails["CustomerComplaint"] = Smart.Convert.ToCommaSeparated((IEnumerable)list);
		logDetails["SymptomFound"] = Smart.Convert.ToCommaSeparated((IEnumerable)list2);
		if (sortedList2.Count < 1)
		{
			Smart.Log.Debug(TAG, "No FSBs found");
			string text5 = Smart.Locale.Xlate("No Applicable FSB found for selected Model, Complaint and Symptoms");
			string text6 = Smart.Locale.Xlate("No FSB");
			string text7 = string.Empty;
			foreach (string item27 in list7)
			{
				text7 = text7 + item27 + Environment.NewLine;
			}
			text7 += text5;
			val.Prompt.MessageBox(text6, text7, (MessageBoxButtons)0, (MessageBoxIcon)64);
		}
		string text8 = string.Empty;
		int num = -1;
		while (sortedList2.Count > 0)
		{
			num++;
			Smart.Log.Debug(TAG, $"Processing FSB{num}");
			Smart.Log.Debug(TAG, $"Showing FSB List with {sortedList2.Count} items");
			string text9 = Smart.Locale.Xlate("Applicable FSBs");
			string empty = string.Empty;
			string text10 = Smart.Locale.Xlate("Validate");
			DialogResult val4 = val.Prompt.FSBList(text9, list4, text10, list7, sortedList2, ref empty);
			if (!Smart.Convert.ToBool(val4))
			{
				throw new OperationCanceledException("User canceled FSB process");
			}
			if (empty == string.Empty)
			{
				num--;
				continue;
			}
			logDetails["FSBNumber" + num] = empty;
			logDetails["FSBName" + num] = sortedList2[empty].Item1;
			Smart.Log.Debug(TAG, $"User selected FSB {empty} - {sortedList2[empty].Item1}");
			List<string> list8 = new List<string>(list7);
			list8.Insert(0, empty);
			list8.Insert(1, sortedList2[empty].Item1);
			List<string> list9 = new List<string>();
			string item10 = Smart.Locale.Xlate("Action Name");
			list9.Add(item10);
			string item11 = Smart.Locale.Xlate("Action Description");
			list9.Add(item11);
			string item12 = Smart.Locale.Xlate("Picture");
			list9.Add(item12);
			string item13 = Smart.Locale.Xlate("Action Remarks");
			list9.Add(item13);
			string item14 = Smart.Locale.Xlate("Result");
			list9.Add(item14);
			List<string> pictureFilePathNames = Smart.Fsb.GetPictureFilePathNames(empty);
			List<string> list10 = new List<string>();
			list10.Add("Applied");
			list10.Add("Not Duplicated");
			list10.Add("No Components");
			List<string> list11 = new List<string>();
			string item15 = Smart.Locale.Xlate("Applied");
			list11.Add(item15);
			string item16 = Smart.Locale.Xlate("Not Duplicated");
			list11.Add(item16);
			string item17 = Smart.Locale.Xlate("No Components");
			list11.Add(item17);
			List<int> list12 = new List<int>();
			List<Tuple<string, string, string, string, List<string>>> list13 = new List<Tuple<string, string, string, string, List<string>>>();
			int num2 = 0;
			foreach (dynamic item28 in ((dynamic)sortedList[empty].Item2)["actions"])
			{
				string item18 = pictureFilePathNames[num2];
				num2++;
				dynamic val5 = item28.Value;
				string text11 = val5["name"].ToString();
				if (!(text11 == string.Empty))
				{
					string item19 = val5["description"].ToString();
					string item20 = val5["confirmation_needed"].ToString();
					Tuple<string, string, string, string, List<string>> item21 = new Tuple<string, string, string, string, List<string>>(text11, item19, item18, item20, list11);
					list13.Add(item21);
				}
			}
			string arg = Smart.Locale.Xlate("FSB");
			string arg2 = Smart.Locale.Xlate("Actions");
			string arg3 = Smart.Locale.Xlate("(This FSB is part of IMEI list)");
			string text12 = $"{arg} {empty} - {arg2}";
			string imeiList = Smart.Fsb.GetImeiList(empty);
			Smart.Log.Debug(TAG, $"FSB {empty} IMEI list: {imeiList}");
			if (imeiList.ToLowerInvariant().Contains(serialNumber.ToLowerInvariant()))
			{
				text12 = $"{text12} {arg3}";
				Smart.Log.Debug(TAG, $"IMEI {serialNumber} found in IMEI list");
			}
			else
			{
				Smart.Log.Debug(TAG, $"IMEI {serialNumber} not found in IMEI list");
			}
			val4 = val.Prompt.FSBEntry(text12, list9, list8, list13, ref list12);
			if (!Smart.Convert.ToBool(val4))
			{
				Smart.Log.Debug(TAG, "User cancelled FSB entry");
				num--;
				continue;
			}
			Smart.Log.Debug(TAG, $"User selected choices {Smart.Convert.ToCommaSeparated((IEnumerable)list12)}");
			bool flag = false;
			bool flag2 = true;
			bool flag3 = false;
			foreach (int item29 in list12)
			{
				if (item29 != 1)
				{
					flag2 = false;
				}
				if (item29 == 0)
				{
					flag = true;
				}
				if (item29 == 2)
				{
					flag3 = true;
				}
			}
			string value4 = "Unknown";
			if (flag2)
			{
				value4 = "Not Duplicated";
			}
			else if (flag3 && !flag)
			{
				value4 = "No Components";
			}
			else if (flag)
			{
				value4 = "Applied";
			}
			logDetails["Status" + num] = value4;
			string repairCodePriorityFileContent = Smart.Fsb.GetRepairCodePriorityFileContent();
			ICsvFile val6 = Smart.NewCsvFile();
			val6.Load(repairCodePriorityFileContent, ',');
			int num3 = int.MaxValue;
			Tuple<string, string, string> tuple = null;
			foreach (Tuple<string, string, string, string, List<string>> item30 in list13)
			{
				int num4 = list13.IndexOf(item30);
				dynamic val7 = ((dynamic)sortedList[empty].Item1)["actions"]["action" + (num4 + 1)];
				dynamic val8 = ((dynamic)sortedList[empty].Item2)["actions"]["action" + (num4 + 1)];
				string text13 = $"{num4 + 1}{num}";
				logDetails["ActionName" + text13] = (object)val7["name"];
				logDetails["Part" + text13] = (object)val7["parts"];
				logDetails["Remark" + text13] = (object)val7["confirmation_needed"];
				logDetails["Result" + text13] = list10[list12[num4]];
				logDetails["ProblemFound" + text13] = (object)val7["claim_problem_found"];
				logDetails["RepairAction" + text13] = (object)val7["repair_action"];
				string text14 = val7["repair_action"];
				if (text14.Contains("-"))
				{
					text14 = text14.Substring(0, text14.IndexOf("-"));
				}
				int num5 = 2147483646;
				foreach (SortedList<string, string> item31 in (IEnumerable<SortedList<string, string>>)val6)
				{
					if (!(item31["Repair Codes"].ToLowerInvariant() != text14.ToLowerInvariant()))
					{
						num5 = int.Parse(item31["Priority"]);
						break;
					}
				}
				if (num5 < num3 && list12[num4] != 1)
				{
					num3 = num5;
					string item22 = val8["claim_problem_found"];
					string text15 = val8["repair_action"];
					Smart.Log.Debug(TAG, $"New highest priority is {text15} repair code");
					string item23 = val8["parts"];
					tuple = new Tuple<string, string, string>(item22, text15, item23);
				}
			}
			if (num3 < int.MaxValue && tuple != null)
			{
				Smart.Log.Debug(TAG, $"Highest priority was {tuple.Item2} repair code");
				string text16 = Smart.Locale.Xlate("Problem Found");
				string text17 = Smart.Locale.Xlate("Repair Action");
				string text18 = Smart.Locale.Xlate("Parts");
				text8 = string.Empty;
				text8 = text8 + text16 + ": " + tuple.Item1 + Environment.NewLine;
				text8 = text8 + text17 + ": " + tuple.Item2 + Environment.NewLine;
				text8 = text8 + text18 + ": " + tuple.Item3 + Environment.NewLine;
			}
			Smart.Log.Debug(TAG, "Finished FSB entry");
			sortedList2.Remove(empty);
		}
		if (text8 != string.Empty)
		{
			string text19 = Smart.Locale.Xlate("Claim Codes and Parts Selection");
			val.Prompt.MessageBox(text19, text8, (MessageBoxButtons)0, (MessageBoxIcon)64);
		}
		Smart.Log.Debug(TAG, "Finished all FSB entry");
		logResult = (Result)8;
		LogPass();
	}
}
