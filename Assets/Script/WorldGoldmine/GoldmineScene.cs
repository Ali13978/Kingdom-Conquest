using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

using CTSCMD;
using STCCMD;
using CMNCMD;
public class GoldmineScene : MonoBehaviour {
	
	static GoldmineScene mInst = null;
	
	public UILabel FixhourCol = null;
	public BaizhanPoster PosterCol = null;
	public UILabel RestTimeCol = null;
	public UILabel PrettyAreaName = null;
	public GoldmineFi goldmineFi = null;
	UIAnchor mDepth = null;
	
	public Transform TopBar = null;
	public Transform BottomBar = null;

	int mCurAreadID = 0;
	bool mPopMyGoldmine = false;
	string mPrettyRestTime;
	string mPrettyFixhour;
	
	void Awake() { mInst = this; onEnableDelegate(); }
	void OnDestroy() { mInst = null; onExceptDelegate(); }
	void onEnableDelegate()
	{
		if (RestTimeCol != null)
		{
			mPrettyRestTime = RestTimeCol.text;
			RestTimeCol.text = string.Format(mPrettyRestTime, 0);
		}
		
		if (PosterCol != null) {
			NGUITools.SetActive(PosterCol.gameObject,false);
		}
		
		if (FixhourCol != null) {
			mPrettyFixhour = FixhourCol.text;
			FixhourCol.enabled = false;
		}
		
		CombatManager.processStopCombatDelegate += OnProcessGoldmineStopCombatDelegate;
		CombatManager.processCombatRstDelegate += OnProcessGoldmineCombatRstDelegate;
		
		// IPAD 适配尺寸 ...
		float OffsetY = 0f;
		UIRoot root = GetComponent<UIRoot>();
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			if (root != null) { root.manualHeight = 320; }
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			OffsetY = 20f;
			if (root != null) { root.manualHeight = 360; }
		}
		
		if (TopBar != null) {
			TopBar.localPosition += new Vector3(0f,OffsetY,0f);
		}
		if (BottomBar != null) {
			BottomBar.localPosition -= new Vector3(0f,OffsetY,0f);
		}
	}
	
	void onExceptDelegate()
	{
		//print ("- onExceptDelegate");
		
		CombatManager.processStopCombatDelegate -= OnProcessGoldmineStopCombatDelegate;
		CombatManager.processCombatRstDelegate -= OnProcessGoldmineCombatRstDelegate;
		GoldmineInstance.processGetWorldGoldmineDelegate -= OnProcessGetWorldGoldmineDelegate;
	}
	
	// Use this for initialization
	void Start () {
		
		// iphone5 挪来 ...
		KeepOut.instance.ShowKeepOut();
	}
	
	public void AssignComm()
	{
		Hashtable fixhourMap = CommonMB.FixhourMBInfo_Map;
		uint ExcelID = 10;
		if (true == fixhourMap.ContainsKey(ExcelID))
		{
			FixhourMBInfo card = (FixhourMBInfo)fixhourMap[ExcelID];
			
			int imax = card.times.Count;
			string sReward = mPrettyFixhour;
			
			string d1 = U3dCmn.GetWarnErrTipFromMB(135);
			
			for(int i=0; i<imax; ++ i)
			{
				int timecc = card.times[i];
				sReward += " ";
				sReward += string.Format(d1,timecc);
			}
			
			if (FixhourCol != null)
			{
				FixhourCol.text = sReward;
				FixhourCol.enabled = true;
			}
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
	
	public float GetDepth()
	{
		if (mDepth == null) return 0f;
		return mDepth.depthOffset;
	}
	
	public void RequestGainWorldGoldmine()
	{
		CTS_GAMECMD_MY_WORLDGOLDMINE_T req;
		req.nCmd1 = (byte) CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint) SUB_CTS_MSG.CTS_GAMECMD_MY_WORLDGOLDMINE;
		TcpMsger.SendLogicData<CTS_GAMECMD_MY_WORLDGOLDMINE_T>(req);
	}
	
	public void GetGoldmineArea(int firstid)
	{
		LoadingManager.instance.ShowLoading();
		GoldmineInstance.processGetWorldGoldmineDelegate = OnProcessGetWorldGoldmineDelegate;
		GoldmineInstance.RequestGetWorldGoldmine(firstid, 0, 0);
	}
	
	public void GetMyGoldmine()
	{
		GoldmineInstance.RequestGetMyGoldmine();
	}
	
	static public void UpdateGoldmineGuard()
	{
		if (mInst == null) return;
		if (mInst.goldmineFi != null)
		{
			List<GoldmineCardData> cacheList = GoldmineInstance.instance.GetGoldmineList();
			mInst.goldmineFi.ApplyGoldmineList(cacheList);
		}
	}
	
	void OnProcessGetWorldGoldmineDelegate()
	{
		LoadingManager.instance.HideLoading();
		mCurAreadID = GoldmineInstance.instance.gid;
		Hashtable gmMap = CommonMB.WorldGoldmine_Map;
		if (true == gmMap.ContainsKey((uint)mCurAreadID))
		{
			GoldmineMBInfo newCard = (GoldmineMBInfo) gmMap[(uint)mCurAreadID];
			if (PrettyAreaName != null)
			{
				PrettyAreaName.text = newCard.name;
			}
		}
		
		if (goldmineFi != null)
		{
			List<GoldmineCardData> cacheList = GoldmineInstance.instance.GetGoldmineList();
			goldmineFi.ApplyGoldmineList(cacheList);
		}
		
		if (true == mPopMyGoldmine)
		{
			mPopMyGoldmine = false;
			goldmineFi.Pop1MyGuard();
		}
	}
	
	void OnGoldmineSceneClose()
	{
		NGUITools.SetActive(gameObject,false);
		
		// iphone5 挪开 ...
		KeepOut.instance.HideKeepOut();
		
		Destroy(gameObject);
		Resources.UnloadUnusedAssets();
	}
	
	static int GoldmineMaxnum()
	{
		Hashtable gmMap = CommonMB.WorldGoldmine_Map;
		int imax = gmMap.Count;
		
		Hashtable cmnDefMap = CommonMB.CmnDefineMBInfo_Map;
		uint cmn6_ID = 6;
		if (true == cmnDefMap.ContainsKey(cmn6_ID))
		{
			CmnDefineMBInfo card = (CmnDefineMBInfo)cmnDefMap[cmn6_ID];
			imax = (int) card.num;
		}
		
		return (imax);
	}
	
	void OnGoldmineScenePrev()
	{
		int imax = GoldmineMaxnum();
		int firstid = mCurAreadID;
		if (firstid == imax) {
			firstid = 1;
		}
		else
		{
			firstid = Mathf.Min(imax, mCurAreadID + 1);
		}
		
		if (firstid != mCurAreadID)
		{
			GetGoldmineArea(firstid);
		}
	}
	
	void OnGoldmineSceneBack()
	{
		int imax = GoldmineMaxnum();
		int firstid = mCurAreadID;
		if (firstid == 1) {
			firstid = imax;
		}
		else 
		{
			firstid = Mathf.Max(1, mCurAreadID - 1);
		}
		
		if (firstid != mCurAreadID)
		{
			GetGoldmineArea(firstid);
		}
	}
	
	void OnGoldmineSceneGetMyMine()
	{
		int nArea = GoldmineInstance.instance.MyGoldmine.nAreaID;
		if (nArea<1) 
		{
			int Tipset = BaizVariableScript.WORLDGOLDMINE_NO_ONE;
			PopTipDialog.instance.VoidSetText1(true,false,Tipset);
			return;
		}
			
		if (mCurAreadID != nArea)
		{
			mPopMyGoldmine = true;
			GetGoldmineArea(nArea);
		}
		else 
		{
			mPopMyGoldmine = false;
			goldmineFi.Pop1MyGuard();
		}
	}
	
	public void OnProcessGoldmineStopCombatDelegate(int nCombatType, int nRst, int eVictoryOrNo)
	{
		if (nCombatType == (int)CombatTypeEnum.COMBAT_PVE_WORLDGOLDMINE || 
			nCombatType == (int)CombatTypeEnum.COMBAT_PVP_WORLDGOLDMINE_ATTACK || 
			nCombatType == (int)CombatTypeEnum.COMBAT_PVP_WORLDGOLDMINE_DEFENSE )
		{
			// 胜利或失败 Victory
			if (PosterCol != null)
			{		
				if (eVictoryOrNo ==1) {
					PosterCol.Play("anim-victory");
				}
				else if (eVictoryOrNo == 2)
				{
					PosterCol.Play("anim-failure");
				}			
			}
		}
	}
	
	public void OnProcessGoldmineCombatRstDelegate(ulong nCombatID, int nCombatType)
	{		
		if (nCombatType == (int)CombatTypeEnum.COMBAT_PVE_WORLDGOLDMINE || 
			nCombatType == (int)CombatTypeEnum.COMBAT_PVP_WORLDGOLDMINE_ATTACK || 
			nCombatType == (int)CombatTypeEnum.COMBAT_PVP_WORLDGOLDMINE_DEFENSE )
		{
			SortedList<ulong, CombatFighting> combatMap = CombatManager.instance.GetCombatFightingMap(); 	
			CombatFighting comFighting = null;
			if (true == combatMap.TryGetValue(nCombatID,out comFighting))
			{
				// 胜负 00
				int endTimeSec = comFighting.nBackRestTime + DataConvert.DateTimeToInt(DateTime.Now);
				Begin(endTimeSec);
				
				// 添加战斗委托 CombatDataDelegate
				OnProcessCombatUnitDataDelegate(nCombatID);
			}
		}
	}
	
	public void Begin(int endTimeSec)
	{
		StopCoroutine("GoldmineCooldown");
		StartCoroutine("GoldmineCooldown", endTimeSec);
	}
	
	// 战斗Cooldown 
	IEnumerator GoldmineCooldown(int endTimeSec)
	{
		if (RestTimeCol != null)
		{
			int nSecs = (endTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
			if (nSecs <0) nSecs = 0;
			
			while(nSecs > 0)
			{
				nSecs = endTimeSec - DataConvert.DateTimeToInt(DateTime.Now);
				if(nSecs <0)
					nSecs = 0;
				
				RestTimeCol.text = string.Format(mPrettyRestTime,nSecs);
				yield return new WaitForSeconds(1);
			}
		}
	}

	static public void OnProcessCombatUnitDataDelegate(ulong nCombatID)
	{	
		if (mInst == null) return;
		mInst.GetGoldmineArea(mInst.mCurAreadID);
		
		if (true == CombatManager.instance.TypeFighting(nCombatID))
		{
			GameObject go = U3dCmn.GetObjFromPrefab("BaizBattlefieldWin");
			if ( go != null)
			{
				float depth = mInst.GetDepth() - BaizVariableScript.DEPTH_OFFSET * 2.0f;
				go.SendMessage("Depth", depth);
			}
		}
	}
	
}
