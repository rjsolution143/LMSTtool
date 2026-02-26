using System;
using System.Collections.Generic;

namespace ISmart;

public interface IWeb
{
	bool TokenConnected { get; }

	bool BrowserSessionActive { get; }

	TokenStatus TokenStatus { get; }

	string TokenIp { get; }

	bool ValidateToken();

	TOKEN_ERROR GenerateDBSRequest(int version, ref byte[] requestMsg, string userID);

	TOKEN_ERROR GenExtendFieldType2(int version, ref byte[] requestMsg, string userID);

	TOKEN_ERROR GenExtendFieldType1(int version, ref byte[] requestMsg, string userID);

	TokenInfo TokenInfo();

	int TokenGetExtendFieldLen(int clientAuthVersion);

	byte[] DbsRequest(string clientIpAddress, string mascId, string requestType, bool productiontype, string oldImei, string newImei, string originalImei, string passwordChangeReq, byte[] signedMessage, string logId, out string ServerId, out int nErrorCode, out string sErrorMessage);

	SortedList<string, string> WarrantyRequest(string serialNumber, bool unlock);

	void WarrantyTransfer(string clientIp, string serialNoIn, string serialNoOut, string serialNoType, string dualSerialNoIn, string dualSerialNoOut, string dualSerialNoType, string triSerialNoIn, string triSerialNoOut, string triSerialNoType, string repairDate, string iccId, string cit, string apc, string transModel, string custModel, string mktModel, string itemCode, string intelControlKey, string swapType, bool validation, bool swapOnly);

	string WebTest(string userId, string password, string serialNumber, string method);

	void UpdUpdate(string serialNumberIn, string serialNumberOut, string serialNumberType, string aKey, string lock1, string lock2, string servicePassword, string repairDate, string macAddress, string hsn, string iccId, string wlanAddress, string wlanAddress2, string wlanAddress3, string wlanAddress4, string wimaxMacAddress, string cit, string apc, string transModel, string custModel, string mktModel, string itemCode, string intelControlKey, string rsuSecretKey);

	SortedList<string, string> GetGppdId(string serialNumber);

	void DeviceInfoUpdate(string recordId, string imei, string imei2, string manufacturingDate, string model, string sn);

	bool ShippableCheck(string imei, string imei2, string trackId);

	void SameSnTransfer(string imeiIn, string imeiOut, string imeiInDual, string imeiOutDual, string trackId);

	void SameSnConfirm(string imei, string imeiDual, string trackId, bool success);

	string GpsRsu(string serialNumber, string trackID);

	void GpsLockCode(string serialNumber, string trackID, string serialNumber2, string nwscp, string sscp, string servicePasscode, string deviceSecretKey, string eSimEid);

	string PcbaSerialNumberRequest(string snType, string customer, string numberOfUlma, string gppdId, string buildType, string protocol, string boardAssembly, string trackId, string miiModel);

	string PcbaSerialNumberRequest(string serialNumber, string snType, string customer, string numberOfUlma, string gppdId, string buildType, string protocol, string boardAssembly, string trackId, string miiModel);

	void PcbaSuccessUpdate(string serialNumber, string snType, string status, string msl, string otksl, string servicePassCode);

	void ServiceSerialNumber(string serialNumberIn, string serialNumberOut, string serialNumberInDual, string serialNumberOutDual, string repairDate, string iccid, string cit, string apc, string transModel, string custModel, string mktModel, string itemCode);

	TokenStatus GetTokenStatus(string eTokenIp);

	void DualConnection(string serialNumber, string serialNumberDual, string trackId);

	void DualConnectionGsn(string serialNumber, string gsn, string trackId);

	string DataSignODM(string newImei, string logId, string clientReqType, string prodId, string keyType, string keyName, string data);

	string KeyDispatchODM(string imei, string logId, string clientReqType, string certModel, string certType);

	KillSwitchData KillSwitchODM(string imei, string logId, string clientReqType, string prodName, string cpuId, string buildType);

	void Rsu(string serialNo, string soCModel, string suid, string receiptData, string sip, string deviceModel, string mnOperator, string trackId);

	void Rpk(string serialNo, string googleCsr, string googleCsr2, string googleCsr3, string trackId);

	SortedList<string, string> TabletWarrantyCheck(string serialNumber);

	string TokenRefresh(string userName);

	string TokenRefresh(string userName, bool allowTrg);

	TimeSpan NistTimeOffset();

	SortedList<string, string> GetMiiCertNo(string serialNumber);

	bool CheckZeroTouch(string serialNumber);

	bool CheckMdsOrder(string imei, string trackId, string originalImei);

	SortedList<string, string> ArgoInfo(string serialNumber);
}
