using System;
using System.Collections.Generic;
using LabelManager2;

namespace ISmart;

public interface IPrintLabel
{
	Dictionary<string, string> CollectDeviceDataForPrint(IDevice device, SortedList<string, dynamic> cache);

	Dictionary<string, Dictionary<string, string>> ReadDataFromParameterFileForPrint(string parameterFile);

	void AssignDataToCSLabelVariable(Document doc, SortedList<string, dynamic> allCachedData);

	string GetLabelParameterFilePath(IDevice device, bool getParamFileFromCommonStorageDir, string expectedParamFileName, UseCase useCase);

	string GetPN(string labelFile, IDevice device, bool getPNFromLabelName, UseCase useCase);

	Dictionary<string, string> ParseAdditionalDataFromParameterFile(Dictionary<string, string> paramData);

	Result ValidateDataMissing(SortedList<string, dynamic> allData, string keysToCheck, out string error_msg);

	Tuple<string, int, string> LoadPrintSettingsFromOptionFile(IDevice device);
}
