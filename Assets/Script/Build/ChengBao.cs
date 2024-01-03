using UnityEngine;
using System;
using System.Collections;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class ChengBao : MonoBehaviour {
	//建筑状态 正常、建造、升级  
	public static uint 	BuildState;
	public UILabel  	Time_Label;
	public GameObject   Time_obj;
	public UILabel  	Name_Label;
	public static uint 	NowLevel;
	public static uint 	TopLevel;
	public bool 		IsBuilding;
	//建造或者升级结束的时间 
	int 			BuildEndTimeSec;
	
	// Use this for initialization
	void Start () {
		IsBuilding = false;
		Time_obj.SetActiveRecursively(false);
	}
	
	
	//根据服务器发来的信息初始化城堡  
	void InitialChengBao(uint level)
	{
		//BuildState = (uint)BuildingState.NORMAL;
		NowLevel = level;
		Name_Label.text = level.ToString();
		if(BuildState == (uint)BuildingState.NORMAL)
		{
			Time_obj.SetActiveRecursively(false);
		}
		
	}
	//打开升级城堡窗口 
	void OpenChengBaoInfoWin()
	{
		ChengBaoInfoWin.ChengBaoUnit unit;
		unit.level = NowLevel;
		unit.build_state = BuildState;
		unit.build_end_time = BuildEndTimeSec;
		GameObject obj = U3dCmn.GetObjFromPrefab("ChengBaoInfoWin");
		if(obj != null)
		{
			obj.SendMessage("RevealPanel",unit);
		}

		
	}
	//升级请求返回开始倒计时  
	void UpgradeBeginCountdown()
	{
		BuildState = (uint)BuildingState.UPGRADING;
		if(CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.CHENGBAO))
		{
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.CHENGBAO,(int)(NowLevel+1));
			int sec = (int)info.BuildTime;
			BuildEndTimeSec = DataConvert.DateTimeToInt(DateTime.Now)+sec;
			Time_obj.SetActiveRecursively(true);
			StartCoroutine("Countdown",BuildEndTimeSec);
			//写入建造升级表 
			if(!BuildingManager.BuildingTeMap.Contains(info.BeginID))
			{
				BuildingTEUnit teunit =  new BuildingTEUnit();
				teunit.nAutoID3 = info.BeginID;
				teunit.nEndTime2 = (uint)BuildEndTimeSec;
				teunit.nExcelID4 = (uint)info.ID;
				teunit.nType5 = (uint)BuildingState.UPGRADING;
				BuildingManager.BuildingTeMap.Add(teunit.nAutoID3,teunit);
			}
		}
		
		// <新手引导> 请求城堡升级成功 ....
		if (NewbieChengBao.processChengBaoReqUpgradeRst != null)
		{
			NewbieChengBao.processChengBaoReqUpgradeRst();
			NewbieChengBao.processChengBaoReqUpgradeRst = null;
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
	//升级成功  
	void ProcessUpgradeRst()
	{
		BuildState = (uint)BuildingState.NORMAL;
		StopCoroutine("Countdown");
		Name_Label.text =(++NowLevel).ToString();
		Time_Label.text = "";
		Time_obj.SetActiveRecursively(false);
				
		// 升级2级城堡时, 可以提醒引导任务 ...
		if (NowLevel == 2) 
		{
			if (NewbieChengBao.processChengBaoUpgradeRst != null) {
				NewbieChengBao.processChengBaoUpgradeRst();
			}
			
			NewbieChengBao.processChengBaoUpgradeRst = null;
		}
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
}
