using UnityEngine;
using System.Collections;
using STCCMD;
using CMNCMD;
public class CangKuGemItem : PagingStorageSlot {
	int mItemID = -1;
	bool hasChanged = false;
	public ulong gem_id = 0;
	public UISprite icon;
	public UISprite kuang;
	public UILabel 	name;
	public UILabel 	num;
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
		if(gid < EquipManager.gem_id_list.Count && gid>=0)
		{
			//print ("gg"+gid);
			gem_id = EquipManager.gem_id_list[gid];
			gameObject.SetActiveRecursively(true);
			gameObject.GetComponent<Collider>().enabled = true;
			ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[gem_id];
			GemInfo gem_info = (GemInfo)CommonMB.Gem_Map[(uint)item_info.nExcelID2];
			icon.spriteName = gem_info.IconName;
			name.text = gem_info.GemName;
			num.text = item_info.nNum3.ToString();
			//embedinfo.text = geminfo_str +GemEmbedGemNum(equip_id).ToString()+"/"+"4";
		}
		else 
		{
			gem_id = 0;
			gameObject.GetComponent<Collider>().enabled = false;
			icon.spriteName = "transparent";
			icon.gameObject.SetActiveRecursively(false); 
			kuang.gameObject.SetActiveRecursively(false); 
			name.text = "";
			num.text = "";
			//embedinfo.text = "";
		}
	}
	void SelectGem()
	{
		GameObject obj = GameObject.Find("GemListWin");
		if(obj!= null)
		{
			obj.SendMessage("DismissPanel");
		}
		obj = GameObject.Find("EmbedGemWin");
		if(obj!=null)
		{
			obj.SendMessage("ReqEmbedGem",gem_id);
		}
	}
}
