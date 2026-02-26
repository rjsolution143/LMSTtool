using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace SmartWeb.WarrantyCheckServiceV1;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[MessageContract(IsWrapped = false)]
public class service
{
	[MessageBodyMember(Name = "service", Namespace = "http://ibase.lenovo.com/webservices/IQS_WARRANTY_CHECK_1.0", Order = 0)]
	public Ws_AHMS_Warranty_Input service1;

	public service()
	{
	}

	public service(Ws_AHMS_Warranty_Input service1)
	{
		this.service1 = service1;
	}
}
