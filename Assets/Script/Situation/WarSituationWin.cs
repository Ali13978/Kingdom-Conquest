using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CMNCMD;
using CTSCMD;
public class WarSituationWin : MonoBehaviour {

	//军情面板
	public GameObject template = null;
	public UIDraggablePanel dragPanel = null;
	public UIGrid uiGrid = null;
	public UILabel PVPMarchStatus = null; // 当前出征队列状态 ...
	
	UIAnchor mDepth = null;

	void Awake() { onEnableDelegate(); }
	void onEnableDelegate()
	{
		// 军情从战斗管理器发来 ...
		CombatManager.processMarchMsgDelegate = OnMarchMsgListCombatDelegate;
				
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
	void onExceptDelegate() {
		
		CombatManager.processMarchMsgDelegate -= OnMarchMsgListCombatDelegate;
		CombatManager.processListCombatDataDelegate -= OnListCombatDataDelegate;
		CombatManager.processListCombatDataDelegate -= OnListCombatDataDelegate;
	}
	
	// Use this for initialization
	void Start () {
		
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
	
	public float GetDepth() 
	{
		if (mDepth == null) return 0f;
		return mDepth.depthOffset;
	}
	
	//关闭窗口   ...
	void DismissPanel()
	{
		// 如果存在任务界面的话， 重新刷下任务 ...
		
		/*
		QuestLaunchWin win1 = GameObject.FindObjectOfType(typeof(QuestLaunchWin)) as QuestLaunchWin;
		if (win1 != null) 
		{
			LoadingManager.instance.ShowLoading();
			win1.RefreshQuestList();
		}
		*/
		
		// 删除自己 ...
		Destroy(gameObject);
	}
	
	public void WaitingMarchListCombat()
	{
		CombatManager.processListCombatDataDelegate = OnListCombatDataDelegate;
		CombatManager.RequestCombatListData();
	}
	public void WaitingReinforceList()
	{
		CombatManager.processListCombatDataDelegate = OnListCombatDataDelegate;
		CombatManager.ReqReinforceListData();
	}
	void OnMarchMsgListCombatDelegate()
	{
		WaitingMarchListCombat();
		
		// <新手引导> 自动返回战斗结果了 ...
		if (NewbieChuZheng.processNewbieCombatRst != null)
		{
			NewbieChuZheng.processNewbieCombatRst();
			NewbieChuZheng.processNewbieCombatRst = null;
		}
	}
	
	// 刷新的的军情卡片出征队 ...
	public void ApplyWarSituation()
	{
		if (template != null)
		{
			List<CombatDescUnit> combatList = CombatManager.instance.GetCombatList();
			
			Transform myTrans = uiGrid.transform;
			int childCount = myTrans.childCount;
			for (int i=0; i<childCount; ++ i)
			{
				Transform child = myTrans.GetChild(i);
				NGUITools.SetActive(child.gameObject, false);
				Destroy(child.gameObject);
			}
			
			int PVPNum = 0;
			for (int i=0, imax = combatList.Count; i<imax; ++ i)
			{
				CombatDescUnit card = combatList[i];
				if (card.nCombatType == (int)CombatTypeEnum.COMBAT_PVP_ATTACK || 
					card.nCombatType == (int)CombatTypeEnum.COMBAT_WORLDCITY ||
					card.nCombatType == (int)CombatTypeEnum.COMBAT_WORLD_RESOURCE)
				{
					PVPNum = PVPNum + 1;	
				}
				
				GameObject go = NGUITools.AddChild(uiGrid.gameObject,template) as GameObject;
				BaizCombatItem item = go.GetComponent<BaizCombatItem>();
				if (item != null) {
					item.ApplyBaizhanItem(card);
				}
			}
			
			// 玩家信息卡片 ... 
			int VipMarchNum = 0;
			CMN_PLAYER_CARD_INFO player = CommonData.player_online_info;
			Hashtable vipMap = CommonMB.VipRightInfo_Map;
			int VipLevel = (int)player.Vip;
			if (true == vipMap.ContainsKey(VipLevel))
			{
				VipRightInfo d2 = (VipRightInfo)vipMap[VipLevel];
				VipMarchNum = d2.MarchAddNum;
			}
			
			uint nExcelID = 7;
			Hashtable cmnMap = CommonMB.CmnDefineMBInfo_Map;
			if (true == cmnMap.ContainsKey(nExcelID))
			{
				CmnDefineMBInfo d1 = (CmnDefineMBInfo)cmnMap[nExcelID];
				VipMarchNum = VipMarchNum + (int) d1.num;
			}
			
			if (PVPMarchStatus != null)
			{
				string cc = U3dCmn.GetWarnErrTipFromMB(452);
				PVPMarchStatus.text = string.Format(cc,PVPNum,VipMarchNum);
			}
			
			//加入联盟增援数据 
			List<ReinforceUnit> ReinforceList = CombatManager.instance.ReinforceList;
			for (int i=0, imax = ReinforceList.Count; i<imax; i++)
			{
				
				ReinforceUnit unit = ReinforceList[i];
				GameObject go = NGUITools.AddChild(uiGrid.gameObject,template) as GameObject;
				BaizCombatItem item = go.GetComponent<BaizCombatItem>();
				if (item != null) {
					item.ApplyReinforceItem(unit);
				}
			}
			uiGrid.Reposition();
		}
		
	
		//uiGrid.Reposition();
	}
	
	void OnListCombatDataDelegate()
	{	
		
		//print ("yyyyyyyyyyyyyyyyyyyyyyyyyyyyy");
		ApplyWarSituation();
	}
	
	// <新手引导> 军情加速 ....
	void NewbieJiaShu()
	{
		List<CombatDescUnit> combatList = CombatManager.instance.GetCombatList();
		if (combatList.Count < 1) return;
		
		Transform myTrans = uiGrid.transform;
		Transform child = myTrans.GetChild(0);
		BaizCombatItem item = child.gameObject.GetComponent<BaizCombatItem>();
		if (item != null) {
			item.NewbieArmyAccel();
		}
	}
	
	// <新手引导> 军情加速 ....
	void NewbieJiashuRepair()
	{
		List<CombatDescUnit> combatList = CombatManager.instance.GetCombatList();
		if (combatList.Count < 1) return;
		
		Transform myTrans = uiGrid.transform;
		Transform child = myTrans.GetChild(0);
		BaizCombatItem item = child.gameObject.GetComponent<BaizCombatItem>();
		if (item != null) {
			item.NewbieArmyRepairAccel();
		}
	}
}
