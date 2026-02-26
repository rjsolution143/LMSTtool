using System;
using ISmart;

namespace SmartDevice.Steps;

public class NewSnValidate : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = val.SerialNumber;
		string text2 = ((dynamic)base.Info.Args).InputName.ToString();
		string text3 = base.Cache[text2];
		base.Log.AddInfo("SerialNumberIn", text3);
		_ = val.ID;
		string text4 = val.SerialNumber2;
		string text5 = string.Empty;
		if (base.Cache.ContainsKey(text2 + "Dual"))
		{
			text5 = base.Cache[text2 + "Dual"];
			base.Log.AddInfo("SerialNumberInDual", text5);
		}
		int num = 1;
		int num2 = 1;
		if (text5 != null && text5.Trim() != string.Empty && text5.Trim() != text3.Trim())
		{
			num++;
		}
		else
		{
			text5 = string.Empty;
		}
		if (text4 != null && text4.Trim() != string.Empty && text4.Trim() != text.Trim())
		{
			num2++;
		}
		else
		{
			text4 = string.Empty;
		}
		Smart.Log.Info(TAG, string.Format("SN {0}: '{1}'", "In", text3));
		Smart.Log.Info(TAG, string.Format("SN {0}: '{1}'", "In Dual", text5));
		Smart.Log.Info(TAG, string.Format("SN {0}: '{1}'", "Out", text));
		Smart.Log.Info(TAG, string.Format("SN {0}: '{1}'", "Out Dual", text4));
		if (num != num2)
		{
			Smart.Log.Error(TAG, $"Incoming SNs ({num}) not equal to outgoing SNs ({num2})");
			if (num < num2)
			{
				throw new NotSupportedException("Not enough incoming serial numbers to map to outgoing serial numbers");
			}
			throw new NotSupportedException("Too many incoming serial numbers to map to outgoing serial numbers");
		}
		SerialNumberType val2 = Smart.Convert.ToSerialNumberType(text3);
		string text6 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
		val2 = Smart.Convert.ToSerialNumberType(text);
		string text7 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
		if (text7 != text6)
		{
			val2 = Smart.Convert.ToSerialNumberType(text4);
			text7 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
			if (text7 != text6)
			{
				throw new NotSupportedException($"Incoming SN Type {text6} does not match outgoing SNs: {text} or {text4}");
			}
			string text8 = text;
			text = text4;
			text4 = text8;
		}
		string empty = string.Empty;
		if (num > 1)
		{
			val2 = Smart.Convert.ToSerialNumberType(text5);
			empty = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
			val2 = Smart.Convert.ToSerialNumberType(text4);
			text7 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
			if (empty != text7)
			{
				throw new NotSupportedException(string.Format("Incoming Dual SN Type {0} does not match outgoing dual SN: {1}", empty, text, text4));
			}
		}
		Smart.Web.ServiceSerialNumber(text3, text, text5, text4, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
		LogPass();
	}
}
