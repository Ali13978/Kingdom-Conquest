using UnityEngine;
using System.Collections;
using STCCMD;
public class DefenseHero : MonoBehaviour {
	public UISprite hero_icon;
	public UILabel 	hero_name;
	public UILabel	soldier_info;
	public UISprite troop_icon;
	public ulong heroID;
	string soldier_str;
	
	void Awake()
	{
		soldier_str = soldier_info.text;
	}
	// Use this for initialization
	void Start () {
	
	}
	void InitialData(ulong hero_id)
	{
		heroID = hero_id;
		if(hero_id!=0)
		{
			hero_icon.gameObject.SetActiveRecursively(true);
			HireHero hero_unit = (HireHero)JiangLingManager.MyHeroMap[hero_id];
			hero_icon.spriteName = U3dCmn.GetHeroIconName(hero_unit.nModel12);
			hero_name.text = DataConvert.BytesToStr(hero_unit.szName3)+" Lv"+hero_unit.nLevel19;
		
			if(CommonMB.SoldierMB_Map.Contains((uint)hero_unit.nArmyType9))
			{
				SoldierMBInfo soldierinfo= U3dCmn.GetSoldierInfoFromMb((uint)hero_unit.nArmyType9,(uint)hero_unit.nLevel19);
				soldier_info.text = string.Format(soldier_str,hero_unit.nArmyLevel10,hero_unit.nArmyNum11);
				troop_icon.spriteName = U3dCmn.GetTroopIconFromID((int)hero_unit.nArmyType9);
				//print("tttttttttttttt"+soldierinfo.ExcelID);
			}
			else 
			{
				soldier_info.text = "";
				troop_icon.spriteName = "transparent";
			}
		
		}
		else 
		{
			hero_icon.gameObject.SetActiveRecursively(false);
			hero_name.text ="";
			soldier_info.text = "";
			troop_icon.spriteName = "transparent";
		}
	}
}
