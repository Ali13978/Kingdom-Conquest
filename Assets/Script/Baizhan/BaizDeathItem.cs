using UnityEngine;
using System.Collections;

public class BaizDeathItem : MonoBehaviour {
	
	public UILabel HeroNameCol = null;
	public UILabel HeroLevelCol = null;
	public UILabel ArmyNumCol = null;
	public UILabel ArmyLevelCol = null;
	public UILabel ArmyDeathCol = null;
	public UILabel ArmyNumRecoverCol = null;
	
	string mPrettyName;
	string mPrettyArmyLevel;
	string mPrettyArmyDeath;
	string mPrettyArmyNum;
	string mPrettyLevel;
	string mPrettyArmyNumRecover;
	
	
	void Awake() {

		if (HeroLevelCol != null)
		{
			mPrettyLevel = HeroLevelCol.text;
			HeroLevelCol.text = "";
		}
		if (ArmyNumCol != null)
		{
			mPrettyArmyNum = ArmyNumCol.text;
			ArmyNumCol.text = "";
		}
		if (ArmyDeathCol != null)
		{
			mPrettyArmyDeath = ArmyDeathCol.text;
			ArmyDeathCol.text = "";
		}
		if (ArmyLevelCol != null)
		{
			mPrettyArmyLevel = ArmyLevelCol.text;
			ArmyLevelCol.text = "";
		}
		if (ArmyNumRecoverCol != null)
		{
			mPrettyArmyNumRecover = ArmyNumRecoverCol.text;
			ArmyNumRecoverCol.text = "";
		}
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	
	public void SetDeathData(COMBAT_DEATH_INFO info)
	{
		if (HeroNameCol != null)
		{
			HeroNameCol.text = info.HeroName;
		}
		if (HeroLevelCol != null)
		{
			HeroLevelCol.text = string.Format(mPrettyLevel,info.nLevel);
		}
		if (ArmyLevelCol != null)
		{
			//Hashtable jlsosMap = CommonMB.SoldierMB_Map;
			
			uint armyId = (uint)info.nArmyType;
			//if (true == jlsosMap.ContainsKey(armyId))
			//{
				SoldierMBInfo g = U3dCmn.GetSoldierInfoFromMb(armyId,1);
				ArmyLevelCol.text = string.Format(mPrettyArmyLevel,info.nArmyLevel,g.Name);
			//}
		}
		if (ArmyNumCol != null)
		{
			ArmyNumCol.text = string.Format(mPrettyArmyNum, info.nArmyNum);
		}
		if (ArmyDeathCol != null)
		{
			ArmyDeathCol.text = string.Format(mPrettyArmyDeath, info.nArmyDeadNum);
		}
		if (ArmyNumRecoverCol != null)
		{
			ArmyNumRecoverCol.text = string.Format(mPrettyArmyNumRecover, info.nArmyNumRecover);
		}
	}
}
