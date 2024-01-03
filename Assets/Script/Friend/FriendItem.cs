using UnityEngine;
using System.Collections;
using STCCMD;
using CMNCMD;
public class FriendItem : MonoBehaviour {
	public UISprite head_icon;
	public UILabel 	char_name;
	public UILabel 	char_level;
	public UIImageButton mail_btn;
	public UIImageButton delete_btn;
	public UIImageButton jinkuang_btn;
	public FriendUnit now_unit; 
	// Use this for initialization
	void Start () {
		
	}
	
	void InitialData(FriendUnit unit)
	{
		now_unit = unit;
		if(unit.nAccountID1 !=0)
		{
			mail_btn.gameObject.SetActiveRecursively(true);
			delete_btn.gameObject.SetActiveRecursively(true);
			head_icon.gameObject.SetActiveRecursively(true);
			jinkuang_btn.gameObject.SetActiveRecursively(true);
			now_unit = unit;
			char_name.text = DataConvert.BytesToStr(unit.szName3);
			char_level.text = "Lv"+unit.nLevel4.ToString();
			head_icon.spriteName = U3dCmn.GetCharIconName((int)unit.nHeadID5);
		}
		else 
		{
			char_name.text = "";
			char_level.text = "";
			char_level.text = "";
			head_icon.gameObject.SetActiveRecursively(false);
			mail_btn.gameObject.SetActiveRecursively(false);
			delete_btn.gameObject.SetActiveRecursively(false);
			jinkuang_btn.gameObject.SetActiveRecursively(false);
		}
		
	}
	//写邮件 
	void WriteMail()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("WriteMessageWin");
		if (obj != null)
		{
			WriteMessageWin win = obj.GetComponent<WriteMessageWin>(); 
			win.char_id = now_unit.nAccountID1;
			win.recv_charname.text = DataConvert.BytesToStr(now_unit.szName3);
			win.MailType = (byte)MAIL_TYPE.MAIL_TYPE_PRIVATE;
			obj.SendMessage("RevealPanel");
		}
	}
	//删除好友 
	void DeleteFriend()
	{
		string str = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.SRUE_DELETE_FRIEND);
		PopConfirmWin.instance.ShowPopWin(str,SrueDeleteFriend);
	}
	void SrueDeleteFriend(GameObject obj)
	{
		FriendManager.DeleteFriend(now_unit.nAccountID1);
	}
	//打开别人的金矿 
	void OpenOtherJinKuang()
	{
		SIMPLE_CHAR_INFO char_info;
		char_info.AccountId = now_unit.nAccountID1;
		char_info.CharName = DataConvert.BytesToStr(now_unit.szName3);
		OthersJinKuangManager.OpenOthersJinKuangWin(char_info);	
	}
	//打开玩家简单信息面板 
	void OpenPlayerCardWin()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("PlayerCardWin");
		if (obj != null)
		{
			obj.SendMessage("RevealPanel",now_unit.nAccountID1);
		}
	}
}
