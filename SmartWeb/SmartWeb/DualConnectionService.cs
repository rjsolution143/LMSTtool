using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;

namespace SmartWeb;

public class DualConnectionService : RestService
{
	public struct DualConnectionInput
	{
		private string TAG => GetType().FullName;

		public string SerialNo { get; private set; }

		public string DualSerialNo { get; private set; }

		public string Gsn { get; private set; }

		public string TrackId { get; private set; }

		public string MascId { get; private set; }

		public string RsdId { get; private set; }

		public DateTime DispatchDate { get; private set; }

		public dynamic Fields
		{
			get
			{
				dynamic val = new ExpandoObject();
				val.imei1 = SerialNo;
				val.imei2 = DualSerialNo;
				val.gsn = Gsn;
				val.trackid = TrackId;
				val.mascid = MascId;
				val.rsd_log_id = RsdId;
				string text = DispatchDate.ToString("yyyyMMdd");
				val.dispatch_date = text;
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

		public DualConnectionInput(string serialNo, string dualSerialNo, string gsn, string trackId, string mascId, string rsdId, DateTime dispatchDate)
		{
			this = default(DualConnectionInput);
			SerialNo = serialNo;
			DualSerialNo = dualSerialNo;
			Gsn = gsn;
			TrackId = trackId;
			MascId = mascId;
			RsdId = rsdId;
			DispatchDate = dispatchDate;
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

	public struct DualConnectionOutput
	{
		private string TAG => GetType().FullName;

		public string ResponseCode { get; private set; }

		public string ResponseMessage { get; private set; }

		public DualConnectionOutput(string responseCode, string responseMessage)
		{
			this = default(DualConnectionOutput);
			ResponseCode = responseCode;
			ResponseMessage = responseMessage;
		}

		public static DualConnectionOutput FromDictionary(dynamic fields)
		{
			return new DualConnectionOutput((string)fields.ev_code, (string)fields.ev_message);
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

	public DualConnectionService()
	{
		base.Url = "https://api-cn.lenovo.com/v1.0/service/mbg_service_imei_number/imei_info";
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendRequest(request);
	}

	public DualConnectionOutput Connect(DualConnectionInput input)
	{
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		Smart.Log.Debug(TAG, $"Contacting DualConnectionService ({base.Url})");
		Smart.Log.Verbose(TAG, input.ToString());
		dynamic fields = input.Fields;
		dynamic val = Invoke(fields);
		DualConnectionOutput result = DualConnectionOutput.FromDictionary(val);
		Smart.Log.Debug(TAG, "DualConnection request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
