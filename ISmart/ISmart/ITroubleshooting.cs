using System.Collections.Generic;

namespace ISmart;

public interface ITroubleshooting
{
	void Load();

	List<TroubleshootingInfo> CalculateTop(string useCase, string model, string stepName, string description, string failureCode, string dynamicData);

	IResultSubLogger NewTroubleshootingLogger(IDevice device);

	List<TroubleshootingInfo> FindInfo(IDevice device);
}
