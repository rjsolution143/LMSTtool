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
[MessageContract(WrapperName = "GenerateVzwKd", WrapperNamespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", IsWrapped = true)]
public class GenerateVzwKdRequest
{
	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 0)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string processname;

	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 1)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string trackid;

	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 2)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string imeimeid;

	public GenerateVzwKdRequest()
	{
	}

	public GenerateVzwKdRequest(string processname, string trackid, string imeimeid)
	{
		this.processname = processname;
		this.trackid = trackid;
		this.imeimeid = imeimeid;
	}
}
