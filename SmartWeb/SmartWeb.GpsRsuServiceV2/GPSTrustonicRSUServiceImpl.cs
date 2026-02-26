using System.CodeDom.Compiler;
using System.ServiceModel;
using System.Threading.Tasks;

namespace SmartWeb.GpsRsuServiceV2;

[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[ServiceContract(Namespace = "http://rsu.programmingservice.cfc.nextest.globaltest.motorolamobility.com/", ConfigurationName = "GpsRsuServiceV2.GPSTrustonicRSUServiceImpl")]
public interface GPSTrustonicRSUServiceImpl
{
	[OperationContract(Action = "", ReplyAction = "*")]
	[XmlSerializerFormat(SupportFaults = true)]
	[return: MessageParameter(Name = "return")]
	GenerateRSUCommandDataResponse1 GenerateRSUCommandData(GenerateRSUCommandDataRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	Task<GenerateRSUCommandDataResponse1> GenerateRSUCommandDataAsync(GenerateRSUCommandDataRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	[XmlSerializerFormat(SupportFaults = true)]
	[return: MessageParameter(Name = "return")]
	SignRSUReceiptResponse1 SignRSUReceipt(SignRSUReceiptRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	Task<SignRSUReceiptResponse1> SignRSUReceiptAsync(SignRSUReceiptRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	[XmlSerializerFormat(SupportFaults = true)]
	[return: MessageParameter(Name = "return")]
	SignBatchRSUReceiptsResponse1 SignBatchRSUReceipts(SignBatchRSUReceiptsRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	Task<SignBatchRSUReceiptsResponse1> SignBatchRSUReceiptsAsync(SignBatchRSUReceiptsRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	[XmlSerializerFormat(SupportFaults = true)]
	[return: MessageParameter(Name = "return")]
	SaveRsuDataForServiceResponse1 SaveRsuDataForService(SaveRsuDataForServiceRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	Task<SaveRsuDataForServiceResponse1> SaveRsuDataForServiceAsync(SaveRsuDataForServiceRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	[XmlSerializerFormat(SupportFaults = true)]
	[return: MessageParameter(Name = "return")]
	RequestDataSignResponse1 RequestDataSign(RequestDataSignRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	Task<RequestDataSignResponse1> RequestDataSignAsync(RequestDataSignRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	[XmlSerializerFormat(SupportFaults = true)]
	[return: MessageParameter(Name = "return")]
	RetrieveCertByTypeResponse RetrieveCertByType(RetrieveCertByTypeRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	Task<RetrieveCertByTypeResponse> RetrieveCertByTypeAsync(RetrieveCertByTypeRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	[XmlSerializerFormat(SupportFaults = true)]
	[return: MessageParameter(Name = "return")]
	EncryptSubsidyLockCodeResponse EncryptSubsidyLockCode(EncryptSubsidyLockCodeRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	Task<EncryptSubsidyLockCodeResponse> EncryptSubsidyLockCodeAsync(EncryptSubsidyLockCodeRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	[XmlSerializerFormat(SupportFaults = true)]
	[return: MessageParameter(Name = "return")]
	RequestUnlockPasswordResponse1 RequestUnlockPassword(RequestUnlockPasswordRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	Task<RequestUnlockPasswordResponse1> RequestUnlockPasswordAsync(RequestUnlockPasswordRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	[XmlSerializerFormat(SupportFaults = true)]
	[return: MessageParameter(Name = "return")]
	SaveRpkDataForServiceResponse1 SaveRpkDataForService(SaveRpkDataForServiceRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	Task<SaveRpkDataForServiceResponse1> SaveRpkDataForServiceAsync(SaveRpkDataForServiceRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	[XmlSerializerFormat(SupportFaults = true)]
	[return: MessageParameter(Name = "return")]
	SaveWeChatDataForServiceResponse SaveWeChatDataForService(SaveWeChatDataForServiceRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	Task<SaveWeChatDataForServiceResponse> SaveWeChatDataForServiceAsync(SaveWeChatDataForServiceRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	[XmlSerializerFormat(SupportFaults = true)]
	[return: MessageParameter(Name = "return")]
	EncryptDecryptSubsidyLockCodeResponse EncryptDecryptSubsidyLockCode(EncryptDecryptSubsidyLockCodeRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	Task<EncryptDecryptSubsidyLockCodeResponse> EncryptDecryptSubsidyLockCodeAsync(EncryptDecryptSubsidyLockCodeRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	[XmlSerializerFormat(SupportFaults = true)]
	[return: MessageParameter(Name = "return")]
	GenerateVzwKdResponse1 GenerateVzwKd(GenerateVzwKdRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	Task<GenerateVzwKdResponse1> GenerateVzwKdAsync(GenerateVzwKdRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	[XmlSerializerFormat(SupportFaults = true)]
	[return: MessageParameter(Name = "return")]
	SaveSubsidyLockCodesForServiceResponse1 SaveSubsidyLockCodesForService(SaveSubsidyLockCodesForServiceRequest request);

	[OperationContract(Action = "", ReplyAction = "*")]
	Task<SaveSubsidyLockCodesForServiceResponse1> SaveSubsidyLockCodesForServiceAsync(SaveSubsidyLockCodesForServiceRequest request);
}
