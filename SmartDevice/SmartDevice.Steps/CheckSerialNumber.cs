using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class CheckSerialNumber : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		List<string> list = new List<string>();
		list.Add(val.SerialNumber);
		if ((((dynamic)base.Info.Args).Dual != null) && (bool)((dynamic)base.Info.Args).Dual)
		{
			list.Add(val.SerialNumber2);
		}
		foreach (string item in list)
		{
			if (item == null || item.Trim() == string.Empty)
			{
				LogResult((Result)1, "Found blank SN");
				return;
			}
			bool flag = true;
			string text = item.Trim();
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] != '0')
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				LogResult((Result)1, "Found all zeros SN");
				return;
			}
		}
		LogPass();
	}
}
