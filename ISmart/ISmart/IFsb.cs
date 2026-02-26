using System;
using System.Collections.Generic;

namespace ISmart;

public interface IFsb
{
	List<KeyValuePair<string, string>> GetAllComplaints();

	Dictionary<string, string> GetAllComplaintIcons();

	List<KeyValuePair<string, string>> GetAllSymptoms();

	Dictionary<string, string> GetAllSymptomIcons();

	List<string> GetAllFsbModelCarriers();

	Dictionary<string, string> GetAllModelCarriers();

	string GetModelCarrierFromSaleModel(string saleModel, IDevice device, out string tableauSvnkit, ref string detectionStatus, out bool inSupTableau);

	string GetModelNameFromSaleModel(string saleModel);

	List<string> GetFSBNumbers(string imei, List<string> complaints, List<string> symptoms, string modelId);

	Tuple<string, string> GetJsonFsb(string fsbName);

	string GetImeiList(string fsbName);

	List<string> GetPictureFilePathNames(string fsbName);

	string GetSaleModelFileContent();

	string GetRepairCodePriorityFileContent();

	object GetFsbLogHandle(IDevice device);

	void AddRecordToFsbLog(object handle, UpgradeLogRecord record, IDevice device);

	void LogDataToFsbLog(object handle, string name, string value);

	void FinalizeFsbLog(object handle);
}
