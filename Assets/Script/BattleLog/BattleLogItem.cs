using UnityEngine;
using System.Collections;
using STCCMD; 
using CMNCMD;
public class BattleLogItem : MonoBehaviour {
	public UILabel hero_name_label;
	public UILabel hero_level_label;
	public UILabel troop_desc_label;
	public UILabel troop_num_label;
	public UILabel troop_death_label;
	public UILabel troop_recover_label;
	
	
	string hero_level_str;
	string troop_desc_str;
	string troop_num_str;
	string troop_death_str;
	string troop_recover_str;
	
	void Awake() {
		hero_level_str = hero_level_label.text;
		//hero_level_label.text = "";
		
		troop_desc_str = troop_desc_label.text;
		//troop_desc_label.text = "";
	
		troop_num_str = troop_num_label.text;
		//troop_num_label.text = "";
		
		troop_death_str = troop_death_label.text;
		//troop_death_label.text = "";
		
		troop_recover_str = troop_recover_label.text;
		//troop_recover_label.text = "";
		
	
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//初始化战报数据（带有数据的）  
	public void InitialHeroData(HeroUnit info)
	{
		hero_name_label.text = DataConvert.BytesToStr(info.szHeroName2);
		hero_level_label.text = string.Format(hero_level_str,info.nHeroLevel3);
			
		SoldierMBInfo troop_info = U3dCmn.GetSoldierInfoFromMb(info.nArmyType7,1);
		troop_desc_label.text = string.Format(troop_desc_str,info.nArmyLevel8,troop_info.Name);
		troop_num_label.text = string.Format(troop_num_str,info.nArmyNum9);
		troop_death_label.text = string.Format(troop_death_str,info.nArmyDeathNum10);
		troop_recover_label.text = string.Format(troop_recover_str,info.nArmyRestoreNum11);
	}
	//初始化战报君主信息 用来分割战报 
	public void InitialCharData(string char_name,uint force)
	{
		hero_name_label.text = "[00FF00]"+char_name+"[-]";
		hero_level_label.text = "[00FF00]"+string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.LORD_FORCE),force)+"[-]";
		troop_desc_label.text = "";
		troop_num_label.text = "";
		troop_death_label.text = "";
		troop_recover_label.text = "";
	}
}
