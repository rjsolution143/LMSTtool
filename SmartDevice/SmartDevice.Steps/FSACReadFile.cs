using System;
using System.Globalization;
using System.IO;
using ISmart;

namespace SmartDevice.Steps;

public class FSACReadFile : TestCommandFSAC
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Invalid comparison between Unknown and I4
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_074c: Invalid comparison between Unknown and I4
		//IL_0785: Unknown result type (might be due to invalid IL or missing references)
		//IL_078b: Invalid comparison between Unknown and I4
		//IL_076b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c4: Unknown result type (might be due to invalid IL or missing references)
		string text = "C:\\";
		if (((dynamic)base.Info.Args).PcFolderToSave != null)
		{
			text = ((dynamic)base.Info.Args).PcFolderToSave;
			if (text.StartsWith("$"))
			{
				string key = text.Substring(1);
				text = base.Cache[key];
			}
		}
		if (text.Contains("{CommonStorageDir}"))
		{
			string commonStorageDir = Smart.File.CommonStorageDir;
			text = text.Replace("{CommonStorageDir}", commonStorageDir);
		}
		if (text.Contains("{$sku}"))
		{
			string newValue = base.Cache["sku"];
			text = text.Replace("{$sku}", newValue);
		}
		if (text.Contains("{datetime}"))
		{
			string newValue2 = DateTime.Now.ToString("yyyyMMddHHmmss");
			text = text.Replace("{datetime}", newValue2);
		}
		if (!Smart.File.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		int num = 1024;
		if (((dynamic)base.Info.Args).PacketSize != null)
		{
			num = ((dynamic)base.Info.Args).PacketSize;
		}
		base.ResultLogged = false;
		base.Action = FSAAction.Open;
		base.Run();
		if ((int)base.TestResult != 8)
		{
			Smart.Log.Error(TAG, string.Format("Failed to open file {0}", ((dynamic)base.Info.Args).FileName));
			LogResult(base.TestResult);
			return;
		}
		string text2 = base.FileName;
		if (base.FileName.Contains("/"))
		{
			int num2 = base.FileName.LastIndexOf("/");
			text2 = base.FileName.Substring(num2 + 1);
		}
		string text3 = Smart.File.PathJoin(text, text2);
		Smart.Log.Error(TAG, $"File will be saved to {text3}");
		base.Cache["fileSaved"] = text3;
		base.Action = FSAAction.Read;
		base.TestResult = (Result)8;
		using (Stream stream = Smart.File.WriteStream(text3, FileMode.Create))
		{
			ITestCommandResponse val = null;
			do
			{
				string text4 = "00010001" + num.ToString("X8", CultureInfo.InvariantCulture);
				val = base.tcmd.SendCommand("004A", text4);
				if (val.Failed || val.DataHex.Length < 2)
				{
					base.TestResult = (Result)1;
				}
				if (val.DataHex.Length > 2)
				{
					byte[] array = Smart.Convert.HexToBytes(val.DataHex);
					stream.Write(array, 0, array.Length);
					continue;
				}
				string dataHex = val.DataHex;
				switch (dataHex)
				{
				case "01":
				case "09":
				case "0B":
					Smart.Log.Error(TAG, "Read Error " + dataHex);
					base.TestResult = (Result)1;
					break;
				}
			}
			while (val != null && val.DataHex.Length > 2);
		}
		if ((int)base.TestResult != 8)
		{
			Smart.Log.Error(TAG, $"Failed to read to file {text3}");
			LogResult(base.TestResult);
			return;
		}
		base.Action = FSAAction.Close;
		base.Run();
		if ((int)base.TestResult != 8)
		{
			Smart.Log.Error(TAG, string.Format("Failed to close file {0}", ((dynamic)base.Info.Args).FileName));
		}
		LogResult(base.TestResult);
	}
}
