using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class CupRankWin : MonoBehaviour {
	const int item_num_max = 10;
	//消息窗口positon TWEEN     
	public TweenPosition tween_position; 
	//个人排名TAB  
	public UICheckbox personal_tab;
	//联盟排名TAB 
	public UICheckbox alliance_tab;
	
	public GameObject rank_item;
	public UIDraggablePanel rank_panel;
	public UIGrid rank_grid;
	GameObject[] rank_item_array = new GameObject[item_num_max];
	
	public UILabel page_num;
	int now_page_num = 1;
	bool refresh = false;
	int page_max_num = 0;
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
		for(int i=0;i<item_num_max;i++)
		{
			GameObject obj = NGUITools.AddChild(rank_grid.gameObject,rank_item);
			obj.name = "item"+i;
			obj.SetActiveRecursively(false);
			rank_item_array[i] = obj;
		}
		rank_grid.Reposition();
		now_page_num = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if(refresh)
		{
			GetRankData();
			refresh = false;
		}
	}
	void RevealPanel()
	{
		now_page_num = 1;
		refresh = true;
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		//now_grid = null;
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//面板更换 
	void ChangePanel()
	{
		now_page_num=1;
		GetRankData();
		rank_panel.gameObject.SetActiveRecursively(false);
	}
	void GetRankData()
	{
		
		//rank_panel.gameObject.SetActiveRecursively(false);
		//uint rank_type =(uint)RANK_TYPE.rank_type_char_level; ;
		if(personal_tab.isChecked)
		{
			CTS_GAMECMD_WORLDCITY_RANK_MAN_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_WORLDCITY_RANK_MAN;
			req.byPageNum3 = (int)item_num_max;
			req.byPageIdx4 = (int)(now_page_num-1);
			TcpMsger.SendLogicData<CTS_GAMECMD_WORLDCITY_RANK_MAN_T>(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_WORLDCITY_RANK_MAN);
		}
		else if(alliance_tab.isChecked)
		{
			CTS_GAMECMD_WORLDCITY_RANK_ALLIANCE_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_WORLDCITY_RANK_ALLIANCE;
			req.byPageNum3 = (int)item_num_max;
			req.byPageIdx4 = (int)(now_page_num-1);
			TcpMsger.SendLogicData<CTS_GAMECMD_WORLDCITY_RANK_ALLIANCE_T>(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_WORLDCITY_RANK_ALLIANCE);
		}
		
		//RankManager.GetRankListInfo(rank_type,(uint)((now_page_num-1)*item_num_max),(uint)(item_num_max));
	}
	//请求个人奖杯排名信息返回    
	void GetPersonalRankRst(byte[] buff)
	{
		STC_GAMECMD_WORLDCITY_RANK_MAN_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_WORLDCITY_RANK_MAN_T>(buff);
		//print ("ssssssssssss"+sub_msg.nNum2);
		int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_WORLDCITY_RANK_MAN_T>();
		int data_len = buff.Length - head_len;
		byte[] data_buff = new byte[data_len];
		Array.Copy(buff,head_len,data_buff,0,data_len);
		WorldCityRankMan[] rank_array = (WorldCityRankMan[])DataConvert.ByteToStructArray<WorldCityRankMan>(data_buff,sub_msg.nNum2);
		//PersonalRankList.Clear();
		int from_num = 0;
		int end_num = item_num_max;
		int i=0;
		rank_panel.gameObject.SetActiveRecursively(true);
		for(int n= from_num;n<rank_array.Length;n++)
		{
			//print ("");
			//int index = RankManager.RankDataList.Count - n-1;
			WorldCityRankMan unit = rank_array[n];
			
			rank_item_array[i].SetActiveRecursively(true);
			rank_item_array[i].SendMessage("InitialPersonalData",unit);
			i++;
		}
		for(;i<5;i++)
		{
			WorldCityRankMan unit =new WorldCityRankMan();
			rank_item_array[i].SetActiveRecursively(true);
			rank_item_array[i].SendMessage("InitialPersonalData",unit);
		}
		for(;i<item_num_max;i++)
		{
			rank_item_array[i].SetActiveRecursively(false);
		}
		page_max_num = Mathf.Max(1,sub_msg.nMaxPage3);
		page_num.text = string.Format("{0}/{1}",now_page_num, page_max_num);
		rank_grid.Reposition();
		rank_panel.ResetPosition();
	}
	//请求联盟奖杯信息返回 
	void GetAllianceRankRst(byte[] buff)
	{
		STC_GAMECMD_WORLDCITY_RANK_ALLIANCE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_WORLDCITY_RANK_ALLIANCE_T>(buff);
		int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_WORLDCITY_RANK_ALLIANCE_T>();
		int data_len = buff.Length - head_len;
		byte[] data_buff = new byte[data_len];
		Array.Copy(buff,head_len,data_buff,0,data_len);
		
		int from_num = 0;
		int end_num = item_num_max;
		int i=0;
		rank_panel.gameObject.SetActiveRecursively(true);
		WorldCityRankAlliance[] rank_array = (WorldCityRankAlliance[])DataConvert.ByteToStructArray<WorldCityRankAlliance>(data_buff,sub_msg.nNum2);
		for(int n= from_num;n<rank_array.Length;n++)
		{
			//print ("");
			//int index = RankManager.RankDataList.Count - n-1;
			WorldCityRankAlliance unit = rank_array[n];
			
			rank_item_array[i].SetActiveRecursively(true);
			rank_item_array[i].SendMessage("InitialAllianceData",unit);
			i++;
		}
		for(;i<5;i++)
		{
			WorldCityRankAlliance unit =new WorldCityRankAlliance();
			rank_item_array[i].SetActiveRecursively(true);
			rank_item_array[i].SendMessage("InitialAllianceData",unit);
		}
		for(;i<item_num_max;i++)
		{
			rank_item_array[i].SetActiveRecursively(false);
		}
		page_max_num = Mathf.Max(1,sub_msg.nMaxPage3);
		page_num.text = string.Format("{0}/{1}",now_page_num, page_max_num);
		rank_grid.Reposition();
		rank_panel.ResetPosition();
	}
	//下一页 
	void NextPage()
	{
		
		if(now_page_num < page_max_num)
		{
			now_page_num++;
			GetRankData();
		}	
		
	}
	//上一页 
	void PrevPage()
	{
	
		if(now_page_num > 1)
		{
			now_page_num--;
			GetRankData();
		}	
	}
}
