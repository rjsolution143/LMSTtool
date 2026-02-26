namespace ISmart;

public struct StationDescriptor
{
	public static StationDescriptor Empty = new StationDescriptor(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

	public string SavedShopId { get; private set; }

	public string RegionId { get; private set; }

	public string StationId { get; private set; }

	public string StationName { get; private set; }

	public string UserName { get; private set; }

	public string ShopId
	{
		get
		{
			string text = SavedShopId;
			if (text.StartsWith("L-"))
			{
				text = text.Substring("L-".Length);
			}
			return text;
		}
	}

	public StationDescriptor(string shopId, string regionId, string stationId, string stationName, string userName)
	{
		SavedShopId = shopId;
		RegionId = regionId;
		StationId = stationId;
		StationName = stationName;
		UserName = userName;
	}

	public StationDescriptor(string stationInfo, string stationName, string userName)
	{
		string[] array = stationInfo.Split(new char[1] { '|' });
		StationId = array[0];
		SavedShopId = ((array.Length > 1) ? array[1] : string.Empty);
		RegionId = ((array.Length > 2) ? array[2] : string.Empty);
		StationName = stationName;
		UserName = userName;
	}

	public override bool Equals(object obj)
	{
		return ToString().Equals(obj.ToString());
	}

	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}

	public override string ToString()
	{
		return StationId + "|" + SavedShopId + "|" + RegionId;
	}

	public string ToId()
	{
		return $"{RegionId}-{SavedShopId}-{StationId}";
	}
}
