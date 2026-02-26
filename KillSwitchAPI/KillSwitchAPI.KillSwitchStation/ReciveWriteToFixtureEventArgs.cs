using System;

namespace KillSwitchAPI.KillSwitchStation;

public class ReciveWriteToFixtureEventArgs : EventArgs
{
	public int FixtureIndex { get; set; }

	public int WriteValue { get; set; }

	public string ExtraInfo { get; set; } = string.Empty;

}
