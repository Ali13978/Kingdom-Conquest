using System;
using UnityEngine;
using System.Collections;
using TinyWar;

public class ArrowStyle : WeaponStyle {
    public tk2dAnimatedSprite ArrowSprite;

    private TweenPosition _tween;

    public override void Attack(Warrior enemy) {
        this._packDefaultTransform();
        //由于右边武器的父元素已经做过角度旋转，所以子元素的坐标还是和左边一致
        float distX = Math.Abs(GBHelper.ConvertGridCoordsToScreen(enemy.AimPos).x - this.gameObject.transform.parent.localPosition.x);
        Vector3 pos = new Vector3(distX, this.gameObject.transform.localPosition.y, this.gameObject.transform.localPosition.z);
        float ratio = distX / Config.GRID_WIDTH;
        _tween = TweenPosition.Begin(this.gameObject, Config.WEAPON_MOVE_DURATION * ratio, pos);
        _tween.onFinished = this._onTweenFinished;
        this.ArrowSprite.Play(this.ClipName);
    }

    private void _onTweenFinished(UITweener tween)
    {
        this._packDefaultTransform();
        this.gameObject.SetActiveRecursively(false);
        this.OnAttackCompleteHandle();
    }
    //把敌兵的弓箭位置调整一下
    private void _packDefaultTransform()
    {
        if (this.Owner.Face == Direction.Left)
        {
            this.gameObject.transform.localPosition = new Vector3(0, this.gameObject.transform.localPosition.y, 1);
        }
        else
        {
            this.gameObject.transform.localPosition = new Vector3(0, this.gameObject.transform.localPosition.y, -1);
        }
        //this.gameObject.transform.localPosition = new Vector3(0, this.gameObject.transform.localPosition.y, 0);
    }
}
