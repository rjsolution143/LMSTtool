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
public class rsuReceiptData : INotifyPropertyChanged
{
	private string deviceModelField;

	private string iMEIField;

	private string mNOField;

	private string receiptCounterField;

	private byte[] receiptDataField;

	private string receiptDataStringField;

	private string sUIDField;

	private string siPField;

	private byte[] signedReceiptField;

	private string soCModelField;

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
	public string deviceModel
	{
		get
		{
			return deviceModelField;
		}
		set
		{
			deviceModelField = value;
			RaisePropertyChanged("deviceModel");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
	public string IMEI
	{
		get
		{
			return iMEIField;
		}
		set
		{
			iMEIField = value;
			RaisePropertyChanged("IMEI");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
	public string MNO
	{
		get
		{
			return mNOField;
		}
		set
		{
			mNOField = value;
			RaisePropertyChanged("MNO");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
	public string receiptCounter
	{
		get
		{
			return receiptCounterField;
		}
		set
		{
			receiptCounterField = value;
			RaisePropertyChanged("receiptCounter");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "base64Binary", Order = 4)]
	public byte[] receiptData
	{
		get
		{
			return receiptDataField;
		}
		set
		{
			receiptDataField = value;
			RaisePropertyChanged("receiptData");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
	public string receiptDataString
	{
		get
		{
			return receiptDataStringField;
		}
		set
		{
			receiptDataStringField = value;
			RaisePropertyChanged("receiptDataString");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
	public string SUID
	{
		get
		{
			return sUIDField;
		}
		set
		{
			sUIDField = value;
			RaisePropertyChanged("SUID");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
	public string siP
	{
		get
		{
			return siPField;
		}
		set
		{
			siPField = value;
			RaisePropertyChanged("siP");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "base64Binary", Order = 8)]
	public byte[] signedReceipt
	{
		get
		{
			return signedReceiptField;
		}
		set
		{
			signedReceiptField = value;
			RaisePropertyChanged("signedReceipt");
		}
	}

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 9)]
	public string soCModel
	{
		get
		{
			return soCModelField;
		}
		set
		{
			soCModelField = value;
			RaisePropertyChanged("soCModel");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
