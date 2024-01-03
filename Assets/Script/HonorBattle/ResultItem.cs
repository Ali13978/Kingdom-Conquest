using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;

public class ResultItem : MonoBehaviour {

	public UILabel	rank_label;
	public UILabel  name_label;
	public UISprite head_icon;
	public UILabel  diamond_label;
	public UILabel 	cup_label;
	
	string rank_str;
	string diamond_str;
	string cup_str;
	
	WorldCityLog personal_result_data;
	WorldCityLogAlliance alliance_result_data;
	void Awake()
	{
		rank_str = rank_label.text;
		diamond_str = diamond_label.text;
		cup_str = cup_label.text;
		rank_label.enabled = false;
		diamond_label.enabled = false;
		cup_label.enabled =  false;
		name_label.enabled = false;
		
		
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//初始化个人奖杯数据 
	void InitialPersonalData(WorldCityLog unit)
	{
		personal_result_data = unit;
		alliance_result_data.nAllianceID2 = 0;
		if(unit.nAccountID2 !=0)
		{
			name_label.text = DataConvert.BytesToStr(unit.szName4);
			rank_label.text = string.Format(rank_str,unit.nID1);
			diamond_label.text = diamond_str+unit.nDiamond6;
			cup_label.text = cup_str+ unit.nCup7;
			head_icon.spriteName = U3dCmn.GetCharIconName((int)unit.nHeadID5);
			rank_label.enabled = true;
			diamond_label.enabled = true;
			cup_label.enabled =  true;
			name_label.enabled = true;
		}
		else 
		{
			name_label.enabled = false;
			rank_label.enabled = false;
			diamond_label.enabled = false;
			cup_label.enabled = false;
			head_icon.spriteName = "transparent";
		}
	}
	//初始化联盟奖杯数据  
	void InitialAllianceData(WorldCityLogAlliance unit)
	{
		alliance_result_data = unit;
		personal_result_data.nAccountID2 = 0;
		if(unit.nAllianceID2 !=0)
		{
			name_label.text = DataConvert.BytesToStr(unit.szName4);
			rank_label.text = string.Format(rank_str,unit.nID1);
			diamond_label.text = diamond_str+unit.nDiamond5;
			cup_label.text = cup_str+ unit.nCup6;
			rank_label.enabled = true;
			diamond_label.enabled = true;
			cup_label.enabled =  true;
			name_label.enabled = true;
		}
		else 
		{
			name_label.enabled = false;
			rank_label.enabled = false;
			diamond_label.enabled = false;
			cup_label.enabled = false;
		}
		
		head_icon.spriteName = "transparent";
	}
	//打开玩家简单信息面板 
	void OpenInfoWin()
	{
		if(personal_result_data.nAccountID2 != 0)
		{
			GameObject obj  = U3dCmn.GetObjFromPrefab("PlayerCardWin");
			if (obj != null)
			{
				obj.SendMessage("RevealPanel",personal_result_data.nAccountID2);
			}
		}
		else if(alliance_result_data.nAllianceID2 != 0)
		{
			GameObject obj  = U3dCmn.GetObjFromPrefab("AllianceInfoWin");
			if (obj != null)
			{
				obj.SendMessage("RevealPanel",alliance_result_data.nAllianceID2);
			}
		}
	}
}
