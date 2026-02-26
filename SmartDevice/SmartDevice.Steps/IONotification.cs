using System;

namespace SmartDevice.Steps;

public interface IONotification
{
	void ResponseCB(IntPtr pCmd, int CmdLen, IntPtr pResp, int length);

	void LogCB(int LogType, int portNum, int length, IntPtr plogInfo);
}
