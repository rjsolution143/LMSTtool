using System.Collections.Generic;

namespace ISmart;

public struct UpgradeLogRecord
{
	public bool LastRecord { get; private set; }

	public UseCase CurrentUseCase { get; private set; }

	public SortedList<string, dynamic> Details { get; set; }

	public Result StepResult { get; set; }

	public string LogId { get; private set; }

	public UpgradeLogRecord(SortedList<string, dynamic> details, Result result, UseCase useCase, bool lastRecord, string logId)
	{
		this = default(UpgradeLogRecord);
		LastRecord = lastRecord;
		CurrentUseCase = useCase;
		Details = details;
		StepResult = result;
		LogId = logId;
	}
}
