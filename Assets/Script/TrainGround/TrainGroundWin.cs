using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class TrainGroundWin : MonoBehaviour {
	public TweenPosition 	tween_position;
//	public TweenScale	 	tween_scale;
	//建筑信息面板  
	public UILabel			need_building;
	public UILabel 			need_gold;
	public UILabel 			need_people;
	public UILabel			need_time;
	public UILabel			describe_label;
	public GameObject       upgrade_btn;
	public GameObject       accelerate_btn;
	public UICheckbox 		building_tab;
	public UICheckbox		train_tab;
	string need_time_str;
	string describe_str;
	
	//训练武将面板 
	public GameObject 		train_item;
	public UIDraggablePanel train_ground_panel;
	public UIGrid 			train_ground_grid;
	List<GameObject> HeroObjList = new List<GameObject>();
	public static TrainGroundUnit now_unit;
	bool isstart;
	
	public struct TrainGroundUnit
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
		describe_str = describe_label.text;
	}
		
	// Use this for initialization
	void Start () {
		//isstart = true;
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
	void RevealPanel(TrainGroundUnit unit)
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
			if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.XIULIAN))
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
				BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.XIULIAN,(int)(now_unit.level+1));
				int sec = (int)info.BuildTime; 
				int hour = sec/3600;
				int minute = sec/60%60;
				int second = sec%60;
				need_time.text= string.Format(need_time_str,hour,minute,second);
				
				if(CommonMB.TrainGround_Map.Contains(TrainGround.NowLevel))
				{
					TrainingGround train_unit = (TrainingGround)CommonMB.TrainGround_Map[TrainGround.NowLevel];
					describe_label.text = string.Format(describe_str,train_unit.HeroTopLevel);
				}
					
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
					need_building.color = new Color(0,0.38f,0,1);
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
					need_gold.color = new Color(0,0.38f,0,1);
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
					need_people.color = new Color(0,0.38f,0,1);
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
		else if(train_tab.isChecked)
		{
			TrainGroundManager.SelectHeroList.Clear();
			train_ground_panel.gameObject.SetActiveRecursively(true);
			int count = JiangLingManager.MyHeroMap.Count;
			int i = 0;
			ArrayList list = new ArrayList(JiangLingManager.MyHeroMap.Keys);
      		list.Sort();
			
			HeroObjList.Clear();
			for(int n = 0;n<list.Count;n++)
			{
				ulong  hero_id= (ulong)list[n];
				string obj_name = string.Format("item{0:D2}",i);
				GameObject obj = U3dCmn.GetChildObjByName(train_ground_grid.gameObject,obj_name);
				if(obj == null)
				{
					obj = NGUITools.AddChild(train_ground_grid.gameObject,train_item);
					obj.name = obj_name;
				}
				obj.SetActiveRecursively(true);
				obj.SendMessage("InitialData",hero_id);
				HeroObjList.Add(obj);
				i++;
			}
			for(;i<3;i++)
			{
				string obj_name = string.Format("item{0:D2}",i);
				GameObject obj = U3dCmn.GetChildObjByName(train_ground_grid.gameObject,obj_name);
				if(obj == null)
				{
					obj = NGUITools.AddChild(train_ground_grid.gameObject,train_item);
					obj.name = obj_name;
				}
				obj.SetActiveRecursively(true);
				ulong hero_id = 0;
				obj.SendMessage("InitialData",hero_id);
				
			}
			int item_count = train_ground_grid.gameObject.transform.childCount;
			for(;i<item_count;i++)
			{
				GameObject obj = U3dCmn.GetChildObjByName(train_ground_grid.gameObject,string.Format("item{0:D2}",i));
				obj.SetActiveRecursively(false);
			}
			train_ground_grid.Reposition();
			train_ground_panel.ResetPosition();
		}
	}
	//关闭窗口   
	void DismissPanel()
	{
		tween_position.Play(false);
		
		//foreach(Transform obj in train_ground_grid.gameObject.transform)
		//{
		//	if(obj.gameObject.active)
		//		obj.gameObject.SendMessage("QuitWin");
		//}
		Destroy(gameObject);
	}
	//升级训练场  
	void ReqUpgradeTrainGround()
	{
	
		if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.XIULIAN))
		{
			CommonMB.InitialBuilding();
		}
		else
		{
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.XIULIAN,1);
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
	//加速训练场升级    
	void ReqAccelerateTrainGround()
	{
		BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.XIULIAN,1);
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
	//训练选中将领 
	void TrainSelectHero()
	{
		//TrainGroundManager.SelectHeroList.Clear();
		/*for(int i=0;i<HeroObjList.Count;i++)
		{
			TrainGroundItem item = HeroObjList[i].GetComponent<TrainGroundItem>();
			if(item.is_select && item.now_hero_id != 0)
			{
				if(!TrainGroundManager.HeroTrainingMap.Contains(item.now_hero_id) && !TrainGroundManager.SelectHeroList.Contains(item.now_hero_id))
				{
					TrainGroundManager.SelectHeroList.Add(item.now_hero_id);
				}
			}
		}*/
		if(TrainGroundManager.SelectHeroList.Count>0)
		{
			if(!CheckHeroFree())
			{
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_NOT_FREE)); 
				return ;
			}
			GameObject infowin = U3dCmn.GetObjFromPrefab("TrainWin");
			if(infowin != null)
			{
				infowin.SendMessage("RevealPanel");
			}
		}
	
	}
	//全部修炼（所有可以修炼的将领）  
	void TrainAllHero()
	{
		TrainGroundManager.SelectHeroList.Clear();
		if(!CommonMB.TrainGround_Map.Contains(TrainGroundWin.now_unit.level))
			return ;
		TrainingGround train_ground = (TrainingGround)CommonMB.TrainGround_Map[TrainGroundWin.now_unit.level];
		ArrayList list = new ArrayList(JiangLingManager.MyHeroMap.Keys);
  		list.Sort();
		
		for(int i = 0;i<list.Count;i++)
		{
			HireHero hero_unit = (HireHero)JiangLingManager.MyHeroMap[list[i]];
			if(!TrainGroundManager.HeroTrainingMap.Contains(hero_unit.nHeroID1))
			{
				HeroExpGoldInfo exp_gold_info =  (HeroExpGoldInfo)CommonMB.HeroExpGold_Map[hero_unit.nLevel19];
				if(hero_unit.nStatus14== (int)HeroState.NORMAL && hero_unit.nLevel19<CommonData.player_online_info.Level)
				{
					TrainGroundManager.SelectHeroList.Add(hero_unit.nHeroID1);
				}
			}
			
		}
		for(int i=0;i<HeroObjList.Count;i++)
		{
			TrainGroundItem item = HeroObjList[i].GetComponent<TrainGroundItem>();
			if(item.hero_unit.nStatus14 == (int)HeroState.NORMAL && item.hero_unit.nLevel19<CommonData.player_online_info.Level)
				item.SetSelect();
		}
		if(TrainGroundManager.SelectHeroList.Count >0)
		{
			if(!CheckHeroFree())
			{
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_NOT_FREE)); 
				return ;
			}
			GameObject infowin = U3dCmn.GetObjFromPrefab("TrainWin");
			if(infowin != null)
			{
				infowin.SendMessage("RevealPanel");
			}
		}
		
	}
	//全部停止修炼 
	void StopAllTrain()
	{
		foreach(DictionaryEntry de in TrainGroundManager.HeroTrainingMap)
		{
			ulong hero_id =(ulong)de.Key;
			TrainGroundManager.ReqStopTrainHero(hero_id);
		}
	}
	//检查所训练的将领是否为空闲状态 
	bool CheckHeroFree()
	{
		bool isfree = true;
		for(int i= 0;i<TrainGroundManager.SelectHeroList.Count;i++)
		{
			ulong hero_id = TrainGroundManager.SelectHeroList[i];
			if(JiangLingManager.MyHeroMap.Contains(hero_id))
			{
				HireHero info =(HireHero)JiangLingManager.MyHeroMap[hero_id];
				if(info.nStatus14 != (int)HeroState.NORMAL)
				{
					isfree = false;
					break;
				}
			}
			
		}	
		return isfree;
	}	
}
