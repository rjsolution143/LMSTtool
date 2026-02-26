using System.Collections.Generic;

namespace ISmart;

public interface IZip
{
	void Compress(string zipFile, List<string> filesToCompress);

	void Compress(string zipFile, List<string> filesToCompress, bool flat);

	void Compress(string zipFile, List<string> filesToCompress, bool flat, string password);

	void Extract(string zipFile, string destinationPath);

	void Extract(string zipFile, string destinationPath, string password);

	void GZipExtract(string gzipFile, string destinationPath);

	ITempZip ExtractTemp(string zipFile);

	ITempZip ExtractTemp(string zipFile, string password);

	void GZipDecompress(string gzFile, string destinationPath);

	byte[] Compress(byte[] bytes);

	byte[] Extract(byte[] bytes);
}
