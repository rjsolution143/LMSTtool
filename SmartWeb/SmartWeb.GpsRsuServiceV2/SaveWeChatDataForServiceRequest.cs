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
[MessageContract(WrapperName = "SaveWeChatDataForService", WrapperNamespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", IsWrapped = true)]
public class SaveWeChatDataForServiceRequest
{
	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 0)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string trackid;

	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 1)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string attkpubkey;

	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 2)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string attkpubkeyuid;

	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 3)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string attkbrandname;

	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 4)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string attkproductmodel;

	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 5)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string attksecuritylevel;

	public SaveWeChatDataForServiceRequest()
	{
	}

	public SaveWeChatDataForServiceRequest(string trackid, string attkpubkey, string attkpubkeyuid, string attkbrandname, string attkproductmodel, string attksecuritylevel)
	{
		this.trackid = trackid;
		this.attkpubkey = attkpubkey;
		this.attkpubkeyuid = attkpubkeyuid;
		this.attkbrandname = attkbrandname;
		this.attkproductmodel = attkproductmodel;
		this.attksecuritylevel = attksecuritylevel;
	}
}
