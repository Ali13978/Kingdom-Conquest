using UnityEngine;
using System;
using System.Collections;
using STCCMD;
using CMNCMD;
public class EventItem : MonoBehaviour {
	public UILabel event_name;
	public UILabel time_label;
	public string  color_str;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//建造事件初始化  
	void InitialBuildEventData(BuildingTEUnit event_data)
	{
		string describe  = U3dCmn.GetBuildingInfoFromMb((int)event_data.nExcelID4,1).Name;
		if(event_data.nType5 == (uint)BuildingState.BUILDING)
		{
			//describe += EventWin.describe_array[0];
		}
		else if(event_data.nType5 == (uint)BuildingState.UPGRADING)
		{
			//describe += EventWin.describe_array[1];
		}
		color_str = "[00FFFF]";
		event_name.text = color_str+describe;
		StopCoroutine("Countdown");
		StartCoroutine("Countdown",(int)event_data.nEndTime2);
	}
	//科技研究事件初始化  
	void InitialResearchEventData(ResearchTimeEvent event_data)
	{
		string describe  = U3dCmn.GetSoldierInfoFromMb(event_data.nExcelID1,1).Name;
		//describe += EventWin.describe_array[2];
		color_str = "[00FF00]";
		event_name.text = color_str+describe;
		StopCoroutine("Countdown");
		StartCoroutine("Countdown",(int)event_data.nEndTime3);
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
				time_label.text =color_str+string.Format("{0}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			else
				time_label.text =color_str+string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			yield return new WaitForSeconds(1);
		}
		event_name.text = "";
		time_label.text = "";
	}
}
