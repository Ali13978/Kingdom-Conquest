using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;

public class AllianceManager : MonoBehaviour {
	public UILabel text_resources;
	public static string create_alliance_name = "";
	public static ulong  apply_alliance_id;
	public static string apply_alliance_name;
	//当前联盟信息 
	public static ALLIANCE_INFO my_alliance_info;
	//联盟成员列表 
	public static List<AllianceJoinEvent> AllianceApplyList = new List<AllianceJoinEvent>();
	//加入联盟申请 
	public static List<AllianceMemberUnit> AllianceMemberList = new List<AllianceMemberUnit>();
	//联盟列表 
	//public static Hashtable AllianceMap = new Hashtable();
	public static List<AllianceCard> AllianceList = new List<AllianceCard>();
	public static int alliance_total_num;
	//文字描述资源 
	public static string  leader_str;
	public static string  vice_leader_str;
	public static string  manager_str;
	public static string  member_str;
	//修改的联盟新简介 
	public static string new_announce;
	int refresh_max = 0;
	void Awake()
	{
		string[] labels = text_resources.text.Split(',');
		leader_str = labels[0];
		vice_leader_str = labels[1];
		manager_str = labels[2];
		member_str = labels[3];
	}
	// Use this for initialization
	void Start () {
		ReqAllianceInfo();
		ReqApplyingData();
	}
	

	//根据职位枚举获得联盟职位字符串 
	public static string GetStrFromPositionID(uint position_id)
	{
		string str = "";
		if(position_id == (uint)ALLIANCE_POSITION.alliance_position_leader)
			str = leader_str;
		else if(position_id == (uint)ALLIANCE_POSITION.alliance_position_vice_leader)
			str = vice_leader_str;
		else if(position_id == (uint)ALLIANCE_POSITION.alliance_position_manager)
			str = manager_str;
		else if(position_id == (uint)ALLIANCE_POSITION.alliance_position_member)
			str = member_str;
		return str;
	}
	//请求自身联盟信息 
	public static void ReqAllianceInfo()
	{
		CTS_GAMECMD_GET_ALLIANCE_INFO_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_ALLIANCE_INFO;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_ALLIANCE_INFO_T >(req);
		//LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_ALLIANCE_INFO);
	}
	//请求自身联盟信息返回 
	public void ReqAllianceInfoRst(byte[] buff)
	{
		//print("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
		STC_GAMECMD_GET_ALLIANCE_INFO_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_ALLIANCE_INFO_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_ALLIANCE_INFO_T.enum_rst.RST_OK)
		{
			CommonData.player_online_info.AllianceID = sub_msg.nAllianceID2;
			refresh_max = 0;
			my_alliance_info.AllianceID = sub_msg.nAllianceID2;
			my_alliance_info.AllianceName = DataConvert.BytesToStr(sub_msg.szAllianceName4);
			my_alliance_info.Development = sub_msg.nDevelopment8;
			my_alliance_info.LeaderID = sub_msg.nLeaderID5;
			my_alliance_info.LeaderName = DataConvert.BytesToStr(sub_msg.szLeaderName7);
			my_alliance_info.AllianceLevel = sub_msg.nLevel16;
			my_alliance_info.AllianceRank = sub_msg.nRank12;
			my_alliance_info.MyPost = sub_msg.nMyPosition15;
			my_alliance_info.MemberNum = sub_msg.nMemberNum13;
			my_alliance_info.MemberNumMax = sub_msg.nMemberMaxNum14;
			my_alliance_info.Introduction = DataConvert.BytesToStr(sub_msg.szIntroduction11);
			my_alliance_info.NowContribute = sub_msg.nContribute17;
			my_alliance_info.TotalContribute = sub_msg.nTotalContribute18;
			//更新一下联盟副本中的最大关卡数 
			int lv = (int)my_alliance_info.AllianceLevel;
			if (CommonMB.AllianceInfo_Map.ContainsKey(lv))
			{
				AllianceInfo alliance_info = (AllianceInfo) CommonMB.AllianceInfo_Map[lv];
				GBStatusManager.Instance.MyStatus.MaxLevel = alliance_info.GuildWarMaxLevel;
			}
			else 
			{
				GBStatusManager.Instance.MyStatus.MaxLevel = 0;
			}
			U3dCmn.SendMessage("DashiGuanWin","SetUIHaveAlliance",my_alliance_info);
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_ALLIANCE_INFO_T.enum_rst.RST_LOADING)
		{
			if(refresh_max++ >10)
			{
				return ;
			}
			else 
			{
				//联盟数据还没载入到内存 所以再次请求 这里有可能会造成LOADING死循环 需要计数 
				ReqAllianceInfo();
			}
			
		}
	}
	//获取联盟申请列表  
	public void ReqApplyingData()
	{
		CTS_GAMECMD_GET_ALLIANCE_JOIN_EVENT_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_ALLIANCE_JOIN_EVENT;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_ALLIANCE_JOIN_EVENT_T >(req);
	}
	//获取联盟申请列表返回 
	public void ReqApplyingDataRst(byte[] buff)
	{
		if(AllianceManager.my_alliance_info.MyPost == (uint)ALLIANCE_POSITION.alliance_position_member || AllianceManager.my_alliance_info.MyPost == (uint)ALLIANCE_POSITION.alliance_position_none)
		{
			return;
		}
		STC_GAMECMD_GET_ALLIANCE_JOIN_EVENT_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_ALLIANCE_JOIN_EVENT_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_ALLIANCE_JOIN_EVENT_T.enum_rst.RST_OK)
		{
			AllianceApplyList.Clear();
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_ALLIANCE_JOIN_EVENT_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			AllianceJoinEvent[] join_array = (AllianceJoinEvent[])DataConvert.ByteToStructArray<AllianceJoinEvent>(data_buff,(int)sub_msg.nNum2);
			for(int i=0;i<join_array.Length;i++)
			{
				AllianceApplyList.Add(join_array[i]);
			}
		}
	}
	//获取联盟成员列表 
	public static void ReqAllianceMember()
	{
		CTS_GAMECMD_GET_ALLIANCE_MEMBER_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_ALLIANCE_MEMBER;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_ALLIANCE_MEMBER_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_ALLIANCE_MEMBER);
	}
	//获取联盟成员列表返回  
	public void ReqAllianceMemberRst(byte[] buff)
	{
		STC_GAMECMD_GET_ALLIANCE_MEMBER_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_ALLIANCE_MEMBER_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_ALLIANCE_MEMBER_T.enum_rst.RST_OK)
		{
			AllianceMemberList.Clear();
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_ALLIANCE_MEMBER_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			AllianceMemberUnit[] member_array = (AllianceMemberUnit[])DataConvert.ByteToStructArray<AllianceMemberUnit>(data_buff,(int)sub_msg.nNum2);
	
			int myLeaders = 0;
			int myElders = 0;
			
			// 统计当前副盟主和管事的数量 ...
			for(int i=0;i<member_array.Length;i++)
			{
				AllianceMemberUnit card = member_array[i];
				if (card.nPosition4 == (int) ALLIANCE_POSITION.alliance_position_vice_leader)
				{
					myLeaders ++;
				}
				else if (card.nPosition4 == (int) ALLIANCE_POSITION.alliance_position_manager)
				{
					myElders ++;
				}
			}
			
			my_alliance_info.LeaderNum = myLeaders;
			my_alliance_info.ManagerNum = myElders;
			
			SortDataByLevel(ref AllianceMemberList,member_array);			
			SortDataByPosition(ref AllianceMemberList);
			U3dCmn.SendMessage("AllianceWin","InitialPanelData",null);
		}
	}
	//按照等级排序 
	void SortDataByLevel(ref List<AllianceMemberUnit> AllianceMemberList,AllianceMemberUnit[] member_array)
	{
		//Hashtable member_map = new Hashtable();
		for(int i=0;i<member_array.Length;i++)
		{
			int max = i;
			for(int j = i+1;j<member_array.Length;j++)
			{
				if(member_array[j].nLevel7>member_array[max].nLevel7)
				{
					max = j;
				}
				
			}
			AllianceMemberList.Add(member_array[max]);
			member_array[max] = member_array[i];
			//AllianceMemberList.Add(member_array[max]);
		}
	}
	//按照职位排序  
	void SortDataByPosition(ref List<AllianceMemberUnit> AllianceMemberList)
	{
	
		for(int i=AllianceMemberList.Count-1,n=AllianceMemberList.Count-1;i>=0;i--)
		{
			if(AllianceMemberList[n].nPosition4 == (uint)ALLIANCE_POSITION.alliance_position_manager)
			{
				AllianceMemberUnit unit = AllianceMemberList[n];
				AllianceMemberList.Remove(unit);
				AllianceMemberList.Insert(0,unit);
			}
			else
			{
				n--;
			}
		}
		
		for(int i=AllianceMemberList.Count-1,n=AllianceMemberList.Count-1;i>=0;i--)
		{
			
			if(AllianceMemberList[n].nPosition4 == (uint)ALLIANCE_POSITION.alliance_position_vice_leader)
			{
				AllianceMemberUnit unit = AllianceMemberList[n];
				AllianceMemberList.Remove(unit);
				AllianceMemberList.Insert(0,unit);
			}
			else
			{
				n--;
			}
		}
		for(int i=AllianceMemberList.Count-1,n=AllianceMemberList.Count-1;i>=0;i--)
		{
			if(AllianceMemberList[n].nPosition4 == (uint)ALLIANCE_POSITION.alliance_position_leader)
			{
				AllianceMemberUnit unit = AllianceMemberList[n];
				AllianceMemberList.Remove(unit);
				AllianceMemberList.Insert(0,unit);
			}
			else
			{
				n--;
			}
		}
	}
	//请求是否处于申请状态 
	public static void ReqApplyingState()
	{
		CTS_GAMECMD_GET_MY_ALLIANCE_JOIN_EVENT_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_MY_ALLIANCE_JOIN_EVENT;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_MY_ALLIANCE_JOIN_EVENT_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_MY_ALLIANCE_JOIN_EVENT);
	}
	//请求是否处于申请状态返回 
	public void ReqApplyingStateRst(byte[] buff)
	{
		STC_GAMECMD_GET_MY_ALLIANCE_JOIN_EVENT_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_MY_ALLIANCE_JOIN_EVENT_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_MY_ALLIANCE_JOIN_EVENT_T.enum_rst.RST_OK)
		{
			apply_alliance_id 	= sub_msg.nAllianceID2;
			apply_alliance_name = DataConvert.BytesToStr(sub_msg.szAllianceName4);
			U3dCmn.SendMessage("DashiGuanWin","SetUIApplying",null);
		}
		else
		{
			U3dCmn.SendMessage("DashiGuanWin","SetUINoAlliance",null);
		}
	}
	
	//请求创建联盟 
	public static void ReqCreateAlliance(string alliance_name)
	{
		CTS_GAMECMD_OPERATE_CREATE_ALLI_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_CREATE_ALLI;
		req.szAllianceName4 = DataConvert.StrToBytes(alliance_name);
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_CREATE_ALLI_T >(req);
		create_alliance_name = alliance_name;
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CREATE_ALLI);
	}
	//请求创建联盟返回  
	public void ReqCreateAllianceRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_CREATE_ALLI_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_CREATE_ALLI_T>(buff);
		//print ("ooooooooo"+sub_msg.nRst1);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_CREATE_ALLI_T.enum_rst.RST_OK)
		{
			CommonData.player_online_info.Gold -= sub_msg.nGold3;
			PlayerInfoManager.RefreshPlayerDataUI();
			ReqAllianceInfo();
		
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_CREATE_ALLI_T.enum_rst.RST_NAME_DUP)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALLIANCE_NAME_DUPLICATE));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_CREATE_ALLI_T.enum_rst.RST_GOLD)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.MONEY_NOT_ENOUGH));
		}	
		else 
		{
			//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.UNKNOWN_ERR)+sub_msg.nRst1);
		}
	}
	//申请加入联盟(通过名字)    
	public static void ReqJoinAlliance(byte[] alliance_name)
	{
		CTS_GAMECMD_OPERATE_JOIN_ALLI_NAME_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_JOIN_ALLI_NAME;
		req.szAllianceName4 = alliance_name;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_JOIN_ALLI_NAME_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_JOIN_ALLI_NAME);
	}
	//申请加入联盟返回(通过名字)  
	public void ReqJoinAllianceRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_JOIN_ALLI_NAME_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_JOIN_ALLI_NAME_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_JOIN_ALLI_NAME_T.enum_rst.RST_OK)
		{
			apply_alliance_id 	= sub_msg.nAllianceID2;
			apply_alliance_name = DataConvert.BytesToStr(sub_msg.szAllianceName4);
			U3dCmn.SendMessage("DashiGuanWin","SetUIApplying",null);
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_JOIN_ALLI_NAME_T.enum_rst.RST_ALLI_FULL)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALLIANCE_MEMBER_FULL));	
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_JOIN_ALLI_NAME_T.enum_rst.RST_NO_ALLI)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALLIANCE_IS_NO_EXIST));
		}
	}
	//申请加入联盟(通过联盟ID)     
	public static void ReqJoinAllianceByID(ulong alliance_id)
	{
		CTS_GAMECMD_OPERATE_JOIN_ALLI_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_JOIN_ALLI;
		req.nAllianceID3 = alliance_id;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_JOIN_ALLI_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_JOIN_ALLI);
	}
	//申请加入联盟返回(通过联盟ID)  
	public void ReqJoinAllianceRstByIDRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_JOIN_ALLI_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_JOIN_ALLI_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_JOIN_ALLI_T.enum_rst.RST_OK)
		{
			apply_alliance_id 	= sub_msg.nAllianceID2;
			//apply_alliance_name = DataConvert.BytesToStr(sub_msg.szAllianceName4);
			U3dCmn.SendMessage("DashiGuanWin","SetUIApplying",null);
			U3dCmn.SendMessage("AllianceInfoWin","DismissPanel",null);
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_JOIN_ALLI_T.enum_rst.RST_ALLI_FULL)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALLIANCE_MEMBER_FULL));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_JOIN_ALLI_T.enum_rst.RST_NO_ALLI)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALLIANCE_IS_NO_EXIST));
		}
	}
	//撤销联盟申请 
	public static void  ReqRecallApply()
	{
		CTS_GAMECMD_OPERATE_CANCEL_JOIN_ALLI_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_CANCEL_JOIN_ALLI;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_CANCEL_JOIN_ALLI_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CANCEL_JOIN_ALLI);
	}
	//撤销联盟申请返回  
	public void  ReqRecallApplyRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_CANCEL_JOIN_ALLI_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_CANCEL_JOIN_ALLI_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_CANCEL_JOIN_ALLI_T.enum_rst.RST_OK)
		{
			apply_alliance_id 	= 0;
			apply_alliance_name = "";
			U3dCmn.SendMessage("DashiGuanWin","SetUINoAlliance",null);
		}
	}
	//解散联盟 
	public static void ReqDismissAlliance()
	{
		CTS_GAMECMD_OPERATE_DISMISS_ALLI_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_DISMISS_ALLI;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_DISMISS_ALLI_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_DISMISS_ALLI);
	}
	//解散联盟返回 
	public void ReqDismissAllianceRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_DISMISS_ALLI_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_DISMISS_ALLI_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_DISMISS_ALLI_T.enum_rst.RST_OK)
		{
			CommonData.player_online_info.AllianceID = 0;
			CommonData.player_online_info.AllianceName ="";
			U3dCmn.SendMessage("DashiGuanWin","SetUINoAlliance",null);
		}
	}
	//退出联盟 
	public static void ReqQuitAlliance()
	{
		CTS_GAMECMD_OPERATE_EXIT_ALLI_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_EXIT_ALLI;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_EXIT_ALLI_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_EXIT_ALLI);
	}
	//退出联盟返回 
	public void ReqQuitAllianceRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_EXIT_ALLI_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_EXIT_ALLI_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_EXIT_ALLI_T.enum_rst.RST_OK)
		{
			CommonData.player_online_info.AllianceID = 0;
			CommonData.player_online_info.AllianceName ="";
			U3dCmn.SendMessage("DashiGuanWin","SetUINoAlliance",null);
		}
	}
	//修改联盟简介 
	public static void ReqModifyAnnounce(string content)
	{
		CTS_GAMECMD_OPERATE_SET_ALLI_INTRO_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_SET_ALLI_INTRO;
		req.szIntroduction4 = DataConvert.StrToBytes(content);
		req.nLen3 = req.szIntroduction4.Length;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_SET_ALLI_INTRO_T >(req);
		new_announce = content;
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_SET_ALLI_INTRO);
	}
	//修改联盟简介返回  
	public void ReqModifyAnnounceRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_SET_ALLI_INTRO_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_SET_ALLI_INTRO_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_SET_ALLI_INTRO_T.enum_rst.RST_OK)
		{
			my_alliance_info.Introduction = new_announce;
			U3dCmn.SendMessage("AllianceWin","InitialPanelData",null);
			U3dCmn.SendMessage("AllianceAnnounceWin","DismissPanel",null);
		}
	}
	//开除联盟成员  
	public static void FireAllianceMember(ulong member_id)
	{
		CTS_GAMECMD_OPERATE_EXPEL_ALLI_MEMBER_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_EXPEL_ALLI_MEMBER;
		req.nMemberID3 = member_id;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_EXPEL_ALLI_MEMBER_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_EXPEL_ALLI_MEMBER);
	}
	//开除联盟成员返回   
	public void FireAllianceMemberRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_EXPEL_ALLI_MEMBER_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_EXPEL_ALLI_MEMBER_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_EXPEL_ALLI_MEMBER_T.enum_rst.RST_OK)
		{
			DeleteMemberData(sub_msg.nMemberID2);
			U3dCmn.SendMessage("AllianceWin","InitialPanelData",null);
			U3dCmn.SendMessage("AllianceMemberInfoWin","DismissPanel",null);
			
		}
		
	}
	//被开除联盟通知 
	public void BeFiredAllianceRst(byte[] buff)
	{
		CommonData.player_online_info.AllianceID = 0;
		CommonData.player_online_info.AllianceName = "";
	}
	//从成员列表中删除成员数据 
	void DeleteMemberData(ulong account_id)
	{
		AllianceMemberUnit unit = new AllianceMemberUnit();
		for(int i=0;i<AllianceManager.AllianceMemberList.Count;i++)
		{
			if(AllianceManager.AllianceMemberList[i].nAccountID1 == account_id)
			{	
				unit = AllianceManager.AllianceMemberList[i];
				break;
			}
				
		}
		AllianceManager.AllianceMemberList.Remove(unit);
	}
	//授权职位 
	public static void ReqAuthorizeMember(ulong member_id,uint position)
	{
		CTS_GAMECMD_OPERATE_SET_ALLI_POSITION_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_SET_ALLI_POSITION;
		req.nMemberID3 = member_id;
		req.nPosition4 = position;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_SET_ALLI_POSITION_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_SET_ALLI_POSITION);
	}
	//授权职位返回    
	public void ReqAuthorizeMemberRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_SET_ALLI_POSITION_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_SET_ALLI_POSITION_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_SET_ALLI_POSITION_T.enum_rst.RST_OK)
		{
			for(int i=0;i<AllianceManager.AllianceMemberList.Count;i++)
			{
				if(AllianceManager.AllianceMemberList[i].nAccountID1 == sub_msg.nMemberID3)
				{	
					AllianceMemberUnit card = AllianceManager.AllianceMemberList[i];
					
					// 对比以前的职位做出相应数量加减 ...
					if (card.nPosition4 == (uint) ALLIANCE_POSITION.alliance_position_vice_leader)
					{
						if (sub_msg.nPosition4 == (uint) ALLIANCE_POSITION.alliance_position_manager)
						{
							AllianceManager.my_alliance_info.ManagerNum ++;
							AllianceManager.my_alliance_info.LeaderNum --;
						}
						else if (sub_msg.nPosition4 == (uint) ALLIANCE_POSITION.alliance_position_member)
						{
							AllianceManager.my_alliance_info.LeaderNum --;
						}
					}
					else if (card.nPosition4 == (uint) ALLIANCE_POSITION.alliance_position_manager )
					{
						if (sub_msg.nPosition4 == (uint) ALLIANCE_POSITION.alliance_position_vice_leader)
						{
							AllianceManager.my_alliance_info.ManagerNum --;
							AllianceManager.my_alliance_info.LeaderNum ++;
						}
						else if (sub_msg.nPosition4 == (uint) ALLIANCE_POSITION.alliance_position_member)
						{
							AllianceManager.my_alliance_info.ManagerNum --;
						}
					}
					else if (card.nPosition4 == (uint) ALLIANCE_POSITION.alliance_position_member)
					{
						if (sub_msg.nPosition4 == (uint) ALLIANCE_POSITION.alliance_position_vice_leader)
						{
							AllianceManager.my_alliance_info.LeaderNum ++;
						}
						else if (sub_msg.nPosition4 == (uint) ALLIANCE_POSITION.alliance_position_manager)
						{
							AllianceManager.my_alliance_info.ManagerNum ++;
						}
					}
					// 更新为新的职位 ...
					card.nPosition4 = sub_msg.nPosition4;
					AllianceManager.AllianceMemberList[i] = card;
					break;
				}
			}
			U3dCmn.SendMessage("AllianceWin","InitialPanelData",null);
			U3dCmn.SendMessage("AllianceMemberInfoWin","RefreshPosition",sub_msg.nPosition4);
			U3dCmn.SendMessage("AuthorizeWin","DismissPanel",null);
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_SET_ALLI_POSITION_T.enum_rst.RST_FULL)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.POSITION_IS_EXIST));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_SET_ALLI_POSITION_T.enum_rst.RST_NO_CHG)
		{
			U3dCmn.SendMessage("AuthorizeWin","DismissPanel",null);
		}
	}
	//禅让盟主 
	public static void ReqDemiseLeader(string member_name)
	{
		CTS_GAMECMD_OPERATE_ABDICATE_ALLI_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_ABDICATE_ALLI;
		byte[] buff = new byte[32];
		byte[] szname = DataConvert.StrToBytes(member_name);
		Array.Copy(szname,0,buff,0,szname.Length);
		req.szMemberName4 = buff;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_ABDICATE_ALLI_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ABDICATE_ALLI);
	}
	//禅让盟主返回  
	public void ReqDemiseLeader(byte[] buff)
	{
		STC_GAMECMD_OPERATE_ABDICATE_ALLI_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_ABDICATE_ALLI_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_ABDICATE_ALLI_T.enum_rst.RST_OK)
		{
			U3dCmn.SendMessage("DashiGuanWin","InitialData",null);
			U3dCmn.SendMessage("DemiseWin","DismissPanel",null);
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_ABDICATE_ALLI_T.enum_rst.RST_MEMBER_NOT_IN)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.THIS_ACCOUNT_NOTIN_ALLIANCE));
		}
	}
	//请求联盟列表 
	public static void ReqAllianceList(int from_rank,int end_rank)
	{
		CTS_GAMECMD_GET_ALLIANCE_CARD_BY_RANK_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_ALLIANCE_CARD_BY_RANK;
		req.nFromRank3 = (uint)from_rank;
		req.nToRank4 = (uint)end_rank;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_ALLIANCE_CARD_BY_RANK_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_ALLIANCE_CARD_BY_RANK);
	}
	//请求联盟列表返回  
	public void ReqAllianceListRst(byte[] buff)
	{
		//print("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
		STC_GAMECMD_GET_ALLIANCE_CARD_BY_RANK_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_ALLIANCE_CARD_BY_RANK_T>(buff);
		alliance_total_num = (int)sub_msg.nTotalNum1;
		int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_ALLIANCE_CARD_BY_RANK_T>();
		int data_len = buff.Length - head_len;
		byte[] data_buff = new byte[data_len];
		Array.Copy(buff,head_len,data_buff,0,data_len);
		AllianceCard[] alliance_array = (AllianceCard[])DataConvert.ByteToStructArray<AllianceCard>(data_buff,(int)sub_msg.nNum2);
		AllianceList.Clear();
		for(int i=0;i<alliance_array.Length;i++)
		{
			
			AllianceList.Add(alliance_array[i]);
		}
		U3dCmn.SendMessage("AllianceListWin","InitialPanelData",null);
	}
	//被允许加入联盟 
	void BeAgreedJoinApply(byte[] buff)
	{
		STC_GAMECMD_IN_ALLIANCE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_IN_ALLIANCE_T>(buff);
		CommonData.player_online_info.AllianceID = sub_msg.nAllianceID1;
		
	}
	//同意玩家加入联盟返回   
	void AgreeJoinApplyRst(byte[] buff)
	{
		//print ("ttttttttttttttttttttttttttttttttttt");
		STC_GAMECMD_OPERATE_ACCEPT_ALLI_MEMBER_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_ACCEPT_ALLI_MEMBER_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_ACCEPT_ALLI_MEMBER_T.enum_rst.RST_MEMBER_FULL)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALLIANCE_MEMBER_FULL));
			return ;
		}
		//{
		DeleteApplyData(sub_msg.nMemberID2);
		U3dCmn.SendMessage("AllianceWin","InitialPanelData",null);
		//	
		//}
	}
	//拒绝玩家加入联盟返回 
	void RefuseJoinApplyRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_REFUSE_JOIN_ALLI_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_REFUSE_JOIN_ALLI_T>(buff);
		//if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_REFUSE_JOIN_ALLI_T.enum_rst.RST_OK)
		//{
		DeleteApplyData(sub_msg.nApplicantID2);
		U3dCmn.SendMessage("AllianceWin","InitialPanelData",null);
			
		//}
	}
	//从申请列表中删除玩家数据 
	void DeleteApplyData(ulong account_id)
	{
		AllianceJoinEvent unit = new AllianceJoinEvent();
		for(int i=0;i<AllianceApplyList.Count;i++)
		{
			if(AllianceApplyList[i].nAccountID1 == account_id)
			{	
				unit = AllianceApplyList[i];
				break;
			}
		}
		AllianceApplyList.Remove(unit);
	}
}
