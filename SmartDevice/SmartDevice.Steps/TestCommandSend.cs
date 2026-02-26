using System;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class TestCommandSend : TestCommandStep
{
	public string dynamicError = string.Empty;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0846: Unknown result type (might be due to invalid IL or missing references)
		//IL_0893: Unknown result type (might be due to invalid IL or missing references)
		//IL_0896: Invalid comparison between Unknown and I4
		//IL_0e00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e03: Invalid comparison between Unknown and I4
		//IL_0e16: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e19: Invalid comparison between Unknown and I4
		//IL_0e0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e14: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ba7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1baa: Invalid comparison between Unknown and I4
		//IL_1bb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b95: Invalid comparison between Unknown and I4
		//IL_1b9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b98: Unknown result type (might be due to invalid IL or missing references)
		//IL_131a: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_1493: Unknown result type (might be due to invalid IL or missing references)
		//IL_1562: Unknown result type (might be due to invalid IL or missing references)
		//IL_1532: Unknown result type (might be due to invalid IL or missing references)
		//IL_1631: Unknown result type (might be due to invalid IL or missing references)
		//IL_1601: Unknown result type (might be due to invalid IL or missing references)
		//IL_1947: Unknown result type (might be due to invalid IL or missing references)
		//IL_1722: Unknown result type (might be due to invalid IL or missing references)
		string text = ((dynamic)base.Info.Args).OpCode;
		string text2 = ((dynamic)base.Info.Args).Data;
		if (text2.StartsWith("$"))
		{
			string key = text2.Substring(1);
			text2 = base.Cache[key];
		}
		bool flag = false;
		if (((dynamic)base.Info.Args).IncludeLength != null)
		{
			flag = ((dynamic)base.Info.Args).IncludeLength;
		}
		string text3 = string.Empty;
		if (flag)
		{
			text3 = Smart.Convert.BytesToHex(Smart.Convert.UShortToBytes((ushort)text2.Length));
		}
		bool flag2 = false;
		if (((dynamic)base.Info.Args).ConvertAscii != null)
		{
			flag2 = ((dynamic)base.Info.Args).ConvertAscii;
		}
		if (flag2)
		{
			text2 = Smart.Convert.AsciiStringToHexString(text2);
		}
		if (flag)
		{
			text2 = text3 + text2;
		}
		string text4 = string.Empty;
		if (((dynamic)base.Info.Args).PreData != null)
		{
			text4 = ((dynamic)base.Info.Args).PreData;
		}
		string text5 = string.Empty;
		if (((dynamic)base.Info.Args).PostData != null)
		{
			text5 = ((dynamic)base.Info.Args).PostData;
		}
		text2 = text4 + text2 + text5;
		Smart.Log.Verbose(TAG, $"Opcode {text},Data {text2}");
		ITestCommandResponse val = base.tcmd.SendCommand(text, text2);
		Result result = (Result)(val.Failed ? 1 : 8);
		string text6 = val.DataHex;
		if ((int)result == 8 && ((dynamic)base.Info.Args).DecodeHex != null && (bool)((dynamic)base.Info.Args).DecodeHex && val.DataHex.Length > 0)
		{
			text6 = Smart.Convert.HexStringToAsciiString(val.DataHex);
			Smart.Log.Verbose(TAG, $"Decoded hex return {val.DataHex} to value {text6}");
		}
		if (((dynamic)base.Info.Args).SetPreCond != null && (bool)((dynamic)base.Info.Args).SetPreCond)
		{
			SetPreCondition(text6);
		}
		if ((int)result == 8)
		{
			result = VerifyPropertyValue(text6);
		}
		if ((int)result == 8)
		{
			IDevice val2 = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
			string strA = string.Empty;
			if (((dynamic)base.Info.Args).RespData != null)
			{
				strA = ((dynamic)base.Info.Args).RespData;
			}
			if (string.Compare(strA, "MfgDate", ignoreCase: true) == 0)
			{
				string text7 = val.DataHex.Substring(4);
				byte[] bytes = Smart.Convert.HexToBytes(text7);
				string @string = Encoding.ASCII.GetString(bytes);
				base.Log.AddInfo("Mfg Date", @string);
				val2.ManufacturingDate = @string;
			}
			else if (string.Compare(strA, "PCBPartNo", ignoreCase: true) == 0)
			{
				byte[] array = Smart.Convert.HexToBytes(val.DataHex);
				string text8 = Smart.Convert.BytesToAscii(array).Substring(1).Replace('\0', ' ');
				int startIndex = 43;
				string text9 = text8.Substring(startIndex, 10).Trim();
				base.Log.AddInfo("pcb-part-no", text9);
				Smart.Log.Debug(TAG, "pcb-part-no is " + text9);
			}
			else if (string.Compare(strA, "GoogleCSR", ignoreCase: true) == 0)
			{
				if (!val.DataHex.StartsWith("00"))
				{
					string text10 = (dynamicError = "Google CSR request failed with status " + val.DataHex.Substring(0, 2));
					Smart.Log.Error(TAG, text10);
					throw new Exception(text10);
				}
				int num = int.Parse(val.DataHex.Substring(2, 4), NumberStyles.HexNumber);
				string text11 = Smart.Convert.BytesToAscii(val.Data).Substring(3);
				if (text11.Length != num || num == 0)
				{
					string text12 = (dynamicError = $"googleCsr key size {text11.Length} does not match expected length {num} or size is 0");
					Smart.Log.Error(TAG, text12);
					throw new Exception(text12);
				}
				base.Log.AddInfo("googleCsr", text11);
				Smart.Log.Debug(TAG, "googleCsr is " + text11);
			}
			else if (string.Compare(strA, "RKP", ignoreCase: true) == 0)
			{
				string dataHex = val.DataHex;
				Smart.Log.Debug(TAG, $"Hex Response: {dataHex}");
				if (dataHex == "00")
				{
					Smart.Log.Debug(TAG, "RKP is supported");
					base.Log.AddInfo("RKP", "Supported");
				}
				else if (dataHex == "06")
				{
					Smart.Log.Error(TAG, "RKP is not supported");
					base.Log.AddInfo("RKP", "Not supported");
					result = (Result)1;
				}
				else
				{
					dynamicError = $"Query RKP response {dataHex} is invalid";
					Smart.Log.Error(TAG, dynamicError);
					result = (Result)1;
				}
			}
			else if (string.Compare(strA, "XCVR", ignoreCase: true) == 0)
			{
				byte[] array2 = Smart.Convert.HexToBytes(val.DataHex);
				string text13 = Smart.Convert.BytesToAscii(array2).Substring(1).Replace('\0', ' ');
				int startIndex2 = 0;
				string text14 = text13.Substring(startIndex2, 10).Trim();
				base.Log.AddInfo("XCVR", text14);
				Smart.Log.Debug(TAG, "XCVR is " + text14);
			}
			else if (string.Compare(strA, "QueryEseBKP", ignoreCase: true) == 0)
			{
				string dataHex2 = val.DataHex;
				Smart.Log.Debug(TAG, $"Hex Response: {dataHex2}");
				if (dataHex2 == "00")
				{
					Smart.Log.Debug(TAG, "Ese BKP is supported");
					base.Log.AddInfo("EseBKP", "Supported");
					SetPreCondition("Supported");
				}
				else if (dataHex2 == "01")
				{
					Smart.Log.Debug(TAG, "Ese BKP is not supported");
					base.Log.AddInfo("EseBKP", "Not supported");
					SetPreCondition("NotSupported");
				}
				else
				{
					dynamicError = $"Query Ese BKP Supported response {dataHex2} is invalid";
					Smart.Log.Error(TAG, dynamicError);
					result = (Result)1;
				}
			}
			else if (string.Compare(strA, "ProgramEseBKP", ignoreCase: true) == 0)
			{
				string dataHex3 = val.DataHex;
				Smart.Log.Debug(TAG, $"Hex Response: {dataHex3}");
				if (dataHex3 == "00")
				{
					Smart.Log.Debug(TAG, "Ese BKP is programmed successfully");
					base.Log.AddInfo("EseBKP", "Programmed");
				}
				else if (dataHex3 == "01")
				{
					Smart.Log.Debug(TAG, "Ese BKP is failed to program");
					result = (Result)1;
				}
				else
				{
					dynamicError = $"ProgramEseBKP response {dataHex3} is invalid";
					Smart.Log.Error(TAG, dynamicError);
					result = (Result)1;
				}
			}
			else if (string.Compare(strA, "VerifyEseBKP", ignoreCase: true) == 0)
			{
				string dataHex4 = val.DataHex;
				Smart.Log.Debug(TAG, $"Hex Response: {dataHex4}");
				if (dataHex4 == "00")
				{
					Smart.Log.Debug(TAG, "Ese BKP is programmed");
					base.Log.AddInfo("EseBKP", "Programmed");
				}
				else if (dataHex4 == "01")
				{
					Smart.Log.Debug(TAG, "Ese BKP is not programmed");
					result = (Result)1;
				}
				else
				{
					dynamicError = $"VerifyEseBKP response {dataHex4} is invalid";
					Smart.Log.Error(TAG, dynamicError);
					result = (Result)1;
				}
			}
			else if (string.Compare(strA, "QueryEseBKPProgrammed", ignoreCase: true) == 0)
			{
				string dataHex5 = val.DataHex;
				Smart.Log.Debug(TAG, $"Hex Response: {dataHex5}");
				if (dataHex5 == "00")
				{
					Smart.Log.Debug(TAG, "Ese BKP is programmed");
					base.Log.AddInfo("EseBKP", "Programmed");
					SetPreCondition("Programmed");
				}
				else if (dataHex5 == "01")
				{
					Smart.Log.Debug(TAG, "Ese BKP is not programmed");
					base.Log.AddInfo("EseBKP", "Not programmed");
					SetPreCondition("NotProgrammed");
				}
				else
				{
					dynamicError = $"Query Ese BKP programmed response {dataHex5} is invalid";
					Smart.Log.Error(TAG, dynamicError);
					result = (Result)1;
				}
			}
			if (((dynamic)base.Info.Args).CheckResponseStartWith != null)
			{
				string empty = string.Empty;
				empty = ((dynamic)base.Info.Args).CheckResponseStartWith;
				if (empty.StartsWith("$"))
				{
					string key2 = empty.Substring(1);
					empty = base.Cache[key2];
				}
				Smart.Log.Debug(TAG, "Check Response StartWith:" + empty);
				if (!val.DataHex.StartsWith(empty))
				{
					result = (Result)1;
				}
			}
		}
		if (((dynamic)base.Info.Args).PromptText != null)
		{
			_ = (string)((dynamic)base.Info.Args).PromptType;
			string text15 = ((dynamic)base.Info.Args).PromptText;
			text15 = Smart.Locale.Xlate(text15);
			result = (((int)Smart.User.MessageBox(Smart.Locale.Xlate(TAG), text15, (MessageBoxButtons)4, (MessageBoxIcon)32) != 6) ? ((Result)1) : ((Result)8));
		}
		VerifyOnly(ref result);
		if ((int)result == 8)
		{
			LogPass();
		}
		else
		{
			LogResult(result, text6, dynamicError);
		}
	}
}
