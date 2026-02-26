using System;
using System.Globalization;
using System.IO;
using System.Text;
using ISmart;

namespace SmartDevice.Steps;

public abstract class TestCommandStep : BaseStep
{
	private TimeSpan existingTimeout = TimeSpan.MinValue;

	private string TAG => GetType().FullName;

	public ITestCommandClient tcmd => (ITestCommandClient)base.Cache["tcmd"];

	public override void Setup()
	{
		base.Setup();
		string value = SystemTimeHex();
		base.Cache["CurrentTime"] = value;
		if (!base.Cache.ContainsKey("tcmd"))
		{
			throw new IOException("Test Command Interface Not Found");
		}
		if (((dynamic)base.Info.Args)["CommandTimeout"] != null)
		{
			existingTimeout = tcmd.Timeout;
			TimeSpan timeout = TimeSpan.FromSeconds((int)((dynamic)base.Info.Args).CommandTimeout);
			tcmd.Timeout = timeout;
		}
	}

	public override void TearDown()
	{
		base.TearDown();
		if (existingTimeout > TimeSpan.MinValue)
		{
			tcmd.Timeout = existingTimeout;
		}
	}

	protected string byteToHexStr(byte[] bytes)
	{
		string text = "";
		if (bytes != null)
		{
			for (int i = 0; i < bytes.Length; i++)
			{
				text += bytes[i].ToString("X2");
			}
		}
		return text;
	}

	protected string StrToHex(string input)
	{
		StringBuilder stringBuilder = new StringBuilder();
		char[] array = input.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			int num = Convert.ToInt32(array[i]);
			stringBuilder.Append($"{num:X}");
		}
		return stringBuilder.ToString();
	}

	private string SystemTimeHex()
	{
		DateTime value = new DateTime(2000, 1, 1, 0, 0, 0);
		TimeSpan timeSpan = DateTime.Now.ToUniversalTime().Subtract(value);
		long num = (long)timeSpan.TotalSeconds;
		long num2 = (long)timeSpan.TotalDays;
		long num3 = num - num2 * 86400;
		return "02" + num2.ToString("X4", CultureInfo.InvariantCulture) + num3.ToString("X8", CultureInfo.InvariantCulture);
	}
}
