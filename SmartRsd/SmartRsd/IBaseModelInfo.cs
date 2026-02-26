namespace SmartRsd;

public class IBaseModelInfo
{
	public string SaleModel { get; private set; }

	public string Model { get; private set; }

	public string Country { get; private set; }

	public string Carrier { get; private set; }

	public string SimInfo { get; private set; }

	public string SvnKitId { get; private set; }

	public string Key { get; private set; }

	public IBaseModelInfo(string saleModel, string model, string country, string carrier, string simInfo, string svnKitId)
	{
		SaleModel = saleModel.Trim();
		Model = model.Trim();
		Country = country.Trim();
		Carrier = carrier.Trim();
		SimInfo = simInfo.Trim();
		SvnKitId = svnKitId.Trim();
		Key = $"{Model.ToLower()},{Country.ToLower()},{Carrier.ToLower()},{SimInfo.ToLower()}";
	}
}
