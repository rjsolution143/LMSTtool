using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace SmartWeb;

public class ZeroTouchService : RestService
{
	public struct ZeroTouchOutput
	{
		private string TAG => GetType().FullName;

		public string Imei { get; private set; }

		public bool Flag { get; private set; }

		public string ResponseCode { get; private set; }

		public string ResponseMessage { get; private set; }

		public ZeroTouchOutput(string imei, bool flag, string responseCode, string responseMessage)
		{
			this = default(ZeroTouchOutput);
			Imei = imei;
			Flag = flag;
			ResponseCode = responseCode;
			ResponseMessage = responseMessage;
		}

		public static ZeroTouchOutput FromDictionary(dynamic fields)
		{
			string imei = fields.imei;
			bool flag = fields.flag;
			string responseCode = fields.responseCode;
			string responseMessage = fields.responseMsg;
			return new ZeroTouchOutput(imei, flag, responseCode, responseMessage);
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["Imei"] = Imei;
			dictionary["Flag"] = Flag;
			dictionary["ResponseCode"] = ResponseCode;
			dictionary["ResponseMessage"] = ResponseMessage;
			return Smart.Convert.ToString(GetType().Name, (IEnumerable<KeyValuePair<string, object>>)dictionary.ToList());
		}

		public override bool Equals(object obj)
		{
			return ToString().Equals(obj.ToString());
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
	}

	private string TAG => GetType().FullName;

	public ZeroTouchService()
	{
		base.Url = "https://ebiz-esb.motorola.com/rsdc/ZeroTouch/getFlag/";
		base.Timeout = TimeSpan.FromSeconds(15.0);
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendRequest(request);
	}

	public ZeroTouchOutput CheckZeroTouch(string imei)
	{
		Smart.Log.Debug(TAG, $"Contacting ZeroTouchService ({base.Url})");
		Smart.Log.Verbose(TAG, imei);
		dynamic val = new ExpandoObject();
		val.imei = imei;
		dynamic val2 = Invoke(val);
		ZeroTouchOutput result = ZeroTouchOutput.FromDictionary(val2);
		Smart.Log.Debug(TAG, "ZeroTouchService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
