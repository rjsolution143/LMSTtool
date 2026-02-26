using System.IO;
using ISmart;

namespace SmartDevice.Steps;

public class CheckLocalFile : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Invalid comparison between Unknown and I4
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		string text = ((dynamic)base.Info.Args).LocalPath;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		if (((dynamic)base.Info.Args).ExtraPath != null)
		{
			string text2 = ((dynamic)base.Info.Args).ExtraPath;
			text += text2;
		}
		FileInfo fileInfo = new FileInfo(text);
		Result result = (Result)8;
		string description = string.Empty;
		string dynamicError = string.Empty;
		if (!fileInfo.Exists)
		{
			result = (Result)1;
			description = "File does not exist";
			dynamicError = $"File: '{text}' not found";
		}
		VerifyOnly(ref result);
		if ((int)result == 1)
		{
			LogResult(result, description, dynamicError);
		}
		else
		{
			LogResult(result);
		}
	}
}
