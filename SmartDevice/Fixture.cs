using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using ISmart;
using KillSwitchAPI.KillSwitchStation;

namespace SmartDevice;

public class Fixture : IDisposable
{
	protected string extraInfo = string.Empty;

	private int RetryTime = 1;

	private const int MaxRetryTimes = 3;

	private const int TimeoutSecToWaitForFastbootPort = 20;

	private UseCase[] DetectUseCases;

	private UseCase[] ReadUseCases;

	protected FakeFixtureWindow fake_client;

	protected DataEraseProcessClient client;

	private string TAG => GetType().FullName;

	public int Index { get; private set; }

	public FixtureStatus Status { get; private set; } = FixtureStatus.Unknown;


	public FixtureResult Result { get; private set; }

	public FixtureStep CurrentStep { get; private set; }

	public IDevice Device { get; private set; }

	public bool Processing { get; private set; }

	public bool Closed { get; private set; }

	public Fixture(int index, FakeFixtureWindow client)
	{
		UseCase[] array = new UseCase[3];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		DetectUseCases = (UseCase[])(object)array;
		UseCase[] array2 = new UseCase[4];
		RuntimeHelpers.InitializeArray(array2, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		ReadUseCases = (UseCase[])(object)array2;
		base._002Ector();
		Index = index;
		fake_client = client;
	}

	public Fixture(int index, DataEraseProcessClient client)
	{
		UseCase[] array = new UseCase[3];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		DetectUseCases = (UseCase[])(object)array;
		UseCase[] array2 = new UseCase[4];
		RuntimeHelpers.InitializeArray(array2, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		ReadUseCases = (UseCase[])(object)array2;
		base._002Ector();
		Index = index;
		this.client = client;
	}

	public void ProcessThread()
	{
		string name = MethodBase.GetCurrentMethod().Name;
		try
		{
			Processing = true;
			bool flag = false;
			Smart.Log.Debug(name, $"Fixture {Index} process thread started");
			DateTime now = DateTime.Now;
			while (true)
			{
				if (CurrentStep == FixtureStep.Disabled)
				{
					Smart.Log.Error(name, $"Fixture {Index} is disabled");
					return;
				}
				if (CurrentStep == FixtureStep.Scan && !flag && DateTime.Now.Subtract(now).TotalSeconds > 20.0)
				{
					Smart.Log.Error(name, $"Fixture {Index} timed out waiting for device");
					return;
				}
				if (CurrentStep == FixtureStep.Detect || CurrentStep == FixtureStep.Run || CurrentStep == FixtureStep.Report)
				{
					if (!flag)
					{
						Smart.Log.Debug(name, $"Fixture {Index} processing started");
					}
					flag = true;
				}
				if (flag && CurrentStep == FixtureStep.Scan)
				{
					break;
				}
				Process();
			}
			Smart.Log.Debug(name, $"Fixture {Index} finished processing thread");
		}
		finally
		{
			if (Device != null)
			{
				Device.Automated = false;
				Smart.Log.Debug(TAG, $"Device {Device.ID} ({Device.Unique}) unset Automated at end of process");
			}
			Smart.Log.Debug(name, $"Fixture {Index} process thread complete");
			Processing = false;
		}
	}

	public void Process()
	{
		switch (CurrentStep)
		{
		case FixtureStep.Disabled:
			Disabled();
			break;
		case FixtureStep.Scan:
			Scan();
			break;
		case FixtureStep.Detect:
			Detect();
			break;
		case FixtureStep.Run:
			Run();
			break;
		case FixtureStep.Report:
			Report();
			break;
		default:
			throw new NotSupportedException("Step not supported: " + CurrentStep);
		}
	}

	private void Disabled()
	{
	}

	private void Scan()
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Invalid comparison between Unknown and I4
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Invalid comparison between Unknown and I4
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Invalid comparison between Unknown and I4
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		string name = MethodBase.GetCurrentMethod().Name;
		if (Device != null)
		{
			Device.Automated = false;
			Smart.Log.Debug(TAG, $"Device {Device.ID} ({Device.Unique}) unset Automated at start of scan");
			Device = null;
		}
		Result = FixtureResult.Testing;
		extraInfo = string.Empty;
		if (Status == FixtureStatus.Ready)
		{
			Smart.Log.Debug(name, $"Fixture {Index} ready to detect");
			CurrentStep = FixtureStep.Detect;
			SendCommand(FixtureCommand.ReportUnderDetection);
			UsbPortStatus val = Smart.UsbPorts.PortStatus[Index];
			if ((int)val == 1 || (int)val == 3 || (int)val == 2)
			{
				Smart.Log.Debug(name, $"Fixture {Index} port status is {val}");
			}
			else
			{
				Smart.Log.Warning(name, $"WARNING: Fixture {Index} port status is {val}");
			}
		}
		else
		{
			Smart.Log.Debug(name, $"Fixture {Index} waiting for fixture to be ready");
			Smart.Thread.Wait(TimeSpan.FromSeconds(2.0));
		}
	}

	private void Detect()
	{
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Invalid comparison between Unknown and I4
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Invalid comparison between Unknown and I4
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Invalid comparison between Unknown and I4
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Invalid comparison between Unknown and I4
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		string name = MethodBase.GetCurrentMethod().Name;
		if (Device != null)
		{
			Device.Automated = false;
			Smart.Log.Debug(TAG, $"Device {Device.ID} ({Device.Unique}) unset Automated at start of detect");
			Device = null;
		}
		if (Status != FixtureStatus.Ready && Status != FixtureStatus.Testing)
		{
			Smart.Log.Debug(name, $"Fixture {Index} is not ready to detect: {Status}");
			CurrentStep = FixtureStep.Scan;
			return;
		}
		bool flag = false;
		try
		{
			Smart.Log.Debug(name, $"Fixture {Index} starting detection process, try {RetryTime}");
			SendCommand(FixtureCommand.PressKey);
			if (Smart.UsbPorts.WaitForPort(Index, TimeSpan.FromSeconds(20.0)))
			{
				Smart.Log.Debug(name, $"Fixture {Index} found device on port");
				SendCommand(FixtureCommand.ReleaseKey);
				flag = true;
				Smart.Thread.Wait(TimeSpan.FromSeconds(3.0));
				Smart.Log.Debug(name, $"Fixture {Index} waiting for full device detection");
				DateTime now = DateTime.Now;
				while (DateTime.Now.Subtract(now).TotalSeconds < 30.0)
				{
					bool flag2 = false;
					foreach (IDevice value in Smart.DeviceManager.Devices.Values)
					{
						if (value.PortIndex == Index)
						{
							Device = value;
							Smart.Log.Debug(name, $"Fixture {Index} found Device: {value.ID}");
							break;
						}
						if (value.PortIndex == 0)
						{
							flag2 = true;
						}
					}
					if (Device != null)
					{
						break;
					}
					if (DateTime.Now.Subtract(now).TotalSeconds > 5.0 && flag2 && (int)Smart.UsbPorts.PortStatus[Index] == 2)
					{
						UsbPortStatus val = Smart.UsbPorts.PortQuery(Index);
						Smart.Log.Error(name, $"Port {Index} status is {((object)(UsbPortStatus)(ref val)).ToString()}");
					}
					Smart.Thread.Wait(TimeSpan.FromSeconds(1.0));
				}
			}
			else
			{
				string text = $"Not found port on Fixture {Index} after {20} sec";
				Smart.Log.Error(name, text);
			}
		}
		finally
		{
			Smart.Log.Debug(name, $"Fixture {Index} ending detection process");
			if (!flag)
			{
				SendCommand(FixtureCommand.ReleaseKey);
				flag = true;
			}
		}
		if (Device != null)
		{
			Smart.Log.Debug(name, $"Fixture {Index} ready to run");
			CurrentStep = FixtureStep.Run;
			Device.Automated = true;
			Smart.Log.Debug(TAG, $"Device {Device.ID} ({Device.Unique}) set Automated after detection");
			SendCommand(FixtureCommand.ReportUnderTesting);
			return;
		}
		Smart.Log.Error(name, $"Fixture {Index} timed out waiting for Device detection");
		UsbPortStatus val2 = Smart.UsbPorts.PortStatus[Index];
		Smart.Log.Debug(name, $"Fixture {Index} port status is {val2}");
		if ((int)val2 == 0)
		{
			Smart.Log.Error(name, $"Fixture {Index} needs port assignment!");
		}
		else if ((int)val2 == 2)
		{
			Smart.Log.Error(name, $"Fixture {Index} has connected device but no fastboot connection");
		}
		else if ((int)val2 == -1)
		{
			Smart.Log.Error(name, $"Fixture {Index} has device assigned to multiple ports");
		}
		else if ((int)val2 == 1)
		{
			Smart.Log.Error(name, $"Fixture {Index} has no USB connection established");
		}
		if (RetryTime == 3)
		{
			Result = FixtureResult.Error;
			CurrentStep = FixtureStep.Report;
			Smart.Log.Error(name, $"Detect port on Fixture {Index} try {RetryTime} still fail, report error");
		}
		else
		{
			RetryTime++;
			Smart.Thread.Wait(TimeSpan.FromSeconds(5.0));
			CurrentStep = FixtureStep.Detect;
		}
	}

	private void Run()
	{
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Invalid comparison between Unknown and I4
		//IL_0667: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f7: Invalid comparison between Unknown and I4
		//IL_067b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		string name = MethodBase.GetCurrentMethod().Name;
		if (Device == null)
		{
			Smart.Log.Debug(name, $"Fixture {Index} is not ready to run, no Device");
			CurrentStep = FixtureStep.Scan;
			SendCommand(FixtureCommand.ReportError);
			return;
		}
		Result = FixtureResult.Testing;
		Smart.Log.Debug(name, $"Fixture {Index} waiting for Device {Device.ID} to finish detection process");
		if (Device.Log == null)
		{
			Smart.Log.Debug(name, "Waiting for Device.Log to be created");
			while (Device.Log == null)
			{
				Smart.Thread.Wait(TimeSpan.FromMilliseconds(50.0));
			}
		}
		bool flag = false;
		bool flag2 = false;
		DateTime value = DateTime.Now.Subtract(TimeSpan.FromDays(900.0));
		while (true)
		{
			bool flag3 = DetectUseCases.Contains(Device.Log.UseCase);
			bool num = ReadUseCases.Contains(Device.Log.UseCase);
			bool locked = Device.Locked;
			double progress = Device.Log.Progress;
			TimeSpan timeSpan = DateTime.Now.Subtract(value);
			if (num)
			{
				Smart.Log.Debug(name, $"Fixture {Index} Device {Device.ID} Read has been started");
				flag = false;
				flag2 = false;
				break;
			}
			if (!flag && !flag3)
			{
				flag = true;
				Smart.Log.Debug(name, $"Fixture {Index} Device {Device.ID} Waiting for Detect to start");
				continue;
			}
			if (flag)
			{
				if (!flag3)
				{
					Smart.Thread.Wait(TimeSpan.FromSeconds(2.0));
					continue;
				}
				Smart.Log.Debug(name, $"Fixture {Index} Device {Device.ID} Detect has started");
				flag = false;
			}
			if (!flag2 && !locked)
			{
				if (timeSpan.TotalDays > 500.0)
				{
					Smart.Log.Debug(name, $"Fixture {Index} Device {Device.ID} Waiting for Detect to Lock device");
					value = DateTime.Now;
					continue;
				}
				if (timeSpan.TotalSeconds > 10.0)
				{
					flag = false;
					flag2 = false;
					Smart.Log.Debug(name, $"Fixture {Index} Device {Device.ID} Detect never became Locked");
					break;
				}
				Smart.Thread.Wait(TimeSpan.FromSeconds(2.0));
			}
			else if (!flag2)
			{
				flag2 = true;
				Smart.Log.Debug(name, $"Fixture {Index} Device {Device.ID} Waiting for Detect to finish");
			}
			else if (flag2 && !locked)
			{
				Smart.Log.Debug(TAG, $"Fixture {Index} Device {Device.ID} Detect has completed, progress reached {progress}%");
				flag = false;
				flag2 = false;
				break;
			}
		}
		bool flag4 = false;
		flag2 = false;
		DateTime now = DateTime.Now;
		while (true)
		{
			bool flag5 = ReadUseCases.Contains(Device.Log.UseCase);
			bool locked2 = Device.Locked;
			double progress2 = Device.Log.Progress;
			TimeSpan timeSpan2 = DateTime.Now.Subtract(now);
			if (!flag4 && !flag5)
			{
				flag4 = true;
				Smart.Log.Debug(name, $"Fixture {Index} Device {Device.ID} Waiting for Read to start");
				continue;
			}
			if (flag4)
			{
				if (timeSpan2.TotalSeconds > 30.0)
				{
					Smart.Log.Debug(name, $"Fixture {Index} Device {Device.ID} Read never started (maybe Detect failed?)");
					flag4 = false;
					break;
				}
				if (!flag5)
				{
					Smart.Thread.Wait(TimeSpan.FromSeconds(2.0));
					continue;
				}
				Smart.Log.Debug(name, $"Fixture {Index} Device {Device.ID} Read has started");
				if (!locked2)
				{
					Smart.Log.Debug(name, $"Fixture {Index} Device {Device.ID} Waiting for Detect to Lock device");
				}
				flag4 = false;
			}
			if (!flag2 && !locked2)
			{
				if (timeSpan2.TotalSeconds > 30.0)
				{
					flag4 = false;
					flag2 = false;
					Smart.Log.Debug(name, $"Fixture {Index} Device {Device.ID} Read never became Locked");
					break;
				}
				Smart.Thread.Wait(TimeSpan.FromSeconds(2.0));
			}
			else if (!flag2)
			{
				flag2 = true;
				Smart.Log.Debug(name, $"Fixture {Index} Device {Device.ID} Waiting for Read to finish");
			}
			else if (flag2 && !locked2)
			{
				Smart.Log.Debug(TAG, $"Fixture {Index} Device {Device.ID} Read has completed, progress reached {progress2}%");
				flag = false;
				flag2 = false;
				break;
			}
		}
		Smart.Log.Debug(name, $"Fixture {Index} Device {Device.ID} has finished detection process");
		while (Device.Locked)
		{
			Smart.Log.Warning(name, $"Fixture {Index} Device {Device.ID} is still locked, waiting...");
			Smart.Thread.Wait(TimeSpan.FromSeconds(2.0));
		}
		try
		{
			UseCase val = (UseCase)211;
			Smart.Log.Debug(name, $"Fixture {Index} Device {Device.ID} running use case {val}");
			Smart.UseCaseRunner.Run(val, Device, false, false);
			Result val2 = (Result)4;
			if (Device != null && Device.Log.UseCase == val)
			{
				val2 = Device.Log.OverallResult;
				Smart.Log.Error(name, $"Fixture {Index} Device {Device.ID} use case OverallResult: {val2}");
			}
			Smart.Log.Error(name, $"Fixture {Index} Device {Device.ID} use case result: {val2}");
			if ((int)val2 == 8)
			{
				Result = FixtureResult.Pass;
			}
			else if ((int)val2 == 1)
			{
				Result = FixtureResult.Fail;
			}
			else
			{
				Result = FixtureResult.Error;
			}
			Smart.Log.Info(name, $"Fixture {Index} Device {Device.ID} finished with result: {Result}");
		}
		catch (Exception ex)
		{
			Result = FixtureResult.Error;
			Smart.Log.Error(name, $"Fixture {Index} Device {Device.ID} error while running: {ex.Message}");
			Smart.Log.Verbose(name, ex.ToString());
		}
		CurrentStep = FixtureStep.Report;
		Smart.Log.Info(name, string.Format("Report Fixture {0} Device {1} finished with result: {2}", Index, (Device == null) ? "UNKNOWN_DEVICE" : Device.ID, Result));
		string serialNumber = Device.SerialNumber;
		string text = string.Empty;
		if (Device.Log.Info.ContainsKey("SKU"))
		{
			text = Device.Log.Info["SKU"];
		}
		string text2 = string.Empty;
		if (Device.Log.Info.ContainsKey("FirstFailedStepName"))
		{
			text2 = Device.Log.Info["FirstFailedStepName"];
		}
		extraInfo = serialNumber + "," + text + "," + text2;
	}

	private void Report()
	{
		string name = MethodBase.GetCurrentMethod().Name;
		FixtureCommand fixtureCommand = FixtureCommand.ReportError;
		switch (Result)
		{
		case FixtureResult.Error:
			fixtureCommand = FixtureCommand.ReportError;
			break;
		case FixtureResult.Testing:
			fixtureCommand = FixtureCommand.ReportError;
			break;
		case FixtureResult.Pass:
			fixtureCommand = FixtureCommand.ReportPass;
			break;
		case FixtureResult.Fail:
			fixtureCommand = FixtureCommand.ReportFail;
			break;
		}
		Smart.Log.Info(name, string.Format("Fixture {0} SN {1} reporting final status: {2}", Index, (Device == null) ? "UNKNOWN_DEVICE" : Device.ID, fixtureCommand));
		SendCommand(fixtureCommand, extraInfo);
		RetryTime = 1;
		CurrentStep = FixtureStep.Scan;
	}

	public void Refresh()
	{
		string name = MethodBase.GetCurrentMethod().Name;
		if (Closed)
		{
			Status = FixtureStatus.Unknown;
			return;
		}
		lock (Automation.ClientLock)
		{
			if (!client.IsConnected)
			{
				Smart.Log.Error(name, "Automation client is not Connected");
				Status = FixtureStatus.Unknown;
				return;
			}
			string empty = string.Empty;
			int num = client.QueryFixtureStatus(Index, ref empty);
			string text = $"Fixture {Index} received status response: {num} - '{empty}'";
			if (num < 0 || num > 3)
			{
				Smart.Log.Error(name, text);
				Smart.Log.Error(name, $"Fixture {Index} could not get status: {empty}");
				Status = FixtureStatus.Error;
				return;
			}
			FixtureStatus fixtureStatus = (FixtureStatus)num;
			if (Status != fixtureStatus)
			{
				Smart.Log.Debug(name, text);
				Smart.Log.Debug(name, $"Fixture {Index} status changed from {Status} to {fixtureStatus}");
				Status = fixtureStatus;
			}
		}
	}

	public void SendCommand(FixtureCommand command, string extraData = "")
	{
		if (Closed)
		{
			Status = FixtureStatus.Unknown;
			return;
		}
		lock (Automation.ClientLock)
		{
			if (!client.IsConnected)
			{
				Smart.Log.Error(TAG, "Automation client is not Connected");
				Status = FixtureStatus.Unknown;
				throw new SocketException();
			}
			Smart.Log.Verbose(TAG, $"Fixture {Index} sending command {command} with extra data {extraData}");
			string empty = string.Empty;
			bool flag = false;
			for (int i = 0; i < 3; i++)
			{
				flag = client.WriteToFixture(Index, (int)command, extraData, ref empty);
				Smart.Log.Verbose(TAG, $"Fixture {Index} received write response: {flag} - '{empty}'");
				if (flag)
				{
					break;
				}
				Smart.Thread.Wait(TimeSpan.FromSeconds(2.0));
			}
			if (!flag)
			{
				Smart.Log.Error(TAG, $"Fixture {Index} could not send command {command}: {empty}");
				Status = FixtureStatus.Unknown;
				throw new WebException(empty);
			}
		}
		Refresh();
	}

	public void Dispose()
	{
		client = null;
		Closed = true;
	}
}
