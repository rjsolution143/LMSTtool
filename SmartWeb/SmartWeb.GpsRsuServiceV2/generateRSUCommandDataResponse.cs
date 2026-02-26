using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SmartWeb.GpsRsuServiceV2;

[Serializable]
[GeneratedCode("System.Xml", "4.8.4084.0")]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/")]
public class generateRSUCommandDataResponse : INotifyPropertyChanged
{
	private errorEntity errorInfoField;

	private byte[] signedCommandDataField;

	private string signedCommandDataInHexField;

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
	public errorEntity errorInfo
	{
		get
		{
			return errorInfoField;
		}
		set
		{
			errorInfoField = value;
			RaisePropertyChanged("errorInfo");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "base64Binary", Order = 1)]
	public byte[] signedCommandData
	{
		get
		{
			return signedCommandDataField;
		}
		set
		{
			signedCommandDataField = value;
			RaisePropertyChanged("signedCommandData");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
	public string signedCommandDataInHex
	{
		get
		{
			return signedCommandDataInHexField;
		}
		set
		{
			signedCommandDataInHexField = value;
			RaisePropertyChanged("signedCommandDataInHex");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
