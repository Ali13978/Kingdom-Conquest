using UnityEngine;
using System.Collections;
using STCCMD;
using CTSCMD;
using CMNCMD;
public class ApplyItem : MonoBehaviour {
	public UISprite	head_icon;
	public UILabel	char_name;
	public UILabel	level;
	public UIImageButton agree_btn;
	public UIImageButton refuse_btn;
	
	AllianceJoinEvent now_unit;
	// Use this for initialization
	void Start () {
	
	}
	
	void InitialData(AllianceJoinEvent unit)
	{
		now_unit = unit;
		if(unit.nAccountID1 == 0)
		{
			head_icon.gameObject.SetActiveRecursively(false);
			char_name.gameObject.SetActiveRecursively(false);
			level.gameObject.SetActiveRecursively(false);
			refuse_btn.gameObject.SetActiveRecursively(false);
			agree_btn.gameObject.SetActiveRecursively(false);
		}
		else 
		{
			char_name.text =  DataConvert.BytesToStr(unit.szName3);
			level.text = "Lv"+unit.nLevel4;
			head_icon.spriteName = U3dCmn.GetCharIconName((int)unit.nHeadID5);
			head_icon.gameObject.SetActiveRecursively(true);
			char_name.gameObject.SetActiveRecursively(true);
			level.gameObject.SetActiveRecursively(true);
			refuse_btn.gameObject.SetActiveRecursively(true);
			agree_btn.gameObject.SetActiveRecursively(true);
			
		}
	}
	//同意其他玩家加入联盟的申请 
	void AgreeJoinApply()
	{
		CTS_GAMECMD_OPERATE_ACCEPT_ALLI_MEMBER_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_ACCEPT_ALLI_MEMBER;
		req.nMemberID3 = now_unit.nAccountID1;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_ACCEPT_ALLI_MEMBER_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ACCEPT_ALLI_MEMBER);
	}
	//拒绝其他玩家加入联盟的申请 
	void RefuseJoinApply()
	{
		CTS_GAMECMD_OPERATE_REFUSE_JOIN_ALLI_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_REFUSE_JOIN_ALLI;
		req.nApplicantID3 = now_unit.nAccountID1;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_REFUSE_JOIN_ALLI_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_REFUSE_JOIN_ALLI);
	}
	//查看玩家基本信息 
	void ShowPlayerCardInfo()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("PlayerCardWin");
		if (obj != null)
		{
			obj.SendMessage("RevealPanel",now_unit.nAccountID1);
		}
	}
	
}
