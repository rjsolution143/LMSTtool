using System.CodeDom.Compiler;
using System.ServiceModel;
using System.Threading.Tasks;

namespace SmartWeb.WarrantyCheckServiceV1;

[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[ServiceContract(Name = "IQS_WARRANTY_CHECK_1.0Port", Namespace = "http://ibase.lenovo.com/webservices/IQS_WARRANTY_CHECK_1.0", ConfigurationName = "WarrantyCheckServiceV1.IQS_WARRANTY_CHECK_10Port")]
public interface IQS_WARRANTY_CHECK_10Port
{
	[OperationContract(Action = "", ReplyAction = "*")]
	serviceResponse service(service request);

	[OperationContract(Action = "", ReplyAction = "*")]
	Task<serviceResponse> serviceAsync(service request);
}
