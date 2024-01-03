using UnityEngine;
using System.Collections;
using System;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class ProduceManager : MonoBehaviour {

	//建筑或者升级时间队列结构 
	public struct ProduceTimeTeUnit
	{
		//生产状态 
		public uint 	ProduceState;
		//剩余时间 
		public uint 	time;
		//产量 
		public uint 	Production;
	}
	// Use this for initialization
	void Start () {
		StartCoroutine("ReqProduceInfo");
	}
	
	//获取生产列表  
	public static void GetProduceList()
	{
		CTS_GAMECMD_GET_PRODUCTION_EVENT_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_PRODUCTION_EVENT;
		req.nAccountID3 = CommonData.player_online_info.AccountID;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_PRODUCTION_EVENT_T >(req);
	}
	//获取生产时间队列   
	public static void GetProduceTEList()
	{
		CTS_GAMECMD_GET_PRODUCTION_TE_LIST_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_PRODUCTION_TE_LIST;
		req.nAccountID3 = CommonData.player_online_info.AccountID;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_PRODUCTION_TE_LIST_T >(req);
	}
	//等待个人信息返回后 请求生产事件 
	IEnumerator ReqProduceInfo()
	{
		while(CommonData.player_online_info.AccountID==0)
		{
			yield return null;
		}
		GetProduceList();
		GetProduceTEList();
	}
	//生产金子返回结果 
	void ProcessProduceRst(byte[] buff)
	{
		
		STC_GAMECMD_OPERATE_PRODUCE_GOLD_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_PRODUCE_GOLD_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_PRODUCE_GOLD_T.enum_rst.RST_OK)
		{
			GameObject obj= (GameObject)BuildingManager.JinKuang_Map[(int)sub_msg.nAutoID2];
			if(obj!=null)
			{	
				obj.SendMessage("ProcessProduceRst");
			}
		}
	}
	// 生产事件 查看时候可以收获金子 
	void ProcessProduceEvent(byte[] buff)
	{
		STC_GAMECMD_GET_PRODUCTION_EVENT_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_PRODUCTION_EVENT_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_PRODUCTION_EVENT_T.enum_rst.RST_OK)
		{	
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_PRODUCTION_EVENT_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			ProductionEvent[] product_array = (ProductionEvent[])DataConvert.ByteToStructArray<ProductionEvent>(data_buff,sub_msg.nNum3);
			for(int i=0;i<product_array.Length;i++)
			{
				
				if(product_array[i].nType3 == (uint)te_type_production.te_subtype_production_gold)
				{
					GameObject obj= (GameObject)BuildingManager.JinKuang_Map[(int)product_array[i].nAutoID1];
					if(obj!=null)
					{
						//收获金子 
						obj.SendMessage("ProcessGoldResult",product_array[i].nProduction2);
					}
					
				}
			}
		}
	}
	// 生产时间队列 
	void ProcessProduceTimeEvent(byte[] buff)
	{
		STC_GAMECMD_GET_PRODUCTION_TE_LIST_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_PRODUCTION_TE_LIST_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_PRODUCTION_TE_LIST_T.enum_rst.RST_OK)
		{
			/*
			// reset gold alert info
			GameObject API91Man = GameObject.Find("91Manager");
			if( API91Man!=null )
			{
				// very strange that const 0 will be explained as no param
				print("**** clear goldmine time");
				API91Man.SendMessage("receiveGoldmineTime", -1);
			}
			*/
		
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_PRODUCTION_TE_LIST_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			ProductionTimeEvent[] produce_time_array = (ProductionTimeEvent[])DataConvert.ByteToStructArray<ProductionTimeEvent>(data_buff,sub_msg.nNum4);
			for(int i=0;i<produce_time_array.Length;i++)
			{
				//produce_time_array[i].
				if(produce_time_array[i].nType3 == (uint)te_type_production.te_subtype_production_gold)
				{
					GameObject obj= (GameObject)BuildingManager.JinKuang_Map[(int)produce_time_array[i].nAutoID1];
					if(obj!=null)
					{	
						ProduceTimeTeUnit unit;
						unit.ProduceState = (uint)ProduceState.PRODUCE;
						unit.time = produce_time_array[i].nEndTime5-sub_msg.nTimeNow3;
						unit.Production = produce_time_array[i].nProduction2;
						obj.SendMessage("ProcessProduceTERst",unit);
					}
				}
				//print ("time event"+produce_time_array[i].nProduction2);
			}
		}
	}
	//请求收获金子的返回结果  
	void ProcessGatherGold(byte[] buff)
	{
		STC_GAMECMD_OPERATE_FETCH_GOLD_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_FETCH_GOLD_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_FETCH_GOLD_T.enum_rst.RST_OK)
		{
			GameObject obj= (GameObject)BuildingManager.JinKuang_Map[(int)sub_msg.nAutoID2];
			if(obj!=null)
			{
				obj.SendMessage("GatherGoldRst",sub_msg.nProduction3);
			}
		}
	}
	//金子可以收获（在线时 主动推送 ）   
	void ProcessGoldCanGather(byte[] buff)
	{
		STC_GAMECMD_GOLD_CAN_FETCH_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GOLD_CAN_FETCH_T>(buff);
		GameObject obj= (GameObject)BuildingManager.JinKuang_Map[(int)sub_msg.nAutoID1];
		if(obj!=null)
		{
			obj.SendMessage("ProcessGoldResult",sub_msg.nProduction2);
		}
	}
	//黄金生产加速 返回  
	void ProcessAcceRst(byte[] buff)
	{
		
		STC_GAMECMD_OPERATE_ACCE_GOLD_PRODUCE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_ACCE_GOLD_PRODUCE_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_ACCE_GOLD_PRODUCE_T.enum_rst.RST_OK)
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
			GameObject obj= (GameObject)BuildingManager.JinKuang_Map[(int)sub_msg.nAutoID2];
			if(obj!=null)
			{
				if(sub_msg.nTime3 >0)
				{
					ProduceTimeTeUnit unit = new ProduceTimeTeUnit();
					unit.ProduceState = (uint)ProduceState.PRODUCE;
					unit.time = sub_msg.nTime3;
					//unit.Production = produce_time_array[i].nProduction2;
					obj.SendMessage("ProcessProduceTERst",unit);
				}
				GameObject win = U3dCmn.GetObjFromPrefab("AccelerateWin"); 
				if(win != null)
				{
					win.SendMessage("CompleteAccelerate");
				}
			}
			
		}
		else
		{
			//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.UNKNOWN_ERR)+sub_msg.nRst1);
			GameObject win = U3dCmn.GetObjFromPrefab("AccelerateWin"); 
			if(win != null)
			{
				win.SendMessage("CompleteAccelerate");
			}
		}
	}
}
