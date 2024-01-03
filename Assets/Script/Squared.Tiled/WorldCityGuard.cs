using UnityEngine;
using System.Collections;
using System;
using CMNCMD;
using CTSCMD;
using STCCMD;

// 城保描述 ...
public class WorldCityGuard : MonoBehaviour {
	
	public UILabel cityName = null;				// 城堡名称 ...
	public UILabel honorCupCol = null;			// 奖杯数量 ...
	public UILabel honorTip = null;				// 城堡说明 ...
	public UILabel cityProfCol = null;			// 初始战力 ...
 	public UILabel cityProfNow = null;			// 当前战力 ...
	public UILabel cityRecoverTime = null;		// 恢复倒计时 ...
	public UILabel cityWarCol = null;			// 战争结束 ...
	public UILabel cityBatRestTime = null;		// 战争结束时间 ...
	public UISprite cityIcon = null;			// 名城图标 ...
	public UILabel cityGoToTime = null;			// 前往时间 ....

	public GameObject BtnCombat = null;			// 开启战斗 ...
	public GameObject BtnClose = null;			// 关闭按钮 ...
	
	ulong cityNowAutoID = 0;				// 当前查看的荣耀城堡ID ...
	int cityNowCoordX = 0;					// 当前城堡坐标 ...
	int cityNowCoordY = 0;					// 当前城堡坐标 ...
	
	bool HonorWarCanFini = false;			// 战争结束 ...
	
	string mPrettyTip;
	string mRecoverFmt;
	string mBatRestFmt;
	
	UIAnchor mDepth = null;
	
	void Awake()
	{
		if (honorTip != null) {
			mPrettyTip = honorTip.text;
			honorTip.enabled = false;
		}
		if (honorCupCol != null) {
			honorCupCol.enabled = false;
		}
		if (cityProfCol != null) {
			cityProfCol.enabled = false;
		}
		if (cityProfNow != null) {
			cityProfNow.enabled = false;
		}
		if (cityRecoverTime != null) {
			mRecoverFmt = cityRecoverTime.text;
			cityRecoverTime.enabled = false;
		}
		if (cityWarCol != null) {
			cityWarCol.enabled = false;
		}
		if (cityBatRestTime != null) {
			mBatRestFmt = cityBatRestTime.text;
			cityBatRestTime.enabled = false;
		}
		if (cityGoToTime != null) {
			cityGoToTime.enabled = false;
		}
		if (cityIcon != null) {
			cityIcon.enabled = false;
		}
	}
	
	// Use this for initialization
	void Start () {
		
		if (BtnClose != null) {
			UIEventListener.Get(BtnClose).onClick = clickBtnCloseDelegate;
		}
		if (BtnCombat != null) {
			UIEventListener.Get(BtnCombat).onClick = clickBtnHonorCombatDelegate;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Depth(float depth)
	{
		if (mDepth == null) {
			mDepth = GetComponentInChildren<UIAnchor>();
		}
		
		if (mDepth == null) return;
		mDepth.depthOffset = depth;
	}
	
	public float GetDepth()
	{
		if (mDepth == null) return 0f;
		return mDepth.depthOffset;
	}
	
	// 显示出征时间 ...
	public void SetGoToTime(int nSecs)
	{
		int march_time  = nSecs;
		if(CommonData.player_online_info.Vip > 0)
		{
			VipRightInfo vipinfo = (VipRightInfo)CommonMB.VipRightInfo_Map[(int)CommonData.player_online_info.Vip];
			march_time = (int)(nSecs*vipinfo.SubBattleTime);
		}
		
		if (cityGoToTime == null) return;
		
		int hour = march_time/3600;
		int minute = march_time/60%60;
		int second = march_time%60;
				
		if(hour>=100)
		{
			string cc = string.Format("{0}",hour)+":" 
				+ string.Format("{0:D2}",minute)+":" 
				+ string.Format("{0:D2}",second);
			cityGoToTime.text = cc;
			cityGoToTime.enabled = true;
		}
		else
		{
			string cc = string.Format("{0:D2}", hour)+":" 
				+ string.Format("{0:D2}", minute)+":" 
				+ string.Format("{0:D2}", second);
			cityGoToTime.text = cc;
			cityGoToTime.enabled = true;
		}
	}
	
	// 关闭窗口 ...
	void clickBtnCloseDelegate(GameObject go)
	{
		Destroy (gameObject);
	}
	
	void clickBtnHonorCombatDelegate(GameObject tween)
	{
		CMN_PLAYER_CARD_INFO info = CommonData.player_online_info;
		if (info.AllianceID == 0) 
		{
			int Tipset = BaizVariableScript.COMBAT_HONOR_NO_JOIN_UNION;
			PopTipDialog.instance.VoidSetText1(true,false,Tipset);
			return;
		}
		
		ulong AutoID = cityNowAutoID;
		Hashtable hcMap = TiledStorgeCacheData.HonorCityMap;
		if (true == hcMap.ContainsKey(AutoID))
		{
			WorldCity card = (WorldCity) hcMap[AutoID];
					
			GameObject go = U3dCmn.GetObjFromPrefab("PopGeneralCrushWin");
			if (go == null) return;
			
			// 获取荣耀之城名称 ...
			string name = DataConvert.BytesToStr(card.szName8);
			PopGeneralCrush win1 = go.GetComponent<PopGeneralCrush>();
			if (win1 != null)
			{
				float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
				win1.Depth(depth);
				
				float d1 = (float) Mathf.Abs((int)info.PosX - cityNowCoordX);
				float d2 = (float) Mathf.Abs((int)info.PosY - cityNowCoordY);
				
				float sq1 = Mathf.Sqrt(d1*d1 + d2*d2);
				float sq2 = Mathf.Sqrt(400f*400f*2f);
				Hashtable cgtMap = CommonMB.CombatGoToTimeMBInfo_Map;
				uint combatTypeID = (uint)CombatTypeEnum.COMBAT_PVP_ATTACK;
				int secs =0;
				if (true == cgtMap.ContainsKey(combatTypeID))
				{
					CombatGoToTimeMBInfo timeinfo = (CombatGoToTimeMBInfo) cgtMap[combatTypeID];
					secs = Mathf.CeilToInt((sq1/sq2) * timeinfo.GoToTime);
		
				}
				//int secs = Mathf.CeilToInt((sq1/sq2) * 8f * 3600f); /// 最长时间为8小时 ...

				win1.ApplyCastleID(AutoID, name);
				win1.ApplyGoToTime(secs);
			}
		}
		
		// 删除自己 ...
		Destroy(gameObject);
	}
	
	public void ApplySetHonorCity(ulong AutoID, int mapCoordX, int mapCoordY)
	{
		cityNowAutoID = AutoID; 
		cityNowCoordX = mapCoordX;
		cityNowCoordY = mapCoordY;
		
		Hashtable hcMap = TiledStorgeCacheData.HonorCityMap;
		if (true == hcMap.ContainsKey(AutoID))
		{
			WorldCity card = (WorldCity) hcMap[AutoID];
			
			if (honorCupCol != null) {
				honorCupCol.text = string.Format("{0}",card.nCup6);
				honorCupCol.enabled = true;
			}
			if (cityProfCol != null) {
				cityProfCol.text = string.Format("{0}",card.nInitProf9);
				cityProfCol.enabled = true;
			}
			if (cityProfNow != null) {
				cityProfNow.text = string.Format("{0}",card.nCurProf11);
				cityProfNow.enabled = true;
			}
			if (honorTip != null) {
				honorTip.text = string.Format(mPrettyTip, card.nInitRecover10);
				honorTip.enabled = true;
			}
			if (cityName != null) 
			{
				cityName.text = DataConvert.BytesToStr(card.szName8);
				cityName.enabled = true;
			}
			if (cityIcon != null)
			{
				cityIcon.enabled = true;
				if (card.nClass2 == 1) cityIcon.spriteName = "gaojicheng";
				if (card.nClass2 == 2) cityIcon.spriteName = "zhongjicheng";
				if (card.nClass2 == 3) cityIcon.spriteName = "dijicheng";
			}
			if (card.nRecover5>0)
			{
				int endTimeSecs = card.nRecover5 + DataConvert.DateTimeToInt(DateTime.Now);
				
				// 开台倒计时 .....
				StopCoroutine("RecoverCooldown");
				StartCoroutine("RecoverCooldown",endTimeSecs);
			}
			else 
			{
				// 填写为 "正常" ...
				cityRecoverTime.text = U3dCmn.GetWarnErrTipFromMB(624);
				cityRecoverTime.enabled = true;
			}
			
			int ccOver = TiledStorgeCacheData.HonorWarRestTime;
			if (ccOver > 0)
			{
				HonorWarCanFini = false;
				int endTimeSecs = ccOver + DataConvert.DateTimeToInt(DateTime.Now);
				
				// 开始倒计时 ...
				StopCoroutine("HonorWarCooldown");
				StartCoroutine("HonorWarCooldown", endTimeSecs);
			}
		}
	}
	// 
	IEnumerator HonorWarCooldown(int endTimeSecs)
	{
		int secs = (int)(endTimeSecs - DataConvert.DateTimeToInt(DateTime.Now));
		if (secs <0) secs = 0;
		
		// 显示倒计时 ...
		cityWarCol.enabled = true;
		cityBatRestTime.enabled = true;
		while(secs>0)
		{
			secs =(int)(endTimeSecs - DataConvert.DateTimeToInt(DateTime.Now));
			if(secs <0) secs = 0;
			int day	= secs/86400;
			int hour = secs/3600%24;
			int minute = secs/60%60;
			int second = secs%60;
			if(day>0)
			{	
				cityBatRestTime.text = string.Format(mBatRestFmt, day);
			}
			else
			{
				cityBatRestTime.text = 
					string.Format("{0:D2}", hour)+":"+
					string.Format("{0:D2}", minute)+":"+
					string.Format("{0:D2}", second);
			}

			// 1秒1跳 ....
			yield return new WaitForSeconds(1);
		}
		
		cityBatRestTime.text = "00:00:00";
		HonorWarCanFini = true;
	}
	
	void LateUpdate()
	{
		if (HonorWarCanFini == true)
		{
			HonorWarCanFini = false;
						
			// 关闭自己 ...
			Destroy(gameObject);
		}
	}
	
	// 战斗力恢复 ...
	IEnumerator RecoverCooldown(int endTimeSecs)
	{
		int secs = (int)(endTimeSecs - DataConvert.DateTimeToInt(DateTime.Now));
		if (secs <0) secs = 0;
		
		// 显示倒计时 ...
		cityRecoverTime.enabled = true;
		while(secs>0)
		{
			secs =(int)(endTimeSecs - DataConvert.DateTimeToInt(DateTime.Now));
			if(secs <0) secs = 0;
			int hour = secs/3600;
			int minute = secs/60%60;
			int second = secs%60;
			if(hour>=100)
			{
				string cc1 = 
					string.Format("{0}", hour)+":"+
					string.Format("{0:D2}", minute)+":"+
					string.Format("{0:D2}", second);
				
				cityRecoverTime.text = string.Format(mRecoverFmt, cc1);
			}
			else
			{
				string cc2 = 
					string.Format("{0:D2}", hour)+":"+
					string.Format("{0:D2}", minute)+":"+
					string.Format("{0:D2}", second);
				
				cityRecoverTime.text = string.Format(mRecoverFmt, cc2);
			}
			
			// 1秒1跳 ....
			yield return new WaitForSeconds(1);
		}
		
		cityRecoverTime.text = U3dCmn.GetWarnErrTipFromMB(624);
		
		// 显示恢复当前战斗力 ...
		if (cityProfNow != null) 
		{
			cityProfNow.text = cityProfCol.text;
			cityProfNow.enabled = true;
		}
	}
}
