using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace SmartWeb.DataBlockSignServiceV3;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[MessageContract(IsWrapped = false)]
public class signDataBlockResponse
{
	[MessageBodyMember(Name = "signDataBlockResponse", Namespace = "http://ibase.lenovo.com/webservices/IQS_DataBlockSign_1.0", Order = 0)]
	public ClientResponse signDataBlockResponse1;

	public signDataBlockResponse()
	{
	}

	public signDataBlockResponse(ClientResponse signDataBlockResponse1)
	{
		this.signDataBlockResponse1 = signDataBlockResponse1;
	}
}
