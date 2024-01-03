using UnityEngine;
using System.Collections;
using CMNCMD;
public class StoreItem : PagingStorageSlot {
	int mItemID = -1;
	bool hasChanged = false;
	public uint excel_id = 0;
	public UISprite icon;
	public UISprite money_type;
	public UILabel 	name;
	public UILabel 	price;
	public UISlicedSprite select_bg;
	public string describe;
	public byte sell_by_crystal;
	public uint diamond_price;
	public uint crystal_price;
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
		bool hasdata = false;
		if(StoreManager.select_panel_sort == (uint)ItemSort.EQUIP)
		{
			if(gid<StoreManager.equip_id_list.Count && gid>=0)
			{
				hasdata = true;
				excel_id = (uint)StoreManager.equip_id_list[gid];
				//SetItemInfo(excel_id);
			}
		}
		else if(StoreManager.select_panel_sort == (uint)ItemSort.GEMSTONE)
		{
			if(gid<StoreManager.gem_id_list.Count)
			{
				hasdata = true;
				excel_id = (uint)StoreManager.gem_id_list[gid];
				//SetItemInfo(excel_id);
			}
		}
		else if(StoreManager.select_panel_sort == (uint)ItemSort.ARTICLES)
		{
			if(gid<StoreManager.articles_id_list.Count)
			{
				hasdata = true;	
				excel_id = (uint)StoreManager.articles_id_list[gid];
				//SetItemInfo(excel_id);
			}
		}
		
		if(hasdata) 
		{
			gameObject.SetActiveRecursively(true);
			gameObject.GetComponent<Collider>().enabled = true;
			money_type.spriteName = "zhuanshi";
			SetItemInfo(excel_id);
		}
		else
		{
			excel_id = 0;
			gameObject.GetComponent<Collider>().enabled = false;
			icon.gameObject.SetActiveRecursively(false);
			money_type.gameObject.SetActiveRecursively(false);
			price.text = "";
			name.text = "";
		}
		select_bg.gameObject.SetActiveRecursively(false);
	}
	public void SetItemInfo(uint excel_id)
	{
		StoreItemInfo item_info = (StoreItemInfo)CommonMB.StoreInfo_Map[(uint)excel_id];
		uint sort_id = (uint)CommonMB.ExcelToSort_Map[excel_id];
		if(sort_id == (uint)ItemSort.EQUIP)
		{
			EquipInfo equip_info = (EquipInfo)CommonMB.EquipInfo_Map[(uint)excel_id];
			icon.spriteName = equip_info.IconName;
			name.text = equip_info.EquipName;
			sell_by_crystal = item_info.SellByCrystal;
			crystal_price = item_info.CrystalPrice;
			diamond_price = item_info.DiamondPrice;
			describe = equip_info.EquipDescribe;
		
		}
		else if(sort_id == (uint)ItemSort.GEMSTONE)
		{
			GemInfo gem_info = (GemInfo)CommonMB.Gem_Map[(uint)excel_id];
			icon.spriteName = gem_info.IconName;
			name.text = gem_info.GemName;
			sell_by_crystal = item_info.SellByCrystal;
			crystal_price = item_info.CrystalPrice;
			diamond_price = item_info.DiamondPrice;
			describe = gem_info.GemDescribe;
		}
		else if(sort_id == (uint)ItemSort.ARTICLES)
		{
			
			ArticlesInfo articles_info = (ArticlesInfo)CommonMB.Articles_Map[(uint)excel_id];
			icon.spriteName = articles_info.IconName;
			name.text = articles_info.Name;
			sell_by_crystal = item_info.SellByCrystal;
			crystal_price = item_info.CrystalPrice;
			diamond_price = item_info.DiamondPrice;
			describe = articles_info.Describe;
			
		}
		if(item_info.SellByCrystal == 1) //是否支持水晶购买  
		{	
			price.text = item_info.CrystalPrice.ToString();
			money_type.gameObject.SetActiveRecursively(true);
		}
		else if(item_info.SellByCrystal == 0)
		{
			price.text = item_info.DiamondPrice.ToString();
			money_type.gameObject.SetActiveRecursively(true);
		}
		else if(item_info.SellByCrystal == 2)
		{
			price.text = "";
			money_type.gameObject.SetActiveRecursively(false);
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
