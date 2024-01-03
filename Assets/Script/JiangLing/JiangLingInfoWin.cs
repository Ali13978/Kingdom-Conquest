using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class JiangLingInfoWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	public UIGrid 			JiangLingGrid;
	public GameObject 		JiangLingIcon;
	//将领详细信息面板 
	public GameObject JiangLingInfoPanel;
	public UILabel hero_num_title;
	public UILabel hero_num_label;
	public UILabel hero_num_info;
	
	public UILabel name_label;
	public UILabel attack_label;
	public UILabel defense_label;
	public UILabel life_label;
	public UILabel leader_label;
	
	// 当前未招募武将提示 ...
	public UILabel no_hero_tip;
	//public UILabel combat_power_label;
	public UILabel combat_power;
	public UILabel level_label;
	public UILabel grow_label;
	public UILabel exp_label;
	public UISprite head_icon;
	public UISlicedSprite panel_bg;
	//当前选择的将领单元 
	HireHero select_hero;
	public static GameObject select_hero_icon;
	public delegate void refreshHeroData();
	public static refreshHeroData RefreshHeroData;
	List<GameObject> JiangLingObjList = new List<GameObject>();
	//string soldier_format;
	//public static ApplyBaizCampDataDelegate applyBaizCampDataDelegate = null; 
	//已招募将领的MAP表
	
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
		
		// 没有招募武将提示 ...
		if (no_hero_tip != null)
		{
			no_hero_tip.enabled = false;
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	//弹出窗口   
	void RevealPanel()
	{
		JiangLingGrid.gameObject.SetActiveRecursively(false);
		JiangLingInfoPanel.gameObject.SetActiveRecursively(false);
		panel_bg.gameObject.SetActiveRecursively(false);
		hero_num_label.text = "";
		hero_num_title.gameObject.SetActiveRecursively(false);
		tween_position.Play(true);
		InitialJiangLingList();
	}
	//关闭窗口   
	void DismissPanel()
	{
		foreach(Transform obj in JiangLingGrid.gameObject.transform)
		{
			Destroy(obj.gameObject);
		}
		//百战不殆等副本配兵回调函数 
		if(RefreshHeroData != null)
			RefreshHeroData();
		
		RefreshHeroData = null;
		//给新手引导一个通知 ...
		tween_position.Play(false);
		Destroy(gameObject);
	}
	
	//初始化将领列表界面   
	void InitialJiangLingList()
	{
		int num = JiangLingManager.MyHeroMap.Count;
		if (num<1)
		{
			if (no_hero_tip != null) 
			{
				no_hero_tip.text = U3dCmn.GetWarnErrTipFromMB(331);
				no_hero_tip.enabled = true;
			}
			
			return ;
		}
		
		select_hero_icon = null;
		JiangLingGrid.gameObject.SetActiveRecursively(true);
		
		if(num>0)
		{
			JiangLingInfoPanel.gameObject.SetActiveRecursively(true);
			panel_bg.gameObject.SetActiveRecursively(true);
			hero_num_title.gameObject.SetActiveRecursively(true);
			int hero_max_num = U3dCmn.GetHeroMaxNumByCharLevel(CommonData.player_online_info.Level);//(int)CommonMB.HeroNum_Map[CommonData.player_online_info.Level];
			hero_num_label.text = num+"/"+hero_max_num;
			uint next_level = U3dCmn.GetNextCharLevelForHero(CommonData.player_online_info.Level);
			int next_hero_num = 0;
			if(next_level!=0)
			{
			 	next_hero_num = (int)CommonMB.HeroNum_Map[next_level];
				hero_num_info.text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.LORD_NEX_HERO_NUM),next_level,next_hero_num);
			}
			else 
			{
				next_hero_num = hero_max_num;
				hero_num_info.text = "";
			}
			
			//SetJiangLingInfoPanelData(jiangling_list[0]);
		}
		num =1;
		ArrayList list = new ArrayList(JiangLingManager.MyHeroMap.Keys);
      	list.Sort();
		JiangLingObjList.Clear();
		for(int i=0; i<list.Count;i++)
		{
			HireHero hero_unit = (HireHero)JiangLingManager.MyHeroMap[list[i]];
			GameObject jiangling_icon = NGUITools.AddChild(JiangLingGrid.gameObject, JiangLingIcon);
			jiangling_icon.name = "jianglingicon"+num;
			UIEventListener.Get(jiangling_icon).onClick -= ShowDetail;
			UIEventListener.Get(jiangling_icon).onClick += ShowDetail;
			JiangLingObjList.Add(jiangling_icon);
			jiangling_icon.SendMessage("InitialJiangLingInfo",hero_unit);
			if(num == 1)
			{
				//select_hero_icon = jiangling_icon;
				SetJiangLingInfoPanelData(hero_unit);
			}
			num++;
		}
		JiangLingGrid.Reposition();
	}
	//配兵后 刷新将领列表界面 
	//void RefreshUI(HireHero refreshinfo)
	//{
	//	select_hero_icon.GetComponent<JiangLingIcon>().SetSelect(true);
	//	SetJiangLingInfoPanelData(refreshinfo);
	//}
	//选中将领 
	//选中显示 
	void ShowDetail(GameObject obj)
	{
		select_hero_icon = obj;
		SetJiangLingInfoPanelData(obj.GetComponent<JiangLingIcon>().now_hero);
	}
	//显示将领详细信息面板 
	void SetJiangLingInfoPanelData(HireHero heroinfo)
	{
		//print("sssssssssssssssssssssss");
		//JiangLingDragPanel.Scroll(1.0f);
		JiangLingInfoPanel.gameObject.SetActiveRecursively(true);
		select_hero = heroinfo;
		name_label.text = DataConvert.BytesToStr(heroinfo.szName3);
		if(heroinfo.nAttackAdd15>0)
			attack_label.text = (heroinfo.nAttack5).ToString()+"[009600](+"+heroinfo.nAttackAdd15+")";
		else 
			attack_label.text = (heroinfo.nAttack5).ToString();
		if(heroinfo.nDefenseAdd16>0)
			defense_label.text = (heroinfo.nDefense6).ToString()+"[009600](+"+heroinfo.nDefenseAdd16+")";
		else
			defense_label.text = (heroinfo.nDefense6).ToString();
		if(heroinfo.nHealthAdd17>0)
			life_label.text = (heroinfo.nHealth7).ToString()+"[009600](+"+heroinfo.nHealthAdd17+")";
		else
			life_label.text = (heroinfo.nHealth7).ToString();
		if(heroinfo.nLeaderAdd18>0)
			leader_label.text = (heroinfo.nLeader13).ToString()+"[009600](+"+heroinfo.nLeaderAdd18+")";
		else 
			leader_label.text = (heroinfo.nLeader13).ToString();
		head_icon.spriteName = U3dCmn.GetHeroIconName(heroinfo.nModel12);
		if(heroinfo.nLevel19 >= CommonData.player_online_info.Level)
		{
			level_label.text = heroinfo.nLevel19.ToString()+"("+ U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_TOP_LEVEL)+ ")";
			
		}
		else 
		{
			level_label.text = heroinfo.nLevel19.ToString();
		}
		
		// 显示将领的成长上限 ...
		grow_label.text = string.Format("{0}/{1}",heroinfo.fGrow8,heroinfo.fGrowMax23);
		//grow_label.text = heroinfo.fGrow8.ToString();
		//int next_level_exp = 0;
		if(CommonMB.HeroExpGold_Map.Contains(heroinfo.nLevel19+1))
		{
			HeroExpGoldInfo info =  (HeroExpGoldInfo)CommonMB.HeroExpGold_Map[heroinfo.nLevel19+1];
			exp_label.text = heroinfo.nExp22+"/"+info.LevelUpExp;
		}
		else 
		{
			exp_label.text = "0/0";
		}
		//HeroExpGoldInfo info =  (HeroExpGoldInfo)CommonMB.HeroExpGold_Map[heroinfo.nLevel19+1];
		//exp_label.text = heroinfo.nExp22+"/"+info.LevelUpExp;
		if(CommonMB.SoldierMB_Map.Contains((uint)heroinfo.nArmyType9))
		{
			SoldierMBInfo soldierinfo= U3dCmn.GetSoldierInfoFromMb((uint)heroinfo.nArmyType9,(uint)heroinfo.nArmyLevel10);
			//army_label.text = string.Format(soldier_format,heroinfo.nArmyLevel10,soldierinfo.Name);
			combat_power.text = heroinfo.nProf20.ToString();
			
		}
		else
		{
			//army_label.text = "";
			combat_power.text = "";
		}
		//选中相应的将领 
		for(int i=0;i<JiangLingObjList.Count;i++)
		{
			if(JiangLingObjList[i].GetComponent<JiangLingIcon>().now_hero.nHeroID1 == heroinfo.nHeroID1)
			{
				
				if(select_hero_icon!=null)
				{
					JiangLingObjList[i].SendMessage("InitialJiangLingInfo",heroinfo);
					JiangLingObjList[i].GetComponent<JiangLingIcon>().SetSelect(true);
				}
					
				JiangLingObjList[i].GetComponent<JiangLingIcon>().now_hero = heroinfo;
				select_hero_icon = JiangLingObjList[i];
				
			}	
			else 
				JiangLingObjList[i].GetComponent<JiangLingIcon>().SetSelect(false);
		}
		
		//army_num_label.text = ;
	}
	//弹出配兵界面 
	void OpenWithSoldierWin()
	{
		if(select_hero.nStatus14 != (int)HeroState.NORMAL)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_NOT_FREE)); 
			return ;
		}
		GameObject win = U3dCmn.GetObjFromPrefab("WithSoldierWin");
		if(win != null)
		{
			win.SendMessage("RevealPanel",select_hero);
		}
		//select_hero
	}
	//解雇将领 
	void DismissJiangLing()
	{
		string str = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.SRUE_DISMISS_HERO);
		PopConfirmWin.instance.ShowPopWin(str,SrueDismissJiangLing);
	}
	void SrueDismissJiangLing(GameObject obj)
	{
		CTS_GAMECMD_OPERATE_FIRE_HERO_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_FIRE_HERO;
		req.nHeroID3 = select_hero.nHeroID1;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_FIRE_HERO_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_FIRE_HERO);
	}
	//解雇将领返回 
	void ProcessDismissJiangLingRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_FIRE_HERO_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_FIRE_HERO_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_FIRE_HERO_T.enum_rst.RST_OK)
		{
			if(JiangLingManager.MyHeroMap.Contains(select_hero.nHeroID1))
			{
				JiangLingManager.MyHeroMap.Remove(select_hero.nHeroID1);
			}
			for(int i=0;i<JiangLingObjList.Count;i++)
			{
				if(JiangLingObjList[i].GetComponent<JiangLingIcon>().now_hero.nHeroID1 == select_hero.nHeroID1)
				{
					JiangLingObjList.Remove(JiangLingObjList[i]);
					break;
				}
			}
			select_hero_icon.active = false;
			NGUITools.Destroy(select_hero_icon);
			JiangLingGrid.Reposition();
			JiangLingInfoPanel.gameObject.SetActiveRecursively(false);
			//InitialJiangLingList();
			select_hero_icon = null;
			select_hero = new HireHero();
			
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_FIRE_HERO_T.enum_rst.RST_EQUIP)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_IS_EQUIPED));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_FIRE_HERO_T.enum_rst.RST_CITYDEFENSE)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_IS_DEFENSE));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_FIRE_HERO_T.enum_rst.RST_ARMY)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_HAS_SOLDIER));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_FIRE_HERO_T.enum_rst.RST_WORLDGOLDMINE)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_IN_WORLDMINE));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_FIRE_HERO_T.enum_rst.RST_INSTANCE)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_IN_CAMPAIN));
		}
		else
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_NOT_FREE));
		}
	}
	//打开将领装备界面 
	void OpenEquipWin()
	{
		GameObject infowin = U3dCmn.GetObjFromPrefab("HeroEquipWin");
		if(infowin != null)
		{
			infowin.SendMessage("RevealPanel",select_hero);
		}
	}
	//打开强化将领界面  
	void OpenStrengthenWin()
	{
		if(select_hero.nStatus14 != (int)HeroState.NORMAL)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_NOT_FREE)); 
			return ;
		}
		GameObject infowin = U3dCmn.GetObjFromPrefab("HeroStrengthenWin");
		if(infowin != null)
		{
			infowin.SendMessage("RevealPanel",select_hero);
		}
	}
	//打开经验窗口 
	void OpenJiangLingExpWin()
	{
		if(select_hero.nHeroID1 != 0)
		{
			GameObject infowin = U3dCmn.GetObjFromPrefab("JiangLingExpWin");
			if(infowin != null)
			{
				infowin.SendMessage("RevealPanel",select_hero.nHeroID1);
			}
		}
	}
	//修改将领名字 
	void ModifyHeroName()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("ModifyNameWin");
		if (obj != null)
		{
			if(select_hero.nHeroID1>0)
			{
				obj.GetComponent<ModifyNameWin>().hero_id = select_hero.nHeroID1;
				obj.SendMessage("RevealPanel",(int)MODIFY_NAME_TYPE.HERO_NAME);
			}
			
		}
	}
}
