using UnityEngine;
using System;
using System.Collections;
using System.IO;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class GameLogicMsg : MonoBehaviour {
	/// <summary> 
	/// 以下是逻辑二级指令 除登陆外所有的逻辑都属于二级逻辑指令 均在此处理分发 
	/// </summary>/// <summary> 
	public static void DealCmdIn(byte[] pData)	
	{	
	
		TTY_LPGAMEPLAY_CLIENT_GAME_CMD_T  msg = DataConvert.ByteToStruct<TTY_LPGAMEPLAY_CLIENT_GAME_CMD_T>(pData);
		//拆分子包  
		int headcmdlen = U3dCmn.GetSizeofSimpleStructure<TTY_LPGAMEPLAY_CLIENT_GAME_CMD_T>();
		int subcmdlen = pData.Length - headcmdlen;
		byte[] SubCmdBuff = new byte[subcmdlen];
		Array.Copy(pData,headcmdlen,SubCmdBuff,0,subcmdlen);
		LoadingManager.instance.LoginRemoveInitialData((int)msg.nGameCmd2);
		LoadingManager.instance.CheckCmdReturn((int)msg.nGameCmd2);
		switch((int)msg.nGameCmd2)
		{
			case (int)GAME_SUB_RST.STC_GAMECMD_CHAT_ALLIANCE: //聊天联盟频道  
			{
				U3dCmn.SendMessage("ChatManager","ProcessAllanceData",SubCmdBuff);	
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_CHAT_WORLD: //聊天世界频道 
			{
				U3dCmn.SendMessage("ChatManager","ProcessWorldData",SubCmdBuff);
			}
			break; 
			case (int)GAME_SUB_RST.STC_GAMECMD_SYS_REPORT: //聊天系统频道  
			{
				U3dCmn.SendMessage("ChatManager","ProcessSysData",SubCmdBuff);
			}
			break; 
			case (int)GAME_SUB_RST.STC_GAMECMD_CHAT_PRIVATE: //聊天私聊频道 
			{
				U3dCmn.SendMessage("ChatManager","ProcessPrivateData",SubCmdBuff);
			}
			break; 
			case (int)GAME_SUB_RST.STC_GAMECMD_CHAT_GROUP: //聊天队伍频道 
			{
				U3dCmn.SendMessage("ChatManager","ProcessTeamData",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_BUILDING_LIST: //获取建筑列表 
			{
			  	U3dCmn.SendMessage("BuildingManager","ProcessBuidingList",SubCmdBuff);
			}
			break; 
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_CHAR_ATB: //获取玩家自身的基本数据 
			{
			  	U3dCmn.SendMessage("PlayerInfoManager","ProcessPlayerInfo",SubCmdBuff);
			}
			break; 
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_IMPORTANT_CHAR_ATB: //获取玩家的重要数据 
			{
			  	U3dCmn.SendMessage("PlayerInfoManager","GetPlayerSmpInfoRst",SubCmdBuff);
			}
			break; 
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_BUILD_BUILDING: //请求建造的结果   
			{
			  	U3dCmn.SendMessage("BuildingManager","ProcessBuidRst",SubCmdBuff);
			}
			break;  
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_UPGRADE_BUILDING: //请求升级的结果  
			{
			  	U3dCmn.SendMessage("BuildingManager","ProcessUpgradeRst",SubCmdBuff);
			}
			break;  
			case (int)GAME_SUB_RST.STC_GAMECMD_BUILDING_TE: //建筑完成后的通知 
			{
			  	U3dCmn.SendMessage("BuildingManager","ProcessBuidEventRst",SubCmdBuff);
			}
			break; 
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_BUILDING_TE_LIST: //建筑时间队列 
			{
				U3dCmn.SendMessage("BuildingManager","ProcessBuidTeList",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_WORLD_AREA_INFO_TERRAIN:
			{
				//TiledStorgeCacheData.instance.ProcessTiledCacheData(SubCmdBuff);
				U3dCmn.SendMessage("_TiledStorgeCacheData","ProcessTiledCacheData",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_WORLD_AREA_INFO_CITY:
			{
				//TiledStorgeCacheData.instance.ProcessTiledCacheCity(SubCmdBuff);
				U3dCmn.SendMessage("_TiledStorgeCacheData","ProcessTiledCacheCity",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_MOVE_CITY:
			{
				U3dCmn.SendMessage("_TiledStorgeCacheData","ProcessCityMovableData",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_WOLRD_RES: //世界资源列表
			{
				U3dCmn.SendMessage("_TiledStorgeCacheData","ProcessWorldResCityList",SubCmdBuff);
				//TiledStorgeCacheData.instance.ProcessWorldResCityList(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_WORLD_RES_CARD:  //世界资源详细信息 
			{
				TiledStorgeCacheData.instance.ProcessWorldResCityInfo(SubCmdBuff);
				//U3dCmn.SendMessage("_TiledStorgeCacheData","ProcessTiledCacheCity",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_KICK_CLIENT_ALL:  //强制玩家下线通知  
			{
				PlayerInfoManager.ReqKickClientDataRst(SubCmdBuff);
				//U3dCmn.SendMessage("_TiledStorgeCacheData","ProcessTiledCacheCity",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_WORLD_RES_CLEAR:  //世界资源被攻占信息 
			{
				U3dCmn.SendMessage("_TiledStorgeCacheData","ProcessWorldResAttacked",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_START_COMBAT:
			{
				CombatManager.instance.ProcessStartCombatData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_GET_COMBAT:
			{
				CombatManager.instance.ProcessCombatUnitData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_SUPPLY_DRUG:
			{
				CombatManager.instance.ProcessManualSupplyDrugData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_LIST_COMBAT:
			{
				CombatManager.instance.ProcessListCombatData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_STOP_COMBAT:
			{
				CombatManager.instance.ProcessStopCombatData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_ARMY_BACK:
			{
				CombatManager.instance.ProcessArmyBackData(SubCmdBuff);
			} 
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_ARMY_ACCELERATE:
			{
				CombatManager.instance.ProcessArmyAccelData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_AUTO_COMBAT:
			{
				CombatManager.instance.ProcessStopAutoCombatData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_AUTO_SUPPLY:
			{
				CombatManager.instance.ProcessAutoSupplyData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_MANUAL_SUPPLY:
			{
				CombatManager.instance.ProcessManualSupplyData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_PVP_RST:
			{
				CombatManager.instance.ProcessPVPRst(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_COMBAT_RST:
			{
				CombatManager.instance.ProcessCombatRstData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_COMBAT_PROF:
			{
				CombatManager.instance.ProcessCombatProfData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_INSTANCESTATUS:
			{
				BaizInstanceManager.instance.ProcessListInstanceStatusData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_CREATE_INSTANCE:
			{
				BaizInstanceManager.instance.ProcessCreateInstanceData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_DESTROY_INSTANCE:
			{
				BaizInstanceManager.instance.ProcessDestroyInstanceData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_INSTANCEDATA:
			{
				BaizInstanceManager.instance.ProcessGetBaizInstanceData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_INSTANCELOOT:
			{
				BaizInstanceManager.instance.ProcessBaizhanGuardLootData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_INSTANCELOOT_ADDITEM:
			{
				BaizInstanceManager.instance.ProcessInstanceLootAddItemData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_PREPARE_INSTANCE:
			{
				BaizInstanceManager.instance.ProcessPrepareInstanceData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_START_INSTANCE:
			{
				BaizInstanceManager.instance.ProcessStartInstanceData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_QUIT_INSTANCE:
			{
				BaizInstanceManager.instance.ProcessQuitInstanceData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_JOIN_INSTANCE:
			{
				BaizInstanceManager.instance.ProcessJoinInstanceData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_CONFIG_INSTANCEHERO:
			{
				BaizInstanceManager.instance.ProcessConfigInstanceHeroData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_KICK_INSTANCE:
			{
				BaizInstanceManager.instance.ProcessKickInstanceHeroData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_INSTANCEDESC:
			{
				BaizInstanceManager.instance.ProcessGetInstanceDescData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_SUPPLY_INSTANCE:
			{
				BaizInstanceManager.instance.ProcessSupplyInstanceData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_WORLDGOLDMINE:
			{
				GoldmineInstance.instance.ProcessGetWorldGoldmineData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GAIN_WORLDGOLDMINE:
			{
				GoldmineInstance.instance.ProcessGainWorldGoldmineData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_ROB_WORLDGOLDMINE:
			{
				GoldmineInstance.instance.ProcessRobWorldGoldmineData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_MY_WORLDGOLDMINE:
			{
				GoldmineInstance.instance.ProcessGetMyGoldmineData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_CONFIG_WORLDGOLDMINE_HERO:
			{
				GoldmineInstance.instance.ProcessConfigGoldmineHeroData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_DROP_WORLDGOLDMINE:
			{
				GoldmineInstance.instance.ProcessDropMyGoldmineData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_QUEST:
			{
				QuestFeeManager.instance.ProcessGetFeeQuestData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_DONE_QUEST:
			{
				QuestFeeManager.instance.ProcessDoneFeeQuestData(SubCmdBuff);
			} 
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_SOLDIER: //军队信息  
			{
				U3dCmn.SendMessage("SoldierManager","ProcessSoldierInfo",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_TECHNOLOGY: //科技信息  
			{
				U3dCmn.SendMessage("TechnologyManager","ProcessTechInfo",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CONSCRIPT_SOLDIER: //训练士兵返回   
			{
				U3dCmn.SendMessage("SoldierManager","ProcessTrainSoldierRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_UPGRADE_SOLDIER: //升级士兵返回   
			{
				U3dCmn.SendMessage("SoldierManager","ProcessUpdateSoldierRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_TAVERN_REFRESH: //刷新酒馆返回   
			{
				U3dCmn.SendMessage("JiuGuanInfoWin","ProcessRefreshJiuGuanRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_GET_HERO_REFRESH: //刷新酒馆将领返回   
			{
				U3dCmn.SendMessage("JiuGuanInfoWin","ProcessRefreshJiangLingRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_HIRE_HERO: //招募将领返回    
			{
				U3dCmn.SendMessage("JiuGuanInfoWin","ProcessJiangLingZhaoMuRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_FIRE_HERO: //解雇将领返回    
			{
				U3dCmn.SendMessage("JiangLingInfoWin","ProcessDismissJiangLingRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_GET_HERO_HIRE: //获取已雇佣将领返回    
			{
				U3dCmn.SendMessage("JiangLingManager","ProcessJiangLingInfo",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CONFIG_HERO: //将领配兵返回    
			{
				U3dCmn.SendMessage("JiangLingManager","ProcessWithSoldierRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_PRODUCE_GOLD: //生产黄金返回     
			{
				U3dCmn.SendMessage("ProduceManager","ProcessProduceRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_PRODUCTION_EVENT: //获取生产事件     
			{
				
				U3dCmn.SendMessage("ProduceManager","ProcessProduceEvent",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_PRODUCTION_TE_LIST: //获取生产时间时间       
			{
				U3dCmn.SendMessage("ProduceManager","ProcessProduceTimeEvent",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_POPULATION_ADD: //居民增长了        
			{
				U3dCmn.SendMessage("PlayerInfoManager","ProcessPopulationIncrease",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_FETCH_GOLD: //收获金子的结果     
			{
				U3dCmn.SendMessage("ProduceManager","ProcessGatherGold",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GOLD_CAN_FETCH: //金子可以收获（在线时 主动推送 ）      
			{
				U3dCmn.SendMessage("ProduceManager","ProcessGoldCanGather",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ACCE_GOLD_PRODUCE: //黄金生产加速返回       
			{
				U3dCmn.SendMessage("ProduceManager","ProcessAcceRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ACCE_BUILDING: //建筑加速返回       
			{
				U3dCmn.SendMessage("BuildingManager","ProcessAcceRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_BUY_ITEM: //购买商城道具返回      
			{
				U3dCmn.SendMessage("StoreManager","ProcessBuyRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CONVERT_DIAMOND_GOLD: //钻石购买金币返回      
			{
				U3dCmn.SendMessage("StoreManager","BuyGoldRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_ITEM: //获取道具返回      
			{
				U3dCmn.SendMessage("CangKuManager","ReqCangKuDataRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_SELL_ITEM: //仓库卖掉道具返回      
			{
				U3dCmn.SendMessage("CangKuWin","SellItemRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_EQUIP: //装备到武将的装备信息     
			{
				U3dCmn.SendMessage("EquipManager","ReqDressedEquipRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_GEM: //镶嵌到装备的宝石信息列表      
			{
				U3dCmn.SendMessage("EquipManager","ReqEquipedGemRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_REPLACE_EQUIP: //更换或者摘除装备返回信息    
			{
				U3dCmn.SendMessage("EquipManager","ReqChangeEquipRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_MOUNT_ITEM: //镶嵌宝石返回   
			{
				U3dCmn.SendMessage("EmbedGemWin","ReqEmbedGemRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_UNMOUNT_ITEM: //摘除宝石返回   
			{
				U3dCmn.SendMessage("EmbedGemWin","ReqRemovalGemRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_COMPOS_ITEM: //合成宝石返回   
			{
				U3dCmn.SendMessage("ComposeGemWin","ReqComposeRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_USE_ITEM: //使用道具返回   
			{
				U3dCmn.SendMessage("CangKuManager","UseArticlesItemRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_MAIL: //获取邮件返回    
			{
				U3dCmn.SendMessage("MessageManager","ReqPersonalMsgDataRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_NOTIFICATION:	//获取通知返回 
			{
				U3dCmn.SendMessage("MessageManager","ReqAnnounceDataRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_NEW_NOTIFICATION:	//新通知到来  
			{
				U3dCmn.SendMessage("MessageManager","NewAnnounceData",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_DELETE_MAIL: //删除邮件返回    
			{
				U3dCmn.SendMessage("MessageManager","ReqDeleteMailRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_DELETE_MAIL_ALL: //删除所有邮件返回    
			{
				U3dCmn.SendMessage("MessageManager","DeleteAllRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_READ_MAIL: //读取邮件返回    
			{
				U3dCmn.SendMessage("MessageManager","ReadMailRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_READ_MAIL_ALL: //读取所有邮件返回    
			{
				U3dCmn.SendMessage("MessageManager","ReadAllRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_ADD_MAIL: //发送私人邮件返回    
			{
				U3dCmn.SendMessage("MessageManager","SendMailRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ADD_ALLIANCE_MAIL: //发送联盟邮件返回    
			{
				U3dCmn.SendMessage("MessageManager","SendAllianceMailRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_NEW_PRIVATE_MAIL: //新邮件到来    
			{
				U3dCmn.SendMessage("MessageManager","NewMail",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ADD_GM_MAIL: //发送GM邮件返回     
			{
				U3dCmn.SendMessage("WriteMessageWin","DismissPanel",SubCmdBuff);
			}
			break;
			
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_FRIEND_LIST: //好友列表     
			{
				U3dCmn.SendMessage("FriendManager","ReqFriendDataRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_ENEMY_LIST: //仇人列表     
			{
				U3dCmn.SendMessage("FriendManager","ReqEnemyDataRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_NEW_ENEMY: //新的仇人推送      
			{
				U3dCmn.SendMessage("FriendManager","NewEnemy",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_OTHERS_FRIEND_APPLY: //别人申请加我好友信息列表      
			{
				U3dCmn.SendMessage("FriendManager","ReqOtherApplyDataRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_APPLY_FRIEND: //申请加别人好友返回       
			{
				U3dCmn.SendMessage("FriendManager","ReqAddFriendRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_NEW_FRIEND_APPLY: //别人的好友申请推送        
			{
				U3dCmn.SendMessage("FriendManager","NewOtherFriendApply",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_APPROVE_FRIEND: //同意成为好友返回         
			{
				U3dCmn.SendMessage("FriendManager","AgreeFriendApplyRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_DELETE_FRIEND: //删除好友返回         
			{
				U3dCmn.SendMessage("FriendManager","DeleteFriendRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_DELETE_ENEMY: //删除仇人返回         
			{
				U3dCmn.SendMessage("FriendManager","DeleteEnemyRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_TRAINING_TE: //获取武将修炼时间事件          
			{
				U3dCmn.SendMessage("TrainGroundManager","GetHeroTrainDataRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ADD_TRAINING: //请求修炼将领返回           
			{
				U3dCmn.SendMessage("TrainGroundManager","ReqTrainHeroRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_EXIT_TRAINING: //停止修炼选中将领返回         
			{
				U3dCmn.SendMessage("TrainGroundManager","ReqStopTrainHeroRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_TRAINING_OVER: //修炼结束          
			{
				U3dCmn.SendMessage("TrainGroundManager","TrainingOver",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_RESEARCH_TE: //获取科技研究队列            
			{
				U3dCmn.SendMessage("TechnologyManager","GetResearchingTechRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_RESEARCH: //研究科技返回            
			{
				U3dCmn.SendMessage("TechnologyManager","ReqResearchTechRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ACCE_RESEARCH: //加速科技研究返回            
			{
				U3dCmn.SendMessage("TechnologyManager","AccelerateTechRst",SubCmdBuff);
			}
			break;	
			case (int)GAME_SUB_RST.STC_GAMECMD_RESEARCH_TE: //科技研究成功            
			{
			//print("jjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjj");
				U3dCmn.SendMessage("TechnologyManager","ResearchEnd",SubCmdBuff);
			}
			break;	
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CREATE_ALLI: //创建联盟返回             
			{
				U3dCmn.SendMessage("AllianceManager","ReqCreateAllianceRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_JOIN_ALLI_NAME: //申请加入联盟返回               
			{
				U3dCmn.SendMessage("AllianceManager","ReqJoinAllianceRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_MY_ALLIANCE_JOIN_EVENT: //是否处于申请加入联盟状态                
			{
				U3dCmn.SendMessage("AllianceManager","ReqApplyingStateRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_ALLIANCE_INFO: //获取自身联盟信息                 
			{
				U3dCmn.SendMessage("AllianceManager","ReqAllianceInfoRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_ALLIANCE_CARD_BY_ID: //获取单个联盟信息                 
			{
				U3dCmn.SendMessage("AllianceInfoWin","AllianceDataRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_ALLIANCE_MEMBER: //获取联盟成员列表                 
			{
				U3dCmn.SendMessage("AllianceManager","ReqAllianceMemberRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_ALLIANCE_JOIN_EVENT: //获取联盟申请列表                 
			{
				U3dCmn.SendMessage("AllianceManager","ReqApplyingDataRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CANCEL_JOIN_ALLI: //撤销联盟申请                  
			{
				U3dCmn.SendMessage("AllianceManager","ReqRecallApplyRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_DISMISS_ALLI: //解散联盟                 
			{
				U3dCmn.SendMessage("AllianceManager","ReqDismissAllianceRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_EXIT_ALLI: //退出联盟                 
			{
				U3dCmn.SendMessage("AllianceManager","ReqQuitAllianceRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ACCEPT_ALLI_MEMBER: //同意玩家加入联盟                 
			{
				U3dCmn.SendMessage("AllianceManager","AgreeJoinApplyRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_IN_ALLIANCE: //被允许加入联盟                 
			{
				U3dCmn.SendMessage("AllianceManager","BeAgreedJoinApply",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_REFUSE_JOIN_ALLI: //拒绝玩家加入联盟                 
			{
				U3dCmn.SendMessage("AllianceManager","RefuseJoinApplyRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_NEW_ALLIANCE_JOIN: //有玩家申请加入联盟通知                 
			{
				U3dCmn.SendMessage("AllianceManager","ReqApplyingData",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_PLAYERCARD: //获取其他玩家基本信息                  
			{
				OtherPlayerInfoManager.ProcessOtherPlayerInfo(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_SET_ALLI_INTRO: //修改联盟简介返回                
			{
				U3dCmn.SendMessage("AllianceManager","ReqModifyAnnounceRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_EXPEL_ALLI_MEMBER: //开除联盟成员返回               
			{
				U3dCmn.SendMessage("AllianceManager","FireAllianceMemberRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_EXPELED_ALLI: 	//被开除联盟消息通知 
			{
				U3dCmn.SendMessage("AllianceManager","BeFiredAllianceRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_SET_ALLI_POSITION: //授权返回               
			{
				U3dCmn.SendMessage("AllianceManager","ReqAuthorizeMemberRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ABDICATE_ALLI: //禅让盟主返回               
			{
				U3dCmn.SendMessage("AllianceManager","ReqDemiseLeader",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_ALLIANCE_CARD_BY_RANK: //获取联盟列表返回                
			{
				U3dCmn.SendMessage("AllianceManager","ReqAllianceListRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_JOIN_ALLI: //通过ID加入联盟返回            
			{
				U3dCmn.SendMessage("AllianceManager","ReqJoinAllianceRstByIDRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_CITYDEFENSE: //获取城防将领数据返回            
			{
				U3dCmn.SendMessage("ShaoTaWin","ReqDefenseDataRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CONFIG_CITYDEFENSE: //保存城防守将信息返回            
			{
				U3dCmn.SendMessage("ShaoTaWin","SaveDefenseHeroInfoRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_USE_DRUG: //治疗将领返回             
			{
				U3dCmn.SendMessage("TreatManager","ReqTreatHeroRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_DRAW_LOTTERY:
			{
				STC_GAMECMD_OPERATE_DRAW_LOTTERY_T	sub_msg	= DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_DRAW_LOTTERY_T>(SubCmdBuff);
				switch (sub_msg.nType2)
				{
					case (uint)money_type.money_type_none:
					case (uint)money_type.money_type_crystal:
					case (uint)money_type.money_type_diamond:
					{
						U3dCmn.SendMessage("ZhuanPan","ReqLotteryDataRst",SubCmdBuff);
					}
					break;
					case (uint)money_type.money_type_alliance_contribute:
					{
						U3dCmn.SendMessage("ZhuanPanAlliance","ReqLotteryDataRst",SubCmdBuff);
					}
					break;
					default:
					{
						
					}
					break;
				}	
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CHECK_DRAW_LOTTERY:
			{
				STC_GAMECMD_OPERATE_CHECK_DRAW_LOTTERY_T	sub_msg	= DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_CHECK_DRAW_LOTTERY_T>(SubCmdBuff);
				switch (sub_msg.nType2)
				{
					case (uint)money_type.money_type_none:
					case (uint)money_type.money_type_crystal:
					case (uint)money_type.money_type_diamond:
					{
						U3dCmn.SendMessage("ZhuanPan","ReqCheckLotteryDataRst",SubCmdBuff);
					}
					break;
					case (uint)money_type.money_type_alliance_contribute:
					{
						U3dCmn.SendMessage("ZhuanPanAlliance","ReqCheckLotteryDataRst",SubCmdBuff);
					}
					break;
					default:
					{
						
					}
					break;
				}
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_FETCH_LOTTERY:
			{
				STC_GAMECMD_OPERATE_FETCH_LOTTERY_T	sub_msg	= DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_FETCH_LOTTERY_T>(SubCmdBuff);
				switch (sub_msg.nMoneyType5)
				{
					case (uint)money_type.money_type_none:
					case (uint)money_type.money_type_crystal:
					case (uint)money_type.money_type_diamond:
					{
						U3dCmn.SendMessage("ZhuanPan","ReqFetchLotteryRst",SubCmdBuff);
					}
					break;
					case (uint)money_type.money_type_alliance_contribute:
					{
						U3dCmn.SendMessage("ZhuanPanAlliance","ReqFetchLotteryRst",SubCmdBuff);
					}
					break;
					default:
					{
						
					}
					break;
				}
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_CHRISTMAS_TREE_INFO: 	//获取自己的摇钱树状态  
			{
				U3dCmn.SendMessage("BuildingManager","GetYaoQianShuInfoRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE: 	//给摇钱树浇水返回 
			{
				if(CommonData.water_yaoqianshu == (int)YAOQIANSHU_CATEGORY.mine)
					U3dCmn.SendMessage("YaoQianShu","WaterTreeRst",SubCmdBuff);
				else if(CommonData.water_yaoqianshu == (int)YAOQIANSHU_CATEGORY.others)
					U3dCmn.SendMessage("OthersJinKuangWin","WaterTreeRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_FETCH_CHRISTMAS_TREE: 	//收获许愿树  
			{
				U3dCmn.SendMessage("YaoQianShu","GetYaoQianResultRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_OTHER_GOLDORE_INFO: 	//获取别人的金矿信息返回(包含摇钱树)   
			{
				U3dCmn.SendMessage("OthersJinKuangWin","InitialJinKuang",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_STEAL_GOLD: 	//偷窃金矿返回 
			{
				U3dCmn.SendMessage("OthersJinKuangWin","ReqStealGoldRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_GOLDORE_SMP_INFO_ALL: 	//获取好友金矿是否可偷窃可浇水状态返回 
			{
				U3dCmn.SendMessage("JinKuangFriendListWin","ReqFriendGoldSmpInfoRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_ALLIANCE_TRADE_INFO: 	//获取联盟跑商状态  
			{
				U3dCmn.SendMessage("AllianceWin","InitialWealUI",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ALLIANCE_TRADE: 	//开始跑商返回  
			{
				U3dCmn.SendMessage("AllianceWin","StartBusinessRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CONTRIBUTE_ALLIANCE: 	//联盟捐献返回  
			{
				U3dCmn.SendMessage("AllianceContributeWin","ReqContributeGoldRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_MY_RANK: 	//获取我的等级排名  
			{
				U3dCmn.SendMessage("RankManager","GetMyRankInfoRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_RANK_LIST: 	//获取各种排名列表 
			{
				U3dCmn.SendMessage("RankManager","GetRankListInfoRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_WRITE_SIGNATURE: 	//修改签名返回  
			{
				U3dCmn.SendMessage("PlayerInfoWin","ModifySignatureRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_BUILD_ITEM_USE_OUT: 	//工匠之书到期   
			{
				U3dCmn.SendMessage("BuildingManager","AddBuildingListOut",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_PAY_SERIAL: 	//工匠之书到期   
			{
				U3dCmn.SendMessage("91Manager","cbGetPaySerial",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_RECHARGE_DIAMOND: 	//工匠之书到期   
			{
				U3dCmn.SendMessage("91Manager","cbRechargeDiamond",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CHANGE_NAME: 	//君主修改名字返回  
			{
				U3dCmn.SendMessage("ModifyNameWin","ModifyLordNameRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CHANGE_HERO_NAME: 	//将领修改名字返回  
			{
				U3dCmn.SendMessage("ModifyNameWin","ModifyHeroNameRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_WORLDCITY_GET: 	//名城信息   
			{
				TiledStorgeCacheData.ProcessHonorCityMap(SubCmdBuff);
				//U3dCmn.SendMessage("HonorInfoWin","InitialData",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_WORLDCITY_RANK_MAN: 	//个人圣杯排行榜    
			{
				U3dCmn.SendMessage("CupRankWin","GetPersonalRankRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_WORLDCITY_RANK_ALLIANCE: 	//联盟圣杯排行榜    
			{
				U3dCmn.SendMessage("CupRankWin","GetAllianceRankRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_WORLDCITY_GETLOG: 	//荣耀之战个人战绩排行    
			{
				U3dCmn.SendMessage("HonorHistoryWin","GetPersonalRankRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_WORLDCITY_GETLOG_ALLIANCE: 	//荣耀之战联盟战绩排行    
			{
				U3dCmn.SendMessage("HonorHistoryWin","GetAllianceRankRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_FETCH_MAIL_REWARD: 	//领取带附件的邮件返回   
			{
				U3dCmn.SendMessage("MessageManager","ReceiveRewardMailRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_REGISTER_ACCOUNT: 	//注册邮箱账号或者修改密码返回   
			{
				U3dCmn.SendMessage("AccountManageWin","MailAccountRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_BIND_DEVICE: 	//绑定设备返回     
			{
				U3dCmn.SendMessage("AccountManageWin","BandUdidRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_SERVER_TIME: 	//绑定设备返回     
			{
				U3dCmn.SendMessage("BuildingManager","BeginWorldTimeCutDown",SubCmdBuff);
			}
			break; 
			case (int)GAME_SUB_RST.STC_GAMECMD_ADCOLONY_AWARD: 	//Adcolony广告奖励      
			{
				U3dCmn.SendMessage("PlayerInfoManager","AdcolonyReward",SubCmdBuff);
			}
			break; 
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_DISMISS_SOLDIER: 	//解散士兵       
			{
				U3dCmn.SendMessage("SoldierManager","DismissSoldierRst",SubCmdBuff);
			}
			break; 
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_LEFT_DONATE_SOLDIER: //获取增援盟友剩余数量         
			{
				U3dCmn.SendMessage("ReinforceSoldierWin","InitialData",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ALLI_DONATE_SOLDIER: //增援盟友返回        
			{
				U3dCmn.SendMessage("ReinforceSoldierWin","ReqReinforceRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_DONATE_SOLDIER_TE: //增援盟友到达通知         
			{
				U3dCmn.SendMessage("SoldierManager","ReinforceRst",SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_DONATE_SOLDIER_QUEUE: //请求联盟增援信息列表        
			{
				CombatManager.instance.ProcessReinforceQueue(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ALLI_ACCE_SOLDIER: //请求联盟增援加速返回       
			{
				CombatManager.instance.ProcessReinforceAccelRst(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ALLI_RECALL_SOLDIER: //请求联盟补兵召回返回       
			{
				CombatManager.instance.ProcessReinforceBackData(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_GOLD_DEAL: //请求金币交易列表返回       
			{
				TradeMarketManager.instance.ReqTradeListRst(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_GET_SELF_GOLD_DEAL: //请求自己的金币交易列表返回       
			{
				TradeMarketManager.instance.ReqSelfTradeListRst(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ADD_GOLD_DEAL: //请求寄卖金币返回     
			{
				TradeMarketManager.instance.ReqSellGoldRst(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CANCEL_GOLD_DEAL: //撤销寄卖金币返回     
			{
				TradeMarketManager.instance.ReqRevokeGoldSaleRst(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_BUY_GOLD_IN_MARKET: //购买金币返回    
			{
				TradeMarketManager.instance.ReqBuyGoldSaleRst(SubCmdBuff);
			}
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_GET_STATUS://获取我的联盟副本状态  
			{
				 GBStatusManager.Instance.RequestMyGBStatusRst(SubCmdBuff);
			} 
            break;
            case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_GET_LIST://联盟副本获取副本列表 
			{
				 GBTeamManager.Instance.RequestTeamListRst(SubCmdBuff);
			} 
            break;
            case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_CREATE://联盟副本创建队伍 
			{
                GBTeamManager.Instance.CreateTeamRst(SubCmdBuff);
			}
            break;
            case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_DESTROY://联盟副本解散队伍 
			{
                GBTeamManager.Instance.DisbandTeamRst(SubCmdBuff);
			}
            break;
            case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_JOIN://联盟副本加入队伍 
			{
                GBTeamManager.Instance.JoinTeamRst(SubCmdBuff);
			}
			break;
            case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_EXIT://联盟副本退出队伍 
			{
                GBTeamManager.Instance.QuitTeamRst(SubCmdBuff);
			}
			break;
            case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_GET_CHAR_DATA://联盟副本获取队伍中君主信息  
			{	
				GBTeamManager.Instance.RequestTeamMemberRst(SubCmdBuff);
			}
            break;
            case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_GET_HERO_DATA://联盟副本获取某君主已配置的将领信息 
			{
				GBTeamManager.Instance.RequestHeroListRst(SubCmdBuff);
			}
            break;
            case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_KICK://联盟副本t人   
            {
                GBTeamManager.Instance.KickMemberRst(SubCmdBuff);
            }
            break;
            case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_CONFIG_HERO://配置将领
            {
                GBTeamManager.Instance.DeployHeroRst(SubCmdBuff);
            }
            break;
            case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_READY: //准备完毕  
            { 
                GBTeamManager.Instance.GetReadyRst(SubCmdBuff);
            }
            break;
            case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_START://出征 
            {
                GBTeamManager.Instance.MarchTeamRst(SubCmdBuff);
            }
            break;
            case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_START_COMBAT://开始战斗 
            {
                //GBTeamManager.Instance.StartCombatRst(SubCmdBuff);
                BattleManager.Instance.StartCombatRst(SubCmdBuff);
            }
            break;
            case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_GET_LOOT://查看奖励 
            {
                GBTeamManager.Instance.GetRewardRst(SubCmdBuff);
            }
            break;
            case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_ACTION_NOTIFY: //服务器推送战斗状态变化：加入、退出、解散等 
            {
                GBTeamManager.Instance.GBActionNotify(SubCmdBuff);
            }
            break;
            case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_BACK: 
            { 
                GBTeamManager.Instance.EndCombat(SubCmdBuff);
            }
            break;
            case (int)GAME_SUB_RST.STC_GAMECMD_HERO_SIMPLE_DATA: //将领状态变化  
            { 
                JiangLingManager.UpdateHeroSimpleInfo(SubCmdBuff);
            }
            break;
            case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_SIMPLE_COMBAT_LOG://简单战报返回 
            {
                //GBTeamManager.Instance.RequestSimpleCombatLogRst(SubCmdBuff);
                BattleManager.Instance.RequestSimpleCombatLogRst(SubCmdBuff);
            }
            break;
            case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_COMBAT_LOG://复杂战报返回 
            {
                //GBTeamManager.Instance.RequestComplexCombatLogRst(SubCmdBuff);
                BattleManager.Instance.RequestComplexCombatLogRst(SubCmdBuff);
				print ("llllllllll"+SubCmdBuff.Length);
            }
            break;
            case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_STOP_COMBAT: //停止自动战斗 
            {
                GBTeamManager.Instance.StopAutoCombatRst(SubCmdBuff);
            }
            break;
            case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_SUPPLY://手动补兵 
            {
                //GBTeamManager.Instance.ManualSupplyForceRst(SubCmdBuff);
                BattleManager.Instance.ManualSupplyRst(SubCmdBuff);
            }
            break;
            case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_SAVE_HERO_DEPLOY: //保存所有将领布局，出征时调用
            {
                GBTeamManager.Instance.SaveHeroDeployRst(SubCmdBuff);
            }
            break;
			case (int)GAME_SUB_RST.STC_GAMECMD_ARENA_GET_STATUS: //竞技场基本信息  
            {
                ArenaManager.Instance.ReqArenaDataRst(SubCmdBuff);
            }
            break;
			case (int)GAME_SUB_RST.STC_GAMECMD_ARENA_GET_RANK_LIST: //竞技场排名数据 
            {
                ArenaManager.Instance.ReqArenaRankDataRst(SubCmdBuff);
            }
            break;
			case (int)GAME_SUB_RST.STC_GAMECMD_ARENA_GET_DEPLOY: //竞技场自己的镜像数据 
            {
                ArenaManager.Instance.ReqDeployDataRst(SubCmdBuff);
            }
            break;
			case (int)GAME_SUB_RST.STC_GAMECMD_ARENA_UPLOAD_DATA: //竞技场上传镜像数据返回  
            {
                ArenaManager.Instance.UploadDeployDataRst(SubCmdBuff);
            }
            break;
			case (int)GAME_SUB_RST.STC_GAMECMD_HERO_SUPPLY: //普通的手动补给（传递HEROID） 
            {
                ArmyDeployManager.instance.ReqManualSupplyRst(SubCmdBuff);
            }
            break;
			case (int)GAME_SUB_RST.STC_GAMECMD_ARENA_PAY: //购买练兵次数返回 
            {
                U3dCmn.SendMessage("ArenaWin","ReqBuyTryRst",SubCmdBuff);
            }
            break;
			case (int)GAME_SUB_RST.STC_GAMECMD_OPERATE_SET_VIP_DISPLAY: //是否开启vip显示  
            {
                U3dCmn.SendMessage("SettingWin","OnShowVipChangedRst",SubCmdBuff);
            }
            break;
			case (int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_GET_DEFENDER_DEPLOY: //获取副本敌方阵容信息   
            {
                ArmyDeployManager.instance.ReqEnemyDepoyDataRst(SubCmdBuff);
            }
            break;
			case (int)GAME_SUB_RST.STC_GAMECMD_POSITION_MARK_ADD_RECORD: //添加到收藏夹返回    
            {
				FavoriteManager.Instance.AddFavoriteDataRst(SubCmdBuff);
            }
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_POSITION_MARK_DEL_RECORD: //删除收藏夹数据返回     
            {
				FavoriteManager.Instance.DeleteFavoriteDataRst(SubCmdBuff);
            }
			break;
			case (int)GAME_SUB_RST.STC_GAMECMD_POSITION_MARK_GET_RECORD: //获取收藏夹数据返回    
            {
				FavoriteManager.Instance.GetFavoriteDataRst(SubCmdBuff);
            }
            break;
			
		}
	}
}
