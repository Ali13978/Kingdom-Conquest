using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using STCCMD;
using CTSCMD;
using CMNCMD;
public class NanBeizhanAutoCombatWin : MonoBehaviour {
	
	public UILabel PrettyHeroPowerCol = null;
	public GameObject PassedAtStop = null;
	
	public int spacing = 80;
	public GameObject RoadGeneralCol = null;
	public GameObject template = null;
	
	NanBeizhanScene mCScene = null;
	PickingGeneralItem[] RoadCol = new PickingGeneralItem[5];
	PickingGeneral[] camps = new PickingGeneral[5];
	
	public GameObject ButtonPicking = null;
	public GameObject ButtonArmy = null;
	
	// 界面深度设置 
	UIAnchor mDepth = null;
	int mNanBeizAutoSupply = 0;
	int m_id = 1;
	
	string mPrettyHeroPower;
	
	void Awake() 
	{
		if (PrettyHeroPowerCol != null)
		{
			mPrettyHeroPower = PrettyHeroPowerCol.text;
			PrettyHeroPowerCol.enabled = false;
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
		BaizGeneralRoadWin.applyBaizCampDataDelegate  = null;
		CombatManager.processStartCombatDelegate -= OnNanBeizhanAutoStartCombatDelegate;	
		BaizInstanceManager.processSupplyInstanceDelegate -= OnProcessManualSupplyInstanceDelegate;
	}
	
	// Use this for initialization
	void Start () {
		if (ButtonPicking != null) {
			UIEventListener.Get (ButtonPicking).onClick = OnPopGeneralsPicking;
		}
		if (ButtonArmy != null) {
			UIEventListener.Get (ButtonArmy).onClick = OnPopArmyExplainDelegate;
		}
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
		get {
			return m_id;
		}
		set {
			m_id = value;
		}
	}
	
	public void SetCScene(NanBeizhanScene obj)
	{
		mCScene = obj;
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
	
	void OnNanBeizhanAutoStartCombat()
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
		
		int nStopLevel = 20;
		if (PassedAtStop != null) {
			UIInput inChar = PassedAtStop.GetComponent<UIInput>();
			int.TryParse(inChar.label.text, out nStopLevel);
			nStopLevel = Mathf.Max(m_id,Mathf.Min(20,nStopLevel));
		}
		
		// 开启战斗 ...
		CTS_GAMECMD_OPERATE_START_COMBAT_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_START_COMBAT;
		req.nCombatType4 = 101;
		req.nAutoCombat5 = 1;
		req.nAutoSupply6 = mNanBeizAutoSupply;
		req.nObjID3 	= 0;
		req.n1Hero7 	= 0;
		req.n2Hero8 	= 0;
		req.n3Hero9 	= 0;
		req.n4Hero10 	= 0;
		req.n5Hero11 	= 0;
		req.nStopLevel12 = nStopLevel;
		
		if (camps[0] != null) { req.n1Hero7 = camps[0].nHeroID; }
		if (camps[1] != null) { req.n2Hero8 = camps[1].nHeroID; }
		if (camps[2] != null) { req.n3Hero9 = camps[2].nHeroID; }
		if (camps[3] != null) { req.n4Hero10 = camps[3].nHeroID; }
		if (camps[4] != null) { req.n5Hero11 = camps[4].nHeroID; }
		
		LoadingManager.instance.ShowLoading();
		CombatManager.processStartCombatDelegate += OnNanBeizhanAutoStartCombatDelegate;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_START_COMBAT_T>(req);
	}
	
	void OnNanBeizhanAutoStartCombatDelegate(ulong nCombatID, int nCombatType)
	{
		LoadingManager.instance.HideLoading();
		if (nCombatType != (int)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN) return;
		
		if (mCScene != null) 
		{
			mCScene.SetCGuardAutoCombat(true);
			mCScene.SetCGuardOnDoor(idGuard);
			mCScene.DisableKicking();
		}
		
		CombatFighting comFighting = null;
		SortedList<ulong,CombatFighting> combatMap = CombatManager.instance.GetCombatFightingMap();
		if (true == combatMap.TryGetValue(nCombatID, out comFighting))
		{
			ulong[] nHeroArray = new ulong[5];
			
			int imax = 0;
			if (camps[0] != null) { nHeroArray[0] = camps[0].nHeroID; imax++; }
			if (camps[1] != null) { nHeroArray[1] = camps[1].nHeroID; imax++; }
			if (camps[2] != null) { nHeroArray[2] = camps[2].nHeroID; imax++; }
			if (camps[3] != null) { nHeroArray[3] = camps[3].nHeroID; imax++; }
			if (camps[4] != null) { nHeroArray[4] = camps[4].nHeroID; imax++; }
		
			Hashtable jlMap = JiangLingManager.MyHeroMap;
			for (int i=0; i<imax; ++i)
			{
				ulong id = nHeroArray[i];
				if (true == jlMap.ContainsKey(id))
				{
					HireHero h1Hero = (HireHero)jlMap[id];
					h1Hero.nStatus14 = (int) CMNCMD.HeroState.COMBAT_INSTANCE_NANZHENGBEIZHAN;
					jlMap[id] = h1Hero;
				}
			}
			
			combatMap[nCombatID] = comFighting;
		}
		
		// 删除 ...
		Destroy(gameObject);
	}
	
	void ResetRoadGeneralCon1()
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
	
	public void SetNanBeizhanInstanceData1(BaizInstanceGroup newGroup)
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
	
	void OnActivateAutoSupply(bool isChecked)
	{
		if (isChecked == true) 
		{
			mNanBeizAutoSupply = 1;
		}
		else 
		{
			mNanBeizAutoSupply = 0;
		}
	}
	
	void OnNanBeizhanAutoCombatClose()
	{
		NGUITools.SetActive(gameObject,false);
		Destroy(gameObject);
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
		
		if (PrettyHeroPowerCol != null)
		{
			PrettyHeroPowerCol.text = string.Format(mPrettyHeroPower,totalProf);
		}
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
