using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;

namespace SmartWeb;

public class SwapService : RestService
{
	public struct SwapServiceInput
	{
		private string TAG => GetType().FullName;

		public string MascId { get; private set; }

		public string SerialNoIn { get; private set; }

		public string SerialNoOut { get; private set; }

		public string SerialNoType { get; private set; }

		public string DualSerialNoIn { get; private set; }

		public string DualSerialNoOut { get; private set; }

		public string DualSerialNoType { get; private set; }

		public string RepairDate { get; private set; }

		public string IccId { get; private set; }

		public string Cit { get; private set; }

		public string Apc { get; private set; }

		public string TransModel { get; private set; }

		public string CustModel { get; private set; }

		public string MktModel { get; private set; }

		public string ItemCode { get; private set; }

		public string IntelControlKey { get; private set; }

		public string SourceCode { get; private set; }

		public string AKey { get; private set; }

		public string LockCode1 { get; private set; }

		public string LockCode2 { get; private set; }

		public string ServicePassCode { get; private set; }

		public string MacAddress { get; private set; }

		public string Hsn { get; private set; }

		public string Wlan1 { get; private set; }

		public string Wlan2 { get; private set; }

		public string Wlan3 { get; private set; }

		public string Wlan4 { get; private set; }

		public string WimaxAddress { get; private set; }

		public string SwapType { get; private set; }

		public dynamic Fields
		{
			get
			{
				dynamic val = new ExpandoObject();
				val.mascID = MascId;
				val.serialNoIn = SerialNoIn;
				val.serialNoOut = SerialNoOut;
				val.serialNoType = SerialNoType;
				val.dualSerialNoIn = DualSerialNoIn;
				val.dualSerialNoOut = DualSerialNoOut;
				val.dualSerialNoType = DualSerialNoType;
				val.repairdate = RepairDate;
				val.iccId = IccId;
				val.cit = Cit;
				val.apc = Apc;
				val.transModel = TransModel;
				val.custModel = CustModel;
				val.mktModel = MktModel;
				val.itemCode = ItemCode;
				val.intelControlKey = IntelControlKey;
				val.sourceCode = SourceCode;
				val.akey = AKey;
				val.lockcode1 = LockCode1;
				val.lockcode2 = LockCode2;
				val.servicepasscode = ServicePassCode;
				val.mac_address = MacAddress;
				val.hsn = Hsn;
				val.wlan1 = Wlan1;
				val.wlan2 = Wlan2;
				val.wlan3 = Wlan3;
				val.wlan4 = Wlan4;
				val.wiMaxAddress = WimaxAddress;
				val.swapType = SwapType;
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

		public SwapServiceInput(string mascId, string serialNoIn, string serialNoOut, string serialNoType, string dualSerialNoIn, string dualSerialNoOut, string dualSerialNoType, string repairDate, string iccId, string cit, string apc, string transModel, string custModel, string mktModel, string itemCode, string intelControlKey, string sourceCode, string akey, string lockCode1, string lockCode2, string servicePassCode, string macAddress, string hsn, string wlan1, string wlan2, string wlan3, string wlan4, string wimaxAddress, string swapType)
		{
			this = default(SwapServiceInput);
			MascId = mascId;
			SerialNoIn = serialNoIn;
			SerialNoOut = serialNoOut;
			SerialNoType = serialNoType;
			DualSerialNoIn = dualSerialNoIn;
			DualSerialNoOut = dualSerialNoOut;
			DualSerialNoType = dualSerialNoType;
			RepairDate = repairDate;
			IccId = iccId;
			Cit = cit;
			Apc = apc;
			TransModel = transModel;
			CustModel = custModel;
			MktModel = mktModel;
			ItemCode = itemCode;
			IntelControlKey = intelControlKey;
			SourceCode = sourceCode;
			AKey = akey;
			LockCode1 = lockCode1;
			LockCode2 = lockCode2;
			ServicePassCode = servicePassCode;
			MacAddress = macAddress;
			Hsn = hsn;
			Wlan1 = wlan1;
			Wlan2 = wlan2;
			Wlan3 = wlan3;
			Wlan4 = wlan4;
			WimaxAddress = wimaxAddress;
			SwapType = swapType;
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

	public struct SwapServiceOutput
	{
		private string TAG => GetType().FullName;

		public string Status { get; private set; }

		public string StatusCode { get; private set; }

		public string StatusDesc { get; private set; }

		public string MsgId { get; private set; }

		public string ReqMsgId { get; private set; }

		public SwapServiceOutput(string status, string statusCode, string statusDesc, string msgId, string reqMsgId)
		{
			this = default(SwapServiceOutput);
			Status = status;
			StatusCode = statusCode;
			StatusDesc = statusDesc;
			MsgId = msgId;
			ReqMsgId = reqMsgId;
		}

		public static SwapServiceOutput FromDictionary(dynamic fields)
		{
			return new SwapServiceOutput((string)fields.status, (string)fields.status_code, (string)fields.status_desc, (string)fields.msg_id, (string)fields.req_msg_id);
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

	public SwapService()
	{
		base.Url = "https://api-cn.lenovo.com/mbg_service_onebrand_swap/1.0/update";
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendRequest(request);
	}

	public virtual SwapServiceOutput Swap(SwapServiceInput input)
	{
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		Smart.Log.Debug(TAG, $"Contacting SwapServiceService  ({base.Url})");
		Smart.Log.Verbose(TAG, input.ToString());
		dynamic fields = input.Fields;
		dynamic val = new SortedList<string, object>();
		val["swap"] = new SortedList<string, object>();
		val["swap"]["record"] = new List<object>();
		val["swap"]["record"].Add(fields);
		dynamic val2 = Invoke(val);
		dynamic val3 = val2["response"]["header"];
		SwapServiceOutput result = SwapServiceOutput.FromDictionary(val3);
		Smart.Log.Info(TAG, "SwapServiceService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
