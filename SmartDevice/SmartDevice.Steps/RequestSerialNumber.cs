using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class RequestSerialNumber : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Invalid comparison between Unknown and I4
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Invalid comparison between Unknown and I4
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ee: Invalid comparison between Unknown and I4
		//IL_08f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d77: Invalid comparison between Unknown and I4
		//IL_0911: Unknown result type (might be due to invalid IL or missing references)
		//IL_092d: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		bool flag = false;
		if (((dynamic)base.Info.Args).Dual != null)
		{
			flag = ((dynamic)base.Info.Args).Dual;
		}
		base.Cache["dualSim"] = flag;
		SortedList<string, string> sortedList = FindPCBARow();
		if (sortedList == null)
		{
			return;
		}
		string text = sortedList["UpdCustomer"];
		string text2 = sortedList["Ulma"];
		string text3 = sortedList["Gppd"];
		string text4 = sortedList["BuildType"];
		string text5 = sortedList["Protocol"];
		string text6 = sortedList["boardAssembly"];
		string text7 = val.ID;
		string empty = string.Empty;
		string text8 = ((val.Group == string.Empty) ? val.SerialNumber : val.GetLogInfoValue("IMEI"));
		string text9 = string.Empty;
		base.Cache["SerialNumberIn"] = text8;
		if ((int)val.Type == 2)
		{
			string[] array = text3.Split(new char[1] { ',' });
			string[] array2 = text.Split(new char[1] { ',' });
			if (val.Group == string.Empty)
			{
				text7 = ((val.PSN.Trim().Length > 10) ? val.PSN.Trim().Substring(val.PSN.Trim().Length - 10, 10) : text7);
			}
			if (text7.Trim() == string.Empty && val.Log.Info.ContainsKey("TrackId") && val.Log.Info["TrackId"] != string.Empty)
			{
				text7 = val.Log.Info["TrackId"];
			}
			text9 = Smart.Web.PcbaSerialNumberRequest("IMEI", array2[0], text2, array[0], text4, text5, text6, text7, empty);
			base.Cache["GSNOut"] = text9;
			base.Cache["updatedStatus"] = false;
			val.Log.AddInfo("DispatchedGSN", text9);
			Smart.Log.Debug(TAG, "GSNOut: " + text9);
			if (val.WiFiOnlyDevice)
			{
				base.Cache["SerialNumberOut"] = text9;
				LogPass();
				return;
			}
			if (array.Length > 1)
			{
				text3 = array[1];
			}
			if (array2.Length > 1)
			{
				text = array2[1];
			}
		}
		string text10 = string.Empty;
		if (flag)
		{
			text10 = val.SerialNumber2;
			base.Cache["SerialNumberInDual"] = text10;
		}
		SerialNumberType val2 = Smart.Convert.ToSerialNumberType(text8);
		string text11 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
		if ((int)val.Type == 1)
		{
			string text12 = text11;
			val2 = (SerialNumberType)1;
			if (text12 != ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant())
			{
				string text13 = text11;
				val2 = (SerialNumberType)2;
				if (text13 != ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant())
				{
					val2 = (SerialNumberType)1;
					text11 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
				}
			}
		}
		Smart.Log.Debug(TAG, $"existingSerialNumber = {text8}, SnType = {text11}");
		if (((dynamic)base.Info.Args).Sn1Type != null && ((dynamic)base.Info.Args).Sn1Type != string.Empty)
		{
			text11 = ((dynamic)base.Info.Args).Sn1Type.ToString();
		}
		string text14 = Smart.Web.PcbaSerialNumberRequest(text11, text, text2, text3, text4, text5, text6, text7, empty);
		base.Cache["updatedStatus"] = false;
		base.Cache["SerialNumberOut"] = text14;
		val.Log.AddInfo("DispatchedSn", text14);
		string text15 = string.Empty;
		if (flag)
		{
			val2 = Smart.Convert.ToSerialNumberType(text8);
			text11 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
			if ((int)val.Type == 1)
			{
				string text16 = text11;
				val2 = (SerialNumberType)1;
				if (text16 != ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant())
				{
					string text17 = text11;
					val2 = (SerialNumberType)2;
					if (text17 != ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant())
					{
						val2 = (SerialNumberType)1;
						text11 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
					}
				}
			}
			Smart.Log.Debug(TAG, $"existingSerialNumber = {text8}, SnType = {text11}");
			if (((dynamic)base.Info.Args).Sn2Type != null && ((dynamic)base.Info.Args).Sn2Type != string.Empty)
			{
				text11 = ((dynamic)base.Info.Args).Sn2Type.ToString();
			}
			text15 = Smart.Web.PcbaSerialNumberRequest(text11, text, "0", text3, text4, text5, text6, text7, empty);
			base.Cache["SerialNumberOutDual"] = text15;
			val.Log.AddInfo("DispatchedSn2", text15);
			Smart.Web.DualConnection(text14, text15, text7);
			Smart.Log.Info(TAG, $"Binding IMEI: {text14} and IMEI2: {text15} using TrackId: {text7}");
		}
		Smart.Log.Debug(TAG, "SerialNumberIn: " + text8);
		if (flag)
		{
			Smart.Log.Debug(TAG, "SerialNumberInDual: " + text10);
		}
		Smart.Log.Debug(TAG, "SerialNumberOut: " + text14);
		if (flag)
		{
			Smart.Log.Debug(TAG, "SerialNumberOutDual: " + text15);
		}
		if ((int)val.Type == 2)
		{
			Smart.Web.DualConnectionGsn(text14, text9, text7);
			Smart.Log.Info(TAG, $"Binding IMEI: {text14} and GSN: {text9} using TrackId: {text7}");
		}
		val.SerialNumber = text14;
		LogPass();
	}

	protected SortedList<string, string> FindPCBARow()
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		bool flag = default(bool);
		string value = Smart.Rsd.GetValue("phoneModel", base.Recipe.Info.UseCase, device, ref flag, false);
		string[] array = device.ModelId.Split(new char[1] { '|' });
		string text = "UNKNOWN";
		if (array.Length > 1)
		{
			text = array[1];
		}
		List<string> pcbaFilePathNames = Smart.Rsd.GetPcbaFilePathNames();
		string text2 = string.Empty;
		string text3 = string.Empty;
		device.Log.Info.TryGetValue("pcb-part-no", out var value2);
		for (int i = 0; i < pcbaFilePathNames.Count; i++)
		{
			string text4 = Smart.Rsd.ReadPcbaFileContent(pcbaFilePathNames[i]);
			ICsvFile val = Smart.NewCsvFile();
			val.Load(text4, ',');
			if (string.IsNullOrEmpty(value2))
			{
				string text5 = Smart.Locale.Xlate("Please scan or type the PCBA Assembly Part Number of the device being programmed");
				string text6 = "Enter Assembly Part Number for Device";
				value2 = Smart.User.InputBox(text5, text6, (string)null);
				Smart.Log.Info(TAG, "Using manual scan PCBA Assembly Part Number: " + value2);
			}
			if (value2.Trim() == string.Empty || value2 == "UNKNOWN")
			{
				if (i == pcbaFilePathNames.Count - 1)
				{
					LogResult((Result)1, "PCB Board Part Number cannot be read");
					return null;
				}
				continue;
			}
			SortedList<string, string> sortedList = null;
			foreach (SortedList<string, string> item in (IEnumerable<SortedList<string, string>>)val)
			{
				array = item["PCBAssemblyPart"].Trim().Split(new char[1] { '|' });
				bool flag2 = false;
				string[] array2 = array;
				for (int j = 0; j < array2.Length; j++)
				{
					string text7 = array2[j].Trim();
					if (text7 != string.Empty && value2.StartsWith(text7))
					{
						flag2 = true;
						text2 = item["RsdFamily"].Trim();
						text3 = item["RsdCarrier"].Trim();
						break;
					}
				}
				if (flag2 && item["RsdCarrier"].Trim() == text && item["RsdFamily"].Trim() == value)
				{
					sortedList = item;
				}
			}
			if (sortedList == null)
			{
				if (i != pcbaFilePathNames.Count - 1)
				{
					continue;
				}
				string text8 = $"PCBA information missing for this configuration-Board assembly, carrier, model: {value2} - {text} - {value}";
				Smart.Log.Debug(TAG, text8);
				if (text2 != string.Empty)
				{
					string promptText = string.Format("{0} {1}-{2}-{3}.{4}.", Smart.Locale.Xlate("The connected model is"), value2, text2, text3, string.Format("{0} {1}-{2}", Smart.Locale.Xlate("You may select a wrong model"), text, value));
					Task.Run(delegate
					{
						//IL_002a: Unknown result type (might be due to invalid IL or missing references)
						string text9 = Smart.Locale.Xlate("PCBA Model Info");
						device.Prompt.MessageBox(text9, promptText, (MessageBoxButtons)0, (MessageBoxIcon)64);
					});
					text8 = promptText;
				}
				LogResult((Result)1, "PCBA information missing for this configuration", text8);
				return null;
			}
			sortedList["boardAssembly"] = value2;
			return sortedList;
		}
		LogResult((Result)1, "Unknown error during PCBA lookup");
		return null;
	}
}
