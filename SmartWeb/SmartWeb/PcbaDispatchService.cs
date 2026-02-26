using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace SmartWeb;

public class PcbaDispatchService : RestService
{
	public struct PcbaDispatchInput
	{
		private string TAG => GetType().FullName;

		public string SerialNumber { get; private set; }

		public string RequestType { get; private set; }

		public string Customer { get; private set; }

		public string SnRequestType { get; private set; }

		public string NumberOfUlma { get; private set; }

		public string GppdId { get; private set; }

		public string MascId { get; private set; }

		public string RsdId { get; private set; }

		public string BuildType { get; private set; }

		public string Protocol { get; private set; }

		public string TrackId { get; private set; }

		public string XcvrCode { get; private set; }

		public string Apc { get; private set; }

		public dynamic Fields
		{
			get
			{
				dynamic val = new ExpandoObject();
				val.serialNumber = SerialNumber;
				val.requestType = RequestType;
				val.customer = Customer;
				val.snRequestType = SnRequestType;
				val.numberOfUlma = NumberOfUlma;
				val.gppdID = GppdId;
				val.mascID = MascId;
				val.rsdID = RsdId;
				val.buildType = BuildType;
				val.protocol = Protocol;
				val.trackID = TrackId;
				val.xcvrCode = XcvrCode;
				val.apc = Apc;
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

		public PcbaDispatchInput(string serialNumber, string requestType, string customer, string snRequestType, string numberOfUlma, string gppdId, string mascId, string rsdId, string buildType, string protocol, string trackId, string xcvrCode, string apc, string miiModel)
		{
			this = default(PcbaDispatchInput);
			SerialNumber = serialNumber;
			RequestType = requestType;
			Customer = customer;
			SnRequestType = snRequestType;
			NumberOfUlma = numberOfUlma;
			GppdId = gppdId;
			MascId = mascId;
			RsdId = rsdId;
			BuildType = buildType;
			Protocol = protocol;
			TrackId = trackId;
			XcvrCode = xcvrCode;
			Apc = apc;
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

	public struct PcbaDispatchOutput
	{
		private string TAG => GetType().FullName;

		public string NewSerialNo { get; private set; }

		public string BuildType { get; private set; }

		public string Customer { get; private set; }

		public string DispatchedDate { get; private set; }

		public string MascId { get; private set; }

		public string GppdId { get; private set; }

		public string RequestType { get; private set; }

		public string ResponseCode { get; private set; }

		public string ResponseMsg { get; private set; }

		public string RsdId { get; private set; }

		public string UlmaAddress { get; private set; }

		public PcbaDispatchOutput(string newSerialNo, string buildType, string customer, string dispatchedDate, string mascId, string gppdId, string requestType, string responseCode, string responseMsg, string rsdId, string ulmaAddress, string miiCertNo, string scramblingCode)
		{
			this = default(PcbaDispatchOutput);
			NewSerialNo = newSerialNo;
			BuildType = buildType;
			Customer = customer;
			DispatchedDate = dispatchedDate;
			MascId = mascId;
			GppdId = gppdId;
			RequestType = requestType;
			ResponseCode = responseCode;
			ResponseMsg = responseMsg;
			RsdId = rsdId;
			UlmaAddress = ulmaAddress;
		}

		public static PcbaDispatchOutput FromDictionary(dynamic fields)
		{
			return new PcbaDispatchOutput(fields.newSerialNo.ToString(), fields.buildType.ToString(), fields.customer.ToString(), fields.dispatchedDate.ToString(), (string)fields.mascID.ToString(), fields.gppdID.ToString(), fields.requestType.ToString(), fields.responseCode.ToString(), fields.responseMsg.ToString(), fields.rsdID.ToString(), fields.ulmaAddress.ToString(), string.Empty, string.Empty);
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["NewSerialNo"] = NewSerialNo;
			dictionary["BuildType"] = BuildType;
			dictionary["Customer"] = Customer;
			dictionary["DispatchedDate"] = DispatchedDate;
			dictionary["MascId"] = MascId;
			dictionary["GppdId"] = GppdId;
			dictionary["RequestType"] = RequestType;
			dictionary["ResponseCode"] = ResponseCode;
			dictionary["ResponseMsg"] = ResponseMsg;
			dictionary["RsdId"] = RsdId;
			dictionary["UlmaAddress"] = UlmaAddress;
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

	public PcbaDispatchService()
	{
		base.Url = "https://wsgw04.motorola.com/Updpcba_dispatchserialNumber-Prod";
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendRequest(request);
	}

	public PcbaDispatchOutput PcbaDispatch(PcbaDispatchInput input)
	{
		Smart.Log.Debug(TAG, $"Contacting PcbaDispatchService  ({base.Url})");
		Smart.Log.Verbose(TAG, input.ToString());
		base.ExtraHeaders = Smart.Security.StationSign();
		dynamic fields = input.Fields;
		dynamic val = Invoke(fields);
		PcbaDispatchOutput result = PcbaDispatchOutput.FromDictionary(val);
		Smart.Log.Debug(TAG, "PcbaDispatchService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
