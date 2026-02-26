using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using ISmart;

namespace SmartDevice.Steps;

public class FastbootFlash : FastbootStep
{
	private static Dictionary<string, int> OperationToTimeout = new Dictionary<string, int>
	{
		{ "flash", 300000 },
		{ "erase", 60000 },
		{ "oem", 60000 },
		{ "getvar", 20000 },
		{ "reboot", 20000 },
		{ "reboot-bootloader", 10000 },
		{ "format", 60000 },
		{ "flashall", 600000 },
		{ "continue", 10000 }
	};

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_069b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0637: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_085a: Unknown result type (might be due to invalid IL or missing references)
		//IL_085d: Invalid comparison between Unknown and I4
		//IL_0868: Unknown result type (might be due to invalid IL or missing references)
		//IL_080b: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).XML;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		bool invertPartitions = false;
		if (((dynamic)base.Info.Args).DefaultSlot != null)
		{
			string text2 = ((dynamic)base.Info.Args).DefaultSlot;
			text2 = text2.Trim().ToLowerInvariant();
			string text3 = val.GetLogInfoValue("current-slot").Trim().ToLowerInvariant();
			Smart.Log.Debug(TAG, "current-slot: " + text3);
			if (text3 != string.Empty)
			{
				if ((text3 == "a" && text2 == "b") || (text3 == "b" && text2 == "a"))
				{
					Smart.Log.Debug(TAG, $"Inverting partition from default slot {text2} to active slot {text3}");
					invertPartitions = true;
				}
				else
				{
					if (!(text3 == text2))
					{
						throw new NotSupportedException($"Could not find way to change default partition slot {text2} to active slot {text3}");
					}
					Smart.Log.Debug(TAG, $"Active partition active slot {text3} matches default slot {text2}");
					invertPartitions = false;
				}
			}
		}
		bool flag = true;
		if (((dynamic)base.Info.Args).FwFastboot != null)
		{
			flag = (bool)((dynamic)base.Info.Args).FwFastboot;
		}
		string text4 = Smart.Rsd.CreateMovedFolder(text);
		try
		{
			long totalDataSize;
			List<CommandLine> list = ParseCommands(text4, invertPartitions, out totalDataSize);
			foreach (CommandLine item in list)
			{
				if (item.CmdString.StartsWith("flash") && item.DataSize == 0L)
				{
					string text5 = item.CmdString.Substring(item.CmdString.IndexOf(":\\") - 1);
					Smart.User.MessageBox(Smart.Locale.Xlate(TAG), Smart.Locale.Xlate("Image file damaged , the whole folder will be deleted, please redownload the whole files:") + text5, (MessageBoxButtons)0, (MessageBoxIcon)48);
					text5 = text5.Substring(0, text5.LastIndexOf("\\"));
					Smart.File.Remove(text5);
					throw new FileNotFoundException("Image file damaged");
				}
			}
			string text6 = Smart.Rsd.GetFilePathName("fastbootExe", base.Recipe.Info.UseCase, val);
			if (flag)
			{
				string directoryName = Path.GetDirectoryName(text4);
				string text7 = Path.Combine(directoryName, "fastboot.exe");
				if (File.Exists(text7))
				{
					text6 = text7;
				}
				else
				{
					text7 = Path.Combine(directoryName, "Windows\\fastboot.exe");
					if (File.Exists(text7))
					{
						text6 = text7;
					}
				}
			}
			long num = 0L;
			if (list.Count > 0)
			{
				Result result = (Result)8;
				string description = "";
				string text8 = "";
				int num2 = default(int);
				foreach (CommandLine item2 in list)
				{
					List<string> list2 = Smart.MotoAndroid.Shell(val.ID, item2.CmdString, item2.TimeOut, text6, ref num2, 6000, false);
					if (num2 != 0)
					{
						Smart.MotoAndroid.Shell(val.ID, "oem read_sv", 3000, text6, ref num2, 6000, false);
						description = "Fastboot flash shell command failed";
						text8 = "";
						foreach (string item3 in list2)
						{
							if (item3.ToLowerInvariant().Contains("rollback downgrade"))
							{
								description = "Fastboot flash rejected due to downgrade";
							}
							if ((item3 != null) & (item3.Trim().Length > 0))
							{
								if (text8.Length > 0)
								{
									text8 += " ";
								}
								text8 += item3;
							}
						}
						result = (Result)1;
						break;
					}
					num += item2.DataSize;
					double progress = 100.0 * (double)num / (double)totalDataSize;
					ProgressUpdate(progress);
				}
				VerifyOnly(ref result);
				if ((int)result == 8)
				{
					LogPass();
				}
				else
				{
					LogResult(result, description, text8);
				}
			}
			else
			{
				Smart.Log.Error(TAG, $"No fastboot command in the {text} file");
				LogResult((Result)1, "No fastboot command found in file");
			}
		}
		finally
		{
			Smart.Rsd.RemoveMovedFolder(text);
		}
	}

	private List<CommandLine> ParseCommands(string xmlFile, bool invertPartitions, out long totalDataSize)
	{
		bool flag = true;
		List<string> list = new List<string>();
		if (((dynamic)base.Info.Args).Partitions != null)
		{
			string[] array = ((string)((dynamic)base.Info.Args).Partitions).Trim().Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text in array)
			{
				list.Add(text.Trim());
			}
		}
		if (base.Cache.ContainsKey("ErasingModems"))
		{
			flag = base.Cache["ErasingModems"] == "yes";
		}
		List<CommandLine> list2 = new List<CommandLine>();
		totalDataSize = 0L;
		if (!File.Exists(xmlFile))
		{
			return list2;
		}
		string xml = File.ReadAllText(xmlFile, Encoding.UTF8);
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(xml);
		XmlNode xmlNode = xmlDocument.SelectSingleNode("/flashing/steps");
		string directoryName = Path.GetDirectoryName(xmlFile);
		if (xmlNode != null)
		{
			XmlNodeList xmlNodeList = xmlNode.SelectNodes("step");
			if (xmlNodeList != null)
			{
				foreach (XmlNode item in xmlNodeList)
				{
					XmlNode namedItem;
					string text2 = (((namedItem = item.Attributes.GetNamedItem("operation")) != null) ? namedItem.Value : string.Empty).Trim();
					string text3 = (((namedItem = item.Attributes.GetNamedItem("partition")) != null) ? namedItem.Value : string.Empty).Trim();
					string partitionBase = GetPartitionBase(text3);
					if (invertPartitions)
					{
						if (text3.ToLowerInvariant().EndsWith("_a"))
						{
							text3 = text3.Substring(0, text3.Length - 2);
							text3 += "_b";
						}
						else if (text3.ToLowerInvariant().EndsWith("_b"))
						{
							text3 = text3.Substring(0, text3.Length - 2);
							text3 += "_a";
						}
					}
					string text4 = (((namedItem = item.Attributes.GetNamedItem("filename")) != null) ? namedItem.Value : string.Empty).Trim();
					string text5 = (((namedItem = item.Attributes.GetNamedItem("var")) != null) ? namedItem.Value : string.Empty).Trim();
					long num = 0L;
					string text6;
					if (text4 != string.Empty)
					{
						text6 = Path.Combine(directoryName, text4);
						if (File.Exists(text6))
						{
							num = new FileInfo(text6).Length;
							if (list.Count > 0)
							{
								if (list.Contains(partitionBase))
								{
									totalDataSize += num;
								}
							}
							else
							{
								totalDataSize += num;
							}
						}
					}
					else
					{
						text6 = string.Empty;
					}
					int timeOut;
					if (OperationToTimeout.ContainsKey(text2))
					{
						timeOut = OperationToTimeout[text2];
					}
					else
					{
						Smart.Log.Debug(TAG, $"No timeout pre-specified for fastboot op: {text2}");
						timeOut = 120000;
					}
					string text7 = ((!(text5 != string.Empty)) ? (text2 + " " + text3 + " " + Smart.Convert.QuoteFilePathName(text6)) : (text2 + " " + text5));
					text7 = text7.Trim();
					if (!flag && text7.Contains("erase modemst"))
					{
						continue;
					}
					if (list.Count > 0)
					{
						if (list.Contains(partitionBase))
						{
							list2.Add(new CommandLine(text7, timeOut, num));
							Smart.Log.Debug(TAG, $"Add cmdString: '{text7}' size {num} bytes");
						}
					}
					else
					{
						list2.Add(new CommandLine(text7, timeOut, num));
						Smart.Log.Debug(TAG, $"Add cmdString: '{text7}' size {num} bytes");
					}
				}
			}
		}
		return list2;
	}

	private string GetPartitionBase(string partition)
	{
		string result = partition;
		if (partition.EndsWith("_a") || partition.EndsWith("_b"))
		{
			result = partition.Substring(0, partition.Length - 2);
		}
		return result;
	}
}
