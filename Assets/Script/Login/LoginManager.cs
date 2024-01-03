using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using CMNCMD;
using CTSCMD;
using TMPro;

public class LoginManager : MonoBehaviour {
	public static bool  enable_login = false; 
	delegate void func();
	func ConnectOKCallBack;
	//是否是用账号名字登陆 
	bool LoginWithAccountName = false;
	//创建账号面板 
	public TweenPosition create_account_panel;
	public UIInput manual_account;
	public UIInput manual_pass;
	public UILabel pass_desc;
	public UIInput confirm_manual_pass;
	public UILabel confirm_pass_desc;
	//创建角色面板
	public TweenPosition create_char_panel;
	public UIGrid 		 head_grid;
	public GameObject 	 head_icon;
	public UILabel account_name;
	//public UIInput char_name;
	public TMP_InputField char_name;
	//内部版本按钮 
	public GameObject inner_obj;
	public GameObject inner_trybtn;
	public GameObject inner_startbtn;
	//91版本按钮 
	public GameObject obj_91;
	//苹果版本按钮 
	public GameObject apple_obj;
	//账号界面 
	public GameObject account_obj;
	public GameObject account_register_btn;
	public GameObject account_login_btn;
	public UIRoot uiroot;
	GameObject select_head_icon;
	List<GameObject> HeadObjList = new List<GameObject>();
	//账号管理页面
	//public UIInput account_input_name;
	public TMP_InputField account_input_name;
	//public UIInput account_input_pass;
	public TMP_InputField account_input_pass;

	//版本号 
	public UILabel version_label;
	//大区 
	public TweenPosition server_list_panel;
	public UILabel server_name_label;
	public UIGrid 		 server_list_grid;
	public GameObject 	 server_item;
	public byte[] dat;
	
	// 为了载入MB 
	public static  bool	canLoadMB		= false;
	public static bool	alreadyLoadMB	= false;

	public static LoginManager instance;

	public GameObject loginPanel;
	public GameObject switchAccPannel;
	public GameObject createCharPanel;

	void Awake()
	{
		instance = this;

		if(CommonData.VERSION == VERSION_TYPE.VERSION_INNER)
		{
			inner_obj.transform.localPosition =  inner_obj.transform.localPosition+ new Vector3(0,200,0);
			account_register_btn.gameObject.SetActiveRecursively(true);
			//inner_trybtn.transform.localPosition = inner_trybtn.transform.localPosition+ new Vector3(0,100,0);
			//inner_startbtn.transform.localPosition =  inner_startbtn.transform.localPosition+ new Vector3(0,100,0);
			//inner_accountbtn.transform.localPosition =  inner_accountbtn.transform.localPosition+ new Vector3(0,100,0);
			//account_name.transform.localPosition =  account_name.transform.localPosition+ new Vector3(0,-200,0);
		}
		else if(CommonData.VERSION == VERSION_TYPE.VERSION_APPSTORE)
		{
			inner_obj.transform.localPosition =  inner_obj.transform.localPosition+ new Vector3(0,200,0);
			//apple_obj.transform.localPosition =  apple_obj.transform.localPosition+ new Vector3(0,200,0);
			account_register_btn.gameObject.SetActiveRecursively(false);
			//account_obj.transform.localPosition = account_obj.transform.localPosition+ new Vector3(-38,0,0);
			//account_register_btn.SetActiveRecursively(false);
			//account_login_btn.transform.localPosition = account_login_btn.transform.localPosition+ new Vector3(-85.5f,0,0);
		}
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			uiroot.manualHeight = 320;
			CommonMB.InitialCommDefMBInfo();
			if(U3dCmn.GetCmnDefNum((uint)CMN_DEF.ADMOB_LOGIN) != 0)
			{
				string admob_iphone_id = "";
				if(CommonData.APP_VERSION == APP_VERSION_TYPE.TINYKINGDOM)
					admob_iphone_id = "972a3abc8a474d10";
				
				//AdMobBinding.init(admob_iphone_id);
				//AdMobBinding.createBanner( AdMobBannerType.iPhone_320x50, AdMobAdPosition.TopCenter);
			}
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			uiroot.manualHeight = 360;
			CommonMB.InitialCommDefMBInfo();
			if(U3dCmn.GetCmnDefNum((uint)CMN_DEF.ADMOB_LOGIN) != 0)
			{
				string admob_ipad_id = "";
				if(CommonData.APP_VERSION == APP_VERSION_TYPE.TINYKINGDOM)
					admob_ipad_id = "0103dda42c5247a1";

				
				//AdMobBinding.init(admob_ipad_id);
				//AdMobBinding.createBanner( AdMobBannerType.iPad_728x90, AdMobAdPosition.TopCenter);
			}
		}
	}
	// Use this for initialization
	void Start () {	
		LoadUserInfo();
		
	}
	// Update is called once per frame
	void Update() {
		if (canLoadMB && !alreadyLoadMB)
		{
			
			CommonMB.InitialMB();
			alreadyLoadMB	= true;
			SoundManager.PlayLoginMusic();
			InitialServerList();
			CommonData.select_server.ClientVersion = CommonMB.ClientVersion;
			if(CommonData.VERSION == VERSION_TYPE.VERSION_APPSTORE)
			{
				version_label.text = "v"+CommonMB.ClientVersion;
				CheckVersion();
				if(U3dCmn.GetCmnDefNum((uint)CMN_DEF.PLAYHAVEN_LOGIN) != 0)
				{
			
				}
				
				//U3dCmn.GetObjFromPrefab("91Manager").SendMessage("ShowAdmob");
			}
		}
		if(enable_login)
		{
			ConnectOKCallBack();
			enable_login = false;
		}
	}
	//检查版本是否一致 
	void CheckVersion()
	{
		string client = CommonMB.ClientVersion.Split('.')[0];	
		string server = U3dCmn.GetServerVersionByAppID(CommonMB.AppID).Split('.')[0];//CommonMB.ServerVersion.Split('.')[0];
		if(client != server)
		{
			//大版本不一致 阻止玩家进入 提醒玩家更新版本 
			U3dCmn.GetObjFromPrefab("RenewVersionWin").SendMessage("RevealPanel");
			
			
		}
	}
	public void LoadUserInfo()
	{
		CommonData.player_online_info.AccountName = PlayerPrefs.GetString("AccountName");
		CommonData.select_server.ServerName = PlayerPrefs.GetString("ServerName");
		//if(CommonData.player_online_info.AccountName != "")
		//{
			
			CommonData.player_online_info.AccountPass = PlayerPrefs.GetString("AccountPass");
		
			inner_trybtn.SetActiveRecursively(false);
			inner_startbtn.SetActiveRecursively(true);
			//account_name.text = CommonData.player_online_info.AccountName;
			
		//}
		/*else
		{
			inner_trybtn.SetActiveRecursively(true);
			inner_startbtn.SetActiveRecursively(false);
			account_name.text = CommonData.player_online_info.AccountName;
		}*/
	}
	//玩家输入账号登陆 
	void InputLogin()
	{
		MD5 md5hash = new MD5();
		if(account_input_name.text == "")
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ACCOUNT_EMPUTY));
			return;
		}
		CommonData.player_online_info.AccountName = account_input_name.text;
		//if(account_input_pass.text =="")
		//{
		//	CommonData.player_online_info.AccountPass = "";
		//}
		//else
			CommonData.player_online_info.AccountPass  =  md5hash.CalcMD5String(DataConvert.StrToBytes(account_input_pass.text));
		
		
		LoadingManager.instance.ShowLoading();
		ConnectOKCallBack = StartLoginCallBack;
		ConnectServer();
	}
	//内部账号登陆 
	public void StartLogin()
	{
		//if(CommonData.VERSION == VERSION_TYPE.VERSION_APPSTORE)
		//{
		CommonData.player_online_info.AccountName = PlayerPrefs.GetString("AccountName");
		CommonData.player_online_info.AccountPass = PlayerPrefs.GetString("AccountPass");
		if(CommonData.player_online_info.AccountName != "")
		{
			LoadingManager.instance.ShowLoading();
			ConnectOKCallBack = StartLoginCallBack;
			ConnectServer();
		}	
		else 
		{
			CreateAccount();
		}
		//}
		/*else 
		{
			GameObject API91Man = GameObject.Find("91Manager");
			if( API91Man!=null )
			{
				API91Man.SendMessage("getLoginMacAddress", null);
				LoadingManager.instance.ShowLoading();
			}
			LoginWithAccountName = true;
		}*/
		//SoundManager.PlayClickSound();
		
	}
	
	//账号登陆回调 
	public void StartLoginCallBack()
	{
		MD5 md5hash =  new MD5();
		TTY_CLIENT_LPGAMEPLAY_LOGIN_T req =  new TTY_CLIENT_LPGAMEPLAY_LOGIN_T();
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_LOGIN;
		req.szAccount3 = DataConvert.StrToBytes(CommonData.player_online_info.AccountName);
		req.nAppID5 = (uint)CommonMB.AppID;
		byte[] bytepass = DataConvert.StrToBytes(CommonData.player_online_info.AccountPass);
		//req.nDeviceType4 = (uint)UnityEngine.iOS.Device.generation;
		req.szPass9=  bytepass;
		req.nPassLen8 = req.szPass9.Length;
		req.szMac7 =  DataConvert.StrToBytes(CommonData.NowDeviceMac);

		
		TcpMsger.SendLogicData<TTY_CLIENT_LPGAMEPLAY_LOGIN_T >(req);
	}
	
	//无需输入账号名一键创建账号（一般是内部版本） 
	public void CreateAccount()
	{
		//if(CommonData.VERSION == VERSION_TYPE.VERSION_INNER)
		//{
		LoadingManager.instance.ShowLoading();
		ConnectOKCallBack = CreateAccountCallBack;
			ConnectServer();
			loginPanel.SetActive(false);
			switchAccPannel.SetActive(false);
			createCharPanel.SetActive(true);
		//}
		//else if(CommonData.VERSION == VERSION_TYPE.VERSION_APPSTORE)
		//{
		//	create_account_panel.Play(true);
		//}
	}
	//一键创建账号回调 
	public void CreateAccountCallBack()
	{
		MD5 md5hash =  new MD5();
		TTY_CLIENT_LPGAMEPLAY_CREATEACCOUNT_T req = new TTY_CLIENT_LPGAMEPLAY_CREATEACCOUNT_T();
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_CREATEACCOUNT;
		byte[] account =  new byte[32];
		req.szAccountName3 = account;
		string pass = "";
		//byte[] pass =  new byte[256];
		req.szPass5 = DataConvert.StrToBytes(md5hash.CalcMD5String(DataConvert.StrToBytes(pass)));
		req.szMAC7 = DataConvert.StrToBytes(CommonData.NowDeviceMac);
		//req.nDeviceType8 = (uint)UnityEngine.iOS.Device.generation;
		req.bUseRandomName9 = 1;
		req.nAppID10 = CommonMB.AppID;
		TcpMsger.SendLogicData<TTY_CLIENT_LPGAMEPLAY_CREATEACCOUNT_T >(req);
		CommonData.player_online_info.AccountPass = md5hash.CalcMD5String(DataConvert.StrToBytes(""));
	}
	//使用邮箱和密码创建账号 
	public void CreateAccountManual()
	{
		if(U3dCmn.CheckMailFormat(manual_account.text) && U3dCmn.CheckPassFormat(manual_pass.text) && U3dCmn.CheckPassFormat(confirm_manual_pass.text))
		{
			if(manual_pass.text != confirm_manual_pass.text)
			{
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.PASS_NOT_SAME));
				return ;
			}
			ConnectOKCallBack = CreateAccountManualCallBack;
			ConnectServer();
			LoadingManager.instance.ShowLoading();
		}
	}
	
	//密码获取焦点 隐藏描述字符 
	void ManulPassInput()
	{
		pass_desc.gameObject.SetActiveRecursively(false);
	}
	//检查密码 
	void ManulPassInputCheck()
	{
		if(manual_pass.text.Length > 0)
		{
			pass_desc.gameObject.SetActiveRecursively(false);
			if(!U3dCmn.CheckPassFormat(manual_pass.text))
			{
				manual_pass.text = "";
				pass_desc.gameObject.SetActiveRecursively(true);
			}
		}
		else 
		{
			pass_desc.gameObject.SetActiveRecursively(true);
		}
		
	}
	//确认密码获取焦点 隐藏描述字符 
	void ConfirmPassInput()
	{
		confirm_pass_desc.gameObject.SetActiveRecursively(false);
	}
	//检查确认密码 
	void ConfirmPassInputCheck()
	{
		if(confirm_manual_pass.text.Length > 0)
		{
			confirm_pass_desc.gameObject.SetActiveRecursively(false);
			if(!U3dCmn.CheckPassFormat(confirm_manual_pass.text))
			{
				confirm_manual_pass.text = "";
				confirm_pass_desc.gameObject.SetActiveRecursively(true);
			}
			
		}
		else 
		{
			confirm_pass_desc.gameObject.SetActiveRecursively(true);
		}
		
	}
	//使用邮箱和密码创建账号回调  
	public void CreateAccountManualCallBack()
	{
		MD5 md5hash =  new MD5();
		TTY_CLIENT_LPGAMEPLAY_CREATEACCOUNT_T req = new TTY_CLIENT_LPGAMEPLAY_CREATEACCOUNT_T();
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_CREATEACCOUNT;
		//byte[] account =  new byte[32];
		req.szAccountName3 = DataConvert.StrToBytes(manual_account.text);
		//string pass = "";
		//byte[] pass =  new byte[256];
		req.szPass5 = DataConvert.StrToBytes(md5hash.CalcMD5String(DataConvert.StrToBytes(manual_pass.text)));
		req.szMAC7 = DataConvert.StrToBytes(CommonData.NowDeviceMac);
		//req.nDeviceType8 = (uint)UnityEngine.iOS.Device.generation;
		req.bUseRandomName9 = 0;
		req.nAppID10 = CommonMB.AppID;
		TcpMsger.SendLogicData<TTY_CLIENT_LPGAMEPLAY_CREATEACCOUNT_T >(req);
		CommonData.player_online_info.AccountPass = md5hash.CalcMD5String(DataConvert.StrToBytes(manual_pass.text));
	}
	//连接服务器 
	void   ConnectServer()
	{	
		//IPAddress[] ips = Dns.GetHostAddresses("pixelgame.vicp.net");
		//print (ips[0]);
		//TcpMsger.ConnectCaafs("192.168.11.20",4200,3000);	
		//IPAdress[] ips= GetHostAddresses("pixelgame.vicp.net")
		//ip_address = GetIPAddress();
		if(CommonData.select_server.ServerIsFull == 0)
			TcpMsger.ConnectCaafs(CommonData.select_server.ServerIP,CommonData.select_server.ServerPort,3000);	
		else 
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.SERVER_IS_FULL));
	}
	/*public string GetIPAddress()
	{
		string ip_str = null;
		
		ip_str = CommonData.select_server.ServerIP;
		
		
		return ip_str;
	}*/
	//弹出创建角色面板  
	public void ShowCreateCharPanel()
	{
		// in case this panel popout while first login  
		LoadingManager.instance.HideLoading();
		//TextAsset name_asset = (TextAsset)Resources.Load("player_name",typeof(TextAsset));
		//dat = DataConvert.StrToBytes(name_asset.text);
		//string name = GetRandomName(true);
		
		//char_name.text = U3dCmn.GetRandomName(true);
		char_name.text	= /*U3dCmn.GetRandomNameEn(true, true) */" ";
		if(U3dCmn.GetChildObjByName(head_grid.gameObject,"head0") == null)
		{
			HeadObjList.Clear();
			for(int i=0; i<9;i++)
			{
				
				GameObject icon = NGUITools.AddChild(head_grid.gameObject, head_icon);
				icon.name = "head"+(i+1);
				
				icon.GetComponentInChildren<UISprite>().spriteName = "junzhu"+(i+1);
				if(i==0)
				{
					select_head_icon = icon;
				}
				else 
				{
					U3dCmn.GetChildObjByName(icon.gameObject,"SelectBg").gameObject.SetActiveRecursively(false);
				}
				
				UIEventListener.Get(icon).onClick -= ShowDetail;
				UIEventListener.Get(icon).onClick += ShowDetail;
				HeadObjList.Add(icon);
				
				//JiangLingObjList.Add(jiangling_icon);
				//jiangling_icon.SendMessage("InitialJiangLingInfo",hero_unit);
				
			}
			head_grid.Reposition();
		}
		
		create_char_panel.Play(true);
	}
	//关闭创建角色面板 
	void DismissCreateCharPanel()
	{
		foreach(Transform obj in head_grid.gameObject.transform)
		{
			Destroy(obj.gameObject);
		}
		HeadObjList.Clear();
		create_char_panel.Play(false);
	}

	void ShowDetail(GameObject obj)
	{
		//print (obj.name);
		for(int i=0 ;i<HeadObjList.Count;i++)
		{
			U3dCmn.GetChildObjByName(HeadObjList[i].gameObject,"SelectBg").SetActiveRecursively(false);
			//HeadObjList[i].GetComponentInChildren<UISlicedSprite>().gameObject.SetActiveRecursively(false);
		}
		select_head_icon = obj;
		//GameObject icon_bg  = U3dCmn.GetChildObjByName(select_head_icon.gameObject,"HeadIcon");
		//print (icon_bg.GetComponent<UISprite>().spriteName);
		U3dCmn.GetChildObjByName(select_head_icon.gameObject,"SelectBg").SetActiveRecursively(true);
		//select_hero_icon = obj;
		//SetJiangLingInfoPanelData(obj.GetComponent<JiangLingIcon>().now_hero);
	}
	//更换名字 
	void ChangeName()
	{
		//SoundManager.PlayClickSound();
		//char_name.text =  U3dCmn.GetRandomName(true);
		string icon_name = select_head_icon.name;
		string icon_head ="head";
		string num_str = icon_name.Substring(icon_head.Length,icon_name.Length-icon_head.Length);
		uint head_id = uint.Parse(num_str);
		if(head_id == 3 || head_id == 5 || head_id==7 )
			char_name.text	= U3dCmn.GetRandomNameEn(false,false);
		else 
			char_name.text	= U3dCmn.GetRandomNameEn(true,false);
	}
	
	//创建角色 
	void CreateChar()
	{
		if(char_name.text.Length<1)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NAME_IS_NULL));
			return ;
		}
		//SoundManager.PlayClickSound();
		if(select_head_icon == null)
		{
			return ;	
		}
		if(U3dCmn.IsContainBadName(char_name.text))
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.BAD_NAME));
			return;
		}
		TTY_CLIENT_LPGAMEPLAY_CREATECHAR_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_CREATECHAR;
		req.szName3 = DataConvert.StrToBytes(char_name.text);
		if(req.szName3.Length > TTY_CLIENT_LPGAMEPLAY_CREATECHAR_T.szNameLen2)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NAME_TOO_LONG));
			return ;	
		}
		req.nSex4 = 0;
		string icon_name = select_head_icon.name;
		string icon_head ="head";
		string num_str = icon_name.Substring(icon_head.Length,icon_name.Length-icon_head.Length);
		req.nHeadID5 = uint.Parse(num_str);
		req.nCountry6 = 1;
		TcpMsger.SendLogicData<TTY_CLIENT_LPGAMEPLAY_CREATECHAR_T >(req);
		LoadingManager.instance.ShowLoading();
		string gCol = string.Format(TaskManager.QUEST_POP,CommonData.player_online_info.AccountName);
		PlayerPrefs.SetInt(gCol, 1);
	}
	//打开大区列表界面 
	void OpenServerListWin()
	{
		//int i = 1;
		Hashtable ServerInfoMap = new Hashtable();
		foreach(DictionaryEntry de in CommonMB.ServerInfo_Map)
		{
			
			ServerInfo info = (ServerInfo)de.Value;
			ServerInfoMap.Add(info.Priority,info);
		}
		ArrayList list = new ArrayList(ServerInfoMap.Keys);
      	list.Sort();
		for(int i=0; i<list.Count;i++)
		{
			ServerInfo info = (ServerInfo)ServerInfoMap[list[i]];
			GameObject item = NGUITools.AddChild(server_list_grid.gameObject, server_item);
			item.name = "head"+(i+1);
			item.SendMessage("InitialData",info);
		}
		server_list_grid.Reposition();
		server_list_panel.Play(true);
	}
	//关闭大区列表 
	void DismissServerListWin()
	{
		foreach(Transform obj in server_list_grid.gameObject.transform)
		{
			Destroy(obj.gameObject);
		}
		server_list_panel.Play(false);
	}
	//初始化大区列表 
	void InitialServerList()
	{
		//string servername = PlayerPrefs.GetString("ServerName");
		ServerInfo info  = new ServerInfo();
		if(CommonData.select_server.ServerName == "" || !CommonMB.ServerInfo_Map.Contains(CommonData.select_server.ServerName))
		{
			string server_name = "";
			//不太可能开到10000个服吧 
			int priority = 10000;
			foreach(DictionaryEntry de in CommonMB.ServerInfo_Map)
			{
				info = (ServerInfo)de.Value;
				if(info.Priority < priority)
				{
					server_name = info.ServerName;
					priority = info.Priority;
				}
					
				//break;
			}
			info = (ServerInfo)CommonMB.ServerInfo_Map[server_name];
		}
		else 
		{
			info = (ServerInfo)CommonMB.ServerInfo_Map[CommonData.select_server.ServerName];
		}
		if(info.ServerStatus!="")
			server_name_label.text = info.ServerName+info.ServerStatus;
		else
			server_name_label.text = info.ServerName;
		CommonData.select_server.ServerName = info.ServerName;
		CommonData.select_server.ServerIP =info.ServerIP;//"72.14.191.241";
		CommonData.select_server.ServerPort = info.ServerPort;
		CommonData.select_server.ServerStatus = info.ServerStatus;
		CommonData.select_server.ServerIsFull = info.IsFull;
		CommonData.select_server.ServerID = info.ServerID;
	}
	//重新选择大区 
	void ReSelectServer()
	{
		InitialServerList();
		DismissServerListWin();
	}
	//苹果账号登陆 先去获取mac地址然后发起登陆 第一步   
	void AppleStartLogin()
	{
		// tst
		GameObject API91Man = GameObject.Find("91Manager");
		if( API91Man!=null )
		{
			API91Man.SendMessage("getLoginMacAddress", null);
			LoadingManager.instance.ShowLoading();
		}
	}
	// 传回MAC地址登录  第二步 
	void LoginWithMac()
	{
		//DeviceUDID = uuid;
		//SoundManager.PlayClickSound();
		if(LoginWithAccountName)
		{
			ConnectOKCallBack = StartLoginCallBack;
			ConnectServer();
		}
		else
		{
			ConnectOKCallBack = StartLoginUDIDCallBack;
			ConnectServer();
			print("**** LoginWithMac:"+CommonData.NowDeviceMac);
		}
		LoginWithAccountName = false;
		
	}
	//使用UDID账号登陆回调  第三步  
	public void StartLoginUDIDCallBack()
	{
		if(CommonData.NowDeviceMac != "")
		{
			TTY_CLIENT_LPGAMEPLAY_LOGIN_BY_DEVICEID_T req = new TTY_CLIENT_LPGAMEPLAY_LOGIN_BY_DEVICEID_T();
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_LOGIN_BY_DEVICEID;
			req.szMAC3 = DataConvert.StrToBytes(CommonData.NowDeviceMac);
			//req.nDeviceType4 = (uint)UnityEngine.iOS.Device.generation;
			req.nAppID5 = (uint)CommonMB.AppID;
			TcpMsger.SendLogicData<TTY_CLIENT_LPGAMEPLAY_LOGIN_BY_DEVICEID_T >(req);
		}
		else
		{
			if(CommonData.player_online_info.AccountName == "")
			{
				CreateAccount();
			}
			else 
			{
				StartLogin();
			}
			
		}
		/*TTY_CLIENT_LPGAMEPLAY_LOGIN_T req =  new TTY_CLIENT_LPGAMEPLAY_LOGIN_T();
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_LOGIN;
		req.szAccount3 = DataConvert.StrToBytes(CommonData.player_online_info.AccountName);
		byte[] bytepass = DataConvert.StrToBytes(CommonData.player_online_info.AccountPass);
		req.nPassLen4 = bytepass.Length;
		req.szPass5 =  bytepass;
		TcpMsger.SendLogicData<TTY_CLIENT_LPGAMEPLAY_LOGIN_T >(req);*/
		
	}
}
