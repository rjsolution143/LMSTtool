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
public class generateVzwKdResponse : INotifyPropertyChanged
{
	private errorEntity errorInfoField;

	private string inputimeimeidField;

	private string siteIdField;

	private string vzwkdField;

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
	public string inputimeimeid
	{
		get
		{
			return inputimeimeidField;
		}
		set
		{
			inputimeimeidField = value;
			RaisePropertyChanged("inputimeimeid");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
	public string siteId
	{
		get
		{
			return siteIdField;
		}
		set
		{
			siteIdField = value;
			RaisePropertyChanged("siteId");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
	public string vzwkd
	{
		get
		{
			return vzwkdField;
		}
		set
		{
			vzwkdField = value;
			RaisePropertyChanged("vzwkd");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
