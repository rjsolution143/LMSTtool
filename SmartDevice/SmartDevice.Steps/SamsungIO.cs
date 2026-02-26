using System;
using System.Runtime.InteropServices;

namespace SmartDevice.Steps;

public class SamsungIO
{
	public delegate void GetResponseCB(IntPtr pCmd, int CmdLen, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1024)] byte[] pResp, int length);

	public delegate void GetLogCB(int LogType, int portNum, int length, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1024)] byte[] plogInfo);

	public const string SPCBA_DLL = "SamsungIOAdapt.dll";

	[DllImport("SamsungIOAdapt.dll")]
	public static extern void CreateSamsungIO(int index);

	[DllImport("SamsungIOAdapt.dll")]
	public static extern void CreateSamsungIOSimulator(int index);

	[DllImport("SamsungIOAdapt.dll")]
	public static extern void DestroySamsungIO(int index);

	[DllImport("SamsungIOAdapt.dll")]
	public static extern int OpenCOM(int index);

	[DllImport("SamsungIOAdapt.dll")]
	public static extern int CloseCOM(int index);

	[DllImport("SamsungIOAdapt.dll")]
	public static extern int SetExtInfo(int index, int CommandTimeOut, string serialNumber);

	[DllImport("SamsungIOAdapt.dll")]
	public static extern int SetCOM(int index, string comport, int baudrate, byte bytesize, int fparity, byte parity, byte stopbits);

	[DllImport("SamsungIOAdapt.dll")]
	public static extern int SendCommand(int index, string WriteBuf, int length, int timeout, int retryCnt, byte sync);

	[DllImport("SamsungIOAdapt.dll")]
	public static extern int RecvResponse(int index, ref byte readBuf, ref int readLen);

	[DllImport("SamsungIOAdapt.dll")]
	public static extern int SendCmdUntilResp(int index, string writeBuf, int writeLen, ref byte readBuf, ref int readLen, int timeout, int retryCnt);

	[DllImport("SamsungIOAdapt.dll")]
	public static extern int GetSimLockInfo(int index, string path, string data, int dataLen, ref byte pOutCmd, ref int outCmdLen);

	[DllImport("SamsungIOAdapt.dll")]
	public static extern int SetCallBack(int index, GetResponseCB responseCB, GetLogCB logCB);

	public void createSamsungIO(int index, IONotification notify)
	{
		CreateSamsungIO(index);
	}

	public void createSamsungIOSimulator(int index, IONotification notify)
	{
		CreateSamsungIOSimulator(index);
	}

	public void destroySamsungIO(int index)
	{
		DestroySamsungIO(index);
	}

	public int openCOM(int index)
	{
		return OpenCOM(index);
	}

	public int closeCOM(int index)
	{
		return CloseCOM(index);
	}

	public int setExtInfo(int index, int CommandTimeOut, string serialNumber)
	{
		return SetExtInfo(index, CommandTimeOut, serialNumber);
	}

	public int setCallBack(int index, GetResponseCB responseCB, GetLogCB logCB)
	{
		return SetCallBack(index, responseCB, logCB);
	}

	public int setCOM(int index, string comport, int baudrate, byte bytesize, int fparity, byte parity, byte stopbits)
	{
		return SetCOM(index, comport, baudrate, bytesize, fparity, parity, stopbits);
	}

	public int sendCommand(int index, string WriteBuf, int length, int timeout, int retryCnt, byte sync)
	{
		return SendCommand(index, WriteBuf, length, timeout, retryCnt, sync);
	}

	public int sendCmdUntilResp(int index, string writeBuf, int writeLen, ref byte readBuf, ref int readLen, int timeout, int retryCnt)
	{
		return SendCmdUntilResp(index, writeBuf, writeLen, ref readBuf, ref readLen, timeout, retryCnt);
	}

	public int recvResponse(int index, ref byte readBuf, ref int readLen)
	{
		return RecvResponse(index, ref readBuf, ref readLen);
	}

	public int getSimLockInfo(int index, string path, string data, int dataLen, ref byte pOutCmd, ref int outCmdLen)
	{
		return GetSimLockInfo(index, path, data, dataLen, ref pOutCmd, ref outCmdLen);
	}
}
