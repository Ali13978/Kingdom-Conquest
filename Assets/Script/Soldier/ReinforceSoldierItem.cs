using UnityEngine;
using System.Collections;
using STCCMD;
//当前增援士兵的ICON单元脚本  
public class ReinforceSoldierItem : PagingStorageSlot {
	int mItemID = -1;
	bool hasChanged = false;
	public UILabel name;
	public UISprite icon;
	public UILabel num_label;
	public UISlicedSprite select_bg;
	public UISlicedSprite bg;
	public uint excel_id;
	public SoldierUnit nowinfo;
	string name_str;
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
		name_str = name.text;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	void LateUpdate()
	{
		if(hasChanged)
		{
			hasChanged = false;
			InitialSoldierInfo();
		}
	}
	//初始化配兵兵种信息 
	void InitialSoldierInfo( )
	{
		/*ArrayList soldier_key_array = new ArrayList();
		foreach(DictionaryEntry	de	in	SoldierManager.SoldierMap)  
  		{
			SoldierUnit unit = (SoldierUnit)de.Value;
			if(unit.nNum3 >0 || (WithSoldierWin.select_hero.nArmyType9 == unit.nExcelID1 && WithSoldierWin.select_hero.nArmyLevel10 == unit.nLevel2))
			{
				print (unit.nLevel2+" "+unit.nExcelID1);
				soldier_key_array.Add(de.Key);
			}
				
		}*/
		if(gid>=0 && gid<ReinforceSoldierWin.soldier_key_array.Count)
		{
			ReinforceSoldierWin.soldier_key_array.Sort();
			string soldier_key = (string)ReinforceSoldierWin.soldier_key_array[gid];
			if(!SoldierManager.SoldierMap.Contains(soldier_key))
			{
				//print("rrrrrrrrrrrrr"+soldier_key);
				return;
			}
			//ArrayList soldier_key_array = new ArrayList(SoldierManager.SoldierMap.Keys);
			nowinfo =  (SoldierUnit)SoldierManager.SoldierMap[soldier_key];
			excel_id = nowinfo.nExcelID1;
			SoldierMBInfo soldierMB_info =  U3dCmn.GetSoldierInfoFromMb(nowinfo.nExcelID1,nowinfo.nLevel2);
			name.text = string.Format(name_str,nowinfo.nLevel2,soldierMB_info.Name);
			int soldier_num = 0;
			soldier_num = (int)nowinfo.nNum3;
		
			
			num_label.text = soldier_num.ToString();
			icon.gameObject.SetActiveRecursively(true);
	
			icon.spriteName = U3dCmn.GetIconFromSoldierID(nowinfo.nExcelID1);
			bg.spriteName = "sliced_bg5";
			select_bg.gameObject.SetActiveRecursively(false);
			
			if(gid == 0)
			{
				U3dCmn.SendMessage("ReinforceSoldierWin","ShowDetail",gameObject);
				SetSelect(true);
			}
		}
		else 
		{
			excel_id = 0;
			icon.gameObject.SetActiveRecursively(false);
			select_bg.gameObject.SetActiveRecursively(false);
			bg.spriteName = "transparent";
			name.text = "";
			num_label.text = "";
		}
	}
	//显示将领详细信息 
	//void ShowInfoDetail()
	//{
	//	GameObject.Find("WithSoldierWin").SendMessage("SetWithSoldierPanelData",nowinfo);
	//}
	public void SetSelect(bool isselect)
	{
		if(isselect && excel_id >0)
		{
			//bg.spriteName = "sliced_bg3";
			select_bg.gameObject.SetActiveRecursively(true);
		}
		else if(!isselect && excel_id >0)
		{
			//bg.spriteName = "sliced_bg5";
			select_bg.gameObject.SetActiveRecursively(false);
		}
	}
}
