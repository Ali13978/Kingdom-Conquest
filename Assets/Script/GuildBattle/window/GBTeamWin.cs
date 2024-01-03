using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using TinyWar;
using STCCMD;
using CMNCMD;

public class GBTeamWin : MonoBehaviour {
    public UIGrid MemberList;
    public UILabel CmdBtn1Label;
    public UILabel CmdBtn2Label;
    public UILabel CmdBtn3Label;
    public UIImageButton CmdBtn1;
    public UIImageButton CmdBtn2;
    public UIImageButton CmdBtn3;
    public GameObject BottomPanel;
    public GameObject MemberTemplate;

    private GBTeamListItem _teamInfo;
    private ulong _accountID;
    private bool _isMarch;

	void Awake () {
        U3dCmn.ScaleScreen(this.transform);
        CmdBtn1.gameObject.SetActiveRecursively(false);
        CmdBtn2.gameObject.SetActiveRecursively(false);
        CmdBtn3.gameObject.SetActiveRecursively(false);

        _accountID = CommonData.player_online_info.AccountID;
        GBTeamManager.Instance.OnQuitTeamComplete += this.OnQuitTeamComplete;
        GBTeamManager.Instance.OnDisbandTeamCompelte += this.OnDisbandTeam;
        GBTeamManager.Instance.OnMemeberListGet += this.OnMemberListGet;
        GBTeamManager.Instance.OnJoinTeamComplete += this.OnJoinTeamComplete;
        GBTeamManager.Instance.OnKickMemberComplete += this.OnKickMemberComplete;
        GBTeamManager.Instance.OnMarchTeam += this.OnMarchTeam;
        GBTeamManager.Instance.OnOtherMemberDeploy += this.OnOtherMemberDeploy;
	}

    /// <summary>
    /// 关闭窗体
    /// </summary>
    void DismissPanel()
    {
        GBTeamManager.Instance.OnMemeberListGet -= this.OnMemberListGet;
        GBTeamManager.Instance.OnQuitTeamComplete -= this.OnQuitTeamComplete;
        GBTeamManager.Instance.OnDisbandTeamCompelte -= this.OnDisbandTeam;
        GBTeamManager.Instance.OnJoinTeamComplete -= this.OnJoinTeamComplete;
        GBTeamManager.Instance.OnKickMemberComplete -= this.OnKickMemberComplete;
        GBTeamManager.Instance.OnMarchTeam -= this.OnMarchTeam;
        GBTeamManager.Instance.OnOtherMemberDeploy -= this.OnOtherMemberDeploy;
        GameObject obj = U3dCmn.GetObjFromPrefab("AllianceWin");
        if (!this._isMarch)
        {
            AllianceWin win = obj.GetComponent<AllianceWin>();
            if (win != null)
            {
                win.SendMessage("OpenGuildWarPanel");
            }
        }
        else { //出征界面销毁联盟界面 
            obj.SendMessage("DismissPanel");
        }
        Destroy(gameObject);
    }

    void ClearList() {
        GBTeamMemberItemUI[] ts = MemberList.GetComponentsInChildren<GBTeamMemberItemUI>();
        for (int i = 0; i < ts.Length; ++i)
        {
            ts[i].gameObject.SetActiveRecursively(false);
            Destroy(ts[i].gameObject);
        }
    }

    void _SetBtnAndLabel() {
        UIButtonMessage msg1 = CmdBtn1.GetComponent<UIButtonMessage>();
        UIButtonMessage msg2 = CmdBtn2.GetComponent<UIButtonMessage>();
        UIButtonMessage msg3 = CmdBtn3.GetComponent<UIButtonMessage>();

        //如果是我创建的队伍就可以出征
        if (_accountID == _teamInfo.LordID)
        {
            CmdBtn1Label.text = "Deploy";
            msg1.functionName = "DeployTeam";

            CmdBtn2Label.text = "Disband";
            msg2.functionName = "DisbandTeam";

            CmdBtn3Label.text = "March";
            msg3.functionName = "MarchTeam";

            CmdBtn1.gameObject.SetActiveRecursively(true);
            CmdBtn2.gameObject.SetActiveRecursively(true);
            CmdBtn3.gameObject.SetActiveRecursively(true);

            BottomPanel.transform.localPosition = new Vector3(85, BottomPanel.transform.localPosition.y);
        }
        else {//如果是别人创建的队伍，先看是否已经加入，加入的话则显示配兵和退出按钮，否则只显示加入按钮
            if (GBStatusManager.Instance.MyStatus.nInstanceID == this._teamInfo.TeamID)
            {
                CmdBtn1Label.text = "Deploy";
                msg1.functionName = "DeployTeam";

                CmdBtn2Label.text = "Quit";
                msg2.functionName = "QuitTeam";

                CmdBtn1.gameObject.SetActiveRecursively(true);
                CmdBtn2.gameObject.SetActiveRecursively(true);

                BottomPanel.transform.localPosition = new Vector3(120, BottomPanel.transform.localPosition.y);
            }
            else { 
                CmdBtn1Label.text = "Join";
                msg1.functionName = "JoinTeam";

                CmdBtn1.gameObject.SetActiveRecursively(true);

                BottomPanel.transform.localPosition = new Vector3(160, BottomPanel.transform.localPosition.y);
            }
        }
    }
    /// <summary>
    /// 初始化队伍
    /// </summary>
    /// <param name="team"></param>
    public void InitTeamInfo(GBTeamListItem team) {
        this._teamInfo = team;
        this._SetBtnAndLabel();
        GBTeamManager.Instance.RequestTeamMember(team.TeamID);
    }

    /// <summary>
    /// 获取到组内队员数据的回调
    /// </summary>
    /// <param name="list"></param>
    public void OnMemberListGet(List<GBTeamMemberItem> list)
    {
        this.ClearList();
        for (int i = 0; i < list.Count; ++i)
        {
            GameObject item = NGUITools.AddChild(MemberList.gameObject, MemberTemplate);
            item.name = string.Format("member{0:D2}", i);
            GBTeamMemberItemUI lordInfo = item.GetComponent<GBTeamMemberItemUI>();
            lordInfo.TeamInfo = this._teamInfo;
            lordInfo.SetItemInfo(list[i]);
            lordInfo.TeamWin = this;
        }
        MemberList.Reposition();
    }

    /// <summary>
    /// 配置队伍，弹出配置队伍面板
    /// </summary>
    public void DeployTeam() {
        GameObject obj = U3dCmn.GetObjFromPrefab(GBWindowPath.GBLordTroopWin);
        if (obj != null)
        {
            GBLordTroopWin win = obj.GetComponent<GBLordTroopWin>();
            win.LoadLordTroop();
            win.TeamWin = this;
        }
    }
    /// <summary>
    /// 组内其他队员配兵完毕时回调 
    /// </summary>
    private void OnOtherMemberDeploy() {
        this.InitTeamInfo(GBTeamManager.Instance.GetTeamData());
    }

    /// <summary>
    /// 离开队伍
    /// </summary>
    public void QuitTeam() {
        GBTeamManager.Instance.QuitTeam(this._teamInfo.TeamID);
    }

    private void OnQuitTeamComplete(STC_GAMECMD_ALLI_INSTANCE_EXIT_T.enum_rst result) {
        switch (result)
        {
            case STC_GAMECMD_ALLI_INSTANCE_EXIT_T.enum_rst.RST_NO_INSTANCE://副本不存在
                U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_TEAM_NOT_EXIST));
                break;
        }
        this.DismissPanel();
    }

    public void JoinTeam() {
        if (this._teamInfo.JoinedCount == GBTeamListItem.FullCount) {
            U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_TEAM_FULL));
            return;
        }
        ulong teamID = this._teamInfo.TeamID;
        GBTeamManager.Instance.JoinTeam(teamID);
    }

    private void OnJoinTeamComplete(STC_GAMECMD_ALLI_INSTANCE_JOIN_T.enum_rst result)
    {
        if (result == STC_GAMECMD_ALLI_INSTANCE_JOIN_T.enum_rst.RST_OK)
        {
            GBTeamListItem team = GBTeamManager.Instance.GetTeamData(this._teamInfo.TeamID);
            this.InitTeamInfo(team);
        }
        else
        {
            string warnText = "";
            switch (result)
            {
                case STC_GAMECMD_ALLI_INSTANCE_JOIN_T.enum_rst.RST_NO_ALLI_INSTANCE://队伍不存在
                case STC_GAMECMD_ALLI_INSTANCE_JOIN_T.enum_rst.RST_NO_INSTANCE:
                case STC_GAMECMD_ALLI_INSTANCE_JOIN_T.enum_rst.RST_INSTANCE_START:
                    warnText = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_TEAM_NOT_EXIST);
                    break;
                case STC_GAMECMD_ALLI_INSTANCE_JOIN_T.enum_rst.RST_ROOM_FULL://队伍已满 
                case STC_GAMECMD_ALLI_INSTANCE_JOIN_T.enum_rst.RST_TEAM_FULL:
                    warnText = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_TEAM_FULL);
                    break;
                case STC_GAMECMD_ALLI_INSTANCE_JOIN_T.enum_rst.RST_EXISTS_INSTANCE://有队伍尚未退出 
                    warnText = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_TEAM_NOT_EXIT);
                    break;
                case STC_GAMECMD_ALLI_INSTANCE_JOIN_T.enum_rst.RST_NO_TIMES_LEFT: //没有可用次数了 
                    warnText = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_TIMES_USEDUP);
                    break;
            }
            if (warnText != "")
            {
                U3dCmn.ShowWarnWindow(warnText);
            }
        }
    }

    /// <summary>
    /// t出队员
    /// </summary>
    /// <param name="lordID"></param>
    public void KickMember(ulong lordID) {
        GBTeamManager.Instance.KickMember(this._teamInfo.TeamID, lordID);
    }

    private void OnKickMemberComplete(STC_GAMECMD_ALLI_INSTANCE_KICK_T.enum_rst result) {
        if (result == STC_GAMECMD_ALLI_INSTANCE_KICK_T.enum_rst.RST_OK)
        {
            GBTeamListItem team = GBTeamManager.Instance.GetTeamData();
            this.InitTeamInfo(team);
        }
    }

    /// <summary>
    /// 解散队伍
    /// </summary>
    public void DisbandTeam() {
        GBTeamManager.Instance.DisbandTeam(this._teamInfo.TeamID);
    }

    private void OnDisbandTeam(STC_GAMECMD_ALLI_INSTANCE_DESTROY_T.enum_rst result) {
        if (result == STC_GAMECMD_ALLI_INSTANCE_DESTROY_T.enum_rst.RST_OK)
        {
            this.DismissPanel();
        }
        else { 
            
        }
    }

    /// <summary>
    /// 出征界面
    /// </summary>
    public void MarchTeam() {
        GBTeamManager.Instance.MarchTeam();
        LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_START);
    }

    private void OnMarchTeam(STC_GAMECMD_ALLI_INSTANCE_START_T.enum_rst result) {
        if (result == STC_GAMECMD_ALLI_INSTANCE_START_T.enum_rst.RST_OK)
        {
            U3dCmn.GetObjFromPrefab(GBWindowPath.GBLevelWin);
            GameObject embassyWin = U3dCmn.GetObjFromPrefab("DashiGuanWin");
            if (embassyWin != null)
            {
                embassyWin.SendMessage("DismissPanel");
            }
            this._isMarch = true;
            this.DismissPanel();
        }
        else {
            string warnText = "";   
            switch (result) { 
                case STC_GAMECMD_ALLI_INSTANCE_START_T.enum_rst.RST_NOT_READY://有人尚未准备成功 
                    warnText = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_TEAM_NOT_READY);
                    break;
            }
            if (warnText != "") {
                U3dCmn.ShowWarnWindow(warnText);
            }
        }
    }

    void CreateTeam() {
        GameObject infowin = U3dCmn.GetObjFromPrefab(GBWindowPath.GBLordTroopWin);
        if (infowin != null)
        {
            GBLordTroopWin action = infowin.GetComponent<GBLordTroopWin>();
            action.LoadLordTroop();
        }
    }
}