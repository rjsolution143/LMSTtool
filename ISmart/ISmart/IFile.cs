using System;
using System.Collections.Generic;
using System.IO;

namespace ISmart;

public interface IFile
{
	string StorageDirName { get; set; }

	string LogName { get; set; }

	string CommonStorageDir { get; }

	string ResourceNameToFilePath(string name, string extension);

	string ResourceNameToFilePathMulti(string name, List<string> extensions);

	string TempFolder();

	string Uuid();

	string FakeUid(string seed);

	Stream ReadStream(string filePath);

	Stream WriteStream(string filePath);

	Stream WriteStream(string filePath, FileMode mode);

	void CopyStream(Stream input, Stream output);

	void CopyStream(Stream input, Stream output, Func<byte[], byte[]> modifier);

	string ReadText(string filePath);

	void WriteText(string filePath, string text);

	bool Exists(string filePath);

	long FileSize(string filePath);

	void Delete(string filePath);

	void Remove(string folderPath);

	string PathJoin(string path1, string path2);

	List<string> FindFiles(string searchPattern, string path, bool recursive);

	void MirrorFiles(string sourcePath, string destinationPath);

	void FileCopy(string source, string destination, bool overwrite);

	void ClipboardWrite(string content);

	string ClipboardRead();

	List<string> FindApps();

	string RunCommand(string command);
}
