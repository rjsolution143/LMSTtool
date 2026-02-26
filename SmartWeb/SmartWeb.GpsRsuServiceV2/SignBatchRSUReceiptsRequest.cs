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
[MessageContract(WrapperName = "SignBatchRSUReceipts", WrapperNamespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", IsWrapped = true)]
public class SignBatchRSUReceiptsRequest
{
	[MessageBodyMember(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", Order = 0)]
	[XmlElement("rsureceiptdatalist", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
	public rsuReceiptData[] rsureceiptdatalist;

	public SignBatchRSUReceiptsRequest()
	{
	}

	public SignBatchRSUReceiptsRequest(rsuReceiptData[] rsureceiptdatalist)
	{
		this.rsureceiptdatalist = rsureceiptdatalist;
	}
}
