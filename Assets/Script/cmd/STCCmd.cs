using UnityEngine;
using System.Collections;

namespace STCCMD
{
	enum STC_MSG
	{
		CAAFS4Web_CLIENT_CAAFS4WebINFO = 91,
		CAAFS4Web_CLIENT_QUEUEINFO=92,
		CAAFS4Web_CLIENT_GOTOCLS4Web = 93,
			
	}
	enum TTY_STC_MSG
	{
		// 登录结果 
		TTY_LPGAMEPLAY_CLIENT_LOGIN_RST			= 0x03,
		// 创建角色 
		TTY_LPGAMEPLAY_CLIENT_CREATECHAR_RST	= 0x04,
		// 创建账号 
		TTY_LPGAMEPLAY_CLIENT_CREATEACCOUNT_RST	= 0x05,
		// 游戏指令 
		TTY_LPGAMEPLAY_CLIENT_GAME_CMD			= 0x06,
		// 登出返回 
		TTY_LPGAMEPLAY_CLIENT_LOGOUT_RST		= 0x07,
		// 通知 
		TTY_LPGAMEPLAY_CLIENT_NOTIFY			= 0x20,
	}
	enum LOGIN_RST
	{
		TTY_LOGIN_RST_SUCCEED						= 0x01,	
		TTY_LOGIN_RST_BADACCOUNT			= 0x02,
		TTY_LOGIN_RST_BADPASS						= 0x03,	
		TTY_LOGIN_RST_ALREADYINTHIS		= 0x04,
		TTY_LOGIN_RST_ALREADYINOTHER	= 0x05,	
	}
	enum CHAR_RST
	{
		TTY_CHARCMD_GETCHARINFO_SMP_RST				= 0x00,
		TTY_CHARCMD_GETALLCHARINFO_SMP_RST		= 0X01,
		TTY_CHARCMD_SELECT_RST											= 0x04,
	}
	enum CHAR_SUB_RST
	{
		TTY_CHARCMD_RST_OK							= 0x00,			
	}
	enum GAME_SUB_RST
	{
		////////////////////////////////////////////////////////////////////////// 
		// 聊天相关 
		////////////////////////////////////////////////////////////////////////// 
		STC_GAMECMD_CHAT_BEGIN					= 0,
		STC_GAMECMD_CHAT_PRIVATE				= STC_GAMECMD_CHAT_BEGIN + 1,		// 私聊 
		STC_GAMECMD_CHAT_ALLIANCE				= STC_GAMECMD_CHAT_BEGIN + 2,		// 联盟  
		STC_GAMECMD_CHAT_WORLD					= STC_GAMECMD_CHAT_BEGIN + 3,		// 世界  
		STC_GAMECMD_CHAT_GROUP					= STC_GAMECMD_CHAT_BEGIN + 5,		// 组队(多人副本) 
		STC_GAMECMD_SYS_REPORT					= STC_GAMECMD_CHAT_BEGIN + 10,		// 系统
		STC_GAMECMD_CHAT_END					= STC_GAMECMD_CHAT_BEGIN + 20, 
	
		////////////////////////////////////////////////////////////////////////// 
		// 查询相关 
		////////////////////////////////////////////////////////////////////////// 
		STC_GAMECMD_GET_BEGIN					= STC_GAMECMD_CHAT_END,
		STC_GAMECMD_GET_NOTIFICATION			= STC_GAMECMD_GET_BEGIN + 1,		// 查看通知 
		STC_GAMECMD_GET_TERRAIN					= STC_GAMECMD_GET_BEGIN + 2,
		STC_GAMECMD_GET_PLAYERCARD				= STC_GAMECMD_GET_BEGIN + 3,
		STC_GAMECMD_GET_TILE_INFO				= STC_GAMECMD_GET_BEGIN + 7,		// 获取tile信息 
		STC_GAMECMD_GET_TILE_NONE				= STC_GAMECMD_GET_BEGIN + 8,		// 获取tile信息失败 
		STC_GAMECMD_GET_BUILDING_LIST			= STC_GAMECMD_GET_BEGIN + 9,		// 获取建筑列表 
		STC_GAMECMD_GET_BUILDING_TE_LIST		= STC_GAMECMD_GET_BEGIN + 10,		// 获取建筑时间事件列表 
		STC_GAMECMD_GET_PRODUCTION_EVENT		= STC_GAMECMD_GET_BEGIN + 11,		// 获取生产事件 
		STC_GAMECMD_GET_PRODUCTION_TE_LIST		= STC_GAMECMD_GET_BEGIN + 12,		// 获取生产时间事件 
		STC_GAMECMD_GET_TECHNOLOGY				= STC_GAMECMD_GET_BEGIN + 13,		// 获取科技  
		STC_GAMECMD_GET_RESEARCH_TE				= STC_GAMECMD_GET_BEGIN + 14,		// 获取科技研究队列  
		STC_GAMECMD_GET_SOLDIER					= STC_GAMECMD_GET_BEGIN + 15,		// 获取所有军队  
		STC_GAMECMD_GET_ALLIANCE_INFO			= STC_GAMECMD_GET_BEGIN + 16,		// 获取联盟基本信息 
		STC_GAMECMD_GET_ALLIANCE_MEMBER			= STC_GAMECMD_GET_BEGIN + 17,		// 获取联盟成员列表 
		STC_GAMECMD_GET_ALLIANCE_BUILDING_TE	= STC_GAMECMD_GET_BEGIN + 18,		// 获取联盟建筑时间事件列表 
		STC_GAMECMD_GET_ALLIANCE_BUILDING		= STC_GAMECMD_GET_BEGIN + 19,		// 获取联盟建筑列表 
		STC_GAMECMD_GET_ALLIANCE_JOIN_EVENT		= STC_GAMECMD_GET_BEGIN + 20,		// 获取联盟申请成员列表 
		STC_GAMECMD_GET_CHAR_ATB				= STC_GAMECMD_GET_BEGIN + 21,		// 获取角色基本数据 
		STC_GAMECMD_GET_MY_FRIEND_APPLY			= STC_GAMECMD_GET_BEGIN + 22,		// 获取自己发出的好友申请 
		STC_GAMECMD_GET_OTHERS_FRIEND_APPLY		= STC_GAMECMD_GET_BEGIN + 23,		// 获取自己收到的好友申请(别人发给自己的) 
		STC_GAMECMD_GET_FRIEND_LIST				= STC_GAMECMD_GET_BEGIN + 24,		// 获取好友列表 
		STC_GAMECMD_GET_WORLD_AREA_INFO_TERRAIN	= STC_GAMECMD_GET_BEGIN	+ 25,		// 获取世界地图某块的信息(地理信息) 
		STC_GAMECMD_GET_WORLD_AREA_INFO_CITY	= STC_GAMECMD_GET_BEGIN	+ 26,		// 获取世界地图某块的信息(城市信息) 
		STC_GAMECMD_GET_ALLIANCE_MAIL			= STC_GAMECMD_GET_BEGIN + 27,		// 获取联盟邮件 
		STC_GAMECMD_GET_ALLIANCE_LOG			= STC_GAMECMD_GET_BEGIN + 28,		// 获取联盟日志 
		STC_GAMECMD_GET_PRIVATE_LOG				= STC_GAMECMD_GET_BEGIN + 29,		// 获取私人日志  
		STC_GAMECMD_GET_RELATION_LOG			= STC_GAMECMD_GET_BEGIN + 30,		// 获取关系日志,附加account_id 
		STC_GAMECMD_GET_TRAINING_TE				= STC_GAMECMD_GET_BEGIN + 31,		// 训练时间队列  
		STC_GAMECMD_GET_PLAYERCARD_NAME			= STC_GAMECMD_GET_BEGIN + 32,		// 通过角色名获取名片信息 
		STC_GAMECMD_GET_MY_ALLIANCE_JOIN_EVENT	= STC_GAMECMD_GET_BEGIN + 33,		// 获取我的联盟申请记录 
		STC_GAMECMD_GET_ALLIANCE_CARD_BY_ID		= STC_GAMECMD_GET_BEGIN + 34,		// 根据联盟ID获取联盟卡片 
		STC_GAMECMD_GET_ALLIANCE_CARD_BY_RANK	= STC_GAMECMD_GET_BEGIN + 35,		// 根据联盟排名获取联盟卡片  
		STC_GAMECMD_GET_OTHER_GOLDORE_INFO 		= STC_GAMECMD_GET_BEGIN + 36,		// 获取别人的金矿信息 
		STC_GAMECMD_GET_CHRISTMAS_TREE_INFO		= STC_GAMECMD_GET_BEGIN + 41,		// 获取自己的许愿树状态 
		STC_GAMECMD_GET_GOLDORE_SMP_INFO_ALL	= STC_GAMECMD_GET_BEGIN + 42,		// 获取一群人的金矿是否有事件发生  
		STC_GAMECMD_GET_ALLIANCE_TRADE_INFO		= STC_GAMECMD_GET_BEGIN + 43,		// 获取联盟跑商状态 
		STC_GAMECMD_GET_ENEMY_LIST				= STC_GAMECMD_GET_BEGIN + 44,		// 获取仇人列表 
		STC_GAMECMD_GET_RANK_LIST				= STC_GAMECMD_GET_BEGIN + 45,		// 获取排行榜信息 
		STC_GAMECMD_GET_MY_RANK					= STC_GAMECMD_GET_BEGIN + 46,		// 获取我的排名 
		STC_GAMECMD_GET_IMPORTANT_CHAR_ATB		= STC_GAMECMD_GET_BEGIN + 47,		// 获取重要的角色属性 
		STC_GAMECMD_GET_PAY_SERIAL				= STC_GAMECMD_GET_BEGIN + 48,		// 获取交易序列号 
		STC_GAMECMD_GET_SERVER_TIME				= STC_GAMECMD_GET_BEGIN + 50,		// 获取服务器时间 
		STC_GAMECMD_GET_DONATE_SOLDIER_QUEUE	= STC_GAMECMD_GET_BEGIN + 51,		// 获取盟友送兵队列 
		STC_GAMECMD_GET_LEFT_DONATE_SOLDIER		= STC_GAMECMD_GET_BEGIN + 52,		// 获取今日剩余可送兵数量 
		STC_GAMECMD_GET_GOLD_DEAL				= STC_GAMECMD_GET_BEGIN + 53,		// 获取交易列表  
		STC_GAMECMD_GET_SELF_GOLD_DEAL			= STC_GAMECMD_GET_BEGIN + 54,		// 获取自己的交易列表  
		STC_GAMECMD_GET_WOLRD_RES				= STC_GAMECMD_GET_BEGIN + 55,		// 获取世界资源 
		STC_GAMECMD_GET_WORLD_RES_CARD			= STC_GAMECMD_GET_BEGIN + 56,		// 获取世界资源详细信息 
		STC_GAMECMD_GET_KICK_CLIENT_ALL			= STC_GAMECMD_GET_BEGIN + 57,		// 获取踢出用户字串,version*appid 
		STC_GAMECMD_GET_END						= STC_GAMECMD_GET_BEGIN	+ 100,
	
		////////////////////////////////////////////////////////////////////////// 
		// 发送邮件的返回值 
		////////////////////////////////////////////////////////////////////////// 
		STC_GAMECMD_ADD_MAIL					= STC_GAMECMD_GET_END + 1,			// 发送邮件 
		STC_GAMECMD_GET_MAIL					= STC_GAMECMD_GET_END + 2,			// 获取邮件列表 
		STC_GAMECMD_READ_MAIL					= STC_GAMECMD_GET_END + 3,			// 阅读邮件 
		STC_GAMECMD_DELETE_MAIL					= STC_GAMECMD_GET_END + 4,			// 删除邮件 
		STC_GAMECMD_ADD_MESSAGE_N				= STC_GAMECMD_GET_END + 5,			// 向N个人发送同一封邮件 
		STC_GAMECMD_GET_MAIL_1					= STC_GAMECMD_GET_END + 6,			// 获取一封邮件 
		STC_GAMECMD_NEW_PRIVATE_MAIL			= STC_GAMECMD_GET_END + 7,			// 新的邮件到来 
		STC_GAMECMD_ADD_MAIL_WITH_EXT_DATA		= STC_GAMECMD_GET_END + 8,			// 发送带附件的邮件(这个是用于测试的指令) 
		STC_GAMECMD_READ_MAIL_ALL				= STC_GAMECMD_GET_END + 9,			// 将所有邮件标记为已读 
		STC_GAMECMD_DELETE_MAIL_ALL				= STC_GAMECMD_GET_END + 10,			// 删除所有邮件 
		STC_GAMECMD_MAIL_END					= STC_GAMECMD_GET_END + 10,
	
		////////////////////////////////////////////////////////////////////////// 
		// 一些操作 
		////////////////////////////////////////////////////////////////////////// 
		STC_GAMECMD_OPERATE_BEGIN				= STC_GAMECMD_MAIL_END + 0,
		STC_GAMECMD_OPERATE_BUILD_BUILDING		= STC_GAMECMD_OPERATE_BEGIN + 1,	// 建造建筑 
		STC_GAMECMD_OPERATE_UPGRADE_BUILDING	= STC_GAMECMD_OPERATE_BEGIN + 2,	// 升级建筑 
		STC_GAMECMD_OPERATE_PRODUCE_GOLD		= STC_GAMECMD_OPERATE_BEGIN + 3,	// 生产金子 
		STC_GAMECMD_OPERATE_FETCH_GOLD			= STC_GAMECMD_OPERATE_BEGIN + 4,	// 采摘金子 
		STC_GAMECMD_OPERATE_ACCE_BUILDING		= STC_GAMECMD_OPERATE_BEGIN + 5,	// 加速建筑(升级/建造) 
		STC_GAMECMD_OPERATE_ACCE_GOLD_PRODUCE	= STC_GAMECMD_OPERATE_BEGIN + 6,	// 加速黄金生产 
		STC_GAMECMD_OPERATE_RESEARCH			= STC_GAMECMD_OPERATE_BEGIN + 7,	// 研究科技 
		STC_GAMECMD_OPERATE_CONSCRIPT_SOLDIER	= STC_GAMECMD_OPERATE_BEGIN + 8,	// 征召士兵 
		STC_GAMECMD_OPERATE_UPGRADE_SOLDIER		= STC_GAMECMD_OPERATE_BEGIN + 9,	// 升级士兵 
		STC_GAMECMD_OPERATE_ACCE_RESEARCH		= STC_GAMECMD_OPERATE_BEGIN + 10,	// 加速科技研究 
		STC_GAMECMD_OPERATE_ACCEPT_ALLI_MEMBER	= STC_GAMECMD_OPERATE_BEGIN + 11,	// 接受新成员加入联盟 
		STC_GAMECMD_OPERATE_BUILD_ALLI_BUILDING	= STC_GAMECMD_OPERATE_BEGIN + 12,	// 建造联盟建筑 
		STC_GAMECMD_OPERATE_CANCEL_JOIN_ALLI	= STC_GAMECMD_OPERATE_BEGIN + 13,	// 取消联盟加入申请 
		STC_GAMECMD_OPERATE_CREATE_ALLI			= STC_GAMECMD_OPERATE_BEGIN + 14,	// 创建联盟 
		STC_GAMECMD_OPERATE_DISMISS_ALLI		= STC_GAMECMD_OPERATE_BEGIN + 15,	// 解散联盟 
		STC_GAMECMD_OPERATE_EXIT_ALLI			= STC_GAMECMD_OPERATE_BEGIN + 16,	// 退出联盟 
		STC_GAMECMD_OPERATE_EXPEL_ALLI_MEMBER	= STC_GAMECMD_OPERATE_BEGIN + 17,	// 开除联盟成员 
		STC_GAMECMD_OPERATE_JOIN_ALLI			= STC_GAMECMD_OPERATE_BEGIN + 18,	// 申请加入联盟 
		STC_GAMECMD_OPERATE_SET_ALLI_POSITION	= STC_GAMECMD_OPERATE_BEGIN + 19,	// 设置成员职位 
		STC_GAMECMD_OPERATE_REFUSE_JOIN_ALLI	= STC_GAMECMD_OPERATE_BEGIN + 20,	// 拒绝加入联盟申请 
		STC_GAMECMD_OPERATE_ABDICATE_ALLI		= STC_GAMECMD_OPERATE_BEGIN + 21,	// 禅让盟主 
		STC_GAMECMD_OPERATE_APPROVE_FRIEND		= STC_GAMECMD_OPERATE_BEGIN + 22,	// 允许成为好友 
		STC_GAMECMD_OPERATE_APPLY_FRIEND		= STC_GAMECMD_OPERATE_BEGIN + 23,	// 申请成为好友 
		STC_GAMECMD_OPERATE_REFUSE_FRIEND_APPLY	= STC_GAMECMD_OPERATE_BEGIN + 24,	// 拒绝对方的好友申请 
		STC_GAMECMD_OPERATE_CANCEL_FRIEND_APPLY	= STC_GAMECMD_OPERATE_BEGIN + 25,	// 取消自己发出的好友申请 
		STC_GAMECMD_OPERATE_DELETE_FRIEND		= STC_GAMECMD_OPERATE_BEGIN + 26,	// 删除好友 
		STC_GAMECMD_OPERATE_ADD_ALLIANCE_MAIL	= STC_GAMECMD_OPERATE_BEGIN + 27,	// 添加联盟邮件 
		STC_GAMECMD_OPERATE_BUY_ITEM			= STC_GAMECMD_OPERATE_BEGIN + 28,	// 通过钻石购买道具  
		STC_GAMECMD_OPERATE_SELL_ITEM			= STC_GAMECMD_OPERATE_BEGIN + 30,	// 出售道具(出售道具也走这个接口)  
		STC_GAMECMD_OPERATE_FETCH_CHRISTMAS_TREE	= STC_GAMECMD_OPERATE_BEGIN + 31,	// 收获许愿树 
		STC_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE	= STC_GAMECMD_OPERATE_BEGIN + 32,	// 许愿树浇水 
		STC_GAMECMD_OPERATE_STEAL_GOLD			= STC_GAMECMD_OPERATE_BEGIN + 33,	// 偷窃黄金 
		STC_GAMECMD_OPERATE_ADD_TRAINING		= STC_GAMECMD_OPERATE_BEGIN + 34,	// 将领训练 
		STC_GAMECMD_OPERATE_EXIT_TRAINING		= STC_GAMECMD_OPERATE_BEGIN + 35,	// 退出训练 
		STC_GAMECMD_OPERATE_CONTRIBUTE_ALLIANCE	= STC_GAMECMD_OPERATE_BEGIN + 36,	// 联盟捐献 
		STC_GAMECMD_OPERATE_CONVERT_DIAMOND_GOLD= STC_GAMECMD_OPERATE_BEGIN	+ 37,	// 钻石兑换黄金 
		STC_GAMECMD_OPERATE_JOIN_ALLI_NAME		= STC_GAMECMD_OPERATE_BEGIN + 38,	// 通过名字申请加入联盟 
		STC_GAMECMD_OPERATE_SET_ALLI_INTRO		= STC_GAMECMD_OPERATE_BEGIN + 39,	// 设置联盟的简介 
		STC_GAMECMD_OPERATE_DRAW_LOTTERY		= STC_GAMECMD_OPERATE_BEGIN + 40,	// 抽奖 
		STC_GAMECMD_OPERATE_CHECK_DRAW_LOTTERY	= STC_GAMECMD_OPERATE_BEGIN + 41,	// 抽奖条件校验  
		STC_GAMECMD_OPERATE_FETCH_LOTTERY		= STC_GAMECMD_OPERATE_BEGIN + 42,	// 获取抽奖结果  
		STC_GAMECMD_OPERATE_ALLIANCE_TRADE		= STC_GAMECMD_OPERATE_BEGIN + 43,	// 联盟跑商 
		STC_GAMECMD_OPERATE_DELETE_ENEMY		= STC_GAMECMD_OPERATE_BEGIN + 44,	// 删除仇人 
		STC_GAMECMD_OPERATE_WRITE_SIGNATURE		= STC_GAMECMD_OPERATE_BEGIN + 45,	// 书写个性签名 
		STC_GAMECMD_OPERATE_CHANGE_NAME			= STC_GAMECMD_OPERATE_BEGIN + 46,	// 修改名字 
		STC_GAMECMD_OPERATE_MOVE_CITY			= STC_GAMECMD_OPERATE_BEGIN + 47,	// 城市迁移 
		STC_GAMECMD_OPERATE_READ_NOTIFICATION	= STC_GAMECMD_OPERATE_BEGIN + 48,	// 阅读通知 
		STC_GAMECMD_OPERATE_CHANGE_HERO_NAME	= STC_GAMECMD_OPERATE_BEGIN + 49,	// 修改英雄名字 
		STC_GAMECMD_OPERATE_ADD_GM_MAIL			= STC_GAMECMD_OPERATE_BEGIN + 50,	// 给GM发邮件 
		STC_GAMECMD_OPERATE_SET_ALLI_NAME		= STC_GAMECMD_OPERATE_BEGIN + 51,	// 设置联盟名字 
		STC_GAMECMD_OPERATE_FETCH_MAIL_REWARD	= STC_GAMECMD_OPERATE_BEGIN + 52,	// 获取邮件中的奖励 
		STC_GAMECMD_OPERATE_UPLOAD_BILL			= STC_GAMECMD_OPERATE_BEGIN + 53,	// 客户端上传苹果账单 
		STC_GAMECMD_OPERATE_REGISTER_ACCOUNT	= STC_GAMECMD_OPERATE_BEGIN + 54,	// 注册账号(修改密码) 
		STC_GAMECMD_OPERATE_BIND_DEVICE			= STC_GAMECMD_OPERATE_BEGIN + 55,	// 绑定设备(通过设备ID登陆) 
		STC_GAMECMD_OPERATE_DISMISS_SOLDIER		= STC_GAMECMD_OPERATE_BEGIN	+ 56,	// 解散士兵 
		STC_GAMECMD_OPERATE_ALLI_DONATE_SOLDIER	= STC_GAMECMD_OPERATE_BEGIN	+ 57,	// 对盟友赠送士兵
		STC_GAMECMD_OPERATE_ALLI_RECALL_SOLDIER	= STC_GAMECMD_OPERATE_BEGIN + 58,	// 对盟友赠送士兵进行召回 
		STC_GAMECMD_OPERATE_ALLI_ACCE_SOLDIER	= STC_GAMECMD_OPERATE_BEGIN + 59,	// 对盟友赠送士兵的行军加速 
		STC_GAMECMD_OPERATE_ADD_GOLD_DEAL		= STC_GAMECMD_OPERATE_BEGIN + 60,	// 添加黄金交易 
		STC_GAMECMD_OPERATE_BUY_GOLD_IN_MARKET	= STC_GAMECMD_OPERATE_BEGIN + 61,	// 进行黄金家里  
		STC_GAMECMD_OPERATE_CANCEL_GOLD_DEAL	= STC_GAMECMD_OPERATE_BEGIN + 62,	// 取消黄金交易 
		STC_GAMECMD_OPERATE_SET_VIP_DISPLAY		= STC_GAMECMD_OPERATE_BEGIN + 63,	// 设置vip显示 
		
		STC_GAMECMD_OPERATE_TAVERN_REFRESH		= STC_GAMECMD_OPERATE_BEGIN + 101,	// 酒馆刷新 
		STC_GAMECMD_OPERATE_HIRE_HERO			= STC_GAMECMD_OPERATE_BEGIN + 102,	// 招募武将 
		STC_GAMECMD_OPERATE_FIRE_HERO			= STC_GAMECMD_OPERATE_BEGIN + 103,	// 解雇武将 
		STC_GAMECMD_OPERATE_LEVELUP_HERO		= STC_GAMECMD_OPERATE_BEGIN + 104,	// 武将升级 
		STC_GAMECMD_OPERATE_ADD_GROW			= STC_GAMECMD_OPERATE_BEGIN + 105,	// 增加成长率 
		STC_GAMECMD_OPERATE_CONFIG_HERO			= STC_GAMECMD_OPERATE_BEGIN + 106,	// 武将配兵 
		STC_GAMECMD_OPERATE_START_COMBAT		= STC_GAMECMD_OPERATE_BEGIN + 107,	// 开始战斗 
		STC_GAMECMD_OPERATE_STOP_COMBAT			= STC_GAMECMD_OPERATE_BEGIN + 108,	// 结束战斗 
		STC_GAMECMD_OPERATE_GET_COMBAT			= STC_GAMECMD_OPERATE_BEGIN + 109,	// 获取战斗数据 
		STC_GAMECMD_OPERATE_GET_HERO_REFRESH	= STC_GAMECMD_OPERATE_BEGIN + 110,	// 获取刷新武将信息 
		STC_GAMECMD_OPERATE_GET_HERO_HIRE		= STC_GAMECMD_OPERATE_BEGIN + 111,	// 获取雇佣武将信息 
		STC_GAMECMD_OPERATE_LIST_COMBAT			= STC_GAMECMD_OPERATE_BEGIN + 113,	// 获取所有战斗摘要 
		STC_GAMECMD_OPERATE_CONFIG_CITYDEFENSE	= STC_GAMECMD_OPERATE_BEGIN + 114,	// 配置城防武将 

		STC_GAMECMD_GET_ITEM					= STC_GAMECMD_OPERATE_BEGIN + 115,	// 获取道具 
		STC_GAMECMD_ADD_ITEM					= STC_GAMECMD_OPERATE_BEGIN + 116,	// 添加道具 
		STC_GAMECMD_DEL_ITEM					= STC_GAMECMD_OPERATE_BEGIN + 117,	// 删除道具 
		STC_GAMECMD_EQUIP_ITEM					= STC_GAMECMD_OPERATE_BEGIN + 118,	// 装备道具 
		STC_GAMECMD_DISEQUIP_ITEM				= STC_GAMECMD_OPERATE_BEGIN + 119,	// 卸下装备 
		STC_GAMECMD_MOUNT_ITEM					= STC_GAMECMD_OPERATE_BEGIN + 120,	// 镶嵌道具 
		STC_GAMECMD_UNMOUNT_ITEM				= STC_GAMECMD_OPERATE_BEGIN + 121,	// 卸下镶嵌 
		STC_GAMECMD_COMPOS_ITEM					= STC_GAMECMD_OPERATE_BEGIN + 122,	// 合成道具 
		STC_GAMECMD_DISCOMPOS_ITEM				= STC_GAMECMD_OPERATE_BEGIN + 123,	// 分解道具 
	 
		STC_GAMECMD_GET_QUEST					= STC_GAMECMD_OPERATE_BEGIN + 124,	// 获取任务 
		STC_GAMECMD_DONE_QUEST					= STC_GAMECMD_OPERATE_BEGIN + 125,	// 完成任务 
	
		STC_GAMECMD_GET_INSTANCESTATUS			= STC_GAMECMD_OPERATE_BEGIN + 128,	// 获取玩家副本当前状态 
		STC_GAMECMD_GET_INSTANCEDESC			= STC_GAMECMD_OPERATE_BEGIN + 129,	// 获取副本实例信息 
		STC_GAMECMD_CREATE_INSTANCE				= STC_GAMECMD_OPERATE_BEGIN + 130,	// 创建副本 
		STC_GAMECMD_JOIN_INSTANCE				= STC_GAMECMD_OPERATE_BEGIN + 131,	// 加入副本 
		STC_GAMECMD_QUIT_INSTANCE				= STC_GAMECMD_OPERATE_BEGIN + 132,	// 退出副本  
		STC_GAMECMD_DESTROY_INSTANCE			= STC_GAMECMD_OPERATE_BEGIN + 133,	// 销毁副本 
		STC_GAMECMD_GET_INSTANCEDATA			= STC_GAMECMD_OPERATE_BEGIN + 134,	// 获取副本数据 
		STC_GAMECMD_CONFIG_INSTANCEHERO			= STC_GAMECMD_OPERATE_BEGIN + 135,	// 配置副本武将 
		STC_GAMECMD_GET_INSTANCELOOT			= STC_GAMECMD_OPERATE_BEGIN + 136,	// 获取副本掉落   
		STC_GAMECMD_KICK_INSTANCE				= STC_GAMECMD_OPERATE_BEGIN + 137,	// 踢人   
		STC_GAMECMD_START_INSTANCE				= STC_GAMECMD_OPERATE_BEGIN + 138,	// 副本出征   
		STC_GAMECMD_PREPARE_INSTANCE			= STC_GAMECMD_OPERATE_BEGIN + 139,	// 副本就绪     
		
		STC_GAMECMD_EQUIP_ITEM_ALL				= STC_GAMECMD_OPERATE_BEGIN + 140,	// 一键换装 
		STC_GAMECMD_MOUNT_ITEM_ALL				= STC_GAMECMD_OPERATE_BEGIN + 141,	// 一键镶嵌 
		
		STC_GAMECMD_USE_ITEM					= STC_GAMECMD_OPERATE_BEGIN + 145,	// 使用道具  
		STC_GAMECMD_AUTO_COMBAT					= STC_GAMECMD_OPERATE_BEGIN + 146,	// 自动战斗开关  
		STC_GAMECMD_SYNC_CHAR					= STC_GAMECMD_OPERATE_BEGIN + 147,	// 同步数据，君主 
		STC_GAMECMD_SYNC_HERO					= STC_GAMECMD_OPERATE_BEGIN + 148,	// 同步数据，武将 
	
		STC_GAMECMD_AUTO_SUPPLY					= STC_GAMECMD_OPERATE_BEGIN + 149,	// 自动补给开关 
		STC_GAMECMD_GET_EQUIP					= STC_GAMECMD_OPERATE_BEGIN + 150,	// 获取已装备的装备 
		STC_GAMECMD_GET_GEM						= STC_GAMECMD_OPERATE_BEGIN + 151,	// 获取已镶嵌的宝石 
		STC_GAMECMD_MANUAL_SUPPLY				= STC_GAMECMD_OPERATE_BEGIN + 152,	// 手动补给 
		STC_GAMECMD_REPLACE_EQUIP				= STC_GAMECMD_OPERATE_BEGIN + 153,	// 替换装备 
		
		STC_GAMECMD_GET_WORLDGOLDMINE			= STC_GAMECMD_OPERATE_BEGIN + 155,	// 获取世界金矿   
		STC_GAMECMD_CONFIG_WORLDGOLDMINE_HERO	= STC_GAMECMD_OPERATE_BEGIN + 156,	// 配置金矿武将   
		STC_GAMECMD_ROB_WORLDGOLDMINE			= STC_GAMECMD_OPERATE_BEGIN + 157,	// 夺取世界金矿   
		STC_GAMECMD_DROP_WORLDGOLDMINE			= STC_GAMECMD_OPERATE_BEGIN + 158,	// 放弃世界金矿  
		STC_GAMECMD_GAIN_WORLDGOLDMINE			= STC_GAMECMD_OPERATE_BEGIN + 159,	// 收获世界金矿   
		STC_GAMECMD_MY_WORLDGOLDMINE			= STC_GAMECMD_OPERATE_BEGIN + 160,	// 我的世界金矿   

		STC_GAMECMD_COMBAT_PROF					= STC_GAMECMD_OPERATE_BEGIN + 161,	// 获取战力    
		STC_GAMECMD_SUPPLY_INSTANCE				= STC_GAMECMD_OPERATE_BEGIN + 162, 	// 副本补给     
		STC_GAMECMD_GET_CITYDEFENSE				= STC_GAMECMD_OPERATE_BEGIN + 163,	// 获取城防武将 
		STC_GAMECMD_USE_DRUG					= STC_GAMECMD_OPERATE_BEGIN + 164,	// 使用草药 
		STC_GAMECMD_ARMY_ACCELERATE				= STC_GAMECMD_OPERATE_BEGIN + 166,	// 行军加速 ...
		STC_GAMECMD_ARMY_BACK					= STC_GAMECMD_OPERATE_BEGIN + 167,	// 召回 ....
		

		STC_GAMECMD_GET_INSTANCELOOT_ADDITEM	= STC_GAMECMD_OPERATE_BEGIN + 170,	// 增加道具，副本掉落     
		STC_GAMECMD_SUPPLY_DRUG					= STC_GAMECMD_OPERATE_BEGIN + 171,	// 手动补给消耗的草药数量 .... 
		STC_GAMECMD_PVP_RST						= STC_GAMECMD_OPERATE_BEGIN + 172,	// 玩家对战结果
		STC_GAMECMD_WORLDCITY_GET				= STC_GAMECMD_OPERATE_BEGIN + 173,	// 获得当前世界名城信息 
		STC_GAMECMD_WORLDCITY_GETLOG			= STC_GAMECMD_OPERATE_BEGIN + 174,	// 获得当前世界名城奖杯战绩 
		
		STC_GAMECMD_WORLDCITY_RANK_MAN			= STC_GAMECMD_OPERATE_BEGIN + 175,	// 获得当前奖杯个人排名 
		STC_GAMECMD_WORLDCITY_RANK_ALLIANCE		= STC_GAMECMD_OPERATE_BEGIN + 176,	// 获得当前奖杯联盟排名 
		STC_GAMECMD_WORLDCITY_GETLOG_ALLIANCE	= STC_GAMECMD_OPERATE_BEGIN + 177,	// 获得上次世界名城奖杯战绩联盟 
		STC_GAMECMD_OPERATE_END					= STC_GAMECMD_OPERATE_BEGIN + 200,
	
		////////////////////////////////////////////////////////////////////////// 
		// 服务器给客户端的一些通知 
		////////////////////////////////////////////////////////////////////////// 
		STC_GAMECMD_NOTIFY_BEGIN				= STC_GAMECMD_OPERATE_END + 0,
		STC_GAMECMD_POPULATION_ADD				= STC_GAMECMD_NOTIFY_BEGIN + 1,		// 增加了新的人口 
		STC_GAMECMD_GOLD_CAN_FETCH				= STC_GAMECMD_NOTIFY_BEGIN + 2,		// 金子已经成熟 
		STC_GAMECMD_BUILDING_TE					= STC_GAMECMD_NOTIFY_BEGIN + 3,		// 建筑时间事件通知 
		STC_GAMECMD_RESEARCH_TE					= STC_GAMECMD_NOTIFY_BEGIN + 4,		// 研究时间事件通知 
		STC_GAMECMD_CHAR_IS_ONLINE				= STC_GAMECMD_NOTIFY_BEGIN + 5,		// 查询某玩家是否在线 
		STC_GAMECMD_CHRISTMAS_TREE_READY_RIPE	= STC_GAMECMD_NOTIFY_BEGIN + 6,		// 许愿树准备成熟 
		STC_GAMECMD_CHRISTMAS_TREE_RIPE			= STC_GAMECMD_NOTIFY_BEGIN + 7,		// 许愿树成熟 
		STC_GAMECMD_NEW_ALLIANCE_MAIL			= STC_GAMECMD_NOTIFY_BEGIN + 8,		// 新的联盟邮件 
		STC_GAMECMD_TRAINING_OVER				= STC_GAMECMD_NOTIFY_BEGIN + 9,		// 训练结束通知 
		STC_GAMECMD_NEW_FRIEND_APPLY			= STC_GAMECMD_NOTIFY_BEGIN + 10,	// 新的好友申请到来 
		STC_GAMECMD_IN_ALLIANCE					= STC_GAMECMD_NOTIFY_BEGIN + 11,	// 你被允许加入联盟通知 
		STC_GAMECMD_NEW_ALLIANCE_JOIN			= STC_GAMECMD_NOTIFY_BEGIN + 12,	// 新的申请通知 
		STC_GAMECMD_NEW_ENEMY					= STC_GAMECMD_NOTIFY_BEGIN + 13,	// 新的敌人 
		STC_GAMECMD_NEW_NOTIFICATION			= STC_GAMECMD_NOTIFY_BEGIN + 14,	// 新的通知 
		STC_GAMECMD_RECHARGE_DIAMOND			= STC_GAMECMD_NOTIFY_BEGIN + 15,	// 充值成功通知 
		STC_GAMECMD_BUILD_ITEM_USE_OUT			= STC_GAMECMD_NOTIFY_BEGIN + 16,	// 工匠之书使用结束，建筑队列变化 
		STC_GAMECMD_EXPELED_ALLI				= STC_GAMECMD_NOTIFY_BEGIN + 17,	// 被开除出联盟 
		STC_GAMECMD_NEW_ALLIANCE_POSITION		= STC_GAMECMD_NOTIFY_BEGIN + 18,	// 被设置了新的职位 
		STC_GAMECMD_ADCOLONY_AWARD				= STC_GAMECMD_NOTIFY_BEGIN + 19,	// 观看AdColony的奖励 
		STC_GAMECMD_DONATE_SOLDIER_TE			= STC_GAMECMD_NOTIFY_BEGIN + 20,	// 联盟赠送士兵结束事件 
		STC_GAMECMD_WORLD_RES_CLEAR				= STC_GAMECMD_NOTIFY_BEGIN + 21,	// 世界资源被清理 
        STC_GAMECMD_HERO_SIMPLE_DATA            = STC_GAMECMD_NOTIFY_BEGIN + 22,    //将领的简单信息
		STC_GAMECMD_COMBAT_RST					= STC_GAMECMD_NOTIFY_BEGIN + 51,	// 战斗结果 
		STC_GAMECMD_NOTIFY_END					= STC_GAMECMD_NOTIFY_BEGIN + 200,

        //////////////////////////////////////////////////////////////////////////
        // 联盟副本 
        //////////////////////////////////////////////////////////////////////////
        STC_GAMECMD_ALLI_INSTANCE_BEGIN = STC_GAMECMD_NOTIFY_END,
        STC_GAMECMD_ALLI_INSTANCE_CREATE = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 1,		            // 创建副本 
        STC_GAMECMD_ALLI_INSTANCE_JOIN = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 2,		            // 加入副本 
        STC_GAMECMD_ALLI_INSTANCE_EXIT = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 3,		            // 退出副本 
        STC_GAMECMD_ALLI_INSTANCE_KICK = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 4,		            // 踢出副本 
        STC_GAMECMD_ALLI_INSTANCE_DESTROY = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 5,		        // 销毁副本 
        STC_GAMECMD_ALLI_INSTANCE_CONFIG_HERO = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 6,		    // 配置将领 
        STC_GAMECMD_ALLI_INSTANCE_START = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 7,		            // 副本出征 
        STC_GAMECMD_ALLI_INSTANCE_READY = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 8,		            // 准备就绪 
        STC_GAMECMD_ALLI_INSTANCE_GET_LIST = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 9,		        // 获取副本列表 
        STC_GAMECMD_ALLI_INSTANCE_GET_CHAR_DATA = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 10,		    // 获取副本角色详细信息 
        STC_GAMECMD_ALLI_INSTANCE_GET_HERO_DATA = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 11,		    // 获取副本将领详细信息 
        STC_GAMECMD_ALLI_INSTANCE_GET_STATUS = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 12,		    // 自己的副本状态 
        STC_GAMECMD_ALLI_INSTANCE_GET_LOOT = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 13,		        // 获取掉落 
        STC_GAMECMD_ALLI_INSTANCE_SAVE_HERO_DEPLOY = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 14,	    // 保存将领布局 
        STC_GAMECMD_ALLI_INSTANCE_START_COMBAT = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 15,		    // 开始战斗  
        STC_GAMECMD_ALLI_INSTANCE_ACTION_NOTIFY = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 16,		    // 服务器推送队伍和人员变化消息
        STC_GAMECMD_ALLI_INSTANCE_COMBAT_LOG = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 17,		    // 战报
        STC_GAMECMD_ALLI_INSTANCE_BACK = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 18,		            // 战斗返回 
        STC_GAMECMD_ALLI_INSTANCE_SIMPLE_COMBAT_LOG = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 19,     // 简单战报 
        STC_GAMECMD_ALLI_INSTANCE_STOP_COMBAT = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 20,		    // 停止战斗  
        STC_GAMECMD_ALLI_INSTANCE_SUPPLY = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 21,		        // 补给  
		STC_GAMECMD_ALLI_INSTANCE_GET_DEFENDER_DEPLOY	= STC_GAMECMD_ALLI_INSTANCE_BEGIN + 22,		// 获取军队布局 
        STC_GAMECMD_ALLI_INSTANCE_END = STC_GAMECMD_ALLI_INSTANCE_BEGIN + 100,
		
		////////////////////////////////////////////////////////////////////////// 
		// 竞技场
		////////////////////////////////////////////////////////////////////////// 
		STC_GAMECMD_ARENA_BEGIN					= STC_GAMECMD_ALLI_INSTANCE_END,
		STC_GAMECMD_ARENA_UPLOAD_DATA			= STC_GAMECMD_ARENA_BEGIN + 1,
		STC_GAMECMD_ARENA_GET_RANK_LIST			= STC_GAMECMD_ARENA_BEGIN + 2,
		STC_GAMECMD_ARENA_GET_STATUS			= STC_GAMECMD_ARENA_BEGIN + 3,
		STC_GAMECMD_ARENA_PAY					= STC_GAMECMD_ARENA_BEGIN + 4,
		STC_GAMECMD_ARENA_GET_DEPLOY			= STC_GAMECMD_ARENA_BEGIN + 5,
		STC_GAMECMD_ARENA_END					= STC_GAMECMD_ARENA_BEGIN + 100,
		
		//////////////////////////////////////////////////////////////////////////
		// 将领
		//////////////////////////////////////////////////////////////////////////
		STC_GAMECMD_HERO_BEGIN					= STC_GAMECMD_ARENA_END,
		STC_GAMECMD_HERO_SUPPLY					= STC_GAMECMD_HERO_BEGIN + 1,				// 将领补给
		STC_GAMECMD_HERO_END					= STC_GAMECMD_HERO_BEGIN + 200,
		
		////////////////////////////////////////////////////////////////////////// 
		// 位置收藏夹 
		////////////////////////////////////////////////////////////////////////// 
		STC_GAMECMD_POSITION_MARK_BEGIN			= STC_GAMECMD_HERO_END,
		STC_GAMECMD_POSITION_MARK_ADD_RECORD	= STC_GAMECMD_POSITION_MARK_BEGIN + 1,
		STC_GAMECMD_POSITION_MARK_CHG_RECORD	= STC_GAMECMD_POSITION_MARK_BEGIN + 2,
		STC_GAMECMD_POSITION_MARK_DEL_RECORD	= STC_GAMECMD_POSITION_MARK_BEGIN + 3,
		STC_GAMECMD_POSITION_MARK_GET_RECORD	= STC_GAMECMD_POSITION_MARK_BEGIN + 4,
		STC_GAMECMD_POSITION_MARK_END			= STC_GAMECMD_POSITION_MARK_BEGIN + 10,
	}
	// Use this for initialization 
	public struct CAAFS4Web_CLIENT_CAAFS4WebINFO_T 
	{
		public enum CAAFS4Web_CLIENT_CAAFS4WebINFO_T_CMD
		{
			SELF_NOTIFY_REFUSEALL		= 0x00000001,	
		}
		public byte				nCmd1;
		public byte				nVerCmpMode2;				
		public const     int   PNGS_VER_LEN3 = 32;
		public byte[]			szVer4;
		public short			nQueueSize5;
		public int				nSelfNotify6;	
		public int 				len7;
		public byte[]			info8;
	}
	public struct CAAFS4Web_CLIENT_QUEUEINFO_T 
	{
		public byte			nCmd1;
		public int				nChannel2;	
		public int				nQueueSize3;	
	}
	public struct CAAFS4Web_CLIENT_GOTOCLS4Web_T
	{
		public byte				nCmd1;
		public short			nPort2;						
		public int				IP3;						
		public int				nPassword4;				
		public int				nClientID5;	
	}
	public struct TTY_LPGAMEPLAY_CLIENT_GAME_CMD_T
	{
		public byte				nCmd1;
		public uint     		nGameCmd2;
	}
	public struct TTY_LPGAMEPLAY_CLIENT_CREATEACCOUNT_RST_T 
	{
		public enum enum_rst
		{
			TTY_ACCOUNT_CREATE_RST_OK					= 0x00,				// 创建成功 
			TTY_ACCOUNT_CREATE_RST_GS_CNTR_ERR			= 0x01,				// 和全局服的连接出错 
			TTY_ACCOUNT_CREATE_RST_DB_ERR				= 0x02,				// 数据库出错(断连等) 
			TTY_ACCOUNT_CREATE_RST_SQL_ERR				= 0x03,				// SQL执行出错 
			TTY_ACCOUNT_CREATE_RST_TIMEOUT				= 0x04,				// 超时 
			TTY_ACCOUNT_CREATE_RST_DUP					= 0x05,				// 名字重复
		}
		public byte			nCmd1;
		public int			nRst2;
		public const int 	szAccountLen3 = 32; 
		public byte[]		szAccount4;
	
	}
	public struct TTY_LPGAMEPLAY_CLIENT_CREATECHAR_RST_T 
	{
		// 创建角色结果 
		public enum enum_rst
		{
			TTY_CHAR_CREATE_RST_OK					= 0x00,				// 创建成功 
			TTY_CHAR_CREATE_RST_NAMEDUP				= 0x01,				// 名字重复 
			TTY_CHAR_CREATE_RST_DB_ERR				= 0x02,				// 数据库出错(断连等) 
			TTY_CHAR_CREATE_RST_SQL_ERR				= 0x03,				// SQL执行出错 
			TTY_CHAR_CREATE_RST_TIMEOUT				= 0x04,				// 超时 
			TTY_CHAR_CREATE_RST_ALREADYEXISTCHAR	= 0x05,				// 已经在本大区创建过角色了 
			TTY_CHAR_CREATE_RST_WORLD_FULL			= 0x06,				// 世界满了 
			TTY_CHAR_CREATE_RST_POSITION_DUP		= 0x07,				// 位置重复 
			TTY_CHAR_CREATE_RST_NAME_INVALID		= 0x08,				// 名字有非法字符 

		}
		public byte			nCmd1;
		public int			nRst2;
	}
	public struct TTY_LPGAMEPLAY_CLIENT_LOGIN_RST_T 
	{
		public enum enum_rst
		{
			TTY_LOGIN_RST_SUCCEED						= 0x01,				// login成功 
			TTY_LOGIN_RST_BADACCOUNT					= 0x02,				// 用户不存在 
			TTY_LOGIN_RST_BADPASS						= 0x03,				// 密码错误 
			TTY_LOGIN_RST_ALREADYINTHIS					= 0x04,				// 用户已在本服务器组在线  
			TTY_LOGIN_RST_ALREADYINOTHER				= 0x05,				// 用户已在其它服务器组在线 
																			// 后面会跟其他服务器组ID(tty_svrgrpid_t类型) 
			TTY_LOGIN_RST_OTHEREER						= 0x06,				// billing内部的其他错误，可能是数据库连接问题（或者是内部的存储过程错误），也可能是服务器间通讯或者服务器内部问题  
			TTY_LOGIN_RST_REFUSED						= 0x07,				// 被拒绝 
			TTY_LOGIN_RST_LOGININPROCESS				= 0x09,				// 有相同名字的帐号正在登录过程中（2007-06-15加） 
			
			TTY_LOGIN_RST_NO_DEVICEID					= 0x0A,				// 没有该DeviceID对应的账号 
			TTY_LOGIN_RST_LOGININPROCESS_DEVICEID		= 0x0B,				// 有使用该DeviceID的账号正在登陆过程中 
			//
			TTY_LOGIN_RST_NOCHAR						= 0x0C,				// 角色不存在 
			TTY_LOGIN_RST_LOADCHAR						= 0x0E,				// 载入角色信息时出错 
	
			TTY_LOGIN_RST_DBERR							= 0xF1,				// DB无法正常使用 
			TTY_LOGIN_RST_TRANSACTION_TIMEOUT			= 0xF6,				// 在transaction的某一步超时了 
			TTY_LOGIN_RST_OUTOF_TRANSACTION				= 0xF7,				// 无法分配transaction了 
			TTY_LOGIN_RST_SQLERR						= 0xFE,				// SQL错误 
			TTY_LOGIN_RST_UNKNOWNERR					= 0xFF,				// 其他未知错误（也可能由于没有删除的用户太多，无法分配了；或者是内存，或者自动数据对象流写入错误） 
		}
		public byte			nCmd1;
		public int			nRst2;
		public const int 	len3 = 64;
		public byte[]		szDeviceID4;
		public const int 	len5 = 32;
		public byte[]		szAccount6;
		public const int 	len7 = 33;
		public byte[]		szPass8;
	}
	/// <summary>
	/// 以下是逻辑二级指令 除登陆外所有的逻辑都属于二级逻辑指令 
	/// </summary>/// <summary>
	 
	struct STC_GAMECMD_CHAT_ALLIANCE_T  
	{
		public uint			nChatID1;
		public const int 	len2 = 32;
		public byte[]		szFrom3;		// 聊天串的来源   
		public ulong		nCharID4;
		public uint         nTime5;
		public uint			nVip6;
		public int			nTextLen7;
		public byte[]		szText8;
	}  
	struct STC_GAMECMD_CHAT_WORLD_T 
	{
		public uint			nChatID1;
		public const int 	len2 = 32;
		public byte[]		szFrom3;
		public ulong		nCharID4;
		public uint         nTime5;
		public uint			nVip6;
		public int			nTextLen7;
		public byte[]		szText8;
	}
	struct STC_GAMECMD_CHAT_GROUP_T 
	{
		public uint			nChatID1;
		public const int 	len2 = 32;
		public byte[]		szFrom3;
		public ulong		nCharID4;
		public uint         nTime5;
		public uint			nVip6;
		public int			nTextLen7;
		public byte[]		szText8;
	}
	struct STC_GAMECMD_CHAT_PRIVATE_T 
	{
		public uint			nChatID1;
		public const int 	len2 = 32;
		public byte[]		szFrom3;
		public ulong		nFromAccountID4;
		public const int 	len5 = 32;
		public byte[]		szTo6;
		public ulong		nToAccountID7;
		public uint			nTime8;
		public uint			nVip9;
		public int			nTextLen10;
		public byte[]		szText11;
	}
	struct STC_GAMECMD_ADD_MAIL_T 
	{ 
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_RECEIVER_NOT_EXIST	= -1,	// 接受者不存在  
			RST_SENDER_SEND_MAX		= -2,	// 发送者今天不能再发送邮件 
			RST_RECEIVER_MAIL_FULL	= -3,	// 邮箱已满  
			RST_MGSID_NOT_EXIST		= -4,	// 邮件ID不存在(SQL有问题)  
	
			// 其他返回值  
			RST_DB_ERR				= -200, 
			RST_SQL_ERR				= -201, 
			RST_TEXT_TOO_LONG		= -220, 
		}
		public 	int		nRst1; 
	}
	struct STC_GAMECMD_GET_MAIL_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0, 
	
			// 其他返回值 
			RST_DB_ERR				= -200, 
			RST_SQL_ERR				= -201, 
		} 
		public int		nRst1; 
		public int		nTotalMailNum2;
		public int      nUnreadMailNum3;
		public int		nNum4;
		
		// 后续跟随nNum个PrivateMailUnit   
	} 
	struct PrivateMailUnit 
	{
		public  uint	nMailID1;
		public  ulong	nSenderID2;
		public	byte	nType3;
		public  byte	nFlag4;
		public  byte	bReaded5;
		public 	uint	nTime6;
		public  const int len7 = 32;
		public 	byte[]	szSender8;
		public  int		nTextLen9;
		public  byte[]	szText10;
		// 后面还有两个字段  
		public 	int		nExtDataLen11;
		public 	byte[]	szExtData12;
	}
	struct STC_GAMECMD_NEW_PRIVATE_MAIL_T 
	{
		public int		nTotalMailNum1;
		// 后续跟随1个PrivateMailUnit   
	} 
	
	struct STC_GAMECMD_READ_MAIL_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
	
			// 其他返回值 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int		nRst1;
		public uint		nMailID2;
	}
	struct STC_GAMECMD_DELETE_MAIL_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
	
			// 其他返回值 
			RST_DB_ERR				= -200, 
			RST_SQL_ERR				= -201, 
		}
		public int		nRst1;
		public int		nTotalMailNum2;
		public uint 	nMailID3;
	}
	struct STC_GAMECMD_GET_MAIL_1_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NOT_EXIST			= -1,	// 不存在 
	
			// 其他返回值 
			RST_DB_ERR				= -200, 
			RST_SQL_ERR				= -201, 
		}
		public int		nRst1;
		// 后面是一个PrivateMailUnit 
	}

	struct STC_GAMECMD_QUICK_LOGIN_KEY_T 
	{
		public const int 	len1 = 33;
		public byte[]		szQuickLoginKey2;
	}
	struct STC_GAMECMD_GET_NOTIFICATION_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NOT_EXIST			= -1,
		}
		public int			nRst1;
		public int			nNum2;
	}
	struct Web_Notification_T 
	{
		public int			nGroupID1;
		public int			nNotificationID2;
		public uint			nTime3;
		public int			nTitleLen4;
		public byte[]		pszTitle5;
		public int			nContentLen6;
		public byte[]		pszContent7;
	}
	//查看别人基本信息 
	public struct STC_GAMECMD_GET_PLAYERCARD_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NOTEXIST			= 1,			// 不存在  
		}
		public int			nRst1;
		public ulong		nAccountID2;
		public const int 	len3=32;
		public byte[]		szName4;
		public uint			nLevel5;
		public uint			nVip6;
		public uint			nSex7;
		public uint			nHeadID8;
		public uint			nPosX9;
		public uint			nPosY10;
		public ulong		nAllianceID11;
		public const int 	len12=32;
		public byte[]		szAllianceName13;
		public uint			nDevelopment14;
		public const int    len15 = 129;
		public byte[]		szSignature16;						// 个性签名 
		public uint			nProtectTime17;						// 保护到期时刻 
		public uint			nLevelRank18;							// 君主等级排行 
		public uint			nCityLevel19;							// 城市等级  
		public uint			nProtectTimeEndTick20;				// 保护到期时刻 
		public uint			nInstanceWangzhe21;						// 王者之路当前最大关卡
		public uint			nInstanceZhengzhan22;						// 征战天下当前最大关卡
		public uint 		nCup23;								//圣杯数量 
		public byte			bVipDisplay24;						// 是否显示vip 
	}
	struct STC_GAMECMD_GET_TILE_INFO_T 
	{
		public  uint		nPosX1;
		public  uint		nPosY2;
		public 	ulong		nAccountID3;
		public 	const int	len4 = 32;
		public  byte[]		szName5;
		public 	ulong		nAllianceID6;
		public  const int   len7=32;
		public 	byte[]		szAllianceName8;
	}
	struct TerrainUnit
	{
		public	uint		nPosX1;
		public	uint		nPosY2;
		public	byte		canBuild3;
		public	byte		nFloor4;
		public	byte 		nFloor5;
	}
	struct CityCard
	{
		public	ulong		nAccountID1;
		public	const int 	len2=32;
		public 	byte[]		szName3;
		public 	uint		nLevel4;
		public 	uint		nVip5;
		public	ulong		nAllianceID6;
		public 	const int 	len7=32;
		public	byte[]		szAllianceName8;
		public	uint		nPosX9;
		public	uint		nPosY10;
		public	uint		nProtectTime11;
		public 	byte		bDisplayVip12;
	}
	
	struct STC_GAMECMD_GET_TILE_NONE_T 
	{
		public  uint		nPosX1;
		public  uint		nPosY2;
	}
	
	struct STC_GAMECMD_GET_BUILDING_LIST_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_DB_ERR				= -1,		// 数据库出错(断连等)  
			RST_SQL_ERR				= -2,		// SQL错误  
		}
		public 	int		nRst1;
		public  int			nNum2;
		// 后面跟nNum个BuildingUnit 
	}
	struct BuildingUnit
	{
		public  uint	nAutoID1;
		public  uint	nExcelID2;
		public  uint	nLevel3;
	}

	struct STC_GAMECMD_GET_BUILDING_TE_LIST_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_DB_ERR				= -1,		// 数据库出错(断连等) 
			RST_SQL_ERR				= -2,		// SQL错误 
		}
		public int		nRst1;
		public uint		nTimeNow2;				// 服务器当前时间 
		public int		nNum3; 
		// 后面跟nNum个BuildingTEUnit 
	}
	struct BuildingTEUnit 
	{
		public  uint	nBeginTime1;
		public  uint	nEndTime2;
		public  uint	nAutoID3;
		public  uint	nExcelID4;		// 建造新建筑时就需要autoid+excelid 
		public  uint	nType5;
	}
	struct STC_GAMECMD_GET_CHAR_ATB_T 
	{
		public ulong 		nAccountID1;						// 账号OnlyID 
		public const int 	len2 = 32;
		public byte[]		szName3;							// 角色名字 
		public uint			nLastLoginTime4;					// 登录时间 
		public uint			nLastLogoutTime5;					// 登出时间 
		public uint			nExp6;								// 当前经验值 
		public uint			nLevel7;							// 当前等级 
		public uint			nDiamond8;							// 钻石 
		public uint			nCrystal9;							// 水晶 
		public uint			nVip10;								// Vip级别 
		public uint			nSex11;								// 性别 
		public uint			nHeadID12;							// 头像ID 
		public uint			nPosX13;							// 世界X坐标 
		public uint			nPosY14;							// 世界Y坐标  
		public uint			nDevelopment15;						// 发展度  
		public uint			nGold16;							// 黄金    
		public uint			nPopulation17;						// 人口 
		public uint			nBuildNum18;						// 建造队列 
		public uint			nAddedBuildNum19;					// 附加建造队列  
		// 其他表联查结果 
		public ulong		nAllianceID20;					    // 联盟ID  
		public const int 	len21=32;
		public byte[]		szAllianceName22;					// 联盟名字  
		public uint			nDrug23;							// 草药 
		public uint			nFreeDrawLotteryNum24;				// 免费抽奖剩余次数   
		public uint			nTradeNum25;						// 今天剩余的跑商次数 
		public const int    len26 = 129;
		public byte[]		szSignature27;						// 个性签名 
		public uint			nChangeNameNum28;					// 改名次数 		
		public uint			nProtectTime29;						// 保护期剩余时间 
		public uint			nProtectTimeEndTick30;				// 保护到期时刻  
		public uint			nAddBuildNumTime31;						// 工匠之书剩余时间 
		public uint			nAddBuildNumTimeEndTick32;				// 工匠之书到期时刻 
		public uint			nTotalBuildNum33;						// 总共的建筑队列 
		public uint			nNotificationID34;						// 最近阅读了的通知最新ID  
		public uint			nAllianceDrawLotteryNum35;				// 联盟抽奖今天剩余次数 
		public uint			nTotalDiamond36;						// 总充值钻石 
		public uint			nInstanceWangzhe37;						// 王者之路当前最大关卡  
		public uint			nInstanceZhengzhan38;					// 征战天下当前最大关卡  
		public uint			nCup39;									// 奖杯数 
		public byte			bBinded40;								// 是否已经绑定邮箱账号 
		public byte			bVipDisplay41;							// 是否显示vip 
	}
	// 君主属性摘要
	struct CharDesc
	{
		public int			nLevel1;
		public int			nExp2;
		public int			nDiamond3;
		public int			nCrystal4;
		public int			nGold5;
		public int			nVIP6;
		public int			nPopulation7;
	}
	struct STC_GAMECMD_OPERATE_BUILD_BUILDING_T 
	{
		// 返回值 
		public enum enum_rst
		{
			RST_OK						= 0,
			RST_ALREADY_BUILDING_AUTOID	= -1,		// 该AutoID上已经有建筑了 
			RST_AUTOID_ERR				= -2,		// 不能在该位置建造建筑 
			RST_GOLD_NOT_ENOUGH			= -3,		// 金钱不够 
			RST_POPULATION_NOT_ENOUGH	= -4,		// 人口不足 
			RST_BUILDING_QUEUE_ENOUGH	= -5,		// 建筑队列已满 
			RST_BUILDING_NUM_MAX		= -6,		// 该建筑已经到达上限 
			RST_UNKNOWN_ERR				= -100,		// 不明错误 
	
			// 其他返回值 
			RST_DB_ERR					= -200, 
			RST_SQL_ERR					= -201, 
		}
		public int 		    nRst1;
		public uint	 		nExcelID2;
		public uint			nAutoID3;
		public uint			nAddedCharExp4;
		public uint			nAddedCharDev5;
		public uint			nCurCharExp6;
		public uint			nCurCharLevel7;
	}
	struct STC_GAMECMD_OPERATE_UPGRADE_BUILDING_T
	{
		// 返回值 
		public enum enum_rst
		{
			RST_OK						= 0,
			RST_ALREADY_BUILDING_AUTOID	= -1,		// 该AutoID上已经有建筑了 
			RST_BUILDING_QUEUE_ENOUGH	= -2,		// 建筑队列已满 
			RST_BUILDING_LEVEL_MAX		= -3,		// 该建筑等级已经到达上限 
			RST_GOLD_NOT_ENOUGH			= -4,		// 金钱不够 
			RST_POPULATION_NOT_ENOUGH	= -5,		// 人口不足 
			RST_UNKNOWN_ERR				= -100,		// 不明错误 
			// 其他返回值 
			RST_DB_ERR					= -200, 
			RST_SQL_ERR					= -201, 
		} 
		public int			nRst1;
		public uint			nAutoID2;
		public uint			nExcelID3;
		public uint			nAddedCharExp4;
		public uint			nAddedCharDev5;
		public uint			nCurCharExp6;
		public uint			nCurCharLevel7;
	}
	struct STC_GAMECMD_POPULATION_ADD_T   
	{
		public int				nProduction1;
	}
	struct STC_GAMECMD_OPERATE_PRODUCTION_T
	{
		// 返回值 
		public enum enum_rst
		{
			// 对应存储过程,add_production_time_event 
			RST_OK						= 0,
			RST_TE_ALREADY_EXIST		= -1,		// 事件已经存在 
			RST_RIPE					= -2,		// 已经成熟,尚未采摘 
			RST_GOLDORE_NOT_EXIST		= -3,		// 没有对应的金矿 
			RST_CHOICE_ERR				= -4,		// 没有该生产选项 
			RST_TYPE_ERR				= -5,		// 没有该生产类型 
			RST_TE_INSERT_ERR			= -6,		// 事件插入出错 
			RST_TE_DATA_INSERT_ERR		= -7,		// 事件数据插入出错 
	
			// 其他返回值 
			RST_DB_ERR					= -200, 
			RST_SQL_ERR					= -201, 
		}
		public	int				nRst1;
		public	uint			nAutoID2;
		public  uint			nProduction3;
	}
	struct STC_GAMECMD_OPERATE_PRODUCE_GOLD_T 
	{
		// 返回值 
		public enum enum_rst
		{
			// 对应存储过程,add_production_time_event 
			RST_OK						= 0,
			RST_TE_ALREADY_EXIST		= -1,		// 事件已经存在 
			RST_RIPE					= -2,		// 已经成熟,尚未采摘 
			RST_GOLDORE_NOT_EXIST		= -3,		// 没有对应的金矿 
			RST_CHOICE_ERR				= -4,		// 没有该生产选项 
			RST_TYPE_ERR				= -5,		// 没有该生产类型 
			RST_TE_INSERT_ERR			= -6,		// 事件插入出错 
			RST_TE_DATA_INSERT_ERR		= -7,		// 事件数据插入出错 
	
			// 其他返回值 
			RST_DB_ERR					= -200, 
			RST_SQL_ERR					= -201, 
		}
		public	int				nRst1;
		public	uint			nAutoID2;
		public  uint			nProduction3;
	}
	struct STC_GAMECMD_FETCH_PRODUCTION_T 
	{
		// 返回值 
		public enum enum_rst
		{
			// 对应存储过程 
			RST_OK						= 0,
			RST_EVENT_NOT_EXIST			= -1,		// 不存在该成熟事件 
			RST_TYPE_ERR				= -2,		// 没有该类型的结果 
			RST_TE_DEL_ERR				= -3,		// 事件删除出错 
	
			// 其他返回值 
			RST_DB_ERR					= -200, 
			RST_SQL_ERR					= -201, 
		}
		public	int			nRst1;
		public  int			nAutoID2;
		public  int			nProduction3;
	}
	struct STC_GAMECMD_OPERATE_FETCH_GOLD_T  
	{
			// 返回值 
		public enum enum_rst
		{
			// 对应存储过程,add_production_time_event 
			RST_OK						= 0,
			RST_TE_ALREADY_EXIST		= -1,		// 事件已经存在 
			RST_RIPE					= -2,		// 已经成熟,尚未采摘 
			RST_GOLDORE_NOT_EXIST		= -3,		// 没有对应的金矿 
			RST_CHOICE_ERR				= -4,		// 没有该生产选项 
			RST_TYPE_ERR				= -5,		// 没有该生产类型 
			RST_TE_INSERT_ERR			= -6,		// 事件插入出错 
			RST_TE_DATA_INSERT_ERR		= -7,		// 事件数据插入出错 
	
			// 其他返回值 
			RST_DB_ERR					= -200, 
			RST_SQL_ERR					= -201, 
		}
		public	int				nRst1;
		public	uint			nAutoID2;
		public  uint			nProduction3;
	}
	struct STC_GAMECMD_GOLD_CAN_FETCH_T  
	{
		public uint		nAutoID1;
		public uint		nProduction2;
	}
	struct STC_GAMECMD_GET_PRODUCTION_EVENT_T 
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_DB_ERR		= -1,
			RST_SQL_ERR		= -2,
		}
		public	int				nRst1;
		public 	ulong			nAccountID2;
		public 	int				nNum3;
		// 后面跟nNum个ProductionEvent 
	}
	struct ProductionEvent 
	{
		public uint	nAutoID1;
		public uint	nProduction2;
		public uint	nType3;
	}
	struct STC_GAMECMD_GET_PRODUCTION_TE_LIST_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_DB_ERR		= -1,
			RST_SQL_ERR		= -2,
		}
		public	int			nRst1;
		public 	ulong		nAccountID2;
		public	uint		nTimeNow3;
		public  int			nNum4;
		// 后面跟nNum个ProductionTimeEvent 
	}
	struct ProductionTimeEvent 
	{
		public uint	nAutoID1;
		public uint	nProduction2;
		public uint	nType3;
		public uint	nBeginTime4;
		public uint	nEndTime5;
	}
	struct STC_GAMECMD_BUILDING_TE_T 
	{
		public	uint		nAutoID1;
		public	uint		nExcelID2;
		public	uint		nType3;			// te_subtype_building_build等 
	}
	struct STC_GAMECMD_OPERATE_ACCE_BUILDING_T
	{
		public enum enum_rst
		{
			RST_OK						= 0,
			RST_TYPE_ERR				= -1,			// 类型不对 
			RST_DIAMOND_NOT_ENOUGH		= -2,			// 钻石不够 
			RST_CHANGE_TIME_FAIL		= -3,			// 修改时间失败 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR				= -100,			// 数据库执行存储过程出错 
			RST_SELECT_INTO				= -101,			// select into出错 
			RST_VALUE_OVERFLOW			= -102,			// 值溢出 
			RST_DEADLOCK				= -103,			// 死锁 
	
			// 其他错误  
			RST_DB_ERR					= -200,
			RST_SQL_ERR					= -201,
		}
		public	int			nRst1;
		public	uint		nAutoID2;
		public	uint		nTime3;			 // 加速的时间,单位:秒(s)  
		public  uint		nMoneyNum4;		// 花销的金钱数量  
		public 	uint		nMoneyType5;		// 金钱类型   
		public  uint		nExcelID6;
		public	uint		nType7;
	}
	struct STC_GAMECMD_OPERATE_ACCE_GOLD_PRODUCE_T 
	{
		public enum enum_rst
		{
			RST_OK						= 0,
			RST_TYPE_ERR				= -1,			// 类型不对 
			RST_DIAMOND_NOT_ENOUGH		= -2,			// 钻石不够 
			RST_CHANGE_TIME_FAIL		= -3,			// 修改时间失败 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR				= -100,			// 数据库执行存储过程出错 
			RST_SELECT_INTO				= -101,			// select into出错 
			RST_VALUE_OVERFLOW			= -102,			// 值溢出 
			RST_DEADLOCK				= -103,			// 死锁 
	
			// 其他错误  
			RST_DB_ERR					= -200,
			RST_SQL_ERR					= -201,
		}
		public	int			nRst1;
		public	uint		nAutoID2;
		public	uint		nTime3;			 // 加速的时间,单位:秒(s)  
		public  uint		nMoneyNum4;		 // 花销的金钱数量  
		public 	uint		nMoneyType5;	 // 金钱类型  
		public  uint		nExcelID6;
	}
	struct STC_GAMECMD_OPERATE_ACCE_RESEARCH_T  
	{
		public enum enum_rst
		{
			RST_OK						= 0,
			RST_TYPE_ERR				= -1,			// 类型不对 
			RST_DIAMOND_NOT_ENOUGH		= -2,			// 钻石不够 
			RST_CHANGE_TIME_FAIL		= -3,			// 修改时间失败 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR				= -100,			// 数据库执行存储过程出错 
			RST_SELECT_INTO				= -101,			// select into出错 
			RST_VALUE_OVERFLOW			= -102,			// 值溢出 
			RST_DEADLOCK				= -103,			// 死锁 
	
			// 其他错误  
			RST_DB_ERR					= -200,
			RST_SQL_ERR					= -201,
		}
		public	int			nRst1;
		public	uint		nAutoID2;
		public	uint		nTime3;			 // 加速的时间,单位:秒(s)  
		public  uint		nMoneyNum4;		 // 花销的金钱数量  
		public 	uint		nMoneyType5;	 // 金钱类型  
		public	uint		nExcelID6;
	}
	struct STC_GAMECMD_OPERATE_RESEARCH_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_TECH_NOT_EXISTS		= -1,		// 不存在这种科技 
			RST_TECH_LEVEL_MAX		= -2,		// 科技已经达到上限 
			RST_INST_NOT_EXISTS		= -3,		// 未建造研究院 
			RST_INST_LEVEL_ERR		= -4,		// 研究院等级不够 
			RST_ALREADY_RESEARCH	= -5,		// 已经有科技在研究中 
			RST_GOLD_NOT_ENOUGH		= -6,		// 金钱不够 
			RST_INSERT_TE_ERR		= -7,		// 时间事件插入失败 
			RST_INSERT_TE_DATA_ERR	= -8,		// 事件数据插入失败 
			 
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR					= -200,
			RST_SQL_ERR					= -201,
		}
		public	int			nRst1;
		public	uint		nLevel2;
		public	uint		nTime3;
		public	uint		nGold4;
		public	uint		nExcelID5;
	}
	struct STC_GAMECMD_GET_TECHNOLOGY_T 
	{
		public enum enum_rst
		{
			RST_OK			= 0,
	
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public	int			nRst1;
		public	int			nNum2;
		// 后面跟nNum个TechnologyUnit 
	}
	struct TechnologyUnit
	{
		public  uint	nExcelID1;
		public  uint	nLevel2;
	}
	struct STC_GAMECMD_GET_RESEARCH_TE_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
	
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		public uint			nTimeNow2;
		public int			nNum3;
		// 后面跟nNum个ResearchTimeEvent 
	}
	struct ResearchTimeEvent
	{
		public uint	nExcelID1;
		public uint	nBeginTime2;
		public uint	nEndTime3;
	}
	// 获取alliance数据  
	struct STC_GAMECMD_GET_ALLIANCE_INFO_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_NO_ALLI		= -1,		// 联盟不存在 
	
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
	
			RST_LOADING		= -220,		// 载入中 
		}
		public int			nRst1;
		public ulong		nAllianceID2;
		public const int	len3 =32;
		public byte[]		szAllianceName4;
		public ulong		nLeaderID5;
		public const int	len6 =32;
		public byte[]		szLeaderName7;
		public uint			nDevelopment8; //联盟发展度 
		public uint			nTotalDevelopment9;
		public const int 	len10 =512;
		public byte[]		szIntroduction11;
		public uint			nRank12;				// 联盟排名 
		public uint			nMemberNum13;
		public uint			nMemberMaxNum14;
		public uint			nMyPosition15;			// 见枚举alliance_position_member等	
		public uint			nLevel16;
		public uint			nContribute17; //个人贡献度 
		public uint			nTotalContribute18;//个人历史贡献度 
		
	}
	struct STC_GAMECMD_GET_ALLIANCE_MEMBER_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		public uint			nNum2;
		// 后面跟nNum个AllianceMemberUnit 
	}
	public struct AllianceMemberUnit	// 其他信息再通过playercard获取 
	{
		public ulong		nAccountID1;
		public const int 	len2 = 32;
		public byte[]		szName3;
		public uint			nPosition4;
		public uint			nDevelopment5;
		public uint			nTotalDevelopment6;
		public uint			nLevel7;
		public uint			nCup8;
	}
	struct STC_GAMECMD_GET_ALLIANCE_BUILDING_TE_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
	
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		public uint			nNum2;
		// 后面跟nNum个AllianceBuildingTimeEvent 
	}
	struct STC_GAMECMD_GET_ALLIANCE_BUILDING_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
	
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		public uint			nNum2;
		// 后面跟nNum个AllianceBuildingUnit  
	}
	struct STC_GAMECMD_GET_ALLIANCE_JOIN_EVENT_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
	
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		public uint			nNum2;
		// 后面跟nNum个AllianceJoinEvent 
	}
	public struct AllianceJoinEvent 
	{
		public ulong		nAccountID1;
		public const int	len2 = 32;
		public byte[]		szName3;
		public uint			nLevel4;
		public uint			nHeadID5;
	}
	struct STC_GAMECMD_RESEARCH_TE_T 
	{
		public uint			nExcelID1;
		public uint			nLevel2;
	}
	struct STC_GAMECMD_OPERATE_CONSCRIPT_SOLDIER_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_TYPE_ERR			= -1,		// 不存在该兵种  
			RST_TECH_ERR			= -2,		// 科技等级不够  
			RST_POPULATION_ERR		= -3,		// 人口不足 
			RST_GOLD_ERR			= -4,		// 金钱不够 
			RST_GET_CAPACITY		= -5,		// 获取兵营容量时出错 
			RST_CAPACITY			= -6,		// 兵营容量不够 
			RST_DEDUCT_ERR			= -7,		// 扣除金钱人口时出错  
			RST_ADD_SOLDIER			= -8,		// 插入士兵时出错 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int		nRst1;
		public uint		nExcelID2;
		public uint		nLevel3;
		public uint		nGold4;
		public uint		nNum5;
	}
	struct STC_GAMECMD_OPERATE_UPGRADE_SOLDIER_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_TECH_ERR			= -1,		// 科技等级不够 
			RST_SOLDIER_NUM			= -2,		// 原等级兵种不够 
			RST_GOLD				= -3,		// 金钱不够 
			RST_ADD_SOLDIER			= -4,		// 插入士兵时出错 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int		nRst1;
		public uint		nExclID2;
		public uint		nFromLevel3;
		public uint		nToLevel4;
		public uint		nGold5;
		public uint		nNum6;
	}
	struct STC_GAMECMD_GET_SOLDIER_T
	{
		public enum enum_rst
		{
			RST_OK			= 0,
	
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int				nRst1;
		public int				nNum2;
		// 后面跟nNum个SoldierUnit 
	}
	public struct SoldierUnit 
	{
		public  uint	nExcelID1;
		public  uint	nLevel2;
		public	int		nNum3;
	}
	// 好友 
	struct STC_GAMECMD_GET_MY_FRIEND_APPLY_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
	
			// 其他错误  
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int			nRst1;
		public uint			nNum2;
		// 后面nNum个FriendApplyUnit   
	}
	public struct FriendApplyUnit 
	{
		public ulong		nAccountID1;
		public const int 	len2 = 32;
		public byte[]		szName3;
		public uint 		nLevel4;
		public uint			nHeadID5;
	}
	struct STC_GAMECMD_NEW_FRIEND_APPLY_T 
	{
		//后面跟一个FriendApplyUnit	unit;
	}
	struct STC_GAMECMD_GET_OTHERS_FRIEND_APPLY_T
	{
		public enum enum_rst
		{
			RST_OK					= 0,
	
			// 其他错误  
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public 	int			nRst1;
		public  uint		nNum2;
		// 后面nNum个FriendApplyUnit    
	}
	struct STC_GAMECMD_GET_FRIEND_LIST_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int			nRst1;
		public uint			nNum2;
		// 后面nNum个FriendUnit  
	}
	public struct FriendUnit 
	{
		public ulong		nAccountID1;
		public const int 	len2 = 32;
		public byte[]		szName3;
		public uint 		nLevel4;
		public uint 		nHeadID5;
	}
	struct STC_GAMECMD_OPERATE_APPROVE_FRIEND_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NO_EVENT			= -1,		// 申请好友事件不存在 
			RST_MY_FRIEND_MAX		= -2,		// 自己好友已达上限 
			RST_PEER_FRIEND_MAX		= -3,		// 对方好友已达上限  
	
			// 其他错误  
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int			nRst1;
		//后面跟一个FriendUnit   
	}
	struct STC_GAMECMD_OPERATE_APPLY_FRIEND_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_ALREADY_FRIEND		= -1,		// 已经是好友了   
			RST_ALREADY_EVENT		= -2,		// 已经有申请事件了  
			RST_SELF_MAX_FRIEND		= -4,		// 自己好友已达上限
			RST_PEER_MAX_FRIEND		= -5,		// 对方好友已达上限
	
			// 其他错误   
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int			nRst1;
		public ulong		nPeerAccountID2;
	}
	struct STC_GAMECMD_OPERATE_REFUSE_FRIEND_APPLY_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NO_EVENT			= -1,		// 没有申请事件 
	
			// 其他错误
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int			nRst;
		public ulong		nPeerAccountID;
	}
	struct STC_GAMECMD_OPERATE_CANCEL_FRIEND_APPLY_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NO_EVENT			= -1,		// 没有申请事件 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int			nRst1;
		public ulong		nPeerAccountID2;
	}
	struct STC_GAMECMD_OPERATE_DELETE_FRIEND_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NO_EVENT			= -1,		// 没有申请事件 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int			nRst1;
		public ulong		nPeerAccountID2;
	}
	struct STC_GAMECMD_OPERATE_TAVERN_REFRESH_T
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_OK_USEITEM	= 1,	// 成功，道具刷新 
			RST_CD			= -3,	// 刷新冷却中  
			RST_ITEM		= -4,	// 没有道具
			// 其他错误  
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int				nRst1;
		public int			    nCD2;
	}
	struct STC_GAMECMD_OPERATE_HIRE_HERO_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_TAVERN_ERR	= -2,	// 酒馆不存在 
			RST_HERO_ERR	= -3,	// 武将不存在 
			RST_HERO_FULL	= -10,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int				nRst1;
		public long				nHeroID2;
		public int				nSlotID3;
	}
	struct STC_GAMECMD_OPERATE_FIRE_HERO_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_HERO_ERR	= -2,	// 武将不存在 
			RST_EQUIP		= -3,	// 武将还穿着装备 
			RST_ARMY		= -4,	// 武将还带着部队 
			RST_CITYDEFENSE	= -5,	// 在城防配置里 
			RST_INSTANCE	= -6,	// 在副本配置里 
			RST_WORLDGOLDMINE	= -7,	// 在世界金矿配置里 
			RST_STATUS		= -8,	// 武将不是空闲状态 
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int				nRst1;
	}
	struct STC_GAMECMD_OPERATE_LEVELUP_HERO_T 
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_HERO_ERR	= -1,	// 武将不存在 
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
	}
	struct STC_GAMECMD_OPERATE_ADD_GROW_T   
	{
		public enum enum_rst
		{
			RST_OK			= 1,
			RST_HERO_ERR	= -1,	// 武将不存在 
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int				nRst1;
	}
	struct STC_GAMECMD_OPERATE_CONFIG_HERO_T 
	{
		public enum enum_rst
		{
			RST_OK			= 1,
			RST_HERO_ERR	= -1,	// 武将不存在 
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int				nRst1;
		public int				nProf2;
	}
	// 刷新武将 
	struct STC_GAMECMD_OPERATE_GET_HERO_REFRESH_T 
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误  
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int				nRst1;
		public int				nNum2;
		//后面跟nNum2个RefreshHero 
	}
	struct RefreshHero
	{
		public int		nSlotID1;		// 插槽位置 
		public ulong 	nHeroID2;		// 武将ID 
		public const int len3 = 32;
		public byte[]	szName4;		// 名字 
		public int		nProfession5;	// 职业 
		public int		nAttack6;		// 攻击 
		public int		nDefense7;		// 防御 
		public int		nHealth8;		// 生命 
		public int		nModel9;		// 模型ID 
	    public float	fGrow10;		// 成长率 
		public float	fGrowMax11;		// 成长率上限
	}
	//获取雇佣武将 
	struct STC_GAMECMD_OPERATE_GET_HERO_HIRE_T
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误  
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int				nRst1;
		public int				nNum2;
		//后面跟nNum2个HireHero 
	}
	public struct HireHero
	{
		public ulong		nHeroID1;			// 武将ID 
		public const int 	len2 = 32;
		public byte[]	 	szName3;			// 名字 
		public int			nProfession4;		// 职业 
		public int			nAttack5;			// 攻击 
		public int			nDefense6;			// 防御  
		public int			nHealth7;			// 生命  
		public float		fGrow8;				// 成长率  
		public int			nArmyType9;			// 部队类型 
		public int			nArmyLevel10;		// 部队等级 
		public int			nArmyNum11;			// 部队数量  
		public int			nModel12;			// 模型ID 
		public int			nLeader13;			// 统率 
		public int 			nStatus14;			// 是否出征(0:空闲,>0非空闲) nStatus 
	 	public int			nAttackAdd15;		// 攻击，装备加成的 
		public int			nDefenseAdd16;		// 防御，装备加成的 
		public int			nHealthAdd17;		// 生命，装备加成的 
		public int			nLeaderAdd18;		// 统率，装备加成的 
		public int			nLevel19;			// 等级 
		public int 			nProf20;			// 战力 prof
		public int			nHealthState21;		// 武将健康状态 nState
		public int 			nExp22;				// 武将经验值 Exp
		public float		fGrowMax23;			// 成长率上限
	}
	struct STC_GAMECMD_OPERATE_START_COMBAT_T
	{
		public enum enum_rst
		{
			RST_OK			= 1,
			RST_HERO_ERR	= -2,	//  武将不在空闲状态，此时 nCombatID 为武将ID  
			RST_OBJ_ERR		= -3,	// 不能对自己开战 OBJ
			RST_COMBAT_ID	= -4,	// 副本生成失败 ID
			RST_NOTEND		= -10,	// 上一次副本战斗还未结束 ID
			RST_HERO_ISNULL = -13,	// 不能没有武将 no
			RST_LEADER		= -14, 	// 只有队长才能发起战斗  bool 
			RST_PREPARE		= -16,	// 没有准备的玩家  num 
			RST_OBJ_NULL	= -17,	// 目标不存在 ...
			RST_COMBATTYPE	= -18, 	// 战斗类型错误 ...
			RST_PROTECT		= -19,	// 目标在保护期内 ...
			RST_PVPQUEUE	= -20,	// 出征队列已满 ...
			
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int		nRst1;
		public ulong	nCombatID2;
		public int 		nCombatType3;		// 战斗类型 ...
		public int		nGoTime4;			// 前往时间 ...
		public int		nBackTime5;			// 返回时间 ...
		public int 		nAutoCombat6;		// 返回是否是自动战斗 ...
	}
	struct STC_GAMECMD_OPERATE_STOP_COMBAT_T
	{
		public enum enum_rst
		{
			RST_ERR			= 0,
			RST_OK			= 1,
			RST_AUTO_COMBAT = 2,				// 战斗结束，（由于打开了自动战斗）新战斗已经开始 AutoCombat 
			RST_INSTANCE_FINISH = 3,				// 战斗结束，副本通关 InstancePass  
			RST_INSTANCE_FAILURE = 4,			// 战斗失败, 副本可重用次数用完 Failure
			RST_COMBAT_ERR	= -1,	// 战斗不存在  ...
			RST_IS_NULL		= -2,	// 副本不存在 ...
			RST_RESTART		= -3,	// 自动战斗中开启下一次战斗失败 ...
			RST_DESTROYINSTANCE = -4,
			
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int				nRst1;
		public int 				nCombatRst2; 
		public ulong			nCombatID3;
		public int 				nCombatType4;
		public ulong			n1HeroID5;
		public ulong			n2HeroID6;
		public ulong			n3HeroID7;
		public ulong			n4HeroID8;
		public ulong			n5HeroID9;
		public int 				nDrug10;		// 药物损失量 ...
		public ulong			nNewCombatID11;
		public int 				nGoTime12;
		public int 				nBackTime13;
		public int 				nNum14;
		// 是副本战斗并且开启自动补给，后面跟 InstanceHeroDesc  
	}
	public struct CombatDesc
	{
		public ulong 		nCombatID1;
		public int			nCombatType2;
		public ulong		nObjID3;
		public int			nRestTime4;  	// 剩余时间 
		public int			nStatus5; 		// 战斗状态  
		public const int 	len6 = 32;
		public byte[]		szName7;		// 目标名字 szName 
	}
	struct STC_GAMECMD_OPERATE_LIST_COMBAT_T 
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误  
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int				nRst1;
		public int				nNum2;
		// 后面跟nNum个战斗描述(CombatDesc)   
	}
	struct CombatLog
	{
		public int 			nCombatType1;
		public int 			nCombatRst2;
		public const int 	len3 = 32;
		public byte[]		szAttackName4;		// 攻方名字 name 
		public const int 	len5 = 32;
		public byte[]		szDefenseName6;		// 防守名字 name 
		public int 			n1Num7;				// 攻方武将数量 num 
		public int 			n2Num8;				// 守方武将数量 num 
		public int			nAttackHeadID9;
		public int			nDefenseHeadID10;
		public ulong		nObjID11;
		public int 			nPosX12;
		public int			nPosY13;
		// 后面跟 武将信息 CombatUnit4Log  
		// 后面跟 附加信息，根据 nCombatType  
	}
	struct CombatUnit4Log
	{
		public int 			nSlot1;
		public int			nLevel2;			// 武将等级   ...
		public const int	len3 = 32;
		public byte[]		szName4;			// 武将名字  
		public int 			nArmyType5;			// 部队类型  
		public int 			nArmyLevel6;		// 部队等级 
		public int 			nArmyNumBefore7;	// 部队数量,战前   num
		public int 			nArmyNumAfter8;		// 部队数量,战后   num
		public ulong 		nHeroID9;			// 武将ID 
		public int			nArmyNumRecover10;	// 部队恢复数量
		public int			nArmyAttack11;		// 部队攻击
		public int			nArmyDefense12;		// 部队防御
		public int			nArmyHealth13;		// 部队生命
	}
	struct CombatRst
	{
		public ulong		nHeroID1;			// 武将ID ...
		public int 			nArmyNum2;			// 部队数量 ...
		public int 			nArmyAttack3;		// 部队攻击力 ...
		public int 			nArmyDefense4;		// 部队防御 ...
		public int 			nArmyHealth5;		// 部队生命 ...
		public int 			nHealthStatus6;		// 健康状况 ...
		public int 			nArmyProf7;			// 军队战斗力 ...
		public int			nArmyNumRecover8;	// 部队恢复数量 ...
		public int 			nHeroLevel9;		// 英雄等级 ...
		public int			nHeroExp10;			// 英雄经验 ...
	}
	struct CombatUnit
	{
		public int 			nSlot1;				// 战场通道 
		public ulong 		nHeroID2;			// 英雄ID 
		public const int 	len3 = 32;			// 
		public byte[] 		szName4;			// 武将名字 
		public int 			nArmyType5;			// 部队类型 
		public int 			nArmyLevel6;		// 部队等级   
		public int 			nArmyNum7;			// 部队数量   
		public int 			nArmyAttack8;		// 部队攻击   
		public int 			nArmyDefense9;		// 部队防御   
		public int 			nHealth10;			// 部队生命   
		public int 			nModel11;			// 头像 ID
		public int 			nLevel12;			// 等级 Lv.
		public int 			nHealthStatus13;	// 健康状况 State
	}
	//PVP报告 
	struct CombatLog_PVP
	{
		public int				nGold1;				// 损失金币 
		public int				nPopulation2;		// 损失人口  
		public int				nAddPopulation3;	// 实际增加 
		public int				nCup4;				// 奖杯  
	}
	//PVE报告 
	struct CombatLog_PVE_Raid 
	{
		public int				nExcelID1;		// 道具ID 
		public int				nNum2;			// 道具数量 
	}
	//王者之路 征战天下 
	struct CombatLog_InstanceLoot
	{
		public int nNum1;			// 掉落数量 
	}
	// 掉落摘要 
	struct LootDesc
	{
		public int				nExcelID1;
		public int				nNum2;
	}
	// 战斗日志附加信息，世界金矿  
	struct CombatLog_WorldGoldMine
	{
		public int				nArea1;			// 金矿区域 
		public int				nClass2;			// 金矿等级 
		public int				nIdx3;			// 金矿序号 
	}
	struct STC_GAMECMD_OPERATE_GET_COMBAT_T
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_COMBAT_ERR	= -1,
			// 其他错误... 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		public ulong		nCombatID2;			// 战斗ID 
		public int 			nCombatType3;		// 战斗类型 
		public int 			nNumA4;				// A方单元数量 
		public int 			nNumB5;				// B方单元数量 ..
		public const int	len6 = 32;
		public byte[]		szObjName7;			// 对方玩家名字 ...
		public int 			nObjHeadID8; 		// 头像ID ... 
		// 后面跟nNumA+nNumB个战斗单元数量 
	}
	struct STC_GAMECMD_ARMY_BACK_T
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_COMBAT		= -2,				// 战斗不存在 ...
			RST_TE			= -3,
			// 其他错误...
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		
		public int 			nRst1;				// 返回值 ...
		public ulong		nCombatID2;			// 战斗ID ...
		public int 			nBackTime3;			// 剩余时间 ...
	}
	struct STC_GAMECMD_ARMY_ACCELERATE_T
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_COMBAT		= -2,				// 战斗不存在 ...
			RST_TE			= -3,				// 时间事件不存在 ...
			RST_CRYSTAL		= -4,				// 水晶扣费失败 ...
			// 其他错误...
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		
		public int 			nRst1;
		public ulong		nCombatID2;		
		public	int 		nCostCrystal3;		// 水晶费数量 ...
		public byte			byType4;			// 类型 ...
	}
	// alliance
	struct STC_GAMECMD_OPERATE_ACCEPT_ALLI_MEMBER_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,		// 成功 
			RST_NO_JOIN_EVENT		= -1,		// 没有申请加入联盟 
			RST_NO_RIGHT			= -2,		// 没有权限批准加入 
			RST_MEMBER_FULL			= -3,		// 人数已满 
			RST_SET_ALLI_ID			= -4,		// 设置玩家的联盟ID出错 
			RST_ADD_MEMBER			= -5,		// 插入成员表出错 
			RST_DELETE_EVENT		= -6,		// 删除申请事件出错 
			RST_ADD_DEVELOPMENT		= -7,		// 增加联盟发展度出错 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错  
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int			nRst1;
		public ulong		nMemberID2;
	}
	struct STC_GAMECMD_OPERATE_BUILD_ALLI_BUILDING_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NO_ALLI				= -1,		// 没有联盟 
			RST_NO_BUILD_RIGHT		= -2,		// 没有建设权限 
			RST_EXIST_BUILD_EVENT	= -3,		// 已经在建设中了 
			RST_NO_CONGRESS			= -4,		// 议会不存在 
			RST_LEVEL_MAX			= -5,		// 当前等级已达上限 
			RST_DEVELOPMENT			= -6,		// 发展度不够扣除 
			RST_ADD_TE				= -7,		// 插入时间事件出错 
			RST_ADD_TE_DATA			= -8,		// 插入时间事件数据出错 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int			nRst1;
		public uint			nExcelID2;
		public uint			nType3;
	}
	struct STC_GAMECMD_OPERATE_CANCEL_JOIN_ALLI_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int			nRst1;
	}
	struct STC_GAMECMD_OPERATE_CREATE_ALLI_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_ALREADY_IN_ALLI		= -1,		// 已经有联盟了,先退出或解散才能创建联盟 
			RST_NAME_DUP			= -2,		// 联盟名字重复 
			RST_GOLD				= -3,		// 黄金不够 
			RST_ALLI_CREATE			= -4,		// 联盟创建出错 
			RST_SET_POSITIOn		= -5,		// 设置权限出错 
			RST_SET_ALLI_ID			= -6,		// 设置联盟ID出错 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int			nRst1;
		public ulong		nAllianceID2;
		public uint			nGold3;
	}
	struct STC_GAMECMD_OPERATE_DISMISS_ALLI_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NO_ALLIANCE			= -1,		// 没有联盟 
			RST_NO_RIGHT			= -2,		// 无权限,只有盟主能解除 
			RST_DEL_ALLI			= -3,		// 删除联盟数据出错 
			RST_DEL_ALLI_JOIN_EVENT	= -4,		// 删除申请加入联盟事件出错 
			RST_DEL_ALLI_MEMBER		= -5,		// 删除联盟成员出错 
			RST_CLEAR_ALLI_ID		= -6,		// 清空联盟ID出错 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int		nRst1;
	}
	struct STC_GAMECMD_OPERATE_EXIT_ALLI_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NO_ALLI				= -1,		// 没有联盟 
			RST_LEADER				= -2,		// 盟主不能退出联盟,只能解散或者让位后退出 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	 
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int		nRst1;
	}
	struct STC_GAMECMD_OPERATE_EXPEL_ALLI_MEMBER_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NOT_IN_ALLI			= -1,		// 你不在联盟中 
			RST_MEMBER_NOT_IN_ALLI	= -2,		// 被开除者不在联盟中 
			RST_POSITION			= -3,		// 职位不够 
			RST_DEL_MEMBER			= -4,		// 从成员表中删除出错 
			RST_CLEAR_ALLI_ID		= -5,		// 修改联盟ID出错 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int			nRst1;
		public ulong		nMemberID2;
	}
	struct STC_GAMECMD_OPERATE_JOIN_ALLI_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NO_ALLI				= -1,		// 联盟不存在 
			RST_ALREADY_ALLI		= -2,		// 已经有联盟了 
			RST_ALREADY_APPLY		= -3,		// 已经申请过了 
			RST_ADD_EVENT			= -4,		// 插入申请事件出错 
			RST_ALLI_FULL			= -5,		// 联盟人数已满
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int			nRst1;
		public ulong		nAllianceID2;
	}
	struct STC_GAMECMD_OPERATE_SET_ALLI_POSITION_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NOT_IN_ALLI			= -1,		// 你不在联盟里  
			RST_MEMBER_NOT_IN_ALLI	= -2,		// 被任命者不在联盟里 
			RST_POSITION			= -3,		// 职位不够高 
			RST_NO_RIGHT			= -4,		// 没权力任命 
			RST_NO_CHG				= -5,		// 职位没变动 
			RST_NO_POSITION			= -6,		// 没有该职位 
			RST_FULL				= -7,		// 该职位人数已满 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int			nRst1;
		public ulong		nAllianceID2;
		public ulong		nMemberID3;
		public uint			nPosition4;
	}
	struct STC_GAMECMD_OPERATE_REFUSE_JOIN_ALLI_T   
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_MANAGER_NOT_IN_ALLI	= -1,		// 管理者不在联盟内 
			RST_EVENT_NOT_EXIST		= -2,		// 没有申请加入事件 
			RST_NO_RIGHT			= -3,		// 没有权力拒绝申请 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int			nRst1;
		public ulong		nApplicantID2;
	}
	struct STC_GAMECMD_OPERATE_ABDICATE_ALLI_T  
	{
		public enum enum_rst 
		{
			RST_OK					= 0,
			RST_NOT_LEADER			= -1,		// 不是盟主,不能禅让 
			RST_MEMBER_NOT_IN		= -2,		// 不能禅让给非联盟中成员 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int			nRst1;
	}
	struct STC_GAMECMD_GET_RELATION_LOG_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_DB_ERR				= -1,		// 数据库出错(断连等) 
			RST_SQL_ERR				= -2,		// SQL错误 
		}
		public int				nRst1;
		public int				nNum2;
		// 后面跟nNum个RelationLogUnit  
	}
	struct STC_GAMECMD_OPERATE_FETCH_CHRISTMAS_TREE_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NOT_RIPE			= -1,		// 尚未成熟 
			RST_ADD_ITEM_FAIL		= -3,		// 添加道具失败 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public enum enum_award_type
		{
			award_type_none			= 0,
			award_type_crystal		= 1,
			award_type_item			= 2,
		}
		public int		nRst1;
		public uint		nType2;
		public uint		nCrystal3;
		public uint		nExcelID4;
		public ulong	nItemID5;
	}
	public struct STC_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_ALREADY_RIPE		= -1,		// 已经成熟，不能浇水 
			RST_READY_RIPE			= -2,		// 准备成熟，不能浇水 
			RST_IN_CD				= -3,		// 自己给自己浇水，有CD 
			RST_ALREADY_WATERING	= -4,		// 已经浇过水了(别人) 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	 
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int		nRst1;
	}
	struct STC_GAMECMD_OPERATE_STEAL_GOLD_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_STEAL_SELF			= -1,		// 不能自己偷自己 
			RST_NOT_RIPE			= -2,		// 还没成熟 
			RST_CANNOT_STEAL		= -3,		// 不能偷  
			RST_STEAL_NUM_MAX		= -4,		// 偷窃次数已满 
			RST_ALREADY_STEAL		= -5,		// 已经偷过了 
			RST_SUB_GOLD_ERR		= -10,		// 扣除黄金失败 
			RST_GOLD_PROTECT		= -20,		// 现在是保护期 
			
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int		nRst1;
		public uint		nGold2;
	}
	struct STC_GAMECMD_GET_TRAINING_TE_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int		nRst1;
		public uint		nNum2;
		// 后面跟nNum个TrainingTimeEvent  
	}
	struct TrainingTimeEvent 
	{
		public int			nBeginTime1;
		public int			nEndTime2;
		public ulong		nHeroID3;
		public uint			nExp4;
	}
	struct STC_GAMECMD_OPERATE_ADD_TRAINING_T  
	{
		public enum enum_rst
		{	
			RST_OK					= 0,
			RST_ERR_STATUS			= -1,		// 将领状态不能修炼 
			RST_TRAINING_HALL_LEVEL	= -2,		// 修炼馆等级太低 
			RST_TOO_LONG_TIME		= -3,		// 修炼时间太长 
			RST_NO_ENOUGH_GOLD		= -10,		// 黄金不够 
			RST_ERR_ADD_TE			= -11,		// 添加时间事件出错 
			RST_ERR_ADD_TE_DATA		= -12,		// 添加时间事件数据出错 
			RST_ERR_CHANGE_STATUS	= -13,		// 修改将领状态出错 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int		nRst1;
	}
	struct STC_GAMECMD_OPERATE_EXIT_TRAINING_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int		nRst1;
		//后面跟着 一个HireHero  
	}
	struct STC_GAMECMD_TRAINING_OVER_T  
	{
		//后面跟着 一个HireHero 
	}
	// worldgoldmine 
	public struct WorldGoldmineDesc
	{
		public int 			nClass1;			// 金矿等级     
		public int 			nIdx2;				// 金矿序号     
		public ulong		nAccountID3;		// 账号ID 
		public const int 	len4 = 32;			// 
		public byte[] 		szName5;			// 账号名称
		public int 			nSafeTime6;			// 安全时间    
	}
	struct STC_GAMECMD_GET_WORLDGOLDMINE_T
	{
		public enum enum_rst
		{
			RST_OK 			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int 			nRst1;
		public int 			nNum2;
		public int			nAreaID3;
		// 后面跟 WorldGoldMineDesc
	}
	struct STC_GAMECMD_ROB_WORLDGOLDMINE_T
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_HASIT		= -2,	// 已经占领一个了 ... 
			RST_ISNULL		= -3,	// 金矿不存在 ... 
			RST_SAFETIME	= -4,	// 金矿还处在保护时段 ... 
			RST_COMBAT		= -5,	// 发起战斗失败 ... 
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int 			nRst1;
		public ulong		nCombatID2;
	}
	struct STC_GAMECMD_DROP_WORLDGOLDMINE_T
	{
		public enum enum_rst
		{
			RST_OK			= 1,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public	int 		nRst1;
	}
	struct STC_GAMECMD_GAIN_WORLDGOLDMINE_T
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public	int 		nRst1;
		public	int 		nArea2;
		public 	int 		nClass3;
		public	int 		nIdx4;
		public	int 		nGold5;
	}
	struct STC_GAMECMD_MY_WORLDGOLDMINE_T
	{
		public enum enum_rst
		{
			RST_OK			= 1,
			RST_ISNULL		= 0,	// 你未占领任何金矿 ... 
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public	int 		nRst1;
		public	int 		nArea2;
		public	int 		nClass3;
		public	int 		nIdx4;
		public  int 		nSafeTime5;
		public	ulong 		n1Hero6;
		public	ulong 		n2Hero7;
		public	ulong 		n3Hero8;
		public	ulong 		n4Hero9;
		public	ulong 		n5Hero10;
	}
	struct STC_GAMECMD_CONFIG_WORLDGOLDMINE_HERO_T
	{
		public enum enum_rst
		{
			RST_OK			= 1,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public 	int 		nRst1;
	}

	struct STC_GAMECMD_OPERATE_CONFIG_CITYDEFENSE_T 
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误  
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
	}
	// 道具 
	struct STC_GAMECMD_GET_ITEM_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		public int			nNum2;
		// 后面跟nNum2个ItemDesc 
	}
	public struct ItemDesc
	{
		public ulong		nItemID1;		// 道具ID 
		public int			nExcelID2;		// excel ID 
		public int			nNum3;			// 数量  
	}
	struct STC_GAMECMD_ADD_ITEM_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
	}
	struct STC_GAMECMD_DEL_ITEM_T  
	{		
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
	}
	struct STC_GAMECMD_EQUIP_ITEM_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
	}
	struct STC_GAMECMD_DISEQUIP_ITEM_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
	}
	struct STC_GAMECMD_MOUNT_ITEM_T  
	{
		public enum enum_rst
		{
			RST_OK			= 1,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		// 后面跟 HeroDesc 
	}
	struct STC_GAMECMD_UNMOUNT_ITEM_T  
	{
		public enum enum_rst
		{
			RST_OK			= 1,
			// 其他错误  
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
	}
	struct STC_GAMECMD_COMPOS_ITEM_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误  
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1; 
		public ulong		nItemID2;
	}
	struct STC_GAMECMD_DISCOMPOS_ITEM_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
	}
	struct STC_GAMECMD_EQUIP_ITEM_ALL_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
	}
	struct STC_GAMECMD_MOUNT_ITEM_ALL_T 
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
	}
	// 任务摘要 ... 
	public struct QuestDesc
	{
		public int ExcelID1;
		public int nDone2;
	}
	// 任务列表 ...
	struct STC_GAMECMD_GET_QUEST_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误  
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		public int			nNum2;
		// 后面跟nNum2个任务描述 ...  
	}
	struct STC_GAMECMD_DONE_QUEST_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		public int 			nNum2;
		// 后面跟nNum2个任务道具 ... AddItemDesc
	}
	// 副本状态摘要 
	struct InstanceStatus
	{
		public int 			nExcelID1; 				// 副本ID 
		public int 			nResDayTimesFree2; 		// 今天剩余免费次数 
		public int 			nDayTimesFree3;			// 今天剩余收费次数  
		public int			nResDayTimesFee4;	// 今天剩余收费次数 
		public int 			nDayTimesFee5;			//  
		public ulong		nInstanceID6;			// 实例ID，>0表示当前正在副本里   
		public int 			nStatus7;				// 副本状态 status
		public int 			nClassID8;				// 副本战役类型 classid
		public ulong		nCreatorID9;			// 副本创建者 ID
		public int			nCurLevel10;			// 当前关卡
	}
	// 副本 
	struct STC_GAMECMD_GET_INSTANCESTATUS_T 
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		public int			nNum2;
		// 后面跟nNumA+nNumB个战斗单元数量 ...  
	}
	struct InstanceDesc
	{
		public ulong		nInstanceID1; 			// 副本ID
		public const int 	len2 = 32;				// 
		public byte[] 		szName3;				// 队伍名字 name
		public int 			nCurLevel4;				// 当前进度 level
		public int 			nLeaderLevel5;			// 首领等级 level
		public int 			nNumPlayer6;			// 当前进组人数 num
		public ulong		nCreatorID7;			// 副本创建者 id
	}
	struct STC_GAMECMD_GET_INSTANCEDESC_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		public int 			nClassID2;
		public int			nNum3;
		// 后面跟 InstanceDesc  
	}
	struct STC_GAMECMD_CREATE_INSTANCE_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_INNOW		= -2,	// 已经在此副本里了 innow 
			RST_ISNULL		= -3,	// 副本ID不存在 isnull
			RST_LEVEL		= -4,	// 玩家等级未达到副本进入要求 level 
			RST_OPENTIME	= -5,	// 此刻不在副本开放时段 open 
			RST_TIMES		= -6,	// 当天的免费次数和收费次数都用完了 times 
			RST_COST		= -7,	// 收费次数需要的消耗品没有 ....
			// 其他错误  
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		public ulong		nInstanceID2;
	}
	struct STC_GAMECMD_JOIN_INSTANCE_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_INNOW			= -2,	// 已经在此副本里了 ...
			RST_ISNULL			= -3,	// 副本ID不存在 ...
			RST_LEVEL			= -4,	// 玩家等级未达到副本进入要求 ...
			RST_OPENTIME		= -5,	// 此刻不在副本开放时段 ...
			RST_TIMES			= -6,	// 当天的免费次数和收费次数都用完了 ...
			RST_COST			= -7,	// 收费次数需要的消耗品没有 ...
			RST_PLAYER			= -8,	// 副本满员了 ....
			RST_START			= -9,	// 副本已经出征了 ...
			RST_INSTANCE		= -16,	// 副本实例不存在 ....

			// 其他错误  
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1; 
	}
	struct STC_GAMECMD_QUIT_INSTANCE_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_UPDATE		= -3,	// 更新副本人数时失败
			RST_COMBAT		= -4,	// 还在战斗中
			RST_ISNULL		= -5,	// 副本不存在
			RST_UNLOCK		= -6,	// 武将解锁失败
			// 其他错误  
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		public ulong 		nInstanceID2;
	}
	struct STC_GAMECMD_DESTROY_INSTANCE_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_ISNULL		= -2,	// 副本不存在 ...
			RST_UPDATE		= -3,	// 更新副本人数时失败 ...
			RST_COMBAT		= -4,	// 还在战斗中 ...
			
			// 其他错误  
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		public ulong		nInstanceID2;
	}
	// 副本成员  
	struct InstanceData
	{
		public ulong		nAccountID1;			// 成员账号ID  
		public const int 	len2 = 32;
		public byte[]		szName3;				// 成员账号名字   
		public ulong		n1Hero4;				// 武将ID  
		public ulong		n2Hero5;
		public ulong		n3Hero6;
		public ulong		n4Hero7;
		public ulong		n5Hero8;
		public int 			nStatus9;			//是否就绪 nStatus
	}
	struct STC_GAMECMD_GET_INSTANCEDATA_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		public int 			nCurLevel2;
		public int 			nAutoCombat3;
		public int 			nAutoSupply4;
		public int 			nRetryTimes5;
		public ulong		nInstanceID6;
		public int			nNum7;
		public int 			nNumHero8;			// 多人副本武将信息 hero
		// 后面跟Num个InstanceData
		// 后面跟NumHero个 CombatUnit
	}
	struct STC_GAMECMD_CONFIG_INSTANCEHERO_T 
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_HERO_ERR	= -1,	// 武将不存在 
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
	}
	
	public struct InstanceLootDesc
	{
		public ulong		nItemID1;
		public int 			nExcelID2;
		public int			nNum3;
	}
	struct STC_GAMECMD_GET_INSTANCELOOT_T 
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		public int			nNum2;
		// 后面跟nNum2个 InstanceLootDesc 
	}
	struct STC_GAMECMD_GET_INSTANCELOOT_ADDITEM_T
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public ulong 		nInstanceID1;
		public int			nCombatType2;
		public int			nObjID3;
		public int 			nNum4;
		public int 			nClassID5;
		// 后面跟nNum2个 InstanceLootDesc 
	}
	struct STC_GAMECMD_KICK_INSTANCE_T
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,			
		}
		public int 			nRst1;	
	}
	struct STC_GAMECMD_PREPARE_INSTANCE_T
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_TYPE		= -2,	// 不是多人副本 ... 
			RST_HERO		= -3,	// 还未配置武将 ... 

			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,			
		}
		public int 			nRst1;
	}
	struct STC_GAMECMD_SUPPLY_INSTANCE_T
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_TYPE		= -2,	// 不是多人副本 ... 
			RST_LEADER		= -3,	// 不是队长 ... 

			// 其他错误   
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int 			nRst1;
		public int 			nExcelID2;
		public ulong		nInstanceID3;
	}
	struct STC_GAMECMD_START_INSTANCE_T
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_TYPE		= -2,	// 不是多人副本 ... type
			RST_LEADER		= -3,	// 不是队长 ... leader
			RST_PREPARE		= -4,	// 还有玩家未准备就绪 ... ok
			RST_PLAYER		= -5,	// 没有玩家准备就绪 ... ok

			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,			
		}
		public int 			nRst1;
		public ulong		n1Account2;				// 未就绪的玩家     
		public ulong		n2Account3;				// 未就绪的玩家     
		public ulong 		n3Account4;				// 未就绪的玩家     
		public ulong 		n4Account5;				// 未就绪的玩家     
		public ulong		n5Account6;				// 未就绪的玩家           
	}
	
	struct STC_GAMECMD_AUTO_COMBAT_T
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int 			nRst1;
	}
	struct STC_GAMECMD_COMBAT_PROF_T
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public	int 		nRst1;
		public 	int 		nProf2;			// 战力   
		public 	int 		n1ArmyType3;	// 1部队类型     
		public 	int 		n2ArmyType4;	// 2部队类型    
		public 	int 		n3ArmyType5;	// 3部队类型   
		public 	int 		n4ArmyType6;	// 4部队类型   
		public 	int 		n5ArmyType7;	// 5部队类型     
	}
	struct STC_GAMECMD_COMBAT_RST_T 
	{
		public enum enum_rst
		{
			RST_OK			= 1,
			RST_OK_ZHANBAI 	= 2,
			// 其他错误 s 
			RST_DB_ERR		= -200,
		}
		
		public ulong			nCombatID1;
		public int				nCombatType2;
		public ulong			nObjID3;
		public const int 		len4 = 32;
		public byte[]			szObjName5;
		public int				nObjHeadID6;
		public int				nRst7;
		public int 				nNum8;
		public int 				nClassID9;
		// 后面跟Num个CombatRst  
	}
	struct STC_GAMECMD_USE_ITEM_T 
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_ITEM_ERR	= -1,	// 道具不存在  
			RST_USE_IN_COMBAT	= -15,	// 战斗过程中不能使用 
			// 其他错误  
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		public int			nType2;
		// nType==SYNC_TYPE_HERO，后面跟 HeroDesc  
	}
	struct STC_GAMECMD_SYNC_CHAR_T  
	{
		public enum enum_rst
		{
			RST_OK			= 1,
			// 其他错误  
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
	}
	struct STC_GAMECMD_SYNC_HERO_T 
	{
		public enum enum_rst
		{
			RST_OK			= 1,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
	}
	struct STC_GAMECMD_AUTO_SUPPLY_T 
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
	}
	struct InstanceHeroDesc
	{
		public ulong		nHeroID1;
		public int			nArmyNum2;
		public int 			nArmyProf3;			// 军队战力 prof 
		public int 			nHealthState4;		// 武将健康状态 ... 
		public int 			nLevel5;			// 武将等级 ... 
		public int 			nMaxArmyNum6;		// 最大军队补给 ... 
	}
	struct STC_GAMECMD_MANUAL_SUPPLY_T
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_WORLDGOLDMINE	= -3,	// 世界金矿未配置武将 ... 
			RST_INSTANCE		= -4,	// 副本不存在 ... 
			RST_PLAYER			= -5,	// 玩家不在副本里 ... 

			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;	
		public int 			nNum2;
		// 后面跟 InstanceHeroDesc 
	}
	struct STC_GAMECMD_REPLACE_EQUIP_T 
	{
		public enum enum_rst
		{
			RST_OK			= 1,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		// 后面跟 HeroDesc  
	}
	// 武将属性摘要  
	public struct HeroDesc
	{
		public int				nAttackBase1;
		public int				nAttackAdd2;
		public int				nDefenseBase3;
		public int				nDefenseAdd4;
		public int				nHealthBase5;
		public int				nHealthAdd6;
		public int				nLeaderBase7;
		public int				nLeaderAdd8;
		public float			fGrow9;
		public ulong			nHeroID10;
		public int				nHealthState11;
		public int				nArmyType12;
		public int				nArmyLevel13;
		public int				nArmyNum14;
		public int				nExp15;
		public int				nLevel16;
		public int				nProf17;
	}
	struct STC_GAMECMD_GET_EQUIP_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		public int			nNum2;
	}
	// 武将装备关联摘要  
	struct EquipDesc
	{
		public ulong		nEquipID1;		// 装备ID 
		public int			nEquipType2;	// 装备类型  
		public int			nExcelID3;		// 装备excel id  
		public ulong		nHeroID4;		// 武将ID   
	}
	struct STC_GAMECMD_GET_GEM_T  
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			// 其他错误  
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int			nRst1;
		public int			nNum2;
	}
	// 装备宝石关联摘要 
	struct GemDesc
	{
		public ulong		nEquipID1;		// 装备ID 
		public int			nSlotIdx2;		// 插槽号 
		public int			nSlotType3;		// 插槽类型 
		public int			nExcelID4;		// 宝石excel id 
		public ulong		nGemID5;			// 宝石唯一id 
	}
	struct STC_GAMECMD_CHAR_IS_ONLINE_T  
	{
		public byte				bOnline1;
		public ulong			nAccountID2;
	}
	struct STC_GAMECMD_GET_WORLD_AREA_INFO_TERRAIN_T  
	{
		public uint	nFromX1;
		public uint	nFromY2;
		public uint	nToX3;
		public uint	nToY4;
		public uint	nNum5;
		// 后面跟nNum个TerrainUnit 
	}
	struct STC_GAMECMD_GET_WORLD_AREA_INFO_CITY_T  
	{
		public uint	nFromX1;
		public uint	nFromY2;
		public uint	nToX3;
		public uint	nToY4;
		public uint	nNum5;
		// 后面跟nNum个CityCard 
	}
	struct STC_GAMECMD_OPERATE_MOVE_CITY_T
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_ALREADY_PLAYER		= -1,	// 已经有人
			RST_NO_ITEM				= -2,	// 没有迁城卷轴
			RST_DEL_ITEM_FAIL		= -3,	// 删除道具失败
			RST_CHG_POSITION_FAIL	= -4,	// 更新坐标失败
	
			// 其他错误
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
	
			RST_ALREADY_BOOKED		= -220,	// 已经有其他人往这个位置迁移(暂废弃)
			RST_CANNOT_BUILD_CITY	= -221,	// 这个地方不能建城(暂废弃)
			RST_INVALID_POSITION	= -222,	// 超出地图范围的位置(暂废弃)
			RST_CANNOT_MOVE_CITY	= -223,	// 这个位置不能迁城
		}
		
		public int		nRst1;
	}
	struct STC_GAMECMD_OPERATE_ADD_ALLIANCE_MAIL_T  
	{
		public enum enum_rst 
		{
			RST_OK					= 0,
			RST_NOT_IN_ALLIANCE		= -1,	// 没有加入联盟 
	
			// 其他返回值 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
	
			RST_TEXT_TOO_LONG		= -220,	// 文本超长  
		}
		public int		nRst1;
	}
	struct STC_GAMECMD_OPERATE_BUY_ITEM_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NUM_0				= -1,	// 购买数量不能为0 
			RST_ITEM_NOT_EXIST		= -2,	// 道具不存在 
			RST_CANNOT_USE_CRYSTAL	= -3,	// 该物品不能用水晶购买 
			RST_MONEY_NOT_ENOUGH	= -4,	// 金钱不够 
			RST_ADD_ITEM_FAIl		= -5,	// 添加道具失败 
			RST_ERR_MONEYTYPE		= -10,	// 金钱类型不对 
	
			// 其他返回值 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
	
			RST_TOO_MANY			= -220,	// 一次买入过多 
		}
		public int		nRst1;
		public uint		nExcelID2;
		public uint		nNum3;
		public uint		nMoneyNum4;
		public uint		nMoneyType5;
		public uint		nItemIDNum6;
		// 后面跟nItemIDNum个item_id(tty_id_t)  
	}
	struct STC_GAMECMD_OPERATE_SELL_ITEM_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_ITEM_NOT_EXIST		= -1,	// 道具不存在 
			RST_TOO_MANY			= -2,	// 数量超过拥有量 
			RST_DEL_ITEM_FAIl		= -3,	// 扣除道具失败 
	
			// 其他返回值 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int		nRst1;
		public ulong	nItemID2;
		public uint		nExcelID3;
		public uint		nNum4;
		public uint		nGold5;
	}
	
	struct STC_GAMECMD_OPERATE_CONTRIBUTE_ALLIANCE_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NUM_0				= -1,	// 不能兑换0贡献度 
			RST_NO_ALLIANCE			= -2,	// 不在联盟中 
			RST_GOLD_NOT_ENOUGH		= -3,	// 黄金不够  
			RST_P_ADD_FAIL			= -4,	// 个人贡献度添加失败 
			RST_A_ADD_FAIL			= -5,	// 联盟发展度添加失败 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他返回值 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int		nRst1;
		public uint		nGold2;
		public uint		nDevelopment3;	// 贡献度   
	}
	struct STC_GAMECMD_OPERATE_CONVERT_DIAMOND_GOLD_T
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_DIAMOND_0			= -1,	// 兑换的钻石为0 
			RST_DIAMOND_NOT_ENOUGH	= -2,	// 钻石不够 
	
			// 其他返回值 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int		nRst1;
		public uint 	nDiamond2;
		public uint		nGold3;
	}
	struct STC_GAMECMD_OPERATE_JOIN_ALLI_NAME_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NO_ALLI				= -1,		// 联盟不存在 
			RST_ALREADY_ALLI		= -2,		// 已经有联盟了 
			RST_ALREADY_APPLY		= -3,		// 已经申请过了 
			RST_ADD_EVENT			= -4,		// 插入申请事件出错 
			RST_ALLI_FULL			= -5,		// 联盟人数已满 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int			nRst1;
		public ulong		nAllianceID2;
		public const int 	len3 = 32;
		public byte[]		szAllianceName4;
	}
	struct STC_GAMECMD_GET_MY_ALLIANCE_JOIN_EVENT_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NO_EVENT			= -1,		// 没有申请事件 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int			nRst1;
		public ulong		nAllianceID2;
		public const int 	len3=32;
		public byte[]		szAllianceName4;
	}
	struct STC_GAMECMD_IN_ALLIANCE_T  
	{
		public ulong		nAllianceID1;
	}
	struct STC_GAMECMD_OPERATE_SET_ALLI_INTRO_T 
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_NO_ALLI		= -1,	// 没有联盟 
			RST_NO_RIGHT	= -2,	// 权限不够 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
	
			RST_INTRO_TOO_LONG	= -220,	// 简介太长 
		}
		public int			nRst1;
	}
	struct STC_GAMECMD_GET_ALLIANCE_CARD_BY_ID_T 
	{
		public enum enum_rst
		{
			RST_OK			= 0,
			RST_NOT_EXIST	= -1,
		}
		public int			nRst1;
		//AllianceCard	card; 
	}
	struct STC_GAMECMD_GET_ALLIANCE_CARD_BY_RANK_T 
	{
		public uint		nTotalNum1;
		public uint		nNum2;
		// nNum个card 
	}
	// 联盟名片 
	public struct AllianceCard 
	{
		public ulong		nAllianceID1;
		public const int	len2 = 32;
		public byte[]		szAllianceName3;
		public ulong		nLeaderID4;
		public const int	len5 = 32;
		public byte[]		szLeaderName6;
		public uint			nDevelopment7;
		public uint			nTotalDevelopment8;
		public uint			nRank9;
		public uint			nLevel10;
		public uint			nMemberNum11;
		public uint			nMaxMemberNum12;
		public int			nIntroductionLen13;
		public byte[]		szIntroduction14;
	}
	struct STC_GAMECMD_GET_CITYDEFENSE_T
	{
		public enum enum_rst
		{
			RST_OK				= 0, 
			// 其他错误 
			RST_DB_ERR		= -200, 
			RST_SQL_ERR		= -201, 
		}
		public int				nRst1;
		public ulong			n1Hero2;
		public ulong			n2Hero3;
		public ulong			n3Hero4;
		public ulong			n4Hero5;
		public ulong			n5Hero6;
		public int				nAutoSupply7;
		
	}
	struct STC_GAMECMD_USE_DRUG_T 
	{
		public enum enum_rst
		{
			RST_OK				= 0,
			// 其他错误 
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public int				nRst1;
		public int				nNum2;
		// 后面跟 HeroDesc 
	}
	// 使用草药包返回 
	struct DrugDesc
	{
		public int				nNum1;			// 当前草药数量 
	}
	struct STC_GAMECMD_SUPPLY_DRUG_T
	{
		public int 				nNum1;			// 使用的数量 ... 
	}
	struct STC_GAMECMD_PVP_RST_T
	{
		public ulong		nCombatID1;			// 战斗ID
		public int			nCombatRst2;		// 战斗结果
		public int			nGold3;				// 金币
		public int			nMan4;				// 人口
	}
	
	struct LotteryEntry 
	{
		public byte	nType1;
		public uint	nData2;		// lottery_type_item->excel_id,其他为数量  
		public byte bTopReward3; 
	}
	enum lottery_type
	{
		none				= 0,			// 没有奖励  
		item				= 1,			// 道具 
		crystal				= 2,			// 水晶 
		gold				= 3,			// 黄金 
	}
	struct STC_GAMECMD_OPERATE_DRAW_LOTTERY_T 
	{
		public enum enum_rst
		{
			RST_OK			= 0,
		}
		public	int			nRst1;
		public	uint		nType2;
		public	uint		nPrice3;
		public	uint		nChoice4;
		public	int			nNum5;
		// 后面跟nNum个LotteryEntry 
	}
	struct STC_GAMECMD_OPERATE_CHECK_DRAW_LOTTERY_T
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NO_FREE				= -1,			// 免费抽奖次数已经用完
			RST_NO_DIAMOND			= -2,			// 钻石不够
			RST_NO_CRYSTAL			= -3,			// 水晶不够
			RST_ALLIANCE_DEV_NOT_ENOUGH		= -4,			// 贡献度不够
			RST_ALLIANCE_DRAW_LOTTERY_NUM_0	= -6,	// 联盟抽奖次数为0 
			RST_ERR_TYPE			= -10,			// 抽奖类型不对
	
			// 其他错误
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public	int		nRst1;
		public	uint	nType2;
		public	uint	nPrice3;			// money_type_none,price为0 
	}
	struct STC_GAMECMD_OPERATE_FETCH_LOTTERY_T
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_ADD_ITEM_FAIL		= -1,		// 添加道具失败
			RST_ERR_TYPE			= -2,		// 抽奖类型不对
	
			// 其他错误
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
	
			RST_NO_LOTTERY			= -220,		// 没有抽奖
		}
		public int			nRst1;
		public uint			nType2;
		public uint			nData3;
		public ulong		nItemID4;
		public uint			nMoneyType5;
	}
	//使用道具返回包 
	struct UseItemDesc
	{
		public int			nNum1;
	}
	struct AddItemDesc
	{
		public ulong		nItemID1;
		public int			nExcelID2;
		public int			nNum3;
	}
	struct STC_GAMECMD_CHRISTMAS_TREE_READY_RIPE_T  
	{
		// 有成熟时间事件时会有以下倒计时 
		public  uint	nBeginTime1;		 
		public  uint	nEndTime2;
	}
	public struct STC_GAMECMD_GET_CHRISTMAS_TREE_INFO_T
	{
		public enum enum_rst
		{
			RST_OK			= 0,
	
			// 其他错误
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public enum enum_state 
		{
			state_none			= 0,	
			state_ripe			= 1,	// 已经成熟可收获 
			state_can_watering	= 2,	// 可以浇水 
			state_ready_ripe	= 3,	// 准备成熟，开始成熟倒计时，已经浇满水了 
		}
		public  int		nRst1;
		public 	byte	nState2;
		public 	uint	nCountDownTime3;	// 有成熟时间事件时间倒计时 
		public  uint	nCanWaterCountDownTime4;	// 能够浇水的时间事件倒计时 
		public  uint	nWateredNum5;	// 已浇水次数  
		public  byte	bICanWater6;		// 我是否可以浇水  
	}
	public struct STC_GAMECMD_GET_OTHER_GOLDORE_INFO_T 
	{
		public enum enum_rst
		{
			RST_OK			= 0,
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public enum enum_tree_rst
		{
			state_none			= 0,	
			state_ripe			= 1,	// 已经成熟可收获 
			state_can_watering	= 2,	// 可以浇水 
			state_ready_ripe	= 3,	// 准备成熟，开始成熟倒计时，已经浇满水了 
		}
		public  int			nRst1;
		public 	ulong		nAccountID2;
		public  uint		nCharLevel3;
		public  uint		nCharHeadID4;
		public  byte 		nState5;
		public 	uint		nCountDownTime6;		// 有成熟时间事件时间倒计时  
		public 	uint		nWateredNum7;			// 已浇水次数  
		public 	byte		bICanWater8;			// 我是否可以浇水  
		public  int			nNum9;
		// 后面跟nNum个GoldoreInfo 
	}
	struct GoldoreInfo 
	{
		public enum enum_rst
		{
			state_none			= 0,
			state_producing		= 1,	// 生产中，倒计时中 
			state_can_fetch		= 2,	// 可以摘取 
		}
		public 	uint	nAutoID1;
		public 	uint	nLevel2;
		public 	uint	nCountDownTime3;
		public 	uint	nState4;
	}
	struct STC_GAMECMD_GET_GOLDORE_SMP_INFO_ALL_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
	
			RST_TOO_MANY			= -220,		// 传来的id太多了  
			RST_NEGTIVE_0_NUM		= -221,		// 数量为<=0  
		}
		public  int				nRst1;
		public 	int				nNum2;
		// 后面跟nNum个GoldoreSmpInfo  
	}
	struct GoldoreSmpInfo 
	{
		public ulong	nAccountID1;
		public byte		bCanFetchGold2;
		public byte		bCanWaterChristmasTree3;
	}
	struct STC_GAMECMD_GET_ALLIANCE_TRADE_INFO_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,		// 存在 
			RST_NO_ALLIANCE			= -1,		// 没有联盟  
			RST_NO_EMBASSY			= -2,		// 没有大使馆 
			RST_NO_CONGRESS			= -3,		// 没有联盟议会 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public  int			nRst1;
		public  uint		nCountDownTime2;
		public  uint		nGold3;
		public  uint		nTradeNum4;
		public  uint		nExcelTradeNum5;
		public  uint		nExcelTradeTime6;
		public  uint		nExcelTradeGold7;
	}
	struct STC_GAMECMD_OPERATE_ALLIANCE_TRADE_T
	{
		public enum enum_rst
		{
			RST_OK					= 0,		// 存在 
			RST_NO_ALLIANCE			= -1,		// 联盟不存在 
			RST_TRADE_NUM_0			= -2,		// 跑商次数为0 
			RST_NO_EMBASSY			= -3,		// 没有大使馆 
			RST_NO_ALLI_CONGRESS	= -4,		// 没有联盟议会 
			RST_TRADE_NOT_END		= -5,		// 已经在跑商了 
			RST_TE_DATA_ADD_FAIL	= -6,		// 时间事件数据插入失败 
			RST_TE_ADD_FAIL			= -7,		// 时间事件插入失败 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public  int		nRst1;
		public  uint	nCountDownTime2;
		public  uint	nGold3;
	}
	struct STC_GAMECMD_GET_ENEMY_LIST_T
	{
		public enum enum_rst
		{
			RST_OK					= 0,
	
			// 其他错误  
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public  int		nRst1;
		public 	uint	nNum2;
		// 后面跟nNum个EnemyUnit 
	}
	public struct EnemyUnit 
	{
		public ulong		nAccountID1;
		public const int 	len2 =32;
		public byte[]		szName3;
		public uint			nLevel4;
		public uint			nHeadID5;
	}
	struct STC_GAMECMD_NEW_ENEMY_T
	{
		//后面是一个EnemyUnit
		//EnemyUnit		enemy;
	}
	struct STC_GAMECMD_NEW_NOTIFICATION_T 
	{
	}
	struct STC_GAMECMD_OPERATE_DELETE_ENEMY_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
	
			// 其他错误  
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public  int			nRst1;
		public 	ulong		nEnemyID2;
	}
	public struct STC_GAMECMD_GET_RANK_LIST_T 
	{
		public uint		nType1;
		public uint		nMyRank2;
		public uint		nTotalNum3;
		public int		nNum4;
		// 后面跟nNum个CharRankInfo   
	}
	public struct CharRankInfo 
	{
		public ulong		nAccountID1;
		public const int 	len2=32;
		public byte[]		szName3;
		public uint			nLevel4;
		public uint			nHeadID5;
		public uint			nRank6;
		public uint			nData7;
	}
	struct STC_GAMECMD_GET_MY_RANK_T 
	{
		public uint		nType1;
		public uint		nRank2;
		public uint		nTotalNum3;
	}
	struct STC_GAMECMD_GET_IMPORTANT_CHAR_ATB_T
	{
		public uint		nExp1;
		public uint		nDiamond2;
		public uint		nCrystal3;
		public uint		nDevelopment4;
		public uint		nGold5;
		public uint		nPopulation6;
		public uint		nLevel7;
		public uint		nProtectTime8;							// 保护期剩余时间 
		public uint		nAddBuildNumTime9;						// 工匠之书剩余时间 
		public uint		nTotalBuildNum10;							// 总共的建筑队列 
		public uint		nCup11;
	}
	struct STC_GAMECMD_OPERATE_WRITE_SIGNATURE_T
	{
		public enum enum_rst
		{
			RST_OK					= 0,
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public  int			nRst1;
	}
	struct STC_GAMECMD_BUILD_ITEM_USE_OUT_T 
	{
		public int				nCurrentBuildNum1;
	}
	struct STC_GAMECMD_GET_PAY_SERIAL_T
	{
		public enum enum_rst
		{
			RST_OK					= 0,
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public  int			nRst1;
		public const int 	nLen2 = 36;
		public byte[]		szPaySerial3;
	}
	struct STC_GAMECMD_RECHARGE_DIAMOND_T
	{
		public uint	nAddedDiamond1;
		public uint	nGiftDiamond2;		// 充值附赠的钻石(增加nDiamond,不增加nTotalDiamond) 
		public uint	nVip3;
	}
	struct STC_GAMECMD_OPERATE_CHANGE_NAME_T
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NAME_DUP			= -1,	// 名字重复 
			RST_DIAMOND_NOT_ENOUGH	= -2,	// 钻石不够 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
	
			RST_NAME_INVALID		= -220,	// 名字有非法字符 
		}
		public  int			nRst1;
		public  uint		nDiamond2;
	}
	struct STC_GAMECMD_OPERATE_CHANGE_HERO_NAME_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NO_HERO				= -1,			// 英雄不存在 
			RST_NO_DIAMOND			= -2,			// 钻石不够 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public  int			nRst1;
		public  uint		nDiamond2;
	}
	struct STC_GAMECMD_READ_MAIL_ALL_T
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_ERR_TYPE			= -1,		// 类型不对 
	
			// 其他错误
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public  int			nRst1;
		public  uint		nMailType2;
	}
	struct STC_GAMECMD_DELETE_MAIL_ALL_T
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_ERR_TYPE			= -1,		// 类型不对 
	
			// 其他错误
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public  int			nRst1;
		public  uint		nMailType2;
	}
	struct STC_GAMECMD_EXPELED_ALLI_T 
	{
		
	}
	// 世界名城 
	struct	WorldCity
	{
		public ulong 		nID1;			// ID  
		public int			nClass2;			// 等级 
		public int			nPosX3;			// 坐标 
		public int			nPosY4;		
		public int			nRecover5;		// 距离恢复的时间 
		public int			nCup6;			// 奖杯数 
		public const int 	len7 = 64;		
		public byte[]		szName8;		// 名字 
		public int			nInitProf9;		// 初始战力 
 		public int			nInitRecover10;	// 恢复间隔 
		public int			nCurProf11;		// 当前战力 
	}
	struct STC_GAMECMD_WORLDCITY_GET_T 
	{
		public enum enum_rst 
		{
			RST_OK			= 0, 
			// 其他错误 
			RST_DB_ERR		= -200, 
			RST_SQL_ERR		= -201, 
		}
		public  int			nRst1;
		public 	int			nNum2;
		public 	int 		nRefreshTime3;	// 下次荣耀之城刷新时间 ...
		public  int			nEndTime4;		// 本次荣耀之城结算时间 ...
		// 后面跟 WorldCity 
	}
	// 奖杯战绩
	struct	WorldCityLog
	{
		public int		nID1;			// 名次 
		public ulong	nAccountID2;		// 账号ID 
		public const int Len3 = 32;
		public byte[] 	szName4;		// 名字  
		public int		nHeadID5;		// 头像 
		public int		nDiamond6;		// 钻石数 
		public int		nCup7;			// 奖杯数 
	}
	struct STC_GAMECMD_WORLDCITY_GETLOG_T 
	{
		public enum enum_rst 
		{
			RST_OK			= 0, 
			// 其他错误 
			RST_DB_ERR		= -200, 
			RST_SQL_ERR		= -201, 
		}
		public  int			nRst1;
		public 	int			nNum2;
		public 	int			nMaxPage3;
		// 后面跟 WorldCityLog 
	}
	// 奖杯战绩联盟 
	struct	WorldCityLogAlliance
	{
		public int			nID1;			// 名次 
		public ulong		nAllianceID2;	// 联盟ID 
		public const int 	len3 = 32;
		public byte[]		szName4;		// 名字 
		public	int			nDiamond5;		// 钻石数 
		public 	int			nCup6;			// 奖杯数 
	}
	struct STC_GAMECMD_WORLDCITY_GETLOG_ALLIANCE_T 
	{
		public enum enum_rst  
		{
			RST_OK			= 0, 
			// 其他错误 
			RST_DB_ERR		= -200, 
			RST_SQL_ERR		= -201, 
		}
		public  int			nRst1;
		public 	int			nNum2;
		public  int			nMaxPage3;
		// 后面跟 WorldCityLogAlliance
	}
	struct STC_GAMECMD_WORLDCITY_RANK_MAN_T  
	{
		public enum enum_rst 
		{
			RST_OK			= 0,
			// 其他错误
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public  int			nRst1;
		public 	int			nNum2;
		public	int			nMaxPage3;
		// 后面跟 WorldCityRankMan 
	}
	struct STC_GAMECMD_WORLDCITY_RANK_ALLIANCE_T  
	{
		public enum enum_rst 
		{
			RST_OK			= 0,
			// 其他错误
			RST_DB_ERR		= -200,
			RST_SQL_ERR		= -201,
		}
		public  int			nRst1;
		public 	int			nNum2;
		public	int			nMaxPage3;
		// 后面跟 WorldCityRankAlliance 
	}
	// 奖杯排名个人 
	struct	WorldCityRankMan
	{
		public int			nID1;				// 名次
		public ulong		nAccountID2;		// 账号ID 
		public const int	len3 = 32;
		public byte[]		szName4;		// 名字  
		public int			nHeadID5;		// 头像  
		public int			nLevel6;		// 等级  
		public int			nCup7;			// 奖杯数  
	}
	// 奖杯排名联盟 
	struct	WorldCityRankAlliance
	{
		public	int			nID1;			// 名次
		public ulong		nAllianceID2;	// 联盟ID 
		public const int	len3 = 32;
		public byte[]		szName4;		// 名字 
		public int			nLevel5;		// 等级 
		public int			nCup6;			// 奖杯数 
	}
	struct RewardUnit 
	{
		public  uint	nType1;
		public  uint	nNum2;
		public  uint	nData3;				// 道具的excel_id  
		public ulong	nID4;				// 道具的only_id  

	}
	//领取附件邮件返回 
	struct STC_GAMECMD_OPERATE_FETCH_MAIL_REWARD_T 
	{
		public enum enum_rst 
		{
			RST_OK					= 0,
			RST_MAIL_ERR			= -1,		// 邮件类型不对,(邮件不存在或者标识不对) 
			RST_REWARD_STR_ERR		= -2,		// 奖励发放的字符串不对 
			RST_ADD_ITEM_ERR		= -10,		// 添加道具失败 
			RST_ADD_DIAMOND_ERR		= -20,		// 添加钻石失败 
			RST_ADD_CRYSTAL_ERR		= -30,		// 添加水晶失败 
			RST_ADD_GOLD_ERR		= -40,		// 添加黄金失败 
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public  int			nRst1;
		public  uint		nMailID2;
		public  uint		nNum3;
		// nNum个RewardUnit 
	}
	struct STC_GAMECMD_OPERATE_UPLOAD_BILL_T 
	{
		public enum enum_rst 
		{
			RST_OK					= 0,
			RST_INSERT_FAIL			= -1,		// 插入失败 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
	
			RST_TEXT_TOO_LONG		= -220,		// 账单字串太长 
		}
		public int 			nRst1;
	}
	struct STC_GAMECMD_OPERATE_REGISTER_ACCOUNT_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NOT_EXIST			= -1,		// 账号不存在 
			RST_OLD_PASS_ERR		= -10,		// 老密码错误 
			RST_MAIL_REGISGERED		= -20,		//邮箱已经被注册 
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
	
			RST_PASS_TOO_LONG		= -220,		// 密码太长 
			RST_PASS_INVALID_LEN	= -221,		// 密码长度不合法 
			RST_CONNECT_GS_FAIL		= -222,		// 和全局服断连 
		}
		public int 			nRst1;
	}
	struct STC_GAMECMD_OPERATE_BIND_DEVICE_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_BIND_FAIL			= -1,		// 绑定失败 
			RST_BINDED_IN_GROUP 	= -2,		//已经绑定 
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
	
			RST_CONNECT_GS_FAIL		= -220,		// 和全局服断连 
		} 
		public int 			nRst1;
	}
	struct STC_GAMECMD_GET_SERVER_TIME_T  
	{
		public uint	nTimeNow1;
	}
	struct STC_GAMECMD_ADCOLONY_AWARD_T 
	{
		public uint		nMoneyType1;
		public uint		nMoneyNum2;
	}
	struct STC_GAMECMD_OPERATE_DISMISS_SOLDIER_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NUM_NOT_ENOUGH		= -1,			// 兵种数量不够 
			RST_DEL_ERR				= -2,			// 删除士兵失败 
			RST_ADD_POP_ERR			= -3,			// 增加人口失败 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int 			nRst1;
		public uint			nExcelID2;
		public uint			nLevel3;
		public uint			nNum4;
	}
	struct STC_GAMECMD_OPERATE_ALLI_DONATE_SOLDIER_T
	{
		public enum enum_rst 
		{
			RST_OK					= 0,
			RST_QUEUE_FULL			= -1,		// 派遣队列已满(一次只能一个) 
			RST_NO_ALLI				= -2,		// 没有联盟 
			RST_NOT_ONE_ALLI		= -3,		// 和目标不在同一个联盟 
			RST_LEVEL_ERR			= -4,		// 等级不够 
			RST_SOLDIER_NOT_ENOUGH	= -5,		// 士兵不够 
			RST_TODAY_FULL			= -6,		// 今日派遣已满 
			RST_ALLY_FULL			= -7,		// 对方兵营已满 
			RST_TE_DATA_ERR			= -10,		// 时间事件数据插入失败 
			RST_TE_TIME_ERR			= -11,		// 时间事件时间事件插入失败 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int 			nRst1;
		public ulong  		nObjID2;
		public uint			nExcelID3;
		public uint			nLevel4;
		public uint			nNum5;
		public uint			nTime6;
		public ulong		nEventID7;
	}
	struct STC_GAMECMD_OPERATE_ALLI_RECALL_SOLDIER_T  
	{
		public enum enum_rst 
		{
			RST_OK					= 0,
			RST_NO_EVENT			= -1,		// 事件不存在 
			RST_NO_TE				= -2,		// 事件不存在  
			RST_CANNOT_RECALL		= -3,		// 最后5s无法召回 
			RST_NEW_TE_ERR			= -4,		// 插入新事件失败 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int 			nRst1;
		public ulong		nOldEventID2;
		public ulong		nNewEventID3;
		public uint			nTime4;
	}
	struct STC_GAMECMD_OPERATE_ALLI_ACCE_SOLDIER_T
	{
		public enum enum_rst 
		{
			RST_OK					= 0,
			RST_NO_EVENT			= -1,		// 事件不存在 
			RST_DIAMOND_NOT_ENOUGH	= -2,		// 钻石不够  
			RST_CRYSTAL_NOT_ENOUGH	= -3,		// 水晶不够 
			RST_ERR_MONEY_TYPE		= -10,		// 货币类型不对 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int 			nRst1;
		public ulong		nEventID2;
		public uint			nMoneyType3;
		public uint			nMoney4;
		public uint			nTime5;
	}
	struct DONATE_SOLDIER_TE_UNIT 
	{
		public uint		nDstType1;			// 目标类型(1为自己派遣士兵,2为别人派遣士兵给自己) 
		public ulong	nPeerID2;
		public uint		nExcelID3;
		public uint		nLevel4;
		public uint		nNum5;
		public uint		nType6;				// 1为出征,2为召回  
		public ulong	nEventID7;
		public uint		nTime8;
		public const int len9 = 32;
		public byte[]	szCharName10;
	}
	struct STC_GAMECMD_GET_DONATE_SOLDIER_QUEUE_T 
	{
		
		public enum enum_rst
		{
			RST_OK					= 0,  
	
			// 其他错误  
			RST_DB_ERR				= -200, 
			RST_SQL_ERR				= -201, 
		} 
		public int 			nRst1;
		public uint			nNum2;
		//后面跟nnum个DONATE_SOLDIER_TE_UNIT  
	}
	struct STC_GAMECMD_DONATE_SOLDIER_TE_T 
	{
		public ulong		nEventID1;
		public uint			nExcelID2;
		public uint			nLevel3;
		public uint			nNum4;
		public uint			nType5;
	}
	struct STC_GAMECMD_GET_LEFT_DONATE_SOLDIER_T
	{
		public enum enum_rst
		{
			RST_OK					= 0,
	
			// 其他错误 
			RST_DB_ERR				= -200, 
			RST_SQL_ERR				= -201, 
		}
		public int 			nRst1;
		public uint			nLeftNum2;
		public uint			nMaxNum3;
	}
	
	struct STC_GAMECMD_GET_GOLD_DEAL_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int 			nRst1;
		public uint			nPage2;
		public uint			nTotalNum3;
		public uint			nNum4;
		// 后面跟nNum个GoldMarketDeal_T 
	}
	public struct GoldMarketDeal_T 
	{
		public ulong	nTransactionID1;
		public uint		nGold2;
		public uint		nPrice3;
		public uint		nTotalPrice4;
		public ulong	nAccountID5;
	}
	struct STC_GAMECMD_GET_SELF_GOLD_DEAL_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
	
			// 其他错误  
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int 			nRst1;
		public uint			nNum2;
		// 后面跟nNum个GoldMarketDeal_T 
	}

	struct STC_GAMECMD_OPERATE_ADD_GOLD_DEAL_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_TOO_MANY			= -1,	// 交易太多,一次只能一笔 
			RST_LEVEL_LOW			= -2,	// 等级太低 
			RST_GOLD_NOT_ENOUGH		= -3,	// 黄金不够  
			RST_ADD_DEAL_ERR		= -4,	// 添加交易失败 
			RST_GOLD_0				= -10,	// 黄金出售0 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int 			nRst1;
		public uint			nGold2;
		public ulong		nTransactionID3;
	}
	struct STC_GAMECMD_OPERATE_BUY_GOLD_IN_MARKET_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_DEAL_NOT_EXISTS		= -1,		// 交易不存在 
			RST_CANNOT_BUY_SELF		= -2,		// 不能自买自卖 
			RST_LEVEL_TOO_LOW		= -3,  		//等级不够 
			RST_DEAL_DEL_ERR		= -4,		// 删除交易失败 
			RST_DEL_MONEY_ERR		= -5,		// 扣款失败 
			RST_ADD_BUYER_MAIL_ERR	= -6,		// 发送买方邮件失败 
			RST_ADD_SELLER_MAIL_ERR	= -7,		// 发送卖方邮件失败 
			RST_DIAMOND_NOT_ENOUGH	= -10,		// 钻石不够 
			RST_CRYSTAL_NOT_ENOUGH	= -11,		// 水晶不够 
			RST_MONEY_TYPE_ERR		= -20,		// 货币类型不对 
	
			// 存储过程执行失败 
			RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
			RST_SELECT_INTO			= -101,		// select into出错 
			RST_VALUE_OVERFLOW		= -102,		// 值溢出 
			RST_DEADLOCK			= -103,		// 死锁 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int 			nRst1;
		public uint			nMoneyType2;
		public uint			nTotalPrice3;
		public uint			nGold4;
	}
	struct STC_GAMECMD_OPERATE_CANCEL_GOLD_DEAL_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_DEAL_NOT_EXISTS		= -1,		// 交易不存在 
			RST_CANCEL_INTERVAL		= -2,		// 取消交易时间太短 
			RST_ADD_GOLD_FAIL		= -3,		// 增加黄金失败 
	
			// 其他错误  
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int 			nRst1;
		public uint			nGold2;
	}
	struct STC_GAMECMD_GET_WOLRD_RES_T  
	{
		public uint	nFromX1;
		public uint	nFromY2;
		public uint	nToX3;
		public uint	nToY4;
		public uint	nNum5;
		// 后面跟nNum个WorldRes   
	} 
	struct WorldResSimpleInfo  
	{
		public ulong		nID1;
		public uint			nType2;
		public uint			nLevel3;
		public uint			nPosX4;
		public uint			nPosY5;
	}
	struct STC_GAMECMD_GET_WORLD_RES_CARD_T  
	{
		public enum enum_rst
		{
			RST_OK				= 0,
			RST_NOT_EXIST		= -1,		// 不存在
		}
		public int 			nRst1;
		//后面跟一个WorldRes
	}
	public struct WorldRes 
	{
		public ulong		nID1;
		public uint			nType2;
		public uint			nLevel3;
		public uint			nPosX4;
		public uint			nPosY5;
		public uint			nGold6;
		public uint			nPop7;
		public uint			nCrystal8;
		public uint			nForce9;	
	}
	struct STC_GAMECMD_WORLD_RES_CLEAR_T   
	{
		public uint	nPosX1;
		public uint	nPosY2;
	}
	struct STC_GAMECMD_GET_KICK_CLIENT_ALL_T
	{
		public enum enum_rst
		{
			RST_OK					= 0,
	
			// 其他错误  
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int		nRst1; 
		public int		nLength2; 
		public byte[]   strbuff3;
		// 后面跟字串  
	}
    //////////////////////////////////////////////
    /// 团队副本
    /////////////////////////////////////////////

    public struct STC_GAMECMD_ALLI_INSTANCE_CREATE_T
    {
	    public enum enum_rst
	    {
		    RST_OK					= 0,
		    RST_NO_ALLI				= -1,		// 自己没有联盟 
		    RST_ADD_INSTANCE_ERR	= -2,		// 添加副本出错 
		    RST_SET_STATUS			= -3,		// 修改自己状态出错 
		    RST_EXISTS_INSTANCE		= -1001,	// 有副本未退出 
		    RST_NO_INSTANCE_EXCEL	= -1002,	// excel中未配置该副本 
		    RST_LEVEL_LOW			= -1003,	// 等级太低 
		    RST_NOT_IN_TIME			= -1004,	// 不在开放时间段 
		    RST_NO_TIMES_LEFT		= -1005,	// 没有可用次数了 
		    RST_NO_INSTANCE			= -1010,	// 副本不存在 
		    RST_TEAM_FULL			= -1011,	// 队伍已满 
		    RST_INSTANCE_START		= -1012,	// 副本已经开始了 

		    // 存储过程执行失败 
		    RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
		    RST_SELECT_INTO			= -101,		// select into出错 
		    RST_VALUE_OVERFLOW		= -102,		// 值溢出 
		    RST_DEADLOCK			= -103,		// 死锁 

		    // 其他错误 
		    RST_DB_ERR				= -200,
		    RST_SQL_ERR				= -201,
	    };
	    public int nRst1;
	    public ulong nInstanceID2;
    }

    public struct STC_GAMECMD_ALLI_INSTANCE_JOIN_T 
    {
	    public enum enum_rst
	    {
		    RST_OK					= 0,
		    RST_NO_ALLI_INSTANCE	= -1,		// 副本不存在 
		    RST_NOT_SAME_ALLI		= -2,		// 不是同一个联盟 
		    RST_ROOM_FULL			= -3,		// 队伍已满 
		    RST_CHANGE_INFO_FAIL	= -4,		// 修改信息未成功 
		    RST_EXISTS_INSTANCE		= -1001,	// 有副本未退出 
		    RST_NO_INSTANCE_EXCEL	= -1002,	// excel中未配置该副本 
		    RST_LEVEL_LOW			= -1003,	// 等级太低 
		    RST_NOT_IN_TIME			= -1004,	// 不在开放时间段 
		    RST_NO_TIMES_LEFT		= -1005,	// 没有可用次数了 
		    RST_NO_INSTANCE			= -1010,	// 副本不存在 
		    RST_TEAM_FULL			= -1011,	// 队伍已满 
		    RST_INSTANCE_START		= -1012,	// 副本已经开始了 

		    // 存储过程执行失败 
		    RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错  
		    RST_SELECT_INTO			= -101,		// select into出错 
		    RST_VALUE_OVERFLOW		= -102,		// 值溢出 
		    RST_DEADLOCK			= -103,		// 死锁 

		    // 其他错误 
		    RST_DB_ERR				= -200,
		    RST_SQL_ERR				= -201,
	    };
	    public int nRst1;
    }

    public struct STC_GAMECMD_ALLI_INSTANCE_EXIT_T 
    {
	    public enum enum_rst
	    {
		    RST_OK					= 0,
		    RST_NO_INSTANCE			= -1,		// 副本不存在 
		    RST_COMBAT				= -2,		// 战斗中 
		    RST_UNLOCK_ERR			= -3,		// 将领解锁失败 
		    RST_CHANGE_CHAR_INFO	= -4,		// 修改玩家状态失败 
		    RST_CHANGE_INST_INFO	= -5,		// 修改副本失败 
		    RST_DESTROY_INSTANCE	= -6,		// 销毁副本失败 
		    RST_CHANGE_LEADER		= -7,		// 转让队长失败 

		    // 存储过程执行失败  
		    RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
		    RST_SELECT_INTO			= -101,		// select into出错 
		    RST_VALUE_OVERFLOW		= -102,		// 值溢出 
		    RST_DEADLOCK			= -103,		// 死锁 

		    // 其他错误 
		    RST_DB_ERR				= -200,
		    RST_SQL_ERR				= -201,
	    };
	    public int nRst1;
    }

    public struct STC_GAMECMD_ALLI_INSTANCE_KICK_T
    {
	    public enum enum_rst
	    {
		    RST_OK					= 0,
		    RST_NOT_LEADER			= -1,		// 不是队长 
		    RST_COMBAT				= -2,		// 战斗中不能踢人 
		    RST_EXIT_INSTANCE		= -3,		// 退出副本失败 

		    // 存储过程执行失败 
		    RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
		    RST_SELECT_INTO			= -101,		// select into出错 
		    RST_VALUE_OVERFLOW		= -102,		// 值溢出 
		    RST_DEADLOCK			= -103,		// 死锁 

		    // 其他错误 
		    RST_DB_ERR				= -200,
		    RST_SQL_ERR				= -201,
	    };
	    public int nRst1;
        public ulong nAccountID2;
    }

    public struct STC_GAMECMD_ALLI_INSTANCE_DESTROY_T
    {
	    public enum enum_rst
	    {
		    RST_OK					= 0,
		    RST_COMBAT				= -1,		// 战斗中 
		    RST_NOT_LEADER			= -2,		// 不是队长 
		    RST_EXIT_ERR			= -3,		// 退出副本失败 

		    // 存储过程执行失败 
		    RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
		    RST_SELECT_INTO			= -101,		// select into出错 
		    RST_VALUE_OVERFLOW		= -102,		// 值溢出 
		    RST_DEADLOCK			= -103,		// 死锁 

		    // 其他错误 
		    RST_DB_ERR				= -200,
		    RST_SQL_ERR				= -201,
	    };
	    public int nRst1;
    }

    public struct STC_GAMECMD_ALLI_INSTANCE_CONFIG_HERO_T
    {
	    public enum enum_rst
	    {
		    RST_OK					= 0,
		    RST_HERO_NOT_SELF		= -1,		// 英雄不是自己的 
		    RST_NO_IDLE				= -2,		// 副本不是空闲状态 
		    RST_NOT_IN_ISNTANCE		= -3,		// 不在副本中 
		    RST_UNLOCK_HERO_FAIL	= -4,		// 解锁英雄失败 
		    RST_LOCK_HERO_FAIL		= -5,		// 锁定英雄失败 
		    RST_DEPLOY_HERO			= -6,		// 配置武将失败 

		    // 存储过程执行失败 
		    RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
		    RST_SELECT_INTO			= -101,		// select into出错 
		    RST_VALUE_OVERFLOW		= -102,		// 值溢出 
		    RST_DEADLOCK			= -103,		// 死锁 

		    // 其他错误 
		    RST_DB_ERR				= -200,
		    RST_SQL_ERR				= -201,
	    };
	    public int nRst1;
    }

    public struct STC_GAMECMD_ALLI_INSTANCE_START_T
    {
	    public enum enum_rst
	    {
		    RST_OK					= 0,
		    RST_NOT_LEADER			= -1,		// 不是队长 
		    RST_NOT_READY			= -2,		// 有人未就绪 
		    RST_NO_PLAYER			= -3,		// 没人也不能开始 
		    RST_SET_STAUTS			= -4,		// 修改副本状态失败 

		    // 存储过程执行失败 
		    RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
		    RST_SELECT_INTO			= -101,		// select into出错 
		    RST_VALUE_OVERFLOW		= -102,		// 值溢出 
		    RST_DEADLOCK			= -103,		// 死锁 

		    // 其他错误 
		    RST_DB_ERR				= -200,
		    RST_SQL_ERR				= -201,
	    };
	    public int nRst1;
    }

    public struct STC_GAMECMD_ALLI_INSTANCE_READY_T
    {
	    public enum enum_rst
	    {
		    RST_OK					= 0,
		    RST_NO_INSTANCE			= -1,		// 副本不存在 
		    RST_NO_HERO				= -2,		// 没有配置将领 
		    RST_SET_INFO_FAIL		= -3,		// 设置状态失败 

		    // 存储过程执行失败 
		    RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
		    RST_SELECT_INTO			= -101,		// select into出错 
		    RST_VALUE_OVERFLOW		= -102,		// 值溢出 
		    RST_DEADLOCK			= -103,		// 死锁 

		    // 其他错误 
		    RST_DB_ERR				= -200,
		    RST_SQL_ERR				= -201,
	    };
	    public int nRst1;
    }

    public struct STC_GAMECMD_ALLI_INSTANCE_GET_LIST_T
    {
	    public enum enum_est
	    {
		    RST_OK					= 0,

		    // 存储过程执行失败 
		    RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
		    RST_SELECT_INTO			= -101,		// select into出错 
		    RST_VALUE_OVERFLOW		= -102,		// 值溢出 
		    RST_DEADLOCK			= -103,		// 死锁 

		    // 其他错误 
		    RST_DB_ERR				= -200,
		    RST_SQL_ERR				= -201,
	    };
	
	    public int nRst1;
	    public int nNum2;

        public struct GBTeamData
        {
            public ulong nInstanceID1;
            public uint nPlayerNum2;
            public ulong nCreatorID3;
            public const int szCreatorNameLen4 = 32;
            public byte[] szCreatorName5; // szCreatorName[TTY_CHARACTERNAME_LEN];
            public uint nLevel6;
            public uint nHeadID7;//君主头像 
        }
    }

    public struct STC_GAMECMD_ALLI_INSTANCE_GET_CHAR_DATA_T
    {
	    public enum enum_rst
	    {
		    RST_OK					= 0,

		    // 存储过程执行失败 
		    RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
		    RST_SELECT_INTO			= -101,		// select into出错 
		    RST_VALUE_OVERFLOW		= -102,		// 值溢出 
		    RST_DEADLOCK			= -103,		// 死锁 

		    // 其他错误 
		    RST_DB_ERR				= -200,
		    RST_SQL_ERR				= -201,
	    };
	
	    public int nRst1;
        public ulong nInstanceID2;
	    public int	nNum3;

        public struct GBCharData
        {
            public uint nStatus1;
            public ulong nAccountID2;
            public const int szName3 = 32;
            public byte[] szName4;//szName[TTY_CHARACTERNAME_LEN];
            public uint nLevel5;
            public uint nHeadID6;
        }
    }

    public struct STC_GAMECMD_ALLI_INSTANCE_GET_HERO_DATA_T
    {
	    public enum enum_est
	    {
		    RST_OK					= 0,

		    // 存储过程执行失败 
		    RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
		    RST_SELECT_INTO			= -101,		// select into出错 
		    RST_VALUE_OVERFLOW		= -102,		// 值溢出 
		    RST_DEADLOCK			= -103,		// 死锁 

		    // 其他错误 
		    RST_DB_ERR				= -200,
		    RST_SQL_ERR				= -201,
	    }
	
	    public int nRst1;
        public ulong nInstanceID2;
	    public int nNum3;

        public struct GBHeroData
        {
            public ulong nHeroID1;
            public const int szHeroNameLen2 = 32;
            public byte[] szHeroName3; //szHeroName[TTY_CHARACTERNAME_LEN];
            public ulong nAccountID4;
            public uint nLevel5;
            public uint nHealth6;
            public uint nProf7;
            public uint nArmyType8;
            public uint nArmyLevel9;
            public uint nArmyNum10;
            public uint nRow11;
            public uint nCol12;
            public uint nHeadID13;
        }
    }

    struct STC_GAMECMD_ALLI_INSTANCE_GET_STATUS_T
    {
	    public enum enum_rst
	    {
		    RST_OK					= 0,

		    // 存储过程执行失败 
		    RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
		    RST_SELECT_INTO			= -101,		// select into出错 
		    RST_VALUE_OVERFLOW		= -102,		// 值溢出 
		    RST_DEADLOCK			= -103,		// 死锁 

		    // 其他错误 
		    RST_DB_ERR				= -200,
		    RST_SQL_ERR				= -201,
	    };
        public int nRst1;
	    public uint	nTotalFreeTimes2;
	    public uint	nUsedFreeTimes3;
	    public ulong nInstanceID4;
	    public uint	nCharStatus5;				// instance_player_status_xxx
	    public uint	nInstanceStatus6;			// instance_status_xxx
	    public uint	nCurLevel7;
	    public byte bAutoCombat8;
	    public byte bAutoSupply9;
	    public uint	nRetryTimes10;
        public ulong nCreatorID11;
        public uint nStopLevel12;
        public uint nLastresult13;
    }

    public struct STC_GAMECMD_ALLI_INSTANCE_SAVE_HERO_DEPLOY_T 
    {
	    public enum enum_rst
	    {
		    RST_OK					= 0,
		    RST_NOT_LEADER			= -1,		// 不是队长 
		    RST_INSTANCE_EMPTY		= -2,		// 副本为空 

		    // 存储过程执行失败 
		    RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
		    RST_SELECT_INTO			= -101,		// select into出错 
		    RST_VALUE_OVERFLOW		= -102,		// 值溢出 
		    RST_DEADLOCK			= -103,		// 死锁 

		    // 其他错误 
		    RST_DB_ERR				= -200,
		    RST_SQL_ERR				= -201,
	    };
	    public int nRst1;
    }

    public struct STC_GAMECMD_ALLI_INSTANCE_GET_LOOT_T
    {
	    public enum enum_rst
	    {
		    RST_OK					= 0,

		    // 存储过程执行失败 
		    RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
		    RST_SELECT_INTO			= -101,		// select into出错 
		    RST_VALUE_OVERFLOW		= -102,		// 值溢出 
		    RST_DEADLOCK			= -103,		// 死锁 

		    // 其他错误 
		    RST_DB_ERR				= -200,
		    RST_SQL_ERR				= -201,
	    };
	    public int nRst1;
        public int nNum2;
        public struct LootUnit 
	    {
		    public uint	nExcelID1;
		    public uint	nNum2;
	    };
    }

    public struct STC_GAMECMD_ALLI_INSTANCE_START_COMBAT_T
    {
	    public enum enum_rst
	    {
		    RST_OK					= 0,
			RST_NO_HERO				= -1,		// 必须配置将领才能出征 
			RST_NO_INSTANCE			= -2,		// 副本不存在 
			RST_CANNOT_FIGHT		= -3,		// 不能开战(不是队长,或者还处于战斗中) 
			RST_NO_OBJ				= -4,		// 目标不存在 
			RST_SAVE_HERO_DEPLOY	= -5,		// 保存将领布局失败 
			RST_ADD_TE_ERR			= -10,		// 添加时间事件失败 
			RST_ARENA_NEED_UPLOAD	= -14,		// 竞技场必须先上传数据  
			RST_ARENA_RANK_LOW		= -15,		// 排名比自己低  
			RST_ARENA_RANK_HIGH		= -16,		// 排名比自己高太多 
			RST_ARENA_NO_TRIES		= -17,		// 排名比自己高太多  
			RST_ERR_COMBAT_TYPE		= -20,		// 战斗类型不对 
			RST_CHECK_HERO_FAIL		= -30,		// 校验英雄失败 

		    // 存储过程执行失败 
		    RST_DB_EXE_ERR			= -100,		// 数据库执行存储过程出错 
		    RST_SELECT_INTO			= -101,		// select into出错 
		    RST_VALUE_OVERFLOW		= -102,		// 值溢出 
		    RST_DEADLOCK			= -103,		// 死锁 

		    // 其他错误 
		    RST_DB_ERR				= -200,
		    RST_SQL_ERR				= -201,
	    };
	    public int nRst1;
    };

    public struct STC_GAMECMD_ALLI_INSTANCE_ACTION_NOTIFY_T
    {
	    public ulong nCharID1;
	    public ulong nInstanceID2;
	    public int nAction3;
        public enum GB_SERVER_ACTION
        {
            AI_ACTION_JOIN = 1,		// 加入  
            AI_ACTION_READY = 2,		// 准备就绪  
            AI_ACTION_UNREADY = 3,		// 取消准备就绪  
            AI_ACTION_EXIT = 4,		// 退出  
            AI_ACTION_KICK = 5,		// 被踢出  
            AI_ACTION_DESTROY = 6,		// 队长销毁副本   
            AI_ACTION_SYS_DESTROY = 7,		// 系统销毁副本     
            AI_ACTION_START = 8,		// 副本开始,进入副本场景     
            AI_ACTION_START_COMBAT = 9,		// 开始战斗  
        }
    }

    /// <summary>
    /// 战斗完毕回调
    /// </summary>
    public struct STC_GAMECMD_ALLI_INSTANCE_BACK {
        public byte bInstanceDestroyed1;
        public int nCombatType2;
        public ulong nObjID3;
        public uint	nInstanceLevel4;
        public ulong nEventID5;
    }

    /// <summary>
    /// 简单将领信息
    /// </summary>
    public struct STC_GAMECMD_HERO_SIMPLE_DATA_T { 
		public enum reason_type
		{
			reason_auto_supply		= 1,		// 自动补给 
			reason_self_supply		= 2,		// 手动补给 
			reason_fight_cost		= 3,		// 战斗损耗 
		};

        public uint	nCombatType1;
		public uint	nReason2;
        public int  nNum3;
    }
	public struct HeroSimpleData
    {
        public ulong 	nHeroID1;
        public int 	 	nArmyType2;
        public int	 	nArmyLevel3;
        public int 	 	nArmyNum4;
        public int 		nProf5;
        public int 		nHealthState6;
        public int 		nLevel7;
        public int 		nExp8;
        public int 		nStatus9;
    }
    /// <summary>
    /// 简单战报
    /// </summary>
    public struct STC_GAMECMD_ALLI_INSTANCE_GET_SIMPLE_COMBAT_LOG_T
    {
        public enum enum_rst
        {
            RST_OK = 0,
            // 其他错误
            RST_DB_ERR = -200,
            RST_SQL_ERR = -201,
            RST_NO_RECORD = -220,		// 没有记录
        };
        public int nRst1;
        public uint nBackLeftTime2;			//倒计时
        public uint nCombatType3;           //战斗类型参考CMSCmd.HeroState
        public uint nCombatResult4;         //战斗结果，1成功，2失败
        public uint nInstanceLevel5;        //当前的关卡值
        public ulong nObjID6;               //副本id
        public ulong nEventID7;
    }

    /// <summary>
    /// 复杂战报 
    /// </summary>
    public struct STC_GAMECMD_ALLI_INSTANCE_GET_COMBAT_LOG_T
    {
        public enum enum_rst
        {
            RST_OK = 0,
            // 其他错误
            RST_DB_ERR = -200,
            RST_SQL_ERR = -201,

            RST_NO_RECORD = -220,		// 没有记录
        }
        public int nRst1;
        public uint nCombatType2;
        public ulong nObjID3;
        // 战报AlliInstanceCombatLog
    }

	public struct AlliInstanceCombatLog 
	{
		// combat_result,战斗结果 
		public enum enum_rst
		{
			combat_result_win							= 1,		// 成功 
			combat_result_lose							= 2,		// 失败 
		}; 
		public int			nCombatResult1;
		public uint			nCombatType2;				// 战斗类型 
		public ulong		nAttackAccID3;
		public ulong		nDefenseAccID4;
		public int			nRandSeed5;					// 种子类型 
		public int			nAttackNum6;
		public int			nDefenseNum7;
		public int			nLootNum8;  
		public float		fcounterrate9;				//克制比例 
		// int				nAttackNum; 
		// HeroUnit数组 
		// int				nDefenseNum; 
		// HeroUnit数组 
		// int				nLootNum; 
		// LootUnit数组 
	}
	//英雄简单信息 
	public struct HeroUnit 
	{
		public const int	len1 = 32;
		public byte[]		szHeroName2;
		public uint			nHeroLevel3;
		public uint			nProf4;
		public uint			nRow5;
		public uint			nCol6;
		public uint			nArmyType7;
		public uint			nArmyLevel8;
		public uint			nArmyNum9;
		public uint			nArmyDeathNum10;
		public uint			nArmyRestoreNum11;
		public ulong		nHeroID12;
		public const int	len13 = 32;
		public byte[]		szCharName14;
	}
	//掠夺物品结构体 
	public struct LootUnit 
	{
		// 以后最好在同一个地方把所有的东西分类,thing_type_xxx 
		public enum enum_rst
		{
			loot_type_none				= 0,		// 无 
			loot_type_gold				= 1,		// 黄金  
			loot_type_crystal			= 2,		// 水晶 
			loot_type_diamond			= 3,		// 钻石 
			loot_type_pop				= 4,		// 人口 
			loot_type_item				= 5,		// 道具 
		};
		public uint		nType1;
		public uint		nExtData2;
		public uint		nNum3;
	}

    /// <summary>
    /// 停止自动战斗返回 
    /// </summary>
    public struct STC_GAMECMD_ALLI_INSTANCE_STOP_COMBAT_T
    {
	    public enum enum_rst
	    {
		    RST_OK					= 0,
		    RST_NOT_CREATOR			= -1,			// 不是队长

		    // 其他错误
		    RST_DB_ERR				= -200,
		    RST_SQL_ERR				= -201,
	    };
	    public int	nRst1;
	    public uint nCombatType2;
	    public ulong nInstanceID3;
    }

    /// <summary>
    /// 手动补给返回 
    /// </summary>
    public struct STC_GAMECMD_ALLI_INSTANCE_SUPPLY_T
    {
	    public enum enum_rst
	    {
		    RST_OK					= 0,
		    RST_ERR_STATUS			= -1,			// 副本状态必须为出征,且必须是队长
		    RST_SUPPLY_FAILED		= -2,			// 补给失败

		    // 其他错误
		    RST_DB_ERR				= -200,
		    RST_SQL_ERR				= -201,
	    };
	    public int nRst1;
	    public uint nCombatType2;
        public ulong nInstanceID3;
    }
	struct STC_GAMECMD_ARENA_UPLOAD_DATA_T 
	{
		public enum enum_rst 
		{
			RST_OK					= 0,
			RST_LEVEL_LOW			= -1,			// 等级太低 
			RST_UPLOAD_FAILED		= -2,			// 上传失败 
			RST_UPLOAD_CD			= -3,			//CD 
			RST_NO_TROOP			= -4,			//没有配兵 
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		};
		public int		nRst1;
		public uint		nRank2;
		public byte		bFirstUpload3;	// 是否第一次上传数据 
	}
	public struct STC_GAMECMD_ARENA_GET_RANK_LIST_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0, 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		};
		public int		nRst1;
		public uint		nTotalNum2;
		public uint		nPage3;
		public int		nNum4;
	}
	public struct RankListUnit 
	{
		public uint		nRank1;
		public ulong	nAccountID2;
		public const int len3 = 32;
		public byte[]	szName4;
		public uint		nLevel5;
		public uint		nHeadID6;
		public uint		nTotalForce7;
		public const int len8 = 32;
		public byte[]	szAllianceName9;
	}
	public struct STC_GAMECMD_ARENA_GET_STATUS_T  
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_NOT_UPLOAD			= -1,		// 没有上传排行榜数据 
	
			// 其他错误
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		};
		public int		nRst1;
		public uint		nChallengeTimesLeft2;	// 今日剩余挑战次数 
		public uint		nUploadTimeLeft3;		// 下一次可以上传镜像的CD 
		public uint		nPayTimesLeft4;			// 付费次数剩余 
		public uint		nRank5;					// 我的排名
		public int		nNum6;
		// 后面跟nNum个ListUnit 
	}
	public struct ChallengeListUnit 
	{
		public uint		nRank1;
		public ulong 	nAccountID2;
		public const int len3 = 32;
		public byte[]	szName4;
		public uint		nLevel5;
		public uint		nHeadID6;
		public uint 	nTotalForce7;
	}
	struct STC_GAMECMD_ARENA_PAY_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0,
			RST_FREE_TIMES_LEFT		= -1,		// 还有剩余次数 
			RST_NO_PAY_TIMES		= -2,		// 没有付费次数 
			RST_DIAMOND_NOT_ENOUGH	= -3,		// 钻石不足 
			RST_CRYSTAL_NOT_ENOUGH	= -4,		// 水晶不足 
			RST_PAY_FAILED			= -5,		// 付费失败 
			RST_ERR_MONEY_TYPE		= -10,		// 货币类型错误 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		};
		public int		nRst1;
		public uint		nMoneyType2;
		public uint		nPrice3;
	}
	struct STC_GAMECMD_ARENA_GET_DEPLOY_T 
	{
		public enum enum_rst
		{
			RST_OK					= 0, 
			RST_NOT_UPLOAD			= -1,		// 没有上传数据 
	
			// 其他错误
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		};
		public int		nRst1;
		public uint		nNum2;
		// 后面跟ArenaHeroDeploy[nNum]  
	}
	public struct ArenaHeroDeploy 
	{
		public uint		nCol1;
		public uint		nRow2;
		public ulong	nHeroID3;
		public int		nArmyType4;
		public int		nArmyLevel5;
		public int		nArmyNum6;
		public int		nProf7;
	}
	struct STC_GAMECMD_HERO_SUPPLY_T 
	{
		public enum enum_rst 
		{
			RST_OK					= 0,
			RST_SUPPLY_ERR			= -1,	// 补给失败 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
	
			RST_TOO_MANY_HERO		= -220,	// 太多英雄 
		}
		public int		nRst1;
	}
	struct STC_GAMECMD_OPERATE_SET_VIP_DISPLAY_T 
	{
		public enum enum_rst 
		{
			RST_OK					= 0,
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public int		nRst1;
		public byte		bVipDisplay2;
	}
	struct ArmyUnit
	{
		public  uint		nArmyType1;
		public  uint		nForce2;
		public  uint		nRow3;
		public  uint		nCol4;
	}
	struct STC_GAMECMD_ALLI_INSTANCE_GET_DEFENDER_DEPLOY_T 
	{
		public enum enum_rst 
		{
			RST_OK					= 0,
			RST_NOT_EXISTS			= -1,
	 
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		}
		public  int		nRst1;
		public  uint	nExcelID2;			// 副本ID 
		public  uint	nLevel3; 
		public  uint	nNum4; 
	}
	struct STC_GAMECMD_POSITION_MARK_ADD_RECORD_T 
	{
		public enum enum_rst 
		{
			RST_OK					= 0,
			RST_FULL				= -1,		// 收藏夹满 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
	
			RST_PROMPT_TOO_LONG		= -220,		// 注释太长 
		}
		public  int		nRst1;
	}
	struct STC_GAMECMD_POSITION_MARK_CHG_RECORD_T 
	{
		public enum enum_rst 
		{
			RST_OK					= 0,
			RST_FAIL				= -1,		// 修改失败(可能是因为没变化或者记录不存在) 
	
			// 其他错误  
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
	
			RST_PROMPT_TOO_LONG		= -220,		// 注释太长  
		}
		public  int		nRst1;
	}
	struct STC_GAMECMD_POSITION_MARK_DEL_RECORD_T 	
	{
		public enum enum_rst 
		{
			RST_OK					= 0,
			RST_FAIL				= -1,		// 删除失败(可能是因为记录不存在) 
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
	
			RST_PROMPT_TOO_LONG		= -220,		// 注释太长
		}
		public  int		nRst1;
	}
	public struct PositionMark 
	{
		public uint	nPosX1;
		public uint	nPosY2;
		public uint	nHeadID3;
		public int	nLength4;
		public byte[] szName5;
	}
	struct STC_GAMECMD_POSITION_MARK_GET_RECORD_T 
	{
		public enum enum_rst 
		{
			RST_OK					= 0,
	
			// 其他错误 
			RST_DB_ERR				= -200,
			RST_SQL_ERR				= -201,
		} 
		public  int		nRst1;
		public  int		nNum2;
		//nNum2个PositionMark 
	}
}

