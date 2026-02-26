using System;
using System.Linq;
using System.Text.RegularExpressions;
using ISmart;

namespace SmartDevice.Steps;

public class UTAGWrite : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0adc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ade: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae1: Invalid comparison between Unknown and I4
		//IL_0ec8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec5: Unknown result type (might be due to invalid IL or missing references)
		string text = ((dynamic)base.Info.Args).OpCode;
		string text2 = ((dynamic)base.Info.Args).Data;
		string text3 = string.Empty;
		if (((dynamic)base.Info.Args).Value != null)
		{
			text3 = ((dynamic)base.Info.Args).Value;
			if (text3.StartsWith("$"))
			{
				string text4 = text3.Substring(1);
				text3 = base.Cache[text4];
				Smart.Log.Debug(TAG, $"UTAG value {text3} was loaded by Cache key {text4}");
			}
		}
		if (((dynamic)base.Info.Args).Field != null)
		{
			IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
			string text5 = ((dynamic)base.Info.Args).Field;
			if (text5.ToLowerInvariant() == "trackid".ToLowerInvariant())
			{
				text3 = val.ID;
			}
			else if (text5.ToLowerInvariant() == "serialnumber".ToLowerInvariant())
			{
				text3 = val.SerialNumber;
			}
			else if (text5.ToLowerInvariant() == "serialnumber2".ToLowerInvariant())
			{
				text3 = val.SerialNumber2;
			}
			else if (text5.ToLowerInvariant() == "datecode".ToLowerInvariant())
			{
				text3 = DateTime.Now.ToString("MM-dd-yyyy");
			}
			else if (text5.ToLowerInvariant() == "universal_lan_mac_address".ToLowerInvariant())
			{
				if (!base.Cache.ContainsKey("universal_lan_mac_address"))
				{
					throw new NotSupportedException("No universal_lan_mac_address in cache");
				}
				text3 = base.Cache["universal_lan_mac_address"];
				if ((bool)((dynamic)base.Info.Args).IncludeMacAddressColon)
				{
					text3 = string.Join(":", (from m in Regex.Matches(text3, ".{2}").OfType<Match>()
						select m.Value).ToArray());
				}
			}
			else
			{
				if (!text5.ToLowerInvariant().Contains("wlan_mac"))
				{
					throw new NotSupportedException($"UTAG field type {text5} is unrecognized");
				}
				if (text5.ToLowerInvariant().Contains(","))
				{
					string[] array = text5.Split(new char[1] { ',' });
					for (int i = 0; i < array.Length; i++)
					{
						if (base.Cache.ContainsKey(array[i]))
						{
							array[i] = base.Cache[array[i]];
							if ((bool)((dynamic)base.Info.Args).IncludeMacAddressColon)
							{
								array[i] = string.Join(":", (from m in Regex.Matches(array[i], ".{2}").OfType<Match>()
									select m.Value).ToArray());
							}
							continue;
						}
						throw new NotSupportedException("No wlan_mac in cache");
					}
					text3 = string.Join(",", array);
				}
				else
				{
					if (!base.Cache.ContainsKey(text5))
					{
						throw new NotSupportedException("No wlan_mac in cache");
					}
					text3 = base.Cache[text5];
					if ((bool)((dynamic)base.Info.Args).IncludeMacAddressColon)
					{
						text3 = string.Join(":", (from m in Regex.Matches(text3, ".{2}").OfType<Match>()
							select m.Value).ToArray());
					}
				}
			}
		}
		string text6 = Smart.Convert.BytesToHex(Smart.Convert.AsciiToBytes(text3));
		ushort num = (ushort)text3.Length;
		string text7 = Smart.Convert.BytesToHex(Smart.Convert.UShortToBytes(num));
		text2 = text2 + text7 + text6;
		ITestCommandResponse val2 = base.tcmd.SendCommand(text, text2);
		Result val3 = (Result)(val2.Failed ? 1 : 8);
		if ((int)val3 == 8 && ((((dynamic)base.Info.Args).Verify != null && (bool)((dynamic)base.Info.Args).Verify) ? true : false))
		{
			string strB = "GenericResponse";
			if (((dynamic)base.Info.Args).ResponseCode != null)
			{
				strB = ((dynamic)base.Info.Args).ResponseCode;
			}
			ResponseCode responseCode = val2.ResponseCode;
			if (string.Compare(((object)(ResponseCode)(ref responseCode)).ToString(), strB, ignoreCase: true) != 0)
			{
				val3 = (Result)1;
			}
		}
		LogResult(val3);
	}
}
