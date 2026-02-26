using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace SmartWeb.WarrantyCheckServiceV1;

[Serializable]
[DebuggerStepThrough]
[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
[DataContract(Name = "Ws_DevUnlock_Result", Namespace = "java:ibase.lenovo.com.services")]
public class Ws_DevUnlock_Result : IExtensibleDataObject, INotifyPropertyChanged
{
	[NonSerialized]
	private ExtensionDataObject extensionDataField;

	private string response_codeField;

	private string response_messageField;

	private string serial_noField;

	private string serial_no_typeField;

	private string status_codeField;

	private string master_service_lockField;

	private string onetime_lockField;

	private string lock_4Field;

	private string lock_5Field;

	private string a_key1Field;

	private string a_key2Field;

	private string evolution_dataField;

	private string apcField;

	private string universal_lan_mac_addressField;

	private string service_passwordField;

	private string wlan_mac_addressField;

	private string wimax_mac_addressField;

	private string device_typeField;

	private string statusField;

	private string carrier_model_infoField;

	private string external_marketing_nameField;

	private string country_to_shipField;

	private string direct_ship_country_codeField;

	private string warranty_country_codeField;

	private string wlan_mac_address2Field;

	private string wlan_mac_address3Field;

	private string wlan_mac_address4Field;

	private string icc_idField;

	private string citField;

	private string imckeyField;

	private string ship_to_cust_nameField;

	private string sold_to_cust_nameField;

	private string direct_ship_cust_nameField;

	private string base_processor_idField;

	[Browsable(false)]
	public ExtensionDataObject ExtensionData
	{
		get
		{
			return extensionDataField;
		}
		set
		{
			extensionDataField = value;
		}
	}

	[DataMember(IsRequired = true)]
	public string response_code
	{
		get
		{
			return response_codeField;
		}
		set
		{
			if ((object)response_codeField != value)
			{
				response_codeField = value;
				RaisePropertyChanged("response_code");
			}
		}
	}

	[DataMember(IsRequired = true)]
	public string response_message
	{
		get
		{
			return response_messageField;
		}
		set
		{
			if ((object)response_messageField != value)
			{
				response_messageField = value;
				RaisePropertyChanged("response_message");
			}
		}
	}

	[DataMember(IsRequired = true)]
	public string serial_no
	{
		get
		{
			return serial_noField;
		}
		set
		{
			if ((object)serial_noField != value)
			{
				serial_noField = value;
				RaisePropertyChanged("serial_no");
			}
		}
	}

	[DataMember(IsRequired = true)]
	public string serial_no_type
	{
		get
		{
			return serial_no_typeField;
		}
		set
		{
			if ((object)serial_no_typeField != value)
			{
				serial_no_typeField = value;
				RaisePropertyChanged("serial_no_type");
			}
		}
	}

	[DataMember(IsRequired = true)]
	public string status_code
	{
		get
		{
			return status_codeField;
		}
		set
		{
			if ((object)status_codeField != value)
			{
				status_codeField = value;
				RaisePropertyChanged("status_code");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 5)]
	public string master_service_lock
	{
		get
		{
			return master_service_lockField;
		}
		set
		{
			if ((object)master_service_lockField != value)
			{
				master_service_lockField = value;
				RaisePropertyChanged("master_service_lock");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 6)]
	public string onetime_lock
	{
		get
		{
			return onetime_lockField;
		}
		set
		{
			if ((object)onetime_lockField != value)
			{
				onetime_lockField = value;
				RaisePropertyChanged("onetime_lock");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 7)]
	public string lock_4
	{
		get
		{
			return lock_4Field;
		}
		set
		{
			if ((object)lock_4Field != value)
			{
				lock_4Field = value;
				RaisePropertyChanged("lock_4");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 8)]
	public string lock_5
	{
		get
		{
			return lock_5Field;
		}
		set
		{
			if ((object)lock_5Field != value)
			{
				lock_5Field = value;
				RaisePropertyChanged("lock_5");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 9)]
	public string a_key1
	{
		get
		{
			return a_key1Field;
		}
		set
		{
			if ((object)a_key1Field != value)
			{
				a_key1Field = value;
				RaisePropertyChanged("a_key1");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 10)]
	public string a_key2
	{
		get
		{
			return a_key2Field;
		}
		set
		{
			if ((object)a_key2Field != value)
			{
				a_key2Field = value;
				RaisePropertyChanged("a_key2");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 11)]
	public string evolution_data
	{
		get
		{
			return evolution_dataField;
		}
		set
		{
			if ((object)evolution_dataField != value)
			{
				evolution_dataField = value;
				RaisePropertyChanged("evolution_data");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 12)]
	public string apc
	{
		get
		{
			return apcField;
		}
		set
		{
			if ((object)apcField != value)
			{
				apcField = value;
				RaisePropertyChanged("apc");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 13)]
	public string universal_lan_mac_address
	{
		get
		{
			return universal_lan_mac_addressField;
		}
		set
		{
			if ((object)universal_lan_mac_addressField != value)
			{
				universal_lan_mac_addressField = value;
				RaisePropertyChanged("universal_lan_mac_address");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 14)]
	public string service_password
	{
		get
		{
			return service_passwordField;
		}
		set
		{
			if ((object)service_passwordField != value)
			{
				service_passwordField = value;
				RaisePropertyChanged("service_password");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 15)]
	public string wlan_mac_address
	{
		get
		{
			return wlan_mac_addressField;
		}
		set
		{
			if ((object)wlan_mac_addressField != value)
			{
				wlan_mac_addressField = value;
				RaisePropertyChanged("wlan_mac_address");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 16)]
	public string wimax_mac_address
	{
		get
		{
			return wimax_mac_addressField;
		}
		set
		{
			if ((object)wimax_mac_addressField != value)
			{
				wimax_mac_addressField = value;
				RaisePropertyChanged("wimax_mac_address");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 17)]
	public string device_type
	{
		get
		{
			return device_typeField;
		}
		set
		{
			if ((object)device_typeField != value)
			{
				device_typeField = value;
				RaisePropertyChanged("device_type");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 18)]
	public string status
	{
		get
		{
			return statusField;
		}
		set
		{
			if ((object)statusField != value)
			{
				statusField = value;
				RaisePropertyChanged("status");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 19)]
	public string carrier_model_info
	{
		get
		{
			return carrier_model_infoField;
		}
		set
		{
			if ((object)carrier_model_infoField != value)
			{
				carrier_model_infoField = value;
				RaisePropertyChanged("carrier_model_info");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 20)]
	public string external_marketing_name
	{
		get
		{
			return external_marketing_nameField;
		}
		set
		{
			if ((object)external_marketing_nameField != value)
			{
				external_marketing_nameField = value;
				RaisePropertyChanged("external_marketing_name");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 21)]
	public string country_to_ship
	{
		get
		{
			return country_to_shipField;
		}
		set
		{
			if ((object)country_to_shipField != value)
			{
				country_to_shipField = value;
				RaisePropertyChanged("country_to_ship");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 22)]
	public string direct_ship_country_code
	{
		get
		{
			return direct_ship_country_codeField;
		}
		set
		{
			if ((object)direct_ship_country_codeField != value)
			{
				direct_ship_country_codeField = value;
				RaisePropertyChanged("direct_ship_country_code");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 23)]
	public string warranty_country_code
	{
		get
		{
			return warranty_country_codeField;
		}
		set
		{
			if ((object)warranty_country_codeField != value)
			{
				warranty_country_codeField = value;
				RaisePropertyChanged("warranty_country_code");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 24)]
	public string wlan_mac_address2
	{
		get
		{
			return wlan_mac_address2Field;
		}
		set
		{
			if ((object)wlan_mac_address2Field != value)
			{
				wlan_mac_address2Field = value;
				RaisePropertyChanged("wlan_mac_address2");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 25)]
	public string wlan_mac_address3
	{
		get
		{
			return wlan_mac_address3Field;
		}
		set
		{
			if ((object)wlan_mac_address3Field != value)
			{
				wlan_mac_address3Field = value;
				RaisePropertyChanged("wlan_mac_address3");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 26)]
	public string wlan_mac_address4
	{
		get
		{
			return wlan_mac_address4Field;
		}
		set
		{
			if ((object)wlan_mac_address4Field != value)
			{
				wlan_mac_address4Field = value;
				RaisePropertyChanged("wlan_mac_address4");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 27)]
	public string icc_id
	{
		get
		{
			return icc_idField;
		}
		set
		{
			if ((object)icc_idField != value)
			{
				icc_idField = value;
				RaisePropertyChanged("icc_id");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 28)]
	public string cit
	{
		get
		{
			return citField;
		}
		set
		{
			if ((object)citField != value)
			{
				citField = value;
				RaisePropertyChanged("cit");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 29)]
	public string imckey
	{
		get
		{
			return imckeyField;
		}
		set
		{
			if ((object)imckeyField != value)
			{
				imckeyField = value;
				RaisePropertyChanged("imckey");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 30)]
	public string ship_to_cust_name
	{
		get
		{
			return ship_to_cust_nameField;
		}
		set
		{
			if ((object)ship_to_cust_nameField != value)
			{
				ship_to_cust_nameField = value;
				RaisePropertyChanged("ship_to_cust_name");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 31)]
	public string sold_to_cust_name
	{
		get
		{
			return sold_to_cust_nameField;
		}
		set
		{
			if ((object)sold_to_cust_nameField != value)
			{
				sold_to_cust_nameField = value;
				RaisePropertyChanged("sold_to_cust_name");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 32)]
	public string direct_ship_cust_name
	{
		get
		{
			return direct_ship_cust_nameField;
		}
		set
		{
			if ((object)direct_ship_cust_nameField != value)
			{
				direct_ship_cust_nameField = value;
				RaisePropertyChanged("direct_ship_cust_name");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 33)]
	public string base_processor_id
	{
		get
		{
			return base_processor_idField;
		}
		set
		{
			if ((object)base_processor_idField != value)
			{
				base_processor_idField = value;
				RaisePropertyChanged("base_processor_id");
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
