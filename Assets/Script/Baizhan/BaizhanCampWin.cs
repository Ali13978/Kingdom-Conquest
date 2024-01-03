using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using STCCMD;
using CTSCMD;
using CMNCMD;
public class BaizhanCampWin : MonoBehaviour {
	
	UIAnchor mDepth = null;

	private int mCGuardLootID = 0;
	
	public UILabel AutoCombatCol = null;
	public UILabel RestTimeCol = null;
	public UILabel RetryTimesCol = null;
	public UIButton AutoButtonCol = null;
	
	public Transform TopBar = null;
	public Transform BottomBar = null;
	
	string mPrettyRestTime = "";
	string mPrettyRetry;
	public BaizFi PrettyBaizFi = null;
	public BaizhanPoster PosterCol = null;
	
	bool inCurFighting = false;

	void Awake() { 		
		if (RestTimeCol != null) 
		{
			mPrettyRestTime = RestTimeCol.text;
			RestTimeCol.text = string.Format(mPrettyRestTime,0);
		}
		if (RetryTimesCol != null)
		{
			mPrettyRetry = RetryTimesCol.text;
			RetryTimesCol.text = string.Format(mPrettyRetry, 0,1);
		}
		if (PosterCol != null) {
			NGUITools.SetActive(PosterCol.gameObject, false);
		}
		
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
		
		onEnableDelegate(); 
	}

	void OnDestroy() { onExceptDelegate(); }
	
	void onEnableDelegate()
	{
		CombatManager.processCombatRstDelegate += OnProcessBaizCombatRstDelegate;
		CombatManager.processStopCombatDelegate += OnProcessStopCombatDelegate;
	}
	
	void onExceptDelegate()
	{
		//print ("- onExceptDelegate");
		
		CombatManager.processCombatRstDelegate -= OnProcessBaizCombatRstDelegate;
		CombatManager.processStopCombatDelegate -= OnProcessStopCombatDelegate;
		
		// 取消委托 ...
		CombatManager.processAutoCombatDelegate -= OnProcessStopAutoCombat;
		CombatManager.processCombatDataDelegate -= OnProcessBaizCombatDataDelegate;
		
		BaizInstanceManager.processInstanceDataDelegate -= OnProcessInstanceDataDelegate;
		BaizInstanceManager.processCGuardLootDelegate -= OnProcessBaizGuardLootDelegate;
		BaizInstanceManager.processListInstanceStatusDelegate -= OnProcessListInstanceStatusDataDelegate;
		BaizInstanceManager.processDestroyInstanceDelegate = null;
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// 界面z设置 ...
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
	
	// 开启 自动挑战模式 ...
	void OnBaizAutoTiaoZhan()
	{		
		bool isAuto = BaizhanInstance.instance.isAutoCombat;
		if (isAuto == false)
		{
			GameObject go = U3dCmn.GetObjFromPrefab("BaizChuZhenWin");
			if (go == null) return;
			
			BaizChuZhenWin t = go.GetComponent<BaizChuZhenWin>();
			if (t != null)
			{
				float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;	
				t.Depth(depth);
				t.idGuard = PrettyBaizFi.level;
				t.GetBaizhanGenerals();
				t.SetCScene(this);
			}
		}
		else 
		{
			ulong nInstanceID = BaizhanInstance.instance.idInstance;
			CombatManager.processAutoCombatDelegate += OnProcessStopAutoCombat;
			CombatManager.RequestStopAutoCombat(nInstanceID, 0);
		}
	}
	
	// 停止自动战斗 ...
	void OnProcessStopAutoCombat()
	{
		bool isAuto = BaizhanInstance.instance.isAutoCombat;
		SetBaizAutoCombat((isAuto == false));
		
		if (true == PrettyBaizFi.isOnDoor())
		{
			DisableAutoTiao();
		}
	}
	
	// 开启观战 ...
	void OnBaizAutoGuanZhan()
	{
		CombatFighting comFighting = BaizhanInstance.instance.idFighting;
		if (comFighting == null)
		{
			ulong idCombat = BaizhanInstance.instance.idCombat;
			CombatManager.processCombatDataDelegate = OnProcessBaizCombatDataDelegate;
			CombatManager.instance.ApplyBaizFighting(idCombat);
		}
		else 
		{
			Globe.comFighting = comFighting;
			OnProcessBaizCombatDataDelegate(comFighting.nCombatID);
		}
	}
	
	void OnProcessBaizCombatDataDelegate(ulong nCombatID)
	{
		GameObject go = U3dCmn.GetObjFromPrefab("BaizBattlefieldWin");
		if ( go != null)
		{
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET * 3.0f;
			go.SendMessage("Depth", depth);
		}
	}
	
	// 结束副本2 ...
	void OnButtonAutoDestroyInstance(GameObject go)
	{
		ulong nInstanceID = BaizhanInstance.instance.idInstance;
		
		LoadingManager.instance.ShowLoading();
		BaizInstanceManager.processDestroyInstanceDelegate = null;
		BaizInstanceManager.processDestroyInstanceDelegate = OnProcessDestroyInstanceDelegate;
		BaizInstanceManager.RequestDestroyInstance(100, nInstanceID);
		PopTipDialog.instance.Dissband();
	}
	
	// 结束副本询问1 ...
	void OnBaizAutoDestroy()
	{
		ulong nInstanceID = BaizhanInstance.instance.idInstance;
		if (nInstanceID == 0) return;
		
		int Tipset = BaizVariableScript.INSTANCE_DESTROY_ASKING_OK;
		PopTipDialog.instance.VoidSetText1(true,true,Tipset);
		PopTipDialog.instance.VoidButton1(OnButtonAutoDestroyInstance);
	}
	
	void OnProcessDestroyInstanceDelegate()
	{
		LoadingManager.instance.HideLoading();
		BaizhanInstance.instance.idInstance = 0;
		
		if (SoundManager.palying_music == CMNCMD.BG_MUSIC.BATTLE_MUSIC)
		{
			SoundManager.StopBattleSound();
			SoundManager.PlayMainSceneSound();
		}
		
		// iphone5 遮挡挪开 ...
		KeepOut.instance.HideKeepOut();
		
		// 删除副本图片 ...
		Destroy(gameObject);
		Resources.UnloadUnusedAssets();
	}
	
	// 关闭界面 ...
	void OnBaizhanCampClose()
	{
		if (SoundManager.palying_music == CMNCMD.BG_MUSIC.BATTLE_MUSIC)
		{
			SoundManager.StopBattleSound();
			SoundManager.PlayMainSceneSound();
		}
		
		// iphone5 遮挡挪开 ...
		KeepOut.instance.HideKeepOut();
		
		Destroy(gameObject);
		Resources.UnloadUnusedAssets();
	}
	
	// 战斗结果返回处理 ...
	public void OnProcessBaizCombatRstDelegate(ulong nCombatID, int nCombatType)
	{
		if (nCombatType != (int)CombatTypeEnum.COMBAT_INSTANCE_BAIZHANBUDAI) return;
		BaizhanInstance.instance.idCombat = nCombatID;
		BaizhanInstance.instance.idFighting = null; /// 清空等待新的战斗信息来了 ... 
		
		// Rest time calc
		SortedList<ulong,CombatFighting> combatMap = CombatManager.instance.GetCombatFightingMap();
		CombatFighting comFighting = null;
		if (true == combatMap.TryGetValue(nCombatID, out comFighting))
		{			
			int endTimeSec = comFighting.nBackRestTime + DataConvert.DateTimeToInt(DateTime.Now);
			this.Begin(endTimeSec);
			
			bool onAutoCombat = BaizhanInstance.instance.isAutoCombat;
			if (false == onAutoCombat)
			{
				CombatManager.processCombatDataDelegate = OnProcessBaizCombatDataDelegate;
				CombatManager.instance.ApplyBaizFighting(nCombatID);
			}
			else if(false == comFighting.eVictoryOrNo)
			{
				CombatManager.processCombatDataDelegate = null;
				CombatManager.RequestGetCombatData(nCombatID);
			}
		}
	}
	
	// 请求副本状态, 回到启动界面 ...
	void OnWaitingListInstanceStatus(GameObject go)
	{
		LoadingManager.instance.ShowLoading();
		BaizInstanceManager.processListInstanceStatusDelegate += OnProcessListInstanceStatusDataDelegate;
		BaizInstanceManager.RequestListInstanceStatusData();
		PopTipDialog.instance.Dissband();
	}
	
	// 副本结束战斗, 更新关卡数标记 ...
	public void OnProcessStopCombatDelegate(int nCombatType, int nRst, int eVictoryOrNo)
	{		
		if (nCombatType != (int)CombatTypeEnum.COMBAT_INSTANCE_BAIZHANBUDAI) return;
		
		// 副本通关 ...
		if (nRst == (int) STC_GAMECMD_OPERATE_STOP_COMBAT_T.enum_rst.RST_INSTANCE_FINISH)
		{
			if (CommonData.player_online_info.WangZhe < 100) {
				CommonData.player_online_info.WangZhe = 100;
			}
			
			// 等级提升 100 ...
			PrettyBaizFi.StopAtPoster();
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET * 2.0f;
			int Tipset = BaizVariableScript.COMBAT_INSTANCE_FINISH;
			PopTipDialog.instance.VoidSetText1(true,false,Tipset);
			PopTipDialog.instance.VoidButton1(OnWaitingListInstanceStatus);
			PopTipDialog.instance.Popup1(depth);
		}
		else if (nRst == (int) STC_GAMECMD_OPERATE_STOP_COMBAT_T.enum_rst.RST_INSTANCE_FAILURE)
		{
			// 副本失败退出 ...
			PrettyBaizFi.StopAtPoster();
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET * 2.0f;
			int Tipset = BaizVariableScript.COMBAT_INSTANCE_OVER;
			PopTipDialog.instance.VoidSetText1(true,false,Tipset);
			PopTipDialog.instance.VoidButton1(OnWaitingListInstanceStatus);
			PopTipDialog.instance.Popup1(depth);
		}
		else 
		{			
			if (AutoButtonCol != null) {
				AutoButtonCol.isEnabled = true;
			}
			
			// 胜利或失败   Victory
			if (PosterCol != null)
			{
				if (eVictoryOrNo ==1) 
				{
					BaizhanInstance.instance.idFighting = null;
					
					// 播放成功动画 ...
					PosterCol.Play("anim-victory");
				}
				else if (eVictoryOrNo == 2)
				{
					CombatFighting comFighting = null;
					ulong combatID = BaizhanInstance.instance.idCombat;
					SortedList<ulong,CombatFighting> comFightMap = CombatManager.instance.GetCombatFightingMap();
					comFightMap.TryGetValue(combatID, out comFighting);
					BaizhanInstance.instance.idFighting = comFighting;
					// 播放失败动画 ...
					PosterCol.Play("anim-failure");
				}
			}
			
			// 获取副本的信息 RequestGetInstanceData	
			ulong nInstanceID = BaizhanInstance.instance.idInstance;
			BaizInstanceManager.processInstanceDataDelegate += OnProcessInstanceDataDelegate;
			BaizInstanceManager.RequestGetInstanceData(100, nInstanceID);
		}
	}
	
	public void OnProcessInstanceDataDelegate(BaizInstanceGroup newGroup)
	{
		
		if (newGroup.nInstanceID != BaizhanInstance.instance.idInstance) return;
		//EPIC战役 
		if(BaizInstanceManager.EasyOrEpic == 2)
		{
			if ((int)CommonData.player_online_info.WangZhe-100 < newGroup.nCurLevel-1) 
			{
				CommonData.player_online_info.WangZhe = (uint)(newGroup.nCurLevel-1+100);
			}
		}
		else
		{
			if ((int)CommonData.player_online_info.WangZhe < newGroup.nCurLevel-1) {
				CommonData.player_online_info.WangZhe = (uint)(newGroup.nCurLevel-1);
			}
		}
		
		
		bool onOrStop = (newGroup.nAutoCombat==1);
		SetBaizAutoCombat(onOrStop);
		
		PrettyBaizFi.SetBaizhanCurLevel(newGroup.nCurLevel);
		
		if (onOrStop == true)
		{
			PrettyBaizFi.SetBaizOnDoor(newGroup.nCurLevel);
		}
		
		if (RetryTimesCol != null)
		{
			int nRetryTimes = 0;
			Hashtable dmMap = CommonMB.InstanceDescMBInfo_Map;
			uint ExcelID = (uint)CombatTypeEnum.COMBAT_INSTANCE_BAIZHANBUDAI;
			if (true == dmMap.ContainsKey(ExcelID))
			{
				InstanceDescMBInfo card = (InstanceDescMBInfo)dmMap[ExcelID];
				nRetryTimes = card.nRetryTimes;
			}
			
			RetryTimesCol.text = string.Format(mPrettyRetry, nRetryTimes - newGroup.nRetryTimes, nRetryTimes+1);
		}
	}
	
	void OnProcessListInstanceStatusDataDelegate()
	{
		LoadingManager.instance.HideLoading();
		GameObject go = U3dCmn.GetObjFromPrefab("BaizhanLaunchWin");
		if (go == null ) return;
		
		BaizhanLaunchWin t = go.GetComponent<BaizhanLaunchWin>();
		if (t != null)
		{
			float depth = -BaizVariableScript.DEPTH_OFFSET;
			t.Depth(depth);
			t.ApplyBaizInstanceStatus();
			
			NGUITools.SetActive(gameObject, false);
			Destroy(gameObject); // 隐藏自己    hide
			
			// iphone5 挪开 ...
			KeepOut.instance.HideKeepOut();
			
			// -- 除去启动画面图 ... 
			Resources.UnloadUnusedAssets();
		}
	}
	
	/// <summary>
	/// Popups the baizhan on guard window.
	/// </summary>
	public void PopupBaizhanGuardWin(int gid)
	{		
		GameObject go = U3dCmn.GetObjFromPrefab("BaizOnGuardWin");
		if (go == null ) return;
		
		BaizOnGuardWin win1 = go.GetComponent<BaizOnGuardWin>();
		if (win1 != null)
		{		
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			win1.Depth(depth);
			win1.idGuard = gid;
			win1.GetCombatProf();
			win1.GetBaizhanGenerals();
			win1.SetCScene(this);
		}
	}
	
	public void PopupBaizhanGuardLoot(int gid)
	{	
		mCGuardLootID = gid;
		
		ulong nInstanceID = BaizhanInstance.instance.idInstance;
		BaizInstanceManager.processCGuardLootDelegate += OnProcessBaizGuardLootDelegate;
		BaizInstanceManager.RequestInstanceGuardLoot(100,nInstanceID);
	}
	
	// 显示当前已获取的奖励 ....
	void OnProcessBaizGuardLootDelegate(List<BaizInstanceLoot> lootList)
	{		
		GameObject go = U3dCmn.GetObjFromPrefab("BaizhanLootScript");
		if (go == null) return;
		
		BaizhanLootScript t = go.GetComponent<BaizhanLootScript>();
		if (t != null)
		{
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET * 2.0f;
			
			t.Depth(depth);
			t.gid = mCGuardLootID;
			t.ApplyLootList(lootList);
		}
	}
	
	public void SetBaizAutoCombat(bool on)
	{
		BaizhanInstance.instance.isAutoCombat = on;
		if (AutoCombatCol != null)
		{
			if (true == on)
			{
				string cc = U3dCmn.GetWarnErrTipFromMB(134);
				AutoCombatCol.text = cc;
			}
			else 
			{
				string cc = U3dCmn.GetWarnErrTipFromMB(133);
				AutoCombatCol.text = cc;
			}
		}
	}
	
	// 刷新当前的副本数据 ...
	public void ApplyInstanceGroup(BaizInstanceGroup newGroup)
	{
		BaizhanInstance.instance.idInstance = newGroup.nInstanceID;
		inCurFighting = false;
		
		ulong nCombatID = BaizhanInstance.instance.idCombat;
		CombatDescUnit combatCard = CombatManager.instance.TryGetCombat(nCombatID);
		if (combatCard != null)
		{
			inCurFighting = true;
			Begin(combatCard.endTimeSec);
		}
		
		int nLevel = newGroup.nCurLevel;
		bool onOrStop = (newGroup.nAutoCombat==1);
		if (inCurFighting == true) { nLevel = Mathf.Max(1,nLevel - 1); }
		
		if (PrettyBaizFi != null)
		{
			PrettyBaizFi.SetBaizhanCurLevel(nLevel);
			if (inCurFighting || onOrStop)
			{
				PrettyBaizFi.SetBaizOnDoor(nLevel);
			}
		}
		
		if (RetryTimesCol != null)
		{
			int nRetryTimes = 0;
			Hashtable dmMap = CommonMB.InstanceDescMBInfo_Map;
			uint ExcelID = (uint)CombatTypeEnum.COMBAT_INSTANCE_BAIZHANBUDAI;
			if (true == dmMap.ContainsKey(ExcelID))
			{
				InstanceDescMBInfo card = (InstanceDescMBInfo)dmMap[ExcelID];
				nRetryTimes = card.nRetryTimes;
			}
			
			RetryTimesCol.text = string.Format(mPrettyRetry, nRetryTimes - newGroup.nRetryTimes, nRetryTimes+1);
		}
		
		// 解锁吧 inCurFighting
		SetBaizAutoCombat(onOrStop);
		inCurFighting = false;
	}
	
	public void Begin(int endTimeSec)
	{
		StopCoroutine("BaizCooldown");
		StartCoroutine("BaizCooldown", endTimeSec);
	}
	
	// 战斗Cooldown 
	IEnumerator BaizCooldown(int endTimeSec)
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
		
		this.inCurFighting = false;
		BaizhanInstance.instance.idCombat = 0;
	}
	
	public void SetCGuardOnDoor(int gid)
	{
		if (PrettyBaizFi != null) {
			PrettyBaizFi.SetBaizOnDoor(gid);
		}
	}
	
	public void DisableAutoTiao()
	{
		if (AutoButtonCol != null) {
			AutoButtonCol.isEnabled = false;
		}
	}
}
