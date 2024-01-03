using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using CTSCMD;
using STCCMD;

public class CombatCacheUnit
{
	public int 			nSlot;								// 战道编号 
	public int 			nModel;								// 头像 ID
	public ulong		nHeroID;							// 英雄ID
	public string 		HeroName;							// 英雄名字 
	public int 			nLevel;								// 英雄等级  Lv.
	public int 			nArmyType;							// 部队类型 
	public int 			nArmyLevel;							// 部队等级 
	public int 			nArmyNum;							// 部队数量 
	public int 			nAttack;							// 攻击 
	public int 			nDefense;							// 防御 
	public int 			nHealth;							// 生命 
}

public class CombatDescUnit
{
	public ulong		nCombatID;
	public string 		EnemyName;
	public int			nCombatType;
	public int 			nRestTime;
	public int 			nStatus;
	public int 			endTimeSec; 
	public int 			nRestTime2;							// 保存当前加速时间的时刻 ...
}

public class CombatProfData
{
	public int 			nProf;
	public int 			n1ArmyType;							// 1部队类型     
	public int 			n2ArmyType;							// 2部队类型    
	public int 			n3ArmyType;							// 3部队类型   
	public int 			n4ArmyType;							// 4部队类型   
	public int 			n5ArmyType;							// 5部队类型     
}

public enum CombatTypeEnum
{
	COMBAT_PVE_RAID						= 1,								// 普通战斗 raid
	COMBAT_PVP_ATTACK					= 2,								// 玩家对战 攻方 attack
	COMBAT_PVP_DEFENSE					= 3,								// 玩家对战 守方 defense
	COMBAT_PVE_WORLDGOLDMINE 			= 10,								// 世界金矿 (AI)
	COMBAT_PVP_WORLDGOLDMINE_ATTACK 	= COMBAT_PVE_WORLDGOLDMINE+1,		// 世界金矿 攻方 attack
	COMBAT_PVP_WORLDGOLDMINE_DEFENSE	= COMBAT_PVE_WORLDGOLDMINE+2,		// 世界金矿 守方 defense
	COMBAT_WORLDCITY					= 20,								// 世界名城 ....
	COMBAT_INSTANCE_BAIZHANBUDAI		= 100,								// 副本, 百战不殆 baizhan
	COMBAT_INSTANCE_NANZHENGBEIZHAN		= 101,								// 副本, 南征北战 101
	
	COMBAT_HERO_TRAINING				= 1000,								// 武将训练   training
	COMBAT_WORLD_RESOURCE				= 1001,								//野外资源 
};

public class CombatFighting 
{
	public enum EnumStatus
	{
		RST_OK = 1,
		NO_DATA = 0,
	}
	
	public string 		szAttackName;
	public string 		szDefenseName;
	public int 			nAttackModel;
	public int 			nDefenseModel;
	public bool 		eVictoryOrNo;
	public ulong		nObjID;
	
	public ulong 		nCombatID;
	public int 			nCombatType = 0;
	public int			nNumA = 0;
	public int 			nNumB = 0;
	public int 			nBackRestTime = 0;
	public int 			endTimeSec = 0;
	public int 			nStatus = 0;
	public CombatCacheUnit[] combatCards;
}
//增援数据 
public struct ReinforceUnit
{
	public uint		DstType;			// 目标类型(1为自己派遣士兵,2为别人派遣士兵给自己)  
	public uint		ExcelID;
	public uint		Level;
	public uint		Num;
	public uint		Type;				// 1为出征,2为召回  
	public ulong	EventID;
	public int		EndTime;
	public ulong	AccountID;
	public string   CharName;
}
static public class Globe {
	public static CombatFighting comFighting = null; 
}

public class CombatManager : MonoBehaviour 
{
	//军情提示图标 
	public GameObject war_situation_icon;
	static CombatManager mInst = null;
	List<CombatDescUnit> combatList = new List<CombatDescUnit>();
	
	// 战斗双方数据缓存 ...
	SortedList<ulong, CombatFighting> combatFightingMap = new SortedList<ulong, CombatFighting>();
	
	// 获取战斗双方数据  ....
	public delegate void ProcessCombatDataDelegate(ulong nCombatID);
	public static ProcessCombatDataDelegate processCombatDataDelegate;
	
	// 开启战斗 ...
	public delegate void ProcessStartCombatDelegate (ulong nCombatID, int nCombatType);
	public delegate void ProcessStartCombatCon1Delegate(ulong nCombatID, int nCombatType, int nAutoCombat);
	public static ProcessStartCombatDelegate processStartCombatDelegate;
	public static ProcessStartCombatCon1Delegate processStartCombatCon1;
	
	// 列举当前的所有战斗事件 ...
	public delegate void ProcessListCombatDataDelegate();
	public static ProcessListCombatDataDelegate processListCombatDataDelegate;
	// 战斗结果返回 ...
	public delegate void ProcessCombatRstDelegate(ulong nCombatID, int nCombatType);
	public static ProcessCombatRstDelegate processCombatRstDelegate;
	// 自动战斗控制 ...
	public delegate void ProcessAutoCombatDelegate();
	public static ProcessAutoCombatDelegate processAutoCombatDelegate;
	// 结束战斗 ...
	public delegate void ProcessStopCombatDelegate(int nCombatType, int nRst, int nCombatRst);
	public static ProcessStopCombatDelegate processStopCombatDelegate;
	// 自动补给标记 ...
	public delegate void ProcessAutoSupplyDelegate();
	public static ProcessAutoSupplyDelegate processAutoSupplyDelegate;
	// 队伍手动补给 ...
	public delegate void ProcessManualSupplyDelegate();
	public static ProcessManualSupplyDelegate processManualSupplyDelegate;
	// 获取敌方战斗力和阵形 ...
	public delegate void ProcessCombatProfDelegate(CombatProfData data);
	public static ProcessCombatProfDelegate processCombatProfDelegate;
	// 战斗聊天信息发送 ...
	public delegate void ProcessChatMsgCombatRstDelegate(string text);
	public static ProcessChatMsgCombatRstDelegate processChatMsgCombatRstDelegate;
	// 军队召回时/战斗结果返回时军情推送 ...
	public delegate void ProcessMarchMsgDelegate();
	public static ProcessMarchMsgDelegate processMarchMsgDelegate;
	// 军队召回 ...
	public delegate void ProcessArmyBackDelegate(ulong nCombatID, int nBackTime);
	public static ProcessArmyBackDelegate processArmyBackDelegate;
	// 军情加速 ...
	public delegate void ProcessArmyAccelDelegate(ulong nCombatID, int nCostCrystal, byte money_type);
	public static ProcessArmyAccelDelegate processArmyAccelDelegate;
	
	//增援军情  
	public List<ReinforceUnit> ReinforceList = new List<ReinforceUnit>();
	//增援加速 
	public delegate void ProcessReinforceAccelDelegate(ulong nEventID, uint nCostCrystal, uint money_type);
	public static ProcessReinforceAccelDelegate processReinforceAccelDelegate;
	// 增援召回 ... 
	public delegate void ProcessReinforceBackDelegate(ulong NewEventID, ulong OldEventID,int nBackTime);
	public static ProcessReinforceBackDelegate processReinforceBackDelegate;
	
	/// <summary>
	/// The instance of the CombatManager class. Will create it if one isn't already around.
	/// </summary>

	static public CombatManager instance
	{
		get
		{
			if (mInst == null)
			{
				mInst = UnityEngine.Object.FindObjectOfType(typeof(CombatManager)) as CombatManager;

				if (mInst == null)
				{
					GameObject go = new GameObject("_CombatManager");
					DontDestroyOnLoad(go);
					mInst = go.AddComponent<CombatManager>();
				}
			}
			
			return mInst;
		}
	}
	
	void Awake() { if (mInst == null) { mInst = this; DontDestroyOnLoad(gameObject); } else { Destroy(gameObject); } }
	void OnDestroy() { if (mInst == this) mInst = null; }
	
	// Use this for initialization
	void Start () {
		StartCoroutine("CheckWarSituaition");
		RequestCombatListData();
		ReqReinforceListData();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	static public void RequestArmyBack(ulong nCombatID)
	{
		CTS_GAMECMD_ARMY_BACK_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ARMY_BACK;
		req.nCombatID3 = nCombatID;
		TcpMsger.SendLogicData<CTS_GAMECMD_ARMY_BACK_T>(req);
	}
	
	public void ProcessArmyBackData(byte[] data)
	{
		STC_GAMECMD_ARMY_BACK_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_ARMY_BACK_T>(data);
		if (msgCmd.nRst1 == (int)STC_GAMECMD_ARMY_BACK_T.enum_rst.RST_OK)
		{
			if (processArmyBackDelegate != null)
			{
				processArmyBackDelegate(msgCmd.nCombatID2, msgCmd.nBackTime3);
			}
			else if (processMarchMsgDelegate != null)
			{
				processMarchMsgDelegate();
			}
			else 
			{
				// 每一秒监测状态 ....
				CombatDescUnit d1 = TryGetCombat(msgCmd.nCombatID2);
				if (d1 != null)
				{
					d1.nStatus = 2; // 标记为返回状态 ...
					d1.nRestTime = msgCmd.nBackTime3;
					d1.endTimeSec = msgCmd.nBackTime3 + DataConvert.DateTimeToInt(DateTime.Now);	
				}
			}
		}
		else {
			
			LoadingManager.instance.HideLoading();
		}
		
		processArmyBackDelegate = null;
		//print("STC_GAMECMD_ARMY_BACK:" + msgCmd.nRst1);
	}
	
	static public void RequestArmyAccel(ulong nCombatID, int money_type, int nTime)
	{
		CTS_GAMECMD_ARMY_ACCELERATE_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ARMY_ACCELERATE;
		req.nCombatID3 = nCombatID;
		req.nTime4 = nTime;
		req.byType5 = (byte) money_type;
		TcpMsger.SendLogicData<CTS_GAMECMD_ARMY_ACCELERATE_T>(req);
	}
	
	public void ProcessArmyAccelData(byte[] data)
	{
		STC_GAMECMD_ARMY_ACCELERATE_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_ARMY_ACCELERATE_T>(data);
		if (msgCmd.nRst1 == (int)STC_GAMECMD_ARMY_ACCELERATE_T.enum_rst.RST_OK)
		{
			if (processArmyAccelDelegate != null)
			{
				processArmyAccelDelegate(msgCmd.nCombatID2, msgCmd.nCostCrystal3, msgCmd.byType4);
			}
		}
		else {
			
			LoadingManager.instance.HideLoading();
		}
		
		processArmyAccelDelegate = null;
		//print("STC_GAMECMD_ARMY_ACCELERATE:" + msgCmd.nRst1);
	}
	//请求加速增援 
	static public void RequestReinforceAccel(ulong nEventID, uint money_type, uint nTime)
	{
		CTS_GAMECMD_OPERATE_ALLI_ACCE_SOLDIER_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_ALLI_ACCE_SOLDIER;
		req.nEventID3 = nEventID;
		req.nTime4 = nTime;
		req.nMoneyType5 = money_type;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_ALLI_ACCE_SOLDIER_T>(req);
	}
	//请求加速增援返回 
	public void ProcessReinforceAccelRst(byte[] data)
	{
		STC_GAMECMD_OPERATE_ALLI_ACCE_SOLDIER_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_ALLI_ACCE_SOLDIER_T>(data);
		if (msgCmd.nRst1 == (int)STC_GAMECMD_OPERATE_ALLI_ACCE_SOLDIER_T.enum_rst.RST_OK)
		{
			if (processReinforceAccelDelegate != null)
			{
				processReinforceAccelDelegate(msgCmd.nEventID2, msgCmd.nMoney4, msgCmd.nMoneyType3);
			}
		}
		else {
			
			LoadingManager.instance.HideLoading();
		}
		
		processReinforceAccelDelegate = null;
	}
	static public void RequestCombatProf(ulong nObjID, int nClassID, int nCombatType)
	{
		CTS_GAMECMD_COMBAT_PROF_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_COMBAT_PROF;
		req.nClass3 = nClassID;
		req.nObjID4 = nObjID;
		req.nCombatType5 = nCombatType;
		TcpMsger.SendLogicData<CTS_GAMECMD_COMBAT_PROF_T>(req);
	}
	
	public void ProcessCombatProfData(byte[] data)
	{
		STC_GAMECMD_COMBAT_PROF_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_COMBAT_PROF_T>(data);
		if (msgCmd.nRst1 == (int)STC_GAMECMD_COMBAT_PROF_T.enum_rst.RST_OK)
		{
			CombatProfData newCard = new CombatProfData();
			newCard.nProf 		= msgCmd.nProf2;
			newCard.n1ArmyType 	= msgCmd.n1ArmyType3;
			newCard.n2ArmyType 	= msgCmd.n2ArmyType4;
			newCard.n3ArmyType 	= msgCmd.n3ArmyType5;
			newCard.n4ArmyType 	= msgCmd.n4ArmyType6;
			newCard.n5ArmyType 	= msgCmd.n5ArmyType7;
			
			if (processCombatProfDelegate != null)
			{
				processCombatProfDelegate(newCard);
			}
		}
		else 
		{
			LoadingManager.instance.HideLoading();
		}
		
		processCombatProfDelegate = null;
		//print("STC_GAMECMD_COMBAT_PROF:" + msgCmd.nRst1);
	}
	
	static public void RequestStopAutoCombat(ulong nInstanceID, int nVal)
	{
		CTS_GAMECMD_AUTO_COMBAT_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_AUTO_COMBAT;
		req.nInstanceID3 = nInstanceID;
		req.nVal4 = nVal;
		TcpMsger.SendLogicData<CTS_GAMECMD_AUTO_COMBAT_T>(req);
	}
	
	public void ProcessStopAutoCombatData(byte[] data)
	{
		STC_GAMECMD_AUTO_COMBAT_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_AUTO_COMBAT_T>(data);
		if (msgCmd.nRst1 == (int)STC_GAMECMD_AUTO_COMBAT_T.enum_rst.RST_OK)
		{
			if (processAutoCombatDelegate != null)
			{
				processAutoCombatDelegate();
			}
		}
		else 
		{
			LoadingManager.instance.HideLoading();
		}
		
		processAutoCombatDelegate = null;
		//print("STC_GAMECMD_AUTO_COMBAT:" + msgCmd.nRst1);
	}
	
	public void ProcessAutoSupplyData(byte[] data)
	{
		STC_GAMECMD_AUTO_SUPPLY_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_AUTO_SUPPLY_T>(data);
		if (msgCmd.nRst1 == (int)STC_GAMECMD_AUTO_SUPPLY_T.enum_rst.RST_OK)
		{
			if (processAutoSupplyDelegate != null)
			{
				processAutoSupplyDelegate();
			}
		}
		else
		{	
			LoadingManager.instance.HideLoading();
		}
		
		processAutoSupplyDelegate = null;
		//print("STC_GAMECMD_AUTO_SUPPLY:" + msgCmd.nRst1);
	}
	
	public void ProcessManualSupplyDrugData(byte[] data)
	{
		STC_GAMECMD_SUPPLY_DRUG_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_SUPPLY_DRUG_T>(data);
		CommonData.player_online_info.CaoYao = (uint) msgCmd.nNum1;		
		//print("STC_GAMECMD_SUPPLY_DRUG:" + msgCmd.nNum1); // 草药数 ...
	}
	
	public void ProcessManualSupplyData(byte[] data)
	{
		STC_GAMECMD_MANUAL_SUPPLY_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_MANUAL_SUPPLY_T>(data);
		if (msgCmd.nRst1 == (int)STC_GAMECMD_MANUAL_SUPPLY_T.enum_rst.RST_OK)
		{				
			if (msgCmd.nNum2>0)
			{
				int num = msgCmd.nNum2;
				int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_MANUAL_SUPPLY_T>();
				int data_len = data.Length - head_len;
				byte[] data_buff = new byte[data_len];
				System.Array.Copy(data,head_len,data_buff,0,data_len);
				InstanceHeroDesc[] card_array = (InstanceHeroDesc[])DataConvert.ByteToStructArray<InstanceHeroDesc>(data_buff,num);
				
				Hashtable jlMap = JiangLingManager.MyHeroMap;
				Hashtable jlsosMap = SoldierManager.SoldierMap;
				
				string first_nofree_hero = "-";
				int msgStatus = 0;
				bool noFull = false;
				for(int i=0; i<num; ++ i)
				{
					InstanceHeroDesc g = card_array[i];
					if (true == jlMap.ContainsKey(g.nHeroID1))
					{
						HireHero h1Hero = (HireHero) jlMap[g.nHeroID1];
						int delta = h1Hero.nArmyNum11 - g.nArmyNum2;
						//print ("+Auto Hero SoldierUnit:" + g.nHeroID1 + "," + delta); // 补给 ...
						h1Hero.nArmyNum11 = g.nArmyNum2;
						h1Hero.nProf20	= g.nArmyProf3;
						h1Hero.nHealthState21 = g.nHealthState4;
						h1Hero.nLevel19 = g.nLevel5;
						jlMap[g.nHeroID1] = h1Hero;
						
						string k = string.Format("{0}_{1}",h1Hero.nArmyType9,h1Hero.nArmyLevel10);
						if (true == jlsosMap.ContainsKey(k))
						{
							SoldierUnit si = (SoldierUnit) jlsosMap[k];
							
							int nLastNum = si.nNum3; 
							si.nNum3 += delta;
							jlsosMap[k] = si;
							
							// 如果上次的兵数>0 但是并没有补给上,可能是状态不对了 ...
							if (nLastNum>0 && delta == 0)
							{
								if (msgStatus == 0 && (h1Hero.nStatus14 != (int)CMNCMD.HeroState.NORMAL))
								{
									msgStatus = 1;
									first_nofree_hero = DataConvert.BytesToStr(h1Hero.szName3);
								}
							}
						}
					}
					
					if (g.nArmyNum2 < g.nMaxArmyNum6) { noFull = true; }
				}

				if (processManualSupplyDelegate != null)
				{
					processManualSupplyDelegate();
				
					// 手动补给结果的, 未补满情况 ...
					if (noFull == true)
					{
						if (msgStatus == 1)
						{
							int Tipset1 = BaizVariableScript.INSTANCE_SUPPLY_RST_NO_FREE;
							string cc = U3dCmn.GetWarnErrTipFromMB(Tipset1);
							string cs1 = string.Format(cc, first_nofree_hero);
							PopTipDialog.instance.VoidSetText2(true, false, cs1);
						}
						else 
						{
							int Tipset = BaizVariableScript.INSTANCE_SUPPLY_NOFULL;
							PopTipDialog.instance.VoidSetText1(true,false,Tipset);
						}
					}
				}
			}
			else
			{
				// 取消 Loading ...
				LoadingManager.instance.HideLoading();
			}
		}
		else 
		{
			LoadingManager.instance.HideLoading();
			int Tipset = 0;
			switch (msgCmd.nRst1)
			{
			case (int)STC_GAMECMD_MANUAL_SUPPLY_T.enum_rst.RST_WORLDGOLDMINE:
				{
					Tipset = BaizVariableScript.WORLDGOLDMINE_SUPPLY_ISNULL;
				} break;
			case (int)STC_GAMECMD_MANUAL_SUPPLY_T.enum_rst.RST_INSTANCE:
				{
					Tipset = BaizVariableScript.INSTANCE_SUPPLY_ISNULL;
				} break;
				
			default: break;
			}
			
			PopTipDialog.instance.VoidSetText1(true, false, Tipset);
		}
		
		processManualSupplyDelegate = null;
		// print("STC_GAMECMD_MANUAL_SUPPLY:" + msgCmd.nRst1);
	}
	// 在指定的倍数位置更换不同的图标 ....
	static int Head2ObjID(int[] ac, int nObjID, int n0)
	{
		for (int i=ac.Length-1; i>0; -- i)
		{
			if (nObjID % ac[i] == 0) return i;
		}
		
		// Fini
		return n0;
	}
	// 随机取出头像 ID 和战斗双方名字 ...
	static void Fighting2name(int nCombatType, int nObjID, string objName, int nHeadID, ref CombatFighting comFighting)
	{
		CMN_PLAYER_CARD_INFO info1 = CommonData.player_online_info;
		comFighting.szAttackName = info1.CharName;
		comFighting.nAttackModel = (int) info1.nHeadID;
		
		if (nCombatType == (int)CombatTypeEnum.COMBAT_PVP_DEFENSE ||
			nCombatType == (int)CombatTypeEnum.COMBAT_PVP_WORLDGOLDMINE_DEFENSE)
		{
			comFighting.szDefenseName = info1.CharName;
			comFighting.nDefenseModel = (int) info1.nHeadID;
			comFighting.szAttackName = objName;
			comFighting.nAttackModel = nHeadID;
		}
		else if (nCombatType == (int)CombatTypeEnum.COMBAT_PVP_ATTACK ||
			nCombatType == (int)CombatTypeEnum.COMBAT_PVP_WORLDGOLDMINE_ATTACK)
		{
			comFighting.szDefenseName = objName;
			comFighting.nDefenseModel = nHeadID;
		}
		else if (nCombatType == (int)CombatTypeEnum.COMBAT_INSTANCE_BAIZHANBUDAI)
		{
			int Tipset = BaizVariableScript.COMBAT_TYPE_INSTANCE_BAIZHANBUDAI;
			comFighting.szDefenseName = U3dCmn.GetWarnErrTipFromMB(Tipset);
			
			int[] ac = new int[7] { 2, 4, 8, 16, 32, 48, 64 }; 
			comFighting.nDefenseModel = Head2ObjID(ac, nObjID, 5);
		}
		else if (nCombatType == (int)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN)
		{
			int Tipset = BaizVariableScript.COMBAT_TYPE_INSTANCE_NANZHENGBEIZHAN;
			comFighting.szDefenseName = U3dCmn.GetWarnErrTipFromMB(Tipset);
			int[] ac = new int[7] { 3, 5, 7, 9, 11, 13, 17 }; 
			comFighting.nDefenseModel = Head2ObjID(ac, nObjID, 2);
		}
		else if (nCombatType == (int)CombatTypeEnum.COMBAT_PVE_WORLDGOLDMINE)
		{
			int Tipset = BaizVariableScript.COMBAT_TYPE_PVE_WORLDGOLDMINE;
			comFighting.szDefenseName = U3dCmn.GetWarnErrTipFromMB(Tipset);
			int[] ac = new int[7] { 3, 5, 7, 9, 11, 13, 17 }; 
			comFighting.nDefenseModel = Head2ObjID(ac, nObjID, 4);
		}
		else if (nCombatType == (int)CombatTypeEnum.COMBAT_WORLDCITY)
		{
			int Tipset = BaizVariableScript.COMBAT_TYPE_PVE_WORLDCITY;
			comFighting.szDefenseName = U3dCmn.GetWarnErrTipFromMB(Tipset);
			int[] ac = new int[7] { 3, 5, 7, 9, 11, 13, 17 };
			comFighting.nDefenseModel = Head2ObjID(ac, nObjID, 4);
		}
		else if (nCombatType == (int)CombatTypeEnum.COMBAT_PVE_RAID)
		{	
			Hashtable qwMap = CommonMB.WildBossMBInfo_Map;
			uint qwID = (uint) nObjID;
			if (true == qwMap.ContainsKey(qwID)) 
			{
				WildBossMBInfo card = (WildBossMBInfo)qwMap[qwID];
				comFighting.szDefenseName = card.name;
			}
			
			int[] ac = new int[7] { 3, 5, 7, 9, 11, 13, 17 }; 
			comFighting.nDefenseModel = Head2ObjID(ac, nObjID, 6);
		}
		else if (nCombatType == (int)CombatTypeEnum.COMBAT_WORLD_RESOURCE)
		{
			int Tipset = BaizVariableScript.COMBAT_TYPE_WORLD_RESOURCE;
			comFighting.szDefenseName = U3dCmn.GetWarnErrTipFromMB(Tipset);
			int[] ac = new int[7] { 3, 5, 7, 9, 11, 13, 17 };
			comFighting.nDefenseModel = Head2ObjID(ac, nObjID, 4);
		}
		
	}
	
	/// <summary>
	/// Processes the combat rst data.
	/// </summary>
	public void ProcessCombatRstData(byte[] data)
	{
		STC_GAMECMD_COMBAT_RST_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_COMBAT_RST_T>(data);
		if (msgCmd.nRst7 == (int)STC_GAMECMD_COMBAT_RST_T.enum_rst.RST_OK ||
			msgCmd.nRst7 == (int)STC_GAMECMD_COMBAT_RST_T.enum_rst.RST_OK_ZHANBAI )
		{
			// 可以获取武将信息 msgCmd.nNum5;
			if (msgCmd.nNum8>0)
			{
				int num = msgCmd.nNum8;
				int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_COMBAT_RST_T>();
				int data_len = data.Length - head_len;
				byte[] data_buff = new byte[data_len];
				System.Array.Copy(data,head_len,data_buff,0,data_len);
				CombatRst[] card_array = (CombatRst[])DataConvert.ByteToStructArray<CombatRst>(data_buff,num);
			
				Hashtable jlMap = JiangLingManager.MyHeroMap;
				for (int i=0; i<num; ++ i)
				{
					CombatRst g = card_array[i];
					if (true == jlMap.ContainsKey(g.nHeroID1))
					{
						HireHero h1Hero = (HireHero) jlMap[g.nHeroID1];
						h1Hero.nArmyNum11 = g.nArmyNum2; // 给个英雄等级 ....
						h1Hero.nHealthState21 = g.nHealthStatus6;
						h1Hero.nLevel19 = g.nHeroLevel9;
						h1Hero.nProf20 = g.nArmyProf7;
						h1Hero.nExp22 = g.nHeroExp10;
						jlMap[g.nHeroID1] = h1Hero;
					}
				}
				
				CombatFighting comFighting = null;
				if (true == combatFightingMap.TryGetValue(msgCmd.nCombatID1, out comFighting))
				{
					// 战斗是否胜利 Victory 
					comFighting.nCombatType = msgCmd.nCombatType2;
					comFighting.eVictoryOrNo = (msgCmd.nRst7 == 1);
					comFighting.nObjID = msgCmd.nObjID3;
					
					string objName = DataConvert.BytesToStr(msgCmd.szObjName5);
					Fighting2name(msgCmd.nCombatType2, (int) msgCmd.nObjID3, objName, msgCmd.nObjHeadID6, ref comFighting);					
					combatFightingMap[msgCmd.nCombatID1] = comFighting;
					
					// 获取战斗数据进模拟 CombatData
					if (processCombatRstDelegate != null)
					{
						processCombatRstDelegate(msgCmd.nCombatID1, msgCmd.nCombatType2);
					}
				}
			}
			
			// 每次战斗结果, 统计更新关卡数 ....
			if (msgCmd.nCombatType2 == (int)CombatTypeEnum.COMBAT_INSTANCE_BAIZHANBUDAI)
			{
				if(msgCmd.nRst7 == 1)
				{
					int nCurLevel = (int) msgCmd.nObjID3;
					if(BaizInstanceManager.EasyOrEpic == 2)
					{
						if ((int)CommonData.player_online_info.WangZhe-100 < nCurLevel) 
						{
							CommonData.player_online_info.WangZhe = (uint)(nCurLevel+100);
						}
					}
					else
					{
						if ((int)CommonData.player_online_info.WangZhe < nCurLevel) {
							CommonData.player_online_info.WangZhe = (uint)nCurLevel;
						}
					}
				}
				
			}
			else if (msgCmd.nCombatType2 == (int)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN)
			{
				if(msgCmd.nObjID3 == 20 && msgCmd.nRst7 ==1)
				{
					if(CommonData.player_online_info.ZhengZhan < (uint) msgCmd.nClassID9)
						CommonData.player_online_info.ZhengZhan = (uint) msgCmd.nClassID9;
				}
			
			}
			
			// 聊天推送...
			if (processChatMsgCombatRstDelegate != null)
			{
				AddChatMsgCombatRstDelegate(msgCmd.nCombatType2, (int) msgCmd.nObjID3, msgCmd.nClassID9, msgCmd.nRst7);
			}			
			
			// 军情推送 ...
			if (processMarchMsgDelegate != null)
			{
				processMarchMsgDelegate();
			}
			else { // 军情界面未存在 ...
				processAutoMarchCon1();
			}
		}
		
		//print("STC_GAMECMD_COMBAT_RST:" + msgCmd.nRst7 + "," + msgCmd.nNum8 + "," + msgCmd.nCombatType2);
	}
	
	void AddChatMsgCombatRstDelegate(int nCombatType, int nGuard, int nClass, int nRst)
	{
		int Tip1 = 0;
		string combatRst;
		string csName = "-";
		if (nRst == (int)STC_GAMECMD_COMBAT_RST_T.enum_rst.RST_OK) {
			combatRst = U3dCmn.GetWarnErrTipFromMB(50);
		}
		else {
			combatRst = U3dCmn.GetWarnErrTipFromMB(51);
		}
		
		if (nCombatType == (int)CombatTypeEnum.COMBAT_INSTANCE_BAIZHANBUDAI)
		{
			// 
			if (nClass == 1) {
				csName = U3dCmn.GetWarnErrTipFromMB(148);
			}
			else if (nClass == 2) {
				csName = U3dCmn.GetWarnErrTipFromMB(149);
			}
			Tip1 = BaizVariableScript.CHAT_MSG_INSTANCE_BAIZHANBUDAI;
		}
		else if (nCombatType == (int)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN)
		{
			Hashtable nzbzMap = CommonMB.NanBeizhanInfo_Map;
			uint ExcelID = (uint)nClass;
			if (true == nzbzMap.ContainsKey(ExcelID))
			{
				NanBeizhanMBInfo card = (NanBeizhanMBInfo) nzbzMap[ExcelID];
				csName = card.name;
			}
			Tip1 = BaizVariableScript.CHAT_MSG_INSTANCE_NANZHENGBEIZHAN;
		}
		
		if (Tip1 > 0)
		{
			string cc = U3dCmn.GetWarnErrTipFromMB(Tip1);
			string c1 = string.Format(cc, csName, nGuard, combatRst);
			processChatMsgCombatRstDelegate(c1);
		}
	}
	
	void Hero2normal(ulong[] nHeroArray, int num)
	{		
		Hashtable jlMap = JiangLingManager.MyHeroMap;
		for (int i=0; i<num; ++ i)
		{
			ulong n1HeroID = nHeroArray[i];
			if (true == jlMap.ContainsKey(n1HeroID))
			{
				HireHero h1Hero = (HireHero) jlMap[n1HeroID];
				h1Hero.nStatus14 = (int) CMNCMD.HeroState.NORMAL;
				jlMap[n1HeroID] = h1Hero;
			}
		}
	}
	
	void Hero2chat(int nCombatType, ulong[] nHeroArray, int num)
	{		
		int Tipset = 0;
		if (nCombatType == (int)CombatTypeEnum.COMBAT_INSTANCE_BAIZHANBUDAI) {
			Tipset = BaizVariableScript.CHAT_MSG_BAIZHANBUDAI_IS_FREE;
		}
		else if (nCombatType == (int)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN) {
			Tipset = BaizVariableScript.CHAT_MSG_NANZHENGBEIZHAN_IS_FREE;
		}
		
		if (Tipset == 0) return;
		Hashtable jlMap = JiangLingManager.MyHeroMap;
		for (int i=0; i<num; ++ i)
		{
			ulong n1HeroID = nHeroArray[i];
			if (true == jlMap.ContainsKey(n1HeroID))
			{
				HireHero h1Hero = (HireHero) jlMap[n1HeroID];
				// 带兵数为 0的标记下 ...
				if ( h1Hero.nArmyNum11 == 0)
				{
					string name = DataConvert.BytesToStr(h1Hero.szName3);
					string cc = U3dCmn.GetWarnErrTipFromMB(Tipset);
					processChatMsgCombatRstDelegate( string.Format(cc, name));
					
					return;
				}
			}
		}
	}
	
	/// <summary>
	/// Processes the stop combat data.
	/// </summary>
	public void ProcessStopCombatData(byte[] data)
	{
		STC_GAMECMD_OPERATE_STOP_COMBAT_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_STOP_COMBAT_T>(data);
		
		ulong[] nHeroArray = new ulong[5];
		nHeroArray[0] = msgCmd.n1HeroID5;
		nHeroArray[1] = msgCmd.n2HeroID6;
		nHeroArray[2] = msgCmd.n3HeroID7;
		nHeroArray[3] = msgCmd.n4HeroID8;
		nHeroArray[4] = msgCmd.n5HeroID9;
		
		bool autoStopCombat = false;
		switch (msgCmd.nRst1)
		{
		case (int) STC_GAMECMD_OPERATE_STOP_COMBAT_T.enum_rst.RST_ERR:
			{
				//print ("STC_GAMECMD_OPERATE_STOP_COMBAT - RST_ERR:" + msgCmd.nCombatID3);
				return;
			} 
		case (int) STC_GAMECMD_OPERATE_STOP_COMBAT_T.enum_rst.RST_OK:
			{
				autoStopCombat = true;
			
				// 征战天下是锁定状态的 ...
				if (msgCmd.nCombatType4 != (int)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN)
				{
					Hero2normal(nHeroArray, 5); // 武将切换为空闲状态 ...
				}
			} break;
		case (int) STC_GAMECMD_OPERATE_STOP_COMBAT_T.enum_rst.RST_RESTART:
			{
				autoStopCombat = true;
				// 征战天下是锁定状态的 ...
				if (msgCmd.nCombatType4 != (int)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN)
				{
					Hero2normal(nHeroArray, 5); // 武将切换为空闲状态 ...
				}
				if (processChatMsgCombatRstDelegate != null) 
				{
					Hero2chat(msgCmd.nCombatType4, nHeroArray, 5);
				}
			} break;
			
		case (int) STC_GAMECMD_OPERATE_STOP_COMBAT_T.enum_rst.RST_AUTO_COMBAT:
			{
				//print("STC_GAMECMD_OPERATE_STOP_COMBAT - RST_AUTO_COMBAT:" + 
				//	msgCmd.nCombatID3 + "," + msgCmd.nNewCombatID11 + "," + msgCmd.nNum14);
			
				CombatFighting comFighting = null;
				if (true == combatFightingMap.TryGetValue(msgCmd.nNewCombatID11,out comFighting))
				{	
					autoStopCombat = false;
				}
				else 
				{
					comFighting = new CombatFighting();
					autoStopCombat = (msgCmd.nCombatID3 != msgCmd.nNewCombatID11);					
				}
			
				if (comFighting != null)
				{
					comFighting.nCombatID = msgCmd.nNewCombatID11;
					comFighting.nBackRestTime = msgCmd.nBackTime13;
					comFighting.endTimeSec = 0;
					comFighting.nStatus = (int) CombatFighting.EnumStatus.NO_DATA;
					combatFightingMap[msgCmd.nNewCombatID11] = comFighting;
				}
			
				// 扣除草药使用量 ...
				if (msgCmd.nDrug10>0)
				{
					CommonData.player_online_info.CaoYao = (uint) msgCmd.nDrug10;
					//print ("+ Auto Drugs:" + msgCmd.nDrug10); // 测试当前草药用量 ...
				}
			
				// 自动补给开启后，每结束时会有新补给数据 auto supply
				if (msgCmd.nNum14>0)
				{
					int num = msgCmd.nNum14;
					int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_OPERATE_STOP_COMBAT_T>();
					int data_len = data.Length - head_len;
					byte[] data_buff = new byte[data_len];
					System.Array.Copy(data,head_len,data_buff,0,data_len);
					InstanceHeroDesc[] card_array = (InstanceHeroDesc[])DataConvert.ByteToStructArray<InstanceHeroDesc>(data_buff,num);
	
					Hashtable jlMap = JiangLingManager.MyHeroMap;
					Hashtable jlsosMap = SoldierManager.SoldierMap;

					for(int i=0; i<num; ++ i)
					{
						InstanceHeroDesc g = card_array[i];
						//print ("+ Auto HeroID SoliderUnit:" + g.nHeroID1);
						if (true == jlMap.ContainsKey(g.nHeroID1))
						{
							HireHero h1Hero = (HireHero) jlMap[g.nHeroID1];
							int delta = h1Hero.nArmyNum11 - g.nArmyNum2;
							//print ("+ Auto Hero SoliderUnit:" + g.nHeroID1 + "," + delta); // 测试自动补给数量 ...
							h1Hero.nArmyNum11 = g.nArmyNum2;
							h1Hero.nProf20	= g.nArmyProf3;
							h1Hero.nHealthState21 = g.nHealthState4;
							// h1Hero.nStatus14 = 1;	// 标记非空闲 nn
							jlMap[g.nHeroID1] = h1Hero;
													
							string k = string.Format("{0}_{1}",h1Hero.nArmyType9,h1Hero.nArmyLevel10);
							if (true == jlsosMap.ContainsKey(k))
							{
								SoldierUnit new1 = (SoldierUnit) jlsosMap[k];
								new1.nNum3 = new1.nNum3 + delta;
								jlsosMap[k] = new1;
							}
						}
					}
				}
	
			} break;
			
		case (int) STC_GAMECMD_OPERATE_STOP_COMBAT_T.enum_rst.RST_INSTANCE_FINISH:
		case (int) STC_GAMECMD_OPERATE_STOP_COMBAT_T.enum_rst.RST_INSTANCE_FAILURE:
			{
				autoStopCombat = true;
				Hero2normal(nHeroArray, 5); // 武将切换为空闲状态 ...
			} break;
			
		default:
			{
				autoStopCombat = true;
				// 征战天下是锁定状态的 ...
				if (msgCmd.nCombatType4 != (int)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN)
				{
					Hero2normal(nHeroArray, 5); // 武将切换为空闲状态 ...
				}
			} break;
		}
		
		// 常规战斗结束 ...
		if (processStopCombatDelegate != null)
		{
			processStopCombatDelegate(msgCmd.nCombatType4, msgCmd.nRst1, msgCmd.nCombatRst2);
		}
	
		// 战斗列表数据中删除 StopCombat
		if (true == autoStopCombat)
		{
			if (true == combatFightingMap.ContainsKey(msgCmd.nCombatID3))
			{	
				if (Globe.comFighting != null)
				{
					Globe.comFighting = null;
				}
				
				combatFightingMap.Remove(msgCmd.nCombatID3);
			}
		}
		
		// 军情推送 ...
		if (processMarchMsgDelegate != null)
		{
			processMarchMsgDelegate ();
		}
		else 
		{ // 军情界面未打开 ...
			processAutoMarchCon1();
		}
		
		//print( "STC_GAMECMD_OPERATE_STOP_COMBAT:" + msgCmd.nRst1 + "," + msgCmd.nCombatID3 + "," + msgCmd.nCombatRst2);
	}
	
	/// <summary>
	/// Processes the start combat data.
	/// </summary>
	/// <param name='data'>
	/// Data.
	/// </param>
	public void ProcessStartCombatData(byte[] data)
	{
		STC_GAMECMD_OPERATE_START_COMBAT_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_START_COMBAT_T>(data);
		
		if (msgCmd.nRst1 == (int)STC_GAMECMD_OPERATE_START_COMBAT_T.enum_rst.RST_OK)
		{
			CombatFighting comFighting = null;
			if (false == combatFightingMap.TryGetValue(msgCmd.nCombatID2,out comFighting))
			{
				comFighting = new CombatFighting();
				comFighting.nCombatID = msgCmd.nCombatID2;
				comFighting.nBackRestTime = msgCmd.nBackTime5;
				comFighting.endTimeSec = 0;
				combatFightingMap[msgCmd.nCombatID2] = comFighting;
			}
			if (msgCmd.nCombatType3 == (int)CombatTypeEnum.COMBAT_PVE_RAID ||
				msgCmd.nCombatType3 == (int)CombatTypeEnum.COMBAT_PVP_ATTACK )
			{
				CombatDescUnit newCard = new CombatDescUnit();
				newCard.nCombatID = msgCmd.nCombatID2;
				newCard.nCombatType = msgCmd.nCombatType3;
				newCard.nRestTime = msgCmd.nGoTime4 + msgCmd.nBackTime5;
				newCard.endTimeSec = newCard.nRestTime + DataConvert.DateTimeToInt(DateTime.Now);
				newCard.nStatus = 1;
				combatList.Add(newCard);
			}
			
			if (processStartCombatDelegate != null)
			{
				processStartCombatDelegate(msgCmd.nCombatID2, msgCmd.nCombatType3);
			}
			else if (processStartCombatCon1 != null)
			{
				processStartCombatCon1(msgCmd.nCombatID2, msgCmd.nCombatType3,msgCmd.nAutoCombat6);
			}
		}
		else 
		{	
			LoadingManager.instance.HideLoading();
			int Tipset = 0;
			
			switch (msgCmd.nRst1)
			{
			case (int)STC_GAMECMD_OPERATE_START_COMBAT_T.enum_rst.RST_HERO_ISNULL:
				{
					Tipset = BaizVariableScript.COMBAT_HERO_ISNULL;
				} break;
			case (int)STC_GAMECMD_OPERATE_START_COMBAT_T.enum_rst.RST_LEADER:
				{
					Tipset = BaizVariableScript.COMBAT_NOT_LEADER;
				} break;
			case (int)STC_GAMECMD_OPERATE_START_COMBAT_T.enum_rst.RST_HERO_ERR:
				{
					Tipset = BaizVariableScript.COMBAT_HERO_NOT_FREE;
				} break;
			case (int)STC_GAMECMD_OPERATE_START_COMBAT_T.enum_rst.RST_NOTEND:
				{
					Tipset = BaizVariableScript.COMBAT_NOT_END;
				} break;
			case (int)STC_GAMECMD_OPERATE_START_COMBAT_T.enum_rst.RST_OBJ_NULL:
				{
					Tipset = BaizVariableScript.COMBAT_OBJ_IS_NULL;
				} break;
			case (int)STC_GAMECMD_OPERATE_START_COMBAT_T.enum_rst.RST_PROTECT:
				{
					Tipset = BaizVariableScript.COMBAT_RST_PROTECT;
				} break;
			case (int)STC_GAMECMD_OPERATE_START_COMBAT_T.enum_rst.RST_PVPQUEUE:
				{
					Tipset = BaizVariableScript.COMBAT_RST_PVPQUEUE;
				} break;

			default: break;
			}
			
			PopTipDialog.instance.VoidSetText1(true, false, Tipset);
		}
			
		// 默认为一次处理 delegate
		processStartCombatDelegate = null;
		//print("STC_GAMECMD_OPERATE_START_COMBAT:" + msgCmd.nRst1);
	}

	/// <summary>
	/// Requests the get combat data.
	/// </summary>
	public static void RequestGetCombatData(ulong combatID)
	{
		CTS_GAMECMD_OPERATE_GET_COMBAT_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_GET_COMBAT;
		req.nCombatID3 = combatID;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_GET_COMBAT_T>(req);
	}
	
	/// <summary>
	/// Processes the combat unit data.
	/// </summary>
	/// 
	public void ProcessCombatUnitData(byte[] data)
	{
		STC_GAMECMD_OPERATE_GET_COMBAT_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_GET_COMBAT_T>(data);
		if (msgCmd.nRst1 == (int)STC_GAMECMD_OPERATE_GET_COMBAT_T.enum_rst.RST_OK)
		{
			// 取消 Loading ...
			LoadingManager.instance.HideLoading();
			
			// 战斗序列获取 ...
			int num = msgCmd.nNumA4 + msgCmd.nNumB5;
			if (num > 0)
			{
				int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_OPERATE_GET_COMBAT_T>();
				int data_len = data.Length - head_len;
				byte[] data_buff = new byte[data_len];
				System.Array.Copy(data,head_len,data_buff,0,data_len);
				CombatUnit[] card_array = (CombatUnit[])DataConvert.ByteToStructArray<CombatUnit>(data_buff,num);
				
				CombatFighting comFighting = null;
				if (true == combatFightingMap.TryGetValue(msgCmd.nCombatID2,out comFighting))
				{
					// 战斗实例 endTimeSec
					comFighting.endTimeSec = comFighting.nBackRestTime + DataConvert.DateTimeToInt(DateTime.Now);
					comFighting.combatCards = new CombatCacheUnit[num];
					comFighting.nCombatType = msgCmd.nCombatType3;
					
					// 要再次确定目录头像和名字 ...
					int objHeadID = msgCmd.nObjHeadID8;	
					int objID = (int) comFighting.nObjID;
					string objName = DataConvert.BytesToStr(msgCmd.szObjName7);
					Fighting2name(msgCmd.nCombatType3, objID, objName, objHeadID, ref comFighting);
					
					// 添加战斗单元数据 ...
					for (int i=0; i<num; ++ i)
					{
						CombatUnit g = card_array[i];
						CombatCacheUnit newCard = new CombatCacheUnit();
						newCard.HeroName 	= DataConvert.BytesToStr(g.szName4);
						newCard.nArmyType 	= g.nArmyType5;
						newCard.nArmyLevel 	= g.nArmyLevel6;
						newCard.nArmyNum 	= g.nArmyNum7;
						newCard.nAttack 	= g.nArmyAttack8;
						newCard.nDefense 	= g.nArmyDefense9;
						newCard.nHealth 	= g.nHealth10;
						newCard.nHeroID 	= g.nHeroID2;
						newCard.nModel		= g.nModel11;
						newCard.nLevel		= g.nLevel12;
						newCard.nSlot 		= g.nSlot1;
						
						comFighting.combatCards[i] = newCard;
					}
					
					// 战况双方 CombatFighting
					comFighting.nNumA = msgCmd.nNumA4;
					comFighting.nNumB = msgCmd.nNumB5;
					comFighting.nStatus = (int)CombatFighting.EnumStatus.RST_OK;
					combatFightingMap[msgCmd.nCombatID2] = comFighting;
				}
				
				// 消息 Delegate 
				if (processCombatDataDelegate != null)
				{
					processCombatDataDelegate(msgCmd.nCombatID2);
				}
			}
		}
		else {
			
			// 取消 Loading ...
			LoadingManager.instance.HideLoading();
		}
		
		// 属于一次获取处理的 delegate
		processCombatDataDelegate = null;
		//print ("STC_GAMECMD_OPERATE_GET_COMBAT:" + msgCmd.nCombatType3 + "," + msgCmd.nNumA4 + "," + msgCmd.nNumB5);
	}
	
	/// <summary>
	/// Request the combat list data.
	/// </summary>
	public static void RequestCombatListData()
	{
		CTS_GAMECMD_OPERATE_LIST_COMBAT_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_LIST_COMBAT;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_LIST_COMBAT_T>(req);
	}
	//请求增援信息 
	public static void ReqReinforceListData()
	{
		CTS_GAMECMD_GET_DONATE_SOLDIER_QUEUE_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_DONATE_SOLDIER_QUEUE;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_DONATE_SOLDIER_QUEUE_T >(req);
		//LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_DONATE_SOLDIER_QUEUE);
	}
	// 战斗类型提示 ...
	public static int GetCombatTipset(int combatType)
	{
		int Tipset = 0;
		if (combatType == (int)CombatTypeEnum.COMBAT_PVE_RAID) {
			Tipset = BaizVariableScript.COMBAT_TYPE_PVE_RAID;
		}
		else if (combatType == (int)CombatTypeEnum.COMBAT_WORLDCITY) {
			Tipset = BaizVariableScript.COMBAT_TYPE_PVE_WORLDCITY;
		}
		else if (combatType == (int)CombatTypeEnum.COMBAT_PVE_WORLDGOLDMINE) {
			Tipset = BaizVariableScript.COMBAT_TYPE_PVE_WORLDGOLDMINE;
		}
		else if (combatType == (int)CombatTypeEnum.COMBAT_INSTANCE_BAIZHANBUDAI) {
			Tipset = BaizVariableScript.COMBAT_TYPE_INSTANCE_BAIZHANBUDAI;
		}
		else if (combatType == (int)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN) {
			Tipset = BaizVariableScript.COMBAT_TYPE_INSTANCE_NANZHENGBEIZHAN;
		}
		else if (combatType == (int)CombatTypeEnum.COMBAT_WORLD_RESOURCE) {
			Tipset = BaizVariableScript.COMBAT_TYPE_WORLD_RESOURCE;
		}
		return Tipset;
	}
	
	// 战斗 PVE的武将描述 .... 
	public static int GetCombatGeneral(int combatType)
	{
		int Tipset = 0;
		if (combatType == (int)CombatTypeEnum.COMBAT_PVE_RAID) {
			Tipset = BaizVariableScript.COMBAT_GENERAL_PVE_RAID;
		}
		else if (combatType == (int)CombatTypeEnum.COMBAT_PVE_WORLDGOLDMINE) {
			Tipset = BaizVariableScript.COMBAT_GENERAL_PVE_WORLDGOLDMINE;
		}
		else if (combatType == (int)CombatTypeEnum.COMBAT_INSTANCE_BAIZHANBUDAI) {
			Tipset = BaizVariableScript.COMBAT_GENERAL_PVE_BAIZHANBUDAI;
		}
		else if (combatType == (int)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN) {
			Tipset = BaizVariableScript.COMBAT_GENERAL_PVE_NANZHENGBEIZHAN;
		}
		else if (combatType == (int)CombatTypeEnum.COMBAT_WORLDCITY) {
			Tipset = BaizVariableScript.COMBAT_GENERAL_PVE_WORLDCITY;
		}
		else if (combatType == (int)CombatTypeEnum.COMBAT_WORLD_RESOURCE) {
			Tipset = BaizVariableScript.COMBAT_GENERAL_PVE_WORLDCITY;
		}
		return Tipset;
	}
	
	/// <summary>
	/// Processes the list combat data.
	/// </summary>
	/// <param name='data'>
	/// Data.
	/// </param>
	public void ProcessListCombatData(byte[] data)
	{			
		STC_GAMECMD_OPERATE_LIST_COMBAT_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_LIST_COMBAT_T>(data);
		if (msgCmd.nRst1 == (int)STC_GAMECMD_OPERATE_LIST_COMBAT_T.enum_rst.RST_OK)
		{				
			combatList.Clear();
			
			if (msgCmd.nNum2 > 0)
			{
				int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_OPERATE_LIST_COMBAT_T>();
				int data_len = data.Length - head_len;
				byte[] data_buff = new byte[data_len];
				System.Array.Copy(data,head_len,data_buff,0,data_len);
				CombatDesc[] card_array = (CombatDesc[])DataConvert.ByteToStructArray<CombatDesc>(data_buff,msgCmd.nNum2);
				
				// Store combat desc for data
				for (int i=0; i<msgCmd.nNum2; ++ i)
				{
					CombatDesc g = card_array[i];
					CombatDescUnit newCard = new CombatDescUnit();
					
					if (g.nRestTime4 > 0)
					{
						int Tipset = GetCombatTipset(g.nCombatType2);
						if (Tipset == 0)
						{
							newCard.EnemyName = DataConvert.BytesToStr(g.szName7);
						}
						else 
						{
							newCard.EnemyName = U3dCmn.GetWarnErrTipFromMB(Tipset);
						}
						
						newCard.nCombatType = g.nCombatType2;
						newCard.nCombatID = g.nCombatID1;
						newCard.nRestTime = g.nRestTime4;
						newCard.nStatus = g.nStatus5;
						newCard.endTimeSec = g.nRestTime4 + DataConvert.DateTimeToInt(DateTime.Now);
						combatList.Add(newCard);
					}
				}					
			}
			
			if (processListCombatDataDelegate != null)
			{
				processListCombatDataDelegate();
			}
		}
		else {
			
			LoadingManager.instance.HideLoading();
		}
		
		processListCombatDataDelegate = null;
		//print("STC_GAMECMD_OPERATE_LIST_COMBAT:" + msgCmd.nRst1 + "," + msgCmd.nNum2);

	}
	 
	/// <summary>
	/// Gets the combat list.
	/// </summary>
	/// <returns>
	/// The combat list.
	/// </returns> 
	public List<CombatDescUnit> GetCombatList()
	{
		return combatList;
	}
	
	/// <summary>
	/// Finds the combat.
	/// </summary>
	public CombatDescUnit TryGetCombat(ulong combatID)
	{
		int imax = combatList.Count;
		for (int i=0; i<imax; ++ i)
		{
			CombatDescUnit child = combatList[i];
			if (child.nCombatID == combatID)
			{
				return child;
			}
		}
		
		return null;
	}
	
	public SortedList<ulong,CombatFighting> GetCombatFightingMap()
	{
		return combatFightingMap;
	}
	
	/// <summary>
	/// Gets the current fighting.
	/// </summary>
	
	public bool TypeFighting(ulong combatID)
	{
		CombatFighting combatVal = null;
		if (true == combatFightingMap.TryGetValue(combatID,out combatVal))
		{
			if (combatVal.nStatus == (int) CombatFighting.EnumStatus.RST_OK)
			{
				Globe.comFighting = combatVal;
				return true;
			}
		}
		
		return false;
	}
	
	public bool ApplyBaizFighting(ulong combatID)
	{
		CombatFighting combatVal = null;
		if (true == combatFightingMap.TryGetValue(combatID,out combatVal))
		{
			if (combatVal.nStatus == (int) CombatFighting.EnumStatus.RST_OK)
			{
				Globe.comFighting = combatVal;
				if (processCombatDataDelegate != null)
				{
					processCombatDataDelegate(combatID);
					processCombatDataDelegate = null;
				}
				
				// print ("comFighting:" + combatID);
				
				return true;
			}
			else if (combatVal.nStatus == (int) CombatFighting.EnumStatus.NO_DATA) 
			{
				//
				Globe.comFighting = combatVal;
				LoadingManager.instance.ShowLoading();
				RequestGetCombatData(combatID);
				return true;
			}
		}
	
		Globe.comFighting = null;
		CombatDescUnit combatCard = TryGetCombat(combatID);
		if (combatCard == null) return false;
			
		// Create fighting wow!
		CombatFighting comFighting = new CombatFighting();
		comFighting.nCombatID = combatID;

		combatFightingMap[combatID] = comFighting;
		Globe.comFighting = comFighting;
		LoadingManager.instance.ShowLoading();
		RequestGetCombatData(combatID);
		
		// 结束返回 card
		return true;
	}

	static public CombatFighting AcceptMyCombatLogFighting(byte[] data)
	{
		CombatLog log = DataConvert.ByteToStruct<CombatLog>(data);
		int num = log.n1Num7 + log.n2Num8;
		if (num >0)
		{
			int clog_len = U3dCmn.GetSizeofSimpleStructure<CombatUnit4Log>();
			int head_len = U3dCmn.GetSizeofSimpleStructure<CombatLog>();
			int data_len = clog_len * num;
			
			byte[] data_buff = new byte[data_len];
			System.Array.Copy(data,head_len,data_buff,0,data_len);
			CombatUnit4Log[] card_array = (CombatUnit4Log[])DataConvert.ByteToStructArray<CombatUnit4Log>(data_buff,num);
			
			CombatFighting comFighting = new CombatFighting();
			comFighting.nCombatType = log.nCombatType1;
			
			int objHeadID = 1;
			string objName = "**";
			if (log.nCombatType1 == (int)CombatTypeEnum.COMBAT_PVP_DEFENSE ||
				log.nCombatType1 == (int)CombatTypeEnum.COMBAT_PVP_WORLDGOLDMINE_DEFENSE)
			{
				objHeadID = log.nAttackHeadID9;
				objName = DataConvert.BytesToStr(log.szAttackName4);
			}
			else 
			{
				objHeadID = log.nDefenseHeadID10;
				objName = DataConvert.BytesToStr(log.szDefenseName6);
			}
			
			Fighting2name(log.nCombatType1, (int)log.nObjID11, objName, objHeadID, ref comFighting); 
			comFighting.combatCards = new CombatCacheUnit[num];
			
			int[] n_hero = new int[2];
			int[] n_enmey = new int[2];
			int i=0, gid=0, ox=0;

			n_hero[0] 	= 0;
			n_hero[1] 	= log.n1Num7;
			comFighting.nNumA = log.n1Num7;
		
			n_enmey[0] 	= log.n1Num7;
			n_enmey[1] 	= log.n2Num8;
			comFighting.nNumB = log.n2Num8;
			
			ox = n_hero[0];
			for (i=0; i<n_hero[1]; ++ i)
			{
				CombatUnit4Log g = card_array[i+ox];
				CombatCacheUnit newCard = new CombatCacheUnit();
				newCard.HeroName = DataConvert.BytesToStr(g.szName4);
				newCard.nArmyLevel = g.nArmyLevel6;
				newCard.nArmyNum = g.nArmyNumBefore7;
				newCard.nAttack = g.nArmyAttack11;
				newCard.nDefense = g.nArmyDefense12;
				newCard.nHealth = g.nArmyHealth13;
				newCard.nArmyType = g.nArmyType5;
				newCard.nHeroID = g.nHeroID9;
				newCard.nLevel = g.nLevel2;
				newCard.nSlot = g.nSlot1;
				comFighting.combatCards[gid ++] = newCard;
			}
			
			ox = n_enmey[0];
			for (i=0; i<n_enmey[1]; ++ i)
			{
				CombatUnit4Log g = card_array[i+ox];
				CombatCacheUnit newCard = new CombatCacheUnit();
				newCard.HeroName = DataConvert.BytesToStr(g.szName4);
				newCard.nArmyLevel = g.nArmyLevel6;
				newCard.nArmyNum = g.nArmyNumBefore7;
				newCard.nAttack = g.nArmyAttack11;
				newCard.nDefense = g.nArmyDefense12;
				newCard.nHealth = g.nArmyHealth13;
				newCard.nArmyType = g.nArmyType5;
				newCard.nHeroID = g.nHeroID9;
				newCard.nLevel = g.nLevel2;
				newCard.nSlot = g.nSlot1;
				comFighting.combatCards[gid ++] = newCard;
			}
			
			comFighting.nStatus = (int) CombatFighting.EnumStatus.RST_OK;
			return comFighting;
		}
		
		// Fini
		return null;
	}
	
	static public List<COMBAT_DEATH_INFO> AcceptMyCombatLogData(byte[] data, out int n_HeroNum, out int n_EnemyNum, out int n_HeroProf, out int n_EnemyProf)
	{
		CombatLog log = DataConvert.ByteToStruct<CombatLog>(data);
		
		// 初始化英雄和敌人数 num
		n_HeroNum = 0;
		n_EnemyNum = 0;
		n_HeroProf = 0;
		n_EnemyProf = 0;
		
		int[] n_hero = new int[2];
		int[] n_enmey = new int[2];
		int i=0, ox=0;
		
		int num = log.n1Num7 + log.n2Num8;
		if (num >0)
		{
			int clog_len = U3dCmn.GetSizeofSimpleStructure<CombatUnit4Log>();
			int head_len = U3dCmn.GetSizeofSimpleStructure<CombatLog>();
			int data_len = clog_len * num;
			byte[] data_buff = new byte[data_len];
			System.Array.Copy(data,head_len,data_buff,0,data_len);
			CombatUnit4Log[] card_array = (CombatUnit4Log[])DataConvert.ByteToStructArray<CombatUnit4Log>(data_buff,num);
			
			switch(log.nCombatType1)
			{
			case (int)CombatTypeEnum.COMBAT_PVP_DEFENSE:
			case (int)CombatTypeEnum.COMBAT_PVP_WORLDGOLDMINE_DEFENSE:
				{
					n_hero[0] 	= log.n1Num7;
					n_hero[1] 	= log.n2Num8;
					n_HeroNum	= log.n2Num8;
				
					n_enmey[0] 	= 0;
					n_enmey[1] 	= log.n1Num7;
					n_EnemyNum	= log.n1Num7;
				} 
				
				break;
				
			default:
				{
					n_hero[0] 	= 0;
					n_hero[1] 	= log.n1Num7;
					n_HeroNum	= log.n1Num7;
				
					n_enmey[0] 	= log.n1Num7;
					n_enmey[1] 	= log.n2Num8;
					n_EnemyNum	= log.n2Num8;
				}
				
				break;
			}
			
			// 防守方武将描述  {0}
			string d1 = "";
			bool b_AiEnemy = false;
			int Tipset = GetCombatTipset(log.nCombatType1);
			if (Tipset != 0)
			{
				b_AiEnemy = true;
				Tipset = GetCombatGeneral(log.nCombatType1);
				d1 = U3dCmn.GetWarnErrTipFromMB(Tipset);			
			}
			
			// 伤亡列表 {0}
			List<COMBAT_DEATH_INFO> cacheList = new List<COMBAT_DEATH_INFO>();
			
			ox = n_hero[0];
			for(i=0; i<n_hero[1]; ++ i)
			{
				CombatUnit4Log g = card_array[i+ox];
				COMBAT_DEATH_INFO info = new COMBAT_DEATH_INFO();
				info.HeroName = DataConvert.BytesToStr(g.szName4);
				info.nArmyDeadNum = g.nArmyNumBefore7 - g.nArmyNumAfter8;
				info.nArmyNum = g.nArmyNumBefore7;
				info.nArmyType = g.nArmyType5;
				info.nArmyLevel = g.nArmyLevel6;
				info.nLevel = g.nLevel2;
				
				if (log.nCombatType1 == (int)CombatTypeEnum.COMBAT_PVP_ATTACK ||
					log.nCombatType1 == (int)CombatTypeEnum.COMBAT_PVP_DEFENSE )
				{
					info.nArmyNumRecover = 0;
				}
				else
				{
					info.nArmyNumRecover = g.nArmyNumRecover10;
				}
				
				// 计算战力 ...
				float csProfA = ((float)g.nArmyAttack11+(float)g.nArmyDefense12*0.9f+g.nArmyHealth13*0.8f)/3f;
				n_HeroProf = n_HeroProf + Mathf.CeilToInt(csProfA);
				
				// 添加队列 ...
				cacheList.Add(info);
			}
			
			ox = n_enmey[0];
			for (i=0; i<n_enmey[1]; ++ i)
			{
				CombatUnit4Log g = card_array[i+ox];
				COMBAT_DEATH_INFO info = new COMBAT_DEATH_INFO();
				if (b_AiEnemy == true) {
					info.HeroName = d1;
				}
				else 
				{
					info.HeroName = DataConvert.BytesToStr(g.szName4);
				}
				
				// 战斗统计 ...
				info.nArmyDeadNum = g.nArmyNumBefore7 - g.nArmyNumAfter8;
				info.nArmyNum = g.nArmyNumBefore7;
				info.nArmyType = g.nArmyType5;
				info.nArmyLevel = g.nArmyLevel6;
				info.nLevel = g.nLevel2;
				
				if (log.nCombatType1 == (int)CombatTypeEnum.COMBAT_PVE_WORLDGOLDMINE ||
					log.nCombatType1 == (int)CombatTypeEnum.COMBAT_PVE_RAID ||
					log.nCombatType1 == (int)CombatTypeEnum.COMBAT_PVP_ATTACK ||
					log.nCombatType1 == (int)CombatTypeEnum.COMBAT_PVP_DEFENSE ||
					log.nCombatType1 == (int)CombatTypeEnum.COMBAT_WORLDCITY ||
					log.nCombatType1 == (int)CombatTypeEnum.COMBAT_INSTANCE_BAIZHANBUDAI ||
					log.nCombatType1 == (int)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN)
				{
					info.nArmyNumRecover = 0;
				}
				else 
				{
					info.nArmyNumRecover = g.nArmyNumRecover10;
				}
				
				// 计算战力 ...
				float csProfD = ((float)g.nArmyAttack11+(float)g.nArmyDefense12*0.9f+(float)g.nArmyHealth13*0.8f)/3f;
				n_EnemyProf = n_EnemyProf + Mathf.CeilToInt(csProfD);
				
				// 添加队列 ...
				cacheList.Add(info);
			}
			
			return cacheList;
		}
		
		return null;
	}
	
	public void ProcessPVPRst(byte[] data)
	{
		STC_GAMECMD_PVP_RST_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_PVP_RST_T>(data);
		if (msgCmd.nCombatRst2==1)
		{
			CommonData.player_online_info.Gold += (uint)msgCmd.nGold3;
			CommonData.player_online_info.Population += (uint)msgCmd.nMan4;
		}
		else
		{
			CommonData.player_online_info.Gold -= (uint)msgCmd.nGold3;
			CommonData.player_online_info.Population -= (uint)msgCmd.nMan4;
		}
		
		PlayerInfoManager.RefreshPlayerDataUI();
		//print("STC_GAMECMD_PVP_RST:" + msgCmd.nCombatID1 + "," + msgCmd.nGold3 + "," + msgCmd.nMan4);
	}
	
	// 如果没有军情界面自动刷 ...
	static void processAutoMarchCon1()
	{
		RequestCombatListData();
	}
	
	//每秒检查一下是否有军情  ...
	IEnumerator CheckWarSituaition()
	{
		//int sec = (int)(EndTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
		
		while(true)
		{
			int war_num = 0;
			if(combatList.Count >0)
			{	
				for(int i=0;i<combatList.Count;)
				{
					CombatDescUnit unit = combatList[i];
					if(unit.endTimeSec > DataConvert.DateTimeToInt(DateTime.Now))
					{
						i++;
						war_num++;
					}
					else 
					{
						combatList.Remove(unit);
					}	
				}
			}
			if(ReinforceList.Count >0)
			{
				for(int i=0;i<ReinforceList.Count;i++)
				{
					ReinforceUnit unit = ReinforceList[i];
					if(unit.EndTime > DataConvert.DateTimeToInt(DateTime.Now))
					{
						war_num++;
					}
					else 
					{
						ReinforceList.Remove(unit);
					}	
				}
			}
			ShowWarSituationIcon(war_num);
			yield return new WaitForSeconds(1);
		}
	}
	//显示军情图标 ....
	void ShowWarSituationIcon(int WarNum)
	{
		if(WarNum >0)
		{
			war_situation_icon.SetActiveRecursively(true);
			war_situation_icon.GetComponentInChildren<UILabel>().text = WarNum.ToString();
		}
		else
		{
			war_situation_icon.SetActiveRecursively(false);
		}
	}
	
	// 解析为战斗环境 ....
	
	//联盟增援信息队列 
	public void ProcessReinforceQueue(byte[] buff)
	{
		STC_GAMECMD_GET_DONATE_SOLDIER_QUEUE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_DONATE_SOLDIER_QUEUE_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_DONATE_SOLDIER_QUEUE_T.enum_rst.RST_OK)
		{
			
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_DONATE_SOLDIER_QUEUE_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			DONATE_SOLDIER_TE_UNIT[] unit_array = (DONATE_SOLDIER_TE_UNIT[])DataConvert.ByteToStructArray<DONATE_SOLDIER_TE_UNIT>(data_buff,(int)sub_msg.nNum2);
			ReinforceList.Clear();
			for(int i=0;i<unit_array.Length;i++)
			{
				ReinforceUnit unit  = new ReinforceUnit();
				unit.AccountID = unit_array[i].nPeerID2;
				unit.CharName = DataConvert.BytesToStr(unit_array[i].szCharName10);
				unit.DstType = unit_array[i].nDstType1;
				unit.EndTime =  (int)unit_array[i].nTime8+DataConvert.DateTimeToInt(DateTime.Now);
				unit.EventID = unit_array[i].nEventID7;
				unit.ExcelID = unit_array[i].nExcelID3;
				unit.Level = unit_array[i].nLevel4;
				unit.Num = unit_array[i].nNum5;
				unit.Type = unit_array[i].nType6;
	
				ReinforceList.Add(unit);
			}
			if (processListCombatDataDelegate != null)
			{
				processListCombatDataDelegate();
			}
		}
		processListCombatDataDelegate = null;		
	}
	//请求增援召回 
	static public void RequestReinforceBack(ulong EventID)
	{
		CTS_GAMECMD_OPERATE_ALLI_RECALL_SOLDIER_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_ALLI_RECALL_SOLDIER;
		req.nEventID3 = EventID;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_ALLI_RECALL_SOLDIER_T>(req);
	}
	//增援召回返回 
	public void ProcessReinforceBackData(byte[] data)
	{
		STC_GAMECMD_OPERATE_ALLI_RECALL_SOLDIER_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_ALLI_RECALL_SOLDIER_T>(data);
		if (msgCmd.nRst1 == (int)STC_GAMECMD_OPERATE_ALLI_RECALL_SOLDIER_T.enum_rst.RST_OK)
		{
			if (processReinforceBackDelegate != null)
			{
				processReinforceBackDelegate(msgCmd.nNewEventID3,msgCmd.nOldEventID2, (int)msgCmd.nTime4);
			}
			
		}
		else {
			
			LoadingManager.instance.HideLoading();
		}
		
		processReinforceBackDelegate = null;
		//print("STC_GAMECMD_ARMY_BACK:" + msgCmd.nRst1);
	}
	//删除增兵时间队列数据 
	public void DeleteReinforceData(ulong EventID)
	{
		ReinforceUnit delete_unit =  new ReinforceUnit();
		List<ReinforceUnit> ReinforceList = CombatManager.instance.ReinforceList;
		for (int i=0, imax = ReinforceList.Count; i<imax; i++)
		{
			
			ReinforceUnit unit = ReinforceList[i];
			if(unit.EventID == EventID)
			{
				delete_unit = unit;
				
			}
		}
		ReinforceList.Remove(delete_unit);	
	}
	//修改增兵时间数据 
	public void ModifyReinforceData(ulong EventID,ReinforceUnit newUnit)
	{
		List<ReinforceUnit> ReinforceList = CombatManager.instance.ReinforceList;
		for (int i=0, imax = ReinforceList.Count; i<imax; i++)
		{
			
			ReinforceUnit unit = ReinforceList[i];
			if(unit.EventID == EventID)
			{
				ReinforceList[i] = newUnit;
				break;
			}
		}
	}
	//修改军情数据 
	public void ModifyCombatData(CombatDescUnit combatCard)
	{
		for (int i=0, imax = combatList.Count; i<imax; i++)
		{
			
			CombatDescUnit unit = combatList[i];
			if(unit.nCombatID == combatCard.nCombatID)
			{
				combatList[i] = combatCard;
				break;
			}
		}
	}
	
}
