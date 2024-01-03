using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class ChatWin : MonoBehaviour {
	public TweenPosition tween_position; 
	public UIInput input_text;
	public UIDraggablePanel chat_panel;
	public GameObject chat_item;
	public UITable chat_table;
	public UIScrollBar chat_scroll_bar;
	//全部频道 
	public UICheckbox all_channel_tab;
	//世界频道  
	public UICheckbox world_channel_tab;
	//联盟频道  
	public UICheckbox alliance_channel_tab;
	//队伍频道 
	public UICheckbox team_channel_tab;
	//私聊频道  
	public UICheckbox private_channel_tab;
	bool front = false;
	
	//聊天ITEM数组 
	GameObject[] chat_item_array = new GameObject[ChatManager.chat_item_max];
	public UISlicedSprite select_bg = null;
	int index = 0;
	
	void Awake()
	{
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			transform.localScale = CommonData.iphonescale;
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			transform.localScale =  new Vector3(0.0057f,0.0057f,0.0057f);
		}
		
	}
	// Use this for initialization
	void Start () {
		for(int i=0;i<ChatManager.chat_item_max;i++)
		{
			GameObject obj = NGUITools.AddChild(chat_table.gameObject,chat_item);
			obj.name = string.Format("item{0:D2}", i);
			//obj.SetActiveRecursively(false);
			chat_item_array[i] = obj;
		}
		
		gameObject.SetActiveRecursively(false);
		chat_panel.ResetPosition();
	}
	
	void RevealPanel()
	{
		tween_position.Play(true);
		ChangeChannel();
		front = true;
	}
	//隐藏窗口 
	void DismissPanel()
	{
		front = false;
		tween_position.Play(false);
		
		//gameObject.transform.localPosition = new Vector3(5.0f,-2.0f,-1);
		//chat_panel.ResetPosition();
	}
	void ResetWinPosition()
	{
		if(!front)
		{
			gameObject.SetActiveRecursively(false);
			gameObject.transform.localPosition = new Vector3(5.0f,-2.0f,-1);
			
		}
		chat_table.Reposition();
		chat_panel.ResetPosition();
		chat_scroll_bar.scrollValue = 1.0f;	
	}
	//发送聊天信息 
	void SendChatData()
	{
		if(input_text.text == "")
		{
			return ;
		}
		if(ChatManager.chat_channel ==  (int)CHATCHANNEL.ALL_CHANNEL || ChatManager.chat_channel ==  (int)CHATCHANNEL.WORLD_CHANNEL)
		{
			ChatManager.SendWorldData(input_text.text);
		}
		else if(ChatManager.chat_channel ==  (int)CHATCHANNEL.ALLIANCE_CHANNEL)
		{
			ChatManager.SendAlliancedData(input_text.text);
		}
		else if(ChatManager.chat_channel ==  (int)CHATCHANNEL.TEAM_CHANNEL)
		{
			ChatManager.SendTeamData(input_text.text);
		} 
		else if(ChatManager.chat_channel ==  (int)CHATCHANNEL.PRIVATE_CHANNEL)
		{
		
			if(ChatManager.to_char_id != 0 && ChatManager.to_char_id!=CommonData.player_online_info.AccountID)
			{
				string title = ChatManager.to_char_name+":";
				if(input_text.text.Length>=title.Length)
				{
					string str = input_text.text.Substring(0,title.Length);
					if(str == title)
					{
						ChatManager.SendPrivateData(input_text.text.Substring(title.Length,input_text.text.Length-title.Length));
					}
					else
						ChatManager.SendPrivateData(input_text.text);
				}
				else 
					ChatManager.SendPrivateData(input_text.text);
			}
				
		}
		if(ChatManager.chat_channel == (int)CHATCHANNEL.PRIVATE_CHANNEL)
		{
			input_text.text = ChatManager.to_char_name+":";
		}
		else
		{
			input_text.text = "";
		}
	}
	//更换频道 
	public void ChangeChannel()
	{
		if(all_channel_tab.isChecked)
		{
			input_text.text = "";
			//ChatManager.to_char_id = 0;
			ChatManager.chat_channel = (int)CHATCHANNEL.ALL_CHANNEL;
		}
		else if(world_channel_tab.isChecked)
		{
			input_text.text = "";
			//ChatManager.to_char_id = 0;
			ChatManager.chat_channel = (int)CHATCHANNEL.WORLD_CHANNEL;
		}
		else if(alliance_channel_tab.isChecked)
		{
			input_text.text = "";
			//ChatManager.to_char_id = 0;
			ChatManager.chat_channel = (int)CHATCHANNEL.ALLIANCE_CHANNEL;
		}
		else if(team_channel_tab.isChecked)
		{
			input_text.text = "";
			//ChatManager.to_char_id = 0;
			ChatManager.chat_channel = (int)CHATCHANNEL.TEAM_CHANNEL;
		}
		else if(private_channel_tab.isChecked)
		{
			input_text.text = ChatManager.to_char_name+":";
			ChatManager.chat_channel = (int)CHATCHANNEL.PRIVATE_CHANNEL;
		}
		RefreshUI();
	}
	//收到聊天信息 刷新界面 
	public void RefreshUI()
	{
		int index=0;
		if(all_channel_tab.isChecked)
		{
			//input_text.text = "";
			//ChatManager.chat_chanel = (int)CHATCHANEL.ALL_CHANEL;
			for(int i=ChatManager.AllChatDataList.Count;i<6;i++)
			{
				CHAT_DATA unit = new CHAT_DATA();
				chat_item_array[index].SetActiveRecursively(true);
				chat_item_array[index].SendMessage("InitialData",unit);
				index++;
			}
			foreach(CHAT_DATA chat_unit in ChatManager.AllChatDataList)
			{
				chat_item_array[index].SetActiveRecursively(true);
				chat_item_array[index].SendMessage("InitialData",chat_unit);
				index++;
			}
			for(;index<ChatManager.chat_item_max;index++)
			{
				chat_item_array[index].gameObject.SetActiveRecursively(false);
			}
		}
		else if(world_channel_tab.isChecked)
		{
			//input_text.text = "";
			//ChatManager.chat_chanel = (int)CHATCHANEL.WORLD_CHANEL;
			for(int i=ChatManager.WorldChatDataList.Count;i<6;i++)
			{
				CHAT_DATA unit = new CHAT_DATA();
				chat_item_array[index].SetActiveRecursively(true);
				chat_item_array[index].SendMessage("InitialData",unit);
				index++;
			}
			foreach(CHAT_DATA chat_unit in ChatManager.WorldChatDataList)
			{
				chat_item_array[index].SetActiveRecursively(true);
				chat_item_array[index].SendMessage("InitialData",chat_unit);
				index++;
			}
			for(;index<ChatManager.chat_item_max;index++)
			{
				chat_item_array[index].gameObject.SetActiveRecursively(false);
			}
		}
		else if(alliance_channel_tab.isChecked)
		{
			//input_text.text = "";
			//ChatManager.chat_chanel = (int)CHATCHANEL.ALLIANCE_CHANEL;
			for(int i=ChatManager.AllianceChatDataList.Count;i<6;i++)
			{
				CHAT_DATA unit = new CHAT_DATA();
				chat_item_array[index].SetActiveRecursively(true);
				chat_item_array[index].SendMessage("InitialData",unit);
				index++;
			}
			foreach(CHAT_DATA chat_unit in ChatManager.AllianceChatDataList)
			{
				chat_item_array[index].SetActiveRecursively(true);
				chat_item_array[index].SendMessage("InitialData",chat_unit);
				index++;
			}
			for(;index<ChatManager.chat_item_max;index++)
			{
				chat_item_array[index].gameObject.SetActiveRecursively(false);
			}
		}
		else if(team_channel_tab.isChecked)
		{
			//input_text.text = "";
			//ChatManager.chat_chanel = (int)CHATCHANEL.ALLIANCE_CHANEL;
			for(int i=ChatManager.TeamChatDataList.Count;i<6;i++)
			{
				CHAT_DATA unit = new CHAT_DATA();
				chat_item_array[index].SetActiveRecursively(true);
				chat_item_array[index].SendMessage("InitialData",unit);
				index++;
			}
			foreach(CHAT_DATA chat_unit in ChatManager.TeamChatDataList)
			{
				chat_item_array[index].SetActiveRecursively(true);
				chat_item_array[index].SendMessage("InitialData",chat_unit);
				index++;
			}
			for(;index<ChatManager.chat_item_max;index++)
			{
				chat_item_array[index].gameObject.SetActiveRecursively(false);
			}
		}
		else if(private_channel_tab.isChecked)
		{
			//input_text.text = ChatManager.to_char_name+":";
			//ChatManager.chat_chanel = (int)CHATCHANEL.PRIVATE_CHANEL;
			for(int i=ChatManager.PrivateChatDataList.Count;i<6;i++)
			{
				CHAT_DATA unit = new CHAT_DATA();
				chat_item_array[index].SetActiveRecursively(true);
				chat_item_array[index].SendMessage("InitialData",unit);
				index++;
			}
			foreach(CHAT_DATA chat_unit in ChatManager.PrivateChatDataList)
			{
				chat_item_array[index].SetActiveRecursively(true);
				chat_item_array[index].SendMessage("InitialData",chat_unit);
				index++;
			}
			for(;index<ChatManager.chat_item_max;index++)
			{
				chat_item_array[index].gameObject.SetActiveRecursively(false);
			}
		}
		//if()
		
		chat_table.Reposition();
		chat_panel.ResetPosition();
		chat_scroll_bar.scrollValue = 1.0f;
	}
	public void RefreshSelectBg(UISlicedSprite bg)
	{
		if(select_bg != null)
		{
			select_bg.spriteName = "sliced_bg2";
		}
		select_bg = bg;
	}
	public void ClearSelectBg()
	{
		select_bg.spriteName = "sliced_bg2";
	}
	void RefreshPrivateChat()
	{
		input_text.text = ChatManager.to_char_name+":";
	}
}
