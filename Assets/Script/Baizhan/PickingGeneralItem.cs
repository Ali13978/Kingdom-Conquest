using UnityEngine;
using System.Collections;

public class PickingGeneralItem : PagingStorageSlot {
	
	public UISprite Faceid;
	public UILabel PrettyNameCol = null;
	public UILabel PrettyRoadCol = null;
	public UILabel PrettyLevelCol = null;
	public UILabel ArmyLevelOrNumCol = null;
	public UILabel PrettyHealthCol = null;
	public UISprite Substrate = null;
	public UISprite TroopIcon;
	string mPrettyLevel;
	string mArmyLevelOrNum;
	string mPrettyHealth;
	
	BaizGeneralRoadWin mBaizPanel = null;
	PickingGeneralWin mPickPanel = null;
	
	int idForRoad = -1;
	
	int m_id = -1;
	bool hasChanged = false;
	
	/// <summary>
	/// Cache the transform.
	/// </summary>
	void Awake() {
		
		if (PrettyRoadCol != null) {
			PrettyRoadCol.enabled = false;
		}
		if (PrettyHealthCol != null) {
			mPrettyHealth = PrettyHealthCol.text;
			PrettyHealthCol.enabled = false;
		}
		if (PrettyLevelCol != null) {
			mPrettyLevel = PrettyLevelCol.text;
			PrettyLevelCol.enabled = false;
		}
		if (ArmyLevelOrNumCol != null) {
			mArmyLevelOrNum = ArmyLevelOrNumCol.text;
			ArmyLevelOrNumCol.enabled = false;
			TroopIcon.gameObject.SetActiveRecursively(false);
		}
		if (PrettyNameCol != null) {
			PrettyNameCol.enabled = false;
		}
		if (Faceid != null) {
			Faceid.enabled = false;
		}
		if (Substrate != null) {
			Substrate.enabled = false;
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
				m_id = value;
				hasChanged = true;
			}
		}
	}
	
	override public void ResetItem()
	{
		hasChanged = true;
	}
	
	public int campid
	{
		get {
			return idForRoad;
		}
		
		set {
			
			if (idForRoad == value) return;
			
			idForRoad = value;
			if (PrettyRoadCol != null)
			{
				if (idForRoad <1) PrettyRoadCol.enabled = false;
				else if (idForRoad>5) 
				{
					PrettyRoadCol.text = "[*]";
					PrettyRoadCol.enabled = true;
				}
				else
				{
					PrettyRoadCol.text = string.Format("[{0}]", idForRoad);
					PrettyRoadCol.enabled = true;
				}
			}
		}
	}

	// Use this for initialization
	void Start () {
			
		if (mBaizPanel == null)
		{
			mBaizPanel = NGUITools.FindInParents<BaizGeneralRoadWin>(gameObject);
		}
		
		if (mPickPanel == null)
		{
			mPickPanel = NGUITools.FindInParents<PickingGeneralWin>(gameObject);
		}
	}
	
	
	public void ApplyGeneralItem(PickingGeneral item)
	{
		if (item == null)
		{
			if (PrettyRoadCol != null) {
				PrettyRoadCol.enabled = false;
			}
			if (PrettyLevelCol != null) {
				PrettyLevelCol.enabled = false;
			}
			if (Faceid != null) {
				Faceid.enabled = false;
			}
			if (ArmyLevelOrNumCol != null) {
				ArmyLevelOrNumCol.enabled = false;
				TroopIcon.gameObject.SetActiveRecursively(false);
			}
			if (PrettyHealthCol != null) {
				PrettyHealthCol.enabled = false;
			}
			if (PrettyNameCol != null) {
				PrettyNameCol.enabled = false;
			}
			if (Substrate != null) {
				Substrate.enabled = false;
			}
			return;
		}
	
		if (Faceid != null)
		{
			Faceid.enabled = true;
			Faceid.spriteName = U3dCmn.GetHeroIconName(item.nModel);
			Faceid.MakePixelPerfect();
		}
		
		if (PrettyNameCol != null)
		{
			PrettyNameCol.text = item.name;
			PrettyNameCol.enabled = true;
		}
		
		if (Substrate != null)
		{
			Substrate.enabled = true;
		}
		
		if (ArmyLevelOrNumCol != null)
		{
			uint idx = (uint)item.nArmyType;
			SoldierMBInfo info = U3dCmn.GetSoldierInfoFromMb(idx,1);
			ArmyLevelOrNumCol.text = string.Format(mArmyLevelOrNum, item.nArmyLevel, item.nArmyNum);
			ArmyLevelOrNumCol.enabled = true;
			TroopIcon.gameObject.SetActiveRecursively(true);
			TroopIcon.spriteName = U3dCmn.GetTroopIconFromID((int)info.ExcelID);
		}
		
		if (PrettyLevelCol != null)
		{
			PrettyLevelCol.text = string.Format(mPrettyLevel,item.nLevel);
			PrettyLevelCol.enabled = true;
		}
		
		if (PrettyHealthCol != null)
		{
			int hh = item.nHealthStatus;
			if (hh > 80) {
				PrettyHealthCol.text = string.Format("[00FF00]{0}{1}[-]",mPrettyHealth,hh);
			}
			else if (hh>50) {
				PrettyHealthCol.text = string.Format("[FFEE00]{0}{1}[-]",mPrettyHealth,hh);
			}
			else {
				PrettyHealthCol.text = string.Format("[EE0000]{0}{1}[-]",mPrettyHealth,hh);
			}

			PrettyHealthCol.enabled = true;
		}
	}
	
	void LateUpdate()
	{
		if (hasChanged == true)
		{
			hasChanged = false;
						
			PickingGeneral item = PickingGeneralManager.instance.GetItem(m_id);
			ApplyGeneralItem(item);
		}
	}
	
	void OnBaizGeneralsSelect()
	{
		if (mBaizPanel != null)
		{
			PickingGeneral item = PickingGeneralManager.instance.GetItem(m_id);
			if (item == null) return;
			if (false == item.campIn)
			{
				if (item.nArmyNum == 0)
				{
					int Tipset = BaizVariableScript.PICKING_GENERAL_NO_ARMY;
					string cc = U3dCmn.GetWarnErrTipFromMB(Tipset);
					string text = string.Format(cc, item.name);
					PopTipDialog.instance.VoidSetText2(true,false,text);
				}
				else if (true == mBaizPanel.OnActivateCampSlot(this))
				{
					item.campIn = true;
				}
			}
			else
			{
				mBaizPanel.OnDeactivateCampSlot(this);
				item.campIn = false;
			}
		}
	}
	
	void OnPickingGeneralClick()
	{
		if (mPickPanel != null)
		{
			PickingGeneral item = PickingGeneralManager.instance.GetItem(m_id);
			if (item == null) return;
			if (item.nArmyNum == 0)
			{
				int Tipset = BaizVariableScript.PICKING_GENERAL_NO_ARMY;
				string cc = U3dCmn.GetWarnErrTipFromMB(Tipset);
				string text = string.Format(cc, item.name);
				PopTipDialog.instance.VoidSetText2(true,false,text);
			}
			else
			{
				mPickPanel.OnApplyGeneralItem(item);
			}
		}
	}

}
