using UnityEngine;
using System;
using System.Collections;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class TechnologyWin : MonoBehaviour {
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
	public GameObject soldier1;
	public GameObject soldier2;
	public GameObject soldier3;
	public GameObject soldier4;
	public GameObject soldier5;
	//研究科技面板 
	public UICheckbox	research_tab;
	public GameObject ResearchPanel;

	TechnologyUnit now_unit;
	bool isstart;
	
	public struct TechnologyUnit
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
	}
	
	// Use this for initialization
	void Start () {
		isstart = true;
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
	void RevealPanel(TechnologyUnit unit)
	{
		now_unit = unit;
		isstart = true;
		tween_position.Play(true);
	}
	//初始化数据 
	void InitialData()
	{
		if(building_tab.isChecked)
		{
			if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.YANJIUYUAN))
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
				BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.YANJIUYUAN,(int)(now_unit.level+1));
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
					upgrade_btn.GetComponent<UIImageButton>().target.spriteName = "button1_disable"; 
					upgrade_btn.GetComponent<Collider>().enabled = false;
				}
				else
				{
					upgrade_btn.GetComponent<UIImageButton>().target.spriteName = "button1"; 
					upgrade_btn.GetComponent<Collider>().enabled = true;
				}
			}
		}
		else if(research_tab.isChecked)
		{
			
			foreach(DictionaryEntry de in SoldierManager.SoldierTopMap)
			{
				
				SoldierUnit soldier_unit = (SoldierUnit)de.Value;
				if(soldier_unit.nExcelID1 == (uint)SoldierSort.QIBING)
				{
					soldier1.SendMessage("InitialData",soldier_unit);
				}
				else if(soldier_unit.nExcelID1 == (uint)SoldierSort.QIANGBING)
				{
					soldier2.SendMessage("InitialData",soldier_unit);
				}
				else if(soldier_unit.nExcelID1 == (uint)SoldierSort.JIANSHI)
				{
					soldier3.SendMessage("InitialData",soldier_unit);
				}
				else if(soldier_unit.nExcelID1 == (uint)SoldierSort.GONGBING)
				{
					soldier4.SendMessage("InitialData",soldier_unit);
				}
				else if(soldier_unit.nExcelID1 == (uint)SoldierSort.QIXIE)
				{
					soldier5.SendMessage("InitialData",soldier_unit);
				}
			}
		}
		
	}
	//切换到科技研究面板 
	void TabYanJiuYuan()
	{
		research_tab.isChecked = true;
		InitialData();
	}
	
	//关闭窗口   
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//升级研究院  
	void ReqUpgradeInstitute()
	{
		if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.YANJIUYUAN))
		{
			CommonMB.InitialBuilding();
		}
		else
		{
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.YANJIUYUAN,1);
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
	//加速研究院升级   
	void ReqAccelerateInstitute()
	{
		BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.YANJIUYUAN,1);
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
