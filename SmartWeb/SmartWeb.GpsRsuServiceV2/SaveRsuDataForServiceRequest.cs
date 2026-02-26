using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SmartWeb.GpsRsuServiceV2;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[MessageContract(WrapperName = "SaveRsuDataForService", WrapperNamespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", IsWrapped = true)]
public class SaveRsuDataForServiceRequest
{
	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 0)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string trackid;

	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 1)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public rsuReceiptData rsureceiptdata;

	public SaveRsuDataForServiceRequest()
	{
	}

	public SaveRsuDataForServiceRequest(string trackid, rsuReceiptData rsureceiptdata)
	{
		this.trackid = trackid;
		this.rsureceiptdata = rsureceiptdata;
	}
}
