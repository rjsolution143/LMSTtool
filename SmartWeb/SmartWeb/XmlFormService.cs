using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace SmartWeb;

public abstract class XmlFormService : WebService
{
	private string TAG => GetType().FullName;

	public TimeSpan Timeout { get; protected set; } = TimeSpan.FromSeconds(100.0);


	protected string FormTemplate { get; set; }

	protected dynamic SendContent(string input, string contentType)
	{
		using WebClientTimeout webClientTimeout = new WebClientTimeout();
		webClientTimeout.Timeout = Timeout;
		SentRequest(input);
		webClientTimeout.Headers[HttpRequestHeader.ContentType] = contentType;
		string text = webClientTimeout.UploadString(base.Url, "POST", input);
		ReceivedReply(text);
		return ParseResponse(text);
	}

	protected dynamic SendForm(dynamic input)
	{
		XDocument val = XDocument.Parse(FormTemplate);
		foreach (KeyValuePair<string, object> item in (IDictionary<string, object>)input)
		{
			string key = item.Key;
			string value = item.Value.ToString();
			(((XContainer)val).Descendants(XName.op_Implicit(key)).FirstOrDefault() ?? throw new NotSupportedException($"Unrecognized XML field name: {key}")).Value = value;
		}
		string input2 = "xml=" + ((object)val).ToString();
		return SendContent(input2, "application/x-www-form-urlencoded");
	}

	protected SortedList<string, string> ParseResponse(string responseContent)
	{
		XDocument obj = XDocument.Parse(responseContent);
		SortedList<string, string> sortedList = new SortedList<string, string>();
		foreach (XElement item in ((XContainer)obj).Descendants())
		{
			if (!item.HasElements)
			{
				sortedList[item.Name.LocalName] = item.Value;
			}
		}
		return sortedList;
	}
}
