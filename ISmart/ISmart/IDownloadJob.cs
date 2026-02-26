using System;
using System.Threading;

namespace ISmart;

public interface IDownloadJob
{
	string Name { get; }

	string URL { get; }

	bool AutoUnzip { get; }

	string UnzipPath { get; }

	bool Locked { get; }

	string LocalPath { get; }

	DownloadStatus Status { get; }

	DownloadStep Step { get; }

	long Size { get; }

	long Progress { get; }

	string ProgressText { get; }

	int Percent { get; }

	long BytesPerSecond { get; }

	string SpeedText { get; }

	DateTime LastActive { get; }

	int FailureCount { get; }

	int FailureDelay { get; }

	TimeSpan OfflineTime { get; }

	CancellationTokenSource Cancel { get; }

	void Process();

	void Pause();

	void UnPause();
}
