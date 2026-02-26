using System.Collections.Generic;

namespace ISmart;

public struct DownloadResponse
{
	public bool Success { get; private set; }

	public string Message { get; set; }

	public string Status { get; set; }

	public bool Downloading { get; private set; }

	public int DownloadsAdded { get; private set; }

	public int TotalDownloads { get; private set; }

	public string DownloadList { get; private set; }

	public int Percentage { get; private set; }

	public long FileSize { get; private set; }

	public static DownloadResponse BlankStatus
	{
		get
		{
			DownloadResponse result = default(DownloadResponse);
			result.Success = false;
			result.Status = DownloadStep.Initialize.ToString();
			result.Message = "UNKNOWN";
			result.Downloading = false;
			result.DownloadsAdded = -1;
			result.TotalDownloads = -1;
			result.DownloadList = string.Empty;
			result.Percentage = 0;
			result.FileSize = -1L;
			return result;
		}
	}

	public static DownloadResponse FromData(dynamic data)
	{
		DownloadResponse result = default(DownloadResponse);
		result.Success = bool.Parse(data["success"].ToString());
		result.Status = data["status"];
		result.Message = data["message"];
		if (result.Message == null)
		{
			result.Message = string.Empty;
		}
		result.Downloading = bool.Parse(data["downloading"].ToString());
		result.DownloadsAdded = int.Parse(data["downloadsadded"].ToString());
		result.TotalDownloads = int.Parse(data["totaldownloads"].ToString());
		result.DownloadList = data["downloadlist"].ToString();
		result.Percentage = int.Parse(data["percentage"].ToString());
		if (data["filesize"] != null)
		{
			result.FileSize = long.Parse(data["filesize"].ToString());
		}
		else
		{
			result.FileSize = -1L;
		}
		return result;
	}

	public SortedList<string, string> ToData()
	{
		return new SortedList<string, string>
		{
			["success"] = Success.ToString(),
			["message"] = Message,
			["status"] = Status.ToString(),
			["downloading"] = Downloading.ToString(),
			["downloadsadded"] = DownloadsAdded.ToString(),
			["totaldownloads"] = TotalDownloads.ToString(),
			["downloadlist"] = DownloadList.ToString(),
			["percentage"] = Percentage.ToString(),
			["filesize"] = FileSize.ToString()
		};
	}

	public DownloadResponse(bool downloading, string status, int percentage, int downloadsAdded, int totalDownloads, string downloadList, long fileSize)
	{
		Success = true;
		Status = status;
		Percentage = percentage;
		Message = "UNKNOWN";
		Downloading = downloading;
		DownloadsAdded = downloadsAdded;
		TotalDownloads = totalDownloads;
		DownloadList = downloadList;
		FileSize = fileSize;
	}
}
