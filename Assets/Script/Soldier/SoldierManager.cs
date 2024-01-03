using UnityEngine;
using System.Collections;
using System;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class SoldierManager : MonoBehaviour {
	//public static SoldierUnit 
	//服务器获取当前军队信息  
	public static Hashtable SoldierMap = new Hashtable();
	//当前兵种科技信息  
	public static Hashtable SoldierTopMap = new Hashtable();
	// Use this for initialization 
	void Start () {
		//SoldierPage.transform.
		//GameObject obj =  (GameObject)GameObject.Instantiate(SoldierPage);
		
		//GameObject obj2 = NGUITools.AddChild(SoldierGrid, obj);
		//obj2.name = "Page";
		SoldierManager.GetSoldierInfo();	
	}
	//计算当前兵营的总容量 
	public static int GetBingYingCapacity()
	{
		int num =0;
		foreach(DictionaryEntry	de	in BuildingManager.BingYing_Capacity_Map)  
  		{
			num += (int)de.Value;
		}
		return num;
	}
	//计算当前士兵的总量 
	public static int GetSoldierNum()
	{
		int num =0;
		foreach(DictionaryEntry	de	in SoldierMap)  
  		{
			SoldierUnit unit = (SoldierUnit)de.Value;
			num += (int)unit.nNum3;
		}
		foreach(DictionaryEntry	de	in JiangLingManager.MyHeroMap)  
  		{
			HireHero unit = (HireHero)de.Value;
			num += unit.nArmyNum11; 
		}
		return num;
	}
	//请求获取当前士兵信息 
	public static void GetSoldierInfo()
	{
		CTS_GAMECMD_GET_SOLDIER_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_SOLDIER;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_SOLDIER_T >(req);
	}
	//得到当前士兵信息 
	void ProcessSoldierInfo(byte[] buff)
	{
		STC_GAMECMD_GET_SOLDIER_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_SOLDIER_T>(buff);
		if(sub_msg.nRst1 == (byte)STC_GAMECMD_GET_SOLDIER_T.enum_rst.RST_OK)
		{
			SoldierMap.Clear();
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_SOLDIER_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			SoldierUnit[] soldier_list = (SoldierUnit[])DataConvert.ByteToStructArray<SoldierUnit>(data_buff,sub_msg.nNum2);
			for(int i=0;i<soldier_list.Length;i++)
			{
				SoldierMap.Add(soldier_list[i].nExcelID1+"_"+soldier_list[i].nLevel2,soldier_list[i]);
			}
		}
	}
	//训练士兵返回 
	void ProcessTrainSoldierRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_CONSCRIPT_SOLDIER_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_CONSCRIPT_SOLDIER_T>(buff);
		if(sub_msg.nRst1 == (byte)STC_GAMECMD_OPERATE_CONSCRIPT_SOLDIER_T.enum_rst.RST_OK)
		{
			//PlayerInfoManager.GetPlayerInfo();
			CommonData.player_online_info.Gold -= sub_msg.nGold4;
			CommonData.player_online_info.Population -= sub_msg.nNum5;
			PlayerInfoManager.RefreshPlayerDataUI();
			if(SoldierMap.Contains(sub_msg.nExcelID2+"_"+sub_msg.nLevel3))
			{
				SoldierUnit unit = (SoldierUnit)SoldierMap[sub_msg.nExcelID2+"_"+sub_msg.nLevel3];
				unit.nNum3 += (int) sub_msg.nNum5;
				SoldierMap[sub_msg.nExcelID2+"_"+sub_msg.nLevel3] = unit;
			}
			else
			{
				SoldierUnit unit;
				unit.nExcelID1 = sub_msg.nExcelID2;
				unit.nLevel2 = sub_msg.nLevel3;
				unit.nNum3 = (int) sub_msg.nNum5;
				SoldierMap.Add(sub_msg.nExcelID2+"_"+sub_msg.nLevel3,unit);
			}
			
			//刷新训练士兵窗口 关闭弹出窗口 
			U3dCmn.SendMessage("BingYingInfoWin","RefreshUI",null);
			U3dCmn.SendMessage("TrainSoldierWin","DismissPanel",null);
			U3dCmn.SendMessage("BuildingManager","RefreshBingYingInfo",null);
			
			// <新手引导> 训练士兵 ....
			if (NewbieBingYing.processBingYingTrainRst != null)
			{
				NewbieBingYing.processBingYingTrainRst();
				NewbieBingYing.processBingYingTrainRst = null;
			}
		}
		else 
		{
		//	U3dCmn.ShowWarnWindow( U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.UNKNOWN_ERR)+sub_msg.nRst1);
			U3dCmn.SendMessage("TrainSoldierWin","DismissPanel",null);
		}
		
		// <新手引导> 置空 .... 
		NewbieBingYing.processBingYingTrainRst = null;
	}
	//升级士兵返回 
	void ProcessUpdateSoldierRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_UPGRADE_SOLDIER_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_UPGRADE_SOLDIER_T>(buff);
		if(sub_msg.nRst1 == (byte)STC_GAMECMD_OPERATE_UPGRADE_SOLDIER_T.enum_rst.RST_OK)
		{
			CommonData.player_online_info.Gold -= sub_msg.nGold5;
			PlayerInfoManager.RefreshPlayerDataUI();
			if(SoldierMap.Contains(sub_msg.nExclID2+"_"+sub_msg.nFromLevel3))
			{
				SoldierUnit unit = (SoldierUnit)SoldierMap[sub_msg.nExclID2+"_"+sub_msg.nFromLevel3];
				if(unit.nNum3 < sub_msg.nNum6)
					return;
				unit.nNum3 -= (int) sub_msg.nNum6;
				SoldierMap[sub_msg.nExclID2+"_"+sub_msg.nFromLevel3] = unit;
			}
			if(SoldierMap.Contains(sub_msg.nExclID2+"_"+sub_msg.nToLevel4))
			{
				SoldierUnit unit = (SoldierUnit)SoldierMap[sub_msg.nExclID2+"_"+sub_msg.nToLevel4];
				unit.nNum3 += (int) sub_msg.nNum6;
				SoldierMap[sub_msg.nExclID2+"_"+sub_msg.nToLevel4] = unit;
			}
			else
			{
				SoldierUnit unit;
				unit.nExcelID1 = sub_msg.nExclID2;
				unit.nLevel2 = sub_msg.nToLevel4;
				unit.nNum3 = (int) sub_msg.nNum6;
				SoldierMap.Add(sub_msg.nExclID2+"_"+sub_msg.nToLevel4,unit);
			}
			
			//刷新训练士兵窗口 关闭弹出窗口 
			U3dCmn.SendMessage("BingYingInfoWin","RefreshUI",null);
			U3dCmn.SendMessage("UpdateSoldierWin","DismissPanel",null);
		}
		else 
		{
			//U3dCmn.ShowWarnWindow( U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.UNKNOWN_ERR));
			U3dCmn.SendMessage("TrainSoldierWin","DismissPanel",null);
		}
	}
	//根据EXCEL_ID获取最高级兵种科技信息  
	public static SoldierUnit  GetTopSoldierInfo(uint excel_id)
	{
		SoldierUnit soldier_unit = new SoldierUnit();
		foreach(DictionaryEntry de in SoldierManager.SoldierTopMap)
		{
			SoldierUnit unit = (SoldierUnit)de.Value;
			if(unit.nExcelID1 == excel_id)
			{
				soldier_unit = unit;
				break;
			}
		}
		return soldier_unit;
	}
	//解散士兵返回 
	void DismissSoldierRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_DISMISS_SOLDIER_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_DISMISS_SOLDIER_T>(buff);
		CommonData.player_online_info.Population += sub_msg.nNum4;
		PlayerInfoManager.RefreshPlayerDataUI();
		
		
		if(SoldierMap.Contains(sub_msg.nExcelID2+"_"+sub_msg.nLevel3))
		{
			SoldierUnit unit = (SoldierUnit)SoldierMap[sub_msg.nExcelID2+"_"+sub_msg.nLevel3];
			if(unit.nNum3 <= sub_msg.nNum4)
			{
				SoldierMap.Remove(sub_msg.nExcelID2+"_"+sub_msg.nLevel3);
			}
			else 
			{
				unit.nNum3 -= (int) sub_msg.nNum4;
				SoldierMap[sub_msg.nExcelID2+"_"+sub_msg.nLevel3] = unit;
			}
			
		}
		
		//刷新训练士兵窗口 关闭弹出窗口 
		U3dCmn.SendMessage("BingYingInfoWin","RefreshUI",null);
		U3dCmn.SendMessage("DismissSoldierWin","DismissPanel",null);
		U3dCmn.SendMessage("BuildingManager","RefreshBingYingInfo",null);
	}
	//增援盟友到达通知 
	void ReinforceRst(byte[] buff)
	{
		STC_GAMECMD_DONATE_SOLDIER_TE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_DONATE_SOLDIER_TE_T>(buff);
		string soldier_key = sub_msg.nExcelID2+"_"+sub_msg.nLevel3;
		if(SoldierMap.Contains(soldier_key))
		{
			SoldierUnit unit = (SoldierUnit)SoldierMap[soldier_key];
			unit.nNum3 += (int) sub_msg.nNum4;
			SoldierMap[soldier_key] = unit;
		}
		else
		{
			SoldierUnit unit;
			unit.nExcelID1 = sub_msg.nExcelID2;
			unit.nLevel2 = sub_msg.nLevel3;
			unit.nNum3 = (int) sub_msg.nNum4;
			SoldierMap.Add(soldier_key,unit);
		}
	}
}
