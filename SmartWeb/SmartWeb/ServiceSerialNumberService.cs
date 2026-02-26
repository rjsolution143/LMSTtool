using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;

namespace SmartWeb;

public class ServiceSerialNumberService : RestService
{
	public struct ServiceSerialNumberInput
	{
		private string TAG => GetType().FullName;

		public string MascID { get; private set; }

		public string SerialNumberIn { get; private set; }

		public string SerialNumberOut { get; private set; }

		public string SerialNumberType { get; private set; }

		public string SerialNumberInDual { get; private set; }

		public string SerialNumberOutDual { get; private set; }

		public string SerialNumberTypeDual { get; private set; }

		public string RepairDate { get; private set; }

		public string Iccid { get; private set; }

		public string Cit { get; private set; }

		public string Apc { get; private set; }

		public string TransModel { get; private set; }

		public string CustModel { get; private set; }

		public string MktModel { get; private set; }

		public string ItemCode { get; private set; }

		public dynamic Fields
		{
			get
			{
				dynamic val = new ExpandoObject();
				val.mascID = MascID;
				val.serialNoIn = SerialNumberIn;
				val.serialNoOut = SerialNumberOut;
				val.serialNoType = SerialNumberType;
				val.dualSerialNoIn = SerialNumberInDual;
				val.dualSerialNoOut = SerialNumberOutDual;
				val.dualSerialNoType = SerialNumberTypeDual;
				val.repairdate = RepairDate;
				val.iccId = Iccid;
				val.cit = Cit;
				val.apc = Apc;
				val.transModel = TransModel;
				val.custModel = CustModel;
				val.mktModel = MktModel;
				val.itemCode = ItemCode;
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

		public ServiceSerialNumberInput(string mascId, string serialNumberIn, string serialNumberOut, string serialNumberType, string serialNumberInDual, string serialNumberOutDual, string serialNumberTypeDual, string repairDate, string iccid, string cit, string apc, string transModel, string custModel, string mktModel, string itemCode)
		{
			this = default(ServiceSerialNumberInput);
			MascID = mascId;
			SerialNumberIn = serialNumberIn;
			SerialNumberOut = serialNumberOut;
			SerialNumberType = serialNumberType;
			SerialNumberInDual = serialNumberInDual;
			SerialNumberOutDual = serialNumberOutDual;
			SerialNumberTypeDual = serialNumberTypeDual;
			RepairDate = repairDate;
			Iccid = iccid;
			Cit = cit;
			Apc = apc;
			TransModel = transModel;
			CustModel = custModel;
			MktModel = mktModel;
			ItemCode = itemCode;
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

	public struct ServiceSerialNumberOutput
	{
		private string TAG => GetType().FullName;

		public string ResponseCode { get; private set; }

		public string ResponseMessage { get; private set; }

		public ServiceSerialNumberOutput(string responseCode, string responseMessage)
		{
			this = default(ServiceSerialNumberOutput);
			ResponseCode = responseCode;
			ResponseMessage = responseMessage;
		}

		public static ServiceSerialNumberOutput FromDictionary(dynamic fields)
		{
			return new ServiceSerialNumberOutput((string)fields.RES_CODE, (string)fields.EV_RES_MESSAGE);
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

	public ServiceSerialNumberService()
	{
		base.Url = "https://api-cn.lenovo.com/v1.0/service/mbg_service_serial_number/info";
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendRequest(request);
	}

	public virtual ServiceSerialNumberOutput Request(ServiceSerialNumberInput input)
	{
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		Smart.Log.Debug(TAG, $"Contacting ServiceSerialNumberService  ({base.Url})");
		Smart.Log.Verbose(TAG, input.ToString());
		dynamic fields = input.Fields;
		dynamic val = Invoke(fields);
		ServiceSerialNumberOutput result = ServiceSerialNumberOutput.FromDictionary(val);
		Smart.Log.Debug(TAG, "ServiceSerialNumberService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
