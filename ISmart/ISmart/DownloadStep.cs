namespace ISmart;

public enum DownloadStep
{
	Initialize,
	GetSize,
	Allocate,
	Download,
	Unzip,
	Cleanup,
	Downloaded
}
