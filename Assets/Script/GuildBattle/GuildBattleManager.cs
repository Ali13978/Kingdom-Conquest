using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TinyWar;


public class GuildBattleManager : MonoBehaviour
{	
    float xStart;
    float yStrat;
    int rowCount;
    int columnCount;

    bool _watchEnd;
    GuildBattle _battle;
    IList<WarriorSp> _teamAll;
    GameObject _warriorTemplate;                        //用来做士兵精灵的克隆模板
    public TagType Group = TagType.Left;                //标定阵营
    public int Seed;                                    //随机数种子  
    public List<BattleWarrior> WarriorLeftList;         //左侧将领列表
    public List<BattleWarrior> WarriorRightList;        //右侧将领列表
    public bool IsWin;                                  //战斗结果，左方输赢结果
    public float AttackEnhanceRatio;                    //克制比率
    public bool IsSmallField;                    //是否是小战场
    public bool ShowSkipBtn = true;                     //是否可以跳过
    public tk2dAnimatedSprite ResultAnimSprite;
    public GameObject SkipSprite;
    public GameObject BattleRootObj;
    public tk2dBaseSprite BackgroundLeft;
    public tk2dBaseSprite BackgroundRight;
    public tk2dBaseSprite BackgroundSmall;
    public OnBattleFieldEnd OnBattleFieldEnd;
    #region 绘制debug表格和reset
    void OnGUI()
    {
        //if (GUI.Button(new Rect(20, 20, 80, 30), "Reset"))
        //{
        //    StopAllCoroutines();
        //    Application.LoadLevel("GuildBattle");
        //}
    }

    void OnDrawGizmos()
    {
        //this._DrawGrid();
    }

    private void _DrawGrid()
    {
        for (int i = 0; i < columnCount; i++)
        {
            //绘制顺序，纵向横向
            Vector3 start = new Vector3((xStart + i * Config.GRID_WIDTH) * Config.GRID_RESOLUTION, yStrat * Config.GRID_RESOLUTION, 0);
            Vector3 end = new Vector3((xStart + i * Config.GRID_WIDTH) * Config.GRID_RESOLUTION, (yStrat + Config.GRID_ROW_COUNT * Config.GRID_HEIGHT) * Config.GRID_RESOLUTION, 0);
            Debug.DrawLine(start, end);
        }
        for (int j = 0; j < rowCount; j++)
        {
            Vector3 start = new Vector3(xStart * Config.GRID_RESOLUTION, (yStrat + j * Config.GRID_HEIGHT) * Config.GRID_RESOLUTION, 0);
            Vector3 end = new Vector3((xStart + Config.GRID_COLUMN_COUNT * Config.GRID_WIDTH) * Config.GRID_RESOLUTION, (yStrat + j * Config.GRID_HEIGHT) * Config.GRID_RESOLUTION, 0);
            Debug.DrawLine(start, end);
        }

    }
    #endregion

    void Awake() {
        U3dCmn.ScaleScreen(this.transform);
        if (U3dCmn.GetIphoneType() == CMNCMD.IPHONE_TYPE.IPHONE5) {
            Vector3 pos = this.BattleRootObj.transform.localPosition;
            this.BattleRootObj.transform.localPosition = new Vector3(-44, pos.y, pos.z);
        }
        if (U3dCmn.GetDeviceType() == CMNCMD.DEVICE_TYPE.IPAD) {
            this.SkipSprite.transform.localPosition += new Vector3(0, -20, 0);
        }
        KeepOut.instance.HideKeepOut();
        this.ResultAnimSprite.gameObject.active = false;
        this.SkipSprite.gameObject.SetActiveRecursively(false);
        this.ResultAnimSprite.animationCompleteDelegate += this._resultAnimComplete;
        this.BackgroundSmall.gameObject.active = false;
        GBHelper.RegisterEvent();
    }

    void Start() {
    }

    public void ShowField() {
        if (this.IsSmallField)
        {
            SmallFieldPack();
        }
        else {
            Config.GRID_COLUMN_COUNT = 16;
            Config.GRID_ORIGIN[0] = -160f;
            Config.MIN_HALF_SPACE = 3;
        }
        float xStart = Config.GRID_ORIGIN[0];
        float yStrat = Config.GRID_ORIGIN[1];
        int rowCount = Config.GRID_ROW_COUNT + 1;
        int columnCount = Config.GRID_COLUMN_COUNT + 1;

        this._battle = new GuildBattle();
        if (this.AttackEnhanceRatio != 0f)
        {
            this._battle.AttackEnhanceRatio = this.AttackEnhanceRatio;
        }
        this._battle.Prepare(this.Seed);
        this._teamAll = new List<WarriorSp>();
        this.generateData();

        this._DrawWarrior();
        //先看双方的布阵
        //this.StartCoroutine("_WatchTroop");
        if (!this.IsSmallField)
        {
            //先看双方的布阵
            this.StartCoroutine("_WatchTroop");
        }
        else
        {
            if (this.ShowSkipBtn)
            {
                this.SkipSprite.gameObject.SetActiveRecursively(true);
            }
            this.StartCoroutine("GameLogicUpdate");
        }
    }

    void SmallFieldPack()
    {
        if (U3dCmn.GetIphoneType() == CMNCMD.IPHONE_TYPE.IPHONE5)
        {
            Vector3 pos = this.BattleRootObj.transform.localPosition;
            this.BattleRootObj.transform.localPosition = new Vector3(0, pos.y, pos.z);
            KeepOut.instance.ShowKeepOut();
        }
        GameObject move = U3dCmn.GetChildObjByName(this.gameObject, "MoveCamera");
        Destroy(move);
        Destroy(this.BackgroundLeft.gameObject);
        Destroy(this.BackgroundRight.gameObject);
        this.BackgroundSmall.gameObject.active = true;
        Config.GRID_COLUMN_COUNT = 8;
        Config.GRID_ORIGIN[0] -= 40f;
        Config.MIN_HALF_SPACE = 2;
    }

    void DismissPanel() {
        this.ResultAnimSprite.animationCompleteDelegate -= this._resultAnimComplete;
        this.OnBattleFieldEnd = null;
		if(GameObject.Find(GBWindowPath.GBLevelWin) != null)
		{
			KeepOut.instance.ShowKeepOut();
		}
        if (IsSmallField) {
            KeepOut.instance.HideKeepOut();
        }
        Destroy(gameObject);
		Resources.UnloadUnusedAssets();
    }
    #region 观看阵型
    private IEnumerator _WatchTroop() {
        float xMin = -480f;
        float xMax = 0f;
        if (U3dCmn.GetIphoneType() == CMNCMD.IPHONE_TYPE.IPHONE5) {
            xMin = -436f;
            xMax = -44f;
        }
        Vector3 des = new Vector3(xMin, 0, -6);
        if (this.Group == TagType.Right) {
            this.BattleRootObj.transform.localPosition = new Vector3(xMin, 0, -6);
            des = new Vector3(xMax, 0, -6);
        }
        yield return new WaitForSeconds(0.3f);

        TweenPosition posTween = UITweener.Begin<TweenPosition>(this.BattleRootObj, 1f);
        posTween.from = posTween.position;
        posTween.to = des;
        posTween.onFinished += this._moveScreenEnd;
        posTween.Play(true);
    }

    private void _moveScreenEnd(UITweener tween) {
        TweenPosition posTween = tween as TweenPosition;
        if (this._watchEnd) {
            posTween.onFinished -= this._moveScreenEnd;
            this.StartCoroutine("GameLogicUpdate");
            if (this.ShowSkipBtn) { 
                this.SkipSprite.gameObject.SetActiveRecursively(true);
            }
            return;
        }
        this.StartCoroutine("_reverseMoveScreen", posTween);
    }

    private IEnumerator _reverseMoveScreen(TweenPosition tween) {
        yield return new WaitForSeconds(0.2f);
        tween.Toggle();
        _watchEnd = true;
    }
    #endregion
    
    void generateData() {
        IList<BattleWarrior> list = new List<BattleWarrior>();
        float totalForce = 0;
        
        for (int i = 0; i < this.WarriorLeftList.Count; ++i) {
            totalForce += this.WarriorLeftList[i].Force;
        }
        this._battle.ConvertToWarriorList(this.WarriorLeftList, TagType.Left);
        GBHelper.Print("left force:"+ totalForce);
        totalForce = 0;
        //右侧的先处理一下
        for (int i = 0; i < this.WarriorRightList.Count; ++i) {
            totalForce += this.WarriorRightList[i].Force;
        }
        this._battle.ConvertToWarriorList(this.WarriorRightList, TagType.Right);
        GBHelper.Print("right force:" + totalForce);
    }

    IEnumerator GameLogicUpdate() {
        while (true) {
            bool gameover = this._battle.Tick();
            if (gameover) {
                U3dCmn.WriteText("gameresult.text", _battle.BattleLog);
                this._GameOver();
                break;
            }
            yield return new WaitForSeconds(Config.BATTLE_LOGIC_DURATION);
        }
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    private void _GameOver() {
        for (int i = 0; i < this._teamAll.Count; ++i)
        {
            var sp = this._teamAll[i];
            if (sp.Warrior.isAlive)
            {
                TweenPosition tween = sp.GetComponent<TweenPosition>() as TweenPosition;
                if (tween != null)
                {
                    TweenPosition.Destroy(tween);
                }
                sp.Stand();
            }
            string clipName = "";
            if (this.IsWin)
            {
                clipName = "anim-victory";
            }
            else {
                clipName = "anim-failure";
            }
            this.ResultAnimSprite.gameObject.active = true;
            this.ResultAnimSprite.Play(clipName);
        }
    }

    private void _resultAnimComplete(tk2dAnimatedSprite sprite, int clipId)
    {
        this.ResultAnimSprite.gameObject.active = false;
        if (this.OnBattleFieldEnd != null) {
            this.OnBattleFieldEnd(this.IsWin, false);
        }
        this.DismissPanel();
    }

    /// <summary>
    /// 绘制小兵队伍
    /// </summary>
    private void _DrawWarrior()
    {
        IList<Warrior> allWarrior = this._battle.WarriorAll.Count == 0 ? 
            this._battle.WarriorLeft.Concat(this._battle.WarriorRight).ToList<Warrior>() 
            : this._battle.WarriorAll;
        for (int i = 0; i < allWarrior.Count; ++i) {
            Warrior w = allWarrior[i];
            WarriorSp sp = this._CloneSPFromTemplate();
            sp.transform.parent = this.BattleRootObj.transform;//直接就能添加到父节点上
            sp.Warrior = w;
            this._teamAll.Add(sp);
        }
    }

    /// <summary>
    /// 克隆生成一个动画节点
    /// </summary>
    /// <returns></returns>
    private WarriorSp _CloneSPFromTemplate()
    {
        if (this._warriorTemplate == null)
        {
            this._warriorTemplate = GBHelper.LoadObjFromPrefab("GuildBattle/WarriorTemplate") as GameObject;
            this._warriorTemplate.transform.position = new Vector3(-999f, -999f);//移动到屏幕外
        }
        WarriorSp sp = Instantiate(this._warriorTemplate.GetComponent<WarriorSp>()) as WarriorSp;
        return sp;
    }

    void SkipCombat() {
		if (this.OnBattleFieldEnd != null) {
            this.OnBattleFieldEnd(this.IsWin, true);
	    }
	    this.DismissPanel();
    }
}

public delegate void OnBattleFieldEnd(bool isWin, bool isSkip);
