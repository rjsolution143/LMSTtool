namespace ISmart;

public class ExtDownloadInfo
{
	public string UserName { get; private set; }

	public string Password { get; private set; }

	public string Url { get; private set; }

	public string FileType { get; private set; }

	public bool IsFolder { get; private set; }

	public bool CreateFolder { get; private set; }

	public bool Unzipped { get; private set; }

	public bool Encrypted { get; private set; }

	public string LocalFileExtensions { get; private set; }

	public ExtDownloadInfo(string userName, string password, string url, string fileType, bool isFolder, bool createFolder, bool unzipped, bool encrypted, string localFileExtensions)
	{
		UserName = userName;
		Password = password;
		Url = url;
		FileType = fileType;
		IsFolder = isFolder;
		CreateFolder = createFolder;
		Unzipped = unzipped;
		Encrypted = encrypted;
		LocalFileExtensions = localFileExtensions;
	}
}
