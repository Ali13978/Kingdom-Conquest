using UnityEngine;
using System;
using System.Collections;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class JinKuang : MonoBehaviour {
	//建筑状态 正常、建造、升级 
	public uint 		BuildState;
	//金矿生产状态 正常、生产、可收获  
	public uint 		GoldState;
	public int		AutoID;
	public GameObject build_time_obj;
	public UILabel  build_time_label;
	public GameObject produce_time_obj;
	public UILabel  produce_time_label;
	public UILabel  level_label;
	public UILabel  addnum_label;
	public UISprite GatherIcon;
	public UISprite ProduceIcon;
	public uint 	NowLevel;
	public uint 	production_num;
	
	//建造或者升级完成时间  
	int 	BuildEndTimeSec;
	//生产完成时间  
	int   ProduceEndTimeSec;
	public int getProduceEndTimeSec()
	{
		return	ProduceEndTimeSec;
	}
	//新手引导回调 
	public  delegate void BuildSuccess(); 
	public static BuildSuccess	BuildSuccessCallBack; 
	// Use this for initialization

	void Start () {
		
	}
	//根据服务器发来的信息初始化金矿     
	void InitialJinKuang(uint level)
	{
		//BuildState = (uint)BuildingState.NORMAL;
		//GoldState = (uint)ProduceState.NORMAL;
		
		addnum_label.gameObject.SetActiveRecursively(false);
		NowLevel = level;
		level_label.text = level.ToString();
		if(BuildState == (uint)BuildingState.NORMAL)
		{
			build_time_obj.SetActiveRecursively(false);
		}
		if(GoldState == (uint)ProduceState.NORMAL)
		{
			GatherIcon.gameObject.SetActiveRecursively(false);
			ProduceIcon.gameObject.SetActiveRecursively(true);
			produce_time_obj.SetActiveRecursively(false);
		}
	} 
	//打开建造金矿窗口  
	void OpenJinKuangBuildWin()
	{
		if(BuildState != (uint)BuildingState.BUILDING)
		{
			BuildWin.BuildUnit unit;
			unit.sort = (byte)BuildingSort.JINKUANG;
			unit.autoid = (uint)AutoID;
	
			GameObject buildwin = U3dCmn.GetObjFromPrefab("BuildWin"); 
			if(buildwin != null)
			{
				buildwin.SendMessage("RevealPanel",unit);

			}	
			
			// 如果有好友列表显示则隐藏 ....
			U3dCmn.SendMessage("JinKuangFriendListWin", "DisFriendList", null);
		}
	}
	//打开金矿窗口（生产 或者加速） 
	void OpenJinKuangInfoWin()
	{
		if(BuildState != (uint)BuildingState.BUILDING)
		{
			if(GoldState == (uint)ProduceState.RESUIT)
			{
				ReqGatherGold();
			}
			else // 如果是金子可以收获了 则调用收获函数 
			{
				JinKuangInfoWin.JinKuangUnit unit = new JinKuangInfoWin.JinKuangUnit();
				unit.openwin =true;
				unit.autoid = (uint)AutoID;
				unit.level = NowLevel;
				unit.build_state = BuildState;
				unit.build_end_time = BuildEndTimeSec;
				unit.produce_state = GoldState;
				unit.produce_end_time = ProduceEndTimeSec;
				unit.production_num = production_num;
				GameObject win = U3dCmn.GetObjFromPrefab("JinKuangInfoWin"); 
				if(win != null)
				{
					win.SendMessage("RevealPanel",unit);
				}	
			}
		}
		else 
		{
			//直接打开加速界面 加速建造 
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.JINKUANG,1);
			AccelerateWin.AccelerateUnit unit;
			unit.Type = (int)enum_accelerate_type.building;
			unit.BuildingType = (uint)te_type_building.te_subtype_building_build; 
			unit.autoid = info.BeginID + (uint)AutoID;
			unit.EndTimeSec = BuildEndTimeSec;
			GameObject win = U3dCmn.GetObjFromPrefab("AccelerateWin"); 
			if(win != null)
			{
				win.SendMessage("RevealPanel",unit);
			}	
		}
		
		// 好友列表不须要显示 ....
		// 如果有好友列表显示则隐藏 ....
		U3dCmn.SendMessage("JinKuangFriendListWin", "DisFriendList", null);
	}
	//刷新金矿窗口  
	void RefreshJinKuangInfoWin()
	{
		JinKuangInfoWin.JinKuangUnit unit = new JinKuangInfoWin.JinKuangUnit();
		unit.openwin =false;
		unit.autoid = (uint)AutoID;
		unit.level = NowLevel;
		unit.build_state = BuildState;
		unit.build_end_time = BuildEndTimeSec;
		unit.produce_state = GoldState;
		unit.produce_end_time = ProduceEndTimeSec;
		unit.production_num = production_num;
		GameObject win = GameObject.Find("JinKuangInfoWin"); 
		if(win != null)
		{
			win.SendMessage("RevealPanel",unit);
		}	
	}
	//建造请求返回开始倒计时 
	void BuildBeginCountdown()
	{
		BuildState = (uint)BuildingState.BUILDING;
		SetTopIcon();
		if(CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.JINKUANG))
		{
			
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.JINKUANG,(int)(NowLevel+1));
			int sec = info.BuildTime;
			BuildEndTimeSec = DataConvert.DateTimeToInt(DateTime.Now)+sec;
			
			uint auto_id = info.BeginID+(uint)AutoID;
			//写入建造升级表 
			if(!BuildingManager.BuildingTeMap.Contains(auto_id))
			{
				BuildingTEUnit teunit =  new BuildingTEUnit();
				teunit.nAutoID3 = auto_id;
				teunit.nEndTime2 = (uint)BuildEndTimeSec;
				teunit.nExcelID4 = (uint)info.ID;
				teunit.nType5 = (uint)BuildingState.BUILDING;
				BuildingManager.BuildingTeMap.Add(auto_id,teunit);
			}
			
		}
		RefreshJinKuangInfoWin();
		StopCoroutine("BuildingCountdown");
		StartCoroutine("BuildingCountdown",BuildEndTimeSec);
	}
	
	//初始化建造时间队列 一般是APP打开时获取 
	void InitialTimeTe(BuildingManager.TimeTeUnit timeunit)
	{
		
		BuildState = timeunit.BuildState;
		SetTopIcon();
		BuildEndTimeSec = (int)(DataConvert.DateTimeToInt(DateTime.Now)+timeunit.time);
		StopCoroutine("BuildingCountdown");
		StartCoroutine("BuildingCountdown",BuildEndTimeSec);
	}
	//建造成功 
	void ProcessBuildRst()
	{
		StopCoroutine("BuildingCountdown");
		BuildState = (uint)BuildingState.NORMAL;
		SetTopIcon();
		level_label.text = (++NowLevel).ToString();
		build_time_label.text = "";
		if(BuildSuccessCallBack!=null)
		{
			BuildSuccessCallBack();
			BuildSuccessCallBack = null; 
		}
		//刷新基本信息 
		U3dCmn.SendMessage("BuildingManager","RefreshJinKuangInfo",null);	
	}
	//升级请求返回开始倒计时 
	void UpgradeBeginCountdown()
	{
		BuildState = (uint)BuildingState.UPGRADING;
		SetTopIcon();
		if(CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.JINKUANG))
		{
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.JINKUANG,(int)(NowLevel+1));
			int sec = (int)info.BuildTime;
			BuildEndTimeSec = DataConvert.DateTimeToInt(DateTime.Now)+sec;
			RefreshJinKuangInfoWin();
			StopCoroutine("BuildingCountdown");
			StartCoroutine("BuildingCountdown",BuildEndTimeSec);
			
			uint auto_id = info.BeginID+(uint)AutoID;
			//写入建造升级表 
			if(!BuildingManager.BuildingTeMap.Contains(auto_id))
			{
				BuildingTEUnit teunit =  new BuildingTEUnit();
				teunit.nAutoID3 = auto_id;
				teunit.nEndTime2 = (uint)BuildEndTimeSec;
				teunit.nExcelID4 = (uint)info.ID;
				teunit.nType5 = (uint)BuildingState.UPGRADING;
				BuildingManager.BuildingTeMap.Add(auto_id,teunit);
			}
		}
		
		
	}
	//升级成功  
	void ProcessUpgradeRst()
	{
		StopCoroutine("BuildingCountdown");
		BuildState = (uint)BuildingState.NORMAL;
		SetTopIcon();
		
		level_label.text = (++NowLevel).ToString();
		build_time_label.text = "";
		RefreshJinKuangInfoWin();
		//刷新基本信息 
		U3dCmn.SendMessage("BuildingManager","RefreshJinKuangInfo",null);	
	}
	
	
	//显示生产金子浮动按钮 
	void ShowProduceIcon()
	{
		ProduceIcon.gameObject.SetActiveRecursively(true);
	}
	//建造或者升级倒计时  
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
				build_time_label.text =string.Format("{0}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			else
				build_time_label.text =string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			yield return new WaitForSeconds(1);
		}
	}
	//生产黄金返回  
	void ProcessProduceRst()
	{
		GoldState = (uint)ProduceState.PRODUCE;
		SetTopIcon();
		int product_time = 0;
		if(CommonMB.GoldProduction_Map.Contains((int)NowLevel))
		{
			GoldProduction info = (GoldProduction)CommonMB.GoldProduction_Map[(int)NowLevel];
			if(JinKuangInfoWin.select_index == 0)
			{
				production_num = (uint)info.Production1;
				product_time = info.ProduceTime1;
			}
			else if(JinKuangInfoWin.select_index == 1)
			{
				production_num = (uint)info.Production2;
				product_time = info.ProduceTime2;
			}
			else if(JinKuangInfoWin.select_index == 2)
			{
				production_num = (uint)info.Production3;
				product_time = info.ProduceTime3;
			}
			else if(JinKuangInfoWin.select_index == 3)
			{
				production_num = (uint)info.Production4;
				product_time = info.ProduceTime4;
			}
		}
		ProduceEndTimeSec = DataConvert.DateTimeToInt(DateTime.Now)+ product_time;
		StopCoroutine("ProduceCountdown");
		StartCoroutine("ProduceCountdown",ProduceEndTimeSec);
		RefreshJinKuangInfoWin();
		
		
		// < 新手引导 > 生产金矿 ....
		if (NewbieJinKuang.processJinKuangProduceRst != null)
		{
			NewbieJinKuang.processJinKuangProduceRst();
			NewbieJinKuang.processJinKuangProduceRst = null;
		}
	}
	//生产黄金时间事件 
	void ProcessProduceTERst(ProduceManager.ProduceTimeTeUnit timeunit)
	{
		production_num = timeunit.Production;
		GoldState = timeunit.ProduceState;
		SetTopIcon();
		ProduceEndTimeSec = (int)(DataConvert.DateTimeToInt(DateTime.Now)+timeunit.time);
		StopCoroutine("ProduceCountdown");
		StartCoroutine("ProduceCountdown",ProduceEndTimeSec);
	}
	//生产黄金倒计时   
	IEnumerator ProduceCountdown(int EndTimeSec)
	{
		int sec = (int)(EndTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
		if(sec <=0 )
			sec = 0;
		/*
		else
		{
			GameObject API91Man = GameObject.Find("91Manager");
			if( API91Man!=null )
			{
				print("**** try call receiveGoldmineTime:"+EndTimeSec);
				API91Man.SendMessage("receiveGoldmineTime", EndTimeSec);
			}
		}
		*/
		while(sec!=0)
		{
			sec =(int)(EndTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
			if(sec <0)
				sec = 0;
			int hour = sec/3600;
			int minute = sec/60%60;
			int second = sec%60;
			if(hour>=100)
				produce_time_label.text =string.Format("{0}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			else
				produce_time_label.text =string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			yield return new WaitForSeconds(1);
		}
	}
	//金子可以收获  
	void ProcessGoldResult(uint production)
	{
		StopCoroutine("ProduceCountdown");
		produce_time_label.text = "";
		GoldState = (uint)ProduceState.RESUIT;
		SetTopIcon();
		
	}
	//请求收获金子 
	void ReqGatherGold()
	{
		if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.JINKUANG))
		{
			CommonMB.InitialBuilding();
		}
		else
		{
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.JINKUANG,(int)(NowLevel));
			CTS_GAMECMD_OPERATE_FETCH_GOLD_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_FETCH_GOLD;
			req.nAutoID3 = info.BeginID+(uint)AutoID; 
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_FETCH_GOLD_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_FETCH_GOLD);
		}
	}
	//请求收获金子返回成功 
	void  GatherGoldRst(uint production)
	{
		GoldState = (uint)ProduceState.NORMAL;
		SetTopIcon();
		PlayGatherGold(production);
		CommonData.player_online_info.Gold += production;		
		PlayerInfoManager.RefreshPlayerDataUI();
		
	}
	
	//收获金子动画   
	void PlayGatherGold(uint production)
	{
		GatherIcon.gameObject.SetActiveRecursively(false);
		addnum_label.gameObject.SetActiveRecursively(true);
		addnum_label.text = "+"+production.ToString();
		TweenColor tween_color = addnum_label.GetComponent<TweenColor>();
		TweenPosition tween_position = addnum_label.GetComponent<TweenPosition>();
		tween_color.Reset(); 
		tween_position.Reset();
		tween_color.Play(true);
		tween_position.Play(true);
	}
	//根据当前状态显示可生产或者收获图标 
	void SetTopIcon()
	{
		if(GoldState == (uint)ProduceState.NORMAL)
		{
			GatherIcon.gameObject.SetActiveRecursively(false);
			ProduceIcon.gameObject.SetActiveRecursively(true);
			addnum_label.gameObject.SetActiveRecursively(false);
			produce_time_obj.SetActiveRecursively(false);
		}
		else if(GoldState == (uint)ProduceState.PRODUCE)
		{
			GatherIcon.gameObject.SetActiveRecursively(false);
			ProduceIcon.gameObject.SetActiveRecursively(false);
			addnum_label.gameObject.SetActiveRecursively(false);
			produce_time_obj.SetActiveRecursively(true);
		}
		else if(GoldState == (uint)ProduceState.RESUIT)
		{
			GatherIcon.gameObject.SetActiveRecursively(true);
			ProduceIcon.gameObject.SetActiveRecursively(false);
			addnum_label.gameObject.SetActiveRecursively(false);
			produce_time_obj.SetActiveRecursively(false);
		}
		if(BuildState == (uint)BuildingState.BUILDING)
		{
			GatherIcon.gameObject.SetActiveRecursively(false);
			ProduceIcon.gameObject.SetActiveRecursively(false);
			addnum_label.gameObject.SetActiveRecursively(false);
			build_time_obj.SetActiveRecursively(true);
		}
		else if(BuildState == (uint)BuildingState.UPGRADING)
		{
			build_time_obj.SetActiveRecursively(true);
		}
		else if(BuildState == (uint)BuildingState.NORMAL)
		{
			build_time_obj.SetActiveRecursively(false);
		}
	}
}
