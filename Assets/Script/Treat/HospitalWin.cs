using UnityEngine;
using System;
using System.Collections;
using CMNCMD;
using CTSCMD;
using STCCMD;

public class HospitalWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	//建筑信息面板  ...
	public UICheckbox		building_tab;
	public UILabel			need_building;
	public UILabel 			need_gold;
	public UILabel 			need_people;
	public UILabel			need_time;
	public GameObject       upgrade_btn;
	public GameObject       accelerate_btn;
	string need_time_str;
	//治疗面板 ...
	public	UICheckbox		treat_tab;
	public	GameObject 		hurt_hero_item;
	public	UIDraggablePanel treat_panel;
	public	UIGrid 			treat_grid;
	public 	UILabel			my_caoyao;
	public	UILabel 		need_caoyao;
	public  UILabel			production_label;
	ArrayList hurt_hero_list = new ArrayList();
	int total_caoyao; // 全部治疗须要花费多少草药 ...
	HospitalUnit now_unit;
	bool isstart;
	public struct HospitalUnit
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
	void RevealPanel(HospitalUnit unit)
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
			if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.YIGUAN))
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
				BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.YIGUAN,(int)(now_unit.level+1));
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
		else if(treat_tab.isChecked)
		{
			total_caoyao = 0;
			hurt_hero_list.Clear();
			foreach(DictionaryEntry de in JiangLingManager.MyHeroMap)
			{
				HireHero hero_info = (HireHero)de.Value;
				if(hero_info.nHealthState21>=0 && hero_info.nHealthState21<100)
				{
					total_caoyao += TreatManager.CalNeedCaoYao(hero_info.nHealthState21,hero_info.nLevel19);
					hurt_hero_list.Add(hero_info.nHeroID1);
				}
			}
			if(total_caoyao >0)
			{
				need_caoyao.text = total_caoyao.ToString();
			}
			else
				need_caoyao.text = "";
			my_caoyao.text = CommonData.player_online_info.CaoYao+"/"+(int)CommonMB.CaoYaoByHospital[(int)Hospital.NowLevel];
			production_label.text = (int)CommonMB.CaoYaoByHospital[(int)Hospital.NowLevel]/24+"/hr";
			treat_panel.gameObject.SetActiveRecursively(true);
			int count = hurt_hero_list.Count;
			int i = 0;
			
      		hurt_hero_list.Sort();
			for(int n = 0;n<hurt_hero_list.Count;n++)
			{
				ulong  hero_id= (ulong)hurt_hero_list[n];
				string obj_name = string.Format("item{0:D2}",i);
				GameObject obj = U3dCmn.GetChildObjByName(treat_grid.gameObject,obj_name);
				if(obj == null)
				{
					obj = NGUITools.AddChild(treat_grid.gameObject,hurt_hero_item);
					obj.name = obj_name;
				}
				obj.SetActiveRecursively(true);
				obj.SendMessage("InitialData",hero_id);
				i++;
			}
			for(;i<4;i++)
			{
				string obj_name = string.Format("item{0:D2}",i);
				GameObject obj = U3dCmn.GetChildObjByName(treat_grid.gameObject,obj_name);
				if(obj == null)
				{
					obj = NGUITools.AddChild(treat_grid.gameObject,hurt_hero_item);
					obj.name = obj_name;
				}
				obj.SetActiveRecursively(true);
				ulong hero_id = 0;
				obj.SendMessage("InitialData",hero_id);
				
			}
			int item_count = treat_grid.gameObject.transform.childCount;
			for(;i<item_count;i++)
			{
				GameObject obj = U3dCmn.GetChildObjByName(treat_grid.gameObject,string.Format("item{0:D2}",i));
				obj.SetActiveRecursively(false);
			}
			treat_grid.Reposition();
			treat_panel.ResetPosition();
		}
	}
	
	//关闭窗口   
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//升级医馆   
	void ReqUpgradeHospital()
	{
		if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.YIGUAN))
		{
			CommonMB.InitialBuilding();
		}
		else
		{
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.YIGUAN,1);
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
	//加速医馆升级    
	void ReqAccelerateHospital()
	{
		BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.YIGUAN,1);
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
	//治疗全部武将 
	void TreatAllHero()
	{
		if(total_caoyao>CommonData.player_online_info.CaoYao)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CAOYAO_NOT_ENOUGH)); 
		}
		else
		{
			ulong[] hero_id_array = new ulong[hurt_hero_list.Count];
			for(int i=0;i<hurt_hero_list.Count;i++)
			{
				hero_id_array[i] = (ulong)hurt_hero_list[i];			
			}
			TreatManager.ReqTreatHero(hero_id_array,total_caoyao);
		}
	}
	//补给药水包 
	void OpenSupplyDrugWin()
	{
		GameObject win = U3dCmn.GetObjFromPrefab("SupplyDrugWin"); 
		if(win != null)
		{
			win.SendMessage("RevealPanel",null);
		}
	}
	
	// 新手引导 <切换到医疗面板> ...
	void TabYiLiao()
	{
		treat_tab.isChecked = true;
		InitialData();
	}
	
	void NewbieTreatOneHero()
	{
		if (hurt_hero_list.Count == 0) 
		{
			if (NewbieYiGuan.processTreatOneHeroRst != null)
			{
				NewbieYiGuan.processTreatOneHeroRst();
				NewbieYiGuan.processTreatOneHeroRst = null;
			}
			return;
		}
		
		Hashtable jlMap = JiangLingManager.MyHeroMap;
		ulong nHeroID = (ulong) hurt_hero_list[0];
		if (true == jlMap.ContainsKey(nHeroID))
		{
			HireHero h1Hero = (HireHero) jlMap[nHeroID];
			int num = TreatManager.CalNeedCaoYao(h1Hero.nHealthState21,h1Hero.nLevel19);
			
			ulong[] hero_array = { nHeroID };
			TreatManager.ReqTreatHero(hero_array,total_caoyao);
		}
	}
}
