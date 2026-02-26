using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace SmartWeb.DataBlockSignServiceV3;

[Serializable]
[DebuggerStepThrough]
[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
[DataContract(Name = "RequestBean", Namespace = "java:com.lenovo.iqs.datablocksign.bean")]
public class RequestBean : IExtensibleDataObject, INotifyPropertyChanged
{
	[NonSerialized]
	private ExtensionDataObject extensionDataField;

	private string MASCIDField;

	private string clientIPField;

	private string clientReqTypeField;

	private string crc32Field;

	private string newIMEIField;

	private string oldIMEIField;

	private string passChgRequdField;

	private byte[] reqParamField;

	[OptionalField]
	private string rsd_log_idField;

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
	public string MASCID
	{
		get
		{
			return MASCIDField;
		}
		set
		{
			if ((object)MASCIDField != value)
			{
				MASCIDField = value;
				RaisePropertyChanged("MASCID");
			}
		}
	}

	[DataMember(IsRequired = true)]
	public string clientIP
	{
		get
		{
			return clientIPField;
		}
		set
		{
			if ((object)clientIPField != value)
			{
				clientIPField = value;
				RaisePropertyChanged("clientIP");
			}
		}
	}

	[DataMember(IsRequired = true)]
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

	[DataMember(IsRequired = true)]
	public string crc32
	{
		get
		{
			return crc32Field;
		}
		set
		{
			if ((object)crc32Field != value)
			{
				crc32Field = value;
				RaisePropertyChanged("crc32");
			}
		}
	}

	[DataMember(IsRequired = true)]
	public string newIMEI
	{
		get
		{
			return newIMEIField;
		}
		set
		{
			if ((object)newIMEIField != value)
			{
				newIMEIField = value;
				RaisePropertyChanged("newIMEI");
			}
		}
	}

	[DataMember(IsRequired = true)]
	public string oldIMEI
	{
		get
		{
			return oldIMEIField;
		}
		set
		{
			if ((object)oldIMEIField != value)
			{
				oldIMEIField = value;
				RaisePropertyChanged("oldIMEI");
			}
		}
	}

	[DataMember(IsRequired = true)]
	public string passChgRequd
	{
		get
		{
			return passChgRequdField;
		}
		set
		{
			if ((object)passChgRequdField != value)
			{
				passChgRequdField = value;
				RaisePropertyChanged("passChgRequd");
			}
		}
	}

	[DataMember(IsRequired = true)]
	public byte[] reqParam
	{
		get
		{
			return reqParamField;
		}
		set
		{
			if (reqParamField != value)
			{
				reqParamField = value;
				RaisePropertyChanged("reqParam");
			}
		}
	}

	[DataMember]
	public string rsd_log_id
	{
		get
		{
			return rsd_log_idField;
		}
		set
		{
			if ((object)rsd_log_idField != value)
			{
				rsd_log_idField = value;
				RaisePropertyChanged("rsd_log_id");
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
