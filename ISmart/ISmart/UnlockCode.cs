namespace ISmart;

public struct UnlockCode
{
	public enum CodeType
	{
		UnknownCode,
		ApcCode,
		CdmaAKey1,
		CdmaAKey2,
		CdmaEvdo,
		CdmaMasterLockCode,
		CdmaOneTimeLockCode,
		CdmaServicePasscode,
		Cit,
		GsmPrimaryLockCode,
		GsmSecondaryLockCode,
		GsmServicePasscode,
		IccId,
		IntelControlKey,
		Lock4,
		Lock5,
		MacAddress,
		WiMaxAddress,
		WlanAddress,
		WlanAddress2,
		WlanAddress3,
		WlanAddress4,
		SerialNumber
	}

	private string TAG => GetType().FullName;

	public CodeType Type { get; private set; }

	public string Code { get; private set; }

	public static UnlockCode BlankCode => new UnlockCode(CodeType.UnknownCode, "UNKNOWN");

	public UnlockCode(CodeType type, string code)
	{
		this = default(UnlockCode);
		Type = type;
		Code = code;
	}

	public override string ToString()
	{
		return $"{Type}: {Code}";
	}

	public override bool Equals(object obj)
	{
		return ToString().Equals(obj.ToString());
	}

	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}
}
