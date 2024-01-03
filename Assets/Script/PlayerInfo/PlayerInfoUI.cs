using UnityEngine;
using System;
using System.Collections;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class PlayerInfoUI : MonoBehaviour {
	public TweenPosition 	tween_position;
	public UISprite			char_icon;
	public UILabel 			char_name;
	public UILabel 			exp;
	public UILabel			level;
	//public UILabel			sex;
	public UILabel			status;
	//public UILabel			VIP;
	public UILabel 			AllianceName;
	public UILabel          LevelRank;
	public UILabel          Glory;
	public UILabel 			Position;
	public UIInput			signature;
	public UISprite			vip_icon;
	public UILabel			Gold;
	public UILabel			Diamond;
	public UILabel			Crystal;
	public UILabel 			People;
	public UILabel			WangZhe;
	public UILabel			ZhengZhan;
	
	string normal_str;
	string protect_str;
	
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
		string[] str_array = status.text.Split(':');
		normal_str = str_array[0];
		protect_str = str_array[1];
	}
	
	// Use this for initialization
	void Start () {
		PlayerInfoManager.RegisterCallBack(gameObject,"UpdatePlayerInfo");
		
	}
	
	//弹出窗口 
	void RevealPanel()
	{
		CMN_PLAYER_CARD_INFO info = CommonData.player_online_info;
		MainCity.touch_enabel = false;
		UpdatePlayerInfo();
	
		//posx.text = info.PosX.ToString();
		//posy.text = info.PosY.ToString();
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//更新玩家基本信息 
	void UpdatePlayerInfo()
	{
		uint next_level_exp =  CommonData.player_online_info.Exp;
		
		if(CommonMB.CharLevelExp_Map.Contains(CommonData.player_online_info.Level+1))
		{
			CharLevelExp char_level_exp = (CharLevelExp)CommonMB.CharLevelExp_Map[CommonData.player_online_info.Level+1];
			next_level_exp = (uint)char_level_exp.Exp;
		}
			
		
		char_icon.spriteName = U3dCmn.GetCharIconName((int)CommonData.player_online_info.nHeadID);
		char_name.text = CommonData.player_online_info.CharName;
		if(next_level_exp == 0)
			exp.text = "";
		else
			exp.text = CommonData.player_online_info.Exp+"/"+next_level_exp;
		level.text = CommonData.player_online_info.Level.ToString();
		//sex.text = CommonData.player_online_info.Sex.ToString();
		//VIP.text = CommonData.player_online_info.Vip.ToString();
		AllianceName.text = CommonData.player_online_info.AllianceName;
		LevelRank.text = CommonData.player_online_info.LevelRank.ToString();
		signature.text = CommonData.player_online_info.Signature;
		Glory.text = CommonData.player_online_info.Development.ToString();
		Position.text = CommonData.player_online_info.PosX+","+CommonData.player_online_info.PosY;
		Gold.text = CommonData.player_online_info.Gold.ToString();
		Diamond.text = CommonData.player_online_info.Diamond.ToString();
		Crystal.text  = CommonData.player_online_info.Crystal.ToString();
		People.text = CommonData.player_online_info.Population.ToString();
		if(CommonData.player_online_info.Vip >0)
		{
			//vip_icon.gameObject.SetActiveRecursively(true);	
			vip_icon.spriteName = "vip_"+CommonData.player_online_info.Vip;
		}
		else 
		{
			vip_icon.spriteName = "transparent";
		}
		if(CommonData.player_online_info.ShowVip != 1)
		{
			vip_icon.spriteName = "transparent";
		}
		//如果拥有奖杯 则免战消失 
		if(CommonData.player_online_info.HonorCup >0)
		{
			status.text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HONOR_BATTLE_STATUS),CommonData.player_online_info.HonorCup); 	
		}
		else 
		{
			if((int)CommonData.player_online_info.ProtectEndTime - DataConvert.DateTimeToInt(DateTime.Now)> 0)
			{
				status.text = protect_str;	
				int EndTimeSec = CommonData.player_online_info.ProtectEndTime;
				StopCoroutine("ProtectTimeCountdown");
				StartCoroutine("ProtectTimeCountdown",EndTimeSec);
				//TIME_REMAINING
			}
			else 
			{
				status.text = normal_str;
				//protect_time.text = "";
			}
		}
		
		if(CommonData.player_online_info.WangZhe >0)
		{
			if(CommonData.player_online_info.WangZhe <= 100)
			{
				WangZhe.text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.WANGZHE_NORMAL),CommonData.player_online_info.WangZhe);
			}
			else if(CommonData.player_online_info.WangZhe > 100 && CommonData.player_online_info.WangZhe<= 200)
			{
				uint wangnzhe_level =  CommonData.player_online_info.WangZhe - 100; 
				WangZhe.text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.WANGZHE_HIGHER),wangnzhe_level);
			}
		}	
		else 
			WangZhe.text = "";
		if(CommonMB.NanBeizhanInfo_Map.Contains(CommonData.player_online_info.ZhengZhan))
		{
			NanBeizhanMBInfo info = (NanBeizhanMBInfo)CommonMB.NanBeizhanInfo_Map[CommonData.player_online_info.ZhengZhan];
			ZhengZhan.text = info.name;
		}	
		else
			ZhengZhan.text = "";
		
		//posx.text = CommonData.player_online_info.PosX.ToString();
		//posy.text = CommonData.player_online_info.PosY.ToString();
	}
	//打开排行榜窗口 
	void OpenRankWin()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("RankWin");
		if (obj != null)
		{
			obj.SendMessage("RevealPanel",null);
		}
	}
	//修改个人签名 
	void ModifySignature()
	{
		
		byte[] buff = DataConvert.StrToBytes(signature.text);
		if(buff.Length >128)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.SIGNATURE_TOO_LONG));
			return ;
		}
		byte[] data_buff = new byte[129];
		Array.Copy(buff,0,data_buff,0,buff.Length);
		data_buff[128] = 0;	
		CTS_GAMECMD_OPERATE_WRITE_SIGNATURE_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_WRITE_SIGNATURE;
		req.szSignature4 = data_buff;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_WRITE_SIGNATURE_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_WRITE_SIGNATURE);
	}
	//修改个人签名返回 
	void ModifySignatureRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_WRITE_SIGNATURE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_WRITE_SIGNATURE_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_WRITE_SIGNATURE_T.enum_rst.RST_OK)
		{
			CommonData.player_online_info.Signature = signature.text;
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.MODIFY_SUCCESS));
			
		}
	}
	//修改君主名字 
	void ModifyCharName()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("ModifyNameWin");
		if (obj != null)
		{
			obj.SendMessage("RevealPanel",(int)MODIFY_NAME_TYPE.LORD_NAME);
		}
	}
	//保护时间倒计时 
	IEnumerator ProtectTimeCountdown(int EndTimeSec)
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
				status.text ="[00FF00]"+protect_str+"[-] "+string.Format("{0}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			else
				status.text ="[00FF00]"+protect_str+"[-] "+string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			yield return new WaitForSeconds(1);
		}
		CommonData.player_online_info.ProtectEndTime = DataConvert.DateTimeToInt(DateTime.Now);
		status.text = normal_str;	
		//protect_time.text = "";
	}
	//打开VIP说明 
	void OpenVipExplainWin()
	{
		GameObject infowin = U3dCmn.GetObjFromPrefab("BulletinWin");
		if(infowin != null)
		{
			InstanceMBInfo info = (InstanceMBInfo)CommonMB.InstanceMBInfo_Map[(uint)HELP_TYPE.VIP_HELP];
			infowin.GetComponent<BulletinWin>().title_label.text = info.mode;
			infowin.GetComponent<BulletinWin>().text_label.text = info.Rule1;
			infowin.SendMessage("RevealPanel");
		}
	}
}
