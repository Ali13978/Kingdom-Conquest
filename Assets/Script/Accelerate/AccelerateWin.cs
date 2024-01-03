using UnityEngine;
using System;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class AccelerateWin : MonoBehaviour {
	public static GameObject MotherWin;
	public static string CloseCallBack;
	//加速的几个选项  
	float[] AccelerateSort = {600.0f,3600.0f,21600.0f,43200.0f};
	string cost_str;
	AccelerateUnit 			NowUnit;
	public TweenPosition 	tween_position;
	public GameObject 		AccTimeRoot;
	public GameObject 		CostSortRoot;
	public UILabel		 	TimeLabel;
	public UICheckbox		DiamandCheck;
	public UICheckbox		CrystalCheck;
	public UILabel			DiamondNum;
	public UILabel			CrystalNum;
	ArrayList AccelerateChecks = new ArrayList(); 
	
	public struct AccelerateUnit
	{
		//建筑加速还是黄金生产加速或者研究加速 
		public int 		Type;
		public uint 	BuildingType;
		public uint 	autoid;
		public int  	EndTimeSec;
	}
	
	public void Awake()
	{
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			transform.localScale = CommonData.iphonescale;
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			transform.localScale = CommonData.ipadscale;
		}
		GameObject acconce = U3dCmn.GetChildObjByName(AccTimeRoot,"AccCost");
		cost_str = acconce.GetComponent<UILabel>().text;
	}
	// Use this for initialization 
	void Start () {
		AccelerateChecks.Clear();
		for(int i=1;i<5;i++)
		{
			AccelerateChecks.Add(U3dCmn.GetChildObjByName(AccTimeRoot,"Checkbox"+i).GetComponent<UICheckbox>());
			UILabel cost_label = U3dCmn.GetChildObjByName(AccTimeRoot,"AccCost"+i).GetComponent<UILabel>();
			cost_label.text =cost_str+" "+ U3dCmn.GetAccelerateDiscount((int)((AccelerateSort[i-1]/CommonMB.Accelerate_Info.UnitTime*CommonMB.Accelerate_Info.UnitCost)));
		}
	}
	
	//弹出窗口    
	void RevealPanel(AccelerateUnit unit)
	{
		MotherWin = null;
		CloseCallBack = "";
		NowUnit = unit; 
		StopCoroutine("TimeCountdown");
		TimeLabel.text = "";
		StartCoroutine("TimeCountdown",unit.EndTimeSec);
		UILabel acc_atonce = U3dCmn.GetChildObjByName(AccTimeRoot,"AccCost").GetComponent<UILabel>();
		float sec = (float)(unit.EndTimeSec-DataConvert.DateTimeToInt(DateTime.Now));
		//print ("ttttttttt"+minute+"  "+CommonMB.Accelerate_Info.UnitCost+" "+CommonMB.Accelerate_Info.UnitTime);
		//uint cost = (uint)((sec/(CommonMB.Accelerate_Info.UnitTime))*CommonMB.Accelerate_Info.UnitCost+1);
		
		int  cost = U3dCmn.UpperInt((sec/(CommonMB.Accelerate_Info.UnitTime))*CommonMB.Accelerate_Info.UnitCost);
		int discountcost = U3dCmn.GetAccelerateDiscount(cost);
		
		acc_atonce.text = cost_str + " "+discountcost.ToString();
		DiamondNum.text = CommonData.player_online_info.Diamond.ToString();
		CrystalNum.text = CommonData.player_online_info.Crystal.ToString();
		tween_position.Play(true);
	}
	//关闭窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//倒计时 
	IEnumerator TimeCountdown(int EndTimeSec)
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
				TimeLabel.text =string.Format("{0}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			else
				TimeLabel.text =string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			yield return new WaitForSeconds(1);
		}
		TimeLabel.text = "";
	}
	//请求加速  
	void ReqAccelerate()
	{
		uint acctime = 0;
		//UICheckbox check = U3dCmn.GetChildObjByName(AccTimeRoot,"Checkbox").GetComponent<UICheckbox>();
		if(U3dCmn.GetChildObjByName(AccTimeRoot,"Checkbox").GetComponent<UICheckbox>().isChecked)
		{
			//多加点时间 避免客户端时间不正确 反正是服务器计算正确的扣钱数量 
			acctime = (uint)((NowUnit.EndTimeSec-DataConvert.DateTimeToInt(DateTime.Now))+60);
		}
		for(int i =0;i<AccelerateChecks.Count;i++)
		{
			
			UICheckbox check =(UICheckbox)AccelerateChecks[i];
			if(check.isChecked)
			{
				acctime = (uint)(AccelerateSort[i]);	
			}
		}
		uint moneytype = 0;
		int  cost = U3dCmn.UpperInt((acctime/(CommonMB.Accelerate_Info.UnitTime))*CommonMB.Accelerate_Info.UnitCost);
		int discountcost = U3dCmn.GetAccelerateDiscount(cost);
		if(DiamandCheck.isChecked)
		{
			//检查钻石是否够用  
			if(CommonData.player_online_info.Diamond<discountcost)
			{
				//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.DIAMOND_NOT_ENOUGH));
				U3dCmn.ShowRechargePromptWindow();
				return ;
			}
			moneytype = (uint)money_type.money_type_diamond;
		}
		else if(CrystalCheck.isChecked)
		{
			//检查水晶是否够用  
			if(CommonData.player_online_info.Crystal<discountcost)
			{
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CRYSTAL_NOT_ENOUGH));
				return ;
			}
			moneytype = (uint)money_type.money_type_crystal;
		}
		if(NowUnit.Type == (int)enum_accelerate_type.building)
		{
			CTS_GAMECMD_OPERATE_ACCE_BUILDING_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_ACCE_BUILDING;
			req.nAutoID3 = NowUnit.autoid;
			req.nType4 = NowUnit.BuildingType;
			req.nTime5 = acctime;
			req.nMoneyType6 = moneytype;
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_ACCE_BUILDING_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ACCE_BUILDING);
		}
		else if(NowUnit.Type == (int)enum_accelerate_type.gold)
		{
			CTS_GAMECMD_OPERATE_ACCE_GOLD_PRODUCE_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_ACCE_GOLD_PRODUCE;
			req.nAutoID3 = (uint)NowUnit.autoid;
			req.nTime4 = acctime;
			req.nMoneyType5 = moneytype;
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_ACCE_GOLD_PRODUCE_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ACCE_GOLD_PRODUCE);
		}
		else if(NowUnit.Type == (int)enum_accelerate_type.research)
		{
			CTS_GAMECMD_OPERATE_ACCE_RESEARCH_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_ACCE_RESEARCH;
			req.nExcelID3 = NowUnit.autoid;
			req.nTime4 = acctime;
			req.nMoneyType5 = moneytype;
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_ACCE_RESEARCH_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ACCE_RESEARCH);
		}
		//LoadingManager.instance.ShowLoading();
	}
	//关闭窗口   
	void CompleteAccelerate()
	{
		tween_position.Play(false);
		if(MotherWin != null && CloseCallBack!="")
			MotherWin.SendMessage(CloseCallBack);
		
	}
	
}
