using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
//用于士兵管理的单元脚本   
public class SoldierClass : PagingStorageSlot {
	int mItemID = -1;
	bool hasChanged = false;
	public uint excel_id = 0;
	SoldierUnit soldier_unit;
	public UISprite soldier_icon;
	public UILabel soldier_name;
	public UILabel soldier_num;
	public UIImageButton train_btn;
	public UIImageButton update_btn;
	public UIImageButton dismiss_btn;
	public GameObject soldier_objs;
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
	//初始化界面 
	void InitialData()
	{
		if(gid>=0&&gid<BingYingInfoWin.soldier_map.Count)
		{
			soldier_objs.SetActiveRecursively(true);
			SoldierUnit unit = (SoldierUnit)BingYingInfoWin.soldier_map[BingYingInfoWin.soldier_key_array[gid]];
			soldier_unit = unit;
			SoldierMBInfo soldier_info =  U3dCmn.GetSoldierInfoFromMb(unit.nExcelID1,unit.nLevel2);
			soldier_name.text = soldier_info.Name + " Lv"+unit.nLevel2;
			soldier_num.text = unit.nNum3.ToString();
			soldier_icon.spriteName = U3dCmn.GetIconFromSoldierID(unit.nExcelID1);
			if(IsAdvancedSoldier(unit.nExcelID1,unit.nLevel2))
			{
				update_btn.gameObject.SetActiveRecursively(true);
				train_btn.gameObject.SetActiveRecursively(false);
			}
			else
			{
				update_btn.gameObject.SetActiveRecursively(false);
				train_btn.gameObject.SetActiveRecursively(true);
			}
			if(unit.nNum3 <=0)
			{
				dismiss_btn.gameObject.SetActiveRecursively(false);
			}
			else
			{
				dismiss_btn.gameObject.SetActiveRecursively(true);
			}
			//检查一下是否超出了当前兵种科技（一般来自联盟增兵） 
			if(!IsSoldierTech(unit.nExcelID1,unit.nLevel2))
			{
				train_btn.gameObject.SetActiveRecursively(false);
				update_btn.gameObject.SetActiveRecursively(false);
			}
				
		}
		else 
		{
			soldier_objs.SetActiveRecursively(false);
		}
		
		
	}
	//弹出训练士兵界面 
	void TrainSoldier()
	{
		GameObject obj =  U3dCmn.GetObjFromPrefab("TrainSoldierWin");
		if(obj != null)
			obj.SendMessage("RevealPanel",soldier_unit);
	}
	//升级士兵 
	void UpdateSoldier()
	{
		GameObject obj =  U3dCmn.GetObjFromPrefab("UpdateSoldierWin");
		if(obj != null)
			obj.SendMessage("RevealPanel",soldier_unit);
	}
	//检查是否有高级兵 
	bool IsAdvancedSoldier(uint soldier_excel_id,uint soldier_level)
	{
		
		return SoldierManager.GetTopSoldierInfo(soldier_excel_id).nLevel2>soldier_level?true:false;
	
	}
	//检查是否是超出当前科技上限的兵（一般来自联盟增兵） 
	bool IsSoldierTech(uint soldier_excel_id,uint soldier_level)
	{
		return SoldierManager.GetTopSoldierInfo(soldier_excel_id).nLevel2>=soldier_level?true:false;
	}
	//弹出解散士兵窗口 
	void OpenDismissWin()
	{
		GameObject obj =  U3dCmn.GetObjFromPrefab("DismissSoldierWin");
		if(obj != null)
			obj.SendMessage("RevealPanel",soldier_unit);
	}
	
}
