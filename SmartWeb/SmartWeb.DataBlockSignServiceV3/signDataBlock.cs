using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace SmartWeb.DataBlockSignServiceV3;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[MessageContract(IsWrapped = false)]
public class signDataBlock
{
	[MessageBodyMember(Name = "signDataBlock", Namespace = "http://ibase.lenovo.com/webservices/IQS_DataBlockSign_1.0", Order = 0)]
	public RequestBean signDataBlock1;

	public signDataBlock()
	{
	}

	public signDataBlock(RequestBean signDataBlock1)
	{
		this.signDataBlock1 = signDataBlock1;
	}
}
