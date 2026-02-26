using System;
using ISmart;

namespace SmartDevice.Steps;

public class WarrantyTransfer : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Invalid comparison between Unknown and I4
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Invalid comparison between Unknown and I4
		//IL_0804: Unknown result type (might be due to invalid IL or missing references)
		//IL_0809: Unknown result type (might be due to invalid IL or missing references)
		//IL_0825: Unknown result type (might be due to invalid IL or missing references)
		//IL_082a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0894: Unknown result type (might be due to invalid IL or missing references)
		//IL_089a: Invalid comparison between Unknown and I4
		//IL_0852: Unknown result type (might be due to invalid IL or missing references)
		//IL_0857: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08eb: Unknown result type (might be due to invalid IL or missing references)
		string text = "PCBA";
		if (((dynamic)base.Info.Args).SwapType != null)
		{
			text = ((dynamic)base.Info.Args).SwapType;
			Smart.Log.Info(TAG, $"Using recipe-selected SwapType {text}");
		}
		else
		{
			Smart.Log.Info(TAG, $"Using default SwapType {text}");
		}
		bool flag = false;
		if (((dynamic)base.Info.Args).SwapOnly != null)
		{
			flag = ((dynamic)base.Info.Args).SwapOnly;
		}
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text2 = val.SerialNumber;
		string text3 = ((dynamic)base.Info.Args).InputName.ToString();
		string text4 = base.Cache[text3];
		base.Log.AddInfo("SerialNumberIn", text4);
		if ((int)val.Type == 2 && !val.WiFiOnlyDevice && (int)Smart.Convert.ToSerialNumberType(text4) == 3)
		{
			text2 = val.GSN;
		}
		string text5 = val.SerialNumber2;
		string text6 = string.Empty;
		if (base.Cache.ContainsKey(text3 + "Dual"))
		{
			text6 = base.Cache[text3 + "Dual"];
			base.Log.AddInfo("SerialNumberInDual", text6);
		}
		string empty = string.Empty;
		string text7 = string.Empty;
		if (base.Cache.ContainsKey(text3 + "Tri"))
		{
			text7 = base.Cache[text3 + "Tri"];
		}
		int num = 1;
		int num2 = 1;
		if (text6 != null && text6.Trim() != string.Empty && text6.Trim() != text4.Trim())
		{
			num++;
		}
		else
		{
			text6 = string.Empty;
		}
		if (text7 != null && text7.Trim() != string.Empty && text7.Trim() != text4.Trim() && text7.Trim() != text6.Trim())
		{
			num++;
		}
		else
		{
			text7 = string.Empty;
		}
		if (text5 != null && text5.Trim() != string.Empty && text5.Trim() != text2.Trim())
		{
			num2++;
		}
		else
		{
			text5 = string.Empty;
		}
		Smart.Log.Info(TAG, string.Format("SN {0}: '{1}'", "In", text4));
		Smart.Log.Info(TAG, string.Format("SN {0}: '{1}'", "In Dual", text6));
		Smart.Log.Info(TAG, string.Format("SN {0}: '{1}'", "In Tri", text7));
		Smart.Log.Info(TAG, string.Format("SN {0}: '{1}'", "Out", text2));
		Smart.Log.Info(TAG, string.Format("SN {0}: '{1}'", "Out Dual", text5));
		if (num != num2)
		{
			throw new NotSupportedException($"Incoming SNs ({num}) not equal to outgoing SNs ({num2})");
		}
		SerialNumberType val2 = Smart.Convert.ToSerialNumberType(text4);
		string text8 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
		val2 = Smart.Convert.ToSerialNumberType(text2);
		string text9 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
		if (text9 != text8)
		{
			val2 = Smart.Convert.ToSerialNumberType(text5);
			text9 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
			if (text9 != text8)
			{
				throw new NotSupportedException($"Incoming SN Type {text8} does not match outgoing SNs: {text2} or {text5}");
			}
			string text10 = text2;
			text2 = text5;
			text5 = text10;
		}
		if ((int)val.Type == 2 && text8 == "MSN")
		{
			text8 = "IMEI";
		}
		string text11 = string.Empty;
		if (num > 1)
		{
			val2 = Smart.Convert.ToSerialNumberType(text6);
			text11 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
			val2 = Smart.Convert.ToSerialNumberType(text5);
			text9 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
			if (text11 != text9)
			{
				throw new NotSupportedException(string.Format("Incoming Dual SN Type {0} does not match outgoing dual SN: {1}", text11, text2, text5));
			}
		}
		string empty2 = string.Empty;
		string text12 = DateTime.Now.ToString("yyyy-MM-dd");
		string text13 = string.Empty;
		if (base.Cache.ContainsKey("DeviceSerialNumberDetails"))
		{
			text13 = base.Cache["DeviceSerialNumberDetails"]["apc_code"];
		}
		bool flag2 = true;
		Smart.Web.WarrantyTransfer("127.0.0.1", text4, text2, text8, text6, text5, text11, text7, empty, empty2, text12, string.Empty, string.Empty, text13, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, text, flag2, flag);
		LogPass();
	}
}
