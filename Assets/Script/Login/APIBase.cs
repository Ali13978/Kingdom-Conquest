using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using CMNCMD;
using STCCMD;
using CTSCMD;
public class APIBase : MonoBehaviour {
	private int m_nLatestGoldMine;
	private int m_nYaoQianShuWaterTime;
	private int m_nYaoQianShuOverTime;
	private bool m_bAlertOn;

	bool m_bLogin = false;
	int  m_alertCount = 0;
	System.Random m_rd = new System.Random();
	private int recharge_diamond_num;

	protected virtual int do_init(string obj, string method)
	{
		print("null func: do_init");
		return 0;
	}
	protected virtual int do_setcallback(string obj, string method)
	{
		print("null func: do_setcallback");
		return 0;
	}
	protected virtual int do_cmd(string cmd, string param)
	{
		print("null func: do_cmd");
		return 0;
	}
	
	// notifications
	void OnAlertSoundSwitcherValueChanged(bool val)
	{
		// print("**** OnAlertSoundSwitcherValueChanged:"+val);
		string cmd = "0";
		if( val ) {
			cmd		= "1";
		}
		do_cmd("alertsound", cmd);
	}
	void OnAlertSwitcherValueChanged(bool val)
	{
		print("**** alert is :"+val);
		m_bAlertOn	= val;
	}
	void OnApplicationPause(bool pause)
	{
		print("**** OnApplicationPause:"+pause+" alert:"+m_bAlertOn);
		if( pause ) {
			if( m_bAlertOn )
			{
				string	str;
				// check if building manager exists
				GameObject BldMan = GameObject.Find("BuildingManager");
				if( BldMan!=null )
				{
					int	nNow	= DataConvert.DateTimeToInt(System.DateTime.Now);
					// check current unfinished work lists
					// tst for direct check
					m_nLatestGoldMine	= -1;
					foreach(DictionaryEntry de in BuildingManager.JinKuang_Map)
					{
						GameObject obj = (GameObject)de.Value;
						JinKuang jinkuang = obj.GetComponent<JinKuang>();
						int	ProduceEndTimeSec	= jinkuang.getProduceEndTimeSec();
						if( ProduceEndTimeSec>0 && (m_nLatestGoldMine<0 || ProduceEndTimeSec<m_nLatestGoldMine ) )
						{
							m_nLatestGoldMine	= ProduceEndTimeSec;
						}
					}
					int	nGoldTime = m_nLatestGoldMine - nNow;
					if( nGoldTime>0 ) {
						str	= U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALERT_MSG_GOLD_IS_READY);
						AlertMsg(nGoldTime, str);
						print("**** alerting GoldMine:"+nGoldTime);
					}
					// check building list
					int	nBuildTime	= -1;
					foreach(DictionaryEntry de in BuildingManager.BuildingTeMap)
					{
						BuildingTEUnit event_data = (BuildingTEUnit)de.Value;
						if(event_data.nType5 == (uint)BuildingState.BUILDING
						|| event_data.nType5 == (uint)BuildingState.UPGRADING
						)
						{
							if( nBuildTime<=0 || nBuildTime>(int)event_data.nEndTime2 )
							{
								nBuildTime	= (int)event_data.nEndTime2;
							}
						}
					}
					if( nBuildTime>0 )
					{
						str	= U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALERT_MSG_BUILDING_IS_READY);
						nBuildTime	= nBuildTime - nNow;
						AlertMsg(nBuildTime, str);
						print("**** alerting Building:"+nBuildTime);
					}
					// check tech
					int	nTechTime	= -1;
					foreach(DictionaryEntry de in TechnologyManager.ResearchingTechMap)
					{
						ResearchTimeEvent event_data = (ResearchTimeEvent)de.Value;
						if( nTechTime<=0 || nBuildTime>(int)event_data.nEndTime3 )
						{
							nTechTime	= (int)event_data.nEndTime3;
						}
					}
					if( nTechTime>0 )
					{
						str	= U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALERT_MSG_TECH_IS_READY);
						nTechTime	= nTechTime - nNow;
						AlertMsg(nTechTime, str);
						print("**** alerting Tech:"+nTechTime);
					}
					// yaoqianshu water
					int nYaoQianShuWater	= m_nYaoQianShuWaterTime - nNow;
					if( nYaoQianShuWater>0 )
					{
						str	= U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALERT_MSG_TREE_WAIT_WATER);
						AlertMsg(nYaoQianShuWater+10, str);
						print("**** alerting tree water:"+nYaoQianShuWater);
					}
					else
					{
						int nYaoQianShuOver	= m_nYaoQianShuOverTime - nNow;
						if( nYaoQianShuOver>0 )
						{
							str	= U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALERT_MSG_TREE_IS_READY);
							AlertMsg(nYaoQianShuOver+10, str);
							print("**** alerting tree OVER:"+nYaoQianShuOver);
						}
					}
				}
	
				// setup long time no see alerts
				str		= U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALERT_MSG_1DAY_NO_SEE);
				AlertMsg(12*3600, str);
				str		= U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALERT_MSG_2DAY_NO_SEE);
				AlertMsg(24*3600, str);
				str		= U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALERT_MSG_7DAY_NO_SEE);
				AlertMsg(3*24*3600, str);
			}
		}
		else {
			//m_nLatestGoldMine	= 0;
			// clear all notifications
			clearAllNotification();
		}
	}
	/*
	// receive the latest goldmine time event
	void receiveGoldmineTime(int sec)
	{
		if( m_nLatestGoldMine<=0 || sec<=0 || m_nLatestGoldMine>sec )
		{
			m_nLatestGoldMine	= sec;
		}
		print("**** m_nLatestGoldMine:"+m_nLatestGoldMine+" "+sec);
	}
	*/
	void receiveYaoQianShuWaterTime(int sec)
	{
		if( m_nYaoQianShuWaterTime<=0 || sec<=0 || m_nYaoQianShuWaterTime>sec )
		{
			m_nYaoQianShuWaterTime	= sec;
		}
		print("**** m_nYaoQianShuWaterTime:"+m_nYaoQianShuWaterTime);
	}
	void receiveYaoQianShuOverTime(int sec)
	{
		if( m_nYaoQianShuOverTime<=0 || sec<=0 || m_nYaoQianShuOverTime>sec )
		{
			m_nYaoQianShuOverTime	= sec;
		}
		print("**** m_nYaoQianShuOverTime:"+m_nYaoQianShuOverTime);
	}

	// Use this for initialization
	void Start () {
		if( Application.isEditor ) {
			//print("**** "+gameObject.name);
		}
		else {
			do_init(gameObject.name, "CallbackFromPlugin");
			//InitAdmob();
			print("**** do_init in device ...");
			// 设置解锁条上的文字（如果有定义的话）    
			string act = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALERT_MSG_ON_LOCK_BAR);
			if( act!="unknown err" )
			{
				do_cmd("alertaction", act);
			}
		}
		DontDestroyOnLoad(gameObject);
		// if not set, default to true (need alert)
		m_bAlertOn	= true;
	}
	// Update is called once per frame
	void Update () {
	
	}

	public string getGUID()
    {
		return	System.Guid.NewGuid().ToString();
    }

	// callback from plugin
	public void CallbackFromPlugin(string str) {
		// split cmd and param
		char [] delimiter = " ".ToCharArray();
		string [] aa = str.Split(delimiter, 2);
		if( aa[0]=="prompt" ) {
			//m_prompt.text	= aa[1];
		}
		else if( aa[0]=="Login_OK" ) {
			// Login_OK loginUin nickName sessionId
			string []bb = aa[1].Split(delimiter, 3);
			m_bLogin	= true;
			//U3dCmn.ShowWarnWindow("LoginOK acc:"+bb[0]+" "+bb[1]);
			// try login
			//print("Login params:"+bb[0]+":"+bb[1]+":"+bb[2]);
			CommonData.player_online_info.AccountName	= bb[0];
			CommonData.player_online_info.AccountPass	= bb[2];
			U3dCmn.SendMessage("LoginManager","StartLogin",null);
		}
		else if( aa[0]=="Login_Canceled" ) {
			m_bLogin	= false;
			//U3dCmn.ShowWarnWindow("Login canceled");
		}
		else if( aa[0]=="Logout_OK" ) {
			m_bLogin	= false;
			if(GameObject.Find("SceneManager") !=null) 
			{
				StartLogin();
			}
			//U3dCmn.ShowWarnWindow("Logout rst:"+aa[1]);
		}
		else if( aa[0]=="buy_err" ) {
			// 购买过程出错 
			print("IAP err:"+aa[1]);
			//U3dCmn.ShowWarnWindow(aa[1]);
			LoadingManager.instance.HideLoading();
		}
		else if( aa[0]=="buy_rst" ) {
			// 购买结果返回（主要应该是appstore返回的receipt）  
			//print("IAP rst:"+aa[1]);
			SendIAPReceipt(aa[1]);
		}
		else if( aa[0]=="uuid" ) {
			if( aa[1]=="null" ) {
				aa[1]	= "";
			}
			print("**** apibase->UUID:"+"["+aa[1]+"]");
			CommonData.NowDeviceMac = aa[1];
			
			GameObject login_obj = GameObject.Find("LoginManager");
			if( login_obj!=null )
			{
				login_obj.SendMessage("LoginWithMac");
			}
			else
			{
				GameObject win = U3dCmn.GetObjFromPrefab("AccountManageWin"); 
				if(win != null)
				{
					win.SendMessage("RevealPanel",null);
				}
			}
		}
		else {
			print("unknown rst:"+str);
		}
	}

	public void StartLogin() {
		do_cmd("login", "");
	}
	public void clearAllNotification() {
		do_cmd("cancelalert", "");
		}
	public void AlertMsg(int sec, string msg) {
		if( sec>0 && msg !="unknown err")
		{
			do_cmd("alert", sec+" "+msg); 
		}
	}
	public void SwitchAccount() {
		do_cmd("switch", "");
	}
	public void BuyDiamond(int num) {
		do_cmd("buy", num+" "+0+" "+0);
	}
	public void ShowShop() {
		do_cmd("shop", getGUID());
	}
	public void InitAdmob()
	{	
		do_cmd("InitAdmob", "");
	}
	public void ShowAdmob()
	{	
		do_cmd("ShowAdmob", "");
	}
	public void OpenChargeMoneyWin(int DiamondNum) {
		recharge_diamond_num	= DiamondNum;
		CTSCMD.CTS_GAMECMD_GET_PAY_SERIAL_T	req;
		req.nCmd1		= (byte)CTSCMD.CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2	= (uint)CTSCMD.SUB_CTS_MSG.CTS_GAMECMD_GET_PAY_SERIAL;
		TcpMsger.SendLogicData<CTSCMD.CTS_GAMECMD_GET_PAY_SERIAL_T>(req);
	}
	
	// callback from network 
	public void cbGetPaySerial(byte[] buff) {
		STCCMD.STC_GAMECMD_GET_PAY_SERIAL_T	sub_msg	= DataConvert.ByteToStruct<STCCMD.STC_GAMECMD_GET_PAY_SERIAL_T>(buff);
		if (sub_msg.nRst1 == (byte)STCCMD.STC_GAMECMD_GET_PAY_SERIAL_T.enum_rst.RST_OK)
		{
			string serial = Encoding.ASCII.GetString(sub_msg.szPaySerial3);
			//print("**** get pay serial:"+serial);
			do_cmd("buy", recharge_diamond_num +" "+CommonData.player_online_info.AccountID+" "+serial);
		}
	}
	public void cbRechargeDiamond(byte[] buff) {
		STCCMD.STC_GAMECMD_RECHARGE_DIAMOND_T	sub_msg	= DataConvert.ByteToStruct<STCCMD.STC_GAMECMD_RECHARGE_DIAMOND_T>(buff);
		CommonData.player_online_info.Diamond	+= (sub_msg.nAddedDiamond1 + sub_msg.nGiftDiamond2);
		CommonData.player_online_info.TotalDiamond += sub_msg.nAddedDiamond1;
		CommonData.player_online_info.Vip		= sub_msg.nVip3;
		// make ui reflect the diamond change
		PlayerInfoManager.RefreshPlayerDataUI();
		U3dCmn.SendMessage("RechargeWin91","CalculateVIPDiamond",null);
		U3dCmn.SendMessage("RechargeWinApple","CalculateVIPDiamond",null);
		LoadingManager.instance.HideLoading();
		// show alert
		U3dCmn.ShowWarnWindow(string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.RECHARGE_SUCCESS),sub_msg.nAddedDiamond1,sub_msg.nGiftDiamond2));
	}
	void Open91Platform()
	{
		do_cmd("enterplatform","");
	}
	//朝服务器发送苹果充值的校验串 
	void SendIAPReceipt(string receipt)
	{
		CTS_GAMECMD_OPERATE_UPLOAD_BILL_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_UPLOAD_BILL;
		req.nClientTime3 = (uint)DataConvert.DateTimeToInt(DateTime.Now);
		req.szText5 = DataConvert.StrToBytes(receipt);
		req.nTextLen4 = req.szText5.Length;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_UPLOAD_BILL_T>(req);
	}
	// 获取设备返回给LoginMananger  
	void getLoginMacAddress()
	{
		do_cmd("getMacAddress", "");
	}
}
