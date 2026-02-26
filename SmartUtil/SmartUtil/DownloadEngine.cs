using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ISmart;

namespace SmartUtil;

public class DownloadEngine : IDownloadEngine, IDisposable
{
	private TimeSpan TICK_TIME = TimeSpan.FromMilliseconds(200.0);

	private TimeSpan WAIT_TIME = TimeSpan.FromSeconds(10.0);

	private TimeSpan PAUSE_TIME = TimeSpan.FromSeconds(60.0);

	private TimeSpan PURGE_TIME = TimeSpan.FromHours(24.0);

	private object jobLock = new object();

	private SortedList<DateTime, DownloadJob> Completed = new SortedList<DateTime, DownloadJob>();

	private string downloadDrive = "C";

	private bool started;

	private bool cancelled;

	private bool paused;

	private bool manualPause;

	private CancellationTokenSource canceller = new CancellationTokenSource();

	private bool disposedValue;

	private string TAG => GetType().FullName;

	public List<IDownloadJob> Jobs { get; set; }

	public int CurrentDownloads { get; set; }

	public int CurrentJobs { get; set; }

	public int MaxDownloads { get; set; }

	public int MaxJobs { get; set; }

	public int MaxStreams { get; set; }

	public long MaxBytesPerSecond { get; set; }

	public DateTime ActiveStart { get; set; }

	public DateTime ActiveEnd { get; set; }

	public DateTime DelayUntil { get; set; }

	private int JobCount
	{
		get
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Invalid comparison between Unknown and I4
			lock (jobLock)
			{
				int num = 0;
				foreach (DownloadJob job in Jobs)
				{
					if ((int)job.Status != 6)
					{
						num++;
					}
				}
				return num;
			}
		}
	}

	private string JobsList
	{
		get
		{
			lock (jobLock)
			{
				string text = string.Empty;
				foreach (DownloadJob job in Jobs)
				{
					text = text + job.URL + "|";
				}
				text.TrimEnd(new char[1] { '|' });
				return text;
			}
		}
	}

	public bool ActiveNow
	{
		get
		{
			if (manualPause || !started)
			{
				return false;
			}
			if (!DelayUntil.Equals(DateTime.MinValue) && DateTime.Now < DelayUntil)
			{
				return false;
			}
			if (!ActiveStart.Equals(DateTime.MinValue) && !ActiveEnd.Equals(DateTime.MinValue))
			{
				TimeSpan timeOfDay = ActiveStart.TimeOfDay;
				TimeSpan timeOfDay2 = ActiveEnd.TimeOfDay;
				if (!(timeOfDay2 < timeOfDay))
				{
					if (DateTime.Now.TimeOfDay > timeOfDay)
					{
						return DateTime.Now.TimeOfDay < timeOfDay2;
					}
					return false;
				}
				if (!(DateTime.Now.TimeOfDay > timeOfDay))
				{
					return DateTime.Now.TimeOfDay < timeOfDay2;
				}
				return true;
			}
			return true;
		}
	}

	public long BytesPerSecond
	{
		get
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Invalid comparison between Unknown and I4
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Invalid comparison between Unknown and I4
			if (Jobs.Count < 1)
			{
				return 0L;
			}
			double num = 0.0;
			int num2 = 0;
			int num3 = 0;
			lock (jobLock)
			{
				foreach (DownloadJob job in Jobs)
				{
					if ((int)job.Status == 1 && (int)job.Step == 3)
					{
						num3++;
						if (job.BytesPerSecond >= 1)
						{
							num2++;
							num += (double)job.BytesPerSecond;
						}
					}
				}
			}
			if (num2 < 1)
			{
				return 0L;
			}
			double num4 = num / (double)num2;
			if (Completed.Count < 1)
			{
				return (long)(num4 * (double)num3);
			}
			double num5 = num4 * 0.7;
			double num6 = num4 * 1.5;
			int num7 = Completed.Count - 5;
			if (num7 < 0)
			{
				num7 = 0;
			}
			double num8 = 0.0;
			int num9 = 0;
			for (int i = num7; i < Completed.Count; i++)
			{
				long bytesPerSecond = Completed.Values[i].BytesPerSecond;
				if (!((double)bytesPerSecond < num5) && !((double)bytesPerSecond > num6))
				{
					num8 += (double)bytesPerSecond;
					num9++;
				}
			}
			if (num9 < 1)
			{
				return (long)(num4 * (double)num3);
			}
			num8 += num;
			num9 += num2;
			return (long)(num8 / (double)num9 * (double)num3);
		}
	}

	public string SpeedText
	{
		get
		{
			long bytesPerSecond = BytesPerSecond;
			if (bytesPerSecond < 1)
			{
				return string.Empty;
			}
			return Smart.Convert.ByteSizeToDisplay(bytesPerSecond, true) + "/s";
		}
	}

	public long CompletedBytes
	{
		get
		{
			long num = 0L;
			lock (jobLock)
			{
				foreach (DownloadJob value in Completed.Values)
				{
					num += value.Size;
				}
				return num;
			}
		}
	}

	public long RemainingBytes
	{
		get
		{
			long num = 0L;
			lock (jobLock)
			{
				foreach (DownloadJob job in Jobs)
				{
					num += job.Size - job.Progress;
				}
				return num;
			}
		}
	}

	public long TotalBytes
	{
		get
		{
			long num = 0L;
			lock (jobLock)
			{
				foreach (DownloadJob job in Jobs)
				{
					num += job.Size;
				}
				return num;
			}
		}
	}

	public int Percent
	{
		get
		{
			long completedBytes = CompletedBytes;
			long totalBytes = TotalBytes;
			if (completedBytes < 1)
			{
				return 0;
			}
			if (completedBytes == totalBytes)
			{
				return 100;
			}
			return (int)Math.Floor((double)completedBytes / (double)totalBytes * 100.0);
		}
	}

	public string ProgressText
	{
		get
		{
			long completedBytes = CompletedBytes;
			long totalBytes = TotalBytes;
			if (completedBytes < 1 || totalBytes < 1)
			{
				return string.Empty;
			}
			return $"{Smart.Convert.ByteSizeToDisplay(completedBytes, true)} of {Smart.Convert.ByteSizeToDisplay(totalBytes, true)}";
		}
	}

	public TimeSpan TimeRemaining
	{
		get
		{
			long remainingBytes = RemainingBytes;
			long bytesPerSecond = BytesPerSecond;
			if (remainingBytes < 1 || bytesPerSecond < 1)
			{
				return TimeSpan.MinValue;
			}
			return TimeSpan.FromSeconds((double)remainingBytes / (double)bytesPerSecond);
		}
	}

	public string TimeRemainingText
	{
		get
		{
			TimeSpan timeRemaining = TimeRemaining;
			if (timeRemaining.Equals(TimeSpan.MinValue))
			{
				return "unknown";
			}
			double totalSeconds = timeRemaining.TotalSeconds;
			string text = "s's'";
			if (totalSeconds > 86400.0)
			{
				text = "d'd 'h'h'";
			}
			if (totalSeconds > 3600.0)
			{
				text = "h'h 'm'm'";
			}
			else if (totalSeconds > 60.0)
			{
				text = "m'm 's's'";
			}
			return timeRemaining.ToString(text);
		}
	}

	public long FreeSpace => new DriveInfo(downloadDrive).AvailableFreeSpace;

	public string FreeSpaceText => Smart.Convert.ByteSizeToDisplay(FreeSpace, true);

	public TimeSpan OfflineTime
	{
		get
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Invalid comparison between Unknown and I4
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Invalid comparison between Unknown and I4
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Invalid comparison between Unknown and I4
			TimeSpan result = TimeSpan.FromSeconds(0.0);
			lock (jobLock)
			{
				TimeSpan timeSpan = TimeSpan.MaxValue;
				foreach (DownloadJob job in Jobs)
				{
					if ((int)job.Step == 3 && ((int)job.Status == 1 || (int)job.Status == 2))
					{
						TimeSpan offlineTime = job.OfflineTime;
						if (offlineTime < timeSpan)
						{
							timeSpan = offlineTime;
						}
					}
				}
				if (timeSpan < TimeSpan.MaxValue)
				{
					return timeSpan;
				}
				return result;
			}
		}
	}

	public List<Action<string>> Callbacks { get; private set; }

	public DownloadEngine()
	{
		Jobs = new List<IDownloadJob>();
		CurrentDownloads = 0;
		CurrentJobs = 0;
		MaxDownloads = 2;
		MaxJobs = 4;
		MaxStreams = 2;
		MaxBytesPerSecond = 0L;
		ActiveStart = DateTime.MinValue;
		ActiveEnd = DateTime.MinValue;
		DelayUntil = DateTime.MinValue;
		downloadDrive = Path.GetPathRoot(Assembly.GetEntryAssembly().Location).Substring(0, 1);
		Callbacks = new List<Action<string>>();
	}

	public void LoadOptions(dynamic options)
	{
		Smart.Log.Debug(TAG, "Loading options");
		MaxDownloads = options["MaxDownloads"] ?? ((object)MaxDownloads);
		MaxJobs = options["MaxJobs"] ?? ((object)MaxJobs);
		MaxStreams = options["MaxStreams"] ?? ((object)MaxStreams);
		MaxBytesPerSecond = options["MaxBytesPerSecond"] ?? ((object)MaxBytesPerSecond);
		string text = options["ActiveStart"];
		if (text != null)
		{
			ActiveStart = DateTime.Parse(text);
		}
		string text2 = options["ActiveEnd"];
		if (text2 != null)
		{
			ActiveEnd = DateTime.Parse(text2);
		}
		string text3 = options["DelayUntil"];
		if (text3 != null)
		{
			DelayUntil = DateTime.Parse(text3);
		}
	}

	public dynamic SaveOptions()
	{
		Smart.Log.Debug(TAG, "Saving options");
		return new SortedList<string, object>
		{
			["MaxDownloads"] = MaxDownloads,
			["MaxJobs"] = MaxJobs,
			["MaxStreams"] = MaxStreams,
			["MaxBytesPerSecond"] = MaxBytesPerSecond,
			["ActiveStart"] = ActiveStart.ToString("o"),
			["ActiveEnd"] = ActiveEnd.ToString("o"),
			["DelayUntil"] = DelayUntil.ToString("o")
		};
	}

	public void LoadJobs(dynamic jobs)
	{
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		Smart.Log.Debug(TAG, "Loading saved jobs");
		List<string> list = new List<string>();
		string text = jobs["UserName"];
		Smart.Rsd.Login = new Login(text, string.Empty);
		foreach (dynamic item in jobs["Jobs"])
		{
			string text2 = item["Name"];
			string remoteUrl = item["URL"];
			string text3 = item["LocalPath"];
			string directoryName = Path.GetDirectoryName(text3);
			if (!list.Contains(directoryName))
			{
				list.Add(directoryName);
			}
			bool unzip = item["Unzip"];
			string unzipPath = item["UnzipPath"];
			Smart.Log.Debug(TAG, "Loaded saved job: " + text2);
			DownloadFile(text2, remoteUrl, text3, unzip, unzipPath);
		}
		ClearFiles(list);
	}

	private void ClearFiles(List<string> locations)
	{
		Smart.Log.Debug(TAG, "Clearing temporary files");
		foreach (string location in locations)
		{
			foreach (string item in Smart.File.FindFiles("*.dl", location, false))
			{
				System.IO.File.Delete(item);
			}
		}
		string tempPath = Path.GetTempPath();
		List<string> list = Smart.File.FindFiles("*.dl-temp", tempPath, true);
		List<string> list2 = new List<string>();
		foreach (string item2 in list)
		{
			string directoryName = Path.GetDirectoryName(item2);
			if (!list2.Contains(directoryName))
			{
				list2.Add(directoryName);
			}
		}
		foreach (string item3 in list2)
		{
			if (Directory.Exists(item3))
			{
				Directory.Delete(item3, recursive: true);
			}
		}
	}

	public void Clear()
	{
		PauseJobs();
		lock (jobLock)
		{
			Jobs.Clear();
		}
		if (Callbacks.Count <= 0)
		{
			return;
		}
		foreach (Action<string> callback in Callbacks)
		{
			callback("Cleared jobs");
		}
	}

	public dynamic SaveJobs()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Invalid comparison between Unknown and I4
		Smart.Log.Debug(TAG, "Saving jobs");
		SortedList<string, object> sortedList = new SortedList<string, object>();
		Login login = Smart.Rsd.Login;
		sortedList["UserName"] = ((Login)(ref login)).UserName;
		List<object> list = new List<object>();
		lock (jobLock)
		{
			foreach (IDownloadJob job in Jobs)
			{
				if ((int)job.Status != 6)
				{
					SortedList<string, object> sortedList2 = new SortedList<string, object>();
					sortedList2["Name"] = job.Name;
					sortedList2["URL"] = job.URL;
					sortedList2["LocalPath"] = job.LocalPath;
					sortedList2["Unzip"] = job.AutoUnzip;
					sortedList2["UnzipPath"] = job.UnzipPath;
					list.Add(sortedList2);
				}
			}
		}
		sortedList["Jobs"] = list;
		return sortedList;
	}

	private DownloadResponse Status(int added, IDownloadJob job)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Invalid comparison between Unknown and I4
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		bool flag = ActiveNow && JobCount > 0;
		int percent = Percent;
		string text = string.Empty;
		int num = JobCount;
		string text2 = JobsList;
		string message = "Reported current download status";
		long num2 = -1L;
		if (job != null)
		{
			flag = (int)job.Status == 1;
			DownloadStep step = job.Step;
			text = ((object)(DownloadStep)(ref step)).ToString();
			percent = job.Percent;
			num = 1;
			text2 = job.URL;
			num2 = job.Size;
			message = "Found status for URL: " + job.URL;
		}
		DownloadResponse result = default(DownloadResponse);
		((DownloadResponse)(ref result))._002Ector(flag, text, percent, added, num, text2, num2);
		((DownloadResponse)(ref result)).Message = message;
		return result;
	}

	public DownloadResponse Status()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return Status(0, null);
	}

	public DownloadResponse Status(string URL)
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		IDownloadJob val = null;
		lock (jobLock)
		{
			foreach (IDownloadJob job in Jobs)
			{
				if (job.URL.ToLowerInvariant() == URL.ToLowerInvariant())
				{
					val = job;
					break;
				}
			}
		}
		if (val == null)
		{
			DownloadResponse blankStatus = DownloadResponse.BlankStatus;
			((DownloadResponse)(ref blankStatus)).Message = "Download Job not found: " + URL;
			return blankStatus;
		}
		return Status(0, val);
	}

	public IDownloadJob Prioritize(string URL)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Invalid comparison between Unknown and I4
		IDownloadJob val = null;
		lock (jobLock)
		{
			foreach (IDownloadJob job in Jobs)
			{
				if (job.URL.ToLowerInvariant() == URL.ToLowerInvariant())
				{
					val = job;
					break;
				}
			}
		}
		if (val == null)
		{
			throw new FileNotFoundException("Could not find an existing job with URL: " + URL);
		}
		if ((int)val.Status == 6 && !Smart.File.Exists(val.UnzipPath))
		{
			lock (jobLock)
			{
				Jobs.Remove(val);
			}
			val = (IDownloadJob)(object)DownloadFile(val.Name, val.URL, val.LocalPath, val.AutoUnzip, val.UnzipPath);
			if (val == null)
			{
				throw new FileNotFoundException("Could not create new job with URL: " + URL);
			}
		}
		List<IDownloadJob> list = new List<IDownloadJob>();
		list.Add(val);
		Move(list, 0);
		return val;
	}

	public DownloadResponse DownloadFiles(IDownloadRequest request)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		Smart.Log.Debug(TAG, $"Received requestion for {request.Downloads.Count} files from {request.AppName} {request.AppVersion}");
		int jobCount = JobCount;
		Smart.Rsd.Login = request.Credentials;
		foreach (SortedList<string, string> download in request.Downloads)
		{
			string name = download["name"];
			string remoteUrl = download["remoteUrl"];
			string localPath = download["localFile"];
			bool unzip = bool.Parse(download["unzip"]);
			bool createFolder = bool.Parse(download["createFolder"]);
			DownloadFile(name, remoteUrl, localPath, unzip, createFolder);
		}
		int added = JobCount - jobCount;
		DownloadResponse result = Status(added, null);
		((DownloadResponse)(ref result)).Message = "Processed download request";
		return result;
	}

	public void DownloadFile(string name, string remoteUrl, string localPath, bool unzip, bool createFolder)
	{
		string unzipPath = new FileInfo(localPath).Directory.FullName;
		if (createFolder)
		{
			int length = localPath.Length - ".zip".Length;
			if (localPath.ToLowerInvariant().EndsWith(".tar.gz"))
			{
				length = localPath.Length - ".tar.gz".Length;
			}
			unzipPath = localPath.Substring(0, length);
		}
		DownloadFile(name, remoteUrl, localPath, unzip, unzipPath);
	}

	private DownloadJob DownloadFile(string name, string remoteUrl, string localPath, bool unzip, string unzipPath)
	{
		Smart.Log.Debug(TAG, $"Downloading file {name} ({remoteUrl})");
		downloadDrive = Path.GetPathRoot(localPath).Substring(0, 1);
		DownloadJob downloadJob = new DownloadJob(name, remoteUrl, localPath, unzip, unzipPath, this);
		lock (jobLock)
		{
			bool flag = false;
			foreach (DownloadJob job in Jobs)
			{
				if (job.LocalPath == downloadJob.LocalPath)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				Smart.Log.Debug(TAG, "Ignoring duplicated file: " + localPath);
				return null;
			}
			Jobs.Add((IDownloadJob)(object)downloadJob);
		}
		if (Callbacks.Count > 0)
		{
			foreach (Action<string> callback in Callbacks)
			{
				callback($"Added job {downloadJob.URL}");
			}
		}
		return downloadJob;
	}

	public void Start()
	{
		Smart.Log.Debug(TAG, "Starting jobs");
		started = true;
		UnPauseJobs();
		if (!manualPause)
		{
			Smart.Thread.Run((ThreadStart)DownloadLooper);
			return;
		}
		manualPause = false;
		canceller.Cancel();
		canceller = new CancellationTokenSource();
	}

	public void Pause()
	{
		Smart.Log.Debug(TAG, "Pausing jobs");
		manualPause = true;
		PauseJobs();
	}

	private void DownloadLooper()
	{
		while (!cancelled)
		{
			TimeSpan timeSpan = DownloadLoop();
			if (timeSpan.TotalSeconds > 1.0)
			{
				Smart.Log.Debug(TAG, $"Waiting for {timeSpan.TotalSeconds} seconds");
				_ = canceller.Token;
				try
				{
					Task.Delay(timeSpan, canceller.Token).Wait();
				}
				catch (TaskCanceledException)
				{
					Smart.Log.Debug(TAG, "Wait cancelled");
				}
				catch (AggregateException ex2)
				{
					if (ex2.InnerException.GetType() == typeof(TaskCanceledException))
					{
						Smart.Log.Debug(TAG, "Wait cancelled");
						continue;
					}
					throw;
				}
			}
			else
			{
				System.Threading.Thread.Sleep(timeSpan);
			}
		}
	}

	private TimeSpan DownloadLoop()
	{
		PurgeJobs();
		if (!ActiveNow)
		{
			if (!paused)
			{
				PauseJobs();
				paused = true;
			}
			return PAUSE_TIME;
		}
		if (paused)
		{
			UnPauseJobs();
			paused = false;
		}
		DownloadJob newJob = FindJob();
		if (newJob == null)
		{
			return WAIT_TIME;
		}
		Smart.Thread.Run((ThreadStart)delegate
		{
			ProcessJob(newJob);
		});
		return TICK_TIME;
	}

	private void PurgeJobs()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Invalid comparison between Unknown and I4
		lock (jobLock)
		{
			List<DownloadJob> list = new List<DownloadJob>();
			foreach (DownloadJob job in Jobs)
			{
				if (((int)job.Status == 6 || (int)job.Status == 4) && DateTime.Now.Subtract(job.LastActive).TotalMilliseconds > PURGE_TIME.TotalMilliseconds)
				{
					list.Add(job);
				}
			}
			foreach (DownloadJob item in list)
			{
				Jobs.Remove((IDownloadJob)(object)item);
			}
		}
	}

	private void PauseJobs()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Invalid comparison between Unknown and I4
		lock (jobLock)
		{
			foreach (DownloadJob job in Jobs)
			{
				if (job.Locked && (int)job.Step == 3 && (int)job.Status == 1)
				{
					job.Pause();
				}
			}
		}
	}

	private void UnPauseJobs()
	{
		lock (jobLock)
		{
			foreach (DownloadJob job in Jobs)
			{
				job.UnPause();
			}
		}
	}

	private DownloadJob FindJob()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Invalid comparison between Unknown and I4
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Invalid comparison between Unknown and I4
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Invalid comparison between Unknown and I4
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Invalid comparison between Unknown and I4
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Invalid comparison between Unknown and I4
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Invalid comparison between Unknown and I4
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Invalid comparison between Unknown and I4
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Invalid comparison between Unknown and I4
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Invalid comparison between Unknown and I4
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Invalid comparison between Unknown and I4
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Invalid comparison between Unknown and I4
		lock (jobLock)
		{
			if (CurrentJobs >= MaxJobs)
			{
				return null;
			}
			int num = 0;
			int num2 = 0;
			foreach (DownloadJob job in Jobs)
			{
				bool flag = (int)job.Step == 2 || (int)job.Step == 3;
				if (flag && (int)job.Status != 5 && (int)job.Status != 4)
				{
					num++;
				}
				if ((int)job.Step == 3 && (int)job.Status != 5 && (int)job.Status != 4)
				{
					num2++;
				}
				if (!job.Locked && (int)job.Step != 6 && (int)job.Status != 5 && (int)job.Status != 4 && (int)job.Status != 3 && job.FailureDelay <= 0)
				{
					if (CurrentDownloads < MaxDownloads && flag && num <= MaxDownloads * 2 && num2 <= MaxDownloads)
					{
						CurrentJobs++;
						CurrentDownloads++;
						job.Locked = true;
						return job;
					}
					if (!flag)
					{
						CurrentJobs++;
						job.Locked = true;
						return job;
					}
				}
			}
			return null;
		}
	}

	private void ProcessJob(DownloadJob job)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Invalid comparison between Unknown and I4
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Invalid comparison between Unknown and I4
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Invalid comparison between Unknown and I4
		Smart.Log.Assert(TAG, job.Locked, "Job being processed should already be locked");
		bool flag = (int)job.Step == 2 || (int)job.Step == 3;
		job.Status = (DownloadStatus)1;
		string text = string.Empty;
		try
		{
			job.Process();
		}
		finally
		{
			lock (jobLock)
			{
				if ((int)job.Status == 1)
				{
					job.Status = (DownloadStatus)2;
				}
				if ((int)job.Step == 6)
				{
					job.Status = (DownloadStatus)6;
					Completed.Add(job.LastActive, job);
					text = $"Finished job {job.URL}";
				}
				job.Locked = false;
				CurrentJobs--;
				if (flag)
				{
					CurrentDownloads--;
				}
			}
			if (text != string.Empty && Callbacks.Count > 0)
			{
				foreach (Action<string> callback in Callbacks)
				{
					callback(text);
				}
			}
		}
	}

	public void Move(List<IDownloadJob> jobs, int newIndex)
	{
		lock (jobLock)
		{
			List<int> list = new List<int>();
			foreach (IDownloadJob job in jobs)
			{
				int num = Jobs.IndexOf(job);
				if (num >= 0)
				{
					list.Add(num);
					if (num < newIndex)
					{
						newIndex--;
					}
				}
			}
			list.Sort();
			list.Reverse();
			List<IDownloadJob> list2 = new List<IDownloadJob>();
			foreach (int item in list)
			{
				list2.Insert(0, Jobs[item]);
				Jobs.RemoveAt(item);
			}
			foreach (IDownloadJob item2 in list2)
			{
				Jobs.Insert(newIndex, item2);
				newIndex++;
			}
		}
	}

	public void ReloadCredentials()
	{
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			string text = Smart.File.PathJoin(folderPath, "Motorola");
			string text2 = Smart.File.PathJoin(text, "SmartTool");
			string text3 = Smart.File.PathJoin(text, "MotoService");
			string text4 = "credentials.sv";
			string text5 = Smart.File.PathJoin(text2, text4);
			string text6 = Smart.File.PathJoin(text3, text4);
			SortedList<string, string> sortedList = new SortedList<string, string>();
			string text7 = string.Empty;
			string text8 = string.Empty;
			string text9 = string.Empty;
			string text10 = string.Empty;
			if (Smart.File.Exists(text5))
			{
				sortedList = Smart.Security.LoadLogin(text5);
				if (sortedList.ContainsKey("user"))
				{
					text7 = sortedList["user"];
				}
				if (sortedList.ContainsKey("pass"))
				{
					text8 = sortedList["pass"];
				}
				if (sortedList.ContainsKey("rsduniqueid"))
				{
					text9 = sortedList["rsduniqueid"];
				}
				if (sortedList.ContainsKey("stationid"))
				{
					text10 = sortedList["stationid"];
				}
				Smart.Rsd.Login = new Login(text7, text8);
				Smart.Log.Debug(TAG, $"Loaded saved LMST credentials for {text7}");
			}
			if (text9 == string.Empty && Smart.File.Exists(text6))
			{
				sortedList = Smart.Security.LoadLogin(text6);
				if (sortedList.ContainsKey("user") && text7 == string.Empty)
				{
					text7 = sortedList["user"];
				}
				if (sortedList.ContainsKey("pass"))
				{
					text8 = sortedList["pass"];
				}
				if (sortedList.ContainsKey("rsduniqueid"))
				{
					text9 = sortedList["rsduniqueid"];
				}
				if (sortedList.ContainsKey("stationid"))
				{
					text10 = sortedList["stationid"];
				}
				Smart.Rsd.Login = new Login(text7, text8);
				Smart.Log.Debug(TAG, $"Loaded saved MotoService credentials for {text7}");
			}
			if (text9 != string.Empty && text10 != string.Empty)
			{
				Smart.Security.RSDStationID = text10;
				Smart.Security.RSDUniqueID = text9;
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Could not load user credentials");
			Smart.Log.Verbose(TAG, ex.ToString());
		}
	}

	public string Print()
	{
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		string text = "";
		string arg = "Stopped";
		if (manualPause)
		{
			arg = "Paused";
		}
		else if (ActiveNow && JobCount > 0)
		{
			arg = "Running";
		}
		int jobCount = JobCount;
		text += $"{arg} with {jobCount} incomplete jobs out of {Jobs.Count} total";
		text = text + Environment.NewLine + $"{CurrentDownloads} download slots used, {CurrentJobs} total slots used";
		text = text + Environment.NewLine + $"Last Internet activity {OfflineTime} seconds ago";
		lock (jobLock)
		{
			foreach (DownloadJob job in Jobs)
			{
				text = text + Environment.NewLine + $"{job.Name} is {job.Status} in {job.Step}";
				text += $", {Smart.Convert.ByteSizeToDisplay(job.Progress, true)} of {Smart.Convert.ByteSizeToDisplay(job.Size, true)} complete";
				if (job.FailureDelay > 0)
				{
					text += $" - Failed {job.FailureCount} times, waiting {job.FailureDelay} seconds to try again";
				}
			}
			return text;
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}
}
