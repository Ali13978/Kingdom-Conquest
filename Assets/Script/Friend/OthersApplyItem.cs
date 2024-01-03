using UnityEngine;
using System.Collections;
using STCCMD;
public class OthersApplyItem : MonoBehaviour {
	public UISprite head_icon;
	public UILabel 	char_name;
	public UILabel 	char_level;
	public UIImageButton agree_btn;
	public UIImageButton refuse_btn;
	public FriendApplyUnit now_unit; 
	// Use this for initialization
	void Start () {
	
	}
	
	void InitialData(FriendApplyUnit unit)
	{
		if(unit.nAccountID1 !=0)
		{
			agree_btn.gameObject.SetActiveRecursively(true);
			refuse_btn.gameObject.SetActiveRecursively(true);
			head_icon.gameObject.SetActiveRecursively(true);
			now_unit = unit;
			char_name.text = DataConvert.BytesToStr(unit.szName3);
			char_level.text = "Lv"+unit.nLevel4.ToString();
			//print ("hhhhhhhhhhhhhh"+unit.nHeadID5);
			head_icon.spriteName = U3dCmn.GetCharIconName((int)unit.nHeadID5);
		}
		else 
		{
			char_name.text = "";
			char_level.text = "";
			char_level.text = "";
			head_icon.gameObject.SetActiveRecursively(false);
			agree_btn.gameObject.SetActiveRecursively(false);
			refuse_btn.gameObject.SetActiveRecursively(false);
		}
		
	}
	//同意成为好友
	void AgreeFriend()
	{
		FriendManager.AgreeFriendApply(now_unit);
	}
	//拒绝成为好友 
	void RefuseFriend()
	{
		FriendManager.RefuseFriendApply(now_unit);
		U3dCmn.SendMessage("FriendWin","InitialPanelData",null);
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
