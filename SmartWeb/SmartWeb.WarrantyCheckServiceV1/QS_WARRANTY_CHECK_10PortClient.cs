using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace SmartWeb.WarrantyCheckServiceV1;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
public class QS_WARRANTY_CHECK_10PortClient : ClientBase<IQS_WARRANTY_CHECK_10Port>, IQS_WARRANTY_CHECK_10Port
{
	public QS_WARRANTY_CHECK_10PortClient()
	{
	}

	public QS_WARRANTY_CHECK_10PortClient(string endpointConfigurationName)
		: base(endpointConfigurationName)
	{
	}

	public QS_WARRANTY_CHECK_10PortClient(string endpointConfigurationName, string remoteAddress)
		: base(endpointConfigurationName, remoteAddress)
	{
	}

	public QS_WARRANTY_CHECK_10PortClient(string endpointConfigurationName, EndpointAddress remoteAddress)
		: base(endpointConfigurationName, remoteAddress)
	{
	}

	public QS_WARRANTY_CHECK_10PortClient(Binding binding, EndpointAddress remoteAddress)
		: base(binding, remoteAddress)
	{
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	serviceResponse IQS_WARRANTY_CHECK_10Port.service(service request)
	{
		return base.Channel.service(request);
	}

	public Ws_AHMS_Warranty_Result service(Ws_AHMS_Warranty_Input service1)
	{
		service service2 = new service();
		service2.service1 = service1;
		return ((IQS_WARRANTY_CHECK_10Port)this).service(service2).serviceResponse1;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<serviceResponse> IQS_WARRANTY_CHECK_10Port.serviceAsync(service request)
	{
		return base.Channel.serviceAsync(request);
	}

	public Task<serviceResponse> serviceAsync(Ws_AHMS_Warranty_Input service1)
	{
		service service2 = new service();
		service2.service1 = service1;
		return ((IQS_WARRANTY_CHECK_10Port)this).serviceAsync(service2);
	}
}
