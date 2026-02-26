using System;
using ISmart;

namespace SmartDevice.Steps;

public class SameSnTransfer : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Invalid comparison between Unknown and I4
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = val.SerialNumber;
		string text2 = ((dynamic)base.Info.Args).InputName.ToString();
		string text3 = base.Cache[text2];
		base.Log.AddInfo("SerialNumberIn", text3);
		string text4 = val.ID;
		string text5 = val.SerialNumber2;
		string text6 = string.Empty;
		if (base.Cache.ContainsKey(text2 + "Dual"))
		{
			text6 = base.Cache[text2 + "Dual"];
			base.Log.AddInfo("SerialNumberInDual", text6);
		}
		SerialNumberType val2;
		if ((int)val.Type == 2)
		{
			if (val.Group == string.Empty)
			{
				text4 = ((val.PSN.Length > 10) ? val.PSN.Substring(val.PSN.Length - 10, 10) : val.PSN);
			}
			if (val.WiFiOnlyDevice)
			{
				string text7 = base.Cache["GSNIn"];
				string gSN = val.GSN;
				val2 = Smart.Convert.ToSerialNumberType(text7);
				string text8 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
				val2 = Smart.Convert.ToSerialNumberType(gSN);
				if (((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant() != text8)
				{
					throw new NotSupportedException($"Incoming SN Type {text8} does not match outgoing SNs: {text}");
				}
				Smart.Web.SameSnTransfer(text7, gSN, string.Empty, string.Empty, text4);
				LogPass();
				return;
			}
		}
		int num = 1;
		int num2 = 1;
		if (text6 != null && text6.Trim() != string.Empty && text6.Trim() != text3.Trim())
		{
			num++;
		}
		else
		{
			text6 = string.Empty;
		}
		if (text5 != null && text5.Trim() != string.Empty && text5.Trim() != text.Trim())
		{
			num2++;
		}
		else
		{
			text5 = string.Empty;
		}
		Smart.Log.Info(TAG, string.Format("SN {0}: '{1}'", "In", text3));
		Smart.Log.Info(TAG, string.Format("SN {0}: '{1}'", "In Dual", text6));
		Smart.Log.Info(TAG, string.Format("SN {0}: '{1}'", "Out", text));
		Smart.Log.Info(TAG, string.Format("SN {0}: '{1}'", "Out Dual", text5));
		if (num != num2)
		{
			Smart.Log.Error(TAG, $"Incoming SNs ({num}) not equal to outgoing SNs ({num2})");
			if (num < num2)
			{
				throw new NotSupportedException("Not enough incoming serial numbers to map to outgoing serial numbers");
			}
			throw new NotSupportedException("Too many incoming serial numbers to map to outgoing serial numbers");
		}
		val2 = Smart.Convert.ToSerialNumberType(text3);
		string text9 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
		val2 = Smart.Convert.ToSerialNumberType(text);
		string text10 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
		if (text10 != text9)
		{
			val2 = Smart.Convert.ToSerialNumberType(text5);
			text10 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
			if (text10 != text9)
			{
				throw new NotSupportedException($"Incoming SN Type {text9} does not match outgoing SNs: {text} or {text5}");
			}
			string text11 = text;
			text = text5;
			text5 = text11;
		}
		string empty = string.Empty;
		if (num > 1)
		{
			val2 = Smart.Convert.ToSerialNumberType(text6);
			empty = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
			val2 = Smart.Convert.ToSerialNumberType(text5);
			text10 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
			if (empty != text10)
			{
				throw new NotSupportedException(string.Format("Incoming Dual SN Type {0} does not match outgoing dual SN: {1}", empty, text, text5));
			}
		}
		Smart.Web.SameSnTransfer(text3, text, text6, text5, text4);
		LogPass();
	}
}
