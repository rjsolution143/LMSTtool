using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ISmart;
using Microsoft.Win32;

namespace SmartUtil;

public class Security : ISecurity
{
	internal struct OsVersionInfo
	{
		private readonly uint OsVersionInfoSize;

		internal readonly uint MajorVersion;

		internal readonly uint MinorVersion;

		public readonly uint BuildNumber;

		private readonly uint PlatformId;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		private readonly string CSDVersion;
	}

	public const string VERIFY_TOKEN = "VERIFY_TOKEN_LMST_SECURITY_XXXXX";

	private const char HASH_SEPARATOR = '|';

	private static byte[] genericKey = new byte[16]
	{
		102, 31, 223, 107, 156, 145, 240, 81, 147, 197,
		183, 64, 241, 210, 48, 103
	};

	private string TAG => GetType().FullName;

	public string RSDUniqueID { get; set; }

	public string RSDHardwareFingerprint { get; private set; }

	public string RSDStationID { get; set; } = string.Empty;


	public string RSDWeekly { get; set; } = string.Empty;


	private RSAParameters StationKey { get; set; }

	public bool HasAdminRights
	{
		get
		{
			using WindowsIdentity ntIdentity = WindowsIdentity.GetCurrent();
			return new WindowsPrincipal(ntIdentity).IsInRole(WindowsBuiltInRole.Administrator);
		}
	}

	[DllImport("ntdll.dll", SetLastError = true)]
	internal static extern uint RtlGetVersion(out OsVersionInfo versionInformation);

	public Security()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		RSDUniqueID = "UNKNOWN";
		RSDHardwareFingerprint = "UNKNOWN";
		if (Smart.App.Name.ToLowerInvariant() == "smarthelper")
		{
			return;
		}
		try
		{
			Action action = StationResetPrompt;
			ModifierKeys val = (ModifierKeys)6;
			Keys val2 = (Keys)82;
			Smart.HotKey.Register(action, val2, val);
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Error setting Station Reset hotkey: " + ex.Message);
			Smart.Log.Debug(TAG, ex.ToString());
		}
	}

	public void UpdateIDs()
	{
		try
		{
			RSDUniqueID = FindUniqueId();
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Could not load/generate RSD Unique ID");
			Smart.Log.Verbose(TAG, ex.ToString());
			try
			{
				RSDUniqueID = LoadUserUniqueId();
			}
			catch (Exception ex2)
			{
				Smart.Log.Error(TAG, "Could not load alternative RSD Unique ID");
				Smart.Log.Verbose(TAG, ex2.ToString());
			}
		}
		try
		{
			RSDHardwareFingerprint = FindHardwareId();
		}
		catch (Exception ex3)
		{
			Smart.Log.Error(TAG, "Could not load/generate RSD Hardware Fingerprint");
			Smart.Log.Verbose(TAG, ex3.ToString());
		}
		string text = Path.Combine(Smart.File.CommonStorageDir, "station.key");
		if (!Smart.File.Exists(text))
		{
			StationKeyGen(RSDUniqueID, text);
		}
	}

	private static void StationResetPrompt()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Invalid comparison between Unknown and I4
		try
		{
			Smart.Log.Info("StationResetPrompt", "User requested Station Reset");
			Login val = Smart.Rsd.Login;
			string userName = ((Login)(ref val)).UserName;
			val = Login.Default;
			if (userName != ((Login)(ref val)).UserName)
			{
				ILog log = Smart.Log;
				val = Smart.Rsd.Login;
				log.Debug("StationResetPrompt", $"User already logged in as {((Login)(ref val)).UserName}, cancelling station reset");
				return;
			}
			string text = Smart.Locale.Xlate("Warning: Station Reset will permanently delete current Station ID");
			string text2 = Smart.Security.RSDStationID;
			if (text2 == null || text2 == string.Empty)
			{
				StationDescriptor stationDescriptor = Smart.Rsd.GetStationDescriptor();
				string stationId = ((StationDescriptor)(ref stationDescriptor)).StationId;
				text2 = ((!(stationId != string.Empty)) ? "None" : stationId);
			}
			text = text + "\r\n\r\n" + Smart.Locale.Xlate("(Station ID: ") + text2 + ") ";
			text = text + "\r\n\r\n" + Smart.Locale.Xlate("Are you sure you want to perform a Station Reset?");
			string text3 = Smart.Locale.Xlate("Station Reset");
			if ((int)Smart.User.MessageBox(text3, text, (MessageBoxButtons)4, (MessageBoxIcon)48) == 6)
			{
				Smart.Security.ResetStation();
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Error("StationResetPrompt", "Error during Station Reset: " + ex.Message);
			Smart.Log.Debug("StationResetPrompt", ex.ToString());
		}
	}

	public SortedList<string, string> StationSign()
	{
		string text = Path.Combine(Smart.File.CommonStorageDir, "station.key");
		if (text.ToLowerInvariant().Contains("smarthelper"))
		{
			text = text.ToLowerInvariant().Replace("smarthelper", "smarttool");
		}
		if (!Smart.File.Exists(text))
		{
			Smart.Log.Error(TAG, $"StationSign failed due to missing station key: '{text}'");
			throw new FileNotFoundException("No station key file found, restart application to generate");
		}
		string text2 = Smart.File.ReadText(text);
		string text3 = Smart.Convert.BytesToAscii(Smart.Convert.Base64ToBytes(text2));
		SortedList<string, string> sortedList = Smart.Json.LoadString<SortedList<string, string>>(text3);
		Tuple<string, string> tuple = StationKeySign(sortedList);
		SortedList<string, string> obj = new SortedList<string, string>
		{
			["logintext"] = tuple.Item1,
			["signedhex"] = tuple.Item2,
			["exponent"] = sortedList["Exponent"],
			["modulus"] = sortedList["Modulus"],
			["rsduniqueid"] = RSDUniqueID
		};
		string text4 = Smart.App.Name;
		if (text4.ToLowerInvariant() == "lm smart tool")
		{
			text4 = "LMST";
		}
		else if (Smart.App.Name.ToLowerInvariant() == "smarthelper")
		{
			text4 = "SD";
		}
		obj["clientid"] = $"{text4}_{Smart.App.Version}";
		return obj;
	}

	private void StationKeyGen(string uniqueId, string keyPath)
	{
		string text = string.Empty;
		using (RSA rSA = RSA.Create())
		{
			rSA.KeySize = 4096;
			RSAParameters rSAParameters = rSA.ExportParameters(includePrivateParameters: true);
			rSA.ExportParameters(includePrivateParameters: false);
			SortedList<string, string> sortedList = new SortedList<string, string>();
			sortedList["ID"] = uniqueId;
			sortedList["Version"] = "1.0";
			sortedList["KeySize"] = "4096";
			sortedList["HashFormat"] = "SHA256";
			sortedList["D"] = Smart.Convert.BytesToHex(rSAParameters.D);
			sortedList["DP"] = Smart.Convert.BytesToHex(rSAParameters.DP);
			sortedList["DQ"] = Smart.Convert.BytesToHex(rSAParameters.DQ);
			sortedList["Exponent"] = Smart.Convert.BytesToHex(rSAParameters.Exponent);
			sortedList["InverseQ"] = Smart.Convert.BytesToHex(rSAParameters.InverseQ);
			sortedList["Modulus"] = Smart.Convert.BytesToHex(rSAParameters.Modulus);
			sortedList["P"] = Smart.Convert.BytesToHex(rSAParameters.P);
			sortedList["Q"] = Smart.Convert.BytesToHex(rSAParameters.Q);
			text = Smart.Json.Dump((object)sortedList);
		}
		string text2 = Smart.Convert.BytesToBase64(Smart.Convert.AsciiToBytes(text));
		Smart.File.WriteText(keyPath, text2);
	}

	private Tuple<string, string> StationKeySign(SortedList<string, string> stationKey)
	{
		string arg = DateTime.UtcNow.ToString("yyyy-MM-dd-HH:mm:ss:ffff");
		string text = $"[{RSDUniqueID}] - '{arg}'";
		if (stationKey["Version"] != "1.0")
		{
			string text2 = string.Format("Unsupported station key version: {0}", stationKey["Version"]);
			Smart.Log.Error(TAG, text2);
		}
		if (stationKey["ID"] != RSDUniqueID)
		{
			throw new NotSupportedException("Station Key is corrupt, please remove application and re-install");
		}
		RSAParameters parameters = default(RSAParameters);
		parameters.D = Smart.Convert.HexToBytes(stationKey["D"]);
		parameters.DP = Smart.Convert.HexToBytes(stationKey["DP"]);
		parameters.DQ = Smart.Convert.HexToBytes(stationKey["DQ"]);
		parameters.Exponent = Smart.Convert.HexToBytes(stationKey["Exponent"]);
		parameters.InverseQ = Smart.Convert.HexToBytes(stationKey["InverseQ"]);
		parameters.Modulus = Smart.Convert.HexToBytes(stationKey["Modulus"]);
		parameters.P = Smart.Convert.HexToBytes(stationKey["P"]);
		parameters.Q = Smart.Convert.HexToBytes(stationKey["Q"]);
		string empty = string.Empty;
		using SHA256 sHA = SHA256.Create();
		byte[] buffer = Smart.Convert.AsciiToBytes(text);
		byte[] rgbHash = sHA.ComputeHash(buffer);
		using RSA rSA = RSA.Create();
		rSA.KeySize = 4096;
		rSA.ImportParameters(parameters);
		RSAPKCS1SignatureFormatter rSAPKCS1SignatureFormatter = new RSAPKCS1SignatureFormatter(rSA);
		rSAPKCS1SignatureFormatter.SetHashAlgorithm("SHA256");
		byte[] array = rSAPKCS1SignatureFormatter.CreateSignature(rgbHash);
		empty = Smart.Convert.BytesToHex(array);
		return new Tuple<string, string>(text, empty);
	}

	public void ResetStation()
	{
		Smart.Log.Warning(TAG, "Station Reset initiated!");
		Smart.Log.Info(TAG, "Loading existing IDs");
		UpdateIDs();
		string rSDUniqueID = RSDUniqueID;
		Smart.Log.Debug(TAG, $"Deleting existing RSD Unique ID {rSDUniqueID}");
		try
		{
			string name = "RSDUniqueID";
			RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("Software\\Motorola\\RSD", writable: true);
			if (registryKey != null && registryKey.GetValue(name) != null)
			{
				registryKey.DeleteValue(name);
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Error deleting RSD Unique ID");
			Smart.Log.Verbose(TAG, ex.ToString());
		}
		try
		{
			string name2 = "RSDUniqueID";
			RegistryKey registryKey2 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey("Software\\Lenovo\\Service", writable: false);
			if (registryKey2 != null && registryKey2.GetValue(name2) != null)
			{
				registryKey2.DeleteValue(name2);
			}
		}
		catch (Exception ex2)
		{
			Smart.Log.Error(TAG, "Error deleting alternative RSD Unique ID");
			Smart.Log.Verbose(TAG, ex2.ToString());
		}
		Smart.Log.Debug(TAG, "Deleting Station Key");
		try
		{
			string path = Path.Combine(Smart.File.CommonStorageDir, "station.key");
			if (System.IO.File.Exists(path))
			{
				System.IO.File.Delete(path);
			}
		}
		catch (Exception ex3)
		{
			Smart.Log.Error(TAG, "Error deleting Station Key");
			Smart.Log.Verbose(TAG, ex3.ToString());
		}
		Smart.Log.Debug(TAG, "Generating New Station IDs");
		UpdateIDs();
	}

	private string FindUniqueId()
	{
		string name = "RSDUniqueID";
		RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
		RegistryKey registryKey2 = registryKey.OpenSubKey("Software\\Motorola\\RSD", writable: true);
		if (registryKey2 == null)
		{
			registryKey2 = registryKey.CreateSubKey("Software\\Motorola\\RSD");
		}
		string text = (string)registryKey2.GetValue(name);
		if (text == null)
		{
			text = Smart.File.Uuid();
			text = text.ToUpperInvariant();
			registryKey2.SetValue(name, text);
			Smart.Log.Debug(TAG, "Generated new RSD Unique ID: " + text);
		}
		else
		{
			Smart.Log.Debug(TAG, "Found existing RSD Unique ID: " + text);
		}
		SaveUserUniqueId(text);
		return text;
	}

	private string LoadUserUniqueId()
	{
		string name = "RSDUniqueID";
		RegistryKey? registryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey("Software\\Lenovo\\Service", writable: false);
		if (registryKey == null)
		{
			Smart.Log.Error(TAG, "Could not read user registry");
			throw new NotSupportedException("Could not read user registry");
		}
		string obj = (string)registryKey.GetValue(name);
		if (obj == null)
		{
			Smart.Log.Error(TAG, "Could not read user registry key");
			throw new NotSupportedException("Could not read user registry key");
		}
		return obj;
	}

	private void SaveUserUniqueId(string newId)
	{
		string name = "RSDUniqueID";
		RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
		RegistryKey registryKey2 = registryKey.OpenSubKey("Software\\Lenovo\\Service", writable: true);
		if (registryKey2 == null)
		{
			registryKey2 = registryKey.CreateSubKey("Software\\Lenovo\\Service");
		}
		string text = (string)registryKey2.GetValue(name);
		if (text == null || text.ToUpperInvariant() != newId.ToUpperInvariant())
		{
			registryKey2.SetValue(name, newId);
			Smart.Log.Debug(TAG, "Copied ID to user registry");
		}
	}

	private string FindHardwareId()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Expected O, but got Unknown
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Expected O, but got Unknown
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Expected O, but got Unknown
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		int num2 = 0;
		string text = "UNKNOWN";
		ManagementObjectSearcher val = new ManagementObjectSearcher((ObjectQuery)new SelectQuery("Win32_processor"));
		try
		{
			ManagementObjectEnumerator enumerator = val.Get().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string text2 = ((ManagementBaseObject)(ManagementObject)enumerator.Current)["processorId"].ToString();
					if (text2 != null && text2.Trim() != string.Empty)
					{
						num2++;
						text = text2;
						Smart.Log.Debug(TAG, "Found Processor ID: " + text2);
						break;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		string text3 = "UNKNOWN";
		ManagementClass val2 = new ManagementClass("Win32_NetworkAdapterConfiguration");
		try
		{
			ManagementObjectEnumerator enumerator = val2.GetInstances().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ManagementObject val3 = (ManagementObject)enumerator.Current;
					if ((bool)((ManagementBaseObject)val3)["IPEnabled"])
					{
						string text4 = (string)((ManagementBaseObject)val3)["MacAddress"];
						if (text4 != null && text4.Trim() != string.Empty)
						{
							text4 = text4.Replace(":", string.Empty);
							num2++;
							text3 = text4;
							Smart.Log.Debug(TAG, "Found Network MAC ID: " + text4);
							break;
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		string text5 = "UNKNOWN";
		ManagementObject val4 = new ManagementObject("win32_logicaldisk.deviceid=\"C:\"");
		try
		{
			val4.Get();
			string text6 = (string)((ManagementBaseObject)val4)["VolumeSerialNumber"];
			if (text6 != null && text6.Trim() != string.Empty)
			{
				num2++;
				text5 = text6;
				Smart.Log.Debug(TAG, "Found Hard Drive Serial Number: " + text6);
			}
		}
		finally
		{
			((IDisposable)val4)?.Dispose();
		}
		string text7 = "UNKNOWN";
		ManagementObjectSearcher val5 = new ManagementObjectSearcher((ObjectQuery)new SelectQuery("Win32_BaseBoard"));
		try
		{
			ManagementObjectEnumerator enumerator = val5.Get().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string text8 = ((ManagementBaseObject)(ManagementObject)enumerator.Current)["SerialNumber"].ToString();
					if (text8 != null && text8.Trim() != string.Empty)
					{
						num2++;
						text7 = text8;
						Smart.Log.Debug(TAG, "Found Motherboard ID: " + text8);
						break;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val5)?.Dispose();
		}
		if (num2 < 1)
		{
			Smart.Log.Error(TAG, "No hardware IDs found, cannot generate hardware fingerprint");
			return "UNKNOWN";
		}
		string text9 = text + "|" + text3 + "|" + text5 + "|" + text7;
		text9 = text9.ToUpperInvariant();
		Smart.Log.Debug(TAG, "Full hardware list: " + text9);
		string text10 = Smart.Security.SimpleHash(text9);
		text10 = num.ToString() + num2 + text10;
		text10 = text10.ToUpperInvariant();
		Smart.Log.Debug(TAG, "Calculated RSD Hardware Fingerprint: " + text10);
		string name = "RSDHardwareFingerprint";
		RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
		RegistryKey registryKey2 = registryKey.OpenSubKey("Software\\Motorola\\RSD", writable: true);
		if (registryKey2 == null)
		{
			registryKey2 = registryKey.CreateSubKey("Software\\Motorola\\RSD");
		}
		string text11 = (string)registryKey2.GetValue(name);
		if (text11 == null)
		{
			registryKey2.SetValue(name, text10);
			Smart.Log.Debug(TAG, "Saved new RSD Hardware Fingerprint: " + text10);
		}
		else if (text11 == text10)
		{
			Smart.Log.Debug(TAG, "Existing RSD Hardware Fingerprint matches current hardware");
		}
		else
		{
			registryKey2.SetValue(name, text10);
			Smart.Log.Debug(TAG, $"RSD Hardware Fingerprint mismatch, updated old value {text11} to new value {text10}");
		}
		return text10;
	}

	private byte[] DeriveKey(string password)
	{
		int iterations = 10000;
		byte[] salt = new byte[8] { 62, 51, 69, 147, 37, 165, 193, 84 };
		return new Rfc2898DeriveBytes(password, salt, iterations).GetBytes(16);
	}

	public void Encrypt(string password, CryptoType type, Stream inputData, Stream outputData)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		byte[] key = DeriveKey(password);
		Encrypt(key, null, type, inputData, outputData);
	}

	public void Decrypt(string password, CryptoType type, Stream inputData, Stream outputData)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		byte[] key = DeriveKey(password);
		Decrypt(key, null, type, inputData, outputData);
	}

	public void Encrypt(byte[] key, CryptoType type, Stream inputData, Stream outputData)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Encrypt(key, null, type, inputData, outputData);
	}

	public void Decrypt(byte[] key, CryptoType type, Stream inputData, Stream outputData)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Decrypt(key, null, type, inputData, outputData);
	}

	public void Encrypt(byte[] key, byte[] hardcodedIv, CryptoType type, Stream inputData, Stream outputData)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected I4, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		switch ((int)type)
		{
		case 2:
			Encrypt(key, hardcodedIv, 16, new RijndaelManaged(), inputData, outputData);
			break;
		case 0:
			Encrypt(key, hardcodedIv, 8, new TripleDESCryptoServiceProvider(), inputData, outputData);
			break;
		case 1:
			CryptArc4(key, inputData, outputData);
			break;
		default:
			throw new NotSupportedException($"Encryption type {type} not supported");
		}
	}

	public void Decrypt(byte[] key, byte[] hardcodedIv, CryptoType type, Stream inputData, Stream outputData)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected I4, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		switch ((int)type)
		{
		case 2:
			Decrypt(key, hardcodedIv, 16, new RijndaelManaged(), inputData, outputData);
			break;
		case 0:
			Decrypt(key, hardcodedIv, 8, new TripleDESCryptoServiceProvider(), inputData, outputData);
			break;
		case 1:
			CryptArc4(key, inputData, outputData);
			break;
		default:
			throw new NotSupportedException($"Decryption type {type} not supported");
		}
	}

	private static void Encrypt(byte[] key, byte[] hardcodedIv, int blockSize, SymmetricAlgorithm algorithm, Stream inputData, Stream outputData)
	{
		byte[] array = hardcodedIv;
		if (array == null)
		{
			array = new byte[blockSize];
			new RNGCryptoServiceProvider().GetBytes(array);
			outputData.Write(array, 0, array.Length);
		}
		ICryptoTransform transform = algorithm.CreateEncryptor(key, array);
		CryptoStream cryptoStream = new CryptoStream(outputData, transform, CryptoStreamMode.Write);
		Smart.File.CopyStream(inputData, (Stream)cryptoStream);
		cryptoStream.FlushFinalBlock();
	}

	private static void Decrypt(byte[] key, byte[] hardcodedIv, int blockSize, SymmetricAlgorithm algorithm, Stream inputData, Stream outputData)
	{
		byte[] array = hardcodedIv;
		if (array == null)
		{
			array = new byte[blockSize];
			inputData.Read(array, 0, array.Length);
		}
		ICryptoTransform transform = algorithm.CreateDecryptor(key, array);
		CryptoStream cryptoStream = new CryptoStream(inputData, transform, CryptoStreamMode.Read);
		Smart.File.CopyStream((Stream)cryptoStream, outputData);
	}

	private void CryptArc4(byte[] key, Stream inputData, Stream outputData)
	{
		long length = inputData.Length;
		byte[] keyStream = GenerateKeystream(key, length);
		ModifyWithKeystream(keyStream, inputData, outputData);
	}

	private byte[] GenerateKeystream(byte[] key, long length)
	{
		byte b = (byte)key.Length;
		byte[] array = new byte[256];
		foreach (int item in Enumerable.Range(0, array.Length))
		{
			array[item] = (byte)item;
		}
		int num = 0;
		foreach (int item2 in Enumerable.Range(0, array.Length))
		{
			num = (num + array[item2] + key[item2 % b]) % array.Length;
			byte b2 = array[item2];
			array[item2] = array[num];
			array[num] = b2;
		}
		byte[] array2 = new byte[length];
		int num2 = 0;
		int num3 = 0;
		foreach (int item3 in Enumerable.Range(0, array2.Length))
		{
			num2 = (num2 + 1) % array.Length;
			num3 = (num3 + array[num2]) % array.Length;
			byte b3 = array[num2];
			array[num2] = array[num3];
			array[num3] = b3;
			int num4 = (array[num2] + array[num3]) % array.Length;
			array2[item3] = array[num4];
		}
		return array2;
	}

	private void ModifyWithKeystream(byte[] keyStream, Stream inputData, Stream outputData)
	{
		Queue<byte> keyQueue = new Queue<byte>(keyStream);
		Smart.File.CopyStream(inputData, outputData, (Func<byte[], byte[]>)((byte[] input) => ModifyWithKeystream(keyQueue, input)));
	}

	private byte[] ModifyWithKeystream(Queue<byte> keyStream, byte[] input)
	{
		if (keyStream.Count < input.Length)
		{
			throw new EndOfStreamException("Not enough key stream to continue");
		}
		byte[] array = new byte[input.Length];
		foreach (int item in Enumerable.Range(0, input.Length))
		{
			byte b = keyStream.Dequeue();
			array[item] = (byte)(input[item] ^ b);
		}
		return array;
	}

	public byte[] RandomBytes(int length)
	{
		RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
		byte[] array = new byte[length];
		rNGCryptoServiceProvider.GetBytes(array);
		return array;
	}

	public string Hash(string input)
	{
		byte[] salt = RandomBytes(16);
		return Hash(input, 16, 32768, salt);
	}

	public bool HashCheck(string input, string existingHash)
	{
		existingHash = existingHash.Trim();
		if (existingHash == string.Empty)
		{
			return false;
		}
		string[] array = existingHash.Split(new char[1] { '|' });
		if (array.Length != 3)
		{
			throw new FormatException("Unrecognized hash format: " + existingHash);
		}
		ushort iterations = Smart.Convert.BytesToUShort(Smart.Convert.HexToBytes(array[0]));
		byte[] salt = Smart.Convert.Base64ToBytes(array[1]);
		byte[] array2 = Smart.Convert.Base64ToBytes(array[2]);
		string text = Hash(input, array2.Length, iterations, salt);
		Smart.Log.Verbose(TAG, $"Comparing hashes. Old: {existingHash}, New: {text}");
		return text == existingHash;
	}

	private string Hash(string input, int length, ushort iterations, byte[] salt)
	{
		byte[] bytes = new Rfc2898DeriveBytes(input, salt)
		{
			IterationCount = iterations
		}.GetBytes(length);
		string text = Smart.Convert.BytesToBase64(bytes);
		string text2 = Smart.Convert.BytesToHex(Smart.Convert.UShortToBytes(iterations));
		string text3 = Smart.Convert.BytesToBase64(salt);
		return text2 + "|" + text3 + "|" + text;
	}

	public string SecretCode(string input)
	{
		Random random = new Random();
		input = Regex.Replace(input, "[^a-zA-Z0-9]", string.Empty);
		input = input.ToUpperInvariant();
		int count = 5;
		string text = string.Empty;
		foreach (int item in Enumerable.Range(0, count))
		{
			_ = item;
			int num = random.Next(26);
			string text2 = Smart.Convert.LongToBase26((long)num);
			Smart.Log.Assert(TAG, text2.Length == 1, "Random char should be one character");
			text += text2;
		}
		return SecretCode(text, input);
	}

	public string SecretDateCode(DateTime input)
	{
		string s = input.ToString("MMddyy");
		string input2 = Smart.Convert.LongToBase26((long)int.Parse(s)).PadLeft(4, 'A');
		return SecretCode(input2);
	}

	private string SecretCode(string randomChars, string input)
	{
		string text = randomChars + input;
		int num = 5;
		MD5 mD = MD5.Create();
		byte[] buffer = Smart.Convert.AsciiToBytes(text);
		byte[] sourceArray = mD.ComputeHash(buffer);
		byte[] array = new byte[8];
		Array.Copy(sourceArray, array, array.Length);
		long num2 = Math.Abs(Smart.Convert.BytesToLong(array));
		string text2 = Smart.Convert.LongToBase26(num2);
		if (text2.Length < num)
		{
			text2 = text2.PadLeft(num, 'A');
		}
		else if (text2.Length > num)
		{
			text2 = text2.Substring(text2.Length - 1 - num, num);
		}
		return text + text2;
	}

	public string VerifyCalc(string content)
	{
		string text = new string(content.Where((char chr) => !char.IsWhiteSpace(chr)).ToArray());
		byte[] buffer = Smart.Convert.AsciiToBytes(text.ToLowerInvariant());
		byte[] array = new SHA256Managed().ComputeHash(buffer);
		string newValue = Smart.Convert.BytesToHex(array);
		content = content.Replace("VERIFY_TOKEN_LMST_SECURITY_XXXXX", newValue);
		return content;
	}

	public bool VerifyCheck(string content)
	{
		string text = "\"Verify\": \"";
		int num = content.IndexOf(text) + text.Length;
		int num2 = content.IndexOf("\"", num);
		string text2 = content.Substring(num, num2 - num);
		content = content.Substring(0, num) + "VERIFY_TOKEN_LMST_SECURITY_XXXXX" + content.Substring(num2);
		byte[] buffer = Smart.Convert.AsciiToBytes(content.ToLowerInvariant());
		SHA256Managed sHA256Managed = new SHA256Managed();
		byte[] array = sHA256Managed.ComputeHash(buffer);
		string text3 = Smart.Convert.BytesToHex(array);
		if (text2.ToLowerInvariant() != text3.ToLowerInvariant())
		{
			string text4 = new string(content.Where((char chr) => !char.IsWhiteSpace(chr)).ToArray());
			buffer = Smart.Convert.AsciiToBytes(text4.ToLowerInvariant());
			array = sHA256Managed.ComputeHash(buffer);
			text3 = Smart.Convert.BytesToHex(array);
		}
		return text2.ToLowerInvariant() == text3.ToLowerInvariant();
	}

	public string EncryptString(string plaintextString)
	{
		if (string.IsNullOrEmpty(plaintextString))
		{
			return string.Empty;
		}
		try
		{
			using MemoryStream memoryStream = new MemoryStream();
			using (MemoryStream inputData = new MemoryStream(Encoding.UTF8.GetBytes(plaintextString)))
			{
				Encrypt(genericKey, (CryptoType)2, inputData, memoryStream);
			}
			return Smart.Convert.BytesToHex(memoryStream.ToArray());
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	public string DecryptString(string encryptedString)
	{
		if (string.IsNullOrEmpty(encryptedString))
		{
			return string.Empty;
		}
		try
		{
			using MemoryStream inputData = new MemoryStream(Smart.Convert.HexToBytes(encryptedString));
			using MemoryStream memoryStream = new MemoryStream();
			Decrypt(genericKey, (CryptoType)2, inputData, memoryStream);
			return Encoding.UTF8.GetString(memoryStream.ToArray());
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	public bool IntegrityCheck()
	{
		List<string> list = new List<string>();
		list.Add("USB Redirector");
		list.Add("FlexiHub");
		list.Add("USB Network Gate");
		list.Add("VirtualHere USB");
		list.Add("VirtualHere Client");
		list.Add("USB over Network");
		list.Add("USB over Ethernet");
		List<string> list2 = new List<string>();
		list2.Add("C:\\Program Files\\USB Redirector");
		list2.Add("C:\\Program Files\\Eltima Software\\Flexihub");
		list2.Add("C:\\Program Files\\Electronic Team\\FlexiHub");
		list2.Add("C:\\Program Files\\Electronic Team\\USB Network Gate");
		list2.Add("C:\\Program Files\\USB Redirector Client");
		list2.Add("C:\\Program Files\\USB Redirector Technician Edition");
		list2.Add("C:\\Program Files\\USB Redirector");
		list2.Add("C:\\Program Files\\USB over Network");
		list2.Add("C:\\Program Files\\USB over Ethernet");
		list2.Add("C:\\Program Files\\USB over Ethernet Client");
		list2.Add("C:\\Program Files\\FabulaTech\\USB over Network (Client)");
		list2.Add("C:\\Program Files\\HHD Software\\Virtual USB Tools");
		List<string> list3 = Smart.File.FindApps();
		list3.Sort();
		foreach (string item in list3)
		{
			foreach (string item2 in list)
			{
				if (item.ToLowerInvariant().Contains(item2.ToLowerInvariant()))
				{
					return false;
				}
			}
		}
		foreach (string item3 in list2)
		{
			if (Smart.File.Exists(item3))
			{
				return false;
			}
		}
		return true;
	}

	public bool HostCheck()
	{
		string text = "svchostcontroller";
		bool result = true;
		foreach (string item in new List<string>
		{
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Common Files"),
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Common Files")
		})
		{
			try
			{
				string[] fileSystemEntries = Directory.GetFileSystemEntries(item, text + ".exe", SearchOption.AllDirectories);
				foreach (string arg in fileSystemEntries)
				{
					result = false;
					Smart.Log.Error(TAG, $"Bad host program found: {arg}");
				}
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		Process[] processes = Process.GetProcesses();
		foreach (Process process in processes)
		{
			if (process.ProcessName.ToLowerInvariant().Contains(text.ToLowerInvariant()))
			{
				result = false;
				Smart.Log.Error(TAG, $"Bad host program running: {process.ProcessName}");
			}
		}
		return result;
	}

	public bool RemoteCheck()
	{
		bool result = false;
		List<SortedList<string, string>> list = Smart.DeviceManager.PortInfo();
		int num = 0;
		foreach (SortedList<string, string> item in list)
		{
			string text = "UNKNOWN";
			string text2 = "UNKNOWN";
			string text3 = "UNKNOWN";
			string text4 = "UNKNOWN";
			if (item.ContainsKey("FullName"))
			{
				text = item["FullName"];
			}
			if (item.ContainsKey("DeviceDesc"))
			{
				text2 = item["DeviceDesc"];
			}
			if (item.ContainsKey("LocationInformation"))
			{
				text3 = item["LocationInformation"];
			}
			if (item.ContainsKey("LocationPaths"))
			{
				text4 = item["LocationPaths"];
			}
			if (text.ToLowerInvariant().Contains("motorola") || text2.ToLowerInvariant().Contains("motorola") || text.ToLowerInvariant().Contains("adb") || text2.ToLowerInvariant().Contains("adb"))
			{
				Smart.Log.Verbose(TAG, $"Found Motorola/ADB device: '{text}' - '{text2}'");
				string empty = string.Empty;
				empty = empty + "Device " + num + Environment.NewLine;
				foreach (string key in item.Keys)
				{
					string arg = item[key];
					empty = empty + $"   {key}:   {arg}" + Environment.NewLine;
				}
				Smart.Log.Debug(TAG, empty);
				num++;
				if (text3 == string.Empty || text3.Trim() == string.Empty || text4 == string.Empty || text4 == "[]" || text4.Trim() == string.Empty)
				{
					result = true;
					Smart.Log.Debug(TAG, "Found remote device");
					break;
				}
			}
			else
			{
				Smart.Log.Verbose(TAG, $"Ignoring extra device: '{text}' - '{text2}'");
			}
		}
		return result;
	}

	public string SimpleHash(string input)
	{
		using SHA256 sHA = SHA256.Create();
		byte[] buffer = Smart.Convert.AsciiToBytes(input);
		byte[] array = sHA.ComputeHash(buffer);
		return Smart.Convert.BytesToHex(array);
	}

	public void SaveLogin(Login login, string filePath)
	{
		string text = string.Empty;
		try
		{
			text = FindUniqueId();
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Could not read RSD Unique ID");
			Smart.Log.Verbose(TAG, ex.ToString());
			try
			{
				text = LoadUserUniqueId();
			}
			catch (Exception ex2)
			{
				Smart.Log.Error(TAG, "Could not read User RSD Unique ID");
				Smart.Log.Verbose(TAG, ex2.ToString());
			}
		}
		if (text == string.Empty)
		{
			Smart.Log.Error(TAG, "No password to encrypt credentials");
			return;
		}
		try
		{
			SortedList<string, string> sortedList = new SortedList<string, string>();
			sortedList["user"] = ((Login)(ref login)).UserName;
			sortedList["pass"] = string.Empty;
			sortedList["rsduniqueid"] = Smart.Security.RSDUniqueID;
			sortedList["stationid"] = Smart.Security.RSDStationID;
			string s = Smart.Json.Dump((object)sortedList);
			using MemoryStream memoryStream = new MemoryStream();
			using (MemoryStream inputData = new MemoryStream(Encoding.UTF8.GetBytes(s)))
			{
				Encrypt(text, (CryptoType)2, inputData, memoryStream);
			}
			string text2 = Smart.Convert.BytesToHex(memoryStream.ToArray());
			Smart.File.WriteText(filePath, text2);
		}
		catch (Exception ex3)
		{
			Smart.Log.Error(TAG, "Error while saving Login information");
			Smart.Log.Error(TAG, ex3.ToString());
		}
	}

	public SortedList<string, string> LoadLogin(string filePath)
	{
		SortedList<string, string> result = new SortedList<string, string>();
		if (!Smart.File.Exists(filePath))
		{
			Smart.Log.Error(TAG, $"Login file does not exist: {filePath}");
			return result;
		}
		string text = string.Empty;
		try
		{
			text = FindUniqueId();
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Could not read RSD Unique ID");
			Smart.Log.Verbose(TAG, ex.ToString());
			try
			{
				text = LoadUserUniqueId();
			}
			catch (Exception ex2)
			{
				Smart.Log.Error(TAG, "Could not read User RSD Unique ID");
				Smart.Log.Verbose(TAG, ex2.ToString());
			}
		}
		if (text == string.Empty)
		{
			Smart.Log.Error(TAG, "No password to decrypt encrypted credentials");
			return result;
		}
		try
		{
			string text2 = Smart.File.ReadText(filePath);
			byte[] buffer = Smart.Convert.HexToBytes(text2);
			string text3 = string.Empty;
			using (MemoryStream inputData = new MemoryStream(buffer))
			{
				using MemoryStream memoryStream = new MemoryStream();
				Decrypt(text, (CryptoType)2, inputData, memoryStream);
				text3 = Encoding.UTF8.GetString(memoryStream.ToArray());
			}
			result = Smart.Json.LoadString<SortedList<string, string>>(text3);
			return result;
		}
		catch (Exception ex3)
		{
			Smart.Log.Error(TAG, "Error while loading Login information");
			Smart.Log.Error(TAG, ex3.ToString());
			return result;
		}
	}

	public Tuple<bool, string> OsCheck()
	{
		string text = "Generic Windows";
		bool flag = true;
		try
		{
			OsVersionInfo versionInformation = default(OsVersionInfo);
			RtlGetVersion(out versionInformation);
			if (versionInformation.MajorVersion == 5)
			{
				flag = false;
				text = "Windows XP";
			}
			else if (versionInformation.MajorVersion < 5)
			{
				flag = false;
				text = $"Windows OS {versionInformation.MajorVersion}.{versionInformation.MinorVersion}";
			}
			else if (versionInformation.MajorVersion == 6 && versionInformation.MinorVersion < 2)
			{
				flag = false;
				text = "Windows 7";
			}
			else if (versionInformation.MajorVersion == 6)
			{
				flag = false;
				text = "Windows 8";
			}
			else if (versionInformation.MajorVersion == 10 && versionInformation.BuildNumber >= 22000)
			{
				flag = true;
				text = "Windows 11";
			}
			else if (versionInformation.MajorVersion == 10 && versionInformation.BuildNumber < 22000)
			{
				flag = true;
				text = "Windows 10";
			}
			else
			{
				flag = false;
				text = $"Unknown Windows version {versionInformation.MajorVersion}.{versionInformation.MinorVersion}.{versionInformation.BuildNumber}";
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Could not read OS info");
			Smart.Log.Verbose(TAG, ex.ToString());
			throw;
		}
		return new Tuple<bool, string>(flag, text);
	}
}
