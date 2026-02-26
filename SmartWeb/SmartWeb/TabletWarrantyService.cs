using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;

namespace SmartWeb;

public class TabletWarrantyService : XmlFormService
{
	public struct TabletWarrantyInput
	{
		private string TAG => GetType().FullName;

		public string UserId { get; private set; }

		public string Password { get; private set; }

		public string Language { get; private set; }

		public string SerialNumber { get; private set; }

		public string Service { get; private set; }

		public string Parts { get; private set; }

		public string Machine { get; private set; }

		public string Aod { get; private set; }

		public string Entitle { get; private set; }

		public string Upma { get; private set; }

		public dynamic Fields
		{
			get
			{
				dynamic val = new ExpandoObject();
				val.id = UserId;
				val.pw = Password;
				val.language = Language;
				val.serial = SerialNumber;
				val.service = Service;
				val.parts = Parts;
				val.machine = Machine;
				val.aod = Aod;
				val.entitle = Entitle;
				val.upma = Upma;
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

		public TabletWarrantyInput(string userId, string password, string language, string serialNumber, string service, string parts, string machine, string aod, string entitle, string upma)
		{
			this = default(TabletWarrantyInput);
			UserId = userId;
			Password = password;
			Language = language;
			SerialNumber = serialNumber;
			Service = service;
			Parts = parts;
			Machine = machine;
			Aod = aod;
			Entitle = entitle;
			Upma = upma;
		}

		public TabletWarrantyInput(string serialNumber)
		{
			this = default(TabletWarrantyInput);
			UserId = "LMSA";
			Password = "LMSA4IQS";
			Language = "English";
			SerialNumber = serialNumber;
			Service = null;
			Parts = null;
			Machine = null;
			Aod = null;
			Entitle = null;
			Upma = null;
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			List<string> list = new List<string> { "id", "pw" };
			foreach (KeyValuePair<string, object> item in (IDictionary<string, object>)Fields)
			{
				if (!list.Contains(item.Key))
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

	public struct TabletWarrantyOutput
	{
		private string TAG => GetType().FullName;

		public SortedList<string, string> Fields { get; private set; }

		public TabletWarrantyOutput(SortedList<string, string> fields)
		{
			this = default(TabletWarrantyOutput);
			Fields = fields;
		}
	}

	private string TAG => GetType().FullName;

	public TabletWarrantyService()
	{
		base.Url = "https://ibase.lenovo.com/POIRequest.aspx";
		base.FormTemplate = "<?xml version=\"1.0\"?> <wiInputForm source = \"spiceworks\"><id></id><pw></pw><language></language><serial></serial><service/><parts/><machine/><aod/><entitle/><upma/></wiInputForm>";
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendForm(request);
	}

	public TabletWarrantyOutput WarrantyCheck(TabletWarrantyInput input)
	{
		Smart.Log.Debug(TAG, $"Contacting TabletWarrantyService  ({base.Url})");
		Smart.Log.Verbose(TAG, input.ToString());
		SortedList<string, string> fields = Invoke(input.Fields);
		TabletWarrantyOutput result = new TabletWarrantyOutput(fields);
		Smart.Log.Debug(TAG, "TabletWarrantyService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
