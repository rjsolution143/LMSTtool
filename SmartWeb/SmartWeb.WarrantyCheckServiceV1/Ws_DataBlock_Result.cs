using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace SmartWeb.WarrantyCheckServiceV1;

[Serializable]
[DebuggerStepThrough]
[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
[DataContract(Name = "Ws_DataBlock_Result", Namespace = "java:ibase.lenovo.com.services")]
public class Ws_DataBlock_Result : IExtensibleDataObject, INotifyPropertyChanged
{
	[NonSerialized]
	private ExtensionDataObject extensionDataField;

	private string response_codeField;

	private string response_messageField;

	private string service_passcodeField;

	private string network_passcodeField;

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

	[DataMember(IsRequired = true, EmitDefaultValue = false)]
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

	[DataMember(IsRequired = true, EmitDefaultValue = false, Order = 3)]
	public string network_passcode
	{
		get
		{
			return network_passcodeField;
		}
		set
		{
			if ((object)network_passcodeField != value)
			{
				network_passcodeField = value;
				RaisePropertyChanged("network_passcode");
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
