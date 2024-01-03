using System;
using UnityEngine;
using System.Collections;
using TinyWar;

public class RockStyle : WeaponStyle
{
    public tk2dAnimatedSprite RockSprite;

    private TweenPosition _tween;
    private float _defaultY = 10f;

    public override void Attack(Warrior enemy)
    {
        this._packDefaultTransform();
        this.gameObject.transform.localScale = new Vector3(100, 100);
        float x = Math.Abs(GBHelper.ConvertGridCoordsToScreen(enemy.AimPos).x - this.gameObject.transform.parent.localPosition.x);
        Vector3 pos = new Vector3(x, this._defaultY, this.gameObject.transform.localPosition.z);
        float ratio = x / Config.GRID_WIDTH;
        _tween = TweenPosition.Begin(this.gameObject, Config.WEAPON_MOVE_DURATION * ratio, pos);
        _tween.onFinished = _onTweenFinished;
        this.RockSprite.animationCompleteDelegate = _onAttackFinished;
    }

    private void _onTweenFinished(UITweener tween)
    {
        this.RockSprite.Play(this.ClipName);
        //this.gameObject.SetActiveRecursively(false);
        this.OnAttackCompleteHandle();
    }

    private void _onAttackFinished(tk2dAnimatedSprite sprite, int clipId)
    {
        int[] cidsid = GBHelper.GetClipIdAndSpriteIdFromClipName(this.ClipName, this.RockSprite.anim);
        int spriteId = cidsid[1];
        int cId = cidsid[0];
        this.RockSprite.SwitchCollectionAndSprite(this.RockSprite.anim.clips[cId].frames[0].spriteCollection, spriteId);
        this._packDefaultTransform();
        this.gameObject.SetActiveRecursively(false);
    }

    private void _packDefaultTransform() { 
        //朝左z轴为+数，朝右z轴为-数
        if (this.Owner.Face == Direction.Left)
        {
            this.gameObject.transform.localPosition = new Vector3(0, this._defaultY, 1);
        }
        else
        {
            this.gameObject.transform.localPosition = new Vector3(0, this._defaultY, -1);
        }
    }
}
