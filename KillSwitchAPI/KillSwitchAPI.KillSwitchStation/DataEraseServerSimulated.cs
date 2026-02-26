using System;

namespace KillSwitchAPI.KillSwitchStation;

public class DataEraseServerSimulated : DataEraseProcessServerBase
{
	public DataEraseServerSimulated(string ipPort)
		: base(ipPort)
	{
	}

	public override int ReQueryFixtureStatus(int fixtureIndex)
	{
		int num = new Random().Next(0, 5);
		if (num == 4)
		{
			errorMsg = "Simulated Other Case,Random 4;";
		}
		return num;
	}

	public override bool ReWriteToFixture(int fixtureIndex, int writeValue, string extraInfo)
	{
		return true;
	}
}
