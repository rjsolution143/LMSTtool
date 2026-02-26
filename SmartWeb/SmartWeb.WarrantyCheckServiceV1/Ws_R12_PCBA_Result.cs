using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace SmartWeb.WarrantyCheckServiceV1;

[Serializable]
[DebuggerStepThrough]
[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
[DataContract(Name = "Ws_R12_PCBA_Result", Namespace = "java:ibase.lenovo.com.services")]
public class Ws_R12_PCBA_Result : IExtensibleDataObject, INotifyPropertyChanged
{
	[NonSerialized]
	private ExtensionDataObject extensionDataField;

	private string response_codeField;

	private string response_messageField;

	private string serial_inField;

	private string serial_outField;

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
	public string serial_in
	{
		get
		{
			return serial_inField;
		}
		set
		{
			if ((object)serial_inField != value)
			{
				serial_inField = value;
				RaisePropertyChanged("serial_in");
			}
		}
	}

	[DataMember(IsRequired = true)]
	public string serial_out
	{
		get
		{
			return serial_outField;
		}
		set
		{
			if ((object)serial_outField != value)
			{
				serial_outField = value;
				RaisePropertyChanged("serial_out");
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
