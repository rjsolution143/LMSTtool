using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;

namespace SmartWeb;

public class SyncService : RestService
{
	public struct SyncServiceInput
	{
		private string TAG => GetType().FullName;

		public string NewSerialNo { get; private set; }

		public string SerailNumberType { get; private set; }

		public string Protocol { get; private set; }

		public string TrackId { get; private set; }

		public string RsdId { get; private set; }

		public string MasterSubLockCode { get; private set; }

		public string Customer { get; private set; }

		public string DispatchedDate { get; private set; }

		public string XcvrCode { get; private set; }

		public string DispatchType { get; private set; }

		public string UlmaAddress { get; private set; }

		public string Akey1 { get; private set; }

		public string Akey2 { get; private set; }

		public string ServicePassCode { get; private set; }

		public string CarrierName { get; private set; }

		public string GppdId { get; private set; }

		public string PmdId { get; private set; }

		public string OneTimeLockCode { get; private set; }

		public string RsuSecretKey { get; private set; }

		public dynamic Fields
		{
			get
			{
				dynamic val = new ExpandoObject();
				val.newSerialNo = NewSerialNo;
				val.serailNumberType = SerailNumberType;
				val.protocol = Protocol;
				val.trackId = TrackId;
				val.rsdid = RsdId;
				val.masterSubLockCode = MasterSubLockCode;
				val.customer = Customer;
				val.dispatchedDate = DispatchedDate;
				val.xcvrCode = XcvrCode;
				val.dispatchType = DispatchType;
				val.ulmaAddress = UlmaAddress;
				val.akey1 = Akey1;
				val.akey2 = Akey2;
				val.servicePassCode = ServicePassCode;
				val.carrierName = CarrierName;
				val.gppdId = GppdId;
				val.pmdID = PmdId;
				val.oneTimeLockCode = OneTimeLockCode;
				val.rsuSecretKey = RsuSecretKey;
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

		public SyncServiceInput(string newSerialNo, string serialNumberType, string protocol, string trackId, string rsdId, string masterSubLockCode, string customer, string dispatchDate, string xcvrCode, string dispatchType, string ulmaAddress, string akey1, string akey2, string servicePassCode, string carrierName, string gppId, string pmdId, string oneTimeLockCode, string rsuSecretKey)
		{
			this = default(SyncServiceInput);
			NewSerialNo = newSerialNo;
			SerailNumberType = serialNumberType;
			Protocol = protocol;
			TrackId = trackId;
			RsdId = RsdId;
			MasterSubLockCode = masterSubLockCode;
			Customer = customer;
			DispatchedDate = dispatchDate;
			XcvrCode = xcvrCode;
			DispatchType = dispatchType;
			UlmaAddress = ulmaAddress;
			Akey1 = akey1;
			Akey2 = akey2;
			ServicePassCode = servicePassCode;
			CarrierName = carrierName;
			GppdId = gppId;
			PmdId = pmdId;
			OneTimeLockCode = oneTimeLockCode;
			RsuSecretKey = rsuSecretKey;
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

	public struct SyncServiceOutput
	{
		private string TAG => GetType().FullName;

		public string Status { get; private set; }

		public string StatusCode { get; private set; }

		public string StatusDesc { get; private set; }

		public string MsgId { get; private set; }

		public string ReqMsgId { get; private set; }

		public SyncServiceOutput(string status, string statusCode, string statusDesc, string msgId, string reqMsgId)
		{
			this = default(SyncServiceOutput);
			Status = status;
			StatusCode = statusCode;
			StatusDesc = statusDesc;
			MsgId = msgId;
			ReqMsgId = reqMsgId;
		}

		public static SyncServiceOutput FromDictionary(dynamic fields)
		{
			return new SyncServiceOutput((string)fields.status, (string)fields.status_code, (string)fields.status_desc, (string)fields.msg_id, (string)fields.req_msg_id);
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["Status"] = Status;
			dictionary["StatusCode"] = StatusCode;
			dictionary["StatusDesc"] = StatusDesc;
			dictionary["MsgId"] = MsgId;
			dictionary["ReqMsgId"] = ReqMsgId;
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

	public SyncService()
	{
		base.Url = "https://api-cn.lenovo.com/mbg_service_onebrand_dispatched_imei/1.0/update";
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendRequest(request);
	}

	public virtual SyncServiceOutput Sync(SyncServiceInput input)
	{
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		Smart.Log.Debug(TAG, $"Contacting SyncServiceService  ({base.Url})");
		Smart.Log.Verbose(TAG, input.ToString());
		dynamic fields = input.Fields;
		dynamic val = new SortedList<string, object>();
		val["imei"] = new SortedList<string, object>();
		val["imei"]["record"] = new List<object>();
		val["imei"]["record"].Add(fields);
		dynamic val2 = Invoke(val);
		dynamic val3 = val2["response"]["header"];
		SyncServiceOutput result = SyncServiceOutput.FromDictionary(val3);
		Smart.Log.Debug(TAG, "SyncServiceService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
