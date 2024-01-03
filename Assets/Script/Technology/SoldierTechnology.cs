using UnityEngine;
using System;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class SoldierTechnology : MonoBehaviour {
	public UISprite soldier_kuang;
	public UISprite soldier_icon;
	public UILabel  soldier_name;
	public UILabel  time_label;
	public UIImageButton research_btn;
	public UIImageButton accelerate_btn;
	string name_str;
	//研究结束时间 
	int BuildEndTimeSec;
	SoldierUnit now_soldier_unit;
	void Awake()
	{
		name_str = soldier_name.text;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	void InitialData(SoldierUnit soldier_unit)
	{
		if(TechnologyManager.ResearchingTechMap.Contains(soldier_unit.nExcelID1))
		{
			ResearchTimeEvent unit = (ResearchTimeEvent)TechnologyManager.ResearchingTechMap[soldier_unit.nExcelID1];
			StopCoroutine("ResearchCountdown");
			BuildEndTimeSec = (int)unit.nEndTime3;
			StartCoroutine("ResearchCountdown",BuildEndTimeSec);
			research_btn.gameObject.SetActiveRecursively(false);
			accelerate_btn.gameObject.SetActiveRecursively(true);
		}
		else 
		{
			research_btn.gameObject.SetActiveRecursively(true);
			accelerate_btn.gameObject.SetActiveRecursively(false);
		}
		now_soldier_unit = soldier_unit;
		soldier_name.text = string.Format(name_str,soldier_unit.nLevel2);
	}
	void OpenResearchWin()
	{
		GameObject win = U3dCmn.GetObjFromPrefab("ResearchSoldierWin"); 
		if(win != null)
		{
			win.SendMessage("RevealPanel",now_soldier_unit);
		}	
		//soldier_kuang.spriteName = "kuang2";
	}
	//倒计时  
	IEnumerator ResearchCountdown(int EndTimeSec)
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
				time_label.text =string.Format("{0}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			else
				time_label.text =string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			yield return new WaitForSeconds(1);
		}
		time_label.text = "";
	}
	//加速科技研究 
	void OpenAccelerateWin()
	{
		AccelerateWin.AccelerateUnit unit;
		unit.Type = (int)enum_accelerate_type.research;
		unit.BuildingType = 0; 
		unit.autoid = now_soldier_unit.nExcelID1;
		unit.EndTimeSec = BuildEndTimeSec;
		GameObject win = U3dCmn.GetObjFromPrefab("AccelerateWin"); 
		if(win != null)
		{
			win.SendMessage("RevealPanel",unit);
		}	
	}
	
}
