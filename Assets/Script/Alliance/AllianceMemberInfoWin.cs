using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class AllianceMemberInfoWin : MonoBehaviour {
	//消息窗口positon TWEEN     
	public TweenPosition tween_position;  
	public UISprite	head_icon;
	public UILabel	char_name;
	public UILabel	position_label;
	public UILabel	char_level_label;
	public UILabel	glory_label;
	public UILabel	contribution_label;
	public UILabel  char_rank;
	public UISprite	vip_icon;
	public UIImageButton	fire_btn;
	public UIImageButton	authorize_btn;
	public UIImageButton	mail_btn;
	public UIImageButton	friend_btn;
	public UIImageButton    reinforce_btn;
	public GameObject		all_ui;
	AllianceMemberUnit now_unit;
	
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
	
	
	//弹出窗口   
	void RevealPanel(AllianceMemberUnit unit)
	{
		now_unit = unit;
		OtherPlayerInfoManager.GetOtherPlayerCard(unit.nAccountID1);
		//注册回调 
		OtherPlayerInfoManager.RegisterCallBack(gameObject,"InitialData");
		all_ui.gameObject.SetActiveRecursively(false);
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	
	//初始化UI数据  
	void InitialData(STC_GAMECMD_GET_PLAYERCARD_T player_card)
	{
		//注销回调 
		OtherPlayerInfoManager.UnRegisterCallBack(gameObject);
		all_ui.gameObject.SetActiveRecursively(true);
		//根据职位确定是否显示开除和授权按钮 
		if(player_card.nAccountID2 == CommonData.player_online_info.AccountID)
		{
			//如果是自己的信息 
			authorize_btn.gameObject.SetActiveRecursively(false);
			fire_btn.gameObject.SetActiveRecursively(false);
			mail_btn.gameObject.SetActiveRecursively(false);
			friend_btn.gameObject.SetActiveRecursively(false);
			reinforce_btn.gameObject.SetActiveRecursively(false);
		}
		else
		{
			//mail_btn.gameObject.SetActiveRecursively(true);
			//friend_btn.gameObject.SetActiveRecursively(true);
			
			if(U3dCmn.GetCmnDefNum((uint)CMN_DEF.GUILD_REINFORCE) == 1)
			{
				reinforce_btn.gameObject.SetActiveRecursively(true);
			}
			else 
			{
				reinforce_btn.gameObject.SetActiveRecursively(false);
			}
			if(AllianceManager.my_alliance_info.MyPost == (uint)ALLIANCE_POSITION.alliance_position_leader)
			{
				//authorize_btn.gameObject.SetActiveRecursively(true);
				authorize_btn.GetComponent<Collider>().enabled = true;
				authorize_btn.target.spriteName = "button1";
				fire_btn.GetComponent<Collider>().enabled = true;
				fire_btn.target.spriteName = "button1";
				//fire_btn.gameObject.SetActiveRecursively(true);
			}
			else if(AllianceManager.my_alliance_info.MyPost == (uint)ALLIANCE_POSITION.alliance_position_vice_leader)
			{
				bool canBeFire = (now_unit.nPosition4 > (uint)ALLIANCE_POSITION.alliance_position_vice_leader);
				if (true == canBeFire)
				{
					authorize_btn.GetComponent<Collider>().enabled = true;
					authorize_btn.target.spriteName = "button1";

					fire_btn.GetComponent<Collider>().enabled = true;
					fire_btn.target.spriteName = "button1";
				}
				else 
				{
					authorize_btn.GetComponent<Collider>().enabled = false;
					authorize_btn.target.spriteName = "button1_disable";
					
					fire_btn.GetComponent<Collider>().enabled = false;
					fire_btn.target.spriteName = "button1_disable";
				}
			}
			else
			{
				authorize_btn.GetComponent<Collider>().enabled = false;
				authorize_btn.target.spriteName = "button1_disable";
				fire_btn.GetComponent<Collider>().enabled = false;
				fire_btn.target.spriteName = "button1_disable";
			}
		}
		//print("hhhhhhhhhhhhhhhhhh"+player_card.nHeadID8);
		head_icon.spriteName = U3dCmn.GetCharIconName((int)player_card.nHeadID8);
		char_name.text = DataConvert.BytesToStr(now_unit.szName3);
		position_label.text = AllianceManager.GetStrFromPositionID(now_unit.nPosition4);
		char_level_label.text = player_card.nLevel5.ToString();
		glory_label.text = player_card.nDevelopment14.ToString();
		contribution_label.text = now_unit.nTotalDevelopment6.ToString();
		char_rank.text = player_card.nLevelRank18.ToString();
		if(player_card.nVip6 >0)
		{
			//vip_icon.gameObject.SetActiveRecursively(true);	
			vip_icon.spriteName = "vip_"+player_card.nVip6;
		}
		else 
		{
			vip_icon.spriteName = "transparent";
		}
		if(player_card.bVipDisplay24 != 1)
		{
			vip_icon.spriteName = "transparent";
		}
		
	}
	//刷新职位 
	void RefreshPosition(uint position)
	{
		position_label.text = AllianceManager.GetStrFromPositionID(position);
	}
	//开除成员 
	void FireMember()
	{
		string str = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.SURE_FIRE_THIS_MEMBER);
		PopConfirmWin.instance.ShowPopWin(str,SureFireMember);
	}
	void SureFireMember(GameObject obj)
	{
		AllianceManager.FireAllianceMember(now_unit.nAccountID1);
	}
	//授权成员  
	void OpenAuthorizeWin()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("AuthorizeWin");
		if (obj != null)
		{
			obj.SendMessage("RevealPanel",now_unit.nAccountID1);
			
			if(AllianceManager.my_alliance_info.MyPost == (uint)ALLIANCE_POSITION.alliance_position_vice_leader)
			{
				obj.SendMessage("Deputyleader", null);
			}
		}
	}
	//发邮件 
	void MailMember()
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
	//加好友 
	void FriendMember()
	{
		FriendManager.ReqAddFriend(now_unit.nAccountID1);
	}
	//增援 
	void Reinforce()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("ReinforceSoldierWin");
		if (obj != null)
		{
			obj.SendMessage("RevealPanel",now_unit.nAccountID1);
		}
	}
}
