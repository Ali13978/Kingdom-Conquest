using UnityEngine;
using System.Collections;
using CMNCMD;
public class AuthorizeWin : MonoBehaviour {
	//消息窗口positon TWEEN     
	public TweenPosition tween_position;
	public UICheckbox viceleader_check;
	public UICheckbox manager_check;
	public UICheckbox member_check;
	ulong 	now_member_id;
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
	void RevealPanel(ulong member_id)
	{
		now_member_id = member_id;
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//授权成员  
	void AuthorizeMember()
	{
		if(viceleader_check.isChecked)
		{
			AllianceManager.ReqAuthorizeMember(now_member_id,(uint)ALLIANCE_POSITION.alliance_position_vice_leader);
		}
		else if(manager_check.isChecked)
		{
			AllianceManager.ReqAuthorizeMember(now_member_id,(uint)ALLIANCE_POSITION.alliance_position_manager);
		}
		else if(member_check.isChecked)
		{
			AllianceManager.ReqAuthorizeMember(now_member_id,(uint)ALLIANCE_POSITION.alliance_position_member);
		}
	}
	//管事界面 ...
	void Deputyleader()
	{
		NGUITools.SetActive(viceleader_check.gameObject,false);
		manager_check.startsChecked = true;
	}
}
