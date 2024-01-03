using UnityEngine;
using System.Collections;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class CangKuItem : PagingStorageSlot {
	int mItemID = -1;
	bool hasChanged = false;
	public int excel_id = -1;
	public ulong item_id = 0;
	public int 		item_num;
	public uint 	sell_price;
	public string 	describe;
	public UISprite icon;
	public UISlicedSprite select_bg;
	public UILabel 	name_label;
	public UILabel 	num_label;
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
	// Use this for initialization
	void Start () {
		icon.spriteName = "transparent";
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
		gameObject.SetActiveRecursively(true);
		gameObject.GetComponent<Collider>().enabled = true;
	
		bool hasdata = false;
		if(CangKuManager.select_panel_sort == (uint)ItemSort.ALLITEM)
		{
			if(gid<CangKuManager.all_id_list.Count && gid>=0)
			{
				if(CangKuManager.CangKuItemMap.Contains(CangKuManager.all_id_list[gid]))
				{
					hasdata = true;
					ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[CangKuManager.all_id_list[gid]];
					item_id = CangKuManager.all_id_list[gid];
					excel_id = item_info.nExcelID2;
					item_num = item_info.nNum3;
					uint sort_id = (uint)CommonMB.ExcelToSort_Map[(uint)excel_id];
					SetItemInfo(sort_id,item_id);
				}
			}
		
		}
		else if(CangKuManager.select_panel_sort == (uint)ItemSort.EQUIP)
		{
			if(gid<CangKuManager.equip_id_list.Count && gid>=0)
			{
				if(CangKuManager.CangKuItemMap.Contains(CangKuManager.equip_id_list[gid]))
				{
					hasdata = true;
					ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[CangKuManager.equip_id_list[gid]];
					item_id = CangKuManager.all_id_list[gid];
					excel_id = item_info.nExcelID2;
					item_num = item_info.nNum3;
					uint sort_id = (uint)CommonMB.ExcelToSort_Map[(uint)excel_id];
					SetItemInfo(sort_id,item_id);
				}
			}
		
		}
		else if(CangKuManager.select_panel_sort == (uint)ItemSort.GEMSTONE)
		{
			if(gid<CangKuManager.gem_id_list.Count && gid>=0)
			{
				if(CangKuManager.CangKuItemMap.Contains(CangKuManager.gem_id_list[gid]))
				{
					hasdata = true;
					ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[CangKuManager.gem_id_list[gid]];
					item_id = CangKuManager.gem_id_list[gid];
					excel_id = item_info.nExcelID2;
					item_num = item_info.nNum3;
					uint sort_id = (uint)CommonMB.ExcelToSort_Map[(uint)excel_id];
					SetItemInfo(sort_id,item_id);
				}
			}
		
		}
		else if(CangKuManager.select_panel_sort == (uint)ItemSort.ARTICLES)
		{
			if(gid<CangKuManager.article_id_list.Count && gid>=0)
			{
				if(CangKuManager.CangKuItemMap.Contains(CangKuManager.article_id_list[gid]))
				{
					hasdata = true;
					ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[CangKuManager.article_id_list[gid]];
					item_id = CangKuManager.article_id_list[gid];
					excel_id = item_info.nExcelID2;
					item_num = item_info.nNum3;
					uint sort_id = (uint)CommonMB.ExcelToSort_Map[(uint)excel_id];
					SetItemInfo(sort_id,item_id);
					
				}
			}
		
		}
		else if(CangKuManager.select_panel_sort == (uint)ItemSort.TASKITEM)
		{
			if(gid<CangKuManager.task_id_list.Count && gid>=0)
			{
				if(CangKuManager.CangKuItemMap.Contains(CangKuManager.task_id_list[gid]))
				{
					hasdata = true;
					ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[CangKuManager.task_id_list[gid]];
					item_id = CangKuManager.task_id_list[gid];
					excel_id = item_info.nExcelID2;
					item_num = item_info.nNum3;
					uint sort_id = (uint)CommonMB.ExcelToSort_Map[(uint)excel_id];
					SetItemInfo(sort_id,item_id);
				}
			}
		
		}
		if(!hasdata)
		{
			excel_id = -1;
	 		item_id = 0;
			icon.spriteName = "transparent";
			icon.gameObject.SetActiveRecursively(false);
			gameObject.GetComponent<Collider>().enabled = false;
			name_label.text = "";
			num_label.text = "";
		}
		select_bg.gameObject.SetActiveRecursively(false);
	}
	public void SetItemInfo(uint sort_id,ulong item_id)
	{
		if(sort_id == (uint)ItemSort.EQUIP)
		{
			EquipInfo equip_info = (EquipInfo)CommonMB.EquipInfo_Map[(uint)excel_id];
			
			icon.spriteName = equip_info.IconName;
			int gem_num = EquipManager.GetEmbedGemNum(item_id);
			
			if(gem_num >0)
				name_label.text = equip_info.EquipName +"\n"+U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GEM_EMBED)+gem_num+"/4";
			else
				name_label.text = equip_info.EquipName +"\n";
			sell_price = (uint)equip_info.SellPrice;
			num_label.text = "";
			describe = equip_info.EquipDescribe;
		
		}
		else if(sort_id == (uint)ItemSort.GEMSTONE)
		{
			
			GemInfo gem_info = (GemInfo)CommonMB.Gem_Map[(uint)excel_id];
			icon.spriteName = gem_info.IconName;
			name_label.text = gem_info.GemName+"\n";
			sell_price = (uint)gem_info.SellPrice;
			num_label.text = item_num.ToString();
			describe = gem_info.GemDescribe;
		}
		else if(sort_id == (uint)ItemSort.ARTICLES)
		{
			ArticlesInfo articles_info = (ArticlesInfo)CommonMB.Articles_Map[(uint)excel_id];
			icon.spriteName = articles_info.IconName;
			name_label.text = articles_info.Name+"\n";
			sell_price = (uint)articles_info.SellPrice;
			num_label.text = item_num.ToString();
			describe = articles_info.Describe;
		}
		else if(sort_id == (uint)ItemSort.TASKITEM)
		{
			TaskInfo task_info = (TaskInfo)CommonMB.Task_Map[(uint)excel_id];
			icon.spriteName = task_info.IconName;
			name_label.text = task_info.Name+"\n";
			sell_price = (uint)task_info.SellPrice;
			num_label.text = item_num.ToString();
			describe = task_info.Describe;
		}
	
	}
	public void SetSelect(bool isselect)
	{
		if(isselect)
			select_bg.gameObject.SetActiveRecursively(true);
		else 
			select_bg.gameObject.SetActiveRecursively(false);
	}
	
}
