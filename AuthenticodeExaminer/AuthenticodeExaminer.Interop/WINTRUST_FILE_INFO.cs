using System;

namespace AuthenticodeExaminer.Interop;

internal struct WINTRUST_FILE_INFO
{
	public uint cbStruct;

	public IntPtr pcwszFilePath;

	public IntPtr hFile;

	public IntPtr pgKnownSubject;
}
