using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TinyWar;
using CMNCMD;
using STCCMD;

namespace TinyWar {
    public class GBHelper
    {
        static bool _hasRegistered = false;
        public static void RegisterEvent() {
            if (!_hasRegistered) {
                Util.OnMsgGet += GBHelper._Print;
                GBHelper._hasRegistered = true;
            }
            
        }
        /// <summary>
        /// 从网格坐标转化到屏幕坐标
        /// </summary>
        /// <param name="pos">网格坐标</param>
        /// <returns></returns>
        public static Vector3 ConvertGridCoordsToScreen(Position pos) {
            float x = pos.x * Config.GRID_WIDTH + Config.GRID_ORIGIN[0] + Config.GRID_WIDTH / 2;
            float y = pos.y * Config.GRID_HEIGHT + Config.GRID_ORIGIN[1] + Config.GRID_HEIGHT / 2;
            float z = pos.y - Config.MAX_WARRIOR_SPACE;
            return new Vector3(x, y, z);
        }

        public static GameObject LoadObjFromPrefab(string prefabName)
        {
            return UnityEngine.Object.Instantiate(Resources.Load("Prefab/" + prefabName)) as GameObject;
        }

        public static GameObject LoadObjFromPrefab(string prefabName, Type type)
        {
            return UnityEngine.Object.Instantiate(Resources.Load("Prefab/" + prefabName, type)) as GameObject;
        }

        /// <summary>
        /// 从一个animation中获取相对应clip的第一个frame的spriteid和这个clip的id
        /// </summary>
        /// <param name="clipName">片段名字</param>
        /// <param name="animation">动画</param>
        /// <returns></returns>
        public static int[] GetClipIdAndSpriteIdFromClipName(string clipName, tk2dSpriteAnimation animation) {
            tk2dSpriteAnimationClip[] clips = animation.clips;
            int length = clips.Length;
            if (length > 0) {
                for (int i = 0; i < length; ++i) {
                    if (clips[i].name == clipName) {
                        return new int[]{i,clips[i].frames[0].spriteId};
                    }
                }
            }
            return new int[]{0,0};
        }

        public static void Print(params object[] objs){
            string result = "";
            for (int i = 0; i < objs.Length; ++i)
            {
                result += objs[i].ToString() + "\t\t\t";
            }
            Debug.Log(result);
        }

        static void _Print(string str)
        {
            GBHelper.Print(str);
        }

        /// <summary>
        /// 把老战斗中的战士类型转换成团队副本中的战士类型
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static WarriorType ConvertSoldierSort2WarriorType(SoldierSort t) {
            switch (t) { 
                case SoldierSort.GONGBING:
                    return WarriorType.Archers;
                case SoldierSort.JIANSHI:
                    return WarriorType.Swordsmen;
                case SoldierSort.QIANGBING:
                    return WarriorType.Lancers;
                case SoldierSort.QIBING:
                    return WarriorType.Cavalrys;
                case SoldierSort.QIXIE:
                    return WarriorType.Catapults;
                default:
                    return WarriorType.LeaderSwordsmen;
            }
        }

        /// <summary>
        /// 把团队副本中的战士类型，转换成老系统的战士类型
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static SoldierSort ConvertWarriorType2SoldierSort(WarriorType t) {
            switch (t) { 
                case WarriorType.Archers:
                    return SoldierSort.GONGBING;
                case WarriorType.Catapults:
                    return SoldierSort.QIXIE;
                case WarriorType.Cavalrys:
                    return SoldierSort.QIBING;
                case WarriorType.Lancers:
                    return SoldierSort.QIANGBING;
                case WarriorType.Swordsmen:
                    return SoldierSort.JIANSHI;
                default:
                    return SoldierSort.GONGBING;
            }
        }

        /// <summary>
        /// 把STC中的将领转化成战场中的将领数据
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static BattleWarrior ConvertHeroUnit2BattleWarrior(HeroUnit unit){
            BattleWarrior bw = new BattleWarrior { 
                ID = unit.nHeroID12,
                Name = DataConvert.BytesToStr(unit.szHeroName2),
                Force = unit.nProf4,
                TeamID = (int)unit.nCol6,
                Sequence = (int)unit.nRow5,
                ArmType = GBHelper.ConvertSoldierSort2WarriorType((SoldierSort)unit.nArmyType7)
            };
            return bw;
        }
        
        public static List<BattleWarrior> ConvertHeroUnit2BattleWarrior(HeroUnit[] heros) {
            List<BattleWarrior> result = new List<BattleWarrior>();
            if (heros != null)
            {
                for (int i = 0; i < heros.Length; ++i)
                {
                    result.Add(GBHelper.ConvertHeroUnit2BattleWarrior(heros[i]));
                }
            }
            return result;
        }
    }
}
