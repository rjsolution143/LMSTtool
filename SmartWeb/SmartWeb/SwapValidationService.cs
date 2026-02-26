using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;

namespace SmartWeb;

public class SwapValidationService : RestService
{
	public struct SwapValidationServiceInput
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

		public string SwapDate { get; private set; }

		public string Message { get; private set; }

		public string Validation { get; private set; }

		public string ProcessDate { get; private set; }

		public string MessageCode { get; private set; }

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
				val.repairDate = RepairDate;
				val.ICCID = IccId;
				val.CIT = Cit;
				val.APC = Apc;
				val.transModel = TransModel;
				val.custModel = CustModel;
				val.MKTModel = MktModel;
				val.itemCode = ItemCode;
				val.intelControlKey = IntelControlKey;
				val.sourceCode = SourceCode;
				val.AKEY = AKey;
				val.lockCode1 = LockCode1;
				val.lockCode2 = LockCode2;
				val.servicePassCode = ServicePassCode;
				val.macAddress = MacAddress;
				val.HSN = Hsn;
				val.WLAN1 = Wlan1;
				val.WLAN2 = Wlan2;
				val.WLAN3 = Wlan3;
				val.WLAN4 = Wlan4;
				val.wimaxAddress = WimaxAddress;
				val.swapDate = SwapDate;
				val.message = Message;
				val.validation = Validation;
				val.processDate = ProcessDate;
				val.messageCode = MessageCode;
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

		public SwapValidationServiceInput(string mascId, string serialNoIn, string serialNoOut, string serialNoType, string dualSerialNoIn, string dualSerialNoOut, string dualSerialNoType, string repairDate, string iccId, string cit, string apc, string transModel, string custModel, string mktModel, string itemCode, string intelControlKey, string sourceCode, string akey, string lockCode1, string lockCode2, string servicePassCode, string macAddress, string hsn, string wlan1, string wlan2, string wlan3, string wlan4, string wimaxAddress, string swapDate, string message, string validation, string processDate, string messageCode, string swapType)
		{
			this = default(SwapValidationServiceInput);
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
			SwapDate = swapDate;
			Message = message;
			Validation = validation;
			ProcessDate = processDate;
			MessageCode = messageCode;
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

	public struct SwapValidationServiceOutput
	{
		private string TAG => GetType().FullName;

		public string ResponseCode { get; private set; }

		public string ResponseMessage { get; private set; }

		public SwapValidationServiceOutput(string responseCode, string responseMessage)
		{
			this = default(SwapValidationServiceOutput);
			ResponseCode = responseCode;
			ResponseMessage = responseMessage;
		}

		public static SwapValidationServiceOutput FromDictionary(dynamic fields)
		{
			return new SwapValidationServiceOutput((string)fields.response_code, (string)fields.response_message);
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

	public SwapValidationService()
	{
		base.Url = "https://api-cn.lenovo.com/validation/v1.0/validate";
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendRequest(request);
	}

	public SwapValidationServiceOutput Swap(SwapValidationServiceInput input)
	{
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		Smart.Log.Debug(TAG, $"Contacting SwapValidationService  ({base.Url})");
		Smart.Log.Verbose(TAG, input.ToString());
		dynamic fields = input.Fields;
		dynamic val = Invoke(fields);
		SwapValidationServiceOutput result = SwapValidationServiceOutput.FromDictionary(val);
		Smart.Log.Debug(TAG, "SwapValidationService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
