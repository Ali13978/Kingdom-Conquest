using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class HeroStrengthenWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	//将领基本信息 
	public UISprite 	hero_icon;
	public UILabel		hero_name;
	public UILabel 		hero_grow;
	public UILabel 		hero_top_grow;
	public UILabel 		attack_label;
	public UILabel 		defense_label;
	public UILabel 		life_label;
	public UILabel 		leader_label;
	
	public UILabel		describe_label;
	public UIImageButton 	strengthen_btn;
	string describe;
	HireHero now_hero;
	ulong item_id = 0;
	
	// Use this for initialization
	void Awake()
	{
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			transform.localScale = CommonData.iphonescale;
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			transform.localScale = CommonData.ipadscale;
		}
		describe = describe_label.text;
	}
	
	void Start () {
	
	}
	
	//弹出窗口   
	void RevealPanel(HireHero hero_unit)
	{
		InitialPanelData(hero_unit);
		tween_position.Play(true);
	}
	//关闭窗口   
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//初始化数据 
	void InitialPanelData(HireHero hero_unit)
	{
		now_hero = hero_unit;
		if(hero_unit.nAttackAdd15>0)
			attack_label.text = (hero_unit.nAttack5).ToString()+"[009600](+"+hero_unit.nAttackAdd15+")";
		else 
			attack_label.text = (hero_unit.nAttack5).ToString();
		if(hero_unit.nDefenseAdd16>0)
			defense_label.text = (hero_unit.nDefense6).ToString()+"[009600](+"+hero_unit.nDefenseAdd16+")";
		else
			defense_label.text = (hero_unit.nDefense6).ToString();
		if(hero_unit.nHealthAdd17>0)
			life_label.text = (hero_unit.nHealth7).ToString()+"[009600](+"+hero_unit.nHealthAdd17+")";
		else
			life_label.text = (hero_unit.nHealth7).ToString();
		if(hero_unit.nLeaderAdd18>0)
			leader_label.text = (hero_unit.nLeader13).ToString()+"[009600](+"+hero_unit.nLeaderAdd18+")";
		else 
			leader_label.text = (hero_unit.nLeader13).ToString();
		
		//attack_label.text = hero_unit.nAttack5.ToString()+"[006400](+"+hero_unit.nAttackAdd15+")";
		//defense_label.text = hero_unit.nDefense6.ToString()+"[006400](+"+hero_unit.nDefenseAdd16+")";
		//life_label.text = hero_unit.nHealth7.ToString()+"[006400](+"+hero_unit.nHealthAdd17+")";
		//leader_label.text = hero_unit.nLeader13.ToString()+"[006400](+"+hero_unit.nLeaderAdd18+")";
		// 将领的成长上限 ...
		hero_top_grow.text = string.Format("{0}",hero_unit.fGrowMax23);
		//hero_top_grow.text = CommonMB.HeroTopGrow.ToString();
		ItemDesc item_info = CangKuManager.GetItemInfoByExcelID((int)ARTICLES.YINGXIONGCHENGZHANG);
		hero_icon.spriteName =  U3dCmn.GetHeroIconName(hero_unit.nModel12);
		hero_name.text = DataConvert.BytesToStr(hero_unit.szName3);
		hero_grow.text = hero_unit.fGrow8.ToString();
		describe_label.text = string.Format(describe,item_info.nNum3);
		item_id = item_info.nItemID1;
		if(item_info.nNum3 <=0)
		{
			describe_label.color = new Color(1,0,0,1);
			strengthen_btn.gameObject.SetActiveRecursively(false);
		}
		else 
		{
			describe_label.color = new Color(0,0.4f,0,1);
			strengthen_btn.gameObject.SetActiveRecursively(true);
		}
	}
	//强化武将 
	void ReqStrengthenHero()
	{
		
		if(now_hero.fGrow8 < now_hero.fGrowMax23)
		{
			CangKuManager.UseArticlesItem((int)ARTICLES.YINGXIONGCHENGZHANG,item_id,now_hero.nHeroID1,1);
		}
		else
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_TO_TOP_GROW));
		}
		
	}
	//强化武将返回 
	void ReqStrengthenHeroRst(HireHero hero_unit)
	{
		//print ("qianghua"+grow);
		//hero_grow.text = grow.ToString();
		//CangKuManager.SubItem(item_id,1);
		InitialPanelData(hero_unit);
	}
}
