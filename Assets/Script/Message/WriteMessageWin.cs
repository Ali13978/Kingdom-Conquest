using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class WriteMessageWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	public UILabel recv_charname;
	public UIInput  context;
	public ulong char_id;
	public byte  MailType;
	//public bool isAllianceMail = false;
	// Use this for initialization
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
	}
	
	void RevealPanel()
	{
		context.text = "";
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	void OnSubmit()
	{
		if(context.text.Length>1024)
		{
			//print ("too lang");
		}
	}
	void SendMessage()
	{
		if(context.text.Length<1) return;
		if(context.text.Length<1024 && char_id>0 && MailType == (byte)MAIL_TYPE.MAIL_TYPE_PRIVATE)
		{
			CTS_GAMECMD_ADD_MAIL_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ADD_MAIL;
			req.nToCharID3 = char_id;
			req.szText5 = DataConvert.StrToBytes(context.text);
			req.nTextLen4 = req.szText5.Length+1;
			TcpMsger.SendLogicData<CTS_GAMECMD_ADD_MAIL_T>(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_ADD_MAIL);
		}
		else if(MailType == (byte)MAIL_TYPE.MAIL_TYPE_ALLIANCE)
		{
			CTS_GAMECMD_OPERATE_ADD_ALLIANCE_MAIL_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_ADD_ALLIANCE_MAIL;
			req.szText4 = DataConvert.StrToBytes(context.text);
			req.nLen3= req.szText4.Length+1;
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_ADD_ALLIANCE_MAIL_T>(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ADD_ALLIANCE_MAIL);
		}
		else if(MailType == (byte)MAIL_TYPE.MAIL_TYPE_GM)
		{
			CTS_GAMECMD_OPERATE_ADD_GM_MAIL_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_ADD_GM_MAIL;
			req.szGmMail4 = DataConvert.StrToBytes(context.text);
			req.nLength3= req.szGmMail4.Length+1;
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_ADD_GM_MAIL_T>(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ADD_GM_MAIL);
		}
	}
}
