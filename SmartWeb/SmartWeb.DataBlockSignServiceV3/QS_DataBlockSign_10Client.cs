using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace SmartWeb.DataBlockSignServiceV3;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
public class QS_DataBlockSign_10Client : ClientBase<IQS_DataBlockSign_10>, IQS_DataBlockSign_10
{
	public QS_DataBlockSign_10Client()
	{
	}

	public QS_DataBlockSign_10Client(string endpointConfigurationName)
		: base(endpointConfigurationName)
	{
	}

	public QS_DataBlockSign_10Client(string endpointConfigurationName, string remoteAddress)
		: base(endpointConfigurationName, remoteAddress)
	{
	}

	public QS_DataBlockSign_10Client(string endpointConfigurationName, EndpointAddress remoteAddress)
		: base(endpointConfigurationName, remoteAddress)
	{
	}

	public QS_DataBlockSign_10Client(Binding binding, EndpointAddress remoteAddress)
		: base(binding, remoteAddress)
	{
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	signDataBlockResponse IQS_DataBlockSign_10.signDataBlock(signDataBlock request)
	{
		return base.Channel.signDataBlock(request);
	}

	public ClientResponse signDataBlock(RequestBean signDataBlock1)
	{
		signDataBlock signDataBlock2 = new signDataBlock();
		signDataBlock2.signDataBlock1 = signDataBlock1;
		return ((IQS_DataBlockSign_10)this).signDataBlock(signDataBlock2).signDataBlockResponse1;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<signDataBlockResponse> IQS_DataBlockSign_10.signDataBlockAsync(signDataBlock request)
	{
		return base.Channel.signDataBlockAsync(request);
	}

	public Task<signDataBlockResponse> signDataBlockAsync(RequestBean signDataBlock1)
	{
		signDataBlock signDataBlock2 = new signDataBlock();
		signDataBlock2.signDataBlock1 = signDataBlock1;
		return ((IQS_DataBlockSign_10)this).signDataBlockAsync(signDataBlock2);
	}
}
