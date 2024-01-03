using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CMNCMD;
using CTSCMD;
using STCCMD;
using TinyWar;

/// <summary>
/// 队伍操作管理
/// </summary>
public class GBTeamManager : MonoBehaviour
{
    #region singleton
    static GBTeamManager _instance;
    private GBTeamManager() {
        this._teamList = new List<GBTeamListItem>();
        this._teamMemberList = new List<GBTeamMemberItem>();
        this._status = GBStatusManager.Instance.MyStatus;

        if (_status.nInstanceStatus == (uint)INSTANCE_STATUS.instance_status_fight)
        {
            this.RequestSimpleCombatLog();
        }
        else
        {
            if (_status.nLastResult == 1)
            {
                _status.nCurLevel++;
            }
        }
    }
    static GBTeamManager()
    {
        _instance = new GBTeamManager();
    }
    public static GBTeamManager Instance
    {
        get { return _instance; }
    }
    #endregion

    #region Event
    /// <summary>
    /// 获取队伍列表
    /// </summary>
    public event GetTeamListHandler OnTeamListGet;
    /// <summary>
    /// 创建队伍事件
    /// </summary>
    public event CreateTeamHandler OnCreateTeamComplete;
    /// <summary>
    /// 解散队伍事件
    /// </summary>
    public event DisbandTeamHandler OnDisbandTeamCompelte;
    /// <summary>
    /// 加入队伍完毕
    /// </summary>
    public event JoinTeamHandler OnJoinTeamComplete;
    /// <summary>
    /// 退出队伍完毕
    /// </summary>
    public event QuitTeamHandler OnQuitTeamComplete;
    /// <summary>
    /// 获取某个君主英雄列表
    /// </summary>
    public event GetHeroListHandler OnHeroListGet;
    /// <summary>
    /// 获取某个队伍的所有成员信息
    /// </summary>
    public event GetMemberListHandler OnMemeberListGet;
    /// <summary>
    /// t人
    /// </summary>
    public event KickMemberHander OnKickMemberComplete;
    /// <summary>
    /// 配置将领结束
    /// </summary>
    public event DeployHeroHandler OnDeployHero;
    /// <summary>
    /// 准备完毕
    /// </summary>
    public event GetReadyHandler OnReady;
    /// <summary>
    /// 队伍出征
    /// </summary>
    public event MarchTeamHandler OnMarchTeam;
    /// <summary>
    /// 其他队员配置完毕 
    /// </summary>
    public event OnOtherMemberDeployHandler OnOtherMemberDeploy;
    /// <summary>
    /// 每场战斗开始   
    /// </summary>
    public event OnCombatStartHandler OnCombatStart;
    /// <summary>
    /// 每场战斗结束
    /// </summary>
    public event OnCombatEndHandler OnCombatEnd;
    /// <summary>
    /// 简单战报获取
    /// </summary>
    public event OnSimpleCombatLogGetHandler OnSimpleCombatLogGet;
    /// <summary>
    /// 复杂战报获取 
    /// </summary>
    public event OnComplexCombatLogGetHandler OnComplexCombatLogGet;
    /// <summary>
    /// 停止自动战斗
    /// </summary>
    public event OnAutoCombatStopHandler OnAutoCombatStop;
    /// <summary>
    /// 奖品
    /// </summary>
    public event OnRewardGet OnRewardGet;
    #endregion

    #region Fields and Properties
    /// <summary>
    /// 所有队伍列表 
    /// </summary>
    public List<GBTeamListItem> TeamList {
        get { return this._teamList; }
    }
    private List<GBTeamListItem> _teamList;
    
    /// <summary>
    /// 某队伍中所有队员列表 
    /// </summary>
    public List<GBTeamMemberItem> TeamMemberList {
        get { return this._teamMemberList; }
    }
    private List<GBTeamMemberItem> _teamMemberList;
    /// <summary>
    /// 上次战斗结束时间
    /// </summary>
    public DateTime LastEndTime;
    public bool LastWin;
    private GBTeamListItem _lastAssociateTeam;                                          //上一个操作关联的队伍信息
    private GBTeamMemberItem _lastAssociateMember;                                      //上一个操作关联的成员信息
    private List<GBLordTroopHero> _lastAssociateHeros;                                  //上一个操作关联的英雄列表  
    private STC_GAMECMD_ALLI_INSTANCE_GET_CHAR_DATA_T.GBCharData[] _charData;           //stc返回的临时成员信息，用来做转换
    private MY_GUILDBATTLE_STATUS _status;
    private int _gid = 0;
    public const int LevelGroup = 10;
    #endregion

    /// <summary>
    /// 获取队伍列表
    /// </summary>
    public void RequestTeamList()
    {
        CTS_GAMECMD_ALLI_INSTANCE_GET_LIST_T req;
        req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ALLI_INSTANCE_GET_LIST;
        TcpMsger.SendLogicData<CTS_GAMECMD_ALLI_INSTANCE_GET_LIST_T>(req);
    }
	/// <summary>
    /// 获取队伍列表返回
	/// </summary>
	/// <param name="buff"></param>
  	public void RequestTeamListRst(byte[] buff)
	{
        STC_GAMECMD_ALLI_INSTANCE_GET_LIST_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_GET_LIST_T>(buff);
        if (msg.nRst1 == (int)STC_GAMECMD_ALLI_INSTANCE_GET_LIST_T.enum_est.RST_OK) {
            if (this._teamList != null)
            {
                this._teamList.Clear();
            }
            int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_ALLI_INSTANCE_GET_LIST_T>();
            int data_len = buff.Length - head_len;
            byte[] data_buff = new byte[data_len];
            System.Array.Copy(buff, head_len, data_buff, 0, data_len);
            STC_GAMECMD_ALLI_INSTANCE_GET_LIST_T.GBTeamData[] allRoom = DataConvert.ByteToStructArray<STC_GAMECMD_ALLI_INSTANCE_GET_LIST_T.GBTeamData>(data_buff, msg.nNum2);
            if (allRoom.Length > 0) {
                for (int i = 0; i < allRoom.Length; ++i) {
                    this._teamList.Add(allRoom[i]);
                }
            }
            if (this.OnTeamListGet != null) { 
                this.OnTeamListGet(this._teamList);
            }
        }
	}


    /// <summary>
    /// 获取某个队伍中的君主信息
    /// </summary>
    /// <param name="teamID"></param>
    public void RequestTeamMember(ulong teamID) {
        CTS_GAMECMD_ALLI_INSTANCE_GET_CHAR_DATA_T req;
        req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ALLI_INSTANCE_GET_CHAR_DATA;
        req.nInstanceID3 = teamID;
        TcpMsger.SendLogicData<CTS_GAMECMD_ALLI_INSTANCE_GET_CHAR_DATA_T>(req);
    }
    /// <summary>
    /// 获取队伍中君主信息回调
    /// </summary>
    /// <param name="buffer"></param>
    public void RequestTeamMemberRst(byte[] buffer)
    {
        STC_GAMECMD_ALLI_INSTANCE_GET_CHAR_DATA_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_GET_CHAR_DATA_T>(buffer);
        if (msg.nRst1 == (int)STC_GAMECMD_ALLI_INSTANCE_GET_CHAR_DATA_T.enum_rst.RST_OK) {
            List<GBTeamMemberItem> list = new List<GBTeamMemberItem>();
            int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_ALLI_INSTANCE_GET_CHAR_DATA_T>();
            int data_len = buffer.Length - head_len;
            int struct_Len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_ALLI_INSTANCE_GET_CHAR_DATA_T.GBCharData>() * msg.nNum3;
            if (struct_Len + data_len < buffer.Length) {
                return;
            }
            byte[] data_buff = new byte[data_len];
            System.Array.Copy(buffer, head_len, data_buff, 0, data_len);
            STC_GAMECMD_ALLI_INSTANCE_GET_CHAR_DATA_T.GBCharData[] charData = DataConvert.ByteToStructArray<STC_GAMECMD_ALLI_INSTANCE_GET_CHAR_DATA_T.GBCharData>(data_buff, msg.nNum3);
            this._charData = charData;
            if (charData.Length > 0) {
                this.RequestHeroList(msg.nInstanceID2);
            }
        }
    }

    /// <summary>
    /// 获取君主的将领信息
    /// </summary>
    public void RequestHeroList(ulong instanceID) {
        CTS_GAMECMD_ALLI_INSTANCE_GET_HERO_DATA_T req;
        req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ALLI_INSTANCE_GET_HERO_DATA;
        req.nInstanceID3 = instanceID;
        TcpMsger.SendLogicData<CTS_GAMECMD_ALLI_INSTANCE_GET_HERO_DATA_T>(req);
    }
    /// <summary>
    /// 获取君主的将领信息回调
    /// </summary>
    /// <param name="buffer"></param>
    public void RequestHeroListRst(byte[] buffer) {
        STC_GAMECMD_ALLI_INSTANCE_GET_HERO_DATA_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_GET_HERO_DATA_T>(buffer);
        if (msg.nRst1 == (int)STC_GAMECMD_ALLI_INSTANCE_GET_HERO_DATA_T.enum_est.RST_OK) {
            int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_ALLI_INSTANCE_GET_HERO_DATA_T>();
            int data_len = buffer.Length - head_len;
            byte[] data_buff = new byte[data_len];
            System.Array.Copy(buffer, head_len, data_buff, 0, data_len);
            STC_GAMECMD_ALLI_INSTANCE_GET_HERO_DATA_T.GBHeroData[] heroData = DataConvert.ByteToStructArray<STC_GAMECMD_ALLI_INSTANCE_GET_HERO_DATA_T.GBHeroData>(data_buff, msg.nNum3);
            this._teamMemberList = this._CombineLordAndHeroInfo(this._charData, heroData);
            if (this.OnMemeberListGet != null) { 
                this.OnMemeberListGet(this._teamMemberList);
            }
        }
    }

    /// <summary>
    /// 创建队伍
    /// </summary>
    public void CreateTeam() {
        CTS_GAMECMD_ALLI_INSTANCE_CREATE_T req;
        req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ALLI_INSTANCE_CREATE;
        TcpMsger.SendLogicData<CTS_GAMECMD_ALLI_INSTANCE_CREATE_T>(req);
    }
	/// <summary>
    /// 创建队伍回调
	/// </summary>
	/// <param name="buff"></param>
    public void CreateTeamRst(byte[] buff)
	{
        STC_GAMECMD_ALLI_INSTANCE_CREATE_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_CREATE_T>(buff);
        if (msg.nRst1 == (int)STC_GAMECMD_ALLI_INSTANCE_CREATE_T.enum_rst.RST_OK) {
            GBStatusManager.Instance.MyStatus.nInstanceID = msg.nInstanceID2;
            GBStatusManager.Instance.MyStatus.nCreatorID = CommonData.player_online_info.AccountID;
            GBStatusManager.Instance.MyStatus.nInstanceStatus = (int)INSTANCE_STATUS.instance_status_normal;
            GBStatusManager.Instance.MyStatus.nCharStatus = (int)PLAYER_STATUS.instance_player_status_normal;

            GBTeamListItem team = new GBTeamListItem { 
                JoinedCount = 1,
                LordID = CommonData.player_online_info.AccountID,
                LordLevel = CommonData.player_online_info.Level,
                LordName = CommonData.player_online_info.CharName,
                LordModelID = CommonData.player_online_info.nHeadID,
                TeamID = msg.nInstanceID2,
                Members = new List<GBTeamMemberItem>()
            };
            GBTeamMemberItem member = new GBTeamMemberItem { 
                Heros = new List<GBLordTroopHero>(),
                LordForce = 0,
                LordID = team.LordID,
                LordModelID = team.LordModelID,
                LordName = team.LordName,
                IsLeader = true,
                Selected = false
            };
            team.Members.Add(member);
            this._teamList.Add(team);
            if (this._teamMemberList == null) {
                this._teamMemberList = new List<GBTeamMemberItem>();
            }
            this._teamMemberList.Add(member);
        }

        if (this.OnCreateTeamComplete != null) { 
            this.OnCreateTeamComplete(msg);
        }
	}

    /// <summary>
    /// 加入队伍  
    /// </summary>
    /// <param name="teamID"></param>
    public void JoinTeam(ulong teamID) {
        CTS_GAMECMD_ALLI_INSTANCE_JOIN_T req;
        req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ALLI_INSTANCE_JOIN;
        req.nInstanceID3 = teamID;
        this._lastAssociateTeam = this.GetTeamData(teamID);
        TcpMsger.SendLogicData<CTS_GAMECMD_ALLI_INSTANCE_JOIN_T>(req);
    }
    /// <summary>
    /// 加入队伍回调 
    /// </summary>
    /// <param name="buffer"></param>
    public void JoinTeamRst(byte[] buffer) {
        STC_GAMECMD_ALLI_INSTANCE_JOIN_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_JOIN_T>(buffer);
        STC_GAMECMD_ALLI_INSTANCE_JOIN_T.enum_rst val = (STC_GAMECMD_ALLI_INSTANCE_JOIN_T.enum_rst)msg.nRst1;
        if (val == STC_GAMECMD_ALLI_INSTANCE_JOIN_T.enum_rst.RST_OK)
        {
            this._status.nInstanceID = _lastAssociateTeam.TeamID;
            //如果加入队伍成功，就把我的信息添加到队伍列表中
            GBTeamMemberItem member = new GBTeamMemberItem { 
                Heros = new List<GBLordTroopHero>(),
                LordForce = 0,
                LordID = CommonData.player_online_info.AccountID,
                LordModelID = CommonData.player_online_info.nHeadID,
                LordName = CommonData.player_online_info.CharName,
                IsLeader = false,
                Selected = false
            };
            this._teamMemberList.Add(member);
            GBTeamListItem team = this.GetTeamData(_lastAssociateTeam.TeamID);
            team.JoinedCount++;
            this._status.nCreatorID = team.LordID;
            this._status.nInstanceStatus = (int)INSTANCE_STATUS.instance_status_normal;
        }
        
        this._lastAssociateTeam = null;

        if (this.OnJoinTeamComplete != null) { 
            this.OnJoinTeamComplete(val);
        }
    }

    /// <summary>
    /// 解散队伍
    /// </summary>
    /// <param name="teamID"></param>
    public void DisbandTeam(ulong teamID){
        CTS_GAMECMD_ALLI_INSTANCE_DESTROY_T req;
        req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ALLI_INSTANCE_DESTROY;
        req.nInstanceID3 = teamID;
        TcpMsger.SendLogicData<CTS_GAMECMD_ALLI_INSTANCE_DESTROY_T>(req);
    }
    /// <summary>
    /// 解散队伍返回
    /// </summary>
    /// <param name="buffer"></param>
    public void DisbandTeamRst(byte[] buffer) {
        STC_GAMECMD_ALLI_INSTANCE_DESTROY_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_DESTROY_T>(buffer);
        STC_GAMECMD_ALLI_INSTANCE_DESTROY_T.enum_rst val = (STC_GAMECMD_ALLI_INSTANCE_DESTROY_T.enum_rst)msg.nRst1;
        if (this.OnDisbandTeamCompelte != null) { 
            this.OnDisbandTeamCompelte(val);
        }
        if (val == STC_GAMECMD_ALLI_INSTANCE_DESTROY_T.enum_rst.RST_OK)
        {
            this._TeamDisbandLogic();
        }
    }

    /// <summary>
    /// 将某个君主t出队伍
    /// </summary>
    /// <param name="teamID"></param>
    /// <param name="lordID"></param>
    public void KickMember(ulong teamID, ulong lordID) {
        CTS_GAMECMD_ALLI_INSTANCE_KICK_T req;
        req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ALLI_INSTANCE_KICK;
        req.nInstanceID3 = teamID;
        req.nObjID4 = lordID;
        this._lastAssociateMember = this.GetTeamMemberData(lordID);
        TcpMsger.SendLogicData<CTS_GAMECMD_ALLI_INSTANCE_KICK_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_KICK);
    }
    /// <summary>
    /// t人返回
    /// </summary>
    /// <param name="buffer"></param>
    public void KickMemberRst(byte[] buffer) {
        STC_GAMECMD_ALLI_INSTANCE_KICK_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_KICK_T>(buffer);
        STC_GAMECMD_ALLI_INSTANCE_KICK_T.enum_rst val = (STC_GAMECMD_ALLI_INSTANCE_KICK_T.enum_rst)msg.nRst1;
        
        if (val == STC_GAMECMD_ALLI_INSTANCE_KICK_T.enum_rst.RST_OK)
        {
            GBTeamMemberItem delete = this.GetTeamMemberData(msg.nAccountID2);
            this._teamMemberList.Remove(delete);
            GBTeamListItem team = this.GetTeamData();
            if (team != null) { 
                team.JoinedCount--;
            }
            //如果队伍已经出征了，则把出征队伍中的相关将领删除
            if (_status.nInstanceStatus == (uint)INSTANCE_STATUS.instance_status_start) {
                this._RemoveTroopHeros(delete);
            }
        }
        this._lastAssociateMember = null;

        if (this.OnKickMemberComplete != null) { 
            this.OnKickMemberComplete(val);
        }
    }

    /// <summary>
    /// 退出队伍
    /// </summary>
    /// <param name="teamID"></param>
    public void QuitTeam(ulong teamID) {
        CTS_GAMECMD_ALLI_INSTANCE_EXIT_T req;
        req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ALLI_INSTANCE_EXIT;
        req.nInstanceID3 = teamID;
        TcpMsger.SendLogicData<CTS_GAMECMD_ALLI_INSTANCE_EXIT_T>(req);
        
    }
    public void QuitTeamRst(byte[] buffer) {
        STC_GAMECMD_ALLI_INSTANCE_EXIT_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_EXIT_T>(buffer);
        STC_GAMECMD_ALLI_INSTANCE_EXIT_T.enum_rst val = (STC_GAMECMD_ALLI_INSTANCE_EXIT_T.enum_rst)msg.nRst1;
        //把team中的信息做update
        if (val == STC_GAMECMD_ALLI_INSTANCE_EXIT_T.enum_rst.RST_OK) {
            this.UpdateHeroStatus(HeroState.NORMAL);//要放到前面，先把将领状态修改后再删除
            this._teamMemberList.Remove(this.GetTeamMemberData());
            GBTeamListItem team = this.GetTeamData();
            if (team != null) { //防止尚未获取teamlist时该值为null
                team.JoinedCount--;
            }
            _status.SetDefault();
        }
        if (this.OnQuitTeamComplete != null) { 
            this.OnQuitTeamComplete(val);
        }
    }

    /// <summary>
    /// 配置将领
    /// </summary>
    /// <param name="heroList"></param>
    public void DeployHero(List<GBLordTroopHero> heroList) {
        CTS_GAMECMD_ALLI_INSTANCE_CONFIG_HERO_T req = new CTS_GAMECMD_ALLI_INSTANCE_CONFIG_HERO_T();
        req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ALLI_INSTANCE_CONFIG_HERO;
        req.nInstanceID3 = GBStatusManager.Instance.MyStatus.nInstanceID;
        int totalCount = heroList.Count;
        if (totalCount > 0)
        {
            req.n1HeroID4 = heroList[0].HeroID;
        }
        if (totalCount > 1) {
            req.n2HeroID5 = heroList[1].HeroID;
        }
        if (totalCount > 2) {
            req.n3HeroID6 = heroList[2].HeroID;
        }
        if (totalCount > 3)
        {
            req.n4HeroID7 = heroList[3].HeroID;
        }
        if (totalCount > 4)
        {
            req.n5HeroID8 = heroList[4].HeroID;
        }
        this._lastAssociateHeros = heroList;
        TcpMsger.SendLogicData<CTS_GAMECMD_ALLI_INSTANCE_CONFIG_HERO_T>(req);
    }
    public void DeployHeroRst(byte[] buffer) {
        STC_GAMECMD_ALLI_INSTANCE_CONFIG_HERO_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_CONFIG_HERO_T>(buffer);
        STC_GAMECMD_ALLI_INSTANCE_CONFIG_HERO_T.enum_rst val = (STC_GAMECMD_ALLI_INSTANCE_CONFIG_HERO_T.enum_rst)msg.nRst1;
        if (val == STC_GAMECMD_ALLI_INSTANCE_CONFIG_HERO_T.enum_rst.RST_OK) {
            this.GetReady();
        }

        if (this.OnDeployHero != null) { 
            this.OnDeployHero(val);
        }
    }
    /// <summary>
    /// 保存所有的将领信息
    /// </summary>
    public void SaveHeroDeploy() {
        CTS_GAMECMD_ALLI_INSTANCE_SAVE_HERO_DEPLOY_T req;
        req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ALLI_INSTANCE_SAVE_HERO_DEPLOY;
        List<GBTeamMemberItem> members = this.TeamMemberList;
        List<HeroDeploy> deploys = new List<HeroDeploy>();
        for (int i = 0; i < members.Count; ++i)
        {
            List<GBLordTroopHero> heros = members[i].Heros;
            for (int j = 0; j < heros.Count; ++j) {
                HeroDeploy dep = new HeroDeploy { 
                    nCol1 = (uint)j + 1,
                    nRow2 = (uint)i + 1,
                    nHeroID3 = heros[j].HeroID
                };
                deploys.Add(dep);
            }
        }
        byte[] data = DataConvert.StructArrayToByte<HeroDeploy>(deploys.ToArray());
        req.nNum3 = data.Length;
        req.data4 = data;
        TcpMsger.SendLogicData<CTS_GAMECMD_ALLI_INSTANCE_SAVE_HERO_DEPLOY_T>(req);
    }
    public void SaveHeroDeployRst(byte[] buffer) {
        STC_GAMECMD_ALLI_INSTANCE_SAVE_HERO_DEPLOY_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_SAVE_HERO_DEPLOY_T>(buffer);
        STC_GAMECMD_ALLI_INSTANCE_SAVE_HERO_DEPLOY_T.enum_rst val = (STC_GAMECMD_ALLI_INSTANCE_SAVE_HERO_DEPLOY_T.enum_rst)msg.nRst1;
        if (val == STC_GAMECMD_ALLI_INSTANCE_SAVE_HERO_DEPLOY_T.enum_rst.RST_OK) {
            if (this.OnMarchTeam != null) {
                this.OnMarchTeam(STC_GAMECMD_ALLI_INSTANCE_START_T.enum_rst.RST_OK);
            }
        }
    }

    /// <summary>
    /// 出征准备完毕
    /// </summary>
    public void GetReady() {
        CTS_GAMECMD_ALLI_INSTANCE_READY_T req;
        req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ALLI_INSTANCE_READY;
        req.nInstanceID3 = GBStatusManager.Instance.MyStatus.nInstanceID;
        req.bReady4 = 1;
        TcpMsger.SendLogicData<CTS_GAMECMD_ALLI_INSTANCE_READY_T>(req);
    }
    /// <summary>
    /// 出征准备完毕回调
    /// </summary>
    /// <param name="buffer"></param>
    public void GetReadyRst(byte[] buffer) {
        STC_GAMECMD_ALLI_INSTANCE_READY_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_READY_T>(buffer);
        STC_GAMECMD_ALLI_INSTANCE_READY_T.enum_rst val = (STC_GAMECMD_ALLI_INSTANCE_READY_T.enum_rst)msg.nRst1;
        if (val == STC_GAMECMD_ALLI_INSTANCE_READY_T.enum_rst.RST_OK) {
            uint force = 0;
            for (int i = 0; i < this._lastAssociateHeros.Count; ++i)
            {
                if (this._lastAssociateHeros[i] != null)
                {
                    force += this._lastAssociateHeros[i].HeroForce;
                }
            }
            GBTeamMemberItem member = this.GetTeamMemberData();
            member.Heros = this._lastAssociateHeros;
            member.LordForce = force;
            
            this.UpdateMyTeamMemberData(member);
            this.UpdateHeroStatus(HeroState.COMBAT_INSTANCE_GUILD);
        }
        this._lastAssociateHeros = null;
        if (this.OnReady != null) { 
            this.OnReady(val);
        }
    }

    /// <summary>
    /// 队伍出征
    /// </summary>
    public void MarchTeam() {
        CTS_GAMECMD_ALLI_INSTANCE_START_T req;
        req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ALLI_INSTANCE_START;
        req.nInstanceID3 = GBStatusManager.Instance.MyStatus.nInstanceID;
        TcpMsger.SendLogicData<CTS_GAMECMD_ALLI_INSTANCE_START_T>(req);
    }
    public void MarchTeamRst(byte[] buffer) {
        STC_GAMECMD_ALLI_INSTANCE_START_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_START_T>(buffer);
        STC_GAMECMD_ALLI_INSTANCE_START_T.enum_rst val = (STC_GAMECMD_ALLI_INSTANCE_START_T.enum_rst)msg.nRst1;
        if (val == STC_GAMECMD_ALLI_INSTANCE_START_T.enum_rst.RST_OK) { 
            _status.nCurLevel = 1;
            _status.nRetryTimes = 0;
            _status.nInstanceStatus = (int)INSTANCE_STATUS.instance_status_start;
            _status.nCreatorID = CommonData.player_online_info.AccountID;
            this.SaveHeroDeploy();
        }
        if (this.OnMarchTeam != null)
        {
            this.OnMarchTeam(val);
        }
    }

    /// <summary>
    /// 每关开始战斗 
    /// </summary>
    public void StartCombat() {
        List<TroopData> list = ArmyDeployManager.instance.TroopDataList;
        List<CTS_HeroDeploy> heros = new List<CTS_HeroDeploy>();
        for (int i = 0; i < list.Count; ++i)
        {
            TroopData troop = list[i];
            if (troop.HeroID != 0)
            {
                heros.Add(new CTS_HeroDeploy
                {
                    nCol1 = troop.Column,
                    nRow2 = troop.Row,
                    nHeroID3 = troop.HeroID
                });
            }
        }
        byte[] troopData = DataConvert.StructArrayToByte<CTS_HeroDeploy>(heros.ToArray());

        BattleManager.CombatInfo combatInfo = new BattleManager.CombatInfo {
            AutoCombat = _status.bAutoCombat,
            AutoSupply = _status.bAutoSupply,
            CombatType = (uint)HeroState.COMBAT_INSTANCE_GUILD,
            ObjectID = _status.nInstanceID,
            StopLevel = _status.nStopLevel,
            HeroData = troopData
        };
        BattleManager.Instance.StartCombat(combatInfo);
    }
    public void StartCombatRst(byte[] buffer) {
        STC_GAMECMD_ALLI_INSTANCE_START_COMBAT_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_START_COMBAT_T>(buffer);
        STC_GAMECMD_ALLI_INSTANCE_START_COMBAT_T.enum_rst val = (STC_GAMECMD_ALLI_INSTANCE_START_COMBAT_T.enum_rst)msg.nRst1;
    }

    /// <summary>
    /// 获取奖励信息
    /// </summary>
    public void GetReward() {
        CTS_GAMECMD_ALLI_INSTANCE_GET_LOOT_T req;
        req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ALLI_INSTANCE_GET_LOOT;
        req.nInstanceID3 = _status.nInstanceID;
        TcpMsger.SendLogicData<CTS_GAMECMD_ALLI_INSTANCE_GET_LOOT_T>(req);
    }
    public void GetRewardRst(byte[] buffer) {
        STC_GAMECMD_ALLI_INSTANCE_GET_LOOT_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_GET_LOOT_T>(buffer);
        STC_GAMECMD_ALLI_INSTANCE_GET_LOOT_T.enum_rst val = (STC_GAMECMD_ALLI_INSTANCE_GET_LOOT_T.enum_rst)msg.nRst1;
        List<BaizInstanceLoot> list = new List<BaizInstanceLoot>();
        if (val == STC_GAMECMD_ALLI_INSTANCE_GET_LOOT_T.enum_rst.RST_OK) {
            int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_ALLI_INSTANCE_GET_LOOT_T>();
            STC_GAMECMD_ALLI_INSTANCE_GET_LOOT_T.LootUnit[] loots = DataConvert.ByteToStructArrayByOffset<STC_GAMECMD_ALLI_INSTANCE_GET_LOOT_T.LootUnit>(buffer, msg.nNum2, head_len);
            for (int i = 0; i < loots.Length; ++i) {
                BaizInstanceLoot loot = new BaizInstanceLoot{
                    nExcelID = (int)loots[i].nExcelID1
                };
                list.Add(loot);
            }
        }
        if (this.OnRewardGet != null) {
            this.OnRewardGet(val, list);
        }
    }

    /// <summary>
    /// 请求简单战报 
    /// </summary>
    public void RequestSimpleCombatLog() {
        BattleManager.Instance.RequestSimpleCombatLog((uint)HeroState.COMBAT_INSTANCE_GUILD, _status.nInstanceID);
    }
    /// <summary>
    /// 简单战报返回 
    /// </summary>
    /// <param name="buffer"></param>
    public void RequestSimpleCombatLogRst(byte[] buffer) {
        Debug.Log("sim combat" + this._gid++);
        STC_GAMECMD_ALLI_INSTANCE_GET_SIMPLE_COMBAT_LOG_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_GET_SIMPLE_COMBAT_LOG_T>(buffer);
        STC_GAMECMD_ALLI_INSTANCE_GET_SIMPLE_COMBAT_LOG_T.enum_rst val = (STC_GAMECMD_ALLI_INSTANCE_GET_SIMPLE_COMBAT_LOG_T.enum_rst)msg.nRst1;
        Debug.Log("combat result:" + msg.nCombatResult4);
        if (val == STC_GAMECMD_ALLI_INSTANCE_GET_SIMPLE_COMBAT_LOG_T.enum_rst.RST_OK) {
            //if (msg.nCombatResult4 == 2 || msg.nInstanceLevel5 == _status.nStopLevel)
            //{//如果战斗失败或者是最后一关了，就把自动战斗设置成0
            //    _status.bAutoCombat = 0;
            //}
            if (msg.nCombatResult4 != 1) {//如果战斗失败，重试次数+1 
                _status.nRetryTimes++;
            }
            _status.nCurLevel = msg.nInstanceLevel5;
            this.LastEndTime = DateTime.Now.AddSeconds(msg.nBackLeftTime2);
            this.LastWin = msg.nCombatResult4 == 1 ? true : false;
            _status.nInstanceStatus = (uint)INSTANCE_STATUS.instance_status_fight;
            if (this.OnSimpleCombatLogGet != null)
            {
                this.OnSimpleCombatLogGet(msg);
            }   
        }
    }

    /// <summary>
    /// 请求复杂战报
    /// </summary>
    public void RequestComplexCombatLog() {
        BattleManager.Instance.RequestComplexCombatLog((uint)HeroState.COMBAT_INSTANCE_GUILD, _status.nInstanceID);
    }
    /// <summary>
    /// 复杂战报推送
    /// </summary>
    /// <param name="buffer"></param>
    public void RequestComplexCombatLogRst(byte[] buffer)
    {
        STC_GAMECMD_ALLI_INSTANCE_GET_COMBAT_LOG_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_GET_COMBAT_LOG_T>(buffer);
        STC_GAMECMD_ALLI_INSTANCE_GET_COMBAT_LOG_T.enum_rst val = (STC_GAMECMD_ALLI_INSTANCE_GET_COMBAT_LOG_T.enum_rst)msg.nRst1;
        ComplexCombatLog clog = null;
        if (val == STC_GAMECMD_ALLI_INSTANCE_GET_COMBAT_LOG_T.enum_rst.RST_OK)
        {
            int buff_head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_ALLI_INSTANCE_GET_COMBAT_LOG_T>();
            byte[] logBuffer = new byte[buffer.Length - buff_head_len];
            Array.Copy(buffer, buff_head_len, logBuffer, 0, logBuffer.Length);
			
			int log_head_len = U3dCmn.GetSizeofSimpleStructure<AlliInstanceCombatLog>();
			if(logBuffer.Length<log_head_len)
			{
				return;
			}
            AlliInstanceCombatLog combat_log = DataConvert.ByteToStruct<AlliInstanceCombatLog>(logBuffer);
            
            int data_len = logBuffer.Length - log_head_len;
            byte[] data_buff = new byte[data_len];                                                              //战报中双方信息和掉落信息  
            Array.Copy(logBuffer, log_head_len, data_buff, 0, data_len);

            int attack_data_len = U3dCmn.GetSizeofSimpleStructure<HeroUnit>() * combat_log.nAttackNum6;
            byte[] attack_hero_buff = new byte[attack_data_len];                                                //攻方信息  
            Array.Copy(data_buff, 0, attack_hero_buff, 0, attack_data_len);
            HeroUnit[] attack_hero_units = DataConvert.ByteToStructArray<HeroUnit>(attack_hero_buff, combat_log.nAttackNum6);

            int defense_data_len = U3dCmn.GetSizeofSimpleStructure<HeroUnit>() * combat_log.nDefenseNum7;
            byte[] defense_hero_buff = new byte[defense_data_len];                                              //守方信息  
            Array.Copy(data_buff, attack_data_len, defense_hero_buff, 0, defense_data_len);
            HeroUnit[] defense_hero_units = DataConvert.ByteToStructArray<HeroUnit>(defense_hero_buff, combat_log.nDefenseNum7);

            int loot_data_len = U3dCmn.GetSizeofSimpleStructure<LootUnit>() * combat_log.nLootNum8;
            byte[] loot_buff = new byte[loot_data_len];                                                         //掉落信息  
            Array.Copy(data_buff, attack_data_len + defense_data_len, loot_buff, 0, loot_data_len);
            LootUnit[] loot_units = DataConvert.ByteToStructArray<LootUnit>(loot_buff, combat_log.nLootNum8);

            clog = new ComplexCombatLog();
            clog.CombatSeed = combat_log.nRandSeed5;
            clog.AttackEnhanceRatio = combat_log.fcounterrate9;
            //看我方是攻还是守
            if (combat_log.nAttackAccID3 == _status.nCreatorID) //我是攻方  
            {
                clog.AttackHeros = attack_hero_units;
                clog.DefenseHeros = defense_hero_units;
                if (combat_log.nCombatResult1 == (int)AlliInstanceCombatLog.enum_rst.combat_result_win)
                {
                    clog.IsWin = true;
                }
                else
                {
                    clog.IsWin = false;
                }
            }
            else
            {
                clog.AttackHeros = defense_hero_units;
                clog.DefenseHeros = attack_hero_units;
                if (combat_log.nCombatResult1 == (int)AlliInstanceCombatLog.enum_rst.combat_result_win)
                {
                    clog.IsWin = false;
                }
                else
                {
                    clog.IsWin = true;
                }
            }
            if (this.OnComplexCombatLogGet != null)
            {
                this.OnComplexCombatLogGet(val, clog, logBuffer);
            }
        }
    }

    /// <summary>
    /// 战斗结束服务器推送
    /// </summary>
    /// <param name="buffer"></param>
    public void EndCombat(byte[] buffer) {
        Debug.Log("end combat" + this._gid++);
        STC_GAMECMD_ALLI_INSTANCE_BACK msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_BACK>(buffer);
        //如果是我所在的副本结束就处理
        if (msg.nObjID3 == _status.nInstanceID && msg.nCombatType2 == (int)HeroState.COMBAT_INSTANCE_GUILD) {
            _status.nInstanceStatus = msg.bInstanceDestroyed1 == 1 ? (uint)INSTANCE_STATUS.instance_status_normal : (uint)INSTANCE_STATUS.instance_status_start;
            Debug.Log("end combat level:"+ msg.nInstanceLevel4);
            _status.nCurLevel = msg.nInstanceLevel4;
            bool lastAutoCombat = _status.bAutoCombat == 1 ? true : false;
            if (!this.LastWin || msg.nInstanceLevel4 == _status.nStopLevel)
            {//如果战斗失败或者是最后一关了，就把自动战斗设置成0
                _status.bAutoCombat = 0;
            }
            if (this.LastWin) {
                _status.nCurLevel = msg.nInstanceLevel4 + 1;
            }
            if (this.OnCombatEnd != null) {
                this.OnCombatEnd(msg.bInstanceDestroyed1 == 1 ? true : false, lastAutoCombat);
            }
        }
    }

    /// <summary>
    /// 服务器推送的消息
    /// </summary>
    /// <param name="buffer"></param>
    public void GBActionNotify(byte[] buffer) {
        STC_GAMECMD_ALLI_INSTANCE_ACTION_NOTIFY_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_ACTION_NOTIFY_T>(buffer);
        STC_GAMECMD_ALLI_INSTANCE_ACTION_NOTIFY_T.GB_SERVER_ACTION action = (STC_GAMECMD_ALLI_INSTANCE_ACTION_NOTIFY_T.GB_SERVER_ACTION)msg.nAction3;
        ulong associateCharID = msg.nCharID1;                               //此操作涉及的用户id
        ulong myAccountID = CommonData.player_online_info.AccountID;        //当前用户的id
        GBTeamListItem team = this.GetTeamData();                           //当前用户所在的队伍列表信息

        switch (action) {
            //有人加入 
            case STC_GAMECMD_ALLI_INSTANCE_ACTION_NOTIFY_T.GB_SERVER_ACTION.AI_ACTION_JOIN:
                if (associateCharID == myAccountID) {//如果是我自己加入的话，就不用触发操作，因为在加入回调里面已经做了 
                    return;
                }
                //直接重新刷新的页面
                if (this.OnJoinTeamComplete != null) { 
                    this.OnJoinTeamComplete(STC_GAMECMD_ALLI_INSTANCE_JOIN_T.enum_rst.RST_OK);
                }
            break;
            case STC_GAMECMD_ALLI_INSTANCE_ACTION_NOTIFY_T.GB_SERVER_ACTION.AI_ACTION_EXIT://有人退出 
                if (associateCharID == myAccountID) {
                    return;
                }
                GBTeamMemberItem quitMember = this.GetTeamMemberData(associateCharID);
                this._teamMemberList.Remove(quitMember);
                if (team != null)
                {
                    team.JoinedCount--;
                }
                //如果队伍已经出征了，则把出征队伍中的相关将领删除
                if (GBStatusManager.Instance.MyStatus.nInstanceStatus == (uint)INSTANCE_STATUS.instance_status_start)
                {
                    this._RemoveTroopHeros(quitMember);
                }

                if (this.OnKickMemberComplete != null) { 
                    this.OnKickMemberComplete(STC_GAMECMD_ALLI_INSTANCE_KICK_T.enum_rst.RST_OK);
                }
            break;
            case STC_GAMECMD_ALLI_INSTANCE_ACTION_NOTIFY_T.GB_SERVER_ACTION.AI_ACTION_KICK://有人被t   
                //如果我是leader，则在t人回调里面已经处理过了，直接退出
                if (GBStatusManager.Instance.MyStatus.nCreatorID == myAccountID) {
                    return;
                }
                //如果是我被t，则调用quit的回调；如果是别人被t，则调用kick的回调
                if (associateCharID == myAccountID)
                {
                    this.UpdateHeroStatus(HeroState.NORMAL);
                    this._status.SetDefault();
                    if (this.OnQuitTeamComplete != null) { 
                        this.OnQuitTeamComplete(STC_GAMECMD_ALLI_INSTANCE_EXIT_T.enum_rst.RST_OK);
                    }
                }
                else {
                    if (this.OnKickMemberComplete != null) { 
                       this.OnKickMemberComplete(STC_GAMECMD_ALLI_INSTANCE_KICK_T.enum_rst.RST_OK); 
                    }
                }
                this._teamMemberList.Remove(this.GetTeamMemberData(associateCharID));
                if (team != null) { 
                    team.JoinedCount--;
                }
                
            break;
            case STC_GAMECMD_ALLI_INSTANCE_ACTION_NOTIFY_T.GB_SERVER_ACTION.AI_ACTION_READY:        //有人配兵完毕  
                if (associateCharID == myAccountID) {
                    return;
                }
                if (this.OnOtherMemberDeploy != null) { 
                    this.OnOtherMemberDeploy();
                }
            break;
            case STC_GAMECMD_ALLI_INSTANCE_ACTION_NOTIFY_T.GB_SERVER_ACTION.AI_ACTION_DESTROY:      //队长销毁  
            case STC_GAMECMD_ALLI_INSTANCE_ACTION_NOTIFY_T.GB_SERVER_ACTION.AI_ACTION_SYS_DESTROY:  //系统销毁
                if (this.OnDisbandTeamCompelte != null) { 
                    this.OnDisbandTeamCompelte(STC_GAMECMD_ALLI_INSTANCE_DESTROY_T.enum_rst.RST_OK);
                }
                this._TeamDisbandLogic();
            break;
            case STC_GAMECMD_ALLI_INSTANCE_ACTION_NOTIFY_T.GB_SERVER_ACTION.AI_ACTION_START://副本开始 
                if (team.LordID == myAccountID) { return; }//如果是我开始的副本，则直接跳过
                _status.nCurLevel = 1;
                _status.nInstanceStatus = (int)INSTANCE_STATUS.instance_status_start;
                _status.bAutoCombat = 0;
                _status.nRetryTimes = 0;
                if (this.OnMarchTeam != null) { 
                    this.OnMarchTeam(STC_GAMECMD_ALLI_INSTANCE_START_T.enum_rst.RST_OK);
                }
            break;
            case STC_GAMECMD_ALLI_INSTANCE_ACTION_NOTIFY_T.GB_SERVER_ACTION.AI_ACTION_START_COMBAT://战斗开始  
                _status.nInstanceStatus = (uint)INSTANCE_STATUS.instance_status_fight;
                _status.nCharStatus = (uint)PLAYER_STATUS.instance_player_status_start;
                if (this.OnCombatStart != null) {
                    this.OnCombatStart(STC_GAMECMD_ALLI_INSTANCE_START_COMBAT_T.enum_rst.RST_OK);
                }
            break;
        }
    }

    /// <summary>
    /// 停止自动战斗
    /// </summary>
    public void StopAutoCombat() {
        CTS_GAMECMD_ALLI_INSTANCE_STOP_COMBAT_T req;
        req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ALLI_INSTANCE_STOP_COMBAT;
        req.nCombatType3 = (uint)HeroState.COMBAT_INSTANCE_GUILD;
        req.nInstanceID4 = _status.nInstanceID;
        TcpMsger.SendLogicData<CTS_GAMECMD_ALLI_INSTANCE_STOP_COMBAT_T>(req);
    }
    /// <summary>
    /// 停止自动战斗返回
    /// </summary>
    /// <param name="buffer"></param>
    public void StopAutoCombatRst(byte[] buffer) {
        STC_GAMECMD_ALLI_INSTANCE_STOP_COMBAT_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_STOP_COMBAT_T>(buffer);
        STC_GAMECMD_ALLI_INSTANCE_STOP_COMBAT_T.enum_rst val = (STC_GAMECMD_ALLI_INSTANCE_STOP_COMBAT_T.enum_rst)msg.nRst1;
        if (val == STC_GAMECMD_ALLI_INSTANCE_STOP_COMBAT_T.enum_rst.RST_OK) {
            _status.bAutoCombat = 0;
            //_status.nInstanceStatus = (uint)INSTANCE_STATUS.instance_status_start;
        }
        if (this.OnAutoCombatStop != null) {
            this.OnAutoCombatStop(val);
        }
    }

    /// <summary>
    /// 手动补兵
    /// </summary>
    public void ManualSupplyForce() {
        BattleManager.Instance.ManualSupply((uint)HeroState.COMBAT_INSTANCE_GUILD, _status.nInstanceID);
    }
    /// <summary>
    /// 手动补兵返回
    /// </summary>
    /// <param name="buffer"></param>
    public void ManualSupplyForceRst(byte[] buffer) {
        STC_GAMECMD_ALLI_INSTANCE_SUPPLY_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_SUPPLY_T>(buffer);
        if (msg.nRst1 != (int)STC_GAMECMD_ALLI_INSTANCE_SUPPLY_T.enum_rst.RST_OK)//补给成功以后等待将领状态改变后的回调 
        {
            LoadingManager.instance.HideLoading();
        }
    }
    /// <summary>
    /// 修改将领状态 
    /// </summary>
    /// <param name="heros"></param>
    public void ChangeHeroState(uint reason, HeroSimpleData[] heros) {
        List<GBLordTroopHero> teamHeros = this.TeamHeros;
        for (int i = 0; i < teamHeros.Count; ++i)
        {
            GBLordTroopHero thero = teamHeros[i];
            for (int j = 0; j < heros.Length; ++j)
            {
                HeroSimpleData hero = heros[j];
                if (thero.HeroID == hero.nHeroID1)
                {
                    thero.HeroForce = (uint)hero.nProf5;
                    thero.HeroHealth = (uint)hero.nHealthState6;
                    thero.HeroLevel = (uint)hero.nLevel7;
                    thero.TroopAmount = (uint)hero.nArmyNum4;
                    thero.TroopID = (uint)hero.nArmyType2;
                    thero.TroopLevel = (uint)hero.nArmyLevel3;
                    break;
                }
            }
        }
        if (reason == (uint)STC_GAMECMD_HERO_SIMPLE_DATA_T.reason_type.reason_self_supply) {
            LoadingManager.instance.HideLoading();
        }
    }

    private List<GBTeamMemberItem> _CombineLordAndHeroInfo(STC_GAMECMD_ALLI_INSTANCE_GET_CHAR_DATA_T.GBCharData[] chars, STC_GAMECMD_ALLI_INSTANCE_GET_HERO_DATA_T.GBHeroData[] heros)
    {
        List<GBTeamMemberItem> list = new List<GBTeamMemberItem>();
        for (int i = 0; i < chars.Length; ++i) {
            GBTeamMemberItem lord = chars[i];
            list.Add(lord);
            float force = 0f;
            for (int j = 0; j < heros.Length; ++j) {
                GBLordTroopHero hero = heros[j];
                if (lord.LordID == heros[j].nAccountID4) {
                    lord.Heros.Add(hero);
                    force += hero.HeroForce;
                }
            }
            lord.LordForce = (uint)force;
            //判断出队长是谁 
            if (lord.LordID == _status.nCreatorID) {
                lord.IsLeader = true;
            }
        }
        return list;
    }
    /// <summary>
    /// 查找队伍信息在，只有队伍的基本信息，不包含君主和队员的详细信息  
    /// </summary>
    /// <returns></returns>
	public GBTeamListItem GetTeamData(){
        return this.GetTeamData(GBStatusManager.Instance.MyStatus.nInstanceID);
    }
    public GBTeamListItem GetTeamData(ulong teamID)
    {
        for (int i = 0; i < this._teamList.Count; ++i)
        {
            if (this._teamList[i].TeamID == teamID)
            {
                return this._teamList[i];
            }
        }
        return null;
    }
    /// <summary>
    /// 队伍中所有的将领信息 
    /// </summary>
    public List<GBLordTroopHero> TeamHeros {
        get { 
            List<GBLordTroopHero> list = new List<GBLordTroopHero>();
            if (this._teamMemberList != null)
            {
                for (int i = 0; i < this._teamMemberList.Count; ++i)
                {
                    List<GBLordTroopHero> heros = this._teamMemberList[i].Heros;
                    list.AddRange(heros);
                }
            }
            
            return list;
        }
    }

    /// <summary>
    /// 查找副本中的队员的信息   
    /// </summary>
    /// <returns></returns>
    public GBTeamMemberItem GetTeamMemberData() {
        return this.GetTeamMemberData(CommonData.player_online_info.AccountID);
    }
    public GBTeamMemberItem GetTeamMemberData(ulong accountID) {
        for (int i = 0; i < this._teamMemberList.Count; ++i)
        {
            if (this._teamMemberList[i].LordID == accountID)
            {
                return this._teamMemberList[i];
            }
        }
        return null;
    }
    public GBTeamMemberItem MyTeamMember
    {
        get { return this.GetTeamMemberData(); }
    }

    public void UpdateMyTeamMemberData(GBTeamMemberItem newMember) {
        for (int i = 0; i < this._teamMemberList.Count; ++i)
        {
            if (this._teamMemberList[i].LordID == newMember.LordID)
            {
                this._teamMemberList[i] = newMember;
            }
        }
    }

    /// <summary>
    /// 修改我的所有将领的状态 
    /// </summary>
    /// <param name="state"></param>
    public void UpdateHeroStatus(HeroState state) {
        GBTeamMemberItem member = this.GetTeamMemberData();
        if (member != null)
        {
            List<GBLordTroopHero> heroList = this.GetTeamMemberData().Heros;
            if (heroList.Count > 0)
            {
                ulong[] heroIDs = new ulong[heroList.Count];
                for (int i = 0; i < heroList.Count; ++i)
                {
                    heroIDs[i] = heroList[i].HeroID;
                }
                JiangLingManager.UpdateHeroStatus(heroIDs, state);
            }
        }
    }

    /// <summary>
    /// 删除队伍中的英雄
    /// </summary>
    /// <param name="member"></param>
    private void _RemoveTroopHeros(GBTeamMemberItem member) {
        if (member == null) return;
        List<GBLordTroopHero> heros = member.Heros;
        ulong[] ids = new ulong[heros.Count];
        for (int i = 0; i < heros.Count; ++i)
        {
            ids[i] = heros[i].HeroID;
        }
        ArmyDeployManager.instance.ReleaseTroop(ids);
    }

    /// <summary>
    /// 副本被解散的处理
    /// </summary>
    private void _TeamDisbandLogic() {
        _status.SetDefault();
        this._teamList.Remove(this.GetTeamData());
        //解锁武将
        this.UpdateHeroStatus(HeroState.NORMAL);
        this.LastWin = false;
        //释放出兵信息
        if (_status.nInstanceStatus == (uint)INSTANCE_STATUS.instance_status_start)
        {
            this._RemoveTroopHeros(this.GetTeamMemberData());
        }
    } 
}
public delegate void GetTeamListHandler(List<GBTeamListItem> list);
public delegate void CreateTeamHandler(STC_GAMECMD_ALLI_INSTANCE_CREATE_T data);
public delegate void KickMemberHander(STC_GAMECMD_ALLI_INSTANCE_KICK_T.enum_rst result);
public delegate void DisbandTeamHandler(STC_GAMECMD_ALLI_INSTANCE_DESTROY_T.enum_rst result);
public delegate void JoinTeamHandler(STC_GAMECMD_ALLI_INSTANCE_JOIN_T.enum_rst result);
public delegate void QuitTeamHandler(STC_GAMECMD_ALLI_INSTANCE_EXIT_T.enum_rst result);
public delegate void GetHeroListHandler(List<GBLordTroopHero> list);
public delegate void GetMemberListHandler(List<GBTeamMemberItem> list);
public delegate void DeployHeroHandler(STC_GAMECMD_ALLI_INSTANCE_CONFIG_HERO_T.enum_rst result);
public delegate void GetReadyHandler(STC_GAMECMD_ALLI_INSTANCE_READY_T.enum_rst result);
public delegate void MarchTeamHandler(STC_GAMECMD_ALLI_INSTANCE_START_T.enum_rst result);
public delegate void OnOtherMemberDeployHandler();
public delegate void OnCombatStartHandler(STC_GAMECMD_ALLI_INSTANCE_START_COMBAT_T.enum_rst result);
public delegate void OnCombatEndHandler(bool instanceDestroyed, bool isLastAutoCombat);
public delegate void OnSimpleCombatLogGetHandler(STC_GAMECMD_ALLI_INSTANCE_GET_SIMPLE_COMBAT_LOG_T result);
public delegate void OnComplexCombatLogGetHandler(STC_GAMECMD_ALLI_INSTANCE_GET_COMBAT_LOG_T.enum_rst result, ComplexCombatLog complexLog, byte[] srcData);
public delegate void OnAutoCombatStopHandler(STC_GAMECMD_ALLI_INSTANCE_STOP_COMBAT_T.enum_rst result);
public delegate void OnManualSupplyHandler(STC_GAMECMD_ALLI_INSTANCE_SUPPLY_T.enum_rst result);
public delegate void OnManualSupplyHeroStateChangeHandler();
public delegate void OnRewardGet(STC_GAMECMD_ALLI_INSTANCE_GET_LOOT_T.enum_rst val, List<BaizInstanceLoot> list);