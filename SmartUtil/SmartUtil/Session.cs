using System.Collections.Generic;
using System.IO;
using ISmart;

namespace SmartUtil;

public class Session : ISession
{
	private string TAG => GetType().FullName;

	protected string SessionFilePath => Path.Combine(Smart.File.CommonStorageDir, "session.json");

	public void Delete(SessionType type)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Save(type, new SortedList<string, string>());
	}

	public bool IsSaved(SessionType type)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return Load(type).Count > 0;
	}

	public SortedList<string, string> Load(SessionType type)
	{
		string sessionFilePath = SessionFilePath;
		if (!Smart.File.Exists(sessionFilePath))
		{
			return new SortedList<string, string>();
		}
		string text = Smart.File.ReadText(sessionFilePath);
		string text2 = Smart.Security.DecryptString(text);
		SortedList<string, SortedList<string, string>> sortedList = Smart.Json.LoadString<SortedList<string, SortedList<string, string>>>(text2);
		string key = ((object)(SessionType)(ref type)).ToString().ToLowerInvariant();
		if (!sortedList.ContainsKey(key))
		{
			return new SortedList<string, string>();
		}
		return sortedList[key];
	}

	public void Save(SessionType type, SortedList<string, string> data)
	{
		string sessionFilePath = SessionFilePath;
		SortedList<string, SortedList<string, string>> sortedList = new SortedList<string, SortedList<string, string>>();
		if (Smart.File.Exists(sessionFilePath))
		{
			string text = Smart.File.ReadText(sessionFilePath);
			string text2 = Smart.Security.DecryptString(text);
			sortedList = Smart.Json.LoadString<SortedList<string, SortedList<string, string>>>(text2);
		}
		string key = ((object)(SessionType)(ref type)).ToString().ToLowerInvariant();
		sortedList[key] = data;
		string text3 = Smart.Json.Dump((object)sortedList);
		string text4 = Smart.Security.EncryptString(text3);
		Smart.File.WriteText(sessionFilePath, text4);
	}
}
