using System;
using System.Collections.Generic;
using System.Net;

namespace SmartWeb;

public abstract class RestService : WebService
{
	private string TAG => GetType().FullName;

	public TimeSpan Timeout { get; protected set; } = TimeSpan.FromSeconds(100.0);


	protected SortedList<string, string> ExtraHeaders { get; set; } = new SortedList<string, string>();


	protected dynamic SendContent(string input, string contentType)
	{
		using WebClientTimeout webClientTimeout = new WebClientTimeout();
		webClientTimeout.Timeout = Timeout;
		Smart.Log.Verbose(TAG, $"Adding Authorization header with value '{BasicAuthentication}'");
		webClientTimeout.Headers[HttpRequestHeader.Authorization] = BasicAuthentication;
		SentRequest(input);
		webClientTimeout.Headers[HttpRequestHeader.ContentType] = contentType;
		if (ExtraHeaders.Count > 0)
		{
			foreach (string key in ExtraHeaders.Keys)
			{
				string value = ExtraHeaders[key];
				webClientTimeout.Headers.Add(key, value);
			}
		}
		string text = webClientTimeout.UploadString(base.Url, "POST", input);
		ReceivedReply(text);
		return ParseResponse(text);
	}

	protected dynamic SendRequest(dynamic input)
	{
		string input2 = Smart.Json.Dump(input);
		return SendContent(input2, "application/json");
	}

	protected dynamic SendForm(dynamic input)
	{
		string text = string.Empty;
		foreach (dynamic item in input)
		{
			if (text != string.Empty)
			{
				text += "&";
			}
			string arg = item.Key.ToString();
			string arg2 = item.Value.ToString();
			text += $"{arg}={arg2}";
		}
		return SendContent(text, "application/x-www-form-urlencoded");
	}

	protected dynamic SendGet(string urlArgs)
	{
		using WebClientTimeout webClientTimeout = new WebClientTimeout();
		webClientTimeout.Timeout = Timeout;
		Smart.Log.Verbose(TAG, $"Adding Authorization header with value '{BasicAuthentication}'");
		webClientTimeout.Headers[HttpRequestHeader.Authorization] = BasicAuthentication;
		SentRequest(urlArgs);
		string text = webClientTimeout.DownloadString(base.Url + urlArgs);
		ReceivedReply(text);
		return ParseResponse(text);
	}

	protected dynamic ParseResponse(string responseContent)
	{
		return Smart.Json.Load(responseContent);
	}
}
