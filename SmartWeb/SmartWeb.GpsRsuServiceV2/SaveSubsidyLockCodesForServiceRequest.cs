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
[MessageContract(WrapperName = "SaveSubsidyLockCodesForService", WrapperNamespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", IsWrapped = true)]
public class SaveSubsidyLockCodesForServiceRequest
{
	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 0)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string trackid;

	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 1)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string gsm_imei;

	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 2)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string gsm_imei2;

	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 3)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string gsm_nwscp;

	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 4)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string gsm_sscp;

	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 5)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string service_passcode;

	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 6)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string device_secret_key;

	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 7)]
	[XmlElement(Form = XmlSchemaForm.Unqualified)]
	public string esim_eid;

	public SaveSubsidyLockCodesForServiceRequest()
	{
	}

	public SaveSubsidyLockCodesForServiceRequest(string trackid, string gsm_imei, string gsm_imei2, string gsm_nwscp, string gsm_sscp, string service_passcode, string device_secret_key, string esim_eid)
	{
		this.trackid = trackid;
		this.gsm_imei = gsm_imei;
		this.gsm_imei2 = gsm_imei2;
		this.gsm_nwscp = gsm_nwscp;
		this.gsm_sscp = gsm_sscp;
		this.service_passcode = service_passcode;
		this.device_secret_key = device_secret_key;
		this.esim_eid = esim_eid;
	}
}
