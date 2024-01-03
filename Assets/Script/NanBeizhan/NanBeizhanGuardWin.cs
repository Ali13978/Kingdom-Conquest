using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class NanBeizhanGuardWin : MonoBehaviour {
	
	public UILabel PrettyEnemyPowerCol = null;		// 敌方战力 ...
	public UILabel PrettyHeroPowerCol = null;		// 已方战力 ...
	public UILabel PrettyEnemyArmyCol = null;		// 军队阵形 ...
	public UILabel PrettyTitle = null;
	public GameObject Troop;
	
	public GameObject ButtonManual = null;
	public GameObject ButtonTiaoZhan = null;
	public GameObject ButtonPicking = null; 
	public GameObject ButtonArmy  = null;
	public GameObject ButtonKick = null;			// 踢出队友 , (只有队长有权利) ....
	
	public int spacing = 80;
	public GameObject RoadGeneralCol = null;
	public GameObject template = null;
	
	PickingGeneralItem[] RoadCol = new PickingGeneralItem[5];
	PickingGeneral[] camps = new PickingGeneral[5];
	UIAnchor mDepth = null;
	
	string mPrettyEnemyPower;
	string mPrettyHeroPower;
	string mPrettyEnemyArmy;
	string mPretty;
	
	NanBeizhanScene mCScene = null;
	
	int m_id = -1;
	
	void Awake() {
		
		if (PrettyTitle != null)
		{
			mPretty = PrettyTitle.text;
		}
		if (PrettyEnemyPowerCol != null)
		{
			mPrettyEnemyPower = PrettyEnemyPowerCol.text;
			PrettyEnemyPowerCol.enabled = false;
		}
		if (PrettyHeroPowerCol != null)
		{
			mPrettyHeroPower = PrettyHeroPowerCol.text;
			PrettyHeroPowerCol.enabled = false;
		}
		if (PrettyEnemyArmyCol != null)
		{
			mPrettyEnemyArmy = PrettyEnemyArmyCol.text;
			PrettyEnemyArmyCol.enabled = false;
			Troop.SetActiveRecursively(false);
		}
		if (ButtonArmy != null)
		{
			UIEventListener.Get(ButtonArmy).onClick = OnPopArmyExplain;
		}
		
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
		
		onCreateOrReposition();
	}
	
	void OnDestroy() { onExceptDelegate(); }
	void onExceptDelegate()
	{
		//print ("- onExceptDelegate");
		
		CombatManager.processCombatProfDelegate -= OnProcessCGuardProfDelegate;
		CombatManager.processStartCombatDelegate -= OnProcessStartCombatDelegate;
		
		BaizGeneralRoadWin.applyBaizCampDataDelegate  = null;
		BaizInstanceManager.processSupplyInstanceDelegate -= OnProcessManualSupplyInstanceDelegate;
		BaizInstanceManager.processCGuardLootDelegate -= OnProcessCGuardLootDelegate;
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	
	void onCreateOrReposition()
	{
		if (template != null)
		{
			float[] fCol = {-2f,-1f,0,1f,2f};
			RoadCol = new PickingGeneralItem[5];
			for(int i=0, imax=5; i<imax; ++ i)
			{
				GameObject go = NGUITools.AddChild(RoadGeneralCol, template);
				go.transform.localPosition = new Vector3(fCol[i] * spacing, 0f, 0f);
				PickingGeneralItem t = go.GetComponent<PickingGeneralItem>();
				RoadCol[i] = t;
			}
		}
	}
	
	public int idGuard 
	{
		get { return m_id; }
		set { 
			m_id = value; 
			
			if (PrettyTitle != null)
			{
				PrettyTitle.text = string.Format(mPretty, m_id);
				PrettyTitle.enabled = (m_id>0);
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
	
	float GetDepth()
	{
		if (mDepth == null) return 0f;
		return mDepth.depthOffset;
	}
	
	public void SetCScene(NanBeizhanScene _obj)
	{
		this.mCScene = _obj;
	}
	
	void OnNanBeizhanGuardClose()
	{
		NGUITools.SetActive(gameObject, false);
		Destroy(gameObject);
	}
	
	public void ApplyNanBeizhanInstanceData1(BaizInstanceGroup newGroup)
	{
		PickingGeneral[] args = NanBeizhanInstance.instance.GetGenerals();
		args[0] = null;
		args[1] = null;
		args[2] = null;
		args[3] = null;
		args[4] = null;
		
		ulong myAccountID = CommonData.player_online_info.AccountID;
		PickingGeneral myGeneral = null;
		int i=0;
		for(i=0; i<newGroup.nNum; ++ i)
		{
			BaizInstanceData item = newGroup.m_ma[i];
			if (newGroup.nNumHero>0)
			{
				PickingGeneral gen = newGroup.m_ga[i];
				if (gen != null)
				{
					args[i] = gen;
					
					if (myAccountID == item.nAccountID)
					{
						NanBeizhanInstance.instance.idCol = i;
						myGeneral = gen;
					}
				}
			}
		}
		
		// 原有阵形重新显示 ....
		ResetRoadGeneralCon1();
		
		if (myGeneral != null)
		{
			Hashtable jlMap = JiangLingManager.MyHeroMap;
			Hashtable jlsosMap = SoldierManager.SoldierMap;
			
			ulong h1HeroID = myGeneral.nHeroID;
			if (true == jlMap.ContainsKey(h1HeroID))
			{
				HireHero h1Hero = (HireHero) jlMap[h1HeroID];
				int delta = h1Hero.nArmyNum11 - myGeneral.nArmyNum;
				// print ("+ InstanceData Hero SoliderUnit:" + delta); // 测试自动补给数量 ...
				h1Hero.nArmyNum11 = myGeneral.nArmyNum;
				h1Hero.nProf20	= myGeneral.nProf;
				h1Hero.nHealthState21 = myGeneral.nHealthStatus;
				jlMap[h1HeroID] = h1Hero;
				
				string k = string.Format("{0}_{1}",h1Hero.nArmyType9,h1Hero.nArmyLevel10);
				if (true == jlsosMap.ContainsKey(k))
				{
					SoldierUnit si = (SoldierUnit) jlsosMap[k];
					si.nNum3 += delta;
					jlsosMap[k] = si;
				}
			}
		}
	}
	
	void OnProcessNanBeizhanInstanceDataDelegate(BaizInstanceGroup newGroup)
	{
		LoadingManager.instance.HideLoading();
		ApplyNanBeizhanInstanceData1(newGroup);
	}
	
	void OnProcessCGuardProfDelegate(CombatProfData data)
	{
		if (PrettyEnemyPowerCol != null)
		{
			PrettyEnemyPowerCol.text = string.Format(mPrettyEnemyPower,data.nProf);
			PrettyEnemyPowerCol.enabled = true;
		}
		
		uint[] idArray = new uint[5];
		idArray[0] = (uint)data.n1ArmyType;
		idArray[1] = (uint)data.n2ArmyType;
		idArray[2] = (uint)data.n3ArmyType;
		idArray[3] = (uint)data.n4ArmyType;
		idArray[4] = (uint)data.n5ArmyType;
		
		string[] ilArray = new string[5];
		
		for (int i=0; i<5; ++ i)
		{
			uint n1ArmyID = idArray[i];
			SoldierMBInfo info = U3dCmn.GetSoldierInfoFromMb(n1ArmyID,1);
			ilArray[i] = info.Name.Substring(0,1);
			int index = i+1;
			UISprite troop_icon = U3dCmn.GetChildObjByName(Troop,"TroopIcon"+index).GetComponent<UISprite>();
			
			troop_icon.gameObject.SetActiveRecursively(true);
			troop_icon.spriteName = U3dCmn.GetTroopIconFromID((int)info.ExcelID);
		}
		
		if (PrettyEnemyArmyCol != null)
		{
			//PrettyEnemyArmyCol.text = string.Format(mPrettyEnemyArmy,ilArray);
			PrettyEnemyArmyCol.enabled = true;
		}
	}
	
	public void GetCGuardProf()
	{		
		int clsID = NanBeizhanInstance.instance.idClass;
		CombatManager.processCombatProfDelegate += OnProcessCGuardProfDelegate;
		CombatManager.RequestCombatProf((ulong)m_id, clsID, 101);
	}
	
	public void ApplyRoadGeneral()
	{
		if (RoadCol == null) return;
		PickingGeneral[] args = NanBeizhanInstance.instance.GetGenerals();
		
		int totalProf = 0;
		int i=0;
		
		for(i=0; i<args.Length; ++ i)
		{
			PickingGeneral t = args[i];
			camps[i] = t;
			
			if (t != null) {
				totalProf = totalProf + t.nProf;
			}
			
			RoadCol[i].ApplyGeneralItem(t);
		}
		
		if (PrettyHeroPowerCol != null)
		{
			PrettyHeroPowerCol.text = string.Format(mPrettyHeroPower,totalProf);
			PrettyHeroPowerCol.enabled = true;
		}
	}
	
	public void ResetRoadGeneralCon1()
	{
		if (RoadCol == null) return;
		PickingGeneral[] args = NanBeizhanInstance.instance.GetGenerals();
		
		int totalProf = 0;
		int i=0;
		for(i=0; i<args.Length; ++ i)
		{
			PickingGeneral t = args[i];
			if (t != null) 
			{
				totalProf = totalProf + t.nProf;
				for (int j=0; j<camps.Length; ++ j)
				{
					PickingGeneral t1 = camps[j];
					if (t1 == null) continue;
					if (t.nHeroID == t1.nHeroID)
					{
						camps[j] = t;
						RoadCol[j].ApplyGeneralItem(t);
					}
				}
			}
		}
		
		if (PrettyHeroPowerCol != null)
		{
			PrettyHeroPowerCol.text = string.Format(mPrettyHeroPower,totalProf);
			PrettyHeroPowerCol.enabled = true;
		}
	}
	
	public void ApplyLeaderOrNo(bool isLeaderOrNo)
	{
		if (ButtonManual != null) {
			NGUITools.SetActive(ButtonManual, isLeaderOrNo);
		}
		if (ButtonTiaoZhan != null) {
			NGUITools.SetActive(ButtonTiaoZhan, isLeaderOrNo);
		}
		
		if (true == isLeaderOrNo) 
		{
			UIEventListener.Get(ButtonPicking).onClick = OnPopGeneralsPicking;
			
		}
		else 
		{
			Collider col = ButtonPicking.GetComponent<Collider>();
			if (col != null) col.enabled = false;
		}
	}
	
	void OnProcessStartCombatDelegate(ulong nCombatID, int nCombatType)
	{
		LoadingManager.instance.HideLoading();
		if (nCombatType != (int)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN) return;
		
		if (mCScene != null) {
			mCScene.SetCGuardOnDoor(m_id);
			mCScene.DisableAutoTiaoZhan();
		}
		
		CombatFighting comFighting = null;
		SortedList<ulong,CombatFighting> combatMap = CombatManager.instance.GetCombatFightingMap();
		if (true == combatMap.TryGetValue(nCombatID, out comFighting))
		{
			int iCol = NanBeizhanInstance.instance.idCol;
			
			Hashtable jlMap = JiangLingManager.MyHeroMap;
			PickingGeneral[] args = NanBeizhanInstance.instance.GetGenerals();
			ulong n1HeroID = args[iCol].nHeroID;
			if (true == jlMap.ContainsKey(n1HeroID))
			{
				HireHero h1Hero = (HireHero)jlMap[n1HeroID];
				h1Hero.nStatus14 = (int)CMNCMD.HeroState.COMBAT_INSTANCE_NANZHENGBEIZHAN; // 标记为出征状态   Status
				jlMap[n1HeroID] = h1Hero;
			}
			
			combatMap[nCombatID] = comFighting;
		}
		
		// 删除 ...
		Destroy(gameObject);
	}
	
	void OnCGuardStartCombat()
	{
		// 查询下当前将领带兵是否有为0的....
		for(int i=0,imax=camps.Length; i<imax; ++ i)
		{
			if (camps[i] == null) continue;
			if (camps[i].nArmyNum == 0)
			{
				int Tipset = BaizVariableScript.PICKING_GENERAL_NO_ARMY;
				string cc = U3dCmn.GetWarnErrTipFromMB(Tipset);
				string text = string.Format(cc, camps[i].name);
				PopTipDialog.instance.VoidSetText2(true,false,text);
				
				return;
			}
		}
		
		CTS_GAMECMD_OPERATE_START_COMBAT_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_START_COMBAT;
		req.nObjID3 = (ulong) m_id;
		req.nCombatType4 = 101;
		req.nAutoCombat5 = 0;
		req.nAutoSupply6 = 0;
		req.n1Hero7 = 0;
		req.n2Hero8 = 0;
		req.n3Hero9 = 0;
		req.n4Hero10 = 0;
		req.n5Hero11 = 0;
		req.nStopLevel12 = 0;
		
		if (camps[0] != null) { req.n1Hero7  = camps[0].nHeroID; }
		if (camps[1] != null) { req.n2Hero8  = camps[1].nHeroID; }
		if (camps[2] != null) { req.n3Hero9  = camps[2].nHeroID; }
		if (camps[3] != null) { req.n4Hero10 = camps[3].nHeroID; }
		if (camps[4] != null) { req.n5Hero11 = camps[4].nHeroID; }
	
		LoadingManager.instance.ShowLoading();
		CombatManager.processStartCombatDelegate += OnProcessStartCombatDelegate;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_START_COMBAT_T>(req);
	}
	
	void OnProcessManualSupplyInstanceDelegate()
	{
		LoadingManager.instance.HideLoading();
	}
	
	void OnCGuardManualSupply()
	{
		ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
		if (nInstanceID == 0) return;
		
		LoadingManager.instance.ShowLoading();
		BaizInstanceManager.processSupplyInstanceDelegate = OnProcessManualSupplyInstanceDelegate;
		BaizInstanceManager.RequestSupplyInstance(101, nInstanceID);
	}
	
	void OnProcessCGuardLootDelegate(List<BaizInstanceLoot> lootList)
	{	
		LoadingManager.instance.HideLoading();
		GameObject go = U3dCmn.GetObjFromPrefab("BaizhanLootScript");
		if (go == null) return;
		
		BaizhanLootScript t = go.GetComponent<BaizhanLootScript>();
		if (t != null)
		{
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			
			t.Depth(depth);
			t.gid = m_id;
			t.ApplyLootList(lootList);
		}
	}
	
	void OnCGuardLootPopup()
	{
		LoadingManager.instance.ShowLoading();
		ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
		BaizInstanceManager.processCGuardLootDelegate += OnProcessCGuardLootDelegate;
		BaizInstanceManager.RequestInstanceGuardLoot(101, nInstanceID);
	}
	// 踢出武将 ...
	void OnPopKickGeneral(GameObject tween)
	{
	}
	
	/// <summary>
	/// 选择武将 ....
	/// </summary>
	void OnPopGeneralsPicking(GameObject tween)
	{		
		GameObject go = U3dCmn.GetObjFromPrefab("BaizGeneralRoadWin");
		if (go == null) return;
		
		BaizGeneralRoadWin t = go.GetComponent<BaizGeneralRoadWin>();
		if (t != null)
		{
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			t.Depth(depth);
			t.AssignNzbzRoadGeneral();
			
			BaizGeneralRoadWin.applyBaizCampDataDelegate  = OnApplyBaizCampData;	
		}
	}
	
	void OnApplyBaizCampData(List<PickingGeneral> data)
	{
		int imax = Mathf.Min(5,data.Count);
		int totalProf = 0;
		int i=0;
		for (; i<imax; ++ i)
		{
			PickingGeneral t = data[i];
			camps[i] = t;
			
			if (t != null)
			{
				totalProf = totalProf + t.nProf;
			}
			
			PickingGeneralItem item = RoadCol[i];
			item.ApplyGeneralItem(t);
		}
		
		for (; i<5; ++ i)
		{
			PickingGeneralItem item = RoadCol[i];
			camps[i] = null;
			
			item.ApplyGeneralItem(null);
		}
	}
	
	void OnPopArmyExplain(GameObject go)
	{
		GameObject infowin = U3dCmn.GetObjFromPrefab("BulletinWin");
		if(infowin == null) return;
		
		uint ExcelID = (uint)CMNCMD.HELP_TYPE.SOLDIER_SORT_HELP;
		Hashtable mbMap = CommonMB.InstanceMBInfo_Map;
		if (true == mbMap.ContainsKey(ExcelID))
		{
			InstanceMBInfo info = (InstanceMBInfo)CommonMB.InstanceMBInfo_Map[ExcelID];
			infowin.GetComponent<BulletinWin>().title_label.text = info.mode;
			infowin.GetComponent<BulletinWin>().text_label.text = info.Rule1;
			infowin.SendMessage("RevealPanel");
		}
	}
}
