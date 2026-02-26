using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartRsd.AWSSigners;
using SmartRsd.FSB;

namespace SmartRsd;

public class FileDownloader
{
	public delegate bool AsyncExtractFile(string zipFilePathName, string destDir, string filename, bool encrypted, bool createMarker, long zipFileSize, ManualResetEvent doneEvent, string fileType, string finalDir);

	private const int MAX_BUFFER_SIZE = 1024000;

	private const int SMALL_ZIP_FILE_SIZE = 51200;

	private const int OVERFLOW_GUARD_LENGTH = 12288;

	private const int MAX_SDU_SIZE = 20480;

	private const string FAILED_TO_DOWNLOAD = "Failed to download ";

	private int mActiveUnzipProcessCount;

	private int mCurrentDisplayedPercent;

	private double mCurrentPercent;

	private SRPOperationRest mSrpOperation;

	private ManualResetEvent mUnzipGoEvent = new ManualResetEvent(initialState: true);

	private EventHandler mPostMessage;

	private long mBytesToNextPercent;

	private long mBytesFromLastPercent;

	private long mTotalDownloadSize;

	private long mTotalDownloadBytes;

	private int mDirIndex;

	public bool Stalled { get; set; }

	private string TAG => GetType().FullName;

	public FileDownloader(SRPOperationRest srpOperation, EventHandler postMessage, long totalDownloadSize)
	{
		mSrpOperation = srpOperation;
		mPostMessage = postMessage;
		mTotalDownloadSize = totalDownloadSize;
		mTotalDownloadBytes = totalDownloadSize;
		Stalled = false;
	}

	public FileDownloader(SRPOperationRest srpOperation, EventHandler postMessage)
	{
		mSrpOperation = srpOperation;
		mPostMessage = postMessage;
		Stalled = false;
	}

	public FileDownloader(SRPOperationRest srpOperation)
	{
		mSrpOperation = srpOperation;
		Stalled = false;
	}

	public long DownloadFile(string url, string localDir, out string localFileName)
	{
		Uri uri = null;
		try
		{
			uri = new Uri(url);
		}
		catch (Exception)
		{
			Smart.Log.Error(TAG, $"Bad Url: {url} Cannot download the file.");
		}
		if (uri == null)
		{
			localFileName = string.Empty;
			return 0L;
		}
		string fileName = Path.GetFileName(uri.LocalPath);
		long num = 0L;
		long num2 = 0L;
		long num3 = 0L;
		localFileName = Path.Combine(localDir, fileName);
		string text = localFileName + ".tmp";
		for (int i = 0; i < 3; i++)
		{
			if (num != 0L)
			{
				break;
			}
			Smart.Log.Info(TAG, $"Downloading file {localFileName} - try {i + 1}");
			try
			{
				HttpWebRequest httpWebRequest;
				if (AuthenticationRequired(url))
				{
					mSrpOperation.GenerateSignedUrl(url, "GET", out var signedUrl);
					httpWebRequest = (HttpWebRequest)WebRequest.Create(signedUrl);
				}
				else
				{
					httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
				}
				httpWebRequest.Method = "GET";
				mCurrentDisplayedPercent = (int)mCurrentPercent;
				using HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				num2 = httpWebResponse.ContentLength;
				Smart.Log.Debug(TAG, $"fileSize: {num2} bytes");
				if (mTotalDownloadSize == 0L)
				{
					mTotalDownloadBytes = num2;
					mBytesFromLastPercent = 0L;
					mBytesToNextPercent = 0L;
					mCurrentPercent = 0.0;
				}
				using Stream stream = httpWebResponse.GetResponseStream();
				stream.ReadTimeout = 240000;
				byte[][] buffers = GetBuffers(num2);
				using FileStream fileStream = new FileStream(text, FileMode.Create, FileAccess.Write, FileShare.None, buffers[0].Length, useAsync: true);
				int num4 = 0;
				int num5 = 0;
				int num6 = buffers[num4].Length;
				ManualResetEvent manualResetEvent = new ManualResetEvent(initialState: true);
				AsyncStreamInfo asyncStreamInfo = new AsyncStreamInfo(fileStream, manualResetEvent);
				AsyncCallback callback = EndWrite;
				num3 = 0L;
				if (mCurrentPercent == 0.0)
				{
					mPostMessage?.Invoke(this, new PostMessage(PostMessage.DISPLAY_PROGRESS, "Percent", 0L));
				}
				int num7;
				while ((num7 = stream.Read(buffers[num4], num5, num6)) > 0)
				{
					num5 += num7;
					num6 -= num7;
					num3 += num7;
					mBytesFromLastPercent += num7;
					UpdateDownloadStatus();
					if (num6 < 12288 || mActiveUnzipProcessCount == 0)
					{
						manualResetEvent.WaitOne();
						if (num5 < 20480)
						{
							fileStream.Write(buffers[num4], 0, num5);
						}
						else
						{
							manualResetEvent.Reset();
							fileStream.BeginWrite(buffers[num4], 0, num5, callback, asyncStreamInfo);
							num4 ^= 1;
						}
						num5 = 0;
						num6 = buffers[num4].Length;
					}
					if (Stalled)
					{
						Smart.Thread.Wait(TimeSpan.FromMilliseconds(1000.0));
					}
				}
				manualResetEvent.WaitOne();
				if (num5 > 0)
				{
					fileStream.Write(buffers[num4], 0, num5);
				}
				fileStream.Flush();
				num = fileStream.Length;
			}
			catch (Exception ex2)
			{
				num = 0L;
				Smart.Log.Error(TAG, "Exception in DownloadFile. Error: " + ex2.Message);
				Smart.Log.Verbose(TAG, ex2.StackTrace);
			}
			if (num2 > 0 && num != num2)
			{
				mBytesFromLastPercent -= num3;
				UpdateDownloadStatus();
				num = 0L;
			}
			else if (num > 0)
			{
				try
				{
					File.Move(text, localFileName);
				}
				catch (Exception ex3)
				{
					Smart.Log.Error(TAG, $"Failed to rename file {text} to {localFileName}. ErrorMsg: {ex3.Message}");
				}
			}
		}
		return num;
	}

	public bool ExtractFile(string zipFilePathName, string destDir, string filename, bool encrypted, bool createMarker, long zipFileSize, ManualResetEvent doneEvent, string fileType, string finalDir)
	{
		mUnzipGoEvent.WaitOne();
		bool result = true;
		try
		{
			bool flag = true;
			if (zipFileSize > 102400000 && ++mActiveUnzipProcessCount > 1)
			{
				mUnzipGoEvent.Reset();
			}
			Thread thread = null;
			if (createMarker)
			{
				int waitMs = (zipFilePathName.EndsWith(".gz") ? 300000 : 2000);
				thread = new Thread((ThreadStart)delegate
				{
					CreateUncompleteRecord(filename, zipFilePathName, waitMs);
				});
				thread.Start();
			}
			try
			{
				if (zipFilePathName.EndsWith(".gz"))
				{
					Smart.Zip.GZipExtract(zipFilePathName, destDir);
				}
				else if (zipFilePathName.EndsWith(".zip"))
				{
					Smart.Zip.Extract(zipFilePathName, destDir);
				}
				else
				{
					Smart.Log.Debug(TAG, $"Unrecognized archive type: {zipFilePathName} - skipping unzipping");
					flag = false;
				}
			}
			catch (Exception ex)
			{
				flag = false;
				result = false;
				string message = Smart.Locale.Xlate("Failed to unzip file") + ": " + zipFilePathName + "\r\n" + Smart.Locale.Xlate("Destination") + ": " + destDir + "\r\n" + Smart.Locale.Xlate("Error Message") + ": " + ex.Message;
				Smart.Log.Error(TAG, message);
				Task.Run(delegate
				{
					//IL_001f: Unknown result type (might be due to invalid IL or missing references)
					string text2 = Smart.Locale.Xlate("Unzipping Error");
					Smart.User.MessageBox(text2, message, (MessageBoxButtons)0, (MessageBoxIcon)16);
				});
			}
			if (flag)
			{
				File.Delete(zipFilePathName);
				if (!string.IsNullOrEmpty(finalDir))
				{
					Utilities.MoveDirectory(destDir, finalDir);
				}
				if (createMarker && !thread.Join(1000))
				{
					thread.Abort();
				}
				if (Directory.Exists(filename))
				{
					string text = Path.Combine(filename, "UnzipFailedTag.txt");
					if (File.Exists(text))
					{
						File.Delete(text);
						Smart.Log.Verbose(TAG, "Deleted uncompleted marker " + text);
					}
					if (fileType == "FSBICONS")
					{
						FsbLocalData.EncryptComplaintProblemInfoFile(filename);
					}
				}
			}
			string path = filename;
			string downloadFile = JsonMatrixParser.Instance.FileNameToDownloadInfoLookup[filename].DownloadFile;
			if (downloadFile != string.Empty)
			{
				path = Path.Combine(destDir, downloadFile);
			}
			if (File.Exists(path))
			{
				if (encrypted)
				{
					string content;
					using (StreamReader streamReader = new StreamReader(path))
					{
						content = streamReader.ReadToEnd();
					}
					File.Delete(path);
					Utilities.AesEncryptFile(filename, content, FileAttributes.ReadOnly, fileType);
				}
				else
				{
					File.SetAttributes(filename, FileAttributes.ReadOnly);
				}
			}
		}
		catch (Exception ex2)
		{
			Smart.Log.Error(TAG, ex2.Message);
			Smart.Log.Verbose(TAG, ex2.ToString());
			result = false;
		}
		finally
		{
			if (zipFileSize > 102400000 && --mActiveUnzipProcessCount <= 1)
			{
				mUnzipGoEvent.Set();
			}
			doneEvent?.Set();
		}
		return result;
	}

	public void DownloadAndUnzipFile(string fileURL, string fileParentDir, bool createDir, string fileType, string filename, ManualResetEvent doneEvent, List<KeyValuePair<AsyncExtractFile, IAsyncResult>> asyncExtractFileResults, ref string result)
	{
		string finalDir = null;
		string fileDir = fileParentDir;
		Smart.Log.Debug(TAG, $"Downloading... fileUrl={fileURL} fileParentDir={fileParentDir} fileType={fileType} filename={filename} createDir={createDir}");
		if (createDir)
		{
			finalDir = filename;
			fileDir = Path.Combine(fileParentDir, mDirIndex.ToString());
			mDirIndex++;
			if (Directory.Exists(fileDir))
			{
				Utilities.DeleteDirectory(fileDir);
			}
			Directory.CreateDirectory(fileDir);
			new Thread((ThreadStart)delegate
			{
				CreateUncompleteRecord(fileDir, filename);
			}).Start();
		}
		long zipFileSize;
		if ((zipFileSize = DownloadFile(fileURL, fileDir, out var localFileName)) > 0)
		{
			try
			{
				FileTypeInfo fileTypeInfo = Configurations.FileTypeInfos[fileType];
				if (fileTypeInfo.IsFolder)
				{
					AsyncExtractFile asyncExtractFile = ExtractFile;
					if (File.Exists(filename))
					{
						File.SetAttributes(filename, FileAttributes.Normal);
						File.Delete(filename);
					}
					IAsyncResult value = asyncExtractFile.BeginInvoke(localFileName, fileDir, filename, encrypted: false, !createDir, zipFileSize, doneEvent, fileType, finalDir, null, null);
					asyncExtractFileResults.Add(new KeyValuePair<AsyncExtractFile, IAsyncResult>(asyncExtractFile, value));
				}
				else if (fileTypeInfo.Unzipped)
				{
					AsyncExtractFile asyncExtractFile2 = ExtractFile;
					IAsyncResult value2 = asyncExtractFile2.BeginInvoke(localFileName, fileDir, filename, fileTypeInfo.Encrypted, createMarker: false, zipFileSize, doneEvent, fileType, finalDir, null, null);
					asyncExtractFileResults.Add(new KeyValuePair<AsyncExtractFile, IAsyncResult>(asyncExtractFile2, value2));
				}
				else
				{
					if (fileTypeInfo.Encrypted)
					{
						string content;
						using (StreamReader streamReader = new StreamReader(localFileName))
						{
							content = streamReader.ReadToEnd();
						}
						File.Delete(localFileName);
						Utilities.AesEncryptFile(filename, content, FileAttributes.ReadOnly, fileType);
					}
					doneEvent?.Set();
				}
				return;
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, "Exception in DownloadAnUnzipFile. Error message: " + ex.Message);
				Smart.Log.Verbose(TAG, ex.StackTrace);
				doneEvent?.Set();
				return;
			}
		}
		Smart.Log.Error(TAG, $"Fail to download file: {filename} from URL: {fileURL}");
		result = "Failed to download " + fileType + " file";
		doneEvent?.Set();
	}

	public long GetFileSize(string url)
	{
		if (string.IsNullOrEmpty(url))
		{
			return 0L;
		}
		long num = 0L;
		try
		{
			HttpWebRequest httpWebRequest;
			if (AuthenticationRequired(url))
			{
				mSrpOperation.GenerateSignedUrl(url, "HEAD", out var signedUrl);
				httpWebRequest = (HttpWebRequest)WebRequest.Create(signedUrl);
			}
			else
			{
				httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			}
			httpWebRequest.Method = "HEAD";
			using WebResponse webResponse = httpWebRequest.GetResponse();
			num = webResponse.ContentLength;
		}
		catch (Exception ex)
		{
			Smart.Log.Error("SmartRsd.FileDownloader", "Exception - errorMsg: " + ex.Message);
		}
		Smart.Log.Debug(TAG, $"Determined file size {Smart.Convert.ByteSizeToDisplay(num, true)} for file {url}");
		return num;
	}

	public void UpdateTotalFileSize(long size)
	{
		mTotalDownloadSize += size;
		mTotalDownloadBytes += size;
	}

	private void EndWrite(IAsyncResult result)
	{
		AsyncStreamInfo asyncStreamInfo = (AsyncStreamInfo)result.AsyncState;
		asyncStreamInfo.AsyncFileStream.EndWrite(result);
		asyncStreamInfo.WriteDoneEvent.Set();
	}

	private byte[][] GetBuffers(long fileSize)
	{
		int num = ((fileSize >= 1024000) ? 1024000 : ((int)fileSize + 12288));
		return new byte[2][]
		{
			new byte[num],
			new byte[num]
		};
	}

	public void CreateUncompleteRecord(string dir, string zipFilePathName, int waitMs = 1200)
	{
		try
		{
			int num = ((waitMs > 5000) ? 500 : 100);
			for (int num2 = waitMs; num2 > 0; num2 -= num)
			{
				if (Directory.Exists(dir))
				{
					string text = Path.Combine(dir, "UnzipFailedTag.txt");
					string contents = zipFilePathName + "," + dir + "," + DateTime.Now;
					File.WriteAllText(text, contents, Encoding.UTF8);
					Smart.Log.Debug(TAG, "Created incomplete marker " + text);
					break;
				}
				Thread.Sleep(TimeSpan.FromMilliseconds(num));
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Debug(TAG, "Exception - msg: " + ex.Message);
		}
	}

	private static void SetRequestAuthorization(string fileName, HttpWebRequest request, byte[] content, string contentType)
	{
		Uri endpointUri = new Uri($"https://{Configurations.BucketName}.s3.amazonaws.com/{fileName}");
		string text;
		Dictionary<string, string> dictionary;
		if (content == null)
		{
			text = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
			dictionary = new Dictionary<string, string> { { "X-Amz-Content-SHA256", text } };
		}
		else
		{
			text = AWS4SignerBase.ToHexString(AWS4SignerBase.CanonicalRequestHashAlgorithm.ComputeHash(content), lowercase: true);
			dictionary = new Dictionary<string, string>
			{
				{ "X-Amz-Content-SHA256", text },
				{
					"content-length",
					content.Length.ToString()
				},
				{ "content-type", contentType }
			};
		}
		string value = new AWS4SignerForAuthorizationHeader
		{
			EndpointUri = endpointUri,
			HttpMethod = request.Method,
			Service = "s3",
			Region = Configurations.RegionName
		}.ComputeSignature(dictionary, "", text, Configurations.AWSAccessKey, Configurations.AWSSecretKey);
		dictionary.Add("Authorization", value);
		foreach (string key in dictionary.Keys)
		{
			if (!key.Equals("host", StringComparison.OrdinalIgnoreCase))
			{
				if (key.Equals("content-length", StringComparison.OrdinalIgnoreCase))
				{
					request.ContentLength = long.Parse(dictionary[key]);
				}
				else if (key.Equals("content-type", StringComparison.OrdinalIgnoreCase))
				{
					request.ContentType = dictionary[key];
				}
				else
				{
					request.Headers.Add(key, dictionary[key]);
				}
			}
		}
		request.Headers.Add("Pragma", "akamai-x-cache-on, akamai-x-cache-remote-on, akamai-x-check-cacheable, akamai-x-get-cache-key, akamai-x-get-extracted-values, akamai-x-get-nonces, akamai-x-get-ssl-client-session-id, akamai-x-get-true-cache-key, akamai-x-serial-no, akamai-x-get-request-id, akamai-x-request-trace, akamai-x--meta-trace, akamai-x-get-extracted-values");
	}

	private void UpdateDownloadStatus()
	{
		if (mPostMessage == null)
		{
			return;
		}
		lock (this)
		{
			if (mBytesFromLastPercent < mBytesToNextPercent && mBytesFromLastPercent >= 0)
			{
				return;
			}
			double num = mCurrentPercent;
			mCurrentPercent += (double)mBytesFromLastPercent * 100.0 / (double)mTotalDownloadBytes;
			int num2 = (int)mCurrentPercent;
			if (num2 < 0)
			{
				Smart.Log.Debug(TAG, $"### percent = {num2}");
				Smart.Log.Debug(TAG, $"currentPercent={num} mBytesFromLastPercent={mBytesFromLastPercent} mTotalDownloadBytes={mTotalDownloadBytes}");
				num2 = 0;
				mCurrentPercent = 0.0;
				mBytesFromLastPercent = 0L;
			}
			if (num2 != mCurrentDisplayedPercent)
			{
				mCurrentDisplayedPercent = num2;
				if (num2 <= 100)
				{
					mPostMessage(this, new PostMessage(PostMessage.DISPLAY_PROGRESS, "Percent", num2));
				}
			}
			mBytesFromLastPercent = 0L;
			mBytesToNextPercent = (long)((1.0 + (double)num2 - mCurrentPercent) * (double)(mTotalDownloadBytes / 100));
		}
	}

	private bool AuthenticationRequired(string url)
	{
		bool result = false;
		foreach (string key in Configurations.SecureHostToGenerateSignedURLs.Keys)
		{
			if (url.Contains(key))
			{
				result = true;
				break;
			}
		}
		return result;
	}
}
