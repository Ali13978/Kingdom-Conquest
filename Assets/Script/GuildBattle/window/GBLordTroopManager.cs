using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using STCCMD;
using CMNCMD;

 public class GBLordTroopManager
 {
     #region singleton
     private static GBLordTroopManager _instance;
     private GBLordTroopManager() { 
        this._list = new List<GBLordTroopHero>();
     }
     
     static GBLordTroopManager() {
         _instance = new GBLordTroopManager(); 
     }

     public static GBLordTroopManager Instance {
         get { return _instance; }
     }
     #endregion

     private List<GBLordTroopHero> _list;
     public event GBHeroStateChangeHandler OnGBHeroStateChange;
     /// <summary>
     /// 从JianglingManager中获取所有将领信息
     /// </summary>
     public void ApplyHero(HeroState state)
     {
         this._list.Clear();
         Hashtable jlMap = JiangLingManager.MyHeroMap;
         int i = 0;
         foreach (DictionaryEntry de in jlMap)
         {
             HireHero hero = (HireHero)de.Value;
             if (hero.nArmyNum11 == 0)
             {
                 continue;
             }
             if (hero.nStatus14 == (int)HeroState.NORMAL  || hero.nStatus14 == (int)state)
             {
                 GBLordTroopHero item = new GBLordTroopHero();
                 item.HeroModelID = (uint)hero.nModel12;
                 item.HeroID = hero.nHeroID1;
                 item.HeroHealth = (uint)hero.nHealthState21;
                 item.TroopLevel = (uint)hero.nArmyLevel10;
                 item.TroopAmount = (uint)hero.nArmyNum11;
                 item.HeroLevel = (uint)hero.nLevel19;
                 item.TroopID = (uint)hero.nArmyType9;
                 item.HeroForce = (uint)hero.nProf20;
                 item.HeroName = DataConvert.BytesToStr(hero.szName3);
                 item.IsCampIn = false;
                 _list.Add(item);
             }
         }

         this._list.Sort(this.Compare);
         for (int j = 0; j < this._list.Count; ++j) {
             this._list[j].HeroSeq = (uint)j;
         }
     }

     /// <summary>
     /// 所有将领
     /// </summary>
     public List<GBLordTroopHero> HeroList {
         get { return this._list; }
     }

     /// <summary>
     /// 单个将领
     /// </summary>
     /// <param name="num">将领序号</param>
     /// <returns></returns>
     public GBLordTroopHero GetHero(int num) {
         if (this._list.Count == 0 || num > this._list.Count - 1) {
             return null;
         }
         return this._list[num];
     }

     public void ActiveHero(GBLordTroopHero hero) {
         int num = 0;
         for (int i = 0; i < this._list.Count; ++i) {
             if (this._list[i].IsCampIn) {
                 num++;
             }
         }
         hero.IsCampIn = true;
         hero.CampSeq = (uint)(num + 1);
         if (this.OnGBHeroStateChange != null) { 
            OnGBHeroStateChange((int)hero.HeroSeq, (int)hero.CampSeq);
         }
     }

     public void DeactiveHero(GBLordTroopHero hero){
         hero.IsCampIn = false;

         for (int i = 0; i < this._list.Count; ++i) {
             GBLordTroopHero cur = this._list[i];
             if (cur.IsCampIn && cur.CampSeq > hero.CampSeq) {
                 cur.CampSeq--;
                 if (this.OnGBHeroStateChange != null) { 
                    OnGBHeroStateChange((int)cur.HeroSeq, (int)cur.CampSeq);
                 }
             }
         }
         
         hero.CampSeq = 0;
         if (this.OnGBHeroStateChange != null) { 
            OnGBHeroStateChange((int)hero.HeroSeq, (int)hero.CampSeq);
         }
     }

     private int Compare(GBLordTroopHero a, GBLordTroopHero b) {
         if (a.HeroID > b.HeroID) {
             return 1;
         }
         else if (a.HeroID < b.HeroID) {
             return -1;
         }
         return 0;
     }
 }

public delegate void GBHeroStateChangeHandler(int heroSeq, int campSeq);
