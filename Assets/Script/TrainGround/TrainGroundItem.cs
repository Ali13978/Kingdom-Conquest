using UnityEngine;
using System;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class TrainGroundItem : MonoBehaviour {
	public UISlicedSprite select_bg;
	public GameObject HeroInfoObj;
	public UISprite head_icon;
	public UILabel char_name;
	public UILabel level;
	public UILabel state;
	public UILabel exp_rate;
	public UILabel exp_rate_label;
	public UILabel has_exp;
	public UILabel has_exp_label;
	public UILabel train_time;
	public UILabel train_time_last;
	public UILabel level_top;
	public UIImageButton stop_train_btn;
	public HireHero hero_unit;
	public ulong  now_hero_id;
	public bool is_select = false;
	// Use this for initialization
	void Start () {
	
	}
	
	void InitialData(ulong hero_id)
	{
		is_select = false;
		select_bg.spriteName = "sliced_bg";
		now_hero_id = hero_id;
		if(hero_id != 0)
		{
			head_icon.gameObject.SetActiveRecursively(true);
			hero_unit = (HireHero)JiangLingManager.MyHeroMap[hero_id];
			head_icon.spriteName = U3dCmn.GetHeroIconName(hero_unit.nModel12);
			char_name.text = DataConvert.BytesToStr(hero_unit.szName3);
			level.text = "Lv"+hero_unit.nLevel19;
			float now_exp = (float)(hero_unit.nExp22);
			if(CommonMB.HeroExpGold_Map.Contains(hero_unit.nLevel19+1))
			{
				HeroExpGoldInfo info =  (HeroExpGoldInfo)CommonMB.HeroExpGold_Map[hero_unit.nLevel19+1];
				float need_exp = (float)info.LevelUpExp;
				exp_rate.text = (int)(now_exp/need_exp*100)+"%";
			
			}
			else
			{
				exp_rate.text = 0+"%";
			}
			if(hero_unit.nStatus14 == (int)HeroState.COMBAT_PVP_ATTACK || hero_unit.nStatus14 == (int)HeroState.COMBAT_PVE_RAID 
				|| hero_unit.nStatus14 == (int)HeroState.HONOR_CITY_ATTACK || hero_unit.nStatus14 == (int)HeroState.COMBAT_HERO_WORLD_RESOURCE)
			{
			
				state.color = new Color(0,1,1,1);
				state.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_IN_BATTLE);
			}
			else if(hero_unit.nStatus14 == (int)HeroState.COMBAT_INSTANCE_BAIZHANBUDAI)
			{
				state.color = new Color(0,1,1,1);
				state.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_BAIZHAN);
			}
			else if(hero_unit.nStatus14 == (int)HeroState.COMBAT_INSTANCE_NANZHENGBEIZHAN)
			{
				state.color = new Color(0,1,1,1);
				state.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_NANZHENG);
			}
			else if(hero_unit.nStatus14 == (int)HeroState.COMBAT_INSTANCE_GUILD)
			{
				state.color = new Color(0,1,1,1);
				state.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GUILD_WAR);
			}
			else if(hero_unit.nStatus14 == (int)HeroState.COMBAT_HERO_TRAINING)
			{
				state.color = new Color(0,1,1,1);
				state.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_IN_TRAIN);
			}
			else 
			{
				state.color = new Color(0,1,0,1);
				state.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_FREE);
			}
			if(TrainGroundManager.HeroTrainingMap.Contains(hero_id))
			{
				TrainingTimeEvent train_unit = (TrainingTimeEvent)TrainGroundManager.HeroTrainingMap[hero_id];
				int train_sec = train_unit.nEndTime2-train_unit.nBeginTime1;
				int last_time = (train_unit.nEndTime2 - DataConvert.DateTimeToInt(DateTime.Now));
				last_time = last_time>0?last_time:0;
				int exp = last_time>0?(int)(train_unit.nExp4*((float)(train_sec-last_time)/(float)train_sec)):(int)train_unit.nExp4;
				has_exp.text = exp.ToString();
				int train_hour = train_sec/3600;
				train_time.text = train_hour+"H";
				StopCoroutine("TrainCountdown");
				StartCoroutine("TrainCountdown",train_unit.nEndTime2);
				//train_time_last.text = "";
				stop_train_btn.gameObject.SetActiveRecursively(true);
				level_top.gameObject.SetActiveRecursively(false);
			}
			else
			{
				//判断将领是否满级 
				if(hero_unit.nLevel19 >= CommonData.player_online_info.Level)
				{
					level_top.gameObject.SetActiveRecursively(true);
					has_exp_label.gameObject.SetActiveRecursively(false);
					exp_rate_label.gameObject.SetActiveRecursively(false);
					exp_rate.text = "";
				}
				else 
				{
					level_top.gameObject.SetActiveRecursively(false);
					has_exp_label.gameObject.SetActiveRecursively(true);
					exp_rate_label.gameObject.SetActiveRecursively(true);
				}
				//has_exp = hero_unit.
				has_exp.text  = "";
				train_time.text = "";
				train_time_last.text = "";
				stop_train_btn.gameObject.SetActiveRecursively(false);
				StopCoroutine("TrainCountdown");
			}
			
		}
		else 
		{
			HeroInfoObj.SetActiveRecursively(false);
		}
		
	}
	//点击 
	void SelectItem()
	{
		if(now_hero_id != 0 && !TrainGroundManager.HeroTrainingMap.Contains(now_hero_id))
		{
			if(JiangLingManager.MyHeroMap.Contains(now_hero_id))
			{
				HireHero info =(HireHero)JiangLingManager.MyHeroMap[now_hero_id];
				if(info.nStatus14 != (int)HeroState.NORMAL || info.nLevel19 >= CommonData.player_online_info.Level)
				{
					return;
				}
			}
			is_select = !is_select;
			if(is_select)
			{
				if(!TrainGroundManager.SelectHeroList.Contains(now_hero_id))
				{
					TrainGroundManager.SelectHeroList.Add(now_hero_id);
				}
				select_bg.spriteName = "sliced_bg3";
			}
			else
			{
				TrainGroundManager.SelectHeroList.Remove(now_hero_id);
				select_bg.spriteName = "sliced_bg";
			}
				
		}
		
	}
	//设为选中 
	public void SetSelect()
	{
		if(now_hero_id != 0 && !TrainGroundManager.HeroTrainingMap.Contains(now_hero_id))
		{
			is_select = true;
			select_bg.spriteName = "sliced_bg3";
		}
		
	}
	//退出界面停止倒计时  
	void QuitWin()
	{
		StopCoroutine("TrainCountdown");
	}
	//倒计时  
	IEnumerator TrainCountdown(int EndTimeSec)
	{
		int sec = (int)(EndTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
		
		if(sec <0)
			sec = 0;
		while(sec!=0)
		{
			 sec =(int)(EndTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
			if(sec <0)
				sec = 0;
			int hour = sec/3600;
			int minute = sec/60%60;
			int second = sec%60;
			if(hour>=100)
				train_time_last.text =string.Format("{0}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			else
				train_time_last.text =string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			yield return new WaitForSeconds(1);
		}
	}
	//停止修炼 
	void StopTraining()
	{
		TrainGroundManager.ReqStopTrainHero(now_hero_id);
	}
}
