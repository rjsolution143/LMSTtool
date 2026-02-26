using System.Collections.Generic;
using System.IO;
using System.Xml;
using ISmart;

namespace SmartDevice.Steps;

public class CitDeviceBasedTestLog
{
	public enum COLUMN
	{
		TestName,
		Measurement,
		LowerLimit,
		UpperLimit,
		Units,
		Status,
		ErrorMessage
	}

	public const string PASSED = "2";

	public const string FAILED = "1";

	public const string SKIPPED = "0";

	public List<string[]> Records { get; private set; }

	public Result TestResult { get; private set; }

	private string TAG => GetType().FullName;

	public CitDeviceBasedTestLog(string testLog)
	{
		Records = new List<string[]>();
		string xml = File.ReadAllText(testLog);
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(xml);
		XmlNode xmlNode;
		string text = (((xmlNode = xmlDocument.SelectSingleNode("hwDetection/hwDetectionResult")) != null) ? xmlNode.InnerText : string.Empty);
		if (text == "2")
		{
			TestResult = (Result)8;
		}
		else if (text == "0")
		{
			TestResult = (Result)7;
		}
		else
		{
			TestResult = (Result)1;
		}
		foreach (XmlNode item in xmlDocument.SelectNodes("hwDetection/testItem"))
		{
			string[] array = new string[7]
			{
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty
			};
			array[0] = (((xmlNode = item.SelectSingleNode("itemName")) != null) ? xmlNode.InnerText : string.Empty);
			array[5] = (((xmlNode = item.SelectSingleNode("itemResult")) != null) ? xmlNode.InnerText : string.Empty);
			if (array[5] != "2" && array[5] != "0")
			{
				TestResult = (Result)1;
			}
			array[6] = (((xmlNode = item.SelectSingleNode("errorMessage")) != null) ? xmlNode.InnerText : string.Empty);
			Records.Add(array);
		}
	}
}
