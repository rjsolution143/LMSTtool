using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using ISmart;

namespace SmartUtil;

public class Locale : ILocale
{
	private CultureInfo DEFAULT_CULTURE = new CultureInfo("en-US");

	private string TAG => GetType().FullName;

	public string LanguageCode { get; private set; }

	private dynamic Xlations { get; set; }

	private dynamic MissingXlations { get; set; }

	public Locale()
	{
		LanguageCode = "xx";
		string text = Smart.File.ResourceNameToFilePath("xlate", ".json");
		if (!Smart.File.Exists(text))
		{
			Smart.Log.Debug(TAG, $"No xlate file found at {text}");
			try
			{
				Smart.File.WriteText(text, "{\"FileFormatVersion\":1.0}");
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, ex.Message);
				Smart.Log.Verbose(TAG, ex.ToString());
			}
		}
		try
		{
			SetXlatePath(text);
		}
		catch (Exception ex2)
		{
			Smart.Log.Error(TAG, ex2.Message);
			Smart.Log.Verbose(TAG, ex2.ToString());
		}
		string text2 = Smart.File.ResourceNameToFilePath("missing_xlate", ".json");
		string text3 = "{\"FileFormatVersion\":1.0}";
		if (Smart.File.Exists(text2))
		{
			try
			{
				string text4 = Smart.File.ReadText(text2);
				if (text4.Trim() == string.Empty)
				{
					throw new NotSupportedException("Missing xlate content is empty");
				}
				text3 = text4;
				MissingXlations = Smart.Json.Load(text3);
			}
			catch (Exception ex3)
			{
				Smart.Log.Error(TAG, ex3.Message);
				Smart.Log.Verbose(TAG, ex3.ToString());
			}
		}
		if (Xlations != null && Xlations["LanguageCode"] != null && Xlations["LanguageCode"] != string.Empty)
		{
			LanguageCode = Xlations["LanguageCode"];
		}
	}

	public void SetXlatePath(string path)
	{
		try
		{
			string text = Smart.File.ReadText(path);
			Xlations = Smart.Json.Load(text);
			if (Xlations != null && Xlations["LanguageCode"] != null && Xlations["LanguageCode"] != string.Empty)
			{
				LanguageCode = Xlations["LanguageCode"];
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, ex.Message);
			Smart.Log.Verbose(TAG, ex.ToString());
			throw;
		}
	}

	public void SetDefaultCulture()
	{
		Type typeFromHandle = typeof(CultureInfo);
		CultureInfo dEFAULT_CULTURE = DEFAULT_CULTURE;
		bool flag = System.Threading.Thread.CurrentThread.CurrentCulture.Name != DEFAULT_CULTURE.Name;
		if (flag)
		{
			Smart.Log.Debug(TAG, "Current culture: " + System.Threading.Thread.CurrentThread.CurrentCulture.Name);
			Smart.Log.Debug(TAG, "Target culture: " + DEFAULT_CULTURE.Name);
		}
		bool flag2 = false;
		try
		{
			typeFromHandle.InvokeMember("s_userDefaultCulture", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, dEFAULT_CULTURE, new object[1] { dEFAULT_CULTURE });
			typeFromHandle.InvokeMember("s_userDefaultUICulture", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, dEFAULT_CULTURE, new object[1] { dEFAULT_CULTURE });
			flag2 = true;
		}
		catch
		{
		}
		try
		{
			typeFromHandle.InvokeMember("m_userDefaultCulture", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, dEFAULT_CULTURE, new object[1] { dEFAULT_CULTURE });
			typeFromHandle.InvokeMember("m_userDefaultUICulture", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, dEFAULT_CULTURE, new object[1] { dEFAULT_CULTURE });
			flag2 = true;
		}
		catch
		{
		}
		if (flag2)
		{
			Smart.Log.Debug(TAG, "Default culture set: " + DEFAULT_CULTURE.Name);
		}
		else
		{
			Smart.Log.Warning(TAG, "Could not set default culture: " + DEFAULT_CULTURE.Name);
		}
		if (flag)
		{
			Smart.Log.Debug(TAG, "New culture: " + System.Threading.Thread.CurrentThread.CurrentCulture.Name);
		}
	}

	public string Xlate(string text)
	{
		string text2 = Xlations[text];
		if (string.IsNullOrWhiteSpace(text2))
		{
			if (text2 == null)
			{
				try
				{
					if (MissingXlations == null)
					{
						lock (this)
						{
							if (MissingXlations == null)
							{
								MissingXlations = new SortedList<string, string>();
								MissingXlations["FileFormatVersion"] = "0.9";
							}
						}
					}
					MissingXlations[text] = string.Empty;
					string filePath = Smart.File.ResourceNameToFilePath("missing_xlate", ".json");
					string content = Smart.Json.Dump(MissingXlations);
					WriteWithRetry(filePath, content);
				}
				catch (Exception ex)
				{
					Smart.Log.Error(TAG, $"Xlate translation failed: {ex.Message}");
					Smart.Log.Verbose(TAG, $"Xlate translation failed: {ex.ToString()}");
				}
			}
			text2 = text;
		}
		return text2;
	}

	private void WriteWithRetry(string filePath, string content)
	{
		int num = 3;
		int num2 = 1000;
		int num3 = 0;
		while (num3 < num)
		{
			try
			{
				string directoryName = Path.GetDirectoryName(filePath);
				if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				Smart.File.WriteText(filePath, content);
				break;
			}
			catch (IOException ex) when (ex.HResult == -2147024864)
			{
				num3++;
				if (num3 >= num)
				{
					throw new IOException($"File write failed; the max {num} retries has been reached.", ex);
				}
				Smart.Log.Verbose(TAG, $"The file is being used by another process，waiting {num2}ms and retry for {num3} times");
				System.Threading.Thread.Sleep(num2);
			}
		}
	}
}
