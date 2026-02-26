using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;

namespace SmartWeb;

public class MachineInfoService : RestService
{
	public struct MachineInfoInput
	{
		private string TAG => GetType().FullName;

		public string RecordId { get; private set; }

		public string IMEI { get; private set; }

		public string IMEI2 { get; private set; }

		public string CountryCode { get; private set; }

		public string ManufacturingDate { get; private set; }

		public string Model { get; private set; }

		public string LoginId { get; private set; }

		public string SN { get; private set; }

		public dynamic Fields
		{
			get
			{
				dynamic val = new ExpandoObject();
				val.RecordId = RecordId;
				val.IMEI = IMEI;
				val.IMEI2 = IMEI2;
				val.Country_Code = CountryCode;
				val.Manufacturing_Date = ManufacturingDate;
				val.Model = Model;
				val.Login_Id = LoginId;
				val.SN = SN;
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

		public MachineInfoInput(string recordId, string imei, string imei2, string countryCode, string manufacturingDate, string model, string loginId, string sn)
		{
			this = default(MachineInfoInput);
			RecordId = recordId;
			IMEI = imei;
			IMEI2 = imei2;
			CountryCode = countryCode;
			ManufacturingDate = manufacturingDate;
			Model = model;
			LoginId = loginId;
			SN = sn;
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

	public struct MachineInfoOutput
	{
		private string TAG => GetType().FullName;

		public string Status { get; private set; }

		public string StatusCode { get; private set; }

		public string StatusDescription { get; private set; }

		public MachineInfoOutput(string status, string statusCode, string statusDescription)
		{
			this = default(MachineInfoOutput);
			Status = status;
			StatusCode = statusCode;
			StatusDescription = statusDescription;
		}

		public static MachineInfoOutput FromDictionary(dynamic fields)
		{
			return new MachineInfoOutput((string)fields.status, (string)fields.status_code, (string)fields.status_desc);
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["Status"] = Status;
			dictionary["StatusCode"] = StatusCode;
			dictionary["StatusDescription"] = StatusDescription;
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

	public MachineInfoService()
	{
		base.Url = "https://api-cn.lenovo.com/moto_machine_info/1.0/add";
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendRequest(request);
	}

	public MachineInfoOutput SendInfo(MachineInfoInput input)
	{
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		Smart.Log.Debug(TAG, $"Contacting MachineInfoService  ({base.Url})");
		Smart.Log.Verbose(TAG, input.ToString());
		dynamic fields = input.Fields;
		dynamic val = new SortedList<string, object>();
		val["request"] = new SortedList<string, object>();
		val["request"]["input"] = new SortedList<string, object>();
		val["request"]["input"]["header"] = fields;
		dynamic val2 = Invoke(val);
		dynamic val3 = val2["response"]["header"];
		MachineInfoOutput result = MachineInfoOutput.FromDictionary(val3);
		Smart.Log.Debug(TAG, "MachineInfoService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
