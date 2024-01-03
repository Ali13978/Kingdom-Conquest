using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class CityItem : MonoBehaviour {
	public UILabel city_name;
	public UILabel cup_num;
	public UILabel city_status;
	public UILabel city_position;
	public UILabel city_attack;
	
	WorldCity now_city_data;
	string normal_str;
	string recover_str;
	void Awake()
	{
		normal_str = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NORMAL_STATUS);
		recover_str = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.RECOVER_STATUS);
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//初始化数据 
	void InitialData(WorldCity city_data)
	{
		now_city_data = city_data;
		city_name.text = DataConvert.BytesToStr(city_data.szName8);
		cup_num.text = city_data.nCup6.ToString();
		city_attack.text = city_data.nInitProf9.ToString();
		city_position.text = city_data.nPosX3+","+city_data.nPosY4;
		
		
		if(city_data.nRecover5 == 0)
		{
			city_status.text = normal_str;
		}
		else
		{
			StopCoroutine("Countdown");
			int BuildEndTimeSec = (int)(DataConvert.DateTimeToInt(DateTime.Now)+city_data.nRecover5);
			StartCoroutine("Countdown",BuildEndTimeSec);
		}
		
	}
	//定位名城 
	void LocateCity()
	{
		SceneManager.OpenGlobalMapLocation(now_city_data.nPosX3,now_city_data.nPosY4);
		U3dCmn.SendMessage("HonorInfoWin","DismissPanel",null);
	}
	//恢复时间倒计时 
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
				city_status.text =recover_str+": "+string.Format("{0}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			else
				city_status.text =recover_str+": "+string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			yield return new WaitForSeconds(1);
		}
		city_status.text = normal_str;
	}
}
