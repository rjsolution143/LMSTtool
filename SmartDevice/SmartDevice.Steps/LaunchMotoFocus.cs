using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using ISmart;

namespace SmartDevice.Steps;

public class LaunchMotoFocus : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Expected O, but got Unknown
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Expected O, but got Unknown
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		bool flag = val.ID.StartsWith("BULK_MOTOFOCUS");
		SortedList<string, object> sortedList = new SortedList<string, object>();
		Login login = Smart.Rsd.Login;
		sortedList["username"] = ((Login)(ref login)).UserName.Replace(',', '.');
		sortedList["password"] = string.Empty;
		sortedList["imei"] = val.SerialNumber;
		SortedList<string, string> sortedList2 = new SortedList<string, string>();
		login = Smart.Rsd.Login;
		sortedList2["rsdLoginId"] = ((Login)(ref login)).UserName;
		StationDescriptor stationDescriptor = Smart.Rsd.GetStationDescriptor();
		sortedList2["stationId"] = ((StationDescriptor)(ref stationDescriptor)).ToId().Replace(',', '.');
		stationDescriptor = Smart.Rsd.GetStationDescriptor();
		sortedList2["siteIdentifier"] = ((StationDescriptor)(ref stationDescriptor)).ShopId.Replace(',', '.');
		string text = "UNKNOWN";
		string text2 = "UNKNOWN";
		string[] array = val.ModelId.Split(new char[1] { '|' });
		if (array.Length != 0)
		{
			text2 = array[0];
		}
		if (array.Length > 1)
		{
			text = array[1];
		}
		sortedList2["rsdCarrierName"] = text;
		sortedList2["flexModel"] = text2;
		Smart.Log.Debug(TAG, string.Format("fsbParams Carrier:{0},Model:{1} for IMEI:{2}", text, text2, sortedList["imei"].ToString()));
		List<SortedList<string, string>> list = new List<SortedList<string, string>>();
		list.Add(sortedList2);
		sortedList["fsbParams"] = list;
		string text3 = Smart.Json.Dump((object)sortedList);
		_ = string.Empty;
		string empty = string.Empty;
		try
		{
			IWeb web = Smart.Web;
			login = Smart.Rsd.Login;
			empty = web.TokenRefresh(((Login)(ref login)).UserName);
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Error getting MotoFocus token " + ex.Message);
			Smart.Log.Verbose(TAG, ex.ToString());
			LogResult((Result)4, "Error getting MotoFocus token " + ex.Message);
			return;
		}
		SortedList<string, string> sortedList3 = new SortedList<string, string>();
		sortedList3["Content-Type"] = "application/json";
		sortedList3["Authorization"] = "Bearer " + empty;
		string text4 = "https://moto-focus.appspot.com/login";
		SortedList<string, string> sortedList4 = new SortedList<string, string>();
		if (!flag)
		{
			Smart.Log.Debug(TAG, "Starting internal web browser");
			try
			{
				sortedList4 = val.Prompt.WebBrowser(text4, text3, sortedList3);
			}
			catch (Exception ex2)
			{
				Smart.Log.Error(TAG, "Error during internal web browser session: " + ex2.ToString());
				throw new WebException("UI ERROR: " + ex2.Message, ex2);
			}
			Smart.Log.Debug(TAG, "Internal web browser session completed successfully");
		}
		else
		{
			Smart.Log.Debug(TAG, "Starting bulk web browser");
			if (((dynamic)base.Info.Args).RetryLoops == null)
			{
				((dynamic)base.Info.Args).RetryLoops = 3;
			}
			try
			{
				sortedList4 = Smart.TestUI.WebBrowser(text4, text3, sortedList3);
			}
			catch (Exception ex3)
			{
				Smart.Log.Error(TAG, "Error during bulk browser session: " + ex3.ToString());
				throw new WebException("BROWSER ERROR: " + ex3.Message, ex3);
			}
			Smart.Log.Debug(TAG, "Bulk web browser processing complete");
		}
		string text5 = Smart.Convert.ToString("ReturnHeaders", (IEnumerable<KeyValuePair<string, string>>)sortedList4.ToList());
		Smart.Log.Debug(TAG, "Return headers found: " + text5);
		if (!sortedList4.ContainsKey("set-cookie"))
		{
			throw new WebException("MotoFocus session info not returned");
		}
		string text6 = sortedList4["set-cookie"];
		string text7 = "session=";
		int num = text6.IndexOf(text7) + text7.Length;
		string value = "; Path=";
		int num2 = text6.IndexOf(value);
		string value2 = "; Secure;";
		if (text6.Contains(value2))
		{
			num2 = text6.IndexOf(value2);
		}
		string text8 = "UNKNOWN";
		try
		{
			text8 = text6.Substring(num, num2 - num);
		}
		catch (Exception innerException)
		{
			throw new WebException("Could not parse MotoFocus session ID", innerException);
		}
		Smart.Log.Debug(TAG, "Session ID found: " + text8);
		string text9 = "http://moto-focus.appspot.com/getdata";
		List<string> list2 = new List<string>();
		List<SortedList<string, string>> list3 = new List<SortedList<string, string>>();
		string text10 = "UNKNOWN";
		string text11 = "UNKNOWN";
		string text12 = "UNKNOWN";
		HttpClientHandler val2 = new HttpClientHandler();
		try
		{
			CookieContainer cookieContainer = new CookieContainer();
			val2.CookieContainer = cookieContainer;
			Cookie cookie = new Cookie("session", text8, "/");
			Uri uri = new Uri(text9);
			cookie.Domain = uri.Host;
			val2.CookieContainer.Add(cookie);
			HttpClient val3 = new HttpClient((HttpMessageHandler)(object)val2);
			try
			{
				Smart.Log.Debug(TAG, "Sending web request to " + text9);
				string result = val3.GetAsync(text9).Result.Content.ReadAsStringAsync().Result;
				Smart.Log.Debug(TAG, "Web response: " + result);
				foreach (string item in BracketSplitter(result))
				{
					if (item.Contains("\"Charts\""))
					{
						list2 = new List<string>(Smart.Json.LoadString<SortedList<string, List<string>>>(item)["Charts"]);
					}
					if (item.Contains("\"Warnings\":"))
					{
						list3 = new List<SortedList<string, string>>(Smart.Json.LoadString<SortedList<string, List<SortedList<string, string>>>>(item)["Warnings"]);
					}
					if (item.Contains("\"NTFScore\":"))
					{
						text10 = Smart.Json.LoadString<SortedList<string, string>>(item)["NTFScore"].ToString();
					}
					if (item.Contains("\"BatteryHealth\":"))
					{
						text11 = Smart.Json.LoadString<SortedList<string, string>>(item)["BatteryHealth"].ToString();
					}
					if (item.Contains("\"WarningRecommendations\":"))
					{
						text12 = Smart.Json.LoadString<SortedList<string, string>>(item)["WarningRecommendations"].ToString();
						text12.ToLowerInvariant().Contains("not");
					}
				}
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		base.Log.AddInfo("BatteryHealth", text11);
		base.Log.AddInfo("NTFScore", text10);
		base.Log.AddInfo("WarningsApplied", text12);
		foreach (string item2 in list2)
		{
			string text13 = new Regex("[^a-zA-Z0-9 ]").Replace(item2, "_");
			text13 = "Chart_" + text13;
			base.Log.AddResult(text13, text13, (Result)8, "Chart selected by user as failing", "", "", double.MinValue, double.MinValue, double.MinValue, (SortedList<string, object>)null);
		}
		foreach (SortedList<string, string> item3 in list3)
		{
			string input = item3["name"];
			string text14 = item3["value"];
			string text15 = new Regex("[^a-zA-Z0-9 ]").Replace(input, "_");
			text15 = "Warning_" + text15;
			double num3 = 0.0;
			Match match = new Regex("(\\d+(\\.\\d+)?)|(\\.\\d+)").Match(text14);
			if (match.Success || match.Groups.Count < 2)
			{
				num3 = double.Parse(match.Groups[1].Value);
			}
			base.Log.AddResult(text15, text15, (Result)8, text14, "", "", double.MinValue, double.MinValue, num3, (SortedList<string, object>)null);
		}
		LogPass();
	}

	private List<string> BracketSplitter(string json)
	{
		List<string> list = new List<string>();
		json = json.Trim();
		json = json.Trim(new char[1] { '"' });
		int num = 0;
		int num2 = 0;
		string text = string.Empty;
		string text2 = json;
		for (int i = 0; i < text2.Length; i++)
		{
			char c = text2[i];
			switch (c)
			{
			case '{':
				num++;
				break;
			case '}':
				num2++;
				break;
			}
			text += c;
			if (num == num2)
			{
				if (text.Trim() != string.Empty)
				{
					list.Add(text);
				}
				text = string.Empty;
				num = 0;
				num2 = 0;
			}
		}
		if (text.Trim() != string.Empty)
		{
			list.Add(text);
		}
		return list;
	}
}
