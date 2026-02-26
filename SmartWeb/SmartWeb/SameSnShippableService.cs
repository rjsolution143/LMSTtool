using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;

namespace SmartWeb;

public class SameSnShippableService : RestService
{
	public struct SameSnShippableInput
	{
		private string TAG => GetType().FullName;

		public string SerialNo { get; private set; }

		public string DualSerialNo { get; private set; }

		public string TrackId { get; private set; }

		public string RsdId { get; private set; }

		public dynamic Fields
		{
			get
			{
				dynamic val = new ExpandoObject();
				val.serial_no = SerialNo;
				val.dual_serial_no = DualSerialNo;
				val.track_id = TrackId;
				val.rsd_id = RsdId;
				IDictionary<string, object> dictionary = val;
				foreach (string item in new List<string>(dictionary.Keys))
				{
					if (dictionary[item] == null)
					{
						dictionary.Remove(item);
					}
				}
				return val;
			}
		}

		public SameSnShippableInput(string serialNo, string dualSerialNo, string trackId, string rsdId)
		{
			this = default(SameSnShippableInput);
			SerialNo = serialNo;
			DualSerialNo = dualSerialNo;
			TrackId = trackId;
			RsdId = rsdId;
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<string, object> item in (IDictionary<string, object>)Fields)
			{
				dictionary[item.Key] = item.Value.ToString();
			}
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

	public struct SameSnShippableOutput
	{
		private string TAG => GetType().FullName;

		public string ResponseCode { get; private set; }

		public string ResponseMessage { get; private set; }

		public SameSnShippableOutput(string responseCode, string responseMessage)
		{
			this = default(SameSnShippableOutput);
			ResponseCode = responseCode;
			ResponseMessage = responseMessage;
		}

		public static SameSnShippableOutput FromDictionary(dynamic fields)
		{
			return new SameSnShippableOutput((string)fields.response_code, (string)fields.response_message);
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
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

	public SameSnShippableService()
	{
		base.Url = "https://soa.lenovo.com/api/samesnshippable/1.0/update";
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendRequest(request);
	}

	public SameSnShippableOutput Request(SameSnShippableInput input)
	{
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		Smart.Log.Debug(TAG, $"Contacting SameSnShippableService ({base.Url})");
		Smart.Log.Verbose(TAG, input.ToString());
		dynamic fields = input.Fields;
		dynamic val = Invoke(fields);
		SameSnShippableOutput result = SameSnShippableOutput.FromDictionary(val);
		Smart.Log.Debug(TAG, "SameSnShippableService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
