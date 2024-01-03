using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;

// 时常任务的显示卡片 ...
public class QuestDayFreeline : PagingStorageSlot {
	
	public GameObject Button1 = null;
	
	public UILabel AwardCol = null;
	public UILabel DayFreeCol = null;
	public UILabel DayDescCol = null;
	public UILabel HeadCol = null;
	public UISprite DarkGoldCol = null;
	//public UILabel TimelineCol = null;
	public UISprite Button1gnd = null;
	
	int m_id = 0;
	uint m_excel_id = 0;
	bool hasChanged = false;
	
	string mPrettyAward;
	string mPrettyDayFree;
	string mPrettyHeadline;
	string mPrettyTime;
	string mPrettyDayDesc;
	
	QuestLaunchWin mCScene = null;
	
	void Awake() {
		
		if (AwardCol != null) {
			mPrettyAward = AwardCol.text;
			NGUITools.SetActiveSelf(AwardCol.gameObject,false);
		}
		/*if (TimelineCol != null) {
			mPrettyTime = TimelineCol.text;
			NGUITools.SetActiveSelf(TimelineCol.gameObject,false);
		}*/
		if (DayFreeCol != null) {
			mPrettyDayFree = DayFreeCol.text;
			NGUITools.SetActiveSelf(DayFreeCol.gameObject,false);
		}
		if (DayDescCol != null) {
			mPrettyDayDesc = DayDescCol.text;
			NGUITools.SetActiveSelf(DayDescCol.gameObject,false);
		}
		if (HeadCol != null) {
			mPrettyHeadline = HeadCol.text;
			HeadCol.enabled = false;
		}
		if (DarkGoldCol != null) {
			NGUITools.SetActiveSelf(DarkGoldCol.gameObject,false);
		}
		if (Button1 != null) {
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
		if (mCScene == null) {
			mCScene = NGUITools.FindInParents<QuestLaunchWin>(gameObject);
		}
	}
	
	
	void LateUpdate() {
		
		if (true == hasChanged)
		{
			this.hasChanged = false;
			bool isDone = false;
			int ExcelID = QuestFeeManager.instance.GetDayFreeExcelID(m_id, out isDone);
			uint idQuest = (uint) ExcelID;
			QuestFeeMBInfo info = QuestFeeManager.GetItem(idQuest);
			this.ApplyQuestFee(info, isDone);
		}
	}
	
	// 领取奖励返回处理 ...
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
		if (true == QuestFeeManager.EmbedGemDetermine(m_excel_id))
		{
			float depth = mCScene.GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			PopTipDialog.instance.Mode1(true,true,318);
			PopTipDialog.instance.VoidButton1(VoidButton1AskForOkDoneDelegate);
			PopTipDialog.instance.Popup1(depth);
		}
		else 
		{
			int ExcelID = (int) m_excel_id;
			LoadingManager.instance.ShowLoading();
			QuestFeeManager.processDoneQuestDelegate = OnProcessDoneQuestDelegate;
			QuestFeeManager.RequestDoneFeeQuest(ExcelID);
		}
	}
	// 询问是否可以提交 ...
	void VoidButton1AskForOkDoneDelegate(GameObject go)
	{
		PopTipDialog.instance.Dissband();
	
		int ExcelID = (int) m_excel_id;
		LoadingManager.instance.ShowLoading();
		QuestFeeManager.processDoneQuestDelegate = OnProcessDoneQuestDelegate;
		QuestFeeManager.RequestDoneFeeQuest(ExcelID);
	}
	
	// 初始化任务描述 ....
	void ApplyQuestFee(QuestFeeMBInfo info, bool isDone)
	{
		if (info == null)
		{
			if (HeadCol != null) {
				HeadCol.enabled = false;
			}
			if (AwardCol != null) {
				NGUITools.SetActiveSelf(AwardCol.gameObject,false);
			}
			if (DayFreeCol != null) {
				NGUITools.SetActiveSelf(DayFreeCol.gameObject,false);
			}
			if (DayDescCol != null) {
				NGUITools.SetActiveSelf(DayDescCol.gameObject,false);
			}
			if (DarkGoldCol != null) {
				NGUITools.SetActiveSelf(DarkGoldCol.gameObject,false);
			}
			if (Button1 != null) {
				NGUITools.SetActive(Button1.gameObject,false);
			}
			/*if (TimelineCol != null) {
				NGUITools.SetActiveSelf(TimelineCol.gameObject,false);
			}*/
			
			return ;
		}
		
		// 当前任务唯一ID ...
		m_excel_id = info.ExcelID;
		
		// 文件描述 Col ...
		if (DarkGoldCol != null) {
			NGUITools.SetActiveSelf(DarkGoldCol.gameObject,true);
		}
		if (Button1 != null) {
			NGUITools.SetActive(Button1,true);
			Button1.GetComponent<Collider>().enabled = isDone;
			if (false == isDone) {
				Button1gnd.spriteName = "button1_disable";
			}
			else {
				Button1gnd.spriteName = "button1";
			}
		}
		if (HeadCol != null) {
			HeadCol.text = info.name;
			HeadCol.enabled = true;
		}
		if (AwardCol != null) {
			string reward = QuestFeeManager.StringReward(info);
			AwardCol.text = string.Format(mPrettyAward, reward);
			NGUITools.SetActiveSelf(AwardCol.gameObject,true);
		}
		if (DayFreeCol != null) {
			DayFreeCol.text = string.Format(mPrettyDayFree, (info.Target));
			NGUITools.SetActiveSelf(DayFreeCol.gameObject,true);
		}
		if (DayDescCol != null) {
			DayDescCol.text = string.Format(mPrettyDayDesc, (info.Desc));
			NGUITools.SetActiveSelf(DayDescCol.gameObject,true);
		}
		/*if (TimelineCol != null) {
			string day = QuestFeeManager.StringYearMonthDay(info);
			TimelineCol.text = string.Format(mPrettyTime, day);
			NGUITools.SetActiveSelf(TimelineCol.gameObject,true);
		}*/
	}
}
