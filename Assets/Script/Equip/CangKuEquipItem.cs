using UnityEngine;
using System.Collections;
using STCCMD;
using CMNCMD;
public class CangKuEquipItem : PagingStorageSlot {
	int mItemID = -1;
	bool hasChanged = false;
//	public int excel_id = -1;
	public ulong equip_id = 0;
	public UISprite icon;
	public UISprite kuang;
	public UILabel 	name;
	public UILabel 	embedinfo;
	//当前的装备信息 
	public EquipInfo equip_info;
	string geminfo_str;
	override public int gid
	{
		get {	
			return mItemID;
		}
		
		set {
			
			if (mItemID != value)
			{
				mItemID = value;
				hasChanged = true;
			}
		}
	}
	
	override public void ResetItem()
	{
		hasChanged = true;
	}
	void Awake()
	{
		geminfo_str = embedinfo.text;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	void LateUpdate()
	{
		if(hasChanged)
		{
			hasChanged = false;
			InitialData();
			
		}
	}
	void InitialData()
	{
		equip_id = 0;
		if(gid < EquipManager.equip_id_list.Count && gid>=0)
		{
			equip_id = EquipManager.equip_id_list[gid];
			gameObject.SetActiveRecursively(true);
			gameObject.GetComponent<Collider>().enabled = true;
			ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[equip_id];
			equip_info = (EquipInfo)CommonMB.EquipInfo_Map[(uint)item_info.nExcelID2];
			icon.spriteName = equip_info.IconName;
			name.text = equip_info.EquipName;
			embedinfo.text = geminfo_str +GemEmbedGemNum(equip_id).ToString()+"/"+"4";
			kuang.spriteName = "xuanzhong_kuang";
		}
		else 
		{
			gameObject.GetComponent<Collider>().enabled = false;
			icon.spriteName = "transparent";
			icon.gameObject.SetActiveRecursively(false); 
			kuang.gameObject.SetActiveRecursively(false); 
			name.text = ""; 
			embedinfo.text = "";
		}
		
	}
	int GemEmbedGemNum(ulong equip_id)
	{
		int num =0;
		if(EquipManager.EquipToGemMap.Contains(equip_id))
		{
			EquipManager.EquipGemUnit unit =  (EquipManager.EquipGemUnit)EquipManager.EquipToGemMap[equip_id];
			if(unit.Slot1!=0)
				num++;
			if(unit.Slot2!=0)
				num++;
			if(unit.Slot3!=0)
				num++;
			if(unit.Slot4!=0)
				num++;
		}
		return num;	
	}
}
