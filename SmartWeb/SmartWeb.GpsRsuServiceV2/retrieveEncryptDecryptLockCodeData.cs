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
public class retrieveEncryptDecryptLockCodeData : INotifyPropertyChanged
{
	private string devicesecretkeyField;

	private errorEntity errorInfoField;

	private string inputimeimeidField;

	private string lockcodeencryptversionField;

	private string lockcodenwscpinputField;

	private string lockcodenwscpoutputField;

	private string lockcodeservicepasscodeinputField;

	private string lockcodeservicepasscodeoutputField;

	private string lockcodesscpinputField;

	private string lockcodesscpoutputField;

	private string siteIdField;

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
	public string devicesecretkey
	{
		get
		{
			return devicesecretkeyField;
		}
		set
		{
			devicesecretkeyField = value;
			RaisePropertyChanged("devicesecretkey");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
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

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
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

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
	public string lockcodeencryptversion
	{
		get
		{
			return lockcodeencryptversionField;
		}
		set
		{
			lockcodeencryptversionField = value;
			RaisePropertyChanged("lockcodeencryptversion");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
	public string lockcodenwscpinput
	{
		get
		{
			return lockcodenwscpinputField;
		}
		set
		{
			lockcodenwscpinputField = value;
			RaisePropertyChanged("lockcodenwscpinput");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
	public string lockcodenwscpoutput
	{
		get
		{
			return lockcodenwscpoutputField;
		}
		set
		{
			lockcodenwscpoutputField = value;
			RaisePropertyChanged("lockcodenwscpoutput");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
	public string lockcodeservicepasscodeinput
	{
		get
		{
			return lockcodeservicepasscodeinputField;
		}
		set
		{
			lockcodeservicepasscodeinputField = value;
			RaisePropertyChanged("lockcodeservicepasscodeinput");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
	public string lockcodeservicepasscodeoutput
	{
		get
		{
			return lockcodeservicepasscodeoutputField;
		}
		set
		{
			lockcodeservicepasscodeoutputField = value;
			RaisePropertyChanged("lockcodeservicepasscodeoutput");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 8)]
	public string lockcodesscpinput
	{
		get
		{
			return lockcodesscpinputField;
		}
		set
		{
			lockcodesscpinputField = value;
			RaisePropertyChanged("lockcodesscpinput");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 9)]
	public string lockcodesscpoutput
	{
		get
		{
			return lockcodesscpoutputField;
		}
		set
		{
			lockcodesscpoutputField = value;
			RaisePropertyChanged("lockcodesscpoutput");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 10)]
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

	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
