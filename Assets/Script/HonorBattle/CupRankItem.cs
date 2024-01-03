using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class CupRankItem : MonoBehaviour {
	public UILabel	rank_label;
	public UILabel 	level_label;
	public UILabel  name_label;
	public UISprite head_icon;
	public UILabel  data_label;
	
	string rank_str;
	string level_str;
	string data_str;
	
	WorldCityRankMan personal_rank_data;
	WorldCityRankAlliance alliance_rank_data;
	void Awake()
	{
		rank_str = rank_label.text;
		rank_label.enabled = false;
		level_str = level_label.text;
		level_label.enabled = false;
		data_str = data_label.text;
		data_label.enabled = false;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//初始化个人奖杯数据 
	void InitialPersonalData(WorldCityRankMan unit)
	{
		personal_rank_data = unit;
		alliance_rank_data.nAllianceID2 = 0;
		if(unit.nAccountID2 !=0)
		{
			name_label.text = DataConvert.BytesToStr(unit.szName4);
			name_label.enabled = true;
			rank_label.text = string.Format(rank_str,unit.nID1);
			rank_label.enabled = true;
			level_label.text = level_str+unit.nLevel6;
			level_label.enabled = true;
			data_label.text = data_str+unit.nCup7;
			data_label.enabled = true;
			head_icon.spriteName = U3dCmn.GetCharIconName((int)unit.nHeadID5);
		}
		else 
		{
			name_label.enabled = false;
			rank_label.enabled = false;
			level_label.enabled = false;
			data_label.enabled = false;
			head_icon.spriteName = "transparent";
		}
	}
	//初始化联盟奖杯数据  
	void InitialAllianceData(WorldCityRankAlliance unit)
	{
		alliance_rank_data = unit;
		personal_rank_data.nAccountID2 = 0;
		if(unit.nAllianceID2 !=0)
		{
			name_label.text = DataConvert.BytesToStr(unit.szName4);
			name_label.enabled = true;
			rank_label.text = string.Format(rank_str,unit.nID1);
			rank_label.enabled = true;
			level_label.text = level_str+unit.nLevel5;
			level_label.enabled = true;
			data_label.text = data_str+unit.nCup6;
			data_label.enabled = true;
		}
		else 
		{
			name_label.enabled = false;
			rank_label.enabled = false;
			level_label.enabled = false;
			data_label.enabled = false;
		}
		
		head_icon.spriteName = "transparent";
	}
	//打开玩家简单信息面板 
	void OpenInfoWin()
	{
		if(personal_rank_data.nAccountID2 != 0)
		{
			GameObject obj  = U3dCmn.GetObjFromPrefab("PlayerCardWin");
			if (obj != null)
			{
				obj.SendMessage("RevealPanel",personal_rank_data.nAccountID2);
			}
		}
		else if(alliance_rank_data.nAllianceID2 !=0)
		{
			GameObject obj  = U3dCmn.GetObjFromPrefab("AllianceInfoWin");
			if (obj != null)
			{
				obj.SendMessage("RevealPanel",alliance_rank_data.nAllianceID2);
			}
		}
	}
}
