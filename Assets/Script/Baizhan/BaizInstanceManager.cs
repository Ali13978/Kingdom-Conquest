using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using CTSCMD;
using STCCMD;

public class BaizInstanceStatus
{
	public int 			nExcelID;	
	public int 			nResDayTimesFree; 		// 今天剩余免费次数 ...
	public int 			nDayTimesFree;			// 今天剩余收费次数  ...
	public int			nResDayTimesFee;		// 今天剩余收费次数 ...
	public int 			nDayTimesFee;			// 
	public ulong		nInstanceID;			// 实例ID，>0表示当前正在副本里 ....
	public int 			nStatus;
	public int 			nClassID;				
	public ulong		nCreatorID;
	public int 			nCurLevel;				// 当前关卡 ...
}

public class BaizInstanceData
{
	public ulong		nAccountID;
	public string 		name;
	public ulong 		n1HeroID;
	public ulong 		n2HeroID;
	public ulong 		n3HeroID;
	public ulong 		n4HeroID;
	public ulong		n5HeroID;
	public int 			nStauts;
}

public class BaizInstanceGroup
{
	public ulong		nInstanceID;
	public int 			nNum;
	public int 			nNumHero;
	public int 			nCurLevel;
	public int 			nAutoCombat;
	public int 			nRetryTimes;
	public BaizInstanceData[] m_ma;
	public PickingGeneral[] m_ga;
}

public class BaizInstanceLoot
{
	public int 			nExcelID;
}

public class BaizInstanceDesc
{
	public ulong		nInstanceID;
	public int 			nClassID;
	public string		name;
	public int 			nCurLevel;
	public int 			nLeaderLevel;
	public int 			nNumPlayer;
	public ulong		nCreatorID;
}

public class BaizInstanceManager : MonoBehaviour {
	public static int EasyOrEpic = 1;
	static BaizInstanceManager mInstance = null;

	SortedList<int, BaizInstanceStatus> mInstanceStatusMap = new SortedList<int, BaizInstanceStatus>();
	SortedList<ulong, BaizInstanceGroup> mInstanceGroupMap = new SortedList<ulong, BaizInstanceGroup>();
	
	List<BaizInstanceDesc> mInstanceDescs = new List<BaizInstanceDesc>();
	
	public delegate void ProcessCGuardLootDelegate(List<BaizInstanceLoot> lootList);
	public static ProcessCGuardLootDelegate processCGuardLootDelegate;
	
	public delegate void ProcessListInstanceStatusDelegate();
	public static ProcessListInstanceStatusDelegate processListInstanceStatusDelegate;
	
	public delegate void ProcessInstanceDataDelegate(BaizInstanceGroup newGroup);
	public static ProcessInstanceDataDelegate processInstanceDataDelegate;
	public static ProcessInstanceDataDelegate processInstanceDataCon1;
	public static ProcessInstanceDataDelegate processNanBeizhanInstanceDataCon1; // 特立为 NanBeizhanInstance消息响应 ...
	
	public delegate void ProcessCreateInstanceDelegate(ulong nInstanceID);
	public static ProcessCreateInstanceDelegate processCreateInstanceDelegate;
	
	public delegate void ProcessDestroyInstanceDelegate();
	public static ProcessDestroyInstanceDelegate processDestroyInstanceDelegate;
	
	public delegate void ProcessDestroyInstanceCon1(ulong nInstanceID);
	public static ProcessDestroyInstanceCon1 processDestroyInstanceCon1;
	
	public delegate void ProcessGetInstanceDescDelegate();
	public static ProcessGetInstanceDescDelegate processGetInstanceDescDelegate;
	
	public delegate void ProcessPrepareInstanceDelegate();
	public static ProcessPrepareInstanceDelegate processPrepareInstanceDelegate;
	
	public delegate void ProcessStartInstanceDelegate();
	public static ProcessStartInstanceDelegate processStartInstanceCon1;
	
	public delegate void ProcessQuitInstanceDelegate();
	public static ProcessQuitInstanceDelegate processQuitInstanceDelegate;
	public delegate void ProcessNanBeizhanQuitInstanceCon1(ulong nInstanceID);
	public static ProcessNanBeizhanQuitInstanceCon1 processNanBeizhanQuitInstanceCon1;
	
	public delegate void ProcessJoinInstanceDelegate();
	public static ProcessJoinInstanceDelegate processJoinInstanceDelegate;
	
	public delegate void ProcessConfigInstanceHeroDelegate();
	public static ProcessConfigInstanceHeroDelegate processConfigInstanceHeroDelegate;
	
	public delegate void ProcessKickInstanceHeroDelegate();
	public static ProcessKickInstanceHeroDelegate processKickInstanceHeroDelegate;
	
	public delegate void ProcessSupplyInstanceDelegate();
	public static ProcessSupplyInstanceDelegate processSupplyInstanceDelegate;
	
	public delegate void ProcessChatMsgAddLootItemDelegate(string chatMsg);
	public static ProcessChatMsgAddLootItemDelegate processChatMsgAddLootItemDelegate;
	
	/// <summary>
	/// The instance of the CombatManager class. Will create it if one isn't already around.
	/// </summary>

	static public BaizInstanceManager instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = Object.FindObjectOfType(typeof(BaizInstanceManager)) as BaizInstanceManager;

				if (mInstance == null)
				{
					GameObject go = new GameObject("_BaizInstanceManager");
					DontDestroyOnLoad(go);
					mInstance = go.AddComponent<BaizInstanceManager>();
				}
			}
			
			return mInstance;
		}
	}
	
	void Awake() { if (mInstance == null) { mInstance = this; DontDestroyOnLoad(gameObject); } else { Destroy(gameObject); } }
	void OnDestroy() { if (mInstance == this) mInstance = null; }
	
	void Start() {
		
		//_onLoadInstanceStatus();

	}
	
	public void _onLoadInstanceStatus()
	{
		processListInstanceStatusDelegate = OnProcessListInstanceStatusDelegate;
		RequestListInstanceStatusData();
	}
	
	void OnProcessListInstanceStatusDelegate()
	{
		BaizInstanceStatus grp = null;
		if (true == mInstanceStatusMap.TryGetValue(100, out grp))
		{
			// 王者之路中 grp. nClassID 暂时没有用到, 为0则表示为 普通战役 ....
			if (grp.nInstanceID > 0) 
			{
				int nCurLevel = Mathf.Max(1, grp.nCurLevel);
				AddChatMsgInstanceStatusDelegate(100, nCurLevel, 1, 1);
			}
		}
		
		if (true == mInstanceStatusMap.TryGetValue(101, out grp))
		{
			if (grp.nInstanceID > 0) 
			{
				int nCurLevel = Mathf.Max(1, grp.nCurLevel);
				AddChatMsgInstanceStatusDelegate(101, nCurLevel, grp.nClassID, grp.nStatus);
			}
		}
	}
	
	public void AddChatMsgInstanceStatusDelegate(int nCombatType, int nGuard, int nClass, int nStatus)
	{
		int Tip1 = 0;
		int mode = 0;
		string csName = "-";
		if (nCombatType == (int)CombatTypeEnum.COMBAT_INSTANCE_BAIZHANBUDAI)
		{
			// 后续补充值 nClass=1 普通战役 nClass=2 史诗战役 ...
			if (nClass == 1) {
				csName = U3dCmn.GetWarnErrTipFromMB(148);
			}
			else if (nClass == 2) {
				csName = U3dCmn.GetWarnErrTipFromMB(149);
			}
			
			Tip1 = BaizVariableScript.CHAT_MSG_ONLINE_BAIZHANBUDAI;
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
			
			if (nStatus == 0) {
				mode = 1;
				Tip1 = BaizVariableScript.CHAT_MSG_ONLINE_NANZHENGBEIZHAN_RST_READY;
			}
			else 
			{
				Tip1 = BaizVariableScript.CHAT_MSG_ONLINE_NANZHENGBEIZHAN;
			}
		}
		
		if (Tip1 > 0)
		{
			string cc = U3dCmn.GetWarnErrTipFromMB(Tip1);
			if (mode == 1)
			{
				string c1 = string.Format(cc, csName);
				processChatMsgAddLootItemDelegate(c1);
			}
			else 
			{
				string c2 = string.Format(cc, csName, nGuard);
				processChatMsgAddLootItemDelegate(c2);
			}
		}
	}

	public List<BaizInstanceDesc> GetInstanceDescList()
	{
		return mInstanceDescs;
	}
	
	public BaizInstanceGroup TryGetInstanceGroup(ulong nInstanceID)
	{
		BaizInstanceGroup newGroup = null;
		mInstanceGroupMap.TryGetValue(nInstanceID, out newGroup);
		return newGroup;
	}
	
	public BaizInstanceStatus TryGetInstanceStatus(int nExcelID)
	{
		BaizInstanceStatus newCard = null;
		mInstanceStatusMap.TryGetValue(nExcelID,out newCard);
		return newCard;
	}
	
	public static void RequestDestroyInstance(int nExcelID, ulong nInstanceID)
	{
		CTS_GAMECMD_DESTROY_INSTANCE_T req;
		req.nCmd1 = (byte) CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint) SUB_CTS_MSG.CTS_GAMECMD_DESTROY_INSTANCE;
		req.nExcelID3 = nExcelID;
		req.nInstanceID4 = nInstanceID;
		TcpMsger.SendLogicData<CTS_GAMECMD_DESTROY_INSTANCE_T>(req);
	}
	
	public static void RequestInstanceGuardLoot(int nExcelID, ulong nInstanceID)
	{
		CTS_GAMECMD_GET_INSTANCELOOT_T req;
		req.nCmd1 = (byte) CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint) SUB_CTS_MSG.CTS_GAMECMD_GET_INSTANCELOOT;
		req.nInstanceID4 = nInstanceID;
		req.nExcelID3 = nExcelID;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_INSTANCELOOT_T>(req);
	}
	
	public void ProcessBaizhanGuardLootData(byte[] data)
	{
		STC_GAMECMD_GET_INSTANCELOOT_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_GET_INSTANCELOOT_T>(data);
		if (msgCmd.nRst1 == (int) STC_GAMECMD_GET_INSTANCELOOT_T.enum_rst.RST_OK)
		{
			if (msgCmd.nNum2>0)
			{
				int num = msgCmd.nNum2;
				int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_INSTANCELOOT_T>();
				int data_len = data.Length - head_len;
				byte[] data_buff = new byte[data_len];
				System.Array.Copy(data,head_len,data_buff,0,data_len);
				LootDesc[] card_array = (LootDesc[])DataConvert.ByteToStructArray<LootDesc>(data_buff,num);
				
				List<BaizInstanceLoot> newLootList = new List<BaizInstanceLoot>();
				for (int i=0; i<num; ++ i)
				{
					LootDesc g = card_array[i];
					BaizInstanceLoot newCard = new BaizInstanceLoot();
					newCard.nExcelID = g.nExcelID1;
					newLootList.Add(newCard);
				}
				
				if (processCGuardLootDelegate != null) {
					processCGuardLootDelegate(newLootList);
				}	
			}
			else 
			{
				LoadingManager.instance.HideLoading();
				int Tipset = BaizVariableScript.INSTANCE_LOOT_ISNULL;
				PopTipDialog.instance.VoidSetText1(true,false,Tipset);
			}
		}
		else
		{
			LoadingManager.instance.HideLoading();
		}
		
		processCGuardLootDelegate = null;
		//print("STC_GAMECMD_GET_INSTANCELOOT:" + msgCmd.nRst1 + "," + msgCmd.nNum2);
	}
	
	/// <summary>
	/// Processes the destroy instance data.
	/// </summary>
	/// <param name='data'>
	/// Data.
	/// </param>
	public void ProcessDestroyInstanceData(byte[] data)
	{
		STC_GAMECMD_DESTROY_INSTANCE_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_DESTROY_INSTANCE_T>(data);
		if (msgCmd.nRst1 == (int) STC_GAMECMD_DESTROY_INSTANCE_T.enum_rst.RST_OK)
		{
			if (true == mInstanceGroupMap.ContainsKey(msgCmd.nInstanceID2))
			{
				mInstanceGroupMap.Remove(msgCmd.nInstanceID2);
			}
			
			if (processDestroyInstanceDelegate != null)
			{
				processDestroyInstanceDelegate();
			}
			else if (processDestroyInstanceCon1 != null)
			{
				// 特定为"征战天下" 准备 ...
				processDestroyInstanceCon1(msgCmd.nInstanceID2);
			}
		}
		else 
		{
			LoadingManager.instance.HideLoading();
			
			int Tipset = 0;
			switch (msgCmd.nRst1)
			{
			case (int)STC_GAMECMD_DESTROY_INSTANCE_T.enum_rst.RST_COMBAT:
				{
					Tipset = BaizVariableScript.INSTANCE_DESTROY_RST_COMBAT;
				} break;
			case (int)STC_GAMECMD_DESTROY_INSTANCE_T.enum_rst.RST_ISNULL:
				{
					Tipset = BaizVariableScript.INSTANCE_DESTROY_RST_IS_NULL;
				} break;
			case (int)STC_GAMECMD_DESTROY_INSTANCE_T.enum_rst.RST_UPDATE:
				{
					Tipset = BaizVariableScript.INSTANCE_DESTROY_RST_UPDATE;
				} break;
			}
			
			PopTipDialog.instance.VoidSetText1(true, false, Tipset);
		}
		
		// -- 除去副本图片 ...
		//print("STC_GAMECMD_DESTROY_INSTANCE:" + msgCmd.nRst1);
	}
	
	/// <summary>
	/// Requests the list instance status data.
	/// </summary>
	public static void RequestListInstanceStatusData()
	{
		CTS_GAMECMD_GET_INSTANCESTATUS_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_INSTANCESTATUS;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_INSTANCESTATUS_T>(req);
	}
	
	/// <summary>
	/// Processes the list instance status data.
	/// </summary>
	/// <param name='data'>
	/// Data.
	/// </param>
	public void ProcessListInstanceStatusData(byte[] data)
	{
		STC_GAMECMD_GET_INSTANCESTATUS_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_GET_INSTANCESTATUS_T>(data);
		if (msgCmd.nRst1 == (int) STC_GAMECMD_GET_INSTANCESTATUS_T.enum_rst.RST_OK)
		{
			mInstanceStatusMap.Clear();
			if (msgCmd.nNum2>0)
			{
				int num = msgCmd.nNum2;
				int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_INSTANCESTATUS_T>();
				int data_len = data.Length - head_len;
				byte[] data_buff = new byte[data_len];
				System.Array.Copy(data,head_len,data_buff,0,data_len);
				InstanceStatus[] card_array = (InstanceStatus[])DataConvert.ByteToStructArray<InstanceStatus>(data_buff,num);
				
				for (int i=0; i<num; ++ i)
				{
					InstanceStatus g = card_array[i];
					BaizInstanceStatus newCard 	= new BaizInstanceStatus();
					newCard.nExcelID 			= g.nExcelID1;
					newCard.nDayTimesFee 		= g.nDayTimesFee5;
					newCard.nDayTimesFree 		= g.nDayTimesFree3;
					newCard.nInstanceID 		= g.nInstanceID6;
					newCard.nResDayTimesFee 	= g.nResDayTimesFee4;
					newCard.nResDayTimesFree 	= g.nResDayTimesFree2;
					newCard.nStatus				= g.nStatus7;
					newCard.nClassID			= g.nClassID8;
					newCard.nCreatorID			= g.nCreatorID9;
					newCard.nCurLevel			= g.nCurLevel10;
					mInstanceStatusMap.Add(newCard.nExcelID, newCard);
					if(newCard.nExcelID == 100)
					{
						BaizInstanceManager.EasyOrEpic = newCard.nClassID;
					}
				}
				
				if (processListInstanceStatusDelegate != null)
				{
					processListInstanceStatusDelegate();
				}
			}
			else 
			{
				// 取消 Loading ...
				LoadingManager.instance.HideLoading();
			}
			BaizInstanceStatus card = BaizInstanceManager.instance.TryGetInstanceStatus(101);
			if (card == null) return;
			
			NanBeizhanInstance.instance.idInstance = card.nInstanceID;
			
			int d1 = card.nDayTimesFee - card.nResDayTimesFee;
			int d2 = card.nDayTimesFree - card.nResDayTimesFree;
			NanBeizhanInstance.instance.idRestDayTimesFee = d1;
			NanBeizhanInstance.instance.idRestDayTimesFree = d2;
			U3dCmn.SendMessage("NanBeizhanLaunchWin","RefreshUI",null);
				
		}
		else {
			
			LoadingManager.instance.HideLoading();
		}
		
		processListInstanceStatusDelegate = null;
		//print("STC_GAMECMD_GET_INSTANCESTATUS:" + msgCmd.nRst1 + "," + msgCmd.nNum2);
	}
	
	public static void RequestCreateInstance(int nExcelID, int nClassID)
	{
		CTS_GAMECMD_CREATE_INSTANCE_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint) SUB_CTS_MSG.CTS_GAMECMD_CREATE_INSTANCE;
		req.nExcelID3 = nExcelID;
		req.nClass4	= nClassID;
		TcpMsger.SendLogicData<CTS_GAMECMD_CREATE_INSTANCE_T>(req);
	}
	
	public void ProcessCreateInstanceData(byte[] data)
	{
		STC_GAMECMD_CREATE_INSTANCE_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_CREATE_INSTANCE_T>(data);
		if (msgCmd.nRst1 == (int) STC_GAMECMD_CREATE_INSTANCE_T.enum_rst.RST_OK)
		{
			if (processCreateInstanceDelegate != null)
			{
				processCreateInstanceDelegate(msgCmd.nInstanceID2);
			}
		}
		else 
		{
			LoadingManager.instance.HideLoading();
			int Tipset = 0;
			int method = 0;
			if (msgCmd.nRst1 == (int)STC_GAMECMD_CREATE_INSTANCE_T.enum_rst.RST_LEVEL)
			{
				Tipset = BaizVariableScript.CREATE_INSTANCE_LEVEL;
			}
			else if (msgCmd.nRst1 == (int)STC_GAMECMD_CREATE_INSTANCE_T.enum_rst.RST_TIMES)
			{
				Tipset = BaizVariableScript.CREATE_INSTANCE_TIMES;
			}
			else if (msgCmd.nRst1 == (int)STC_GAMECMD_CREATE_INSTANCE_T.enum_rst.RST_OPENTIME)
			{
				Tipset = BaizVariableScript.CREATE_INSTANCE_OPENTIME;
			}
			else if (msgCmd.nRst1 == (int)STC_GAMECMD_CREATE_INSTANCE_T.enum_rst.RST_COST)
			{
				Tipset = BaizVariableScript.CREATE_INSTANCE_COST;
				method = 1;
			}
			else if (msgCmd.nRst1 == (int)STC_GAMECMD_CREATE_INSTANCE_T.enum_rst.RST_INNOW)
			{
				Tipset = BaizVariableScript.CREATE_INSTANCE_INNOW;
			}
			
			if (method == 0)
			{
				PopTipDialog.instance.VoidSetText1(true,false,Tipset);
			}
			else if (method == 1)
			{
				string cc = U3dCmn.GetWarnErrTipFromMB(Tipset);
				ITEM_INFO desc = U3dCmn.GetItemIconByExcelID(2016);
				string cs = string.Format(cc, desc.Name);
				PopTipDialog.instance.VoidSetText2(true,false,cs);
			}
		}
		
		processCreateInstanceDelegate = null;
		//print ("STC_GAMECMD_CREATE_INSTANCE:" + msgCmd.nRst1 + "," + msgCmd.nInstanceID2);
	}
	
	public static void RequestGetInstanceData(int nExcelID, ulong nInstanceID)
	{
		CTS_GAMECMD_GET_INSTANCEDATA_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint) SUB_CTS_MSG.CTS_GAMECMD_GET_INSTANCEDATA;
		req.nExcelID3 = nExcelID;
		req.nInstanceID4 = nInstanceID;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_INSTANCEDATA_T>(req);
	}
	
	BaizInstanceData GetAccountByHeroID(BaizInstanceData[] ma, ulong nHeroID, out int acid)
	{
		acid = 0;
		int imax = ma.Length;
		for(int i=0; i<imax; ++ i)
		{
			if (ma[i].n1HeroID == nHeroID) 
			{
				acid = i;
				return ma[i];
			}
		}
		
		// Fini
		return null;
	}
	
	public void ProcessGetBaizInstanceData(byte[] data)
	{
		STC_GAMECMD_GET_INSTANCEDATA_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_GET_INSTANCEDATA_T>(data);
		if (msgCmd.nRst1 == (int) STC_GAMECMD_GET_INSTANCEDATA_T.enum_rst.RST_OK)
		{
			BaizInstanceGroup newGroup = null;
			if (false == mInstanceGroupMap.TryGetValue(msgCmd.nInstanceID6, out newGroup))
			{
				newGroup = new BaizInstanceGroup();
				newGroup.m_ma = new BaizInstanceData[5];
				newGroup.m_ga = new PickingGeneral[5];
				newGroup.nInstanceID = msgCmd.nInstanceID6;
			}
			
			if (newGroup != null)
			{
				newGroup.nCurLevel = msgCmd.nCurLevel2;
				newGroup.nAutoCombat = msgCmd.nAutoCombat3;
				newGroup.nRetryTimes = msgCmd.nRetryTimes5;
			}
			
			// 武将数据解析 ...
			int ofsBytes = 0;
			
			System.Array.Clear(newGroup.m_ma, 0, 5);
			newGroup.nNum = msgCmd.nNum7;
			//print ("ssssssssssssssssssssss"+msgCmd.nNum7 +" "+msgCmd.nNumHero8);
			if (msgCmd.nNum7>0) 
			{
				int num = msgCmd.nNum7;
				int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_INSTANCEDATA_T>();
				
				int _len = U3dCmn.GetSizeofSimpleStructure<InstanceData>();
				int data_len = _len * num;

				ofsBytes = head_len;				
				byte[] data_buff = new byte[data_len];
				System.Array.Copy(data,ofsBytes,data_buff,0,data_len);
				InstanceData[] card_array = (InstanceData[])DataConvert.ByteToStructArray<InstanceData>(data_buff,num);
				
				for (int i=0; i<num; ++ i)
				{
					
					InstanceData g = card_array[i];
					BaizInstanceData newCard = new BaizInstanceData();
					newCard.name = DataConvert.BytesToStr(g.szName3);
					newCard.nAccountID  = g.nAccountID1;
					newCard.n1HeroID = g.n1Hero4;
					newCard.n2HeroID = g.n2Hero5;
					newCard.n3HeroID = g.n3Hero6;
					newCard.n4HeroID = g.n4Hero7;
					newCard.n5HeroID = g.n5Hero8;
					newCard.nStauts	 = g.nStatus9;
					newGroup.m_ma[i] = newCard;
				}
				
				ofsBytes = ofsBytes + data_len;
			}
			
			System.Array.Clear(newGroup.m_ga, 0, 5);
			newGroup.nNumHero = msgCmd.nNumHero8;
			
			if (msgCmd.nNumHero8>0)
			{
				int num = msgCmd.nNumHero8;
				int _len = U3dCmn.GetSizeofSimpleStructure<CombatUnit>();
				int data_len = _len * num;

				byte[] data_buff = new byte[data_len];
				System.Array.Copy(data,ofsBytes,data_buff,0,data_len);
				CombatUnit[] card_array = (CombatUnit[])DataConvert.ByteToStructArray<CombatUnit>(data_buff,num);
				
				for (int i=0; i<num; ++ i)
				{
					CombatUnit g = card_array[i];
					if (newGroup.nNum>0)
					{
						int acID = 0;
						BaizInstanceData newOx = GetAccountByHeroID(newGroup.m_ma, g.nHeroID2,out acID);
						if (newOx != null)
						{
							PickingGeneral newCard = new PickingGeneral();
							newCard.name = DataConvert.BytesToStr(g.szName4);
							float fProf = (float)g.nArmyAttack8 + (float)g.nArmyDefense9*0.9f + (float)g.nHealth10*0.8f;							
							newCard.nProf = Mathf.CeilToInt(fProf / 3f); // 战力取值 ...
							newCard.nArmyType = g.nArmyType5;
							newCard.nArmyLevel = g.nArmyLevel6;
							newCard.nArmyNum = g.nArmyNum7;
							newCard.nHealth = g.nHealth10;
							newCard.nHeroID = g.nHeroID2;
							newCard.nLevel = g.nLevel12;
							newCard.nModel = g.nModel11;
							newCard.nHealthStatus = g.nHealthStatus13;
							newCard.campIn = true;
							newCard.charName = newOx.name;
							newCard.nAccountID = newOx.nAccountID;
							newGroup.m_ga[acID] = newCard;
						}
					}
				}
			}
			
			mInstanceGroupMap[msgCmd.nInstanceID6] = newGroup;
			
			// Initial instance Group
			if (processInstanceDataDelegate != null)
			{
				processInstanceDataDelegate(newGroup);
			}
			else if (processInstanceDataCon1 != null)
			{
				processInstanceDataCon1(newGroup);
			}
			else if (processNanBeizhanInstanceDataCon1 != null)
			{
				// 当在主场景中时, 不能接收 副本"征战天下"的InstanceData 特设立回调...
				processNanBeizhanInstanceDataCon1(newGroup);
			}
		}
		else 
		{
			LoadingManager.instance.HideLoading();
		}
		
		processInstanceDataDelegate = null;
		//print("STC_GAMECMD_GET_INSTANCEDATA:" + msgCmd.nRst1 + "," + msgCmd.nAutoCombat3 + "," + msgCmd.nNum7 + "," + msgCmd.nNumHero8);
	}
	
	public void ProcessInstanceLootAddItemData(byte[] data)
	{
		STC_GAMECMD_GET_INSTANCELOOT_ADDITEM_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_GET_INSTANCELOOT_ADDITEM_T>(data);
		if (msgCmd.nNum4>0)
		{
			int num = msgCmd.nNum4;
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_INSTANCELOOT_ADDITEM_T>();
			int data_len = data.Length - head_len;
			byte[] data_buff = new byte[data_len];
			System.Array.Copy(data,head_len,data_buff,0,data_len);
			InstanceLootDesc[] card_array = (InstanceLootDesc[])DataConvert.ByteToStructArray<InstanceLootDesc>(data_buff,num);
			
			for (int i=0; i<num; ++ i)
			{
				InstanceLootDesc card = card_array[i];
				CangKuManager.AddItem(card.nItemID1,card.nExcelID2,card.nNum3);
			}
			
			if (processChatMsgAddLootItemDelegate != null)
			{
				for (int i=0; i<num; ++ i)
				{
					InstanceLootDesc card = card_array[i];
					AddChatMsgAddLootItemDelegate((int)msgCmd.nCombatType2, msgCmd.nObjID3, msgCmd.nClassID5, card.nExcelID2, card.nNum3);
				}
			}
		}
		
		//print("STC_GAMECMD_GET_INSTANCELOOT_ADDITEM:" + msgCmd.nInstanceID1 + "," + msgCmd.nNum4);
	}
	
	public void AddChatMsgAddLootItemDelegate(int nCombatType, int nGuard, int nClass, int nExcelID, int num)
	{
		int Tip1 = 0;
		string csName = "-";
		if (nCombatType == (int)CombatTypeEnum.COMBAT_INSTANCE_BAIZHANBUDAI)
		{
			if (nClass == 1) {
				csName = U3dCmn.GetWarnErrTipFromMB(148);
			}
			else if (nClass == 2) {
				csName = U3dCmn.GetWarnErrTipFromMB(149);
			}
			Tip1 = BaizVariableScript.CHAT_MSG_ADDLOOT_BAIZHANBUDAI;
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
			Tip1 = BaizVariableScript.CHAT_MSG_ADDLOOT_NANZHENGBEIZHAN;
		}
		
		if (Tip1 > 0)
		{
			ITEM_INFO info = U3dCmn.GetItemIconByExcelID((uint)nExcelID);
			string cc = U3dCmn.GetWarnErrTipFromMB(Tip1);
			string c1 = string.Format(cc, csName, nGuard, info.Name, num);
			processChatMsgAddLootItemDelegate(c1);
		}
	}
	
	static public void RequestInstanceDescData(int nExcelID, int nClassID)
	{
		CTS_GAMECMD_GET_INSTANCEDESC_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint) SUB_CTS_MSG.CTS_GAMECMD_GET_INSTANCEDESC;
		req.nExcelID3 = nExcelID;
		req.nClass4 = nClassID;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_INSTANCEDESC_T>(req);
	}
	
	public void ProcessGetInstanceDescData(byte[] data)
	{
		STC_GAMECMD_GET_INSTANCEDESC_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_GET_INSTANCEDESC_T>(data);
		if (msgCmd.nRst1 == (int) STC_GAMECMD_GET_INSTANCEDESC_T.enum_rst.RST_OK)
		{
			mInstanceDescs.Clear();
			if (msgCmd.nNum3>0)
			{
				int num = msgCmd.nNum3;
				int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_INSTANCEDESC_T>();
				int data_len = data.Length - head_len;
				byte[] data_buff = new byte[data_len];
				System.Array.Copy(data,head_len,data_buff,0,data_len);
				InstanceDesc[] card_array = (InstanceDesc[])DataConvert.ByteToStructArray<InstanceDesc>(data_buff,num);
				
				for (int i=0; i<num; ++ i)
				{
					InstanceDesc g = card_array[i];
					BaizInstanceDesc newCard = new BaizInstanceDesc();
					newCard.name = DataConvert.BytesToStr(g.szName3);
					newCard.nCurLevel = g.nCurLevel4;
					newCard.nInstanceID = g.nInstanceID1;
					newCard.nClassID = msgCmd.nClassID2;
					newCard.nLeaderLevel = g.nLeaderLevel5;
					newCard.nNumPlayer = g.nNumPlayer6;
					newCard.nCreatorID = g.nCreatorID7;	
					mInstanceDescs.Add(newCard);
				}
			}
			
			if (processGetInstanceDescDelegate != null)
			{
				processGetInstanceDescDelegate();
			}
		}
		else
		{
			// 取消 Loading ...
			LoadingManager.instance.HideLoading();
		}
		
		processGetInstanceDescDelegate = null;
		//print ("STC_GAMECMD_GET_INSTANCEDESC:" + msgCmd.nRst1 + "," + msgCmd.nNum3);
	}
	
	static public void RequestPrepareInstance(int nExcelID, ulong nInstanceID, int nVal)
	{
		CTS_GAMECMD_PREPARE_INSTANCE_T req;
		req.nCmd1 = (byte) CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint) SUB_CTS_MSG.CTS_GAMECMD_PREPARE_INSTANCE;
		req.nExcelID3 = nExcelID;
		req.nInstanceID4 = nInstanceID;
		req.nVal5 = nVal;
	
		TcpMsger.SendLogicData<CTS_GAMECMD_PREPARE_INSTANCE_T>(req);	
	}
	
	public void ProcessPrepareInstanceData(byte[] data)
	{
		STC_GAMECMD_PREPARE_INSTANCE_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_PREPARE_INSTANCE_T>(data);
		if (msgCmd.nRst1 == (int)STC_GAMECMD_PREPARE_INSTANCE_T.enum_rst.RST_OK)
		{
			if (processPrepareInstanceDelegate != null)
			{
				processPrepareInstanceDelegate();
			}
		}
		else
		{
			LoadingManager.instance.HideLoading();
			int Tipset = 0;
			if (msgCmd.nRst1 == (int) STC_GAMECMD_PREPARE_INSTANCE_T.enum_rst.RST_HERO)
			{
				Tipset = BaizVariableScript.INSTANCE_PREPARE_NOT_HERO;
				PopTipDialog.instance.VoidSetText1(true,false,Tipset);
			}
		}
		
		
		processPrepareInstanceDelegate = null;
		//print ("STC_GAMECMD_PREPARE_INSTANCE:" + msgCmd.nRst1);
	}
	
	static public void RequestStartInstance(int nExcelID, ulong nInstanceID)
	{
		CTS_GAMECMD_START_INSTANCE_T req;
		req.nCmd1 = (byte) CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint) SUB_CTS_MSG.CTS_GAMECMD_START_INSTANCE;
		req.nExcelID3 = nExcelID;
		req.nInstanceID4 = nInstanceID;
		TcpMsger.SendLogicData<CTS_GAMECMD_START_INSTANCE_T>(req);
	}
	
	public void ProcessStartInstanceData(byte[] data)
	{
		STC_GAMECMD_START_INSTANCE_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_START_INSTANCE_T>(data);
		if (msgCmd.nRst1 == (int)STC_GAMECMD_START_INSTANCE_T.enum_rst.RST_OK)
		{
			if (processStartInstanceCon1 != null)
			{
				processStartInstanceCon1();
			}
		}
		else {
			
			LoadingManager.instance.HideLoading();
			int Tipset = 0;
			if (msgCmd.nRst1 == (int)STC_GAMECMD_START_INSTANCE_T.enum_rst.RST_PREPARE)
			{
				Tipset = BaizVariableScript.INSTANCE_START_NOT_PREPARE;
			}
			else if (msgCmd.nRst1 == (int)STC_GAMECMD_START_INSTANCE_T.enum_rst.RST_LEADER)
			{
				Tipset = BaizVariableScript.INSTANCE_START_NOT_LEADER;
			}

			PopTipDialog.instance.VoidSetText1(true,false,Tipset);
		}
		
		//print ("STC_GAMECMD_START_INSTANCE:" + msgCmd.nRst1);
	}
	
	static public void RequestQuitInstance(int nExcelID, ulong nInstanceID)
	{
		CTS_GAMECMD_QUIT_INSTANCE_T req;
		req.nCmd1 = (byte) CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint) SUB_CTS_MSG.CTS_GAMECMD_QUIT_INSTANCE;
		req.nExcelID3 = nExcelID;
		req.nInstanceID4 = nInstanceID;
		TcpMsger.SendLogicData<CTS_GAMECMD_QUIT_INSTANCE_T>(req);
	}
	
	public void ProcessQuitInstanceData(byte[] data)
	{
		STC_GAMECMD_QUIT_INSTANCE_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_QUIT_INSTANCE_T>(data);
		if (msgCmd.nRst1 == (int)STC_GAMECMD_QUIT_INSTANCE_T.enum_rst.RST_OK)
		{
			if (processQuitInstanceDelegate != null)
			{
				processQuitInstanceDelegate();
			}
			else if (processNanBeizhanQuitInstanceCon1 != null)
			{
				// 在主场景中, 收不到QuitInstance 消息 ... 特做 ...
				processNanBeizhanQuitInstanceCon1(msgCmd.nInstanceID2);
			}
		}
		else {
			
			LoadingManager.instance.HideLoading();
						
			int Tipset = 0;
			switch (msgCmd.nRst1)
			{
			case (int)STC_GAMECMD_QUIT_INSTANCE_T.enum_rst.RST_COMBAT:
				{
					Tipset = BaizVariableScript.INSTANCE_DESTROY_RST_COMBAT;
				} break;
			case (int)STC_GAMECMD_QUIT_INSTANCE_T.enum_rst.RST_ISNULL:
				{
					Tipset = BaizVariableScript.INSTANCE_DESTROY_RST_IS_NULL;
				} break;
			case (int)STC_GAMECMD_QUIT_INSTANCE_T.enum_rst.RST_UPDATE:
				{
					Tipset = BaizVariableScript.INSTANCE_DESTROY_RST_UPDATE;
				} break;
			case (int)STC_GAMECMD_QUIT_INSTANCE_T.enum_rst.RST_UNLOCK:
				{
					Tipset = BaizVariableScript.INSTANCE_QUIT_RST_UNLOCK;
				} break;
			}
			
			PopTipDialog.instance.VoidSetText1(true, false, Tipset);
			
		}
		
		//print ("STC_GAMECMD_QUIT_INSTANCE:" + msgCmd.nRst1);
	}
	
	static public void RequestJoinInstance(int nExcelID, ulong nInstanceID)
	{
		CTS_GAMECMD_JOIN_INSTANCE_T req;
		req.nCmd1 = (byte) CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint) SUB_CTS_MSG.CTS_GAMECMD_JOIN_INSTANCE;
		req.nExcelID3 = nExcelID;
		req.nInstanceID4 = nInstanceID;
		TcpMsger.SendLogicData<CTS_GAMECMD_JOIN_INSTANCE_T>(req);
	}
	
	public void ProcessJoinInstanceData(byte[] data)
	{
		STC_GAMECMD_JOIN_INSTANCE_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_JOIN_INSTANCE_T>(data);
		if (msgCmd.nRst1 == (int) STC_GAMECMD_JOIN_INSTANCE_T.enum_rst.RST_OK)
		{
			if (processJoinInstanceDelegate != null)
			{
				processJoinInstanceDelegate();
			}
		}
		else
		{
			LoadingManager.instance.HideLoading();
			int Tipset = 0;
			int method = 0;
			if (msgCmd.nRst1 == (int)STC_GAMECMD_JOIN_INSTANCE_T.enum_rst.RST_INNOW)
			{
				Tipset = BaizVariableScript.CREATE_INSTANCE_INNOW;
			}
			else if (msgCmd.nRst1 == (int)STC_GAMECMD_JOIN_INSTANCE_T.enum_rst.RST_LEVEL)
			{
				Tipset = BaizVariableScript.CREATE_INSTANCE_LEVEL;
			}
			else if (msgCmd.nRst1 == (int)STC_GAMECMD_JOIN_INSTANCE_T.enum_rst.RST_OPENTIME)
			{
				Tipset = BaizVariableScript.CREATE_INSTANCE_OPENTIME;
			}
			else if (msgCmd.nRst1 == (int)STC_GAMECMD_JOIN_INSTANCE_T.enum_rst.RST_TIMES)
			{
				Tipset = BaizVariableScript.CREATE_INSTANCE_TIMES;
			}
			else if (msgCmd.nRst1 == (int)STC_GAMECMD_JOIN_INSTANCE_T.enum_rst.RST_ISNULL)
			{
				Tipset = BaizVariableScript.CREATE_INSTANCE_ISNULL;
			}
			else if (msgCmd.nRst1 == (int)STC_GAMECMD_JOIN_INSTANCE_T.enum_rst.RST_PLAYER)
			{
				Tipset = BaizVariableScript.CREATE_INSTANCE_RST_PLAYER;
			}
			else if (msgCmd.nRst1 == (int)STC_GAMECMD_JOIN_INSTANCE_T.enum_rst.RST_COST)
			{
				Tipset = BaizVariableScript.CREATE_INSTANCE_COST;
				method = 1;
			}
			
			if (method == 0) 
			{
				PopTipDialog.instance.VoidSetText1(true,false,Tipset);
			}
			else if (method == 1)
			{
				string cc = U3dCmn.GetWarnErrTipFromMB(Tipset);
				ITEM_INFO desc = U3dCmn.GetItemIconByExcelID(2016);
				string cs = string.Format(cc, desc.Name);
				PopTipDialog.instance.VoidSetText2(true,false,cs);
			}
		}
		
		processJoinInstanceDelegate = null;
		//print ("STC_GAMECMD_JOIN_INSTANCE:" + msgCmd.nRst1);
	}
	
	static public void RequestConfigInstanceHero(int nExcelID, ulong nInstanceID, ulong nHeroID)
	{
		CTS_GAMECMD_CONFIG_INSTANCEHERO_T req;
		req.nCmd1 = (byte) CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint) SUB_CTS_MSG.CTS_GAMECMD_CONFIG_INSTANCEHERO;
		req.nExcelID3 = nExcelID;
		req.nInstanceID4 = nInstanceID;
		req.n1Hero5 = nHeroID;
		req.n2Hero6 = 0;
		req.n3Hero7 = 0;
		req.n4Hero8 = 0;
		req.n5Hero9 = 0;
		TcpMsger.SendLogicData<CTS_GAMECMD_CONFIG_INSTANCEHERO_T>(req);
	}
	
	public void ProcessConfigInstanceHeroData(byte[] data)
	{
		STC_GAMECMD_CONFIG_INSTANCEHERO_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_CONFIG_INSTANCEHERO_T>(data);
		if (msgCmd.nRst1 == (int) STC_GAMECMD_CONFIG_INSTANCEHERO_T.enum_rst.RST_OK)
		{
			if (processConfigInstanceHeroDelegate != null)
			{
				processConfigInstanceHeroDelegate();
			}
		}
		else 
		{
			LoadingManager.instance.HideLoading();
			int Tipset = 0;
			if (msgCmd.nRst1 == (int) STC_GAMECMD_CONFIG_INSTANCEHERO_T.enum_rst.RST_HERO_ERR)
			{
				Tipset = BaizVariableScript.INSTANCE_PICKING_GENERAL_NO_FREE;
			}
			
			PopTipDialog.instance.VoidSetText1(true, false, Tipset);
		}
		
		processConfigInstanceHeroDelegate = null;
		//print ("STC_GAMECMD_CONFIG_INSTANCEHERO:" + msgCmd.nRst1);
	}
	
	static public void RequestKickInstanceHero(int nExcelID, ulong nInstanceID, ulong nHeroID)
	{
		CTS_GAMECMD_KICK_INSTANCE_T req;
		req.nCmd1 = (byte) CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint) SUB_CTS_MSG.CTS_GAMECMD_KICK_INSTANCE;
		req.nExcelID3 = nExcelID;
		req.nInstnaceID4 = nInstanceID;
		req.nObjID5 = nHeroID;
		TcpMsger.SendLogicData<CTS_GAMECMD_KICK_INSTANCE_T>(req);
	}
	
	public void ProcessKickInstanceHeroData(byte[] data)
	{
		STC_GAMECMD_KICK_INSTANCE_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_KICK_INSTANCE_T>(data);
		if (msgCmd.nRst1 == (int)STC_GAMECMD_KICK_INSTANCE_T.enum_rst.RST_OK)
		{
			if (processKickInstanceHeroDelegate != null)
			{
				processKickInstanceHeroDelegate();
			}
		}
		else 
		{
			LoadingManager.instance.HideLoading();
		}
		
		processKickInstanceHeroDelegate = null;
		//print ("STC_GAMECMD_KICK_INSTANCE:" + msgCmd.nRst1);
	}
	
	static public void RequestSupplyInstance(int nExcelID, ulong nInstanceID)
	{
		CTS_GAMECMD_SUPPLY_INSTANCE_T req;
		req.nCmd1 = (byte) CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint) SUB_CTS_MSG.CTS_GAMECMD_SUPPLY_INSTANCE;
		req.nExcelID3 = nExcelID;
		req.nInstanceID4 = nInstanceID;
		TcpMsger.SendLogicData<CTS_GAMECMD_SUPPLY_INSTANCE_T>(req);
	}
	
	public void ProcessSupplyInstanceData(byte[] data)
	{
		STC_GAMECMD_SUPPLY_INSTANCE_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_SUPPLY_INSTANCE_T>(data);
		if (msgCmd.nRst1 == (int)STC_GAMECMD_SUPPLY_INSTANCE_T.enum_rst.RST_OK)
		{
			if (processSupplyInstanceDelegate != null)
			{
				processSupplyInstanceDelegate();
			}
		}
		else 
		{	
			LoadingManager.instance.HideLoading();
		}
		
		processSupplyInstanceDelegate = null;
		//print ("STC_GAMECMD_SUPPLY_INSTANCE:" + msgCmd.nRst1 + "," + msgCmd.nInstanceID3);
	}
	
}
