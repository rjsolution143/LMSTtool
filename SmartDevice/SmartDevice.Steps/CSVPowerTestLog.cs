using System;
using System.Collections.Generic;
using System.IO;

namespace SmartDevice.Steps;

public class CSVPowerTestLog
{
	public enum COLUMN
	{
		TestName,
		Measurement,
		LowerLimit,
		UpperLimit,
		Units,
		Status,
		Serial
	}

	public List<string[]> Records { get; private set; }

	private string TAG => GetType().FullName;

	public CSVPowerTestLog(List<string> resultCSVs)
	{
		Records = new List<string[]>();
		for (int i = 0; i < resultCSVs.Count; i++)
		{
			using StreamReader streamReader = new StreamReader(resultCSVs[i]);
			try
			{
				Smart.Log.Debug(TAG, $" Reading {resultCSVs[i]}...");
				string text;
				while ((text = streamReader.ReadLine()) != null)
				{
					Smart.Log.Verbose(TAG, text);
					string[] array = text.Split(new char[1] { ',' });
					if (array.Length >= 6 && array[5] != "Status" && array[4] != "P/F")
					{
						for (int j = 0; j < array.Length; j++)
						{
							array[j] = array[j].Trim();
						}
						Records.Add(array);
					}
				}
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, $"Exception while parsing file {resultCSVs[i]} - error {ex.Message}");
			}
		}
	}
}
