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
[MessageContract(WrapperName = "EncryptDecryptSubsidyLockCode", WrapperNamespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", IsWrapped = true)]
public class EncryptDecryptSubsidyLockCodeRequest
{
	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 0)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string processname;

	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 1)]
	[XmlElement("encryptdecryptinput", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
	public retrieveEncryptDecryptLockCodeData[] encryptdecryptinput;

	public EncryptDecryptSubsidyLockCodeRequest()
	{
	}

	public EncryptDecryptSubsidyLockCodeRequest(string processname, retrieveEncryptDecryptLockCodeData[] encryptdecryptinput)
	{
		this.processname = processname;
		this.encryptdecryptinput = encryptdecryptinput;
	}
}
