namespace SmartRsd;

internal class FileParameter
{
	public byte[] FileContent { get; private set; }

	public string FileName { get; private set; }

	public string ContentType { get; private set; }

	public FileParameter(byte[] fileContent, string fileName, string contentType)
	{
		FileContent = fileContent;
		FileName = fileName;
		ContentType = contentType;
	}
}
