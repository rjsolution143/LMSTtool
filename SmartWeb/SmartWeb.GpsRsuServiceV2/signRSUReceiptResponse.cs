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
public class signRSUReceiptResponse : INotifyPropertyChanged
{
	private errorEntity errorInfoField;

	private rsuReceiptData rSUReceiptDataField;

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

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
	public rsuReceiptData RSUReceiptData
	{
		get
		{
			return rSUReceiptDataField;
		}
		set
		{
			rSUReceiptDataField = value;
			RaisePropertyChanged("RSUReceiptData");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
