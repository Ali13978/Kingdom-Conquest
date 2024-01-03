using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CMNCMD;

public class NanBeizhanHall : MonoBehaviour {
	
	UIAnchor mDepth = null;
	public PagingDraggablePanel panelFee = null;
	
	void Awake()
	{
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
		BaizInstanceManager.processCreateInstanceDelegate -= OnProcessCreateInstanceDelegate;
		BaizInstanceManager.processGetInstanceDescDelegate -= OnProcessGetInstanceDescDelegate;
		BaizInstanceManager.processJoinInstanceDelegate -= OnProcessJoinInstanceDelegate;
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
	
	public void OnNanBeizhanHallClose()
	{		
		// 删除 ...
		NGUITools.SetActive(gameObject, false);
		Destroy(gameObject);
	}
	
	public void ApplyInstanceDescList()
	{
		List<BaizInstanceDesc> cacheList = BaizInstanceManager.instance.GetInstanceDescList();
		int imax = cacheList.Count;
		ulong nCreatorID = NanBeizhanInstance.instance.idCreator;
		for (int i=0; i<imax; ++ i)
		{
			BaizInstanceDesc new1 = cacheList[i];
			if (new1.nCreatorID == nCreatorID)
			{
				NanBeizhanInstance.instance.idCaptain = new1.name;
			}
		}
		
		if (panelFee != null)
		{
			PagingStorage card = panelFee.Storage;
			card.SetCapacity(imax);
			card.ResetAllSurfaces1(0);
			panelFee.RestrictVisibleWithinBounds();
		}
	}
	
	void OnButtonNanBeizhanCreateInstance(GameObject go)
	{
		LoadingManager.instance.ShowLoading();
		int nClassID = NanBeizhanInstance.instance.idClass;
		BaizInstanceManager.processCreateInstanceDelegate -= OnProcessCreateInstanceDelegate;
		BaizInstanceManager.processCreateInstanceDelegate += OnProcessCreateInstanceDelegate;
		BaizInstanceManager.RequestCreateInstance(101, nClassID);
		CangKuManager.SubItem(2016,1);
		PopTipDialog.instance.Dissband();
	}
	
	void OnNanBeizhanInstanceGather()
	{
		int d1 = NanBeizhanInstance.instance.idRestDayTimesFree;
		int d2 = NanBeizhanInstance.instance.idRestDayTimesFee;
		if (d1>0)
		{
			LoadingManager.instance.ShowLoading();
			int nClassID = NanBeizhanInstance.instance.idClass;
			
			BaizInstanceManager.processCreateInstanceDelegate -= OnProcessCreateInstanceDelegate;
			BaizInstanceManager.processCreateInstanceDelegate += OnProcessCreateInstanceDelegate;
			BaizInstanceManager.RequestCreateInstance(101, nClassID);
		}
		else if (d2>0)
		{
			int Tipset = BaizVariableScript.INSTANCE_NANZHENGBEIZHAN_ITEM_COST;
			string cc = U3dCmn.GetWarnErrTipFromMB(Tipset);
			ITEM_INFO desc = U3dCmn.GetItemIconByExcelID(2016);
			
			string cs = string.Format(cc, desc.Name);
			PopTipDialog.instance.VoidSetText2(true,true,cs);
			PopTipDialog.instance.VoidButton1(OnButtonNanBeizhanCreateInstance);
		}
		else 
		{
			int Tipset = BaizVariableScript.CREATE_INSTANCE_TIMES;
			PopTipDialog.instance.VoidSetText1(true,false, Tipset);
		}
	}
	
	void OnProcessCreateInstanceDelegate(ulong nInstanceID)
	{
		LoadingManager.instance.HideLoading();
		NanBeizhanInstance.instance.idInstance = nInstanceID;
		NanBeizhanInstance.instance.idCreator = CommonData.player_online_info.AccountID;
		NanBeizhanInstance.instance.idCaptain = CommonData.player_online_info.CharName;
		// 房间弹出 ...
		PopupNanBeizhanRoom();
	}
	
	public void PopupNanBeizhanRoom()
	{
		GameObject go = U3dCmn.GetObjFromPrefab("NanBeizhanRoomWin");
		if (go == null ) return;
		
		NanBeizhanRoom t = go.GetComponent<NanBeizhanRoom>();
		if (t != null)
		{
			bool isLeaderOrNo = NanBeizhanInstance.instance.IsInstanceCreator();
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			t.Depth(depth);
			t.ApplyLeaderOrNo(isLeaderOrNo);
			t.ApplyGetInstanceDataConf();
			t.ApplyGetDiTuConf();
		}
	}
	
	void OnNanBeizhanResetHall()
	{
		LoadingManager.instance.ShowLoading();
		int nClassID = NanBeizhanInstance.instance.idClass;
		BaizInstanceManager.processGetInstanceDescDelegate = OnProcessGetInstanceDescDelegate;
		BaizInstanceManager.RequestInstanceDescData(101, nClassID);
	}
	
	void OnProcessGetInstanceDescDelegate()
	{
		LoadingManager.instance.HideLoading();
		if (panelFee != null)
		{
			List<BaizInstanceDesc> cacheList = BaizInstanceManager.instance.GetInstanceDescList();
			int imax = cacheList.Count;
			PagingStorage card = panelFee.Storage;
			card.SetCapacity(imax);
			card.ResetAllSurfaces();
			panelFee.RestrictVisibleWithinBounds();
		}
	}
	
	public void ResetHall()
	{
		LoadingManager.instance.ShowLoading();
		int nClassID = NanBeizhanInstance.instance.idClass;
		BaizInstanceManager.processGetInstanceDescDelegate = OnProcessGetInstanceDescDelegate;
		BaizInstanceManager.RequestInstanceDescData(101, nClassID);
	}
	
	void OnProcessJoinInstanceDelegate()
	{
		LoadingManager.instance.HideLoading();
		PopupNanBeizhanRoom();
	}
	
	void OnButtonNanBeizhanJoinInstance(GameObject go)
	{
		LoadingManager.instance.ShowLoading();
		ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
		BaizInstanceManager.processJoinInstanceDelegate -= OnProcessJoinInstanceDelegate;
		BaizInstanceManager.processJoinInstanceDelegate += OnProcessJoinInstanceDelegate;
		BaizInstanceManager.RequestJoinInstance(101, nInstanceID);
		CangKuManager.SubItem(2016,1); // 花了钱的 ...
		PopTipDialog.instance.Dissband();
	}
	
	public void PopupNanBeizhanInstanceRoom(BaizInstanceDesc item)
	{
		NanBeizhanInstance.instance.idInstance = item.nInstanceID;
		NanBeizhanInstance.instance.idCreator = item.nCreatorID;
		NanBeizhanInstance.instance.idCaptain = item.name;

		int d1 = NanBeizhanInstance.instance.idRestDayTimesFree;
		int d2 = NanBeizhanInstance.instance.idRestDayTimesFee;
		if (d1>0)
		{
			LoadingManager.instance.ShowLoading();
			BaizInstanceManager.processJoinInstanceDelegate -= OnProcessJoinInstanceDelegate;
			BaizInstanceManager.processJoinInstanceDelegate += OnProcessJoinInstanceDelegate;
			BaizInstanceManager.RequestJoinInstance(101, item.nInstanceID);
		}
		else if (d2>0)
		{
			int Tipset = BaizVariableScript.INSTANCE_NANZHENGBEIZHAN_ITEM_COST;
			string cc = U3dCmn.GetWarnErrTipFromMB(Tipset);
			ITEM_INFO desc = U3dCmn.GetItemIconByExcelID(2016);
			
			string cs = string.Format(cc, desc.Name);
			PopTipDialog.instance.VoidSetText2(true,true,cs);
			PopTipDialog.instance.VoidButton1(OnButtonNanBeizhanJoinInstance);
		}
		else 
		{
			int Tipset = BaizVariableScript.CREATE_INSTANCE_TIMES;
			PopTipDialog.instance.VoidSetText1(true,false, Tipset);
		}
	}
}
