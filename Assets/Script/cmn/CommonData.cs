using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using CMNCMD;
public class CommonData {
	//版本 
	public static VERSION_TYPE VERSION = VERSION_TYPE.VERSION_INNER;
	//APP版本 
	public static APP_VERSION_TYPE APP_VERSION = APP_VERSION_TYPE.TINYKINGDOM;
	//设备 
	//public static int DEVICE = (int)DEVICE_TYPE.IPHONE;
	public static Vector3 iphonescale =  new Vector3(0.00625f,0.00625f,0.00625f);
	public static Vector3 ipadscale =  new Vector3(0.005555556f,0.005555556f,0.005555556f);
	public static SERVER_UNIT select_server;
	//public static CMN_PLAYER_UNIT  player_online;
	public static CMN_PLAYER_CARD_INFO player_online_info;
	
	//当前浇水的摇钱树是自己的还是别人的 
	public static int water_yaoqianshu;
	//当前账号绑定的mac地址 
	public static string AccountBandMac = "";
	//当前设备的MAC地址 
	public static string NowDeviceMac = "";
	//public static short[] EQUIP_LIST = {1800,1801,1802,1803,1804,1805,1806,1807,1808,1809,1810,1811,1812,1813,1814};
	//public static short[] DRESS_LIST = {1815,1816,1817,1818,1819};
	// Use this for initialization

	public static void Clear()
	{
			
	}
	
}

public struct SERVER_UNIT
{
	//大区名字 
	public string ServerName;
	//大区IP 
	public string ServerIP;
	//大区端口 
	public int ServerPort;
	//大区状况 
	public string ServerStatus;
	//客户端版本号 
	public string ClientVersion;
	//服务器版本号 
	public string ServerVersion;
	//是否已经满员 
	public int ServerIsFull;
	//大区ID 
	public int ServerID;
}
//账号信息
public struct CMN_PLAYER_UNIT
{
	public string 			AccountName;
	public string 			AccountPass;
	public ulong 			AccountID;
	public string 			CharName;
	public ulong 			CharID;
	public uint 			level;
	public ushort			wExcelID;
	public byte				bySex;
}
//玩家简单信息 
public struct CMN_PLAYER_CARD_INFO
{
	public string 		AccountName;						//账号名字 
	public string 		AccountPass;						//账号密码 
	public ulong 		AccountID;							//账号ID 
	public string		CharName;							// 角色名字  

	public uint			Sex;								//性别 
	public ulong 		AllianceID;							//联盟ID 
	public string		AllianceName;						//联盟名字 
	public uint			nHeadID;							//头像ID 
	public uint			Level;								//等级 
	public uint 		LevelRank;							//等级排行 
	public uint			Vip;								//VIP 
	public uint			Exp;								//当前经验值   
	public uint			Diamond;							//钻石  
	public uint			Crystal;							//水晶  
	public uint			Development;						//发展度    
	public uint			Gold;								//黄金      
	public uint 		Population;							//人口  
	public uint 		CaoYao;								//草药 
	public uint 		BuildNum;							//建造队列  
	public uint 		AddedBuildNum;						//附加建造队列    
	public uint			PosX;								//世界坐标X   
	public uint			PosY;								//世界坐标Y  

	public uint			FreeDrawLotteryNum;					//免费抽奖次数 	
	public string 		Signature;							//个人签名 
	public int 			ProtectEndTime;						//保护结束时间 
	public int 			AddBuildNumTime;					//建筑队列增加剩余时间 
	public uint 		TotalBuildNum;						//总共的建筑队列 
	public uint  		NotificationID;						//最近阅读了的通知最新ID 
	public uint 		AllianceLotteryNum;					//今日联盟抽奖剩余次数 
	public uint 		TotalDiamond;						//总充值钻石数(不含赠送)
	public uint 		WangZhe;							//王者之路战绩  
	public uint 		ZhengZhan;							//征战天下战绩 
	public uint 		HonorCup;							//圣杯 
	public byte			BindMail;							//是否绑定邮箱 
	public byte			ShowVip;							//是否显示VIP 
}
//聊天信息 
public struct CHAT_DATA 
{
	public uint 	ChatId;
	public int 		ChatType;
	public string	szFrom;
	public string 	szTo;
	public ulong	nCharID;
	public uint 	ChatTime;
	public string	szText;
	public uint 	Vip;

}

public class COMBAT_DEATH_INFO
{
	public string 	HeroName;
	public int	 	nLevel;
	public int 		nArmyLevel;
	public int 		nArmyType;
	public int 		nArmyNum;
	public int 		nArmyDeadNum;
	public int		nArmyNumRecover;
}
//联盟信息 
public struct ALLIANCE_INFO
{
	public ulong 		AllianceID;			//联盟ID   
	public string   	AllianceName;		//联盟名称 
	public uint 		AllianceLevel;		//联盟等级 	
	public uint 		AllianceRank;		//联盟排名  	
	public ulong 		LeaderID;			//盟主ID 
	public string		LeaderName;			//盟主名称 
	public uint 		MyPost;				//我的职位 
	public uint 		MemberNum;			//联盟成员数量 
	public uint 		MemberNumMax;		//联盟最大成员数 
	public int			LeaderNum;			//副盟主成员数量 .... 
	public int			ManagerNum;			//管事数量 ... 
	public uint			Development;		//联盟发展度 
	public string 		Introduction;		//联盟简介 
	public uint 		NowContribute;		//个人可用贡献度 
	public uint 	 	TotalContribute;	//个人历史贡献度 ... 

}
//物品简单描述信息 
public struct ITEM_INFO
{
	public uint 	ExcelID;
	//用品名字  
	public string 	Name;
	//用品描述   
	public string 	Describe; 
	//图标名字 
	public string 	IconName;	
}
//角色简单信息 
public struct SIMPLE_CHAR_INFO
{
	public ulong 	AccountId;
	public string 	CharName;
}
