using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ISmart;

namespace SmartUtil;

public class DownloadJob : IDownloadJob
{
	private static object zipLock = new object();

	private DownloadEngine engine;

	private List<Tuple<long, long>> chunks = new List<Tuple<long, long>>();

	private List<DateTime> chunkTimes = new List<DateTime>();

	private string tempDir = "NO_TEMP_DIR_WAS_CREATED";

	private object downloadLocker = new object();

	private object safeLocker = new object();

	private DateTime downloadStart = DateTime.MinValue;

	private DateTime lastFailure = DateTime.Now;

	private DateTime lastPause = DateTime.Now;

	private DateTime lastStart = DateTime.Now;

	private long CHUNK_SIZE = 20971520L;

	private string TEMP_EXT = ".dl";

	private int[] failureSeconds = new int[6] { 0, 5, 15, 30, 150, 300 };

	private string TAG => GetType().FullName;

	public string Name { get; private set; }

	public string URL { get; private set; }

	public bool AutoUnzip { get; private set; }

	public string UnzipPath { get; private set; }

	public bool Locked { get; set; }

	public string LocalPath { get; private set; }

	public DownloadStatus Status { get; set; }

	public DownloadStep Step { get; private set; }

	public long Size { get; private set; }

	public CancellationTokenSource Cancel { get; private set; }

	public long Progress
	{
		get
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Invalid comparison between Unknown and I4
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Invalid comparison between Unknown and I4
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Invalid comparison between Unknown and I4
			lock (downloadLocker)
			{
				if ((int)Step == 0 || (int)Step == 1 || (int)Step == 2 || (int)Status == 4)
				{
					return 0L;
				}
				long num = 0L;
				foreach (Tuple<long, long> chunk in chunks)
				{
					num += chunk.Item2 - chunk.Item1;
				}
				return Size - num;
			}
		}
	}

	public string ProgressText
	{
		get
		{
			long progress = Progress;
			if (progress < 1)
			{
				return string.Empty;
			}
			return Smart.Convert.ByteSizeToDisplay(progress, true);
		}
	}

	public int Percent
	{
		get
		{
			long progress = Progress;
			long size = Size;
			if (progress < 1)
			{
				return 0;
			}
			if (progress == size)
			{
				return 100;
			}
			return (int)Math.Floor((double)progress / (double)size * 100.0);
		}
	}

	public long BytesPerSecond
	{
		get
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Invalid comparison between Unknown and I4
			if (downloadStart.Equals(DateTime.MinValue) || chunkTimes.Count < 1 || (int)Status != 1)
			{
				return 0L;
			}
			DateTime dateTime = lastStart;
			if (downloadStart > lastStart)
			{
				dateTime = downloadStart;
			}
			int num = 0;
			foreach (DateTime chunkTime in chunkTimes)
			{
				if (chunkTime < lastPause || chunkTime < dateTime)
				{
					num++;
					continue;
				}
				break;
			}
			DateTime dateTime2 = chunkTimes.Last();
			if (num >= chunkTimes.Count || dateTime2 < dateTime)
			{
				return 0L;
			}
			double totalSeconds = dateTime2.Subtract(dateTime).TotalSeconds;
			long num2 = Progress;
			if (num > 0)
			{
				num2 -= num * CHUNK_SIZE;
				if (num2 < 0)
				{
					num2 = 0L;
				}
			}
			return (long)((double)num2 / totalSeconds);
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

	public DateTime LastActive
	{
		get
		{
			if (chunkTimes.Count < 1)
			{
				return DateTime.MinValue;
			}
			return chunkTimes.Last();
		}
	}

	public int FailureDelay
	{
		get
		{
			int num = FailureCount;
			if (num >= failureSeconds.Length)
			{
				num = failureSeconds.Length - 1;
			}
			int num2 = failureSeconds[num];
			if (num2 < 1)
			{
				return 0;
			}
			TimeSpan timeSpan = DateTime.Now.Subtract(lastFailure);
			if (timeSpan.TotalSeconds > (double)num2)
			{
				return 0;
			}
			return num2 - (int)timeSpan.TotalSeconds;
		}
	}

	public int FailureCount { get; private set; }

	private DateTime stepStartTime { get; set; }

	public TimeSpan OfflineTime
	{
		get
		{
			DateTime value = stepStartTime;
			if (chunkTimes.Count > 0)
			{
				value = chunkTimes.Last();
			}
			return DateTime.Now.Subtract(value);
		}
	}

	public DownloadJob(string name, string url, string localPath, bool unzip, string unzipPath, DownloadEngine engine)
	{
		Name = name;
		URL = url;
		AutoUnzip = unzip;
		UnzipPath = unzipPath;
		Locked = false;
		LocalPath = localPath;
		Status = (DownloadStatus)0;
		Step = (DownloadStep)0;
		Size = CHUNK_SIZE;
		Cancel = new CancellationTokenSource();
		chunks.Add(new Tuple<long, long>(0L, Size));
		FailureCount = 0;
		this.engine = engine;
		stepStartTime = DateTime.Now;
	}

	public void Pause()
	{
		Status = (DownloadStatus)3;
		lastPause = DateTime.Now;
		Cancel.Cancel();
	}

	public void UnPause()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)Status == 3)
		{
			Status = (DownloadStatus)1;
			lastStart = DateTime.Now;
		}
	}

	public void Process()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected I4, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Invalid comparison between Unknown and I4
		try
		{
			DownloadStep step = Step;
			switch ((int)step)
			{
			case 0:
				Initialize();
				Step = (DownloadStep)1;
				break;
			case 1:
				GetSize();
				Step = (DownloadStep)2;
				break;
			case 2:
				Allocate();
				Step = (DownloadStep)3;
				break;
			case 3:
				Download();
				if (AutoUnzip)
				{
					Step = (DownloadStep)4;
				}
				else
				{
					Step = (DownloadStep)5;
				}
				break;
			case 4:
				Unzip();
				Step = (DownloadStep)5;
				break;
			case 5:
				Cleanup();
				Step = (DownloadStep)6;
				break;
			case 6:
				throw new NotSupportedException("File is already fully downloaded");
			default:
				stepStartTime = DateTime.Now;
				break;
			}
		}
		catch (OperationCanceledException)
		{
			Smart.Log.Error(TAG, $"Operation cancelled while processing job {Name}");
			throw;
		}
		catch (Exception ex2)
		{
			Smart.Log.Error(TAG, $"Error while processing job {Name}: {ex2.ToString()}");
			FailureCount++;
			lastFailure = DateTime.Now;
			if ((int)Step == 4 && FailureCount > 2)
			{
				Status = (DownloadStatus)4;
			}
			throw;
		}
	}

	private void Initialize()
	{
		Smart.Log.Info(TAG, $"Initializing {URL}");
	}

	private void GetSize()
	{
		Smart.Log.Info(TAG, $"Getting size of file {URL}");
		string text = URL;
		if (text.Contains("rsdsecure") || text.Contains("rsddownload"))
		{
			text = Smart.Rsd.GetSecureUrl(text, "HEAD");
		}
		WebRequest webRequest = WebRequest.Create(text);
		webRequest.Method = "HEAD";
		long size;
		using (WebResponse webResponse = webRequest.GetResponse())
		{
			size = long.Parse(webResponse.Headers.Get("Content-Length"));
		}
		Size = size;
	}

	private void Allocate()
	{
		Smart.Log.Info(TAG, $"Allocating {Smart.Convert.ByteSizeToDisplay(Size, true)} for file {URL}");
		using (FileStream fileStream = new FileStream(LocalPath + TEMP_EXT, FileMode.Create, FileAccess.Write, FileShare.None))
		{
			fileStream.SetLength(Size);
		}
		long num = 0L;
		long cHUNK_SIZE = CHUNK_SIZE;
		chunks.Clear();
		while (num < Size)
		{
			long num2 = num + cHUNK_SIZE;
			if (num2 > Size)
			{
				num2 = Size;
			}
			Tuple<long, long> item = new Tuple<long, long>(num, num2);
			chunks.Add(item);
			num = num2;
		}
		tempDir = Smart.File.TempFolder();
		Directory.CreateDirectory(tempDir);
	}

	private void Download()
	{
		Smart.Log.Info(TAG, $"Downloading {URL}");
		List<Tuple<long, long>> source = new List<Tuple<long, long>>(chunks);
		if (downloadStart.Equals(DateTime.MinValue))
		{
			downloadStart = DateTime.Now;
		}
		lastStart = DateTime.Now;
		TimeSpan minChunkTime = TimeSpan.MinValue;
		long num = engine.MaxBytesPerSecond;
		if (num > 0)
		{
			if (engine.MaxDownloads > 1)
			{
				num /= engine.MaxDownloads;
			}
			if (engine.MaxStreams > 1)
			{
				num /= engine.MaxStreams;
			}
			double value = (double)CHUNK_SIZE / (double)num;
			minChunkTime = TimeSpan.FromSeconds(value);
		}
		Cancel = new CancellationTokenSource();
		ParallelOptions parallelOptions = new ParallelOptions
		{
			CancellationToken = Cancel.Token,
			MaxDegreeOfParallelism = engine.MaxStreams
		};
		if (!Parallel.ForEach(source, parallelOptions, delegate(Tuple<long, long> chunk)
		{
			DownloadChunk(URL, LocalPath + TEMP_EXT, tempDir, chunk, chunks, chunkTimes, minChunkTime, downloadLocker, safeLocker);
		}).IsCompleted)
		{
			throw new IOException("Not all content was downloaded");
		}
	}

	private static void DownloadChunk(string remoteUrl, string localPath, string tempDir, Tuple<long, long> chunk, List<Tuple<long, long>> chunks, List<DateTime> chunkTimes, TimeSpan minChunkTime, object downloadLocker, object safeLocker)
	{
		DateTime now = DateTime.Now;
		int num = 3;
		string path = Smart.File.PathJoin(tempDir, Smart.File.Uuid() + ".dl-temp");
		if (remoteUrl.Contains("rsdsecure") || remoteUrl.Contains("rsddownload"))
		{
			remoteUrl = Smart.Rsd.GetSecureUrl(remoteUrl, "GET");
		}
		HttpWebRequest httpWebRequest = WebRequest.Create(remoteUrl) as HttpWebRequest;
		httpWebRequest.Method = "GET";
		httpWebRequest.AddRange(chunk.Item1, chunk.Item2);
		Smart.Log.Verbose("DownloadJob.Worker", $"Sending request for chunk {chunk.Item1} to {chunk.Item2}");
		int num2 = 0;
		do
		{
			try
			{
				using HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
				using FileStream destination = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write);
				httpWebResponse.GetResponseStream().CopyTo(destination);
			}
			catch (OperationCanceledException)
			{
				Smart.Log.Error("DownloadJob.Worker", "Download operation cancelled");
				throw;
			}
			catch (Exception ex2)
			{
				Smart.Log.Error("DownloadJob.Worker", "Error while downloading chunk: " + ex2.ToString());
				num2++;
				if (num2 >= num)
				{
					throw;
				}
				Smart.Thread.Wait(TimeSpan.FromMilliseconds(300.0));
				continue;
			}
			break;
		}
		while (num2 <= num);
		Smart.Log.Verbose("DownloadJob.Worker", $"Writing chunk {chunk.Item1} to {chunk.Item2}");
		num2 = 0;
		lock (safeLocker)
		{
			do
			{
				try
				{
					using FileStream fileStream2 = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
					using FileStream fileStream = new FileStream(localPath, FileMode.Open, FileAccess.Write, FileShare.Write);
					fileStream.Seek(chunk.Item1, SeekOrigin.Begin);
					fileStream2.CopyTo(fileStream);
				}
				catch (Exception ex3)
				{
					Smart.Log.Error("DownloadJob.Worker", "Error while moving chunk: " + ex3.ToString());
					num2++;
					if (num2 >= num)
					{
						throw;
					}
					Smart.Thread.Wait(TimeSpan.FromMilliseconds(300.0));
					continue;
				}
				break;
			}
			while (num2 < num);
		}
		if (minChunkTime > TimeSpan.MinValue)
		{
			TimeSpan timeSpan = DateTime.Now.Subtract(now);
			if (timeSpan < minChunkTime)
			{
				System.Threading.Thread.Sleep((int)(minChunkTime - timeSpan).TotalMilliseconds);
			}
		}
		lock (downloadLocker)
		{
			chunks.Remove(chunk);
			chunkTimes.Add(DateTime.Now);
		}
	}

	private void Unzip()
	{
		string text = LocalPath + TEMP_EXT;
		string text2 = UnzipPath;
		lock (zipLock)
		{
			int num = 1;
			do
			{
				string text3 = UnzipPath;
				if (text3.Length > 249)
				{
					text3 = text3.Substring(0, 250);
				}
				text3 = Path.GetDirectoryName(text3);
				text2 = Smart.File.PathJoin(text3, num.ToString());
				num++;
			}
			while (Smart.File.Exists(text2));
			Directory.CreateDirectory(text2);
		}
		if (text.ToLowerInvariant().Contains(".tar.gz"))
		{
			Smart.Zip.GZipExtract(text, text2);
			Smart.Log.Debug(TAG, "tar.gz successfully extracted");
		}
		else
		{
			Smart.Zip.Extract(text, text2);
			Smart.Log.Debug(TAG, "Zip successfully extracted");
		}
		string text4 = text2;
		string text5 = UnzipPath;
		string text6 = LocalPath;
		if (text6.ToLowerInvariant().EndsWith(".zip") || text6.ToLowerInvariant().EndsWith(".gz"))
		{
			text6 = Path.GetDirectoryName(text6);
		}
		bool flag = !text6.ToLowerInvariant().Contains(text5.ToLowerInvariant());
		if (Directory.Exists(text5) && flag)
		{
			Directory.Delete(text5, recursive: true);
		}
		if (Directory.Exists(text5))
		{
			int num2 = Directory.GetFiles(text4).Length;
			string[] directories = Directory.GetDirectories(text4);
			int num3 = directories.Length;
			if (num2 < 1 && num3 == 1)
			{
				string path = directories[0];
				path = Path.GetFileName(path);
				text4 = Path.Combine(text4, path);
				text5 = Path.Combine(text5, path);
			}
		}
		lock (zipLock)
		{
			Directory.Move(text4, text5);
			if (Directory.Exists(text2))
			{
				Directory.Delete(text2);
			}
			Smart.File.Delete(text);
		}
	}

	private void Cleanup()
	{
		Smart.Log.Info(TAG, $"Cleaning up {URL}");
		if (Directory.Exists(tempDir))
		{
			Directory.Delete(tempDir, recursive: true);
		}
		string text = LocalPath + TEMP_EXT;
		if (Smart.File.Exists(text))
		{
			System.IO.File.Move(text, LocalPath);
		}
	}
}
