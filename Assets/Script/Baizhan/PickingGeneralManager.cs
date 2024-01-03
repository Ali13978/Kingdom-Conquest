using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using STCCMD;

public class PickingGeneral
{
	public ulong nAccountID = 0;			// 英雄是哪个玩家的 ...
	public string charName;					// 角色名称 ...
	public ulong nHeroID = 0;
	public int nModel = 0;
	public int nHealth = 0;
	public string name;
	public int nArmyLevel;					// 兵种等级 Level
	public int nArmyNum;					// 带兵数量 num
	public int nLevel = 0;
	public int nArmyType;					// 兵种 type
	public int nProf;
	public int nHealthStatus;				// 健康状态   health
	public bool campIn = false;
}

public class PickingGeneralManager : MonoBehaviour {
	
	static PickingGeneralManager mInst = null;
	
	List<PickingGeneral> mGenerals = new List<PickingGeneral>();

	/// <summary>
	/// The instance of the CombatManager class. Will create it if one isn't already around.
	/// </summary>

	static public PickingGeneralManager instance
	{
		get
		{
			if (mInst == null)
			{
				mInst = Object.FindObjectOfType(typeof(PickingGeneralManager)) as PickingGeneralManager;

				if (mInst == null)
				{
					GameObject go = new GameObject("_PickingGeneralManager");
					DontDestroyOnLoad(go);
					mInst = go.AddComponent<PickingGeneralManager>();
				}
			}
			
			return mInst;
		}
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// 根据征战天的武将数组生成武将列表 ...
	public void ApplyNzbzGeneral()
	{
		mGenerals.Clear();
		
		PickingGeneral[] args = NanBeizhanInstance.instance.GetGenerals();
		for (int i=0,imax=args.Length; i<imax; ++ i)
		{
			PickingGeneral item = args[i];
			if (item != null)
			{
				item.campIn = false;
				
				// 加入列表里 ...
				mGenerals.Add(item);
			}
		}
	}
	
	// 根据招募的武将生成武将列表 ...
	public void ApplyHireGeneral()
	{
		mGenerals.Clear();
		
		Hashtable jlMap = JiangLingManager.MyHeroMap;
		foreach(DictionaryEntry	de in jlMap)  
		{
			HireHero obj = (HireHero) de.Value;
			
			// 有军队么?  
			//if (obj.nArmyType9 == 0) continue; 
			if (obj.nStatus14 == 0) // war in false;
			{
				PickingGeneral item = new PickingGeneral();
				item.nHeroID = (ulong) obj.nHeroID1;
				item.nHealth = obj.nHealth7;
				item.nArmyLevel = obj.nArmyLevel10;
				item.nArmyNum = obj.nArmyNum11;
				item.nLevel = obj.nLevel19;
				item.nArmyType = obj.nArmyType9;
				item.nProf	= obj.nProf20;
				item.nModel = obj.nModel12;
				item.nHealthStatus = obj.nHealthState21;
				item.name = DataConvert.BytesToStr(obj.szName3);
				item.campIn = false;				
				
				mGenerals.Add(item);
			}
		}
	}
	
	public PickingGeneral GetItem(int itemID)
	{
		if (itemID<0 || itemID> (mGenerals.Count-1)) return null;
		return mGenerals[itemID];
	}
	
	public List<PickingGeneral> GetGeneralList()
	{
		return mGenerals;
	}
	
	// 获取第一个可以出征的武将 ...
	public PickingGeneral GetFirst()
	{
		int imax = mGenerals.Count;
		for (int i=0; i<imax; ++ i)
		{
			PickingGeneral gen = mGenerals[i];
			if (gen.nArmyNum>0) return gen;
		}
		
		// Fini
		return null;
	}
}
