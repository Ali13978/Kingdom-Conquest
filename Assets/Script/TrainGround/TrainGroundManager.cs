using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class TrainGroundManager : MonoBehaviour {
	//修炼将领队列 
	public static Hashtable HeroTrainingMap = new Hashtable();
	//选中的将领列表 
	public static List<ulong>SelectHeroList = new List<ulong>();
	public static  int train_hour;
	//public static int item_num_max = 
	// Use this for initialization
	void Start () {
		GetHeroTrainData();
	}
	
	//获取将领修炼时间事件列表  
	void GetHeroTrainData()
	{
		CTS_GAMECMD_GET_TRAINING_TE_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_TRAINING_TE;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_TRAINING_TE_T>(req);
	}
	//获取将领修炼时间事件 
	void GetHeroTrainDataRst(byte[] buff)
	{
		STC_GAMECMD_GET_TRAINING_TE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_TRAINING_TE_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_TRAINING_TE_T.enum_rst.RST_OK)
		{	
			HeroTrainingMap.Clear();
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_TRAINING_TE_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			TrainingTimeEvent[] train_array = (TrainingTimeEvent[])DataConvert.ByteToStructArray<TrainingTimeEvent>(data_buff,(int)sub_msg.nNum2);
			for(int i=0;i<train_array.Length;i++)
			{
				HeroTrainingMap.Add(train_array[i].nHeroID3,train_array[i]);
			}
		}
	}
	//请求修炼将领 
	public static void ReqTrainHero(uint hour)
	{
		CTS_GAMECMD_OPERATE_ADD_TRAINING_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_ADD_TRAINING;
		req.nTimeUnitNum3  = hour;
		req.nNum4 = SelectHeroList.Count;
		ulong[] train_array = new ulong[req.nNum4];
		SelectHeroList.CopyTo(train_array);
		req.HeroIDs5 = train_array;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_ADD_TRAINING_T>(req);
		train_hour = (int)hour;
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ADD_TRAINING);
	}
	//请求修炼将领返回  
	public void ReqTrainHeroRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_ADD_TRAINING_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_ADD_TRAINING_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_ADD_TRAINING_T.enum_rst.RST_OK)
		{
			int total_money = 0;
			for(int i=0; i<SelectHeroList.Count;i++)
			{
				TrainingTimeEvent train_unit;
				train_unit.nHeroID3 = SelectHeroList[i];
				train_unit.nBeginTime1 = DataConvert.DateTimeToInt(DateTime.Now);
				train_unit.nEndTime2 = DataConvert.DateTimeToInt(DateTime.Now)+3600*train_hour;
				HireHero hero_info = (HireHero)JiangLingManager.MyHeroMap[train_unit.nHeroID3];
				hero_info.nStatus14 = (int)HeroState.COMBAT_HERO_TRAINING;
				JiangLingManager.MyHeroMap[train_unit.nHeroID3] = hero_info;
				HeroExpGoldInfo exp_gold_info =  (HeroExpGoldInfo)CommonMB.HeroExpGold_Map[hero_info.nLevel19];
				train_unit.nExp4 =(uint)(exp_gold_info.TrainExpPerHour*train_hour);
				total_money += exp_gold_info.TrainMoneyPerHour*train_hour;
				if(HeroTrainingMap.Contains(train_unit.nHeroID3))
					HeroTrainingMap[train_unit.nHeroID3] = train_unit;
				else
					HeroTrainingMap.Add(train_unit.nHeroID3,train_unit);
			}
			U3dCmn.SendMessage("TrainWin","DismissPanel",null);
			U3dCmn.SendMessage("TrainGroundWin","InitialData",null);
			CommonData.player_online_info.Gold -= (uint)total_money;
			PlayerInfoManager.RefreshPlayerDataUI();
		}
		
		train_hour = 0;
		
	}
	//停止修炼选中的将领 
	public static void ReqStopTrainHero(ulong hero_id)
	{
		CTS_GAMECMD_OPERATE_EXIT_TRAINING_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_EXIT_TRAINING;
		req.nHeroID3 = hero_id;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_EXIT_TRAINING_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_EXIT_TRAINING);
	}
	//停止修炼选中的将领返回  
	public void ReqStopTrainHeroRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_EXIT_TRAINING_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_EXIT_TRAINING_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_EXIT_TRAINING_T.enum_rst.RST_OK)
		{
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_OPERATE_EXIT_TRAINING_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			HireHero  hero_unit = DataConvert.ByteToStruct<HireHero>(data_buff);
			JiangLingManager.MyHeroMap[hero_unit.nHeroID1] =hero_unit;
			HeroTrainingMap.Remove(hero_unit.nHeroID1);
			
			U3dCmn.SendMessage("TrainGroundWin","InitialData",null);
		}
	}
	//修炼结束 
	public void TrainingOver(byte[] buff)
	{
		HireHero hero_unit = DataConvert.ByteToStruct<HireHero>(buff);
		JiangLingManager.MyHeroMap[hero_unit.nHeroID1] =hero_unit;
		HeroTrainingMap.Remove(hero_unit.nHeroID1);
		U3dCmn.SendMessage("TrainGroundWin","InitialData",null);
	}
}
