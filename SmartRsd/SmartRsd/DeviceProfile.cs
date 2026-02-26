using System.Collections.Generic;
using ISmart;

namespace SmartRsd;

public class DeviceProfile
{
	public string Carrier { get; private set; } = string.Empty;


	public string CarrierCode { get; private set; } = string.Empty;


	public string PhoneModel { get; private set; } = string.Empty;


	public string PhoneName { get; private set; } = string.Empty;


	public string Technology { get; private set; } = string.Empty;


	public string ModelImageFilename { get; private set; } = string.Empty;


	public string ModelConfigFilename { get; private set; } = string.Empty;


	public JsonConfig Config { get; set; } = new JsonConfig();


	public string ConfigId { get; set; } = string.Empty;


	public SortedList<UseCase, RecipeDescriptor> UseCasesToRecipes { get; set; } = new SortedList<UseCase, RecipeDescriptor>();


	public DeviceProfile(string carrier, string carrierCode, string phoneModel, string phoneName, string technology, string modelImageFileName, string modelInfoFileName, JsonConfig config)
	{
		Carrier = carrier;
		CarrierCode = carrierCode;
		PhoneModel = phoneModel;
		PhoneName = phoneName;
		Technology = technology;
		ModelImageFilename = modelImageFileName;
		ModelConfigFilename = modelInfoFileName;
		Config = config;
	}

	public DeviceProfile()
	{
	}

	public DeviceProfile(DeviceProfile fromDeviceProfile)
	{
		Carrier = fromDeviceProfile.Carrier;
		CarrierCode = fromDeviceProfile.CarrierCode;
		PhoneModel = fromDeviceProfile.PhoneModel;
		PhoneName = fromDeviceProfile.PhoneName;
		Technology = fromDeviceProfile.Technology;
		ModelImageFilename = fromDeviceProfile.ModelImageFilename;
		ModelConfigFilename = fromDeviceProfile.ModelConfigFilename;
		Config = fromDeviceProfile.Config;
		ConfigId = fromDeviceProfile.ConfigId;
		UseCasesToRecipes = new SortedList<UseCase, RecipeDescriptor>(fromDeviceProfile.UseCasesToRecipes);
	}
}
