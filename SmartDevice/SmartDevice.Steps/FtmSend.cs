using System;
using ISmart;

namespace SmartDevice.Steps;

public class FtmSend : FtmStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0889: Unknown result type (might be due to invalid IL or missing references)
		//IL_0890: Unknown result type (might be due to invalid IL or missing references)
		//IL_0892: Invalid comparison between Unknown and I4
		//IL_08d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0864: Unknown result type (might be due to invalid IL or missing references)
		//IL_0869: Unknown result type (might be due to invalid IL or missing references)
		//IL_0854: Unknown result type (might be due to invalid IL or missing references)
		Result val = (Result)8;
		bool flag = false;
		if (((dynamic)base.Info.Args).Loop != null && (bool)((dynamic)base.Info.Args).Loop)
		{
			flag = true;
		}
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
		DateTime now = DateTime.Now;
		do
		{
			try
			{
				val = (Result)8;
				string text = ((dynamic)base.Info.Args).Data;
				IFtmResponse val2 = base.ftm.SendCommand(text);
				if ((((dynamic)base.Info.Args).IgnoreError == null || !((dynamic)base.Info.Args).IgnoreError) && val2.ErrorCode != 0)
				{
					Smart.Log.Error(TAG, string.Format("Command failed with error code value {1}", val2.ErrorCode));
					val = (Result)1;
				}
				val = VerifyPropertyValue(val2.ToHex());
			}
			catch (Exception ex)
			{
				if (!flag)
				{
					throw;
				}
				Smart.Log.Error(TAG, ex.Message);
				val = (Result)4;
			}
			if ((int)val == 8)
			{
				flag = false;
			}
			else if (DateTime.Now.Subtract(now).TotalSeconds > (double)num)
			{
				flag = false;
			}
			if (flag)
			{
				Smart.Thread.Wait(TimeSpan.FromSeconds(num2));
			}
		}
		while (flag);
		LogResult(val);
	}
}
