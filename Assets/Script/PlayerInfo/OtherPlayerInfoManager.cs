using UnityEngine;
using System.Collections;
using System;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class OtherPlayerInfoManager : MonoBehaviour {
	//获取其他玩家基本信息的回调注册表 里面包含需要回调的object和回调函数名 
	public static Hashtable CallBackObjMap = new Hashtable();
	// Use this for initialization
	void Start () {
	
	}
	//通过ID请求玩家基本信息 
	public static void GetOtherPlayerCard(ulong account_id)
	{
		CTS_GAMECMD_GET_PLAYERCARD_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_PLAYERCARD;
		req.nAccountID3 = account_id;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_PLAYERCARD_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_PLAYERCARD);
	}
	//通过玩家名字查找玩家基本信息 
	public static void GetOtherPlayerCard(string char_name)
	{
		CTS_GAMECMD_GET_PLAYERCARD_NAME_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_PLAYERCARD_NAME;
		req.szCharName4 = DataConvert.StrToBytes(char_name);
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_PLAYERCARD_NAME_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_PLAYERCARD);
	}
	//注册其他玩家基本信息UI回调 
	public static void RegisterCallBack(GameObject obj,string func_name)
	{
		if(!CallBackObjMap.Contains(obj))
			CallBackObjMap.Add(obj,func_name);
	} 
	//注销回调  
	public static void UnRegisterCallBack(GameObject obj)
	{
		if(CallBackObjMap.Contains(obj))
		{
			CallBackObjMap.Remove(obj);
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
	//处理玩家基本信息
	public static void ProcessOtherPlayerInfo(byte[] buff)
	{
		STC_GAMECMD_GET_PLAYERCARD_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_PLAYERCARD_T>(buff);
		
		//执行各UI OBJ的回调函数刷新界面 
		Hashtable callback_map = (Hashtable)CallBackObjMap.Clone();
		foreach(DictionaryEntry	de	in	callback_map)  
  		{
			GameObject obj = (GameObject)de.Key;
			string func_name = (string)de.Value;
			if(obj!=null && func_name!="")
			{
				obj.SendMessage(func_name,sub_msg);
			}
		}
	}
}
