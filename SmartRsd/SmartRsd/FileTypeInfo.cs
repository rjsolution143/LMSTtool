namespace SmartRsd;

public class FileTypeInfo
{
	public string ParentDir { get; private set; }

	public bool IsFolder { get; private set; }

	public bool Unzipped { get; private set; }

	public bool Encrypted { get; private set; }

	public long AverageSize { get; private set; }

	public string LocalFileExtensions { get; private set; }

	public FileTypeInfo(string parentDir, bool isFolder, bool unzipped, bool encrypted, long averageSize, string localFileExtensions)
	{
		ParentDir = parentDir;
		IsFolder = isFolder;
		Unzipped = unzipped;
		Encrypted = encrypted;
		AverageSize = averageSize;
		LocalFileExtensions = localFileExtensions;
	}
}
