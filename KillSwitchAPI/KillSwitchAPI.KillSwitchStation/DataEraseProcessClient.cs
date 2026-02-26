using System;
using System.Text;
using System.Threading.Tasks;
using KillSwitchAPI.TinyTcp;

namespace KillSwitchAPI.KillSwitchStation;

public class DataEraseProcessClient
{
	private TinyTcpClient tcpClient;

	public Action<string> Logger = null;

	public bool RecodeInnerLog { get; set; } = false;


	public bool IsConnected => tcpClient.IsConnected;

	public string ErrorMessage { get; private set; }

	public DataEraseProcessClient(string ipPort)
	{
		tcpClient = new TinyTcpClient(ipPort);
		TinyTcpClient tinyTcpClient = tcpClient;
		tinyTcpClient.Logger = (Action<string>)Delegate.Combine(tinyTcpClient.Logger, new Action<string>(TcpClientLogging));
		tcpClient.DataReceived += TcpClient_DataReceived;
	}

	private void TcpClient_DataReceived(object sender, DataReceivedEventArgs e)
	{
	}

	private void TcpClientLogging(string obj)
	{
		if (RecodeInnerLog)
		{
			Logging("tinyTcpClient:" + obj);
		}
	}

	private void Logging(string obj)
	{
		Logger?.Invoke(obj);
	}

	public bool Init(string ipAddress, out string errorMsg)
	{
		errorMsg = "connection success";
		tcpClient = new TinyTcpClient(ipAddress);
		TinyTcpClient tinyTcpClient = tcpClient;
		tinyTcpClient.Logger = (Action<string>)Delegate.Combine(tinyTcpClient.Logger, new Action<string>(Logging));
		bool flag = tcpClient.Connect();
		if (!flag)
		{
			errorMsg = "connection failed,please check the log";
		}
		return flag;
	}

	public int QueryFixtureStatus(int fixtureId, out string errorMsg)
	{
		errorMsg = "Query Success";
		if (!IsConnected)
		{
			errorMsg = "Client is not connected to the server";
			return 4;
		}
		string s = tcpClient.SendAndWait($"Cmd:QueryFixtureStatus;fixtureId={fixtureId};");
		int result = AnaylizeReQueryFixtureStatus(s, out errorMsg);
		ErrorMessage = errorMsg;
		return result;
	}

	public async Task<int> QueryFixtureStatusAsync(int fixtureId)
	{
		ErrorMessage = "Query Success";
		if (!IsConnected)
		{
			ErrorMessage = "Client is not connected to the server";
			return 4;
		}
		string errorMsg;
		int re = AnaylizeReQueryFixtureStatus(await tcpClient.SendAndWaitAsync($"Cmd:QueryFixtureStatus;fixtureId={fixtureId};").ConfigureAwait(continueOnCapturedContext: false), out errorMsg);
		ErrorMessage = errorMsg;
		return re;
	}

	private int AnaylizeReQueryFixtureStatus(string s, out string errorMsg)
	{
		errorMsg = string.Empty;
		if (string.IsNullOrWhiteSpace(s))
		{
			errorMsg = "Query failed,communitcation failed";
			return 4;
		}
		if (s.Contains("ReQueryFixtureStatus"))
		{
			string[] array = s.Split(new char[1] { ';' });
			if (array.Length >= 2)
			{
				string[] array2 = array[1].Split(new char[1] { '=' });
				if (array2.Length == 2 && int.TryParse(array2[1], out var result) && result >= 0 && result <= 3)
				{
					return result;
				}
				string[] array3 = array[2].Split(new char[1] { '=' });
				if (array3.Length == 2)
				{
					errorMsg = array3[1];
				}
			}
		}
		errorMsg = "Query failed,unknown response;Fail to parse the protocol";
		return 4;
	}

	[Obsolete("use method with param extraInfo")]
	public bool WriteToFixture(int fixtureId, int value, out string errorMsg)
	{
		errorMsg = string.Empty;
		if (!IsConnected)
		{
			errorMsg = "Client is not connected to the server";
			return false;
		}
		string s = tcpClient.SendAndWait($"Cmd:WriteToFixture;fixtureId={fixtureId};value={value}");
		bool result = AnaylizeReWriteToFixture(s, out errorMsg);
		ErrorMessage = errorMsg;
		return result;
	}

	public bool WriteToFixture(int fixtureId, int value, string extraInfo, out string errorMsg)
	{
		errorMsg = string.Empty;
		if (!IsConnected)
		{
			errorMsg = "Client is not connected to the server";
			return false;
		}
		string s = tcpClient.SendAndWait($"Cmd:WriteToFixture;fixtureId={fixtureId};value={value};extraInfo={extraInfo}");
		bool result = AnaylizeReWriteToFixture(s, out errorMsg);
		ErrorMessage = errorMsg;
		return result;
	}

	[Obsolete("use method with param extraInfo")]
	public async Task<bool> WriteToFixtureAsync(int fixtureId, int value)
	{
		ErrorMessage = string.Empty;
		if (!IsConnected)
		{
			ErrorMessage = "Client is not connected to the server";
			return false;
		}
		string errorMsg;
		bool re = AnaylizeReWriteToFixture(await tcpClient.SendAndWaitAsync($"Cmd:WriteToFixture;fixtureId={fixtureId};value={value}").ConfigureAwait(continueOnCapturedContext: false), out errorMsg);
		ErrorMessage = errorMsg;
		return re;
	}

	public async Task<bool> WriteToFixture(int fixtureId, int value, string extraInfo)
	{
		ErrorMessage = string.Empty;
		if (!IsConnected)
		{
			ErrorMessage = "Client is not connected to the server";
			return false;
		}
		string errorMsg;
		bool re = AnaylizeReWriteToFixture(await tcpClient.SendAndWaitAsync($"Cmd:WriteToFixture;fixtureId={fixtureId};value={value};extraInfo={extraInfo}").ConfigureAwait(continueOnCapturedContext: false), out errorMsg);
		ErrorMessage = errorMsg;
		return re;
	}

	private bool AnaylizeReWriteToFixture(string s, out string errorMsg)
	{
		errorMsg = string.Empty;
		if (string.IsNullOrWhiteSpace(s))
		{
			errorMsg = "WriteToFixture failed,communitcation failed";
			return false;
		}
		if (s.Contains("ReWriteToFixture"))
		{
			string[] array = s.Split(new char[1] { ';' });
			if (array.Length >= 3)
			{
				string[] array2 = array[1].Split(new char[1] { '=' });
				if (array2.Length == 2 && bool.TryParse(array2[1], out var result) && result)
				{
					return true;
				}
				string[] array3 = array[2].Split(new char[1] { '=' });
				if (array3.Length == 2)
				{
					errorMsg = array3[1];
				}
			}
		}
		if (string.IsNullOrWhiteSpace(errorMsg))
		{
			errorMsg = "WriteToFixture failed,unknown response;Fail to parse the protocol";
		}
		return false;
	}

	public static string Protocol()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Protocol:");
		stringBuilder.AppendLine("1,QueryFixtureStatus");
		stringBuilder.AppendLine("   client Send::     Cmd:QueryFixtureStatus;fixtureId=1;");
		stringBuilder.AppendLine("   server Response:: ReQueryFixtureStatus;fixtureValue=1;errorMsg=xxxxx;");
		stringBuilder.AppendLine("");
		stringBuilder.AppendLine("2,WriteToFixture");
		stringBuilder.AppendLine("   client Send::     Cmd:WriteToFixture;fixtureId=1;value=1;extraInfo=354990480022031,XT2000-1,motorola/kansas gsys/kansas:15/V1VK35.22-100/03d13-66ed72:user/release-keys");
		stringBuilder.AppendLine("   server Response:: ReWriteToFixture;result=true/false;errorMsg=xxxxx;");
		return stringBuilder.ToString();
	}
}
