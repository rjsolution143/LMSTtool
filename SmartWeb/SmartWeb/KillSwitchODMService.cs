using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;

namespace SmartWeb;

public class KillSwitchODMService : RestService
{
	public struct KillSwitchODMInput
	{
		private string TAG => GetType().FullName;

		public string NewImei { get; private set; }

		public string MascId { get; private set; }

		public string ClientIp { get; private set; }

		public string ClientReqType { get; private set; }

		public string RsdLogId { get; private set; }

		public string ProdName { get; private set; }

		public string CpuId { get; private set; }

		public string BuildType { get; private set; }

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
				val.pname = ProdName;
				val.cpuid = CpuId;
				val.buildType = BuildType;
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

		public KillSwitchODMInput(string newImei, string mascId, string clientIp, string clientReqType, string rsdLogId, string prodName, string cpuId, string buildType)
		{
			this = default(KillSwitchODMInput);
			NewImei = newImei;
			MascId = mascId;
			ClientIp = clientIp;
			ClientReqType = clientReqType;
			RsdLogId = rsdLogId;
			ProdName = prodName;
			CpuId = cpuId;
			BuildType = buildType;
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

	public struct KillSwitchODMOutput
	{
		private string TAG => GetType().FullName;

		public string ResponseCode { get; private set; }

		public string ResponseMessage { get; private set; }

		public string ReturnedData { get; private set; }

		public string Password { get; private set; }

		public KillSwitchODMOutput(string responseCode, string responseMessage, string returnedData, string password)
		{
			this = default(KillSwitchODMOutput);
			ResponseCode = responseCode;
			ResponseMessage = responseMessage;
			ReturnedData = returnedData;
			Password = password;
		}

		public static KillSwitchODMOutput FromDictionary(dynamic fields)
		{
			string responseCode = fields.responseCode;
			string responseMessage = fields.responseMsg;
			string returnedData = fields.msg;
			string password = fields.password;
			return new KillSwitchODMOutput(responseCode, responseMessage, returnedData, password);
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["ResponseCode"] = ResponseCode;
			dictionary["ResponseMessage"] = ResponseMessage;
			dictionary["ReturnedData"] = ReturnedData;
			dictionary["Password"] = Password;
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

	public KillSwitchODMService()
	{
		base.Url = "https://ebiz-esb.motorola.com/callksunlock";
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendRequest(request);
	}

	public KillSwitchODMOutput Request(KillSwitchODMInput input)
	{
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		Smart.Log.Debug(TAG, $"Contacting KillSwitchODMService ({base.Url})");
		Smart.Log.Verbose(TAG, input.ToString());
		dynamic fields = input.Fields;
		dynamic val = Invoke(fields);
		KillSwitchODMOutput result = KillSwitchODMOutput.FromDictionary(val);
		Smart.Log.Debug(TAG, "KillSwitchODMService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
