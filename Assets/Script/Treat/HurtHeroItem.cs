using UnityEngine;
using System.Collections;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class HurtHeroItem : MonoBehaviour {
	public GameObject hero_obj;
	public UISprite head_icon;
	public UILabel	hero_name;
	public UILabel	hero_level;
	public UILabel	health;
	public UILabel need_caoyao;
	string health_str;
	int need_caoyao_num;
	ulong 	now_hero_id;
	void Awake()
	{
		health_str = health.text;		
	}
	// Use this for initialization
	void Start () {
	
	}
	
	//初始化受伤将领信息 
	void InitialData(ulong hero_id)
	{
		now_hero_id= hero_id;
		if(JiangLingManager.MyHeroMap.Contains(hero_id))
		{
			hero_obj.SetActiveRecursively(true);
			HireHero hero_info =  (HireHero)JiangLingManager.MyHeroMap[hero_id];
			head_icon.spriteName = U3dCmn.GetHeroIconName(hero_info.nModel12);
			hero_name.text = DataConvert.BytesToStr(hero_info.szName3);
			hero_level.text = "Lv "+hero_info.nLevel19;
			health.text = hero_info.nHealthState21.ToString();
			
			if (hero_info.nHealthState21 > 80) {
				health.text = string.Format("[00FF00]{0}{1}[-]",health_str,hero_info.nHealthState21);
			}
			else if (hero_info.nHealthState21>50) {
				health.text = string.Format("[FFEE00]{0}{1}[-]",health_str,hero_info.nHealthState21);
			}
			else {
				health.text = string.Format("[EE0000]{0}{1}[-]",health_str,hero_info.nHealthState21);
			}
			need_caoyao_num = TreatManager.CalNeedCaoYao(hero_info.nHealthState21,hero_info.nLevel19);
			need_caoyao.text = need_caoyao_num.ToString();
		}
		else
		{
			hero_obj.SetActiveRecursively(false);
		}
	}
	//治疗受伤将领 
	void TreatHero()
	{
		int my_caoyao_nym = (int)CommonData.player_online_info.CaoYao;
		int use_caoyao_num = need_caoyao_num>my_caoyao_nym?my_caoyao_nym:need_caoyao_num;
		if(use_caoyao_num<=0)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CAOYAO_NOT_ENOUGH)); 
			return ;
		}
		ulong[] hero_array = {now_hero_id};
		TreatManager.ReqTreatHero(hero_array,use_caoyao_num);
		//CangKuManager.UseArticlesItem((int)ARTICLES.CAOYAO,0,now_hero_id,use_caoyao_num);
	}
}
