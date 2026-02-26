using System;

namespace SmartDevice.Cfc;

public class VerifyPKI : BaseTest
{
	private string TAG => GetType().FullName;

	public bool Execute(string pkiKeyType, Func<string, string, string> testcommand, bool production, bool FourByteType)
	{
		string empty = string.Empty;
		bool flag = true;
		try
		{
			empty = (production ? "01" : "02");
			if (string.IsNullOrEmpty(pkiKeyType))
			{
				throw new NotSupportedException("PKI key type value must be non-empty");
			}
			empty = ((!FourByteType) ? ("02" + empty + pkiKeyType + new string('0', 60)) : ("09" + empty + pkiKeyType));
			string text = testcommand("006A", empty);
			flag = text.StartsWith("00");
			Smart.Log.Debug(TAG, string.Format("Pki key type = '{0}' ,response data = '{1}', verification result = {2}", pkiKeyType, text, flag ? "PASS" : "FALL"));
		}
		catch (Exception ex)
		{
			if (ex.Message != string.Empty)
			{
				flag = flag && false;
				Smart.Log.Debug(TAG, ex.Message + "," + ex.StackTrace);
			}
		}
		return flag;
	}
}
