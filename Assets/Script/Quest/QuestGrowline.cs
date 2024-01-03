using UnityEngine;
using System.Collections;

using CTSCMD;
using STCCMD;

// 成长任务的显示卡片 ...
public class QuestGrowline : PagingStorageSlot {
	
	public GameObject Button1 = null;
		
	public UILabel AwardCol = null;
	public UILabel HeadCol = null;
	public UISprite DarkGoldCol = null;
	public UILabel TargetCol = null;
	public UISprite Button1gnd = null;
	
	string mPrettyTarget;
	string mPrettyHead;
	string mPrettyAward;
	
	int m_id = 0;
	uint m_excel_id = 0;
	bool hasChanged = false;
	
	QuestLaunchWin mCScene = null;
	
	void Awake() {
		
		if (AwardCol != null) {
			mPrettyAward = AwardCol.text;
			NGUITools.SetActiveSelf(AwardCol.gameObject,false);
		}
		if (HeadCol != null) {
			mPrettyHead = HeadCol.text;
			HeadCol.enabled = false;
		}
		if (TargetCol != null) {
			mPrettyTarget = TargetCol.text;
			NGUITools.SetActiveSelf(TargetCol.gameObject,false);
		}
		if (DarkGoldCol != null) {
			NGUITools.SetActiveSelf(DarkGoldCol.gameObject,false);
		}
		if (Button1 != null) 
		{
			NGUITools.SetActive(Button1.gameObject,false);
			UIEventListener.Get(Button1.gameObject).onClick = VoidButton1Delegate;
		}
	}
	
	void OnDestroy() { onExceptDelegate(); }
	void onExceptDelegate()
	{
		QuestFeeManager.processDoneQuestDelegate -= OnProcessDoneQuestDelegate;
	}
	
	public override int gid
	{
		get {
			return m_id;
		}
		set 
		{
			m_id = value;
			hasChanged = true;
		}
	}
	
	public override void ResetItem ()
	{
		ApplyQuestFee(null, false);
		hasChanged = true;
	}
	
	// Use this for initialization
	void Start () {
		if (mCScene == null) 
		{
			mCScene = NGUITools.FindInParents<QuestLaunchWin>(gameObject);
		}
	}

	
	void OnProcessDoneQuestDelegate()
	{
		QuestFeeMBInfo info = QuestFeeManager.GetItem(m_excel_id);
		if (info != null)
		{
			CommonData.player_online_info.Crystal += (uint)info.AwardCrystal;
			CommonData.player_online_info.Exp += (uint)info.AwardExp;
			CommonData.player_online_info.Gold += (uint)info.AwardGold;
			CommonData.player_online_info.Diamond += (uint)info.AwardDiamond;
			PlayerInfoManager.RefreshPlayerDataUI();
		}
		
		if (mCScene != null) {
			mCScene.RefreshQuestList();
		}
	}
	
	void VoidButton1Delegate(GameObject go)
	{
		int ExcelID = (int) m_excel_id;
		
		LoadingManager.instance.ShowLoading();
		QuestFeeManager.processDoneQuestDelegate = OnProcessDoneQuestDelegate;
		QuestFeeManager.RequestDoneFeeQuest(ExcelID);
	}
	
	void ApplyQuestFee(QuestFeeMBInfo info, bool isDone)
	{
		if (info == null )
		{
			if (AwardCol != null) {
				NGUITools.SetActiveSelf(AwardCol.gameObject,false);
			}
			if (HeadCol != null) {
				HeadCol.enabled = false;
			}			
			if (TargetCol != null) {
				NGUITools.SetActiveSelf(TargetCol.gameObject,false);
			}			
			if (DarkGoldCol != null) {
				NGUITools.SetActiveSelf(DarkGoldCol.gameObject,false);
			}
			if (Button1 != null) {
				NGUITools.SetActive(Button1.gameObject,false);
			}
			
			return;
		}
		
		// 当前任务唯一ID ... 
		m_excel_id = info.ExcelID;
		
		// 文字描述 ... 
		if (DarkGoldCol != null) {
			NGUITools.SetActiveSelf(DarkGoldCol.gameObject,true);
		}
		if (HeadCol != null) {
			HeadCol.text = string.Format(mPrettyHead,info.name);
			HeadCol.enabled = true;
		}
		if (AwardCol != null) {
			string np = QuestFeeManager.StringReward(info);
			AwardCol.text = string.Format(mPrettyAward, np);
			NGUITools.SetActiveSelf(AwardCol.gameObject,true);
		}
		if (TargetCol != null) {
			TargetCol.text = string.Format(mPrettyTarget, info.Target);
			NGUITools.SetActiveSelf(TargetCol.gameObject,true);
		}
		if (Button1 != null) {
			NGUITools.SetActive(Button1.gameObject,true);
			Button1.GetComponent<Collider>().enabled = isDone;
			if (false == isDone ) {
				Button1gnd.spriteName = "button1_disable";
			}
			else {
				Button1gnd.spriteName = "button1";
			}
		}
		
	}
	
	void LateUpdate() {
		if (true == hasChanged) 
		{
			this.hasChanged = false;
			
			bool isDone = false;
			int ExcelID = QuestFeeManager.instance.GetGrowFeeExcelID(m_id, out isDone);
			uint idQuest = (uint) ExcelID;
			QuestFeeMBInfo info = QuestFeeManager.GetItem(idQuest);
			this.ApplyQuestFee(info, isDone);
		}
	}
}
