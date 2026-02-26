using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class GPPDCheck : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Invalid comparison between Unknown and I4
		//IL_075c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Invalid comparison between Unknown and I4
		//IL_0890: Unknown result type (might be due to invalid IL or missing references)
		//IL_086d: Unknown result type (might be due to invalid IL or missing references)
		string empty = string.Empty;
		string empty2 = string.Empty;
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		Result result = (Result)8;
		string description = string.Empty;
		string dynamicError = string.Empty;
		if (((dynamic)base.Info.Args).InputName != null && ((dynamic)base.Info.Args).InputName != string.Empty)
		{
			empty = ((dynamic)base.Info.Args).InputName.ToString();
			empty2 = base.Cache[empty];
		}
		else
		{
			empty = "DeviceSerialNumber";
			empty2 = val.SerialNumber;
		}
		bool flag = default(bool);
		string value = Smart.Rsd.GetValue("phoneModel", base.Recipe.Info.UseCase, val, ref flag, false);
		string logInfoValue = val.GetLogInfoValue("pcb-part-no");
		string[] array = val.ModelId.Split(new char[1] { '|' });
		string text = "UNKNOWN";
		if (array.Length > 1)
		{
			text = array[1];
		}
		Smart.Log.Info(TAG, $"Search GPPD ID info in PCBA file for {empty} on {value} {text}");
		List<string> pcbaFilePathNames = Smart.Rsd.GetPcbaFilePathNames();
		int num = 0;
		while (num < pcbaFilePathNames.Count)
		{
			string text2 = Smart.Rsd.ReadPcbaFileContent(pcbaFilePathNames[num]);
			ICsvFile val2 = Smart.NewCsvFile();
			val2.Load(text2, ',');
			string text3 = base.Cache[empty + "GppdId"];
			string text4 = base.Cache[empty + "Customer"];
			string text5 = "##IGNORE##";
			string text6 = text5;
			if ((int)Smart.Convert.ToSerialNumberType(empty2) == 2)
			{
				text6 = base.Cache[empty + "Protocol"];
			}
			flag = false;
			foreach (SortedList<string, string> item in (IEnumerable<SortedList<string, string>>)val2)
			{
				string text7 = item["RsdFamily"];
				string text8 = item["RsdCarrier"];
				if (text7.ToLowerInvariant() != value.ToLowerInvariant() || text8.ToLowerInvariant() != text.ToLowerInvariant())
				{
					continue;
				}
				string value2 = GetValue(item["Gppd"], val);
				string value3 = GetValue(item["UpdCustomer"], val);
				if (!(value2.ToLowerInvariant() != text3.ToLowerInvariant()) && !(value3.ToLowerInvariant() != text4.ToLowerInvariant()))
				{
					string text9 = item["Protocol"];
					if (!(text6 != text5) || !(text9.ToLowerInvariant() != text6.ToLowerInvariant()))
					{
						flag = true;
						Smart.Log.Info(TAG, $"GPPD ID info mached for {text3} {text4} {text6} {empty} on {value} {text}");
						break;
					}
				}
			}
			if (!flag && (int)base.Recipe.Info.UseCase == 158)
			{
				Smart.Log.Info(TAG, "Not found matched upd_customer+gppd+rsd carrier+rsd model in PCBA file, fall back to use carrier+model+pcb part no to check whether hw is compatible for NewSN use case");
				foreach (SortedList<string, string> item2 in (IEnumerable<SortedList<string, string>>)val2)
				{
					string text10 = item2["RsdFamily"];
					string text11 = item2["RsdCarrier"];
					if (!(text10.ToLowerInvariant() != value.ToLowerInvariant()) && !(text11.ToLowerInvariant() != text.ToLowerInvariant()) && !(item2["PCBAssemblyPart"].ToLowerInvariant() != logInfoValue.ToLowerInvariant()))
					{
						flag = true;
						Smart.Log.Info(TAG, $"GPPD ID info mached for {logInfoValue} on {value} {text}");
						break;
					}
				}
			}
			if (!flag)
			{
				if (num != pcbaFilePathNames.Count - 1)
				{
					num++;
					continue;
				}
				Smart.Log.Error(TAG, $"Could not find matching info for {text3}, {text4} and {text6}");
				result = (Result)1;
				description = "PCBA entry missing for this configuration";
				dynamicError = $"GPPD ID, Customer, Protocol values not found <{text3}+{text4}+{text6}>";
			}
			VerifyOnly(ref result);
			LogResult(result, description, dynamicError);
			break;
		}
	}

	private string GetValue(string value, IDevice device)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		string text = value;
		if ((int)device.Type == 2)
		{
			string[] array = text.Split(new char[1] { ',' });
			if (array.Length > 1)
			{
				text = array[1];
			}
		}
		return text;
	}
}
