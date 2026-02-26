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
[MessageContract(WrapperName = "GenerateVzwKdResponse", WrapperNamespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", IsWrapped = true)]
public class GenerateVzwKdResponse1
{
	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 0)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public generateVzwKdResponse @return;

	public GenerateVzwKdResponse1()
	{
	}

	public GenerateVzwKdResponse1(generateVzwKdResponse @return)
	{
		this.@return = @return;
	}
}
