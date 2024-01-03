using UnityEngine;
using System.Collections;
using CMNCMD;
using STCCMD;
using CTSCMD;
public class RankItem : MonoBehaviour {
	public UILabel	rank_label;
	public UILabel 	level_label;
	public UILabel  char_name;
	public UISprite head_icon;
	public UILabel  data_label;
	
	string rank_str;
	string level_str;
	string data_level_str;
	string data_diamond_str;
	string data_gold_str;
	
	CharRankInfo now_unit;
	void Awake()
	{
		rank_str = rank_label.text;
		level_str = level_label.text;
		string[] str_array = data_label.text.Split('|');
		data_level_str = str_array[0];
		data_diamond_str = str_array[1];
		data_gold_str = str_array[2];
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void InitialData(CharRankInfo unit)
	{
		now_unit = unit;
		if(unit.nAccountID1 !=0)
		{
			char_name.text = DataConvert.BytesToStr(unit.szName3);
			rank_label.text = string.Format(rank_str,unit.nRank6);
			level_label.text = level_str+unit.nLevel4;
			if(RankManager.rank_type == (uint)RANK_TYPE.rank_type_char_level)
			{
				data_label.text = data_level_str+unit.nData7;
			}
			else if(RankManager.rank_type == (uint)RANK_TYPE.rank_type_char_diamond)
			{
				data_label.text = data_diamond_str+unit.nData7;
			}
			else if(RankManager.rank_type == (uint)RANK_TYPE.rank_type_char_gold)
			{
				data_label.text = data_gold_str+unit.nData7;
			}
			head_icon.spriteName = U3dCmn.GetCharIconName((int)unit.nHeadID5);
		}
		else 
		{
			char_name.text = "";
			rank_label.text = "";
			level_label.text = "";
			data_label.text = "";
			head_icon.spriteName = "transparent";
		}
	}

	//打开玩家简单信息面板 
	void OpenPlayerCardWin()
	{
		if(now_unit.nAccountID1 != 0)
		{
			GameObject obj  = U3dCmn.GetObjFromPrefab("PlayerCardWin");
			if (obj != null)
			{
				obj.SendMessage("RevealPanel",now_unit.nAccountID1);
			}
		}
	}
}	
