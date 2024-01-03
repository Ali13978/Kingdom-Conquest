using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;

public class MY_GUILDBATTLE_STATUS
{
    //每天总次数 
    public uint nTotalFreeTimes;
    //已经使用次数 
    public uint nUsedFreeTimes;
    //副本id 
    public ulong nInstanceID;
    //用户状态 
    public uint nCharStatus;
    //副本状态  
    public uint nInstanceStatus;	//instance_status_xxx
    //当前关卡  
    public uint nCurLevel;
    //是否自动战斗  
    public byte bAutoCombat;
    //是否自动补给 
    public byte bAutoSupply;
    //当前关卡已重试次数 
    public uint nRetryTimes;
    //每关可重试总次数 
    public uint nTotalRetryTimes;
    //副本创建者id 
    public ulong nCreatorID;
	//联盟副本最大关卡 
	public int MaxLevel;
    //自动战斗停止关卡   
    public uint nStopLevel;
    //上次战斗结果
    public uint nLastResult;

    public void SetDefault() {
        this.bAutoCombat = 0;
        this.bAutoSupply = 0;
        this.nCharStatus = (uint)PLAYER_STATUS.instance_player_status_normal;
        this.nCurLevel = 0;
        this.nInstanceID = 0;
        this.nInstanceStatus = (uint)INSTANCE_STATUS.instance_status_normal;
        //this.nRetryTimes = 0;
        //this.nTotalFreeTimes = 0;
        //this.nUsedFreeTimes = 0;
        this.nCreatorID = 0;
        this.nStopLevel = 0;
        this.nLastResult = 0;
    }
}

/// <summary>
/// 联盟副本状态
/// </summary>
enum INSTANCE_STATUS
{
    instance_status_normal = 0,		//未开战 
    instance_status_start = 1,		//已出征 
    instance_status_fight = 2,		//战斗中   
}
/// <summary>
/// 用户状态 
/// </summary>
enum PLAYER_STATUS
{
    instance_player_status_normal = 0,		// 正常 
    instance_player_status_ready = 1,		// 已经准备完毕 
    instance_player_status_start = 2,		// 已出征 
}

public class GBStatusManager : MonoBehaviour {
    #region singleton
    static GBStatusManager _instance;
    private GBStatusManager() { }
    static GBStatusManager()
    {
        _instance = new GBStatusManager();
    }
    public static GBStatusManager Instance
    {
        get { return _instance; }
    }
    #endregion

    private MY_GUILDBATTLE_STATUS _status = new MY_GUILDBATTLE_STATUS();

    public MY_GUILDBATTLE_STATUS MyStatus {
        get { return this._status; }
    }

	void Start () {
        this.RequestMyGBStatus();
	}

    /// <summary>
    /// 请求状态 
    /// </summary>
    public void RequestMyGBStatus()
    {
        CTS_GAMECMD_ALLI_INSTANCE_GET_STATUS_T req;
        req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ALLI_INSTANCE_GET_STATUS;
        req.nCombatType3 = (uint)HeroState.COMBAT_INSTANCE_GUILD;
        TcpMsger.SendLogicData<CTS_GAMECMD_ALLI_INSTANCE_GET_STATUS_T>(req);
    }
    /// <summary>
    /// 请求状态返回 
    /// </summary>
    /// <param name="buff"></param>
    public void RequestMyGBStatusRst(byte[] buff)
    {
        STC_GAMECMD_ALLI_INSTANCE_GET_STATUS_T msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_GET_STATUS_T>(buff);
        if (msg.nRst1 == (int)STC_GAMECMD_ALLI_INSTANCE_GET_STATUS_T.enum_rst.RST_OK)
        {
            this._status.bAutoCombat = msg.bAutoCombat8;
            this._status.bAutoSupply = msg.bAutoSupply9;
            this._status.nCharStatus = msg.nCharStatus5;
            this._status.nCurLevel = msg.nCurLevel7;
            this._status.nInstanceID = msg.nInstanceID4;
            this._status.nInstanceStatus = msg.nInstanceStatus6;
            this._status.nRetryTimes = msg.nRetryTimes10;
            this._status.nUsedFreeTimes = msg.nUsedFreeTimes3;
            this._status.nTotalFreeTimes = msg.nTotalFreeTimes2;
            this._status.nCreatorID = msg.nCreatorID11;
            this._status.nStopLevel = msg.nStopLevel12;
            this._status.nLastResult = msg.nLastresult13;
            Hashtable dmMap = CommonMB.InstanceDescMBInfo_Map;
            uint excelID = (uint)HeroState.COMBAT_INSTANCE_GUILD;
            if (dmMap.ContainsKey(excelID)) {
                InstanceDescMBInfo card = (InstanceDescMBInfo)dmMap[excelID];
                this._status.nTotalRetryTimes = (uint)card.nRetryTimes;
            }
            U3dCmn.SendMessage(GBWindowPath.GBLevelWin,"Start",null);
        }
    }
}
