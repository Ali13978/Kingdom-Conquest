using UnityEngine;
using System.Collections;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class BingYingInfoWin : MonoBehaviour {
	BingYingUnit now_unit;
	public TweenPosition 	tween_position;
	//建筑信息面板  
	public UILabel			soldier_num;
	public UILabel			need_building;
	public UILabel 			need_gold;
	public UILabel 			need_people;
	public UILabel			need_time;
	public UILabel			no_soldier_tech;
	public GameObject       upgrade_btn;
	public GameObject       accelerate_btn;
	string need_time_str;
	//训练士兵面板  
	//全部grid管理器   
	public PagingStorage soldier_storage;
	public static Hashtable soldier_map = new Hashtable();
	public static ArrayList soldier_key_array; 
	//Hashtable soldierpage_map = new Hashtable();
	//切换面板标签 
	public UICheckbox buildpaneltab;
	public UICheckbox soldierpaneltab;
	bool isstart;
	public struct BingYingUnit
	{
		public uint level;
		public uint autoid;
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
		
		// 当没有兵种科技的时候提醒 ....
		if (no_soldier_tech != null) {
			no_soldier_tech.enabled = false;
		}
	}
	
	// Use this for initialization
	void Start () {
		
		//RevealPanel(now_unit);
	}
	// Update is called once per frame
	void Update () {
		//需要OBJ初始化在START函数之后执行的函数 
		if(isstart)
		{
			InitialData();
			isstart = false;
		}
	}
	
	//弹出窗口   
	void RevealPanel(BingYingUnit unit)
	{
		now_unit =unit;
		isstart = true;
		tween_position.Play(true);
	}
	//关闭窗口   
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//点击训练士兵按钮切换面板初始化   
	void InitialData()
	{
		if(buildpaneltab.isChecked)
		{
			if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.BINGYING))
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
				BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.BINGYING,(int)(now_unit.level+1));
				//AutoID = info.BeginID+(uint)unit.autoid;
			
				soldier_num.text = SoldierManager.GetSoldierNum()+"/"+SoldierManager.GetBingYingCapacity();
				
				int sec = (int)info.BuildTime;
				int hour = sec/3600;
				int minute = sec/60%60;
				int second = sec%60;
				need_time.text= string.Format(need_time_str,hour,minute,second);
				
				bool btn_enable = true;
				uint need = now_unit.level+1;
				uint now = ChengBao.NowLevel;
			
				need_building.text = need+"/"+now;
			
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
				need_gold.text = need+"/"+U3dCmn.GetNumStr(now);
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
				need_people.text = need+"/"+U3dCmn.GetNumStr(now);
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
				tween_position.Play(true);
			}
		}
		else if(soldierpaneltab.isChecked)
		{
			//计算目前需要几个兵种的item     
			soldier_map.Clear(); 
			foreach(DictionaryEntry	de	in	SoldierManager.SoldierMap)  
	  		{
				//if()
				SoldierUnit unit = (SoldierUnit)de.Value;
				if(unit.nNum3 >0)
					soldier_map.Add(de.Key,de.Value);
			}
			
			foreach(DictionaryEntry	de	in	SoldierManager.SoldierTopMap)  
	  		{
				SoldierUnit info = (SoldierUnit) de.Value;
				if(!soldier_map.Contains(de.Key) && info.nLevel2>0)
				{
					soldier_map.Add(de.Key,de.Value);
				}
				
			}
			soldier_key_array = new ArrayList(soldier_map.Keys);
			soldier_key_array.Sort();
			
			int capacity = soldier_map.Count;
			soldier_storage.SetCapacity(capacity);
			soldier_storage.ResetAllSurfaces();			
			
			// 现在没有兵种科技 ....
			if (capacity == 0)
			{
				if (no_soldier_tech != null) {
					no_soldier_tech.text = U3dCmn.GetWarnErrTipFromMB(330);
					no_soldier_tech.enabled = true;
				}
			}
		}
		
		//sodier_map.Add 
	
			//InitialSoldierPanel();
		
	}

	//升级兵营    
	void ReqUpgradeBingYing()
	{
		
		if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.BINGYING))
		{
			CommonMB.InitialBuilding();
		}
		else
		{
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.BINGYING,(int)(now_unit.level));
			uint AutoID = info.BeginID+(uint)now_unit.autoid;
			CTS_GAMECMD_OPERATE_UPGRADE_BUILDING_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_UPGRADE_BUILDING;
			req.nAutoID3 = AutoID;
			//print ("upgrade bingying"+req.nAutoID3 ); 
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_UPGRADE_BUILDING_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_UPGRADE_BUILDING);
		}
		DismissPanel();
	}
	//刷新所有界面 
	void RefreshUI()
	{
		InitialData();
	}

	//加速 
	void ReqAccelerateBingYing()
	{
		BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.BINGYING,(int)(now_unit.level));
		uint AutoID = info.BeginID+(uint)now_unit.autoid;
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
	//打开兵种说明界面 
	void OpenSoldierExplain()
	{
		GameObject infowin = U3dCmn.GetObjFromPrefab("BulletinWin");
		if(infowin != null)
		{
			InstanceMBInfo info = (InstanceMBInfo)CommonMB.InstanceMBInfo_Map[(uint)HELP_TYPE.SOLDIER_SORT_HELP];
			infowin.GetComponent<BulletinWin>().title_label.text = info.mode;
			infowin.GetComponent<BulletinWin>().text_label.text = info.Rule1;
			infowin.SendMessage("RevealPanel");
		}
	}
	void TabXunLian()
	{
		soldierpaneltab.isChecked = true;
		InitialData();
	}
	
	// 为新引导特做功能 ....
	void OpenTrainSoldierWin()
	{
		Hashtable jsMap = BingYingInfoWin.soldier_map;
		ArrayList ls = BingYingInfoWin.soldier_key_array;
		
		if (true == jsMap.ContainsKey(ls[0]))
		{
			SoldierUnit card = (SoldierUnit)jsMap[ls[0]];
			
			GameObject obj =  U3dCmn.GetObjFromPrefab("TrainSoldierWin");
			if(obj != null)
			{
				obj.SendMessage("RevealPanel",card);
			}
		}
	}
	
}
