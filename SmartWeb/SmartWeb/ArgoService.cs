using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace SmartWeb;

public class ArgoService : RestService
{
	public struct ArgoOutput
	{
		private string TAG => GetType().FullName;

		public string Imei { get; private set; }

		public string ChannelId { get; private set; }

		public string EnterpriseEdition { get; private set; }

		public string Error { get; private set; }

		public string ResponseCode { get; private set; }

		public string ResponseMessage { get; private set; }

		public ArgoOutput(string imei, string channelId, string enterpriseEdition, string error, string responseCode, string responseMessage)
		{
			this = default(ArgoOutput);
			Imei = imei;
			ChannelId = channelId;
			EnterpriseEdition = enterpriseEdition;
			Error = error;
			ResponseCode = responseCode;
			ResponseMessage = responseMessage;
		}

		public static ArgoOutput FromDictionary(dynamic fields)
		{
			return new ArgoOutput((string)fields.imei, (string)fields.channelId, (string)fields.ee.ToString(), (string)fields.error, (string)fields.responseCode, (string)fields.responseMsg);
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["Imei"] = Imei;
			dictionary["ChannelId"] = ChannelId;
			dictionary["EnterpriseEdition"] = EnterpriseEdition;
			dictionary["Error"] = Error;
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

	public ArgoService()
	{
		base.Url = "https://ebiz-esb.motorola.com/rsdc/ARGO/getInfo/ ";
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendRequest(request);
	}

	public ArgoOutput GetArgoInfo(string serialNumber)
	{
		Smart.Log.Debug(TAG, $"Contacting ArgoService ({base.Url})");
		Smart.Log.Verbose(TAG, serialNumber);
		dynamic val = new ExpandoObject();
		val.imei = serialNumber;
		dynamic val2 = Invoke(val);
		ArgoOutput result = ArgoOutput.FromDictionary(val2);
		Smart.Log.Debug(TAG, "ArgoService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
