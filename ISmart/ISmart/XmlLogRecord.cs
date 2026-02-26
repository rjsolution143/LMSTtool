using System;
using System.Security;

namespace ISmart;

public struct XmlLogRecord
{
	private const string OemName = "MOTOROLA";

	public string MobileSerialNumber { get; set; }

	public string TimeStamp { get; set; }

	public string Result { get; private set; }

	public string NewFlashVersion { get; set; }

	public string NewFlexVersion { get; set; }

	public UseCase CurrentUseCase { get; set; }

	public XmlLogRecord(Result result, UseCase useCase)
	{
		this = default(XmlLogRecord);
		Result = ((result == ISmart.Result.Passed) ? "PASS" : "FAIL");
		CurrentUseCase = useCase;
	}

	public override string ToString()
	{
		return "<UNIT><ESN>" + EscapeXML(MobileSerialNumber) + "</ESN><TIMESTAMP>" + EscapeXML(TimeStamp) + "</TIMESTAMP><RESULT>" + EscapeXML(Result) + "</RESULT><SOFTWARE_NAME>" + EscapeXML(NewFlexVersion) + "</SOFTWARE_NAME><SOFTWARE_VER>" + EscapeXML(NewFlashVersion) + "</SOFTWARE_VER><OEM_NAME>" + EscapeXML("MOTOROLA") + "</OEM_NAME></UNIT>";
	}

	private static string EscapeXML(string rawtext)
	{
		try
		{
			return SecurityElement.Escape(rawtext);
		}
		catch (Exception)
		{
			return rawtext;
		}
	}
}
