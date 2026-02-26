using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using SmartWeb.WarrantyCheckServiceV1;

namespace SmartWeb;

public class WarrantyCheckService : SoapService
{
	public struct WarrantyCheckInput
	{
		public enum WarrantyCheckType
		{
			Normal,
			Unlock
		}

		private string TAG => GetType().FullName;

		public string SerialNumber { get; private set; }

		public string SerialNumberType { get; private set; }

		public WarrantyCheckType CheckType { get; private set; }

		public dynamic Fields
		{
			get
			{
				dynamic val = new ExpandoObject();
				val.serialNumber = SerialNumber;
				val.serialNumberType = SerialNumberType;
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

		public WarrantyCheckInput(string serialNumber, string serialNumberType)
			: this(serialNumber, serialNumberType, WarrantyCheckType.Normal)
		{
		}

		public WarrantyCheckInput(string serialNumber, string serialNumberType, WarrantyCheckType checkType)
		{
			this = default(WarrantyCheckInput);
			SerialNumber = serialNumber;
			SerialNumberType = serialNumberType;
			CheckType = checkType;
		}

		public Ws_AHMS_Warranty_Input ToServiceInput()
		{
			Ws_AHMS_Warranty_Input ws_AHMS_Warranty_Input = new Ws_AHMS_Warranty_Input();
			ws_AHMS_Warranty_Input.@string = SerialNumberType;
			ws_AHMS_Warranty_Input.string0 = SerialNumber;
			ws_AHMS_Warranty_Input.string1 = string.Empty;
			ws_AHMS_Warranty_Input.string2 = string.Empty;
			ws_AHMS_Warranty_Input.string3 = string.Empty;
			ws_AHMS_Warranty_Input.string4 = string.Empty;
			ws_AHMS_Warranty_Input.string5 = string.Empty;
			ws_AHMS_Warranty_Input.string6 = string.Empty;
			ws_AHMS_Warranty_Input.clientReqType = string.Empty;
			ws_AHMS_Warranty_Input.datablocksign = string.Empty;
			ws_AHMS_Warranty_Input.deviceunlockcode = string.Empty;
			ws_AHMS_Warranty_Input.R12DMP = string.Empty;
			ws_AHMS_Warranty_Input.R12PCBA = string.Empty;
			ws_AHMS_Warranty_Input.sublockcode = string.Empty;
			if (CheckType == WarrantyCheckType.Unlock)
			{
				ws_AHMS_Warranty_Input.deviceunlockcode = "Y";
			}
			ws_AHMS_Warranty_Input.id = "MST";
			ws_AHMS_Warranty_Input.pw = "NS1xpq#Y";
			return ws_AHMS_Warranty_Input;
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<string, object> item in (IDictionary<string, object>)Fields)
			{
				if (!(item.Key == "id") && !(item.Key == "pw"))
				{
					dictionary[item.Key] = item.Value.ToString();
				}
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

	public struct WarrantyCheckOutput
	{
		private string TAG => GetType().FullName;

		public SortedList<string, string> Fields { get; private set; }

		public WarrantyCheckOutput(SortedList<string, string> fields)
		{
			this = default(WarrantyCheckOutput);
			Fields = fields;
		}

		public static WarrantyCheckOutput FromServiceOutput(Ws_AHMS_Warranty_Result output)
		{
			object obj = output;
			if (output.device_unlock_code_result != null)
			{
				obj = output.device_unlock_code_result;
			}
			SortedList<string, string> sortedList = new SortedList<string, string>();
			PropertyInfo[] properties = obj.GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.CanRead && propertyInfo.PropertyType.IsPublic)
				{
					string name = propertyInfo.Name;
					object value = propertyInfo.GetValue(obj, null);
					string value2 = null;
					if (value != null)
					{
						value2 = value.ToString();
					}
					sortedList[name] = value2;
				}
			}
			return new WarrantyCheckOutput(sortedList);
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<string, string> item in Fields.ToList())
			{
				dictionary[item.Key] = item.Value;
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

	private string TAG => GetType().FullName;

	public WarrantyCheckService()
	{
		base.Url = "https://wsgw.motorola.com:443/IQS_WARRANTY_CHECK_1.0";
	}

	public override dynamic Invoke(dynamic request)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		QS_WARRANTY_CHECK_10PortClient qS_WARRANTY_CHECK_10PortClient = new QS_WARRANTY_CHECK_10PortClient(binding, new EndpointAddress(base.Url));
		KeyValuePair<string, string> item = new KeyValuePair<string, string>("Authorization", BasicAuthentication);
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
		list.Add(item);
		((ClientBase<IQS_WARRANTY_CHECK_10Port>)qS_WARRANTY_CHECK_10PortClient).Endpoint.EndpointBehaviors.Add((IEndpointBehavior)(object)new SoapListener(base.SentRequest, base.ReceivedReply, list));
		dynamic val = qS_WARRANTY_CHECK_10PortClient.service(request.ToServiceInput());
		return WarrantyCheckOutput.FromServiceOutput(val);
	}

	public WarrantyCheckOutput WarrantyCheck(WarrantyCheckInput input)
	{
		Smart.Log.Debug(TAG, $"Contacting WarrantyCheckService  ({base.Url})");
		Smart.Log.Verbose(TAG, input.ToString());
		WarrantyCheckOutput result = Invoke(input);
		Smart.Log.Debug(TAG, "WarrantyCheckService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
