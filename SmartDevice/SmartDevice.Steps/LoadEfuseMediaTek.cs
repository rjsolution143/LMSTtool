using System;
using System.IO;
using System.Xml;
using ISmart;

namespace SmartDevice.Steps;

public class LoadEfuseMediaTek : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7c: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		Result result = (Result)8;
		string text = "efuseMediaTek";
		string filePathName = Smart.Rsd.GetFilePathName(text, base.Recipe.Info.UseCase, val);
		Smart.Log.Debug(TAG, $"file symbol: {text} maps to path: {filePathName}");
		if (!Smart.File.Exists(filePathName))
		{
			Smart.Log.Error(TAG, $"Symbol: {text} => file: {filePathName} does not exist");
			LogResult((Result)1, "Requested file not found", $"Symbol: {text} => file: {filePathName} does not exist");
			return;
		}
		string text2 = File.ReadAllText(filePathName);
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(text2);
		XmlNode xmlNode = xmlDocument.SelectSingleNode("/flashtool-config/general/rom-list");
		XmlNodeList xmlNodeList = xmlNode.SelectNodes("rom");
		if (text2.Contains("scatter_file"))
		{
			string text3 = "MT6765";
			if (((dynamic)base.Info.Args).ChipSet != null)
			{
				text3 = ((dynamic)base.Info.Args).ChipSet;
			}
			if (text3.StartsWith("$"))
			{
				string key = text3.Substring(1);
				text3 = base.Cache[key];
			}
			string text4 = ((dynamic)base.Info.Args).DownloadAgent;
			if (text4.StartsWith("$"))
			{
				string key2 = text4.Substring(1);
				text4 = base.Cache[key2];
			}
			Smart.Log.Verbose(TAG, "downloadAgent " + text4);
			string text5 = "MT6765_Android_scatter.txt";
			if (((dynamic)base.Info.Args).ScatterFile != null && ((dynamic)base.Info.Args).ScatterFile != string.Empty)
			{
				text5 = ((dynamic)base.Info.Args).ScatterFile;
			}
			if (text5.StartsWith("$"))
			{
				string key3 = text5.Substring(1);
				text5 = base.Cache[key3];
			}
			Smart.Log.Verbose(TAG, "scatter " + text5);
			string text7;
			if (((dynamic)base.Info.Args).PreloaderBinFile == null)
			{
				string text6 = ((dynamic)base.Info.Args).PreloaderBinFormat;
				if (text6.StartsWith("$"))
				{
					string key4 = text6.Substring(1);
					text6 = base.Cache[key4];
				}
				string[] files = Directory.GetFiles(Path.GetDirectoryName(xmlNodeList[0].InnerText.Trim()), text6);
				if (files.Length == 0)
				{
					Smart.Log.Error(TAG, "No efuse preloader matched the pattern " + text6);
					throw new Exception("No efuse preloader matched the pattern " + text6);
				}
				text7 = Path.GetFileName(files[0]);
			}
			else
			{
				text7 = ((dynamic)base.Info.Args).PreloaderBinFile;
				if (text7.StartsWith("$"))
				{
					string key5 = text7.Substring(1);
					text7 = base.Cache[key5];
				}
			}
			Smart.Log.Verbose(TAG, "preloaderBinFile " + text7);
			string text8 = ((dynamic)base.Info.Args).SbcPublicKey;
			if (text8.StartsWith("$"))
			{
				string key6 = text8.Substring(1);
				text8 = base.Cache[key6];
			}
			text2 = text2.Replace("chipset", text3);
			text2 = text2.Replace("downloadAgent", text4);
			text2 = text2.Replace("scatter_file", text5);
			text2 = text2.Replace("preloader_bin_file", text7);
			text2 = text2.Replace("sbc_public_key_n", text8);
			File.WriteAllText(filePathName, text2);
			xmlDocument.LoadXml(text2);
			xmlNode = xmlDocument.SelectSingleNode("/flashtool-config/general/rom-list");
			xmlNodeList = xmlNode.SelectNodes("rom");
		}
		string text9 = xmlDocument.SelectSingleNode("/flashtool-config/general/download-agent").InnerText.Trim();
		if (!File.Exists(text9))
		{
			Smart.Log.Error(TAG, $"Download agent file {text9} does not exist");
			result = (Result)1;
		}
		string text10 = xmlDocument.SelectSingleNode("/flashtool-config/general/scatter").InnerText.Trim();
		if (!File.Exists(text10))
		{
			Smart.Log.Error(TAG, $"Scatter file {text10} does not exist");
			result = (Result)1;
		}
		if (xmlNode != null)
		{
			foreach (XmlNode item in xmlNodeList)
			{
				string text11 = item.InnerText.Trim();
				if (!File.Exists(text11))
				{
					Smart.Log.Error(TAG, $"ROM file {text11} does not exist");
					result = (Result)1;
					break;
				}
			}
		}
		string key7 = "efuse";
		if (((dynamic)base.Info.Args).File != null)
		{
			key7 = ((dynamic)base.Info.Args).File;
		}
		base.Cache[key7] = filePathName;
		VerifyOnly(ref result);
		LogResult(result);
	}
}
