using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;

namespace SmartWeb;

public class KeyDispatchODMService : RestService
{
	public struct KeyDispatchODMInput
	{
		private string TAG => GetType().FullName;

		public string NewImei { get; private set; }

		public string MascId { get; private set; }

		public string ClientIp { get; private set; }

		public string ClientReqType { get; private set; }

		public string RsdLogId { get; private set; }

		public string CertModel { get; private set; }

		public string CertType { get; private set; }

		public dynamic Fields
		{
			get
			{
				dynamic val = new ExpandoObject();
				val.newIMEI = NewImei;
				val.mascid = MascId;
				val.clientIP = ClientIp;
				val.clientReqType = ClientReqType;
				val.rsd_log_id = RsdLogId;
				val.certModel = CertModel;
				val.certType = CertType;
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

		public KeyDispatchODMInput(string newImei, string mascId, string clientIp, string clientReqType, string rsdLogId, string certModel, string certType)
		{
			this = default(KeyDispatchODMInput);
			NewImei = newImei;
			MascId = mascId;
			ClientIp = clientIp;
			ClientReqType = clientReqType;
			RsdLogId = rsdLogId;
			CertModel = certModel;
			CertType = certType;
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

	public struct KeyDispatchODMOutput
	{
		private string TAG => GetType().FullName;

		public string ResponseCode { get; private set; }

		public string ResponseMessage { get; private set; }

		public string ReturnedData { get; private set; }

		public KeyDispatchODMOutput(string responseCode, string responseMessage, string returnedData)
		{
			this = default(KeyDispatchODMOutput);
			ResponseCode = responseCode;
			ResponseMessage = responseMessage;
			ReturnedData = returnedData;
		}

		public static KeyDispatchODMOutput FromDictionary(dynamic fields)
		{
			string responseCode = fields.responseCode;
			string responseMessage = fields.responseMsg;
			string returnedData = fields.certBlob;
			return new KeyDispatchODMOutput(responseCode, responseMessage, returnedData);
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["ResponseCode"] = ResponseCode;
			dictionary["ResponseMessage"] = ResponseMessage;
			dictionary["ReturnedData"] = ReturnedData;
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

	public KeyDispatchODMService()
	{
		base.Url = "https://ebiz-esb.motorola.com/calldispatchkeys";
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendRequest(request);
	}

	public KeyDispatchODMOutput Dispatch(KeyDispatchODMInput input)
	{
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		Smart.Log.Debug(TAG, $"Contacting KeyDispatchODMService ({base.Url})");
		Smart.Log.Verbose(TAG, input.ToString());
		dynamic fields = input.Fields;
		dynamic val = Invoke(fields);
		KeyDispatchODMOutput result = KeyDispatchODMOutput.FromDictionary(val);
		Smart.Log.Debug(TAG, "KeyDispatchODMService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
