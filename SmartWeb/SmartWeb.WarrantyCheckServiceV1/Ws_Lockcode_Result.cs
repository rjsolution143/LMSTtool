using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace SmartWeb.WarrantyCheckServiceV1;

[Serializable]
[DebuggerStepThrough]
[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
[DataContract(Name = "Ws_Lockcode_Result", Namespace = "java:ibase.lenovo.com.services")]
public class Ws_Lockcode_Result : IExtensibleDataObject, INotifyPropertyChanged
{
	[NonSerialized]
	private ExtensionDataObject extensionDataField;

	private string response_codeField;

	private string response_messageField;

	private string serial_noField;

	private string serial_no_typeField;

	private string a_key_randomField;

	private string track_idField;

	private string msnField;

	private string btField;

	private string wlanField;

	private string wlan2Field;

	private string wlan3Field;

	private string wlan4Field;

	private string software_versionField;

	private string lock_4Field;

	private string service_passcodeField;

	private string secondary_unlock_codeField;

	private string hsn_dataField;

	private string a_key_zeroField;

	private string motorola_masterField;

	private string motorola_onetimeField;

	private string icc_idField;

	private string customer_modelField;

	private string flex_swField;

	private string citField;

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

	[DataMember(IsRequired = true, Order = 4)]
	public string a_key_random
	{
		get
		{
			return a_key_randomField;
		}
		set
		{
			if ((object)a_key_randomField != value)
			{
				a_key_randomField = value;
				RaisePropertyChanged("a_key_random");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 5)]
	public string track_id
	{
		get
		{
			return track_idField;
		}
		set
		{
			if ((object)track_idField != value)
			{
				track_idField = value;
				RaisePropertyChanged("track_id");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 6)]
	public string msn
	{
		get
		{
			return msnField;
		}
		set
		{
			if ((object)msnField != value)
			{
				msnField = value;
				RaisePropertyChanged("msn");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 7)]
	public string bt
	{
		get
		{
			return btField;
		}
		set
		{
			if ((object)btField != value)
			{
				btField = value;
				RaisePropertyChanged("bt");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 8)]
	public string wlan
	{
		get
		{
			return wlanField;
		}
		set
		{
			if ((object)wlanField != value)
			{
				wlanField = value;
				RaisePropertyChanged("wlan");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 9)]
	public string wlan2
	{
		get
		{
			return wlan2Field;
		}
		set
		{
			if ((object)wlan2Field != value)
			{
				wlan2Field = value;
				RaisePropertyChanged("wlan2");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 10)]
	public string wlan3
	{
		get
		{
			return wlan3Field;
		}
		set
		{
			if ((object)wlan3Field != value)
			{
				wlan3Field = value;
				RaisePropertyChanged("wlan3");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 11)]
	public string wlan4
	{
		get
		{
			return wlan4Field;
		}
		set
		{
			if ((object)wlan4Field != value)
			{
				wlan4Field = value;
				RaisePropertyChanged("wlan4");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 12)]
	public string software_version
	{
		get
		{
			return software_versionField;
		}
		set
		{
			if ((object)software_versionField != value)
			{
				software_versionField = value;
				RaisePropertyChanged("software_version");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 13)]
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

	[DataMember(IsRequired = true, Order = 14)]
	public string service_passcode
	{
		get
		{
			return service_passcodeField;
		}
		set
		{
			if ((object)service_passcodeField != value)
			{
				service_passcodeField = value;
				RaisePropertyChanged("service_passcode");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 15)]
	public string secondary_unlock_code
	{
		get
		{
			return secondary_unlock_codeField;
		}
		set
		{
			if ((object)secondary_unlock_codeField != value)
			{
				secondary_unlock_codeField = value;
				RaisePropertyChanged("secondary_unlock_code");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 16)]
	public string hsn_data
	{
		get
		{
			return hsn_dataField;
		}
		set
		{
			if ((object)hsn_dataField != value)
			{
				hsn_dataField = value;
				RaisePropertyChanged("hsn_data");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 17)]
	public string a_key_zero
	{
		get
		{
			return a_key_zeroField;
		}
		set
		{
			if ((object)a_key_zeroField != value)
			{
				a_key_zeroField = value;
				RaisePropertyChanged("a_key_zero");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 18)]
	public string motorola_master
	{
		get
		{
			return motorola_masterField;
		}
		set
		{
			if ((object)motorola_masterField != value)
			{
				motorola_masterField = value;
				RaisePropertyChanged("motorola_master");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 19)]
	public string motorola_onetime
	{
		get
		{
			return motorola_onetimeField;
		}
		set
		{
			if ((object)motorola_onetimeField != value)
			{
				motorola_onetimeField = value;
				RaisePropertyChanged("motorola_onetime");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 20)]
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

	[DataMember(IsRequired = true, Order = 21)]
	public string customer_model
	{
		get
		{
			return customer_modelField;
		}
		set
		{
			if ((object)customer_modelField != value)
			{
				customer_modelField = value;
				RaisePropertyChanged("customer_model");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 22)]
	public string flex_sw
	{
		get
		{
			return flex_swField;
		}
		set
		{
			if ((object)flex_swField != value)
			{
				flex_swField = value;
				RaisePropertyChanged("flex_sw");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 23)]
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

	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
