using UnityEngine;
using System;
using System.Collections;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class MinJu : MonoBehaviour {
	//建筑状态 正常、建造、升级 
	public uint 	BuildState;
	public int		AutoID;
	public GameObject Time_obj;
	public UILabel  Time_Label;
	public UILabel  Level_Label;

	public uint 	NowLevel;

	//建造或者升级结束的时间 
	int 			BuildEndTimeSec;
	//新手引导回调 
	public  delegate void BuildSuccess(); 
	public static BuildSuccess	BuildSuccessCallBack; 
	// Use this for initialization
	void Start () {
	
	}
	
	
	//根据服务器发来的信息初始化民居   
	void InitialMinJu(uint level)
	{
		//BuildState = (uint)BuildingState.NORMAL;
		NowLevel = level;
		Level_Label.text = level.ToString();
		CalCapacityProduction();
		if(BuildState != (uint)BuildingState.UPGRADING && BuildState != (uint)BuildingState.BUILDING)
		{
			Time_obj.SetActiveRecursively(false);
		}
	}
	//打开建造民居控制面板    
	void OpenMinJuBuildWin()
	{
		
		BuildWin.BuildUnit unit;
		unit.sort = (byte)BuildingSort.MINJU;
		unit.autoid = (uint)AutoID;
		GameObject buildwin = U3dCmn.GetObjFromPrefab("BuildWin"); 
		if(buildwin != null)
		{
			buildwin.SendMessage("RevealPanel",unit);
		}	
		
	}
	//打开升级民居控制面板   
	void OpenMinJuInfoWin()
	{
		if(BuildState != (uint)BuildingState.BUILDING)
		{
			MinJuInfoWin.MinJuUnit unit;
			unit.level = NowLevel;
			unit.autoid = (uint)AutoID;
			unit.build_state = BuildState;
			unit.build_end_time = BuildEndTimeSec;
			GameObject infowin = U3dCmn.GetObjFromPrefab("MinJuInfoWin");
			if(infowin != null)
			{
				infowin.SendMessage("RevealPanel",unit);
			}
		}
		else 
		{
			//直接打开加速界面 加速建造 
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.MINJU,1);
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
		
	}
	//建造请求返回开始倒计时 
	void BuildBeginCountdown()
	{
		BuildState = (uint)BuildingState.BUILDING;
		if(CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.MINJU))
		{
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.MINJU,(int)(NowLevel+1));
			int sec = info.BuildTime;
			BuildEndTimeSec = DataConvert.DateTimeToInt(DateTime.Now)+sec;
			Time_obj.SetActiveRecursively(true);
			StartCoroutine("Countdown",BuildEndTimeSec);
			
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
		
	}
	//初始化时间队列 一般是APP打开时获取 
	void InitialTimeTe(BuildingManager.TimeTeUnit timeunit)
	{
		BuildState = timeunit.BuildState;
		StopCoroutine("Countdown");
		BuildEndTimeSec = (int)(DataConvert.DateTimeToInt(DateTime.Now)+timeunit.time);
		Time_obj.SetActiveRecursively(true);
		StartCoroutine("Countdown",BuildEndTimeSec);
	}
	//建造成功 
	void ProcessBuildRst()
	{
		Time_obj.SetActiveRecursively(false);
		BuildState = (uint)BuildingState.NORMAL;
		StopCoroutine("Countdown");
		Level_Label.text = (++NowLevel).ToString();
		Time_Label.text = "";
		CalCapacityProduction();
		//刷新基本信息 
		U3dCmn.SendMessage("BuildingManager","RefreshMinJuInfo",null);
		if(BuildSuccessCallBack!=null)
		{
			BuildSuccessCallBack();
			BuildSuccessCallBack = null; 
		}	
	}
	//升级请求返回开始倒计时 
	void UpgradeBeginCountdown()
	{
		BuildState = (uint)BuildingState.UPGRADING;
		if(CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.MINJU))
		{
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.MINJU,(int)(NowLevel+1));
			int sec = (int)info.BuildTime;
			BuildEndTimeSec = DataConvert.DateTimeToInt(DateTime.Now)+sec;
			Time_obj.SetActiveRecursively(true);
			StartCoroutine("Countdown",BuildEndTimeSec);
			
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
		
		// <新手引导> 民居升级 ...
		if (NewbieHouse.processUpgradeMinJuRst != null)
		{
			NewbieHouse.processUpgradeMinJuRst();
			NewbieHouse.processUpgradeMinJuRst = null;
		}
	}
	//升级成功  
	void ProcessUpgradeRst()
	{
		Time_obj.SetActiveRecursively(false);
		BuildState = (uint)BuildingState.NORMAL;
		StopCoroutine("Countdown");
		Level_Label.text = (++NowLevel).ToString();
		Time_Label.text = "";
		CalCapacityProduction();
		//刷新基本信息 
		U3dCmn.SendMessage("BuildingManager","RefreshMinJuInfo",null);
	}
	//倒计时 
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
				Time_Label.text =string.Format("{0}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			else
				Time_Label.text =string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			yield return new WaitForSeconds(1);
		}
	}
	//计算此民居的容量和生产率 存入静态哈希表  
	void  CalCapacityProduction()
	{
		
		int capacity =0;
		int production=0;
		if(CommonMB.HouseProduction_Map.Contains((int)NowLevel))
		{
			HouseProduction unit = (HouseProduction)CommonMB.HouseProduction_Map[(int)NowLevel];
			capacity = unit.Capacity;
			production = (int)((float)unit.Production*((float)3600/(float)unit.ProductTime));
		}
		
		BuildingManager.MinJu_Capacity_Map.Remove(AutoID);
		BuildingManager.MinJu_Capacity_Map.Add(AutoID,capacity);
	//	int production = (int)(CommonMB.House_Production.BaseProduct*(1+NowLevel*CommonMB.House_Production.ProductScale)*1.0f/CommonMB.House_Production.ProductTime);
		BuildingManager.MinJu_Product_Map.Remove(AutoID);
		BuildingManager.MinJu_Product_Map.Add(AutoID,production);
		
		
	}
}
