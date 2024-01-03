using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class NanBeizhanLaunchWin : MonoBehaviour {
	
	
	public UILabel DayTimesFreeCol = null;
	public UILabel DayTimesFeeCol = null;
	
	public Transform TopBar = null;
	public Transform BottomBar = null;
	
	string mPrettyFree;
	string mPrettyFee;
	
	int _prettyStatus = -1;
	int _classID = 0;
	
	UIAnchor mDepth = null;
	
	public NanBeizhanDiTu DayDiTu = null;
	
	void Awake() {
				
		// IPAD 适配尺寸 ...
		float OffsetY = 0f;
		UIRoot root = GetComponent<UIRoot>();
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			if (root != null) { root.manualHeight = 320; }
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			OffsetY = 20f;
			if (root != null) { root.manualHeight = 360; }
		}
		
		if (TopBar != null) {
			TopBar.localPosition += new Vector3(0f,OffsetY,0f);
		}
		if (BottomBar != null) {
			TopBar.localPosition -= new Vector3(0f,OffsetY,0f);
		}
		
		if (DayTimesFreeCol != null)
		{
			mPrettyFree = DayTimesFreeCol.text;
			DayTimesFreeCol.text = string.Format(mPrettyFree,0,10);
		}
		if (DayTimesFeeCol != null)
		{
			mPrettyFee = DayTimesFeeCol.text;
			DayTimesFeeCol.text = string.Format(mPrettyFee,0,10);
		}
	}
	
	void OnDestroy() { onExceptDelegate(); }
	void onExceptDelegate() 
	{
		//print("- onExceptDelegate");
		CombatManager.processListCombatDataDelegate -= OnProcessListCombatDataDelegate;
		// 副本描述 ...
		BaizInstanceManager.processGetInstanceDescDelegate -= OnProcessGetInstanceDescDelegate;
		BaizInstanceManager.processInstanceDataDelegate -= OnProcessGetInstanceDataDelegate;
	}
	
	// Use this for initialization
	void Start () {
		
		// iphone5 挪来 ...
		KeepOut.instance.ShowKeepOut();
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
	
	public void OnNanBeizhanLaunchClose()
	{
		// iphone5 挪开 ...
		KeepOut.instance.HideKeepOut();
		
		// 删除 ...
		Destroy(gameObject);
		
		// 解除资源 ...
		Resources.UnloadUnusedAssets();
	}
	
	public void DoLaunchClose()
	{
		NGUITools.SetActive(gameObject, false);
		Destroy(gameObject);
	}
	
	public void PopupNanBeizhanHall(int nClassID)
	{
		LoadingManager.instance.ShowLoading();
		NanBeizhanInstance.instance.idClass = nClassID;
		BaizInstanceManager.processGetInstanceDescDelegate = OnProcessGetInstanceDescDelegate;
		BaizInstanceManager.RequestInstanceDescData(101, nClassID);
	}
	
	void OnProcessGetInstanceDescDelegate()
	{
		LoadingManager.instance.HideLoading();
		
		GameObject go = U3dCmn.GetObjFromPrefab("NanBeizhanHallWin");
		if (go == null) return;
		
		NanBeizhanHall t = go.GetComponent<NanBeizhanHall>();
		if (t != null)
		{
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			t.Depth(depth);
			t.ApplyInstanceDescList();
		
			if (this._prettyStatus == 0)
			{
				this._prettyStatus = -1; // 清除状态   
				t.PopupNanBeizhanRoom();
			}
		}
	}
		
	void OnProcessGetInstanceDataDelegate(BaizInstanceGroup newGroup)
	{
		LoadingManager.instance.HideLoading();
		NanBeizhanInstance.instance.ApplyInstanceGroupGeneral(newGroup);
		
		// 直接进入战斗场景  newGroup
		GameObject go = U3dCmn.GetObjFromPrefab("NanBeizhanScene");
		if (go == null) return;
		
		bool isLeaderOrNo = NanBeizhanInstance.instance.IsInstanceCreator();
		NanBeizhanScene t = go.GetComponent<NanBeizhanScene>();
		if (t != null)
		{
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			t.Depth(depth);
			t.ApplyLeaderOrNo(isLeaderOrNo);
			t.ApplyInstanceGroup(newGroup);
		}
		
		// 删除界面   Lanuch
		NGUITools.SetActive(gameObject,false);
		Destroy(gameObject);
	}
	
	public void ApplyCDiTuLevel(int nLevel)
	{
		if (DayDiTu != null) {
			DayDiTu.SetCDiTuCar(nLevel);
		}
	}
	public void RefreshUI()
	{
		BaizInstanceStatus card = BaizInstanceManager.instance.TryGetInstanceStatus(101);
		if (card == null) return;
		
		NanBeizhanInstance.instance.idInstance = card.nInstanceID;
		
		int d1 = card.nDayTimesFee - card.nResDayTimesFee;
		int d2 = card.nDayTimesFree - card.nResDayTimesFree;
		NanBeizhanInstance.instance.idRestDayTimesFee = d1;
		NanBeizhanInstance.instance.idRestDayTimesFree = d2;
				

		if (DayTimesFreeCol != null)
		{
			DayTimesFreeCol.text = string.Format(mPrettyFree,card.nResDayTimesFree,card.nDayTimesFree);
		}
		if (DayTimesFeeCol != null)
		{
			DayTimesFeeCol.text = string.Format(mPrettyFee, card.nResDayTimesFee, card.nDayTimesFee);
		}
	}
	public void ApplyBaizInstanceStatus()
	{		
		
		BaizInstanceStatus card = BaizInstanceManager.instance.TryGetInstanceStatus(101);
		if (card == null) return;
		
		NanBeizhanInstance.instance.idInstance = card.nInstanceID;
		NanBeizhanInstance.instance.idClass = 0;
		NanBeizhanInstance.instance.idCreator = 0;
		
		int d1 = card.nDayTimesFee - card.nResDayTimesFee;
		int d2 = card.nDayTimesFree - card.nResDayTimesFree;
		NanBeizhanInstance.instance.idRestDayTimesFee = d1;
		NanBeizhanInstance.instance.idRestDayTimesFree = d2;
				
		this._prettyStatus  = -1; // 无效初始状态 // 

		if (DayTimesFreeCol != null)
		{
			DayTimesFreeCol.text = string.Format(mPrettyFree,card.nResDayTimesFree,card.nDayTimesFree);
		}
		if (DayTimesFeeCol != null)
		{
			DayTimesFeeCol.text = string.Format(mPrettyFee, card.nResDayTimesFee, card.nDayTimesFee);
		}
		
		if (card.nInstanceID == 0) 
		{			
			// 删除战斗场景 ...
			NanBeizhanScene combatWin1 = GameObject.FindObjectOfType(typeof(NanBeizhanScene)) as NanBeizhanScene;
			if (combatWin1 != null) { Destroy(combatWin1.gameObject); }
			// print ("Retry instance no exists");
			
			return;
		}
		else
		{	
			NanBeizhanInstance.instance.idClass = card.nClassID;
			NanBeizhanInstance.instance.idCreator = card.nCreatorID;

			if (card.nStatus == 0) 
			{
				this._prettyStatus  = card.nStatus;
				this._classID = card.nClassID;
				
				LoadingManager.instance.ShowLoading();
				BaizInstanceManager.processGetInstanceDescDelegate = OnProcessGetInstanceDescDelegate;
				BaizInstanceManager.RequestInstanceDescData(101, _classID);
			}
			else if (card.nStatus == 1)
			{
				// 更新战斗列表项目 CombatDataList
				LoadingManager.instance.ShowLoading();
				CombatManager.processListCombatDataDelegate = OnProcessListCombatDataDelegate;
				CombatManager.RequestCombatListData();
			}
		}
	}
	
	void OnProcessListCombatDataDelegate()
	{
		NanBeizhanInstance.instance.ApplyInstanceCombatID();
		ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
		BaizInstanceManager.processInstanceDataDelegate += OnProcessGetInstanceDataDelegate;
		BaizInstanceManager.RequestGetInstanceData(101, nInstanceID);
	}
	
	void OnPopNanBeizhanBulletin()
	{
		GameObject go = U3dCmn.GetObjFromPrefab("BaizBulletinWin");
		if (go == null) return;
		
		BaizBulletinWin win1 = go.GetComponent<BaizBulletinWin>();
		if (win1 != null) 
		{
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			win1.Depth(depth);
			win1.ApplyBulletin(101);
		}
	}
	

	
}
