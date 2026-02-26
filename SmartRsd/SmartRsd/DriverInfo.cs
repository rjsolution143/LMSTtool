namespace SmartRsd;

public class DriverInfo
{
	public string file_type { get; set; }

	public string driver_name { get; set; }

	public string filename { get; set; }

	public string extranet_url { get; set; }

	public string device_supported { get; set; }

	public string create_folder { get; set; }

	public DriverInfo(string fileType, string driverName, string fileName, string extranetUrl, string deviceSupported, bool createFolder)
	{
		file_type = fileType;
		driver_name = driverName;
		filename = fileName;
		extranet_url = extranetUrl;
		device_supported = deviceSupported;
		create_folder = createFolder.ToString();
	}
}
