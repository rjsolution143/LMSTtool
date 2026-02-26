using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;

namespace SmartWeb;

public class SameSnTransferService : RestService
{
	public struct SameSnTransferInput
	{
		private string TAG => GetType().FullName;

		public string SerialIn { get; private set; }

		public string SerialOut { get; private set; }

		public string SerialInDual { get; private set; }

		public string SerialOutDual { get; private set; }

		public string TrackIdOut { get; private set; }

		public string ProgramDate { get; private set; }

		public string RsdId { get; private set; }

		public string MascId { get; private set; }

		public dynamic Fields
		{
			get
			{
				dynamic val = new ExpandoObject();
				val.serial_in = SerialIn;
				val.serial_out = SerialOut;
				val.serial_in_dual = SerialInDual;
				val.serial_Out_dual = SerialOutDual;
				val.trackid_out = TrackIdOut;
				val.program_date = ProgramDate;
				val.rsdID = RsdId;
				val.mascID = MascId;
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

		public SameSnTransferInput(string serialIn, string serialOut, string serialInDual, string serialOutDual, string trackIdOut, string programDate, string rsdId, string mascId)
		{
			this = default(SameSnTransferInput);
			SerialIn = serialIn;
			SerialOut = serialOut;
			SerialInDual = serialInDual;
			SerialOutDual = serialOutDual;
			TrackIdOut = trackIdOut;
			ProgramDate = programDate;
			RsdId = rsdId;
			MascId = mascId;
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

	public struct SameSnTransferOutput
	{
		private string TAG => GetType().FullName;

		public string ResponseCode { get; private set; }

		public string ResponseMessage { get; private set; }

		public SameSnTransferOutput(string responseCode, string responseMessage)
		{
			this = default(SameSnTransferOutput);
			ResponseCode = responseCode;
			ResponseMessage = responseMessage;
		}

		public static SameSnTransferOutput FromDictionary(dynamic fields)
		{
			return new SameSnTransferOutput((string)fields.response_code, (string)fields.response_message);
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

	public SameSnTransferService()
	{
		base.Url = "https://rsgw-v2.motorola.com/api/samesntransfer/1.0/update";
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendRequest(request);
	}

	public SameSnTransferOutput Request(SameSnTransferInput input)
	{
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		Smart.Log.Debug(TAG, $"Contacting SameSnTransferService ({base.Url})");
		Smart.Log.Verbose(TAG, input.ToString());
		dynamic fields = input.Fields;
		dynamic val = Invoke(fields);
		SameSnTransferOutput result = SameSnTransferOutput.FromDictionary(val);
		Smart.Log.Debug(TAG, "SameSnTransferService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
