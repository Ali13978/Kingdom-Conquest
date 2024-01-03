using UnityEngine;
using System;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class JiangLingManager : MonoBehaviour {
	//已招募将领MAP表 将领ID做主键  
	public static Hashtable MyHeroMap =  new Hashtable();
	 static int rst = 0;
	//用于登记配兵结果分发对象（OBJ的脚本必须含有void SaveWithSoldierDataRst(int rst) 
	public static GameObject CallBackObj;
	
	public delegate void ProcessHireHeroInfoDelegate();
	public static ProcessHireHeroInfoDelegate processHireHeroInfoDelegate;
	
	// Use this for initialization
	void Start () {
		JiangLingManager.GetJiangLingInfo();
	}
	
	//请求雇佣武将信息  
	public static void GetJiangLingInfo()
	{
		CTS_GAMECMD_OPERATE_GET_HERO_HIRE_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_GET_HERO_HIRE;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_GET_HERO_HIRE_T >(req);
	}
	//打开将领管理界面 
	void OpenJiangLingInfoWin()
	{
		GameObject infowin = U3dCmn.GetObjFromPrefab("JiangLingInfoWin");
		if(infowin != null)
		{
			infowin.SendMessage("RevealPanel");
		}
	}
	//请求雇佣武将信息返回   
	void ProcessJiangLingInfo(byte[] buff)
	{
		MyHeroMap.Clear();
		STC_GAMECMD_OPERATE_GET_HERO_HIRE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_GET_HERO_HIRE_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_GET_HERO_HIRE_T.enum_rst.RST_OK)
		{
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_OPERATE_GET_HERO_HIRE_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			HireHero[] jiangling_list = (HireHero[])DataConvert.ByteToStructArray<HireHero>(data_buff,sub_msg.nNum2);
			for(int i=0;i<jiangling_list.Length;i++)
			{
				HireHero h1 = jiangling_list[i];
				MyHeroMap.Add(jiangling_list[i].nHeroID1,jiangling_list[i]);
			}
			
			if (processHireHeroInfoDelegate != null)
			{
				processHireHeroInfoDelegate();
			}
		}
		
		processHireHeroInfoDelegate = null;
		U3dCmn.SendMessage("JiuGuanInfoWin","RefreshHeroInfo",null);
	}
	//请求将领配兵返回指令处理（根据情况把结果分发到不同的界面进行处理） 
	void ProcessWithSoldierRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_CONFIG_HERO_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_CONFIG_HERO_T>(buff);
		if(CallBackObj!=null)
		{
			CallBackObj.SendMessage("SaveWithSoldierDataRst",sub_msg);
		}
		
	}
	//改变当前将领的基本信息并判断是否需要把兵放回兵营 
	public static HireHero UpdateHeroInfo(ulong now_hero_id,HeroDesc update_hero)
	{
		HireHero now_hero =new HireHero();
		if(now_hero_id == update_hero.nHeroID10)
		{
			now_hero = (HireHero)JiangLingManager.MyHeroMap[now_hero_id];
			//先判断是否需要把兵放回兵营 
			if(now_hero.nArmyType9 == update_hero.nArmyType12 && now_hero.nArmyLevel10== update_hero.nArmyLevel13)
			{
				if(now_hero.nArmyNum11	> update_hero.nArmyNum14)
				{
					int soldier_num = now_hero.nArmyNum11-update_hero.nArmyNum14;
					string key = now_hero.nArmyType9+"_"+now_hero.nArmyLevel10;
					if(SoldierManager.SoldierMap.Contains(key))
					{
						SoldierUnit soldierinfo = (SoldierUnit)SoldierManager.SoldierMap[key];
						soldierinfo.nNum3 += soldier_num;
						SoldierManager.SoldierMap[key] = soldierinfo;
					}
					else 
					{
						SoldierUnit soldierinfo;
						soldierinfo.nExcelID1 = (uint)now_hero.nArmyType9;
						soldierinfo.nLevel2 = (uint)now_hero.nArmyLevel10;
						soldierinfo.nNum3 = soldier_num;
						SoldierManager.SoldierMap.Add(key,soldierinfo);
					}
				}
				else if(now_hero.nArmyNum11	< update_hero.nArmyNum14) 
				{
					//不会到这里 目前还没有通过更改武将属性自动补兵的功能 
					
				}
			}
		
			now_hero.nAttack5 = update_hero.nAttackBase1;
			now_hero.nAttackAdd15 = update_hero.nAttackAdd2;
			now_hero.nDefense6 = update_hero.nDefenseBase3;
			now_hero.nDefenseAdd16 = update_hero.nDefenseAdd4;
			now_hero.nHealth7 = update_hero.nHealthBase5;
			now_hero.nHealthAdd17 = update_hero.nHealthAdd6;
			now_hero.nLeader13 = update_hero.nLeaderBase7;
			now_hero.nLeaderAdd18 = update_hero.nLeaderAdd8;
			now_hero.fGrow8		 = update_hero.fGrow9;
			now_hero.nHealthState21 = update_hero.nHealthState11;
			now_hero.nArmyType9 = update_hero.nArmyType12;
			now_hero.nArmyLevel10	= update_hero.nArmyLevel13;
			now_hero.nArmyNum11	= update_hero.nArmyNum14;
			now_hero.nExp22 = update_hero.nExp15;
			now_hero.nLevel19 = update_hero.nLevel16;
			now_hero.nProf20 = update_hero.nProf17;
			JiangLingManager.MyHeroMap[now_hero_id] = now_hero;
			
		}
		return now_hero;
	}
    /// <summary>
    /// 批量修改英雄状态 
    /// </summary>
    /// <param name="heroIDs"></param>
    /// <param name="state">HeroState状态</param>
    public static void UpdateHeroStatus(ulong[] heroIDs, HeroState state){
        foreach (ulong id in heroIDs) {
            HireHero hero = (HireHero)MyHeroMap[id];
            hero.nStatus14 = (int)state;
            MyHeroMap[id] = hero;
        }
    }
	//处理将领的简单信息 
	public static void UpdateHeroSimpleInfo(byte[] buff)
	{
		STC_GAMECMD_HERO_SIMPLE_DATA_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_HERO_SIMPLE_DATA_T>(buff);
		HeroSimpleData[] hero_array = DataConvert.ByteToStructArrayByOffset<HeroSimpleData>(buff,sub_msg.nNum3,U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_HERO_SIMPLE_DATA_T>());
        //print ("bbbbbbbbbbbbbbbbbbbbbbbbb"+rst+" "+sub_msg.nCombatType1+" "+hero_array.Length);
        //print ("rrrrrrrrrrrrrrrrrrrrrrrrr"+rst+" "+sub_msg.nReason2);
		if(sub_msg.nCombatType1 == (uint)COMBAT_TYPE.COMBAT_INSTANCE_GUILD)
		{
			GBTeamManager.Instance.ChangeHeroState(sub_msg.nReason2,hero_array);
			UpdateArmyDeployData(hero_array,COMBAT_TYPE.COMBAT_INSTANCE_GUILD);
		}
		else if(sub_msg.nCombatType1 == (uint)COMBAT_TYPE.COMBAT_ARENA)
		{
			//GBTeamManager.Instance.ChangeHeroState(sub_msg.nReason2,hero_array);
			//UpdateArmyDeployData(hero_array);
			//ArenaManager.Instance.in
			UpdateArmyDeployData(hero_array,COMBAT_TYPE.COMBAT_ARENA);
			LoadingManager.instance.HideLoading();
		}
		for(int i=0;i<sub_msg.nNum3;i++)
		{
			HeroSimpleData hero_info = hero_array[i];
            //print ("ssss"+rst+" "+hero_info.nArmyNum4);
			Hashtable soldier_map = SoldierManager.SoldierMap;
			if (true == MyHeroMap.ContainsKey(hero_info.nHeroID1))
			{
				HireHero pre_hero = (HireHero) MyHeroMap[hero_info.nHeroID1];
				int delta = pre_hero.nArmyNum11 - hero_info.nArmyNum4;
				//print ("+Auto Hero SoldierUnit:" + g.nHeroID1 + "," + delta); // 补给 ...
				pre_hero.nArmyNum11 = hero_info.nArmyNum4;
				pre_hero.nProf20	= hero_info.nProf5;
				pre_hero.nHealthState21 = hero_info.nHealthState6;
				pre_hero.nLevel19 = hero_info.nLevel7;
				pre_hero.nExp22 = hero_info.nExp8;
				pre_hero.nStatus14 = hero_info.nStatus9;
				MyHeroMap[hero_info.nHeroID1] = pre_hero;
                //print ("dddd"+delta);
				if(delta < 0)
				{
					string soldier_key = string.Format("{0}_{1}",pre_hero.nArmyType9,pre_hero.nArmyLevel10);
					if (true == soldier_map.ContainsKey(soldier_key))
					{
						SoldierUnit unit = (SoldierUnit) soldier_map[soldier_key];
						int nLastNum = unit.nNum3; 
						unit.nNum3 += delta;
						if(unit.nNum3 <0)
						{
							//出问题了 先赋成0吧 
							unit.nNum3 = 0;
						}
							
						soldier_map[soldier_key] = unit;
					}	
				}
				
			}
		}
		rst ++;
		
	}
	//刷新配兵数据及界面  
	public static void UpdateArmyDeployData(HeroSimpleData[] hero_data,COMBAT_TYPE type)
	{
		ArmyDeployManager.instance.UpdateArmyData(hero_data,type);
	}
}
