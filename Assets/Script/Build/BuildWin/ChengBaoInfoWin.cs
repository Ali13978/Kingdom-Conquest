using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class ChengBaoInfoWin : MonoBehaviour {
	uint 	AutoID;
	public TweenPosition 	tween_position;
	public UILabel			need_devexp;
	public UILabel			now_devexp;
	public UILabel 			need_gold;
	public UILabel 			now_gold;
	public UILabel 			need_people;
	public UILabel 			now_people;
	public UILabel			need_time;
	public GameObject 		build_panel;
	public GameObject       upgrade_btn;
	public GameObject       accelerate_btn;
	public UILabel			level_top;
	
	string need_building_str;
	string need_time_str;
	ChengBaoUnit now_unit;
	bool refresh = false;
	public struct ChengBaoUnit
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
		need_building_str = need_devexp.text;
		need_time_str = need_time.text;
	}
		
	// Use this for initialization
	void Start () {
	
	}
	void Update () {
		if(refresh)
		{
			InitialPanelData();
			refresh = false;
		}
	}
	//弹出窗口 
	void RevealPanel(ChengBaoUnit unit)
	{
		now_unit = unit;
		refresh = true;
		level_top.gameObject.SetActiveRecursively(false);
		tween_position.Play(true);
	}
	//初始化面板数据 
	void InitialPanelData()
	{
		
		if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.CHENGBAO))
		{
			CommonMB.InitialBuilding();
		}
		else
		{
			//now_unit = unit;
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
			
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.CHENGBAO,(int)(now_unit.level+1));
			if(info.ID== 0)
			{
				build_panel.SetActiveRecursively(false);
				level_top.gameObject.SetActiveRecursively(true);
				return ;
			}
			
			//AutoID = info.BeginID;
			int sec = (int)info.BuildTime; 
			int hour = sec/3600;
			int minute = sec/60%60;
			int second = sec%60;
			need_time.text= string.Format(need_time_str,hour,minute,second);
			
			bool btn_enable = true;
			uint need =0;
			if(CommonMB.ChengBaoDevExp_Map.Contains((int)(now_unit.level+1)))
			{
				ChengBaoDevExp chengbao_unit = (ChengBaoDevExp)CommonMB.ChengBaoDevExp_Map[(int)(now_unit.level+1)];
				need = (uint)chengbao_unit.DevExp;
			}
			else 
			{
				tween_position.Play(true);
				return ;
			}
			uint now = CommonData.player_online_info.Development;
			need_devexp.text = need.ToString();
			now_devexp.text = now.ToString();
			if(now<need)
			{
				btn_enable = false;
				now_devexp.color = new Color(1,0,0,1);
			}
			else
			{
				now_devexp.color = new Color(0,0.38f,0,1);
			}
			
			need = (uint)info.Money;
			now = CommonData.player_online_info.Gold;
			need_gold.text = need.ToString();
			now_gold.text = U3dCmn.GetNumStr(now);
			if(now<need)
			{
				btn_enable = false;
				now_gold.color = new Color(1,0,0,1);
			}
			else
			{
				now_gold.color = new Color(0,0.38f,0,1);
			}
			
			need = (uint)info.Worker; 
			now = CommonData.player_online_info.Population;
			need_people.text = need.ToString();
			now_people.text = U3dCmn.GetNumStr(now);
			if(now<need)
			{
				btn_enable = false;
				now_people.color = new Color(1,0,0,1);
			}
			else
			{
				now_people.color = new Color(0,0.38f,0,1);
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
			
			tween_position.Play(true);
		}
		
	}
	
	//隐藏窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//升级城堡   
	void ReqUpgradeChengBao()
	{
		
		if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.CHENGBAO))
		{
			CommonMB.InitialBuilding();
		}
		else
		{
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.CHENGBAO,1);
			CTS_GAMECMD_OPERATE_UPGRADE_BUILDING_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_UPGRADE_BUILDING;
			req.nAutoID3 = info.BeginID;
			//print ("upgrade bingying"+req.nAutoID3 );
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_UPGRADE_BUILDING_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_UPGRADE_BUILDING);
		}
		// DismissPanel();
	}
	//加速   
	void ReqAccelerateChengBao()
	{
		BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.CHENGBAO,1);
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
}
