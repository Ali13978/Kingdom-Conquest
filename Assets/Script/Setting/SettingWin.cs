using UnityEngine;
using System.Collections;
using CMNCMD;
using CTSCMD;
using STCCMD;

public class SettingWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	
	public GameObject music_obj;
	public UILabel 	now_server;
	//public UILabel	account_name;
	public UISlider bg_music_slider;
	//public UISlider other_music_slider;
	public UICheckbox chk_AlertSound_On;
	public UICheckbox chk_Alert_On;
	public UICheckbox show_vip_On;
	public GameObject btn_91;
	public GameObject quit_btn;
	public GameObject account_btn;

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
		if(CommonData.VERSION == VERSION_TYPE.VERSION_INNER)
		{
			music_obj.transform.localPosition += new Vector3(0,7,0);
			btn_91.SetActiveRecursively(false);
			quit_btn.SetActiveRecursively(false);
			account_btn.SetActiveRecursively(true);
		}
		else if(CommonData.VERSION == VERSION_TYPE.VERSION_APPSTORE)
		{
			music_obj.transform.localPosition += new Vector3(0,7,0);
			btn_91.SetActiveRecursively(false);
			quit_btn.SetActiveRecursively(false);
			account_btn.SetActiveRecursively(true);
		}
	}
	// Use this for initialization
	void Start () {
		if(CommonData.player_online_info.ShowVip == 1)
		{
			show_vip_On.isChecked = true;
		}
		else 
		{
			show_vip_On.isChecked = false;
		}
		//PlayerPrefs.SetInt("setting_first",0);
		//other_music_slider.sliderValue = SoundManager.click_music.volume;
		now_server.text = CommonData.select_server.ServerName;
		//account_name.text = CommonData.player_online_info.AccountName;
		// set alert on if it is the first time to play
		if( PlayerPrefs.GetInt("setting_first")==0 ) {
			//print("*** first time setting ***");
			PlayerPrefs.SetInt("setting_first",1);
			PlayerPrefs.SetInt("setting_alert_on",1);
			bg_music_slider.sliderValue = SoundManager.mainscene_music.volume;
		}
		else
		{
			bg_music_slider.sliderValue = PlayerPrefs.GetFloat("setting_music_vol");
		}
		chk_AlertSound_On.isChecked = PlayerPrefs.GetInt("setting_alert_sound_on")>0 ? true : false;
		chk_Alert_On.isChecked = PlayerPrefs.GetInt("setting_alert_on")>0 ? true : false;
	}

	// Update is called once per frame
	void Update () {
	
	}
	//弹出窗口    
	void RevealPanel()
	{
		tween_position.Play(true);
	}
	//关闭窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//更改背景音乐大小 
	void ChangeBgMusicVolume()
	{
		SoundManager.ChangeBgMusicVolume(bg_music_slider.sliderValue);
		PlayerPrefs.SetFloat("setting_music_vol", bg_music_slider.sliderValue);
	}
	//更改声效大小 
	void ChangeOtherMusicVolume()
	{
		//SoundManager.click_music.volume = other_music_slider.sliderValue;
	}
	//退出游戏 
	void QuitGame()
	{
		Application.Quit();
	}
	//反馈问题 
	void GmMail()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("WriteMessageWin");
		if (obj != null)
		{
			WriteMessageWin win = obj.GetComponent<WriteMessageWin>(); 
			win.char_id = 0;
			win.recv_charname.text = "GM";
			win.MailType = (byte)MAIL_TYPE.MAIL_TYPE_GM;
			obj.SendMessage("RevealPanel");
		}
	}
	//打开91平台 
	void Open91Platform()
	{
		U3dCmn.SendMessage("91Manager","Open91Platform",null);
	}
	//
	void OnAlertSoundValueChanged(bool val)
	{
		PlayerPrefs.SetInt("setting_alert_sound_on", val?1:0);
		U3dCmn.SendMessage("91Manager","OnAlertSoundSwitcherValueChanged",val);
	}
	void OnAlertValueChanged(bool val)
	{
		PlayerPrefs.SetInt("setting_alert_on", val?1:0);
		U3dCmn.SendMessage("91Manager","OnAlertSwitcherValueChanged",val);
	}
	//打开账户管理窗口 
	void OpenAccountManageWin()
	{
		//if(CommonData.VERSION == VERSION_TYPE.VERSION_APPSTORE)
		//{
		//	U3dCmn.SendMessage("91Manager","getLoginMacAddress", null);
		//}
		//else 
		//{
			GameObject win = U3dCmn.GetObjFromPrefab("AccountManageWin"); 
			if(win != null)
			{
				win.SendMessage("RevealPanel",null);
			}
		//}
		
	}
	//控制VIP显示 
	void OnShowVipChanged(bool val)
	{
		CTS_GAMECMD_OPERATE_SET_VIP_DISPLAY_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_SET_VIP_DISPLAY;
		if(val)
		{
			req.bVipDisplay3 = 1;
		}
		else 
		{
			req.bVipDisplay3 = 0;
		}
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_SET_VIP_DISPLAY_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_SET_VIP_DISPLAY);
		//CTS_GAMECMD_OPERATE_SET_VIP_DISPLAY_T
	}
	//控制VIP显示 
	void OnShowVipChangedRst(byte[] buff)
	{
		
		STC_GAMECMD_OPERATE_SET_VIP_DISPLAY_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_SET_VIP_DISPLAY_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_SET_VIP_DISPLAY_T.enum_rst.RST_OK)
		{
			CommonData.player_online_info.ShowVip = sub_msg.bVipDisplay2;
		}
	}
}
