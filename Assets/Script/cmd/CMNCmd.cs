using UnityEngine;
using System.Collections;


namespace CMNCMD {
	
	enum MB_WARN_ERR
	{
		//未知错误 
		UNKNOWN_ERR				=0,
		//网络断开，请检查网络是否连接 
		NET_ERR					=1,
		//创建账号失败:  
		CREATE_ACCOUNT_ERR 		=2,
		//创建角色失败:  
		CREATE_CHAR_ERR			=3,
		//角色名字重复  
		CHAR_NAME_DUPLICATE		=4,
		//账号不存在  
		NO_ACCOUNT				=5,
		//密码错误  
		PASS_WRONG				=6,
		//玩家已在线  
		PLAYER_ALREADY_ONLINE	=7,
		//角色为空 请创建角色 
		NO_CHAR					=8,
		//请输出账号  
		ACCOUNT_EMPUTY			=9,
		//名字超长  
		NAME_TOO_LONG			=10,
		//金币不足 
		MONEY_NOT_ENOUGH 		=11,
		//居民不足  
		PEOPLE_NOT_ENOUGH 		=12,
		//登陆失败 
		LOGIN_ERR		 		=13,
		//兵营建造成功 
		BINGYING_BUILD_OK 		=14,
		//兵营升级成功 
		BINGYING_UPGRADE_OK		=15,
		//民居建造成功 
		MINJU_BUILD_OK 			=16,
		//民居升级成功 
		MINJU_UPGRADE_OK		=17,
		//建筑等级已达上限  
		BUILD_LEVEL_LIMIT		=18,
		//资源载入出错   
		LOAD_MB_ERR				=19,
		//水晶不足    
		CRYSTAL_NOT_ENOUGH		=20,
		//钻石不足
		DIAMOND_NOT_ENOUGH		=21,
		//未选择购买商品 
		GOODS_NOT_SELECT		=22,
		//未选择物品				
		ITEM_NOT_SELECT			=23,
		//购买成功 
		STORE_BUY_OK			=24,
		//一件装备只能镶嵌同类宝石一颗  
		HAVE_EMBED_THISGEM		=25,
		//建筑队列已满 
		BUILD_QUE_ENOUGH		=26,
		//该建筑数量已达到上限  
		BUILDING_NUM_MAX		=27,
		//申请好友成功,等候对方答复 
		ADD_FRIEND_OK			=28,
		//你和对方已经是好友 
		YOU_HAVE_FRIENDS		=29,
		//已经有研究中的科技  
		HAS_RESEARCH_TECH		=30,
		//联盟名字不能为空   
		ALLIANCE_NAME_IS_NULL	=31,
		//联盟名字重复   
		ALLIANCE_NAME_DUPLICATE	=32,
		//联盟不存在 
		ALLIANCE_IS_NO_EXIST	=33,
		//职位已存在  
		POSITION_IS_EXIST		=34,
		//联盟中没有此账号   
		THIS_ACCOUNT_NOTIN_ALLIANCE		=35,
		//数量不足 
		NUM_NOT_ENOUGH			= 36,
		//使用成功 
		USE_SUCCESS				= 37,
		//草药 
		CAOYAO_NOT_ENOUGH		= 38,
		//医馆空间不足  
		YIGUAN_LACK_SPACE		= 39,
		//收获水晶  
		RESULT_CRYSTAL			= 40,
		//收获道具   
		RESULT_ITEM				= 41,
		//捐献成功 
		CONTRIBUTE_OK			= 42,
		//贡献度不足 
		CONTRIBUTE_NOT_ENOUGH	= 43,
		//当前武将名额已满 
		HERO_IS_FULL			= 44,
		//当前武将还穿着装备 
		HERO_IS_EQUIPED			= 45,	
		//当前武将还带有士兵 
		HERO_HAS_SOLDIER		= 46,	
		//当前武将处于城防状态 
		HERO_IS_DEFENSE			= 47,	
		//当前武将处于出征状态  
		HERO_IS_FIGHTING		= 48,
		//当前科技已达上限  
		TECH_IS_TOP				= 49,	
		//PVP掠夺  
		PVP_WIN_RESULT			= 52,
		//PVP被掠夺   
		PVP_LOSE_RESULT			= 53,
		//PVE通关收获    
		PVE_WIN_RESULT			= 54,
		//请输入正确数字     
		INPUT_IS_WRONG			= 55,
		//签名超长  
		SIGNATURE_TOO_LONG		= 56,
		//修改成功 
		MODIFY_SUCCESS			= 57,
		//武将攻击 
		HERO_ATTACK				= 58,
		//武将防御 
		HERO_DEFENSE			= 59,
		//武将生命 
		HERO_LIFE				= 60,
		//武将统帅 
		HERO_LEADER				= 61,
		//宝石镶嵌 
		GEM_EMBED				= 62,
		//居民生产:{0}/小时 
		JUMIN_PRODUCT			= 63,
		//民居容量:{0}/{1} 
		MINJU_CAPACITY			= 64,
		//兵营容量:{0}/{1} 
		BINGYING_CAPACITY		= 65,
		//聊天[世界]频道 
		CHAT_WORLD				= 66,
		//聊天[联盟]频道 
		CHAT_ALLIANCE			= 67,
		//聊天[队伍]频道 
		CHAT_TEAM				= 68,
		//聊天[私聊]频道
		CHAT_PRIVATE			= 69,
		//君主达到{0}级 可携带{1}武将
		LORD_NEX_HERO_NUM		= 70,
		//镶嵌宝石的装备不能出售  
		EMBED_EQUIP_NOT_SELL	= 71,
		//缺少道具英雄榜 可在商城购买 
		LACK_YINGXIONGBANG		= 72,
		//单位（万） 
		TEN_THOUSAND			= 73,
		//购买钻石描述 
		RECHARGE_DESCRIBE		= 74,
		//购买钻石价格  
		RECHARGE_PRICE_91			= 75,
		//剩余时间 
		TIME_REMAINING 			= 76,
		//将领等级已达上限 请先提高君主等级 
		HERO_TO_TOP_LEVEL		= 77,	
		//将领成长率已达上限 
		HERO_TO_TOP_GROW		= 78,	
		//修改名字需花费钻石{0} 
		MODIFY_NAME_SPEND		= 79,	
		//获得奖励如下: 
		GET_REWARD_LIST			= 80,	
		//今日联盟抽奖次数已达上限 
		ALLIANCE_LOTTERY_ISFULL	= 81,
		//每日可领取{0}次免费抽奖机会
		FREE_LOTTERY_NUM		= 82,	
		//查找不到此玩家 
		NO_THIS_PALYER			= 83,
		//招募名将消耗道具英雄榜 可招募到高成长率名将 
		ZHAOMU_MINGJIANG		= 84,
		//确认删除当前类别的所有邮件么 
		SRUE_DELETE_ALL_MAIL	= 85,
		//确认把当前类别的所有邮件设置为已读么 
		SRUE_READ_ALL_MAIL		= 86,
		//确认删除当前邮件?  
		SRUE_DELETE_THIS_MAIL	= 87,
		//确认出售当前物品? 
		SRUE_SELL_THIS_ITEM		= 88,
		//确认解雇武将? 
		SRUE_DISMISS_HERO		= 89,
		//确认删除好友? 
		SRUE_DELETE_FRIEND		= 90,
		//确认删除仇人?
		SRUE_DELETE_ENEMY		= 91,
		//当前建筑数量{0}个 君主等级达到{1}级可建造{2}个 
		BUILDING_NUM_DESCRIBE 	= 92,
		//民居空间不足，人口卷轴不能最大发挥作用，您确定使用么? 
		SRUE_USE_RENKOUJUANZHOU	= 93,
		//君主已满级，不能使用经验秘药
		JUNZHU_TOP_LEVEL_NOT_ADD_EXP = 94,
		//充值成功，获得钻石{0}，赠送{1} 
		RECHARGE_SUCCESS		= 95,
		//掠夺金币{0} 居民{1}(因民居容量已满，实际获得居民{2})
		PVP_WIN_RESULT_NOT_FULL	= 96,
		//含有非法字符，请重新输入 
		BAD_NAME				= 97,
		//普通战役({0}关) 
		WANGZHE_NORMAL			= 98,
		//史诗战役({0}关) 
		WANGZHE_HIGHER			= 99,
		//武将不在空闲状态  
		HERO_NOT_FREE			= 101,
		//网络请求超时 ...		
		NET_REQ_TIMEOUT 		= 150,
		//网络繁忙,充值失败 ...
		RECHARGE_TIMEOUT		= 151,
		//将领在副本中 
		HERO_IN_CAMPAIN			= 153,
		//将领在世界金矿中 
		HERO_IN_WORLDMINE		= 154,
		//没有AdColony视频广告可看  
		NO_ADCOLONY_SHOW		= 155,	
		//解散士兵 
		SURE_DISMISS_TROOPS	 	= 156,   
		//账号已经存在  
		ACCOUNT_NAME_EXISTS		= 157,
		//价格必须介于1-100 
		GOLD_PRICE_NUM_LIMIT    = 158,
		//金币销售数量必须大于0 
		GOLD_SELL_NUM_LOWER 	= 159,
		//金币销售数量超上限 
		GOLD_SELL_NUM_TOP       = 160,	
		//君主等级太低比不能寄卖 
		LORD_LV_LOW_FOR_SALE	= 161,
		//同一时间只能有一笔寄卖 
		ONLY_ONE_SALE_SAMETIME  = 162,
		//金币已经被别人购买 
		GOLD_BOUGHT_BY_OTHERS	= 163,	
		//金币交易说明 
		GOLD_TRADE_DESC   	    = 164,
		//不能重复使用工匠之书 
		SMITHBOOK_STACK			= 165,
		//处于保护中 不能使用免战牌 
		IN_PROTECT_NO_TRUCE_TOKEN = 166,
		//联盟下一等级对应增兵数量 
		NEXT_GUILD_REINFORCE_NUM  = 167,
		//撤销交易的间隔太短 
		TRADE_INTERVAL_IS_TOO_SHORT = 168,
		//行军过程不能使用免战 
		MARCH_NOT_USE_TRUCE     = 169,
		//英雄达不到该装备的等级需求 
		EQUIP_LIMIT_HERO_LEVEL	= 170,
		//盟友的接受增援的名额已满 
		ALLY_BARRACK_CAP_FULL   = 171,
		//君主等级到达标准才能购买黄金 
		LORD_LEVEL_LIMIT_BUY_GOLD = 172,
		//设备已经绑定到账号 
		DEVICE_BOUND_ACCOUNT	 = 173,	
		//世界资源描述 
		WORLD_RES_DESC			 = 174,	
		//联盟增兵君主等级限制  
		REINFORCE_LEVEL_LIMIT	 = 175,	
		//攻击目标的等级差不能超过限制 
		ATTACK_LEVEL_LIMIT		 = 176,
		//上传镜像前必须先部署部队 
		MUST_DEPLOY_FIRST        = 177,	
		//竞技场奖励描述 
	 	ARENA_REWARD_INTRO		 = 178,	
		//竞技场挑战次数用光 
		ARENA_TRY_USEDUP		 = 179,
		//你不能挑战比你排行低的君主 
		CAN_NOT_CHALLENGE_LOW    = 180,
		//你的上传时间在CD中 
		ARENA_UPLOAD_CD          = 181,
		//参加竞技必须先上传镜像 
		ARENA_MUST_UPLOAD_FIRST  = 182,
		//君主多少级才能参加竞技场 
		ARENA_LORD_LEVEL_LIMIT	 = 183,
		//任然拥有竞技场挑战的次数 
		STILL_HAVE_CHALLENGE_TRY = 184,	
		//收藏成功 
		COLLECT_SUCCESS			= 185,
		//收藏夹已满 
		FAVORITE_IS_FULL		= 186,
		//增援 
		REINFORCE_TITLE			= 332,
		//撤回增援 
		RECALL_REINFORCEMENTS	= 333,
		//增援数量已达上限 
		REINFORCE_MAX_NUM		= 334,
		//增援队列已满 
		REINFORCE_QUEUE_MAX     = 335,
		//使用英雄卷轴提示 
		USE_HEROLIST_PROMPT		= 338,
		//我们的总战力 
		OUR_TOTAL_FORCE			= 339,
		//敌人的总战力 
		ENEMY_TOTAL_FORCE		= 340,
		//君主战力 
		LORD_FORCE				= 341,
		//百战不殆 
		HERO_BAIZHAN			= 402,	
		//南征北战 
		HERO_NANZHENG			= 403, 
		//空闲 
		HERO_FREE				= 418,
		//出征 
		HERO_IN_BATTLE			= 419,
		//修炼 
		HERO_IN_TRAIN			= 420,
		//联盟副本 
		GUILD_WAR				= 454,
        //没有参加团队副本 
        NOT_IN_GB_TEAM          = 500,
        //团队副本不存在 
        GB_TEAM_NOT_EXIST       = 501,
        //团队副本已满
        GB_TEAM_FULL            = 502,
        //有团队副本未退出  
        GB_TEAM_NOT_EXIT        = 503,
        //有人尚未准备成功  
        GB_TEAM_NOT_READY       = 504,
        //至少要t一个人
        GB_TEAM_NOBODY_KICK     = 505,
        //副本战斗中 
        GB_TEAM_COMBAT_GOING    = 506,
        //至少分配一个将领
        GB_DEPLOY_ONE_HERO      = 507,
        //退出退伍提示 
        GB_QUIT_TEAM            = 508,
        //团队副本次数已经用完 
        GB_TIMES_USEDUP         = 509,
        //重试次数已经用完
        GB_RETRY_TIMES_USEDUP   = 510,
		//金矿VIP成熟保护器 
		GOLD_VIP_PROTECT		= 600,
		//普通玩家每日发送邮件上限100，今日数量已满(VIP可扩充邮件上限) 
		SEND_MAIL_FULL			= 601,
		//对方邮箱已满，发送失败 
		RECEIVER_MAIL_FULL		= 602,
		//联盟成员（联盟邮件不受邮件发送数量限制） 
		ALLIANCE_MAIL_EXPLAIN	= 603,
		//604	个人消息数量已达上限，请及时清理 
		PRIVATE_MAIL_IS_FULL	= 604,
		//好友数量已达上限，可升级VIP 
		FRIEND_NUM_FULL			= 605,
		//对方好友数量已达上限 
		OTHER_FRIEND_NUM_FULL	= 606,
		//您目前充值总数为{0}(不含赠送)，继续充值{1}钻石即可升级VIP{3} 
		RECHARGE_VIP_DESCRIBE	= 607,
		//目前VIP最高级别为5，更高权限的VIP即将开放，感谢您对【口袋帝国】的支持 
		VIP_IS_TOP				= 608,
		//确认撤销联盟申请? 
		SURE_CANCEL_ALLIANCE_APPLY = 609,
		//确认退出联盟? 
		SURE_QUIT_ALLIANCE		= 610,
		//确认解散联盟? 
		SURE_DISMISS_ALLIANCE	= 611,
		//确认开除此成员? 
		SURE_FIRE_THIS_MEMBER	= 612,
		//联盟成员已满员 
		ALLIANCE_MEMBER_FULL	= 613,
		//我方：{0}战力 
		BATTLE_LOG_MY_TITLE		= 614,
		//敌方：{0}战力 
		BATTLE_LOG_ENEMY_TITLE	= 615,
		//骑兵(克制 剑士、器械) 
		QIBING_DESCRIBE			= 616,
		//枪兵(克制 骑兵、剑士) 
		QIANGBING_DESCRIBE		= 617,
		//剑士(克制 弓兵、器械) 
		JIANSHI_DESCRIBE		= 618,
		//弓兵(克制 骑兵、枪兵) 
		KONGBING_DESCRIBE		= 619,
		//器械(克制 枪兵、弓兵) 
		QIXIE_DESCRIBE			= 620,
		//本大区已满员，请选择其他大区注册 
		SERVER_IS_FULL			= 621,
		//本次战争结束时间： 
		HONOR_BATTLE_OVER_TIME	= 622,
		//荣耀之战状态和圣杯数量 
		HONOR_BATTLE_STATUS		= 623,
		//正常 
		NORMAL_STATUS			= 624,
		//恢复 
		RECOVER_STATUS			= 625,
		//已达君主等级上限 
		HERO_TOP_LEVEL			= 626,
		//宝石等级已达上限 
		GEM_LEVEL_TOP			= 627,
		//荣耀之城刷新时间 
		HONOR_CITY_REFRESH_TIME	= 628,
		//掠夺圣杯{0}，金币{1}，居民{2} 
		PVP_WIN_CUP				= 629,
		//掠夺圣杯{0}，金币{1}，居民{2}（因民居容量已满，实际获得居民{3} 
		PVP_WIN_CUP_FULL		= 630,
		//被掠夺圣杯{0}，金币{0}，居民{1}
		PVP_LOSE_CUP			= 631,
		//您处于荣耀之战状态(拥有圣杯)，无法开启免战 
		MIANZHAN_DISABLE_FOR_HONOR=632,
		//钻石RMB价格（appstore）
		RECHARGE_PRICE_APPLE    = 633,
		//天 
		CHINESE_DAY				= 634,		
		//荣耀之战结束 
		HONOR_BATTLE_OVER		= 635,
		//名字不能为空 
		NAME_IS_NULL			= 636,
		//邮箱格式不正确 
		MAIL_ADDRESS_WRONG		= 637,
		//密码只能为数字或字母 
		PASS_ISONLY_NUM_LETTER	= 638,
		//密码长度不足 
		PASS_TOO_SHORT			= 639,
		//密码长度超长 
		PASS_TOO_LONG			= 640,
		//密码两次输入不一致 
		PASS_NOT_SAME			= 641,
		//邮箱账号名超长 
		MAIL_ACCOUNT_TOO_LONG	= 642,	
		//旧密码错误 
		OLD_PASS_WRONG			= 643,
		 //注册邮箱账号成功 
		REGISTER_MAIL_ACCOUNT_OK= 644,
		//修改密码成功 
		MODIFY_PASS_OK			= 645,
		//绑定设备成功 
		BAND_DEVICE_OK			= 646,	
		//该邮箱已被注册 
		THE_MAIL_REGISTERED		= 647,
		// 701	金矿可以收获了 
		ALERT_MSG_GOLD_IS_READY	= 701,
		// 702	摇钱树成熟了 
		ALERT_MSG_TREE_IS_READY	= 702,
		// 703	建筑升级完毕 
		ALERT_MSG_BUILDING_IS_READY	= 703,
		// 704	科技升级完毕 
		ALERT_MSG_TECH_IS_READY	= 704,
		// 705 摇钱树可以浇水了 
		ALERT_MSG_TREE_WAIT_WATER	= 705,
		// 设置解锁条上的文字  
		// 709	返回帝国   
		ALERT_MSG_ON_LOCK_BAR	= 709,
		// 710 恭请君主临朝 
		ALERT_MSG_1DAY_NO_SEE	= 710,
		// 711	最近战事跌起，将领们都在等候您的召唤
		ALERT_MSG_2DAY_NO_SEE	= 711,
		// 712	望您在百忙之中照看一下您的口袋帝国，您的臣民想念您
		ALERT_MSG_7DAY_NO_SEE	= 712,
		//钻石 
		DIAMOND					= 1000,
		//水晶 
		CRYSTAL					= 1001,
		//金币 
		GOLD					= 1002,
		//人口  
		POP 					= 1003,
	}
	enum CMN_DEF 
	{
		//联盟增兵君主等级限制  
		REINFORCE_LEVEL_LIMIT   = 21,
		//是否开启联盟增兵 
		GUILD_REINFORCE			= 28,
		//是否开启金币交易市场 
		GOLD_TRADE_MARKET		= 29,
		//交易市场卖方最低等级 
		SELL_GOLD_LEVEL_LIMIT	= 22,
		//交易市场买方最低等级 
		BUY_GOLD_LEVEL_LIMIT	= 33,
		//是否在游戏登陆界面弹出playhaven的广告 
		PLAYHAVEN_LOGIN			= 34,
		//差距多少级不能攻击对方 
		PROTECT_LEVEL_GAP   	= 35,
		//首页是否开启ADMOB 
		ADMOB_LOGIN				= 36,	
		//是否开启竞技场 
		HERO_ARENA				= 37,
		//是否开启联盟副本 
		GUILD_WAR				= 38,
	}
	enum MSG_HEAD 
	{
		MSG_EXCHANGE_KEY		= 1,
		MSG_KA					= 2,
		MSG_CLOSE				= 3,
		MSG_DATA				= 4,
	}
	enum SUBCMD
	{
		CLIENT_CLS4Web_DATA = 102,
		CLS4Web_CLIENT_DATA = 112,
	}

	public struct  CMN_MSG_HEAD_T{
		 //包的大小
		public int 			nSize1;
		//连接句柄
		public int			CntrID2;
	}
	public struct CMN_MSG_BASE_T
	{
		//大包种类
		public byte  		Cmd1;
	}
	public struct CMN_MSG_SUBCMD_T
	{
		public byte  		SubCmd1;
	}
	public struct  CMN_MSG_EXCHANGEKEY_T{
		public int			CryptType1;
		public int			KATimeout2;
		public int 			Len3;	
		public byte[]		szKey4;
	}
	public struct  CMN_MSG_CAAFS_EXCHANGEKEY_T{
		public int			CryptType1;
		public int			KATimeout2;
		public int 			Len3;	
		public byte[]		szKey4;
		public int 			TermType5;
	}
	public struct  CMN_MSG_CLS_EXCHANGEKEY_T{
		public int				CryptType1;
		public int				KATimeout2;
		public int 			Len3;	
		public byte[]		szKey4;
		public int 			ConID5;
		public int 			Password6;
	}
	//内部或者外部版本  
	public enum VERSION_TYPE
	{
		//内部版本  
		VERSION_INNER	= 0x01,
		//苹果官方 
		VERSION_APPSTORE = 0x02,
	}
	//APP版本 
	public enum APP_VERSION_TYPE
	{
		//TinyKingdom版本  
		TINYKINGDOM			= 0x01,

	
	}
	//设备种类  
	public enum DEVICE_TYPE
	{
		//iphone TOUCH 
		IPHONE			= 0x01,
		//ipad 
 		IPAD			= 0x02,
	}
	//IPHONE种类 
	public enum IPHONE_TYPE
	{
		//iphone4 3 2 1 
		IPHONE			= 0x01,
		//iphone5 touch5 
		IPHONE5			= 0x02,
		
	}
	enum BuildingSort
	{
		// 城堡 
		CHENGBAO		= 0x01,
		// 科技研究院 
		YANJIUYUAN		= 0x02,
		// 酒馆 
		JIUGUAN			= 0x03,
		// 修炼馆 
		XIULIAN 		= 0x04,
		// 大使馆  
		DASHIGUAN		= 0x05,
		//医馆 
		YIGUAN			= 0x06,
		// 铁匠铺 
		TIEJIANGPU		= 0x07,
		// 市场 
		SHICHANG		= 0x08,
		// 民居  
		MINJU	 		= 0x09,
		// 兵营 
		BINGYING		= 0x0a,
		//金矿 
		JINKUANG		= 0x0b,
		//许愿树 
		XUYUANSHU		= 0x0c,
	}
	enum BuildingState
	{
		//正常状态 
		NORMAL			= 0x00,
		//建造状态 
		BUILDING		= 0x01,
		//升级状态 
		UPGRADING		= 0x02,
	
	}
	enum ProduceState
	{
		//正常状态 
		NORMAL			= 0x00,
		//生产状态 
		PRODUCE			= 0x01,
		//收获状态 		
		RESUIT			= 0x02,
	}
	//将领 
	public enum HeroState
	{
		//空闲状态 
		NORMAL							= 0,
		// 普通战斗，讨伐 
		COMBAT_PVE_RAID					= 1,	
		// 玩家对战，攻方 
		COMBAT_PVP_ATTACK				= 2,
		//世界名城 
		HONOR_CITY_ATTACK				= 20,
		// 副本，百战不殆 
		COMBAT_INSTANCE_BAIZHANBUDAI	= 100,	
		// 副本，南征北战 
		COMBAT_INSTANCE_NANZHENGBEIZHAN	= 101,	
		//联盟副本 
		COMBAT_INSTANCE_GUILD           = 102,	
		// 将领在武将馆修炼 
		COMBAT_HERO_TRAINING			= 1000,		
		// 将领在攻打世界资源  
		COMBAT_HERO_WORLD_RESOURCE		= 1001,		
	}


	//士兵种类 
	public enum SoldierSort
	{
		//骑兵  
		QIBING			= 0x01,
		//枪兵  
		QIANGBING		= 0x02,
		// 剑士   
		JIANSHI			= 0x03,
		//弓兵   
		GONGBING 		= 0x04,
		// 器械   
		QIXIE 			= 0x05,
	}
	//物品大类 
	enum GoodsSort
	{
		mail_reward_type_none				= 0,			// 无奖励 
		mail_reward_type_item				= 1,			// 道具 
		mail_reward_type_diamond			= 2,			// 钻石 
		mail_reward_type_crystal			= 3,			// 水晶 
		mail_reward_type_gold				= 4,			// 黄金 
	}
	//道具种类 
	enum ItemSort
	{
		ALLITEM			= 0x00,
		//装备 
		EQUIP			= 0x01,
		//宝石 
		GEMSTONE		= 0x02,
		//用品 
		ARTICLES		= 0x03,
		//任务道具 
		TASKITEM		= 0x04,
	}
	//装备部位 
	enum EquipSlot
	{
		//头盔 
		HEAD			= 0x01,
		//护甲 
		ARMOR			= 0x02,
		//鞋子 
		SHOE			= 0x03,
		//武器 
		ARM				= 0x04,
		//饰品 
		JEWELERY		= 0x05,
	}
	//宝石种类 
	enum GemSort
	{
		//攻击  
		ATTACK			= 0x01,
		//防御   
		DEFENSE			= 0x02,
		//生命  
		LIFE			= 0x03,
		//统帅  
		LEADER			= 0x04,	
	}
	//用品道具 
	enum ARTICLES
	{
		//迁城卷轴 
		QIANCHENGJUANZHOU 	= 2004,
		//免战牌(老的 已经作废)  
		MIANZHANPAI			= 2005,
		//英雄经验秘药
		YINGXIONGJINGYAN 	= 2006,
		//君主经验秘药 
		JUNZHUJINGYAN		= 2007,
		//英雄成长秘药 
		YINGXIONGCHENGZHANG = 2009,
		//英雄榜 
		YINGXIONGBANG 		= 2010,
		//工匠之书(老的 已经作废) 
		SMITHBOOK			= 2012,
		//人口卷轴 
		RENKOUJUANZHOU		= 2013,
		//草药 
		CAOYAO	   			= 2014, 
		//草药包  
		CAOYAOBAO   		= 2015, 
		//征战卷轴 
		ZHENGZHANJUANZHOU	= 2016,	
		//新手礼包 
		XINSHOULIBAO 		= 2017,
		//24小时工匠之书  
		HOUR24_SMITHBOOK	= 2030,
		//48小时工匠之书  
		HOUR48_SMITHBOOK	= 2031,
		//7天工匠之书 
		DAY7_SMITHBOOK		= 2032,
		//24小时免战牌  
		HOUR24_TRUCETOKEN 	= 2080,
		//10小时免战牌  
		HOUR10_TRUCETOKEN 	= 2081,
		//金币 
		JINBI				= 100000,
	}
	//宝石镶嵌位置 
	enum GemSlot
	{
		SLOT1			= 0x01,
		SLOT2			= 0x02, 
		SLOT3			= 0x03,
		SLOT4			= 0x04,	
	}
	//商城标签分类 
	enum STORETAB
	{
		//装备  
		EQUIP			= 0x01,
		//宝石    
		GEM				= 0x02,
	
	}
	//使用道具返回更新的数据类型 
	enum UPDATETYPE
	{
		SYNC_TYPE_HERO					= 1,						// 武将 
		SYNC_TYPE_CHAR					= 2,						// 君主 
		SYNC_TYPE_ITEM					= 3,						// 道具 
		SYNC_TYPE_DRUG					= 4,						// 草药包 
	}
	//聊天频道 
	enum CHATCHANNEL
	{
		ALL_CHANNEL						= 1,						// 全部频道 
		WORLD_CHANNEL					= 2,						// 世界频道 
		ALLIANCE_CHANNEL					= 3,						// 联盟频道
		PRIVATE_CHANNEL					=4,							// 私聊频道
		TEAM_CHANNEL					= 5,						// 队伍频道 
	}
	// time_event 大类  
	enum  time_event
	{
		te_type_building				= 1,		// 建筑时间事件  
		te_type_once_perperson			= 2,		// 每个瞬间每个人只能有一次的时间事件(如许愿树、人口增长)  
		te_type_production				= 3,		// 生产事件，目前只有金矿生产，需要记录产量  
		te_type_research				= 4,		// 研究  
		te_type_alliance_building		= 5,		// 联盟建设  
	}

	enum money_type
	{
		money_type_none				= 0,		// 无类型  
		money_type_diamond			= 1,		// 钻石  
		money_type_crystal			= 2,		// 水晶  
		money_type_gold				= 3,		// 黄金  
		money_type_alliance_contribute	= 4,	// 联盟贡献度抽奖   
	}
	enum MAIL_TYPE
	{
		MAIL_TYPE_PRIVATE		= 1,		// 私人邮件 
		MAIL_TYPE_ALLIANCE		= 2,		// 联盟邮件 
		MAIL_TYPE_SYSTEM		= 4,		// 系统邮件 
		MAIL_TYPE_ALLIANCE_EVENT	= 8,		// 联盟事件 
		MAIL_TYPE_GOLDORE_EVENT		= 16,		// 金矿事件,发送者是偷金者或者浇水者 
		MAIL_TYPE_ANNOUNCE		= 100,
		MAIL_TYPE_GM			= 110,		//gm邮件 
	}
	enum MAIL_FLAG
	{
		MAIL_FLAG_NORMAL		= 1,		// 普通邮件 
		MAIL_FLAG_FIGHT			= 2,		// 附加信息为战斗 
		MAIL_FLAG_REWARD		= 3,		// 奖励，附件为奖励 
		MAIL_FLAG_NEWFIGHT		= 4,		// 新的战斗附件 
	}
	
	// te_type_once_perperson,一人一次时间事件的子类   
	enum te_type_once_person
	{ 
		te_subtype_once_christmas_tree	= 1,		// 圣诞树成熟事件   
		te_subtype_once_population		= 2,		// 人口增长事件   
	}
	// te_type_production,生产事件子类  
	enum te_type_production
	{
		te_subtype_production_gold		= 1,		// 黄金生产 
	}
	enum te_type_building
	{
		te_subtype_building_build		= 1,		// 建造 
		te_subtype_building_upgrade		= 2,		// 升级 
	}
	// te_type_alliance_build,联盟建造事件 
	enum te_type_alliance_build
	{
		te_subtype_alliance_building_build		= 1,		// 建造  
		te_subtype_alliance_building_upgrade	= 2,		// 升级  
	}
	//加速种类  
	enum enum_accelerate_type
	{
		building		= 1,		// 建筑  
		gold			= 2,		// 黄金生产  
		research		= 3,		// 研究 
	}
	//联盟职位 
	enum ALLIANCE_POSITION
	{
		alliance_position_none			= 0,	// 不是该联盟的 
		alliance_position_leader		= 1,	// 盟主 
		alliance_position_vice_leader	= 2,	// 副盟主 
		alliance_position_manager		= 3,	// 管事 
		alliance_position_member		= 4,	// 成员 
	} 
	//摇钱树类别（自己的还是别人的） 
	enum YAOQIANSHU_CATEGORY
	{
		mine			= 1,	// 自己的 
		others			= 2,	// 别人的  
	}
	public enum RANK_TYPE
	{
		rank_type_char_level						= 1,		// 角色等级 
		rank_type_char_gold							= 2,		// 角色黄金数量 
		rank_type_char_diamond						= 3,		// 角色钻石数量 
		rank_type_instance_wangzhe					= 4,		// 王者之路排行 
		max_rank_query_num							= 50,		// 一次性查询的排行榜数据量最大值 
	}
	//战斗类型 
	public enum COMBAT_TYPE
	{
		NONE				= 0,
		COMBAT_PVE 			= 1,
		COMBAT_PVP_ATTACK	= 2,
		COMBAT_PVP_DEFENSE 	= 3,
		COMBAT_GOLD 		= 10,
		COMBAT_WANGZHEZHILU = 100,
		COMBAT_ZHENGZHANTIANXIA = 101,
		//竞技场 			
		COMBAT_ARENA					= 200,
		//联盟副本 
		COMBAT_INSTANCE_GUILD           = 102,
	}
	//主界面提醒种类 
	public enum REMIND_TYPE
	{
		NO_REMIND				= 0, 	//没有提醒 
		ANNOUNCE_REMIND			= 1,	//通知提醒  
		FRIEND_APPLY			= 2,	//好友申请  
		GOLD_RIPE				= 3,	//金矿成熟  
		ALLIANCE_APPLY			= 4,	//联盟申请   
	}
	//修改名字的类型 
	public enum MODIFY_NAME_TYPE
	{
		LORD_NAME = 1,		//君主名字 
		HERO_NAME = 2,		//将领名字 
	}
	//当前播放的背景音乐 
	public enum BG_MUSIC
	{
		NONE			= 0,
		LOGIN_MUSIC		= 1, //登陆背景音乐 
		MAINSCENE_MUSIC = 2, //主场景音乐 
		BATTLE_MUSIC 	= 3, //战斗音乐 
	}
	//帮助说明的类型 
	public enum HELP_TYPE
	{
		//兵种说明 
		SOLDIER_SORT_HELP = 1,
		//金矿说明 
		GOLD_HELP		  = 2,	
		//VIP说明 
		VIP_HELP		  = 3,
		//荣耀之战说明 
		HONOR_BATTLE	  = 4,
	}
	//城防信息 
	public enum CITY_DEFENSE_AUTOSUPPLY
	{
		//城防自动补兵开启  
		OPEN = 1,
		//城防自动补兵关闭 
		CLOSE = 0,
	}
	//世界资源种类 
	public enum WORLD_RESOURCE
	{
		//金矿 
		GOLDMINE = 1,
		//民居 
		COTTAGE = 2
	}
	//排兵布阵 拖拽单元还是拖拽列 
	public enum DRAG_TYPE
	{
		//无法拖拽 
		NONE  = -1,
		//单元 
		UNIT = 1,
		//列 
		COLUMN = 2
	}
}
