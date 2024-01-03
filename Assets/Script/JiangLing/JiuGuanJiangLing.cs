using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;

public class JiuGuanJiangLing : MonoBehaviour {
	public UILabel name;
	public UILabel attack;
	public UILabel defense;
	public UILabel life;
	public UILabel grow;
	public UIImageButton zhaomu_btn;
	public UISprite	head_icon;
	public UISlicedSprite senior_bg;
	public UISprite mingjiang_icon;
	public int		slotid;
	ulong 	now_hero_id;
	// Use this for initialization
	void Start () {
	
	}
	
	//初始化界面 
	void InitialUI(ulong hero_id)
	{
		if(JiuGuanInfoWin.JiangLingMap.Contains(hero_id))
		{
			now_hero_id = hero_id;
			RefreshHero heroinfo = (RefreshHero)JiuGuanInfoWin.JiangLingMap[hero_id];
			head_icon.spriteName = U3dCmn.GetHeroIconName(heroinfo.nModel9);
			
			name.text = DataConvert.BytesToStr(heroinfo.szName4);
			attack.text = heroinfo.nAttack6.ToString();
			defense.text = heroinfo.nDefense7.ToString();
			life.text = heroinfo.nHealth8.ToString();
			//grow.text =heroinfo.fGrow10.ToString(); // 显示成长上限 ...
			grow.text = string.Format("{0}/{1}",heroinfo.fGrow10,heroinfo.fGrowMax11);
			if(heroinfo.fGrow10 >5 )
			{
				mingjiang_icon.gameObject.SetActiveRecursively(true);
				senior_bg.gameObject.SetActiveRecursively(true);
			}
			else 
			{
				mingjiang_icon.gameObject.SetActiveRecursively(false);
				senior_bg.gameObject.SetActiveRecursively(false);
			}	
		}
	
	}
	//招募将领 
	void ZhaoMuJiangLing()
	{
		if(now_hero_id !=0)
		{
			CTS_GAMECMD_OPERATE_HIRE_HERO_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_HIRE_HERO;
			req.nHeroID4 = now_hero_id;
			req.nSlotID3 = (uint)slotid;
			//req.nSlotID3
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_HIRE_HERO_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_HIRE_HERO);
		}
		
	}
}
