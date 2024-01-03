using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CMNCMD;
using STCCMD;

public class GBLevelItemUI : MonoBehaviour {
    public List<int> tt;
    public tk2dBaseSprite BackgroundImg;
    public UILabel StageIDLabel;
    public UISlicedSprite StageSprite;
    public tk2dAnimatedSprite BattleAnimSprite;
    public tk2dAnimatedSprite BattleNextSprite;
    public GBLevelWin GBLevelWin;

    public enum GuardState { 
        Passed = 0, //已经通过 
        Next,       //即将攻打 
        InBattle,   //正在攻打 
        Normal      //尚未攻打 
    }
    private GuardState _state;
    private bool _isInBattle;
    private bool _isNextStage;
    private MY_GUILDBATTLE_STATUS _status = GBStatusManager.Instance.MyStatus;

    public void SetStageInfo(int stageID) {
        if (stageID % 10 == 0) {
            this.BackgroundImg.spriteId = this.BackgroundImg.GetSpriteIdByName("nzbz-da");
        }
        StageIDLabel.text = stageID.ToString();
        this.State = GuardState.Normal;
    }

    public GuardState State {
        get { return this._state; }
        set {
            this._state = value;
            switch (value) { 
                case GuardState.Normal:
                    BattleAnimSprite.gameObject.SetActiveRecursively(false);
                    BattleNextSprite.gameObject.SetActiveRecursively(false);
                    this.BackgroundImg.spriteId = this.BackgroundImg.GetSpriteIdByName("nzbz-xiao");
                    this.StageSprite.color = new Color(0, 255, 255);
                break;
                case GuardState.Passed:
                    BattleAnimSprite.gameObject.SetActiveRecursively(false);
                    BattleNextSprite.gameObject.SetActiveRecursively(false);
                    this.BackgroundImg.spriteId = this.BackgroundImg.GetSpriteIdByName("nzbz-xiao-gray");
                    this.StageSprite.color = Color.gray;
                break;
                case GuardState.Next:
                    BattleNextSprite.gameObject.SetActiveRecursively(true);
                    BattleAnimSprite.gameObject.SetActiveRecursively(false);
                    this.BackgroundImg.spriteId = this.BackgroundImg.GetSpriteIdByName("nzbz-xiao");
                    BattleNextSprite.Play();
                    this.StageSprite.color = Color.green;
                break;
                case GuardState.InBattle:
                    BattleNextSprite.gameObject.SetActiveRecursively(false);
                    BattleAnimSprite.gameObject.SetActiveRecursively(true);
                    this.BackgroundImg.spriteId = this.BackgroundImg.GetSpriteIdByName("nzbz-xiao");
                    BattleAnimSprite.Play();
                    this.StageSprite.color = Color.green;
                break;
            }
        }
    }

    void OnItemClick() {
        switch (this._state) { 
            case GuardState.Passed:
            case GuardState.Normal:
            case GuardState.InBattle:
                break;
            case GuardState.Next:
                GBLevelWin.StartManualCombat();
                break;
        }
    }
}
