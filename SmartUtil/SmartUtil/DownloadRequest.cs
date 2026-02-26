using System;
using System.Collections.Generic;
using ISmart;

namespace SmartUtil;

public class DownloadRequest : IDownloadRequest
{
	private string DOWNLOAD_HANDLER = "SmartHelper";

	private string TAG => GetType().FullName;

	public Login Credentials { get; private set; }

	public string AppName { get; private set; }

	public string AppVersion { get; private set; }

	public string FormatVersion { get; private set; }

	public string Target { get; private set; }

	public DownloadRequestType RequestType { get; private set; }

	public string Json
	{
		get
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			SortedList<string, object> sortedList = new SortedList<string, object>();
			Login credentials = Credentials;
			sortedList["user"] = ((Login)(ref credentials)).UserName;
			sortedList["pass"] = string.Empty;
			sortedList["format"] = FormatVersion;
			sortedList["app"] = AppName;
			sortedList["appVersion"] = AppVersion;
			sortedList["downloads"] = Downloads;
			sortedList["target"] = Target;
			DownloadRequestType requestType = RequestType;
			sortedList["requestType"] = ((object)(DownloadRequestType)(ref requestType)).ToString();
			return Smart.Json.Dump((object)sortedList);
		}
	}

	public bool Available => Smart.Messages.IsChannelOpen(DOWNLOAD_HANDLER);

	public List<SortedList<string, string>> Downloads { get; private set; }

	public DownloadRequest()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Downloads = new List<SortedList<string, string>>();
		Credentials = Smart.Rsd.Login;
		AppName = Smart.App.Name;
		AppVersion = Smart.App.Version;
		Target = string.Empty;
		FormatVersion = "1.0";
		RequestType = (DownloadRequestType)1;
	}

	public void Load(string jsonContent)
	{
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		Downloads = new List<SortedList<string, string>>();
		dynamic val = Smart.Json.Load(jsonContent);
		string text = val["user"];
		string text2 = val["pass"];
		Login credentials = default(Login);
		((Login)(ref credentials))._002Ector(text, text2);
		Credentials = credentials;
		string appName = val["app"];
		string appVersion = val["appVersion"];
		AppName = appName;
		AppVersion = appVersion;
		string formatVersion = val["format"];
		FormatVersion = formatVersion;
		string text3 = val["target"];
		if (text3 == null)
		{
			text3 = string.Empty;
		}
		Target = text3;
		string text4 = val["requestType"];
		if (text4 == null)
		{
			text4 = "Download";
		}
		if (text4.ToLowerInvariant() == "new")
		{
			text4 = "Download";
		}
		RequestType = (DownloadRequestType)Enum.Parse(typeof(DownloadRequestType), text4);
		foreach (dynamic item in val["downloads"])
		{
			SortedList<string, string> sortedList = new SortedList<string, string>();
			foreach (dynamic item2 in item)
			{
				sortedList[item2.Name] = item2.Value.ToString();
			}
			Downloads.Add(sortedList);
		}
	}

	public void AddDownload(string name, string remoteUrl, string localFile, bool unzip, bool createFolder)
	{
		SortedList<string, string> sortedList = new SortedList<string, string>();
		sortedList["name"] = name;
		sortedList["remoteUrl"] = remoteUrl;
		sortedList["localFile"] = localFile;
		sortedList["unzip"] = unzip.ToString();
		sortedList["createFolder"] = createFolder.ToString();
		Downloads.Add(sortedList);
	}

	private DownloadResponse Send()
	{
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		string text = Smart.Messages.SendMessage(DOWNLOAD_HANDLER, Json);
		try
		{
			dynamic val = Smart.Json.Load(text);
			return (DownloadResponse)DownloadResponse.FromData(val);
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Could not parse DownloadRequest response: " + ex.Message);
			Smart.Log.Error(TAG, ex.ToString());
			return DownloadResponse.BlankStatus;
		}
	}

	public DownloadResponse Prioritize(string remoteUrl)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		RequestType = (DownloadRequestType)4;
		Target = remoteUrl;
		return Send();
	}

	public DownloadResponse Status(string remoteUrl)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		RequestType = (DownloadRequestType)2;
		Target = remoteUrl;
		return Send();
	}

	public DownloadResponse Status()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		RequestType = (DownloadRequestType)1;
		return Send();
	}

	public DownloadResponse RequestDownloads()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		RequestType = (DownloadRequestType)0;
		if (Downloads.Count < 1)
		{
			throw new NotSupportedException("Download request must contain at least one download item");
		}
		DownloadResponse result = Send();
		if (((DownloadResponse)(ref result)).Success)
		{
			Smart.Maintenance.ReportEvent("SmartDownloader");
		}
		return result;
	}

	public DownloadResponse Open()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		RequestType = (DownloadRequestType)3;
		return Send();
	}
}
