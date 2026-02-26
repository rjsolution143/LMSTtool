using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class TcmdBattIDList : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)1;
		List<string> list = new List<string>();
		string description = string.Empty;
		string text = ((dynamic)base.Info.Args).BattIDListCacheName;
		string text2 = ((dynamic)base.Info.Args).OpCode;
		string text3 = ((dynamic)base.Info.Args).Data;
		string text4 = ((dynamic)base.Info.Args).InputName;
		if (text == null || text == string.Empty || text2 == null || text2 == string.Empty || text3 == null || text3 == string.Empty)
		{
			throw new NotSupportedException("Args of BattIDList is missing");
		}
		if (text4.StartsWith("$"))
		{
			string key = text4.Substring(1);
			text4 = base.Cache[key];
		}
		try
		{
			string text5 = TestCommand(text2, text3);
			if (text5.Length > 2)
			{
				text5 = text5.Substring(2);
			}
			for (int i = 0; i < text5.Length; i += 20)
			{
				int length = Math.Min(20, text5.Length - i);
				string text6 = text5.Substring(i, length);
				if (text6.Length == 20)
				{
					list.Add(Smart.Convert.HexStringToAsciiString(text6));
				}
			}
			if (list.Count < 1)
			{
				result = (Result)1;
				description = "No battID list found";
			}
			if (list.Count >= 1 && list.Contains(text4.ToUpperInvariant()))
			{
				result = (Result)8;
				base.Cache.Add(text, string.Join(",", list));
			}
			else
			{
				result = (Result)1;
				description = "Scanned BattID is not in the list";
			}
			Smart.Log.Debug(TAG, string.Format("Scanned BattID = {0}, BattIDList = '{1}', result = {2}", text4, string.Join(",", list), ((object)(Result)(ref result)).ToString()));
			VerifyOnly(ref result);
		}
		catch (Exception ex)
		{
			Smart.Log.Debug(TAG, ex.Message + ex.StackTrace);
		}
		LogResult(result, description);
	}

	protected string TestCommand(string opCode, string data)
	{
		return base.tcmd.SendCommand(opCode, data).DataHex;
	}
}
