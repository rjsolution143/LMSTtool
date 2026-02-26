using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;

namespace SmartWeb;

public class RpkService : RestService
{
	public struct RpkInput
	{
		private string TAG => GetType().FullName;

		public string SerialNo { get; private set; }

		public string RsdUser { get; private set; }

		public string MascId { get; private set; }

		public string GoogleCsr { get; private set; }

		public string GoogleCsr2 { get; private set; }

		public string GoogleCsr3 { get; private set; }

		public string TrackId { get; private set; }

		public dynamic Fields
		{
			get
			{
				dynamic val = new ExpandoObject();
				val.serialNo = SerialNo;
				val.rsdUser = RsdUser;
				val.mascId = MascId;
				val.googleCsr = GoogleCsr;
				val.googleCsr2 = GoogleCsr2;
				val.googleCsr3 = GoogleCsr3;
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

		public RpkInput(string serialNo, string rsdUser, string mascId, string googleCsr, string googleCsr2, string googleCsr3, string trackId)
		{
			this = default(RpkInput);
			SerialNo = serialNo;
			RsdUser = rsdUser;
			MascId = mascId;
			GoogleCsr = googleCsr;
			GoogleCsr2 = googleCsr2;
			GoogleCsr3 = googleCsr3;
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

	public struct RpkOutput
	{
		private string TAG => GetType().FullName;

		public string ResponseCode { get; private set; }

		public string ResponseMsg { get; private set; }

		public RpkOutput(string responseCode, string responseMsg)
		{
			this = default(RpkOutput);
			ResponseCode = responseCode;
			ResponseMsg = responseMsg;
		}

		public static RpkOutput FromDictionary(dynamic fields)
		{
			return new RpkOutput(fields.responseCode.ToString(), fields.responseMsg.ToString());
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

	public RpkService()
	{
		base.Url = "https://ebiz-esb.motorola.com/callGoogleRPKService";
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendRequest(request);
	}

	public RpkOutput RsuRequest(RpkInput input)
	{
		Smart.Log.Debug(TAG, $"Contacting RpkService  ({base.Url})");
		Smart.Log.Verbose(TAG, input.ToString());
		dynamic fields = input.Fields;
		dynamic val = Invoke(fields);
		RpkOutput result = RpkOutput.FromDictionary(val);
		Smart.Log.Debug(TAG, "RpkService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
