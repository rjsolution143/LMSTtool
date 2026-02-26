using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace SmartWeb.DataBlockSignServiceV3;

[Serializable]
[DebuggerStepThrough]
[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
[DataContract(Name = "ClientResponse", Namespace = "java:com.mot.dbs.services.bean")]
public class ClientResponse : IExtensibleDataObject, INotifyPropertyChanged
{
	[NonSerialized]
	private ExtensionDataObject extensionDataField;

	private byte[] pkiResponseField;

	private string statusCodeField;

	private string statusDataField;

	private string transactionIDField;

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
	public byte[] pkiResponse
	{
		get
		{
			return pkiResponseField;
		}
		set
		{
			if (pkiResponseField != value)
			{
				pkiResponseField = value;
				RaisePropertyChanged("pkiResponse");
			}
		}
	}

	[DataMember(IsRequired = true)]
	public string statusCode
	{
		get
		{
			return statusCodeField;
		}
		set
		{
			if ((object)statusCodeField != value)
			{
				statusCodeField = value;
				RaisePropertyChanged("statusCode");
			}
		}
	}

	[DataMember(IsRequired = true)]
	public string statusData
	{
		get
		{
			return statusDataField;
		}
		set
		{
			if ((object)statusDataField != value)
			{
				statusDataField = value;
				RaisePropertyChanged("statusData");
			}
		}
	}

	[DataMember(IsRequired = true)]
	public string transactionID
	{
		get
		{
			return transactionIDField;
		}
		set
		{
			if ((object)transactionIDField != value)
			{
				transactionIDField = value;
				RaisePropertyChanged("transactionID");
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
