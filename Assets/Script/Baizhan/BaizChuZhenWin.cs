using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class BaizChuZhenWin : MonoBehaviour {
	
	public PopGeneralRoadBar RoadBar = null;
	BaizhanCampWin mCScene = null;
	
	public GameObject PassedAtStop = null;
	public GameObject ButtonArmy = null;
	
	// 界面深度设置 
	UIAnchor mDepth = null;
	int mBaizAutoSupply = 0;
	int m_id = 1;
	
	void Awake() {
		
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
		
		CombatManager.processStartCombatDelegate -= OnAutoStartCombatDelegate;
	}
	
	// Use this for initialization
	void Start () 
	{
		if (ButtonArmy != null)
		{
			UIEventListener.Get(ButtonArmy).onClick = OnPopArmyExplainDelegate;
		}
	}
	
	public int idGuard 
	{
		get {
			return m_id;
		}
		set {
			m_id = value;
		}
	}
	
	public void SetCScene(BaizhanCampWin obj)
	{
		mCScene = obj;
	}
	
	/// <summary>
	/// Depth the specified depth.
	/// </summary>
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
	
	public void GetBaizhanGenerals()
	{
		if (RoadBar == null) return;
		ulong nInstanceID = BaizhanInstance.instance.idInstance;
		RoadBar.SetCGenerals(nInstanceID);
	}
	
	void OnBaizChuZhengClose()
	{		
		NGUITools.SetActive(gameObject,false);
		Destroy(gameObject);
	}
	
	void OnActivateAutoSupply(bool isChecked)
	{
		if (isChecked == true) 
		{
			mBaizAutoSupply = 1;
		}
		else 
		{
			mBaizAutoSupply = 0;
		}
	}
	
	void OnBaizhanAutoStartCombat()
	{		
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
		
		int nStopLevel = 100;
		if (PassedAtStop != null) {
			UIInput inChar = PassedAtStop.GetComponent<UIInput>();
			int.TryParse(inChar.label.text, out nStopLevel);
			nStopLevel = Mathf.Max(m_id,Mathf.Min(100,nStopLevel));
		}
					
		// 开始战斗 ....
		CTS_GAMECMD_OPERATE_START_COMBAT_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_START_COMBAT;
		req.nCombatType4 = (int)CombatTypeEnum.COMBAT_INSTANCE_BAIZHANBUDAI;
		req.nAutoCombat5 = 1;
		req.nAutoSupply6 = mBaizAutoSupply;
		req.nObjID3 = 0;
		req.n1Hero7 = 0;
		req.n2Hero8 = 0;
		req.n3Hero9 = 0;
		req.n4Hero10 = 0;
		req.n5Hero11 = 0;
		req.nStopLevel12 = nStopLevel;
		
		if (camps[0] != null) { req.n1Hero7 = camps[0].nHeroID; }
		if (camps[1] != null) { req.n2Hero8 = camps[1].nHeroID; }
		if (camps[2] != null) { req.n3Hero9 = camps[2].nHeroID; }
		if (camps[3] != null) { req.n4Hero10 = camps[3].nHeroID; }
		if (camps[4] != null) { req.n5Hero11 = camps[4].nHeroID; }
		
		LoadingManager.instance.ShowLoading();
		CombatManager.processStartCombatDelegate += OnAutoStartCombatDelegate;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_START_COMBAT_T>(req);
	}
	
	// 自动战斗返回处理 ...
	void OnAutoStartCombatDelegate(ulong nCombatID, int nCombatType)
	{
		LoadingManager.instance.HideLoading();
		if (nCombatType != (int)CombatTypeEnum.COMBAT_INSTANCE_BAIZHANBUDAI) return;
		
		if (mCScene != null) 
		{
			mCScene.SetBaizAutoCombat(true);
			mCScene.SetCGuardOnDoor(idGuard);
		}
		
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
				h1Hero.nStatus14 = (int) CMNCMD.HeroState.COMBAT_INSTANCE_BAIZHANBUDAI;
				jlMap[id] = h1Hero;
			}
			
			combatMap[nCombatID] = comFighting;
		}
		
		NGUITools.SetActive(gameObject,false);
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