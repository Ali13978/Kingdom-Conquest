using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;

// 任务显示面板 ...
public class QuestLaunchWin : MonoBehaviour {
	
	public GameObject vip_unread_icon = null;
	public GameObject grow_unread_icon = null;
	public GameObject day_unread_icon = null;
	
	public PagingDraggablePanel panelDayFree1 = null; 		// 日常活动表 ...
	public PagingDraggablePanel panelGrowFee1 = null;		// 成长任务面板 ...
	public PagingDraggablePanel panelExpedFee1 = null;		// 出征列表面板 ...
	public PagingDraggablePanel panelVipFee1 = null;		// VIP任务面板 ...
	public PagingDragContents dragContents = null;
	
	public UICheckbox checkDayFree = null;
	public UICheckbox checkGrowFee = null;
	public UICheckbox checkExpedFee = null;
	public UICheckbox checkVipFee = null;
	
	public GameObject VipRight = null;
	
	UIAnchor mDepth = null;
	
	void Awake()
	{
		UIRoot root = GetComponent<UIRoot>();
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			if (root != null) { root.manualHeight = 320; }
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			if (root != null) { root.manualHeight = 360; }
		}
		
		if (day_unread_icon != null) {
			NGUITools.SetActive(day_unread_icon,false);
		}
		if (grow_unread_icon != null) {
			NGUITools.SetActive(grow_unread_icon,false);
		}
		if (vip_unread_icon != null) {
			NGUITools.SetActive(vip_unread_icon,false);
		}
	}
	
	void OnDestroy() { onExceptDelegate(); }
	void onExceptDelegate()
	{
		//print ("- onExceptDelegate");
		QuestFeeManager.processDoneQuestDelegate -= OnProcessDoneQuestDelegate;
	}
	
	// Use this for initialization
	void Start () {
	
		if (VipRight != null) {
			UIEventListener.Get(VipRight).onClick = clickBtnVipRightDelegate;
		}
		
		// 刷下界面 ...
		SetUnReadNow();
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
	
	void ApplyQuestListStatus()
	{
		if (true == checkDayFree.isChecked)
		{
			List<QuestDesc> cacheList = QuestFeeManager.instance.GetDayFreeList();
			if (panelDayFree1 != null)
			{
				PagingStorage card = panelDayFree1.Storage;
				card.SetCapacity(cacheList.Count);
				card.ResetAllSurfaces();
				// Fini
				panelDayFree1.RestrictVisibleWithinBounds();
			}
		}
		else if (true == checkGrowFee.isChecked)
		{
			List<QuestDesc> cacheList = QuestFeeManager.instance.GetGrowFeeList();
			if (panelGrowFee1 != null)
			{
				PagingStorage card = panelGrowFee1.Storage;
				card.SetCapacity(cacheList.Count);
				card.ResetAllSurfaces();
				// Fini
				panelGrowFee1.RestrictVisibleWithinBounds();
			}
		}
		else if (true == checkExpedFee.isChecked)
		{
			List<WildBossMBInfo> cacheList = CommonMB.WildBossMBInfo_List;
			if (panelExpedFee1 != null)
			{
				PagingStorage card = panelExpedFee1.Storage;
				card.SetCapacity(cacheList.Count);
				card.ResetAllSurfaces();
				// Fini
				panelExpedFee1.RestrictVisibleWithinBounds();
			}
		}
		else if (true == checkVipFee.isChecked)
		{
			List<QuestDesc> cacheList = QuestFeeManager.instance.GetVipFeeList();
			if (panelVipFee1 != null)
			{
				PagingStorage card = panelVipFee1.Storage;
				card.SetCapacity(cacheList.Count);
				card.ResetAllSurfaces();
				
				// Fini
				panelVipFee1.RestrictVisibleWithinBounds();
			}
		}
	}
	
	void OnProcessListQuestDelegate()
	{
		LoadingManager.instance.HideLoading();
		this.ApplyQuestListStatus();
	}
	
	// 刷新任务面板 ...
	public void RefreshQuestList()
	{
		QuestFeeManager.processListQuestDelegate = OnProcessListQuestDelegate;
		QuestFeeManager.RequesGetFeeQuest();
	}
	
	// 切换成日常任务面板 ...
	void OnQuestDayFreePopup()
	{
		if (dragContents != null) {
			dragContents.draggablePanel = panelDayFree1;
		}
		
		this.ApplyQuestListStatus();
	}
	// 切换至成长任务面板 ....
	void OnQuestGrowFeePopup()
	{
		if (dragContents != null) {
			dragContents.draggablePanel = panelGrowFee1;
		}
		
		this.ApplyQuestListStatus();
	}
	// 切换至出征面板 ...
	void OnQuestExpeditionFeePopup()
	{
		if (dragContents != null) {
			dragContents.draggablePanel = panelExpedFee1;
		}
		
		this.ApplyQuestListStatus();
	}
	// 切换至VIP任务面板 ...
	void OnQuestVipRightFeePopup()
	{
		if (dragContents != null) {
			dragContents.draggablePanel = panelVipFee1;
		}
		
		this.ApplyQuestListStatus();
	}
	
	void OnQuestLaunchClose()
	{
		// 新手引导 ...
		Destroy(gameObject);
	}
	
	// 显示VIP任务说明 ...
	void clickBtnVipRightDelegate(GameObject go)
	{
		GameObject infowin = U3dCmn.GetObjFromPrefab("BulletinWin");
		if(infowin != null)
		{
			InstanceMBInfo info = (InstanceMBInfo)CommonMB.InstanceMBInfo_Map[(uint)HELP_TYPE.VIP_HELP];
			infowin.GetComponent<BulletinWin>().title_label.text = info.mode;
			infowin.GetComponent<BulletinWin>().text_label.text = info.Rule1;
			infowin.SendMessage("RevealPanel");
		}
	}
	
	// 更新未领取奖励 ...
	void SetUnReadNow()
	{
		SetUnReadIcon(vip_unread_icon, TaskManager.vipFee_unread_num);
		SetUnReadIcon(grow_unread_icon, TaskManager.growFee_unread_num);
		SetUnReadIcon(day_unread_icon, TaskManager.dayFee_unread_num);
	}
	
	// 更新未读任务图标 ...
	void SetUnReadIcon(GameObject unread_icon, int unread_num)
	{
		if(unread_num >0)
		{
			unread_icon.SetActiveRecursively(true);
			if(unread_num >99)
				unread_icon.GetComponentInChildren<UILabel>().text = "99+";
			else 
				unread_icon.GetComponentInChildren<UILabel>().text = unread_num.ToString();
		}
		else
		{
			unread_icon.SetActiveRecursively(false);
		}
	}
	
	// 新手引导 ...	
	void OnProcessDoneQuestDelegate()
	{
		RefreshQuestList();
		
		// 返回处理, 更新下一步 ....
		if (NewbieQuestbook.processTaskDoneRst != null)
		{
			NewbieQuestbook.processTaskDoneRst();
			NewbieQuestbook.processTaskDoneRst = null;
		}
	}
	
	void VoidDoneQuest()
	{
		List<QuestDesc> cacheList = QuestFeeManager.instance.GetGrowFeeList();
		if (cacheList.Count == 0) return;
		
		QuestDesc d1 = cacheList[0];
		
		// 显示 ...
		LoadingManager.instance.ShowLoading();
		QuestFeeManager.processDoneQuestDelegate = OnProcessDoneQuestDelegate;
		QuestFeeManager.RequestDoneFeeQuest(d1.ExcelID1);
	}
	
	// (新手引导)显示出征任务 ...
	void TabChuZheng()
	{
		checkExpedFee.isChecked = true;
		ApplyQuestListStatus();
	}
	
	void VoidDoneChuZheng()
	{
		List<WildBossMBInfo> cacheList = CommonMB.WildBossMBInfo_List;
		if (cacheList.Count == 0) return;
		
		GameObject go = U3dCmn.GetObjFromPrefab("PopGeneralCrushWin");
		if (go == null) return;		
		
		PopGeneralCrush win1 = go.GetComponent<PopGeneralCrush>();
		if (win1 != null)
		{
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			win1.Depth(depth);
			
			WildBossMBInfo info = cacheList[0];
			win1.ApplyFeeExcelID(info.name, info.ExcelID);
			win1.ApplyFeeEnemyPower();
			win1.TabPopPicking();
		}
	}
	
	
}
