using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ISmart;

namespace SmartRsd;

public class JsonMatrixParser
{
	private const int MD5_HEX_LEN = 32;

	private static JsonMatrixParser sThis;

	private List<JsonMatrix> mAllMatrixes = new List<JsonMatrix>();

	public Dictionary<string, DownloadInfo> FileNameToDownloadInfoLookup { get; private set; } = new Dictionary<string, DownloadInfo>();


	public Dictionary<DetectionKey, List<JsonConfig>> DetectionKeyToJsonConfigLookup { get; private set; } = new Dictionary<DetectionKey, List<JsonConfig>>(new DetectionKeyComparer());


	public Dictionary<DetectionKey, List<JsonConfig>> AllDetectionKeyToJsonConfigLookup { get; private set; } = new Dictionary<DetectionKey, List<JsonConfig>>(new DetectionKeyComparer());


	public Dictionary<string, JsonPhoneModel> ModelIdToPhoneModelookup { get; private set; } = new Dictionary<string, JsonPhoneModel>();


	public Dictionary<string, List<string>> ModelToCarriersLookup { get; private set; } = new Dictionary<string, List<string>>();


	public Dictionary<string, string> AllModelCarrierToFlexModelLookup { get; private set; } = new Dictionary<string, string>();


	public Dictionary<string, JsonPhoneModel> AllModelIdToPhoneModelLookup { get; private set; } = new Dictionary<string, JsonPhoneModel>();


	public Dictionary<string, List<string>> CarrierToModelsLookup { get; private set; } = new Dictionary<string, List<string>>();


	public Dictionary<string, List<JsonPhoneModel>> AllSvnkidIdToPhoneModelLookup { get; private set; } = new Dictionary<string, List<JsonPhoneModel>>();


	public Dictionary<string, List<ProductModelInfo>> AllInternalSwToPhoneModelLookup { get; private set; } = new Dictionary<string, List<ProductModelInfo>>();


	public Dictionary<MatrixType, MatrixInfo> MatrixTypeToMatrixInfoLookup { get; private set; }

	public UserAccount Account { get; private set; }

	public bool Parsed { get; private set; }

	public List<JsonMatrix> Matrixes { get; private set; } = new List<JsonMatrix>();


	public static JsonMatrixParser Instance
	{
		get
		{
			if (sThis == null)
			{
				sThis = new JsonMatrixParser();
			}
			return sThis;
		}
	}

	private static string TAG => typeof(JsonMatrixParser).FullName;

	public void ParseMatrices(UserAccount account)
	{
		lock (this)
		{
			Matrixes.Clear();
			mAllMatrixes.Clear();
			Clear(FileNameToDownloadInfoLookup);
			DetectionKeyToJsonConfigLookup.Clear();
			AllDetectionKeyToJsonConfigLookup.Clear();
			ModelIdToPhoneModelookup.Clear();
			CarrierToModelsLookup.Clear();
			ModelToCarriersLookup.Clear();
			AllSvnkidIdToPhoneModelLookup.Clear();
			AllInternalSwToPhoneModelLookup.Clear();
			AllModelCarrierToFlexModelLookup.Clear();
			AllModelIdToPhoneModelLookup.Clear();
			Account = account;
			Parsed = false;
			foreach (MatrixType key in MatrixTypeToMatrixInfoLookup.Keys)
			{
				string[] files = Directory.GetFiles(MatrixTypeToMatrixInfoLookup[key].DirectoryPath, "*.json.msu");
				foreach (string text in files)
				{
					try
					{
						string text2 = DecryptMatrix(text);
						JsonMatrix item = new JsonMatrix(Smart.Json.LoadString<MatrixDef>(text2), key);
						if (key == MatrixType.LMST_ALLMATRICES)
						{
							mAllMatrixes.Add(item);
						}
						else
						{
							Matrixes.Add(item);
						}
					}
					catch (Exception ex)
					{
						Smart.Log.Error(TAG, string.Format("Exception while parsing matrix {0}. Error {1}", text, key, ex.Message));
					}
				}
			}
			SaleModelCarrierParser.Instance.SvnKitIDToPhoneModels = AllSvnkidIdToPhoneModelLookup;
			AllSvnkidIdToPhoneModelLookup = new Dictionary<string, List<JsonPhoneModel>>();
			MtmCountryParser.Instance.InternalSwToPhoneModels = AllInternalSwToPhoneModelLookup;
			AllInternalSwToPhoneModelLookup = new Dictionary<string, List<ProductModelInfo>>();
			Parsed = true;
		}
	}

	public static string[] GetFileNameAndVersionMap(string matrixDir, MatrixType matrixType)
	{
		string[] array = new string[2]
		{
			string.Empty,
			string.Empty
		};
		string[] files = Directory.GetFiles(matrixDir, "*.json.msu");
		for (int i = 0; i < files.Length; i++)
		{
			try
			{
				string fileName = Path.GetFileName(files[i]);
				string text = fileName.Substring(0, fileName.Length - ".json.msu".Length);
				string text2 = DecryptMatrix(files[i]);
				dynamic val = Smart.Json.Load(text2);
				if (val.matrixType != matrixType.ToString())
				{
					Smart.Log.Debug(TAG, $"Deleting rogue matrix, {files[i]}");
					File.SetAttributes(files[i], FileAttributes.Normal);
					File.Delete(files[i]);
					continue;
				}
				string text3 = val.internalName;
				string text4 = ((text.Length > text3.Length) ? text.Substring(text3.Length) : string.Empty);
				ref string reference = ref array[0];
				reference = reference + ((i > 0) ? ";" : string.Empty) + text3 + text4;
				string[] array2 = array;
				array2[1] = (string)(array2[1] + (((i > 0) ? ";" : string.Empty) + val.version));
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, $"Exception while parsing matrix {files[i]}. Error {ex.Message}");
			}
		}
		return array;
	}

	public static string DecryptMatrix(string matrixName)
	{
		string result = string.Empty;
		byte[] array = new byte[32];
		byte[] array2 = new byte[32];
		FileStream fileStream = File.Open(matrixName, FileMode.Open, FileAccess.Read);
		int num = (int)fileStream.Length - 64;
		byte[] array3 = new byte[num];
		fileStream.Read(array, 0, 32);
		fileStream.Read(array2, 0, 32);
		string @string = Encoding.UTF8.GetString(array, 0, array.Length);
		if (Encoding.UTF8.GetString(array2, 0, array2.Length).Equals(Utilities.GetMD5Hash(Configurations.FakeMacId), StringComparison.OrdinalIgnoreCase))
		{
			fileStream.Read(array3, 0, num);
			for (int i = 0; i < num; i++)
			{
				array3[i] = (byte)(~array3[i]);
			}
			if (@string.Equals(Utilities.GetMD5Hash(array3), StringComparison.OrdinalIgnoreCase))
			{
				result = Encoding.UTF8.GetString(array3, 0, array3.Length);
			}
		}
		fileStream.Close();
		return result;
	}

	public static void RemoveObsoleteMatrix(string supportedCarriers, string matrixDir)
	{
		string[] files = Directory.GetFiles(matrixDir, "*.*");
		if (files.Length == 0)
		{
			return;
		}
		string[] array = supportedCarriers.Split(new char[1] { ';' });
		List<string> list = new List<string>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Length > 0)
			{
				string item = array[i].Replace(':', '_') + ".json.msu";
				list.Add(item);
			}
		}
		for (int j = 0; j < files.Length; j++)
		{
			string item = Path.GetFileName(files[j]);
			if (!list.Contains(item))
			{
				File.SetAttributes(files[j], FileAttributes.Normal);
				File.Delete(files[j]);
			}
		}
	}

	public static void EncryptMatrix(string content, string outFilename)
	{
		if (File.Exists(outFilename))
		{
			File.SetAttributes(outFilename, FileAttributes.Normal);
			File.Delete(outFilename);
		}
		byte[] bytes = Encoding.UTF8.GetBytes(content);
		using (FileStream fileStream = File.Open(outFilename, FileMode.Create, FileAccess.Write))
		{
			string mD5Hash = Utilities.GetMD5Hash(bytes);
			fileStream.Write(Encoding.UTF8.GetBytes(mD5Hash), 0, 32);
			mD5Hash = Utilities.GetMD5Hash(Configurations.FakeMacId);
			fileStream.Write(Encoding.UTF8.GetBytes(mD5Hash), 0, 32);
			for (int i = 0; i < bytes.Length; i++)
			{
				bytes[i] = (byte)(~bytes[i]);
			}
			fileStream.Write(bytes, 0, bytes.Length);
		}
		File.SetAttributes(outFilename, FileAttributes.ReadOnly);
	}

	public void ParseAllMatrices()
	{
		lock (this)
		{
			mAllMatrixes.Clear();
			AllDetectionKeyToJsonConfigLookup.Clear();
			AllSvnkidIdToPhoneModelLookup.Clear();
			AllInternalSwToPhoneModelLookup.Clear();
			AllModelCarrierToFlexModelLookup.Clear();
			AllModelIdToPhoneModelLookup.Clear();
			string[] files = Directory.GetFiles(Configurations.AllMatrixPath, "*.json.msu");
			foreach (string text in files)
			{
				try
				{
					string text2 = DecryptMatrix(text);
					JsonMatrix item = new JsonMatrix(Smart.Json.LoadString<MatrixDef>(text2), MatrixType.LMST_ALLMATRICES);
					mAllMatrixes.Add(item);
				}
				catch (Exception ex)
				{
					Smart.Log.Error(TAG, $"Exception while parsing matrix {text}. Error {ex.Message}");
				}
			}
			SaleModelCarrierParser.Instance.SvnKitIDToPhoneModels = AllSvnkidIdToPhoneModelLookup;
			AllSvnkidIdToPhoneModelLookup = new Dictionary<string, List<JsonPhoneModel>>();
			MtmCountryParser.Instance.InternalSwToPhoneModels = AllInternalSwToPhoneModelLookup;
			AllInternalSwToPhoneModelLookup = new Dictionary<string, List<ProductModelInfo>>();
		}
	}

	public void ParseUpgradeMatrices()
	{
		Matrixes.Clear();
		Clear(FileNameToDownloadInfoLookup);
		DetectionKeyToJsonConfigLookup.Clear();
		ModelIdToPhoneModelookup.Clear();
		CarrierToModelsLookup.Clear();
		ModelToCarriersLookup.Clear();
		string[] files = Directory.GetFiles(Configurations.MatrixPath, "*.json.msu");
		foreach (string text in files)
		{
			try
			{
				string text2 = DecryptMatrix(text);
				JsonMatrix item = new JsonMatrix(Smart.Json.LoadString<MatrixDef>(text2), MatrixType.LMST_UPGRADE);
				Matrixes.Add(item);
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, string.Format("Exception while parsing matrix {0}. Error {1}", text, MatrixType.LMST_UPGRADE.ToString(), ex.Message));
			}
		}
	}

	public void Clear(Dictionary<string, DownloadInfo> fileNameToDownloadInfoLookup, bool clearReserved = false)
	{
		foreach (string item in new List<string>(fileNameToDownloadInfoLookup.Keys))
		{
			if (fileNameToDownloadInfoLookup[item].Reserved == clearReserved)
			{
				fileNameToDownloadInfoLookup.Remove(item);
			}
		}
	}

	public List<string> ParseUseCasesInAllMatrixBySKU(string SKU)
	{
		List<JsonMatrix> list = new List<JsonMatrix>(mAllMatrixes);
		List<string> list2 = new List<string>();
		foreach (JsonMatrix item in list)
		{
			foreach (JsonPhoneModel phoneModel in item.PhoneModels)
			{
				string text = phoneModel.Name.Split(new char[1] { '/' })[0].Trim().ToUpper();
				if (!text.Contains(SKU.ToUpper()))
				{
					continue;
				}
				Smart.Log.Debug(TAG, $"Model fuzzily matched: matrix model name = {phoneModel.Name},given SKU = {SKU}");
				if (!(text != "IGNORE") || !(text != string.Empty))
				{
					continue;
				}
				foreach (JsonRecipe recipe in phoneModel.RecipeGroup.Recipes)
				{
					if (!list2.Contains(recipe.Usecase))
					{
						list2.Add(recipe.Usecase);
					}
					Smart.Log.Debug(TAG, $"Usecase {recipe.Usecase} was found and added by 'modelID = {text} , carrier = {item.Carrier}'");
				}
			}
		}
		return list2;
	}

	public List<string> ParseStepsInAllMatrixBySKU(string SKU)
	{
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		List<JsonMatrix> list = new List<JsonMatrix>(mAllMatrixes);
		List<string> list2 = new List<string>();
		string[] array = new string[5] { "Command", "Activity", "OpCode", "Data", "ResultCommand" };
		string empty = string.Empty;
		IRecipeInfo val = Smart.NewRecipeInfo();
		FileDownloader fileDownloader = new FileDownloader(Account.SrpOperation);
		try
		{
			foreach (JsonMatrix item in list)
			{
				foreach (JsonPhoneModel phoneModel in item.PhoneModels)
				{
					string text = phoneModel.Name.Split(new char[1] { '/' })[0];
					if (!text.Contains(SKU.ToUpper()))
					{
						continue;
					}
					Smart.Log.Debug(TAG, $"Model fuzzily matched: matrix model name = {phoneModel.Name},given SKU = {SKU}");
					if (!(text != "IGNORE") || !(text != string.Empty))
					{
						continue;
					}
					foreach (JsonRecipe recipe in phoneModel.RecipeGroup.Recipes)
					{
						foreach (JsonStep step in recipe.Steps)
						{
							if (!(step.Type == "STFFILE"))
							{
								continue;
							}
							string fileName = Path.GetFileName(new Uri(step.FileURL).LocalPath);
							string text2 = Path.Combine(Configurations.FileTypeInfos["STFFILE"].ParentDir, fileName);
							if (!File.Exists(text2) && fileDownloader.DownloadFile(step.FileURL, Configurations.FileTypeInfos["STFFILE"].ParentDir, out var localFileName) > 0)
							{
								string content = File.ReadAllText(localFileName);
								Utilities.AesEncryptFile(text2, content, FileAttributes.ReadOnly, "STFFILE");
							}
							val.Load(Smart.Rsd.ReadRecipeContent(text2), (UseCase)Enum.Parse(typeof(UseCase), recipe.Usecase), text2);
							foreach (IStepInfo step2 in val.Steps)
							{
								empty = step2.Name;
								if (empty != null && empty != string.Empty && !list2.Contains(empty))
								{
									list2.Add(empty);
								}
								Smart.Log.Verbose(TAG, $"Stepname = {empty} was loaded from file path = {text2} by ' modelID = {text} & usecase = {recipe.Usecase} & carrier = {item.Carrier} '");
								empty = step2.Step;
								if (empty != null && empty != string.Empty && !list2.Contains(empty))
								{
									list2.Add(empty);
								}
								Smart.Log.Verbose(TAG, $"StepMethod = {empty} was loaded from file path = {text2} by ' modelID = {text} & usecase = {recipe.Usecase} & carrier = {item.Carrier} '");
								foreach (dynamic ArgPara in (dynamic)step2.Args)
								{
									if (Array.Exists(array, (string s) => s == ArgPara.Name.ToString()))
									{
										empty = ArgPara.Value.ToString();
										if (empty != null && empty != string.Empty && !list2.Contains(empty))
										{
											list2.Add(empty);
										}
										Smart.Log.Verbose(TAG, $"Stepvalue = {empty} was loaded from file path = {text2} by ' modelID = {text} & usecase = {recipe.Usecase} & carrier = {item.Carrier} '");
									}
								}
							}
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Debug(TAG, ex.Message + ex.StackTrace);
			list2.Clear();
		}
		return list2;
	}

	private JsonMatrixParser()
	{
		MatrixTypeToMatrixInfoLookup = new Dictionary<MatrixType, MatrixInfo>
		{
			{
				MatrixType.LMST_UPGRADE,
				new MatrixInfo(fullyParsed: true, Configurations.MatrixPath, DetectionKeyToJsonConfigLookup)
			},
			{
				MatrixType.LMST_ALLMATRICES,
				new MatrixInfo(fullyParsed: false, Configurations.AllMatrixPath, AllDetectionKeyToJsonConfigLookup)
			}
		};
	}
}
