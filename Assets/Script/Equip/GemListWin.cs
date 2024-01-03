using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using STCCMD;
using CMNCMD;
public class GemListWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	//可用的宝石grid管理器  
	public PagingStorage 	gem_paging_storage;

	// Use this for initialization
	void Awake()
	{
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			transform.localScale = CommonData.iphonescale;
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			transform.localScale = CommonData.ipadscale;
		}
		
	}
	

	//弹出窗口   
	void RevealPanel()
	{
		InitialPanelData();
		tween_position.Play(true);
	}
	//关闭窗口   
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	void InitialPanelData()
	{
		EquipManager.gem_id_list.Clear();
		//初始化仓库里可用的宝石列表 
		for(int i=0;i<CangKuManager.Item_Id_List.Count;i++)
		{
			ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[CangKuManager.Item_Id_List[i]];
			if((uint)CommonMB.ExcelToSort_Map[(uint)item_info.nExcelID2] == (uint)ItemSort.GEMSTONE)
			{
				GemInfo gem_info = (GemInfo)CommonMB.Gem_Map[(uint)item_info.nExcelID2];
				EquipManager.gem_id_list.Add(item_info.nItemID1);
				//print ("eeeeeeeeeeeeeee"+item_info.nItemID1);
				
			}
	
		}
	//	print ("ggggggggggggggggggggggggggg"+EquipManager.gem_id_list.Count);
		//EquipManager.gem_id_list.Sort();
		//初始化宝石面板   
		int capacity = EquipManager.gem_id_list.Count;
		gem_paging_storage.SetCapacity(capacity);
		gem_paging_storage.ResetAllSurfaces();
	}
}
