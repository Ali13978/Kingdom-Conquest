using UnityEngine;
using System;
using System.Collections;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class DashiGuanWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	//建筑信息面板  
	public UICheckbox		building_tab;
	public UILabel			need_building;
	public UILabel 			need_gold;
	public UILabel 			need_people;
	public UILabel			need_time;
	public GameObject       upgrade_btn;
	public GameObject       accelerate_btn;
	string need_time_str;
	//大使馆面板 
	public UICheckbox		allianceinfo_tab;
	public GameObject 		no_alliance_obj;
	public GameObject 		applying_alliance_obj;
	public GameObject 		have_alliance_obj;
	//没有联盟信息板  
	public UIInput			alliance_name;
	public UILabel			describe_label;
	string describe_str;
	//申请中信息板  
	public UILabel 			apply_alliance_name;
	//已加入联盟信息板  
	public GameObject		leader_btn;
	public GameObject		member_btn;
	public UILabel			belong_alliance_name;
	public UILabel			leader_label;
	public UILabel			member_num_label;
	public UILabel			development_label;
	//public UILabel			introduction_label;
	public UILabel 			level_label;
	public UILabel			rank_label;
	//public UILabel RefreshTime;
	//public UILabel JiangLingNum; 
	//酒馆武将缓存 
	//ArrayList JiangLingArray = new ArrayList();
	
	DashiGuanUnit now_unit;
	bool isstart;
	
	public struct DashiGuanUnit
	{
		public uint level;
		public uint build_state;
		public int 	build_end_time;
	}
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
		//记录一下初始格式化字符串   
		need_time_str = need_time.text;
		describe_str  = describe_label.text;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//需要OBJ初始化在START函数之后执行的函数  
		if(isstart)
		{
			InitialData();
			isstart =false;
		}
	}
	//弹出窗口   
	void RevealPanel(DashiGuanUnit unit)
	{
		now_unit = unit;
		isstart = true;
		tween_position.Play(true);
	}
	//关闭窗口   
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//初始化数据 
	void InitialData()
	{
		if(building_tab.isChecked)
		{
			if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.DASHIGUAN))
			{
				CommonMB.InitialBuilding();
			}
			else
			{
				if(now_unit.build_state == (uint)BuildingState.NORMAL)
				{
					accelerate_btn.SetActiveRecursively(false);
					upgrade_btn.SetActiveRecursively(true);
				}
				else if(now_unit.build_state == (uint)BuildingState.UPGRADING)
				{
					accelerate_btn.SetActiveRecursively(true);
					upgrade_btn.SetActiveRecursively(false);
				}
				BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.DASHIGUAN,(int)(now_unit.level+1));
				int sec = (int)info.BuildTime; 
				int hour = sec/3600;
				int minute = sec/60%60;
				int second = sec%60;
				need_time.text= string.Format(need_time_str,hour,minute,second);
				
				bool btn_enable = true;
				uint need = now_unit.level+1;
				uint now = ChengBao.NowLevel;
				need_building.text = need + "/"+now;
				if(now<need)
				{
					btn_enable = false;
					need_building.color = new Color(1,0,0,1);
				}
				else 
				{
					need_building.color =  new Color(0,0.38f,0,1);
				}
				
				need = (uint)info.Money;
				now = CommonData.player_online_info.Gold;
				need_gold.text = need + "/"+U3dCmn.GetNumStr(now);
				if(now<need)
				{
					btn_enable = false;
					need_gold.color = new Color(1,0,0,1);
				}
				else 
				{
					need_gold.color =  new Color(0,0.38f,0,1);
				}
				
				need = (uint)info.Worker; 
				now = CommonData.player_online_info.Population;
				need_people.text = need + "/"+U3dCmn.GetNumStr(now);
				if(now<need)
				{
					btn_enable = false;
					need_people.color = new Color(1,0,0,1);
				}
				else 
				{
					need_people.color =  new Color(0,0.38f,0,1);
				}
				
				if(!btn_enable)
				{
					upgrade_btn.GetComponent<Collider>().enabled = false;
					upgrade_btn.GetComponent<UIImageButton>().target.spriteName = "button1_disable"; 
				}
				else
				{
					upgrade_btn.GetComponent<Collider>().enabled = true;
					upgrade_btn.GetComponent<UIImageButton>().target.spriteName = "button1"; 
				}
			}
		}
		else if(allianceinfo_tab.isChecked)
		{
			no_alliance_obj.SetActiveRecursively(false);
			have_alliance_obj.SetActiveRecursively(false);
			applying_alliance_obj.SetActiveRecursively(false);
			AllianceManager.ReqAllianceInfo();
		}
	}
	//当前处于无联盟状态 
	public void SetUINoAlliance()
	{
		no_alliance_obj.SetActiveRecursively(true);
		have_alliance_obj.SetActiveRecursively(false);
		applying_alliance_obj.SetActiveRecursively(false);
		describe_label.text = string.Format(describe_str,CommonMB.CreateAllianceGold/1000);
		alliance_name.text = "";
	}
	//当前处于联盟申请中 
	public void SetUIApplying()
	{
		applying_alliance_obj.SetActiveRecursively(true);
		no_alliance_obj.SetActiveRecursively(false);
		have_alliance_obj.SetActiveRecursively(false);
		apply_alliance_name.text  = AllianceManager.apply_alliance_name;
	}
	//当前处于已加入联盟状态  
	void SetUIHaveAlliance(ALLIANCE_INFO alliance_info)
	{
		have_alliance_obj.SetActiveRecursively(true);
		no_alliance_obj.SetActiveRecursively(false);
		applying_alliance_obj.SetActiveRecursively(false);
		belong_alliance_name.text = alliance_info.AllianceName;
		leader_label.text = alliance_info.LeaderName;
		
		level_label.text = alliance_info.AllianceLevel.ToString();
		member_num_label.text = alliance_info.MemberNum+"/"+alliance_info.MemberNumMax;
		rank_label.text = alliance_info.AllianceRank.ToString();
		int next_level = (int)alliance_info.AllianceLevel+1;
		if(CommonMB.AllianceInfo_Map.Contains(next_level))
		{
			AllianceInfo info = (AllianceInfo)CommonMB.AllianceInfo_Map[next_level];
			development_label.text = alliance_info.Development+"/"+info.Development;
		}
		else 
		{
			development_label.text = alliance_info.Development.ToString();
		}
		//introduction_label.text = alliance_info.Introduction;
		if(alliance_info.MyPost == (uint)ALLIANCE_POSITION.alliance_position_leader)
		{
			leader_btn.gameObject.SetActiveRecursively(true);
			member_btn.gameObject.SetActiveRecursively(false);
		}
		else 
		{
			leader_btn.gameObject.SetActiveRecursively(false);
			member_btn.gameObject.SetActiveRecursively(true);
		}
	}
	//升级大使馆   
	void ReqUpgradeDashiGuan()
	{
		if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.DASHIGUAN))
		{
			CommonMB.InitialBuilding();
		}
		else
		{
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.DASHIGUAN,1);
			CTS_GAMECMD_OPERATE_UPGRADE_BUILDING_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_UPGRADE_BUILDING;
			req.nAutoID3 = info.BeginID;
			//print ("upgrade bingying"+req.nAutoID3 );
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_UPGRADE_BUILDING_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_UPGRADE_BUILDING);
		}
		DismissPanel();
	}
	//加速大使馆升级    
	void ReqAccelerateDashiGuan()
	{
		BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.DASHIGUAN,1);
		uint AutoID = info.BeginID;
		AccelerateWin.AccelerateUnit unit;
		unit.Type = (int)enum_accelerate_type.building;
		unit.BuildingType = (uint)te_type_building.te_subtype_building_upgrade; 
		unit.autoid = AutoID;
		unit.EndTimeSec = now_unit.build_end_time;
		GameObject win = U3dCmn.GetObjFromPrefab("AccelerateWin"); 
		if(win != null)
		{
			win.SendMessage("RevealPanel",unit);
			AccelerateWin.MotherWin = gameObject;
			AccelerateWin.CloseCallBack = "DismissPanel";
		}
	}
	//创建联盟 
	void CreateAlliance()
	{
		if(alliance_name.text=="")
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALLIANCE_NAME_IS_NULL));
		}
		else
		{
			AllianceManager.ReqCreateAlliance(alliance_name.text);
		}
	}
	//申请加入联盟 
	void ApplyJoinAlliance()
	{
		byte[] alliance_name_array = DataConvert.StrToBytes(alliance_name.text);
		if(alliance_name.text=="")
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALLIANCE_NAME_IS_NULL));
		}
		else if(alliance_name_array.Length >32)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NAME_TOO_LONG));
		}
		else
		{
			AllianceManager.ReqJoinAlliance(alliance_name_array);
		}
	}
	//撤销申请加入联盟 
	void RecallAllianceApply()
	{
		string str = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.SURE_CANCEL_ALLIANCE_APPLY);
		PopConfirmWin.instance.ShowPopWin(str,SureRecallAllianceApply);
	}
	void SureRecallAllianceApply(GameObject obj)
	{
		AllianceManager.ReqRecallApply();
	}
	//解散联盟  
	void DismissAlliance()
	{
		string str = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.SURE_DISMISS_ALLIANCE);
		PopConfirmWin.instance.ShowPopWin(str,SureDismissAlliance);
		
	}
	void SureDismissAlliance(GameObject obj)
	{
		AllianceManager.ReqDismissAlliance();
	}
	//退出联盟 
	void QuitAlliance()
	{
		string str = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.SURE_QUIT_ALLIANCE);
		PopConfirmWin.instance.ShowPopWin(str,SureQuitAlliance);
		
	}
	void SureQuitAlliance(GameObject obj)
	{
		AllianceManager.ReqQuitAlliance();
	}
	//打开联盟界面 
	void OpenAllianceWin()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("AllianceWin");
		if (obj != null)
		{
			obj.SendMessage("RevealPanel");
		}
	}
	//打开禅让盟主界面 
	void OpenDemiseWin()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("DemiseWin");
		if (obj != null)
		{
			obj.SendMessage("RevealPanel");
		}
	}
	//打开联盟列表 
	void OpenAllianceListWin()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("AllianceListWin");
		if (obj != null)
		{
			obj.SendMessage("RevealPanel");
		}
	}
	//发送联盟邮件 
	void SendAllianceMail()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("WriteMessageWin");
		if (obj != null)
		{
			WriteMessageWin win = obj.GetComponent<WriteMessageWin>(); 
			win.char_id =0;
			win.recv_charname.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALLIANCE_MAIL_EXPLAIN);
			win.MailType = (byte)MAIL_TYPE.MAIL_TYPE_ALLIANCE;
			//win.isAllianceMail = true;
			obj.SendMessage("RevealPanel");
		}
	}
}
