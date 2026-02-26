using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;

namespace SmartWeb;

public class RsuService : RestService
{
	public struct RsuInput
	{
		private string TAG => GetType().FullName;

		public string SerialNo { get; private set; }

		public string RsdUser { get; private set; }

		public string MascId { get; private set; }

		public string SoCModel { get; private set; }

		public string Suid { get; private set; }

		public string ReceiptData { get; private set; }

		public string Sip { get; private set; }

		public string DeviceModel { get; private set; }

		public string MnOperator { get; private set; }

		public string TrackId { get; private set; }

		public dynamic Fields
		{
			get
			{
				dynamic val = new ExpandoObject();
				val.serialNo = SerialNo;
				val.rsdUser = RsdUser;
				val.mascId = MascId;
				val.soCModel = SoCModel;
				val.suid = Suid;
				val.receiptData = ReceiptData;
				val.sip = Sip;
				val.deviceModel = DeviceModel;
				val.mn_operator = MnOperator;
				val.track_id = TrackId;
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

		public RsuInput(string serialNo, string rsdUser, string mascId, string soCModel, string suid, string receiptData, string sip, string deviceModel, string mnOperator, string trackId)
		{
			this = default(RsuInput);
			SerialNo = serialNo;
			RsdUser = rsdUser;
			MascId = mascId;
			SoCModel = soCModel;
			Suid = suid;
			ReceiptData = receiptData;
			Sip = sip;
			DeviceModel = deviceModel;
			MnOperator = mnOperator;
			TrackId = trackId;
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

	public struct RsuOutput
	{
		private string TAG => GetType().FullName;

		public string ResponseCode { get; private set; }

		public string ResponseMsg { get; private set; }

		public RsuOutput(string responseCode, string responseMsg)
		{
			this = default(RsuOutput);
			ResponseCode = responseCode;
			ResponseMsg = responseMsg;
		}

		public static RsuOutput FromDictionary(dynamic fields)
		{
			return new RsuOutput(fields.responseCode.ToString(), fields.responseMsg.ToString());
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
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

	public RsuService()
	{
		base.Url = "https://rsgw-v2.motorola.com/MBG_IQS_QUERY/webservice/rest/rsuService/callrsuService";
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendRequest(request);
	}

	public RsuOutput RsuRequest(RsuInput input)
	{
		Smart.Log.Debug(TAG, $"Contacting RsuService ({base.Url})");
		Smart.Log.Verbose(TAG, input.ToString());
		dynamic fields = input.Fields;
		dynamic val = Invoke(fields);
		RsuOutput result = RsuOutput.FromDictionary(val);
		Smart.Log.Debug(TAG, "RsuService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
