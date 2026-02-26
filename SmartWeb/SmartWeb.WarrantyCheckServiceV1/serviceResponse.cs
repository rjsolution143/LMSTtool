using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace SmartWeb.WarrantyCheckServiceV1;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[MessageContract(IsWrapped = false)]
public class serviceResponse
{
	[MessageBodyMember(Name = "serviceResponse", Namespace = "http://ibase.lenovo.com/webservices/IQS_WARRANTY_CHECK_1.0", Order = 0)]
	public Ws_AHMS_Warranty_Result serviceResponse1;

	public serviceResponse()
	{
	}

	public serviceResponse(Ws_AHMS_Warranty_Result serviceResponse1)
	{
		this.serviceResponse1 = serviceResponse1;
	}
}
