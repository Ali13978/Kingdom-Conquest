using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class JinKuangFriendItem : MonoBehaviour {
	public UISlicedSprite	select_bg;
	public UISprite jinkang_icon;
	public UISprite yaoqianshu_icon;
	public UILabel	char_name;
	public ulong now_friend_id;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//打开好友的金矿窗口 
	void OpenFriendGoldWin()
	{
		if(now_friend_id!=0)
		{
			FriendUnit unit = FriendManager.GetFriendDataFromID(now_friend_id);
			SIMPLE_CHAR_INFO char_info;
			char_info.AccountId = now_friend_id;
			char_info.CharName = DataConvert.BytesToStr(unit.szName3);
			GameObject infowin = U3dCmn.GetObjFromPrefab("OthersJinKuangWin");
			if(infowin != null)
			{
				infowin.SendMessage("RevealPanel",char_info);
			}
		}
		
	}
}
