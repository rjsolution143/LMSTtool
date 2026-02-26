using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace SmartWeb.WarrantyCheckServiceV1;

[Serializable]
[DebuggerStepThrough]
[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
[DataContract(Name = "Ws_Sublock_Result", Namespace = "java:ibase.lenovo.com.services")]
public class Ws_Sublock_Result : IExtensibleDataObject, INotifyPropertyChanged
{
	[NonSerialized]
	private ExtensionDataObject extensionDataField;

	private string response_codeField;

	private string response_messageField;

	private string serial_noField;

	private string serial_no_typeField;

	private string msnField;

	private string status_codeField;

	private string master_lock_codeField;

	private string onetime_lock_codeField;

	private string service_passwordField;

	private string ship_to_customer_idField;

	private string ship_to_customer_addressField;

	private string ship_dateField;

	private string swap_reference_numberField;

	private string prime_coField;

	private string verizonField;

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

	[DataMember(IsRequired = true, Order = 5)]
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

	[DataMember(IsRequired = true, Order = 6)]
	public string master_lock_code
	{
		get
		{
			return master_lock_codeField;
		}
		set
		{
			if ((object)master_lock_codeField != value)
			{
				master_lock_codeField = value;
				RaisePropertyChanged("master_lock_code");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 7)]
	public string onetime_lock_code
	{
		get
		{
			return onetime_lock_codeField;
		}
		set
		{
			if ((object)onetime_lock_codeField != value)
			{
				onetime_lock_codeField = value;
				RaisePropertyChanged("onetime_lock_code");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 8)]
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

	[DataMember(IsRequired = true, Order = 9)]
	public string ship_to_customer_id
	{
		get
		{
			return ship_to_customer_idField;
		}
		set
		{
			if ((object)ship_to_customer_idField != value)
			{
				ship_to_customer_idField = value;
				RaisePropertyChanged("ship_to_customer_id");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 10)]
	public string ship_to_customer_address
	{
		get
		{
			return ship_to_customer_addressField;
		}
		set
		{
			if ((object)ship_to_customer_addressField != value)
			{
				ship_to_customer_addressField = value;
				RaisePropertyChanged("ship_to_customer_address");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 11)]
	public string ship_date
	{
		get
		{
			return ship_dateField;
		}
		set
		{
			if ((object)ship_dateField != value)
			{
				ship_dateField = value;
				RaisePropertyChanged("ship_date");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 12)]
	public string swap_reference_number
	{
		get
		{
			return swap_reference_numberField;
		}
		set
		{
			if ((object)swap_reference_numberField != value)
			{
				swap_reference_numberField = value;
				RaisePropertyChanged("swap_reference_number");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 13)]
	public string prime_co
	{
		get
		{
			return prime_coField;
		}
		set
		{
			if ((object)prime_coField != value)
			{
				prime_coField = value;
				RaisePropertyChanged("prime_co");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 14)]
	public string verizon
	{
		get
		{
			return verizonField;
		}
		set
		{
			if ((object)verizonField != value)
			{
				verizonField = value;
				RaisePropertyChanged("verizon");
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
