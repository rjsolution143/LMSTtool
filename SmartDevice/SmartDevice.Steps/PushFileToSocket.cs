using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ISmart;

namespace SmartDevice.Steps;

public class PushFileToSocket : BaseStep
{
	private string mDevicePath;

	private long mDeviceFileSize;

	private IDevice mDevice;

	private IPEndPoint mRemoteServer;

	private Socket mClientSocket;

	private volatile bool mCancelReceive;

	private AutoResetEvent mAutoResetEvent = new AutoResetEvent(initialState: false);

	private ConcurrentQueue<string> mResponseList = new ConcurrentQueue<string>();

	private volatile bool mCancelResquest;

	private const int BUFFSIZE = 5120;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		mDevice = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).LocalPath;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		FileInfo fileInfo = new FileInfo(text);
		mDeviceFileSize = fileInfo.Length;
		mDevicePath = ((dynamic)base.Info.Args).DevicePath;
		int num = ((dynamic)base.Info.Args).Port;
		string key2 = $"Port{num}";
		int port = base.Cache[key2];
		try
		{
			mRemoteServer = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
			mClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			mClientSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, optionValue: true);
			mClientSocket.Connect(mRemoteServer);
			Smart.Thread.Wait(TimeSpan.FromMilliseconds(1000.0));
			Task.Factory.StartNew(ReceviceHandler);
			if (!mClientSocket.Connected)
			{
				Smart.Log.Error(TAG, "Can not connect android service");
				LogResult((Result)1, "Could not connect to android service");
				return;
			}
			using FileStream fs = fileInfo.OpenRead();
			Smart.Log.Debug(TAG, "Send head information ......");
			byte[] array = CreateHeadInformation(fileInfo, text, mDevicePath);
			mClientSocket.Send(array, 0, array.Length, SocketFlags.None);
			if (!mAutoResetEvent.WaitOne(20000))
			{
				Smart.Log.Error(TAG, "Send head information timeout");
				LogResult((Result)1, "Send head timeout");
				return;
			}
			string aPKResponse = GetAPKResponse();
			if (aPKResponse == "CanPushRom")
			{
				if (!PushRomFile(fs))
				{
					Smart.Log.Error(TAG, "Push rom failed");
					LogResult((Result)1, "Push rom failed");
					return;
				}
				Smart.Thread.Wait(TimeSpan.FromMilliseconds(2000.0));
				byte[] bytes = Encoding.UTF8.GetBytes("####");
				mClientSocket.Send(bytes, 0, bytes.Length, SocketFlags.None);
				if (!mAutoResetEvent.WaitOne(300000))
				{
					Smart.Log.Error(TAG, "Wait MD5 check response timeout");
					LogResult((Result)1, "MD5 timeout");
					return;
				}
				aPKResponse = GetAPKResponse();
				if (aPKResponse == "MD5CheckSuccess")
				{
					Smart.Log.Debug(TAG, "Rom md5 check success");
					LogPass();
				}
				else
				{
					Smart.Log.Error(TAG, $"MD% check error response {aPKResponse}");
					LogResult((Result)1, "MD5 check failed");
				}
			}
			else
			{
				Smart.Log.Error(TAG, $"Send header error response string {aPKResponse}");
				LogResult((Result)1, "Send header failed");
			}
		}
		catch (Exception ex)
		{
			Exit();
			Smart.Log.Error(TAG, $"Push Rom exception {ex.ToString()}");
			LogResult((Result)1, "Generic push rom failure");
		}
		finally
		{
			mCancelReceive = true;
			try
			{
				if (mClientSocket.Connected)
				{
					mClientSocket.Disconnect(reuseSocket: false);
					mClientSocket.Close();
				}
			}
			catch (Exception)
			{
			}
		}
	}

	private byte[] CreateHeadInformation(FileInfo file, string localRomFullName, string pushRomPath)
	{
		string s = GetMd5Hash(localRomFullName).ToLower();
		byte[] bytes = Encoding.UTF8.GetBytes("****");
		byte[] bytes2 = Encoding.UTF8.GetBytes(s);
		byte[] bytes3 = BitConverter.GetBytes(bytes2.Length);
		byte[] bytes4 = BitConverter.GetBytes(file.Length);
		byte[] bytes5 = Encoding.UTF8.GetBytes(pushRomPath);
		return Enumerable.Concat(second: BitConverter.GetBytes(bytes5.Length), first: bytes.Concat(bytes3).Concat(bytes2).Concat(bytes4)).Concat(bytes5).ToArray();
	}

	private bool PushRomFile(FileStream fs)
	{
		try
		{
			int num = -1;
			_ = mDevicePath;
			int num2 = (int)(fs.Length / 5120);
			int num3 = (int)(fs.Length - 5120 * num2);
			byte[] array = new byte[5120];
			long num4 = 0L;
			for (int i = 0; i < num2; i++)
			{
				if (mCancelResquest)
				{
					Smart.Log.Error(TAG, "Push Rom cancelled DeviceID: " + mDevice.ID);
					return false;
				}
				fs.Read(array, 0, array.Length);
				mClientSocket.Send(array, 0, array.Length, SocketFlags.None);
				num4 += array.Length;
				int num5 = (int)(num4 * 100 / mDeviceFileSize);
				if (num5 > num)
				{
					Smart.Log.Verbose(TAG, $"DeviceId: {mDevice.ID} Pushed: {num5}%");
					ProgressUpdate(num5);
					num = num5;
				}
			}
			if (num3 != 0)
			{
				if (mCancelResquest)
				{
					Smart.Log.Error(TAG, "Push Rom cancelled DeviceID: " + mDevice.ID);
					return false;
				}
				array = new byte[num3];
				fs.Read(array, 0, array.Length);
				mClientSocket.Send(array, 0, array.Length, SocketFlags.None);
				num4 += array.Length;
				int num5 = (int)(num4 * 100 / mDeviceFileSize);
				Smart.Log.Verbose(TAG, $"DeviceId: {mDevice.ID} Pushed: {num5}%");
				ProgressUpdate(num5);
				Smart.Thread.Wait(TimeSpan.FromMilliseconds(500.0));
			}
			return true;
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Push File exception:" + ex.StackTrace);
			return false;
		}
	}

	private void ReceviceHandler()
	{
		byte[] array = new byte[1024];
		int num = 0;
		while (mClientSocket.Connected && !mCancelReceive)
		{
			try
			{
				num = mClientSocket.Receive(array);
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, "Received response exception:" + ex.ToString());
				break;
			}
			if (num != 0)
			{
				string @string = Encoding.UTF8.GetString(array, 0, num);
				Smart.Log.Verbose(TAG, $"Received response {@string}");
				ResponseHandler(@string);
			}
		}
	}

	private void ResponseHandler(string responseString)
	{
		switch (responseString)
		{
		default:
			return;
		case "ReceiveHeadInformationSuccess\r\n":
			mResponseList.Enqueue("ReceiveHeadInformationSuccess");
			break;
		case "ReceiveHeadInformationFailed\r\n":
			mResponseList.Enqueue("ReceiveHeadInformationFailed");
			break;
		case "CreateDirectoryFailed\r\n":
			mResponseList.Enqueue("CreateDirectoryFailed");
			break;
		case "CanPushRom\r\n":
			mResponseList.Enqueue("CanPushRom");
			break;
		case "MD5CheckSuccess\r\n":
			mResponseList.Enqueue("MD5CheckSuccess");
			break;
		case "MD5CheckFail\r\n":
			mResponseList.Enqueue("MD5CheckFail");
			break;
		case "PushRomWithError\r\n":
			mResponseList.Enqueue("PushRomWithError");
			break;
		case "LowStorage\r\n":
			mResponseList.Enqueue("LowStorage");
			break;
		}
		mAutoResetEvent.Set();
	}

	private string GetAPKResponse()
	{
		string result = string.Empty;
		while (!mResponseList.TryDequeue(out result))
		{
		}
		return result;
	}

	private string GetMd5Hash(string pathName)
	{
		string result = "";
		FileStream fileStream = null;
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		try
		{
			fileStream = new FileStream(pathName.Replace("\"", ""), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			byte[] array = mD5CryptoServiceProvider.ComputeHash(fileStream);
			fileStream.Close();
			fileStream.Dispose();
			result = BitConverter.ToString(array).Replace("-", "");
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, pathName + " get MD5 code failed! errorMsg: " + ex.StackTrace);
		}
		return result;
	}

	private void Exit()
	{
		if (mClientSocket.Connected)
		{
			mClientSocket.Disconnect(reuseSocket: false);
		}
	}
}
