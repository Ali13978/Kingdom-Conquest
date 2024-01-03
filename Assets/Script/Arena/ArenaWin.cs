using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;

public class ArenaWin : MonoBehaviour {
	//消息窗口positon TWEEN     
	public TweenPosition tween_position; 
	
	
	//竞技场排行榜TAB
	public UICheckbox area_rank_tab;
	//排行榜 
	public GameObject rank_obj;
	public GameObject rank_item;
	public UIDraggablePanel rank_panel;
	public UIGrid rank_grid;
	GameObject[] rank_item_array = new GameObject[ArenaManager.item_num_max];
	//竞技场TAB   
	public UICheckbox area_tab;
	//竞技场 
	public GameObject area_obj;
	public UILabel my_rank;
	public UILabel try_num;
	public UILabel reward_intro_label;
	//可挑战的君主列表 
	GameObject[] challenge_lord_array = new GameObject[ArenaManager.challenge_num];
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
		reward_intro_label.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ARENA_REWARD_INTRO);
		for(int i = 0;i<ArenaManager.challenge_num;i++)
		{
			string item_name = "Lord"+(i+1);
			GameObject obj = U3dCmn.GetChildObjByName(area_obj,item_name);
			challenge_lord_array[i] = obj; 
			ChallengeListUnit item_data= new ChallengeListUnit();
			obj.GetComponent<LordItem>().InitialLordData(item_data);
		}
		for(int i=0;i<ArenaManager.item_num_max;i++)
		{
			GameObject obj = NGUITools.AddChild(rank_grid.gameObject,rank_item);
			obj.name = "item"+i;
			//obj.SetActiveRecursively(false);
			rank_item_array[i] = obj;
		}
		rank_grid.Reposition();
		now_page_num = 1;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void RevealPanel()
	{
		OpenArenaPanel();
		now_page_num = 1;
		
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		//now_grid = null;
		tween_position.Play(false);
		Destroy(gameObject);
		ArenaManager.is_deploy = false;
	}
	//打开竞技场面板 
	void OpenArenaPanel()
	{
		area_obj.SetActiveRecursively(false);
		ReqArenaData();
		
	}
	//请求竞技场数据 
	void ReqArenaData()
	{
		ArenaManager.ReqArenaDataFinish += RefreshArenaUI;
		ArenaManager.Instance.ReqArenaData();
	}
	//刷新竞技场界面  
	void RefreshArenaUI(STC_GAMECMD_ARENA_GET_STATUS_T status,ChallengeListUnit[] challenge_array)
	{
		area_obj.SetActiveRecursively(true);
		my_rank.text = ArenaManager.my_rank.ToString();
		try_num.text = status.nChallengeTimesLeft2.ToString();
	
		for(int i=0;i<ArenaManager.challenge_num;i++)
		{
			if(challenge_array.Length > i)
			{
				int index = challenge_array.Length-i-1;
				challenge_lord_array[i].GetComponent<LordItem>().InitialLordData(challenge_array[index]);
			}
			else 
			{
				ChallengeListUnit item_data= new ChallengeListUnit();
				challenge_lord_array[i].GetComponent<LordItem>().InitialLordData(item_data);
			}
		}
		
	}
	void OpenRankPanel()
	{
		now_page_num = 1;
		ArenaManager.ReqArenaRankDataFinish = RefreshArenaRankUI;
		ArenaManager.Instance.ReqArenaRankData((uint)ArenaManager.item_num_max,(uint)now_page_num-1);
		rank_obj.SetActiveRecursively(false);
		//rank_grid.Reposition();
	}
	//刷新排行榜界面 
	void RefreshArenaRankUI(STC_GAMECMD_ARENA_GET_RANK_LIST_T rank_data,RankListUnit[] rank_array)
	{
		int total_page = U3dCmn.GetPageNum((int)rank_data.nTotalNum2,ArenaManager.item_num_max);
		page_num.text = now_page_num+"/"+total_page;
		print (page_num.text+" "+rank_array.Length);
		for(int i=0;i<ArenaManager.item_num_max;i++)
		{
			if(i<rank_array.Length)
			{
				rank_item_array[i].GetComponent<ArenaRankItem>().InitialRankData(rank_array[i]);
			}
			else
			{
				RankListUnit data = new RankListUnit();
				rank_item_array[i].GetComponent<ArenaRankItem>().InitialRankData(data);
			}
		}
		rank_grid.Reposition();
		rank_panel.ResetPosition();
		rank_obj.SetActiveRecursively(true);
		
	}
	//打开部署军队界面 准备上传镜像 
	void OpenDeployWin()
	{  
		//先请求当前的镜像数据 
		ArenaManager.Instance.ReqDeployData();
		
	}
	//打开购买竞技次数窗口 
	void OpenBuyTryWin()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("BuySingleItemWin");
		if (obj != null)
		{
			BuySingleItemWin.ReqBuyDelegate = ReqBuyTry;
			obj.GetComponent<BuySingleItemWin>().RevealPanel(CommonMB.Arena_Info.TryPrice);
		}
	}
	void ReqBuyTry(uint moneytype,int price)
	{
		BuySingleItemWin.ReqBuyDelegate = null;
		CTS_GAMECMD_ARENA_PAY_T req;
		req.nCmd1     = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ARENA_PAY;
		req.nMoneyType3 = moneytype;
		req.nNum4 = 1;
		TcpMsger.SendLogicData<CTS_GAMECMD_ARENA_PAY_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_ARENA_PAY);
	}
	public void ReqBuyTryRst(byte[] buff)
	{
		U3dCmn.SendMessage("BuySingleItemWin","DismissPanel",null);
		STC_GAMECMD_ARENA_PAY_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_ARENA_PAY_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_ARENA_PAY_T.enum_rst.RST_OK)
		{
			if(sub_msg.nMoneyType2 == (uint)money_type.money_type_diamond)
			{
				CommonData.player_online_info.Diamond	-= sub_msg.nPrice3;
			}
			else if(sub_msg.nMoneyType2 == (uint)money_type.money_type_crystal)
			{
				CommonData.player_online_info.Crystal	-= sub_msg.nPrice3;
			}
			// 刷新一次角色数据  
			PlayerInfoManager.RefreshPlayerDataUI();
			ArenaManager.try_num++;
			try_num.text = ArenaManager.try_num.ToString();
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_ARENA_PAY_T.enum_rst.RST_CRYSTAL_NOT_ENOUGH)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CRYSTAL_NOT_ENOUGH));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_ARENA_PAY_T.enum_rst.RST_DIAMOND_NOT_ENOUGH)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.DIAMOND_NOT_ENOUGH));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_ARENA_PAY_T.enum_rst.RST_FREE_TIMES_LEFT)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.STILL_HAVE_CHALLENGE_TRY));
		}
	}
	//竞技排名下一页 
	void NextPage()
	{
		if(now_page_num < U3dCmn.GetPageNum((int)ArenaManager.total_rank_num,ArenaManager.item_num_max))
		{
			now_page_num++;
			ArenaManager.Instance.ReqArenaRankData((uint)ArenaManager.item_num_max,(uint)now_page_num-1);
			ArenaManager.ReqArenaRankDataFinish = RefreshArenaRankUI;
		}	
	}
	//竞技排名上一页 
	void PrevPage()
	{
		if(now_page_num > 1)
		{
			--now_page_num;
			ArenaManager.Instance.ReqArenaRankData((uint)ArenaManager.item_num_max,(uint)now_page_num-1);
			ArenaManager.ReqArenaRankDataFinish = RefreshArenaRankUI;
		}	
	}
	//打开联盟副本简介 
	void OpenArenaIntroWin()
	{
		GameObject go = U3dCmn.GetObjFromPrefab("BaizBulletinWin");
		if (go == null) return;
		
		BaizBulletinWin win1 = go.GetComponent<BaizBulletinWin>();
		if (win1 != null) 
		{
			float depth = transform.localPosition.z - BaizVariableScript.DEPTH_OFFSET;
			
			win1.Depth(depth);
			win1.ApplyBulletin((int)COMBAT_TYPE.COMBAT_ARENA);
		}
	}
}
