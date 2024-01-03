using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;

// 世界金矿出征界面 ....
public class GoldmineChuZhen : MonoBehaviour {
	
	public UILabel PrettyHoldCol = null;
	public UILabel PrettyAddCol = null;
	public GameObject ButtonArmy = null;
	
	public PopGeneralRoadBar RoadBar = null;
	UIAnchor mDepth = null;
	
	GoldmineCardData mPrettyCard = null;
	
	string mPrettyHold;
	string mPrettyAdd;
	
	void Awake() {
		if (PrettyHoldCol != null)
		{
			mPrettyHold = PrettyHoldCol.text;
			PrettyHoldCol.text = string.Format(mPrettyHold,"");
		}
		if (PrettyAddCol != null)
		{
			mPrettyAdd = PrettyAddCol.text;
			PrettyAddCol.enabled = false;
		}
		if (ButtonArmy != null)
		{
			UIEventListener.Get(ButtonArmy).onClick = OnPopArmyExplainDelegate;
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
		
		GoldmineInstance.processRobGoldmineDelegate -= OnProcessRobGoldmineDelegate;
		GoldmineInstance.processConfigGoldmineHeroDelegate -= OnProcessConfigGoldmineHeroDelegate;
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
	
	public void OnGoldmineChuZhenClose()
	{		
		// 删除 ...
		Destroy(gameObject);
	}
	
	public void ApplyGoldmineData(GoldmineCardData newCard)
	{
		mPrettyCard = newCard;
		
		if (PrettyHoldCol != null) {
			PrettyHoldCol.text = string.Format(mPrettyHold,newCard.name);
		}
		
		if (PrettyAddCol != null) 
		{
			uint myLevel = (uint)CommonData.player_online_info.Level;
			Hashtable wgeMap = CommonMB.WorldGoldmineYieldMBInfo_Map;
			int d1 = newCard.n1Addition;
			if (true == wgeMap.ContainsKey(myLevel))
			{
				GoldmineYieldMBInfo card = (GoldmineYieldMBInfo) wgeMap[myLevel];
				d1 = Mathf.FloorToInt((float)d1 * card.exp);
			}
			
			PrettyAddCol.text = string.Format(mPrettyAdd,newCard.n1Secs, d1);
			PrettyAddCol.enabled = true;
		}
	}
	
	public void ApplyMyGoldmineData(MyGoldmineData newCard)
	{
		if (PrettyHoldCol != null) {
			PrettyHoldCol.text = string.Format(mPrettyHold,newCard.name);
		}
		
		if (PrettyAddCol != null)
		{
			uint myLevel = (uint)CommonData.player_online_info.Level;
			Hashtable wgeMap = CommonMB.WorldGoldmineYieldMBInfo_Map;
			int d1 = newCard.n1Addition;
			if (true == wgeMap.ContainsKey(myLevel))
			{
				GoldmineYieldMBInfo card = (GoldmineYieldMBInfo) wgeMap[myLevel];
				d1 = Mathf.FloorToInt((float)d1 * card.exp);
			}
			
			PrettyAddCol.text = string.Format(mPrettyAdd,newCard.n1Secs, d1);
			PrettyAddCol.enabled = true;
		}
		
		if (RoadBar != null)
		{
			RoadBar.SetMyGoldmineCGenerals(newCard);
		}
	}
	// 发起金矿抢夺 ....
	public void OnGoldmineRobStartCombat()
	{
		if (mPrettyCard == null) return;
		if (RoadBar == null) return;
		
		// 查询下当前将领带兵是否有为0的....
		PickingGeneral[] camps = RoadBar.GetCGenerals();
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
		
		// 开启战斗 ...
		CTS_GAMECMD_ROB_WORLDGOLDMINE_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ROB_WORLDGOLDMINE;
		req.nArea3 = mPrettyCard.nAreaID;
		req.nClass4 = mPrettyCard.nClass;
		req.nIdx5 = mPrettyCard.nIdx;
		req.n1Hero6 = 0;
		req.n2Hero7 = 0;
		req.n3Hero8 = 0;
		req.n4Hero9 = 0;
		req.n5Hero10 = 0;
		
		if (camps[0] != null) { req.n1Hero6 	= camps[0].nHeroID; }
		if (camps[1] != null) { req.n2Hero7 	= camps[1].nHeroID; }
		if (camps[2] != null) { req.n3Hero8 	= camps[2].nHeroID; }
		if (camps[3] != null) { req.n4Hero9 	= camps[3].nHeroID; }
		if (camps[4] != null) { req.n5Hero10 	= camps[4].nHeroID; }
		
		LoadingManager.instance.ShowLoading();
		GoldmineInstance.processRobGoldmineDelegate += OnProcessRobGoldmineDelegate;
		TcpMsger.SendLogicData<CTS_GAMECMD_ROB_WORLDGOLDMINE_T>(req);
	}
	
	void OnProcessRobGoldmineDelegate(ulong nCombatID)
	{
		LoadingManager.instance.HideLoading();
		// 战斗实例 ...
		CombatFighting comFighting = null;
		SortedList<ulong,CombatFighting> combatMap = CombatManager.instance.GetCombatFightingMap();
		if (true == combatMap.TryGetValue(nCombatID, out comFighting))
		{
			ulong[] nHeroArray = new ulong[5];
			int imax = 0;
			
			PickingGeneral[] camps = RoadBar.GetCGenerals();
			if (camps[0] != null) { nHeroArray[0] = camps[0].nHeroID; imax++; }
			if (camps[1] != null) { nHeroArray[1] = camps[1].nHeroID; imax++; }
			if (camps[2] != null) { nHeroArray[2] = camps[2].nHeroID; imax++; }
			if (camps[3] != null) { nHeroArray[3] = camps[3].nHeroID; imax++; }
			if (camps[4] != null) { nHeroArray[4] = camps[4].nHeroID; imax++; }
		
			Hashtable jlMap = JiangLingManager.MyHeroMap;
			for (int i=0; i<imax; ++i)
			{
				ulong id = nHeroArray[i];
				HireHero h1Hero = (HireHero)jlMap[id];
				h1Hero.nStatus14 = (int) CMNCMD.HeroState.NORMAL;
				jlMap[id] = h1Hero;
			}
			
			combatMap[nCombatID] = comFighting;
		}
		
		// 删除自己 ...
		Destroy(gameObject);
	}
	
	void OnGoldmineDefenseConfirm()
	{	
		if (RoadBar == null) return;
		
		CTS_GAMECMD_CONFIG_WORLDGOLDMINE_HERO_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_CONFIG_WORLDGOLDMINE_HERO;
		req.n1Hero3 = 0;
		req.n2Hero4 = 0;
		req.n3Hero5 = 0;
		req.n4Hero6 = 0;
		req.n5Hero7 = 0;
		
		PickingGeneral[] camps = RoadBar.GetCGenerals();
		if (camps[0] != null) { req.n1Hero3 = camps[0].nHeroID; }
		if (camps[1] != null) { req.n2Hero4 = camps[1].nHeroID; }
		if (camps[2] != null) { req.n3Hero5 = camps[2].nHeroID; }
		if (camps[3] != null) { req.n4Hero6 = camps[3].nHeroID; }
		if (camps[4] != null) { req.n5Hero7 = camps[4].nHeroID; }
		
		MyGoldmineData d1 = GoldmineInstance.instance.MyGoldmine;
		if (d1.n1HeroID == req.n1Hero3 && 
			d1.n2HeroID == req.n2Hero4 && 
			d1.n3HeroID == req.n3Hero5 && 
			d1.n4HeroID == req.n4Hero6 && d1.n5HeroID == req.n5Hero7)
		{
			NGUITools.SetActive(gameObject, false);
			Destroy(gameObject);
		}
		else 
		{
			GoldmineInstance.processConfigGoldmineHeroDelegate += OnProcessConfigGoldmineHeroDelegate;
			TcpMsger.SendLogicData<CTS_GAMECMD_CONFIG_WORLDGOLDMINE_HERO_T>(req);
		}
	}
	
	void OnProcessConfigGoldmineHeroDelegate()
	{
		MyGoldmineData d1 = GoldmineInstance.instance.MyGoldmine;
		d1.n1HeroID = 0;
		d1.n2HeroID = 0;
		d1.n3HeroID = 0;
		d1.n4HeroID = 0;
		d1.n5HeroID = 0;		
		
		PickingGeneral[] camps = RoadBar.GetCGenerals();
		if (camps[0] != null) { d1.n1HeroID = camps[0].nHeroID; }
		if (camps[1] != null) { d1.n2HeroID = camps[1].nHeroID; }
		if (camps[2] != null) { d1.n3HeroID = camps[2].nHeroID; }
		if (camps[3] != null) { d1.n4HeroID = camps[3].nHeroID; }
		if (camps[4] != null) { d1.n5HeroID = camps[4].nHeroID; }
		
		// 删除 ....
		Destroy(gameObject);
	}
	
	void OnPopArmyExplainDelegate(GameObject go)
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
