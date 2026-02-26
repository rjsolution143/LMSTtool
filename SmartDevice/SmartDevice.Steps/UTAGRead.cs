using System;
using System.Linq;
using System.Text.RegularExpressions;
using ISmart;

namespace SmartDevice.Steps;

public class UTAGRead : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Invalid comparison between Unknown and I4
		//IL_1218: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_11dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e0: Invalid comparison between Unknown and I4
		//IL_0b75: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c25: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c88: Unknown result type (might be due to invalid IL or missing references)
		//IL_1039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e36: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b7: Unknown result type (might be due to invalid IL or missing references)
		string text = ((dynamic)base.Info.Args).OpCode;
		string text2 = ((dynamic)base.Info.Args).Data;
		string text3 = ((dynamic)base.Info.Args).SaveToCache;
		ITestCommandResponse val = base.tcmd.SendCommand(text, text2);
		Result val2 = (Result)(val.Failed ? 1 : 8);
		if ((int)val2 == 8)
		{
			byte[] data = val.Data;
			string text4 = Smart.Convert.BytesToAscii(data.Skip(2).ToArray());
			if (((dynamic)base.Info.Args).Verify != null && (bool)((dynamic)base.Info.Args).Verify && ((dynamic)base.Info.Args).Value != null)
			{
				string text5 = ((dynamic)base.Info.Args).Value;
				if (text5.StartsWith("$"))
				{
					string key = text5.Substring(1);
					text5 = base.Cache[key];
				}
				if (text4.Trim() != text5.Trim())
				{
					Smart.Log.Error(TAG, $"UTAG value {text4} does not match expected value {text5}");
					val2 = (Result)1;
				}
			}
			else if (((dynamic)base.Info.Args).Field != null)
			{
				string text6 = ((dynamic)base.Info.Args).Field;
				base.Log.AddInfo($"UTAG{text6}", text4);
				if (((dynamic)base.Info.Args).Verify != null && (bool)((dynamic)base.Info.Args).Verify)
				{
					IDevice val3 = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
					if (text6.ToLowerInvariant() == "trackid".ToLowerInvariant())
					{
						if (text4 != val3.ID)
						{
							Smart.Log.Error(TAG, $"UTAG {text6} value {text4} does not match device value {val3.ID}");
							val2 = (Result)1;
						}
					}
					else if (text6.ToLowerInvariant() == "serialnumber".ToLowerInvariant())
					{
						if (text4 != val3.SerialNumber)
						{
							Smart.Log.Error(TAG, $"UTAG {text6} value {text4} does not match device value {val3.SerialNumber}");
							val2 = (Result)1;
						}
					}
					else if (text6.ToLowerInvariant() == "serialnumber2".ToLowerInvariant())
					{
						if (text4 != val3.SerialNumber2)
						{
							Smart.Log.Error(TAG, $"UTAG {text6} value {text4} does not match device value {val3.SerialNumber}");
							val2 = (Result)1;
						}
					}
					else if (text6.ToLowerInvariant() == "datecode".ToLowerInvariant())
					{
						string text7 = DateTime.Now.ToString("MM-dd-yyyy");
						if (text4 != text7)
						{
							Smart.Log.Error(TAG, $"UTAG {text6} value {text4} does not match correct value {text7}");
							val2 = (Result)1;
						}
					}
					else if (text6.ToLowerInvariant() == "universal_lan_mac_address".ToLowerInvariant())
					{
						string empty = string.Empty;
						if (!base.Cache.ContainsKey("universal_lan_mac_address"))
						{
							throw new NotSupportedException("No universal_lan_mac_address in cache");
						}
						empty = base.Cache["universal_lan_mac_address"];
						if ((bool)((dynamic)base.Info.Args).IncludeMacAddressColon)
						{
							empty = string.Join(":", (from m in Regex.Matches(empty, ".{2}").OfType<Match>()
								select m.Value).ToArray());
						}
						if (text4.Trim() != empty.Trim())
						{
							Smart.Log.Error(TAG, $"UTAG {text6} value {text4} does not match correct value {empty}");
							val2 = (Result)1;
						}
					}
					else
					{
						if (!text6.ToLowerInvariant().Contains("wlan_mac"))
						{
							throw new NotSupportedException($"UTAG field type {text6} is unrecognized");
						}
						string empty2 = string.Empty;
						if (text6.ToLowerInvariant().Contains(","))
						{
							string[] array = text6.Split(new char[1] { ',' });
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
							empty2 = string.Join(",", array);
							if (text4.Trim() != empty2.Trim())
							{
								Smart.Log.Error(TAG, $"UTAG {text6} value {text4} does not match correct value {empty2}");
								val2 = (Result)1;
							}
						}
						else
						{
							if (!base.Cache.ContainsKey(text6))
							{
								throw new NotSupportedException("No wlan_mac in cache");
							}
							empty2 = base.Cache[text6];
							if ((bool)((dynamic)base.Info.Args).IncludeMacAddressColon)
							{
								empty2 = string.Join(":", (from m in Regex.Matches(empty2, ".{2}").OfType<Match>()
									select m.Value).ToArray());
							}
							if (!text4.Trim().Contains(empty2))
							{
								Smart.Log.Error(TAG, $"UTAG {text6} value {text4} does not contain correct value {empty2}");
								val2 = (Result)1;
							}
						}
					}
				}
			}
			if (text3 != null && text3 != string.Empty && (int)val2 == 8)
			{
				base.Cache.Add(text3, text4.Trim());
				Smart.Log.Debug(TAG, $"UTAG value {text4.Trim()} was saved to Cache key {text3}");
			}
		}
		LogResult(val2);
	}
}
