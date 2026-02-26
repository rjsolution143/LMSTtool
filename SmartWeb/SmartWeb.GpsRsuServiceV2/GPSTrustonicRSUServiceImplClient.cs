using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace SmartWeb.GpsRsuServiceV2;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
public class GPSTrustonicRSUServiceImplClient : ClientBase<GPSTrustonicRSUServiceImpl>, GPSTrustonicRSUServiceImpl
{
	public GPSTrustonicRSUServiceImplClient()
	{
	}

	public GPSTrustonicRSUServiceImplClient(string endpointConfigurationName)
		: base(endpointConfigurationName)
	{
	}

	public GPSTrustonicRSUServiceImplClient(string endpointConfigurationName, string remoteAddress)
		: base(endpointConfigurationName, remoteAddress)
	{
	}

	public GPSTrustonicRSUServiceImplClient(string endpointConfigurationName, EndpointAddress remoteAddress)
		: base(endpointConfigurationName, remoteAddress)
	{
	}

	public GPSTrustonicRSUServiceImplClient(Binding binding, EndpointAddress remoteAddress)
		: base(binding, remoteAddress)
	{
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	GenerateRSUCommandDataResponse1 GPSTrustonicRSUServiceImpl.GenerateRSUCommandData(GenerateRSUCommandDataRequest request)
	{
		return base.Channel.GenerateRSUCommandData(request);
	}

	public generateRSUCommandDataResponse GenerateRSUCommandData()
	{
		GenerateRSUCommandDataRequest request = new GenerateRSUCommandDataRequest();
		return ((GPSTrustonicRSUServiceImpl)this).GenerateRSUCommandData(request).@return;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<GenerateRSUCommandDataResponse1> GPSTrustonicRSUServiceImpl.GenerateRSUCommandDataAsync(GenerateRSUCommandDataRequest request)
	{
		return base.Channel.GenerateRSUCommandDataAsync(request);
	}

	public Task<GenerateRSUCommandDataResponse1> GenerateRSUCommandDataAsync()
	{
		GenerateRSUCommandDataRequest request = new GenerateRSUCommandDataRequest();
		return ((GPSTrustonicRSUServiceImpl)this).GenerateRSUCommandDataAsync(request);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	SignRSUReceiptResponse1 GPSTrustonicRSUServiceImpl.SignRSUReceipt(SignRSUReceiptRequest request)
	{
		return base.Channel.SignRSUReceipt(request);
	}

	public signRSUReceiptResponse SignRSUReceipt(rsuReceiptData rsureceiptdata)
	{
		SignRSUReceiptRequest signRSUReceiptRequest = new SignRSUReceiptRequest();
		signRSUReceiptRequest.rsureceiptdata = rsureceiptdata;
		return ((GPSTrustonicRSUServiceImpl)this).SignRSUReceipt(signRSUReceiptRequest).@return;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<SignRSUReceiptResponse1> GPSTrustonicRSUServiceImpl.SignRSUReceiptAsync(SignRSUReceiptRequest request)
	{
		return base.Channel.SignRSUReceiptAsync(request);
	}

	public Task<SignRSUReceiptResponse1> SignRSUReceiptAsync(rsuReceiptData rsureceiptdata)
	{
		SignRSUReceiptRequest signRSUReceiptRequest = new SignRSUReceiptRequest();
		signRSUReceiptRequest.rsureceiptdata = rsureceiptdata;
		return ((GPSTrustonicRSUServiceImpl)this).SignRSUReceiptAsync(signRSUReceiptRequest);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	SignBatchRSUReceiptsResponse1 GPSTrustonicRSUServiceImpl.SignBatchRSUReceipts(SignBatchRSUReceiptsRequest request)
	{
		return base.Channel.SignBatchRSUReceipts(request);
	}

	public signBatchRSUReceiptsResponse SignBatchRSUReceipts(rsuReceiptData[] rsureceiptdatalist)
	{
		SignBatchRSUReceiptsRequest signBatchRSUReceiptsRequest = new SignBatchRSUReceiptsRequest();
		signBatchRSUReceiptsRequest.rsureceiptdatalist = rsureceiptdatalist;
		return ((GPSTrustonicRSUServiceImpl)this).SignBatchRSUReceipts(signBatchRSUReceiptsRequest).@return;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<SignBatchRSUReceiptsResponse1> GPSTrustonicRSUServiceImpl.SignBatchRSUReceiptsAsync(SignBatchRSUReceiptsRequest request)
	{
		return base.Channel.SignBatchRSUReceiptsAsync(request);
	}

	public Task<SignBatchRSUReceiptsResponse1> SignBatchRSUReceiptsAsync(rsuReceiptData[] rsureceiptdatalist)
	{
		SignBatchRSUReceiptsRequest signBatchRSUReceiptsRequest = new SignBatchRSUReceiptsRequest();
		signBatchRSUReceiptsRequest.rsureceiptdatalist = rsureceiptdatalist;
		return ((GPSTrustonicRSUServiceImpl)this).SignBatchRSUReceiptsAsync(signBatchRSUReceiptsRequest);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	SaveRsuDataForServiceResponse1 GPSTrustonicRSUServiceImpl.SaveRsuDataForService(SaveRsuDataForServiceRequest request)
	{
		return base.Channel.SaveRsuDataForService(request);
	}

	public saveRsuDataForServiceResponse SaveRsuDataForService(string trackid, rsuReceiptData rsureceiptdata)
	{
		SaveRsuDataForServiceRequest saveRsuDataForServiceRequest = new SaveRsuDataForServiceRequest();
		saveRsuDataForServiceRequest.trackid = trackid;
		saveRsuDataForServiceRequest.rsureceiptdata = rsureceiptdata;
		return ((GPSTrustonicRSUServiceImpl)this).SaveRsuDataForService(saveRsuDataForServiceRequest).@return;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<SaveRsuDataForServiceResponse1> GPSTrustonicRSUServiceImpl.SaveRsuDataForServiceAsync(SaveRsuDataForServiceRequest request)
	{
		return base.Channel.SaveRsuDataForServiceAsync(request);
	}

	public Task<SaveRsuDataForServiceResponse1> SaveRsuDataForServiceAsync(string trackid, rsuReceiptData rsureceiptdata)
	{
		SaveRsuDataForServiceRequest saveRsuDataForServiceRequest = new SaveRsuDataForServiceRequest();
		saveRsuDataForServiceRequest.trackid = trackid;
		saveRsuDataForServiceRequest.rsureceiptdata = rsureceiptdata;
		return ((GPSTrustonicRSUServiceImpl)this).SaveRsuDataForServiceAsync(saveRsuDataForServiceRequest);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	RequestDataSignResponse1 GPSTrustonicRSUServiceImpl.RequestDataSign(RequestDataSignRequest request)
	{
		return base.Channel.RequestDataSign(request);
	}

	public requestDataSignResponse RequestDataSign(string proid, string type, string keyname, string data)
	{
		RequestDataSignRequest requestDataSignRequest = new RequestDataSignRequest();
		requestDataSignRequest.proid = proid;
		requestDataSignRequest.type = type;
		requestDataSignRequest.keyname = keyname;
		requestDataSignRequest.data = data;
		return ((GPSTrustonicRSUServiceImpl)this).RequestDataSign(requestDataSignRequest).@return;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<RequestDataSignResponse1> GPSTrustonicRSUServiceImpl.RequestDataSignAsync(RequestDataSignRequest request)
	{
		return base.Channel.RequestDataSignAsync(request);
	}

	public Task<RequestDataSignResponse1> RequestDataSignAsync(string proid, string type, string keyname, string data)
	{
		RequestDataSignRequest requestDataSignRequest = new RequestDataSignRequest();
		requestDataSignRequest.proid = proid;
		requestDataSignRequest.type = type;
		requestDataSignRequest.keyname = keyname;
		requestDataSignRequest.data = data;
		return ((GPSTrustonicRSUServiceImpl)this).RequestDataSignAsync(requestDataSignRequest);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	RetrieveCertByTypeResponse GPSTrustonicRSUServiceImpl.RetrieveCertByType(RetrieveCertByTypeRequest request)
	{
		return base.Channel.RetrieveCertByType(request);
	}

	public retrieveCertResponse RetrieveCertByType(string certModel, string certType)
	{
		RetrieveCertByTypeRequest retrieveCertByTypeRequest = new RetrieveCertByTypeRequest();
		retrieveCertByTypeRequest.certModel = certModel;
		retrieveCertByTypeRequest.certType = certType;
		return ((GPSTrustonicRSUServiceImpl)this).RetrieveCertByType(retrieveCertByTypeRequest).@return;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<RetrieveCertByTypeResponse> GPSTrustonicRSUServiceImpl.RetrieveCertByTypeAsync(RetrieveCertByTypeRequest request)
	{
		return base.Channel.RetrieveCertByTypeAsync(request);
	}

	public Task<RetrieveCertByTypeResponse> RetrieveCertByTypeAsync(string certModel, string certType)
	{
		RetrieveCertByTypeRequest retrieveCertByTypeRequest = new RetrieveCertByTypeRequest();
		retrieveCertByTypeRequest.certModel = certModel;
		retrieveCertByTypeRequest.certType = certType;
		return ((GPSTrustonicRSUServiceImpl)this).RetrieveCertByTypeAsync(retrieveCertByTypeRequest);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	EncryptSubsidyLockCodeResponse GPSTrustonicRSUServiceImpl.EncryptSubsidyLockCode(EncryptSubsidyLockCodeRequest request)
	{
		return base.Channel.EncryptSubsidyLockCode(request);
	}

	public retrieveEncryptDecryptLockCodeData[] EncryptSubsidyLockCode(string processname, retrieveEncryptDecryptLockCodeData[] encryptdecryptinput)
	{
		EncryptSubsidyLockCodeRequest encryptSubsidyLockCodeRequest = new EncryptSubsidyLockCodeRequest();
		encryptSubsidyLockCodeRequest.processname = processname;
		encryptSubsidyLockCodeRequest.encryptdecryptinput = encryptdecryptinput;
		return ((GPSTrustonicRSUServiceImpl)this).EncryptSubsidyLockCode(encryptSubsidyLockCodeRequest).@return;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<EncryptSubsidyLockCodeResponse> GPSTrustonicRSUServiceImpl.EncryptSubsidyLockCodeAsync(EncryptSubsidyLockCodeRequest request)
	{
		return base.Channel.EncryptSubsidyLockCodeAsync(request);
	}

	public Task<EncryptSubsidyLockCodeResponse> EncryptSubsidyLockCodeAsync(string processname, retrieveEncryptDecryptLockCodeData[] encryptdecryptinput)
	{
		EncryptSubsidyLockCodeRequest encryptSubsidyLockCodeRequest = new EncryptSubsidyLockCodeRequest();
		encryptSubsidyLockCodeRequest.processname = processname;
		encryptSubsidyLockCodeRequest.encryptdecryptinput = encryptdecryptinput;
		return ((GPSTrustonicRSUServiceImpl)this).EncryptSubsidyLockCodeAsync(encryptSubsidyLockCodeRequest);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	RequestUnlockPasswordResponse1 GPSTrustonicRSUServiceImpl.RequestUnlockPassword(RequestUnlockPasswordRequest request)
	{
		return base.Channel.RequestUnlockPassword(request);
	}

	public requestUnlockPasswordResponse RequestUnlockPassword(string product_name, string cpu_id, string build_type)
	{
		RequestUnlockPasswordRequest requestUnlockPasswordRequest = new RequestUnlockPasswordRequest();
		requestUnlockPasswordRequest.product_name = product_name;
		requestUnlockPasswordRequest.cpu_id = cpu_id;
		requestUnlockPasswordRequest.build_type = build_type;
		return ((GPSTrustonicRSUServiceImpl)this).RequestUnlockPassword(requestUnlockPasswordRequest).@return;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<RequestUnlockPasswordResponse1> GPSTrustonicRSUServiceImpl.RequestUnlockPasswordAsync(RequestUnlockPasswordRequest request)
	{
		return base.Channel.RequestUnlockPasswordAsync(request);
	}

	public Task<RequestUnlockPasswordResponse1> RequestUnlockPasswordAsync(string product_name, string cpu_id, string build_type)
	{
		RequestUnlockPasswordRequest requestUnlockPasswordRequest = new RequestUnlockPasswordRequest();
		requestUnlockPasswordRequest.product_name = product_name;
		requestUnlockPasswordRequest.cpu_id = cpu_id;
		requestUnlockPasswordRequest.build_type = build_type;
		return ((GPSTrustonicRSUServiceImpl)this).RequestUnlockPasswordAsync(requestUnlockPasswordRequest);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	SaveRpkDataForServiceResponse1 GPSTrustonicRSUServiceImpl.SaveRpkDataForService(SaveRpkDataForServiceRequest request)
	{
		return base.Channel.SaveRpkDataForService(request);
	}

	public saveRpkDataForServiceResponse SaveRpkDataForService(string trackid, string googlecsr)
	{
		SaveRpkDataForServiceRequest saveRpkDataForServiceRequest = new SaveRpkDataForServiceRequest();
		saveRpkDataForServiceRequest.trackid = trackid;
		saveRpkDataForServiceRequest.googlecsr = googlecsr;
		return ((GPSTrustonicRSUServiceImpl)this).SaveRpkDataForService(saveRpkDataForServiceRequest).@return;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<SaveRpkDataForServiceResponse1> GPSTrustonicRSUServiceImpl.SaveRpkDataForServiceAsync(SaveRpkDataForServiceRequest request)
	{
		return base.Channel.SaveRpkDataForServiceAsync(request);
	}

	public Task<SaveRpkDataForServiceResponse1> SaveRpkDataForServiceAsync(string trackid, string googlecsr)
	{
		SaveRpkDataForServiceRequest saveRpkDataForServiceRequest = new SaveRpkDataForServiceRequest();
		saveRpkDataForServiceRequest.trackid = trackid;
		saveRpkDataForServiceRequest.googlecsr = googlecsr;
		return ((GPSTrustonicRSUServiceImpl)this).SaveRpkDataForServiceAsync(saveRpkDataForServiceRequest);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	SaveWeChatDataForServiceResponse GPSTrustonicRSUServiceImpl.SaveWeChatDataForService(SaveWeChatDataForServiceRequest request)
	{
		return base.Channel.SaveWeChatDataForService(request);
	}

	public saveAttkDataForServiceResponse SaveWeChatDataForService(string trackid, string attkpubkey, string attkpubkeyuid, string attkbrandname, string attkproductmodel, string attksecuritylevel)
	{
		SaveWeChatDataForServiceRequest saveWeChatDataForServiceRequest = new SaveWeChatDataForServiceRequest();
		saveWeChatDataForServiceRequest.trackid = trackid;
		saveWeChatDataForServiceRequest.attkpubkey = attkpubkey;
		saveWeChatDataForServiceRequest.attkpubkeyuid = attkpubkeyuid;
		saveWeChatDataForServiceRequest.attkbrandname = attkbrandname;
		saveWeChatDataForServiceRequest.attkproductmodel = attkproductmodel;
		saveWeChatDataForServiceRequest.attksecuritylevel = attksecuritylevel;
		return ((GPSTrustonicRSUServiceImpl)this).SaveWeChatDataForService(saveWeChatDataForServiceRequest).@return;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<SaveWeChatDataForServiceResponse> GPSTrustonicRSUServiceImpl.SaveWeChatDataForServiceAsync(SaveWeChatDataForServiceRequest request)
	{
		return base.Channel.SaveWeChatDataForServiceAsync(request);
	}

	public Task<SaveWeChatDataForServiceResponse> SaveWeChatDataForServiceAsync(string trackid, string attkpubkey, string attkpubkeyuid, string attkbrandname, string attkproductmodel, string attksecuritylevel)
	{
		SaveWeChatDataForServiceRequest saveWeChatDataForServiceRequest = new SaveWeChatDataForServiceRequest();
		saveWeChatDataForServiceRequest.trackid = trackid;
		saveWeChatDataForServiceRequest.attkpubkey = attkpubkey;
		saveWeChatDataForServiceRequest.attkpubkeyuid = attkpubkeyuid;
		saveWeChatDataForServiceRequest.attkbrandname = attkbrandname;
		saveWeChatDataForServiceRequest.attkproductmodel = attkproductmodel;
		saveWeChatDataForServiceRequest.attksecuritylevel = attksecuritylevel;
		return ((GPSTrustonicRSUServiceImpl)this).SaveWeChatDataForServiceAsync(saveWeChatDataForServiceRequest);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	EncryptDecryptSubsidyLockCodeResponse GPSTrustonicRSUServiceImpl.EncryptDecryptSubsidyLockCode(EncryptDecryptSubsidyLockCodeRequest request)
	{
		return base.Channel.EncryptDecryptSubsidyLockCode(request);
	}

	public retrieveEncryptDecryptLockCodeData[] EncryptDecryptSubsidyLockCode(string processname, retrieveEncryptDecryptLockCodeData[] encryptdecryptinput)
	{
		EncryptDecryptSubsidyLockCodeRequest encryptDecryptSubsidyLockCodeRequest = new EncryptDecryptSubsidyLockCodeRequest();
		encryptDecryptSubsidyLockCodeRequest.processname = processname;
		encryptDecryptSubsidyLockCodeRequest.encryptdecryptinput = encryptdecryptinput;
		return ((GPSTrustonicRSUServiceImpl)this).EncryptDecryptSubsidyLockCode(encryptDecryptSubsidyLockCodeRequest).@return;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<EncryptDecryptSubsidyLockCodeResponse> GPSTrustonicRSUServiceImpl.EncryptDecryptSubsidyLockCodeAsync(EncryptDecryptSubsidyLockCodeRequest request)
	{
		return base.Channel.EncryptDecryptSubsidyLockCodeAsync(request);
	}

	public Task<EncryptDecryptSubsidyLockCodeResponse> EncryptDecryptSubsidyLockCodeAsync(string processname, retrieveEncryptDecryptLockCodeData[] encryptdecryptinput)
	{
		EncryptDecryptSubsidyLockCodeRequest encryptDecryptSubsidyLockCodeRequest = new EncryptDecryptSubsidyLockCodeRequest();
		encryptDecryptSubsidyLockCodeRequest.processname = processname;
		encryptDecryptSubsidyLockCodeRequest.encryptdecryptinput = encryptdecryptinput;
		return ((GPSTrustonicRSUServiceImpl)this).EncryptDecryptSubsidyLockCodeAsync(encryptDecryptSubsidyLockCodeRequest);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	GenerateVzwKdResponse1 GPSTrustonicRSUServiceImpl.GenerateVzwKd(GenerateVzwKdRequest request)
	{
		return base.Channel.GenerateVzwKd(request);
	}

	public generateVzwKdResponse GenerateVzwKd(string processname, string trackid, string imeimeid)
	{
		GenerateVzwKdRequest generateVzwKdRequest = new GenerateVzwKdRequest();
		generateVzwKdRequest.processname = processname;
		generateVzwKdRequest.trackid = trackid;
		generateVzwKdRequest.imeimeid = imeimeid;
		return ((GPSTrustonicRSUServiceImpl)this).GenerateVzwKd(generateVzwKdRequest).@return;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<GenerateVzwKdResponse1> GPSTrustonicRSUServiceImpl.GenerateVzwKdAsync(GenerateVzwKdRequest request)
	{
		return base.Channel.GenerateVzwKdAsync(request);
	}

	public Task<GenerateVzwKdResponse1> GenerateVzwKdAsync(string processname, string trackid, string imeimeid)
	{
		GenerateVzwKdRequest generateVzwKdRequest = new GenerateVzwKdRequest();
		generateVzwKdRequest.processname = processname;
		generateVzwKdRequest.trackid = trackid;
		generateVzwKdRequest.imeimeid = imeimeid;
		return ((GPSTrustonicRSUServiceImpl)this).GenerateVzwKdAsync(generateVzwKdRequest);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	SaveSubsidyLockCodesForServiceResponse1 GPSTrustonicRSUServiceImpl.SaveSubsidyLockCodesForService(SaveSubsidyLockCodesForServiceRequest request)
	{
		return base.Channel.SaveSubsidyLockCodesForService(request);
	}

	public saveSubsidyLockCodesForServiceResponse SaveSubsidyLockCodesForService(string trackid, string gsm_imei, string gsm_imei2, string gsm_nwscp, string gsm_sscp, string service_passcode, string device_secret_key, string esim_eid)
	{
		SaveSubsidyLockCodesForServiceRequest saveSubsidyLockCodesForServiceRequest = new SaveSubsidyLockCodesForServiceRequest();
		saveSubsidyLockCodesForServiceRequest.trackid = trackid;
		saveSubsidyLockCodesForServiceRequest.gsm_imei = gsm_imei;
		saveSubsidyLockCodesForServiceRequest.gsm_imei2 = gsm_imei2;
		saveSubsidyLockCodesForServiceRequest.gsm_nwscp = gsm_nwscp;
		saveSubsidyLockCodesForServiceRequest.gsm_sscp = gsm_sscp;
		saveSubsidyLockCodesForServiceRequest.service_passcode = service_passcode;
		saveSubsidyLockCodesForServiceRequest.device_secret_key = device_secret_key;
		saveSubsidyLockCodesForServiceRequest.esim_eid = esim_eid;
		return ((GPSTrustonicRSUServiceImpl)this).SaveSubsidyLockCodesForService(saveSubsidyLockCodesForServiceRequest).@return;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<SaveSubsidyLockCodesForServiceResponse1> GPSTrustonicRSUServiceImpl.SaveSubsidyLockCodesForServiceAsync(SaveSubsidyLockCodesForServiceRequest request)
	{
		return base.Channel.SaveSubsidyLockCodesForServiceAsync(request);
	}

	public Task<SaveSubsidyLockCodesForServiceResponse1> SaveSubsidyLockCodesForServiceAsync(string trackid, string gsm_imei, string gsm_imei2, string gsm_nwscp, string gsm_sscp, string service_passcode, string device_secret_key, string esim_eid)
	{
		SaveSubsidyLockCodesForServiceRequest saveSubsidyLockCodesForServiceRequest = new SaveSubsidyLockCodesForServiceRequest();
		saveSubsidyLockCodesForServiceRequest.trackid = trackid;
		saveSubsidyLockCodesForServiceRequest.gsm_imei = gsm_imei;
		saveSubsidyLockCodesForServiceRequest.gsm_imei2 = gsm_imei2;
		saveSubsidyLockCodesForServiceRequest.gsm_nwscp = gsm_nwscp;
		saveSubsidyLockCodesForServiceRequest.gsm_sscp = gsm_sscp;
		saveSubsidyLockCodesForServiceRequest.service_passcode = service_passcode;
		saveSubsidyLockCodesForServiceRequest.device_secret_key = device_secret_key;
		saveSubsidyLockCodesForServiceRequest.esim_eid = esim_eid;
		return ((GPSTrustonicRSUServiceImpl)this).SaveSubsidyLockCodesForServiceAsync(saveSubsidyLockCodesForServiceRequest);
	}
}
