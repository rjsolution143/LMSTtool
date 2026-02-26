using System.Collections.Generic;

namespace SmartRsd;

public class JsonMatrix
{
	public string Carrier { get; private set; }

	public string InternalName { get; private set; }

	public string CarrierCode { get; private set; }

	public string Version { get; private set; }

	public string LastUpdated { get; private set; }

	public List<JsonPhoneModel> PhoneModels { get; private set; } = new List<JsonPhoneModel>();


	public bool FullyParsed { get; private set; }

	public MatrixType MxType { get; private set; }

	public Dictionary<string, ModelDef> ModelNameToModelDefLookUp { get; private set; } = new Dictionary<string, ModelDef>();


	private string TAG => GetType().FullName;

	public JsonMatrix(MatrixDef matrixDef, MatrixType matrixType)
	{
		string text = ((matrixDef.matrixType != null) ? matrixDef.matrixType : string.Empty).Trim();
		if (text != matrixType.ToString())
		{
			Smart.Log.Error(TAG, $"This {text} matrix is in the folder of {matrixType.ToString()} matrices");
			return;
		}
		InternalName = ((matrixDef.internalName != null) ? matrixDef.internalName : string.Empty).Trim();
		Carrier = ((matrixDef.carrier != null) ? matrixDef.carrier : string.Empty).Trim();
		CarrierCode = ((matrixDef.carrierCode != null) ? matrixDef.carrierCode : string.Empty).Trim();
		Version = ((matrixDef.version != null) ? matrixDef.version : string.Empty).Trim();
		LastUpdated = ((matrixDef.lastUpdated != null) ? matrixDef.lastUpdated : string.Empty).Trim();
		FullyParsed = JsonMatrixParser.Instance.MatrixTypeToMatrixInfoLookup[matrixType].FullyParsed;
		MxType = matrixType;
		if (FullyParsed && !JsonMatrixParser.Instance.CarrierToModelsLookup.ContainsKey(InternalName))
		{
			JsonMatrixParser.Instance.CarrierToModelsLookup.Add(InternalName, new List<string>());
		}
		if (matrixDef.phoneModels != null)
		{
			ModelDef[] phoneModels = matrixDef.phoneModels;
			foreach (ModelDef modelDef in phoneModels)
			{
				JsonPhoneModel jsonPhoneModel = new JsonPhoneModel(modelDef, this);
				if (jsonPhoneModel.Valid)
				{
					PhoneModels.Add(jsonPhoneModel);
					ModelNameToModelDefLookUp[jsonPhoneModel.Name] = modelDef;
				}
			}
		}
		else if (FullyParsed)
		{
			Smart.Log.Error(TAG, string.Format("{0} entry is missing [Carrier {1}]", "phoneModels", InternalName));
		}
	}

	public JsonMatrix(JsonPhoneModel model, MatrixType matrixType)
	{
		Carrier = model.Matrix.Carrier;
		InternalName = model.Matrix.InternalName;
		CarrierCode = model.Matrix.CarrierCode;
		Version = model.Matrix.Version;
		LastUpdated = model.Matrix.LastUpdated;
		FullyParsed = JsonMatrixParser.Instance.MatrixTypeToMatrixInfoLookup[matrixType].FullyParsed;
		MxType = matrixType;
		if (FullyParsed && !JsonMatrixParser.Instance.CarrierToModelsLookup.ContainsKey(InternalName))
		{
			JsonMatrixParser.Instance.CarrierToModelsLookup.Add(InternalName, new List<string>());
		}
		ModelDef modelDef = model.Matrix.ModelNameToModelDefLookUp[model.Name];
		JsonPhoneModel jsonPhoneModel = new JsonPhoneModel(modelDef, this);
		if (jsonPhoneModel.Valid)
		{
			PhoneModels.Add(jsonPhoneModel);
			ModelNameToModelDefLookUp[jsonPhoneModel.Name] = modelDef;
		}
	}

	public void Delete()
	{
		foreach (JsonPhoneModel phoneModel in PhoneModels)
		{
			phoneModel.Delete();
		}
	}
}
