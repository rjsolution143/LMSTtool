using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KillSwitchAPI.TinyTcp;

namespace KillSwitchAPI.KillSwitchStation;

public class DataEraseProcessServerBase
{
	private TinyTcpServer tcpServer;

	public Action<string> Logger = null;

	protected string errorMsg = string.Empty;

	public EventHandler<ReciveWriteToFixtureEventArgs> ReciveWriteToFixtureEvent;

	public TinyTcpServer TcpServer => tcpServer;

	public bool RecodeInnerLog { get; set; } = false;


	public event EventHandler<DataReceivedEventArgs> DataReceived;

	public DataEraseProcessServerBase(string ipPort)
	{
		tcpServer = new TinyTcpServer(ipPort);
		TinyTcpServer tinyTcpServer = tcpServer;
		tinyTcpServer.Logger = (Action<string>)Delegate.Combine(tinyTcpServer.Logger, new Action<string>(TcpServerLogging));
		tcpServer.DataReceived += TcpServer_DataReceived;
	}

	private void TcpServer_DataReceived(object sender, DataReceivedEventArgs e)
	{
		DataReceivedEventArgs e2 = e;
		this.DataReceived?.Invoke(this, e2);
		byte[] bytes = Enumerable.ToArray(e2.Data);
		string @string = Encoding.UTF8.GetString(bytes);
		if (@string.StartsWith("Cmd"))
		{
			string[] array = @string.Split(new char[1] { ';' });
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (array.Length >= 2)
			{
				for (int i = 1; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[1] { '=' });
					if (array2.Length == 2)
					{
						dictionary.Add(array[i].Split(new char[1] { '=' })[0], array[i].Split(new char[1] { '=' })[1]);
					}
				}
			}
			if (@string.Contains("QueryFixtureStatus"))
			{
				if (!dictionary.TryGetValue("fixtureId", out var value))
				{
					return;
				}
				if (int.TryParse(value, out var fixtureIndex2))
				{
					Task.Run(delegate
					{
						int num = ReQueryFixtureStatus(fixtureIndex2);
						string data2 = $"ReQueryFixtureStatus;fixtureValue={num};errorMsg={errorMsg}";
						tcpServer.Send(e2.IpPort, data2);
					});
				}
			}
			else
			{
				if (!@string.Contains("WriteToFixture") || !dictionary.TryGetValue("fixtureId", out var value2) || !dictionary.TryGetValue("value", out var value3))
				{
					return;
				}
				if (int.TryParse(value2, out var fixtureIndex) && int.TryParse(value3, out var writeValue))
				{
					if (dictionary.TryGetValue("extraInfo", out var extraInfo))
					{
						ProcessBeforeResponseWriteToFixture(fixtureIndex, writeValue, extraInfo);
						ReciveWriteToFixtureEvent?.Invoke(this, new ReciveWriteToFixtureEventArgs
						{
							FixtureIndex = fixtureIndex,
							WriteValue = writeValue,
							ExtraInfo = extraInfo
						});
					}
					else
					{
						extraInfo = string.Empty;
					}
					Task.Run(delegate
					{
						bool flag = ReWriteToFixture(fixtureIndex, writeValue, extraInfo);
						string data = $"ReWriteToFixture;result={flag};errorMsg={errorMsg};";
						tcpServer.Send(e2.IpPort, data);
					});
				}
			}
		}
		else
		{
			tcpServer.Send(e2.IpPort, "No Cmd");
		}
	}

	public virtual bool ProcessBeforeResponseWriteToFixture(int fixtureIndex, int writeValue, string extraInfo = "")
	{
		return false;
	}

	public virtual bool ReWriteToFixture(int fixtureIndex, int writeValue, string extraInfo)
	{
		errorMsg = "method DataEraseProcessServerBase need to override";
		return false;
	}

	public virtual int ReQueryFixtureStatus(int fixtureIndex)
	{
		errorMsg = "method DataEraseProcessServerBase need to override";
		return 4;
	}

	private void TcpServerLogging(string obj)
	{
		if (RecodeInnerLog)
		{
			Logging("tinyTcpServer:" + obj);
		}
	}

	private void Logging(string obj)
	{
		Logger?.Invoke(obj);
	}

	public void Start()
	{
		tcpServer.Start();
	}

	public void Stop()
	{
		tcpServer.Stop();
	}
}
