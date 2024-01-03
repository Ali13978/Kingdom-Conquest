using UnityEngine;
using System.Collections;
using CMNCMD;
using CTSCMD;
public class ReLoginManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(LoginManager.enable_login)
		{
			StartLoginCallBack();
			LoginManager.enable_login = false;
		}
	}
	//账号登陆 
	public void StartLogin()
	{
		LoadingManager.instance.ShowLoading();
		ConnectServer();
	}
	//账号登陆回调 
	public void StartLoginCallBack()
	{
		TTY_CLIENT_LPGAMEPLAY_LOGIN_T req =  new TTY_CLIENT_LPGAMEPLAY_LOGIN_T();
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_LOGIN;
		req.szAccount3 = DataConvert.StrToBytes(CommonData.player_online_info.AccountName);
		byte[] bytepass = DataConvert.StrToBytes(U3dCmn.CheckNullStr(CommonData.player_online_info.AccountPass));
		//req.nDeviceType4 = (uint)UnityEngine.iOS.Device.generation;
		req.nAppID5 = (uint)CommonMB.AppID;
		req.szMac7 = DataConvert.StrToBytes(CommonData.NowDeviceMac);
		req.nPassLen8 = bytepass.Length;
		req.szPass9 =  bytepass;
		TcpMsger.SendLogicData<TTY_CLIENT_LPGAMEPLAY_LOGIN_T >(req);
		
	}
	//连接服务器 
	void   ConnectServer()
	{	
		TcpMsger.ConnectCaafs(CommonData.select_server.ServerIP,CommonData.select_server.ServerPort,3000);		
	}
}
