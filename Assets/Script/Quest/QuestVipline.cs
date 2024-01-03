using UnityEngine;
using System.Collections;

// VIP任务的显示卡片 ...
public class QuestVipline : PagingStorageSlot {
	
	public GameObject Button1 = null;
		
	// 奖励 ...
	public UILabel AwardCol = null;
	public UILabel HeadCol = null;				// 标题 ...
	public UISprite DarkGoldCol = null;			
	public UILabel TargetCol = null;			// 任务目标 ...
	public UISprite Button1gnd = null;			// 按钮背景 ...
	
	string mPrettyTarget;
	string mPrettyHead;
	string mPrettyAward;
	
	int m_id = 0;
	uint m_excel_id = 0;
	bool hasChanged = false;
	
	QuestLaunchWin mCScene = null;
	
	void Awake()
	{
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
	
	// Use this for initialization
	void Start () {
		if (mCScene == null) 
		{
			mCScene = NGUITools.FindInParents<QuestLaunchWin>(gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
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
		ApplyVipRightFee(null, false);
		hasChanged = true;
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
	
	void ApplyVipRightFee(QuestFeeMBInfo info, bool isDone)
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
			TargetCol.text = string.Format(mPrettyTarget, info.Desc);
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
	
	void LateUpdate()
	{
		if (true == hasChanged) 
		{
			this.hasChanged = false;
			
			bool isDone = false;
			int ExcelID = QuestFeeManager.instance.GetVipFeeExcelID(m_id, out isDone);
			uint idQuest = (uint) ExcelID;
			QuestFeeMBInfo info = QuestFeeManager.GetItem(idQuest);
			this.ApplyVipRightFee(info, isDone);
		}
	}
	
}
