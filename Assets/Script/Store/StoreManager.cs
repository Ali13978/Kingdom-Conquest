using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class StoreManager : MonoBehaviour {
	//商城当前选择的面板  
	public  static uint select_panel_sort;
	public static  List<uint> equip_id_list = new List<uint>();
	public static  List<uint> gem_id_list = new List<uint>();
	public static  List<uint> articles_id_list = new List<uint>();
	// Use this for initialization
	void Start () {
		//item = new StoreItem();
		//StoreItem.itemid = 1111111;
		InitialStoreData();
	}
	
	void OpenStoreWin()
	{
		GameObject win = U3dCmn.GetObjFromPrefab("StoreWin"); 
		if(win != null)
		{
			win.SendMessage("RevealPanel");
		}	
		
	}
	//初始化 
	void InitialStoreData()
	{
		equip_id_list.Clear();
		gem_id_list.Clear();
		articles_id_list.Clear();
		foreach(DictionaryEntry	de	in CommonMB.StoreInfo_Map)
		{
			
			StoreItemInfo item_info = (StoreItemInfo)de.Value;
			uint sort_id = (uint)CommonMB.ExcelToSort_Map[item_info.ExcelID];
			if(sort_id == (uint)ItemSort.EQUIP)
			{
				equip_id_list.Add(item_info.ExcelID);
			}
			else if(sort_id == (uint)ItemSort.GEMSTONE)
			{
				gem_id_list.Add(item_info.ExcelID);
			}
			else if(sort_id == (uint)ItemSort.ARTICLES)
			{
				articles_id_list.Add(item_info.ExcelID);
			}
		}
		equip_id_list.Sort();
		gem_id_list.Sort();
		articles_id_list.Sort();
	}
	//购买商城道具返回 
	void ProcessBuyRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_BUY_ITEM_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_BUY_ITEM_T>(buff);
		if(sub_msg.nRst1 == (byte)STC_GAMECMD_OPERATE_BUY_ITEM_T.enum_rst.RST_OK)
		{
			if(sub_msg.nMoneyType5 == (uint)money_type.money_type_crystal)
			{
				CommonData.player_online_info.Crystal -= sub_msg.nMoneyNum4;
			}
			else if(sub_msg.nMoneyType5 == (uint)money_type.money_type_diamond)
			{
				CommonData.player_online_info.Diamond -= sub_msg.nMoneyNum4;
			}
			PlayerInfoManager.RefreshPlayerDataUI();
			//PlayerInfoManager.GetPlayerInfo();
			
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_OPERATE_BUY_ITEM_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			byte[] temparray = new byte[8];
			ulong[] itemid_array = new ulong[sub_msg.nItemIDNum6];
			for (int i = 0; i < sub_msg.nItemIDNum6; i++)  
		    {  
				Array.Copy(data_buff,i*8,temparray,0,8);
		        itemid_array[i] =(ulong)( BitConverter.ToUInt64(temparray,0));  
		    }
			//修改仓库ITEM表 
			if(sub_msg.nItemIDNum6 == 1)
			{
				CangKuManager.AddItem(itemid_array[0],(int)sub_msg.nExcelID2,(int)sub_msg.nNum3);
				
			}
			else if(sub_msg.nItemIDNum6>1)
			{
				//这里应该是武将的装备 
				for (int i = 0; i < sub_msg.nItemIDNum6; i++)  
		    	{  
					CangKuManager.AddItem(itemid_array[i],(int)sub_msg.nExcelID2,1);
				}
			}
		
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.STORE_BUY_OK));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_BUY_ITEM_T.enum_rst.RST_MONEY_NOT_ENOUGH)
		{
			if(sub_msg.nMoneyType5 == (uint)money_type.money_type_diamond)
				U3dCmn.ShowRechargePromptWindow();
				//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.DIAMOND_NOT_ENOUGH));
			else if(sub_msg.nMoneyType5 == (uint)money_type.money_type_crystal)
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CRYSTAL_NOT_ENOUGH));
		}
		else
		{
			//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.UNKNOWN_ERR)+sub_msg.nRst1);
		}
	}
	//购买金币返回 
	void BuyGoldRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_CONVERT_DIAMOND_GOLD_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_CONVERT_DIAMOND_GOLD_T>(buff);
		if(sub_msg.nRst1 == (byte)STC_GAMECMD_OPERATE_CONVERT_DIAMOND_GOLD_T.enum_rst.RST_OK)
		{
			CommonData.player_online_info.Gold += sub_msg.nGold3;
			CommonData.player_online_info.Diamond -= sub_msg.nDiamond2;
			PlayerInfoManager.RefreshPlayerDataUI();
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.STORE_BUY_OK));
		}
	}
	//打开充值界面 
	void OpenReChargeWin()
	{
		if(CommonData.VERSION == VERSION_TYPE.VERSION_APPSTORE)
		{
			GameObject win = U3dCmn.GetObjFromPrefab("RechargeWinApple"); 
			if(win != null)
			{
				win.SendMessage("RevealPanel",null);
			}
		}
		else if(CommonData.VERSION == VERSION_TYPE.VERSION_INNER)
		{
			GameObject win = U3dCmn.GetObjFromPrefab("RechargeWinApple"); 
			if(win != null)
			{
				win.SendMessage("RevealPanel",null);
			}
		}
	}
}
