using System.IO;

namespace SmartRsd;

public class JsonFileEntry
{
	public const string MODEL_IMAGE_FILE = "DEVICEIMAGEFILE";

	public const string MODEL_CONFIG_FILE = "DEVICECONFIGFILE";

	public string Type { get; private set; }

	public string FileName { get; private set; }

	public string FileURL { get; private set; }

	public JsonPhoneModel Model { get; private set; }

	private string TAG => GetType().FullName;

	public JsonFileEntry(FileEntryDef fileEntry, JsonPhoneModel model)
	{
		Model = model;
		Type = ((fileEntry.type != null) ? fileEntry.type : string.Empty).Trim();
		if (Type.Length == 0 && Model.Matrix.FullyParsed)
		{
			Smart.Log.Debug(TAG, string.Format("{0} entry is empty missing [PhoneModel {1}] [Carrier {2}]", "type", Model.Name, Model.Matrix.InternalName));
		}
		FileName = ((fileEntry.fileName != null) ? fileEntry.fileName : string.Empty).Trim();
		if (FileName.Length == 0 && Model.Matrix.FullyParsed)
		{
			Smart.Log.Debug(TAG, string.Format("{0} is empty or missing [PhoneModel {1}] [Carrier {2}]", "fileName", Model.Name, Model.Matrix.InternalName));
		}
		FileURL = ((fileEntry.fileURL != null) ? fileEntry.fileURL : string.Empty).Trim();
		if (FileURL.Length == 0 && Model.Matrix.FullyParsed)
		{
			Smart.Log.Debug(TAG, string.Format("{0} is empty or missing [PhoneModel {1}] [Carrier {2}]", "fileURL", Model.Name, Model.Matrix.InternalName));
		}
		if (Type.Length > 0 && FileURL.Length > 0 && FileName.Length > 0 && Model.Matrix.FullyParsed && Configurations.FileTypeInfos.ContainsKey(Type))
		{
			string key = Path.Combine(Configurations.FileTypeInfos[Type].ParentDir, FileName);
			if (!JsonMatrixParser.Instance.FileNameToDownloadInfoLookup.ContainsKey(key))
			{
				DownloadInfo value = new DownloadInfo(Type, FileURL, createFolder: false);
				JsonMatrixParser.Instance.FileNameToDownloadInfoLookup.Add(key, value);
			}
		}
	}
}
