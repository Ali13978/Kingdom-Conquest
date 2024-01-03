using UnityEngine;
using System.Collections;
using System;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class PlayerCardWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	public UILabel CityDevelop;
	public UILabel CityStatus;
	public UILabel CharRank;
	public UILabel CharLevel;
	public UILabel CharName;
	public UILabel AllianceName;
	public UILabel Signature;
	public UISprite HeadIcon;
	public UISprite VipIcon;
	public UILabel WangZhe;
	public UILabel ZhengZhan;
	
	public UIImageButton ProclaimWarbBtn;
	public UIImageButton FriendBtn;
	public UIImageButton MailBtn;
	public UIImageButton ReinforceBtn;
	public UIImageButton MineBtn;
	public UIImageButton FavoriteBtn;
	STC_GAMECMD_GET_PLAYERCARD_T now_player_card;
	public static bool Counterattack = false;
	public static bool Favorite = false;
	string status_normal;
	string status_protect;
	void Awake()
	{
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			transform.localScale = CommonData.iphonescale;
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			transform.localScale = CommonData.ipadscale;
		}
		string[] str_array = CityStatus.text.Split(':');
		status_normal = str_array[0];
		status_protect = str_array[1];
		CityStatus.text = "";
		ReinforceBtn.gameObject.SetActiveRecursively(false);
		ProclaimWarbBtn.gameObject.SetActiveRecursively(false);
		MineBtn.gameObject.SetActiveRecursively(false);
		FriendBtn.gameObject.SetActiveRecursively(false);
		MailBtn.gameObject.SetActiveRecursively(false);
		FavoriteBtn.gameObject.SetActiveRecursively(false);
		//FriendBtn.gameObject.transform.localPosition = new Vector3(-55f,-105.32f,0);
		//MailBtn.gameObject.transform.localPosition = new Vector3(55f,-105.32f,0);
		
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//弹出窗口   
	void RevealPanel(ulong player_id)
	{
		OtherPlayerInfoManager.GetOtherPlayerCard(player_id);
		//注册回调 
		OtherPlayerInfoManager.RegisterCallBack(gameObject,"InitialPanelData");
		//InitialPanelData(hero_unit);
		tween_position.Play(true);
	}
	//关闭窗口   
	void DismissPanel()
	{
		Counterattack = false;
		Favorite = false;
		Counterattack = false;
		tween_position.Play(false);
		Destroy(gameObject);
		
	}
	//传入玩家参数直接打开界面 
	public void RevealPanelByParam(STC_GAMECMD_GET_PLAYERCARD_T player_card )
	{		
		InitialPanelData(player_card);
		tween_position.Play(true);
		
	
	}
	//初始化界面UI 
	public void InitialPanelData(STC_GAMECMD_GET_PLAYERCARD_T player_card)
	{
		if(Favorite)
		{
			FavoriteBtn.gameObject.SetActiveRecursively(true);
		}
		MineBtn.gameObject.SetActiveRecursively(true);
		FriendBtn.gameObject.SetActiveRecursively(true);
		MailBtn.gameObject.SetActiveRecursively(true);
		if(Counterattack)
		{
			ProclaimWarbBtn.gameObject.SetActiveRecursively(true);
			ProclaimWarbBtn.gameObject.transform.localPosition = new Vector3(-150f,-105.32f,0);
			MineBtn.gameObject.transform.localPosition = new Vector3(150f,-105.32f,0);
			FriendBtn.gameObject.transform.localPosition = new Vector3(-50f,-105.32f,0);
			MailBtn.gameObject.transform.localPosition = new Vector3(50f,-105.32f,0);
		}
		else 
		{
			ProclaimWarbBtn.gameObject.SetActiveRecursively(false);
			if(U3dCmn.GetCmnDefNum((uint)CMN_DEF.GUILD_REINFORCE) == 1)
			{
				if(player_card.nAllianceID11 !=0 && 
					player_card.nAllianceID11 == CommonData.player_online_info.AllianceID &&
					player_card.nAccountID2 != CommonData.player_online_info.AccountID)
				{
					ReinforceBtn.gameObject.SetActiveRecursively(true);
					ReinforceBtn.gameObject.transform.localPosition = new Vector3(-150f,-105.32f,0);
					MineBtn.gameObject.transform.localPosition = new Vector3(150f,-105.32f,0);
					FriendBtn.gameObject.transform.localPosition = new Vector3(-50f,-105.32f,0);
					MailBtn.gameObject.transform.localPosition = new Vector3(50f,-105.32f,0);
				}
				else
				{
					ReinforceBtn.gameObject.SetActiveRecursively(false);
					MineBtn.gameObject.transform.localPosition = new Vector3(150f,-105.32f,0);
					FriendBtn.gameObject.transform.localPosition = new Vector3(-150,-105.32f,0);
					MailBtn.gameObject.transform.localPosition = new Vector3(0,-105.32f,0);
				}
			}
			else 
			{
				MineBtn.gameObject.transform.localPosition = new Vector3(150f,-105.32f,0);
				FriendBtn.gameObject.transform.localPosition = new Vector3(-150,-105.32f,0);
				MailBtn.gameObject.transform.localPosition = new Vector3(0,-105.32f,0);
			}
		}
		
		if(player_card.nRst1 != (int)STC_GAMECMD_GET_PLAYERCARD_T.enum_rst.RST_OK)
		{
			//注销回调 
			OtherPlayerInfoManager.UnRegisterCallBack(gameObject);
			DismissPanel();
			return;
		}
		if(player_card.nAccountID2 == CommonData.player_online_info.AccountID)
		{
			ProclaimWarbBtn.GetComponent<Collider>().enabled = false;
			ProclaimWarbBtn.target.spriteName = "button1_disable"; 
			FriendBtn.GetComponent<Collider>().enabled = false;
			FriendBtn.target.spriteName = "button1_disable"; 
			MailBtn.GetComponent<Collider>().enabled = false;
			MailBtn.target.spriteName = "button1_disable"; 
		}
		else 
		{
			FriendBtn.GetComponent<Collider>().enabled = true;
			FriendBtn.target.spriteName = "button1";
			MailBtn.GetComponent<Collider>().enabled = true;
			MailBtn.target.spriteName = "button1";
		}
		
		now_player_card = player_card;
		//注销回调 
		OtherPlayerInfoManager.UnRegisterCallBack(gameObject);
		CityDevelop.text = player_card.nDevelopment14.ToString();
		if(player_card.nCup23 >0)
		{
			CityStatus.text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HONOR_BATTLE_STATUS),player_card.nCup23);
		}
		else
		{
			if(player_card.nProtectTime17 >0)
			{
				//CityStatus.text = "";
				StopCoroutine("ProtectCountdown");
				StartCoroutine("ProtectCountdown",(int)(DataConvert.DateTimeToInt(DateTime.Now)+ player_card.nProtectTime17));
			}
			else
			{
				CityStatus.text = status_normal;
			}
		}
		
		if(player_card.nVip6 >0)
		{
			//vip_icon.gameObject.SetActiveRecursively(true);	
			VipIcon.spriteName = "vip_"+player_card.nVip6;
		}
		else 
		{
			VipIcon.spriteName = "transparent";
		}
		if(player_card.bVipDisplay24 != 1)
		{
			VipIcon.spriteName = "transparent";
		}
		
	  	CharRank.text = player_card.nLevelRank18.ToString();
		CharLevel.text = player_card.nLevel5.ToString();
	  	CharName.text = DataConvert.BytesToStr(player_card.szName4);
		AllianceName.text = DataConvert.BytesToStr(player_card.szAllianceName13);
		Signature.text = DataConvert.BytesToStr(player_card.szSignature16);
		HeadIcon.spriteName = U3dCmn.GetCharIconName((int)player_card.nHeadID8);
		if(player_card.nInstanceWangzhe21 >0)
		{
			if(player_card.nInstanceWangzhe21 <= 100)
			{
				WangZhe.text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.WANGZHE_NORMAL),player_card.nInstanceWangzhe21);
			}
			else if(player_card.nInstanceWangzhe21 > 100 && player_card.nInstanceWangzhe21<= 200)
			{
				uint wangnzhe_level =  player_card.nInstanceWangzhe21 - 100; 
				WangZhe.text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.WANGZHE_HIGHER),wangnzhe_level);
			}
		}
		else 
			WangZhe.text = "";
		
		
		
		if(CommonMB.NanBeizhanInfo_Map.Contains(player_card.nInstanceZhengzhan22))
		{
			NanBeizhanMBInfo info = (NanBeizhanMBInfo)CommonMB.NanBeizhanInfo_Map[player_card.nInstanceZhengzhan22];
			ZhengZhan.text = info.name;
		}	
		else
			ZhengZhan.text = "";
		//WangZhe
	}
	//打开别人的金矿 
	void OpenOtherJinKuang()
	{
		if(now_player_card.nAccountID2 != 0)
		{
			if(now_player_card.nAccountID2 == CommonData.player_online_info.AccountID)
			{
				U3dCmn.SendMessage("BuildingManager","RevealJinKuangPanel",null);
				U3dCmn.SendMessage("PlayerCardWin","DismissPanel",null);
				U3dCmn.SendMessage("RankWin","DismissPanel",null);
				U3dCmn.SendMessage("PlayerInfoWin","DismissPanel",null);
				U3dCmn.SendMessage("HonorInfoWin","DismissPanel",null);
				U3dCmn.SendMessage("HonorHistoryWin","DismissPanel",null);
				U3dCmn.SendMessage("CupRankWin","DismissPanel",null);
				U3dCmn.SendMessage("ChatWin","DismissPanel",null);
				U3dCmn.SendMessage("BaizAresRankWin","OnBaizhanRankClose",null);
				U3dCmn.SendMessage("BaizhanLaunchWin" ,"OnBaizhanLaunchClose",null);
			}
			else 
			{
				SIMPLE_CHAR_INFO char_info;
				char_info.AccountId = now_player_card.nAccountID2;
				char_info.CharName = DataConvert.BytesToStr(now_player_card.szName4);
				OthersJinKuangManager.OpenOthersJinKuangWin(char_info);
			}
				
		}
		
	}
	//写邮件 
	void WriteMail()
	{
		if(now_player_card.nAccountID2 != 0)
		{
			GameObject obj  = U3dCmn.GetObjFromPrefab("WriteMessageWin");
			if (obj != null)
			{
				WriteMessageWin win = obj.GetComponent<WriteMessageWin>(); 
				win.char_id = now_player_card.nAccountID2;
				win.recv_charname.text = DataConvert.BytesToStr(now_player_card.szName4);
				win.MailType = (byte)MAIL_TYPE.MAIL_TYPE_PRIVATE;
				obj.SendMessage("RevealPanel");
			}
		}
		
	}
	//加好友 
	void AddFriend()
	{
		if(now_player_card.nAccountID2 != 0)
		{
			FriendManager.ReqAddFriend(now_player_card.nAccountID2);
		}
	}
	//宣战 
	void ProclaimWar()
	{	
		if(now_player_card.nAccountID2 != 0)
		{
			int level_gap = U3dCmn.GetCmnDefNum((uint)CMN_DEF.PROTECT_LEVEL_GAP);
			if(level_gap != 0)
			{
				int gap = (int)CommonData.player_online_info.Level-(int)now_player_card.nLevel5;
				if(Math.Abs(gap) > level_gap)
				{
					U3dCmn.ShowWarnWindow(string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ATTACK_LEVEL_LIMIT),level_gap));
					return ;
				}
			}
			GameObject go = U3dCmn.GetObjFromPrefab("PopGeneralCrushWin");
			if (go == null) return;		
			
			PopGeneralCrush win1 = go.GetComponent<PopGeneralCrush>();
			if (win1 != null)
			{
				float depth = - BaizVariableScript.DEPTH_OFFSET * 3.0f;
				win1.Depth(depth);
				
				CMN_PLAYER_CARD_INFO info = CommonData.player_online_info;
				float d1 = (float) Mathf.Abs((int)info.PosX - (int)now_player_card.nPosX9);
				float d2 = (float) Mathf.Abs((int)info.PosY - (int)now_player_card.nPosY10);
				
				float sq1 = Mathf.Sqrt(d1*d1 + d2*d2);
				float sq2 = Mathf.Sqrt(400f*400f*2f);
				Hashtable cgtMap = CommonMB.CombatGoToTimeMBInfo_Map;
				uint combatTypeID = (uint)CombatTypeEnum.COMBAT_PVP_ATTACK;
				int secs =0;
				if (true == cgtMap.ContainsKey(combatTypeID))
				{
					CombatGoToTimeMBInfo card = (CombatGoToTimeMBInfo) cgtMap[combatTypeID];
					secs = Mathf.CeilToInt((sq1/sq2) * card.GoToTime);
		
				}
				//int secs = Mathf.CeilToInt((sq1/sq2) * 8f * 3600f); /// 最长时间为8小时 ...
				
				win1.ApplyCrush(now_player_card.nAccountID2, DataConvert.BytesToStr(now_player_card.szName4));
				win1.ApplyGoToTime(secs);
			}
			
			DismissPanel();
		}
	}
	//倒计时 
	IEnumerator ProtectCountdown(int EndTimeSec)
	{
		int sec = (int)(EndTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
		
		if(sec <0)
			sec = 0;
		while(sec!=0)
		{
			 sec =(int)(EndTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
			if(sec <0)
				sec = 0;
			int hour = sec/3600;
			int minute = sec/60%60;
			int second = sec%60;
			if(hour>=100)
				CityStatus.text ="[00FF00]"+status_protect + "[-] "+string.Format("{0}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			else
				CityStatus.text ="[00FF00]"+status_protect + "[-] "+string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			yield return new WaitForSeconds(1);
		}
	}
	//出城定位 
	void LocateCity()
	{
		U3dCmn.SendMessage("PlayerCardWin","DismissPanel",null);
		U3dCmn.SendMessage("RankWin","DismissPanel",null);
		U3dCmn.SendMessage("PlayerInfoWin","DismissPanel",null);
		U3dCmn.SendMessage("ChatWin","DismissPanel",null);
		U3dCmn.SendMessage("MessageWin","DismissPanel",null);
		U3dCmn.SendMessage("ReadMessageWin","DismissPanel",null);
		U3dCmn.SendMessage("CupRankWin","DismissPanel",null);
		U3dCmn.SendMessage("HonorInfoWin","DismissPanel",null);
		U3dCmn.SendMessage("HonorHistoryWin","DismissPanel",null);
		U3dCmn.SendMessage("BaizAresRankWin","OnBaizhanRankClose",null);
		U3dCmn.SendMessage("BaizhanLaunchWin","OnBaizhanLaunchClose",null);
		U3dCmn.SendMessage("WorldGoldmineScene","OnGoldmineSceneClose",null);
		
		SceneManager.OpenGlobalMapLocation((int)now_player_card.nPosX9,(int)now_player_card.nPosY10);
	}
	//增援 
	void Reinforce()
	{
		if(now_player_card.nAllianceID11 == CommonData.player_online_info.AllianceID)
		{
			GameObject obj  = U3dCmn.GetObjFromPrefab("ReinforceSoldierWin");
			if (obj != null)
			{
				obj.SendMessage("RevealPanel",now_player_card.nAccountID2);
			}
		}
		//print ("reinforce");
	}
	//收藏坐标 
	void CollectPos()
	{
		PositionMark data;
		data.nPosX1 = now_player_card.nPosX9;
		data.nPosY2 = now_player_card.nPosY10;
		data.nHeadID3 = now_player_card.nHeadID8;
		data.nLength4 = now_player_card.szName4.Length;
		data.szName5 = now_player_card.szName4;
		FavoriteManager.Instance.AddFavoriteData(data);
	}
}
