using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class LoadPKIKeys : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		_ = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).LookUpCondition;
		Result val = (Result)0;
		if (text == null || text == string.Empty)
		{
			throw new NotSupportedException("LookUpCondition is required");
		}
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		SortedList<string, string> valuePKIKeysByModel = Smart.Rsd.GetValuePKIKeysByModel(text);
		if (valuePKIKeysByModel.Count < 1)
		{
			Smart.Log.Debug(TAG, $"Value of {text} was not found in PKI key list file");
			val = (Result)1;
			VerifyOnly(ref val);
			LogResult(val, "Value not found");
			return;
		}
		string value = valuePKIKeysByModel.Keys[0];
		string value2 = valuePKIKeysByModel.Values[0];
		if (valuePKIKeysByModel.Count > 1)
		{
			List<string> list = new List<string>(valuePKIKeysByModel.Keys);
			string text2 = list[0];
			string text3 = Smart.Locale.Xlate("Load PKI Keys");
			string text4 = Smart.Locale.Xlate("Multiple PKI keys found, please select the one you need");
			Smart.User.SearchSelect(text3, text4, list, ref text2);
			Smart.Log.Debug(TAG, $"Product {text2} was selected for PKI key");
			value = text2;
			value2 = valuePKIKeysByModel[text2];
		}
		base.Cache["pkiKeys"] = value2;
		base.Cache["recipeProduct"] = value;
		val = (Result)8;
		VerifyOnly(ref val);
		LogPass();
	}
}
