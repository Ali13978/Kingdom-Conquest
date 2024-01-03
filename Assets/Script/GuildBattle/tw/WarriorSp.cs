using System;
using UnityEngine;
using TinyWar;

public class WarriorSp : IgnoreTimeScale {

    public tk2dAnimatedSprite sprite;
    public UILabel debugLabel;
    public UISlider bloodBar;
    [SerializeField] private AudioClip walkAudio;
    [SerializeField] private AudioClip AttackSound;

    private WeaponStyle _weaponSprite;//弓箭和投石器的武器精灵
    private Warrior _enemy;
    private Warrior _warrior;
    private AudioSource audioSource;
    //站立姿势的id，其实就是Move动画的第一帧id
    private int _standSpriteId;
    //满血量
    private float _maxhp;

    private void Start() {
        this.sprite.animationCompleteDelegate = this._animationComplete;
        this.sprite.animationEventDelegate = this._animationFrameEvent;
        this.debugLabel.transform.parent = gameObject.transform;
        this.debugLabel.transform.localRotation = new Quaternion();
        GetComponent<TweenPosition>().duration = UnityEngine.Random.Range(1.3f, 3f);
    }

    public Warrior Warrior {
        get { return this._warrior; }
        set {
            this._warrior = value;
            this._warrior.OnDie += this._warriorOnDie;
            this._warrior.OnAttackBegin += this._warriorOnAttackBegin;
            this._warrior.OnFaceChanged += this._warriorOnFaceChanged;
            this._warrior.OnMove += this._warriorOnMove;
            this._warrior.OnStandBy += this._warriorOnStandBy;
            this._warrior.OnHPChanged += this._warriorOnHPChanged;
            this.transform.localPosition = GBHelper.ConvertGridCoordsToScreen(value.Pos);
            this.transform.localScale = new Vector3(1, 1, 1);
            this._setAnimation();
            this._attachWeaponStyle();
            this.Stand();
            this._setLocationTransform();
            this._maxhp = this._warrior.HP;

            if (Config.SHOW_DEBUG_CONTENT)
            {
                this.debugLabel.text = this._warrior.Name.ToString();
            }
        }
    }

    

    /// <summary>
    /// 直接设置成站立的姿势
    /// </summary>
    public void Stand() {
        this.sprite.Stop();
        this.sprite.SwitchCollectionAndSprite(this.sprite.anim.clips[0].frames[0].spriteCollection, this._standSpriteId);
    }

    #region 设置精灵的初始位置、动画和武器效果函数
    private void _setAnimation() {
        //设置站立动画，直接展示move动画的第一帧即可
        string warriorType = this._warrior.ArmType.ToString();
        tk2dSpriteAnimation animation = Resources.Load("Animation/" + warriorType, typeof(tk2dSpriteAnimation)) as tk2dSpriteAnimation;
        this.sprite.anim = animation;
        string clipName = "MoveA";
        if (this._warrior.Tag == TagType.Right) {
            this.gameObject.transform.Rotate(new Vector3(0, 1), 180f);
            this.bloodBar.transform.Rotate(new Vector3(0, 1), 180f);
            this.bloodBar.transform.localPosition = new Vector3(-this.bloodBar.transform.localPosition.x, this.bloodBar.transform.localPosition.y);
            clipName = "MoveB";
        }
        int[] cidsid =  GBHelper.GetClipIdAndSpriteIdFromClipName(clipName, animation);
        //展示红色或者蓝色兵种
        this._standSpriteId = cidsid[1];
    }

    private void _setLocationTransform() { 
        int type = (int)this._warrior.ArmType;
        TagType tag = this._warrior.Tag;
        float[] param = Config.WARRIOR_DEFAULT_TRANSFORM[type];
        float posx,posy;
        float scalex,scaley;
        if(tag == TagType.Left){
            posx = param[0];
            posy = param[1];
            scalex = param[2];
            scaley = param[3];
        }
        else{
            posx = param[4];
            posy = param[5];
            scalex = param[6];
            scaley = param[7];
        }
        Transform transform = this.sprite.transform;
        transform.localPosition = new Vector3(posx, posy);
        transform.localScale = new Vector3(scalex * transform.localScale.x, scaley * transform.localScale.y);
    }
    
    /// <summary>
    /// 设置攻击动画，现在只对远程攻击有效
    /// </summary>
    private void _attachWeaponStyle()
    {
        if (this._warrior.AtkDistance > 1 && Config.SHOW_WEAPON_EFFECT) {//远程职业
            this._weaponSprite = WarriorSp._InstantiateWeaponStyle(this._warrior);
            this._weaponSprite.transform.parent = this.gameObject.transform;
            this._weaponSprite.transform.localScale = new Vector3(80, 80, 0);
            this._weaponSprite.transform.localPosition = new Vector3(0, 4, 0);
            this._weaponSprite.transform.localRotation = new Quaternion(0, 0, 0, 0);
            this._weaponSprite.gameObject.SetActiveRecursively(false);
            this._weaponSprite.OnAttackComplete += this._weaponSpriteOnAttackComplete;
        }
    }

    #endregion

    #region 勇士行为回调
    private void _warriorOnHPChanged(float hp) {
        if (hp <= 0) {
            GameObject.Destroy(this.bloodBar.gameObject);
        }
        this.bloodBar.sliderValue = hp / this._maxhp;
    }

    private void _warriorOnStandBy(){
        this.sprite.Play("Move" + (this._warrior.Tag == TagType.Left ? "A" : "B"));
    }

    private void _warriorOnDie() {
        this.sprite.Play("Die");
    }

    private void _warriorOnAttackBegin(AttackEventArgs args) {
        string clipName = "Attack" + (this._warrior.Tag == TagType.Left ? "A" : "B");
        //远程类攻击动画结束以后，增加投石和弓箭飞出效果
        this.sprite.Play(clipName);
        this._enemy = args.BeAttacked;
    }

    private void _warriorOnFaceChanged(Direction args) {
        float curRotationY = this.gameObject.transform.localRotation.y;
        this.gameObject.transform.localRotation = new Quaternion(0, curRotationY == 0 ? 180f : 0f, 0, 0);
        this.bloodBar.transform.Rotate(new Vector3(0, 1), 180f);
        this.bloodBar.transform.localPosition = new Vector3(-this.bloodBar.transform.localPosition.x, this.bloodBar.transform.localPosition.y);
    }

    private void _warriorOnMove(MoveEventArgs args) {
        this.sprite.Stop();
        Vector3 to = GBHelper.ConvertGridCoordsToScreen(args.To);
        TweenPosition tween = TweenPosition.Begin(this.gameObject, Config.WARRIOR_MOVE_DURATION, to);
        tween.onFinished = this._onMoveFinished;
        string clipName = "Move" + (this._warrior.Tag == TagType.Left ? "A" : "B");
        this.sprite.Play(clipName);
    }
    #endregion

    #region 精灵动画回调函数
    private void _animationComplete(tk2dAnimatedSprite sprite, int clipId)
    {
        string clipName = this.sprite.anim.clips[clipId].name.ToLower();
        switch (clipName)
        {
            case "die":
                GameObject.Destroy(this.gameObject);
                break;
            case "attacka":
            case "attackb":
                this.Stand();
                break;
        }
    }

    private void _animationFrameEvent(tk2dAnimatedSprite sprite, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame, int frameNum) {
        //远程兵种等待远程动画播放到特定帧再播放武器飞出动画
        switch (frame.eventInfo) { 
            case "OnArrowBegin":
            case "OnRockBegin":
                if (this._enemy.isAlive)
                {
                    this._weaponSprite.gameObject.SetActiveRecursively(true);
                    this._weaponSprite.Attack(this._enemy);
                }
                break;
        }
    }

    private void _onMoveFinished (UITweener tween) {
    }

    private void _weaponSpriteOnAttackComplete() {
        
    }
    #endregion

    /// <summary>
    /// 初始化武器效果类，主要是针对远程武器有效
    /// </summary>
    /// <param name="warrior"></param>
    /// <returns></returns>
    private static WeaponStyle _InstantiateWeaponStyle(Warrior warrior){
        string weaponPrefabName = "";
        switch(warrior.ArmType){
            case WarriorType.Archers:
                weaponPrefabName = "ArrowTemplate";
                break;
            case WarriorType.Catapults:
                weaponPrefabName = "RockTemplate";
                break;
        }
        WeaponStyle weaponStyle =  Instantiate(Resources.Load("Prefab/GuildBattle/" + weaponPrefabName, typeof(WeaponStyle))) as WeaponStyle;
        weaponStyle.ClipName = "Weapon";
        weaponStyle.Owner = warrior;
        return weaponStyle;
    }
}
