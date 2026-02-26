using System;
using System.ComponentModel;

namespace ISmart;

public interface IDevice : INotifyPropertyChanged
{
	string ID { get; set; }

	string Unique { get; }

	string SerialNumber { get; set; }

	string SerialNumber2 { get; set; }

	string ModelId { get; set; }

	DeviceMode LastMode { get; }

	DeviceMode Mode { get; }

	bool UnknownMode { get; }

	DeviceMode DetectMode { get; set; }

	bool Locked { get; set; }

	IResultLogger Log { get; set; }

	IPrompt Prompt { get; }

	string RoCarrier { get; set; }

	string RecordId { get; set; }

	string IP { get; set; }

	string ManufacturingDate { get; set; }

	bool ManualDevice { get; set; }

	string UserSelectedCarrier { get; set; }

	string UserSelectedModel { get; set; }

	string UserSerialNumber { get; set; }

	DeviceType Type { get; set; }

	string Group { get; set; }

	bool WiFiOnlyDevice { get; set; }

	string GSN { get; set; }

	string PSN { get; set; }

	bool Communicating { get; set; }

	DateTime LastConnected { get; set; }

	bool MultiUp { get; set; }

	bool Removed { get; set; }

	string ScannedGsn { get; set; }

	string ScannedTrackId { get; set; }

	bool ZeroTouchDevice { get; set; }

	bool InvalidSerialNumber { get; set; }

	int PortIndex { get; set; }

	bool Automated { get; set; }

	string PnpDbccName { get; set; }

	bool OutOfProfile { get; set; }

	void ReportMode(DeviceMode newMode);

	string GetLogInfoValue(string name);

	void MergeDevice(IDevice otherDevice);
}
