using UnityEngine;
using System.Collections;

public class NanBeizhanCaptain : PagingStorageSlot {
	
	public UILabel PrettyLevelCol = null;
	public UILabel PrettyMemberCol = null;
	public UILabel PrettyName = null;
	public UISlicedSprite PrettyBackgroud = null;
	public UISprite Headicon = null;

	string mPrettyLevel;
	string mPrettyMember;
	
	NanBeizhanHall mHallPanel = null;
	
	int m_id = -1;
	bool hasChanged = false;
	
	void Awake() {
		if (PrettyLevelCol != null)
		{
			mPrettyLevel = PrettyLevelCol.text;
			PrettyLevelCol.enabled = false;
		}
		if (PrettyMemberCol != null)
		{
			mPrettyMember = PrettyMemberCol.text;
			PrettyMemberCol.enabled = false;
		}
		if (PrettyBackgroud != null)
		{
			PrettyBackgroud.enabled = false;
		}
		if (PrettyName != null)
		{
			PrettyName.enabled = false;
		}
		if (Headicon != null) 
		{
			Headicon.enabled = false;
		}
	}
	
	override public int gid
	{
		get {
			return m_id;
		}
		
		set {
			
			if (m_id != value)
			{
				hasChanged = true;
				m_id = value;
			}
		}
	}
	
	override public void ResetItem()
	{
		hasChanged = true;
	}
	
	// Use this for initialization
	void Start () {
		if (mHallPanel == null)
		{
			mHallPanel = NGUITools.FindInParents<NanBeizhanHall>(gameObject);
		}
	}
	
	
	void ApplyBaizInstanceDesc(BaizInstanceDesc data)
	{
		if (data == null)
		{
			if (PrettyBackgroud != null)
			{
				PrettyBackgroud.enabled = false;
			}
			if (PrettyLevelCol != null)
			{
				PrettyLevelCol.enabled = false;
			}
			if (PrettyMemberCol != null)
			{
				PrettyMemberCol.enabled = false;
			}
			if (PrettyName != null)
			{
				PrettyName.enabled = false;
			}
			if (Headicon != null) 
			{
				Headicon.enabled = false;
			}
			
			return;
		}
		
		if (PrettyLevelCol != null)
		{
			PrettyLevelCol.text = string.Format(mPrettyLevel, data.nLeaderLevel);
			PrettyLevelCol.enabled = true;
		}
		
		if (PrettyMemberCol != null)
		{
			PrettyMemberCol.text = string.Format(mPrettyMember, data.nNumPlayer);
			PrettyMemberCol.enabled = true;
		}
		
		if (PrettyName != null)
		{
			PrettyName.text = data.name;
			PrettyName.enabled = true;
		}
		
		if (PrettyBackgroud != null)
		{
			PrettyBackgroud.enabled = true;
		}
	}
	
	void LateUpdate()
	{
		if (hasChanged == true)
		{
			hasChanged = false;
			
			BaizInstanceDesc item = NanBeizhanInstance.instance.GetItem(m_id);
			ApplyBaizInstanceDesc(item);
		}
	}
	
	void OnBaizInstanceDescChoose()
	{		
		BaizInstanceDesc item = NanBeizhanInstance.instance.GetItem(m_id);
		if (item == null) return;
		
		if (mHallPanel != null)
		{
			mHallPanel.PopupNanBeizhanInstanceRoom(item);
		}
	}
	
}  