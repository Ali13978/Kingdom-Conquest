using UnityEngine;
using System;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class JinKuangInfoWin : MonoBehaviour {
	JinKuangUnit 			select_jinkuang_unit;
	public TweenPosition 	tween_position;

	//建造PANEL  
	public UIPanel			build_panel;
	public UIPanel			build_accelerate_panel;
	public UILabel 			build_accelerate_time;
	public UILabel			need_building;
	public UILabel 			need_gold;
	public UILabel 			need_people;
	public UILabel			need_time;
	public GameObject		upgrade_btn;
	//生产黄金  
	public static int 		select_index;	
	public UILabel 			describe_str;
	public UILabel			production_label;
	public UIPanel			produce_panel;
	public UIPanel			produce_accelerate_panel;
	public UILabel			produce_accelerate_time;
	public GameObject ProduceSelectRoot;
	ArrayList ProduceItems = new ArrayList(); 
	string	production_str;
	string[] describe_array;
	//加速panel 
	
	
	public struct JinKuangUnit
	{
		public bool openwin;
		public uint autoid;
		public uint level;
		public uint build_state;
		public int 	build_end_time;
		public uint produce_state;
		public int 	produce_end_time;
		public uint production_num;
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
		
		production_str = production_label.text;
		production_label.text = "";
		describe_array = describe_str.text.Split(':');
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	//弹出窗口  
	void RevealPanel(JinKuangUnit unit)
	{
		
		select_jinkuang_unit = unit;
		if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.JINKUANG))
		{
			CommonMB.InitialBuilding();
		}
		else
		{
			if(ProduceItems.Count != 4)
			{
				ProduceItems.Clear();
				for(int i=1;i<5;i++)
				{
					ProduceItems.Add(U3dCmn.GetChildObjByName(ProduceSelectRoot,"Select"+i));
				}
			}
			
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.JINKUANG,(int)(select_jinkuang_unit.level+1));
		//	AutoID = info.BeginID+(uint)unit.autoid;
			
			//处理建筑信息 
			if(unit.build_state == (uint)BuildingState.NORMAL)
			{
				build_panel.gameObject.SetActiveRecursively(true);
				build_accelerate_panel.gameObject.SetActiveRecursively(false);
				
				
				int sec = (int)info.BuildTime;
				int hour = sec/3600;
				int minute = sec/60%60;
				int second = sec%60;
				need_time.text= string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
				
				
				bool btn_enable = true;
				uint need = select_jinkuang_unit.level+1;
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
				
			}
			else if(unit.build_state == (uint)BuildingState.BUILDING)
			{
				build_panel.gameObject.SetActiveRecursively(false);
				build_accelerate_panel.gameObject.SetActiveRecursively(true);
				StopCoroutine("BuildingCountdown");
				StartCoroutine("BuildingCountdown",unit.build_end_time);
			}
			else if(unit.build_state == (uint)BuildingState.UPGRADING)
			{
				build_panel.gameObject.SetActiveRecursively(false);
				build_accelerate_panel.gameObject.SetActiveRecursively(true);
				StopCoroutine("BuildingCountdown");
				StartCoroutine("BuildingCountdown",unit.build_end_time);
			}
			//处理生产信息 
			if(unit.produce_state == (uint)ProduceState.NORMAL)
			{
				produce_panel.gameObject.SetActiveRecursively(true);
				produce_accelerate_panel.gameObject.SetActiveRecursively(false);
				int gold_level = (int)select_jinkuang_unit.level;
				if(!CommonMB.GoldProduction_Map.Contains(gold_level))
				{
					gold_level = 1;
				}
				GoldProduction production_info = (GoldProduction)CommonMB.GoldProduction_Map[gold_level];	
				//else
				//	GoldProduction production_info = (GoldProduction)CommonMB.GoldProduction_Map[1];
				for(int i =0;i<ProduceItems.Count;i++)
				{
					int product_time = 0;
					int product_num = 0;
					if(i==0)
					{
						product_time= production_info.ProduceTime1;
						product_num = production_info.Production1;
					}
					else if(i==1)
					{
						product_time= production_info.ProduceTime2;
						product_num = production_info.Production2;
					}
					else if(i==2)
					{
						product_time= production_info.ProduceTime3;
						product_num = production_info.Production3;
					}
					else if(i==3)
					{
						product_time= production_info.ProduceTime4;
						product_num = production_info.Production4;
					}
					int minite = product_time / 60;
					int hour = product_time/3600;
					string str = "";
					if(minite<60 && minite>0)
						str = minite+describe_array[0];
					else
						str = hour+ describe_array[1];
					GameObject obj = (GameObject)ProduceItems[i];
					UILabel time_label = U3dCmn.GetChildObjByName(obj,"Time").GetComponentInChildren<UILabel>();
					UILabel product_label = U3dCmn.GetChildObjByName(obj,"Product").GetComponentInChildren<UILabel>();
					product_label.text = production_str+product_num.ToString();
					time_label.text = str;
					UICheckbox check = obj.GetComponent<UICheckbox>();
					if(i==0)
						check.isChecked = true;
					else
						check.isChecked = false;
				}
				select_index = 0;
				ShowGoldTimeAndProduct();
		
			}
			else if(unit.produce_state == (uint)ProduceState.PRODUCE)
			{
				
				produce_panel.gameObject.SetActiveRecursively(false);
				produce_accelerate_panel.gameObject.SetActiveRecursively(true);
				production_label.text = production_str+": "+unit.production_num;
				StopCoroutine("ProduceCountdown");
				StartCoroutine("ProduceCountdown",unit.produce_end_time);
			}
			else if(unit.produce_state == (uint)ProduceState.RESUIT)
			{
				
			}
			if(unit.openwin)
			{
				tween_position.Play(true);
			}
		}
	
	}
	//升级金矿     
	void ReqUpgradeJinKuang()
	{
		
		if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.JINKUANG))
		{
			CommonMB.InitialBuilding();
		}
		else
		{
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.JINKUANG,(int)(select_jinkuang_unit.level));
			uint AutoID = info.BeginID+(uint)select_jinkuang_unit.autoid;
			CTS_GAMECMD_OPERATE_UPGRADE_BUILDING_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_UPGRADE_BUILDING;
			req.nAutoID3 = AutoID;
			//print ("upgrade bingying"+req.nAutoID3 ); 
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_UPGRADE_BUILDING_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_UPGRADE_BUILDING);
		}
		//DismissPanel();
	}
	//倒计时 
	IEnumerator BuildingCountdown(int EndTimeSec)
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
				build_accelerate_time.text =string.Format("{0}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			else
				build_accelerate_time.text =string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			yield return new WaitForSeconds(1);
		}
		build_accelerate_time.text = "";
	}
	//加速金矿建设或升级  
	void AccelerateBuild()
	{
		BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.JINKUANG,(int)(select_jinkuang_unit.level));
		uint AutoID = info.BeginID+(uint)select_jinkuang_unit.autoid;
		AccelerateWin.AccelerateUnit unit;
		unit.Type = (int)enum_accelerate_type.building;
		unit.BuildingType = (uint)te_type_building.te_subtype_building_upgrade; 
		unit.autoid = AutoID;
		unit.EndTimeSec = select_jinkuang_unit.build_end_time;
		GameObject win = U3dCmn.GetObjFromPrefab("AccelerateWin"); 
		if(win != null)
		{
			win.SendMessage("RevealPanel",unit);
			AccelerateWin.MotherWin = gameObject;
			AccelerateWin.CloseCallBack = "DismissPanel";
		}	
	}
	//显示金矿生产时间和产量  
	void ShowGoldTimeAndProduct()
	{
		for(int i =0;i<ProduceItems.Count;i++)
		{
			GameObject obj = (GameObject)ProduceItems[i];
			UICheckbox check = obj.GetComponent<UICheckbox>();
			if(check.isChecked)
			{
				select_index = i;	
			}
		}
		//int product_time =0;
		int gold_production = 0;
		if(CommonMB.GoldProduction_Map.Contains((int)select_jinkuang_unit.level))
		{
			GoldProduction info = (GoldProduction)CommonMB.GoldProduction_Map[(int)select_jinkuang_unit.level];
			if(JinKuangInfoWin.select_index == 0)
			{
				//product_time = info.ProduceTime1;
				gold_production = info.Production1;
			}
			else if(JinKuangInfoWin.select_index == 1)
			{
				//product_time = info.ProduceTime2;
				gold_production = info.Production2;
			}
			else if(JinKuangInfoWin.select_index == 2)
			{
				//product_time = info.ProduceTime3;
				gold_production = info.Production3;
			}
			else if(JinKuangInfoWin.select_index == 3)
			{
				//product_time = info.ProduceTime4;
				gold_production = info.Production4;
			}
		}
		
	
		//production.text = gold_production.ToString();
		
		
	}
	//请求生产黄金 
	void ReqProduce()
	{
		if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.BINGYING))
		{
			CommonMB.InitialBuilding();
		}
		else
		{
			//uint select_index= 0;
			if(select_index>=0)
			{
				BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.JINKUANG,(int)(select_jinkuang_unit.level));
				uint AutoID = info.BeginID+(uint)select_jinkuang_unit.autoid;
				CTS_GAMECMD_OPERATE_PRODUCE_GOLD_T req;
				req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
				req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_PRODUCE_GOLD;
				req.nAutoID3 = AutoID;
				req.nProductionChoice4 = (uint)select_index;
				//print ("upgrade bingying"+req.nAutoID3 ); 
				TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_PRODUCE_GOLD_T >(req);
				LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_PRODUCE_GOLD);
			}
			
		}
		//DismissPanel();
	}
	//生产黄金倒计时   
	IEnumerator ProduceCountdown(int EndTimeSec)
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
				produce_accelerate_time.text =string.Format("{0}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			else
				produce_accelerate_time.text =string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			yield return new WaitForSeconds(1);
		}
		produce_accelerate_time.text = "";
	}
	//加速黄金生产 
	void AccelerateProduce()
	{
		BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.JINKUANG,(int)(select_jinkuang_unit.level));
		uint AutoID = info.BeginID+(uint)select_jinkuang_unit.autoid;
		AccelerateWin.AccelerateUnit unit;
		unit.Type = (int)enum_accelerate_type.gold;
		unit.BuildingType = 0; //这个值在这里不起作用 
		unit.autoid = AutoID;
		unit.EndTimeSec = select_jinkuang_unit.produce_end_time;
		GameObject win = U3dCmn.GetObjFromPrefab("AccelerateWin"); 
		if(win != null)
		{
			win.SendMessage("RevealPanel",unit);
			AccelerateWin.MotherWin = gameObject;
			AccelerateWin.CloseCallBack = "DismissPanel";
		}
	}
	//隐藏窗口  
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
}
