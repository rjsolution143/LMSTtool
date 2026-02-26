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
[MessageContract(WrapperName = "SaveRpkDataForServiceResponse", WrapperNamespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", IsWrapped = true)]
public class SaveRpkDataForServiceResponse1
{
	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 0)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public saveRpkDataForServiceResponse @return;

	public SaveRpkDataForServiceResponse1()
	{
	}

	public SaveRpkDataForServiceResponse1(saveRpkDataForServiceResponse @return)
	{
		this.@return = @return;
	}
}
