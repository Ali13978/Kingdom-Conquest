using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;

public class BaizhanInstance : MonoBehaviour {
	
	static BaizhanInstance mInst = null;
	
	ulong m_id = 0;				// 副本ID
	ulong m_combat_id = 0;
	ulong m_creator_id = 0;
	
	bool mIsAutoCombat = false;
	int m_class_id = 0;
	
	// 失败后的战斗演示数据 ...
	CombatFighting PosterAtFighting = null;
	
	/// <summary>
	/// The instance of the BaizhanInstance class. Will create it if one isn't already around.
	/// </summary>

	static public BaizhanInstance instance
	{
		get
		{
			if (mInst == null)
			{
				mInst = Object.FindObjectOfType(typeof(BaizhanInstance)) as BaizhanInstance;

				if (mInst == null)
				{
					GameObject go = new GameObject("_BaizhanInstance");
					DontDestroyOnLoad(go);
					mInst = go.AddComponent<BaizhanInstance>();
				}
			}
			
			return mInst;
		}
	}
	
	// 战斗ID ... 
	public ulong idCombat
	{
		get {
			return m_combat_id;
		}
		set {
			m_combat_id = value;
		}
	}
	// 副本ID ... 
	public ulong idInstance
	{
		get {
			return m_id;
		}
		set {
			m_id = value;
		}
	}
	// 创建者ID ...
	public ulong idCreator
	{
		get {
			return m_creator_id;
		}
		set {
			m_creator_id = value;
		}
	}
	// 是否是自动战斗状态 ... 
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
	// 副本类型  ....
	public int idClass
	{
		get {
			return m_class_id;
		}
		set {
			m_class_id = value;
		}
	}
	
	void Awake() { if (mInst == null) { mInst = this; DontDestroyOnLoad(gameObject); } else { Destroy(gameObject); } }
	void OnDestroy() { if (mInst == this) mInst = null; }
	
	// Use this for initialization
	void Start () {
		
	}
	// 查找当前匹配的战斗 ID ...
	public void ApplyInstanceCombatID()
	{
		List<CombatDescUnit> cacheList = CombatManager.instance.GetCombatList();
		for (int i=0,imax=cacheList.Count; i<imax; ++ i)
		{
			CombatDescUnit new1 = cacheList[i];
			if (new1.nCombatType == (int)CombatTypeEnum.COMBAT_INSTANCE_BAIZHANBUDAI) {
				m_combat_id = new1.nCombatID;
				break;
			}
		}
	}
	
	/// <summary>
	/// 断线重连接 ...
	/// </summary>
	void DisbandForUnpack1()
	{
		BaizhanLaunchWin go2 = GameObject.FindObjectOfType(typeof(BaizhanLaunchWin)) as BaizhanLaunchWin;
		BaizhanCampWin combatWin1 = GameObject.FindObjectOfType(typeof(BaizhanCampWin)) as BaizhanCampWin;
		if (combatWin1 == null && go2 == null) return;
		
		BaizOnGuardWin go3 = GameObject.FindObjectOfType(typeof(BaizOnGuardWin)) as BaizOnGuardWin;
		if (go3 != null) { Destroy(go3.gameObject); }
		
		BaizChuZhenWin go4 = GameObject.FindObjectOfType(typeof(BaizChuZhenWin)) as BaizChuZhenWin;
		if (go4 != null) { Destroy(go4.gameObject); }
		
		BaizhanDeathWin go5 = GameObject.FindObjectOfType(typeof(BaizhanDeathWin)) as BaizhanDeathWin;
		if (go5 != null) { Destroy(go5.gameObject); }
		
		BaizGeneralRoadWin win1 = GameObject.FindObjectOfType(typeof(BaizGeneralRoadWin)) as BaizGeneralRoadWin;
		if (win1 != null) { Destroy(win1.gameObject); }
		
		// 将重新开启iphone5 遮挡 ...
		if (go2 == null)
		{
			KeepOut.byShown = 0;
			// KeepOut.instance.HideKeepOut();
		}
		
		// 删除战斗场景 ...
		BuildingManager cc = GameObject.FindObjectOfType(typeof(BuildingManager)) as BuildingManager;
		if (cc != null)
		{
			cc.SendMessage("OpenBaiZhan", null);
		}
	}
	
}
