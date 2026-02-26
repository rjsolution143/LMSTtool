using System;

namespace ISmart;

public struct RefurbInfo
{
	public string ToolVersion { get; }

	public DateTime StartTime { get; }

	public TimeSpan Duration { get; }

	public bool PassStatus { get; }

	public UseCase UseCase { get; }

	public string ModelName { get; }

	public string FlashSize { get; }

	public string Fingerprint { get; }

	public string AndroidVersion { get; }

	public string TrackID { get; }

	public string Imei { get; }

	public string Carrier { get; }

	public RefurbInfo(string toolVersion, DateTime startTime, TimeSpan duration, bool passStatus, UseCase useCase, string modelName, string flashSize, string fingerprint, string androidVersion, string trackId, string imei, string carrier)
	{
		ToolVersion = toolVersion;
		StartTime = startTime;
		Duration = duration;
		PassStatus = passStatus;
		UseCase = useCase;
		ModelName = modelName;
		FlashSize = flashSize;
		Fingerprint = fingerprint;
		AndroidVersion = androidVersion;
		TrackID = trackId;
		Imei = imei;
		Carrier = carrier;
	}
}
