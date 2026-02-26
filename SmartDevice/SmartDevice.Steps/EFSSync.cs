using System;
using System.Linq;
using System.Text;
using ISmart;

namespace SmartDevice.Steps;

public class EFSSync : FtmStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		int num = 30;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
		}
		int num2 = 0;
		if (((dynamic)base.Info.Args).Delay != null)
		{
			num2 = ((dynamic)base.Info.Args).Delay;
		}
		string text = ((dynamic)base.Info.Args).Data;
		IFtmResponse val = base.ftm.SendCommand(text);
		Smart.Convert.BytesToUShort(val.Raw.Skip(4).Take(2).Reverse()
			.ToArray());
		int num3 = Smart.Convert.BytesToInt(val.Raw.Skip(6).Take(4).Reverse()
			.ToArray());
		ushort num4 = Smart.Convert.BytesToUShort(val.Raw.Skip(10).Take(2).Reverse()
			.ToArray());
		if (num4 != 0 && num4 != 306)
		{
			Smart.Log.Error(TAG, $"EFS Sync failed with error {num4}");
			LogResult((Result)4, "EFS Sync command failed", $"Error code {num4}");
			return;
		}
		StringBuilder stringBuilder = new StringBuilder(text);
		stringBuilder[5] = '1';
		string value = Smart.Convert.BytesToHex(Smart.Convert.IntToBytes(num3).Reverse().ToArray());
		stringBuilder.Insert(12, value);
		string text2 = stringBuilder.ToString();
		Result result = (Result)1;
		DateTime now = DateTime.Now;
		string description = string.Empty;
		do
		{
			try
			{
				val = base.ftm.SendCommand(text2);
				Smart.Convert.BytesToUShort(val.Raw.Skip(4).Take(2).Reverse()
					.ToArray());
				byte num5 = val.Raw[6];
				Smart.Convert.BytesToUShort(val.Raw.Skip(7).Take(2).Reverse()
					.ToArray());
				if (num5 == 1)
				{
					LogPass();
					return;
				}
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, ex.Message);
				result = (Result)4;
				description = "FTM Command Failed";
			}
			Smart.Thread.Wait(TimeSpan.FromSeconds(num2));
		}
		while (DateTime.Now.Subtract(now).TotalSeconds < (double)num);
		LogResult(result, description);
	}
}
