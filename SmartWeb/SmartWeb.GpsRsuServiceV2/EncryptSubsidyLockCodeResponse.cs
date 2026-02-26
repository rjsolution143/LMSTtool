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
[MessageContract(WrapperName = "EncryptSubsidyLockCodeResponse", WrapperNamespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", IsWrapped = true)]
public class EncryptSubsidyLockCodeResponse
{
	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 0)]
	[XmlElement("return", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
	public retrieveEncryptDecryptLockCodeData[] @return;

	public EncryptSubsidyLockCodeResponse()
	{
	}

	public EncryptSubsidyLockCodeResponse(retrieveEncryptDecryptLockCodeData[] @return)
	{
		this.@return = @return;
	}
}
