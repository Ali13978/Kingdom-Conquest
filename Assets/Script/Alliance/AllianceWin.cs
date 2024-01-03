using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class AllianceWin : MonoBehaviour {
	const int item_num_max = 10;
	//消息窗口positon TWEEN     
	public TweenPosition tween_position; 
	//简介面板  
	public UICheckbox 	announce_tab;
	public UILabel		announce_text;
	public UIImageButton modify_btn;
	//成员面板   
	public UICheckbox 	member_tab;
	public GameObject 	member_item;
	public UIDraggablePanel member_panel;
	public UIGrid member_grid;
	GameObject[] member_item_array = new GameObject[item_num_max];
	//联盟成员长老/成员管事 ....
	public UILabel member_leaders = null;
	public UILabel member_elders = null;
	string member_leaders_profit;
	string member_elders_profit;
	//联盟申请面板    
	public UICheckbox 	apply_tab;
	public GameObject 	apply_item;
	public UIDraggablePanel apply_panel;
	public UIGrid apply_grid;
	GameObject[] apply_item_array = new GameObject[item_num_max];
	//联盟副本面板 
	public UICheckbox	guild_war_tab;
	public GameObject	team_item;
	public UIDraggablePanel guild_war_panel;
	public UIGrid team_grid;
	//联盟福利面板 
	public UICheckbox 	weal_tab;
	public UILabel	profit_label;
	public UILabel	business_num;
	public UILabel	business_time;
	public UILabel  business_countdown_time;
	public UIImageButton	business_btn;
	public UILabel	contribute_num;
	public UILabel	lottery_num;
	public UILabel 	now_lottery_num;
	public UIImageButton    lottery_btn;
	public UIImageButton    contribute_btn;
	uint   now_business_num;
	uint   total_business_num;
	string profit_unit;
		
	public UISprite next_btn;
	public UISprite	pre_btn;
	public UILabel page_num;
	int now_page_num;
	UIGrid now_grid = null;
	bool refresh = false;
	public static bool open_apply = false;
	
	//apply的两个坐标 
	Vector3 apply_left_pos =  new Vector3(56.79f,117f,0f);
	Vector3 apply_right_pos =  new Vector3(130f,117f,0f);
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
		
		if (profit_label != null) {
			profit_unit = profit_label.text;
		}
		
		// 显示当前有多少个副盟主 ...
		if (member_leaders != null) {
			member_leaders_profit = member_leaders.text;
			member_leaders.enabled = false;
		}
		// 显示当前有多少个管事 ....
		if (member_elders != null) {
			member_elders_profit = member_elders.text;
			member_elders.enabled = false;
		}
        GBTeamManager manager = GBTeamManager.Instance;
	}
	// Use this for initialization
	void Start () {
		for(int i=0;i<item_num_max;i++)
		{
			GameObject obj = NGUITools.AddChild(member_grid.gameObject,member_item);
			obj.name = "item"+i;
			obj.SetActiveRecursively(false);
			member_item_array[i] = obj;
			
			obj = NGUITools.AddChild(apply_grid.gameObject,apply_item);
			obj.name = "item"+i;
			obj.SetActiveRecursively(false);
			apply_item_array[i] = obj;
		}
		GBTeamManager.Instance.OnCreateTeamComplete += this.OnCreateTeamComplete;
        GBTeamManager.Instance.OnTeamListGet += this.OnTeamListGetComplete;
        GBTeamManager.Instance.OnMemeberListGet += this.OnMemberListGet;
		now_page_num = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if(refresh)
		{
			InitialPanelData();
			refresh = false;
		}
		if(open_apply)
		{
			apply_tab.isChecked = true;
			InitialPanelData();
			open_apply =  false;
		}
		
	}
	void RevealPanel()
	{
		if(AllianceManager.my_alliance_info.MyPost == (uint)ALLIANCE_POSITION.alliance_position_leader || 
			AllianceManager.my_alliance_info.MyPost == (uint)ALLIANCE_POSITION.alliance_position_vice_leader ||
			AllianceManager.my_alliance_info.MyPost == (uint)ALLIANCE_POSITION.alliance_position_manager)
		{
			//apply_panel.gameObject.SetActiveRecursively(true);
			//apply_tab.transform.localPosition = apply_right_pos;
			apply_tab.gameObject.SetActiveRecursively(true);
			apply_tab.isChecked = false;
			
		}
		else 
		{
			apply_panel.gameObject.SetActiveRecursively(false);
			apply_tab.gameObject.SetActiveRecursively(false);
			//apply_tab.isChecked = false;
		}
		if(U3dCmn.GetCmnDefNum((uint)CMN_DEF.GUILD_WAR) == 0)
		{
			guild_war_tab.isChecked =false;
			guild_war_tab.transform.localPosition += new Vector3(-1000,0,0);
			//guild_war_panel.gameObject.SetActiveRecursively(false);
			apply_tab.transform.localPosition = apply_left_pos;
		}
		else
		{
			apply_tab.transform.localPosition = apply_right_pos;
			guild_war_tab.gameObject.SetActiveRecursively(true);
		}
		now_page_num = 1;
		refresh = true;
		//open_apply = false;
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		now_grid = null;
		tween_position.Play(false);
		Destroy(gameObject);
		GBTeamManager.Instance.OnCreateTeamComplete -= this.OnCreateTeamComplete;
        GBTeamManager.Instance.OnTeamListGet -= this.OnTeamListGetComplete;
        GBTeamManager.Instance.OnMemeberListGet -= this.OnMemberListGet;
	}
	//切换面板 
	void ChangePanel()
	{
		now_page_num = 1;
		InitialPanelData();
	}
	//切换到申请面板 
	//void ChangeApplyPanel()
	//{
	//	apply_tab.isChecked = true;
	//	InitialPanelData();
	//}
	//打开成员面板 
	void OpenMemberPanel()
	{
		AllianceManager.ReqAllianceMember();
		member_panel.gameObject.SetActiveRecursively(false);
	}
	// 管理者数量 ....
	void SetLeaderNumInfo()
	{
		int lv = (int) AllianceManager.my_alliance_info.AllianceLevel;
		if (CommonMB.AllianceInfo_Map.ContainsKey(lv))
		{
			AllianceInfo card = (AllianceInfo) CommonMB.AllianceInfo_Map[lv];
			
			if (member_leaders != null)
			{
				member_leaders.text = string.Format(member_leaders_profit, 
					AllianceManager.my_alliance_info.LeaderNum, card.ViceLeaderNum);
				member_leaders.enabled = true;
			}
			if (member_elders != null)
			{
				member_elders.text = string.Format(member_elders_profit,
					AllianceManager.my_alliance_info.ManagerNum, card.ManagerNum);
				member_elders.enabled = true;
			}
		}
	}
	//打开联盟副本面板 
	void OpenGuildWarPanel()
	{
		if(GBStatusManager.Instance.MyStatus.nInstanceStatus ==  (uint)INSTANCE_STATUS.instance_status_normal)
		{
			//还未开战 进入队伍列表界面 
			guild_war_panel.gameObject.SetActiveRecursively(false);
			next_btn.gameObject.SetActiveRecursively(false);
			pre_btn.gameObject.SetActiveRecursively(false);
			page_num.gameObject.SetActiveRecursively(false);
			guild_war_panel.gameObject.SetActiveRecursively(false);
            GBTeamManager.Instance.RequestTeamList();
            LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_GET_LIST);
		}
		else 
		{
            GameObject embassyWin = U3dCmn.GetObjFromPrefab("DaShiGuanWin");
            if (embassyWin != null) {
                embassyWin.SendMessage("DismissPanel");
            }
            GBTeamManager.Instance.RequestTeamMember(GBStatusManager.Instance.MyStatus.nInstanceID);
            LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_GET_HERO_DATA);
		}
	}
	void InitialPanelData()
	{
		if(announce_tab.isChecked)
		{
			announce_text.text = AllianceManager.my_alliance_info.Introduction;
			next_btn.gameObject.SetActiveRecursively(false);
			pre_btn.gameObject.SetActiveRecursively(false);
			page_num.gameObject.SetActiveRecursively(false);
			if(AllianceManager.my_alliance_info.MyPost == (uint)ALLIANCE_POSITION.alliance_position_leader)
				modify_btn.gameObject.SetActiveRecursively(true);
			else 
				modify_btn.gameObject.SetActiveRecursively(false);
			
		}
		else if(member_tab.isChecked)
		{
			//SortDataByPosition(AllianceManager.AllianceMemberList);
			member_panel.gameObject.SetActiveRecursively(true);
			int member_list_count = AllianceManager.AllianceMemberList.Count;
			if(now_page_num>GetPageNum(member_list_count))
				now_page_num = GetPageNum(member_list_count);
		
			int from_num = (now_page_num-1)*item_num_max;
			int end_num = from_num+item_num_max>member_list_count?member_list_count:from_num+item_num_max;
			
			int i=0;
			for(int n= from_num;n<end_num;n++)
			{
				//int index = member_list_count- n-1;
				AllianceMemberUnit unit = (AllianceMemberUnit)AllianceManager.AllianceMemberList[n];
				member_item_array[i].SetActiveRecursively(true);
				member_item_array[i].SendMessage("InitialData",unit);
				//print(n+" "+DataConvert.BytesToStr(unit.szName3));
				i++;
			}
			for(;i<4;i++)
			{
				AllianceMemberUnit unit =new AllianceMemberUnit();
				member_item_array[i].SetActiveRecursively(true);
				member_item_array[i].SendMessage("InitialData",unit);
			}
			for(;i<item_num_max;i++)
			{
				member_item_array[i].SetActiveRecursively(false);
			}
			
			// 设置管事数量 ....
			SetLeaderNumInfo();

			page_num.text = now_page_num +"/"+GetPageNum(member_list_count);
			member_grid.Reposition();
			member_panel.ResetPosition();
			next_btn.gameObject.SetActiveRecursively(true);
			pre_btn.gameObject.SetActiveRecursively(true);
			page_num.gameObject.SetActiveRecursively(true);
		
		}
		else if(apply_tab.isChecked)
		{
			int apply_list_count = AllianceManager.AllianceApplyList.Count;
			if(now_page_num>GetPageNum(apply_list_count))
				now_page_num = GetPageNum(apply_list_count);
		
			int from_num = (now_page_num-1)*item_num_max;
			int end_num = from_num+item_num_max>apply_list_count?apply_list_count:from_num+item_num_max;
			
			int i=0;
			for(int n= from_num;n<end_num;n++)
			{
				int index = apply_list_count- n-1;
				AllianceJoinEvent unit = (AllianceJoinEvent)AllianceManager.AllianceApplyList[index];
				
				apply_item_array[i].SetActiveRecursively(true);
				apply_item_array[i].SendMessage("InitialData",unit);
				i++;
			}
			for(;i<4;i++)
			{
				AllianceJoinEvent unit =new AllianceJoinEvent();
				apply_item_array[i].SetActiveRecursively(true);
				apply_item_array[i].SendMessage("InitialData",unit);
			}
			for(;i<item_num_max;i++)
			{
				apply_item_array[i].SetActiveRecursively(false);
			}

			page_num.text = now_page_num +"/"+GetPageNum(apply_list_count);
			apply_grid.Reposition();
			apply_panel.ResetPosition();
			next_btn.gameObject.SetActiveRecursively(true);
			pre_btn.gameObject.SetActiveRecursively(true);
			page_num.gameObject.SetActiveRecursively(true);
			
			
		}
		else if(weal_tab.isChecked)
		{
			profit_label.text = "";
			business_num.text = "";
			business_time.text = "";
			contribute_num.text = "";
			lottery_num.text = "";
			next_btn.gameObject.SetActiveRecursively(false);
			pre_btn.gameObject.SetActiveRecursively(false);
			page_num.gameObject.SetActiveRecursively(false);
			business_btn.gameObject.SetActiveRecursively(false);
			//lottery_btn.gameObject.SetActiveRecursively(false);
			//contribute_btn.gameObject.SetActiveRecursively(false);
			//请求联盟跑商数据 
			CTS_GAMECMD_GET_ALLIANCE_TRADE_INFO_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_ALLIANCE_TRADE_INFO;
			TcpMsger.SendLogicData<CTS_GAMECMD_GET_ALLIANCE_TRADE_INFO_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_ALLIANCE_TRADE_INFO);
			//请求贡献值数据 
		}
		else if(guild_war_tab.isChecked)
		{	
			guild_war_panel.gameObject.SetActiveRecursively(true);
			foreach(Transform obj in team_grid.gameObject.transform)
			{
				obj.gameObject.active = false;
				Destroy(obj.gameObject);
			}
			List<GBTeamListItem> result = GBTeamManager.Instance.TeamList;
			int i = 0;
			for (int max =result.Count; i < max; ++i)
			{
				GameObject obj = NGUITools.AddChild(team_grid.gameObject,team_item);
				string obj_name = string.Format("item{0:D2}",i);
				obj.name = obj_name;
	            GBTeamItemUI teamItemUI = obj.GetComponent<GBTeamItemUI>();
	            teamItemUI.SetTeamInfo(result[i]);
       		}
			for(;i<4;i++)
			{
				GameObject obj = NGUITools.AddChild(team_grid.gameObject,team_item);
				string obj_name = string.Format("item{0:D2}",i);
				obj.name = obj_name;
				GBTeamListItem item_data = new GBTeamListItem();
	            GBTeamItemUI teamItemUI = obj.GetComponent<GBTeamItemUI>();
	            teamItemUI.SetTeamInfo(item_data);
			}
			
			team_grid.Reposition();
			guild_war_panel.ResetPosition();
		}
	
	}
	//初始化联盟福利界面  
	void InitialWealUI(byte[] buff)
	{
		STC_GAMECMD_GET_ALLIANCE_TRADE_INFO_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_ALLIANCE_TRADE_INFO_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_ALLIANCE_TRADE_INFO_T.enum_rst.RST_OK)
		{
			now_business_num = sub_msg.nTradeNum4;
			total_business_num = sub_msg.nExcelTradeNum5;
			if(sub_msg.nCountDownTime2 == 0)
			{
				profit_label.text = sub_msg.nExcelTradeGold7+" "+profit_unit;
				business_num.text = (sub_msg.nExcelTradeNum5-sub_msg.nTradeNum4)+"/"+sub_msg.nExcelTradeNum5;
				business_time.text = U3dCmn.GetTimeStrFromSec((int)sub_msg.nExcelTradeTime6);
				business_btn.gameObject.SetActiveRecursively(true);
				business_countdown_time.text = "";
				if(sub_msg.nTradeNum4<=0)
				{
					business_btn.GetComponent<Collider>().enabled = false;
					business_btn.target.spriteName = "button1_disable"; 
				}
				else 
				{
					business_btn.GetComponent<Collider>().enabled = true;
					business_btn.target.spriteName = "button1"; 
				}
			}
			else 
			{
				profit_label.text = sub_msg.nExcelTradeGold7+" "+profit_unit;
				business_num.text =(sub_msg.nExcelTradeNum5-sub_msg.nTradeNum4)+"/"+sub_msg.nExcelTradeNum5;
				business_time.text = U3dCmn.GetTimeStrFromSec((int)sub_msg.nExcelTradeTime6);
				business_btn.gameObject.SetActiveRecursively(false);
				StopCoroutine("Countdown");
				StartCoroutine("Countdown",(int)(DataConvert.DateTimeToInt(DateTime.Now)+sub_msg.nCountDownTime2));
			}
			InitialContributeUI();
		}
	}
	//初始化捐献界面 
	void InitialContributeUI()
	{
		contribute_num.text = AllianceManager.my_alliance_info.NowContribute.ToString();
		lottery_num.text = (AllianceManager.my_alliance_info.NowContribute/ CommonMB.Lottery_Info.AllianceLotteryPrice).ToString();
		now_lottery_num.text = (CommonMB.Lottery_Info.AllianceLotteryNum - CommonData.player_online_info.AllianceLotteryNum)+"/"+CommonMB.Lottery_Info.AllianceLotteryNum;
	}
	//开始跑商 
	void StartBusiness()
	{
		CTS_GAMECMD_OPERATE_ALLIANCE_TRADE_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_ALLIANCE_TRADE;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_ALLIANCE_TRADE_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ALLIANCE_TRADE);
	}
	//开始跑商返回  
	void StartBusinessRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_ALLIANCE_TRADE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_ALLIANCE_TRADE_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_ALLIANCE_TRADE_T.enum_rst.RST_OK)
		{
			business_num.text = (total_business_num-(--now_business_num))+"/"+total_business_num;
			business_btn.gameObject.SetActiveRecursively(false);
			StopCoroutine("Countdown");
			StartCoroutine("Countdown",(int)(DataConvert.DateTimeToInt(DateTime.Now)+sub_msg.nCountDownTime2));
		}
	}
	//下一页 
	void NextPage()
	{
		if(member_tab.isChecked)
		{
			if(now_page_num < GetPageNum(AllianceManager.AllianceMemberList.Count))
			{
				now_page_num++;
				InitialPanelData();
			}	
		}
		else if(apply_tab.isChecked)
		{
			if(now_page_num < GetPageNum(AllianceManager.AllianceApplyList.Count))
			{
				now_page_num++;
				InitialPanelData();
			}	
		}
	}
	//上一页 
	void PrevPage()
	{
		if(now_page_num > 1)
		{
			now_page_num--;
			InitialPanelData();
		}	
		
	}

	//获取页数 
	int GetPageNum(int num)
	{
		int pagenum = 1;
		if(num>0)
			pagenum = (num-1)/item_num_max+1;
			
		return pagenum;
	}

	
	//打开修改联盟简介窗口 
	void OpenAnnounceWin()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("AllianceAnnounceWin");
		if (obj != null)
		{
			obj.SendMessage("RevealPanel",announce_text.text);
		}
	}
	//跑商倒计时 
	IEnumerator Countdown(int EndTimeSec)
	{
		int sec = (int)(EndTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
		
		if(sec <0)
			sec = 0;
		while(sec!=0)
		{
			 sec =(int)(EndTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
			if(sec <0)
				sec = 0;
			int hour = sec/3600;
			int minute = sec/60%60;
			int second = sec%60;
			if(hour>=100)
				business_countdown_time.text =string.Format("{0}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			else
				business_countdown_time.text =string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			yield return new WaitForSeconds(1);
		}
	}
	//打开捐献金币窗口    
	void OpenContributeWin()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("AllianceContributeWin");
		if (obj != null)
		{
			obj.SendMessage("RevealPanel");
		}
		
	}
	//打开抽奖窗口   
	void OpenLotteryWin()
	{
		if(CommonData.player_online_info.AllianceLotteryNum <=0)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALLIANCE_LOTTERY_ISFULL));
			return;
		}
		GameObject win = U3dCmn.GetObjFromPrefab("ZhuanPanAlliance"); 
		if(win != null)
		{
			win.SendMessage("RevealPanel");
		}
	}
	//我的联盟副本队伍  
	void EnterMyTeam()
	{
        //如果用户没有参加任何队伍的话，则instanceID为0
        if (GBStatusManager.Instance.MyStatus.nInstanceID == 0)
        {
            U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NOT_IN_GB_TEAM));
        }
        else { 
            GameObject obj = U3dCmn.GetObjFromPrefab(GBWindowPath.GBTeamWin);
            GBTeamWin win = obj.GetComponent<GBTeamWin>();
            GBTeamListItem item = GBTeamManager.Instance.GetTeamData();
            win.InitTeamInfo(item);
        }
        
	}
	#region 回调函数 
    void OnTeamListGetComplete(List<GBTeamListItem> list) {
        InitialPanelData();
    }

    void OnMemberListGet(List<GBTeamMemberItem> list)
    {
        if (GBStatusManager.Instance.MyStatus.nInstanceStatus != (uint)INSTANCE_STATUS.instance_status_normal)
        {
            //删除之前的两个窗口
            GameObject embassyWin = U3dCmn.GetObjFromPrefab("DashiGuanWin");
            if (embassyWin != null) {
                embassyWin.SendMessage("DismissPanel");
            }
            GameObject allianceWin = U3dCmn.GetObjFromPrefab("AllianceWin");
            if (allianceWin != null) {
                allianceWin.SendMessage("DismissPanel");
            }
            //进入战斗场景 获取当前副本战斗数据
            GameObject obj = U3dCmn.GetObjFromPrefab(GBWindowPath.GBLevelWin);
        }
    }

    /// <summary>
    /// 加入队伍
    /// </summary>
    /// <param name="teamID"></param>
    public void JoinTeam(GBTeamListItem team)
    {
        GameObject obj = U3dCmn.GetObjFromPrefab(GBWindowPath.GBTeamWin);
        NGUITools.AddChild(obj);
        GBTeamWin win = obj.GetComponent<GBTeamWin>();
        if (win != null)
        {
            win.InitTeamInfo(team);
        }
    }

	/// <summary>
	/// 创建队伍
	/// </summary>
	void CreateGuildWarTeam()
	{
		GBTeamManager.Instance.CreateTeam();
        LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_CREATE);
	}
    private GBTeamListItem _joinedTeam;
    
    /// <summary>
    /// 创建队伍列表回调
    /// </summary>
    void OnCreateTeamComplete(STC_GAMECMD_ALLI_INSTANCE_CREATE_T result) {
        STC_GAMECMD_ALLI_INSTANCE_CREATE_T.enum_rst val = (STC_GAMECMD_ALLI_INSTANCE_CREATE_T.enum_rst)result.nRst1;
        if (val == STC_GAMECMD_ALLI_INSTANCE_CREATE_T.enum_rst.RST_OK)
        {
            ulong teamID = result.nInstanceID2;
            this.InitialPanelData();
            GameObject obj = U3dCmn.GetObjFromPrefab(GBWindowPath.GBTeamWin);
            NGUITools.AddChild(obj);
            GBTeamWin win = obj.GetComponent<GBTeamWin>();
            if (win != null)
            {
                CMN_PLAYER_CARD_INFO info = CommonData.player_online_info;
                GBTeamListItem item = new GBTeamListItem
                {
                    JoinedCount = 1,
                    LordID = info.AccountID,
                    LordName = info.AccountName,
                    TeamID = teamID
                };
                win.InitTeamInfo(item);
            }
        }
        else{
            string text = "";
            switch(val){
                case STC_GAMECMD_ALLI_INSTANCE_CREATE_T.enum_rst.RST_EXISTS_INSTANCE: //有副本未退出  
                    text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_TEAM_NOT_EXIT);
                break;
                case STC_GAMECMD_ALLI_INSTANCE_CREATE_T.enum_rst.RST_NO_TIMES_LEFT: //没有可用次数了 
                text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_TIMES_USEDUP);
                break;
            }
            if(text != ""){
                U3dCmn.ShowWarnWindow(text);
            }
        }
    }
    #endregion
	//打开联盟副本简介 
	void OpenGuildWarIntroWin()
	{
		GameObject go = U3dCmn.GetObjFromPrefab("BaizBulletinWin");
		if (go == null) return;
		
		BaizBulletinWin win1 = go.GetComponent<BaizBulletinWin>();
		if (win1 != null) 
		{
			float depth = transform.localPosition.z - BaizVariableScript.DEPTH_OFFSET;
			
			win1.Depth(depth);
			win1.ApplyBulletin((int)COMBAT_TYPE.COMBAT_INSTANCE_GUILD);
		}
	}
}
