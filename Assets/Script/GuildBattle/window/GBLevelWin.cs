using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TinyWar;
using STCCMD;
using CTSCMD;
using CMNCMD;

public class GBLevelWin : MonoBehaviour {
	Vector3[] npc_pos = null;
	
	public GameObject end_battle_btn;
    public UILabel end_battle_label;
    public UILabel battle_time_label;
	public GameObject view_battle_btn;
    public GameObject auto_battle_btn;
    public UILabel auto_battle_label;
    public tk2dAnimatedSprite winAnimSprite;
	public GameObject npc_item;
    public UILabel battle_level_label;
    public UILabel retry_label;
    public GameObject reward_btn;
    public const int LevelGroup = 10;
    public bool IsManual;    //是否手动的 
    private bool _isWin;
    private bool _isWatch;   //是否在观战 
    private byte[] _combatLog;
    private bool _serverEnd = true;             //服务器端是否结束战斗了 
    private bool _instanceDestroyed;            //本关之后是否副本被销毁 
	public GameObject TopBtn;
	public GameObject BottomBtn;
    public bool _battleFieldBack = true;
    public bool _delayDisband;
    public bool _startLock;                   //开始战斗以后，简单战报返回以前的锁定，防止用户点击stopautocombat按钮
    /// <summary>
    /// 起始关数，每10关算一个Node
    /// </summary>
    private int _startGroupID;
    private MY_GUILDBATTLE_STATUS _status = GBStatusManager.Instance.MyStatus;
	void Awake()
	{
        if (U3dCmn.GetIphoneType() == IPHONE_TYPE.IPHONE5) { 
            KeepOut.instance.ShowKeepOut();
        }
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			TopBtn.transform.localPosition = TopBtn.transform.localPosition+new Vector3(0,20,0);
			BottomBtn.transform.localPosition = BottomBtn.transform.localPosition+new Vector3(0,-20,0);
		}
        U3dCmn.ScaleScreen(this.transform);
        
        this.winAnimSprite.animationCompleteDelegate += this.ResultAnimComplete;
        this.winAnimSprite.gameObject.active = false;
        this.battle_time_label.gameObject.active = false;
        GBTeamManager.Instance.OnQuitTeamComplete += this.OnQuitTeam;
        GBTeamManager.Instance.OnDisbandTeamCompelte += this.OnDisbandTeam;
        GBTeamManager.Instance.OnCombatEnd += this.OnCombatEnd;
        GBTeamManager.Instance.OnSimpleCombatLogGet += this.OnSimpleCombatLogGet;
        GBTeamManager.Instance.OnComplexCombatLogGet += this.OnComplexCombatLogGet;
        GBTeamManager.Instance.OnAutoCombatStop += this.OnStopAutoCombat;
        GBTeamManager.Instance.OnMemeberListGet += this.OnMemberListGet;
        GBTeamManager.Instance.OnRewardGet += this.OnRewardGet;
	}

    void DismissPanel()
    {
		 if (U3dCmn.GetIphoneType() == IPHONE_TYPE.IPHONE5) {
            KeepOut.instance.HideKeepOut();
        }
        this.winAnimSprite.animationCompleteDelegate -= this.ResultAnimComplete;
        GBTeamManager.Instance.OnQuitTeamComplete -= this.OnQuitTeam;
        GBTeamManager.Instance.OnDisbandTeamCompelte -= this.OnDisbandTeam;
        GBTeamManager.Instance.OnCombatEnd -= this.OnCombatEnd;
        GBTeamManager.Instance.OnSimpleCombatLogGet -= this.OnSimpleCombatLogGet;
        GBTeamManager.Instance.OnAutoCombatStop -= this.OnStopAutoCombat;
        GBTeamManager.Instance.OnMemeberListGet -= this.OnMemberListGet;
        GBTeamManager.Instance.OnRewardGet -= this.OnRewardGet;
        Destroy(gameObject);
       
		
    }
	
	void Start() {
        this.CreateNPC();
        ulong instanceLeaderID = this._status.nCreatorID;
        if (instanceLeaderID != CommonData.player_online_info.AccountID)//队员
        {
            this.auto_battle_btn.gameObject.SetActiveRecursively(false);
            end_battle_label.text = "Quit";
            UIButtonMessage msg = end_battle_btn.GetComponent<UIButtonMessage>();
            if (msg != null)
            {
                msg.functionName = "QuitTeamConfirm";
            }

            Vector3 pos = reward_btn.transform.localPosition;
            reward_btn.transform.localPosition = new Vector3(pos.x, -95, pos.z);
        }
        else
        {
            UpdateAutoCombatState();
        }

        print(string.Format("instanceStatus:{0} currentLevel:{1}", _status.nInstanceStatus, _status.nCurLevel));
        UpdateRetryState();
        if (_status.nInstanceStatus == (uint)INSTANCE_STATUS.instance_status_fight)
        {
            this.UpdateStageState2((int)_status.nCurLevel, GBLevelItemUI.GuardState.InBattle);
        }
        else {
            //如果是自动战斗，恰好上关结束，下一关的简单战报没有返回
            if (_status.bAutoCombat == 1)
            {
                this.UpdateStageState2((int)_status.nCurLevel, GBLevelItemUI.GuardState.InBattle);
            }
            else { 
                this.UpdateStageState((int)_status.nCurLevel);
            }
        }

        if (GBTeamManager.Instance.LastEndTime > DateTime.Now)
        {
            this.battle_time_label.gameObject.SetActiveRecursively(true);
            this._combatEndTime = GBTeamManager.Instance.LastEndTime;
            this.StartCoroutine("CombatGoing", GBTeamManager.Instance.LastWin);
        }
	}

    #region 关卡相关
    //创建关卡 
	void CreateNPC()
	{
        npc_pos = new Vector3[10];
        npc_pos[0] = new Vector3(-112f, -82f, 0f);
        npc_pos[1] = new Vector3(20f, -91f, 0f);
        npc_pos[2] = new Vector3(94f, -42f, 0f);
        npc_pos[3] = new Vector3(-57f, -12f, 0f);
        npc_pos[4] = new Vector3(-180f, 12f, 0f);
        npc_pos[5] = new Vector3(-149f, 71f, 0f);
        npc_pos[6] = new Vector3(-51f, 100f, 0f);
        npc_pos[7] = new Vector3(32f, 93f, 0f);
        npc_pos[8] = new Vector3(118f, 98f, 0f);
        npc_pos[9] = new Vector3(158f, 27f, 0f);
	}

    void UpdateRetryState() {
        this.retry_label.text = string.Format("Retry Times: {0}/{1}", _status.nRetryTimes, _status.nTotalRetryTimes);
    }
    /// <summary>
    /// 更新所有关卡状态
    /// </summary>
    void UpdateStageState(int curLevel)
    {
        this.InitStage((int)_status.nCurLevel);
        int currentLevel = (curLevel - 1) % LevelGroup;
        INSTANCE_STATUS curSt = (INSTANCE_STATUS)_status.nInstanceStatus;

        for (int i = 0; i < npc_pos.Length; ++i)
        {
            GameObject item = U3dCmn.GetChildObjByName(this.gameObject, string.Format("st{0:D2}", i));
            GBLevelItemUI guard = item.GetComponent<GBLevelItemUI>();

            if (i == currentLevel)
            {
                if (curSt == INSTANCE_STATUS.instance_status_start)
                {
                    guard.State = GBLevelItemUI.GuardState.Next;
                }
                else
                {
                    guard.State = GBLevelItemUI.GuardState.InBattle;
                }
            }
            else if (i < currentLevel)
            {
                guard.State = GBLevelItemUI.GuardState.Passed;
            }
        }
    }

    /// <summary>
    /// 更新某个关卡
    /// </summary>
    /// <param name="level"></param>
    /// <param name="state"></param>
    void UpdateStageState(int level, GBLevelItemUI.GuardState state)
    {
        this.InitStage((int)_status.nCurLevel);
        int objID = (level - 1) % LevelGroup;
        for (int i = 0; i < npc_pos.Length; ++i)
        {
            if (i == objID)
            {
                string objName = string.Format("st{0:D2}", objID);
                GameObject item = U3dCmn.GetChildObjByName(this.gameObject, objName);
                GBLevelItemUI guard = item.GetComponent<GBLevelItemUI>();
                guard.State = state;
                break;
            }
        }
    }

    void UpdateStageState2(int level, GBLevelItemUI.GuardState state)
    {
        this.InitStage((int)_status.nCurLevel);
        int objID = (level - 1) % LevelGroup;
        for (int i = 0; i < npc_pos.Length; ++i)
        {
            string objName = string.Format("st{0:D2}", i);
            GameObject item = U3dCmn.GetChildObjByName(this.gameObject, objName);
            GBLevelItemUI guard = item.GetComponent<GBLevelItemUI>();
            if (i == objID)
            {
                guard.State = state;
            }
            else if (i < objID)
            {
                guard.State = GBLevelItemUI.GuardState.Passed;
            }
        }
    }

    /// <summary>
    /// 根据level生成所有关卡
    /// </summary>
    /// <param name="level"></param>
    void InitStage(int level)
    {
        this._startGroupID = (level - 1) / LevelGroup;
        for (int i = 0; i < npc_pos.Length; ++i)
        {
            int stageID = i + 1 + _startGroupID * LevelGroup;
            string name = string.Format("st{0:D2}", (stageID - 1) % LevelGroup);
            GameObject addItem = U3dCmn.GetChildObjByName(this.gameObject, name);
            if (addItem == null)
            {
                addItem = NGUITools.AddChild(this.gameObject, npc_item);
                addItem.transform.localPosition = npc_pos[i];
                addItem.name = name;
            }
            GBLevelItemUI guardItem = addItem.GetComponent<GBLevelItemUI>();
            guardItem.SetStageInfo(stageID);
            guardItem.GBLevelWin = this;
        }
        this.battle_level_label.text = string.Format("Lv {0} - {1}", _startGroupID * LevelGroup + 1, (_startGroupID + 1) * LevelGroup);
    }
    #endregion

    void UpdateAutoCombatState() {
        UIButtonMessage msg = auto_battle_btn.GetComponent<UIButtonMessage>();
        if (_status.bAutoCombat == 1) {
            auto_battle_label.text = "Stop-battle";
            msg.functionName = "StopAutoCombat";
        }
        else {
            auto_battle_label.text = "Auto-battle";
            msg.functionName = "StartAutoCombat";
        }
    }

    /// <summary>
    /// 自动战斗
    /// </summary>
    void StartAutoCombat() {
        if (_status.nInstanceStatus == (uint)INSTANCE_STATUS.instance_status_fight)
        {
            U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_TEAM_COMBAT_GOING));
            return;
        }
        _status.bAutoCombat = 1;
        this.ShowArmyDeployWin();
    }
    /// <summary>
    /// 手动战斗
    /// </summary>
    public void StartManualCombat() {
        //如果是队员的话，需要重新获取队伍列表,因为可能队长的排兵布阵会发生变化
        if (_status.nCreatorID != CommonData.player_online_info.AccountID)
        {
            GBTeamManager.Instance.RequestTeamMember(_status.nInstanceID);
            LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_GET_HERO_DATA);
        }
        else
        {
            this.IsManual = true;
            ShowArmyDeployWin();
        }
    }
    /// <summary>
    /// 开始战斗
    /// </summary>
    void StartCombat()
    {
        this._startLock = true;
        this.UpdateStageState2((int)_status.nCurLevel, GBLevelItemUI.GuardState.InBattle);
        GBTeamManager.Instance.StartCombat();
        if (_status.bAutoCombat == 1)
        {
            this.UpdateAutoCombatState();
        }
    }

    void OnMemberListGet(List<GBTeamMemberItem> list)
    {
        this.IsManual = true;
        this.ShowArmyDeployWin();
    }

    void ShowArmyDeployWin()
    {
        GameObject obj = U3dCmn.GetObjFromPrefab("ArmyDeployWin");
        if (obj != null)
        {
            ArmyDeployManager.instance.InitialArmyDeployData();
            obj.SendMessage("RevealPanel", COMBAT_TYPE.COMBAT_INSTANCE_GUILD);
        }
    }

    /// <summary>
    /// 停止自动战斗
    /// </summary>
    void StopAutoCombat() {
        if (this._startLock) {
            return;
        }
        GBTeamManager.Instance.StopAutoCombat();
        LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_STOP_COMBAT);
    }
    void OnStopAutoCombat(STC_GAMECMD_ALLI_INSTANCE_STOP_COMBAT_T.enum_rst val) {
        if (val == STC_GAMECMD_ALLI_INSTANCE_STOP_COMBAT_T.enum_rst.RST_OK) {
            this.UpdateAutoCombatState();
        }
    }

    /// <summary>
    /// 退出队伍
    /// </summary>
    void QuitTeam(GameObject obj) {
        GBTeamManager.Instance.QuitTeam(_status.nInstanceID);
        LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_EXIT);
    }
    void QuitTeamConfirm()
    {
        if (_status.nInstanceStatus == (uint)INSTANCE_STATUS.instance_status_fight)
        {
            U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_TEAM_COMBAT_GOING));
            return;
        }
        PopConfirmWin.instance.ShowPopWin(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_QUIT_TEAM), QuitTeam);
    }
    void OnQuitTeam(STC_GAMECMD_ALLI_INSTANCE_EXIT_T.enum_rst result) {
        if (result == STC_GAMECMD_ALLI_INSTANCE_EXIT_T.enum_rst.RST_OK)
        {
            this.DismissPanel();
            U3dCmn.SendMessage("AllianceWin", "OpenGuildWarPanel", null);
        }
        else {
            string warnText = "";
            switch (result) { 
                case STC_GAMECMD_ALLI_INSTANCE_EXIT_T.enum_rst.RST_COMBAT://战斗中无法退出  
                    warnText = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_TEAM_COMBAT_GOING);
                break;
                case STC_GAMECMD_ALLI_INSTANCE_EXIT_T.enum_rst.RST_NO_INSTANCE://副本不存在了
                    warnText = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_TEAM_NOT_EXIST);
                    this.DismissPanel();
                break;
            }
            if (warnText != "") {
                U3dCmn.ShowWarnWindow(warnText);
            }
        }
    }

    /// <summary>
    /// 解散退伍
    /// </summary>
    public void DisbandTeam(GameObject obj) {
        GBTeamManager.Instance.DisbandTeam(_status.nInstanceID);
        LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_DESTROY);
    }
    void DisbandTeamConfirm() {
        if (_status.nInstanceStatus == (uint)INSTANCE_STATUS.instance_status_fight) {
            U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_TEAM_COMBAT_GOING));
            return;
        }
        PopConfirmWin.instance.ShowPopWin(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_QUIT_TEAM), DisbandTeam);
    }
    void OnDisbandTeam(STC_GAMECMD_ALLI_INSTANCE_DESTROY_T.enum_rst result)
    {
        if (result == STC_GAMECMD_ALLI_INSTANCE_DESTROY_T.enum_rst.RST_OK)
        {
            //如果是重试次数到了被销毁的则通知用户
            if (_status.nRetryTimes == _status.nTotalRetryTimes)
            {
                if (!this._battleFieldBack && GBTeamManager.Instance.MyTeamMember.LordID == _status.nCreatorID)//队长可能需要等待战场动画结束再销毁窗口
                {
                    this._delayDisband = true;
                }
                else { //队员不知道战斗状态，收到这个消息就直接销毁窗口
                    PopTipDialog.instance.VoidSetText2(true, false, U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_RETRY_TIMES_USEDUP));
                    this.DismissPanel();
                }
            }
            else { 
                this.DismissPanel();
            }
        }
        else {
            string warnText = "";
            switch (result) { 
                case STC_GAMECMD_ALLI_INSTANCE_DESTROY_T.enum_rst.RST_COMBAT://正在战斗中，无法退出  
                    warnText = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_TEAM_COMBAT_GOING);
                    break;
            }
            if (warnText != "") {
                U3dCmn.ShowWarnWindow(warnText);
            }
        }
    }

    /// <summary>
    /// 观战
    /// </summary>
    void WatchCombat()
    {
        this._isWatch = true;
        GBTeamManager.Instance.RequestComplexCombatLog();
        LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_COMBAT_LOG);
    }

    /// <summary>
    /// T人
    /// </summary>
    void KickMember()
    {
        if (_status.nInstanceStatus == (uint)INSTANCE_STATUS.instance_status_fight)
        {
            return;
        }
        GameObject obj = U3dCmn.GetObjFromPrefab(GBWindowPath.GBKickWin);
        GBKickWin win = obj.GetComponent<GBKickWin>();
        win.MemberList = GBTeamManager.Instance.TeamMemberList;
    }

    /// <summary>
    /// 载入战斗场景，加载GBBattleField
    /// </summary>
    /// <param name="result"></param>
    /// <param name="seed"></param>
    /// <param name="leftList"></param>
    /// <param name="rightList"></param>
    void EnterBattleField(ComplexCombatLog log)
    {
        log.IsLeft = true;
        log.OnBattleFieldEnd = this.OnBattleFieldEnd;
        BattleManager.Instance.ShowBattleWindow(log,false,true);
        this._battleFieldBack = false;
    }

    /// <summary>
    /// GBBattleField回调，战场画面结束
    /// </summary>
    /// <param name="isWin">战斗结果</param> 
    /// <param name="isSkip">是否跳过</param> 
    public void OnBattleFieldEnd(bool isWin, bool isSkip)
    {
            this._battleFieldBack = true;
			if (this._isWatch)//如果是观战观看完毕，则不要修改关卡状态 
	        {
	            if (isSkip)
	            {
	                this.PlayResultAnim(isWin);
	            }
                this.StartCoroutine("ShowBattleLog", 1);
	            this._isWatch = false;
	        }
	        else
	        {
                //ManualCombatComplete(new BattleResuult { IsWin = isWin, IsSkip = isSkip });
                StartCoroutine("ManualCombatComplete", new BattleResuult { IsWin = isWin, IsSkip = isSkip });
	        }
    }

    /// <summary>
    /// 服务器推送的战斗结束，一旦推送过来就修改关卡状态
    /// </summary>
    void OnCombatEnd(bool destroyed, bool lastAuto) {
        this._serverEnd = true;
        print("instance destroyed:\t"+ destroyed);
        this._instanceDestroyed = destroyed;
        //print("auto combat: "+ _status.bAutoCombat);
        this.battle_time_label.gameObject.active = false;
        this.PlayResultAnim(GBTeamManager.Instance.LastWin);
        
        if (_status.bAutoCombat == 1) { 
            AutoCombatComplete(GBTeamManager.Instance.LastWin);
        }
        else
        {
            IsManual = false;
            this.UpdateStageState2((int)_status.nCurLevel, GBLevelItemUI.GuardState.Next);
            this.UpdateRetryState();
            if (destroyed && lastAuto)//如果上轮自动战斗并且副本已经被销毁
            {
                PopTipDialog.instance.VoidSetText2(true, false, U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_RETRY_TIMES_USEDUP));
                this.DismissPanel();
                return;
            }
        }
        this.UpdateAutoCombatState();
    }

    private DateTime _combatEndTime;
    /// <summary>
    /// 播放时间倒计时
    /// </summary>
    /// <param name="win"></param>
    /// <returns></returns>
    IEnumerator CombatGoing(bool win) {
        int leftSeconds = (int)((_combatEndTime - DateTime.Now).TotalSeconds);
        while (leftSeconds >= 0)
        {
            //if (_combatEndTime < DateTime.Now)//如果已经是0秒了，但是服务器端还是没有返回，那么就一直等待服务器返回
            //{
            //    if (!this._serverEnd)
            //    {
            //        _combatEndTime = _combatEndTime.AddSeconds(1);
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}
            this.battle_time_label.text = string.Format("Battle Time: {0} s", leftSeconds);
            if (leftSeconds <= 0) {
                break;
            }
            leftSeconds = (int)(_combatEndTime.Subtract(TimeSpan.FromSeconds(1)) - DateTime.Now).TotalSeconds;
            yield return new WaitForSeconds(1);
        }
    }
    /// <summary>
    /// 播放战斗结果动画
    /// </summary>
    /// <param name="win"></param>
    void PlayResultAnim(bool win)
    {
        this.winAnimSprite.gameObject.SetActiveRecursively(true);
        if (win)
        {
            this.winAnimSprite.Play("anim-victory");
        }
        else
        {
            this.winAnimSprite.Play("anim-failure");
        }
    }
    void ResultAnimComplete(tk2dAnimatedSprite sprite, int clipId)
    {
        sprite.gameObject.SetActiveRecursively(false);
    }

    /// <summary>
    /// 简单战报返回
    /// </summary>
    /// <param name="result"></param>
    void OnSimpleCombatLogGet(STC_GAMECMD_ALLI_INSTANCE_GET_SIMPLE_COMBAT_LOG_T result){
        STC_GAMECMD_ALLI_INSTANCE_GET_SIMPLE_COMBAT_LOG_T.enum_rst val = (STC_GAMECMD_ALLI_INSTANCE_GET_SIMPLE_COMBAT_LOG_T.enum_rst)result.nRst1;
        if (val == STC_GAMECMD_ALLI_INSTANCE_GET_SIMPLE_COMBAT_LOG_T.enum_rst.RST_OK)
        {
            this._startLock = false;
            bool isWin = result.nCombatResult4 == 1 ? true : false;
            this._serverEnd = false;
            this._isWin = isWin;
            if (_status.bAutoCombat != 1  && GBTeamManager.Instance.MyTeamMember.IsLeader)//手动的获取复杂战报
            {
                GBTeamManager.Instance.RequestComplexCombatLog();
                LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_COMBAT_LOG);
            }
            this.UpdateStageState2((int)_status.nCurLevel, GBLevelItemUI.GuardState.InBattle);
            this.battle_time_label.gameObject.active = true;
            this._combatEndTime = GBTeamManager.Instance.LastEndTime;
            this.StartCoroutine("CombatGoing", isWin);
        }
    }
    /// <summary>
    /// 复杂战报返回
    /// </summary>
    /// <param name="result"></param>
    /// <param name="complexLog"></param>
    /// <param name="data"></param>
    void OnComplexCombatLogGet(STC_GAMECMD_ALLI_INSTANCE_GET_COMBAT_LOG_T.enum_rst result, ComplexCombatLog complexLog, byte[] data) {
        if (result == STC_GAMECMD_ALLI_INSTANCE_GET_COMBAT_LOG_T.enum_rst.RST_OK)
        {
            this.EnterBattleField(complexLog);
            this._combatLog = data;
        }
    }

    struct BattleResuult {
        public bool IsWin;
        public bool IsSkip;
    }

    /// <summary>
    /// 自动战斗结束
    /// </summary>
    /// <param name="win"></param>
    void AutoCombatComplete(bool win)
    {
        if (win)
        {
            if (!this._instanceDestroyed)
            {
                if (_status.bAutoCombat == 1)
                {
                    if (GBTeamManager.Instance.MyTeamMember.IsLeader){
                        this.UpdateStageState2((int)_status.nCurLevel, GBLevelItemUI.GuardState.InBattle);
                    }
                    else {
                        this.UpdateStageState2((int)_status.nCurLevel, GBLevelItemUI.GuardState.Next);
                    }
                }
                else
                {
                    this.UpdateStageState2((int)_status.nCurLevel, GBLevelItemUI.GuardState.Next);
                }
            }
        }
        else
        {
            this.UpdateStageState2((int)_status.nCurLevel, GBLevelItemUI.GuardState.Next);
            this.UpdateRetryState();
        }
    }

    /// <summary>
    /// 手动战斗结束
    /// </summary>
    /// <param name="re"></param>
    /// <returns></returns>
    IEnumerator ManualCombatComplete(BattleResuult re) {
        while (true)
        {
            if (this._serverEnd)
            {
                //失败就停止当前关卡
                if (!re.IsWin)
                {
                    this.UpdateStageState2((int)_status.nCurLevel, GBLevelItemUI.GuardState.Next);
                    this.UpdateRetryState();
                }
                else
                {
                    if (!this._instanceDestroyed)
                    {
                        this.UpdateStageState2((int)_status.nCurLevel, GBLevelItemUI.GuardState.Next);
                    }
                }
                if (re.IsSkip)
                {
                    this.PlayResultAnim(re.IsWin);
                }
                this.StartCoroutine("ShowBattleLog", 1);
                break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
    
    /// <summary>
    /// 显示战斗日志
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator ShowBattleLog(int time) {
        yield return new WaitForSeconds(time);
        GameObject obj = U3dCmn.GetObjFromPrefab("BattleLogWin");
        if (obj != null)
        {
            BattleLogWin win = obj.GetComponent<BattleLogWin>();
            win.InitialByBuff(this._combatLog);
        }
        if (this._delayDisband) {
            this._delayDisband = false;
            PopTipDialog.instance.VoidSetText2(true, false, U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_RETRY_TIMES_USEDUP));
            this.DismissPanel();
        }
    }

    /// <summary>
    /// 获取本关奖励
    /// </summary>
    void GetReward() {
        GBTeamManager.Instance.GetReward();
        LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_GET_LOOT);
    }
    void OnRewardGet(STC_GAMECMD_ALLI_INSTANCE_GET_LOOT_T.enum_rst val, List<BaizInstanceLoot> list)
    {
        if (val == STC_GAMECMD_ALLI_INSTANCE_GET_LOOT_T.enum_rst.RST_OK)
        {
            GameObject go = U3dCmn.GetObjFromPrefab("BaizhanLootScript");
            if (go == null) return;

            BaizhanLootScript t = go.GetComponent<BaizhanLootScript>();
            if (t != null)
            {
                t.Depth(this.transform.localPosition.z - BaizVariableScript.DEPTH_OFFSET);
                t.gid = 0;
                t.ApplyLootList(list);
            }
        }
    }
}
