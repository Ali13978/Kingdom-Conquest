using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using CTSCMD;
using STCCMD;
using CMNCMD;

public class HonorInfoWin : MonoBehaviour {
	//消息窗口positon TWEEN    
	public TweenPosition tween_position; 
	public static Hashtable HonorCityMap =  new Hashtable();
	public GameObject 	city_item;
	public UILabel    	battle_time_label;
	public UILabel		refresh_city_label;
	public UIDraggablePanel city_panel;
	public UIGrid city_grid;
	string battle_time_str;
	string refresh_time_str;
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
		battle_time_str = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HONOR_BATTLE_OVER_TIME);
		refresh_time_str = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HONOR_CITY_REFRESH_TIME);
		
	}
	// Use this for initialization
	void Start () {
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void RevealPanel()
	{	
		ReqWorldCity();
		tween_position.Play(true);
	}
	//请求名城信息 
	void ReqWorldCity()
	{
		TiledStorgeCacheData.RequestHonorCity();
		TiledStorgeCacheData.processHonorCityDelegate -= InitialData;
		TiledStorgeCacheData.processHonorCityDelegate += InitialData;
	}
	//初始化数据 
	void InitialData()
	{
		
		//TiledStorgeCacheData.HonorCityMap
		for(int i=0;i<TiledStorgeCacheData.HonorCityList.Count;i++)
		{
			WorldCity city_data = (WorldCity)TiledStorgeCacheData.HonorCityList[i];
			string obj_name = string.Format("item{0:D2}",i);
			GameObject obj = NGUITools.AddChild(city_grid.gameObject,city_item);
			obj.name = obj_name;
			obj.SendMessage("InitialData",city_data);
			//city_grid.
			
		}
		city_grid.Reposition();
			
		
		//print ("allover,time:" + TiledStorgeCacheData.HonorWarRestTime);
		int EndTimeSec = (int)(DataConvert.DateTimeToInt(DateTime.Now)+ TiledStorgeCacheData.HonorWarRestTime);
		StopCoroutine("BattleCountdown");
		StartCoroutine("BattleCountdown",EndTimeSec);
		//print ("allover,refreshtime:" + TiledStorgeCacheData.HonorCityRefreshTime);
		EndTimeSec = (int)(DataConvert.DateTimeToInt(DateTime.Now)+ TiledStorgeCacheData.HonorCityRefreshTime);	
		StopCoroutine("RefreshCityCountdown");
		StartCoroutine("RefreshCityCountdown",EndTimeSec);
		TiledStorgeCacheData.processHonorCityDelegate -= InitialData;
		//time_label.text =  time_describe+
	}
	//隐藏窗口 
	void DismissPanel()
	{
		
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//打开圣杯排行榜 
	void OpenCupRankWin()
	{
		GameObject win = U3dCmn.GetObjFromPrefab("CupRankWin"); 
		if(win != null)
		{
			win.SendMessage("RevealPanel");
		}	
	}
	//打开历史战绩窗口 
	void OpenHistoryWin()
	{
		GameObject win = U3dCmn.GetObjFromPrefab("HonorHistoryWin"); 
		if(win != null)
		{
			win.SendMessage("RevealPanel");
		}	
	}
	//战争倒计时  
	IEnumerator BattleCountdown(int EndTimeSec)
	{
		int sec = (int)(EndTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
		
		if(sec <0)
			sec = 0;
		while(sec!=0)
		{
			 sec =(int)(EndTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
			if(sec <0)
				sec = 0;
			int day	= sec/86400;
			int hour = sec/3600;
			int minute = sec/60%60;
			int second = sec%60;
			if(hour>=24)
				battle_time_label.text =battle_time_str+day+U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CHINESE_DAY);
			else
				battle_time_label.text =battle_time_str+string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			yield return new WaitForSeconds(1);
			battle_time_label.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HONOR_BATTLE_OVER);
		}
		battle_time_label.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HONOR_BATTLE_OVER);
	}
	//刷新名城倒计时 
	IEnumerator RefreshCityCountdown(int EndTimeSec)
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
				refresh_city_label.text =refresh_time_str+string.Format("{0}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			else
				refresh_city_label.text =refresh_time_str+string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			yield return new WaitForSeconds(1);
			//refresh_city_label.text = refresh_time_str;
		}
		refresh_city_label.text = refresh_time_str+"00:00:00";
	}
	//打开活动说明  
	void OpenRongYaoInfoWin()
	{
		GameObject infowin = U3dCmn.GetObjFromPrefab("BulletinWin");
		if(infowin != null)
		{
			InstanceMBInfo info = (InstanceMBInfo)CommonMB.InstanceMBInfo_Map[(uint)HELP_TYPE.HONOR_BATTLE];
			infowin.GetComponent<BulletinWin>().title_label.text = info.mode;
			infowin.GetComponent<BulletinWin>().text_label.text = info.Rule1;
			infowin.SendMessage("RevealPanel");
		}
	}
}
