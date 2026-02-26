namespace ISmart;

public class DeviceAttributes
{
	public string BlurId { get; set; } = string.Empty;


	public string FingerPrint { get; set; } = string.Empty;


	public string RoCarrier { get; set; } = string.Empty;


	public string FsgVersion { get; set; } = string.Empty;


	public string FlashId { get; set; } = string.Empty;


	public string FlexId { get; set; } = string.Empty;


	public string CarrierSku { get; set; } = string.Empty;


	public string Sku { get; set; } = string.Empty;


	public string ChannelId { get; set; } = string.Empty;


	public string BuildDisplay { get; set; } = string.Empty;


	public string ProductModel { get; set; } = string.Empty;


	public string InternalVersion { get; set; } = string.Empty;


	public DeviceAttributes(string blurId, string fingerPrint, string roCarrier, string fsgVersion, string flashId, string flexId, string carrierSku, string sku, string channelId, string buildDisplay, string productModel, string internalVersion)
	{
		BlurId = blurId;
		FingerPrint = fingerPrint;
		RoCarrier = roCarrier;
		FsgVersion = fsgVersion;
		FlashId = flashId;
		FlexId = flexId;
		CarrierSku = carrierSku;
		Sku = sku;
		ChannelId = channelId;
		BuildDisplay = buildDisplay;
		ProductModel = productModel;
		InternalVersion = internalVersion;
	}

	public DeviceAttributes()
	{
	}
}
