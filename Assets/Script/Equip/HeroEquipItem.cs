using UnityEngine;
using System.Collections;
using STCCMD;
using CMNCMD;
public class HeroEquipItem : MonoBehaviour {
	public ulong equip_id = 0;
	public int excel_id = 0;
	public int slot_id =0;
	public UISprite icon;
	public UISprite kuang;
	// Use this for initialization
	void Start () {
	
	}
	
	public void InitialData()
	{
		if(EquipManager.EquipItemMap.Contains(equip_id))
		{
			
			icon.gameObject.SetActiveRecursively(true); 
			ItemDesc item_info = (ItemDesc)EquipManager.EquipItemMap[equip_id];
			EquipInfo equip_info = (EquipInfo)CommonMB.EquipInfo_Map[(uint)item_info.nExcelID2];
			icon.spriteName = equip_info.IconName;
			excel_id = item_info.nExcelID2;
		}
		else 
		{
			icon.gameObject.SetActiveRecursively(false); 
		}
		kuang.color = new Color(1,1,1,1);
	}
	public void SetSelect(bool isselect)
	{
		if(isselect)
			kuang.color = new Color(1,0,0,1);
		else 
			kuang.color = new Color(1,1,1,1);
	}
}
