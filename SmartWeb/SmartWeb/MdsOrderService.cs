using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace SmartWeb;

public class MdsOrderService : RestService
{
	public struct MdsOrderInput
	{
		private string TAG => GetType().FullName;

		public string Imei { get; private set; }

		public string TrackId { get; private set; }

		public string UserName { get; private set; }

		public string UserImei { get; private set; }

		public dynamic Fields
		{
			get
			{
				dynamic val = new ExpandoObject();
				val.imei = Imei;
				val.trackid = TrackId;
				val.username = UserName;
				val.user_imei = UserImei;
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

		public MdsOrderInput(string imei, string trackId, string userName, string userImei)
		{
			this = default(MdsOrderInput);
			Imei = imei;
			TrackId = trackId;
			UserName = userName;
			UserImei = userImei;
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

	public struct MdsOrderOutput
	{
		private string TAG => GetType().FullName;

		public string ResponseCode { get; private set; }

		public string ResponseMessage { get; private set; }

		public bool Required { get; private set; }

		public MdsOrderOutput(string responseCode, string responseMessage, bool required)
		{
			this = default(MdsOrderOutput);
			ResponseCode = responseCode;
			ResponseMessage = responseMessage;
			Required = required;
		}

		public static MdsOrderOutput FromDictionary(dynamic fields)
		{
			string responseCode = fields.responseCode;
			string responseMessage = fields.responseMsg;
			bool required = false;
			if (fields.required != null)
			{
				required = fields.required;
			}
			return new MdsOrderOutput(responseCode, responseMessage, required);
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["ResponseCode"] = ResponseCode;
			dictionary["ResponseMessage"] = ResponseMessage;
			dictionary["Required"] = Required;
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

	public MdsOrderService()
	{
		base.Url = "https://ebiz-esb.motorola.com/rsdc/MDS/getOrder/";
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendRequest(request);
	}

	public MdsOrderOutput GetMdsOrder(MdsOrderInput input)
	{
		Smart.Log.Debug(TAG, $"Contacting MdsOrderService ({base.Url})");
		Smart.Log.Verbose(TAG, input.ToString());
		dynamic val = Invoke(input.Fields);
		MdsOrderOutput result = MdsOrderOutput.FromDictionary(val);
		Smart.Log.Debug(TAG, "MdsOrderService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
