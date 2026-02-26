using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class MultiStep : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		bool flag = true;
		string description = "No checks failed";
		List<string> passedSteps = base.Log.PassedSteps;
		if (((dynamic)base.Info.Args).AllPass != null)
		{
			string[] array = ((string)((dynamic)base.Info.Args).AllPass).Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text in array)
			{
				if (!passedSteps.Contains(text.ToLowerInvariant().Trim()))
				{
					flag = false;
					description = "Not all required steps have passed";
					break;
				}
			}
		}
		if (((dynamic)base.Info.Args).AnyPass != null)
		{
			string[] array2 = ((string)((dynamic)base.Info.Args).AnyPass).Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			bool flag2 = false;
			string[] array = array2;
			foreach (string text2 in array)
			{
				if (passedSteps.Contains(text2.ToLowerInvariant().Trim()))
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				flag = false;
				description = "None of the required steps have passed";
			}
		}
		if (flag)
		{
			LogPass();
		}
		else
		{
			LogResult((Result)1, description);
		}
	}
}
