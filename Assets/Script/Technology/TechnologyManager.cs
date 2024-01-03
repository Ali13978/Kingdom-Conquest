using UnityEngine;
using System;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class TechnologyManager : MonoBehaviour {
	//正在研究的科技列表 
	public static Hashtable ResearchingTechMap = new Hashtable();
	// Use this for initialization 
	void Start () {
		GetTechInfo();
		GetResearchingTech();
	}
	
	//获取科技信息列表（目前都是士兵等级科技）   
	
	public static void GetTechInfo()
	{
		CTS_GAMECMD_GET_TECHNOLOGY_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_TECHNOLOGY;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_TECHNOLOGY_T >(req);
	}
	void ProcessTechInfo(byte[] buff)
	{
		STC_GAMECMD_GET_TECHNOLOGY_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_TECHNOLOGY_T>(buff);
		if(sub_msg.nRst1 == (byte)STC_GAMECMD_GET_TECHNOLOGY_T.enum_rst.RST_OK)
		{
			SoldierManager.SoldierTopMap.Clear();
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_TECHNOLOGY_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			TechnologyUnit[] tech_list = (TechnologyUnit[])DataConvert.ByteToStructArray<TechnologyUnit>(data_buff,sub_msg.nNum2);
			for(int i=0;i<tech_list.Length;i++)
			{
				if(tech_list[i].nExcelID1>=(int)SoldierSort.QIBING && tech_list[i].nExcelID1<=(int)SoldierSort.QIXIE)
				{
					SoldierUnit unit;
					unit.nExcelID1 = tech_list[i].nExcelID1;
					unit.nLevel2 =tech_list[i].nLevel2;
					unit.nNum3=0;
					SoldierManager.SoldierTopMap.Add(unit.nExcelID1+"_"+unit.nLevel2,unit);
				}
			}
		}
	}
	//获取研究科技时间队列  
	public static void  GetResearchingTech()
	{
		CTS_GAMECMD_GET_RESEARCH_TE_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_RESEARCH_TE;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_RESEARCH_TE_T >(req);
	}
	//获取研究科技时间队列返回  
	public void GetResearchingTechRst(byte[] buff)
	{
		STC_GAMECMD_GET_RESEARCH_TE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_RESEARCH_TE_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_RESEARCH_TE_T.enum_rst.RST_OK)
		{
			ResearchingTechMap.Clear();
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_RESEARCH_TE_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			ResearchTimeEvent[] research_array = (ResearchTimeEvent[])DataConvert.ByteToStructArray<ResearchTimeEvent>(data_buff,(int)sub_msg.nNum3);
			for(int i=0;i<research_array.Length;i++)
			{
				research_array[i].nEndTime3 = (uint)(research_array[i].nEndTime3-sub_msg.nTimeNow2 +DataConvert.DateTimeToInt(DateTime.Now));
				ResearchingTechMap.Add(research_array[i].nExcelID1,research_array[i]);
			}
		}
	}
	//研究科技  
	public static void  ReqResearchTech(uint excel_id)
	{
		CTS_GAMECMD_OPERATE_RESEARCH_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_RESEARCH;
		req.nExcelID3  = excel_id;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_RESEARCH_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_RESEARCH);
	}
	//研究科技返回  
	public void  ReqResearchTechRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_RESEARCH_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_RESEARCH_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_RESEARCH_T.enum_rst.RST_OK)
		{
			ResearchTimeEvent time_event;
			time_event.nExcelID1 = sub_msg.nExcelID5;
			time_event.nBeginTime2 = (uint)DataConvert.DateTimeToInt(DateTime.Now);
			time_event.nEndTime3   = (uint)(sub_msg.nTime3+DataConvert.DateTimeToInt(DateTime.Now));
			CommonData.player_online_info.Gold -= sub_msg.nGold4;
			PlayerInfoManager.RefreshPlayerDataUI();
			ResearchingTechMap.Add(sub_msg.nExcelID5,time_event);
			U3dCmn.SendMessage("TechnologyWin","InitialData",null);
			U3dCmn.SendMessage("ResearchSoldierWin","DismissPanel",null);
			
			// <新手引导> 请求升级兵种科技返回 ...
			if (NewbieYanJiuYuan.processReqResearchEndRst != null)
			{
				NewbieYanJiuYuan.processReqResearchEndRst();
				NewbieYanJiuYuan.processReqResearchEndRst = null;
			}
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_RESEARCH_T.enum_rst.RST_ALREADY_RESEARCH)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HAS_RESEARCH_TECH));
			U3dCmn.SendMessage("ResearchSoldierWin","DismissPanel",null);
		}
		else
		{
			//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.UNKNOWN_ERR)+sub_msg.nRst1);

			// <新手引导> 关闭 ....
			NewbieYanJiuYuan.processReqResearchEndRst = null;
		}
		
		
	}
	//加速科技研究返回 
	public void AccelerateTechRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_ACCE_RESEARCH_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_ACCE_RESEARCH_T>(buff);
	
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_ACCE_RESEARCH_T.enum_rst.RST_OK)
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
			ResearchTimeEvent time_event = (ResearchTimeEvent)ResearchingTechMap[sub_msg.nExcelID6];
			time_event.nEndTime3 =  (uint)(DataConvert.DateTimeToInt(DateTime.Now) + sub_msg.nTime3);
			ResearchingTechMap[sub_msg.nExcelID6] = time_event;
			U3dCmn.SendMessage("TechnologyWin","InitialData",null);
		
			GameObject win = U3dCmn.GetObjFromPrefab("AccelerateWin"); 
			if(win != null)
			{
				win.SendMessage("CompleteAccelerate");
			}
		}
	}
	//科技研究完毕 
	public void ResearchEnd(byte[] buff)
	{
		STC_GAMECMD_RESEARCH_TE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_RESEARCH_TE_T>(buff);
		ResearchingTechMap.Remove(sub_msg.nExcelID1);
			
		SoldierManager.SoldierTopMap.Remove(sub_msg.nExcelID1+"_"+(sub_msg.nLevel2-1));
		if(!SoldierManager.SoldierTopMap.Contains(sub_msg.nExcelID1+"_"+sub_msg.nLevel2))
		{
			SoldierUnit unit;
			unit.nExcelID1 = sub_msg.nExcelID1;
			unit.nLevel2 = sub_msg.nLevel2;
			unit.nNum3 = 0;
			SoldierManager.SoldierTopMap.Add(sub_msg.nExcelID1+"_"+sub_msg.nLevel2,unit);
		}
		
		U3dCmn.SendMessage("TechnologyWin","InitialData",null);
		
		// 新手引导的研究完毕回调 ...
		if (NewbieYanJiuYuan.processNewbieResearchEnd != null) {
			NewbieYanJiuYuan.processNewbieResearchEnd();
			NewbieYanJiuYuan.processNewbieResearchEnd = null;
		}
	}
}
