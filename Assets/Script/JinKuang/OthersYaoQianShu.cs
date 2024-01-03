using UnityEngine;
using System;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class OthersYaoQianShu : MonoBehaviour {
	const int SHUIKENGNUM = 8;
	public GameObject jiaoshui;
	public UILabel	produce_time;
	public GameObject produce_obj;
	public GameObject	get_result_icon;
	public UISprite	yaoqianshu_sprite;
	UISprite[] water_array;
	ulong  now_account_id;
	//当前摇钱树已经浇水次数 
	int watered_num =0;
	// Use this for initialization
	void Start () {
		water_array = new UISprite[SHUIKENGNUM];
		for(int i=0;i<SHUIKENGNUM;i++)
		{
			water_array[i] = U3dCmn.GetChildObjByName(gameObject,"ShuiKeng"+(i+1)).GetComponent<UISprite>();
		}
	}
	

	public void InitialData(STC_GAMECMD_GET_OTHER_GOLDORE_INFO_T info)
	{
		now_account_id = info.nAccountID2;
		if(info.nState5 == (byte)STC_GAMECMD_GET_OTHER_GOLDORE_INFO_T.enum_tree_rst.state_can_watering)//可以浇水 
		{
			yaoqianshu_sprite.spriteName = "yaoqianshu";
			get_result_icon.SetActiveRecursively(false);
			produce_obj.SetActiveRecursively(false);
			watered_num = (int)info.nWateredNum7;
			RefreshWaterUI(watered_num);
			if(info.bICanWater8 == 1) //我是否现在可以浇水 
			{
				jiaoshui.SetActiveRecursively(true);
			}
			else
			{
				jiaoshui.SetActiveRecursively(false);
			}
		}
		else if(info.nState5 == (byte)STC_GAMECMD_GET_OTHER_GOLDORE_INFO_T.enum_tree_rst.state_ready_ripe)//成熟倒计时 
		{
			yaoqianshu_sprite.spriteName = "yaoqianshu";
			jiaoshui.SetActiveRecursively(false);
			get_result_icon.SetActiveRecursively(false);
			//produce_time.gameObject.SetActiveRecursively(true);
			produce_obj.SetActiveRecursively(true);
			StopCoroutine("ProduceCountdown");
			StartCoroutine("ProduceCountdown",(int)(DataConvert.DateTimeToInt(DateTime.Now)+info.nCountDownTime6));
			RefreshWaterUI(-1);
		}
		else if(info.nState5 == (byte)STC_GAMECMD_GET_OTHER_GOLDORE_INFO_T.enum_tree_rst.state_ripe) //成熟 
		{
			yaoqianshu_sprite.spriteName = "yaoqianshu_active";
			jiaoshui.SetActiveRecursively(false);
			get_result_icon.SetActiveRecursively(true);
			
			//produce_time.gameObject.SetActiveRecursively(false);
			produce_obj.SetActiveRecursively(false);
			RefreshWaterUI(-1);
		}
	}
	//浇水 
	void WaterTree()
	{
		CTS_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE;
		req.nPeerAccountID3 = now_account_id;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE_T >(req);
		CommonData.water_yaoqianshu = (int)YAOQIANSHU_CATEGORY.others;
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE);
	}
	//浇水返回 
	public void WaterTreeRst(STC_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE_T sub_msg)
	{
		watered_num++;
		RefreshWaterUI(watered_num);
		jiaoshui.SetActiveRecursively(false);
		CommonData.water_yaoqianshu = (int)YAOQIANSHU_CATEGORY.others;
	}
	//刷新浇水界面 
	void RefreshWaterUI(int watered_num)
	{
		if(watered_num == -1)
		{
			for(int n=0;n<SHUIKENGNUM;n++)
			{
				water_array[n].spriteName = "transparent";
			}
			return ;
		}
		int i=0;
		for(;i<watered_num;i++)
		{
			water_array[i].spriteName = "shuikengfull";
		}
		for(;i<SHUIKENGNUM;i++)
		{
			water_array[i].spriteName = "shuikeng";
		}
	}
	//成熟倒计时  
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
				produce_time.text =string.Format("{0}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			else
				produce_time.text =string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			yield return new WaitForSeconds(1);
		}
		produce_obj.SetActiveRecursively(false);
	
	}
}
