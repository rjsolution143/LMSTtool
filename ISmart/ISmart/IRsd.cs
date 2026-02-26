using System.Collections.Generic;

namespace ISmart;

public interface IRsd
{
	RsdServerType ServerType { get; set; }

	Login Login { get; set; }

	bool FirstLogin { get; set; }

	bool NewVersion { get; set; }

	void DoLogin(string userName, string password, bool offLine);

	void Update(ProcessUpdate callback);

	string DetectModel(DetectionKey detectionKey, IDevice device);

	string DetectModel(string model, string carrier, IDevice device);

	string DetectModel(string serialNumber, bool isTrackId, ref string carrier, ref string model, out string iBaseImei, out string saleModel, out string svnkit, out string iBaseImei2, out string iBaseTrackId, out string iBaseModel, ref string detectionStatus, IDevice device, out bool inSupTableau);

	string LoadRecipe(UseCase useCase, ref string modelId, IDevice device = null);

	string GetFilePathName(string fileName, UseCase useCase, IDevice device);

	string GetFilePathName(string name, UseCase useCase, IDevice device, Progress callback, out string cachedValue);

	string GetValue(string name, UseCase useCase, IDevice device, out bool foundValue, bool latest = false);

	SortedList<string, string> GetValuePKIKeysByModel(string deviceModel);

	string ComPortId(string comPortType, int timeout);

	List<string> ComPortIdList(string comPortType);

	void UpdateUpgradeMatrices();

	List<UseCase> AllowedUseCases();

	List<UseCase> UseCaseLocked();

	List<string> GetCarriers();

	List<string> GetPhoneModelsFromCarrier(string carrier);

	List<string> GetPhoneModels();

	List<string> GetCarriersFromPhoneModel(string model);

	List<string> GetCarriersContainPhoneModel(string model);

	List<string> FilterPhoneModels(string ModelID, List<string> PhoneModes);

	List<string> FilterPhoneCarriers(string ModelID, List<string> PhoneCarriers);

	string GetModelImage(ref string modelId);

	List<string> GetFlashFileNames(UseCase useCase, ref string modelId);

	List<UseCase> GetUseCasesFromCarrierPhoneModel(string carrier, string phoneModel);

	List<UseCase> GetGroupUseCasesFromCarrierPhoneModel(string carrier, string model, UseCase groupUseCase, bool includeGlobalUseCases);

	List<UseCase> GetUseCasesFromModelId(string modelId);

	List<string> GetUseCasesInAllMatrixBySKU(string SKU);

	List<string> GetStepsInAllMatrixBySKU(string SKU);

	List<UseCase> GetGroupUseCasesFromModelId(string modelId, UseCase groupUseCase, bool includeGlobalUseCases);

	object GetUpgradeLogHandle(IDevice device);

	void AddRecordToUpgradeLog(object handle, UpgradeLogRecord record, IDevice device);

	void LogDataToUpgradeLog(object handle, string name, string value);

	void FinalizeUpgradeLog(object handle);

	bool UploadMQSLog(string dir, string fileName);

	StationDescriptor GetStationDescriptor();

	object GetXmlLogHandle(IDevice device);

	void AddRecordToXmlLog(object handle, XmlLogRecord record, IDevice device);

	void FinalizeXmlLog(object handle);

	IThreadLocked LocalOptions();

	bool CheckAvailDiskSpace();

	void StartPendingDownload();

	string ReadRecipeContent(string recipePathName);

	string ReadShellResponseUpdateContent(string updatePathName);

	List<string> GetCarrierParameters(UseCase useCase, ref string modelId);

	bool SetCarrierParameter(UseCase useCase, ref string modelId, int selectedIndex);

	List<string> GetXlateFiles();

	string GetUsersContent();

	void SaveUsersContent(string usersJson);

	string GetUserQualityContent();

	void SaveUserQualityContent(string qualityJson);

	UsersRequestResult UploadUsers(string usersJson, bool savedToLocal);

	UsersRequestResult DownloadUsers(out string usersJson);

	UsersRequestResult GetGamificationPoint(string uuid, out string totalPoints);

	string GetDefaultCitPrintFilePath();

	string GetDefaultRefurbInfoPath();

	DeviceAttributes GetDeviceLatestAttributes(string modelId);

	DeviceAttributes GetDeviceAttributes(ref string modelId, IDevice device);

	void CleanUpFiles();

	string ReadPcbaFileContent(string filePathName);

	string GetSOAToken(SOATokenType tokenType, out string expiredIn, out string apiKey);

	string GetJsonPushNotification();

	int GetUpgradeLogCount();

	string GetSecureUrl(string url, string method = "GET");

	bool OpenSmartHelper();

	string GetCurrentSharedFolder();

	bool MoveSharedFolder(string newFolderPathName, out string errorMsg);

	string GetRepairHistory(string serialNumber);

	bool IsWifiDevice(string productName, out SortedList<string, string> props);

	DeviceType GetDeviceType(string productName, out string group);

	string GetLatestFlashId(string modelId);

	string LoadDeviceConnImg(UseCase useCase, ref string modelId, IDevice device = null);

	string LoadDeviceConnImg(DeviceMode deviceMode);

	List<string> GetPcbaFilePathNames();

	string GetLatestSoftwareHistory(string serialNumber);

	string CreateMovedFolder(string filePathName);

	void RemoveMovedFolder(string filePathName);

	bool FileExists(string filePathName);

	byte[] FileReadAllBytes(string filePathName);

	long FileSize(string filePathName);

	string OfflineEnabled();

	bool IsCsvFileSpecified(UseCase useCase, IDevice device);

	string ReadTroubleShootFileContent(string filePathName);

	string GeneratePdf();

	List<string> GetPartNumberInfos(UseCase useCase, ref string modelId);

	bool SetPartNumberInfo(UseCase useCase, ref string modelId, string partNumber);

	void IgnoreHWSerNum();

	List<string[]> ParseSameFamily();

	string GetFamilyFromPhoneName(string phoneName);
}
