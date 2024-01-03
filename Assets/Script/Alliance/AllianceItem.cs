using UnityEngine;
using System.Collections;
using STCCMD;
public class AllianceItem : MonoBehaviour {
	public UILabel	rank_label;
	public UILabel	alliance_name_label;
	public UILabel	level_label;
	public UILabel	member_num_label;
	public UILabel	development_label;
	string level_str;
	string member_num_str;
	string development_str;
	AllianceCard now_unit;
	void Awake()
	{
		level_str = level_label.text;
		member_num_str = member_num_label.text;
		development_str = development_label.text;
	}
	// Use this for initialization
	void Start () {
	
	}

	//数据初始化 
	void InitialData(int index)
	{
		if(index>=0&&index<AllianceManager.AllianceList.Count)
		{
			AllianceCard unit = (AllianceCard)AllianceManager.AllianceList[index];
			rank_label.text = unit.nRank9.ToString();
			alliance_name_label.text = DataConvert.BytesToStr(unit.szAllianceName3);
			level_label.text = level_str+unit.nLevel10;
			member_num_label.text = member_num_str +" "+ unit.nMemberNum11;
			development_label.text = development_str+unit.nDevelopment7;
			now_unit = unit;
		}
		else
		{
			rank_label.text = "";
			alliance_name_label.text = "";
			level_label.text = "";
			member_num_label.text = "";
			development_label.text = "";
			now_unit.nAllianceID1 = 0;
		}
	}
	//打开联盟详细信息窗口 
	void OpenAllianceInfoWin()
	{
		if(now_unit.nAllianceID1 !=0)
		{
			GameObject obj  = U3dCmn.GetObjFromPrefab("AllianceInfoWin");
			if (obj != null)
			{
				obj.SendMessage("RevealPanelByParam",now_unit);
			}
		}
		
	}
}
