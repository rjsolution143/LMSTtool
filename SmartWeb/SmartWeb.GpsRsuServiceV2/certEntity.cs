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
public class certEntity : INotifyPropertyChanged
{
	private string certBlobField;

	[XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
	public string certBlob
	{
		get
		{
			return certBlobField;
		}
		set
		{
			certBlobField = value;
			RaisePropertyChanged("certBlob");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
