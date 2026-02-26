using System;
using System.Collections.Generic;
using ISmart;

namespace SmartUtil;

public class TroubleshootingLogger : IResultSubLogger, IDisposable
{
	private const string UNKNOWN_SERIAL_NUMBER = "000000000000000";

	private IDevice device;

	private Dictionary<string, string> dataSets = new Dictionary<string, string>();

	private bool open;

	private string TAG => GetType().FullName;

	public string Name => "Troubleshooting";

	public UseCase UseCase { get; set; }

	public List<TroubleshootingInfo> TopInfo { get; private set; }

	public bool IsOpen => open;

	public TroubleshootingLogger(IDevice device)
	{
		Smart.Log.Debug(TAG, "Troubleshooting log opened for " + device.ID.ToString());
		this.device = device;
		TopInfo = new List<TroubleshootingInfo>();
		open = true;
	}

	public void AddResult(string name, SortedList<string, dynamic> details)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Invalid comparison between Unknown and I4
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Invalid comparison between Unknown and I4
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Invalid comparison between Unknown and I4
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		if (TopInfo.Count > 0)
		{
			return;
		}
		Result val = (Result)details["result"];
		if ((int)val == 4 || (int)val == 1 || (int)val == 3)
		{
			string text = details["name"];
			string text2 = details["step"];
			string text3 = details["description"];
			string text4 = details["dynamic"];
			string text5 = device.ModelId;
			if (text5 != null && text5 != string.Empty)
			{
				bool flag = false;
				text5 = Smart.Rsd.GetValue("sku", (UseCase)134, device, ref flag, false);
				Smart.Log.Verbose(TAG, $"Found Troubleshooting SKU '{text5}'");
			}
			ITroubleshooting troubleshooting = Smart.Troubleshooting;
			UseCase useCase = UseCase;
			TopInfo = troubleshooting.CalculateTop(((object)(UseCase)(ref useCase)).ToString(), text5, text2, text, text3, text4);
		}
	}

	public void AddInfo(string name, string value)
	{
	}

	public void Dispose()
	{
		open = false;
	}
}
