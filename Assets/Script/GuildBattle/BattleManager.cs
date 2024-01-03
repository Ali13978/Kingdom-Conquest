using System;
using System.Collections;
using CMNCMD;
using CTSCMD;
using STCCMD;
using TinyWar;
using UnityEngine;

/// <summary>
/// 战斗管理 
/// </summary>
public class BattleManager: MonoBehaviour {
    #region Singleton
    static BattleManager _instance;
    private BattleManager() {
    }
    static BattleManager()
    {
        _instance = new BattleManager();
    }
    public static BattleManager Instance
    {
        get { return _instance; }
    }
    #endregion
	byte[] now_battle_log_buff = new byte[0];
	AlliInstanceCombatLog now_combat_log;
	
    /// <summary>
    /// 开战数据，根本需要发送，ObjectID和CombatType必须要有
    /// </summary>
    public class CombatInfo {
        public byte AutoCombat;
        public byte AutoSupply;
        public uint StopLevel;
        public ulong ObjectID;
        public uint CombatType;
        public byte[] HeroData = new byte[0];
    }

    #region Method
    /// <summary>
    /// 开始没关战斗 
    /// </summary>
    /// <param name="info">战斗数据</param>
    public void StartCombat(CombatInfo info) {
        CTS_GAMECMD_ALLI_INSTANCE_START_COMBAT_T req;
        req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ALLI_INSTANCE_START_COMBAT;
        req.bAutoCombat3 = info.AutoCombat;
        req.bAutoSupply4 = info.AutoSupply;
        req.nStopLevel5 = info.StopLevel;
        req.nObjID6 = info.ObjectID;
        req.nCombatType7 = info.CombatType;
        req.nNum8 = info.HeroData.Length;
        req.nHeroData9 = info.HeroData;
        TcpMsger.SendLogicData<CTS_GAMECMD_ALLI_INSTANCE_START_COMBAT_T>(req);
    }
    public void StartCombatRst(byte[] buffer) { 
        STC_GAMECMD_ALLI_INSTANCE_START_COMBAT_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_START_COMBAT_T>(buffer);
		
		if(msg.nRst1 != (int)STC_GAMECMD_ALLI_INSTANCE_START_COMBAT_T.enum_rst.RST_OK)
		{
			print (msg.nRst1);
			LoadingManager.instance.HideLoading();
		}
		if(msg.nRst1 == (int)STC_GAMECMD_ALLI_INSTANCE_START_COMBAT_T.enum_rst.RST_ARENA_RANK_LOW)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CAN_NOT_CHALLENGE_LOW));
		}
		else if(msg.nRst1 == (int)STC_GAMECMD_ALLI_INSTANCE_START_COMBAT_T.enum_rst.RST_ARENA_NO_TRIES)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ARENA_TRY_USEDUP));
		}
		else if(msg.nRst1 == (int)STC_GAMECMD_ALLI_INSTANCE_START_COMBAT_T.enum_rst.RST_ARENA_NEED_UPLOAD)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ARENA_MUST_UPLOAD_FIRST));
		}
		 
    }

    /// <summary>
    /// 手动补兵 
    /// </summary>
    /// <param name="combatType">战斗类型 </param>
    /// <param name="InstanceID">副本ID</param>
    public void ManualSupply(uint combatType, ulong instanceID) {
        CTS_GAMECMD_ALLI_INSTANCE_SUPPLY_T req;
        req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ALLI_INSTANCE_SUPPLY;
        req.nCombatType3 = combatType;
        req.nInstanceID4 = instanceID;
        TcpMsger.SendLogicData<CTS_GAMECMD_ALLI_INSTANCE_SUPPLY_T>(req);
    }
    /// <summary>
    /// 手动补兵返回 
    /// </summary>
    /// <param name="buffer"></param>
    public void ManualSupplyRst(byte[] buffer) {
        STC_GAMECMD_ALLI_INSTANCE_SUPPLY_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_SUPPLY_T>(buffer);
        uint combatType = msg.nCombatType2;
        switch (combatType) { 
            case (uint)HeroState.COMBAT_INSTANCE_GUILD://联盟战  
                GBTeamManager.Instance.ManualSupplyForceRst(buffer);
            break;
        }
        
    }
    /// <summary>
    /// 请求简单战报 
    /// </summary>
    /// <param name="combatType">战斗类型 </param>
    /// <param name="instanceID">副本ID</param>
    public void RequestSimpleCombatLog(uint combatType, ulong instanceID)
    {
        CTS_GAMECMD_ALLI_INSTANCE_GET_SIMPLE_COMBAT_LOG_T req;
        req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ALLI_INSTANCE_SIMPLE_COMBAT_LOG;
        req.nCombatType3 = combatType;
        req.nInstanceID4 = instanceID;
        TcpMsger.SendLogicData<CTS_GAMECMD_ALLI_INSTANCE_GET_SIMPLE_COMBAT_LOG_T>(req);
    }
    /// <summary>
    /// 简单战报返回 
    /// </summary>
    /// <param name="buffer"></param>
    public void RequestSimpleCombatLogRst(byte[] buffer) {
        STC_GAMECMD_ALLI_INSTANCE_GET_SIMPLE_COMBAT_LOG_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_GET_SIMPLE_COMBAT_LOG_T>(buffer);
        uint combatType = msg.nCombatType3;
        switch (combatType) { 
            case (uint)COMBAT_TYPE.COMBAT_INSTANCE_GUILD:
                GBTeamManager.Instance.RequestSimpleCombatLogRst(buffer);
            break;
			case (uint)COMBAT_TYPE.COMBAT_ARENA:
                RequestComplexCombatLog(combatType,msg.nEventID7);
            break;
        }
        
    }
    /// <summary>
    /// 请求复杂战报 
    /// </summary>
    /// <param name="combatType">战斗类型</param>
    /// <param name="instanceID">副本ID</param>
    public void RequestComplexCombatLog(uint combatType, ulong instanceID)
    {
	
        CTS_GAMECMD_ALLI_INSTANCE_GET_COMBAT_LOG_T req;
        req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ALLI_INSTANCE_COMBAT_LOG;
        req.nCombatType3 = combatType;
        req.nInstanceID4 = instanceID;
        TcpMsger.SendLogicData<CTS_GAMECMD_ALLI_INSTANCE_GET_COMBAT_LOG_T>(req);
    }
    /// <summary>
    /// 复杂战报返回 
    /// </summary>
    /// <param name="buffer"></param>
    public void RequestComplexCombatLogRst(byte[] buffer) {
        STC_GAMECMD_ALLI_INSTANCE_GET_COMBAT_LOG_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_GET_COMBAT_LOG_T>(buffer);
        uint combatType = msg.nCombatType2;
        switch (combatType) { 
            case (uint)COMBAT_TYPE.COMBAT_INSTANCE_GUILD:
                GBTeamManager.Instance.RequestComplexCombatLogRst(buffer);
            break;
			case (uint)COMBAT_TYPE.COMBAT_ARENA:
			{
				LoadingManager.instance.HideLoading();
				LaunchBattleWin(buffer);
			}
                
            break;
        }
    }
	//拉起战斗画面 
	public void LaunchBattleWin(byte[] buff)
	{
		int buff_head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_ALLI_INSTANCE_GET_COMBAT_LOG_T>();
        byte[] logBuffer = new byte[buff.Length - buff_head_len];
        Array.Copy(buff, buff_head_len, logBuffer, 0, logBuffer.Length);
		
		int head_len = U3dCmn.GetSizeofSimpleStructure<AlliInstanceCombatLog>();
		print (logBuffer.Length + "----"+head_len);
		
		if(logBuffer.Length<head_len)
		{
			return;
		}
		now_battle_log_buff = logBuffer;
		AlliInstanceCombatLog combat_log = DataConvert.ByteToStruct<AlliInstanceCombatLog>(logBuffer);	
		now_combat_log = combat_log;
		
		int data_len = logBuffer.Length - head_len;
		byte[] data_buff = new byte[data_len];
		Array.Copy(logBuffer,head_len,data_buff,0,data_len);
			
		int attack_data_len = U3dCmn.GetSizeofSimpleStructure<HeroUnit>()*combat_log.nAttackNum6;
		byte[] attack_hero_buff = new byte[attack_data_len];
		Array.Copy(data_buff,0,attack_hero_buff,0,attack_data_len);
		HeroUnit[] attack_hero_units = DataConvert.ByteToStructArray<HeroUnit>(attack_hero_buff,combat_log.nAttackNum6);	
		
		int defense_data_len = U3dCmn.GetSizeofSimpleStructure<HeroUnit>()*combat_log.nDefenseNum7;
		byte[] defense_hero_buff = new byte[defense_data_len];
		Array.Copy(data_buff,attack_data_len,defense_hero_buff,0,defense_data_len);
		HeroUnit[] defense_hero_units = DataConvert.ByteToStructArray<HeroUnit>(defense_hero_buff,combat_log.nDefenseNum7);	
		
		bool is_win = true;
		bool is_left = true;
		CheckBattleRst(combat_log,ref is_win,ref is_left);
		ComplexCombatLog combat_data = new ComplexCombatLog();
		combat_data.CombatSeed = combat_log.nRandSeed5;
		combat_data.IsWin = is_win;
		combat_data.IsLeft = is_left;
		combat_data.AttackHeros = attack_hero_units;
		combat_data.DefenseHeros = defense_hero_units;
		combat_data.OnBattleFieldEnd = ShowBattleLog;
        combat_data.AttackEnhanceRatio = combat_log.fcounterrate9;
        BattleManager.Instance.ShowBattleWindow(combat_data,false,true);
	}
	//检查我是不是攻击方 
	public bool CheckMeAttack(AlliInstanceCombatLog log_data)
	{
		bool rst = true;
		if(log_data.nCombatType2 == (uint)COMBAT_TYPE.COMBAT_PVP_ATTACK)
		{
			rst = true;
		}
		else if(log_data.nCombatType2 == (uint)COMBAT_TYPE.COMBAT_PVP_DEFENSE)
		{
			rst = false;
		}
		else
		{
			rst = true;
		}
		return rst;
	}
	//检验战斗胜利方和失败方 
	public  void CheckBattleRst(AlliInstanceCombatLog battle_data,ref bool is_win,ref bool is_left)
	{
		bool rst = true;
		if(battle_data.nCombatResult1 == (int)AlliInstanceCombatLog.enum_rst.combat_result_win )
		{
			rst = true;
		}
		else
		{
			rst = false;	
		}
		if(CheckMeAttack(battle_data))
		{
			is_win = rst;
			is_left = true; 
		}
		else
		{
			is_win = !rst;
			is_left = false;
		}
	}
	
    /// <summary>
    /// 弹出战场画面
    /// </summary>
    /// <param name="log"></param>
    public void ShowBattleWindow(ComplexCombatLog log,bool SmallField,bool ShowSkip)
    {
        GameObject obj = U3dCmn.GetObjFromPrefab(GBWindowPath.GBBattleFieldWin);
        GuildBattleManager gb = obj.GetComponent<GuildBattleManager>();
        if (gb != null)
        {
			gb.IsSmallField = SmallField;
			gb.ShowSkipBtn = ShowSkip;
            gb.Seed = log.CombatSeed;
            gb.WarriorLeftList = GBHelper.ConvertHeroUnit2BattleWarrior(log.AttackHeros);
            gb.WarriorRightList = GBHelper.ConvertHeroUnit2BattleWarrior(log.DefenseHeros);
            gb.IsWin = log.IsWin;
            gb.OnBattleFieldEnd = log.OnBattleFieldEnd;
            gb.Group = log.IsLeft ? TagType.Left : TagType.Right;
            gb.AttackEnhanceRatio = log.AttackEnhanceRatio;
            gb.ShowField();
        }
    }
	//弹出战报LOG  
	public void ShowBattleLog(bool isWin, bool isSkip)
	{
		if(isSkip)
				U3dCmn.SendMessage("SceneManager","OpenWinLoseAnimation",isWin);
		GameObject obj = U3dCmn.GetObjFromPrefab("BattleLogWin");
		
		if(now_battle_log_buff.Length >0)
		{
		    if (obj != null)
		    {
				
				BattleLogWin win = obj.GetComponent<BattleLogWin>();
	        	win.InitialByBuff(now_battle_log_buff);
		        
		    }
		}
		
		if(now_combat_log.nCombatType2 == (uint)COMBAT_TYPE.COMBAT_ARENA)
		{
			U3dCmn.SendMessage("ArenaWin","ReqArenaData",null);
		}
	}
    #endregion
}

/// <summary>
/// 团队副本复杂战报信息，简单战报参见 STC_GAMECMD_ALLI_INSTANCE_SIMPLE_COMBAT_LOG_T
/// </summary>
public class ComplexCombatLog
{
    /// <summary>
    /// 我方战斗结果
    /// </summary>
    public bool IsWin;
    /// <summary>
    /// 战斗种子  
    /// </summary>
    public int CombatSeed;
    /// <summary>
    /// 我方所有将领信息 
    /// </summary>
    public HeroUnit[] AttackHeros;
    /// <summary>
    /// 敌方所有将领信息 
    /// </summary>
    public HeroUnit[] DefenseHeros;
    /// <summary>
    /// 当前角色是左边还是右边
    /// </summary>
    public bool IsLeft;
    /// <summary>
    /// 战斗结束的回调函数
    /// </summary>
    public OnBattleFieldEnd OnBattleFieldEnd;
    /// <summary>
    /// 克制系数，战力放大系数
    /// </summary>
    public float AttackEnhanceRatio;
}
