using System;
using System.Collections.Generic;
using System.Net.Http;
using ISmart;

namespace SmartDevice.Steps;

public class PrefetchMotoFocucs : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		string serialNumber = ((IDevice)((dynamic)base.Recipe.Info.Args).Device).SerialNumber;
		string text = "https://moto-focus.appspot.com/service/prefetch";
		HttpClientHandler val = new HttpClientHandler();
		try
		{
			HttpClient val2 = new HttpClient((HttpMessageHandler)(object)val);
			try
			{
				FormUrlEncodedContent val3 = new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)new Dictionary<string, string> { { "imei", serialNumber } });
				Smart.Log.Debug(TAG, "Sending web request to " + text);
				string result = val2.PostAsync(text, (HttpContent)(object)val3).Result.Content.ReadAsStringAsync().Result;
				Smart.Log.Debug(TAG, "Prefetch response: " + result);
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		LogPass();
	}
}
