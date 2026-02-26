using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace SmartWeb;

public class GetGppdIdService : RestService
{
	public struct GetGppdIdOutput
	{
		private string TAG => GetType().FullName;

		public string SerialNo { get; private set; }

		public string Customer { get; private set; }

		public string GppdId { get; private set; }

		public string Protocol { get; private set; }

		public string ResponseCode { get; private set; }

		public string ResponseMessage { get; private set; }

		public GetGppdIdOutput(string serialNo, string customer, string gppdId, string protocol, string responseCode, string responseMessage)
		{
			this = default(GetGppdIdOutput);
			SerialNo = serialNo;
			Customer = customer;
			GppdId = gppdId;
			Protocol = protocol;
			ResponseCode = responseCode;
			ResponseMessage = responseMessage;
		}

		public static GetGppdIdOutput FromDictionary(dynamic fields)
		{
			string serialNo = string.Empty;
			string customer = string.Empty;
			string gppdId = string.Empty;
			string protocol = string.Empty;
			if (fields.prodlist != null && fields.prodlist.Count > 0)
			{
				serialNo = (string)fields.prodlist[0].serial_no;
				customer = (string)fields.prodlist[0].customer;
				gppdId = (string)fields.prodlist[0].gppd_ID;
				protocol = (string)fields.prodlist[0].protocol;
			}
			return new GetGppdIdOutput(serialNo, customer, gppdId, protocol, (string)fields.response_code, (string)fields.response_message);
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["SerialNo"] = SerialNo;
			dictionary["Customer"] = Customer;
			dictionary["GppdId"] = GppdId;
			dictionary["Protocol"] = Protocol;
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

	public GetGppdIdService()
	{
		base.Url = "https://rsgw-v2.motorola.com/imeitac/updweb/getGppdIdCustForSerial";
	}

	public override dynamic Invoke(dynamic request)
	{
		string text = request.serialNumber;
		return SendGet("?inputStr=" + text.Trim());
	}

	public GetGppdIdOutput GetGppdId(string serialNumber)
	{
		Smart.Log.Debug(TAG, $"Contacting GetGppdIdService ({base.Url})");
		Smart.Log.Verbose(TAG, serialNumber);
		dynamic val = new ExpandoObject();
		val.serialNumber = serialNumber;
		dynamic val2 = Invoke(val);
		GetGppdIdOutput result = GetGppdIdOutput.FromDictionary(val2);
		Smart.Log.Debug(TAG, "GetGppdIdService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
