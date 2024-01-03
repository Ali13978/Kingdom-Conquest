using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using STCCMD;

// 地图站     
public class NanBeizhanDiTuCli
{
	public string name;
	public List<NanBeizhanMBInfo> d5 = new List<NanBeizhanMBInfo>();
}

// 盟军将领 General

public class NanBeizhanInstance : MonoBehaviour {
	public static int NanBeizhanLevelNum = 5;
	static NanBeizhanInstance mInst = null;
	
	BaizInstanceData[] mAllies = new BaizInstanceData[5];
	
	PickingGeneral[] mGenerals = new PickingGeneral[5]; 
	NanBeizhanDiTuCli[] mDiTuCol = null;//new NanBeizhanDiTuCli[NanBeizhanLevelNum];		// 五个难度级别...
	
	CombatFighting PosterAtFighting = null;
	
	ulong m_combat_id = 0;
	ulong m_id = 0;
	ulong m_creator_id = 0;
	
	string m_captain_name;
	
	// 副本类型   Dot
	int m_general_id = 0;
	int m_class_id = 0;
	int m_day_rest_times_free = 0;
	int m_day_rest_times_fee = 0;
	
	bool mIsAutoCombat = false;
		
	// nan bei zhan
	static public NanBeizhanInstance instance
	{
		get
		{
			if (mInst == null)
			{
				mInst = UnityEngine.Object.FindObjectOfType(typeof(NanBeizhanInstance)) as NanBeizhanInstance;

				if (mInst == null)
				{
					GameObject go = new GameObject("_NanBeizhanInstance");
					DontDestroyOnLoad(go);
					mInst = go.AddComponent<NanBeizhanInstance>();
					mInst.ApplyNanBeizhanDiTuStatus();
				}
			}
			
			return mInst;
		}
	}
	
	public ulong idInstance
	{
		get {
			return m_id;
		}
		
		set {
			m_id = value;
		}
	}
	
	public int idClass
	{
		get {
			return m_class_id;
		}
		
		set {
			m_class_id = value;
		}
	}
	
	public int idRestDayTimesFree
	{
		get {
			return m_day_rest_times_free;
		}
		set {
			m_day_rest_times_free = value;
		}
	}
	
	public int idRestDayTimesFee
	{
		get {
			return m_day_rest_times_fee;
		}
		set {
			m_day_rest_times_fee = value;
		}
	}
	
	public ulong idCombat
	{
		get {
			return m_combat_id;
		}
		set {
			m_combat_id = value;
		}
	}
	
	public int idCol
	{
		get {
			return m_general_id;
		}
		set {
			m_general_id = value;
		}
	}
	
	public ulong idCreator
	{
		get {
			return m_creator_id;
		}
		set {
			m_creator_id = value;
		}
	}
	
	public string idCaptain
	{
		get {
			return m_captain_name;
		}
		set {
			m_captain_name = value;
		}
	}
	
	public bool isAutoCombat
	{
		get {
			return mIsAutoCombat;
		}
		set {
			mIsAutoCombat = value;
		}
	}
	
	public CombatFighting idFighting
	{
		get {
			return PosterAtFighting;
		}
		set {
			PosterAtFighting = value;
		}
	}
	
	void Awake() {
		if (mInst == null) 
		{ 
			mInst = this; 
			DontDestroyOnLoad(gameObject); 
			onEnableDelegate(); 
		} 
		else { 
			Destroy(gameObject);
		} 
		if(CommonData.player_online_info.ZhengZhan >=10)
		{
			if(CommonData.player_online_info.ZhengZhan < U3dCmn.GetWorldConquestMaxLv())
				NanBeizhanLevelNum +=  (int)(CommonData.player_online_info.ZhengZhan-9);
			else
				NanBeizhanLevelNum += U3dCmn.GetWorldConquestMaxLv() - 10;
		}
		mInst.mDiTuCol = new NanBeizhanDiTuCli[NanBeizhanLevelNum];
	}
	void OnDestroy() { if (mInst == this) mInst = null; onExceptDelegate(); }
	void onEnableDelegate()
	{
		
		BaizInstanceManager.processDestroyInstanceCon1 = OnProcessDestroyInstanceCon1;
		BaizInstanceManager.processNanBeizhanInstanceDataCon1 = OnProcessNanBeizhanInstanceDataCon1;
		BaizInstanceManager.processNanBeizhanQuitInstanceCon1 = OnProcessNanBeizhanQuitInstanceCon1;
	}
	void onExceptDelegate()
	{
		BaizInstanceManager.processDestroyInstanceCon1 -= OnProcessDestroyInstanceCon1;
		BaizInstanceManager.processNanBeizhanInstanceDataCon1 -= OnProcessNanBeizhanInstanceDataCon1;
		BaizInstanceManager.processNanBeizhanQuitInstanceCon1 -= OnProcessNanBeizhanQuitInstanceCon1;
	}
	
	void OnProcessDestroyInstanceCon1(ulong nInstanceID)
	{
		//print("+ OnProcessDestroyInstanceCon1 _NanBeizhanInstance:" + nInstanceID);
		if (m_id != nInstanceID) return;
		AssignInstanceGeneralFree();
	}
	
	void OnProcessNanBeizhanInstanceDataCon1(BaizInstanceGroup newGroup)
	{
		//print("+ ApplyNanBeizhanInstanceCon1 _NanBeizhanInstance:" + newGroup.nInstanceID);
		if (newGroup.nInstanceID != m_id) return;
		ApplyNanBeizhanInstanceCon1(newGroup);
	}
	
	void OnProcessNanBeizhanQuitInstanceCon1(ulong nInstanceID)
	{
		//print("+ OnProcessNanBeizhanQuitInstanceCon1 _NanBeizhanInstance:" + nInstanceID);
		if (m_id != nInstanceID) return;
		AssignInstanceGeneralFree();
	}
	
	void ApplyNanBeizhanDiTuStatus()
	{
		Hashtable nzbzMap = CommonMB.NanBeizhanInfo_Map;
		foreach(DictionaryEntry	de in nzbzMap)  
		{
			NanBeizhanMBInfo obj = (NanBeizhanMBInfo) de.Value;
			
			int i = obj.nLevel - 1;
			if (i<0 || i>NanBeizhanLevelNum-1) continue;
			
			NanBeizhanDiTuCli item = mDiTuCol[i];
			if (item == null) {
				item = new NanBeizhanDiTuCli();
			}
			
			item.d5.Add(obj);
			mDiTuCol[i] = item;
		}
	}
	
	public NanBeizhanDiTuCli GetDiTuCli(int itemID)
	{
		if (itemID<0 || itemID>NanBeizhanLevelNum-1) return null;
		return mDiTuCol[itemID];
		
	}
	
	public BaizInstanceDesc GetItem(int itemID)
	{
		List<BaizInstanceDesc> cacheList = BaizInstanceManager.instance.GetInstanceDescList();
		if (itemID<0 || itemID> (cacheList.Count-1)) return null;
		return cacheList[itemID];
	}
	
	public PickingGeneral[] GetGenerals() 
	{
		return mGenerals;
	}
	
	public BaizInstanceData[] GetAllies()
	{
		return mAllies;
	}
	
	public void AssignInstanceGeneralFree()
	{
		int iCol = m_general_id;
		if (idCol<0 || idCol>4) return;
		
		PickingGeneral[] args = mGenerals;
		PickingGeneral gen = args[iCol];
		if (gen == null) return;
		
		Hashtable jlMap = JiangLingManager.MyHeroMap;
		ulong n1HeroID = gen.nHeroID;
		if (true == jlMap.ContainsKey(n1HeroID))
		{
			HireHero h1Hero = (HireHero)jlMap[n1HeroID];
			h1Hero.nStatus14 = (int) CMNCMD.HeroState.NORMAL; // 标记为出征状态   Status
			jlMap[n1HeroID] = h1Hero;
		}
	}
	
	public void ApplyNanBeizhanInstanceCon1(BaizInstanceGroup newGroup)
	{
		PickingGeneral[] args = mGenerals;
		args[0] = null;
		args[1] = null;
		args[2] = null;
		args[3] = null;
		args[4] = null;
		
		ulong myAccountID = CommonData.player_online_info.AccountID;
		PickingGeneral myGeneral = null;
		int i=0;
		for(i=0; i<newGroup.nNumHero; ++ i)
		{
			BaizInstanceData data = newGroup.m_ma[i];
			PickingGeneral gen = newGroup.m_ga[i];
			args[i] = gen;		
			
			if (newGroup.nNumHero>0)
			{
				if (gen != null)
				{
					if (myAccountID == data.nAccountID)
					{
						m_general_id = i;
						myGeneral = gen;
					}
				}
			}
		}
		
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
				
				if (delta != 0)
				{
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
	}
	
	public void ApplyInstanceGroupGeneral(BaizInstanceGroup newGroup)
	{
		PickingGeneral[] args = mGenerals;
		args[0] = null;
		args[1] = null;
		args[2] = null;
		args[3] = null;
		args[4] = null;
		
		ulong myAccountID = CommonData.player_online_info.AccountID;
		int i=0;
		for(i=0; i<newGroup.nNumHero; ++ i)
		{
			BaizInstanceData data = newGroup.m_ma[i];
			PickingGeneral gen = newGroup.m_ga[i];
			args[i] = gen;	
			
			if (newGroup.nNumHero>0)
			{
				if (gen != null)
				{
					if (myAccountID == data.nAccountID)
					{
						m_general_id = i;
					}
				}
			}
		}
	}
	
	public bool IsInstanceCreator()
	{
		ulong nAccountID = CommonData.player_online_info.AccountID;
		return (nAccountID == m_creator_id);
	}
	
	public void ApplyInstanceCombatID()
	{
		List<CombatDescUnit> cacheList = CombatManager.instance.GetCombatList();
		for (int i=0,imax=cacheList.Count; i<imax; ++ i)
		{
			CombatDescUnit new1 = cacheList[i];
			if (new1.nCombatType == (int)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN) {
				m_combat_id = new1.nCombatID;
				break;
			}
		}
	}
	
	void DisbandForUnpack1()
	{
		NanBeizhanLaunchWin go1 = GameObject.FindObjectOfType(typeof(NanBeizhanLaunchWin)) as NanBeizhanLaunchWin;
		NanBeizhanHall go2 = GameObject.FindObjectOfType(typeof(NanBeizhanHall)) as NanBeizhanHall;
		NanBeizhanRoom go3 = GameObject.FindObjectOfType(typeof(NanBeizhanRoom)) as NanBeizhanRoom;
		NanBeizhanScene combatWin1 = GameObject.FindObjectOfType(typeof(NanBeizhanScene)) as NanBeizhanScene;
		if (go1 == null && go2 == null && go3 == null && combatWin1 == null) return;
		
		NanBeizhanGuardWin go5 = GameObject.FindObjectOfType(typeof(NanBeizhanGuardWin)) as NanBeizhanGuardWin;
		if (go5 != null) { Destroy(go5.gameObject); }
		
		NanBeizhanAutoCombatWin go6 = GameObject.FindObjectOfType(typeof(NanBeizhanAutoCombatWin)) as NanBeizhanAutoCombatWin;
		if (go6 != null) { Destroy(go6.gameObject); }
		
		NanBeizhanTroop go7 = GameObject.FindObjectOfType(typeof(NanBeizhanTroop)) as NanBeizhanTroop;
		if (go7 != null) { Destroy(go7.gameObject); }
		
		NanBeizhanReadyWin go8 = GameObject.FindObjectOfType(typeof(NanBeizhanReadyWin)) as NanBeizhanReadyWin;
		if (go8 != null) { Destroy(go8.gameObject); }
		
		PickingGeneralWin win2 = GameObject.FindObjectOfType(typeof(PickingGeneralWin)) as PickingGeneralWin;
		if (win2 != null) { Destroy(win2.gameObject); }
		
		BaizGeneralRoadWin win1 = GameObject.FindObjectOfType(typeof(BaizGeneralRoadWin)) as BaizGeneralRoadWin;
		if (win1 != null) { Destroy(win1.gameObject); }
		
		// 将重新开启iphone5 遮挡 ...
		if (go1 == null)
		{
			KeepOut.byShown = 0;
			//KeepOut.instance.HideKeepOut();
		}
		
		// 重新开启副本
		BuildingManager cc = GameObject.FindObjectOfType(typeof(BuildingManager)) as BuildingManager;
		if (cc != null) {
			cc.SendMessage("OpenNanZheng", null); 
		}
	}
	
	
}
