using System;
using System.Collections.Generic;
using ISmart;

namespace SmartUtil;

public class Alerts : IAlerts
{
	private string TAG => GetType().FullName;

	private SortedList<string, List<IAlert>> Languages { get; set; }

	public IAlert CurrentAlert { get; set; }

	public List<IAlert> Messages
	{
		get
		{
			string languageCode = Smart.Locale.LanguageCode;
			languageCode = languageCode.ToLowerInvariant();
			if (!Languages.ContainsKey(languageCode))
			{
				languageCode = "en";
			}
			if (!Languages.ContainsKey(languageCode))
			{
				return new List<IAlert>();
			}
			return Languages[languageCode];
		}
	}

	public Alerts()
	{
		Languages = new SortedList<string, List<IAlert>>();
		CurrentAlert = (IAlert)(object)new Alert("", (AlertTypes)2, "", "", DateTime.Now, DateTime.Now, DateTime.Now, "");
		CurrentAlert.TimeStart = DateTime.Now.Subtract(TimeSpan.FromMinutes(10.0));
		CurrentAlert.TimeEnd = DateTime.Now.Subtract(TimeSpan.FromMinutes(5.0));
		CurrentAlert.Alertstatus = (AlertStatus)0;
		Load();
		Refresh();
	}

	private SortedList<string, List<IAlert>> ParseJson(dynamic alertsData)
	{
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_085d: Unknown result type (might be due to invalid IL or missing references)
		//IL_095c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0833: Unknown result type (might be due to invalid IL or missing references)
		//IL_0838: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b62: Unknown result type (might be due to invalid IL or missing references)
		SortedList<string, List<IAlert>> sortedList = new SortedList<string, List<IAlert>>();
		DateTime dateTime = DateTime.Now.Subtract(TimeSpan.FromMinutes(10.0));
		DateTime dateTime2 = DateTime.Now.Subtract(TimeSpan.FromMinutes(5.0));
		string empty = string.Empty;
		string value = string.Empty;
		if (alertsData == null || alertsData["messages"] == null)
		{
			Smart.Log.Debug(TAG, "No Alert JSON content to load");
			return sortedList;
		}
		foreach (dynamic item2 in alertsData["messages"])
		{
			string id = item2["msgid"];
			DateTime timestamp = DateTime.Parse(item2["datetime"]);
			AlertTypes alerttype = (AlertTypes)3;
			bool isRead = false;
			bool? flag = item2["isread"];
			if (flag.HasValue && flag == true)
			{
				isRead = true;
			}
			empty = item2["title"];
			value = string.Empty;
			if (empty == null)
			{
				empty = string.Empty;
			}
			string text = item2["type"];
			if (text != null && text.Trim().ToLowerInvariant().Contains("downtime"))
			{
				string text2 = item2["start_time"];
				string text3 = item2["end_time"];
				dateTime = DateTime.Now.Subtract(TimeSpan.FromMinutes(10.0));
				dateTime2 = DateTime.Now.Subtract(TimeSpan.FromMinutes(5.0));
				DateTime dateTime3 = Smart.Convert.ServerToLocalTime(text2);
				DateTime dateTime4 = Smart.Convert.ServerToLocalTime(text3);
				if (dateTime4 > DateTime.Now && (dateTime3 < dateTime || dateTime2 < DateTime.Now))
				{
					dateTime = dateTime3;
					dateTime2 = dateTime4;
				}
				if (text != null && text.Contains("-"))
				{
					alerttype = (AlertTypes)Enum.Parse(typeof(AlertTypes), text.Replace("-", "").Trim(), ignoreCase: true);
				}
			}
			if (text != null && text.Trim().ToLowerInvariant().Contains("clientrelease"))
			{
				alerttype = (AlertTypes)2;
				dateTime = DateTime.Now.Subtract(TimeSpan.FromDays(999.0));
				string text4 = item2["start_time"];
				DateTime dateTime5 = Smart.Convert.ServerToLocalTime(text4);
				if (dateTime5 > dateTime)
				{
					dateTime = dateTime5;
				}
				dateTime2 = dateTime.AddDays(15.0);
			}
			if (text != null && text.Trim().ToLowerInvariant().Contains("notification"))
			{
				alerttype = (AlertTypes)3;
			}
			foreach (dynamic item3 in item2["translated_msgs"])
			{
				string text5 = item3.Name;
				text5 = text5.ToLowerInvariant();
				string text6 = item3.Value;
				string text7 = empty;
				if (empty == string.Empty)
				{
					text7 = text6;
					if (text7.Length > 15)
					{
						text7 = text7.Substring(0, 15) + "...";
					}
				}
				Alert item = new Alert(id, alerttype, text7, text6, timestamp, dateTime, dateTime2, text5, isRead);
				if (string.IsNullOrEmpty(value) && Smart.Locale.LanguageCode == text5)
				{
					value = text6;
				}
				if (!sortedList.ContainsKey(text5))
				{
					sortedList[text5] = new List<IAlert>();
				}
				sortedList[text5].Add((IAlert)(object)item);
			}
		}
		if (sortedList.ContainsKey("en"))
		{
			List<IAlert> list = sortedList["en"];
			foreach (string key in sortedList.Keys)
			{
				if (key == "en")
				{
					continue;
				}
				List<IAlert> list2 = sortedList[key];
				foreach (IAlert item4 in list)
				{
					if (string.IsNullOrEmpty(value))
					{
						value = item4.Message;
					}
					bool flag2 = false;
					foreach (IAlert item5 in list2)
					{
						if (item5.ID == item4.ID)
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						list2.Add(item4);
					}
				}
			}
		}
		return sortedList;
	}

	private void Save()
	{
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		string text = Smart.File.ResourceNameToFilePath("alerts", ".json");
		SortedList<string, object> sortedList = new SortedList<string, object>();
		List<object> list2 = (List<object>)(sortedList["messages"] = new List<object>());
		SortedList<string, DateTime> sortedList2 = new SortedList<string, DateTime>();
		foreach (string key in Languages.Keys)
		{
			foreach (IAlert item in Languages[key])
			{
				string iD = item.ID;
				sortedList2[iD] = item.Timestamp;
			}
		}
		foreach (string key2 in sortedList2.Keys)
		{
			DateTime dateTime = sortedList2[key2];
			SortedList<string, object> sortedList3 = new SortedList<string, object>();
			list2.Add(sortedList3);
			sortedList3["msgid"] = key2;
			sortedList3["datetime"] = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
			SortedList<string, string> sortedList5 = (SortedList<string, string>)(sortedList3["translated_msgs"] = new SortedList<string, string>());
			bool flag = false;
			AlertTypes val = (AlertTypes)3;
			string value = ((object)(AlertTypes)(ref val)).ToString();
			DateTime dateTime2 = DateTime.Now;
			DateTime dateTime3 = DateTime.Now;
			foreach (string key3 in Languages.Keys)
			{
				foreach (IAlert item2 in Languages[key3])
				{
					if (!(item2.ID != key2))
					{
						if (item2.IsRead)
						{
							flag = true;
						}
						val = item2.AlertType;
						value = ((object)(AlertTypes)(ref val)).ToString();
						dateTime2 = item2.TimeStart;
						dateTime3 = item2.TimeEnd;
						sortedList5[item2.LanguageCode] = item2.Message;
					}
				}
			}
			sortedList3["isread"] = flag;
			sortedList3["alerttype"] = value;
			sortedList3["timestart"] = dateTime2;
			sortedList3["timeend"] = dateTime3;
		}
		string text2 = Smart.Json.Dump((object)sortedList);
		Smart.File.WriteText(text, text2);
	}

	private void Load()
	{
		string text = Smart.File.ResourceNameToFilePath("alerts", ".json");
		if (!Smart.File.Exists(text))
		{
			Smart.Log.Debug(TAG, "No alerts.json file found, skipping load");
			return;
		}
		try
		{
			string text2 = Smart.File.ReadText(text);
			dynamic val = Smart.Json.Load(text2);
			SortedList<string, List<IAlert>> languages = ParseJson(val);
			Languages = languages;
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, $"Error reading Alerts content: {ex.Message}");
			Smart.Log.Debug(TAG, ex.ToString());
		}
	}

	private void Delete()
	{
		try
		{
			string text = Smart.File.ResourceNameToFilePath("alerts", ".json");
			if (Smart.File.Exists(text))
			{
				Smart.File.Delete(text);
				Smart.Log.Debug(TAG, "Local alerts.json was deleted");
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, $"Error delete Alerts content: {ex.Message}");
			Smart.Log.Debug(TAG, ex.ToString());
		}
	}

	public void Refresh()
	{
		string text = "";
		try
		{
			text = Smart.Rsd.GetJsonPushNotification();
			if (string.IsNullOrEmpty(text) || text.Contains("no new message found"))
			{
				Delete();
				Languages.Clear();
				Messages.Clear();
				return;
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, $"Error during Alerts refresh: {ex.Message}");
			Smart.Log.Debug(TAG, ex.ToString());
			return;
		}
		SortedList<string, List<IAlert>> sortedList = new SortedList<string, List<IAlert>>();
		try
		{
			dynamic val = Smart.Json.Load(text);
			sortedList = ParseJson(val);
		}
		catch (Exception ex2)
		{
			Smart.Log.Error(TAG, $"Error during Alerts refresh loading: {ex2.Message}");
			Smart.Log.Debug(TAG, ex2.ToString());
		}
		if (sortedList.Count < 1)
		{
			Smart.Log.Debug(TAG, "No Alerts JSON content found, skipping refresh");
			return;
		}
		DateTime dateTime = DateTime.Now.AddDays(14.0);
		DateTime dateTime2 = dateTime;
		List<string> list = new List<string>();
		foreach (string key in Languages.Keys)
		{
			foreach (IAlert item in Languages[key])
			{
				if (item.Timestamp < dateTime2)
				{
					dateTime2 = item.Timestamp;
				}
				if (!list.Contains(item.ID) && item.IsRead)
				{
					list.Add(item.ID);
				}
			}
		}
		if (dateTime2 == dateTime)
		{
			dateTime2 = DateTime.MinValue;
		}
		foreach (string item2 in list)
		{
			foreach (string key2 in sortedList.Keys)
			{
				foreach (IAlert item3 in sortedList[key2])
				{
					if (item3.Timestamp < dateTime2)
					{
						((Alert)(object)item3).IsRead = true;
					}
					else if (item3.ID == item2)
					{
						((Alert)(object)item3).IsRead = true;
					}
				}
			}
		}
		foreach (List<IAlert> value in sortedList.Values)
		{
			value.Sort();
			value.Reverse();
		}
		Languages = sortedList;
		Save();
	}

	public void MarkRead(IAlert alert)
	{
		foreach (IAlert message in Messages)
		{
			if (message.ID == alert.ID)
			{
				message.IsRead = true;
			}
		}
		Save();
	}
}
