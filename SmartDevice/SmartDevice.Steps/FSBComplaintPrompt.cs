using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class FSBComplaintPrompt : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		SortedList<string, string> sortedList = new SortedList<string, string>();
		try
		{
			foreach (KeyValuePair<string, string> allComplaint in Smart.Fsb.GetAllComplaints())
			{
				sortedList.Add(allComplaint.Key, allComplaint.Value);
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, ex.Message);
			Smart.Log.Verbose(TAG, ex.ToString());
			sortedList.Add("No Complaint", "No Complaint");
		}
		string[] array = Smart.File.ClipboardRead().Split(new char[1] { '\n' });
		List<string> list = new List<string>();
		if (array.Length != 0 && array.Length < 4)
		{
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i].Trim().ToLowerInvariant();
				foreach (KeyValuePair<string, string> item in sortedList)
				{
					string text2 = item.Key.Trim().ToLowerInvariant();
					if (text == text2)
					{
						list.Add(item.Key);
					}
				}
			}
		}
		SortedList<string, string> sortedList2 = new SortedList<string, string>();
		if (list.Count > 0)
		{
			foreach (KeyValuePair<string, string> item2 in sortedList)
			{
				if (list.Contains(item2.Key))
				{
					sortedList2[item2.Key] = item2.Value;
				}
			}
			sortedList = sortedList2;
		}
		SortedList<string, string> sortedList3 = new SortedList<string, string>();
		try
		{
			foreach (KeyValuePair<string, string> allSymptom in Smart.Fsb.GetAllSymptoms())
			{
				sortedList3.Add(allSymptom.Key, allSymptom.Value);
			}
		}
		catch (Exception ex2)
		{
			Smart.Log.Error(TAG, ex2.Message);
			Smart.Log.Verbose(TAG, ex2.ToString());
			sortedList3.Add("No Symptom", "No Symptom");
		}
		SortedList<string, string> sortedList4 = null;
		try
		{
			sortedList4 = new SortedList<string, string>(Smart.Fsb.GetAllComplaintIcons());
		}
		catch (Exception ex3)
		{
			Smart.Log.Error(TAG, ex3.Message);
			Smart.Log.Verbose(TAG, ex3.ToString());
			sortedList4 = new SortedList<string, string>();
			foreach (string key in sortedList.Keys)
			{
				sortedList4.Add(key, "C:\\Windows\\System32\\SecurityAndMaintenance_Error.png");
			}
		}
		SortedList<string, string> sortedList5 = null;
		try
		{
			sortedList5 = new SortedList<string, string>(Smart.Fsb.GetAllSymptomIcons());
		}
		catch (Exception ex4)
		{
			Smart.Log.Error(TAG, ex4.Message);
			Smart.Log.Verbose(TAG, ex4.ToString());
			sortedList5 = new SortedList<string, string>();
			foreach (string key2 in sortedList3.Keys)
			{
				sortedList5.Add(key2, "C:\\Windows\\System32\\SecurityAndMaintenance_Error.png");
			}
		}
		Smart.Log.Verbose(TAG, $"Complaint options: {Smart.Convert.ToCommaSeparated((IEnumerable)sortedList.Keys)}");
		Smart.Log.Verbose(TAG, $"Symptom options: {Smart.Convert.ToCommaSeparated((IEnumerable)sortedList3.Keys)}");
		string text3 = Smart.Locale.Xlate("Complaint & Symptom Selection");
		string text4 = Smart.Locale.Xlate("Please select the CUSTOMER COMPLAINTS (Max 2)");
		string text5 = Smart.Locale.Xlate("Please select the SYMPTOMS OBSERVED (Max 2)");
		List<string> list2 = new List<string>();
		List<string> list3 = new List<string>();
		DialogResult val2 = val.Prompt.ComplaintSelect(text3, text4, sortedList, sortedList4, text5, sortedList3, sortedList5, ref list2, ref list3);
		List<string> list4 = new List<string>();
		foreach (string item3 in list2)
		{
			if (!sortedList.ContainsKey(item3))
			{
				string text6 = null;
				foreach (string key3 in sortedList.Keys)
				{
					if (sortedList[key3] == item3)
					{
						text6 = key3;
						break;
					}
				}
				if (text6 != null)
				{
					list4.Add(text6);
					continue;
				}
			}
			list4.Add(item3);
		}
		list2 = list4;
		list4 = new List<string>();
		foreach (string item4 in list3)
		{
			if (!sortedList3.ContainsKey(item4))
			{
				string text7 = null;
				foreach (string key4 in sortedList3.Keys)
				{
					if (sortedList3[key4] == item4)
					{
						text7 = key4;
						break;
					}
				}
				if (text7 != null)
				{
					list4.Add(text7);
					continue;
				}
			}
			list4.Add(item4);
		}
		list3 = list4;
		if (!Smart.Convert.ToBool(val2))
		{
			throw new OperationCanceledException("User canceled complaint/symptom selection");
		}
		Smart.Log.Debug(TAG, $"User complaints: {Smart.Convert.ToCommaSeparated((IEnumerable)list2)}");
		Smart.Log.Debug(TAG, $"User symptoms: {Smart.Convert.ToCommaSeparated((IEnumerable)list3)}");
		base.Cache["FSBComplaint"] = list2;
		base.Cache["FSBSymptom"] = list3;
		LogPass();
	}
}
