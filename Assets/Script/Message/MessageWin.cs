using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class MessageWin : MonoBehaviour {
	// unread icons
	public GameObject unread_icon_announce;
	public GameObject unread_icon_personal_msg;
	public GameObject unread_icon_sys_msg;
	//系统通知面板 
	public UICheckbox announce_tab;
	public GameObject announce_item;
	public UIDraggablePanel announce_panel;
	public UIGrid announce_grid;
	GameObject[] announce_msg_item_array = new GameObject[MessageManager.item_num_max];
	//消息窗口positon TWEEN    
	public TweenPosition tween_position; 
	//个人消息面板 
	public UICheckbox personal_msg_tab;
	public GameObject personal_msg_item;
	public UILabel    personal_content;
	public UIDraggablePanel personal_msg_panel;
	public UIGrid personal_msg_grid;
	GameObject[] personal_msg_item_array = new GameObject[MessageManager.item_num_max];
	//系统消息面板 
	public UICheckbox sys_msg_tab;
	public GameObject sys_msg_item;
	public UIDraggablePanel sys_msg_panel;
	public UIGrid sys_msg_grid;
	GameObject[] sys_msg_item_array = new GameObject[MessageManager.item_num_max];
	//联盟事件面板 
	public UICheckbox alliance_event_tab;
	//金矿事件面板 
	public UICheckbox  gold_event_tab;
	//当前信息 
	public UILabel page_num;
	public static int now_page_num;
	UIGrid now_grid = null;
	//bool font = false;																																																														
	bool refresh = false;
	
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
	// Use this for initialization
	void Start () {
		for(int i=0;i<MessageManager.item_num_max;i++)
		{
			
			GameObject obj = NGUITools.AddChild(announce_grid.gameObject,announce_item);
			obj.name = "item"+i;
			obj.SetActiveRecursively(false);
			announce_msg_item_array[i] = obj;
			
			obj = NGUITools.AddChild(personal_msg_grid.gameObject,personal_msg_item);
			obj.name = "item"+i;
			obj.SetActiveRecursively(false);
			personal_msg_item_array[i] = obj;
			
			obj = NGUITools.AddChild(sys_msg_grid.gameObject,sys_msg_item);
			obj.SetActiveRecursively(false);
			obj.name = "item"+i;
			sys_msg_item_array[i] = obj;
		}
		now_page_num = 1;
		SetUnReadIcons();
	}
	
	// Update is called once per frame
	void Update () {
		if(refresh)
		{
			CheckUnreadPanel();
			InitialPanelData();
			refresh = false;
			
		}
	}
	void RevealPanel()
	{
	
		now_page_num = 1;
		//war_situation_panel.gameObject.SetActiveRecursively(false);
		//personal_msg_panel.gameObject.SetActiveRecursively(false);
		//sys_msg_panel.gameObject.SetActiveRecursively(false);
		//font = true;
		refresh = true;
		if(CommonData.player_online_info.AllianceID ==0)
			alliance_event_tab.gameObject.SetActiveRecursively(false);
		else 
			alliance_event_tab.gameObject.SetActiveRecursively(true);
		
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		//font = false;
		//ClearGrid(now_grid);
		now_grid = null;
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//切换面板 
	void ChangePanel()
	{
		now_page_num = 1;
		InitialPanelData();
	}
	//初始化面板数据 
	void InitialPanelData()
	{
		//if(!font)
		//	return;
		if(announce_tab.isChecked)
		{
			announce_panel.gameObject.SetActiveRecursively(true);	
			
			ArrayList list = new ArrayList(MessageManager.AnnounceMsgMap.Keys);
      		list.Sort();
			
			int from_num = 0;
			int end_num =  MessageManager.AnnounceMsgMap.Count;
			//print ("llllllllllllll"+end_num);
			int i=0;
			for(int n= from_num;n<end_num;n++)
			{
				//print ("");
				int index = list.Count - n-1;
				Web_Notification_T unit = (Web_Notification_T)MessageManager.AnnounceMsgMap[list[index]];
				//GameObject obj = NGUITools.AddChild(personal_msg_grid.gameObject,personal_msg_item);
				//obj.name = "item"+i++;
				
				MessageManager.MessageData data;
				data.mail_id = (uint)unit.nNotificationID2;
				data.msg_type = (byte)MAIL_TYPE.MAIL_TYPE_ANNOUNCE;

				announce_msg_item_array[i].SetActiveRecursively(true);
				announce_msg_item_array[i].SendMessage("InitialData",data);
				i++;
			}
			for(;i<4;i++)
			{
				//GameObject obj = NGUITools.AddChild(personal_msg_grid.gameObject,personal_msg_item);
				//obj.name = "item"+i;
				MessageManager.MessageData data = new MessageManager.MessageData();
				announce_msg_item_array[i].SetActiveRecursively(true);
				announce_msg_item_array[i].SendMessage("InitialData",data);
			}
			for(;i<MessageManager.item_num_max;i++)
			{
				announce_msg_item_array[i].SetActiveRecursively(false);
			}
			announce_grid.Reposition();
			announce_panel.ResetPosition();
			page_num.text = "1/1";
			//SetAnnounceReaded();
		}
		else if(personal_msg_tab.isChecked)
		{
			personal_msg_panel.gameObject.SetActiveRecursively(true);
			int max_page_num = U3dCmn.GetPageNum(MessageManager.personal_msg_total_num,MessageManager.item_num_max);
			if(now_page_num>max_page_num)
				now_page_num = max_page_num;
			//ClearGrid(now_grid);
			
			
			ArrayList list = new ArrayList(MessageManager.PersonalMsgMap.Keys);
      		list.Sort();
			
			int from_num = (now_page_num-1)*MessageManager.item_num_max;
			int end_num = from_num+MessageManager.item_num_max>list.Count?list.Count:from_num+MessageManager.item_num_max;
			int i=0;
			for(int n= from_num;n<end_num;n++)
			{
				//print ("");
				int index = list.Count - n-1;
				PrivateMailUnit unit = (PrivateMailUnit)MessageManager.PersonalMsgMap[list[index]];
				//GameObject obj = NGUITools.AddChild(personal_msg_grid.gameObject,personal_msg_item);
				//obj.name = "item"+i++;
				MessageManager.MessageData data;
				data.mail_id = unit.nMailID1;
				data.msg_type = unit.nType3;
				personal_msg_item_array[i].SetActiveRecursively(true);
				personal_msg_item_array[i].SendMessage("InitialData",data);
				i++;
			}
			for(;i<4;i++)
			{
				//GameObject obj = NGUITools.AddChild(personal_msg_grid.gameObject,personal_msg_item);
				//obj.name = "item"+i;
				MessageManager.MessageData data = new MessageManager.MessageData();
				personal_msg_item_array[i].SetActiveRecursively(true);
				personal_msg_item_array[i].SendMessage("InitialData",data);
			}
			for(;i<MessageManager.item_num_max;i++)
			{
				personal_msg_item_array[i].SetActiveRecursively(false);
			}
			personal_msg_grid.Reposition();
			personal_msg_panel.ResetPosition();
			//print ("totaltotal"+MessageManager.personal_msg_total_num);
			page_num.text = now_page_num +"/"+max_page_num;
		}
		else if(sys_msg_tab.isChecked)
		{
			sys_msg_panel.gameObject.SetActiveRecursively(true);
			int max_page_num = U3dCmn.GetPageNum(MessageManager.sys_msg_total_num,MessageManager.item_num_max);
			if(now_page_num>max_page_num)
				now_page_num = max_page_num;
		
			ArrayList list = new ArrayList(MessageManager.SysMsgMap.Keys);
      		list.Sort();
			int from_num = (now_page_num-1)*MessageManager.item_num_max;
			int end_num = from_num+MessageManager.item_num_max>list.Count?list.Count:from_num+MessageManager.item_num_max;
			int i=0;
			for(int n= from_num;n<end_num;n++)
			{
				int index = list.Count - n-1;
				PrivateMailUnit unit = (PrivateMailUnit)MessageManager.SysMsgMap[list[index]];
				MessageManager.MessageData data;
				data.mail_id = unit.nMailID1;
				data.msg_type = unit.nType3;
				sys_msg_item_array[i].SetActiveRecursively(true);
				sys_msg_item_array[i].SendMessage("InitialData",data);
				i++;
			}
			for(;i<4;i++)
			{
				MessageManager.MessageData data = new MessageManager.MessageData();
				sys_msg_item_array[i].SetActiveRecursively(true);
				sys_msg_item_array[i].SendMessage("InitialData",data);
			}
			for(;i<MessageManager.item_num_max;i++)
			{
				sys_msg_item_array[i].SetActiveRecursively(false);
			}
			sys_msg_grid.Reposition();
			sys_msg_panel.ResetPosition();
			page_num.text = now_page_num +"/"+max_page_num;
		}
		else if(alliance_event_tab.isChecked)
		{
			sys_msg_panel.gameObject.SetActiveRecursively(true);
			int max_page_num = U3dCmn.GetPageNum(MessageManager.alliance_event_total_num,MessageManager.item_num_max);
			if(now_page_num>max_page_num)
				now_page_num = max_page_num;
		
			ArrayList list = new ArrayList(MessageManager.AllianceEventMap.Keys);
      		list.Sort();
			int from_num = (now_page_num-1)*MessageManager.item_num_max;
			int end_num = from_num+MessageManager.item_num_max>list.Count?list.Count:from_num+MessageManager.item_num_max;
			int i=0;
			for(int n= from_num;n<end_num;n++)
			{
				int index = list.Count - n-1;
				PrivateMailUnit unit = (PrivateMailUnit)MessageManager.AllianceEventMap[list[index]];
				MessageManager.MessageData data;
				data.mail_id = unit.nMailID1;
				data.msg_type = unit.nType3;
				sys_msg_item_array[i].SetActiveRecursively(true);
				sys_msg_item_array[i].SendMessage("InitialData",data);
				i++;
			}
			for(;i<4;i++)
			{
				MessageManager.MessageData data = new MessageManager.MessageData();
				sys_msg_item_array[i].SetActiveRecursively(true);
				sys_msg_item_array[i].SendMessage("InitialData",data);
			}
			for(;i<MessageManager.item_num_max;i++)
			{
				sys_msg_item_array[i].SetActiveRecursively(false);
			}
			sys_msg_grid.Reposition();
			sys_msg_panel.ResetPosition();
			page_num.text = now_page_num +"/"+max_page_num;
		}
		else if(gold_event_tab.isChecked)
		{
			sys_msg_panel.gameObject.SetActiveRecursively(true);
			int max_page_num = U3dCmn.GetPageNum(MessageManager.gold_event_total_num,MessageManager.item_num_max);
			if(now_page_num>max_page_num)
				now_page_num = max_page_num;
		
			ArrayList list = new ArrayList(MessageManager.GoldEventMap.Keys);
      		list.Sort();
			int from_num = (now_page_num-1)*MessageManager.item_num_max;
			int end_num = from_num+MessageManager.item_num_max>list.Count?list.Count:from_num+MessageManager.item_num_max;
			int i=0;
			for(int n= from_num;n<end_num;n++)
			{
				int index = list.Count - n-1;
				PrivateMailUnit unit = (PrivateMailUnit)MessageManager.GoldEventMap[list[index]];
				MessageManager.MessageData data;
				data.mail_id = unit.nMailID1;
				data.msg_type = unit.nType3;
				sys_msg_item_array[i].SetActiveRecursively(true);
				sys_msg_item_array[i].SendMessage("InitialData",data);
				i++;
			}
			for(;i<4;i++)
			{
				MessageManager.MessageData data = new MessageManager.MessageData();
				sys_msg_item_array[i].SetActiveRecursively(true);
				sys_msg_item_array[i].SendMessage("InitialData",data);
			}
			for(;i<MessageManager.item_num_max;i++)
			{
				sys_msg_item_array[i].SetActiveRecursively(false);
			}
			sys_msg_grid.Reposition();
			sys_msg_panel.ResetPosition();
			page_num.text = now_page_num +"/"+max_page_num;
		}
		else
		{
			//ClearGrid(now_grid);
			//now_grid = null;
			now_page_num = 1;
			page_num.text = now_page_num +"/"+1;
		}
		
	}
	void SetUnReadIcon(GameObject unread_icon, int unread_num)
	{
		// unread_num ++; for testing display
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
	}
	void SetUnReadIcons()
	{
		//print("SetUnReadIcons:"+MessageManager.sys_unread_num);
		int nAnnounceCount = MessageManager.CheckUnreadAnnounce();
		SetUnReadIcon(unread_icon_announce, nAnnounceCount);
		SetUnReadIcon(unread_icon_personal_msg, MessageManager.personal_unread_num);
		SetUnReadIcon(unread_icon_sys_msg, MessageManager.sys_unread_num);
	}
	//根据未读的邮件种类 选择标签 
	void CheckUnreadPanel()
	{
		int nAnnounceCount = MessageManager.CheckUnreadAnnounce();
		if(nAnnounceCount>0)
		{
			announce_tab.isChecked	= true;
		}
		else if(MessageManager.personal_unread_num>0)
		{
			personal_msg_tab.isChecked = true;
			//personal_msg_grid.Reposition();
			//personal_msg_panel.ResetPosition();
		}
		else if(MessageManager.sys_unread_num >0)
		{
			sys_msg_tab.isChecked = true;
			//sys_msg_grid.Reposition();
			//sys_msg_panel.ResetPosition();
		}
		SetUnReadIcons();
	}
	
	
	//下一页 
	void NextPage()
	{
		if(personal_msg_tab.isChecked)
		{
			if(now_page_num < U3dCmn.GetPageNum(MessageManager.personal_msg_total_num,MessageManager.item_num_max))
			{
				if((++now_page_num)*10 > MessageManager.PersonalMsgMap.Count)
				{
					//print ("ttttttttttttttttttttttt");
					MessageManager.ReqPersonalMsgData((uint)((now_page_num-1)*10),(uint)((now_page_num-1)*10+9));
					LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_MAIL);
				}
				else
				{
					InitialPanelData();
				}
			}	
		}
		else if(sys_msg_tab.isChecked)
		{
			if(now_page_num < U3dCmn.GetPageNum(MessageManager.sys_msg_total_num,MessageManager.item_num_max))
			{
				if((++now_page_num)*10 > MessageManager.SysMsgMap.Count)
				{
					MessageManager.ReqSysMsgData((uint)((now_page_num-1)*10),(uint)((now_page_num-1)*10+9));
					LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_MAIL);
				}
				else
				{
					InitialPanelData();
				}
			}	
		}
		else if(alliance_event_tab.isChecked)
		{
			if(now_page_num < U3dCmn.GetPageNum(MessageManager.alliance_event_total_num,MessageManager.item_num_max))
			{
				if((++now_page_num)*10 > MessageManager.AllianceEventMap.Count)
				{
					MessageManager.ReqAllianceEventData((uint)((now_page_num-1)*10),(uint)((now_page_num-1)*10+9));
					LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_MAIL);
				}
				else
				{
					InitialPanelData();
				}
			}	
		}
		else if(gold_event_tab.isChecked)
		{
			if(now_page_num < U3dCmn.GetPageNum(MessageManager.gold_event_total_num,MessageManager.item_num_max))
			{
				if((++now_page_num)*10 > MessageManager.GoldEventMap.Count)
				{
					MessageManager.ReqGoldEventData((uint)((now_page_num-1)*10),(uint)((now_page_num-1)*10+9));
					LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_MAIL);
				}
				else
				{
					InitialPanelData();
				}
			}	
		}
	}
	//上一页 
	void PrevPage()
	{
		if(personal_msg_tab.isChecked)
		{
			if(now_page_num > 1)
			{
				
				if((--now_page_num)*10 > MessageManager.PersonalMsgMap.Count)
				{
					MessageManager.ReqPersonalMsgData((uint)((now_page_num-1)*10),(uint)((now_page_num-1)*10+9));
				}
				else
				{
					InitialPanelData();
				}
			}	
		}
		else if(sys_msg_tab.isChecked)
		{
			if(now_page_num > 1)
			{
				
				if((--now_page_num)*10 > MessageManager.SysMsgMap.Count)
				{
					MessageManager.ReqSysMsgData((uint)((now_page_num-1)*10),(uint)((now_page_num-1)*10+9));
				}
				else
				{
					InitialPanelData();
				}
			}	
		}
		else if(alliance_event_tab.isChecked)
		{
			if(now_page_num > 1)
			{
				if((--now_page_num)*10 > MessageManager.AllianceEventMap.Count)
				{
					MessageManager.ReqAllianceEventData((uint)((now_page_num-1)*10),(uint)((now_page_num-1)*10+9));
				}
				else
				{
					InitialPanelData();
				}
			}	
		}
		else if(gold_event_tab.isChecked)
		{
			if(now_page_num > 1)
			{
				if((--now_page_num)*10 > MessageManager.GoldEventMap.Count)
				{
					MessageManager.ReqGoldEventData((uint)((now_page_num-1)*10),(uint)((now_page_num-1)*10+9));
				}
				else
				{
					InitialPanelData();
				}
			}	
		}
	}
	
	//全部删除 
	void TryDeleteAll()
	{
		string str = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.SRUE_DELETE_ALL_MAIL);
		PopConfirmWin.instance.ShowPopWin(str,DeleteAll);
	}
	void DeleteAll(GameObject obj)
	{
		uint mail_type = 0;
		if(personal_msg_tab.isChecked)
			mail_type = (uint)(MAIL_TYPE.MAIL_TYPE_ALLIANCE | MAIL_TYPE.MAIL_TYPE_PRIVATE);
		else if(sys_msg_tab.isChecked)
			mail_type = (uint)MAIL_TYPE.MAIL_TYPE_SYSTEM;
		else if(alliance_event_tab.isChecked)
			mail_type = (uint)MAIL_TYPE.MAIL_TYPE_ALLIANCE_EVENT;
		else if(gold_event_tab.isChecked)
			mail_type = (uint)MAIL_TYPE.MAIL_TYPE_GOLDORE_EVENT;	
		if(mail_type!=0)
			MessageManager.DeleteAll(mail_type);
	}
	//全部已读 
	void TryReadAll()
	{
		string str = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.SRUE_READ_ALL_MAIL);
		PopConfirmWin.instance.ShowPopWin(str,ReadAll);
	}
	//读取所有邮件 
	void ReadAll(GameObject obj)
	{
		uint mail_type = 0;
		if(personal_msg_tab.isChecked)
			mail_type = (uint)(MAIL_TYPE.MAIL_TYPE_ALLIANCE | MAIL_TYPE.MAIL_TYPE_PRIVATE);
		else if(sys_msg_tab.isChecked)
			mail_type = (uint)MAIL_TYPE.MAIL_TYPE_SYSTEM;
		if(mail_type!=0)
			MessageManager.ReadAll(mail_type);
	}
}
