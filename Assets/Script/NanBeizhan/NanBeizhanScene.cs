using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using STCCMD;
using CTSCMD;
using CMNCMD;

public class NanBeizhanScene : MonoBehaviour {
	
	UIAnchor mDepth = null;
	
	Vector3[] mArrange = null;
	
	public GameObject ButtonDestroy = null;
	public GameObject ButtonAutoCombat = null;
	public GameObject ButtonKick = null;

	public GameObject BattleFi = null;
	public GameObject template = null;
	public Transform TopBar = null;
	
	public UIButton AutoButton = null;
	public UIButton AutoButtonKick = null;
	public UILabel AutoCombatCol = null;
	public UILabel PrettySceneCol = null;
	public UILabel RestTimeCol = null;
	public UILabel RetryTimesCol = null;
	public BaizhanPoster PosterCol = null;
	string mPrettyScene;
	string mPrettyRest;
	string mPrettyRetry;
	
	NanBeizhanGuard[] mCGuards = null;
	
	int mCGuardCar = 1;
	int mCGuardLevel  = 1;
	
	bool inCurFighting = false;
	
	void Awake() {
		
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
			TopBar.localPosition = new Vector3(0f,OffsetY,0f);
		}
		
		if (PrettySceneCol != null) {
			mPrettyScene = PrettySceneCol.text;
		}
		if (RestTimeCol != null) {
			mPrettyRest = RestTimeCol.text;
			RestTimeCol.text = string.Format(mPrettyRest,0);
		}
		if (PosterCol != null) {
			PosterCol.gameObject.SetActiveRecursively(false);
		}
		if (RetryTimesCol != null) {
			mPrettyRetry = RetryTimesCol.text;
			RetryTimesCol.text = string.Format(mPrettyRetry,0,1);
		}
		
		onCreateOrReposition();
		
		// 战斗细节   
		onEnableDelegate();
	}
	
	void OnDestroy() { onExceptDelegate(); }
	
	void onEnableDelegate()
	{
		CombatManager.processStartCombatCon1 += OnProcessStartCombatCon1;
		CombatManager.processCombatRstDelegate += OnProcessBaizCombatRstDelegate;
		CombatManager.processStopCombatDelegate += OnProcessStopCombatDelegate;
		
		BaizInstanceManager.processQuitInstanceDelegate += OnProcessQuitInstanceDelegate;
		BaizInstanceManager.processInstanceDataCon1 += OnProcessInstanceDataCon1;
		BaizInstanceManager.processDestroyInstanceDelegate = null;
		BaizInstanceManager.processDestroyInstanceDelegate = OnProcessDestroyInstanceDelegate;
	}
	
	void onExceptDelegate()
	{
		//print("- onExceptDelegate");
		
		CombatManager.processStartCombatCon1 -= OnProcessStartCombatCon1;
		CombatManager.processCombatRstDelegate -= OnProcessBaizCombatRstDelegate;
		CombatManager.processStopCombatDelegate -= OnProcessStopCombatDelegate;
		CombatManager.processCombatDataDelegate -= OnProcessBaizCombatDataDelegate;
		CombatManager.processAutoCombatDelegate -= OnProcessStopNanBeizhanAutoCombat;
		
		BaizInstanceManager.processListInstanceStatusDelegate -= OnProcessListInstanceStatusDataDelegate;
		BaizInstanceManager.processQuitInstanceDelegate -= OnProcessQuitInstanceDelegate;
		BaizInstanceManager.processInstanceDataDelegate -= OnProcessInstanceDataDelegate;
		BaizInstanceManager.processInstanceDataCon1 -= OnProcessInstanceDataCon1;
		BaizInstanceManager.processDestroyInstanceDelegate -= OnProcessDestroyInstanceDelegate;
	}
	
	// Use this for initialization
	void Start () {
		SoundManager.StopMainSceneSound();
		SoundManager.PlayBattleSound();
		
		if (ButtonKick != null) {
			UIEventListener.Get(ButtonKick).onClick = clickButtonKickDelegate;
		}
	}
	
	// 场景关卡分布图 ...
	void onCreateOrReposition()
	{
		if (template != null)
		{
			mArrange 		= new Vector3[10];
			mArrange[0] 	= new Vector3(-112f,	-82f,	0f);
			mArrange[1] 	= new Vector3(20f,	-91f,	0f);
			mArrange[2] 	= new Vector3(94f,	-42f,	0f);
			mArrange[3] 	= new Vector3(-57f,-12f,	0f);
			mArrange[4] 	= new Vector3(-180f, 12f,	0f);		
			mArrange[5] 	= new Vector3(-149f,71f,	0f);
			mArrange[6] 	= new Vector3(-51f,	100f,	0f);
			mArrange[7] 	= new Vector3(32f, 93f,	0f);
			mArrange[8] 	= new Vector3(118f,	98f,	0f);
			mArrange[9] 	= new Vector3(158f,  27f, 	0f);

			mCGuards = new NanBeizhanGuard[10];
			for(int i=0, imax=10; i<imax; ++ i)
			{
				GameObject go = NGUITools.AddChild(BattleFi, template);
				go.transform.localPosition = mArrange[i];
				
				NanBeizhanGuard t = go.GetComponent<NanBeizhanGuard>();
				t.gid = (i+1);
				t.SetCScene(this);
				if (i==9) { t.BossSet1(); }
				
				mCGuards[i] = t;
			}
			
		}
		
		if (PrettySceneCol != null)
		{
			PrettySceneCol.text = string.Format(mPrettyScene, mCGuardCar, mCGuardCar+9);
		}
	}
	
	// 界面 z设置 ...
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
	
	void OnNanBeizhanSceneClose()
	{		
		// 切换声音 ...
		SoundManager.StopBattleSound();
		SoundManager.PlayMainSceneSound();
		
		// iphone5 挪开 ...
		KeepOut.instance.HideKeepOut();
		
		// 删除自己 ...
		Destroy(gameObject);
	}
	
	// 结束副本， 开启副本后一直处于非空闲锁定状态 (附: 将武将置空闲状态 )
	void OnProcessDestroyInstanceDelegate()
	{
		LoadingManager.instance.HideLoading();
		NanBeizhanInstance.instance.idInstance = 0;
		
		int iCol = NanBeizhanInstance.instance.idCol;
		PickingGeneral[] args = NanBeizhanInstance.instance.GetGenerals();
			
		Hashtable jlMap = JiangLingManager.MyHeroMap;
		ulong n1HeroID = args[iCol].nHeroID;
		if (true == jlMap.ContainsKey(n1HeroID))
		{
			HireHero h1Hero = (HireHero)jlMap[n1HeroID];
			h1Hero.nStatus14 = (int) CMNCMD.HeroState.NORMAL; // 标记为出征状态   Status
			jlMap[n1HeroID] = h1Hero;
		}
		
		// 选将界面的删除 ....
		NanBeizhanGuardWin win1 = GameObject.FindObjectOfType(typeof(NanBeizhanGuardWin)) as NanBeizhanGuardWin;
		if (win1 != null) { Destroy (win1.gameObject); }
		
		// 切换声音 ...
		SoundManager.StopBattleSound();
		SoundManager.PlayMainSceneSound();
		
		// iphone5 挪开 ...
		KeepOut.instance.HideKeepOut();
		
		// 删除副本图片...
		Destroy(gameObject);
		Resources.UnloadUnusedAssets();
	}
	
	// 非队长退出副本 ...
	void OnProcessQuitInstanceDelegate()
	{
		LoadingManager.instance.HideLoading();
		
		int iCol = NanBeizhanInstance.instance.idCol;
		PickingGeneral[] args = NanBeizhanInstance.instance.GetGenerals();
			
		Hashtable jlMap = JiangLingManager.MyHeroMap;
		ulong n1HeroID = args[iCol].nHeroID;
		if (true == jlMap.ContainsKey(n1HeroID))
		{
			HireHero h1Hero = (HireHero)jlMap[n1HeroID];
			h1Hero.nStatus14 = (int) CMNCMD.HeroState.NORMAL; // 标记为出征状态   Status
			jlMap[n1HeroID] = h1Hero;
		}
		
		// 切换声音 ...
		SoundManager.StopBattleSound();
		SoundManager.PlayMainSceneSound();
		
		// iphone5 挪开 ...
		KeepOut.instance.HideKeepOut();
		
		// ....
		NanBeizhanInstance.instance.idInstance = 0;
		Destroy(gameObject);
		
		// 删除副本图片...
		Resources.UnloadUnusedAssets();
	}
	
	void OnButtonNanBeizhanDestroyInstance(GameObject go)
	{
		LoadingManager.instance.ShowLoading();
		ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
		BaizInstanceManager.RequestDestroyInstance(101, nInstanceID);
		PopTipDialog.instance.Dissband();
	}
	
	// 队长有权力结束副本 ...
	void OnNanBeizhanInstanceDestroy(GameObject go)
	{
		ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
		if (nInstanceID == 0) return;
		
		int Tipset = BaizVariableScript.INSTANCE_DESTROY_ASKING_OK;
		PopTipDialog.instance.VoidSetText1(true,true,Tipset);
		PopTipDialog.instance.VoidButton1(OnButtonNanBeizhanDestroyInstance);
	}
	
	void OnButtonNanBeizhanQuitInstance(GameObject go)
	{
		ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
		if (nInstanceID == 0) return;
		
		LoadingManager.instance.ShowLoading();
		BaizInstanceManager.RequestQuitInstance(101, nInstanceID);
		PopTipDialog.instance.Dissband();
	}
	// 队伍成员退出副本 ...
	void OnNanBeizhanInstanceQuit(GameObject go)
	{
		ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
		if (nInstanceID == 0) return;
		
		int Tipset = BaizVariableScript.INSTANCE_DESTROY_ASKING_OK;
		PopTipDialog.instance.VoidSetText1(true,true,Tipset);
		PopTipDialog.instance.VoidButton1(OnButtonNanBeizhanQuitInstance);
	}
	
	public void PopupNanBeizhanGuardWin(int id)
	{
		GameObject go = U3dCmn.GetObjFromPrefab("NanBeizhanGuardWin");
		if (go == null ) return;

		NanBeizhanGuardWin win1 = go.GetComponent<NanBeizhanGuardWin>();
		if (win1 != null)
		{
			bool isLeaderOrNo = NanBeizhanInstance.instance.IsInstanceCreator();
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			win1.Depth(depth);
			win1.idGuard = id;
			win1.ApplyLeaderOrNo(isLeaderOrNo);
			win1.GetCGuardProf();
			win1.ApplyRoadGeneral();
			win1.SetCScene(this);
		}
	}
	
	public void ApplyLeaderOrNo(bool isLeaderOrNo)
	{
		if (ButtonDestroy != null) 
		{
			if (isLeaderOrNo == true)
			{
				UIEventListener.Get(ButtonDestroy).onClick = OnNanBeizhanInstanceDestroy;
			}
			else 
			{
				UIEventListener.Get(ButtonDestroy).onClick = OnNanBeizhanInstanceQuit;
			}
		}
		
		if (ButtonAutoCombat != null) {
			NGUITools.SetActive(ButtonAutoCombat, isLeaderOrNo);
		}
		
		if (ButtonKick != null) {
			NGUITools.SetActive(ButtonKick.gameObject, isLeaderOrNo);
		}
	}
	// 初始化当前副本的数据 ...
	public void ApplyInstanceGroup(BaizInstanceGroup newGroup)
	{
		inCurFighting = false;
		
		ulong nCombatID = NanBeizhanInstance.instance.idCombat;
		CombatDescUnit combatCard = CombatManager.instance.TryGetCombat(nCombatID);
		if (combatCard != null)
		{
			inCurFighting = true;
			Begin(combatCard.endTimeSec);
		}
		
		int nLevel = newGroup.nCurLevel;
		bool onOrStop = (newGroup.nAutoCombat==1);
		if (inCurFighting == true) 
		{
			nLevel = Mathf.Max(1,nLevel-1);
		}

		this.SetCGuardLevel(nLevel);
		
		if (inCurFighting == true || onOrStop == true)
		{
			this.SetCGuardOnDoor(nLevel);
		}
		
		if (RetryTimesCol != null)
		{
			int nRetryTimes = 0;
			Hashtable dmMap = CommonMB.InstanceDescMBInfo_Map;
			uint ExcelID = (uint)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN;
			if (true == dmMap.ContainsKey(ExcelID))
			{
				InstanceDescMBInfo card = (InstanceDescMBInfo)dmMap[ExcelID];
				nRetryTimes = card.nRetryTimes;
			}
			
			RetryTimesCol.text = string.Format(mPrettyRetry, nRetryTimes - newGroup.nRetryTimes, nRetryTimes+1);
		}
		
		// 解锁吧 inCurFighting
		this.SetCGuardAutoCombat(onOrStop);
		inCurFighting = false;
	}
	// 自动战斗 ...
	public void SetCGuardAutoCombat(bool on)
	{
		NanBeizhanInstance.instance.isAutoCombat = on;
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
	// 当前关卡号 ...
	void SetCGuardCar(int newCar)
	{
		mCGuardCar = newCar;
				
		if (PrettySceneCol != null)
		{
			PrettySceneCol.text = string.Format(mPrettyScene, mCGuardCar, mCGuardCar+9);
		}

		int i=0;
		foreach (NanBeizhanGuard go in mCGuards)
		{
			go.gid = mCGuardCar + i;
			
			if (go.gid > mCGuardLevel)
			{
				go.PrettyStatus = NanBeizhanGuard.GuardStatus.no;
				go.Reset1();
				if (go.ImageGuardCol != null)
				{
					go.ImageGuardCol.color = Color.cyan;
				}
			}
			else if (go.gid == mCGuardLevel)
			{
				go.PrettyStatus = NanBeizhanGuard.GuardStatus.Door;
				go.Door1();
				if (go.ImageGuardCol != null)
				{
					go.ImageGuardCol.color = Color.green;
				}
			}
			else 
			{
				go.PrettyStatus = NanBeizhanGuard.GuardStatus.Passed;
				go.PassedSet1();
				if (go.ImageGuardCol != null)
				{
					go.ImageGuardCol.color = Color.gray;
				}
			}
			
			i ++;
		}
	}
	// 定公到当前的关卡页面 ...
	public void SetCGuardLevel(int nLevel)
	{
		mCGuardLevel = nLevel;		
		if (mCGuardLevel > (mCGuardCar+9))
		{
			int paged = (nLevel-1)/10;
			int newCar = Mathf.Min(11,paged*10 + 1);
			SetCGuardCar(newCar);
			return;
		}
				
		int i=0;
		int toCar = 10;
		int curCheck = mCGuardLevel - mCGuardCar;
		
		//未知关卡 
		int nDead = Mathf.Min(curCheck,toCar);
		for (i=0; i<nDead; ++ i)
		{
			NanBeizhanGuard go = mCGuards[i];
			go.PrettyStatus = NanBeizhanGuard.GuardStatus.Passed;
			go.PassedSet1();
			if (go.ImageGuardCol != null)
			{
				go.ImageGuardCol.color = Color.gray;
			}
		}
		
		NanBeizhanGuard goCur = mCGuards[i++];		
		goCur.PrettyStatus = NanBeizhanGuard.GuardStatus.Door;
		goCur.Door1();
		if (goCur.ImageGuardCol != null)
		{
			goCur.ImageGuardCol.color = Color.green;
		}
		
		//未知关卡 
		for (; i<toCar; ++ i)
		{
			NanBeizhanGuard go = mCGuards[i];
			go.PrettyStatus = NanBeizhanGuard.GuardStatus.no;
			if (go.ImageGuardCol != null)
			{
				go.ImageGuardCol.color = Color.cyan;
			}
		}
		
		// Fini
	}
	// 设置为战斗状态 ...
	public void SetCGuardOnDoor(int gid)
	{
		int to = gid - mCGuardCar;
		if (to<0 || to>9) return;

		NanBeizhanGuard goCur = mCGuards[to];
		goCur.PrettyStatus = NanBeizhanGuard.GuardStatus.OnDoor;
		goCur.Warc1();
	}
	
	public bool isOnDoor()
	{
		int to = mCGuardLevel - mCGuardCar;
		if (to<0 || to>9) return false;
		NanBeizhanGuard goCur = mCGuards[to];
		return (goCur.PrettyStatus == NanBeizhanGuard.GuardStatus.OnDoor);
	}
	
	void StopAtPoster()
	{
		int to = mCGuardLevel - mCGuardCar;
		if (to<0 || to>9) return;
		NanBeizhanGuard goCur = mCGuards[to];
		goCur.PrettyStatus = NanBeizhanGuard.GuardStatus.Door;
		goCur.Door1();
	}
	// 战斗结果返回, 是否保存失败的战斗数据? ...
	void OnProcessBaizCombatRstDelegate(ulong nCombatID, int nCombatType)
	{
		if (nCombatType != (int)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN) return;
		NanBeizhanInstance.instance.idCombat = nCombatID;
		NanBeizhanInstance.instance.idFighting = null;

		// Rest time calc
		SortedList<ulong,CombatFighting> combatMap = CombatManager.instance.GetCombatFightingMap();
		CombatFighting comFighting = null;
		if (true == combatMap.TryGetValue(nCombatID, out comFighting))
		{
			int endTimeSec = comFighting.nBackRestTime + DataConvert.DateTimeToInt(DateTime.Now);
			Begin(endTimeSec);

			// 打开观战画面 combatID
			bool onOrStop = NanBeizhanInstance.instance.isAutoCombat;
			if (false == onOrStop)
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
				
				RestTimeCol.text = string.Format(mPrettyRest,nSecs);
				yield return new WaitForSeconds(1);
			}
		}

		inCurFighting = false;
		NanBeizhanInstance.instance.idCombat = 0;
	}
	// 通关后或失败后重新拉起多人副本启动界面 ...
	void OnProcessListInstanceStatusDataDelegate()
	{
		LoadingManager.instance.HideLoading();
		GameObject go = U3dCmn.GetObjFromPrefab("NanBeizhanLaunchWin");
		if (go == null ) return;
		
		NanBeizhanLaunchWin win1 = go.GetComponent<NanBeizhanLaunchWin>();
		if (win1 != null)
		{
			float depth = -BaizVariableScript.DEPTH_OFFSET;
			win1.Depth(depth);
			win1.ApplyCDiTuLevel(1);
			win1.ApplyBaizInstanceStatus();
			
			// 切换声音 ...				
			if (SoundManager.palying_music == CMNCMD.BG_MUSIC.BATTLE_MUSIC)
			{
				SoundManager.StopBattleSound();
				SoundManager.PlayMainSceneSound();
			}
			
			// iphone5 挪开 ...
			KeepOut.instance.HideKeepOut();
			
			// 关闭自己吧 .... 
			NGUITools.SetActive(gameObject, false);
			Destroy(gameObject);
		}
	}
	// 更新服务器推送回来的副本数据(武将状态,重试次数) ...
	void OnProcessInstanceDataCon1(BaizInstanceGroup newGroup)
	{
		if (newGroup.nInstanceID != NanBeizhanInstance.instance.idInstance) return;
		
		// 更新选将界面 ...
		NanBeizhanGuardWin win1 = GameObject.FindObjectOfType(typeof(NanBeizhanGuardWin)) as NanBeizhanGuardWin;
		NanBeizhanAutoCombatWin win2 = GameObject.FindObjectOfType(typeof(NanBeizhanAutoCombatWin)) as NanBeizhanAutoCombatWin;
		if (win1 == null && win2 == null) 
		{
			NanBeizhanInstance.instance.ApplyNanBeizhanInstanceCon1(newGroup);
		}
		else if (win1 != null)
		{
			win1.ApplyNanBeizhanInstanceData1(newGroup);
		}
		else if (win2 != null)
		{
			win2.SetNanBeizhanInstanceData1(newGroup);
		}
		
		// 重试桨数 ...
		if (RetryTimesCol != null)
		{
			int nRetryTimes = 0;
			Hashtable dmMap = CommonMB.InstanceDescMBInfo_Map;
			uint ExcelID = (uint)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN;
			if (true == dmMap.ContainsKey(ExcelID))
			{
				InstanceDescMBInfo card = (InstanceDescMBInfo)dmMap[ExcelID];
				nRetryTimes = card.nRetryTimes;
			}
			
			RetryTimesCol.text = string.Format(mPrettyRetry, nRetryTimes - newGroup.nRetryTimes, nRetryTimes+1);
		}
	}
	
	void OnProcessInstanceDataDelegate(BaizInstanceGroup newGroup)
	{
		if (newGroup.nInstanceID != NanBeizhanInstance.instance.idInstance) return;
		
		bool onOrStop = (newGroup.nAutoCombat==1);
		this.SetCGuardAutoCombat(onOrStop);
		
		this.SetCGuardLevel(newGroup.nCurLevel);
		if (true == onOrStop)
		{
			this.SetCGuardOnDoor(newGroup.nCurLevel);
		}
		
		// 武将顺序 ....
		NanBeizhanInstance.instance.ApplyInstanceGroupGeneral(newGroup);
		
		if (RetryTimesCol != null)
		{
			int nRetryTimes = 0;
			Hashtable dmMap = CommonMB.InstanceDescMBInfo_Map;
			uint ExcelID = (uint)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN;
			if (true == dmMap.ContainsKey(ExcelID))
			{
				InstanceDescMBInfo card = (InstanceDescMBInfo)dmMap[ExcelID];
				nRetryTimes = card.nRetryTimes;
			}
			
			RetryTimesCol.text = string.Format(mPrettyRetry, nRetryTimes - newGroup.nRetryTimes, nRetryTimes+1);
		}
	}
		
	void OnWaitingListInstanceStatus(GameObject go)
	{
		LoadingManager.instance.ShowLoading();
		BaizInstanceManager.processListInstanceStatusDelegate += OnProcessListInstanceStatusDataDelegate;
		BaizInstanceManager.RequestListInstanceStatusData();
		PopTipDialog.instance.Dissband();
	}
	
	void DoGeneralFreeState()
	{
		int iCol = NanBeizhanInstance.instance.idCol;
		PickingGeneral[] args = NanBeizhanInstance.instance.GetGenerals();
		
		Hashtable jlMap = JiangLingManager.MyHeroMap;
		ulong n1HeroID = args[iCol].nHeroID;
		if (true == jlMap.ContainsKey(n1HeroID))
		{
			HireHero h1Hero = (HireHero)jlMap[n1HeroID];
			h1Hero.nStatus14 = (int) CMNCMD.HeroState.NORMAL; // 标记为出征状态   Status
			jlMap[n1HeroID] = h1Hero;
		}
	}
	// 结束战斗返回处理 ... (胜利/失败)动画播放, 通关或失败 ...
	void OnProcessStopCombatDelegate(int nCombatType, int nRst, int eVictoryOrNo)
	{
		if (nCombatType != (int)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN) return;
		
		if (nRst == (int) STC_GAMECMD_OPERATE_STOP_COMBAT_T.enum_rst.RST_INSTANCE_FINISH)
		{
			//print ("jjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjj");
			//if (CommonData.player_online_info.ZhengZhan < 100) {
				//CommonData.player_online_info.ZhengZhan = 100;
			//}
			
			this.StopAtPoster();
			this.DoGeneralFreeState();
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET * 2.0f;
			int Tipset = BaizVariableScript.COMBAT_INSTANCE_FINISH;
			PopTipDialog.instance.VoidSetText1(true,false,Tipset);
			PopTipDialog.instance.VoidButton1(OnWaitingListInstanceStatus);
			PopTipDialog.instance.Popup1(depth);
		}
		else if (nRst == (int) STC_GAMECMD_OPERATE_STOP_COMBAT_T.enum_rst.RST_INSTANCE_FAILURE)
		{
			this.StopAtPoster();
			this.DoGeneralFreeState();
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET * 2.0f;
			int Tipset = BaizVariableScript.COMBAT_INSTANCE_OVER;
			PopTipDialog.instance.VoidSetText1(true,false,Tipset);
			PopTipDialog.instance.VoidButton1(OnWaitingListInstanceStatus);
			PopTipDialog.instance.Popup1(depth);
		}
		else 
		{
			// 自动战斗可以使用 ....
			if (AutoButton != null) {
				AutoButton.isEnabled = true;
			}
			
			// 踢出设置 ...
			if (AutoButtonKick != null) {
				bool onOrStop = NanBeizhanInstance.instance.isAutoCombat;
				AutoButtonKick.isEnabled = (onOrStop == false);
			}
			
			// 胜利或失败 Victory
			if (PosterCol != null)
			{		
				if (eVictoryOrNo ==1) 
				{
					NanBeizhanInstance.instance.idFighting = null;
					
					// 播放成功动画 ...
					PosterCol.Play("anim-victory");
				}
				else if (eVictoryOrNo == 2)
				{
					CombatFighting comFighting = null;
					ulong combatID = NanBeizhanInstance.instance.idCombat;
					SortedList<ulong,CombatFighting> comFightMap = CombatManager.instance.GetCombatFightingMap();
					comFightMap.TryGetValue(combatID, out comFighting);
					NanBeizhanInstance.instance.idFighting = comFighting;
					
					// 播放失败动画 ...
					PosterCol.Play("anim-failure");
				}			
			}
			
			// 获取副本的信息 RequestGetInstanceData		
			ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
			BaizInstanceManager.processInstanceDataDelegate += OnProcessInstanceDataDelegate;
			BaizInstanceManager.RequestGetInstanceData(101, nInstanceID);
		}
	}
	
	void OnProcessBaizCombatDataDelegate(ulong nCombatID)
	{
		BattlefieldWin win1 = GameObject.FindObjectOfType(typeof(BattlefieldWin)) as BattlefieldWin;
		if (win1 != null) { Destroy(win1.gameObject); }
		BaizhanDeathWin win2 = GameObject.FindObjectOfType(typeof(BaizhanDeathWin)) as BaizhanDeathWin;
		if (win2 != null) { Destroy(win2.gameObject); }
		NanBeizhanGuardWin win3 = GameObject.FindObjectOfType(typeof(NanBeizhanGuardWin)) as NanBeizhanGuardWin;
		if (win3 != null) { Destroy(win3.gameObject); }
		
		GameObject go = U3dCmn.GetObjFromPrefab("BaizBattlefieldWin");
		if ( go != null)
		{
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET * 3.0f;
			go.SendMessage("Depth", depth);
		}
	}
	// 服务器开启战斗后的推送 ...
	void OnProcessStartCombatCon1(ulong nCombatID, int nCombatType, int nAutoCombat)
	{
		if (nCombatType != (int)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN) return;
		NanBeizhanInstance.instance.isAutoCombat = (nAutoCombat==1);
		
		SetCGuardOnDoor(mCGuardLevel);
		DisableAutoTiaoZhan();
		
		// 监控队长的战斗是否开启   con1
		CombatFighting comFighting = null;
		SortedList<ulong,CombatFighting> combatMap = CombatManager.instance.GetCombatFightingMap();
		if (true == combatMap.TryGetValue(nCombatID, out comFighting))
		{
			int iCol = NanBeizhanInstance.instance.idCol;
			PickingGeneral[] args = NanBeizhanInstance.instance.GetGenerals();
			
			Hashtable jlMap = JiangLingManager.MyHeroMap;
			ulong n1HeroID = args[iCol].nHeroID;
			if (true == jlMap.ContainsKey(n1HeroID))
			{
				HireHero h1Hero = (HireHero)jlMap[n1HeroID];
				h1Hero.nStatus14 = (int) CMNCMD.HeroState.COMBAT_INSTANCE_NANZHENGBEIZHAN; // 标记为出征状态   Status
				jlMap[n1HeroID] = h1Hero;
			}
			
			combatMap[nCombatID] = comFighting;
		}
	}
	// 当前战斗的观战 ...
	void OnNanBeizhanGuanZhan()
	{
		CombatFighting comFighting = NanBeizhanInstance.instance.idFighting;
		if (comFighting == null)
		{
			ulong combatID = NanBeizhanInstance.instance.idCombat;
			CombatManager.processCombatDataDelegate = OnProcessBaizCombatDataDelegate;
			CombatManager.instance.ApplyBaizFighting(combatID);
		}
		else 
		{
			Globe.comFighting = comFighting;
			OnProcessBaizCombatDataDelegate(comFighting.nCombatID);
		}
	}
	
	void OnProcessStopNanBeizhanAutoCombat()
	{
		bool isAuto = NanBeizhanInstance.instance.isAutoCombat;
		SetCGuardAutoCombat((isAuto==false));
		
		if (true == isOnDoor())
		{
			DisableAutoTiaoZhan();
		}
	}
	// 开启自动挑战 ...
	void OnNanBeizhanAutoCombat()
	{
		bool isAuto = NanBeizhanInstance.instance.isAutoCombat;
		if (isAuto == false)
		{
			GameObject go = U3dCmn.GetObjFromPrefab("NanBeizAutoCombatWin");
			if (go == null) return;
			
			NanBeizhanAutoCombatWin t = go.GetComponent<NanBeizhanAutoCombatWin>();
			if (t != null)
			{
				float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;	
				t.Depth(depth);
				t.idGuard = mCGuardLevel;
				t.ApplyRoadGeneral();
				t.SetCScene(this);
			}
		}
		else 
		{
			ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
			CombatManager.processAutoCombatDelegate += OnProcessStopNanBeizhanAutoCombat;
			CombatManager.RequestStopAutoCombat(nInstanceID, 0);
		}
	}
	
	public void DisableAutoTiaoZhan()
	{
		if (AutoButton != null) {
			AutoButton.isEnabled = false;
		}
		
		if (AutoButtonKick != null) {
			AutoButtonKick.isEnabled = false;
		}
	}
	
	public void DisableKicking()
	{
		if (AutoButtonKick != null) {
			AutoButtonKick.isEnabled = false;
		}
	}
	
	public void clickButtonKickDelegate(GameObject tween)
	{
		GameObject go = U3dCmn.GetObjFromPrefab("NanBeizhanKickWin");
		if (go == null) return;
		
		NanBeizhanKickWin win1 = go.GetComponent<NanBeizhanKickWin>();
		if (win1 != null)
		{
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			win1.Depth(depth);
			win1.ApplyRoadGeneral();
			win1.ApplyEnemyProf();
		}
	}
}
