using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using FuzzySharp;
using ISmart;
using SmartUtil.TestForms;

namespace SmartUtil;

public class TestUI : ITestUI
{
	private enum Date31
	{
		Code1 = 1,
		Code2,
		Code3,
		Code4,
		Code5,
		Code6,
		Code7,
		Code8,
		Code9,
		CodeA,
		CodeB,
		CodeC,
		CodeD,
		CodeE,
		CodeF,
		CodeG,
		CodeH,
		CodeJ,
		CodeK,
		CodeL,
		CodeM,
		CodeN,
		CodeP,
		CodeR,
		CodeS,
		CodeT,
		CodeV,
		CodeW,
		CodeX,
		CodeY,
		CodeZ
	}

	private Form testForm;

	private string TAG => GetType().FullName;

	public void Test()
	{
		Smart.Log.Debug(TAG, "Printing out debug message as simple test");
	}

	private void ScanTest()
	{
		Random random = new Random();
		int num = 20;
		for (int i = 0; i < num; i++)
		{
			int num2 = random.Next(5, 10);
			Smart.Log.Debug(TAG, $"Disabling scan for {num2} seconds");
			Smart.DeviceManager.BackgroundScan = false;
			Smart.Thread.DelayedCallback((ThreadStart)ScanTestHelper, TimeSpan.FromSeconds(num2));
			int num3 = random.Next(1, 15);
			Smart.Log.Debug(TAG, $"Waiting {num3} seconds");
			Smart.Thread.Wait(TimeSpan.FromSeconds(num3));
		}
	}

	private void ScanTestHelper()
	{
		Smart.Log.Debug(TAG, "Re-enabling scan");
		Smart.DeviceManager.BackgroundScan = true;
	}

	private void WeeklyTest()
	{
		string text = string.Empty;
		DateTime dateTime = DateTime.Now;
		string text2 = string.Empty;
		for (int i = 0; i < 3651; i++)
		{
			string empty = string.Empty;
			int num = 2000;
			int num2 = 0;
			int num3 = 0;
			if (text2 != null && text2 != string.Empty)
			{
				empty = text2;
				string s = empty.Substring(0, 4);
				string s2 = empty.Substring(5, 2);
				string s3 = empty.Substring(empty.Length - 2);
				num = int.Parse(s);
				num2 = int.Parse(s2);
				num3 = int.Parse(s3);
				num3++;
				num3 %= 100;
			}
			Calendar calendar = new CultureInfo("en-US").Calendar;
			DateTime time = dateTime;
			int year = calendar.GetYear(time);
			int weekOfYear = calendar.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
			if (year > num || (year == num && weekOfYear > num2))
			{
				string text3 = Smart.File.Uuid();
				text3 = text3.Substring(0, 7).ToUpperInvariant();
				string rSDUniqueID = Smart.Security.RSDUniqueID;
				rSDUniqueID = rSDUniqueID.Substring(rSDUniqueID.Length - 3).ToUpperInvariant();
				object[] args = new object[5] { year, weekOfYear, rSDUniqueID, text3, num3 };
				empty = string.Format("{0:0000}-{1:00}:{2}{3}{4:00}", args);
				text2 = empty;
				text = text + empty + Environment.NewLine;
			}
			dateTime = dateTime.AddDays(1.0);
		}
		Smart.Log.Verbose(TAG, text);
	}

	private void MessageTest()
	{
		Smart.Messages.CreateChannel("TestChannel", (Func<string, string>)MessageReceive);
		DateTime now = DateTime.Now;
		while (DateTime.Now.Subtract(now).TotalMinutes < 5.0)
		{
			Smart.Thread.Wait(TimeSpan.FromSeconds(20.0));
			Smart.Messages.SendMessage("TestChannel", "PING");
		}
	}

	private string MessageReceive(string text)
	{
		Smart.Log.Info(TAG, text);
		return "OKAY";
	}

	private void WebAuthTest()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		Smart.Rsd.Login = new Login("w30465", "FAKE");
		string empty = string.Empty;
		string empty2 = string.Empty;
		int num = 0;
		try
		{
			Smart.Web.WarrantyRequest("FAKE", false);
		}
		catch (Exception ex)
		{
			Smart.Log.Debug(TAG, ex.ToString());
		}
		try
		{
			Smart.Web.DbsRequest("FAKE", "FAKE", "FAKE", false, "FAKE", "FAKE", "FAKE", "FAKE", Smart.Convert.HexToBytes("FFFFFFFFFFFFFFFF"), "FAKE", ref empty, ref num, ref empty2);
		}
		catch (Exception ex2)
		{
			Smart.Log.Debug(TAG, ex2.ToString());
		}
		try
		{
			Smart.Web.WarrantyTransfer("FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", false, false);
		}
		catch (Exception ex3)
		{
			Smart.Log.Debug(TAG, ex3.ToString());
		}
		try
		{
			Smart.Web.PcbaSerialNumberRequest("FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE");
		}
		catch (Exception ex4)
		{
			Smart.Log.Debug(TAG, ex4.ToString());
		}
		try
		{
			Smart.Web.GetGppdId("FAKE");
		}
		catch (Exception ex5)
		{
			Smart.Log.Debug(TAG, ex5.ToString());
		}
		try
		{
			Smart.Web.SameSnTransfer("FAKE", "FAKE", "FAKE", "FAKE", "FAKE");
		}
		catch (Exception ex6)
		{
			Smart.Log.Debug(TAG, ex6.ToString());
		}
		try
		{
			Smart.Web.GpsRsu("FAKE", "FAKE");
		}
		catch (Exception ex7)
		{
			Smart.Log.Debug(TAG, ex7.ToString());
		}
		try
		{
			Smart.Web.GpsLockCode("FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE");
		}
		catch (Exception ex8)
		{
			Smart.Log.Debug(TAG, ex8.ToString());
		}
		try
		{
			Smart.Web.GetTokenStatus("FAKE");
		}
		catch (Exception ex9)
		{
			Smart.Log.Debug(TAG, ex9.ToString());
		}
		try
		{
			Smart.Web.DataSignODM("FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE");
		}
		catch (Exception ex10)
		{
			Smart.Log.Debug(TAG, ex10.ToString());
		}
		try
		{
			Smart.Web.KeyDispatchODM("FAKE", "FAKE", "FAKE", "FAKE", "FAKE");
		}
		catch (Exception ex11)
		{
			Smart.Log.Debug(TAG, ex11.ToString());
		}
		try
		{
			Smart.Web.KillSwitchODM("FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE");
		}
		catch (Exception ex12)
		{
			Smart.Log.Debug(TAG, ex12.ToString());
		}
		try
		{
			Smart.Web.Rsu("FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE", "FAKE");
		}
		catch (Exception ex13)
		{
			Smart.Log.Debug(TAG, ex13.ToString());
		}
		try
		{
			Smart.Web.Rpk("FAKE", "FAKE", "FAKE", "FAKE", "FAKE");
		}
		catch (Exception ex14)
		{
			Smart.Log.Debug(TAG, ex14.ToString());
		}
	}

	private void RandomTest()
	{
		string text = Smart.File.Uuid();
		string text2 = string.Empty;
		SortedList<string, string> sortedList = new SortedList<string, string>();
		RSAParameters parameters;
		using (RSA rSA = RSA.Create())
		{
			rSA.KeySize = 4096;
			parameters = rSA.ExportParameters(includePrivateParameters: true);
			rSA.ExportParameters(includePrivateParameters: false);
			sortedList["ID"] = text;
			sortedList["Version"] = "1.0";
			sortedList["KeySize"] = "4096";
			sortedList["HashFormat"] = "SHA256";
			sortedList["D"] = Smart.Convert.BytesToHex(parameters.D);
			sortedList["DP"] = Smart.Convert.BytesToHex(parameters.DP);
			sortedList["DQ"] = Smart.Convert.BytesToHex(parameters.DQ);
			sortedList["Exponent"] = Smart.Convert.BytesToHex(parameters.Exponent);
			sortedList["InverseQ"] = Smart.Convert.BytesToHex(parameters.InverseQ);
			sortedList["Modulus"] = Smart.Convert.BytesToHex(parameters.Modulus);
			sortedList["P"] = Smart.Convert.BytesToHex(parameters.P);
			sortedList["Q"] = Smart.Convert.BytesToHex(parameters.Q);
			text2 = Smart.Json.Dump((object)sortedList);
		}
		string text3 = "C:\\temp\\test-key.json";
		Smart.File.WriteText(text3, text2);
		string arg = DateTime.UtcNow.ToString("yyyy-MM-dd-HH:mm:ss:ffff");
		string text4 = $"[{text}] - '{arg}'";
		SortedList<string, string> sortedList2 = new SortedList<string, string>();
		string empty = string.Empty;
		using (SHA256 sHA = SHA256.Create())
		{
			byte[] buffer = Smart.Convert.AsciiToBytes(text4);
			byte[] rgbHash = sHA.ComputeHash(buffer);
			using RSA rSA2 = RSA.Create();
			rSA2.KeySize = 4096;
			rSA2.ImportParameters(parameters);
			RSAPKCS1SignatureFormatter rSAPKCS1SignatureFormatter = new RSAPKCS1SignatureFormatter(rSA2);
			rSAPKCS1SignatureFormatter.SetHashAlgorithm("SHA256");
			byte[] array = rSAPKCS1SignatureFormatter.CreateSignature(rgbHash);
			empty = Smart.Convert.BytesToHex(array);
			sortedList2["SignedHex"] = empty;
			sortedList2["LoginText"] = text4;
			sortedList2["ID"] = sortedList["ID"];
			sortedList2["KeySize"] = sortedList["KeySize"];
			sortedList2["HashFormat"] = sortedList["HashFormat"];
			sortedList2["Version"] = sortedList["Version"];
			sortedList2["Exponent"] = sortedList["Exponent"];
			sortedList2["Modulus"] = sortedList["Modulus"];
		}
		string text5 = Smart.Json.Dump((object)sortedList2);
		string text6 = "C:\\temp\\example-request.json";
		Smart.File.WriteText(text6, text5);
	}

	private void FuzzTest()
	{
		string text = Smart.File.ReadText("C:\\temp\\test.txt");
		string text2 = "Waiting for 10 seconds";
		string[] separator = new string[1] { Environment.NewLine };
		string[] array = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		DateTime now = DateTime.Now;
		int num = -1;
		string arg = string.Empty;
		string[] array2 = array;
		foreach (string text3 in array2)
		{
			int num2 = Fuzz.Ratio(text2, text3);
			if (num2 > num)
			{
				num = num2;
				arg = text3;
			}
		}
		TimeSpan timeSpan = DateTime.Now.Subtract(now);
		Smart.Log.Info(TAG, $"Completed search in {Smart.Convert.TimeSpanToDisplay(timeSpan)}");
		Smart.Log.Info(TAG, $"Found text '{arg}'");
	}

	private string ParseVersionNew(string input, out string androidVersion)
	{
		androidVersion = string.Empty;
		Regex regex = new Regex("^(?<androidVersion>([a-z]|[s-z][0-9]))[a-z]{2,4}[0-9]{2,3}\\.[a-z]?[0-9]+[a-z]?([-.][a-z]?[0-9]+)*", RegexOptions.IgnoreCase);
		string[] array = input.Split(new char[1] { ' ' });
		foreach (string text in array)
		{
			if (regex.IsMatch(text))
			{
				androidVersion = regex.Match(text).Groups["androidVersion"].Value;
				return text;
			}
		}
		return string.Empty;
	}

	private List<int> PullVersionNumbersNew(string parsed, out string alphas)
	{
		List<int> list = new List<int>();
		alphas = string.Empty;
		if (parsed.Length < 2)
		{
			return list;
		}
		char c = parsed[1];
		string text = string.Empty;
		if (char.IsDigit(c))
		{
			text = parsed.Substring(0, 2);
			parsed = parsed.Substring(2);
		}
		List<char> list2 = new List<char>();
		list2.Add('.');
		list2.Add('-');
		string text2 = string.Empty;
		string text3 = parsed;
		for (int i = 0; i < text3.Length; i++)
		{
			char c2 = text3[i];
			if (char.IsLetter(c2))
			{
				alphas += c2;
			}
			else if (char.IsDigit(c2))
			{
				text2 += c2;
			}
			else if (list2.Contains(c2) && text2.Length > 0)
			{
				int item = int.Parse(text2);
				list.Add(item);
				text2 = string.Empty;
			}
		}
		alphas = text + alphas;
		if (text2.Length > 0)
		{
			int item2 = int.Parse(text2);
			list.Add(item2);
			text2 = string.Empty;
		}
		return list;
	}

	private string ParseVersion(string input)
	{
		Regex regex = new Regex("^[a-z]{3,4}[0-9]{2,3}\\.[a-z]?[0-9]+[a-z]?([-.][a-z]?[0-9]+)*", RegexOptions.IgnoreCase);
		string[] array = input.Split(new char[1] { ' ' });
		foreach (string text in array)
		{
			if (regex.IsMatch(text))
			{
				return text;
			}
		}
		return string.Empty;
	}

	private List<int> PullVersionNumbers(string parsed, out string alphas)
	{
		List<char> list = new List<char>();
		list.Add('.');
		list.Add('-');
		alphas = string.Empty;
		string text = string.Empty;
		List<int> list2 = new List<int>();
		for (int i = 0; i < parsed.Length; i++)
		{
			char c = parsed[i];
			if (char.IsLetter(c))
			{
				alphas += c;
			}
			else if (char.IsDigit(c))
			{
				text += c;
			}
			else if (list.Contains(c) && text.Length > 0)
			{
				int item = int.Parse(text);
				list2.Add(item);
				text = string.Empty;
			}
		}
		if (text.Length > 0)
		{
			int item2 = int.Parse(text);
			list2.Add(item2);
			text = string.Empty;
		}
		return list2;
	}

	public void UUIDTest()
	{
		string text = "000000002034304258475006002E0035";
		string text2 = "000000002038304D3442500D0018003F";
		try
		{
			Smart.Web.WarrantyRequest(text, false);
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Test UUID Warranty Request Failed");
			Smart.Log.Error(TAG, ex.ToString());
		}
		string text3 = DateTime.Now.ToString("yyyy-MM-dd");
		string text4 = "PCBA";
		bool flag = false;
		bool flag2 = true;
		try
		{
			Smart.Web.WarrantyTransfer("127.0.0.1", text, text2, "UID", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, text3, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, text4, flag2, flag);
		}
		catch (Exception ex2)
		{
			Smart.Log.Error(TAG, "Test UUID Warranty Transfer Failed");
			Smart.Log.Error(TAG, ex2.ToString());
		}
	}

	public void BulkMotoFocus()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		string text = "COMPLETE";
		string text2 = Path.Combine(Smart.File.CommonStorageDir, "bulk_motofocus.csv");
		if (!System.IO.File.Exists(text2))
		{
			return;
		}
		Smart.User.MessageBox("Bulk MotoFocus", "Starting bulk MotoFocus process", (MessageBoxButtons)0, (MessageBoxIcon)64);
		ICsvFile val = Smart.NewCsvFile();
		val.LoadFile(text2, ',');
		Smart.Log.Debug(TAG, val.Rows.Count.ToString());
		new List<List<string>>(val.Rows);
		foreach (List<string> row in val.Rows)
		{
			if (row.Count < 1)
			{
				continue;
			}
			string text3 = row[0];
			if (row.Count > 1 && row[1].ToLowerInvariant() == text.ToLowerInvariant())
			{
				Smart.Log.Debug(TAG, "Skipping completed IMEI");
				continue;
			}
			IDevice val2 = Smart.DeviceManager.ManualDevice();
			val2.SerialNumber = text3;
			val2.ID = "BULK_MOTOFOCUS_" + text3;
			Smart.Log.Debug(TAG, $"Running MotoFocus Bulk for IMEI {text3}");
			Smart.UseCaseRunner.Run((UseCase)164, val2, false, false);
			if (row.Count < 2)
			{
				row.Add(text);
			}
			else
			{
				row[1] = text;
			}
			val.SaveFile(text2, false);
		}
		Smart.User.MessageBox("Bulk MotoFocus", "Bulk MotoFocus completed", (MessageBoxButtons)0, (MessageBoxIcon)64);
	}

	private void CheckErrorLogs()
	{
		List<string> list = new List<string>();
		foreach (string item in Smart.File.FindFiles("LMST*.zip", "C:\\errorlogs\\", true))
		{
			ITempZip val = Smart.Zip.ExtractTemp(item);
			try
			{
				foreach (string item2 in (IEnumerable<string>)val)
				{
					if (!item2.ToLowerInvariant().EndsWith("lmst-debug.log.zip"))
					{
						continue;
					}
					try
					{
						ITempZip val2 = Smart.Zip.ExtractTemp(item2, "deBug:20@)");
						try
						{
							foreach (string item3 in (IEnumerable<string>)val2)
							{
								if (item3.ToLowerInvariant().EndsWith("lmst-debug.log") && Smart.File.ReadText(item3).Contains("search text"))
								{
									list.Add(item);
								}
							}
						}
						finally
						{
							((IDisposable)val2)?.Dispose();
						}
					}
					catch (Exception)
					{
					}
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		foreach (string item4 in list)
		{
			_ = item4;
		}
	}

	private void CheckErrorLogs2()
	{
		TestCommandClient.Response response = TestCommandClient.Response.BlankResponse;
		List<string> list = new List<string>();
		foreach (string item in Smart.File.FindFiles("LMST*.zip", "C:\\download\\temptest\\", true))
		{
			ITempZip val = Smart.Zip.ExtractTemp(item);
			try
			{
				foreach (string item2 in (IEnumerable<string>)val)
				{
					SortedList<byte, TestCommandClient.Response> sortedList = new SortedList<byte, TestCommandClient.Response>();
					if (!item2.ToLowerInvariant().EndsWith("lmst-debug.log.zip"))
					{
						continue;
					}
					try
					{
						ITempZip val2 = Smart.Zip.ExtractTemp(item2, "deBug:20@)");
						try
						{
							foreach (string item3 in (IEnumerable<string>)val2)
							{
								if (!item3.ToLowerInvariant().EndsWith("lmst-debug.log"))
								{
									continue;
								}
								string text = Smart.File.ReadText(item3);
								string text2 = "TestCommandClient[Verbose]** Received TCMD response:";
								string value = " **";
								while (text.Contains(text2))
								{
									int num = text.IndexOf(text2) + text2.Length;
									int num2 = text.IndexOf(value, num);
									string text3 = text.Substring(num, num2 - num).Trim();
									text = text.Substring(num2);
									byte[] raw = Smart.Convert.HexToBytes(text3);
									Smart.Log.Verbose(TAG, $"Received TCMD response: {text3}");
									lock (new object())
									{
										TestCommandClient.Response response2 = new TestCommandClient.Response(raw);
										Smart.Log.Verbose(TAG, string.Format(response2.ToString()));
										if (!response2.Incomplete)
										{
											goto IL_037c;
										}
										Smart.Log.Debug(TAG, "Received response is incomplete");
										if (response.Equals(TestCommandClient.Response.BlankResponse))
										{
											Smart.Log.Debug(TAG, "Holding incomplete response for processing");
											response = response2;
											continue;
										}
										TestCommandClient.Response existing = response;
										TestCommandClient.Response response3 = response2;
										if (response3.Length == response3.Data.Length + existing.RawResponse.Length)
										{
											existing = response2;
											response3 = response;
										}
										if (existing.Length == existing.Data.Length + response3.RawResponse.Length)
										{
											Smart.Log.Debug(TAG, "Found extra raw response for incomplete response");
											Smart.Log.Debug(TAG, $"{existing.Data.Length} incomplete response + {response3.RawResponse.Length} extra response = {existing.Length} expected response");
											response2 = TestCommandClient.Response.Append(existing, response3.RawResponse);
											Smart.Log.Verbose(TAG, response2.ToString());
											Smart.Log.Verbose(TAG, $"Modified TCMD response: {Smart.Convert.BytesToHex(response2.RawResponse)}");
											response = TestCommandClient.Response.BlankResponse;
											goto IL_037c;
										}
										Smart.Log.Error(TAG, "Discarding old incomplete response for new incomplete response");
										Smart.Log.Debug(TAG, $"{existing.Data.Length} incomplete response + {response3.RawResponse.Length} extra response != {existing.Length} expected response");
										Smart.Log.Debug(TAG, $"{response3.Data.Length} extra response + {existing.RawResponse.Length} incomplete response != {response3.Length} expected response");
										response = response2;
										goto end_IL_014b;
										IL_037c:
										sortedList[response2.SequenceTag] = response2;
										end_IL_014b:;
									}
								}
							}
						}
						finally
						{
							((IDisposable)val2)?.Dispose();
						}
					}
					catch (Exception)
					{
					}
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		foreach (string item4 in list)
		{
			_ = item4;
		}
	}

	private void XlateDump()
	{
		List<string> list = Smart.File.FindFiles("*.cs", "C:\\git\\smart", true);
		new List<string>();
		List<Regex> list2 = new List<Regex>
		{
			new Regex("Smart\\.Locale\\.Xlate\\(\"(?<xlate>.+)\"\\)")
		};
		List<string> list3 = new List<string> { "SmartInternal" };
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string item in list)
		{
			bool flag = false;
			foreach (string item2 in list3)
			{
				if (item.ToLowerInvariant().Contains(item2.ToLowerInvariant()))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				continue;
			}
			string input = Smart.File.ReadText(item);
			foreach (Regex item3 in list2)
			{
				foreach (Match item4 in item3.Matches(input))
				{
					string value = item4.Groups["xlate"].Value;
					value = value.Replace('|', '/');
					value = value.Replace("\n", "\\n");
					value = value.Replace("\r", "\\r");
					int num = LineFromPos(input, item4.Index);
					string fileName = Path.GetFileName(item);
					string value2 = $"{value}|file {fileName} line {num}";
					stringBuilder.AppendLine(value2);
				}
			}
		}
		Smart.File.WriteText("C:\\git\\smart\\xlate_dump.txt", stringBuilder.ToString());
	}

	private void ProcessBQ()
	{
		SortedList<string, int> sortedList = new SortedList<string, int>();
		int num = 0;
		using (FileStream stream = new FileStream("C:\\download\\use_case_ordering_full_10-22.csv", FileMode.Open))
		{
			using TextReader textReader = new StreamReader(stream);
			while (textReader.Peek() >= 0)
			{
				string text = textReader.ReadLine();
				num++;
				_ = num % 1000;
				_ = 1;
				if (text.Trim() == string.Empty)
				{
					continue;
				}
				string[] array = ProcessLine(text);
				if (array.Length < 3)
				{
					continue;
				}
				string[] array2 = array[2].Split(new char[1] { ',' });
				if (array2.Length < 1)
				{
					continue;
				}
				string text2 = array2[0];
				string[] array3 = array2;
				for (int i = 0; i < array3.Length; i++)
				{
					if (array3[i] != text2)
					{
						text2 = null;
						break;
					}
				}
				if (text2 == null)
				{
					continue;
				}
				string text3 = array[0];
				if (Enumerable.Contains(text3, ','))
				{
					string[] strings = text3.Split(new char[1] { ',' });
					strings = DupeRemove(strings);
					text3 = string.Join(",", strings);
					if (!sortedList.ContainsKey(text3))
					{
						sortedList[text3] = 1;
					}
					else
					{
						sortedList[text3]++;
					}
				}
			}
		}
		List<KeyValuePair<string, int>> list = sortedList.ToList();
		list.RemoveAll((KeyValuePair<string, int> item) => item.Value < 5);
		foreach (KeyValuePair<string, int> item in list.OrderBy((KeyValuePair<string, int> item) => item.Value).Reverse())
		{
			_ = item;
		}
	}

	private string[] DupeRemove(string[] strings)
	{
		List<string> list = new List<string>();
		string text = null;
		foreach (string text2 in strings)
		{
			if (!(text == text2))
			{
				text = text2;
				list.Add(text2);
			}
		}
		return list.ToArray();
	}

	private string[] ProcessLine(string csvLine)
	{
		List<string> list = new List<string>();
		bool flag = false;
		string text = "";
		char[] array = csvLine.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			char c = array[i];
			switch (c)
			{
			case '"':
				flag = !flag;
				continue;
			case ',':
				if (!flag)
				{
					list.Add(text);
					text = "";
					continue;
				}
				break;
			}
			text += c;
		}
		list.Add(text);
		return list.ToArray();
	}

	private int LineFromPos(string input, int indexPosition)
	{
		int num = 1;
		for (int i = 0; i < indexPosition; i++)
		{
			if (input[i] == '\n')
			{
				num++;
			}
		}
		return num;
	}

	public DialogResult SearchSelect(string title, string text, List<string> choices, out string choice)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return SmartUtil.TestForms.SearchSelect.Select(title, text, choices, out choice);
	}

	public DialogResult ComplaintSelect(string title, string complaintPrompt, SortedList<string, string> complaintNames, SortedList<string, string> complaintIcons, string symptomPrompt, SortedList<string, string> symptomNames, SortedList<string, string> symptomIcons, out List<string> complaintChoice, out List<string> symptomChoice)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return SmartUtil.TestForms.ComplaintSelect.Complaint(title, complaintPrompt, complaintNames, complaintIcons, symptomPrompt, symptomNames, symptomIcons, out complaintChoice, out symptomChoice);
	}

	public DialogResult FSBList(string title, List<string> columns, string buttonText, List<string> deviceInfo, SortedList<string, Tuple<string, string>> fsbs, out string fsbChoice)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return SmartUtil.TestForms.FSBList.List(title, columns, buttonText, deviceInfo, fsbs, out fsbChoice);
	}

	public DialogResult FSBEntry(string title, List<string> columns, List<string> fsbInfo, List<Tuple<string, string, string, string, List<string>>> actions, out List<int> actionChoices)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return SmartUtil.TestForms.FSBEntry.Entry(title, columns, fsbInfo, actions, out actionChoices);
	}

	public SortedList<string, string> WebBrowser(string url)
	{
		string postData = string.Empty;
		SortedList<string, string> headers = new SortedList<string, string>();
		return Smart.Thread.RunAndWait<SortedList<string, string>>((Func<SortedList<string, string>>)(() => SmartUtil.TestForms.WebBrowser.Browse(url, postData, headers)), true);
	}

	public SortedList<string, string> WebBrowser(string url, string postData, SortedList<string, string> headers)
	{
		return Smart.Thread.RunAndWait<SortedList<string, string>>((Func<SortedList<string, string>>)(() => SmartUtil.TestForms.WebBrowser.Browse(url, postData, headers)), true);
	}
}
