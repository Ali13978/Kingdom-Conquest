using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using STCCMD;
using CTSCMD;
using CMNCMD;
public class CangKuManager : MonoBehaviour {
	//更新仓库信息的回调注册表 里面包含需要回调的object和回调函数名  
	public static Hashtable CallBackObjMap = new Hashtable();
	//未装备的ITEM列表(包括已经镶嵌宝石但是未穿到武将身上的装备)  
	public static Hashtable CangKuItemMap = new Hashtable();
	//ITEMI—Id列表 为了保证物品的先后顺序 
	public static List<ulong> Item_Id_List = new List<ulong>();
	
	//仓库当前选择的面板 
	public  static uint select_panel_sort;
	//全部面ITEM列表 
	public  static List<ulong> all_id_list = new List<ulong>();
	//装备面板ITEM列表  
	public static List<ulong> equip_id_list = new List<ulong>();
	//宝石面板ITEM列表  
	public static List<ulong> gem_id_list = new List<ulong>();
	//用品面板ITEM列表  
	public static List<ulong> article_id_list = new List<ulong>();
	//任务道具面板ITEM列表  
	public static List<ulong> task_id_list = new List<ulong>();
	//当前使用的道具ID ]
	public static int 	use_excel_id;
	public static ulong use_item_id;
	public static int 	use_num;
	// Use this for initialization
	void Start () {
		ReqCangKuData();
	}
	
	//注册仓库信息刷新回调  
	public static void RegisterCallBack(GameObject obj,string func_name)
	{
		if(!CallBackObjMap.Contains(obj))
		{
			CallBackObjMap.Add(obj,func_name);
		}
		else
		{
			CallBackObjMap[obj] = func_name;
		}
	} 
	//注销仓库信息刷新回调  
	public static void UnRegisterCallBack(GameObject obj)
	{
		if(CallBackObjMap.Contains(obj))
		{
			CallBackObjMap.Remove(obj);
		}
	} 
	//请求未装备的仓库数据  
	public static void ReqCangKuData()
	{	
		CTS_GAMECMD_GET_ITEM_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_ITEM;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_ITEM_T >(req);
		
		
	}
	void OpenCangKuInfoWin()
	{
		GameObject win = U3dCmn.GetObjFromPrefab("CangKuWin"); 
		if(win != null)
		{
			win.SendMessage("RevealPanel");
		}
	}
	//请求未装备的仓库数据返回 
	void ReqCangKuDataRst(byte[] buff)
	{
		Item_Id_List.Clear();
		CangKuItemMap.Clear();
		STC_GAMECMD_GET_ITEM_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_ITEM_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_ITEM_T.enum_rst.RST_OK)
		{	
			
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_ITEM_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			ItemDesc[] item_array = (ItemDesc[])DataConvert.ByteToStructArray<ItemDesc>(data_buff,sub_msg.nNum2);
			
			for(int i=0;i<item_array.Length;i++)
			{
				Item_Id_List.Add(item_array[i].nItemID1);
				CangKuItemMap.Add(item_array[i].nItemID1,item_array[i]);
			}
			//执行各OBJ的回调函数刷新数据 
			Hashtable callback_map = (Hashtable)CallBackObjMap.Clone();
			foreach(DictionaryEntry	de	in	callback_map)  
	  		{
				GameObject obj = (GameObject)de.Key;
				string func_name = (string)de.Value;
				if(obj!=null && func_name!="")
				{
					obj.SendMessage(func_name);
				}
			}
		}
	}
	//减少物品 途径有：卖掉、装备到武将身上(同步操作缓存) 
	public static void SubItem(ulong item_id,int num)
	{
	
		if(CangKuManager.CangKuItemMap.Contains(item_id))
		{
			ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[item_id];
			if(item_info.nNum3 > num)
			{
				item_info.nNum3-=num;
				CangKuManager.CangKuItemMap[item_id] =item_info; 
				
			}
			else
			{
				CangKuManager.Item_Id_List.Remove(item_id);
				CangKuManager.CangKuItemMap.Remove(item_id);
				
			}
		}
	}
	//增加物品 途径有：商城购买、装备上拆卸 副本掉落或者摇钱树(同步操作缓存) 
	public static void AddItem(ulong item_id,int excel_id,int num)
	{
		if(item_id!=0)
		{
			if(CangKuManager.CangKuItemMap.Contains(item_id))
			{
				ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[item_id];
				item_info.nNum3+=num;
				CangKuManager.CangKuItemMap[item_id] =item_info;
			}
			else
			{	
				ItemDesc item_info ;
				item_info.nItemID1 = item_id;
				item_info.nExcelID2 = excel_id;
				item_info.nNum3 = num;
				CangKuManager.CangKuItemMap.Add(item_info.nItemID1,item_info);
				CangKuManager.Item_Id_List.Add(item_id);
			}
		}
	}
	//根据EXCELID获取用品ID 这里不包括装备  
	public static ItemDesc GetItemInfoByExcelID(int excel_id)
	{
		ItemDesc item = new ItemDesc();
		for(int i=0;i<Item_Id_List.Count;i++)
		{
			ItemDesc item_info = (ItemDesc)CangKuItemMap[Item_Id_List[i]];
			if(item_info.nExcelID2 == excel_id)
			{
				item = item_info;
				break;
				
			}		
		}
		return item;
	}
	//根据ITEMID获取物品种类 
	public static uint GetSortIDByItemID(ulong item_id)
	{
		uint sort_id = 0;
		if(CangKuManager.CangKuItemMap.Contains(item_id))
		{
			ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[item_id];
			int excel_id = item_info.nExcelID2;
			sort_id = (uint)CommonMB.ExcelToSort_Map[(uint)excel_id];
		}
	
		return sort_id;
	}
	//对武将使用道具 
	public static void UseArticlesItem(int excel_id,ulong item_id,ulong hero_id,int num)
	{
		CTS_GAMECMD_USE_ITEM_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_USE_ITEM;
		req.nHeroID3 = hero_id;
		req.nExcelID4 = excel_id;
		req.nNum5 = num;
		TcpMsger.SendLogicData<CTS_GAMECMD_USE_ITEM_T >(req);
		use_excel_id = excel_id;
		use_item_id = item_id;
		use_num = num;
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_USE_ITEM);
		//print ("uuuuuuuuuuuuuuuuuuuuuu"+hero_id+" "+excel_id+" "+num);
	}
	//使用道具返回 
	void UseArticlesItemRst(byte[] buff)
	{
		STC_GAMECMD_USE_ITEM_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_USE_ITEM_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_USE_ITEM_T.enum_rst.RST_OK)
		{
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_ITEM_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			if(sub_msg.nType2 == (int)UPDATETYPE.SYNC_TYPE_HERO) //对将领使用道具 经验丹 强化丹等 重新刷新武将属性 
			{
				HeroDesc hero_unit = DataConvert.ByteToStruct<HeroDesc>(data_buff);
				//print ("jjjjjjjjjjjjjjj"+hero_unit.nExp15);
				if(!JiangLingManager.MyHeroMap.Contains(hero_unit.nHeroID10))
					return ;
				HireHero unit = JiangLingManager.UpdateHeroInfo(hero_unit.nHeroID10,hero_unit);
				if(use_excel_id == (int)ARTICLES.YINGXIONGCHENGZHANG)
				{
					CangKuManager.SubItem(use_item_id,use_num);
					U3dCmn.SendMessage("HeroStrengthenWin","ReqStrengthenHeroRst",unit);
					//刷新将领列表页 
					U3dCmn.SendMessage("JiangLingInfoWin","SetJiangLingInfoPanelData",unit);
				}
				else if(use_excel_id == (int)ARTICLES.YINGXIONGJINGYAN)
				{
					CangKuManager.SubItem(use_item_id,use_num);
					U3dCmn.SendMessage("JiangLingExpWin","DismissPanel",null);
					//刷新将领列表页 
					U3dCmn.SendMessage("JiangLingInfoWin","SetJiangLingInfoPanelData",unit);
				}
				//SubItem(use_item_id,use_num);
				
			}
			else if(sub_msg.nType2 == (int)UPDATETYPE.SYNC_TYPE_CHAR) //对角色使用道具 
			{
				/*CharDesc char_unit = DataConvert.ByteToStruct<CharDesc>(data_buff);
				CommonData.player_online_info.Level = (uint)char_unit.nLevel1;
				CommonData.player_online_info.Exp = (uint)char_unit.nExp2;
				CommonData.player_online_info.Diamond = (uint)char_unit.nDiamond3;
				CommonData.player_online_info.Crystal = (uint)char_unit.nCrystal4;
				CommonData.player_online_info.Gold = (uint)char_unit.nGold5;
				CommonData.player_online_info.Vip = (uint)char_unit.nVIP6;
				PlayerInfoManager.RefreshPlayerDataUI();*/
				CangKuManager.SubItem(use_item_id,use_num);
				U3dCmn.SendMessage("CangKuWin","RefreshSelect",null);
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.USE_SUCCESS));
			}
			else if(sub_msg.nType2 == (int)UPDATETYPE.SYNC_TYPE_ITEM) //各种礼包的使用  
			{
				CangKuManager.SubItem(use_item_id,use_num);
				UseItemDesc use_item_info = DataConvert.ByteToStruct<UseItemDesc>(data_buff);
				head_len = U3dCmn.GetSizeofSimpleStructure<UseItemDesc>();
			 	data_len = data_buff.Length - head_len;
				byte[] sub_data_buff = new byte[data_len];
				Array.Copy(data_buff,head_len,sub_data_buff,0,data_len);
				AddItemDesc[] item_array = (AddItemDesc[])DataConvert.ByteToStructArray<AddItemDesc>(sub_data_buff,use_item_info.nNum1);
				for(int i=0;i<use_item_info.nNum1;i++)
				{
					CangKuManager.AddItem(item_array[i].nItemID1,item_array[i].nExcelID2,item_array[i].nNum3);
				}
				U3dCmn.SendMessage("CangKuWin","RefreshSelect",null);
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.USE_SUCCESS));
				
				// < 新手引导> 礼包使用 ...
				if (NewbieBackpack.processUseArticlesItemRst != null)
				{
					NewbieBackpack.processUseArticlesItemRst();
					NewbieBackpack.processUseArticlesItemRst = null;
				}
			}
			
			else if(sub_msg.nType2 == (int)UPDATETYPE.SYNC_TYPE_DRUG)//使用草药包 
			{
				CangKuManager.SubItem(use_item_id,use_num);
				DrugDesc drug_unit = DataConvert.ByteToStruct<DrugDesc>(data_buff);		
				CommonData.player_online_info.CaoYao =(uint)drug_unit.nNum1;
				U3dCmn.SendMessage("CangKuWin","RefreshSelect",null);
				U3dCmn.SendMessage("HospitalWin","InitialData",null);
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.USE_SUCCESS));
			}
			//else if()
	
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_USE_ITEM_T.enum_rst.RST_USE_IN_COMBAT)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.MARCH_NOT_USE_TRUCE));
		}
		else
		{
			use_excel_id = 0;
			use_item_id = 0;
			use_num = 0;
		}
	}
}
