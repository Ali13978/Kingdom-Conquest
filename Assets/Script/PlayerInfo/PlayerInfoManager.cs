using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class PlayerInfoManager : MonoBehaviour {

	


	public UILabel DiamondLabel;
	public UILabel CrystalLabel;
	public UILabel GoldLabel;
	public UILabel PopulationLabel;
	public UISprite char_icon;
	public UISlider exp_slider;
	public UILabel  exp_label;
	public UILabel 	level_label;
	public UISprite protect_icon;
	public static bool showad = false;
	//更新玩家基本信息的回调注册表 里面包含需要回调的object和回调函数名 
	public static Hashtable CallBackObjMap = new Hashtable();

  
    // Use this for initialization
    void Start () {
		PlayerInfoManager.RegisterCallBack(gameObject,"UpdateTopBarPlayerInfo");
		PlayerInfoManager.GetPlayerInfo();
		PlayerInfoManager.ReqKickClientData();
		
	}
	//注册玩家基本信息UI回调 
	public static void RegisterCallBack(GameObject obj,string func_name)
	{
		if(!CallBackObjMap.Contains(obj))
			CallBackObjMap.Add(obj,func_name);
	} 
	//请求玩家基本信息 
	public static void GetPlayerInfo()
	{
		CTS_GAMECMD_GET_CHAR_ATB_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_CHAR_ATB;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_CHAR_ATB_T >(req);
		showad = true;
	}
	//请求玩家重要基本信息 
	public static void GetPlayerSmpInfo()
	{
		CTS_GAMECMD_GET_IMPORTANT_CHAR_ATB_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_IMPORTANT_CHAR_ATB;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_IMPORTANT_CHAR_ATB_T >(req);
	}
	//处理玩家基本信息
	void ProcessPlayerInfo(byte[] buff)
	{
		STC_GAMECMD_GET_CHAR_ATB_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_CHAR_ATB_T>(buff);	

		CommonData.player_online_info.AccountID = sub_msg.nAccountID1;
		CommonData.player_online_info.CharName = DataConvert.BytesToStr(sub_msg.szName3);
		CommonData.player_online_info.Exp = sub_msg.nExp6;
		CommonData.player_online_info.Level = sub_msg.nLevel7;
		CommonData.player_online_info.Diamond = sub_msg.nDiamond8;
		CommonData.player_online_info.Crystal = sub_msg.nCrystal9;
		CommonData.player_online_info.Gold = sub_msg.nGold16;
		CommonData.player_online_info.Vip = sub_msg.nVip10;
		CommonData.player_online_info.Sex = sub_msg.nSex11;
		CommonData.player_online_info.nHeadID = sub_msg.nHeadID12;
		CommonData.player_online_info.PosX = sub_msg.nPosX13;
		CommonData.player_online_info.PosY = sub_msg.nPosY14;
		CommonData.player_online_info.Population = sub_msg.nPopulation17;
		CommonData.player_online_info.BuildNum = sub_msg.nBuildNum18;
		CommonData.player_online_info.AddedBuildNum = sub_msg.nAddedBuildNum19;
		CommonData.player_online_info.Development = sub_msg.nDevelopment15;
		CommonData.player_online_info.AllianceID = sub_msg.nAllianceID20;
		CommonData.player_online_info.AllianceName = DataConvert.BytesToStr(sub_msg.szAllianceName22);
		CommonData.player_online_info.CaoYao	= sub_msg.nDrug23;
		CommonData.player_online_info.FreeDrawLotteryNum	= sub_msg.nFreeDrawLotteryNum24;
	 	CommonData.player_online_info.Signature = DataConvert.BytesToStr(sub_msg.szSignature27);
	 	CommonData.player_online_info.ProtectEndTime =	(int)sub_msg.nProtectTime29 + DataConvert.DateTimeToInt(DateTime.Now);
	 	CommonData.player_online_info.AddBuildNumTime = (int)sub_msg.nAddBuildNumTime31+DataConvert.DateTimeToInt(DateTime.Now);
		CommonData.player_online_info.TotalBuildNum = sub_msg.nTotalBuildNum33;
		CommonData.player_online_info.NotificationID = sub_msg.nNotificationID34;
		CommonData.player_online_info.AllianceLotteryNum = sub_msg.nAllianceDrawLotteryNum35;
		CommonData.player_online_info.TotalDiamond = sub_msg.nTotalDiamond36;
		CommonData.player_online_info.WangZhe = sub_msg.nInstanceWangzhe37;
		CommonData.player_online_info.ZhengZhan = sub_msg.nInstanceZhengzhan38;
		CommonData.player_online_info.HonorCup  = sub_msg.nCup39;
		CommonData.player_online_info.BindMail = sub_msg.bBinded40;
		CommonData.player_online_info.ShowVip = sub_msg.bVipDisplay41;
		if((int)CommonData.player_online_info.ProtectEndTime - DataConvert.DateTimeToInt(DateTime.Now)> 0)
			protect_icon.gameObject.SetActiveRecursively(true);
		else 
			protect_icon.gameObject.SetActiveRecursively(false);
		
		//U3dCmn.ShowWarnWindow(DataConvert.BytesToStr(sub_msg.szName3)+" "+sub_msg.nLevel7+" "+sub_msg.nPosX13+" "+sub_msg.nPosY14);
		//U3dCmn.ShowPlayerInfoWindow(info);
		if(showad)
		{
			showad = false;
			//是否可以弹出广告 
			Hashtable sMap = CommonMB.CmnDefineMBInfo_Map;
			uint ShowPopad = 17;
			if (true == sMap.ContainsKey(ShowPopad))
			{
				CmnDefineMBInfo cmndef = (CmnDefineMBInfo) sMap[ShowPopad];
				if (cmndef.num >0 && CommonData.player_online_info.Level>=cmndef.num)
				{
					bool pop = false;
					string gCol = string.Format(TaskManager.QUEST_POP,CommonData.player_online_info.AccountName);
					int BooDone = PlayerPrefs.GetInt(gCol, 0);
					if (BooDone != 1) 
					{
						pop = true;
						
					}
					else 
					{
						pop = QuestGuideIcon.CheckQuestOver();
					
					}
					if(pop)
					{
		
					}
				}
			}
			
		}
		BaizInstanceManager.instance._onLoadInstanceStatus();
		RefreshPlayerDataUI();
	}
	//玩家重要信息返回 
	void GetPlayerSmpInfoRst(byte[] buff)
	{
		
		STC_GAMECMD_GET_IMPORTANT_CHAR_ATB_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_IMPORTANT_CHAR_ATB_T>(buff);
		
		// 识别是否有升级信息 ...
		uint oldLevel = CommonData.player_online_info.Level;
		// 更新玩家信息 ....
		CommonData.player_online_info.Exp = sub_msg.nExp1;
		CommonData.player_online_info.Diamond = sub_msg.nDiamond2;
		CommonData.player_online_info.Crystal = sub_msg.nCrystal3;
		CommonData.player_online_info.Development = sub_msg.nDevelopment4;
		CommonData.player_online_info.Gold = sub_msg.nGold5;
		CommonData.player_online_info.Population = sub_msg.nPopulation6;
		CommonData.player_online_info.Level = sub_msg.nLevel7;
		CommonData.player_online_info.ProtectEndTime =	(int)sub_msg.nProtectTime8 + DataConvert.DateTimeToInt(DateTime.Now);
		CommonData.player_online_info.AddBuildNumTime = (int)sub_msg.nAddBuildNumTime9+DataConvert.DateTimeToInt(DateTime.Now);
		CommonData.player_online_info.TotalBuildNum = sub_msg.nTotalBuildNum10;
		CommonData.player_online_info.HonorCup = sub_msg.nCup11;
		if((int)CommonData.player_online_info.ProtectEndTime - DataConvert.DateTimeToInt(DateTime.Now)> 0)
			protect_icon.gameObject.SetActiveRecursively(true);
		else 
			protect_icon.gameObject.SetActiveRecursively(false);
		RefreshPlayerDataUI();
		
		// 获取任务列表， 是否有新手礼包可以领取了 ...
		bool BeWarningTip = (sub_msg.nLevel7>oldLevel);
		if (true == BeWarningTip)
		{
			RefreshQuestUnReadData();
		}
	}
	//刷新玩家信息界面 
	public static void RefreshPlayerDataUI()
	{
		//执行各UI OBJ的回调函数刷新界面 
		foreach(DictionaryEntry	de	in	CallBackObjMap)  
  		{
			GameObject obj = (GameObject)de.Key;
			string func_name = (string)de.Value;
			if(obj!=null && func_name!="")
			{
				obj.SendMessage(func_name);
			}
		}
	}
	
	// 刷新玩家任务 ...
	static void RefreshQuestUnReadData()
	{
		// 当前有没有谁提交任务处理 ....
		if (QuestFeeManager.processListQuestDelegate == null) {
			// 没有主动请求后的处理回调, 提醒有多少任务奖励未接 ...
			QuestFeeManager.processListQuestDelegate = null;
			QuestFeeManager.RequesGetFeeQuest();
		}
	}
	
	//显示玩家基本信息 
	void ShowPlayerInfo()
	{
		U3dCmn.ShowPlayerInfoWindow();
		
		// 如果有好友列表显示则隐藏 ....
		U3dCmn.SendMessage("JinKuangFriendListWin", "DisFriendList", null);
	}

	




    void UpdateTopBarPlayerInfo()
	{
		DiamondLabel.text = CommonData.player_online_info.Diamond.ToString();
		CrystalLabel.text = CommonData.player_online_info.Crystal.ToString();
		GoldLabel.text = U3dCmn.GetNumStr(CommonData.player_online_info.Gold);
		PopulationLabel.text = CommonData.player_online_info.Population.ToString();
		char_icon.spriteName = U3dCmn.GetCharSmpIconName((int)CommonData.player_online_info.nHeadID);
		if(CommonMB.CharLevelExp_Map.Contains(CommonData.player_online_info.Level))
		{
			uint next_level_exp = CommonData.player_online_info.Exp;
			if(CommonMB.CharLevelExp_Map.Contains(CommonData.player_online_info.Level+1))
			{
				CharLevelExp char_level_exp = (CharLevelExp)CommonMB.CharLevelExp_Map[CommonData.player_online_info.Level+1];
				next_level_exp = (uint)char_level_exp.Exp;
			}
			
			float slider_num = (float)CommonData.player_online_info.Exp/(float)next_level_exp;
			if(next_level_exp == 0)
			{
				exp_slider.sliderValue = 1;
				exp_label.text = "";
			}
			else 
			{
				exp_slider.sliderValue = slider_num;
				exp_label.text = CommonData.player_online_info.Exp+"/"+next_level_exp;
			}
			
			level_label.text = "Lv"+CommonData.player_online_info.Level;
		}
		else 
		{
			exp_label.text = "";
			exp_slider.sliderValue = 1.0f;
		}
	}
	//居民增长了  
	void ProcessPopulationIncrease(byte[] buff)
	{
		STC_GAMECMD_POPULATION_ADD_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_POPULATION_ADD_T>(buff);
		CommonData.player_online_info.Population += (uint)sub_msg.nProduction1;
		RefreshPlayerDataUI();
	}
	//adcolony广告回报 
	void AdcolonyReward(byte[] buff)
	{
		
		STC_GAMECMD_ADCOLONY_AWARD_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_ADCOLONY_AWARD_T>(buff);
		if(sub_msg.nMoneyType1 == (uint)money_type.money_type_diamond)
			CommonData.player_online_info.Diamond += sub_msg.nMoneyNum2;
		else if(sub_msg.nMoneyType1 == (uint)money_type.money_type_crystal)
			CommonData.player_online_info.Crystal += sub_msg.nMoneyNum2;
		RefreshPlayerDataUI();
	}
	//请求一下需不需要强制用户下线 
	public static void ReqKickClientData()
	{	
		CTS_GAMECMD_GET_KICK_CLIENT_ALL_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_KICK_CLIENT_ALL;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_KICK_CLIENT_ALL_T >(req);
	}
	//请求要强制用户下线数据返回 
	public static void ReqKickClientDataRst(byte[] buff)
	{
		STC_GAMECMD_GET_KICK_CLIENT_ALL_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_KICK_CLIENT_ALL_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_KICK_CLIENT_ALL_T.enum_rst.RST_OK)
		{
			string str = DataConvert.BytesToStr(sub_msg.strbuff3);
			string[] strarr = str.Split(',');
			for(int i= 0,max=strarr.Length;i<max;i++)
			{
				string[] subarr =  strarr[i].Split('*');
				if(subarr[0] == CommonMB.AppID.ToString() &&  CommonData.select_server.ClientVersion == subarr[1])
				{
					//提醒玩家从新登陆  
					U3dCmn.GetObjFromPrefab("NewVersionWin").SendMessage("RevealPanel");
				}
			}
		}
	}
}
