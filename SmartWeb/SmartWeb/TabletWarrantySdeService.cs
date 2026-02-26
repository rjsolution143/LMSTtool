using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;

namespace SmartWeb;

public class TabletWarrantySdeService : RestService
{
	public struct TabletWarrantyInput
	{
		private string TAG => GetType().FullName;

		public string UserId { get; private set; }

		public string Password { get; private set; }

		public string Language { get; private set; }

		public string SerialNumber { get; private set; }

		public string MachineType { get; private set; }

		public string ProductId { get; private set; }

		public string FruNumber { get; private set; }

		public string ServiceDeliveryProduct { get; private set; }

		public string ServiceDate { get; private set; }

		public bool xAodInfo { get; private set; }

		public bool xBusinessPartnerInfo { get; private set; }

		public bool xContractInfo { get; private set; }

		public bool xX86ContractInfo { get; private set; }

		public bool xEntitleInfo { get; private set; }

		public bool xMachineInfo { get; private set; }

		public bool xPartsInfo { get; private set; }

		public bool xPartsEnhancedInfo { get; private set; }

		public bool xProductHierarchyInfo { get; private set; }

		public bool xServiceInfo { get; private set; }

		public bool xServiceDeliveryType { get; private set; }

		public bool xActivationDate { get; private set; }

		public bool xServiceIdInfo { get; private set; }

		public bool xSoftwareSaeInfo { get; private set; }

		public bool xUpmaInfo { get; private set; }

		public bool xMotoInfo { get; private set; }

		public bool xMhsn { get; private set; }

		public bool xPromptMultiMTFound { get; private set; }

		public dynamic Fields
		{
			get
			{
				dynamic val = new ExpandoObject();
				val.userId = UserId;
				val.password = Password;
				val.language = Language;
				val.serialNumber = SerialNumber;
				val.machineType = MachineType;
				val.productId = ProductId;
				val.fruNumber = FruNumber;
				val.serviceDeliveryProduct = ServiceDeliveryProduct;
				val.serviceDate = ServiceDate;
				if (xAodInfo)
				{
					val.getAodInfo = "X";
				}
				if (xBusinessPartnerInfo)
				{
					val.getBusinessPartnerInfo = "X";
				}
				if (xContractInfo)
				{
					val.getContractInfo = "X";
				}
				if (xX86ContractInfo)
				{
					val.getX86ContractInfo = "X";
				}
				if (xEntitleInfo)
				{
					val.getEntitleInfo = "X";
				}
				if (xMachineInfo)
				{
					val.getMachineInfo = "X";
				}
				if (xPartsInfo)
				{
					val.getPartsInfo = "X";
				}
				if (xPartsEnhancedInfo)
				{
					val.getPartsEnhancedInfo = "X";
				}
				if (xProductHierarchyInfo)
				{
					val.getProductHierarchyInfo = "X";
				}
				if (xServiceInfo)
				{
					val.getServiceInfo = "X";
				}
				if (xServiceDeliveryType)
				{
					val.getServiceDeliveryType = "X";
				}
				if (xActivationDate)
				{
					val.getActivationDate = "X";
				}
				if (xServiceIdInfo)
				{
					val.getServiceIdInfo = "X";
				}
				if (xSoftwareSaeInfo)
				{
					val.getSoftwareSaeInfo = "X";
				}
				if (xUpmaInfo)
				{
					val.getUpmaInfo = "X";
				}
				if (xMotoInfo)
				{
					val.getMotoInfo = "X";
				}
				if (xMhsn)
				{
					val.getMhsn = "X";
				}
				if (xPromptMultiMTFound)
				{
					val.promptMultiMTFound = "X";
				}
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

		public TabletWarrantyInput(string userId, string password, string language, string serialNumber, string machineType, string productId, string fruNumber, string serviceDeliveryProduct, string serviceDate, bool aodInfo, bool businessPartnerInfo, bool x86ContractInfo, bool entitleInfo, bool machineInfo, bool partsInfo, bool partsEnhancedInfo, bool productHierarchyInfo, bool serviceInfo, bool serviceDeliveryType, bool activationDate, bool serviceIdInfo, bool softwareSaeInfo, bool upmaInfo, bool motoInfo, bool mhsn, bool promptMultiMTFound)
		{
			this = default(TabletWarrantyInput);
			UserId = userId;
			Password = password;
			Language = language;
			SerialNumber = serialNumber;
			MachineType = machineType;
			ProductId = productId;
			FruNumber = fruNumber;
			ServiceDeliveryProduct = serviceDeliveryProduct;
			ServiceDate = serviceDate;
			xAodInfo = aodInfo;
			xBusinessPartnerInfo = businessPartnerInfo;
			xContractInfo = xContractInfo;
			xX86ContractInfo = x86ContractInfo;
			xEntitleInfo = entitleInfo;
			xMachineInfo = machineInfo;
			xPartsInfo = partsInfo;
			xPartsEnhancedInfo = partsEnhancedInfo;
			xProductHierarchyInfo = productHierarchyInfo;
			xServiceInfo = serviceInfo;
			xServiceDeliveryType = serviceDeliveryType;
			xActivationDate = activationDate;
			xServiceIdInfo = serviceIdInfo;
			xSoftwareSaeInfo = softwareSaeInfo;
			xUpmaInfo = upmaInfo;
			xMotoInfo = motoInfo;
			xMhsn = mhsn;
			xPromptMultiMTFound = promptMultiMTFound;
		}

		public TabletWarrantyInput(string serialNumber)
		{
			this = default(TabletWarrantyInput);
			UserId = "LMSA";
			Password = "LMSA4IQS";
			Language = "EN";
			SerialNumber = serialNumber;
			xMachineInfo = true;
			xServiceInfo = true;
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

	public struct TabletWarrantyOutput
	{
		private string TAG => GetType().FullName;

		public SortedList<string, string> Fields { get; private set; }

		public TabletWarrantyOutput(dynamic response)
		{
			this = default(TabletWarrantyOutput);
			Fields = new SortedList<string, string>();
			foreach (dynamic item in response)
			{
				dynamic val = item == null;
				if (val || item.Count < 1)
				{
					continue;
				}
				bool flag = false;
				if ((item.Name != null) && ((string)item.Name).EndsWith("List"))
				{
					flag = true;
				}
				foreach (dynamic item2 in item)
				{
					val = item2 == null;
					if (val || item2.Count < 1)
					{
						continue;
					}
					dynamic val2 = new object[1] { item2 };
					if (flag && item2.Count > 0)
					{
						val2 = item2;
					}
					foreach (dynamic item3 in val2)
					{
						foreach (dynamic item4 in item3)
						{
							val = item4.Name == null;
							if (!(val ? true : false) && !((val | (item4.Value == null)) ? true : false))
							{
								string text = item4.Name;
								string text2 = item4.Value;
								if (Fields.ContainsKey(text))
								{
									Smart.Log.Warning(TAG, $"Overwriting existing field {text} value {Fields[text]} with {text2}");
								}
								Fields[text] = text2;
							}
						}
					}
				}
			}
		}
	}

	private string TAG => GetType().FullName;

	public TabletWarrantySdeService()
	{
		base.Url = "https://microapi-cn-t.lenovo.com/sit/v1.0/service/sde/poi_request";
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendRequest(request);
	}

	public TabletWarrantyOutput WarrantyCheck(TabletWarrantyInput input)
	{
		Smart.Log.Debug(TAG, $"Contacting TabletWarrantySdeService  ({base.Url})");
		Smart.Log.Verbose(TAG, input.ToString());
		dynamic val = Invoke(input.Fields);
		TabletWarrantyOutput result = new TabletWarrantyOutput(val);
		Smart.Log.Debug(TAG, "TabletWarrantySdeService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
