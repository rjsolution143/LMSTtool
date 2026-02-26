using System;
using System.Collections.Generic;

namespace ISmart;

public interface IDownloadEngine : IDisposable
{
	List<IDownloadJob> Jobs { get; }

	int CurrentDownloads { get; }

	int CurrentJobs { get; }

	long BytesPerSecond { get; }

	long CompletedBytes { get; }

	long RemainingBytes { get; }

	long TotalBytes { get; }

	int Percent { get; }

	string ProgressText { get; }

	string SpeedText { get; }

	TimeSpan TimeRemaining { get; }

	string TimeRemainingText { get; }

	TimeSpan OfflineTime { get; }

	long FreeSpace { get; }

	string FreeSpaceText { get; }

	List<Action<string>> Callbacks { get; }

	int MaxDownloads { get; set; }

	int MaxJobs { get; set; }

	int MaxStreams { get; set; }

	long MaxBytesPerSecond { get; set; }

	DateTime ActiveStart { get; set; }

	DateTime ActiveEnd { get; set; }

	DateTime DelayUntil { get; set; }

	bool ActiveNow { get; }

	void DownloadFile(string name, string remoteUrl, string localPath, bool unzip, bool createFolder);

	DownloadResponse DownloadFiles(IDownloadRequest request);

	DownloadResponse Status();

	DownloadResponse Status(string URL);

	void LoadOptions(dynamic options);

	dynamic SaveOptions();

	void LoadJobs(dynamic jobData);

	dynamic SaveJobs();

	void ReloadCredentials();

	void Start();

	void Pause();

	IDownloadJob Prioritize(string URL);

	void Clear();

	string Print();

	void Move(List<IDownloadJob> jobs, int newIndex);
}
