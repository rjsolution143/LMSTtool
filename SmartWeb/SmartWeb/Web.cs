using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security;
using ISmart;

namespace SmartWeb;

public class Web : IWeb
{
	private object tokenLock = new object();

	private HardwareToken token = new HardwareToken();

	private ITimedCache oauth = Smart.NewTimedCache();

	private WarrantyCheckService warranty = new WarrantyCheckService();

	private SignDataBlockService signDataBlock = new SignDataBlockService();

	private GetGppdIdService getGppdId = new GetGppdIdService();

	private MachineInfoService machineInfo = new MachineInfoService();

	private SameSnShippableService shippable = new SameSnShippableService();

	private SameSnTransferService sameTransfer = new SameSnTransferService();

	private SameSnConfirmService sameConfirm = new SameSnConfirmService();

	private SwapService swap = new SwapService();

	private SwapValidationService swapValidate = new SwapValidationService();

	private SyncService sync = new SyncService();

	private GpsRsuService gpsRsu = new GpsRsuService();

	private GpsLockCodeService gpsLockCode = new GpsLockCodeService();

	private PcbaDispatchService dispatch = new PcbaDispatchService();

	private ServiceSerialNumberService serviceSn = new ServiceSerialNumberService();

	private ETokenValidationService etokenValidation = new ETokenValidationService();

	private DualConnectionService dualConnection = new DualConnectionService();

	private DataSigningODMService dataSignOdm = new DataSigningODMService();

	private KeyDispatchODMService keyDispatchOdm = new KeyDispatchODMService();

	private KillSwitchODMService killSwitchOdm = new KillSwitchODMService();

	private RsuService rsu = new RsuService();

	private RpkService rpk = new RpkService();

	private TabletWarrantyService tabletWarranty = new TabletWarrantyService();

	private TabletWarrantySdeService tabletWarrantySde = new TabletWarrantySdeService();

	private GetMiiCertNoService getMiiCertNo = new GetMiiCertNoService();

	private ZeroTouchService zeroTouch = new ZeroTouchService();

	private MdsOrderService mdsOrder = new MdsOrderService();

	private ArgoService argoInfo = new ArgoService();

	private TokenRefreshService tokenRefresh = new TokenRefreshService();

	private TokenRefreshService tokenRefreshRsd = new TokenRefreshService();

	private string TAG => GetType().FullName;

	public TokenStatus TokenStatus { get; private set; }

	public string TokenIp { get; private set; }

	public bool TokenConnected { get; private set; }

	public TokenInfo TokenDetails { get; private set; }

	public bool BrowserSessionActive { get; private set; }

	public Web()
	{
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Invalid comparison between Unknown and I4
		TokenConnected = false;
		TokenIp = string.Empty;
		TokenStatus = (TokenStatus)0;
		TokenDetails = TokenInfo.BlankInfo;
		BrowserSessionActive = true;
		if ((int)Smart.Rsd.ServerType == 1)
		{
			tokenRefreshRsd = new TokenRefreshService(trg: true);
		}
	}

	public bool ValidateToken()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			TokenInfo info = token.Info;
			if (((object)(TokenInfo)(ref info)).Equals((object?)TokenInfo.BlankInfo))
			{
				Smart.Log.Error(TAG, "Unknown issue validating hardware token");
				TokenStatus = (TokenStatus)2;
				TokenIp = string.Empty;
				TokenConnected = false;
				TokenDetails = TokenInfo.BlankInfo;
				return false;
			}
			TokenDetails = info;
			TokenConnected = true;
			TokenIp = ((TokenInfo)(ref info)).HwDongleIp;
			return true;
		}
		catch (AccessViolationException ex)
		{
			Smart.Log.Error(TAG, "Warning, hardware token is corrupt!: " + ex.Message);
			Smart.Log.Verbose(TAG, ex.ToString());
			TokenStatus = (TokenStatus)4;
			TokenIp = string.Empty;
			TokenDetails = TokenInfo.BlankInfo;
			return false;
		}
		catch (Exception ex2)
		{
			Smart.Log.Error(TAG, "Error validating hardware token: " + ex2.Message);
			Smart.Log.Verbose(TAG, ex2.ToString());
			TokenStatus = (TokenStatus)2;
			TokenIp = string.Empty;
			TokenConnected = false;
			TokenDetails = TokenInfo.BlankInfo;
			return false;
		}
	}

	public TokenInfo TokenInfo()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		TokenInfo result = TokenDetails;
		if (!((object)(TokenInfo)(ref result)).Equals((object?)TokenInfo.BlankInfo))
		{
			return TokenDetails;
		}
		try
		{
			TokenInfo info = token.Info;
			TokenIp = ((TokenInfo)(ref info)).HwDongleIp;
			TokenConnected = true;
			TokenDetails = info;
			result = info;
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Error getting hardware token info: " + ex.Message);
			Smart.Log.Verbose(TAG, ex.ToString());
			TokenStatus = (TokenStatus)2;
			TokenIp = string.Empty;
			TokenDetails = TokenInfo.BlankInfo;
			result = TokenInfo.BlankInfo;
		}
		return result;
	}

	public TOKEN_ERROR GenerateDBSRequest(int version, ref byte[] requestMsg, string userID)
	{
		string ipAddress = string.Empty;
		string location = string.Empty;
		string sourceType = string.Empty;
		string benchId = string.Empty;
		return (TOKEN_ERROR)token.RawToken.GenerateDBSRequest(version, ref requestMsg, userID, out ipAddress, out location, out sourceType, out benchId);
	}

	public TOKEN_ERROR GenExtendFieldType2(int version, ref byte[] requestMsg, string userID)
	{
		string ipAddress = string.Empty;
		string location = string.Empty;
		string sourceType = string.Empty;
		string benchId = string.Empty;
		byte[] tnlRequest = null;
		return (TOKEN_ERROR)token.RawToken.GenExtendFieldType2(version, ref requestMsg, userID, out ipAddress, out location, out sourceType, out benchId, out tnlRequest);
	}

	public TOKEN_ERROR GenExtendFieldType1(int version, ref byte[] requestMsg, string userID)
	{
		string ipAddress = string.Empty;
		string location = string.Empty;
		string sourceType = string.Empty;
		string benchId = string.Empty;
		byte[] tnlRequest = null;
		return (TOKEN_ERROR)token.RawToken.GenExtendFieldType1(version, ref requestMsg, userID, out ipAddress, out location, out sourceType, out benchId, out tnlRequest);
	}

	public int TokenGetExtendFieldLen(int clientAuthVersion)
	{
		return token.RawToken.GetExtendFieldLen(clientAuthVersion);
	}

	public SortedList<string, string> WarrantyRequest(string serialNumber, bool unlock)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		SerialNumberType val = Smart.Convert.ToSerialNumberType(serialNumber);
		string text = ((object)(SerialNumberType)(ref val)).ToString().ToUpperInvariant();
		if (text == "MSN".ToUpperInvariant() && serialNumber.Length == 10)
		{
			text = "TRACK_ID";
		}
		if (text == "UUID".ToUpperInvariant())
		{
			text = "UID";
		}
		WarrantyCheckService.WarrantyCheckInput.WarrantyCheckType checkType = WarrantyCheckService.WarrantyCheckInput.WarrantyCheckType.Normal;
		if (unlock)
		{
			checkType = WarrantyCheckService.WarrantyCheckInput.WarrantyCheckType.Unlock;
		}
		WarrantyCheckService.WarrantyCheckInput input = new WarrantyCheckService.WarrantyCheckInput(serialNumber, text, checkType);
		WarrantyCheckService warrantyCheckService = warranty;
		Login login = Smart.Rsd.Login;
		warrantyCheckService.OAuth = TokenRefresh(((Login)(ref login)).UserName);
		WarrantyCheckService.WarrantyCheckOutput warrantyCheckOutput = warranty.WarrantyCheck(input);
		string value = string.Empty;
		if (warrantyCheckOutput.Fields.TryGetValue("status_code", out value) && value != null && value != string.Empty)
		{
			Smart.Log.Debug(TAG, "WarrantyRequest Success");
			return warrantyCheckOutput.Fields;
		}
		throw new WebException($"SN {serialNumber} not found in warranty database");
	}

	public void WarrantyTransfer(string clientIp, string serialNoIn, string serialNoOut, string serialNoType, string dualSerialNoIn, string dualSerialNoOut, string dualSerialNoType, string triSerialNoIn, string triSerialNoOut, string triSerialNoType, string repairDate, string iccId, string cit, string apc, string transModel, string custModel, string mktModel, string itemCode, string intelControlKey, string swapType, bool validation, bool swapOnly)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		StationDescriptor stationDescriptor = Smart.Rsd.GetStationDescriptor();
		string mascId = ((StationDescriptor)(ref stationDescriptor)).ToId();
		string sourceCode = "MST";
		if (!swapOnly)
		{
			string validation2 = "NO";
			if (validation)
			{
				validation2 = "YES";
			}
			string oAuth = RequestSecuirtyToken((SOATokenType)10);
			swapValidate.OAuth = oAuth;
			SwapValidationService.SwapValidationServiceInput input = new SwapValidationService.SwapValidationServiceInput(mascId, serialNoIn, serialNoOut, serialNoType, dualSerialNoIn, dualSerialNoOut, dualSerialNoType, repairDate, iccId, cit, apc, transModel, custModel, mktModel, itemCode, intelControlKey, sourceCode, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, validation2, string.Empty, string.Empty, swapType);
			SwapValidationService.SwapValidationServiceOutput swapValidationServiceOutput = swapValidate.Swap(input);
			if (swapValidationServiceOutput.ResponseCode != "0000" && swapValidationServiceOutput.ResponseCode != "001")
			{
				throw new WebException($"Invalid response code '{swapValidationServiceOutput.ResponseCode}' from Swap Validation: {swapValidationServiceOutput.ResponseMessage}");
			}
		}
		string oAuth2 = RequestSecuirtyToken((SOATokenType)10);
		swap.OAuth = oAuth2;
		SwapService.SwapServiceInput input2 = new SwapService.SwapServiceInput(mascId, serialNoIn, serialNoOut, serialNoType, dualSerialNoIn, dualSerialNoOut, dualSerialNoType, repairDate, iccId, cit, apc, transModel, custModel, mktModel, itemCode, intelControlKey, sourceCode, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, swapType);
		SwapService.SwapServiceOutput swapServiceOutput = swap.Swap(input2);
		if (swapServiceOutput.StatusCode != "001")
		{
			throw new WebException($"Invalid response code '{swapServiceOutput.StatusCode}' from Swap: {swapServiceOutput.StatusDesc}");
		}
	}

	public byte[] DbsRequest(string clientIpAddress, string mascId, string requestType, bool productiontype, string oldImei, string newImei, string originalImei, string passwordChangeReq, byte[] signedMessage, string logId, out string ServerId, out int nErrorCode, out string sErrorMessage)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		SignDataBlockService signDataBlockService = signDataBlock;
		Login login = Smart.Rsd.Login;
		signDataBlockService.OAuth = TokenRefresh(((Login)(ref login)).UserName);
		StationDescriptor stationDescriptor = Smart.Rsd.GetStationDescriptor();
		mascId = ((StationDescriptor)(ref stationDescriptor)).ToId();
		SignDataBlockService.SignDataBlockOutput signDataBlockOutput = default(SignDataBlockService.SignDataBlockOutput);
		SignDataBlockService.SignDataBlockInput input = new SignDataBlockService.SignDataBlockInput(mascId, logId, clientIpAddress, requestType, newImei, oldImei, originalImei, passwordChangeReq, signedMessage);
		try
		{
			signDataBlockOutput = signDataBlock.SignDataBlock(input);
		}
		catch (Exception ex)
		{
			string[] obj = new string[8] { "(401)", "(601)", "(602)", "(603)", "(604)", "(605)", "(606)", "(607)" };
			string text = string.Empty;
			string[] array = obj;
			foreach (string value in array)
			{
				if (ex.Message.Contains(value))
				{
					text = "Invalid eToken Config-RSD";
					break;
				}
			}
			if (text != string.Empty)
			{
				throw new WebException(text, ex);
			}
			throw;
		}
		ServerId = signDataBlockOutput.TransactionId;
		nErrorCode = -1;
		int.TryParse(signDataBlockOutput.StatusCode, out nErrorCode);
		if (nErrorCode == 8022 || nErrorCode == 8049)
		{
			nErrorCode = 0;
		}
		sErrorMessage = signDataBlockOutput.StatusData;
		if (nErrorCode == 9999)
		{
			throw new WebException("Invalid eToken config-IQS");
		}
		return signDataBlockOutput.PkiResponse;
	}

	public void UpdUpdate(string serialNumberIn, string serialNumberOut, string serialNumberType, string aKey, string lock1, string lock2, string servicePassword, string repairDate, string macAddress, string hsn, string iccId, string wlanAddress, string wlanAddress2, string wlanAddress3, string wlanAddress4, string wimaxMacAddress, string cit, string apc, string transModel, string custModel, string mktModel, string itemCode, string intelControlKey, string rsuSecretKey)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		string oAuth = RequestSecuirtyToken((SOATokenType)10);
		sync.OAuth = oAuth;
		Login login = Smart.Rsd.Login;
		string userName = ((Login)(ref login)).UserName;
		SyncService.SyncServiceInput input = new SyncService.SyncServiceInput(serialNumberIn, serialNumberType, string.Empty, cit, userName, lock1, string.Empty, string.Empty, string.Empty, string.Empty, macAddress, aKey, string.Empty, servicePassword, string.Empty, string.Empty, string.Empty, lock2, rsuSecretKey);
		SyncService.SyncServiceOutput syncServiceOutput = sync.Sync(input);
		if (syncServiceOutput.StatusCode != "001")
		{
			throw new WebException($"Invalid response code '{syncServiceOutput.StatusCode}' from Swap: {syncServiceOutput.StatusDesc}");
		}
	}

	public SortedList<string, string> GetGppdId(string serialNumber)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		GetGppdIdService getGppdIdService = getGppdId;
		Login login = Smart.Rsd.Login;
		getGppdIdService.OAuth = TokenRefresh(((Login)(ref login)).UserName);
		GetGppdIdService.GetGppdIdOutput gppdId = getGppdId.GetGppdId(serialNumber);
		if (gppdId.ResponseCode != "0000")
		{
			throw new WebException($"Invalid response code '{gppdId.ResponseCode}' from Get GPPD ID: {gppdId.ResponseMessage}");
		}
		if (gppdId.GppdId != null && gppdId.GppdId.ToLowerInvariant().Contains("not found"))
		{
			throw new WebException($"Error from Get GPPD ID, {serialNumber} not found in database");
		}
		return new SortedList<string, string>
		{
			["SerialNo"] = gppdId.SerialNo,
			["Customer"] = gppdId.Customer,
			["GppdId"] = gppdId.GppdId,
			["Protocol"] = gppdId.Protocol
		};
	}

	private string RequestSecuirtyToken(SOATokenType type)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		string text = $"Token-{type}";
		string empty = string.Empty;
		if (((IDictionary<string, string>)oauth).ContainsKey(text))
		{
			empty = ((IDictionary<string, string>)oauth)[text];
		}
		else
		{
			string empty2 = string.Empty;
			empty = Smart.Rsd.GetSOAToken(type, ref empty2);
			int num = int.Parse(empty2);
			num -= 30;
			if (num <= 0)
			{
				num = 0;
			}
			else
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds(num);
				oauth.Add(text, empty, timeSpan);
			}
		}
		return empty;
	}

	private string RequestSdeToken()
	{
		string text = "SDE-LMST";
		string empty = string.Empty;
		if (((IDictionary<string, string>)oauth).ContainsKey(text))
		{
			return ((IDictionary<string, string>)oauth)[text];
		}
		string key = "IMdS7VbOz88Cucwg4duMFjVF7LMc";
		string secret = "bWJdJ3Yfi3WUd1mH4RDvWrsYnVBY";
		OAuthTokenService.OAuthTokenOutput oAuthTokenOutput = new OAuthTokenService("https://microapi-cn-t.lenovo.com/token", key, secret).GetToken();
		empty = oAuthTokenOutput.AccessToken;
		int expiresIn = oAuthTokenOutput.ExpiresIn;
		expiresIn -= 30;
		if (expiresIn <= 0)
		{
			expiresIn = 0;
		}
		else
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(expiresIn);
			oauth.Add(text, empty, timeSpan);
		}
		return empty;
	}

	public void DeviceInfoUpdate(string recordId, string imei, string imei2, string manufacturingDate, string model, string sn)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		string oAuth = RequestSecuirtyToken((SOATokenType)10);
		machineInfo.OAuth = oAuth;
		string text = "US";
		StationDescriptor stationDescriptor = Smart.Rsd.GetStationDescriptor();
		string shopId = ((StationDescriptor)(ref stationDescriptor)).ShopId;
		if (shopId.Length > 2 && !shopId.Substring(0, 2).All(char.IsNumber))
		{
			text = shopId.Substring(0, 2);
		}
		string countryCode = text;
		Login login = Smart.Rsd.Login;
		MachineInfoService.MachineInfoInput input = new MachineInfoService.MachineInfoInput(recordId, imei, imei2, countryCode, manufacturingDate, model, ((Login)(ref login)).UserName, sn);
		MachineInfoService.MachineInfoOutput machineInfoOutput = machineInfo.SendInfo(input);
		if (machineInfoOutput.StatusCode != "001" || machineInfoOutput.Status.ToLowerInvariant() != "success")
		{
			throw new WebException($"Invalid response code '{machineInfoOutput.StatusCode}' from Machine Info: {machineInfoOutput.Status} - {machineInfoOutput.StatusDescription}");
		}
	}

	public bool ShippableCheck(string imei, string imei2, string trackId)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		string oAuth = RequestSecuirtyToken((SOATokenType)10);
		shippable.OAuth = oAuth;
		Login login = Smart.Rsd.Login;
		SameSnShippableService.SameSnShippableInput input = new SameSnShippableService.SameSnShippableInput(imei, imei2, trackId, ((Login)(ref login)).UserName);
		SameSnShippableService.SameSnShippableOutput sameSnShippableOutput = shippable.Request(input);
		if (sameSnShippableOutput.ResponseCode == "09" || sameSnShippableOutput.ResponseCode == "10")
		{
			return true;
		}
		if (sameSnShippableOutput.ResponseCode == "07")
		{
			return false;
		}
		throw new WebException($"Invalid response code '{sameSnShippableOutput.ResponseCode}' from Shippable Check: {sameSnShippableOutput.ResponseMessage}");
	}

	public void SameSnTransfer(string imeiIn, string imeiOut, string imeiInDual, string imeiOutDual, string trackId)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		SameSnTransferService sameSnTransferService = sameTransfer;
		Login login = Smart.Rsd.Login;
		sameSnTransferService.OAuth = TokenRefresh(((Login)(ref login)).UserName);
		StationDescriptor stationDescriptor = Smart.Rsd.GetStationDescriptor();
		string mascId = ((StationDescriptor)(ref stationDescriptor)).ToId();
		string programDate = DateTime.Now.ToString("yyyy-MM-dd");
		login = Smart.Rsd.Login;
		SameSnTransferService.SameSnTransferInput input = new SameSnTransferService.SameSnTransferInput(imeiIn, imeiOut, imeiInDual, imeiOutDual, trackId, programDate, ((Login)(ref login)).UserName, mascId);
		SameSnTransferService.SameSnTransferOutput sameSnTransferOutput = sameTransfer.Request(input);
		if (sameSnTransferOutput.ResponseCode != "20")
		{
			throw new WebException($"Invalid response code '{sameSnTransferOutput.ResponseCode}' from Same SN Transfer: {sameSnTransferOutput.ResponseMessage}");
		}
	}

	public void SameSnConfirm(string imei, string imeiDual, string trackId, bool success)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		string oAuth = RequestSecuirtyToken((SOATokenType)10);
		sameConfirm.OAuth = oAuth;
		string text = "FAILURE";
		if (success)
		{
			text = "SUCCESS";
		}
		string programDate = DateTime.Now.ToString("yyyy-MM-dd");
		string programStatus = text;
		Login login = Smart.Rsd.Login;
		SameSnConfirmService.SameSnConfirmInput input = new SameSnConfirmService.SameSnConfirmInput(imei, imeiDual, trackId, programStatus, programDate, ((Login)(ref login)).UserName);
		SameSnConfirmService.SameSnConfirmOutput sameSnConfirmOutput = sameConfirm.Request(input);
		if (sameSnConfirmOutput.ResponseCode != "10")
		{
			throw new WebException($"Invalid response code '{sameSnConfirmOutput.ResponseCode}' from Same SN Confirm: {sameSnConfirmOutput.ResponseMessage}");
		}
	}

	public string GpsRsu(string serialNumber, string trackID)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		GpsRsuService gpsRsuService = gpsRsu;
		Login login = Smart.Rsd.Login;
		gpsRsuService.OAuth = TokenRefresh(((Login)(ref login)).UserName);
		GpsRsuService.GpsRsuInput input = new GpsRsuService.GpsRsuInput(serialNumber, trackID);
		GpsRsuService.GpsRsuOutput gpsRsuOutput = gpsRsu.GpsRsu(input);
		if (gpsRsuOutput.ErrorCode != "0" && gpsRsuOutput.ErrorCode != "0")
		{
			throw new WebException($"Invalid response code '{gpsRsuOutput.ErrorCode}' from GPS RSU Service: {gpsRsuOutput.ErrorMessage}");
		}
		return gpsRsuOutput.VzwKd;
	}

	public void GpsLockCode(string serialNumber, string trackID, string serialNumber2, string nwscp, string sscp, string servicePasscode, string deviceSecretKey, string eSimEid)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		GpsLockCodeService gpsLockCodeService = gpsLockCode;
		Login login = Smart.Rsd.Login;
		gpsLockCodeService.OAuth = TokenRefresh(((Login)(ref login)).UserName);
		GpsLockCodeService.GpsLockCodeInput input = new GpsLockCodeService.GpsLockCodeInput(serialNumber, trackID, serialNumber2, nwscp, sscp, servicePasscode, deviceSecretKey, eSimEid);
		GpsLockCodeService.GpsLockCodeOutput gpsLockCodeOutput = gpsLockCode.GpsLockCode(input);
		if (gpsLockCodeOutput.ErrorCode != "0" && gpsLockCodeOutput.ErrorCode != "0")
		{
			throw new WebException($"Invalid response code '{gpsLockCodeOutput.ErrorCode}' from GPS Lock Code Service: {gpsLockCodeOutput.ErrorMessage}");
		}
	}

	public string PcbaSerialNumberRequest(string snType, string customer, string numberOfUlma, string gppdId, string buildType, string protocol, string boardAssembly, string trackId, string miiModel)
	{
		return PcbaSerialNumberRequest(null, snType, customer, numberOfUlma, gppdId, buildType, protocol, boardAssembly, trackId, miiModel);
	}

	public string PcbaSerialNumberRequest(string serialNumber, string snType, string customer, string numberOfUlma, string gppdId, string buildType, string protocol, string boardAssembly, string trackId, string miiModel)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		PcbaDispatchService pcbaDispatchService = dispatch;
		Login login = Smart.Rsd.Login;
		pcbaDispatchService.OAuth = TokenRefresh(((Login)(ref login)).UserName);
		StationDescriptor stationDescriptor = Smart.Rsd.GetStationDescriptor();
		string mascId = ((StationDescriptor)(ref stationDescriptor)).ToId();
		if (trackId.Trim() == string.Empty)
		{
			Smart.Log.Debug(TAG, "No Track ID read from device, using dummy value");
			trackId = "11111111";
		}
		string apc = string.Empty;
		if (snType.ToLowerInvariant() == "MSN".ToLowerInvariant())
		{
			apc = gppdId;
			gppdId = string.Empty;
		}
		string gppdId2 = gppdId;
		login = Smart.Rsd.Login;
		PcbaDispatchService.PcbaDispatchInput input = new PcbaDispatchService.PcbaDispatchInput(serialNumber, "D", customer, snType, numberOfUlma, gppdId2, mascId, ((Login)(ref login)).UserName, buildType, protocol, trackId, boardAssembly, apc, miiModel);
		PcbaDispatchService.PcbaDispatchOutput pcbaDispatchOutput = dispatch.PcbaDispatch(input);
		if (pcbaDispatchOutput.ResponseCode != "0000")
		{
			throw new WebException($"Invalid response code '{pcbaDispatchOutput.ResponseCode}' from PCBA Dispatch: {pcbaDispatchOutput.ResponseMsg}");
		}
		return pcbaDispatchOutput.NewSerialNo;
	}

	public void PcbaSuccessUpdate(string serialNumber, string snType, string status, string msl, string otksl, string servicePassCode)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		string oAuth = RequestSecuirtyToken((SOATokenType)10);
		sync.OAuth = oAuth;
		StationDescriptor stationDescriptor = Smart.Rsd.GetStationDescriptor();
		((StationDescriptor)(ref stationDescriptor)).ToId();
		Smart.Log.Info(TAG, "Not reporting status to web service: " + status);
		string empty = string.Empty;
		string empty2 = string.Empty;
		Login login = Smart.Rsd.Login;
		SyncService.SyncServiceInput input = new SyncService.SyncServiceInput(serialNumber, snType, empty, empty2, ((Login)(ref login)).UserName, msl, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, servicePassCode, string.Empty, string.Empty, string.Empty, otksl, string.Empty);
		SyncService.SyncServiceOutput syncServiceOutput = sync.Sync(input);
		if (syncServiceOutput.StatusCode != "001")
		{
			throw new WebException($"Invalid response code '{syncServiceOutput.StatusCode}' from PCBA Success: {syncServiceOutput.StatusDesc}");
		}
	}

	public void ServiceSerialNumber(string serialNumberIn, string serialNumberOut, string serialNumberInDual, string serialNumberOutDual, string repairDate, string iccid, string cit, string apc, string transModel, string custModel, string mktModel, string itemCode)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		string oAuth = RequestSecuirtyToken((SOATokenType)10);
		serviceSn.OAuth = oAuth;
		StationDescriptor stationDescriptor = Smart.Rsd.GetStationDescriptor();
		string mascId = ((StationDescriptor)(ref stationDescriptor)).ToId();
		SerialNumberType val = Smart.Convert.ToSerialNumberType(serialNumberIn);
		string serialNumberType = ((object)(SerialNumberType)(ref val)).ToString().ToUpperInvariant();
		string serialNumberTypeDual = string.Empty;
		if (serialNumberInDual != null && serialNumberInDual.Trim() != string.Empty && serialNumberInDual != "UNKNOWN")
		{
			val = Smart.Convert.ToSerialNumberType(serialNumberInDual);
			serialNumberTypeDual = ((object)(SerialNumberType)(ref val)).ToString().ToUpperInvariant();
		}
		ServiceSerialNumberService.ServiceSerialNumberInput input = new ServiceSerialNumberService.ServiceSerialNumberInput(mascId, serialNumberIn, serialNumberOut, serialNumberType, serialNumberInDual, serialNumberOutDual, serialNumberTypeDual, repairDate, iccid, cit, apc, transModel, custModel, mktModel, itemCode);
		ServiceSerialNumberService.ServiceSerialNumberOutput serviceSerialNumberOutput = serviceSn.Request(input);
		if (serviceSerialNumberOutput.ResponseCode != "00")
		{
			throw new WebException($"Invalid response code '{serviceSerialNumberOutput.ResponseCode}' from Service SN WS: {serviceSerialNumberOutput.ResponseMessage}");
		}
	}

	public TokenStatus GetTokenStatus(string eTokenIp)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		ETokenValidationService eTokenValidationService = etokenValidation;
		Login login = Smart.Rsd.Login;
		eTokenValidationService.OAuth = TokenRefresh(((Login)(ref login)).UserName);
		ETokenValidationService.ETokenValidationOutput tokenStatus = etokenValidation.GetTokenStatus(eTokenIp);
		string text = tokenStatus.ResponseResult.ToLowerInvariant();
		if (text != "success")
		{
			string responseMessage = tokenStatus.ResponseMessage;
			throw new WebException($"Validation for {eTokenIp} {text} with message {responseMessage}");
		}
		string status = tokenStatus.Status;
		TokenStatus val = (TokenStatus)0;
		return TokenStatus = ((!(status.ToLowerInvariant() == "active")) ? ((TokenStatus)3) : ((TokenStatus)1));
	}

	public void DualConnectionGsn(string serialNumber, string gsn, string trackId)
	{
		DualConnectionRequest(serialNumber, gsn, trackId, useGsn: true);
	}

	public void DualConnection(string serialNumber, string serialNumberDual, string trackId)
	{
		DualConnectionRequest(serialNumber, serialNumberDual, trackId, useGsn: false);
	}

	private void DualConnectionRequest(string serialNumber, string serialNumberDual, string trackId, bool useGsn)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		string oAuth = RequestSecuirtyToken((SOATokenType)10);
		dualConnection.OAuth = oAuth;
		StationDescriptor stationDescriptor = Smart.Rsd.GetStationDescriptor();
		string mascId = ((StationDescriptor)(ref stationDescriptor)).ToId();
		Login login = Smart.Rsd.Login;
		string userName = ((Login)(ref login)).UserName;
		string gsn = string.Empty;
		if (useGsn)
		{
			gsn = serialNumberDual;
			serialNumberDual = string.Empty;
		}
		DualConnectionService.DualConnectionInput input = new DualConnectionService.DualConnectionInput(serialNumber, serialNumberDual, gsn, trackId, mascId, userName, DateTime.Now);
		DualConnectionService.DualConnectionOutput dualConnectionOutput = dualConnection.Connect(input);
		if (dualConnectionOutput.ResponseCode != "0000")
		{
			throw new WebException($"Invalid response code '{dualConnectionOutput.ResponseCode}' from Dual Connection service: {dualConnectionOutput.ResponseMessage}");
		}
	}

	public string DataSignODM(string newImei, string logId, string clientReqType, string prodId, string keyType, string keyName, string data)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		DataSigningODMService dataSigningODMService = dataSignOdm;
		Login login = Smart.Rsd.Login;
		dataSigningODMService.OAuth = TokenRefresh(((Login)(ref login)).UserName);
		StationDescriptor stationDescriptor = Smart.Rsd.GetStationDescriptor();
		string mascId = ((StationDescriptor)(ref stationDescriptor)).ToId();
		login = Smart.Rsd.Login;
		_ = ((Login)(ref login)).UserName;
		TokenInfo val = TokenInfo();
		string hwDongleIp = ((TokenInfo)(ref val)).HwDongleIp;
		DataSigningODMService.DataSigningODMInput input = new DataSigningODMService.DataSigningODMInput(newImei, mascId, hwDongleIp, clientReqType, logId, prodId, keyType, keyName, data);
		DataSigningODMService.DataSigningODMOutput dataSigningODMOutput = dataSignOdm.SignData(input);
		if (dataSigningODMOutput.ResponseCode != "0")
		{
			throw new WebException($"Invalid response code '{dataSigningODMOutput.ResponseCode}' from ODM Data Signing service: {dataSigningODMOutput.ResponseMessage}");
		}
		return dataSigningODMOutput.ReturnedData;
	}

	public string KeyDispatchODM(string imei, string logId, string clientReqType, string certModel, string certType)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		KeyDispatchODMService keyDispatchODMService = keyDispatchOdm;
		Login login = Smart.Rsd.Login;
		keyDispatchODMService.OAuth = TokenRefresh(((Login)(ref login)).UserName);
		StationDescriptor stationDescriptor = Smart.Rsd.GetStationDescriptor();
		string mascId = ((StationDescriptor)(ref stationDescriptor)).ToId();
		login = Smart.Rsd.Login;
		_ = ((Login)(ref login)).UserName;
		TokenInfo val = TokenInfo();
		string hwDongleIp = ((TokenInfo)(ref val)).HwDongleIp;
		KeyDispatchODMService.KeyDispatchODMInput input = new KeyDispatchODMService.KeyDispatchODMInput(imei, mascId, hwDongleIp, clientReqType, logId, certModel, certType);
		KeyDispatchODMService.KeyDispatchODMOutput keyDispatchODMOutput = keyDispatchOdm.Dispatch(input);
		if (keyDispatchODMOutput.ResponseCode != "0")
		{
			throw new WebException($"Invalid response code '{keyDispatchODMOutput.ResponseCode}' from ODM Key Dispatch service: {keyDispatchODMOutput.ResponseMessage}");
		}
		return keyDispatchODMOutput.ReturnedData;
	}

	public KillSwitchData KillSwitchODM(string imei, string logId, string clientReqType, string prodName, string cpuId, string buildType)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		KillSwitchODMService killSwitchODMService = killSwitchOdm;
		Login login = Smart.Rsd.Login;
		killSwitchODMService.OAuth = TokenRefresh(((Login)(ref login)).UserName);
		StationDescriptor stationDescriptor = Smart.Rsd.GetStationDescriptor();
		string mascId = ((StationDescriptor)(ref stationDescriptor)).ToId();
		login = Smart.Rsd.Login;
		_ = ((Login)(ref login)).UserName;
		TokenInfo val = TokenInfo();
		string hwDongleIp = ((TokenInfo)(ref val)).HwDongleIp;
		KillSwitchODMService.KillSwitchODMInput input = new KillSwitchODMService.KillSwitchODMInput(imei, mascId, hwDongleIp, clientReqType, logId, prodName, cpuId, buildType);
		KillSwitchODMService.KillSwitchODMOutput killSwitchODMOutput = killSwitchOdm.Request(input);
		if (killSwitchODMOutput.ResponseCode != "0")
		{
			throw new WebException($"Invalid response code '{killSwitchODMOutput.ResponseCode}' from ODM KillSwitch service: {killSwitchODMOutput.ResponseMessage}");
		}
		return new KillSwitchData(killSwitchODMOutput.ReturnedData, killSwitchODMOutput.Password);
	}

	public void Rsu(string serialNo, string soCModel, string suid, string receiptData, string sip, string deviceModel, string mnOperator, string trackId)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		RsuService rsuService = rsu;
		Login login = Smart.Rsd.Login;
		rsuService.OAuth = TokenRefresh(((Login)(ref login)).UserName);
		StationDescriptor stationDescriptor = Smart.Rsd.GetStationDescriptor();
		string mascId = ((StationDescriptor)(ref stationDescriptor)).ToId();
		if (trackId.Trim() == string.Empty)
		{
			Smart.Log.Debug(TAG, "No Track ID read from device, using dummy value");
			trackId = "11111111";
		}
		login = Smart.Rsd.Login;
		RsuService.RsuInput input = new RsuService.RsuInput(serialNo, ((Login)(ref login)).UserName, mascId, soCModel, suid, receiptData, sip, deviceModel, mnOperator, trackId);
		RsuService.RsuOutput rsuOutput = rsu.RsuRequest(input);
		if (rsuOutput.ResponseCode != "0000" && rsuOutput.ResponseCode != "0")
		{
			throw new WebException($"Invalid response code '{rsuOutput.ResponseCode}' from RSU Service: {rsuOutput.ResponseMsg}");
		}
	}

	public void Rpk(string serialNo, string googleCsr, string googleCsr2, string googleCsr3, string trackId)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		RpkService rpkService = rpk;
		Login login = Smart.Rsd.Login;
		rpkService.OAuth = TokenRefresh(((Login)(ref login)).UserName);
		StationDescriptor stationDescriptor = Smart.Rsd.GetStationDescriptor();
		string mascId = ((StationDescriptor)(ref stationDescriptor)).ToId();
		if (trackId.Trim() == string.Empty)
		{
			Smart.Log.Debug(TAG, "No Track ID read from device, using dummy value");
			trackId = "11111111";
		}
		login = Smart.Rsd.Login;
		RpkService.RpkInput input = new RpkService.RpkInput(serialNo, ((Login)(ref login)).UserName, mascId, googleCsr, googleCsr2, googleCsr3, trackId);
		RpkService.RpkOutput rpkOutput = rpk.RsuRequest(input);
		if (rpkOutput.ResponseCode != "0000" && rpkOutput.ResponseCode != "0")
		{
			throw new WebException($"Invalid response code '{rpkOutput.ResponseCode}' from RPK Service: {rpkOutput.ResponseMsg}");
		}
	}

	public SortedList<string, string> TabletWarrantyCheck(string serialNumber)
	{
		TabletWarrantyService.TabletWarrantyInput input = new TabletWarrantyService.TabletWarrantyInput(serialNumber);
		SortedList<string, string> fields = tabletWarranty.WarrantyCheck(input).Fields;
		if (!fields.ContainsKey("xmlMessage"))
		{
			Smart.Log.Error(TAG, "Invalid response from Tablet Warranty Check");
		}
		else
		{
			string text = fields["xmlMessage"];
			if (text != string.Empty)
			{
				Smart.Log.Error(TAG, $"Tablet Warranty Check failed with '{text}' response message");
			}
		}
		return fields;
	}

	public SortedList<string, string> TabletWarrantyCheckNew(string serialNumber)
	{
		string oAuth = RequestSdeToken();
		tabletWarrantySde.OAuth = oAuth;
		TabletWarrantySdeService.TabletWarrantyInput input = new TabletWarrantySdeService.TabletWarrantyInput(serialNumber);
		SortedList<string, string> fields = tabletWarrantySde.WarrantyCheck(input).Fields;
		if (fields.ContainsKey("errorCode") || fields.ContainsKey("errorDesc"))
		{
			string arg = "-1";
			string arg2 = "UNKNOWN error";
			if (fields.ContainsKey("errorCode"))
			{
				arg = fields["errorCode"];
			}
			if (fields.ContainsKey("errorDesc"))
			{
				arg2 = fields["errorDesc"];
			}
			Smart.Log.Error(TAG, $"Invalid response from Tablet SDE Warranty Check ({arg}): {arg2}");
		}
		return fields;
	}

	public SortedList<string, string> GetMiiCertNo(string serialNumber)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		GetMiiCertNoService getMiiCertNoService = getMiiCertNo;
		Login login = Smart.Rsd.Login;
		getMiiCertNoService.OAuth = TokenRefresh(((Login)(ref login)).UserName);
		GetMiiCertNoService.GetMiiCertNoOutput miiCertNo = getMiiCertNo.GetMiiCertNo(serialNumber);
		if (miiCertNo.ResponseCode != "0000")
		{
			throw new WebException($"Invalid response code '{miiCertNo.ResponseCode}' from GetMiiCertNo: {miiCertNo.ResponseMessage}");
		}
		return new SortedList<string, string>
		{
			["serialNo"] = miiCertNo.SerialNo,
			["miiCertNo"] = miiCertNo.MiiCertNo,
			["scramblingCode"] = miiCertNo.ScramblingCode,
			["miiModel"] = miiCertNo.MiiModel
		};
	}

	public bool CheckZeroTouch(string serialNumber)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		ZeroTouchService zeroTouchService = zeroTouch;
		Login login = Smart.Rsd.Login;
		zeroTouchService.OAuth = TokenRefresh(((Login)(ref login)).UserName);
		ZeroTouchService.ZeroTouchOutput zeroTouchOutput = zeroTouch.CheckZeroTouch(serialNumber);
		if (zeroTouchOutput.ResponseCode == "7000")
		{
			Smart.Log.Debug(TAG, $"Found ZeroTouch record with status {zeroTouchOutput.Flag}");
			return zeroTouchOutput.Flag;
		}
		if (zeroTouchOutput.ResponseCode == "7009")
		{
			Smart.Log.Debug(TAG, "ZeroTouch status not found");
			return false;
		}
		throw new WebException($"Invalid response code '{zeroTouchOutput.ResponseCode}' from ZeroTouch: {zeroTouchOutput.ResponseMessage}");
	}

	public bool CheckMdsOrder(string imei, string trackId, string originalImei)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		MdsOrderService mdsOrderService = mdsOrder;
		Login login = Smart.Rsd.Login;
		mdsOrderService.OAuth = TokenRefresh(((Login)(ref login)).UserName);
		login = Smart.Rsd.Login;
		string userName = ((Login)(ref login)).UserName;
		MdsOrderService.MdsOrderInput input = new MdsOrderService.MdsOrderInput(imei, trackId, userName, originalImei);
		MdsOrderService.MdsOrderOutput mdsOrderOutput = mdsOrder.GetMdsOrder(input);
		string text = mdsOrderOutput.ResponseCode.ToLowerInvariant().Trim();
		if (text != "success" && ((text != "failure") & (text != "failed")))
		{
			throw new WebException($"Invalid Response Code '{mdsOrderOutput.ResponseCode}' From MdsOrderService: {mdsOrderOutput.ResponseMessage}");
		}
		string arg = "is";
		if (!mdsOrderOutput.Required)
		{
			arg = "is not";
		}
		Smart.Log.Debug(TAG, $"MDS Status: Customer IMEI {arg} required, {mdsOrderOutput.ResponseCode} due to {mdsOrderOutput.ResponseMessage}");
		if (mdsOrderOutput.Required)
		{
			return text == "success";
		}
		return true;
	}

	public SortedList<string, string> ArgoInfo(string serialNumber)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		ArgoService argoService = argoInfo;
		Login login = Smart.Rsd.Login;
		argoService.OAuth = TokenRefresh(((Login)(ref login)).UserName);
		ArgoService.ArgoOutput argoOutput = argoInfo.GetArgoInfo(serialNumber);
		if (argoOutput.ResponseCode != "7000")
		{
			throw new WebException($"Invalid response code '{argoOutput.ResponseCode}' from ArgoInfo: {argoOutput.ResponseMessage} - {argoOutput.Error}");
		}
		return new SortedList<string, string>
		{
			["Imei"] = argoOutput.Imei,
			["EnterpriseEdition"] = argoOutput.EnterpriseEdition,
			["ChannelId"] = argoOutput.ChannelId
		};
	}

	public string TokenRefresh(string userName)
	{
		return TokenRefresh(userName, allowTrg: false);
	}

	public string TokenRefresh(string userName, bool allowTrg)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		TokenRefreshService tokenRefreshService = tokenRefresh;
		string text = "LoginToken";
		if (allowTrg)
		{
			tokenRefreshService = tokenRefreshRsd;
			text = "LoginTokenRSD";
		}
		string empty = string.Empty;
		if (((IDictionary<string, string>)oauth).ContainsKey(text))
		{
			Smart.Log.Verbose(TAG, "Using cached RSD Token");
			return ((IDictionary<string, string>)oauth)[text];
		}
		StationDescriptor stationDescriptor = Smart.Rsd.GetStationDescriptor();
		string text2 = ((StationDescriptor)(ref stationDescriptor)).StationId;
		string rSDStationID = Smart.Security.RSDStationID;
		Smart.Log.Debug(TAG, "RSD Weekly Code: " + Smart.Security.RSDWeekly);
		if (rSDStationID != null && rSDStationID != string.Empty && rSDStationID != text2)
		{
			Smart.Log.Debug(TAG, $"Station ID changing from {text2} to {rSDStationID}");
			text2 = rSDStationID;
		}
		string rSDUniqueID = Smart.Security.RSDUniqueID;
		Smart.Log.Verbose(TAG, "Requesting new RSD Token");
		TokenRefreshService.TokenRefreshInput input = new TokenRefreshService.TokenRefreshInput(userName, text2, rSDUniqueID);
		TokenRefreshService.TokenRefreshOutput tokenRefreshOutput = tokenRefreshService.RefreshToken(input);
		if (tokenRefreshOutput.StationID != null && tokenRefreshOutput.StationID != string.Empty)
		{
			Smart.Security.RSDStationID = tokenRefreshOutput.StationID;
		}
		if (tokenRefreshOutput.Status != "success" || tokenRefreshOutput.Token == null)
		{
			if (tokenRefreshOutput.Message != null && tokenRefreshOutput.Message.ToLowerInvariant().Contains("failed signature validation"))
			{
				throw new SecurityException("Station rejected");
			}
			if (tokenRefreshOutput.Status == "nosession")
			{
				BrowserSessionActive = false;
				throw new ProtocolViolationException("Please login via local web browser");
			}
			if (tokenRefreshOutput.Message != null && tokenRefreshOutput.Message.ToLowerInvariant().Contains("registered successfully"))
			{
				throw new ArgumentException("Station ID Registered, waiting for approval", "StationID");
			}
			throw new WebException($"Invalid status value '{tokenRefreshOutput.Status}' from Token Refresh Service: {tokenRefreshOutput.Message}");
		}
		empty = tokenRefreshOutput.Token;
		int expiresIn = tokenRefreshOutput.ExpiresIn;
		expiresIn -= 30;
		if (expiresIn <= 0)
		{
			BrowserSessionActive = false;
			throw new ProtocolViolationException("Session has expired. Please login via local web browser");
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(expiresIn);
		oauth.Add(text, empty, timeSpan);
		BrowserSessionActive = true;
		return empty;
	}

	public TimeSpan NistTimeOffset()
	{
		string[] obj = new string[9] { "https://www.motorola.com", "https://www.lenovo.com", "https://www.google-analytics.com", "https://www.bing.com", "https://svckm.lenovo.com", "https://osd.lenovo.com/", "https://soa.lenovo.com", "https://www.appspot.com", "https://www.thinkpad.com" };
		List<TimeSpan> list = new List<TimeSpan>();
		string[] array = obj;
		foreach (string url in array)
		{
			try
			{
				TimeSpan offset = GetOffset(url);
				list.Add(offset);
			}
			catch (Exception)
			{
			}
		}
		list.Sort();
		if (list.Count < 3)
		{
			throw new NotSupportedException("Not enough time servers available for time check");
		}
		return list[list.Count / 2];
	}

	private TimeSpan GetOffset(string url)
	{
		WebRequest webRequest = WebRequest.Create(url);
		webRequest.Timeout = 10000;
		webRequest.Method = "HEAD";
		DateTime now = DateTime.Now;
		using WebResponse webResponse = webRequest.GetResponse();
		DateTime now2 = DateTime.Now;
		TimeSpan timeSpan = now2.Subtract(now);
		if (!webResponse.Headers.AllKeys.Contains("Date"))
		{
			throw new NotSupportedException("No date found in HTTP response from " + url);
		}
		DateTime dateTime = DateTime.Parse(webResponse.Headers["Date"], CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.None);
		timeSpan = new TimeSpan(timeSpan.Ticks / 2 + 200);
		dateTime = dateTime.Add(timeSpan);
		return now2.Subtract(dateTime);
	}

	public string WebTest(string userId, string password, string serialNumber, string method)
	{
		TabletWarrantyCheckNew("FAKETEST");
		return "test";
	}
}
