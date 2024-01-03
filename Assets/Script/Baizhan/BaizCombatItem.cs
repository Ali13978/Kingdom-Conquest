using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using CMNCMD;
// 军情状态 ... 
public class BaizCombatItem : MonoBehaviour {
	
	// 敌方名字 ...
	public UILabel EnemyCol = null;
	// 意图 ...
	public UILabel IntentCol = null;
	// 君主 ...
	public UILabel LordCol = null;
	// 剩余时间 ...
	public UILabel RestTimeCol = null;
	// 状态 ...
	public UILabel StatusCol = null;
	public UILabel btnLabel = null;
	
	public GameObject Button1 = null;
	public GameObject Button2 = null;
	public CombatDescUnit combatCard = null;
	public ReinforceUnit reinforceUnit;
	
	WarSituationWin mParent = null;
	
	void OnDestroy() { onExceptDelegate(); }
	void onExceptDelegate()
	{
		CombatManager.processCombatDataDelegate -= OnProcessCombatDataDelegate;
		CombatManager.processArmyBackDelegate -= OnProcessArmyBackDelegate;
		CombatManager.processArmyAccelDelegate -= OnProcessArmyAccelDelegate;
		PopAccelatePin.processAccelDelegate -= OnProcessPopAccelatePin;
	}
	
	void Awake() {
	}
	
	// Use this for initialization
	void Start () {
		if (mParent == null) {
			mParent = NGUITools.FindInParents<WarSituationWin>(gameObject);
		}
		
		if (Button1 != null) {
			//UIEventListener.Get(Button1).onClick = OnBaizhanArmyAccel;
		}
	}
	
	// 军情观战 ...
	void OnBaizhanCombatDoor(GameObject tween)
	{
		if (combatCard == null) return;	
		if (combatCard.nStatus == 1)
		{
			// 记得清除 delegate
			CombatManager.processCombatDataDelegate = OnProcessCombatDataDelegate;
			CombatManager.instance.ApplyBaizFighting(combatCard.nCombatID);
		}
	}
	
	void OnButtonArmyBackDelegate(GameObject tween)
	{
		LoadingManager.instance.ShowLoading();
		CombatManager.processArmyBackDelegate = OnProcessArmyBackDelegate;
		CombatManager.RequestArmyBack(combatCard.nCombatID);
		PopTipDialog.instance.Dissband();
		
	}
	// 军情召回 ...
	void OnBaizhanArmyBack(GameObject tween)
	{
		if (combatCard == null) return;
		if (combatCard.nStatus == 0)
		{
			float depth = -BaizVariableScript.DEPTH_OFFSET * 4.0f;
			int Tipset = BaizVariableScript.COMBAT_MSG_ARMY_BACK_ASK;
			PopTipDialog.instance.Mode1(true,true,Tipset);
			PopTipDialog.instance.VoidButton1(OnButtonArmyBackDelegate);
			PopTipDialog.instance.Popup1(depth);
		}
	}
	// 军情加速 ...
	void OnBaizhanArmyAccel(GameObject tween)
	{
		if (combatCard == null) return;
		
		GameObject go = U3dCmn.GetObjFromPrefab("PopupAccelatePin");
		if (go == null) return;
		
		PopAccelatePin.processAccelDelegate -= OnProcessPopAccelatePin;
		PopAccelatePin.processAccelDelegate += OnProcessPopAccelatePin;
		PopAccelatePin win = go.GetComponent<PopAccelatePin>();
		if (win != null)
		{
			float depth = -BaizVariableScript.DEPTH_OFFSET * 4.0f;
			win.Depth(depth);
			win.SetAccel(combatCard.endTimeSec);
		}
	}
	
	// 保存当前加速的时刻, 还剩下多少秒 ...
	void OnProcessPopAccelatePin(int money_type, int nTime)
	{		
		PopAccelatePin.processAccelDelegate -= OnProcessPopAccelatePin;
		
		LoadingManager.instance.ShowLoading();
		combatCard.nRestTime2 = combatCard.endTimeSec - DataConvert.DateTimeToInt(DateTime.Now);
		CombatManager.processArmyAccelDelegate = OnProcessArmyAccelDelegate;
		CombatManager.RequestArmyAccel(combatCard.nCombatID, money_type, nTime);
	}
	
	void OnProcessArmyAccelDelegate(ulong nCombatID, int nCost, byte money_type)
	{		
		// 解除 ...
		LoadingManager.instance.HideLoading();
		
		// 加速返回处理 ...
		if (nCombatID == combatCard.nCombatID)
		{
			Hashtable accelMap = CommonMB.CmnDefineMBInfo_Map;
			uint idAccel = 2;
			int AccelCostUnit = 0;
			if (true == accelMap.ContainsKey(idAccel))
			{
				CmnDefineMBInfo info = (CmnDefineMBInfo) accelMap[idAccel];
				AccelCostUnit = (int) info.num;
			}
				
			if (money_type == (byte)CMNCMD.money_type.money_type_crystal)
			{
				CommonData.player_online_info.Crystal -= (uint) nCost;
				PlayerInfoManager.RefreshPlayerDataUI();
			}
			else if (money_type == (byte) CMNCMD.money_type.money_type_diamond)
			{
				CommonData.player_online_info.Diamond -= (uint) nCost;
				PlayerInfoManager.RefreshPlayerDataUI();
			}
			
			// 获取当前还剩下多少时间 ... 
			combatCard.nRestTime = Mathf.Max(0,(combatCard.nRestTime2 - nCost * AccelCostUnit));
			if (combatCard.nRestTime>0)
			{
				combatCard.endTimeSec = combatCard.nRestTime + DataConvert.DateTimeToInt(DateTime.Now);
				CombatManager.instance.ModifyCombatData(combatCard);
				Begin(combatCard.endTimeSec);
			}
			else 
			{
				RestTimeCol.enabled = false;
				StopCoroutine("BaizCooldown");
				
				if (Button1 != null) {
					NGUITools.SetActive(Button1, false);
				}
		
				if (Button2 != null) {
					NGUITools.SetActive(Button2, false);
				}
				CombatManager.instance.ModifyCombatData(combatCard);
			}
		}
		
		// 新手引导 军情加速 ...
		if (NewbieChuZheng.processNewbieArmyAccel != null)
		{
			NewbieChuZheng.processNewbieArmyAccel();
			NewbieChuZheng.processNewbieArmyAccel = null;
		}
		
		// 加速完成了... 
		U3dCmn.SendMessage("PopupAccelatePin", "DismissPanel", null);
	}
	
	void OnProcessArmyBackDelegate(ulong nCombatID, int nBackTime)
	{
		LoadingManager.instance.HideLoading();
		if (combatCard.nCombatID != nCombatID) return;
		
		combatCard.nCombatID = nCombatID;
		combatCard.nStatus = 2;
		combatCard.nRestTime = nBackTime;
		combatCard.endTimeSec = nBackTime + DataConvert.DateTimeToInt(DateTime.Now);
		
		if (this.StatusCol != null)
		{
			int Tipset = GetCombatStatus(combatCard.nStatus);
			StatusCol.text = U3dCmn.GetWarnErrTipFromMB(Tipset);
		}
		
		if (Button2 != null) {
			NGUITools.SetActive(Button2, false);
		}
		CombatManager.instance.ModifyCombatData(combatCard);
		// 倒计时 ...
		Begin(combatCard.endTimeSec);
	}
	
	void OnProcessCombatDataDelegate(ulong nCombatID)
	{
		GameObject go = U3dCmn.GetObjFromPrefab("BaizBattlefieldWin");
		if (go == null) return;
		
		BattlefieldWin win = go.GetComponent<BattlefieldWin>();
		if (win != null)
		{
			float depth = -BaizVariableScript.DEPTH_OFFSET * 4f;
			win.Depth(depth);
		}
	}
	// 根据战斗类型选择名字描述...
	int GetCombatPlan(int combatType)
	{
		int Tipset = 0;
		
		switch (combatType)
		{
		case (int)CombatTypeEnum.COMBAT_PVE_RAID:
		case (int)CombatTypeEnum.COMBAT_INSTANCE_BAIZHANBUDAI:
		case (int)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN:
			{
				Tipset = BaizVariableScript.COMBAT_PLAN_PVE;
			} break;
		case (int)CombatTypeEnum.COMBAT_PVE_WORLDGOLDMINE:
		case (int)CombatTypeEnum.COMBAT_PVP_WORLDGOLDMINE_ATTACK:
		case (int)CombatTypeEnum.COMBAT_WORLD_RESOURCE:
		case (int)CombatTypeEnum.COMBAT_WORLDCITY:
			{
				Tipset = BaizVariableScript.COMBAT_PLAN_LOOT;
			} break;
		case (int)CombatTypeEnum.COMBAT_PVP_ATTACK:
			{
				Tipset = BaizVariableScript.COMBAT_PLAN_PVP;
			} break;
			
		case (int)CombatTypeEnum.COMBAT_PVP_DEFENSE:
		case (int)CombatTypeEnum.COMBAT_PVP_WORLDGOLDMINE_DEFENSE:
			{
				Tipset = BaizVariableScript.COMBAT_PLAN_DEFENSE;
			} break;
		default:
			{
				Tipset = BaizVariableScript.COMBAT_PLAN_PVE;
			} break;
		}
		
		return Tipset;
	}
	// 当前军情状态 ...
	int GetCombatStatus(int nStatus)
	{
		int Tipset = 0;
		if (nStatus == 1)
		{
			Tipset = BaizVariableScript.COMBAT_STATUS_BACKING;
		}
		else if (nStatus == 2)
		{
			Tipset = BaizVariableScript.COMBAT_OP_ARMY_BACKING;
		}
		else
		{
			Tipset = BaizVariableScript.COMBAT_STATUS_MARCH;
		}
		
		return Tipset;
	}
	
	
	public void ApplyBaizhanItem(CombatDescUnit newCard)
	{
		UIEventListener.Get(Button1).onClick = OnBaizhanArmyAccel;
		this.combatCard = newCard;
		
		int Tipset = 0;
		if (IntentCol != null) {
			Tipset = GetCombatPlan(newCard.nCombatType);
			IntentCol.text = U3dCmn.GetWarnErrTipFromMB(Tipset);
		}
		
		if (StatusCol != null) {
			Tipset = GetCombatStatus(newCard.nStatus);
			StatusCol.text = U3dCmn.GetWarnErrTipFromMB(Tipset);
		}
		// 如果是防御状态 君主填攻击方名字, 目的地填写防卫方名字...
		if (newCard.nCombatType == (int)CombatTypeEnum.COMBAT_PVP_DEFENSE)
		{
			if (LordCol != null)
			{
				LordCol.text = newCard.EnemyName; // 
			}
			if (EnemyCol != null) 
			{
				EnemyCol.text = CommonData.player_online_info.CharName;
			}
		}
		else  // 其他通用吧 ...
		{
			if (LordCol != null) {
				LordCol.text = CommonData.player_online_info.CharName;
			}	
			if (EnemyCol != null) {
				EnemyCol.text = newCard.EnemyName;
			}
		}

		if (RestTimeCol != null)
		{					
			int endTimeSec = newCard.endTimeSec;//DataConvert.DateTimeToInt(DateTime.Now) + Mathf.Max(1,newCard);
			Begin(endTimeSec);
		}
		
		int combatType = newCard.nCombatType;
		if (combatType == (int)CombatTypeEnum.COMBAT_INSTANCE_BAIZHANBUDAI ||
			combatType == (int)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN ||
			combatType == (int)CombatTypeEnum.COMBAT_PVE_WORLDGOLDMINE ||
			combatType == (int)CombatTypeEnum.COMBAT_PVP_WORLDGOLDMINE_ATTACK ||
			combatType == (int)CombatTypeEnum.COMBAT_PVP_WORLDGOLDMINE_DEFENSE ||
			combatType == (int)CombatTypeEnum.COMBAT_PVP_DEFENSE)
		{
			if (Button1 != null) {
				NGUITools.SetActive(Button1, false);
			}
			
			if (newCard.nStatus == 1) {
				UIEventListener.Get(Button2).onClick = OnBaizhanCombatDoor;
				Tipset = BaizVariableScript.COMBAT_OP_ARMY_GUANZHAN;
				btnLabel.text = U3dCmn.GetWarnErrTipFromMB(Tipset);
			}
			else 
			{	
				NGUITools.SetActive(Button2, false);
			}
		}
		else 
		{
			if (newCard.nStatus == 1) {
				UIEventListener.Get(Button2).onClick = OnBaizhanCombatDoor;
				Tipset = BaizVariableScript.COMBAT_OP_ARMY_GUANZHAN;
				btnLabel.text = U3dCmn.GetWarnErrTipFromMB(Tipset);
			}
			else if (newCard.nStatus == 2) {
				NGUITools.SetActive(Button2, false);
			}
			else 
			{
				UIEventListener.Get(Button2).onClick = OnBaizhanArmyBack;
				Tipset = BaizVariableScript.COMBAT_OP_ARMY_BACK;
				btnLabel.text = U3dCmn.GetWarnErrTipFromMB(Tipset);
			}
		}
	}
	//增援 
	public void ApplyReinforceItem(ReinforceUnit unit)
	{
		reinforceUnit = unit;
		if (IntentCol != null) {
			IntentCol.text =  U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.REINFORCE_TITLE);
		}
		
		if (StatusCol != null) {
			if(unit.Type == 1)
			{
				StatusCol.text = U3dCmn.GetWarnErrTipFromMB((int)BaizVariableScript.COMBAT_STATUS_MARCH);
				Button2.gameObject.SetActiveRecursively(true);
				UIEventListener.Get(Button2).onClick = OnReinforceBack;
			}
			else if(unit.Type == 2)
			{
				Button2.gameObject.SetActiveRecursively(false);
				StatusCol.text = U3dCmn.GetWarnErrTipFromMB((int)BaizVariableScript.COMBAT_OP_ARMY_BACKING);
			}
				
				
		}
			
		if(unit.DstType == 1)
		{
			LordCol.text = CommonData.player_online_info.CharName; // 
			EnemyCol.text = unit.CharName;
		}
		else if(unit.DstType == 2)
		{
			LordCol.text = unit.CharName; // 
			EnemyCol.text =  CommonData.player_online_info.CharName; 
		}
		
		

		if (RestTimeCol != null)
		{					
			//int endTimeSec = DataConvert.DateTimeToInt(DateTime.Now) + Mathf.Max(1,100);
			StopCoroutine("ReinforceCooldown");
			StartCoroutine("ReinforceCooldown",unit.EndTime);
			//Begin(endTimeSec);
		}	
			
		
	
		int Tipset = BaizVariableScript.COMBAT_OP_ARMY_BACK;//BaizVariableScript.COMBAT_OP_ARMY_GUANZHAN;
		btnLabel.text = U3dCmn.GetWarnErrTipFromMB(Tipset);;//U3dCmn.GetWarnErrTipFromMB(Tipset);
		//UIEventListener.Get(Button1).onClick -= OnBaizhanArmyAccel;
		UIEventListener.Get(Button1).onClick = OnReinforceAccel;	
		if(reinforceUnit.DstType == 2) 
		{
			NGUITools.SetActive(Button1, false);
			NGUITools.SetActive(Button2, false);
		}
		//NGUITools.SetActive(Button2, false);
		
		
	}
	// 增援加速 ... 
	void OnReinforceAccel(GameObject tween)
	{
		
		//if (combatCard == null) return;
		
		GameObject go = U3dCmn.GetObjFromPrefab("PopupAccelatePin");
		if (go == null) return;
		
		PopAccelatePin.processAccelDelegate -= OnReinforceAccelRst;
		PopAccelatePin.processAccelDelegate += OnReinforceAccelRst;
		PopAccelatePin win = go.GetComponent<PopAccelatePin>();
		
		if (win != null)
		{
			float depth = -BaizVariableScript.DEPTH_OFFSET * 4.0f;
			win.Depth(depth);
			int sec = reinforceUnit.EndTime;
			win.SetAccel(sec);
		}
	}
	// 增援加速返回 
	void OnReinforceAccelRst(int money_type, int nTime)
	{		
		PopAccelatePin.processAccelDelegate -= OnProcessPopAccelatePin;
		
		LoadingManager.instance.ShowLoading();
		//combatCard.nRestTime2 = combatCard.endTimeSec - DataConvert.DateTimeToInt(DateTime.Now);
		CombatManager.processReinforceAccelDelegate = OnProcessReinforceAccelDelegate;
		CombatManager.RequestReinforceAccel(reinforceUnit.EventID, (uint)money_type, (uint)nTime);	
		
		// 加速完成了... 
	}
	void OnProcessReinforceAccelDelegate(ulong nEventID, uint nCost, uint money_type)
	{		
		// 解除 ...
		LoadingManager.instance.HideLoading();
		
		// 加速返回处理 ...
		if (nEventID == reinforceUnit.EventID)
		{
			Hashtable accelMap = CommonMB.CmnDefineMBInfo_Map;
			uint idAccel = 2;
			int AccelCostUnit = 0;
			if (true == accelMap.ContainsKey(idAccel))
			{
				CmnDefineMBInfo info = (CmnDefineMBInfo) accelMap[idAccel];
				AccelCostUnit = (int) info.num;
			}
				
			if (money_type == (byte)CMNCMD.money_type.money_type_crystal)
			{
				CommonData.player_online_info.Crystal -= (uint) nCost;
				PlayerInfoManager.RefreshPlayerDataUI();
			}
			else if (money_type == (byte) CMNCMD.money_type.money_type_diamond)
			{
				CommonData.player_online_info.Diamond -= (uint) nCost;
				PlayerInfoManager.RefreshPlayerDataUI();
			}
			
			// 获取当前还剩下多少时间 ... 
			int nRestTime = reinforceUnit.EndTime - DataConvert.DateTimeToInt(DateTime.Now);
			//print ("eeeeeeeeeeeeeeee"+nRestTime + " "+(int)nCost * AccelCostUnit);
			nRestTime = Mathf.Max(0,(nRestTime - (int)nCost * AccelCostUnit));
			//print ("oooooooooooo"+nRestTime);
			if (nRestTime>0)
			{
				reinforceUnit.EndTime = nRestTime + DataConvert.DateTimeToInt(DateTime.Now);
				StopCoroutine("ReinforceCooldown");
				StartCoroutine("ReinforceCooldown",reinforceUnit.EndTime);
				CombatManager.instance.ModifyReinforceData(reinforceUnit.EventID,reinforceUnit);
			}
			else 
			{
				RestTimeCol.enabled = false;
				StopCoroutine("ReinforceCooldown");
				
				if (Button1 != null) {
					NGUITools.SetActive(Button1, false);
				}
		
				if (Button2 != null) {
					NGUITools.SetActive(Button2, false);
				}
				
				//List<ReinforceUnit> reinforceList = CombatManager.instance.ReinforceList;
				CombatManager.instance.DeleteReinforceData(reinforceUnit.EventID);
				this.OnListCombatDataDelegate(); 
			}
		}
		
		// 加速完成了... 
		U3dCmn.SendMessage("PopupAccelatePin", "DismissPanel", null);
	}
	// 增援召回 ... 
	void OnReinforceBack(GameObject tween)
	{
		if (reinforceUnit.EventID == 0) return;
		if (reinforceUnit.DstType == 1 &&  reinforceUnit.Type == 1)
		{
			float depth = -BaizVariableScript.DEPTH_OFFSET * 4.0f;
			int Tipset = (int)MB_WARN_ERR.RECALL_REINFORCEMENTS;
			PopTipDialog.instance.Mode1(true,true,Tipset);
			PopTipDialog.instance.VoidButton1(OnButtonReinforceBackDelegate);
			PopTipDialog.instance.Popup1(depth);
		}
	}
	void OnButtonReinforceBackDelegate(GameObject tween)
	{
		LoadingManager.instance.ShowLoading();
		CombatManager.processReinforceBackDelegate = OnProcessReinforceBackDelegate;
		CombatManager.RequestReinforceBack(reinforceUnit.EventID);
		PopTipDialog.instance.Dissband();
		
	}
	//增援召回返回处理 
	void OnProcessReinforceBackDelegate(ulong NewEventID,ulong OldEventID,int nBackTime)
	{
		LoadingManager.instance.HideLoading();
		if (reinforceUnit.EventID != OldEventID) return;

		reinforceUnit.EventID = NewEventID;
		reinforceUnit.Type = 2;

		reinforceUnit.EndTime = nBackTime + DataConvert.DateTimeToInt(DateTime.Now);
		
		CombatManager.instance.ModifyReinforceData(OldEventID,reinforceUnit);
		if (this.StatusCol != null)
		{
			if(reinforceUnit.Type == 1)
				StatusCol.text = U3dCmn.GetWarnErrTipFromMB((int)BaizVariableScript.COMBAT_STATUS_MARCH);
			else if(reinforceUnit.Type == 2)
				StatusCol.text = U3dCmn.GetWarnErrTipFromMB((int)BaizVariableScript.COMBAT_OP_ARMY_BACKING);
		}
		
		if (Button2 != null) {
			NGUITools.SetActive(Button2, false);
		}
		
		StopCoroutine("ReinforceCooldown");
		StartCoroutine("ReinforceCooldown",reinforceUnit.EndTime);
	}
	//战斗Cooldown   
	IEnumerator BaizCooldown(int endTimeSec)
	{
		if (RestTimeCol != null)
		{
			RestTimeCol.enabled = true;
			int nSecs = (endTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
			if (nSecs <0) nSecs = 0;
			
			while(nSecs > 0)
			{
				nSecs = endTimeSec - DataConvert.DateTimeToInt(DateTime.Now);
				if(nSecs <0)
					nSecs = 0;
				
				int hour = nSecs/3600;
				int minute = nSecs/60%60;
				int second = nSecs%60;
				
				if(hour>=100)
				{
					RestTimeCol.text = string.Format("{0}",hour)+":"
							+ string.Format("{0:D2}",minute)+":" 
							+ string.Format("{0:D2}",second);
					
				}
				else
				{
					RestTimeCol.text =string.Format("{0:D2}", hour)+":"
							+ string.Format("{0:D2}", minute)+":"
							+ string.Format("{0:D2}", second);
				}
				
				yield return new WaitForSeconds(1);
			}
			
			RestTimeCol.enabled = false;
		}
		// 请求军情列表 ... 
		if (combatCard.nCombatType == (int)CombatTypeEnum.COMBAT_PVP_DEFENSE && 
			combatCard.nStatus == 2 )
		{
			List<CombatDescUnit> combatList = CombatManager.instance.GetCombatList();
			combatList.Remove(combatCard);			
			this.OnListCombatDataDelegate();
		}
	}
	//增援Cooldown   
	IEnumerator ReinforceCooldown(int endTimeSec)
	{
		if (RestTimeCol != null)
		{
			RestTimeCol.enabled = true;
			int nSecs = (endTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
			
			if (nSecs <0) nSecs = 0;
			while(nSecs > 0)
			{
				nSecs = endTimeSec - DataConvert.DateTimeToInt(DateTime.Now);
				if(nSecs <0)
					nSecs = 0;
				
				int hour = nSecs/3600;
				int minute = nSecs/60%60;
				int second = nSecs%60;
				
				if(hour>=100)
				{
					RestTimeCol.text = string.Format("{0}",hour)+":"
							+ string.Format("{0:D2}",minute)+":" 
							+ string.Format("{0:D2}",second);
					
				}
				else
				{
					RestTimeCol.text =string.Format("{0:D2}", hour)+":"
							+ string.Format("{0:D2}", minute)+":"
							+ string.Format("{0:D2}", second);
				}
				
				yield return new WaitForSeconds(1);
			}
			
			RestTimeCol.enabled = false;
		}
		//print ("dddddddddddddddddddddddddddddddddd");
		List<ReinforceUnit> reinforceList = CombatManager.instance.ReinforceList;
		reinforceList.Remove(reinforceUnit);			
		this.OnListCombatDataDelegate(); 
		
		// 请求增援列表 ... 
		/*if (combatCard.nCombatType == (int)CombatTypeEnum.COMBAT_PVP_DEFENSE && 
			combatCard.nStatus == 2 )
		{
			List<CombatDescUnit> combatList = CombatManager.instance.GetCombatList();
			combatList.Remove(combatCard);			
			this.OnListCombatDataDelegate();
		}*/
	}
	void OnListCombatDataDelegate()
	{
		if (mParent == null) return;
		mParent.ApplyWarSituation();
	}
	
	public void Begin(int endTimeSec)
	{
		StopCoroutine("BaizCooldown");
		StartCoroutine("BaizCooldown", endTimeSec);
	}
	
	// 新手加速 ...
	public void NewbieArmyAccel()
	{
		OnBaizhanArmyAccel(Button1);
	}
	
	// <新手引导> 重连状态下, 重新加速 ...
	public void NewbieArmyRepairAccel()
	{
		PopAccelatePin win1 = GameObject.FindObjectOfType(typeof(PopAccelatePin)) as PopAccelatePin;
		if (win1 != null)
		{
			PopAccelatePin.processAccelDelegate -= OnProcessPopAccelatePin;
			PopAccelatePin.processAccelDelegate += OnProcessPopAccelatePin;
			win1.SetAccel(combatCard.endTimeSec);
		}
	}
	
	
}
