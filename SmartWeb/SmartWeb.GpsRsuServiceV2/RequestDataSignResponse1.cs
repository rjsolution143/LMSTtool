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
[MessageContract(WrapperName = "RequestDataSignResponse", WrapperNamespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", IsWrapped = true)]
public class RequestDataSignResponse1
{
	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 0)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public requestDataSignResponse @return;

	public RequestDataSignResponse1()
	{
	}

	public RequestDataSignResponse1(requestDataSignResponse @return)
	{
		this.@return = @return;
	}
}
