using System.Collections.Generic;

namespace ISmart;

public interface IDownloadRequest
{
	Login Credentials { get; }

	string AppName { get; }

	string AppVersion { get; }

	string FormatVersion { get; }

	string Target { get; }

	DownloadRequestType RequestType { get; }

	string Json { get; }

	bool Available { get; }

	List<SortedList<string, string>> Downloads { get; }

	void AddDownload(string name, string remoteUrl, string localFile, bool unzip, bool createFolder);

	DownloadResponse Prioritize(string remoteUrl);

	DownloadResponse Status(string remoteUrl);

	DownloadResponse Status();

	DownloadResponse RequestDownloads();

	DownloadResponse Open();

	void Load(string jsonContent);
}
