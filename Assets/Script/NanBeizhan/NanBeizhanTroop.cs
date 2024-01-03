using UnityEngine;
using System.Collections;
using STCCMD;
using CTSCMD;
using CMNCMD;
public class NanBeizhanTroop: MonoBehaviour {
	
	public GameObject ButtonPicking = null;
	public GameObject ButtonKick = null;
	public GameObject ButtonManual = null;
	
	public UISprite Faceid = null;
	public UILabel PrettyNameCol = null;
	public UILabel PrettyHealthCol = null;
	public UILabel PrettyLevelCol = null;
	public UILabel ArmyLevelOrNumCol = null;
	public UILabel PowerCol = null;
	public UILabel TipCol = null;
	
	string mPrettyLevel;
	string mArmyLevelOrNum;
	string mPrettyPower;
	string mPrettyHealth;
	string mPrettyTipCol;
	
	private ulong mAccountID = 0;
	
	PickingGeneral m_CGeneral = null;
	PickingGeneral m_cache = null;
	UIAnchor mDepth = null;
	
	void Awake() {
		
		if (PrettyNameCol != null) {
			NGUITools.SetActiveSelf(PrettyNameCol.gameObject, false);
		}
		if (PrettyLevelCol != null)
		{
			mPrettyLevel = PrettyLevelCol.text;
			PrettyLevelCol.text = string.Format(mPrettyLevel,0);
			NGUITools.SetActiveSelf(PrettyLevelCol.gameObject, false);
		}
		if (TipCol != null)
		{
			mPrettyTipCol = TipCol.text;
			TipCol.text = string.Format(mPrettyTipCol,"...");
		}
		if (ArmyLevelOrNumCol != null)
		{
			mArmyLevelOrNum = ArmyLevelOrNumCol.text;
			NGUITools.SetActiveSelf(ArmyLevelOrNumCol.gameObject,false);
		}
		if (PowerCol != null)
		{
			mPrettyPower = PowerCol.text;
			PowerCol.text = string.Format(mPrettyPower,0);
			NGUITools.SetActiveSelf(PowerCol.gameObject, false);
		}
		if (PrettyHealthCol != null)
		{
			mPrettyHealth = PrettyHealthCol.text;
			PrettyHealthCol.text = string.Format(mPrettyHealth,100);
			NGUITools.SetActiveSelf(PrettyHealthCol.gameObject, false);
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
	}
	
	void OnDestroy() { onExceptDelegate(); }
	void onExceptDelegate()
	{
		//print ("- onExceptDelegate");
		
		CombatManager.processManualSupplyDelegate -= OnProcessNanBeizhanManualSupplyDelegate;
		
		BaizInstanceManager.processKickInstanceHeroDelegate -= OnNanBeizhanTroopClose;
		BaizInstanceManager.processConfigInstanceHeroDelegate -= OnProcessConfigInstanceHeroDelegate;
		
		PickingGeneralWin.applyGeneralDelegate = null;
	}
	
	// Use this for initialization
	void Start () {
	
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
	
	void OnNanBeizhanTroopClose()
	{
		NGUITools.SetActive(gameObject, false);
		Destroy(gameObject);
	}
	
	public void AssignTroopAction(bool onKick, bool onPicking)
	{
		if (ButtonKick != null)
		{
			ButtonKick.gameObject.SetActiveRecursively(onKick);
		}
		if (ButtonPicking != null)
		{
			ButtonPicking.gameObject.SetActiveRecursively(onPicking);
		}
		if (ButtonManual != null)
		{
			ButtonManual.gameObject.SetActiveRecursively(onPicking);
		}
	}
	
	public void AssignTroopTip()
	{
		if (TipCol != null)
		{
			uint cls = (uint) NanBeizhanInstance.instance.idClass;
			Hashtable csMap = CommonMB.NanBeizhanInfo_Map;
			if (true == csMap.ContainsKey(cls))
			{
				NanBeizhanMBInfo info = (NanBeizhanMBInfo) csMap[cls];
				TipCol.text = string.Format(mPrettyTipCol, info.name);
			}
		}
	}
	
	public void AssignAccountID(ulong nAccountID)
	{
		mAccountID = nAccountID;
	}
	
	public void AssignGeneralPicking(PickingGeneral item)
	{
		m_CGeneral = item;
		
		if (m_CGeneral == null) return;
				
		if (Faceid != null)
		{
			Faceid.spriteName = U3dCmn.GetHeroIconName(item.nModel);
			Faceid.MakePixelPerfect();
		}
		if (PrettyNameCol != null)
		{
			PrettyNameCol.text = item.name;
			NGUITools.SetActiveSelf(PrettyNameCol.gameObject,true);
		}
		if (ArmyLevelOrNumCol != null)
		{
			uint armyid = (uint)item.nArmyType;
			SoldierMBInfo info = U3dCmn.GetSoldierInfoFromMb(armyid,1);
			ArmyLevelOrNumCol.text = string.Format(mArmyLevelOrNum, item.nArmyLevel, info.Name, item.nArmyNum);
			NGUITools.SetActiveSelf(ArmyLevelOrNumCol.gameObject,true);
			
		}
		if (PrettyHealthCol != null)
		{
			PrettyHealthCol.text = string.Format(mPrettyHealth, item.nHealthStatus);
			NGUITools.SetActiveSelf(PrettyHealthCol.gameObject, true);
		}
		if (PrettyLevelCol != null)
		{
			PrettyLevelCol.text = string.Format(mPrettyLevel, item.nLevel);
			NGUITools.SetActiveSelf(PrettyLevelCol.gameObject, true);
		}
		if (PowerCol != null)
		{
			PowerCol.text = string.Format(mPrettyPower, item.nProf);
			NGUITools.SetActiveSelf(PowerCol.gameObject, true);
		}
	}
	
	void OnProcessConfigInstanceHeroDelegate()
	{
		LoadingManager.instance.HideLoading(); 
		AssignGeneralPicking(m_cache);
		
		if (m_cache != null)
		{
			AssignHeroStatus(m_cache.nHeroID);
			
			//print ("Troop HeroID:" + m_cache.nHeroID);
		}
	}
	
	void AssignHeroStatus(ulong nHeroID)
	{
		Hashtable jlMap = JiangLingManager.MyHeroMap;
		
		ulong emptyHeroID = 0, deHeroID = 0;
		foreach(DictionaryEntry	de in jlMap)  
		{
			HireHero h1Hero = (HireHero) de.Value;
			
			if (h1Hero.nStatus14 == (int) CMNCMD.HeroState.COMBAT_INSTANCE_NANZHENGBEIZHAN)
			{
				if (h1Hero.nHeroID1 == nHeroID) {
					return;
				}
				else {
					emptyHeroID = h1Hero.nHeroID1;
				}
			}
			else if (h1Hero.nHeroID1 == nHeroID)
			{
				deHeroID = h1Hero.nHeroID1;
			}
		}
		
		if (true == jlMap.ContainsKey(emptyHeroID))
		{
			HireHero hh1 = (HireHero) jlMap[emptyHeroID];
			hh1.nStatus14 = (int)CMNCMD.HeroState.NORMAL;
			jlMap[emptyHeroID] = hh1;
		}
		
		if (true == jlMap.ContainsKey(deHeroID))
		{
			HireHero hh1 = (HireHero) jlMap[deHeroID];
			hh1.nStatus14 = (int) CMNCMD.HeroState.COMBAT_INSTANCE_NANZHENGBEIZHAN;
			jlMap[deHeroID] = hh1;
		}
	}
	
	void OnKickNanBeizhanGeneral()
	{		
		ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
		if (nInstanceID == 0) return;
		BaizInstanceManager.processKickInstanceHeroDelegate = OnNanBeizhanTroopClose;
		BaizInstanceManager.RequestKickInstanceHero(101, nInstanceID, mAccountID);
	}
	
	public void AssignGeneralPickingCon1(PickingGeneral item)
	{
		// 配置武将 config
		ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
		if (nInstanceID == 0) return;
		
		if (item == null) return;
		m_cache = item;
		
		// 显示 Loading ....
		LoadingManager.instance.ShowLoading();
		BaizInstanceManager.processConfigInstanceHeroDelegate = OnProcessConfigInstanceHeroDelegate;
		BaizInstanceManager.RequestConfigInstanceHero(101, nInstanceID, item.nHeroID);
	}
	
	void OnOpenPickingGeneral()
	{
		GameObject go = U3dCmn.GetObjFromPrefab("PickingGeneralWin");
		if (go == null) return;
		
		PickingGeneralWin t = go.GetComponent<PickingGeneralWin>();
		if (t != null)
		{
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			t.Depth(depth);
			t.AssignPickingGeneral();
			PickingGeneralWin.applyGeneralDelegate = AssignGeneralPickingCon1;
		}
	}
	
	void OnProcessNanBeizhanManualSupplyDelegate()
	{
		LoadingManager.instance.HideLoading();
		Hashtable jlMap = JiangLingManager.MyHeroMap;
		PickingGeneral t = m_CGeneral;
		if (true == jlMap.ContainsKey(t.nHeroID))
		{
			HireHero h1Hero = (HireHero) jlMap[t.nHeroID];
			t.nArmyNum = h1Hero.nArmyNum11;
			t.nHealthStatus = h1Hero.nHealthState21;
			t.nProf = h1Hero.nProf20;
			AssignGeneralPickingCon1(t);
		}
	}
	
	void OnNanBeizhanGeneralManualSupply()
	{
		ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
		if (nInstanceID == 0) return;
		
		CTS_GAMECMD_MANUAL_SUPPLY_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_MANUAL_SUPPLY;
		req.nInstanceID3 = nInstanceID;
		req.nCombatType4 = 101;
		req.n1Hero5 = 0;
		req.n2Hero6 = 0;
		req.n3Hero7 = 0;
		req.n4Hero8 = 0;
		req.n5Heor9 = 0;
		
		// 显示 Loading ....
		LoadingManager.instance.ShowLoading();
		CombatManager.processManualSupplyDelegate = OnProcessNanBeizhanManualSupplyDelegate;
		TcpMsger.SendLogicData<CTS_GAMECMD_MANUAL_SUPPLY_T>(req);
	}

}
