using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class EquipGemItem : MonoBehaviour {
	public ulong gem_id =0;
	public int slot_id;
	public UISprite icon;
	//public UIImageButton  uprade_btn;
	public UIImageButton  removal_btn;
	public UIImageButton  embed_btn;
	public UILabel 	name;
	public UILabel 	describe;
	// Use this for initialization
	void Start () {
	
	}
	

	public void InitialData()
	{
		if(EquipManager.EquipItemMap.Contains(gem_id))
		{
			icon.gameObject.SetActiveRecursively(true); 
			ItemDesc item_info = (ItemDesc)EquipManager.EquipItemMap[gem_id];
			GemInfo gem_info = (GemInfo)CommonMB.Gem_Map[(uint)item_info.nExcelID2];
			icon.spriteName = gem_info.IconName;
			name.text = gem_info.GemName;
			//describe.text = gem_info.GemDescribe;
			describe.text = U3dCmn.GetGemTipByExcelID((uint)item_info.nExcelID2);
			//uprade_btn.gameObject.SetActiveRecursively(true);
			removal_btn.gameObject.SetActiveRecursively(true);
			embed_btn.gameObject.SetActiveRecursively(false);
		}
		else 
		{
			icon.gameObject.SetActiveRecursively(false); 
			name.text = "";
			describe.text= "";
			//uprade_btn.gameObject.SetActiveRecursively(false);
			removal_btn.gameObject.SetActiveRecursively(false);
			embed_btn.gameObject.SetActiveRecursively(true);
		}
	}
	//打开镶嵌页面 
	void OpenEmbedWin()
	{
		GameObject infowin = U3dCmn.GetObjFromPrefab("GemListWin");
		if(infowin != null)
		{
			infowin.SendMessage("RevealPanel");
			EmbedGemWin.gem_slot = slot_id;
		}
	}
	//摘除宝石 
	void RemovalGem()
	{
		GameObject infowin = U3dCmn.GetObjFromPrefab("EmbedGemWin");
		if(infowin != null)
		{
			EmbedGemWin.gem_slot = slot_id;
			infowin.SendMessage("ReqRemovalGem",gem_id);
			
		}
	}
}
