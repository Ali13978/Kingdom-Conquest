using UnityEngine;
using System.Collections;
using CMNCMD;
public class BaizhanLaunchWin : MonoBehaviour {
	
	//	每日收费次数 ...
	public UILabel DayTimesFeeCol = null;
	// 每日免费次数 ...
	public UILabel DayTimesFreeCol = null;
	public GameObject ButtonRank = null;
	public Transform TopBar = null;
	public Transform BottomBar = null;
	public  tk2dSprite EpicBtn = null;
	UIAnchor mDepth = null;
	
	string mPrettyFee = "";
	string mPrettyFree = "";
	
	int mRestDayTimesFee = 0;
	int mRestDayTimesFree = 0;
	
	void Awake() {
		
		if (DayTimesFeeCol != null)
		{
			mPrettyFee = DayTimesFeeCol.text;
			DayTimesFeeCol.text = "";
		}
		
		if (DayTimesFreeCol != null)
		{
			mPrettyFree = DayTimesFreeCol.text;
			DayTimesFreeCol.text = "";
		}
		
		if (ButtonRank != null)
		{
			UIEventListener.Get(ButtonRank).onClick = OnPopAresRankWinDelegate;
		}
		
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
			BottomBar.localPosition -= new Vector3(0f,OffsetY,0f);
		}
		if(CommonData.player_online_info.WangZhe >= 100)
		{
			EpicBtn.spriteId = EpicBtn.GetSpriteIdByName("baizh-epic-normal");
			EpicBtn.color = new Color(255f,255f,255f);
			NGUITools.FindInParents<UIButtonMessage>(EpicBtn.gameObject).GetComponent<Collider>().enabled = true;
			//EpicBtn.gameObject.get
			//EpicBtn.s
		}
		else 
		{
			EpicBtn.spriteId = EpicBtn.GetSpriteIdByName("baizh-epic");
			NGUITools.FindInParents<UIButtonMessage>(EpicBtn.gameObject).GetComponent<Collider>().enabled = false;
			//EpicBtn.s
		}
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
	
	void OnDestroy() { onExceptDelegate(); }
		
	void onExceptDelegate()
	{		
		// 取消委托 ...
		CombatManager.processListCombatDataDelegate -= OnProcessListCombatDataDelegate;
		BaizInstanceManager.processCreateInstanceDelegate -= OnProcessCreateInstanceDelegate;
		BaizInstanceManager.processInstanceDataDelegate -= OnProcessGetInstanceDataDelegate;
		AresRankInstance.processAresRankCacheCB -= OnProcessAresRankCacheCBDelegate;
	}
	
	// Use this for initialization
	void Start () {
		SoundManager.StopMainSceneSound();
		SoundManager.PlayBattleSound();
		
		KeepOut.instance.ShowKeepOut();
	}
	
	// 刷新界面的副本状态 ...
	public void ApplyBaizInstanceStatus()
	{
		BaizInstanceStatus card = BaizInstanceManager.instance.TryGetInstanceStatus(100);
		if (card == null) return;
		
		BaizhanInstance.instance.idInstance = card.nInstanceID;
		BaizhanInstance.instance.idCreator = card.nCreatorID;
		BaizhanInstance.instance.idClass = card.nClassID;
		mRestDayTimesFee = card.nDayTimesFee - card.nResDayTimesFee;
		mRestDayTimesFree = card.nDayTimesFree - card.nResDayTimesFree;
		
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
			BaizhanCampWin combatWin1 = GameObject.FindObjectOfType(typeof(BaizhanCampWin)) as BaizhanCampWin;
			if (combatWin1 != null) { Destroy(combatWin1.gameObject); }
			return;
		}
		else 
		{		
			LoadingManager.instance.ShowLoading();
			// 更新战斗列表项目 CombatDataList
			CombatManager.processListCombatDataDelegate = OnProcessListCombatDataDelegate;
			CombatManager.RequestCombatListData();
		}
	}
	
	void OnProcessListCombatDataDelegate()
	{
		BaizhanInstance.instance.ApplyInstanceCombatID();
		ulong nInstanceID = BaizhanInstance.instance.idInstance;
		BaizInstanceManager.processInstanceDataDelegate += OnProcessGetInstanceDataDelegate;
		BaizInstanceManager.RequestGetInstanceData(100, nInstanceID);
	}
	// 进入副本战场 ...
	void OnProcessGetInstanceDataDelegate(BaizInstanceGroup newGroup)
	{	
		LoadingManager.instance.HideLoading();
		// 更新关卡数 ....
		if ((int)CommonData.player_online_info.WangZhe+1 < newGroup.nCurLevel) {
			if(newGroup.nCurLevel >0)
				CommonData.player_online_info.WangZhe = (uint)(newGroup.nCurLevel-1);
		}
		
		GameObject go = U3dCmn.GetObjFromPrefab("BaizhanCampWin");
		if (go == null) return;

		BaizhanCampWin win1 = go.GetComponent<BaizhanCampWin>();
		if (win1 != null)
		{			
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			win1.Depth(depth);
			win1.ApplyInstanceGroup(newGroup);
		}
		
		// 删除 ...
		Destroy(gameObject);
	}
	
	void OnProcessCreateInstanceDelegate(ulong nInstanceID)
	{
		BaizInstanceManager.processInstanceDataDelegate += OnProcessGetInstanceDataDelegate;
		BaizInstanceManager.RequestGetInstanceData(100, nInstanceID);
	}
		
	void OnBaizhanEpicGameStart()
	{
		if (mRestDayTimesFree>0) 
		{
			LoadingManager.instance.ShowLoading();
			BaizInstanceManager.processCreateInstanceDelegate = OnProcessCreateInstanceDelegate;
			BaizInstanceManager.RequestCreateInstance(100, 2);
		}
		else if (mRestDayTimesFee>0)
		{
			int Tipset = BaizVariableScript.INSTANCE_BAIZHANBUDAI_ITEM_COST;
			string cc = U3dCmn.GetWarnErrTipFromMB(Tipset);
			ITEM_INFO desc = U3dCmn.GetItemIconByExcelID(2016);
			
			string cs = string.Format(cc, desc.Name);
			PopTipDialog.instance.VoidSetText2(true,true,cs);
			PopTipDialog.instance.VoidButton1(OnButtonEpicCreateInstance);
		}
		else
		{
			int Tipset = BaizVariableScript.CREATE_INSTANCE_TIMES;
			PopTipDialog.instance.VoidSetText1(true,false, Tipset);
		}
		BaizInstanceManager.EasyOrEpic = 2;
	}
	void OnButtonEpicCreateInstance(GameObject go)
	{
		LoadingManager.instance.ShowLoading();
		BaizInstanceManager.processCreateInstanceDelegate = OnProcessCreateInstanceDelegate;
		BaizInstanceManager.RequestCreateInstance(100, 2);
		CangKuManager.SubItem(2016,1); // 花了钱的处理 ...
		PopTipDialog.instance.Dissband();
	}
	void OnButtonEasyCreateInstance(GameObject go)
	{
		LoadingManager.instance.ShowLoading();
		BaizInstanceManager.processCreateInstanceDelegate = OnProcessCreateInstanceDelegate;
		BaizInstanceManager.RequestCreateInstance(100, 1);
		CangKuManager.SubItem(2016,1); // 花了钱的处理 ...
		PopTipDialog.instance.Dissband();
	}
	
	void OnBaizhanEasyGameStart()
	{
		if (mRestDayTimesFree>0) 
		{
			LoadingManager.instance.ShowLoading();
			BaizInstanceManager.processCreateInstanceDelegate = OnProcessCreateInstanceDelegate;
			BaizInstanceManager.RequestCreateInstance(100, 1);
		}
		else if (mRestDayTimesFee>0)
		{
			int Tipset = BaizVariableScript.INSTANCE_BAIZHANBUDAI_ITEM_COST;
			string cc = U3dCmn.GetWarnErrTipFromMB(Tipset);
			ITEM_INFO desc = U3dCmn.GetItemIconByExcelID(2016);
			
			string cs = string.Format(cc, desc.Name);
			PopTipDialog.instance.VoidSetText2(true,true,cs);
			PopTipDialog.instance.VoidButton1(OnButtonEasyCreateInstance);
		}
		else
		{
			int Tipset = BaizVariableScript.CREATE_INSTANCE_TIMES;
			PopTipDialog.instance.VoidSetText1(true,false, Tipset);
		}
		BaizInstanceManager.EasyOrEpic = 1;
	}
	
	void OnProcessAresRankCacheCBDelegate()
	{
		GameObject go = U3dCmn.GetObjFromPrefab("BaizAresRankWin");
		if (go == null) return;
		
		BaizAresRankWin win1 = go.GetComponent<BaizAresRankWin>();
		if (win1 != null)
		{
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			win1.Depth(depth);
			win1.ApplyBaizhanAresList();
		}
	}
	// 排行榜窗口弹出 ...
	void OnPopAresRankWinDelegate(GameObject tween)
	{
		uint rank_type = (uint) RANK_TYPE.rank_type_instance_wangzhe;
		uint rank_num = (uint) BaizAresRankWin.PAGED_RANK_NUM;
		AresRankInstance.processAresRankCacheCB = OnProcessAresRankCacheCBDelegate;
		RankManager.GetRankListInfo(rank_type, 0, rank_num);
	}
	
	void OnBaizhanLaunchClose()
	{
		SoundManager.StopBattleSound();
		SoundManager.PlayMainSceneSound();
		
		// iphone5 遮挡挪开 ...
		KeepOut.instance.HideKeepOut();
		
		// 关闭 ...
		Destroy(gameObject);
	}
	
	void OnPopBaizhanBulletin()
	{
		GameObject go = U3dCmn.GetObjFromPrefab("BaizBulletinWin");
		if (go == null) return;
		
		BaizBulletinWin win1 = go.GetComponent<BaizBulletinWin>();
		if (win1 != null) 
		{
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			
			win1.Depth(depth);
			win1.ApplyBulletin(100);
		}
	}

	
}
