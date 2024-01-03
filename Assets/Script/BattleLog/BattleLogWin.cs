using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using STCCMD;
using CMNCMD;

public class BattleLogWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	public GameObject log_item;
	public GameObject title_item;
	public UIDraggablePanel log_panel;
	public UIGrid log_grid;
	public UIImageButton view_battle_btn;
	public UIImageButton self_btn;
	public UIImageButton enemy_btn;
	public UIImageButton lord_info_btn;
	List<HeroUnit> attack_unit_list = new List<HeroUnit>();
	List<HeroUnit> defense_unit_list = new List<HeroUnit>();

	AlliInstanceCombatLog now_combat_log =  new AlliInstanceCombatLog();
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
	}
	// Use this for initialization
	void Start () {
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//打开窗口 
	void RevealPanel()
	{
		SelfBattleLog();
		tween_position.Play(true);
	}
	//关闭窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//根据战报邮件初始化 
	public void InitialByMail(MessageManager.MessageData unit)
	{
		if(unit.msg_type ==(byte)MAIL_TYPE.MAIL_TYPE_SYSTEM)
		{
			PrivateMailUnit mail_data = (PrivateMailUnit)MessageManager.SysMsgMap[unit.mail_id];
			if(mail_data.nFlag4 == (byte)MAIL_FLAG.MAIL_FLAG_NEWFIGHT)
			{
				InitialByBuff(mail_data.szExtData12);
		
			}
		}
	}
	//根据战报数据BUFF初始化 
	public void InitialByBuff(byte[] buff)
	{
		AlliInstanceCombatLog combat_log = DataConvert.ByteToStruct<AlliInstanceCombatLog>(buff);	
		now_combat_log = combat_log;
		if(combat_log.nCombatType2 == (uint)COMBAT_TYPE.COMBAT_PVP_ATTACK || combat_log.nCombatType2 == (uint)COMBAT_TYPE.COMBAT_PVP_DEFENSE)
		{
			lord_info_btn.gameObject.SetActiveRecursively(true);
			view_battle_btn.gameObject.transform.localPosition = new Vector3(165,-123,0); 
			lord_info_btn.gameObject.transform.localPosition = new Vector3(55,-123,0);
			enemy_btn.gameObject.transform.localPosition = new Vector3(-55,-123,0); 
			self_btn.gameObject.transform.localPosition = new Vector3(-165,-123,0);
			
		}
		else
		{
			view_battle_btn.gameObject.transform.localPosition = new Vector3(120,-123,0); 
			enemy_btn.gameObject.transform.localPosition = new Vector3(0,-123,0);
			self_btn.gameObject.transform.localPosition = new Vector3(-120,-123,0); 
			lord_info_btn.gameObject.SetActiveRecursively(false);
		}
		int head_len = U3dCmn.GetSizeofSimpleStructure<AlliInstanceCombatLog>();
		int data_len = buff.Length - head_len;
		byte[] data_buff = new byte[data_len];
		Array.Copy(buff,head_len,data_buff,0,data_len);
			
		int attack_data_len = U3dCmn.GetSizeofSimpleStructure<HeroUnit>()*combat_log.nAttackNum6;
		byte[] attack_hero_buff = new byte[attack_data_len];
		Array.Copy(data_buff,0,attack_hero_buff,0,attack_data_len);
		HeroUnit[] attack_hero_units = DataConvert.ByteToStructArray<HeroUnit>(attack_hero_buff,combat_log.nAttackNum6);	
		
		int defense_data_len = U3dCmn.GetSizeofSimpleStructure<HeroUnit>()*combat_log.nDefenseNum7;
		byte[] defense_hero_buff = new byte[defense_data_len];
		Array.Copy(data_buff,attack_data_len,defense_hero_buff,0,defense_data_len);
		HeroUnit[] defense_hero_units = DataConvert.ByteToStructArray<HeroUnit>(defense_hero_buff,combat_log.nDefenseNum7);	
		
		int loot_data_len = U3dCmn.GetSizeofSimpleStructure<LootUnit>()*combat_log.nLootNum8;
		byte[] loot_buff = new byte[loot_data_len];
		Array.Copy(data_buff,attack_data_len+defense_data_len,loot_buff,0,loot_data_len);
		LootUnit[] loot_units = DataConvert.ByteToStructArray<LootUnit>(loot_buff,combat_log.nLootNum8);
		
		U3dCmn.ArrayToList<HeroUnit>(attack_hero_units,attack_unit_list);
		U3dCmn.ArrayToList<HeroUnit>(defense_hero_units,defense_unit_list);
	
		
		attack_unit_list.Sort(CompareHeroUnitData);
		defense_unit_list.Sort(CompareHeroUnitData);
		
		//打开窗口 
		RevealPanel();
	}

	//自身损失战报 
	void SelfBattleLog()
	{
		AnalyzeBattleLog(true);
	}
	//敌人的损失战报 
	void EnemyBattleLog()
	{
		AnalyzeBattleLog(false);
	}
	//解析战报数据 
	void AnalyzeBattleLog(bool is_self)
	{
		List<HeroUnit> log_list;
		if(is_self)
		{
			if(BattleManager.Instance.CheckMeAttack(now_combat_log))
			{
				log_list =  attack_unit_list;
			}
			else 
			{
				log_list = defense_unit_list;
			}
		}
		else 
		{
			if(BattleManager.Instance.CheckMeAttack(now_combat_log))
			{
				log_list =  defense_unit_list;
			}
			else 
			{
				log_list = attack_unit_list;
			}
		}
		ClearBattleLogPanel(log_grid);
		int index=0;
		GameObject title_obj = NGUITools.AddChild(log_grid.gameObject,title_item);
		string obj_name = string.Format("item{0:D2}",index);
		title_obj.name = obj_name;
		string pre_char_name = "GM";
		GameObject pre_char_obj = null;
		uint total_prof = 0;
		uint lord_prof = 0;
		for(int i=0;i<log_list.Count;i++)
		{
			index++;
			GameObject obj = NGUITools.AddChild(log_grid.gameObject,log_item);
			obj_name = string.Format("item{0:D2}",index);
			obj.name = obj_name;
			HeroUnit info =  log_list[i];
			
			string char_name = DataConvert.BytesToStr(info.szCharName14);
			if(char_name != pre_char_name)
			{
				if(pre_char_obj != null)
				{
					pre_char_obj.GetComponent<BattleLogItem>().InitialCharData(pre_char_name,lord_prof);
				}
				
				pre_char_name = char_name;
				pre_char_obj = obj;
				lord_prof = 0;
				i--;
			}
			else
			{
				obj.GetComponent<BattleLogItem>().InitialHeroData(info);
				total_prof += info.nProf4;
				lord_prof += info.nProf4;
				if(i+1 == log_list.Count)
				{
					if(pre_char_obj != null)
					{
						pre_char_obj.GetComponent<BattleLogItem>().InitialCharData(char_name,lord_prof);
					}
				}
			}	
		}
		UILabel title_label = title_obj.GetComponentInChildren<UILabel>();
		if(is_self)
			title_label.text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.OUR_TOTAL_FORCE),total_prof);
		else 
			title_label.text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ENEMY_TOTAL_FORCE),total_prof);
		log_grid.Reposition();
		log_panel.ResetPosition();
	}
	//清理GRID面板 
	void ClearBattleLogPanel(UIGrid grid)
	{
		foreach(Transform obj in grid.gameObject.transform)
		{
			obj.gameObject.active = false;
			Destroy(obj.gameObject);
		}
	}
	//给自身损失战报排序的COMPARE 
	public int  CompareHeroUnitData(HeroUnit data1,HeroUnit data2)
	{ 
		int rst = 0;
		string str1 = DataConvert.BytesToStr(data1.szCharName14);
		string str2 = DataConvert.BytesToStr(data2.szCharName14);
		rst = string.Compare(str1,str2);
		if(rst == 0)
		{
			rst = CompareXY(data1,data2);
		}
		return rst;
	}
	//按照XY布阵顺序排序COMPARE  
	public int  CompareXY(HeroUnit data1,HeroUnit data2)
	{ 
		int rst = 0;

		if(data1.nCol6 > data2.nCol6)
		{
			rst = 1;
		}
		else if(data1.nCol6 < data2.nCol6)
		{
			rst = -1;
		}
		else if(data1.nCol6 == data2.nCol6)
		{
			if(data1.nRow5 > data2.nRow5)
			{
				rst = 1;
			}
			else if(data1.nRow5 < data2.nRow5)
			{
				rst = -1;
			}
		}
		return rst;
	}
	//查看敌人信息 
	public void OpenPlayerInfoWin()
	{
		if(now_combat_log.nCombatType2 != 0)
		{
			if(now_combat_log.nCombatType2 == (uint)COMBAT_TYPE.COMBAT_PVP_ATTACK || now_combat_log.nCombatType2 == (uint)COMBAT_TYPE.COMBAT_PVP_DEFENSE)
			{
				if(BattleManager.Instance.CheckMeAttack(now_combat_log))
				{
					U3dCmn.OpenPlayerCardWin(now_combat_log.nDefenseAccID4);
				}
				else 
				{
					U3dCmn.OpenPlayerCardWin(now_combat_log.nAttackAccID3);
				}
			}
		}
	}
	//观战 
	public void WatchBattle()
	{
		bool is_win = true;
		bool is_left = true;
		BattleManager.Instance.CheckBattleRst(now_combat_log,ref is_win,ref is_left);
		ComplexCombatLog combat_data = new ComplexCombatLog();
		combat_data.CombatSeed = now_combat_log.nRandSeed5;
		combat_data.IsWin = is_win;
		combat_data.IsLeft = is_left;
		combat_data.AttackHeros = attack_unit_list.ToArray();
		combat_data.DefenseHeros = defense_unit_list.ToArray();
		combat_data.OnBattleFieldEnd = BattleEnd;
		combat_data.AttackEnhanceRatio = now_combat_log.fcounterrate9;
        //StringBuilder sb = new StringBuilder();
        //for (int i = 0; i < combat_data.AttackHeros.Length; ++i) {
        //    HeroUnit hu = combat_data.AttackHeros[i];
        //    if (sb.Length > 0) {
        //        sb.Append(",");
        //    }
        //    sb.Append(string.Format("{0}*{1}*{2}*{3}*{4}", hu.nHeroID12, hu.nCol6, hu.nRow5, hu.nProf4, hu.nArmyType7));
        //}
        //print("left data: "+ sb.ToString());
        //StringBuilder sb1 = new StringBuilder();
        //for (int i = 0; i < combat_data.DefenseHeros.Length; ++i)
        //{
        //    HeroUnit hu = combat_data.DefenseHeros[i];
        //    if (sb1.Length > 0)
        //    {
        //        sb1.Append(",");
        //    }
        //    sb1.Append(string.Format("{0}*{1}*{2}*{3}*{4}", hu.nHeroID12, hu.nCol6, hu.nRow5, hu.nProf4, hu.nArmyType7));
        //}
        //print("right data: " + sb1.ToString());
        //print("seed is "+ combat_data.CombatSeed);
        //print("atk is " + combat_data.AttackEnhanceRatio);
        BattleManager.Instance.ShowBattleWindow(combat_data,false,true);
	}
	public void BattleEnd(bool isWin, bool isSkip)
	{
		if(isSkip)
			U3dCmn.SendMessage("SceneManager","OpenWinLoseAnimation",isWin);
	}
}
