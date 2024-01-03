using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using STCCMD;
using CTSCMD;

// 武将 选择,补给,出征 等公用面板 

public class PopGeneralRoadBar : MonoBehaviour {
	
	public UILabel EnemyArmyCol = null;
	public UILabel EnemyPowerCol = null;
	public UILabel HeroPowerCol = null;
	
	public GameObject Troop;
	
	public int spacing = 80;
	public GameObject RoadGeneralCol = null;
	public GameObject template = null;
	
	PickingGeneralItem[] RoadCol = new PickingGeneralItem[5];
	PickingGeneral[] camps = new PickingGeneral[5];
	
	public GameObject PopDisband = null;
	public GameObject ButtonPeibing = null;
	public GameObject ButtonManualSupply = null;
	public GameObject ButtonPicking = null;
	
	string mPrettyEnemyPower;
	string mPrettyHeroPower;
	string mPrettyEnemyArmy;
	
	void Awake() {
		if (EnemyPowerCol != null)
		{
			mPrettyEnemyPower = EnemyPowerCol.text;
			EnemyPowerCol.enabled = false;
		}
		if (HeroPowerCol != null)
		{
			mPrettyHeroPower = HeroPowerCol.text;
			HeroPowerCol.text = string.Format(mPrettyHeroPower,0);
		}
		if (EnemyArmyCol != null)
		{
			mPrettyEnemyArmy = EnemyArmyCol.text;
			EnemyArmyCol.enabled = false;
			//Troop.SetActiveRecursively(false);
		}

		onCreateOrReposition();
	}
	
	void OnDestroy() { onExceptDelegate(); }
	void onExceptDelegate() 
	{			
		//print("- onExceptDelegate");
		
		BaizGeneralRoadWin.applyBaizCampDataDelegate  = null;
		CombatManager.processManualSupplyDelegate -= OnProcessManualSupplyDelegate;
		JiangLingManager.processHireHeroInfoDelegate -= OnProcessHireHeroInfoDelegate;
	}
	
	// Use this for initialization
	void Start () {
		
		if (ButtonPeibing != null) {
			UIEventListener.Get(ButtonPeibing).onClick = OnPopJiangLingReveal;
		}
		if (ButtonManualSupply != null) {
			UIEventListener.Get(ButtonManualSupply).onClick = OnPopManualSupply;
		}
		if (ButtonPicking != null) {
			UIEventListener.Get (ButtonPicking).onClick = OnPopGeneralsPicking;
		}
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
	
	public PickingGeneral[] GetCGenerals()
	{
		return camps;
	}
	
	/// <summary>
	/// 选择武将 ....
	/// </summary>
	void OnPopGeneralsPicking(GameObject tween)
	{
		JiangLingManager.processHireHeroInfoDelegate += OnProcessHireHeroInfoDelegate;
		JiangLingManager.GetJiangLingInfo();
	}
	
	void OnProcessHireHeroInfoDelegate()
	{
		if (PopDisband == null) return;
		
		UIAnchor comp = PopDisband.GetComponentInChildren<UIAnchor>();
		if (comp == null) return;
		
		GameObject go = U3dCmn.GetObjFromPrefab("BaizGeneralRoadWin");
		if (go == null) return;
		
		BaizGeneralRoadWin t = go.GetComponent<BaizGeneralRoadWin>();
		if (t != null)
		{
			float depth = comp.depthOffset - BaizVariableScript.DEPTH_OFFSET;
			t.Depth(depth);
			t.AssignRoadGeneral();
			
			BaizGeneralRoadWin.applyBaizCampDataDelegate  = OnApplyBaizCampData;	
		}
	}
	
	/// <summary>
	/// Raises the accept baiz camp data event.
	/// </summary>
	/// <param name='data'>
	/// Data.
	/// </param>
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
		
		if (HeroPowerCol != null)
		{
			HeroPowerCol.text = string.Format(mPrettyHeroPower,totalProf);
		}
	}
	
	// 配兵界面返回后的刷新界面操作 ...
	void OnPopRefreshHeroDataDelegate()
	{
		int totalProf = 0;
		int i=0;
		
		Hashtable jlMap = JiangLingManager.MyHeroMap;
		for (; i<5; ++ i)
		{
			PickingGeneral t = camps[i];
			if (t == null) continue;
			if (true == jlMap.ContainsKey(t.nHeroID))
			{
				HireHero h1Hero = (HireHero) jlMap[t.nHeroID];
				t.nHealth = h1Hero.nHealth7;
				t.nHeroID = (ulong) h1Hero.nHeroID1;
				t.nModel = h1Hero.nModel12;
				t.nLevel = h1Hero.nLevel19;	
				t.nArmyLevel = h1Hero.nArmyLevel10;
				t.nArmyNum = h1Hero.nArmyNum11;
				t.nArmyType = h1Hero.nArmyType9;
				t.nProf = h1Hero.nProf20;
				t.nHealthStatus = h1Hero.nHealthState21;
			}
			
			totalProf = totalProf + t.nProf;
			PickingGeneralItem item = RoadCol[i];
			item.ApplyGeneralItem(t);
		}
		
		if (HeroPowerCol != null)
		{
			HeroPowerCol.text = string.Format(mPrettyHeroPower,totalProf);
		}
	}
	
	// 调用将领界面 ...
	void OnPopJiangLingReveal(GameObject tween)
	{
		GameObject go = U3dCmn.GetObjFromPrefab("JiangLingInfoWin");
		if (go == null ) return;
		JiangLingInfoWin.RefreshHeroData = OnPopRefreshHeroDataDelegate;
		go.SendMessage("RevealPanel");
	}
	
	// 敌方战力, 布阵描述 ...
	public void ApplyCombatProfData(CombatProfData data)
	{
		if (EnemyPowerCol != null)
		{
			EnemyPowerCol.text = string.Format(mPrettyEnemyPower,data.nProf);
			EnemyPowerCol.enabled = true;
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
		
		if (EnemyArmyCol != null)
		{
			//EnemyArmyCol.text = string.Format(mPrettyEnemyArmy,ilArray);
			EnemyArmyCol.enabled = true;
		}
	}
	
	// 手动补给界面 ...
	void OnPopManualSupply(GameObject tween)
	{
		CTS_GAMECMD_MANUAL_SUPPLY_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_MANUAL_SUPPLY;
		req.nInstanceID3 = 0;
		req.nCombatType4 = 0;
		req.n1Hero5 = 0;
		req.n2Hero6 = 0;
		req.n3Hero7 = 0;
		req.n4Hero8 = 0;
		req.n5Heor9 = 0;
		
		if (camps[0] != null) { req.n1Hero5 = camps[0].nHeroID; }
		if (camps[1] != null) { req.n2Hero6 = camps[1].nHeroID; }
		if (camps[2] != null) { req.n3Hero7 = camps[2].nHeroID; }
		if (camps[3] != null) { req.n4Hero8 = camps[3].nHeroID; }
		if (camps[4] != null) { req.n5Heor9 = camps[4].nHeroID; }
		
		LoadingManager.instance.ShowLoading();
		CombatManager.processManualSupplyDelegate = OnProcessManualSupplyDelegate;
		TcpMsger.SendLogicData<CTS_GAMECMD_MANUAL_SUPPLY_T>(req);
	}
	
	void OnProcessManualSupplyDelegate()
	{
		LoadingManager.instance.HideLoading();
		if (camps == null) return;
		
		Hashtable jlMap = JiangLingManager.MyHeroMap;
		
		int totalProf = 0;
		for (int i=0; i<5; ++ i)
		{
			PickingGeneral t = camps[i];
			if (t == null) continue;
			
			if (true == jlMap.ContainsKey(t.nHeroID))
			{
				HireHero h1Hero = (HireHero) jlMap[t.nHeroID];
				t.nArmyNum = h1Hero.nArmyNum11;
				t.nProf = h1Hero.nProf20;
				t.nHealthStatus = h1Hero.nHealthState21;
				
				totalProf = totalProf + t.nProf;
				
				// Set item
				PickingGeneralItem item = RoadCol[i];
				item.ApplyGeneralItem(t);
			}
		}
		
		if (HeroPowerCol != null)
		{
			HeroPowerCol.text = string.Format(mPrettyHeroPower,totalProf);
		}
	}
	
	public void SetMyGoldmineCGenerals(MyGoldmineData newCard)
	{
		List<PickingGeneral> cacheList = new List<PickingGeneral>();
		
		ulong[] nHeroArray = new ulong[5];
		nHeroArray[0] = newCard.n1HeroID;
		nHeroArray[1] = newCard.n2HeroID;
		nHeroArray[2] = newCard.n3HeroID;
		nHeroArray[3] = newCard.n4HeroID;
		nHeroArray[4] = newCard.n5HeroID;
		
		Hashtable jlMap = JiangLingManager.MyHeroMap;
		for (int i=0; i<5; ++ i)
		{
			ulong id = nHeroArray[i];
			if (true == jlMap.ContainsKey(id))
			{
				HireHero h1Hero = (HireHero) jlMap[id];
				PickingGeneral new1 = new PickingGeneral();
				new1.name = DataConvert.BytesToStr(h1Hero.szName3);
				new1.nHealth = h1Hero.nHealth7;
				new1.nHeroID = (ulong) h1Hero.nHeroID1;
				new1.nHealthStatus = h1Hero.nHealthState21;
				new1.nModel = h1Hero.nModel12;
				new1.nLevel = h1Hero.nLevel19;	
				new1.nArmyLevel = h1Hero.nArmyLevel10;
				new1.nArmyNum = h1Hero.nArmyNum11;
				new1.nArmyType = h1Hero.nArmyType9;
				new1.nProf = h1Hero.nProf20;
				new1.campIn = (h1Hero.nStatus14>0);
				
				cacheList.Add(new1);
			}	
		}
		
		OnApplyBaizCampData(cacheList);
	}
	
	public void SetCGenerals(ulong nInstanceID)
	{
		BaizInstanceGroup newGroup = null;
		newGroup = BaizInstanceManager.instance.TryGetInstanceGroup(nInstanceID);
		if (newGroup != null)
		{
			if (newGroup.nNum>0)
			{
				List<PickingGeneral> cacheList = new List<PickingGeneral>();
				
				BaizInstanceData data = newGroup.m_ma[0];
				ulong[] nHeroArray = new ulong[5];
				nHeroArray[0] = data.n1HeroID;
				nHeroArray[1] = data.n2HeroID;
				nHeroArray[2] = data.n3HeroID;
				nHeroArray[3] = data.n4HeroID;
				nHeroArray[4] = data.n5HeroID;

				Hashtable jlMap = JiangLingManager.MyHeroMap;
				for (int i=0; i<5; ++ i)
				{
					ulong id = nHeroArray[i];
					if (true == jlMap.ContainsKey(id))
					{
						HireHero h1Hero = (HireHero) jlMap[id];
						PickingGeneral newCard = new PickingGeneral();
						newCard.name = DataConvert.BytesToStr(h1Hero.szName3);
						newCard.nHealth = h1Hero.nHealth7;
						newCard.nHeroID = (ulong) h1Hero.nHeroID1;
						newCard.nModel = h1Hero.nModel12;
						newCard.nLevel = h1Hero.nLevel19;	
						newCard.nArmyLevel = h1Hero.nArmyLevel10;
						newCard.nArmyNum = h1Hero.nArmyNum11;
						newCard.nArmyType = h1Hero.nArmyType9;
						newCard.nProf = h1Hero.nProf20;
						newCard.nHealthStatus = h1Hero.nHealthState21;
						newCard.campIn = (h1Hero.nStatus14>0);
						cacheList.Add(newCard);
					}	
				}
				
				OnApplyBaizCampData(cacheList);
			}
		}
	}
	
	// 新手引导设置一个武将 ...
	public void NewbieSetCGeneral()
	{
		Hashtable jlMap = JiangLingManager.MyHeroMap;
		ArrayList idList = new ArrayList(jlMap.Keys);
		if (idList.Count == 0) return;
		
		List<PickingGeneral> cacheList = new List<PickingGeneral>();
					
		ulong id = (ulong)idList[0];
		if (true == jlMap.ContainsKey(id))
		{
			HireHero h1Hero = (HireHero) jlMap[id];
			PickingGeneral new1 = new PickingGeneral();
			new1.name = DataConvert.BytesToStr(h1Hero.szName3);
			new1.nHealth = h1Hero.nHealth7;
			new1.nHeroID = (ulong) h1Hero.nHeroID1;
			new1.nHealthStatus = h1Hero.nHealthState21;
			new1.nModel = h1Hero.nModel12;
			new1.nLevel = h1Hero.nLevel19;	
			new1.nArmyLevel = h1Hero.nArmyLevel10;
			new1.nArmyNum = h1Hero.nArmyNum11;
			new1.nArmyType = h1Hero.nArmyType9;
			new1.nProf = h1Hero.nProf20;
			new1.campIn = (h1Hero.nStatus14>0);
			
			cacheList.Add(new1);
		}
		
		OnApplyBaizCampData(cacheList);
	}
}
