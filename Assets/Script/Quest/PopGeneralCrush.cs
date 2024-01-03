using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;
// 针对PVP , RAID 的出征公共界面 ...
public class PopGeneralCrush : MonoBehaviour {
	
	public UILabel RoadTitle = null;
	public UILabel AbCol = null;
	public UILabel EnemyCol = null;
	public UILabel GoToTimeCol = null;
	public GameObject ButtonArmy = null;
	
	UIAnchor mDepth = null;
	public PopGeneralRoadBar RoadBar = null;
	
	ulong m_excel_id = 0;
	int RoadCombatType = 0;
	
	string mEnemyNameFmt;
	string mTagName = " ";
	string mGoToTimeFmt;
	
	void Awake() {
		if (AbCol != null)
		{
			AbCol.enabled = false;
		}
		if (EnemyCol != null)
		{
			mEnemyNameFmt = EnemyCol.text;
			EnemyCol.enabled = false;
		}
		if (GoToTimeCol != null)
		{
			mGoToTimeFmt = GoToTimeCol.text;
			GoToTimeCol.enabled = false;
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
		CombatManager.processCombatProfDelegate -= OnProcessFeeEnemyProfDelegate;
		CombatManager.processStartCombatDelegate -= OnProcessStartCombatDelegate;
	}
	
	// Use this for initialization
	void Start () {
	
		if (ButtonArmy != null) {
			UIEventListener.Get(ButtonArmy).onClick = OnPopArmyExplainDelegate;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
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
	
	void OnQuestFeeGeneralClose()
	{
		// 删除 ...
		Destroy(gameObject);
	}
	// 野外BOSS 出征 ...
	public void ApplyFeeExcelID(string name, uint ExcelID)
	{
		this.RoadCombatType = (int)CombatTypeEnum.COMBAT_PVE_RAID;
		this.m_excel_id = (ulong) ExcelID;
		
		//if (RoadTitle != null) {
		//	RoadTitle.text = name;
		//}
		
		Hashtable cgtMap = CommonMB.CombatGoToTimeMBInfo_Map;
		uint combatTypeID = (uint)CombatTypeEnum.COMBAT_PVE_RAID;
		if (true == cgtMap.ContainsKey(combatTypeID))
		{
			CombatGoToTimeMBInfo card = (CombatGoToTimeMBInfo) cgtMap[combatTypeID];
			ApplyGoToTime(card.GoToTime);
		}
	}
	// 掠夺玩家 出征 ...
	public void ApplyCrush(ulong nAcctID, string name)
	{
		this.RoadCombatType = (int)CombatTypeEnum.COMBAT_PVP_ATTACK;
		this.m_excel_id = (ulong) nAcctID;
		this.mTagName = name;
		
		if (EnemyCol != null) {
			EnemyCol.text = string.Format(mEnemyNameFmt, name);
			EnemyCol.enabled = true;
		}
		
		if (AbCol != null) {
			AbCol.text = U3dCmn.GetWarnErrTipFromMB(314);
			AbCol.enabled = true;
		}
	}
	// 名城 ID ...
	public void ApplyCastleID(ulong AutoID, string name)
	{
		this.RoadCombatType = (int)CombatTypeEnum.COMBAT_WORLDCITY;
		this.m_excel_id = AutoID;
		this.mTagName = name;
		
		if (EnemyCol != null) {
			EnemyCol.text = string.Format(mEnemyNameFmt, name);
			EnemyCol.enabled = true;
		}
		if (AbCol != null) {
			AbCol.text = U3dCmn.GetWarnErrTipFromMB(315);
			AbCol.enabled = true;
		}
	}
	// 世界资源 
	public void ApplyResCityID(ulong AutoID,uint Type)
	{
		this.RoadCombatType = (int)CombatTypeEnum.COMBAT_WORLD_RESOURCE;
		this.m_excel_id = AutoID;
		if (EnemyCol != null) {
			EnemyCol.text = string.Format(mEnemyNameFmt, U3dCmn.GetWarnErrTipFromMB((int)BaizVariableScript.COMBAT_TYPE_WORLD_RESOURCE));
			EnemyCol.enabled = true;
		}
		if (AbCol != null) {
			AbCol.text =  U3dCmn.GetWarnErrTipFromMB((int)BaizVariableScript.COMBAT_TYPE_WORLD_RESOURCE_DESC); 
			AbCol.enabled = true;
		}
	}
	// 显示出征把用时间 ...
	public void ApplyGoToTime(int nSecs)
	{
		int march_time  = nSecs;
		if(CommonData.player_online_info.Vip > 0)
		{
			VipRightInfo vipinfo = (VipRightInfo)CommonMB.VipRightInfo_Map[(int)CommonData.player_online_info.Vip];
			march_time = (int)(nSecs*vipinfo.SubBattleTime);
		}
		
		if (GoToTimeCol == null) return;
		
		int hour = march_time/3600;
		int minute = march_time/60%60;
		int second = march_time%60;
				
		if(hour>=100)
		{
			string cc = string.Format("{0}",hour)+":" 
				+ string.Format("{0:D2}",minute)+":" 
				+ string.Format("{0:D2}",second);
			GoToTimeCol.text = string.Format(mGoToTimeFmt, cc);
			GoToTimeCol.enabled = true;
		}
		else
		{
			string cc = string.Format("{0:D2}", hour)+":" 
				+ string.Format("{0:D2}", minute)+":" 
				+ string.Format("{0:D2}", second);
			GoToTimeCol.text = string.Format(mGoToTimeFmt, cc);
			GoToTimeCol.enabled = true;
		}
	}
	
	void OnProcessFeeEnemyProfDelegate(CombatProfData data)
	{	
		if (RoadBar != null) {
			RoadBar.ApplyCombatProfData(data);
		}
	}
	
	public void ApplyFeeEnemyPower()
	{
		CombatManager.processCombatProfDelegate += OnProcessFeeEnemyProfDelegate;
		CombatManager.RequestCombatProf( (ulong)m_excel_id, 0, 1);
	}
	
	void OnQuestFeeStartCombat()
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
		
		CTS_GAMECMD_OPERATE_START_COMBAT_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_START_COMBAT;
		req.nObjID3 = (ulong) m_excel_id;
		req.nCombatType4 = RoadCombatType;
		req.nAutoCombat5 = 0;
		req.nAutoSupply6 = 0;

		req.n1Hero7 	= 0;
		req.n2Hero8 	= 0;
		req.n3Hero9 	= 0;
		req.n4Hero10 	= 0;
		req.n5Hero11 	= 0;
		req.nStopLevel12 = 0;
		if (camps[0] != null) { req.n1Hero7 	= camps[0].nHeroID; }
		if (camps[1] != null) { req.n2Hero8 	= camps[1].nHeroID; }
		if (camps[2] != null) { req.n3Hero9 	= camps[2].nHeroID; }
		if (camps[3] != null) { req.n4Hero10 	= camps[3].nHeroID; }
		if (camps[4] != null) { req.n5Hero11 	= camps[4].nHeroID; }
		
		LoadingManager.instance.ShowLoading();
		CombatManager.processStartCombatDelegate += OnProcessStartCombatDelegate;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_START_COMBAT_T>(req);
	}

	void OnProcessStartCombatDelegate(ulong nCombatID, int nCombatType)
	{
		LoadingManager.instance.HideLoading();
		if (nCombatType != this.RoadCombatType) return;
		
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
				h1Hero.nStatus14 = (int) CMNCMD.HeroState.COMBAT_PVE_RAID; /// 状态是一样的 ...
				jlMap[id] = h1Hero;
			}
			
			comFighting.szDefenseName = mTagName;
			combatMap[nCombatID] = comFighting;
		}
		
		// 拉出军情界面 ...
		if (nCombatType == (int)CombatTypeEnum.COMBAT_PVE_RAID ||
			nCombatType == (int)CombatTypeEnum.COMBAT_PVP_ATTACK || 
			nCombatType == (int)CombatTypeEnum.COMBAT_WORLDCITY ||
			nCombatType == (int)CombatTypeEnum.COMBAT_WORLD_RESOURCE)
		{
			GameObject go = U3dCmn.GetObjFromPrefab("WarSituationWin");
			if (go != null ) {
				
				WarSituationWin win = go.GetComponent<WarSituationWin>();
				if (win != null) 
				{
					win.Depth(GetDepth());
					win.WaitingMarchListCombat();
				}
			}
		}
		
		// 填补首次出征标记 ...
		// 如果有保护圈的话, 你主动打别人, 不受保护, 移除保护圈 ...
		if (RoadCombatType == (int)CombatTypeEnum.COMBAT_PVP_ATTACK)
		{
			// PVP 世界大地图的回调, 发起攻击后要消除掉保护圈的保护 ...
			int ProtectTime = (int)CommonData.player_online_info.ProtectEndTime - DataConvert.DateTimeToInt(DateTime.Now);
			if (ProtectTime>0)
			{
				TiledStorgeCacheData.GPSRemoveProtect();
			}
		}
		
		// 新手引导战斗发起响应 ....
		if (NewbieChuZheng.processNewbieStartCombat != null)
		{
			NewbieChuZheng.processNewbieStartCombat();
			NewbieChuZheng.processNewbieStartCombat = null;
		}
		
		// 删除 ...
		Destroy(gameObject);
	}
	// 兵种说明窗口 ...
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
	
	// 新手引导 ... 
	public void TabPopPicking()
	{
		RoadBar.NewbieSetCGeneral();
	}
	
	
}
