using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class NanBeizhanRoom : MonoBehaviour {
	
	UIAnchor mDepth = null;
	
	public GameObject ButtonReadyNow = null;
	public GameObject ButtonQuitNow = null;
	public GameObject ButtonChuZhen = null;
	public GameObject ButtonDisband = null;
	public GameObject ButtonClose = null;
	public Transform TopBar = null;
	
	public UILabel PrettyDiTuCol = null;
	public UILabel PrettyCaptainCol = null;
	
	string mPrettyCaptain;
	
	NanBeizhanLaunchWin mLauncher = null;
	NanBeizhanHall mHall = null;
	
	public NanBeizhanItem[] AlliedCol = new NanBeizhanItem[5];
	
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
		
		if (PrettyCaptainCol != null) {
			mPrettyCaptain = PrettyCaptainCol.text;
		}
		
		onEnableDelegate(); 
	}
	void OnDestroy() { onExceptDelegate(); }
	
	void onEnableDelegate()
	{
		BaizInstanceManager.processStartInstanceCon1 += OnProcessStartInstanceCon1;
		BaizInstanceManager.processQuitInstanceDelegate += OnProcessNanBeizhanQuitDelegate;
		BaizInstanceManager.processInstanceDataCon1 += OnProcessNanBeizhanInstanceDataCon1;
		BaizInstanceManager.processDestroyInstanceDelegate = null;
		BaizInstanceManager.processDestroyInstanceDelegate = OnProcessNanBeizhanDisbandDelegate;
	}
	
	void onExceptDelegate()
	{
		BaizInstanceManager.processStartInstanceCon1 -= OnProcessStartInstanceCon1;
		BaizInstanceManager.processQuitInstanceDelegate -= OnProcessNanBeizhanQuitDelegate;
		BaizInstanceManager.processInstanceDataCon1 -= OnProcessNanBeizhanInstanceDataCon1;
		BaizInstanceManager.processDestroyInstanceDelegate -= OnProcessNanBeizhanDisbandDelegate;
		
		BaizInstanceManager.processGetInstanceDescDelegate -= OnProcessGetInstanceDescDelegate;
		BaizInstanceManager.processPrepareInstanceDelegate -= OnProcessPrepareInstanceDelegate;
		BaizInstanceManager.processInstanceDataDelegate -= OnProcessInstanceDataDelegate;
	}
	// Use this for initialization
	void Start () 
	{
		if (mHall == null) {
			mHall = UnityEngine.Object.FindObjectOfType(typeof(NanBeizhanHall)) as NanBeizhanHall;
		}
		if (mLauncher == null) {
			mLauncher = UnityEngine.Object.FindObjectOfType(typeof(NanBeizhanLaunchWin)) as NanBeizhanLaunchWin;
		}		
	}

	void OnProcessNanBeizhanInstanceDataCon1(BaizInstanceGroup newGroup)
	{		
		// 确定武将数目和状态 ...
		ulong nAccountID = CommonData.player_online_info.AccountID;
		PickingGeneral[] args = NanBeizhanInstance.instance.GetGenerals();
		args[0] = null;
		args[1] = null;
		args[2] = null;
		args[3] = null;
		args[4] = null;
		
		int i=0;
		for(i=0; i<newGroup.nNum; ++ i)
		{
			BaizInstanceData item = newGroup.m_ma[i];
			NanBeizhanItem gCol = AlliedCol[i];
			gCol.ApplyGeneralItem(item);
			gCol.idAccount = item.nAccountID;
			gCol.idCol = i;
			
			if (newGroup.nNumHero>0)
			{
				PickingGeneral gen = newGroup.m_ga[i];
				if (gen != null)
				{
					args[i] = gen;
					gCol.ApplyGeneralProf(gen.nProf);
				}
			}
			
			if (item.nAccountID == nAccountID)
			{
				NanBeizhanInstance.instance.idCol = i;
			}
		}
		
		for (; i<5; ++ i)
		{
			NanBeizhanItem gCol = AlliedCol[i];
			gCol.ApplyGeneralItem(null);
			gCol.ApplyGeneralProf(0);
			gCol.idAccount = 0;
			gCol.idCol = -1;
		}
		
		//print ("OnProcessNanBeizhanInstanceDataCon1:" + newGroup.nInstanceID);
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
	
	void OnProcessGetInstanceDescDelegate()
	{	
		LoadingManager.instance.HideLoading();
		if (mHall != null) {
			mHall.ApplyInstanceDescList();
		}
		
		// 关闭调兵界面 ...
		NanBeizhanTroop win1 = GameObject.FindObjectOfType(typeof(NanBeizhanTroop)) as NanBeizhanTroop;
		if (win1 != null) { Destroy(win1.gameObject); }
		PickingGeneralWin win2 = GameObject.FindObjectOfType(typeof(PickingGeneralWin)) as PickingGeneralWin;
		if (win2 != null) { Destroy(win2.gameObject); }
		
		NGUITools.SetActive(gameObject,false);
		Destroy(gameObject);
	}
	
	void OnProcessNanBeizhanDisbandDelegate()
	{
		NanBeizhanInstance.instance.AssignInstanceGeneralFree();
		
		// 重新获取列表 ...
		LoadingManager.instance.ShowLoading();
		int nClassID = NanBeizhanInstance.instance.idClass;
		BaizInstanceManager.processGetInstanceDescDelegate = OnProcessGetInstanceDescDelegate;
		BaizInstanceManager.RequestInstanceDescData(101, nClassID);
	} 
	
	void OnButtonNanBeizhanDestroyInstance(GameObject go)
	{
		ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
		BaizInstanceManager.RequestDestroyInstance(101, nInstanceID);
		PopTipDialog.instance.Dissband();
	}
	
	void OnNanBeizhanRoomDisband(GameObject go)
	{
		ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
		if (nInstanceID == 0) return;
		
		int Tipset = BaizVariableScript.INSTANCE_DESTROY_ASKING_OK;
		PopTipDialog.instance.VoidSetText1(true,true,Tipset);
		PopTipDialog.instance.VoidButton1(OnButtonNanBeizhanDestroyInstance);
	}
	
	void OnProcessPrepareInstanceDelegate()
	{ 
		LoadingManager.instance.HideLoading();
		int idCol = NanBeizhanInstance.instance.idCol;
		NanBeizhanItem item = AlliedCol[idCol];
		if (item.ArmyOrReadyCol != null) {
			NGUITools.SetActiveSelf(item.ArmyOrReadyCol.gameObject, true);
		}
	}
	
	public void ApplyGetInstanceDataConf()
	{
		ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
		BaizInstanceManager.RequestGetInstanceData(101, nInstanceID);
	}
	
	public void ApplyGetDiTuConf()
	{
		if (PrettyCaptainCol != null)
		{
			string name = NanBeizhanInstance.instance.idCaptain;
			PrettyCaptainCol.text = string.Format(mPrettyCaptain, name);
		}
		
		if (PrettyDiTuCol != null)
		{
			uint nExcelID = (uint) NanBeizhanInstance.instance.idClass;
			Hashtable nzbzMap = CommonMB.NanBeizhanInfo_Map;
			if (true == nzbzMap.ContainsKey(nExcelID))
			{
				NanBeizhanMBInfo obj = (NanBeizhanMBInfo) nzbzMap[nExcelID];
				PrettyDiTuCol.text = obj.name;
			}
		}
	}
	
	void OnNanBeizhanRoomClose(GameObject go)
	{
		if (mLauncher != null) {
			mLauncher.DoLaunchClose();
		}
		
		if (mHall != null) {
			mHall.OnNanBeizhanHallClose();
		}
		
		// iphone5 挪开 ...
		KeepOut.instance.HideKeepOut();
		
		// 关闭 ...
		NGUITools.SetActive(gameObject, false);
		Destroy(gameObject);
	}
	
	public void ApplyLeaderOrNo(bool isLeaderOrNo)
	{		
		if (true == isLeaderOrNo)
		{
			if (ButtonQuitNow != null) 
			{
				NGUITools.SetActive(ButtonQuitNow, false);
			}
			if (ButtonReadyNow != null)
			{
				NGUITools.SetActive(ButtonReadyNow, false);
			}
			if (ButtonChuZhen != null) 
			{
				NGUITools.SetActive(ButtonChuZhen, true);
			}
			if (ButtonDisband != null) {
				NGUITools.SetActive(ButtonDisband, true);
				UIEventListener.Get(ButtonDisband).onClick = OnNanBeizhanRoomDisband;
			}
			if (ButtonClose != null) {
				UIEventListener.Get(ButtonClose).onClick = OnNanBeizhanRoomClose;
			}	
		}
		else 
		{
			if (ButtonReadyNow != null) {
				NGUITools.SetActive(ButtonReadyNow, true);
			}
			if (ButtonChuZhen != null) {
				NGUITools.SetActive(ButtonChuZhen, false);
			}
			if (ButtonDisband != null) {
				NGUITools.SetActive(ButtonDisband, false);
			}
			if (ButtonQuitNow != null) {
				NGUITools.SetActive(ButtonQuitNow, true);
				UIEventListener.Get(ButtonQuitNow).onClick = OnNanBeizhanQuitNow;
			}
			if (ButtonClose != null) {
				UIEventListener.Get(ButtonClose).onClick = OnNanBeizhanRoomClose;
			}
		}
	}
	
	public void PopupNanBeizhanTroop(int firstid, ulong nAccountID)
	{		
		GameObject go = U3dCmn.GetObjFromPrefab("NanBeizhanTroopWin");
		if (go == null) return;
	
		NanBeizhanTroop t = go.GetComponent<NanBeizhanTroop>();
		if (t != null)
		{
			bool isLeaderOrNo = NanBeizhanInstance.instance.IsInstanceCreator();		
			int idCol = NanBeizhanInstance.instance.idCol;
			bool onPick = (idCol == firstid);
			bool onKick = ((isLeaderOrNo==true) && (onPick == false));
			t.AssignTroopAction(onKick, onPick);
			t.AssignAccountID(nAccountID);
			t.AssignTroopTip();
			
			PickingGeneral[] args = NanBeizhanInstance.instance.GetGenerals();
			PickingGeneral data = args[firstid];
			if (data != null)
			{
				t.AssignGeneralPicking(data);
			}
			else if (onPick == true)
			{	
				// 默认选择第一个可以用的 Apply
				PickingGeneralManager.instance.ApplyHireGeneral();
				PickingGeneral theData = PickingGeneralManager.instance.GetFirst();
						
				if (theData == null) 
				{
					int Tipset = BaizVariableScript.INSTANCE_PICKING_GENERAL_NO_FREE;
					PopTipDialog.instance.VoidSetText1(true, false, Tipset);
					
					NGUITools.SetActive(go, false);
					Destroy(go);
					
					return;
				}
				else 
				{
					// 设置武将 ...
					t.AssignGeneralPickingCon1(theData);
				}
			}
			
			// 深度设置 ...
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			t.Depth(depth);
		}
	}
	
	void OnPickingNanBeizhanGeneral()
	{
		int idCol = NanBeizhanInstance.instance.idCol;
		ulong nAccountID = CommonData.player_online_info.AccountID;
		PopupNanBeizhanTroop(idCol, nAccountID);
	}
	
	void OnNanBeizhanReadyNow()
	{
		LoadingManager.instance.ShowLoading();
		ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
		BaizInstanceManager.processPrepareInstanceDelegate += OnProcessPrepareInstanceDelegate;
		BaizInstanceManager.RequestPrepareInstance(101, nInstanceID, 1);
	}
	
	void OnProcessNanBeizhanQuitDelegate()
	{
		LoadingManager.instance.HideLoading();
		
		NanBeizhanInstance.instance.idInstance = 0;
		int idCol = NanBeizhanInstance.instance.idCol;
		if (idCol>-1 && idCol<5)
		{
			PickingGeneral[] args = NanBeizhanInstance.instance.GetGenerals();
			PickingGeneral gen = args[idCol];
			if (gen != null)
			{
				Hashtable jlMap = JiangLingManager.MyHeroMap;
				if (true == jlMap.ContainsKey(gen.nHeroID))
				{
					HireHero hh1 = (HireHero) jlMap[gen.nHeroID];
					hh1.nStatus14 = (int)CMNCMD.HeroState.NORMAL;
					jlMap[gen.nHeroID] = hh1;
				}
			}
		}
		
		if (mHall != null) { mHall.ResetHall(); }
		
		// 关闭调兵界面 ...
		NanBeizhanTroop win1 = GameObject.FindObjectOfType(typeof(NanBeizhanTroop)) as NanBeizhanTroop;
		if (win1 != null) { Destroy(win1.gameObject); }
		PickingGeneralWin win2 = GameObject.FindObjectOfType(typeof(PickingGeneralWin)) as PickingGeneralWin;
		if (win2 != null) { Destroy(win2.gameObject); }
		
		// Fini
		NGUITools.SetActive(gameObject, false);
		Destroy(gameObject);
	}
	
	void OnButtonNanBeizhanQuitNow(GameObject go)
	{		
		LoadingManager.instance.ShowLoading();
		ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
		BaizInstanceManager.RequestQuitInstance(101, nInstanceID);
		PopTipDialog.instance.Dissband();
	}
	
	void OnNanBeizhanQuitNow(GameObject go)
	{
		ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
		if (nInstanceID == 0) return;
		
		int Tipset = BaizVariableScript.INSTANCE_DESTROY_ASKING_OK;
		PopTipDialog.instance.VoidSetText1(true,true,Tipset);
		PopTipDialog.instance.VoidButton1(OnButtonNanBeizhanQuitNow);
	}
	
	void OnProcessInstanceDataDelegate(BaizInstanceGroup newGroup)
	{
		LoadingManager.instance.HideLoading();
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
		
		if (mLauncher != null) {
			mLauncher.DoLaunchClose();
		}
		
		if (mHall != null) {
			mHall.OnNanBeizhanHallClose();
		}
	
		PopTipDialog.instance.Dissband();
		
		// 关闭调兵界面 ...
		NanBeizhanTroop go1 = GameObject.FindObjectOfType(typeof(NanBeizhanTroop)) as NanBeizhanTroop;
		if (go1 != null) { Destroy(go1.gameObject); }
		PickingGeneralWin win2 = GameObject.FindObjectOfType(typeof(PickingGeneralWin)) as PickingGeneralWin;
		if (win2 != null) { Destroy(win2.gameObject); }
		

		NGUITools.SetActive(gameObject,false);
		Destroy(gameObject);
	}
	
	void OnProcessStartInstanceCon1()
	{
		ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
		BaizInstanceManager.processInstanceDataDelegate += OnProcessInstanceDataDelegate;
		BaizInstanceManager.RequestGetInstanceData(101, nInstanceID);
	}
	
	void OnNanBeizhanStartInstanceApplyDelegate(GameObject go)
	{
		LoadingManager.instance.ShowLoading();
		ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
		BaizInstanceManager.RequestStartInstance(101, nInstanceID);
		
		NanBeizhanReadyWin win1 = NGUITools.FindInParents<NanBeizhanReadyWin>(go);
		if (win1 != null) {
			Destroy(win1.gameObject);
		}
	}
	
	void OnNanBeizhanStartInstanceNow()
	{
		PickingGeneral[] args = NanBeizhanInstance.instance.GetGenerals();
		int genNum = 0;
		for (int i=0; i<args.Length; ++ i)
		{
			if (args[i] != null) genNum ++;
		}
		
		GameObject go = U3dCmn.GetObjFromPrefab("NanBeizhanReadyWin");
		if (go == null ) return;
		
		NanBeizhanReadyWin win1 = go.GetComponent<NanBeizhanReadyWin>();
		if (win1 != null)
		{
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			win1.Depth(depth);
			win1.ApplyRoadGeneral();
			win1.ApplyEnemyProf();
			win1.VoidButton1(OnNanBeizhanStartInstanceApplyDelegate);
		}
	}
	
}
