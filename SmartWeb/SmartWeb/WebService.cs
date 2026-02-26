using System.Collections.Generic;

namespace SmartWeb;

public abstract class WebService
{
	private string TAG => GetType().FullName;

	public string Url { get; set; }

	protected virtual string BasicAuthentication => $"Bearer {OAuth}";

	public string OAuth { get; set; }

	public abstract dynamic Invoke(dynamic request);

	protected void SentRequest(string request)
	{
		foreach (string item in new List<string> { "<d3p1:id>", "<d3p1:pw>", "<id>", "<pw>" })
		{
			int num = request.IndexOf(item);
			if (num >= 0)
			{
				int startIndex = request.IndexOf('<', num + item.Length);
				request = request.Substring(0, num + item.Length) + "********" + request.Substring(startIndex);
			}
		}
		string arg = $"{GetType().Name} request";
		Smart.Log.Debug(TAG, "Sent request to " + Url);
		Smart.Log.Verbose(TAG, $"{arg} sent:\n{request}");
	}

	protected void ReceivedReply(string reply)
	{
		string text = "\"token\":\"";
		if (reply.Contains(text))
		{
			int num = reply.IndexOf(text);
			int startIndex = reply.IndexOf('"', num + text.Length + 1);
			reply = reply.Substring(0, num + text.Length) + "********" + reply.Substring(startIndex);
		}
		string arg = $"{GetType().Name} reply";
		Smart.Log.Debug(TAG, "Received reply from " + Url);
		Smart.Log.Verbose(TAG, $"{arg} received:\n{reply}");
	}
}
