using UnityEngine;
using System.Collections;
namespace CTSCMD
{
	enum CTS_MSG
	{
		// 登录指令 
		TTY_CLIENT_LPGAMEPLAY_LOGIN				= 0x03,
		// 创建角色 
		TTY_CLIENT_LPGAMEPLAY_CREATECHAR		= 0x04,
		// 创建账号 
		TTY_CLIENT_LPGAMEPLAY_CREATEACCOUNT		= 0x05,
		// 游戏中使用的指令 
		TTY_CLIENT_LPGAMEPLAY_GAME_CMD			= 0x06,
		// 登出 
		TTY_CLIENT_LPGAMEPLAY_LOGOUT			= 0x07,
		// 通过设备ID登陆 
		TTY_CLIENT_LPGAMEPLAY_LOGIN_BY_DEVICEID	= 0x08, 
	}
	enum CTS_CHAR_CMD
	{
		TTY_CHARCMD_GETALLCHARINFO_SMP				= 0x01,	
		TTY_CHARCMD_SELECT							= 0x04,	
	}
	enum SUB_CTS_MSG
	{
		
		//////////////////////////////////////////////////////////////////////////
		// 聊天相关 
		////////////////////////////////////////////////////////////////////////// 
		CTS_GAMECMD_CHAT_BEGIN					= 0,
		CTS_GAMECMD_CHAT_PRIVATE				= CTS_GAMECMD_CHAT_BEGIN + 1,		// 私聊 
		CTS_GAMECMD_CHAT_ALLIANCE				= CTS_GAMECMD_CHAT_BEGIN + 2,		// 联盟 
		CTS_GAMECMD_CHAT_WORLD					= CTS_GAMECMD_CHAT_BEGIN + 3,		// 世界 
		CTS_GAMECMD_CHAT_GET_RECENT_CHAT		= CTS_GAMECMD_CHAT_BEGIN + 4,		// 获得最近的聊天记录 
		CTS_GAMECMD_CHAT_GROUP					= CTS_GAMECMD_CHAT_BEGIN + 5,		// 组队(多人副本) 
		CTS_GAMECMD_CHAT_END					= CTS_GAMECMD_CHAT_BEGIN + 20, 
	
		////////////////////////////////////////////////////////////////////////// 
		// 查询相关 
		////////////////////////////////////////////////////////////////////////// 
		CTS_GAMECMD_GET_BEGIN					= CTS_GAMECMD_CHAT_END,
		CTS_GAMECMD_GET_NOTIFICATION			= CTS_GAMECMD_GET_BEGIN + 1,		// 查看通知 
		CTS_GAMECMD_GET_TERRAIN					= CTS_GAMECMD_GET_BEGIN	+ 2,		// 查询附近地理信息 
		CTS_GAMECMD_GET_PLAYERCARD				= CTS_GAMECMD_GET_BEGIN + 3,		// 根据accountID获取某个玩家的名片 
		CTS_GAMECMD_GET_PLAYERCARD_POS			= CTS_GAMECMD_GET_BEGIN	+ 4,		// 根据位置获取PlayerCard 
		CTS_GAMECMD_GET_TILE_INFO				= CTS_GAMECMD_GET_BEGIN + 7,		// 获取地图tile信息 
		CTS_GAMECMD_GET_BUILDING_LIST			= CTS_GAMECMD_GET_BEGIN + 9,		// 获取建筑列表 
		CTS_GAMECMD_GET_BUILDING_TE_LIST		= CTS_GAMECMD_GET_BEGIN + 10,		// 获取建筑时间事件列表 
		CTS_GAMECMD_GET_PRODUCTION_EVENT		= CTS_GAMECMD_GET_BEGIN + 11,		// 获取生产事件(就是结果) 
		CTS_GAMECMD_GET_PRODUCTION_TE_LIST		= CTS_GAMECMD_GET_BEGIN + 12,		// 获取生产事件事件 
		CTS_GAMECMD_GET_TECHNOLOGY				= CTS_GAMECMD_GET_BEGIN + 13,		// 获取科技 
		CTS_GAMECMD_GET_RESEARCH_TE				= CTS_GAMECMD_GET_BEGIN + 14,		// 获取科技研究队列 
		CTS_GAMECMD_GET_SOLDIER					= CTS_GAMECMD_GET_BEGIN + 15,		// 获取所有军队 
		CTS_GAMECMD_GET_ALLIANCE_INFO			= CTS_GAMECMD_GET_BEGIN + 16,		// 获取联盟基本信息 
		CTS_GAMECMD_GET_ALLIANCE_MEMBER			= CTS_GAMECMD_GET_BEGIN + 17,		// 获取联盟成员列表  
		CTS_GAMECMD_GET_ALLIANCE_BUILDING_TE	= CTS_GAMECMD_GET_BEGIN + 18,		// 获取联盟建筑时间事件列表  
		CTS_GAMECMD_GET_ALLIANCE_BUILDING		= CTS_GAMECMD_GET_BEGIN + 19,		// 获取联盟建筑列表  
		CTS_GAMECMD_GET_ALLIANCE_JOIN_EVENT		= CTS_GAMECMD_GET_BEGIN + 20,		// 获取联盟申请成员列表  
		CTS_GAMECMD_GET_CHAR_ATB				= CTS_GAMECMD_GET_BEGIN + 21,		// 获取角色基本数据   
		CTS_GAMECMD_GET_MY_FRIEND_APPLY			= CTS_GAMECMD_GET_BEGIN + 22,		// 获取自己发出的好友申请 
		CTS_GAMECMD_GET_OTHERS_FRIEND_APPLY		= CTS_GAMECMD_GET_BEGIN + 23,		// 获取自己收到的好友申请(别人发给自己的) 
		CTS_GAMECMD_GET_FRIEND_LIST				= CTS_GAMECMD_GET_BEGIN + 24,		// 获取好友列表 
		CTS_GAMECMD_GET_WORLD_AREA_INFO			= CTS_GAMECMD_GET_BEGIN	+ 25,		// 获取世界地图某块的信息(地理信息、城市信息)  
		CTS_GAMECMD_GET_WORLD_AREA_INFO1		= CTS_GAMECMD_GET_BEGIN	+ 26,		// 占位 
		CTS_GAMECMD_GET_ALLIANCE_MAIL			= CTS_GAMECMD_GET_BEGIN + 27,		// 获取联盟邮件(作废)   
		CTS_GAMECMD_GET_ALLIANCE_LOG			= CTS_GAMECMD_GET_BEGIN + 28,		// 获取联盟日志  
		CTS_GAMECMD_GET_PRIVATE_LOG				= CTS_GAMECMD_GET_BEGIN + 29,		// 获取私人日志  
		CTS_GAMECMD_GET_RELATION_LOG			= CTS_GAMECMD_GET_BEGIN + 30,		// 获取关系日志(附加对方account_id) 
		CTS_GAMECMD_GET_TRAINING_TE				= CTS_GAMECMD_GET_BEGIN + 31,		// 获取修炼时间事件 
		CTS_GAMECMD_GET_PLAYERCARD_NAME			= CTS_GAMECMD_GET_BEGIN + 32,		// 通过角色名获取名片信息  
		CTS_GAMECMD_GET_MY_ALLIANCE_JOIN_EVENT	= CTS_GAMECMD_GET_BEGIN + 33,		// 获取我的联盟申请记录 
		CTS_GAMECMD_GET_ALLIANCE_CARD_BY_ID		= CTS_GAMECMD_GET_BEGIN + 34,		// 根据联盟ID获取联盟卡片 
		CTS_GAMECMD_GET_ALLIANCE_CARD_BY_RANK	= CTS_GAMECMD_GET_BEGIN + 35,		// 根据联盟排名获取联盟卡片 
		CTS_GAMECMD_GET_OTHER_GOLDORE_INFO		= CTS_GAMECMD_GET_BEGIN + 36,		// 获取别人的金矿信息 

		CTS_GAMECMD_GET_CHRISTMAS_TREE_INFO		= CTS_GAMECMD_GET_BEGIN + 41,		// 获取自己的许愿树状态  
		CTS_GAMECMD_GET_GOLDORE_SMP_INFO_ALL	= CTS_GAMECMD_GET_BEGIN + 42,		// 获取一群人的金矿是否有事件发生 
		CTS_GAMECMD_GET_ALLIANCE_TRADE_INFO		= CTS_GAMECMD_GET_BEGIN + 43,		// 获取联盟跑商状态 
		CTS_GAMECMD_GET_ENEMY_LIST				= CTS_GAMECMD_GET_BEGIN + 44,		// 获取仇人列表 
		CTS_GAMECMD_GET_RANK_LIST				= CTS_GAMECMD_GET_BEGIN + 45,		// 获取排行榜信息 
		CTS_GAMECMD_GET_MY_RANK					= CTS_GAMECMD_GET_BEGIN + 46,		// 获取我的排名 
		CTS_GAMECMD_GET_IMPORTANT_CHAR_ATB		= CTS_GAMECMD_GET_BEGIN + 47,		// 获取重要的角色属性 
		CTS_GAMECMD_GET_PAY_SERIAL				= CTS_GAMECMD_GET_BEGIN + 48,		// 获取交易序列号  
		CTS_GAMECMD_GET_SERVER_TIME				= CTS_GAMECMD_GET_BEGIN + 50,		// 获取服务器时间 
		CTS_GAMECMD_GET_DONATE_SOLDIER_QUEUE	= CTS_GAMECMD_GET_BEGIN + 51,		// 获取盟友送兵队列 
		CTS_GAMECMD_GET_LEFT_DONATE_SOLDIER		= CTS_GAMECMD_GET_BEGIN + 52,		// 获取今日剩余可送兵数量 
		CTS_GAMECMD_GET_GOLD_DEAL				= CTS_GAMECMD_GET_BEGIN + 53,		// 获取交易列表  
		CTS_GAMECMD_GET_SELF_GOLD_DEAL			= CTS_GAMECMD_GET_BEGIN + 54,		// 获取自己的交易列表  
		CTS_GAMECMD_GET_WOLRD_RES				= CTS_GAMECMD_GET_BEGIN + 55,		// 获取世界资源(占位) 
		CTS_GAMECMD_GET_WORLD_RES_CARD			= CTS_GAMECMD_GET_BEGIN + 56,		// 获取世界资源详细信息 
		CTS_GAMECMD_GET_KICK_CLIENT_ALL			= CTS_GAMECMD_GET_BEGIN + 57,		// 获取踢出用户字串,version*appid 
		CTS_GAMECMD_GET_END						= CTS_GAMECMD_GET_BEGIN + 100,
	
		////////////////////////////////////////////////////////////////////////// 
		// 发送邮件(客户端只能发送普通邮件) 
		////////////////////////////////////////////////////////////////////////// 
		CTS_GAMECMD_ADD_MAIL					= CTS_GAMECMD_GET_END + 1,			// 发送邮件 
		CTS_GAMECMD_GET_MAIL					= CTS_GAMECMD_GET_END + 2,			// 获取邮件列表 
		CTS_GAMECMD_READ_MAIL					= CTS_GAMECMD_GET_END + 3,			// 阅读邮件 
		CTS_GAMECMD_DELETE_MAIL					= CTS_GAMECMD_GET_END + 4,			// 删除邮件 
		CTS_GAMECMD_ADD_MESSAGE_N				= CTS_GAMECMD_GET_END + 5,			// 向N个人发送同一封邮件 
		CTS_GAMECMD_NEW_PRIVATE_MAIL			= CTS_GAMECMD_GET_END + 7,			// 新的邮件到来 
		CTS_GAMECMD_ADD_MAIL_WITH_EXT_DATA		= CTS_GAMECMD_GET_END + 8,			// 发送带附件的邮件(这个是用于测试的指令) 
		CTS_GAMECMD_READ_MAIL_ALL				= CTS_GAMECMD_GET_END + 9,			// 将所有邮件标记为已读 
		CTS_GAMECMD_DELETE_MAIL_ALL				= CTS_GAMECMD_GET_END + 10,			// 删除所有邮件 
		CTS_GAMECMD_MAIL_END					= CTS_GAMECMD_GET_END + 10,			// 
	
		////////////////////////////////////////////////////////////////////////// 
		// 一些操作 
		////////////////////////////////////////////////////////////////////////// 
		CTS_GAMECMD_OPERATE_BEGIN				= CTS_GAMECMD_MAIL_END + 0,
		CTS_GAMECMD_OPERATE_BUILD_BUILDING		= CTS_GAMECMD_OPERATE_BEGIN + 1,	// 建造建筑 
		CTS_GAMECMD_OPERATE_UPGRADE_BUILDING	= CTS_GAMECMD_OPERATE_BEGIN + 2,	// 升级建筑 
		CTS_GAMECMD_OPERATE_PRODUCE_GOLD		= CTS_GAMECMD_OPERATE_BEGIN + 3,	// 生产金子 
		CTS_GAMECMD_OPERATE_FETCH_GOLD			= CTS_GAMECMD_OPERATE_BEGIN + 4,	// 采摘金子 
		CTS_GAMECMD_OPERATE_ACCE_BUILDING		= CTS_GAMECMD_OPERATE_BEGIN + 5,	// 加速建筑(升级/建造) 
		CTS_GAMECMD_OPERATE_ACCE_GOLD_PRODUCE	= CTS_GAMECMD_OPERATE_BEGIN + 6,	// 加速黄金生产 
		CTS_GAMECMD_OPERATE_RESEARCH			= CTS_GAMECMD_OPERATE_BEGIN + 7,	// 研究科技  
		CTS_GAMECMD_OPERATE_CONSCRIPT_SOLDIER	= CTS_GAMECMD_OPERATE_BEGIN + 8,	// 征召士兵   
		CTS_GAMECMD_OPERATE_UPGRADE_SOLDIER		= CTS_GAMECMD_OPERATE_BEGIN + 9,	// 升级士兵  
		CTS_GAMECMD_OPERATE_ACCE_RESEARCH		= CTS_GAMECMD_OPERATE_BEGIN + 10,	// 加速科技研究  
		CTS_GAMECMD_OPERATE_ACCEPT_ALLI_MEMBER	= CTS_GAMECMD_OPERATE_BEGIN + 11,	// 接受新成员加入联盟  
		CTS_GAMECMD_OPERATE_BUILD_ALLI_BUILDING	= CTS_GAMECMD_OPERATE_BEGIN + 12,	// 建造联盟建筑  
		CTS_GAMECMD_OPERATE_CANCEL_JOIN_ALLI	= CTS_GAMECMD_OPERATE_BEGIN + 13,	// 取消联盟加入申请  
		CTS_GAMECMD_OPERATE_CREATE_ALLI			= CTS_GAMECMD_OPERATE_BEGIN + 14,	// 创建联盟  
		CTS_GAMECMD_OPERATE_DISMISS_ALLI		= CTS_GAMECMD_OPERATE_BEGIN + 15,	// 解散联盟  
		CTS_GAMECMD_OPERATE_EXIT_ALLI			= CTS_GAMECMD_OPERATE_BEGIN + 16,	// 退出联盟   
		CTS_GAMECMD_OPERATE_EXPEL_ALLI_MEMBER	= CTS_GAMECMD_OPERATE_BEGIN + 17,	// 开除联盟成员  
		CTS_GAMECMD_OPERATE_JOIN_ALLI			= CTS_GAMECMD_OPERATE_BEGIN + 18,	// 申请加入联盟  
		CTS_GAMECMD_OPERATE_SET_ALLI_POSITION	= CTS_GAMECMD_OPERATE_BEGIN + 19,	// 设置成员职位  
		CTS_GAMECMD_OPERATE_REFUSE_JOIN_ALLI	= CTS_GAMECMD_OPERATE_BEGIN + 20,	// 拒绝加入联盟申请  
		CTS_GAMECMD_OPERATE_ABDICATE_ALLI		= CTS_GAMECMD_OPERATE_BEGIN + 21,	// 禅让盟主  
		CTS_GAMECMD_OPERATE_APPROVE_FRIEND		= CTS_GAMECMD_OPERATE_BEGIN + 22,	// 允许成为好友  
		CTS_GAMECMD_OPERATE_APPLY_FRIEND		= CTS_GAMECMD_OPERATE_BEGIN + 23,	// 申请成为好友  
		CTS_GAMECMD_OPERATE_REFUSE_FRIEND_APPLY	= CTS_GAMECMD_OPERATE_BEGIN + 24,	// 拒绝对方的好友申请  
		CTS_GAMECMD_OPERATE_CANCEL_FRIEND_APPLY	= CTS_GAMECMD_OPERATE_BEGIN + 25,	// 取消自己发出的好友申请  
		CTS_GAMECMD_OPERATE_DELETE_FRIEND		= CTS_GAMECMD_OPERATE_BEGIN + 26,	// 删除好友  
		CTS_GAMECMD_OPERATE_ADD_ALLIANCE_MAIL	= CTS_GAMECMD_OPERATE_BEGIN + 27,	// 添加联盟邮件  
		CTS_GAMECMD_OPERATE_BUY_ITEM			= CTS_GAMECMD_OPERATE_BEGIN + 28,	// 通过钻石或者水晶购买道具   
		CTS_GAMECMD_OPERATE_SELL_ITEM			= CTS_GAMECMD_OPERATE_BEGIN + 30,	// 出售道具(出售道具也走这个接口)   
		CTS_GAMECMD_OPERATE_FETCH_CHRISTMAS_TREE	= CTS_GAMECMD_OPERATE_BEGIN + 31,	// 收获许愿树 
		CTS_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE	= CTS_GAMECMD_OPERATE_BEGIN + 32,	// 许愿树浇水 
		CTS_GAMECMD_OPERATE_STEAL_GOLD			= CTS_GAMECMD_OPERATE_BEGIN + 33,	// 偷窃黄金 
		CTS_GAMECMD_OPERATE_ADD_TRAINING		= CTS_GAMECMD_OPERATE_BEGIN + 34,	// 将领修炼 
		CTS_GAMECMD_OPERATE_EXIT_TRAINING		= CTS_GAMECMD_OPERATE_BEGIN + 35,	// 退出修炼 
		CTS_GAMECMD_OPERATE_CONTRIBUTE_ALLIANCE	= CTS_GAMECMD_OPERATE_BEGIN + 36,	// 联盟捐献 
		CTS_GAMECMD_OPERATE_CONVERT_DIAMOND_GOLD= CTS_GAMECMD_OPERATE_BEGIN	+ 37,	// 钻石兑换黄金 
		CTS_GAMECMD_OPERATE_JOIN_ALLI_NAME		= CTS_GAMECMD_OPERATE_BEGIN + 38,	// 通过名字申请加入联盟 
		CTS_GAMECMD_OPERATE_SET_ALLI_INTRO		= CTS_GAMECMD_OPERATE_BEGIN + 39,	// 设置联盟的简介 
		CTS_GAMECMD_OPERATE_DRAW_LOTTERY		= CTS_GAMECMD_OPERATE_BEGIN + 40,	// 抽奖 
		CTS_GAMECMD_OPERATE_CHECK_DRAW_LOTTERY	= CTS_GAMECMD_OPERATE_BEGIN + 41,	// 抽奖条件校验  
		CTS_GAMECMD_OPERATE_FETCH_LOTTERY		= CTS_GAMECMD_OPERATE_BEGIN + 42,	// 获取抽奖结果  
		CTS_GAMECMD_OPERATE_ALLIANCE_TRADE		= CTS_GAMECMD_OPERATE_BEGIN + 43,	// 联盟跑商 
		CTS_GAMECMD_OPERATE_DELETE_ENEMY		= CTS_GAMECMD_OPERATE_BEGIN + 44,	// 删除仇人 
		CTS_GAMECMD_OPERATE_WRITE_SIGNATURE		= CTS_GAMECMD_OPERATE_BEGIN + 45,	// 书写个性签名 
		CTS_GAMECMD_OPERATE_CHANGE_NAME			= CTS_GAMECMD_OPERATE_BEGIN + 46,	// 修改名字
		CTS_GAMECMD_OPERATE_MOVE_CITY			= CTS_GAMECMD_OPERATE_BEGIN + 47,	// 城市迁移
		CTS_GAMECMD_OPERATE_READ_NOTIFICATION	= CTS_GAMECMD_OPERATE_BEGIN + 48,	// 阅读通知 
		CTS_GAMECMD_OPERATE_CHANGE_HERO_NAME	= CTS_GAMECMD_OPERATE_BEGIN + 49,	// 修改英雄名字 
		CTS_GAMECMD_OPERATE_ADD_GM_MAIL			= CTS_GAMECMD_OPERATE_BEGIN + 50,	// 给GM发邮件 
		CTS_GAMECMD_OPERATE_SET_ALLI_NAME		= CTS_GAMECMD_OPERATE_BEGIN + 51,	// 设置联盟名字 
		CTS_GAMECMD_OPERATE_FETCH_MAIL_REWARD	= CTS_GAMECMD_OPERATE_BEGIN + 52,	// 获取邮件中的奖励 
		CTS_GAMECMD_OPERATE_UPLOAD_BILL			= CTS_GAMECMD_OPERATE_BEGIN + 53,	// 客户端上传苹果账单 
		CTS_GAMECMD_OPERATE_REGISTER_ACCOUNT	= CTS_GAMECMD_OPERATE_BEGIN + 54,	// 注册账号(修改密码) 
		CTS_GAMECMD_OPERATE_BIND_DEVICE			= CTS_GAMECMD_OPERATE_BEGIN + 55,	// 绑定设备(通过设备ID登陆) 
		CTS_GAMECMD_OPERATE_DISMISS_SOLDIER		= CTS_GAMECMD_OPERATE_BEGIN	+ 56,	// 解散士兵 
		CTS_GAMECMD_OPERATE_ALLI_DONATE_SOLDIER	= CTS_GAMECMD_OPERATE_BEGIN	+ 57,	// 对盟友赠送士兵 
		CTS_GAMECMD_OPERATE_ALLI_RECALL_SOLDIER	= CTS_GAMECMD_OPERATE_BEGIN + 58,	// 对盟友赠送士兵进行召回 
		CTS_GAMECMD_OPERATE_ALLI_ACCE_SOLDIER	= CTS_GAMECMD_OPERATE_BEGIN + 59,	// 对盟友赠送士兵的行军加速 
		CTS_GAMECMD_OPERATE_ADD_GOLD_DEAL		= CTS_GAMECMD_OPERATE_BEGIN + 60,	// 添加黄金交易 
		CTS_GAMECMD_OPERATE_BUY_GOLD_IN_MARKET	= CTS_GAMECMD_OPERATE_BEGIN + 61,	// 进行黄金家里 
		CTS_GAMECMD_OPERATE_CANCEL_GOLD_DEAL	= CTS_GAMECMD_OPERATE_BEGIN + 62,	// 取消黄金交易 
		CTS_GAMECMD_OPERATE_SET_VIP_DISPLAY		= CTS_GAMECMD_OPERATE_BEGIN + 63,	// 设置vip显示 
		
		CTS_GAMECMD_OPERATE_TAVERN_REFRESH		= CTS_GAMECMD_OPERATE_BEGIN + 101,	// 酒馆刷新 
		CTS_GAMECMD_OPERATE_HIRE_HERO			= CTS_GAMECMD_OPERATE_BEGIN + 102,	// 招募武将 
		CTS_GAMECMD_OPERATE_FIRE_HERO			= CTS_GAMECMD_OPERATE_BEGIN + 103,	// 解雇武将 
		CTS_GAMECMD_OPERATE_LEVELUP_HERO		= CTS_GAMECMD_OPERATE_BEGIN + 104,	// 武将升级 
		CTS_GAMECMD_OPERATE_ADD_GROW			= CTS_GAMECMD_OPERATE_BEGIN + 105,	// 增加成长率 
		CTS_GAMECMD_OPERATE_CONFIG_HERO			= CTS_GAMECMD_OPERATE_BEGIN + 106,	// 武将配兵 
		CTS_GAMECMD_OPERATE_START_COMBAT		= CTS_GAMECMD_OPERATE_BEGIN + 107,	// 开始战斗 
		CTS_GAMECMD_OPERATE_STOP_COMBAT			= CTS_GAMECMD_OPERATE_BEGIN + 108,	// 结束战斗 
		CTS_GAMECMD_OPERATE_GET_COMBAT			= CTS_GAMECMD_OPERATE_BEGIN + 109,	// 获取战斗数据 
		CTS_GAMECMD_OPERATE_GET_HERO_REFRESH	= CTS_GAMECMD_OPERATE_BEGIN + 110,	// 获取刷新武将  
		CTS_GAMECMD_OPERATE_GET_HERO_HIRE		= CTS_GAMECMD_OPERATE_BEGIN + 111,	// 获取雇佣武将 
		CTS_GAMECMD_OPERATE_SAVE_COMBAT			= CTS_GAMECMD_OPERATE_BEGIN + 112,	// 战斗结果存盘  
		CTS_GAMECMD_OPERATE_LIST_COMBAT			= CTS_GAMECMD_OPERATE_BEGIN + 113,	// 获取所有战斗摘要   
		CTS_GAMECMD_OPERATE_CONFIG_CITYDEFENSE	= CTS_GAMECMD_OPERATE_BEGIN + 114,	// 配置城防武将  

		CTS_GAMECMD_GET_ITEM					= CTS_GAMECMD_OPERATE_BEGIN + 115,	// 获取道具  
		CTS_GAMECMD_ADD_ITEM					= CTS_GAMECMD_OPERATE_BEGIN + 116,	// 添加道具 
		CTS_GAMECMD_DEL_ITEM					= CTS_GAMECMD_OPERATE_BEGIN + 117,	// 删除道具 
		CTS_GAMECMD_EQUIP_ITEM					= CTS_GAMECMD_OPERATE_BEGIN + 118,	// 装备道具  
		CTS_GAMECMD_DISEQUIP_ITEM				= CTS_GAMECMD_OPERATE_BEGIN + 119,	// 卸下装备 
		CTS_GAMECMD_MOUNT_ITEM					= CTS_GAMECMD_OPERATE_BEGIN + 120,	// 镶嵌道具 
		CTS_GAMECMD_UNMOUNT_ITEM				= CTS_GAMECMD_OPERATE_BEGIN + 121,	// 卸下镶嵌 
		CTS_GAMECMD_COMPOS_ITEM					= CTS_GAMECMD_OPERATE_BEGIN + 122,	// 合成道具 
		CTS_GAMECMD_DISCOMPOS_ITEM				= CTS_GAMECMD_OPERATE_BEGIN + 123,	// 分解道具 
	
		CTS_GAMECMD_GET_QUEST					= CTS_GAMECMD_OPERATE_BEGIN + 124,	// 获取任务 
		CTS_GAMECMD_DONE_QUEST					= CTS_GAMECMD_OPERATE_BEGIN + 125,	// 完成任务 
	
		CTS_GAMECMD_GET_INSTANCESTATUS			= CTS_GAMECMD_OPERATE_BEGIN + 128,	// 获取玩家副本当前状态 
		CTS_GAMECMD_GET_INSTANCEDESC			= CTS_GAMECMD_OPERATE_BEGIN + 129,	// 获取副本实例摘要 
		CTS_GAMECMD_CREATE_INSTANCE				= CTS_GAMECMD_OPERATE_BEGIN + 130,	// 创建副本  
		CTS_GAMECMD_JOIN_INSTANCE				= CTS_GAMECMD_OPERATE_BEGIN + 131,	// 加入副本 
		CTS_GAMECMD_QUIT_INSTANCE				= CTS_GAMECMD_OPERATE_BEGIN + 132,	// 退出副本 
		CTS_GAMECMD_DESTROY_INSTANCE			= CTS_GAMECMD_OPERATE_BEGIN + 133,	// 销毁副本 
		CTS_GAMECMD_GET_INSTANCEDATA			= CTS_GAMECMD_OPERATE_BEGIN + 134,	// 获取副本数据 
		CTS_GAMECMD_CONFIG_INSTANCEHERO			= CTS_GAMECMD_OPERATE_BEGIN + 135,	// 配置副本武将  
		CTS_GAMECMD_GET_INSTANCELOOT			= CTS_GAMECMD_OPERATE_BEGIN + 136,	// 获取副本掉落  
		CTS_GAMECMD_KICK_INSTANCE				= CTS_GAMECMD_OPERATE_BEGIN + 137,	// 踢人    
		CTS_GAMECMD_START_INSTANCE				= CTS_GAMECMD_OPERATE_BEGIN + 138,	// 副本出征    
		CTS_GAMECMD_PREPARE_INSTANCE			= CTS_GAMECMD_OPERATE_BEGIN + 139,	// 副本就绪    
		
		CTS_GAMECMD_EQUIP_ITEM_ALL				= CTS_GAMECMD_OPERATE_BEGIN + 140,	// 一键换装    
		CTS_GAMECMD_MOUNT_ITEM_ALL				= CTS_GAMECMD_OPERATE_BEGIN + 141,	// 一键镶嵌    

		CTS_GAMECMD_USE_ITEM					= CTS_GAMECMD_OPERATE_BEGIN + 145,	// 使用道具 
		CTS_GAMECMD_AUTO_COMBAT					= CTS_GAMECMD_OPERATE_BEGIN + 146,	// 自动战斗开关 
		CTS_GAMECMD_SYNC_CHAR					= CTS_GAMECMD_OPERATE_BEGIN + 147,	// 同步数据，君主 
		CTS_GAMECMD_SYNC_HERO					= CTS_GAMECMD_OPERATE_BEGIN + 148,	// 同步数据，武将 
	
		CTS_GAMECMD_AUTO_SUPPLY					= CTS_GAMECMD_OPERATE_BEGIN + 149,	// 自动补给开关 
		CTS_GAMECMD_GET_EQUIP					= CTS_GAMECMD_OPERATE_BEGIN + 150,	// 获取已装备的装备 
		CTS_GAMECMD_GET_GEM						= CTS_GAMECMD_OPERATE_BEGIN + 151,	// 获取已镶嵌的宝石 
		CTS_GAMECMD_MANUAL_SUPPLY				= CTS_GAMECMD_OPERATE_BEGIN + 152,	// 手动补给 
		CTS_GAMECMD_REPLACE_EQUIP				= CTS_GAMECMD_OPERATE_BEGIN + 153,	// 替换装备   
		
		CTS_GAMECMD_GET_WORLDGOLDMINE			= CTS_GAMECMD_OPERATE_BEGIN + 155,	// 获取世界金矿    
		CTS_GAMECMD_CONFIG_WORLDGOLDMINE_HERO	= CTS_GAMECMD_OPERATE_BEGIN + 156,	// 配置金矿武将         
		CTS_GAMECMD_ROB_WORLDGOLDMINE			= CTS_GAMECMD_OPERATE_BEGIN + 157,	// 夺取世界金矿    
		CTS_GAMECMD_DROP_WORLDGOLDMINE			= CTS_GAMECMD_OPERATE_BEGIN + 158,	// 放弃世界金矿       
		CTS_GAMECMD_MY_WORLDGOLDMINE			= CTS_GAMECMD_OPERATE_BEGIN + 159,	// 我的世界金矿    

		CTS_GAMECMD_COMBAT_PROF					= CTS_GAMECMD_OPERATE_BEGIN + 161,	// 获取战力       
		CTS_GAMECMD_SUPPLY_INSTANCE				= CTS_GAMECMD_OPERATE_BEGIN + 162,	// 副本补给      
		CTS_GAMECMD_GET_CITYDEFENSE				= CTS_GAMECMD_OPERATE_BEGIN + 163,	// 获取城防武将  
		CTS_GAMECMD_USE_DRUG					= CTS_GAMECMD_OPERATE_BEGIN + 164,	// 使用草药 
		
		CTS_GAMECMD_ARMY_ACCELERATE				= CTS_GAMECMD_OPERATE_BEGIN + 166,	// 行军加速 ...
		CTS_GAMECMD_ARMY_BACK					= CTS_GAMECMD_OPERATE_BEGIN + 167,	// 召回 ...
		
		CTS_GAMECMD_WORLDCITY_GET				= CTS_GAMECMD_OPERATE_BEGIN + 170,	// 获得当前世界名城信息 
		CTS_GAMECMD_WORLDCITY_GETLOG			= CTS_GAMECMD_OPERATE_BEGIN + 171,	// 获得当前世界名城奖杯战绩 
		
		CTS_GAMECMD_WORLDCITY_RANK_MAN			= CTS_GAMECMD_OPERATE_BEGIN + 172,	// 获得当前奖杯个人排名  
		CTS_GAMECMD_WORLDCITY_RANK_ALLIANCE		= CTS_GAMECMD_OPERATE_BEGIN + 173,	// 获得当前奖杯联盟排名  
		CTS_GAMECMD_WORLDCITY_GETLOG_ALLIANCE	= CTS_GAMECMD_OPERATE_BEGIN + 174,	// 获得上次世界名城奖杯战绩联盟 
		
		CTS_GAMECMD_RECHARGE_TRY				= CTS_GAMECMD_OPERATE_BEGIN + 175,	// 充值尝试，玩家在后续操作中可能会取消 
		
		CTS_GAMECMD_OPERATE_END					= CTS_GAMECMD_OPERATE_BEGIN + 200,
	
		////////////////////////////////////////////////////////////////////////// 
		// 服务器给客户端的一些通知 
		////////////////////////////////////////////////////////////////////////// 
		CTS_GAMECMD_NOTIFY_BEGIN				= CTS_GAMECMD_OPERATE_END + 0,
		CTS_GAMECMD_POPULATION_ADD				= CTS_GAMECMD_NOTIFY_BEGIN + 1,		// 增加了新的人口  
		CTS_GAMECMD_GOLD_CAN_FETCH				= CTS_GAMECMD_NOTIFY_BEGIN + 2,		// 金子已经成熟  
		CTS_GAMECMD_BUILDING_TE					= CTS_GAMECMD_NOTIFY_BEGIN + 3,		// 建筑时间事件通知  
		CTS_GAMECMD_RESEARCH_TE					= CTS_GAMECMD_NOTIFY_BEGIN + 4,		// 研究时间事件通知  
		CTS_GAMECMD_CHAR_IS_ONLINE				= CTS_GAMECMD_NOTIFY_BEGIN + 5,		// 查询某玩家是否在线 
		CTS_GAMECMD_CHRISTMAS_TREE_READY_RIPE	= CTS_GAMECMD_NOTIFY_BEGIN + 6,		// 许愿树准备成熟 
		CTS_GAMECMD_CHRISTMAS_TREE_RIPE			= CTS_GAMECMD_NOTIFY_BEGIN + 7,		// 许愿树成熟 
		CTS_GAMECMD_TRAINING_OVER				= CTS_GAMECMD_NOTIFY_BEGIN + 8,		// 训练结束 
		CTS_GAMECMD_NOTIFY_END					= CTS_GAMECMD_NOTIFY_BEGIN + 200,

        //////////////////////////////////////////////////////////////////////////
        // 联盟副本 
        //////////////////////////////////////////////////////////////////////////
        CTS_GAMECMD_ALLI_INSTANCE_BEGIN = CTS_GAMECMD_NOTIFY_END,
        CTS_GAMECMD_ALLI_INSTANCE_CREATE = CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 1,		                // 创建副本 
        CTS_GAMECMD_ALLI_INSTANCE_JOIN = CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 2,		                // 加入副本 
        CTS_GAMECMD_ALLI_INSTANCE_EXIT = CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 3,		                // 退出副本 
        CTS_GAMECMD_ALLI_INSTANCE_KICK = CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 4,		                // 踢出副本 
        CTS_GAMECMD_ALLI_INSTANCE_DESTROY = CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 5,		            // 销毁副本 
        CTS_GAMECMD_ALLI_INSTANCE_CONFIG_HERO = CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 6,		        // 配置将领 
        CTS_GAMECMD_ALLI_INSTANCE_START = CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 7,		                // 副本出征 
        CTS_GAMECMD_ALLI_INSTANCE_READY = CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 8,		                // 准备就绪 
        CTS_GAMECMD_ALLI_INSTANCE_GET_LIST = CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 9,		            // 获取副本列表 
        CTS_GAMECMD_ALLI_INSTANCE_GET_CHAR_DATA = CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 10,		        // 获取副本角色详细信息 
        CTS_GAMECMD_ALLI_INSTANCE_GET_HERO_DATA = CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 11,		        // 获取副本将领详细信息 
        CTS_GAMECMD_ALLI_INSTANCE_GET_STATUS = CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 12,		        // 自己的副本状态 
        CTS_GAMECMD_ALLI_INSTANCE_GET_LOOT = CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 13,		            // 获取掉落 
        CTS_GAMECMD_ALLI_INSTANCE_SAVE_HERO_DEPLOY = CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 14,	        // 保存将领布局 
        CTS_GAMECMD_ALLI_INSTANCE_START_COMBAT = CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 15,		        // 开始战斗 
        CTS_GAMECMD_ALLI_INSTANCE_COMBAT_LOG = CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 17,                // 复杂战报  
        CTS_GAMECMD_ALLI_INSTANCE_SIMPLE_COMBAT_LOG = CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 19,         // 简单战报 
        CTS_GAMECMD_ALLI_INSTANCE_STOP_COMBAT = CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 20,		        // 停止战斗  
        CTS_GAMECMD_ALLI_INSTANCE_SUPPLY = CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 21,		            // 补给 
		CTS_GAMECMD_ALLI_INSTANCE_GET_DEFENDER_DEPLOY	= CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 22,		// 获取军队布局 
        CTS_GAMECMD_ALLI_INSTANCE_END = CTS_GAMECMD_ALLI_INSTANCE_BEGIN + 100,
		
		////////////////////////////////////////////////////////////////////////// 
		// 竞技场 
		////////////////////////////////////////////////////////////////////////// 
		CTS_GAMECMD_ARENA_BEGIN					= CTS_GAMECMD_ALLI_INSTANCE_END,
		CTS_GAMECMD_ARENA_UPLOAD_DATA			= CTS_GAMECMD_ARENA_BEGIN + 1,				// 上传数据 
		CTS_GAMECMD_ARENA_GET_RANK_LIST			= CTS_GAMECMD_ARENA_BEGIN + 2,				// 排行榜 
		CTS_GAMECMD_ARENA_GET_STATUS			= CTS_GAMECMD_ARENA_BEGIN + 3,				// 竞技场状态,CD,挑战列表等 
		CTS_GAMECMD_ARENA_PAY					= CTS_GAMECMD_ARENA_BEGIN + 4,				// 付费竞技 
		CTS_GAMECMD_ARENA_GET_DEPLOY			= CTS_GAMECMD_ARENA_BEGIN + 5,				// 获取竞技场布局 
		CTS_GAMECMD_ARENA_END					= CTS_GAMECMD_ARENA_BEGIN + 100,
		
		//////////////////////////////////////////////////////////////////////////
		// 将领
		//////////////////////////////////////////////////////////////////////////
		CTS_GAMECMD_HERO_BEGIN					= CTS_GAMECMD_ARENA_END,
		CTS_GAMECMD_HERO_SUPPLY					= CTS_GAMECMD_HERO_BEGIN + 1,				// 将领补给 
		CTS_GAMECMD_HERO_END					= CTS_GAMECMD_HERO_BEGIN + 200,
		
		////////////////////////////////////////////////////////////////////////// 
		// 位置收藏夹 
		//////////////////////////////////////////////////////////////////////////  
		CTS_GAMECMD_POSITION_MARK_BEGIN			= CTS_GAMECMD_HERO_END,
		CTS_GAMECMD_POSITION_MARK_ADD_RECORD	= CTS_GAMECMD_POSITION_MARK_BEGIN + 1,
		CTS_GAMECMD_POSITION_MARK_CHG_RECORD	= CTS_GAMECMD_POSITION_MARK_BEGIN + 2,
		CTS_GAMECMD_POSITION_MARK_DEL_RECORD	= CTS_GAMECMD_POSITION_MARK_BEGIN + 3,
		CTS_GAMECMD_POSITION_MARK_GET_RECORD	= CTS_GAMECMD_POSITION_MARK_BEGIN + 4,
		CTS_GAMECMD_POSITION_MARK_END			= CTS_GAMECMD_POSITION_MARK_BEGIN + 10,
	}
	/*struct data_msg_t 
	{
		public int 		DataCmd1;
		public int		iInt2;
		public byte		cChar3;
		public float	fFloat4;
		public int		iLen5;
		public byte[]	szInfo6;
	}*/
	struct TTY_CLIENT_LPGAMEPLAY_LOGIN_T
	{
		public byte			nCmd1;
		public const int 	szAccountLen2 = 32; 
		public byte[]		szAccount3;
		public uint			nDeviceType4;
		public uint			nAppID5;
		public const int 	Len6 = 64;
		public byte[]		szMac7;
		public int			nPassLen8;
		public byte[]		szPass9;
		
	}
	//通过UDID登录 
	struct TTY_CLIENT_LPGAMEPLAY_LOGIN_BY_DEVICEID_T
	{
		public byte			nCmd1; 
		public const int 	Len2 = 64; 
		public byte[]		szMAC3; 
		public uint			nDeviceType4; 
		public uint			nAppID5;
	}
	struct TTY_CLIENT_LPGAMEPLAY_CREATEACCOUNT_T
	{
		public byte			nCmd1;
		public const int 	len2 = 32;
		public byte[]		szAccountName3;
		public const int 	len4 = 256;
		public byte[]		szPass5;
		public const int 	len6 = 64;
		public byte[]		szMAC7;
		public uint		    nDeviceType8;
		public byte			bUseRandomName9;
		public int			nAppID10;							// 游戏ID 

	}
	struct TTY_CLIENT_LPGAMEPLAY_CREATECHAR_T 
	{
		public byte			nCmd1;
		public const int 	szNameLen2 = 32; 
		public byte[]		szName3;
		public uint			nSex4;							// 性别 
		public uint			nHeadID5;						// 头像 
		public uint			nCountry6;						// 国家 
	} 
	struct TTY_CLIENT_LPGAMEPLAY_LOGOUT_T 
	{
		public byte			nCmd1;
	}
	struct CTS_GAMECMD_CHAT_PRIVATE_T 
	{
		public 	byte	nCmd1;
		public	uint	nGameCmd2;
		public 	ulong	nPeerAccountID3;
		public  int		nTextLen4;
		public  byte[]	szText5;
	}
	struct	CTS_GAMECMD_CHAT_WORLD_T
	{
		public 	byte	nCmd1;
		public	uint	nGameCmd2;
		public	int		nTextLen3;
		public  byte[]	szText4;
	}
	struct	CTS_GAMECMD_CHAT_GROUP_T
	{
		public 	byte	nCmd1;
		public	uint	nGameCmd2;
		public	int		nTextLen3;
		public  byte[]	szText4;
	}
	struct CTS_GAMECMD_CHAT_ALLIANCE_T 
	{
		public 	byte	nCmd1;
		public	uint	nGameCmd2;
		public	int		nTextLen3;
		public  byte[]	szText4;
	}
	struct CTS_GAMECMD_CHAT_GET_RECENT_CHAT_T 
	{
		public 	byte	nCmd1;
		public	uint	nGameCmd2;
	}
	struct CTS_GAMECMD_ADD_MAIL_T  
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  ulong	nToCharID3;
		public 	int		nTextLen4;
		public	byte[]	szText5;
	}
	struct CTS_GAMECMD_ADD_MAIL_N_T  
	{ 
		// 	int					nNameTotalLen;   
		// 	char				szNameBegin[1];   
		// 	int					nTextLen;   
		//	char				szTextBegin[1];   
		// int名字总长度(除以TTY_CHARACTERNAME_LEN就是个数),char[]名字数组(每个单元长TTY_CHARACTERNAME_LEN),int邮件内容长度,char[]邮件内容  
		public	byte	nCmd1; 
		public  uint	nGameCmd2; 
	} 
	struct CTS_GAMECMD_GET_MAIL_T  
	{ 
		public 	byte	nCmd1; 
		public  uint	nGameCmd2; 
		public  uint	nFromNum3;	// 起始条目 
		public 	uint	nToNum4;	// 结束条目 
		public  byte	nType5;		// 类型 
	}
	struct CTS_GAMECMD_READ_MAIL_T
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  uint	nMailID3; // 所读邮件的id 
	}
	struct CTS_GAMECMD_DELETE_MAIL_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  uint	nMailI3;			// 所删除邮件的id 
	}
	struct CTS_GAMECMD_GET_NOTIFICATION_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_GET_PLAYERCARD_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  ulong	nAccountID3;		// 需要名片的账号ID 
	}
	struct CTS_GAMECMD_GET_PLAYERCARD_POS_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  int		nPosX3;
		public  uint	nPosY4;
	}
	struct CTS_GAMECMD_GET_TILE_INFO_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  uint	nPosX3;
		public  uint	nPosY4;
	}
	struct CTS_GAMECMD_GET_WORLD_AREA_INFO_T
	{
		public 	byte 	nCmd1;
		public 	uint 	nGameCmd2;
		public 	uint	nPosX3;
		public	uint	nPosY4;
		public	uint	nToPosX5;
		public	uint	nToPosY6;
	}
	struct CTS_GAMECMD_OPERATE_MOVE_CITY_T
	{
		public	byte	nCmd1;
		public	uint	nGameCmd2;
		public 	uint	nPosX3;
		public 	uint	nPosY4;
	}
	struct CTS_GAMECMD_GET_BUILDING_LIST_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_GET_BUILDING_TE_LIST_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	
	struct CTS_GAMECMD_GET_ALLIANCE_INFO_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_GET_ALLIANCE_MEMBER_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_GET_ALLIANCE_BUILDING_TE_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_GET_ALLIANCE_BUILDING_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_GET_ALLIANCE_JOIN_EVENT_T
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_GET_CHAR_ATB_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_GET_ALLIANCE_MAIL_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_GET_ALLIANCE_LOG_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_GET_PRIVATE_LOG_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_GET_PLAYERCARD_NAME_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public  const int 	len3 = 32;
		public 	byte[]		szCharName4;
	}
	struct CTS_GAMECMD_OPERATE_BUILD_BUILDING_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  uint	nExcelID3;
		public  uint	nAutoID4;
	}
	struct CTS_GAMECMD_OPERATE_UPGRADE_BUILDING_T
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  uint	nAutoID3;
	}
	struct CTS_GAMECMD_OPERATE_PRODUCE_GOLD_T 
	{
		public enum enum_produce
		{
			PRODUCE_HALF				= 0x00,				// 半小时  
			PRODUCE_ONE					= 0x01,				// 一小时  
			PRODUCE_FOUR				= 0x02,				// 四个小时  
			PRODUCE_EIGHT				= 0x03,				// 八个小时  
		} 
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  uint	nAutoID3;
		public  uint	nProductionChoice4;
	}
	struct CTS_GAMECMD_OPERATE_FETCH_GOLD_T  
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  uint	nAutoID3;
	}
	struct CTS_GAMECMD_OPERATE_ACCE_BUILDING_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  uint	nAutoID3;
		public  uint	nType4;				// te_subtype_building_build,te_subtype_building_upgrade 
		public  uint	nTime5;				// 加速的时间(单位:秒) 
		public  uint	nMoneyType6;
	}
	struct CTS_GAMECMD_OPERATE_ACCE_GOLD_PRODUCE_T  
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  uint	nAutoID3;
		public  uint	nTime4;				// 加速的时间(单位:秒)   
		public 	uint	nMoneyType5;
	}
	struct CTS_GAMECMD_GET_PRODUCTION_EVENT_T  
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  ulong	nAccountID3;		// 可能会拿自己的,也可能拿别人的  
	}
	struct CTS_GAMECMD_GET_PRODUCTION_TE_LIST_T  
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  ulong	nAccountID3;		
	}
	struct CTS_GAMECMD_OPERATE_RESEARCH_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  uint	nExcelID3;
	}
	struct CTS_GAMECMD_GET_TECHNOLOGY_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_GET_RESEARCH_TE_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_OPERATE_CONSCRIPT_SOLDIER_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  uint	nExcelID3;
		public  uint	nLevel4;
		public  uint	nNum5;
	}
	struct CTS_GAMECMD_OPERATE_UPGRADE_SOLDIER_T  
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  uint	nExcelID3;
		public  uint	nFromLevel4;
		public  uint	nToLevel5;
		public  uint	nNum6;
	}
	struct CTS_GAMECMD_GET_SOLDIER_T  
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_OPERATE_ACCE_RESEARCH_T  
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  uint	nExcelID3;
		public  uint	nTime4;
		public  uint	nMoneyType5;
	}
	// alliance  
	struct CTS_GAMECMD_OPERATE_ACCEPT_ALLI_MEMBER_T  
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public 	ulong	nMemberID3;
	}
	struct CTS_GAMECMD_OPERATE_BUILD_ALLI_BUILDING_T  
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  int		nExcelID3;
	}
	struct CTS_GAMECMD_OPERATE_CANCEL_JOIN_ALLI_T  
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_OPERATE_CREATE_ALLI_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public 	const int len3 = 32;
		public  byte[]	szAllianceName4;
	}
	struct CTS_GAMECMD_OPERATE_DISMISS_ALLI_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_OPERATE_EXIT_ALLI_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_OPERATE_EXPEL_ALLI_MEMBER_T  
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public 	ulong	nMemberID3;
	}
	struct CTS_GAMECMD_OPERATE_JOIN_ALLI_T  
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public 	ulong	nAllianceID3;
	}
	struct CTS_GAMECMD_OPERATE_SET_ALLI_POSITION_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public 	ulong	nMemberID3;
		public  uint	nPosition4;
	}
	struct CTS_GAMECMD_OPERATE_REFUSE_JOIN_ALLI_T  
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public 	ulong	nApplicantID3;
	}
	struct CTS_GAMECMD_OPERATE_ABDICATE_ALLI_T  
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	const int 	len3 = 32;
		public  byte[]		szMemberName4;
	}
	struct CTS_GAMECMD_OPERATE_ADD_ALLIANCE_MAIL_T  
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public 	int	nLen3;
		public  byte[]	szText4;
	} 
	struct CTS_GAMECMD_OPERATE_BUY_ITEM_T   
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  uint	nExcelID3;
		public  uint	nNum4;
		public  uint	nMoneyType5;
	}
	struct CTS_GAMECMD_OPERATE_SELL_ITEM_T  
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public 	ulong	nItemID3;
		public  uint	nNum4;
	}
	struct CTS_GAMECMD_GET_TRAINING_TE_T  
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_OPERATE_ADD_TRAINING_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  uint	nTimeUnitNum3;		// 就是时间长度,1代表1个基本时间单位，10代表10个基本时间单位(目前为小时) 
		public  int		nNum4;
		public 	ulong[] HeroIDs5;
	}
	struct CTS_GAMECMD_OPERATE_EXIT_TRAINING_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public 	ulong	nHeroID3;
	}
	struct CTS_GAMECMD_TRAINING_OVER_T 
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public 	ulong	nHeroID3;
	}
	
	struct CTS_GAMECMD_OPERATE_TAVERN_REFRESH_T
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public	int		nUseItem3;
	}
	struct CTS_GAMECMD_OPERATE_HIRE_HERO_T
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  uint	nSlotID3;
		public  ulong	nHeroID4;
	}
	struct CTS_GAMECMD_OPERATE_FIRE_HERO_T
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  ulong	nHeroID3;
	}
	struct CTS_GAMECMD_OPERATE_LEVELUP_HERO_T
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public 	ulong	nHeroID3;
	}
	struct CTS_GAMECMD_OPERATE_ADD_GROW_T
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  ulong	nHeroID3;
		public  float	fGrow4;
	}
	struct CTS_GAMECMD_OPERATE_CONFIG_HERO_T
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  ulong	nHeroID3;
		public  int		nArmyType4;
		public  int		nArmyLevel5;
		public  int		nArmyNum6;
	}
	struct CTS_GAMECMD_OPERATE_GET_HERO_REFRESH_T
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_OPERATE_GET_HERO_HIRE_T
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_OPERATE_START_COMBAT_T
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public  ulong	nObjID3;
		public  int		nCombatType4;
		public 	int 	nAutoCombat5;
		public	int 	nAutoSupply6;
		public  ulong	n1Hero7;
		public  ulong	n2Hero8;
		public  ulong	n3Hero9;
		public  ulong	n4Hero10;
		public 	ulong	n5Hero11;
		public	int		nStopLevel12; // 第几关停止 ...
	}
	struct CTS_GAMECMD_OPERATE_STOP_COMBAT_T
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public 	ulong  	nCombatID3;
	}
	struct CTS_GAMECMD_ARMY_ACCELERATE_T
	{
		public	byte	nCmd1;
		public 	uint	nGameCmd2;
		public 	ulong	nCombatID3;
		public 	int 	nTime4;
		public 	byte	byType5;
	}
	struct CTS_GAMECMD_ARMY_BACK_T
	{
		public	byte	nCmd1;
		public	uint	nGameCmd2;
		public	ulong	nCombatID3;
	}
	struct CTS_GAMECMD_OPERATE_GET_COMBAT_T
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public 	ulong	nCombatID3;
	}
	struct CTS_GAMECMD_OPERATE_SAVE_COMBAT_T
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
		public 	ulong	nCombatID;
	}
	struct CTS_GAMECMD_OPERATE_LIST_COMBAT_T
	{
		public 	byte	nCmd1;
		public  uint	nGameCmd2;
	}
	struct CTS_GAMECMD_OPERATE_CONFIG_CITYDEFENSE_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		n1Hero3;
		public 	ulong		n2Hero4;
		public 	ulong		n3Hero5;
		public 	ulong		n4Hero6;
		public 	ulong		n5Hero7;
		public	int			nAutoSupply8;
	}
	// 道具 
	struct CTS_GAMECMD_GET_ITEM_T 
	{	
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_ADD_ITEM_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public  int			nExcelID3;
		public 	int			nNum4;
	}
	struct CTS_GAMECMD_DEL_ITEM_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nItemID3;
		public 	int			nNum4;
	}
	struct CTS_GAMECMD_EQUIP_ITEM_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nHeroID3;
		public 	ulong		nItemID4;
	}
	struct CTS_GAMECMD_DISEQUIP_ITEM_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nHeroID3;
		public 	ulong		nItemID4;
	}
	struct CTS_GAMECMD_MOUNT_ITEM_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nHeroID3;
		public 	ulong		nEquipID4;
		public 	int			nSlotIdx5;
		public 	int			nExcelID6;
	}
	struct CTS_GAMECMD_UNMOUNT_ITEM_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nHeroID3;
		public 	ulong		nEquipID4;
		public 	int			nSlotIdx5;
	}
	struct CTS_GAMECMD_COMPOS_ITEM_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public  int			nExcelID3;
		public  int			nNum4;
	}
	struct CTS_GAMECMD_DISCOMPOS_ITEM_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	int			nExcelID3;
		public  int			nNum4;
	}
	// 获取任务列表 ... 
	struct CTS_GAMECMD_GET_QUEST_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	// 确认完成任务 ...
	struct CTS_GAMECMD_DONE_QUEST_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	int			nExcelID3;
	}
	// 副本 
	struct CTS_GAMECMD_GET_INSTANCESTATUS_T  
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_GET_INSTANCEDESC_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	int			nExcelID3;
		public	int 		nClass4;
	} 
	struct CTS_GAMECMD_CREATE_INSTANCE_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	int			nExcelID3;
		public	int 		nClass4;
	}
	struct CTS_GAMECMD_JOIN_INSTANCE_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	int			nExcelID3;
		public 	ulong		nInstanceID4;
	}
	struct CTS_GAMECMD_QUIT_INSTANCE_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	int			nExcelID3;
		public 	ulong		nInstanceID4;
	}
	struct CTS_GAMECMD_DESTROY_INSTANCE_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public  int			nExcelID3;
		public  ulong		nInstanceID4;
	}
	struct CTS_GAMECMD_GET_INSTANCEDATA_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	int			nExcelID3;
		public 	ulong		nInstanceID4;
	}
	struct CTS_GAMECMD_CONFIG_INSTANCEHERO_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	int			nExcelID3;
		public 	ulong		nInstanceID4;
		public 	ulong		n1Hero5;
		public 	ulong		n2Hero6;
		public 	ulong		n3Hero7;
		public 	ulong		n4Hero8;
		public 	ulong		n5Hero9;
	}
	struct CTS_GAMECMD_GET_INSTANCELOOT_T  
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public	int			nExcelID3;
		public 	ulong		nInstanceID4;
	}
	struct CTS_GAMECMD_KICK_INSTANCE_T
	{
		public 	byte		nCmd1;
		public	uint		nGameCmd2;
		public	int 		nExcelID3;
		public	ulong		nInstnaceID4;
		public	ulong		nObjID5;
	}
	struct CTS_GAMECMD_START_INSTANCE_T
	{
		public	byte		nCmd1;
		public	uint		nGameCmd2;
		public	int			nExcelID3;
		public	ulong		nInstanceID4;
	}
	struct CTS_GAMECMD_PREPARE_INSTANCE_T
	{
		public	byte		nCmd1;
		public	uint		nGameCmd2;
		public	int			nExcelID3;
		public	ulong		nInstanceID4;
		public	int 		nVal5;
	}
	struct CTS_GAMECMD_SUPPLY_INSTANCE_T
	{
		public  byte 		nCmd1;
		public 	uint		nGameCmd2;
		public	int 		nExcelID3;
		public 	ulong		nInstanceID4;
	}
	// worldgoldmine
	struct CTS_GAMECMD_GET_WORLDGOLDMINE_T
	{
		public	byte		nCmd1;
		public	uint		nGameCmd2;
		public	int			nArea3;
		public	int 		nClass4;
		public 	int 		nIdx5;
	}
	struct CTS_GAMECMD_CONFIG_WORLDGOLDMINE_HERO_T
	{
		public	byte		nCmd1;
		public	uint		nGameCmd2;
		public 	ulong		n1Hero3;
		public	ulong		n2Hero4;
		public	ulong		n3Hero5;
		public	ulong		n4Hero6;
		public 	ulong		n5Hero7;
	}
	struct CTS_GAMECMD_ROB_WORLDGOLDMINE_T
	{
		public 	byte		nCmd1;
		public	uint		nGameCmd2;
		public	int 		nArea3;
		public	int 		nClass4;
		public 	int 		nIdx5;
		public	ulong		n1Hero6;
		public	ulong		n2Hero7;
		public	ulong		n3Hero8;
		public	ulong		n4Hero9;
		public	ulong		n5Hero10;
	}
	struct CTS_GAMECMD_DROP_WORLDGOLDMINE_T
	{
		public	byte		nCmd1;
		public	uint		nGameCmd2;
	}
	struct CTS_GAMECMD_MY_WORLDGOLDMINE_T
	{
		public	byte		nCmd1;
		public	uint		nGameCmd2;
	}
	struct CTS_GAMECMD_AUTO_COMBAT_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nInstanceID3;
		public  int			nVal4;
	}
	struct CTS_GAMECMD_COMBAT_PROF_T
	{
		public 	byte 	nCmd1;
		public 	uint	nGameCmd2;
		public	int		nClass3;
		public 	ulong	nObjID4;
		public	int		nCombatType5;
	}
	struct CTS_GAMECMD_GET_CITYDEFENSE_T
	{
		public 	byte 	nCmd1;
		public 	uint	nGameCmd2;
	}
	struct CTS_GAMECMD_USE_DRUG_T 
	{
		public 	byte 	nCmd1;
		public 	uint	nGameCmd2;
		public  int		nNum3;
		public  ulong[] HeroIDs4;
		// 后面跟nNum个nHeroID 
	}
	struct CTS_GAMECMD_USE_ITEM_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nHeroID3;
		public  int			nExcelID4;
		public	int			nNum5;
	}
	struct CTS_GAMECMD_SYNC_CHAR_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_SYNC_HERO_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nHeroID3;
	}
	struct CTS_GAMECMD_AUTO_SUPPLY_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nInstanceID3;
		public	int			nVal4;
	}
	struct CTS_GAMECMD_MANUAL_SUPPLY_T
	{
		public 	byte		nCmd1;
		public	uint		nGameCmd2;
		public	ulong		nInstanceID3;
		public	int			nCombatType4;
		public	ulong 		n1Hero5;
		public	ulong 		n2Hero6;
		public	ulong 		n3Hero7;
		public	ulong 		n4Hero8;
		public	ulong 		n5Heor9;
	}
	struct CTS_GAMECMD_REPLACE_EQUIP_T 
	{
		public 	byte		nCmd1;
		public	uint		nGameCmd2;
		public	ulong		nHeroID3;
		public 	int			nEquipType4;
		public	ulong		nItemID5;
	}
	struct CTS_GAMECMD_GET_EQUIP_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_GET_GEM_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_CHAR_IS_ONLINE_T  
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nAccountID3;
	}
	// 好友 
	struct CTS_GAMECMD_GET_MY_FRIEND_APPLY_T  
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_GET_OTHERS_FRIEND_APPLY_T  
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_GET_FRIEND_LIST_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_OPERATE_APPROVE_FRIEND_T  
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public  ulong		nPeerAccountID3;
	}
	struct CTS_GAMECMD_OPERATE_APPLY_FRIEND_T  
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nPeerAccountID3;
	}
	struct CTS_GAMECMD_OPERATE_REFUSE_FRIEND_APPLY_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nPeerAccountID3;
	}
	struct CTS_GAMECMD_OPERATE_CANCEL_FRIEND_APPLY_T  
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nPeerAccountID3;
	}
	struct CTS_GAMECMD_OPERATE_DELETE_FRIEND_T  
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nFriendID3;
	}
	struct CTS_GAMECMD_OPERATE_CONTRIBUTE_ALLIANCE_T  
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public uint		nGold3;
	}
	struct CTS_GAMECMD_OPERATE_CONVERT_DIAMOND_GOLD_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public uint		nDiamond3;
	}
	struct CTS_GAMECMD_OPERATE_JOIN_ALLI_NAME_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	const int 	len3 = 32;
		public  byte[]		szAllianceName4;
	}
	struct CTS_GAMECMD_GET_MY_ALLIANCE_JOIN_EVENT_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_OPERATE_SET_ALLI_INTRO_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	int			nLen3;
		public 	byte[]		szIntroduction4;
	}
	struct CTS_GAMECMD_GET_ALLIANCE_CARD_BY_ID_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nAllianceID3;
	}
	struct CTS_GAMECMD_GET_ALLIANCE_CARD_BY_RANK_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	uint		nFromRank3;
		public  uint		nToRank4;
	}
	struct CTS_GAMECMD_OPERATE_DRAW_LOTTERY_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_OPERATE_CHECK_DRAW_LOTTERY_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public	uint		nType3;
	}
	struct CTS_GAMECMD_OPERATE_FETCH_LOTTERY_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public	uint		nMoneyType3;
	}
	struct CTS_GAMECMD_OPERATE_FETCH_CHRISTMAS_TREE_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nPeerAccountID3;
	}
	struct CTS_GAMECMD_OPERATE_STEAL_GOLD_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nPeerAccountID3;
		public  uint		nAutoID4;
	}
	struct CTS_GAMECMD_GET_CHRISTMAS_TREE_INFO_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_GET_OTHER_GOLDORE_INFO_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nAccountID3;
	}
	struct CTS_GAMECMD_GET_GOLDORE_SMP_INFO_ALL_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	int			nNum3;
		public  ulong[] 	AccountIdArray4;
		// 后面跟nNum个account_id  
	}
	struct CTS_GAMECMD_GET_ALLIANCE_TRADE_INFO_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_OPERATE_ALLIANCE_TRADE_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_GET_ENEMY_LIST_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_OPERATE_DELETE_ENEMY_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nEnemyID3;
	}
	struct CTS_GAMECMD_GET_RANK_LIST_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public  uint		nType3;
		public  uint		nFrom4;
		public  uint		nNum5;
	}
	struct CTS_GAMECMD_GET_MY_RANK_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public  uint		nType3;
	}
	struct CTS_GAMECMD_GET_IMPORTANT_CHAR_ATB_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_GET_PAY_SERIAL_T
	{	
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_OPERATE_WRITE_SIGNATURE_T  
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	const int 	len3 = 129;
		public 	byte[]		szSignature4;
	}
	struct CTS_GAMECMD_OPERATE_READ_NOTIFICATION_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public  uint		nNotificationID3;
	}
	struct CTS_GAMECMD_OPERATE_CHANGE_NAME_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public  const int 	len3 = 32;
		public 	byte[]		szNewName4;
	}
	struct CTS_GAMECMD_OPERATE_CHANGE_HERO_NAME_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public	ulong		nHeroID3;
		public  const int 	len4 = 32;
		public 	byte[]		szNewName5;
	}
	struct CTS_GAMECMD_READ_MAIL_ALL_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public  uint		nMailType3;
	}
	struct CTS_GAMECMD_DELETE_MAIL_ALL_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public  uint		nMailType3;
	}
	struct CTS_GAMECMD_OPERATE_ADD_GM_MAIL_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public  int			nLength3;
		public 	byte[]		szGmMail4;
		// char数组,长nLength
	}
	struct CTS_GAMECMD_WORLDCITY_GET_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_WORLDCITY_GETLOG_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public	int			nPageNum3;
		public	int			nPageIdx4;
	}
	struct CTS_GAMECMD_WORLDCITY_GETLOG_ALLIANCE_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public	int			nPageNum3;
		public	int			nPageIdx4;
	}
	struct CTS_GAMECMD_WORLDCITY_RANK_MAN_T  
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public  int			byPageNum3;
		public 	int 		byPageIdx4;
	}
	struct CTS_GAMECMD_WORLDCITY_RANK_ALLIANCE_T  
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	int		byPageNum3;
		public 	int		byPageIdx4;
	}
	struct CTS_GAMECMD_OPERATE_FETCH_MAIL_REWARD_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public  uint		nMailID3;
	}
	struct CTS_GAMECMD_OPERATE_UPLOAD_BILL_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	uint		nClientTime3;
		public  int			nTextLen4;
		public  byte[]		szText5;
	}
	struct CTS_GAMECMD_OPERATE_REGISTER_ACCOUNT_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public  const int	len3 = 32;
		public  byte[]		szAccount4;
		public  int			nPassLen5;
		public  byte[]		szPass6;
	// 1.修改密码则后面跟着老密码串 
	// 2.注册则将nOldPassLen设置为0 
		public 	int			nOldPassLen7;
		public  byte[]		szOldPass8;
	}
	struct CTS_GAMECMD_OPERATE_BIND_DEVICE_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public const int 	len3 = 64;
		public  byte[]		szMAC4;
	}
	struct CTS_GAMECMD_RECHARGE_TRY_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public	int			nDiamond3;
	}
	struct CTS_GAMECMD_GET_SERVER_TIME_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_OPERATE_DISMISS_SOLDIER_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public  uint		nExcelID3;
		public  uint		nLevel4;
		public  uint		nNum5;
	}
	struct CTS_GAMECMD_GET_DONATE_SOLDIER_QUEUE_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_GET_LEFT_DONATE_SOLDIER_T
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_OPERATE_ALLI_DONATE_SOLDIER_T  
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nObjID3;
		public 	uint		nExcelID4;
		public 	uint		nLevel5;
		public 	uint		nNum6;
	}
	struct CTS_GAMECMD_OPERATE_ALLI_RECALL_SOLDIER_T  
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nEventID3;
	}
	struct CTS_GAMECMD_OPERATE_ALLI_ACCE_SOLDIER_T  
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nEventID3;
		public  uint		nTime4;
		public  uint		nMoneyType5;
	}
	struct CTS_GAMECMD_GET_GOLD_DEAL_T  
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public  uint		nPage3;
		public  uint		nPageSize4;
	}
	struct CTS_GAMECMD_GET_SELF_GOLD_DEAL_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
	struct CTS_GAMECMD_OPERATE_ADD_GOLD_DEAL_T  
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public  uint		nGold3;
		public  uint		nPrice4;			// 单价 
	}
	struct CTS_GAMECMD_OPERATE_BUY_GOLD_IN_MARKET_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public ulong		nTransactionID3;
		public  uint		nMoneyType4;
	}
	struct CTS_GAMECMD_OPERATE_CANCEL_GOLD_DEAL_T  
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public  ulong		nTransactionID3;
	}
	struct CTS_GAMECMD_GET_WORLD_RES_CARD_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
		public 	ulong		nID3;
	}
	struct CTS_GAMECMD_GET_KICK_CLIENT_ALL_T 
	{
		public 	byte		nCmd1;
		public  uint		nGameCmd2;
	}
    /// <summary>
    /// 创建联盟副本
    /// </summary>
    struct CTS_GAMECMD_ALLI_INSTANCE_CREATE_T 
    {
        public byte nCmd1;
        public uint nGameCmd2;
    }
    /// <summary>
    /// 加入联盟副本
    /// </summary>
    struct CTS_GAMECMD_ALLI_INSTANCE_JOIN_T 
    {
        public byte nCmd1;
        public uint nGameCmd2;
	    public ulong nInstanceID3;
    }
    /// <summary>
    /// 退出联盟副本
    /// </summary>
    struct CTS_GAMECMD_ALLI_INSTANCE_EXIT_T 
    {
        public byte nCmd1;
        public uint nGameCmd2;
	    public ulong nInstanceID3;
    }
    /// <summary>
    /// t出联盟副本
    /// </summary>
    struct CTS_GAMECMD_ALLI_INSTANCE_KICK_T 
    {
        public byte nCmd1;
        public uint nGameCmd2;
	    public ulong nInstanceID3;
	    public ulong nObjID4;
    }
    /// <summary>
    /// 解散联盟副本
    /// </summary>
    struct CTS_GAMECMD_ALLI_INSTANCE_DESTROY_T 
    {
        public byte nCmd1;
        public uint nGameCmd2;
	    public ulong nInstanceID3;
    }
    /// <summary>
    /// 联盟副本中君主设定将领
    /// </summary>
    struct CTS_GAMECMD_ALLI_INSTANCE_CONFIG_HERO_T
    {
        public byte nCmd1;
        public uint nGameCmd2;
	    public ulong nInstanceID3;
	    public ulong n1HeroID4;
	    public ulong n2HeroID5;
	    public ulong n3HeroID6;
	    public ulong n4HeroID7;
	    public ulong n5HeroID8;		
    }
    /// <summary>
    /// 联盟副本开战
    /// </summary>
    struct CTS_GAMECMD_ALLI_INSTANCE_START_T 
    {
        public byte nCmd1;
        public uint nGameCmd2;
	    public ulong nInstanceID3;
    }

    struct CTS_GAMECMD_ALLI_INSTANCE_READY_T 
    {
        public byte nCmd1;
        public uint nGameCmd2;
	    public ulong nInstanceID3;
	    public byte	bReady4;
    }
    /// <summary>
    /// 联盟副本中获取所有队伍列表 
    /// </summary>
    struct CTS_GAMECMD_ALLI_INSTANCE_GET_LIST_T 
    {
        public byte nCmd1;
        public uint nGameCmd2;
    }
    /// <summary>
    /// 联盟副本中获取君主信息 
    /// </summary>
    struct CTS_GAMECMD_ALLI_INSTANCE_GET_CHAR_DATA_T 
    {
        public byte nCmd1;
        public uint nGameCmd2;
        public ulong nInstanceID3;
    }
    /// <summary>
    /// 联盟副本中获取将领信息 
    /// </summary>
    struct CTS_GAMECMD_ALLI_INSTANCE_GET_HERO_DATA_T 
    {
        public byte nCmd1;
        public uint nGameCmd2;
        public ulong nInstanceID3;
    }
    /// <summary>
    /// 联盟副本中获取副本状态 
    /// </summary>
    struct CTS_GAMECMD_ALLI_INSTANCE_GET_STATUS_T 
    {
        public byte nCmd1;
        public uint nGameCmd2;
        public uint nCombatType3;
    }
    /// <summary>
    /// 联盟副本中保存将领位置 
    /// </summary>
    struct CTS_GAMECMD_ALLI_INSTANCE_SAVE_HERO_DEPLOY_T 
    {
        public byte nCmd1;
        public uint nGameCmd2;
	    public int	nNum3;
        public byte[] data4;
    };

    struct HeroDeploy
    {
		public uint	nCol1;
		public uint	nRow2;
		public ulong nHeroID3;
    }

    struct CTS_GAMECMD_ALLI_INSTANCE_GET_LOOT_T
    {
        public byte nCmd1;
        public uint nGameCmd2;
        public ulong nInstanceID3;
    }

    /// <summary>
    /// 开始每场战斗  
    /// </summary>
    struct CTS_GAMECMD_ALLI_INSTANCE_START_COMBAT_T 
    {
        public byte nCmd1;
        public uint nGameCmd2;
        public byte	bAutoCombat3;
	    public byte	bAutoSupply4;
	    public uint	nStopLevel5;//停止关卡 
	    public ulong nObjID6;// 目标ID可以是副本或者人  
	    public uint	nCombatType7;// 战斗类型  
	    public int nNum8;
        public byte[] nHeroData9;
    }

    public struct CTS_HeroDeploy 
    {
        public uint	nCol1;
        public uint	nRow2;
        public ulong nHeroID3;
    };

    /// <summary>
    /// 简单战报
    /// </summary>
    struct CTS_GAMECMD_ALLI_INSTANCE_GET_SIMPLE_COMBAT_LOG_T {
        public byte nCmd1;
        public uint nGameCmd2;
        public uint	nCombatType3;
	    public ulong nInstanceID4;
    }

    /// <summary>
    /// 复杂战报
    /// </summary>
    struct CTS_GAMECMD_ALLI_INSTANCE_GET_COMBAT_LOG_T {
        public byte nCmd1;
        public uint nGameCmd2;
        public uint	nCombatType3;
	    public ulong nInstanceID4;
    }

    /// <summary>
    /// 停止自动战斗 
    /// </summary>
    public struct CTS_GAMECMD_ALLI_INSTANCE_STOP_COMBAT_T
    {
        public byte nCmd1;
        public uint nGameCmd2;
	    public uint nCombatType3;
	    public ulong nInstanceID4;
    }

    /// <summary>
    /// 手动补兵 
    /// </summary>
    public struct CTS_GAMECMD_ALLI_INSTANCE_SUPPLY_T {
        public byte nCmd1;
        public uint nGameCmd2;
        public uint nCombatType3;
        public ulong nInstanceID4;
    }
	struct CTS_GAMECMD_ARENA_UPLOAD_DATA_T 
	{
		public byte nCmd1;
        public uint nGameCmd2;
		public int	nSize3;
		public byte[] Data4;
	}
	struct CTS_GAMECMD_ARENA_GET_RANK_LIST_T 
	{
		public byte 	nCmd1; 
        public uint 	nGameCmd2; 
		public uint		nPageSize3; 
		public uint		nPage4; 
	}
	struct CTS_GAMECMD_ARENA_GET_STATUS_T  
	{
		public byte 	nCmd1; 
        public uint 	nGameCmd2;  
		// 获取的挑战列表长度  
		public uint 	nNum3; 
	}
	struct CTS_GAMECMD_ARENA_PAY_T 
	{
		public byte 	nCmd1; 
        public uint 	nGameCmd2;  
		public uint 	nMoneyType3;  
		public uint 	nNum4;
	}
	struct CTS_GAMECMD_ARENA_GET_DEPLOY_T 
	{
		public byte 	nCmd1; 
        public uint 	nGameCmd2;  
	}
	struct CTS_GAMECMD_HERO_SUPPLY_T 
	{
		public byte 	nCmd1; 
        public uint 	nGameCmd2;  
		public uint		nCombatType3;
		public int		nNum4;
		public ulong[]  HeroIDs5;    
		// nNum*HeroID 
	}
	struct CTS_GAMECMD_OPERATE_SET_VIP_DISPLAY_T 
	{
		public byte 	nCmd1; 
        public uint 	nGameCmd2; 
		public byte 	bVipDisplay3;
	}
	struct CTS_GAMECMD_ALLI_INSTANCE_GET_DEFENDER_DEPLOY_T  
	{
		public byte 	nCmd1; 
        public uint 	nGameCmd2;
		public uint		nExcelID3;			// 副本ID 
		public uint		nLevel4;
	}
	
	// 位置收藏夹 
	struct CTS_GAMECMD_POSITION_MARK_ADD_RECORD_T 
	{
		public byte 	nCmd1; 
        public uint 	nGameCmd2;
		public uint		nPosX3;
		public uint		nPosY4;
		public uint		nHeadID5;
		public int		nLength6;
		public byte[]   szName7;
	}
	struct CTS_GAMECMD_POSITION_MARK_CHG_RECORD_T  	
	{
		public byte 	nCmd1; 
        public uint 	nGameCmd2;
		public uint		nPosX3;
		public uint		nPosY4;
		public uint		nHeadID5;
		public int		nLength6;
		public byte[]   szName7;
	}	
	struct CTS_GAMECMD_POSITION_MARK_DEL_RECORD_T  	
	{
		public byte 	nCmd1; 
        public uint 	nGameCmd2;
		public uint 	nPosX3;
		public uint 	nPosY4;
	}
	struct CTS_GAMECMD_POSITION_MARK_GET_RECORD_T  
	{
		public byte 	nCmd1; 
        public uint 	nGameCmd2;
	}
}