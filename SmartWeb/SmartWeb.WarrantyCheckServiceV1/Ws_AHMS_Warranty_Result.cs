using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace SmartWeb.WarrantyCheckServiceV1;

[Serializable]
[DebuggerStepThrough]
[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
[DataContract(Name = "Ws_AHMS_Warranty_Result", Namespace = "java:ibase.lenovo.com.services")]
public class Ws_AHMS_Warranty_Result : IExtensibleDataObject, INotifyPropertyChanged
{
	[Serializable]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[CollectionDataContract(Name = "Ws_AHMS_Warranty_Result.msg_varsType", Namespace = "java:ibase.lenovo.com.services", ItemName = "string")]
	public class msg_varsType : List<string>
	{
	}

	[NonSerialized]
	private ExtensionDataObject extensionDataField;

	[OptionalField]
	private string actionField;

	[OptionalField]
	private string apc_codeField;

	[OptionalField]
	private string assign_dateField;

	[OptionalField]
	private string bat_idField;

	[OptionalField]
	private string bt_chipField;

	[OptionalField]
	private string bt_idField;

	[OptionalField]
	private string btchip_manufacturerField;

	[OptionalField]
	private string carton_idField;

	[OptionalField]
	private string citField;

	[OptionalField]
	private string curr_ext_warr_exp_dateField;

	[OptionalField]
	private string curr_std_warr_exp_dateField;

	[OptionalField]
	private string curr_warr_type_codeField;

	[OptionalField]
	private string customer_model_noField;

	[OptionalField]
	private string customer_serial_noField;

	[OptionalField]
	private string direct_ship_customer_idField;

	[OptionalField]
	private string direct_ship_customer_nameField;

	[OptionalField]
	private string direct_ship_region_codeField;

	[OptionalField]
	private string direct_ship_ship_to_idField;

	[OptionalField]
	private string dnField;

	[OptionalField]
	private string dual_mode_serial_no_typeField;

	[OptionalField]
	private string dual_mode_serial_noField;

	[OptionalField]
	private string eraField;

	[OptionalField]
	private string factory_codeField;

	[OptionalField]
	private string flex_optionField;

	[OptionalField]
	private string flex_swField;

	[OptionalField]
	private Ws_FSB_Data_Result fsb_data_resultField;

	[OptionalField]
	private string fsbField;

	[OptionalField]
	private string guidField;

	[OptionalField]
	private string handset_typeField;

	[OptionalField]
	private string hardWare_versionField;

	[OptionalField]
	private string hsnField;

	[OptionalField]
	private string iccidField;

	[OptionalField]
	private string input_serial_no_typeField;

	[OptionalField]
	private string input_serial_noField;

	[OptionalField]
	private string is_rmaField;

	[OptionalField]
	private string item_codeField;

	[OptionalField]
	private string last_repair_dateField;

	[OptionalField]
	private string local_customer_idField;

	[OptionalField]
	private string local_customer_nameField;

	[OptionalField]
	private string local_market_nameField;

	[OptionalField]
	private string local_shop_idField;

	[OptionalField]
	private string local_shop_nameField;

	[OptionalField]
	private string local_warehouse_delivery_dateField;

	[OptionalField]
	private string manufacture_dateField;

	[OptionalField]
	private string market_nameField;

	[OptionalField]
	private string mfg_dateField;

	[OptionalField]
	private string mfg_factory_codeField;

	[OptionalField]
	private string mfg_line_noField;

	[OptionalField]
	private string mfg_region_idField;

	[OptionalField]
	private string mfg_system_idField;

	[OptionalField]
	private string minField;

	[OptionalField]
	private string mkt_modelField;

	[OptionalField]
	private string mru_countField;

	[OptionalField]
	private string msg_codeField;

	[OptionalField]
	private msg_varsType msg_varsField;

	[OptionalField]
	private string msnField;

	[OptionalField]
	private string org_codeField;

	[OptionalField]
	private string org_ext_warr_exp_dateField;

	[OptionalField]
	private string org_std_warr_exp_dateField;

	[OptionalField]
	private string org_warr_type_codeField;

	[OptionalField]
	private string pallet_idField;

	[OptionalField]
	private string pdbidField;

	[OptionalField]
	private string po_numField;

	[OptionalField]
	private string pop_dateField;

	[OptionalField]
	private string pop_error_flagField;

	[OptionalField]
	private string product_typeField;

	[OptionalField]
	private string protocolField;

	[OptionalField]
	private string receive_dateField;

	[OptionalField]
	private string region_idField;

	[OptionalField]
	private string renewal_ext_warr_exp_dateField;

	[OptionalField]
	private string renewal_std_warr_exp_dateField;

	[OptionalField]
	private string renewal_warr_type_codeField;

	[OptionalField]
	private string return_dateField;

	[OptionalField]
	private string rma_dateField;

	[OptionalField]
	private string rmaField;

	[OptionalField]
	private string roll_idField;

	[OptionalField]
	private string rsnField;

	[OptionalField]
	private string sale_dateField;

	[OptionalField]
	private string scrap_dateField;

	[OptionalField]
	private string serial_no_return_statusField;

	[OptionalField]
	private string serial_no_typeField;

	[OptionalField]
	private string ship_dateField;

	[OptionalField]
	private string ship_to_cityField;

	[OptionalField]
	private string ship_to_country_codeField;

	[OptionalField]
	private string ship_to_cust_addr_idField;

	[OptionalField]
	private string ship_to_cust_address_countryField;

	[OptionalField]
	private string ship_to_cust_idField;

	[OptionalField]
	private string ship_to_cust_nameField;

	[OptionalField]
	private string shipment_numberField;

	[OptionalField]
	private string software_versionField;

	[OptionalField]
	private string sold_to_cust_country_nameField;

	[OptionalField]
	private string sold_to_cust_idField;

	[OptionalField]
	private string sold_to_cust_nameField;

	[OptionalField]
	private string source_serial_noField;

	[OptionalField]
	private string status_codeField;

	[OptionalField]
	private string status_idField;

	[OptionalField]
	private string status_nameField;

	[OptionalField]
	private string stored_serial_noField;

	[OptionalField]
	private string sw_upg_elg_flagField;

	[OptionalField]
	private string sw_upg_warr_exp_dateField;

	[OptionalField]
	private string swap_reference_numberField;

	[OptionalField]
	private string system_idField;

	[OptionalField]
	private string ta_numberField;

	[OptionalField]
	private string tech_no_typeField;

	[OptionalField]
	private string transaction_typeField;

	[OptionalField]
	private string transceiver_model_noField;

	[OptionalField]
	private string tri_mode_serial_no_typeField;

	[OptionalField]
	private string tri_mode_serial_noField;

	[OptionalField]
	private string ulmaField;

	[OptionalField]
	private string upd_date_processedField;

	[OptionalField]
	private string updbyField;

	[OptionalField]
	private string updtimeField;

	[OptionalField]
	private string upload_dateField;

	[OptionalField]
	private string warehouse_idField;

	[OptionalField]
	private string warehouse_nameField;

	[OptionalField]
	private string warr_can_code_effective_dateField;

	[OptionalField]
	private string warr_cancel_codeField;

	[OptionalField]
	private string warr_country_codeField;

	[OptionalField]
	private string warr_flagField;

	[OptionalField]
	private string warr_regionField;

	[OptionalField]
	private string warranty_cancel_descriptionField;

	[OptionalField]
	private string warranty_date_processedField;

	[OptionalField]
	private string warranty_expiration_dateField;

	[OptionalField]
	private string wip_djField;

	[OptionalField]
	private Ws_Sublock_Result sublock_code_resultField;

	[OptionalField]
	private Ws_DevUnlock_Result device_unlock_code_resultField;

	[OptionalField]
	private Ws_DataBlock_Result data_block_sign_resultField;

	[OptionalField]
	private Ws_Lockcode_Result lock_code_resultField;

	[OptionalField]
	private Ws_R12_PCBA_Result r12_pcba_resultField;

	[Browsable(false)]
	public ExtensionDataObject ExtensionData
	{
		get
		{
			return extensionDataField;
		}
		set
		{
			extensionDataField = value;
		}
	}

	[DataMember(EmitDefaultValue = false)]
	public string action
	{
		get
		{
			return actionField;
		}
		set
		{
			if ((object)actionField != value)
			{
				actionField = value;
				RaisePropertyChanged("action");
			}
		}
	}

	[DataMember(EmitDefaultValue = false)]
	public string apc_code
	{
		get
		{
			return apc_codeField;
		}
		set
		{
			if ((object)apc_codeField != value)
			{
				apc_codeField = value;
				RaisePropertyChanged("apc_code");
			}
		}
	}

	[DataMember(EmitDefaultValue = false)]
	public string assign_date
	{
		get
		{
			return assign_dateField;
		}
		set
		{
			if ((object)assign_dateField != value)
			{
				assign_dateField = value;
				RaisePropertyChanged("assign_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false)]
	public string bat_id
	{
		get
		{
			return bat_idField;
		}
		set
		{
			if ((object)bat_idField != value)
			{
				bat_idField = value;
				RaisePropertyChanged("bat_id");
			}
		}
	}

	[DataMember(EmitDefaultValue = false)]
	public string bt_chip
	{
		get
		{
			return bt_chipField;
		}
		set
		{
			if ((object)bt_chipField != value)
			{
				bt_chipField = value;
				RaisePropertyChanged("bt_chip");
			}
		}
	}

	[DataMember(EmitDefaultValue = false)]
	public string bt_id
	{
		get
		{
			return bt_idField;
		}
		set
		{
			if ((object)bt_idField != value)
			{
				bt_idField = value;
				RaisePropertyChanged("bt_id");
			}
		}
	}

	[DataMember(EmitDefaultValue = false)]
	public string btchip_manufacturer
	{
		get
		{
			return btchip_manufacturerField;
		}
		set
		{
			if ((object)btchip_manufacturerField != value)
			{
				btchip_manufacturerField = value;
				RaisePropertyChanged("btchip_manufacturer");
			}
		}
	}

	[DataMember(EmitDefaultValue = false)]
	public string carton_id
	{
		get
		{
			return carton_idField;
		}
		set
		{
			if ((object)carton_idField != value)
			{
				carton_idField = value;
				RaisePropertyChanged("carton_id");
			}
		}
	}

	[DataMember(EmitDefaultValue = false)]
	public string cit
	{
		get
		{
			return citField;
		}
		set
		{
			if ((object)citField != value)
			{
				citField = value;
				RaisePropertyChanged("cit");
			}
		}
	}

	[DataMember(EmitDefaultValue = false)]
	public string curr_ext_warr_exp_date
	{
		get
		{
			return curr_ext_warr_exp_dateField;
		}
		set
		{
			if ((object)curr_ext_warr_exp_dateField != value)
			{
				curr_ext_warr_exp_dateField = value;
				RaisePropertyChanged("curr_ext_warr_exp_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false)]
	public string curr_std_warr_exp_date
	{
		get
		{
			return curr_std_warr_exp_dateField;
		}
		set
		{
			if ((object)curr_std_warr_exp_dateField != value)
			{
				curr_std_warr_exp_dateField = value;
				RaisePropertyChanged("curr_std_warr_exp_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false)]
	public string curr_warr_type_code
	{
		get
		{
			return curr_warr_type_codeField;
		}
		set
		{
			if ((object)curr_warr_type_codeField != value)
			{
				curr_warr_type_codeField = value;
				RaisePropertyChanged("curr_warr_type_code");
			}
		}
	}

	[DataMember(EmitDefaultValue = false)]
	public string customer_model_no
	{
		get
		{
			return customer_model_noField;
		}
		set
		{
			if ((object)customer_model_noField != value)
			{
				customer_model_noField = value;
				RaisePropertyChanged("customer_model_no");
			}
		}
	}

	[DataMember(EmitDefaultValue = false)]
	public string customer_serial_no
	{
		get
		{
			return customer_serial_noField;
		}
		set
		{
			if ((object)customer_serial_noField != value)
			{
				customer_serial_noField = value;
				RaisePropertyChanged("customer_serial_no");
			}
		}
	}

	[DataMember(EmitDefaultValue = false)]
	public string direct_ship_customer_id
	{
		get
		{
			return direct_ship_customer_idField;
		}
		set
		{
			if ((object)direct_ship_customer_idField != value)
			{
				direct_ship_customer_idField = value;
				RaisePropertyChanged("direct_ship_customer_id");
			}
		}
	}

	[DataMember(EmitDefaultValue = false)]
	public string direct_ship_customer_name
	{
		get
		{
			return direct_ship_customer_nameField;
		}
		set
		{
			if ((object)direct_ship_customer_nameField != value)
			{
				direct_ship_customer_nameField = value;
				RaisePropertyChanged("direct_ship_customer_name");
			}
		}
	}

	[DataMember(EmitDefaultValue = false)]
	public string direct_ship_region_code
	{
		get
		{
			return direct_ship_region_codeField;
		}
		set
		{
			if ((object)direct_ship_region_codeField != value)
			{
				direct_ship_region_codeField = value;
				RaisePropertyChanged("direct_ship_region_code");
			}
		}
	}

	[DataMember(EmitDefaultValue = false)]
	public string direct_ship_ship_to_id
	{
		get
		{
			return direct_ship_ship_to_idField;
		}
		set
		{
			if ((object)direct_ship_ship_to_idField != value)
			{
				direct_ship_ship_to_idField = value;
				RaisePropertyChanged("direct_ship_ship_to_id");
			}
		}
	}

	[DataMember(EmitDefaultValue = false)]
	public string dn
	{
		get
		{
			return dnField;
		}
		set
		{
			if ((object)dnField != value)
			{
				dnField = value;
				RaisePropertyChanged("dn");
			}
		}
	}

	[DataMember(EmitDefaultValue = false)]
	public string dual_mode_serial_no_type
	{
		get
		{
			return dual_mode_serial_no_typeField;
		}
		set
		{
			if ((object)dual_mode_serial_no_typeField != value)
			{
				dual_mode_serial_no_typeField = value;
				RaisePropertyChanged("dual_mode_serial_no_type");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 20)]
	public string dual_mode_serial_no
	{
		get
		{
			return dual_mode_serial_noField;
		}
		set
		{
			if ((object)dual_mode_serial_noField != value)
			{
				dual_mode_serial_noField = value;
				RaisePropertyChanged("dual_mode_serial_no");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 21)]
	public string era
	{
		get
		{
			return eraField;
		}
		set
		{
			if ((object)eraField != value)
			{
				eraField = value;
				RaisePropertyChanged("era");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 22)]
	public string factory_code
	{
		get
		{
			return factory_codeField;
		}
		set
		{
			if ((object)factory_codeField != value)
			{
				factory_codeField = value;
				RaisePropertyChanged("factory_code");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 23)]
	public string flex_option
	{
		get
		{
			return flex_optionField;
		}
		set
		{
			if ((object)flex_optionField != value)
			{
				flex_optionField = value;
				RaisePropertyChanged("flex_option");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 24)]
	public string flex_sw
	{
		get
		{
			return flex_swField;
		}
		set
		{
			if ((object)flex_swField != value)
			{
				flex_swField = value;
				RaisePropertyChanged("flex_sw");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 25)]
	public Ws_FSB_Data_Result fsb_data_result
	{
		get
		{
			return fsb_data_resultField;
		}
		set
		{
			if (fsb_data_resultField != value)
			{
				fsb_data_resultField = value;
				RaisePropertyChanged("fsb_data_result");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 26)]
	public string fsb
	{
		get
		{
			return fsbField;
		}
		set
		{
			if ((object)fsbField != value)
			{
				fsbField = value;
				RaisePropertyChanged("fsb");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 27)]
	public string guid
	{
		get
		{
			return guidField;
		}
		set
		{
			if ((object)guidField != value)
			{
				guidField = value;
				RaisePropertyChanged("guid");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 28)]
	public string handset_type
	{
		get
		{
			return handset_typeField;
		}
		set
		{
			if ((object)handset_typeField != value)
			{
				handset_typeField = value;
				RaisePropertyChanged("handset_type");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 29)]
	public string hardWare_version
	{
		get
		{
			return hardWare_versionField;
		}
		set
		{
			if ((object)hardWare_versionField != value)
			{
				hardWare_versionField = value;
				RaisePropertyChanged("hardWare_version");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 30)]
	public string hsn
	{
		get
		{
			return hsnField;
		}
		set
		{
			if ((object)hsnField != value)
			{
				hsnField = value;
				RaisePropertyChanged("hsn");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 31)]
	public string iccid
	{
		get
		{
			return iccidField;
		}
		set
		{
			if ((object)iccidField != value)
			{
				iccidField = value;
				RaisePropertyChanged("iccid");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 32)]
	public string input_serial_no_type
	{
		get
		{
			return input_serial_no_typeField;
		}
		set
		{
			if ((object)input_serial_no_typeField != value)
			{
				input_serial_no_typeField = value;
				RaisePropertyChanged("input_serial_no_type");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 33)]
	public string input_serial_no
	{
		get
		{
			return input_serial_noField;
		}
		set
		{
			if ((object)input_serial_noField != value)
			{
				input_serial_noField = value;
				RaisePropertyChanged("input_serial_no");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 34)]
	public string is_rma
	{
		get
		{
			return is_rmaField;
		}
		set
		{
			if ((object)is_rmaField != value)
			{
				is_rmaField = value;
				RaisePropertyChanged("is_rma");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 35)]
	public string item_code
	{
		get
		{
			return item_codeField;
		}
		set
		{
			if ((object)item_codeField != value)
			{
				item_codeField = value;
				RaisePropertyChanged("item_code");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 36)]
	public string last_repair_date
	{
		get
		{
			return last_repair_dateField;
		}
		set
		{
			if ((object)last_repair_dateField != value)
			{
				last_repair_dateField = value;
				RaisePropertyChanged("last_repair_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 37)]
	public string local_customer_id
	{
		get
		{
			return local_customer_idField;
		}
		set
		{
			if ((object)local_customer_idField != value)
			{
				local_customer_idField = value;
				RaisePropertyChanged("local_customer_id");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 38)]
	public string local_customer_name
	{
		get
		{
			return local_customer_nameField;
		}
		set
		{
			if ((object)local_customer_nameField != value)
			{
				local_customer_nameField = value;
				RaisePropertyChanged("local_customer_name");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 39)]
	public string local_market_name
	{
		get
		{
			return local_market_nameField;
		}
		set
		{
			if ((object)local_market_nameField != value)
			{
				local_market_nameField = value;
				RaisePropertyChanged("local_market_name");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 40)]
	public string local_shop_id
	{
		get
		{
			return local_shop_idField;
		}
		set
		{
			if ((object)local_shop_idField != value)
			{
				local_shop_idField = value;
				RaisePropertyChanged("local_shop_id");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 41)]
	public string local_shop_name
	{
		get
		{
			return local_shop_nameField;
		}
		set
		{
			if ((object)local_shop_nameField != value)
			{
				local_shop_nameField = value;
				RaisePropertyChanged("local_shop_name");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 42)]
	public string local_warehouse_delivery_date
	{
		get
		{
			return local_warehouse_delivery_dateField;
		}
		set
		{
			if ((object)local_warehouse_delivery_dateField != value)
			{
				local_warehouse_delivery_dateField = value;
				RaisePropertyChanged("local_warehouse_delivery_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 43)]
	public string manufacture_date
	{
		get
		{
			return manufacture_dateField;
		}
		set
		{
			if ((object)manufacture_dateField != value)
			{
				manufacture_dateField = value;
				RaisePropertyChanged("manufacture_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 44)]
	public string market_name
	{
		get
		{
			return market_nameField;
		}
		set
		{
			if ((object)market_nameField != value)
			{
				market_nameField = value;
				RaisePropertyChanged("market_name");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 45)]
	public string mfg_date
	{
		get
		{
			return mfg_dateField;
		}
		set
		{
			if ((object)mfg_dateField != value)
			{
				mfg_dateField = value;
				RaisePropertyChanged("mfg_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 46)]
	public string mfg_factory_code
	{
		get
		{
			return mfg_factory_codeField;
		}
		set
		{
			if ((object)mfg_factory_codeField != value)
			{
				mfg_factory_codeField = value;
				RaisePropertyChanged("mfg_factory_code");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 47)]
	public string mfg_line_no
	{
		get
		{
			return mfg_line_noField;
		}
		set
		{
			if ((object)mfg_line_noField != value)
			{
				mfg_line_noField = value;
				RaisePropertyChanged("mfg_line_no");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 48)]
	public string mfg_region_id
	{
		get
		{
			return mfg_region_idField;
		}
		set
		{
			if ((object)mfg_region_idField != value)
			{
				mfg_region_idField = value;
				RaisePropertyChanged("mfg_region_id");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 49)]
	public string mfg_system_id
	{
		get
		{
			return mfg_system_idField;
		}
		set
		{
			if ((object)mfg_system_idField != value)
			{
				mfg_system_idField = value;
				RaisePropertyChanged("mfg_system_id");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 50)]
	public string min
	{
		get
		{
			return minField;
		}
		set
		{
			if ((object)minField != value)
			{
				minField = value;
				RaisePropertyChanged("min");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 51)]
	public string mkt_model
	{
		get
		{
			return mkt_modelField;
		}
		set
		{
			if ((object)mkt_modelField != value)
			{
				mkt_modelField = value;
				RaisePropertyChanged("mkt_model");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 52)]
	public string mru_count
	{
		get
		{
			return mru_countField;
		}
		set
		{
			if ((object)mru_countField != value)
			{
				mru_countField = value;
				RaisePropertyChanged("mru_count");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 53)]
	public string msg_code
	{
		get
		{
			return msg_codeField;
		}
		set
		{
			if ((object)msg_codeField != value)
			{
				msg_codeField = value;
				RaisePropertyChanged("msg_code");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 54)]
	public msg_varsType msg_vars
	{
		get
		{
			return msg_varsField;
		}
		set
		{
			if (msg_varsField != value)
			{
				msg_varsField = value;
				RaisePropertyChanged("msg_vars");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 55)]
	public string msn
	{
		get
		{
			return msnField;
		}
		set
		{
			if ((object)msnField != value)
			{
				msnField = value;
				RaisePropertyChanged("msn");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 56)]
	public string org_code
	{
		get
		{
			return org_codeField;
		}
		set
		{
			if ((object)org_codeField != value)
			{
				org_codeField = value;
				RaisePropertyChanged("org_code");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 57)]
	public string org_ext_warr_exp_date
	{
		get
		{
			return org_ext_warr_exp_dateField;
		}
		set
		{
			if ((object)org_ext_warr_exp_dateField != value)
			{
				org_ext_warr_exp_dateField = value;
				RaisePropertyChanged("org_ext_warr_exp_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 58)]
	public string org_std_warr_exp_date
	{
		get
		{
			return org_std_warr_exp_dateField;
		}
		set
		{
			if ((object)org_std_warr_exp_dateField != value)
			{
				org_std_warr_exp_dateField = value;
				RaisePropertyChanged("org_std_warr_exp_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 59)]
	public string org_warr_type_code
	{
		get
		{
			return org_warr_type_codeField;
		}
		set
		{
			if ((object)org_warr_type_codeField != value)
			{
				org_warr_type_codeField = value;
				RaisePropertyChanged("org_warr_type_code");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 60)]
	public string pallet_id
	{
		get
		{
			return pallet_idField;
		}
		set
		{
			if ((object)pallet_idField != value)
			{
				pallet_idField = value;
				RaisePropertyChanged("pallet_id");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 61)]
	public string pdbid
	{
		get
		{
			return pdbidField;
		}
		set
		{
			if ((object)pdbidField != value)
			{
				pdbidField = value;
				RaisePropertyChanged("pdbid");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 62)]
	public string po_num
	{
		get
		{
			return po_numField;
		}
		set
		{
			if ((object)po_numField != value)
			{
				po_numField = value;
				RaisePropertyChanged("po_num");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 63)]
	public string pop_date
	{
		get
		{
			return pop_dateField;
		}
		set
		{
			if ((object)pop_dateField != value)
			{
				pop_dateField = value;
				RaisePropertyChanged("pop_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 64)]
	public string pop_error_flag
	{
		get
		{
			return pop_error_flagField;
		}
		set
		{
			if ((object)pop_error_flagField != value)
			{
				pop_error_flagField = value;
				RaisePropertyChanged("pop_error_flag");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 65)]
	public string product_type
	{
		get
		{
			return product_typeField;
		}
		set
		{
			if ((object)product_typeField != value)
			{
				product_typeField = value;
				RaisePropertyChanged("product_type");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 66)]
	public string protocol
	{
		get
		{
			return protocolField;
		}
		set
		{
			if ((object)protocolField != value)
			{
				protocolField = value;
				RaisePropertyChanged("protocol");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 67)]
	public string receive_date
	{
		get
		{
			return receive_dateField;
		}
		set
		{
			if ((object)receive_dateField != value)
			{
				receive_dateField = value;
				RaisePropertyChanged("receive_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 68)]
	public string region_id
	{
		get
		{
			return region_idField;
		}
		set
		{
			if ((object)region_idField != value)
			{
				region_idField = value;
				RaisePropertyChanged("region_id");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 69)]
	public string renewal_ext_warr_exp_date
	{
		get
		{
			return renewal_ext_warr_exp_dateField;
		}
		set
		{
			if ((object)renewal_ext_warr_exp_dateField != value)
			{
				renewal_ext_warr_exp_dateField = value;
				RaisePropertyChanged("renewal_ext_warr_exp_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 70)]
	public string renewal_std_warr_exp_date
	{
		get
		{
			return renewal_std_warr_exp_dateField;
		}
		set
		{
			if ((object)renewal_std_warr_exp_dateField != value)
			{
				renewal_std_warr_exp_dateField = value;
				RaisePropertyChanged("renewal_std_warr_exp_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 71)]
	public string renewal_warr_type_code
	{
		get
		{
			return renewal_warr_type_codeField;
		}
		set
		{
			if ((object)renewal_warr_type_codeField != value)
			{
				renewal_warr_type_codeField = value;
				RaisePropertyChanged("renewal_warr_type_code");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 72)]
	public string return_date
	{
		get
		{
			return return_dateField;
		}
		set
		{
			if ((object)return_dateField != value)
			{
				return_dateField = value;
				RaisePropertyChanged("return_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 73)]
	public string rma_date
	{
		get
		{
			return rma_dateField;
		}
		set
		{
			if ((object)rma_dateField != value)
			{
				rma_dateField = value;
				RaisePropertyChanged("rma_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 74)]
	public string rma
	{
		get
		{
			return rmaField;
		}
		set
		{
			if ((object)rmaField != value)
			{
				rmaField = value;
				RaisePropertyChanged("rma");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 75)]
	public string roll_id
	{
		get
		{
			return roll_idField;
		}
		set
		{
			if ((object)roll_idField != value)
			{
				roll_idField = value;
				RaisePropertyChanged("roll_id");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 76)]
	public string rsn
	{
		get
		{
			return rsnField;
		}
		set
		{
			if ((object)rsnField != value)
			{
				rsnField = value;
				RaisePropertyChanged("rsn");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 77)]
	public string sale_date
	{
		get
		{
			return sale_dateField;
		}
		set
		{
			if ((object)sale_dateField != value)
			{
				sale_dateField = value;
				RaisePropertyChanged("sale_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 78)]
	public string scrap_date
	{
		get
		{
			return scrap_dateField;
		}
		set
		{
			if ((object)scrap_dateField != value)
			{
				scrap_dateField = value;
				RaisePropertyChanged("scrap_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 79)]
	public string serial_no_return_status
	{
		get
		{
			return serial_no_return_statusField;
		}
		set
		{
			if ((object)serial_no_return_statusField != value)
			{
				serial_no_return_statusField = value;
				RaisePropertyChanged("serial_no_return_status");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 80)]
	public string serial_no_type
	{
		get
		{
			return serial_no_typeField;
		}
		set
		{
			if ((object)serial_no_typeField != value)
			{
				serial_no_typeField = value;
				RaisePropertyChanged("serial_no_type");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 81)]
	public string ship_date
	{
		get
		{
			return ship_dateField;
		}
		set
		{
			if ((object)ship_dateField != value)
			{
				ship_dateField = value;
				RaisePropertyChanged("ship_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 82)]
	public string ship_to_city
	{
		get
		{
			return ship_to_cityField;
		}
		set
		{
			if ((object)ship_to_cityField != value)
			{
				ship_to_cityField = value;
				RaisePropertyChanged("ship_to_city");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 83)]
	public string ship_to_country_code
	{
		get
		{
			return ship_to_country_codeField;
		}
		set
		{
			if ((object)ship_to_country_codeField != value)
			{
				ship_to_country_codeField = value;
				RaisePropertyChanged("ship_to_country_code");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 84)]
	public string ship_to_cust_addr_id
	{
		get
		{
			return ship_to_cust_addr_idField;
		}
		set
		{
			if ((object)ship_to_cust_addr_idField != value)
			{
				ship_to_cust_addr_idField = value;
				RaisePropertyChanged("ship_to_cust_addr_id");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 85)]
	public string ship_to_cust_address_country
	{
		get
		{
			return ship_to_cust_address_countryField;
		}
		set
		{
			if ((object)ship_to_cust_address_countryField != value)
			{
				ship_to_cust_address_countryField = value;
				RaisePropertyChanged("ship_to_cust_address_country");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 86)]
	public string ship_to_cust_id
	{
		get
		{
			return ship_to_cust_idField;
		}
		set
		{
			if ((object)ship_to_cust_idField != value)
			{
				ship_to_cust_idField = value;
				RaisePropertyChanged("ship_to_cust_id");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 87)]
	public string ship_to_cust_name
	{
		get
		{
			return ship_to_cust_nameField;
		}
		set
		{
			if ((object)ship_to_cust_nameField != value)
			{
				ship_to_cust_nameField = value;
				RaisePropertyChanged("ship_to_cust_name");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 88)]
	public string shipment_number
	{
		get
		{
			return shipment_numberField;
		}
		set
		{
			if ((object)shipment_numberField != value)
			{
				shipment_numberField = value;
				RaisePropertyChanged("shipment_number");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 89)]
	public string software_version
	{
		get
		{
			return software_versionField;
		}
		set
		{
			if ((object)software_versionField != value)
			{
				software_versionField = value;
				RaisePropertyChanged("software_version");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 90)]
	public string sold_to_cust_country_name
	{
		get
		{
			return sold_to_cust_country_nameField;
		}
		set
		{
			if ((object)sold_to_cust_country_nameField != value)
			{
				sold_to_cust_country_nameField = value;
				RaisePropertyChanged("sold_to_cust_country_name");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 91)]
	public string sold_to_cust_id
	{
		get
		{
			return sold_to_cust_idField;
		}
		set
		{
			if ((object)sold_to_cust_idField != value)
			{
				sold_to_cust_idField = value;
				RaisePropertyChanged("sold_to_cust_id");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 92)]
	public string sold_to_cust_name
	{
		get
		{
			return sold_to_cust_nameField;
		}
		set
		{
			if ((object)sold_to_cust_nameField != value)
			{
				sold_to_cust_nameField = value;
				RaisePropertyChanged("sold_to_cust_name");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 93)]
	public string source_serial_no
	{
		get
		{
			return source_serial_noField;
		}
		set
		{
			if ((object)source_serial_noField != value)
			{
				source_serial_noField = value;
				RaisePropertyChanged("source_serial_no");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 94)]
	public string status_code
	{
		get
		{
			return status_codeField;
		}
		set
		{
			if ((object)status_codeField != value)
			{
				status_codeField = value;
				RaisePropertyChanged("status_code");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 95)]
	public string status_id
	{
		get
		{
			return status_idField;
		}
		set
		{
			if ((object)status_idField != value)
			{
				status_idField = value;
				RaisePropertyChanged("status_id");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 96)]
	public string status_name
	{
		get
		{
			return status_nameField;
		}
		set
		{
			if ((object)status_nameField != value)
			{
				status_nameField = value;
				RaisePropertyChanged("status_name");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 97)]
	public string stored_serial_no
	{
		get
		{
			return stored_serial_noField;
		}
		set
		{
			if ((object)stored_serial_noField != value)
			{
				stored_serial_noField = value;
				RaisePropertyChanged("stored_serial_no");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 98)]
	public string sw_upg_elg_flag
	{
		get
		{
			return sw_upg_elg_flagField;
		}
		set
		{
			if ((object)sw_upg_elg_flagField != value)
			{
				sw_upg_elg_flagField = value;
				RaisePropertyChanged("sw_upg_elg_flag");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 99)]
	public string sw_upg_warr_exp_date
	{
		get
		{
			return sw_upg_warr_exp_dateField;
		}
		set
		{
			if ((object)sw_upg_warr_exp_dateField != value)
			{
				sw_upg_warr_exp_dateField = value;
				RaisePropertyChanged("sw_upg_warr_exp_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 100)]
	public string swap_reference_number
	{
		get
		{
			return swap_reference_numberField;
		}
		set
		{
			if ((object)swap_reference_numberField != value)
			{
				swap_reference_numberField = value;
				RaisePropertyChanged("swap_reference_number");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 101)]
	public string system_id
	{
		get
		{
			return system_idField;
		}
		set
		{
			if ((object)system_idField != value)
			{
				system_idField = value;
				RaisePropertyChanged("system_id");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 102)]
	public string ta_number
	{
		get
		{
			return ta_numberField;
		}
		set
		{
			if ((object)ta_numberField != value)
			{
				ta_numberField = value;
				RaisePropertyChanged("ta_number");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 103)]
	public string tech_no_type
	{
		get
		{
			return tech_no_typeField;
		}
		set
		{
			if ((object)tech_no_typeField != value)
			{
				tech_no_typeField = value;
				RaisePropertyChanged("tech_no_type");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 104)]
	public string transaction_type
	{
		get
		{
			return transaction_typeField;
		}
		set
		{
			if ((object)transaction_typeField != value)
			{
				transaction_typeField = value;
				RaisePropertyChanged("transaction_type");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 105)]
	public string transceiver_model_no
	{
		get
		{
			return transceiver_model_noField;
		}
		set
		{
			if ((object)transceiver_model_noField != value)
			{
				transceiver_model_noField = value;
				RaisePropertyChanged("transceiver_model_no");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 106)]
	public string tri_mode_serial_no_type
	{
		get
		{
			return tri_mode_serial_no_typeField;
		}
		set
		{
			if ((object)tri_mode_serial_no_typeField != value)
			{
				tri_mode_serial_no_typeField = value;
				RaisePropertyChanged("tri_mode_serial_no_type");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 107)]
	public string tri_mode_serial_no
	{
		get
		{
			return tri_mode_serial_noField;
		}
		set
		{
			if ((object)tri_mode_serial_noField != value)
			{
				tri_mode_serial_noField = value;
				RaisePropertyChanged("tri_mode_serial_no");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 108)]
	public string ulma
	{
		get
		{
			return ulmaField;
		}
		set
		{
			if ((object)ulmaField != value)
			{
				ulmaField = value;
				RaisePropertyChanged("ulma");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 109)]
	public string upd_date_processed
	{
		get
		{
			return upd_date_processedField;
		}
		set
		{
			if ((object)upd_date_processedField != value)
			{
				upd_date_processedField = value;
				RaisePropertyChanged("upd_date_processed");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 110)]
	public string updby
	{
		get
		{
			return updbyField;
		}
		set
		{
			if ((object)updbyField != value)
			{
				updbyField = value;
				RaisePropertyChanged("updby");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 111)]
	public string updtime
	{
		get
		{
			return updtimeField;
		}
		set
		{
			if ((object)updtimeField != value)
			{
				updtimeField = value;
				RaisePropertyChanged("updtime");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 112)]
	public string upload_date
	{
		get
		{
			return upload_dateField;
		}
		set
		{
			if ((object)upload_dateField != value)
			{
				upload_dateField = value;
				RaisePropertyChanged("upload_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 113)]
	public string warehouse_id
	{
		get
		{
			return warehouse_idField;
		}
		set
		{
			if ((object)warehouse_idField != value)
			{
				warehouse_idField = value;
				RaisePropertyChanged("warehouse_id");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 114)]
	public string warehouse_name
	{
		get
		{
			return warehouse_nameField;
		}
		set
		{
			if ((object)warehouse_nameField != value)
			{
				warehouse_nameField = value;
				RaisePropertyChanged("warehouse_name");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 115)]
	public string warr_can_code_effective_date
	{
		get
		{
			return warr_can_code_effective_dateField;
		}
		set
		{
			if ((object)warr_can_code_effective_dateField != value)
			{
				warr_can_code_effective_dateField = value;
				RaisePropertyChanged("warr_can_code_effective_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 116)]
	public string warr_cancel_code
	{
		get
		{
			return warr_cancel_codeField;
		}
		set
		{
			if ((object)warr_cancel_codeField != value)
			{
				warr_cancel_codeField = value;
				RaisePropertyChanged("warr_cancel_code");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 117)]
	public string warr_country_code
	{
		get
		{
			return warr_country_codeField;
		}
		set
		{
			if ((object)warr_country_codeField != value)
			{
				warr_country_codeField = value;
				RaisePropertyChanged("warr_country_code");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 118)]
	public string warr_flag
	{
		get
		{
			return warr_flagField;
		}
		set
		{
			if ((object)warr_flagField != value)
			{
				warr_flagField = value;
				RaisePropertyChanged("warr_flag");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 119)]
	public string warr_region
	{
		get
		{
			return warr_regionField;
		}
		set
		{
			if ((object)warr_regionField != value)
			{
				warr_regionField = value;
				RaisePropertyChanged("warr_region");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 120)]
	public string warranty_cancel_description
	{
		get
		{
			return warranty_cancel_descriptionField;
		}
		set
		{
			if ((object)warranty_cancel_descriptionField != value)
			{
				warranty_cancel_descriptionField = value;
				RaisePropertyChanged("warranty_cancel_description");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 121)]
	public string warranty_date_processed
	{
		get
		{
			return warranty_date_processedField;
		}
		set
		{
			if ((object)warranty_date_processedField != value)
			{
				warranty_date_processedField = value;
				RaisePropertyChanged("warranty_date_processed");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 122)]
	public string warranty_expiration_date
	{
		get
		{
			return warranty_expiration_dateField;
		}
		set
		{
			if ((object)warranty_expiration_dateField != value)
			{
				warranty_expiration_dateField = value;
				RaisePropertyChanged("warranty_expiration_date");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 123)]
	public string wip_dj
	{
		get
		{
			return wip_djField;
		}
		set
		{
			if ((object)wip_djField != value)
			{
				wip_djField = value;
				RaisePropertyChanged("wip_dj");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 124)]
	public Ws_Sublock_Result sublock_code_result
	{
		get
		{
			return sublock_code_resultField;
		}
		set
		{
			if (sublock_code_resultField != value)
			{
				sublock_code_resultField = value;
				RaisePropertyChanged("sublock_code_result");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 125)]
	public Ws_DevUnlock_Result device_unlock_code_result
	{
		get
		{
			return device_unlock_code_resultField;
		}
		set
		{
			if (device_unlock_code_resultField != value)
			{
				device_unlock_code_resultField = value;
				RaisePropertyChanged("device_unlock_code_result");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 126)]
	public Ws_DataBlock_Result data_block_sign_result
	{
		get
		{
			return data_block_sign_resultField;
		}
		set
		{
			if (data_block_sign_resultField != value)
			{
				data_block_sign_resultField = value;
				RaisePropertyChanged("data_block_sign_result");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 127)]
	public Ws_Lockcode_Result lock_code_result
	{
		get
		{
			return lock_code_resultField;
		}
		set
		{
			if (lock_code_resultField != value)
			{
				lock_code_resultField = value;
				RaisePropertyChanged("lock_code_result");
			}
		}
	}

	[DataMember(EmitDefaultValue = false, Order = 128)]
	public Ws_R12_PCBA_Result r12_pcba_result
	{
		get
		{
			return r12_pcba_resultField;
		}
		set
		{
			if (r12_pcba_resultField != value)
			{
				r12_pcba_resultField = value;
				RaisePropertyChanged("r12_pcba_result");
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
