using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GBHeroUI : MonoBehaviour {
    public UILabel HeroNameAndLevel;
    public UISprite HeroIcon;
    public UILabel HeroTroopLevelInfo;
    public UISprite HeroTroopIcon;

    public void SetHeroInfo(GBHero hero) {
        if (hero != null)
        {
            this.HeroNameAndLevel.enabled = true;
            this.HeroIcon.enabled = true;
            this.HeroTroopLevelInfo.enabled = true;
            this.HeroTroopIcon.enabled = true;

            this.HeroNameAndLevel.text = string.Format("{0} {1}", hero.name, hero.level);
            this.HeroIcon.spriteName = U3dCmn.GetHeroIconName(hero.heroId);
            this.HeroTroopLevelInfo.text = string.Format("Lv{0}x{1}", hero.level, hero.amount);
            this.HeroTroopIcon.spriteName = U3dCmn.GetTroopIconFromID(hero.troopId);
        }
        else {
            this.HeroNameAndLevel.enabled = false;
            this.HeroIcon.enabled = false;
            this.HeroTroopLevelInfo.enabled = false;
            this.HeroTroopIcon.enabled = false;
        }
    }
}
