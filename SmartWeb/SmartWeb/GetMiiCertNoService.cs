using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace SmartWeb;

public class GetMiiCertNoService : RestService
{
	public struct GetMiiCertNoOutput
	{
		private string TAG => GetType().FullName;

		public string SerialNo { get; private set; }

		public string MiiCertNo { get; private set; }

		public string ScramblingCode { get; private set; }

		public string MiiModel { get; private set; }

		public string ResponseCode { get; private set; }

		public string ResponseMessage { get; private set; }

		public GetMiiCertNoOutput(string serialNo, string miiCertNo, string scramblingCode, string miiModel, string responseCode, string responseMessage)
		{
			this = default(GetMiiCertNoOutput);
			SerialNo = serialNo;
			MiiCertNo = miiCertNo;
			ScramblingCode = scramblingCode;
			MiiModel = miiModel;
			ResponseCode = responseCode;
			ResponseMessage = responseMessage;
		}

		public static GetMiiCertNoOutput FromDictionary(dynamic fields)
		{
			return new GetMiiCertNoOutput((string)fields.serialNo, (string)fields.miiCertNo, (string)fields.scramblingCode, (string)fields.miiModel, (string)fields.responseCode, (string)fields.responseMsg);
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["SerialNo"] = SerialNo;
			dictionary["MiiCertNo"] = MiiCertNo;
			dictionary["ScramblingCode"] = ScramblingCode;
			dictionary["MiiModel"] = MiiModel;
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

	public GetMiiCertNoService()
	{
		base.Url = "https://rsgw-v2.motorola.com/Updpcba_GetMiiCertNo-Prod";
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendRequest(request);
	}

	public GetMiiCertNoOutput GetMiiCertNo(string serialNumber)
	{
		Smart.Log.Debug(TAG, $"Contacting GetMiiCertNoService ({base.Url})");
		Smart.Log.Verbose(TAG, serialNumber);
		dynamic val = new ExpandoObject();
		val.serialNo = serialNumber;
		dynamic val2 = Invoke(val);
		GetMiiCertNoOutput result = GetMiiCertNoOutput.FromDictionary(val2);
		Smart.Log.Debug(TAG, "GetMiiCertNoService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
