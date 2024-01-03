using UnityEngine;
using System;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class YaoQianShu : MonoBehaviour {
	const int SHUIKENGNUM = 8;
	public GameObject jiaoshui;
	public UILabel	produce_time;
	public GameObject produce_obj;
	public GameObject	get_result_icon;
	public UISprite	yaoqianshu_sprite;
	UISprite[] water_array;
	public static byte now_state;
	//新手引导回调 
	public  delegate void WaterSuccess(); 
	public static WaterSuccess	WaterSuccessCallBack; 
	public static bool can_water = false;
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
	
	// Update is called once per frame
	void Update () {
	}
	public void InitialData(STC_GAMECMD_GET_CHRISTMAS_TREE_INFO_T info)
	{
		now_state = info.nState2;
		if(info.nState2 == (byte)STC_GAMECMD_GET_CHRISTMAS_TREE_INFO_T.enum_state.state_can_watering)//可以浇水 
		{
			yaoqianshu_sprite.spriteName = "yaoqianshu";
			get_result_icon.SetActiveRecursively(false);
			//produce_time.gameObject.SetActiveRecursively(false);
			watered_num = (int)info.nWateredNum5;
			RefreshWaterUI(watered_num);
			if(info.bICanWater6 == 1) //我是否现在可以浇水 
			{
				jiaoshui.SetActiveRecursively(true);
				can_water =  true;
			}
			else
			{
				can_water =  false;
				jiaoshui.SetActiveRecursively(false);
				int CanWaterTime =DataConvert.DateTimeToInt(DateTime.Now)+(int)info.nCanWaterCountDownTime4;
				StopCoroutine("Countdown");
				StartCoroutine("Countdown",CanWaterTime);
			}
			produce_obj.SetActiveRecursively(false);
		}
		else if(info.nState2 == (byte)STC_GAMECMD_GET_CHRISTMAS_TREE_INFO_T.enum_state.state_ready_ripe)//成熟倒计时 
		{
			yaoqianshu_sprite.spriteName = "yaoqianshu";
			can_water =  false;
			jiaoshui.SetActiveRecursively(false);
			get_result_icon.SetActiveRecursively(false);
			produce_obj.SetActiveRecursively(true);
			//produce_time.gameObject.SetActiveRecursively(true);
			StopCoroutine("ProduceCountdown");
			//print ("cccccccccccc"+info.nCountDownTime3);
			StartCoroutine("ProduceCountdown",(int)(DataConvert.DateTimeToInt(DateTime.Now)+info.nCountDownTime3+10));
			RefreshWaterUI(-1);
			//PlayerPrefs.SetInt("XuYuanShu"+CommonData.player_online_info.CharName,1);
		}
		else if(info.nState2 == (byte)STC_GAMECMD_GET_CHRISTMAS_TREE_INFO_T.enum_state.state_ripe) //成熟 
		{
			yaoqianshu_sprite.spriteName = "yaoqianshu_active";
			can_water =  false;
			jiaoshui.SetActiveRecursively(false);
			get_result_icon.SetActiveRecursively(true);
			
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
		req.nPeerAccountID3 = CommonData.player_online_info.AccountID;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE_T >(req);
		CommonData.water_yaoqianshu = (int)YAOQIANSHU_CATEGORY.mine;
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE);
		
	}
	//新手引导使用 
	public static void NewCharWaterTree()
	{
		if(now_state == (byte)STC_GAMECMD_GET_CHRISTMAS_TREE_INFO_T.enum_state.state_can_watering)
		{
			CTS_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE;
			req.nPeerAccountID3 = CommonData.player_online_info.AccountID;
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE_T >(req);
			CommonData.water_yaoqianshu = (int)YAOQIANSHU_CATEGORY.mine;
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE);
		}
	}
	//浇水返回 
	void WaterTreeRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE_T>(buff);
		//print ("sssssssssssss"+sub_msg.nRst1);
		if(sub_msg.nRst1 == (byte)STC_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE_T.enum_rst.RST_OK)
		{
			
			watered_num++;
			RefreshWaterUI(watered_num);
			can_water =  false;
			jiaoshui.SetActiveRecursively(false);
			//新手引导回调 
			if(WaterSuccessCallBack!=null)
			{
				WaterSuccessCallBack();
				WaterSuccessCallBack = null; 
			}	
			
		}
		else 
		{
			U3dCmn.SendMessage("BuildingManager","GetYaoQianShuInfo",null);
		}
		CommonData.water_yaoqianshu = 0;
		if(watered_num==SHUIKENGNUM)
			U3dCmn.SendMessage("BuildingManager","GetYaoQianShuInfo",null);
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
	//个人浇水倒计时  
	IEnumerator Countdown(int EndTimeSec)
	{
		int sec = (int)(EndTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
		if(sec <=0)
			sec = 0;
		if( sec>0 )
		{
			// refresh alert info
			GameObject API91Man = GameObject.Find("91Manager");
			if( API91Man!=null )
			{
				API91Man.SendMessage("receiveYaoQianShuWaterTime", EndTimeSec);
			}
		}
		while(sec!=0)
		{
			sec =(int)(EndTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
			
			yield return new WaitForSeconds(1);
		}
		can_water =  true;
		jiaoshui.SetActiveRecursively(true);
	}
	//成熟倒计时  
	IEnumerator ProduceCountdown(int EndTimeSec)
	{
		int sec = (int)(EndTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
		if(sec <=0)
			sec = 0;
		if( sec>0 )
		{
			// refresh alert info
			GameObject API91Man = GameObject.Find("91Manager");
			if( API91Man!=null )
			{
				API91Man.SendMessage("receiveYaoQianShuOverTime", EndTimeSec);
			}
		}
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
		U3dCmn.SendMessage("BuildingManager","GetYaoQianShuInfo",null);
	}
	//收获摇钱树 
	void GetYaoQianShuResult()
	{
		CTS_GAMECMD_OPERATE_FETCH_CHRISTMAS_TREE_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_FETCH_CHRISTMAS_TREE;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_FETCH_CHRISTMAS_TREE_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_FETCH_CHRISTMAS_TREE);
	}
	//收获摇钱树返回  
	void GetYaoQianResultRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_FETCH_CHRISTMAS_TREE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_FETCH_CHRISTMAS_TREE_T>(buff);
		if(sub_msg.nRst1 == (byte)STC_GAMECMD_OPERATE_FETCH_CHRISTMAS_TREE_T.enum_rst.RST_OK)
		{
			if(sub_msg.nType2 == (uint)STC_GAMECMD_OPERATE_FETCH_CHRISTMAS_TREE_T.enum_award_type.award_type_crystal)
			{
				CommonData.player_online_info.Crystal += sub_msg.nCrystal3;
				PlayerInfoManager.RefreshPlayerDataUI();
				string str = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.RESULT_CRYSTAL),sub_msg.nCrystal3);
				U3dCmn.ShowWarnWindow(str);
			}
			else if(sub_msg.nType2 == (uint)STC_GAMECMD_OPERATE_FETCH_CHRISTMAS_TREE_T.enum_award_type.award_type_item)
			{
				CangKuManager.AddItem(sub_msg.nItemID5,(int)sub_msg.nExcelID4,1);	
				string str = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.RESULT_ITEM),U3dCmn.GetItemIconByExcelID(sub_msg.nExcelID4).Name);
				U3dCmn.ShowWarnWindow(str);
			}
			RefreshWaterUI(0);
			yaoqianshu_sprite.spriteName = "yaoqianshu";
			can_water =  true;
			jiaoshui.SetActiveRecursively(true);
			get_result_icon.SetActiveRecursively(false);
			//produce_time.gameObject.SetActiveRecursively(false);
			produce_obj.SetActiveRecursively(false);
			watered_num = 0;
		}
	}
}
