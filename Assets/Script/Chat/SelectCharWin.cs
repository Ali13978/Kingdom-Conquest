using UnityEngine;
using System.Collections;
using CMNCMD;
public class SelectCharWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	CHAT_DATA now_data;
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
	
	void RevealPanel(CHAT_DATA chat_data)
	{
		now_data = chat_data;
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		U3dCmn.SendMessage("ChatWin","ClearSelectBg",null);
		Destroy(gameObject);
	}
	//加好友 
	void AddFriend()
	{
		FriendManager.ReqAddFriend(now_data.nCharID);
		DismissPanel();
	}
	//私聊 
	void PrivateChat()
	{
		ChatManager.chat_channel = (int)CHATCHANNEL.PRIVATE_CHANNEL;
		ChatManager.to_char_name = now_data.szFrom;
		ChatManager.to_char_id = now_data.nCharID;
		U3dCmn.SendMessage("ChatWin","RefreshPrivateChat",null);
		DismissPanel();
	}
	//发邮件  
	void WriteMail()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("WriteMessageWin");
		if (obj != null)
		{
			WriteMessageWin win = obj.GetComponent<WriteMessageWin>(); 
			win.char_id = now_data.nCharID;
			win.recv_charname.text = now_data.szFrom;
			win.MailType = (byte)MAIL_TYPE.MAIL_TYPE_PRIVATE;
			obj.SendMessage("RevealPanel");
		}
		DismissPanel();
	}
	//查看玩家信息 
	void ShowPlayerInfo()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("PlayerCardWin");
		if (obj != null)
		{
			obj.SendMessage("RevealPanel",now_data.nCharID);
		}
		DismissPanel();
	}
}
