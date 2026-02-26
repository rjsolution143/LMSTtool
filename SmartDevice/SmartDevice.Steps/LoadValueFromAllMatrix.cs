using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class LoadValueFromAllMatrix : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		string text = ((dynamic)base.Info.Args).LookUpCondition;
		string text2 = ((dynamic)base.Info.Args).LookUpValues;
		List<string> list = new List<string>();
		Result val = (Result)0;
		if (text == null || text == string.Empty || text2 == null || text2 == string.Empty)
		{
			throw new NotSupportedException("LookUpCondition and LookUpValue are all required");
		}
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		string text3 = text2.ToLowerInvariant();
		if (!(text3 == "usecase"))
		{
			if (!(text3 == "stepvalue"))
			{
				throw new NotSupportedException($"LookUpValue type '{text2}' is not supported yet");
			}
			list = Smart.Rsd.GetStepsInAllMatrixBySKU(text);
		}
		else
		{
			list = Smart.Rsd.GetUseCasesInAllMatrixBySKU(text);
		}
		if (list.Count < 1)
		{
			Smart.Log.Info(TAG, "Download file error or totally no value found in all matrix files including DFS matrix");
			val = (Result)1;
			VerifyOnly(ref val);
			LogResult(val, "Download file error or totally no value found in all matrix files including DFS matrix");
		}
		else
		{
			string text4 = "\"" + string.Join("\"", list) + "\"";
			base.Cache.Add(text2, text4);
			Smart.Log.Info(TAG, $"{text2} for {text} were collected to be: {text4}");
			val = (Result)8;
			VerifyOnly(ref val);
			SetPreCondition(text4);
			LogPass();
		}
	}
}
