using UnityEngine;
using System.Collections;
using STCCMD;
using CTSCMD;
using CMNCMD;

public class MemberItem : MonoBehaviour {
	public UILabel	char_name_label;
	public UILabel	level_label;
	public UILabel	positon_label;
	public UILabel	contribution_label;
	public UILabel  honor_cup;
	
	AllianceMemberUnit now_unit;
	// Use this for initialization
	void Start () {
	
	}
	
	
	void InitialData(AllianceMemberUnit unit)
	{
		now_unit = unit;
		if(unit.nAccountID1 == 0)
		{
			
			char_name_label.gameObject.SetActiveRecursively(false);
			level_label.gameObject.SetActiveRecursively(false);
			positon_label.gameObject.SetActiveRecursively(false);
			contribution_label.gameObject.SetActiveRecursively(false);
			honor_cup.text = "";
		}
		else 
		{
			char_name_label.text =  DataConvert.BytesToStr(unit.szName3);
			level_label.text = unit.nLevel7.ToString();
			positon_label.text = AllianceManager.GetStrFromPositionID(unit.nPosition4);
			contribution_label.text = unit.nTotalDevelopment6.ToString();
			honor_cup.text = unit.nCup8.ToString();
			char_name_label.gameObject.SetActiveRecursively(true);
			level_label.gameObject.SetActiveRecursively(true);
			positon_label.gameObject.SetActiveRecursively(true);
			contribution_label.gameObject.SetActiveRecursively(true);
			
		}
	}
	void OpenMemberInfoWin()
	{
		if(now_unit.nAccountID1 != 0)
		{
			GameObject obj  = U3dCmn.GetObjFromPrefab("AllianceMemberInfoWin");
			if (obj != null)
			{
				obj.SendMessage("RevealPanel",now_unit);
			}
		}
	
	}
}
