using UnityEngine;
using System.Collections;

public class NanBeizhanGeneralItem : MonoBehaviour {
	
	public UISprite Faceid = null;
	public UILabel NameOrLevelCol = null;
	public UILabel ArmyLevelOrNumCol = null;
	public UILabel PrettyHealthCol = null;
	public UILabel HeroPowerCol = null;
	public UILabel AllyCol = null;
	public UISprite Leaveins = null;
	public UISprite TroopIcon;
	public int AllyID = -1;
	
	string mPrettyLevel;
	string mArmyLevelOrNum;
	string mPrettyHealth;
	string mPrettyPower;
	
	void Awake() {
		if (NameOrLevelCol != null) {
			mPrettyLevel = NameOrLevelCol.text;
			NameOrLevelCol.enabled = false;
		}
		if (ArmyLevelOrNumCol != null) {
			mArmyLevelOrNum = ArmyLevelOrNumCol.text;
			ArmyLevelOrNumCol.enabled = false;
			TroopIcon.gameObject.SetActiveRecursively(false);
		}
		if (HeroPowerCol != null) {
			mPrettyPower = HeroPowerCol.text;
			HeroPowerCol.enabled = false;
		}
		if (PrettyHealthCol != null) {
			mPrettyHealth = PrettyHealthCol.text;
			PrettyHealthCol.enabled = false;
		}
		if (AllyCol != null) {
			AllyCol.enabled = false;
		}
		if (Faceid != null) {
			Faceid.enabled = false;
		}
		if (Leaveins != null) {
			Leaveins.enabled = false;
		}
	}
	
	void ForceBake()
	{
		if (NameOrLevelCol != null) {
			NameOrLevelCol.enabled = false;
		}
		if (Faceid != null) {
			Faceid.enabled = false;
		}
		if (Leaveins != null) {
			Leaveins.enabled = false;
		}
		if (ArmyLevelOrNumCol != null) {
			ArmyLevelOrNumCol.enabled = false;
			TroopIcon.gameObject.SetActiveRecursively(false);
		}
		if (PrettyHealthCol != null) {
			PrettyHealthCol.enabled = false;
		}
		if (HeroPowerCol != null) {
			HeroPowerCol.enabled = false;
		}
		if (AllyCol != null) {
			AllyCol.enabled = false;
		}
	}
	
	public void TurnLeaveins()
	{
		if (Leaveins != null) 
		{ 
			bool force = Leaveins.enabled;
			Leaveins.enabled = (force == false);
		}
	}
	
	public void ApplyGeneralItem(PickingGeneral item)
	{
		if (item == null)
		{
			ForceBake();
			return;
		}
	
		if (Faceid != null)
		{
			Faceid.enabled = true;
			Faceid.spriteName = U3dCmn.GetHeroIconName(item.nModel);
			Faceid.MakePixelPerfect();
		}
		
		if (ArmyLevelOrNumCol != null)
		{
			uint nExcelID = (uint)item.nArmyType;
			//Hashtable hireMap = CommonMB.SoldierMB_Map;			
			SoldierMBInfo info = U3dCmn.GetSoldierInfoFromMb(nExcelID,1);
			ArmyLevelOrNumCol.text = string.Format(mArmyLevelOrNum, item.nArmyLevel, item.nArmyNum);
			ArmyLevelOrNumCol.enabled = true;
			TroopIcon.gameObject.SetActiveRecursively(true);
			TroopIcon.spriteName = U3dCmn.GetTroopIconFromID((int)info.ExcelID);
		}
		
		if (HeroPowerCol != null)
		{
			HeroPowerCol.text = string.Format(mPrettyPower, item.nProf);
			HeroPowerCol.enabled = true;
		}
		
		if (NameOrLevelCol != null)
		{
			NameOrLevelCol.text = string.Format(mPrettyLevel,item.name, item.nLevel);
			NameOrLevelCol.enabled = true;
		}
		
		if (AllyCol != null)
		{
			AllyCol.text = item.charName;
			AllyCol.enabled = true;
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

}
