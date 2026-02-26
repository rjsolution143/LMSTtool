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
public class errorEntity : INotifyPropertyChanged
{
	private int errorCodeField;

	private string errorMessageField;

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
	public int errorCode
	{
		get
		{
			return errorCodeField;
		}
		set
		{
			errorCodeField = value;
			RaisePropertyChanged("errorCode");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
	public string errorMessage
	{
		get
		{
			return errorMessageField;
		}
		set
		{
			errorMessageField = value;
			RaisePropertyChanged("errorMessage");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
