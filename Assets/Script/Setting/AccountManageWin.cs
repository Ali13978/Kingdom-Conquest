using UnityEngine;
using System.Collections;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class AccountManageWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	public UIInput account_mail;
	public UIInput new_pass;
	public UIInput confirm_pass;
	public UILabel old_pass_label;
	public UIInput old_pass;
	public GameObject unregister_obj;
	public GameObject register_btn;
	public GameObject band_btn;
	public GameObject modify_pass_btn;
	public GameObject describe_pass_new;
	public GameObject describe_pass_confirm;
	public UILabel 	 register_desc;
	//是注册账号还是修改密码 1是注册 2是修改 
	int register_or_modify;
	
	void Awake()
	{
		if(CommonData.player_online_info.BindMail == 0)
		{
			register_desc.gameObject.SetActiveRecursively(true);
			old_pass.gameObject.SetActiveRecursively(false);
			old_pass_label.gameObject.SetActiveRecursively(false);
			unregister_obj.transform.localPosition += new Vector3(0,-31,0);
			modify_pass_btn.SetActiveRecursively(false);
			band_btn.SetActiveRecursively(false);
		}
		else 
		{
			register_desc.gameObject.SetActiveRecursively(false);
			account_mail.text = CommonData.player_online_info.AccountName;
			if(CommonData.NowDeviceMac == CommonData.AccountBandMac)
			{
				//modify_pass_btn.transform.localScale= new Vector3(1,1,1);
				modify_pass_btn.transform.localPosition += new Vector3(-97,0,0);
				band_btn.SetActiveRecursively(false);
			}
			account_mail.gameObject.GetComponent<Collider>().enabled =  false;
			register_btn.SetActiveRecursively(false);
		}
	}
	// Use this for initialization
	void Start () {
		if(new_pass.text != "")
		{
			describe_pass_new.SetActiveRecursively(false);
		}
		if(confirm_pass.text != "")
		{
			describe_pass_confirm.SetActiveRecursively(false);
		}
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
		if(GameObject.Find("SettingWin") == null)
		{
			if(CommonData.player_online_info.FreeDrawLotteryNum >0)
			{
				GameObject win = U3dCmn.GetObjFromPrefab("ZhuanPan"); 
				if(win != null)
				{
					win.SendMessage("RevealPanel");
				}
			}	
		}
		
	}
	//检查邮箱格式是否正确 
	void MailInputCheck()
	{
		if(!U3dCmn.CheckMailFormat(account_mail.text))
		{
			
			account_mail.text = "";
		}
		//print ("asdafffffffff");
		//if(U3dCmn.)
	}
	//新密码获得焦点 隐藏描述字符 
	void NewPassInput()
	{
		describe_pass_new.SetActiveRecursively(false);
	}
	//检查新密码 
	void NewPassInputCheck()
	{
		if(new_pass.text.Length > 0)
		{
			describe_pass_new.SetActiveRecursively(false);
			if(!U3dCmn.CheckPassFormat(new_pass.text))
			{
				new_pass.text = "";
				describe_pass_new.SetActiveRecursively(true);
			}
		}
		else 
		{
			describe_pass_new.SetActiveRecursively(true);
		}
	}
	//确认密码获得焦点 隐藏描述字符 
	void ConfirmPassInput()
	{
		describe_pass_confirm.SetActiveRecursively(false);
	}
	//检查确认密码 
	void ConfirmPassInputCheck()
	{
		if(confirm_pass.text.Length > 0)
		{
			describe_pass_confirm.SetActiveRecursively(false);
			if(!U3dCmn.CheckPassFormat(confirm_pass.text))
			{
				confirm_pass.text = "";
				describe_pass_confirm.SetActiveRecursively(true);
			}
			
		}
		else 
		{
			describe_pass_confirm.SetActiveRecursively(true);
		}
	}
	
	//注册邮箱账号  
	void RegisterMailAccount()
	{
		if(U3dCmn.CheckPassFormat(new_pass.text) && U3dCmn.CheckPassFormat(confirm_pass.text) && U3dCmn.CheckMailFormat(account_mail.text))
		{
			if(new_pass.text != confirm_pass.text)
			{
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.PASS_NOT_SAME));
				return ;
			}
			MD5 md5hash = new MD5();
			
			CTS_GAMECMD_OPERATE_REGISTER_ACCOUNT_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_REGISTER_ACCOUNT;
			req.szAccount4 = DataConvert.StrToBytes(account_mail.text);
			req.szPass6 = DataConvert.StrToBytes(md5hash.CalcMD5String(DataConvert.StrToBytes(new_pass.text)));
			req.nPassLen5 = req.szPass6.Length;
			req.szOldPass8 = DataConvert.StrToBytes("");
			req.nOldPassLen7 = 0;
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_REGISTER_ACCOUNT_T>(req);
			register_or_modify = 1;
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_REGISTER_ACCOUNT);
		}
	}
	//修改密码 
	void ModifyPass()
	{
		if(U3dCmn.CheckPassFormat(new_pass.text) && U3dCmn.CheckPassFormat(confirm_pass.text) && U3dCmn.CheckMailFormat(account_mail.text))
		{
			if(new_pass.text != confirm_pass.text)
			{
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.PASS_NOT_SAME));
				return ;
			}
			else if(old_pass.text.Length <=0)
			{
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.OLD_PASS_WRONG));
			}
			MD5 md5hash = new MD5();
			CTS_GAMECMD_OPERATE_REGISTER_ACCOUNT_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_REGISTER_ACCOUNT;
			req.szAccount4 = DataConvert.StrToBytes(account_mail.text);;
			req.szPass6 = DataConvert.StrToBytes(md5hash.CalcMD5String(DataConvert.StrToBytes(new_pass.text)));
			req.nPassLen5 = req.szPass6.Length;
			req.szOldPass8 =DataConvert.StrToBytes(md5hash.CalcMD5String(DataConvert.StrToBytes(old_pass.text)));
			req.nOldPassLen7 = req.szOldPass8.Length;
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_REGISTER_ACCOUNT_T>(req);
			register_or_modify = 2;
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_REGISTER_ACCOUNT);
		}
	}
	//注册邮箱账号或者修改密码返回 
	void MailAccountRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_REGISTER_ACCOUNT_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_REGISTER_ACCOUNT_T>(buff);
		//print ("xxxxxxxxxxxxxx"+sub_msg.nRst1);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_REGISTER_ACCOUNT_T.enum_rst.RST_OK)
		{
			if(register_or_modify == 1)
			{
				CommonData.player_online_info.BindMail = 1;
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.REGISTER_MAIL_ACCOUNT_OK));
			}
			else if(register_or_modify == 2)
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.MODIFY_PASS_OK));
			MD5 md5hash = new MD5();
			CommonData.player_online_info.AccountName = account_mail.text;
			CommonData.player_online_info.AccountPass = md5hash.CalcMD5String(DataConvert.StrToBytes(new_pass.text));
			
			PlayerPrefs.SetString("AccountName",CommonData.player_online_info.AccountName);
			PlayerPrefs.SetString("AccountPass",CommonData.player_online_info.AccountPass);
			//CommonData.
			DismissPanel();
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_REGISTER_ACCOUNT_T.enum_rst.RST_MAIL_REGISGERED)
		{
			
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.THE_MAIL_REGISTERED));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_REGISTER_ACCOUNT_T.enum_rst.RST_OLD_PASS_ERR)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.OLD_PASS_WRONG));
		}
	}
	//绑定设备 
	void BandUdid()
	{
		if(CommonData.NowDeviceMac != "")
		{
			CTS_GAMECMD_OPERATE_BIND_DEVICE_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_BIND_DEVICE;
			req.szMAC4 = DataConvert.StrToBytes(CommonData.NowDeviceMac);
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_BIND_DEVICE_T>(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_BIND_DEVICE);
		}
		//CTS_GAMECMD_OPERATE_BIND_DEVICE	
		
	}
	//绑定设备返回 
	void BandUdidRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_BIND_DEVICE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_BIND_DEVICE_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_BIND_DEVICE_T.enum_rst.RST_OK)
		{
			CommonData.AccountBandMac = CommonData.NowDeviceMac;
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.BAND_DEVICE_OK));
			DismissPanel();
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_BIND_DEVICE_T.enum_rst.RST_BINDED_IN_GROUP)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.DEVICE_BOUND_ACCOUNT));	
		}
	}
}
