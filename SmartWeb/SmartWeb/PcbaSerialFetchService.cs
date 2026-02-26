using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace SmartWeb;

public class PcbaSerialFetchService : RestService
{
	public struct PcbaSerialFetchOutput
	{
		private string TAG => GetType().FullName;

		public string SerialIn { get; private set; }

		public string SerialOut { get; private set; }

		public string ResponseCode { get; private set; }

		public string ResponseMsg { get; private set; }

		public PcbaSerialFetchOutput(string serialIn, string serialOut, string responseCode, string responseMsg)
		{
			this = default(PcbaSerialFetchOutput);
			SerialIn = serialIn;
			SerialOut = serialOut;
			ResponseCode = responseCode;
			ResponseMsg = responseMsg;
		}

		public static PcbaSerialFetchOutput FromDictionary(dynamic fields)
		{
			return new PcbaSerialFetchOutput((string)fields.serialIn, (string)fields.serialOut, (string)fields.responseCode, (string)fields.responseMsg);
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["SerialIn"] = SerialIn;
			dictionary["SerialOut"] = SerialOut;
			dictionary["ResponseCode"] = ResponseCode;
			dictionary["ResponseMsg"] = ResponseMsg;
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

	public PcbaSerialFetchService()
	{
		base.Url = "https://wsgw04.motorola.com/Updpcba_serialFetchRS-Prod";
	}

	public override dynamic Invoke(dynamic request)
	{
		string text = request.serialNumber;
		return SendGet("/" + text.Trim());
	}

	public PcbaSerialFetchOutput PcbaFetch(string serialNumber)
	{
		Smart.Log.Debug(TAG, $"Contacting PcbaSerialFetchService ({base.Url})");
		Smart.Log.Verbose(TAG, serialNumber);
		dynamic val = new ExpandoObject();
		val.serialNumber = serialNumber;
		dynamic val2 = Invoke(val);
		PcbaSerialFetchOutput result = PcbaSerialFetchOutput.FromDictionary(val2);
		Smart.Log.Debug(TAG, "PcbaSerialFetchService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
