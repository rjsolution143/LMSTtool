using System;
using System.Collections.Generic;
using System.Threading;
using ISmart;

namespace SmartDevice.Steps;

public abstract class CommServerStep : BaseStep
{
	protected bool ignoreErrors;

	protected static bool CitDelay;

	private string TAG => GetType().FullName;

	protected string Activity { get; set; }

	protected ICommServerClient CommServer => (ICommServerClient)base.Cache["commServer"];

	private void CheckCommServer()
	{
		if (!base.Cache.ContainsKey("commServer"))
		{
			throw new NotSupportedException("No existing CommServer connection found");
		}
	}

	public override void Setup()
	{
		base.Setup();
		StartActivity();
	}

	public override Result Audit()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		CheckCommServer();
		((object)CommServer).ToString();
		return base.Audit();
	}

	protected void StartActivity()
	{
		if (((dynamic)base.Info.Args).IgnoreErrors != null)
		{
			ignoreErrors = ((dynamic)base.Info.Args).IgnoreErrors;
		}
		if (!((((dynamic)base.Info.Args)["Activity"] != null) ? true : false))
		{
			return;
		}
		Activity = ((dynamic)base.Info.Args).Activity;
		try
		{
			CheckCommServer();
			CommServer.SendCommand("START", Activity);
		}
		catch (Exception)
		{
			if (!ignoreErrors)
			{
				throw;
			}
		}
		int num = 1;
		if (((dynamic)base.Info.Args).DelaySecAfterStartActivity != null)
		{
			num = ((dynamic)base.Info.Args).DelaySecAfterStartActivity;
		}
		if (CitDelay && num < 1)
		{
			Smart.Log.Debug(TAG, "Adding extra delay for CIT Activity Start");
			num = 1;
		}
		if (num > 0)
		{
			Thread.Sleep(num * 1000);
		}
	}

	protected void StopActivity()
	{
		if (!((((dynamic)base.Info.Args)["Activity"] != null) ? true : false))
		{
			return;
		}
		Activity = ((dynamic)base.Info.Args).Activity;
		try
		{
			CheckCommServer();
			CommServer.SendCommand("STOP", Activity);
		}
		catch (Exception)
		{
			if (!ignoreErrors)
			{
				throw;
			}
		}
	}

	public override void TearDown()
	{
		base.TearDown();
		StopActivity();
	}

	public override void Restart()
	{
		StopActivity();
		Thread.Sleep(5 * 1000);
		StartActivity();
	}

	protected SortedList<string, string> Tell(string command)
	{
		return Tell(command, string.Empty);
	}

	protected SortedList<string, string> Tell(string command, string data)
	{
		string command2 = $"TELL {Activity} {command}";
		return SendCommand(command2, data);
	}

	protected SortedList<string, string> SendCommand(string command, string data)
	{
		CheckCommServer();
		return CommServer.SendCommand(command, data);
	}

	protected override void Set(string settingType, dynamic settings)
	{
		foreach (dynamic setting in settings)
		{
			string text = DynamicKey(setting);
			string data = string.Format("{0}={1}", text, setting.Value);
			Tell(settingType, data);
		}
	}
}
