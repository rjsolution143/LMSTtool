using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace SmartWeb.WarrantyCheckServiceV1;

[Serializable]
[DebuggerStepThrough]
[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
[DataContract(Name = "Ws_FSB_Data_Result", Namespace = "java:ibase.lenovo.com.services")]
public class Ws_FSB_Data_Result : IExtensibleDataObject, INotifyPropertyChanged
{
	[NonSerialized]
	private ExtensionDataObject extensionDataField;

	private string fsb_dataField;

	private string response_CodeField;

	private string response_MessageField;

	private int? number_of_fsbField;

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
	public string fsb_data
	{
		get
		{
			return fsb_dataField;
		}
		set
		{
			if ((object)fsb_dataField != value)
			{
				fsb_dataField = value;
				RaisePropertyChanged("fsb_data");
			}
		}
	}

	[DataMember(IsRequired = true)]
	public string response_Code
	{
		get
		{
			return response_CodeField;
		}
		set
		{
			if ((object)response_CodeField != value)
			{
				response_CodeField = value;
				RaisePropertyChanged("response_Code");
			}
		}
	}

	[DataMember(IsRequired = true)]
	public string response_Message
	{
		get
		{
			return response_MessageField;
		}
		set
		{
			if ((object)response_MessageField != value)
			{
				response_MessageField = value;
				RaisePropertyChanged("response_Message");
			}
		}
	}

	[DataMember(IsRequired = true, Order = 3)]
	public int? number_of_fsb
	{
		get
		{
			return number_of_fsbField;
		}
		set
		{
			if (!number_of_fsbField.Equals(value))
			{
				number_of_fsbField = value;
				RaisePropertyChanged("number_of_fsb");
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
