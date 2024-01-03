using System;
using UnityEngine;
using System.Collections.Generic;

public class GBDeployItemUI : PagingStorageSlot
{
    public UILabel HealthLabel;
    public UISprite HeroIcon;
    public UILabel HeroNameLabel;
    public UILabel HeroLevelLabel;
    public UISprite TroopIcon;
    public UILabel TroopLevelAndAmountLabel;
    public UILabel SeqLabel;
    public GBLordDeployWin lordDeployWin;

    int m_id = -1;
    bool hasChanged = false;

    void Awake() {
        MakeChildrenVisible(false);
        GBLordTroopManager.Instance.OnGBHeroStateChange += CampStateChange;
    }

    void OnDestroy() {
        GBLordTroopManager.Instance.OnGBHeroStateChange -= CampStateChange;
    }

    public void SetHeroItem(GBLordTroopHero item) {
        if (item != null)
        {
            MakeChildrenVisible(true);
            this.HeroIcon.spriteName = U3dCmn.GetHeroIconName((int)item.HeroModelID);
            //健康度
            int heroHealth = (int)item.HeroHealth;
            string colorStr = "";
            if (heroHealth > 80)
            {
                colorStr = "00FF00";
            }
            else if (heroHealth > 50)
            {
                colorStr = "FFEE00";
            }
            else
            {
                colorStr = "EE0000";
            }
            this.HealthLabel.text = string.Format(this.HealthLabel.text, colorStr, heroHealth);
            this.HeroNameLabel.text = item.HeroName;
            this.HeroLevelLabel.text = string.Format(this.HeroLevelLabel.text, item.HeroLevel);
            this.TroopIcon.spriteName = U3dCmn.GetTroopIconFromID((int)item.TroopID);
            this.TroopLevelAndAmountLabel.text = String.Format(this.TroopLevelAndAmountLabel.text, item.TroopLevel, item.TroopAmount);
            this.SeqLabel.enabled = false;
        }
        
    }

    public override int gid
    {
        get
        {
            return m_id;
        }

        set
        {

            if (m_id != value)
            {
                m_id = value;
                hasChanged = true;
            }
        }
    }

    void MakeChildrenVisible(bool visible) {
        Transform[] ts = gameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i < ts.Length; ++i) {
            Transform obj = ts[i];
            UIWidget widget = obj.GetComponent<UIWidget>();
            if (widget != null) { widget.enabled = visible; }
        }
    }

    public override void ResetItem()
    {
        hasChanged = true;
    }

    void LateUpdate() {
        if (hasChanged == true)
        {
            hasChanged = false;
            GBLordTroopHero hero = GBLordTroopManager.Instance.GetHero(m_id);
            this.SetHeroItem(hero);
        }
    }

    void OnItemSelect() {
        GBLordTroopHero item = GBLordTroopManager.Instance.GetHero(this.m_id);
        if (item == null) {
            return;
        }
        //如果没有出征则计算序号并设置出征，如果已经出征则取消
        if (!item.IsCampIn)
        {
            //if (item.TroopAmount == 0)
            //{
            //    int Tipset = BaizVariableScript.PICKING_GENERAL_NO_ARMY;
            //    string cc = U3dCmn.GetWarnErrTipFromMB(Tipset);
            //    string text = string.Format(cc, item.HeroName);
            //    PopTipDialog.instance.VoidSetText2(true, false, text);
            //}
            //else
            //{
                
            //}
            List<GBLordTroopHero> list = GBLordTroopManager.Instance.HeroList;
            int num = 0;
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].IsCampIn)
                {
                    num++;
                }
            }
            if (num == lordDeployWin.MaxHeroCapacity)
            {
                return;
            }
            GBLordTroopManager.Instance.ActiveHero(item);
        }
        else {
            GBLordTroopManager.Instance.DeactiveHero(item);
        }
    }

    public void CheckItem() {
        GBLordTroopHero item = GBLordTroopManager.Instance.GetHero(this.m_id);
        if (item == null)
        {
            return;
        }
        List<GBLordTroopHero> list = GBLordTroopManager.Instance.HeroList;
        int num = 0;
        for (int i = 0; i < list.Count; ++i)
        {
            if (list[i].IsCampIn)
            {
                num++;
            }
        }
        if (num == lordDeployWin.MaxHeroCapacity)
        {
            return;
        }
        GBLordTroopManager.Instance.ActiveHero(item);
    }

    public void UnCheckItem() {
        GBLordTroopHero item = GBLordTroopManager.Instance.GetHero(this.m_id);
        if (item == null)
        {
            return;
        }
        if (item.IsCampIn)
        {
            GBLordTroopManager.Instance.DeactiveHero(item);
        }
    }

    void CampStateChange(int heroSeq, int campSeq) {
        if (this.m_id == heroSeq) {
            if (campSeq == 0)
            {
                this.SeqLabel.enabled = false;
            }
            else {
                this.SeqLabel.text = string.Format("[{0}]", campSeq);
                this.SeqLabel.enabled = true;
            }
        }
    }
}
