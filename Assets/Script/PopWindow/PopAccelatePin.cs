using UnityEngine;
using System.Collections;
using System;

using CMNCMD;

public class PopAccelatePin : MonoBehaviour {
	
	public UICheckbox chkNow = null;
	public UICheckbox chk10min = null;
	public UICheckbox chk1hours = null;
	public UICheckbox chk2hours = null;
	public UICheckbox chk4hours = null;
	public UICheckbox chkCrystal = null;
	public UICheckbox chkDiamond = null;

	public GameObject btnAccel = null; 	
	public GameObject btnClose = null;
	public UILabel RestTime = null;
	public UILabel DimondCol = null;
	public UILabel CrystalCol = null;
	
	public UILabel _nowCol = null;
	public UILabel _10minCol = null;
	public UILabel _1hourCol = null;
	public UILabel _2hoursCol = null;
	public UILabel _4hoursCol = null;
	
	int AccelEndTimeSecs = 0;
	float AccelCostUnit = 50f;
	
	static string AcctMoneyType = "Acct:{0}:MoneyType"; // 记录用户使用Money方式 ...
	
	UIAnchor mDepth = null;
	
	static float[] CostMinAccel = new float[5]{ 0f, 10f, 60f, 120f, 240f };
	
	public delegate void ProcessAccelDelegate(int money_type, int nTime);
	static public ProcessAccelDelegate processAccelDelegate;
	
	void Awake()
	{
		// IPAD 适配尺寸 ...
		UIRoot root = GetComponent<UIRoot>();
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			if (root != null) { root.manualHeight = 320; }
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			if (root != null) { root.manualHeight = 360; }
		}
	}
	
	void OnDestroy() { PopAccelatePin.processAccelDelegate = null; }

	// Use this for initialization
	void Start () {
	
		if (btnAccel != null) {
			UIEventListener.Get(btnAccel).onClick = clickBtnAccelDelegate;
		}
		if (btnClose != null) {
			UIEventListener.Get(btnClose).onClick = clickBtnCloseDelegate;
		}
	}
	
	public void Depth(float depth)
	{
		if (mDepth == null) {
			mDepth = GetComponentInChildren<UIAnchor>();
		}
		
		if (mDepth == null) return;
		mDepth.depthOffset = depth;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	// 删除自己 ...
	void DismissPanel()
	{
		PopAccelatePin.processAccelDelegate = null;
		Destroy(gameObject);
	}
	
	//倒计时 
	IEnumerator TimeCountdown(int endTimeSecs)
	{
		int secs = (int)(endTimeSecs - DataConvert.DateTimeToInt(DateTime.Now));
		if (secs <0) secs = 0;
		while(secs>0)
		{
			secs =(int)(endTimeSecs - DataConvert.DateTimeToInt(DateTime.Now));
			if(secs <0) secs = 0;
			int hour = secs/3600;
			int minute = secs/60%60;
			int second = secs%60;
			if(hour>=100)
				RestTime.text =string.Format("{0}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			else
				RestTime.text =string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			
			if (_nowCol != null)
			{
				string cc = U3dCmn.GetWarnErrTipFromMB(310);
				int  cost = U3dCmn.UpperInt(secs/(AccelCostUnit));
				_nowCol.text = string.Format(cc, cost);
			}
			
			yield return new WaitForSeconds(1);
		}
		
		// <新手引导> 超时了 ...
		if (NewbieChuZheng.processNewbieArmyAccel != null)
		{
			NewbieChuZheng.processNewbieArmyAccel();
			NewbieChuZheng.processNewbieArmyAccel = null;
		}
		
		// 删除吧 ...
		PopAccelatePin.processAccelDelegate = null;
		Destroy(gameObject);
	}
	
	// 确定加速 ...
	void clickBtnAccelDelegate(GameObject go)
	{
		UICheckbox[] cacheList = new UICheckbox[5]{ chkNow, chk10min, chk1hours, chk2hours, chk4hours };
		
		int idChoice = -1;
		for (int i=0; i<5; ++ i)
		{
			if (true == cacheList[i].isChecked)
			{
				idChoice = i;
				break;
			}
		}
		
		uint cost = 0;
		int seconds = 0; 
		if (idChoice == 0)
		{
			float secs = (float)((this.AccelEndTimeSecs-DataConvert.DateTimeToInt(DateTime.Now)));
			cost = (uint) U3dCmn.UpperInt(secs/(this.AccelCostUnit));
			seconds = U3dCmn.UpperInt(secs);
		}
		else 
		{
			float secs = CostMinAccel[idChoice] * 60;
			cost = (uint) U3dCmn.UpperInt(secs/(this.AccelCostUnit));
			seconds = U3dCmn.UpperInt(secs);
		}
		
		int money_type = 0;
		
		CMN_PLAYER_CARD_INFO info = CommonData.player_online_info;
		if(true == chkDiamond.isChecked)
		{
			money_type = (int)CMNCMD.money_type.money_type_diamond;
			string pay = string.Format(AcctMoneyType, info.AccountID);
			PlayerPrefs.SetInt(pay, money_type);
			
			//检查钻石是否够用  ... 
			if (info.Diamond < cost)
			{
				int Tipset = (int)MB_WARN_ERR.DIAMOND_NOT_ENOUGH;
				//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB(Tipset));
				U3dCmn.ShowRechargePromptWindow();
				return ;
			}
		}
		else if(true == chkCrystal.isChecked)
		{
			money_type = (int)CMNCMD.money_type.money_type_crystal;
			string pay = string.Format(AcctMoneyType, info.AccountID);
			PlayerPrefs.SetInt(pay, money_type);
			
			//检查水晶是否够用  ...
			if (info.Crystal < cost)
			{
				int Tipset = (int)MB_WARN_ERR.CRYSTAL_NOT_ENOUGH;
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB(Tipset));
				return;
			}
		}
		
		// 加速后  ...
		if (processAccelDelegate != null) 
		{
			processAccelDelegate( money_type, seconds);
			processAccelDelegate = null;
		}
		else
		{
			// 删除自己 ...
			DismissPanel();
		}
	}
	
	void clickBtnCloseDelegate(GameObject go)
	{
		PopAccelatePin.processAccelDelegate = null;
		Destroy(gameObject);
	}
	
	public void SetAccel(int endTimeSecs)
	{
		AccelEndTimeSecs = endTimeSecs;
	
		CMN_PLAYER_CARD_INFO card = CommonData.player_online_info;
		string pay = string.Format(AcctMoneyType, card.AccountID);
		int money_type = (int) CMNCMD.money_type.money_type_crystal;
		if (true == PlayerPrefs.HasKey(pay))
		{
			money_type = PlayerPrefs.GetInt(pay);
			if (money_type == (int) CMNCMD.money_type.money_type_diamond)
			{
				chkDiamond.isChecked = true;
			}
		}
		
		if (CrystalCol != null) {
			CrystalCol.text = card.Crystal.ToString();
		}
		if (DimondCol != null) {
			DimondCol.text = card.Diamond.ToString();
		}
				
		float sec = (float)(endTimeSecs-DataConvert.DateTimeToInt(DateTime.Now));
		Hashtable accelMap = CommonMB.CmnDefineMBInfo_Map;
		uint idAccel = 2;
		if (true == accelMap.ContainsKey(idAccel))
		{
			CmnDefineMBInfo info = (CmnDefineMBInfo) accelMap[idAccel];
			AccelCostUnit = info.num;
			
			string cc = U3dCmn.GetWarnErrTipFromMB(310);
			int  cost = U3dCmn.UpperInt(sec/(info.num));
			_nowCol.text = string.Format(cc, cost);
			
			UILabel[] cacheList = new UILabel[4] { _10minCol, _1hourCol, _2hoursCol, _4hoursCol };
			for (int i=0; i<4; ++ i)
			{
				sec = CostMinAccel[i+1] * 60f;
				cost = U3dCmn.UpperInt(sec/(info.num));
				cacheList[i].text = string.Format(cc, cost);
			}
		}
		
		StopCoroutine("TimeCountdown");
		StartCoroutine("TimeCountdown",endTimeSecs);
	}
	
	// <新手引导> 加速功能 ...
	void NewbieDoArmyAccel()
	{
		clickBtnAccelDelegate(btnAccel);
	}
}
