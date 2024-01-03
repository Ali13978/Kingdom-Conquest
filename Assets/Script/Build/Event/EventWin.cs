using UnityEngine;
using System;
using System.Collections;
using STCCMD;
using CTSCMD;
using CMNCMD;
public class EventWin : MonoBehaviour {
	//好友列表窗口  
	public TweenPosition 	event_list_position;
	bool  event_list_show = false;
	public UIDraggablePanel 	event_panel;
	public UIGrid event_grid;
	public GameObject event_item;
	public UILabel describe_str;
	public UILabel build_num;
	public UILabel build_num_only;
	public UILabel build_add_time;
	public static string[] describe_array = new string[3];
	string build_str;
	string build_add_time_str;
	void Awake()
	{
		build_str = build_num.text;
		build_add_time_str = build_add_time.text;
		describe_array = describe_str.text.Split(':');
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void RevealPanel()
	{
		event_list_show = !event_list_show;
		event_list_position.Play(event_list_show);
		if(event_list_show)
		{
			if(CommonData.player_online_info.AddBuildNumTime-DataConvert.DateTimeToInt(DateTime.Now)>0)
			{
				//build_add_time.text = build_add_time_str;
				build_num.text = build_str+"   "+BuildingManager.BuildingTeMap.Count+"/"+CommonData.player_online_info.TotalBuildNum;
				build_num_only.text = "";
				int EndTimeSec = CommonData.player_online_info.AddBuildNumTime;
				StopCoroutine("AddBuildCountdown");
				StartCoroutine("AddBuildCountdown",EndTimeSec);
			}
			else 
			{
				build_num.text = "";
				build_num_only.text = build_str+"   "+BuildingManager.BuildingTeMap.Count+"/"+CommonData.player_online_info.TotalBuildNum;
				build_add_time.text = "";
			}
			int i =0;
			foreach(DictionaryEntry de in TechnologyManager.ResearchingTechMap)
			{
				ResearchTimeEvent research_event = (ResearchTimeEvent)de.Value;
				GameObject obj = NGUITools.AddChild(event_grid.gameObject,event_item);	
				obj.name = string.Format("EventItem{0:D2}",i);
				obj.SendMessage("InitialResearchEventData",research_event);
				i++;
			}
			foreach(DictionaryEntry de in BuildingManager.BuildingTeMap)
			{
				BuildingTEUnit building_event = (BuildingTEUnit)de.Value;
				GameObject obj = NGUITools.AddChild(event_grid.gameObject,event_item);	
				obj.name = string.Format("EventItem{0:D2}",i);
				obj.SendMessage("InitialBuildEventData",building_event);
				i++;
			}
		
			event_grid.Reposition();	
			event_panel.ResetPosition();
		}
		else
		{
			foreach(Transform obj in event_grid.gameObject.transform)
			{
				Destroy(obj.gameObject);
			}
			StopCoroutine("AddBuildCountdown");
		}
	}
	//工匠之书时间倒计时 
	IEnumerator AddBuildCountdown(int EndTimeSec)
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
				build_add_time.text =build_add_time_str+"   "+string.Format("{0}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			else
				build_add_time.text =build_add_time_str+"   "+string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			yield return new WaitForSeconds(1);
		}
		build_num.text = "";
		build_num_only.text = build_str+BuildingManager.BuildingTeMap.Count+"/"+build_str+CommonData.player_online_info.TotalBuildNum;
		build_add_time.text = "";
	}
}
