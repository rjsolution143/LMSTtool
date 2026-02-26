namespace SmartRsd;

public class DownloadInfo
{
	public string FileType { get; private set; }

	public string FileUrl { get; private set; }

	public bool CreateFolder { get; private set; }

	public string DownloadFile { get; private set; }

	public bool Reserved { get; private set; }

	public long FileSize { get; set; }

	public string Model { get; private set; }

	public string Carrier { get; private set; }

	public bool SentToSmartHelper { get; set; }

	public DownloadInfo(string fileType, string fileUrl, bool createFolder, bool reserved = false, string downloadFile = "", string model = "", string carrier = "")
	{
		FileType = fileType;
		FileUrl = fileUrl;
		CreateFolder = createFolder;
		DownloadFile = downloadFile;
		Reserved = reserved;
		Model = model;
		Carrier = carrier;
	}
}
