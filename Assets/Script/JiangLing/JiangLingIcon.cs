using UnityEngine;
using System.Collections;
using STCCMD;
using CMNCMD;
public class JiangLingIcon : MonoBehaviour {
	public UILabel name;
	public UISprite headicon;
	public UILabel status;
	public UILabel soldier_label;
	public UILabel health_label;
	public UISlicedSprite select_bg;
	public UISprite troop_icon;
	public HireHero now_hero;

	string soldier_str;
	string health_str;
	void Awake()
	{
		health_str = health_label.text;
		soldier_str = soldier_label.text;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	//初始化将领信息 
	void InitialJiangLingInfo(HireHero heroinfo)
	{
		headicon.spriteName = U3dCmn.GetHeroIconName(heroinfo.nModel12);
		now_hero = heroinfo;
		name.text = DataConvert.BytesToStr(heroinfo.szName3)+" Lv"+heroinfo.nLevel19;
		if(heroinfo.nStatus14 == (int)HeroState.COMBAT_PVP_ATTACK || heroinfo.nStatus14 == (int)HeroState.COMBAT_PVE_RAID ||  
			heroinfo.nStatus14 == (int)HeroState.HONOR_CITY_ATTACK || heroinfo.nStatus14 == (int)HeroState.COMBAT_HERO_WORLD_RESOURCE)
		{
		
			status.color = new Color(0,1,1,1);
			status.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_IN_BATTLE);
		}
		else if(heroinfo.nStatus14 == (int)HeroState.COMBAT_INSTANCE_BAIZHANBUDAI)
		{
			status.color = new Color(0,1,1,1);
			status.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_BAIZHAN);
		}
		else if(heroinfo.nStatus14 == (int)HeroState.COMBAT_INSTANCE_NANZHENGBEIZHAN)
		{
			status.color = new Color(0,1,1,1);
			status.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_NANZHENG);
		}
		else if(heroinfo.nStatus14 == (int)HeroState.COMBAT_HERO_TRAINING)
		{
			status.color = new Color(0,1,1,1);
			status.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_IN_TRAIN);
		}
		else if(heroinfo.nStatus14 == (int)HeroState.COMBAT_INSTANCE_GUILD)
		{
			status.color = new Color(0,1,1,1);
			status.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GUILD_WAR);
		}
		else 
		{
			status.color = new Color(0,1,0,1);
			status.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_FREE);
		}
		if(CommonMB.SoldierMB_Map.Contains((uint)heroinfo.nArmyType9))
		{
			SoldierMBInfo soldierinfo= U3dCmn.GetSoldierInfoFromMb((uint)heroinfo.nArmyType9,(uint)heroinfo.nArmyLevel10);
			troop_icon.gameObject.SetActiveRecursively(true);
			troop_icon.spriteName = U3dCmn.GetTroopIconFromID((int)soldierinfo.ExcelID);
			soldier_label.text = string.Format(soldier_str,heroinfo.nArmyLevel10,heroinfo.nArmyNum11);
		}
		else
		{
			soldier_label.text = "";
			
			troop_icon.gameObject.SetActiveRecursively(false);
		}
		
		if (heroinfo.nHealthState21 > 80) {
			health_label.text = string.Format("[00FF00]{0}{1}[-]",health_str,heroinfo.nHealthState21);
		}
		else if (heroinfo.nHealthState21>50) {
			health_label.text = string.Format("[FFEE00]{0}{1}[-]",health_str,heroinfo.nHealthState21);
		}
		else {
			health_label.text = string.Format("[EE0000]{0}{1}[-]",health_str,heroinfo.nHealthState21);
		}
		//health_label.text = ;
		select_bg.gameObject.SetActiveRecursively(false);
	} 
	
	//是否选中 
	public void SetSelect(bool isselect)
	{
		if(isselect)
			select_bg.gameObject.SetActiveRecursively(true);
		else 
			select_bg.gameObject.SetActiveRecursively(false);
	}
}
