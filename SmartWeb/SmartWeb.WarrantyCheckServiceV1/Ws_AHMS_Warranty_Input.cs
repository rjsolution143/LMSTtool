using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace SmartWeb.WarrantyCheckServiceV1;

[Serializable]
[DebuggerStepThrough]
[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
[DataContract(Name = "Ws_AHMS_Warranty_Input", Namespace = "java:ibase.lenovo.com.services")]
public class Ws_AHMS_Warranty_Input : IExtensibleDataObject, INotifyPropertyChanged
{
	[NonSerialized]
	private ExtensionDataObject extensionDataField;

	private string idField;

	private string pwField;

	private string stringField;

	private string string0Field;

	[OptionalField]
	private string string1Field;

	[OptionalField]
	private string string2Field;

	[OptionalField]
	private string string3Field;

	[OptionalField]
	private string string4Field;

	[OptionalField]
	private string string5Field;

	[OptionalField]
	private string string6Field;

	[OptionalField]
	private string sublockcodeField;

	[OptionalField]
	private string deviceunlockcodeField;

	[OptionalField]
	private string datablocksignField;

	[OptionalField]
	private string clientReqTypeField;

	[OptionalField]
	private string R12DMPField;

	[OptionalField]
	private string R12PCBAField;

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
	public string id
	{
		get
		{
			return idField;
		}
		set
		{
			if ((object)idField != value)
			{
				idField = value;
				RaisePropertyChanged("id");
			}
		}
	}

	[DataMember(IsRequired = true)]
	public string pw
	{
		get
		{
			return pwField;
		}
		set
		{
			if ((object)pwField != value)
			{
				pwField = value;
				RaisePropertyChanged("pw");
			}
		}
	}

	[DataMember(IsRequired = true)]
	public string @string
	{
		get
		{
			return stringField;
		}
		set
		{
			if ((object)stringField != value)
			{
				stringField = value;
				RaisePropertyChanged("string");
			}
		}
	}

	[DataMember(IsRequired = true)]
	public string string0
	{
		get
		{
			return string0Field;
		}
		set
		{
			if ((object)string0Field != value)
			{
				string0Field = value;
				RaisePropertyChanged("string0");
			}
		}
	}

	[DataMember]
	public string string1
	{
		get
		{
			return string1Field;
		}
		set
		{
			if ((object)string1Field != value)
			{
				string1Field = value;
				RaisePropertyChanged("string1");
			}
		}
	}

	[DataMember]
	public string string2
	{
		get
		{
			return string2Field;
		}
		set
		{
			if ((object)string2Field != value)
			{
				string2Field = value;
				RaisePropertyChanged("string2");
			}
		}
	}

	[DataMember]
	public string string3
	{
		get
		{
			return string3Field;
		}
		set
		{
			if ((object)string3Field != value)
			{
				string3Field = value;
				RaisePropertyChanged("string3");
			}
		}
	}

	[DataMember]
	public string string4
	{
		get
		{
			return string4Field;
		}
		set
		{
			if ((object)string4Field != value)
			{
				string4Field = value;
				RaisePropertyChanged("string4");
			}
		}
	}

	[DataMember]
	public string string5
	{
		get
		{
			return string5Field;
		}
		set
		{
			if ((object)string5Field != value)
			{
				string5Field = value;
				RaisePropertyChanged("string5");
			}
		}
	}

	[DataMember]
	public string string6
	{
		get
		{
			return string6Field;
		}
		set
		{
			if ((object)string6Field != value)
			{
				string6Field = value;
				RaisePropertyChanged("string6");
			}
		}
	}

	[DataMember]
	public string sublockcode
	{
		get
		{
			return sublockcodeField;
		}
		set
		{
			if ((object)sublockcodeField != value)
			{
				sublockcodeField = value;
				RaisePropertyChanged("sublockcode");
			}
		}
	}

	[DataMember(Order = 11)]
	public string deviceunlockcode
	{
		get
		{
			return deviceunlockcodeField;
		}
		set
		{
			if ((object)deviceunlockcodeField != value)
			{
				deviceunlockcodeField = value;
				RaisePropertyChanged("deviceunlockcode");
			}
		}
	}

	[DataMember(Order = 12)]
	public string datablocksign
	{
		get
		{
			return datablocksignField;
		}
		set
		{
			if ((object)datablocksignField != value)
			{
				datablocksignField = value;
				RaisePropertyChanged("datablocksign");
			}
		}
	}

	[DataMember(Order = 13)]
	public string clientReqType
	{
		get
		{
			return clientReqTypeField;
		}
		set
		{
			if ((object)clientReqTypeField != value)
			{
				clientReqTypeField = value;
				RaisePropertyChanged("clientReqType");
			}
		}
	}

	[DataMember(Order = 14)]
	public string R12DMP
	{
		get
		{
			return R12DMPField;
		}
		set
		{
			if ((object)R12DMPField != value)
			{
				R12DMPField = value;
				RaisePropertyChanged("R12DMP");
			}
		}
	}

	[DataMember(Order = 15)]
	public string R12PCBA
	{
		get
		{
			return R12PCBAField;
		}
		set
		{
			if ((object)R12PCBAField != value)
			{
				R12PCBAField = value;
				RaisePropertyChanged("R12PCBA");
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
