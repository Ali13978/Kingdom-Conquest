using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using CTSCMD;
using STCCMD;

public class QuestFeeBuilding
{
	public int ExcelID;
	public int Level;
	public int num;
}

public class QuestFeeProps
{
	public int ExcelID;
	public int num;
}


// 任务Quest表信息 ID
public class QuestFeeMBInfo
{
	//类型唯一ID
	public uint		ExcelID;
	// 任务标题 ...
	public string 	name;
	// 任务描述 ...
	public string 	Desc;
	// 任务目标 ...
	public string 	Target;
	// 任务奖励 ... 
	public string 	Award;
	
	// 是否可以重复 ...
	public int 		canRepeat;
	
	// 活动开启时段 ... 
	public int 		yearFrom, yearTo;
	public int 		monthFrom, monthTo;
	public int 		weekend1, weekend2;
	public int 		dayFrom, dayTo;
	public int		hourFrom, hourTo;
	
	public int 		TimesPerDay;
	
	// 前提条件... 
	public int 		preLevel;
	public int 		preIsVip;
	public int 		preGloryNum;
	public int 		prePopulation;
	public int 		preSoliderNum;
	public int 		preHeroNum;
	
	// 前提建筑条件...
	public List<QuestFeeBuilding> preBuilding = new List<QuestFeeBuilding>();
	
	// 消耗条件 ...
	public List<QuestFeeProps> preConsProps = new List<QuestFeeProps>();
	
	// 奖励 ...
	public int 		AwardGold;
	public int 		AwardDiamond;
	public int 		AwardCrystal;
	public int 		AwardExp;
	
	// 奖励道具
	public List<QuestFeeProps> AwardProps = new List<QuestFeeProps>();
	
	// 是否已完成 ...
	public int 		BooDone;
	
	// 等级加成系数 ...
	public float 	powExp;
	
	// 分类 ...
	public int 		preAssort;
	
	// Vip right ...
	public int 		vipRight;
}

public class QuestFeeManager : MonoBehaviour {
	
	static QuestFeeManager mInst = null;
	
	// 任务分类总表 ...
	public static Hashtable QuestFeeDesc_Map = new Hashtable();
	
	// 每日任务分类表 ...
	List<QuestDesc> mDayFreeList = new List<QuestDesc>();
	// 成长任务分类表 ...
	List<QuestDesc> mGrowFeeList = new List<QuestDesc>();
	// 出征任务分类表 ...
	List<QuestDesc> mExpeditionList = new List<QuestDesc>();
	// VIP分类表 ...
	List<QuestDesc> mVipFeeList = new List<QuestDesc>();
	
	public delegate void ProcessListQuestDelegate();
	static public ProcessListQuestDelegate processListQuestDelegate;
	
	public delegate void ProcessDoneQuestDelegate();
	static public ProcessDoneQuestDelegate processDoneQuestDelegate;
	
	/// <summary>
	/// The instance of the CombatManager class. Will create it if one isn't already around.
	/// </summary>

	static public QuestFeeManager instance
	{
		get
		{
			if (mInst == null)
			{
				mInst = UnityEngine.Object.FindObjectOfType(typeof(QuestFeeManager)) as QuestFeeManager;

				if (mInst == null)
				{
					GameObject go = new GameObject("_QuestFeeManager");
					DontDestroyOnLoad(go);
					mInst = go.AddComponent<QuestFeeManager>();
				}
			}
			
			return mInst;
		}
	}
	
	void Awake() { if (mInst == null) { mInst = this; DontDestroyOnLoad(gameObject); } else { Destroy(gameObject); } }
	void OnDestroy() { if (mInst == this) mInst = null; }
	
	// Use this for initialization
	void Start () {
	
	}
	
	// 根据序列ID 获取每日活动任务状态 ...
	public int GetDayFreeExcelID(int itemID, out bool BooDone)
	{
		BooDone = false;
		if (itemID<0 || itemID> (mDayFreeList.Count-1)) return 0;
		QuestDesc c = mDayFreeList[itemID];
		BooDone = (c.nDone2 == 1);
		return c.ExcelID1;
	}
	
	public List<QuestDesc> GetDayFreeList()
	{
		return mDayFreeList;
	}
	
	// 根据序列ID 获取成长任务状态 ...
	public int GetGrowFeeExcelID(int itemID, out bool BooDone)
	{
		BooDone = false;
		if (itemID<0 || itemID> (mGrowFeeList.Count-1)) return 0;
		QuestDesc c = mGrowFeeList[itemID];
		BooDone = (c.nDone2 == 1);
		return c.ExcelID1;
	}
	
	public List<QuestDesc> GetGrowFeeList()
	{
		return mGrowFeeList;
	}
	
	// 概据序列ID 获取出征任务状态 ...
	public int GetExpeditionExcelID(int itemID, out bool BooDone)
	{
		BooDone = false;
		if (itemID<0 || itemID> (mExpeditionList.Count-1)) return 0;
		QuestDesc c = mExpeditionList[itemID];
		BooDone = (c.nDone2 == 1);
		return c.ExcelID1;
	}
	
	public List<QuestDesc> GetExpeditionList()
	{
		return mExpeditionList;
	}
	
	// 根据序列ID, 获取VIP任务状态 ...
	public int GetVipFeeExcelID(int itemID, out bool BooDone)
	{
		BooDone = false;
		if (itemID<0 || itemID> (mVipFeeList.Count-1)) return 0;
		QuestDesc c = mVipFeeList[itemID];
		BooDone = (c.nDone2 == 1);
		return c.ExcelID1;
	}
	
	public List<QuestDesc> GetVipFeeList()
	{
		return mVipFeeList;
	}
	
	static public string StringYearMonthDay(QuestFeeMBInfo info)
	{
		if (info.yearFrom<1 || info.monthFrom<1 || info.dayFrom<1)
		{
			string d1 = U3dCmn.GetWarnErrTipFromMB(306);
			return d1;
		}
		
		return string.Format("{0:D2}-{1:D2} ~ {2:D2}-{3:D2}",
			info.monthFrom, info.dayFrom, info.monthTo, info.dayTo );
	}
	
	// 解析任务奖励描述  ....
	static public string StringReward(QuestFeeMBInfo info)
	{
		string sReward = "";
		string d1;
		
		CMN_PLAYER_CARD_INFO pc = CommonData.player_online_info;
		
		int imax = info.AwardProps.Count;
		int sp = 0;
		if (info.AwardExp>0)
		{
			d1 = U3dCmn.GetWarnErrTipFromMB(301);
			sp ++;
			sReward += string.Format(d1,info.AwardExp);
		}
		if (info.AwardGold>0)
		{
			d1 = U3dCmn.GetWarnErrTipFromMB(302);
			if (sp> 0) { sReward += " "; } sp ++;
			float gold = Mathf.Pow((1f+info.powExp), (float)(pc.Level-1)) * ((float)info.AwardGold);
			sReward += string.Format(d1,Mathf.FloorToInt(gold));
		}
		if (info.AwardDiamond>0)
		{
			d1 = U3dCmn.GetWarnErrTipFromMB(304);
			if (sp> 0) { sReward += " "; } sp ++;
			sReward += string.Format(d1,info.AwardDiamond);
		}
		if (info.AwardCrystal>0)
		{
			d1 = U3dCmn.GetWarnErrTipFromMB(305);
			if (sp> 0) { sReward += " "; } sp ++;
			sReward += string.Format(d1,info.AwardCrystal);
		}
		
		if (imax>0)
		{
			if (sp> 0) { sReward += " "; } sp ++;
			d1 = U3dCmn.GetWarnErrTipFromMB(303);
			for(int i=0; i<imax; ++ i)
			{
				QuestFeeProps item = info.AwardProps[i];
				uint id = (uint) item.ExcelID;
				
				ITEM_INFO card = U3dCmn.GetItemIconByExcelID(id);
				sReward += " ";
				sReward += string.Format(d1,cast1(card.Name), item.num);
			}
		}
		
		// Fini
		return sReward;
	}
	
	static public QuestFeeMBInfo GetItem(uint idQuest)
	{
		if (false == QuestFeeDesc_Map.ContainsKey(idQuest)) return null;
		// Fini
		return (QuestFeeMBInfo) QuestFeeDesc_Map[idQuest];
	}
	
	static public bool AwardDetermine(QuestFeeMBInfo data)
	{
		CMN_PLAYER_CARD_INFO card = CommonData.player_online_info;
		
		// 常规前提条件 ...
		if (data.preIsVip>0) 		{ if ((int) card.Vip<data.preIsVip) return false; }
		if (data.preLevel>0)		{ if ((int) card.Level<data.preLevel) return false; }
		if (data.preHeroNum>0)		{ int num = JiangLingManager.MyHeroMap.Count; if (num<data.preHeroNum) return false; }
		if (data.preGloryNum>0)		{ if ((int) card.Development<data.preGloryNum) return false; }
		if (data.prePopulation>0) 	{ if ((int) card.Population<data.prePopulation) return false; }
		if (data.preSoliderNum>0)	{ int num = SoldierManager.GetSoldierNum(); if (num<data.preSoliderNum) return false; }
		
		// 建筑前提条件 ...
		int imax = data.preBuilding.Count;
		if (imax>0)
		{
			for (int i=0; i<imax; ++ i)
			{
				QuestFeeBuilding con = data.preBuilding[i];
				int num = BuildingManager.GetBuildingNum(con.ExcelID);
				if ((int)num<con.num) return false;
				int level = (int)U3dCmn.GetBuildingLevelByID(con.ExcelID);
				if (level>0 && level<con.Level) return false;
			}
		}
		
		// 特殊条件 ...
		if (data.ExcelID == 10) {
			int num = FriendManager.FriendDataList.Count;
			if (num<1) return false;
		}

		// 消费条件 ... 
		
		
		
		// Fini
		return true;
	}
	
	// 解析物品消耗中是否有装备嵌有宝石 ....
	static public bool EmbedGemDetermine(uint ExcelID)
	{
		Hashtable qMap = QuestFeeDesc_Map;
		if (true == qMap.ContainsKey(ExcelID))
		{
			QuestFeeMBInfo info = (QuestFeeMBInfo) qMap[ExcelID];
			
			int imax = info.preConsProps.Count;
			for (int i=0;i<imax; ++ i)
			{
				QuestFeeProps cc = (QuestFeeProps)info.preConsProps[i];
				uint eqExcelID = (uint)cc.ExcelID;
				if (true == CommonMB.ExcelToSort_Map.ContainsKey(eqExcelID))
				{
					uint eqType = (uint) CommonMB.ExcelToSort_Map[eqExcelID];
					if (eqType == (uint)CMNCMD.ItemSort.EQUIP)
					{
						return true;	
					}
				}
			}
		}
		
		// 返回 ...
		return false;
	}
	
	static string cast1(string s)
	{
		string g = s; if(g == null) g = "0"; return g;
	}
	// 建筑前提条件解析 ...
	static void SplitBuilding(ref QuestFeeMBInfo data, string textVal)
	{
		string[] sSp = new string[1]{ "*" };
		string[] coSp = textVal.Split(sSp, System.StringSplitOptions.None);
		
		int imax = coSp.Length/3;
		data.preBuilding.Clear();
		
		for (int i=0; i<imax; ++ i)
		{
			QuestFeeBuilding con = new QuestFeeBuilding();
			con.ExcelID = int.Parse(cast1(coSp[i*3+0]));
			con.Level = int.Parse(cast1(coSp[i*3+1]));
			con.num = int.Parse(cast1(coSp[i*3+2]));
			data.preBuilding.Add(con);
		}
	}
	
	static void SplitTwo(ref int[] data, string textVal)
	{
		char[] sSp = new char[1]{ '*' };
		string[] coSp = textVal.Split(sSp,2);
		
		data[0] = data[1] = 0;
		
		int imax = Mathf.Min(2,coSp.Length);
		for (int i=0; i<imax; ++ i)
		{
			data[i] = int.Parse(cast1(coSp[i]));
		}
	}
	// 解析消耗物品 ...
	static void SplitPreCons(ref QuestFeeMBInfo data, string textVal)
	{
		string[] sSp = new string[1]{"*"};
		string[] coSp = textVal.Split(sSp, System.StringSplitOptions.None);
		
		int imax = coSp.Length/2;
		data.preConsProps.Clear();
		
		for (int i=0; i<imax; ++ i)
		{
			QuestFeeProps con = new QuestFeeProps();
			con.ExcelID = int.Parse(cast1(coSp[i*2+0]));
			con.num = int.Parse(cast1(coSp[i*2+1]));
			data.preConsProps.Add(con);
		}
	}
	// 解析奖励物品 ...
	static void SplitProps(ref QuestFeeMBInfo data, string textVal)
	{
		string[] sSp = new string[1]{"*"};
		string[] coSp = textVal.Split(sSp, System.StringSplitOptions.None);
		
		int imax = coSp.Length/2;
		data.AwardProps.Clear();
		
		for (int i=0; i<imax; ++ i)
		{
			QuestFeeProps con = new QuestFeeProps();
			con.ExcelID = int.Parse(cast1(coSp[i*2+0]));
			con.num = int.Parse(cast1(coSp[i*2+1]));
			data.AwardProps.Add(con);
		}
	}
	// 从MB表格中获取当前的任务列表 ...
	static public void ParseByQuestList(string[][] data)
	{
		QuestFeeDesc_Map.Clear();
		
		int[] FeeTwo = new int[2] { 0, 0 };
		for (int i=0, imax = data[0].Length; i<imax; ++ i)
		{
			QuestFeeMBInfo newCard = new QuestFeeMBInfo();
			
			newCard.ExcelID = uint.Parse(cast1(data[0][i]));
			newCard.name = data[1][i];
			newCard.Desc = data[2][i];
			newCard.Target = data[3][i];
			newCard.Award = data[4][i];
			newCard.canRepeat = int.Parse(cast1(data[5][i]));

			SplitTwo(ref FeeTwo, cast1(data[6][i]));
			newCard.yearFrom = FeeTwo[0]; newCard.yearTo = FeeTwo[1];
			SplitTwo(ref FeeTwo, cast1(data[7][i]));
			newCard.monthFrom = FeeTwo[0]; newCard.monthTo = FeeTwo[1];
			SplitTwo(ref FeeTwo, cast1(data[8][i]));
			newCard.dayFrom = FeeTwo[0]; newCard.dayTo = FeeTwo[1];
			SplitTwo(ref FeeTwo, cast1(data[9][i]));
			newCard.weekend1 = FeeTwo[0]; newCard.weekend2 = FeeTwo[1];
			SplitTwo(ref FeeTwo, cast1(data[10][i]));
			newCard.hourFrom = FeeTwo[0]; newCard.hourTo = FeeTwo[1];
			
			newCard.TimesPerDay = int.Parse(cast1(data[11][i]));
			
			newCard.preIsVip = int.Parse(cast1(data[13][i]));
			newCard.preLevel = int.Parse(cast1(data[12][i]));
			newCard.preGloryNum = int.Parse(cast1(data[14][i]));
			newCard.prePopulation = int.Parse(cast1(data[15][i]));
			newCard.preSoliderNum = int.Parse(cast1(data[16][i]));
			newCard.preHeroNum = int.Parse(cast1(data[17][i]));
			
			SplitBuilding(ref newCard, cast1(data[18][i]));
			
			newCard.AwardGold = int.Parse(cast1(data[21][i]));
			newCard.AwardCrystal = int.Parse(cast1(data[22][i]));
			newCard.AwardDiamond = int.Parse(cast1(data[23][i]));
			newCard.AwardExp = int.Parse(cast1(data[24][i]));
			
			// 道具奖励 ... 
			SplitProps(ref newCard, cast1(data[25][i]));
			
			// 道具消耗 ...
			SplitPreCons(ref newCard, cast1(data[29][i]));
			
			// 奖励等级加成 ...
			newCard.powExp = float.Parse(cast1(data[30][i]));
			
			// 任务分类 ...
			newCard.preAssort = int.Parse(cast1(data[37][i]));
			
			// Vip 分类
			newCard.vipRight = int.Parse(cast1(data[40][i]));
			
			// 添加任务 ...
			QuestFeeDesc_Map.Add(newCard.ExcelID, newCard);
		}
	}
	
	static public void RequestDoneFeeQuest(int ExcelID)
	{
		CTS_GAMECMD_DONE_QUEST_T req;
		req.nCmd1 = (byte) CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint) SUB_CTS_MSG.CTS_GAMECMD_DONE_QUEST;
		req.nExcelID3 = ExcelID;
		TcpMsger.SendLogicData<CTS_GAMECMD_DONE_QUEST_T>(req);
	}
	
	public void ProcessDoneFeeQuestData(byte[] data)
	{
		STC_GAMECMD_DONE_QUEST_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_DONE_QUEST_T>(data);
		if (msgCmd.nRst1 == (int)STC_GAMECMD_DONE_QUEST_T.enum_rst.RST_OK)
		{
			int num = msgCmd.nNum2;
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_DONE_QUEST_T>();
			int data_len = data.Length - head_len;
			byte[] data_buff = new byte[data_len];
			System.Array.Copy(data,head_len,data_buff,0,data_len);
			AddItemDesc[] card_array = (AddItemDesc[])DataConvert.ByteToStructArray<AddItemDesc>(data_buff,num);
			
			// 添加奖励物品 ...
			for (int i=0; i<num; ++ i) 
			{
				AddItemDesc card = card_array[i];
				CangKuManager.AddItem(card.nItemID1, card.nExcelID2, card.nNum3);
			}
			
			if (processDoneQuestDelegate != null) {
				processDoneQuestDelegate();
			}
		}
		else 
		{
			LoadingManager.instance.HideLoading();
		}
		
		processDoneQuestDelegate = null;
		//print("STC_GAMECMD_DONE_QUEST_T:" + msgCmd.nRst1);
	}
	
	// 获取当前已完成的任务列表 ...
	static public void RequesGetFeeQuest()
	{
		CTS_GAMECMD_GET_QUEST_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_QUEST;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_QUEST_T>(req);
	}
	
	public void ProcessGetFeeQuestData(byte[] data)
	{
		STC_GAMECMD_GET_QUEST_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_GET_QUEST_T>(data);
		if (msgCmd.nRst1 == (int)STC_GAMECMD_GET_QUEST_T.enum_rst.RST_OK)
		{
			mGrowFeeList.Clear();
			mDayFreeList.Clear();
			mExpeditionList.Clear();
			mVipFeeList.Clear();
			
			byte[] growCullMask = new byte[256]; // 暂定为256类 ...
			byte[] vipCullMask = new byte[256]; // VIP任务分类上限256 ...
			System.Array.Clear(vipCullMask,0,256); // 
			
			int nDayFeeNum = 0;
			int nVipLevel = (int) CommonData.player_online_info.Vip;
			for (int c = 0; c<nVipLevel; ++ c) {
				vipCullMask[c] = 1;
			}
			
			if (msgCmd.nNum2>0)
			{
				int num = msgCmd.nNum2;
				int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_QUEST_T>();
				int data_len = data.Length - head_len;
				byte[] data_buff = new byte[data_len];
				System.Array.Copy(data,head_len,data_buff,0,data_len);
				QuestDesc[] card_array = (QuestDesc[])DataConvert.ByteToStructArray<QuestDesc>(data_buff,num);
				
				for (int i=0; i<num; ++ i)
				{
					QuestDesc g = card_array[i];
					QuestFeeMBInfo info = GetItem((uint) g.ExcelID1);
					if (info != null)
					{
						if (g.ExcelID1>0)
						{
							if (info.vipRight>0) {
								mVipFeeList.Add(g);
							}
							else if (info.canRepeat == 1) {
								mDayFreeList.Add(g);
								if (g.nDone2 == 1) nDayFeeNum ++ ;
							}
							else if (info.canRepeat == 0)
							{ // 成长任务 ...
								if (g.nDone2 == 1)
								{
									if  (info.preAssort<1)
									{
										mGrowFeeList.Add(g);
									}
									else if (growCullMask[info.preAssort] == 0)
									{
			
										mGrowFeeList.Add(g);
										growCullMask[info.preAssort] = 1;
									}
								}
							}
						}
												
						info.BooDone = g.nDone2;
					}
				}
			}
			
			// 更新领取任务奖励图标 ...
			TaskManager.dayFee_unread_num = nDayFeeNum;
			TaskManager.growFee_unread_num = mGrowFeeList.Count;
			TaskManager.vipFee_unread_num = mVipFeeList.Count;
			U3dCmn.SendMessage("TaskManager", "SetUnReadIcon", null);
			
			// 不显示当前未显示 ...
			vipCullMask[nVipLevel] = 1;
			
			// 遍历任务描述 ...
			ArrayList list = new ArrayList(QuestFeeDesc_Map.Keys);
			list.Sort();
			for(int i=0;i<list.Count;i++)
			{
				QuestFeeMBInfo obj = (QuestFeeMBInfo) QuestFeeDesc_Map[list[i]];
				if (obj.ExcelID>0 && obj.vipRight>0)
				{
					if (obj.BooDone == 0 && vipCullMask[obj.vipRight] == 0)
					{
						// 未完成的VIP任务 ...
						QuestDesc boo;
						boo.ExcelID1 = (int) obj.ExcelID;
						boo.nDone2 = obj.BooDone;
						mVipFeeList.Add(boo);
					}
				}
				else if (obj.ExcelID>0 && obj.canRepeat == 0)
				{
					if (obj.BooDone == 0)
					{
						// 未完成的成长任务 ...
						QuestDesc boo;
						boo.ExcelID1 = (int) obj.ExcelID;
						boo.nDone2 = obj.BooDone;
						//print ("tttttttttttttt"+boo.ExcelID1);
						
						// VIP任务 ....
						if (obj.preAssort<1)
						{
							mGrowFeeList.Add(boo);
						}
						else if (growCullMask[obj.preAssort] == 0)
						{
							mGrowFeeList.Add(boo);
							growCullMask[obj.preAssort] = 1;
						}
					}
				}
			}
			
			if (processListQuestDelegate != null) {
				processListQuestDelegate();
			}
		}
		else 
		{
			LoadingManager.instance.HideLoading();
		}
		
		processListQuestDelegate = null;
		// print("STC_GAMECMD_GET_QUEST_T:" + msgCmd.nRst1 + "," + msgCmd.nNum2);
	}
	
	
}
