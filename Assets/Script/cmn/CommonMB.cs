using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CMNCMD;

//大区列表配置 
public struct ServerInfo
{
	//大区名称 
	public string ServerName;
	//大区IP 
	public string ServerIP;
	//大区端口 
	public int 	ServerPort;
	//大区繁忙程度 
	public string ServerStatus;
	//大区注册优先等级  
	public int 	 Priority;
	//是否已经满员 
	public int  IsFull;
	//大区ID 
	public int  ServerID;
}
//君主升级经验表  
public struct CharLevelExp
{
	//等级 
	public uint	Level;
	//需要经验 
	public int 	Exp;
}
//VIP特权信息  
public struct VipRightInfo
{
	//VIP等级 
	public int VipLevel;
	//累计钻石数 
	public int NeedDiamond;
	//邮件箱上限  
	public int MailBoxTop;
	//好友上限 
	public int FriendTop;
	//每天发送邮件上限 
	public int SendMailTop;
	//附加建筑队列 
	public int AddBuildingNum;
	//金矿成熟保护时间 
	public int GoldProtectTime;
	//许愿树成熟时间减少量 
	public int XuYuanShuSubTime;
	//副本免费次数 
	public int InstanceFreeNum;
	//副本收费次数 
	public int InstancePayNum;
	//行军加速 
	public float SubBattleTime;
	//PVP附加出征队列 ...
	public int MarchAddNum;
}
//建筑信息表（等级上限 建筑消耗时间 花费金钱等 从MB中获取） 
public struct BuildInfo
{
	//建筑ID 
	public int ID;
	//名字 
	public string Name;
	//等级 
	public int Level;
	//建造时间 
	public int BuildTime;
	//建造黄金 
	public int Money;
	//建造人口 
	public int Worker;
	//数量上限 
	public int TopNum;
	//auto_id_begin  
	public uint BeginID;
	//auto_id_end 
	public uint EndID;
}
//建筑数量上限表（金矿、居民、军营）  
public struct BuildingNum
{
	//君主等级  
	public uint LordLevel;
	//金矿数量上限  
	public int GoldNum;
	//居民数量上限 
	public int HouseNum;
	//军营数量上限 
	public int CampNum;
}
//城堡等级上限表（对应需要的发展度）   
public struct ChengBaoDevExp
{
	//城堡等级   
	public int ChengBaoLevel;
	//发展度  
	public int DevExp;
}
//民居生产表 （从MB中获取） 
public struct HouseProduction
{
	//等级 
	public int  Level;
	//生产速度  
	public int	Production;
	//生产时间  
	public int ProductTime;
	//容量  
	public int	Capacity;
}
//兵营容量表(MB获取)
public struct BingYingCapacity
{
	//等级 
	public int Level;
	//容量  
	public int	Capacity;
	
} 
//士兵信息表（MB获取） 
public struct SoldierMBInfo
{
	//士兵ID 
	public uint ExcelID;
	//等级 
	public uint 	Level;
	//士兵种类名字 
	public string Name;
	//生产黄金 
	public int Money;
	//攻击 
	public int Attack;
	//防御  
	public int Defense;
	//生命 
	public int Life;
	
}
//金矿生产配置表(MB获取)  
public struct GoldProduction
{
	//等级 
	public int 	Level;
	//生产时间1  
	public int 	ProduceTime1;
	//产量2   
	public int 	Production1;
	//生产时间2   
	public int 	ProduceTime2;
	//产量2  
	public int 	Production2;
	//生产时间3   
	public int 	ProduceTime3;
	//产量3  
	public int 	Production3;
	//生产时间4  
	public int 	ProduceTime4;
	//产量4  
	public int 	Production4;
	//是否允许偷窃 
	public byte EnabelSteal;
	//偷窃比率 
	public float StealScale;
	//偷窃次数 
	public float StealNum;
}
//将领升级修炼信息表 
public struct HeroExpGoldInfo
{
	//将领等级
	public int HeroLevel;
	//将领升级经验  
	public int LevelUpExp;
	//将领训练单位时间(1小时)获取经验值  
	public int TrainExpPerHour;
	//将领训练单位时间(1小时)花费金钱  
	public int TrainMoneyPerHour;
}
//武将修炼馆配置 
public struct TrainingGround
{
	//等级 
	public uint Level;
	//修炼将领等级上限 
	public int HeroTopLevel;
	//修炼时间上限 
	public int TrainTopTime;
}
//科技研究院配置  
public struct TechnologyInfo
{
	//科技等级  
	public uint TechLevel;
	//研究院等级 
	public int InsituteLevel;
	//研究费用 
	public int Money;
	//研究时间  
	public float ResearchTime;
}
//加速表（MB获取） 
public struct AccelerateInfo
{
	//单位时间  
	public float UnitTime;
	//消耗砖石或者水晶的数量 
	public int UnitCost;
}
//商城表 
public struct StoreItemInfo
{
	//类型唯一ID 
	public uint 	ExcelID;
	//钻石售价 
	public uint 	DiamondPrice;
	//水晶售价 
	public uint 	CrystalPrice;
	//是否可用水晶购买 
	public byte		SellByCrystal;
}
//钻石购买金币表 
public struct DiamondToGoldInfo
{
	//钻石数量  
	public int 	DiamondNum;
	//购买金币数量 
	public int 	GoldNum;
	
}
//装备ITEM表 
public struct EquipInfo
{
	//类型唯一ID 
	public uint 	ExcelID;
	//装备名字 
	public string 	EquipName;
	//装备描述  
	public string 	EquipDescribe ;
	//图标名字 
	public string 	IconName;
	//攻击 
	public int 		Attack;
	//防御  
	public int 		Defense;
	//生命 
	public int 		Life;
	//统帅 
	public int 		Leader;	
	//出售价格 
	public int 		SellPrice;
	//等级需求 
	public int 		NeedLevel;
	//装备部位 参考 enum EquipSlot 
	public int 		EuipSlot;
	
}
//宝石ITEM表  
public struct GemInfo
{
	//类型唯一ID  
	public uint 	ExcelID;
	//宝石名字  
	public string 	GemName;
	//宝石描述   
	public string 	GemDescribe ;
	//图标名字 
	public string 	IconName;
	//攻击 
	public int 		Attack;
	//防御  
	public int 		Defense;
	//生命 
	public int 		Life;
	//统帅 
	public int 		Leader;	
	//出售价格 
	public int 		SellPrice;
	//宝石类型  参考 enum GemSort 
	public int 		GemSort;
	
}
//用品ITEM表 
public struct ArticlesInfo
{
	//类型唯一ID  
	public uint 	ExcelID;
	//用品名字  
	public string 	Name;
	//用品描述   
	public string 	Describe ;
	//图标名字 
	public string 	IconName;
	//出售价格 
	public int 		SellPrice;
	//作用数值 
	public int 		EffectNum;
	//描述字符串(使用后获取的道具种类数量等)
	public string 	DescribeStr;
	//时效 (hour) 
	public int 		EffectTime;
	//抽奖表ID 
	public int      LotteryID;
}
//任务ITEM表 
public struct TaskInfo
{
	//类型唯一ID  
	public uint 	ExcelID;
	//用品名字  
	public string 	Name;
	//用品描述   
	public string 	Describe ;
	//图标名字 
	public string 	IconName;
	//出售价格 
	public int 		SellPrice;
}
//抽奖配置 
public struct LotteryMBInfo
{
	//每天免费抽奖次数 
	public int FreeLottery;
	//付费抽奖花费钻石或水晶数 
	public int PayLotteryPrice;
	//联盟抽奖消耗贡献度值 
	public int AllianceLotteryPrice;
	//联盟抽奖每天次数 
	public int AllianceLotteryNum;
}
//联盟信息表 
public struct AllianceInfo
{
	//等级 
	public int Level;
	//发展度 
	public int Development;
	//总人数 
	public int MemberNum;
	//副盟主数量 
	public int ViceLeaderNum;
	//管事  
	public int ManagerNum;
	//跑商加成 
	public float AddBusinessRate;
	//联盟副本上限 
	public int GuildWarMaxLevel;
}
// 新手引导 Guide
public struct QuestGuideMBInfo
{
	//步骤唯一ID
	public uint 	ExcelID;
	//名字 ...
	public string 	target;
	//终结技 - 存储点 ...
	public int 		Breakpoint;
	//任务点 ... 
	public uint		Taskpoint;
	//结束点 ...
	public int 		Finipoint;
	//连接点 ...
	public int 		Savepoint;
}

// 战斗前往时间 ...
public struct CombatGoToTimeMBInfo
{
	// 类型 ID
	public uint		ExcelID;
	// 前住时间 ...
	public int 		GoToTime;
	// 返回时是 ...
	public int 		BackTime;
}

// 出征列表 ...
public class WildBossMBInfo
{
	// 唯一ID
	public uint 	ExcelID;
	// 名字 ...
	public string 	name;
	// 描述 ...
	public string 	Desc;
	// 君主经验 ...
	public int 		Exp1;
	// 武将经验...
	public int 		Exp2;
	// 掉落物品 ...
	public int 		LootItem;
}

// 副本介绍 ...
public struct InstanceMBInfo
{
	// 嗆唯一ID
	public uint		ExcelID;
	// 模式 ...
	public string 	mode;
	// 奖励 - 存储点 ...
	public string	award;
	// 规则 (1-7)...
	public string 	Rule1;
}

// 副本描述信息 ...
public struct InstanceDescMBInfo
{
	// 唯一ID
	public uint		ExcelID;
	public int 		nRetryTimes;
}

//世界金矿 goldmine
public struct GoldmineMBInfo
{
	//区域类型唯一ID
	public uint 	ExcelID;
	//名字 
	public string	name;
	//空置时刻
	public int 		nVacantTime;
	//产出效果
	public int 		n1Addition;
	public int 		n1Secs;
	public int 		n1SafeTime;
	public int 		n2Addition;
	public int		n2Secs;
	public int 		n2SafeTime;
	public int 		n3Addition;
	public int 		n3Secs;
	public int 		n3SafeTime;
	public int 		n4Addition;
	public int 		n4Secs;
	public int 		n4SafeTime;
}

public struct GoldmineYieldMBInfo
{
	public uint 	ExcelID;
	public float 	exp;
}

//世界金矿 
public class FixhourMBInfo
{
	// 标记ID ...
	public uint		ExcelID;
	// 时刻表
	public List<int> times = new List<int>();
}

//兵种克制，加成 army addition
public struct ArmyDescMBInfo
{
	//唯一ID
	public uint		ExcelID;
	public int[]	aCrush;						// 克制兵种 crush
	public float	fBonus;						// 攻击加成百分比 fBonus
}

//多人副本，描述信息 nanzhenbeizhan
public struct NanBeizhanMBInfo
{
	//唯一ID
	public uint		ExcelID;
	public string	name;
	public int 		nLevel;	
	public int		nCGuard;
	public int 		nProf;
}

// 通用加速表格 ....
public struct CmnDefineMBInfo
{
	// 唯一ID
	public uint 	ExcelID;
	public float	num;
	public string 	explain;
}
//充值优惠价格 
public struct RechargeDiamond
{
	//充值金额 
	public float Money;
	//钻石数量 
	public int DiamondNum;
	//赠送钻石数量 
	public int SendDiamondNum;
}
//修改名字表（包括君主修改名字和将领修改名字） 
public struct ChangeNameInfo
{
	//改名类型 
	public int Type;
	//免费次数 
	public int FreeNum;
	//改名花费 
	public int Price;
}
//君主等级对应金币寄卖上限 
public struct GoldMarketLevel
{
	//君主等级  
	public uint LordLevel;
	//金币寄卖单笔最大数量  
	public int GoldSellMax;
}
//联盟等级对应的增兵数量和加速上限 
public struct GuildReinforceInfo
{
	//联盟等级 
	public uint GuildLevel;
	//联盟单日增兵上限 
	public int ReinforceMax;
}
//竞技场数据 
public struct ArenaInfo
{
	//竞技场可挑战排名差 
	public int Gap;
	//竞技场每日挑战次数 
	public int FreeTryNum;
	//竞技场每日付费次数 
	public int PaidTryNum;
	//竞技场上传数据时间CD 
	public int UploadCD;
	//参加竞技场最低等级 
	public int LevelLimit;
	//付费价格 
	public int TryPrice;
	
}
//服务器大区版本信息   
public struct ServerVersion
{
	//APPID 
	public int AppID;
	//版本号 
	public string Version;
}
public class CommonMB : MonoBehaviour {
	//大区列表 
	public static Hashtable ServerInfo_Map = new Hashtable();
	//当前客户端大版本号 
	public static string ClientVersion;
	//游戏APP号 
	public static int AppID;
	
	//君主升级经验表 
	public static Hashtable CharLevelExp_Map = new Hashtable();
	//VIP特权表 
	public static Hashtable VipRightInfo_Map = new Hashtable();
	//君主等级对应武将数量表 
	public static Hashtable HeroNum_Map	= new Hashtable();
	//建筑信息表  
	public static Hashtable  BuildingInfo_Map = new Hashtable();
	//建筑数量信息表  
	public static Hashtable  BuildingNum_Map = new Hashtable();
	//城堡等级信息表   
	public static Hashtable  ChengBaoDevExp_Map = new Hashtable();
	//民居的生产表   
	public static Hashtable HouseProduction_Map = new Hashtable();
	//兵营的容量表   
	public static Hashtable BingYingCapacity_Map = new Hashtable();
	//士兵MB信息表   
	public static Hashtable SoldierMB_Map = new Hashtable();
	//金矿生产信息表   
	public static Hashtable GoldProduction_Map = new Hashtable();
	//加速信息表  
	public static AccelerateInfo Accelerate_Info;
	//加速钻石打折表 
	public static Hashtable Accelereate_Discount_Map =  new Hashtable();
	//武将训练场信息表  
	public static Hashtable TrainGround_Map =  new Hashtable();
	//将领升级或修炼经验金钱信息表
	public static Hashtable HeroExpGold_Map = new Hashtable();
	//将领最高成长率 
	public static float HeroTopGrow;
	//医馆的草药产出表 
	public static Hashtable	CaoYaoByHospital =  new Hashtable();
	//草药治疗将领受伤点数对应表 
	public static Hashtable	HeroTreatCaoYao = new Hashtable();
	//科技研究院信息表 
	public static Hashtable  Technology_Map = new Hashtable();
	////excelID 和sortID对应表 
	public static Hashtable  ExcelToSort_Map = new Hashtable();
	//装备ITEM表 
	public static Hashtable  EquipInfo_Map  = new Hashtable();
	//宝石ITEM表   
	public static Hashtable  Gem_Map = new Hashtable(); 
	//用品ITEM表   
	public static Hashtable  Articles_Map  = new Hashtable();
	//任务ITEM表   
	public static Hashtable  Task_Map  = new Hashtable();
	//商城表     
	public static Hashtable  StoreInfo_Map = new Hashtable();
	//钻石购买金币表 
	public static DiamondToGoldInfo DiamondToGold_Info;
	//创建联盟花费金币数  
	public static int CreateAllianceGold;
	//联盟捐献一个贡献度对应的金币数 
	public static int AllianceContributeRate;
	//联盟信息表 
	public static Hashtable  AllianceInfo_Map =  new Hashtable();
	//抽奖信息表 
	public static LotteryMBInfo Lottery_Info;
	//世界金矿信息表 
	public static Hashtable  WorldGoldmine_Map = new Hashtable();
	//世界金矿产出系数表 ...
	public static Hashtable WorldGoldmineYieldMBInfo_Map = new Hashtable();
	//前住时间列表 ...
	public static Hashtable CombatGoToTimeMBInfo_Map = new Hashtable();
	// 重置时刻 ...
	public static Hashtable  FixhourMBInfo_Map = new Hashtable();
	//副本描述 ...
	public static Hashtable  InstanceMBInfo_Map = new Hashtable();
	//副本描述 ...
	public static Hashtable  InstanceDescMBInfo_Map = new Hashtable();
	//野外BOSS ....
	public static List<WildBossMBInfo>  WildBossMBInfo_List = new List<WildBossMBInfo>();
	public static Hashtable WildBossMBInfo_Map = new Hashtable();
	//兵种克制,加 addition
	public static Hashtable  ArmyDesc_Map = new Hashtable();
	//战役难度级别 nLevel
	public static Hashtable  NanBeizhanInfo_Map = new Hashtable();
	//任务列表 ...
	public static Hashtable  QuestDesc_Map = new Hashtable(); 
	//MB 任务指引表 ...
	public static Hashtable  QuestGuideInfo_Map = new Hashtable();
	//MB 新手引导表 ...
	public static Hashtable  NewbieGuideInfo_Map = new Hashtable();
	//MB 行军加带表数据 ...
	public static Hashtable	 CmnDefineMBInfo_Map = new Hashtable();
	//充值优惠表 
	public static Hashtable  RechargeDiamond_Map = new Hashtable();
	//修改名字表 
	public static Hashtable  ChangeNameInfo_Map =  new Hashtable();
	//寄卖金币等级表 
	public static Hashtable  GoldMarketLevel_Map = new Hashtable();
	//联盟增兵信息表 
	public static Hashtable  GuildReinforceInfo_Map = new Hashtable();
	//竞技场信息 
	public static ArenaInfo   Arena_Info;
	//非法字符串 
	public static List<string> BadNameList = new List<string>();
	//警告 错误TIP哈希表  
	public static Hashtable  Warn_Err_Tips_Map = new Hashtable();
	//服务器大区版本表 
	public static Hashtable ServerVersion_Map = new Hashtable();
 	// Use this for initialization  
	void Start () {
	
	}
	
	// Update is called once per frame 
	void Update () {
	
	}
	//从MB中获取二维字符串结构体(加密的) 
	public static string[][] GetStringFromMBText(string mb_name,bool Filled)
	{
		//TextAsset text_asset = (TextAsset)Resources.Load(mb_name,typeof(TextAsset));
		//byte[]	dat			= (byte[])text_asset.bytes.Clone();
		if(!File.Exists(mb_name+".txt"))
		{
			return null;
		}
			
		FileStream	text_asset		= new FileStream(mb_name+".txt", FileMode.Open);
		byte[]		dat		= new byte[text_asset.Length];
		text_asset.Read(dat, 0, dat.Length);
		byte[]	key			= DataConvert.StrToBytes(MBAutoUpdate.Pass);
		//	DataConvert.StrToBytes
		string [][] str_array = MBDeal.readMBFromBytes(dat,false,Filled);
		text_asset.Close();
		return str_array;
	}
	//从MB中获取二维字符串结构体(未加密的) 
	public static string[][] GetStringFromMBNoEncrypt(string mb_name)
	{
		TextAsset text_asset = (TextAsset)Resources.Load(mb_name,typeof(TextAsset));
		byte[]	dat			= (byte[])text_asset.bytes.Clone();
		string [][] str_array = MBDeal.readMBFromBytes(dat,false,true);
		return str_array;
	}
	//把NULL字符串转换为"0" 
	public static string CheckNullStr(string str)
	{
		string return_str = str;
		if(str == null)
			return_str = "0";
		if(str == "")
			return_str = "0"; 
		return return_str;
	}
	//初始化MB资源 
	public static void InitialMB()
	{
		InitialServerInfo();
		InitialClientVersion();
		InitialServerVersion();
		InitialCharLevelExp();
		InitialHeroNumInfo();
		InitialVipRightInfo();
		InitialTipFromMB();
		InitialBuilding();
		InitialBuildingNum();
		InitialChengBaoDevExp();
		InitialHouseProduction();
		InitialBingYingCapacity();
		InitialSoldierInfo();
		InitialGoldProductInfo();
		InitialAccelerateInfo();
		InitialAccelereateDiscountInfo();
		InitialTrainGroundInfo();
		InitialHeroExpGoldInfo();
		InitialHeroTopGrow();
		InitialCaoYaoProduction();
		InitialHeroCaoYaoInfo();
		InitialTechnologyInfo();
		InitialItemInfo();
		InitialStoreInfo();
		InitialDiamondToGoldInfo();
		InitialCreateAllianceGold();
		InitialAllianceContributeRate();
		InitialAllianceInfo();
		InitialLotteryInfo();
		InitialArmyDescInfo();
		InitialInstanceMBInfo();
		InitialWorldGoldmineInfo();
		InitialWorldGoldmineYieldMBInfo();
		InitialNanBeizhanMBInfo();
		//InitialQuestGuideMBInfo();
		InitialNewbieGuideMBInfo();
		InitialQuestMBInfo();
		InitialCombatGoToTimeMBInfo();
		InitialWildBossMBInfo();
		InitialInstanceListMBInfo();
		InitialFixhourMBInfo();
		InitialCommDefMBInfo();
	  	InitialRechargeDiamondInfo();
		InitialGoldMarketLevel();
		InitialChangeNameInfo();
		InitialArenaInfo();
		InitialBadNameList();
		InitialGuildReinforceInfo();
	}
	//MB载入大区列表 
	public static void InitialServerInfo()
	{
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "server_list",true);
		ServerInfo_Map.Clear();
		for(int i=0;i<str_array[0].Length;i++)
		{
			ServerInfo info = new ServerInfo();
			info.ServerName = str_array[0][i];
			info.ServerIP   = str_array[1][i];
			info.ServerPort = int.Parse(CheckNullStr(str_array[2][i]));
			info.ServerStatus  = str_array[3][i];
			info.Priority	= int.Parse(CheckNullStr(str_array[4][i]));
			info.IsFull = int.Parse(CheckNullStr(str_array[5][i]));
			info.ServerID = int.Parse(CheckNullStr(str_array[6][i]));
			ServerInfo_Map.Add(info.ServerName,info);
		}
		
	}
	//读取游戏本身大版本号 
	public static void InitialClientVersion()
	{
		string[][] str_array = GetStringFromMBNoEncrypt("MB/BigVersion");
		ClientVersion =	U3dCmn.CheckNullStr(str_array[0][0]);
		AppID = int.Parse(U3dCmn.CheckNullStr(str_array[0][1]));

	}
	//MB载入版本号 
	public static void InitialServerVersion()
	{
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "version",true);
		ServerVersion_Map.Clear();
		for(int i=0;i<str_array[0].Length;i++)
		{
			ServerVersion info= new ServerVersion();
			info.Version = str_array[0][i];
			info.AppID   = int.Parse(CheckNullStr(str_array[1][i]));
			ServerVersion_Map.Add(info.AppID,info);
		}

	}
	
	//MB载入君主升级经验信息 
	public static void InitialCharLevelExp()
	{
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "char_level_exp",true);
		CharLevelExp_Map.Clear();
		for(int i=0;i<str_array[0].Length;i++)
		{
			CharLevelExp info= new CharLevelExp();
			info.Level = uint.Parse(CheckNullStr(str_array[0][i]));
			info.Exp   = int.Parse(CheckNullStr(str_array[1][i]));
			CharLevelExp_Map.Add(info.Level,info);
		}
	}
	// vip特权表 
	public static void InitialVipRightInfo()
	{
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "vip_right",true);
		VipRightInfo_Map.Clear();
		for(int i=0;i<str_array[0].Length;i++)
		{
			VipRightInfo info= new VipRightInfo();
			info.VipLevel = int.Parse(CheckNullStr(str_array[0][i]));
			info.NeedDiamond   = int.Parse(CheckNullStr(str_array[1][i]));
			info.MailBoxTop   = int.Parse(CheckNullStr(str_array[2][i]));
			info.FriendTop   = int.Parse(CheckNullStr(str_array[3][i]));
			info.SendMailTop   = int.Parse(CheckNullStr(str_array[4][i]));
			info.AddBuildingNum   = int.Parse(CheckNullStr(str_array[5][i]));
			info.GoldProtectTime   = int.Parse(CheckNullStr(str_array[6][i]));
			info.XuYuanShuSubTime   = int.Parse(CheckNullStr(str_array[7][i]));
			info.InstanceFreeNum   = int.Parse(CheckNullStr(str_array[8][i]));
			info.InstancePayNum   = int.Parse(CheckNullStr(str_array[9][i]));
			info.SubBattleTime   = float.Parse(CheckNullStr(str_array[10][i]));
			info.MarchAddNum	= int.Parse(CheckNullStr(str_array[11][i]));
			VipRightInfo_Map.Add(info.VipLevel,info);
		}
	}
	//MB载入君主对应英雄数量表  
	public static void InitialHeroNumInfo()
	{
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "hero_num",true);
		HeroNum_Map.Clear();
		for(int i=0;i<str_array[0].Length;i++)
		{
			HeroNum_Map.Add(uint.Parse(CheckNullStr(str_array[0][i])),int.Parse(CheckNullStr(str_array[1][i])));
		}
	}
	
	//MB载入各建筑信息 
	public static void InitialBuilding()
	{
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "building_list",true);
		BuildingInfo_Map.Clear();
		
		for(int i=0;i<str_array[0].Length;i++)
		{
			BuildInfo info= new BuildInfo();
			info.ID = int.Parse(CheckNullStr(str_array[0][i]));
			info.Name = str_array[1][i];
			info.Level = int.Parse(CheckNullStr(str_array[2][i]));
			info.BuildTime = int.Parse(CheckNullStr(str_array[3][i]));	
			info.Money = int.Parse(CheckNullStr(str_array[4][i]));	
			info.Worker = int.Parse(CheckNullStr(str_array[5][i]));	
			info.TopNum = int.Parse(CheckNullStr(str_array[8][i]));
			info.BeginID = uint.Parse(CheckNullStr(str_array[9][i]));
			info.EndID = uint.Parse(CheckNullStr(str_array[10][i]));
			if(BuildingInfo_Map.Contains(info.ID))
			{
				Hashtable table = (Hashtable)BuildingInfo_Map[info.ID];
				table.Add(info.Level,info);
				BuildingInfo_Map[info.ID] = table;
			}
			else 
			{
				Hashtable table = new Hashtable();
				table.Add(info.Level,info);
				BuildingInfo_Map.Add(info.ID,table);
			}
		}
	}
	//MB载入建筑数量信息 
	public static void InitialChengBaoDevExp()
	{
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "building_level_limit",true);
		ChengBaoDevExp_Map.Clear();
		for(int i=0;i<str_array[0].Length;i++)
		{
			ChengBaoDevExp info= new ChengBaoDevExp();
			info.DevExp = int.Parse(str_array[0][i]);
			info.ChengBaoLevel = int.Parse(str_array[1][i]);
			ChengBaoDevExp_Map.Add(info.ChengBaoLevel,info);
		}
	}
	//MB载入城堡等级信息表  
	public static void InitialBuildingNum()
	{
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "building_num_limit",true);
		BuildingNum_Map.Clear();
		for(int i=0;i<str_array[0].Length;i++)
		{
			BuildingNum info= new BuildingNum();
			info.LordLevel = uint.Parse(str_array[0][i]);
			info.GoldNum = int.Parse(str_array[1][i]);
			info.HouseNum = int.Parse(str_array[2][i]);
			info.CampNum = int.Parse(str_array[3][i]);
	
			BuildingNum_Map.Add(info.LordLevel,info);
		}
	}
	//MB载入民居生产和容量信息 
	public static void InitialHouseProduction()
	{
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "house_production",true);
		HouseProduction_Map.Clear();
		for(int i=0;i<str_array[0].Length;i++)
		{
			HouseProduction info;
			info.Level = int.Parse(str_array[0][i]);
			info.Production = int.Parse(str_array[1][i]);
			info.ProductTime = int.Parse(str_array[2][i]);
			info.Capacity = int.Parse(str_array[3][i]);
		
			HouseProduction_Map.Add(info.Level,info);
		}
	}
	//MB载入兵营容量信息 
	public static void InitialBingYingCapacity()
	{
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "barrack_production",true);
		BingYingCapacity_Map.Clear();
		for(int i=0;i<str_array[0].Length;i++)
		{
			BingYingCapacity info;
			info.Level = int.Parse(str_array[0][i]);
			info.Capacity = int.Parse(str_array[1][i]);
			BingYingCapacity_Map.Add(info.Level,info);
		}
	}
	//MB载入士兵信息  
	public static void InitialSoldierInfo()
	{
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "soldier",true);
		SoldierMB_Map.Clear();
		for(int i=0;i<str_array[0].Length;i++)
		{
			SoldierMBInfo info= new SoldierMBInfo();
			info.ExcelID = uint.Parse(str_array[0][i]);
			info.Name = str_array[1][i];
			info.Level =  uint.Parse(str_array[2][i]);
			info.Money = int.Parse(str_array[3][i]);
			info.Attack = int.Parse(str_array[4][i]);
			info.Defense = int.Parse(str_array[5][i]);	
			info.Life = int.Parse(str_array[6][i]);	
			if(SoldierMB_Map.Contains(info.ExcelID))
			{
				Hashtable table = (Hashtable)SoldierMB_Map[info.ExcelID];
				table.Add(info.Level,info);
				SoldierMB_Map[info.ExcelID] = table;
			}
			else 
			{
				Hashtable table = new Hashtable();
				table.Add(info.Level,info);
				SoldierMB_Map.Add(info.ExcelID,table);
			}
		}
	}
	//MB载入金矿生产信息   
	public static void InitialGoldProductInfo()
	{
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "goldore_production",true);
		GoldProduction_Map.Clear();
		for(int i=0;i<str_array[0].Length;i++)
		{
			GoldProduction info= new GoldProduction();
			info.Level = int.Parse(str_array[0][i]);
			info.ProduceTime1 = int.Parse(str_array[1][i]);
			info.Production1 = int.Parse(str_array[2][i]);
			info.ProduceTime2 = int.Parse(str_array[3][i]);
			info.Production2 = int.Parse(str_array[4][i]);
			info.ProduceTime3 = int.Parse(str_array[5][i]);
			info.Production3 = int.Parse(str_array[6][i]);
			info.ProduceTime4 = int.Parse(str_array[7][i]);
			info.Production4 = int.Parse(str_array[8][i]);
			info.EnabelSteal = byte.Parse(str_array[9][i]);
			info.StealScale = float.Parse(str_array[10][i]);
			info.StealNum = int.Parse(str_array[11][i]);
			GoldProduction_Map.Add(info.Level,info);
		}
	}
	//MB载入加速信息表     
	public static void InitialAccelerateInfo()
	{
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "accelerate_cost",true);
		Accelerate_Info.UnitTime = float.Parse(str_array[0][0]);
		Accelerate_Info.UnitCost = int.Parse(str_array[1][0]);
	}
	//MB载入加速钻石打折表    
	public static void InitialAccelereateDiscountInfo()
	{
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "accelerate_cost_discount",true);
		Accelereate_Discount_Map.Clear();
		for(int i=0;i<str_array[0].Length;i++)
		{	
			Accelereate_Discount_Map.Add(int.Parse(str_array[0][i]),float.Parse(str_array[1][i]));
		}
	}
	//MB载入道具信息(包含装备 宝石 和其他商城售卖的道具)      
	public static void InitialItemInfo()
	{
		EquipInfo_Map.Clear();
		Gem_Map.Clear();
		Articles_Map.Clear();
		Task_Map.Clear();
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "item_list",true);
		for(int i=0;i<str_array[0].Length;i++)
		{
			uint SortID = uint.Parse(CheckNullStr(str_array[3][i]));
			ExcelToSort_Map.Add(uint.Parse(CheckNullStr(str_array[0][i])),SortID);
			switch(SortID)
			{
				case (uint)ItemSort.EQUIP: // 装备信息表  
				{
					EquipInfo equip_info;
					equip_info.ExcelID =  uint.Parse(CheckNullStr(str_array[0][i]));
					equip_info.EquipName =  str_array[1][i];
					equip_info.EquipDescribe =  str_array[2][i];
					equip_info.IconName =  str_array[4][i];
					equip_info.SellPrice = int.Parse(CheckNullStr(str_array[5][i]));
					equip_info.Attack =  int.Parse(CheckNullStr(str_array[6][i]));
					equip_info.Defense =  int.Parse(CheckNullStr(str_array[7][i]));
					equip_info.Life =  int.Parse(CheckNullStr(str_array[8][i]));
					equip_info.Leader =  int.Parse(CheckNullStr(str_array[9][i]));
					
					equip_info.NeedLevel = int.Parse(CheckNullStr(str_array[14][i]));
					equip_info.EuipSlot = int.Parse(CheckNullStr(str_array[16][i]));
					EquipInfo_Map.Add(equip_info.ExcelID,equip_info);
				}
				break;
				case (uint)ItemSort.GEMSTONE: //宝石信息表  
				{
					GemInfo gem_info;
					gem_info.ExcelID =  uint.Parse(CheckNullStr(str_array[0][i]));
					gem_info.GemName =  str_array[1][i];
					gem_info.GemDescribe =  str_array[2][i];
					gem_info.IconName =  str_array[4][i];
					gem_info.SellPrice = int.Parse(CheckNullStr(str_array[5][i]));
					gem_info.Attack =  int.Parse(CheckNullStr(str_array[6][i]));
					gem_info.Defense =  int.Parse(CheckNullStr(str_array[7][i]));
					gem_info.Life =  int.Parse(CheckNullStr(str_array[8][i]));
					gem_info.Leader =  int.Parse(CheckNullStr(str_array[9][i]));
					gem_info.GemSort	= int.Parse(CheckNullStr(str_array[17][i]));
					Gem_Map.Add(gem_info.ExcelID,gem_info);
				}
				break;
				case (uint)ItemSort.ARTICLES: //用品信息  
				{
					ArticlesInfo articles_info;
					articles_info.ExcelID =  uint.Parse(CheckNullStr(str_array[0][i]));
					articles_info.Name =  str_array[1][i];
					articles_info.Describe =  str_array[2][i];
					articles_info.IconName =  str_array[4][i];
					articles_info.SellPrice = int.Parse(CheckNullStr(str_array[5][i]));
					articles_info.EffectNum =  int.Parse(CheckNullStr(str_array[10][i]));
					articles_info.DescribeStr = str_array[11][i];
					articles_info.EffectTime =  int.Parse(CheckNullStr(str_array[12][i]));
					articles_info.LotteryID =  int.Parse(CheckNullStr(str_array[13][i]));
					Articles_Map.Add(articles_info.ExcelID,articles_info);
				}
				break;
				case (uint)ItemSort.TASKITEM: //任务信息  
				{
					TaskInfo task_info;
					task_info.ExcelID =  uint.Parse(CheckNullStr(str_array[0][i]));
					task_info.Name =  str_array[1][i];
					task_info.Describe =  str_array[2][i];
					task_info.IconName =  str_array[4][i];
					task_info.SellPrice = int.Parse(CheckNullStr(str_array[5][i]));
					Task_Map.Add(task_info.ExcelID,task_info);
				}
				
				break;
			}
		}
	}
	//MB载入商城信息     
	public static void InitialStoreInfo()
	{
		StoreInfo_Map.Clear();
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "store_items",true);
		for(int i=0;i<str_array[0].Length;i++)
		{
			StoreItemInfo store_item;
			store_item.ExcelID =  uint.Parse(CheckNullStr(str_array[0][i]));
			store_item.CrystalPrice =  uint.Parse(CheckNullStr(str_array[1][i]));
			store_item.DiamondPrice =  uint.Parse(CheckNullStr(str_array[2][i]));
			store_item.SellByCrystal =  byte.Parse(CheckNullStr(str_array[3][i]));
			StoreInfo_Map.Add(store_item.ExcelID,store_item);
		}
	}
	//MB载入钻石购买金币信息     
	public static void InitialDiamondToGoldInfo()
	{
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "store_conversion_rate",true);	
		DiamondToGold_Info.DiamondNum = int.Parse(CheckNullStr(str_array[0][0]));
		DiamondToGold_Info.GoldNum = int.Parse(CheckNullStr(str_array[1][0]));
	}
	//MB载入武将修炼馆信息 
	public static void InitialTrainGroundInfo()
	{
		
		TrainGround_Map.Clear();
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "training_hall",true);
		for(int i=0;i<str_array[0].Length;i++)
		{
			TrainingGround unit;
			unit.Level =  uint.Parse(CheckNullStr(str_array[0][i]));
			unit.HeroTopLevel =  int.Parse(CheckNullStr(str_array[1][i]));
			unit.TrainTopTime =  int.Parse(CheckNullStr(str_array[2][i]));
			TrainGround_Map.Add(unit.Level,unit);
		}
	}
	
	//MB载入将领升级或修炼经验金钱信息 
	public static void InitialHeroExpGoldInfo()
	{
		HeroExpGold_Map.Clear();
		//HeroLevelExp_Map.Clear();
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "hero_level_exp",true);
		for(int i=0;i<str_array[0].Length;i++)
		{
			HeroExpGoldInfo unit;
			unit.HeroLevel =  int.Parse(CheckNullStr(str_array[0][i]));
			unit.LevelUpExp =  int.Parse(CheckNullStr(str_array[1][i]));
			unit.TrainExpPerHour =  int.Parse(CheckNullStr(str_array[2][i]));
			unit.TrainMoneyPerHour =  int.Parse(CheckNullStr(str_array[3][i]));
			HeroExpGold_Map.Add(unit.HeroLevel,unit);
		}
	}
	//初始化将领最高成长率 
	public static void InitialHeroTopGrow()
	{
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "hero_grow",true);
		HeroTopGrow =  float.Parse(CheckNullStr(str_array[0][str_array[0].Length-1]));
	}
	//MB载入医馆的草药产出表 
	public static void InitialCaoYaoProduction()
	{
		CaoYaoByHospital.Clear();
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "drug",true);
		for(int i=0;i<str_array[0].Length;i++)
		{
			CaoYaoByHospital.Add(int.Parse(CheckNullStr(str_array[0][i])),int.Parse(CheckNullStr(str_array[1][i])));
		}
		
	}
	//MB载入草药治疗将领受伤点对应表 
	public static void InitialHeroCaoYaoInfo()
	{
		HeroTreatCaoYao.Clear();
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "heal",true);
		for(int i=0;i<str_array[0].Length;i++)
		{
			HeroTreatCaoYao.Add(int.Parse(CheckNullStr(str_array[0][i])),int.Parse(CheckNullStr(str_array[1][i])));
		}
		
	}
	//MB载入科技研究院信息 
	public static void InitialTechnologyInfo()
	{
		Technology_Map.Clear();
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "technology",true);
		for(int i=0;i<str_array[0].Length;i++)
		{
			TechnologyInfo unit;
			unit.TechLevel =  uint.Parse(CheckNullStr(str_array[0][i]));
			unit.InsituteLevel =  int.Parse(CheckNullStr(str_array[1][i]));
			unit.Money =  int.Parse(CheckNullStr(str_array[2][i]));
			unit.ResearchTime =  float.Parse(CheckNullStr(str_array[3][i]));
			Technology_Map.Add(unit.TechLevel,unit);
		}
	}
	//MB载入创建联盟花费的金币数量  
	public static void InitialCreateAllianceGold()
	{
		CreateAllianceGold = 0;
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "alliance_create_cost",true);
		CreateAllianceGold = int.Parse(CheckNullStr(str_array[0][0]));
	}
	//MB载入联盟捐献贡献度对应金币数  
	public static void InitialAllianceContributeRate()
	{
		AllianceContributeRate = 0;
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "alliance_contribute_rate",true);
		AllianceContributeRate = int.Parse(CheckNullStr(str_array[0][0]));
	}
	//MB载入联盟信息表 
	public static void InitialAllianceInfo()
	{
		AllianceInfo_Map.Clear();
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "alliance_congress",true);
		for(int i=0;i<str_array[0].Length;i++)
		{
			AllianceInfo unit;
			unit.Level =  int.Parse(CheckNullStr(str_array[0][i]));
			unit.Development =  int.Parse(CheckNullStr(str_array[1][i]));
			unit.MemberNum =  int.Parse(CheckNullStr(str_array[2][i]));
			unit.ViceLeaderNum =  int.Parse(CheckNullStr(str_array[3][i]));
			unit.ManagerNum =  int.Parse(CheckNullStr(str_array[4][i]));
			unit.AddBusinessRate =  float.Parse(CheckNullStr(str_array[5][i]));
			unit.GuildWarMaxLevel = int.Parse(CheckNullStr(str_array[6][i]));
			AllianceInfo_Map.Add(unit.Level,unit);
		}
	}
	//MB载入抽奖信息 
	public static void InitialLotteryInfo()
	{
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "lottery_cfg",true);
		Lottery_Info.FreeLottery = int.Parse(CheckNullStr(str_array[0][0]));
		Lottery_Info.PayLotteryPrice = int.Parse(CheckNullStr(str_array[1][0]));
		Lottery_Info.AllianceLotteryPrice = int.Parse(CheckNullStr(str_array[2][0]));
		Lottery_Info.AllianceLotteryNum = int.Parse(CheckNullStr(str_array[3][0]));
	}
	//MB载入军队克制信息 crush
	public static void InitialArmyDescInfo()
	{
		ArmyDesc_Map.Clear();
		
		char[] chSep = { '*' };
		
		string cSplit;
		string[] d2;
		
		string[][] data = GetStringFromMBText(MBAutoUpdate.localDir + "army",true);
		for (int i=0, imax=data[0].Length; i<imax; ++ i)
		{
			ArmyDescMBInfo crush;
			crush.ExcelID = uint.Parse(data[0][i]);
			crush.fBonus = float.Parse(data[3][i]);
			
			cSplit = data[2][i];
			d2 = cSplit.Split(chSep,2);
			
			crush.aCrush = new int[2];
			crush.aCrush[0] = int.Parse(d2[0]);
			crush.aCrush[1] = int.Parse(d2[1]);
			
			ArmyDesc_Map.Add(crush.ExcelID, crush);
		}
		
	}
	
	//MB多人副本信息排布 init
	public static void InitialNanBeizhanMBInfo()
	{
		NanBeizhanInfo_Map.Clear();
		
		string[][] data = GetStringFromMBText(MBAutoUpdate.localDir + "nanzhengbeizhan_cli", true);
		//string[][] data = GetStringFromMBNoEncrypt("MB/nanzhengbeizhan_cli");
		for (int i=0, imax=data[0].Length; i<imax; ++ i)
		{
			NanBeizhanMBInfo info;
			info.ExcelID = uint.Parse(CheckNullStr(data[0][i]));
			info.name = data[1][i];
			info.nLevel = int.Parse(data[2][i]);
			info.nCGuard = int.Parse(data[3][i]);
			info.nProf = int.Parse(data[4][i]);
			NanBeizhanInfo_Map.Add(info.ExcelID, info);
		}
	}
	
	//MB任务列表信息 ...
	public static void InitialQuestMBInfo()
	{
		string[][] data = GetStringFromMBText(MBAutoUpdate.localDir + "quest_list", true);
		QuestFeeManager.ParseByQuestList(data);
	}
	
	//MB任务引导列表信息 ...
	public static void InitialQuestGuideMBInfo()
	{
		QuestGuideInfo_Map.Clear();	
		string[][] data = GetStringFromMBText(MBAutoUpdate.localDir + "quest_guide", true);
		//string[][] data = GetStringFromMBNoEncrypt("MB/quest_guide");
		int imax = data[0].Length;
		for(int i=0; i<imax; ++ i)
		{
			QuestGuideMBInfo info;
			info.ExcelID = uint.Parse(CheckNullStr(data[0][i]));
			info.target = data[1][i];
			info.Breakpoint = int.Parse(CheckNullStr(data[2][i]));
			info.Taskpoint = uint.Parse(CheckNullStr(data[3][i]));
			info.Finipoint = int.Parse(CheckNullStr(data[5][i]));
			info.Savepoint = int.Parse(CheckNullStr(data[5][i]));
			QuestGuideInfo_Map.Add(info.ExcelID, info);
		}
	}
	
	//MB任务引导列表信息 ...
	public static void InitialNewbieGuideMBInfo()
	{
		NewbieGuideInfo_Map.Clear();
				
		string[][] data = GetStringFromMBText(MBAutoUpdate.localDir + "newbie_guide", true);
		//string[][] data = GetStringFromMBNoEncrypt("MB/newbie_guide");
		int imax = data[0].Length;
		for(int i=0; i<imax; ++ i)
		{
			QuestGuideMBInfo info;
			info.ExcelID = uint.Parse(CheckNullStr(data[0][i]));
			info.target = data[1][i];
			info.Breakpoint = int.Parse(CheckNullStr(data[2][i]));
			info.Taskpoint = uint.Parse(CheckNullStr(data[3][i]));
			info.Finipoint = int.Parse(CheckNullStr(data[5][i]));
			info.Savepoint = int.Parse(CheckNullStr(data[5][i]));
			NewbieGuideInfo_Map.Add(info.ExcelID, info);
		}
	}
	
	//MB世界金矿信息初始化 init
	public static void InitialWorldGoldmineInfo()
	{
		WorldGoldmine_Map.Clear();
		
		char[] chSep = { '*' };
		
		string cSplit;
		string[] d3;
		
		string[][] data = GetStringFromMBText(MBAutoUpdate.localDir + "worldgoldmine",true);
		for (int i=0, imax=data[0].Length; i<imax; ++ i)
		{
			GoldmineMBInfo newCard;
			newCard.ExcelID = uint.Parse(CheckNullStr(data[0][i]));
			newCard.name = data[1][i];
			newCard.nVacantTime = int.Parse(data[2][i]);
			
			cSplit = data[3][i];
			d3 = cSplit.Split(chSep,4);
			
			newCard.n1Addition = int.Parse(d3[1]);
			newCard.n1Secs = int.Parse(d3[2]);
			newCard.n1SafeTime = int.Parse(d3[3]);
			
			cSplit = data[4][i];
			d3 = cSplit.Split(chSep,4);
			
			newCard.n2Addition = int.Parse(d3[1]);
			newCard.n2Secs = int.Parse(d3[2]);
			newCard.n2SafeTime = int.Parse(d3[3]);
			
			cSplit = data[5][i];
			d3 = cSplit.Split(chSep,4);

			newCard.n3Addition = int.Parse(d3[1]);
			newCard.n3Secs = int.Parse(d3[2]);
			newCard.n3SafeTime = int.Parse(d3[3]);
			
			cSplit = data[6][i];
			d3 = cSplit.Split(chSep,4);
			
			newCard.n4Addition = int.Parse(d3[1]);
			newCard.n4Secs = int.Parse(d3[2]);
			newCard.n4SafeTime = int.Parse(d3[3]);
			
			WorldGoldmine_Map.Add(newCard.ExcelID, newCard);
		}
		
	}
	
	//MB行军加速表 ...
	public static void InitialCommDefMBInfo()
	{
		CmnDefineMBInfo_Map.Clear();
		string[][] data = GetStringFromMBText(MBAutoUpdate.localDir + "cmndef",true);
		if(data != null)
		{
			for (int i=0, imax=data[0].Length; i<imax; ++ i)
			{
				CmnDefineMBInfo newCard;
				newCard.ExcelID = uint.Parse(CheckNullStr(data[0][i]));
				newCard.num = float.Parse(CheckNullStr(data[1][i]));
				newCard.explain = CheckNullStr(data[2][i]);
				
				CmnDefineMBInfo_Map.Add(newCard.ExcelID, newCard);
			}
		}
		
	}
	//MB获取充值优惠打折数据 
	public static void InitialRechargeDiamondInfo()
	{
		RechargeDiamond_Map.Clear();
		string[][] data = GetStringFromMBText(MBAutoUpdate.localDir + "recharge_diamond",true);
		for (int i=0, imax=data[0].Length; i<imax; ++ i)
		{
			RechargeDiamond rechargeinfo;
			rechargeinfo.Money = float.Parse(CheckNullStr(data[0][i]));
			rechargeinfo.DiamondNum = int.Parse(CheckNullStr(data[1][i]));
			rechargeinfo.SendDiamondNum = int.Parse(CheckNullStr(data[2][i]));
			
			RechargeDiamond_Map.Add(rechargeinfo.Money, rechargeinfo);
		}
	}
	
	//MB
	public static void InitialInstanceMBInfo()
	{
		InstanceMBInfo_Map.Clear();
		string[][] data = GetStringFromMBText(MBAutoUpdate.localDir + "instance_tips", true);
		int imax = data[0].Length;
		for (int i=0; i<imax; ++ i)
		{
			InstanceMBInfo card;
			card.ExcelID = uint.Parse(CheckNullStr(data[0][i]));
			card.mode = data[1][i];
			card.award = data[2][i];
			card.Rule1 = data[3][i];
			InstanceMBInfo_Map.Add(card.ExcelID, card);
		}
		
	}
	
	public static void InitialInstanceListMBInfo()
	{
		InstanceDescMBInfo_Map.Clear();
		string[][] data = GetStringFromMBText(MBAutoUpdate.localDir + "instance_list", true);
		int imax = data[0].Length;
		for (int i=0; i<imax; ++ i)
		{
			InstanceDescMBInfo card;
			card.ExcelID = uint.Parse(CheckNullStr(data[0][i]));
			card.nRetryTimes = int.Parse(CheckNullStr(data[11][i]));
			InstanceDescMBInfo_Map.Add(card.ExcelID, card);
		}
	}
			
	//从MB获取警告或者错误提示   
	public static void InitialTipFromMB()
	{
		Warn_Err_Tips_Map.Clear();
		string[][] str_array  = GetStringFromMBText(MBAutoUpdate.localDir + "warn_err_tips", true);
		//string[][] str_array = GetStringFromMBNoEncrypt("MB/warn_err_tips");
		for(int i=0;i<str_array[0].Length;i++)
		{
			int tipid 	=  int.Parse(CheckNullStr(str_array[0][i]));
			string tip 	=  CheckNullStr(str_array[1][i]);
			Warn_Err_Tips_Map.Add(tipid,tip);
		}
	}
	//从MB获取修改名字信息 
	public static  void InitialChangeNameInfo()
	{
		ChangeNameInfo_Map.Clear();
		string[][] data = GetStringFromMBText(MBAutoUpdate.localDir + "change_name",true);
		for (int i=0, imax=data[0].Length; i<imax; ++ i)
		{
			ChangeNameInfo changenameinfo;
			changenameinfo.Type = int.Parse(CheckNullStr(data[0][i]));
			changenameinfo.FreeNum = int.Parse(CheckNullStr(data[1][i]));
			changenameinfo.Price = int.Parse(CheckNullStr(data[2][i]));
			
			ChangeNameInfo_Map.Add(changenameinfo.Type, changenameinfo);
		}
	}
	
	static void SplitAwardProps(ref int awardId, string textVal)
	{
		string[] sSp = new string[1]{"*"};
		string[] coSp = textVal.Split(sSp, System.StringSplitOptions.None);
		
		int imax = coSp.Length;
		if (imax>0)
		{
			awardId = int.Parse(CheckNullStr(coSp[0]));
		}
	}
	
	public static void InitialWildBossMBInfo()
	{
		WildBossMBInfo_Map.Clear();
		WildBossMBInfo_List.Clear();
		string[][] data = GetStringFromMBText(MBAutoUpdate.localDir + "combat_raid", true);
		for (int i=0, imax=data[0].Length; i<imax; ++ i)
		{
			WildBossMBInfo card = new WildBossMBInfo();
			card.ExcelID = uint.Parse(CheckNullStr(data[0][i]));
			card.name = data[1][i];
			card.Desc = data[2][i];
			card.Exp1 = int.Parse(CheckNullStr(data[6][i]));
			card.Exp2 = int.Parse(CheckNullStr(data[7][i]));
			SplitAwardProps(ref card.LootItem, CheckNullStr(data[5][i]));
			WildBossMBInfo_List.Add(card);
			WildBossMBInfo_Map.Add(card.ExcelID, card);
		}
	}
	
	public static WildBossMBInfo GetWildBossItem(int idx)
	{
		int imax = WildBossMBInfo_List.Count;
		if (idx<0 || idx>(imax-1)) return null;
		return WildBossMBInfo_List[idx];
	}
	
	static void SplitTimeProps(ref FixhourMBInfo data, string textVal)
	{
		string[] sSp = new string[1]{"*"};
		string[] coSp = textVal.Split(sSp, System.StringSplitOptions.None);
		
		int imax = coSp.Length;
		data.times.Clear();
		
		for (int i=0; i<imax; ++ i)
		{
			int cc = int.Parse(CheckNullStr(coSp[i*1]));
			data.times.Add(cc);
		}
	}
	
	public static void InitialFixhourMBInfo()
	{
		FixhourMBInfo_Map.Clear();
		string[][] data = GetStringFromMBText(MBAutoUpdate.localDir + "fixhour", true);
		for (int i=0, imax=data[0].Length; i<imax; ++ i)
		{
			FixhourMBInfo card = new FixhourMBInfo();
			card.ExcelID = uint.Parse(CheckNullStr(data[0][i]));
			SplitTimeProps(ref card, CheckNullStr(data[1][i]));
			FixhourMBInfo_Map.Add(card.ExcelID, card);
		}
	}
	
	public static void InitialWorldGoldmineYieldMBInfo()
	{
		WorldGoldmineYieldMBInfo_Map.Clear();
		string[][] data = GetStringFromMBText(MBAutoUpdate.localDir + "worldgoldmine_out", true);
		for (int i=0, imax=data[0].Length; i<imax; ++ i)
		{
			GoldmineYieldMBInfo card;
			card.ExcelID = uint.Parse(CheckNullStr(data[0][i]));
			card.exp = float.Parse(CheckNullStr(data[1][i]));
			WorldGoldmineYieldMBInfo_Map.Add(card.ExcelID,card);
		}
	}
	
	public static void InitialCombatGoToTimeMBInfo()
	{
		CombatGoToTimeMBInfo_Map.Clear();
		string[][] data = GetStringFromMBText(MBAutoUpdate.localDir + "combat", true);
		for (int i=0, imax=data[0].Length; i<imax; ++ i)
		{
			CombatGoToTimeMBInfo card;
			card.ExcelID = uint.Parse(CheckNullStr(data[0][i]));
			card.GoToTime = int.Parse(CheckNullStr(data[1][i]));
			card.BackTime = int.Parse(CheckNullStr(data[2][i]));
			CombatGoToTimeMBInfo_Map.Add(card.ExcelID, card);
		}
	}
	//寄卖金币等级表 
	public static void InitialGoldMarketLevel()
	{
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "gold_market_level",true);
		GoldMarketLevel_Map.Clear();
		for(int i=0;i<str_array[0].Length;i++)
		{
			GoldMarketLevel info= new GoldMarketLevel();
			info.LordLevel  = uint.Parse(CheckNullStr(str_array[0][i]));
			info.GoldSellMax   = int.Parse(CheckNullStr(str_array[1][i]));
			GoldMarketLevel_Map.Add(info.LordLevel,info);
		}
	}
	//联盟增兵信息表 
	public static void InitialGuildReinforceInfo()
	{
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "alliance_donate_soldier",true);
		GuildReinforceInfo_Map.Clear();
		for(int i=0;i<str_array[0].Length;i++)
		{
			GuildReinforceInfo info = new GuildReinforceInfo();
			info.GuildLevel  = uint.Parse(CheckNullStr(str_array[0][i]));
			info.ReinforceMax   = int.Parse(CheckNullStr(str_array[1][i]));
			GuildReinforceInfo_Map.Add(info.GuildLevel,info);
		}
	}
	//竞技场信息表  
	public static void InitialArenaInfo()
	{
		string[][] str_array = GetStringFromMBText(MBAutoUpdate.localDir + "arena",true);
		Arena_Info.Gap  = int.Parse(CheckNullStr(str_array[0][0]));
		Arena_Info.FreeTryNum   = int.Parse(CheckNullStr(str_array[1][0]));
		Arena_Info.PaidTryNum  = int.Parse(CheckNullStr(str_array[2][0]));
		Arena_Info.UploadCD   = int.Parse(CheckNullStr(str_array[3][0]));
		Arena_Info.LevelLimit  = int.Parse(CheckNullStr(str_array[4][0]));
		Arena_Info.TryPrice   = int.Parse(CheckNullStr(str_array[5][0]));
		
	}
	//非法字符串 
	public static void InitialBadNameList()
	{
		BadNameList.Clear();
		string[][] data = GetStringFromMBText(MBAutoUpdate.localDir + "badname",true);
		for (int i=0, imax=data[0].Length; i<imax; ++ i)
		{	
			string str = CheckNullStr(data[0][i]);
			BadNameList.Add(str);
		}
	}
}
