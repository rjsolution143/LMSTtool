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
[MessageContract(WrapperName = "RequestDataSign", WrapperNamespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", IsWrapped = true)]
public class RequestDataSignRequest
{
	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 0)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string proid;

	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 1)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string type;

	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 2)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string keyname;

	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 3)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string data;

	public RequestDataSignRequest()
	{
	}

	public RequestDataSignRequest(string proid, string type, string keyname, string data)
	{
		this.proid = proid;
		this.type = type;
		this.keyname = keyname;
		this.data = data;
	}
}
