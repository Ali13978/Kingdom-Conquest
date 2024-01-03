using UnityEngine;
using System;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class MessageManager : MonoBehaviour {
	//未读邮件图标 
	public GameObject unread_icon;
	public static int sys_unread_num = 0;
	public static int personal_unread_num =0;
	//每页最大条数 
	public static int item_num_max = 10;
	//系统公告列表 
	public static Hashtable AnnounceMsgMap = new Hashtable();
	//个人邮件表 
	public static Hashtable PersonalMsgMap = new Hashtable();
	//系统消息表 
	public static Hashtable SysMsgMap = new Hashtable();
	//联盟事件 
	public static Hashtable AllianceEventMap = new Hashtable();
	//金矿事件  
	public static Hashtable GoldEventMap = new Hashtable();
	//用于计算请求了几种消息 确定所有请求都返回 
	//public static int ReqNum = 0;
	//个人消息总数量 用于翻页  
	public static int personal_msg_total_num;
	//系统消息总数量 用于翻页  
	public static int sys_msg_total_num;
	//联盟事件总数量 用于翻页  
	public static int alliance_event_total_num;
	//金矿总数量 用于翻页  
	public static int gold_event_total_num;
	public struct MessageData
	{
		public uint mail_id;
		public byte msg_type;
	}
	// Use this for initialization
	void Start () {
		InitialAnnounceFromDisk();
		ReqAnnounceData();
		ReqPersonalMsgData(0,(uint)(item_num_max-1));
		ReqSysMsgData(0,(uint)(item_num_max-1));
		ReqAllianceEventData(0,(uint)(item_num_max-1));
		ReqGoldEventData(0,(uint)(item_num_max-1));
		sys_unread_num = 0;
		personal_unread_num = 0;
	}
	void OpenMsgWin()
	{
		// Ensure data on
		GameObject obj  = U3dCmn.GetObjFromPrefab("MessageWin");
		if (obj != null)
		{
			//MessageWin win = obj.GetComponent<MessageWin>(); 
			obj.SendMessage("RevealPanel");
		}
	}
	//从硬盘中初始化前十条公告数据 
	void InitialAnnounceFromDisk()
	{
		AnnounceMsgMap.Clear();
		for(int i =0;i<item_num_max;i++)
		{
			//string announce_str = PlayerPrefs.GetString("Notification"+(i+1));
			string announce_str = U3dCmn.ReadText("Notification"+(i+1));
			//print("Notification"+(i+1)+announce_str);
			if(announce_str!="")
			{
				//print (announce_str);
				string[] str_array = announce_str.Split('#');
				Web_Notification_T announce= new Web_Notification_T();
				announce.nNotificationID2 = int.Parse(U3dCmn.CheckNullStr(str_array[0]));
				announce.nTime3 = uint.Parse(U3dCmn.CheckNullStr(str_array[1]));
				announce.pszContent7 = DataConvert.StrToBytes(U3dCmn.CheckNullStr(str_array[2]));
				if(!AnnounceMsgMap.Contains(announce.nNotificationID2))
				{
					AnnounceMsgMap.Add(announce.nNotificationID2,announce);
				}
			}
			
		}
		//print ("nnnnnnnnnnnnnnnnn"+AnnounceMsgMap.Count);
	}
	//请求公告数据 
	void ReqAnnounceData()
	{
		CTS_GAMECMD_GET_NOTIFICATION_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_NOTIFICATION;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_NOTIFICATION_T>(req);
		
	}
	//请求公告数据返回 
	void ReqAnnounceDataRst(byte[] buff)
	{
		STC_GAMECMD_GET_NOTIFICATION_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_NOTIFICATION_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_NOTIFICATION_T.enum_rst.RST_OK)
		{	
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_NOTIFICATION_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			Web_Notification_T[] announce_array = (Web_Notification_T[])DataConvert.ByteToStructArray<Web_Notification_T>(data_buff,sub_msg.nNum2);
			//PersonalMsgMap.Clear();
			//SysMsgMap.Clear();
			//if(announce_array.Length>0)
			//	AnnounceMsgMap.Clear();

			for(int i=0;i<announce_array.Length;i++)
			{
				if(!AnnounceMsgMap.Contains(announce_array[i].nNotificationID2))
				{
					AnnounceMsgMap.Add(announce_array[i].nNotificationID2,announce_array[i]);
				}	
			}
			int n =0;
			ArrayList list = new ArrayList(MessageManager.AnnounceMsgMap.Keys);
			list.Sort();
			for(int i=item_num_max;i<list.Count;i++)
			{
				MessageManager.AnnounceMsgMap.Remove((int)list[n]);
				n++;
			}
		}
	}
	//新的通知到来 
	void NewAnnounceData(byte[] buff)
	{
		Web_Notification_T sub_msg = DataConvert.ByteToStruct<Web_Notification_T>(buff);
		if(!AnnounceMsgMap.Contains(sub_msg.nNotificationID2))
		{
			AnnounceMsgMap.Add(sub_msg.nNotificationID2,sub_msg);
		}
		int n =0;
		ArrayList list = new ArrayList(MessageManager.AnnounceMsgMap.Keys);
		list.Sort();
		for(int i=item_num_max;i<list.Count;i++)
		{
			MessageManager.AnnounceMsgMap.Remove((int)list[n]);
			n++;
		}
	}
	//检查是否有未读的公告 
	public static int CheckUnreadAnnounce()
	{
		int num = 0;
		string content = "";
		const string sp = "\n\n";
		
		// 按照公告ID大小顺序来显示未读取公告 ....
		ArrayList list = new ArrayList ( AnnounceMsgMap.Keys );
        list.Sort();
		
		for (int i=list.Count-1; i>=0; -- i)
		{
			int nUnReadID = (int) list[i];
			
			// 取出当前的消息内容 ....
			Web_Notification_T info = (Web_Notification_T)AnnounceMsgMap[nUnReadID];
			if(info.nNotificationID2 > CommonData.player_online_info.NotificationID)
			{
				if(content != "") content += sp;
				content += DataConvert.BytesToStr(info.pszContent7);
				num++;
			}		
		}
		
		/*
		foreach(DictionaryEntry de in AnnounceMsgMap)
		{
			Web_Notification_T info = (Web_Notification_T)de.Value;
			if(info.nNotificationID2 > CommonData.player_online_info.NotificationID)
			{
				if(content != "") content += sp;
				content += DataConvert.BytesToStr(info.pszContent7);
				num++;
			}
		}
				
		*/

		
		if(num>0)
		{
			GameObject infowin = U3dCmn.GetObjFromPrefab("AnnounceWin");
			infowin.SendMessage("RevealPanel",content);
			SetAnnounceReaded();
		}
		
		return num;
	}
	//设置公告为已读 
	public static void SetAnnounceReaded()
	{
		int num = AnnounceMsgMap.Count;
		ArrayList list = new ArrayList(AnnounceMsgMap.Keys);
      	list.Sort();
	
		for(int i=0;i<num;i++)
		{
			Web_Notification_T announce = (Web_Notification_T)AnnounceMsgMap[(int)list[i]];
			string announce_str = announce.nNotificationID2+"#"+announce.nTime3+"#"+DataConvert.BytesToStr(announce.pszContent7);
			string file_path = U3dCmn.GetCachePath()+"/Notification"+(i+1);
			
			U3dCmn.WriteText("Notification"+(i+1),announce_str);
			//PlayerPrefs.SetString("Notification"+(i+1),announce_str);
			//PlayerPrefs.SetString
			//string str = PlayerPrefs.GetString("Notification"+(i+1));
			
			if(i == (num-1))
			{
				CommonData.player_online_info.NotificationID = (uint)announce.nNotificationID2;
			}
			//PlayerPrefs.SetString("Notification"+(i+1),);
		}
		if(CommonData.player_online_info.NotificationID >0)
		{
			CTS_GAMECMD_OPERATE_READ_NOTIFICATION_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_READ_NOTIFICATION;
			req.nNotificationID3 = CommonData.player_online_info.NotificationID;
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_READ_NOTIFICATION_T>(req);
		}
	}
	//请求个人消息数据(联盟邮件、个人邮件)   
	public static void ReqPersonalMsgData(uint from_num,uint to_num)
	{	
		CTS_GAMECMD_GET_MAIL_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_MAIL;
		req.nFromNum3 = from_num;
		req.nToNum4 = to_num;
		req.nType5 = (byte)(MAIL_TYPE.MAIL_TYPE_ALLIANCE | MAIL_TYPE.MAIL_TYPE_PRIVATE);
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_MAIL_T>(req);
		
		//ReqNum++;
	}
	//请求系统消息数据  
	public static void ReqSysMsgData(uint from_num,uint to_num)
	{	
		CTS_GAMECMD_GET_MAIL_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_MAIL;
		req.nFromNum3 = from_num;
		req.nToNum4 = to_num;
		req.nType5 = (byte)MAIL_TYPE.MAIL_TYPE_SYSTEM;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_MAIL_T>(req);
		//ReqNum++;
	}
	//请求联盟事件数据  
	public static void ReqAllianceEventData(uint from_num,uint to_num)
	{	
		CTS_GAMECMD_GET_MAIL_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_MAIL;
		req.nFromNum3 = from_num;
		req.nToNum4 = to_num;
		req.nType5 = (byte)MAIL_TYPE.MAIL_TYPE_ALLIANCE_EVENT;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_MAIL_T>(req);
		//ReqNum++;
	}
	//请求金矿事件数据  
	public static void ReqGoldEventData(uint from_num,uint to_num)
	{	
		CTS_GAMECMD_GET_MAIL_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_MAIL;
		req.nFromNum3 = from_num;
		req.nToNum4 = to_num;
		req.nType5 = (byte)MAIL_TYPE.MAIL_TYPE_GOLDORE_EVENT;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_MAIL_T>(req);
		//ReqNum++;
	}
	//请求消息数据返回 
	public void ReqPersonalMsgDataRst(byte[] buff)
	{
		STC_GAMECMD_GET_MAIL_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_MAIL_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_MAIL_T.enum_rst.RST_OK)
		{	
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_MAIL_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			PrivateMailUnit[] mail_array = (PrivateMailUnit[])DataConvert.ByteToStructArray<PrivateMailUnit>(data_buff,sub_msg.nNum4);
			//PersonalMsgMap.Clear();
			//SysMsgMap.Clear();
			
			for(int i=0;i<mail_array.Length;i++)
			{
				
				string str = DataConvert.BytesToStr(mail_array[i].szText10);
				if(mail_array[i].nType3 == (byte)MAIL_TYPE.MAIL_TYPE_PRIVATE || mail_array[i].nType3 == (byte)MAIL_TYPE.MAIL_TYPE_ALLIANCE)
				{
					personal_unread_num = sub_msg.nUnreadMailNum3;	
					SetUnReadIcon();
					personal_msg_total_num = sub_msg.nTotalMailNum2;
				
					VipRightInfo vipinfo = (VipRightInfo)CommonMB.VipRightInfo_Map[(int)CommonData.player_online_info.Vip];
					//print("MMMMMMMMMMMMMMMMMMMMMMM"+vipinfo.MailBoxTop);
					if(personal_msg_total_num >= vipinfo.MailBoxTop)
					{
						U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.PRIVATE_MAIL_IS_FULL));
					}
					if(!PersonalMsgMap.Contains(mail_array[i].nMailID1))
						PersonalMsgMap.Add(mail_array[i].nMailID1,mail_array[i]);
				}
				else if(mail_array[i].nType3 == (byte)MAIL_TYPE.MAIL_TYPE_SYSTEM)
				{	
					sys_unread_num = sub_msg.nUnreadMailNum3;
					SetUnReadIcon();
					sys_msg_total_num = sub_msg.nTotalMailNum2;
					if(!SysMsgMap.Contains(mail_array[i].nMailID1))
						SysMsgMap.Add(mail_array[i].nMailID1,mail_array[i]);
				}
				else if(mail_array[i].nType3 == (byte)MAIL_TYPE.MAIL_TYPE_ALLIANCE_EVENT)
				{
					alliance_event_total_num = sub_msg.nTotalMailNum2;
					if(!AllianceEventMap.Contains(mail_array[i].nMailID1))
						AllianceEventMap.Add(mail_array[i].nMailID1,mail_array[i]);
				}
				else if(mail_array[i].nType3 == (byte)MAIL_TYPE.MAIL_TYPE_GOLDORE_EVENT)
				{
					gold_event_total_num = sub_msg.nTotalMailNum2;
					if(!GoldEventMap.Contains(mail_array[i].nMailID1))
						GoldEventMap.Add(mail_array[i].nMailID1,mail_array[i]);
				}
				//Item_Id_List.Add(mail_array[i].nItemID1);
				//CangKuItemMap.Add(mail_array[i].nItemID1,item_array[i]);
			}
			
			U3dCmn.SendMessage("MessageWin","InitialPanelData",null);
			
			
			//print("wwwwwwwwwwwwwwww"+unread_num);
			
			//if(--ReqNum == 0)
			//{
			//	//通知消息界面刷新数据 
				//U3dCmn.SendMessage("MessageWin","InitialPanelData",null);
				
			//}
		}
	}
	//查看邮件 
	public static void ReadMail(uint mail_id)
	{
		
		CTS_GAMECMD_READ_MAIL_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_READ_MAIL;
		req.nMailID3 = mail_id;
		TcpMsger.SendLogicData<CTS_GAMECMD_READ_MAIL_T>(req);
		if(SysMsgMap.Contains(mail_id))
		{
			PrivateMailUnit unit = (PrivateMailUnit)SysMsgMap[mail_id];
			unit.bReaded5 = 1;
			SysMsgMap[mail_id] = unit;
		}
		else if(PersonalMsgMap.Contains(mail_id))
		{
			PrivateMailUnit unit = (PrivateMailUnit)PersonalMsgMap[mail_id];
			unit.bReaded5 = 1;
			PersonalMsgMap[mail_id] = unit;
		}
		
	}
	//查看邮件返回  
	void ReadMailRst(byte[] buff)
	{
		STC_GAMECMD_READ_MAIL_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_READ_MAIL_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_READ_MAIL_T.enum_rst.RST_OK)
		{
			if(GetMailTypeFromID(sub_msg.nMailID2) == (byte)MAIL_TYPE.MAIL_TYPE_PRIVATE)
			{
				personal_unread_num --;
				SetUnReadIcon();
			}
			else if(GetMailTypeFromID(sub_msg.nMailID2) == (byte)MAIL_TYPE.MAIL_TYPE_SYSTEM)
			{
				sys_unread_num --;
				SetUnReadIcon();
			}
		}
	}
	
	//请求删除邮件 
	public static void ReqDeleteMail(uint mail_id)
	{
		CTS_GAMECMD_DELETE_MAIL_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_DELETE_MAIL;
		req.nMailI3 = mail_id;
		TcpMsger.SendLogicData<CTS_GAMECMD_DELETE_MAIL_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_DELETE_MAIL);
	}
	//请求删除邮件返回 
	public void ReqDeleteMailRst(byte[] buff)
	{
		STC_GAMECMD_DELETE_MAIL_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_DELETE_MAIL_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_DELETE_MAIL_T.enum_rst.RST_OK)
		{
			if(PersonalMsgMap.Contains(sub_msg.nMailID3))
			{
				PrivateMailUnit unit = (PrivateMailUnit)PersonalMsgMap[sub_msg.nMailID3];
				if(unit.bReaded5 == 0)
				{
					personal_unread_num --;
					SetUnReadIcon();
				}
				personal_msg_total_num = sub_msg.nTotalMailNum2;
				PersonalMsgMap.Remove(sub_msg.nMailID3);
				if(PersonalMsgMap.Count <MessageWin.now_page_num*10)
				{
					ReqPersonalMsgData((uint)(PersonalMsgMap.Count-1),(uint)(MessageWin.now_page_num*10-1));
				}
			}
			else if(SysMsgMap.Contains(sub_msg.nMailID3))
			{
				PrivateMailUnit unit = (PrivateMailUnit)SysMsgMap[sub_msg.nMailID3];
				if(unit.bReaded5 == 0)
				{
					sys_unread_num --;
					SetUnReadIcon();
				}
				sys_msg_total_num= sub_msg.nTotalMailNum2;
				SysMsgMap.Remove(sub_msg.nMailID3);
				if(SysMsgMap.Count <MessageWin.now_page_num*10)
				{
					//MessageWin.
					ReqSysMsgData((uint)(SysMsgMap.Count-1),(uint)(MessageWin.now_page_num*10-1));
				}
			}
			else if(AllianceEventMap.Contains(sub_msg.nMailID3))
			{
				alliance_event_total_num= sub_msg.nTotalMailNum2;
				AllianceEventMap.Remove(sub_msg.nMailID3);
				if(AllianceEventMap.Count <MessageWin.now_page_num*10)
				{
					ReqAllianceEventData((uint)(AllianceEventMap.Count-1),(uint)(MessageWin.now_page_num*10-1));
				}
			}	
			else if(GoldEventMap.Contains(sub_msg.nMailID3))
			{
				gold_event_total_num = sub_msg.nTotalMailNum2;
				GoldEventMap.Remove(sub_msg.nMailID3);
				if(GoldEventMap.Count <MessageWin.now_page_num*10)
				{
					ReqGoldEventData((uint)(GoldEventMap.Count-1),(uint)(MessageWin.now_page_num*10-1));
				}
			}
			//通知消息界面刷新数据 
			U3dCmn.SendMessage("MessageWin","InitialPanelData",null);
			SetUnReadIcon();
		}
	}
	//发送私人邮件返回 
	void SendMailRst(byte[] buff)
	{
		STC_GAMECMD_ADD_MAIL_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_ADD_MAIL_T>(buff);
		//print ("mmmmmmmmmmmmm"+sub_msg.nRst1);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_ADD_MAIL_T.enum_rst.RST_OK)
		{
			U3dCmn.SendMessage("WriteMessageWin","DismissPanel",null);
			U3dCmn.SendMessage("ReadMessageWin","DismissPanel",null);
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_ADD_MAIL_T.enum_rst.RST_SENDER_SEND_MAX)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.SEND_MAIL_FULL));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_ADD_MAIL_T.enum_rst.RST_RECEIVER_MAIL_FULL)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.RECEIVER_MAIL_FULL));
		}
		
	}
	//发送联盟邮件返回 
	void SendAllianceMailRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_ADD_ALLIANCE_MAIL_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_ADD_ALLIANCE_MAIL_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_ADD_ALLIANCE_MAIL_T.enum_rst.RST_OK)
		{
			U3dCmn.SendMessage("WriteMessageWin","DismissPanel",null);
			U3dCmn.SendMessage("ReadMessageWin","DismissPanel",null);
		}
	}
	//新邮件到来 
	public void NewMail(byte[] buff)
	{
		STC_GAMECMD_NEW_PRIVATE_MAIL_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_NEW_PRIVATE_MAIL_T>(buff);
		int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_NEW_PRIVATE_MAIL_T>();
		int data_len = buff.Length - head_len;
		byte[] data_buff = new byte[data_len];
		Array.Copy(buff,head_len,data_buff,0,data_len);
		
		PrivateMailUnit mail_unit = DataConvert.ByteToStruct<PrivateMailUnit>(data_buff);
		if(mail_unit.nType3 == (byte)MAIL_TYPE.MAIL_TYPE_PRIVATE || mail_unit.nType3 == (byte)MAIL_TYPE.MAIL_TYPE_ALLIANCE)
		{
			personal_unread_num ++;
			SetUnReadIcon();
			personal_msg_total_num = sub_msg.nTotalMailNum1;
			if(!PersonalMsgMap.Contains(mail_unit.nMailID1))
				PersonalMsgMap.Add(mail_unit.nMailID1,mail_unit);
		}
		else if(mail_unit.nType3 == (byte)MAIL_TYPE.MAIL_TYPE_SYSTEM)
		{
			sys_unread_num++;
			SetUnReadIcon();
			sys_msg_total_num = sub_msg.nTotalMailNum1;
			if(!SysMsgMap.Contains(mail_unit.nMailID1))
				SysMsgMap.Add(mail_unit.nMailID1,mail_unit);
		}
		else if(mail_unit.nType3 == (byte)MAIL_TYPE.MAIL_TYPE_ALLIANCE_EVENT)
		{
			//mail_unit. 
			alliance_event_total_num = sub_msg.nTotalMailNum1;
			if(!AllianceEventMap.Contains(mail_unit.nMailID1))
				AllianceEventMap.Add(mail_unit.nMailID1,mail_unit);
		}
		else if(mail_unit.nType3 == (byte)MAIL_TYPE.MAIL_TYPE_GOLDORE_EVENT)
		{
			//mail_unit. 
			gold_event_total_num = sub_msg.nTotalMailNum1;
			if(!GoldEventMap.Contains(mail_unit.nMailID1))
				GoldEventMap.Add(mail_unit.nMailID1,mail_unit);
		}
		//通知消息界面刷新数据 
		U3dCmn.SendMessage("MessageWin","InitialPanelData",null);
		
		
	}
	//根据邮件ID查看属于哪个邮件种类 
	byte GetMailTypeFromID(uint MailID)
	{
		byte mail_type = 0;
		if(PersonalMsgMap.Contains(MailID))
		{
			mail_type = (byte)MAIL_TYPE.MAIL_TYPE_PRIVATE;
		}
		else if(SysMsgMap.Contains(MailID))
		{
			mail_type = (byte)MAIL_TYPE.MAIL_TYPE_SYSTEM;
		}
		else if(AllianceEventMap.Contains(MailID))
		{
			mail_type = (byte)MAIL_TYPE.MAIL_TYPE_ALLIANCE_EVENT;
		}	
		else if(GoldEventMap.Contains(MailID))
		{
			mail_type = (byte)MAIL_TYPE.MAIL_TYPE_GOLDORE_EVENT;
		}
		return mail_type;
	}
	public void SetUnReadIcon()
	{
		int unread_num = personal_unread_num+sys_unread_num;
		if(unread_num >0)
		{
			unread_icon.SetActiveRecursively(true);
			if(unread_num >99)
				unread_icon.GetComponentInChildren<UILabel>().text = "99+";
			else 
				unread_icon.GetComponentInChildren<UILabel>().text = unread_num.ToString();
		}
		else
		{
			unread_icon.SetActiveRecursively(false);
		}
		// if messagewin is open, refresh the unread icon in it
		GameObject msgWin	= GameObject.Find("MessageWin");
		if( msgWin != null ) {
			msgWin.SendMessage("SetUnReadIcons");
		}
	}
	//读取所有邮件 
	public static void ReadAll(uint mail_type)
	{
		
		CTS_GAMECMD_READ_MAIL_ALL_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_READ_MAIL_ALL;
		req.nMailType3 = mail_type;
		TcpMsger.SendLogicData<CTS_GAMECMD_READ_MAIL_ALL_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_READ_MAIL_ALL);
	}
	//全部已读返回 
	void ReadAllRst(byte[] buff)
	{
		STC_GAMECMD_READ_MAIL_ALL_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_READ_MAIL_ALL_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_READ_MAIL_ALL_T.enum_rst.RST_OK)
		{
			if(sub_msg.nMailType2 == (uint)(MAIL_TYPE.MAIL_TYPE_ALLIANCE | MAIL_TYPE.MAIL_TYPE_PRIVATE))
			{
				ArrayList list = new ArrayList(PersonalMsgMap.Keys);
				for(int i=0;i<list.Count;i++)
				{
					uint mail_id =(uint)list[i];
					PrivateMailUnit unit =  (PrivateMailUnit)PersonalMsgMap[mail_id];
					unit.bReaded5 = 1;
					PersonalMsgMap[mail_id] = unit;
				}
				personal_unread_num = 0;
				SetUnReadIcon();
				//MessageManager
			}
			else if(sub_msg.nMailType2 == (uint)MAIL_TYPE.MAIL_TYPE_SYSTEM)
			{
				
				ArrayList list = new ArrayList(SysMsgMap.Keys);
				int reward_num = 0;
				for(int i=0;i<list.Count;i++)
				{
					uint mail_id =(uint)list[i];
					PrivateMailUnit unit =  (PrivateMailUnit)SysMsgMap[mail_id];
					if(unit.nFlag4 ==(byte)MAIL_FLAG.MAIL_FLAG_REWARD)
					{
						reward_num ++;
					}
					else 
					{
						unit.bReaded5 = 1;
						SysMsgMap[mail_id] = unit;
					}
				}
				sys_unread_num = reward_num;
				SetUnReadIcon();
			}
			//通知消息界面刷新数据 
			U3dCmn.SendMessage("MessageWin","InitialPanelData",null);
		}
	}
	//删除所有邮件  
	public static void DeleteAll(uint mail_type)
	{
		CTS_GAMECMD_DELETE_MAIL_ALL_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_DELETE_MAIL_ALL;
		req.nMailType3 = mail_type;
		TcpMsger.SendLogicData<CTS_GAMECMD_DELETE_MAIL_ALL_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_DELETE_MAIL_ALL);
	}
	//全部删除返回 
	void DeleteAllRst(byte[] buff)
	{
		STC_GAMECMD_DELETE_MAIL_ALL_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_DELETE_MAIL_ALL_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_DELETE_MAIL_ALL_T.enum_rst.RST_OK)
		{
			if(sub_msg.nMailType2 == (uint)(MAIL_TYPE.MAIL_TYPE_ALLIANCE | MAIL_TYPE.MAIL_TYPE_PRIVATE))
			{
				PersonalMsgMap.Clear();
				personal_unread_num = 0;
				personal_msg_total_num = 0;
				SetUnReadIcon();
				//MessageManager
			}
			else if(sub_msg.nMailType2 == (uint)MAIL_TYPE.MAIL_TYPE_SYSTEM)
			{
				SysMsgMap.Clear();
				sys_unread_num = 0;
				sys_msg_total_num = 0;
				SetUnReadIcon();
			}
			else if(sub_msg.nMailType2 == (uint)MAIL_TYPE.MAIL_TYPE_ALLIANCE_EVENT)
			{
				//print ("aaaaaaaaaaaaaaaaaaaaaaaa");
				AllianceEventMap.Clear();
				alliance_event_total_num = 0;
			}
			else if(sub_msg.nMailType2 == (uint)MAIL_TYPE.MAIL_TYPE_GOLDORE_EVENT)
			{
				GoldEventMap.Clear();
				gold_event_total_num = 0;
			}
			//通知消息界面刷新数据 
			U3dCmn.SendMessage("MessageWin","InitialPanelData",null);
		}
	}
	//解析带附件的邮件 
	public static string ExplainRewardMail(byte[] buff)
	{
		string result ="";
		string str = DataConvert.BytesToStr(buff);
		string[] reward_array = str.Split(',');
		for(int i= 0;i<reward_array.Length-1;i++)
		{
			string[] str_array = reward_array[i].Split('*');
			//print (str_array[0]);
			if(int.Parse(str_array[0]) == (int)GoodsSort.mail_reward_type_diamond)
			{
				result += U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.DIAMOND)+str_array[1];
			}
			else if(int.Parse(str_array[0]) == (int)GoodsSort.mail_reward_type_crystal)
			{
				result += U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CRYSTAL)+str_array[1];
				//U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CRYSTAL);
			}
			else if(int.Parse(str_array[0]) == (int)GoodsSort.mail_reward_type_gold)
			{
				result += U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GOLD)+str_array[1];
			}
			else if(int.Parse(str_array[0]) ==(int)GoodsSort.mail_reward_type_item)
			{
				uint excel_id = uint.Parse(str_array[2]);
				ITEM_INFO item_info = U3dCmn.GetItemIconByExcelID(excel_id);
				result += item_info.Name + "x"+str_array[1];
			}
			if(i<reward_array.Length-2)
			{
				result+=",";
			}
		}
		return result;
	}
	//领取带附件的邮件 
	public static void ReceiveRewardMail(uint MailID)
	{
		CTS_GAMECMD_OPERATE_FETCH_MAIL_REWARD_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_FETCH_MAIL_REWARD;
		req.nMailID3 = MailID;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_FETCH_MAIL_REWARD_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_FETCH_MAIL_REWARD);
		
	}
	//领取带附件的邮件返回 
	void ReceiveRewardMailRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_FETCH_MAIL_REWARD_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_FETCH_MAIL_REWARD_T>(buff);
		int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_OPERATE_FETCH_MAIL_REWARD_T>();
		int data_len = buff.Length - head_len;
		byte[] data_buff = new byte[data_len];
		Array.Copy(buff,head_len,data_buff,0,data_len);
		
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_FETCH_MAIL_REWARD_T.enum_rst.RST_OK)
		{
			RewardUnit[] reward_array = (RewardUnit[])DataConvert.ByteToStructArray<RewardUnit>(data_buff,(int)sub_msg.nNum3);
			for(int i=0;i<reward_array.Length;i++)
			{
				if(reward_array[i].nType1 == (uint)GoodsSort.mail_reward_type_diamond)
				{
					CommonData.player_online_info.Diamond += reward_array[i].nNum2;
					PlayerInfoManager.RefreshPlayerDataUI();
				}
				else if(reward_array[i].nType1 == (uint)GoodsSort.mail_reward_type_crystal)
				{
					CommonData.player_online_info.Crystal += reward_array[i].nNum2;
					PlayerInfoManager.RefreshPlayerDataUI();
				}
				else if(reward_array[i].nType1 == (uint)GoodsSort.mail_reward_type_gold)
				{
					CommonData.player_online_info.Gold += reward_array[i].nNum2;
					PlayerInfoManager.RefreshPlayerDataUI();
				}
				else if(reward_array[i].nType1 == (uint)GoodsSort.mail_reward_type_item)
				{ 
					CangKuManager.AddItem(reward_array[i].nID4,(int)reward_array[i].nData3,(int)reward_array[i].nNum2);
				}
				
			}
			if(SysMsgMap.Contains(sub_msg.nMailID2))
			{
				sys_msg_total_num --;
				sys_unread_num --;
				SetUnReadIcon();
				SysMsgMap.Remove(sub_msg.nMailID2);
			}
			//通知消息界面刷新数据 
			U3dCmn.SendMessage("MessageWin","InitialPanelData",null);
		}
		
	}
}
