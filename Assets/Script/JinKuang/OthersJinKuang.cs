using UnityEngine;
using System;
using System.Collections;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class OthersJinKuang : MonoBehaviour {
	
	//金矿生产状态 正常、生产、可收获  
	public uint 		GoldState;
	public int		AutoID;
	public GameObject produce_time_obj;
	public UILabel  produce_time_label;
	public UILabel  level_label;
	public UILabel  addnum_label;
	public UISprite GatherIcon;

	
	//生产完成时间  
	int   ProduceEndTimeSec;
	// Use this for initialization

	void Start () {
		
	}
	//根据服务器发来的信息初始化金矿     
	void InitialJinKuang(GoldoreInfo gold_info)
	{
		level_label.text = gold_info.nLevel2.ToString();
		if(gold_info.nState4 == (uint)GoldoreInfo.enum_rst.state_none) 
		{
			GoldState = (uint)ProduceState.NORMAL;
			SetTopIcon();
		}
		else if(gold_info.nState4 == (uint)GoldoreInfo.enum_rst.state_producing) //生产状态 
		{
			GoldState = (uint)ProduceState.PRODUCE;
			SetTopIcon();
			ProduceEndTimeSec = (int)(DataConvert.DateTimeToInt(DateTime.Now)+gold_info.nCountDownTime3+5);
			StopCoroutine("ProduceCountdown");
			StartCoroutine("ProduceCountdown",ProduceEndTimeSec);
		}
		else if(gold_info.nState4 == (uint)GoldoreInfo.enum_rst.state_can_fetch) //可摘取状态 
		{
			StopCoroutine("ProduceCountdown");
			GoldState = (uint)ProduceState.RESUIT;
			SetTopIcon();
		}
	} 
	//生产黄金倒计时   
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
				produce_time_label.text =string.Format("{0}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			else
				produce_time_label.text =string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			yield return new WaitForSeconds(1);
		}
		U3dCmn.SendMessage("OthersJinKuangWin","RefreshPanel",null);
	}
	//金子可以收获  
	/*void ProcessGoldResult(uint production)
	{
		StopCoroutine("ProduceCountdown");
		produce_time_label.text = "";
		GoldState = (uint)ProduceState.RESUIT;
		SetTopIcon();
		
	}*/

	//请求偷窃金子  
	void ReqStealGold()
	{
		BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.JINKUANG,1);
		if(GoldState == (uint)ProduceState.RESUIT)
		{
			CTS_GAMECMD_OPERATE_STEAL_GOLD_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_STEAL_GOLD;
			req.nPeerAccountID3 = OthersJinKuangWin.now_char_info.AccountId;
			req.nAutoID4 =info.BeginID+(uint)AutoID;
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_STEAL_GOLD_T >(req);
			OthersJinKuangWin.steal_jinkuang_id = req.nAutoID4;
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_STEAL_GOLD);
		}
	}
	//请求偷窃金子返回成功 
	void  ReqStealGoldRst(uint gold_num)
	{
		GoldState = (uint)ProduceState.NORMAL;
		SetTopIcon();
		PlayGatherGold(gold_num);
		CommonData.player_online_info.Gold += gold_num;
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
			addnum_label.gameObject.SetActiveRecursively(false);
			produce_time_obj.SetActiveRecursively(false);
		}
		else if(GoldState == (uint)ProduceState.PRODUCE)
		{
			GatherIcon.gameObject.SetActiveRecursively(false);
			addnum_label.gameObject.SetActiveRecursively(false);
			produce_time_obj.SetActiveRecursively(true);
		}
		else if(GoldState == (uint)ProduceState.RESUIT)
		{
			GatherIcon.gameObject.SetActiveRecursively(true);
			addnum_label.gameObject.SetActiveRecursively(false);
			produce_time_obj.SetActiveRecursively(false);
		}
	}
}
