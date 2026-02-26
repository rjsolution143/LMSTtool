using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice;

public class RsdLogger : IResultSubLogger, IDisposable
{
	private object upgradeLogHandle;

	private IDevice device;

	private string TAG => GetType().FullName;

	public string Name => "Upgrade";

	public UseCase UseCase { get; set; }

	public bool IsOpen => upgradeLogHandle != null;

	public RsdLogger(IDevice device)
	{
		this.device = device;
		Smart.Log.Info(TAG, "Upgrade log opened for " + device.ID);
		upgradeLogHandle = Smart.Rsd.GetUpgradeLogHandle(device);
	}

	public void AddResult(string name, SortedList<string, dynamic> details)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (!IsOpen)
		{
			throw new NotSupportedException("Upgrade log is not open for writing");
		}
		Result val = (Result)details["result"];
		UseCase useCase = UseCase;
		UseCase useCase2 = UseCase;
		UpgradeLogRecord val2 = default(UpgradeLogRecord);
		((UpgradeLogRecord)(ref val2))._002Ector((SortedList<string, object>)details, val, useCase, name == ((object)(UseCase)(ref useCase2)).ToString(), device.Log.RsdLogId);
		Smart.Rsd.AddRecordToUpgradeLog(upgradeLogHandle, val2, device);
	}

	public void AddInfo(string name, string value)
	{
		if (!IsOpen)
		{
			throw new NotSupportedException("Upgrade log is not open for writing");
		}
		Smart.Rsd.LogDataToUpgradeLog(upgradeLogHandle, name, value);
	}

	public void Dispose()
	{
		if (IsOpen)
		{
			Smart.Rsd.FinalizeUpgradeLog(upgradeLogHandle);
			Smart.Log.Info(TAG, "Upgrade log closed for " + device.ID);
			upgradeLogHandle = null;
		}
	}
}
