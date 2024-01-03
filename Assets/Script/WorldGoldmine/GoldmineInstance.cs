using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using CTSCMD;
using STCCMD;


public class GoldmineCardData
{
	public int nAreaID;				// 金矿区域 id
	public int nClass;				// 金矿类型 class
	public int nIdx;				// 金矿序号 nidx 
	public ulong nAccountID;		// 占领者 id
	public string name;				// 占领者名称 name
	public int nSafeTime;			// 保护时间   time
	
	public int n1Addition;
	public int n1Secs;
	public int n1SafeTime;			// 最大值 SafeTime
}

public class MyGoldmineData
{
	public int nAreaID;				// 金矿区域 id
	public int nClass;				// 金矿类型 class
	public int nIdx;				// 金矿序号 nidx 
	public ulong nAccountID;		// 占领者 id
	public string name;				// 占领者名称 name
	public int nSafeTime;			// 保护时间   time
	public ulong n1HeroID;
	public ulong n2HeroID;
	public ulong n3HeroID;
	public ulong n4HeroID;
	public ulong n5HeroID;
	
	public int n1Addition;
	public int n1Secs;
	public int n1SafeTime;			// 最大值 SafeTime
}

public class GoldmineInstance : MonoBehaviour {
	
	static GoldmineInstance mInst = null;
	
	public MyGoldmineData MyGoldmine = new MyGoldmineData();
	
	List<GoldmineCardData> mGoldmines = new List<GoldmineCardData>();
	
	int m_curarea_id = 0;
	
	ulong m_combat_id = 0;
	
	public delegate void ProcessGetWorldGoldmineDelegate();
	public static ProcessGetWorldGoldmineDelegate processGetWorldGoldmineDelegate;
	public static ProcessGetWorldGoldmineDelegate processGetMyGoldmineCon1;
	
	public delegate void ProcessRobGoldmineDelegate(ulong nCombatID);
	public static ProcessRobGoldmineDelegate processRobGoldmineDelegate;
	
	public delegate void ProcessDropGoldmineDelegate();
	public static ProcessDropGoldmineDelegate processDropGoldmineDelegate;
	
	public delegate void ProcessConfigGoldmineHeroDelegate();
	public static ProcessConfigGoldmineHeroDelegate processConfigGoldmineHeroDelegate;
		
	static public GoldmineInstance instance
	{
		get
		{
			if (mInst == null)
			{
				mInst = UnityEngine.Object.FindObjectOfType(typeof(GoldmineInstance)) as GoldmineInstance;

				if (mInst == null)
				{
					GameObject go = new GameObject("_GoldmineInstance");
					DontDestroyOnLoad(go);
					mInst = go.AddComponent<GoldmineInstance>();
				}
			}
			
			return mInst;
		}
	}
	
	public int gid
	{
		get {
			return m_curarea_id;
		}
	}
	
	public ulong idCombat
	{
		get {
			return m_combat_id;
		}
	}
	
	void Awake() { if (mInst == null) { mInst = this; DontDestroyOnLoad(gameObject); } else { Destroy(gameObject); } }
	void OnDestroy() { if (mInst == this) mInst = null; }
	
	// Use this for initialization
	void Start () {
	
	}
	
	public void MyGoldmineDataCopy(GoldmineCardData data)
	{
		MyGoldmine.nAccountID = data.nAccountID;
		MyGoldmine.name = data.name;
		MyGoldmine.nAreaID  = data.nAreaID;
		MyGoldmine.nClass = data.nClass;
		MyGoldmine.nIdx = data.nIdx;
		MyGoldmine.nSafeTime = data.nSafeTime;
		MyGoldmine.n1SafeTime = data.n1SafeTime;
		MyGoldmine.n1Secs = data.n1Secs;
		MyGoldmine.n1Addition = data.n1Addition;
	}
	
	static public void RequestDropMyGoldmine()
	{
		CTS_GAMECMD_DROP_WORLDGOLDMINE_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint) SUB_CTS_MSG.CTS_GAMECMD_DROP_WORLDGOLDMINE;
		TcpMsger.SendLogicData<CTS_GAMECMD_DROP_WORLDGOLDMINE_T>(req);
	}
	
	public void ProcessDropMyGoldmineData(byte[] data)
	{
		STC_GAMECMD_DROP_WORLDGOLDMINE_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_DROP_WORLDGOLDMINE_T>(data);
		if (msgCmd.nRst1 == (int) STC_GAMECMD_DROP_WORLDGOLDMINE_T.enum_rst.RST_OK)
		{
			MyGoldmineData d1 = MyGoldmine;
			int firstid = 0;
			if (d1.nClass>1) {
				firstid = firstid + 1;
			}
			if (d1.nClass>2) {
				firstid = firstid + 2;
			}
			if (d1.nClass>3) {
				firstid = firstid + 3;
			}
					
			firstid = firstid + d1.nIdx - 1;
			GoldmineCardData new1 = GetItem(firstid);
			if (new1 != null)
			{
				new1.nAccountID = 0;
				new1.n1SafeTime = 0;
				new1.name = "";
			}
			
			MyGoldmine.nAreaID = 0;
			MyGoldmine.nClass  = 0;
			MyGoldmine.nIdx    = 0;
			MyGoldmine.nAccountID = 0;
			
			if (processDropGoldmineDelegate != null)
			{
				processDropGoldmineDelegate();
			}
		}
		else 
		{
			LoadingManager.instance.HideLoading();
		}
		
		processDropGoldmineDelegate = null;
		//print ("STC_GAMECMD_DROP_WORLDGOLDMINE:" + msgCmd.nRst1);
		
	}
	
	static public void RequestGetMyGoldmine()
	{
		CTS_GAMECMD_MY_WORLDGOLDMINE_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint) SUB_CTS_MSG.CTS_GAMECMD_MY_WORLDGOLDMINE;
		TcpMsger.SendLogicData<CTS_GAMECMD_MY_WORLDGOLDMINE_T>(req);
	}
	
	public void ProcessGetMyGoldmineData(byte[] data)
	{
		STC_GAMECMD_MY_WORLDGOLDMINE_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_MY_WORLDGOLDMINE_T>(data);
		if (msgCmd.nRst1 == (int) STC_GAMECMD_MY_WORLDGOLDMINE_T.enum_rst.RST_OK)
		{
			MyGoldmine.nAccountID = CommonData.player_online_info.AccountID;
			MyGoldmine.name = CommonData.player_online_info.CharName;
			MyGoldmine.nAreaID = msgCmd.nArea2;
			MyGoldmine.nClass = msgCmd.nClass3;
			MyGoldmine.nIdx = msgCmd.nIdx4;
			MyGoldmine.nSafeTime = msgCmd.nSafeTime5;
			MyGoldmine.n1HeroID = msgCmd.n1Hero6;
			MyGoldmine.n2HeroID = msgCmd.n2Hero7;
			MyGoldmine.n3HeroID = msgCmd.n3Hero8;
			MyGoldmine.n4HeroID = msgCmd.n4Hero9;
			MyGoldmine.n5HeroID = msgCmd.n5Hero10;
			
			if (processGetMyGoldmineCon1 != null)
			{
				processGetMyGoldmineCon1();
			}
		}
		else 
		{
			LoadingManager.instance.HideLoading();
			MyGoldmine.nAreaID = 0;
			MyGoldmine.nClass = 0;
			MyGoldmine.nIdx = 0;
		}
		
		processGetMyGoldmineCon1 = null;
		
		//print ("STC_GAMECMD_MY_WORLDGOLDMINE:" + msgCmd.nRst1 + "," + msgCmd.nArea2);
	}
	
	static public void RequestGetWorldGoldmine(int nArea, int nClass, int nIdx)
	{
		CTS_GAMECMD_GET_WORLDGOLDMINE_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_WORLDGOLDMINE;
		req.nArea3 = nArea;
		req.nClass4 = nClass;
		req.nIdx5 = nIdx;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_WORLDGOLDMINE_T>(req);
	}
	
	void ABGoldmineData(ref GoldmineCardData newCard, WorldGoldmineDesc g, GoldmineMBInfo info)
	{
		newCard.nClass 		= g.nClass1;
		newCard.nAccountID 	= g.nAccountID3;
		newCard.nIdx 		= g.nIdx2;
		newCard.nSafeTime	= g.nSafeTime6;
		newCard.name 		= DataConvert.BytesToStr(g.szName5);
		
		int Tipset = 0;
		if (newCard.nClass == 1)
		{
			newCard.n1Addition = info.n1Addition;
			newCard.n1Secs = info.n1Secs;
			newCard.n1SafeTime = info.n1SafeTime;
			Tipset = BaizVariableScript.WORLDGOLDMINE_NPC_1;
		}
		else if (newCard.nClass == 2)
		{
			newCard.n1Addition = info.n2Addition;
			newCard.n1Secs = info.n2Secs;
			newCard.n1SafeTime = info.n2SafeTime;
			Tipset = BaizVariableScript.WORLDGOLDMINE_NPC_2;
		}
		else if (newCard.nClass == 3)
		{
			newCard.n1Addition = info.n3Addition;
			newCard.n1Secs = info.n3Secs;
			newCard.n1SafeTime = info.n3SafeTime;
			Tipset = BaizVariableScript.WORLDGOLDMINE_NPC_3;
		}
		else if (newCard.nClass == 4)
		{
			newCard.n1Addition = info.n4Addition;
			newCard.n1Secs = info.n4Secs;
			newCard.n1SafeTime = info.n4SafeTime;
			Tipset = BaizVariableScript.WORLDGOLDMINE_NPC_4;
		}
		
		if (newCard.nAccountID == 0)
		{
			newCard.name = U3dCmn.GetWarnErrTipFromMB(Tipset);
		}
	}
	
	public void ProcessGetWorldGoldmineData(byte[] data)
	{
		STC_GAMECMD_GET_WORLDGOLDMINE_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_GET_WORLDGOLDMINE_T>(data);
		if (msgCmd.nRst1 == (int) STC_GAMECMD_GET_WORLDGOLDMINE_T.enum_rst.RST_OK)
		{
			if (msgCmd.nNum2>0)
			{
				int num = msgCmd.nNum2;
				int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_WORLDGOLDMINE_T>();
				int data_len = data.Length - head_len;
				byte[] data_buff = new byte[data_len];
				System.Array.Copy(data,head_len,data_buff,0,data_len);
				WorldGoldmineDesc[] card_array = (WorldGoldmineDesc[])DataConvert.ByteToStructArray<WorldGoldmineDesc>(data_buff,num);
			
				uint id = (uint) msgCmd.nAreaID3;
				Hashtable goldMB = CommonMB.WorldGoldmine_Map;
				GoldmineMBInfo info = new GoldmineMBInfo();
				if (true == goldMB.ContainsKey(id))
				{
					info = (GoldmineMBInfo) goldMB[id];
				}
				
				if (num == 1)
				{
					WorldGoldmineDesc d1 = card_array[0];
					int firstid = 0;
					if (d1.nClass1>1) {
						firstid = firstid + 1;
					}
					if (d1.nClass1>2) {
						firstid = firstid + 2;
					}
					if (d1.nClass1>3) {
						firstid = firstid + 3;
					}
					
					firstid = firstid + d1.nIdx2 - 1;
					GoldmineCardData new1 = GetItem(firstid);
					if (new1 != null)
					{
						ABGoldmineData(ref new1, d1, info);
						if (new1.nAccountID == CommonData.player_online_info.AccountID)
						{
							MyGoldmineDataCopy(new1);
						}
					}
				}
				else if (num > 1)
				{
					m_curarea_id = msgCmd.nAreaID3;
					mGoldmines.Clear();
					for (int i=0; i<num; ++ i)
					{
						WorldGoldmineDesc g = card_array[i];
						GoldmineCardData newCard = new GoldmineCardData();
						newCard.nAreaID 	= msgCmd.nAreaID3;
						ABGoldmineData(ref newCard, g, info);
						
						if (g.nAccountID3 == CommonData.player_online_info.AccountID)
						{
							MyGoldmineDataCopy(newCard);
						}
						
						mGoldmines.Add(newCard);
					}
				}
				
				if (processGetWorldGoldmineDelegate != null)
				{
					processGetWorldGoldmineDelegate();
				}
			}
		}
		else 
		{
			LoadingManager.instance.HideLoading();
		}
		
		processGetWorldGoldmineDelegate = null;
		//print("STC_GAMECMD_GET_WORLDGOLDMINE:" + msgCmd.nRst1 + "," + msgCmd.nNum2);
	}
	
	public void ProcessRobWorldGoldmineData(byte[] data)
	{
		STC_GAMECMD_ROB_WORLDGOLDMINE_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_ROB_WORLDGOLDMINE_T>(data);
		if (msgCmd.nRst1 == (int)STC_GAMECMD_ROB_WORLDGOLDMINE_T.enum_rst.RST_OK)
		{
			this.m_combat_id = msgCmd.nCombatID2;
			
			SortedList<ulong, CombatFighting> combatMap = CombatManager.instance.GetCombatFightingMap();
			
			CombatFighting comFighting = null;
			if (false == combatMap.TryGetValue(msgCmd.nCombatID2,out comFighting))
			{
				comFighting = new CombatFighting();
				comFighting.nCombatID = msgCmd.nCombatID2;
				comFighting.nBackRestTime = 5;
				comFighting.endTimeSec = 0;
				combatMap[msgCmd.nCombatID2] = comFighting;
			}
			
			if (processRobGoldmineDelegate != null)
			{
				processRobGoldmineDelegate(msgCmd.nCombatID2);
			}
		}
		else 
		{	
			LoadingManager.instance.HideLoading();
			int Tipset = 0;
			
			switch (msgCmd.nRst1)
			{
			case (int)STC_GAMECMD_ROB_WORLDGOLDMINE_T.enum_rst.RST_ISNULL:
				{
					Tipset = BaizVariableScript.WORLDGOLDMINE_ROB_ISNULL;
				} break;
			case (int)STC_GAMECMD_ROB_WORLDGOLDMINE_T.enum_rst.RST_SAFETIME:
				{
					Tipset = BaizVariableScript.WORLDGOLDMINE_ROB_SAFETIME;
				} break;
			case (int)STC_GAMECMD_ROB_WORLDGOLDMINE_T.enum_rst.RST_COMBAT:
				{
					Tipset = BaizVariableScript.WORLDGOLDMINE_ROB_NOT_COMBAT;
				} break;
			default: break;
			}
			
			PopTipDialog.instance.VoidSetText1(true, false, Tipset);
		}
		
		processRobGoldmineDelegate = null;
		//print("STC_GAMECMD_ROB_WORLDGOLDMINE:" + msgCmd.nRst1);
	}
	
	public void ProcessGainWorldGoldmineData(byte[] data)
	{
		STC_GAMECMD_GAIN_WORLDGOLDMINE_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_GAIN_WORLDGOLDMINE_T>(data);
		if (msgCmd.nRst1 == (int)STC_GAMECMD_GAIN_WORLDGOLDMINE_T.enum_rst.RST_OK)
		{
			CommonData.player_online_info.Gold += (uint) msgCmd.nGold5;
			PlayerInfoManager.RefreshPlayerDataUI();
		}
		
		//print("STC_GAMECMD_GAIN_WORLDGOLDMINE:" + msgCmd.nRst1);
	}
	
	public void ProcessConfigGoldmineHeroData(byte[] data)
	{
		STC_GAMECMD_CONFIG_WORLDGOLDMINE_HERO_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_CONFIG_WORLDGOLDMINE_HERO_T>(data);
		if (msgCmd.nRst1 == (int)STC_GAMECMD_CONFIG_WORLDGOLDMINE_HERO_T.enum_rst.RST_OK)
		{
			if (processConfigGoldmineHeroDelegate != null)
			{
				processConfigGoldmineHeroDelegate();
			}
		}
		else 
		{
			LoadingManager.instance.HideLoading();
		}
		
		processConfigGoldmineHeroDelegate = null;
		//print ("STC_GAMECMD_CONFIG_WORLDGOLDMINE_HERO:" + msgCmd.nRst1);
	}
	
	public List<GoldmineCardData> GetGoldmineList()
	{
		return mGoldmines;
	}
	
	public GoldmineCardData GetItem(int itemID)
	{
		if (itemID<0 || itemID> (mGoldmines.Count-1)) return null;
		return mGoldmines[itemID];
	}
	
	void DisbandForUnpack1()
	{
		GoldmineScene go = GameObject.FindObjectOfType(typeof(GoldmineScene)) as GoldmineScene;
		if (go == null) return;
		
		// 重新获取世界金矿信息
		go.GetGoldmineArea(m_curarea_id);
		go.GetMyGoldmine();
		go.AssignComm();
		
		GoldmineGuardWin go1 = GameObject.FindObjectOfType(typeof(GoldmineGuardWin)) as GoldmineGuardWin;
		if (go1 != null) { Destroy(go1.gameObject); }
		
		GoldmineChuZhen go2 = GameObject.FindObjectOfType(typeof(GoldmineChuZhen)) as GoldmineChuZhen;
		if (go2 != null) { Destroy(go2.gameObject); }
		
		BaizGeneralRoadWin win1 = GameObject.FindObjectOfType(typeof(BaizGeneralRoadWin)) as BaizGeneralRoadWin;
		if (win1 != null) { Destroy(win1.gameObject); }
	}
}
