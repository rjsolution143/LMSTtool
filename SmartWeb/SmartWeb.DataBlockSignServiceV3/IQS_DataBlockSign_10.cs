using System.CodeDom.Compiler;
using System.ServiceModel;
using System.Threading.Tasks;

namespace SmartWeb.DataBlockSignServiceV3;

[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[ServiceContract(Name = "IQS_DataBlockSign_1.0", Namespace = "http://ibase.lenovo.com/webservices/IQS_DataBlockSign_1.0", ConfigurationName = "DataBlockSignServiceV3.IQS_DataBlockSign_10")]
public interface IQS_DataBlockSign_10
{
	[OperationContract(Action = "", ReplyAction = "*")]
	signDataBlockResponse signDataBlock(signDataBlock request);

	[OperationContract(Action = "", ReplyAction = "*")]
	Task<signDataBlockResponse> signDataBlockAsync(signDataBlock request);
}
