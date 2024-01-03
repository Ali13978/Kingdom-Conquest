using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CMNCMD;
using STCCMD;

public class GBKickWin : MonoBehaviour {
    public GBKickLordUI Template;
    public GameObject List;
    public UISprite KickBtnBackground;
    public UIButtonMessage KickBtnMsg;
    public List<GBTeamMemberItem> MemberList;
    public List<GBKickLordUI> lordPanels;
    public List<ulong> SelectedMemberIDList {
        get {
            List<ulong> result = new List<ulong>();
            for (int i = 0; i < this.MemberList.Count; ++i) {
                if (this.MemberList[i].Selected) {
                    result.Add(this.MemberList[i].LordID);
                }
            }
            return result;
        }
    }
    void Awake() {
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			transform.localScale = CommonData.iphonescale;
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			transform.localScale = CommonData.ipadscale;
		}
        lordPanels = new List<GBKickLordUI>();
        GBTeamManager.Instance.OnKickMemberComplete += this.OnKickMemberComplete;
    }

    void ClearLordUIList() {
        for (int i = 0; i < this.lordPanels.Count; ++i) {
            this.lordPanels[i].SetLordInfo(null);
        }
    }

    void SetState() { 
        if (OnlyMe())
        {
            this.KickBtnBackground.spriteName = "button1_disable";
            this.KickBtnMsg.functionName = "";
        }
        for (int i = 0; i < MemberList.Count; ++i) {
            lordPanels[i].SetLordInfo(MemberList[i]);
        }
    }

	void Start () {
        for (int i = 0; i < 5; ++i) {
            GameObject obj = NGUITools.AddChild(List, Template.gameObject);
            obj.name = "lord" + i + 1;
            obj.transform.localPosition = new Vector3(i * 82, 0, -1);
            GBKickLordUI ui = obj.GetComponent<GBKickLordUI>();
            ui.SetLordInfo(null);
            lordPanels.Add(ui);
        }
        this.SetState();
	}

    void DismissPanel() {
        GBTeamManager.Instance.OnKickMemberComplete -= this.OnKickMemberComplete;
        Destroy(gameObject);
    }

    void KickMember() {
        List<ulong> ids = this.SelectedMemberIDList;
        if (ids.Count == 0)
        {
            U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_TEAM_NOBODY_KICK));
        }
        else {
            for (int i = 0; i < ids.Count; ++i) {
                GBTeamManager.Instance.KickMember(GBStatusManager.Instance.MyStatus.nInstanceID, ids[i]);
            }
        }
    }

    void OnKickMemberComplete(STC_GAMECMD_ALLI_INSTANCE_KICK_T.enum_rst result) {
        if (result == STC_GAMECMD_ALLI_INSTANCE_KICK_T.enum_rst.RST_OK) {
            this.ClearLordUIList();
            this.SetState();
        }
		DismissPanel();
    }

    bool OnlyMe() {
        if (this.MemberList.Count == 1) {
            if (this.MemberList[0].LordID == GBStatusManager.Instance.MyStatus.nCreatorID) {
                return true;
            }
        }
        return false;
    }
}
