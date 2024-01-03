using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 出征任务的显示卡片 ...
public class QuestExpedition : PagingStorageSlot {
	
	public GameObject Button1 = null;			// 出征
	
	public UILabel AwardCol = null;
	public UILabel HeadCol = null;
	public UISprite DarkGoldCol = null;
	public UILabel DescCol = null;
	public UILabel LootCol = null;
	
	int m_id = 0;
	uint m_excel_id = 0;
	bool hasChanged = false;
	
	string mPrettyAward;
	string mPrettyDayFree;
	string mPrettyHeadline;
	string mPrettyTime;
	string mPrettyDesc;
	string mPrettyLoot;
	
	QuestLaunchWin mCScene = null;
	
	void Awake() {
		
		if (AwardCol != null) {
			mPrettyAward = AwardCol.text;
			NGUITools.SetActiveSelf(AwardCol.gameObject,false);
		}
		if (DescCol != null) {
			mPrettyDesc = DescCol.text;
			NGUITools.SetActiveSelf(DescCol.gameObject,false);
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
		if (LootCol != null) {
			mPrettyLoot = LootCol.text;
			LootCol.enabled = false;
		}
	}
	
	void OnDestroy() { onExceptDelegate(); }
	void onExceptDelegate()
	{

	}
	
	// Use this for initialization
	void Start () 
	{
		if (mCScene == null) {
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
		ApplyRaidFee(null);
		hasChanged = true;
	}
	
	void LateUpdate()
	{
		if (hasChanged == true)
		{
			this.hasChanged = false;
			
			WildBossMBInfo info = CommonMB.GetWildBossItem(m_id);
			ApplyRaidFee(info);
		}
	}
	
	void VoidButton1Delegate(GameObject noo)
	{		
		GameObject go = U3dCmn.GetObjFromPrefab("PopGeneralCrushWin");
		if (go == null) return;		
		
		PopGeneralCrush win1 = go.GetComponent<PopGeneralCrush>();
		if (win1 != null)
		{
			float depth = mCScene.GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			win1.Depth(depth);
			
			WildBossMBInfo info = CommonMB.GetWildBossItem(m_id);
			win1.ApplyFeeExcelID(info.name, m_excel_id);
			win1.ApplyFeeEnemyPower();
		}
	}
	
	static string StringAward(WildBossMBInfo info)
	{
		string sReward = " ";
		string d1;
		int sp = 0;
		
		if (info.Exp1>0)
		{
			d1 = U3dCmn.GetWarnErrTipFromMB(301);
			sReward += string.Format(d1,info.Exp1);
			sp ++;
		}
		
		if (info.Exp2>0)
		{
			d1 = U3dCmn.GetWarnErrTipFromMB(312);
			if (sp>0) { sReward += " "; }
			sReward += string.Format(d1,info.Exp2);
			sp ++;
		}
		
		return sReward;
	}
	
	void ApplyRaidFee(WildBossMBInfo info)
	{
		if (info == null)
		{
			if (AwardCol != null) {
				NGUITools.SetActiveSelf(AwardCol.gameObject,false);
			}
			if (HeadCol != null) {
				HeadCol.enabled = false;
			}
			if (LootCol != null) {
				LootCol.enabled = false;
			}
			if (DescCol != null) {
				NGUITools.SetActiveSelf(DescCol.gameObject, false);	
			}
			if (DarkGoldCol != null) {
				NGUITools.SetActiveSelf(DarkGoldCol.gameObject,false);
			}
			if (Button1 != null) {
				NGUITools.SetActive(Button1, false);
			}
			
			return;
		}
		
		// 当前任务唯一ID ...
		m_excel_id = info.ExcelID;
	
		if (AwardCol != null) {
			NGUITools.SetActiveSelf(AwardCol.gameObject,true);
			string cc = StringAward(info);
			AwardCol.text = string.Format(mPrettyAward, cc);
		}
		// 文件描述 Col ...
		if (DarkGoldCol != null) {
			NGUITools.SetActiveSelf(DarkGoldCol.gameObject,true);
		}
		if (HeadCol != null) {
			HeadCol.text = info.name;
			HeadCol.enabled = true;
		}
		if (DescCol != null) {
			DescCol.text = string.Format(mPrettyDesc, (info.Desc));
			NGUITools.SetActiveSelf(DescCol.gameObject, true);
		}
		if (Button1 != null) {
			NGUITools.SetActive(Button1, true);
		}
		if (LootCol != null) {
			ITEM_INFO qw = U3dCmn.GetItemIconByExcelID((uint)info.LootItem);
			LootCol.text = string.Format(mPrettyLoot, qw.Name);
			LootCol.enabled = true;
		}
	}
	
	
	
	
	
	
	
	
	
}
