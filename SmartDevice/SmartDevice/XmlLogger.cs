using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice;

public class XmlLogger : IResultSubLogger, IDisposable
{
	private IDevice device;

	private object xmlLogHandle;

	private string TAG => GetType().FullName;

	public string Name => "XML";

	public UseCase UseCase { get; set; }

	public bool IsOpen => xmlLogHandle != null;

	public XmlLogger(IDevice device)
	{
		Smart.Log.Info(TAG, "XMLLog opened for " + device.ID);
		this.device = device;
		xmlLogHandle = Smart.Rsd.GetXmlLogHandle(device);
	}

	public void AddResult(string name, SortedList<string, dynamic> details)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (!IsOpen)
		{
			throw new NotSupportedException("XmlLog is not open for writing");
		}
		XmlLogRecord val = default(XmlLogRecord);
		((XmlLogRecord)(ref val))._002Ector((Result)details["result"], UseCase);
		UseCase useCase = UseCase;
		if (name == ((object)(UseCase)(ref useCase)).ToString() && IsSoftwareUpdatingUseCase(UseCase))
		{
			Smart.Rsd.AddRecordToXmlLog(xmlLogHandle, val, device);
		}
	}

	public void AddInfo(string name, string value)
	{
	}

	public void Dispose()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (IsOpen)
		{
			if (IsSoftwareUpdatingUseCase(UseCase))
			{
				Smart.Rsd.FinalizeXmlLog(xmlLogHandle);
			}
			xmlLogHandle = null;
			Smart.Log.Info(TAG, "XML log closed for " + device.ID);
		}
	}

	private bool IsSoftwareUpdatingUseCase(UseCase useCase)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Invalid comparison between Unknown and I4
		if ((int)useCase != 129 && (int)useCase != 135 && (int)useCase != 144 && (int)useCase != 153 && (int)useCase != 142)
		{
			return (int)useCase == 211;
		}
		return true;
	}
}
