using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class ChatManager : MonoBehaviour {
	//聊天ITEM数量 
	public static  int chat_item_max = 20;
	public static Queue<CHAT_DATA>AllChatDataList = new Queue<CHAT_DATA>();
	public static Queue<CHAT_DATA>WorldChatDataList = new Queue<CHAT_DATA>();
	public static Queue<CHAT_DATA>AllianceChatDataList = new Queue<CHAT_DATA>();
	public static Queue<CHAT_DATA>TeamChatDataList = new Queue<CHAT_DATA>();
	public static Queue<CHAT_DATA>PrivateChatDataList = new Queue<CHAT_DATA>();
	public ChatWin chat_win;
	public UILabel chat_smp_label;
	//当前频道 
	public static int chat_channel =0;
	//当前私聊对象CHAR_ID  和角色名字 
	public static ulong to_char_id = 0;
	public static string to_char_name = "";
	//排序用的哈希表 
	Hashtable sort_map = new Hashtable();
	// Use this for initialization
	void Start () {
		ReqRecentChatData();
		//注册副本战报回调 
		BaizInstanceManager.processChatMsgAddLootItemDelegate -= BattlefieldReport;
		BaizInstanceManager.processChatMsgAddLootItemDelegate += BattlefieldReport;
		CombatManager.processChatMsgCombatRstDelegate -= BattlefieldReport;
		CombatManager.processChatMsgCombatRstDelegate += BattlefieldReport;
	}
	
	void OpenChatWin()
	{
		
		chat_win.gameObject.SetActiveRecursively(true);
		chat_win.SendMessage("RevealPanel");
		
	}
	//请求最近的聊天记录（玩家登陆时请求）
	void ReqRecentChatData()
	{
		CTS_GAMECMD_CHAT_GET_RECENT_CHAT_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_CHAT_GET_RECENT_CHAT;
		TcpMsger.SendLogicData<CTS_GAMECMD_CHAT_GET_RECENT_CHAT_T>(req);
	}
	//发送世界聊天信息 
	public static void SendWorldData(string chat_data)
	{
		CTS_GAMECMD_CHAT_WORLD_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_CHAT_WORLD;
	
		req.szText4 = DataConvert.StrToBytes(chat_data);
		req.nTextLen3 = req.szText4.Length;
		TcpMsger.SendLogicData<CTS_GAMECMD_CHAT_WORLD_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_CHAT_WORLD);
	}
	//发送联盟聊天信息  
	public static void SendAlliancedData(string chat_data)
	{
		CTS_GAMECMD_CHAT_ALLIANCE_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_CHAT_ALLIANCE;;
	
		req.szText4 = DataConvert.StrToBytes(chat_data);
		req.nTextLen3 = req.szText4.Length;
		TcpMsger.SendLogicData<CTS_GAMECMD_CHAT_ALLIANCE_T>(req);
		//LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_CHAT_ALLIANCE);
	}
	//发送队伍聊天信息  
	public static void SendTeamData(string chat_data)
	{
		CTS_GAMECMD_CHAT_GROUP_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_CHAT_GROUP;;
	
		req.szText4 = DataConvert.StrToBytes(chat_data);
		req.nTextLen3 = req.szText4.Length;
		TcpMsger.SendLogicData<CTS_GAMECMD_CHAT_GROUP_T>(req);
		//LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_CHAT_GROUP);
	}
	//发送私聊信息 
	public static void SendPrivateData(string chat_data)
	{
		//print ("ssssssssssssssssss");
		CTS_GAMECMD_CHAT_PRIVATE_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_CHAT_PRIVATE;
		req.nPeerAccountID3 = to_char_id;
		req.szText5 = DataConvert.StrToBytes(chat_data);
		req.nTextLen4 = req.szText5.Length;
		TcpMsger.SendLogicData<CTS_GAMECMD_CHAT_PRIVATE_T>(req);
		//LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_CHAT_PRIVATE);
	}
	//处理联盟聊天信息 
	void ProcessAllanceData(byte[] buff)
	{
		
		STC_GAMECMD_CHAT_ALLIANCE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_CHAT_ALLIANCE_T>(buff);
		if(!CheckDataExist(AllianceChatDataList,sub_msg.nChatID1))
		{
			CHAT_DATA chat_unit =  new CHAT_DATA();
			chat_unit.ChatId = sub_msg.nChatID1;
			chat_unit.ChatType = (int)GAME_SUB_RST.STC_GAMECMD_CHAT_ALLIANCE;
			chat_unit.nCharID = sub_msg.nCharID4;
			chat_unit.ChatTime = sub_msg.nTime5;
			chat_unit.szFrom = DataConvert.BytesToStr(sub_msg.szFrom3);
			chat_unit.Vip	= sub_msg.nVip6;
			chat_unit.szText = DataConvert.BytesToStr(sub_msg.szText8);
			
			AddChatContent(chat_unit);
		}
	
	}
	//处理世界聊天信息 
	void ProcessWorldData(byte[] buff)
	{
		STC_GAMECMD_CHAT_WORLD_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_CHAT_WORLD_T>(buff);
		if(!CheckDataExist(WorldChatDataList,sub_msg.nChatID1))
		{
			CHAT_DATA chat_unit =  new CHAT_DATA();
			chat_unit.ChatId = sub_msg.nChatID1;
			chat_unit.ChatType = (int)GAME_SUB_RST.STC_GAMECMD_CHAT_WORLD;
			chat_unit.nCharID = sub_msg.nCharID4;
			chat_unit.ChatTime = sub_msg.nTime5;
			chat_unit.szFrom = DataConvert.BytesToStr(sub_msg.szFrom3);
			chat_unit.Vip	= sub_msg.nVip6;
			chat_unit.szText = DataConvert.BytesToStr(sub_msg.szText8);
			AddChatContent(chat_unit);
		}
	}
	//处理私聊信息 
	void ProcessPrivateData(byte[] buff)
	{
		STC_GAMECMD_CHAT_PRIVATE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_CHAT_PRIVATE_T>(buff);
		if(!CheckDataExist(PrivateChatDataList,sub_msg.nChatID1))
		{	
			CHAT_DATA chat_unit =  new CHAT_DATA();
			chat_unit.ChatId = sub_msg.nChatID1;
			chat_unit.ChatType = (int)GAME_SUB_RST.STC_GAMECMD_CHAT_PRIVATE;
			chat_unit.nCharID = sub_msg.nFromAccountID4;
			chat_unit.ChatTime = sub_msg.nTime8;
			chat_unit.szFrom = DataConvert.BytesToStr(sub_msg.szFrom3);
			chat_unit.szTo = DataConvert.BytesToStr(sub_msg.szTo6);
			chat_unit.Vip = sub_msg.nVip9;
			chat_unit.szText = DataConvert.BytesToStr(sub_msg.szText11);
			AddChatContent(chat_unit);
		}
	}
	//处理队伍聊天信息 
	void ProcessTeamData(byte[] buff)
	{
		STC_GAMECMD_CHAT_GROUP_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_CHAT_GROUP_T>(buff);
		if(!CheckDataExist(TeamChatDataList,sub_msg.nChatID1))
		{	
			CHAT_DATA chat_unit =  new CHAT_DATA();
			chat_unit.ChatId = sub_msg.nChatID1;
			chat_unit.ChatType = (int)GAME_SUB_RST.STC_GAMECMD_CHAT_GROUP;
			chat_unit.nCharID = sub_msg.nCharID4;
			chat_unit.ChatTime = sub_msg.nTime5;
			chat_unit.szFrom = DataConvert.BytesToStr(sub_msg.szFrom3);
			chat_unit.Vip	= sub_msg.nVip6;
			chat_unit.szText = DataConvert.BytesToStr(sub_msg.szText8);
			AddChatContent(chat_unit);
		}
	}
	//处理系统推送信息 
	void ProcessSysData(byte[] buff)
	{
		
		STC_GAMECMD_CHAT_WORLD_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_CHAT_WORLD_T>(buff);
		
	//	print("ssssssssssssssssssssssssssssssssss"+(int)iPhone.generation);
		if(!CheckDataExist(AllChatDataList,sub_msg.nChatID1))
		{
			CHAT_DATA chat_unit =  new CHAT_DATA();
			chat_unit.ChatId = sub_msg.nChatID1;
			chat_unit.nCharID = sub_msg.nCharID4;
			chat_unit.ChatType = (int)GAME_SUB_RST.STC_GAMECMD_SYS_REPORT;
			chat_unit.ChatTime = sub_msg.nTime5;
			chat_unit.szFrom = "";
			chat_unit.szText = DataConvert.BytesToStr(sub_msg.szText8);
			AddChatContent(chat_unit);
			
		}
	}
	//保存聊天信息 并发往UI显示 
	void AddChatContent(CHAT_DATA chat_unit)
	{
		
		if(chat_unit.ChatType  == (uint)GAME_SUB_RST.STC_GAMECMD_CHAT_WORLD)
		{
			InsertData(ref WorldChatDataList,chat_unit);
			InsertData(ref AllChatDataList,chat_unit);
			/*if(WorldChatDataList.Count >= chat_item_max)
			{
				WorldChatDataList.Dequeue();
				WorldChatDataList.Enqueue(chat_unit);
			}
			else
			{
				WorldChatDataList.Enqueue(chat_unit);
			}
			SortData(ref WorldChatDataList);*/
		}
		else if(chat_unit.ChatType ==(uint)GAME_SUB_RST.STC_GAMECMD_CHAT_ALLIANCE)
		{
			InsertData(ref AllianceChatDataList,chat_unit);
			InsertData(ref AllChatDataList,chat_unit);
			/*if(AllianceChatDataList.Count >= chat_item_max)
			{
				AllianceChatDataList.Dequeue();
				AllianceChatDataList.Enqueue(chat_unit);
			}
			else
			{
				AllianceChatDataList.Enqueue(chat_unit);
			}
			SortData(ref AllianceChatDataList);*/
		}
		else if(chat_unit.ChatType ==(uint)GAME_SUB_RST.STC_GAMECMD_CHAT_GROUP)
		{
			InsertData(ref TeamChatDataList,chat_unit);
			InsertData(ref AllChatDataList,chat_unit);
		}
		else if(chat_unit.ChatType ==(uint)GAME_SUB_RST.STC_GAMECMD_CHAT_PRIVATE)
		{
			InsertData(ref PrivateChatDataList,chat_unit);
			InsertData(ref AllChatDataList,chat_unit);
			/*if(PrivateChatDataList.Count >= chat_item_max)
			{
				PrivateChatDataList.Dequeue();
				PrivateChatDataList.Enqueue(chat_unit);
			}
			else
			{
				PrivateChatDataList.Enqueue(chat_unit);
			}
			SortData(ref PrivateChatDataList);*/
		}
		else if(chat_unit.ChatType ==(uint)GAME_SUB_RST.STC_GAMECMD_SYS_REPORT)
		{
			InsertData(ref AllChatDataList,chat_unit);
		
		}
		/*if(AllChatDataList.Count >= chat_item_max)
		{
			AllChatDataList.Dequeue();
			AllChatDataList.Enqueue(chat_unit);
		}
		else
		{
			AllChatDataList.Enqueue(chat_unit);
		}
		SortData(ref AllChatDataList);*/
		
		//chat_win.RefreshUI();
		RefreshChatSmpUI();
	}
	void InsertData(ref Queue<CHAT_DATA>data_queue,CHAT_DATA data)
	{
		if(data_queue.Count == 0)
		{
			data_queue.Enqueue(data);
			return ;
		}
		CHAT_DATA[] data_array =data_queue.ToArray();
		data_queue.Clear();
		bool insert = false;

		for(int i=0;i<data_array.Length;i++)
		{
			if(!insert)
			{
				if(data_array[i].ChatTime<=data.ChatTime)
				{
					data_queue.Enqueue(data_array[i]);
				}
				else
				{
					data_queue.Enqueue(data);
					data_queue.Enqueue(data_array[i]);
					insert= true;
				}
			}
			else 
				data_queue.Enqueue(data_array[i]);

		
		}
		if(!insert)
				data_queue.Enqueue(data);
		if(data_queue.Count > chat_item_max)
		{
			data_queue.Dequeue();
		}
	}
	/*void InsertBattleData(ref Queue<CHAT_DATA>data_queue,CHAT_DATA data)
	{
		if(data_queue.Count == 0)
		{
			data_queue.Enqueue(data);
			return ;
		}
		data_queue.Enqueue(data);
		if(data_queue.Count > chat_item_max)
		{
			data_queue.Dequeue();
		}
	}*/
	bool CheckDataExist(Queue<CHAT_DATA>list,uint chat_id)
	{
		foreach(CHAT_DATA chat_unit in list)
		{
			if(chat_unit.ChatId == chat_id)
				return true;
		}
		return false;
	}
	//初始化聊天简略信息 
	void RefreshChatSmpUI()
	{
		CHAT_DATA[] data_array =AllChatDataList.ToArray();
		
		CHAT_DATA unit = data_array[data_array.Length-1];
		chat_smp_label.text = "";
	
		chat_smp_label.text += "[00FF00]"+unit.szFrom+"[-]: ";	
		chat_smp_label.text +=unit.szText;	
	}
	//副本战报回调 
	void BattlefieldReport(string report)
	{
		//print (report);
		CHAT_DATA chat_data = new CHAT_DATA();
		chat_data.ChatType = (int)GAME_SUB_RST.STC_GAMECMD_SYS_REPORT;
		chat_data.szFrom = "";
		chat_data.szTo = "";
		chat_data.szText = report;
		
		chat_data.ChatTime = (uint)(DataConvert.DateTimeToInt(DateTime.Now)- BuildingManager.WorldTimeModify);
		InsertData(ref AllChatDataList,chat_data);
		//chat_win.RefreshUI();
		RefreshChatSmpUI();
	}
}
