using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ISmart;

namespace SmartWeb;

public class TokenWrapper
{
	public enum TOKEN_ERROR
	{
		SUCCESS,
		INVALID_IP_FORMAT,
		MSG_BUF_SIZE_TOO_SMALL,
		SOCKET_NOT_OPEN,
		TLS_SRV_REFUSE_CON,
		TLS_COM_FAIL,
		TOKEN_OPEN_FAIL,
		TOKEN_INTERNAL_ERR,
		TOKEN_ID_MISMATCH,
		TOKEN_NOT_PRESENT,
		INVALID_VERSION,
		INVALID_MSG_FORMAT,
		INVALID_USER_ID,
		TOKEN_SIGN_ERR,
		TOKEN_KEY_NOT_FOUND,
		TOKEN_DECRYPT_FAILED,
		TOKEN_BLOCK_WRONG_SIZE,
		TOKEN_GEN_DATA_BLOCK_FAIL,
		TOKEN_EXPONENT_TOO_LARGE,
		TOKEN_TOO_MANY_DATABLOCKS,
		TOKEN_WRONG_DATABLOCK_TYPE
	}

	private class ResponseReceiver
	{
		public List<string> StdOut { get; private set; }

		public List<string> StdError { get; private set; }

		public ResponseReceiver()
		{
			StdOut = new List<string>();
			StdError = new List<string>();
		}

		public void OutHandler(object sender, DataReceivedEventArgs e)
		{
			if (e.Data != null)
			{
				StdOut.Add(e.Data);
			}
		}

		public void ErrorHandler(object sender, DataReceivedEventArgs e)
		{
			if (e.Data != null)
			{
				StdError.Add(e.Data);
			}
		}
	}

	private object tokenLock = new object();

	private DateTime tokenReleased = DateTime.Now.Subtract(TimeSpan.FromMinutes(1.0));

	private TimeSpan tokenWaitTime = TimeSpan.FromSeconds(2.0);

	private string TAG => GetType().FullName;

	private SortedList<string, string> TokenConnect(SortedList<string, string> input)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(2.0);
		lock (tokenLock)
		{
			timeSpan = tokenWaitTime.Subtract(DateTime.Now.Subtract(tokenReleased));
		}
		if (timeSpan.TotalMilliseconds > 0.0)
		{
			Smart.Thread.Wait(timeSpan);
		}
		lock (tokenLock)
		{
			try
			{
				return TokenConnectUnsafe(input);
			}
			finally
			{
				tokenReleased = DateTime.Now;
			}
		}
	}

	private SortedList<string, string> TokenConnectUnsafe(SortedList<string, string> input)
	{
		string filePathName = Smart.Rsd.GetFilePathName("hwTokenExe", (UseCase)0, (IDevice)null);
		string directoryName = Path.GetDirectoryName(filePathName);
		Smart.Log.Debug(TAG, "Token connect path: " + filePathName);
		Process process = new Process();
		ResponseReceiver responseReceiver = new ResponseReceiver();
		process.StartInfo.FileName = filePathName;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.WorkingDirectory = directoryName;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.RedirectStandardInput = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.EnableRaisingEvents = true;
		process.OutputDataReceived += responseReceiver.OutHandler;
		process.ErrorDataReceived += responseReceiver.ErrorHandler;
		try
		{
			string text = ToFlat(input);
			Smart.Log.Debug(TAG, "Token input: " + text);
			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			process.StandardInput.WriteLine(text);
			process.WaitForExit(30000);
			text = string.Join(Environment.NewLine, responseReceiver.StdOut.ToArray());
			if (text.Trim() == string.Empty)
			{
				if ((text = string.Join(Environment.NewLine, responseReceiver.StdError.ToArray())).Contains("AccessViolationException"))
				{
					throw new AccessViolationException("Hardware token is corrupt");
				}
				throw new IOException("No data returned");
			}
			SortedList<string, string> sortedList = FromFlat(text);
			SortedList<string, object> sortedList2 = new SortedList<string, object>();
			foreach (string key in sortedList.Keys)
			{
				sortedList2[key] = sortedList[key];
			}
			Smart.Log.Debug(TAG, Smart.Convert.ToString("Token Output", (IEnumerable<KeyValuePair<string, object>>)sortedList2.ToList()));
			return sortedList;
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Token connection failed: " + ex.Message);
			Smart.Log.Verbose(TAG, ex.ToString());
			throw;
		}
		finally
		{
			if (!process.HasExited)
			{
				process.Kill();
			}
			string text2 = string.Join(Environment.NewLine, responseReceiver.StdError.ToArray());
			Smart.Log.Debug(TAG, "Token stderr: " + text2);
		}
	}

	public TOKEN_ERROR Decrypt(byte[] cipherText, ref byte[] clearText)
	{
		SortedList<string, string> sortedList = new SortedList<string, string>();
		sortedList["method"] = "decrypt";
		sortedList["cipherText"] = Smart.Convert.BytesToHex(cipherText);
		sortedList["dataSize"] = clearText.Length.ToString();
		SortedList<string, string> sortedList2 = TokenConnect(sortedList);
		int result = int.Parse(sortedList2["code"]);
		clearText = Smart.Convert.HexToBytes(sortedList2["clearText"]);
		return (TOKEN_ERROR)result;
	}

	public TOKEN_ERROR GenerateDBSRequest(int version, ref byte[] requestMsg, string userID, out string ipAddress, out string location, out string sourceType, out string benchId)
	{
		SortedList<string, string> sortedList = new SortedList<string, string>();
		sortedList["method"] = "generatedbsrequest";
		sortedList["version"] = version.ToString();
		sortedList["requestMsg"] = Smart.Convert.BytesToHex(requestMsg);
		sortedList["userID"] = userID;
		SortedList<string, string> sortedList2 = TokenConnect(sortedList);
		int result = int.Parse(sortedList2["code"]);
		requestMsg = Smart.Convert.HexToBytes(sortedList2["extendedMessage"]);
		ipAddress = sortedList2["ipAddress"];
		location = sortedList2["location"];
		sourceType = sortedList2["sourceType"];
		benchId = sortedList2["benchId"];
		return (TOKEN_ERROR)result;
	}

	public TOKEN_ERROR GenExtendFieldType1(int version, ref byte[] reqMsg, string userID, out string ipAddress, out string location, out string sourceType, out string benchId, out byte[] tnlRequest)
	{
		SortedList<string, string> sortedList = new SortedList<string, string>();
		sortedList["method"] = "genextendfieldtype1";
		sortedList["version"] = version.ToString();
		sortedList["reqMsg"] = Smart.Convert.BytesToHex(reqMsg);
		sortedList["userID"] = userID;
		SortedList<string, string> sortedList2 = TokenConnect(sortedList);
		int result = int.Parse(sortedList2["code"]);
		reqMsg = Smart.Convert.HexToBytes(sortedList2["extendedMessage"]);
		tnlRequest = null;
		if (sortedList2.ContainsKey("tnlRequest"))
		{
			tnlRequest = Smart.Convert.HexToBytes(sortedList2["tnlRequest"]);
		}
		ipAddress = sortedList2["ipAddress"];
		location = sortedList2["location"];
		sourceType = sortedList2["sourceType"];
		benchId = sortedList2["benchId"];
		return (TOKEN_ERROR)result;
	}

	public TOKEN_ERROR GenExtendFieldType2(int version, ref byte[] reqMsg, string userID, out string ipAddress, out string location, out string sourceType, out string benchId, out byte[] tnlRequest)
	{
		SortedList<string, string> sortedList = new SortedList<string, string>();
		sortedList["method"] = "genextendfieldtype2";
		sortedList["version"] = version.ToString();
		sortedList["reqMsg"] = Smart.Convert.BytesToHex(reqMsg);
		sortedList["userID"] = userID;
		SortedList<string, string> sortedList2 = TokenConnect(sortedList);
		int result = int.Parse(sortedList2["code"]);
		reqMsg = Smart.Convert.HexToBytes(sortedList2["extendedMessage"]);
		tnlRequest = null;
		if (sortedList2.ContainsKey("tnlRequest"))
		{
			tnlRequest = Smart.Convert.HexToBytes(sortedList2["tnlRequest"]);
		}
		ipAddress = sortedList2["ipAddress"];
		location = sortedList2["location"];
		sourceType = sortedList2["sourceType"];
		benchId = sortedList2["benchId"];
		return (TOKEN_ERROR)result;
	}

	public int GetExtendFieldLen(int clientAuthVersion)
	{
		SortedList<string, string> sortedList = new SortedList<string, string>();
		sortedList["method"] = "getextendfieldlen";
		sortedList["clientAuthVersion"] = clientAuthVersion.ToString();
		return int.Parse(TokenConnect(sortedList)["extendFieldLen"]);
	}

	public TOKEN_ERROR ProcessTnl(byte[] reqMsg, byte[] signedMsg, byte[] tnlRequest, byte[] tnlResponse)
	{
		SortedList<string, string> sortedList = new SortedList<string, string>();
		sortedList["method"] = "processtnl";
		sortedList["reqMsg"] = Smart.Convert.BytesToHex(reqMsg);
		sortedList["signedMsg"] = Smart.Convert.BytesToHex(signedMsg);
		sortedList["tnlRequest"] = Smart.Convert.BytesToHex(tnlRequest);
		sortedList["tnlResponse"] = Smart.Convert.BytesToHex(tnlResponse);
		SortedList<string, string> sortedList2 = TokenConnect(sortedList);
		TOKEN_ERROR result = (TOKEN_ERROR)int.Parse(sortedList2["code"]);
		_ = sortedList2["status"];
		int.Parse(sortedList2["type"]);
		_ = sortedList2["detailed"];
		return result;
	}

	private static string ToFlat(SortedList<string, string> param)
	{
		string text = string.Empty;
		foreach (KeyValuePair<string, string> item in param)
		{
			text += $"{item.Key}={item.Value.Replace('=', '-').Replace('|', ':')}|";
		}
		return text.TrimEnd(new char[1] { '|' });
	}

	private static SortedList<string, string> FromFlat(string flat)
	{
		SortedList<string, string> sortedList = new SortedList<string, string>();
		string[] array = flat.Trim().Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[1] { '=' }, StringSplitOptions.RemoveEmptyEntries);
			string key = array2[0];
			string value = string.Empty;
			if (array2.Length > 1)
			{
				value = array2[1];
			}
			sortedList[key] = value;
		}
		return sortedList;
	}

	private static string ToHex(byte[] bytes)
	{
		return BitConverter.ToString(bytes).Replace("-", string.Empty);
	}

	private static byte[] FromHex(string hex)
	{
		int length = hex.Length;
		byte[] array = new byte[length / 2];
		for (int i = 0; i < length; i += 2)
		{
			array[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
		}
		return array;
	}
}
