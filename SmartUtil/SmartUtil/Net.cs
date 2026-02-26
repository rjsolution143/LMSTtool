using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using ISmart;

namespace SmartUtil;

public class Net : INet
{
	protected DateTime NetworkLastSuccess = DateTime.Now.Subtract(TimeSpan.FromHours(2.0));

	protected bool NetworkState;

	protected TimeSpan NetworkTimeout = TimeSpan.FromSeconds(10.0);

	private string TAG => GetType().FullName;

	public string WebRequest(string url)
	{
		using WebClient webClient = new WebClient();
		return webClient.DownloadString(url);
	}

	public TimeSpan NistTimeOffset()
	{
		string[] obj = new string[9] { "https://www.motorola.com", "https://www.lenovo.com", "https://www.google-analytics.com", "https://www.bing.com", "https://svckm.lenovo.com", "https://osd.lenovo.com/", "https://soa.lenovo.com", "https://www.appspot.com", "https://www.thinkpad.com" };
		List<TimeSpan> list = new List<TimeSpan>();
		string[] array = obj;
		foreach (string url in array)
		{
			try
			{
				TimeSpan offset = GetOffset(url);
				list.Add(offset);
			}
			catch (Exception)
			{
			}
		}
		list.Sort();
		if (list.Count < 3)
		{
			throw new NotSupportedException("Not enough time servers available for time check");
		}
		return list[list.Count / 2];
	}

	private TimeSpan GetOffset(string url)
	{
		WebRequest webRequest = System.Net.WebRequest.Create(url);
		webRequest.Timeout = 10000;
		webRequest.Method = "HEAD";
		DateTime now = DateTime.Now;
		using WebResponse webResponse = webRequest.GetResponse();
		DateTime now2 = DateTime.Now;
		TimeSpan timeSpan = now2.Subtract(now);
		if (!webResponse.Headers.AllKeys.Contains("Date"))
		{
			throw new NotSupportedException("No date found in HTTP response from " + url);
		}
		DateTime dateTime = DateTime.Parse(webResponse.Headers["Date"], CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.None);
		timeSpan = new TimeSpan(timeSpan.Ticks / 2 + 200);
		dateTime = dateTime.Add(timeSpan);
		return now2.Subtract(dateTime);
	}

	public void Browser(string url)
	{
		Process.Start(url);
	}

	public long CheckSize(string url)
	{
		return CheckSize(url, new SortedList<string, string>());
	}

	public long CheckSize(string url, SortedList<string, string> headers)
	{
		long num = 0L;
		WebRequest webRequest = System.Net.WebRequest.Create(url);
		webRequest.Method = "HEAD";
		foreach (string key in headers.Keys)
		{
			webRequest.Headers.Add(key, headers[key]);
		}
		using WebResponse webResponse = webRequest.GetResponse();
		return webResponse.ContentLength;
	}

	public void WebHit(string screen)
	{
	}

	private void WebHitThread(string screen)
	{
	}

	public bool NetworkCheck()
	{
		if (NetworkState && DateTime.Now.Subtract(NetworkLastSuccess) < TimeSpan.FromMinutes(15.0))
		{
			return true;
		}
		Smart.Log.Verbose(TAG, "Checking network availability");
		System.Threading.Thread thread = new System.Threading.Thread(NetworkCheckThread);
		thread.IsBackground = true;
		thread.Start();
		if (!thread.Join(NetworkTimeout))
		{
			Smart.Log.Error(TAG, "Network check timed out");
			thread.Abort();
			NetworkState = false;
		}
		bool networkState = NetworkState;
		if (networkState)
		{
			Smart.Log.Debug(TAG, "Network check passed");
			NetworkLastSuccess = DateTime.Now;
			return networkState;
		}
		Smart.Log.Error(TAG, "Network check FAIL");
		return networkState;
	}

	private void NetworkCheckThread()
	{
		try
		{
			NetworkState = NetworkInterface.GetIsNetworkAvailable();
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Error while checking network: " + ex.Message);
			Smart.Log.Verbose(TAG, ex.ToString());
			NetworkState = false;
		}
	}

	private string PostEncode(SortedList<string, string> args)
	{
		List<string> list = new List<string>();
		foreach (string key in args.Keys)
		{
			string value = args[key];
			list.Add(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value));
		}
		return string.Join("&", list);
	}
}
